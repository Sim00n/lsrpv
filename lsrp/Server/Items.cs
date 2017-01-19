using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTANetworkServer;
using GTANetworkShared;
using Insight.Database;

class Items : Script
{
	private static Items instance = null;
	private static List<Item> items;
	private static List<GTANetworkServer.Object> dropped_items;

	public Items()
	{
		if (Items.instance != null)
		{
			return;
		}
		instance = this;
		items = new List<Item>();
		dropped_items = new List<GTANetworkServer.Object>();

		API.onClientEventTrigger += lsrp_OnClientEventTrigger;
	}

	public static Items getInstance()
	{
		if (Items.instance == null)
		{
			Items.instance = new Items();

		}
		return Items.instance;
	}

	public void Add(Item item)
	{
		if (this.isLoaded(item))
			return;

		foreach(Item it in this.GetAllItems())
		{
			if(it.iid == item.iid)
			{
				return;
			}
		}

		items.Add(item);
	}

	public void Remove(Item item)
	{
		items.Remove(item);
	}

	public bool isLoaded(Item item)
	{
		return items.Contains(item);
	}

	public List<Item> GetAllItems()
	{
		return Items.items;
	}

	public List<Item> GetAllPlayerItems(Client player)
	{
		List<Item> item_list = new List<Item>();

		foreach(Item item in items)
		{
			API.consoleOutput("Going through " + item.name);
			if(item.game_owner == player)
			{
				API.consoleOutput("^- matches");
				item_list.Add(item);
			}
		}

		return item_list;
	}

	public void LoadPlayerItems(Client player)
	{
		Database.characters character = API.getEntityData(player, "char");
		string str = String.Format("SELECT * FROM items WHERE place = '{0}' AND owner = '{1}'", (int) Config.OWNERS.PLAYER, character.cid);
		IList<Database.items> loaded_items = Database.getInstance().db().QuerySql<Database.items>(str);

		if (loaded_items.Count > 0)
		{
			foreach (Database.items it in loaded_items)
			{
				Item new_item = new Item(it.iid, it.name, player, (Item.TYPE)it.type, (Config.OWNERS)it.place, it.owner, it.int1, it.int2, it.int3, it.str1, it.str2, it.str3);
				this.Add(new_item);
			}
		}
	}

	public Item LoadSingleItem(Client player, int iid)
	{
		Database.characters character = API.getEntityData(player, "char");
		IList<Database.items> loaded_items = Database.getInstance().db().QuerySql<Database.items>(String.Format("SELECT * FROM items WHERE place = '{0}' AND owner = '{1}' AND iid = '{2}'", (int)Config.OWNERS.PLAYER, character.cid, iid));

		if (loaded_items.Count == 1)
		{
			foreach (Database.items it in loaded_items)
			{
				Item new_item = new Item(it.iid, it.name, player, (Item.TYPE)it.type, (Config.OWNERS)it.place, it.owner, it.int1, it.int2, it.int3, it.str1, it.str2, it.str3);
				this.Add(new_item);
				return new_item;
			}
		}
		return null;
	}

	public Item FindItemByID(int iid)
	{
		foreach(Item it in GetAllItems())
		{
			if(it.iid == iid)
			{
				return it;
			}
		}

		return null;
	}

	public bool DoesPlayerHaveItemType(Client player, Item.TYPE type)
	{
		foreach (Item item in GetAllPlayerItems(player))
		{
			if(item.type == type)
			{
				return true;
			}
		}

		return false;
	}

	public List<ItemTransportObject> ListNearbyItems(Client player)
	{
		//Database.characters character = API.getEntityData(player, "char");
		Vector3 pos = player.position;
		IList<Database.items> found_items = Database.getInstance().db().QuerySql<Database.items>(String.Format("SELECT * FROM items WHERE place = '{0}' AND posx > '{1}' AND posy > '{2}' AND posz > '{3}' AND posx < '{4}' AND posy < '{5}' AND posz < '{6}' ", (int)Config.OWNERS.WORLD, pos.X - 2.5F, pos.Y - 2.5F, pos.Z - 2.5F, pos.X + 2.5, pos.Y + 2.5F, pos.Z + 2.5f));

		List<ItemTransportObject> itos = new List<ItemTransportObject>();

		if (found_items.Count > 0)
		{
			foreach (Database.items it in found_items)
			{
				ItemTransportObject ito = new ItemTransportObject();
				ito.iid = it.iid;
				ito.name = it.name;
				itos.Add(ito);
			}
		}

		return itos;
	}

	/** 
	 * TODO: Test if we can remove items in the middle of the loop...
	 */
	public void UnloadPlayerItems(Client player)
	{
		foreach(Item it in GetAllItems())
		{
			if(it.game_owner == player)
			{
				this.Remove(it);
			}
		}
	}

	public void lsrp_OnClientEventTrigger(Client player, string eventName, params object[] arguments)
	{
		if(eventName == "lsrp_use_item")
		{
			int iid = Convert.ToInt32(arguments[0]);

			// -1 means they clicked pick up items
			if (iid == -1)
			{
				List<ItemTransportObject> items_nearby = Items.getInstance().ListNearbyItems(player);
				API.triggerClientEvent(player, "lsrp_show_nearby_items", API.toJson(items_nearby));
				return;
			} else
			{
				Item item = FindItemByID(iid);
				if(item != null)
				{
					item.Use(player);
				}
			}
		}
		if (eventName == "lsrp_drop_item")
		{
			int iid = Convert.ToInt32(arguments[0]);
			Item item = FindItemByID(iid);
			if (item != null)
			{
				item.PutDown(player);
			}
		}
		if(eventName == "lsrp_pickup_item")
		{
			int iid = Convert.ToInt32(arguments[0]);
			if (iid >= 0)
			{
				PickUpItem(player, iid);
			}
		}
	}

	public void PickUpItem(Client player, int iid)
	{
		Database.characters character = API.getEntityData(player, "char");
		Database.getInstance().db().QuerySql(String.Format("UPDATE items SET place = '{1}', owner = '{2}' WHERE iid = '{0}'", iid, (int)Config.OWNERS.PLAYER, character.cid));

		Item dropped_item = LoadSingleItem(player, iid);
		if (dropped_item != null)
		{
			GTANetworkServer.Object item_object = Dropped_Find(player.position, Config.OBJECTS.Get(dropped_item.type).model);
			if (item_object != null)
			{
				Dropped_Remove(item_object);
				item_object.delete();
			}
		}
	}

	public void Dropped_Add(GTANetworkServer.Object obj)
	{
		if (dropped_items.Contains(obj))
			return;

		dropped_items.Add(obj);
	}

	public void Dropped_Remove(GTANetworkServer.Object obj)
	{
		dropped_items.Remove(obj);
	}

	public GTANetworkServer.Object Dropped_Find(Vector3 pos, int object_model)
	{
		foreach(GTANetworkServer.Object obj in dropped_items)
		{
			if(pos.DistanceTo(obj.position) <= 5F)
			{
				if(obj.model == object_model)
				{
					return obj;
				}
			}
		}
		return null;
	}
}

public class Item : Script
{
	public enum TYPE
	{
		WATCH		= 1,			// Watch
		FOOD		= 2,			// Food item (param 1: how much HP to regenerate)
		CIGARETTE	= 3,			// A pack of cigarettes (param 1: how many left in the pack)
		DICE		= 4,			// A playing dice (param 1: number of sides)
		CELLPHONE	= 5,			// Cellphone (param 1: phone number)
	};

	public int iid;
	public string name;

	public TYPE type;

	public Config.OWNERS owner_type;
	public int owner;
	public Client game_owner;

	public int int1, int2, int3;
	public string str1, str2, str3;
	public float posx, posy, posz;
	public int dimension;
		
	public Item(int iid, string name, Client game_owner, TYPE type, Config.OWNERS owner_type, int owner, int int1, int int2, int int3, string str1, string str2, string str3)
	{
		this.iid = iid;
		this.name = name;
		this.game_owner = game_owner;
		this.type = type;
		this.owner_type = owner_type;
		this.owner = owner;
		this.int1 = int1;
		this.int2 = int2;
		this.int3 = int3;
		this.str1 = str1;
		this.str2 = str2;
		this.str3 = str3;
		this.posx = 0.0F;
		this.posy = 0.0F;
		this.posz = 0.0F;
		this.dimension = 0;
	}

	public Item(int iid, string name, Client game_owner, TYPE type, Config.OWNERS owner_type, int owner)
	{
		this.iid = iid;
		this.name = name;
		this.game_owner = game_owner;
		this.type = type;
		this.owner_type = owner_type;
		this.owner = owner;
		this.int1 = 0;
		this.int2 = 0;
		this.int3 = 0;
		this.str1 = "";
		this.str2 = "";
		this.str3 = "";
		this.posx = 0.0F;
		this.posy = 0.0F;
		this.posz = 0.0F;
		this.dimension = 0;
	}

	public Item()
	{
		// nuugh
	}

	public void Use(Client player) {

		if (type == TYPE.WATCH)
		{
			Commands.PLAYER_ME(player, String.Format("spogląda na swój zegarek marki {0}.", name));
			Commands.SELF_DO(player, String.Format("Zegarek pokazuje {0}", DateTime.Now.ToString("hh:mm:ss")));
			return;
		}
		else if (type == TYPE.FOOD)
		{
			if (int1 <= 0)
			{
				Destroy();
				return;
			}

			Player.getInstance().addToPlayerHealth(player, int1);
			Commands.PLAYER_ME(player, String.Format(" spożywa {0}.", name));
			Destroy();
			return;

		}
		else if(type == TYPE.CIGARETTE)
		{
			// 
		}
		else if(type == TYPE.DICE)
		{
			if(int1 <= 0)
			{
				Destroy();
				return;
			}

			Random rand = new Random();
			int result = rand.Next(1, int1+1);
			Commands.PLAYER_ME(player, String.Format("rzucił kostką z {0} ściankami i wyrzucił {1}.", int1, result));
			return;
		}
		else if(type == TYPE.CELLPHONE)
		{
			// 
		}
	}

	public void Destroy()
	{
		Database.getInstance().db().QuerySql(String.Format("DELETE FROM items WHERE iid = '{0}'", iid));
		Items.getInstance().Remove(this);
	}

	public void PutDown(Client player)
	{
		//Database.characters character = API.getEntityData(player, "char");

		// Put the item down in the vehicle
		if(API.isPlayerInAnyVehicle(player))
		{

		} else
		{
			Vector3 pos = API.getEntityPosition(player);
			int dimension = API.getEntityDimension(player);
			Config.ITEM_OBJECT io = Config.OBJECTS.Get(type);

			this.posx = pos.X + io.xoff;
			this.posy = pos.Y + io.yoff;
			this.posz = pos.Z + io.zoff;
			this.dimension = dimension;

			Database.getInstance().db().QuerySql(String.Format("UPDATE items SET posx = '{1}', posy = '{2}', posz = '{3}', dimension = '{4}', place = '{5}' WHERE iid = '{0}'", iid, posx, posy, posz, dimension, (int)Config.OWNERS.WORLD));
			Items.getInstance().Remove(this);

			
			GTANetworkServer.Object dropped_object = API.createObject(io.model, new Vector3(posx, posy, posz), io.rot, dimension);
			Items.getInstance().Dropped_Add(dropped_object);
		}
	}
}

public class ItemTransportObject
{
	public int iid { get; set; }
	public string name { get; set; }
}
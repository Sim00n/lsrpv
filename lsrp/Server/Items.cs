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

	public Items()
	{
		if (Items.instance != null)
		{
			return;
		}
		instance = this;
		items = new List<Item>();

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
		API.consoleOutput("testing query: " + str);
		IList<Database.items> loaded_items = Database.getInstance().db().QuerySql<Database.items>(str);

		if (loaded_items.Count > 0)
		{
			API.consoleOutput("Found some items ... ");
			foreach (Database.items it in loaded_items)
			{
				Item new_item = new Item(it.iid, it.name, player, (Item.TYPE)it.type, (Config.OWNERS)it.place, it.owner, it.int1, it.int2, it.int3, it.str1, it.str2, it.str3);
				this.Add(new_item);
				API.consoleOutput("Loading he he " + new_item.name + " he he ");
			}
		}
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
			Item item = FindItemByID(iid);
			if(item != null)
			{
				item.Use(player);
			}
		}
	}
}

class Item : Script
{
	public enum TYPE
	{
		WATCH		= 1,			// Watch
		FOOD		= 2,			// Food item (param 1: how much HP to regenerate)
		CIGARETTE	= 3,			// A pack of cigarettes (param 1: how many left in the pack)
		DICE		= 4				// A playing dice (param 1: number of sides)
	};

	public int iid;
	public string name;

	public TYPE type;

	public Config.OWNERS owner_type;
	public int owner;
	public Client game_owner;

	public int int1, int2, int3;
	public string str1, str2, str3;
		
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
	}

	public Item()
	{

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

			int health = API.getPlayerHealth(player);
			health = Tools.clamp(health + int1, 0, 100);
			API.setPlayerHealth(player, health);

			Commands.PLAYER_ME(player, String.Format(" spożywa {0}.", name));
			Destroy();
			return;
		} else if(type == TYPE.CIGARETTE)
		{
			//
		} else if(type == TYPE.DICE)
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


		}

	}
}

public class ItemTransportObject
{
	public int iid { get; set; }
	public string name { get; set; }
}
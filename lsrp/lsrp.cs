using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTANetworkServer;
using GTANetworkShared;
using MySql.Data.MySqlClient;
using Insight.Database.Providers.MySql;
using Insight.Database;
using BCr = BCrypt.Net;
using System.Globalization;

public class LSRP : Script
{
	public LSRP()
	{
		/**
		 * Set up all the server-side callbacks.
		 */
		API.onResourceStart += lsrp_OnGamemodeInit;
		API.onClientEventTrigger += lsrp_OnClientEventTrigger;
		API.onChatMessage += lsrp_OnChatMessage;

		API.onPlayerConnected += lsrp_OnPlayerConnected;
		API.onPlayerDisconnected += lsrp_OnPlayerDisconnect;
		API.onPlayerFinishedDownload += lsrp_OnPlayerFinishedDownload;

		API.onPlayerRespawn += lsrp_OnPlayerRespawn;
	}

	public void lsrp_OnGamemodeInit()
	{
		Tools.getInstance().log("OnGamemodeInit: loading ...", Config.LOGS.INFO);
	}

	public void lsrp_OnPlayerConnected(Client player)
	{
		Tools.getInstance().log(player.name + " has connected.");

		API.clearPlayerTasks(player);
		API.freezePlayer(player, true);
		API.removeAllPlayerWeapons(player);
		API.setPlayerToSpectator(player);

		API.setEntityData(player, "logged_in", false);
		API.triggerClientEvent(player, "lsrp_loginscreen", "");
	}

	public void lsrp_OnPlayerDisconnect(Client player, string reason)
	{
		Database.characters char_info = API.getEntityData(player, "char");
		Vector3 pos = API.getEntityPosition(player);
		char_info.posx = pos.X;
		char_info.posy = pos.Y;
		char_info.posz = pos.Z;
		char_info.dimension = API.getEntityDimension(player);
		char_info.health = API.getPlayerHealth(player);
		char_info.save();
	}

	public void lsrp_OnPlayerRespawn(Client player)
	{
		Tools.getInstance().log(player.name + " has respawned.");
		if(API.getEntityData(player, "logged_in") == false)
		{
			API.triggerClientEvent(player, "lsrp_loginscreen", "");
		}
	}

	public void lsrp_OnPlayerFinishedDownload(Client player)
	{
		Tools.getInstance().log(player.name + " has finished dl and is spawning now.");
		API.triggerClientEvent(player, "lsrp_loginscreen", "");
	}

	public void lsrp_OnClientEventTrigger(Client player, string eventName, params object[] arguments)
	{
		Tools.getInstance().log("Jakis event nadchodzi! : " + eventName);
		if (eventName == "lsrp_signin")
		{
			string username = arguments[0].ToString();
			string password = arguments[1].ToString();
			int login_result = lsrp_PlayerLoginAttempt(player, username, password);
			if(login_result == 3)
			{
				API.triggerClientEvent(player, "lsrp_loginscreen", "Błędne dane logowania.");
			} else if(login_result == 1 || login_result == 0)
			{
				API.triggerClientEvent(player, "lsrp_loginscreen", "Taki użytkownik nie istnieje.");
			} else if(login_result == 2)
			{
				API.triggerClientEvent(player, "lsrp_hideloginscreen");
				int selection_status = SelectCharacter(player);
				if(selection_status == 0)
				{
					API.sendNotificationToPlayer(player, "Nie posiadasz żadnej postaci.", true);
					API.kickPlayer(player);
				}
			}
		} else if(eventName == "lsrp_characterselection")
		{
			int cid = Convert.ToInt32(arguments[0]);
			int player_global_id = API.getEntityData(player, "global_id");

			IList<Database.characters> chars_query = Database.getInstance().db().QuerySql<Database.characters>(String.Format("SELECT * FROM characters WHERE owner = '{0}' AND cid = '{1}'", player_global_id, cid));
			if(chars_query.Count <= 0)
			{
				API.sendChatMessageToPlayer(player, "Inside");
				SelectCharacter(player);
				return;
			}

			Database.characters c = chars_query[0];
			API.setEntityData(player, "char", c);
			API.sendChatMessageToPlayer(player, "Outside");
			UseCharacter(player);
			return;
		}
	}

	public void lsrp_OnChatMessage(Client player, string message, CancelEventArgs e)
	{
		Tools.getInstance().ProxDetector(player, 30, "E6E6E6", String.Format("{0} mówi: {1}", player.name, message));
		e.Cancel = true;
	}

	public int lsrp_PlayerLoginAttempt(Client player, string username, string password)
	{
		IList<Database.users> user_query = Database.getInstance().db().QuerySql<Database.users>(String.Format("SELECT * FROM users WHERE username = '{0}'", username));
		if(user_query.Count <= 0)
		{
			return 1;
		}

		Database.users u = user_query.First<Database.users>();
		if (u == null)
		{
			return 0;
		}

		if(BCr.BCrypt.Verify(password, u.hash))
		{
			API.setEntityData(player, "global_username", u.username);
			API.setEntityData(player, "global_id", u.id);
			API.setEntityData(player, "global_admin", u.admin);
			return 2;
		} else
		{
			return 3;
		}
	}

	public int SelectCharacter(Client player)
	{
		int player_global_id = API.getEntityData(player, "global_id");
		IList<Database.characters> chars_query = Database.getInstance().db().QuerySql<Database.characters>(String.Format("SELECT * FROM characters WHERE owner = '{0}'", player_global_id));
		if(chars_query.Count <= 0)
		{
			return 0;
		}

		API.setEntityData(player, "char_list", chars_query);
		API.triggerClientEvent(player, "lsrp_choosecharacter", API.toJson(chars_query));

		return 1;
	}

	public void UseCharacter(Client player)
	{
		API.freezePlayer(player, false);
		API.unspectatePlayer(player);
		API.setEntityData(player, "logged_in", true);

		Database.characters char_info = API.getEntityData(player, "char");
		API.setPlayerHealth(player, (int) char_info.health);
		API.setPlayerSkin(player, (GTANetworkServer.PedHash) char_info.skin);
		API.setPlayerName(player, char_info.name.Replace('_', ' '));
		API.setPlayerNametag(player, char_info.name.Replace('_', ' '));
		API.setEntityPosition(player, new Vector3(char_info.posx, char_info.posy, char_info.posz));
		API.setEntityDimension(player, char_info.dimension);

		Items.getInstance().LoadPlayerItems(player);

		API.sendChatMessageToPlayer(player, "~#FF00FF~Witaj ~g~na ~#FF0000~serwerze!");
	}
}
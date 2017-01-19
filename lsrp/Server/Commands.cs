using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTANetworkServer;
using GTANetworkShared;

class Commands : Script
{
	public Commands()
	{
		API.onChatCommand += lsrp_OnChatCommand;
	}

	/**
	 * **************************************************
	 * Debug commands
	 * **************************************************
	 */

	[Command("triger")]
	public void CMD_triger(Client player)
	{
		API.triggerClientEvent(player, "lsrp_loginscreen", "");
	}

	[Command("itemtest")]
	public void COMMAND_itemtest(Client player)
	{
		Items.getInstance().LoadPlayerItems(player);
		API.consoleOutput("Loading all player itesm ...");
	}

	[Command("500plus")]
	public void COMMAND_500plus(Client player)
	{
		Database.characters character = API.getEntityData(player, "char");
		API.sendNotificationToPlayer(player, "Na Twoje konto wpłynęła wypłata $500.", true);
		Player.getInstance().setBankMoney(player, character.bankmoney + Config.HOURLY_PAY);
	}

	[Command("a")]
	public void COMMAND_anim(Client player, string a, string b)
	{
		API.playPlayerAnimation(player, (int) Config.AnimationFlags.Loop, a, b);
		return;
	}
	// a_arrest_on_floor mp_arresting
	[Command("s")]
	public void COMMAND_stop(Client player)
	{
		API.stopPlayerAnimation(player);
		return;
	}

	[Command("o")]
	public void COMMAND_o(Client player, int id, float zoff, int r1, int r2, int r3)
	{
		Vector3 pos = player.position;
		pos.Z += zoff;
		API.createObject(id, pos, new Vector3(r1, r2, r3), 0);
		return;
	}


	/**
	 * **************************************************
	 * Chat commands
	 * **************************************************
	 */

	[Command("do", GreedyArg = true)]
	public void CHAT_do(Client player, string text)
	{
		PLAYER_DO(player, text);
		return;
	}

	[Command("b", GreedyArg = true)]
	public void CHAT_b(Client player, string text)
	{
		Tools.getInstance().ProxDetector(player, 30, Config.COLOR_B, String.Format("(({0}: {1}))", API.getEntityData(player, "global_username"), text));
		return;
	}

	[Command("c", GreedyArg = true)]
	public void CHAT_c(Client player, string text)
	{
		Tools.getInstance().ProxDetector(player, 15, Config.COLOR_C, String.Format("{0} (szepcze): {1}", player.name, text));
		return;
	}

	[Command("k", GreedyArg = true)]
	public void CHAT_k(Client player, string text)
	{
		Tools.getInstance().ProxDetector(player, 45, Config.COLOR_K, String.Format("{0} (krzyczy): {1}", player.name, text));
		return;
	}

	[Command("i", GreedyArg = true)]
	public void CHAT_i(Client player, string text)
	{
		if (API.getEntityData(player, "global_admin") <= 0)
		{
			Tools.getInstance().ChatError(player, "Nie masz dostępu do tej komendy.");
			return;
		}

		API.sendChatMessageToAll("~#" + Config.COLOR_I + "~", String.Format("{0}: {1}", API.getEntityData(player, "global_username"), text));
		return;
	}

	/**
	 * **************************************************
	 * Player commands
	 * **************************************************
	 */
		
	[Command("p")]
	public void COMMAND_p(Client player)
	{
		List<Item> player_items = Items.getInstance().GetAllPlayerItems(player);
		List<ItemTransportObject> itos = new List<ItemTransportObject>();
		foreach(Item it in player_items)
		{
			ItemTransportObject ito = new ItemTransportObject();
			ito.iid = it.iid;
			ito.name = it.name;
			itos.Add(ito);
		}
		API.triggerClientEvent(player, "lsrp_show_own_items", API.toJson(itos));
		return;
	}

	/**
	 * **************************************************
	 * Admin commands
	 * **************************************************
	 */
	[Command("bw", GreedyArg = true)]
	public void COMMAND_bw(Client player, string command)
	{
		// Make sure the player can access this command.
		if (!Player.getInstance().IsPlayerAdmin(player))
		{
			NoPermission(player);
			return;
		}

		string[] cmds = command.Split(' ');

		// If no parameters passed show command error.
		if(cmds.Length <= 0)
		{
			WrongCommandUse(player, "/bw [zdejmij/naloz]");
			return;
		}

		// Turning someone's BW off
		if(cmds[0] == "zdejmij")
		{
			if(cmds.Length < 3)
			{
				WrongCommandUse(player, "/bw [zdejmij] [imie i nazwisko]");
				return;
			}

			string player_name = Tools.Capitalize(cmds[1]) + ' ' + Tools.Capitalize(cmds[2]);
			API.sendChatMessageToPlayer(player, player_name);
			Client give_player = API.getPlayerFromName(player_name);

			if (give_player == null)
			{
				WrongCommandUse(player, "Ten gracz nie istnieje.");
				return;
			}

			Database.characters character = API.getEntityData(player, "char");
			character.bw = 1; // Let tasks handle actually stopping bw.
			Success(player, "BW zostało zdjęte.");
			return;

		} else if(cmds[0] == "naloz")
		{
			if (cmds.Length < 4)
			{
				WrongCommandUse(player, "/bw [zdejmij] [imie i nazwisko] [czas w minutach]");
				return;
			}

			string player_name = Tools.Capitalize(cmds[1]) + ' ' + Tools.Capitalize(cmds[2]);
			Client give_player = API.getPlayerFromName(player_name);

			if (give_player == null)
			{
				WrongCommandUse(player, "Ten gracz nie istnieje.");
				return;
			}

			int bw_time = Convert.ToInt32(cmds[3]);

			if (bw_time <= 0 || bw_time > 60)
			{
				WrongCommandUse(player, "BW może trwać od 1 minuty do 60 minut.");
				return;
			}

			Player.getInstance().SetPlayerBW(player, bw_time * 60);
			Success(player, "BW zostało nadane.");
			
		} else
		{
			WrongCommandUse(player, "/bw [zdejmij/naloz]");
		}
	}


	/**
	 * **************************************************
	 * Utils commands
	 * **************************************************
	 */

	[Command("pos")]
	public void COMMAND_pos(Client player, int x, int y, int z)
	{
		API.setEntityPosition(player, new Vector3(x, y, z));
	}
	
	[Command("getpos")]
	public void COMMAND_getpos(Client player)
	{
		Vector3 pos = API.getEntityPosition(player);
		API.sendChatMessageToPlayer(player, String.Format("Current Position: {0}, {1}, {2}", pos.X, pos.Y, pos.Z));
	}

	[Command("getvw")]
	public void COMMAND_getvw(Client player)
	{
		API.sendChatMessageToPlayer(player, String.Format("Aktualny świat to: {0}.", API.getEntityDimension(player)));
	}


	/**
	 * **************************************************
	 * Quick util functions to send DOs and MEs to players around the param player and to themselves.
	 * **************************************************
	 */
	public static void PLAYER_ME(Client player, string text)
	{
		Tools.getInstance().ProxDetector(player, 30, Config.COLOR_ME, String.Format("*** {0} {1} ***", player.name, text));
	}

	public static void PLAYER_DO(Client player, string text)
	{
		Tools.getInstance().ProxDetector(player, 30, Config.COLOR_DO, String.Format("** {1} ({0}) **", player.name, text));
	}

	public static void SELF_DO(Client player, string text)
	{
		Tools.getInstance().SelfMessage(player, Config.COLOR_DO, String.Format("** {0} **", text));
	}

	/**
	 * **************************************************
	 * Errors / warnings / bad param messages / etc.
	 * **************************************************
	 */
	public static void WrongCommandUse(Client player, string proper_use)
	{
		Tools.getInstance().SelfMessage(player, Config.COLOR_RED, String.Format("[Błąd]: {0}", proper_use));
	}
	public static void Success(Client player, string success_msg)
	{
		Tools.getInstance().SelfMessage(player, Config.COLOR_GREEN, String.Format("[Sukces]: {0}", success_msg));
	}
	public static void NoPermission(Client player)
	{
		Tools.getInstance().SelfMessage(player, Config.COLOR_RED, String.Format("[Błąd]: Nie posiadasz uprawnień do tej komendy."));
	}

	/**
	 * **************************************************
	 * This is a chat command handler but we'll only use
	 * it for /me as gtan devs fucked us with their own
	 * implementation.
	 * **************************************************
	 */
	public void lsrp_OnChatCommand(Client player, string command, CancelEventArgs e)
	{
		if(command.Split(' ')[0] == "/me")
		{
			PLAYER_ME(player, string.Join(" ", command.Split(' ').Skip(1)));
			e.Cancel = true;
		}
		return;
	}
}


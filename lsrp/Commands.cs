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

	[Command("triger")]
	public void CMD_triger(Client player)
	{
		API.triggerClientEvent(player, "lsrp_loginscreen", "");
	}
	
	[Command("do", GreedyArg = true)]
	public void CHAT_do(Client player, string text)
	{
		PLAYER_DO(player, text);
		return;
	}

	[Command("b", GreedyArg = true)]
	public void CHAT_b(Client player, string text)
	{
		Tools.getInstance().ProxDetector(player, 30, "AFAFAF", String.Format("(({0}: {1}))", API.getEntityData(player, "global_username"), text));
		return;
	}

	[Command("c", GreedyArg = true)]
	public void CHAT_c(Client player, string text)
	{
		Tools.getInstance().ProxDetector(player, 15, "E6E6E6", String.Format("{0} (szepcze): {1}", player.name, text));
		return;
	}

	[Command("k", GreedyArg = true)]
	public void CHAT_k(Client player, string text)
	{
		Tools.getInstance().ProxDetector(player, 45, "E6E6E6", String.Format("{0} (krzyczy): {1}", player.name, text));
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

		API.sendChatMessageToAll("~#FFFFFF~", String.Format("{0}: {1}", API.getEntityData(player, "global_username"), text));
		return;
	}

	[Command("itemtest")]
	public void COMMAND_itemtest(Client player)
	{
		Items.getInstance().LoadPlayerItems(player);
		API.consoleOutput("Loading all player itesm ...");
	}
	
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
		string items_string = API.toJson(itos);
		API.consoleOutput("Param: " + items_string);
		API.triggerClientEvent(player, "lsrp_show_own_items", items_string);
		return;
	}

	/**
	 * Ability to quickly send player actions.
	 */
	public static void PLAYER_ME(Client player, string text)
	{
		Tools.getInstance().ProxDetector(player, 30, "C2A2DA", String.Format("*** {0} {1} ***", player.name, text));
	}

	public static void PLAYER_DO(Client player, string text)
	{
		Tools.getInstance().ProxDetector(player, 30, "9A9CCD", String.Format("** {1} ({0}) **", player.name, text));
	}

	public static void SELF_DO(Client player, string text)
	{
		Tools.getInstance().SelfMessage(player, "9A9CCD", String.Format("** {0} **", text));
	}
	// **************************** //


	/**
	 * This is a chat command handler but we'll only use it for /me as gtan devs fucked us with their own implementation.
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


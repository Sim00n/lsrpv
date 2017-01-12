using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTANetworkServer;
using GTANetworkShared;

class Commands : Script
{
	Tools tools;
	public Commands()
	{
		tools = new Tools();
	}

	[Command("triger")]
	public void CMD_triger(Client player)
	{
		API.triggerClientEvent(player, "lsrp_loginscreen", "");
	}

	[Command("me")]
	public void CHAT_me(Client player, string text)
	{
		tools.ProxDetector(player, 30, "C2A2DA", String.Format("** {0} {1} **", player.name, text));
		return;
	}

	[Command("do")]
	public void CHAT_do(Client player, string text)
	{
		tools.ProxDetector(player, 30, "9A9CCD", String.Format("** {1} ({0}) **", player.name, text));
		return;
	}

	[Command("b")]
	public void CHAT_b(Client player, string text)
	{
		tools.ProxDetector(player, 30, "AFAFAF", String.Format("(({0}: {1}))", API.getEntityData(player, "global_username"), text));
		return;
	}

	[Command("c")]
	public void CHAT_c(Client player, string text)
	{
		tools.ProxDetector(player, 15, "E6E6E6", String.Format("{0} (szepcze): {1}", player.name, text));
		return;
	}

	[Command("k")]
	public void CHAT_k(Client player, string text)
	{
		tools.ProxDetector(player, 45, "E6E6E6", String.Format("{0} (krzyczy): {1}", player.name, text));
		return;
	}

	[Command("i")]
	public void CHAT_i(Client player, string text)
	{
		if (API.getEntityData(player, "global_admin") <= 0)
		{
			tools.ChatError(player, "Nie masz dostępu do tej komendy.");
			return;
		}

		API.sendChatMessageToAll("~#FFFFFF~", String.Format("{0}: {1}", API.getEntityData(player, "global_username"), text));
		return;
	}
}


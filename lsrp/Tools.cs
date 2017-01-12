using GTANetworkServer.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTANetworkServer;
using GTANetworkShared;

public class Tools : Script
{
	public void ChatError(Client player, string error)
	{
		API.sendChatMessageToPlayer(player, String.Format("~#FF0000~[Błąd]~#FFFFFF~: {0}", error));
		return;
	}

	public void ProxDetector(Client player, float radius, string color, string message)
	{
		foreach (Client p in API.getPlayersInRadiusOfPlayer(radius, player))
		{
			API.sendChatMessageToPlayer(p, String.Format("~#{0}~", color), message);
		}
	}

	public void log(string log_message, Config.LOGS log_type = Config.LOGS.DEBUG)
	{
		switch (log_type)
		{
			case Config.LOGS.DB:
				{
					API.consoleOutput("[DB] " + log_message);
					break;
				}
			case Config.LOGS.DEBUG:
				{
					API.consoleOutput("[DEBUG] " + log_message);
					break;
				}
			case Config.LOGS.INFO:
				{
					API.consoleOutput("[INFO] " + log_message);
					break;
				}
			default:
				{
					API.consoleOutput("[DEBUG] " + log_message);
					break;
				}
		}
	}
}
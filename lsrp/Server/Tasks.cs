using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GTANetworkServer;
using GTANetworkShared;

class Tasks : Script
{
	private DateTime counter_last, counter_second, counter_minute, counter_hour;

	public Tasks()
	{
		counter_last = counter_second = counter_minute = counter_hour = DateTime.Now;
		API.onUpdate += lsrp_OnUpdate;
	}

	public void lsrp_OnUpdate()
	{
		if (DateTime.Now.Subtract(counter_second).TotalSeconds >= 1)
		{
			counter_second = DateTime.Now;
			lsrp_OneSecondTimer();
		} 

		if(DateTime.Now.Subtract(counter_minute).TotalSeconds >= 59)
		{
			counter_minute = DateTime.Now;
			lsrp_OneMinuteTimer();
		}

		if(DateTime.Now.Subtract(counter_hour).TotalMinutes >= 59)
		{
			counter_hour = DateTime.Now;
			lsrp_OneHourTimer();
		}
	}

	public void lsrp_OneSecondTimer()
	{
		foreach (Client player in API.getAllPlayers())
		{
			// Ignore players that aren't logged in.
			if (API.getEntityData(player, "logged_in") == false)
			{
				continue;
			}

			// Get player's db structure.
			Database.characters character = API.getEntityData(player, "char");

			// Brutally Wounder timer.
			if(character.bw > 0)
			{
				API.sendChatMessageToPlayer(player, String.Format("Pozostało Ci {0} sekund", character.bw));
				character.bw--;
				if (character.bw == 0)
				{
					API.freezePlayer(player, false);
					Commands.SELF_DO(player, "Otrząsnąłeś się po ostatniej wpadce i powoli dochodzisz do siebie.");
				}
			}

			// Inform how many more minutes of BW status is left.
			if (character.bw > 0)
			{
				int minutes = (int)Math.Floor((float)(character.bw / 60));
				API.sendNotificationToPlayer(player, String.Format("Pozostało {0} minut BW.", minutes), false);
			}
		}
	}

	public void lsrp_OneMinuteTimer()
	{
		API.setTime(DateTime.Now.Hour, DateTime.Now.Minute);

		foreach (Client player in API.getAllPlayers())
		{
			// Ignore players that aren't logged in.
			if (API.getEntityData(player, "logged_in") == false)
			{
				continue;
			}

			// Get player's db structure
			Database.characters character = API.getEntityData(player, "char");

			// Increase player time on the server by 1 minute.
			character.minutes++;
			if(character.minutes == 60)
			{
				character.hours++;
				character.minutes = 0;
			}



		}
	}

	public void lsrp_OneHourTimer()
	{
		foreach(Client player in API.getAllPlayers())
		{
			// Ignore players that aren't logged in.
			if(API.getEntityData(player, "logged_in") == false)
			{
				continue;
			}

			// Get player's db structure
			Database.characters character = API.getEntityData(player, "char");

			// If the player has a watch, notify them of a full hour passing.
			if(Items.getInstance().DoesPlayerHaveItemType(player, Item.TYPE.WATCH)) {
				Commands.SELF_DO(player, String.Format("Twój zegarek zawibrował pokazując godzinę {0}:{1}.", DateTime.Now.Hour, DateTime.Now.Minute));
			}

		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GTANetworkServer;
using GTANetworkShared;

class Events : Script
{
	public static Events events = null;

	public Events()
	{
		API.onPlayerHealthChange += lsrp_OnPlayerHealthChange;
		API.onPlayerDeath += lsrp_OnPlayerDeath;
		API.onPlayerRespawn += lsrp_OnPlayerRespawn;

		Events.events = this;
	}

	public static Events getInstance()
	{
		if (Events.events == null)
		{
			Events.events = new Events();
		}
		return Events.events;
	}

	/**
	 * **************************************************
	 * GTAN event handlers.
	 * **************************************************
	 */

	public void lsrp_OnPlayerHealthChange(Client player, int oldHealth)
	{
		
	}

	public void lsrp_OnPlayerRespawn(Client player)
	{
		Tools.getInstance().log(player.name + " has respawned.");
		if (API.getEntityData(player, "logged_in") == false)
		{
			API.triggerClientEvent(player, "lsrp_loginscreen", "");
			return;
		}

		Database.characters character = API.getEntityData(player, "char");

		/**
		 * Respawn when Brutally Wounded
		 */
		if (character.bw > 0)
		{
			API.setEntityPosition(player, new Vector3(character.posx, character.posy, character.posz));
			API.setEntityDimension(player, character.dimension);
			API.freezePlayer(player, true);
			return;
		}

		/**
		 * Respawn when nothing is affecting the player.
		 */
		
			API.setEntityPosition(player, Config.SPAWN_POSITIONS[Tools.getInstance().rand.Next(Config.SPAWN_POSITIONS.Length)]);
			API.setEntityDimension(player, character.dimension);
			return;
		
	}



	/**
	 * **************************************************
	 * LS-RP Events generated from GTAN events.
	 * **************************************************
	 */

	public void lsrp_OnPlayerDeath(Client player, NetHandle entityKiller, int weapon)
	{
		Player.getInstance().SetPlayerBW(player, Config.BW_TIME);
	}
}
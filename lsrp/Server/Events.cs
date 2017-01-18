﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GTANetworkServer;
using GTANetworkShared;

class Events : Script
{
	public Events()
	{
		API.onPlayerHealthChange += lsrp_OnPlayerHealthChange;
		API.onPlayerDeath += lsrp_OnPlayerDeath;
		API.onPlayerRespawn += lsrp_OnPlayerRespawn;
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
		if (character.bw >= 0)
		{
			API.setEntityPosition(player, new Vector3(character.posx, character.posy, character.posz));
			API.freezePlayer(player, true);
			return;
		}

		/**
		 * Respawn when nothing is affecting the player.
		 */
		{
			API.setEntityPosition(player, Config.SPAWN_POSITIONS[Tools.getInstance().rand.Next(Config.SPAWN_POSITIONS.Length)]);
			return;
		}
	}



	/**
	 * **************************************************
	 * LS-RP Events generated from GTAN events.
	 * **************************************************
	 */

	public void lsrp_OnPlayerDeath(Client player, NetHandle entityKiller, int weapon)
	{
		Vector3 pos = API.getEntityPosition(player);
		Database.characters character = API.getEntityData(player, "char");
		character.posx = pos.X;
		character.posy = pos.Y;
		character.posz = pos.Z;
		character.bw = Config.BW_TIME;
		character.save();
	}
}
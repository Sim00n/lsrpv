using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTANetworkServer;
using GTANetworkShared;

class Player : Script
{
	public static Player player = null;

	public Player()
	{
		Player.player = this;
	}

	public static Player getInstance()
	{
		if(Player.player == null)
		{
			Player.player = new Player();
		}
		return Player.player;
	}

	public void setMoney(Client player, int money)
	{
		Database.characters character = API.getEntityData(player, "char");
		character.money = money;
		character.save();
		API.triggerClientEvent(player, "lsrp_update_hud", character.money, character.bankmoney);
		return;
	}

	public void setBankMoney(Client player, int bankmoney)
	{
		Database.characters character = API.getEntityData(player, "char");
		character.bankmoney = bankmoney;
		character.save();
		API.triggerClientEvent(player, "lsrp_update_hud", character.money, character.bankmoney);
		return;
	}

	/*public void SetPlayerBW(Client player, int duration)
	{
		if(duration <= 0)
		{
			return;
		}

		Database.characters character = API.getEntityData(player, "char");
		character.bw = duration;
		character.save();

		//API.

		return;
	}*/
}
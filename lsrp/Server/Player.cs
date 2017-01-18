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

	public bool IsPlayerAdmin(Client player)
	{
		return API.getEntityData(player, "global_admin") != 0;
	}

	public void SetPlayerBW(Client player, int duration)
	{
		if(duration <= 0)
		{
			return;
		}

		Database.characters character = API.getEntityData(player, "char");
		Vector3 pos = API.getEntityPosition(player);
		character.posx = pos.X;
		character.posy = pos.Y;
		character.posz = pos.Z;
		character.bw = duration;
		character.save();

		API.freezePlayer(player, true);
		return;
	}
}
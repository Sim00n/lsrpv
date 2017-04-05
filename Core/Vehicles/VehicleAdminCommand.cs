using System;
using System.Collections.Generic;
using System.Collections;

using GTANetworkServer;
using GTANetworkShared;

namespace Core
{
    class VehicleAdminCommand : Script
    {
        /// <summary>
        /// Command /v 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="uid"></param>
        /// 
        [Command("aveh", "~y~Użyj: ~w~/aveh [stworz]", Alias = "av", GreedyArg = true)]
        public void apojazdy(Client sender, string arg)
        {
            Player player = PlayerManager.Instance.findPlayerByHandle(sender);
            if (player == null || player.CurrentCharacter == null)
            {
                return;
            }

            if(arg == "stworz")
            {

            }

            
        }
    }
}

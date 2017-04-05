using System;
using System.Text;

using GTANetworkServer;

namespace Core {

    class Utils {


        // ----- [ Prox Detector - chat like SA-MP ] ----- ///

        public static void ProxDetector(float radius, Client player, string message, string col1, string col2, string col3, string col4, string col5)
        {
            var players = API.shared.getPlayersInRadiusOfPlayer(radius, player);

            foreach (Client c in players)
            {
                if (player.position.DistanceTo(c.position) <= radius / 16)
                {
                    API.shared.sendChatMessageToPlayer(c, col1, message);
          
                }
                else if (player.position.DistanceTo(c.position) <= radius / 8)
                {
                    API.shared.sendChatMessageToPlayer(c, col2, message);
                }
                else if (player.position.DistanceTo(c.position) <= radius / 4)
                {
                    API.shared.sendChatMessageToPlayer(c, col3, message);
                }
                else if (player.position.DistanceTo(c.position) <= radius / 2)
                {
                    API.shared.sendChatMessageToPlayer(c, col4, message);
                }
                else if (player.position.DistanceTo(c.position) <= radius)
                {
                    API.shared.sendChatMessageToPlayer(c, col5, message);
                }
            }
        }
    }
}

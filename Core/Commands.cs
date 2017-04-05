using System;
using System.Collections;
using System.Collections.Generic;

using GTANetworkServer;
using GTANetworkShared;

namespace Core {
    /// <summary>
    /// Sub-script used to handle commands.
    /// </summary>
    class Commands : Script {
        public const float LOCAL_CHAT_RADIUS = 10.0f;
        public const float WISPER_CHAT_RADIUS = 5.0f;
        public const float SCREAM_CHAT_RADIUS = 20.0f;

       // private VehicleManager vehicleManager = null;


        [Command("me", GreedyArg = true, Description = "Użyj: /me [akcja]")]
        public void MeCommand(Client sender, string text)
        {
            Player player = PlayerManager.Instance.findPlayerByHandle(sender);
            if (player == null || player.CurrentCharacter == null)
            {
                return;
            }

            Utils.ProxDetector(LOCAL_CHAT_RADIUS, sender, "** " + player.CurrentCharacter.GetFullName() + " " + text, "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~");
            
        }

        [Command("do", GreedyArg = true, Description = "Użyj: /do [akcja]")]
        public void doCommand(Client sender, string text)
        {
            Player player = PlayerManager.Instance.findPlayerByHandle(sender);
            if (player == null || player.CurrentCharacter == null)
            {
                return;
            }

            Utils.ProxDetector(LOCAL_CHAT_RADIUS, sender, "** " + text + " (( "+ player.CurrentCharacter.GetFullName() + " )) ", "~#2b0046~", "~#2b0046~", "~#2b0046~", "~#2b0046~", "~#2b0046~");
            

        }

       

        [Command("krzyk", "~y~Użyj: ~w~/krzyk [wiadomość]", Alias = "k", GreedyArg = true)]
        public void Scream(Client invoker, String message) {
            Player player = PlayerManager.Instance.findPlayerByHandle(invoker);
            if (player == null || player.CurrentCharacter == null) {
                return;
            }

            Utils.ProxDetector(SCREAM_CHAT_RADIUS, player, player.CurrentCharacter.GetFullName() + " krzyczy: " + message, "~#FFFFFF~", "~#C8C8C8~", "~#AAAAAA~", "~#8C8C8C~", "~#6E6E6E~");
        }

        [Command("szept", "~y~Użyj: ~w~/szept [wiadomość]", Alias = "s", GreedyArg = true)]
        public void Wisper(Client invoker, String message) {
            Player player = PlayerManager.Instance.findPlayerByHandle(invoker);
            if (player == null || player.CurrentCharacter == null) {
                return;
            }

            Utils.ProxDetector(WISPER_CHAT_RADIUS, player, player.CurrentCharacter.GetFullName() + " szepcze: " + message, "~#FFFFFF~", "~#C8C8C8~", "~#AAAAAA~", "~#8C8C8C~", "~#6E6E6E~");
        }

        [Command("ooc", "~y~Użyj: ~w~/ooc [wiadomość]", Alias = "b", GreedyArg = true)]
        public void OOC(Client invoker, String message) {
            Player player = PlayerManager.Instance.findPlayerByHandle(invoker);
            if (player == null || player.CurrentCharacter == null) {
                return;
            }

            Utils.ProxDetector(LOCAL_CHAT_RADIUS, player, "(( (GID: "+player.AccountId+") "+ player.CurrentCharacter.GetFullName() + ": " + message+ " ))", "~#FFFFFF~", "~#C8C8C8~", "~#AAAAAA~", "~#8C8C8C~", "~#6E6E6E~");
        }

        [Command("admin", "~y~Użyj: ~w~/ac [wiadomość]", Alias = "ac", GreedyArg = true)]
        public void ac(Client invoker, String message)
        {
            Player player = PlayerManager.Instance.findPlayerByHandle(invoker);
            if (player == null || player.CurrentCharacter == null)
            {
                return;
            }

            API.sendChatMessageToAll("~#1e5800~[[ " + player.CurrentCharacter.GetFullName() + " : " + message+" ]]");
        }

        [Command("getpos")]
        public void GetPos(Client invoker) {
            API.consoleOutput(invoker.position.ToString());
            API.consoleOutput(invoker.rotation.ToString());
        }

        [Command("tp")]
        public void TeleportPlayerToPlayerCommand(Client sender, Client target)
        {
            var pos = API.getEntityPosition(sender.handle);

            API.createParticleEffectOnPosition("scr_rcbarry1", "scr_alien_teleport", pos, new Vector3(), 1f);

            API.setEntityPosition(sender.handle, API.getEntityPosition(target.handle));
        }
        [Command("pos")]
        public void TeleportPlayerToPosCommand(Client sender, float x, float y, float z)
        {
            Vector3 posi = new Vector3(x, y, z);
            API.setEntityPosition(sender.handle, posi);
        }

        [Command("veh")]
        public void spawnVehicle(Client player, string veh, int col1, int col2)
        {
            Vector3 vehPos = API.getEntityPosition(player);
            Vector3 vehRot = API.getEntityRotation(player);
            VehicleHash myVehicle = API.vehicleNameToModel(veh);
            Vehicle veh2 = API.createVehicle(myVehicle, vehPos, vehRot, col1, col2); //Spawned vehicle is visible in all Dimensions
            API.setPlayerIntoVehicle(player, veh2, -1);
        }
        [Command("loadipl")]
        public void LoadIplCommand(Client sender, string ipl)
        {
            API.requestIpl(ipl);
            API.consoleOutput("LOADED IPL " + ipl);
            API.sendChatMessageToPlayer(sender, "Loaded IPL ~b~" + ipl + "~w~.");
        }

        [Command("removeipl")]
        public void RemoveIplCommand(Client sender, string ipl)
        {
            API.removeIpl(ipl);
            API.consoleOutput("REMOVED IPL " + ipl);
            API.sendChatMessageToPlayer(sender, "Removed IPL ~b~" + ipl + "~w~.");
        }
        [Command("anim")]
        public void anim(Client sender, string dict, string dict2)
        {

            //API.playPedAnimation(sender.handle, false, dict, dict2);
            API.playPlayerAnimation(sender, 0, dict, dict2);

        }
        [Command("skins")]
        public void selectskin(Client sender)
        {
            API.setEntityPosition(sender.handle, new Vector3(122.0416, 548.738, 180.4973));
            sender.freezePosition = true;
            API.triggerClientEvent(sender, "SELECT_SKIN");
        }

        [Command("vehman")]
        public void vehman(Client player, string veh)
        {
            Vector3 vehPos = API.getEntityPosition(player);
            Vector3 vehRot = API.getEntityRotation(player);
            VehicleHash myVehicle = API.vehicleNameToModel(veh);
            Vehicle veh2 = API.createVehicle(myVehicle, vehPos, vehRot, 0, 0); //Spawned vehicle is visible in all Dimensions
            API.setPlayerIntoVehicle(player, veh2, -1);
            //VehicleManager vehicle = VehicleData.Add()
            //vehicleManager.addVehicle(veh2);
            //API.consoleOutput("Test veh: " + veh2.handle);

        }

        [Command("pojazdy")]
        public void pojazdy(Client sender, int uid)
        {
            VehicleManager veh = VehicleData.getVehicleByUID(uid);

            API.setPlayerIntoVehicle(sender, veh.vehicle.handle, -1);
        }

        
    }
}

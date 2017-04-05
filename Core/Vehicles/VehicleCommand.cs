using System;
using System.Collections.Generic;
using System.Collections;

using GTANetworkServer;
using GTANetworkShared;

namespace Core
{
    class VehicleCommand : Script
    {
        /// <summary>
        /// Command /v 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="uid"></param>
        /// 
        [Command("vehicle", "~y~Użyj: ~w~/v [lista, z(amknij), silnik, lista, zaparkuj]", Alias = "v", GreedyArg = true)]
        public void pojazdy(Client sender, string arg)
        {
            Player player = PlayerManager.Instance.findPlayerByHandle(sender);
            if (player == null || player.CurrentCharacter == null)
            {
                API.sendChatMessageToPlayer(sender, "Musisz być zalogowany aby użyć tej komendy.");
                return;
            }

            // ----- [ Start/stop engine vehicle ]: ----- //

            if (arg == "silnik")
            {
                if (!(sender.vehicleSeat == -1))
                {
                    API.sendNotificationToPlayer(sender, "Aby użyć tej komendy musisz siedzieć w pojeździe jako kierowca."); 
                    return;
                }

                
                VehicleManager veh = VehicleData.getVehicleByHandle(sender.vehicle.handle);

                if(!(veh.data.owner == player.CurrentCharacter.getUID())) 
                {
                    API.sendNotificationToPlayer(sender, "Nie posiadasz kluczy do tego pojazdu.");
                    return;
                }

                if(API.getVehicleEngineStatus(veh.vehicle.handle) == true)
                {
                    API.setVehicleEngineStatus(veh.vehicle.handle, false);
                    veh.vehicle.engineStatus = false;
                }
                else
                {
                    API.setVehicleEngineStatus(veh.vehicle.handle, true);
                    veh.vehicle.engineStatus = true;
                }

                return;
            }

            // ----- [ Lock vehicle ]: ----- //

            if (arg == "z" || arg == "zamknij")
            {
                NetHandle vehnear = VehicleManager.GetClosestVehicle(sender, 3);
                VehicleManager veh = VehicleData.getVehicleByHandle(vehnear);

                if (vehnear.IsNull) return;

                if (!(veh.data.owner == player.CurrentCharacter.getUID()))
                {
                    API.sendNotificationToPlayer(sender, "Nie posiadasz kluczy do tego pojazdu.");
                    return;
                }

                if(API.getVehicleLocked(vehnear) == true)
                {
                    API.setVehicleLocked(vehnear, false);
                    API.sendNotificationToPlayer(player, "Pojazd został otwarty.");

                    Utils.ProxDetector(Commands.LOCAL_CHAT_RADIUS, sender, "* " + player.CurrentCharacter.GetFullName()+ " otwiera pojazd", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~");
            

                    API.playPlayerAnimation(sender, 0, "veh@std@rps@enter_exit", "d_locked");
                    return;
                }
                else
                {
                    API.setVehicleLocked(vehnear, true);
                    API.sendNotificationToPlayer(player, "Pojazd został zamknięty.");
                    Utils.ProxDetector(Commands.LOCAL_CHAT_RADIUS, sender, "* " + player.CurrentCharacter.GetFullName() + " zamyka pojazd", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~", "~#C2A2DA~");

                    API.playPlayerAnimation(sender, 0, "veh@std@rps@enter_exit", "d_locked");
                    return;
                }
            }

            // ----- [ Park vehicle ]: ----- //
            
            if (arg == "spawn" || arg == "zaparkuj")
            {
                if (!(sender.vehicleSeat == -1))
                {
                    API.sendNotificationToPlayer(sender, "Aby użyć tej komendy musisz siedzieć w pojeździe jako kierowca.");
                    return;
                }


                VehicleManager veh = VehicleData.getVehicleByHandle(sender.vehicle.handle);

                if (!(veh.data.owner == player.CurrentCharacter.getUID()))
                {
                    API.sendNotificationToPlayer(sender, "Ten pojazd nie należy do Ciebie.");
                    return;
                }

                
                bool res = veh.Save();
                if(res == true)
                {
                    API.sendNotificationToPlayer(sender, "Pojazd został zapisany w tej pozycji"); 
                }
                return;
            }

            if (arg == "lista")
            {
                ArrayList vehicleslist = Database.Context.Instance.getVehiclesByOwner(player.CurrentCharacter.getUID());
                
                if (vehicleslist.Count == 0)
                {
                    API.sendChatMessageToPlayer(sender, "Nie posiadasz żadnych pojazdów.");
                    return;
                }
                API.triggerClientEvent(sender, "VEHICLES_SHOW");
                
                
            }

            ArrayList vehicles = Database.Context.Instance.getVehiclesByOwner(player.CurrentCharacter.getUID());

            if (vehicles.Count == 0)
            {
                API.sendChatMessageToPlayer(sender, "Nie posiadasz żadnych pojazdów.");
                return;
            }


            API.sendChatMessageToPlayer(sender, "~g~| ----- [ Pojazdy ] ----- |");
            foreach (var veh in vehicles)
            {
                Database.Data.Vehicle VehicleData = (Database.Data.Vehicle)veh;
                API.sendChatMessageToPlayer(sender, string.Format("~r~(UID: {0})~w~ {1}", VehicleData.uid, (VehicleHash)VehicleData.model));
            }
            // New show
            //Send array with vehicles to Client 

            /// TO DO
            //API.triggerClientEvent(sender, "LOGIN_INCORRECT", "Nie znaleziono użytkownika o nazwie " + username);
        }
    }
}

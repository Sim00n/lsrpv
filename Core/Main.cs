using System;
using System.Collections.Generic;
using System.Collections;
using GTANetworkServer;
using GTANetworkShared;

namespace Core
{
    /// <summary>
    /// Main resource script.
    /// </summary>
    public class Main : Script, IDisposable
    {
        /// <summary>
        /// The instance of the player manager.
        /// </summary>
        private PlayerManager       playerManager = null;


        /// <summary>
        /// The vehicle Manager
        /// </summary>
        /// 
       // private VehicleManager vehicleManager = null;

        /// <summary>
        /// The database context.
        /// </summary>
        private Database.Context    db = null;

        /// <summary>
        /// The spawn point. (at the moment rotation is 0,0,0)
        /// </summary>
        private static Vector3 SpawnPoint = new Vector3(-1041.671, -2743.54, 21.3594);

        /// <summary>
        /// The main script constructor.
        /// </summary>
        public Main() {
            API.consoleOutput("Initializing RP Core");

            API.consoleOutput("Connecting to database..");

            db = new Database.Context();

            API.consoleOutput("Connected!");

            API.consoleOutput("Creating player manager..");

            playerManager = new PlayerManager();

            API.consoleOutput("Player manager created.");

            setupCallbacks();

            API.consoleOutput("RP Core started");

           VehicleManager.LoadAllVehicles();

            API.delay(60 * 1000, false, () => {
                syncTime();
            });
            syncTime();
        }

        private void syncTime() {
            DateTime Now = DateTime.Now;
            API.setTime(Now.Hour, Now.Minute);

            API.consoleOutput("Synchronizing time: " + Now.Hour + ":" + Now.Minute);
        }


        /// <summary>
        /// Delete all sub-objects.
        /// </summary>
        public void Dispose() {
            playerManager.Dispose();

            db.Dispose();
        }

        /// <summary>
        /// Setup all callbacks.
        /// </summary>
        private void setupCallbacks() {
            API.onPlayerConnected += onPlayerConnected;
            API.onPlayerDisconnected += onPlayerDisconnected;
            API.onChatMessage += onChatMessage;
            API.onClientEventTrigger += onClientEventTrigger;
            API.onResourceStop += onResourceStop;
            API.onPlayerFinishedDownload += OnPlayerFinishedDownload;
        }

        /// <summary>
        /// Handle moment when this resource stops.
        /// </summary>
        private void onResourceStop() {
            Dispose();
        }

        public void OnPlayerFinishedDownload(Client player)
        {
            API.triggerClientEvent(player, "SHOW_LOGIN_SCREEN");
            API.fetchNativeFromPlayer<bool>(player, 0xA328A24AAA6B7FDC, 1); // BLUR
        }

        /// <summary>
        /// Handle events triggered by client.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventName"></param>
        /// <param name="arguments"></param>
        private void onClientEventTrigger(Client sender, string eventName, params object[] arguments) {
            Player player = playerManager.findPlayerByHandle(sender);
            if (player == null) {
                API.consoleOutput("Player " + sender.name + " triggered event however was not registered in player manager.");
                API.kickPlayer(sender);
                return;
            }

            if (eventName == "doLogin") {
                if (arguments.Length != 2) {
                    API.consoleOutput(sender.name + " send invalid doLogin event");
                    return;
                }

                string username = ((String) arguments[0]).ToLower();
                string password = (string) arguments[1];

                Database.Data.User user = db.findUserByName(username);

                if (user == null) {
                    API.triggerClientEvent(sender, "LOGIN_INCORRECT", "Nie znaleziono użytkownika o nazwie " + username);
                    //API.sendNotificationToPlayer(sender, "Nie znaleziono użytkownika o nazwie ~r~" + username);
                    // Invalid username
                    return;
                }

                if (playerManager.findPlayerByAccountId(user.id) != null) {
                    API.triggerClientEvent(sender, "LOGIN_INCORRECT", "Próbujesz się zalogować na konto które jest już obecnie zalogowane.");
                    //API.sendNotificationToPlayer(sender, "Próbujesz się zalogować na konto które jest już w grze.");
                    // player online
                    return;
                }
                string hash = CryptSharp.Crypter.Blowfish.Crypt(password, "$2a$13$" + user.password_salt); // Encode password with salt
                if (user.password_hash != hash) {
                    API.triggerClientEvent(sender, "LOGIN_INCORRECT", "Podane hasło jest niepoprawne, wprowadź hasło ponownie lub spróbuj z <kbd>Caps Lock</kbd>");
                    //API.sendNotificationToPlayer(sender, "Podane hasło jest ~r~niepoprawne~w~, spróbuj ponownie.");
                    // invalid password
                    return;
                }

                API.triggerClientEvent(sender, "LOGIN_PASSWORD_OK");
                API.sendNativeToPlayer(sender, Hash.DO_SCREEN_FADE_OUT, 300);
                API.sendChatMessageToPlayer(sender, "Witaj ~g~" + user.name + "~w~, pomyślnie zalogowałeś się do swojego konta.");

                // ----- [ All flags admin ]: ----- //

                if((user.member_game_admin_perm & AdminPerms.FLAG_HASALL) == AdminPerms.FLAG_HASALL)
                {
                    API.sendChatMessageToPlayer(sender, "~r~Zalogowano z pełnym zestawem uprawnień administratora głównego.");
                    
                }

                API.fetchNativeFromPlayer<bool>(player, 0xEFACC8AEF94430D5, 1); // Disable Blur Effect
                API.delay(900, true, () => {
                    API.setEntityPositionFrozen(sender, false);
                    player.InitAuth(user.id);
                    Character.LoadPlayerCharacter(player);
                    API.sendNativeToPlayer(sender, Hash.DO_SCREEN_FADE_IN, 300);
                    
                });
            }
            if(eventName == "showVehicles")
            {
                ArrayList vehicleslist = Database.Context.Instance.getVehiclesByOwner(player.CurrentCharacter.getUID());

                if (vehicleslist.Count == 0)
                {
                    API.sendChatMessageToPlayer(sender, "Nie posiadasz żadnych pojazdów.");
                    return;
                }
                string items = "";
               // var json = "";
                foreach (var veh in vehicleslist)
                {
                    Database.Data.Vehicle VehicleData = (Database.Data.Vehicle)veh;
                    API.sendChatMessageToPlayer(sender, string.Format("~r~(UID: {0})~w~ {1}", VehicleData.uid, (VehicleHash)VehicleData.model));
                    //items = string.Format("<div class='notification'>(UID: {0}) {1}<a href = '#' class='button'>Informacje</a><a href = '#' class='button'>Zespawnuj pojazd</a></div>", VehicleData.uid, (VehicleHash)VehicleData.model);
                    
                    items = items.Insert(items.Length, string.Format("<div class='notification'>(UID: {0}) {1}<a href = '#' class='button'>Informacje</a><a href = '#' class='button'>Zespawnuj pojazd</a></div>", VehicleData.uid, (VehicleHash)VehicleData.model));
                    items.Insert(items.Length, "test");
                    //API.triggerClientEvent(sender, "vehicleList", json);
                    //API.sendChatMessageToPlayer(sender, json);

                }
                //API.sendChatMessageToPlayer(sender, items.ToString() + " - ehe");
                API.triggerClientEvent(sender, "vehicleList", items.ToString());
            }
        }

        /// <summary>
        /// Handle chat message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        /// <param name="cancel"></param>
        private void onChatMessage(Client sender, string message, CancelEventArgs cancel) {
            Player player = playerManager.findPlayerByHandle(sender);
            if (player == null || player.CurrentCharacter == null) {
                return;
            }

            Utils.ProxDetector(30, player, player.CurrentCharacter.GetFullName() + " mówi: " + message, "~#FFFFFF~", "~#C8C8C8~", "~#AAAAAA~", "~#8C8C8C~", "~#6E6E6E~");
            
            //string finalMessage = player.CurrentCharacter.GetFullName() + " mówi: " + message;
            /*foreach (Client client in API.getPlayersInRadiusOfPlayer(Commands.LOCAL_CHAT_RADIUS, sender)) {
                client.sendChatMessage(finalMessage);
            }*/

            cancel.Cancel = true;
        }


        /// <summary>
        /// Handle player connection.
        /// </summary>
        /// <param name="player"></param>
        private void onPlayerConnected(Client player)
        {
            playerManager.registerPlayer(player);

            // Set player dimension to player handle + 10000. It should be suffficiently unique.
            API.setEntityDimension(player, player.handle.Value + 10000);
            API.setEntityPositionFrozen(player, true);

            API.freezePlayerTime(player, true);
        }

        /// <summary>
        /// Handle player disconnect.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="reason"></param>
        private void onPlayerDisconnected(Client player, string reason) {
            playerManager.unregisterPlayer(player);
        }
    }
}

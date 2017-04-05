using System;

using GTANetworkServer;
using GTANetworkShared;
using System.Collections.Generic;

namespace Core {
    /// <summary>
    /// The role-play player manager.
    /// </summary>
    class PlayerManager : IDisposable {
        /// <summary>
        /// The static instance of the player manager.
        /// </summary>
        public static PlayerManager Instance = null;

        /// <summary>
        /// The dictionary mapping client network handle to role-play player object.
        /// </summary>
        private Dictionary<NetHandle, Player> playersDict = new Dictionary<NetHandle, Player>();

        /// <summary>
        /// Constructor.
        /// </summary>
        public PlayerManager() {
            Instance = this;
        }

        /// <summary>
        /// Save players and release all the data held by player manager.
        /// </summary>
        public void Dispose() {
            foreach (KeyValuePair<NetHandle, Player> kv in playersDict) {
                Player player = kv.Value;
                player.Save();
                player.Dispose();
            }
            playersDict.Clear();
        }

        /// <summary>
        /// Register new player into the players manager.
        /// </summary>
        /// <param name="client">The client to register player for.</param>
        /// <returns>The role-play player object.</returns>
        public Player registerPlayer(Client client) {
            Player player = new Player(client);
            playersDict.Add(client.handle, player);
            
            return player;
        }

        /// <summary>
        /// Unregister player by net handle from players manager.
        /// </summary>
        /// <param name="handle">The handle of the player to unregister</param>
        /// <returns>Return true if player was registered false otherwise</returns>
        public bool unregisterPlayer(NetHandle handle) {
            Player player = findPlayerByHandle(handle);
            if (player != null) {
                player.Save();
                player.Dispose();
                playersDict.Remove(handle);
            }
            return false;
        }

        /// <summary>
        /// Find role-play player by network handle of the client.
        /// </summary>
        /// <param name="handle">The network handle of the client used for lookup.</param>
        /// <returns>Role-play player object.</returns>
        public Player findPlayerByHandle(NetHandle handle) {
           return playersDict.Get(handle);
        }


        /// <summary>
        /// Find player object by account id.
        /// </summary>
        /// <param name="AccountId">The account id used for lookup.</param>
        /// <returns>Player using the account by given id or null in case no matching player is found.</returns>
        public Player findPlayerByAccountId(long AccountId) {
            foreach (Player player in playersDict.Values) {
                if (player.AccountId == AccountId) {
                    return player;
                }
            }
            return null;
        }
    }
}

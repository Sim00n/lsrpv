using System;
using System.Diagnostics;

using GTANetworkServer;
using GTANetworkShared;

namespace Core {

    /// <summary>
    /// The RP player data class.
    /// </summary>
    class Player : IDisposable {
        /// <summary>
        /// The GTA:N client object.
        /// </summary>
        public Client client    = null;

        /// <summary>
        /// Current player character.
        /// </summary>
        public Character character = null;

        /// <summary>
        /// Public getter for current player character.
        /// </summary>
        public Character CurrentCharacter {
            get {
                return character;
            }
        }

        /// <summary>
        /// The RP account id.
        /// </summary>
        private int accountId = Database.Data.User.INVALID_ACCOUNT_ID;

        /// <summary>
        /// The RP account id getter.
        /// </summary>
        public int AccountId {
            get {
                return accountId;
            }
        }



       
        /// <summary>
        /// Constructor of player object.
        /// </summary>
        /// <param name="Client">The GTA:N client representing the player.</param>
        public Player(Client Client) {
            client = Client;
        }


        /// <summary>
        /// Delete sub-objects.
        /// </summary>
        public void Dispose() {
            if (character != null) {
                character.Dispose();
                character = null;
            }
        }

        /// <summary>
        /// Initialize authentication info for the player.
        /// </summary>
        /// <remarks>
        /// This is not possible to call this method twice for single account.
        /// In case there will be need for logout the logout method needs to be added.
        /// </remarks>
        /// <param name="AccountId">The id of the account player was authenticated to.</param>
        public void InitAuth(int AccountId) {
            Debug.Assert(accountId == Database.Data.User.INVALID_ACCOUNT_ID);
            Debug.Assert(AccountId != Database.Data.User.INVALID_ACCOUNT_ID);

            accountId = AccountId;

            client.setSyncedData("AccountId", accountId);
        }

        /// <summary>
        /// Check if given player is authenticated.
        /// </summary>
        /// <returns>true if player is authenticated false otherwise</returns>
        bool IsAuthenticated() {
            return accountId != Database.Data.User.INVALID_ACCOUNT_ID;
        }

        /// <summary>
        /// Set current player character.
        /// </summary>
        /// <param name="Char">The character to use</param>
        public void SetCharacter(Character Char) {
            Debug.Assert(character == null && Char != null);

            character = Char;

            character.Spawn();
        }

        /// <summary>
        /// Save player data.
        /// </summary>
        public void Save() {
            if (character != null) {
                character.Save();
            }
        }

        /// <summary>
        /// Implicit operator casting Player to Client.
        /// </summary>
        /// <param name="player">Player to cast</param>
        public static implicit operator Client(Player player) {
            return player.client;
        }

        /// <summary>
        /// Implicit operator casting Player to NetHandle of client.
        /// </summary>
        /// <param name="player">Player to cast</param>
        public static implicit operator NetHandle(Player player) {
            return player.client.handle;
        }
    }
}

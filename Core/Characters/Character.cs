using System;
using System.Diagnostics;
using System.Collections;

using GTANetworkServer;
using GTANetworkShared;

namespace Core {
    /// <summary>
    /// Role play character class.
    /// </summary>
    /// <remarks>Character without owner is prohibited</remarks>
    class Character : IDisposable {

        /// <summary>
        /// The character database object.
        /// </summary>
        private Database.Data.Character data = null;

        /// <summary>
        /// The owner player.
        /// </summary>
        private Player owner;

        public int gameperm { get; set; }
        /// <summary>
        /// Character constructor.
        /// </summary>
        /// <param name="Owner">The character owner.</param>
        public Character(Player Owner, Database.Data.Character Data = null) {
            Debug.Assert(Owner != null, "Character without owner is prohibited");
            owner = Owner;
            data = Data;
        }

        /// <summary>
        /// Delete all sub-objects.
        /// </summary>
        public void Dispose() {
            // no-op
        }

        /// <summary>
        /// Update character state before saving.
        /// </summary>
        private void UpdateState() {
            Debug.Assert(owner.character == this);
            Debug.Assert(data != null);

            data.skin = API.shared.getEntityModel(owner);
            Vector3 pos = API.shared.getEntityPosition(owner);
            data.x = pos.X;
            data.y = pos.Y;
            data.z = pos.Z;
            data.rz = API.shared.getEntityRotation(owner).Z;
            data.dimension = API.shared.getEntityDimension(owner);
            data.health = (short) API.shared.getPlayerHealth(owner);
        }


        /// <summary>
        /// Save or create character in database.
        /// </summary>
        /// <returns>true in case operation succeeds, false otherwise</returns>
        public bool Save() {
            Debug.Assert(data != null);

            if (data.id == Database.Data.Character.INVALID_ID) {
                // Ensure that owner id is set.
                data.owner = owner.AccountId;

                return Database.Context.Instance.createCharacter(ref data);
            }

            UpdateState();
            return Database.Context.Instance.updateCharacter(data);
        }

        /// <summary>
        /// Spawn the character.
        /// </summary>
        public void Spawn() {
            API.shared.setEntityPosition(owner, new Vector3(data.x, data.y, data.z));
            API.shared.setEntityRotation(owner, new Vector3(0.0, 0.0, data.rz));
            API.shared.setEntityDimension(owner, data.dimension);

            API.shared.setPlayerSkin(owner, (PedHash)data.skin);

            API.shared.setPlayerHealth(owner, data.health);
            API.shared.sendChatMessageToPlayer(owner, this.GetFullName() + " (UID: " + data.id + " | GID: " + owner.AccountId + ")");
        }

        /// <summary>
        /// Get character full name.
        /// </summary>
        /// <returns>Character full name.</returns>
        public string GetFullName() {
            return this.data.name + " " + this.data.surname;
        }

        /// <summary>
        /// Get uid character
        /// </summary>
        /// <returns>UID</returns>
        public long getUID()
        {
            return this.data.id;
        }

       

        /// <summary>
        /// Load and set as current character at 1 slot for the player.
        /// </summary>
        /// <param name="player">The player for which to load and set the character.</param>
        static public void LoadPlayerCharacter(Player player) {
            ArrayList characters = Database.Context.Instance.getCharactersByOwner(player.AccountId);
            if (characters.Count == 0) {
                // No characters :-(
                API.shared.sendChatMessageToPlayer(player, "Na Twoim koncie nie ma żadnej postaci.");
                API.shared.kickPlayer(player);
                return;
            }

            Database.Data.Character data = (Database.Data.Character) characters[0];
            Character character = new Character(player, data);
            player.SetCharacter(character);
        }
    }
}

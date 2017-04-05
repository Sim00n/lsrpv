#pragma warning disable CS0649
namespace Database.Data {
    /// <summary>
    /// Role-play character data.
    /// </summary>
    [DBTable("characters")]
    public class Character {
        /// <summary>
        /// Invalid index of the character.
        /// </summary>
        public const long INVALID_ID = 0;

        /// <summary>
        /// The unique id of the character.
        /// </summary>
        [DBField("id", IsUpdateKey: true)]
        public long id = INVALID_ID;

        /// <summary>
        /// The character role-play name.
        /// </summary>
        [DBField("name", SkipUpdate: true)]
        public string name;

        /// <summary>
        /// The character role-play surname.
        /// </summary>
        [DBField("surname", SkipUpdate: true)]
        public string surname;

        /// <summary>
        /// The unique id of the owner account.
        /// </summary>
        [DBField("owner", SkipUpdate: true)]
        public long owner;

        /// <summary>
        /// Is this character male?
        /// </summary>
        [DBField("is_male", SkipUpdate: true)]
        public bool is_male;

        /// <summary>
        /// The hashed name of the skin.
        /// </summary>
        [DBField("skin")]
        public int skin;

        /// <summary>
        /// The character health.
        /// </summary>
        [DBField("health")]
        public short health;

        /// <summary>
        /// The last character position x-axis.
        /// </summary>
        [DBField("x")]
        public float x;

        /// <summary>
        /// The last character position y-axis.
        /// </summary>
        [DBField("y")]
        public float y;

        /// <summary>
        /// The last character position z-axis.
        /// </summary>
        [DBField("z")]
        public float z;

        /// <summary>
        /// The last character rotation rz-axis.
        /// </summary>
        [DBField("rz")]
        public float rz;

        /// <summary>
        /// The last character dimension.
        /// </summary>
        [DBField("dimension")]
        public int dimension;
    }
}

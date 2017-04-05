#pragma warning disable CS0649
namespace Database.Data {
    /// <summary>
    /// User account data.
    /// </summary>
    [DBTable("vrp_core_members")]
    public class User {
        /// <summary>
        /// Invalid account id constant.
        /// </summary>
        public const int INVALID_ACCOUNT_ID = 0;

        /// <summary>
        /// The unique id of the account.
        /// </summary>
        [DBField("member_id")]
        public int id = INVALID_ACCOUNT_ID;

        /// <summary>
        /// The lower cased name of the account.
        /// </summary>
        [DBField("name")]
        public string name;

        /// <summary>
        /// E-mail adress of account
        /// </summary>
        [DBField("email")]
        public string email;

        /// <summary>
        /// Encoded password.
        /// </summary>
        [DBField("members_pass_hash")]
        public string password_hash;

        /// <summary>
        /// Encoded salt
        /// </summary>
        [DBField("members_pass_salt")]
        public string password_salt;

        /// <summary>
        /// Admin permission
        /// </summary>
        [DBField("member_game_admin_perm")]
        public int member_game_admin_perm = 0;
    }
}

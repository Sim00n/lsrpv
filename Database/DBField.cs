using System;

namespace Database {
    /// <summary>
    /// The database field attribute used for easier SQL object deserialization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    class DBField : Attribute {
        /// <summary>
        /// The name of the column in database.
        /// </summary>
        public string name;

        /// <summary>
        /// Is this field update key?
        /// </summary>
        public bool isUpdateKey;

        /// <summary>
        /// Skip this field during update.
        /// </summary>
        public bool skipUpdate;

        /// <summary>
        /// Setup database field.
        /// </summary>
        /// <param name="Name">Name of the column in database</param>
        /// <param name="IsUpdateKey">Is this field an update key? There can be only one update key.</param>
        /// <param name="SkipUpdate">Should this field update be skipped?</param>
        public DBField(string Name, bool IsUpdateKey = false, bool SkipUpdate = false) {
            name = Name;
            isUpdateKey = IsUpdateKey;
            skipUpdate = SkipUpdate;
        }
    }

}

using System;

namespace Database {
    /// <summary>
    /// The database table attribute used for easier SQL query creation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    class DBTable : Attribute {
        /// <summary>
        /// The name of the table.
        /// </summary>
        public string name;

        /// <summary>
        /// Construct the table attribute.
        /// </summary>
        /// <param name="Name">The database table name.</param>
        public DBTable(string Name) {
            name = Name;
        }
    }
}

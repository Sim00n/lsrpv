using System;
using System.Collections;
using System.Reflection;

using MySql.Data.MySqlClient;

namespace Database {
    /// <summary>
    /// Database context allowing easy queries to the server database.
    /// </summary>
    public class Context : IDisposable {
        /// <summary>
        /// Static instance to the context for better accessibility.
        /// </summary>
        public static Context Instance = null;

        /// <summary>
        /// The database connection.
        /// </summary>
        private MySqlConnection dbConnection = null;

        /// <summary>
        /// Database context constructor.
        /// </summary>
        public Context() {
            dbConnection = new MySqlConnection("server=127.0.0.1;port=3306;uid=root;pwd=dupa323232;database=vrp");
            dbConnection.Open();

            Instance = this;
        }

        /// <summary>
        /// Destroy object resources.
        /// </summary>
        public void Dispose() {
            dbConnection.Close();
            dbConnection.Dispose();
            dbConnection = null;

            Instance = null;
        }


        /// <summary>
        /// Internal helper used to deserialize SQL data into the data object.
        /// </summary>
        /// <param name="Obj">The object where to deserialize data into</param>
        /// <param name="Reader">The SQL reader used to read data</param>
        private void deserializeData(object Obj, MySqlDataReader Reader) {
            foreach (FieldInfo field in Obj.GetType().GetFields()) {
                DBField dbField = field.GetCustomAttribute<DBField>();
                if (dbField == null) {
                    continue;
                }

                field.SetValue(Obj, Reader[dbField.name]);
            }
        }

        /// <summary>
        /// Internal helper used to build an update query from the object.
        /// </summary>
        /// <remarks>
        /// The update queries are quite limited at the moment and support
        /// only one update key however it is sufficient for now.
        ///
        /// The method may throw exception in case object is somehow broken.
        /// </remarks>
        /// <param name="Obj">The object to build update query for.</param>
        /// <returns>Npgsql command with generated query.</returns>
        private MySqlCommand buildUpdateCommand(object Obj) {
            Type type = Obj.GetType();

            DBTable tableData = type.GetCustomAttribute<DBTable>();
            if (tableData == null) {
                throw new Exception("Tried to build update command using the object without DBTable attribute defined. Type: " + type.AssemblyQualifiedName);
            }

            if (tableData.name.Length == 0) {
                throw new Exception("Tried to build update command using the object which table name is zero length. Type: " + type.AssemblyQualifiedName);
            }

            string where = "";

            MySqlCommand command = new MySqlCommand("", dbConnection);

            command.CommandText = "UPDATE " + tableData.name + " ";

            bool isFirstField = true;
            foreach (FieldInfo field in type.GetFields()) {
                DBField dbField = field.GetCustomAttribute<DBField>();
                if (dbField == null) {
                    continue;
                }

                if (dbField.skipUpdate) {
                    continue;
                }

                string fieldName = dbField.name;
                string paramName = "@" + fieldName;
                object value = field.GetValue(Obj);

                command.Parameters.AddWithValue(paramName, value);

                if (dbField.isUpdateKey) {
                    if (where.Length > 0) {
                        throw new Exception("Unsupported duplicate update key detected name = " + fieldName + ", where value = " + where);
                    }
                    else {
                        where = " WHERE " + fieldName + " = " + paramName;
                    }
                    continue;
                }

                if (isFirstField) {
                    command.CommandText += "SET ";
                    isFirstField = false;
                }
                else {
                    command.CommandText += ", ";
                }

                command.CommandText += dbField.name + " = " + paramName;
            }

            command.CommandText += where;
            return command;
        }

        /// <summary>
        /// Find user by username.
        /// </summary>
        /// <param name="username">The username used to find user (should be lower case)</param>
        /// <returns>The database data object of the user - null in case no user was found</returns>
        public Data.User findUserByName(string username) {
            MySqlCommand command = new MySqlCommand("SELECT member_id, name, email, members_pass_hash, members_pass_salt, member_game_admin_perm FROM vrp_core_members WHERE name=@username", dbConnection);
            command.Parameters.AddWithValue("@username", username);

            MySqlDataReader reader = command.ExecuteReader();
            if (!reader.Read()) {
                reader.Close();
                return null;
            }

            Data.User user = new Data.User();
            deserializeData(user, reader);
            reader.Close();
            return user;
        }

        /// <summary>
        /// Get characters array by owner.
        /// </summary>
        /// <param name="OwnerAccountId">The owner account id</param>
        /// <returns>The array list containing database data objects of the characters.</returns>
        public ArrayList getCharactersByOwner(long OwnerAccountId) {
            MySqlCommand command = new MySqlCommand("SELECT * FROM characters WHERE owner=@owner", dbConnection);
            command.Parameters.AddWithValue("@owner", OwnerAccountId);

            ArrayList characters = new ArrayList();

            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read()) {
                Data.Character character = new Data.Character();
                deserializeData(character, reader);
                characters.Add(character);
            }
            reader.Close();
            return characters;
        }


        /// <summary>
        /// Create character in database.
        /// </summary>
        /// <param name="CharacterData">The data of character to create</param>
        /// <returns>Returns true and sets the id to id of the created entry if character was created returns false otherwise.</returns>
        public bool createCharacter(ref Data.Character CharacterData) {
            if (CharacterData.id != Data.Character.INVALID_ID) {
                throw new Exception("Tried to create character from data of already exising character.");
            }

            MySqlCommand command = new MySqlCommand("INSERT INTO characters (name,surname,owner,is_male,skin) VALUES(@name,@surname,@owner,@is_male,@skin) RETURNING id", dbConnection);
            command.Parameters.AddWithValue("@name", CharacterData.name);
            command.Parameters.AddWithValue("@surname", CharacterData.surname);
            command.Parameters.AddWithValue("@owner", CharacterData.owner);
            command.Parameters.AddWithValue("@is_male", CharacterData.is_male);
            command.Parameters.AddWithValue("@skin", CharacterData.skin);

            object id = command.ExecuteScalar();
            if (id == null) {
                return false;
            }

            CharacterData.id = (long)id;
            return true;
        }


        /// <summary>
        /// Update character state in database.
        /// </summary>
        /// <param name="data">The data of the character to update</param>
        /// <returns>true if character was updated false otherwise</returns>
        public bool updateCharacter(Data.Character data) {
            MySqlCommand command = buildUpdateCommand(data);
            return command.ExecuteNonQuery() == 1;
        }

        /// <summary>
        /// Create vehicle in database.
        /// </summary>
        /// <param name="VehicleData">The data of character to create</param>
        /// <returns>Returns true and sets the id to id of the created entry if character was created returns false otherwise.</returns>
        public bool createVehicle(ref Data.Vehicle VehicleData)
        {
            if (VehicleData.uid != Data.Vehicle.INVALID_ID)
            {
                throw new Exception("Tried to create vehicle from data of already exising vehicle.");
            }

            MySqlCommand command = new MySqlCommand("INSERT INTO vehicles (model,posx,posy,posz,posa, world, interior, color1, color2, fuel, fueltype, health, mileage, locked, visual, registered, registerplate, owner, ownertype) VALUES(@model,@posx,@posy,@posz,@posa, @world, @interior, @color1, @color2, @fuel, @fueltype, @health, @mileage, @locked, @visual, @registered, @registerplate, @owner, @ownertype) RETURNING id", dbConnection);
            command.Parameters.AddWithValue("@model", VehicleData.model);
            command.Parameters.AddWithValue("@posx", VehicleData.posx);
            command.Parameters.AddWithValue("@posy", VehicleData.posy);
            command.Parameters.AddWithValue("@posz", VehicleData.posz);
            command.Parameters.AddWithValue("@posa", VehicleData.posa);
            command.Parameters.AddWithValue("@world", VehicleData.world);
            command.Parameters.AddWithValue("@interior", VehicleData.interior);
            command.Parameters.AddWithValue("@color1", VehicleData.color1);
            command.Parameters.AddWithValue("@color2", VehicleData.color2);
            command.Parameters.AddWithValue("@fuel", VehicleData.fuel);
            command.Parameters.AddWithValue("@fueltype", VehicleData.fueltype);
            command.Parameters.AddWithValue("@health", VehicleData.health);
            command.Parameters.AddWithValue("@mileage", VehicleData.mileage);
            command.Parameters.AddWithValue("@locked", VehicleData.locked);
            command.Parameters.AddWithValue("@visual", VehicleData.visual);
            command.Parameters.AddWithValue("@registered", VehicleData.registered);
            command.Parameters.AddWithValue("@registerplate", VehicleData.registerplate);
            command.Parameters.AddWithValue("@owner", VehicleData.owner);
            command.Parameters.AddWithValue("@ownertype", VehicleData.ownertype);



            object id = command.ExecuteScalar();
            if (id == null)
            {
                return false;
            }

            VehicleData.uid = (long)id;
            return true;
        }

        /// <summary>
        /// Load Player Vehicles
        /// </summary>
        /// <param name="OwnerAccountId"></param>
        /// <returns></returns>
        public ArrayList getVehiclesByOwner(long OwnerAccountId)
        {
            MySqlCommand command = new MySqlCommand("SELECT * FROM vehicles WHERE owner=@owner", dbConnection);
            command.Parameters.AddWithValue("@owner", OwnerAccountId);

            ArrayList vehicles = new ArrayList();

            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Data.Vehicle vehicle = new Data.Vehicle();
                deserializeData(vehicle, reader);
                vehicles.Add(vehicle);
            }
            reader.Close();
            return vehicles;
        }

        /// <summary>
        /// Load Vehicles
        /// </summary>
        /// 
        public ArrayList getVehicles()
        {
            MySqlCommand command = new MySqlCommand("SELECT * FROM vehicles WHERE 1", dbConnection);

            ArrayList vehicles = new ArrayList();

            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Data.Vehicle vehicle = new Data.Vehicle();
                deserializeData(vehicle, reader);
                vehicles.Add(vehicle);
            }
            reader.Close();
            return vehicles;
        }

        /// <summary>
        /// Update vehicle state in database
        /// </summary>
        /// <param name="data">The data of the vehicle to update</param>
        /// <returns>true if vehicle was updated false otherwise</returns>
        public bool updateVehicle(Data.Vehicle data)
        {
            MySqlCommand command = buildUpdateCommand(data);
            return command.ExecuteNonQuery() == 1;
        }


    }
}

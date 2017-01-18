using MySql.Data.MySqlClient;
using Insight.Database.Providers.MySql;
using Insight.Database;
using System;

public class Database
{
	private static Database instance = null;
	private static MySqlConnectionStringBuilder _database;

	public Database()
	{
		if(Database.instance != null)
		{
			return;
		}

		MySqlInsightDbProvider.RegisterProvider();
		_database = new MySqlConnectionStringBuilder(String.Format("server={0};user={1};database={2};port={3};password={4};", Config.DB_HOST, Config.DB_USER, Config.DB_DB, Config.DB_PORT, Config.DB_PASS));
	}

	public static Database getInstance()
	{
		if (Database.instance == null)
		{
			Database.instance = new Database();
			
		}
		return Database.instance;
	}

	public System.Data.Common.DbConnection db()
	{
		return _database.Connection();
	}

	public class users
	{
		public int id { get; set; }
		public string username { get; set; }
		public string hash { get; set; }
		public int admin { get; set; }
	}

	public class characters
	{
		/**
		 * Database saved stuff.
		 */
		public int cid { get; set; }
		public int owner { get; set; }
		public string name { get; set; }
		public int hours { get; set; }
		public int minutes { get; set; }
		public float health { get; set; }
		public int gender { get; set; }
		public int skin { get; set; }
		public int money { get; set; }
		public int bankmoney { get; set; }
		public string dob { get; set; }
		public int dimension { get; set; }
		public float posx { get; set; }
		public float posy { get; set; }
		public float posz { get; set; }
		public int bw { get; set; }
		public bool online { get; set; }
		
		/**
		 * Runtime stuff
		 */


		public void save()
		{
			string query = @"UPDATE characters SET
				hours = '{1}',
				minutes = '{2}',
				health = '{3}',
				skin = '{4}',
				money = '{5}',
				bankmoney = '{6}',
				dimension = '{7}',
				posx = '{8}',
				posy = '{9}',
				posz = '{10}',
				bw = '{11}'
				WHERE cid = '{0}'";

			Database.getInstance().db().QuerySql(String.Format(query, cid, hours, minutes, health, skin, money, bankmoney, dimension, posx, posy, posz, bw));
		}
	}

	public class items
	{
		public int iid { get; set; }
		public int type { get; set; }
		public int owner { get; set; }
		public int place { get; set; }
		public string name { get; set; }
		public int int1 { get; set; }
		public int int2 { get; set; }
		public int int3 { get; set; }
		public string str1 { get; set; }
		public string str2 { get; set; }
		public string str3 { get; set; }
	}
}
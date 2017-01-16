public class Config
{
	public static readonly string DB_HOST = "127.0.0.1";
	public static readonly string DB_USER = "lsrpv";
	public static readonly string DB_PASS = "lsrpv123";
	public static readonly string DB_PORT = "3306";
	public static readonly string DB_DB = "lsrpv";

	public enum OWNERS {
		WORLD		= 1,
		PLAYER		= 2,
		VEHICLE		= 3,
		ITEM		= 4
	};

	public enum LOGS { DEBUG, INFO, DB };
}
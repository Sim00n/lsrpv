﻿using GTANetworkShared;

public class Config
{
	public static readonly string DB_HOST = "127.0.0.1";
	public static readonly string DB_USER = "lsrpv";
	public static readonly string DB_PASS = "lsrpv123";
	public static readonly string DB_PORT = "3306";
	public static readonly string DB_DB = "lsrpv";

	public static readonly Vector3 LOGIN_POSITION = new Vector3(-102, 7481, 3);
	public static readonly Vector3[] SPAWN_POSITIONS =
	{
		new Vector3(-1041, -2745, 22),
		new Vector3(-1045, -2743, 22),
		new Vector3(-1048, 2741, 22),
		new Vector3(-1038, -2748, 22),
		new Vector3(-1032, -2752, 21),
		new Vector3(-1037, -2743, 21),
		new Vector3(-1042, -2739, 21)
	};

	public static readonly int BW_TIME = 60 * 60;

	public enum OWNERS {
		WORLD		= 1,
		PLAYER		= 2,
		VEHICLE		= 3,
		ITEM		= 4
	};

	public enum LOGS { DEBUG, INFO, DB };
}
using GTANetworkShared;
using System.Collections.Generic;

public class Config
{
	/**
	 * Database
	 */
	public static readonly string DB_HOST = "127.0.0.1";
	public static readonly string DB_USER = "lsrpv";
	public static readonly string DB_PASS = "lsrpv123";
	public static readonly string DB_PORT = "3306";
	public static readonly string DB_DB = "lsrpv";

	/**
	 * Settings
	 */
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
	public static readonly int HOURLY_PAY = 500;

	/**
	 * Enums and lists
	 */
	public enum OWNERS {
		WORLD		= 1,
		PLAYER		= 2,
		VEHICLE		= 3,
		ITEM		= 4
	};

	public enum AnimationFlags
	{
		Loop = 1 << 0,
		StopOnLastFrame = 1 << 1,
		OnlyAnimateUpperBody = 1 << 4,
		AllowPlayerControl = 1 << 5,
		Cancellable = 1 << 7
	}

	public enum LOGS { DEBUG, INFO, DB };

	public struct ITEM_OBJECT
	{
		public int model;
		public float xoff, yoff, zoff, rx, ry, rz;
		public Vector3 pos { get { return new Vector3(xoff, yoff, zoff); } } // no set
		public Vector3 rot { get { return new Vector3(rx, ry, rz); } } // no set
		public ITEM_OBJECT(int model, float xoff, float yoff, float zoff, float rx, float ry, float rz)
		{
			this.model = model;
			this.xoff = xoff;
			this.yoff = yoff;
			this.zoff = zoff;
			this.rx = rx;
			this.ry = ry;
			this.rz = rz;
		}
	}

	public static Dictionary<Item.TYPE, ITEM_OBJECT> OBJECTS = new Dictionary<Item.TYPE, ITEM_OBJECT>()
	{
		{ Item.TYPE.WATCH,			new ITEM_OBJECT(1407761612,	0F, 0F, -1F, 0F, 0F, 0F)	},	// good
		{ Item.TYPE.FOOD,			new ITEM_OBJECT(1655278098,	0F, 0F, -1F, 0F, 0F, 0F)	},	// good
		{ Item.TYPE.CIGARETTE,		new ITEM_OBJECT(66849370,	0F, 0F, -1F, -90F, 0F, 0F)	},	// good
		{ Item.TYPE.DICE,			new ITEM_OBJECT(2104951174,	0F, 0F, -1F, 0F, 0F, 0F)	},	// shitty pool ball
		{ Item.TYPE.CELLPHONE,		new ITEM_OBJECT(760935785,	0F, 0F, -1F, -90F, 0F, 0F)	},	// good
	};


	/**
	 * Colors
	 */
	public static readonly string COLOR_ME = "C2A2DA";
	public static readonly string COLOR_DO = "9A9CCD";
	public static readonly string COLOR_B = "AFAFAF";
	public static readonly string COLOR_C = "E6E6E6";
	public static readonly string COLOR_K = "E6E6E6";
	public static readonly string COLOR_I = "FFFFFF";
	public static readonly string COLOR_RED = "F84C31";
	public static readonly string COLOR_GREEN = "B6F831";
}
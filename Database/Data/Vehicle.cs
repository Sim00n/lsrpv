#pragma warning disable CS0649
namespace Database.Data
{
    /// <summary>
    /// Role-play vehicle data.
    /// </summary>
    [DBTable("vehicles")]
    public class Vehicle
    {
        public const long INVALID_ID = 0;

        [DBField("uid", IsUpdateKey: true)]
        public long uid = INVALID_ID;

        [DBField("model")]
        public long model;

        // Position

        [DBField("posx")]
        public float posx;
        [DBField("posy")]
        public float posy;
        [DBField("posz")]
        public float posz;
        [DBField("posa")]
        public float posa;

        [DBField("world")]
        public int world;

        [DBField("interior")]
        public int interior;

        [DBField("color1")]
        public int color1;

        [DBField("color2")]
        public int color2;

        [DBField("fuel")]
        public int fuel;

        [DBField("fueltype")]
        public int fueltype;

        [DBField("health")]
        public float health;

        [DBField("mileage")]
        public long mileage;

        [DBField("locked")]
        public int locked;

        [DBField("visual")]
        public string visual;

        [DBField("registered")]
        public int registered;

        [DBField("registerplate")]
        public string registerplate;

        [DBField("owner")]
        public int owner;

        [DBField("ownertype")]
        public int ownertype;
    }
}

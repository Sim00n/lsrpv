using System;
using System.Collections.Generic;

using GTANetworkServer;
using GTANetworkShared;

namespace Core
{
    class VehicleData : Script
    {
        private static List<VehicleManager> Vehicles = new List<VehicleManager>();

        /// <summary>
        /// Add vehicle to VehicleManager
        /// </summary>
        /// <param name="vehicle"></param>

        public static void Add(VehicleManager vehicle)
        {
            Vehicles.Add(vehicle);
        }

        /// <summary>
        /// Remove vehicle from VehicleManager
        /// </summary>
        /// <param name="vehicle"></param>

        public static void Remove(VehicleManager vehicle)
        {
            Vehicles.Remove(vehicle);
        }

        /// <summary>
        /// Search vehicle by vehicle data
        /// </summary>
        /// <param name="VehicleData"> Vehicle Data </param>
        /// <returns>Vehicle</returns>

        public static VehicleManager getVehicleByData(Database.Data.Vehicle VehicleData)
        {
            return Vehicles.Find(x => x.data == VehicleData); ;
        }

        /// <summary>
        /// Search vehicle
        /// </summary>
        /// <param name="vehicle">Vehicle</param>
        /// <returns>Vehicle</returns>

        public static VehicleManager getVehicle(Vehicle vehicle)
        {
            return Vehicles.Find(x => x.vehicle == vehicle); ;
        }

        /// <summary>
        /// Search vehicle by vehicle nethandle
        /// </summary>
        /// <param name="vehicle">NetHandle Vehicle</param>
        /// <returns>Vehicle</returns>
        /// 

        public static VehicleManager getVehicleByHandle(NetHandle vehicle)
        {
            return Vehicles.Find(x => x.vehicle.handle == vehicle); ;
        }

        /// <summary>
        /// Search vehicle by uid
        /// </summary>
        /// <param name="uid">Vehicle UID</param>
        /// <returns>Vehicle</returns>
        
        public static VehicleManager getVehicleByUID(int uid)
        {
            return Vehicles.Find(x => x.data.uid == uid); ;
        }
    }
}

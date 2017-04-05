using System;

using System.Diagnostics;
using GTANetworkServer;
using GTANetworkShared;
using System.Collections.Generic;
using System.Collections;


namespace Core
{

    class VehicleManager : Script
    {
        public Database.Data.Vehicle data = null;

        public Vehicle vehicle;

        // this is space for the group

        public VehicleManager(){} // Constructor

        public VehicleManager(Database.Data.Vehicle vehData, Vehicle veh)
        {
            this.data = vehData;
            vehicle = veh;
            API.setVehicleEngineStatus(veh, false);
            API.setVehicleLocked(veh, true);
            VehicleData.Add(this);
        }

        /// <summary>
        /// Load all vehicles from database
        /// </summary>

        public static void LoadAllVehicles()
        {
            ArrayList vehicles = Database.Context.Instance.getVehicles();
            if (vehicles.Count == 0)
            {
                API.shared.consoleOutput("[Vehicles] Not found vehicles in Database");
                return;
            }
            API.shared.consoleOutput(string.Format("[Vehicles] Loaded {0} vehicles from database", vehicles.Count));

            foreach(var veh in vehicles)
            {
                Database.Data.Vehicle VehicleData = (Database.Data.Vehicle)veh;

                VehicleManager vehicle = new VehicleManager(VehicleData, API.shared.createVehicle((VehicleHash)VehicleData.model, new Vector3(VehicleData.posx, VehicleData.posy, VehicleData.posz), new Vector3(0.0f, 0.0f, VehicleData.posa), VehicleData.color1, VehicleData.color2));

                // Vehicle register
                if (VehicleData.registerplate.Length > 1) API.shared.setVehicleNumberPlate(vehicle.vehicle.handle, VehicleData.registerplate);


            }

        }

        /// <summary>
        /// Load all player vehicles from database
        /// </summary>

        public static void LoadPlayerVehicles(long character)
        {
            ArrayList vehicles = Database.Context.Instance.getVehiclesByOwner(character);
            if (vehicles.Count == 0)
            {
                API.shared.consoleOutput("[Vehicles] Not found vehicles in Database");
                return;
            }
            API.shared.consoleOutput(string.Format("[Vehicles] Loaded {0} vehicles from database", vehicles.Count));

            foreach (var veh in vehicles)
            {
                Database.Data.Vehicle VehicleData = (Database.Data.Vehicle)veh;

                VehicleManager vehicle = new VehicleManager(VehicleData, API.shared.createVehicle((VehicleHash)VehicleData.model, new Vector3(VehicleData.posx, VehicleData.posy, VehicleData.posz), new Vector3(0.0f, 0.0f, VehicleData.posa), VehicleData.color1, VehicleData.color2));

                // Vehicle register
                if (VehicleData.registerplate.Length > 1) API.shared.setVehicleNumberPlate(vehicle.vehicle.handle, VehicleData.registerplate);


            }

        }

        /// <summary>
        /// Get Closest Vehicle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        /// 

        public static NetHandle GetClosestVehicle(Client sender, float distance = 10.0f)
        {
            NetHandle handleReturned = new NetHandle();
            foreach (var veh in API.shared.getAllVehicles())
            {
                Vector3 vehPos = API.shared.getEntityPosition(veh);
                float distanceVehicleToPlayer = sender.position.DistanceTo(vehPos);
                if (distanceVehicleToPlayer < distance)
                {
                    distance = distanceVehicleToPlayer;
                    handleReturned = veh;

                }
            }
            return handleReturned;
        }

        /// <summary>
        /// Update vehicle state before saving.
        /// </summary>
        private void UpdateState()
        {
            data.model = API.shared.getEntityModel(vehicle.handle);
            Vector3 pos = API.shared.getEntityPosition(vehicle.handle);
            Vector3 posa = API.shared.getEntityRotation(vehicle.handle);
            data.posx = pos.X;
            data.posy = pos.Y;
            data.posz = pos.Z;
            data.posa = posa.Z;
        }


        /// <summary>
        /// Save or create character in database.
        /// </summary>
        /// <returns>true in case operation succeeds, false otherwise</returns>
        public bool Save()
        {
            Debug.Assert(data != null);

            if (data.uid == Database.Data.Vehicle.INVALID_ID)
            {
                return Database.Context.Instance.createVehicle(ref data);
            }

            UpdateState();
            return Database.Context.Instance.updateVehicle(data);
        }
    }
}

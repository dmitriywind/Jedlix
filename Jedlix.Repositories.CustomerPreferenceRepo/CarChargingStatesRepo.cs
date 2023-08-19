using System.Collections.Generic;
using System.Threading.Tasks;
using AssignmentTestConsole.Helpers;
using Jedlix.Models;
using Jedlix.Repositories.Contracts;
using Jedlix.Repositories.Entities;

namespace Jedlix.Repositories
{
    public class CarChargingStatesRepo : ICarChargingStatesRepo
    {
        public async Task SaveCarChargingState(CarChargingState carChargingState)
        {
            CarChargingStatesFakeRepo.CarChargingStateEntity = new CarChargingStateEntity
            {
                ChargingPowerKWh = carChargingState.ChargingPowerKWh,
                BatteryCapacity = carChargingState.BatteryCapacity,
                BatteryCurrentLevel = carChargingState.BatteryCurrentLevel,
            };
        }

        public async Task<CarChargingState> GetCarChargingState()
        {
            return new CarChargingState
            {
                ChargingPowerKWh = CarChargingStatesFakeRepo.CarChargingStateEntity.ChargingPowerKWh,
                BatteryCapacity = CarChargingStatesFakeRepo.CarChargingStateEntity.BatteryCapacity,
                BatteryCurrentLevel = CarChargingStatesFakeRepo.CarChargingStateEntity.BatteryCurrentLevel,
            };
        }
    }
}

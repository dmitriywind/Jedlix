using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AssignmentTestConsole.Helpers;
using Jedlix.Models;
using Jedlix.Repositories.Contracts;
using Jedlix.Repositories.Entities;

namespace Jedlix.Repositories
{
    public class ChargingTariffsRepo : IChargingTariffsRepo
    {
        public async Task AddChargingTariffs(IEnumerable<ChargingTariff> chargingTariffs)
        {
            if (ChargingTariffsFakeRepo.ChargingTariffEntities == null)
            {
                ChargingTariffsFakeRepo.ChargingTariffEntities = new List<ChargingTariffEntity>();
            }

            foreach (var chargingTariff in chargingTariffs)
            {
                ChargingTariffsFakeRepo.ChargingTariffEntities.Add(new ChargingTariffEntity
                {
                    DayOfWeek = (int) chargingTariff.DayOfWeek,
                    StartingFromTimeSpan = chargingTariff.StartingFromTimeSpan.ToString(),
                    EndingAtTimeSpan = chargingTariff.EndingAtTimeSpan.ToString(),
                    Price = chargingTariff.Price
                });
            }
        }

        public async Task<IEnumerable<ChargingTariff>> GetChargingTariffs()
        {
            var result = new List<ChargingTariff>();
            foreach (var chargingTariff in ChargingTariffsFakeRepo.ChargingTariffEntities)
            {
                result.Add(new ChargingTariff
                {
                    DayOfWeek = (DayOfWeek) chargingTariff.DayOfWeek,
                    StartingFromTimeSpan = TimeSpan.Parse(chargingTariff.StartingFromTimeSpan),
                    EndingAtTimeSpan = TimeSpan.Parse(chargingTariff.EndingAtTimeSpan),
                    Price = chargingTariff.Price
                });
            }

            return result;
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Jedlix.Models;

namespace Jedlix.Repositories.Contracts
{
    public interface IChargingTariffsRepo
    {
        Task AddChargingTariffs(IEnumerable<ChargingTariff> chargingTariffs);
        Task<IEnumerable<ChargingTariff>> GetChargingTariffs();
    }
}

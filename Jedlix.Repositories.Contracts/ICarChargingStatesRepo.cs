using Jedlix.Models;
using System.Threading.Tasks;

namespace Jedlix.Repositories.Contracts
{
    public interface ICarChargingStatesRepo
    {
        Task SaveCarChargingState(CarChargingState carChargingState);
        Task<CarChargingState> GetCarChargingState();
    }
}

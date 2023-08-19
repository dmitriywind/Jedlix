using Jedlix.Models.Events;
using System.Threading.Tasks;

namespace Jedlix.Core.Contracts
{
    public interface ICarChargingService
    {
        Task ProcessStartingChargingEvent(CarIsStartingChargingEvent startingChargingEvent);
    }
}

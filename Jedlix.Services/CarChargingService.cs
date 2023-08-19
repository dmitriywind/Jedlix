using System.Threading.Tasks;
using Jedlix.Core.Contracts;
using Jedlix.Models.Events;
using Jedlix.ServiceBusPublisher;
using Jedlix.ServiceBusPublisher.Messages;

namespace Jedlix.Core.Services
{
    public class CarChargingService : ICarChargingService
    {
        private readonly IChargingScheduleGenerationService _chargingScheduleGenerationService;
        private readonly IServiceBusMessageService<CarChargingProfileMessage> _chargingProfileMessageSender;

        public CarChargingService(
            IChargingScheduleGenerationService chargingScheduleGenerationService,
            IServiceBusMessageService<CarChargingProfileMessage> chargingProfileMessageSender)
        {
            _chargingScheduleGenerationService = chargingScheduleGenerationService;
            _chargingProfileMessageSender = chargingProfileMessageSender;
        }

        public async Task ProcessStartingChargingEvent(CarIsStartingChargingEvent startingChargingEvent)
        {
            var chargingProfiles = await _chargingScheduleGenerationService.GenerateChargingProfiles(startingChargingEvent.StartDateTimeOffset);
            await _chargingProfileMessageSender.SendAsync(new CarChargingProfileMessage
            {
                ChargingProfiles = chargingProfiles
            });
        }
    }
}

using System.Collections.Generic;
using Jedlix.Models;

namespace Jedlix.ServiceBusPublisher.Messages
{
    public class CarChargingProfileMessage
    {
        public List<CarChargingProfile> ChargingProfiles { get; set; }
    }
}

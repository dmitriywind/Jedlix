using System;

namespace Jedlix.Models
{
    public class CarChargingProfile
    {
        public DateTimeOffset StartDateTime { get; set; }
        public DateTimeOffset EndDateTime { get; set; }
        public bool ChargingAllowed { get; set; }
    }
}

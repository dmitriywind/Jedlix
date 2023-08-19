using System;

namespace Jedlix.Models.Events
{
    public class CarIsStartingChargingEvent
    {
        public DateTimeOffset StartDateTimeOffset { get; set; }
    }
}

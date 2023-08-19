using System;

namespace Jedlix.Models
{
    public class CustomerChargingPreference
    {   
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan LeavingTimeSpan { get; set; }
        public decimal LeavingBatteryLevel { get; set; }

        /// <summary>
        /// This is the minimum percentage of the battery we will always charge directly
        /// For example: if the car battery level would be let's say 15%, always first charge up to 20% before doing anything else
        /// </summary>
        public decimal DirectChargingPercentage { get; set; }
    }
}

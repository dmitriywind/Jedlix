using System;

namespace Jedlix.Models
{
    public class ChargingTariff
    {
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartingFromTimeSpan { get; set; }
        public TimeSpan EndingAtTimeSpan { get; set; }
        public decimal Price { get; set; }
    }
}

namespace Jedlix.Models
{
    public class CarChargingState
    {
        public decimal ChargingPowerKWh { get; set; }

        /// <summary>
        /// kWh
        /// </summary>
        public decimal BatteryCapacity { get; set; }

        /// <summary>
        /// kWh
        /// </summary>
        public decimal BatteryCurrentLevel { get; set; }
    }
}

namespace Jedlix.Repositories.Entities
{
    public class CarChargingStateEntity
    {
      public decimal ChargingPowerKWh { get; set; }
    
      public decimal BatteryCapacity { get; set; }
    
      public decimal BatteryCurrentLevel { get; set; }
    }
}

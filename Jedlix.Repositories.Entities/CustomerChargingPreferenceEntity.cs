namespace Jedlix.Repositories.Entities
{
    public class CustomerChargingPreferenceEntity
    {
        public int DayOfWeek { get; set; }
        
        public string LeavingDateTimeOffset { get; set; }
        
        public decimal LeavingBatteryLevel { get; set; }
        
        public decimal DirectChargingPercentage { get; set; }
    }
}

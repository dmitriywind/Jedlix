namespace Jedlix.Repositories.Entities
{
    public class ChargingTariffEntity
    {
        public int DayOfWeek { get; set; }

        public string StartingFromTimeSpan { get; set; }

        public string EndingAtTimeSpan { get; set; }

        public decimal Price { get; set; }
    }
}

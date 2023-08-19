using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jedlix.Models;

namespace Jedlix.Core.Contracts
{
    public interface IChargingScheduleGenerationService
    {
        Task<List<CarChargingProfile>> GenerateChargingProfiles(DateTimeOffset startDateTimeOffset);
    }
}

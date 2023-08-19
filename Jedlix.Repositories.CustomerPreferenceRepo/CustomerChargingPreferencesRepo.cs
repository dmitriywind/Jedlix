using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AssignmentTestConsole.Helpers;
using Jedlix.Models;
using Jedlix.Repositories.Contracts;
using Jedlix.Repositories.Entities;

namespace Jedlix.Repositories
{
    public class CustomerChargingPreferencesRepo : ICustomerChargingPreferencesRepo
    {
        public async Task AddCustomerChargingPreferences(IEnumerable<CustomerChargingPreference> customerChargingPreferences)
        {
            if (CustomerChargingPreferencesFakeRepo.CustomerChargingPreferenceEntities == null)
            {
                CustomerChargingPreferencesFakeRepo.CustomerChargingPreferenceEntities = new List<CustomerChargingPreferenceEntity>();
            }

            foreach (var customerChargingPreference in customerChargingPreferences)
            {
                CustomerChargingPreferencesFakeRepo.CustomerChargingPreferenceEntities.Add(new CustomerChargingPreferenceEntity
                {
                    DayOfWeek = (int)customerChargingPreference.DayOfWeek,
                    LeavingDateTimeOffset = customerChargingPreference.LeavingTimeSpan.ToString(),
                    LeavingBatteryLevel = customerChargingPreference.LeavingBatteryLevel,
                    DirectChargingPercentage = customerChargingPreference.DirectChargingPercentage
                });
            }
        }

        public async Task<IEnumerable<CustomerChargingPreference>> GetCustomerChargingPreferences()
        {
            var result = new List<CustomerChargingPreference>();
            foreach (var customerChargingPreference in CustomerChargingPreferencesFakeRepo.CustomerChargingPreferenceEntities)
            {
               result.Add(new CustomerChargingPreference
                {
                    DayOfWeek = (DayOfWeek)customerChargingPreference.DayOfWeek,
                    LeavingTimeSpan = TimeSpan.Parse(customerChargingPreference.LeavingDateTimeOffset),
                    LeavingBatteryLevel = customerChargingPreference.LeavingBatteryLevel,
                    DirectChargingPercentage = customerChargingPreference.DirectChargingPercentage
               });
            }

            return result;
        }
    }
}

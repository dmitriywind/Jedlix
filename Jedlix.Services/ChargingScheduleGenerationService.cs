using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jedlix.Core.Contracts;
using Jedlix.Models;
using Jedlix.Repositories.Contracts;

namespace Jedlix.Core.Services
{
    public class ChargingScheduleGenerationService : IChargingScheduleGenerationService
    {
        private readonly IChargingTariffsRepo _chargingTariffsRepo;
        private readonly ICustomerChargingPreferencesRepo _customerChargingPreferencesRepo;
        private readonly ICarChargingStatesRepo _carChargingStatesRepo;
        
        public ChargingScheduleGenerationService(
            IChargingTariffsRepo chargingTariffsRepo,
            ICustomerChargingPreferencesRepo customerChargingPreferencesRepo,
            ICarChargingStatesRepo carChargingStatesRepo)
        {
            _chargingTariffsRepo = chargingTariffsRepo;
            _customerChargingPreferencesRepo = customerChargingPreferencesRepo;
            _carChargingStatesRepo = carChargingStatesRepo;
        }

        public async Task<List<CarChargingProfile>> GenerateChargingProfiles(DateTimeOffset startDateTimeOffset)
        {
            var result = new List<CarChargingProfile>();

            var startDateTime = startDateTimeOffset; // some postponing required due to time of processing and profile sending+retrieving
            var startDateOfWeek = startDateTime.DayOfWeek;

            // assumption is that there is only one preference per day
            var customerPreference = await GetCustomerChargingPreferenceOrThrow(startDateOfWeek);

            var carChargingState = await _carChargingStatesRepo.GetCarChargingState(); // could also be input param for this generation
            var currentBatteryLevelPercentage = carChargingState.BatteryCurrentLevel * 100 / carChargingState.BatteryCapacity;
            if (currentBatteryLevelPercentage < customerPreference.DirectChargingPercentage)
            {
                // Always first charge up to 20% before doing anything else

                var percentageRequiredForMinBatteryLevel = customerPreference.DirectChargingPercentage - currentBatteryLevelPercentage;
                var kWhRequiredForMinBatteryLevel = carChargingState.BatteryCapacity * percentageRequiredForMinBatteryLevel / 100;
                var hoursRequiredForMinBatteryLevel = kWhRequiredForMinBatteryLevel / carChargingState.ChargingPowerKWh;
                var requiredTimeSpanForMinBatteryLevel = TimeSpan.FromHours(Convert.ToDouble(hoursRequiredForMinBatteryLevel));

                var chargeToMinBatteryLevelProfile = new CarChargingProfile
                {
                    StartDateTime = startDateTime,
                    EndDateTime = startDateTime + requiredTimeSpanForMinBatteryLevel,
                    ChargingAllowed = true,
                };

                result.Add(chargeToMinBatteryLevelProfile);

                carChargingState.BatteryCurrentLevel = customerPreference.DirectChargingPercentage;
                startDateTime = startDateTime.Add(requiredTimeSpanForMinBatteryLevel);
                startDateOfWeek = startDateTime.DayOfWeek;
            }

            //reread because it might already next day
            customerPreference = await GetCustomerChargingPreferenceOrThrow(startDateOfWeek);

            var remainingPercentageToCharge = customerPreference.LeavingBatteryLevel - carChargingState.BatteryCurrentLevel;
            if (remainingPercentageToCharge > 0)
            {
                var kWhRequired = carChargingState.BatteryCapacity * remainingPercentageToCharge / 100;
                var hoursRequired = kWhRequired / carChargingState.ChargingPowerKWh;
                var timeSpanRequired = TimeSpan.FromHours(Convert.ToDouble(hoursRequired));

                // in order to keep assignment simple and prevent writing complicated algorithm, I will consider only min rate as the one when we're allowed to charge
                // ideally it should be like:
                // - use as much as possible during low tariff
                // - if it's not enough to meet customer set, take rest of required energy during high tariff
                
                var chargingEndsNextDay = customerPreference.LeavingTimeSpan < startDateTime.TimeOfDay;
                if (!chargingEndsNextDay)
                {
                    var todayChargingTariffs = await GetChargingTariffsOrThrow(startDateOfWeek);
                    todayChargingTariffs = todayChargingTariffs
                        .Where(x => x.Price.Equals(todayChargingTariffs.Min(m => m.Price)))
                        .Where(x => x.StartingFromTimeSpan < customerPreference.LeavingTimeSpan)
                        .OrderBy(x => x.StartingFromTimeSpan)
                        .ToList();

                    foreach (var todayChargingTariff in todayChargingTariffs)
                    {
                        result.CalcProfiles(todayChargingTariff, startDateTime, customerPreference.LeavingTimeSpan, ref timeSpanRequired);
                    }
                }
                else
                {
                    var todayChargingTariffs = await GetChargingTariffsOrThrow(startDateOfWeek);
                    todayChargingTariffs = todayChargingTariffs
                        .Where(x => x.Price.Equals(todayChargingTariffs.Min(m => m.Price)))
                        .Where(x => x.EndingAtTimeSpan > startDateTime.TimeOfDay)
                        .OrderBy(x => x.StartingFromTimeSpan)
                        .ToList();

                    foreach (var todayChargingTariff in todayChargingTariffs)
                    {
                        result.CalcProfiles(todayChargingTariff, startDateTime, startDateTime.Date.AddDays(1).AddSeconds(-1).TimeOfDay, ref timeSpanRequired);
                    }

                    var nextDayStartTime = (startDateTime + timeSpanRequired).Date;
                    var nextDayOfWeek = (startDateTime + timeSpanRequired).DayOfWeek;
                    customerPreference = await GetCustomerChargingPreferenceOrThrow(nextDayOfWeek);

                    var tomorrowChargingTariffs = await GetChargingTariffsOrThrow(nextDayOfWeek);
                    tomorrowChargingTariffs = tomorrowChargingTariffs
                        .Where(x => x.Price.Equals(tomorrowChargingTariffs.Min(m => m.Price)))
                        .Where(x => x.StartingFromTimeSpan < customerPreference.LeavingTimeSpan)
                        .OrderBy(x => x.StartingFromTimeSpan)
                        .ToList();

                    foreach (var tomorrowChargingTariff in tomorrowChargingTariffs)
                    {
                        result.CalcProfiles(tomorrowChargingTariff, nextDayStartTime, customerPreference.LeavingTimeSpan, ref timeSpanRequired);
                    }
                }
            }


            return result;
        }


        public async Task<List<ChargingTariff>> GetChargingTariffsOrThrow(DayOfWeek dayOfWeek)
        {
            // repo should support retrieve by day of week
            var chargingTariffs = (await _chargingTariffsRepo.GetChargingTariffs()).ToList();
            return chargingTariffs.Where(x => x.DayOfWeek.Equals(dayOfWeek)).ToList();
        }


        public async Task<CustomerChargingPreference> GetCustomerChargingPreferenceOrThrow(DayOfWeek dayOfWeek)
        {
            // repo should support retrieve by day of week
            var customerChargingPreferences = (await _customerChargingPreferencesRepo.GetCustomerChargingPreferences()).ToList();
            return customerChargingPreferences.First(x => x.DayOfWeek.Equals(dayOfWeek));
        }
    }

    public static class CalculationHelper
    {
        public static List<CarChargingProfile> CalcProfiles(this List<CarChargingProfile> result, ChargingTariff todayChargingTariff, DateTimeOffset startDateTime, TimeSpan endTimeSpan, ref TimeSpan timeSpanRequiredForFullCharge)
        {
            if (timeSpanRequiredForFullCharge <= TimeSpan.Zero)
            {
                return result;
            }

            var profileStartingFromDateTime = todayChargingTariff.StartingFromTimeSpan < startDateTime.TimeOfDay
                ? startDateTime
                : new DateTimeOffset(startDateTime.Date.Add(todayChargingTariff.StartingFromTimeSpan), TimeSpan.Zero);

            var profileEndingAtDateTime = todayChargingTariff.EndingAtTimeSpan < endTimeSpan
                ? new DateTimeOffset(startDateTime.Date.Add(todayChargingTariff.EndingAtTimeSpan), TimeSpan.Zero)
                : new DateTimeOffset(startDateTime.Date.Add(endTimeSpan), TimeSpan.Zero);

            // in case it required less time than profile lasts - we should decrease charging time to match customer preference and not charge more than required
            timeSpanRequiredForFullCharge -= profileEndingAtDateTime - profileStartingFromDateTime;
            if (timeSpanRequiredForFullCharge < TimeSpan.Zero)
            {
                profileEndingAtDateTime = profileEndingAtDateTime.Add(timeSpanRequiredForFullCharge);
                timeSpanRequiredForFullCharge = TimeSpan.Zero;
            }

            if (profileStartingFromDateTime - result.LastOrDefault()?.EndDateTime > TimeSpan.FromMinutes(1))
            {
                result.Add(new CarChargingProfile
                {
                    StartDateTime = result.Last().EndDateTime,
                    EndDateTime = profileStartingFromDateTime,
                    ChargingAllowed = false
                });
            }

            result.Add(new CarChargingProfile
            {
                StartDateTime = profileStartingFromDateTime,
                EndDateTime = profileEndingAtDateTime,
                ChargingAllowed = true
            });

            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Jedlix.Core.Contracts;
using Jedlix.Models;
using Jedlix.Models.Events;
using Jedlix.Repositories.Contracts;
using Newtonsoft.Json;

namespace AssignmentTestConsole
{
    public class App
    {
        private readonly ICarChargingStatesRepo _carChargingStatesRepo;
        private readonly IChargingTariffsRepo _chargingTariffsRepo;
        private readonly ICustomerChargingPreferencesRepo _customerChargingPreferencesRepo;
        private readonly ICarChargingService _carChargingService;


        public const string Input_EndsSameDay = @"
                                    {
                                        ""startingTime"": ""2023-08-18T02:12:23Z"",
                                        ""userSettings"": {
                                            ""desiredStateOfCharge"": 70,
                                            ""leavingTime"": ""07:50"",
                                            ""directChargingPercentage"": 20,
                                            ""tariffs"":
                                            [
                                                { 
                                                    startTime: ""00:00"",
                                                    endTime: ""02:59"",
                                                    energyPrice: 0.23
                                                },
                                                { 
                                                    startTime: ""03:00"",
                                                    endTime: ""03:59"",
                                                    energyPrice: 0.25
                                                },
                                                { 
                                                    startTime: ""04:00"",
                                                    endTime: ""06:59"",
                                                    energyPrice: 0.23
                                                },
                                                { 
                                                    startTime: ""07:00"",
                                                    endTime: ""19:00"",
                                                    energyPrice: 0.25
                                                },
                                                { 
                                                    startTime: ""19:00"",
                                                    endTime: ""23:59"",
                                                    energyPrice: 0.23
                                                }
                                            ],
                                        },
                                        ""carData"":
                                        {
                                            ""chargePower"": 9.6,
                                            ""batteryCapacity"": 55,
                                            ""currentBatteryLevel"" : 5
                                        }
                                    }";


        public const string Input_EndsNextDay = @"
                                    {
                                        ""startingTime"": ""2023-08-18T22:12:23Z"",
                                        ""userSettings"": {
                                            ""desiredStateOfCharge"": 70,
                                            ""leavingTime"": ""07:50"",
                                            ""directChargingPercentage"": 20,
                                            ""tariffs"":
                                            [
                                                { 
                                                    startTime: ""00:00"",
                                                    endTime: ""02:59"",
                                                    energyPrice: 0.23
                                                },
                                                { 
                                                    startTime: ""03:00"",
                                                    endTime: ""03:59"",
                                                    energyPrice: 0.25
                                                },
                                                { 
                                                    startTime: ""04:00"",
                                                    endTime: ""06:59"",
                                                    energyPrice: 0.23
                                                },
                                                { 
                                                    startTime: ""07:00"",
                                                    endTime: ""19:00"",
                                                    energyPrice: 0.25
                                                },
                                                { 
                                                    startTime: ""19:00"",
                                                    endTime: ""23:59"",
                                                    energyPrice: 0.23
                                                }
                                            ],
                                        },
                                        ""carData"":
                                        {
                                            ""chargePower"": 9.6,
                                            ""batteryCapacity"": 55,
                                            ""currentBatteryLevel"" : 5
                                        }
                                    }";

        public App(
            ICarChargingStatesRepo carChargingStatesRepo,
            IChargingTariffsRepo chargingTariffsRepo,
            ICustomerChargingPreferencesRepo customerChargingPreferencesRepo,
            ICarChargingService carChargingService)
        {
            _carChargingStatesRepo = carChargingStatesRepo;
            _chargingTariffsRepo = chargingTariffsRepo;
            _customerChargingPreferencesRepo = customerChargingPreferencesRepo;
            _carChargingService = carChargingService;
        }

        public async void Run()
        {
            //var input = JsonConvert.DeserializeObject<InputModel>(Input_EndsSameDay);
            var input = JsonConvert.DeserializeObject<InputModel>(Input_EndsNextDay);
            
            for (var dayIndex = 1; dayIndex < 8; dayIndex++)
            {
                var index = dayIndex;

                await _customerChargingPreferencesRepo.AddCustomerChargingPreferences(new List<CustomerChargingPreference>
                {
                    new CustomerChargingPreference
                    {
                        DayOfWeek = (DayOfWeek) index,
                        LeavingTimeSpan = input.UserSettings.LeavingTime,
                        LeavingBatteryLevel = input.UserSettings.DesiredStateOfCharge,
                        DirectChargingPercentage = input.UserSettings.DirectChargingPercentage,
                    }
                });

                await _chargingTariffsRepo.AddChargingTariffs(input.UserSettings.Tariffs.Select(x => new ChargingTariff
                {
                    DayOfWeek = (DayOfWeek) index,
                    StartingFromTimeSpan = x.StartTime,
                    EndingAtTimeSpan = x.EndTime,
                    Price = x.EnergyPrice
                }));
            }

            await _carChargingStatesRepo.SaveCarChargingState(new CarChargingState
            {
                BatteryCapacity = input.CarData.BatteryCapacity,
                BatteryCurrentLevel = input.CarData.CurrentBatteryLevel,
                ChargingPowerKWh = input.CarData.ChargePower,
            });


            await _carChargingService.ProcessStartingChargingEvent(new CarIsStartingChargingEvent
            {
                StartDateTimeOffset = input.StartingTime
            });

            Console.ReadLine();
        }
    }



    public class InputModel
    {
        public DateTimeOffset StartingTime { get; set; }
        public InputUserSettingsModel UserSettings { get; set; }
        public InputCarDataModel CarData { get; set; }
    }


    public class InputUserSettingsModel
    {
        public decimal DesiredStateOfCharge { get; set; }
        public TimeSpan LeavingTime { get; set; }
        public decimal DirectChargingPercentage  { get; set; }
        public List<InputUserChargingTariffModel> Tariffs  { get; set; }
    }

    public class InputUserChargingTariffModel
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public decimal EnergyPrice { get; set; }
    }

    public class InputCarDataModel
    {
        public decimal ChargePower { get; set;}
        public decimal BatteryCapacity { get; set;}
        public decimal CurrentBatteryLevel { get; set;}
    }
}

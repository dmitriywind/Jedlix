using Autofac;
using Jedlix.Core.Contracts;
using Jedlix.Core.Services;
using Jedlix.Repositories;
using Jedlix.Repositories.Contracts;
using Jedlix.ServiceBusPublisher.Messages;
using Jedlix.ServiceBusPublisher;

namespace AssignmentTestConsole
{
    internal class Program
    {
        private static IContainer CompositionRoot()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<App>();

            builder.RegisterType<CarChargingStatesRepo>().As<ICarChargingStatesRepo>();
            builder.RegisterType<ChargingTariffsRepo>().As<IChargingTariffsRepo>();
            builder.RegisterType<CustomerChargingPreferencesRepo>().As<ICustomerChargingPreferencesRepo>();

            builder.RegisterType<ServiceBusMessageService<CarChargingProfileMessage>>().As<IServiceBusMessageService<CarChargingProfileMessage>>();

            builder.RegisterType<CarChargingService>().As<ICarChargingService>();
            builder.RegisterType<ChargingScheduleGenerationService>().As<IChargingScheduleGenerationService>();

            return builder.Build();
        }

        public static void Main()  //Main entry point
        {
            CompositionRoot().Resolve<App>().Run();
        }
    }
}

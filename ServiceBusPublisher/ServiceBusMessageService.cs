using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Jedlix.ServiceBusPublisher
{
    public class ServiceBusMessageService<T> : IServiceBusMessageService<T>
    {
        public async Task SendAsync(T message, string messageId = null)
        {
            var messageBody = JsonConvert.SerializeObject(message);
            Console.WriteLine(messageBody);
        }
    }
}

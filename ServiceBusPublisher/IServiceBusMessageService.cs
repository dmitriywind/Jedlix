using System.Threading.Tasks;

namespace Jedlix.ServiceBusPublisher
{
    public interface IServiceBusMessageService<T>
    {
        Task SendAsync(T message, string messageId = null);
    }
}

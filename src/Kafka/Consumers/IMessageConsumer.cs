namespace Kafka.Consumers
{
    using System.Threading.Tasks;

    public interface IMessageConsumer
    {
        Task Cosume(ConsumerMessage message);
    }
}

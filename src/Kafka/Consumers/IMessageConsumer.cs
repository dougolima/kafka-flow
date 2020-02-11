namespace Kafka.Consumers
{
    using System.Threading.Tasks;
    using Kafka.Configuration.Consumers;

    public interface IMessageConsumer
    {
        MessageContext CreateMessageContext(ConsumerMessage message);

        Task Cosume(MessageContext context);
    }
}

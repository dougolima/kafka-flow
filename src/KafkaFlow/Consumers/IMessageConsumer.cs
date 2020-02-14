namespace KafkaFlow.Consumers
{
    using System.Threading.Tasks;

    public interface IMessageConsumer
    {
        MessageContext CreateMessageContext(ConsumerMessage message);

        Task Cosume(MessageContext context);
    }
}

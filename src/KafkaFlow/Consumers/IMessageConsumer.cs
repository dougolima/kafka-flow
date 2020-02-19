namespace KafkaFlow.Consumers
{
    using System.Threading.Tasks;

    public interface IMessageConsumer
    {
        MessageContext CreateMessageContext(ConsumerMessage message, IOffsetManager offsetManager);

        Task Consume(MessageContext context);
    }
}

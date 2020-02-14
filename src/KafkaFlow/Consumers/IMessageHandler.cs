namespace KafkaFlow.Consumers
{
    using System.Threading.Tasks;

    public interface IMessageHandler<in TMessage> : IMessageHandler
    {
        Task Handle(MessageContext context, TMessage message);
    }

    public interface IMessageHandler
    {
    }
}

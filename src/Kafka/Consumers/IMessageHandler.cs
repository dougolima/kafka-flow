namespace Kafka.Consumers
{
    using System.Threading.Tasks;

    public interface IMessageHandler<in TMessage> : IMessageHandler
    {
        Task Handle(TMessage message, MessageContext context);
    }

    public interface IMessageHandler
    {
    }
}

namespace KafkaFlow.Producers
{
    using System.Threading.Tasks;

    public interface IMessageProducer<TProducer> : IMessageProducer
    {
    }

    public interface IMessageProducer
    {
        Task ProduceAsync(
            string topic,
            string partitionKey,
            object message,
            IMessageHeaders headers = null);

        Task ProduceAsync(
            string partitionKey,
            object message,
            IMessageHeaders headers = null);
    }
}

namespace Kafka.Producers
{
    using System.Collections.Generic;
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
            Dictionary<string, byte[]> headers = null);

        Task ProduceAsync(
            string partitionKey,
            object message,
            Dictionary<string, byte[]> headers = null);
    }
}

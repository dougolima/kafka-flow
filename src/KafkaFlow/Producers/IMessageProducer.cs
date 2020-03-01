namespace KafkaFlow.Producers
{
    using System.Threading.Tasks;

    public interface IMessageProducer<TProducer> : IMessageProducer
    {
    }

    public interface IMessageProducer
    {
        /// <summary>
        /// Produces a new message
        /// </summary>
        /// <param name="topic">The topic where the message wil be produced</param>
        /// <param name="partitionKey">The message partition key raw value</param>
        /// <param name="message">The message raw value</param>
        /// <param name="headers">The message headers</param>
        Task ProduceAsync(
            string topic,
            byte[] partitionKey,
            byte[] message,
            IMessageHeaders headers = null);

        /// <summary>
        /// Produces a new message
        /// </summary>
        /// <param name="topic">The topic where the message wil be produced</param>
        /// <param name="partitionKey">The message partition key, the value will be encoded suing UTF8</param>
        /// <param name="message">The message object to be encoded or serialized</param>
        /// <param name="headers">The message headers</param>
        /// <returns></returns>
        Task ProduceAsync(
            string topic,
            string partitionKey,
            object message,
            IMessageHeaders headers = null);

        /// <summary>
        /// Produces a new message in the configured default topic
        /// </summary>
        /// <param name="partitionKey">The message partition key, the value will be encoded suing UTF8</param>
        /// <param name="message">The message object to be encoded or serialized</param>
        /// <param name="headers">The message headers</param>
        /// <returns></returns>
        Task ProduceAsync(
            string partitionKey,
            object message,
            IMessageHeaders headers = null);
    }
}

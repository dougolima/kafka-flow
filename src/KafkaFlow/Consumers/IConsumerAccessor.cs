namespace KafkaFlow.Consumers
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides access to configured consumers
    /// </summary>
    public interface IConsumerAccessor
    {
        /// <summary>
        /// Get a Consumer by it´s name
        /// </summary>
        /// <param name="name">The name defined in the consumer configuration</param>
        /// <returns></returns>
        IMessageConsumer GetConsumer(string name);

        /// <summary>
        /// Returns all configured consumers
        /// </summary>
        IEnumerable<IMessageConsumer> All { get; }
    }
}

namespace KafkaFlow.Extensions
{
    using System.Collections.Generic;
    using Confluent.Kafka;

    internal static class MessageHeaderExtensions
    {
        internal static Headers ToKafkaHeaders(this Dictionary<string, byte[]> headers)
        {
            var kafkaHeaders = new Headers();

            foreach (var header in headers)
            {
                kafkaHeaders.Add(header.Key, header.Value);
            }

            return kafkaHeaders;
        }
    }
}

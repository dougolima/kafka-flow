namespace Kafka.Samples.Consumer
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Kafka.Consumers;

    internal class RawConsumerHandler : IMessageHandler<byte[]>
    {
        public Task Handle(MessageContext context, byte[] message)
        {
            Console.WriteLine("Hit raw consumer. Partition Key: {0}", Encoding.UTF8.GetString(context.Message.Key));
            return Task.CompletedTask;
        }
    }
}

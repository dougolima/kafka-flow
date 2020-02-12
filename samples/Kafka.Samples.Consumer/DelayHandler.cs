namespace Kafka.Samples.Consumer
{
    using System.Threading.Tasks;
    using Kafka.Consumers;
    using Kafka.Samples.Common;

    public class DelayHandler : IMessageHandler<TestMessage>
    {
        public Task Handle(TestMessage message, MessageContext context)
        {
            return Task.Delay(10);
        }
    }
}

namespace KafkaFlow.Samples.Consumer
{
    using System.Threading.Tasks;
    using KafkaFlow;
    using KafkaFlow.Consumers;
    using KafkaFlow.Samples.Common;

    public class DelayHandler : IMessageHandler<TestMessage>
    {
        public Task Handle(MessageContext context, TestMessage message)
        {
            return Task.Delay(10);
        }
    }
}

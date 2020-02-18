namespace KafkaFlow.Samples.Consumer
{
    using System.Threading.Tasks;
    using KafkaFlow;
    using KafkaFlow.Consumers;
    using KafkaFlow.Samples.Common;

    public class DelayHandler : IMessageHandler<TestMessage>
    {
        public async Task Handle(MessageContext context, TestMessage message)
        {
            try
            {
                await Task.Delay(10);
            }
            finally
            {
                context.StoreOffset();
            }
        }
    }
}

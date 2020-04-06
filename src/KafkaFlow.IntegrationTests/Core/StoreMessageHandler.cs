namespace KafkaFlow.IntegrationTests.Core
{
    using System.Threading.Tasks;
    using KafkaFlow.TypedHandler;

    public class StoreMessageHandler : IMessageHandler<TestMessage1>, IMessageHandler<TestMessage2>
    {
        public Task Handle(IMessageContext context, TestMessage1 message)
        {
            MessageStorage.Add(message);
            return Task.CompletedTask;
        }

        public Task Handle(IMessageContext context, TestMessage2 message)
        {
            MessageStorage.Add(message);
            return Task.CompletedTask;
        }
    }
}
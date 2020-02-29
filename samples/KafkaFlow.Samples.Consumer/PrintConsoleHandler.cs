namespace KafkaFlow.Samples.Consumer
{
    using System;
    using System.Threading.Tasks;
    using KafkaFlow;
    using KafkaFlow.Consumers;
    using KafkaFlow.Samples.Common;
    using KafkaFlow.TypedHandler;

    public class PrintConsoleHandler : IMessageHandler<TestMessage>
    {
        public Task Handle(IMessageContext context, TestMessage message)
        {
            Console.WriteLine(message.Text);
            return Task.CompletedTask;
        }
    }
}

namespace KafkaFlow.Samples.Consumer
{
    using System;
    using System.Threading.Tasks;
    using KafkaFlow;
    using KafkaFlow.Consumers;
    using KafkaFlow.Samples.Common;

    public class PrintConsoleHandler : IMessageHandler<TestMessage>
    {
        public Task Handle(MessageContext context, TestMessage message)
        {
            Console.WriteLine(message.Text);
            return Task.CompletedTask;
        }
    }
}

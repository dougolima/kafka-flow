namespace KafkaFlow.Samples.Consumer
{
    using System;
    using System.Threading.Tasks;
    using KafkaFlow;
    using KafkaFlow.Samples.Common;
    using KafkaFlow.TypedHandler;

    public class PrintConsoleHandler
        : IMessageHandler<TestMessage>,
            IMessageHandler<TestMessage2>
    {
        public Task Handle(IMessageContext context, TestMessage message)
        {
            Console.WriteLine(message.Text);
            return Task.CompletedTask;
        }

        public Task Handle(IMessageContext context, TestMessage2 message)
        {
            Console.WriteLine(message.Value);
            return Task.CompletedTask;
        }
    }
}

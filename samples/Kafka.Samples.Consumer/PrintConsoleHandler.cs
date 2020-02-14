namespace Kafka.Samples.Consumer
{
    using System;
    using System.Threading.Tasks;
    using Kafka.Consumers;
    using Kafka.Samples.Common;

    public class PrintConsoleHandler : IMessageHandler<TestMessage>
    {
        public Task Handle(MessageContext context, TestMessage message)
        {
            Console.WriteLine(message.Text);
            return Task.CompletedTask;
        }
    }
}

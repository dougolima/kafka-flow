namespace Kafka.Samples.Consumer
{
    using System;
    using System.Threading.Tasks;
    using Kafka.Consumers;
    using Kafka.Samples.Common;

    public class PrintConsoleHandler : IMessageHandler<TestMessage>
    {
        public Task Handle(TestMessage message, MessageContext context)
        {
            Console.WriteLine(message.Text);
            return Task.CompletedTask;
        }
    }
}

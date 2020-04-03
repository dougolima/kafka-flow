namespace KafkaFlow.IntegrationTests.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public static class MessageStorage
    {
        private static readonly ConcurrentBag<ITestMessage> messages = new ConcurrentBag<ITestMessage>();

        public static void Add(ITestMessage message)
        {
            messages.Add(message);
        }

        public static async Task AssertMessageAsync(ITestMessage message)
        {
            var start = DateTime.Now;

            while (!MessageArrived(message))
            {
                if (DateTime.Now.Subtract(start).Seconds > 60)
                {
                    Assert.Fail("Message not received");
                    return;
                }

                await Task.Delay(100);
            }
        }

        private static bool MessageArrived(ITestMessage message)
        {
            return messages.Any(x => x.Id == message.Id && x.Value == message.Value);
        }

        public static void Clear()
        {
            messages.Clear();
        }
    }
}

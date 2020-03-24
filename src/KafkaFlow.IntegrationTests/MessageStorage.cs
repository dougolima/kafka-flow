namespace KafkaFlow.IntegrationTests
{
    using System.Collections.Generic;

    public static class MessageStorage
    {
        public static List<ITestMessage> Messages { get; set; } = new List<ITestMessage>();
    }
}

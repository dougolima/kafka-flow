namespace KafkaFlow
{
    using System;

    public class NullLogHandler : ILogHandler
    {
        public void Error(string message, Exception ex, ConsumerMessage messageData)
        {
        }

        public void Info(string message, ConsumerMessage messageData)
        {
        }

        public void Info(string message, object data)
        {
        }
    }
}

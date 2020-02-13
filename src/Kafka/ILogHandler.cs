namespace Kafka
{
    using System;

    public interface ILogHandler
    {
        void Error(string message, Exception ex, ConsumerMessage messageData);

        void Info(string message, ConsumerMessage messageData);

        void Info(string message, object data);
    }
}

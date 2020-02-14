namespace KafkaFlow.Samples.Common
{
    using System;
    using System.Text.Json;
    using KafkaFlow;

    public class ConsoleLogHandler : ILogHandler
    {
        public void Error(string message, Exception ex, ConsumerMessage messageData)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Kafka Error: {message} | Exception: {JsonSerializer.Serialize(ex)}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void Info(string message, ConsumerMessage messageData)
        {
            Console.WriteLine($"Kafka Info: {message}");
        }

        public void Info(string message, object data)
        {
            Console.WriteLine($"Kafka Info: {message} | Data: {JsonSerializer.Serialize(data)}");
        }
    }
}

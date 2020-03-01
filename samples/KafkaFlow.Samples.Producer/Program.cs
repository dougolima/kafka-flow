namespace KafkaFlow.Samples.Producer
{
    using System;
    using KafkaFlow.Compressor;
    using KafkaFlow.Compressor.Gzip;
    using KafkaFlow.Extensions;
    using KafkaFlow.Samples.Common;
    using KafkaFlow.Serializer;
    using KafkaFlow.Serializer.ProtoBuf;
    using Microsoft.Extensions.DependencyInjection;

    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddKafka(
                kafka => kafka
                    .UseLogHandler<ConsoleLogHandler>()
                    .AddCluster(
                        cluster => cluster
                            .WithBrokers(new[] { "localhost:9092" })
                            .AddProducer<PrintConsoleProducer>(
                                producer => producer
                                    .DefaultTopic("test-topic")
                                    .UseSerializerMiddleware<ProtobufMessageSerializer, SampleMessageTypeResolver>()
                                    .UseCompressorMiddleware<GzipMessageCompressor>()
                                    .WithAcks(Acks.All)
                                )
                    )
            );

            services.AddTransient<PrintConsoleProducer>();

            var provider = services.BuildServiceProvider();

            var bus = provider.UseKafka();

            bus.StartAsync().GetAwaiter().GetResult();

            var printConsole = provider.GetService<PrintConsoleProducer>();

            while (true)
            {
                Console.Write("Number of messages to produce: ");
                var count = int.Parse(Console.ReadLine());

                for (var i = 0; i < count; i++)
                {
                    printConsole.ProduceAsync(new TestMessage { Text = $"Message: {Guid.NewGuid()}" });
                }
            }
        }
    }
}

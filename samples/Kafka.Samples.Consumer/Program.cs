namespace Kafka.Samples.Consumer
{
    using System.Threading;
    using Kafka.Extensions;
    using Kafka.Samples.Common;
    using Kafka.Serializer.ProtoBuf;
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
                            .AddRawConsumer<RawConsumerHandler>(
                                consumer => consumer
                                    .Topic("test-topic")
                                    .WithGroupId("test-group-raw")
                                    .WithMaxWorkersCount(10)
                                    .WithBufferSize(100)
                                )
                            .AddTypedHandlerConsumer(
                                consumer => consumer
                                    .Topic("test-topic")
                                    .WithGroupId("test-group")
                                    .WithBufferSize(1000)
                                    .UseMiddleware<MessageTypeResolverMiddleware>()
                                    .UseSerializer<ProtobufMessageSerializer>()
                                    .WithNoCompressor()
                                    .ScanHandlersFromAssemblyOf<PrintConsoleHandler>()
                                    .WithMaxWorkersCount(30)
                            )
                    )
            );

            var provider = services.BuildServiceProvider();

            var bus = provider.UseKafka();

            bus.StartAsync().GetAwaiter().GetResult();

            while (true)
            {
                Thread.Sleep(10000);
            }
        }
    }
}

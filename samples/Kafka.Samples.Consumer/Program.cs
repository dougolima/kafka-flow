namespace Kafka.Samples.Consumer
{
    using System.Threading;
    using System.Threading.Tasks;
    using Kafka.Extensions;
    using Kafka.Samples.Common;
    using Kafka.Serializer.ProtoBuf;
    using Microsoft.Extensions.DependencyInjection;

    class Program
    {
        static async Task Main(string[] args)
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

            await bus.StartAsync();

            while (true)
            {
                await Task.Delay(10000);
            }
        }
    }
}

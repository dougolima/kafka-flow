namespace Kafka.Samples.Consumer
{
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
                                    .WithGroupId("raw-handler")
                                    .WithMaxWorkersCount(10)
                                    .WithBufferSize(100)
                                )
                            .AddTypedHandlerConsumer(
                                consumer => consumer
                                    .Topic("test-topic")
                                    .WithGroupId("print-console-handler")
                                    .WithBufferSize(100)
                                    .UseMiddleware<MessageTypeResolverMiddleware>()
                                    .UseSerializer<ProtobufMessageSerializer>()
                                    .WithNoCompressor()
                                    .AddHandlers(new[] { typeof(PrintConsoleHandler) })
                                    .WithMaxWorkersCount(10)
                            )
                            .AddTypedHandlerConsumer(
                                consumer => consumer
                                    .Topic("test-topic")
                                    .WithGroupId("delay-handler")
                                    .WithBufferSize(100)
                                    .UseMiddleware<MessageTypeResolverMiddleware>()
                                    .UseSerializer<ProtobufMessageSerializer>()
                                    .WithNoCompressor()
                                    .AddHandlers(new[] { typeof(DelayHandler) })
                                    .WithMaxWorkersCount(10)
                                    .WithAutoCommitIntervalMs(1000)
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

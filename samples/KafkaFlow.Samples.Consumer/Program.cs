namespace KafkaFlow.Samples.Consumer
{
    using System.Threading;
    using KafkaFlow.Extensions;
    using KafkaFlow.Samples.Common;
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
                            .AddRawConsumer<RawConsumerHandler>(
                                consumer => consumer
                                    .Topic("test-topic")
                                    .WithGroupId("raw-handler")
                                    .WithWorkersCount(10)
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
                                    .WithWorkersCount(10)
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
                                    .WithWorkersCount(10)
                                    .WithManualStoreOffsets()
                                    .WithAutoCommitIntervalMs(1000)
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

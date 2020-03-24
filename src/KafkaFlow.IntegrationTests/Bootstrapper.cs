namespace KafkaFlow.IntegrationTests
{
    using System;
    using KafkaFlow.Compressor;
    using KafkaFlow.Compressor.Gzip;
    using KafkaFlow.Extensions;
    using KafkaFlow.Serializer;
    using KafkaFlow.Serializer.Json;
    using KafkaFlow.Serializer.ProtoBuf;
    using KafkaFlow.TypedHandler;
    using Microsoft.Extensions.DependencyInjection;

    public class Bootstrapper
    {
        private static readonly Lazy<IServiceProvider> lazyProvider = new Lazy<IServiceProvider>(SetupProvider);

        public static IServiceProvider GetServiceProvider() => lazyProvider.Value;

        private static IServiceProvider SetupProvider()
        {
            var services = new ServiceCollection();

            services.AddKafka(
                kafka => kafka
                    .AddCluster(
                        cluster => cluster
                            .WithBrokers(new[] { "localhost:9092" })
                            .AddConsumer(
                                consumer => consumer
                                    .Topic("test-gzip-protobuf")
                                    .WithGroupId("test")
                                    .WithBufferSize(100)
                                    .WithWorkersCount(10)
                                    .WithAutoOffsetReset(AutoOffsetReset.Latest)
                                    .UseCompressorMiddleware<GzipMessageCompressor>()
                                    .UseSerializerMiddleware<ProtobufMessageSerializer, TestMessageTypeResolver>()
                                    .UseTypedHandlers(
                                        handlers =>
                                            handlers
                                                .WithHandlerLifetime(ServiceLifetime.Singleton)
                                                .AddHandler<StoreMessageHandler>())
                            )
                            .AddConsumer(
                                consumer => consumer
                                    .Topic("test-gzip-json")
                                    .WithGroupId("test")
                                    .WithBufferSize(100)
                                    .WithWorkersCount(10)
                                    .WithAutoOffsetReset(AutoOffsetReset.Latest)
                                    .UseCompressorMiddleware<GzipMessageCompressor>()
                                    .UseSerializerMiddleware<JsonMessageSerializer, TestMessageTypeResolver>()
                                    .UseTypedHandlers(
                                        handlers =>
                                            handlers
                                                .WithHandlerLifetime(ServiceLifetime.Singleton)
                                                .AddHandler<StoreMessageHandler>())
                            )
                            .AddProducer<JsonProducer>(
                                producer =>
                                    producer
                                        .DefaultTopic("test-gzip-json")
                                        .UseSerializerMiddleware<JsonMessageSerializer, TestMessageTypeResolver>()
                                        .UseCompressorMiddleware<GzipMessageCompressor>()
                            )
                            .AddProducer<ProtobufProducer>(
                                producer =>
                                    producer
                                        .DefaultTopic("test-gzip-protobuf")
                                        .UseSerializerMiddleware<ProtobufMessageSerializer, TestMessageTypeResolver>()
                                        .UseCompressorMiddleware<GzipMessageCompressor>()
                            )
                    )
            );

            var provider = services.BuildServiceProvider();

            var bus = provider.UseKafka();

            bus.StartAsync().GetAwaiter().GetResult();

            return provider;
        }
    }

    internal class ProtobufProducer
    {
    }

    internal class JsonProducer
    {
    }
}

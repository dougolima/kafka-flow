namespace KafkaFlow.IntegrationTests.Core
{
    using System;
    using System.Threading;
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
        private const string JsonTopicName = "test-gzip-json";
        private const string ProtobufTopicName = "test-gzip-protobuf";

        private static readonly Lazy<IServiceProvider> lazyProvider = new Lazy<IServiceProvider>(SetupProvider);

        public static IServiceProvider GetServiceProvider() => lazyProvider.Value;

        private static IServiceProvider SetupProvider()
        {
            var services = new ServiceCollection();

            services.AddKafka(
                kafka => kafka
                    .UseLogHandler<TraceLoghandler>()
                    .AddCluster(
                        cluster => cluster
                            .WithBrokers(new[] { "localhost:9092" })
                            .AddConsumer(
                                consumer => consumer
                                    .Topic(ProtobufTopicName)
                                    .WithGroupId("test-protobuf")
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
                                    .Topic(JsonTopicName)
                                    .WithGroupId("test-json")
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
                                        .DefaultTopic(JsonTopicName)
                                        .UseSerializerMiddleware<JsonMessageSerializer, TestMessageTypeResolver>()
                                        .UseCompressorMiddleware<GzipMessageCompressor>()
                            )
                            .AddProducer<ProtobufProducer>(
                                producer =>
                                    producer
                                        .DefaultTopic(ProtobufTopicName)
                                        .UseSerializerMiddleware<ProtobufMessageSerializer, TestMessageTypeResolver>()
                                        .UseCompressorMiddleware<GzipMessageCompressor>()
                            )
                    )
            );

            services.AddSingleton<JsonProducer>();
            services.AddSingleton<ProtobufProducer>();

            var provider = services.BuildServiceProvider();

            var bus = provider.UseKafka();

            bus.StartAsync().GetAwaiter().GetResult();

            //Wait partition assignment
            Thread.Sleep(5000);

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

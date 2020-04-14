namespace KafkaFlow.IntegrationTests.Core
{
    using System;
    using System.IO;
    using System.Threading;
    using KafkaFlow.Compressor;
    using KafkaFlow.Compressor.Gzip;
    using KafkaFlow.Extensions;
    using KafkaFlow.Serializer;
    using KafkaFlow.Serializer.Json;
    using KafkaFlow.Serializer.ProtoBuf;
    using KafkaFlow.TypedHandler;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public class Bootstrapper
    {
        private const string JsonTopicName = "test-gzip-json";
        private const string JsonTopic2Name = "test-gzip-json2";
        private const string ProtobufTopicName = "test-gzip-protobuf";

        private static readonly Lazy<IServiceProvider> lazyProvider = new Lazy<IServiceProvider>(SetupProvider);

        public static IServiceProvider GetServiceProvider() => lazyProvider.Value;

        private static IServiceProvider SetupProvider()
        {
            var builder = Host
                .CreateDefaultBuilder()
                .ConfigureAppConfiguration(
                    (builderContext, config) =>
                    {
                        var basePath = Directory.GetCurrentDirectory();

                        config
                            .SetBasePath(basePath)
                            .AddJsonFile(
                                "conf/appsettings.json",
                                false,
                                true)
                            .AddEnvironmentVariables();
                    })
                .ConfigureServices(SetupServices)
                .UseDefaultServiceProvider(
                    (context, options) =>
                    {
                        options.ValidateScopes = true;
                        options.ValidateOnBuild = true;
                    });

            var host = builder.Build();

            var bus = host.Services.UseKafka();

            bus.StartAsync().GetAwaiter().GetResult();

            //Wait partition assignment
            Thread.Sleep(5000);

            return host.Services;
        }

        private static void SetupServices(HostBuilderContext context, IServiceCollection services)
        {
            var brokers = context.Configuration.GetValue<string>("Kafka:Brokers");

            services.AddKafka(
                kafka => kafka
                    .UseLogHandler<TraceLoghandler>()
                    .AddCluster(
                        cluster => cluster
                            .WithBrokers(brokers.Split(';'))
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
                                    .Topics(JsonTopicName, JsonTopic2Name)
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
                            .AddProducer<JsonProducer2>(
                                producer =>
                                    producer
                                        .DefaultTopic(JsonTopic2Name)
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
        }
    }

    internal class ProtobufProducer
    {
    }

    internal class JsonProducer
    {
    }

    internal class JsonProducer2
    {
    }
}

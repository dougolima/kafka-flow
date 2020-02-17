namespace KafkaFlow.Configuration.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Confluent.Kafka;
    using KafkaFlow.Extensions;
    using Microsoft.Extensions.DependencyInjection;

    public abstract class ConsumerConfigurationBuilder<TBuilder>
        : IConsumerConfigurationBuilder
        where TBuilder : ConsumerConfigurationBuilder<TBuilder>
    {
        private readonly IServiceCollection services;

        private string topic;
        private string groupId;
        private AutoOffsetReset? autoOffsetReset;
        private int? autoCommitIntervalMs;
        private int? maxPollIntervalMs;
        private int workersCount;
        private int bufferSize;

        private readonly List<MiddlewareDefinition> middlewares = new List<MiddlewareDefinition>();

        protected ConsumerConfigurationBuilder(IServiceCollection services)
        {
            this.services = services;
        }

        public TBuilder Topic(string topic)
        {
            this.topic = topic;
            return (TBuilder)this;
        }

        public TBuilder WithGroupId(string groupId)
        {
            this.groupId = groupId;
            return (TBuilder)this;
        }

        public TBuilder WithAutoOffsetReset(AutoOffsetReset autoOffsetReset)
        {
            this.autoOffsetReset = autoOffsetReset;
            return (TBuilder)this;
        }

        public TBuilder WithAutoCommitIntervalMs(int autoCommitIntervalMs)
        {
            this.autoCommitIntervalMs = autoCommitIntervalMs;
            return (TBuilder)this;
        }

        public TBuilder WithMaxPollIntervalMs(int maxPollIntervalMs)
        {
            this.maxPollIntervalMs = maxPollIntervalMs;
            return (TBuilder)this;
        }

        public TBuilder WithWorkersCount(int maxWorkersCount)
        {
            this.workersCount = maxWorkersCount;
            return (TBuilder)this;
        }

        public TBuilder WithBufferSize(int size)
        {
            this.bufferSize = size;
            return (TBuilder)this;
        }

        public TBuilder UseMiddleware<TMiddleware>(Action<TMiddleware> configurator)
            where TMiddleware : IMessageMiddleware
        {
            this.middlewares.Add(new MiddlewareDefinition(
                typeof(TMiddleware),
                middleware => configurator((TMiddleware)middleware)));

            return (TBuilder)this;
        }

        public TBuilder UseMiddleware<TMiddleware>()
            where TMiddleware : IMessageMiddleware
        {
            return this.UseMiddleware<TMiddleware>(configurator => { });
        }

        public virtual ConsumerConfiguration Build(ClusterConfiguration clusterConfiguration)
        {
            var combinedMiddlewares = clusterConfiguration.ConsumersMiddlewares.Concat(this.middlewares);

            var configuration = new ConsumerConfiguration(
                clusterConfiguration,
                this.topic,
                this.groupId,
                this.workersCount,
                this.bufferSize,
                combinedMiddlewares)
            {
                AutoOffsetReset = this.autoOffsetReset,
                AutoCommitIntervalMs = this.autoCommitIntervalMs,
                MaxPollIntervalMs = this.maxPollIntervalMs
            };

            this.services.AddMiddlewares(configuration.Middlewares);

            return configuration;
        }
    }
}

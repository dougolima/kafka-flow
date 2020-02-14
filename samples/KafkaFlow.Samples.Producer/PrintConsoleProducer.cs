﻿namespace KafkaFlow.Samples.Producer
{
    using System;
    using System.Threading.Tasks;
    using KafkaFlow.Producers;

    internal class PrintConsoleProducer
    {
        private readonly IMessageProducer<PrintConsoleProducer> producer;

        public PrintConsoleProducer(IMessageProducer<PrintConsoleProducer> producer)
        {
            this.producer = producer;
        }

        public Task ProduceAsync(object message)
        {
            return this.producer.ProduceAsync(
                Guid.NewGuid().ToString(),
                message);
        }
    }
}
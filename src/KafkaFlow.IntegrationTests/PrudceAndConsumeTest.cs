namespace KafkaFlow.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoFixture;
    using KafkaFlow.IntegrationTests.Core;
    using KafkaFlow.Producers;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PrudceAndConsumeTest
    {
        private IServiceProvider provider;

        private readonly Fixture fixture = new Fixture();

        [TestInitialize]
        public void Setup()
        {
            this.provider = Bootstrapper.GetServiceProvider();
            MessageStorage.Clear();
        }

        [TestMethod]
        public async Task JsonMessageTest()
        {
            // Arrange
            var producer = this.provider.GetRequiredService<IMessageProducer<JsonProducer>>();
            var messages = this.CreatesMessages();

            // Act
            foreach (var message in messages)
            {
                await producer.ProduceAsync(message.Id.ToString(), message);
            }

            // Assert
            foreach (var message in messages)
            {
                await MessageStorage.AssertMessageAsync(message);
            }
        }

        [TestMethod]
        public async Task ProtobufMessageTest()
        {
            // Arrange
            var producer = this.provider.GetRequiredService<IMessageProducer<ProtobufProducer>>();
            var messages = this.CreatesMessages();

            // Act
            foreach (var message in messages)
            {
                await producer.ProduceAsync(message.Id.ToString(), message);
            }

            // Assert
            foreach (var message in messages)
            {
                await MessageStorage.AssertMessageAsync(message);
            }
        }

        private List<ITestMessage> CreatesMessages()
        {
            var messages = new List<ITestMessage>();

            for (var i = 0; i < 100; i++)
            {
                messages.Add(this.fixture.Create<TestMessage1>());
                messages.Add(this.fixture.Create<TestMessage2>());
            }

            return messages;
        }
    }
}

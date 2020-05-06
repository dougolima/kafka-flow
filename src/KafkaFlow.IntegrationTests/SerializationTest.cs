namespace KafkaFlow.IntegrationTests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.Handlers;
    using Core.Messages;
    using Core.Middlewares;
    using Core.Middlewares.Producers;
    using KafkaFlow.Producers;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SerializationTest
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
            var messages = this.fixture.CreateMany<TestMessage1>(10).ToList();

            // Act
            await Task.WhenAll(messages.Select(m => producer.ProduceAsync(m.Id.ToString(), m)));
            

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
            var messages = this.fixture.CreateMany<TestMessage1>(10).ToList();

            // Act
            await Task.WhenAll(messages.Select(m => producer.ProduceAsync(m.Id.ToString(), m)));
            

            // Assert
            foreach (var message in messages)
            {
                await MessageStorage.AssertMessageAsync(message);
            }
        }
    }
}

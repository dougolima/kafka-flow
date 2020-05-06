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
    public class CompressionSerializationTest
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
        public async Task JsonGzipMessageTest()
        {
            // Arrange
            var producer = this.provider.GetRequiredService<IMessageProducer<JsonGzipProducer>>();
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
        public async Task ProtoBufGzipMessageTest()
        {
            // Arrange
            var producer = this.provider.GetRequiredService<IMessageProducer<ProtobufGzipProducer>>();
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

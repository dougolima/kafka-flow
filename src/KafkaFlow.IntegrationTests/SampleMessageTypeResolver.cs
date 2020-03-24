namespace KafkaFlow.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using KafkaFlow.Serializer;

    public class TestMessageTypeResolver : IMessageTypeResolver
    {
        private readonly Dictionary<string, Type> messageTypes = new Dictionary<string, Type>()
        {
            [typeof(TestMessage1).FullName] = typeof(TestMessage1),
            [typeof(TestMessage2).FullName] = typeof(TestMessage2),
        };

        public Type OnConsume(IMessageContext context)
        {
            var messageTypeName = context.Headers.GetString("Message-Type");

            if (!string.IsNullOrWhiteSpace(messageTypeName) &&
                this.messageTypes.TryGetValue(messageTypeName, out var messageType))
            {
                return messageType;
            }

            return null;
        }

        public void OnProduce(IMessageContext context)
        {
            var messageTypeName = context.Message.GetType().FullName;

            context.Headers.Add("Message-Type", Encoding.UTF8.GetBytes(messageTypeName));
        }
    }
}

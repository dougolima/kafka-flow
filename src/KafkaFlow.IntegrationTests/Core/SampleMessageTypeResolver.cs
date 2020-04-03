namespace KafkaFlow.IntegrationTests.Core
{
    using System;
    using System.Text;
    using KafkaFlow.Serializer;

    public class TestMessageTypeResolver : IMessageTypeResolver
    {
        public Type OnConsume(IMessageContext context)
        {
            var typeName = context.Headers.GetString("Message-Type");

            return Type.GetType(typeName);
        }

        public void OnProduce(IMessageContext context)
        {
            var messageTypeName = context.Message.GetType().FullName;

            context.Headers.Add("Message-Type", Encoding.UTF8.GetBytes(messageTypeName));
        }
    }
}

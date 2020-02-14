namespace KafkaFlow
{
    using System.Collections.Generic;

    public interface IMessage
    {
        byte[] Key { get; }

        byte[] Value { get; }

        Dictionary<string, byte[]> Headers { get; }
    }
}

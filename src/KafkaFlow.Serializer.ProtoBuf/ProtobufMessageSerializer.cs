namespace KafkaFlow.Serializer.ProtoBuf
{
    using System;
    using System.IO;
    using Serializer = global::ProtoBuf.Serializer;

    public class ProtobufMessageSerializer : IMessageSerializer
    {
        public byte[] Serialize(object obj)
        {
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, obj);

                return stream.ToArray();
            }
        }

        public object Deserialize(byte[] data, Type type)
        {
            using (var stream = new MemoryStream(data))
            {
                return Serializer.Deserialize(type, stream);
            }
        }
    }
}

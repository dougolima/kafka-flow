namespace Kafka
{
    using System;

    public interface IMessageSerializer
    {
        byte[] Serialize(object obj);

        object Desserialize(byte[] data, Type type);
    }
}
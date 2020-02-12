namespace Kafka.Consumers
{
    using System.Linq;

    public class SumWorkerDistribuitionStrategy : IWorkerDistribuitionStrategy
    {
        public int Distribute(byte[] messageKey, int workersCount)
        {
            return messageKey.Sum(x => x) % workersCount;
        }
    }
}
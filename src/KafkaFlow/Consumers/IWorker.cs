namespace KafkaFlow.Consumers
{
    using System;

    public interface IWorker
    {
        int Id { get; }

        void OnTaskFinished(Action handler);
    }
}

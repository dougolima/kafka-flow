namespace KafkaFlow.IntegrationTests.Core
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class TestMessage1 : ITestMessage
    {
        [DataMember(Order = 1)]
        public Guid Id { get; set; }

        [DataMember(Order = 2)]
        public string Value { get; set; }
    }

    [DataContract]
    public class TestMessage2 : ITestMessage
    {
        [DataMember(Order = 1)]
        public Guid Id { get; set; }

        [DataMember(Order = 2)]
        public string Value { get; set; }
    }

    public interface ITestMessage
    {
        Guid Id { get; set; }

        string Value { get; set; }
    }
}

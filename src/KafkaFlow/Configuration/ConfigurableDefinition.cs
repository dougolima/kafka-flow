namespace KafkaFlow.Configuration
{
    using System;

    public class ConfigurableDefinition<TDefinition>
    {
        public Type Type { get; }

        public Action<TDefinition, IServiceProvider> Configurator { get; }

        public ConfigurableDefinition(Type type, Action<TDefinition, IServiceProvider> configurator)
        {
            this.Type = type;
            this.Configurator = configurator;
        }
    }
}

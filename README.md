# KafkaFlow

A flexible framewrok to process Kafka messages with multithreading, middlewares, serialization, compression, typed handlers and message order support.

## Features

- Fluent configuration
- In memory message buffering
- Improve client code testability
- Multiple cluster support
- Multiple workers consuming the same topic (limited by the machine resources)
- Message order guarantee for the same partition key
- Multiple consumer groups in the same topic
- Multiple topics in the same consumer
- Different work distribution strategies
- Middleware support for consumers and producers implementing `IMessageMiddleware` interface
- Serialization middleware (ProtoBuf and Json are shipped with the framework but they have different nuget packages, you can support custom serialization using `IMessageSerializer` interface)
- Compression middleware (Gzip is shipped with the framework but it has a different nuget package, you can support custom compressions using `IMessageCompressor` interface)
- Graceful shutdown (waits for the message processor ends to shutdown)
- Store message offset only when message processing ends, avoiding message loss (or you can store it manually)

#### What can we do with Middlewares?

- Read or write message headers
- Ignore messages
- Manipulate the message
- Custom error handling and retry policies
- Monitoring and performance measurement
- Tracing
- Maintain compatibility with other frameworks
- And so on...

## Installation

You should install [KafkaFlow with NuGet](https://www.nuget.org/packages/KafkaFlow):

    Install-Package KafkaFlow
    
Or via the .NET Core command line interface:

    dotnet add package KafkaFlow

Either commands, from Package Manager Console or .NET Core CLI, will download and install KafkaFlow and all required dependencies.

## Usage

See samples folder to see Consumer and Consumer samples

## Contributing

1. Fork this repository
2. Follow project guidelines
3. Do your stuff
4. Open a merge request using the template

## License

[MIT](LICENSE)

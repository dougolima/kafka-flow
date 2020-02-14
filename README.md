## Features

- Fluent configuration
- Serialization support (ProtoBuf and Json are shipped with the framework but they have different nuget packages, you can support custom serialization using `IMessageSerializer` interface)
- Compression support (Gzip is shipped with the framework but it has a different nuget package, you can support custom compressions using `IMessageCompressor` interface)
- In memory message buffering
- Improve client code testability
- Multiple cluster support
- Multiple workers consuming the same topic (limited by the machine resources)
- Message order guarantee for the same partition key
- Multiple consumer groups in the same topic
- Middleware support for consumers and producers implementing `IMessageMiddleware` interface
- Graceful shutdown (waits for the message processor ends to shutdown)
- Store message offset only when message processing ends, avoiding message loss


### What can we do with Middlewares?

- Read or write message headers
- Ignore messages
- Change serialization and compressor in runtime
- Custom error handling and retry policies
- Monitoring and performance measurement
- Tracing
- Maintain compatibility with other frameworks
- They can be reused in different applications
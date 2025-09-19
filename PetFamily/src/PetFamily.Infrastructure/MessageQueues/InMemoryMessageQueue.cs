using PetFamily.Application.MessageQueue;
using System.Threading.Channels;

namespace PetFamily.Infrastructure.MessageQueues;

public class InMemoryMessageQueue<TMessage> : IMessageQueue<TMessage>
{
    //исп Unbounded - канал без ограничения на writer и reader 
    private readonly Channel<TMessage>_channel = Channel.CreateUnbounded<TMessage>();

    public async Task WriteAsync(TMessage message, CancellationToken cancellationToken = default)
    {
        await _channel.Writer.WriteAsync(message, cancellationToken);  //исп WriteAsync т.к. у нас нет ограничения
    }

    public async Task<TMessage> ReadAsync(CancellationToken cancellationToken = default)
    {
        return await _channel.Reader.ReadAsync(cancellationToken);
    }
}
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ServerSentEventAPI.Message
{
    public interface IMessageQueue
    {
        void Register(string id);
        void Unregister(string id);
        IAsyncEnumerable<string> DequeueAsync(string id, CancellationToken cancelToken);
        Task EnqueueAsync(string id, string message, CancellationToken cancelToken);
    }
}

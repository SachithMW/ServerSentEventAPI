using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ServerSentEventAPI.Message
{
    public class MessageQueue : IMessageQueue
    {
		private ConcurrentDictionary<string, Channel<string>> clientToChannelMap;
		public MessageQueue()
		{
			clientToChannelMap = new ConcurrentDictionary<string, Channel<string>>();
		}

		public IAsyncEnumerable<string> DequeueAsync(string cif, CancellationToken cancelToken)
		{
			if (clientToChannelMap.TryGetValue(cif, out Channel<string> channel))
			{
				return channel.Reader.ReadAllAsync(cancelToken);
			}
			else
			{
				throw new ArgumentException($"cif {cif} isn't registered");
			}
		}

		public async Task EnqueueAsync(string cif, string message, CancellationToken cancelToken)
		{
			if (clientToChannelMap.TryGetValue(cif, out Channel<string> channel))
			{
				await channel.Writer.WriteAsync(message, cancelToken);
			}
		}

		public void Register(string cif)
		{
			if (!clientToChannelMap.TryAdd(cif, Channel.CreateUnbounded<string>()))
			{
				throw new ArgumentException($"cif {cif} is already registered");
			}
		}

		public void Unregister(string cif)
		{
			clientToChannelMap.TryRemove(cif, out _);
		}

		private Channel<string> CreateChannel()
		{
			return Channel.CreateUnbounded<string>();
		}
	}
}

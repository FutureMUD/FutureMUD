#nullable enable

using System.Threading.Channels;
using BenchmarkDotNet.Attributes;

namespace MudSharp_Benchmarks;

[MemoryDiagnoser]
public class AsyncNetworkQueueBenchmarks
{
	private byte[][] _payloads = [];

	[Params(16, 256)]
	public int FrameCount { get; set; }

	[GlobalSetup]
	public void Setup()
	{
		_payloads = Enumerable.Range(0, FrameCount)
			.Select(_ => new byte[128])
			.ToArray();
	}

	[Benchmark(Baseline = true)]
	public int LockedQueueRoundTrip()
	{
		var queue = new Queue<byte[]>();
		lock (queue)
		{
			foreach (var payload in _payloads)
			{
				queue.Enqueue(payload);
			}
		}

		var bytes = 0;
		lock (queue)
		{
			while (queue.TryDequeue(out var payload))
			{
				bytes += payload.Length;
			}
		}

		return bytes;
	}

	[Benchmark]
	public async Task<int> BoundedChannelRoundTrip()
	{
		var channel = Channel.CreateBounded<byte[]>(new BoundedChannelOptions(256)
		{
			SingleReader = true,
			SingleWriter = true,
			FullMode = BoundedChannelFullMode.Wait,
			AllowSynchronousContinuations = false
		});
		var producer = ProduceAsync(channel.Writer);
		var bytes = 0;
		await foreach (var payload in channel.Reader.ReadAllAsync())
		{
			bytes += payload.Length;
		}

		await producer;
		return bytes;
	}

	private async Task ProduceAsync(ChannelWriter<byte[]> writer)
	{
		foreach (var payload in _payloads)
		{
			await writer.WriteAsync(payload);
		}

		writer.TryComplete();
	}
}

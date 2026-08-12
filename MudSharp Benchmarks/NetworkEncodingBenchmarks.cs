using System.Buffers;
using System.Text;
using BenchmarkDotNet.Attributes;
using MudSharp.Framework;

namespace MudSharp_Benchmarks;

[MemoryDiagnoser]
public class NetworkEncodingBenchmarks
{
	private string _text = string.Empty;

	[Params(128, 4096, 65_537)]
	public int CharacterCount { get; set; }

	[GlobalSetup]
	public void Setup()
	{
		_text = new string('a', CharacterCount - 1) + "\u2014";
	}

	[Benchmark(Baseline = true)]
	public byte[] LegacyLatin1NormalisationThenEncoding()
	{
		return StringExtensions.Latin1Encoder.GetBytes(_text.ConvertToLatin1());
	}

	[Benchmark]
	public int DirectLatin1EncodingWithBoundedPool()
	{
		var encoding = StringExtensions.Latin1Encoder;
		var byteCount = encoding.GetByteCount(_text);
		if (byteCount > 64 * 1024)
		{
			return encoding.GetBytes(_text).Length;
		}

		var buffer = ArrayPool<byte>.Shared.Rent(byteCount);
		try
		{
			return encoding.GetBytes(_text.AsSpan(), buffer);
		}
		finally
		{
			ArrayPool<byte>.Shared.Return(buffer);
		}
	}

	[Benchmark]
	public int DirectUtf8EncodingWithBoundedPool()
	{
		var byteCount = Encoding.UTF8.GetByteCount(_text);
		if (byteCount > 64 * 1024)
		{
			return Encoding.UTF8.GetBytes(_text).Length;
		}

		var buffer = ArrayPool<byte>.Shared.Rent(byteCount);
		try
		{
			return Encoding.UTF8.GetBytes(_text.AsSpan(), buffer);
		}
		finally
		{
			ArrayPool<byte>.Shared.Return(buffer);
		}
	}
}

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord_Bot;

internal enum DiscordTcpFrameReadResult
{
	NoFrame,
	Frame,
	Overflow
}

internal sealed class DiscordTcpFrameDecoder
{
	public const int MaximumFrameBytes = 64 * 1024;
	public const byte FrameDelimiter = 1;

	private readonly List<byte> _incomingBytes = [];

	public void Append(ReadOnlySpan<byte> bytes)
	{
		foreach (var value in bytes)
		{
			_incomingBytes.Add(value);
		}
	}

	public DiscordTcpFrameReadResult TryRead(out byte[] frame)
	{
		var delimiterIndex = _incomingBytes.IndexOf(FrameDelimiter);
		if (delimiterIndex == -1)
		{
			frame = [];
			if (_incomingBytes.Count <= MaximumFrameBytes)
			{
				return DiscordTcpFrameReadResult.NoFrame;
			}

			Clear();
			return DiscordTcpFrameReadResult.Overflow;
		}

		if (delimiterIndex > MaximumFrameBytes)
		{
			frame = [];
			Clear();
			return DiscordTcpFrameReadResult.Overflow;
		}

		frame = _incomingBytes.GetRange(0, delimiterIndex).ToArray();
		_incomingBytes.RemoveRange(0, delimiterIndex + 1);
		return DiscordTcpFrameReadResult.Frame;
	}

	public void Clear()
	{
		_incomingBytes.Clear();
	}
}

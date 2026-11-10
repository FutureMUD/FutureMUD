#nullable enable

using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MudSharp.Discord;

/// <summary>Version two uses a magic prefix and big-endian UTF-8 byte length, never a character delimiter.</summary>
public static class DiscordBridgeProtocol
{
	public const int MaximumPayloadBytes = 64 * 1024;
	public const int HeaderBytes = 8;
	internal static ReadOnlySpan<byte> Magic => "FMD2"u8;
	internal static readonly UTF8Encoding Utf8 = new(false, true);

	public static byte[] EncodeFrame(string message)
	{
		var length = Utf8.GetByteCount(message);
		if (length > MaximumPayloadBytes)
		{
			throw new ArgumentOutOfRangeException(nameof(message), "Discord bridge frame exceeds 64 KiB.");
		}

		var bytes = new byte[HeaderBytes + length];
		Magic.CopyTo(bytes);
		BinaryPrimitives.WriteInt32BigEndian(bytes.AsSpan(4, 4), length);
		Utf8.GetBytes(message, bytes.AsSpan(HeaderBytes));
		return bytes;
	}
}

/// <summary>Bounded incremental framing. Legacy detection permits existing notification-only engines during rollout.</summary>
public sealed class DiscordBridgeFrameDecoder(byte legacyDelimiter)
{
	private readonly List<byte> _bytes = [];
	private bool _detected;
	public bool IsVersion2 { get; private set; }

	public void Append(ReadOnlySpan<byte> bytes)
	{
		// One network read may contain multiple valid frames. Keep the staging bound finite too.
		if (_bytes.Count + bytes.Length > DiscordBridgeProtocol.MaximumPayloadBytes * 2 + 16)
		{
			throw new InvalidDataException("Discord bridge receive buffer limit exceeded.");
		}

		foreach (var value in bytes)
		{
			_bytes.Add(value);
		}
	}

	public bool TryRead(out string message)
	{
		message = "";
		if (!_detected)
		{
			if (_bytes.Count < 4)
			{
				return false;
			}

			IsVersion2 = _bytes[0] == (byte)'F' && _bytes[1] == (byte)'M' &&
			             _bytes[2] == (byte)'D' && _bytes[3] == (byte)'2';
			_detected = true;
		}

		if (!IsVersion2)
		{
			var delimiter = _bytes.IndexOf(legacyDelimiter);
			if (delimiter < 0)
			{
				if (_bytes.Count > DiscordBridgeProtocol.MaximumPayloadBytes)
				{
					throw new InvalidDataException("Legacy Discord frame limit exceeded.");
				}
				return false;
			}
			if (delimiter > DiscordBridgeProtocol.MaximumPayloadBytes || delimiter % 2 != 0)
			{
				throw new InvalidDataException("Invalid legacy Discord frame.");
			}
			message = Encoding.Unicode.GetString(_bytes.GetRange(0, delimiter).ToArray());
			_bytes.RemoveRange(0, delimiter + 1);
			return true;
		}

		if (_bytes.Count < DiscordBridgeProtocol.HeaderBytes)
		{
			return false;
		}
		var header = _bytes.GetRange(0, DiscordBridgeProtocol.HeaderBytes).ToArray();
		if (!header.AsSpan(0, 4).SequenceEqual(DiscordBridgeProtocol.Magic))
		{
			throw new InvalidDataException("Discord protocol cannot change within a connection.");
		}
		var length = BinaryPrimitives.ReadInt32BigEndian(header.AsSpan(4));
		if (length < 0 || length > DiscordBridgeProtocol.MaximumPayloadBytes)
		{
			throw new InvalidDataException("Invalid Discord frame length.");
		}
		if (_bytes.Count < DiscordBridgeProtocol.HeaderBytes + length)
		{
			return false;
		}
		message = DiscordBridgeProtocol.Utf8.GetString(_bytes.GetRange(DiscordBridgeProtocol.HeaderBytes, length).ToArray());
		_bytes.RemoveRange(0, DiscordBridgeProtocol.HeaderBytes + length);
		return true;
	}
}

using System;
using System.Buffers.Binary;

namespace MudSharp.Framework
{
	public struct BetterRandom
	{
		public BetterRandom(uint position)
		{
			Position = position;
			Seed = (uint)Constants.Random.Next();
		}

		public BetterRandom(uint position, uint seed)
		{
			Position = position;
			Seed = seed;
		}

		public uint Position;
		public uint Seed;

		private const uint BIT_NOISE1 = 0x68E31DA4;
		private const uint BIT_NOISE2 = 0xB5297A4D;
		private const uint BIT_NOISE3 = 0x1B56C4E9;

		private static uint NoiseFunction(uint position, uint seed)
		{
			position *= BIT_NOISE1;
			position += seed;
			position ^= position >> 8;
			position += BIT_NOISE2;
			position ^= position << 8;
			position *= BIT_NOISE3;
			position ^= position >> 8;
			return position;
		}

		public uint Next32()
		{
			return NoiseFunction(Position++, Seed);
		}

		public uint Next32(uint minimum, uint maximum)
		{
			return (uint)(NextDouble() * (maximum - minimum)) + minimum;
		}

		public int Next32i(int minimum, int maximum)
		{
			return (int)(NextDouble() * (maximum - minimum)) + minimum;
		}

		public int Next32i()
		{
			return unchecked((int) Next32());
		}

		public ulong Next64()
		{
			var x = (ulong)Next32();
			var y = (ulong)Next32();
			return (y << 32) | x;
		}

		public ulong Next64(ulong minimum, ulong maximum)
		{
			return (ulong) (NextDouble() * (maximum - minimum)) + minimum;
		}

		public long Next64i()
		{
			return unchecked((long) Next64());
		}

		public long Next64i(long minimum, long maximum)
		{
			return (long) (NextDouble() * (maximum - minimum)) + minimum;
		}

		public double NextDouble()
		{
			return Next32() * (1.0 / uint.MaxValue);
		}

		public void NextBytes(Span<byte> buffer)
		{
			var len8 = buffer.Length / 8;
			for (var i = 0; i < len8; i++)
			{
				BinaryPrimitives.TryWriteUInt64LittleEndian(buffer.Slice(i * 8, 8), Next64());
			}

			switch (buffer.Length % 8)
			{
				case > 4:
					Span<byte> last = stackalloc byte[8];
					BinaryPrimitives.WriteUInt64LittleEndian(last, Next64());
					last[..(buffer.Length % 8)].CopyTo(buffer.Slice((len8 + 1) * 8, buffer.Length % 8));
					break;
				case > 0:
					last = stackalloc byte[4];
					BinaryPrimitives.WriteUInt64LittleEndian(last, Next32());
					last[..(buffer.Length % 8)].CopyTo(buffer.Slice((len8 + 1) * 8, buffer.Length % 8));
					break;
			}
		}
	}
}
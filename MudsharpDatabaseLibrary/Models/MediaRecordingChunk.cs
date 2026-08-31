#nullable enable

using System;

namespace MudSharp.Models;

public partial class MediaRecordingChunk
{
	public long Id { get; set; }
	public long MediaRecordingId { get; set; }
	public int Sequence { get; set; }
	public long OffsetMilliseconds { get; set; }
	public long DurationMilliseconds { get; set; }
	public int UncompressedSizeBytes { get; set; }
	public byte[] Payload { get; set; } = null!;
	public DateTime CreatedAtUtc { get; set; }

	public virtual MediaRecording MediaRecording { get; set; } = null!;
}

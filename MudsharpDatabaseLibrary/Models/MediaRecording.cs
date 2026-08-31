#nullable enable

using System;
using System.Collections.Generic;

namespace MudSharp.Models;

public partial class MediaRecording
{
	public MediaRecording()
	{
		Chunks = new HashSet<MediaRecordingChunk>();
		Frames = new HashSet<MediaRecordingFrame>();
		References = new HashSet<MediaRecordingReference>();
	}

	public long Id { get; set; }
	public int SchemaVersion { get; set; }
	public int Capabilities { get; set; }
	public int Status { get; set; }
	public string Name { get; set; } = null!;
	public DateTime CreatedAtUtc { get; set; }
	public DateTime? FinalisedAtUtc { get; set; }
	public long DurationMilliseconds { get; set; }
	public long LogicalSizeInBytes { get; set; }

	public virtual ICollection<MediaRecordingChunk> Chunks { get; set; }
	public virtual ICollection<MediaRecordingFrame> Frames { get; set; }
	public virtual ICollection<MediaRecordingReference> References { get; set; }
}

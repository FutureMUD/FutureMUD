#nullable enable

using System;
using System.Collections.Generic;

namespace MudSharp.Models;

public partial class MediaSceneSnapshot
{
	public MediaSceneSnapshot()
	{
		Frames = new HashSet<MediaRecordingFrame>();
	}

	public long Id { get; set; }
	public string ContentHash { get; set; } = null!;
	public int UncompressedSizeBytes { get; set; }
	public byte[] Payload { get; set; } = null!;
	public DateTime CreatedAtUtc { get; set; }

	public virtual ICollection<MediaRecordingFrame> Frames { get; set; }
}

#nullable enable

using System;

namespace MudSharp.Models;

public partial class MediaRecordingReference
{
	public long Id { get; set; }
	public long GameItemComponentId { get; set; }
	public long MediaRecordingId { get; set; }
	public string Name { get; set; } = null!;
	public bool PubliclyAccessible { get; set; }
	public DateTime CreatedAtUtc { get; set; }
	public DateTime LastModifiedAtUtc { get; set; }

	public virtual GameItemComponent GameItemComponent { get; set; } = null!;
	public virtual MediaRecording MediaRecording { get; set; } = null!;
}

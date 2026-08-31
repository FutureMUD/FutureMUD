#nullable enable

namespace MudSharp.Models;

public partial class MediaRecordingFrame
{
	public long Id { get; set; }
	public long MediaRecordingId { get; set; }
	public long MediaSceneSnapshotId { get; set; }
	public long StartOffsetMilliseconds { get; set; }
	public long EndOffsetMilliseconds { get; set; }

	public virtual MediaRecording MediaRecording { get; set; } = null!;
	public virtual MediaSceneSnapshot MediaSceneSnapshot { get; set; } = null!;
}

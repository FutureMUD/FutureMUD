#nullable enable

using MudSharp.GameItems;

namespace MudSharp.Computers;

/// <summary>
/// Keeps the two halves of a computer media file in step: the mutable file-system entry and the immutable
/// recording reference owned by the physical host or storage component. Every copy is intentionally charged at
/// its full logical compressed size, even if the recording's snapshot blobs are deduplicated in persistence.
/// </summary>
public static class ComputerMediaFileUtilities
{
	public static bool CopyMediaFile(IFuturemud gameworld, IComputerFile sourceFile, IComputerFileOwner targetOwner,
		string targetFileName, bool publiclyAccessible, out string error)
	{
		error = string.Empty;
		if (sourceFile.Kind != ComputerFileKind.Media || sourceFile.MediaRecordingId is not { } recordingId)
		{
			error = "That source file is not a media recording.";
			return false;
		}

		if (targetOwner is not IGameItemComponent targetComponent || targetComponent.Id <= 0L)
		{
			error = "Media files can only be copied to a physical computer host or storage component.";
			return false;
		}

		var fileSystem = targetOwner.FileSystem;
		if (fileSystem is null)
		{
			error = $"{targetOwner.Name.ColourName()} does not expose a writable file system.";
			return false;
		}

		if (string.IsNullOrWhiteSpace(targetFileName))
		{
			error = "You must supply a destination file name.";
			return false;
		}

		if (fileSystem.FileExists(targetFileName))
		{
			error = $"A file named {targetFileName.ColourName()} already exists on {targetOwner.Name.ColourName()}.";
			return false;
		}

		var recording = gameworld.MediaRecordingService.GetRecording(recordingId);
		if (recording is null || recording.Status == MediaRecordingStatus.Recording)
		{
			error = "That media recording is not complete enough to copy.";
			return false;
		}

		var reference = new MediaRecordingReference(targetComponent.Id, targetFileName.Trim(), recordingId,
			publiclyAccessible, DateTime.UtcNow, DateTime.UtcNow);
		if (!gameworld.MediaRecordingService.CreateReference(reference, out error))
		{
			return false;
		}

		if (fileSystem.WriteMediaFile(targetFileName.Trim(), recordingId, recording.LogicalSizeInBytes,
			publiclyAccessible, out error))
		{
			return true;
		}

		gameworld.MediaRecordingService.DeleteReference(targetComponent.Id, targetFileName.Trim(), out _);
		return false;
	}
}

#nullable enable

using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp.Computers;

/// <summary>
/// Persists immutable media recordings outside item XML. Runtime buffers are deliberately small; only a current
/// five-second event batch is held in memory before it is compressed into a database chunk.
/// </summary>
public sealed class MediaRecordingService : IMediaRecordingService
{
	private const int MaximumUncompressedChunkBytes = 64 * 1024;
	private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

	private sealed class PendingChunk
	{
		public List<MediaPacket> Packets { get; } = [];
		public DateTime FirstPacketAtUtc { get; set; }
	}

	private readonly object _sync = new();
	private readonly IFuturemud _gameworld;
	private readonly Dictionary<long, PendingChunk> _pendingChunks = [];

	public MediaRecordingService(IFuturemud gameworld)
	{
		_gameworld = gameworld;
		_gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat += FlushExpiredChunks;
	}

	public MediaRecordingDescriptor CreateRecording(MediaRecordingCreateRequest request)
	{
		if (request.Capabilities == MediaCapabilities.None)
		{
			throw new ArgumentException("A recording must have at least one media capability.", nameof(request));
		}

		if (string.IsNullOrWhiteSpace(request.Name))
		{
			throw new ArgumentException("A recording must have a name.", nameof(request));
		}

		using (new FMDB())
		{
			var recording = new MediaRecording
			{
				SchemaVersion = 1,
				Capabilities = (int)request.Capabilities,
				Status = (int)MediaRecordingStatus.Recording,
				Name = request.Name.Trim(),
				CreatedAtUtc = request.StartedAtUtc,
				DurationMilliseconds = 0L,
				LogicalSizeInBytes = 0L
			};
			FMDB.Context.MediaRecordings.Add(recording);
			FMDB.Context.SaveChanges();
			return ToDescriptor(recording);
		}
	}

	public bool AppendPacket(long recordingId, MediaPacket packet, out string error)
	{
		error = string.Empty;
		if (recordingId <= 0L)
		{
			error = "That media recording does not have a valid identifier.";
			return false;
		}

		lock (_sync)
		{
			if (!_pendingChunks.TryGetValue(recordingId, out var pending))
			{
				pending = new PendingChunk { FirstPacketAtUtc = packet.TimestampUtc };
				_pendingChunks[recordingId] = pending;
			}

			pending.Packets.Add(packet);
			if (Encoding.UTF8.GetByteCount(JsonSerializer.Serialize(pending.Packets, JsonOptions)) >=
			    MaximumUncompressedChunkBytes)
			{
				return FlushChunk(recordingId, pending, out error);
			}
		}

		return true;
	}

	public bool AppendSceneFrame(long recordingId, TimeSpan startOffset, TimeSpan endOffset, string canonicalScene,
		out string error)
	{
		error = string.Empty;
		if (recordingId <= 0L || string.IsNullOrWhiteSpace(canonicalScene))
		{
			error = "A valid recording and scene snapshot are required.";
			return false;
		}

		var start = Math.Max(0L, (long)startOffset.TotalMilliseconds);
		var end = Math.Max(start, (long)endOffset.TotalMilliseconds);
		var canonicalBytes = Encoding.UTF8.GetBytes(canonicalScene);
		var hash = Convert.ToHexStringLower(SHA256.HashData(canonicalBytes));
		var compressed = Compress(canonicalBytes);

		using (new FMDB())
		{
			var recording = FMDB.Context.MediaRecordings
				.Include(x => x.Frames)
					.ThenInclude(x => x.MediaSceneSnapshot)
				.FirstOrDefault(x => x.Id == recordingId);
			if (recording is null || recording.Status != (int)MediaRecordingStatus.Recording)
			{
				error = "That recording is no longer accepting scene frames.";
				return false;
			}

			var snapshot = FMDB.Context.MediaSceneSnapshots.FirstOrDefault(x => x.ContentHash == hash);
			if (snapshot is null)
			{
				snapshot = new MediaSceneSnapshot
				{
					ContentHash = hash,
					Payload = compressed,
					UncompressedSizeBytes = canonicalBytes.Length,
					CreatedAtUtc = DateTime.UtcNow
				};
				FMDB.Context.MediaSceneSnapshots.Add(snapshot);
				FMDB.Context.SaveChanges();
			}

			var previous = recording.Frames
				.OrderByDescending(x => x.StartOffsetMilliseconds)
				.FirstOrDefault();
			if (previous is not null)
			{
				previous.EndOffsetMilliseconds = Math.Max(previous.EndOffsetMilliseconds, start);
			}

			if (previous is not null && previous.MediaSceneSnapshot.ContentHash == hash)
			{
				previous.EndOffsetMilliseconds = Math.Max(previous.EndOffsetMilliseconds, end);
			}
			else
			{
				FMDB.Context.MediaRecordingFrames.Add(new MediaRecordingFrame
				{
					MediaRecordingId = recording.Id,
					MediaSceneSnapshotId = snapshot.Id,
					StartOffsetMilliseconds = start,
					EndOffsetMilliseconds = end
				});
			}

			recording.DurationMilliseconds = Math.Max(recording.DurationMilliseconds, end);
			FMDB.Context.SaveChanges();
			recording.LogicalSizeInBytes = CalculateLogicalSize(FMDB.Context, recording.Id);
			FMDB.Context.SaveChanges();
		}

		return true;
	}

	public bool FinaliseRecording(long recordingId, MediaRecordingStatus status, DateTime completedAtUtc, out string error)
	{
		error = string.Empty;
		lock (_sync)
		{
			if (_pendingChunks.TryGetValue(recordingId, out var pending) && !FlushChunk(recordingId, pending, out error))
			{
				return false;
			}

			_pendingChunks.Remove(recordingId);
		}

		using (new FMDB())
		{
			var recording = FMDB.Context.MediaRecordings
				.Include(x => x.Chunks)
				.Include(x => x.Frames)
					.ThenInclude(x => x.MediaSceneSnapshot)
				.FirstOrDefault(x => x.Id == recordingId);
			if (recording is null)
			{
				error = "There is no such media recording.";
				return false;
			}

			if (recording.Status != (int)MediaRecordingStatus.Recording)
			{
				error = "That media recording has already been finalised.";
				return false;
			}

			recording.Status = (int)status;
			recording.FinalisedAtUtc = completedAtUtc;
			recording.DurationMilliseconds = Math.Max(recording.DurationMilliseconds,
				Math.Max(recording.Frames.Select(x => x.EndOffsetMilliseconds).DefaultIfEmpty(0L).Max(),
					Math.Max(0L, (long)(completedAtUtc - recording.CreatedAtUtc).TotalMilliseconds)));
			var finalFrame = recording.Frames
				.OrderByDescending(x => x.StartOffsetMilliseconds)
				.FirstOrDefault();
			if (finalFrame is not null)
			{
				finalFrame.EndOffsetMilliseconds = Math.Max(finalFrame.EndOffsetMilliseconds,
					recording.DurationMilliseconds);
			}

			recording.LogicalSizeInBytes = CalculateLogicalSize(FMDB.Context, recording.Id);
			FMDB.Context.SaveChanges();
		}

		return true;
	}

	public MediaRecordingDescriptor? GetRecording(long recordingId)
	{
		using (new FMDB())
		{
			var recording = FMDB.Context.MediaRecordings.AsNoTracking().FirstOrDefault(x => x.Id == recordingId);
			return recording is null ? null : ToDescriptor(recording);
		}
	}

	public IEnumerable<MediaPacket> ReadPackets(long recordingId)
	{
		if (recordingId <= 0L)
		{
			return [];
		}

		using (new FMDB())
		{
			var chunks = FMDB.Context.MediaRecordingChunks
				.AsNoTracking()
				.Where(x => x.MediaRecordingId == recordingId)
				.OrderBy(x => x.Sequence)
				.Select(x => x.Payload)
				.ToList();
			var packets = new List<MediaPacket>();
			foreach (var chunk in chunks)
			{
				try
				{
					var decoded = JsonSerializer.Deserialize<List<MediaPacket>>(Decompress(chunk), JsonOptions);
					if (decoded is not null)
					{
						packets.AddRange(decoded);
					}
				}
				catch (InvalidDataException)
				{
					// A damaged historic chunk is deliberately skipped. It must not prevent an otherwise valid
					// immutable recording from being played as far as possible.
				}
				catch (JsonException)
				{
					// See the invalid-data handling above.
				}
			}

			return packets
				.OrderBy(x => x.TimestampUtc)
				.ThenBy(x => x.Sequence)
				.ToList();
		}
	}

	public MediaScenePayload? GetSceneAt(long recordingId, TimeSpan offset)
	{
		if (recordingId <= 0L)
		{
			return null;
		}

		var offsetMilliseconds = Math.Max(0L, (long)offset.TotalMilliseconds);
		using (new FMDB())
		{
			var frames = FMDB.Context.MediaRecordingFrames
				.AsNoTracking()
				.Include(x => x.MediaSceneSnapshot)
				.Where(x => x.MediaRecordingId == recordingId);
			var frame = frames
				.Where(x => x.StartOffsetMilliseconds <= offsetMilliseconds &&
				            x.EndOffsetMilliseconds >= offsetMilliseconds)
				.OrderByDescending(x => x.StartOffsetMilliseconds)
				.FirstOrDefault() ?? frames
				.Where(x => x.StartOffsetMilliseconds <= offsetMilliseconds)
				.OrderByDescending(x => x.StartOffsetMilliseconds)
				.FirstOrDefault();
			if (frame is null)
			{
				return null;
			}

			try
			{
				return new MediaScenePayload(Encoding.UTF8.GetString(Decompress(frame.MediaSceneSnapshot.Payload)),
					frame.MediaSceneSnapshot.ContentHash);
			}
			catch (InvalidDataException)
			{
				return null;
			}
		}
	}

	public IEnumerable<MediaRecordingDescriptor> GetRecordings(long ownerGameItemComponentId)
	{
		using (new FMDB())
		{
			return FMDB.Context.MediaRecordingReferences
				.AsNoTracking()
				.Where(x => x.GameItemComponentId == ownerGameItemComponentId)
				.Select(x => x.MediaRecording)
				.OrderByDescending(x => x.CreatedAtUtc)
				.Select(ToDescriptor)
				.ToList();
		}
	}

	public IEnumerable<MediaRecordingReference> GetReferences(long ownerGameItemComponentId)
	{
		using (new FMDB())
		{
			return FMDB.Context.MediaRecordingReferences
				.AsNoTracking()
				.Where(x => x.GameItemComponentId == ownerGameItemComponentId)
				.OrderBy(x => x.Name)
				.Select(x => new MediaRecordingReference(x.GameItemComponentId, x.Name, x.MediaRecordingId,
					x.PubliclyAccessible, x.CreatedAtUtc, x.LastModifiedAtUtc))
				.ToList();
		}
	}

	public bool CreateReference(MediaRecordingReference reference, out string error)
	{
		error = string.Empty;
		if (reference.OwnerGameItemComponentId <= 0L || reference.RecordingId <= 0L || string.IsNullOrWhiteSpace(reference.Name))
		{
			error = "A media reference needs a component, recording and name.";
			return false;
		}

		using (new FMDB())
		{
			if (!FMDB.Context.MediaRecordings.Any(x => x.Id == reference.RecordingId))
			{
				error = "That media recording no longer exists.";
				return false;
			}

			if (FMDB.Context.MediaRecordingReferences.Any(x =>
				x.GameItemComponentId == reference.OwnerGameItemComponentId && x.Name == reference.Name))
			{
				error = $"There is already a media recording named {reference.Name}.";
				return false;
			}

			FMDB.Context.MediaRecordingReferences.Add(new Models.MediaRecordingReference
			{
				GameItemComponentId = reference.OwnerGameItemComponentId,
				MediaRecordingId = reference.RecordingId,
				Name = reference.Name.Trim(),
				PubliclyAccessible = reference.PubliclyAccessible,
				CreatedAtUtc = reference.CreatedAtUtc,
				LastModifiedAtUtc = reference.LastModifiedAtUtc
			});
			FMDB.Context.SaveChanges();
		}

		return true;
	}

	public bool DeleteReference(long ownerGameItemComponentId, string name, out string error)
	{
		error = string.Empty;
		using (new FMDB())
		{
			using var transaction = FMDB.Context.Database.BeginTransaction();
			var reference = FMDB.Context.MediaRecordingReferences
				.FirstOrDefault(x => x.GameItemComponentId == ownerGameItemComponentId && x.Name == name);
			if (reference is null)
			{
				error = "There is no media recording with that name.";
				return false;
			}

			var recordingId = reference.MediaRecordingId;
			FMDB.Context.MediaRecordingReferences.Remove(reference);
			FMDB.Context.SaveChanges();
			DeleteRecordingIfUnreferenced(FMDB.Context, recordingId);
			FMDB.Context.SaveChanges();
			transaction.Commit();
		}

		return true;
	}

	public MediaRecordingReference? GetReference(long ownerGameItemComponentId, string name)
	{
		using (new FMDB())
		{
			var reference = FMDB.Context.MediaRecordingReferences.AsNoTracking()
				.FirstOrDefault(x => x.GameItemComponentId == ownerGameItemComponentId && x.Name == name);
			return reference is null
				? null
				: new MediaRecordingReference(reference.GameItemComponentId, reference.Name, reference.MediaRecordingId,
					reference.PubliclyAccessible, reference.CreatedAtUtc, reference.LastModifiedAtUtc);
		}
	}

	public bool SetReferencePubliclyAccessible(long ownerGameItemComponentId, string name, bool publiclyAccessible,
		out string error)
	{
		error = string.Empty;
		using (new FMDB())
		{
			var reference = FMDB.Context.MediaRecordingReferences
				.FirstOrDefault(x => x.GameItemComponentId == ownerGameItemComponentId && x.Name == name);
			if (reference is null)
			{
				error = "There is no media recording with that name.";
				return false;
			}

			reference.PubliclyAccessible = publiclyAccessible;
			reference.LastModifiedAtUtc = DateTime.UtcNow;
			FMDB.Context.SaveChanges();
		}

		return true;
	}

	public void RecoverInterruptedRecordings()
	{
		using (new FMDB())
		{
			var active = FMDB.Context.MediaRecordings
				.Where(x => x.Status == (int)MediaRecordingStatus.Recording)
				.ToList();
			foreach (var recording in active)
			{
				recording.Status = (int)MediaRecordingStatus.Interrupted;
				recording.FinalisedAtUtc = DateTime.UtcNow;
			}

			if (active.Any())
			{
				FMDB.Context.SaveChanges();
			}
		}
	}

	public void FlushAll()
	{
		lock (_sync)
		{
			foreach (var (recordingId, pending) in _pendingChunks.ToList())
			{
				FlushChunk(recordingId, pending, out _);
			}
		}
	}

	private void FlushExpiredChunks()
	{
		lock (_sync)
		{
			foreach (var (recordingId, pending) in _pendingChunks
				         .Where(x => x.Value.Packets.Any() && DateTime.UtcNow - x.Value.FirstPacketAtUtc >= TimeSpan.FromSeconds(5))
				         .ToList())
			{
				FlushChunk(recordingId, pending, out _);
			}
		}
	}

	private static bool FlushChunk(long recordingId, PendingChunk pending, out string error)
	{
		error = string.Empty;
		if (!pending.Packets.Any())
		{
			return true;
		}

		using (new FMDB())
		{
			var recording = FMDB.Context.MediaRecordings
				.Include(x => x.Chunks)
				.Include(x => x.Frames)
					.ThenInclude(x => x.MediaSceneSnapshot)
				.FirstOrDefault(x => x.Id == recordingId);
			if (recording is null || recording.Status != (int)MediaRecordingStatus.Recording)
			{
				error = "That media recording is no longer accepting events.";
				return false;
			}

			var json = JsonSerializer.SerializeToUtf8Bytes(pending.Packets, JsonOptions);
			var firstOffset = Math.Max(0L, (long)(pending.Packets.Min(x => x.TimestampUtc) - recording.CreatedAtUtc).TotalMilliseconds);
			var lastOffset = Math.Max(firstOffset,
				(long)(pending.Packets.Max(x => x.TimestampUtc) - recording.CreatedAtUtc).TotalMilliseconds);
			FMDB.Context.MediaRecordingChunks.Add(new MediaRecordingChunk
			{
				MediaRecordingId = recording.Id,
				Sequence = recording.Chunks.Select(x => x.Sequence).DefaultIfEmpty(-1).Max() + 1,
				OffsetMilliseconds = firstOffset,
				DurationMilliseconds = lastOffset - firstOffset,
				UncompressedSizeBytes = json.Length,
				Payload = Compress(json),
				CreatedAtUtc = DateTime.UtcNow
			});
			recording.DurationMilliseconds = Math.Max(recording.DurationMilliseconds, lastOffset);
			FMDB.Context.SaveChanges();
			recording.LogicalSizeInBytes = CalculateLogicalSize(FMDB.Context, recording.Id);
			FMDB.Context.SaveChanges();
		}

		pending.Packets.Clear();
		pending.FirstPacketAtUtc = DateTime.UtcNow;
		return true;
	}

	private static byte[] Compress(byte[] data)
	{
		using var output = new MemoryStream();
		using (var brotli = new BrotliStream(output, CompressionLevel.SmallestSize, leaveOpen: true))
		{
			brotli.Write(data, 0, data.Length);
		}

		return output.ToArray();
	}

	private static byte[] Decompress(byte[] data)
	{
		using var input = new MemoryStream(data, writable: false);
		using var brotli = new BrotliStream(input, CompressionMode.Decompress);
		using var output = new MemoryStream();
		brotli.CopyTo(output);
		return output.ToArray();
	}

	private static MediaRecordingDescriptor ToDescriptor(MediaRecording recording)
	{
		return new MediaRecordingDescriptor(recording.Id, (MediaCapabilities)recording.Capabilities,
			(MediaRecordingStatus)recording.Status, recording.CreatedAtUtc, recording.FinalisedAtUtc,
			TimeSpan.FromMilliseconds(recording.DurationMilliseconds), recording.LogicalSizeInBytes, recording.Name);
	}

	private static long CalculateLogicalSize(FuturemudDatabaseContext context, long recordingId)
	{
		var chunkSize = context.MediaRecordingChunks
			.Where(x => x.MediaRecordingId == recordingId)
			.Select(x => x.Payload)
			.ToList()
			.Sum(x => (long)x.Length);
		var snapshotIds = context.MediaRecordingFrames
			.Where(x => x.MediaRecordingId == recordingId)
			.Select(x => x.MediaSceneSnapshotId)
			.Distinct()
			.ToList();
		var snapshotSize = snapshotIds.Count == 0
			? 0L
			: context.MediaSceneSnapshots
				.Where(x => snapshotIds.Contains(x.Id))
				.Select(x => x.Payload)
				.ToList()
				.Sum(x => (long)x.Length);
		return chunkSize + snapshotSize;
	}

	private static void DeleteRecordingIfUnreferenced(FuturemudDatabaseContext context, long recordingId)
	{
		var recording = context.MediaRecordings
			.Include(x => x.Frames)
				.ThenInclude(x => x.MediaSceneSnapshot)
			.Include(x => x.References)
			.FirstOrDefault(x => x.Id == recordingId);
		if (recording is null || recording.References.Any())
		{
			return;
		}

		var snapshotIds = recording.Frames.Select(x => x.MediaSceneSnapshotId).Distinct().ToList();
		context.MediaRecordings.Remove(recording);
		context.SaveChanges();
		var snapshots = context.MediaSceneSnapshots
			.Include(x => x.Frames)
			.Where(x => snapshotIds.Contains(x.Id))
			.Where(x => !x.Frames.Any())
			.ToList();
		context.MediaSceneSnapshots.RemoveRange(snapshots);
	}
}

#nullable enable

using MudSharp.Computers;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class MediaStorageMediumGameItemComponent : GameItemComponent, IMediaStorageMedium
{
	private MediaStorageMediumGameItemComponentProto _prototype;
	private bool _writeProtected;

	public MediaStorageMediumGameItemComponent(MediaStorageMediumGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public MediaStorageMediumGameItemComponent(MudSharp.Models.GameItemComponent component,
		MediaStorageMediumGameItemComponentProto proto, IGameItem parent)
		: base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadRuntimeState(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public MediaStorageMediumGameItemComponent(MediaStorageMediumGameItemComponent rhs, IGameItem newParent,
		bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_writeProtected = rhs._writeProtected;
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public string FormatKey => _prototype.FormatKey;
	public MediaCapabilities MediaCapabilities => _prototype.Capabilities;
	public TimeSpan Capacity => _prototype.Capacity;
	public TimeSpan UsedCapacity => TimeSpan.FromMilliseconds(Recordings
		.Select(x => Gameworld.MediaRecordingService.GetRecording(x.RecordingId)?.Duration.TotalMilliseconds ?? 0.0)
		.Sum());
	public TimeSpan RemainingCapacity => Capacity > UsedCapacity ? Capacity - UsedCapacity : TimeSpan.Zero;

	public bool WriteProtected
	{
		get => _writeProtected;
		set
		{
			if (_writeProtected == value)
			{
				return;
			}

			_writeProtected = value;
			Changed = true;
		}
	}

	public IReadOnlyCollection<MediaRecordingReference> Recordings => Gameworld.MediaRecordingService
		.GetRecordings(Id)
		.Select(x => Gameworld.MediaRecordingService.GetReference(Id, x.Name))
		.Where(x => x is not null)
		.Cast<MediaRecordingReference>()
		.ToList();

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new MediaStorageMediumGameItemComponent(this, newParent, temporary);
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (MediaStorageMediumGameItemComponentProto)newProto;
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Full;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		if (type != DescriptionType.Full)
		{
			return description;
		}

		var sb = new StringBuilder(description);
		sb.AppendLine();
		sb.AppendLine();
		sb.AppendLine($"It is a {FormatKey.ColourName()} {MediaComponentUtilities.DescribeCapabilities(MediaCapabilities).ColourValue()} medium with {Capacity.Describe(voyeur).ColourValue()} capacity.");
		sb.AppendLine($"It has {RemainingCapacity.Describe(voyeur).ColourValue()} remaining and is {(WriteProtected ? "write-protected".ColourError() : "write-enabled".ColourValue())}.");
		sb.AppendLine($"It contains {Recordings.Count.ToString("N0", voyeur).ColourValue()} recording{"".Pluralise(Recordings.Count != 1)}.");
		return sb.ToString();
	}

	public bool HasRecording(string name)
	{
		return GetRecording(name) is not null;
	}

	public MediaRecordingReference? GetRecording(string name)
	{
		return Gameworld.MediaRecordingService.GetReference(Id, name.Trim());
	}

	public bool CanStoreRecording(MediaRecordingDescriptor recording, out string error)
	{
		if (WriteProtected)
		{
			error = "That physical medium is write-protected.";
			return false;
		}

		if ((recording.Capabilities & ~MediaCapabilities) != MediaCapabilities.None)
		{
			error = "That recording has media capabilities this physical medium cannot store.";
			return false;
		}

		var existing = GetRecording(recording.Name);
		if (existing is not null && existing.RecordingId != recording.RecordingId)
		{
			error = "That physical medium already has a recording with that name. Erase it before replacing it.";
			return false;
		}

		var existingDuration = existing is null
			? TimeSpan.Zero
			: Gameworld.MediaRecordingService.GetRecording(existing.RecordingId)?.Duration ?? TimeSpan.Zero;
		if (UsedCapacity - existingDuration + recording.Duration > Capacity)
		{
			error = "That physical medium does not have enough remaining recording capacity.";
			return false;
		}

		error = string.Empty;
		return true;
	}

	public bool StoreRecording(MediaRecordingDescriptor recording, out string error)
	{
		if (!CanStoreRecording(recording, out error))
		{
			return false;
		}

		var existing = GetRecording(recording.Name);
		if (existing?.RecordingId == recording.RecordingId)
		{
			error = string.Empty;
			return true;
		}

		return Gameworld.MediaRecordingService.CreateReference(new MediaRecordingReference(Id, recording.Name,
			recording.RecordingId, false, DateTime.UtcNow, DateTime.UtcNow), out error);
	}

	public bool DeleteRecording(string name, out string error)
	{
		if (WriteProtected)
		{
			error = "That physical medium is write-protected.";
			return false;
		}

		return Gameworld.MediaRecordingService.DeleteReference(Id, name.Trim(), out error);
	}

	public override void Delete()
	{
		foreach (var recording in Recordings.ToList())
		{
			Gameworld.MediaRecordingService.DeleteReference(Id, recording.Name, out _);
		}

		base.Delete();
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition", new XElement("WriteProtected", WriteProtected)).ToString();
	}

	private void LoadRuntimeState(XElement root)
	{
		_writeProtected = bool.TryParse(root.Element("WriteProtected")?.Value, out var writeProtected) && writeProtected;
	}
}

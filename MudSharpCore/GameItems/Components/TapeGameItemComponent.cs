#nullable enable
using MudSharp.Character;
using MudSharp.Form.Audio;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.GameItems.Components;

public class TapeGameItemComponent : GameItemComponent, IAudioStorageTape
{
    private readonly List<StoredAudioRecording> _recordings = [];
    private TapeGameItemComponentProto _prototype;
    private bool _writeProtected;

    public TapeGameItemComponent(TapeGameItemComponentProto proto, IGameItem parent, bool temporary = false)
        : base(parent, proto, temporary)
    {
        _prototype = proto;
    }

    public TapeGameItemComponent(Models.GameItemComponent component, TapeGameItemComponentProto proto, IGameItem parent)
        : base(component, parent)
    {
        _prototype = proto;
        _noSave = true;
        LoadFromXml(XElement.Parse(component.Definition));
        _noSave = false;
    }

    public TapeGameItemComponent(TapeGameItemComponent rhs, IGameItem newParent, bool temporary = false)
        : base(rhs, newParent, temporary)
    {
        _prototype = rhs._prototype;
        _writeProtected = rhs._writeProtected;
        _recordings.AddRange(rhs._recordings);
    }

    public override IGameItemComponentProto Prototype => _prototype;

    protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
    {
        _prototype = (TapeGameItemComponentProto)newProto;
    }

    public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
    {
        return new TapeGameItemComponent(this, newParent, temporary);
    }

    private void LoadFromXml(XElement root)
    {
        _writeProtected = bool.Parse(root.Element("WriteProtected")?.Value ?? "false");
        _recordings.Clear();
        foreach (XElement recording in root.Element("Recordings")?.Elements("StoredRecording") ?? Enumerable.Empty<XElement>())
        {
            _recordings.Add(StoredAudioRecording.LoadFromXml(recording));
        }
    }

    protected override string SaveToXml()
    {
        return new XElement("Definition",
            new XElement("WriteProtected", _writeProtected),
            new XElement("Recordings",
                from recording in _recordings
                select recording.SaveToXml())
        ).ToString();
    }

    public override bool DescriptionDecorator(DescriptionType type)
    {
        return type == DescriptionType.Full;
    }

    public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type, bool colour,
        PerceiveIgnoreFlags flags)
    {
        if (type != DescriptionType.Full)
        {
            return base.Decorate(voyeur, name, description, type, colour, flags);
        }

        StringBuilder sb = new();
        sb.AppendLine(description);
        sb.AppendLine($"It can store up to {Capacity.TotalMinutes.ToString("N1", voyeur).ColourValue()} minutes of audio.");
        sb.AppendLine($"It is currently using {UsedCapacity.TotalMinutes.ToString("N1", voyeur).ColourValue()} minutes.");
        sb.AppendLine($"It is currently {(WriteProtected ? "write-protected".ColourError() : "write-enabled".ColourValue())}.");
        sb.AppendLine($"It contains {Recordings.Count.ToString("N0", voyeur).ColourValue()} recording{(Recordings.Count == 1 ? string.Empty : "s")}.");
        return sb.ToString();
    }

    public TimeSpan Capacity => _prototype.Capacity;
    public TimeSpan UsedCapacity => TimeSpan.FromMilliseconds(_recordings.Sum(x => x.Recording.TotalDuration.TotalMilliseconds));
    public TimeSpan RemainingCapacity => Capacity - UsedCapacity > TimeSpan.Zero ? Capacity - UsedCapacity : TimeSpan.Zero;

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

    public IReadOnlyCollection<StoredAudioRecording> Recordings => _recordings.AsReadOnly();

    public bool HasRecording(string name)
    {
        return _recordings.Any(x => x.Name.EqualTo(name));
    }

    public StoredAudioRecording? GetRecording(string name)
    {
        return _recordings.FirstOrDefault(x => x.Name.EqualTo(name));
    }

    public bool CanStoreRecording(StoredAudioRecording recording, out string error)
    {
        if (WriteProtected)
        {
            error = "That tape is write-protected.";
            return false;
        }

        StoredAudioRecording? existing = GetRecording(recording.Name);
        TimeSpan replacedDuration = existing?.Recording.TotalDuration ?? TimeSpan.Zero;
        if (UsedCapacity - replacedDuration + recording.Recording.TotalDuration > Capacity)
        {
            error = "That tape does not have enough remaining capacity.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    public bool StoreRecording(StoredAudioRecording recording, out string error)
    {
        if (!CanStoreRecording(recording, out error))
        {
            return false;
        }

        _recordings.RemoveAll(x => x.Name.EqualTo(recording.Name));
        _recordings.Add(recording);
        Changed = true;
        error = string.Empty;
        return true;
    }

    public bool DeleteRecording(string name)
    {
        if (WriteProtected)
        {
            return false;
        }

        if (_recordings.RemoveAll(x => x.Name.EqualTo(name)) == 0)
        {
            return false;
        }

        Changed = true;
        return true;
    }

    public void DeleteAllRecordings()
    {
        if (WriteProtected || !_recordings.Any())
        {
            return;
        }

        _recordings.Clear();
        Changed = true;
    }
}

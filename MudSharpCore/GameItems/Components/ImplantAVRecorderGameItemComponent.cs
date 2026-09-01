#nullable enable

using MudSharp.Body;
using MudSharp.Communication.Language;
using MudSharp.Computers;
using MudSharp.Construction;
using MudSharp.Form.Audio;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Law;

namespace MudSharp.GameItems.Components;

public class ImplantAVRecorderGameItemComponent : DigitalMediaRecorderGameItemComponent, IImplantAVRecorder,
	IImplantReportStatus, IMediaCaptureSource, IComputerMediaStorageTarget
{
	private ImplantAVRecorderGameItemComponentProto _implantPrototype;
	private ImplantMachineRuntime _implant;
	private string _alias = "recorder";
	private long _selectedHostId;
	private long _selectedStorageId;
	private Guid _streamId = Guid.NewGuid();
	private long _captureSequence;
	private DateTime _lastSnapshotAtUtc;
	private bool _snapshotSubscribed;
	public ImplantAVRecorderGameItemComponent(ImplantAVRecorderGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(proto, parent, temporary) { _implantPrototype = proto; _implant = CreateSupport(); }
	public ImplantAVRecorderGameItemComponent(MudSharp.Models.GameItemComponent component, ImplantAVRecorderGameItemComponentProto proto, IGameItem parent) : base(component, proto, parent)
	{
		_implantPrototype = proto; _implant = CreateSupport(); var root = XElement.Parse(component.Definition); _implant.Load(root); _alias = root.Element("ImplantAlias")?.Value ?? "recorder"; _selectedHostId = long.TryParse(root.Element("SelectedImplantHost")?.Value, out var h) ? h : 0L; _selectedStorageId = long.TryParse(root.Element("SelectedImplantStorage")?.Value, out var s) ? s : 0L;
	}
	private ImplantAVRecorderGameItemComponent(ImplantAVRecorderGameItemComponent rhs, IGameItem parent, bool temporary) : base(rhs, parent, temporary) { _implantPrototype = rhs._implantPrototype; _implant = CreateSupport(); _alias = rhs._alias; _streamId = Guid.NewGuid(); }
	private ImplantMachineRuntime CreateSupport() => new(Parent, () => _implantPrototype, () => Changed = true);
	public override IGameItemComponentProto Prototype => _implantPrototype;
	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false) => new ImplantAVRecorderGameItemComponent(this, newParent, temporary);
	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto) { base.UpdateComponentNewPrototype(newProto); _implantPrototype = (ImplantAVRecorderGameItemComponentProto)newProto; }
	protected override XElement SaveToXml(XElement root) { base.SaveToXml(root); _implant.Save(root); root.Add(new XElement("ImplantAlias", new XCData(_alias)), new XElement("SelectedImplantHost", _selectedHostId), new XElement("SelectedImplantStorage", _selectedStorageId)); return root; }
	public override bool Powered => IsPowered && FunctionFactor > 0.0;
	public double FunctionFactor => _implant.FunctionFactor(IsPowered);
	public bool External => ((IImplantMachinePrototypeSettings)_implantPrototype).External;
	public string ExternalDescription => ((IImplantMachinePrototypeSettings)_implantPrototype).ExternalDescription;
	public IBodyPrototype TargetBody => ((IImplantMachinePrototypeSettings)_implantPrototype).TargetBody;
	public IBodypart TargetBodypart { get => _implant.TargetBodypart; set => _implant.TargetBodypart = value; }
	public IBody InstalledBody => _implant.InstalledBody!;
	public void InstallImplant(IBody body) { _implant.Install(body); RefreshPowerSourceConnection(); }
	public void RemoveImplant() { Stop(out _); ReleasePowerSourceConnection(); _implant.Remove(); }
	public double ImplantSpaceOccupied => ((IImplantMachinePrototypeSettings)_implantPrototype).ImplantSpaceOccupied;
	public Difficulty InstallDifficulty => ((IImplantMachinePrototypeSettings)_implantPrototype).InstallDifficulty;
	private IImplantComputerHost? AssignedHost => InstalledBody?.Implants
		.OfType<IImplantComputerHost>()
		.FirstOrDefault(x => x.Id == _selectedHostId);
	private IImplantComputerHost? SelectedHost => AssignedHost is { } host &&
		ImplantComputerUtilities.GetPoweredBus(host) is { } bus &&
		ReferenceEquals(bus, ImplantComputerUtilities.GetPoweredBus(this)) ? host : null;
	private ImplantComputerStorageGameItemComponent? AssignedStorage => InstalledBody?.Implants
		.OfType<ImplantComputerStorageGameItemComponent>()
		.FirstOrDefault(x => x.Id == _selectedStorageId);
	private ImplantComputerStorageGameItemComponent? SelectedStorage => AssignedStorage is { FileSystem: not null } storage &&
		ImplantComputerUtilities.GetPoweredBus(storage) == ImplantComputerUtilities.GetPoweredBus(this) ? storage : null;
	protected override IComputerHost? MediaConnectedHost => (IComputerHost?)SelectedHost ?? this;
	public IImplantComputerHost? AssignedComputerHost => AssignedHost;
	public override IComputerFileSystem? FileSystem => _selectedStorageId == 0 ? base.FileSystem : SelectedStorage?.FileSystem;
	public IComputerFileOwner ActiveMediaStorage => _selectedStorageId == 0 ? this : (IComputerFileOwner?)AssignedStorage ?? this;
	public override bool MediaAvailable => Powered && SwitchedOn && InstalledBody is not null;
	public string AliasForCommands { get => _alias; set { _alias = value; Changed = true; } }
	public IEnumerable<string> Commands => ["on", "off", "host", "storage", "record", "snapshot", "stop", "list", "erase", "play", "still"];
	public string CommandHelp => "on|off; host <alias|none>; storage <internal|drive alias>; record <name>; snapshot <name>; stop; list; erase <name>; play <name>; still <name> [hh:mm:ss]";
	public void IssueCommand(string command, StringStack arguments)
	{
		var actor = InstalledBody?.Actor; if (actor is null) return;
		var exact = Commands.FirstOrDefault(x => x.EqualTo(command)) ?? Commands.FirstOrDefault(x => x.StartsWith(command, StringComparison.InvariantCultureIgnoreCase)) ?? string.Empty;
		if (exact.EqualTo("on") || exact.EqualTo("off")) { Switch(actor, exact); actor.Send($"The implant recorder is now {(SwitchedOn ? "on" : "off").ColourValue()}."); return; }
		if (!Powered || ImplantComputerUtilities.GetPoweredBus(this) is null) { actor.Send("The recorder or its neural data link is unpowered."); return; }
		if (exact.EqualTo("storage")) { SetStorage(actor, arguments); return; }
		if (_selectedStorageId > 0 && SelectedStorage is null) { actor.Send("The selected implant storage is unavailable. Select internal storage or another powered drive."); return; }
		switch (exact)
		{
			case "host": SetHost(actor, arguments); break;
			case "record":
				if (arguments.IsFinished) { actor.Send("Name the recording."); break; }
				if (!StartRecording(arguments.SafeRemainingArgument, out var recordError)) actor.Send(recordError); else actor.Send("Recording started.");
				break;
			case "snapshot":
				if (arguments.IsFinished) { actor.Send("Name the snapshot."); break; }
				if (!CaptureStill(arguments.SafeRemainingArgument, out var snapshotError)) actor.Send(snapshotError); else actor.Send("Snapshot captured.");
				break;
			case "stop": if (!Stop(out var stopError)) actor.Send(stopError); else actor.Send("Recording or playback stopped."); break;
			case "list": actor.Send(MediaFiles.Any() ? MediaFiles.Select(x => x.FileName.ColourCommand()).ListToString() : "There are no stored recordings."); break;
			case "erase": if (arguments.IsFinished || !RecordingFileSystem.DeleteFile(arguments.SafeRemainingArgument)) actor.Send("There is no such recording."); else actor.Send("Recording erased."); break;
			case "play":
				if (arguments.IsFinished) { actor.Send("Name the recording."); break; }
				if (!CanNeurallyPresent(arguments.SafeRemainingArgument, out var playError) || !StartPlayback(arguments.SafeRemainingArgument, out playError)) actor.Send(playError); else actor.Send("Neural playback started.");
				break;
			case "still": ShowStill(actor, arguments); break;
		}
	}
	private void SetHost(ICharacter actor, StringStack args) { if (IsRecording || IsPlaying) { actor.Send("Stop the active job before changing host."); return; } if (args.IsFinished) { actor.Send("Specify a host alias or none."); return; } var alias = args.PopSpeech(); if (alias.EqualTo("none")) { _selectedHostId = 0; Changed = true; actor.Send("Host assignment cleared."); return; } var host = ImplantComputerUtilities.ResolveAliased<IImplantComputerHost>(this, alias, out var error); if (host is null) { actor.Send(error); return; } _selectedHostId = host.Id; Changed = true; actor.Send($"Recorder assigned to {host.Parent.HowSeen(actor).ColourName()}."); }
	private void SetStorage(ICharacter actor, StringStack args) { if (IsRecording || IsPlaying) { actor.Send("Stop the active job before changing storage."); return; } if (args.IsFinished) { actor.Send("Specify internal or a drive alias."); return; } var alias = args.PopSpeech(); if (alias.EqualTo("internal")) { _selectedStorageId = 0; Changed = true; actor.Send("The recorder will use internal storage."); return; } var storage = ImplantComputerUtilities.ResolveAliased<ImplantComputerStorageGameItemComponent>(this, alias, out var error); if (storage is null || storage.FileSystem is null) { actor.Send(storage is null ? error : "That drive is unpowered."); return; } _selectedStorageId = storage.Id; Changed = true; actor.Send($"The recorder will use {storage.Parent.HowSeen(actor).ColourName()}."); }
	private bool CanNeurallyPresent(string name, out string error) { var file = RecordingFileSystem.GetFile(name); if (file?.MediaRecordingId is null) { error = "There is no recording with that name."; return false; } var descriptor = Gameworld.MediaRecordingService.GetRecording(file.MediaRecordingId.Value); var bus = ImplantComputerUtilities.GetPoweredBus(this); if (descriptor is null || bus is null || (descriptor.Capabilities.HasFlag(MediaCapabilities.Audio) && !bus.PermitsAudio) || (descriptor.Capabilities.HasFlag(MediaCapabilities.Video) && !bus.PermitsVisual)) { error = "The neural interface does not permit that recording's media."; return false; } error = string.Empty; return true; }
	private void ShowStill(ICharacter actor, StringStack args) { if (args.IsFinished) { actor.Send("Name a recording."); return; } var name = args.PopSpeech(); TimeSpan? offset = null; if (!args.IsFinished) { if (!TimeSpan.TryParse(args.SafeRemainingArgument, actor, out var parsed) || parsed < TimeSpan.Zero) { actor.Send("Use a non-negative hh:mm:ss timestamp."); return; } offset = parsed; } var scene = GetStill(name, offset, out var error); actor.Send(scene ?? error); }
	public string ReportStatus() => $"\t* Power: {Powered.ToColouredString()}\n\t* State: {(IsRecording ? "recording" : IsPlaying ? "playing" : "stopped").ColourValue()}\n\t* Host: {(((IComputerHost?)SelectedHost)?.Name ?? "none").ColourName()}\n\t* Storage: {(_selectedStorageId == 0 ? "internal" : SelectedStorage?.Name ?? "unavailable").ColourName()}\n";
	public override bool PublishOutput(string endpoint, MediaPacket packet, out string error)
	{
		var bus = ImplantComputerUtilities.GetPoweredBus(this);
		var actor = InstalledBody?.Actor;
		if (bus is null || actor is null)
		{
			error = "The recorder has no powered neural presentation link.";
			return false;
		}
		actor.OutputHandler.Send(new MudSharp.PerceptionEngine.Outputs.MediaPlaybackOutput(Gameworld, actor, packet));
		error = string.Empty;
		return true;
	}

	public bool TryCapture(ILocation location, IOutput output, out MediaPacket packet)
	{
		packet = null!; var actor = InstalledBody?.Actor; if (!MediaAvailable || actor is null || !ReferenceEquals(actor.Location, location) || location is not ICell cell) return false;
		var sensor = new MediaSensorPerceiver(actor, cell, _implantPrototype.SensorSensitivity); if (!output.ShouldSee(sensor)) return false;
		if (output is IMediaPacketOutput mediaOutput)
		{
			var sourcePacket = mediaOutput.MediaPacket;
			if (sourcePacket.HasVisited(CaptureEndpoint) || sourcePacket.Source == CaptureEndpoint ||
			    (sourcePacket.Capabilities & MediaCapabilities) == MediaCapabilities.None)
			{
				if (MediaCapabilities.HasFlag(MediaCapabilities.Audio) &&
				    MediaComponentUtilities.IsLoudFeedbackLoop(sourcePacket, CaptureEndpoint))
				{
					MediaAudioPresentation.EmitFeedback(mediaOutput.PresentationSource as IGameItem ?? Parent);
				}

				return false;
			}

			packet = sourcePacket with
			{
				Source = CaptureEndpoint,
				Sequence = ++_captureSequence,
				TimestampUtc = DateTime.UtcNow,
				Capabilities = sourcePacket.Capabilities & MediaCapabilities,
				Provenance = sourcePacket.Provenance.Append(CaptureEndpoint).ToArray()
			};
			return true;
		}
		var source = output is IEmoteOutput emote ? emote.DefaultSource : null;
		if (MediaComponentUtilities.IsAudible(output) && source is not null &&
		    (!sensor.CanHear(source) || cell.LocalAudioDifficulty(sensor,
			    MediaComponentUtilities.GetAudioVolume(output) ?? AudioVolume.Decent,
			    actor.GetProximity(source)) == Difficulty.Impossible)) return false;
		if (output is IRecordableLanguageOutput language) { var signed = language.LanguageInfo.Form == LanguageForm.Signed; var caps = signed ? MediaCapabilities.Video : MediaCapabilities.Audio; if ((caps & MediaCapabilities) == MediaCapabilities.None) return false; packet = CreateCaptured(caps, signed ? MediaEventKind.Video : MediaEventKind.Audio, MediaComponentUtilities.CreateLanguagePayload(language)); return true; }
		var caps2 = (MediaComponentUtilities.IsAudible(output) ? MediaCapabilities.Audio : MediaCapabilities.None) | (MediaComponentUtilities.IsVisual(output) ? MediaCapabilities.Video : MediaCapabilities.None); caps2 &= MediaCapabilities; if (caps2 == MediaCapabilities.None) return false;
		var kind = caps2 == (MediaCapabilities.Audio | MediaCapabilities.Video) ? MediaEventKind.AudioVideo : caps2 == MediaCapabilities.Audio ? MediaEventKind.Audio : MediaEventKind.Video;
		packet = CreateCaptured(caps2, kind, new MediaTextPayload(output.ParseFor(sensor), caps2.HasFlag(MediaCapabilities.Audio), caps2.HasFlag(MediaCapabilities.Video), caps2.HasFlag(MediaCapabilities.Audio) ? (int)(MediaComponentUtilities.GetAudioVolume(output) ?? AudioVolume.Decent) : null)); return true;
	}
	public bool TryCaptureCrime(ICrime crime, out MediaPacket packet) { packet = null!; var actor = InstalledBody?.Actor; if (!MediaAvailable || actor is null || !MediaCapabilities.HasFlag(MediaCapabilities.Video) || !ReferenceEquals(actor.Location, crime.CrimeLocation)) return false; var sensor = new MediaSensorPerceiver(actor, actor.Location, _implantPrototype.SensorSensitivity); if (!sensor.CanSee(crime.Criminal)) return false; packet = CreateCaptured(MediaCapabilities.Video, MediaEventKind.CrimeWitness, new MediaCrimePayload(0L)) with { TimestampUtc = crime.RealTimeOfCrime }; return true; }
	public string? CaptureCanonicalScene() { var actor = InstalledBody?.Actor; if (!MediaAvailable || actor is null || !MediaCapabilities.HasFlag(MediaCapabilities.Video)) return null; var sensor = new MediaSensorPerceiver(actor, actor.Location, _implantPrototype.SensorSensitivity); return actor.Location.HowSeen(sensor, type: DescriptionType.Full, colour: false, flags: PerceiveIgnoreFlags.IgnoreNamesSetting | PerceiveIgnoreFlags.IgnoreLiquidsAndFlags).NormaliseSpacing(); }
	private MediaEndpointAddress CaptureEndpoint => new(Parent.Id, Id, $"{MediaEndpoint.EndpointKey}:capture", MediaEndpointDirection.Output);
	private MediaPacket CreateCaptured(MediaCapabilities caps, MediaEventKind kind, MediaPayload payload) => new(_streamId, ++_captureSequence, DateTime.UtcNow, caps, kind, CaptureEndpoint, [CaptureEndpoint], payload);
	private void SnapshotHeartbeat()
	{
		if (_selectedStorageId > 0 && SelectedStorage is null && (IsRecording || IsPlaying))
		{
			Gameworld.ComputerMediaService.InterruptJobs(this);
		}
		if (_selectedHostId > 0 && SelectedHost is null && AssignedHost is { } assignedHost)
		{
			Gameworld.ComputerMediaService.InterruptJobs(assignedHost, this);
		}
		if (!MediaAvailable || DateTime.UtcNow - _lastSnapshotAtUtc < _implantPrototype.SnapshotInterval) return;
		var scene = CaptureCanonicalScene();
		if (string.IsNullOrWhiteSpace(scene)) return;
		var packet = CreateCaptured(MediaCapabilities.Video, MediaEventKind.SceneSnapshot,
			new MediaScenePayload(scene, string.Empty));
		if (!Gameworld.MediaChannelService.GetSinks().Any(x => x.Accepts(packet))) return;
		_lastSnapshotAtUtc = DateTime.UtcNow;
		Gameworld.MediaChannelService.Publish(packet);
	}
	protected override void OnPowerCutInAction() { base.OnPowerCutInAction(); if (!_snapshotSubscribed) { Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat += SnapshotHeartbeat; _snapshotSubscribed = true; } SnapshotHeartbeat(); }
	protected override void OnPowerCutOutAction() { if (_snapshotSubscribed) { Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat -= SnapshotHeartbeat; _snapshotSubscribed = false; } base.OnPowerCutOutAction(); }
}

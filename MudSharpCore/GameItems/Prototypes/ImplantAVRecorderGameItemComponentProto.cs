#nullable enable

using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Prototypes;

public class ImplantAVRecorderGameItemComponentProto : DigitalMediaRecorderGameItemComponentProto,
	IImplantAVRecorderPrototype, IImplantReportStatusPrototype, IMediaCaptureSourcePrototype,
	IImplantMachinePrototypeSettings
{
	private ImplantMachinePrototypeSettings _implant = null!;
	public ImplantAVRecorderGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator, "Implant A/V Recorder")
	{
		_implant ??= new ImplantMachinePrototypeSettings(gameworld); SensorSensitivity = 0.1; SnapshotInterval = TimeSpan.FromSeconds(5);
	}
	protected ImplantAVRecorderGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) : base(proto, gameworld) => _implant ??= new ImplantMachinePrototypeSettings(gameworld);
	public override string TypeDescription => "Implant A/V Recorder";
	public double SensorSensitivity { get; private set; }
	public TimeSpan SnapshotInterval { get; private set; }
	bool IImplantMachinePrototypeSettings.External => _implant.External;
	string IImplantMachinePrototypeSettings.ExternalDescription => _implant.ExternalDescription;
	IBodyPrototype IImplantMachinePrototypeSettings.TargetBody => _implant.TargetBody;
	IBodypart IImplantMachinePrototypeSettings.TargetBodypart => _implant.TargetBodypart;
	double IImplantMachinePrototypeSettings.ImplantSpaceOccupied => _implant.ImplantSpaceOccupied;
	Difficulty IImplantMachinePrototypeSettings.InstallDifficulty => _implant.InstallDifficulty;
	double IImplantMachinePrototypeSettings.ImplantDamageFunctionGrace => _implant.ImplantDamageFunctionGrace;
	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		_implant ??= new ImplantMachinePrototypeSettings(Gameworld);
		_implant.Load(root);
		SensorSensitivity = double.TryParse(root.Element("SensorSensitivity")?.Value, out var sensitivity)
			? sensitivity
			: 0.1;
		SnapshotInterval = TimeSpan.FromSeconds(double.TryParse(root.Element("SnapshotIntervalSeconds")?.Value,
			out var interval) ? Math.Max(5.0, interval) : 5.0);
	}
	protected override XElement SaveSubtypeToXml(XElement root) { base.SaveSubtypeToXml(root); _implant.Save(root); root.Add(new XElement("SensorSensitivity", SensorSensitivity), new XElement("SnapshotIntervalSeconds", SnapshotInterval.TotalSeconds)); return root; }
	public override string ShowBuildingHelp => $"{base.ShowBuildingHelp}{ImplantMachinePrototypeSettings.BuildingHelp}\n\t#3sensitivity <number>#0 - minimum illumination\n\t#3interval <seconds>#0 - snapshot interval, minimum five seconds";
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var verb = command.PopForSwitch();
		var implantResult = _implant.BuildingCommand(actor, verb, command, () => Changed = true);
		if (implantResult.HasValue) return implantResult.Value;
		if (verb.EqualTo("sensitivity") && double.TryParse(command.PopSpeech(), out var sensitivity) && sensitivity >= 0.0) { SensorSensitivity = sensitivity; Changed = true; actor.Send("Sensor sensitivity updated."); return true; }
		if (verb.EqualTo("interval") && double.TryParse(command.PopSpeech(), out var seconds) && seconds >= 5.0) { SnapshotInterval = TimeSpan.FromSeconds(seconds); Changed = true; actor.Send("Snapshot interval updated."); return true; }
		return base.BuildingCommand(actor, command.GetUndo());
	}
	public static new void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("implantavrecorder", true, (g, a) => new ImplantAVRecorderGameItemComponentProto(g, a));
		manager.AddBuilderLoader("implant recorder", false, (g, a) => new ImplantAVRecorderGameItemComponentProto(g, a));
		manager.AddDatabaseLoader("Implant A/V Recorder", (p, g) => new ImplantAVRecorderGameItemComponentProto(p, g));
		manager.AddTypeHelpInfo("Implant A/V Recorder", "A neurally controlled implanted audio/video recorder", ImplantMachinePrototypeSettings.BuildingHelp);
	}
	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false) => new ImplantAVRecorderGameItemComponent(this, parent, temporary);
	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent) => new ImplantAVRecorderGameItemComponent(component, this, parent);
	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator) => CreateNewRevision(initiator, (p, g) => new ImplantAVRecorderGameItemComponentProto(p, g));
}

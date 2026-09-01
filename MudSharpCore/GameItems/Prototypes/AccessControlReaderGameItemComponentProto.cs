#nullable enable

using MudSharp.Accounts;
using MudSharp.GameItems.Components;

namespace MudSharp.GameItems.Prototypes;

public abstract class AccessControlReaderGameItemComponentProto : PoweredMachineBaseGameItemComponentProto,
	IAccessControlReaderPrototype
{
	protected const string AccessControlBuildingHelpText = @"
	#3value <number>#0 - sets the successful access signal value
	#3duration <seconds>#0 - sets how long a successful access signal remains active
	#3selftarget none|<lock component>#0 - optionally drives a built-in sibling lock directly

#6Notes:#0

	Readers can be embedded directly in an item or installed in a compatible automation bay. Self-targeting is intended for a single built-in sibling lock; use normal signal automation and a microcontroller when several readers share one lock.";

	protected AccessControlReaderGameItemComponentProto(IFuturemud gameworld, IAccount originator, string type)
		: base(gameworld, originator, type)
	{
		UseMountHostPowerSource = true;
		Wattage = 35.0;
		SignalValue = 1.0;
		SignalDuration = TimeSpan.FromSeconds(3);
	}

	protected AccessControlReaderGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto,
		IFuturemud gameworld) : base(proto, gameworld)
	{
	}

	public double SignalValue { get; protected set; }
	public TimeSpan SignalDuration { get; protected set; }
	public long SelfTargetLockPrototypeId { get; protected set; }
	public string SelfTargetLockPrototypeName { get; protected set; } = string.Empty;
	public abstract string AccessMountType { get; }

	protected void LoadAccessControlFromXml(XElement root)
	{
		SignalValue = double.TryParse(root.Element("SignalValue")?.Value, out var signalValue) &&
					  double.IsFinite(signalValue)
			? signalValue
			: 1.0;
		SignalDuration = TimeSpan.FromSeconds(
			double.TryParse(root.Element("SignalDurationSeconds")?.Value, out var duration) &&
			double.IsFinite(duration) && duration > 0.0 && duration <= TimeSpan.MaxValue.TotalSeconds
				? duration
				: 3.0);
		SelfTargetLockPrototypeId = long.TryParse(root.Element("SelfTargetLockPrototypeId")?.Value, out var targetId)
			? targetId
			: 0L;
		SelfTargetLockPrototypeName = root.Element("SelfTargetLockPrototypeName")?.Value ?? string.Empty;
	}

	protected XElement SaveAccessControlToXml(XElement root)
	{
		root.Add(new XElement("SignalValue", SignalValue));
		root.Add(new XElement("SignalDurationSeconds", SignalDuration.TotalSeconds));
		root.Add(new XElement("SelfTargetLockPrototypeId", SelfTargetLockPrototypeId));
		root.Add(new XElement("SelfTargetLockPrototypeName", new XCData(SelfTargetLockPrototypeName)));
		return root;
	}

	protected sealed override XElement SaveSubtypeToXml(XElement root)
	{
		SaveAccessControlToXml(root);
		return SaveAccessSubtypeToXml(root);
	}

	protected abstract XElement SaveAccessSubtypeToXml(XElement root);

	protected string AccessControlDescription(ICharacter actor)
	{
		var selfTarget = SelfTargetLockPrototypeId > 0 || !string.IsNullOrWhiteSpace(SelfTargetLockPrototypeName)
			? $"{SelfTargetLockPrototypeName.ColourName()} (#{SelfTargetLockPrototypeId.ToString("N0", actor)})"
			: "None".ColourError();
		return
			$"Signal: {SignalValue.ToString("N2", actor).ColourValue()} for {SignalDuration.Describe(actor).ColourValue()}\nMount Type: {AccessMountType.ColourCommand()}\nSelf-Target Lock: {selfTarget}";
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "value":
			case "signal":
				return BuildingCommandSignalValue(actor, command);
			case "duration":
			case "time":
				return BuildingCommandDuration(actor, command);
			case "selftarget":
			case "targetlock":
				return BuildingCommandSelfTarget(actor, command);
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandSignalValue(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, out var value) ||
			!double.IsFinite(value))
		{
			actor.Send("What valid numeric signal value should this reader emit?");
			return false;
		}

		SignalValue = value;
		Changed = true;
		actor.Send($"This reader now emits {value.ToString("N2", actor).ColourValue()} after successful access.");
		return true;
	}

	private bool BuildingCommandDuration(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, out var seconds) ||
			!double.IsFinite(seconds) || seconds <= 0.0 || seconds > TimeSpan.MaxValue.TotalSeconds)
		{
			actor.Send("How many positive seconds should the access signal remain active?");
			return false;
		}

		SignalDuration = TimeSpan.FromSeconds(seconds);
		Changed = true;
		actor.Send($"This reader now remains active for {SignalDuration.Describe(actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandSelfTarget(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which sibling lock component should this reader drive, or NONE?");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			SelfTargetLockPrototypeId = 0L;
			SelfTargetLockPrototypeName = string.Empty;
			Changed = true;
			actor.Send("This reader will no longer directly drive a sibling lock.");
			return true;
		}

		var prototype = long.TryParse(command.SafeRemainingArgument, out var id)
			? Gameworld.ItemComponentProtos.Get(id)
			: Gameworld.ItemComponentProtos.GetByName(command.SafeRemainingArgument);
		if (prototype is not ILockPrototype)
		{
			actor.Send("You must specify an item component prototype that supplies a lock.");
			return false;
		}

		SelfTargetLockPrototypeId = prototype.Id;
		SelfTargetLockPrototypeName = prototype.Name;
		Changed = true;
		actor.Send($"This reader will now directly drive {prototype.Name.ColourName()} when embedded with it.");
		return true;
	}

	public override bool CanSubmit()
	{
		if (SignalDuration <= TimeSpan.Zero || !double.IsFinite(SignalValue))
		{
			return false;
		}

		if (SelfTargetLockPrototypeId > 0 || !string.IsNullOrWhiteSpace(SelfTargetLockPrototypeName))
		{
			var prototype = SelfTargetLockPrototypeId > 0
				? Gameworld.ItemComponentProtos.Get(SelfTargetLockPrototypeId)
				: Gameworld.ItemComponentProtos.GetByName(SelfTargetLockPrototypeName);
			if (prototype is not ILockPrototype)
			{
				return false;
			}
		}

		return base.CanSubmit();
	}

	public override string WhyCannotSubmit()
	{
		if (SignalDuration <= TimeSpan.Zero || !double.IsFinite(SignalValue))
		{
			return "The signal value and duration must be valid.";
		}

		if (SelfTargetLockPrototypeId > 0 || !string.IsNullOrWhiteSpace(SelfTargetLockPrototypeName))
		{
			var prototype = SelfTargetLockPrototypeId > 0
				? Gameworld.ItemComponentProtos.Get(SelfTargetLockPrototypeId)
				: Gameworld.ItemComponentProtos.GetByName(SelfTargetLockPrototypeName);
			if (prototype is not ILockPrototype)
			{
				return "The configured self-target component must be a current lock component prototype.";
			}
		}

		return base.WhyCannotSubmit();
	}
}

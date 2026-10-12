#nullable enable

using System.Globalization;
using System.Xml.Linq;
using MudSharp.Events;
using MudSharp.Character;
using MudSharp.Models;
using MudSharp.Effects.Concrete;

namespace MudSharp.NPC.AI;

/// <summary>Explicit native command surface for lifecycle-bound spell companions.</summary>
public sealed class SpellCompanionAI : ArtificialIntelligenceBase
{
	private static readonly string[] Orders = ["follow", "stop", "guard", "attack"];
	private string MissionRole => Purpose switch { "owned_seek" => "seek", "owned_courier" => "courier", "owned_haunt" => "haunt", _ => "" };
	private readonly HashSet<string> _permitted = new(Orders, StringComparer.OrdinalIgnoreCase);
	public string Mode { get; private set; } = "companion";
	public string Purpose { get; private set; } = "companion";
	public double? LossChancePerGrade { get; private set; }
	public double LossChanceCap { get; private set; } = 0.5;

	private SpellCompanionAI(ArtificialIntelligence ai, IFuturemud gameworld) : base(ai, gameworld)
	{
		var root = XElement.Parse(ai.Definition);
		Mode = root.Element("Mode")?.Value ?? "companion";
		Purpose = root.Element("Purpose")?.Value ?? "companion";
		_permitted.Clear();
		foreach (var value in root.Element("Permitted")?.Elements("Order").Select(x => x.Value) ?? Orders)
		{
			if (!Orders.Contains(value, StringComparer.OrdinalIgnoreCase)) throw new FormatException("Unknown spell companion order.");
			_permitted.Add(value);
		}
		LossChancePerGrade = (double?)root.Element("LossChancePerGrade");
		LossChanceCap = (double?)root.Element("LossChanceCap") ?? 0.5;
		ValidateConfiguration();
	}
	private SpellCompanionAI() { }
	private SpellCompanionAI(IFuturemud gameworld, string name) : base(gameworld, name, "SpellCompanion") => DatabaseInitialise();

	protected override string SaveToXml() => new XElement("Definition", new XElement("Mode", Mode), new XElement("Purpose", Purpose),
		new XElement("Permitted", _permitted.OrderBy(x => x).Select(x => new XElement("Order", x))),
		LossChancePerGrade is { } chance ? new XElement("LossChancePerGrade", chance) : null, new XElement("LossChanceCap", LossChanceCap)).ToString();

	private void ValidateConfiguration()
	{
		if (Mode is not ("companion" or "outsider") || string.IsNullOrWhiteSpace(Purpose) || Purpose.Length > 128 ||
			LossChancePerGrade is { } chance && (!double.IsFinite(chance) || chance < 0 || chance > 1) ||
			!double.IsFinite(LossChanceCap) || LossChanceCap is < 0 or > 0.5 || _permitted.Any(x => !Orders.Contains(x, StringComparer.OrdinalIgnoreCase)))
			throw new FormatException("Spell companion AI has invalid mode, purpose, order subset or loss probabilities.");
	}

	public override bool HandlesEvent(params EventType[] types) => types.Contains(EventType.CommandIssuedToCharacter) ||
		(Mode == "outsider" && LossChancePerGrade is > 0 || MissionRole.Length > 0) && types.Contains(EventType.FiveSecondTick);
	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		if (type == EventType.FiveSecondTick && arguments.Length > 0 && arguments[0] is ICharacter missionCreature &&
			missionCreature.EffectsOfType<SpellNpcControlEffect>().SingleOrDefault() is { } missionControl &&
			missionCreature.EffectsOfType<SpellNpcMissionEffect>().SingleOrDefault() is { } mission)
		{
			var expected = MissionRole;
			if (mission.Mission == expected) mission.Process(missionControl);
			else if (Mode == "outsider" && missionControl.ControlLost) missionControl.AttemptHostileAction();
			return false;
		}
		if (type == EventType.FiveSecondTick && arguments.Length > 0 && arguments[0] is ICharacter lostCreature &&
			lostCreature.EffectsOfType<SpellNpcControlEffect>().SingleOrDefault() is { ControlLost: true } lostBond)
		{
			lostBond.AttemptHostileAction(); return false;
		}
		if (type != EventType.CommandIssuedToCharacter || arguments.Length != 3 || arguments[0] is not ICharacter creature ||
			arguments[1] is not ICharacter commander || arguments[2] is not string raw || creature is not IArtificialIntelligenceControlledCharacter ||
			creature.EffectsOfType<SpellNpcControlEffect>().SingleOrDefault() is not { } bond) return false;

		var stack = new StringStack(raw);
		var requested = stack.PopSpeech().ToLowerInvariant();
		if (!_permitted.Contains(requested)) return false;
		ICharacter? target = null;
		if (requested == "attack")
		{
			if (stack.IsFinished) { commander.Send("Attack whom?"); return true; }
			var targetText = stack.SafeRemainingArgument;
			target = commander.TargetActor(targetText, PerceiveIgnoreFlags.None);
			if (target is null || !ReferenceEquals(target.Gameworld, Gameworld) || !Gameworld.Actors.Any(x => ReferenceEquals(x, target)) ||
				!creature.CanSee(target) || !creature.CanEngage(target)) { commander.Send("That target cannot be engaged by the creature."); return true; }
		}
		else if (!stack.IsFinished) { commander.Send("Use one of the configured orders without extra arguments."); return true; }

		if (!bond.CanAcceptOrder(commander)) { commander.Send("This creature does not recognise your authority."); return true; }
		if (Mode == "outsider" && LossChancePerGrade is { } perGrade && LossChanceCap > 0 &&
			Random.Shared.NextDouble() < Math.Min(perGrade * bond.Grade, LossChanceCap))
		{
			if (bond.LoseControl(commander)) commander.Send("The creature breaks your control.");
			else commander.Send("The creature's control state could not be safely persisted; no order was carried out.");
			return true;
		}
		if (!bond.CanAcceptOrder(commander)) { commander.Send("This creature does not recognise your authority."); return true; }
		if (!bond.SetOrder(requested switch { "stop" => "none", _ => requested }, target, commander))
			commander.Send("The creature's order could not be safely persisted; no order was carried out.");
		return true;
	}

	public static void RegisterLoader()
	{
		RegisterAIType("SpellCompanion", (ai, world) => new SpellCompanionAI(ai, world));
		RegisterAIBuilderInformation("spellcompanion", (world, name) => new SpellCompanionAI(world, name), new SpellCompanionAI().HelpText);
	}

	public override string Show(ICharacter actor) => base.Show(actor) +
		$"\nMode: {Mode.ColourValue()}\nPurpose: {Purpose.ColourValue()}\nPermitted orders: {_permitted.OrderBy(x => x).ListToCommaSeparatedValues(", ")}\n" +
		$"Control loss per grade: {LossChancePerGrade?.ToString("P", actor) ?? "disabled"}\nControl loss cap: {LossChanceCap.ToString("P", actor)}";

	protected override string TypeHelpText => @"
	#3mode companion|outsider#0 - selects ordinary bond or bounded control loss
	#3purpose <text>#0 - sets a descriptive purpose (128 characters maximum)
	#3orders <follow|stop|guard|attack|clear>#0 - toggles one order or restores all four
	#3losschance <0-1>#0 - optional per-grade outsider control-loss chance
	#3losscap <0-0.5>#0 - maximum control-loss chance";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "mode":
				var mode = command.SafeRemainingArgument.ToLowerInvariant();
				if (mode is not ("companion" or "outsider")) { actor.Send("Specify companion or outsider."); return false; }
				Mode = mode; Changed = true; actor.Send($"Mode is now {Mode.ColourValue()}."); return true;
			case "purpose":
				var purpose = command.SafeRemainingArgument;
				if (string.IsNullOrWhiteSpace(purpose) || purpose.Length > 128) { actor.Send("Purpose must contain 1 to 128 characters."); return false; }
				Purpose = purpose; Changed = true; actor.Send("Purpose updated."); return true;
			case "orders":
				var order = command.SafeRemainingArgument.ToLowerInvariant();
				if (order == "clear") _permitted.Clear(); else if (Orders.Contains(order, StringComparer.OrdinalIgnoreCase))
				{ if (!_permitted.Remove(order)) _permitted.Add(order); }
				else { actor.Send("Specify follow, stop, guard, attack or clear."); return false; }
				if (_permitted.Count == 0 && order == "clear") _permitted.UnionWith(Orders);
				Changed = true; actor.Send($"Permitted orders: {_permitted.OrderBy(x => x).ListToCommaSeparatedValues(", ")}."); return true;
			case "losschance":
				if (!double.TryParse(command.SafeRemainingArgument, NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var chance) || !double.IsFinite(chance) || chance is < 0 or > 1)
				{ actor.Send("Specify a finite probability from 0 to 1."); return false; }
				LossChancePerGrade = chance; Changed = true; actor.Send("Control-loss chance updated."); return true;
			case "losscap":
				if (!double.TryParse(command.SafeRemainingArgument, NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var cap) || !double.IsFinite(cap) || cap is < 0 or > 0.5)
				{ actor.Send("Specify a finite cap from 0 to 0.5."); return false; }
				LossChanceCap = cap; Changed = true; actor.Send("Control-loss cap updated."); return true;
			default: return base.BuildingCommand(actor, command.GetUndo());
		}
	}
}

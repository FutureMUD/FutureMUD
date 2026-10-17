#nullable enable

using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellTriggers;

/// <summary>Resolves the caster's room and explicit exact mission inputs through native target parsers.</summary>
public sealed class CastingTriggerRoomMission : CastingTriggerBase
{
	public SpellNpcMissionKind MissionKind { get; private set; }

	public static void RegisterFactory()
	{
		SpellTriggerFactory.RegisterBuilderFactory("roommission", BuilderFactory,
			"Targets the caster's room with explicit native companion mission inputs", "room",
			"mission <seek|courier|haunt> - selects the explicit input syntax");
		SpellTriggerFactory.RegisterLoadTimeFactory("roommission", (root, spell) => new CastingTriggerRoomMission(root, spell));
	}

	private static (IMagicTrigger Trigger, string Error) BuilderFactory(StringStack command, IMagicSpell spell)
	{
		var text = command.IsFinished ? "seek" : command.PopSpeech();
		if (!TryKind(text, out var kind) || !command.IsFinished)
			return (null!, "Use roommission seek, courier or haunt.");
		return (new CastingTriggerRoomMission(new XElement("Trigger", new XAttribute("type", "roommission"),
			new XAttribute("version", 1), new XElement("MinimumPower", (int)SpellPower.Insignificant),
			new XElement("MaximumPower", (int)SpellPower.RecklesslyPowerful), new XElement("Mission", kind)), spell), "");
	}

	public CastingTriggerRoomMission(XElement definition, IMagicSpell spell) : base(definition, spell)
	{
		if ((string?)definition.Attribute("version") != "1" || !TryKind(definition.Element("Mission")?.Value, out var kind))
			throw new FormatException("Room mission triggers require version 1 and an explicit seek, courier or haunt mission.");
		MissionKind = kind;
	}

	private static bool TryKind(string? text, out SpellNpcMissionKind kind) =>
		Enum.TryParse(text, true, out kind) && Enum.IsDefined(kind) && kind != SpellNpcMissionKind.None;

	public override XElement SaveToXml() => new("Trigger", new XAttribute("type", "roommission"),
		new XAttribute("version", 1), new XElement("MinimumPower", (int)MinimumPower),
		new XElement("MaximumPower", (int)MaximumPower), new XElement("Mission", MissionKind));
	public override IMagicTrigger Clone() => new CastingTriggerRoomMission(SaveToXml(), Spell);
	public override string TargetTypes => "room";
	public override bool TriggerYieldsTarget => true;
	public override bool TriggerMayFailToYieldTarget => true;
	public override string SubtypeBuildingCommandHelp => "\t#3mission <seek|courier|haunt>#0 - selects the explicit input syntax";
	public override string Show(ICharacter actor) => $"{"Cast@Room Mission".ColourName()} ({MissionKind.DescribeEnum().ColourName()}) - {base.Show(actor)}";
	public override string ShowPlayer(ICharacter actor) => $"Cast Command - {Spell.School.SchoolVerb} cast {Spell.Name.ToLowerInvariant().DoubleQuotes()} <power> " +
		(MissionKind == SpellNpcMissionKind.Courier ? "<held parcel> to <recipient>" : "<target character>");

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		if (!command.PeekSpeech().EqualTo("mission")) return base.BuildingCommand(actor, command);
		command.PopSpeech();
		if (!TryKind(command.SafeRemainingArgument, out var kind))
		{
			actor.OutputHandler.Send("Specify seek, courier or haunt.".ColourCommand());
			return false;
		}
		MissionKind = kind; Spell.Changed = true;
		actor.OutputHandler.Send($"This trigger now accepts {kind.DescribeEnum().ColourName()} mission inputs.");
		return true;
	}

	public override void DoTriggerCast(ICharacter actor, StringStack additionalArguments)
	{
		if (!CheckBaseTriggerCase(actor, additionalArguments, out var power)) return;
		if (additionalArguments.IsFinished)
		{
			actor.OutputHandler.Send(ShowPlayer(actor).ColourCommand());
			return;
		}
		IGameItem? parcel = null;
		if (MissionKind == SpellNpcMissionKind.Courier)
		{
			var parcelText = additionalArguments.PopSpeech();
			if (additionalArguments.IsFinished || !additionalArguments.PopSpeech().EqualTo("to") || additionalArguments.IsFinished)
			{
				actor.OutputHandler.Send("Specify a held parcel followed by to and its recipient.".ColourCommand());
				return;
			}
			parcel = actor.TargetHeldItem(parcelText);
			if (parcel is null || !actor.Body.HeldOrWieldedItems.Any(x => ReferenceEquals(x, parcel)))
			{
				actor.OutputHandler.Send("You must select one exact parcel you are holding or wielding.".ColourError());
				return;
			}
		}
		var target = actor.TargetActorOrCorpse(additionalArguments.SafeRemainingArgument);
		if (target is null)
		{
			actor.OutputHandler.Send("You do not see a character matching that mission target.".ColourError());
			return;
		}
		Spell.CastSpell(actor, actor.Location, power, new SpellAdditionalParameter
		{
			ParameterName = SpellNpcMissionInputs.ParameterName,
			Item = new SpellNpcMissionInputs(MissionKind, target, parcel)
		});
	}
}

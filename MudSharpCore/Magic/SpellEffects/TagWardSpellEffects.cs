#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public abstract class TagWardSpellEffectBase : IMagicSpellEffectTemplate
{
	protected TagWardSpellEffectBase(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		Tag = root.Element("Tag")?.Value ?? "magic";
		Value = root.Element("Value")?.Value ?? string.Empty;
		MatchValue = bool.Parse(root.Element("MatchValue")?.Value ?? "false");
		Mode = Enum.Parse<MagicInterdictionMode>(root.Element("Mode")?.Value ?? nameof(MagicInterdictionMode.Fail), true);
		Coverage = Enum.Parse<MagicInterdictionCoverage>(root.Element("Coverage")?.Value ?? nameof(MagicInterdictionCoverage.Both), true);
		Prog = Gameworld.FutureProgs.Get(long.Parse(root.Element("Prog")?.Value ?? "0"));
	}

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public string Tag { get; private set; }
	public string Value { get; private set; }
	public bool MatchValue { get; private set; }
	public MagicInterdictionMode Mode { get; private set; }
	public MagicInterdictionCoverage Coverage { get; private set; }
	public IFutureProg? Prog { get; private set; }
	protected abstract string EffectType { get; }
	protected abstract string EffectName { get; }
	protected abstract string[] CompatibleTargetTypes { get; }

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", EffectType),
			new XElement("Tag", new XCData(Tag)),
			new XElement("Value", new XCData(Value)),
			new XElement("MatchValue", MatchValue),
			new XElement("Mode", Mode),
			new XElement("Coverage", Coverage),
			new XElement("Prog", Prog?.Id ?? 0L)
		);
	}

	public bool IsInstantaneous => false;
	public bool RequiresTarget => true;
	public bool IsCompatibleWithTrigger(IMagicTrigger trigger) => CompatibleTargetTypes.Contains(trigger.TargetTypes);

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		return target is null ? null : CreateWardEffect(target, parent);
	}

	protected abstract IMagicSpellEffect? CreateWardEffect(IPerceivable target, IMagicSpellEffectParent parent);
	protected abstract IMagicSpellEffectTemplate CloneEffect(XElement root, IMagicSpell spell);

	public IMagicSpellEffectTemplate Clone() => CloneEffect(SaveToXml(), Spell);

	public string Show(ICharacter actor)
	{
		return
			$"{EffectName.ColourName()} - {Tag.ColourValue()}{(MatchValue ? $"={Value.ColourValue()}" : "")} - {Coverage.DescribeEnum().ColourValue()} - {Mode.DescribeEnum().ColourValue()} - Prog: {Prog?.MXPClickableFunctionName() ?? "None".ColourError()}";
	}

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "tag":
			case "key":
				if (command.IsFinished)
				{
					actor.OutputHandler.Send("Which magic tag should this ward interdict?");
					return false;
				}

				Tag = command.PopSpeech();
				if (!command.IsFinished)
				{
					Value = command.SafeRemainingArgument;
					MatchValue = true;
				}
				Spell.Changed = true;
				actor.OutputHandler.Send($"This ward now catches tag {Tag.ColourValue()}{(MatchValue ? $" with value {Value.ColourValue()}" : "")}.");
				return true;
			case "value":
				if (command.IsFinished || command.SafeRemainingArgument.EqualTo("none"))
				{
					Value = string.Empty;
					MatchValue = false;
				}
				else
				{
					Value = command.SafeRemainingArgument;
					MatchValue = true;
				}
				Spell.Changed = true;
				actor.OutputHandler.Send(MatchValue
					? $"This ward now requires tag value {Value.ColourValue()}."
					: "This ward now matches any value for its tag.");
				return true;
			case "mode":
				if (!command.SafeRemainingArgument.TryParseEnum(out MagicInterdictionMode mode))
				{
					actor.OutputHandler.Send($"Valid modes are {Enum.GetValues<MagicInterdictionMode>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
					return false;
				}

				Mode = mode;
				Spell.Changed = true;
				actor.OutputHandler.Send($"This ward will now {mode.DescribeEnum().ToLowerInvariant().ColourValue()} matching invocations.");
				return true;
			case "coverage":
				if (!command.SafeRemainingArgument.TryParseEnum(out MagicInterdictionCoverage coverage))
				{
					actor.OutputHandler.Send($"Valid coverage values are {Enum.GetValues<MagicInterdictionCoverage>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
					return false;
				}

				Coverage = coverage;
				Spell.Changed = true;
				actor.OutputHandler.Send($"This ward now applies to {coverage.DescribeEnum().ToLowerInvariant().ColourValue()} magic.");
				return true;
			case "prog":
				return BuildingCommandProg(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandProg(ICharacter actor, StringStack command)
	{
		if (command.SafeRemainingArgument.EqualToAny("none", "clear", "delete"))
		{
			Prog = null;
			Spell.Changed = true;
			actor.OutputHandler.Send("This ward no longer uses a custom prog.");
			return true;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean,
			[
				[ProgVariableTypes.Character, ProgVariableTypes.Perceivable],
				[ProgVariableTypes.Character, ProgVariableTypes.Perceivable, ProgVariableTypes.Text, ProgVariableTypes.Text]
			]).LookupProg();
		if (prog is null)
		{
			return false;
		}

		Prog = prog;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This ward will now use {prog.MXPClickableFunctionName()} when deciding whether to interdict.");
		return true;
	}

	public const string HelpText = @"You can use the following options with this effect:

	#3tag <tag> [value]#0 - sets the magic tag and optional value to catch
	#3value <value|none>#0 - sets or clears value matching
	#3mode fail|reflect#0 - sets whether matching invocations fail or reflect
	#3coverage incoming|outgoing|both#0 - sets whether the ward catches incoming, outgoing, or both
	#3prog <prog>|none#0 - sets or clears an optional custom prog";
}

public sealed class RoomTagWardEffect : TagWardSpellEffectBase
{
	private static readonly string[] CompatibleTypes = ["room"];

	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("roomtagward", (root, spell) => new RoomTagWardEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("roomtagward", BuilderFactory,
			"Applies a room ward that catches magic by magic tag",
			HelpText,
			false,
			true,
			SpellTriggerFactory.MagicTriggerTypes
			                   .Where(x => CompatibleTypes.Contains(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes))
			                   .ToArray());
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
	{
		return (new RoomTagWardEffect(DefaultRoot("roomtagward"), spell), string.Empty);
	}

	private static XElement DefaultRoot(string type)
	{
		return new XElement("Effect",
			new XAttribute("type", type),
			new XElement("Tag", new XCData("magic")),
			new XElement("Value", new XCData(string.Empty)),
			new XElement("MatchValue", false),
			new XElement("Mode", MagicInterdictionMode.Fail),
			new XElement("Coverage", MagicInterdictionCoverage.Both),
			new XElement("Prog", 0L)
		);
	}

	private RoomTagWardEffect(XElement root, IMagicSpell spell) : base(root, spell)
	{
	}

	protected override string EffectType => "roomtagward";
	protected override string EffectName => "Room Tag Ward";
	protected override string[] CompatibleTargetTypes => CompatibleTypes;

	protected override IMagicSpellEffect? CreateWardEffect(IPerceivable target, IMagicSpellEffectParent parent)
	{
		return target is ICell ? new SpellRoomTagWardEffect(target, parent, Tag, Value, MatchValue, Mode, Coverage, Prog) : null;
	}

	protected override IMagicSpellEffectTemplate CloneEffect(XElement root, IMagicSpell spell)
	{
		return new RoomTagWardEffect(root, spell);
	}
}

public sealed class PersonalTagWardEffect : TagWardSpellEffectBase
{
	private static readonly string[] CompatibleTypes = ["character", "characters", "character&room", "character&exit"];

	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("personaltagward", (root, spell) => new PersonalTagWardEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("personaltagward", BuilderFactory,
			"Applies a personal ward that catches magic by magic tag",
			HelpText,
			false,
			true,
			SpellTriggerFactory.MagicTriggerTypes
			                   .Where(x => CompatibleTypes.Contains(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes))
			                   .ToArray());
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
	{
		return (new PersonalTagWardEffect(new XElement("Effect",
			new XAttribute("type", "personaltagward"),
			new XElement("Tag", new XCData("magic")),
			new XElement("Value", new XCData(string.Empty)),
			new XElement("MatchValue", false),
			new XElement("Mode", MagicInterdictionMode.Fail),
			new XElement("Coverage", MagicInterdictionCoverage.Both),
			new XElement("Prog", 0L)
		), spell), string.Empty);
	}

	private PersonalTagWardEffect(XElement root, IMagicSpell spell) : base(root, spell)
	{
	}

	protected override string EffectType => "personaltagward";
	protected override string EffectName => "Personal Tag Ward";
	protected override string[] CompatibleTargetTypes => CompatibleTypes;

	protected override IMagicSpellEffect? CreateWardEffect(IPerceivable target, IMagicSpellEffectParent parent)
	{
		return target is ICharacter ? new SpellPersonalTagWardEffect(target, parent, Tag, Value, MatchValue, Mode, Coverage, Prog) : null;
	}

	protected override IMagicSpellEffectTemplate CloneEffect(XElement root, IMagicSpell spell)
	{
		return new PersonalTagWardEffect(root, spell);
	}
}

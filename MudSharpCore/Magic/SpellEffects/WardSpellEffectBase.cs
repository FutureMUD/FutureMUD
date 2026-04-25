using MudSharp.Character;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

#nullable enable

namespace MudSharp.Magic.SpellEffects;

public abstract class WardSpellEffectBase : IMagicSpellEffectTemplate
{
	protected WardSpellEffectBase(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		School = Gameworld.MagicSchools.Get(long.Parse(root.Element("School")!.Value))!;
		Mode = Enum.Parse<MagicInterdictionMode>(root.Element("Mode")?.Value ?? nameof(MagicInterdictionMode.Fail), true);
		Coverage = Enum.Parse<MagicInterdictionCoverage>(root.Element("Coverage")?.Value ?? nameof(MagicInterdictionCoverage.Both), true);
		IncludesSubschools = bool.Parse(root.Element("IncludesSubschools")?.Value ?? "true");
		Prog = Gameworld.FutureProgs.Get(long.Parse(root.Element("Prog")?.Value ?? "0"));
	}

	protected WardSpellEffectBase(IMagicSpell spell)
	{
		Spell = spell;
		School = spell.School;
		Mode = MagicInterdictionMode.Fail;
		Coverage = MagicInterdictionCoverage.Both;
		IncludesSubschools = true;
	}

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public IMagicSchool School { get; protected set; }
	public MagicInterdictionMode Mode { get; protected set; }
	public MagicInterdictionCoverage Coverage { get; protected set; }
	public bool IncludesSubschools { get; protected set; }
	public IFutureProg? Prog { get; protected set; }

	protected abstract string EffectType { get; }
	protected abstract string EffectName { get; }
	protected abstract string[] CompatibleTargetTypes { get; }
	protected abstract IMagicSpellEffect CreateWardEffect(IPerceivable target, IMagicSpellEffectParent parent);

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", EffectType),
			new XElement("School", School.Id),
			new XElement("Mode", Mode),
			new XElement("Coverage", Coverage),
			new XElement("IncludesSubschools", IncludesSubschools),
			new XElement("Prog", Prog?.Id ?? 0L)
		);
	}

	public bool IsInstantaneous => false;
	public bool RequiresTarget => true;

	public bool IsCompatibleWithTrigger(IMagicTrigger trigger)
	{
		return CompatibleTargetTypes.Contains(trigger.TargetTypes);
	}

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (target is null)
		{
			return null;
		}

		return CreateWardEffect(target, parent);
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return CloneEffect(SaveToXml(), Spell);
	}

	protected abstract IMagicSpellEffectTemplate CloneEffect(XElement root, IMagicSpell spell);

	public string Show(ICharacter actor)
	{
		return
			$"{EffectName.ColourName()} - {School.Name.Colour(School.PowerListColour)} - {Coverage.DescribeEnum().ColourValue()} - {Mode.DescribeEnum().ColourValue()}{(IncludesSubschools ? " incl. subschools" : "")} - Prog: {Prog?.MXPClickableFunctionName() ?? "None".ColourError()}";
	}

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "school":
				return BuildingCommandSchool(actor, command);
			case "mode":
				return BuildingCommandMode(actor, command);
			case "coverage":
				return BuildingCommandCoverage(actor, command);
			case "subschool":
			case "subschools":
				return BuildingCommandSubschools(actor);
			case "prog":
				return BuildingCommandProg(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandSchool(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which magic school should this ward interdict?");
			return false;
		}

		IMagicSchool? school = Gameworld.MagicSchools.GetByIdOrName(command.SafeRemainingArgument);
		if (school is null)
		{
			actor.OutputHandler.Send("There is no such magic school.");
			return false;
		}

		School = school;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This ward will now interdict {school.Name.Colour(school.PowerListColour)} magic.");
		return true;
	}

	private bool BuildingCommandMode(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which mode should this ward use? Valid options are {Enum.GetValues<MagicInterdictionMode>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum(out MagicInterdictionMode mode))
		{
			actor.OutputHandler.Send($"That is not a valid interdiction mode. Valid options are {Enum.GetValues<MagicInterdictionMode>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		Mode = mode;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This ward will now {mode.DescribeEnum().ToLowerInvariant().ColourValue()} matching invocations.");
		return true;
	}

	private bool BuildingCommandCoverage(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which coverage should this ward use? Valid options are {Enum.GetValues<MagicInterdictionCoverage>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum(out MagicInterdictionCoverage coverage))
		{
			actor.OutputHandler.Send($"That is not a valid coverage. Valid options are {Enum.GetValues<MagicInterdictionCoverage>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		Coverage = coverage;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This ward now applies to {coverage.DescribeEnum().ToLowerInvariant().ColourValue()} magic.");
		return true;
	}

	private bool BuildingCommandSubschools(ICharacter actor)
	{
		IncludesSubschools = !IncludesSubschools;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This ward will {(IncludesSubschools ? "now" : "no longer")} include child schools of {School.Name.Colour(School.PowerListColour)}.");
		return true;
	}

	private bool BuildingCommandProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which prog should further control whether this ward interdicts? Use {"none".ColourCommand()} to clear it.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("none", "clear", "delete"))
		{
			Prog = null;
			Spell.Changed = true;
			actor.OutputHandler.Send("This ward no longer uses a custom prog.");
			return true;
		}

		IFutureProg? prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean,
			[
				[ProgVariableTypes.Character, ProgVariableTypes.Perceivable],
				[ProgVariableTypes.Character, ProgVariableTypes.Perceivable, ProgVariableTypes.MagicSchool]
			]
		).LookupProg();
		if (prog is null)
		{
			return false;
		}

		Prog = prog;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This ward will now use the {prog.MXPClickableFunctionName()} prog when deciding whether to interdict.");
		return true;
	}

	public const string HelpText = @"You can use the following options with this effect:

	#3school <school>#0 - sets which magic school the ward interdicts
	#3mode fail|reflect#0 - sets whether matching invocations fail or reflect
	#3coverage incoming|outgoing|both#0 - sets whether the ward catches incoming, outgoing, or both
	#3subschools#0 - toggles whether child schools also match
	#3prog <prog>|none#0 - sets or clears an optional custom prog";
}

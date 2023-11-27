using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Lists;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic;

public class MagicSpell : SaveableItem, IMagicSpell
{
	public MagicSpell(Models.MagicSpell spell, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = spell.Id;
		_name = spell.Name;
		Blurb = spell.Blurb;
		Description = spell.Description;
		ExclusiveDelay = TimeSpan.FromSeconds(spell.ExclusiveDelay);
		NonExclusiveDelay = TimeSpan.FromSeconds(spell.NonExclusiveDelay);
		AppliedEffectsAreExclusive = spell.AppliedEffectsAreExclusive;
		School = Gameworld.MagicSchools.Get(spell.MagicSchoolId);
		SpellKnownProg = Gameworld.FutureProgs.Get(spell.SpellKnownProgId);
		EffectDurationExpression = Gameworld.TraitExpressions.Get(spell.EffectDurationExpressionId ?? 0);
		MinimumSuccessThreshold = (Outcome)spell.MinimumSuccessThreshold;

		CastingTrait = Gameworld.Traits.Get(spell.CastingTraitDefinitionId ?? 0);
		OpposedTrait = Gameworld.Traits.Get(spell.ResistingTraitDefinitionId ?? 0);
		CastingDifficulty = (Difficulty)spell.CastingDifficulty;
		OpposedDifficulty = spell.ResistingDifficulty.HasValue ? (Difficulty)spell.ResistingDifficulty : default;

		CastingEmote = spell.CastingEmote;
		FailCastingEmote = spell.FailCastingEmote;
		TargetEmote = spell.TargetEmote;
		TargetResistedEmote = spell.TargetResistedEmote;
		CastingEmoteFlags = (OutputFlags)spell.CastingEmoteFlags;
		TargetEmoteFlags = (OutputFlags)spell.TargetEmoteFlags;

		var definition = XElement.Parse(spell.Definition);
		if (definition.Element("NoTrigger") == null)
		{
			Trigger = SpellTriggerFactory.LoadTrigger(definition.Element("Trigger"), this);
		}

		foreach (var cost in definition.Element("Costs").Elements())
		{
			_castingCosts[Gameworld.MagicResources.Get(long.Parse(cost.Attribute("resource").Value))] =
				Gameworld.TraitExpressions.Get(long.Parse(cost.Attribute("expression").Value));
		}

		foreach (var effect in definition.Element("Effects").Elements())
		{
			_spellEffects.Add(SpellEffectFactory.LoadEffect(effect, this));
		}

		foreach (var effect in definition.Element("CasterEffects").Elements())
		{
			_casterSpellEffects.Add(SpellEffectFactory.LoadEffect(effect, this));
		}

		InventoryPlanTemplate = new InventoryPlanTemplate(definition.Element("Plan"), gameworld);
	}

	public MagicSpell(string name, IMagicSchool school)
	{
		Gameworld = school.Gameworld;
		School = school;
		_name = name;
		Blurb = "An undescribed magic spell";
		Description = "An undescribed magic spell";
		ExclusiveDelay = TimeSpan.Zero;
		NonExclusiveDelay = TimeSpan.Zero;
		AppliedEffectsAreExclusive = true;
		SpellKnownProg = Gameworld.FutureProgs.GetByName("AlwaysFalse");
		InventoryPlanTemplate = new InventoryPlanTemplate(Gameworld,
			new List<IInventoryPlanPhaseTemplate>
				{ new InventoryPlanPhaseTemplate(1, Enumerable.Empty<IInventoryPlanAction>()) });

		CastingEmoteFlags = OutputFlags.Normal;
		TargetEmoteFlags = OutputFlags.Normal;
		MinimumSuccessThreshold = Outcome.Fail;

		using (new FMDB())
		{
			var dbitem = new Models.MagicSpell()
			{
				Name = name,
				Blurb = "An undescribed magic spell",
				Description = "An undescribed magic spell",
				ExclusiveDelay = 0.0,
				NonExclusiveDelay = 0.0,
				MagicSchoolId = school.Id,
				SpellKnownProgId = SpellKnownProg.Id,
				AppliedEffectsAreExclusive = AppliedEffectsAreExclusive,
				Definition = SaveDefinition().ToString()
			};
			FMDB.Context.MagicSpells.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public MagicSpell(MagicSpell rhs, string name)
	{
		Gameworld = rhs.Gameworld;
		School = rhs.School;
		_name = name;
		Blurb = rhs.Blurb;
		Description = rhs.Description;
		AppliedEffectsAreExclusive = rhs.AppliedEffectsAreExclusive;
		ExclusiveDelay = rhs.ExclusiveDelay;
		NonExclusiveDelay = rhs.NonExclusiveDelay;
		SpellKnownProg = rhs.SpellKnownProg;
		Trigger = rhs.Trigger.Clone();
		CastingTrait = rhs.CastingTrait;
		CastingDifficulty = rhs.CastingDifficulty;
		OpposedDifficulty = rhs.OpposedDifficulty;
		OpposedTrait = rhs.OpposedTrait;
		EffectDurationExpression = rhs.EffectDurationExpression;
		MinimumSuccessThreshold = rhs.MinimumSuccessThreshold;
		InventoryPlanTemplate = new InventoryPlanTemplate(rhs.InventoryPlanTemplate.SaveToXml(), Gameworld);

		foreach (var effect in rhs.SpellEffects)
		{
			_spellEffects.Add(effect.Clone());
		}

		foreach (var effect in rhs.CasterSpellEffects)
		{
			_casterSpellEffects.Add(effect.Clone());
		}

		CastingEmote = rhs.CastingEmote;
		FailCastingEmote = rhs.FailCastingEmote;
		TargetEmote = rhs.TargetEmote;
		TargetResistedEmote = rhs.TargetResistedEmote;
		CastingEmoteFlags = rhs.CastingEmoteFlags;
		TargetEmoteFlags = rhs.TargetEmoteFlags;

		foreach (var resource in rhs._castingCosts)
		{
			_castingCosts[resource.Key] = resource.Value;
		}

		using (new FMDB())
		{
			var dbitem = new Models.MagicSpell
			{
				Name = Name,
				Blurb = Blurb,
				Description = Description,
				ExclusiveDelay = ExclusiveDelay.TotalSeconds,
				NonExclusiveDelay = NonExclusiveDelay.TotalSeconds,
				MagicSchoolId = School.Id,
				SpellKnownProgId = SpellKnownProg.Id,
				CastingTraitDefinitionId = CastingTrait?.Id,
				ResistingTraitDefinitionId = OpposedTrait?.Id,
				CastingDifficulty = (int)CastingDifficulty,
				ResistingDifficulty = (int?)OpposedDifficulty,
				EffectDurationExpressionId = EffectDurationExpression?.Id,
				AppliedEffectsAreExclusive = AppliedEffectsAreExclusive,
				Definition = SaveDefinition().ToString()
			};
			FMDB.Context.MagicSpells.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	private XElement SaveDefinition()
	{
		return new XElement("Spell",
			Trigger?.SaveToXml() ?? new XElement("NoTrigger"),
			new XElement("Costs",
				from cost in _castingCosts
				select new XElement("Cost", new XAttribute("resource", cost.Key.Id),
					new XAttribute("expression", cost.Value.Id))
			),
			new XElement("Effects",
				from effect in _spellEffects
				select effect.SaveToXml()
			),
			new XElement("CasterEffects",
				from effect in _casterSpellEffects
				select effect.SaveToXml()
			),
			InventoryPlanTemplate.SaveToXml()
		);
	}

	#region Overrides of FrameworkItem

	public override string FrameworkItemType => "MagicSpell";

	#endregion

	#region Overrides of SaveableItem

	public override void Save()
	{
		var dbitem = FMDB.Context.MagicSpells.Find(Id);
		dbitem.Name = Name;
		dbitem.Blurb = Blurb;
		dbitem.Description = Description;
		dbitem.ExclusiveDelay = ExclusiveDelay.TotalSeconds;
		dbitem.NonExclusiveDelay = NonExclusiveDelay.TotalSeconds;
		dbitem.MagicSchoolId = School.Id;
		dbitem.SpellKnownProgId = SpellKnownProg.Id;
		dbitem.CastingTraitDefinitionId = CastingTrait?.Id;
		dbitem.ResistingTraitDefinitionId = OpposedTrait?.Id;
		dbitem.CastingDifficulty = (int)CastingDifficulty;
		dbitem.ResistingDifficulty = (int?)OpposedDifficulty;
		dbitem.EffectDurationExpressionId = EffectDurationExpression?.Id;
		dbitem.CastingEmote = CastingEmote;
		dbitem.FailCastingEmote = FailCastingEmote;
		dbitem.TargetEmote = TargetEmote;
		dbitem.TargetResistedEmote = TargetResistedEmote;
		dbitem.CastingEmoteFlags = (int)CastingEmoteFlags;
		dbitem.TargetEmoteFlags = (int)TargetEmoteFlags;
		dbitem.MinimumSuccessThreshold = (int)MinimumSuccessThreshold;
		dbitem.Definition = SaveDefinition().ToString();
		Changed = false;
	}

	#endregion

	#region Implementation of IEditableItem

	private const string BuildingCommandHelp = @"You can use the following options with this command:

    #3name <name>#0 - renames this spell
    #3summary <text>#0 - the summary text that appears in the SPELLS output
    #3description#0 - drops you into an editor for a more detailed description
    #3school <school>#0 - changes the school of this spell
    #3prog <prog>#0 - sets the prog that controls a character knowing the spell
    #3exclusivedelay <seconds>#0 - sets the post-cast lockout of all spells
    #3nonexclusivedelay <seconds>#0 - sets the post-cast lockout of same school spells
    #3trigger new <type> [...]#0 - changes the trigger for the spell to a new type
    #3trigger set ...#0 - changes properties of the trigger. See individual trigger help.
    #3effect add <type> [...]#0 - adds a new effect to the spell
    #3effect remove <##>#0 - removes an effect from the spell
    #3effect <##> ...#0 - changes the properties of a spell effect
	#3castereffect add <type> [...]#0 - adds a new caster-only effect to the spell
    #3castereffect remove <##>#0 - removes a caster-only effect from the spell
    #3castereffect <##> ...#0 - changes the properties of a caster-only spell effect
    #3material add held|wielded|inroom|consumed|consumedliquid ...#0 - adds a new material requirement to this spell
    #3material delete <#>#0 - deletes a material requirement
    #3cost <resource> <trait expression>#0 - sets the trait expression for casting cost for a resource
    #3cost <resource> remove#0 - removes a casting cost for a resource
    #3castemote <emote>#0 - sets the cast emote. $0 is caster, $1 is target (if any)
    #3targetemote <emote>#0 - sets the target emote. $0 is caster, $1 is target (if any)
    #3failcastemote <emote>#0 - sets the fail cast emote. $0 is caster, $1 is target (if any)
    #3targetresistemote <emote>#0 - sets the target resist emote. $0 is caster, $1 is target (if any)
    #3emoteflags <flags>#0 - changes the output flags for the casting emotes
    #3targetemoteflags <flags>#0 - changes the output flags for the target emotes
    #3trait <skill/attribute>#0 - sets the trait used for casting this spell
    #3difficulty <difficulty>#0 - sets the difficulty of casting this spell
    #3threshold <outcome>#0 - sets the minimum success threshold for casting the spell
    #3resist none#0 - makes this spell not resisted
    #3resist <trait> <difficulty>#0 - makes this spell resisted by a trait check
    #3duration <trait expression>#0 - sets the trait expression that controls effect duration
	#3exclusiveeffect#0 - toggles whether effects are exclusive (and overwrite) or not (and stack)";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "blurb":
			case "summary":
				return BuildingCommandBlurb(actor, command);
			case "exclusiveeffect":
				return BuildingCommandExclusiveEffect(actor);
			case "description":
			case "desc":
				return BuildingCommandDescription(actor);
			case "threshold":
				return BuildingCommandThreshold(actor, command);
			case "school":
				return BuildingCommandSchool(actor, command);
			case "prog":
				return BuildingCommandProg(actor, command);
			case "exclusivedelay":
				return BuildingCommandExclusiveDelay(actor, command);
			case "nonexclusivedelay":
				return BuildingCommandNonExclusiveDelay(actor, command);
			case "trigger":
				return BuildingCommandTrigger(actor, command);
			case "effect":
				return BuildingCommandEffect(actor, command);
			case "castereffect":
				return BuildingCommandCasterEffect(actor, command);
			case "plan":
			case "material":
				return BuildingCommandPlan(actor, command);
			case "cost":
				return BuildingCommandCost(actor, command);
			case "castemote":
				return BuildingCommandCastEmote(actor, command);
			case "targetemote":
				return BuildingCommandTargetEmote(actor, command);
			case "failcastemote":
				return BuildingCommandFailCastEmote(actor, command);
			case "targetresistemote":
				return BuildingCommandTargetResistEmote(actor, command);
			case "emoteflags":
				return BuildingCommandEmoteFlags(actor, command);
			case "targetemoteflags":
				return BuildingCommandTargetEmoteFlags(actor, command);
			case "trait":
				return BuildingCommandTrait(actor, command);
			case "difficulty":
				return BuildingCommandDifficulty(actor, command);
			case "resist":
				return BuildingCommandResist(actor, command);
			case "duration":
				return BuildingCommandDuration(actor, command);
			default:
				actor.OutputHandler.Send(BuildingCommandHelp);
				return false;
		}
	}

	private bool BuildingCommandCasterEffect(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "add":
			case "new":
			case "create":
				return BuildingCommandCasterEffectAdd(actor, command);
			case "remove":
			case "rem":
			case "delete":
			case "del":
				return BuildingCommandCasterEffectDelete(actor, command);
		}

		if (string.IsNullOrEmpty(command.Last) || !int.TryParse(command.Last, out var value))
		{
			actor.OutputHandler.Send(
				"You must specify ADD, REMOVE, or the # of a spell effect that you want to edit.");
			return false;
		}

		if (_casterSpellEffects.Count == 0)
		{
			actor.OutputHandler.Send(
				"There are not currently any caster spell effects on this spell for you to edit. You must add one first.");
			return false;
		}

		if (value < 1 || value > _casterSpellEffects.Count)
		{
			actor.OutputHandler.Send(
				$"You must enter a number between {1.ToString("N0", actor).ColourValue()} and {_casterSpellEffects.Count.ToString("N0", actor).ColourValue()}.");
			return false;
		}

		var effect = _casterSpellEffects[value - 1];
		return effect.BuildingCommand(actor, command);
	}

	private bool BuildingCommandCasterEffectDelete(ICharacter actor, StringStack command)
	{
		if (_casterSpellEffects.Count == 0)
		{
			actor.OutputHandler.Send(
				"There are not currently any caster spell effects on this spell for you to remove.");
			return false;
		}

		if (command.IsFinished || !int.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send(
				"You must specify the number of a caster spell effect that you wish to remove.");
			return false;
		}

		if (value < 1 || value > _casterSpellEffects.Count)
		{
			actor.OutputHandler.Send(
				$"You must enter a number between {1.ToString("N0", actor).ColourValue()} and {_casterSpellEffects.Count.ToString("N0", actor).ColourValue()}.");
			return false;
		}

		var effect = _casterSpellEffects[value - 1];
		_casterSpellEffects.RemoveAt(value - 1);
		Changed = true;
		actor.OutputHandler.Send(
			$"You remove the {value.ToOrdinal().ColourValue()} caster spell effect: {effect.Show(actor)}");
		return true;
	}

	private bool BuildingCommandCasterEffectAdd(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which effect type do you want to set up for this spell?\nThe valid options are: {SpellEffectFactory.MagicEffectTypes.Select(x => x.ColourValue()).ListToString()}.");
			return false;
		}

		var (effect, error) = SpellEffectFactory.LoadEffectFromBuilderInput(command.PopSpeech(), command, this);
		if (effect == null)
		{
			actor.OutputHandler.Send(error);
			return false;
		}

		_casterSpellEffects.Add(effect);
		Changed = true;
		actor.OutputHandler.Send(
			$"The {Name.Colour(School.PowerListColour)} spell now has the following caster-only effect (#{_casterSpellEffects.Count.ToString("N0", actor)}):\n{effect.Show(actor)}");
		return true;
	}

	private bool BuildingCommandExclusiveEffect(ICharacter actor)
	{
		AppliedEffectsAreExclusive = !AppliedEffectsAreExclusive;
		Changed = true;
		if (AppliedEffectsAreExclusive)
		{
			actor.OutputHandler.Send(
				"The effects applied by this spell are now exclusive, and will overwrite rather than stack.");
		}
		else
		{
			actor.OutputHandler.Send(
				"The effects applied by this spell are no longer exclusive, and will stack rather than overwrite.");
		}

		return true;
	}

	private bool BuildingCommandThreshold(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What would you like to set as the minimum threshold needed for success on the casting check? The valid values are {Enum.GetValues<Outcome>().Except(new List<Outcome> { Outcome.None, Outcome.NotTested }).Select(x => x.DescribeColour()).ListToString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<Outcome>(out var outcome) ||
		    outcome.In(Outcome.None, Outcome.NotTested))
		{
			actor.OutputHandler.Send(
				$"That is not a valid outcome. The valid values are {Enum.GetValues<Outcome>().Except(new List<Outcome> { Outcome.None, Outcome.NotTested }).Select(x => x.DescribeColour()).ListToString()}.");
			return false;
		}

		MinimumSuccessThreshold = outcome;
		Changed = true;
		actor.OutputHandler.Send(
			$"The casting test for this spell will now require a minimum threshold of {outcome.DescribeColour()} to be considered a successful cast.");
		return true;
	}

	private bool BuildingCommandResist(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"You must either specify 'none' to make this spell have no resist check, or specify a trait to be used for the resistance check.");
			return false;
		}

		if (command.PeekSpeech().EqualToAny("none", "clear", "delete"))
		{
			OpposedDifficulty = null;
			OpposedTrait = null;
			Changed = true;
			actor.OutputHandler.Send("This spell will no longer afford its targets a resistance check.");
			return false;
		}

		var trait = Gameworld.Traits.GetByIdOrName(command.PopSpeech());
		if (trait == null)
		{
			actor.OutputHandler.Send("There is no such trait.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a difficulty for the target's resistance check.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<Difficulty>(out var difficutly))
		{
			actor.OutputHandler.Send(
				"That is not a valid difficulty. Please see SHOW DIFFICULTIES for a list of difficulties.");
			return false;
		}

		OpposedTrait = trait;
		OpposedDifficulty = difficutly;
		Changed = true;
		actor.OutputHandler.Send(
			$"This spell will now have its targets make a {difficutly.Describe().ColourValue()} check with the {trait.Name.ColourName()} trait.");
		return true;
	}

	private bool BuildingCommandDifficulty(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What difficulty do you want to set for the casting roll?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<Difficulty>(out var difficulty))
		{
			actor.OutputHandler.Send(
				"That is not a valid difficulty. See SHOW DIFFICULTIES for a list of difficulties.");
			return false;
		}

		CastingDifficulty = difficulty;
		Changed = true;
		actor.OutputHandler.Send($"It will now be {difficulty.Describe().ColourValue()} to cast this spell.");
		return true;
	}

	private bool BuildingCommandTrait(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which trait do you want to use for the check for casting this spell?");
			return false;
		}

		var trait = Gameworld.Traits.GetByIdOrName(command.SafeRemainingArgument);
		if (trait == null)
		{
			actor.OutputHandler.Send("There is no such trait.");
			return false;
		}

		CastingTrait = trait;
		Changed = true;
		actor.OutputHandler.Send($"This spell will now use the {trait.Name.ColourValue()} trait for casting checks.");
		return true;
	}

	private bool BuildingCommandTargetEmoteFlags(ICharacter actor, StringStack command)
	{
		var flags = OutputFlags.Normal;
		while (!command.IsFinished)
		{
			if (!command.PopSpeech().TryParseEnum<OutputFlags>(out var flag))
			{
				actor.OutputHandler.Send(
					$"Unfortunately {command.Last.ColourName()} is not a valid output flag. The valid flags are {OutputFlags.Normal.GetSingleFlags().Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
			}

			flags |= flag;
		}

		Changed = true;
		TargetEmoteFlags = flags;
		actor.OutputHandler.Send(
			$"The target emote will now have the following flags: {flags.DescribeEnum(colour: Telnet.Cyan)}.");
		return true;
	}

	private bool BuildingCommandEmoteFlags(ICharacter actor, StringStack command)
	{
		var flags = OutputFlags.Normal;
		while (!command.IsFinished)
		{
			if (!command.PopSpeech().TryParseEnum<OutputFlags>(out var flag))
			{
				actor.OutputHandler.Send(
					$"Unfortunately {command.Last.ColourName()} is not a valid output flag. The valid flags are {OutputFlags.Normal.GetSingleFlags().Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
			}

			flags |= flag;
		}

		Changed = true;
		CastingEmoteFlags = flags;
		actor.OutputHandler.Send(
			$"The casting emote will now have the following flags: {flags.DescribeEnum(colour: Telnet.Cyan)}.");
		return true;
	}

	private bool BuildingCommandDuration(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which trait expression do you want to use to determine the duration of this spell's effect?");
			return false;
		}

		var expression = Gameworld.TraitExpressions.GetByIdOrName(command.SafeRemainingArgument);
		if (expression == null)
		{
			actor.OutputHandler.Send("That is not a valid trait expression.");
			return false;
		}

		EffectDurationExpression = expression;
		actor.OutputHandler.Send(
			$"This spell will now use expression #{expression.Id} {expression.Name.ColourName()} ({expression.OriginalFormulaText.ColourCommand()}) to determine duration.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandTargetResistEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to set the target resist emote to?");
			return false;
		}

		var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable(),
			new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		TargetResistedEmote = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send(
			$"This spell will now have the following target resist emote:\n{CastingEmote.ColourCommand()}\nNote: $0 is the caster, $1 is the target (if applicable)");
		return true;
	}

	private bool BuildingCommandFailCastEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to set the fail casting emote to?");
			return false;
		}

		var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable(),
			new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		FailCastingEmote = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send(
			$"This spell will now have the following fail casting emote:\n{CastingEmote.ColourCommand()}\nNote: $0 is the caster, $1 is the target (if applicable)");
		return true;
	}

	private bool BuildingCommandTargetEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to set the target emote to?");
			return false;
		}

		var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable(),
			new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		TargetEmote = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send(
			$"This spell will now have the following target emote:\n{CastingEmote.ColourCommand()}\nNote: $0 is the caster, $1 is the target (if applicable)");
		return true;
	}

	private bool BuildingCommandCastEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to set the casting emote to?");
			return false;
		}

		var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable(),
			new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		CastingEmote = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send(
			$"This spell will now have the following casting emote:\n{CastingEmote.ColourCommand()}\nNote: $0 is the caster, $1 is the target (if applicable)");
		return true;
	}

	private bool BuildingCommandCost(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which magic resource do you want to change the cost for?");
			return false;
		}

		var resource = Gameworld.MagicResources.GetByIdOrName(command.PopSpeech());
		if (resource == null)
		{
			actor.OutputHandler.Send("There is no such magic resource.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must either specify a trait expression for the cost of this resource, or use {"remove".ColourCommand()} to remove the existing cost.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("remove"))
		{
			_castingCosts.Remove(resource);
			Changed = true;
			actor.OutputHandler.Send(
				$"It will no longer cost any {resource.Name.Pluralise().ColourValue()} to cast this spell.");
			return true;
		}

		var expression = Gameworld.TraitExpressions.GetByIdOrName(command.SafeRemainingArgument);
		if (expression == null)
		{
			actor.OutputHandler.Send("There is no such trait expression.");
			return false;
		}

		_castingCosts[resource] = expression;
		Changed = true;
		actor.OutputHandler.Send(
			$"This spell will now cost {resource.Name.ColourValue()}, determined by the expression {expression.Name.ColourName()} ({expression.OriginalFormulaText.ColourCommand()})");
		return true;
	}

	private bool BuildingCommandPlan(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "add":
			case "new":
			case "create":
				return BuildingCommandPlanAdd(actor, command);
			case "remove":
			case "rem":
			case "delete":
			case "del":
				return BuildingCommandPlanDelete(actor, command);
		}

		actor.OutputHandler.Send($"You must either ADD a material to the plan or REMOVE one from the plan.");
		return false;
	}

	private bool BuildingCommandPlanDelete(ICharacter actor, StringStack command)
	{
		var count = InventoryPlanTemplate.Phases.First().Actions.Count();
		if (count == 0)
		{
			actor.OutputHandler.Send(
				"There are not currently any material requirements on this spell for you to remove.");
			return false;
		}

		if (command.IsFinished || !int.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send(
				"You must specify the number of a material requirement that you wish to remove.");
			return false;
		}

		if (value < 1 || value > count)
		{
			actor.OutputHandler.Send(
				$"You must enter a number between {1.ToString("N0", actor).ColourValue()} and {count.ToString("N0", actor).ColourValue()}.");
			return false;
		}

		var action = InventoryPlanTemplate.Phases.First().Actions.ElementAt(value - 1);
		InventoryPlanTemplate.Phases.First().RemoveAction(action);
		Changed = true;
		actor.OutputHandler.Send(
			$"You remove the {value.ToOrdinal().ColourValue()} material requirement:\n{action.Describe(actor)}");
		return true;
	}

	private bool BuildingCommandPlanAdd(ICharacter actor, StringStack command)
	{
		var action = GameItems.Inventory.Plans.InventoryPlanTemplate.ParseActionFromBuilderInput(actor, command);
		if (action is null)
		{
			return false;
		}

		InventoryPlanTemplate.Phases.First().AddAction(action);
		actor.OutputHandler.Send(
			$"You add the following new material requirement to the spell:\n{action.Describe(actor)}");
		Changed = true;
		return true;
	}

	private bool BuildingCommandEffect(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "add":
			case "new":
			case "create":
				return BuildingCommandEffectAdd(actor, command);
			case "remove":
			case "rem":
			case "delete":
			case "del":
				return BuildingCommandEffectDelete(actor, command);
		}

		if (string.IsNullOrEmpty(command.Last) || !int.TryParse(command.Last, out var value))
		{
			actor.OutputHandler.Send(
				"You must specify ADD, REMOVE, or the # of a spell effect that you want to edit.");
			return false;
		}

		if (_spellEffects.Count == 0)
		{
			actor.OutputHandler.Send(
				"There are not currently any spell effects on this spell for you to edit. You must add one first.");
			return false;
		}

		if (value < 1 || value > _spellEffects.Count)
		{
			actor.OutputHandler.Send(
				$"You must enter a number between {1.ToString("N0", actor).ColourValue()} and {_spellEffects.Count.ToString("N0", actor).ColourValue()}.");
			return false;
		}

		var effect = _spellEffects[value - 1];
		return effect.BuildingCommand(actor, command);
	}

	private bool BuildingCommandEffectDelete(ICharacter actor, StringStack command)
	{
		if (_spellEffects.Count == 0)
		{
			actor.OutputHandler.Send(
				"There are not currently any spell effects on this spell for you to remove.");
			return false;
		}

		if (command.IsFinished || !int.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send(
				"You must specify the number of a spell effect that you wish to remove.");
			return false;
		}

		if (value < 1 || value > _spellEffects.Count)
		{
			actor.OutputHandler.Send(
				$"You must enter a number between {1.ToString("N0", actor).ColourValue()} and {_spellEffects.Count.ToString("N0", actor).ColourValue()}.");
			return false;
		}

		var effect = _spellEffects[value - 1];
		_spellEffects.RemoveAt(value - 1);
		Changed = true;
		actor.OutputHandler.Send(
			$"You remove the {value.ToOrdinal().ColourValue()} spell effect: {effect.Show(actor)}");
		return true;
	}

	private bool BuildingCommandEffectAdd(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which effect type do you want to set up for this spell?\nThe valid options are: {SpellEffectFactory.MagicEffectTypes.Select(x => x.ColourValue()).ListToString()}.");
			return false;
		}

		var (effect, error) = SpellEffectFactory.LoadEffectFromBuilderInput(command.PopSpeech(), command, this);
		if (effect == null)
		{
			actor.OutputHandler.Send(error);
			return false;
		}

		_spellEffects.Add(effect);
		Changed = true;
		actor.OutputHandler.Send(
			$"The {Name.Colour(School.PowerListColour)} spell now has the following effect (#{_spellEffects.Count.ToString("N0", actor)}):\n{effect.Show(actor)}");
		return true;
	}

	private bool BuildingCommandBlurb(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What blurb do you want to give this spell for the spells list?");
			return false;
		}

		Blurb = command.SafeRemainingArgument.Trim().ProperSentences();
		actor.OutputHandler.Send(
			$"You set the blurb for the {_name.Colour(School.PowerListColour)} spell to {Blurb.ColourCommand()}.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandTriggerNew(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which trigger type do you want to set up for this spell?\nThe valid options are: {SpellTriggerFactory.MagicTriggerTypes.Select(x => x.ColourValue()).ListToString()}.");
			return false;
		}

		var (trigger, error) = SpellTriggerFactory.LoadTriggerFromBuilderInput(command.PopSpeech(), command, this);
		if (trigger == null)
		{
			actor.OutputHandler.Send(error);
			return false;
		}

		Trigger = trigger;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {Name.Colour(School.PowerListColour)} spell now has the following trigger:\n{trigger.Show(actor)}");
		return true;
	}

	private bool BuildingCommandTrigger(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "new":
				return BuildingCommandTriggerNew(actor, command);
			case "set":
				if (Trigger == null)
				{
					actor.OutputHandler.Send(
						"There is no magic trigger set up for this spell. Use MAGIC SPELL SET TRIGGER NEW to create a new one.");
					return false;
				}

				return Trigger.BuildingCommand(actor, command);
			default:
				actor.OutputHandler.Send(
					"You must either specify new (to create a new trigger) or set (to edit the properties of the existing one).");
				return false;
		}
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to this spell?");
			return false;
		}

		var name = command.SafeRemainingArgument.ToLowerInvariant().TitleCase();
		if (Gameworld.MagicSpells.Where(x => x.School == School).Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				"There is already a spell for that magic school with that name. Names must be unique per school.");
			return false;
		}

		actor.OutputHandler.Send(
			$"You rename the magic spell {_name.Colour(School.PowerListColour)} to {name.Colour(School.PowerListColour)}.");
		_name = name;
		Changed = true;
		return true;
	}

	private bool BuildingCommandDescription(ICharacter actor)
	{
		if (!string.IsNullOrEmpty(Description))
		{
			actor.OutputHandler.Send("Replacing:\n" + Description.Wrap(actor.InnerLineFormatLength, "\t"));
		}

		actor.OutputHandler.Send("Enter the description in the editor below.");
		actor.EditorMode(BuildingCommandDescPost, BuildingCommandDescCancel, 1.0, Description);
		return true;
	}

	private void BuildingCommandDescCancel(IOutputHandler handler, object[] parameters)
	{
		handler.Send("You decide not to change the description.");
	}

	private void BuildingCommandDescPost(string description, IOutputHandler handler, object[] parameters)
	{
		Description = description.Trim().ProperSentences();
		Changed = true;
		handler.Send(
			$"You set the description for the spell {Name.Colour(School.PowerListColour)} to:\n\n{Description.Wrap(80, "\t")}");
	}

	private bool BuildingCommandSchool(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which school would you like to change this spell to belong to?");
			return false;
		}

		var school = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.MagicSchools.Get(value)
			: Gameworld.MagicSchools.GetByName(command.SafeRemainingArgument);
		if (school == null)
		{
			actor.OutputHandler.Send("There is no such magic school.");
			return false;
		}

		if (Gameworld.MagicSpells.Where(x => x.School == school).Any(x => x.Name.EqualTo(Name)))
		{
			actor.OutputHandler.Send(
				$"The {school.Name.ColourName()} school of magic already has a spell named {Name.Colour(school.PowerListColour)}, and names must be unique per school.");
			return false;
		}

		School = school;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {Name.Colour(School.PowerListColour)} spell now belongs to the {School.Name.ColourName()} school.");
		return true;
	}

	private bool BuildingCommandProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which prog would you like to use to control whether a character knows this spell?");
			return false;
		}

		var prog = Gameworld.FutureProgs.GetByIdOrName(command.SafeRemainingArgument);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (!prog.ReturnType.CompatibleWith(FutureProgVariableTypes.Boolean))
		{
			actor.OutputHandler.Send(
				$"The prog that you supply must have a return type of {FutureProgVariableTypes.Boolean.Describe().ColourName()}, whereas {prog.MXPClickableFunctionName()} returns {prog.ReturnType.Describe().ColourName()}.");
			return false;
		}

		if (!prog.MatchesParameters(new List<FutureProgVariableTypes> { FutureProgVariableTypes.Character }) &&
		    !prog.MatchesParameters(new List<FutureProgVariableTypes>
			    { FutureProgVariableTypes.Character, FutureProgVariableTypes.MagicSpell }))
		{
			actor.OutputHandler.Send(
				$"The prog that you supply must accept a single character parameter, or a character and a spell, whereas {prog.MXPClickableFunctionName()} does not.");
			return false;
		}

		SpellKnownProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {Name.Colour(School.PowerListColour)} spell now uses the {prog.MXPClickableFunctionName()} prog to determine whether a character knows it.");
		return true;
	}

	private bool BuildingCommandExclusiveDelay(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What do you want to be the exclusive delay for this spell, in seconds?\nThe exclusive delay means that all other spells are blocked for the duration after casting.\nUsing 0 disables the exclusive delay.");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value) || value < 0.0)
		{
			actor.OutputHandler.Send(
				"You must enter a valid number of seconds to block all other spell-casting after using this spell, or 0 to not block.");
			return false;
		}

		ExclusiveDelay = TimeSpan.FromSeconds(value);
		Changed = true;
		actor.OutputHandler.Send(
			$"This spell will now block all spell-casting for {ExclusiveDelay.Describe().ColourValue()} after using this spell.");
		return true;
	}

	private bool BuildingCommandNonExclusiveDelay(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What do you want to be the non-exclusive delay for this spell, in seconds?\nThe non-exclusive delay means that all other spells in the same school are blocked for the duration after casting.\nUsing 0 disables the non-exclusive delay.");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value) || value < 0.0)
		{
			actor.OutputHandler.Send(
				"You must enter a valid number of seconds to block all other spell-casting from the same school after using this spell, or 0 to not block.");
			return false;
		}

		NonExclusiveDelay = TimeSpan.FromSeconds(value);
		Changed = true;
		actor.OutputHandler.Send(
			$"This spell will now block all spell-casting from the same school for {NonExclusiveDelay.Describe().ColourValue()} after using this spell.");
		return true;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Magic Spell #{Id.ToString("N0", actor)} - {Name}");
		sb.AppendLine($"Summary: {Blurb.ColourCommand()}");
		sb.AppendLine($"School: {School.Name.Colour(School.PowerListColour)}");
		sb.AppendLine($"Exclusive Delay: {ExclusiveDelay.Describe().ColourValue()}");
		sb.AppendLine($"Non-Exclusive Delay: {NonExclusiveDelay.Describe().ColourValue()}");
		sb.AppendLine($"Known Prog: {SpellKnownProg?.MXPClickableFunctionNameWithId() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine(
			$"Effect Duration Expression: {(EffectDurationExpression is not null ? $"#{EffectDurationExpression.Id.ToString("N0", actor)} {EffectDurationExpression.Name.ColourName()} ({EffectDurationExpression.OriginalFormulaText.ColourCommand()})" : "None".Colour(Telnet.Red))}");
		sb.AppendLine($"Casting Trait: {CastingTrait?.Name.ColourValue() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine($"Casting Difficulty: {CastingDifficulty.Describe().ColourValue()}");
		sb.AppendLine($"Casting Threshold: {MinimumSuccessThreshold.DescribeColour()}");
		sb.AppendLine($"Resisting Trait: {OpposedTrait?.Name.ColourValue() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine($"Resisting Difficulty: {OpposedDifficulty?.Describe().ColourValue() ?? "N/A"}");
		sb.AppendLine();
		sb.AppendLine($"Casting Emote: {CastingEmote?.ColourCommand() ?? ""}");
		sb.AppendLine($"Fail Casting Emote: {FailCastingEmote?.ColourCommand() ?? ""}");
		sb.AppendLine($"Emote Flags: {CastingEmoteFlags.DescribeEnum(colour: Telnet.Green)}");
		sb.AppendLine($"Target Emote: {TargetEmote?.ColourCommand() ?? ""}");
		sb.AppendLine($"Target Resisted Emote: {TargetResistedEmote?.ColourCommand() ?? ""}");
		sb.AppendLine($"Target Emote Flags: {TargetEmoteFlags.DescribeEnum(colour: Telnet.Green)}");
		sb.AppendLine();
		sb.AppendLine($"Trigger: {Trigger?.Show(actor) ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine();
		sb.AppendLine(Description.Wrap(actor.InnerLineFormatLength, "\t"));
		sb.AppendLine();
		sb.AppendLine("Effects:");
		var i = 0;
		foreach (var effect in SpellEffects)
		{
			sb.AppendLine($"\t{(++i).ToString("N0", actor)}) {effect.Show(actor)}");
		}

		sb.AppendLine();
		sb.AppendLine("Caster Effects:");
		i = 0;
		foreach (var effect in CasterSpellEffects)
		{
			sb.AppendLine($"\t{(++i).ToString("N0", actor)}) {effect.Show(actor)}");
		}

		sb.AppendLine();
		sb.AppendLine("Casting Costs:");
		foreach (var (resource, formula) in _castingCosts)
		{
			sb.AppendLine($"\t{resource.Name.ColourValue()}: {formula.OriginalFormulaText.ColourCommand()}");
		}

		sb.AppendLine();
		i = 0;
		sb.AppendLine("Material Requirements:");
		foreach (var action in InventoryPlanTemplate.Phases.First().Actions)
		{
			sb.AppendLine($"\t{(++i).ToString("N0", actor)}) {action.Describe(actor)}");
		}

		if (!ReadyForGame)
		{
			sb.AppendLine();
			sb.AppendLine($"Building Error: {WhyNotReadyForGame}");
		}

		return sb.ToString();
	}

	#endregion

	#region Implementation of IMagicSpell

	public IFutureProg SpellKnownProg { get; set; }
	public IMagicSchool School { get; set; }
	public string Description { get; set; }
	public string Blurb { get; set; }
	public TimeSpan ExclusiveDelay { get; set; }
	public TimeSpan NonExclusiveDelay { get; set; }
	public ITraitDefinition CastingTrait { get; set; }
	public Difficulty CastingDifficulty { get; set; }
	public bool AppliedEffectsAreExclusive { get; set; }
	[CanBeNull] public ITraitDefinition OpposedTrait { get; set; }
	public Outcome MinimumSuccessThreshold { get; set; }
	public Difficulty? OpposedDifficulty { get; set; }
	public ITraitExpression EffectDurationExpression { get; set; }

	public IInventoryPlanTemplate InventoryPlanTemplate { get; set; }

	public IMagicTrigger Trigger { get; set; }

	private readonly List<IMagicSpellEffectTemplate> _spellEffects = new();
	public IEnumerable<IMagicSpellEffectTemplate> SpellEffects => _spellEffects;

	private readonly List<IMagicSpellEffectTemplate> _casterSpellEffects = new();
	public IEnumerable<IMagicSpellEffectTemplate> CasterSpellEffects => _casterSpellEffects;

	private readonly Dictionary<IMagicResource, ITraitExpression> _castingCosts = new();

	public string CastingEmote { get; set; }
	public string FailCastingEmote { get; set; }
	public string TargetEmote { get; set; }
	public string TargetResistedEmote { get; set; }
	public OutputFlags CastingEmoteFlags { get; set; }
	public OutputFlags TargetEmoteFlags { get; set; }

	public void CastSpell(ICharacter magician, IPerceivable target, SpellPower power,
		params SpellAdditionalParameter[] additionalParameters)
	{
		if (magician.CombinedEffectsOfType<MagicSpellLockout>().Any(x => x.Applies(School)))
		{
			magician.OutputHandler.Send(
				$"You are currently locked out from casting {School.SchoolAdjective.Colour(School.PowerListColour)} spells.");
			return;
		}

		var realCosts = new List<(IMagicResource Resource, double Cost)>();
		foreach (var (resource, expression) in _castingCosts)
		{
			expression.Formula.Parameters["power"] = (int)power;
			expression.Formula.Parameters["self"] = magician == target ? 1 : 0;
			var cost = expression.Evaluate(magician, CastingTrait, TraitBonusContext.SpellCost);
			if (!magician.CanUseResource(resource, cost))
			{
				magician.OutputHandler.Send(
					$"You do not have enough {resource.Name.Pluralise().ColourValue()} to cast that spell.");
				return;
			}

			realCosts.Add((resource, cost));
		}

		var plan = InventoryPlanTemplate.CreatePlan(magician);
		switch (plan.PlanIsFeasible())
		{
			case InventoryPlanFeasibility.Feasible:
				break;
			case InventoryPlanFeasibility.NotFeasibleNotEnoughHands:
				magician.OutputHandler.Send(
					"You do not have enough free hands to manipulate material components to cast that spell.");
				return;
			case InventoryPlanFeasibility.NotFeasibleNotEnoughWielders:
				magician.OutputHandler.Send(
					"You do not have enough free hands to wield material components to cast that spell.");
				return;
			case InventoryPlanFeasibility.NotFeasibleMissingItems:
				magician.OutputHandler.Send(
					"You are missing material components to cast that spell.");
				return;
			default:
				throw new ArgumentOutOfRangeException();
		}

		foreach (var (resource, cost) in realCosts)
		{
			magician.UseResource(resource, cost);
		}

		plan.ExecuteWholePlan();
		if (ExclusiveDelay > TimeSpan.Zero)
		{
			magician.AddEffect(new MagicSpellLockout(magician, Enumerable.Empty<IMagicSchool>()), ExclusiveDelay);
		}

		if (NonExclusiveDelay > TimeSpan.Zero)
		{
			magician.AddEffect(new MagicSpellLockout(magician, new List<IMagicSchool> { School }), NonExclusiveDelay);
		}

		var check = Gameworld.GetCheck(CheckType.CastSpellCheck);
		var resistCheck = Gameworld.GetCheck(CheckType.ResistMagicSpellCheck);
		var result = check.CheckAgainstAllDifficulties(magician, CastingDifficulty, CastingTrait, target);
		if (result[CastingDifficulty].Outcome < MinimumSuccessThreshold)
		{
			magician.OutputHandler.Handle(new EmoteOutput(new Emote(FailCastingEmote, magician, magician, target),
				flags: CastingEmoteFlags));
			return;
		}

		magician.OutputHandler.Handle(new EmoteOutput(new Emote(CastingEmote, magician, magician, target),
			flags: CastingEmoteFlags));
		EffectDurationExpression.Formula.Parameters["degrees"] = result[CastingDifficulty].CheckDegrees();
		EffectDurationExpression.Formula.Parameters["success"] = result[CastingDifficulty].SuccessDegrees();
		EffectDurationExpression.Formula.Parameters["power"] = (int)power;
		var duration =
			TimeSpan.FromSeconds(
				EffectDurationExpression.Evaluate(magician, CastingTrait, TraitBonusContext.SpellDuration));
		var outcome = new OpposedOutcome(result[CastingDifficulty], Outcome.NotTested);

		void ApplySpellEffect(IPerceivable effectTarget, IEnumerable<IMagicSpellEffectTemplate> effects)
		{
			var head = new MagicSpellParent(effectTarget, this, magician);
			foreach (var effect in effects)
			{
				var child = effect.GetOrApplyEffect(magician, effectTarget, outcome.Degree, power, head);
				if (child == null)
				{
					continue;
				}

				effectTarget.AddEffect(child);
				head.AddSpellEffect(child);
			}

			if (AppliedEffectsAreExclusive)
			{
				target.RemoveAllEffects<MagicSpellParent>(x => x.Spell == this);
			}

			// It's possible that all of the spell effects were instantaneous, in which case do not apply the effect
			if (head.SpellEffects.Any())
			{
				target.AddEffect(head, duration);
			}
		}

		if (target is PerceivableGroup pg)
		{
			foreach (var individual in pg.Members)
			{
				outcome = new OpposedOutcome(result[CastingDifficulty], Outcome.NotTested);
				if (OpposedTrait is not null && individual is ICharacter tch)
				{
					var resist =
						resistCheck.CheckAgainstAllDifficulties(tch, OpposedDifficulty ?? Difficulty.Normal, OpposedTrait,
							magician);
					outcome = new OpposedOutcome(result, resist, CastingDifficulty, OpposedDifficulty.Value);
					if (outcome.Outcome == OpposedOutcomeDirection.Opponent)
					{
						if (!string.IsNullOrEmpty(TargetResistedEmote))
						{
							tch.OutputHandler.Handle(new EmoteOutput(
								new Emote(TargetResistedEmote, magician, magician, tch), flags: TargetEmoteFlags));
						}

						continue;
					}
				}

				if (!string.IsNullOrEmpty(TargetEmote))
				{
					individual.OutputHandler.Handle(new EmoteOutput(
						new Emote(TargetEmote, magician, magician, individual), flags: TargetEmoteFlags));
				}

				ApplySpellEffect(individual, _spellEffects);
			}
		}
		else if (target is ICharacter tch && tch != magician)
		{
			if (OpposedTrait is not null)
			{
				var resist =
					resistCheck.CheckAgainstAllDifficulties(tch, OpposedDifficulty ?? Difficulty.Normal, OpposedTrait,
						magician);
				outcome = new OpposedOutcome(result, resist, CastingDifficulty, OpposedDifficulty.Value);
				if (outcome.Outcome == OpposedOutcomeDirection.Opponent)
				{
					if (!string.IsNullOrEmpty(TargetResistedEmote))
					{
						tch.OutputHandler.Handle(new EmoteOutput(
							new Emote(TargetResistedEmote, magician, magician, tch), flags: TargetEmoteFlags));
					}

					return;
				}
			}

			if (!string.IsNullOrEmpty(TargetEmote))
			{
				tch.OutputHandler.Handle(new EmoteOutput(new Emote(TargetEmote, magician, magician, tch),
					flags: TargetEmoteFlags));
			}

			ApplySpellEffect(target, _spellEffects);
		}
		else
		{
			if (!string.IsNullOrEmpty(TargetEmote))
			{
				target.OutputHandler.Handle(new EmoteOutput(new Emote(TargetEmote, magician, magician, target),
					flags: TargetEmoteFlags));
			}

			ApplySpellEffect(target, _spellEffects);
		}

		if (_casterSpellEffects.Any())
		{
			ApplySpellEffect(magician, _casterSpellEffects);
		}
	}

	public bool ReadyForGame =>
		Trigger != null &&
		!string.IsNullOrEmpty(CastingEmote) &&
		(Trigger.TriggerYieldsTarget || _spellEffects.All(x => !x.RequiresTarget)) &&
		(EffectDurationExpression != null || _spellEffects.All(x => x.IsInstantaneous)) &&
		CastingTrait != null;

	public string WhyNotReadyForGame
	{
		get
		{
			if (Trigger == null)
			{
				return "every spell much have a trigger set.";
			}

			if (string.IsNullOrEmpty(CastingEmote))
			{
				return "every spell must have a casting emote.";
			}

			if (!Trigger.TriggerYieldsTarget && _spellEffects.Any(x => x.RequiresTarget))
			{
				return
					"at least one of the spell effects requires a target, but the spell trigger does not supply one.";
			}

			if (EffectDurationExpression == null && _spellEffects.Any(x => !x.IsInstantaneous))
			{
				return
					"there is no effect duration expression set, and at least one of the spell effects is not instantaneous.";
			}

			if (CastingTrait == null)
			{
				return "every spell must have a casting trait.";
			}

			throw new ApplicationException("Got to the end of MagicSpell.WhyNotReadyForGame without finding an error.");
		}
	}

	public string ShowPlayerHelp(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"{Name.Colour(School.PowerListColour)}");
		sb.AppendLine($"Blurb: {Blurb.ColourCommand()}");
		sb.AppendLine($"Exclusive Effects: {AppliedEffectsAreExclusive.ToColouredString()}");
		sb.AppendLine();
		sb.AppendLine("Description:");
		sb.AppendLine();
		sb.AppendLine(Description.Wrap(actor.InnerLineFormatLength, "\t"));
		if (_castingCosts.Any() && Trigger is ICastMagicTrigger ct)
		{
			sb.AppendLine();
			sb.AppendLine("Casting Costs:");
			foreach (var power in Enum.GetValues<SpellPower>()
			                          .Where(x => x >= ct.MinimumPower && x <= ct.MaximumPower))
			{
				sb.AppendLine(
					$"\t{power.DescribeEnum().ColourName()}: {_castingCosts.Select(x => $"{x.Value.EvaluateWith(actor, CastingTrait, TraitBonusContext.SpellCost, ("self", 0), ("power", (int)power))} {x.Key.ShortName}".ColourValue()).ListToString()}");
			}
		}

		if (InventoryPlanTemplate.Phases.First().Actions.Any())
		{
			sb.AppendLine();
			sb.AppendLine("Material Requirements:");
			foreach (var action in InventoryPlanTemplate.Phases.First().Actions)
			{
				sb.AppendLine($"\t{action.Describe(actor)}");
			}
		}

		sb.AppendLine();
		sb.AppendLine($"Trigger: {Trigger.ShowPlayer(actor).ColourCommand()}");
		return sb.ToString();
	}

	#endregion

	#region Implementation of IFutureProgVariable

	public FutureProgVariableTypes Type => FutureProgVariableTypes.MagicSpell;
	public object GetObject => this;

	public IFutureProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "name":
				return new TextVariable(Name);
			case "id":
				return new NumberVariable(Id);
			case "description":
				return new TextVariable(Description);
			case "school":
				return School;
			case "exclusivedelay":
				return new TimeSpanVariable(ExclusiveDelay);
			case "nonexclusivedelay":
				return new TimeSpanVariable(NonExclusiveDelay);
			case "castingtrait":
				return CastingTrait;
			case "opposedtrait":
				return OpposedTrait;
		}

		throw new ApplicationException("Invalid property requested in MagicSpell.GetProperty");
	}

	private static IReadOnlyDictionary<string, FutureProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, FutureProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "name", FutureProgVariableTypes.Text },
			{ "id", FutureProgVariableTypes.Number },
			{ "description", FutureProgVariableTypes.Text },
			{ "school", FutureProgVariableTypes.MagicSchool },
			{ "exclusivedelay", FutureProgVariableTypes.TimeSpan },
			{ "nonexclusivedelay", FutureProgVariableTypes.TimeSpan },
			{ "castingtrait", FutureProgVariableTypes.Trait },
			{ "opposedtrait", FutureProgVariableTypes.Trait }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "name", "The name of the spell" },
			{ "id", "The Id of the spell" },
			{ "description", "The description of the spell as seen in spellinfo" },
			{ "school", "The MagicSchool to which this spell belongs" },
			{ "exclusivedelay", "The lock-out timer to all other spells when cast" },
			{ "nonexclusivedelay", "The lock-out timer to spells in the same school when cast" },
			{ "castingtrait", "The trait that this spell uses to cast in checks" },
			{ "opposedtrait", "If set, the trait rolled by those opposing this spell. Can be null." }
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.MagicSpell, DotReferenceHandler(),
			DotReferenceHelp());
	}

	#endregion
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.RPG.Merits.Interfaces;
using MudSharp.Character;
using System.Text;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class MultiTraitBonusMerit : CharacterMeritBase, ITraitBonusMerit
{
	private readonly HashSet<TraitBonusContext> _contexts = new();

	/// <inheritdoc />
	protected override XElement SaveSubtypeDefinition(XElement root)
	{
		root.Add(new XElement("Traits",
			from item in SpecificTraits
			select new XElement("Trait", 
				new XAttribute("id", item.Id)
			)
		));
		root.Add(new XAttribute("bonus", SpecificBonus));
		root.Add(new XElement("Contexts",
			from item in _contexts
			select new XElement("Context", (int)item)
		));
		return root;
	}

	protected MultiTraitBonusMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		var traits = (from item in definition.Element("Traits")?.Elements() ?? Enumerable.Empty<XElement>()
		              select gameworld.Traits.Get(long.Parse(item.Attribute("id")?.Value ?? "0"))).ToList();
		foreach (var trait in traits)
		{
			SpecificTraits.Add(trait);
		}

		SpecificBonus = double.Parse(definition.Attribute("bonus")?.Value ?? "0.0");
		var element = definition.Element("Contexts");
		if (element is not null)
		{
			foreach (var item in element.Elements())
			{
				if (int.TryParse(item.Value, out var value))
				{
					if (Enum.IsDefined(typeof(TraitBonusContext), value))
					{
						_contexts.Add((TraitBonusContext)value);
					}
					else
					{
						Console.WriteLine(
							$"Warning: SpecificTraitBonusMerit {Id} had a context of {value}, which is not a valid value.");
					}
				}
				else
				{
					if (Enum.TryParse<TraitBonusContext>(item.Value, out var evalue))
					{
						_contexts.Add(evalue);
					}
					else
					{
						Console.WriteLine(
							$"Warning: SpecificTraitBonusMerit {Id} had a context of {item.Value}, which is not a valid value.");
					}
				}
			}
		}
	}

	protected MultiTraitBonusMerit()
	{

	}

	protected MultiTraitBonusMerit(IFuturemud gameworld, string name) : base(gameworld, name, "Multi Trait Bonus", "@ have|has bonuses to multiple traits")
	{
		DoDatabaseInsert();
	}

	public HashSet<ITraitDefinition> SpecificTraits { get; } = new();
	public double SpecificBonus { get; set; }

	#region Implementation of ISkillBonusMerit

	public double BonusForTrait(ITraitDefinition trait, TraitBonusContext context)
	{
		return SpecificTraits.Contains(trait) && 
		       (_contexts.Contains(context) || _contexts.Count == 0) ? SpecificBonus : 0.0;
	}

	#endregion

	protected override void SubtypeShow(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Traits: {SpecificTraits.Select(x => x.Name.ColourValue()).ListToString()}");
		sb.AppendLine($"Bonus for Trait: {SpecificBonus.ToBonusString(actor)}");
		sb.AppendLine($"Contexts: {(_contexts.Count == 0 ? "All".ColourValue() : _contexts.Select(x => x.DescribeEnum().ColourValue()).ListToString())}");
	}

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "bonus":
				return BuildingCommandBonus(actor, command);
			case "trait":
			case "skill":
			case "attribute":
			case "attr":
				return BuildingCommandTrait(actor, command);
			case "context":
				return BuildingCommandContext(actor, command);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandContext(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"You must either specify {"all".ColourCommand()} or a list of values from the following:\n{Enum.GetValues<TraitBonusContext>().Select(x => x.DescribeEnum().ColourName()).ListToString()}");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("all"))
		{
			_contexts.Clear();
			Changed = true;
			actor.OutputHandler.Send($"This merit will now give a bonus to its assigned trait in all contexts.");
			return true;
		}

		var contexts = new List<TraitBonusContext>();
		while (!command.IsFinished)
		{
			var cmd = command.PopSpeech();
			if (!cmd.TryParseEnum(out TraitBonusContext context))
			{
				actor.OutputHandler.Send($"The text {cmd.ColourCommand()} is not a valid trait bonus context. You can select values from the following:\n{Enum.GetValues<TraitBonusContext>().Select(x => x.DescribeEnum().ColourName()).ListToString()}");
				return false;
			}

			contexts.Add(context);
		}

		_contexts.Clear();
		foreach (var context in contexts.Distinct())
		{
			_contexts.Add(context);
		}

		Changed = true;
		actor.OutputHandler.Send($"This merit will now only apply its bonus in the following contexts: {_contexts.Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
		return true;
	}

	private bool BuildingCommandTrait(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which trait do you want to toggle?");
			return false;
		}

		var trait = Gameworld.Traits.GetByIdOrName(command.SafeRemainingArgument);
		if (trait is null)
		{
			actor.OutputHandler.Send($"There is no such trait identified by the text {command.SafeRemainingArgument.ColourCommand()}.");
			return false;
		}

		Changed = true;
		if (SpecificTraits.Remove(trait))
		{
			actor.OutputHandler.Send($"This merit will no longer give a bonus or penalty to the {trait.Name.ColourValue()} trait.");
			return true;
		}

		SpecificTraits.Add(trait);
		actor.OutputHandler.Send($"This merit will now give a bonus or penalty to the {trait.Name.ColourValue()} trait.");
		return true;
	}

	private bool BuildingCommandBonus(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should the bonus be when this merit applies?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid number.");
			return false;
		}

		SpecificBonus = value;
		Changed = true;
		actor.OutputHandler.Send($"This merit will now add a bonus of {SpecificBonus.ToBonusString(actor)} to the affected trait when it applies.");
		return true;
	}

	/// <inheritdoc />
	protected override string SubtypeHelp => $@"{base.SubtypeHelp}
	#3bonus <##>#0 - sets the bonus for this merit
	#3trait <which>#0 - toggles a trait that gets the bonus
	#3context all#0 - makes the bonus apply in all contexts
	#3context <context1> [<context2>] ... [<contextn>]#0 - sets the contexts in which the traits will get a bonus";

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Multi Trait Bonus",
			(merit, gameworld) => new MultiTraitBonusMerit(merit, gameworld));
		MeritFactory.RegisterBuilderMeritInitialiser("Multi Trait Bonus", (gameworld, name) => new MultiTraitBonusMerit(gameworld, name));
		MeritFactory.RegisterMeritHelp("Multi Trait Bonus", "Adds a bonus or penalty to a multiple traits", new MultiTraitBonusMerit().HelpText);
	}
}
using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Models;
using MudSharp.Combat;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class NaturalAttackQualityMerit : CharacterMeritBase, INaturalAttackQualityMerit
{
	protected NaturalAttackQualityMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		Verb = (MeleeWeaponVerb)int.Parse(definition.Attribute("verb")?.Value ?? "0");
		Boosts = int.Parse(definition.Attribute("boosts")?.Value ?? "0");
	}

	protected NaturalAttackQualityMerit(){}

	protected NaturalAttackQualityMerit(IFuturemud gameworld, string name) : base(gameworld, name, "Natural Attack Quality", "@ have|has a natural attack quality modifier")
	{
		Verb = MeleeWeaponVerb.Punch;
		DoDatabaseInsert();
	}

	/// <inheritdoc />
	protected override XElement SaveSubtypeDefinition(XElement root)
	{
		root.Add(new XAttribute("verb", (int)Verb));
		root.Add(new XAttribute("boosts", Boosts));
		return root;
	}

	public MeleeWeaponVerb Verb { get; set; }
	public int Boosts { get; set; }

	#region Implementation of INaturalAttackQualityMerit

	public ItemQuality GetQuality(ItemQuality baseQuality)
	{
		return baseQuality.Stage(Boosts);
	}

	#endregion

	/// <inheritdoc />
	protected override void SubtypeShow(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Natural Attack Verb: {Verb.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Boost Steps: {Boosts.ToBonusString(actor)}");
	}

	/// <inheritdoc />
	protected override string SubtypeHelp => $@"{base.SubtypeHelp}
	#3verb <verb>#0 - which verb this merit applies to
	#3boosts <##>#0 - the number of steps to boost it";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "boost":
			case "boosts":
				return BuildingCommandBoosts(actor, command);
			case "verb":
				return BuildingCommandVerb(actor, command);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandBoosts(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How many levels of quality should the natural attack improve or worsen?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid number.");
			return false;
		}

		Boosts = value;
		Changed = true;
		actor.OutputHandler.Send($"This merit will now give a {value.ToBonusString(actor)} level bonus to affected natural attack quality.");
		return true;
	}

	private bool BuildingCommandVerb(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which natural attack verb should this boost apply to? The valid options are {Enum.GetValues<MeleeWeaponVerb>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum(out MeleeWeaponVerb verb))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid verb. The valid options are {Enum.GetValues<MeleeWeaponVerb>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		Verb = verb;
		Changed = true;
		actor.OutputHandler.Send($"This merit now applies to natural attacks with the {verb.DescribeEnum().ColourValue()} verb.");
		return true;
	}


	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Natural Attack Quality",
			(merit, gameworld) => new NaturalAttackQualityMerit(merit, gameworld));
		MeritFactory.RegisterBuilderMeritInitialiser("Natural Attack Quality", (gameworld, name) => new NaturalAttackQualityMerit(gameworld, name));
		MeritFactory.RegisterMeritHelp("Natural Attack Quality", "Gives a person a better quality natural attack", new NaturalAttackQualityMerit().HelpText);
	}
}
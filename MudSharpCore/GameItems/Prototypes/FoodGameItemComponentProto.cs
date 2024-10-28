using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Units;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Decorators;
using MudSharp.PerceptionEngine;
using MudSharp.FutureProg;
using System;

namespace MudSharp.GameItems.Prototypes;

public class FoodGameItemComponentProto : GameItemComponentProto
{
	public IStackDecorator Decorator { get; protected set; }
	public double Calories { get; protected set; }
	public double SatiationPoints { get; protected set; }
	public double WaterLitres { get; protected set; }
	public double ThirstPoints { get; protected set; }
	public double AlcoholLitres { get; protected set; }
	public string TasteString { get; protected set; }
	public double Bites { get; protected set; }
	public IFutureProg OnEatProg { get; protected set; }

	public override string TypeDescription => "Food";

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new FoodGameItemComponentProto(proto, gameworld));
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("food", true,
			(gameworld, account) => new FoodGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Food", (proto, gameworld) => new FoodGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"Food",
			$"Turns an item into an edible food object",
			BuildingHelpText
		);
	}

	public override bool CanSubmit()
	{
		return Decorator != null;
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{10:N0}r{11:N0}, {12})\n\nThis item is food. When eaten it provides {1} calories and satisfies hunger for {2} hours, and provides {3} of water whilst satisfying {4} hours of regular thirst. It contains {5} of alcohol. It can be eaten in {6} bite{7}.\n\nWhen bites are taken out of it, it uses the {8} decorator and has the taste string {9}\n\nProg fired on eat: {13}",
			"Food Item Component".Colour(Telnet.Cyan),
			Calories.ToString("N0", actor).Colour(Telnet.Green),
			SatiationPoints.ToString("N1", actor).Colour(Telnet.Green),
			actor.Gameworld.UnitManager.Describe(WaterLitres / actor.Gameworld.UnitManager.BaseFluidToLitres,
				UnitType.FluidVolume, actor).Colour(Telnet.Green),
			ThirstPoints.ToString("N1", actor).Colour(Telnet.Green),
			actor.Gameworld.UnitManager.Describe(AlcoholLitres / actor.Gameworld.UnitManager.BaseFluidToLitres,
				UnitType.FluidVolume, actor).Colour(Telnet.Green),
			Bites.ToString("N2", actor).Colour(Telnet.Green),
			Bites == 1 ? "" : "s",
			Decorator == null ? "none".Colour(Telnet.Red) : Decorator.Name.TitleCase().Colour(Telnet.Green),
			TasteString.Fullstop().Colour(Telnet.Green),
			Id,
			RevisionNumber,
			Name,
			OnEatProg == null ? "None" : $"Prog #{OnEatProg.Id} ({OnEatProg.Name})"
		);
	}

	protected override void LoadFromXml(XElement root)
	{
		var attribute = root.Attribute("Calories");
		if (attribute != null)
		{
			Calories = double.Parse(attribute.Value);
		}

		attribute = root.Attribute("Satiation");
		if (attribute != null)
		{
			SatiationPoints = double.Parse(attribute.Value);
		}

		attribute = root.Attribute("Water");
		if (attribute != null)
		{
			WaterLitres = double.Parse(attribute.Value);
		}

		attribute = root.Attribute("Thirst");
		if (attribute != null)
		{
			ThirstPoints = double.Parse(attribute.Value);
		}

		attribute = root.Attribute("Alcohol");
		if (attribute != null)
		{
			AlcoholLitres = double.Parse(attribute.Value);
		}

		attribute = root.Attribute("Bites");
		if (attribute != null)
		{
			Bites = double.Parse(attribute.Value);
		}

		var element = root.Element("Taste");
		if (element != null)
		{
			TasteString = element.Value;
		}

		element = root.Element("OnEatProg");
		if (element != null)
		{
			OnEatProg = Gameworld.FutureProgs.Get(long.Parse(element.Value));
		}

		attribute = root.Attribute("Decorator");
		if (attribute != null)
		{
			Decorator = Gameworld.StackDecorators.Get(long.Parse(attribute.Value));
		}
	}

	protected override string SaveToXml()
	{
		return
			new XElement("Definition", new XAttribute("Calories", Calories),
				new XAttribute("Satiation", SatiationPoints), new XAttribute("Water", WaterLitres),
				new XAttribute("Thirst", ThirstPoints), new XAttribute("Alcohol", AlcoholLitres),
				new XAttribute("Bites", Bites), new XAttribute("Decorator", Decorator?.Id ?? 0),
				new XElement("Taste", new XCData(TasteString)),
				new XElement("OnEatProg", OnEatProg?.Id ?? 0)
			).ToString();
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new FoodGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new FoodGameItemComponent(component, this, parent);
	}

	#region Constructors

	protected FoodGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected FoodGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Food")
	{
		var stringValue = gameworld.GetStaticConfiguration("DefaultFoodStackDecorator");
		if (stringValue != null)
		{
			Decorator = Gameworld.StackDecorators.Get(long.Parse(stringValue));
		}

		Calories = 500;
		SatiationPoints = 6;
		WaterLitres = 0.05;
		ThirstPoints = 0;
		AlcoholLitres = 0;
		TasteString = "It has an unremarkable taste";
		Bites = 1;
		Changed = true;
	}

	#endregion Constructors

	#region Building Commands

	private const string BuildingHelpText = @"You can use the following options with this component:
	name <name> - sets the name of the component
	desc <desc> - sets the description of the component
	hunger <hours> - the hours of food satiation from eating the whole thing
	thirst <hours> - the hours of thirst satiation from eating the whole thing
	alcohol <amount> - the liquid volume of alcohol from eating the whole thing
	bites <#> - the number of bites before the food is consumed
	calories <cals> - the number of Calories (kCal) from eating the whole thing
	water <amount> - the liquid volume of water from eating the whole thing
	taste <string> - a taste string to display to the character eating it
	prog <which> - sets a prog to be executed when the food is eaten
	prog clear - clears the on-eat prog

Note: Calories/Water is mostly obsolete and may be removed in future, so don't stress those values too much";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "calories":
			case "cals":
				return BuildingCommandCalories(actor, command);
			case "satiation":
			case "hunger":
			case "hours":
				return BuildingCommandSatiation(actor, command);
			case "thirst":
				return BuildingCommandThirst(actor, command);
			case "water":
				return BuildingCommandWater(actor, command);
			case "alcohol":
				return BuildingCommandAlcohol(actor, command);
			case "taste":
				return BuildingCommandTaste(actor, command);
			case "bites":
				return BuildingCommandBites(actor, command);
			case "prog":
				return BuildingCommandProg(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("You can either CLEAR to remove the existing prog, or specify a prog name or ID to add.");
			return false;
		}

		if (command.Peek().EqualTo("clear"))
		{
			OnEatProg = null;
			Changed = true;
			actor.Send("This edible will no longer execute any prog when eaten.");
			return true;
		}

		var prog = Gameworld.FutureProgs.GetByIdOrName(command.SafeRemainingArgument);
		if (prog == null)
		{
			actor.Send("There is no such prog.");
			return false;
		}

		if (!prog.MatchesParameters(new[]
			    { ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Number }))
		{
			actor.Send("The prog should accept a character, item and number.");
			return false;
		}

		OnEatProg = prog;
		Changed = true;
		actor.Send($"This edible will now execute the {OnEatProg.Name.Colour(Telnet.Green)} prog when eaten.");
		return true;
	}

	private bool BuildingCommandBites(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How many bites should this food take to be completely consumed?");
			return false;
		}

		if (!double.TryParse(command.Pop(), out var value) || value < 1)
		{
			actor.Send("You must enter a valid, positive number of bites.");
			return false;
		}

		Bites = value;
		Changed = true;
		actor.Send("This food now requires {0} bite{1} to be completely consumed.",
			Bites.ToString("N2", actor).Colour(Telnet.Green),
			Bites == 1 ? "" : "s"
		);
		return true;
	}

	private bool BuildingCommandCalories(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How many calories do you want this food to give when eaten?");
			return false;
		}

		if (!double.TryParse(command.Pop(), out var value))
		{
			actor.Send("You must enter a valid number of calories.");
			return false;
		}

		Calories = value;
		Changed = true;
		actor.Send("This food now gives a total of {0} calories{1}.",
			Calories.ToString("N0", actor).Colour(Telnet.Green),
			Bites == 1
				? ""
				: $", or {(Calories / Bites).ToString("N0", actor).Colour(Telnet.Green)} calories per bite"
		);
		return true;
	}

	private bool BuildingCommandSatiation(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How many hours of satiation do you want this food to give when eaten?");
			return false;
		}

		if (!double.TryParse(command.Pop(), out var value))
		{
			actor.Send("You must enter a valid number of hours.");
			return false;
		}

		SatiationPoints = value;
		Changed = true;
		actor.Send("This food now gives a total of {0} hours of satiation{1}.",
			SatiationPoints.ToString("N1", actor).Colour(Telnet.Green),
			Bites == 1
				? ""
				: $", or {(SatiationPoints / Bites).ToString("N1", actor).Colour(Telnet.Green)} hours per bite"
		);
		return true;
	}

	private bool BuildingCommandWater(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What fluid volume of water should this food give when eaten?");
			return false;
		}

		var value = actor.Gameworld.UnitManager.GetBaseUnits(command.SafeRemainingArgument, UnitType.FluidVolume,
			out var success);
		if (!success)
		{
			actor.Send("That is not a valid fluid volume.");
			return false;
		}

		WaterLitres = value * actor.Gameworld.UnitManager.BaseFluidToLitres;
		Changed = true;
		actor.Send("This food now gives {0} of water when eaten{1}.",
			actor.Gameworld.UnitManager.Describe(value, UnitType.FluidVolume, actor).Colour(Telnet.Green),
			Bites == 1
				? ""
				: $", or {actor.Gameworld.UnitManager.Describe(value / Bites, UnitType.FluidVolume, actor).Colour(Telnet.Green)} per bite"
		);

		return true;
	}

	private bool BuildingCommandThirst(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How many hours of thirst satiation do you want this food to give when eaten?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.Send("You must enter a valid number of hours.");
			return false;
		}

		ThirstPoints = value;
		Changed = true;
		actor.Send("This food now gives a total of {0} hours of thirst satiation{1}.",
			ThirstPoints.ToString("N1", actor).Colour(Telnet.Green),
			Bites == 1
				? ""
				: $", or {(ThirstPoints / Bites).ToString("N1", actor).Colour(Telnet.Green)} hours per bite"
		);
		return true;
	}

	private bool BuildingCommandAlcohol(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What fluid volume of alcohol should this food give when eaten?");
			return false;
		}

		var value = actor.Gameworld.UnitManager.GetBaseUnits(command.SafeRemainingArgument, UnitType.FluidVolume,
			out var success);
		if (!success)
		{
			actor.Send("That is not a valid fluid volume.");
			return false;
		}

		AlcoholLitres = value * actor.Gameworld.UnitManager.BaseFluidToLitres;
		Changed = true;
		actor.Send("This food now gives {0} of alcohol when eaten{1}.",
			actor.Gameworld.UnitManager.Describe(value, UnitType.FluidVolume, actor).Colour(Telnet.Green),
			Bites == 1
				? ""
				: $", or {actor.Gameworld.UnitManager.Describe(value / Bites, UnitType.FluidVolume, actor).Colour(Telnet.Green)} per bite"
		);

		return true;
	}

	private bool BuildingCommandTaste(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What taste string would you like to set for this food?");
			return false;
		}

		TasteString = command.SafeRemainingArgument;
		Changed = true;
		actor.Send("This food will now be described as follows when eaten:\n\n{0}",
			TasteString.Wrap(80, "\t").ColourCommand());
		return true;
	}

	#endregion
}
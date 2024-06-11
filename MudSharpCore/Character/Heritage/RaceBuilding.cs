using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Combat;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;
using Org.BouncyCastle.Asn1.TeleTrust;
using Org.BouncyCastle.Asn1.X509;

namespace MudSharp.Character.Heritage;

public partial class Race
{
	#region Implementation of IEditableItem

	public const string HelpText = @"You can use the following options with this command:

	#6Core Properties#0

	#3name <name>#0 - renames the race
	#3desc#0 - drops you into an editor to describe the race
	#3parent <race>#0 - sets a parent race for this race
	#3parent none#0 - clears a parent race from this race
	#3body <template>#0 - changes the body template of the race
	#3parthealth <%>#0 - sets a multiplier for bodypart HPs
	#3partsize <##>#0 - sets a number of steps bigger/smaller for bodyparts

	#6Chargen Properties#0

	#3chargen <prog>#0 - sets a prog that controls chargen availability
	#3advice <which>#0 - toggles a chargen advice applying to this race
	#3cost <resource> <amount>#0 - sets a cost for character creation
	#3require <resource> <amount>#0 - sets a non-cost requirement for character creation
	#3cost <resource> clear#0 - clears a resource cost for character creation	

	#6Combat Properties#0

	#3armour <type>#0 - sets the natural armour type for this race
	#3armour none#0 - clears the natural armour type
	#3armourquality <quality>#0 - sets the quality of the natural armour
	#3armourmaterial <material>#0 - sets the default material for the race's natural armour
	#3canattack#0 - toggles the race being able to use attacks
	#3candefend#0 - toggles the race being able to dodge/parry/block
	#3canuseweapons#0 - toggles the race being able to use weapons (if it has wielding parts)

	#6Physical Properties#0

	#3age <category> <minimum>#0 - sets the minimum age for a specified age category
	#3variable all <characteristic>#0 - adds or sets a specified characteristic for all genders
	#3variable male <characteristic>#0 - adds or sets a specified characteristic for males only
	#3variable female <characteristic>#0 - adds or sets a specified characteristic for females only
	#3variable remove <characteristic>#0 - removes a characteristic from this race
	#3variable promote <characteristic>#0 - pushes a characteristic up to the parent race
	#3variable demote <characteristic>#0 - pushes a characteristic down to all child races (and remove from this)
	#3attribute <which>#0 - toggles this race having the specified attribute
	#3attribute promote <which>#0 - pushes this attribute up to the parent race
	#3attribute demote <which>#0 - pushes this attribute down to all child races (and remove from this)
	#3roll <dice>#0 - the dice roll expression (#6xdy+z#0) for attributes for this race
	#3cap <number>#0 - the total cap on the sum of attributes for this race
	#3bonusprog <which>#0 - sets the prog that controls attribute bonuses
	#3corpse <model>#0 - changes the corpse model of the race
	#3health <model>#0 - changes the health mode of the race
	#3perception <%>#0 - sets the light-percetion multiplier of the race (higher is better)
	#3genders <list of genders>#0 - sets the allowable genders for this race
	#3butcher <profile>#0 - sets a butchery profile for this race
	#3butcher none#0 - clears a butchery profile from this race
	#3breathing nonbreather|simple|lung|gill|blowhole#0 - sets the breathing model
	#3breathingrate <volume per minute>#0 - sets the volume of breathing per minute
	#3holdbreath <seconds expression>#0 - sets the formula for breathe-holding length
	#3sweat <liquid>#0 - sets the race's sweat liquid
	#3sweat none#0 - disables sweating for this race
	#3sweatrate <volume per minute>#0 - sets the volume of sweating per minute
	#3blood <liquid>#0 - sets the race's blood liquid
	#3blood none#0 - disables bleeding for this race
	#3bloodmodel <model>#0 - sets the blood antigen typing model for this race
	#3bloodmodel none#0 - clears a blood antigen typing model from this race
	#3tempfloor <temperature>#0 - sets the base minimum tolerable temperature for this race
	#3tempceiling <temperature>#0 - sets the base maximum tolerable temperature for this race

	#6Eating Properties#0

	#3hunger <%>#0 - sets a percentage multiplier to base rate at which they will get hungry
	#3thirst <%>#0 - sets a percentage multiplier to base rate at which they will get thirsty
	#3caneatcorpses#0 - toggles the race being able to eat corpses directly (without butchering)
	#3biteweight <weight>#0 - sets the amount of corpse weight eaten per bite
	#3material add <material>#0 - adds a material definition for corpse-eating
	#3material remove <material>#0 - removes a material as eligible for corpse-eating
	#3material alcohol|thirst|hunger|water|calories <amount>#0 - sets the per-kg nutrition for this material
	#3optinediblematerial#0 - toggles whether the race can only eat materials from the pre-defined list
	#3emotecorpse <emote>#0 - sets the emote for eating corpses. $0 is eater, $1 is corpse.
	#3yield add <type> <hunger> <thirst> <emote>#0 - adds an edible foragable yield profile
	#3yield remove <type>#0 - removes an edible foragable yield profile
	#3yield <type> calories <amount>#0 - edits the calories of a profile
	#3yield <type> water <amount>#0 - edits the water of a profile
	#3yield <type> thirst <amount>#0 - edits the thirst hours of a profile
	#3yield <type> hunger <amount>#0 - edits the hunger hours of a profile
	#3yield <type> bites <amount>#0 - edits the yield per bite of a profile
	#3yield <type> emote <emote>#0 - edits the emote of a profile";

	/// <inheritdoc />
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "parent":
				return BuildingCommandParent(actor, command);
			case "chargen":
			case "chargenprog":
			case "available":
			case "availability":
			case "chargenavailability":
			case "chargenavailabilityprog":
				return BuildingCommandChargenAvailabilityProg(actor, command);
			case "body":
				return BuildingCommandBody(actor, command);
			case "parthealth":
			case "bodyparthealth":
			case "healthmultiplier":
			case "healthmod":
			case "damagemod":
			case "damagemodifier":
			case "partdamage":
			case "partdam":
			case "bodypartdamage":
			case "bodypartdam":
				return BuildingCommandBodypartHealth(actor, command);
			case "bodypartsize":
				case "partsize":
				return BuildingCommandBodypartSize(actor, command);
			case "hungry":
			case "hunger":
				return BuildingCommandHunger(actor, command);
			case "thirst":
			case "thirsty":
				return BuildingCommandThirst(actor, command);
			case "corpse":
			case "corpsemodel":
				return BuildingCommandCorpseModel(actor, command);
			case "health":
			case "healthmodel":
				return BuildingCommandHealthModel(actor, command);
			case "illumination":
			case "illuminationmodifier":
			case "perceptionmodifier":
			case "perception":
				return BuildingCommandIlluminationModifier(actor, command);
			case "genders":
			case "gender":
				return BuildingCommandGenders(actor, command);
			case "butcher":
			case "butchery":
			case "butcherprofile":
			case "butcheryprofile":
				return BuildingCommandButcheryProfile(actor, command);
			case "breathing":
			case "breathingmodel":
				return BuildingCommandBreathingModel(actor, command);
			case "breathingrate":
				return BuildingCommandBreathingRate(actor, command);
			case "holdbreath":
				return BuildingCommandHoldBreath(actor, command);
			case "sweat":
			case "sweatliquid":
				return BuildingCommandSweatLiquid(actor, command);
			case "sweatrate":
				return BuildingCommandSweatRate(actor, command);
			case "blood":
				return BuildingCommandBlood(actor, command);
			case "bloodmodel":
				return BuildingCommandBloodModel(actor, command);
			case "temperaturefloor":
			case "tempfloor":
				return BuildingCommandTemperatureFloor(actor, command);
			case "temperatureceiling":
			case "tempceiling":
				return BuildingCommandTemperatureCeiling(actor, command);
			case "armour":
			case "armor":
			case "armourtype":
			case "armortype":
				return BuildingCommandArmourType(actor, command);
			case "armourquality":
			case "armorquality":
				return BuildingCommandArmourQuality(actor, command);
			case "armourmaterial":
			case "armormaterial":
				return BuildingCommandArmourMaterial(actor, command);
			case "canattack":
				return BuildingCommandCanAttack(actor);
			case "candefend":
				return BuildingCommandCanDefend(actor);
			case "canuseweapons":
				return BuildingCommandCanUseWeapons(actor);
			case "age":
				return BuildingCommandAge(actor, command);
			case "characteristic":
			case "variable":
			case "var":
				return BuildingCommandCharacteristic(actor, command);
			case "attribute":
			case "attr":
			case "att":
				return BuildingCommandAttribute(actor, command);
			case "attributeroll":
			case "roll":
				return BuildingCommandAttributeRoll(actor, command);
			case "attributecap":
			case "cap":
				return BuildingCommandAttributeCap(actor, command);
			case "attributebonusprog":
			case "bonusprog":
			case "attributeprog":
			case "attrbonusprog":
				return BuildingCommandAttributeBonusProg(actor, command);
			case "caneatcorpses":
			case "eatcorpses":
			case "caneatcorpse":
			case "eatcorpse":
				return BuildingCommandCanEatCorpses(actor);
			case "biteweight":
				return BuildingCommandBiteWeight(actor, command);
			case "eatmaterial":
			case "eatmaterials":
			case "ediblematerials":
			case "ediblematerial":
			case "material":
				return BuildingCommandEdibleMaterial(actor, command);
			case "optinediblematerial":
			case "optin":
				return BuildingCommandOptInEdibleMaterial(actor);
			case "eatcorpseemote":
			case "corpseemote":
			case "emotecorpse":
				return BuildingCommandEatCorpseEmote(actor, command);
			case "edibleyield":
			case "yield":
				return BuildingCommandEdibleYield(actor, command);
			case "advice":
				return BuildingCommandAdvice(actor, command);
			case "cost":
			case "costs":
				return BuildingCommandCost(actor, command, true);
			case "require":
			case "requirement":
				return BuildingCommandCost(actor, command, false);
			case "description":
			case "desc":
				return BuildingCommandDescription(actor, command);
			default:
				actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandThirst(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value))
		{
			actor.OutputHandler.Send("You must enter a valid percentage.");
			return false;
		}

		ThirstRate = value;
		Changed = true;
		actor.OutputHandler.Send($"This race will now get thirsty at a {value.ToString("P2").ColourValue()} rate compared to baseline.");
		return true;
	}

	private bool BuildingCommandHunger(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value))
		{
			actor.OutputHandler.Send("You must enter a valid percentage.");
			return false;
		}

		HungerRate = value;
		Changed = true;
		actor.OutputHandler.Send($"This race will now get hungry at a {value.ToString("P2").ColourValue()} rate compared to baseline.");
		return true;
	}

	private bool BuildingCommandBodypartSize(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How many sizes bigger (+ve) or smaller (-ve) should this race's bodyparts be?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send("You must enter a valid integer number.");
			return false;
		}

		_bodypartSizeModifier = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This race's bodyparts will now be {Math.Abs(value).ToString("N0", actor).ColourValue()} {"step".Pluralise(Math.Abs(value) != 1)} {(value < 0 ? "smaller" : "larger")} than the base body.");
		return true;
	}

	private bool BuildingCommandBodypartHealth(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What percentage modifier to bodypart hitpoints should this race have?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value) || value <= 0.0)
		{
			actor.OutputHandler.Send("That is not a valid percentage.");
			return false;
		}

		_bodypartDamageMultiplier = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This race will now have {value.ToString("P2", actor).ColourValue()} hitpoints for its bodyparts.");
		return true;
	}

	private bool BuildingCommandDescription(ICharacter actor, StringStack command)
	{
		var sb = new StringBuilder();
		if (!string.IsNullOrEmpty(Description))
		{
			sb.AppendLine("Replacing:\n");
			sb.AppendLine(Description.ProperSentences().Wrap(actor.InnerLineFormatLength, "\t"));
			sb.AppendLine();
		}

		sb.AppendLine("Enter the description in the editor below.");
		sb.AppendLine();
		actor.OutputHandler.Send(sb.ToString());
		actor.EditorMode(BuildingCommandDescPost, BuildingCommandDescCancel, 1.0);
		return true;
	}

	private void BuildingCommandDescCancel(IOutputHandler handler, object[] args)
	{
		handler.Send("You decide not to change the description.");
	}

	private void BuildingCommandDescPost(string text, IOutputHandler handler, object[] arg3)
	{
		Description = text.Trim().ProperSentences();
		Changed = true;
		handler.Send($"You set the description of this race to:\n\n{Description.Wrap(80, "\t")}");
	}

	private bool BuildingCommandCost(ICharacter actor, StringStack command, bool isCost)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter a chargen resource.");
			return false;
		}

		var which = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.ChargenResources.Get(value)
			: Gameworld.ChargenResources.GetByName(command.Last) ??
			  Gameworld.ChargenResources.FirstOrDefault(x => x.Alias.EqualTo(command.Last));
		if (which == null)
		{
			actor.OutputHandler.Send("There is no such chargen resource.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must either specify a cost, or use the keyword {"clear".ColourCommand()} to clear a cost.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("clear"))
		{
			var amount = _costs.RemoveAll(x => x.Resource == which);
			if (amount == 0)
			{
				actor.OutputHandler.Send("This race has no such cost to clear.");
				return false;
			}

			Changed = true;
			actor.OutputHandler.Send(
				$"The {Name.ColourName()} race will no longer cost or require any {which.PluralName.ColourValue()}.");
			return true;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var cost))
		{
			actor.OutputHandler.Send("You must enter a valid cost.");
			return false;
		}

		_costs.RemoveAll(x => x.Resource == which);
		_costs.Add(new ChargenResourceCost
		{
			Resource = which,
			Amount = cost,
			RequirementOnly = !isCost
		});
		Changed = true;
		actor.OutputHandler.Send(
			$"This race will now {(isCost ? "cost" : "require, but not cost,")} {cost.ToString("N0", actor).ColourValue()} {(cost == 1 ? which.Name.ColourValue() : which.PluralName.ColourValue())}.");
		return true;
	}

	private bool BuildingCommandAdvice(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which chargen advice would you like to toggle applying to this race?");
			return false;
		}

		var which = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.ChargenAdvices.Get(value)
			: Gameworld.ChargenAdvices.GetByName(command.SafeRemainingArgument);
		if (which == null)
		{
			actor.OutputHandler.Send("There is no such chargen advice.");
			return false;
		}

		if (_chargenAdvices.Contains(which))
		{
			_chargenAdvices.Remove(which);
			actor.OutputHandler.Send(
				$"This race will no longer trigger the {which.AdviceTitle.ColourValue()} piece of chargen advice.");
		}
		else
		{
			_chargenAdvices.Add(which);
			actor.OutputHandler.Send(
				$"This race will now trigger the {which.AdviceTitle.ColourValue()} piece of chargen advice.");
		}

		Changed = true;
		return true;
	}

	private bool BuildingCommandEdibleYield(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which yield type do you want to edit the edible properties of?");
			return false;
		}

		var yield = command.PopSpeech().ToLowerInvariant();
		switch (yield)
		{
			case "add":
				return BuildingCommandYieldAdd(actor, command);
			case "remove":
				return BuildingCommandYieldRemove(actor, command);
		}

		if (!EdibleForagableYields.Any(x => x.YieldType.EqualTo(yield)))
		{
			actor.OutputHandler.Send($"There are no existing yield profiles for the yield type {yield.ColourValue()}.");
			return false;
		}

		switch (command.PopForSwitch())
		{
			case "calories":
				return BuildingCommandEdibleYieldCalories(actor, command, yield);
			case "hunger":
				return BuildingCommandEdibleYieldHunger(actor, command, yield);
			case "water":
				return BuildingCommandEdibleYieldWater(actor, command, yield);
			case "thirst":
				return BuildingCommandEdibleYieldThirst(actor, command, yield);
			case "alcohol":
				return BuildingCommandEdibleYieldAlcohol(actor, command, yield);
			case "emote":
				return BuildingCommandEdibleYieldEmote(actor, command, yield);
			case "bite":
				return BuildingCommandEdibleYieldBite(actor, command, yield);
			default:
				actor.OutputHandler.Send(@"Your valid options are as follows:

	#3calories <amount>#0
	#3hunger <hours>#0
	#3water <amount>#0
	#3thirst <hours>#0
	#3alcohol <grams>#0
	#3emote <emote>#0
	#3bite <yield per bite>#0".SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandYieldAdd(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What yield type would you like to add?");
			return false;
		}

		var yield = command.PopSpeech().ToLowerInvariant();
		if (Gameworld.ForagableProfiles.All(x => !x.MaximumYieldPoints.ContainsKey(yield)))
		{
			actor.OutputHandler.Send("None of the foragable profiles contain any yield with that name. You must first make sure at least one foragable profile contains such a yield.");
			return false;
		}

		if (EdibleForagableYields.Any(x => x.YieldType.EqualTo(yield)))
		{
			actor.OutputHandler.Send("This race already has a profile for that yield type.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How many hours of hunger should this yield fulfil?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var hunger))
		{
			actor.OutputHandler.Send("That is not a valid number of hours.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How many hours of thirst should this yield fulfil?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var thirst))
		{
			actor.OutputHandler.Send("That is not a valid number of hours.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the emote for a character eating this yield? Use $0 to refer to the character.");
			return false;
		}

		var emoteText = command.SafeRemainingArgument;
		var emote = new Emote(emoteText, new DummyPerceiver(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		_edibleForagableYields.Add(new EdibleForagableYield { 
			YieldType = yield,
			YieldPerBite = 1.0,
			HungerPerYield = hunger,
			ThirstPerYield = thirst,
			CaloriesPerYield = hunger * 100,
			WaterPerYield = 0.1 * thirst,
			AlcoholPerYield = 0.0,
			EmoteText = emoteText
		});
		Changed = true;
		actor.OutputHandler.Send($"You add an edible foragable type for {yield.ColourValue()} to this race with {hunger.ToString("N2", actor).ColourValue()} hours hunger and {thirst.ToString("N2", actor).ColourValue()} hours thirst and an emote of {emoteText.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandYieldRemove(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What yield type would you like to remove?");
			return false;
		}

		var yield = command.PopSpeech().ToLowerInvariant();		
		if (!EdibleForagableYields.Any(x => x.YieldType.EqualTo(yield)))
		{
			actor.OutputHandler.Send("This race has no profile for such a yield type.");
			return false;
		}

		_edibleForagableYields.RemoveAll(x => x.YieldType.EqualTo(yield));
		Changed = true;
		actor.OutputHandler.Send($"You remove the edible foragable type for the {yield.ColourValue()} yield type.");
		return true;
	}

	private bool BuildingCommandEdibleYieldCalories(ICharacter actor, StringStack command, string yield)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How many calories do you want one point of yield to give?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send("That is not a valid number.");
			return false;
		}

		EdibleForagableYields.First(x => x.YieldType.EqualTo(yield)).CaloriesPerYield = value;
		Changed = true;
		actor.OutputHandler.Send($"Eating one unit of the {yield.ColourValue()} yield will now give {value.ToString("N3", actor).ColourValue()} calories.");
		return true;
	}

	private bool BuildingCommandEdibleYieldHunger(ICharacter actor, StringStack command, string yield)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How many hours of hunger satisfaction do you want one point of yield to give?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send("That is not a valid number.");
			return false;
		}

		EdibleForagableYields.First(x => x.YieldType.EqualTo(yield)).HungerPerYield = value;
		Changed = true;
		actor.OutputHandler.Send($"Eating one unit of the {yield.ColourValue()} yield will now give {value.ToString("N3", actor).ColourValue()} hours of hunger satisfaction.");
		return true;
	}

	private bool BuildingCommandEdibleYieldWater(ICharacter actor, StringStack command, string yield)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How much water do you want one point of yield to give?");
			return false;
		}

		if (!Gameworld.UnitManager.TryGetBaseUnits(command.SafeRemainingArgument, UnitType.FluidVolume, out var value))
		{
			actor.OutputHandler.Send("That is not a valid quantity.");
			return false;
		}

		EdibleForagableYields.First(x => x.YieldType.EqualTo(yield)).WaterPerYield = value * Gameworld.UnitManager.BaseFluidToLitres;
		Changed = true;
		actor.OutputHandler.Send($"Eating one unit of the {yield.ColourValue()} yield will now give {Gameworld.UnitManager.DescribeMostSignificantExact(value, UnitType.FluidVolume, actor).ColourValue()} of water.");
		return true;
	}

	private bool BuildingCommandEdibleYieldThirst(ICharacter actor, StringStack command, string yield)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How many hours of thirst satisfaction do you want one point of yield to give?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send("That is not a valid number.");
			return false;
		}

		EdibleForagableYields.First(x => x.YieldType.EqualTo(yield)).ThirstPerYield = value;
		Changed = true;
		actor.OutputHandler.Send($"Eating one unit of the {yield.ColourValue()} yield will now give {value.ToString("N3", actor).ColourValue()} hours of thirst satisfaction.");
		return true;
	}

	private bool BuildingCommandEdibleYieldAlcohol(ICharacter actor, StringStack command, string yield)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How much alcohol do you want one point of yield to give?");
			return false;
		}

		if (!Gameworld.UnitManager.TryGetBaseUnits(command.SafeRemainingArgument, UnitType.Mass, out var value))
		{
			actor.OutputHandler.Send("That is not a valid quantity.");
			return false;
		}

		EdibleForagableYields.First(x => x.YieldType.EqualTo(yield)).AlcoholPerYield = value * Gameworld.UnitManager.BaseWeightToKilograms;
		Changed = true;
		actor.OutputHandler.Send($"Eating one unit of the {yield.ColourValue()} yield will now give {Gameworld.UnitManager.DescribeMostSignificantExact(value, UnitType.Mass, actor).ColourValue()} of alcohol.");
		return true;
	}

	private bool BuildingCommandEdibleYieldEmote(ICharacter actor, StringStack command, string yield)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should the emote be when someone of this race eats this yield? Use $0 for the character.");
			return false;
		}

		var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		EdibleForagableYields.First(x => x.YieldType.EqualTo(yield)).EmoteText = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"The emote when eating {yield.ColourValue()} yield will now be {command.SafeRemainingArgument.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandEdibleYieldBite(ICharacter actor, StringStack command, string yield)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How much yield should be consumed per \"bite\" of food eaten?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value) || value <= 0.0)
		{
			actor.OutputHandler.Send("That is not a valid number.");
			return false;
		}

		EdibleForagableYields.First(x => x.YieldType.EqualTo(yield)).YieldPerBite = value;
		Changed = true;
		actor.OutputHandler.Send($"Each bite of {yield.ColourValue()} yield will now consume {value.ToString("N3", actor).ColourValue()} units of yield.");
		return true;
	}

	private bool BuildingCommandEatCorpseEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the emote for this race when eating a corpse?");
			return false;
		}

		var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceiver(),
			new DummyPerceiver());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		EatCorpseEmoteText = emote.RawText;
		Changed = true;
		actor.OutputHandler.Send($"The emote when this race eats corpses is now {EatCorpseEmoteText.ColourCommand()}");
		return true;
	}

	private bool BuildingCommandOptInEdibleMaterial(ICharacter actor)
	{
		_optInMaterialEdibility = !_optInMaterialEdibility;
		Changed = true;
		if (_optInMaterialEdibility)
		{
			actor.OutputHandler.Send(
				"This race can now only eat foods and corpses which are made from its specified edible material list.");
		}
		else
		{
			actor.OutputHandler.Send("This race is now free to eat foods and corpses of any material.");
		}

		return true;
	}

	private bool BuildingCommandEdibleMaterial(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which material do you want to edit the edible properties of?");
			return false;
		}

		var material = Gameworld.Materials.GetByIdOrName(command.SafeRemainingArgument);
		if (material is null)
		{
			actor.OutputHandler.Send("There is no such material.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What do you want to do with that edible material? Options are #3add#0, #3remove#0, #3calories#0, #3water#0, #3thirst#0, #3hunger#0, #3alcohol#0."
					.SubstituteANSIColour());
			return false;
		}

		switch (command.PopForSwitch())
		{
			case "add":
			case "new":
			case "create":
				return BuildingCommandEdibleMaterialAdd(actor, command, material);
			case "delete":
			case "del":
			case "remove":
			case "rem":
				return BuildingCommandEdibleMaterialRemove(actor, command, material);
			case "calories":
			case "water":
			case "thirst":
			case "hunger":
			case "alcohol":
				if (_edibleMaterials.All(x => x.Material != material))
				{
					actor.OutputHandler.Send("You must first #3add#0 that material before you can edit its properties."
						.SubstituteANSIColour());
					return false;
				}

				return BuildingCommandEdibleMaterialProperties(actor, command, material);
			default:
				actor.OutputHandler.Send(
					"The valid options are #3add#0, #3remove#0, #3calories#0, #3water#0, #3thirst#0, #3hunger#0, #3alcohol#0."
						.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandEdibleMaterialProperties(ICharacter actor, StringStack command, ISolid material)
	{
		var property = command.Last.ToLowerInvariant().CollapseString();
		if (command.IsFinished)
		{
			switch (property)
			{
				case "calories":
					actor.OutputHandler.Send(
						$"How many calories should eating {Gameworld.UnitManager.DescribeDecimal(1.0 / Gameworld.UnitManager.BaseWeightToKilograms, UnitType.Mass, actor).ColourValue()} of this material give?");
					return false;
				case "water":
					actor.OutputHandler.Send(
						$"How much bio-available water should eating {Gameworld.UnitManager.DescribeDecimal(1.0 / Gameworld.UnitManager.BaseWeightToKilograms, UnitType.Mass, actor).ColourValue()} of this material give?");
					return false;
				case "thirst":
					actor.OutputHandler.Send(
						$"How many hours of thirst satiation should eating {Gameworld.UnitManager.DescribeDecimal(1.0 / Gameworld.UnitManager.BaseWeightToKilograms, UnitType.Mass, actor).ColourValue()} of this material give?");
					return false;
				case "hunger":
					actor.OutputHandler.Send(
						$"How many hours of hunger satiation should eating {Gameworld.UnitManager.DescribeDecimal(1.0 / Gameworld.UnitManager.BaseWeightToKilograms, UnitType.Mass, actor).ColourValue()} of this material give?");
					return false;
				case "alcohol":
					actor.OutputHandler.Send(
						$"What weight of alcohol should eating {Gameworld.UnitManager.DescribeDecimal(1.0 / Gameworld.UnitManager.BaseWeightToKilograms, UnitType.Mass, actor).ColourValue()} of this material give?");
					return false;
			}

			return false;
		}

		var quantity = 0.0;
		switch (property)
		{
			case "calories":
			case "thirst":
			case "hunger":
				if (!double.TryParse(command.SafeRemainingArgument, out quantity))
				{
					actor.OutputHandler.Send("You must enter a valid number.");
					return false;
				}

				break;
			case "water":
				if (!Gameworld.UnitManager.TryGetBaseUnits(command.SafeRemainingArgument, UnitType.FluidVolume,
					    out quantity))
				{
					actor.OutputHandler.Send(
						$"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid fluid volume.");
					return false;
				}

				break;
			case "alcohol":
				if (!Gameworld.UnitManager.TryGetBaseUnits(command.SafeRemainingArgument, UnitType.Mass, out quantity))
				{
					actor.OutputHandler.Send(
						$"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid mass.");
					return false;
				}

				break;
		}

		var em = _edibleMaterials.First(x => x.Material == material);
		Changed = true;
		switch (property)
		{
			case "calories":
				em.CaloriesPerKilogram = quantity;
				actor.OutputHandler.Send(
					$"Eating corpses made from {material.Name.Colour(material.ResidueColour)} will now give {quantity.ToString("N2", actor).ColourValue()} calories per {Gameworld.UnitManager.DescribeDecimal(1.0 / Gameworld.UnitManager.BaseWeightToKilograms, UnitType.Mass, actor).ColourValue()} eaten.");
				return true;
			case "water":
				em.WaterPerKilogram = Gameworld.UnitManager.BaseFluidToLitres * quantity;
				actor.OutputHandler.Send(
					$"Eating corpses made from {material.Name.Colour(material.ResidueColour)} will now give {Gameworld.UnitManager.DescribeDecimal(quantity, UnitType.FluidVolume, actor).ColourValue()} bio-available water per {Gameworld.UnitManager.DescribeDecimal(1.0 / Gameworld.UnitManager.BaseWeightToKilograms, UnitType.Mass, actor).ColourValue()} eaten.");
				return true;
			case "thirst":
				em.ThirstPerKilogram = quantity;
				actor.OutputHandler.Send(
					$"Eating corpses made from {material.Name.Colour(material.ResidueColour)} will now give {quantity.ToString("N2", actor).ColourValue()} hours of thirst satiation per {Gameworld.UnitManager.DescribeDecimal(1.0 / Gameworld.UnitManager.BaseWeightToKilograms, UnitType.Mass, actor).ColourValue()} eaten.");
				return true;
			case "hunger":
				em.HungerPerKilogram = quantity;
				actor.OutputHandler.Send(
					$"Eating corpses made from {material.Name.Colour(material.ResidueColour)} will now give {quantity.ToString("N2", actor).ColourValue()} hours of hunger satiation per {Gameworld.UnitManager.DescribeDecimal(1.0 / Gameworld.UnitManager.BaseWeightToKilograms, UnitType.Mass, actor).ColourValue()} eaten.");
				return true;
			case "alcohol":
				actor.OutputHandler.Send(
					$"Eating corpses made from {material.Name.Colour(material.ResidueColour)} will now give {Gameworld.UnitManager.DescribeDecimal(quantity, UnitType.Mass, actor).ColourValue()} of alcohol per {Gameworld.UnitManager.DescribeDecimal(1.0 / Gameworld.UnitManager.BaseWeightToKilograms, UnitType.Mass, actor).ColourValue()} eaten.");
				return true;
		}

		return true;
	}

	private bool BuildingCommandEdibleMaterialRemove(ICharacter actor, StringStack command, ISolid material)
	{
		if (_edibleMaterials.All(x => x.Material != material))
		{
			actor.OutputHandler.Send(
				$"This race has no edible material profile for {material.Name.Colour(material.ResidueColour)}.");
			return false;
		}

		actor.OutputHandler.Send(
			$"Are you sure that you want to delete the edible material profile for {material.Name.Colour(material.ResidueColour)}? This cannot be undone.\n{Accept.StandardAcceptPhrasing}");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			AcceptAction = text =>
			{
				_edibleMaterials.RemoveAll(x => x.Material == material);
				Changed = true;
				actor.OutputHandler.Send(
					$"You delete the edible material profile for {material.Name.Colour(material.ResidueColour)}.");
			},
			RejectAction = text =>
			{
				actor.OutputHandler.Send(
					$"You decide not to delete the edible material profile for {material.Name.Colour(material.ResidueColour)}.");
			},
			ExpireAction = () =>
			{
				actor.OutputHandler.Send(
					$"You decide not to delete the edible material profile for {material.Name.Colour(material.ResidueColour)}.");
			},
			DescriptionString = $"Deleting the editable material profile for {material.Name}",
			Keywords = new List<string> { "delete", "edible", "material" }
		}));
		return true;
	}

	private bool BuildingCommandEdibleMaterialAdd(ICharacter actor, StringStack command, ISolid material)
	{
		if (_edibleMaterials.Any(x => x.Material == material))
		{
			actor.OutputHandler.Send(
				$"There is already an edible material profile for {material.Name.Colour(material.ResidueColour)}. You should instead edit its properties directly.");
			return false;
		}

		_edibleMaterials.Add(new EdibleMaterial
		{
			Material = material,
			AlcoholPerKilogram = 0.0,
			ThirstPerKilogram = 0.0,
			WaterPerKilogram = 0.0,
			CaloriesPerKilogram = 1600,
			HungerPerKilogram = 6.0
		});
		Changed = true;
		actor.OutputHandler.Send($"This race can now eat the {material.Name.Colour(material.ResidueColour)} material.");
		return true;
	}

	private bool BuildingCommandBiteWeight(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How much weight should a single bite from this race take from corpses?");
			return false;
		}

		if (!Gameworld.UnitManager.TryGetBaseUnits(command.SafeRemainingArgument, UnitType.Mass, out var value))
		{
			actor.OutputHandler.Send(
				$"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid mass.");
			return false;
		}

		BiteWeight = value;
		actor.OutputHandler.Send(
			$"This race will now eat {Gameworld.UnitManager.DescribeDecimal(value, UnitType.Mass, actor).ColourValue()} of material per bite of a corpse.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandCanEatCorpses(ICharacter actor)
	{
		CanEatCorpses = !CanEatCorpses;
		Changed = true;
		actor.OutputHandler.Send($"This race can {(CanEatCorpses ? "now" : "no longer")} eat corpses.");
		return true;
	}

	private bool BuildingCommandAttributeBonusProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should be used to determine the attribute bonuses for this race?");
			return false;
		}

		var prog = new FutureProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Number, 
			[
				[FutureProgVariableTypes.Trait],
				[FutureProgVariableTypes.Trait, FutureProgVariableTypes.Chargen]
			]
			).LookupProg();
		if (prog is null)
		{
			return false;
		}

		AttributeBonusProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This race now uses the {prog.MXPClickableFunctionName()} prog to determine its attribute bonuses.");
		return true;
	}

	private bool BuildingCommandAttributeCap(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send(
				"You must enter a number that is the cap for the sum of attribute values for this race.");
			return false;
		}

		AttributeTotalCap = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"The total cap for this race's attributes is now {value.ToString("N0", actor).ColourValue()}. This would lead to an average value of {(value / Attributes.Count().IfZero(1)).ToString("N0", actor).ColourValue()} per attribute.");
		return true;
	}

	private bool BuildingCommandAttributeRoll(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must enter a dice expression for the default roll for each attribute (in the form #2x#6d#2y#6+#2z#0, i.e. 3d6, 2d10-1, 1d20 etc).");
			return false;
		}

		if (!Dice.IsDiceExpression(command.SafeRemainingArgument))
		{
			actor.OutputHandler.Send(
				$"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid dice expression.");
			return false;
		}

		DiceExpression = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send(
			$"The dice expression for this race's attributes is now {DiceExpression.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandAttribute(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which attribute do you want to toggle for this race?");
			return false;
		}

		switch (command.SafeRemainingArgument.ToLowerInvariant().CollapseString())
		{
			case "promote":
				return BuildingCommandAttributePromote(actor, command);
			case "demote":
				return BuildingCommandAttributeDemote(actor, command);
		}

		var attribute = Gameworld.Traits.GetByIdOrName(command.SafeRemainingArgument) as IAttributeDefinition;
		if (attribute is null)
		{
			actor.OutputHandler.Send("There is no such attribute like that.");
			return false;
		}

		if (!_attributes.Contains(attribute))
		{
			_attributes.Add(attribute);
			Changed = true;
			actor.OutputHandler.Send(
				$"This race now has the {attribute.Name.ColourName()} attribute. All existing characters have been given a value of {10.ToString("N0", actor).ColourValue()} in this attribute.");
			RecalculateCharactersBecauseOfRaceChange();
			return true;
		}

		actor.OutputHandler.Send(
			$"Are you sure you want to remove the {attribute.Name.ColourName()} attribute from the {Name.ColourName()} race? This will irrevocably remove the existing value of this attribute from all existing characters.\n{Accept.StandardAcceptPhrasing}");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			AcceptAction = text =>
			{
				_attributes.Remove(attribute);
				Changed = true;
				RecalculateCharactersBecauseOfRaceChange();
				actor.OutputHandler.Send(
					$"You remove the {attribute.Name.ColourName()} attribute from the {Name.ColourName()} race.");
			},
			RejectAction = text =>
			{
				actor.OutputHandler.Send(
					$"You decide not to remove the {attribute.Name.ColourName()} attribute from the {Name.ColourName()} race.");
			},
			ExpireAction = () =>
			{
				actor.OutputHandler.Send(
					$"You decide not to remove the {attribute.Name.ColourName()} attribute from the {Name.ColourName()} race.");
			},
			Keywords = new List<string> { "attribute", "remove", attribute.Name.ToLowerInvariant() },
			DescriptionString =
				$"Removing the {attribute.Name.ColourName()} attribute from the {Name.ColourName()} race"
		}));
		return true;
	}

	private bool BuildingCommandAttributeDemote(ICharacter actor, StringStack command)
	{
		var children = Gameworld.Races.Where(x => x.ParentRace == this).ToList();
		if (!children.Any())
		{
			actor.OutputHandler.Send("This race has no child races.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which attribute do you want to demote?");
			return false;
		}

		var attribute = _attributes.GetByIdOrName(command.SafeRemainingArgument);
		if (attribute is null)
		{
			actor.OutputHandler.Send("This race has no such attribute like that.");
			return false;
		}

		foreach (var child in children)
		{
			child.AddAttributeFromDemotion(attribute);
		}

		_attributes.Remove(attribute);
		Changed = true;
		actor.OutputHandler.Send(
			$"The {attribute.Name.ColourName()} attribute has now been demoted to being on all of the child races.");
		RecalculateCharactersBecauseOfRaceChange();
		return true;
	}

	private bool BuildingCommandAttributePromote(ICharacter actor, StringStack command)
	{
		if (ParentRace is null)
		{
			actor.OutputHandler.Send(
				"This race does not have a parent race, so cannot promote any attributes.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which attribute do you want to promote?");
			return false;
		}

		var attribute = _attributes.GetByIdOrName(command.SafeRemainingArgument);
		if (attribute is null)
		{
			actor.OutputHandler.Send("This race has no such attribute like that.");
			return false;
		}

		_attributes.Remove(attribute);
		Changed = true;
		actor.OutputHandler.Send(
			$"You push the {attribute.Name.ColourName()} attribute up to the parent race. Those without the attribute will start with a new value of {10.ToString("N0", actor).ColourValue()}.");
		ParentRace.AddAttributeFromPromotion(attribute);
		return true;
	}

	private bool BuildingCommandCharacteristic(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "all":
				return BuildingCommandCharacteristicAddOrSet(actor, command, Gender.Indeterminate);
			case "male":
				return BuildingCommandCharacteristicAddOrSet(actor, command, Gender.Male);
			case "female":
				return BuildingCommandCharacteristicAddOrSet(actor, command, Gender.Female);
			case "remove":
				return BuildingCommandCharacterisitcRemove(actor, command);
			case "promote":
				return BuildingCommandCharacteristicPromote(actor, command);
			case "demote":
				return BuildingCommandCharacteristicDemote(actor, command);
			default:
				actor.OutputHandler.Send(@"You can use the following options with this command:

	#3all <which>#0 - adds or changes a characteristic to be present for all genders
	#3male <which>#0 - adds or changes a characteristic to be present for males only
	#3female <which>#0 - adds or changes a characteristic to be present for females only
	#3remove <which>#0 - removes a characteristic
	#3promote <which>#0 - pushes a characteristic up to the parent race
	#3demote <which>#0 - pushes a characteristic down to all child races".SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandCharacteristicDemote(ICharacter actor, StringStack command)
	{
		var children = Gameworld.Races.Where(x => x.ParentRace == this).ToList();
		if (!children.Any())
		{
			actor.OutputHandler.Send("This race has no child races.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which characteristic do you want to demote?");
			return false;
		}

		var characteristic = _baseCharacteristics.Concat(_maleAndFemaleCharacteristics)
		                                         .GetByIdOrName(command.SafeRemainingArgument);
		if (characteristic is null)
		{
			actor.OutputHandler.Send("This race has no such characteristic to demote.");
			return false;
		}

		var gender = Gender.Indeterminate;
		if (_maleOnlyCharacteristics.Contains(characteristic))
		{
			gender = Gender.Male;
		}
		else if (_femaleOnlyCharacteristics.Contains(characteristic))
		{
			gender = Gender.Female;
		}

		RemoveCharacteristicDueToPromotion(characteristic);
		foreach (var child in children)
		{
			child.DemoteCharacteristicFromParent(characteristic, gender);
		}

		actor.OutputHandler.Send(
			$"You demote the {characteristic.Name.ColourName()} characteristic to all the child races of this race.");
		RecalculateCharactersBecauseOfRaceChange();
		return true;
	}

	private bool BuildingCommandCharacteristicPromote(ICharacter actor, StringStack command)
	{
		if (ParentRace is null)
		{
			actor.OutputHandler.Send(
				"This race does not have a parent race, so cannot promote any characteristics.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which characteristic do you want to promote?");
			return false;
		}

		var characteristic = _baseCharacteristics.Concat(_maleAndFemaleCharacteristics)
		                                         .GetByIdOrName(command.SafeRemainingArgument);
		if (characteristic is null)
		{
			actor.OutputHandler.Send("This race has no such characteristic to promote.");
			return false;
		}

		var gender = Gender.Indeterminate;
		if (_maleOnlyCharacteristics.Contains(characteristic))
		{
			gender = Gender.Male;
		}
		else if (_femaleOnlyCharacteristics.Contains(characteristic))
		{
			gender = Gender.Female;
		}

		ParentRace.PromoteCharacteristicFromChildren(characteristic, gender);
		actor.OutputHandler.Send(
			$"You push the {characteristic.Name.ColourName()} characteristic up to the parent race.");
		return true;
	}

	private bool BuildingCommandCharacterisitcRemove(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which characteristic do you want to remove?");
			return false;
		}

		var characteristic = _baseCharacteristics.Concat(_maleAndFemaleCharacteristics)
		                                         .GetByIdOrName(command.SafeRemainingArgument);
		if (characteristic is null)
		{
			actor.OutputHandler.Send("This race has no such characteristic to remove.");
			return false;
		}

		actor.OutputHandler.Send(
			$"Are you sure that you want to remove the {characteristic.Name.ColourName()} from this race? This will have immediate effect and will remove current values from all characters.\n{Accept.StandardAcceptPhrasing}");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			AcceptAction = text =>
			{
				_baseCharacteristics.Remove(characteristic);
				_femaleOnlyCharacteristics.Remove(characteristic);
				_maleOnlyCharacteristics.Remove(characteristic);
				_maleAndFemaleCharacteristics.Remove(characteristic);
				Changed = true;
				RecalculateCharactersBecauseOfRaceChange();
				actor.OutputHandler.Send(
					$"The {Name.ColourName()} race no longer has the {characteristic.Name.ColourName()} characteristic.");
			},
			RejectAction = text =>
			{
				actor.OutputHandler.Send(
					$"You decide not to remove the {characteristic.Name.ColourName()} characteristic from the {Name.ColourName()} race.");
			},
			ExpireAction = () =>
			{
				actor.OutputHandler.Send(
					$"You decide not to remove the {characteristic.Name.ColourName()} characteristic from the {Name.ColourName()} race.");
			},
			Keywords = new List<string> { "characteristic", "variable", "remove" },
			DescriptionString =
				$"Removing the {characteristic.Name.ColourName()} characteristic from the {Name.ColourName()} race"
		}), TimeSpan.FromSeconds(120));
		return true;
	}

	private bool BuildingCommandCharacteristicAddOrSet(ICharacter actor, StringStack command, Gender gender)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which characteristic are you adding or changing?");
			return false;
		}

		var characteristic = Gameworld.Characteristics
		                              .GetByIdOrName(command.SafeRemainingArgument);
		if (characteristic is null)
		{
			actor.OutputHandler.Send("There is no such characteristic to add.");
			return false;
		}

		if (ParentRace is not null && ParentRace.GenderedCharacteristics.Any(x => x.Definition == characteristic))
		{
			actor.OutputHandler.Send(
				$"The {Name.ColourName()} race already has that characteristic from a parent race, and so cannot have it independently.");
			return false;
		}

		foreach (var race in Gameworld.Races.Where(x => x.SameRace(this)))
		{
			if (race.GenderedCharacteristics.Any(x => x.Definition == characteristic))
			{
				actor.OutputHandler.Send(
					"Some of the child race of this race already have that characteristic. You must first remove it from them before you can add it to the parent race.");
				return false;
			}
		}

		_baseCharacteristics.Remove(characteristic);
		_maleOnlyCharacteristics.Remove(characteristic);
		_femaleOnlyCharacteristics.Remove(characteristic);
		_maleAndFemaleCharacteristics.Remove(characteristic);

		switch (gender)
		{
			case Gender.Indeterminate:
				_baseCharacteristics.Add(characteristic);
				actor.OutputHandler.Send(
					$"The {characteristic.Name.ColourName()} characteristic is now possessed by all genders of this race.");
				break;
			case Gender.Male:
				_maleOnlyCharacteristics.Add(characteristic);
				_maleAndFemaleCharacteristics.Add(characteristic);
				actor.OutputHandler.Send(
					$"The {characteristic.Name.ColourName()} characteristic is now possessed by males of this race.");
				break;
			case Gender.Female:
				_femaleOnlyCharacteristics.Add(characteristic);
				_maleAndFemaleCharacteristics.Add(characteristic);
				actor.OutputHandler.Send(
					$"The {characteristic.Name.ColourName()} characteristic is now possessed by females of this race.");
				break;
		}

		Changed = true;
		RecalculateCharactersBecauseOfRaceChange();
		return true;
	}

	private bool BuildingCommandAge(ICharacter actor, StringStack command)
	{
		if (!command.PopSpeech().TryParseEnum(out AgeCategory category))
		{
			actor.OutputHandler.Send(
				$"The valid age categories are {Enum.GetValues<AgeCategory>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What should be the minimum age for {category.DescribeEnum().ColourValue()} members of this species?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value < 1)
		{
			actor.OutputHandler.Send("You must enter a number 1 or higher.");
			return false;
		}

		if (category == Heritage.AgeCategory.Baby)
		{
			actor.OutputHandler.Send("You cannot set the minimum age for babies. Their minimum age is fixed.");
			return false;
		}

		if (!Ages.ChangeLowerBounds(category, value))
		{
			switch (category)
			{
				case Heritage.AgeCategory.Venerable:
					actor.OutputHandler.Send(
						$"You cannot enter an age for {category.DescribeEnum().ColourValue()} that is equal or lower than the age for {category.StageDown().DescribeEnum().ColourValue()} ({MinimumAgeForCategory(category.StageDown()).ToString("N0", actor).ColourValue()}).");
					return false;
				default:
					actor.OutputHandler.Send(
						$"You cannot enter an age for {category.DescribeEnum().ColourValue()} that is equal or lower than the age for {category.StageDown().DescribeEnum().ColourValue()} ({MinimumAgeForCategory(category.StageDown()).ToString("N0", actor).ColourValue()}) or equal or higher than the age for than the age for {category.StageUp().DescribeEnum().ColourValue()} ({MinimumAgeForCategory(category.StageUp()).ToString("N0", actor).ColourValue()}).");
					return false;
			}
		}

		actor.OutputHandler.Send(
			$"The minimum age for a member of this race to be considered a {category.DescribeEnum().ColourValue()} is now {value.ToString("N0", actor).ColourValue()}.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandCanUseWeapons(ICharacter actor)
	{
		CombatSettings.CanUseWeapons = !CombatSettings.CanUseWeapons;
		Changed = true;
		actor.OutputHandler.Send($"This race can {(CombatSettings.CanUseWeapons ? "now" : "no longer")} use weapons.");
		return true;
	}

	private bool BuildingCommandCanDefend(ICharacter actor)
	{
		CombatSettings.CanDefend = !CombatSettings.CanDefend;
		Changed = true;
		actor.OutputHandler.Send($"This race can {(CombatSettings.CanDefend ? "now" : "no longer")} defend.");
		return true;
	}

	private bool BuildingCommandCanAttack(ICharacter actor)
	{
		CombatSettings.CanAttack = !CombatSettings.CanAttack;
		Changed = true;
		actor.OutputHandler.Send($"This race can {(CombatSettings.CanAttack ? "now" : "no longer")} attack.");
		return true;
	}

	private bool BuildingCommandArmourMaterial(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"You must either specify a material or use {"none".ColourCommand()}.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			NaturalArmourMaterial = null;
			Changed = true;
			actor.OutputHandler.Send("This race will no longer have any material type for its natural armour.");
			return true;
		}

		var material = Gameworld.Materials.GetByIdOrName(command.SafeRemainingArgument);
		if (material is null)
		{
			actor.OutputHandler.Send("There is no such material.");
			return false;
		}

		NaturalArmourMaterial = material;
		Changed = true;
		actor.OutputHandler.Send(
			$"This race will now use the {material.Name.Colour(material.ResidueColour)} material for its natural armour.");
		return true;
	}

	private bool BuildingCommandArmourQuality(ICharacter actor, StringStack command)
	{
		throw new NotImplementedException();
	}

	private bool BuildingCommandArmourType(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"You must either specify an armour type or use {"none".ColourCommand()}.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			NaturalArmourType = null;
			Changed = true;
			actor.OutputHandler.Send("This race will no longer have any natural armour.");
			return true;
		}

		var armour = Gameworld.ArmourTypes.GetByIdOrName(command.SafeRemainingArgument);
		if (armour is null)
		{
			actor.OutputHandler.Send("That is not a valid armour type.");
			return false;
		}

		NaturalArmourType = armour;
		Changed = true;
		actor.OutputHandler.Send($"This race now has a natural armour type of {armour.Name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandTemperatureCeiling(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the maximum temperature that this race can tolerate be?");
			return false;
		}

		if (!Gameworld.UnitManager.TryGetBaseUnits(command.SafeRemainingArgument, UnitType.Temperature,
			    out var value))
		{
			actor.OutputHandler.Send(
				$"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid temperature.");
			return false;
		}

		TemperatureRangeCeiling = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This race will now be able tolerate temperatures as high as {Gameworld.UnitManager.DescribeExact(value, UnitType.Temperature, actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandTemperatureFloor(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the minimum temperature that this race can tolerate be?");
			return false;
		}

		if (!Gameworld.UnitManager.TryGetBaseUnits(command.SafeRemainingArgument, UnitType.Temperature,
			    out var value))
		{
			actor.OutputHandler.Send(
				$"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid temperature.");
			return false;
		}

		TemperatureRangeFloor = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This race will now be able tolerate temperatures as low as {Gameworld.UnitManager.DescribeExact(value, UnitType.Temperature, actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandBloodModel(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"You must either specify a blood model or use {"none".ColourCommand()}.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			BloodModel = null;
			Changed = true;
			actor.OutputHandler.Send("This race will not use a blood type model.");
			return true;
		}

		var model = Gameworld.BloodModels.GetByIdOrName(command.SafeRemainingArgument);
		if (model is null)
		{
			actor.OutputHandler.Send("There is no such blood type model.");
			return false;
		}

		BloodModel = model;
		Changed = true;
		actor.OutputHandler.Send($"This race now uses the blood model {BloodModel.Name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandBlood(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"You must either enter a liquid or {"none".ColourCommand()}.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			_bloodLiquid = null;
			_bloodLiquidId = 0;
			Changed = true;
			actor.OutputHandler.Send("This race will no longer bleed.");
			return true;
		}

		var liquid = Gameworld.Liquids.GetByIdOrName(command.SafeRemainingArgument);
		if (liquid is null)
		{
			actor.OutputHandler.Send("There is no such liquid.");
			return false;
		}

		_bloodLiquid = liquid;
		_bloodLiquidId = liquid.Id;
		Changed = true;
		actor.OutputHandler.Send($"This race now bleeds {liquid.Name.Colour(liquid.DisplayColour)}.");
		return true;
	}

	private bool BuildingCommandSweatRate(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the volume of sweat this race produces in one minute?");
			return false;
		}

		if (!Gameworld.UnitManager.TryGetBaseUnits(command.SafeRemainingArgument, UnitType.FluidVolume,
			    out var value))
		{
			actor.OutputHandler.Send(
				$"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid amount of liquid.");
			return false;
		}

		SweatRateInLitresPerMinute = value * Gameworld.UnitManager.BaseFluidToLitres;
		Changed = true;
		actor.OutputHandler.Send(
			$"This race will now sweat at a rate of {Gameworld.UnitManager.DescribeDecimal(value, UnitType.FluidVolume, actor).ColourValue()} per minute.");
		return true;
	}

	private bool BuildingCommandSweatLiquid(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"You must either enter a liquid or {"none".ColourCommand()}.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			_sweatLiquid = null;
			_sweatLiquidId = 0;
			Changed = true;
			actor.OutputHandler.Send("This race will no longer sweat.");
			return true;
		}

		var liquid = Gameworld.Liquids.GetByIdOrName(command.SafeRemainingArgument);
		if (liquid is null)
		{
			actor.OutputHandler.Send("There is no such liquid.");
			return false;
		}

		_sweatLiquid = liquid;
		_sweatLiquidId = liquid.Id;
		Changed = true;
		actor.OutputHandler.Send($"This race now sweats {_sweatLiquid.Name.Colour(_sweatLiquid.DisplayColour)}.");
		return true;
	}

	private bool BuildingCommandHoldBreath(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What would you like the expression for how long this race can hold its breathe to be?");
			return false;
		}

		var expression = new TraitExpression(command.SafeRemainingArgument, Gameworld);
		if (expression.HasErrors())
		{
			actor.OutputHandler.Send($"There is an error with the expression: {expression.Error.ColourError()}");
			return false;
		}

		HoldBreathLengthExpression = expression;
		Changed = true;
		actor.OutputHandler.Send(
			$"This race's holding breathe length in seconds is now {expression.OriginalFormulaText.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandBreathingRate(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What would you like the expression for breathing volume per minute to be?");
			return false;
		}

		var expression = new TraitExpression(command.SafeRemainingArgument, Gameworld);
		if (expression.HasErrors())
		{
			actor.OutputHandler.Send($"There is an error with the expression: {expression.Error.ColourError()}");
			return false;
		}

		BreathingVolumeExpression = expression;
		Changed = true;
		actor.OutputHandler.Send(
			$"This race's breathing rate in L/minute is now {expression.OriginalFormulaText.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandBreathingModel(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"The valid choices for breathing models are #3nonbreather#0, #3simple#0, #3lung#0, #3gill#0 and #3blowhole#0."
					.SubstituteANSIColour());
			return false;
		}

		try
		{
			var model = GetBreathingStrategy(command.SafeRemainingArgument.ToLowerInvariant().CollapseString());
			BreathingStrategy = model;
			NeedsToBreathe = model.NeedsToBreathe;
		}
		catch
		{
			actor.OutputHandler.Send(
				$"The valid choices for breathing models are #3nonbreather#0, #3simple#0, #3lung#0, #3gill#0 and #3blowhole#0."
					.SubstituteANSIColour());
			return false;
		}

		Changed = true;
		actor.OutputHandler.Send($"This race now uses the {BreathingStrategy.Name.ColourName()} breathing model.");
		return true;
	}

	private bool BuildingCommandButcheryProfile(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must either specify a butchery profile or use {"none".ColourCommand()} to clear one.");
			return false;
		}

		if (command.PeekSpeech().EqualTo("none"))
		{
			foreach (var ch in Gameworld.Characters)
			{
				if (!ch.AffectedBy<Butchering>(this))
				{
					continue;
				}

				ch.RemoveEffect(ch.CombinedEffectsOfType<Butchering>().First(), true);
			}

			ButcheryProfile = null;
			Changed = true;
			actor.OutputHandler.Send($"This race will no longer have a butchery profile.");
			return true;
		}

		var profile = Gameworld.RaceButcheryProfiles.GetByIdOrName(command.SafeRemainingArgument);
		if (profile is null)
		{
			actor.OutputHandler.Send("There is no such butchery profile.");
			return false;
		}

		foreach (var ch in Gameworld.Characters)
		{
			if (!ch.AffectedBy<Butchering>(this))
			{
				continue;
			}

			ch.RemoveEffect(ch.CombinedEffectsOfType<Butchering>().First(), true);
		}

		ButcheryProfile = profile;
		Changed = true;
		actor.OutputHandler.Send($"This race now uses the {profile.Name.ColourName()} butchery profile.");
		return true;
	}

	private bool BuildingCommandGenders(ICharacter actor, StringStack command)
	{
		var allowedGenders = new List<Gender>
		{
			Gender.Male,
			Gender.Female,
			Gender.NonBinary,
			Gender.Neuter
		};

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Enter the allowable genders for this race separated by spaces. The allowable genders are {allowedGenders.Select(x => x.DescribeEnum(colour: Telnet.Green)).ListToString()}.");
			return false;
		}

		var genders = new List<Gender>();
		while (!command.IsFinished)
		{
			var text = command.PopSpeech();
			if (!text.TryParseEnum(out Gender gender) || !allowedGenders.Contains(gender))
			{
				actor.OutputHandler.Send(
					$"The text {text.ColourCommand()} is not a valid gender. The allowable genders are {allowedGenders.Select(x => x.DescribeEnum(colour: Telnet.Green)).ListToString()}.");
				return false;
			}

			if (!genders.Contains(gender))
			{
				genders.Add(gender);
			}
		}

		_allowedGenders.Clear();
		_allowedGenders.AddRange(genders);
		Changed = true;
		actor.OutputHandler.Send(
			$"This race will now allow the following genders: {genders.Select(x => x.DescribeEnum(colour: Telnet.Green)).ListToString()}.");
		return true;
	}

	private bool BuildingCommandIlluminationModifier(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !command.SafeRemainingArgument.TryParsePercentage(out var value) || value < 0.0)
		{
			actor.OutputHandler.Send("You must enter a valid percentage value.");
			return false;
		}

		IlluminationPerceptionMultiplier = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This race now has an illumination modifier of {IlluminationPerceptionMultiplier.ToString("P2", actor).ColourValue()}");
		return true;
	}

	private bool BuildingCommandHealthModel(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What health model should this race use?");
			return false;
		}

		var model = Gameworld.HealthStrategies.GetByIdOrName(command.SafeRemainingArgument);
		if (model is null)
		{
			actor.OutputHandler.Send("There is no such health model.");
			return false;
		}

		DefaultHealthStrategy = model;
		Changed = true;
		actor.OutputHandler.Send($"This race now uses the {model.Name.ColourName()} health model.");
		return true;
	}

	private bool BuildingCommandCorpseModel(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What corpse model should this race use?");
			return false;
		}

		var model = Gameworld.CorpseModels.GetByIdOrName(command.SafeRemainingArgument);
		if (model is null)
		{
			actor.OutputHandler.Send("There is no such corpse model.");
			return false;
		}

		CorpseModel = model;
		Changed = true;
		actor.OutputHandler.Send($"This race now uses the {model.Name.ColourName()} corpse model.");
		return true;
	}

	private bool BuildingCommandBody(ICharacter actor, StringStack command)
	{
		if (!actor.IsAdministrator(PermissionLevel.HighAdmin))
		{
			actor.OutputHandler.Send(
				"This command is very dangerous and can only be run by someone of high administrator privileges.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which body template do you want to use for this race?");
			return false;
		}

		var body = Gameworld.BodyPrototypes.GetByIdOrName(command.SafeRemainingArgument);
		if (body is null)
		{
			actor.OutputHandler.Send("There is no such body template.");
			return false;
		}

		if (ParentRace is not null && !body.CountsAs(ParentRace.BaseBody))
		{
			actor.OutputHandler.Send(
				$"As this race has a parent race, the body must be the same or a subtype of the parent's body type ({ParentRace.BaseBody.Name.ColourName()}).");
			return false;
		}

		actor.OutputHandler.Send(
			$"{"Warning: Changing the body type of a race that has any existing characters, especially PCs, is VERY dangerous. This could lead to character dying because of missing organs, dropping their inventory (including offline characters) etc. Be absolutely certain that you are aware of the consequences of what you are doing before you change this body.".ColourError()}\n{Accept.StandardAcceptPhrasing}");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			AcceptAction = text =>
			{
				actor.OutputHandler.Send(
					$"You update the body template of the {Name.ColourName()} race to {body.Name.ColourName()}.");
				BaseBody = body;
				Changed = true;
				RecalculateCharactersBecauseOfRaceChange();
			},
			RejectAction = text => { actor.OutputHandler.Send("You decide not to update the race's body template."); },
			ExpireAction = () => { actor.OutputHandler.Send("You decide not to update the race's body template."); },
			Keywords = new List<string> { "body" },
			DescriptionString = "Changing a body template for a race"
		}));
		return true;
	}

	private bool BuildingCommandChargenAvailabilityProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a prog to control availability of this race in chargen.");
			return false;
		}

		var prog = new FutureProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Boolean, new List<FutureProgVariableTypes>
			{
				FutureProgVariableTypes.Chargen
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		AvailabilityProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This race will now use the {prog.MXPClickableFunctionName()} prog to determine availability in chargen.");
		return true;
	}

	private bool BuildingCommandParent(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must either enter the name or id of another race, or use {"none".ColourCommand()} to clear the parent race.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			if (ParentRace is null)
			{
				actor.OutputHandler.Send("This race does not have a parent race.");
				return false;
			}

			actor.OutputHandler.Send(
				$"Do you want to copy across the attributes, characteristics and additional bodyparts of the former race? Otherwise these will be left as-is, which may cause existing characters to not have these any more.\n{Accept.StandardAcceptPhrasing}");
			actor.AddEffect(new Accept(actor, new GenericProposal
			{
				DescriptionString =
					"Deciding whether to copy attributes, characteristics and bodyparts of the former race",
				AcceptAction = text =>
				{
					if (ParentRace is null)
					{
						actor.OutputHandler.Send("The parent race was already removed.");
						return;
					}

					foreach (var attribute in ParentRace.Attributes)
					{
						if (!_attributes.Contains(attribute))
						{
							_attributes.Add(attribute);
						}
					}

					foreach (var characteristic in ParentRace.GenderedCharacteristics)
					{
						switch (characteristic.Gender)
						{
							case Gender.Indeterminate:
								if (_baseCharacteristics.Contains(characteristic.Definition))
								{
									continue;
								}

								_baseCharacteristics.Add(characteristic.Definition);
								break;
							case Gender.Male:
								if (_maleOnlyCharacteristics.Contains(characteristic.Definition))
								{
									continue;
								}

								_maleOnlyCharacteristics.Add(characteristic.Definition);
								_maleAndFemaleCharacteristics.Add(characteristic.Definition);
								break;
							case Gender.Female:
								if (_femaleOnlyCharacteristics.Contains(characteristic.Definition))
								{
									continue;
								}

								_femaleOnlyCharacteristics.Add(characteristic.Definition);
								_maleAndFemaleCharacteristics.Add(characteristic.Definition);
								break;
							default:
								throw new ArgumentOutOfRangeException();
						}
					}

					foreach (var item in ParentRace.BodypartAdditions)
					{
						if (_bodypartAdditions.Contains(item))
						{
							continue;
						}

						_bodypartAdditions.Add(item);
					}

					foreach (var item in ParentRace.MaleOnlyAdditions)
					{
						if (_maleOnlyAdditions.Contains(item))
						{
							continue;
						}

						_maleOnlyAdditions.Add(item);
					}

					foreach (var item in ParentRace.FemaleOnlyAdditions)
					{
						if (_femaleOnlyAdditions.Contains(item))
						{
							continue;
						}

						_femaleOnlyAdditions.Add(item);
					}

					foreach (var item in ParentRace.BodypartRemovals)
					{
						if (_bodypartRemovals.Contains(item))
						{
							continue;
						}

						_bodypartRemovals.Add(item);
					}

					ParentRace = null;
					Changed = true;
					actor.OutputHandler.Send(
						$"This race is no longer a child race of any other. It copied across any attributes, characteristics or bodyparts from the old parent.");
					RecalculateCharactersBecauseOfRaceChange();
				},
				RejectAction = text =>
				{
					ParentRace = null;
					Changed = true;
					actor.OutputHandler.Send(
						$"This race is no longer a child race of any other. It did not copy across any attributes, characteristics or bodyparts.");
					RecalculateCharactersBecauseOfRaceChange();
				},
				ExpireAction = () => { },
				Keywords = new List<string> { "copy", "race", "parent" }
			}), TimeSpan.FromSeconds(120));

			return true;
		}

		var race = Gameworld.Races.GetByIdOrName(command.SafeRemainingArgument);
		if (race is null)
		{
			actor.OutputHandler.Send("There is no such race to set as a parent.");
			return false;
		}

		if (race.SameRace(this) || SameRace(race))
		{
			actor.OutputHandler.Send(
				$"You cannot make this race a child of the {race.Name.ColourName()} race as that would create a loop.");
			return false;
		}

		if (!BaseBody.CountsAs(ParentRace.BaseBody))
		{
			actor.OutputHandler.Send(
				$"{"Warning: This parent race has a non-compatible body type with this race's current body type, so it will be changed. Changing the body type of a race that has any existing characters, especially PCs, is VERY dangerous. This could lead to character dying because of missing organs, dropping their inventory (including offline characters) etc. Be absolutely certain that you are aware of the consequences of what you are doing before you change this race's body.".ColourError()}\n{Accept.StandardAcceptPhrasing}");
			actor.AddEffect(new Accept(actor, new GenericProposal
			{
				AcceptAction = text =>
				{
					ParentRace = race;
					BaseBody = ParentRace.BaseBody;
					Changed = true;
					actor.OutputHandler.Send($"This race is now a child race of the {race.Name.ColourName()} race.");
					RecalculateCharactersBecauseOfRaceChange();
				},
				RejectAction = text => { actor.OutputHandler.Send("You decide not to update the race's parent."); },
				ExpireAction = () => { actor.OutputHandler.Send("You decide not to update the race's parent."); },
				Keywords = new List<string> { "body" },
				DescriptionString = "Changing a parent for a race"
			}));
			return true;
		}

		ParentRace = race;
		Changed = true;
		actor.OutputHandler.Send($"This race is now a child race of the {race.Name.ColourName()} race.");
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to this race?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.Races.Except(this).Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a race with that name. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename the {_name.ColourName()} race to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	private void RecalculateCharactersBecauseOfRaceChange()
	{
		foreach (var character in Gameworld.Characters)
		{
			if (character.Race.SameRace(this))
			{
				character.Body.RecalculatePartsAndOrgans();
				character.RecalculateCharacteristicsDueToExternalChange();
			}
		}
	}

	/// <inheritdoc />
	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Race #{Id.ToString("N0", actor)} - {Name.ColourName()}");
		sb.AppendLine();
		sb.AppendLine($"\t{Description.Wrap(actor.InnerLineFormatLength, "\t")}");
		sb.AppendLine();
		sb.AppendLine("Core Properties".GetLineWithTitle(actor, Telnet.Blue, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLineColumns((uint)actor.LineFormatLength, 3,
			$"Parent Race: {ParentRace?.Name.ColourValue() ?? "None".ColourError()}",
			$"Chargen Prog: {AvailabilityProg?.MXPClickableFunctionName() ?? "None".ColourError()}",
			$""
		);
		sb.AppendLineColumns((uint)actor.LineFormatLength, 3,
			$"Body: {BaseBody.Name.ColourValue()}",
			$"Corpse: {CorpseModel.Name.ColourValue()}",
			$"Health Model: {DefaultHealthStrategy.Name.ColourValue()}"
		);

		sb.AppendLineColumns((uint)actor.LineFormatLength, 3,
			$"Bodypart Size Mod: {BodypartSizeModifier.ToString("N0", actor).ColourValue()}",
			$"Bodypart Health Multiplier: {BodypartDamageMultiplier.ToString("P2", actor).ColourValue()}",
			"");

		sb.AppendLineColumns((uint)actor.LineFormatLength, 3,
			$"Illumination Multiplier: {IlluminationPerceptionMultiplier.ToString("P2", actor).ColourValue()}",
			$"Handedness: {HandednessOptions.Select(x => x.Describe().Colour(Telnet.Cyan)).ListToString()}",
			$"Default Hand: {DefaultHandedness.Describe().Colour(Telnet.Green)}"
		);
		sb.AppendLineColumns((uint)actor.LineFormatLength, 3,
			$"Communication: {CommunicationStrategy.Name.ColourValue()}",
			$"Genders: {AllowedGenders.Select(x => Gendering.Get(x).GenderClass(true).Colour(Telnet.Cyan)).ListToString()}",
			$"Butchery: {ButcheryProfile?.Name.Colour(Telnet.Green) ?? "None".Colour(Telnet.Red)}"
		);
		sb.AppendLineColumns((uint)actor.LineFormatLength, 3,
			$"Breathing: {BreathingStrategy.Name.ColourValue()}",
			$"Breathing Rate: {BreathingVolumeExpression.OriginalFormulaText.ColourCommand()}",
			$"Hold Breathe: {HoldBreathLengthExpression.OriginalFormulaText.ColourCommand()}"
		);
		sb.AppendLineColumns((uint)actor.LineFormatLength, 3,
			$"Sweat Liquid: {SweatLiquid?.Name.Colour(SweatLiquid.DisplayColour) ?? "None".ColourError()}",
			$"Sweat Rate: {$"{actor.Gameworld.UnitManager.Describe(SweatRateInLitresPerMinute / actor.Gameworld.UnitManager.BaseFluidToLitres, UnitType.FluidVolume, actor)} per minute".Colour(Telnet.Green)}",
			""
		);
		sb.AppendLineColumns((uint)actor.LineFormatLength, 3,
			$"Blood: {BloodLiquid?.Name.Colour(BloodLiquid.DisplayColour) ?? "None".ColourError()}",
			$"Blood Models: {BloodModel?.Name.ColourValue() ?? "None".ColourError()}",
			""
		);
		sb.AppendLineColumns((uint)actor.LineFormatLength, 3,
			$"Min Temperature: {Gameworld.UnitManager.DescribeDecimal(TemperatureRangeFloor, UnitType.Temperature, actor).ColourValue()}",
			$"Max Temperature: {Gameworld.UnitManager.DescribeDecimal(TemperatureRangeCeiling, UnitType.Temperature, actor).ColourValue()}",
			""
		);
		sb.AppendLineColumns((uint)actor.LineFormatLength, 3,
			$"Armour: {NaturalArmourType?.Name.Colour(Telnet.Green) ?? "None".Colour(Telnet.Red)}",
			$"Armour Material: {NaturalArmourMaterial?.Name.Colour(Telnet.Green) ?? "None".Colour(Telnet.Red)}",
			$"Armour Quality: {NaturalArmourQuality.Describe().Colour(Telnet.Green)}"
		);
		sb.AppendLineColumns((uint)actor.LineFormatLength, 3,
			$"Can Attack: {CombatSettings.CanAttack.ToString().Colour(Telnet.Green)}",
			$"Can Defend: {CombatSettings.CanDefend.ToString().Colour(Telnet.Green)}",
			$"Use Weapons: {CombatSettings.CanUseWeapons.ToString().Colour(Telnet.Green)}"
		);
		sb.AppendLine(
			$"Breathable Fluids: {BreathableFluids.Select(x => x.Name.Colour(x.DisplayColour)).ListToCommaSeparatedValues(", ")}");
		sb.AppendLine();
		sb.AppendLine("Ages".GetLineWithTitle(actor, Telnet.Blue, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(
			$"Baby: {0.ToString("N0", actor).ColourValue()} to {MinimumAgeForCategory(Heritage.AgeCategory.Child).ToString("N0", actor).ColourValue()}");
		sb.AppendLine(
			$"Child: {MinimumAgeForCategory(Heritage.AgeCategory.Child).ToString("N0", actor).ColourValue()} to {MinimumAgeForCategory(Heritage.AgeCategory.Youth).ToString("N0", actor).ColourValue()}");
		sb.AppendLine(
			$"Youth: {MinimumAgeForCategory(Heritage.AgeCategory.Youth).ToString("N0", actor).ColourValue()} to {MinimumAgeForCategory(Heritage.AgeCategory.YoungAdult).ToString("N0", actor).ColourValue()}");
		sb.AppendLine(
			$"Young Adult: {MinimumAgeForCategory(Heritage.AgeCategory.YoungAdult).ToString("N0", actor).ColourValue()} to {MinimumAgeForCategory(Heritage.AgeCategory.Adult).ToString("N0", actor).ColourValue()}");
		sb.AppendLine(
			$"Adult: {MinimumAgeForCategory(Heritage.AgeCategory.Adult).ToString("N0", actor).ColourValue()} to {MinimumAgeForCategory(Heritage.AgeCategory.Elder).ToString("N0", actor).ColourValue()}");
		sb.AppendLine(
			$"Elder: {MinimumAgeForCategory(Heritage.AgeCategory.Elder).ToString("N0", actor).ColourValue()} to {MinimumAgeForCategory(Heritage.AgeCategory.Venerable).ToString("N0", actor).ColourValue()}");
		sb.AppendLine(
			$"Venerable: {$"{MinimumAgeForCategory(Heritage.AgeCategory.Venerable).ToString("N0", actor)}+".ColourValue()}");

		sb.AppendLine();
		sb.AppendLine("Characteristics".GetLineWithTitle(actor, Telnet.Blue, Telnet.BoldWhite));
		sb.AppendLine();
		var allCharacteristics = Characteristics(Gender.Indeterminate).ToList();
		var maleCharacteristics = Characteristics(Gender.Male).ToList();
		var femaleCharacteristics = Characteristics(Gender.Female).ToList();
		foreach (var item in allCharacteristics)
		{
			var tags = new List<string>();
			if (!_baseCharacteristics.Contains(item) && !_maleAndFemaleCharacteristics.Contains(item))
			{
				tags.Add(" (From Parent)".Colour(Telnet.BoldMagenta));
				if (maleCharacteristics.Contains(item) && !femaleCharacteristics.Contains(item))
				{
					tags.Add(" (Male)".Colour(Telnet.BoldBlue));
				}
				else if (femaleCharacteristics.Contains(item) && !maleCharacteristics.Contains(item))
				{
					tags.Add(" (Female)".Colour(Telnet.BoldPink));
				}
			}
			else
			{
				if (_maleOnlyCharacteristics.Contains(item))
				{
					tags.Add(" (Male)".Colour(Telnet.BoldBlue));
				}
				else if (_femaleOnlyCharacteristics.Contains(item))
				{
					tags.Add(" (Female)".Colour(Telnet.BoldPink));
				}
			}

			sb.AppendLine($"\t{item.Name.ColourName()}{tags.ListToCommaSeparatedValues("")}");
		}

		sb.AppendLine();
		sb.AppendLine("Attributes".GetLineWithTitle(actor, Telnet.Blue, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLineColumns((uint)actor.LineFormatLength, 3,
			$"Attribute Roll: {DiceExpression.Colour(Telnet.Green)}",
			$"Attribute Cap: {IndividualAttributeCap.ToString("N0", actor).Colour(Telnet.Green)}",
			$"Total Cap: {AttributeTotalCap.ToString("N0", actor).Colour(Telnet.Green)}"
		);
		sb.AppendLineColumns((uint)actor.LineFormatLength, 3,
			$"Bonus Prog: {(AttributeBonusProg == null ? "None".Colour(Telnet.Red) : string.Format("{0} (#{1:N0})".FluentTagMXP("send", $"href='show futureprog {AttributeBonusProg.Id}'"), AttributeBonusProg.FunctionName, AttributeBonusProg.Id))}",
			$"",
			""
		);
		sb.AppendLine();
		foreach (var attribute in Attributes)
		{
			if (!_attributes.Contains(attribute))
			{
				sb.AppendLine($"\t{attribute.Name.ColourName()} {"(From Parent)".Colour(Telnet.BoldMagenta)}");
				continue;
			}

			sb.AppendLine($"\t{attribute.Name.ColourName()}");
		}

		sb.AppendLine();
		sb.AppendLine("Nourishment".GetLineWithTitle(actor, Telnet.Blue, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLineColumns((uint)actor.LineFormatLength, 3,
			$"Hunger Rate: {HungerRate.ToString("P2", actor).ColourValue()}",
			$"Thirst Rate: {ThirstRate.ToString("P2", actor).ColourValue()}",
			""
		);
		sb.AppendLineColumns((uint)actor.LineFormatLength, 3,
			$"Eat Corpses: {CanEatCorpses.ToColouredString()}",
			$"Eat Yields: {EdibleForagableYields.Any().ToColouredString()}",
			$"Bite Weight: {BiteWeight.ToString("N2", actor).ColourValue()}"
		);
		if (OptInMaterialEdibility)
		{
			sb.AppendLine("Edible Materials:");
			sb.AppendLine(StringUtilities.GetTextTable(
				from item in EdibleMaterials
				select new List<string>
				{
					item.Material.Name,
					item.AlcoholPerKilogram.ToString("N2", actor),
					item.CaloriesPerKilogram.ToString("N2", actor),
					item.WaterPerKilogram.ToString("N2", actor),
					item.HungerPerKilogram.ToString("N2", actor),
					item.ThirstPerKilogram.ToString("N2", actor),
					(!_edibleMaterials.Contains(item)).ToColouredString()
				},
				new List<string>
				{
					"Material",
					"Alc / Kg",
					"Cal / Kg",
					"Wat / Kg",
					"Hun / Kg",
					"Thi / Kg",
					"From Parent?"
				},
				actor
			));
		}

		else
		{
			sb.AppendLine($"Edible Materials: {"Any".ColourValue()}");
		}

		sb.AppendLine($"Eat Corpse Emote: {EatCorpseEmoteText.ColourCommand()}");
		sb.AppendLine();
		sb.AppendLine("Edible Yields:");
		sb.AppendLine(StringUtilities.GetTextTable(
			from yield in EdibleForagableYields
			select new List<string>
			{
				yield.YieldType,
				yield.YieldPerBite.ToString("N2", actor),
				yield.AlcoholPerYield.ToString("N2", actor),
				yield.CaloriesPerYield.ToString("N2", actor),
				yield.WaterPerYield.ToString("N2", actor),
				yield.HungerPerYield.ToString("N2", actor),
				yield.ThirstPerYield.ToString("N2", actor),
				(!_edibleForagableYields.Contains(yield)).ToColouredString(),
				yield.EmoteText
			},
			new List<string>
			{
				"Yield",
				"Yield / Bite",
				"Alc / Yield",
				"Cal / Yield",
				"Wat / Yield",
				"Hun / Yield",
				"Thi / Yield",
				"From Parent?",
				"Emote"
			},
			actor,
			truncatableColumnIndex: 7
		));
		sb.AppendLine();
		sb.AppendLine("Chargen Costs".GetLineWithTitle(actor, Telnet.Blue, Telnet.BoldWhite));
		sb.AppendLine();
		if (_costs.Any())
		{
			foreach (var cost in _costs)
			{
				sb.AppendLine(
					$"\t{cost.Amount.ToString("N0", actor).ColourValue()} {cost.Resource.Alias.ColourName()}{(cost.RequirementOnly ? " (Not Expended)".Colour(Telnet.BoldYellow) : "")}");
			}
		}
		else
		{
			sb.AppendLine("\tNone");
		}

		sb.AppendLine();
		sb.AppendLine("Chargen Advice:");
		if (_chargenAdvices.Any())
		{
			foreach (var advice in _chargenAdvices)
			{
				sb.AppendLine();
				sb.AppendLine(
					$"\t{advice.AdviceTitle.ColourCommand()} @ {advice.TargetStage.DescribeEnum(true).ColourValue()} (prog: {advice.ShouldShowAdviceProg?.MXPClickableFunctionNameWithId() ?? "None".Colour(Telnet.Red)})");
				sb.AppendLine();
				sb.AppendLine(advice.AdviceText.Wrap(actor.InnerLineFormatLength, "\t\t"));
			}
		}
		else
		{
			sb.AppendLine("\tNone");
		}

		sb.AppendLine();
		sb.AppendLine("Natural Weapon Attacks".GetLineWithTitle(actor, Telnet.Blue, Telnet.BoldWhite));
		sb.AppendLine();
		if (NaturalWeaponAttacks.Any())
		{
			sb.AppendLine(StringUtilities.GetTextTable(
				from attack in NaturalWeaponAttacks
				select new List<string>
				{
					attack.Attack.Id.ToString("N0", actor),
					attack.Attack.Name,
					attack.Bodypart.Name,
					attack.Quality.Describe(),
					attack.Attack.MoveType.Describe(),
					(!_naturalWeaponAttacks.Contains(attack)).ToColouredString()
				},
				new List<string>
				{
					"Id",
					"Name",
					"Bodypart",
					"Quality",
					"Type",
					"From Parent?"
				},
				actor,
				Telnet.Blue
			));
		}
		else
		{
			sb.AppendLine("\tNone");
		}

		sb.AppendLine();
		sb.AppendLine("Auxiliary Actions".GetLineWithTitle(actor, Telnet.Blue, Telnet.BoldWhite));
		sb.AppendLine();
		if (AuxiliaryActions.Any())
		{
			sb.AppendLine(StringUtilities.GetTextTable(
				from attack in AuxiliaryActions
				select new List<string>
				{
					attack.Id.ToString("N0", actor),
					attack.Name,
					(!_auxiliaryCombatActions.Contains(attack)).ToColouredString()
				},
				new List<string>
				{
					"Id",
					"Name",
					"From Parent?"
				},
				actor,
				Telnet.Blue
			));
		}
		else
		{
			sb.AppendLine("\tNone");
		}

		return sb.ToString();
	}

	#endregion
}
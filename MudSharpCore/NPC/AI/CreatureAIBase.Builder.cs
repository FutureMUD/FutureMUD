#nullable enable
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Celestial;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.GameItems;
using MudSharp.Models;
using MudSharp.NPC.AI.Groups;
using MudSharp.Work.Crafts;
using MoreLinq;

namespace MudSharp.NPC.AI;

public abstract partial class CreatureAIBase : PathingAIBase
{
	protected bool BuildingCommandMovement(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "ground":
				return SetMovementStrategy(actor, AnimalMovementStrategyType.Ground);
			case "swim":
			case "swimming":
				return SetMovementStrategy(actor, AnimalMovementStrategyType.Swim);
			case "fly":
				return SetMovementStrategy(actor, AnimalMovementStrategyType.Fly);
			case "arboreal":
			case "tree":
			case "trees":
				return SetMovementStrategy(actor, AnimalMovementStrategyType.Arboreal);
			case "amphibious":
			case "amphibian":
				return SetMovementStrategy(actor, AnimalMovementStrategyType.Amphibious);
			case "range":
				return BuildingCommandMovementRange(actor, command);
			case "waterbias":
			case "water":
			case "bias":
				return BuildingCommandMovementWaterBias(actor, command);
			case "chance":
				return BuildingCommandMovementChance(actor, command);
			case "enabled":
			case "enabledprog":
				return BuildingCommandMovementEnabledProg(actor, command);
			case "room":
			case "roomprog":
			case "cell":
			case "cellprog":
				return BuildingCommandMovementRoomProg(actor, command);
			case "preferredhabitat":
			case "preferhabitat":
				return BuildingCommandMovementHabitatProg(actor, command, x => PreferredHabitatProg = x,
					"preferred habitat");
			case "toleratedhabitat":
			case "toleratehabitat":
			case "tolerated":
				return BuildingCommandMovementHabitatProg(actor, command, x => ToleratedHabitatProg = x,
					"tolerated transit habitat");
			case "landprog":
			case "land":
				return BuildingCommandAmphibiousRoomProg(actor, command, x => AmphibiousLandRoomProg = x, "land");
			case "waterprog":
			case "watercell":
				return BuildingCommandAmphibiousRoomProg(actor, command, x => AmphibiousWaterRoomProg = x, "water");
			case "flying":
			case "flyinglayer":
				return BuildingCommandLayer(actor, command, x => TargetFlyingLayer = x, "flying travel");
			case "resting":
			case "restinglayer":
				return BuildingCommandLayer(actor, command, x => TargetRestingLayer = x, "resting");
			case "preferred":
			case "preferredlayer":
				return BuildingCommandTreeLayer(actor, command, x => PreferredTreeLayer = x, "preferred");
			case "secondary":
			case "secondarylayer":
				return BuildingCommandTreeLayer(actor, command, x => SecondaryTreeLayer = x, "secondary");
			case "descent":
			case "descentprog":
				return BuildingCommandDescentProg(actor, command);
			case "emote":
			case "wander":
				return BuildingCommandMovementEmote(actor, command);
		}

		actor.OutputHandler.Send(TypeHelpText.SubstituteANSIColour());
		return false;
	}

	protected bool SetMovementStrategy(ICharacter actor, AnimalMovementStrategyType strategy)
	{
		MovementStrategy = strategy;
		MovementRange = DefaultRangeFor(strategy);
		Changed = true;
		actor.OutputHandler.Send(
			$"This creature AI will now use {strategy.DescribeEnum().ColourName()} movement with a range of {MovementRange.ToString("N0", actor).ColourValue()}.");
		return true;
	}

	protected bool BuildingCommandMovementRange(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.SafeRemainingArgument, out int value) || value < 1)
		{
			actor.OutputHandler.Send("You must specify a positive whole number for the movement range.");
			return false;
		}

		MovementRange = value;
		Changed = true;
		actor.OutputHandler.Send($"This creature AI will now search up to {value.ToString("N0", actor).ColourValue()} cells for movement targets.");
		return true;
	}

	protected bool BuildingCommandMovementChance(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !TerritorialWanderer.TryParseWanderChance(command.SafeRemainingArgument, out double value))
		{
			actor.OutputHandler.Send("You must specify a percentage between 0% and 100%.");
			return false;
		}

		WanderChancePerMinute = value;
		Changed = true;
		actor.OutputHandler.Send($"This creature AI will now have a {value.ToString("P2", actor).ColourValue()} ambient movement chance each minute.");
		return true;
	}

	protected bool BuildingCommandMovementWaterBias(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !TerritorialWanderer.TryParseWanderChance(command.SafeRemainingArgument, out double value))
		{
			actor.OutputHandler.Send("You must specify a percentage between 0% and 100%.");
			return false;
		}

		AmphibiousWaterBias = value;
		Changed = true;
		actor.OutputHandler.Send($"Amphibious ambient movement will now prefer water {value.ToString("P2", actor).ColourValue()} of the time.");
		return true;
	}

	protected bool BuildingCommandMovementEnabledProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should control whether ambient movement is enabled?");
			return false;
		}

		IFutureProg? prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean, new[] { ProgVariableTypes.Character }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		MovementEnabledProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This creature AI will now use {prog.MXPClickableFunctionName()} to control ambient movement.");
		return true;
	}

	protected bool BuildingCommandAmphibiousRoomProg(ICharacter actor, StringStack command, Action<IFutureProg> setter, string label)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which prog should evaluate amphibious {label} cells?");
			return false;
		}

		IFutureProg? prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean,
			new[]
			{
				new List<ProgVariableTypes> { ProgVariableTypes.Character, ProgVariableTypes.Location },
				new List<ProgVariableTypes> { ProgVariableTypes.Character, ProgVariableTypes.Location, ProgVariableTypes.Location },
				new List<ProgVariableTypes> { ProgVariableTypes.Location }
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		setter(prog);
		Changed = true;
		actor.OutputHandler.Send($"This creature AI will now use {prog.MXPClickableFunctionName()} for amphibious {label} cells.");
		return true;
	}

	protected bool BuildingCommandMovementRoomProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should evaluate ambient movement target cells?");
			return false;
		}

		IFutureProg? prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean,
			new[]
			{
				new List<ProgVariableTypes> { ProgVariableTypes.Character, ProgVariableTypes.Location },
				new List<ProgVariableTypes> { ProgVariableTypes.Character, ProgVariableTypes.Location, ProgVariableTypes.Location }
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		MovementRoomProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This creature AI will now use {prog.MXPClickableFunctionName()} for ambient movement targets.");
		return true;
	}

	protected bool BuildingCommandLayer(ICharacter actor, StringStack command, Action<RoomLayer> setter, string label)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"You must specify a room layer. Valid values are {Enum.GetValues<RoomLayer>().ListToColouredString()}.");
			return false;
		}

		string valueText = command.SafeRemainingArgument;
		if (!valueText.TryParseEnum(out RoomLayer value))
		{
			actor.OutputHandler.Send($"The text {valueText.ColourCommand()} is not a valid room layer. Valid values are {Enum.GetValues<RoomLayer>().ListToColouredString()}.");
			return false;
		}

		setter(value);
		Changed = true;
		actor.OutputHandler.Send($"The {label} layer is now {value.DescribeEnum().ColourValue()}.");
		return true;
	}

	protected bool BuildingCommandTreeLayer(ICharacter actor, StringStack command, Action<RoomLayer> setter, string label)
	{
		RoomLayer[] validLayers = [RoomLayer.InTrees, RoomLayer.HighInTrees];
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"You must specify a tree layer. Valid values are {validLayers.ListToColouredString()}.");
			return false;
		}

		string valueText = command.SafeRemainingArgument;
		if (!valueText.TryParseEnum(out RoomLayer value) || !validLayers.Contains(value))
		{
			actor.OutputHandler.Send($"The text {valueText.ColourCommand()} is not a valid tree layer. Valid values are {validLayers.ListToColouredString()}.");
			return false;
		}

		setter(value);
		Changed = true;
		actor.OutputHandler.Send($"The {label} tree layer is now {value.DescribeEnum().ColourValue()}.");
		return true;
	}

	protected bool BuildingCommandDescentProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should decide when arboreal movement may descend?");
			return false;
		}

		IFutureProg? prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean, new[] { ProgVariableTypes.Character, ProgVariableTypes.Location }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		AllowDescentProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This creature AI will now use {prog.MXPClickableFunctionName()} to gate arboreal descent.");
		return true;
	}

	protected bool BuildingCommandMovementEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What movement emote should this animal use?");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "remove", "delete"))
		{
			WanderEmote = string.Empty;
			Changed = true;
			actor.OutputHandler.Send("This creature AI will no longer use a movement emote.");
			return true;
		}

		WanderEmote = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"This creature AI will now use {WanderEmote.ColourCommand()} as its movement emote.");
		return true;
	}

	protected bool BuildingCommandHome(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "none":
				return SetHomeStrategy(actor, AnimalHomeStrategyType.None);
			case "territorial":
				return SetHomeStrategy(actor, AnimalHomeStrategyType.Territorial);
			case "denning":
			case "den":
			case "burrow":
				return SetHomeStrategy(actor, AnimalHomeStrategyType.Denning);
			case "territoryprog":
			case "territory":
				return BuildingCommandTerritoryProg(actor, command);
			case "size":
			case "sizeprog":
				return BuildingCommandTerritorySizeProg(actor, command);
			case "share":
				return ToggleShareTerritory(actor);
			case "shareother":
			case "shareothers":
				return ToggleShareOtherTerritory(actor);
			case "shareshelter":
			case "sharegroup":
				return ToggleShareGroupShelter(actor);
			case "craft":
			case "burrowcraft":
				return BuildingCommandBurrowCraft(actor, command);
			case "site":
			case "siteprog":
			case "burrowsite":
				return BuildingCommandBurrowSiteProg(actor, command);
			case "location":
			case "locationprog":
			case "homeprog":
				return BuildingCommandHomeLocationProg(actor, command);
			case "enabled":
			case "enabledprog":
				return BuildingCommandBuildEnabledProg(actor, command);
			case "anchor":
			case "anchorprog":
				return BuildingCommandAnchorProg(actor, command);
		}

		actor.OutputHandler.Send(TypeHelpText.SubstituteANSIColour());
		return false;
	}

	protected bool SetHomeStrategy(ICharacter actor, AnimalHomeStrategyType strategy)
	{
		HomeStrategy = strategy;
		Changed = true;
		actor.OutputHandler.Send($"This creature AI will now use {strategy.DescribeEnum().ColourName()} home behavior.");
		return true;
	}

	protected bool BuildingCommandTerritoryProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should decide suitable territory cells?");
			return false;
		}

		IFutureProg? prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean,
			new[]
			{
				new[] { ProgVariableTypes.Location },
				new[] { ProgVariableTypes.Location, ProgVariableTypes.Character },
				new[] { ProgVariableTypes.Character, ProgVariableTypes.Location }
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		SuitableTerritoryProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This creature AI will now use {prog.MXPClickableFunctionName()} for territory suitability.");
		return true;
	}

	protected bool BuildingCommandTerritorySizeProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should decide desired territory size?");
			return false;
		}

		IFutureProg? prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Number, new[] { ProgVariableTypes.Character }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		DesiredTerritorySizeProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This creature AI will now use {prog.MXPClickableFunctionName()} for territory size.");
		return true;
	}

	protected bool ToggleShareTerritory(ICharacter actor)
	{
		WillShareTerritory = !WillShareTerritory;
		Changed = true;
		actor.OutputHandler.Send($"This creature AI will {WillShareTerritory.NowNoLonger()} share territory with others of its race.");
		return true;
	}

	protected bool ToggleShareOtherTerritory(ICharacter actor)
	{
		WillShareTerritoryWithOtherRaces = !WillShareTerritoryWithOtherRaces;
		Changed = true;
		actor.OutputHandler.Send($"This creature AI will {WillShareTerritoryWithOtherRaces.NowNoLonger()} share territory with other races.");
		return true;
	}

	protected bool BuildingCommandBurrowCraft(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which craft should this AI use to build its burrow? Use #3clear#0 to remove it."
			                         .SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "remove", "delete"))
		{
			_burrowCraftId = 0;
			Changed = true;
			actor.OutputHandler.Send("This creature AI will no longer use a burrow craft.");
			return true;
		}

		ICraft? craft = Gameworld.Crafts.GetByIdOrName(command.SafeRemainingArgument);
		if (craft is null)
		{
			actor.OutputHandler.Send("There is no such craft.");
			return false;
		}

		_burrowCraftId = craft.Id;
		Changed = true;
		actor.OutputHandler.Send($"This creature AI will now use {craft.Name.ColourName()} to build its burrow.");
		return true;
	}

	protected bool BuildingCommandBurrowSiteProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should decide whether a cell is suitable for a burrow?");
			return false;
		}

		IFutureProg? prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean, new[] { ProgVariableTypes.Character, ProgVariableTypes.Location }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		BurrowSiteProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This creature AI will now use {prog.MXPClickableFunctionName()} for burrow sites.");
		return true;
	}

	protected bool BuildingCommandHomeLocationProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should return the fallback home location? Use #3clear#0 to remove it."
			                         .SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "remove", "delete"))
		{
			HomeLocationProg = null;
			Changed = true;
			actor.OutputHandler.Send("This creature AI will no longer use a fallback home-location prog.");
			return true;
		}

		IFutureProg? prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Location, new[] { ProgVariableTypes.Character }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		HomeLocationProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This creature AI will now use {prog.MXPClickableFunctionName()} as its fallback home source.");
		return true;
	}

	protected bool BuildingCommandBuildEnabledProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should decide whether burrow building is enabled?");
			return false;
		}

		IFutureProg? prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean, new[] { ProgVariableTypes.Character }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		BuildEnabledProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This creature AI will now use {prog.MXPClickableFunctionName()} to gate burrow building.");
		return true;
	}

	protected bool BuildingCommandAnchorProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should identify the completed burrow anchor? Use #3clear#0 to remove it."
			                         .SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "remove", "delete"))
		{
			AnchorItemProg = null;
			Changed = true;
			actor.OutputHandler.Send("This creature AI will use fallback burrow-anchor detection.");
			return true;
		}

		IFutureProg? prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean, new[] { ProgVariableTypes.Character, ProgVariableTypes.Item }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		AnchorItemProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This creature AI will now use {prog.MXPClickableFunctionName()} to identify burrow anchors.");
		return true;
	}

	protected bool BuildingCommandEngageDelay(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !Dice.IsDiceExpression(command.SafeRemainingArgument))
		{
			actor.OutputHandler.Send("You must supply a valid dice expression for a number of milliseconds.");
			return false;
		}

		EngageDelayDiceExpression = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"This creature AI will now wait {EngageDelayDiceExpression.ColourValue()} milliseconds before engaging.");
		return true;
	}

	protected bool BuildingCommandEngageEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either supply an emote or use #3clear#0 to remove the emote."
			                         .SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "remove", "delete"))
		{
			EngageEmote = string.Empty;
			Changed = true;
			actor.OutputHandler.Send("This creature AI will no longer use an engage emote.");
			return true;
		}

		Emote emote = new(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		EngageEmote = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"This creature AI will now use this engage emote:\n{EngageEmote.ColourCommand()}");
		return true;
	}

	protected bool BuildingCommandAwareness(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "none":
				return SetAwarenessStrategy(actor, AnimalAwarenessStrategyType.None);
			case "wary":
				return SetAwarenessStrategy(actor, AnimalAwarenessStrategyType.Wary);
			case "wimpy":
				return SetAwarenessStrategy(actor, AnimalAwarenessStrategyType.Wimpy);
			case "skittish":
			case "skittishbird":
				return SetAwarenessStrategy(actor, AnimalAwarenessStrategyType.Skittish);
			case "guarding":
			case "guard":
				return SetAwarenessStrategy(actor, AnimalAwarenessStrategyType.Guarding);
			case "senses":
			case "sense":
				return BuildingCommandAwarenessSenses(actor, command);
			case "threat":
			case "threatprog":
				return BuildingCommandAwarenessThreatProg(actor, command);
			case "avoid":
			case "avoidprog":
			case "cell":
			case "cellprog":
				return BuildingCommandAwarenessAvoidProg(actor, command);
			case "range":
				return BuildingCommandAwarenessRange(actor, command);
			case "memory":
				return BuildingCommandAwarenessMemory(actor, command);
		}

		actor.OutputHandler.Send(TypeHelpText.SubstituteANSIColour());
		return false;
	}

	protected bool SetAwarenessStrategy(ICharacter actor, AnimalAwarenessStrategyType strategy)
	{
		AwarenessStrategy = strategy;
		AwarenessStrategyChanged(strategy);

		Changed = true;
		actor.OutputHandler.Send($"This creature AI will now use {strategy.DescribeEnum().ColourName()} awareness behavior.");
		return true;
	}

	protected virtual void AwarenessStrategyChanged(AnimalAwarenessStrategyType strategy) { }

	protected bool BuildingCommandAwarenessThreatProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should identify feared or disliked characters?");
			return false;
		}

		IFutureProg? prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean,
			new[] { ProgVariableTypes.Character, ProgVariableTypes.Character }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		AwarenessThreatProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This creature AI will now use {prog.MXPClickableFunctionName()} to identify awareness threats.");
		return true;
	}

	protected bool BuildingCommandAwarenessAvoidProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should identify cells this animal avoids?");
			return false;
		}

		IFutureProg? prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean,
			new[]
			{
				new List<ProgVariableTypes> { ProgVariableTypes.Character, ProgVariableTypes.Location },
				new List<ProgVariableTypes> { ProgVariableTypes.Character, ProgVariableTypes.Location, ProgVariableTypes.Location },
				new List<ProgVariableTypes> { ProgVariableTypes.Location }
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		AwarenessAvoidRoomProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This creature AI will now use {prog.MXPClickableFunctionName()} to avoid cells.");
		return true;
	}

	protected bool BuildingCommandAwarenessRange(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.SafeRemainingArgument, out int value) || value < 0)
		{
			actor.OutputHandler.Send("You must specify a non-negative number of rooms.");
			return false;
		}

		AwarenessRange = value;
		Changed = true;
		actor.OutputHandler.Send($"This creature AI will notice awareness threats within {value.ToString("N0", actor).ColourValue()} rooms.");
		return true;
	}

	protected bool BuildingCommandAwarenessMemory(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.SafeRemainingArgument, out int value) || value < 0)
		{
			actor.OutputHandler.Send("You must specify a non-negative number of minutes.");
			return false;
		}

		AwarenessMemoryMinutes = value;
		Changed = true;
		actor.OutputHandler.Send($"This creature AI will remember threat locations for {value.ToString("N0", actor).ColourValue()} minutes.");
		return true;
	}

	protected bool BuildingCommandRefuge(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "none":
				return SetRefugeStrategy(actor, AnimalRefugeStrategyType.None);
			case "home":
				return SetRefugeStrategy(actor, AnimalRefugeStrategyType.Home);
			case "den":
			case "burrow":
				return SetRefugeStrategy(actor, AnimalRefugeStrategyType.Den);
			case "trees":
			case "tree":
				return SetRefugeStrategy(actor, AnimalRefugeStrategyType.Trees);
			case "sky":
			case "air":
				return SetRefugeStrategy(actor, AnimalRefugeStrategyType.Sky);
			case "water":
				return SetRefugeStrategy(actor, AnimalRefugeStrategyType.Water);
			case "prog":
				return SetRefugeStrategy(actor, AnimalRefugeStrategyType.Prog);
			case "layer":
				return BuildingCommandLayer(actor, command, x => RefugeLayer = x, "refuge");
			case "cell":
			case "cellprog":
				return BuildingCommandRefugeRoomProg(actor, command);
			case "return":
			case "returndelay":
				return BuildingCommandRefugeReturn(actor, command);
		}

		actor.OutputHandler.Send(TypeHelpText.SubstituteANSIColour());
		return false;
	}

	protected bool SetRefugeStrategy(ICharacter actor, AnimalRefugeStrategyType strategy)
	{
		RefugeStrategy = strategy;
		if (strategy == AnimalRefugeStrategyType.Sky)
		{
			MovementStrategy = AnimalMovementStrategyType.Fly;
			RefugeLayer = TargetFlyingLayer;
		}
		else if (strategy == AnimalRefugeStrategyType.Trees)
		{
			MovementStrategy = AnimalMovementStrategyType.Arboreal;
			RefugeLayer = PreferredTreeLayer;
		}
		else if (strategy == AnimalRefugeStrategyType.Den && HomeStrategy == AnimalHomeStrategyType.None)
		{
			HomeStrategy = AnimalHomeStrategyType.Denning;
		}

		Changed = true;
		actor.OutputHandler.Send($"This creature AI will now use {strategy.DescribeEnum().ColourName()} refuge behavior.");
		return true;
	}

	protected bool BuildingCommandRefugeRoomProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should identify refuge cells?");
			return false;
		}

		IFutureProg? prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean,
			new[]
			{
				new List<ProgVariableTypes> { ProgVariableTypes.Character, ProgVariableTypes.Location },
				new List<ProgVariableTypes> { ProgVariableTypes.Character, ProgVariableTypes.Location, ProgVariableTypes.Location },
				new List<ProgVariableTypes> { ProgVariableTypes.Location }
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		RefugeRoomProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This creature AI will now use {prog.MXPClickableFunctionName()} to identify refuge cells.");
		return true;
	}

	protected bool BuildingCommandRefugeReturn(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.SafeRemainingArgument, out int value) || value < 0)
		{
			actor.OutputHandler.Send("You must specify a non-negative number of seconds.");
			return false;
		}

		RefugeReturnSeconds = value;
		Changed = true;
		actor.OutputHandler.Send($"This creature AI will wait {value.ToString("N0", actor).ColourValue()} seconds before returning from refuge behavior.");
		return true;
	}

	protected bool ToggleShareGroupShelter(ICharacter actor)
	{
		AllowGroupShelterSharing = !AllowGroupShelterSharing;
		Changed = true;
		actor.OutputHandler.Send($"This creature AI will {AllowGroupShelterSharing.NowNoLonger()} share claimed wildlife shelters with its live group.");
		return true;
	}

	protected bool BuildingCommandAwarenessSenses(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !command.SafeRemainingArgument.TryParseEnum(out AnimalSensesStrategyType strategy))
		{
			actor.OutputHandler.Send($"You must specify an animal senses strategy. Valid values are {Enum.GetValues<AnimalSensesStrategyType>().ListToColouredString()}.");
			return false;
		}

		SensesStrategy = strategy;
		Changed = true;
		actor.OutputHandler.Send($"This creature AI will now use {strategy.DescribeEnum().ColourName()} senses.");
		return true;
	}

	protected bool BuildingCommandThreatPostureEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must supply a posture emote, or use #3clear#0 to remove it. $0 is the animal and $1 is its target.".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "remove", "delete"))
		{
			PostureEmote = string.Empty;
			Changed = true;
			actor.OutputHandler.Send("This creature AI will no longer emit a posture emote.");
			return true;
		}

		Emote emote = new(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable(),
			new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		PostureEmote = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"This creature AI will now use the posture emote {PostureEmote.ColourCommand()}.");
		return true;
	}

	protected bool BuildingCommandThreatPostureDuration(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !Dice.IsDiceExpression(command.SafeRemainingArgument))
		{
			actor.OutputHandler.Send("You must specify a valid dice expression for posture duration in seconds.");
			return false;
		}

		PostureDurationDiceExpression = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"Postures will now last {PostureDurationDiceExpression.ColourValue()} seconds before escalating.");
		return true;
	}

	protected bool BuildingCommandMovementHabitatProg(ICharacter actor, StringStack command, Action<IFutureProg> setter,
		string label)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which prog should identify {label} cells?");
			return false;
		}

		IFutureProg? prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean,
			new[]
			{
				new List<ProgVariableTypes> { ProgVariableTypes.Character, ProgVariableTypes.Location },
				new List<ProgVariableTypes> { ProgVariableTypes.Character, ProgVariableTypes.Location, ProgVariableTypes.Location },
				new List<ProgVariableTypes> { ProgVariableTypes.Location }
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		setter(prog);
		Changed = true;
		actor.OutputHandler.Send($"This creature AI will now use {prog.MXPClickableFunctionName()} for {label} cells.");
		return true;
	}

}

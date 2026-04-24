#nullable enable
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Models;
using MudSharp.Work.Crafts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.NPC.AI;

public class DenningPredatorAI : TerritorialPredatorAI
{
	public ICraft? BurrowCraft { get; private set; }
	public IFutureProg BurrowSiteProg { get; private set; } = null!;
	public IFutureProg? HomeLocationProg { get; private set; }
	public IFutureProg BuildEnabledProg { get; private set; } = null!;
	public IFutureProg? AnchorItemProg { get; private set; }

	protected DenningPredatorAI(ArtificialIntelligence ai, IFuturemud gameworld) : base(ai, gameworld)
	{
	}

	private DenningPredatorAI(IFuturemud gameworld, string name) : base(gameworld, name, "DenningPredator")
	{
		BurrowSiteProg = Gameworld.AlwaysTrueProg;
		BuildEnabledProg = Gameworld.AlwaysTrueProg;
		DatabaseInitialise();
	}

	private DenningPredatorAI()
	{
	}

	public override bool IsReadyToBeUsed => base.IsReadyToBeUsed && BurrowSiteProg is not null &&
	                                        BuildEnabledProg is not null;

	public static new void RegisterLoader()
	{
		RegisterAIType("DenningPredator", (ai, gameworld) => new DenningPredatorAI(ai, gameworld));
		RegisterAIBuilderInformation("denningpredator",
			(gameworld, name) => new DenningPredatorAI(gameworld, name),
			new DenningPredatorAI().HelpText);
	}

	protected override void LoadFromXML(XElement root)
	{
		base.LoadFromXML(root);
		long craftId = long.Parse(root.Element("BurrowCraftId")?.Value ?? "0");
		BurrowCraft = craftId > 0 ? Gameworld.Crafts.Get(craftId) : null;
		BurrowSiteProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("BurrowSiteProg")?.Value ?? "0")) ??
		                 Gameworld.AlwaysTrueProg;
		long homeProgId = long.Parse(root.Element("HomeLocationProg")?.Value ?? "0");
		HomeLocationProg = homeProgId > 0 ? Gameworld.FutureProgs.Get(homeProgId) : null;
		BuildEnabledProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("BuildEnabledProg")?.Value ?? "0")) ??
		                   Gameworld.AlwaysTrueProg;
		long anchorProgId = long.Parse(root.Element("AnchorItemProg")?.Value ?? "0");
		AnchorItemProg = anchorProgId > 0 ? Gameworld.FutureProgs.Get(anchorProgId) : null;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("SuitableTerritoryProg", SuitableTerritoryProg?.Id ?? 0),
			new XElement("DesiredTerritorySizeProg", DesiredTerritorySizeProg?.Id ?? 0),
			new XElement("WanderChancePerMinute", WanderChancePerMinute),
			new XElement("WanderEmote", new XCData(WanderEmote)),
			new XElement("WillAttackProg", WillAttackProg?.Id ?? 0),
			new XElement("EngageDelayDiceExpression", new XCData(EngageDelayDiceExpression)),
			new XElement("EngageEmote", new XCData(EngageEmote)),
			new XElement("BurrowCraftId", BurrowCraft?.Id ?? 0),
			new XElement("BurrowSiteProg", BurrowSiteProg?.Id ?? 0),
			new XElement("HomeLocationProg", HomeLocationProg?.Id ?? 0),
			new XElement("BuildEnabledProg", BuildEnabledProg?.Id ?? 0),
			new XElement("AnchorItemProg", AnchorItemProg?.Id ?? 0),
			new XElement("OpenDoors", OpenDoors),
			new XElement("UseKeys", UseKeys),
			new XElement("SmashLockedDoors", SmashLockedDoors),
			new XElement("CloseDoorsBehind", CloseDoorsBehind),
			new XElement("UseDoorguards", UseDoorguards),
			new XElement("MoveEvenIfObstructionInWay", MoveEvenIfObstructionInWay),
			new XElement("WillShareTerritory", WillShareTerritory),
			new XElement("WillShareTerritoryWithOtherRaces", WillShareTerritoryWithOtherRaces)
		).ToString();
	}

	public override string Show(ICharacter actor)
	{
		StringBuilder sb = new(base.Show(actor));
		sb.AppendLine($"Burrow Craft: {BurrowCraft?.Name.ColourName() ?? "None".ColourError()}");
		sb.AppendLine($"Burrow Site Prog: {BurrowSiteProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Home Location Prog: {HomeLocationProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Build Enabled Prog: {BuildEnabledProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Anchor Item Prog: {AnchorItemProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		return sb.ToString();
	}

	protected override string TypeHelpText => $@"{base.TypeHelpText}
	#3craft <craft>#0 - sets the optional craft used to build the burrow
	#3site <prog>#0 - sets the prog that chooses suitable burrow cells
	#3home <prog>#0 - sets the fallback home location prog
	#3home clear#0 - clears the fallback home location prog
	#3enabled <prog>#0 - sets the prog that controls whether burrow building is active
	#3anchor <prog>#0 - sets the prog that identifies the completed burrow anchor item
	#3anchor clear#0 - clears the anchor-identification prog";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "craft":
			case "burrow":
			case "burrowcraft":
				return BuildingCommandCraft(actor, command);
			case "site":
			case "siteprog":
			case "burrowsite":
			case "burrowsiteprog":
				return BuildingCommandSiteProg(actor, command);
			case "home":
			case "homeprog":
			case "homebase":
			case "homebaseprog":
				return BuildingCommandHomeProg(actor, command);
			case "enabled":
			case "enabledprog":
				return BuildingCommandEnabledProg(actor, command);
			case "anchor":
			case "anchorprog":
				return BuildingCommandAnchorProg(actor, command);
		}

		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandCraft(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which craft should this AI use to build its burrow? Use #3clear#0 to remove it."
			                         .SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "remove", "delete"))
		{
			BurrowCraft = null;
			Changed = true;
			actor.OutputHandler.Send("This AI will no longer use a craft to build its burrow.");
			return true;
		}

		ICraft? craft = Gameworld.Crafts.GetByIdOrName(command.SafeRemainingArgument);
		if (craft is null)
		{
			actor.OutputHandler.Send("There is no such craft.");
			return false;
		}

		BurrowCraft = craft;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now use the craft {craft.Name.ColourName()} to build its burrow.");
		return true;
	}

	private bool BuildingCommandSiteProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should decide whether a cell is suitable for a burrow?");
			return false;
		}

		IFutureProg? prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean,
			new[] { ProgVariableTypes.Character, ProgVariableTypes.Location }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		BurrowSiteProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now use {prog.MXPClickableFunctionName()} to choose burrow sites.");
		return true;
	}

	private bool BuildingCommandHomeProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should return the fallback home location? Use #3clear#0 to remove it.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "remove", "delete"))
		{
			HomeLocationProg = null;
			Changed = true;
			actor.OutputHandler.Send("This AI will now remember home bases from burrow-site selection only.");
			return true;
		}

		IFutureProg? prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Location,
			new[] { ProgVariableTypes.Character }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		HomeLocationProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now use {prog.MXPClickableFunctionName()} as its fallback home source.");
		return true;
	}

	private bool BuildingCommandEnabledProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should control whether burrow building is enabled?");
			return false;
		}

		IFutureProg? prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean,
			new[] { ProgVariableTypes.Character }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		BuildEnabledProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now use {prog.MXPClickableFunctionName()} to determine whether it should build.");
		return true;
	}

	private bool BuildingCommandAnchorProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should identify the completed burrow item? Use #3clear#0 to remove it.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "remove", "delete"))
		{
			AnchorItemProg = null;
			Changed = true;
			actor.OutputHandler.Send("This AI will now use its fallback burrow-anchor detection.");
			return true;
		}

		IFutureProg? prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean,
			new[] { ProgVariableTypes.Character, ProgVariableTypes.Item }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		AnchorItemProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now use {prog.MXPClickableFunctionName()} to identify its burrow item.");
		return true;
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		if (type == EventType.CharacterDiesWitness)
		{
			ICharacter witness = (ICharacter)arguments[1];
			if (witness.State.IsDead() || witness.State.IsInStatis())
			{
				return false;
			}

			HandleWitnessedDeath(witness, (ICharacter)arguments[0]);
			return false;
		}

		ICharacter? ch = arguments[0] as ICharacter;
		if (ch is null || ch.State.IsDead() || ch.State.IsInStatis())
		{
			return false;
		}

		if (NpcSurvivalAIHelpers.TryDrinkIfThirsty(ch))
		{
			return true;
		}

		if (NpcSurvivalAIHelpers.IsThirsty(ch) && NpcBurrowFoodEffect.Get(ch)?.HasAnyTarget != true)
		{
			return base.HandleEvent(type, arguments);
		}

		if (NpcBurrowFoodEffect.Get(ch)?.HasAnyTarget == true)
		{
			switch (type)
			{
				case EventType.CharacterEntersGame:
				case EventType.CharacterEnterCellFinish:
				case EventType.CharacterStopMovement:
				case EventType.CharacterStopMovementClosedDoor:
				case EventType.LeaveCombat:
				case EventType.TenSecondTick:
				case EventType.MinuteTick:
				case EventType.NPCOnGameLoadFinished:
					EvaluateFoodLifecycle(ch);
					return false;
			}
		}

		if (!PredatorAIHelpers.IsHungry(ch))
		{
			switch (type)
			{
				case EventType.CharacterEntersGame:
				case EventType.CharacterEnterCellFinish:
				case EventType.CharacterStopMovement:
				case EventType.CharacterStopMovementClosedDoor:
				case EventType.LeaveCombat:
				case EventType.MinuteTick:
				case EventType.NPCOnGameLoadFinished:
					EvaluateBurrowLifecycle(ch);
					break;
			}

			if (type == EventType.MinuteTick)
			{
				return false;
			}
		}

		return base.HandleEvent(type, arguments);
	}

	public override bool HandlesEvent(params EventType[] types)
	{
		foreach (EventType type in types)
		{
			switch (type)
			{
				case EventType.CharacterEntersGame:
				case EventType.CharacterStopMovement:
				case EventType.CharacterDiesWitness:
					return true;
			}
		}

		return base.HandlesEvent(types);
	}

	private void HandleWitnessedDeath(ICharacter character, ICharacter victim)
	{
		bool wasFightingVictim = character.CombatTarget == victim ||
		                         victim.CombatTarget == character ||
		                         (character.Combat is not null && ReferenceEquals(character.Combat, victim.Combat));
		if (!PredatorAIHelpers.IsHungry(character) ||
		    !wasFightingVictim ||
		    !PredatorAIHelpers.CouldEatAfterKilling(character, victim))
		{
			return;
		}

		NpcBurrowFoodEffect burrowFood = NpcBurrowFoodEffect.GetOrCreate(character);
		burrowFood.SetPendingVictim(victim);
		burrowFood.ClearFood();
	}

	private void EvaluateFoodLifecycle(ICharacter character)
	{
		if (character.Movement is not null || character.Combat is not null)
		{
			return;
		}

		NpcBurrowFoodEffect? burrowFood = NpcBurrowFoodEffect.Get(character);
		if (burrowFood is null)
		{
			return;
		}

		if (!ResolveBurrowFood(character, burrowFood))
		{
			return;
		}

		ICorpse? corpse = burrowFood.FoodCorpse;
		IGameItem? food = corpse?.Parent;
		if (corpse is null || food is null || !character.CanEat(corpse, character.Race.BiteWeight).Success)
		{
			burrowFood.Clear();
			return;
		}

		NpcHomeBaseEffect home = ResolveHomeBase(character);
		if (home.HomeCell is null)
		{
			if (BurrowSiteProg.ExecuteBool(false, character, character.Location))
			{
				home.SetHomeCell(character.Location);
			}
			else
			{
				CheckPathingEffect(character, true);
				return;
			}
		}

		if (!ReferenceEquals(character.Location, home.HomeCell))
		{
			EnsureDraggingFood(character, food);
			CheckPathingEffect(character, true);
			return;
		}

		StopDraggingFood(character, food);
		if (!ReferenceEquals(food.Location, character.Location) &&
		    character.Body.HeldOrWieldedItems.Contains(food) &&
		    character.Body.CanDrop(food, 0))
		{
			character.Body.Drop(food, silent: true);
		}

		if (ReferenceEquals(food.Location, character.Location) ||
		    character.Body.HeldOrWieldedItems.Contains(food))
		{
			character.Eat(corpse, character.Race.BiteWeight, null);
		}

		if (!PredatorAIHelpers.IsHungry(character) || corpse.Parent?.Location is null)
		{
			burrowFood.Clear();
		}
	}

	private bool ResolveBurrowFood(ICharacter character, NpcBurrowFoodEffect burrowFood)
	{
		if (burrowFood.FoodCorpse is not null)
		{
			return true;
		}

		ICharacter? victim = burrowFood.PendingVictim;
		if (victim?.Corpse?.Parent is IGameItem corpseItem &&
		    PredatorAIHelpers.CouldEatAfterKilling(character, victim))
		{
			burrowFood.SetFoodItem(corpseItem);
			burrowFood.ClearPendingVictim();
			return true;
		}

		if (victim is null)
		{
			burrowFood.Clear();
		}

		return false;
	}

	private void EnsureDraggingFood(ICharacter character, IGameItem food)
	{
		if (character.EffectsOfType<Dragging>().Any(x => ReferenceEquals(x.Target, food)))
		{
			return;
		}

		if (!ReferenceEquals(food.Location, character.Location) ||
		    food.GetItemType<IHoldable>() is not { IsHoldable: true })
		{
			return;
		}

		character.AddEffect(new Dragging(character, null, food));
	}

	private void StopDraggingFood(ICharacter character, IGameItem food)
	{
		foreach (Dragging dragging in character.EffectsOfType<Dragging>()
		                                     .Where(x => ReferenceEquals(x.Target, food))
		                                     .ToList())
		{
			character.RemoveEffect(dragging, true);
		}
	}

	private bool IsBuildEnabled(ICharacter character)
	{
		return BuildEnabledProg.ExecuteBool(true, character);
	}

	private NpcHomeBaseEffect ResolveHomeBase(ICharacter character)
	{
		NpcHomeBaseEffect home = NpcHomeBaseEffect.GetOrCreate(character);
		if (home.HomeCell is not null)
		{
			return home;
		}

		if (HomeLocationProg?.Execute<ICell?>(character) is ICell location)
		{
			home.SetHomeCell(location);
		}

		return home;
	}

	private void EvaluateBurrowLifecycle(ICharacter character)
	{
		if (character.Movement is not null ||
		    character.Combat is not null ||
		    character.Effects.Any(x => x.IsBlockingEffect("movement")) ||
		    character.EffectsOfType<IActiveCraftEffect>().Any(x => !ReferenceEquals(x.Component.Craft, BurrowCraft)))
		{
			return;
		}

		NpcHomeBaseEffect home = ResolveHomeBase(character);
		if (home.HomeCell is null)
		{
			if (BurrowSiteProg.ExecuteBool(false, character, character.Location))
			{
				home.SetHomeCell(character.Location);
			}
			else
			{
				CheckPathingEffect(character, true);
				return;
			}
		}

		if (!ReferenceEquals(home.HomeCell, character.Location))
		{
			CheckPathingEffect(character, true);
			return;
		}

		RefreshAnchorItem(character, home);
		if (home.AnchorItem is not null || BurrowCraft is null || !IsBuildEnabled(character))
		{
			return;
		}

		IActiveCraftGameItemComponent? interruptedCraft = character.Location.LayerGameItems(character.RoomLayer)
			.SelectNotNull(x => x.GetItemType<IActiveCraftGameItemComponent>())
			.FirstOrDefault(x => ReferenceEquals(x.Craft, BurrowCraft));
		if (interruptedCraft is not null)
		{
			(bool canResume, string _) = BurrowCraft.CanResumeCraft(character, interruptedCraft);
			if (canResume)
			{
				BurrowCraft.ResumeCraft(character, interruptedCraft);
			}

			return;
		}

		(bool canDoCraft, string _) = BurrowCraft.CanDoCraft(character, null, true, true);
		if (canDoCraft)
		{
			BurrowCraft.BeginCraft(character);
		}
	}

	private void RefreshAnchorItem(ICharacter character, NpcHomeBaseEffect home)
	{
		if (home.AnchorItem is not null && ReferenceEquals(home.AnchorItem.Location, home.HomeCell))
		{
			return;
		}

		home.ClearAnchorItem();
		IGameItem? anchor = DenBuilderAI.SelectAnchorItem(character, AnchorItemProg);
		if (anchor is not null)
		{
			home.SetAnchorItem(anchor);
		}
	}

	protected override bool WouldMove(ICharacter ch)
	{
		if (NpcBurrowFoodEffect.Get(ch)?.HasAnyTarget == true)
		{
			return true;
		}

		if (NpcSurvivalAIHelpers.IsThirsty(ch) && !NpcSurvivalAIHelpers.HasLocalWaterSource(ch))
		{
			return true;
		}

		if (PredatorAIHelpers.IsHungry(ch))
		{
			return base.WouldMove(ch);
		}

		if (ch.Combat is not null ||
		    ch.EffectsOfType<IActiveCraftEffect>().Any(x => ReferenceEquals(x.Component.Craft, BurrowCraft)))
		{
			return false;
		}

		NpcHomeBaseEffect home = ResolveHomeBase(ch);
		return home.HomeCell is null || !ReferenceEquals(ch.Location, home.HomeCell);
	}

	protected override (ICell Target, IEnumerable<ICellExit>) GetPath(ICharacter ch)
	{
		if (NpcBurrowFoodEffect.Get(ch)?.HasAnyTarget == true)
		{
			NpcBurrowFoodEffect food = NpcBurrowFoodEffect.Get(ch)!;
			ResolveBurrowFood(ch, food);
			NpcHomeBaseEffect foodHome = ResolveHomeBase(ch);
			if (food.FoodCorpse is not null && foodHome.HomeCell is not null && !ReferenceEquals(foodHome.HomeCell, ch.Location))
			{
				List<ICellExit> foodHomePath = ch.PathBetween(foodHome.HomeCell, 20, GetSuitabilityFunction(ch)).ToList();
				return foodHomePath.Any()
					? (foodHome.HomeCell, foodHomePath)
					: (null, Enumerable.Empty<ICellExit>());
			}

			if (foodHome.HomeCell is null)
			{
				Tuple<IPerceivable, IEnumerable<ICellExit>> foodTargetPath = ch.AcquireTargetAndPath(
					x => x is ICell cell && BurrowSiteProg.ExecuteBool(false, ch, cell),
					20,
					GetSuitabilityFunction(ch));
				return foodTargetPath.Item1 is ICell foodBurrowCell && foodTargetPath.Item2.Any()
					? (foodBurrowCell, foodTargetPath.Item2)
					: (null, Enumerable.Empty<ICellExit>());
			}

			return (null, Enumerable.Empty<ICellExit>());
		}

		if (NpcSurvivalAIHelpers.IsThirsty(ch) && !NpcSurvivalAIHelpers.HasLocalWaterSource(ch))
		{
			(ICell? waterTarget, IEnumerable<ICellExit> waterPath) =
				NpcSurvivalAIHelpers.GetPathToWater(ch, GetSuitabilityFunction(ch));
			if (waterTarget is not null && waterPath.Any())
			{
				return (waterTarget, waterPath);
			}
		}

		if (PredatorAIHelpers.IsHungry(ch))
		{
			return base.GetPath(ch);
		}

		NpcHomeBaseEffect home = ResolveHomeBase(ch);
		if (home.HomeCell is not null && !ReferenceEquals(home.HomeCell, ch.Location))
		{
			List<ICellExit> homePath = ch.PathBetween(home.HomeCell, 20, GetSuitabilityFunction(ch)).ToList();
			return homePath.Any()
				? (home.HomeCell, homePath)
				: (null, Enumerable.Empty<ICellExit>());
		}

		if (home.HomeCell is not null)
		{
			return (null, Enumerable.Empty<ICellExit>());
		}

		Tuple<IPerceivable, IEnumerable<ICellExit>> targetPath = ch.AcquireTargetAndPath(
			x => x is ICell cell && BurrowSiteProg.ExecuteBool(false, ch, cell),
			20,
			GetSuitabilityFunction(ch));
		return targetPath.Item1 is ICell burrowCell && targetPath.Item2.Any()
			? (burrowCell, targetPath.Item2)
			: (null, Enumerable.Empty<ICellExit>());
	}
}

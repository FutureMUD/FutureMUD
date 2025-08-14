using System;
using System.Data.SqlTypes;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.GameItems.Inventory.Plans;

public abstract class InventoryPlanAction : IInventoryPlanAction, IHaveFuturemud
{
	private readonly long _desiredSecondaryTagId;

	private readonly long _desiredTagId;

	protected InventoryPlanAction(XElement root, IFuturemud gameworld, DesiredItemState state)
	{
		Gameworld = gameworld;
		_desiredTagId = long.Parse(root.Attribute("tag").Value);
		_desiredSecondaryTagId = long.Parse(root.Attribute("secondtag")?.Value ?? "0");
		ItemsAlreadyInPlaceOverrideFitnessScore = bool.Parse(root.Attribute("inplaceoverride")?.Value ?? "true");
		ItemsAlreadyInPlaceMultiplier =
			double.Parse(root.Attribute("inplacemultiplier")?.Value ?? "100.0");
		OriginalReference = root.Attribute("originalreference")?.Value;
		DesiredState = state;
	}

	protected InventoryPlanAction(IFuturemud gameworld, DesiredItemState state, long primaryTag, long secondaryTag,
		Func<IGameItem, bool> primaryselector, Func<IGameItem, bool> secondaryselector,
		Func<IGameItem, double> fitnessscorer = default)
	{
		Gameworld = gameworld;
		_desiredTagId = primaryTag;
		_desiredSecondaryTagId = secondaryTag;
		PrimaryItemSelector = primaryselector;
		SecondaryItemSelector = secondaryselector;
		DesiredState = state;
		PrimaryItemFitnessScorer = fitnessscorer;
	}

	public Func<IGameItem, bool> PrimaryItemSelector { get; set; }
	public Func<IGameItem, bool> SecondaryItemSelector { get; set; }

	public Func<IGameItem, double> PrimaryItemFitnessScorer { get; init; }

	public bool ItemsAlreadyInPlaceOverrideFitnessScore { get; init; }

	public double ItemsAlreadyInPlaceMultiplier { get; init; } = 1.0;

	public object OriginalReference { get; init; }

	public IFuturemud Gameworld { get; set; }

	public ITag DesiredSecondaryTag => Gameworld.Tags.Get(_desiredSecondaryTagId);

	public DesiredItemState DesiredState { get; set; }

	public abstract bool RequiresFreeHandsToExecute(ICharacter who, IGameItem item);

	public ITag DesiredTag => Gameworld.Tags.Get(_desiredTagId);

	public abstract IGameItem ScoutSecondary(ICharacter executor, IGameItem item);

	public abstract IGameItem ScoutTarget(ICharacter executor);

	public abstract string Describe(ICharacter voyeur);

	#region Implementation of IXmlSavable

	public abstract XElement SaveToXml();

	#endregion

	public static IInventoryPlanAction LoadAction(XElement root, IFuturemud gameworld)
	{
		switch (root.Attribute("state").Value)
		{
			case "held":
			case "hold":
				return new InventoryPlanActionHold(root, gameworld);
			case "wield":
			case "wielded":
				return new InventoryPlanActionWield(root, gameworld);
			case "wear":
			case "worn":
				return new InventoryPlanActionWear(root, gameworld);
			case "attach":
			case "attached":
				return new InventoryPlanActionAttach(root, gameworld);
			case "sheath":
			case "sheathed":
				return new InventoryPlanActionSheath(root, gameworld);
			case "drop":
			case "dropped":
			case "inroom":
			case "room":
				return new InventoryPlanActionDrop(root, gameworld);
			case "put":
			case "incontainer":
			case "container":
				return new InventoryPlanActionPut(root, gameworld);
			case "consume":
			case "consumed":
				return new InventoryPlanActionConsume(root, gameworld);
			case "consumecommodity":
			case "consumedcommodity":
				return new InventoryPlanActionConsumeCommodity(root, gameworld);
                        case "consumeliquid":
                        case "consumedliquid":
                                return new InventoryPlanActionConsumeLiquid(root, gameworld);
                        case "apply":
                        case "applied":
                                return new InventoryPlanActionApply(root, gameworld);
                        default:
                                throw new NotImplementedException(
                                        $"InventoryActionPlan type {root.Attribute("state").Value} not implemented in InventoryActionPlan.LoadAction");
                }
        }

	public static IInventoryPlanAction LoadAction(IFuturemud gameworld, DesiredItemState state, long primaryTag,
		long secondaryTag, Func<IGameItem, bool> primaryselector, Func<IGameItem, bool> secondaryselector,
		int quantity = 0, Func<IGameItem, double> fitnessscorer = default, bool preferitemsinplace = true,
		object originalReference = null)
	{
		switch (state)
		{
			case DesiredItemState.Attached:
				return new InventoryPlanActionAttach(gameworld, primaryTag, secondaryTag, primaryselector,
					secondaryselector)
				{
					PrimaryItemFitnessScorer = fitnessscorer,
					ItemsAlreadyInPlaceOverrideFitnessScore = preferitemsinplace,
					OriginalReference = originalReference
				};
			case DesiredItemState.Consumed:
				return new InventoryPlanActionConsume(gameworld, quantity, primaryTag, secondaryTag, primaryselector,
					secondaryselector)
				{
					PrimaryItemFitnessScorer = fitnessscorer,
					ItemsAlreadyInPlaceOverrideFitnessScore = preferitemsinplace,
					OriginalReference = originalReference
				};
			case DesiredItemState.Held:
				return new InventoryPlanActionHold(gameworld, primaryTag, secondaryTag, primaryselector,
					secondaryselector, quantity)
				{
					PrimaryItemFitnessScorer = fitnessscorer,
					ItemsAlreadyInPlaceOverrideFitnessScore = preferitemsinplace,
					OriginalReference = originalReference
				};
			case DesiredItemState.InContainer:
				return new InventoryPlanActionPut(gameworld, primaryTag, secondaryTag, primaryselector,
					secondaryselector)
				{
					PrimaryItemFitnessScorer = fitnessscorer,
					ItemsAlreadyInPlaceOverrideFitnessScore = preferitemsinplace,
					OriginalReference = originalReference
				};
			case DesiredItemState.InRoom:
				return new InventoryPlanActionDrop(gameworld, primaryTag, secondaryTag, primaryselector,
					secondaryselector)
				{
					PrimaryItemFitnessScorer = fitnessscorer,
					ItemsAlreadyInPlaceOverrideFitnessScore = preferitemsinplace,
					OriginalReference = originalReference
				};
			case DesiredItemState.Sheathed:
				return new InventoryPlanActionSheath(gameworld, primaryTag, secondaryTag, primaryselector,
					secondaryselector)
				{
					PrimaryItemFitnessScorer = fitnessscorer,
					ItemsAlreadyInPlaceOverrideFitnessScore = preferitemsinplace,
					OriginalReference = originalReference
				};
			case DesiredItemState.Wielded:
				return new InventoryPlanActionWield(gameworld, primaryTag, secondaryTag, primaryselector,
					secondaryselector)
				{
					PrimaryItemFitnessScorer = fitnessscorer,
					ItemsAlreadyInPlaceOverrideFitnessScore = preferitemsinplace,
					OriginalReference = originalReference
				};
			case DesiredItemState.Worn:
				return new InventoryPlanActionWear(gameworld, primaryTag, secondaryTag, primaryselector,
					secondaryselector)
				{
					PrimaryItemFitnessScorer = fitnessscorer,
					ItemsAlreadyInPlaceOverrideFitnessScore = preferitemsinplace,
					OriginalReference = originalReference
				};
			default:
				throw new NotImplementedException(
					$"InventoryActionPlan type {state} not implemented in InventoryActionPlan.LoadAction");
		}
	}
}
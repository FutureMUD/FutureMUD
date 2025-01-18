using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Inventory.Plans;

public class InventoryPlanTemplate : IInventoryPlanTemplate
{
	private readonly List<IInventoryPlanPhaseTemplate> _phases = new();

	public InventoryPlanTemplate(IFuturemud gameworld, IInventoryPlanAction action) : this(gameworld, new[] { action })
	{
	}

	public InventoryPlanTemplate(IFuturemud gameworld, IEnumerable<IInventoryPlanAction> actions)
	{
		Gameworld = gameworld;
		_phases.Add(new InventoryPlanPhaseTemplate(1, actions));
	}

	public InventoryPlanTemplate(XElement definition, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		var elements = definition.Elements("Phase");
		if (elements.Any())
		{
			var phasenum = 1;
			foreach (var item in elements)
			{
				_phases.Add(new InventoryPlanPhaseTemplate(item, gameworld, phasenum++));
			}
		}
		else
		{
			_phases.Add(new InventoryPlanPhaseTemplate(definition, gameworld, 1));
		}
	}

	public InventoryPlanTemplate(IFuturemud gameworld, IEnumerable<IInventoryPlanPhaseTemplate> phases)
	{
		Gameworld = gameworld;
		_phases.AddRange(phases);
	}

	#nullable enable
	public static IInventoryPlanAction? ParseActionFromBuilderInput(ICharacter actor, StringStack command)
	{
		DesiredItemState state;
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "held":
			case "hold":
				state = DesiredItemState.Held;
				break;
			case "wield":
			case "wielded":
				state = DesiredItemState.Wielded;
				break;
			case "wear":
			case "worn":
				state = DesiredItemState.Worn;
				break;
			case "attach":
			case "attached":
				state = DesiredItemState.Attached;
				break;
			case "sheath":
			case "sheathed":
				state = DesiredItemState.Sheathed;
				break;
			case "drop":
			case "dropped":
			case "inroom":
			case "room":
				state = DesiredItemState.InRoom;
				break;
			case "put":
			case "incontainer":
			case "container":
				state = DesiredItemState.InContainer;
				break;
			case "consume":
			case "consumed":
			case "use":
			case "used":
				state = DesiredItemState.Consumed;
				break;
			case "liquid":
			case "consumeliquid":
			case "consumedliquid":
				state = DesiredItemState.ConsumeLiquid;
				break;
			default:
				actor.OutputHandler.Send(
					$"That is not a valid action type. The valid options are {new List<string> { "held", "wielded", "worn", "attached", "sheathed", "inroom", "incontainer", "consumed", "consumedliquid" }.Select(x => x.ColourValue()).ListToString()}.");
				return null;
		}

		IInventoryPlanAction action;
		switch (state)
		{
			case DesiredItemState.InRoom:
			case DesiredItemState.Held:
			case DesiredItemState.Wielded:
			case DesiredItemState.Worn:
				if (command.IsFinished)
				{
					actor.OutputHandler.Send("You must specify a tag for the item you want to add to the plan.");
					return null;
				}

				var matchedtags = actor.Gameworld.Tags.FindMatchingTags(command.SafeRemainingArgument);
				if (matchedtags.Count == 0)
				{
					actor.OutputHandler.Send("There is no such tag.");
					return null;
				}

				if (matchedtags.Count > 1)
				{
					actor.OutputHandler.Send(
						$"Your text matched multiple tags. Please specify one of the following tags:\n\n{matchedtags.Select(x => $"\t[{x.Id.ToString("N0", actor)}] {x.FullName.ColourName()}").ListToLines()}");
					return null;
				}

				var tag = matchedtags.Single();

				switch (state)
				{
					case DesiredItemState.InRoom:
						action = new InventoryPlanActionDrop(actor.Gameworld, tag.Id, 0, null, null)
						{ ItemsAlreadyInPlaceOverrideFitnessScore = true };
						break;
					case DesiredItemState.Held:
						action = new InventoryPlanActionHold(actor.Gameworld, tag.Id, 0, null, null, 1)
						{ ItemsAlreadyInPlaceOverrideFitnessScore = true };
						break;
					case DesiredItemState.Wielded:
						action = new InventoryPlanActionWield(actor.Gameworld, tag.Id, 0, null, null)
						{ ItemsAlreadyInPlaceOverrideFitnessScore = true };
						break;
					case DesiredItemState.Worn:
						action = new InventoryPlanActionWear(actor.Gameworld, tag.Id, 0, null, null)
						{ ItemsAlreadyInPlaceOverrideFitnessScore = true };
						break;
					default:
						throw new ApplicationException("Application in unreachable state.");
				}

				break;
			case DesiredItemState.Consumed:
				if (command.IsFinished)
				{
					actor.OutputHandler.Send("You must specify a tag for the item you want to add to the plan.");
					return null;
				}

				matchedtags = actor.Gameworld.Tags.FindMatchingTags(command.PopSpeech());
				if (matchedtags.Count == 0)
				{
					actor.OutputHandler.Send("There is no such tag.");
					return null;
				}

				if (matchedtags.Count > 1)
				{
					actor.OutputHandler.Send(
						$"Your text matched multiple tags. Please specify one of the following tags:\n\n{matchedtags.Select(x => $"\t[{x.Id.ToString("N0", actor)}] {x.FullName.ColourName()}").ListToLines()}");
					return null;
				}

				tag = matchedtags.Single();

				if (command.IsFinished)
				{
					actor.OutputHandler.Send("You must enter a quantity of the items that you want to consume.");
					return null;
				}

				if (!int.TryParse(command.SafeRemainingArgument, out var value) || value < 1)
				{
					actor.OutputHandler.Send("You must enter a valid quantity of 1 or greater.");
					return null;
				}

				action = new InventoryPlanActionConsume(actor.Gameworld, value, tag.Id, 0, null, null)
				{ ItemsAlreadyInPlaceOverrideFitnessScore = true };
				break;
			case DesiredItemState.InContainer:
			case DesiredItemState.Sheathed:
			case DesiredItemState.Attached:
				if (command.IsFinished)
				{
					actor.OutputHandler.Send("You must specify a tag for the item you want to add to the plan.");
					return null;
				}

				matchedtags = actor.Gameworld.Tags.FindMatchingTags(command.PopSpeech());
				if (matchedtags.Count == 0)
				{
					actor.OutputHandler.Send("There is no such tag.");
					return null;
				}

				if (matchedtags.Count > 1)
				{
					actor.OutputHandler.Send(
						$"Your text matched multiple tags. Please specify one of the following tags:\n\n{matchedtags.Select(x => $"\t[{x.Id.ToString("N0", actor)}] {x.FullName.ColourName()}").ListToLines()}");
					return null;
				}

				tag = matchedtags.Single();

				ITag secondTag = null;
				if (!command.IsFinished)
				{
					matchedtags = actor.Gameworld.Tags.FindMatchingTags(command.SafeRemainingArgument);
					if (matchedtags.Count == 0)
					{
						actor.OutputHandler.Send("There is no such tag (second argument).");
						return null;
					}

					if (matchedtags.Count > 1)
					{
						actor.OutputHandler.Send(
							$"Your text matched multiple tags (second argument). Please specify one of the following tags:\n\n{matchedtags.Select(x => $"\t[{x.Id.ToString("N0", actor)}] {x.FullName.ColourName()}").ListToLines()}");
						return null;
					}

					secondTag = matchedtags.Single();
				}

				switch (state)
				{
					case DesiredItemState.InContainer:
						action = new InventoryPlanActionPut(actor.Gameworld, tag.Id, secondTag?.Id ?? 0, null, null)
						{ ItemsAlreadyInPlaceOverrideFitnessScore = true };
						break;
					case DesiredItemState.Sheathed:
						action = new InventoryPlanActionSheath(actor.Gameworld, tag.Id, secondTag?.Id ?? 0, null, null)
						{ ItemsAlreadyInPlaceOverrideFitnessScore = true };
						break;
					case DesiredItemState.Attached:
						action = new InventoryPlanActionAttach(actor.Gameworld, tag.Id, secondTag?.Id ?? 0, null, null)
						{ ItemsAlreadyInPlaceOverrideFitnessScore = true };
						break;
					default:
						throw new ApplicationException("Application in unreachable state.");
				}

				break;
			case DesiredItemState.ConsumeLiquid:
				if (command.IsFinished)
				{
					actor.OutputHandler.Send("Which liquid do you want this plan to consume?");
					return null;
				}

				var liquid = actor.Gameworld.Liquids.GetByIdOrName(command.PopSpeech());
				if (liquid is null)
				{
					actor.OutputHandler.Send("There is no such liquid.");
					return null;
				}

				if (command.IsFinished)
				{
					actor.OutputHandler.Send($"How much {liquid.Name.Colour(liquid.DisplayColour)} should this plan consume?");
					return null;
				}

				if (!actor.Gameworld.UnitManager.TryGetBaseUnits(command.PopSpeech(), UnitType.FluidVolume, actor,
						out var liquidAmount))
				{
					actor.OutputHandler.Send($"The text {command.Last.ColourCommand()} is not a valid fluid quantity.");
				}

				secondTag = null;
				if (!command.IsFinished)
				{
					matchedtags = actor.Gameworld.Tags.FindMatchingTags(command.SafeRemainingArgument);
					if (matchedtags.Count == 0)
					{
						actor.OutputHandler.Send("There is no such tag (second argument).");
						return null;
					}

					if (matchedtags.Count > 1)
					{
						actor.OutputHandler.Send(
							$"Your text matched multiple tags (second argument). Please specify one of the following tags:\n\n{matchedtags.Select(x => $"\t[{x.Id.ToString("N0", actor)}] {x.FullName.ColourName()}").ListToLines()}");
						return null;
					}

					secondTag = matchedtags.Single();
				}

				action = new InventoryPlanActionConsumeLiquid(actor.Gameworld, 0, secondTag?.Id ?? 0, item => true, null,
					mixture => mixture.CountsAs(liquid).Truth,
					new LiquidMixture(liquid, liquidAmount, actor.Gameworld));
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		return action;
	}
#nullable restore
	public IFuturemud Gameworld { get; set; }

	public InventoryPlanOptions Options { get; set; }

	public IEnumerable<IInventoryPlanPhaseTemplate> Phases => _phases;

	public IInventoryPlanPhaseTemplate FirstPhase => _phases.First();

	public IInventoryPlan CreatePlan(ICharacter actor)
	{
		return new InventoryPlan(actor, this);
	}

	public void FinalisePlan(ICharacter executor, bool restore, IInventoryPlan plan, IList<IGameItem> exemptItems)
	{
		if (restore)
		{
			var items = (exemptItems != null
				? plan.AssociatedEffects.Where(x => !exemptItems.Contains(x.TargetItem))
				: plan.AssociatedEffects).ToList();

			//Do restores on things that get items out of our hands first
			foreach (var item in items.Where(x => x.DesiredState != DesiredItemState.Held &&
			                                      x.DesiredState != DesiredItemState.WieldedOneHandedOnly &&
			                                      x.DesiredState != DesiredItemState.WieldedTwoHandedOnly &&
			                                      x.DesiredState != DesiredItemState.Wielded &&
			                                      x.DesiredState != DesiredItemState.Worn))
			{
				if (item.TargetItem.TrueLocations.All(x => x != executor.Location) ||
				    item.TargetItem.Destroyed)
				{
					continue;
				}

				TakeAction(executor, item.TargetItem, item.SecondaryItem, item.DesiredState, null, false);
			}

			//Now items to wear
			foreach (var item in items.Where(x => x.DesiredState == DesiredItemState.Worn).ToList())
			{
				if (item.TargetItem.TrueLocations.All(x => x != executor.Location) ||
				    item.TargetItem.Destroyed)
				{
					continue;
				}

				TakeAction(executor, item.TargetItem, item.SecondaryItem, item.DesiredState, null, false);
			}

			//Now items to wield or hold
			foreach (var item in items.Where(x => x.DesiredState == DesiredItemState.Held
			                                      || x.DesiredState == DesiredItemState.Wielded ||
			                                      x.DesiredState == DesiredItemState.WieldedOneHandedOnly ||
			                                      x.DesiredState == DesiredItemState.WieldedTwoHandedOnly).ToList())
			{
				if (item.TargetItem.TrueLocations.All(x => x != executor.Location) ||
				    item.TargetItem.Destroyed)
				{
					continue;
				}

				TakeAction(executor, item.TargetItem, item.SecondaryItem, item.DesiredState, null, false);
			}
		}

		executor.RemoveAllEffects(x => x.GetSubtype<IInventoryPlanItemEffect>()?.Plan == plan);
	}

	public IEnumerable<InventoryPlanActionResult> PeekPlanResults(ICharacter actor, IInventoryPlanPhase phase,
		IInventoryPlan plan)
	{
		var results = new List<InventoryPlanActionResult>();
		var targets = phase.ScoutedItems;

		var numItemsRequiringHoldLocs =
			targets
				.Where(x => x.Action.DesiredState == DesiredItemState.Held)
				.Count(x => !actor.Body.HeldOrWieldedItems.Contains(x.Primary));

		var numItemsRequiringWieldLocs =
			targets
				.Where(x => x.Action.DesiredState.In(DesiredItemState.Wielded, DesiredItemState.WieldedOneHandedOnly,
					DesiredItemState.WieldedTwoHandedOnly))
				.Count(x => !actor.Body.HeldOrWieldedItems.Contains(x.Primary));

		var numAvailWieldingLocs = actor.Body.WieldLocs
		                                .Where(x => actor.Body.HoldLocs.All(y => y != x) &&
		                                            actor.Body.CanUseBodypart(x) == CanUseBodypartResult.CanUse)
		                                .Count(x => !actor.Body.WieldedItemsFor(x).Any());

		var numAvailHoldingWieldLocs =
			actor.Body.WieldLocs
			     .Where(x => actor.Body.HoldLocs.Any(y => y == x) &&
			                 actor.Body.CanUseBodypart(x) == CanUseBodypartResult.CanUse).Count(x =>
				     !actor.Body.WieldedItemsFor(x).Any() && !actor.Body.HeldItemsFor(x).Any());


		var numAvailHoldingLocs =
			actor.Body.HoldLocs
			     .Where(x => actor.Body.WieldLocs.All(y => y != x) &&
			                 actor.Body.CanUseBodypart(x) == CanUseBodypartResult.CanUse)
			     .Count(x => !actor.Body.HeldItemsFor(x).Any());

		if (numItemsRequiringWieldLocs > numAvailHoldingWieldLocs + numAvailWieldingLocs)
		{
			var count = numAvailWieldingLocs + numAvailHoldingWieldLocs;
			foreach (var item in actor.Body.HeldOrWieldedItems.Where(x => targets.All(y => y.Primary != x)).ToList())
			{
				var wieldLoc = actor.Body.HoldOrWieldLocFor(item);
				if (wieldLoc == null || !actor.Body.WieldLocs.Contains(wieldLoc))
				{
					continue;
				}

				var foundSheath = false;
				foreach (var sheath in actor.Inventory.SelectNotNull(x => x.GetItemType<ISheath>()))
				{
					if (actor.Body.CanSheathe(item, sheath.Parent))
					{
						results.Add(new InventoryPlanActionResult
						{
							ActionState = DesiredItemState.Sheathed,
							OriginalReference = "clearedhands",
							PrimaryTarget = item,
							SecondaryTarget = sheath.Parent
						});
						foundSheath = true;
						break;
					}
				}

				var foundContainer = false;
				if (foundSheath == false)
				{
					foreach (var container in actor.Inventory.SelectNotNull(x => x.GetItemType<IContainer>()))
					{
						if (actor.Body.CanPut(item, container.Parent, null, 0, false))
						{
							results.Add(new InventoryPlanActionResult
							{
								ActionState = DesiredItemState.InContainer,
								OriginalReference = "clearedhands",
								PrimaryTarget = item,
								SecondaryTarget = container.Parent
							});
							foundContainer = true;
							break;
						}
					}
				}

				if (foundContainer == false && foundSheath == false)
				{
					results.Add(new InventoryPlanActionResult
					{
						ActionState = DesiredItemState.InRoom,
						OriginalReference = "clearedhands",
						PrimaryTarget = item
					});
				}

				count++;
				if (count >= numItemsRequiringWieldLocs)
				{
					break;
				}
			}
		}

		if (numItemsRequiringHoldLocs > numAvailHoldingWieldLocs + numAvailHoldingLocs)
		{
			var count = numAvailHoldingLocs + numAvailHoldingWieldLocs;
			foreach (var item in actor.Body.HeldOrWieldedItems.Where(x => targets.All(y => y.Primary != x)).ToList())
			{
				var holdLoc = actor.Body.HoldOrWieldLocFor(item);
				if (holdLoc == null || !actor.Body.HoldLocs.Contains(holdLoc))
				{
					continue;
				}

				var foundSheath = false;
				foreach (var sheath in actor.Inventory.SelectNotNull(x => x.GetItemType<ISheath>()))
				{
					if (actor.Body.CanSheathe(item, sheath.Parent))
					{
						results.Add(new InventoryPlanActionResult
						{
							ActionState = DesiredItemState.Sheathed,
							OriginalReference = "clearedhands",
							PrimaryTarget = item,
							SecondaryTarget = sheath.Parent
						});
						foundSheath = true;
						break;
					}
				}

				var foundContainer = false;
				if (foundSheath == false)
				{
					foreach (var container in actor.Inventory.SelectNotNull(x => x.GetItemType<IContainer>()))
					{
						if (actor.Body.CanPut(item, container.Parent, null, 0, false))
						{
							results.Add(new InventoryPlanActionResult
							{
								ActionState = DesiredItemState.InContainer,
								OriginalReference = "clearedhands",
								PrimaryTarget = item,
								SecondaryTarget = container.Parent
							});
							foundContainer = true;
							break;
						}
					}
				}

				if (foundContainer == false && foundSheath == false)
				{
					results.Add(new InventoryPlanActionResult
					{
						ActionState = DesiredItemState.InRoom,
						OriginalReference = "clearedhands",
						PrimaryTarget = item
					});
				}

				count++;
				if (count >= numItemsRequiringHoldLocs)
				{
					break;
				}
			}
		}

		// in room actions first
		foreach (var target in targets.Where(x => x.Action.DesiredState == DesiredItemState.InRoom).ToList())
		{
			results.Add(new InventoryPlanActionResult
			{
				ActionState = DesiredItemState.InRoom,
				OriginalReference = target.Action.OriginalReference,
				PrimaryTarget = target.Primary
			});
		}

		// containers, sheathes, attach, wear
		foreach (
			var target in
			targets.Where(
				x =>
					x.Action.DesiredState == DesiredItemState.Sheathed ||
					x.Action.DesiredState == DesiredItemState.InContainer ||
					x.Action.DesiredState == DesiredItemState.Attached ||
					x.Action.DesiredState == DesiredItemState.Worn).ToList())
		{
			results.Add(new InventoryPlanActionResult
			{
				ActionState = target.Action.DesiredState,
				OriginalReference = target.Action.OriginalReference,
				PrimaryTarget = target.Primary,
				SecondaryTarget = target.Secondary
			});
		}

		// wielded penultimate in case not using wieldinggrabbing parts
		foreach (var target in targets.Where(x =>
			         x.Action.DesiredState == DesiredItemState.Wielded ||
			         x.Action.DesiredState == DesiredItemState.WieldedOneHandedOnly ||
			         x.Action.DesiredState == DesiredItemState.WieldedTwoHandedOnly).ToList())
		{
			results.Add(new InventoryPlanActionResult
			{
				ActionState = target.Action.DesiredState,
				OriginalReference = target.Action.OriginalReference,
				PrimaryTarget = target.Primary
			});
		}

		// held items next to last
		foreach (var target in targets.Where(x => x.Action.DesiredState == DesiredItemState.Held).ToList())
		{
			results.Add(new InventoryPlanActionResult
			{
				ActionState = target.Action.DesiredState,
				OriginalReference = target.Action.OriginalReference,
				PrimaryTarget = target.Primary
			});
		}

		// consumed items last of all
		foreach (var target in targets.Where(x =>
			         x.Action.DesiredState == DesiredItemState.Consumed ||
			         x.Action.DesiredState == DesiredItemState.ConsumeLiquid).ToList())
		{
			results.Add(new InventoryPlanActionResult
			{
				ActionState = target.Action.DesiredState,
				OriginalReference = target.Action.OriginalReference,
				PrimaryTarget = target.Primary
			});
		}

		return results;
	}

	public IEnumerable<InventoryPlanActionResult> ExecutePhase(ICharacter actor, IInventoryPlanPhase phase,
		IInventoryPlan plan)
	{
		var results = new List<InventoryPlanActionResult>();
		var targets = phase.ScoutedItems.Where(x => x.Primary != null).ToList();
		if (!targets.Any())
		{
			return results;
		}

		var numItemsRequiringHoldLocs =
			targets
				.Where(x => x.Action.DesiredState == DesiredItemState.Held)
				.Count(x => !actor.Body.HeldOrWieldedItems.Contains(x.Primary));

		var numItemsRequiringWieldLocs =
			targets
				.Where(x => x.Action.DesiredState.In(DesiredItemState.Wielded, DesiredItemState.WieldedOneHandedOnly,
					DesiredItemState.WieldedTwoHandedOnly))
				.Count(x => !actor.Body.HeldOrWieldedItems.Contains(x.Primary));

		var numAvailWieldingLocs = actor.Body.WieldLocs
		                                .Where(x => actor.Body.HoldLocs.All(y => y != x) &&
		                                            actor.Body.CanUseBodypart(x) == CanUseBodypartResult.CanUse)
		                                .Count(x => !actor.Body.WieldedItemsFor(x).Any());

		var numAvailHoldingWieldLocs =
			actor.Body.WieldLocs
			     .Where(x => actor.Body.HoldLocs.Any(y => y == x) &&
			                 actor.Body.CanUseBodypart(x) == CanUseBodypartResult.CanUse).Count(x =>
				     !actor.Body.WieldedItemsFor(x).Any() && !actor.Body.HeldItemsFor(x).Any());


		var numAvailHoldingLocs =
			actor.Body.HoldLocs
			     .Where(x => actor.Body.WieldLocs.All(y => y != x) &&
			                 actor.Body.CanUseBodypart(x) == CanUseBodypartResult.CanUse)
			     .Count(x => !actor.Body.HeldItemsFor(x).Any());

		if (numItemsRequiringWieldLocs > numAvailHoldingWieldLocs + numAvailWieldingLocs)
		{
			var count = numAvailWieldingLocs + numAvailHoldingWieldLocs;
			foreach (var item in actor.Body.HeldOrWieldedItems.Where(x => targets.All(y => y.Primary != x)).ToList())
			{
				var wieldLoc = actor.Body.HoldOrWieldLocFor(item);
				if (wieldLoc == null || !actor.Body.WieldLocs.Contains(wieldLoc))
				{
					continue;
				}

				var targetEffect =
					actor.EffectsOfType<IInventoryPlanItemEffect>().FirstOrDefault(x => x.TargetItem == item);
				if (targetEffect != null)
				{
					results.Add(TakeAction(actor, item, targetEffect.SecondaryItem, targetEffect.DesiredState, null,
						false));
					actor.RemoveEffect(targetEffect);
				}

				MarkItemForRestoration(actor, item, plan);

				var foundSheath = false;
				foreach (var sheath in actor.Inventory.SelectNotNull(x => x.GetItemType<ISheath>()))
				{
					if (actor.Body.CanSheathe(item, sheath.Parent))
					{
						results.Add(TakeAction(actor, item, sheath.Parent, DesiredItemState.Sheathed, null, false));
						foundSheath = true;
						break;
					}
				}

				var foundContainer = false;
				if (foundSheath == false)
				{
					foreach (var container in actor.Inventory.SelectNotNull(x => x.GetItemType<IContainer>()))
					{
						if (actor.Body.CanPut(item, container.Parent, null, 0, false))
						{
							results.Add(TakeAction(actor, item, container.Parent, DesiredItemState.InContainer, null,
								false));
							foundContainer = true;
							break;
						}
					}
				}

				if (foundContainer == false && foundSheath == false)
				{
					results.Add(TakeAction(actor, item, null, DesiredItemState.InRoom, null, false));
				}

				count++;
				if (count >= numItemsRequiringWieldLocs)
				{
					break;
				}
			}
		}

		if (numItemsRequiringHoldLocs > numAvailHoldingWieldLocs + numAvailHoldingLocs)
		{
			var count = numAvailHoldingLocs + numAvailHoldingWieldLocs;
			foreach (var item in actor.Body.HeldOrWieldedItems.Where(x => targets.All(y => y.Primary != x)).ToList())
			{
				var holdLoc = actor.Body.HoldOrWieldLocFor(item);
				if (holdLoc == null || !actor.Body.HoldLocs.Contains(holdLoc))
				{
					continue;
				}

				var targetEffect =
					actor.EffectsOfType<IInventoryPlanItemEffect>().FirstOrDefault(x => x.TargetItem == item);
				if (targetEffect != null)
				{
					results.Add(TakeAction(actor, item, targetEffect.SecondaryItem, targetEffect.DesiredState, null,
						false));
					actor.RemoveEffect(targetEffect);
				}

				MarkItemForRestoration(actor, item, plan);

				var foundSheath = false;
				foreach (var sheath in actor.Inventory.SelectNotNull(x => x.GetItemType<ISheath>()))
				{
					if (actor.Body.CanSheathe(item, sheath.Parent))
					{
						results.Add(TakeAction(actor, item, sheath.Parent, DesiredItemState.Sheathed, null, false));
						foundSheath = true;
						break;
					}
				}

				var foundContainer = false;
				if (foundSheath == false)
				{
					foreach (var container in actor.Inventory.SelectNotNull(x => x.GetItemType<IContainer>()))
					{
						if (actor.Body.CanPut(item, container.Parent, null, 0, false))
						{
							results.Add(TakeAction(actor, item, container.Parent, DesiredItemState.InContainer, null,
								false));
							foundContainer = true;
							break;
						}
					}
				}

				if (foundContainer == false && foundSheath == false)
				{
					results.Add(TakeAction(actor, item, null, DesiredItemState.InRoom, null, false));
				}

				count++;
				if (count >= numItemsRequiringHoldLocs)
				{
					break;
				}
			}
		}

		// in room actions first
		foreach (var target in targets.Where(x => x.Action.DesiredState == DesiredItemState.InRoom).ToList())
		{
			MarkItemForRestoration(actor, target.Primary, plan);
			results.Add(TakeAction(actor, target.Primary, null, DesiredItemState.InRoom, target.Action, false));
		}

		// containers, sheathes, attach, wear
		foreach (
			var target in
			targets.Where(
				x =>
					x.Action.DesiredState == DesiredItemState.Sheathed ||
					x.Action.DesiredState == DesiredItemState.InContainer ||
					x.Action.DesiredState == DesiredItemState.Attached ||
					x.Action.DesiredState == DesiredItemState.Worn).ToList())
		{
			MarkItemForRestoration(actor, target.Primary, plan);
			results.Add(TakeAction(actor, target.Primary, target.Secondary, target.Action.DesiredState, target.Action,
				false));
		}

		// wielded penultimate in case not using wieldinggrabbing parts
		foreach (var target in targets.Where(x =>
			         x.Action.DesiredState == DesiredItemState.Wielded ||
			         x.Action.DesiredState == DesiredItemState.WieldedOneHandedOnly ||
			         x.Action.DesiredState == DesiredItemState.WieldedTwoHandedOnly).ToList())
		{
			MarkItemForRestoration(actor, target.Primary, plan);
			results.Add(TakeAction(actor, target.Primary, null, target.Action.DesiredState, target.Action, false));
		}

		// held items next to last
		foreach (var target in targets.Where(x => x.Action.DesiredState == DesiredItemState.Held).ToList())
		{
			MarkItemForRestoration(actor, target.Primary, plan);
			results.Add(TakeAction(actor, target.Primary, null, DesiredItemState.Held, target.Action, false));
		}

		// consumed items last of all
		foreach (var target in targets.Where(x =>
			         x.Action.DesiredState == DesiredItemState.Consumed ||
			         x.Action.DesiredState == DesiredItemState.ConsumeLiquid).ToList())
		{
			results.Add(TakeAction(actor, target.Primary, null, target.Action.DesiredState, target.Action, false));
		}

		return results;
	}

	public IEnumerable<(IInventoryPlanAction Action, InventoryPlanFeasibility Reason)> InfeasibleActions(
		ICharacter actor, IInventoryPlanPhase phase)
	{
		var actions = new List<(IInventoryPlanAction Action, InventoryPlanFeasibility Reason)>();
		var targets = phase.ScoutedItems;

		var wieldingLocs =
			actor.Body.WieldLocs.Where(
				x =>
					actor.Body.HoldLocs.All(y => y != x) &&
					actor.Body.CanUseBodypart(x) == CanUseBodypartResult.CanUse).ToList();
		var holdingWieldingLocs =
			actor.Body.WieldLocs.Where(
				x =>
					actor.Body.HoldLocs.Any(y => y == x) &&
					actor.Body.CanUseBodypart(x) == CanUseBodypartResult.CanUse).ToList();
		var holdingLocs =
			actor.Body.HoldLocs.Where(
				x =>
					actor.Body.WieldLocs.All(y => y != x) &&
					actor.Body.CanUseBodypart(x) == CanUseBodypartResult.CanUse).ToList();

		foreach (var target in targets)
		{
			if (target.Primary == null)
			{
				actions.Add((target.Action, InventoryPlanFeasibility.NotFeasibleMissingItems));
				continue;
			}

			if (target.Action.DesiredState == DesiredItemState.Wielded ||
			    target.Action.DesiredState == DesiredItemState.WieldedOneHandedOnly ||
			    target.Action.DesiredState == DesiredItemState.WieldedTwoHandedOnly)
			{
				if (!wieldingLocs.Any() && !holdingWieldingLocs.Any())
				{
					actions.Add((target.Action, InventoryPlanFeasibility.NotFeasibleNotEnoughWielders));
					continue;
				}

				var minHands = wieldingLocs.Concat(holdingWieldingLocs).FirstMin(x => x.Hands(target.Primary));
				if (minHands.Hands(target.Primary) == 1)
				{
					wieldingLocs.Remove(minHands);
					holdingWieldingLocs.Remove(minHands);
				}
				else
				{
					wieldingLocs.Remove(minHands);
					holdingWieldingLocs.Remove(minHands);
					for (var i = 0; i < minHands.Hands(target.Primary) - 1; i++)
					{
						if (wieldingLocs.Any())
						{
							wieldingLocs.RemoveAt(0);
							continue;
						}

						if (holdingWieldingLocs.Any())
						{
							holdingWieldingLocs.RemoveAt(0);
							continue;
						}

						actions.Add((target.Action, InventoryPlanFeasibility.NotFeasibleNotEnoughHands));
						continue;
					}
				}
			}

			if (target.Action.DesiredState == DesiredItemState.Held)
			{
				if (!holdingLocs.Any() && !holdingWieldingLocs.Any())
				{
					actions.Add((target.Action, InventoryPlanFeasibility.NotFeasibleNotEnoughHands));
					continue;
				}

				if (holdingLocs.Any())
				{
					holdingLocs.RemoveAt(0);
					continue;
				}

				holdingWieldingLocs.RemoveAt(0);
			}

			if (target.Action.DesiredState == DesiredItemState.Attached)
			{
				if (target.Secondary == null)
				{
					actions.Add((target.Action, InventoryPlanFeasibility.NotFeasibleMissingItems));
					continue;
				}
			}

			if (target.Action.DesiredState == DesiredItemState.InContainer)
			{
				if (target.Secondary == null)
				{
					actions.Add((target.Action, InventoryPlanFeasibility.NotFeasibleMissingItems));
					continue;
				}
			}

			if (target.Action.DesiredState == DesiredItemState.Sheathed)
			{
				if (target.Secondary == null)
				{
					actions.Add((target.Action, InventoryPlanFeasibility.NotFeasibleMissingItems));
					continue;
				}
			}
		}

		return actions;
	}

	public InventoryPlanFeasibility PlanIsFeasible(ICharacter actor, IInventoryPlanPhase phase)
	{
		var targets = phase.ScoutedItems;

		if (targets.Any(x => x.Primary == null))
		{
			return InventoryPlanFeasibility.NotFeasibleMissingItems;
		}

		var wieldingLocs =
			actor.Body.WieldLocs.Where(
				x =>
					actor.Body.HoldLocs.All(y => y != x) &&
					actor.Body.CanUseBodypart(x) == CanUseBodypartResult.CanUse).ToList();
		var holdingWieldingLocs =
			actor.Body.WieldLocs.Where(
				x =>
					actor.Body.HoldLocs.Any(y => y == x) &&
					actor.Body.CanUseBodypart(x) == CanUseBodypartResult.CanUse).ToList();
		var holdingLocs =
			actor.Body.HoldLocs.Where(
				x =>
					actor.Body.WieldLocs.All(y => y != x) &&
					actor.Body.CanUseBodypart(x) == CanUseBodypartResult.CanUse).ToList();

		foreach (var item in targets.Where(x =>
			         x.Action.DesiredState == DesiredItemState.Wielded ||
			         x.Action.DesiredState == DesiredItemState.WieldedOneHandedOnly ||
			         x.Action.DesiredState == DesiredItemState.WieldedTwoHandedOnly))
		{
			if (!wieldingLocs.Any() && !holdingWieldingLocs.Any())
			{
				return InventoryPlanFeasibility.NotFeasibleNotEnoughWielders;
			}

			var minHands = wieldingLocs.Concat(holdingWieldingLocs).FirstMin(x => x.Hands(item.Primary));
			if (minHands.Hands(item.Primary) == 1)
			{
				wieldingLocs.Remove(minHands);
				holdingWieldingLocs.Remove(minHands);
			}
			else
			{
				wieldingLocs.Remove(minHands);
				holdingWieldingLocs.Remove(minHands);
				for (var i = 0; i < minHands.Hands(item.Primary) - 1; i++)
				{
					if (wieldingLocs.Any())
					{
						wieldingLocs.RemoveAt(0);
						continue;
					}

					if (holdingWieldingLocs.Any())
					{
						holdingWieldingLocs.RemoveAt(0);
						continue;
					}

					return InventoryPlanFeasibility.NotFeasibleNotEnoughWielders;
				}
			}
		}

		foreach (var item in targets.Where(x => x.Action.DesiredState == DesiredItemState.Held))
		{
			if (!holdingLocs.Any() && !holdingWieldingLocs.Any())
			{
				return InventoryPlanFeasibility.NotFeasibleNotEnoughHands;
			}

			if (holdingLocs.Any())
			{
				holdingLocs.RemoveAt(0);
				continue;
			}

			holdingWieldingLocs.RemoveAt(0);
		}

		foreach (var item in targets.Where(x => x.Action.DesiredState == DesiredItemState.Attached))
		{
			if (item.Secondary == null)
			{
				return InventoryPlanFeasibility.NotFeasibleMissingItems;
			}
		}

		foreach (var item in targets.Where(x => x.Action.DesiredState == DesiredItemState.InContainer))
		{
			if (item.Secondary == null)
			{
				return InventoryPlanFeasibility.NotFeasibleMissingItems;
			}
		}

		foreach (var item in targets.Where(x => x.Action.DesiredState == DesiredItemState.Sheathed))
		{
			if (item.Secondary == null)
			{
				return InventoryPlanFeasibility.NotFeasibleMissingItems;
			}
		}

		return InventoryPlanFeasibility.Feasible;
	}

	private void MarkItemForRestoration(ICharacter actor, IGameItem item, IInventoryPlan plan)
	{
		if (actor == null)
		{
			throw new ArgumentNullException(nameof(actor));
		}

		if (item == null)
		{
			throw new ArgumentNullException(nameof(item));
		}

		if (plan == null)
		{
			throw new ArgumentNullException(nameof(plan));
		}

		if (actor.EffectsOfType<IInventoryPlanItemEffect>().Any(x => x.TargetItem == item))
		{
			return;
		}

		if (item.ContainedIn?.IsItemType<ISheath>() ?? false)
		{
			actor.AddEffect(
				new InventoryPlanItemEffect(actor, plan)
				{
					TargetItem = item,
					DesiredState = DesiredItemState.Sheathed,
					SecondaryItem = item.ContainedIn
				}, TimeSpan.FromSeconds(1000));
			return;
		}

		var beltable = item.GetItemType<IBeltable>();
		if (beltable?.ConnectedTo != null)
		{
			actor.AddEffect(
				new InventoryPlanItemEffect(actor, plan)
				{
					TargetItem = item,
					DesiredState = DesiredItemState.Attached,
					SecondaryItem = beltable.ConnectedTo.Parent
				}, TimeSpan.FromSeconds(1000));
			return;
		}

		if (item.ContainedIn != null)
		{
			actor.AddEffect(
				new InventoryPlanItemEffect(actor, plan)
				{
					TargetItem = item,
					DesiredState = DesiredItemState.InContainer,
					SecondaryItem = item.ContainedIn
				}, TimeSpan.FromSeconds(1000));
			return;
		}

		if (actor.Body.WornItems.Contains(item))
		{
			actor.AddEffect(
				new InventoryPlanItemEffect(actor, plan) { TargetItem = item, DesiredState = DesiredItemState.Worn },
				TimeSpan.FromSeconds(1000));
			return;
		}

		if (actor.Body.WieldedItems.Contains(item))
		{
			actor.AddEffect(
				new InventoryPlanItemEffect(actor, plan)
				{
					TargetItem = item,
					DesiredState = actor.Body.WieldedHandCount(item) == 1
						? DesiredItemState.WieldedOneHandedOnly
						: DesiredItemState.WieldedTwoHandedOnly
				},
				TimeSpan.FromSeconds(1000));
			return;
		}

		if (item.InInventoryOf == null)
		{
			actor.AddEffect(
				new InventoryPlanItemEffect(actor, plan) { TargetItem = item, DesiredState = DesiredItemState.InRoom },
				TimeSpan.FromSeconds(1000));
			return;
		}

		actor.AddEffect(
			new InventoryPlanItemEffect(actor, plan) { TargetItem = item, DesiredState = DesiredItemState.Held },
			TimeSpan.FromSeconds(1000));
	}

	#region TakeAction and Subroutines

	private InventoryPlanActionResult PeekTakeAction(ICharacter actor, IGameItem item, IGameItem target,
		DesiredItemState state, IInventoryPlanAction action)
	{
		throw new NotImplementedException();
	}

	private InventoryPlanActionResult TakeAction(ICharacter actor, IGameItem item, IGameItem target,
		DesiredItemState state,
		IInventoryPlanAction action, bool silent)
	{
		switch (state)
		{
			case DesiredItemState.Held:
				return GetItem(actor, item, action, silent);
			case DesiredItemState.InContainer:
				return PutItemContainer(actor, item, target, silent, action?.OriginalReference);
			case DesiredItemState.InRoom:
				return DropItem(actor, item, silent, action?.OriginalReference);
			case DesiredItemState.Wielded:
				return WieldItem(actor, item, silent, action?.OriginalReference, AttackHandednessOptions.Any);
			case DesiredItemState.WieldedOneHandedOnly:
				return WieldItem(actor, item, silent, action?.OriginalReference, AttackHandednessOptions.OneHandedOnly);
			case DesiredItemState.WieldedTwoHandedOnly:
				return WieldItem(actor, item, silent, action?.OriginalReference, AttackHandednessOptions.TwoHandedOnly);
			case DesiredItemState.Worn:
				return WearItem(actor, item, silent, action?.OriginalReference,
					((InventoryPlanActionWear)action).DesiredProfile);
			case DesiredItemState.Sheathed:
				return SheatheItem(actor, item, target, silent, action?.OriginalReference);
			case DesiredItemState.Attached:
				return AttachItem(actor, item, target, silent, action?.OriginalReference);
			case DesiredItemState.ConsumeCommodity:
				return ConsumeCommodity(actor, item, ((InventoryPlanActionConsumeCommodity)action).Weight, action?.OriginalReference);
			case DesiredItemState.Consumed:
				return ConsumeItem(actor, item, ((InventoryPlanActionConsume)action).Quantity,
					action?.OriginalReference);
			case DesiredItemState.ConsumeLiquid:
				return ConsumeLiquid(actor, item, (InventoryPlanActionConsumeLiquid)action, action?.OriginalReference);
			case DesiredItemState.Unknown:
				return new InventoryPlanActionResult
				{
					PrimaryTarget = item,
					SecondaryTarget = target,
					ActionState = DesiredItemState.Unknown,
					OriginalReference = action?.OriginalReference
				};
			default:
				throw new ApplicationException("Unknown DesiredItemState in InventoryPlanTemplate.TakeAction");
		}
	}

	private InventoryPlanActionResult AttachItem(ICharacter actor, IGameItem item, IGameItem target, bool silent,
		object originalReference)
	{
		if (!actor.Location.LayerGameItems(actor.RoomLayer).Contains(target) && !actor.Inventory.Contains(target))
		{
			return new InventoryPlanActionResult
			{
				PrimaryTarget = item,
				SecondaryTarget = target,
				ActionState = DesiredItemState.Attached,
				OriginalReference = originalReference
			};
		}

		GetItem(actor, item, null, silent);
		actor.Body.Take(item);
		target.GetItemType<IBelt>().AddConnectedItem(item.GetItemType<IBeltable>());
		if (!silent)
		{
			actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ attach|attaches $0 to $1.", actor, item, target)));
		}

		return new InventoryPlanActionResult
		{
			PrimaryTarget = item,
			SecondaryTarget = target,
			ActionState = DesiredItemState.Attached,
			OriginalReference = originalReference
		};
	}

	private InventoryPlanActionResult GetItem(ICharacter actor, IGameItem item, IInventoryPlanAction action,
		bool silent, bool wieldOK = false)
	{
		var originalReference = action?.OriginalReference;
		var quantity = (action as InventoryPlanActionHold)?.Quantity ?? 0;
		if (actor.Body.HeldItems.Contains(item))
		{
			return new InventoryPlanActionResult
			{
				PrimaryTarget = item,
				ActionState = DesiredItemState.Held,
				OriginalReference = originalReference
			};
		}

		if (actor.Body.WieldedItems.Contains(item))
		{
			if (wieldOK)
			{
				return new InventoryPlanActionResult
				{
					PrimaryTarget = item,
					ActionState = DesiredItemState.Wielded,
					OriginalReference = originalReference
				};
			}

			actor.Body.Unwield(item, silent: silent);
			return new InventoryPlanActionResult
			{
				PrimaryTarget = item,
				ActionState = DesiredItemState.Held,
				OriginalReference = originalReference
			};
		}

		if (actor.Body.WornItems.Contains(item))
		{
			if (actor.Body.CanRemoveItem(item, ItemCanGetIgnore.IgnoreWeight))
			{
				actor.Body.RemoveItem(item, null, silent, ItemCanGetIgnore.IgnoreWeight);
				return new InventoryPlanActionResult
				{
					PrimaryTarget = item,
					ActionState = DesiredItemState.Held,
					OriginalReference = originalReference
				};
			}
		}

		if (actor.Location.LayerGameItems(actor.RoomLayer).Contains(item) &&
		    actor.Body.CanGet(item, quantity, ItemCanGetIgnore.IgnoreInventoryPlans))
		{
			actor.Body.Get(item, quantity, silent: silent, ignoreFlags: ItemCanGetIgnore.IgnoreInventoryPlans);
			return new InventoryPlanActionResult
			{
				PrimaryTarget = item,
				ActionState = DesiredItemState.Held,
				OriginalReference = originalReference
			};
		}

		var container =
			actor.Location.LayerGameItems(actor.RoomLayer).SelectNotNull(x => x.GetItemType<IContainer>())
			     .FirstOrDefault(x =>
				     x.Contents.Contains(item) &&
				     actor.Body.CanGet(item, x.Parent, quantity, ItemCanGetIgnore.IgnoreInventoryPlans)) ??
			actor.Inventory.SelectNotNull(x => x.GetItemType<IContainer>())
			     .FirstOrDefault(x =>
				     x.Contents.Contains(item) &&
				     actor.Body.CanGet(item, x.Parent, quantity, ItemCanGetIgnore.IgnoreInventoryPlans));

		if (container != null)
		{
			var openable = container.Parent.GetItemType<IOpenable>();
			if (!openable?.IsOpen ?? false)
			{
				actor.Body.Open(openable, null, null);
			}

			actor.Body.Get(item, container.Parent, quantity, silent: silent,
				ignoreFlags: ItemCanGetIgnore.IgnoreInventoryPlans);
			return new InventoryPlanActionResult
			{
				PrimaryTarget = item,
				SecondaryTarget = container.Parent,
				ActionState = DesiredItemState.Held,
				OriginalReference = originalReference
			};
		}

		if (item.ContainedIn?.IsItemType<ISheath>() ?? false)
		{
			var sheathe = item.ContainedIn;
			actor.Body.Draw(item, null, silent: silent);
			if (wieldOK == false)
			{
				actor.Body.Unwield(item, silent: true);
				return new InventoryPlanActionResult
				{
					PrimaryTarget = item,
					SecondaryTarget = sheathe,
					ActionState = DesiredItemState.Held,
					OriginalReference = originalReference
				};
			}
			else
			{
				return new InventoryPlanActionResult
				{
					PrimaryTarget = item,
					SecondaryTarget = sheathe,
					ActionState = DesiredItemState.Wielded,
					OriginalReference = originalReference
				};
			}
		}

		var beltable = item.GetItemType<IBeltable>();
		if (beltable?.ConnectedTo != null)
		{
			var belt = beltable.ConnectedTo;
			if (!silent)
			{
				actor.OutputHandler.Handle(
					new EmoteOutput(new Emote("@ detach|detaches $0 from $1.", actor, item,
						beltable.ConnectedTo.Parent)));
			}

			beltable.ConnectedTo.RemoveConnectedItem(beltable);
			actor.Body.Get(item, quantity, silent: true, ignoreFlags: ItemCanGetIgnore.IgnoreInventoryPlans);
			return new InventoryPlanActionResult
			{
				PrimaryTarget = item,
				SecondaryTarget = belt.Parent,
				ActionState = DesiredItemState.Held,
				OriginalReference = originalReference
			};
		}

		return new InventoryPlanActionResult
		{
			PrimaryTarget = item,
			ActionState = DesiredItemState.Unknown,
			OriginalReference = originalReference
		};
	}

	private InventoryPlanActionResult WieldItem(ICharacter actor, IGameItem item, bool silent, object originalReference,
		AttackHandednessOptions option)
	{
		var state = DesiredItemState.Wielded;
		switch (option)
		{
			case AttackHandednessOptions.OneHandedOnly:
				state = DesiredItemState.WieldedOneHandedOnly;
				break;
			case AttackHandednessOptions.TwoHandedOnly:
				state = DesiredItemState.WieldedTwoHandedOnly;
				break;
		}

		if (actor.Body.WieldedItems.Contains(item))
		{
			switch (option)
			{
				case AttackHandednessOptions.Any:
					return new InventoryPlanActionResult
					{
						PrimaryTarget = item,
						ActionState = state,
						OriginalReference = originalReference
					};
				case AttackHandednessOptions.OneHandedOnly:
					if (actor.Body.WieldedHandCount(item) == 1)
					{
						return new InventoryPlanActionResult
						{
							PrimaryTarget = item,
							ActionState = state,
							OriginalReference = originalReference
						};
					}

					break;
				case AttackHandednessOptions.TwoHandedOnly:
					if (actor.Body.WieldedHandCount(item) == 2)
					{
						return new InventoryPlanActionResult
						{
							PrimaryTarget = item,
							ActionState = state,
							OriginalReference = originalReference
						};
					}

					break;
			}
		}

		var getResult = GetItem(actor, item, null, silent, true);
		if (getResult.ActionState == DesiredItemState.Held)
		{
			var flags = ItemCanWieldFlags.None;
			switch (option)
			{
				case AttackHandednessOptions.OneHandedOnly:
					flags = ItemCanWieldFlags.RequireOneHand;
					break;
				case AttackHandednessOptions.TwoHandedOnly:
					flags = ItemCanWieldFlags.RequireTwoHands;
					break;
			}

			actor.Body.Wield(item, silent: silent, flags: flags);
		}

		return new InventoryPlanActionResult
		{
			PrimaryTarget = item,
			SecondaryTarget = getResult.SecondaryTarget,
			TertiaryTarget = getResult.TertiaryTarget,
			ActionState = state,
			OriginalReference = originalReference
		};
	}

	private InventoryPlanActionResult WearItem(ICharacter actor, IGameItem item, bool silent, object originalReference,
		IWearProfile desiredProfile)
	{
		if (actor.Body.WornItems.Contains(item))
		{
			if (desiredProfile == null || item?.GetItemType<IWearable>().CurrentProfile == desiredProfile)
			{
				return new InventoryPlanActionResult
				{
					PrimaryTarget = item,
					ActionState = DesiredItemState.Worn,
					OriginalReference = originalReference
				};
			}

			actor.Body.RemoveItem(item);
		}

		var getResult = GetItem(actor, item, null, silent);
		if (desiredProfile != null)
		{
			actor.Body.Wear(item, desiredProfile, silent: silent);
		}
		else
		{
			actor.Body.Wear(item, silent: silent);
		}

		return new InventoryPlanActionResult
		{
			PrimaryTarget = item,
			SecondaryTarget = getResult.SecondaryTarget,
			TertiaryTarget = getResult.TertiaryTarget,
			ActionState = DesiredItemState.Worn,
			OriginalReference = originalReference
		};
	}

	private InventoryPlanActionResult DropItem(ICharacter actor, IGameItem item, bool silent, object originalReference)
	{
		if (!actor.Body.DirectItems.Contains(item) && item.ContainedIn == null)
		{
			return new InventoryPlanActionResult
			{
				PrimaryTarget = item,
				ActionState = DesiredItemState.InRoom,
				OriginalReference = originalReference
			};
		}

		var getResult = GetItem(actor, item, null, silent);
		actor.Body.Drop(item, newStack: true, silent: silent);
		return new InventoryPlanActionResult
		{
			PrimaryTarget = item,
			SecondaryTarget = getResult.SecondaryTarget,
			TertiaryTarget = getResult.TertiaryTarget,
			ActionState = DesiredItemState.InRoom,
			OriginalReference = originalReference
		};
	}

	private InventoryPlanActionResult PutItemContainer(ICharacter actor, IGameItem item, IGameItem container,
		bool silent, object originalReference)
	{
		if (item.ContainedIn == container)
		{
			return new InventoryPlanActionResult
			{
				PrimaryTarget = item,
				SecondaryTarget = container,
				ActionState = DesiredItemState.InContainer,
				OriginalReference = originalReference
			};
		}

		GetItem(actor, item, null, silent);
		actor.Body.Put(item, container, null, silent: silent);
		return new InventoryPlanActionResult
		{
			PrimaryTarget = item,
			SecondaryTarget = container,
			ActionState = DesiredItemState.InContainer,
			OriginalReference = originalReference
		};
	}

	private InventoryPlanActionResult SheatheItem(ICharacter actor, IGameItem item, IGameItem sheath, bool silent,
		object originalReference)
	{
		if (item.ContainedIn == sheath)
		{
			return new InventoryPlanActionResult
			{
				PrimaryTarget = item,
				SecondaryTarget = sheath,
				ActionState = DesiredItemState.Sheathed,
				OriginalReference = originalReference
			};
		}

		GetItem(actor, item, null, silent);
		actor.Body.Sheathe(item, sheath, silent: silent);
		return new InventoryPlanActionResult
		{
			PrimaryTarget = item,
			SecondaryTarget = sheath,
			ActionState = DesiredItemState.Sheathed,
			OriginalReference = originalReference
		};
	}

	private InventoryPlanActionResult ConsumeCommodity(ICharacter actor, IGameItem item, double weight, object originalReference)
	{
		var container = item.ContainedIn;
		var commodity = item.GetItemType<ICommodity>();
		if (commodity.Weight <= weight)
		{
			item.ContainedIn?.Take(item);
			item.InInventoryOf?.Take(item);
			item.Delete();
		}
		else
		{
			item.Weight -= weight;
		}

		return new InventoryPlanActionResult
		{
			PrimaryTarget = item,
			SecondaryTarget = container,
			ActionState = DesiredItemState.Consumed,
			OriginalReference = originalReference
		};
	}

	private InventoryPlanActionResult ConsumeItem(ICharacter actor, IGameItem item, int quantity,
		object originalReference)
	{
		var container = item.ContainedIn;
		var stack = item.GetItemType<IStackable>();
		if (stack != null)
		{
			stack.Quantity -= quantity;
			if (stack.Quantity <= 0)
			{
				item.ContainedIn?.Take(item);
				item.InInventoryOf?.Take(item);
				item.Delete();
			}
		}
		else
		{
			item.ContainedIn?.Take(item);
			item.InInventoryOf?.Take(item);
			item.Delete();
		}

		return new InventoryPlanActionResult
		{
			PrimaryTarget = item,
			SecondaryTarget = container,
			ActionState = DesiredItemState.Consumed,
			OriginalReference = originalReference
		};
	}

	private InventoryPlanActionResult ConsumeLiquid(ICharacter actor, IGameItem item,
		InventoryPlanActionConsumeLiquid action, object originalReference)
	{
		var container = item.GetItemType<ILiquidContainer>();
		foreach (var instance in action.LiquidToTake.Instances)
		{
			var equivalent = container.LiquidMixture.Instances.FirstOrDefault(x => x.CanMergeWith(instance));
			if (equivalent == null)
			{
				continue;
			}

			container.LiquidMixture.RemoveLiquidVolume(equivalent, instance.Amount);
			if (container.LiquidMixture?.IsEmpty != false)
			{
				container.LiquidMixture = null;
				break;
			}

			container.Changed = true;
		}

		return new InventoryPlanActionResult
		{
			PrimaryTarget = item,
			ActionState = DesiredItemState.ConsumeLiquid,
			OriginalReference = originalReference
		};
	}

	#endregion

	#region Implementation of IXmlSavable

	public XElement SaveToXml()
	{
		return new XElement("Plan",
			from phase in _phases
			select phase.SaveToXml()
		);
	}

	#endregion
}
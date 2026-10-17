#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Planes;
using MudSharp.RPG.Checks;

namespace MudSharp.Character;

internal sealed record PeekInspectionResult(bool Interrupted, IReadOnlyList<IGameItem> Items);

/// <summary>Covert observation only. No inventory manipulation, ordinary Look entitlement or crime handling.</summary>
internal static class PeekInspection
{
	private const PerceiveIgnoreFlags SightFlags = PerceiveIgnoreFlags.IgnoreSpotting;

	internal static bool CanUse(ICharacter actor)
	{
		var attempt = actor.Gameworld.GetCheck(CheckType.PeekCheck);
		var reveal = actor.Gameworld.GetCheck(CheckType.PeekRevealCheck);
		if (attempt?.TargetNumberExpression?.Parameters is not { Count: 1 } parameters ||
			!parameters.TryGetValue("peek", out var binding) ||
			binding.Trait is not { TraitType: TraitType.Skill, OwnerScope: TraitOwnerScope.Character } ||
			!actor.HasTrait(binding.Trait) || actor.TraitRawValue(binding.Trait) <= 0.0)
		{
			return false;
		}

		// Fail closed on old-world fallbacks or unsafe builder bindings, rather than train once per item.
		return reveal is { ImproveTraits: false, CanTraitBranchIfMissing: false } &&
			reveal.TargetNumberExpression.Parameters.TryGetValue("peek", out var revealBinding) &&
			revealBinding.Trait.Id == binding.Trait.Id && !revealBinding.CanImprove && !revealBinding.CanBranch;
	}

	internal static bool CanInspectTarget(ICharacter actor, ICharacter target) =>
		actor != target && actor.State.IsAble() && actor.Combat is null && actor.Movement is null && actor.Location is not null &&
		actor.Location == target.Location && actor.RoomLayer == target.RoomLayer && actor.ColocatedWith(target) &&
		actor.CanSee(target, SightFlags) && actor.IlluminationSightDifficulty() != Difficulty.Impossible &&
		actor.CanInteractPlanar(target, PlanarInteractionKind.Inventory);

	private static bool CanInspectItem(ICharacter actor, IGameItem item) =>
		!item.Deleted && actor.CanInteractPlanar(item, PlanarInteractionKind.Inventory) && actor.CanSee(item, SightFlags) &&
		actor.IlluminationSightDifficulty(item.Location ?? actor.Location) != Difficulty.Impossible;

	private static bool IsNormallyExposed(ICharacter target, IGameItem item) =>
		target.Body.ExternalItemsForOtherActors.Contains(item) &&
		(target.Body.HoldOrWieldLocFor(item) as IGrab)?.ItemsVisible() != false;

	internal static IEnumerable<IGameItem> EligibleContainers(ICharacter actor, ICharacter target) =>
		target.Body.ExternalItemsForOtherActors
			.Where(x => IsNormallyExposed(target, x))
			.Where(x => CanInspectItem(actor, x))
			.Where(x => x.GetItemType<IContainer>() is not null && x.GetItemType<IOpenable>()?.IsOpen != false)
			.Distinct();

	internal static PeekInspectionResult Inspect(ICharacter actor, ICharacter target, IGameItem? container = null)
	{
		bool Valid() => CanUse(actor) && CanInspectTarget(actor, target) &&
			(container is null || EligibleContainers(actor, target).Contains(container));
		if (!Valid()) return new(true, []);

		// This is the sole actor check allowed to improve a trait in the action.
		var attempt = actor.Gameworld.GetCheck(CheckType.PeekCheck).Check(actor, Difficulty.Normal, target);
		if (!Valid()) return new(true, []);
		var stopped = false;
		bool CanObserve(ICharacter observer) => observer != actor && observer.Location == actor.Location &&
			observer.RoomLayer == actor.RoomLayer && observer.State.IsConscious() && observer.CanSee(actor, SightFlags) &&
			observer.IlluminationSightDifficulty() != Difficulty.Impossible;
		foreach (var observer in actor.Location.LayerCharacters(actor.RoomLayer)
			.Where(x => x != actor).Distinct().ToList())
		{
			if (!CanObserve(observer)) continue;
			var spotted = actor.Gameworld.GetCheck(CheckType.SpotStealthCheck)
				.Check(observer, observer.Location.SpotDifficulty(observer), actor);
			if (!Valid()) return new(true, []);
			if (!CanObserve(observer)) continue;
			var opposed = new OpposedOutcome(attempt.Outcome, spotted.Outcome);
			if (opposed.Outcome == OpposedOutcomeDirection.Proponent || !(spotted.IsPass() || attempt.IsFail())) continue;
			// Send actual notice immediately, even if a later observer's learning callback interrupts inspection.
			observer.OutputHandler.Send(observer == target
				? $"You notice {actor.HowSeen(observer, true, flags: SightFlags)} trying to peek at your belongings."
				: $"You notice {actor.HowSeen(observer, true, flags: SightFlags)} trying to peek at {target.HowSeen(observer, flags: SightFlags)}'s belongings.");
			if (observer == target && !target.IsHelpless && target.State.IsAble() &&
				opposed.Outcome == OpposedOutcomeDirection.Opponent && opposed.Degree >= OpposedOutcomeDegree.Major)
			{
				stopped = true;
			}
		}

		if (!Valid() || stopped || attempt.IsAbjectFailure || attempt.Outcome == Outcome.MajorFail) return new(true, []);

		// Never recursively enumerate containers, implants or internal possessions.
		var candidates = (container is null
			? target.Body.WornItems.Concat(target.Body.HeldItems).Concat(target.Body.WieldedItems)
			: container.GetItemType<IContainer>().Contents).Distinct().ToList();
		var items = new List<IGameItem>();
		var revealedItems = new HashSet<IGameItem>();
		bool StillEligible(IGameItem item) => Valid() && CanInspectItem(actor, item) &&
			(container is null
				? target.Body.WornItems.Concat(target.Body.HeldItems).Concat(target.Body.WieldedItems).Contains(item)
				: container.GetItemType<IContainer>().Contents.Contains(item));
		foreach (var item in candidates)
		{
			if (!StillEligible(item)) continue;
			if (container is null && IsNormallyExposed(target, item))
			{
				items.Add(item);
				continue;
			}

			var carried = container is null && target.Body.HeldItems.Concat(target.Body.WieldedItems).Contains(item);
			var setting = container is not null ? "PeekContainerItemDifficulty" :
				carried ? "PeekCarriedItemDifficulty" : "PeekCoveredItemDifficulty";
			var difficulty = RevealDifficulty(actor, setting, container is null && !carried ? Difficulty.Hard : Difficulty.Normal);
			var revealed = actor.Gameworld.GetCheck(CheckType.PeekRevealCheck).Check(actor, difficulty, item);
			if (revealed.IsPass() && StillEligible(item))
			{
				items.Add(item);
				revealedItems.Add(item);
			}
		}

		// Checks can invoke learning/prog callbacks. Never disclose a stale inventory snapshot.
		return !Valid() ? new(true, []) : new(false, items.Where(StillEligible)
			.Where(x => revealedItems.Contains(x) || container is null && IsNormallyExposed(target, x)).ToList());
	}

	private static Difficulty RevealDifficulty(ICharacter actor, string setting, Difficulty fallback) =>
		int.TryParse(actor.Gameworld.GetStaticConfiguration(setting), out var value) && Enum.IsDefined((Difficulty)value)
			? (Difficulty)value : fallback;
}

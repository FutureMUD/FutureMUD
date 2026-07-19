#nullable enable

using MudSharp.GameItems;
using MudSharp.NPC;
using MudSharp.PerceptionEngine.Handlers;

namespace MudSharp.Character;

public sealed record PossessionControlRuntimeState(
	ICharacterController PossessorController,
	ICharacterController? DisplacedController,
	PossessionVictimContext? VictimContext
);

public sealed record PossessionControlResult(
	bool Success,
	string Message,
	PossessionControlRuntimeState? RuntimeState = null
);

public static class PossessionControlService
{
	public static bool AnyPossessionEffectsForAnchor(ICharacter anchor)
	{
		var anchorId = CharacterInstanceIdentityComparer.IdentityId(anchor);
		var anchorInstanceId = anchor.InstanceId;
		return PossessionEffectOwners(anchor.Gameworld)
		       .Any(x =>
			       x.EffectsOfType<IPossessedBodyEffect>(effect =>
				        EffectMatchesAnchor(effect.AnchorCharacterId, effect.AnchorInstanceId, anchorId, anchorInstanceId)).Any() ||
			       x.EffectsOfType<ILiveBodyPossessionEffect>(effect =>
				        EffectMatchesAnchor(effect.AnchorCharacterId, effect.AnchorInstanceId, anchorId, anchorInstanceId)).Any() ||
			       x.EffectsOfType<ICorpsePossessionEffect>(effect =>
				        EffectMatchesAnchor(effect.AnchorCharacterId, effect.AnchorInstanceId, anchorId, anchorInstanceId)).Any());
	}

	public static bool RemovePossessionEffectsForAnchor(ICharacter anchor)
	{
		var anchorId = CharacterInstanceIdentityComparer.IdentityId(anchor);
		var anchorInstanceId = anchor.InstanceId;
		var removed = false;
		foreach (var owner in PossessionEffectOwners(anchor.Gameworld))
		{
			removed |= owner.RemoveAllEffects<ILiveBodyPossessionEffect>(
				effect => EffectMatchesAnchor(effect.AnchorCharacterId, effect.AnchorInstanceId, anchorId, anchorInstanceId),
				true);
			removed |= owner.RemoveAllEffects<ICorpsePossessionEffect>(
				effect => EffectMatchesAnchor(effect.AnchorCharacterId, effect.AnchorInstanceId, anchorId, anchorInstanceId),
				true);
		}

		return removed;
	}

	public static bool RemoveLiveBodyPossessionEffectsForTarget(ICharacter target)
	{
		return target.RemoveAllEffects<ILiveBodyPossessionEffect>(
			effect => effect.TargetInstanceId == target.InstanceId,
			true);
	}

	public static bool RemoveCorpsePossessionEffectsForAnimatedInstance(ICharacterInstance instance)
	{
		var removed = false;
		foreach (var owner in instance.Gameworld.Items.ToList())
		{
			removed |= owner.RemoveAllEffects<ICorpsePossessionEffect>(
				effect => effect.AnimatedInstanceId == instance.InstanceId,
				true);
		}

		return removed;
	}

	public static bool RemoveCorpsePossessionEffectsForCorpse(IGameItem corpseItem)
	{
		return corpseItem.RemoveAllEffects<ICorpsePossessionEffect>(
			effect => effect.CorpseItemId == corpseItem.Id,
			true);
	}

	public static bool RemoveAnimatedCorpseEffectsForAnimatedInstance(ICharacterInstance instance)
	{
		var removed = false;
		foreach (var owner in instance.Gameworld.Items.ToList())
		{
			removed |= owner.RemoveAllEffects<IAnimatedCorpseEffect>(
				effect => effect.AnimatedInstanceId == instance.InstanceId,
				true);
		}

		return removed;
	}

	public static bool RemoveAnimatedCorpseEffectsForCorpse(IGameItem corpseItem)
	{
		return corpseItem.RemoveAllEffects<IAnimatedCorpseEffect>(
			effect => effect.CorpseItemId == corpseItem.Id,
			true);
	}

	public static PossessionControlResult BeginLivePossession(
		ICharacter anchor,
		ICharacter target,
		string victimStartEcho,
		string victimEndEcho)
	{
		if (anchor.CharacterController is not ICharacterController possessorController)
		{
			return new PossessionControlResult(false, "You do not currently have a controller to possess with.");
		}

		if (ReferenceEquals(anchor.CharacterController, target.CharacterController))
		{
			return new PossessionControlResult(false, "That body is already controlled by your connection.");
		}

		var displacedController = target.CharacterController;
		PossessionVictimContext? victimContext = null;
		if (displacedController is not null)
		{
			if (target.IsPlayerCharacter && !target.IsGuest)
			{
				victimContext = new PossessionVictimContext(target, anchor, victimStartEcho, victimEndEcho);
				displacedController.SetContext(victimContext);
			}
			else
			{
				target.LoseControl(displacedController);
			}
		}

		possessorController.SetContext(target);
		return new PossessionControlResult(true, string.Empty,
			new PossessionControlRuntimeState(possessorController, displacedController, victimContext));
	}

	public static void RestoreLivePossession(
		ICharacter? anchor,
		ICharacter? target,
		PossessionControlRuntimeState? runtimeState)
	{
		var possessorController = runtimeState?.PossessorController ?? target?.CharacterController;
		if (anchor is not null && possessorController is not null && !anchor.State.IsDead())
		{
			possessorController.SetContext(anchor);
		}
		else if (target?.CharacterController is not null)
		{
			target.LoseControl(target.CharacterController);
		}

		if (target is null || target.State.IsDead())
		{
			return;
		}

		if (runtimeState?.DisplacedController is not null)
		{
			if (runtimeState.VictimContext is not null)
			{
				runtimeState.DisplacedController.SetContext(target);
			}
			else
			{
				target.SilentAssumeControl(runtimeState.DisplacedController);
				target.Register(runtimeState.DisplacedController.OutputHandler);
			}

			return;
		}

		if (target is INPC)
		{
			target.SilentAssumeControl(new NPCController());
			target.Register(new NonPlayerOutputHandler());
		}
	}

	public static IEnumerable<IPerceivable> PossessionEffectOwners(IFuturemud gameworld)
	{
		return gameworld.Actors
		                .Concat(gameworld.CachedActors)
		                .Cast<IPerceivable>()
		                .Concat(gameworld.Items)
		                .Where(x => x is not null)
		                .Distinct()
		                .ToList();
	}

	private static bool EffectMatchesAnchor(long effectAnchorId, long effectAnchorInstanceId, long anchorId,
		long anchorInstanceId)
	{
		return effectAnchorId == anchorId &&
		       (effectAnchorInstanceId == 0 || effectAnchorInstanceId == anchorInstanceId);
	}
}

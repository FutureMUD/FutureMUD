#nullable enable

using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.NPC.AI;

namespace MudSharp.RPG.Law;

/// <summary>Transfers an actually subdued wanted character through native remand and belongings handling.</summary>
public static class CustodyHandover
{
	public static bool IsSubduedBy(ICharacter target, ICharacter holder) => target.Body is not null &&
		target.Body.EffectsOfType<IBeingGrappled>().ToArray().Any(x => x.Applies() && x.UnderControl &&
			CharacterInstanceIdentityComparer.SamePhysicalInstanceOrBody(x.Grappling.CharacterOwner, holder) &&
			target.Body.EffectsOfType<IBeingGrappled>().Any(y => ReferenceEquals(y, x)) &&
			holder.EffectsOfType<IGrappling>().Any(y => ReferenceEquals(y, x.Grappling)));

	public static bool TryHandover(ICharacter holder, ICharacter target, ICharacter receiver, ILegalAuthority authority)
	{
		var location = holder.Location;
		var holderBody = holder.Body;
		var targetBody = target.Body;
		var receiverBody = receiver.Body;
		bool SameBodies() => ReferenceEquals(holder.Body, holderBody) && ReferenceEquals(target.Body, targetBody) &&
			ReferenceEquals(receiver.Body, receiverBody);
		bool SameScene() => CommandExecutionScope.TryContinue() && location is not null &&
			holder.ColocatedWith(target) && holder.ColocatedWith(receiver) && ReferenceEquals(holder.Location, location) &&
			ReferenceEquals(holder.Gameworld, authority.Gameworld) && ReferenceEquals(target.Gameworld, authority.Gameworld) &&
			ReferenceEquals(receiver.Gameworld, authority.Gameworld) && receiver.State.IsConscious() &&
			!receiver.State.HasFlag(CharacterState.Sleeping) && receiver.Combat is null && SameBodies();
		if (CharacterInstanceIdentityComparer.SamePhysicalInstanceOrBody(holder, target) ||
			CharacterInstanceIdentityComparer.SamePhysicalInstanceOrBody(holder, receiver) ||
			CharacterInstanceIdentityComparer.SamePhysicalInstanceOrBody(target, receiver) ||
			!SameScene() || !IsSubduedBy(target, holder)) return false;
		var crime = EnforcementCustodyHelper.SelectArrestableCrime(authority, target);
		var role = authority.GetEnforcementAuthority(receiver);
		if (crime is null || role is null || !role.ArrestableClasses.Contains(authority.GetLegalClass(target)) ||
			!authority.EnforcementZones.Contains(location.Zone) || authority.PrisonerBelongingsStorageLocation is null ||
			!authority.RoomLocations.Any() || !SameScene()) return false;
		// Do not break a third party's physical control as a side effect of this handover.
		if (target.Body.EffectsOfType<IBeingGrappled>().Any(x => x.Applies() &&
			!CharacterInstanceIdentityComparer.SamePhysicalInstanceOrBody(x.Grappling.CharacterOwner, holder)) ||
			target.CombinedEffectsOfType<Dragging.DragTarget>().Any(x => x.Applies() &&
				!ReferenceEquals(x.Drag.Owner, holder))) return false;
		if (!SameScene() || !IsSubduedBy(target, holder) || !SameScene()) return false;

		CommandExecutionScope.MarkCommitted();
		foreach (var drag in holder.EffectsOfType<Dragging>().Where(x => ReferenceEquals(x.Target, target)).ToArray())
		{
			holder.RemoveEffect(drag, true);
			if (!SameScene()) return false;
		}
		target.Body.RemoveAllEffects<IRestraintEffect>(x => true, true);
		if (!SameScene()) return false;
		EnforcementCustodyHelper.ReleaseGrapplesAndCombatAgainst(target, [holder, receiver]);
		if (!SameScene() || EnforcementCustodyHelper.SelectArrestableCrime(authority, target) is null) return false;
		location.OutputHandler.Handle(new EmoteOutput(new Emote("$0 hand|hands $1 over to $2.", holder, holder, target, receiver)));
		if (!SameScene()) return false;
		role = authority.GetEnforcementAuthority(receiver);
		if (role is null || !role.ArrestableClasses.Contains(authority.GetLegalClass(target)) ||
			!SameScene() || EnforcementCustodyHelper.SelectArrestableCrime(authority, target) is null) return false;
		authority.IncarcerateCriminal(target);
		return true;
	}
}

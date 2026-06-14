using MudSharp.Body;
using MudSharp.Framework;

#nullable enable

namespace MudSharp.Character;

public static class CharacterInstanceIdentityComparer
{
	// Identity equality answers "same person"; physical equality answers "same active world presence".
	public static long IdentityId(ICharacter? actor)
	{
		if (actor is null)
		{
			return 0;
		}

		var identityId = actor.Identity?.Id ?? 0;
		return identityId != 0 ? identityId : actor.Id;
	}

	public static long FrameworkItemId(IFrameworkItem? item)
	{
		return item is ICharacter character ? IdentityId(character) : item?.Id ?? 0;
	}

	public static bool SameIdentity(ICharacter actor, ICharacter other)
	{
		if (actor is null || other is null)
		{
			return false;
		}

		if (ReferenceEquals(actor, other))
		{
			return true;
		}

		var actorIdentityId = IdentityId(actor);
		var otherIdentityId = IdentityId(other);
		return actorIdentityId != 0 && actorIdentityId == otherIdentityId;
	}

	public static bool SamePhysicalInstance(ICharacter actor, IPerceivable other)
	{
		if (actor is null || other is null)
		{
			return false;
		}

		if (ReferenceEquals(actor, other) || ReferenceEquals(actor.Body, other))
		{
			return true;
		}

		if (other is IBody body)
		{
			return ReferenceEquals(actor.Body, body) ||
			       body.Actor is not null &&
			       ReferenceEquals(body.Actor.Body, body) &&
			       SamePhysicalInstance(actor, body.Actor);
		}

		if (other is not ICharacter character || !SameIdentity(actor, character))
		{
			return false;
		}

		if (actor.InstanceId != 0 && character.InstanceId != 0)
		{
			return actor.InstanceId == character.InstanceId;
		}

		return actor.Id != 0 && actor.Id == character.Id;
	}
}

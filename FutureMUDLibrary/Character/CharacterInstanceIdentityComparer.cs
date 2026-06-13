using MudSharp.Body;
using MudSharp.Framework;

#nullable enable

namespace MudSharp.Character;

public static class CharacterInstanceIdentityComparer
{
	// Identity equality answers "same person"; physical equality answers "same active world presence".
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

		var actorIdentityId = actor.Identity?.Id ?? actor.Id;
		var otherIdentityId = other.Identity?.Id ?? other.Id;
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

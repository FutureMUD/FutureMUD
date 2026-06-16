using MudSharp.Body;
using MudSharp.Framework;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace MudSharp.Character;

public enum CharacterRecognitionScope
{
	Identity = 0,
	PhysicalBody = 1
}

public sealed record CharacterRecognitionKey(string TargetType, long TargetId);

public static class CharacterInstanceIdentityComparer
{
	public const string CharacterRecognitionTargetType = "Character";
	public const string BodyRecognitionTargetType = "Body";

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

	public static CharacterRecognitionKey RecognitionKeyFor(IPerceivable target,
		CharacterRecognitionScope scope = CharacterRecognitionScope.Identity)
	{
		switch (scope)
		{
			case CharacterRecognitionScope.PhysicalBody when target is ICharacter character:
				return new CharacterRecognitionKey(BodyRecognitionTargetType, character.Body?.Id ?? 0);
			case CharacterRecognitionScope.PhysicalBody when target is IBody body:
				return new CharacterRecognitionKey(BodyRecognitionTargetType, body.Id);
			case CharacterRecognitionScope.Identity when target is ICharacter character:
				return new CharacterRecognitionKey(CharacterRecognitionTargetType, IdentityId(character));
			case CharacterRecognitionScope.Identity when target is IBody { Actor: { } actor }:
				return new CharacterRecognitionKey(CharacterRecognitionTargetType, IdentityId(actor));
			default:
				return new CharacterRecognitionKey(target.FrameworkItemType, target.Id);
		}
	}

	public static IEnumerable<CharacterRecognitionKey> RecognitionLookupKeys(IPerceivable target)
	{
		if (target is ICharacter character)
		{
			yield return RecognitionKeyFor(character);
			if (character.Body is not null)
			{
				yield return RecognitionKeyFor(character, CharacterRecognitionScope.PhysicalBody);
			}

			yield break;
		}

		if (target is IBody body)
		{
			yield return RecognitionKeyFor(body, CharacterRecognitionScope.PhysicalBody);
			if (body.Actor is not null)
			{
				yield return RecognitionKeyFor(body);
			}

			yield break;
		}

		yield return new CharacterRecognitionKey(target.FrameworkItemType, target.Id);
	}

	public static long? InstanceId(ICharacter? actor)
	{
		return actor?.InstanceId is > 0L ? actor.InstanceId : null;
	}

	public static long PhysicalInstanceKey(ICharacter? actor)
	{
		if (actor is null)
		{
			return 0;
		}

		return InstanceId(actor) ?? IdentityId(actor);
	}

	public static ICharacter? ResolvePhysicalInstance(IFuturemud gameworld, long identityId, long? instanceId,
		bool loadIdentityIfNeeded = true, bool fallbackToPrimary = true)
	{
		if (identityId <= 0)
		{
			return null;
		}

		var identity = gameworld.TryGetCharacter(identityId, loadIdentityIfNeeded);
		if (identity is null)
		{
			return null;
		}

		if (instanceId is null or <= 0)
		{
			return identity;
		}

		var instance = identity.Identity.Instances
		                       .OfType<ICharacter>()
		                       .FirstOrDefault(x => x.InstanceId == instanceId.Value);
		return instance ?? (fallbackToPrimary ? identity : null);
	}

	public static bool ContainsPhysicalInstance(this IEnumerable<ICharacter> actors, ICharacter actor)
	{
		return actors.Any(x => SamePhysicalInstance(x, actor));
	}

	public static int RemovePhysicalInstance(this IList<ICharacter> actors, ICharacter actor)
	{
		var removed = 0;
		for (var i = actors.Count - 1; i >= 0; i--)
		{
			if (!SamePhysicalInstance(actors[i], actor))
			{
				continue;
			}

			actors.RemoveAt(i);
			removed++;
		}

		return removed;
	}

	public static IEnumerable<ICharacter> DistinctPhysicalInstances(this IEnumerable<ICharacter> actors)
	{
		return actors.Where(x => x is not null)
		             .DistinctBy(x => x.InstanceId > 0 ? x.InstanceId : IdentityId(x));
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

	public static bool SamePhysicalInstanceOrBody(ICharacter? actor, IPerceivable? other)
	{
		if (actor is null || other is null)
		{
			return false;
		}

		return SamePhysicalInstance(actor, other);
	}

	public static bool SameIdentityOrPrimaryOwner(ICharacter? actor, IPerceivable? other)
	{
		if (actor is null || other is null)
		{
			return false;
		}

		return other switch
		{
			ICharacter character => SameIdentity(actor, character),
			IBody body when body.Actor is ICharacter bodyActor => SameIdentity(actor, bodyActor),
			_ => false
		};
	}
}

public sealed class CharacterPhysicalInstanceEqualityComparer : IEqualityComparer<ICharacter>
{
	public static CharacterPhysicalInstanceEqualityComparer Instance { get; } = new();

	private CharacterPhysicalInstanceEqualityComparer()
	{
	}

	public bool Equals(ICharacter? x, ICharacter? y)
	{
		if (x is null || y is null)
		{
			return x is null && y is null;
		}

		return CharacterInstanceIdentityComparer.SamePhysicalInstance(x, y);
	}

	public int GetHashCode(ICharacter obj)
	{
		var instanceId = CharacterInstanceIdentityComparer.InstanceId(obj);
		return instanceId?.GetHashCode() ?? CharacterInstanceIdentityComparer.IdentityId(obj).GetHashCode();
	}
}

#nullable enable

using System.Collections.Generic;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Framework;

public enum PhysicalEntityKind
{
	Character,
	Body,
	CharacterInstance,
	Wound
}

/// <summary>An identity field whose owning code requires a physical entity to remain available.</summary>
public readonly record struct PhysicalEntityReference(PhysicalEntityKind Kind, long Id, string Field)
{
	public static IEnumerable<PhysicalEntityReference> FromItem(IFrameworkItem? item, string field)
	{
		if (item is ICharacter actor)
		{
			yield return new(PhysicalEntityKind.Character, CharacterInstanceIdentityComparer.IdentityId(actor), field);
			if (CharacterInstanceIdentityComparer.InstanceId(actor) is { } instance)
				yield return new(PhysicalEntityKind.CharacterInstance, instance, field);
			if (actor.Body is { } actorBody)
				yield return new(PhysicalEntityKind.Body, actorBody.Id, field);
		}
		else if (item is IGameItem gameItem && gameItem.GetItemType<IButcherable>() is { } remains)
		{
			yield return new(PhysicalEntityKind.Body, remains.OriginalBodyId, field);
		}
		else if (item is IBody body)
		{
			yield return new(PhysicalEntityKind.Body, body.Id, field);
		}
	}
}

/// <summary>
/// Reports direct live dependencies without resolving lazy objects, serializing state or invoking callbacks.
/// Canonical attribution and static configuration identities are excluded.
/// </summary>
public interface IPhysicalEntityReferenceProvider
{
	IEnumerable<PhysicalEntityReference> PhysicalReferences { get; }
}

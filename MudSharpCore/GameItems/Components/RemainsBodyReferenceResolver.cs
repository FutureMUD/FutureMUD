#nullable enable

using MudSharp.Body;
using MudSharp.Character;

namespace MudSharp.GameItems.Components;

internal static class RemainsBodyReferenceResolver
{
	public static IBody? Resolve(IFuturemud world, ICharacter? owner, long bodyId, bool allowOwnedLiveBody)
	{
		if (owner is null || bodyId <= 0) return null;
		if (owner is MudSharp.Character.Character character)
		{
			return character.LoadBodyForRemains(bodyId, allowOwnedLiveBody);
		}
		var body = world.Bodies.Get(bodyId) ?? owner.Bodies.FirstOrDefault(x => x.Id == bodyId);
		if (body?.Actor is null || CharacterInstanceIdentityComparer.IdentityId(body.Actor) != CharacterInstanceIdentityComparer.IdentityId(owner) ||
		    !allowOwnedLiveBody && (owner.CurrentBody.Id == bodyId ||
		                           owner.Identity?.Instances.Any(x => x.Body.Id == bodyId) == true)) return null;
		return body;
	}
}

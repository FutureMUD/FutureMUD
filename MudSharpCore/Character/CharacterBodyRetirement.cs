#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Body;
using MudSharp.Database;
using MudSharp.Framework.Scheduling;
using RuntimeBody = MudSharp.Body.Implementations.Body;

namespace MudSharp.Character;

public partial class Character
{
	private Dictionary<long, DateTime>? _pendingBodyRetirements;

	private void RecordBodyRetirement(IBody body)
	{
		if (body.Id <= 0 || CurrentBody.Id == body.Id || !_forms.Any(x => SameFormBody(x.Body, body)))
		{
			throw new InvalidOperationException("Only an owned inactive form can be retired after backup death.");
		}
		(_pendingBodyRetirements ??= new()).TryAdd(body.Id, RuntimeClock.UtcNow);
	}

	private void SaveBodyRetirements(Models.Character character)
	{
		if (_pendingBodyRetirements is null) return;
		var previousBodyId = FMDB.Context.Entry(character).Property(x => x.BodyId).OriginalValue;
		foreach (var (bodyId, retiredUtc) in _pendingBodyRetirements)
		{
			var existing = FMDB.Context.CharacterBodyRetirements.Find(bodyId);
			if (existing is not null)
			{
				if (existing.CharacterId != character.Id) throw new InvalidOperationException("Retired body belongs to another identity.");
				continue;
			}
			if (bodyId == Body.Id || !FMDB.Context.Bodies.Any(x => x.Id == bodyId) ||
			    (previousBodyId != bodyId && !character.CharacterBodies.Any(x => x.BodyId == bodyId) &&
			     !character.CharacterBodySources.Any(x => x.BodyId == bodyId)) ||
			    FMDB.Context.Characters.Any(x => x.Id != character.Id && x.BodyId == bodyId) ||
			    FMDB.Context.CharacterBodies.Any(x => x.CharacterId != character.Id && x.BodyId == bodyId) ||
			    FMDB.Context.CharacterBodySources.Any(x => x.CharacterId != character.Id && x.BodyId == bodyId))
			{
				throw new InvalidOperationException("Ordinary retirement needs persisted exact body ownership before mappings are removed.");
			}
			FMDB.Context.CharacterBodyRetirements.Add(new()
			{
				BodyId = bodyId, CharacterId = character.Id, RetiredUtc = retiredUtc
			});
		}
	}

	/// <summary>Read an exact remains body without granting retirement ownership or redirecting its ID.</summary>
	internal IBody? LoadBodyForRemains(long bodyId, bool allowOwnedLiveBody)
	{
		if (bodyId <= 0 || !allowOwnedLiveBody && bodyId == CurrentBody.Id) return null;
		using var scope = FMDB.BeginIndependentScope();
		using var db = new FMDB();
		if (FMDB.Context.Characters.Any(x => x.BodyId == bodyId && x.Id != Id) ||
		    FMDB.Context.CharacterInstances.Any(x => x.BodyId == bodyId && x.CharacterId != Id) ||
		    FMDB.Context.CharacterBodies.Any(x => x.BodyId == bodyId && x.CharacterId != Id) ||
		    FMDB.Context.CharacterBodySources.Any(x => x.BodyId == bodyId && x.CharacterId != Id) ||
		    FMDB.Context.CharacterBodyRetirements.Any(x => x.BodyId == bodyId && x.CharacterId != Id) ||
		    !allowOwnedLiveBody && (FMDB.Context.Characters.Any(x => x.BodyId == bodyId) ||
		                          FMDB.Context.CharacterInstances.Any(x => x.BodyId == bodyId))) return null;
		var cached = Gameworld.Bodies.Get(bodyId) ?? Bodies.FirstOrDefault(x => x.Id == bodyId) ??
		             (CurrentBody.Id == bodyId ? CurrentBody : null);
		if (cached is not null)
		{
			return cached.Actor is not null && CharacterInstanceIdentityComparer.IdentityId(cached.Actor) == Id ? cached : null;
		}
		// A missing runtime instance must not be reconstructed as an inactive body of the canonical actor.
		if (FMDB.Context.CharacterInstances.Any(x => x.BodyId == bodyId)) return null;
		var model = FMDB.Context.Bodies.Find(bodyId);
		if (model is null) return null;
		var body = new RuntimeBody(model, Gameworld, this);
		body.LoadInventory(model);
		Gameworld.Add(body);
		return body;
	}
}

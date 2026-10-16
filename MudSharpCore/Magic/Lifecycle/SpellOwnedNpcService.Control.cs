#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Effects;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.Magic.Lifecycle;

public sealed partial class SpellOwnedNpcService
{
	internal static bool IsExactCurrentNpcPrimary(long rowId, long rowCharacterId, long rowBodyId, bool isPrimary,
		long loadedInstanceId, long loadedCharacterId, long loadedBodyId) =>
		isPrimary && rowId > 0 && rowId == loadedInstanceId && rowCharacterId == loadedCharacterId && rowBodyId == loadedBodyId;

	/// <summary>Read-only authorization for orders; it never loads either actor or changes lifecycle state.</summary>
	public bool HasCreatorMountAdmission(ICharacter mount, ICharacter rider)
	{
		var bonds = mount.EffectsOfType<MudSharp.Effects.Concrete.SpellNpcControlEffect>().ToArray();
		return bonds.Length == 1 && bonds[0].CanAcceptOrder(rider) &&
			bonds[0].CreatorId == CharacterInstanceIdentityComparer.IdentityId(rider) &&
			bonds[0].CreatorInstanceId == rider.InstanceId;
	}

	public bool HasCommandAuthority(ICharacter controlled, ICharacter commander, Guid lifecycleId, long creatorId,
		long creatorInstanceId, long spellId, int grade)
	{
		if (lifecycleId == Guid.Empty || creatorId <= 0 || creatorInstanceId <= 0 || spellId <= 0 || grade is < 1 or > 7 ||
			!ReferenceEquals(controlled.Gameworld, world) || !ReferenceEquals(commander.Gameworld, world) ||
			controlled.Id <= 0 || controlled.Body?.Id <= 0 || commander.InstanceId != creatorInstanceId ||
			CharacterInstanceIdentityComparer.IdentityId(commander) != creatorId ||
			controlled.State.HasFlag(CharacterState.Dead) || controlled.State.HasFlag(CharacterState.Stasis) ||
			commander.State.HasFlag(CharacterState.Dead) || commander.State.HasFlag(CharacterState.Stasis) || !world.Actors.Any(x => ReferenceEquals(x, controlled)) ||
			!world.Actors.Any(x => ReferenceEquals(x, commander))) return false;
		var controlledBody = controlled.Body;
		var commanderBody = commander.Body;
		var controlledBodyId = controlledBody.Id;
		var commanderBodyId = commanderBody.Id;
		var controlledInstanceId = controlled.InstanceId;
		var commanderInstanceId = commander.InstanceId;
		var controlledIdentityId = CharacterInstanceIdentityComparer.IdentityId(controlled);
		var commanderIdentityId = CharacterInstanceIdentityComparer.IdentityId(commander);

		try
		{
			using var isolated = FMDB.BeginIndependentScope(); using var db = new FMDB();
			var lifeRow = FMDB.Context.MagicSpellLifecycles.AsNoTracking().Include(x => x.Entities)
				.SingleOrDefault(x => x.Id == lifecycleId);
			if (lifeRow is null || lifeRow.State != (int)SpellLifecycleState.Active || lifeRow.SpellId != spellId ||
				lifeRow.Grade != grade || lifeRow.CreatorId != creatorId || lifeRow.CreatedUtc > RuntimeClock.UtcNow ||
				lifeRow.DeadlineUtc is { } deadline && deadline <= RuntimeClock.UtcNow || lifeRow.DeathObservedUtc is not null ||
				!HasUsableNpcDiagnostic(lifeRow.Diagnostic)) return false;
			var claims = lifeRow.Entities.Select(x => ((SpellOwnedEntityKind)x.Kind, x.EntityId)).ToArray();
			if (claims.Length != 2 || claims.Count(x => x.Item1 == SpellOwnedEntityKind.AutonomousCharacter && x.EntityId == controlled.Id) != 1 ||
				claims.Count(x => x.Item1 == SpellOwnedEntityKind.Body && x.EntityId == controlled.Body.Id) != 1) return false;

			var currentNpc = FMDB.Context.Characters.AsNoTracking().Where(x => x.Id == controlled.Id && !x.IsArchived && x.BodyId == controlled.Body.Id)
				.Select(x => x.Id).SingleOrDefault();
			if (currentNpc != controlled.Id) return false;
			var npcPrimary = FMDB.Context.CharacterInstances.AsNoTracking().Where(x => x.CharacterId == controlled.Id && x.IsPrimary)
				.Select(x => new { x.Id, x.BodyId, x.CharacterId }).ToArray();
			if (npcPrimary.Length != 1 || !IsExactCurrentNpcPrimary(npcPrimary[0].Id, npcPrimary[0].CharacterId,
				npcPrimary[0].BodyId, true, controlled.InstanceId, controlled.Id, controlled.Body.Id)) return false;

			var creatorPrimary = FMDB.Context.CharacterInstances.AsNoTracking().Where(x => x.Id == creatorInstanceId && x.CharacterId == creatorId && x.IsPrimary)
				.Select(x => new { x.Id, x.BodyId, x.CharacterId }).SingleOrDefault();
			if (creatorPrimary is null || creatorPrimary.Id != commander.InstanceId || commander.Body?.Id != creatorPrimary.BodyId) return false;

			if (world.MagicSpells.Get(spellId) is not MagicSpell { ReadyForGame: true, GradeProfile: { } profile } ||
				profile.Grades.All(x => x.Grade != grade) || world.MagicCasting?.Acquisition(commander, spellId) is not { } acquired ||
				acquired.ProfileVersion != profile.Version) return false;
			var admitted = world.MagicCapabilities.OfType<IMagicCastingCapability>().Any(capability =>
				ReferenceEquals(world.MagicCapabilities.Get(capability.Id), capability) && commander.Capabilities.Any(x => ReferenceEquals(x, capability)) &&
				(commander.CombinedEffectsOfType<IGiveMagicCapabilityEffect>().Any(effect => effect.Capabilities.Any(x => ReferenceEquals(x, capability))) ||
				 commander.Merits.OfType<IMagicCapabilityMerit>().Any(merit => merit.Applies(commander) && merit.Capabilities.Any(x => ReferenceEquals(x, capability)))) &&
				capability.CastingPolicy is { Enabled: true } policy && capability.CastingConfigurationErrors().Count == 0 &&
				policy.Admissions.Any(admission => admission.SpellId == spellId && grade >= admission.MinimumGrade && grade <= admission.MaximumGrade));
			if (!admitted || !ReferenceEquals(controlled.Body, controlledBody) || controlledBody.Id != controlledBodyId ||
				!ReferenceEquals(commander.Body, commanderBody) || commanderBody.Id != commanderBodyId ||
				controlled.InstanceId != controlledInstanceId || commander.InstanceId != commanderInstanceId ||
				CharacterInstanceIdentityComparer.IdentityId(controlled) != controlledIdentityId ||
				CharacterInstanceIdentityComparer.IdentityId(commander) != commanderIdentityId ||
				controlled.State.HasFlag(CharacterState.Dead) || controlled.State.HasFlag(CharacterState.Stasis) ||
				commander.State.HasFlag(CharacterState.Dead) || commander.State.HasFlag(CharacterState.Stasis) || !world.Actors.Any(x => ReferenceEquals(x, controlled)) ||
				!world.Actors.Any(x => ReferenceEquals(x, commander))) return false;

			// Merit applicability callbacks are arbitrary game code. Re-read durable authority after them.
			var currentLife = FMDB.Context.MagicSpellLifecycles.AsNoTracking().Include(x => x.Entities)
				.SingleOrDefault(x => x.Id == lifecycleId);
			if (currentLife is null || currentLife.State != (int)SpellLifecycleState.Active || currentLife.SpellId != spellId ||
				currentLife.Grade != grade || currentLife.CreatorId != creatorId || currentLife.CreatedUtc > RuntimeClock.UtcNow ||
				currentLife.DeadlineUtc is { } currentDeadline && currentDeadline <= RuntimeClock.UtcNow || currentLife.DeathObservedUtc is not null ||
				!HasUsableNpcDiagnostic(currentLife.Diagnostic)) return false;
			var currentClaims = currentLife.Entities.Select(x => ((SpellOwnedEntityKind)x.Kind, x.EntityId)).ToArray();
			if (currentClaims.Length != 2 || currentClaims.Count(x => x.Item1 == SpellOwnedEntityKind.AutonomousCharacter && x.EntityId == controlled.Id) != 1 ||
				currentClaims.Count(x => x.Item1 == SpellOwnedEntityKind.Body && x.EntityId == controlledBody.Id) != 1) return false;
			var currentCharacter = FMDB.Context.Characters.AsNoTracking().Where(x => x.Id == controlled.Id && !x.IsArchived && x.BodyId == controlledBodyId)
				.Select(x => x.Id).SingleOrDefault();
			var currentPrimaries = FMDB.Context.CharacterInstances.AsNoTracking().Where(x => x.CharacterId == controlled.Id && x.IsPrimary)
				.Select(x => new { x.Id, x.CharacterId, x.BodyId, x.IsPrimary }).ToArray();
			var currentCreator = FMDB.Context.CharacterInstances.AsNoTracking()
				.Where(x => x.Id == creatorInstanceId && x.CharacterId == creatorId && x.IsPrimary)
				.Select(x => new { x.Id, x.CharacterId, x.BodyId }).SingleOrDefault();
			return currentCharacter == controlled.Id && currentPrimaries.Length == 1 &&
				IsExactCurrentNpcPrimary(currentPrimaries[0].Id, currentPrimaries[0].CharacterId, currentPrimaries[0].BodyId,
					currentPrimaries[0].IsPrimary, controlledInstanceId, controlledIdentityId, controlledBodyId) &&
				currentCreator is not null && currentCreator.Id == commanderInstanceId && currentCreator.CharacterId == commanderIdentityId &&
				currentCreator.BodyId == commanderBodyId;
		}
		catch
		{
			return false;
		}
	}

	internal static bool HasUsableNpcDiagnostic(string? diagnostic) =>
		string.IsNullOrEmpty(diagnostic) || diagnostic.StartsWith("native-createnpc;", StringComparison.Ordinal);
}

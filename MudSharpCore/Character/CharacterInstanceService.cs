using MudSharp.Accounts;
using MudSharp.Arenas;
using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Models;
using MudSharp.NPC;
using MudSharp.NPC.AI;
using MudSharp.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace MudSharp.Character;

public sealed record CharacterInstanceOperationResult(
	bool Success,
	string Message,
	ICharacterInstance? Instance = null
);

public enum SecondaryCharacterInstanceSpawnMode
{
	Passive,
	PlayerFocusable,
	NpcAiControlled,
	ScriptAiControlled
}

public sealed record CharacterInstanceInventoryCloneResult(
	int WornCloned,
	int WieldedCloned,
	int HeldCloned,
	int DroppedCloned,
	int FailedCloned
);

public sealed record SecondaryCharacterInstanceSpawnOptions
{
	public required ICharacter Owner { get; init; }
	public required ICharacterForm Form { get; init; }
	public required ICell Location { get; init; }
	public required RoomLayer RoomLayer { get; init; }
	public CharacterInstanceKind InstanceKind { get; init; } = CharacterInstanceKind.Other;
	public CharacterInstanceControlPolicy ControlPolicy { get; init; } = CharacterInstanceControlPolicy.NotControllable;
	public CharacterInstanceDeathPolicy DeathPolicy { get; init; } = CharacterInstanceDeathPolicy.DestroyInstanceOnly;
	public CharacterInstancePerceptionPolicy PerceptionPolicy { get; init; } =
		CharacterInstancePerceptionPolicy.OrdinaryEmbodied;
	public CharacterInstancePersistencePolicy PersistencePolicy { get; init; } =
		CharacterInstancePersistencePolicy.DespawnOnReboot;
	public CharacterState InitialState { get; init; } = CharacterState.Awake;
	public CharacterStatus InitialStatus { get; init; } = CharacterStatus.Active;
	public string? InstanceName { get; init; }
	public string? EffectData { get; init; }
	public IReadOnlyCollection<IArtificialIntelligence> ArtificialIntelligences { get; init; } =
		Array.Empty<IArtificialIntelligence>();
	public bool CloneInventoryFromPrimary { get; init; }
}

public static class CharacterInstanceService
{
	public static SecondaryCharacterInstanceSpawnOptions CreateSpawnOptionsForMode(
		ICharacter owner,
		ICharacterForm form,
		ICell location,
		RoomLayer roomLayer,
		CharacterInstancePersistencePolicy persistencePolicy,
		SecondaryCharacterInstanceSpawnMode mode)
	{
		var controlPolicy = mode switch
		{
			SecondaryCharacterInstanceSpawnMode.PlayerFocusable => CharacterInstanceControlPolicy.PlayerFocusable,
			SecondaryCharacterInstanceSpawnMode.NpcAiControlled => CharacterInstanceControlPolicy.NpcAiControlled,
			SecondaryCharacterInstanceSpawnMode.ScriptAiControlled => CharacterInstanceControlPolicy.ScriptOnly,
			_ => CharacterInstanceControlPolicy.NotControllable
		};

		return new SecondaryCharacterInstanceSpawnOptions
		{
			Owner = owner,
			Form = form,
			Location = location,
			RoomLayer = roomLayer,
			InstanceKind = mode == SecondaryCharacterInstanceSpawnMode.ScriptAiControlled
				? CharacterInstanceKind.ScriptedAi
				: CharacterInstanceKind.Other,
			ControlPolicy = controlPolicy,
			DeathPolicy = CharacterInstanceDeathPolicy.DestroyInstanceOnly,
			PerceptionPolicy = CharacterInstancePerceptionPolicy.OrdinaryEmbodied,
			PersistencePolicy = persistencePolicy,
			InstanceName = form.Alias
		};
	}

	public static SecondaryCharacterInstanceSpawnOptions CreateScriptedAiSpawnOptions(
		ICharacter owner,
		ICharacterForm form,
		ICell location,
		RoomLayer roomLayer,
		IEnumerable<IArtificialIntelligence>? artificialIntelligences = null,
		bool cloneInventory = false,
		CharacterInstancePersistencePolicy persistencePolicy = CharacterInstancePersistencePolicy.DespawnOnReboot)
	{
		var ais = artificialIntelligences?.Where(x => x is not null).Distinct().ToList() ??
		          new List<IArtificialIntelligence>();
		return new SecondaryCharacterInstanceSpawnOptions
		{
			Owner = owner,
			Form = form,
			Location = location,
			RoomLayer = roomLayer,
			InstanceKind = CharacterInstanceKind.ScriptedAi,
			ControlPolicy = CharacterInstanceControlPolicy.ScriptOnly,
			DeathPolicy = CharacterInstanceDeathPolicy.DestroyInstanceOnly,
			PerceptionPolicy = CharacterInstancePerceptionPolicy.OrdinaryEmbodied,
			PersistencePolicy = persistencePolicy,
			InstanceName = form.Alias,
			ArtificialIntelligences = ais,
			CloneInventoryFromPrimary = cloneInventory,
			EffectData = CharacterInstanceMetadata.CreateScriptedAiEffectData(
				owner.Id,
				owner.InstanceId,
				form.Body.Id,
				form.Alias,
				ais.Select(x => x.Id),
				cloneInventory)
		};
	}

	public static SecondaryCharacterInstanceSpawnOptions CreateAstralProjectionSpawnOptions(
		ICharacter owner,
		ICharacterForm form,
		ICell location,
		RoomLayer roomLayer,
		long planeId,
		AstralProjectionAnchorPolicy anchorPolicy,
		long sourceSpellId,
		string formKey)
	{
		return new SecondaryCharacterInstanceSpawnOptions
		{
			Owner = owner,
			Form = form,
			Location = location,
			RoomLayer = roomLayer,
			InstanceKind = CharacterInstanceKind.AstralProjection,
			ControlPolicy = CharacterInstanceControlPolicy.PlayerFocusable,
			DeathPolicy = CharacterInstanceDeathPolicy.CollapseToAnchor,
			PerceptionPolicy = CharacterInstancePerceptionPolicy.PlanarProjection,
			PersistencePolicy = CharacterInstancePersistencePolicy.DespawnOnReboot,
			InstanceName = form.Alias,
			EffectData = CharacterInstanceMetadata.CreateAstralProjectionEffectData(
				owner.Id,
				owner.InstanceId,
				form.Body.Id,
				planeId,
				anchorPolicy,
				sourceSpellId,
				formKey)
		};
	}

	public static SecondaryCharacterInstanceSpawnOptions CreateMagicalCopySpawnOptions(
		ICharacter owner,
		ICharacterForm form,
		ICell location,
		RoomLayer roomLayer,
		long planeId,
		long sourceSpellId,
		string formKey,
		bool playerFocusable,
		bool intangible,
		CharacterInstancePersistencePolicy persistencePolicy = CharacterInstancePersistencePolicy.DespawnOnReboot)
	{
		return new SecondaryCharacterInstanceSpawnOptions
		{
			Owner = owner,
			Form = form,
			Location = location,
			RoomLayer = roomLayer,
			InstanceKind = CharacterInstanceKind.MagicalCopy,
			ControlPolicy = playerFocusable
				? CharacterInstanceControlPolicy.PlayerFocusable
				: CharacterInstanceControlPolicy.NotControllable,
			DeathPolicy = CharacterInstanceDeathPolicy.CollapseToAnchor,
			PerceptionPolicy = intangible
				? CharacterInstancePerceptionPolicy.PlanarProjection
				: CharacterInstancePerceptionPolicy.OrdinaryEmbodied,
			PersistencePolicy = persistencePolicy,
			InstanceName = form.Alias,
			EffectData = CharacterInstanceMetadata.CreateMagicalCopyEffectData(
				owner.Id,
				owner.InstanceId,
				form.Body.Id,
				planeId,
				sourceSpellId,
				formKey,
				playerFocusable,
				intangible,
				persistencePolicy)
		};
	}

	public static SecondaryCharacterInstanceSpawnOptions CreatePhysicalCloneSpawnOptions(
		ICharacter owner,
		ICharacterForm form,
		ICell location,
		RoomLayer roomLayer,
		long sourceSpellId,
		string formKey,
		bool playerFocusable,
		CharacterInstancePersistencePolicy persistencePolicy = CharacterInstancePersistencePolicy.DespawnOnReboot)
	{
		return new SecondaryCharacterInstanceSpawnOptions
		{
			Owner = owner,
			Form = form,
			Location = location,
			RoomLayer = roomLayer,
			InstanceKind = CharacterInstanceKind.PhysicalClone,
			ControlPolicy = playerFocusable
				? CharacterInstanceControlPolicy.PlayerFocusable
				: CharacterInstanceControlPolicy.NotControllable,
			DeathPolicy = CharacterInstanceDeathPolicy.DestroyInstanceOnly,
			PerceptionPolicy = CharacterInstancePerceptionPolicy.OrdinaryEmbodied,
			PersistencePolicy = persistencePolicy,
			InstanceName = form.Alias,
			EffectData = CharacterInstanceMetadata.CreatePhysicalCloneEffectData(
				owner.Id,
				owner.InstanceId,
				form.Body.Id,
				sourceSpellId,
				formKey,
				playerFocusable,
				persistencePolicy)
		};
	}

	public static SecondaryCharacterInstanceSpawnOptions CreatePossessedBodySpawnOptions(
		ICharacter owner,
		ICharacterForm form,
		ICell location,
		RoomLayer roomLayer,
		long sourceTargetCharacterId,
		long sourceTargetInstanceId,
		long sourceSpellId,
		string formKey,
		CharacterInstancePersistencePolicy persistencePolicy = CharacterInstancePersistencePolicy.DespawnOnReboot)
	{
		return new SecondaryCharacterInstanceSpawnOptions
		{
			Owner = owner,
			Form = form,
			Location = location,
			RoomLayer = roomLayer,
			InstanceKind = CharacterInstanceKind.PossessedBody,
			ControlPolicy = CharacterInstanceControlPolicy.PlayerFocusable,
			DeathPolicy = CharacterInstanceDeathPolicy.CollapseToAnchor,
			PerceptionPolicy = CharacterInstancePerceptionPolicy.OrdinaryEmbodied,
			PersistencePolicy = persistencePolicy,
			InstanceName = form.Alias,
			EffectData = CharacterInstanceMetadata.CreatePossessedBodyEffectData(
				owner.Id,
				owner.InstanceId,
				form.Body.Id,
				sourceTargetCharacterId,
				sourceTargetInstanceId,
				sourceSpellId,
				formKey,
				persistencePolicy)
		};
	}

	public static CharacterInstanceOperationResult SpawnPossessedCorpseInstance(
		ICharacter owner,
		IBody body,
		ICell location,
		RoomLayer roomLayer,
		long anchorCharacterId,
		long anchorInstanceId,
		long corpseItemId,
		long sourceSpellId,
		CharacterInstancePersistencePolicy persistencePolicy = CharacterInstancePersistencePolicy.TemporaryEffectBound)
	{
		if (owner.Identity is not Character identity)
		{
			return new CharacterInstanceOperationResult(false, "The corpse's original character is not loaded.");
		}

		if (body is null)
		{
			return new CharacterInstanceOperationResult(false, "The corpse does not have an original body.");
		}

		if (location is null)
		{
			return new CharacterInstanceOperationResult(false, "There is no valid location for the possessed corpse.");
		}

		foreach (var loaded in identity.Instances.OfType<Character>()
		                                .Where(x => ReferenceEquals(x.Body, body) && x.IsEmbodied)
		                                .ToList())
		{
			if (!loaded.State.IsDead() && !loaded.State.HasFlag(CharacterState.Stasis))
			{
				return new CharacterInstanceOperationResult(false,
					"That corpse body already has a live embodied instance.");
			}

			loaded.SetInstanceEmbodied(false);
			loaded.SetInstanceControllable(false);
			loaded.Save();
		}

		using (new FMDB())
		{
			var dbchar = FMDB.Context.Characters.Find(identity.Id);
			if (dbchar is null)
			{
				return new CharacterInstanceOperationResult(false,
					"The corpse's original character could not be found in the database.");
			}

			foreach (var existing in FMDB.Context.CharacterInstances
			                             .Where(x => x.BodyId == body.Id && x.IsEmbodied)
			                             .ToList())
			{
				var state = (CharacterState)existing.State;
				if (!state.IsDead() && !state.HasFlag(CharacterState.Stasis))
				{
					return new CharacterInstanceOperationResult(false,
						"That corpse body already has a live embodied instance.");
				}

				existing.IsEmbodied = false;
				existing.IsControllable = false;
			}

			var dbinstance = new MudSharp.Models.CharacterInstance
			{
				CharacterId = identity.Id,
				Character = dbchar,
				BodyId = body.Id,
				InstanceName = "possessed corpse",
				InstanceKind = (int)CharacterInstanceKind.PossessedCorpse,
				ControlPolicy = (int)CharacterInstanceControlPolicy.PlayerRemoteCommandable,
				DeathPolicy = (int)CharacterInstanceDeathPolicy.CollapseToAnchor,
				PerceptionPolicy = (int)CharacterInstancePerceptionPolicy.OrdinaryEmbodied,
				PersistencePolicy = (int)persistencePolicy,
				LocationId = location.Id,
				RoomLayer = (int)roomLayer,
				PositionId = (int)PositionStanding.Instance.Id,
				PositionModifier = (int)PositionModifier.None,
				PositionEmote = string.Empty,
				State = (int)CharacterState.Awake,
				Status = (int)CharacterStatus.Active,
				IsPrimary = false,
				IsEmbodied = true,
				IsControllable = true,
				CreatedDateTime = DateTime.UtcNow,
				EffectData = CharacterInstanceMetadata.CreatePossessedCorpseEffectData(
					anchorCharacterId,
					anchorInstanceId,
					corpseItemId,
					CharacterInstanceIdentityComparer.IdentityId(owner),
					body.Id,
					sourceSpellId,
					persistencePolicy)
			};
			FMDB.Context.CharacterInstances.Add(dbinstance);
			FMDB.Context.SaveChanges();

			var materialised = identity.MaterialiseSecondaryInstance(dbinstance, body);
			return new CharacterInstanceOperationResult(true, "Possessed corpse instance spawned.", materialised);
		}
	}

	public static CharacterInstanceOperationResult SpawnAnimatedCorpseInstance(
		ICharacter owner,
		IBody body,
		ICell location,
		RoomLayer roomLayer,
		long anchorCharacterId,
		long anchorInstanceId,
		long corpseItemId,
		long sourceSpellId,
		IEnumerable<IArtificialIntelligence> artificialIntelligences,
		CharacterInstancePersistencePolicy persistencePolicy = CharacterInstancePersistencePolicy.TemporaryEffectBound)
	{
		if (owner.Identity is not Character identity)
		{
			return new CharacterInstanceOperationResult(false, "The corpse's original character is not loaded.");
		}

		if (body is null)
		{
			return new CharacterInstanceOperationResult(false, "The corpse does not have an original body.");
		}

		if (location is null)
		{
			return new CharacterInstanceOperationResult(false, "There is no valid location for the animated corpse.");
		}

		var ais = artificialIntelligences.Where(x => x is not null).Distinct().ToList();
		foreach (var loaded in identity.Instances.OfType<Character>()
		                                .Where(x => ReferenceEquals(x.Body, body) && x.IsEmbodied)
		                                .ToList())
		{
			if (!loaded.State.IsDead() && !loaded.State.HasFlag(CharacterState.Stasis))
			{
				return new CharacterInstanceOperationResult(false,
					"That corpse body already has a live embodied instance.");
			}

			loaded.SetInstanceEmbodied(false);
			loaded.SetInstanceControllable(false);
			loaded.Save();
		}

		using (new FMDB())
		{
			var dbchar = FMDB.Context.Characters.Find(identity.Id);
			if (dbchar is null)
			{
				return new CharacterInstanceOperationResult(false,
					"The corpse's original character could not be found in the database.");
			}

			foreach (var existing in FMDB.Context.CharacterInstances
			                             .Where(x => x.BodyId == body.Id && x.IsEmbodied)
			                             .ToList())
			{
				var state = (CharacterState)existing.State;
				if (!state.IsDead() && !state.HasFlag(CharacterState.Stasis))
				{
					return new CharacterInstanceOperationResult(false,
						"That corpse body already has a live embodied instance.");
				}

				existing.IsEmbodied = false;
				existing.IsControllable = false;
			}

			var dbinstance = new MudSharp.Models.CharacterInstance
			{
				CharacterId = identity.Id,
				Character = dbchar,
				BodyId = body.Id,
				InstanceName = "animated corpse",
				InstanceKind = (int)CharacterInstanceKind.AnimatedCorpse,
				ControlPolicy = (int)CharacterInstanceControlPolicy.ScriptOnly,
				DeathPolicy = (int)CharacterInstanceDeathPolicy.CollapseToAnchor,
				PerceptionPolicy = (int)CharacterInstancePerceptionPolicy.OrdinaryEmbodied,
				PersistencePolicy = (int)persistencePolicy,
				LocationId = location.Id,
				RoomLayer = (int)roomLayer,
				PositionId = (int)PositionStanding.Instance.Id,
				PositionModifier = (int)PositionModifier.None,
				PositionEmote = string.Empty,
				State = (int)CharacterState.Awake,
				Status = (int)CharacterStatus.Active,
				IsPrimary = false,
				IsEmbodied = true,
				IsControllable = true,
				CreatedDateTime = DateTime.UtcNow,
				EffectData = CharacterInstanceMetadata.CreateAnimatedCorpseEffectData(
					anchorCharacterId,
					anchorInstanceId,
					corpseItemId,
					CharacterInstanceIdentityComparer.IdentityId(owner),
					body.Id,
					sourceSpellId,
					ais.Select(x => x.Id),
					persistencePolicy)
			};
			FMDB.Context.CharacterInstances.Add(dbinstance);
			FMDB.Context.SaveChanges();

			var materialised = identity.MaterialiseSecondaryInstance(dbinstance, body);
			return new CharacterInstanceOperationResult(true, "Animated corpse instance spawned.", materialised);
		}
	}

	public static CharacterInstanceOperationResult ValidateSecondarySpawnMode(ICharacter owner,
		SecondaryCharacterInstanceSpawnMode mode)
	{
		switch (mode)
		{
			case SecondaryCharacterInstanceSpawnMode.PlayerFocusable:
				return owner.IsPlayerCharacter && !owner.IsGuest
					? new CharacterInstanceOperationResult(true, string.Empty)
					: new CharacterInstanceOperationResult(false,
						"Only player characters can have player-focusable secondary instances.");
			case SecondaryCharacterInstanceSpawnMode.NpcAiControlled:
				return owner is INPC || owner.Identity is INPC
					? new CharacterInstanceOperationResult(true, string.Empty)
					: new CharacterInstanceOperationResult(false,
						"Only NPC identities can have AI-controlled secondary instances.");
			case SecondaryCharacterInstanceSpawnMode.ScriptAiControlled:
				return new CharacterInstanceOperationResult(true, string.Empty);
			case SecondaryCharacterInstanceSpawnMode.Passive:
			default:
				return new CharacterInstanceOperationResult(true, string.Empty);
		}
	}

	public static bool CanStaffPossessNpcTarget(ICharacter target)
	{
		return !target.IsPlayerCharacter && !target.Identity.PrimaryInstance.IsPlayerCharacter;
	}

	public static CharacterInstanceOperationResult ValidateSecondarySpawnOptions(
		SecondaryCharacterInstanceSpawnOptions options)
	{
		if (options.ControlPolicy == CharacterInstanceControlPolicy.PlayerFocusable)
		{
			return options.Owner.IsPlayerCharacter && !options.Owner.IsGuest
				? new CharacterInstanceOperationResult(true, string.Empty)
				: new CharacterInstanceOperationResult(false,
					"Only player characters can have player-focusable secondary instances.");
		}

		if (options.ControlPolicy == CharacterInstanceControlPolicy.NpcAiControlled)
		{
			return options.Owner is INPC || options.Owner.Identity is INPC
				? new CharacterInstanceOperationResult(true, string.Empty)
				: new CharacterInstanceOperationResult(false,
					"Only NPC identities can have AI-controlled secondary instances.");
		}

		if (options.ControlPolicy == CharacterInstanceControlPolicy.ScriptOnly &&
		    options.InstanceKind is CharacterInstanceKind.ScriptedAi or CharacterInstanceKind.AnimatedCorpse)
		{
			return new CharacterInstanceOperationResult(true, string.Empty);
		}

		return new CharacterInstanceOperationResult(true, string.Empty);
	}

	public static CharacterInstanceOperationResult SpawnPassiveInstance(
		ICharacter owner,
		ICharacterForm form,
		ICell location,
		RoomLayer roomLayer,
		CharacterInstancePersistencePolicy persistencePolicy,
		bool playerFocusable = false)
	{
		return SpawnSecondaryInstance(
			CreateSpawnOptionsForMode(
				owner,
				form,
				location,
				roomLayer,
				persistencePolicy,
				playerFocusable
					? SecondaryCharacterInstanceSpawnMode.PlayerFocusable
					: SecondaryCharacterInstanceSpawnMode.Passive));
	}

	public static CharacterInstanceOperationResult SpawnSecondaryInstance(
		ICharacter owner,
		ICharacterForm form,
		ICell location,
		RoomLayer roomLayer,
		CharacterInstancePersistencePolicy persistencePolicy,
		SecondaryCharacterInstanceSpawnMode mode)
	{
		return SpawnSecondaryInstance(CreateSpawnOptionsForMode(owner, form, location, roomLayer, persistencePolicy,
			mode));
	}

	public static CharacterInstanceOperationResult SpawnBodyInstance(
		ICharacter owner,
		ICharacterForm form,
		ICell location,
		RoomLayer roomLayer,
		SecondaryCharacterInstanceSpawnMode mode,
		CharacterInstancePersistencePolicy persistencePolicy = CharacterInstancePersistencePolicy.DespawnOnReboot,
		IEnumerable<IArtificialIntelligence>? artificialIntelligences = null,
		bool cloneInventory = false)
	{
		var ais = artificialIntelligences?.Where(x => x is not null).Distinct().ToList() ??
		          new List<IArtificialIntelligence>();
		var options = mode == SecondaryCharacterInstanceSpawnMode.ScriptAiControlled
			? CreateScriptedAiSpawnOptions(owner, form, location, roomLayer, ais, cloneInventory,
				persistencePolicy)
			: CreateSpawnOptionsForMode(owner, form, location, roomLayer, persistencePolicy, mode) with
			{
				ArtificialIntelligences = ais,
				CloneInventoryFromPrimary = cloneInventory
			};

		return SpawnSecondaryInstance(options);
	}

	public static CharacterInstanceOperationResult SpawnSecondaryInstance(
		SecondaryCharacterInstanceSpawnOptions options)
	{
		var owner = options.Owner;
		var form = options.Form;
		var location = options.Location;
		if (owner.Identity is not Character identity)
		{
			return new CharacterInstanceOperationResult(false, "The target character is not loaded in this game world.");
		}

		if (form is null || !identity.Forms.Any(x => ReferenceEquals(x, form)))
		{
			return new CharacterInstanceOperationResult(false, "That form does not belong to the target identity.");
		}

		if (ReferenceEquals(form.Body, identity.CurrentBody))
		{
			return new CharacterInstanceOperationResult(false, "You cannot spawn a secondary instance from the primary body.");
		}

		if (identity.Instances.Any(x => x.IsEmbodied && ReferenceEquals(x.Body, form.Body)))
		{
			return new CharacterInstanceOperationResult(false, "That form already has a live embodied instance.");
		}

		if (location is null)
		{
			return new CharacterInstanceOperationResult(false, "There is no valid location for the instance.");
		}

		var modeValidation = ValidateSecondarySpawnOptions(options);
		if (!modeValidation.Success)
		{
			return modeValidation;
		}

		using (new FMDB())
		{
			var dbchar = FMDB.Context.Characters.Find(identity.Id);
			if (dbchar is null)
			{
				return new CharacterInstanceOperationResult(false, "The target identity could not be found in the database.");
			}

			var dbinstance = new MudSharp.Models.CharacterInstance
			{
				CharacterId = identity.Id,
				Character = dbchar,
				BodyId = form.Body.Id,
				InstanceName = options.InstanceName ?? form.Alias,
				InstanceKind = (int)options.InstanceKind,
				ControlPolicy = (int)options.ControlPolicy,
				DeathPolicy = (int)options.DeathPolicy,
				PerceptionPolicy = (int)options.PerceptionPolicy,
				PersistencePolicy = (int)options.PersistencePolicy,
				LocationId = location.Id,
				RoomLayer = (int)options.RoomLayer,
				PositionId = (int)PositionStanding.Instance.Id,
				PositionModifier = (int)PositionModifier.None,
				PositionEmote = string.Empty,
				State = (int)options.InitialState,
				Status = (int)options.InitialStatus,
				IsPrimary = false,
				IsEmbodied = true,
				IsControllable = options.ControlPolicy != CharacterInstanceControlPolicy.NotControllable,
				CreatedDateTime = DateTime.UtcNow,
				EffectData = options.EffectData.IfNullOrWhiteSpace("<Effects/>")
			};
			FMDB.Context.CharacterInstances.Add(dbinstance);
			FMDB.Context.SaveChanges();

			var materialised = identity.MaterialiseSecondaryInstance(dbinstance, form.Body);
			if (options.CloneInventoryFromPrimary)
			{
				CloneInventory(owner, materialised, out _);
			}

			return new CharacterInstanceOperationResult(true, "Secondary instance spawned.", materialised);
		}
	}

	public static bool CloneInventory(
		ICharacter source,
		ICharacterInstance target,
		out CharacterInstanceInventoryCloneResult result)
	{
		var worn = 0;
		var wielded = 0;
		var held = 0;
		var dropped = 0;
		var failed = 0;

		if (source?.Body is null || target?.Body is null || target.Location is null)
		{
			result = new CharacterInstanceInventoryCloneResult(0, 0, 0, 0, 0);
			return false;
		}

		var wornItems = source.Body.DirectWornItems
		                      .Distinct()
		                      .Select(x => (Item: x, Profile: x.GetItemType<IWearable>()?.CurrentProfile))
		                      .Where(x => x.Profile is not null)
		                      .ToList();
		var wieldedItems = source.Body.WieldedItems
		                         .Distinct()
		                         .Select(x => (Item: x,
			                         Flags: source.Body.WieldedHandCount(x) > 1
				                         ? ItemCanWieldFlags.RequireTwoHands
				                         : ItemCanWieldFlags.None))
		                         .ToList();
		var heldItems = source.Body.HeldItems
		                      .Distinct()
		                      .Except(wieldedItems.Select(x => x.Item))
		                      .ToList();

		foreach (var item in wornItems)
		{
			var clone = CloneInventoryItem(item.Item);
			if (clone is null)
			{
				failed++;
				continue;
			}

			if (!TryPutCloneInHands(target, clone))
			{
				DropCloneInRoom(target, clone);
				dropped++;
				continue;
			}

			if (target.Body.CanWear(clone, item.Profile!))
			{
				target.Body.Wear(clone, item.Profile!, silent: true);
				worn++;
			}
			else if (target.Body.CanWear(clone))
			{
				target.Body.Wear(clone, silent: true);
				worn++;
			}
			else
			{
				if (DropIfNotCarried(target, clone))
				{
					dropped++;
					failed++;
				}
				else
				{
					held++;
				}
			}
		}

		foreach (var item in wieldedItems)
		{
			var clone = CloneInventoryItem(item.Item);
			if (clone is null)
			{
				failed++;
				continue;
			}

			if (!TryPutCloneInHands(target, clone))
			{
				DropCloneInRoom(target, clone);
				dropped++;
				continue;
			}

			if (target.Body.CanWield(clone, item.Flags) && target.Body.Wield(clone, silent: true, flags: item.Flags))
			{
				wielded++;
			}
			else
			{
				held++;
			}
		}

		foreach (var item in heldItems)
		{
			var clone = CloneInventoryItem(item);
			if (clone is null)
			{
				failed++;
				continue;
			}

			if (TryPutCloneInHands(target, clone))
			{
				held++;
			}
			else
			{
				DropCloneInRoom(target, clone);
				dropped++;
			}
		}

		target.Body.RecalculateItemHelpers();
		target.Changed = true;
		result = new CharacterInstanceInventoryCloneResult(worn, wielded, held, dropped, failed);
		return failed == 0;
	}

	private static IGameItem? CloneInventoryItem(IGameItem item)
	{
		return item.DeepCopy(addToGameworld: true, preserveMorphTime: true);
	}

	private static bool TryPutCloneInHands(ICharacterInstance target, IGameItem clone)
	{
		DropCloneInRoom(target, clone);
		target.Body.Get(clone, silent: true, ignoreFlags: ItemCanGetIgnore.IgnoreWeight);
		return target.Body.HeldOrWieldedItems.Contains(clone) || target.Body.WornItems.Contains(clone);
	}

	private static void DropCloneInRoom(ICharacterInstance target, IGameItem clone)
	{
		clone.Location?.Extract(clone);
		clone.RoomLayer = target.RoomLayer;
		target.Location?.Insert(clone);
	}

	private static bool DropIfNotCarried(ICharacterInstance target, IGameItem clone)
	{
		if (target.Body.AllItems.Contains(clone))
		{
			return false;
		}

		DropCloneInRoom(target, clone);
		return true;
	}

	public static CharacterInstanceOperationResult Move(ICharacterInstance instance, ICell location, RoomLayer roomLayer)
	{
		if (instance is not Character secondary || instance.IsPrimaryInstance)
		{
			return new CharacterInstanceOperationResult(false, "Only secondary instances can be moved with this command.");
		}

		if (!secondary.IsEmbodied)
		{
			return new CharacterInstanceOperationResult(false, "That instance is not currently embodied.");
		}

		if (location is null)
		{
			return new CharacterInstanceOperationResult(false, "There is no valid destination location.");
		}

		secondary.Combat?.LeaveCombat(secondary);
		secondary.Movement?.CancelForMoverOnly(secondary);
		secondary.CombatTarget = null;
		secondary.Location?.Leave(secondary);
		location.Enter(secondary, noSave: true, roomLayer: roomLayer);
		secondary.SetPosition(PositionStanding.Instance, PositionModifier.None, null, null);
		secondary.Changed = true;
		secondary.Save();
		return new CharacterInstanceOperationResult(true, "Secondary instance moved.", secondary);
	}

	public static CharacterInstanceOperationResult RestorePersistentSecondaryInstance(
		ICharacter owner,
		long instanceId,
		ICell location,
		RoomLayer roomLayer)
	{
		if (owner.Identity is not Character identity)
		{
			return new CharacterInstanceOperationResult(false, "The owner identity is not loaded.");
		}

		var loaded = identity.Instances
		                     .OfType<ICharacterInstance>()
		                     .FirstOrDefault(x => x.InstanceId == instanceId);
		if (loaded is not null)
		{
			if (loaded.IsPrimaryInstance)
			{
				return new CharacterInstanceOperationResult(false, "Primary instances cannot be restored as secondary mounts.");
			}

			if (loaded is Character loadedCharacter)
			{
				loadedCharacter.Location?.Leave(loadedCharacter);
				location.Enter(loadedCharacter, noSave: true, roomLayer: roomLayer);
				loadedCharacter.SetInstanceEmbodied(true);
				loadedCharacter.SetInstanceControllable(loadedCharacter.ControlPolicy != CharacterInstanceControlPolicy.NotControllable);
				loadedCharacter.SetInstanceStateAndStatus(loadedCharacter.State & ~CharacterState.Stasis, loadedCharacter.Status);
				loadedCharacter.Save();
			}

			return new CharacterInstanceOperationResult(true, "Secondary instance restored.", loaded);
		}

		using (new FMDB())
		{
			var dbinstance = FMDB.Context.CharacterInstances.Find(instanceId);
			if (dbinstance is null || dbinstance.CharacterId != identity.Id)
			{
				return new CharacterInstanceOperationResult(false, "The secondary instance could not be found.");
			}

			if (dbinstance.IsPrimary)
			{
				return new CharacterInstanceOperationResult(false, "Primary instances cannot be restored as secondary mounts.");
			}

			if ((CharacterInstancePersistencePolicy)dbinstance.PersistencePolicy != CharacterInstancePersistencePolicy.Persistent)
			{
				return new CharacterInstanceOperationResult(false, "Only persistent secondary instances can be restored from stabling.");
			}

			var state = (CharacterState)dbinstance.State;
			if (state.IsDead())
			{
				return new CharacterInstanceOperationResult(false, "Dead secondary instances cannot be restored from stabling.");
			}

			var body = identity.Bodies.FirstOrDefault(x => x.Id == dbinstance.BodyId);
			if (body is null)
			{
				return new CharacterInstanceOperationResult(false, "The secondary instance body could not be found.");
			}

			dbinstance.LocationId = location.Id;
			dbinstance.RoomLayer = (int)roomLayer;
			dbinstance.IsEmbodied = true;
			dbinstance.IsControllable = (CharacterInstanceControlPolicy)dbinstance.ControlPolicy !=
			                            CharacterInstanceControlPolicy.NotControllable;
			dbinstance.State = (int)(state & ~CharacterState.Stasis);
			FMDB.Context.SaveChanges();

			var materialised = identity.MaterialiseSecondaryInstance(dbinstance, body);
			return new CharacterInstanceOperationResult(true, "Secondary instance restored.", materialised);
		}
	}

	public static bool Retire(ICharacterInstance instance, out string whyNot, bool deleteTemporaryRows = true,
		bool deathRetirement = false, bool removeOwningEffects = true)
	{
		if (instance is not Character secondary)
		{
			whyNot = "There is no such secondary instance.";
			return false;
		}

		if (secondary.IsPrimaryInstance)
		{
			whyNot = "Primary instances cannot be retired.";
			return false;
		}

		var owner = secondary.Identity as Character;
		if (removeOwningEffects &&
		    owner?.RemoveAllEffects<IAstralProjectionEffect>(x => x.ProjectionInstanceId == secondary.InstanceId,
			    true) == true)
		{
			whyNot = string.Empty;
			return true;
		}

		if (removeOwningEffects &&
		    owner?.RemoveAllEffects<IMagicalCopyEffect>(x => x.CopyInstanceId == secondary.InstanceId,
			    true) == true)
		{
			whyNot = string.Empty;
			return true;
		}

		if (removeOwningEffects &&
		    owner?.RemoveAllEffects<IPhysicalCloneEffect>(x => x.CloneInstanceId == secondary.InstanceId,
			    true) == true)
		{
			whyNot = string.Empty;
			return true;
		}

		if (removeOwningEffects && RemovePossessedBodyEffectsForShell(secondary))
		{
			whyNot = string.Empty;
			return true;
		}

		if (removeOwningEffects && PossessionControlService.RemoveCorpsePossessionEffectsForAnimatedInstance(secondary))
		{
			whyNot = string.Empty;
			return true;
		}

		if (removeOwningEffects && PossessionControlService.RemoveAnimatedCorpseEffectsForAnimatedInstance(secondary))
		{
			whyNot = string.Empty;
			return true;
		}

		CharacterInstanceFocusService.TryReturnFocusToPrimary(
			secondary,
			"Your focus returns to your primary body as the secondary instance fades from control.",
			true,
			suppressAutoLook: true);

		LeaveCurrentProject(secondary);
		ClearVehicleBindings(secondary, deletePersistentHitches: true);
		ClearArenaBindings(secondary);

		if (secondary is ScriptedAiCharacterInstance scriptedAi)
		{
			scriptedAi.ReleaseEventSubscriptions();
		}
		else if (secondary is INPC npc)
		{
			npc.ReleaseEventSubscriptions();
		}

		secondary.Combat?.LeaveCombat(secondary);
		secondary.Movement?.CancelForMoverOnly(secondary);
		secondary.CombatTarget = null;
		secondary.PositionTarget = null!;
		secondary.Gameworld.EffectScheduler.Destroy(secondary, true);
		secondary.Gameworld.Scheduler.Destroy(secondary);
		secondary.Location?.Leave(secondary);
		secondary.ClearInstanceLocation();
		secondary.SetInstanceEmbodied(false);
		secondary.SetInstanceControllable(false);
		if (!deathRetirement)
		{
			secondary.SetInstanceStateAndStatus(CharacterState.Stasis, CharacterStatus.Active);
			secondary.Body.SuspendForCharacter();
		}

		if (secondary.CharacterController is not null)
		{
			secondary.LoseControl(secondary.CharacterController);
		}

		if (owner is not null)
		{
			secondary.Body.Actor = owner;
			owner.ForgetSecondaryInstance(secondary);
		}

		using (new FMDB())
		{
			var dbinstance = FMDB.Context.CharacterInstances.Find(secondary.InstanceId);
			if (dbinstance is not null)
			{
				if (deleteTemporaryRows &&
				    secondary.PersistencePolicy is
					    CharacterInstancePersistencePolicy.DespawnOnReboot or
					    CharacterInstancePersistencePolicy.DespawnOnLogout or
					    CharacterInstancePersistencePolicy.TemporaryEffectBound)
				{
					FMDB.Context.CharacterInstances.Remove(dbinstance);
				}
				else
				{
					secondary.SaveSecondaryInstance(dbinstance);
					dbinstance.LocationId = null;
					dbinstance.IsEmbodied = false;
					dbinstance.IsControllable = false;
				}

				FMDB.Context.SaveChanges();
			}
		}

		secondary.Changed = false;
		whyNot = string.Empty;
		return true;
	}

	public static bool AnyPossessedBodyEffectsForAnchor(ICharacter anchor,
		Predicate<IPossessedBodyEffect>? predicate = null)
	{
		return PossessedBodyEffectOwners(anchor.Gameworld)
		       .Any(x => x.EffectsOfType<IPossessedBodyEffect>(effect =>
			       PossessedBodyEffectMatchesAnchor(effect, anchor) &&
			       (predicate?.Invoke(effect) ?? true)).Any());
	}

	public static bool RemovePossessedBodyEffectsForAnchor(ICharacter anchor,
		Predicate<IPossessedBodyEffect>? predicate = null)
	{
		return RemovePossessedBodyEffects(anchor.Gameworld, effect =>
			PossessedBodyEffectMatchesAnchor(effect, anchor) &&
			(predicate?.Invoke(effect) ?? true));
	}

	public static bool RemovePossessedBodyEffectsForShell(ICharacterInstance shell)
	{
		return RemovePossessedBodyEffects(shell.Gameworld, effect => effect.ShellInstanceId == shell.InstanceId);
	}

	private static IEnumerable<ICharacter> PossessedBodyEffectOwners(IFuturemud gameworld)
	{
		return gameworld.Actors
		                .Concat(gameworld.CachedActors)
		                .Where(x => x is not null)
		                .Distinct()
		                .ToList();
	}

	private static bool RemovePossessedBodyEffects(IFuturemud gameworld, Predicate<IPossessedBodyEffect> predicate)
	{
		var removed = false;
		foreach (var actor in PossessedBodyEffectOwners(gameworld))
		{
			removed |= actor.RemoveAllEffects(predicate, true);
		}

		return removed;
	}

	private static bool PossessedBodyEffectMatchesAnchor(IPossessedBodyEffect effect, ICharacter anchor)
	{
		return effect.AnchorCharacterId == CharacterInstanceIdentityComparer.IdentityId(anchor) &&
		       (effect.AnchorInstanceId == 0 || effect.AnchorInstanceId == anchor.InstanceId);
	}

	public static void UnloadLoadedSecondariesForOwnerLogout(ICharacter owner)
	{
		if (owner.Identity is not Character identity)
		{
			return;
		}

		foreach (var instance in identity.Instances
		                                .Where(x => !x.IsPrimaryInstance)
		                                .OfType<Character>()
		                                .ToList())
		{
			if (instance.PersistencePolicy == CharacterInstancePersistencePolicy.Persistent)
			{
				UnloadPersistentSecondary(identity, instance);
				continue;
			}

			Retire(instance, out _, deleteTemporaryRows: true, removeOwningEffects: false);
		}

		identity.SetFocusedInstance(null);
	}

	private static void UnloadPersistentSecondary(Character owner, Character secondary)
	{
		CharacterInstanceFocusService.TryReturnFocusToPrimary(secondary, string.Empty, false, suppressAutoLook: true);
		LeaveCurrentProject(secondary);
		ClearArenaBindings(secondary);
		if (secondary is ScriptedAiCharacterInstance scriptedAi)
		{
			scriptedAi.ReleaseEventSubscriptions();
		}
		else if (secondary is INPC npc)
		{
			npc.ReleaseEventSubscriptions();
		}

		secondary.Combat?.LeaveCombat(secondary);
		secondary.Movement?.CancelForMoverOnly(secondary);
		secondary.CombatTarget = null;
		secondary.PositionTarget = null!;
		secondary.Save();
		secondary.Gameworld.EffectScheduler.Destroy(secondary, true);
		secondary.Gameworld.Scheduler.Destroy(secondary);
		secondary.Location?.Leave(secondary);
		if (secondary.CharacterController is not null)
		{
			secondary.LoseControl(secondary.CharacterController);
		}

		secondary.Body.SuspendForCharacter();
		secondary.Body.Actor = owner;
		owner.ForgetSecondaryInstance(secondary);
		secondary.Changed = false;
	}

	private static void ClearVehicleBindings(Character secondary, bool deletePersistentHitches)
	{
		foreach (var vehicle in secondary.Gameworld.Vehicles.Where(x => x.IsOccupant(secondary)).ToList())
		{
			vehicle.ForceDisembark(secondary);
		}

		if (!deletePersistentHitches)
		{
			return;
		}

		var hitchService = new VehicleHitchService();
		foreach (var link in hitchService.LinksInvolving(secondary.Gameworld, secondary).ToList())
		{
			hitchService.DeletePersistentLink(secondary.Gameworld, link.Id);
		}
	}

	private static void ClearArenaBindings(Character secondary)
	{
		var arenaEvents = secondary.Gameworld.CombatArenas
		                           .SelectMany(x => x.ActiveEvents)
		                           .Where(x => x.Participants.Any(y => y.ActiveCharacter is { } active &&
		                                                               CharacterInstanceIdentityComparer
			                                                               .SamePhysicalInstance(secondary, active)))
		                           .ToList();
		foreach (var arenaEvent in arenaEvents)
		{
			try
			{
				switch (arenaEvent.State)
				{
					case ArenaEventState.RegistrationOpen:
						arenaEvent.Withdraw(secondary);
						break;
					case ArenaEventState.Live:
						var surrender = arenaEvent.CanSurrender(secondary);
						if (surrender.Truth)
						{
							arenaEvent.Surrender(secondary);
						}

						break;
				}
			}
			catch
			{
				// Retire must still proceed; stale arena references are reported by instance audit.
			}

			secondary.Gameworld.ArenaParticipationService.ClearParticipation(secondary, arenaEvent);
		}

		secondary.RemoveAllEffects(effect =>
			effect.IsEffectType<ArenaStagingEffect>() ||
			effect.IsEffectType<ArenaPreparingEffect>() ||
			effect.IsEffectType<ArenaParticipationEffect>() ||
			effect.IsEffectType<ArenaParticipantPreparationEffect>() ||
			effect.IsEffectType<ArenaNpcPreparationEffect>());
	}

	private static void LeaveCurrentProject(Character secondary)
	{
		if (secondary.CurrentProject.Project is null)
		{
			return;
		}

		secondary.CurrentProject.Project.Leave(secondary);
		secondary.CurrentProjectHours = 0.0;
		secondary.CurrentProjectProjectHours = 0.0;
	}
}

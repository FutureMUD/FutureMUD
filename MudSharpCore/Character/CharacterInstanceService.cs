using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Models;
using MudSharp.NPC;
using System;
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
	NpcAiControlled
}

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
			_ => CharacterInstanceControlPolicy.NotControllable
		};

		return new SecondaryCharacterInstanceSpawnOptions
		{
			Owner = owner,
			Form = form,
			Location = location,
			RoomLayer = roomLayer,
			InstanceKind = CharacterInstanceKind.Other,
			ControlPolicy = controlPolicy,
			DeathPolicy = CharacterInstanceDeathPolicy.DestroyInstanceOnly,
			PerceptionPolicy = CharacterInstancePerceptionPolicy.OrdinaryEmbodied,
			PersistencePolicy = persistencePolicy,
			InstanceName = form.Alias
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
			case SecondaryCharacterInstanceSpawnMode.Passive:
			default:
				return new CharacterInstanceOperationResult(true, string.Empty);
		}
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
			return new CharacterInstanceOperationResult(true, "Secondary instance spawned.", materialised);
		}
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

		CharacterInstanceFocusService.TryReturnFocusToPrimary(
			secondary,
			"Your focus returns to your primary body as the secondary instance fades from control.",
			true);

		if (secondary is INPC npc)
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
		CharacterInstanceFocusService.TryReturnFocusToPrimary(secondary, string.Empty, false);
		if (secondary is INPC npc)
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
}

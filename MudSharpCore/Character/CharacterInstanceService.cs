using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Construction;
using MudSharp.Database;
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

public static class CharacterInstanceService
{
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

	public static CharacterInstanceOperationResult SpawnPassiveInstance(
		ICharacter owner,
		ICharacterForm form,
		ICell location,
		RoomLayer roomLayer,
		CharacterInstancePersistencePolicy persistencePolicy,
		bool playerFocusable = false)
	{
		return SpawnSecondaryInstance(
			owner,
			form,
			location,
			roomLayer,
			persistencePolicy,
			playerFocusable
				? SecondaryCharacterInstanceSpawnMode.PlayerFocusable
				: SecondaryCharacterInstanceSpawnMode.Passive);
	}

	public static CharacterInstanceOperationResult SpawnSecondaryInstance(
		ICharacter owner,
		ICharacterForm form,
		ICell location,
		RoomLayer roomLayer,
		CharacterInstancePersistencePolicy persistencePolicy,
		SecondaryCharacterInstanceSpawnMode mode)
	{
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

		var modeValidation = ValidateSecondarySpawnMode(owner, mode);
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

			var controlPolicy = mode switch
			{
				SecondaryCharacterInstanceSpawnMode.PlayerFocusable => CharacterInstanceControlPolicy.PlayerFocusable,
				SecondaryCharacterInstanceSpawnMode.NpcAiControlled => CharacterInstanceControlPolicy.NpcAiControlled,
				_ => CharacterInstanceControlPolicy.NotControllable
			};
			var dbinstance = new MudSharp.Models.CharacterInstance
			{
				CharacterId = identity.Id,
				Character = dbchar,
				BodyId = form.Body.Id,
				InstanceName = form.Alias,
				InstanceKind = (int)CharacterInstanceKind.Other,
				ControlPolicy = (int)controlPolicy,
				DeathPolicy = (int)CharacterInstanceDeathPolicy.DestroyInstanceOnly,
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
				IsControllable = controlPolicy != CharacterInstanceControlPolicy.NotControllable,
				CreatedDateTime = DateTime.UtcNow,
				EffectData = "<Effects/>"
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
		bool deathRetirement = false)
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

		var owner = secondary.Identity as Character;
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
}

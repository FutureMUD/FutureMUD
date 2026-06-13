using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Accounts;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable

namespace MudSharp.Character;

public sealed class PassiveCharacterInstance : Character
{
	private readonly Character _identity;

	internal PassiveCharacterInstance(Character identity, MudSharp.Models.CharacterInstance instance, IBody body)
		: base(identity, instance, body)
	{
		_identity = identity;
	}

	public override ICharacterIdentity Identity => _identity;
	public override IEnumerable<ICharacterInstance> Instances => _identity.Instances;
	public override ICharacterInstance PrimaryInstance => _identity.PrimaryInstance;
	public override ICharacterInstance? FocusedInstance => _identity.FocusedInstance;
	public override bool IsPlayerCharacter =>
		_identity.IsPlayerCharacter &&
		IsControllable &&
		ControlPolicy == CharacterInstanceControlPolicy.PlayerFocusable;

	internal Character OwningCharacter => _identity;

	public override void Save()
	{
		using (new FMDB())
		{
			var instance = FMDB.Context.CharacterInstances.Find(InstanceId);
			if (instance is null)
			{
				Changed = false;
				return;
			}

			SaveSecondaryInstance(instance);
			FMDB.Context.SaveChanges();
			Changed = false;
		}
	}

	public override object DatabaseInsert()
	{
		throw new NotSupportedException("Passive character instances must be created through CharacterInstanceService.");
	}

	public override bool Quit(bool silent = false)
	{
		if (CharacterInstanceFocusService.IsFocusedSecondary(this) || CharacterController is not null)
		{
			return CharacterInstanceFocusService.QuitThroughPrimary(this, silent);
		}

		return CharacterInstanceService.Retire(this, out _, deleteTemporaryRows: true);
	}

	public override IGameItem? Die()
	{
		if (!IsEmbodied || State.HasFlag(CharacterState.Dead))
		{
			return null;
		}

		if (DeathPolicy == CharacterInstanceDeathPolicy.CollapseToAnchor)
		{
			Combat?.LeaveCombat(this);
			Movement?.CancelForMoverOnly(this);
			CombatTarget = null;
			SetInstanceStateAndStatus(CharacterState.Dead, CharacterStatus.Deceased);
			CharacterInstanceService.Retire(this, out _, deleteTemporaryRows: true, deathRetirement: true);
			Changed = true;
			return null;
		}

		IGameItem? remains = null;
		var oldLocation = Location;
		var oldLayer = RoomLayer;
		if (oldLocation is not null && Body.Race.CorpseModel?.CreateCorpse == true)
		{
			remains = CorpseGameItemComponentProto.CreateNewBodyRemains(this, Body, BodyRemainsContext.AbandonedBody);
			Gameworld.Add(remains);
			remains.RoomLayer = oldLayer;
			oldLocation.Insert(remains);
		}

		Combat?.LeaveCombat(this);
		Movement?.CancelForMoverOnly(this);
		CombatTarget = null;
		SetPosition(PositionSprawled.Instance, PositionModifier.None, null, null);
		Body.Die();
		SetInstanceStateAndStatus(CharacterState.Dead, CharacterStatus.Deceased);
		CharacterInstanceService.Retire(this, out _, deleteTemporaryRows: true, deathRetirement: true);
		Changed = true;
		return remains;
	}
}

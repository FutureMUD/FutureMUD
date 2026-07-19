#nullable enable

using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using BodyImplementation = MudSharp.Body.Implementations.Body;

namespace MudSharp.Character;

public partial class Character
{
	private bool TryGetUsableBodyBackup(IBodyBackupEffect backup, out ICharacterForm? form,
		out BodyImplementation? backupBody, out string whyNot)
	{
		form = Forms.FirstOrDefault(x => x.Body.Id == backup.BackupBodyId);
		backupBody = form?.Body as BodyImplementation;
		if (form is null || backupBody is null)
		{
			whyNot = "The backup body is no longer one of this character's forms.";
			return false;
		}

		if (form.Body == CurrentBody)
		{
			whyNot = "The backup body is already the active body.";
			return false;
		}

		if (backup.DestinationCell is null)
		{
			whyNot = "The backup destination no longer exists.";
			return false;
		}

		whyNot = string.Empty;
		return true;
	}

	private IBodyBackupEffect? SelectDeathBodyBackup(out ICharacterForm? form,
		out BodyImplementation? backupBody)
	{
		form = null;
		backupBody = null;
		foreach (var backup in EffectsOfType<IBodyBackupEffect>()
			         .Where(x => x.Applies())
			         .OrderByDescending(x => x.Priority)
			         .ThenByDescending(x => x.Id)
			         .ToList())
		{
			if (TryGetUsableBodyBackup(backup, out form, out backupBody, out _))
			{
				return backup;
			}
		}

		return null;
	}

	private void RetireBodyForm(IBody body)
	{
		_forms.RemoveAll(x => x.Body == body);
		_formSources.RemoveAll(x => x.Body == body);
		Changed = true;
	}

	private void EmitBodyBackupEcho(string echo, IGameItem? remains, IBody oldBody, IBody newBody)
	{
		if (string.IsNullOrWhiteSpace(echo))
		{
			return;
		}

		OutputHandler.Handle(new EmoteOutput(new Emote(echo, this, BodyBackupEchoTargets(remains, oldBody, newBody)),
			flags: OutputFlags.SuppressObscured | OutputFlags.SuppressSource));
	}

	private void SendBodyBackupSelfEcho(string echo, IGameItem? remains, IBody oldBody, IBody newBody)
	{
		if (string.IsNullOrWhiteSpace(echo))
		{
			return;
		}

		OutputHandler.Send(new EmoteOutput(new Emote(echo, this, BodyBackupEchoTargets(remains, oldBody, newBody))));
	}

	private IPerceivable[] BodyBackupEchoTargets(IGameItem? remains, IBody oldBody, IBody newBody)
	{
		return remains is null
			? new IPerceivable[] { this, oldBody, oldBody, newBody }
			: new IPerceivable[] { this, remains, oldBody, newBody };
	}

	private bool TryTransferToBodyBackupOnDeath(out IGameItem? remains)
	{
		remains = null;
		var backup = SelectDeathBodyBackup(out var form, out var newBody);
		if (backup is null || form is null || newBody is null)
		{
			return false;
		}

		if (Body is not BodyImplementation oldBody ||
		    Location is null ||
		    backup.DestinationCell is not { } destination)
		{
			return false;
		}

		var oldLocation = Location;
		var oldLayer = RoomLayer;
		var oldBodyInterface = Body;
		var combatTarget = CombatTarget;

		Combat?.LeaveCombat(this);
		CombatTarget = null;
		combatTarget?.CheckCombatStatus();
		PrepareForBodySwitch();

		PositionState = PositionSprawled.Instance;
		PositionModifier = PositionModifier.None;
		PositionTarget = null!;
		oldBody.Die();

		if (oldBody.Race.CorpseModel?.CreateCorpse == true)
		{
			remains = CorpseGameItemComponentProto.CreateNewBodyRemains(this, oldBody,
				BodyBackupEffect.NormaliseBackupRemainsContext(backup.RemainsContext));
			Gameworld.Add(remains);
			remains.RoomLayer = oldLayer;
			oldLocation.Insert(remains);
		}

		EmitBodyBackupEcho(BodyBackupEffect.OldLocationEchoForRemains(backup.OldLocationEcho, remains is not null),
			remains, oldBodyInterface, newBody);
		oldLocation.Leave(this);

		Body = newBody;
		Corpse = null;
		newBody.LoginDormantFormItems();
		newBody.ActivateForCharacter();
		_handedness = Body.Handedness;
		_gender = Body.Gender;
		RoomLayer = backup.DestinationLayer;
		destination.Enter(this);
		ReconcileTeleportCellMembership(this, destination, oldLocation);

		RetireBodyForm(oldBody);
		RemoveAllEffects<IBodyBackupEffect>(x => x.BackupBodyId == oldBody.Id, true);
		backup.ConsumeBackup(this);
		if (remains is null)
		{
			TryCleanupRetiredBody(oldBody);
		}

		newBody.FinaliseSwitchActivation();
		EmitBodyBackupEcho(backup.NewLocationEcho, remains, oldBodyInterface, newBody);
		SendBodyBackupSelfEcho(backup.SelfEcho, remains, oldBodyInterface, newBody);
		PostProcessBodySwitch();
		CurrentBodyChanged?.Invoke(this, oldBodyInterface, Body);
		Changed = true;
		return true;
	}
}

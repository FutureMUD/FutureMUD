using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Database;
using MudSharp.Events;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using MudSharp.NPC;
using MudSharp.NPC.AI;
using MudSharp.NPC.Templates;
using MudSharp.PerceptionEngine.Handlers;

#nullable enable annotations

namespace MudSharp.Character;

public sealed class NpcCharacterInstance : Character, INPC
{
	private readonly Character _identity;
	private readonly INPC _npcIdentity;
	private readonly List<IArtificialIntelligence> _AIs = new();
	private long? _bodyguardingCharacterId;

	internal NpcCharacterInstance(Character identity, MudSharp.Models.CharacterInstance instance, IBody body)
		: base(identity, instance, body)
	{
		if (identity is not INPC npcIdentity)
		{
			throw new ArgumentException("NPC character instances require an NPC identity.", nameof(identity));
		}

		_identity = identity;
		_npcIdentity = npcIdentity;
		PermissionLevel = PermissionLevel.NPC;
		CommandTree = Gameworld.RetrieveAppropriateCommandTree(this);
		var controller = new NPCController();
		controller.UpdateControlFocus(this);
		SilentAssumeControl(controller);
		Register(new NonPlayerOutputHandler());
		RefreshArtificialIntelligencesFromIdentity();
	}

	public override ICharacterIdentity Identity => _identity;
	public override IEnumerable<ICharacterInstance> Instances => _identity.Instances;
	public override ICharacterInstance PrimaryInstance => _identity.PrimaryInstance;
	public override ICharacterInstance? FocusedInstance => _identity.FocusedInstance;
	public override bool IsPlayerCharacter => false;
	public INPCTemplate Template => _npcIdentity.Template;
	public IEnumerable<IArtificialIntelligence> AIs => _AIs;

	internal Character OwningCharacter => _identity;

	internal void RefreshArtificialIntelligencesFromIdentity()
	{
		ReleaseEventSubscriptions();
		_AIs.Clear();
		_AIs.AddRange(_npcIdentity.AIs);
		SetupEventSubscriptions();
	}

	public void SetupEventSubscriptions()
	{
		if (State.HasFlag(CharacterState.Dead) || State.HasFlag(CharacterState.Stasis))
		{
			ReleaseEventSubscriptions();
			return;
		}

		if (AIs.Any(x => x.HandlesEvent(EventType.FiveSecondTick)))
		{
			Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat -= FiveSecondHeartbeat;
			Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat += FiveSecondHeartbeat;
		}

		if (AIs.Any(x => x.HandlesEvent(EventType.TenSecondTick)))
		{
			Gameworld.HeartbeatManager.FuzzyTenSecondHeartbeat -= TenSecondHeartbeat;
			Gameworld.HeartbeatManager.FuzzyTenSecondHeartbeat += TenSecondHeartbeat;
		}

		if (AIs.Any(x => x.HandlesEvent(EventType.MinuteTick)))
		{
			Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat -= MinuteHeartbeat;
			Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat += MinuteHeartbeat;
		}

		if (AIs.Any(x => x.HandlesEvent(EventType.HourTick)))
		{
			Gameworld.HeartbeatManager.FuzzyHourHeartbeat -= HourHeartbeat;
			Gameworld.HeartbeatManager.FuzzyHourHeartbeat += HourHeartbeat;
		}
	}

	public void ReleaseEventSubscriptions()
	{
		if (AIs.Any(x => x.HandlesEvent(EventType.FiveSecondTick)))
		{
			Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat -= FiveSecondHeartbeat;
		}

		if (AIs.Any(x => x.HandlesEvent(EventType.TenSecondTick)))
		{
			Gameworld.HeartbeatManager.FuzzyTenSecondHeartbeat -= TenSecondHeartbeat;
		}

		if (AIs.Any(x => x.HandlesEvent(EventType.MinuteTick)))
		{
			Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat -= MinuteHeartbeat;
		}

		if (AIs.Any(x => x.HandlesEvent(EventType.HourTick)))
		{
			Gameworld.HeartbeatManager.FuzzyHourHeartbeat -= HourHeartbeat;
		}
	}

	public void AddAI(IArtificialIntelligence ai)
	{
		ReleaseEventSubscriptions();
		_AIs.Add(ai);
		SetupEventSubscriptions();
		Changed = true;
	}

	public void RemoveAI(IArtificialIntelligence ai)
	{
		ReleaseEventSubscriptions();
		_AIs.Remove(ai);
		SetupEventSubscriptions();
		Changed = true;
	}

	public long? BodyguardingCharacterID
	{
		get => _bodyguardingCharacterId;
		set
		{
			_bodyguardingCharacterId = value;
			Changed = true;
		}
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		if (State.HasFlag(CharacterState.Dead) || State.HasFlag(CharacterState.Stasis))
		{
			return false;
		}

		if (EffectsOfType<IPauseAIEffect>().Any())
		{
			return base.HandleEvent(type, arguments);
		}

		var aiEvents = _AIs.Any(x => x.HandleEvent(type, arguments));
		return base.HandleEvent(type, arguments) || aiEvents;
	}

	public override bool HandlesEvent(params EventType[] types)
	{
		return base.HandlesEvent(types) || AIs.Any(x => x.HandlesEvent(types));
	}

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
		throw new NotSupportedException("NPC character instances must be created through CharacterInstanceService.");
	}

	public override bool Quit(bool silent = false)
	{
		ReleaseEventSubscriptions();
		return CharacterInstanceService.Retire(this, out _, deleteTemporaryRows: true);
	}

	public override IGameItem? Die()
	{
		if (!IsEmbodied || State.HasFlag(CharacterState.Dead))
		{
			return null;
		}

		ReleaseEventSubscriptions();
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

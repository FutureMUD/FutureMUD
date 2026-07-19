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
using MudSharp.PerceptionEngine.Handlers;

#nullable enable annotations

namespace MudSharp.Character;

public sealed class ScriptedAiCharacterInstance : Character, IArtificialIntelligenceControlledCharacter
{
	private readonly Character _identity;
	private readonly List<IArtificialIntelligence> _AIs = new();
	private long? _bodyguardingCharacterId;

	internal ScriptedAiCharacterInstance(Character identity, MudSharp.Models.CharacterInstance instance, IBody body)
		: base(identity, instance, body)
	{
		_identity = identity;
		PermissionLevel = PermissionLevel.NPC;
		CommandTree = Gameworld.RetrieveAppropriateCommandTree(this);
		var controller = new NPCController();
		controller.UpdateControlFocus(this);
		SilentAssumeControl(controller);
		Register(new NonPlayerOutputHandler());
		LoadArtificialIntelligencesFromMetadata();
		SetupEventSubscriptions();
	}

	public override ICharacterIdentity Identity => _identity;
	public override IEnumerable<ICharacterInstance> Instances => _identity.Instances;
	public override ICharacterInstance PrimaryInstance => _identity.PrimaryInstance;
	public override ICharacterInstance? FocusedInstance => _identity.FocusedInstance;
	public override bool IsPlayerCharacter => false;
	public IEnumerable<IArtificialIntelligence> AIs => _AIs;

	internal Character OwningCharacter => _identity;

	private void LoadArtificialIntelligencesFromMetadata()
	{
		_AIs.Clear();
		var aiIds =
			CharacterInstanceMetadata.TryGetAnimatedCorpseMetadata(InstanceEffectData, out var animatedMetadata)
				? animatedMetadata.ArtificialIntelligenceIds
				: CharacterInstanceMetadata.TryGetScriptedAiMetadata(InstanceEffectData, out var metadata)
					? metadata.ArtificialIntelligenceIds
					: Array.Empty<long>();
		if (aiIds.Count == 0)
		{
			return;
		}

		foreach (var aiId in aiIds)
		{
			var ai = Gameworld.AIs.Get(aiId);
			if (ai is not null && !_AIs.Contains(ai))
			{
				_AIs.Add(ai);
			}
		}
	}

	private void PersistArtificialIntelligenceMetadata()
	{
		CharacterInstanceMetadata.TryGetScriptedAiMetadata(InstanceEffectData, out var metadata);
		if (InstanceKind == CharacterInstanceKind.AnimatedCorpse &&
		    CharacterInstanceMetadata.TryGetAnimatedCorpseMetadata(InstanceEffectData, out var animatedMetadata))
		{
			SetInstanceEffectData(CharacterInstanceMetadata.CreateAnimatedCorpseEffectData(
				animatedMetadata.AnchorCharacterId,
				animatedMetadata.AnchorInstanceId,
				animatedMetadata.CorpseItemId,
				animatedMetadata.OriginalCharacterId,
				animatedMetadata.OriginalBodyId,
				animatedMetadata.SourceSpellId,
				_AIs.Select(x => x.Id),
				animatedMetadata.PersistencePolicy));
		}
		else
		{
			var form = Identity.Forms.FirstOrDefault(x => ReferenceEquals(x.Body, Body));
			SetInstanceEffectData(CharacterInstanceMetadata.CreateScriptedAiEffectData(
				Identity.Id,
				PrimaryInstance.InstanceId,
				Body.Id,
				form?.Alias ?? metadata.FormKey,
				_AIs.Select(x => x.Id),
				metadata.CloneInventory));
		}

		Changed = true;
		Save();
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
		if (ai is null || _AIs.Contains(ai))
		{
			return;
		}

		ReleaseEventSubscriptions();
		_AIs.Add(ai);
		PersistArtificialIntelligenceMetadata();
		SetupEventSubscriptions();
	}

	public void RemoveAI(IArtificialIntelligence ai)
	{
		if (ai is null || !_AIs.Contains(ai))
		{
			return;
		}

		ReleaseEventSubscriptions();
		_AIs.Remove(ai);
		PersistArtificialIntelligenceMetadata();
		SetupEventSubscriptions();
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
		throw new NotSupportedException("Scripted AI character instances must be created through CharacterInstanceService.");
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
		if (DeathPolicy == CharacterInstanceDeathPolicy.CollapseToAnchor ||
		    InstanceKind == CharacterInstanceKind.AnimatedCorpse)
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

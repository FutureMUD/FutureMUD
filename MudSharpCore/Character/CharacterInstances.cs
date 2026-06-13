using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Models;
using MudSharp.NPC;
using MudSharp.PerceptionEngine.Handlers;
using MudSharp.RPG.Merits;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable annotations

namespace MudSharp.Character;

public partial class Character
{
	private sealed record CharacterInstanceLoadData(
		int PositionId,
		int PositionModifier,
		string? PositionEmote,
		long? PositionTargetId,
		string? PositionTargetType
	);

	private long _instanceId;
	private string? _instanceName;
	private CharacterInstanceKind _instanceKind;
	private CharacterInstanceControlPolicy _controlPolicy;
	private CharacterInstanceDeathPolicy _deathPolicy;
	private CharacterInstancePerceptionPolicy _perceptionPolicy;
	private CharacterInstancePersistencePolicy _persistencePolicy;
	private string _instanceEffectData = "<Effects/>";
	private bool _isPrimaryInstance;
	private bool _isEmbodied;
	private bool _isControllable;
	private bool _primaryInstancePoliciesInitialised;
	private readonly List<ICharacterInstance> _secondaryInstances = new();
	private ICharacterInstance? _focusedInstance;

	public virtual ICharacterIdentity Identity => this;
	public virtual long InstanceId => _instanceId != 0 ? _instanceId : Id;
	public virtual CharacterInstanceKind InstanceKind => _instanceKind;
	public virtual CharacterInstanceControlPolicy ControlPolicy => _controlPolicy;
	public virtual CharacterInstanceDeathPolicy DeathPolicy => _deathPolicy;
	public virtual CharacterInstancePerceptionPolicy PerceptionPolicy => _perceptionPolicy;
	public virtual CharacterInstancePersistencePolicy PersistencePolicy => _persistencePolicy;
	public virtual string InstanceEffectData => _instanceEffectData;
	public virtual bool IsPrimaryInstance => _isPrimaryInstance;
	public virtual bool IsControllable => _isControllable;
	public virtual bool IsEmbodied => _isEmbodied;

	// Secondary instances are intentionally identity-local and cell-local; do not add them to global actor caches.
	public virtual IEnumerable<ICharacterInstance> Instances =>
		Enumerable.Repeat<ICharacterInstance>(this, 1).Concat(_secondaryInstances);
	public virtual ICharacterInstance PrimaryInstance => this;
	public virtual ICharacterInstance? FocusedInstance => _focusedInstance ?? this;
	public virtual IEnumerable<IMerit> CharacterMerits => _merits;

	private CharacterInstanceControlPolicy DefaultPrimaryInstanceControlPolicy =>
		IsPlayerCharacter ? CharacterInstanceControlPolicy.PlayerFocusable : CharacterInstanceControlPolicy.NpcAiControlled;

	protected Character(Character identity, MudSharp.Models.CharacterInstance instance, IBody body)
		: base(instance.Id)
	{
		_noSave = true;
		_name = identity.Name;
		Gameworld = identity.Gameworld;
		QueuedMoveCommands = new Queue<string>();
		Account = identity.Account;
		PermissionLevel = PermissionLevel.NPC;
		Culture = identity.Culture;
		Currency = identity.Currency;
		Birthday = identity.Birthday;
		_personalName = identity.PersonalName;
		_currentName = identity.CurrentName;
		Aliases = identity.Aliases.ToList();
		Body = body;
		Body.Actor = this;
		Body.ActivateForCharacter();
		_handedness = Body.Handedness;
		_gender = Body.Gender;
		_status = (CharacterStatus)instance.Status;
		_state = (CharacterState)instance.State;
		_instanceId = instance.Id;
		_instanceName = instance.InstanceName;
		_instanceKind = (CharacterInstanceKind)instance.InstanceKind;
		_controlPolicy = (CharacterInstanceControlPolicy)instance.ControlPolicy;
		_deathPolicy = (CharacterInstanceDeathPolicy)instance.DeathPolicy;
		_perceptionPolicy = (CharacterInstancePerceptionPolicy)instance.PerceptionPolicy;
		_persistencePolicy = (CharacterInstancePersistencePolicy)instance.PersistencePolicy;
		_instanceEffectData = instance.EffectData.IfNullOrWhiteSpace("<Effects/>");
		_isPrimaryInstance = instance.IsPrimary;
		_isEmbodied = instance.IsEmbodied;
		_isControllable = instance.IsControllable;
		_primaryInstancePoliciesInitialised = true;
		var canUsePlayerCommandTree =
			identity.IsPlayerCharacter &&
			_isControllable &&
			_controlPolicy == CharacterInstanceControlPolicy.PlayerFocusable;
		PermissionLevel = canUsePlayerCommandTree ? identity.PermissionLevel : PermissionLevel.NPC;
		CommandTree = Gameworld.RetrieveAppropriateCommandTree(this);
		Location = instance.LocationId.HasValue ? Gameworld.Cells.Get(instance.LocationId.Value) : identity.Location;
		_roomLayer = (RoomLayer)instance.RoomLayer;
		InitialiseDefaultForm(body);
		_merits.AddRange(identity._merits);
		_characterTraits.AddRange(identity._characterTraits);
		_roles.AddRange(identity._roles);
		_clanMemberships.AddRange(identity._clanMemberships);
		_characterKnowledges.AddRange(identity._characterKnowledges);
		_languages.AddRange(identity._languages);
		_scripts.AddRange(identity._scripts);
		foreach (var accent in identity._accents)
		{
			_accents[accent.Key] = accent.Value;
		}

		foreach (var preferred in identity._preferredAccents)
		{
			_preferredAccents[preferred.Key] = preferred.Value;
		}

		_currentLanguage = identity._currentLanguage;
		_currentWritingLanguage = identity._currentWritingLanguage;
		_currentAccent = identity._currentAccent;
		_dbTotalMinutesPlayed = identity._dbTotalMinutesPlayed;
		LoginDateTime = identity.LoginDateTime;
		LastMinutesUpdate = identity.LastMinutesUpdate == default ? DateTime.UtcNow : identity.LastMinutesUpdate;
		SetCombatSettingsProvisional(identity.CombatSettings ?? CharacterCombatSettingsResolver.ResolveFallback(this));
		Register(new NonPlayerOutputHandler());
		if (instance.PositionId != 0)
		{
			LoadPosition(instance.PositionId, instance.PositionModifier, instance.PositionEmote,
				instance.PositionTargetId, instance.PositionTargetType);
		}
		else
		{
			SetPosition(PositionStanding.Instance, PositionModifier.None, null, null);
		}

		Changed = false;
		_noSave = false;
	}

	private void InitialisePrimaryInstanceDefaults()
	{
		_focusedInstance = null;
		_instanceName = null;
		_instanceKind = CharacterInstanceKind.Primary;
		_controlPolicy = DefaultPrimaryInstanceControlPolicy;
		_deathPolicy = CharacterInstanceDeathPolicy.FinalCharacterDeath;
		_perceptionPolicy = CharacterInstancePerceptionPolicy.OrdinaryEmbodied;
		_persistencePolicy = CharacterInstancePersistencePolicy.Persistent;
		_instanceEffectData = "<Effects/>";
		_isPrimaryInstance = true;
		_isEmbodied = true;
		_isControllable = _controlPolicy != CharacterInstanceControlPolicy.NotControllable;
		_primaryInstancePoliciesInitialised = true;
	}

	private void EnsurePrimaryInstanceDefaults()
	{
		if (!_primaryInstancePoliciesInitialised)
		{
			InitialisePrimaryInstanceDefaults();
		}
	}

	private CharacterInstanceLoadData LoadPrimaryInstance(MudSharp.Models.Character character)
	{
		_focusedInstance = null;
		var primary = character.CharacterInstances
		                       .Where(x => x.IsPrimary)
		                       .OrderBy(x => x.Id)
		                       .FirstOrDefault() ??
		              character.CharacterInstances
		                       .OrderBy(x => x.Id)
		                       .FirstOrDefault();

		if (primary is null)
		{
			InitialisePrimaryInstanceDefaults();
			return new CharacterInstanceLoadData(
				character.PositionId,
				character.PositionModifier,
				character.PositionEmote,
				character.PositionTargetId,
				character.PositionTargetType
			);
		}

		_instanceId = primary.Id;
		_instanceName = primary.InstanceName;
		_instanceKind = (CharacterInstanceKind)primary.InstanceKind;
		_controlPolicy = (CharacterInstanceControlPolicy)primary.ControlPolicy;
		_deathPolicy = (CharacterInstanceDeathPolicy)primary.DeathPolicy;
		_perceptionPolicy = (CharacterInstancePerceptionPolicy)primary.PerceptionPolicy;
		_persistencePolicy = (CharacterInstancePersistencePolicy)primary.PersistencePolicy;
		_instanceEffectData = primary.EffectData.IfNullOrWhiteSpace("<Effects/>");
		_isPrimaryInstance = primary.IsPrimary;
		_isEmbodied = primary.IsEmbodied;
		_isControllable = primary.IsControllable;
		_primaryInstancePoliciesInitialised = true;

		var primaryBody = Bodies.FirstOrDefault(x => x.Id == primary.BodyId);
		if (primaryBody is not null && !ReferenceEquals(primaryBody, Body))
		{
			Body = primaryBody;
			Body.Actor = this;
			_handedness = Body.Handedness;
			_gender = Body.Gender;
		}

		if (primary.LocationId.HasValue)
		{
			Location = Gameworld.Cells.Get(primary.LocationId.Value) ?? Location;
		}

		_roomLayer = (RoomLayer)primary.RoomLayer;
		_state = (CharacterState)primary.State;
		_state |= CharacterState.Stasis;
		_status = (CharacterStatus)primary.Status;

		return new CharacterInstanceLoadData(
			primary.PositionId != 0 ? primary.PositionId : character.PositionId,
			primary.PositionModifier,
			primary.PositionEmote,
			primary.PositionTargetId,
			primary.PositionTargetType
		);
	}

	private void LoadSecondaryInstances(MudSharp.Models.Character character)
	{
		var persistenceChanged = false;
		foreach (var instance in character.CharacterInstances
		                                  .Where(x => !x.IsPrimary)
		                                  .OrderBy(x => x.Id)
		                                  .ToList())
		{
			if (!instance.IsEmbodied)
			{
				continue;
			}

			if ((CharacterInstancePersistencePolicy)instance.PersistencePolicy ==
			    CharacterInstancePersistencePolicy.DespawnOnReboot)
			{
				FMDB.Context.CharacterInstances.Remove(instance);
				persistenceChanged = true;
				continue;
			}

			var body = Bodies.FirstOrDefault(x => x.Id == instance.BodyId);
			if (body is null)
			{
				instance.IsEmbodied = false;
				instance.IsControllable = false;
				persistenceChanged = true;
				continue;
			}

			MaterialiseSecondaryInstance(instance, body);
		}

		if (persistenceChanged)
		{
			FMDB.Context.SaveChanges();
		}
	}

	internal ICharacterInstance MaterialiseSecondaryInstance(MudSharp.Models.CharacterInstance instance, IBody body)
	{
		var materialised = this is INPC &&
		                   (CharacterInstanceControlPolicy)instance.ControlPolicy ==
		                   CharacterInstanceControlPolicy.NpcAiControlled
			? (ICharacterInstance)new NpcCharacterInstance(this, instance, body)
			: new PassiveCharacterInstance(this, instance, body);
		_secondaryInstances.RemoveAll(x => ReferenceEquals(x, materialised) || x.InstanceId == materialised.InstanceId);
		_secondaryInstances.Add(materialised);
		if (materialised.IsEmbodied && materialised.Location is not null)
		{
			materialised.Location.Enter(materialised, roomLayer: materialised.RoomLayer);
		}

		return materialised;
	}

	protected void RefreshLoadedNpcSecondaryInstances()
	{
		foreach (var instance in _secondaryInstances.OfType<NpcCharacterInstance>())
		{
			instance.RefreshArtificialIntelligencesFromIdentity();
		}
	}

	internal void ForgetSecondaryInstance(ICharacterInstance instance)
	{
		if (_focusedInstance?.InstanceId == instance.InstanceId)
		{
			_focusedInstance = null;
		}

		_secondaryInstances.RemoveAll(x => ReferenceEquals(x, instance) || x.InstanceId == instance.InstanceId);
	}

	internal void SetFocusedInstance(ICharacterInstance? instance)
	{
		_focusedInstance = instance is null || instance.IsPrimaryInstance ? null : instance;
	}

	internal string InstanceFocusPromptLine()
	{
		if (IsPrimaryInstance)
		{
			return string.Empty;
		}

		var form = Identity.Forms.FirstOrDefault(x => ReferenceEquals(x.Body, Body));
		var name = form?.Alias ?? Body.Prototype.Name;
		return $"\n<Focus: {name.ColourName()} #{InstanceId.ToString("N0", this).ColourValue()}>\n";
	}

	internal void SetInstanceEmbodied(bool value)
	{
		_isEmbodied = value;
	}

	internal void SetInstanceControllable(bool value)
	{
		_isControllable = value;
	}

	internal void SetInstancePersistencePolicy(CharacterInstancePersistencePolicy policy)
	{
		_persistencePolicy = policy;
	}

	internal void SetInstanceStateAndStatus(CharacterState state, CharacterStatus status)
	{
		State = state;
		_status = status;
	}

	internal void ClearInstanceLocation()
	{
		Location = null!;
	}

	internal void ResumeAfterProjectionAnchorStasis()
	{
		if (State.HasFlag(CharacterState.Stasis))
		{
			State &= ~CharacterState.Stasis;
		}

		if (!State.HasFlag(CharacterState.Dead))
		{
			StartNeedsHeartbeat();
			ResumeMagicResourceGeneratorHeartbeats();
			RefreshForcedTransformationHeartbeatRegistration();
		}
	}

	private MudSharp.Models.CharacterInstance GetOrCreatePrimaryInstance(MudSharp.Models.Character dbchar)
	{
		var primary = dbchar.CharacterInstances
		                    .Where(x => x.IsPrimary)
		                    .OrderBy(x => x.Id)
		                    .FirstOrDefault();
		if (primary is not null)
		{
			return primary;
		}

		if (_instanceId != 0)
		{
			primary = dbchar.CharacterInstances.FirstOrDefault(x => x.Id == _instanceId);
			if (primary is not null)
			{
				return primary;
			}
		}

		primary = new MudSharp.Models.CharacterInstance
		{
			Character = dbchar,
			CreatedDateTime = DateTime.UtcNow,
			EffectData = "<Effects/>"
		};
		dbchar.CharacterInstances.Add(primary);
		return primary;
	}

	private void SavePrimaryInstance(MudSharp.Models.Character dbchar, CharacterState? stateOverride = null)
	{
		EnsurePrimaryInstanceDefaults();
		var primary = GetOrCreatePrimaryInstance(dbchar);
		primary.Character = dbchar;
		primary.CharacterId = dbchar.Id;
		primary.BodyId = Body.Id;
		primary.InstanceName = _instanceName;
		primary.InstanceKind = (int)_instanceKind;
		primary.ControlPolicy = (int)_controlPolicy;
		primary.DeathPolicy = (int)_deathPolicy;
		primary.PerceptionPolicy = (int)_perceptionPolicy;
		primary.PersistencePolicy = (int)_persistencePolicy;
		primary.LocationId = Location?.Id;
		primary.RoomLayer = (int)RoomLayer;
		primary.PositionId = (int)(PositionState?.Id ?? PositionStanding.Instance.Id);
		primary.PositionModifier = (int)PositionModifier;
		primary.PositionTargetId = PositionTarget?.Id;
		primary.PositionTargetType = PositionTarget?.FrameworkItemType;
		primary.PositionEmote = PositionEmote?.SaveToXml().ToString() ?? string.Empty;
		primary.State = (int)(stateOverride ?? State);
		primary.Status = (int)_status;
		primary.IsPrimary = true;
		primary.IsEmbodied = _isEmbodied;
		primary.IsControllable = _isControllable;
		primary.EffectData = _instanceEffectData.IfNullOrWhiteSpace("<Effects/>");
		if (primary.CreatedDateTime == default)
		{
			primary.CreatedDateTime = DateTime.UtcNow;
		}

		if (primary.Id != 0)
		{
			_instanceId = primary.Id;
		}

		SaveLoadedSecondaryInstances(dbchar);
	}

	private void SaveLoadedSecondaryInstances(MudSharp.Models.Character dbchar)
	{
		foreach (var secondary in _secondaryInstances.OfType<Character>())
		{
			var instance = dbchar.CharacterInstances.FirstOrDefault(x => x.Id == secondary.InstanceId);
			if (instance is null)
			{
				continue;
			}

			secondary.SaveSecondaryInstance(instance);
		}
	}

	private void SaveCompatibilityWorldPresence(MudSharp.Models.Character dbchar, CharacterState? stateOverride = null)
	{
		// Legacy Characters world-presence columns mirror only the primary instance during the transition.
		dbchar.Location = Location?.Id ?? 1L;
		dbchar.RoomLayer = (int)RoomLayer;
		dbchar.State = (int)(stateOverride ?? State);
		dbchar.Status = (int)_status;
		dbchar.BodyId = Body.Id;
		dbchar.PositionId = (int)(PositionState?.Id ?? PositionStanding.Instance.Id);
		dbchar.PositionModifier = (int)PositionModifier;
		dbchar.PositionTargetId = PositionTarget?.Id;
		dbchar.PositionTargetType = PositionTarget?.FrameworkItemType;
		dbchar.PositionEmote = PositionEmote?.SaveToXml().ToString() ?? string.Empty;
	}

	private void SaveInstanceResurrectionState(MudSharp.Models.Character dbchar)
	{
		SaveCompatibilityWorldPresence(dbchar);
		SavePrimaryInstance(dbchar);
	}

	internal void SaveSecondaryInstance(MudSharp.Models.CharacterInstance instance)
	{
		instance.BodyId = Body.Id;
		instance.InstanceName = _instanceName;
		instance.InstanceKind = (int)_instanceKind;
		instance.ControlPolicy = (int)_controlPolicy;
		instance.DeathPolicy = (int)_deathPolicy;
		instance.PerceptionPolicy = (int)_perceptionPolicy;
		instance.PersistencePolicy = (int)_persistencePolicy;
		instance.LocationId = Location?.Id;
		instance.RoomLayer = (int)RoomLayer;
		instance.PositionId = (int)(PositionState?.Id ?? PositionStanding.Instance.Id);
		instance.PositionModifier = (int)PositionModifier;
		instance.PositionTargetId = PositionTarget?.Id;
		instance.PositionTargetType = PositionTarget?.FrameworkItemType;
		instance.PositionEmote = PositionEmote?.SaveToXml().ToString() ?? string.Empty;
		instance.State = (int)State;
		instance.Status = (int)_status;
		instance.IsPrimary = false;
		instance.IsEmbodied = _isEmbodied;
		instance.IsControllable = _isControllable;
		instance.EffectData = _instanceEffectData.IfNullOrWhiteSpace("<Effects/>");
		if (instance.CreatedDateTime == default)
		{
			instance.CreatedDateTime = DateTime.UtcNow;
		}
	}

	private void InsertInitialPrimaryInstance(MudSharp.Models.Character dbitem)
	{
		EnsurePrimaryInstanceDefaults();
		var primary = new MudSharp.Models.CharacterInstance
		{
			Character = dbitem,
			Body = dbitem.Body,
			InstanceName = _instanceName,
			InstanceKind = (int)_instanceKind,
			ControlPolicy = (int)_controlPolicy,
			DeathPolicy = (int)_deathPolicy,
			PerceptionPolicy = (int)_perceptionPolicy,
			PersistencePolicy = (int)_persistencePolicy,
			LocationId = dbitem.Location,
			RoomLayer = (int)RoomLayer,
			PositionId = (int)(PositionState?.Id ?? PositionStanding.Instance.Id),
			PositionModifier = (int)PositionModifier,
			PositionTargetId = PositionTarget?.Id,
			PositionTargetType = PositionTarget?.FrameworkItemType,
			PositionEmote = PositionEmote?.SaveToXml().ToString() ?? string.Empty,
			State = (int)State,
			Status = (int)_status,
			IsPrimary = true,
			IsEmbodied = true,
			IsControllable = _isControllable,
			CreatedDateTime = DateTime.UtcNow,
			EffectData = "<Effects/>"
		};
		dbitem.CharacterInstances.Add(primary);
	}

	private void CaptureInsertedPrimaryInstanceId(MudSharp.Models.Character item)
	{
		_instanceId = item.CharacterInstances
		                  .Where(x => x.IsPrimary)
		                  .OrderBy(x => x.Id)
		                  .FirstOrDefault()
		                  ?.Id ?? _instanceId;
	}

	public bool SameIdentity(ICharacter other)
	{
		return CharacterInstanceIdentityComparer.SameIdentity(this, other);
	}

	public bool SamePhysicalInstance(IPerceivable other)
	{
		return CharacterInstanceIdentityComparer.SamePhysicalInstance(this, other);
	}
}

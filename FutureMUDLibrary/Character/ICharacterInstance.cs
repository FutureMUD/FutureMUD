using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character.Heritage;
using MudSharp.Character.Name;
using MudSharp.Framework;
using MudSharp.RPG.Knowledge;
using MudSharp.RPG.Merits;
using System.Collections.Generic;

#nullable enable

namespace MudSharp.Character;

public enum CharacterInstanceKind
{
	Primary = 0,
	AstralProjection = 1,
	MagicalCopy = 2,
	PhysicalClone = 3,
	Puppet = 4,
	PossessedBody = 5,
	RemoteShell = 6,
	ScriptedAi = 7,
	Other = 99
}

public enum CharacterInstanceControlPolicy
{
	NotControllable = 0,
	PlayerFocusable = 1,
	PlayerRemoteCommandable = 2,
	NpcAiControlled = 3,
	IdentityCoordinatorControlled = 4,
	ScriptOnly = 5
}

public enum CharacterInstanceDeathPolicy
{
	FinalCharacterDeath = 0,
	DestroyInstanceOnly = 1,
	DestroyInstanceAndDamageAnchor = 2,
	CollapseToAnchor = 3,
	TransferControlToAnchor = 4,
	TransferControlToBackup = 5,
	KillIdentityIfNoPrimaryCapableInstance = 6
}

public enum CharacterInstancePerceptionPolicy
{
	OrdinaryEmbodied = 0,
	FocusedOnly = 1,
	RemoteFeedToIdentity = 2,
	SilentUnlessCritical = 3,
	MergedWithFocusedOutput = 4,
	PlanarProjection = 5
}

public enum CharacterInstancePersistencePolicy
{
	Persistent = 0,
	TemporaryEffectBound = 1,
	RecreateFromEffectOnLoad = 2,
	DespawnOnLogout = 3,
	DespawnOnReboot = 4
}

public enum BodyEmbodimentState
{
	DormantForm = 0,
	Embodied = 1,
	Suspended = 2,
	Retired = 3,
	Destroyed = 4
}

public interface ICharacterIdentity : IFrameworkItem
{
	IEnumerable<ICharacterInstance> Instances { get; }
	ICharacterInstance PrimaryInstance { get; }
	ICharacterInstance? FocusedInstance { get; }
	IEnumerable<ICharacterForm> Forms { get; }
	IEnumerable<IBody> Bodies { get; }
	IAccount Account { get; }
	IPersonalName PersonalName { get; }
	ICulture Culture { get; }
	IEnumerable<ITrait> CharacterTraits { get; }
	IEnumerable<IKnowledge> Knowledges { get; }
	IEnumerable<IMerit> CharacterMerits { get; }
}

public interface ICharacterInstance : ICharacter
{
	string InstanceEffectData { get; }
}

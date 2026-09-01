#nullable enable

using System.Collections.Generic;
using MudSharp.Character;

namespace MudSharp.GameItems.Interfaces;

public interface IAccessControlReader : ISignalSourceComponent, IAutomationMountable, IConnectable
{
	long SelfTargetLockPrototypeId { get; }
	bool ActivateAccessSignal();
	bool TrySetSelfTarget(ILock? target, out string error);
}

public interface IKeypad : IAccessControlReader, ISelectable
{
	string Code { get; }
	bool TrySetCode(string code, out string error);
	bool TryCode(string code, out string error);
}

public sealed record BiometricAuthorisation(long CharacterId, string Name);

public interface IBiometricScanner : IAccessControlReader
{
	IReadOnlyCollection<BiometricAuthorisation> AuthorisedPeople { get; }
	bool AddAuthorisedPerson(ICharacter character, out string error);
	bool RemoveAuthorisedPerson(long characterId, out string error);
	bool ClearAuthorisedPeople();
	bool IsAuthorised(long characterId);
	bool CanScan(ICharacter actor, IGameItem? severedBodypart, out long identityId, out string error);
}

public interface IKeycard : IGameItemComponent
{
	IReadOnlyCollection<string> Codes { get; }
	bool AddCode(string code, out string error);
	bool RemoveCode(string code, out string error);
	bool ClearCodes();
	bool HasCode(string code);
}

public interface IKeycardScanner : IAccessControlReader
{
	IReadOnlyCollection<string> AcceptedCodes { get; }
	bool AddAcceptedCode(string code, out string error);
	bool RemoveAcceptedCode(string code, out string error);
	bool ClearAcceptedCodes();
	bool AcceptsCode(string code);
	bool TryCard(IKeycard card, out string error);
}

public interface IKeycardWriter : IGameItemComponent, IConsumePower, ISwitchable, IOnOff, IAutomationMountable,
	IConnectable
{
	bool CanWrite(out string error);
}

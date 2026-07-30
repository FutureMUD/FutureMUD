#nullable enable

using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Interfaces;

public enum MilitaryStandardFamily
{
	InfantryColour = 0,
	CavalryStandard = 1,
	Guidon = 2,
	NavalEnsign = 3,
	Pennant = 4,
	SignalFlag = 5
}

public enum MilitaryStandardAssociationType
{
	None = 0,
	Unit = 1,
	Ship = 2
}

public enum MilitaryStandardCustodyState
{
	Unclaimed = 0,
	Friendly = 1,
	Captured = 2
}

public interface IMilitaryStandard : IGameItemComponent
{
	MilitaryStandardFamily Family { get; }
	string IdentityKey { get; }
	string IdentityName { get; }
	string Design { get; }
	MilitaryStandardAssociationType AssociationType { get; }
	string AssociationKey { get; }
	string AssociationName { get; }
	bool IsPlanted { get; }
	MilitaryStandardCustodyState CustodyState { get; }
	int CaptureCount { get; }
	IReadOnlyCollection<string> SignalPatterns { get; }

	bool IsAuthorisedBearer(ICharacter actor);
	void ReevaluateCustody(ICharacter? actor);
	bool Recognise(ICharacter actor);
	bool IsRecognisedBy(ICharacter actor);
	bool Plant(ICharacter actor, IEmote? playerEmote = null);
	bool TakeUp(ICharacter actor, IEmote? playerEmote = null);
	bool Signal(ICharacter actor, string pattern, IEmote? playerEmote = null);

	void SetIdentityOverride(string key, string name);
	void SetDesignOverride(string design);
	void SetAssociationOverride(MilitaryStandardAssociationType type, string key, string name);
	void ResetOverrides();
	void SetCustody(MilitaryStandardCustodyState state);
	void SetCaptureCount(int count);
	void ResetCaptureCount();
}

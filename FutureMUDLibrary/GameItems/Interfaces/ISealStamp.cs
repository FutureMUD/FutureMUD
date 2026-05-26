#nullable enable

using MudSharp.Character;
using MudSharp.RPG.Checks;
using System;

namespace MudSharp.GameItems.Interfaces;

public sealed record SealImpression(
	string SealDesign,
	string IssuerText,
	string OwnerText,
	string ClanText,
	string OfficeText,
	string StampMaterial,
	Difficulty ForgeryDifficulty,
	long SealingCharacterId,
	string SealingCharacterName,
	DateTime SealedAt,
	string SealMedium);

public interface ISealStamp : IGameItemComponent
{
	string SealDesign { get; }
	string IssuerText { get; }
	string OwnerText { get; }
	string ClanText { get; }
	string OfficeText { get; }
	string StampMaterial { get; }
	Difficulty ForgeryDifficulty { get; }
	bool CanSeal(ICharacter actor, IGameItem target, IGameItem? medium, out string error);
	SealImpression CreateImpression(ICharacter actor, IGameItem target, IGameItem? medium);
}

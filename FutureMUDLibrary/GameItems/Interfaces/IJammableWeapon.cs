using MudSharp.Character;

namespace MudSharp.GameItems.Interfaces;

public interface IJammableWeapon : IRangedWeapon
{
	bool IsJammed { get; set; }
	bool CanUnjam(ICharacter actor);
	string WhyCannotUnjam(ICharacter actor);
	bool Unjam(ICharacter actor);
	string StartUnjamEmote { get; }
	string FinishUnjamEmote { get; }
	string FailUnjamEmote { get; }
}
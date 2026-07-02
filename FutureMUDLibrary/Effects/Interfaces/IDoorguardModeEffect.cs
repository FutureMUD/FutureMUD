using MudSharp.Character;
using MudSharp.Construction.Boundary;

namespace MudSharp.Effects.Interfaces
{
	public enum DoorguardAccessMode
	{
		NormalRules,
		EnforcersOnly,
		Everyone
	}

	public interface IDoorguardModeEffect : IEffectSubtype
	{
		DoorguardAccessMode AccessMode { get; }
		bool? PermitsDoorOpening(ICharacter doorguard, ICharacter target, ICellExit exit);
	}
}

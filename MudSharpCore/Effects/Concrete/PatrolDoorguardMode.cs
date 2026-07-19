using MudSharp.Construction.Boundary;
using MudSharp.RPG.Law;

namespace MudSharp.Effects.Concrete;

public class PatrolDoorguardMode : Effect, IDoorguardModeEffect
{
	public PatrolDoorguardMode(IPerceivable owner, ILegalAuthority legalAuthority, DoorguardAccessMode accessMode)
		: base(owner)
	{
		LegalAuthority = legalAuthority;
		AccessMode = accessMode;
	}

	public ILegalAuthority LegalAuthority { get; }
	public DoorguardAccessMode AccessMode { get; }

	protected override string SpecificEffectType => "PatrolDoorguardMode";

	public bool? PermitsDoorOpening(ICharacter doorguard, ICharacter target, ICellExit exit)
	{
		return AccessMode switch
		{
			DoorguardAccessMode.NormalRules => null,
			DoorguardAccessMode.EnforcersOnly => target is not null &&
			                                     LegalAuthority?.GetEnforcementAuthority(target) is not null,
			DoorguardAccessMode.Everyone => true,
			_ => null
		};
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Patrol Door Guard Mode ({AccessMode.DescribeEnum()})";
	}

	public override string ToString()
	{
		return "Patrol Door Guard Mode Effect";
	}
}

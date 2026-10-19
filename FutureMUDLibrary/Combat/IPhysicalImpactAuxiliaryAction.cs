#nullable enable

using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat;

/// <summary>Opt-in coupled physical impact; ordinary auxiliaries have no such profile.</summary>
public interface IPhysicalImpactAuxiliaryAction
{
	IPhysicalImpactProfile? PhysicalImpact { get; }
}

public interface IPhysicalImpactProfile
{
	ITraitDefinition CounterTrait { get; }
	ITraitDefinition GuardTrait { get; }
	ITraitDefinition AgilityTrait { get; }
	ITraitExpression Damage { get; }
	ITraitExpression Stun { get; }
	ITraitExpression CounterDamage { get; }
	ITraitExpression CounterStun { get; }
	double MinimumStamina { get; }
	double SizeBonusPerCategory { get; }
	OpposedOutcomeDegree CounterDegree { get; }
	bool RequireUprightTarget { get; }
	bool SeatOnMiss { get; }
	bool SeatOnHit { get; }
	bool SeatOnCounter { get; }
	bool ShieldBlock { get; }
	bool AllowUntrained { get; }
	bool RequireUsableLeg { get; }
	bool InitiateCombat { get; }
	double SuccessDelaySeconds { get; }
	XElement Save();
}

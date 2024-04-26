using System;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.GameItems;
using MudSharp.RPG.Checks;

namespace MudSharp.Health;

public class Damage : IDamage
{
	#region Overrides of Object

	/// <summary>Returns a string that represents the current object.</summary>
	/// <returns>A string that represents the current object.</returns>
	public override string ToString()
	{
		return
			$"{DamageAmount:N2} damage of type {DamageType.Describe()} to the {Bodypart?.FullDescription() ?? "nothing"}";
	}

	#endregion

	public Damage()
	{
	}

	public Damage(IDamage rhs)
	{
		DamageType = rhs.DamageType;
		DamageAmount = rhs.DamageAmount;
		PainAmount = rhs.PainAmount;
		ShockAmount = rhs.ShockAmount;
		StunAmount = rhs.StunAmount;
		Bodypart = rhs.Bodypart;
		ActorOrigin = rhs.ActorOrigin;
		ToolOrigin = rhs.ToolOrigin;
		LodgableItem = rhs.LodgableItem;
		AngleOfIncidentRadians = rhs.AngleOfIncidentRadians;
		PenetrationOutcome = rhs.PenetrationOutcome;
	}

	public Damage(IDamage rhs, double damageMultipliers)
	{
		DamageType = rhs.DamageType;
		DamageAmount = rhs.DamageAmount * damageMultipliers;
		PainAmount = rhs.PainAmount * damageMultipliers;
		ShockAmount = rhs.ShockAmount * damageMultipliers;
		StunAmount = rhs.StunAmount * damageMultipliers;
		Bodypart = rhs.Bodypart;
		ActorOrigin = rhs.ActorOrigin;
		ToolOrigin = rhs.ToolOrigin;
		LodgableItem = rhs.LodgableItem;
		AngleOfIncidentRadians = rhs.AngleOfIncidentRadians;
		PenetrationOutcome = rhs.PenetrationOutcome;
	}

	#region IDamage Members

	public DamageType DamageType { get; init; }

	public double DamageAmount { get; init; }

	public double PainAmount { get; init; }

	public double ShockAmount { get; init; }

	public double StunAmount { get; init; }

	public IBodypart Bodypart { get; init; }

	public ICharacter ActorOrigin { get; init; }

	public IGameItem ToolOrigin { get; init; }

	public IGameItem LodgableItem { get; init; }

	public double AngleOfIncidentRadians { get; init; } = Math.PI / 2;

	public Outcome PenetrationOutcome { get; init; } = Outcome.NotTested;

	#endregion
}
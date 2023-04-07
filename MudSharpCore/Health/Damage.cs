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

	public DamageType DamageType { get; set; }

	public double DamageAmount { get; set; }

	public double PainAmount { get; set; }

	public double ShockAmount { get; set; }

	public double StunAmount { get; set; }

	public IBodypart Bodypart { get; set; }

	public ICharacter ActorOrigin { get; set; }

	public IGameItem ToolOrigin { get; set; }

	public IGameItem LodgableItem { get; set; }

	public double AngleOfIncidentRadians { get; set; } = Math.PI / 2;

	public Outcome PenetrationOutcome { get; set; } = Outcome.NotTested;

	#endregion
}
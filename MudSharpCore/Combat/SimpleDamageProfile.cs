using MudSharp.Body.Traits;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat;

public record SimpleDamageProfile : IDamageProfile
{
	#region Constructors

	public SimpleDamageProfile()
	{
	}

	public SimpleDamageProfile(MudSharp.Models.WeaponAttack attack, IFuturemud game)
	{
		BaseAttackerDifficulty = (Difficulty)attack.BaseAttackerDifficulty;
		BaseBlockDifficulty = (Difficulty)attack.BaseBlockDifficulty;
		BaseDodgeDifficulty = (Difficulty)attack.BaseDodgeDifficulty;
		BaseParryDifficulty = (Difficulty)attack.BaseParryDifficulty;
		BaseAngleOfIncidence = attack.BaseAngleOfIncidence;
		DamageType = (DamageType)attack.DamageType;
		DamageExpression = game.TraitExpressions.Get(attack.DamageExpressionId);
		StunExpression = game.TraitExpressions.Get(attack.StunExpressionId);
		PainExpression = game.TraitExpressions.Get(attack.PainExpressionId);
	}

	#endregion

	#region Implementation of IDamageProfile

	public Difficulty BaseAttackerDifficulty { get; set; }
	public Difficulty BaseBlockDifficulty { get; set; }
	public Difficulty BaseDodgeDifficulty { get; set; }
	public Difficulty BaseParryDifficulty { get; set; }
	public double BaseAngleOfIncidence { get; set; }
	public ITraitExpression DamageExpression { get; set; }
	public ITraitExpression StunExpression { get; set; }
	public ITraitExpression PainExpression { get; set; }
	public DamageType DamageType { get; set; }

	#endregion
}
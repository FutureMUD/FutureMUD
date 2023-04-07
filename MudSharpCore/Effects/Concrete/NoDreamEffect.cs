using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Concrete;

public class NoDreamEffect : Effect, INoDreamEffect
{
	public NoDreamEffect(IPerceivable owner, IFutureProg applicabilityProg = null) : base(owner, applicabilityProg)
	{
	}

	public NoDreamEffect(XElement effect, IPerceivable owner) : base(effect, owner)
	{
	}

	#region Overrides of Effect

	/// <summary>
	///     Fires when the scheduled effect "matures"
	/// </summary>
	public override void ExpireEffect()
	{
		var ownerCharacter = (ICharacter)Owner;
		if (!ownerCharacter.State.HasFlag(CharacterState.Sleeping))
		{
			return;
		}

		var dreamCheck = Gameworld.GetCheck(CheckType.DreamCheck);
		var result = dreamCheck.Check(ownerCharacter, Difficulty.Normal);
		if (result.IsPass())
		{
			var dream =
				Gameworld.Dreams.Where(x => x.CanDream(ownerCharacter))
				         .GetWeightedRandom(x => x.Priority);
			if (dream == null)
			{
				return;
			}

			ownerCharacter.AddEffect(new Dreaming(ownerCharacter, dream));
			return;
		}

		ownerCharacter.AddEffect(new NoDreamEffect(Owner), TimeSpan.FromSeconds(60));
		Owner.RemoveEffect(this);
	}

	#endregion

	#region Overrides of Effect

	public override bool SavingEffect { get; } = true;

	#endregion

	public static void InitialiseEffectType()
	{
		RegisterFactory("NoDreamEffect", (effect, owner) => new NoDreamEffect(effect, owner));
	}

	#region Overrides of Effect

	public override string Describe(IPerceiver voyeur)
	{
		return $"{Owner.HowSeen(voyeur, true)} is ineligable for any dreams.";
	}

	#region Overrides of Object

	/// <summary>
	///     Returns a string that represents the current object.
	/// </summary>
	/// <returns>
	///     A string that represents the current object.
	/// </returns>
	public override string ToString()
	{
		return "Owner is ineligable for any dreams.";
	}

	#endregion

	protected override string SpecificEffectType { get; } = "NoDreamEffect";

	#endregion
}
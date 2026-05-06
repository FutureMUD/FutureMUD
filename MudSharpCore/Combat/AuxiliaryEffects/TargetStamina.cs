using System;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.AuxiliaryEffects;

#nullable enable

internal sealed class TargetStamina : OpposedAuxiliaryEffectBase
{
	public TargetStamina(XElement root, IFuturemud gameworld) : base(root, gameworld)
	{
	}

	private TargetStamina(IFuturemud gameworld, MudSharp.Body.Traits.ITraitDefinition defenseTrait,
		Difficulty defenseDifficulty) : base(gameworld, defenseTrait, defenseDifficulty, 3.0, 1.0, 10.0)
	{
	}

	protected override string TypeName => "targetstamina";
	protected override string EffectName => "Target Stamina Drain";
	protected override string AmountName => "Stamina";
	protected override double DefaultFlatAmount => 3.0;
	protected override double DefaultPerDegreeAmount => 1.0;
	protected override double DefaultMaximumAmount => 10.0;

	public static void RegisterTypeHelp()
	{
		AuxiliaryCombatAction.RegisterBuilderParser("targetstamina", (action, actor, command) =>
		{
			return !TryParseDefenseArguments(action, actor, command, out var trait, out var difficulty)
				? null
				: new TargetStamina(actor.Gameworld, trait, difficulty);
		},
			@"The Target Stamina effect drains stamina from the target on a successful opposed auxiliary move.

The syntax to create this type is as follows:

	#3auxiliary set add targetstamina [defense trait] [difficulty]#0

If omitted, the defense trait defaults to the auxiliary action's check trait and difficulty defaults to Normal. The default drain is 3 stamina plus 1 stamina per opposed success degree, capped at 10.",
			true);
	}

	public override XElement Save()
	{
		var root = new XElement("Effect");
		SaveCommonAttributes(root);
		return root;
	}

	public override string DescribeForShow(ICharacter actor)
	{
		return $"{EffectName} | {DescribeCommon(actor)}";
	}

	public override void ApplyEffect(ICharacter attacker, IPerceiver target, CheckOutcome outcome)
	{
		if (target is not ICharacter tch)
		{
			return;
		}

		if (!TryGetOpposedSuccess(attacker, target, outcome, out var opposed))
		{
			SendEcho(FailureEcho, attacker, tch);
			return;
		}

		var amount = Math.Min(tch.CurrentStamina, CalculateAmount(opposed));
		if (amount <= 0.0)
		{
			return;
		}

		tch.SpendStamina(amount);
		SendEcho(SuccessEcho, attacker, tch);
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Linq;
using MudSharp.Body.Disfigurements;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Concrete;

public class UnjammingGun : CharacterActionWithTargetAndTool
{
	public static TimeSpan EffectDuration(ICharacter actor, IJammableWeapon Weapon, IGameItem ramrod)
	{
		var check = actor.Gameworld.GetCheck(CheckType.UnjamGun);
		var difficulty = Difficulty.Normal;
		if (actor.Combat is not null)
		{
			difficulty = difficulty.StageUp(2);
		}

		if (actor.IsEngagedInMelee)
		{
			difficulty = difficulty.StageUp(2);
		}

		var outcome = check.Check(actor, difficulty, Weapon.Parent, Weapon);
		var te = new TraitExpression(actor.Gameworld.GetStaticConfiguration("UnjammingGunDurationExpression"), actor.Gameworld);
		te.Formula.Parameters["degrees"] = outcome.CheckDegrees();
		te.Formula.Parameters["faildegrees"] = outcome.FailureDegrees();
		te.Formula.Parameters["successdegrees"] = outcome.SuccessDegrees();
		te.Formula.Parameters["difficulty"] = (int)difficulty;
		return TimeSpan.FromSeconds(te.Evaluate(actor, Weapon.WeaponType.OperateTrait, TraitBonusContext.UnjamGunDuration));
	}

	public IJammableWeapon Weapon { get; private set; }
	public IGameItem Ramrod { get; private set; }
	public DesiredItemState RamrodFinalisationState { get; private set; }

	/// <inheritdoc />
	public UnjammingGun(ICharacter owner, IJammableWeapon target, IGameItem ramrod, DesiredItemState originalState) : base(owner, target.Parent, [(ramrod, DesiredItemState.Held)])
	{
		Weapon = target;
		Ramrod = ramrod;
		RamrodFinalisationState = originalState;
		WhyCannotMoveEmoteString = "@ cannot move because #0 %0|are|is unjamming $1.";
		LDescAddendum = "unjamming $1";
		_blocks.Add("general");
		_blocks.Add("movement");
		ActionDescription = $"unjamming $1";
	}

	#region Overrides of CharacterAction

	public override void InitialEffect()
	{
		CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote(Weapon.StartUnjamEmote, CharacterOwner, CharacterOwner, Weapon.Parent, Ramrod)));
	}

	/// <inheritdoc />
	public override void ExpireEffect()
	{
		var check = Gameworld.GetCheck(CheckType.UnjamGun);
		var difficulty = Difficulty.Normal;
		if (CharacterOwner.Combat is not null)
		{
			difficulty = difficulty.StageUp(2);
		}

		if (CharacterOwner.IsEngagedInMelee)
		{
			difficulty = difficulty.StageUp(2);
		}

		var outcome = check.Check(CharacterOwner, difficulty, Weapon.Parent, Weapon);
		if (outcome.IsFail())
		{
			CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote(Weapon.FailUnjamEmote, CharacterOwner, CharacterOwner, Weapon.Parent, Ramrod)));
			Owner.Reschedule(this, EffectDuration(CharacterOwner, Weapon, Ramrod));
			return;
		}

		CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote(Weapon.FinishUnjamEmote, CharacterOwner, CharacterOwner, Weapon.Parent, Ramrod)));
		Weapon.IsJammed = false;
		Owner.RemoveEffect(this, true);
	}
	#endregion
}

public class LoadingMusket : CharacterActionWithTarget
{
	public IRangedWeapon Weapon { get; private set; }

	public LoadMode LoadMode { get; private set; }

	public LoadingMusket(ICharacter owner, IRangedWeapon weapon, LoadMode loadMode) : base(owner, weapon.Parent)
	{
		Weapon = weapon;
		LoadMode = loadMode;
		WhyCannotMoveEmoteString = "@ cannot move because #0 %0|are|is loading $1";
		ActionDescription = "loading $1";
	}

	protected override string SpecificEffectType => "LoadingMusket";

	public override string Describe(IPerceiver voyeur)
	{
		return $"Loading {Weapon.Parent.HowSeen(voyeur)} in the {LoadMode.DescribeEnum().ColourName()} mode";
	}

	#region Overrides of Effect

	/// <inheritdoc />
	public override void ExpireEffect()
	{
		base.ExpireEffect();
		if (Weapon.LoadStage < 4)
		{
			Weapon.Load((ICharacter)Owner, false, LoadMode);
		}
	}

	#endregion
}

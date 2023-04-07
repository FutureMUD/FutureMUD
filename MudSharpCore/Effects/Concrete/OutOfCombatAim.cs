using System;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.Combat;
using MudSharp.Construction.Boundary;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;

namespace MudSharp.Effects.Concrete;

public class OutOfCombatAim : Effect, IRemoveOnCombatStart, ILDescSuffixEffect
{
	private static TimeSpan? _effectLength;

	public static TimeSpan EffectLength
	{
		get
		{
			if (_effectLength == null)
			{
				_effectLength =
					TimeSpan.FromSeconds(Futuremud.Games.First().GetStaticDouble("OutOfCombatAimTickSeconds"));
			}

			return _effectLength.Value;
		}
	}

	private static double? _combatAimMajorSuccessPercentage;

	public static double CombatAimMajorSuccessPercentage
	{
		get
		{
			if (_combatAimMajorSuccessPercentage == null)
			{
				_combatAimMajorSuccessPercentage =
					Futuremud.Games.First().GetStaticDouble("CombatAimMajorSuccessPercentage");
			}

			return _combatAimMajorSuccessPercentage.Value;
		}
	}

	private int _fullAimTicks = 0;

	public ICharacter CharacterOwner { get; set; }
	public AimInformation Aim { get; set; }
	public IPerceiver Target { get; set; }
	public IRangedWeapon Weapon { get; set; }

	public override IEnumerable<string> Blocks => new[] { "general", "movement", "aim" };

	public override string BlockingDescription(string blockingType, IPerceiver voyeur)
	{
		return $"aiming {Weapon.Parent.HowSeen(voyeur)}";
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"aiming {Weapon.Parent.HowSeen(voyeur)} at {Target?.HowSeen(voyeur) ?? "the sky"}";
	}

	protected override string SpecificEffectType => "OutOfCombatAim";

	public OutOfCombatAim(ICharacter owner, IPerceiver target, IRangedWeapon weapon, IEnumerable<ICellExit> path)
		: base(owner)
	{
		CharacterOwner = owner;
		Weapon = weapon;
		Target = target;
		Aim = new AimInformation(target, owner, path, weapon);
		RegisterEvents();
	}

	public override void Login()
	{
		RegisterEvents();
	}

	protected void ReleaseEvents()
	{
		Aim.AimInvalidated -= Aim_AimInvalidated;
	}

	protected void RegisterEvents()
	{
		Aim.AimInvalidated += Aim_AimInvalidated;
	}

	private void Aim_AimInvalidated(object sender, EventArgs e)
	{
		Owner.RemoveEffect(this, true);
	}

	#region Overrides of Effect

	/// <summary>Fires when an effect is removed, including a matured scheduled effect</summary>
	public override void RemovalEffect()
	{
		base.RemovalEffect();
		ReleaseEvents();
		//If character has since joined combat, don't clean up the AimInfo since it has been passed over
		//to the character for combat and will be cleaned up by combat.
		if (CharacterOwner.Combat == null)
		{
			Aim.ReleaseEvents();
			Aim = null;
		}
	}

	#endregion

	public double AimCompletion
	{
		get => Aim.AimPercentage;
		set => Aim.AimPercentage = value;
	}

	public override bool CanBeStoppedByPlayer => true;

	public override bool IsBlockingEffect(string blockingType)
	{
		return string.IsNullOrEmpty(blockingType) || base.IsBlockingEffect(blockingType);
	}

	/// <summary>Fires when an effect is removed, including a matured scheduled effect</summary>
	public override void CancelEffect()
	{
		Aim.ReleaseEvents();
	}

	protected static double GetShooterPositionBonus(IPositionState position)
	{
		if (position.CompareTo(PositionKneeling.Instance) == PositionHeightComparison.Equivalent)
		{
			return 1.10;
		}

		if (position.CompareTo(PositionKneeling.Instance) == PositionHeightComparison.Lower)
		{
			return 1.25;
		}

		return 1.0;
	}

	public override void ExpireEffect()
	{
		var check = Gameworld.GetCheck(CheckType.AimRangedWeapon);
		var postureBonus = 1.0;

		if (Target?.Location != Owner.Location)
		{
			var position = CharacterOwner.Body.PositionState;
			postureBonus = GetShooterPositionBonus(position);

			//TODO - Bows shouldn't be usable from Prone, going prone with a bow should prevent/abort aiming
		}

		//These aim amounts are intentionally not the same as in-combat aiming, which gains aim bonus
		//slower and gains nothing on fail/majorfail. The idea is that someone learning can start with
		//out of combat aiming
		if (AimCompletion < 1.0)
		{
			//If already capped on aiming, don't do more aim checks for free skillup chances.
			var result = check.Check(CharacterOwner, Weapon.AimDifficulty, Weapon.WeaponType.FireTrait, Target);
			var aimAmount = 0.0;
			switch (result.Outcome)
			{
				case Outcome.MajorFail:
					aimAmount = 0.05 * CombatAimMajorSuccessPercentage;
					break;
				case Outcome.Fail:
					aimAmount = 0.1 * CombatAimMajorSuccessPercentage;
					break;
				case Outcome.MinorFail:
					aimAmount = 0.2 * CombatAimMajorSuccessPercentage;
					break;
				case Outcome.MinorPass:
					aimAmount = 0.3 * CombatAimMajorSuccessPercentage;
					break;
				case Outcome.Pass:
					aimAmount = 0.5 * CombatAimMajorSuccessPercentage;
					break;
				case Outcome.MajorPass:
					aimAmount = 1.0 * CombatAimMajorSuccessPercentage;
					break;
			}

			aimAmount = aimAmount * postureBonus;
			AimCompletion = Math.Min(1.0, AimCompletion + aimAmount);
		}

		// Owner.OutputHandler.Handle(new EmoteOutput(new Emote("@ continue|continues to aim $2 at $1.", CharacterOwner, CharacterOwner, Target, Weapon.Parent), flags: OutputFlags.SuppressObscured));
		if (AimCompletion >= 1.0 && _fullAimTicks++ % 20 == 0 && Target != Owner)
		{
			CharacterOwner.Send("You feel like you have the best aim you could possibly get.");
		}

		Gameworld.EffectScheduler.AddSchedule(new EffectSchedule(this, EffectLength));
	}

	#region Implementation for ILDescSuffixEffect

	public string SuffixFor(IPerceiver voyeur)
	{
		string suffix;
		if (Target == null)
		{
			suffix = "into the air";
		}
		else if (Target.IsSelf(Owner))
		{
			if (Owner.IsSelf(voyeur))
			{
				suffix = "at yourself";
			}
			else
			{
				suffix = $"at {CharacterOwner.ApparentGender(voyeur).Reflexive()}";
			}
		}
		else
		{
			suffix = $"at {Target.HowSeen(voyeur)}";
		}

		return $"aiming {Weapon.Parent.Name.A_An().ToLowerInvariant()} {suffix}";
	}

	public bool SuffixApplies()
	{
		return true;
	}

	#endregion
}
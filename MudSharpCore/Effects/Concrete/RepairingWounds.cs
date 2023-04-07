using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Effects.Concrete;

public class RepairingWounds : CharacterActionWithTarget, IAffectProximity
{
	private static TimeSpan? _baseEffectDuration;

	private static TimeSpan BaseEffectDuration
	{
		get
		{
			if (_baseEffectDuration == null)
			{
				_baseEffectDuration =
					TimeSpan.FromSeconds(Futuremud.Games.First().GetStaticDouble("RepairEffectTotalDuration"));
			}

			return _baseEffectDuration.Value;
		}
	}

	public static TimeSpan EffectDuration(int phases)
	{
		return TimeSpan.FromSeconds(BaseEffectDuration.TotalSeconds / phases);
	}

	public IRepairKit RepairKit { get; set; }

	public bool RepairWorstFirst { get; set; }

	public int TotalPhases { get; set; }
	public int CurrentPhase { get; set; }

	#region Overrides of Effect

	public override string Describe(IPerceiver voyeur)
	{
		return $"Repairing damage to {Target.HowSeen(voyeur)}.";
	}

	protected override string SpecificEffectType => "RepairingWounds";

	#endregion

	public RepairingWounds(ICharacter owner, IPerceivable target, IRepairKit repairKit, bool repairWorstFirst) :
		base(owner, target)
	{
		WhyCannotMoveEmoteString = "@ cannot move because $0 $0|are|is repairing damage to $1.";
		CancelEmoteString = "@ $0|stop|stops repairing damage to $1.";
		LDescAddendum = "repairing damage to $1";
		ActionDescription = "reparing damage to $1";
		RepairKit = repairKit;
		RepairWorstFirst = repairWorstFirst;
		TotalPhases = repairKit.Echoes.Count();
		CurrentPhase = 1;
		_blocks.Add("general");
		_blocks.Add("movement");
	}

	public (bool Affects, Proximity Proximity) GetProximityFor(IPerceivable thing)
	{
		if (Target == thing)
		{
			return (true, Proximity.Immediate);
		}

		return (false, Proximity.Unapproximable);
	}

	#region Overrides of TargetedBlockingDelayedAction

	/// <summary>
	///     Fires when an effect is removed, including a matured scheduled effect
	/// </summary>
	public override void RemovalEffect()
	{
		ReleaseEventHandlers();
	}

	#endregion

	public override void ExpireEffect()
	{
		CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote(RepairKit.Echoes.ElementAt(CurrentPhase - 1),
			CharacterOwner, CharacterOwner, Target, RepairKit.Parent)));
		if (CurrentPhase++ < TotalPhases)
		{
			CharacterOwner.Reschedule(this, EffectDuration(TotalPhases));
			return;
		}

		List<IWound> wounds;
		if (TargetCharacter != null)
		{
			wounds = TargetCharacter.VisibleWounds(CharacterOwner, WoundExaminationType.Examination).ToList();
		}
		else
		{
			wounds = TargetItem.Wounds.ToList();
		}

		wounds.RemoveAll(x => !RepairKit.CanRepair(x).Success);

		if (!wounds.Any())
		{
			CharacterOwner.OutputHandler.Handle(new EmoteOutput(
				new Emote(
					"@ have|has finished repairing the damage to $1 with $2.",
					CharacterOwner, CharacterOwner, Target, RepairKit.Parent)));
			Owner.RemoveEffect(this, true);
			return;
		}

		Difficulty targetDifficulty;
		if (RepairWorstFirst)
		{
			targetDifficulty = wounds.Select(x => x.CanBeTreated(TreatmentType.Repair))
			                         .Where(x => x != Difficulty.Impossible)
			                         .DefaultIfEmpty()
			                         .Max();
		}
		else
		{
			targetDifficulty = wounds.Select(x => x.CanBeTreated(TreatmentType.Repair))
			                         .Where(x => x != Difficulty.Impossible)
			                         .DefaultIfEmpty()
			                         .Min();
		}

		RepairKit.Repair(wounds.Where(x => x.CanBeTreated(TreatmentType.Repair) == targetDifficulty).ToList(),
			CharacterOwner);
		if (TargetCharacter != null)
		{
			wounds = TargetCharacter.VisibleWounds(CharacterOwner, WoundExaminationType.Examination).ToList();
		}
		else
		{
			wounds = TargetItem.Wounds.ToList();
		}

		if (wounds.Any(x => RepairKit.CanRepair(x).Success))
		{
			CharacterOwner.OutputHandler.Handle(new EmoteOutput(
				new Emote(
					"@ continue|continues &0's repairs, as $1 still $1|have|has further damage.",
					CharacterOwner, CharacterOwner, Target)));
			CharacterOwner.Reschedule(this, EffectDuration(TotalPhases));
			CurrentPhase = 1;
			return;
		}

		CharacterOwner.OutputHandler.Handle(new EmoteOutput(
			new Emote(
				"@ have|has finished repairing the damage to $1.",
				CharacterOwner, CharacterOwner, Target)));
		Owner.RemoveEffect(this, true);
	}
}
using System;
using System.Linq;
using MudSharp.Body;
using MudSharp.Body.PartProtos;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Concrete;

public class PerformingCPR : CharacterActionWithTarget
{
	public int TickCount { get; set; }

	public PerformingCPR(ICharacter owner, ICharacter target) : base(owner, target, null, "performing CPR on $1",
		"$0 stop|stops performing CPR on $1.", "You cannot move because $0 $0|are|is performing CPR on $1.",
		new[] { "general", "movement" }, null)
	{
		target.Body.AddEffect(new CPRTarget(target.Body));
	}

	public override void ExpireEffect()
	{
		var targetHeartFunction = TargetCharacter.Body.Organs.OfType<HeartProto>()
		                                         .Select(x => x.OrganFunctionFactor(TargetCharacter.Body))
		                                         .DefaultIfEmpty(0)
		                                         .Sum();
		var check = Gameworld.GetCheck(CheckType.PerformCPR);
		var result = check.Check(CharacterOwner, targetHeartFunction <= 0.0 ? Difficulty.Hard : Difficulty.Normal,
			TargetCharacter);
		if (RandomUtilities.DoubleRandom(0.0, 1.0) <= (1.0 + result.Outcome.CheckDegrees() * 0.15) *
		    Gameworld.GetStaticDouble(targetHeartFunction <= 0.0
			    ? "ReturnOfSpontaneousCirculationChanceCardiacArrest"
			    : "ReturnOfSpontaneousCirculationChanceHeartAttack"))
		{
			TargetCharacter.Body.AddEffect(new StablisedOrganFunction(TargetCharacter.Body,
				TargetCharacter.Body.Organs.OfType<HeartProto>().GetRandomElement(), 0.3,
				targetHeartFunction > 0.0 ? ExertionLevel.Normal : ExertionLevel.Low));
			if (TargetCharacter.Body.NeedsToBreathe && TargetCharacter.Body.CanBreathe)
			{
				TargetCharacter.OutputHandler.Handle(new EmoteOutput(
					new Emote("@ gasp|gasps suddenly as &0's breathing returns.", TargetCharacter, TargetCharacter)));
			}

			TargetCharacter.Body.CheckHealthStatus();
		}

		CharacterOwner.SpendStamina(Gameworld.GetStaticDouble("CPRStaminaCost"));
		if (CharacterOwner.Body.CurrentStamina <= 0.0)
		{
			Owner.OutputHandler.Handle(new EmoteOutput(new Emote(
				"@ stop|stops performing CPR on $1 because #0 %0|are|is too exhausted to continue.", CharacterOwner,
				CharacterOwner, TargetCharacter)));
			Owner.RemoveEffect(this);
			ReleaseEventHandlers();
			return;
		}

		if (CharacterState.Able.HasFlag(TargetCharacter.State) ||
		    (TargetCharacter.IsBreathing && TargetCharacter.NeedsToBreathe))
		{
			Owner.OutputHandler.Handle(new EmoteOutput(new Emote(
				"@ stop|stops performing CPR on $1 because #1 %1|are|is now responsive.", CharacterOwner,
				CharacterOwner, TargetCharacter)));
			Owner.RemoveEffect(this);
			ReleaseEventHandlers();
			return;
		}

		if (TargetCharacter.IsEngagedInMelee)
		{
			Owner.OutputHandler.Handle(new EmoteOutput(new Emote(
				"@ stop|stops performing CPR on $1 because #1 %1|are|is engaged in melee combat.", CharacterOwner,
				CharacterOwner, TargetCharacter)));
			Owner.RemoveEffect(this);
			ReleaseEventHandlers();
			return;
		}

		if (TickCount++ >= 6)
		{
			TickCount = 0;
			Owner.OutputHandler.Handle(new EmoteOutput(new Emote("@ continue|continues to perform CPR on $1.",
				CharacterOwner, CharacterOwner, TargetCharacter)));
		}

		Owner.Reschedule(this, TimeSpan.FromSeconds(5));
	}

	public override void RemovalEffect()
	{
		ReleaseEventHandlers();
	}

	protected override void ReleaseEventHandlers()
	{
		base.ReleaseEventHandlers();
		TargetCharacter.Body.RemoveAllEffects(x => x.IsEffectType<CPRTarget>());
	}
}
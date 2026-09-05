#nullable enable

using MudSharp.RPG.Checks;
using MudSharp.Body.Traits;

namespace MudSharp.Magic;

public static class MentalActionService
{
	public static bool CanAttempt(MentalActionContext context) =>
		(!context.Hostile || PsionicTrafficHelper.CanReceiveInvoluntaryMentalTraffic(context.Target)) &&
		MagicInterdictionHelper.GetInterdiction(context.Source, context.Target, context.Power.School, false) is null;
	public static MagicInvocationResult Resolve(MentalActionContext context, ITraitDefinition trait,
		Difficulty difficulty, Outcome threshold)
	{
		if (!CanAttempt(context))
		{
			return new MagicInvocationResult(MagicInvocationStatus.Refused);
		}

		var result = context.Source.Gameworld.GetCheck(CheckType.MagicTelepathyCheck)
		                    .Check(context.Source, difficulty, trait, context.Target);
		if (context.Hostile) context.Source.RemoveAllEffects<MudSharp.Effects.Concrete.AttentionSuppressionEffect>(fireRemovalAction: true);
		var success = result.Outcome >= threshold;
		if (success && context.Hostile && context.Source != context.Target)
		{
			var defence = context.Source.Gameworld.GetCheck(CheckType.ResistMagicSpellCheck)
			                     .Check(context.Target, difficulty, context.Source, externalBonus:
							context.Target.Effects.OfType<IMentalActionDefence>().Sum(x => x.DefensiveBonus(context)) +
							-context.Target.EffectsOfType<MudSharp.Effects.Concrete.MindBarrierEffect>().Where(x => x.Applies(context.Source)).Sum(x => x.Bonus));
			success = result.Outcome > defence.Outcome;
		}

		var resolution = new MagicInvocationResult(success ? MagicInvocationStatus.Succeeded : MagicInvocationStatus.Failed,
			result.Outcome);
		foreach (var reaction in context.Target.Effects.OfType<IMentalActionReaction>().ToList())
		{
			reaction.OnMentalAction(context, resolution);
		}

		return resolution;
	}
}

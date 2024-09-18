using System;
using System.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.RPG.Law.PatrolStrategies;

public class SheriffPatrolStrategy : PatrolStrategyBase
{
	/// <inheritdoc />
	public override string Name => "Sheriff";

	public SheriffPatrolStrategy(IFuturemud gameworld) : base(gameworld)
	{
	}

	protected override void PatrolTickPreparationPhase(IPatrol patrol)
	{
		if (patrol.PatrolMembers.All(x => x.Location == patrol.LegalAuthority.CourtLocation))
		{
			patrol.PatrolPhase = PatrolPhase.Patrol;
			patrol.LastArrivedTime = DateTime.UtcNow;
			patrol.LastMajorNode = patrol.LegalAuthority.CourtLocation;
			patrol.NextMajorNode = patrol.LegalAuthority.CourtLocation;
			return;
		}

		base.PatrolTickPreparationPhase(patrol);
	}

	protected void CheckJusticeSystem(IPatrol patrol)
	{
		var authority = patrol.LegalAuthority;

		// Is there already a criminal in the court?
		if (patrol.LegalAuthority.CourtLocation.Characters.Any(x =>
			    x.EffectsOfType<OnTrial>(y => y.LegalAuthority == authority).Any()))
		{
			return;
		}

		var now = authority.EnforcementZones.FirstOrDefault()?.DateTime() ??
		          Gameworld.Calendars.First().CurrentDateTime;

		ICharacter trialCandidate = null;
		// Are there any criminals awaiting sentencing?
		foreach (var criminal in authority.KnownCrimes.Where(x => x.EligableForAutomaticConviction())
		                                  .Select(x => x.Criminal).Distinct())
		{
			var awaitingEffect = criminal.EffectsOfType<AwaitingSentencing>(x => x.LegalAuthority == authority)
			                             .FirstOrDefault();
			if (awaitingEffect is null)
			{
				continue;
			}

			if (criminal.AffectedBy<OnBail>(this) || criminal.AffectedBy<InCustodyOfEnforcer>())
			{
				continue;
			}

			if (now - awaitingEffect.ArrestTime < authority.AutomaticConvictionTime)
			{
				continue;
			}

			trialCandidate = criminal;
			break;
		}

		if (trialCandidate is null)
		{
			return;
		}

		// Fetch the criminal for trial
		var crimes = authority.KnownCrimesForIndividual(trialCandidate).ToList();
		trialCandidate.RemoveAllEffects<AwaitingSentencing>(x => x.LegalAuthority == authority, true);
		trialCandidate.AddEffect(new OnTrial(trialCandidate, authority, DateTime.UtcNow, crimes));
		trialCandidate.OutputHandler.Handle(new EmoteOutput(
			new Emote(Gameworld.GetStaticString("FetchCriminalForTrialEmoteCell"), trialCandidate, trialCandidate),
			flags: OutputFlags.SuppressSource));
		trialCandidate.OutputHandler.Send(new EmoteOutput(
			new Emote(Gameworld.GetStaticString("FetchCriminalForTrialEmoteSelf"), trialCandidate, trialCandidate)));
		trialCandidate.Movement?.CancelForMoverOnly(trialCandidate);
		trialCandidate.RemoveAllEffects(x => x.IsEffectType<IActionEffect>());
		trialCandidate.Location.Leave(trialCandidate);
		trialCandidate.RoomLayer = RoomLayer.GroundLevel;
		authority.CourtLocation.Enter(trialCandidate);
		trialCandidate.Body.Look(true);
		trialCandidate.OutputHandler.Handle(new EmoteOutput(
			new Emote(Gameworld.GetStaticString("FetchCriminalForTrialEmoteCourt"), trialCandidate, trialCandidate),
			flags: OutputFlags.SuppressSource));
	}

	protected override void PatrolTickPatrolPhase(IPatrol patrol)
	{
		if (patrol.ActiveEnforcementTarget != null)
		{
			return;
		}

		// Patrol can only be completed if there is no trial on-going
		if (patrol.PatrolLeader.Location.LayerCharacters(patrol.PatrolLeader.RoomLayer).All(x => !x.EffectsOfType<OnTrial>(y => y.LegalAuthority == patrol.LegalAuthority).Any()))
		{
			if (DateTime.UtcNow - patrol.LastArrivedTime >= patrol.PatrolRoute.LingerTimeMajorNode)
			{
				patrol.CompletePatrol();
				return;
			}

			if (!patrol.PatrolRoute.TimeOfDays.Contains(patrol.PatrolLeader.Location.CurrentTimeOfDay))
			{
				patrol.CompletePatrol();
				return;
			}
		}

		if (patrol.PatrolLeader.Location != patrol.NextMajorNode)
		{
			var effect = patrol.PatrolLeader.CombinedEffectsOfType<FollowingPath>().FirstOrDefault();
			if (effect != null)
			{
				return;
			}

			var path = patrol.PatrolLeader
			                 .PathBetween(patrol.NextMajorNode, 20,
				                 PathSearch.PathIncludeUnlockedDoors(patrol.PatrolLeader))
			                 .ToList();
			if (!path.Any())
			{
				// Abort the patrol - no viable routes found
				patrol.AbortPatrol();
				return;
			}

			var fp = new FollowingPath(patrol.PatrolLeader, path);
			patrol.PatrolLeader.AddEffect(fp);
			fp.FollowPathAction();
			return;
		}

		CheckJusticeSystem(patrol);
	}
}
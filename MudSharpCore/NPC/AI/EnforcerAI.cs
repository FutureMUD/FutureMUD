using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore.Internal;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Movement;
using MudSharp.RPG.Law;

namespace MudSharp.NPC.AI;

public class EnforcerAI : ArtificialIntelligenceBase
{
	public static void RegisterLoader()
	{
		RegisterAIType("Enforcer", (ai, gameworld) => new EnforcerAI(ai, gameworld));
	}

	protected EnforcerAI(ArtificialIntelligence ai, IFuturemud gameworld) : base(ai, gameworld)
	{
		var root = XElement.Parse(ai.Definition);
		IdentityIsKnownProg = long.TryParse(root.Element("IdentityProg")?.Value ?? "0", out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(root.Element("IdentityProg")!.Value);
		WarnEchoProg = long.TryParse(root.Element("WarnEchoProg")?.Value ?? "0", out value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(root.Element("WarnEchoProg")!.Value);
		WarnStartMoveEchoProg = long.TryParse(root.Element("WarnStartMoveEchoProg")?.Value ?? "0", out value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(root.Element("WarnStartMoveEchoProg")!.Value);
		FailToComplyEchoProg = long.TryParse(root.Element("FailToComplyEchoProg")?.Value ?? "0", out value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(root.Element("FailToComplyEchoProg")!.Value);
		ThrowInPrisonEchoProg = long.TryParse(root.Element("ThrowInPrisonEchoProg")?.Value ?? "0", out value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(root.Element("ThrowInPrisonEchoProg")!.Value);
	}

	public IFutureProg IdentityIsKnownProg { get; protected set; }
	public IFutureProg WarnEchoProg { get; protected set; }
	public IFutureProg WarnStartMoveEchoProg { get; protected set; }
	public IFutureProg FailToComplyEchoProg { get; protected set; }
	public IFutureProg ThrowInPrisonEchoProg { get; protected set; }

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		switch (type)
		{
			case EventType.CharacterIncapacitatedWitness:
				return CharacterIncapacitatedWitness((ICharacter)arguments[0], (ICharacter)arguments[1]);
			case EventType.TargetIncapacitated:
				return TargetIncapacitated((ICharacter)arguments[1], (ICharacter)arguments[0]);
			case EventType.NoLongerEngagedInMelee:
			case EventType.TargetSlain:
			case EventType.TruceOffered:
				return false;
			case EventType.WitnessedCrime:
				return WitnessedCrime((ICharacter)arguments[0], (ICharacter)arguments[1], (ICharacter)arguments[2],
					(ICrime)arguments[3]);
			case EventType.FiveSecondTick:
				return CharacterFiveSecondTick((ICharacter)arguments[0]);
		}

		return false;
	}

	private bool CharacterIncapacitatedWitness(ICharacter victim, ICharacter character)
	{
		return false;
	}

	private bool TargetIncapacitated(ICharacter victim, ICharacter character)
	{
		var patrolMember = character.CombinedEffectsOfType<PatrolMemberEffect>().FirstOrDefault();
		if (patrolMember == null || patrolMember.Patrol.ActiveEnforcementTarget != victim || patrolMember.Patrol
			    .ActiveEnforcementCrime?.Law.EnforcementStrategy.ShowMercyToIncapacitatedTarget() !=
		    false)
		{
			character.Combat?.TruceRequested(character);
		}

		return false;
	}

	private EnforcerEffect EnforcerEffect(ICharacter enforcer)
	{
		return enforcer.EffectsOfType<EnforcerEffect>().FirstOrDefault();
	}

	private bool WitnessedCrime(ICharacter criminal, ICharacter victim, ICharacter enforcer, ICrime crime)
	{
		// Enforcers always report crimes whether they're on duty or off duty
		crime.LegalAuthority.ReportCrime(crime, enforcer,
			IdentityIsKnownProg?.Execute<bool?>(enforcer, criminal) == true, 1.0);
		return false;
	}

	private bool HandleGeneral(ICharacter enforcer)
	{
		// If not currently on a patrol, ignore AI
		var effect = EnforcerEffect(enforcer);
		if (effect == null)
		{
			return true;
		}

		// Pause AI while moving or in combat
		if (enforcer.Movement != null || enforcer.Combat != null)
		{
			return true;
		}

		var patrolMember = enforcer.CombinedEffectsOfType<PatrolMemberEffect>().FirstOrDefault();
		if (patrolMember == null && enforcer.Location == effect.LegalAuthority.EnforcerStowingLocation)
		{
			return true;
		}

		// Try to wake up if somehow put to sleep
		if (enforcer.State.IsAsleep())
		{
			enforcer.Awaken();
			return true;
		}

		// Try to stand up if knocked over
		var mobilePosition = enforcer.MostUprightMobilePosition();
		if (mobilePosition != null && enforcer.PositionState != mobilePosition)
		{
			enforcer.MovePosition(mobilePosition, null, null);
			return true;
		}

		// If already following a path, don't do other things
		if (enforcer.AffectedBy<FollowingPath>())
		{
			return false;
		}

		if (patrolMember == null)
		{
			return false;
		}

		var patrol = patrolMember.Patrol;
		// If not the patrol leader and not in the same place as the patrol leader, try to regroup with them
		if (patrol.PatrolLeader != enforcer && !enforcer.ColocatedWith(patrol.PatrolLeader))
		{
			var path = enforcer.PathBetween(patrol.PatrolLeader, 10, PathSearch.PathIncludeUnlockableDoors(enforcer))
			                   .ToList();
			if (path.Any())
			{
				var fp = new FollowingPath(enforcer, path) { UseDoorguards = true, UseKeys = true, OpenDoors = true };
				enforcer.AddEffect(fp);
				fp.FollowPathAction();
				return true;
			}

			return true;
		}

		if (enforcer.Party != patrol.PatrolLeader.Party && patrol.PatrolLeader.Party != null)
		{
			enforcer.JoinParty(patrol.PatrolLeader.Party);
		}

		return false;
	}

	private bool CharacterFiveSecondTick(ICharacter enforcer)
	{
		var effect = EnforcerEffect(enforcer);
		if (effect == null)
		{
			return false;
		}

		if (HandleGeneral(enforcer))
		{
			return false;
		}

		var patrol = enforcer.CombinedEffectsOfType<PatrolMemberEffect>().FirstOrDefault()?.Patrol;
		var fp = enforcer.CombinedEffectsOfType<FollowingPath>().FirstOrDefault();
		if (patrol != null)
		{
			if (HandlePatrolMember(enforcer, patrol))
			{
				if (fp != null)
				{
					if (enforcer == patrol?.PatrolLeader && patrol.PatrolPhase == PatrolPhase.Patrol &&
					    patrol.ActiveEnforcementTarget is null &&
					    DateTime.UtcNow - patrol.LastArrivedTime < patrol.PatrolRoute.LingerTimeMinorNode)
					{
						return false;
					}

					if (enforcer.CouldMove(false, null).Success)
					{
						fp.FollowPathAction();
					}
				}

				return true;
			}
		}

		if (fp != null)
		{
			if (enforcer == patrol?.PatrolLeader && patrol.PatrolPhase == PatrolPhase.Patrol &&
			    patrol.ActiveEnforcementTarget is null &&
			    DateTime.UtcNow - patrol.LastArrivedTime < patrol.PatrolRoute.LingerTimeMinorNode)
			{
				return false;
			}

			if (enforcer.CouldMove(false, null).Success)
			{
				fp.FollowPathAction();
			}

			return true;
		}

		if (patrol == null && enforcer.Location != effect.LegalAuthority.EnforcerStowingLocation)
		{
			var path = enforcer.PathBetween(effect.LegalAuthority.EnforcerStowingLocation, 50,
				PathSearch.PathIncludeUnlockableDoors(enforcer)).ToList();
			if (path.Any())
			{
				fp = new FollowingPath(enforcer, path) { UseDoorguards = true, UseKeys = true, OpenDoors = true };
				enforcer.AddEffect(fp);
				fp.FollowPathAction();
			}
		}

		return false;
	}

	private bool HandlePatrolMember(ICharacter enforcer, IPatrol patrol)
	{
		if (patrol.ActiveEnforcementTarget != null)
		{
			return true;
		}

		if (patrol.PatrolLeader == enforcer)
		{
			var fp = enforcer.CombinedEffectsOfType<FollowingPath>().FirstOrDefault();
			if (fp != null && enforcer.CouldMove(false, null).Success)
			{
				var major = patrol.PatrolRoute.PatrolNodes.Contains(enforcer.Location);
				if ((major && DateTime.UtcNow - patrol.LastArrivedTime >= patrol.PatrolRoute.LingerTimeMajorNode) ||
				    (!major && DateTime.UtcNow - patrol.LastArrivedTime >= patrol.PatrolRoute.LingerTimeMinorNode))
				{
					fp.FollowPathAction();
					return true;
				}
			}

			if (patrol.PatrolPhase == PatrolPhase.Patrol)
			{
				return true;
			}
		}

		return false;
	}

	public override bool HandlesEvent(params EventType[] types)
	{
		foreach (var type in types)
		{
			switch (type)
			{
				case EventType.CharacterIncapacitatedWitness:
				case EventType.TargetIncapacitated:
				case EventType.NoLongerEngagedInMelee:
				case EventType.TargetSlain:
				case EventType.TruceOffered:
				case EventType.WitnessedCrime:
				case EventType.FiveSecondTick:
					return true;
			}
		}

		return false;
	}
}
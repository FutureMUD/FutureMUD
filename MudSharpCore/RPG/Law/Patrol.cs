using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Movement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Database;
using MudSharp.Models;
using MudSharp.NPC;
using MudSharp.NPC.AI;
using MudSharp.PerceptionEngine;

namespace MudSharp.RPG.Law;

public class Patrol : SaveableItem, IPatrol
{
	public Patrol(ILegalAuthority authority, IPatrolRoute route, ICharacter leader, IEnumerable<ICharacter> members)
	{
		Gameworld = authority.Gameworld;
		LegalAuthority = authority;
		PatrolRoute = route;
		PatrolStrategy = route.PatrolStrategy;
		PatrolPhase = PatrolPhase.Preperation;
		LastArrivedTime = DateTime.UtcNow;
		PatrolLeader = leader;
		_members.AddRange(members);
		using (new FMDB())
		{
			var dbitem = new Models.Patrol
			{
				LegalAuthorityId = authority.Id,
				PatrolRouteId = route.Id,
				PatrolLeaderId = leader.Id,
				PatrolPhase = (int)PatrolPhase.Preperation
			};
			dbitem.PatrolMembers =
				members.Select(x => new PatrolMember { Patrol = dbitem, CharacterId = x.Id }).ToList();

			FMDB.Context.Patrols.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}

		Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat += HandlePatrolTick;
		Gameworld.Add(this);
		foreach (var member in members)
		{
			member.AddEffect(new PatrolMemberEffect(member, this));
		}
	}

	public Patrol(Models.Patrol patrol, ILegalAuthority authority)
	{
		Gameworld = authority.Gameworld;
		LegalAuthority = authority;
		PatrolRoute = authority.PatrolRoutes.First(x => x.Id == patrol.PatrolRouteId);
		PatrolStrategy = PatrolRoute.PatrolStrategy;
		_id = patrol.Id;
		PatrolPhase = (PatrolPhase)patrol.PatrolPhase;
		LastArrivedTime = DateTime.UtcNow;
		LastMajorNode = Gameworld.Cells.Get(patrol.LastMajorNodeId ?? 0);
		NextMajorNode = Gameworld.Cells.Get(patrol.NextMajorNodeId ?? 0);
		_members.AddRange(patrol.PatrolMembers.SelectNotNull(x => Gameworld.NPCs.Get(x.CharacterId)));
		PatrolLeader = Gameworld.NPCs.Get(patrol.PatrolLeaderId ?? 0) ?? PatrolMembers.GetRandomElement();
		Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat += HandlePatrolTick;
		Gameworld.Add(this);
	}

	#region Overrides of FrameworkItem

	/// <inheritdoc />
	public override string Name => PatrolRoute.Name;

	#endregion

	public ILegalAuthority LegalAuthority { get; set; }
	public IPatrolRoute PatrolRoute { get; }
	public IPatrolStrategy PatrolStrategy { get; }
	private readonly List<ICharacter> _members = new();
	public IEnumerable<ICharacter> PatrolMembers => _members;
	public ICharacter PatrolLeader { get; set; }
	public PatrolPhase PatrolPhase { get; set; }
	public ICell LastMajorNode { get; set; }
	public ICell NextMajorNode { get; set; }
	public DateTime LastArrivedTime { get; set; }
	public ICharacter ActiveEnforcementTarget { get; set; }
	public ICrime ActiveEnforcementCrime { get; set; }
	public ICell OriginLocation => LegalAuthority.MarshallingLocation;

	public void Delete()
	{
		Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat -= HandlePatrolTick;
		Gameworld.SaveManager.Abort(this);
		Gameworld.Destroy(this);
		using (new FMDB())
		{
			Gameworld.SaveManager.Flush();
			var dbitem = FMDB.Context.Patrols.Find(Id);
			if (dbitem != null)
			{
				FMDB.Context.Patrols.Remove(dbitem);
				FMDB.Context.SaveChanges();
			}
		}
	}

	public void ConcludePatrol()
	{
		foreach (var member in PatrolMembers.ToList())
		{
			member.RemoveAllEffects<PatrolMemberEffect>(fireRemovalAction: true);
		}

		PatrolLeader.Party?.Disband();
		LegalAuthority.PatrolController.ReportPatrolComplete(this);
		Delete();
	}

	private bool EnsureLeader()
	{
		if (PatrolLeader == null || PatrolLeader.State.IsDead())
		{
			PatrolLeader = PatrolMembers.GetRandomElement();
			Changed = true;
		}

		if (PatrolLeader == null)
		{
			AbortPatrol();
			return false;
		}

		return true;
	}

	private void HandlePatrolTick()
	{
		if (!EnsureLeader())
		{
			return;
		}
		PatrolStrategy.HandlePatrolTick(this);
	}

	public void AbortPatrol()
	{
		foreach (var member in PatrolMembers.ToList())
		{
			member.RemoveAllEffects<PatrolMemberEffect>(fireRemovalAction: true);
		}

		PatrolLeader?.Party?.Disband();
		LegalAuthority.PatrolController.ReportPatrolAborted(this);
		Delete();
	}

	public void CompletePatrol()
	{
		PatrolPhase = PatrolPhase.Return;
		LastMajorNode = NextMajorNode;
		NextMajorNode = null;
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.Patrols.Find(Id);
		dbitem.LastMajorNodeId = LastMajorNode?.Id;
		dbitem.NextMajorNodeId = NextMajorNode?.Id;
		dbitem.PatrolLeaderId = PatrolLeader?.Id;
		dbitem.PatrolPhase = (int)PatrolPhase;
		dbitem.PatrolMembers.Clear();
		foreach (var item in PatrolMembers)
		{
			dbitem.PatrolMembers.Add(new PatrolMember { Patrol = dbitem, CharacterId = item.Id });
		}

		Changed = false;
	}

	public override string FrameworkItemType => "Patrol";

	public void CriminalFailedToComply(ICharacter criminal, ICrime crime)
	{
		// Sanity check if they are complying
		if (criminal.IsHelpless)
		{
			return;
		}

		var ai = ((INPC)PatrolLeader).AIs.OfType<EnforcerAI>().First();
		foreach (var action in (ai.FailToComplyEchoProg?.Execute<string>(PatrolLeader, criminal, crime) ?? string.Empty)
		         .Split('\n'))
		{
			PatrolLeader.ExecuteCommand(action);
		}

		var newcrime = LegalAuthority.CheckPossibleCrime(criminal, CrimeTypes.ResistArrest, null, null, "")
		                             .FirstOrDefault();
		if (newcrime != null)
		{
			ActiveEnforcementCrime = newcrime;
		}
	}

	public void CriminalStartedMoving(ICharacter criminal, ICrime crime)
	{
		var ai = ((INPC)PatrolLeader).AIs.OfType<EnforcerAI>().First();
		foreach (var action in
		         (ai.WarnStartMoveEchoProg?.Execute<string>(PatrolLeader, criminal, crime) ?? string.Empty).Split('\n'))
		{
			PatrolLeader.ExecuteCommand(action);
		}
	}

	public void WarnCriminal(ICharacter criminal, ICrime crime)
	{
		if (crime.Law.EnforcementStrategy.In(EnforcementStrategy.KillOnSight,
			    EnforcementStrategy.ArrestAndDetainNoWarning, EnforcementStrategy.LethalForceArrestAndDetainNoWarning))
			// Don't warn kill on sight enemies
		{
			return;
		}

		criminal.AddEffect(new WarnedByEnforcer(criminal, LegalAuthority, crime, this), TimeSpan.FromSeconds(90));

		var ai = ((INPC)PatrolLeader).AIs.OfType<EnforcerAI>().First();
		foreach (var action in
		         (ai.WarnEchoProg?.Execute<string>(PatrolLeader, criminal, crime) ?? string.Empty).Split('\n'))
		{
			PatrolLeader.ExecuteCommand(action);
		}

		criminal.Send($"Hint: Type {"surrender".MXPSend("surrender")} to surrender to the enforcer.".ColourCommand());
	}

	public void InvalidateActiveCrime()
	{
		ActiveEnforcementTarget.RemoveAllEffects<WarnedByEnforcer>(x => x.WhichCrime == ActiveEnforcementCrime, true);
		ActiveEnforcementTarget = null;
		ActiveEnforcementCrime = null;
	}

	public void RemovePatrolMember(ICharacter character)
	{
		_members.Remove(character);
		PatrolLeader?.Party?.Leave(character);
	}
}
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.Health;
using MudSharp.Movement;
using MudSharp.NPC;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.TimeAndDate;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.RPG.Law.PatrolStrategies;

public enum ExecutionPatrolExecutionMethod
{
	CoupDeGraceWithWeapon,
	AdministerDrug,
	FiringSquad
}

internal enum ExecutionPatrolStage
{
	SelectingTarget,
	PreparingEquipment,
	RetrievingPrisoner,
	ComplianceWindow,
	SubduingPrisoner,
	TakingToExecutionRoom,
	RestrainingPrisoner,
	LastWords,
	Script,
	Killing,
	ConfirmingDeath
}

public class ExecutionPatrolStrategy : PatrolStrategyBase, IConfigurablePatrolStrategy
{
	private readonly IInventoryPlanTemplate _meleeWeaponTemplate;
	private readonly IInventoryPlanTemplate _rangedWeaponTemplate;
	private readonly IInventoryPlanTemplate _injectorTemplate;
	private readonly IInventoryPlanTemplate _restraintTemplate;

	private ICharacter _condemned;
	private long _condemnedId;
	private ExecutionPatrolStage _stage = ExecutionPatrolStage.SelectingTarget;
	private DateTime _stageBegan = DateTime.UtcNow;
	private DateTime _lastAction = DateTime.MinValue;
	private int _scriptIndex;
	private int _executionAttempts;
	private bool _retrieveEmoteSent;
	private bool _resistEmoteSent;
	private bool _arrivalEmoteSent;
	private bool _restraintEmoteSent;
	private bool _lastWordsEmoteSent;

	public override string Name => "ExecutionPatrol";

	public ExecutionPatrolStrategy(IFuturemud gameworld, string strategyData = null) : base(gameworld)
	{
		_meleeWeaponTemplate = new InventoryPlanTemplate(Gameworld, new InventoryPlanActionWield(Gameworld, 0, 0,
			item => item.GetItemType<IMeleeWeapon>() is IMeleeWeapon weapon &&
					weapon.WeaponType.Attacks.OfType<IFixedBodypartWeaponAttack>()
						  .Any(),
			null)
		{
			PrimaryItemFitnessScorer = item => item.GetItemType<IMeleeWeapon>()?.WeaponType.Classification switch
			{
				WeaponClassification.Ceremonial => 100.0,
				WeaponClassification.Military => 80.0,
				WeaponClassification.Lethal => 60.0,
				WeaponClassification.NonLethal => -100.0,
				WeaponClassification.Training => -1000.0,
				WeaponClassification.Shield => -500.0,
				_ => 1.0
			},
			ItemsAlreadyInPlaceMultiplier = 10.0
		});

		_rangedWeaponTemplate = new InventoryPlanTemplate(Gameworld, new InventoryPlanActionWield(Gameworld, 0, 0,
			item => item.IsItemType<IRangedWeapon>(), null)
		{
			PrimaryItemFitnessScorer = item => item.GetItemType<IRangedWeapon>()?.Classification switch
			{
				WeaponClassification.Military => 100.0,
				WeaponClassification.Lethal => 80.0,
				WeaponClassification.Ceremonial => 30.0,
				WeaponClassification.NonLethal => -200.0,
				WeaponClassification.Training => -500.0,
				WeaponClassification.Shield => -1000.0,
				_ => 1.0
			},
			ItemsAlreadyInPlaceMultiplier = 10.0
		});

		_injectorTemplate = new InventoryPlanTemplate(Gameworld, new InventoryPlanActionHold(Gameworld, 0, 0,
			item => item.IsItemType<IInject>(), null, 1)
		{
			ItemsAlreadyInPlaceMultiplier = 10.0
		});

		_restraintTemplate = new InventoryPlanTemplate(Gameworld, new InventoryPlanActionHold(Gameworld, 0, 0,
			item => item.IsItemType<IRestraint>(), null, 1)
		{
			ItemsAlreadyInPlaceMultiplier = 10.0
		});

		LoadStrategyData(strategyData);
	}

	public ExecutionPatrolExecutionMethod Method { get; private set; } = ExecutionPatrolExecutionMethod.CoupDeGraceWithWeapon;
	public long EquipmentLocationId { get; private set; }
	public long DrugId { get; private set; }
	public double DrugGrams { get; private set; } = 1.0;
	public DrugVector DrugVector { get; private set; } = DrugVector.Injected;
	public int ComplianceWindowSeconds { get; private set; } = 60;
	public int LastWordsSeconds { get; private set; } = 30;
	public int ScriptDelaySeconds { get; private set; } = 10;
	public int DeathConfirmationSeconds { get; private set; } = 30;
	public int MaximumExecutionAttempts { get; private set; } = 5;
	public string RetrieveEmote { get; private set; } = "@ tell|tells $1, \"Your sentence is to be carried out now. Submit peacefully or make yourself helpless.\"";
	public string ResistEmote { get; private set; } = "@ signal|signals to the guards to subdue $1.";
	public string ArrivalEmote { get; private set; } = "@ lead|leads $1 into the place of execution.";
	public string RestraintEmote { get; private set; } = "@ secure|secures $1 in place for the execution.";
	public string LastWordsEmote { get; private set; } = "@ tell|tells $1, \"You may speak your final words now.\"";
	public string DrugEmote { get; private set; } = "@ administer|administers the execution drug to $1.";
	public string FiringSquadEmote { get; private set; } = "@ give|gives the order to fire on $1.";
	public string CompletionEmote { get; private set; } = "@ confirm|confirms that $1's sentence has been carried out.";
	private readonly List<string> _executionScript = new()
	{
		"@ announce|announces, \"$1 has been sentenced to death by lawful authority.\"",
		"@ pause|pauses solemnly before the sentence is carried out."
	};

	public string HelpText => @"Execution patrol configuration:

	#3method cdg|drug|firing#0 - sets the execution method
	#3equipment here|<room>|none#0 - sets the room used to retrieve tools, defaulting to the legal authority preparation room
	#3drug <id|name>#0 - sets the drug for the administer-drug method
	#3dose <grams>#0 - sets the grams of drug administered per attempt
	#3vector injected|ingested|inhaled|touched#0 - sets the drug vector
	#3window <seconds>#0 - sets how long the prisoner has to go helpless or surrender
	#3lastwordsdelay <seconds>#0 - sets how long to wait after the last-words emote
	#3scriptdelay <seconds>#0 - sets the delay between script steps
	#3confirmdelay <seconds>#0 - sets how long to wait between execution attempts
	#3attempts <number>#0 - sets the maximum execution attempts before aborting
	#3emote retrieve|resist|arrival|restrain|lastwords|drug|firing|complete <emote>#0 - sets a custom emote
	#3script add <emote>#0 - adds a scripted execution step
	#3script delete <##>#0 - deletes a scripted execution step
	#3script swap <##> <##>#0 - swaps two scripted execution steps
	#3script clear#0 - clears the scripted execution steps";

	public bool ReadyToBegin(IPatrolRoute patrol)
	{
		return GetExecutionLocation(patrol) is not null &&
			   ConfigurationIsComplete() &&
			   patrol.LegalAuthority.Patrols.All(x => x.PatrolStrategy.Name != Name) &&
			   SelectDueCondemned(patrol.LegalAuthority) is not null;
	}

	private bool ConfigurationIsComplete()
	{
		return Method != ExecutionPatrolExecutionMethod.AdministerDrug || DrugId > 0;
	}

	private ICell GetEquipmentLocation(IPatrolRoute patrol)
	{
		return EquipmentLocationId > 0
			? Gameworld.Cells.Get(EquipmentLocationId)
			: patrol.LegalAuthority.PreparingLocation;
	}

	private ICell GetExecutionLocation(IPatrolRoute patrol)
	{
		return patrol.PatrolNodes.FirstOrDefault();
	}

	private MudDateTime CurrentLegalTime(ILegalAuthority authority)
	{
		return authority.EnforcementZones.FirstOrDefault()?.DateTime() ?? Gameworld.Calendars.First().CurrentDateTime;
	}

	private ICharacter SelectDueCondemned(ILegalAuthority authority)
	{
		MudDateTime now = CurrentLegalTime(authority);
		return Gameworld.Actors
		                .Where(x => !x.State.IsDead())
		                .Where(x => !x.CombinedEffectsOfType<ExecutionPatrolNoQuit>().Any(y => y.Applies()))
		                .Select(x => (Character: x,
			                Effect: x.EffectsOfType<AwaitingExecution>(y =>
					                y.LegalAuthority == authority &&
					                y.ExecutionDate <= now)
				                .OrderBy(y => y.ExecutionDate)
				                .FirstOrDefault()))
		                .Where(x => x.Effect is not null)
		                .OrderBy(x => x.Effect.ExecutionDate)
		                .Select(x => x.Character)
		                .FirstOrDefault();
	}

	private bool EnsureCondemned(IPatrol patrol)
	{
		if (_condemned is not null)
		{
			return true;
		}

		if (_condemnedId > 0)
		{
			_condemned = Gameworld.TryGetCharacter(_condemnedId, true);
		}

		if (_condemned is not null)
		{
			return true;
		}

		_condemned = SelectDueCondemned(patrol.LegalAuthority);
		_condemnedId = _condemned?.Id ?? 0;
		if (_condemned is null)
		{
			patrol.ConcludePatrol();
			return false;
		}

		EnsureNoQuitEffect(patrol);
		return true;
	}

	private void EnsureNoQuitEffect(IPatrol patrol)
	{
		if (_condemned is null)
		{
			return;
		}

		if (!_condemned.EffectsOfType<ExecutionPatrolNoQuit>(x => x.Patrol == patrol).Any())
		{
			_condemned.AddEffect(new ExecutionPatrolNoQuit(_condemned, patrol));
		}
	}

	public override void HandlePatrolTick(IPatrol patrol)
	{
		if (patrol.PatrolPhase == PatrolPhase.Return)
		{
			PatrolTickReturnPhase(patrol);
			return;
		}

		if (!EnsureCondemned(patrol))
		{
			return;
		}

		EnsureNoQuitEffect(patrol);

		if (_condemned.State.IsDead())
		{
			CompleteExecution(patrol);
			return;
		}

		switch (_stage)
		{
			case ExecutionPatrolStage.SelectingTarget:
			case ExecutionPatrolStage.PreparingEquipment:
				HandlePreparingEquipment(patrol);
				return;
			case ExecutionPatrolStage.RetrievingPrisoner:
				HandleRetrievingPrisoner(patrol);
				return;
			case ExecutionPatrolStage.ComplianceWindow:
				HandleComplianceWindow(patrol);
				return;
			case ExecutionPatrolStage.SubduingPrisoner:
				HandleSubduingPrisoner(patrol);
				return;
			case ExecutionPatrolStage.TakingToExecutionRoom:
				HandleTakingToExecutionRoom(patrol);
				return;
			case ExecutionPatrolStage.RestrainingPrisoner:
				HandleRestrainingPrisoner(patrol);
				return;
			case ExecutionPatrolStage.LastWords:
				HandleLastWords(patrol);
				return;
			case ExecutionPatrolStage.Script:
				HandleScript(patrol);
				return;
			case ExecutionPatrolStage.Killing:
				HandleKilling(patrol);
				return;
			case ExecutionPatrolStage.ConfirmingDeath:
				HandleConfirmingDeath(patrol);
				return;
		}
	}

	protected override void PatrolTickPatrolPhase(IPatrol patrol)
	{
		HandlePatrolTick(patrol);
	}

	private void SetStage(ExecutionPatrolStage stage)
	{
		_stage = stage;
		_stageBegan = DateTime.UtcNow;
		_lastAction = DateTime.MinValue;
	}

	private bool MoveCharacterTo(ICharacter character, ICell target, int maximumDistance = 50)
	{
		if (character.Location == target)
		{
			return true;
		}

		if (character.CombinedEffectsOfType<FollowingPath>().Any())
		{
			return false;
		}

		List<ICellExit> path = character.PathBetween(target, (uint)maximumDistance,
			PathSearch.PathIncludeUnlockableDoors(character)).ToList();
		if (!path.Any())
		{
			path = character.PathBetween(target, (uint)maximumDistance, PathSearch.IgnorePresenceOfDoors).ToList();
			if (!path.Any())
			{
				return false;
			}
		}

		FollowingPath fp = new(character, path)
		{
			UseDoorguards = true,
			UseKeys = true,
			OpenDoors = true
		};
		character.AddEffect(fp);
		fp.FollowPathAction();
		return false;
	}

	private bool MoveLeaderToCharacter(IPatrol patrol, ICharacter target)
	{
		ICharacter leader = patrol.PatrolLeader;
		if (leader.ColocatedWith(target))
		{
			return true;
		}

		if (leader.CombinedEffectsOfType<FollowingPath>().Any())
		{
			return false;
		}

		List<ICellExit> path = leader.PathBetween(target, 50, PathSearch.PathIncludeUnlockableDoors(leader)).ToList();
		if (!path.Any())
		{
			path = leader.PathBetween(target, 50, PathSearch.IgnorePresenceOfDoors).ToList();
			if (!path.Any())
			{
				return false;
			}
		}

		FollowingPath fp = new(leader, path)
		{
			UseDoorguards = true,
			UseKeys = true,
			OpenDoors = true
		};
		leader.AddEffect(fp);
		fp.FollowPathAction();
		return false;
	}

	private void HandlePreparingEquipment(IPatrol patrol)
	{
		if (_stage == ExecutionPatrolStage.SelectingTarget)
		{
			SetStage(ExecutionPatrolStage.PreparingEquipment);
		}

		ICell equipment = GetEquipmentLocation(patrol.PatrolRoute);
		if (equipment is null)
		{
			AbortExecution(patrol);
			return;
		}

		foreach (ICharacter member in patrol.PatrolMembers)
		{
			if (member.Location != equipment)
			{
				MoveCharacterTo(member, equipment, 25);
				continue;
			}

			DoPreparationRoomAction(member, member == patrol.PatrolLeader);
		}

		if (patrol.PatrolMembers.Any(x => x.Location != equipment))
		{
			if (DateTime.UtcNow - _stageBegan > TimeSpan.FromMinutes(3))
			{
				AbortExecution(patrol);
			}

			return;
		}

		if (!EquipmentReady(patrol))
		{
			if (DateTime.UtcNow - _stageBegan > TimeSpan.FromMinutes(3))
			{
				AbortExecution(patrol);
			}

			return;
		}

		FormParty(patrol);
		patrol.PatrolPhase = PatrolPhase.Deployment;
		patrol.LastArrivedTime = DateTime.UtcNow;
		patrol.LastMajorNode = equipment;
		SetStage(ExecutionPatrolStage.RetrievingPrisoner);
	}

	private void FormParty(IPatrol patrol)
	{
		if (patrol.PatrolLeader.Party is not null)
		{
			patrol.PatrolLeader.LeaveParty();
		}

		Party party = new(patrol.PatrolLeader);
		patrol.PatrolLeader.JoinParty(party);
		foreach (ICharacter member in patrol.PatrolMembers.Where(x => x.ColocatedWith(patrol.PatrolLeader)).ToList())
		{
			if (member == patrol.PatrolLeader)
			{
				continue;
			}

			member.LeaveParty();
			member.JoinParty(party);
		}
	}

	private void DoPreparationRoomAction(ICharacter member, bool isLeader)
	{
		PrepareRestraints(member);

		switch (Method)
		{
			case ExecutionPatrolExecutionMethod.CoupDeGraceWithWeapon:
				if (isLeader)
				{
					PrepareInventoryPlan(member, _meleeWeaponTemplate);
				}
				break;
			case ExecutionPatrolExecutionMethod.AdministerDrug:
				if (isLeader)
				{
					PrepareInventoryPlan(member, _injectorTemplate);
				}
				break;
			case ExecutionPatrolExecutionMethod.FiringSquad:
				PrepareInventoryPlan(member, _rangedWeaponTemplate);
				ReadyRangedWeapons(member);
				break;
		}
	}

	private void PrepareRestraints(ICharacter member)
	{
		PrepareInventoryPlan(member, _restraintTemplate);
	}

	private static void PrepareInventoryPlan(ICharacter member, IInventoryPlanTemplate template)
	{
		IInventoryPlan plan = template.CreatePlan(member);
		if (plan.PlanIsFeasible() == InventoryPlanFeasibility.Feasible)
		{
			plan.ExecuteWholePlan();
		}

		plan.FinalisePlanNoRestore();
	}

	private bool EquipmentReady(IPatrol patrol)
	{
		return Method switch
		{
			ExecutionPatrolExecutionMethod.CoupDeGraceWithWeapon => GetCoupDeGraceAttack(patrol.PatrolLeader) is not null,
			ExecutionPatrolExecutionMethod.AdministerDrug => DrugId > 0,
			ExecutionPatrolExecutionMethod.FiringSquad => patrol.PatrolMembers.Any(x => ReadyRangedWeapons(x)),
			_ => false
		};
	}

	private bool ReadyRangedWeapons(ICharacter shooter)
	{
		bool result = false;
		foreach (IRangedWeapon weapon in shooter.Body.WieldedItems.SelectNotNull(x => x.GetItemType<IRangedWeapon>()))
		{
			if (weapon.CanLoad(shooter, true))
			{
				weapon.Load(shooter, true);
			}

			if (weapon.CanReady(shooter))
			{
				result |= weapon.Ready(shooter);
			}

			result |= weapon.ReadyToFire;
		}

		return result;
	}

	private void HandleRetrievingPrisoner(IPatrol patrol)
	{
		if (!MoveLeaderToCharacter(patrol, _condemned))
		{
			if (DateTime.UtcNow - _stageBegan > TimeSpan.FromMinutes(3))
			{
				AbortExecution(patrol);
			}

			return;
		}

		if (!_retrieveEmoteSent)
		{
			DoEmote(patrol.PatrolLeader, RetrieveEmote, _condemned);
			_condemned.Send($"Hint: Type {"helpless".MXPSend("helpless")} to stop resisting, or {"surrender".MXPSend("surrender")} {patrol.PatrolLeader.HowSeen(_condemned)} to submit to the guards.".ColourCommand());
			_retrieveEmoteSent = true;
		}

		SetStage(ExecutionPatrolStage.ComplianceWindow);
	}

	private void HandleComplianceWindow(IPatrol patrol)
	{
		if (!patrol.PatrolLeader.ColocatedWith(_condemned))
		{
			SetStage(ExecutionPatrolStage.RetrievingPrisoner);
			return;
		}

		if (_condemned.IsHelpless || IsBeingDraggedByPatrol(patrol))
		{
			EnsureDraggedByPatrol(patrol);
			SetStage(ExecutionPatrolStage.TakingToExecutionRoom);
			return;
		}

		if (DateTime.UtcNow - _stageBegan < TimeSpan.FromSeconds(ComplianceWindowSeconds))
		{
			return;
		}

		if (!_resistEmoteSent)
		{
			DoEmote(patrol.PatrolLeader, ResistEmote, _condemned);
			_resistEmoteSent = true;
		}

		SetStage(ExecutionPatrolStage.SubduingPrisoner);
		HandleSubduingPrisoner(patrol);
	}

	private bool IsBeingDraggedByPatrol(IPatrol patrol)
	{
		return patrol.PatrolMembers.Any(x => x.CombinedEffectsOfType<Dragging>().Any(y => y.Target == _condemned));
	}

	private void HandleSubduingPrisoner(IPatrol patrol)
	{
		if (_condemned.IsHelpless || IsBeingDraggedByPatrol(patrol))
		{
			EnsureDraggedByPatrol(patrol);
			SetStage(ExecutionPatrolStage.TakingToExecutionRoom);
			return;
		}

		if (!patrol.PatrolLeader.ColocatedWith(_condemned))
		{
			SetStage(ExecutionPatrolStage.RetrievingPrisoner);
			return;
		}

		foreach (ICharacter member in patrol.PatrolMembers.Where(x => x.ColocatedWith(_condemned)))
		{
			EngageToSubdue(member);
		}
	}

	private void EngageToSubdue(ICharacter member)
	{
		if (member.CombatTarget == _condemned)
		{
			return;
		}

		ICharacterCombatSettings grapple = member.Gameworld.CharacterCombatSettings
		                                      .FirstOrDefault(x =>
			                                      x.PreferredMeleeMode.In(CombatStrategyMode.GrappleForControl,
				                                      CombatStrategyMode.GrappleForIncapacitation) &&
			                                      x.CanUse(member));
		if (grapple is not null)
		{
			member.CombatSettings = grapple;
		}

		if (member.CanEngage(_condemned))
		{
			member.Engage(_condemned);
		}
	}

	private bool EnsureDraggedByPatrol(IPatrol patrol)
	{
		if (IsBeingDraggedByPatrol(patrol))
		{
			return true;
		}

		ICharacter dragger = patrol.PatrolMembers
		                            .Where(x => x.ColocatedWith(_condemned))
		                            .Where(x => x.State.IsAble())
		                            .GetRandomElement();
		if (dragger is null)
		{
			return false;
		}

		if (dragger.Combat?.CanFreelyLeaveCombat(dragger) == true)
		{
			dragger.Combat.LeaveCombat(dragger);
		}

		dragger.ExecuteCommand($"drag {dragger.BestKeywordFor(_condemned)}");
		if (!dragger.CombinedEffectsOfType<Dragging>().Any(x => x.Target == _condemned))
		{
			return false;
		}

		foreach (ICharacter helper in patrol.PatrolMembers.Except(dragger).Where(x => x.ColocatedWith(dragger)))
		{
			if (helper.Combat?.CanFreelyLeaveCombat(helper) == true)
			{
				helper.Combat.LeaveCombat(helper);
			}

			helper.ExecuteCommand($"drag help {helper.BestKeywordFor(dragger)}");
		}

		return true;
	}

	private void HandleTakingToExecutionRoom(IPatrol patrol)
	{
		ICell executionLocation = GetExecutionLocation(patrol.PatrolRoute);
		if (executionLocation is null)
		{
			AbortExecution(patrol);
			return;
		}

		if (!IsBeingDraggedByPatrol(patrol) && _condemned.Location != executionLocation)
		{
			if (!patrol.PatrolLeader.ColocatedWith(_condemned))
			{
				SetStage(ExecutionPatrolStage.RetrievingPrisoner);
				return;
			}

			if (!EnsureDraggedByPatrol(patrol))
			{
				SetStage(ExecutionPatrolStage.SubduingPrisoner);
				return;
			}
		}

		if (patrol.PatrolLeader.Location == executionLocation && _condemned.Location == executionLocation)
		{
			_condemned.RemoveAllEffects<Dragging.DragTarget>(fireRemovalAction: true);
			foreach (ICharacter member in patrol.PatrolMembers)
			{
				member.RemoveAllEffects<Dragging>(x => x.Target == _condemned, fireRemovalAction: true);
			}

			if (!_arrivalEmoteSent)
			{
				DoEmote(patrol.PatrolLeader, ArrivalEmote, _condemned);
				_arrivalEmoteSent = true;
			}

			SetStage(ExecutionPatrolStage.RestrainingPrisoner);
			return;
		}

		if (!MoveCharacterTo(patrol.PatrolLeader, executionLocation))
		{
			if (DateTime.UtcNow - _stageBegan > TimeSpan.FromMinutes(3))
			{
				AbortExecution(patrol);
			}
		}
	}

	private void HandleRestrainingPrisoner(IPatrol patrol)
	{
		ICell executionLocation = GetExecutionLocation(patrol.PatrolRoute);
		if (_condemned.Location != executionLocation)
		{
			SetStage(ExecutionPatrolStage.TakingToExecutionRoom);
			return;
		}

		if (_condemned.Body.EffectsOfType<RestraintEffect>().Any())
		{
			SetStage(ExecutionPatrolStage.LastWords);
			return;
		}

		if (TryRestrainCondemned(patrol))
		{
			if (!_restraintEmoteSent)
			{
				DoEmote(patrol.PatrolLeader, RestraintEmote, _condemned);
				_restraintEmoteSent = true;
			}

			SetStage(ExecutionPatrolStage.LastWords);
			return;
		}

		if (DateTime.UtcNow - _stageBegan > TimeSpan.FromMinutes(3))
		{
			AbortExecution(patrol);
		}
	}

	private bool TryRestrainCondemned(IPatrol patrol)
	{
		ICharacter restrainer = patrol.PatrolMembers
		                              .Where(x => x.ColocatedWith(_condemned))
		                              .FirstOrDefault(x => x.Body.HeldItems.Any(y => y.IsItemType<IRestraint>())) ??
		                       patrol.PatrolMembers.FirstOrDefault(x => x.ColocatedWith(_condemned));
		if (restrainer is null)
		{
			return false;
		}

		PrepareRestraints(restrainer);
		IGameItem item = restrainer.Body.HeldItems.FirstOrDefault(x => x.IsItemType<IRestraint>());
		if (item is null)
		{
			item = restrainer.Location.LayerGameItems(restrainer.RoomLayer)
			                 .FirstOrDefault(x => x.IsItemType<IRestraint>() &&
			                                      restrainer.Body.CanGet(x, 0,
				                                      ItemCanGetIgnore.IgnoreInventoryPlans |
				                                      ItemCanGetIgnore.IgnoreFreeHands));
			if (item is not null)
			{
				restrainer.Body.Get(item, silent: true,
					ignoreFlags: ItemCanGetIgnore.IgnoreInventoryPlans | ItemCanGetIgnore.IgnoreFreeHands);
			}
		}

		if (item?.GetItemType<IRestraint>() is not IRestraint restraint)
		{
			return false;
		}

		if (!restraint.CanRestrainCreature(_condemned))
		{
			return false;
		}

		IWearable wearable = item.GetItemType<IWearable>();
		if (wearable is null)
		{
			return false;
		}

		IWearProfile profile = wearable.Profiles
		                               .Where(x => _condemned.Body.Prototype.CountsAs(x.DesignedBody) &&
		                                           x.Profile(_condemned.Body) is not null)
		                               .FirstOrDefault(x => x == wearable.DefaultProfile) ??
		                       wearable.Profiles.FirstOrDefault(x => _condemned.Body.Prototype.CountsAs(x.DesignedBody) &&
		                                                              x.Profile(_condemned.Body) is not null);
		if (profile is null)
		{
			return false;
		}

		IGameItem targetItem = null;
		if (profile.AllProfiles.Select(x => _condemned.Body.GetLimbFor(x.Key)?.LimbType)
		           .Any(x => x == LimbType.Head || x == LimbType.Torso))
		{
			targetItem = restrainer.Location.LayerGameItems(restrainer.RoomLayer)
			                       .Where(x => x != item)
			                       .Where(x => !x.IsItemType<IRestraint>())
			                       .Where(x => !x.IsItemType<IHoldable>() || x.Size >= _condemned.SizeSitting)
			                       .FirstOrDefault(x => restrainer.Location.CanGetAccess(x, restrainer));
			if (targetItem is null)
			{
				return false;
			}
		}

		_condemned.Body.Restrain(item, profile, restrainer, targetItem);
		return _condemned.Body.EffectsOfType<RestraintEffect>().Any(x => x.RestraintItem == item);
	}

	private bool CondemnedReadyForExecution(IPatrol patrol)
	{
		ICell executionLocation = GetExecutionLocation(patrol.PatrolRoute);
		if (executionLocation is null)
		{
			AbortExecution(patrol);
			return false;
		}

		if (_condemned.Location != executionLocation ||
		    patrol.PatrolLeader.Location != executionLocation)
		{
			ResetCeremonyProgress();
			SetStage(ExecutionPatrolStage.TakingToExecutionRoom);
			return false;
		}

		if (!_condemned.Body.EffectsOfType<RestraintEffect>().Any())
		{
			ResetCeremonyProgress();
			SetStage(ExecutionPatrolStage.RestrainingPrisoner);
			return false;
		}

		return true;
	}

	private void ResetCeremonyProgress()
	{
		_lastWordsEmoteSent = false;
		_scriptIndex = 0;
		_lastAction = DateTime.MinValue;
	}

	private void HandleLastWords(IPatrol patrol)
	{
		if (!CondemnedReadyForExecution(patrol))
		{
			return;
		}

		if (!_lastWordsEmoteSent)
		{
			DoEmote(patrol.PatrolLeader, LastWordsEmote, _condemned);
			_lastWordsEmoteSent = true;
			_lastAction = DateTime.UtcNow;
			return;
		}

		if (DateTime.UtcNow - _lastAction < TimeSpan.FromSeconds(LastWordsSeconds))
		{
			return;
		}

		SetStage(ExecutionPatrolStage.Script);
	}

	private void HandleScript(IPatrol patrol)
	{
		if (!CondemnedReadyForExecution(patrol))
		{
			return;
		}

		if (_scriptIndex >= _executionScript.Count)
		{
			SetStage(ExecutionPatrolStage.Killing);
			return;
		}

		if (_lastAction != DateTime.MinValue &&
		    DateTime.UtcNow - _lastAction < TimeSpan.FromSeconds(ScriptDelaySeconds))
		{
			return;
		}

		DoEmote(patrol.PatrolLeader, _executionScript[_scriptIndex++], _condemned);
		_lastAction = DateTime.UtcNow;
	}

	private void HandleKilling(IPatrol patrol)
	{
		if (!CondemnedReadyForExecution(patrol))
		{
			return;
		}

		if (_executionAttempts >= MaximumExecutionAttempts)
		{
			AbortExecution(patrol);
			return;
		}

		bool attempted = Method switch
		{
			ExecutionPatrolExecutionMethod.CoupDeGraceWithWeapon => TryCoupDeGrace(patrol),
			ExecutionPatrolExecutionMethod.AdministerDrug => TryAdministerDrug(patrol),
			ExecutionPatrolExecutionMethod.FiringSquad => TryFiringSquad(patrol),
			_ => false
		};

		if (!attempted)
		{
			if (DateTime.UtcNow - _stageBegan > TimeSpan.FromMinutes(3))
			{
				AbortExecution(patrol);
			}

			return;
		}

		_executionAttempts++;
		SetStage(ExecutionPatrolStage.ConfirmingDeath);
	}

	private void HandleConfirmingDeath(IPatrol patrol)
	{
		if (_condemned.State.IsDead())
		{
			DoEmote(patrol.PatrolLeader, CompletionEmote, _condemned);
			CompleteExecution(patrol);
			return;
		}

		if (!CondemnedReadyForExecution(patrol))
		{
			return;
		}

		if (DateTime.UtcNow - _stageBegan >= TimeSpan.FromSeconds(DeathConfirmationSeconds))
		{
			SetStage(ExecutionPatrolStage.Killing);
		}
	}

	private (IMeleeWeapon Weapon, IFixedBodypartWeaponAttack Attack)? GetCoupDeGraceAttack(ICharacter executioner)
	{
		foreach (IMeleeWeapon weapon in executioner.Body.WieldedItems.SelectNotNull(x => x.GetItemType<IMeleeWeapon>()))
		{
			IFixedBodypartWeaponAttack attack = weapon.WeaponType.Attacks
			                                          .OfType<IFixedBodypartWeaponAttack>()
			                                          .Where(x => x.UsableAttack(executioner, weapon.Parent, _condemned,
				                                          weapon.HandednessForWeapon(executioner), false,
				                                          BuiltInCombatMoveType.CoupDeGrace))
			                                          .GetWeightedRandom(x => x.Weighting);
			if (attack is not null)
			{
				return (weapon, attack);
			}
		}

		return null;
	}

	private bool TryCoupDeGrace(IPatrol patrol)
	{
		var coupDeGrace = GetCoupDeGraceAttack(patrol.PatrolLeader);
		if (coupDeGrace is null)
		{
			PrepareInventoryPlan(patrol.PatrolLeader, _meleeWeaponTemplate);
			coupDeGrace = GetCoupDeGraceAttack(patrol.PatrolLeader);
			if (coupDeGrace is null)
			{
				return false;
			}
		}

		var (weapon, attack) = coupDeGrace.Value;
		CoupDeGrace move = new(attack, _condemned)
		{
			Assailant = patrol.PatrolLeader,
			Weapon = weapon
		};
		CombatMoveResult result = move.ResolveMove(null);
		move.ResolveBloodSpray(result);
		patrol.PatrolLeader.SpendStamina(move.StaminaCost);
		return true;
	}

	private bool TryAdministerDrug(IPatrol patrol)
	{
		IDrug drug = Gameworld.Drugs.Get(DrugId);
		if (drug is null)
		{
			return false;
		}

		DoEmote(patrol.PatrolLeader, DrugEmote, _condemned);
		_condemned.Body.Dose(drug, DrugVector, DrugGrams, patrol.PatrolLeader);
		_condemned.Body.CheckDrugTick();
		return true;
	}

	private bool TryFiringSquad(IPatrol patrol)
	{
		DoEmote(patrol.PatrolLeader, FiringSquadEmote, _condemned);
		bool anyFired = false;
		foreach (ICharacter shooter in patrol.PatrolMembers.Where(x => x.ColocatedWith(_condemned)))
		{
			IRangedWeapon weapon = shooter.Body.WieldedItems
			                            .SelectNotNull(x => x.GetItemType<IRangedWeapon>())
			                            .FirstOrDefault(x => x.ReadyToFire || x.CanReady(shooter));
			if (weapon is null)
			{
				PrepareInventoryPlan(shooter, _rangedWeaponTemplate);
				ReadyRangedWeapons(shooter);
				weapon = shooter.Body.WieldedItems
				                .SelectNotNull(x => x.GetItemType<IRangedWeapon>())
				                .FirstOrDefault(x => x.ReadyToFire);
			}

			if (weapon is null)
			{
				continue;
			}

			if (!weapon.ReadyToFire)
			{
				ReadyRangedWeapons(shooter);
			}

			if (!weapon.ReadyToFire || !weapon.CanFire(shooter, _condemned))
			{
				continue;
			}

			StandAndFireMove move = new(shooter, _condemned, weapon);
			move.ResolveMove(null);
			shooter.SpendStamina(move.StaminaCost);
			anyFired = true;
		}

		return anyFired;
	}

	private void CompleteExecution(IPatrol patrol)
	{
		if (_condemned is not null)
		{
			_condemned.RemoveAllEffects<ExecutionPatrolNoQuit>(x => x.Patrol == patrol, fireRemovalAction: true);
			_condemned.RemoveAllEffects<AwaitingExecution>(x => x.LegalAuthority == patrol.LegalAuthority, fireRemovalAction: true);
			foreach (ICrime crime in patrol.LegalAuthority.ResolvedCrimesForIndividual(_condemned)
			                             .Where(x => x.ExecutionPunishment && !x.SentenceHasBeenServed))
			{
				crime.SentenceHasBeenServed = true;
			}
		}

		ResetRuntimeState();
		patrol.CompletePatrol();
	}

	private void AbortExecution(IPatrol patrol)
	{
		if (_condemned is not null)
		{
			_condemned.RemoveAllEffects<ExecutionPatrolNoQuit>(x => x.Patrol == patrol, fireRemovalAction: true);
		}

		ResetRuntimeState();
		patrol.AbortPatrol();
	}

	private void ResetRuntimeState()
	{
		_condemned = null;
		_condemnedId = 0;
		_stage = ExecutionPatrolStage.SelectingTarget;
		_stageBegan = DateTime.UtcNow;
		_lastAction = DateTime.MinValue;
		_scriptIndex = 0;
		_executionAttempts = 0;
		_retrieveEmoteSent = false;
		_resistEmoteSent = false;
		_arrivalEmoteSent = false;
		_restraintEmoteSent = false;
		_lastWordsEmoteSent = false;
	}

	private static void DoEmote(ICharacter actor, string emoteText, ICharacter condemned)
	{
		Emote emote = new(emoteText, actor, actor, condemned);
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return;
		}

		actor.OutputHandler.Handle(new EmoteOutput(emote));
	}

	public bool BuildingCommand(ICharacter actor, IPatrolRoute patrol, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "method":
				return BuildingCommandMethod(actor, command);
			case "equipment":
			case "equip":
			case "room":
				return BuildingCommandEquipment(actor, command);
			case "drug":
				return BuildingCommandDrug(actor, command);
			case "dose":
			case "grams":
				return BuildingCommandDose(actor, command);
			case "vector":
				return BuildingCommandVector(actor, command);
			case "window":
			case "compliance":
				return BuildingCommandComplianceWindow(actor, command);
			case "lastwordsdelay":
			case "lastworddelay":
				return BuildingCommandLastWordsDelay(actor, command);
			case "scriptdelay":
				return BuildingCommandScriptDelay(actor, command);
			case "confirmdelay":
			case "deathdelay":
				return BuildingCommandDeathConfirmationDelay(actor, command);
			case "attempts":
				return BuildingCommandAttempts(actor, command);
			case "emote":
				return BuildingCommandEmote(actor, command);
			case "script":
				return BuildingCommandScript(actor, command);
			default:
				actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandMethod(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which execution method do you want to use: cdg, drug or firing?");
			return false;
		}

		switch (command.SafeRemainingArgument.ToLowerInvariant())
		{
			case "cdg":
			case "coup":
			case "coup de grace":
			case "coupdegrace":
			case "weapon":
				Method = ExecutionPatrolExecutionMethod.CoupDeGraceWithWeapon;
				break;
			case "drug":
			case "administer drug":
			case "administerdrug":
				Method = ExecutionPatrolExecutionMethod.AdministerDrug;
				break;
			case "firing":
			case "firing squad":
			case "firingsquad":
			case "squad":
				Method = ExecutionPatrolExecutionMethod.FiringSquad;
				break;
			default:
				actor.OutputHandler.Send("That is not a valid execution method. Use cdg, drug or firing.");
				return false;
		}

		actor.OutputHandler.Send($"This execution patrol will now use the {Method.DescribeEnum().ColourValue()} method.");
		return true;
	}

	private bool BuildingCommandEquipment(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which room should this patrol use to retrieve execution equipment? Use here, a room id, or none.");
			return false;
		}

		if (command.PeekSpeech().EqualTo("none") || command.PeekSpeech().EqualTo("clear"))
		{
			EquipmentLocationId = 0;
			actor.OutputHandler.Send("This execution patrol will use the legal authority preparation room for equipment.");
			return true;
		}

		if (command.PeekSpeech().EqualTo("here"))
		{
			EquipmentLocationId = actor.Location.Id;
			actor.OutputHandler.Send($"This execution patrol will retrieve equipment from {actor.Location.HowSeen(actor)}.");
			return true;
		}

		if (!long.TryParse(command.SafeRemainingArgument, out long value))
		{
			actor.OutputHandler.Send("You must specify a room id, here or none.");
			return false;
		}

		ICell location = Gameworld.Cells.Get(value);
		if (location is null)
		{
			actor.OutputHandler.Send("There is no such room.");
			return false;
		}

		EquipmentLocationId = location.Id;
		actor.OutputHandler.Send($"This execution patrol will retrieve equipment from {location.HowSeen(actor)}.");
		return true;
	}

	private bool BuildingCommandDrug(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which drug should this patrol administer?");
			return false;
		}

		IDrug drug = long.TryParse(command.SafeRemainingArgument, out long value)
			? Gameworld.Drugs.Get(value)
			: Gameworld.Drugs.GetByIdOrName(command.SafeRemainingArgument);
		if (drug is null)
		{
			actor.OutputHandler.Send("There is no such drug.");
			return false;
		}

		DrugId = drug.Id;
		actor.OutputHandler.Send($"This execution patrol will administer {drug.Name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandDose(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, out double value) || value <= 0.0)
		{
			actor.OutputHandler.Send("How many grams of the configured drug should be administered per attempt?");
			return false;
		}

		DrugGrams = value;
		actor.OutputHandler.Send($"This execution patrol will administer {DrugGrams.ToString("N3", actor).ColourValue()} grams per attempt.");
		return true;
	}

	private bool BuildingCommandVector(ICharacter actor, StringStack command)
	{
		if (command.IsFinished ||
		    !command.SafeRemainingArgument.TryParseEnum(out DrugVector vector) ||
		    vector == DrugVector.None)
		{
			actor.OutputHandler.Send($"You must specify one of {Enum.GetValues(typeof(DrugVector)).OfType<DrugVector>().Where(x => x != DrugVector.None).Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		DrugVector = vector;
		actor.OutputHandler.Send($"This execution patrol will administer drugs via the {DrugVector.DescribeEnum().ColourValue()} vector.");
		return true;
	}

	private bool BuildingCommandComplianceWindow(ICharacter actor, StringStack command)
	{
		return BuildingCommandPositiveSeconds(actor, command, "compliance window", value => ComplianceWindowSeconds = value);
	}

	private bool BuildingCommandLastWordsDelay(ICharacter actor, StringStack command)
	{
		return BuildingCommandPositiveSeconds(actor, command, "last words delay", value => LastWordsSeconds = value);
	}

	private bool BuildingCommandScriptDelay(ICharacter actor, StringStack command)
	{
		return BuildingCommandPositiveSeconds(actor, command, "script delay", value => ScriptDelaySeconds = value);
	}

	private bool BuildingCommandDeathConfirmationDelay(ICharacter actor, StringStack command)
	{
		return BuildingCommandPositiveSeconds(actor, command, "death confirmation delay", value => DeathConfirmationSeconds = value);
	}

	private static bool BuildingCommandPositiveSeconds(ICharacter actor, StringStack command, string name, Action<int> setter)
	{
		if (command.IsFinished || !int.TryParse(command.SafeRemainingArgument, out int value) || value < 1)
		{
			actor.OutputHandler.Send($"How many seconds should the {name} be?");
			return false;
		}

		setter(value);
		actor.OutputHandler.Send($"The {name} is now {value.ToString("N0", actor).ColourValue()} seconds.");
		return true;
	}

	private bool BuildingCommandAttempts(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.SafeRemainingArgument, out int value) || value < 1)
		{
			actor.OutputHandler.Send("How many execution attempts should the patrol make before aborting?");
			return false;
		}

		MaximumExecutionAttempts = value;
		actor.OutputHandler.Send($"This execution patrol will make up to {value.ToString("N0", actor).ColourValue()} execution attempts.");
		return true;
	}

	private bool BuildingCommandEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which emote do you want to set: retrieve, resist, arrival, restrain, lastwords, drug, firing or complete?");
			return false;
		}

		string which = command.PopSpeech().ToLowerInvariant();
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What emote text do you want to use?");
			return false;
		}

		string emote = command.SafeRemainingArgument;
		if (!ValidateEmote(actor, emote))
		{
			return false;
		}

		switch (which)
		{
			case "retrieve":
			case "retrieval":
				RetrieveEmote = emote;
				break;
			case "resist":
			case "resistance":
				ResistEmote = emote;
				break;
			case "arrival":
			case "arrive":
				ArrivalEmote = emote;
				break;
			case "restrain":
			case "restraint":
				RestraintEmote = emote;
				break;
			case "lastwords":
			case "lastword":
				LastWordsEmote = emote;
				break;
			case "drug":
				DrugEmote = emote;
				break;
			case "firing":
			case "fire":
				FiringSquadEmote = emote;
				break;
			case "complete":
			case "completion":
				CompletionEmote = emote;
				break;
			default:
				actor.OutputHandler.Send("That is not a valid execution patrol emote.");
				return false;
		}

		actor.OutputHandler.Send($"The {which.ColourName()} execution patrol emote is now: {emote.ColourCommand()}");
		return true;
	}

	private bool BuildingCommandScript(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "add":
				if (command.IsFinished)
				{
					actor.OutputHandler.Send("What emote do you want to add as a script step?");
					return false;
				}

				string emote = command.SafeRemainingArgument;
				if (!ValidateEmote(actor, emote))
				{
					return false;
				}

				_executionScript.Add(emote);
				actor.OutputHandler.Send($"You add a new execution script step at position {_executionScript.Count.ToString("N0", actor).ColourValue()}.");
				return true;
			case "delete":
			case "del":
			case "remove":
			case "rem":
				if (command.IsFinished || !int.TryParse(command.SafeRemainingArgument, out int value) ||
				    value < 1 || value > _executionScript.Count)
				{
					actor.OutputHandler.Send($"Which script step do you want to delete? Pick a number between {"1".ColourValue()} and {_executionScript.Count.ToString("N0", actor).ColourValue()}.");
					return false;
				}

				_executionScript.RemoveAt(value - 1);
				actor.OutputHandler.Send($"You delete execution script step {value.ToString("N0", actor).ColourValue()}.");
				return true;
			case "swap":
				if (command.IsFinished || !int.TryParse(command.PopSpeech(), out int first) ||
				    command.IsFinished || !int.TryParse(command.SafeRemainingArgument, out int second) ||
				    first < 1 || second < 1 || first > _executionScript.Count || second > _executionScript.Count)
				{
					actor.OutputHandler.Send("Which two script step numbers do you want to swap?");
					return false;
				}

				(_executionScript[first - 1], _executionScript[second - 1]) = (_executionScript[second - 1], _executionScript[first - 1]);
				actor.OutputHandler.Send($"You swap execution script steps {first.ToString("N0", actor).ColourValue()} and {second.ToString("N0", actor).ColourValue()}.");
				return true;
			case "clear":
				_executionScript.Clear();
				actor.OutputHandler.Send("You clear all execution script steps.");
				return true;
			default:
				actor.OutputHandler.Send("Use script add, script delete, script swap or script clear.");
				return false;
		}
	}

	private static bool ValidateEmote(ICharacter actor, string emote)
	{
		Emote test = new(emote, actor, actor, actor);
		if (test.Valid)
		{
			return true;
		}

		actor.OutputHandler.Send(test.ErrorMessage);
		return false;
	}

	public string ShowConfiguration(ICharacter actor, IPatrolRoute patrol)
	{
		StringBuilder sb = new();
		ICell equipment = GetEquipmentLocation(patrol);
		ICell execution = GetExecutionLocation(patrol);
		IDrug drug = Gameworld.Drugs.Get(DrugId);
		sb.AppendLine("Execution Patrol Configuration".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine($"Execution Location: {execution?.GetFriendlyReference(actor) ?? "None".ColourError()}");
		sb.AppendLine($"Equipment Location: {equipment?.GetFriendlyReference(actor) ?? "None".ColourError()}");
		sb.AppendLine($"Method: {Method.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Drug: {drug?.Name.ColourName() ?? "None".ColourError()}");
		sb.AppendLine($"Drug Dose: {DrugGrams.ToString("N3", actor).ColourValue()} grams via {DrugVector.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Compliance Window: {ComplianceWindowSeconds.ToString("N0", actor).ColourValue()} seconds");
		sb.AppendLine($"Last Words Delay: {LastWordsSeconds.ToString("N0", actor).ColourValue()} seconds");
		sb.AppendLine($"Script Delay: {ScriptDelaySeconds.ToString("N0", actor).ColourValue()} seconds");
		sb.AppendLine($"Death Confirmation Delay: {DeathConfirmationSeconds.ToString("N0", actor).ColourValue()} seconds");
		sb.AppendLine($"Maximum Attempts: {MaximumExecutionAttempts.ToString("N0", actor).ColourValue()}");
		sb.AppendLine("Script Steps:");
		if (_executionScript.Count == 0)
		{
			sb.AppendLine("\tNone".ColourError());
		}
		else
		{
			for (int i = 0; i < _executionScript.Count; i++)
			{
				sb.AppendLine($"\t{(i + 1).ToString("N0", actor)}) {_executionScript[i].ColourCommand()}");
			}
		}

		return sb.ToString();
	}

	public string SaveStrategyData()
	{
		return new XElement("ExecutionPatrol",
			new XAttribute("method", (int)Method),
			new XAttribute("equipment", EquipmentLocationId),
			new XAttribute("drug", DrugId),
			new XAttribute("druggrams", DrugGrams.ToString(CultureInfo.InvariantCulture)),
			new XAttribute("drugvector", (int)DrugVector),
			new XAttribute("compliance", ComplianceWindowSeconds),
			new XAttribute("lastwords", LastWordsSeconds),
			new XAttribute("scriptdelay", ScriptDelaySeconds),
			new XAttribute("confirmdelay", DeathConfirmationSeconds),
			new XAttribute("attempts", MaximumExecutionAttempts),
			new XElement("Emotes",
				new XElement("Retrieve", new XCData(RetrieveEmote)),
				new XElement("Resist", new XCData(ResistEmote)),
				new XElement("Arrival", new XCData(ArrivalEmote)),
				new XElement("Restraint", new XCData(RestraintEmote)),
				new XElement("LastWords", new XCData(LastWordsEmote)),
				new XElement("Drug", new XCData(DrugEmote)),
				new XElement("FiringSquad", new XCData(FiringSquadEmote)),
				new XElement("Completion", new XCData(CompletionEmote))
			),
			new XElement("Script", _executionScript.Select(x => new XElement("Step", new XCData(x))))
		).ToString();
	}

	private void LoadStrategyData(string strategyData)
	{
		if (string.IsNullOrWhiteSpace(strategyData))
		{
			return;
		}

		XElement root;
		try
		{
			root = XElement.Parse(strategyData);
		}
		catch
		{
			return;
		}

		if (int.TryParse(root.Attribute("method")?.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int method) &&
		    Enum.IsDefined(typeof(ExecutionPatrolExecutionMethod), method))
		{
			Method = (ExecutionPatrolExecutionMethod)method;
		}

		if (long.TryParse(root.Attribute("equipment")?.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out long equipment))
		{
			EquipmentLocationId = equipment;
		}

		if (long.TryParse(root.Attribute("drug")?.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out long drug))
		{
			DrugId = drug;
		}

		if (double.TryParse(root.Attribute("druggrams")?.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out double grams) &&
		    grams > 0.0)
		{
			DrugGrams = grams;
		}

		if (int.TryParse(root.Attribute("drugvector")?.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int vector) &&
		    Enum.IsDefined(typeof(DrugVector), vector) &&
		    (DrugVector)vector != DrugVector.None)
		{
			DrugVector = (DrugVector)vector;
		}

		if (int.TryParse(root.Attribute("compliance")?.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int compliance) &&
		    compliance > 0)
		{
			ComplianceWindowSeconds = compliance;
		}

		if (int.TryParse(root.Attribute("lastwords")?.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int lastWords) &&
		    lastWords > 0)
		{
			LastWordsSeconds = lastWords;
		}

		if (int.TryParse(root.Attribute("scriptdelay")?.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int scriptDelay) &&
		    scriptDelay > 0)
		{
			ScriptDelaySeconds = scriptDelay;
		}

		if (int.TryParse(root.Attribute("confirmdelay")?.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int confirmDelay) &&
		    confirmDelay > 0)
		{
			DeathConfirmationSeconds = confirmDelay;
		}

		if (int.TryParse(root.Attribute("attempts")?.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int attempts) &&
		    attempts > 0)
		{
			MaximumExecutionAttempts = attempts;
		}

		XElement emotes = root.Element("Emotes");
		if (emotes is not null)
		{
			RetrieveEmote = emotes.Element("Retrieve")?.Value ?? RetrieveEmote;
			ResistEmote = emotes.Element("Resist")?.Value ?? ResistEmote;
			ArrivalEmote = emotes.Element("Arrival")?.Value ?? ArrivalEmote;
			RestraintEmote = emotes.Element("Restraint")?.Value ?? RestraintEmote;
			LastWordsEmote = emotes.Element("LastWords")?.Value ?? LastWordsEmote;
			DrugEmote = emotes.Element("Drug")?.Value ?? DrugEmote;
			FiringSquadEmote = emotes.Element("FiringSquad")?.Value ?? FiringSquadEmote;
			CompletionEmote = emotes.Element("Completion")?.Value ?? CompletionEmote;
		}

		XElement script = root.Element("Script");
		if (script is not null)
		{
			_executionScript.Clear();
			_executionScript.AddRange(script.Elements("Step").Select(x => x.Value));
		}
	}
}

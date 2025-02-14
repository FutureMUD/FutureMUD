using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MailKit;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.PartProtos;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.Health;
using MudSharp.Health.Infections;
using MudSharp.Health.Surgery;
using MudSharp.Health.Wounds;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using Org.BouncyCastle.Asn1.X509.Qualified;

namespace MudSharp.Commands.Modules;

internal class HealthModule : Module<ICharacter>
{
	private HealthModule()
		: base("Health")
	{
		IsNecessary = true;
	}

	public static HealthModule Instance { get; } = new();

	[PlayerCommand("CPR", "cpr")]
	[NoHideCommand]
	[NoMovementCommand]
	[NoMeleeCombatCommand]
	[RequiredCharacterState(CharacterState.Able)]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	protected static void CPR(ICharacter actor, string command)
	{
		if (!actor.Gameworld.GetStaticBool("CPRAllowed"))
		{
			actor.Send(actor.Gameworld.GetStaticString("FailedToFindCommand"));
			return;
		}

		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.Send("Who do you want to perform CPR on?");
			return;
		}

		var target = actor.TargetActor(ss.Pop());
		if (target == null)
		{
			actor.Send("You do not see that person to perform CPR on.");
			return;
		}

		if (target == actor)
		{
			actor.Send("How do you propose to perform CPR on yourself?");
			return;
		}

		if (CharacterState.Able.HasFlag(target.State) || (target.IsBreathing && target.NeedsToBreathe))
		{
			actor.OutputHandler.Send(new EmoteOutput(
				new Emote("$1 $1|do|does not require CPR because #1 %1|are|is responsive.", actor, actor, target)));
			return;
		}

		if (target.CombinedEffectsOfType<CPRTarget>().Any())
		{
			actor.Send($"{target.HowSeen(actor, true)} already has someone performing CPR on them.");
			return;
		}

		if (target.IsEngagedInMelee)
		{
			actor.OutputHandler.Send($"{target.HowSeen(actor, true)} is in melee combat.");
			return;
		}

		actor.AddEffect(new PerformingCPR(actor, target), TimeSpan.FromSeconds(5));
		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ begin|begins to perform CPR on $1.", actor, actor,
			target)));
	}

	[PlayerCommand("Defibrillate", "defibrillate")]
	[NoHideCommand]
	[NoMeleeCombatCommand]
	[RequiredCharacterState(CharacterState.Able)]
	[NoMovementCommand]
	protected static void Defibrillate(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.Send("Who do you want to defibrillate?");
			return;
		}

		var target = actor.TargetActor(ss.Pop());
		if (target == null)
		{
			actor.Send("You do not see that person to defibrillate.");
			return;
		}

		var defib = actor.Body.HeldOrWieldedItems.SelectNotNull(x => x.GetItemType<IDefibrillator>()).FirstOrDefault();
		if (defib == null)
		{
			actor.Send($"You do not have any device capable of defibrillating {target.HowSeen(actor)}.");
			return;
		}

		defib.Shock(actor, target.Body);
	}

	[PlayerCommand("Vitals", "vitals")]
	[NoHideCommand]
	[NoMovementCommand]
	[NoMeleeCombatCommand]
	[RequiredCharacterState(CharacterState.Able)]
	protected static void Vitals(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		var target = ss.IsFinished ? actor : actor.TargetActor(ss.Pop());

		if (target == null)
		{
			actor.Send("You don't see anyone like that to check the vitals for.");
			return;
		}

		void Action(string text)
		{
			actor.OutputHandler.Handle(
				new EmoteOutput(new Emote($"@ check|checks {(actor == target ? "&0's own" : "$0's")} vitals.", actor,
					target)));
			var breathingDescription = target.IsBreathing ? "breathing" : "not breathing";
			var bloodRatio = target.Body.CurrentBloodVolumeLitres / target.Body.TotalBloodVolumeLitres;
			var heartFunction =
				target.Body.Organs.OfType<HeartProto>()
				      .Select(x => x.OrganFunctionFactor(target.Body))
				      .DefaultIfEmpty(0)
				      .Sum();
			string pulseDescription;
			if (heartFunction <= 0)
			{
				pulseDescription = "no pulse";
			}
			else if (heartFunction <= 0.1)
			{
				pulseDescription = "a rapid, erratic pulse";
			}
			else
			{
				if (bloodRatio < 0.6)
				{
					pulseDescription = "a very weak pulse";
				}
				else if (bloodRatio < 0.8)
				{
					pulseDescription = "a weak pulse";
				}
				else if (heartFunction < 0.75)
				{
					pulseDescription = "a rapid, full pulse";
				}
				else
				{
					switch (target.LongtermExertion)
					{
						case ExertionLevel.Heavy:
							pulseDescription = "a rapid, easily palpable pulse";
							break;
						case ExertionLevel.VeryHeavy:
							pulseDescription = "a rapid, full pulse";
							break;
						case ExertionLevel.ExtremelyHeavy:
							pulseDescription = "a very rapid, full pulse";
							break;
						default:
							pulseDescription = "an ordinary, easily palpable pulse";
							break;
					}
				}
			}

			actor.OutputHandler.Send(
				new EmoteOutput(new Emote($"@ have|has {pulseDescription} and are|is {breathingDescription}.",
					target, target)));
		}

		if (target != actor && CharacterState.Able.HasFlag(target.State))
		{
			actor.OutputHandler.Handle(
				new EmoteOutput(new Emote("@ propose|proposes to check $1's vitals.", actor, actor, target)));
			target.Send(
				$"You must type {"accept".Colour(Telnet.Yellow)} to permit them to do so, or {"decline".Colour(Telnet.Yellow)} to decline.");
			target.AddEffect(
				new Accept(target,
					new GenericProposal(Action,
						text =>
						{
							target.OutputHandler.Handle(
								new EmoteOutput(new Emote("@ decline|declines to allow $1 to check &0's vitals.",
									target, target, actor)));
						},
						() =>
						{
							target.OutputHandler.Handle(
								new EmoteOutput(new Emote("@ decline|declines to allow $1 to check &0's vitals.",
									target, target, actor)));
						},
						"proposing to check vitals",
						"vitals"
					)), TimeSpan.FromSeconds(30));
		}
		else
		{
			Action(string.Empty);
		}
	}

	[PlayerCommand("Relocate", "relocate")]
	[NoHideCommand]
	[NoMovementCommand]
	[NoMeleeCombatCommand]
	[RequiredCharacterState(CharacterState.Able)]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[HelpInfo("relocate",
		"This grizzly command allows you to relocate one of your own or another's bones or joints. The syntax to use this command is: relocate <target> <bodypart>.",
		AutoHelp.HelpArgOrNoArg)]
	protected static void Relocate(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		var target = actor.TargetActor(ss.PopSpeech());
		if (target == null)
		{
			actor.Send("You don't see anyone like that.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(new EmoteOutput(new Emote("Which bodypart of &0's do you want to relocate?",
				target, target)));
			return;
		}

		var targetPart = target.Body.GetTargetPart(ss.PopSpeech());
		if (targetPart == null)
		{
			actor.OutputHandler.Send(new EmoteOutput(new Emote("$0 $0|do|does not have any such bodypart.", actor,
				target)));
			return;
		}

		if (target.Movement != null)
		{
			actor.Send("There is no way to relocate somebody's bones while they are moving.");
			return;
		}

		if (target.Combat != null && target.MeleeRange)
		{
			actor.Send("You cannot relocate somebody's bones while they are still in melee combat.");
			return;
		}

		// TODO - dislocation
		if (targetPart is IBone bone)
		{
			var wounds = target.Wounds.OfType<BoneFracture>().Where(x => x.Bone == bone).ToList();
			if (!wounds.Any())
			{
				actor.OutputHandler.Send(new EmoteOutput(new Emote(
					$"$0's {bone.FullDescription()} does not have any fractures that you can see.", actor, target)));
				return;
			}

			var wound = wounds.FirstOrDefault(x => x.CanBeTreated(TreatmentType.Relocation) != Difficulty.Impossible);
			if (wound == null)
			{
				actor.OutputHandler.Send(new EmoteOutput(new Emote(
					$"$0's {bone.FullDescription()} does not have any fractures that could possibly be relocated:\n{wounds.Select(x => x.WhyCannotBeTreated(TreatmentType.Relocation)).ListToString(separator: "\n", article: "\t", conjunction: "", twoItemJoiner: "\n")}",
					actor, target)));
				return;
			}

			if (target.WillingToPermitMedicalIntervention(actor))
			{
				actor.OutputHandler.Handle(new EmoteOutput(
					new Emote($"@ begin|begins attempting to relocate $0's {bone.FullDescription()}.", actor, target)));
				actor.AddEffect(new RelocatingBone(actor, target, bone), RelocatingBone.EffectDuration);
				return;
			}

			target.AddEffect(new Accept(target, new GenericProposal(text =>
				{
					if (actor.Effects.Any(x => x.IsBlockingEffect("general")))
					{
						actor.OutputHandler.Send(
							$"You must first stop {actor.Effects.First(x => x.IsBlockingEffect("general")).BlockingDescription("general", actor)} before you can relocate any bones.");
						return;
					}

					if (!target.Wounds.Contains(wound))
					{
						actor.OutputHandler.Send("That is no longer a valid wound for you to relocate.");
						return;
					}

					if (target.Movement != null || !target.ColocatedWith(actor))
					{
						actor.OutputHandler.Send("That is no longer a valid wound for you to relocate.");
						return;
					}

					if (target.Combat != null && target.MeleeRange)
					{
						actor.Send("You cannot relocate somebody's bones while they are still in melee combat.");
						return;
					}

					actor.OutputHandler.Handle(new EmoteOutput(
						new Emote($"@ begin|begins attempting to relocate $0's {bone.FullDescription()}.", actor,
							target)));
					actor.AddEffect(new RelocatingBone(actor, target, bone), RelocatingBone.EffectDuration);
					return;
				},
				text =>
				{
					target.OutputHandler.Handle(new EmoteOutput(new Emote(
						$"@ decline|declines $1's offer to relocate &0's {bone.FullDescription()}.", target, target,
						actor)));
				},
				() =>
				{
					target.OutputHandler.Handle(new EmoteOutput(new Emote(
						$"@ decline|declines $1's offer to relocate &0's {bone.FullDescription()}.", target, target,
						actor)));
				}, "relocating a bone", "bone", "relocate", "relocating", "relocation")), TimeSpan.FromSeconds(120));
			actor.OutputHandler.Handle(
				new EmoteOutput(new Emote($"@ are|is proposing to relocate $0's {bone.FullDescription()}.", actor,
					target)));
			target.Send(
				$"Use the command {"accept".Colour(Telnet.Yellow)} to accept or {"decline".Colour(Telnet.Yellow)} to cancel.");
			return;
		}

		actor.Send("That is not a bodypart that can be targeted with the relocate command.");
		return;
	}

	[PlayerCommand("Bind", "bind")]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[RequiredCharacterState(CharacterState.Able)]
	[NoHideCommand]
	[NoMeleeCombatCommand]
	[NoMovementCommand]
	[HelpInfo("bind", "Bind bleeding wounds to stop the bleeding. Syntax: bind [<target>]", AutoHelp.HelpArg)]
	protected static void Bind(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		ICharacter target = null;
		target = ss.IsFinished ? actor : actor.TargetActor(ss.Pop());

		if (target == null)
		{
			actor.Send("You do not see anyone like that whose wounds you can bind.");
			return;
		}

		if (target.Movement != null)
		{
			actor.Send("There is no way to bind somebody's wounds while they are moving.");
			return;
		}

		if (target.Combat != null && target.MeleeRange)
		{
			actor.Send("You cannot bind someone's wounds while they are still in melee combat.");
			return;
		}

		if (target.VisibleWounds(actor, WoundExaminationType.Examination)
		          .All(x => x.BleedStatus != BleedStatus.Bleeding))
		{
			actor.OutputHandler.Send(
				new EmoteOutput(new Emote("$0 $0|are|is not bleeding, to the best of your knowledge.", actor, target)));
			return;
		}

		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ begin|begins tending to $0's bleeding wounds.", actor,
			target)));
		actor.AddEffect(new Binding(actor, target), Binding.EffectDuration);
	}

	[PlayerCommand("CleanWounds", "cleanwounds")]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[RequiredCharacterState(CharacterState.Able)]
	[NoHideCommand]
	[NoMeleeCombatCommand]
	[NoMovementCommand]
	[HelpInfo("cleanwounds", "Clean wounds to reduce the chance for infection. Syntax: cleanwounds [<target>]",
		AutoHelp.HelpArg)]
	protected static void CleanWounds(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		ICharacter target = null;
		target = ss.IsFinished ? actor : actor.TargetActor(ss.Pop());

		if (target == null)
		{
			actor.Send("You do not see anyone like that whose wounds you can clean.");
			return;
		}

		if (target.Movement != null)
		{
			actor.Send("There is no way to clean somebody's wounds while they are moving.");
			return;
		}

		if (target.Combat != null && target.MeleeRange)
		{
			actor.Send("You cannot clean someone's wounds while they are still in melee combat.");
			return;
		}

		var (canClean, reason) = CleaningWounds.PeekCanClean(actor, target);
		if (canClean)
		{
			if (actor != target && !target.WillingToPermitMedicalIntervention(actor))
			{
				target.AddEffect(new Accept(target, new GenericProposal
				{
					AcceptAction = text =>
					{
						if (!actor.ColocatedWith(target))
						{
							target.OutputHandler.Send("They are no longer there.");
							return;
						}

						if (!CharacterState.Able.HasFlag(actor.State))
						{
							target.OutputHandler.Send("They are no longer in a position to assist you.");
							return;
						}

						if (actor.IsEngagedInMelee)
						{
							target.OutputHandler.Send("They are too busy fighting to aide you.");
							return;
						}

						if (actor.Movement != null)
						{
							target.OutputHandler.Send("They are too busy moving to aide you.");
							return;
						}

						if (actor.Effects.Any(x => x.IsBlockingEffect("general")))
						{
							actor.OutputHandler.Send(
								$"You must first stop {actor.Effects.First(x => x.IsBlockingEffect("general")).BlockingDescription("general", actor)} before you can clean any wounds.");
							return;
						}

						if (target.Movement != null)
						{
							actor.Send("There is no way to clean somebody's wounds while they are moving.");
							return;
						}

						if (target.Combat != null && target.MeleeRange)
						{
							actor.Send("You cannot clean someone's wounds while they are still in melee combat.");
							return;
						}

						var (canStillClean, stillReason) = CleaningWounds.PeekCanClean(actor, target);
						if (canStillClean)
						{
							actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ begin|begins cleaning $0's wounds.",
								actor, target)));
							actor.AddEffect(new CleaningWounds(actor, target), CleaningWounds.EffectDuration);
							return;
						}

						if (stillReason == CleaningWounds.PeekCanCleanReason.AntisepticWoundsNoTreatment)
						{
							actor.Send(
								$"You need antiseptics to clean any of {target.HowSeen(actor, type: DescriptionType.Possessive)} wounds any further.");
							return;
						}

						actor.Send(
							$"You can't see any wounds of {target.HowSeen(actor, type: DescriptionType.Possessive)} that would benefit from further cleaning.");
					},
					ExpireAction = () =>
					{
						target.OutputHandler.Handle(new EmoteOutput(
							new Emote("@ decline|declines $0's offer to clean &0's wounds.", target, actor)));
					},
					RejectAction = text =>
					{
						target.OutputHandler.Handle(new EmoteOutput(
							new Emote("@ decline|declines $0's offer to clean &0's wounds.", target, actor)));
					},
					DescriptionString = "an offer to clean wounds"
				}), TimeSpan.FromSeconds(120));
				actor.OutputHandler.Handle(
					new EmoteOutput(new Emote("@ are|is proposing to clean $0's wounds.", actor, target)));
				target.Send(
					$"Use the command {"accept".Colour(Telnet.Yellow)} to accept or {"decline".Colour(Telnet.Yellow)} to cancel.");
				return;
			}

			actor.OutputHandler.Handle(
				new EmoteOutput(new Emote("@ begin|begins cleaning $0's wounds.", actor, target)));
			actor.AddEffect(new CleaningWounds(actor, target), CleaningWounds.EffectDuration);
			return;
		}

		if (reason == CleaningWounds.PeekCanCleanReason.AntisepticWoundsNoTreatment)
		{
			actor.Send(
				$"You need antiseptics to clean any of {target.HowSeen(actor, type: DescriptionType.Possessive)} wounds any further.");
			return;
		}

		actor.Send(
			$"You can't see any wounds of {target.HowSeen(actor, type: DescriptionType.Possessive)} that would benefit from further cleaning.");
	}

	[PlayerCommand("Suture", "suture")]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[RequiredCharacterState(CharacterState.Able)]
	[NoHideCommand]
	[NoMovementCommand]
	[NoMeleeCombatCommand]
	[HelpInfo("suture", "Stitch up a wound to reduce its chance of reopening. Syntax: suture [<target>]",
		AutoHelp.HelpArg)]
	protected static void Suture(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		ICharacter target = null;
		target = ss.IsFinished ? actor : actor.TargetActor(ss.Pop());

		if (target == null)
		{
			actor.Send("You do not see anyone like that whose wounds you can suture.");
			return;
		}

		if (target.Movement != null)
		{
			actor.Send("There is no way to suture somebody's wounds while they are moving.");
			return;
		}

		if (target.Combat != null && target.MeleeRange)
		{
			actor.Send("You cannot suture someone's wounds while they are still in melee combat.");
			return;
		}

		if (target.VisibleWounds(actor, WoundExaminationType.Examination)
		          .All(x => x.CanBeTreated(TreatmentType.Close) == Difficulty.Impossible))
		{
			actor.OutputHandler.Send(new EmoteOutput(new Emote("$0 $0|have|has no wounds that can be sutured by you.",
				actor, target)));
			return;
		}

		var inventoryPlan = actor.Gameworld.SutureInventoryPlanTemplate.CreatePlan(actor);
		switch (inventoryPlan.PlanIsFeasible())
		{
			case InventoryPlanFeasibility.NotFeasibleNotEnoughHands:
				actor.Send("You can't suture because you are unable to pick up the suturing equipment.");
				return;
			case InventoryPlanFeasibility.NotFeasibleNotEnoughWielders:
				actor.Send("You can't suture because you are unable to pick up the suturing equipment.");
				return;
			case InventoryPlanFeasibility.NotFeasibleMissingItems:
				actor.Send("You don't have the necessary equipment to suture wounds.");
				return;
		}

		if (actor != target && !target.WillingToPermitMedicalIntervention(actor))
		{
			target.AddEffect(new Accept(target, new GenericProposal
			{
				AcceptAction = text =>
				{
					if (!actor.ColocatedWith(target))
					{
						target.OutputHandler.Send("They are no longer there.");
						return;
					}

					if (!CharacterState.Able.HasFlag(actor.State))
					{
						target.OutputHandler.Send("They are no longer in a position to assist you.");
						return;
					}

					if (actor.IsEngagedInMelee)
					{
						target.OutputHandler.Send("They are too busy fighting to aide you.");
						return;
					}

					if (actor.Movement != null)
					{
						target.OutputHandler.Send("They are too busy moving to aide you.");
						return;
					}

					if (actor.Effects.Any(x => x.IsBlockingEffect("general")))
					{
						actor.OutputHandler.Send(
							$"You must first stop {actor.Effects.First(x => x.IsBlockingEffect("general")).BlockingDescription("general", actor)} before you can suture any wounds.");
						return;
					}

					if (target.Movement != null)
					{
						actor.Send("There is no way to suture somebody's wounds while they are moving.");
						return;
					}

					if (target.Combat != null && target.MeleeRange)
					{
						actor.Send("You cannot suture someone's wounds while they are still in melee combat.");
						return;
					}

					if (target.VisibleWounds(actor, WoundExaminationType.Examination).All(x =>
						    x.CanBeTreated(TreatmentType.Close) == Difficulty.Impossible))
					{
						actor.OutputHandler.Send(new EmoteOutput(
							new Emote("$0 $0|have|has no wounds that can be sutured by you.", actor, target)));
						return;
					}

					switch (inventoryPlan.PlanIsFeasible())
					{
						case InventoryPlanFeasibility.NotFeasibleNotEnoughHands:
							actor.Send("You can't suture because you are unable to pick up the suturing equipment.");
							return;
						case InventoryPlanFeasibility.NotFeasibleNotEnoughWielders:
							actor.Send("You can't suture because you are unable to pick up the suturing equipment.");
							return;
						case InventoryPlanFeasibility.NotFeasibleMissingItems:
							actor.Send("You don't have the necessary equipment to suture wounds.");
							return;
					}

					actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ begin|begins suturing $0's wounds.", actor,
						target)));
					actor.AddEffect(new Suturing(actor, target), Suturing.EffectDuration);
				},
				ExpireAction = () =>
				{
					target.OutputHandler.Handle(new EmoteOutput(
						new Emote("@ decline|declines $0's offer to clean &0's wounds.", target, actor)));
				},
				RejectAction = text =>
				{
					target.OutputHandler.Handle(new EmoteOutput(
						new Emote("@ decline|declines $0's offer to clean &0's wounds.", target, actor)));
				},
				DescriptionString = "an offer to clean wounds"
			}), TimeSpan.FromSeconds(120));
			actor.OutputHandler.Handle(
				new EmoteOutput(new Emote("@ are|is proposing to suture $0's wounds.", actor, target)));
			target.Send(
				$"Use the command {"accept".Colour(Telnet.Yellow)} to accept or {"decline".Colour(Telnet.Yellow)} to cancel.");
			return;
		}

		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ begin|begins suturing $0's wounds.", actor, target)));
		actor.AddEffect(new Suturing(actor, target), Suturing.EffectDuration);
	}

	[PlayerCommand("Tend", "tend")]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[RequiredCharacterState(CharacterState.Able)]
	[NoHideCommand]
	[NoCombatCommand]
	[NoMovementCommand]
	[HelpInfo("tend", "Tend to a wound so that it heals a little faster. Syntax: tend [<target>]", AutoHelp.HelpArg)]
	protected static void Tend(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		ICharacter target = null;
		target = ss.IsFinished ? actor : actor.TargetActor(ss.Pop());

		if (target == null)
		{
			actor.Send("You do not see anyone like that whose wounds you can tend to.");
			return;
		}

		if (target.Movement != null)
		{
			actor.Send("There is no way to tend to somebody's wounds while they are moving.");
			return;
		}

		if (target.Combat != null && target.MeleeRange)
		{
			actor.Send("You cannot tend to someone's wounds while they are still in melee combat.");
			return;
		}

		if (target.VisibleWounds(actor, WoundExaminationType.Examination)
		          .All(x => x.CanBeTreated(TreatmentType.Tend) == Difficulty.Impossible))
		{
			actor.OutputHandler.Send(new EmoteOutput(new Emote("$0 $0|have|has no wounds that can be tended to by you.",
				actor, target)));
			return;
		}

		var inventoryPlan = actor.Gameworld.TendInventoryPlanTemplate.CreatePlan(actor);
		switch (inventoryPlan.PlanIsFeasible())
		{
			case InventoryPlanFeasibility.NotFeasibleNotEnoughHands:
				actor.Send("You can't tend because you are unable to pick up the tending equipment.");
				return;
			case InventoryPlanFeasibility.NotFeasibleNotEnoughWielders:
				actor.Send("You can't tend because you are unable to pick up the tending equipment.");
				return;
			case InventoryPlanFeasibility.NotFeasibleMissingItems:
				actor.Send("You don't have the necessary equipment to tend to wounds.");
				return;
		}

		if (!target.WillingToPermitMedicalIntervention(actor))
		{
			actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ offer|offers to tend to $1's wounds.", actor, actor,
				target)));
			target.OutputHandler.Send(Accept.StandardAcceptPhrasing);

			target.AddEffect(new Accept(target, new GenericProposal
			{
				AcceptAction = text =>
				{
					actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ begin|begins tending to $1=0's wounds.",
						actor, actor, target)));
					actor.AddEffect(new TendingWounds(actor, target), TendingWounds.EffectDuration);
				},
				RejectAction = text =>
				{
					target.OutputHandler.Handle(new EmoteOutput(
						new Emote("@ decline|declines $1's offer to tend to &0's wounds.", target, target, actor)));
				},
				ExpireAction = () =>
				{
					target.OutputHandler.Handle(new EmoteOutput(
						new Emote("@ decline|declines $1's offer to tend to &0's wounds.", target, target, actor)));
				},
				DescriptionString = "An offer to tend to your wounds",
				Keywords = new List<string> { "tend", "wounds" }
			}), TimeSpan.FromSeconds(120));
			return;
		}

		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ begin|begins tending to $1=0's wounds.", actor, actor,
			target)));
		actor.AddEffect(new TendingWounds(actor, target), TendingWounds.EffectDuration);
	}

	[PlayerCommand("Repair", "repair")]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[RequiredCharacterState(CharacterState.Able)]
	[NoHideCommand]
	[NoMeleeCombatCommand]
	[NoMovementCommand]
	[HelpInfo("repair",
		@"This command is used to repair damage to items or mechanical persons. This requires the use of an appropriate repair kit, which must be held. 

The syntaxes available include:

	#3repair <target> with <kit>#0 - repairs all applicable damage with the specified kit, starting with the easiest damage.
	#3repair <target> with <kit> worst#0 - repairs all applicable damage with the specified kit, starting with the hardest damage.
	#3repair <target> why <kit>#0 - shows the error list as to why repairs can't be completed.
	#3repair <target> <bodypart> with <kit>#0 - repairs all wounds on a target bodypart with the kit.",
		AutoHelp.HelpArgOrNoArg)]
	protected static void Repair(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		var target = actor.Target(ss.PopSpeech());
		if (target == null)
		{
			actor.OutputHandler.Send("You don't see anything like that to repair.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must either specify your repair kit with the syntax {"with <kit>".Colour(Telnet.Yellow)} or specify a bodypart and then the same.");
			return;
		}

		IRepairKit kit = null;
		var worstFirst = false;
		IBodypart bodypart = null;

		if (ss.Peek().EqualTo("with"))
		{
			ss.PopSpeech();
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("Which repair kit do you want to use to carry out the repairs?");
				return;
			}

			var kitItem = actor.TargetHeldItem(ss.PopSpeech());
			if (kitItem == null)
			{
				actor.OutputHandler.Send("You aren't holding anything like that.");
				return;
			}

			kit = kitItem.GetItemType<IRepairKit>();
			if (kit == null)
			{
				actor.OutputHandler.Send($"{kitItem.HowSeen(actor, true)} is not a repair kit.");
				return;
			}

			if (!ss.IsFinished && ss.Pop().EqualTo("worst"))
			{
				worstFirst = true;
			}
		}
		else if (ss.Peek().EqualTo("why"))
		{
			ss.PopSpeech();
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send(
					"Which repair kit do you want to use to see why you can or cannot carry out the repairs?");
				return;
			}

			var kitItem = actor.TargetHeldItem(ss.PopSpeech());
			if (kitItem == null)
			{
				actor.OutputHandler.Send("You aren't holding anything like that.");
				return;
			}

			kit = kitItem.GetItemType<IRepairKit>();
			if (kit == null)
			{
				actor.OutputHandler.Send($"{kitItem.HowSeen(actor, true)} is not a repair kit.");
				return;
			}

			var sb = new StringBuilder();
			var wounds =
				(target is ICharacter ch
					? ch.Body.VisibleWounds(actor,
						actor == target ? WoundExaminationType.Self : WoundExaminationType.Examination)
					: ((IGameItem)target).Wounds).Where(x => x.Repairable).ToList();
			if (!wounds.Any())
			{
				actor.OutputHandler.Send(new EmoteOutput(new Emote("$0 have|has no damage.", actor, target)));
				return;
			}

			foreach (var wound in wounds)
			{
				var (success, message) = kit.CanRepair(wound);
				if (success)
				{
					sb.AppendLine(
						$"{wound.Describe(WoundExaminationType.Examination, Outcome.MajorPass)}: Can be treated [{wound.CanBeTreated(TreatmentType.Repair).Describe().Colour(Telnet.Cyan)}]");
				}
				else
				{
					sb.AppendLine($"{wound.Describe(WoundExaminationType.Examination, Outcome.MajorPass)}: {message}");
				}
			}

			actor.OutputHandler.Send(sb.ToString());
			return;
		}
		else
		{
			if (target is not ICharacter ch)
			{
				actor.OutputHandler.Send("You can only specify target bodyparts for character targets.");
				return;
			}

			bodypart = ch.Body.GetTargetBodypart(ss.PopSpeech());
			if (bodypart == null)
			{
				actor.OutputHandler.Send(new EmoteOutput(new Emote("@ have|has no bodypart like that.", ch)));
				return;
			}

			if (ss.IsFinished || !ss.Peek().EqualTo("with"))
			{
				actor.OutputHandler.Send("Which repair kit do you want to use to carry out the repairs?");
				return;
			}

			ss.PopSpeech();
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("Which repair kit do you want to use to carry out the repairs?");
				return;
			}

			var kitItem = actor.TargetHeldItem(ss.PopSpeech());
			if (kitItem == null)
			{
				actor.OutputHandler.Send("You aren't holding anything like that.");
				return;
			}

			kit = kitItem.GetItemType<IRepairKit>();
			if (kit == null)
			{
				actor.OutputHandler.Send($"{kitItem.HowSeen(actor, true)} is not a repair kit.");
				return;
			}

			if (!ss.IsFinished && ss.Pop().EqualTo("worst"))
			{
				worstFirst = true;
			}
		}

		var tch = target as ICharacter;
		var twounds =
			(tch != null
				? tch.Body.VisibleWounds(actor,
					actor == target ? WoundExaminationType.Self : WoundExaminationType.Examination)
				: ((IGameItem)target).Wounds).Where(x => x.Repairable).ToList();

		if (!twounds.Any())
		{
			actor.OutputHandler.Send(new EmoteOutput(new Emote("$0 have|has no damage.", actor, target)));
			return;
		}

		if (!twounds.Any(x => kit.CanRepair(x).Success))
		{
			actor.OutputHandler.Send(new EmoteOutput(new Emote("You can't repair any of $0's damage with $1.", actor,
				target, kit.Parent)));
			return;
		}

		if (actor != target && tch != null && !tch.WillingToPermitMedicalIntervention(actor))
		{
			tch.AddEffect(new Accept(tch, new GenericProposal
			{
				AcceptAction = text =>
				{
					if (actor.Location != tch.Location)
					{
						tch.OutputHandler.Send("They are no longer there.");
						return;
					}

					if (!CharacterState.Able.HasFlag(actor.State))
					{
						tch.OutputHandler.Send("They are no longer in a position to assist you.");
						return;
					}

					if (actor.IsEngagedInMelee)
					{
						tch.OutputHandler.Send("They are too busy fighting to repair you.");
						return;
					}

					if (actor.Movement != null)
					{
						tch.OutputHandler.Send("They are too busy moving to repair you.");
						return;
					}

					if (!actor.Body.HeldItems.Contains(kit.Parent))
					{
						tch.OutputHandler.Send("They no longer have their repair kit.");
						return;
					}

					if (actor.Effects.Any(x => x.IsBlockingEffect("general")))
					{
						tch.OutputHandler.Send("They are too busy to repair you.");
						return;
					}

					twounds = tch.Body
					             .VisibleWounds(actor,
						             actor == target ? WoundExaminationType.Self : WoundExaminationType.Examination)
					             .Where(x => x.Repairable).ToList();
					if (!twounds.Any())
					{
						tch.OutputHandler.Send("You no longer have any damage.");
						return;
					}

					if (!twounds.Any(x => kit.CanRepair(x).Success))
					{
						tch.OutputHandler.Send(new EmoteOutput(new Emote("You no longer have any repairable damage.",
							actor, target, kit.Parent)));
						return;
					}

					actor.AddEffect(new RepairingWounds(actor, target, kit, worstFirst),
						RepairingWounds.EffectDuration(kit.Echoes.Count()));
					actor.OutputHandler.Send(new EmoteOutput(new Emote("@ begin|begins to repair damage to $1 with $2.",
						actor, actor, target, kit.Parent)));
				},
				ExpireAction = () =>
				{
					tch.OutputHandler.Handle(new EmoteOutput(new Emote("@ decline|declines $0's offer of repair.",
						tch, actor)));
				},
				RejectAction = text =>
				{
					tch.OutputHandler.Handle(new EmoteOutput(new Emote("@ decline|declines $0's offer of repair.",
						tch, actor)));
				},
				DescriptionString = "Accepting an offer of repair"
			}), TimeSpan.FromSeconds(120));
			actor.OutputHandler.Handle(
				new EmoteOutput(new Emote("@ are|is proposing to repair damage to $0.", actor, target)));
			target.Send(
				$"Use the command {"accept".Colour(Telnet.Yellow)} to accept or {"decline".Colour(Telnet.Yellow)} to cancel.");
			return;
		}

		actor.AddEffect(new RepairingWounds(actor, target, kit, worstFirst),
			RepairingWounds.EffectDuration(kit.Echoes.Count()));
		actor.OutputHandler.Send(new EmoteOutput(new Emote("@ begin|begins to repair damage to $1 with $2.", actor,
			actor, target, kit.Parent)));
	}

	[PlayerCommand("Dislodge", "dislodge")]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[RequiredCharacterState(CharacterState.Able)]
	[NoHideCommand]
	[NoCombatCommand]
	[NoMovementCommand]
	protected static void Dislodge(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		var broadTarget = ss.IsFinished ? actor : actor.Target(ss.Pop());

		if (broadTarget == null)
		{
			actor.Send("You do not see anyone or anything like that to dislodge lodged items from.");
			return;
		}

		if (broadTarget is not ICharacter target)
		{
			DislodgeItem(actor, (IGameItem)broadTarget, ss);
			return;
		}

		if (target.Movement != null)
		{
			actor.Send("There is no way to dislodge items from someone's wounds while they are moving.");
			return;
		}

		if (target.Combat != null)
		{
			actor.Send("You cannot dislodge items from someone's wounds while they are still in combat.");
			return;
		}

		var wounds = target.VisibleWounds(actor, WoundExaminationType.Examination).Where(x => x.Lodged != null)
		                   .ToList();
		if (!wounds.Any())
		{
			actor.Send(
				$"{target.HowSeen(actor, true)} {(target == actor ? "do" : "does")} not have any wounds that you can see with anything lodged in them.");
			return;
		}

		var woundsAndLodged = wounds
		                      .Select(x => (Wound: x, Lodged: x.Lodged,
			                      Consequence: x.Lodged.GetItemType<ILodgeConsequence>()))
		                      .Where(x => x.Consequence?.RequiresSurgery != true)
		                      .ToList();

		if (!woundsAndLodged.Any())
		{
			actor.Send(
				$"{target.HowSeen(actor, true)} will require surgical intervention to remove the only lodged items you can see.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send(
				$"Which item do you want to remove from {target.HowSeen(actor, type: DescriptionType.Possessive)} wounds?");
			return;
		}

		var targetLodged = woundsAndLodged.Select(x => x.Lodged).GetFromItemListByKeyword(ss.Pop(), actor);
		if (targetLodged == null)
		{
			actor.Send(
				$"There is no item like that lodged in {target.HowSeen(actor, type: DescriptionType.Possessive)} wounds.");
			return;
		}

		var targetWound = woundsAndLodged.First(x => x.Lodged == targetLodged);

		void RemoveLodged()
		{
			if (targetWound.Wound.Lodged != targetLodged || targetLodged.Deleted)
			{
				actor.Send(
					"You cancel your dislodgement process because the item you were targeting is no longer lodged in that wound.");
				return;
			}

			actor.OutputHandler.Handle(new EmoteOutput(new Emote(
				$"@ finish|finishes removing $2 from {targetWound.Wound.Describe(WoundExaminationType.Look, Outcome.MajorPass)} on $1's {targetWound.Wound.Bodypart.FullDescription()}.",
				actor, actor, target, targetLodged)));
			var check = actor.Gameworld.GetCheck(CheckType.RemoveLodgedObjectCheck);
			var result = check.Check(actor, targetWound.Consequence?.DifficultyToRemove ?? Difficulty.Normal,
				target);
			var damage = targetWound.Consequence?.GetDamageOnRemoval(targetWound.Wound, result);
			if (damage != null)
			{
				targetWound.Wound.CurrentDamage += damage.DamageAmount;
				targetWound.Wound.CurrentPain += damage.PainAmount;
				targetWound.Wound.CurrentStun += damage.StunAmount;
			}

			if (result.Outcome != Outcome.MajorPass && targetWound.Wound.BleedStatus != BleedStatus.NeverBled)
			{
				targetWound.Wound.BleedStatus = BleedStatus.Bleeding;
				target.OutputHandler.Handle(
					new EmoteOutput(new Emote("The wound has begun to bleed again from the trauma.", target)));
			}

			targetWound.Wound.Lodged = null;
			if (actor.Body.CanGet(targetLodged, 0))
			{
				actor.Body.Get(targetLodged, silent: true);
			}
			else
			{
				targetLodged.RoomLayer = actor.RoomLayer;
				actor.Location.Insert(targetLodged);
				actor.Send($"You set {targetLodged.HowSeen(actor)} on the ground as you have nowhere to put it.");
			}

			target.Body.CheckHealthStatus();
		}

		void BeginRemove()
		{
			var effect = new CharacterActionWithTarget(actor, target, perceivable => RemoveLodged(),
				"dislodging an item from a wound", "@ stop|stops dislodging an item from $1's wound",
				"@ cannot move because #0 are|is dislodging an item from $1's wound", new[] { "general", "movement" },
				"dislodging an item from $1's wound");
			actor.OutputHandler.Handle(new EmoteOutput(new Emote(
				$"@ begin|begins to work on removing $2 from {targetWound.Wound.Describe(WoundExaminationType.Look, Outcome.MajorPass)} on $1's {targetWound.Wound.Bodypart.FullDescription()}.",
				actor, actor, target, targetLodged)));
			actor.AddEffect(effect, TimeSpan.FromSeconds(30));
		}

		if (target.WillingToPermitMedicalIntervention(actor))
		{
			BeginRemove();
			return;
		}

		if (target == actor)
		{
			BeginRemove();
		}
		else
		{
			actor.OutputHandler.Handle(new EmoteOutput(new Emote(
				$"@ offer|offers to remove $2 from {targetWound.Wound.Describe(WoundExaminationType.Look, Outcome.MajorPass)} on $1's {targetWound.Wound.Bodypart.FullDescription()}.",
				actor, actor, target, targetLodged)));
			target.OutputHandler.Send(
				$"Use the {"accept".Colour(Telnet.Yellow)} command to accept their help or the {"decline".Colour(Telnet.Yellow)} command to decline it.");
			target.AddEffect(new Accept(target, new GenericProposal
			{
				AcceptAction = text => { BeginRemove(); },
				DescriptionString = "having an item removed from a wound",
				ExpireAction = () =>
				{
					target.OutputHandler.Handle(
						new EmoteOutput(new Emote("@ decline|declines $0's offer to help remove a lodged item.", target,
							actor)));
				},
				Keywords = new List<string> { "dislodge" },
				RejectAction = text =>
				{
					target.OutputHandler.Handle(
						new EmoteOutput(new Emote("@ decline|declines $0's offer to help remove a lodged item.", target,
							actor)));
				}
			}));
		}
	}

	private static void DislodgeItem(ICharacter actor, IGameItem target, StringStack ss)
	{
		var woundsAndLodged = target.Wounds.Where(x => x.Lodged != null).Select(x =>
			                            (Wound: x, Lodged: x.Lodged,
				                            Consequence: x.Lodged?.GetItemType<ILodgeConsequence>()))
		                            .Where(x => x.Consequence?.RequiresSurgery != true)
		                            .ToList();
		if (!woundsAndLodged.Any())
		{
			actor.Send($"{target.HowSeen(actor, true)} has no wounds that have anything lodged in them.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send(
				$"Which item do you want to remove from {target.HowSeen(actor, type: DescriptionType.Possessive)} wounds?");
			return;
		}

		var targetLodged = woundsAndLodged.Select(x => x.Lodged).GetFromItemListByKeyword(ss.Pop(), actor);
		if (targetLodged == null)
		{
			actor.Send(
				$"There is no item like that lodged in {target.HowSeen(actor, type: DescriptionType.Possessive)} wounds.");
			return;
		}

		var targetWound = woundsAndLodged.First(x => x.Lodged == targetLodged);

		void RemoveLodged()
		{
			if (targetWound.Wound.Lodged != targetLodged)
			{
				actor.Send(
					"You cancel your dislodgement process because the item you were targeting is no longer lodged in that wound.");
				return;
			}

			actor.OutputHandler.Handle(new EmoteOutput(new Emote(
				$"@ finish|finishes removing $2 from {targetWound.Wound.Describe(WoundExaminationType.Look, Outcome.MajorPass)} on $1.",
				actor, actor, target, targetLodged)));
			var check = actor.Gameworld.GetCheck(CheckType.RemoveLodgedObjectCheck);
			var result = check.Check(actor, targetWound.Consequence?.DifficultyToRemove ?? Difficulty.Normal,
				target);
			var damage = targetWound.Consequence?.GetDamageOnRemoval(targetWound.Wound, result);
			if (damage != null)
			{
				targetWound.Wound.CurrentDamage += damage.DamageAmount;
				targetWound.Wound.CurrentPain += damage.PainAmount;
				targetWound.Wound.CurrentStun += damage.StunAmount;
			}

			if (result.Outcome != Outcome.MajorPass && targetWound.Wound.BleedStatus != BleedStatus.NeverBled)
			{
				targetWound.Wound.BleedStatus = BleedStatus.Bleeding;
				target.OutputHandler.Handle(
					new EmoteOutput(new Emote("The wound has begun to bleed again from the trauma.", target)));
			}

			targetWound.Wound.Lodged = null;
			if (actor.Body.CanGet(targetLodged, 0))
			{
				actor.Body.Get(targetLodged, silent: true);
			}
			else
			{
				targetLodged.RoomLayer = actor.RoomLayer;
				actor.Location.Insert(targetLodged);
				actor.Send($"You set {targetLodged.HowSeen(actor)} on the ground as you have nowhere to put it.");
			}

			target.CheckHealthStatus();
		}

		void BeginRemove()
		{
			var effect = new CharacterActionWithTarget(actor, target, perceivable => RemoveLodged(),
				"dislodging an item from a wound", "@ stop|stops dislodging an item from $1's wound",
				"@ cannot move because #0 are|is dislodging an item from $1's wound", new[] { "general", "movement" },
				"dislodging an item from $1's wound");
			actor.OutputHandler.Handle(new EmoteOutput(new Emote(
				$"@ begin|begins to work on removing $2 from {targetWound.Wound.Describe(WoundExaminationType.Look, Outcome.MajorPass)} on $1.",
				actor, actor, target, targetLodged)));
			actor.AddEffect(effect, TimeSpan.FromSeconds(30));
		}

		BeginRemove();
	}

	private const string SurgeryPlayerHelp = @"The surgery command allows you to view surgical procedures that you know, information about them, and perform those same surgical procedures.

The syntax is as follows:

	#3surgery list#0 - displays a list of surgical procedures that you know
	#3surgery show <surgery>#0 - shows detailed information about a surgery
	#3surgery perform ""<surgery>"" <target> [<additional arguments>]#0 - performs a surgical procedure on someone";

	private const string SurgeryAdminHelp = @"The surgery command allows you to view and edit surgical procedures. There is also a player version of this command, which you need to be in mortal mode to see or use.

The syntax is as follows:

	#3surgery list#0 - shows all of the surgeries
	#3surgery show <surgery>#0 - shows detailed information abotu a surgery
	#3surgery edit <which>#0 - begins editing a surgery
	#3surgery edit#0 - an alias for #3surgery show#0 on your edited surgery
	#3surgery close#0 - stops editing a surgery
	#3surgery edit new <type> <bodytype> <knowledge> <school> <name> <gerund>#0 - creates a new surgical procedure
	#3surgery set ...#0 - edits the parameters of the surgery";

	[PlayerCommand("Surgery", "surgery")]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[CommandPermission(PermissionLevel.NPC)]
	[RequiredCharacterState(CharacterState.Able)]
	[NoHideCommand]
	[NoCombatCommand]
	[NoMovementCommand]
	[HelpInfo("surgery", SurgeryPlayerHelp, AutoHelp.HelpArgOrNoArg, SurgeryAdminHelp)]
	protected static void Surgery(ICharacter actor, string command)
	{
		var surgeryKnowledges = actor.Gameworld.SurgicalProcedures.Select(x => x.KnowledgeRequired).Distinct().ToList();
		if (!actor.IsAdministrator() && surgeryKnowledges.All(x => !actor.Knowledges.Contains(x)))
		{
			actor.Send("You don't have any knowledge associated with surgery, you wouldn't even know where to begin.");
			return;
		}

		var ss = new StringStack(command.RemoveFirstWord());

		switch (ss.Pop().ToLowerInvariant())
		{
			case "list":
				SurgeryList(actor, ss);
				return;
			case "show":
			case "view":
				SurgeryShow(actor, ss);
				return;
			case "edit":
				if (!actor.IsAdministrator())
				{
					goto default;
				}
				SurgeryEdit(actor, ss);
				return;
			case "close":
				if (!actor.IsAdministrator())
				{
					goto default;
				}

				SurgeryClose(actor, ss);
				return;
			case "set":
				if (!actor.IsAdministrator())
				{
					goto default;
				}

				SurgerySet(actor, ss);
				return;
			case "perform":
				SurgeryPerform(actor, ss);
				return;
			default:
				actor.Send(actor.IsAdministrator() ?
					SurgeryAdminHelp.SubstituteANSIColour() :
					SurgeryPlayerHelp.SubstituteANSIColour());
				return;
		}
	}

	private static void SurgeryEditNew(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which type of surgery do you want to create? The valid options are: {Enum.GetValues<SurgicalProcedureType>().Select(x => x.DescribeEnum().ColourName()).ListToString()}");
			return;
		}

		if (!ss.PopSpeech().TryParseEnum(out SurgicalProcedureType type))
		{
			actor.OutputHandler.Send(
				$"That is not a valid surgery type. The valid options are: {Enum.GetValues<SurgicalProcedureType>().Select(x => x.DescribeEnum().ColourName()).ListToString()}");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which body type should this medical procedure be designed to target?");
			return;
		}

		var body = actor.Gameworld.BodyPrototypes.GetByIdOrName(ss.PopSpeech());
		if (body is null)
		{
			actor.OutputHandler.Send("There is no such body prototype.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What knowledge should be required to know this procedure?");
			return;
		}

		var knowledge = actor.Gameworld.Knowledges.GetByIdOrName(ss.PopSpeech());
		if (knowledge is null)
		{
			actor.OutputHandler.Send("There is no such knowledge.");
			return;
		}

		if (ss.IsFinished)
		{
			var existing = actor.Gameworld.SurgicalProcedures.Select(x => x.MedicalSchool).Distinct().ToList();
			if (existing.Any())
			{
				actor.OutputHandler.Send($"Which medical school should this surgery belong to? There are the following existing medical schools: {existing.Select(x => x.ColourName()).ListToString()}");
			}
			else
			{
				actor.OutputHandler.Send("Which medical school should this surgery belong to?");
			}

			return;
		}

		var school = ss.PopSpeech();
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name should this surgical procedure have?");
			return;
		}

		var name = ss.PopSpeech().TitleCase();
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What gerund (-ing ending word) should this procedure have?");
			return;
		}

		var gerund = ss.PopSpeech();
		var procedure =
			SurgicalProcedureFactory.Instance.CreateProcedureFromBuilderInput(actor.Gameworld, name, gerund, body,
				school, knowledge, type);
		actor.Gameworld.Add(procedure);
		actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<ISurgicalProcedure>>());
		actor.AddEffect(new BuilderEditingEffect<ISurgicalProcedure>(actor) { EditingItem = procedure });
		actor.OutputHandler.Send($"You create a new {type.DescribeEnum().ColourName()} surgical procedure, which you are now editing.");
	}

	private static void SurgerySet(ICharacter actor, StringStack ss)
	{
		var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<ISurgicalProcedure>>().FirstOrDefault();
		if (effect is null)
		{
			actor.OutputHandler.Send("You are not editing any surgical procedures.");
			return;
		}

		effect.EditingItem.BuildingCommand(actor, ss);
	}

	private static void SurgeryClose(ICharacter actor, StringStack ss)
	{
		var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<ISurgicalProcedure>>().FirstOrDefault();
		if (effect is null)
		{
			actor.OutputHandler.Send("You are not editing any surgical procedures.");
			return;
		}

		actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<ISurgicalProcedure>>());
		actor.OutputHandler.Send("You are no longer editing any surgical procedures.");
	}

	private static void SurgeryEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<ISurgicalProcedure>>().FirstOrDefault();
			if (effect is null)
			{
				actor.OutputHandler.Send("Which surgical procedure do you want to edit?");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		if (ss.PeekSpeech().EqualTo("new"))
		{
			ss.PopSpeech();
			SurgeryEditNew(actor, ss);
			return;
		}

		var surgery = actor.Gameworld.SurgicalProcedures.GetByIdOrName(ss.SafeRemainingArgument);
		if (surgery is null)
		{
			actor.OutputHandler.Send("There is no such surgical procedure.");
			return;
		}

		actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<ISurgicalProcedure>>());
		actor.AddEffect(new BuilderEditingEffect<ISurgicalProcedure>(actor){EditingItem = surgery});
		actor.OutputHandler.Send($"You are now editing the {surgery.Name.ColourName()} surgical procedure.");
	}

	[PlayerCommand("Triage", "triage")]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[CommandPermission(PermissionLevel.NPC)]
	[RequiredCharacterState(CharacterState.Able)]
	[NoHideCommand]
	[NoMovementCommand]
	[NoMeleeCombatCommand]
	protected static void Triage(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		ICharacter target = null;
		target = ss.IsFinished ? actor : actor.TargetActor(ss.Pop());

		if (target == null)
		{
			actor.Send("You do not see anyone like that upon whom you can perform a triage.");
			return;
		}

		if (target.Movement != null)
		{
			actor.Send("There is no way to perform a triage of a patient while they are moving.");
			return;
		}

		if (actor.Combat != null && actor.MeleeRange)
		{
			actor.Send("You cannot perform a triage of a patient while you are engaged in melee combat.");
			return;
		}

		if (target.Combat != null && target.MeleeRange)
		{
			actor.Send("You cannot perform a triage of a patient while they are engaged in melee combat.");
			return;
		}

		ISurgicalProcedure procedure = null;
		var procedures =
			actor.Gameworld.SurgicalProcedures.Where(
				     x =>
					     x.Procedure == SurgicalProcedureType.Triage &&
					     (x.KnowledgeRequired == null || actor.Knowledges.Contains(x.KnowledgeRequired)))
			     .OrderBy(x => x.BaseCheckBonus)
			     .ToList();
		do
		{
			if (!string.IsNullOrEmpty(actor.PreferredSurgicalSchool) &&
			    procedures.Any(
				    x =>
					    x.MedicalSchool.Equals(actor.PreferredSurgicalSchool,
						    StringComparison.InvariantCultureIgnoreCase)))
			{
				procedure =
					procedures.First(
						x =>
							x.MedicalSchool.Equals(actor.PreferredSurgicalSchool,
								StringComparison.InvariantCultureIgnoreCase));
				procedures.Clear();
			}
			else
			{
				procedure = procedures.LastOrDefault();
			}

			if (procedure == null)
			{
				actor.Send("You do not know how to perform any triage procedures that you can use in this scenario.");
				return;
			}

			procedures.Remove(procedure);
		} while (!procedure.CanPerformProcedure(actor, target));

		if (!procedure.CanPerformProcedure(actor, target))
		{
			actor.Send(procedure.WhyCannotPerformProcedure(actor, target));
			return;
		}

		if (CharacterState.Able.HasFlag(target.State))
		{
			target.AddEffect(new Accept(target, new GenericProposal(
				text =>
				{
					target.OutputHandler.Handle(
						new EmoteOutput(new Emote($"@ agree|agrees to $0's offer to {procedure.ProcedureName}.",
							target, actor)));
					if (!procedure.CanPerformProcedure(actor, target))
					{
						actor.Send(procedure.WhyCannotPerformProcedure(actor, target));
						return;
					}

					procedure.PerformProcedure(actor, target);
				},
				text =>
				{
					target.OutputHandler.Handle(
						new EmoteOutput(new Emote($"@ decline|declines $0's offer to {procedure.ProcedureName}.",
							target, actor)));
				},
				() =>
				{
					target.OutputHandler.Handle(
						new EmoteOutput(new Emote($"@ decline|declines $0's offer to {procedure.ProcedureName}.",
							target, actor)));
				}, $"proposing to {procedure.ProcedureName}", "surgery",
				procedure.ProcedureName)), TimeSpan.FromSeconds(120));
			actor.OutputHandler.Handle(
				new EmoteOutput(new Emote($"@ propose|proposes that #0 perform the {procedure.ProcedureName.ToLowerInvariant().ColourName()} procedure $1", actor, actor,
					target)));
			target.OutputHandler.Send(Accept.StandardAcceptPhrasing);
			return;
		}

		procedure.PerformProcedure(actor, target);
	}

	[PlayerCommand("Wounds", "wounds")]
	protected static void Wounds(ICharacter actor, string command)
	{
		var sb = new StringBuilder();
		var wounds = actor.VisibleWounds(actor, WoundExaminationType.Self)
		                  .Select(x => (Wound: x,
			                  Description: x.Describe(WoundExaminationType.Self, Outcome.MajorPass)))
		                  .Where(x => !string.IsNullOrWhiteSpace(x.Description))
		                  .Select(x => $"{x.Description} on your {x.Wound.Bodypart.FullDescription()}")
		                  .ToList();
		if (wounds.Any())
		{
			sb.AppendLine("You have the following wounds:");
			sb.AppendLine();
			sb.Append(wounds.ListToString().Proper())
			  .Append(".");
		}
		else
		{
			sb.AppendLine("You do not have any wounds.");
		}

		if (actor.Body.SeveredRoots.Any())
		{
			var severs =
				actor.Body.SeveredRoots.Where(x => !(x is IOrganProto) && x.Significant)
				     .GroupBy(x => actor.Body.GetLimbFor(x)).ToList();
			foreach (var item in severs.Where(x =>
				         !actor.Body.Prosthetics.Any(y => x.Any(z => y.IncludedParts.Contains(z)))))
			{
				if (item.Key == null)
				{
					sb.Append($"\nYou are missing your {item.Select(x => x.FullDescription()).ListToString()}.");
					continue;
				}

				sb.Append(
					$"\nYour {item.Key.Name} is severed at the {item.Single(x => item.Except(x).All(y => y.DownstreamOfPart(x))).FullDescription()}.");
			}

			var insignificantSevers = actor.Body.SeveredRoots.Where(x => !x.Significant).ToList();
			if (insignificantSevers.Any())
			{
				sb.Append(
					$"\nYou are missing your {insignificantSevers.Select(x => x.FullDescription()).ListToString()}.");
			}
		}

		actor.Send(sb.ToString());
	}

	#region Delayed Action Delegates

	private static void CheckTendWounds(ICharacter actor, ICharacter target)
	{
		var inventoryPlan = actor.Gameworld.TendInventoryPlanTemplate.CreatePlan(actor);
		if (target == null)
		{
			actor.Send("You cancel your minstrations as your patient is no longer there.");
			inventoryPlan.FinalisePlan();
			return;
		}

		if (target.Location != actor.Location)
		{
			actor.Send("You cancel your minstrations as your patient is no longer there.");
			inventoryPlan.FinalisePlan();
			return;
		}

		if (target.State == CharacterState.Dead)
		{
			actor.Send("You cancel your minstrations as your patient has died.");
			inventoryPlan.FinalisePlan();
			return;
		}

		if (target.Movement != null)
		{
			actor.Send("You cancel your minstrations as your patient is moving about too much.");
			inventoryPlan.FinalisePlan();
			return;
		}

		if (target.Combat != null)
		{
			actor.Send("You cancel your minstrations as your patient is now in combat.");
			inventoryPlan.FinalisePlan();
			return;
		}

		var wounds = target.VisibleWounds(actor, WoundExaminationType.Look).ToList();

		if (wounds.All(x => x.CanBeTreated(TreatmentType.Tend) == Difficulty.Impossible))
		{
			actor.Send("You cancel your minstrations as your patient no longer has any wounds that need tending to.");
			inventoryPlan.FinalisePlan();
			return;
		}

		if (inventoryPlan.PlanIsFeasible() == InventoryPlanFeasibility.Feasible)
		{
			inventoryPlan.ExecuteWholePlan();
		}

		var maxDifficulty =
			target.Wounds.Select(x => x.CanBeTreated(TreatmentType.Tend))
			      .Where(x => x != Difficulty.Impossible)
			      .DefaultIfEmpty()
			      .Max();
		var treatmentItem =
			actor.Body.HeldItems.SelectNotNull(x => x.GetItemType<ITreatment>())
			     .Where(x => x.IsTreatmentType(TreatmentType.Tend))
			     .FirstMin(x => x.GetTreatmentDifficulty(maxDifficulty));

		if (treatmentItem == null || actor.Body.HeldItems.All(x => x != treatmentItem.Parent))
		{
			actor.Send("You cancel your minstrations as you no longer have any tools to work with.");
			inventoryPlan.FinalisePlan();
			return;
		}

		var worstWound =
			wounds.Where(x => x.CanBeTreated(TreatmentType.Tend) != Difficulty.Impossible)
			      .FirstMax(x => x.Severity);
		actor.OutputHandler.Handle(
			new EmoteOutput(
				new Emote(
					$"@ have|has finished tending to {worstWound.Describe(WoundExaminationType.Glance, Outcome.MajorPass)} on $0's {worstWound.Bodypart.FullDescription()} with $1.",
					actor, target,
					treatmentItem.Parent)));
		var sutureCheck = actor.Gameworld.GetCheck(CheckType.SutureWoundCheck);

		worstWound.Treat(actor, TreatmentType.Tend, treatmentItem,
			sutureCheck.Check(actor, worstWound.CanBeTreated(TreatmentType.Tend)), false);

		if (wounds.Any(x =>
			    x.CanBeTreated(TreatmentType.Tend) != Difficulty.Impossible))
		{
			if (actor.Body.HeldItems.All(x => x != treatmentItem.Parent))
			{
				inventoryPlan.FinalisePlan();
				actor.Send("You cancel your minstrations as you no longer have any tools to work with.");
				return;
			}

			actor.OutputHandler.Handle(
				new EmoteOutput(new Emote("@ continue|continues &0's efforts to tend to &0's patient's wounds.",
					actor, actor)));
			// TODO - tend duration soft coded
			actor.AddEffect(
				new SimpleCharacterAction(actor, perceivable => { CheckTendWounds(actor, target); },
					"tending to wounds", new[] { "general", "movement" }, "tending to wounds",
					perceivable => { inventoryPlan.FinalisePlan(); }),
				TimeSpan.FromSeconds(Dice.Roll(1, 30, 50)));
		}
		else
		{
			inventoryPlan.FinalisePlan();
		}
	}

	#endregion

	#region Surgery Sub-commands

	private static void SurgeryPerform(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Which surgical procedure do you want to perform?");
			return;
		}

		var targetProcedure = ss.PopSpeech();
		var procedure =
			actor.Gameworld.SurgicalProcedures.Where(x => actor.Knowledges.Contains(x.KnowledgeRequired))
			     .FirstOrDefault(x => x.Name.StartsWith(targetProcedure, StringComparison.InvariantCultureIgnoreCase));
		if (procedure == null)
		{
			actor.Send("You don't know any such procedure.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("Who do you want to perform that procedure on?");
			return;
		}

		ICharacter target;
		if (procedure.RequiresLivingPatient)
		{
			target = actor.TargetActor(ss.Pop());
			if (target == null)
			{
				actor.Send("You do not see anybody like that to perform that procedure on.");
				return;
			}
		}
		else
		{
			var targetPerceivable = actor.TargetLocal(ss.Pop());
			if (targetPerceivable == null)
			{
				actor.Send("You do not see anybody like that to perform that procedure on.");
				return;
			}

			if (targetPerceivable is ICharacter ch)
			{
				target = ch;
			}
			else if (targetPerceivable is IGameItem gi && gi.IsItemType<ICorpse>())
			{
				target = gi.GetItemType<ICorpse>().OriginalCharacter;
			}
			else
			{
				actor.Send("This procedure can only be used on people and corpses.");
				return;
			}
		}

		if (target == actor && !procedure.Procedure.In(SurgicalProcedureType.Triage,
			    SurgicalProcedureType.DetailedExamination, SurgicalProcedureType.Cannulation,
			    SurgicalProcedureType.Decannulation))
		{
			actor.Send("You cannot perform surgery on yourself...you're not that hardcore.");
			return;
		}

		if (procedure.RequiresUnconsciousPatient && !target.IsHelpless)
		{
			actor.Send("That procedure requires that your patient is unconscious or fully restrained.");
			return;
		}

		var remains = new StringStack(ss.RemainingArgument);
		remains.PopAll();
		if (!procedure.CanPerformProcedure(actor, target, remains.Memory.ToArray()))
		{
			actor.Send(procedure.WhyCannotPerformProcedure(actor, target, remains.Memory.ToArray()));
			return;
		}

		if (!target.IsHelpless && target != actor)
		{
			target.AddEffect(new Accept(target, new GenericProposal(
				text =>
				{
					target.OutputHandler.Handle(
						new EmoteOutput(new Emote(
							$"@ agree|agrees to $0's offer to perform the {procedure.ProcedureName.ColourName()} surgical procedure.",
							target, actor)));
					if (!procedure.CanPerformProcedure(actor, target, remains.Memory.ToArray()))
					{
						actor.Send(procedure.WhyCannotPerformProcedure(actor, target, remains.Memory.ToArray()));
						return;
					}

					procedure.PerformProcedure(actor, target, remains.Memory.ToArray());
				},
				text =>
				{
					target.OutputHandler.Handle(
						new EmoteOutput(new Emote(
							$"@ decline|declines $0's offer to perform the {procedure.ProcedureName.ColourName()} surgical procedure.",
							target, actor)));
				},
				() =>
				{
					target.OutputHandler.Handle(
						new EmoteOutput(new Emote(
							$"@ decline|declines $0's offer to perform the {procedure.ProcedureName.ColourName()} surgical procedure.",
							target, actor)));
				}, $"proposing to perform {procedure.ProcedureName}", "surgery",
				procedure.ProcedureName)), TimeSpan.FromSeconds(120));
			actor.OutputHandler.Handle(
				new EmoteOutput(new Emote(
					$"@ propose|proposes that #0 perform the {procedure.ProcedureName.ToLowerInvariant().ColourName()} surgical procedure on $1",
					actor, actor,
					target)));
			target.Send(
				$"You must type {"accept".Colour(Telnet.Yellow)} to permit them to do so, or {"decline".Colour(Telnet.Yellow)} to decline.");
			return;
		}

		procedure.PerformProcedure(actor, target, remains.Memory.ToArray());
	}

	private static void SurgeryShow(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			if (actor.IsAdministrator())
			{
				actor.OutputHandler.Send("Which surgical procedure do you want to show?");
				return;
			}

			actor.Send("Which surgical procedure do you want to show? See SURGERY LIST for a list of procedures you know.");
			return;
		}

		var targetProcedure = ss.SafeRemainingArgument;
		var procedure =
			actor.IsAdministrator() ?
				actor.Gameworld.SurgicalProcedures.GetByIdOrName(targetProcedure) :
			actor.Gameworld.SurgicalProcedures.Where(x => actor.Knowledges.Contains(x.KnowledgeRequired))
			     .FirstOrDefault(x => x.Name.StartsWith(targetProcedure, StringComparison.InvariantCultureIgnoreCase));
		if (procedure == null)
		{
			if (actor.IsAdministrator())
			{
				actor.OutputHandler.Send("There is no such procedure.");
				return;
			}
			actor.Send("You don't know any such procedure.");
			return;
		}

		if (actor.IsAdministrator())
		{
			actor.OutputHandler.Send(procedure.Show(actor));
			return;
		}

		// TODO - better player version of this

		var sb = new StringBuilder();
		sb.AppendLine($"Procedure: {procedure.ProcedureName}");
		sb.AppendLine();
		sb.AppendLine(procedure.ProcedureDescription);
		sb.AppendLine();
		sb.AppendLine(
			$"Time Taken: {(TimeSpan.FromTicks(procedure.Phases.Sum(x => x.BaseLength.Ticks)) + TimeSpan.FromSeconds(10)).Describe()}");
		sb.AppendLine($"Requires Living Patient: {(procedure.RequiresLivingPatient ? "Yes" : "No")}");
		sb.AppendLine(
			$"Requires Unconscious/Restrained Patient: {(procedure.RequiresUnconsciousPatient ? "Yes" : "No")}");
		sb.AppendLine($"Requires Finalisation: {(procedure.RequiresInvasiveProcedureFinalisation ? "Yes" : "No")}");

		actor.Send(sb.ToString());
	}

	private static void SurgeryListAdmin(ICharacter actor, StringStack ss)
	{
		var sb = new StringBuilder();
		sb.AppendLine("Surgical Procedures:");
		sb.AppendLine(StringUtilities.GetTextTable(
			actor.Gameworld.SurgicalProcedures
			     .Select(
				     x =>
					     new[]
					     {
							 x.Id.ToString("N0", actor),
						     x.Name, 
						     x.ProcedureName, 
						     x.MedicalSchool,
						     x.Procedure.DescribeEnum(),
						     x.KnowledgeRequired?.Name ?? "",
							 x.CheckTrait?.Name ?? ""
					     }),
			new[] { "Id", "Name", "Procedure", "Medical School", "Type", "Knowledge", "Trait" },
			actor.LineFormatLength,
			colour: Telnet.Green
		));
		actor.Send(sb.ToString());
	}

	private static void SurgeryList(ICharacter actor, StringStack ss)
	{
		if (actor.IsAdministrator())
		{
			SurgeryListAdmin(actor, ss);
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine("You know the following surgeries:");
		sb.AppendLine(StringUtilities.GetTextTable(
			actor.Gameworld.SurgicalProcedures.Where(x => actor.Knowledges.Contains(x.KnowledgeRequired))
			     .Select(
				     x =>
					     new[]
					     {
						     x.Name, x.ProcedureName, x.MedicalSchool,
						     x.RequiresInvasiveProcedureFinalisation.ToString(actor),
						     x.RequiresUnconsciousPatient.ToString(actor)
					     }),
			new[] { "Name", "Procedure", "Medical School", "Requires Stitch Up", "Requires Unconscious" },
			actor.LineFormatLength,
			colour: Telnet.Green
		));
		actor.Send(sb.ToString());
	}

	#endregion

	#region Admin Commands

	[PlayerCommand("Cure", "cure")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void Cure(ICharacter actor, string command)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("Only adminstrators may do that.");
			return;
		}

		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.Peek().EqualToAny("help", "?"))
		{
			actor.Send(
				"This command cures wounds or other ailments. The syntax is:\n\tcure <target> - cures all of the target's wounds one level of severity\n\tcure <target> <bodypart> - cures all of the target's wounds on a particular bodypart one level of severity.\n\tcure <target> all - removes all of a target's wounds, infections, refills blood\n\tcure <target> blood - adds 10% blood to the target\n\tcure <target> infections - removes all of the target's part infections\n\tcure <target> bones - cures all of the targets bone fractures 1 stage");
		}

		var target = ss.IsFinished ? actor : actor.Target(ss.Pop());

		if (target == null)
		{
			actor.Send("There is nothing like that for you to cure.");
			return;
		}

		if ((target as IGameItem)?.IsItemType<ICorpse>() == true)
		{
			target = (target as IGameItem).GetItemType<ICorpse>().OriginalCharacter;
		}

		if (target is not IHaveWounds targetAsHaveWounds)
		{
			actor.Send("{0} is not something that has wounds to be cured.",
				target.HowSeen(actor, true, flags: PerceiveIgnoreFlags.IgnoreSelf));
			return;
		}

		var tch = target as ICharacter;

		if (ss.Peek().EqualTo("blood"))
		{
			if (tch == null)
			{
				actor.Send("{0} is not something that has blood to be restored.",
					target.HowSeen(actor, true, flags: PerceiveIgnoreFlags.IgnoreSelf));
				return;
			}

			tch.Body.CurrentBloodVolumeLitres += Math.Min(tch.Body.TotalBloodVolumeLitres * 0.1,
				tch.Body.TotalBloodVolumeLitres - tch.Body.CurrentBloodVolumeLitres);
			actor.Send("You restore some blood to {0}.",
				target.HowSeen(actor, true, flags: PerceiveIgnoreFlags.IgnoreSelf));
			return;
		}

		if (ss.Peek().EqualTo("infections"))
		{
			if (tch == null)
			{
				actor.Send("{0} is not something that has infections to be removed.",
					target.HowSeen(actor, true, flags: PerceiveIgnoreFlags.IgnoreSelf));
				return;
			}

			foreach (var infection in tch.Body.PartInfections.ToList())
			{
				tch.Body.RemoveInfection(infection);
			}

			actor.Send("You remove all part infections for {0}.",
				target.HowSeen(actor, true, flags: PerceiveIgnoreFlags.IgnoreSelf));
			return;
		}

		if (ss.Peek().EqualTo("bones"))
		{
			if (tch == null)
			{
				actor.OutputHandler.Send(
					$"{target.HowSeen(actor, true, flags: PerceiveIgnoreFlags.IgnoreSelf)} is not something that has bones.");
				return;
			}

			foreach (var wound in tch.Body.Wounds.OfType<BoneFracture>().ToList())
			{
				wound.AddFractureStageProgress(1.0);
			}

			actor.OutputHandler.Send(
				$"You add substantial healing to all bone fractures for {target.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreSelf)}.");
			return;
		}

		var cureType = TreatmentType.Mend;
		if (ss.Peek().EqualTo("all"))
		{
			targetAsHaveWounds.CureAllWounds();
			actor.Send("You remove all of {0} wounds.",
				target.HowSeen(actor, type: DescriptionType.Possessive));
			return;
		}

		IBodypart part = null;
		if (!ss.IsFinished && tch != null)
		{
			part = tch.Body.GetTargetBodypart(ss.PopSpeech()) ??
			       tch.Body.Organs.FirstOrDefault(x => x.FullDescription().EqualTo(ss.Last) ||
			                                           x.Name.EqualTo(ss.Last));
			if (part == null)
			{
				actor.Send($"{target.HowSeen(actor, true)} has no such bodypart or organ to cure.");
				return;
			}
		}

		foreach (var wound in targetAsHaveWounds.Wounds.Where(x => x.Bodypart == part || part == null).ToList())
		{
			wound.Treat(actor, cureType, null, Outcome.MinorPass, true);
		}

		targetAsHaveWounds.EvaluateWounds();
		actor.Send("You {0} all of {1} wounds{2}.", cureType.ToString(),
			target.HowSeen(actor, type: DescriptionType.Possessive),
			part == null ? "" : $" on the {part.FullDescription()}"
		);
	}

	[PlayerCommand("Wound", "wound")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	[HelpInfo("wound", @"This command is used by storytellers to manually wound characters. This could be useful if you need to have an NPC wounded for story purposes, or want to simulate a damage source from something that isn't coded.

The syntax is either of the following:

	#3wound <target> <bodypart/random/all/*limb/none> <damagetype> <damage> [<pain>] [<stun>] [<lodged item>]#0
	#3wound <target> with <weapon> <bodypart/random/all/*limb/none> [<attack>] [<success>]#0", AutoHelp.HelpArgOrNoArg)]
	protected static void Wound(ICharacter actor, string command)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("Only administrators may do that.");
			return;
		}

		var ss = new StringStack(command.RemoveFirstWord());

		var target = actor.Target(ss.Pop());
		if (target == null)
		{
			actor.Send("You do not see that here to wound.");
			return;
		}

		if (target is not IHaveWounds woundable)
		{
			actor.Send("{0} is not something that can be wounded.", target.HowSeen(actor, true));
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(@"What body part do you want to wound?

Your options are to name the specific bodypart, #3random#0 for a random part, #3all#0 to hit all their external bodyparts, #3*limb#0 to hit all bodyparts on a particular limb, or #3none#0 if you're targeting an item.".SubstituteANSIColour().Wrap(actor.InnerLineFormatLength));
			return;
		}

		var targetChar = target as ICharacter;
		IGameItem targetItem = null;
		IMeleeWeapon weapon = null;
		if (ss.Peek().Equals("with", StringComparison.InvariantCultureIgnoreCase))
		{
			ss.Pop();
			if (ss.IsFinished)
			{
				actor.Send($"Which weapon did you want to wound {target.HowSeen(actor)} with?");
				return;
			}

			targetItem = actor.TargetHeldItem(ss.Pop());
			if (targetItem == null)
			{
				actor.Send($"You do not see anything like that to wound {target.HowSeen(actor)} with.");
				return;
			}

			weapon = targetItem.GetItemType<IMeleeWeapon>();
			if (weapon == null)
			{
				actor.Send($"{targetItem.HowSeen(actor, true)} is not a weapon.");
				return;
			}
		}

		var all = false;
		ILimb limb = null;
		var targetBodyparts = new List<IBodypart>();
		var bpTarget = ss.PopSpeech();
		if (string.IsNullOrEmpty(bpTarget))
		{
			var targetBodypart = targetChar?.Body.RandomBodypart;
			if (targetBodypart != null)
			{
				targetBodyparts.Add(targetBodypart);
			}
		}
		else if (bpTarget.Equals("none", StringComparison.InvariantCultureIgnoreCase))
		{
			if (targetChar != null)
			{
				actor.Send("When targeting characters, you must supply a body part or use RANDOM.");
				return;
			}
		}
		else if (bpTarget.Equals("random", StringComparison.InvariantCultureIgnoreCase))
		{
			if (targetChar == null)
			{
				actor.Send("You cannot use body parts when targeting items. Use NONE instead.");
				return;
			}

			targetBodyparts.Add(targetChar.Body.RandomBodypart);
		}
		else if (bpTarget.EqualTo("all"))
		{
			if (targetChar == null)
			{
				actor.Send("You cannot use body parts when targeting items. Use NONE instead.");
				return;
			}

			targetBodyparts.AddRange(targetChar.Body.Bodyparts);
			all = true;
		}
		else if (bpTarget[0] == '*')
		{
			if (targetChar == null)
			{
				actor.Send("You cannot use body parts when targeting items. Use NONE instead.");
				return;
			}

			var limbTarget = bpTarget.Substring(1);
			limb = targetChar.Body.Limbs.FirstOrDefault(x => x.Name.EqualTo(limbTarget));
			if (limb == null)
			{
				actor.Send("Your target has no such limb for you to target.");
				return;
			}

			targetBodyparts.AddRange(targetChar.Body.BodypartsForLimb(limb));
		}
		else
		{
			if (targetChar == null)
			{
				actor.Send("You cannot use body parts when targeting items. Use NONE instead.");
				return;
			}

			var targetBodypart =
				targetChar.Body.Bodyparts.Concat(targetChar.Body.Organs).FirstOrDefault(
					x => x.Name.Equals(bpTarget, StringComparison.InvariantCultureIgnoreCase)) ??
				targetChar.Body.Bodyparts.Concat(targetChar.Body.Organs).FirstOrDefault(
					x => x.FullDescription().Equals(bpTarget, StringComparison.InvariantCultureIgnoreCase));
			if (targetBodypart == null)
			{
				actor.Send("They do not have any such body part for you to wound.");
				return;
			}

			targetBodyparts.Add(targetBodypart);
		}

		IDamage finalDamage;
		if (weapon == null)
		{
			if (ss.IsFinished)
			{
				actor.Send("What damage type do you want to do?");
				return;
			}

			if (!WoundExtensions.TryGetDamageType(ss.Pop(), out var damageType))
			{
				actor.Send("That is not a valid damage type.");
				return;
			}

			if (ss.IsFinished)
			{
				actor.Send("How much damage do you want to do? You can use a dice expression or a number.");
				return;
			}

			var damage = ss.PopSpeech();
			if (!Dice.IsDiceExpression(damage))
			{
				actor.Send("How much damage do you want to do? You can use a dice expression or a number.");
				return;
			}

			var pain = damage;
			if (!ss.IsFinished)
			{
				pain = ss.PopSpeech();
				if (!Dice.IsDiceExpression(pain))
				{
					actor.Send("How much pain do you want to do? You can use a dice expression or a number.");
					return;
				}
			}

			var stun = damage;
			if (!ss.IsFinished)
			{
				stun = ss.PopSpeech();
				if (!Dice.IsDiceExpression(stun))
				{
					actor.Send("How much stun do you want to do? You can use a dice expression or a number.");
					return;
				}
			}

			IGameItem lodged = null;
			if (!ss.IsFinished)
			{
				lodged = actor.TargetHeldItem(ss.Pop());
				if (lodged == null)
				{
					actor.Send("You do not have anything like that to lodge.");
					return;
				}

				if (all || limb != null)
				{
					actor.Send(
						"You cannot lodge things when you use the ALL or limb-target versions of the wound command.");
					return;
				}
			}

			var wounds = new List<IWound>();
			if (targetBodyparts.Count == 0)
			{
				finalDamage = new Damage
				{
					ActorOrigin = actor,
					LodgableItem = lodged,
					DamageType = damageType,
					Bodypart = null,
					DamageAmount = Dice.Roll(damage),
					PainAmount = Dice.Roll(pain),
					ShockAmount = 0,
					StunAmount = Dice.Roll(stun),
					AngleOfIncidentRadians = Math.PI / 2
				};

				wounds.AddRange(woundable.SufferDamage(finalDamage));
			}
			else
			{
				foreach (var bodypart in targetBodyparts)
				{
					finalDamage = new Damage
					{
						ActorOrigin = actor,
						LodgableItem = lodged,
						DamageType = damageType,
						Bodypart = bodypart,
						DamageAmount = Dice.Roll(damage),
						PainAmount = Dice.Roll(pain),
						ShockAmount = 0,
						StunAmount = Dice.Roll(stun),
						AngleOfIncidentRadians = Math.PI / 2
					};

					wounds.AddRange(woundable.SufferDamage(finalDamage));
				}
			}
			

			var sb = new StringBuilder();
			sb.Append($"@ wound|wounds $0 for {damage.Colour(Telnet.Red)}d|{stun.Colour(Telnet.BoldCyan)}s|{pain.Colour(Telnet.Magenta)}p {damageType.Describe().ColourValue()}");
			if (!wounds.Any())
			{
				sb.Append(", but don't|doesn't cause any wounds.");
			}
			else
			{
				sb.AppendLine(", causing the following wounds:");
				foreach (var wound in wounds)
				{
					if (wound.Bodypart is null)
					{
						sb.AppendLine($"\t{wound.Describe(WoundExaminationType.Omniscient, Outcome.MajorPass).Colour(Telnet.Red)} on {wound.Parent.HowSeen(actor)}");
						continue;
					}
					sb.AppendLine($"\t{wound.Describe(WoundExaminationType.Omniscient, Outcome.MajorPass).Colour(Telnet.Red)} on {wound.Bodypart.FullDescription().ColourCommand()}");
				}
			}
			actor.OutputHandler.Handle(
				new EmoteOutput(
					new Emote(sb.ToString(), actor, target), flags: OutputFlags.WizOnly));
			woundable.CheckHealthStatus();
			if (lodged != null && wounds.Any(x => x.Lodged == lodged))
			{
				actor.Body.Take(lodged);
			}
		}
		else
		{
			var attack = ss.IsFinished
				? weapon.WeaponType.Attacks.GetRandomElement()
				: weapon.WeaponType.Attacks.GetFromItemListByKeyword(ss.Pop(), actor);

			if (attack == null)
			{
				actor.Send("There is no such weapon attack to use against that target.");
				return;
			}

			OpposedOutcomeDegree degree;
			if (ss.IsFinished)
			{
				degree = (OpposedOutcomeDegree)RandomUtilities.Random(0, 5);
			}
			else
			{
				if (!OpposedOutcomeExtensions.TryParse(ss.Pop(), out degree))
				{
					actor.Send("There is no such degree of success.");
					return;
				}
			}

			Outcome outcome;
			if (ss.IsFinished)
			{
				outcome = (Outcome)RandomUtilities.Random(2, 7);
			}
			else
			{
				if (!CheckExtensions.GetOutcome(ss.PopSpeech(), out outcome))
				{
					actor.Send("There is no such outcome for the penetration check.");
					return;
				}
			}

			attack.Profile.DamageExpression.Formula.Parameters["degree"] = (int)degree;
			attack.Profile.DamageExpression.Formula.Parameters["quality"] = (int)weapon.Parent.Quality;
			attack.Profile.StunExpression.Formula.Parameters["degree"] = (int)degree;
			attack.Profile.StunExpression.Formula.Parameters["quality"] = (int)weapon.Parent.Quality;
			attack.Profile.PainExpression.Formula.Parameters["degree"] = (int)degree;
			attack.Profile.PainExpression.Formula.Parameters["quality"] = (int)weapon.Parent.Quality;

			var angle = RandomUtilities.DoubleRandom(Math.PI * 0.1, Math.PI);
			var wounds = new List<IWound>();
			if (targetBodyparts.Count == 0)
			{
				finalDamage = new Damage
				{
					ActorOrigin = actor,
					LodgableItem = null,
					ToolOrigin = targetItem,
					AngleOfIncidentRadians = angle,
					Bodypart = null,
					DamageAmount = attack.Profile.DamageExpression.Evaluate(actor) * 2 * angle / Math.PI,
					DamageType = attack.Profile.DamageType,
					PainAmount = attack.Profile.PainExpression.Evaluate(actor) * 2 * angle / Math.PI,
					PenetrationOutcome = outcome,
					ShockAmount = 0,
					StunAmount = attack.Profile.DamageExpression.Evaluate(actor) * 2 * angle / Math.PI
				};

				wounds.AddRange(woundable.SufferDamage(finalDamage));
			}
			else
			{
				foreach (var bodypart in targetBodyparts)
				{
					finalDamage = new Damage
					{
						ActorOrigin = actor,
						LodgableItem = null,
						ToolOrigin = targetItem,
						AngleOfIncidentRadians = angle,
						Bodypart = bodypart,
						DamageAmount = attack.Profile.DamageExpression.Evaluate(actor) * 2 * angle / Math.PI,
						DamageType = attack.Profile.DamageType,
						PainAmount = attack.Profile.PainExpression.Evaluate(actor) * 2 * angle / Math.PI,
						PenetrationOutcome = outcome,
						ShockAmount = 0,
						StunAmount = attack.Profile.DamageExpression.Evaluate(actor) * 2 * angle / Math.PI
					};

					wounds.AddRange(woundable.SufferDamage(finalDamage));
				}
			}
				

			woundable.CheckHealthStatus();
			var sb = new StringBuilder();
			sb.AppendLine("You cause the following wounds:");
			foreach (var wound in wounds)
			{
				if (wound.Bodypart is null)
				{
					sb.AppendLine($"\t{wound.Describe(WoundExaminationType.Omniscient, Outcome.MajorPass).Colour(Telnet.Red)} on {wound.Parent.HowSeen(actor)}");
					continue;
				}
				sb.AppendLine($"\t{wound.Describe(WoundExaminationType.Omniscient, Outcome.MajorPass).Colour(Telnet.Red)} on {wound.Bodypart.FullDescription().ColourCommand()}");
			}
			actor.OutputHandler.Send(sb.ToString());
		}
	}

	private static void PlayerHealth(ICharacter actor)
	{
		actor.Send(actor.ShowHealth(actor));
	}

	[PlayerCommand("Health", "health")]
	protected static void Health(ICharacter actor, string command)
	{
		if (!actor.IsAdministrator(PermissionLevel.JuniorAdmin))
		{
			PlayerHealth(actor);
			return;
		}

		var ss = new StringStack(command.RemoveFirstWord());
		var target = ss.IsFinished ? actor : actor.Target(ss.PopSpeech());

		if (target == null)
		{
			actor.Send("You do not see anyone like that to view the health of.");
			return;
		}

		if (target is not IHaveWounds targetWoundable)
		{
			actor.Send("{0} does not have any wounds.", target.HowSeen(actor, true));
			return;
		}

		var targetAsCharacter = target as ICharacter ??
		                        (target as IGameItem)?.GetItemType<ICorpse>()?.OriginalCharacter;
		if ((target as IGameItem)?.IsItemType<ICorpse>() ?? false)
		{
			targetWoundable = (target as IGameItem).GetItemType<ICorpse>().OriginalCharacter;
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Health information for {target.HowSeen(actor)}:".GetLineWithTitle(actor, Telnet.Red, Telnet.BoldWhite));
		if (targetAsCharacter != null)
		{
			sb.AppendLine();
			sb.AppendLine($"Bloodtype: {targetAsCharacter.Body.Bloodtype?.Name.ColourValue() ?? "None".ColourError()}");
			sb.AppendLine(
				$"Blood: {actor.Gameworld.UnitManager.DescribeMostSignificantExact(targetAsCharacter.Body.CurrentBloodVolumeLitres / actor.Gameworld.UnitManager.BaseFluidToLitres, UnitType.FluidVolume, actor).ColourValue()} of total {actor.Gameworld.UnitManager.DescribeMostSignificantExact(targetAsCharacter.Body.TotalBloodVolumeLitres / actor.Gameworld.UnitManager.BaseFluidToLitres, UnitType.FluidVolume, actor).ColourValue()} - {(targetAsCharacter.Body.CurrentBloodVolumeLitres / targetAsCharacter.Body.TotalBloodVolumeLitres).ToString("P1", actor).ColourValue()}");
			var bac = 10.0 * targetAsCharacter.NeedsModel.AlcoholLitres /
			          targetAsCharacter.Body.CurrentBloodVolumeLitres;
			sb.AppendLine($"BAC: {bac.ToString("N4", actor).ColourValue()}");
			sb.AppendLine(
				$"Condition: {targetAsCharacter.HealthStrategy.ReportConditionPrompt(targetAsCharacter, PromptType.Full)}");
			sb.AppendLine(
				$"Temperature: {targetAsCharacter.HealthStrategy.CurrentTemperatureStatus(targetAsCharacter).DescribeColour()}");
			sb.AppendLine(
				$"Stamina: {targetAsCharacter.CurrentStamina.ToString("N1", actor).ColourValue()} / {targetAsCharacter.MaximumStamina.ToString("N1", actor).ColourValue()}");
		}

		sb.AppendLine();
		if (targetAsCharacter != null)
		{
			sb.AppendLine("Organ Function".GetLineWithTitle(actor, Telnet.Red, Telnet.BoldWhite));
			sb.AppendLine();
			var types = new List<Type>
			{
				typeof(BrainProto),
				typeof(PositronicBrain),
				typeof(HeartProto),
				typeof(PowerCore),
				typeof(LungProto),
				typeof(LiverProto),
				typeof(SpleenProto),
				typeof(KidneyProto),
				typeof(TracheaProto),
				typeof(EsophagusProto),
				typeof(IntestinesProto),
				typeof(EarProto),
				typeof(SpeechSynthesizer)
			};

			foreach (var type in types.ToList())
			{
				if (targetAsCharacter.Body.Prototype.AllBodypartsBonesAndOrgans.All(x => !type.IsInstanceOfType(x)))
				{
					types.Remove(type);
				}
			}

			var strings = new List<string>();
			foreach (var type in types)
			{
				switch (type.Name)
				{
					case nameof(BrainProto):
						strings.Add($"Brain: {targetAsCharacter.Body.OrganFunction<BrainProto>().ToString("P2", actor).ColourValue()}");
						continue;
					case nameof(PositronicBrain):
						strings.Add($"Positronic Brain: {targetAsCharacter.Body.OrganFunction<PositronicBrain>().ToString("P2", actor).ColourValue()}");
						continue;
					case nameof(HeartProto):
						strings.Add($"Heart: {targetAsCharacter.Body.OrganFunction<HeartProto>().ToString("P2", actor).ColourValue()}");
						continue;
					case nameof(PowerCore):
						strings.Add($"Power Core: {targetAsCharacter.Body.OrganFunction<PowerCore>().ToString("P2", actor).ColourValue()}");
						continue;
					case nameof(LungProto):
						strings.Add($"Lungs: {targetAsCharacter.Body.OrganFunction<LungProto>().ToString("P2", actor).ColourValue()}");
						continue;
					case nameof(LiverProto):
						strings.Add($"Liver: {targetAsCharacter.Body.OrganFunction<LiverProto>().ToString("P2", actor).ColourValue()}");
						continue;
					case nameof(SpleenProto):
						strings.Add($"Spleen: {targetAsCharacter.Body.OrganFunction<SpleenProto>().ToString("P2", actor).ColourValue()}");
						continue;
					case nameof(KidneyProto):
						strings.Add($"Kidneys: {targetAsCharacter.Body.OrganFunction<KidneyProto>().ToString("P2", actor).ColourValue()}");
						continue;
					case nameof(TracheaProto):
						strings.Add($"Trachea: {targetAsCharacter.Body.OrganFunction<TracheaProto>().ToString("P2", actor).ColourValue()}");
						continue;
					case nameof(EsophagusProto):
						strings.Add($"Esophagus: {targetAsCharacter.Body.OrganFunction<EsophagusProto>().ToString("P2", actor).ColourValue()}");
						continue;
					case nameof(IntestinesProto):
						strings.Add($"Intestines: {targetAsCharacter.Body.OrganFunction<IntestinesProto>().ToString("P2", actor).ColourValue()}");
						continue;
					case nameof(EarProto):
						strings.Add($"Ears: {targetAsCharacter.Body.OrganFunction<EarProto>().ToString("P2", actor).ColourValue()}");
						continue;
					case nameof(SpeechSynthesizer):
						strings.Add($"Speech Synthesizer: {targetAsCharacter.Body.OrganFunction<SpeechSynthesizer>().ToString("P2", actor).ColourValue()}");
						continue;
				}
			}

			while (strings.Count >= 3)
			{
				sb.AppendLineColumns(Math.Min(120, (uint)actor.LineFormatLength), 3,
					strings[0],
					strings[1],
					strings[2]
				);
				strings.RemoveRange(0,3);
			}

			if (strings.Count == 2)
			{
				sb.AppendLineColumns(Math.Min(120, (uint)actor.LineFormatLength), 3,
					strings[0],
					strings[1],
					""
				);
			}
			else if (strings.Count == 1)
			{
				sb.AppendLineColumns(Math.Min(120, (uint)actor.LineFormatLength), 3,
					strings[0],
					"",
					""
				);
			}

			sb.AppendLine();
		}

		var fractures = targetWoundable.Wounds.OfType<BoneFracture>().ToArray();
		var nonFractures = targetWoundable.Wounds.Except(fractures).ToArray();

		sb.AppendLine("Wounds".GetLineWithTitle(actor, Telnet.Red, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(StringUtilities.GetTextTable(
			from wound in targetWoundable.Wounds
			select new List<string>
			{
				wound.Severity.Describe(),
				wound.WoundTypeDescription,
				wound.Bodypart?.FullDescription() ?? "",
				wound.DamageType.Describe(),
				wound.CurrentDamage.ToStringN2Colour(actor),
				wound.CurrentStun.ToStringN2Colour(actor),
				wound.CurrentPain.ToStringN2Colour(actor),
				wound.TextForAdminWoundsCommand
			},
			new List<string>
			{
				"Severity",
				"Type",
				"Bodypart",
				"Type",
				"Dam",
				"Stun",
				"Pain",
				"Other"
			},
			actor,
			Telnet.Red
		));

		//foreach (var wound in targetWoundable.Wounds)
		//{
		//	if (wound is BoneFracture bf)
		//	{
		//		sb.AppendLine(
		//			$"{$"{wound.Severity.Describe()} Fracture".Colour(Telnet.Yellow)} on {wound.Bodypart.FullDescription().Colour(Telnet.Cyan)} - Hp: {wound.CurrentDamage.ToString("N1", actor).Colour(Telnet.Red)} - {bf.Stage} {bf.FractureStagePercentage():P2}{(bf.HasBeenSurgicallyReinforced ? " [s]" : "")}");
		//		continue;
		//	}

		//	sb.AppendLine(
		//		$"{wound.Describe(WoundExaminationType.Omniscient, Outcome.MajorPass).Colour(Telnet.Yellow)} ({wound.DamageType.Describe().Colour(Telnet.Magenta)}) on {wound.Bodypart?.FullDescription().Colour(Telnet.Cyan) ?? "None"} - Hp: {wound.CurrentDamage.ToString("N1", actor).Colour(Telnet.Red)}, St: {wound.CurrentStun.ToString("N1", actor).Colour(Telnet.Red)}, Pn: {wound.CurrentPain.ToString("N1", actor).Colour(Telnet.Red)} - {wound.BleedStatus}{(wound.Infection != null ? $" - {wound.Infection.InfectionType.Describe()} ({wound.Infection.VirulenceDifficulty.Describe()}) @ {wound.Infection.Intensity.ToString("P3", actor).ColourValue()} i[{wound.Infection.Immunity.ToString("P3", actor).ColourValue()}]" : "")}");
		//}

		if (targetAsCharacter != null && targetAsCharacter.Body.PartInfections.Any())
		{
			sb.AppendLine();
			sb.AppendLine("Part Infections".GetLineWithTitle(actor, Telnet.Red, Telnet.BoldWhite));
			foreach (var infection in targetAsCharacter.Body.PartInfections)
			{
				if (infection.Bodypart == null)
				{
					sb.AppendLine(
						$"{infection.VirulenceDifficulty.Describe()} {infection.InfectionType.Describe()} in whole body, Intensity {infection.Intensity.ToString("P3", actor).ColourValue()} Immunity {infection.Immunity.ToString("P3", actor).ColourValue()}");
				}
				else
				{
					sb.AppendLine(
						$"{infection.VirulenceDifficulty.Describe()} {infection.InfectionType.Describe()} on {infection.Bodypart.FullDescription()}, Intensity {infection.Intensity.ToString("P3", actor).ColourValue()} Immunity {infection.Immunity.ToString("P3", actor).ColourValue()}");
				}
			}
		}

		actor.Send(sb.ToString());
	}

	[PlayerCommand("Sever", "sever")]
	[CommandPermission(PermissionLevel.Admin)]
	protected static void Sever(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.Send("Who is the unlucky victim of your severing?");
			return;
		}

		var target = actor.TargetActorOrCorpse(ss.PopSpeech());
		if (target == null)
		{
			var targetItem = actor.TargetItem(ss.Last);
			if (targetItem?.IsItemType<ICorpse>() ?? false)
			{
				target = targetItem?.GetItemType<ICorpse>().OriginalCharacter;
			}
			else
			{
				actor.Send("You don't see anyone like that to sever from.");
				return;
			}
		}

		if (ss.IsFinished)
		{
			actor.Send($"Which bodypart do you want to sever from {target.HowSeen(actor)}?");
			return;
		}

		var targetBodypartText = ss.PopSpeech();
		var targetBodypart =
				target.Body.Bodyparts.FirstOrDefault(
					x =>
						x.FullDescription()
						 .Equals(targetBodypartText, StringComparison.InvariantCultureIgnoreCase)) ??
				target.Body.Bodyparts.FirstOrDefault(
					x => x.Name.Equals(targetBodypartText, StringComparison.InvariantCultureIgnoreCase)) ??
				target.Body.Bodyparts.FirstOrDefault(
					x =>
						x.FullDescription()
						 .StartsWith(targetBodypartText, StringComparison.InvariantCultureIgnoreCase)) ??
				target.Body.Bodyparts.FirstOrDefault(
					x => x.Name.StartsWith(targetBodypartText, StringComparison.InvariantCultureIgnoreCase)) ??
				target.Body.Organs.FirstOrDefault(
					x =>
						x.FullDescription()
						 .StartsWith(targetBodypartText, StringComparison.InvariantCultureIgnoreCase)) ??
				target.Body.Organs.FirstOrDefault(
					x => x.Name.StartsWith(targetBodypartText, StringComparison.InvariantCultureIgnoreCase))
			;

		if (targetBodypart == null)
		{
			actor.Send("Your target doesn't have any such bodypart or organ. They may have lost it already!");
			return;
		}

		IGameItem item;
		string verb;
		if (targetBodypart is IOrganProto organ)
		{
			item = target.Body.ExciseOrgan(organ);
			verb = "excise|excises";
		}
		else
		{
			if (!target.Body.CanSeverBodypart(targetBodypart))
			{
				actor.Send("That bodypart cannot be severed.");
				return;
			}

			item = target.Body.SeverBodypart(targetBodypart);
			verb = "sever|severs";
		}

		actor.OutputHandler.Handle(
			new EmoteOutput(new Emote($"@ {verb} $0's {targetBodypart.ShortDescription()}!", actor, target)));

		if (item != null)
		{
			actor.Gameworld.Add(item);
			item.RoomLayer = actor.RoomLayer;
			actor.Location.Insert(item);
			item.Login();
			item.HandleEvent(EventType.ItemFinishedLoading, item);
		}

		target.Body.StartHealthTick();
		target.Body.CheckHealthStatus();
	}

	[PlayerCommand("Unsever", "unsever")]
	[CommandPermission(PermissionLevel.Admin)]
	protected static void Unsever(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.Send("To whom do you wish to restore the use of a bodypart?");
			return;
		}

		var target = actor.TargetActorOrCorpse(ss.PopSpeech());
		if (target == null)
		{
			var targetItem = actor.TargetItem(ss.Last);
			if (targetItem?.IsItemType<ICorpse>() ?? false)
			{
				target = targetItem?.GetItemType<ICorpse>().OriginalCharacter;
			}
			else
			{
				actor.Send("You don't see anyone like that to restore severed bodyparts to.");
				return;
			}
		}

		if (ss.IsFinished)
		{
			actor.Send($"Which bodypart do you want to restore to {target.HowSeen(actor)}?");
			return;
		}

		var targetBodypartText = ss.PopSpeech();

		if (targetBodypartText.EqualTo("all"))
		{
			target.Body.RestoreAllBodypartsOrgansAndBones();
			actor.OutputHandler.Handle(
				new EmoteOutput(new Emote($"@ restore|restores all of $0's severed bodyparts, organs and bones!", actor,
					target)));
			return;
		}

		var targetBodypart =
				target.Body.SeveredRoots.FirstOrDefault(
					x =>
						x.FullDescription()
						 .Equals(targetBodypartText, StringComparison.InvariantCultureIgnoreCase)) ??
				target.Body.SeveredRoots.FirstOrDefault(
					x => x.Name.Equals(targetBodypartText, StringComparison.InvariantCultureIgnoreCase)) ??
				target.Body.SeveredRoots.FirstOrDefault(
					x =>
						x.FullDescription()
						 .StartsWith(targetBodypartText, StringComparison.InvariantCultureIgnoreCase)) ??
				target.Body.SeveredRoots.FirstOrDefault(
					x => x.Name.StartsWith(targetBodypartText, StringComparison.InvariantCultureIgnoreCase))
			;

		if (targetBodypart == null)
		{
			actor.Send("Your target doesn't have any sever at the nominated location.");
			return;
		}

		if (targetBodypart is IOrganProto organ)
		{
			target.Body.RestoreOrgan(organ);
		}
		else if (targetBodypart is IBone bone)
		{
			actor.Send("Restoring bones is not currently supported.");
			return;
		}
		else
		{
			target.Body.RestoreBodypart(targetBodypart);
		}

		actor.OutputHandler.Handle(
			new EmoteOutput(new Emote($"@ restore|restores $0's severed {targetBodypart.ShortDescription()}!", actor,
				target)));
	}

	[PlayerCommand("InstallImplant", "installimplant")]
	[CommandPermission(PermissionLevel.Admin)]
	protected static void InstallImplant(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.Send("Which implant do you want to install?");
			return;
		}

		var item = actor.TargetHeldItem(ss.PopSpeech());
		if (item == null)
		{
			actor.Send("You're not holding anything like that.");
			return;
		}

		if (!(item.GetItemType<IImplant>() is IImplant implant))
		{
			actor.OutputHandler.Send($"{item.HowSeen(actor, true)} is not an implant.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("Who do you want to install that implant into?");
			return;
		}

		var target = actor.TargetActorOrCorpse(ss.PopSpeech());
		if (target == null)
		{
			actor.Send("You don't see anyone like that to install that implant into.");
			return;
		}

		IBodypart bodypart = null;
		if (implant.TargetBodypart == null)
		{
			if (ss.IsFinished)
			{
				actor.Send("Which bodypart do you want to install that implant into?");
				return;
			}

			var targetPart = target.Body.GetTargetBodypart(ss.PopSpeech());
			if (targetPart == null)
			{
				actor.Send("They have no such bodypart.");
				return;
			}

			bodypart = targetPart;
		}

		actor.Body.Take(item);
		target.Body.InstallImplant(implant);
		if (bodypart != null)
		{
			implant.TargetBodypart = bodypart;
		}

		actor.Send($"You install {item.HowSeen(actor)} into {target.HowSeen(actor)}.");
	}


	[PlayerCommand("PowerImplant", "powerimplant")]
	[CommandPermission(PermissionLevel.Admin)]
	protected static void PowerImplant(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.Send("Who do you want to change implant powering arrangements for?");
			return;
		}

		var target = actor.TargetActorOrCorpse(ss.PopSpeech());
		if (target == null)
		{
			actor.Send("You don't see anyone like that to change the implant powering arrangements for.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("Which implant of theirs do you want to change the powering arrangements for?");
			return;
		}

		var implant = target.Body.Implants.Select(x => x.Parent).GetFromItemListByKeyword(ss.PopSpeech(), actor)
		                    ?.GetItemType<IImplant>();
		if (implant == null)
		{
			actor.Send($"{target.HowSeen(actor, true)} has no such implant.");
			return;
		}

		var implantPower = implant.Parent.GetItemType<IImplantPowerSupply>();
		if (implantPower == null)
		{
			actor.Send($"{implant.Parent.HowSeen(actor, true)} is not a powered implant.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("Which power plant do you want to connect that implant to?");
			return;
		}

		var powerplant = target.Body.Implants.Select(x => x.Parent).GetFromItemListByKeyword(ss.PopSpeech(), actor)
		                       ?.GetItemType<IImplantPowerPlant>();
		if (powerplant == null)
		{
			actor.Send($"{target.HowSeen(actor, true)} has no such power plant implant.");
			return;
		}

		implantPower.PowerPlant = powerplant;
		actor.Send(
			$"{target.HowSeen(actor, true, DescriptionType.Possessive)} implant {implant.Parent.HowSeen(actor)} will now be powered by {powerplant.Parent.HowSeen(actor)}.");
	}

	[PlayerCommand("ConnectImplants", "connectimplant")]
	[CommandPermission(PermissionLevel.Admin)]
	protected static void ConnectImplant(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.Send("Who do you want to change implant connection arrangements for?");
			return;
		}

		var target = actor.TargetActorOrCorpse(ss.PopSpeech());
		if (target == null)
		{
			actor.Send("You don't see anyone like that to change the implant connection arrangements for.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("Which implant of theirs do you want to change the connection arrangements for?");
			return;
		}

		var implant = target.Body.Implants.Select(x => x.Parent).GetFromItemListByKeyword(ss.PopSpeech(), actor)
		                    ?.GetItemType<IImplant>();
		if (implant == null)
		{
			actor.Send($"{target.HowSeen(actor, true)} has no such implant.");
			return;
		}

		if (implant is IImplantNeuralLink)
		{
			actor.Send("Neural interface implants cannot be connected to other neural interfaces.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("Which neural interface implant do you want to connect that implant to?");
			return;
		}

		var neural = target.Body.Implants.Select(x => x.Parent).GetFromItemListByKeyword(ss.PopSpeech(), actor)
		                   ?.GetItemType<IImplantNeuralLink>();
		if (neural == null)
		{
			actor.Send($"{target.HowSeen(actor, true)} has no such neural implant.");
			return;
		}

		if (neural.IsLinkedTo(implant))
		{
			actor.OutputHandler.Send(
				$"{target.HowSeen(actor, true, DescriptionType.Possessive)} implant {implant.Parent.HowSeen(actor)} is already connected to {neural.Parent.HowSeen(actor)}.");
			return;
		}

		foreach (var otherneural in target.Body.Implants.OfType<IImplantNeuralLink>().Except(neural))
		{
			otherneural.RemoveLink(implant);
		}

		neural.AddLink(implant);
		if (implant is IImplantRespondToCommands irtc)
		{
			irtc.AliasForCommands = $"implant{irtc.Id:F0}";
		}

		actor.Send(
			$"You connect {target.HowSeen(actor, false, DescriptionType.Possessive)} implant {implant.Parent.HowSeen(actor)} to the neural interface implant {neural.Parent.HowSeen(actor)}.");
	}

	[PlayerCommand("Implants", "implants")]
	[CommandPermission(PermissionLevel.Admin)]
	protected static void Implants(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Whose implants do you want to view?");
			return;
		}

		var target = actor.TargetActorOrCorpse(ss.PopSpeech());
		if (target == null)
		{
			actor.OutputHandler.Send("You don't see anyone like that to view the implants for.");
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine(
			$"{target.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreSelf)} has the following implants:");
		var dnis = target.Body.Implants.OfType<IImplantNeuralLink>().ToList();
		foreach (var implant in target.Body.Implants)
		{
			sb.AppendLine(
				$"\t{implant.Parent.HowSeen(actor)} in {implant.TargetBodypart.FullDescription()}: Powerplant [{implant.Parent.GetItemType<IImplantPowerSupply>()?.PowerPlant?.Parent.HowSeen(actor) ?? "none"}], DNI [{dnis.FirstOrDefault(x => x.IsLinkedTo(implant))?.Parent.HowSeen(actor) ?? "none"}]");
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	[PlayerCommand("Infect", "infect")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void Infect(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var targetWound = false;
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Do you want to infect a wound or a bodypart?");
			return;
		}

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "wound":
				targetWound = true;
				break;
			case "bodypart":
			case "part":
				break;
			default:
				actor.OutputHandler.Send("Do you want to infect a wound or a bodypart?");
				return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Who do you want to infect?");
			return;
		}

		var target = actor.TargetActor(ss.PopSpeech());
		if (target == null)
		{
			actor.OutputHandler.Send("You don't see anyone like that.");
			return;
		}

		var terrain = target.Location.Terrain(target);
		if (ss.IsFinished)
		{
			if (targetWound)
			{
				var wound = target.Body.Wounds.Where(x => x.Infection == null && x.EligableForInfection())
				                  .GetRandomElement();
				wound.Infection = Infection.LoadNewInfection(terrain.PrimaryInfection, terrain.InfectionVirulence,
					0.0001, target.Body, wound, wound.Bodypart, terrain.InfectionMultiplier);
				actor.OutputHandler.Send(
					$"You infect {target.HowSeen(actor, type: DescriptionType.Possessive)} wound \"{wound.Describe(WoundExaminationType.Omniscient, Outcome.MajorPass)}\"");
				return;
			}

			var bodypart = target.Body.Bodyparts.Concat(target.Body.Organs).GetRandomElement();
			target.Body.AddInfection(Infection.LoadNewInfection(terrain.PrimaryInfection, terrain.InfectionVirulence,
				0.0001, target.Body, null, bodypart, terrain.InfectionMultiplier));
			actor.OutputHandler.Send(
				$"You infect {target.HowSeen(actor, type: DescriptionType.Possessive)} bodypart \"{bodypart.FullDescription().Colour(Telnet.Yellow)}\"");
			return;
		}

		if (targetWound)
		{
			var wound = target.Body.Wounds.GetFromItemListByKeyword(ss.PopSpeech(), actor);
			if (wound == null)
			{
				actor.OutputHandler.Send($"{target.HowSeen(actor, true)} has no such wound.");
				return;
			}

			wound.Infection = Infection.LoadNewInfection(terrain.PrimaryInfection, terrain.InfectionVirulence, 0.0001,
				target.Body, wound, wound.Bodypart, terrain.InfectionMultiplier);
			actor.OutputHandler.Send(
				$"You infect {target.HowSeen(actor, type: DescriptionType.Possessive)} wound \"{wound.Describe(WoundExaminationType.Omniscient, Outcome.MajorPass)}\"");
			return;
		}

		var targetBodypart = target.Body.Bodyparts.Concat(target.Body.Organs)
		                           .GetFromItemListByKeyword(ss.PopSpeech(), actor);
		if (targetBodypart == null)
		{
			actor.OutputHandler.Send($"{target.HowSeen(actor, true)} has no such bodypart or organ.");
			return;
		}

		target.Body.AddInfection(Infection.LoadNewInfection(terrain.PrimaryInfection, terrain.InfectionVirulence,
			0.0001, target.Body, null, targetBodypart, terrain.InfectionMultiplier));
		actor.OutputHandler.Send(
			$"You infect {target.HowSeen(actor, type: DescriptionType.Possessive)} bodypart \"{targetBodypart.FullDescription().Colour(Telnet.Yellow)}\"");
		return;
	}

	[PlayerCommand("Exsanguinate", "exsanguinate")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	[HelpInfo("exsanguinate",
		"This command allows you to change the current blood percentage of a character. The syntax is EXSANGUINATE <target> [<percentage>]. If you omit the percentage, it will set the blood to 0%.",
		AutoHelp.HelpArgOrNoArg)]
	protected static void Exsanguinate(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var target = actor.TargetActor(ss.PopSpeech());
		if (target == null)
		{
			actor.OutputHandler.Send("You don't see anyone like that.");
			return;
		}

		var percentage = 0.0;
		if (!ss.IsFinished)
		{
			if (!ss.PopSpeech().TryParsePercentage(out percentage) || percentage < 0.0 || percentage > 1.0)
			{
				actor.OutputHandler.Send("That is not a valid percentage.");
				return;
			}
		}

		target.Body.CurrentBloodVolumeLitres = target.Body.TotalBloodVolumeLitres * percentage;
		actor.OutputHandler.Send(new EmoteOutput(new Emote(
			$"@ set|sets $1's blood percentage to {percentage.ToString("P3", actor).ColourValue()}.", actor, actor,
			target)));
		target.Body.StartHealthTick();
		target.Body.CheckHealthStatus();
	}

	#endregion
}
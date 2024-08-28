using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Accounts;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.Construction.Boundary;
using MudSharp.Effects;
using MudSharp.RPG.Law;

namespace MudSharp.Commands.Modules;

public class CombatModule : Module<ICharacter>
{
	private CombatModule()
		: base("Combat")
	{
		IsNecessary = true;
	}

	public static CombatModule Instance { get; } = new();

	public override int CommandsDisplayOrder => 8;

	[PlayerCommand("Targets", "targets")]
	[HelpInfo("Targets", @"The #3Targets#0 command shows you viable ranged targets you have seen, either in your own location or using the various #3scan#0 commands in adjacent locations. Only targets on this list can be targeted with ranged attacks or effects.

See also the related #3quickscan#0, #3scan#0 and #3longscan#0 commands.

The syntax for this command is simply #3targets#0.", AutoHelp.HelpArg)]
	protected static void Targets(ICharacter actor, string command)
	{
		var sb = new StringBuilder();
		sb.AppendLine("You have eyes on the following targets:");
		var targets = actor.Location.LayerCharacters(actor.RoomLayer).Except(actor)
						   .Concat<IMortalPerceiver>(actor.Location.LayerGameItems(actor.RoomLayer))
						   .Where(x => actor.CanSee(x))
						   .Concat(actor.SeenTargets).ToList();
		foreach (var thing in targets)
		{
			sb.AppendLine($"\t{thing.HowSeen(actor)}");
		}

		actor.Send(sb.ToString());
	}

	[PlayerCommand("Burden", "burden")]
	[HelpInfo("burden", @"The burden command allows you to manually make combat checks harder on yourself; to go easy on someone, or appear weaker than you are. Burden for offense and defense are separate values, and they are not saved in the database so need to be set each time you log in.

The degrees referenced below are difficulty levels - for each ""degree"" of burden a check becomes one level harder.

The syntax is:

	#3burden reset#0 - resets your burden to default, i.e. no burden
	#3burden <degrees>#0 - applies a burden of x degrees to all offensive and defensive checks
	#3burden offense <degrees>#0 - sets the offensive burden specifically
	#3burden defense <degrees>#0 - sets the defensive burden specifically", AutoHelp.HelpArgOrNoArg)]
	protected static void Burden(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());

		if (ss.Peek().EqualToAny("reset", "clear", "none", "normal"))
		{
			actor.CombatBurdenDefense = 0;
			actor.CombatBurdenOffense = 0;
			actor.Send("You will now fight to the limits of your own ability only.");
			return;
		}

		bool offense = false, defense = false;
		var degrees = 0;
		switch (ss.Pop().ToLowerInvariant())
		{
			case "offense":
				offense = true;
				if (ss.IsFinished)
				{
					actor.Send("How many degrees of burden do you want to set for yourself on offense?");
					return;
				}

				if (!int.TryParse(ss.Pop(), out degrees) || degrees < 0 || degrees > 10)
				{
					actor.Send("You must set a valid number of degrees. Minimum 0, maximum 10.");
					return;
				}

				break;
			case "defense":
				defense = true;
				if (ss.IsFinished)
				{
					actor.Send("How many degrees of burden do you want to set for yourself on defense?");
					return;
				}

				if (!int.TryParse(ss.Pop(), out degrees) || degrees < 0 || degrees > 10)
				{
					actor.Send("You must set a valid number of degrees. Minimum 0, maximum 10.");
					return;
				}

				break;
			case var degText when int.TryParse(degText, out var value):
				degrees = value;
				offense = true;
				defense = true;
				if (value < 0 || value > 10)
				{
					actor.Send("You must set a valid number of degrees. Minimum 0, maximum 10.");
					return;
				}

				break;
			default:
				actor.Send(
					"The syntax is:\n\tburden reset - resets your burden to default, i.e. no burden\n\tburden <degrees> - applies a burden of x degrees to all offensive and defensive checks\n\tburden offense <degrees> - sets the offensive burden specifically\n\tburden defense <degrees> - sets the defensive burden specifically");
				return;
		}

		if (offense)
		{
			actor.CombatBurdenOffense = degrees;
		}

		if (defense)
		{
			actor.CombatBurdenDefense = degrees;
		}

		actor.Send(
			$"You will now fight with a burden of {degrees} degrees to all {(offense && defense ? "offense and defense" : offense ? "offense" : "defense")} rolls in combat.");
		actor.Send(
			$"This would make all checks that were previously {"normal".Colour(Telnet.Green)} now {Difficulty.Normal.StageUp(degrees).Describe().Colour(Telnet.Red)}, and all checks that were previously previously {"very easy".Colour(Telnet.Green)} now {Difficulty.VeryEasy.StageUp(degrees).Describe().Colour(Telnet.Red)} for example."
				.Wrap(actor.InnerLineFormatLength));
	}

	[PlayerCommand("Release", "release")]
	[HelpInfo("Release", @"The #3Release#0 command is used to let go of someone who you are grappling or dragging.

You can use the command in one of three ways:

	#3release#0 - releases your target normally
	#3release kneel#0 - releases your target and forces them to kneel (if they are capable of kneeling)
	#3release sprawl#0 - releases your target and sends them sprawling", AutoHelp.HelpArg)]
	protected static void Release(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (!actor.EffectsOfType<IGrappling>().Any() && !actor.AffectedBy<Dragging>())
		{
			actor.Send("You are not holding anyone that you can release.");
			return;
		}

		var emote = new PlayerEmote(ss.PopParentheses(), actor);
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return;
		}

		var grapple = actor.EffectsOfType<IGrappling>().FirstOrDefault();
		var drag = actor.EffectsOfType<Dragging>().FirstOrDefault();
		var effect = (IEffect)grapple ?? drag;
		var target = grapple?.Target ?? drag.Target as ICharacter;

		if (target == null)
		{
			actor.Send("You are not holding anyone that you can release.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Handle(
				new MixedEmoteOutput(new Emote("@ release|releases $1 from &0's grip", actor, actor, target),
					flags: OutputFlags.InnerWrap).Append(emote));
			actor.RemoveEffect(effect, true);
			return;
		}

		if (ss.Peek().EqualTo("kneel") || ss.Peek().EqualTo("knees"))
		{
			if (!target.ValidPositions.Contains(PositionKneeling.Instance))
			{
				actor.OutputHandler.Send($"Unfortunately {target.HowSeen(actor)} does not have a physiology capable of kneeling.");
				return;
			}
			actor.OutputHandler.Handle(
				new MixedEmoteOutput(
					new Emote("@ push|pushes $1 down onto &1's knees and release|releases &1 from &0's grip", actor,
						actor, target), flags: OutputFlags.InnerWrap).Append(emote));
			target.SetPosition(PositionKneeling.Instance, PositionModifier.None, actor, null);
			actor.RemoveEffect(effect, true);
			return;
		}

		if (ss.Peek().EqualTo("sprawl"))
		{
			actor.OutputHandler.Handle(
				new MixedEmoteOutput(
					new Emote("@ release|releases &1 from &0's grip and send|sends &1 sprawling", actor, actor, target),
					flags: OutputFlags.InnerWrap).Append(emote));
			target.SetPosition(PositionSprawled.Instance, PositionModifier.None, actor, null);
			actor.RemoveEffect(effect, true);
			return;
		}

		var targetExit = actor.Location.GetExitKeyword(ss.Pop(), actor);
		if (targetExit == null)
		{
			actor.OutputHandler.Send("There is no exit in that direction.");
			return;
		}

		if (targetExit.Exit.Door?.IsOpen == false)
		{
			actor.OutputHandler.Send("There is a closed door in the way of that exit.");
			return;
		}

		if (ss.Peek().EqualTo("sprawl"))
		{
			actor.RemoveEffect(effect, true);
			actor.OutputHandler.Handle(
				new MixedEmoteOutput(
					new Emote(
						$"@ release|releases &1 from &0's grip push|pushes &1 to {targetExit.OutboundDirectionDescription}, sending &1 sprawling",
						actor, actor, target), flags: OutputFlags.InnerWrap).Append(emote));
			actor.Location.Leave(target);
			targetExit.Destination.Enter(target);
			target.OutputHandler.Handle(new EmoteOutput(
				new Emote($"@ stagger|staggers {targetExit.InboundMovementSuffix} and is sent sprawling on the ground.",
					target), flags: OutputFlags.SuppressSource));
			target.SetPosition(PositionSprawled.Instance, PositionModifier.None, actor, null);
			return;
		}

		actor.RemoveEffect(effect, true);
		actor.OutputHandler.Handle(
			new MixedEmoteOutput(
				new Emote(
					$"@ release|releases &1 from &0's grip push|pushes &1 to {targetExit.OutboundDirectionDescription}",
					actor, actor, target), flags: OutputFlags.InnerWrap).Append(emote));
		actor.Location.Leave(target);
		targetExit.Destination.Enter(target);
		target.OutputHandler.Handle(new EmoteOutput(
			new Emote($"@ stagger|staggers {targetExit.InboundMovementSuffix}.", target),
			flags: OutputFlags.SuppressSource));
	}

	protected static (bool Truth, string Error) CanBeSurrenderedTo(ICharacter target, ICharacter surrenderer)
	{
		if (!target.State.IsAble())
		{
			return (false,
				$"You cannot surrender to {target.HowSeen(surrenderer)} because {target.ApparentGender(surrenderer).Subjective()} is {target.State.Describe()}.");
		}

		if (target.Movement?.Phase == Movement.MovementPhase.OriginalRoom)
		{
			return (false,
				$"You cannot surrender to {target.HowSeen(surrenderer)} because {target.ApparentGender(surrenderer).Subjective()} is moving away.");
		}

		var blockingEffects =
			target.CombinedEffectsOfType<IEffect>().Where(x => x.IsBlockingEffect("general")).ToList();
		if (blockingEffects.Any())
		{
			return (false,
				$"You cannot surrender to {target.HowSeen(surrenderer)} because {target.ApparentGender(surrenderer).Subjective()} is {blockingEffects.Select(x => x.BlockingDescription("general", surrenderer)).ListToString()}.");
		}

		if (target.CombinedEffectsOfType<IPacifismEffect>().Any(x => x.IsSuperPeaceful))
		{
			return (false,
				$"You cannot surrender to {target.HowSeen(surrenderer)} because {target.ApparentGender(surrenderer).Subjective()} is too peaceful.");
		}

		if (target.IsHelpless)
		{
			return (false,
				$"You cannot surrender to {target.HowSeen(surrenderer)} because {target.ApparentGender(surrenderer).Subjective()} is helpless.");
		}

		if (target.NoMercy)
		{
			return (false,
				$"You cannot surrender to {target.HowSeen(surrenderer)} because {target.ApparentGender(surrenderer).Subjective()} is showing no mercy.");
		}

		return (true, string.Empty);
	}

	[PlayerCommand("Surrender", "surrender")]
	[RequiredCharacterState(CharacterState.Able)]
	[NoMovementCommand]
	[HelpInfo("Surrender", @"The #3Surrender#0 command is used to voluntarily submit to someone, often a combat opponent or enforcer. The target must be willing to accept your surrender, and not all opponents can or will.

If used outside of combat you will automatically become a drag target for the person you surrender too.
If used inside of combat your opponent will get a grapple on every single one of your limbs.

The syntax is one of the following:

	#3surrender#0 - surrender to any person attacking you who will accept your surrender
	#3surrender <target>#0 - try to surrender to a particular person", AutoHelp.HelpArg)]
	protected static void Surrender(ICharacter actor, string command)
	{
		var blockingEffects = actor.CombinedEffectsOfType<IEffect>()
								   .Where(x => x.IsBlockingEffect("general") && !x.CanBeStoppedByPlayer).ToList();
		if (blockingEffects.Any())
		{
			actor.OutputHandler.Send(
				$"You cannot surrender until you are no longer {blockingEffects.Select(x => x.BlockingDescription("general", actor)).ListToString()}.");
			return;
		}

		var ss = new StringStack(command.RemoveFirstWord());
		ICharacter target = null;
		if (ss.IsFinished)
		{
			if (actor.AffectedBy<WarnedByEnforcer>())
			{
				var wbe = actor.EffectsOfType<WarnedByEnforcer>().First();
				target = wbe.WhichPatrol.PatrolMembers.Where(x => x.ColocatedWith(actor)).GetRandomElement();
			}

			if (target == null)
			{
				if (actor.Combat == null)
				{
					actor.OutputHandler.Send("You are not in combat, so you must specify someone to surrender to.");
					return;
				}

				if (actor.CombatTarget?.ColocatedWith(actor) != true &&
					actor.Combat.Combatants.All(x => x.CombatTarget != actor))
				{
					actor.OutputHandler.Send(
						"You're not in close proximity to anyone you can surrender to. Try specifying a target.");
					return;
				}

				target = actor.Combat.Combatants.OfType<ICharacter>().FirstOrDefault(x =>
					(x.CombatTarget == actor || actor.CombatTarget == x) && CanBeSurrenderedTo(x, actor).Truth);
				if (target == null)
				{
					actor.OutputHandler.Send(
						"There is nobody who could receive your surrender fighting you. Try specifying a target.");
					return;
				}
			}
		}
		else
		{
			target = actor.TargetActor(ss.PopSpeech(), PerceiveIgnoreFlags.IgnoreSelf);
			if (target == null)
			{
				actor.OutputHandler.Send("You don't see anyone like that who you could surrender to.");
				return;
			}

			var (truth, error) = CanBeSurrenderedTo(target, actor);
			if (!truth)
			{
				actor.OutputHandler.Send(error);
				return;
			}
		}

		actor.RemoveAllEffects(x => x.IsBlockingEffect("general") && x.CanBeStoppedByPlayer);
		actor.CombatTarget = null;
		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ surrender|surrenders to $1.", actor, actor, target)));
		if (target.Combat != null)
		{
			if (target.CombatTarget == null || target.CombatTarget == actor)
			{
				if (!target.AffectedBy<ClinchEffect>())
				{
					target.AddEffect(new ClinchEffect(target, actor));
					actor.AddEffect(new ClinchEffect(actor, target));
				}
			}

			var grapple = new Grappling(target, actor);
			target.AddEffect(grapple);
			foreach (var limb in actor.Body.Limbs.Where(x => actor.Body.BodypartsForLimb(x).Any()).ToList())
			{
				grapple.AddLimb(limb);
			}
		}
		else
		{
			var drag = new Dragging(target, null, actor);
			target.AddEffect(drag);
		}
	}

	[PlayerCommand("Grapple", "grapple")]
	[HelpInfo("grapple",
		@"The #3Grapple#0 command is used while you are already in combat to switch to a strategy of trying to grapple your opponent. There are several options for using this command:

	#3grapple#0 - toggles grappling on or off, defaults to control
	#3grapple control#0 - switches to grappling for control, trying to get all limbs grappled
	#3grapple incapacitate#0 - attempts to grapple someone and break their limbs
	#3grapple kill#0 - attempts to grapple someone and strangle them or break their neck",
		AutoHelp.HelpArg)]
	protected static void Grapple(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (actor.Combat == null)
		{
			// TODO - combat initiator?
			actor.Send("This command is only usable when you are in combat.");
			return;
		}

		if (!actor.MeleeRange || actor.CombatTarget == null)
		{
			actor.Send("You can only use this command when you are in melee range of a target.");
			return;
		}

		if (ss.IsFinished)
		{
			switch (actor.CombatStrategyMode)
			{
				case CombatStrategyMode.GrappleForControl:
				case CombatStrategyMode.GrappleForIncapacitation:
				case CombatStrategyMode.GrappleForKill:
					actor.CombatStrategyMode = actor.CombatSettings.PreferredMeleeMode.IsGrappleMode()
						? CombatStrategyMode.StandardMelee
						: actor.CombatSettings.PreferredMeleeMode;
					actor.Send(
						$"You will no longer attempt to grapple, and will instead {actor.CombatStrategyMode.DescribeWordy()}.");
					return;
				default:
					actor.CombatStrategyMode = CombatStrategyMode.GrappleForControl;
					actor.Send($"You will now {actor.CombatStrategyMode.DescribeWordy()}.");
					return;
			}
		}

		var desiredStrategy = CombatStrategyMode.GrappleForControl;
		switch (ss.Pop().ToLowerInvariant())
		{
			case "kill":
			case "strangle":
				desiredStrategy = CombatStrategyMode.GrappleForKill;
				break;
			case "injure":
			case "incapacitate":
			case "maim":
			case "hurt":
				desiredStrategy = CombatStrategyMode.GrappleForIncapacitation;
				break;
			case "control":
				break;
			default:
				actor.Send(
					"That is not a recognised way to grapple someone. Valid options are control, incapacitate and kill.");
				return;
		}

		actor.CombatStrategyMode = desiredStrategy;
		actor.Send($"You will now {actor.CombatStrategyMode.DescribeWordy()}.");
	}

	[PlayerCommand("Strangle", "strangle")]
	[HelpInfo("strangle", @"The #3Strangle#0 command is used to signal that you want to switch to a mode where you will try to grapple your opponent and kill them through moves such as strangulation.

Strangle attacks are only used from within grapples, so this mode will first attempt to get your opponent into a grapple.

The syntax is #3strangle#0 to toggle the mode on and off.", AutoHelp.HelpArg)]
	protected static void Strangle(ICharacter actor, string command)
	{
		if (actor.Combat == null)
		{
			actor.Send("This command is only usable when you are in combat.");
			return;
		}

		if (actor.EffectsOfType<FixedCombatMoveType>()
				 .Any(x => x.FixedTypes.Contains(BuiltInCombatMoveType.StrangleAttack)))
		{
			actor.RemoveAllEffects(x => x.IsEffectType<FixedCombatMoveType>());
			actor.Send("You will no longer try to strangle your opponent.");
			if (actor.MeleeRange && actor.CombatStrategyMode == CombatStrategyMode.GrappleForKill &&
				actor.CombatSettings.PreferredMeleeMode != CombatStrategyMode.GrappleForKill)
			{
				actor.CombatStrategyMode = actor.CombatSettings.PreferredMeleeMode;
			}

			return;
		}

		actor.RemoveAllEffects(x => x.IsEffectType<FixedCombatMoveType>());
		actor.AddEffect(new FixedCombatMoveType(actor, new[] { BuiltInCombatMoveType.StrangleAttack }, true));
		actor.Send("You will now try to strangle your opponent until you use this command again.");
		if (actor.MeleeRange && actor.CombatStrategyMode != CombatStrategyMode.Clinch)
		{
			actor.CombatStrategyMode = CombatStrategyMode.GrappleForKill;
		}
	}

	[PlayerCommand("Stagger", "stagger")]
	[HelpInfo("stagger", @"The #3Stagger#0 command is used in combat to signal that your next attack should be an staggering blow which attempts to render your opponent prone and at disadvantage. This kind of attack could be unarmed or with a weapon, though not all weapons will have staggering attacks.

The syntax is simply #3stagger#0 when in combat to signal your next attack to be a staggering attack.", AutoHelp.HelpArg)]
	protected static void Stagger(ICharacter actor, string command)
	{
		if (actor.Combat == null)
		{
			actor.Send("This command is only usable when you are in combat.");
			return;
		}

		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.RemoveAllEffects(x => x.IsEffectType<FixedCombatMoveType>());
			actor.AddEffect(new FixedCombatMoveType(actor,
				new[]
				{
					BuiltInCombatMoveType.StaggeringBlow, BuiltInCombatMoveType.StaggeringBlowUnarmed,
					BuiltInCombatMoveType.StaggeringBlowClinch
				}, false));
			actor.Send("You will now try to stagger your opponent on your next blow.");
			return;
		}

		if (ss.Peek().EqualTo("cancel") || ss.Peek().EqualTo("off") || ss.Peek().EqualTo("disable"))
		{
			actor.RemoveAllEffects(x => x.IsEffectType<FixedCombatMoveType>());
			actor.Send("You will not seek to stagger your opponent in melee.");
			return;
		}

		if (ss.Peek().EqualTo("always") || ss.Peek().EqualTo("on") || ss.Peek().EqualTo("only"))
		{
			actor.RemoveAllEffects(x => x.IsEffectType<FixedCombatMoveType>());
			actor.AddEffect(new FixedCombatMoveType(actor,
				new[]
				{
					BuiltInCombatMoveType.StaggeringBlow, BuiltInCombatMoveType.StaggeringBlowUnarmed,
					BuiltInCombatMoveType.StaggeringBlowClinch
				}, true));
			actor.Send("You will now try to stagger your opponent until you disable this setting.");
			return;
		}

		actor.Send($"The correct syntax is {"stagger [cancel|always]".Colour(Telnet.Yellow)}");
	}

	[PlayerCommand("Trip", "trip")]
	[HelpInfo("trip", @"The #3Trip#0 command is used in combat to signal that your next attack should be an unbalancing blow which attempts to render your opponent prone and at disadvantage. This kind of attack could be unarmed or with a weapon, though not all weapons will have trip attacks.

The syntax is simply #3trip#0 when you're in combat to signal that your next attack will be a trip attack.", AutoHelp.HelpArg)]
	protected static void Trip(ICharacter actor, string command)
	{
		if (actor.Combat == null)
		{
			actor.Send("This command is only usable when you are in combat.");
			return;
		}

		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.RemoveAllEffects(x => x.IsEffectType<FixedCombatMoveType>());
			actor.AddEffect(new FixedCombatMoveType(actor,
				new[]
				{
					BuiltInCombatMoveType.UnbalancingBlow, BuiltInCombatMoveType.UnbalancingBlowUnarmed,
					BuiltInCombatMoveType.UnbalancingBlowClinch
				}, false));
			actor.Send("You will now try to trip up your opponent on your next blow.");
			return;
		}

		if (ss.Peek().EqualTo("cancel") || ss.Peek().EqualTo("off") || ss.Peek().EqualTo("disable"))
		{
			actor.RemoveAllEffects(x => x.IsEffectType<FixedCombatMoveType>());
			actor.Send("You will not seek to trip up your opponent in melee.");
			return;
		}

		if (ss.Peek().EqualTo("always") || ss.Peek().EqualTo("on") || ss.Peek().EqualTo("only"))
		{
			actor.RemoveAllEffects(x => x.IsEffectType<FixedCombatMoveType>());
			actor.AddEffect(new FixedCombatMoveType(actor,
				new[]
				{
					BuiltInCombatMoveType.UnbalancingBlow, BuiltInCombatMoveType.UnbalancingBlowUnarmed,
					BuiltInCombatMoveType.UnbalancingBlowClinch
				}, true));
			actor.Send("You will now try to trip up your opponent until you disable this setting.");
			return;
		}

		actor.Send($"The correct syntax is {"trip [cancel|always]".Colour(Telnet.Yellow)}");
	}

	[PlayerCommand("Smash", "smash")]
	[RequiredCharacterState(CharacterState.Able)]
	[HelpInfo("smash", @"The #3Smash#0 command makes a single attack against an object not being held with the intention of damaging or breaking the object. It is the primary way to attack inanimate objects.

It can also be used to target doors (by supplying the direction as the target) or locks on items.

If you don't specify a weapon, it will prefer melee weapons over unarmed attacks.

The syntax for the smash command is as follows:

	#3smash <item>#0 - attack an item (with whatever weapon or unarmed attack is available)
	#3smash <item|exit> with <weapon>#0 - attack an item or door with a specified weapon
	#3smash <item|exit> with <bodypart>#0 - attack an item or door with a specified unarmed attack bodypart
	#3smash <item|exit> <lock> <weapon>#0 - attack a lock on an item or door with a specified weapon
	#3smash <item|exit> <lock> <bodypart>#0 - attack a lock on an item or door with a specified unarmed attack bodypart", AutoHelp.HelpArgOrNoArg)]
	protected static void Smash(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());

		var target = ss.PopSpeech();
		var targetExit = actor.Location.ExitsFor(actor).GetFromItemListByKeyword(target, actor);
		var targetItem = targetExit?.Exit.Door?.Parent;
		if (targetItem == null && targetExit != null)
		{
			actor.Send("There is no door in that direction for you to smash.");
			return;
		}

		if (targetItem == null)
		{
			targetItem = actor.TargetLocalItem(target);
			if (targetItem == null)
			{
				actor.Send("You don't see anything like that to smash.");
				return;
			}
		}

		ILock targetLock = null;
		if (!ss.IsFinished && !ss.Peek().EqualTo("with"))
		{
			target = ss.PopSpeech();
			targetLock = targetItem.GetItemType<ILockable>()?.Locks.Select(x => x.Parent)
								   .GetFromItemListByKeyword(target, actor)?.GetItemType<ILock>();
			if (targetLock == null)
			{
				actor.Send("{0} does not have any locks with those keywords for you to smash.",
					targetItem.HowSeen(actor, true));
				return;
			}
		}

		if (!actor.Location.CanGetAccess(targetItem, actor))
		{
			actor.Send(actor.Location.WhyCannotGetAccess(targetItem, actor));
			return;
		}

		IGameItem targetWeapon = null;
		IMeleeWeapon targetAsWeapon = null;

		var part = actor.Body.Prototype.DefaultDoorSmashingPart;
		var designatedPart = false;
		if (!ss.IsFinished)
		{
			ss.PopSpeech();
			if (ss.IsFinished)
			{
				actor.Send("Do you want to smash this thing with a weapon or a specific bodypart?");
				return;
			}

			target = ss.PopSpeech();
			targetWeapon = actor.TargetHeldItem(target);
			if (targetWeapon != null)
			{
				targetAsWeapon = targetWeapon.GetItemType<IMeleeWeapon>();
				if (targetAsWeapon == null)
				{
					actor.Send("{0} is not a melee weapon, and cannot be used to smash things.",
						targetWeapon.HowSeen(actor, true));
					return;
				}

				var attacks = targetAsWeapon.WeaponType.UsableAttacks(actor, targetWeapon,
					targetLock?.Parent ?? targetItem, targetAsWeapon.HandednessForWeapon(actor), false,
					BuiltInCombatMoveType.MeleeWeaponSmashItem).ToList();
				if (!attacks.Any())
				{
					actor.Send("{0} doesn't have any suitable attacks for smashing things.",
						targetWeapon.HowSeen(actor, true));
					return;
				}

				var attack = attacks.Where(x => actor.CanSpendStamina(x.StaminaCost))
									.GetWeightedRandom(x => x.Weighting);
				if (attack == null)
				{
					actor.Send("You don't have enough stamina to use any of {0}'s attacks.",
						targetWeapon.HowSeen(actor));
					return;
				}
			}
			else
			{
				part = actor.Body.GetTargetBodypart(target);
				if (part != null)
				{
					designatedPart = true;
				}
			}
		}
		else
		{
			var possibleWeapon = actor.Body.WieldedItems
									  .SelectNotNull(x => x.GetItemType<IMeleeWeapon>()).FirstOrDefault(x =>
										  x.WeaponType
										   .UsableAttacks(actor, x.Parent, targetLock?.Parent ?? targetItem,
											   x.HandednessForWeapon(actor), false,
											   BuiltInCombatMoveType.MeleeWeaponSmashItem)
										   .Any(y => actor.CanSpendStamina(y.StaminaCost)));
			if (possibleWeapon != null)
			{
				targetWeapon = possibleWeapon.Parent;
				targetAsWeapon = possibleWeapon;
			}
		}

		if (CrimeExtensions.HandleCrimesAndLawfulActing(actor, CrimeTypes.Vandalism, null, targetItem))
		{
			return;
		}

		if (targetAsWeapon != null)
		{
			var attacks = targetAsWeapon.WeaponType.UsableAttacks(actor, targetWeapon,
				targetLock?.Parent ?? targetItem, targetAsWeapon.HandednessForWeapon(actor), false,
				BuiltInCombatMoveType.MeleeWeaponSmashItem).ToList();
			var attack = attacks.Where(x => actor.CanSpendStamina(x.StaminaCost))
								.GetWeightedRandom(x => x.Weighting);
			if (actor.Combat != null)
			{
				if (actor.TakeOrQueueCombatAction(SelectedCombatAction.GetEffectSmashItem(actor,
						targetLock?.Parent ?? targetItem, targetLock == null ? null : targetItem, targetAsWeapon,
						attack)) && actor.Gameworld.GetStaticBool("EchoQueuedActions"))
				{
					actor.Send(
						$"{"[Queued Action]: ".ColourBold(Telnet.Yellow)}Smashing {(targetLock?.Parent ?? targetItem).HowSeen(actor)} with {targetWeapon.HowSeen(actor)}.");
				}

				return;
			}

			var move = new MeleeWeaponSmashItemAttack(attack)
			{
				Assailant = actor, Target = targetLock?.Parent ?? targetItem,
				ParentItem = targetLock == null ? null : targetItem, Weapon = targetAsWeapon
			};
			var result = move.ResolveMove(null);
			actor.SpendStamina(move.StaminaCost);
			actor.AddEffect(
				new CommandDelay(actor, "Smash",
					onExpireAction: () => { actor.Send("You feel as if you could smash something again."); }),
				TimeSpan.FromSeconds(10));
			return;
		}

		if (part == null)
		{
			actor.Send("You have no such bodypart with which to smash anything.");
			return;
		}

		if (!actor.Body.Bodyparts.Contains(part))
		{
			actor.Send(
				$"You are missing your {part.FullDescription()}, so you'll have to specify a different bodypart to smash with.");
			return;
		}

		var nattacks =
			actor.Race.UsableNaturalWeaponAttacks(actor, targetItem, false, BuiltInCombatMoveType.UnarmedSmashItem);
		if (!nattacks.Any())
		{
			actor.Send(
				$"You don't know any suitable unarmed attacks for smashing things with your {part.FullDescription()}.");
			return;
		}

		INaturalAttack nattack = null;
		if (designatedPart)
		{
			nattack = nattacks.FirstOrDefault(x => x.Bodypart.Id == part.Id);
			if (nattack == null)
			{
				actor.Send(
					$"You don't have any suitable unarmed attacks for smashing things with your {part.FullDescription()}.");
				return;
			}
		}
		else
		{
			nattack = nattacks.Where(x => actor.CanSpendStamina(x.Attack.StaminaCost))
							  .GetWeightedRandom(x => x.Attack.Weighting);
		}

		if (nattack == null)
		{
			actor.Send(
				$"You don't have enough stamina to perform any smash attacks with your {part.FullDescription()}.");
			return;
		}

		if (actor.Combat != null)
		{
			if (actor.TakeOrQueueCombatAction(SelectedCombatAction.GetEffectSmashItemUnarmed(actor,
					targetLock?.Parent ?? targetItem, targetLock == null ? null : targetItem, nattack)) &&
				actor.Gameworld.GetStaticBool("EchoQueuedActions"))
			{
				actor.Send(
					$"{"[Queued Action]: ".ColourBold(Telnet.Yellow)}Smashing {(targetLock?.Parent ?? targetItem).HowSeen(actor)}.");
			}

			return;
		}

		var nmove = new UnarmedSmashItemAttack(nattack.Attack)
		{
			Assailant = actor, Target = targetLock?.Parent ?? targetItem,
			ParentItem = targetLock == null ? null : targetItem, NaturalAttack = nattack
		};
		var nresult = nmove.ResolveMove(null);
		actor.SpendStamina(nmove.StaminaCost);
		actor.AddEffect(
			new CommandDelay(actor, "Smash",
				onExpireAction: () => { actor.Send("You feel as if you could smash something again."); }),
			TimeSpan.FromSeconds(10));
	}

	[PlayerCommand("Execute", "execute", "coupdegrace", "cdg")]
	[RequiredCharacterState(CharacterState.Able)]
	[DelayBlock("general", "You must first stop {0} before you can deliver the blow of mercy.")]
	[HelpInfo("execute",
		@"The #6Execute#0 command allows you to execute a powerful finishing move, designed generally to kill, mortally wound or maim a helpless opponent. 

It can be used both inside and outside of combat. When used outside of combat, it sets in motion a delayed action that, if not interrupted, will deliver the blow of mercy to the target. 

When used in combat, it signals to override combat settings which prevent a killing blow being given.

The syntax:
	#3execute list#0 - shows a list of available [5mcoup de grace[25m moves for wielded weapons.
	#3execute#0 - in combat, flags that a killing blow should be given if the opportunity arises.
	#3execute ""<Move Name>"" <target>#0 - delivers the specified [5mcoup de grace[25m to the target.",
		AutoHelp.HelpArgOrNoArg)]
	protected static void CoupDeGrace(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		var weapons = actor.Body.WieldedItems.SelectNotNull(x => x.GetItemType<IMeleeWeapon>()).ToList();

		if (ss.Peek().Equals("list", StringComparison.InvariantCultureIgnoreCase))
		{
			if (!weapons.Any())
			{
				actor.Send("You are not wielding any weapons, and so have no [5mcoup de grace[25m options.");
				return;
			}

			var sb = new StringBuilder();
			foreach (var item in weapons)
			{
				if (sb.Length > 0)
				{
					sb.AppendLine();
				}

				sb.AppendLine($@"[5mCoup de grace[25m options for {item.Parent.HowSeen(actor)}:");
				sb.AppendLine();
				foreach (
					var attack in
					item.WeaponType.Attacks.OfType<IFixedBodypartWeaponAttack>()
						.Where(x => x.UsableAttack(actor, item.Parent, null, AttackHandednessOptions.Any, false,
							BuiltInCombatMoveType.CoupDeGrace))
						.ToList())
				{
					var flag = "";
					switch (attack.HandednessOptions)
					{
						case AttackHandednessOptions.OneHandedOnly:
							flag = " [1h-only]".Colour(Telnet.BoldWhite);
							break;
						case AttackHandednessOptions.TwoHandedOnly:
							flag = " [2h-only]".Colour(Telnet.BoldWhite);
							break;
						case AttackHandednessOptions.DualWieldOnly:
							flag = " [dual-wield]".Colour(Telnet.BoldWhite);
							break;
						case AttackHandednessOptions.SwordAndBoardOnly:
							flag = " [with-shield]".Colour(Telnet.BoldWhite);
							break;
					}

					sb.AppendLine(
						$"\t{attack.Name.Colour(Telnet.Cyan)}{flag} - {attack.Profile.DamageType.Describe().Colour(Telnet.Magenta)} to {attack.Bodypart.Name.Colour(Telnet.Yellow)}.");
				}
			}

			actor.OutputHandler.Send(sb.ToString());
			return;
		}

		if (actor.Combat != null)
		{
			if (actor.EffectsOfType<PreparingCoupDeGrace>().Any())
			{
				actor.RemoveAllEffects(x => x.IsEffectType<PreparingCoupDeGrace>());
				actor.Send("You are no longer trying to deliver a [5mcoup de grace[25m to your target.");
				return;
			}

			if (actor.CombatTarget == null)
			{
				actor.Send("Imaginary opponents need no blows of mercy; get into the action first!");
				return;
			}

			if (!(actor.CombatTarget is ICharacter tch))
			{
				actor.Send("Inanimate objects neither expect nor deserve any mercy.");
				return;
			}

			if (!tch.IsHelpless && !tch.State.HasFlag(CharacterState.Sleeping))
			{
				actor.Send("You can only deliver a [5mcoup de grace[25m to a helpless opponent.");
				return;
			}
		}

		IMeleeWeapon weapon;
		IFixedBodypartWeaponAttack wattack;
		var attackText = ss.PopSpeech();
		if (string.IsNullOrEmpty(attackText))
		{
			weapon = weapons.GetRandomElement();
			if (weapon == null)
			{
				actor.OutputHandler.Send("You don't have any weapons.");
				return;
			}

			wattack = weapon.WeaponType.Attacks.OfType<IFixedBodypartWeaponAttack>()
							.Where(
								x =>
									x.UsableAttack(actor, weapon.Parent, actor.CombatTarget,
										weapon.HandednessForWeapon(actor),
										false, BuiltInCombatMoveType.CoupDeGrace)).GetWeightedRandom(x => x.Weighting);
			if (wattack == null)
			{
				actor.OutputHandler.Send(
					$"There are no available weapon attacks for you to use as a [5mcoup de grace[25m; perhaps try a different type of weapon? Also see {"execute list".MXPSend()} to see a list of available attacks or type {"execute help".MXPSend()} for more help.");
				return;
			}
		}
		else
		{
			wattack = weapons.SelectMany(x => x.WeaponType.Attacks.OfType<IFixedBodypartWeaponAttack>().Where(
								 y =>
									 y.UsableAttack(actor, x.Parent, actor.CombatTarget, x.HandednessForWeapon(actor),
										 false,
										 BuiltInCombatMoveType.CoupDeGrace)).ToList())
							 .FirstOrDefault(
								 x => x.Name.Equals(attackText, StringComparison.InvariantCultureIgnoreCase));
			if (wattack == null)
			{
				actor.Send(
					$"There is no such weapon attack for you to use as a [5mcoup de grace[25m. Try {"execute list".MXPSend()} to see a list of available attacks or type {"execute help".MXPSend()} for more help.");
				return;
			}

			weapon = weapons.First(x => x.WeaponType.Attacks.Contains(wattack));
		}

		if (actor.Combat == null)
		{
			if (ss.IsFinished)
			{
				actor.Send("Who do you want to deliver a [5mcoup de grace[25m to?");
				return;
			}

			var target = actor.TargetActor(ss.PopSpeech());
			if (target == null)
			{
				actor.Send("You don't see anyone like that to deliver a [5mcoup de grace[25m to.");
				return;
			}

			if (!target.IsHelpless && !target.State.HasFlag(CharacterState.Sleeping))
			{
				actor.Send("You can't deliver a [5mcoup de grace[25m to someone who is not helpless.");
				return;
			}

			PlayerEmote emote = null;
			if (!ss.IsFinished)
			{
				var text = ss.PopParentheses();
				if (!string.IsNullOrEmpty(text))
				{
					emote = new PlayerEmote(text, actor);
					if (!emote.Valid)
					{
						actor.Send(emote.ErrorMessage);
						return;
					}
				}
			}

			if (CrimeExtensions.HandleCrimesAndLawfulActing(actor, CrimeTypes.AttemptedMurder, target))
			{
				return;
			}

			if (!wattack.UsableAttack(actor, weapon.Parent, target, weapon.HandednessForWeapon(actor), false, BuiltInCombatMoveType.CoupDeGrace))
			{
				actor.OutputHandler.Send($"The {wattack.Name.ColourName()} [5mcoup de grace[25m is not usable against {target.HowSeen(actor)}.");
				return;
			}

			actor.OutputHandler.Handle(
				new EmoteOutput(new Emote("@ are|is preparing to deliver a [5mcoup de grace[25m to $1 with $2.", actor,
					actor, target, weapon.Parent)));
			actor.AddEffect(new PreparingCoupDeGrace(actor, weapon, wattack, target) { Emote = emote },
				TimeSpan.FromSeconds(25));
			CrimeExtensions.CheckPossibleCrimeAllAuthorities(actor, CrimeTypes.AttemptedMurder, target, weapon.Parent,
				"");
			return;
		}

		PlayerEmote pemote = null;
		if (!ss.IsFinished)
		{
			var text = ss.PopParentheses();
			if (!string.IsNullOrEmpty(text))
			{
				pemote = new PlayerEmote(text, actor);
				if (!pemote.Valid)
				{
					actor.Send(pemote.ErrorMessage);
					return;
				}
			}
		}

		actor.AddEffect(new CombatCoupDeGrace(actor, actor.Combat, weapon, wattack, pemote));
		actor.Send(
			$"You will now deliver the {wattack.Name.Colour(Telnet.Cyan)} [5mcoup de grace[25m to your opponent if you get the opportunity.");
	}

	[PlayerCommand("Rescue", "rescue")]
	[RequiredCharacterState(CharacterState.Able)]
	[DelayBlock("general", "You must first stop {0} before you can fight.")]
	[NoMovementCommand]
	[HelpInfo("rescue", @"The #3Rescue#0 command is used to try to draw the attention away from the attackers of a target. You will attempt to redirect the attackers towards yourself instead. You can only rescue one target at a time.

The syntax is #3rescue <target>#0 or #3rescue none#0 to stop trying to rescue them.", AutoHelp.HelpArgOrNoArg)]
	protected static void Rescue(ICharacter actor, string command)
	{
		var effect = actor.EffectsOfType<Rescue>().FirstOrDefault();
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished || ss.Peek().EqualTo("me") || ss.Peek().EqualTo("self") || ss.Peek().EqualTo("clear") ||
			ss.Peek().EqualTo("none") || ss.Peek().EqualTo("off"))
		{
			if (effect == null)
			{
				actor.Send("You are not currently attempting to rescue anybody.");
				return;
			}

			actor.RemoveEffect(effect);
			actor.Send("You are no longer attempting to rescue {0}.", effect.RescueTarget.HowSeen(actor));
			return;
		}

		var target = actor.TargetActor(ss.Pop());
		if (target == null)
		{
			actor.Send("You don't see anybody like that to rescue.");
			return;
		}

		if (target.Combat == null || !target.MeleeRange ||
			target.Combat.Combatants.All(x => x.CombatTarget != target))
		{
			actor.Send("{0} is not in need of any rescuing.", target.HowSeen(actor, true));
			return;
		}

		if (effect != null)
		{
			if (effect.RescueTarget == target)
			{
				actor.Send("You are already attempting to rescue {0}.", target.HowSeen(actor));
				return;
			}

			actor.RemoveEffect(effect);
		}

		if (actor.Combat != null && actor.Combat != target.Combat)
		{
			actor.Send("You must end your own combat first before you can join someone elses!");
			return;
		}

		if (actor.Combat == null)
		{
			target.Combat.JoinCombat(actor);
			var potentialTargets =
				target.Combat.Combatants.Where(
					x =>
						x.CombatTarget == target && x.ColocatedWith(target) && x.MeleeRange &&
						actor.CanEngage(x)).ToList();
			if (potentialTargets.Any())
			{
				actor.Engage(potentialTargets.GetRandomElement());
			}
		}

		actor.AddEffect(new Rescue(actor, target));
		actor.OutputHandler.Handle(
			new EmoteOutput(new Emote("@ begin|begins to attempt to rescue $0 from combat!", actor, target)));
	}

	[PlayerCommand("Guard", "guard")]
	[HelpInfo("guard", @"The #3Guard#0 command is used for a wide variety of effects that are about keeping something or someone safe.

You can guard #6people#0, #6things#0 and #6exits#0. For all of these scenarios, you can #3guard clear#0 to stop guarding.

#6Guarding People#0

When you guard a person, it means you will automatically use the #3rescue#0 command on them if they are engaged in melee combat. You can guard more than one person at once. 

The syntax for using this version is #6guard <target>#0 where the target is a character. If you target the same person again, it will remove them from the list of people you are guarding.

You can also #3guard interpose <target>#0 as an alias for the #3interpose#0 command.

#6Guarding Items#0

When you guard an item in a room, you will not allow people to use, get or otherwise interact with the item. The syntax for this is #3guard <item>#0.

You can also #3guard <item> vicinity#0 to additionally protect everything in the vicinity of the item (i.e. with a position emote targeting the item)

Anyone who you have marked as an #6ally#0 or #6trusted ally#0 will not be affected by your guarding of the item.

#6Guarding Exits#0

When you guard an exit, you prevent people from moving through that exit. You can also set exceptions that you will allow to pass, and forbid specific people who are listed as an exception.

The syntax for this version of the command is #3guard <direction>#0.

The syntax to set up exceptions is as follows:

	#3guard permit allies#0 - permit all allies
	#3guard permit <target>#0 - permit someone present with you
	#3guard permit *<dub>#0 - permit someone from your dubs list

Similarly to reverse the above you can use the following:

	#3guard forbid allies#0 - remove the permission for allies
	#3guard forbid <target>#0 - remove a particular target
	#3guard forbid *<dub>#0 - remove someone by dubs", AutoHelp.HelpArg)]
	protected static void Guard(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		// Guard with no argument
		if (ss.IsFinished)
		{
			var guardCharacter = actor.EffectsOfType<IGuardCharacterEffect>().FirstOrDefault();
			if (guardCharacter != null)
			{
				actor.Send(
					$"You are guarding {guardCharacter.Targets.Select(x => x.HowSeen(actor)).ListToString()}, and {(guardCharacter.Interdicting ? "are" : "are not")} interposing yourself for ranged attacks.");
				return;
			}

			var guardItem = actor.EffectsOfType<IGuardItemEffect>().FirstOrDefault();
			if (guardItem != null)
			{
				actor.Send(
					$"You are guarding {guardItem.TargetItem.HowSeen(actor)}{(guardItem.IncludeVicinity ? " and everything in its vicinity" : "")}.");
				return;
			}

			var guardExit = actor.EffectsOfType<IGuardExitEffect>().FirstOrDefault();
			if (guardExit != null)
			{
				var sb = new StringBuilder();
				sb.AppendLine($"You {guardExit.SuffixFor(actor)}.");
				if (guardExit.PermitAllies)
				{
					sb.AppendLine("You will permit your allies to pass.");
				}

				if (guardExit.Exemptions.Any())
				{
					sb.AppendLine("Exemptions:");
					foreach (var exemption in guardExit.Exemptions)
					{
						sb.AppendLine($"\t{exemption.Description}");
					}
				}

				actor.Send(sb.ToString());
				return;
			}

			actor.Send("You are not current guarding anyone or anything.");
			return;
		}

		// Clearing Guard
		var targetText = ss.Pop();
		if (targetText.EqualTo("clear") || targetText.EqualTo("none") || targetText.EqualTo("off") ||
			targetText.EqualTo("self") || targetText.EqualTo("me"))
		{
			var guardCharacter = actor.EffectsOfType<IGuardCharacterEffect>().FirstOrDefault();
			if (guardCharacter != null)
			{
				actor.Send($"You stop guarding anyone.");
				actor.RemoveEffect(guardCharacter, true);
				return;
			}

			var guardItem = actor.EffectsOfType<IGuardItemEffect>().FirstOrDefault();
			if (guardItem != null)
			{
				actor.RemoveEffect(guardItem, true);
				return;
			}

			var guardExit = actor.EffectsOfType<IGuardExitEffect>().FirstOrDefault();
			if (guardExit != null)
			{
				actor.RemoveEffect(guardExit, true);
				return;
			}

			actor.Send("You are not current guarding anyone or anything.");
			return;
		}

		// Toggling ranged interdiction
		if (targetText.EqualToAny("interdict", "interpose", "shield"))
		{
			var guardCharacter = actor.EffectsOfType<IGuardCharacterEffect>().FirstOrDefault();
			if (guardCharacter == null)
			{
				actor.OutputHandler.Send(
					"You are not guarding anyone - you must first guard them before you can dictate interposition.");
				return;
			}

			guardCharacter.Interdicting = !guardCharacter.Interdicting;
			actor.OutputHandler.Send(
				$"You will {(guardCharacter.Interdicting ? "now" : "no longer")} interpose yourself between your guard targets and their ranged attackers.");
			return;
		}

		// Permitting people to pass by guarded exits
		var guardExitEffect = actor.EffectsOfType<IGuardExitEffect>().FirstOrDefault();
		if (targetText.EqualTo("permit"))
		{
			if (guardExitEffect == null)
			{
				actor.Send("You are not guarding any exits at the moment, and so permissions for them are irrelevant.");
				return;
			}

			if (ss.IsFinished)
			{
				actor.Send(
					"Who do you wish to permit to pass? You may use ALLIES to allow all allies, specify a target character that you can see or use the syntax *DUB (e.g. *2.bob) to target someone from your dubs list.");
				return;
			}

			if (ss.SafeRemainingArgument.EqualTo("allies"))
			{
				guardExitEffect.PermitAllies = true;
				actor.Send(
					"You will not stop any of your allies, or anyone travelling with them from traversing that exit.");
				return;
			}

			if (ss.SafeRemainingArgument[0] == '*')
			{
				var dubText = ss.SafeRemainingArgument.Substring(1);
				if (string.IsNullOrEmpty(dubText))
				{
					actor.Send("You must specify a dub to permit.");
					return;
				}

				var targetDub = actor.Dubs.GetFromItemListByKeyword(dubText, actor);
				if (targetDub == null)
				{
					actor.Send("You don't have anyone so dubbed that you can permit.");
					return;
				}

				if (!targetDub.TargetType.EqualTo("Character"))
				{
					actor.Send("You can only use dubs for players with this command.");
					return;
				}

				guardExitEffect.Exempt(targetDub.TargetId, targetDub.LastDescription);
				actor.Send(
					$"You will not stop {targetDub.LastDescription.ColourCharacter()} ({targetDub.Keywords.Select(x => x.Colour(Telnet.BoldWhite)).ListToString()}), or anyone travelling with them from traversing that exit.");
				targetDub.LastUsage = DateTime.UtcNow;
				targetDub.Changed = true;
				return;
			}

			var targetCharacter = actor.TargetActor(ss.SafeRemainingArgument);
			if (targetCharacter == null)
			{
				actor.Send("You don't see anyone like that to permit to pass.");
				return;
			}

			if (targetCharacter.IsSelf(actor))
			{
				actor.Send("You can't target yourself with that command.");
				return;
			}

			guardExitEffect.Exempt(targetCharacter);
			actor.Send(
				$"You will not stop {targetCharacter.HowSeen(actor)}, or anyone travelling with them from traversing that exit.");
			return;
		}

		// Forbidding people from passing by guarded exits
		if (targetText.EqualTo("forbid"))
		{
			if (guardExitEffect == null)
			{
				actor.Send("You are not guarding any exits at the moment, and so permissions for them are irrelevant.");
				return;
			}

			if (ss.IsFinished)
			{
				actor.Send(
					"Who do you wish to forbid to pass? You may use ALLIES to remove all allies, specify a target character that you can see or use the syntax *DUB (e.g. *2.bob) to target someone from your dubs list.");
				return;
			}

			if (ss.SafeRemainingArgument.EqualTo("allies"))
			{
				guardExitEffect.PermitAllies = false;
				actor.Send(
					"Your allies will now follow the same rules as everyone else when it comes to traversing that exit.");
				return;
			}

			if (ss.SafeRemainingArgument[0] == '*')
			{
				var dubText = ss.SafeRemainingArgument.Substring(1);
				if (string.IsNullOrEmpty(dubText))
				{
					actor.Send("You must specify a dub to forbid.");
					return;
				}

				var targetDub = actor.Dubs.GetFromItemListByKeyword(dubText, actor);
				if (targetDub == null)
				{
					actor.Send("You don't have anyone so dubbed that you can forbid.");
					return;
				}

				if (!targetDub.TargetType.EqualTo("Character"))
				{
					actor.Send("You can only use dubs for players with this command.");
					return;
				}

				guardExitEffect.RemoveExemption(targetDub.TargetId);
				actor.Send(
					$"You will no longer ignore {targetDub.LastDescription.ColourCharacter()} ({targetDub.Keywords.Select(x => x.Colour(Telnet.BoldWhite)).ListToString()}) when they traverse that exit.");
				targetDub.LastUsage = DateTime.UtcNow;
				targetDub.Changed = true;
				return;
			}

			var targetCharacter = actor.TargetActor(ss.SafeRemainingArgument);
			if (targetCharacter == null)
			{
				guardExitEffect.RemoveExemption(targetText);
				return;
			}

			if (targetCharacter.IsSelf(actor))
			{
				actor.Send("You can't target yourself with that command.");
				return;
			}

			guardExitEffect.RemoveExemption(targetCharacter);
			actor.Send($"You will no longer ignore {targetCharacter.HowSeen(actor)} when they traverse that exit.");
			return;
		}

		// Guard Exit
		var targetDirection = actor.Location.GetExitKeyword(targetText, actor);
		if (targetDirection != null)
		{
			if (guardExitEffect?.Exit == targetDirection)
			{
				actor.Send(
					"You are already guarding that exit. Use the syntax GUARD NONE to stop guarding it, or GUARD PERMIT/FORBID for permissions.");
				return;
			}

			guardExitEffect = new GuardingExit(actor, targetDirection, false);
			actor.AddEffect(guardExitEffect);
			actor.RemoveAllEffects(
				x => x.GetSubtype<IAffectedByChangeInGuarding>()?.ShouldRemove(guardExitEffect) == true, true);
			actor.OutputHandler.Handle(new EmoteOutput(new Emote(
				$"@ are|is now guarding the exit to {targetDirection.OutboundDirectionDescription}.", actor)));
			return;
		}

		// Other types of guard
		var target = actor.TargetLocal(targetText);
		if (target == null)
		{
			actor.Send("You don't see anything or anyone like that to guard.");
			return;
		}

		// Guard Character
		if (target is ICharacter targetActor)
		{
			var guardEffect = targetActor.EffectsOfType<IGuardCharacterEffect>().FirstOrDefault();
			if (guardEffect == null)
			{
				guardEffect = new GuardCharacter(actor, targetActor);
				actor.AddEffect(guardEffect);
				actor.RemoveAllEffects(
					x => x.GetSubtype<IAffectedByChangeInGuarding>()?.ShouldRemove(guardEffect) == true, true);
			}
			else
			{
				if (guardEffect.Targets.Contains(targetActor))
				{
					guardEffect.RemoveTarget(targetActor);
					actor.Send("You are no longer guarding {0}.", targetActor.HowSeen(actor));
					return;
				}

				guardEffect.AddTarget(targetActor);
			}

			actor.Send("You are now guarding {0}.", targetActor.HowSeen(actor));
			return;
		}

		// Guard Item
		if (target is IGameItem targetItem)
		{
			var guardItem = actor.EffectsOfType<IGuardItemEffect>().FirstOrDefault();
			if (guardItem != null)
			{
				actor.RemoveEffect(guardItem, true);
				if (guardItem.TargetItem == targetItem)
				{
					return;
				}
			}

			guardItem = new GuardItem(actor, targetItem, ss.Peek().EqualTo("vicinity"));
			actor.AddEffect(guardItem);
			actor.RemoveAllEffects(x => x.GetSubtype<IAffectedByChangeInGuarding>()?.ShouldRemove(guardItem) == true,
				true);
			actor.OutputHandler.Handle(
				new EmoteOutput(
					new Emote(
						$"@ begin|begins to guard $0{(guardItem.IncludeVicinity ? " and everything in its vicinity" : "")}.",
						actor, guardItem.TargetItem)));
		}
	}

	[PlayerCommand("Interpose", "interpose")]
	[RequiredCharacterState(CharacterState.Able)]
	[NoMeleeCombatCommand]
	[NoHideCommand]
	[HelpInfo("interpose",
		"This command allows you to physically place yourself between a target and their ranged attackers, so that you are more likely to get hit than they. The syntax is #3INTERPOSE <target>#0, or #3INTERPOSE STOP#0 to stop. You can interpose more than one target at a time if they are substantially smaller than you.",
		AutoHelp.HelpArgOrNoArg)]
	[DelayBlock("general", "You must first stop {0} before you can fight.")]
	protected static void Interpose(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.Peek().EqualToAny("none", "cancel", "off", "stop"))
		{
			actor.RemoveAllEffects(x => x.IsEffectType<PassiveInterdiction.PassivelyInterceding>(), true);
			actor.OutputHandler.Handle(
				new EmoteOutput(new Emote("@ are|is no longer interposing in anyone's ranged attacks.", actor)));
			return;
		}

		var target = actor.TargetLocal(ss.PopSpeech());
		if (target == null)
		{
			actor.OutputHandler.Send("You don't see anything like that here.");
			return;
		}

		if (target == actor)
		{
			actor.OutputHandler.Send("You cannot interpose with yourself.");
			return;
		}

		if (actor.EffectsOfType<PassiveInterdiction.PassivelyInterceding>().Any(x => x.Target == target))
		{
			actor.RemoveEffect(
				actor.EffectsOfType<PassiveInterdiction.PassivelyInterceding>().First(x => x.Target == target), true);
			actor.OutputHandler.Handle(new EmoteOutput(new Emote(
				"@ are|is no longer interposing between ranged attackers who fire at $1.", actor, actor, target)));
			return;
		}

		if (!PassiveInterdiction.PassivelyInterceding.CanInterpose(actor, target))
		{
			actor.OutputHandler.Send(
				$"You are already interposing with too many or too large targets to be able to effectively shield {target.HowSeen(actor)} from ranged attack.");
			return;
		}

		actor.OutputHandler.Handle(new EmoteOutput(
			new Emote("@ begin|begins interposing %0 between ranged attackers who fire at $1.", actor, actor, target)));
		actor.AddEffect(new PassiveInterdiction.PassivelyInterceding(actor, target));
	}

	[PlayerCommand("Spar", "spar")]
	[RequiredCharacterState(CharacterState.Able)]
	[CommandPermission(PermissionLevel.Any)]
	[DelayBlock("general", "You must first stop {0} before you can fight.")]
	[HelpInfo("spar", @"The #3Spar#0 command is used to propose a ""friendly"" combat with another target. 

The target has to accept your proposal for the fight to begin (some NPC AIs know how and when to accept these too). If the target accepts, it is otherwise as if the combat had begun with the #3hit#0 command.

Friendly combats end automatically if anyone is rendered unconscious or helpless, or if anyone requests it. You will also release grapples at the conclusion of a friendly fight.

See also the #3hit#0, #3support#0 and #3aim#0 commands.

The syntax for this command is #3spar <target>#0.", AutoHelp.HelpArgOrNoArg)]
	protected static void Spar(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.Send("Who or what do you want to spar?");
			return;
		}

		var target = actor.Target(ss.Pop());
		if (target == null)
		{
			actor.Send("You don't see anyone or anything like that to spar.");
			return;
		}

		if (target == actor)
		{
			actor.Send("You cannot engage yourself in a spar.");
			return;
		}

		if (target is not IPerceiver targetAsPerceiver)
		{
			actor.Send("{0} is not something that can be engaged in combat.", target.HowSeen(actor, true));
			return;
		}

		if (targetAsPerceiver.Combat != null)
		{
			actor.Send("{0} is already engaged in combat and so cannot join a sparring bout.",
				target.HowSeen(actor, true));
			return;
		}

		if (!actor.Combat?.Friendly ?? false)
		{
			actor.Send("You cannot begin a sparring bout while you are engaged in real combat!");
			return;
		}

		targetAsPerceiver.AddEffect(
			new Accept(targetAsPerceiver, new SparInvitationProposal(actor, targetAsPerceiver, actor.Combat)),
			TimeSpan.FromSeconds(120));
		if (actor.Combat == null)
		{
			actor.OutputHandler.Handle(
				new EmoteOutput(new Emote("@ invite|invites $1 to commence a sparring bout with &0.", actor, actor,
					targetAsPerceiver)));
		}
		else
		{
			actor.OutputHandler.Handle(
				new EmoteOutput(new Emote("@ invite|invites $1 to join &0's sparring bout.", actor, actor,
					targetAsPerceiver)));
		}

		if (targetAsPerceiver is ICharacter)
		{
			targetAsPerceiver.HandleEvent(EventType.SparInvitation, actor, targetAsPerceiver);
		}
	}

	[PlayerCommand("Support", "support")]
	[RequiredCharacterState(CharacterState.Able)]
	[CommandPermission(PermissionLevel.Any)]
	[DelayBlock("general", "You must first stop {0} before you can fight.")]
	[HelpInfo("", @"The #3Support#0 command is used to join another character in combat, by joining in to their combat and targeting their current target. 

This command is functionally equivalent to having typed #3hit <target's target>#0. 

See also the #3hit#0, #3spar#0 and #3aim#0 commands for other ways of joining combat.

The syntax for this command is #3support <target>#0.", AutoHelp.HelpArgOrNoArg)]
	protected static void Support(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished || ss.Peek().EqualTo("help") || ss.Peek().EqualTo("?"))
		{
			actor.Send(
				$"The support command is used to join another character in combat, by joining in to their combat and targeting their current target. The syntax is {"support <friend>".Colour(Telnet.Yellow)}.");
			return;
		}

		var target = actor.TargetAlly(ss.Pop()) ?? actor.TargetActor(ss.Last);
		if (target == null)
		{
			actor.Send("You don't see anyone like that to support.");
			return;
		}

		if (target.Combat == null)
		{
			actor.Send("{0} is not currently engaged in combat.", target.HowSeen(actor, true));
			return;
		}

		if (target.CombatTarget == null)
		{
			actor.Send("{0} is not targeting anything or anyone. You should pick a target and HIT them.");
			return;
		}

		if (!actor.CanEngage(target.CombatTarget))
		{
			actor.Send(actor.WhyCannotEngage(target.CombatTarget));
			return;
		}

		actor.Engage(target.CombatTarget);
	}

	[PlayerCommand("Hit", "hit")]
	[RequiredCharacterState(CharacterState.Able)]
	[CommandPermission(PermissionLevel.Any)]
	[DelayBlock("general", "aim", "You must first stop {0} before you can fight.")]
	[HelpInfo("hit", @"The #3Hit#0 command is used to begin combat with a target.

If you begin combat with this command and both you and the target have a ranged combat setting that prefers melee combat, you will begin engaged in melee combat. If the target doesn't want to be in melee though you will begin combat outside of melee range.

There are other ways to begin combat; see also #3support#0, #3aim#0 and #3spar#0.

The syntax for this command is #3hit <target>#0.", AutoHelp.HelpArgOrNoArg)]
	protected static void Hit(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.Send("Who or what do you want to hit?");
			return;
		}

		var target = actor.TargetActor(ss.SafeRemainingArgument);
		if (target == null)
		{
			actor.Send("You don't see anyone or anything like that to engage.");
			return;
		}

		if (target == actor)
		{
			actor.Send("You cannot engage yourself in combat.");
			return;
		}

		if (!actor.CanEngage(target))
		{
			actor.Send(actor.WhyCannotEngage(target));
			return;
		}

		if (CrimeExtensions.HandleCrimesAndLawfulActing(actor, CrimeTypes.Assault, target))
		{
			return;
		}

		actor.Engage(target);
	}

	private const string CombatCommandHelpText = @"#6Combat Command Help#0

The combat command is used to view, manage and adopt various settings that control how your character will act in combat. 

Most important of these are #3combat settings#0, which are pre-defined sets of rules about what your character will do in combat. 
There are a number of global templates for these that cover a variety of situations but you can create and customise your own once you feel comfortable doing so.

The syntax to use this command is as follows:

	#3combat help#0 - shows this list
	#3combat status#0 - shows information about your current conflict
	#3combat list#0 - shows available combat settings
	#3combat show <id/name>#0 - shows details of a particular combat setting
	#3combat set <id/name>#0 - adopts a particular combat setting as your current
	#3combat clone <id/name> <newname>#0 - makes a new combat setting out of an existing one for editing
	#3combat defense <block/dodge/parry/none>#0 - sets or clears a favoured defense type

Additionally, there are numerous options for configuring combat settings. See #3combat config help#0 for more information.";

	[PlayerCommand("Combat", "combat")]
	[HelpInfo("combat", CombatCommandHelpText, AutoHelp.HelpArg)]
	protected static void Combat(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			if (actor.Combat == null)
			{
				actor.OutputHandler.Send(CombatCommandHelpText.SubstituteANSIColour());
				return;
			}

			CombatShowCombat(actor, ss);
			return;
		}

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "list":
				CombatList(actor, ss);
				return;
			case "show":
				CombatShow(actor, ss);
				return;
			case "status":
			case "stat":
			case "":
				CombatShowCombat(actor, ss);
				return;
			case "clone":
				CombatClone(actor, ss);
				return;
			case "config":
				CombatConfig(actor, ss);
				return;
			case "set":
				CombatSet(actor, ss);
				return;
			case "defense":
			case "defence":
			case "def":
				CombatPreferredDefense(actor, ss);
				return;
		}

		CombatHelp(actor, ss);
	}

	[PlayerCommand("Peace", "peace")]
	[HelpInfo("Peace", @"The peace command is used to end combats in your current location or to place a room effect that prevents fights from starting.

There are a few different ways that you can use this command:

	#3peace#0 - this ends all combats that have any combatants in this location
	#3peace permanent#0 - places an effect that blocks all combat here
	#3peace permanent <prog>#0 - same as above, but with an optional prog for exceptions
	#3peace off#0 - removes a permanent peace effect

Note: The prog used with #3peace permanent#0 takes the following parameters and returns #6boolean#0:

#6location#0 room, #6perceiver#0 attacker, #6perceiver#0 target", AutoHelp.HelpArg)]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void Peace(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			var combats = actor.Location.Characters.SelectNotNull(x => x.Combat).Distinct().ToList();
			foreach (var combat in combats)
			{
				combat.EndCombat(true);
			}

			actor.Send($"You end {combats.Count:N0} combats in this location.");
			return;
		}

		if (ss.Peek().StartsWith("permanent", StringComparison.InvariantCultureIgnoreCase))
		{
			ss.Pop();
			IFutureProg prog = null;
			if (!ss.IsFinished)
			{
				prog = long.TryParse(ss.PopSpeech(), out var value)
					? actor.Gameworld.FutureProgs.Get(value)
					: actor.Gameworld.FutureProgs.GetByName(ss.Last);

				if (prog == null)
				{
					actor.Send("There is no such prog for you to apply peace with.");
					return;
				}

				if (
					!prog.MatchesParameters(new[]
					{
						FutureProgVariableTypes.Location, FutureProgVariableTypes.Perceiver,
						FutureProgVariableTypes.Perceiver
					}) ||
					!prog.ReturnType.CompatibleWith(FutureProgVariableTypes.Boolean))
				{
					actor.Send(
						"The prog that you specify for permanent peace must accept a location, perceiver and perceiver and return a boolean.");
					return;
				}
			}

			actor.Location.AddEffect(new PeacefulEffect(actor.Location, prog));
			actor.Send(
				$"This location is now peaceful permanently{(prog != null ? $" unless the prog {prog.FunctionName.Colour(Telnet.Green)} #{prog.Id:N0} is passed" : "")}.");
			return;
		}

		if (ss.Peek().Equals("off", StringComparison.InvariantCultureIgnoreCase))
		{
			if (!actor.Location.EffectsOfType<IPeacefulEffect>().Any())
			{
				actor.Send("This location is not peaceful.");
				return;
			}

			actor.Location.RemoveAllEffects(x => x is IPeacefulEffect);
			actor.Send("You remove the restrictions on combat at this location.");
			return;
		}

		actor.Send(
			$"Correct syntax is either {"peace".Colour(Telnet.Yellow)} on its own or {"peace permanent [<prog>]".Colour(Telnet.Yellow)}.");
	}

	[PlayerCommand("Flee", "flee")]
	[RequiredCharacterState(CharacterState.Able)]
	[HelpInfo("flee", @"The #3Flee#0 command is used to toggle your desire to flee the current combat. If you toggle it on, you will try to leave melee and (depending on your settings) potentially flee to other rooms and try to put enough distance between you and your opponent to get out of combat.

The syntax is simply #3flee#0 to toggle it on, and the same again to return to your default combat setting.", AutoHelp.HelpArg)]
	protected static void Flee(ICharacter actor, string command)
	{
		if (actor.Combat == null)
		{
			actor.Send("You are not in combat, and so do not need to flee.");
			return;
		}

		if (actor.CombatStrategyMode == CombatStrategyMode.Flee)
		{
			var mode = actor.MeleeRange
				? actor.CombatSettings.PreferredMeleeMode
				: actor.CombatSettings.PreferredRangedMode;
			if (mode == CombatStrategyMode.Flee)
			{
				mode = actor.MeleeRange ? CombatStrategyMode.StandardMelee : CombatStrategyMode.StandardRange;
			}

			actor.Send("You are no longer attempting to flee.");
			return;
		}

		if (actor.Combat.Friendly)
		{
			actor.Combat.TruceRequested(actor);
			return;
		}

		actor.CombatStrategyMode = CombatStrategyMode.Flee;
		actor.Send("You resolve to flee combat by any means necessary!");
		actor.OutputHandler.Handle(new EmoteOutput(
			new Emote("@ begins looking for a way to escape combat!", actor), flags: OutputFlags.SuppressSource));
	}

	[PlayerCommand("Ward", "ward")]
	[RequiredCharacterState(CharacterState.Able)]
	[HelpInfo("clinch", @"The #3Ward#0 command is used to focus on the ward defense, with means using the threat of counter-attacking with your weapon to keep your opponent at range and also to defend against attacks with that same threat.

If the ward fails, you can still dodge the attack (albeit at a penalty) but you can't parry or block. If the opponent pushes through the ward attempt you will get a free counter-attack against them.

The syntax is simply #3ward#0 to toggle the setting on or off.", AutoHelp.HelpArg)]
	protected static void Ward(ICharacter actor, string command)
	{
		if (actor.Combat == null)
		{
			actor.Send("You are not in combat, and so cannot begin to ward.");
			return;
		}

		if (!actor.MeleeRange)
		{
			actor.OutputHandler.Send("You are not in melee range, so you cannot switch into warding mode.");
			return;
		}

		if (actor.CombatStrategyMode == CombatStrategyMode.Ward)
		{
			if (actor.CombatSettings.PreferredMeleeMode == CombatStrategyMode.FullDefense)
			{
				actor.CombatStrategyMode = CombatStrategyMode.FullDefense;
				actor.OutputHandler.Handle(
					new EmoteOutput(new Emote("@ shift|shifts out of a warding stance but remains on the full defense.", actor,
						actor), flags: OutputFlags.NoticeCheckRequired));
				return;
			}
			actor.CombatStrategyMode = CombatStrategyMode.StandardMelee;
			actor.OutputHandler.Handle(
				new EmoteOutput(new Emote("@ shift|shifts out of a warding stance and will fight normally.", actor,
					actor), flags: OutputFlags.NoticeCheckRequired));
			return;
		}

		actor.CombatStrategyMode = CombatStrategyMode.Ward;
		actor.OutputHandler.Handle(
			new EmoteOutput(
				new Emote(
					"@ shift|shifts into a warding stance, attempting to use &0's superior reach as a defense.",
					actor, actor), flags: OutputFlags.NoticeCheckRequired));
	}

	[PlayerCommand("Clinch", "clinch")]
	[RequiredCharacterState(CharacterState.Able)]
	[HelpInfo("clinch", @"The #3Clinch#0 command is used to begin to try to get very close to your melee target. Some weapons have special attacks at this close range that often are difficult or impossible to defend against. However, there are also special attacks others can use to break out of clinches, so it isn't without risk.

The syntax is simply #3clinch#0 to toggle the setting on or off.", AutoHelp.HelpArg)]
	protected static void Clinch(ICharacter actor, string command)
	{
		if (actor.Combat == null)
		{
			actor.Send("You are not in combat, and so cannot begin to clinch.");
			return;
		}

		if (!actor.MeleeRange)
		{
			actor.OutputHandler.Send("You are not in melee range, so you cannot switch into clinching mode.");
			return;
		}

		if (actor.CombatStrategyMode == CombatStrategyMode.Clinch)
		{
			if (actor.CombatSettings.PreferredMeleeMode == CombatStrategyMode.FullDefense)
			{
				actor.CombatStrategyMode = CombatStrategyMode.FullDefense;
				actor.OutputHandler.Handle(
					new EmoteOutput(
						new Emote(
							"@ shift|shifts out of &0's aggressive stance, and returns to a stance of full defense.",
							actor, actor), flags: OutputFlags.NoticeCheckRequired));
				return;
			}
			actor.CombatStrategyMode = CombatStrategyMode.StandardMelee;
			actor.OutputHandler.Handle(
				new EmoteOutput(
					new Emote(
						"@ shift|shifts out of &0's aggressive stance, no longer attempting to get up close and personal.",
						actor, actor), flags: OutputFlags.NoticeCheckRequired));
			return;
		}

		actor.CombatStrategyMode = CombatStrategyMode.Clinch;
		actor.OutputHandler.Handle(
			new EmoteOutput(
				new Emote("@ shift|shifts into an aggressive stance, attempting to get up close and personal.",
					actor, actor), flags: OutputFlags.NoticeCheckRequired));
	}

	[PlayerCommand("Defend", "defend")]
	[RequiredCharacterState(CharacterState.Able)]
	[HelpInfo("defend", @"The #3Defend#0 command is used to toggle your melee mode from one of full and total defense (you will get a bonus to defend but will not attack at all) back to a standard melee strategy (where you will attack normally but receive no bonuses).

The syntax is simply #3defend#0 to toggle the setting on or off.", AutoHelp.HelpArg)]
	protected static void Defend(ICharacter actor, string command)
	{
		if (actor.Combat == null)
		{
			actor.Send("You are not in combat, and so cannot begin to fight defensively.");
			return;
		}

		if (!actor.MeleeRange)
		{
			actor.OutputHandler.Send("You are not in melee range, so you cannot begin to fight defensively.");
			return;
		}

		if (actor.CombatStrategyMode == CombatStrategyMode.FullDefense)
		{
			actor.CombatStrategyMode = CombatStrategyMode.StandardMelee;
			actor.OutputHandler.Handle(
				new EmoteOutput(new Emote("@ stop|stops fighting exclusively in defense and begin|begins looking for opportunities to attack.", actor,
					actor), flags: OutputFlags.NoticeCheckRequired));
			return;
		}

		actor.CombatStrategyMode = CombatStrategyMode.FullDefense;
		actor.OutputHandler.Handle(
			new EmoteOutput(
				new Emote(
					"@ begin|begins fighting exclusively in their own defense.",
					actor, actor), flags: OutputFlags.NoticeCheckRequired));
	}

	[PlayerCommand("Load", "load")]
	[RequiredCharacterState(CharacterState.Able)]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	protected static void Load(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		IGameItem target = null;
		target = ss.IsFinished
			? actor.Body.HeldOrWieldedItems
				   .SelectNotNull(x => x.GetItemType<IRangedWeapon>())
				   .FirstOrDefault(x => x.CanLoad(actor, true))?.Parent
			: actor.TargetHeldItem(ss.PopSpeech());

		if (target == null)
		{
			actor.Send(ss.IsFinished
				? "You don't have anything that can be loaded."
				: "There is nothing like that for you to load.");
			return;
		}

		var targetWeapon = target.GetItemType<IRangedWeapon>();
		if (targetWeapon == null)
		{
			actor.Send($"{target.HowSeen(actor, true)} is not something that can be loaded.");
			return;
		}

		if (!targetWeapon.CanLoad(actor))
		{
			actor.Send(targetWeapon.WhyCannotLoad(actor));
			return;
		}

		if (actor.Combat != null)
		{
			if (actor.TakeOrQueueCombatAction(SelectedCombatAction.GetEffectLoadItem(actor, targetWeapon)) &&
				actor.Gameworld.GetStaticBool("EchoQueuedActions"))
			{
				actor.Send($"{"[Queued Action]: ".ColourBold(Telnet.Yellow)}Loading {target.HowSeen(actor)}.");
			}

			return;
		}

		targetWeapon.Load(actor);
	}

	[PlayerCommand("Unload", "unload")]
	[RequiredCharacterState(CharacterState.Able)]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	protected static void Unload(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		IGameItem target = null;
		target = ss.IsFinished
			? actor.Body.HeldOrWieldedItems
				   .SelectNotNull(x => x.GetItemType<IRangedWeapon>())
				   .FirstOrDefault(x => x.CanUnload(actor))?.Parent
			: actor.TargetLocalOrHeldItem(ss.PopSpeech());

		if (target == null)
		{
			actor.Send(ss.IsFinished
				? "You don't have anything that can be unloaded."
				: "There is nothing like that for you to unload.");
			return;
		}

		var targetWeapon = target.GetItemType<IRangedWeapon>();
		if (targetWeapon == null)
		{
			actor.Send($"{target.HowSeen(actor, true)} is not something that can be unloaded.");
			return;
		}

		if (!targetWeapon.CanUnload(actor))
		{
			actor.Send(targetWeapon.WhyCannotUnload(actor));
			return;
		}

		targetWeapon.Unload(actor);
	}

	[PlayerCommand("Ready", "ready")]
	[RequiredCharacterState(CharacterState.Able)]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	protected static void Ready(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		IGameItem target = null;
		target = ss.IsFinished
			? actor.Body.HeldOrWieldedItems
				   .SelectNotNull(x => x.GetItemType<IRangedWeapon>())
				   .FirstOrDefault(x => x.CanReady(actor))?.Parent
			: actor.TargetLocalOrHeldItem(ss.PopSpeech());

		if (target == null)
		{
			actor.Send(ss.IsFinished
				? "You don't have anything that can be readied."
				: "There is nothing like that for you to ready.");
			return;
		}

		var targetWeapon = target.GetItemType<IRangedWeapon>();
		if (targetWeapon == null)
		{
			actor.Send($"{target.HowSeen(actor, true)} is not something that can be readied.");
			return;
		}

		if (!targetWeapon.CanReady(actor))
		{
			actor.Send(targetWeapon.WhyCannotReady(actor));
			return;
		}

		if (actor.Combat != null)
		{
			if (actor.TakeOrQueueCombatAction(SelectedCombatAction.GetEffectReadyItem(actor, targetWeapon)) &&
				actor.Gameworld.GetStaticBool("EchoQueuedActions"))
			{
				actor.Send($"{"[Queued Action]: ".ColourBold(Telnet.Yellow)}Readying {target.HowSeen(actor)}.");
			}

			return;
		}

		targetWeapon.Ready(actor);
	}

	[PlayerCommand("Unready", "unready")]
	[RequiredCharacterState(CharacterState.Able)]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	protected static void Unready(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		IGameItem target = null;
		target = ss.IsFinished
			? actor.Body.HeldOrWieldedItems
				   .SelectNotNull(x => x.GetItemType<IRangedWeapon>())
				   .FirstOrDefault(x => x.CanUnready(actor))?.Parent
			: actor.TargetLocalOrHeldItem(ss.PopSpeech());

		if (target == null)
		{
			actor.Send(ss.IsFinished
				? "You don't have anything that can be unreadied."
				: "There is nothing like that for you to unready.");
			return;
		}

		var targetWeapon = target.GetItemType<IRangedWeapon>();
		if (targetWeapon == null)
		{
			actor.Send($"{target.HowSeen(actor, true)} is not something that can be unreadied.");
			return;
		}

		if (!targetWeapon.CanUnready(actor))
		{
			actor.Send(targetWeapon.WhyCannotUnready(actor));
			return;
		}

		targetWeapon.Unready(actor);
	}

	[PlayerCommand("Aim", "aim")]
	[RequiredCharacterState(CharacterState.Able)]
	[NoMovementCommand]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	protected static void Aim(ICharacter actor, string command)
	{
		var weapons =
			actor.Body.WieldedItems.SelectNotNull(x => x.GetItemType<IRangedWeapon>())
				 .Where(x => x.ReadyToFire || x.WeaponType.RangedWeaponType.IsFirearm())
				 .ToList();
		if (!weapons.Any())
		{
			actor.Send("You aren't wielding any ready ranged weapons that you can aim at anybody.");
			return;
		}

		var ss = new StringStack(command.RemoveFirstWord());

		if (actor.Combat != null && actor.IsEngagedInMelee && weapons.All(x => !x.WeaponType.FireableInMelee))
		{
			actor.Send("You can't focus on aiming at things when you are in melee combat! Try to HIT your target.");
			return;
		}

		var blockingEffect =
			actor.Effects.Where(x => !x.IsEffectType<OutOfCombatAim>())
				 .FirstOrDefault(x => x.IsBlockingEffect("general"));
		if (blockingEffect != null)
		{
			actor.Send(
				$"You have to stop {blockingEffect.BlockingDescription("general", actor)} before you can aim at anything.");
			return;
		}

		IPerceiver target = null;
		var path = Enumerable.Empty<ICellExit>();
		IRangedWeapon weapon = null;
		if (ss.IsFinished)
		{
			if (!actor.MeleeRange || actor.CombatTarget == null)
			{
				actor.Send("Who or what would you like to aim at?");
				return;
			}

			target = actor.CombatTarget;
			weapon = weapons.First();
		}
		else
		{
			var targetText = ss.PopSpeech();

			if (targetText.EqualToAny("me", "self") && actor.Combat == null)
			{
				if (ss.IsFinished)
				{
					actor.Send("What bodypart of yours do you want to aim at?");
					return;
				}

				var bodypart = actor.Body.GetTargetBodypart(ss.PopSpeech());
				if (bodypart == null)
				{
					actor.Send("You don't have any such bodypart to aim at.");
					return;
				}

				weapon = weapons.First();

				if (!weapon.CanBeAimedAtSelf)
				{
					actor.Send(
						$"{weapon.Parent.HowSeen(actor, true)} is not the kind of weapon that can be aimed at your own body.");
					return;
				}

				actor.TargettedBodypart = bodypart;
				actor.RemoveAllEffects(x => x.IsEffectType<OutOfCombatAim>(), true);
				var aimEffect = new OutOfCombatAim(actor, actor, weapon, Enumerable.Empty<ICellExit>());
				actor.AddEffect(aimEffect, TimeSpan.FromSeconds(10));
				aimEffect.AimCompletion = 1.0;
				actor.OutputHandler.Handle(
					new EmoteOutput(
						new Emote($"@ begin|begins to aim $2 at &0's own {bodypart.FullDescription()}.", actor, actor,
							target, weapon.Parent),
						flags: OutputFlags.SuppressObscured | OutputFlags.InnerWrap));
				return;
			}

			if (ss.IsFinished)
			{
				weapon = weapons.First();
			}
			else
			{
				weapon =
					weapons.Select(x => x.Parent)
						   .GetFromItemListByKeyword(targetText, actor)?.GetItemType<IRangedWeapon>();
				if (weapon == null)
				{
					actor.OutputHandler.Send("You do not have any weapon like that to aim.");
					return;
				}

				targetText = ss.PopSpeech();
			}

			if (targetText.EqualToAny("sky", "air"))
			{
				if (actor.Combat == null)
				{
					actor.RemoveAllEffects(x => x.IsEffectType<OutOfCombatAim>(), true);
					actor.AddEffect(new OutOfCombatAim(actor, null, weapon, path));
					actor.OutputHandler.Handle(new EmoteOutput(
						new Emote("@ begin|begins to aim $1 straight up in the air.", actor, actor, weapon.Parent),
						flags: OutputFlags.SuppressObscured | OutputFlags.InnerWrap));
				}
				else
				{
					if (actor.TakeOrQueueCombatAction(SelectedCombatAction.GetEffectAimItem(actor, null, weapon)) &&
						actor.Gameworld.GetStaticBool("EchoQueuedActions"))
					{
						actor.Send(
							$"{"[Queued Action]: ".ColourBold(Telnet.Yellow)}Aiming {weapon.Parent.HowSeen(actor)} into the air.");
					}
				}

				return;
			}

			var targets = actor.Location.LayerCharacters(actor.RoomLayer).Except(actor)
							   .Concat<IMortalPerceiver>(actor.Location.LayerGameItems(actor.RoomLayer))
							   .Where(x => actor.CanSee(x))
							   .Concat(actor.SeenTargets).ToList();
			target = targets.GetFromItemListByKeyword(targetText, actor);
			if (target == null)
			{
				actor.OutputHandler.Send("You haven't seen anything like that to aim your weapon at.");
				return;
			}

			path = actor.PathBetween(target, weapon.WeaponType.DefaultRangeInRooms,
				x => x.Exit.Door?.CanFireThrough != false);
			if (actor.Location != target.Location && !path.Any())
			{
				actor.OutputHandler.Send($"You're not in range of {target.HowSeen(actor)}.");
				return;
			}
		}

		if (actor.CombatTarget != target && !actor.CanEngage(target))
		{
			actor.Send(actor.WhyCannotEngage(target));
			return;
		}

		if (actor.Combat == null || !(target is ICharacter))
		{
			actor.RemoveAllEffects(x => x.IsEffectType<OutOfCombatAim>(), true);
			actor.AddEffect(new OutOfCombatAim(actor, target, weapon, path), TimeSpan.FromSeconds(10));
			actor.OutputHandler.Handle(
				new EmoteOutput(new Emote("@ begin|begins to aim $2 at $1.", actor, actor, target, weapon.Parent),
					flags: OutputFlags.SuppressObscured | OutputFlags.InnerWrap));
			if (target is ICharacter && !actor.CombatSettings.PreferredRangedMode.IsRangedStartDesiringStrategy())
			{
				actor.Send(
					"Warning: Your current combat settings are not range-friendly settings, you will charge into melee when you fire."
						.Colour(Telnet.Yellow));
			}

			return;
		}

		if (actor.TakeOrQueueCombatAction(SelectedCombatAction.GetEffectAimItem(actor, target, weapon)) &&
			actor.Gameworld.GetStaticBool("EchoQueuedActions"))
		{
			actor.Send(
				$"{"[Queued Action]: ".ColourBold(Telnet.Yellow)}Aiming {weapon.Parent.HowSeen(actor)} at {target.HowSeen(actor)}.");
		}
		else
		{
			actor.Engage(target, true);
		}
	}

	[PlayerCommand("Fire", "fire")]
	[RequiredCharacterState(CharacterState.Able)]
	[NoMovementCommand]
	[DelayBlock("general", "aim", "You must first stop {0} before you can do that.")]
	protected static void Fire(ICharacter actor, string command)
	{
		if (actor.Aim != null && actor.Combat != null)
		{
			if (actor.TakeOrQueueCombatAction(
					SelectedCombatAction.GetEffectFireItem(actor, actor.Aim.Target, actor.Aim.Weapon)) &&
				actor.Gameworld.GetStaticBool("EchoQueuedActions"))
			{
				actor.Send(
					$"{"[Queued Action]: ".ColourBold(Telnet.Yellow)}Firing {actor.Aim.Weapon.Parent.HowSeen(actor)} at {actor.Aim.Target?.HowSeen(actor) ?? "the sky"}.");
			}

			return;
		}

		var aiming = actor.EffectsOfType<OutOfCombatAim>().FirstOrDefault();
		if (aiming == null)
		{
			actor.Send("You need to aim at something first before you can fire.");
			return;
		}

		if (aiming.Weapon.WeaponType.StaminaToFire > 0 &&
			!actor.CanSpendStamina(aiming.Weapon.WeaponType.StaminaToFire))
		{
			actor.Send($"You are too exhausted to fire {aiming.Weapon.Parent.HowSeen(actor)}.");
			return;
		}

		if (!aiming.Weapon.CanFire(actor, aiming.Target))
		{
			actor.Send(aiming.Weapon.WhyCannotFire(actor, aiming.Target));
			return;
		}

		if (aiming.Aim.Target == null)
		{
			if (actor.Combat == null)
			{
				aiming.Aim.Weapon.Fire(actor, null, Outcome.NotTested, Outcome.NotTested,
					new OpposedOutcome(Outcome.NotTested, Outcome.NotTested), null, null, null);
				return;
			}

			actor.Aim = aiming.Aim;
			actor.Combat?.CombatAction(actor, new RangedWeaponAttackMove(actor, null, aiming.Weapon));
			return;
		}

		if (aiming.Aim.Target is ICharacter)
		{
			if (aiming.Aim.Target == actor)
			{
				aiming.Weapon.Fire(actor, actor, Outcome.Pass, Outcome.Pass,
					new OpposedOutcome(OpposedOutcomeDirection.Proponent, OpposedOutcomeDegree.Total),
					actor.TargettedBodypart ?? actor.Body.RandomBodypart, null, actor);
				return;
			}

			if (CrimeExtensions.HandleCrimesAndLawfulActing(actor, CrimeTypes.AssaultWithADeadlyWeapon,
					(ICharacter)aiming.Aim.Target))
			{
				return;
			}

			if (actor.Combat == null)
			{
				actor.Engage(aiming.Aim.Target, true);
				actor.Aim = aiming.Aim;
				actor.Combat?.CombatAction(
					actor, new RangedWeaponAttackMove(actor, actor.Aim.Target, aiming.Weapon));
				return;
			}

			if (actor.TakeOrQueueCombatAction(
					SelectedCombatAction.GetEffectFireItem(actor, actor.Aim.Target, aiming.Weapon)) &&
				actor.Gameworld.GetStaticBool("EchoQueuedActions"))
			{
				actor.Send(
					$"{"[Queued Action]: ".ColourBold(Telnet.Yellow)}Firing {actor.Aim.Weapon.Parent.HowSeen(actor)} at {actor.Aim.Target.HowSeen(actor)}.");
			}

			return;
		}

		actor.Aim = aiming.Aim;
		var attack = new RangedWeaponAttackMove(actor, aiming.Target, aiming.Weapon);
		actor.SpendStamina(attack.StaminaCost);
		attack.ResolveMove(null);
		if (aiming.Aim != null && aiming.Aim.Weapon.IsReadied)
		{
			//If we want to resume aiming, set it up here, for now resumed the line since it was causing crashes and lingering aiminfo.
			//aiming.Aim = new AimInformation(aiming.Target, actor, aiming.Aim.Path, aiming.Weapon) { AimPercentage = aiming.Aim.AimPercentage - aiming.Weapon.WeaponType.AimBonusLostPerShot };
		}

		actor.Aim = null;
	}

	[PlayerCommand("Cover", "cover")]
	[NoMovementCommand]
	[RequiredCharacterState(CharacterState.Able)]
	[DelayBlock("general", "aim", "You must first stop {0} before you can do that.")]
	protected static void Cover(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.Peek().Equals("?") || ss.Peek().Equals("help", StringComparison.InvariantCultureIgnoreCase))
		{
			actor.Send(
				$"Use this command to seek cover. Syntax:\n\t{"cover".Colour(Telnet.Yellow)} - seek the best available cover.\n\t{"cover move".Colour(Telnet.Yellow)} - seek the best available cover that permits movement.\n\t{"cover <target>".Colour(Telnet.Yellow)} - take cover in the targeted cover.\n\nUse the syntax {"stand self".Colour(Telnet.Yellow)} to leave cover once in it.");
			return;
		}

		if (!actor.CanMove())
		{
			actor.Send("You need to be able to move to be able to seek cover.");
			return;
		}

		if (ss.IsFinished || ss.Peek().Equals("move", StringComparison.InvariantCultureIgnoreCase))
		{
			var cover = TakeCover.GetCoverFor(actor, !ss.IsFinished);
			if (cover == null)
			{
				actor.Send(
					"Unfortunately there is no suitable cover nearby!");
				return;
			}

			if (actor.Combat != null)
			{
				if (actor.TakeOrQueueCombatAction(SelectedCombatAction.GetEffectTakeCover(actor, cover)) &&
					actor.Gameworld.GetStaticBool("EchoQueuedActions"))
				{
					actor.Send(
						$"{"[Queued Action]: ".ColourBold(Telnet.Yellow)}Seeking cover @ {cover.Cover.Describe(actor, cover.CoverItem?.Parent, actor)}");
				}

				return;
			}

			actor.Cover?.LeaveCover();
			actor.OutputHandler.Handle(new EmoteOutput(cover.Cover.DescribeAction(actor, cover.CoverItem?.Parent)));
			actor.SetPosition(
				cover.Cover.HighestPositionState.CompareTo(actor.PositionState) == PositionHeightComparison.Lower
					? actor.PositionState = cover.Cover.HighestPositionState
					: actor.PositionState, cover.CoverItem != null ? PositionModifier.Behind : PositionModifier.None,
				cover.CoverItem?.Parent, null);
			actor.Cover = cover;
			cover.RegisterEvents();
			return;
		}

		var targets = new List<IKeywordedItem>();
		targets.AddRange(actor.Location.LayerGameItems(actor.RoomLayer)
							  .Where(x => x.GetItemType<IProvideCover>()?.Cover != null));
		targets.AddRange(actor.Location.GetCoverFor(actor));
		var target = targets.GetFromItemListByKeyword(ss.Pop(), actor);
		if (target == null)
		{
			actor.Send("You don't see any cover like that nearby.");
			return;
		}

		CombatantCover newcover;
		if (target is not IRangedCover targetCover)
		{
			newcover = new CombatantCover(actor, ((IGameItem)target).GetItemType<IProvideCover>().Cover,
				((IGameItem)target).GetItemType<IProvideCover>());
		}
		else
		{
			newcover = new CombatantCover(actor, targetCover, null);
		}

		if (TakeCover.CoverFitness(actor, false, newcover.Cover, newcover.CoverItem) <= 0.0)
		{
			actor.Send(TakeCover.WhyZeroCoverFitness(actor, false, newcover.Cover, newcover.CoverItem));
			return;
		}

		if (actor.Combat != null)
		{
			if (actor.TakeOrQueueCombatAction(SelectedCombatAction.GetEffectTakeCover(actor, newcover)) &&
				actor.Gameworld.GetStaticBool("EchoQueuedActions"))
			{
				actor.Send(
					$"{"[Queued Action]: ".ColourBold(Telnet.Yellow)}Seeking cover @ {newcover.Cover.Describe(actor, newcover.CoverItem?.Parent, actor)}");
			}

			return;
		}

		actor.Cover?.LeaveCover();
		actor.OutputHandler.Handle(new EmoteOutput(newcover.Cover.DescribeAction(actor, newcover.CoverItem?.Parent)));
		actor.SetPosition(
			newcover.Cover.HighestPositionState.CompareTo(actor.PositionState) == PositionHeightComparison.Lower
				? actor.PositionState = newcover.Cover.HighestPositionState
				: actor.PositionState, newcover.CoverItem != null ? PositionModifier.Behind : PositionModifier.None,
			newcover.CoverItem?.Parent, null);
		actor.Cover = newcover;
		newcover.RegisterEvents();
	}

	[PlayerCommand("Charge", "charge")]
	[NoMovementCommand]
	[RequiredCharacterState(CharacterState.Able)]
	[DelayBlock("general", "aim", "You must first stop {0} before you can do that.")]
	protected static void Charge(ICharacter actor, string command)
	{
		if (actor.Combat == null)
		{
			actor.Send("You can only use this command when you are already in combat.");
			return;
		}

		if (actor.MeleeRange)
		{
			actor.Send(
				"You are already in melee combat; you must first leave the fray somehow before you can charge back in.");
			return;
		}

		var ss = new StringStack(command.RemoveFirstWord());
		var target = actor.CombatTarget as ICharacter;
		if (!ss.IsFinished)
		{
			var keyword = ss.Pop();
			target = actor.Combat.Friendly ? actor.TargetActor(keyword) : actor.TargetNonAlly(keyword);
		}

		if (target == null)
		{
			actor.Send("You must be fighting, or otherwise specify a target character to charge at.");
			return;
		}

		if (!actor.CanMove())
		{
			actor.Send("You can't charge at anything because you can't move.");
			return;
		}

		if (!actor.CanSpendStamina(ChargeToMeleeMove.MoveStaminaCost(actor)))
		{
			actor.Send("You lack the stamina to charge into melee right now.");
			return;
		}

		if (actor.TakeOrQueueCombatAction(SelectedCombatAction.GetEffectCharge(actor, target)) &&
			actor.Gameworld.GetStaticBool("EchoQueuedActions"))
		{
			actor.Send(
				$"{"[Queued Action]: ".ColourBold(Telnet.Yellow)}Charging into melee with {target.HowSeen(actor)}.");
		}
	}

	[PlayerCommand("Engage", "engage")]
	[NoMovementCommand]
	[RequiredCharacterState(CharacterState.Able)]
	[DelayBlock("general", "aim", "You must first stop {0} before you can do that.")]
	protected static void Engage(ICharacter actor, string command)
	{
		if (actor.Combat == null)
		{
			actor.Send("You can only use this command when you are already in combat.");
			return;
		}

		if (actor.MeleeRange)
		{
			actor.Send(
				"You are already in melee combat; you must first leave the fray somehow before you can move back in.");
			return;
		}

		var ss = new StringStack(command.RemoveFirstWord());
		var target = actor.CombatTarget as ICharacter;
		if (!ss.IsFinished)
		{
			var keyword = ss.Pop();
			target = actor.Combat.Friendly ? actor.TargetActor(keyword) : actor.TargetNonAlly(keyword);
		}

		if (target == null)
		{
			actor.Send("You must be fighting, or otherwise specify a target character to engage.");
			return;
		}

		if (!actor.CanMove())
		{
			actor.Send("You can't engage anything because you can't move.");
			return;
		}

		if (!actor.CanSpendStamina(ChargeToMeleeMove.MoveStaminaCost(actor)))
		{
			actor.Send("You lack the stamina to engage in melee right now.");
			return;
		}

		if (actor.TakeOrQueueCombatAction(SelectedCombatAction.GetEffectMoveToMelee(actor, target)) &&
			actor.Gameworld.GetStaticBool("EchoQueuedActions"))
		{
			actor.Send(
				$"{"[Queued Action]: ".ColourBold(Telnet.Yellow)}Engaging in melee with {target.HowSeen(actor)}.");
		}
	}

	[PlayerCommand("Target", "target")]
	[RequiredCharacterState(CharacterState.Conscious)]
	[HelpInfo("target",
		@"This command allows you to specifically target an opponent's bodypart, such as a hand, an eye, their head etc. This makes it far more likely that you will hit these locations in combat. This works for both melee and ranged combat.

While targeting a bodypart, your melee strikes will be easier to defend against in proportion to how hard the bodypart was to hit in the first place. This is because rather than waiting for openings in your opponent's defense you are choosing to make non-optimal strikes to hit the target bodypart.

Additionally, it will be slightly harder for you to defend against others while you continue to target the bodypart. This is because in making these non-optimal strikes you are finishing in a slightly worse position for maintaining an effective defense.

Finally, in ranged combat your shots will be harder while you are trying to hit a specific bodypart. This penalty gets worse non-linearly over longer ranges.

The syntax for this command is as follows:

  #3target <bodypart>#0 - begins targeting a specific bodypart of your current combat target
  #3target none#0 - stops targeting any specific bodypart", AutoHelp.HelpArg)]
	protected static void Target(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if ((actor.Combat == null || actor.CombatTarget == null) && !actor.EffectsOfType<OutOfCombatAim>().Any())
		{
			actor.Send("You aren't fighting anyone, so there's nothing more specific to target.");
			return;
		}

		if (ss.IsFinished)
		{
			if (actor.TargettedBodypart is not null)
			{
				var target = actor.CombatTarget ?? actor.EffectsOfType<OutOfCombatAim>().FirstOrDefault()?.Target ??
					new DummyPerceiver("your target", sentient: true);
				actor.OutputHandler.Send(
					$"You are currently targeting {target.HowSeen(actor, type: DescriptionType.Possessive)} {actor.TargettedBodypart.FullDescription()}.");
			}
			else
			{
				actor.OutputHandler.Send($"You are not targeting any particular bodyparts on your target.");
			}

			return;
		}

		if (ss.Peek().EqualTo("none"))
		{
			actor.Send(actor.TargettedBodypart == null
				? "You're not targeting any specific bodyparts at the moment."
				: "You are no longer targeting any specific bodyparts.");
			actor.TargettedBodypart = null;
			return;
		}

		var tch = actor.CombatTarget as ICharacter ??
				  actor.EffectsOfType<OutOfCombatAim>().FirstOrDefault()?.Target as ICharacter;
		if (tch == null)
		{
			actor.Send("You can't use this option when you're targeting something other than characters.");
			return;
		}

		var part = tch.Body.GetTargetBodypart(ss.SafeRemainingArgument);
		if (part == null)
		{
			actor.Send($"{tch.HowSeen(actor, true)} doesn't have any bodypart like that to target.");
			return;
		}

		if (part.RelativeHitChance <= 0)
		{
			actor.Send("That bodypart cannot be specifically targeted.");
			return;
		}

		actor.TargettedBodypart = part;
		actor.Send(
			$"You will now try to choose attacks that target {tch.HowSeen(actor, false, DescriptionType.Possessive)} {part.FullDescription()}.");
	}

	#region Combat Subcommands

	protected static IEnumerable<ICharacterCombatSettings> GetSettingsForCharacter(ICharacter actor, string argument)
	{
		if (actor.IsAdministrator())
		{
			return actor.Gameworld.CharacterCombatSettings.ToList();
		}

		return argument.Equals("mine", StringComparison.InvariantCultureIgnoreCase)
			? actor.Gameworld.CharacterCombatSettings.Where(x => x.CharacterOwnerId == actor.Id).ToList()
			: actor.Gameworld.CharacterCombatSettings.Where(
				x => ((bool?)x.AvailabilityProg?.Execute(actor) ?? true) &&
					 (x.GlobalTemplate || x.CharacterOwnerId == actor.Id)
			).ToList();
	}

	protected static void CombatList(ICharacter actor, StringStack command)
	{
		var settings = GetSettingsForCharacter(actor, command.PopSpeech());

		actor.Send("Combat Settings:\n\n" +
				   StringUtilities.GetTextTable(
					   from setting in settings
					   select new[]
					   {
						   setting.Id.ToString("N0", actor),
						   setting.Name.TitleCase(),
						   setting.Description.ProperSentences(),
						   setting.GlobalTemplate ? "Y" : "N"
					   },
					   new[] { "ID", "Name", "Description", "Global" },
					   actor.LineFormatLength,
					   truncatableColumnIndex: 2, unicodeTable: actor.Account.UseUnicode
				   )
		);
	}

	protected static void CombatShow(ICharacter actor, StringStack command)
	{
		ICharacterCombatSettings setting;
		if (command.IsFinished)
		{
			setting = actor.CombatSettings;
		}
		else
		{
			setting = long.TryParse(command.SafeRemainingArgument, out var value)
				? actor.Gameworld.CharacterCombatSettings.Get(value)
				: actor.Gameworld.CharacterCombatSettings.GetByName(command.SafeRemainingArgument);
			if (setting == null)
			{
				actor.Send("There is no such combat setting to show you.");
				return;
			}

			if (!actor.IsAdministrator(PermissionLevel.Admin) && setting.CharacterOwnerId != actor.Id &&
				(!setting.GlobalTemplate || ((bool?)setting.AvailabilityProg?.Execute(actor) ?? true)))
			{
				actor.Send("You do not have permission to view that combat setting.");
				return;
			}
		}

		actor.Send(setting.Show(actor));
	}

	protected static void CombatClone(ICharacter actor, StringStack command)
	{
		if (actor.IsGuest)
		{
			actor.Send("Guests are not allowed to modify combat configurations.");
			return;
		}

		var maxSettings = actor.Gameworld.GetStaticInt("MaximumCombatSettingsPerPlayer");
		if (actor.Gameworld.CharacterCombatSettings.Count(x => x.CharacterOwnerId == actor.Id) >= maxSettings)
		{
			actor.Send(
				$"You are only allowed a maximum of {maxSettings:N0} personal combat settings at any time. You must free some of your existing ones before you can clone any more.");
			return;
		}

		if (command.IsFinished)
		{
			actor.Send(
				$"Which combat setting do you want to clone? In order to clone your current one, use {$"combat clone \"{actor.CombatSettings.Name}\" \"New Name\"".Colour(Telnet.Yellow)}.");
			return;
		}

		var potentialSettings = GetSettingsForCharacter(actor, string.Empty);
		var setting = long.TryParse(command.PopSpeech(), out var value)
			? potentialSettings.FirstOrDefault(x => x.Id == value)
			: potentialSettings.FirstOrDefault(
				  x => x.Name.Equals(command.Last, StringComparison.InvariantCultureIgnoreCase)) ??
			  potentialSettings.FirstOrDefault(
				  x => x.Name.StartsWith(command.Last, StringComparison.InvariantCultureIgnoreCase));

		if (setting == null)
		{
			actor.Send("There is no such setting for you to clone.");
			return;
		}

		if (command.IsFinished)
		{
			actor.Send("You must specify a unique name for your new setting.");
			return;
		}

		var newName = command.SafeRemainingArgument;
		if (potentialSettings.Any(x => x.Name.Equals(newName, StringComparison.InvariantCultureIgnoreCase)))
		{
			actor.Send("That name is not unique amongst the ones you can select. You must select a unique name.");
			return;
		}

		var newSetting = new CharacterCombatSettings(actor, (CharacterCombatSettings)setting, newName);
		actor.Gameworld.Add(newSetting);
		actor.Send(
			$"You clone the combat settings {setting.Name.TitleCase().Colour(Telnet.Green)}, entitling it {newName.TitleCase().Colour(Telnet.Green)}. This has been set to your current setting.");
		actor.CombatSettings = newSetting;
	}

	private static void CombatShowCombat(ICharacter actor, StringStack command)
	{
		if (!command.IsFinished && actor.IsAdministrator())
		{
			var target = actor.TargetActor(command.Pop());
			if (target == null)
			{
				actor.Send("You don't see anyone like that to view their combat status.");
				return;
			}

			if (target.Combat == null)
			{
				actor.Send($"{target.HowSeen(actor, true)} is not engaged in combat.");
				return;
			}

			actor.Send(target.Combat.DescribeFor(actor));
			return;
		}

		actor.Send(actor.Combat?.DescribeFor(actor) ?? "You are not currently engaged in any combats.");
	}

	private static void CombatHelp(ICharacter actor, StringStack ss)
	{
		var sb = new StringBuilder();
		sb.AppendLine("Combat Command Help");
		sb.AppendLine();
		sb.AppendLine(
			@$"The combat command is used to view, manage and adopt various settings that control how your character will act in combat. 

Most important of these are {"combat settings".ColourName()}, which are pre-defined sets of rules about what your character will do in combat. There are a number of global templates for these that cover a variety of situations but you can create and customise your own once you feel comfortable doing so.

The syntax to use this command is as follows:");
		sb.AppendLine();
		sb.AppendLine($"\t{"combat help".Colour(Telnet.Yellow)} - shows this list");
		sb.AppendLine($"\t{"combat status".Colour(Telnet.Yellow)} - shows information about your current conflict");
		sb.AppendLine($"\t{"combat list".Colour(Telnet.Yellow)} - shows available combat settings");
		sb.AppendLine(
			$"\t{"combat show <id/name>".Colour(Telnet.Yellow)} - shows details of a particular combat setting");
		sb.AppendLine(
			$"\t{"combat set <id/name>".Colour(Telnet.Yellow)} - adopts a particular combat setting as your current");
		sb.AppendLine(
			$"\t{"combat clone <id/name> <newname>".Colour(Telnet.Yellow)} - makes a new combat setting out of an existing one for editing");
		sb.AppendLine(
			$"\t{"combat defense <block/dodge/parry/none>".Colour(Telnet.Yellow)} - sets or clears a favoured defense type");
		sb.AppendLine();
		sb.AppendLine("Additionally, there are numerous options for configuring combat settings.");
		sb.AppendLine($"\tSee {"combat config help".Colour(Telnet.Yellow)} for more information.");

		actor.Send(sb.ToString());
	}

	private static void CombatSet(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			CombatShow(actor, ss);
			return;
		}

		var potentialSettings = GetSettingsForCharacter(actor, string.Empty);
		var setting = long.TryParse(ss.PopSpeech(), out var value)
			? potentialSettings.FirstOrDefault(x => x.Id == value)
			: potentialSettings.FirstOrDefault(
				  x => x.Name.Equals(ss.Last, StringComparison.InvariantCultureIgnoreCase)) ??
			  potentialSettings.FirstOrDefault(
				  x => x.Name.StartsWith(ss.Last, StringComparison.InvariantCultureIgnoreCase));

		if (setting == null)
		{
			actor.Send("There is no such Combat Setting for you to adopt.");
			return;
		}

		actor.CombatSettings = setting;
		actor.Send(
			$"You will now fight with the {setting.Name.TitleCase().Colour(Telnet.Green)} package of combat settings.");
	}

	#region Combat Config Subcommands

	private static void CombatConfigHelp(ICharacter actor)
	{
		actor.Send($@"You can edit the following settings:

	#3name <name>#0         - Change the name of the combat setting
	#3desc <description>#0  - Change the description
	#3global#0              - Toggle the Global flag (admin only)
	#3availprog <progID>#0  - Configure the Availability Prog (admin only)
	#3classifications <classifications>#0 - Designated weapon classifications legal for this Combat Config
	#3character <id>#0      - Reassign a Combat Config to a character
	#3aim <percentage>#0    - The percentage of aim before you auto-shoot
	#3stamina <amount>#0    - The amount of stamina below which you will stop attacking
	#3melee <strategy>#0    - Designate overall melee strategy
	#3range <strategy>#0    - Designate overall ranged strategy
	#3grapple <response>#0  - Set the response to someone grappling you
	#3setup <setup>#0       - Preferred overall gear setup for the auto-inventory
	#3weapon <percentage>#0 - configures the percentage chance of you using a weapon attack move
	#3natural <percentage>#0 - configures the percentage chance of you using a natural attack move
	#3auxiliary <percentage>#0 - configures the percentage chance of you using an auxiliary move
	#3magic <percentage>#0 - configures the percentage chance of you using a magic attack move
	#3prefer_armed <true/false>#0 - configures whether you prefer to fight armed and will seek to wield a weapon
	#3prefer_favourite <true/false>#0 - configures whether you prefer to retrieve the weapon you're wielding rather than drawing a new one if you lose it
	#3prefer_shield <true/false>#0 - configures whether you prefer to fight with a shield
	#3prefer_stagger <true/false>#0 - configures whether you prefer using staggering blows over footwork to end clinches
	#3fallback <true/false>#0 - configures whether you will fallback to unarmed attacks if you cannot acquire a weapon
	#3attack_helpless <true/false>#0 - configures whether you will attack a helpless opponent
	#3attack_critical <true/false>#0 - configures whether you will attack a critically injured opponent
	#3pursue <true/false>#0 - configures whether you will pursue targets to other locations if they flee
	#3auto_inventory <auto|manual|no_discard|retrieve_only>#0 - changes the degree of automation of your inventory management
	#3auto_ranged <auto|manual|continue_only>#0 - changes the degree of automation of your ranged combat
	#3auto_move <auto|manual|cover_only|keep_range>#0 - changes the degree of automation of your combat movement
	#3auto_position#0 - Toggle whether position will be managed automatically
	#3auto_melee#0 - Toggle whether you will automatically move to melee if you are unable to engage in ranged combat
	#3movetotarget#0 - Toggle whether you will try to close to the same room as a distant target
	#3skirmish#0 - Toggle whether you will disengage to other rooms when skirmishing
	#3swap <type1> <type2>#0 - swaps the order in which you will execute different attack types

The following options refer to flags listed in the SHOW COMBATFLAGS list:

	#3require <flag>#0 - toggles a flag that is REQUIRED for all combat moves you use. Moves without the flag will not be used.
	#3prefer <flag>#0 - toggles a flag that is PREFERRED for all combat moves you use. Moves with the flag are significantly more likely to be used.
	#3forbid <flag>#0 - toggles a flag that is FORBIDDEN for all combat moves you use. Moves with the flag will not be used.
".SubstituteANSIColour());
	}

	private static void CombatConfigStamina(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.Send("You must specify an amount of stamina below which you will stop attacking.");
			return;
		}

		actor.CombatSettings.MinimumStaminaToAttack = value;
		actor.CombatSettings.Changed = true;
		actor.Send($"You will now stop attacking your target when you reach {value:N2} stamina.");
	}

	private static void CombatConfigSwapOrder(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which two melee attack preferences do you want to swap?");
			return;
		}

		if (!Enum.TryParse<MeleeAttackOrderPreference>(command.PopSpeech(), true, out var pref1))
		{
			actor.OutputHandler.Send(
				$"There is no such attack preference. Valid selections are {Enum.GetNames(typeof(MeleeAttackOrderPreference)).Select(x => x.Colour(Telnet.Yellow)).ListToString()}.");
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What is the second melee attack preference you want to swap?");
			return;
		}

		if (!Enum.TryParse<MeleeAttackOrderPreference>(command.PopSpeech(), true, out var pref2))
		{
			actor.OutputHandler.Send(
				$"There is no such second attack preference. Valid selections are {Enum.GetNames(typeof(MeleeAttackOrderPreference)).Select(x => x.Colour(Telnet.Yellow)).ListToString()}.");
			return;
		}

		if (pref1 == pref2)
		{
			actor.OutputHandler.Send("You can't swap an attack preference with itself.");
			return;
		}

		actor.CombatSettings.MeleeAttackOrderPreferences.Swap(pref1, pref2);
		actor.CombatSettings.Changed = true;
		actor.OutputHandler.Send(
			$"You swap the order of preference for {pref1.DescribeEnum().Colour(Telnet.Yellow)} attacks and {pref2.DescribeEnum().Colour(Telnet.Yellow)} attacks.");
	}

	private static void CombatConfigSetup(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				$"Valid options are {new[] { "1h", "2h", "dwield", "shield", "none" }.Select(x => x.Colour(Telnet.Yellow)).ListToString()}.");
			return;
		}

		AttackHandednessOptions option;
		switch (command.SafeRemainingArgument.ToLowerInvariant())
		{
			case "1h":
			case "1hand":
			case "1-hand":
			case "1-handed":
			case "1 handed":
			case "one handed":
			case "one-handed":
			case "one":
			case "one hand":
			case "one-hand":
				option = AttackHandednessOptions.OneHandedOnly;
				break;
			case "2h":
			case "2hand":
			case "2-hand":
			case "2-handed":
			case "2 handed":
			case "two handed":
			case "two-handed":
			case "two":
			case "two hand":
			case "two-hand":
				option = AttackHandednessOptions.TwoHandedOnly;
				break;
			case "none":
			case "any":
			case "clear":
			case "no":
				option = AttackHandednessOptions.Any;
				break;
			case "dwield":
			case "dual":
			case "dual wield":
				option = AttackHandednessOptions.DualWieldOnly;
				break;
			case "shield":
			case "sword and board":
			case "sword & board":
			case "board":
				option = AttackHandednessOptions.SwordAndBoardOnly;
				break;
			default:
				actor.Send(
					$"Valid options are {new[] { "1h", "2h", "dwield", "shield", "none" }.Select(x => x.Colour(Telnet.Yellow)).ListToString()}.");
				return;
		}

		actor.CombatSettings.PreferredWeaponSetup = option;
		actor.CombatSettings.Changed = true;
		actor.Send($"You will now prefer the {option.Describe()} setup in auto-inventory.");
	}

	private static void CombatConfigAim(ICharacter actor, StringStack command)
	{
		if (command.IsFinished ||
			!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value))
		{
			actor.Send("You must specify a valid percentage of aim at which the auto-shooter will shoot your gun.");
			return;
		}

		actor.CombatSettings.RequiredMinimumAim = Math.Min(Math.Max(value, 0.0), 1.0);
		actor.CombatSettings.Changed = true;
		actor.Send($"You will now shoot at your target as soon as you reach {value:P} aim.");
	}

	private static void CombatConfigPursue(ICharacter actor, StringStack command)
	{
		var truth = GetTruthValue(actor, command, actor.CombatSettings.AutomaticallyMoveTowardsTarget);
		if (truth == null)
		{
			return;
		}

		actor.CombatSettings.AutomaticallyMoveTowardsTarget = truth.Value;
		actor.CombatSettings.Changed = true;
		actor.Send(truth.Value
			? StringUtilities.HMark + "You will now automatically move towards your target when in different locations."
			: StringUtilities.HMark +
			  "You will no longer automatically move towards your target when in different locations.");
	}

	private static void CombatConfigMoveToTarget(ICharacter actor, StringStack command)
	{
		var truth = GetTruthValue(actor, command, actor.CombatSettings.AutomaticallyMoveTowardsTarget);
		if (truth == null)
		{
			return;
		}

		actor.CombatSettings.AutomaticallyMoveTowardsTarget = truth.Value;
		actor.CombatSettings.Changed = true;
		actor.Send(truth.Value
			? StringUtilities.HMark + "You will now chase your target into other rooms."
			: StringUtilities.HMark + "You will no longer chase your target into other rooms.");
	}

	private static void CombatConfigPreferStagger(ICharacter actor, StringStack command)
	{
		var truth = GetTruthValue(actor, command, !actor.CombatSettings.PreferNonContactClinchBreaking);
		if (truth == null)
		{
			return;
		}

		actor.CombatSettings.PreferNonContactClinchBreaking = !truth.Value;
		actor.CombatSettings.Changed = true;
		actor.Send(truth.Value
			? "You will now prefer to use staggering blows to end opponent's clinches."
			: "You will now prefer to use good footwork to end opponent's clinches.");
	}

	private static void CombatConfigForbid(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(StringUtilities.HMark +
					   $"Toggle a Forbidden CombatFlag on or off via: combat config forbid <CombatFlag>\n" +
					   StringUtilities.Indent + "Valid options are: " +
					   Enum.GetValues(typeof(CombatMoveIntentions)).OfType<CombatMoveIntentions>()
						   .OrderBy(x => (int)x)
						   .Select(x => x.Describe().Colour(Telnet.Green))
						   .ListToString() + ".");
			return;
		}

		if (!CombatExtensions.TryParseCombatMoveIntention(command.PopSpeech(), out var intention))
		{
			actor.Send(StringUtilities.HMark +
					   $"There is no such combat flag. You can view the combat flags with the command {"show combatflags".Colour(Telnet.Yellow)}.");
			return;
		}

		if (actor.CombatSettings.ForbiddenIntentions.HasFlag(intention))
		{
			actor.CombatSettings.ForbiddenIntentions &= ~intention;
			actor.Send(StringUtilities.HMark +
					   $"You will once again use combat moves which have the {intention.Describe().Colour(Telnet.Green)} flag.");
		}
		else
		{
			actor.CombatSettings.ForbiddenIntentions |= intention;
			actor.Send(StringUtilities.HMark +
					   $"You will no longer use combat moves which have the {intention.Describe().Colour(Telnet.Green)} flag.");
		}
	}

	private static void CombatConfigPrefer(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(StringUtilities.HMark +
					   $"Toggle a Preferred CombatFlag on or off via: combat config prefer <CombatFlag>\n" +
					   StringUtilities.Indent + "Valid options are: " +
					   Enum.GetValues(typeof(CombatMoveIntentions)).OfType<CombatMoveIntentions>()
						   .OrderBy(x => (int)x)
						   .Select(x => x.Describe().Colour(Telnet.Green))
						   .ListToString() + ".");
			return;
		}

		if (!CombatExtensions.TryParseCombatMoveIntention(command.PopSpeech(), out var intention))
		{
			actor.Send(StringUtilities.HMark +
					   $"There is no such combat flag. You can view the combat flags with the command {"show combatflags".Colour(Telnet.Yellow)}.");
			return;
		}

		if (actor.CombatSettings.PreferredIntentions.HasFlag(intention))
		{
			actor.CombatSettings.PreferredIntentions &= ~intention;
			actor.Send(StringUtilities.HMark +
					   $"You will no longer prefer combat moves which have the {intention.Describe().Colour(Telnet.Green)} flag.");
		}
		else
		{
			actor.CombatSettings.PreferredIntentions |= intention;
			actor.Send(StringUtilities.HMark +
					   $"You will now prefer combat moves which have the {intention.Describe().Colour(Telnet.Green)} flag.");
		}
	}

	private static void CombatConfigRequire(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(StringUtilities.HMark +
					   $"Toggle a Required CombatFlag on or off via: combat config require <CombatFlag>\n" +
					   StringUtilities.Indent + "Valid options are: " +
					   Enum.GetValues(typeof(CombatMoveIntentions)).OfType<CombatMoveIntentions>()
						   .OrderBy(x => (int)x)
						   .Select(x => x.Describe().Colour(Telnet.Green))
						   .ListToString() + ".");
			return;
		}

		if (!CombatExtensions.TryParseCombatMoveIntention(command.PopSpeech(), out var intention))
		{
			actor.Send(StringUtilities.HMark +
					   $"There is no such combat flag. You can view the combat flags with the command {"show combatflags".Colour(Telnet.Yellow)}.");
			return;
		}

		if (actor.CombatSettings.RequiredIntentions.HasFlag(intention))
		{
			actor.CombatSettings.RequiredIntentions &= ~intention;
			actor.Send(StringUtilities.HMark +
					   $"You will no longer require combat moves which have the {intention.Describe().Colour(Telnet.Green)} flag.");
		}
		else
		{
			actor.CombatSettings.RequiredIntentions |= intention;
			actor.Send(StringUtilities.HMark +
					   $"You will now require combat moves which have the {intention.Describe().Colour(Telnet.Green)} flag.");
		}
	}

	private static void CombatConfigMagic(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Syntax: combat config magic <percentage>");
			return;
		}

		if (!command.PopSpeech().TryParsePercentage(actor.Account.Culture, out var value))
		{
			actor.Send("You must enter a percentage value for your magic attack percentage.");
			return;
		}

		if (value < 0.0 || value > 1.0)
		{
			actor.Send($"You must enter a percentage between {0:P0} and {1:P0}.");
			return;
		}

		actor.CombatSettings.MagicUsePercentage = value;
		actor.CombatSettings.Changed = true;
		RebalanceCombatPercentages(actor, fixedMagic: true);
		ShowCurrentBalances(actor);
	}

	private static void CombatConfigPsychic(ICharacter actor, StringStack command)
	{
		actor.Send("Not available yet.");
	}

	private static void RebalanceCombatPercentages(ICharacter actor, bool fixedWeapon = false,
		bool fixedNatural = false,
		bool fixedAuxiliary = false, bool fixedMagic = false, bool fixedPsychic = false)
	{
		double total()
		{
			return actor.CombatSettings.MagicUsePercentage + actor.CombatSettings.PsychicUsePercentage +
				   actor.CombatSettings.NaturalWeaponPercentage + actor.CombatSettings.WeaponUsePercentage +
				   actor.CombatSettings.AuxiliaryPercentage;
		}

		while (1.0 - total() > 0.00005)
		{
			if (!fixedMagic && actor.CombatSettings.MagicUsePercentage > 0)
			{
				actor.CombatSettings.MagicUsePercentage = Math.Max(0,
					actor.CombatSettings.MagicUsePercentage - total() + 1.0);
				continue;
			}

			if (!fixedPsychic && actor.CombatSettings.PsychicUsePercentage > 0)
			{
				actor.CombatSettings.PsychicUsePercentage = Math.Max(0,
					actor.CombatSettings.PsychicUsePercentage - total() + 1.0);
				continue;
			}

			if (!fixedAuxiliary && actor.CombatSettings.AuxiliaryPercentage > 0)
			{
				actor.CombatSettings.AuxiliaryPercentage = Math.Max(0,
					actor.CombatSettings.AuxiliaryPercentage - total() + 1.0);
				continue;
			}

			if (!fixedWeapon && actor.CombatSettings.WeaponUsePercentage > 0)
			{
				actor.CombatSettings.WeaponUsePercentage = Math.Max(0,
					actor.CombatSettings.WeaponUsePercentage - total() + 1.0);
				continue;
			}

			if (!fixedNatural && actor.CombatSettings.NaturalWeaponPercentage > 0)
			{
				actor.CombatSettings.NaturalWeaponPercentage = Math.Max(0,
					actor.CombatSettings.NaturalWeaponPercentage - total() + 1.0);
				continue;
			}

			break;
		}

		actor.CombatSettings.Changed = true;
	}

	private static void ShowCurrentBalances(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.Append(
			$"Your current move balance stands at: Weapon [{actor.CombatSettings.WeaponUsePercentage.ToString("P0", actor).Colour(Telnet.Green)}] Natural [{actor.CombatSettings.NaturalWeaponPercentage.ToString("P0", actor).Colour(Telnet.Green)}] Auxiliary [{actor.CombatSettings.AuxiliaryPercentage.ToString("P0", actor).Colour(Telnet.Green)}] Magic [{actor.CombatSettings.MagicUsePercentage.ToString("P0", actor).Colour(Telnet.Green)}]");
		actor.Send(sb.ToString());
	}

	private static void CombatConfigNatural(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Syntax: #3combat config natural <number>#0".SubstituteANSIColour());
			return;
		}

		if (!command.PopSpeech().TryParsePercentage(actor.Account.Culture, out var value))
		{
			actor.Send("You must enter a percentage value for your natural weapon attacks percentage.");
			return;
		}

		if (value < 0.0 || value > 1.0)
		{
			actor.Send($"You must enter a percentage between {0:P0} and {1:P0}.");
			return;
		}

		actor.CombatSettings.NaturalWeaponPercentage = value;
		actor.CombatSettings.Changed = true;
		RebalanceCombatPercentages(actor, fixedNatural: true);
		ShowCurrentBalances(actor);
	}

	private static void CombatConfigAuxiliary(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Syntax: #3combat config auxiliary <number>#0".SubstituteANSIColour());
			return;
		}

		if (!command.PopSpeech().TryParsePercentage(actor.Account.Culture, out var value))
		{
			actor.Send("You must enter a percentage value for your auxiliary moves percentage.");
			return;
		}

		if (value < 0.0 || value > 1.0)
		{
			actor.Send($"You must enter a percentage between {0:P0} and {1:P0}.");
			return;
		}

		actor.CombatSettings.AuxiliaryPercentage = value;
		actor.CombatSettings.Changed = true;
		RebalanceCombatPercentages(actor, fixedAuxiliary: true);
		ShowCurrentBalances(actor);
	}

	private static bool? GetTruthValue(ICharacter actor, StringStack command, bool defaultTruthValue)
	{
		if (!command.IsFinished)
		{
			switch (command.PopSpeech().ToLowerInvariant())
			{
				case "true":
				case "on":
				case "yes":
				case "y":
					return true;
				case "false":
				case "off":
				case "no":
				case "n":
					return false;
				default:
					actor.Send(StringUtilities.HMark +
							   $"You must either specify {"true".Colour(Telnet.Green)} or {"false".Colour(Telnet.Red)}, or just omit the argument altogether to toggle.");
					return null;
			}
		}

		return !defaultTruthValue;
	}

	private static void CombatConfigPreferFavourite(ICharacter actor, StringStack command)
	{
		var truth = GetTruthValue(actor, command, actor.CombatSettings.PreferFavouriteWeapon);
		if (truth == null)
		{
			return;
		}

		actor.CombatSettings.PreferFavouriteWeapon = truth.Value;
		actor.CombatSettings.Changed = true;
		actor.Send(truth.Value
			? StringUtilities.HMark +
			  "You will now prefer to retrieve your current weapon rather than drawing a new one."
			: StringUtilities.HMark + "You will now prefer to draw a new weapon rather than retrieving a dropped one.");
	}

	private static void CombatConfigPreferShield(ICharacter actor, StringStack command)
	{
		var truth = GetTruthValue(actor, command, actor.CombatSettings.PreferShieldUse);
		if (truth == null)
		{
			return;
		}

		actor.CombatSettings.PreferShieldUse = truth.Value;
		actor.CombatSettings.Changed = true;
		actor.Send(truth.Value
			? StringUtilities.HMark + "You will now prefer to fight with a shield."
			: StringUtilities.HMark + "You will now prefer to fight without a shield.");
	}

	private static void CombatConfigFallback(ICharacter actor, StringStack command)
	{
		var truth = GetTruthValue(actor, command, actor.CombatSettings.FallbackToUnarmedIfNoWeapon);
		if (truth == null)
		{
			return;
		}

		actor.CombatSettings.FallbackToUnarmedIfNoWeapon = truth.Value;
		actor.CombatSettings.Changed = true;
		actor.Send(truth.Value
			? StringUtilities.HMark + "You will now fallback to unarmed combat if you are unable to arm yourself."
			: StringUtilities.HMark + "You will no longer fallback to unarmed combat if you cannot arm yourself.");
	}

	private static void CombatConfigAttackHelpless(ICharacter actor, StringStack command)
	{
		var truth = GetTruthValue(actor, command, actor.CombatSettings.AttackUnarmedOrHelpless);
		if (truth == null)
		{
			return;
		}

		actor.CombatSettings.AttackUnarmedOrHelpless = truth.Value;
		actor.CombatSettings.Changed = true;
		actor.Send(truth.Value
			? StringUtilities.HMark + "You will now attack unarmed or helpless (prone or sitting) opponents."
			: StringUtilities.HMark + "You will no longer attack unarmed or helpless (prone or sitting) opponents.");
	}

	private static void CombatConfigAttackCritical(ICharacter actor, StringStack command)
	{
		var truth = GetTruthValue(actor, command, actor.CombatSettings.AttackCriticallyInjured);
		if (truth == null)
		{
			return;
		}

		actor.CombatSettings.AttackCriticallyInjured = truth.Value;
		actor.CombatSettings.Changed = true;
		actor.Send(truth.Value
			? StringUtilities.HMark +
			  "You will now attack the critically injured (For now, this just forbids [5mcoup de grace[25m)."
			: StringUtilities.HMark + "You will no longer attack the critically injured.");
	}

	private static void CombatConfigSkirmish(ICharacter actor, StringStack command)
	{
		var truth = GetTruthValue(actor, command, actor.CombatSettings.SkirmishToOtherLocations);
		if (truth == null)
		{
			return;
		}

		actor.CombatSettings.SkirmishToOtherLocations = truth.Value;
		actor.CombatSettings.Changed = true;
		actor.Send(truth.Value
			? StringUtilities.HMark + "You will now retreat to other rooms when skirmishing."
			: StringUtilities.HMark + "You will no longer retreat to other rooms when skirmishing.");
	}

	private static void CombatConfigPreferArmed(ICharacter actor, StringStack command)
	{
		var truth = GetTruthValue(actor, command, actor.CombatSettings.PreferToFightArmed);
		if (truth == null)
		{
			return;
		}

		actor.CombatSettings.PreferToFightArmed = truth.Value;
		actor.CombatSettings.Changed = true;
		actor.Send(truth.Value
			? StringUtilities.HMark + "You will now prefer to fight armed with a melee weapon."
			: StringUtilities.HMark + "You will no longer prefer to fight armed with a melee weapon.");
	}

	private static void CombatConfigWeapon(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Syntax: combat config weapon <number>");
			return;
		}

		if (!command.PopSpeech().TryParsePercentage(actor.Account.Culture, out var value))
		{
			actor.Send("You must enter a percentage value for your melee weapon attacks percentage.");
			return;
		}

		if (value < 0.0 || value > 1.0)
		{
			actor.Send($"You must enter a percentage between {0:P0} and {1:P0}.");
			return;
		}

		actor.CombatSettings.WeaponUsePercentage = value;
		actor.CombatSettings.Changed = true;
		RebalanceCombatPercentages(actor, true);
		ShowCurrentBalances(actor);
	}

	private static void CombatConfigDescription(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(StringUtilities.HMark + "Syntax: combat config desc <description>");
			return;
		}

		actor.CombatSettings.Description = command.SafeRemainingArgument;
		actor.CombatSettings.Changed = true;

		actor.Send(StringUtilities.HMark +
				   $"Description for {actor.CombatSettings.Name} updated to '{actor.CombatSettings.Description}'"
					   .Colour(Telnet.BoldGreen));
	}

	private static void CombatConfigGlobal(ICharacter actor, StringStack command)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not permitted to change settings to global settings.".Colour(Telnet.BoldRed));
			return;
		}

		if (actor.CombatSettings.GlobalTemplate)
		{
			actor.CombatSettings.GlobalTemplate = false;
			actor.CombatSettings.Changed = true;
			actor.Send($"{actor.CombatSettings.Name.ColourName()} is no longer a global template.".Colour(Telnet.BoldGreen));
		}
		else
		{
			actor.CombatSettings.GlobalTemplate = true;
			actor.CombatSettings.Changed = true;
			actor.Send($"{actor.CombatSettings.Name.ColourName()} is now a global template.".Colour(Telnet.BoldGreen));
		}
	}

	private static void CombatConfigAvailProg(ICharacter actor, StringStack command)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send(StringUtilities.HMark + "Permission denied.".Colour(Telnet.BoldRed));
			return;
		}

		if (command.IsFinished)
		{
			actor.Send(StringUtilities.HMark + "Syntax: combat config availprog <progID>");
			return;
		}

		if (command.PeekSpeech().Equals("clear", StringComparison.InvariantCultureIgnoreCase))
		{
			actor.CombatSettings.AvailabilityProg = null;
			actor.CombatSettings.Changed = true;
			actor.Send(StringUtilities.HMark +
					   $"Availability prog for {actor.CombatSettings.Name} cleared.".Colour(Telnet.BoldGreen));
			return;
		}

		var prog = long.TryParse(command.PopSpeech(), out var value)
			? actor.Gameworld.FutureProgs.Get(value)
			: actor.Gameworld.FutureProgs.GetByName(command.Last);

		if (prog == null)
		{
			actor.Send(StringUtilities.HMark + "There is no such prog for you to set as the Availability Prog.");
			return;
		}

		if (!prog.MatchesParameters(new[] { FutureProgVariableTypes.Character }))
		{
			actor.Send(StringUtilities.HMark +
					   "Only progs that accept a single character parameter are eligible for Availability Progs.");
			return;
		}

		actor.CombatSettings.AvailabilityProg = prog;
		actor.CombatSettings.Changed = true;

		actor.Send(StringUtilities.HMark +
				   $"AvailProg for {actor.CombatSettings.Name} set to: {prog.FunctionName} (#{prog.Id}).");
	}

	private static void CombatConfigCharacter(ICharacter actor, StringStack command)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send(StringUtilities.HMark + "Permission denied.".Colour(Telnet.BoldRed));
			return;
		}

		if (command.IsFinished)
		{
			actor.Send(StringUtilities.HMark + "Syntax: combat config character <characterID>");
			return;
		}

		var target = actor.Gameworld.Actors.GetByName(command.PopSpeech()) ??
					 actor.Gameworld.Actors.GetFromItemListByKeyword(command.Last, actor);

		if (target == null)
		{
			actor.Send(StringUtilities.HMark + "Unable to find a character by that name.");
			return;
		}

		actor.CombatSettings.CharacterOwnerId = target.Id;
		actor.CombatSettings.Changed = true;

		actor.Send(StringUtilities.HMark +
				   $"{actor.CombatSettings.Name.Proper()} has been assigned to {target.Name.Proper()}.".Colour(
					   Telnet.BoldGreen));
	}

	private static void CombatConfigClassifications(ICharacter actor, StringStack command)
	{
		var classifications = Enum.GetValues(typeof(WeaponClassification)).OfType<WeaponClassification>().ToList();

		if (command.IsFinished)
		{
			actor.Send(StringUtilities.HMark + "Syntax: combat config classifications <classifications list>\n" +
					   StringUtilities.Indent + "Valid Classifications are: " +
					   classifications.OrderBy(x => (int)x)
									  .Select(x => x.Describe().Colour(Telnet.Green))
									  .ListToString() + ".");
			return;
		}

		if (command.Peek().Equals("none", StringComparison.InvariantCultureIgnoreCase))
		{
			actor.CombatSettings.ClearClassifications();
			actor.Send(StringUtilities.HMark +
					   $"Classifications for {actor.CombatSettings.Name} have been cleared. This setting won't use any weapons until new Classifications are added.");
			return;
		}

		actor.CombatSettings.ClearClassifications();
		while (!command.IsFinished)
		{
			var cmd = command.Pop();
			if (!classifications.Any(x => x.Describe().Equals(cmd, StringComparison.InvariantCultureIgnoreCase)))
			{
				actor.CombatSettings.ClearClassifications();
				actor.Send(StringUtilities.HMark +
						   $"Invalid Classification: {command.Last}. Valid choices are: " +
						   classifications.OrderBy(x => (int)x)
										  .Select(x => x.Describe().Colour(Telnet.Green))
										  .ListToString() + ".\n" +
						   StringUtilities.Indent +
						   "Classifications cleared. Must set new list before Settings are usable.");
				return;
			}

			actor.CombatSettings.AddClassification(classifications.First(x => x.Describe()
																			   .Equals(cmd,
																				   StringComparison
																					   .InvariantCultureIgnoreCase)));
		}

		actor.CombatSettings.Changed = true;
		actor.Send(StringUtilities.HMark +
				   $"{actor.CombatSettings.Name.Proper()} has had its Weapon Classifications updated.");
	}

	private static void CombatConfigMovement(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || command.Peek().EqualTo("help") || command.Peek().EqualTo("?"))
		{
			actor.Send(StringUtilities.HMark + $"You can choose to set your automatic combat movement management to " +
					   $"{new[] { "auto", "manual", "cover_only", "keep_range" }.Select(x => x.Colour(Telnet.Cyan)).ListToString(conjunction: "or ")}.");
			return;
		}

		AutomaticMovementSettings setting;
		switch (command.SafeRemainingArgument.ToLowerInvariant())
		{
			case "auto":
			case "automatic":
				setting = AutomaticMovementSettings.FullyAutomatic;
				break;
			case "manual":
				setting = AutomaticMovementSettings.FullyManual;
				break;
			case "cover":
			case "cover_only":
			case "cover only":
				setting = AutomaticMovementSettings.SeekCoverOnly;
				break;
			case "keep_range":
			case "keeprange":
			case "keep range":
				setting = AutomaticMovementSettings.KeepRange;
				break;
			default:
				actor.Send(StringUtilities.HMark +
						   $"You can choose to set your automatic combat movement management to " +
						   $"{new[] { "auto", "manual", "cover_only", "keep_range" }.Select(x => x.Colour(Telnet.Cyan)).ListToString(conjunction: "or ")}.");
				;
				return;
		}

		actor.CombatSettings.MovementManagement = setting;
		actor.CombatSettings.Changed = true;
		actor.Send(StringUtilities.HMark +
				   $"Your combat movement management has been set to {setting.Describe().Colour(Telnet.Green)}.");
	}

	private static void CombatConfigRanged(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || command.Peek().EqualTo("help") || command.Peek().EqualTo("?"))
		{
			actor.Send(StringUtilities.HMark + $"You can choose to set your automatic ranged combat management to " +
					   $"{new[] { "auto", "manual", "continue_only" }.Select(x => x.Colour(Telnet.Cyan)).ListToString(conjunction: "or ")}.");
			return;
		}

		AutomaticRangedSettings setting;
		switch (command.SafeRemainingArgument.ToLowerInvariant())
		{
			case "auto":
			case "automatic":
				setting = AutomaticRangedSettings.FullyAutomatic;
				break;
			case "manual":
				setting = AutomaticRangedSettings.FullyManual;
				break;
			case "continue":
			case "continue_only":
			case "continue only":
				setting = AutomaticRangedSettings.ContinueFiringOnly;
				break;
			default:
				actor.Send(StringUtilities.HMark +
						   $"You can choose to set your automatic ranged combat management to" +
						   $" {new[] { "auto", "manual", "continue_only" }.Select(x => x.Colour(Telnet.Cyan)).ListToString(conjunction: "or ")}.");
				return;
		}

		actor.CombatSettings.RangedManagement = setting;
		actor.CombatSettings.Changed = true;
		actor.Send(StringUtilities.HMark +
				   $"Your ranged combat management has been set to {setting.Describe().Colour(Telnet.Green)}.");
	}

	private static void CombatConfigInventory(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || command.Peek().EqualTo("help") || command.Peek().EqualTo("?"))
		{
			actor.Send(StringUtilities.HMark + $"You can choose to set your automatic inventory management to " +
					   $"{new[] { "auto", "retrieve_only", "no_discard", "manual" }.Select(x => x.Colour(Telnet.Cyan)).ListToString(conjunction: "or ")}.");
			return;
		}

		AutomaticInventorySettings setting;
		switch (command.SafeRemainingArgument.ToLowerInvariant())
		{
			case "auto":
			case "automatic":
				setting = AutomaticInventorySettings.FullyAutomatic;
				break;
			case "manual":
				setting = AutomaticInventorySettings.FullyManual;
				break;
			case "retrieve_only":
			case "retrieve":
			case "retrieve only":
				setting = AutomaticInventorySettings.RetrieveOnly;
				break;
			case "no_discard":
			case "no discard":
				setting = AutomaticInventorySettings.AutomaticButDontDiscard;
				break;
			default:
				actor.Send(StringUtilities.HMark + $"You can choose to set your automatic inventory management to " +
						   $"{new[] { "auto", "retrieve_only", "no_discard", "manual" }.Select(x => x.Colour(Telnet.Cyan)).ListToString(conjunction: "or ")}.");
				return;
		}

		actor.CombatSettings.InventoryManagement = setting;
		actor.CombatSettings.Changed = true;
		actor.Send(StringUtilities.HMark +
				   $"Your inventory management has been set to {setting.Describe().Colour(Telnet.Green)}.");
	}

	private static void CombatConfigPosition(ICharacter actor, StringStack command)
	{
		var truth = GetTruthValue(actor, command, actor.CombatSettings.ManualPositionManagement);
		if (truth == null)
		{
			return;
		}

		actor.CombatSettings.ManualPositionManagement = truth.Value;
		actor.CombatSettings.Changed = true;
		actor.Send(truth.Value
			? StringUtilities.HMark + "You will now manage your position manually."
			: StringUtilities.HMark + "You will allow some automated position management.");
	}

	private static void CombatConfigMeleeStrategy(ICharacter actor, StringStack command)
	{
		var meleeStrategies = Enum.GetValues(typeof(CombatStrategyMode)).OfType<CombatStrategyMode>()
								  .Where(x => x.IsMeleeStrategy())
								  .ToList();
		var validStrategyText = StringUtilities.GetTextTable(
			from item in meleeStrategies
			select new List<string>
			{
				item.DescribeEnum(),
				item.DescribeWordy()
			},
			new List<string>
			{
				"Name",
				"Description"
			},
			actor,
			Telnet.TextRed
		);

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which melee strategy do you want to set? The valid strategies are:\n{validStrategyText}",
				false);
			return;
		}

		var stratName = command.SafeRemainingArgument;
		if (!stratName.TryParseEnum<CombatStrategyMode>(out var value))
		{
			actor.OutputHandler.Send(
				$"That is not a valid melee strategy. The valid strategies are:\n{validStrategyText}",
				false);
			return;
		}

		actor.CombatSettings.PreferredMeleeMode = value;
		actor.CombatSettings.Changed = true;
		actor.OutputHandler.Send(
			$"Changed the melee strategy to {value.Describe().ColourName()} ({value.DescribeWordy()})");
	}

	private static void CombatConfigRangedStrategy(ICharacter actor, StringStack command)
	{
		var rangeStrategies = Enum.GetValues(typeof(CombatStrategyMode)).OfType<CombatStrategyMode>()
								  .Where(x => x.IsRangedStrategy())
								  .ToList();
		var validStrategyText = StringUtilities.GetTextTable(
			from item in rangeStrategies
			select new List<string>
			{
				item.DescribeEnum(),
				item.DescribeWordy()
			},
			new List<string>
			{
				"Name",
				"Description"
			},
			actor,
			Telnet.TextRed
		);

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which ranged strategy do you want to set? The valid strategies are:\n{validStrategyText}",
				false);
			return;
		}

		var stratName = command.SafeRemainingArgument;
		if (!stratName.TryParseEnum<CombatStrategyMode>(out var value))
		{
			actor.OutputHandler.Send(
				$"That is not a valid ranged strategy. The valid strategies are:\n{validStrategyText}",
				false);
			return;
		}

		actor.CombatSettings.PreferredRangedMode = value;
		actor.CombatSettings.Changed = true;
		actor.OutputHandler.Send(
			$"Changed the ranged strategy to {value.Describe().ColourName()} ({value.DescribeWordy()})");
	}

	private static void CombatConfigAutoMelee(ICharacter actor, StringStack command)
	{
		actor.CombatSettings.MoveToMeleeIfCannotEngageInRangedCombat =
			!actor.CombatSettings.MoveToMeleeIfCannotEngageInRangedCombat;
		actor.CombatSettings.Changed = true;
		actor.OutputHandler.Send(
			$"If this setting finds itself unable to use any ranged attacks, it will {(actor.CombatSettings.MoveToMeleeIfCannotEngageInRangedCombat ? "now" : "no longer")} automatically move to melee instead.");
	}

	private static void CombatConfigName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What new name do you want to give to this combat setting?");
			return;
		}

		var oldName = actor.CombatSettings.Name;
		actor.CombatSettings.SetName(command.SafeRemainingArgument.TitleCase());
		actor.Send(
			$"You rename the combat setting {oldName.Colour(Telnet.Cyan)} to {actor.CombatSettings.Name.Colour(Telnet.Cyan)}.");
	}

	private static void CombatConfigGrapple(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which response do you want to configure for grappling response? That valid responses are {Enum.GetValues(typeof(GrappleResponse)).OfType<GrappleResponse>().Select(x => $"{x.DescribeEnum().ColourValue()}").ListToString()}");
			return;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<GrappleResponse>(out var value))
		{
			actor.OutputHandler.Send(
				$"That is not a valid grappling response. That valid responses are {Enum.GetValues(typeof(GrappleResponse)).OfType<GrappleResponse>().Select(x => $"{x.DescribeEnum().ColourValue()}").ListToString()}");
			return;
		}

		actor.CombatSettings.GrappleResponse = value;
		actor.CombatSettings.Changed = true;
		actor.OutputHandler.Send($"You change the grapple response to {value.DescribeEnum(true).ColourValue()}.");
		return;
	}

	#endregion

	protected static void CombatPreferredDefense(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send($"Which defense type would you like to prefer?\nHint: use {"\"none\"".Colour(Telnet.Yellow)} to reset to no preference)");
			return;
		}

		if (command.SafeRemainingArgument.Equals("none", StringComparison.InvariantCultureIgnoreCase))
		{
			actor.PreferredDefenseType = DefenseType.None;
			actor.Send("You will no longer prefer any defense type, instead relying on your judgement.");
			return;
		}

		if (!CombatExtensions.TryParseDefenseType(command.SafeRemainingArgument, out var type))
		{
			actor.Send("There is no such defense type.");
			return;
		}

		actor.PreferredDefenseType = type;
		actor.OutputHandler.Send($"You will now prefer to {type.DescribeEnum().ColourValue()} in defense.");
	}

	protected static void CombatConfig(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || command.Peek().Equals("help", StringComparison.InvariantCultureIgnoreCase) ||
			command.Peek().Equals("?", StringComparison.InvariantCultureIgnoreCase))
		{
			CombatConfigHelp(actor);
			return;
		}

		if (actor.CombatSettings.GlobalTemplate && !actor.IsAdministrator())
		{
			actor.Send(
				"You cannot modify global settings. You should first clone your own copy if you want to fine tune it.");
			return;
		}

		if (actor.IsGuest)
		{
			actor.Send("Guests are not allowed to modify combat configurations.");
			return;
		}

		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "stamina":
			case "stam":
				CombatConfigStamina(actor, command);
				break;
			case "description":
			case "desc":
				CombatConfigDescription(actor, command);
				break;
			case "name":
				CombatConfigName(actor, command);
				break;
			case "global":
				CombatConfigGlobal(actor, command);
				break;
			case "availprog":
				CombatConfigAvailProg(actor, command);
				break;
			case "character":
				CombatConfigCharacter(actor, command);
				break;
			case "melee":
				CombatConfigMeleeStrategy(actor, command);
				break;
			case "range":
			case "ranged":
				CombatConfigRangedStrategy(actor, command);
				break;
			case "weapon":
				CombatConfigWeapon(actor, command);
				break;
			case "magic":
				CombatConfigMagic(actor, command);
				break;
			case "psychic":
				CombatConfigPsychic(actor, command);
				break;
			case "natural":
				CombatConfigNatural(actor, command);
				break;
			case "auxillary":
			case "auxiliary":
				CombatConfigAuxiliary(actor, command);
				break;
			case "prefer_defense":
			case "defense":
			case "prefer_defence":
			case "defence":
			case "prefer defense":
			case "prefer defence":
				CombatPreferredDefense(actor, command);
				break;
			case "prefer_armed":
			case "prefer armed":
			case "preferarmed":
				CombatConfigPreferArmed(actor, command);
				break;
			case "prefer_favourite":
			case "prefer favourite":
			case "preferfavourite":
			case "favourite":
			case "favorite":
			case "prefer_favorite":
			case "prefer favorite":
			case "preferfavorite":
				CombatConfigPreferFavourite(actor, command);
				break;
			case "prefershield":
			case "prefer shield":
			case "prefer_shield":
				CombatConfigPreferShield(actor, command);
				break;
			case "classifications":
				CombatConfigClassifications(actor, command);
				break;
			case "prefer_stagger":
			case "prefer stagger":
			case "preferstagger":
			case "stagger":
				CombatConfigPreferStagger(actor, command);
				break;
			case "fallback":
				CombatConfigFallback(actor, command);
				break;
			case "attack helpless":
			case "attack_helpless":
			case "attackhelpless":
			case "helpless":
				CombatConfigAttackHelpless(actor, command);
				break;
			case "attack_critical":
			case "attackcritical":
				CombatConfigAttackCritical(actor, command);
				break;
			case "skirmish":
				CombatConfigSkirmish(actor, command);
				break;
			case "require":
				CombatConfigRequire(actor, command);
				break;
			case "prefer":
				CombatConfigPrefer(actor, command);
				break;
			case "forbid":
				CombatConfigForbid(actor, command);
				break;
			case "pursue":
				CombatConfigPursue(actor, command);
				break;
			case "movetotarget":
				CombatConfigMoveToTarget(actor, command);
				break;
			case "auto_inventory":
			case "auto_inv":
				CombatConfigInventory(actor, command);
				break;
			case "auto_ranged":
			case "auto_range":
				CombatConfigRanged(actor, command);
				break;
			case "auto_move":
			case "auto_movement":
				CombatConfigMovement(actor, command);
				break;
			case "auto_position":
				CombatConfigPosition(actor, command);
				break;
			case "aim":
				CombatConfigAim(actor, command);
				break;
			case "auto_melee":
				CombatConfigAutoMelee(actor, command);
				break;
			case "setup":
				CombatConfigSetup(actor, command);
				break;
			case "swap":
			case "swaporder":
			case "swap_order":
			case "order":
				CombatConfigSwapOrder(actor, command);
				break;
			case "grapple":
			case "grappling":
				CombatConfigGrapple(actor, command);
				break;
			default:
				CombatConfigHelp(actor);
				return;
		}
	}

	#endregion
}
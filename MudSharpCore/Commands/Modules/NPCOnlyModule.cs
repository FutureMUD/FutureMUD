using System.Linq;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.NPC;
using MudSharp.NPC.AI;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Law;

namespace MudSharp.Commands.Modules;

internal class NPCOnlyModule : Module<ICharacter>
{
	private NPCOnlyModule()
		: base("NPC Only")
	{
		IsNecessary = true;
	}

	public static NPCOnlyModule Instance { get; } = new();

	[PlayerCommand("Pause", "pause")]
	protected static void Pause(ICharacter actor, string command)
	{
		if (actor.EffectsOfType<PauseAI>().Any())
		{
			actor.RemoveAllEffects(x => x.IsEffectType<PauseAI>());
			actor.Send("You resume any AI routines that you had.");
			return;
		}

		actor.AddEffect(new PauseAI(actor));
		actor.Send("You pause all of your AI routines until further notice.");
	}

	[PlayerCommand("Doorguard", "doorguard")]
	protected static void DoorGuard(ICharacter actor, string command)
	{
		if (actor.AffectedBy<IDoorguardModeEffect>())
		{
			actor.OutputHandler.Handle(
				new EmoteOutput(new Emote("@ are|is no longer acting as a door guard.", actor)));
			actor.RemoveAllEffects(x => x.IsEffectType<IDoorguardModeEffect>());
		}
		else
		{
			actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ are|is now acting as a door guard.", actor)));
			actor.AddEffect(new DoorguardMode(actor));
		}
	}

	[PlayerCommand("Enforcer", "enforcer")]
	protected static void Enforcer(ICharacter actor, string command)
	{
		if (actor.AffectedBy<EnforcerEffect>())
		{
			actor.RemoveAllEffects<EnforcerEffect>();
			actor.OutputHandler.Send("You are no longer in enforcer mode.");
			return;
		}

		var npc = (INPC)actor;
		var ss = new StringStack(command.RemoveFirstWord());
		if (!npc.AIs.Any(x => x is EnforcerAI))
		{
			actor.OutputHandler.Send("You do not have any enforcer AIs, and so cannot be an enforcer.");
			return;
		}

		var possibleLegalAuthorities = actor.Gameworld.LegalAuthorities
		                                    .Where(x => x.GetEnforcementAuthority(actor) != null).ToList();
		if (possibleLegalAuthorities.Count == 0)
		{
			actor.OutputHandler.Send("You are not an enforcer for any legal authorities.");
			return;
		}

		ILegalAuthority authority;
		if (possibleLegalAuthorities.Count > 1)
		{
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send(
					"You could be an enforcer for more than one legal authority. Please specify the legal authority that you want to go into enforcer mode for.");
				return;
			}

			authority = long.TryParse(ss.SafeRemainingArgument, out var value)
				? actor.Gameworld.LegalAuthorities.Get(value)
				: actor.Gameworld.LegalAuthorities.GetByName(ss.SafeRemainingArgument);
			if (authority == null || !possibleLegalAuthorities.Contains(authority))
			{
				actor.OutputHandler.Send("You are not an enforcer for any such legal authority.");
				return;
			}
		}
		else
		{
			authority = possibleLegalAuthorities.Single();
		}

		actor.AddEffect(new EnforcerEffect(actor, authority));
		actor.OutputHandler.Send(
			$"You are now in enforcer mode for the {authority.Name.ColourName()} legal authority.");
	}

	[PlayerCommand("Bodyguard", "bodyguard")]
	protected static void Bodyguard(ICharacter actor, string command)
	{
		var npc = (INPC)actor;
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			if (npc.BodyguardingCharacterID == null)
			{
				actor.OutputHandler.Send("You're not currently bodyguarding anyone.");
				return;
			}

			actor.OutputHandler.Send(
				$"You're currently bodyguarding {actor.Gameworld.TryGetCharacter(npc.BodyguardingCharacterID.Value).HowSeen(actor)}.");
			return;
		}

		ICharacter oldCh;
		if (ss.Peek().EqualToAny("none", "clear", "off"))
		{
			oldCh = actor.Gameworld.TryGetCharacter(npc.BodyguardingCharacterID ?? 0, true);
			oldCh?.OutputHandler?.Send($"{npc.HowSeen(oldCh, true)} is no longer bodyguarding you.");
			npc.BodyguardingCharacterID = null;
			actor.OutputHandler.Send("You're no longer bodyguarding anyone.");
			return;
		}

		var target = actor.TargetActor(ss.PopSpeech());
		if (target == null)
		{
			actor.OutputHandler.Send("You don't see anyone like that here.");
			return;
		}

		if (target == actor)
		{
			actor.OutputHandler.Send("You cannot bodyguard yourself.");
			return;
		}

		oldCh = actor.Gameworld.TryGetCharacter(npc.BodyguardingCharacterID ?? 0, true);
		oldCh?.OutputHandler?.Send($"{npc.HowSeen(oldCh, true)} is no longer bodyguarding you.");
		npc.BodyguardingCharacterID = target.Id;
		target.OutputHandler.Send($"{npc.HowSeen(target, true)} is now bodyguarding you.");
	}

	[PlayerCommand("IgnoreForce", "ignoreforce")]
	protected static void IgnoreForce(ICharacter actor, string command)
	{
		if (actor.AffectedBy<IgnoreForce>())
		{
			actor.RemoveAllEffects(x => x.IsEffectType<IgnoreForce>());
			actor.OutputHandler.Send("You are no longer ignoring FORCE commands.");
			return;
		}

		actor.AddEffect(new IgnoreForce(actor));
		actor.OutputHandler.Send("You are now ignoring FORCE commands.");
	}
}
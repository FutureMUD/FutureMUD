﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.Powers;

public class MindLookPower : MagicPowerBase
{
	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("mindlook", (power, gameworld) => new MindLookPower(power, gameworld));
	}

	protected MindLookPower(MagicPower power, IFuturemud gameworld) : base(power, gameworld)
	{
		var root = XElement.Parse(power.Definition);
		var element = root.Element("LookRoomVerb");
		if (element == null)
		{
			throw new ApplicationException($"The MindLookPower #{Id} ({Name}) was missing a LookRoomVerb element.");
		}

		LookRoomVerb = element.Value.ToLowerInvariant();

		element = root.Element("LookThingVerb");
		if (element == null)
		{
			throw new ApplicationException($"The MindLookPower #{Id} ({Name}) was missing a LookThingVerb element.");
		}

		LookThingVerb = element.Value.ToLowerInvariant();

		element = root.Element("LookInThingVerb");
		if (element == null)
		{
			throw new ApplicationException($"The MindLookPower #{Id} ({Name}) was missing a LookInThingVerb element.");
		}

		LookInThingVerb = element.Value.ToLowerInvariant();

		element = root.Element("EmoteText");
		if (element == null)
		{
			throw new ApplicationException($"The MindLookPower #{Id} ({Name}) was missing a EmoteText element.");
		}

		EmoteText = element.Value.ToLowerInvariant();

		element = root.Element("FailEmoteText");
		if (element == null)
		{
			throw new ApplicationException($"The MindLookPower #{Id} ({Name}) was missing a FailEmoteText element.");
		}

		FailEmoteText = element.Value.ToLowerInvariant();

		element = root.Element("SkillCheckDifficultyLookRoom");
		if (element == null)
		{
			throw new ApplicationException(
				$"The MindLookPower #{Id} ({Name}) was missing a SkillCheckDifficultyLookRoom element.");
		}

		if (!int.TryParse(element.Value, out var ivalue))
		{
			if (!CheckExtensions.GetDifficulty(element.Value, out var diff))
			{
				throw new ApplicationException(
					$"The MindLookPower #{Id} ({Name}) had a SkillCheckDifficultyLookRoom value that did not map to a valid Difficulty.");
			}

			SkillCheckDifficultyLookRoom = diff;
		}
		else
		{
			SkillCheckDifficultyLookRoom = (Difficulty)ivalue;
		}

		element = root.Element("SkillCheckDifficultyLookThing");
		if (element == null)
		{
			throw new ApplicationException(
				$"The MindLookPower #{Id} ({Name}) was missing a SkillCheckDifficultyLookThing element.");
		}

		if (!int.TryParse(element.Value, out ivalue))
		{
			if (!CheckExtensions.GetDifficulty(element.Value, out var diff))
			{
				throw new ApplicationException(
					$"The MindLookPower #{Id} ({Name}) had a SkillCheckDifficultyLookThing value that did not map to a valid Difficulty.");
			}

			SkillCheckDifficultyLookThing = diff;
		}
		else
		{
			SkillCheckDifficultyLookThing = (Difficulty)ivalue;
		}

		element = root.Element("SkillCheckDifficultyLookInThing");
		if (element == null)
		{
			throw new ApplicationException(
				$"The MindLookPower #{Id} ({Name}) was missing a SkillCheckDifficultyLookInThing element.");
		}

		if (!int.TryParse(element.Value, out ivalue))
		{
			if (!CheckExtensions.GetDifficulty(element.Value, out var diff))
			{
				throw new ApplicationException(
					$"The MindLookPower #{Id} ({Name}) had a SkillCheckDifficultyLookInThing value that did not map to a valid Difficulty.");
			}

			SkillCheckDifficultyLookInThing = diff;
		}
		else
		{
			SkillCheckDifficultyLookInThing = (Difficulty)ivalue;
		}

		element = root.Element("MinimumSuccessThresholdLookRoom");
		if (element == null)
		{
			throw new ApplicationException(
				$"The MindLookPower #{Id} ({Name}) was missing a MinimumSuccessThresholdLookRoom element.");
		}

		if (!int.TryParse(element.Value, out ivalue))
		{
			if (!CheckExtensions.GetOutcome(element.Value, out var outcome))
			{
				throw new ApplicationException(
					$"The MindLookPower #{Id} ({Name}) had a MinimumSuccessThresholdLookRoom value that did not map to a valid Outcome.");
			}

			MinimumSuccessThresholdLookRoom = outcome;
		}
		else
		{
			MinimumSuccessThresholdLookRoom = (Outcome)ivalue;
		}

		element = root.Element("MinimumSuccessThresholdLookThing");
		if (element == null)
		{
			throw new ApplicationException(
				$"The MindLookPower #{Id} ({Name}) was missing a MinimumSuccessThresholdLookThing element.");
		}

		if (!int.TryParse(element.Value, out ivalue))
		{
			if (!CheckExtensions.GetOutcome(element.Value, out var outcome))
			{
				throw new ApplicationException(
					$"The MindLookPower #{Id} ({Name}) had a MinimumSuccessThresholdLookThing value that did not map to a valid Outcome.");
			}

			MinimumSuccessThresholdLookThing = outcome;
		}
		else
		{
			MinimumSuccessThresholdLookThing = (Outcome)ivalue;
		}

		element = root.Element("MinimumSuccessThresholdLookInThing");
		if (element == null)
		{
			throw new ApplicationException(
				$"The MindLookPower #{Id} ({Name}) was missing a MinimumSuccessThresholdLookInThing element.");
		}

		if (!int.TryParse(element.Value, out ivalue))
		{
			if (!CheckExtensions.GetOutcome(element.Value, out var outcome))
			{
				throw new ApplicationException(
					$"The MindLookPower #{Id} ({Name}) had a MinimumSuccessThresholdLookInThing value that did not map to a valid Outcome.");
			}

			MinimumSuccessThresholdLookInThing = outcome;
		}
		else
		{
			MinimumSuccessThresholdLookInThing = (Outcome)ivalue;
		}

		element = root.Element("SkillCheckTrait");
		if (element == null)
		{
			throw new ApplicationException($"The MindLookPower #{Id} ({Name}) was missing a SkillCheckTrait element.");
		}

		var trait = long.TryParse(element.Value, out var value)
			? Gameworld.Traits.Get(value)
			: Gameworld.Traits.GetByName(element.Value);

		SkillCheckTrait = trait ?? throw new ApplicationException(
			$"The MindLookPower #{Id} ({Name}) had a SkillCheckTrait element that pointed to a null Trait.");
	}

	private enum UseCommandVerb
	{
		Room,
		Thing,
		InThing
	}

	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		var (truth, missing) = CanAffordToInvokePower(actor, verb);
		if (!truth)
		{
			actor.OutputHandler.Send(
				$"You can't do that because you lack sufficient {missing.Name.Colour(Telnet.BoldMagenta)}.");
			return;
		}

		var effect = actor.EffectsOfType<ConnectMindEffect>().FirstOrDefault(x => x.School == School);
		if (effect == null)
		{
			actor.OutputHandler.Send(
				$"Your mind isn't connected to the mind of anyone else via {School.Name.Colour(School.PowerListColour)}.");
			return;
		}

		;

		if ((bool?)CanInvokePowerProg.Execute(actor, effect.TargetCharacter) == false)
		{
			actor.OutputHandler.Send(string.Format(
				WhyCantInvokePowerProg.Execute(actor, effect.TargetCharacter)?.ToString() ??
				"You cannot send any messages to {0}.", effect.TargetCharacter.HowSeen(actor)));
			return;
		}

		var check = Gameworld.GetCheck(CheckType.MindLookPower);
		Difficulty difficulty;
		Outcome threshold;
		UseCommandVerb everb;
		if (verb.EqualTo(LookRoomVerb))
		{
			difficulty = SkillCheckDifficultyLookRoom;
			threshold = MinimumSuccessThresholdLookRoom;
			everb = UseCommandVerb.Room;
		}
		else if (verb.EqualTo(LookThingVerb))
		{
			difficulty = SkillCheckDifficultyLookThing;
			threshold = MinimumSuccessThresholdLookThing;
			everb = UseCommandVerb.Thing;
		}
		else
		{
			difficulty = SkillCheckDifficultyLookInThing;
			threshold = MinimumSuccessThresholdLookInThing;
			everb = UseCommandVerb.InThing;
		}

		var outcome = check.Check(actor, difficulty, SkillCheckTrait, effect.TargetCharacter);
		if (outcome < threshold)
		{
			actor.OutputHandler.Send(new EmoteOutput(new Emote(FailEmoteText, actor, actor, effect.TargetCharacter)));
			return;
		}

		switch (everb)
		{
			case UseCommandVerb.Room:
				if (!string.IsNullOrEmpty(EmoteText))
				{
					actor.OutputHandler.Send(
						new EmoteOutput(new Emote(EmoteText, actor, actor, effect.TargetCharacter)));
				}

				actor.OutputHandler.Send(effect.TargetCharacter.Body.LookText(false), false, true);
				return;
			default:
				if (command.IsFinished)
				{
					actor.OutputHandler.Send("You must specify a target thing to use that power on.");
					return;
				}

				var target = effect.TargetCharacter.Target(command.PopSpeech());
				if (target == null)
				{
					actor.OutputHandler.Send("You cannot see anything like that which your target can also see.");
					return;
				}

				if (!string.IsNullOrEmpty(EmoteText))
				{
					actor.OutputHandler.Send(
						new EmoteOutput(new Emote(EmoteText, actor, actor, effect.TargetCharacter)));
				}

				if (everb == UseCommandVerb.InThing)
				{
					actor.OutputHandler.Send(effect.TargetCharacter.Body.LookInText(target), true, false);
					return;
				}

				actor.OutputHandler.Send(effect.TargetCharacter.Body.LookText(target), true, true);
				break;
		}
	}

	public override IEnumerable<string> Verbs => new[]
	{
		LookRoomVerb,
		LookThingVerb,
		LookInThingVerb
	};

	public string LookRoomVerb { get; protected set; }
	public string LookThingVerb { get; protected set; }
	public string LookInThingVerb { get; protected set; }

	public Difficulty SkillCheckDifficultyLookRoom { get; protected set; }
	public Difficulty SkillCheckDifficultyLookThing { get; protected set; }
	public Difficulty SkillCheckDifficultyLookInThing { get; protected set; }
	public ITraitDefinition SkillCheckTrait { get; protected set; }
	public Outcome MinimumSuccessThresholdLookRoom { get; protected set; }
	public Outcome MinimumSuccessThresholdLookThing { get; protected set; }
	public Outcome MinimumSuccessThresholdLookInThing { get; protected set; }

	public string EmoteText { get; set; }
	public string FailEmoteText { get; set; }
}
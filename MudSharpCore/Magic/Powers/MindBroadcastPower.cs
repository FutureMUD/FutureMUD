using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Communication.Language;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.Powers;

public class MindBroadcastPower : MagicPowerBase
{
	public override string PowerType => "Mind Broadcast";
	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("mindbroadcast",
			(power, gameworld) => new MindBroadcastPower(power, gameworld));
	}

	protected MindBroadcastPower(MagicPower power, IFuturemud gameworld) : base(power, gameworld)
	{
		var root = XElement.Parse(power.Definition);
		var element = root.Element("UnknownIdentityDescription");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no UnknownIdentityDescription in the definition XML for power {Id} ({Name}).");
		}

		UnknownIdentityDescription = element.Value;

		element = root.Element("TargetCanSeeIdentityProg");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no TargetCanSeeIdentityProg in the definition XML for power {Id} ({Name}).");
		}

		TargetCanSeeIdentityProg = long.TryParse(element.Value, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(element.Value);

		element = root.Element("Verb");
		if (element == null)
		{
			throw new ApplicationException($"There was no Verb in the definition XML for power {Id} ({Name}).");
		}

		Verb = element.Value;

		element = root.Element("UseLanguage");
		if (element == null)
		{
			throw new ApplicationException($"There was no UseLanguage in the definition XML for power {Id} ({Name}).");
		}

		UseLanguage = bool.Parse(element.Value);

		element = root.Element("EmoteText");
		if (element == null)
		{
			throw new ApplicationException($"There was no EmoteText in the definition XML for power {Id} ({Name}).");
		}

		EmoteText = element.Value;

		element = root.Element("FailEmoteText");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no FailEmoteText in the definition XML for power {Id} ({Name}).");
		}

		FailEmoteText = element.Value;

		element = root.Element("TargetEmoteText");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no TargetEmoteText in the definition XML for power {Id} ({Name}).");
		}

		TargetEmoteText = element.Value;

		element = root.Element("SkillCheckDifficulty");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no SkillCheckDifficulty in the definition XML for power {Id} ({Name}).");
		}

		SkillCheckDifficulty = (Difficulty)int.Parse(element.Value);

		element = root.Element("SkillCheckTrait");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no SkillCheckTrait in the definition XML for power {Id} ({Name}).");
		}

		SkillCheckTrait = Gameworld.Traits.Get(long.Parse(element.Value));

		element = root.Element("MinimumSuccessThreshold");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no MinimumSuccessThreshold in the definition XML for power {Id} ({Name}).");
		}

		MinimumSuccessThreshold = (Outcome)int.Parse(element.Value);

		element = root.Element("UseAccent");
		if (element == null)
		{
			throw new ApplicationException($"There was no UseAccent in the definition XML for power {Id} ({Name}).");
		}

		UseAccent = bool.Parse(element.Value);

		element = root.Element("TargetIncluded");
		if (element != null)
		{
			TargetIncluded = long.TryParse(element.Value, out value)
				? Gameworld.FutureProgs.Get(value)
				: Gameworld.FutureProgs.GetByName(element.Value);

			if (TargetIncluded == null)
			{
				throw new ApplicationException(
					$"The TargetIncluded specified an invalid prog in the definition XML for power {Id} ({Name}).");
			}

			if (TargetIncluded.ReturnType != FutureProgVariableTypes.Boolean)
			{
				throw new ApplicationException(
					$"The TargetIncluded specified a prog that doesn't return boolean in the definition XML for power {Id} ({Name}).");
			}

			if (!TargetIncluded.MatchesParameters(new[] { FutureProgVariableTypes.Character }))
			{
				throw new ApplicationException(
					$"The TargetIncluded specified  a prog that does not match the required parameter pattern of a single character in the definition XML for power {Id} ({Name}).");
			}
		}
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

		if ((bool?)CanInvokePowerProg.Execute(actor) == false)
		{
			actor.OutputHandler.Send(WhyCantInvokePowerProg.Execute(actor)?.ToString() ??
			                         "You cannot send any broadcasts.");
			return;
		}

		var check = Gameworld.GetCheck(CheckType.MindBroadcastPower);
		var outcome = check.Check(actor, SkillCheckDifficulty, SkillCheckTrait);
		if (outcome < MinimumSuccessThreshold)
		{
			actor.OutputHandler.Send(new EmoteOutput(new Emote(FailEmoteText, actor, actor)));
			return;
		}

		var text = command.RemainingArgument;
		if (UseLanguage)
		{
			var langInfo = new PsychicLanguageInfo(actor.CurrentLanguage, UseAccent ? actor.CurrentAccent : null,
				text, outcome, actor);
			foreach (var target in actor.Location.Room.Characters.Except(actor).ToList())
			{
				if (TargetIncluded?.Execute<bool?>(target) == false)
				{
					continue;
				}

				var emote = new LanguageOutput(
					new Emote(GetAppropriateTargetEmote(actor, target), actor, actor, target), langInfo, null);
				target.OutputHandler.Send(emote);
			}

			actor.OutputHandler.Send(new EmoteOutput(new Emote(
				string.Format(EmoteText, text).SubstituteANSIColour().ProperSentences().Fullstop(), actor, actor)));
		}
		else
		{
			foreach (var target in actor.Location.Room.Characters.Except(actor).ToList())
			{
				if (TargetIncluded?.Execute<bool?>(target) == false)
				{
					continue;
				}

				var emote = new EmoteOutput(new Emote(
					string.Format(GetAppropriateTargetEmote(actor, target), 0,
						command.RemainingArgument.ProperSentences().Fullstop()), actor, actor, target));
				target.OutputHandler.Send(emote);
			}

			actor.OutputHandler.Send(new EmoteOutput(new Emote(
				string.Format(EmoteText, text.SubstituteANSIColour().ProperSentences().Fullstop()), actor, actor)));
		}
	}

	public override IEnumerable<string> Verbs => new[] { Verb };

	public string Verb { get; set; }
	public bool UseLanguage { get; set; }
	public string EmoteText { get; set; }
	public string FailEmoteText { get; set; }
	public string TargetEmoteText { get; set; }
	public IFutureProg TargetCanSeeIdentityProg { get; protected set; }
	public string UnknownIdentityDescription { get; protected set; }
	public Difficulty SkillCheckDifficulty { get; protected set; }
	public ITraitDefinition SkillCheckTrait { get; protected set; }
	public Outcome MinimumSuccessThreshold { get; protected set; }
	public bool UseAccent { get; protected set; }
	public IFutureProg TargetIncluded { get; protected set; }

	public string GetAppropriateHowSeen(ICharacter connecter, ICharacter connectee)
	{
		if ((bool?)TargetCanSeeIdentityProg.Execute(connecter, connectee) == true)
		{
			return connecter.HowSeen(connectee, flags: PerceiveIgnoreFlags.IgnoreConsciousness);
		}

		return UnknownIdentityDescription.ColourCharacter();
	}

	public string GetAppropriateTargetEmote(ICharacter connecter, ICharacter connectee)
	{
		if ((bool?)TargetCanSeeIdentityProg.Execute(connecter, connectee) == true)
		{
			return string.Format(TargetEmoteText, "$0");
		}

		return string.Format(TargetEmoteText, UnknownIdentityDescription.ColourCharacter());
	}
}
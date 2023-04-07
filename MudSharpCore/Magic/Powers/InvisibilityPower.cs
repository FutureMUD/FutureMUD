using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body.Traits;

namespace MudSharp.Magic.Powers;

public class InvisibilityPower : SustainedMagicPower
{
	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("invisibility", (power, gameworld) => new InvisibilityPower(power, gameworld));
	}

	public InvisibilityPower(Models.MagicPower power, IFuturemud gameworld) : base(power, gameworld)
	{
		var root = XElement.Parse(power.Definition);
		var element = root.Element("StartPowerVerb");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no StartPowerVerb in the definition XML for power {Id} ({Name}).");
		}

		StartPowerVerb = element.Value;

		element = root.Element("EndPowerVerb");
		if (element == null)
		{
			throw new ApplicationException($"There was no EndPowerVerb in the definition XML for power {Id} ({Name}).");
		}

		EndPowerVerb = element.Value;

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

		element = root.Element("EndPowerEmoteText");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no EndPowerEmoteText in the definition XML for power {Id} ({Name}).");
		}

		EndPowerEmoteText = element.Value;

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

		element = root.Element("InvisibilityAppliesProg");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no InvisibilityAppliesProg in the definition XML for power {Id} ({Name}).");
		}

		InvisibilityAppliesProg = long.TryParse(element.Value, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(element.Value);

		element = root.Element("CanEndPowerProg");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no CanEndPowerProg in the definition XML for power {Id} ({Name}).");
		}

		CanEndPowerProg = long.TryParse(element.Value, out value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(element.Value);

		element = root.Element("WhyCantEndPowerProg");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no WhyCantEndPowerProg in the definition XML for power {Id} ({Name}).");
		}

		WhyCantEndPowerProg = long.TryParse(element.Value, out value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(element.Value);

		element = root.Element("PerceptionTypes");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no PerceptionTypes in the definition XML for power {Id} ({Name}).");
		}

		PerceptionTypes = (PerceptionTypes)long.Parse(element.Value);
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

		if (verb.EqualTo(EndPowerVerb))
		{
			if (!actor.AffectedBy<MagicInvisibility>(this))
			{
				actor.OutputHandler.Send("You are not currently using that power.");
				return;
			}

			if (CanEndPowerProg?.Execute<bool?>(actor) == false)
			{
				actor.OutputHandler.Send(WhyCantEndPowerProg?.Execute<string>(actor) ?? "You can't do that right now.");
				return;
			}

			actor.RemoveAllEffects<MagicInvisibility>(x => x.InvisibilityPower == this, true);
			actor.OutputHandler.Handle(new EmoteOutput(new Emote(EndPowerEmoteText, actor, actor)));
			ConsumePowerCosts(actor, verb);
			return;
		}

		if (actor.AffectedBy<MagicInvisibility>(this))
		{
			actor.OutputHandler.Send("You are already using that power.");
			return;
		}

		if (CanInvokePowerProg?.Execute<bool?>(actor) == false)
		{
			actor.OutputHandler.Send(WhyCantInvokePowerProg?.Execute<string>(actor) ?? "You cannot use that power.");
			return;
		}

		var check = Gameworld.GetCheck(CheckType.InvisibilityPower);
		var outcome = check.Check(actor, SkillCheckDifficulty, SkillCheckTrait, null);
		if (outcome < MinimumSuccessThreshold)
		{
			actor.OutputHandler.Handle(new EmoteOutput(new Emote(FailEmoteText, actor, actor)));
			return;
		}

		actor.OutputHandler.Handle(new EmoteOutput(new Emote(EmoteText, actor, actor)));
		actor.AddEffect(new MagicInvisibility(actor, this), GetDuration(outcome.SuccessDegrees()));
		ConsumePowerCosts(actor, verb);
	}

	#region Overrides of SustainedMagicPower

	protected override void ExpireSustainedEffect(ICharacter actor)
	{
		actor.RemoveAllEffects<MagicInvisibility>(x => x.InvisibilityPower == this, true);
		actor.OutputHandler.Handle(new EmoteOutput(new Emote(EndPowerEmoteText, actor, actor)));
	}

	#endregion

	public string EndPowerVerb { get; protected set; }

	public string StartPowerVerb { get; protected set; }

	public override IEnumerable<string> Verbs => new[] { EndPowerVerb, StartPowerVerb };

	public IFutureProg InvisibilityAppliesProg { get; protected set; }
	public IFutureProg CanEndPowerProg { get; protected set; }
	public IFutureProg WhyCantEndPowerProg { get; protected set; }

	public string EmoteText { get; protected set; }
	public string FailEmoteText { get; protected set; }
	public string EndPowerEmoteText { get; protected set; }
	public Difficulty SkillCheckDifficulty { get; protected set; }
	public ITraitDefinition SkillCheckTrait { get; protected set; }
	public Outcome MinimumSuccessThreshold { get; protected set; }
	public PerceptionTypes PerceptionTypes { get; protected set; }
}
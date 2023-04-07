using System;
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
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.Powers;

public partial class TelepathyPower : SustainedMagicPower
{
	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("telepathy", (power, gameworld) => new TelepathyPower(power, gameworld));
	}

	protected TelepathyPower(MagicPower power, IFuturemud gameworld) : base(power, gameworld)
	{
		var root = XElement.Parse(power.Definition);
		var element = root.Element("ShowFeels");
		if (element == null)
		{
			throw new ApplicationException($"There was no ShowFeels in the definition XML for power {Id} ({Name}).");
		}

		ShowFeels = bool.Parse(element.Value);

		element = root.Element("ShowThinks");
		if (element == null)
		{
			throw new ApplicationException($"There was no ShowThinks in the definition XML for power {Id} ({Name}).");
		}

		ShowThinks = bool.Parse(element.Value);

		element = root.Element("ShowThinkEmote");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no ShowThinkEmote in the definition XML for power {Id} ({Name}).");
		}

		ShowThinkEmote = bool.Parse(element.Value);

		element = root.Element("ShowThinkerDescription");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no ShowThinkerDescription in the definition XML for power {Id} ({Name}).");
		}

		ShowThinkerDescription = long.TryParse(element.Value, out var progid)
			? Gameworld.FutureProgs.Get(progid)
			: Gameworld.FutureProgs.GetByName(element.Value);

		element = root.Element("Distance");
		if (element == null)
		{
			throw new ApplicationException($"There was no Distance in the definition XML for power {Id} ({Name}).");
		}

		Distance = int.TryParse(element.Value, out var value)
			? (MagicPowerDistance)value
			: (MagicPowerDistance)Enum.Parse(typeof(MagicPowerDistance), element.Value);

		element = root.Element("Verb");
		if (element == null)
		{
			throw new ApplicationException($"There was no Verb in the definition XML for power {Id} ({Name}).");
		}

		Verb = element.Value;

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

		element = root.Element("BeginEmoteText");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no BeginEmoteText in the definition XML for power {Id} ({Name}).");
		}

		BeginEmoteText = element.Value;

		element = root.Element("EndEmoteText");
		if (element == null)
		{
			throw new ApplicationException($"There was no EndEmoteText in the definition XML for power {Id} ({Name}).");
		}

		EndEmoteText = element.Value;
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

		if ((bool?)CanInvokePowerProg?.Execute(actor) == false)
		{
			actor.OutputHandler.Send(WhyCantInvokePowerProg.Execute(actor).ToString());
			return;
		}

		if (actor.EffectsOfType<MagicTelepathyEffect>().Any(x => x.TelepathyPower == this))
		{
			actor.RemoveAllEffects(x => x.GetSubtype<MagicTelepathyEffect>()?.TelepathyPower == this, true);
			actor.OutputHandler.Send(new EmoteOutput(new Emote(EndEmoteText, actor, actor)));
			return;
		}

		var check = Gameworld.GetCheck(CheckType.MagicTelepathyCheck);
		var result = check.Check(actor, SkillCheckDifficulty, SkillCheckTrait, null);

		actor.AddEffect(new MagicTelepathyEffect(actor, this), GetDuration(result.SuccessDegrees()));
		actor.OutputHandler.Send(new EmoteOutput(new Emote(BeginEmoteText, actor, actor)));
		ConsumePowerCosts(actor, verb);
	}

	#region Overrides of SustainedMagicPower

	protected override void ExpireSustainedEffect(ICharacter actor)
	{
		actor.RemoveAllEffects(x => x.GetSubtype<MagicTelepathyEffect>()?.TelepathyPower == this, true);
	}

	#endregion

	public override IEnumerable<string> Verbs => new[] { Verb };

	public bool ShowFeels { get; protected set; }
	public bool ShowThinks { get; protected set; }
	public bool ShowThinkEmote { get; protected set; }
	public IFutureProg ShowThinkerDescription { get; protected set; }
	public MagicPowerDistance Distance { get; protected set; }
	public string Verb { get; protected set; }

	public Difficulty SkillCheckDifficulty { get; protected set; }
	public ITraitDefinition SkillCheckTrait { get; protected set; }
	public Outcome MinimumSuccessThreshold { get; protected set; }

	public string BeginEmoteText { get; protected set; }
	public string EndEmoteText { get; protected set; }
}
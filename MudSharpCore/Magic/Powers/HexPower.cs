#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Models;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.Magic.Powers;

public sealed class HexPower : PsionicTargetedPowerBase
{
	public override string PowerType => "Hex";
	public override string DatabaseType => "hex";
	protected override string DefaultVerb => "hex";

	public TimeSpan Duration { get; private set; }
	public double Penalty { get; private set; }
	public PsionicHexCheckCategory Categories { get; private set; }
	public bool ReplaceExisting { get; private set; }
	public string TargetEcho { get; private set; }

	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("hex", (power, gameworld) => new HexPower(power, gameworld));
		MagicPowerFactory.RegisterBuilderLoader("hex", BuilderLoader);
	}

	private static IMagicPower? BuilderLoader(IFuturemud gameworld, IMagicSchool school, string name, ICharacter actor,
		StringStack command)
	{
		return PsionicV4PowerBuilderHelpers.BuildWithSkill(gameworld, school, name, actor, command,
			trait => new HexPower(gameworld, school, name, trait));
	}

	private HexPower(IFuturemud gameworld, IMagicSchool school, string name, ITraitDefinition trait) :
		base(gameworld, school, name, trait)
	{
		Blurb = "Curse a target with broad skill check penalties";
		_showHelpText =
			$"Use {school.SchoolVerb.ToUpperInvariant()} HEX <target> to curse a target with a configurable skill-check penalty.";
		PowerDistance = MagicPowerDistance.AnyConnectedMindOrConnectedTo;
		SkillCheckDifficulty = Difficulty.Normal;
		MinimumSuccessThreshold = Outcome.MinorPass;
		Duration = TimeSpan.FromMinutes(10);
		Penalty = Gameworld.GetStaticDouble("CheckBonusPerDifficultyLevel") * 1.0;
		Categories = PsionicHexCheckCategory.General | PsionicHexCheckCategory.TargetedHostile |
		             PsionicHexCheckCategory.OffensiveCombat | PsionicHexCheckCategory.DefensiveCombat;
		ReplaceExisting = true;
		FailEcho = "You reach for $1's fortune, but your malice slips away.";
		SuccessEcho = "@ narrow|narrows &0's eyes as a hostile psychic pressure settles over $1.";
		TargetEcho = "A hostile psychic pressure settles over you.";
		DoDatabaseInsert();
	}

	private HexPower(MagicPower power, IFuturemud gameworld) : base(power, gameworld)
	{
		var root = XElement.Parse(power.Definition);
		Duration = TimeSpan.FromSeconds(double.Parse(root.Element("DurationSeconds")?.Value ?? "600"));
		Penalty = double.Parse(root.Element("Penalty")?.Value ??
		                       Gameworld.GetStaticDouble("CheckBonusPerDifficultyLevel").ToString());
		Categories = Enum.Parse<PsionicHexCheckCategory>(root.Element("Categories")?.Value ??
		                                                 PsionicHexCheckCategory.All.ToString(), true);
		ReplaceExisting = bool.Parse(root.Element("ReplaceExisting")?.Value ?? "true");
		TargetEcho = root.Element("TargetEcho")?.Value ?? "A hostile psychic pressure settles over you.";
	}

	protected override XElement SaveDefinition()
	{
		return SaveTargetedDefinition(
			new XElement("DurationSeconds", Duration.TotalSeconds),
			new XElement("Penalty", Penalty),
			new XElement("Categories", Categories),
			new XElement("ReplaceExisting", ReplaceExisting),
			new XElement("TargetEcho", new XCData(TargetEcho))
		);
	}

	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		if (!TryPrepareTarget(actor, command, "Whom do you want to hex?", out var target) || target is null)
		{
			return;
		}

		if (!PsionicTrafficHelper.CanReceiveInvoluntaryMentalTraffic(target))
		{
			actor.OutputHandler.Send($"{target.HowSeen(actor, true)} refuses involuntary mental traffic.");
			return;
		}

		var outcome = CheckPower(actor, target, CheckType.HexPower);
		if (outcome < MinimumSuccessThreshold)
		{
			SendFailure(actor, target);
			return;
		}

		if (ReplaceExisting)
		{
			target.RemoveAllEffects<MagicHexEffect>(x => x.Power == this, true);
		}

		var effect = new MagicHexEffect(target, this, Penalty, Categories);
		target.AddEffect(effect, Duration);
		if (!string.IsNullOrWhiteSpace(SuccessEcho))
		{
			actor.OutputHandler.Send(new EmoteOutput(new Emote(SuccessEcho, actor, actor, target)));
		}

		if (!string.IsNullOrWhiteSpace(TargetEcho))
		{
			target.OutputHandler.Send(new EmoteOutput(new Emote(TargetEcho, target, actor, target)));
		}

		PsionicActivityNotifier.Notify(actor, this, "a hostile psychic curse");
		ConsumePowerCosts(actor, Verb);
	}

	protected override void ShowSubtypeDetails(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Duration: {Duration.Describe(actor).ColourValue()}");
		sb.AppendLine($"Penalty: {(-Penalty).ToBonusString(actor)}");
		sb.AppendLine($"Categories: {Categories.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Replace Existing: {ReplaceExisting.ToColouredString()}");
		sb.AppendLine($"Target Echo: {TargetEcho.ColourCommand()}");
	}

	protected override string SubtypeHelpText => $@"{base.SubtypeHelpText}
	#3duration <timespan|seconds>#0 - sets how long the hex lasts
	#3penalty <number>#0 - sets the positive check penalty amount
	#3categories <flags>#0 - sets affected check categories
	#3replace#0 - toggles replacing existing hexes from this power
	#3targetecho <emote|none>#0 - sets the target-only echo";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "duration":
				return BuildingCommandDuration(actor, command);
			case "penalty":
				return BuildingCommandPenalty(actor, command);
			case "category":
			case "categories":
				return BuildingCommandCategories(actor, command);
			case "replace":
				ReplaceExisting = !ReplaceExisting;
				Changed = true;
				actor.OutputHandler.Send($"This hex will {ReplaceExisting.NowNoLonger()} replace existing instances from the same power.");
				return true;
			case "targetecho":
				return BuildingCommandTargetEcho(actor, command);
		}

		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandDuration(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How long should the hex last?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var seconds))
		{
			if (!TimeSpan.TryParse(command.SafeRemainingArgument, actor, out var parsed))
			{
				actor.OutputHandler.Send("That is not a valid number of seconds or timespan.");
				return false;
			}

			Duration = parsed;
		}
		else
		{
			Duration = TimeSpan.FromSeconds(seconds);
		}

		if (Duration <= TimeSpan.Zero)
		{
			actor.OutputHandler.Send("The duration must be positive.");
			return false;
		}

		Changed = true;
		actor.OutputHandler.Send($"This hex now lasts {Duration.Describe(actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandPenalty(ICharacter actor, StringStack command)
	{
		if (!double.TryParse(command.SafeRemainingArgument, out var value) || value < 0.0)
		{
			actor.OutputHandler.Send("You must enter a positive number for the check penalty.");
			return false;
		}

		Penalty = value;
		Changed = true;
		actor.OutputHandler.Send($"This hex now applies a penalty of {(-Penalty).ToBonusString(actor)}.");
		return true;
	}

	private bool BuildingCommandCategories(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !command.SafeRemainingArgument.TryParseEnum(out PsionicHexCheckCategory value))
		{
			actor.OutputHandler.Send(
				$"Valid categories are {Enum.GetValues<PsionicHexCheckCategory>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		Categories = value;
		Changed = true;
		actor.OutputHandler.Send($"This hex now affects {Categories.DescribeEnum().ColourValue()} checks.");
		return true;
	}

	private bool BuildingCommandTargetEcho(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || command.SafeRemainingArgument.EqualToAny("none", "clear", "delete"))
		{
			TargetEcho = string.Empty;
			Changed = true;
			actor.OutputHandler.Send("This hex no longer has a target echo.");
			return true;
		}

		var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable(),
			new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		TargetEcho = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"The target echo is now {TargetEcho.ColourCommand()}.");
		return true;
	}
}

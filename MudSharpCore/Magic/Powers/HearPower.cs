#nullable enable

using MudSharp.Body.Needs;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.Magic.Powers;

public sealed class HearPower : PsionicSustainedSelfPowerBase
{
	public override string PowerType => "Hear";
	public override string DatabaseType => "hear";
	protected override string DefaultBeginVerb => "hear";
	protected override string DefaultEndVerb => "endhear";

	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("hear", (power, gameworld) => new HearPower(power, gameworld));
		MagicPowerFactory.RegisterBuilderLoader("hear", BuilderLoader);
	}

	private static IMagicPower? BuilderLoader(IFuturemud gameworld, IMagicSchool school, string name, ICharacter actor,
		StringStack command)
	{
		return PsionicV4PowerBuilderHelpers.BuildWithSkill(gameworld, school, name, actor, command,
			trait => new HearPower(gameworld, school, name, trait));
	}

	private HearPower(IFuturemud gameworld, IMagicSchool school, string name, ITraitDefinition trait) : base(gameworld, school, name, trait)
	{
		Blurb = "Listen to psionic thought and emotion traffic";
		_showHelpText = $"Use {school.SchoolVerb.ToUpperInvariant()} HEAR to listen for thought and feeling traffic. This does not hear ordinary room audio.";
		PowerDistance = MagicPowerDistance.AnyConnectedMindOrConnectedTo;
		ShowThinks = true;
		ShowFeels = true;
		ShowName = false;
		ShowEmotes = true;
		ShowDescriptionProg = Gameworld.AlwaysTrueProg;
		DoDatabaseInsert();
	}

	private HearPower(MagicPower power, IFuturemud gameworld) : base(power, gameworld)
	{
		var root = XElement.Parse(power.Definition);
		PowerDistance = Enum.Parse<MagicPowerDistance>(root.Element("PowerDistance")?.Value ?? nameof(MagicPowerDistance.AnyConnectedMindOrConnectedTo), true);
		ShowThinks = bool.Parse(root.Element("ShowThinks")?.Value ?? "true");
		ShowFeels = bool.Parse(root.Element("ShowFeels")?.Value ?? "true");
		ShowName = bool.Parse(root.Element("ShowName")?.Value ?? "false");
		ShowEmotes = bool.Parse(root.Element("ShowEmotes")?.Value ?? "true");
		ShowDescriptionProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("ShowDescriptionProg")?.Value ?? "0")) ?? Gameworld.AlwaysTrueProg;
	}

	protected override XElement SaveDefinition()
	{
		return SaveSustainedSelfDefinition(
			new XElement("PowerDistance", PowerDistance),
			new XElement("ShowThinks", ShowThinks),
			new XElement("ShowFeels", ShowFeels),
			new XElement("ShowName", ShowName),
			new XElement("ShowEmotes", ShowEmotes),
			new XElement("ShowDescriptionProg", ShowDescriptionProg.Id)
		);
	}

	public MagicPowerDistance PowerDistance { get; private set; }
	public bool ShowThinks { get; private set; }
	public bool ShowFeels { get; private set; }
	public bool ShowName { get; private set; }
	public bool ShowEmotes { get; private set; }
	public IFutureProg ShowDescriptionProg { get; private set; }

	public bool TargetFilter(ICharacter listener, ICharacter thinker)
	{
		return TargetIsValid(listener, thinker);
	}

	public bool CheckCanHear(ICharacter listener, ICharacter thinker)
	{
		return Gameworld.GetCheck(CheckType.MagicTelepathyCheck)
		                .Check(listener, SkillCheckDifficulty, SkillCheckTrait, thinker) >= MinimumSuccessThreshold;
	}

	protected override IEffect CreateEffect(ICharacter actor)
	{
		return new PsionicHearEffect(actor, this);
	}

	protected override IEnumerable<IEffect> ActiveEffects(ICharacter actor)
	{
		return actor.EffectsOfType<PsionicHearEffect>().Where(x => x.Power == this);
	}

	protected override void ShowSubtypeDetails(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Power Distance: {PowerDistance.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Show Thinks: {ShowThinks.ToColouredString()}");
		sb.AppendLine($"Show Feels: {ShowFeels.ToColouredString()}");
		sb.AppendLine($"Show Name: {ShowName.ToColouredString()}");
		sb.AppendLine($"Show Emotes: {ShowEmotes.ToColouredString()}");
		sb.AppendLine($"Show Description Prog: {ShowDescriptionProg.MXPClickableFunctionName()}");
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "distance":
				if (!command.SafeRemainingArgument.TryParseEnum(out MagicPowerDistance value))
				{
					actor.OutputHandler.Send($"Valid distances are {Enum.GetValues<MagicPowerDistance>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
					return false;
				}

				PowerDistance = value;
				Changed = true;
				actor.OutputHandler.Send($"This power now listens across {value.LongDescription().ColourValue()}.");
				return true;
			case "thinks":
				ShowThinks = !ShowThinks;
				Changed = true;
				actor.OutputHandler.Send($"This power will {ShowThinks.NowNoLonger()} show thoughts.");
				return true;
			case "feels":
				ShowFeels = !ShowFeels;
				Changed = true;
				actor.OutputHandler.Send($"This power will {ShowFeels.NowNoLonger()} show feelings.");
				return true;
			case "name":
				ShowName = !ShowName;
				Changed = true;
				actor.OutputHandler.Send($"This power will {ShowName.NowNoLonger()} show real names.");
				return true;
			case "emotes":
				ShowEmotes = !ShowEmotes;
				Changed = true;
				actor.OutputHandler.Send($"This power will {ShowEmotes.NowNoLonger()} show think/feel emotes.");
				return true;
			case "descprog":
				return BuildingCommandDescriptionProg(actor, command);
		}

		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandDescriptionProg(ICharacter actor, StringStack command)
	{
		if (command.SafeRemainingArgument.EqualToAny("none", "clear", "delete"))
		{
			ShowDescriptionProg = Gameworld.AlwaysTrueProg;
			Changed = true;
			actor.OutputHandler.Send("This power will always show descriptions for heard minds.");
			return true;
		}

		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Boolean,
			[
				[ProgVariableTypes.Character, ProgVariableTypes.Character]
			]).LookupProg();
		if (prog is null)
		{
			return false;
		}

		ShowDescriptionProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This power now uses {prog.MXPClickableFunctionName()} to decide whether descriptions are shown.");
		return true;
	}
}


using MudSharp.Models;
using MudSharp.RPG.Merits.Interfaces;

#nullable enable
namespace MudSharp.RPG.Merits.CharacterMerits;

internal sealed class BlindFightingMerit : CharacterMeritBase, IBlindFightingMerit
{
	private BlindFightingMerit() { }

	internal BlindFightingMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		DifficultyStages = Math.Clamp(int.Parse(XElement.Parse(merit.Definition).Element("DifficultyStages")?.Value ?? "0"), -10, 10);
	}

	private BlindFightingMerit(IFuturemud gameworld, string name)
		: base(gameworld, name, "Blind Fighting", "@ have|has an adjustment to fighting without clear sight")
	{
		DoDatabaseInsert();
	}

	public int DifficultyStages { get; private set; }

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Blind Fighting", (merit, game) => new BlindFightingMerit(merit, game));
		MeritFactory.RegisterBuilderMeritInitialiser("Blind Fighting", (game, name) => new BlindFightingMerit(game, name));
		MeritFactory.RegisterMeritHelp("Blind Fighting", "Changes blind-fighting difficulty", new BlindFightingMerit().HelpText);
	}

	protected override XElement SaveSubtypeDefinition(XElement root)
	{
		root.Add(new XElement("DifficultyStages", DifficultyStages));
		return root;
	}

	protected override string SubtypeHelp => $@"{base.SubtypeHelp}
	#3stages <number>#0 - signed stages (-10 to 10); negative is easier, positive harder";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		if (command.PopForSwitch() != "stages") return base.BuildingCommand(actor, command.GetUndo());
		if (!int.TryParse(command.SafeRemainingArgument, out var stages) || stages is < -10 or > 10)
		{
			actor.OutputHandler.Send("Enter a whole number of stages from -10 (easier) to 10 (harder).");
			return false;
		}
		DifficultyStages = stages;
		Changed = true;
		actor.OutputHandler.Send($"This merit now shifts blind-fighting difficulty by {stages.ToString("N0", actor).ColourValue()} stages (negative is easier).");
		return true;
	}

	protected override void SubtypeShow(ICharacter actor, StringBuilder sb) =>
		sb.AppendLine($"Blind-Fighting Difficulty Stages: {DifficultyStages.ToString("N0", actor).ColourValue()} (negative is easier)");
}

#nullable enable

using System.Globalization;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat;

public sealed class BackstabAttack : WeaponAttack, IBackstabAttack
{
	public Difficulty SecondaryDifficulty { get; private set; } = Difficulty.Normal;
	public double ReelSecondsPerDegree { get; private set; }
	public double AttackerOffensiveAdvantagePerDegree { get; private set; }
	public double AttackerDefensiveAdvantagePerDegree { get; private set; }
	public double TargetOffensiveDisadvantagePerDegree { get; private set; }
	public double TargetDefensiveDisadvantagePerDegree { get; private set; }
	public double PenetrationBonus { get; private set; }
	public double ArmourBypassChance { get; private set; }

	public BackstabAttack(Models.WeaponAttack attack, IFuturemud gameworld) : base(attack, gameworld) { }
	public BackstabAttack(IFuturemud gameworld, BuiltInCombatMoveType type) : base(gameworld, type) { }

	protected override void LoadFromDatabase(Models.WeaponAttack attack)
	{
		base.LoadFromDatabase(attack);
		MaximumTargets = 1;
		LoadConfiguration(string.IsNullOrWhiteSpace(attack.AdditionalInfo) ? null : XElement.Parse(attack.AdditionalInfo));
	}

	internal void LoadConfiguration(XElement? root)
	{
		SecondaryDifficulty = Enum.TryParse<Difficulty>(root?.Element("Resist")?.Value, out var difficulty) &&
		                      Enum.IsDefined(difficulty) ? difficulty : Difficulty.Normal;
		ReelSecondsPerDegree = ReadAmount(root, "Reel", TimeSpan.MaxValue.TotalSeconds / 3.0);
		AttackerOffensiveAdvantagePerDegree = ReadAmount(root, "AttackerOffence");
		AttackerDefensiveAdvantagePerDegree = ReadAmount(root, "AttackerDefence");
		TargetOffensiveDisadvantagePerDegree = ReadAmount(root, "TargetOffence");
		TargetDefensiveDisadvantagePerDegree = ReadAmount(root, "TargetDefence");
		PenetrationBonus = ReadAmount(root, "Penetration");
		ArmourBypassChance = ReadAmount(root, "ArmourBypass", 1.0);
	}

	private static double ReadAmount(XElement? root, string name, double maximum = double.MaxValue / 3.0) =>
		double.TryParse(root?.Element(name)?.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var value) &&
		double.IsFinite(value) && value >= 0.0 && value <= maximum ? value : 0.0;

	internal XElement SaveConfiguration() => new("Backstab",
		new XElement("Resist", SecondaryDifficulty), new XElement("Reel", ReelSecondsPerDegree),
		new XElement("AttackerOffence", AttackerOffensiveAdvantagePerDegree),
		new XElement("AttackerDefence", AttackerDefensiveAdvantagePerDegree),
		new XElement("TargetOffence", TargetOffensiveDisadvantagePerDegree),
		new XElement("TargetDefence", TargetDefensiveDisadvantagePerDegree),
		new XElement("Penetration", PenetrationBonus), new XElement("ArmourBypass", ArmourBypassChance));

	protected override void SeedInitialData(Models.WeaponAttack attack) => attack.AdditionalInfo = SaveConfiguration().ToString();
	protected override void AddAttackSpecificCloneData(Models.WeaponAttack attack)
	{
		attack.MaximumTargets = 1;
		attack.AdditionalInfo = SaveConfiguration().ToString();
	}
	protected override void SaveAttackSpecificData(Models.WeaponAttack attack) => AddAttackSpecificCloneData(attack);
	public override string SpecialListText => $"Backstab, resist {SecondaryDifficulty.Describe()}, single target";
	protected override string ShowBuilderInternal(ICharacter actor) =>
		$"{SpecialListText}\nReel: {ReelSecondsPerDegree.ToString("N2", actor).ColourValue()}s per failure degree\n" +
		$"Attacker advantage per degree: offence {AttackerOffensiveAdvantagePerDegree.ToString("N2", actor).ColourValue()}, defence {AttackerDefensiveAdvantagePerDegree.ToString("N2", actor).ColourValue()}\n" +
		$"Target disadvantage per degree: offence {TargetOffensiveDisadvantagePerDegree.ToString("N2", actor).ColourValue()}, defence {TargetDefensiveDisadvantagePerDegree.ToString("N2", actor).ColourValue()}\n" +
		$"Penetration bonus: {PenetrationBonus.ToString("N2", actor).ColourValue()}, worn armour bypass: {ArmourBypassChance.ToString("P2", actor).ColourValue()}";
	protected override void DescribeForAttacksCommandInternal(StringBuilder sb, ICharacter actor) => sb.Append(ShowBuilderInternal(actor));
	public override string HelpText => $@"{base.HelpText}
	#3resist <difficulty>#0 - resistance to all backstab secondary effects
	#3reel <seconds>#0 - combat action delay per resistance failure degree
	#3attackeradvantage <offence> <defence>#0 - attacker gains per failure degree
	#3targetdisadvantage <offence> <defence>#0 - target penalties per failure degree
	#3penetration <bonus>#0 - bonus to the armour penetration check
	#3armourbypass <percentage>#0 - chance to skip worn armour (e.g. 25%)
	Backstabs always strike one target. All optional effects default to zero.";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var option = command.PopForSwitch();
		switch (option)
		{
			case "resist":
				if (!CheckExtensions.GetDifficulty(command.SafeRemainingArgument, out var difficulty) || !Enum.IsDefined(difficulty))
					return Error(actor, "Specify a valid resistance difficulty.");
				SecondaryDifficulty = difficulty;
				break;
			case "reel":
			case "penetration":
				if (!TryAmount(command.SafeRemainingArgument, actor, out var amount) ||
				    option == "reel" && amount > TimeSpan.MaxValue.TotalSeconds / 3.0)
					return Error(actor, "Specify a finite, nonnegative amount within the supported range.");
				if (option == "reel") ReelSecondsPerDegree = amount; else PenetrationBonus = amount;
				break;
			case "attackeradvantage":
			case "targetdisadvantage":
				if (!TryAmount(command.PopSpeech(), actor, out var offence) ||
				    !TryAmount(command.PopSpeech(), actor, out var defence) || !command.IsFinished)
					return Error(actor, "Specify finite, nonnegative offence and defence amounts.");
				if (option == "attackeradvantage")
				{ AttackerOffensiveAdvantagePerDegree = offence; AttackerDefensiveAdvantagePerDegree = defence; }
				else { TargetOffensiveDisadvantagePerDegree = offence; TargetDefensiveDisadvantagePerDegree = defence; }
				break;
			case "armourbypass":
				if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var chance) ||
				    !double.IsFinite(chance) || chance < 0.0 || chance > 1.0)
					return Error(actor, "Specify a percentage from 0% to 100%.");
				ArmourBypassChance = chance;
				break;
			case "targets": case "targetcount": case "maxtargets": case "multitargets": case "opponents":
				if (command.SafeRemainingArgument != "1") return Error(actor, "Backstabs can only strike one target.");
				MaximumTargets = 1;
				break;
			default: return base.BuildingCommand(actor, command.GetUndo());
		}
		Changed = true;
		actor.OutputHandler.Send(ShowBuilderInternal(actor));
		return true;
	}

	private static bool TryAmount(string text, ICharacter actor, out double amount) =>
		double.TryParse(text, NumberStyles.Float | NumberStyles.AllowThousands, actor, out amount) &&
		double.IsFinite(amount) && amount >= 0.0 && amount <= double.MaxValue / 3.0;
	private static bool Error(ICharacter actor, string message)
	{
		actor.OutputHandler.Send(message);
		return false;
	}
}

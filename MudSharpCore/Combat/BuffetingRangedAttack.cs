using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.Combat;

public class BuffetingRangedAttack : RangedNaturalAttackBase, IBuffetingRangedAttack
{
	public BuffetingRangedAttack(Models.WeaponAttack attack, IFuturemud gameworld) : base(attack, gameworld)
	{
	}

	public BuffetingRangedAttack(IFuturemud gameworld, BuiltInCombatMoveType type) : base(gameworld, type)
	{
		MaximumPushDistance = 1;
		InflictsDamage = false;
	}

	public int MaximumPushDistance { get; set; }
	public double OffensiveAdvantagePerDegree { get; set; }
	public double DefensiveAdvantagePerDegree { get; set; }
	public bool InflictsDamage { get; set; }

	protected override void LoadFromXElement(XElement root)
	{
		MaximumPushDistance = int.Parse(root.Element("MaximumPushDistance")?.Value ?? "1");
		OffensiveAdvantagePerDegree = double.Parse(root.Element("OffensiveAdvantagePerDegree")?.Value ?? "0");
		DefensiveAdvantagePerDegree = double.Parse(root.Element("DefensiveAdvantagePerDegree")?.Value ?? "0");
		InflictsDamage = bool.Parse(root.Element("InflictsDamage")?.Value ?? "false");
	}

	protected override void SaveToXml(XElement root)
	{
		root.Add(
			new XElement("MaximumPushDistance", MaximumPushDistance),
			new XElement("OffensiveAdvantagePerDegree", OffensiveAdvantagePerDegree),
			new XElement("DefensiveAdvantagePerDegree", DefensiveAdvantagePerDegree),
			new XElement("InflictsDamage", InflictsDamage)
		);
	}

	public override string HelpText => $@"{base.HelpText}
	#3push <rooms>#0 - sets the maximum push distance
	#3offadv <amount>#0 - sets offensive advantage applied per success degree
	#3defadv <amount>#0 - sets defensive advantage applied per success degree
	#3damage#0 - toggles whether this buffeting attack also inflicts normal damage";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "push":
			case "distance":
				return BuildingCommandPush(actor, command);
			case "offadv":
				return BuildingCommandOffAdv(actor, command);
			case "defadv":
				return BuildingCommandDefAdv(actor, command);
			case "damage":
				return BuildingCommandDamage(actor);
		}

		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandPush(ICharacter actor, StringStack command)
	{
		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value < 0)
		{
			actor.OutputHandler.Send("You must enter a non-negative integer.");
			return false;
		}

		MaximumPushDistance = value;
		Changed = true;
		actor.OutputHandler.Send($"This attack can now push up to {MaximumPushDistance.ToString("N0", actor).ColourValue()} room(s).");
		return true;
	}

	private bool BuildingCommandOffAdv(ICharacter actor, StringStack command)
	{
		if (!double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send("You must enter a number.");
			return false;
		}

		OffensiveAdvantagePerDegree = value;
		Changed = true;
		actor.OutputHandler.Send($"This attack now applies {OffensiveAdvantagePerDegree.ToString("N2", actor).ColourValue()} offensive advantage per degree.");
		return true;
	}

	private bool BuildingCommandDefAdv(ICharacter actor, StringStack command)
	{
		if (!double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send("You must enter a number.");
			return false;
		}

		DefensiveAdvantagePerDegree = value;
		Changed = true;
		actor.OutputHandler.Send($"This attack now applies {DefensiveAdvantagePerDegree.ToString("N2", actor).ColourValue()} defensive advantage per degree.");
		return true;
	}

	private bool BuildingCommandDamage(ICharacter actor)
	{
		InflictsDamage = !InflictsDamage;
		Changed = true;
		actor.OutputHandler.Send($"This attack will {InflictsDamage.NowNoLonger()} inflict ordinary damage.");
		return true;
	}

	protected override string ShowBuilderInternal(ICharacter actor)
	{
		var sb = new StringBuilder(base.ShowBuilderInternal(actor));
		sb.AppendLine($"Push Distance: {MaximumPushDistance.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Off. Adv./Degree: {OffensiveAdvantagePerDegree.ToString("N2", actor).ColourValue()}");
		sb.AppendLine($"Def. Adv./Degree: {DefensiveAdvantagePerDegree.ToString("N2", actor).ColourValue()}");
		sb.AppendLine($"Inflicts Damage: {InflictsDamage.ToColouredString()}");
		return sb.ToString();
	}
}

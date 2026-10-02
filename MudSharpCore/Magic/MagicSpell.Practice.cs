using System.Globalization;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

#nullable enable
namespace MudSharp.Magic;

public partial class MagicSpell
{
	public IInventoryPlanTemplate? PracticeInventoryPlanTemplate { get; private set; }

	private ControlledSpellPractice LoadPractice(XElement root)
	{
		if ((int?)root.Attribute("schema") != 1 || root.Element("Plan") is not { } plan)
			throw new FormatException("Practice requires supported schema 1 and an explicit material plan.");
		PracticeInventoryPlanTemplate = new InventoryPlanTemplate(plan, Gameworld);
		return new((bool)root.Attribute("enabled")!, (Difficulty)(int)root.Attribute("difficulty")!,
			TimeSpan.FromSeconds((double)root.Attribute("seconds")!), (double)root.Attribute("energy")!,
			(int?)root.Attribute("maxGrade"), (bool)root.Attribute("speech")!, (bool)root.Attribute("hand")!,
			(bool)root.Attribute("movement")!);
	}

	private XElement? SavePractice() => GradeProfile?.Practice is not { } p ? null :
		new("Practice", new XAttribute("schema", 1), new XAttribute("enabled", p.Enabled),
			new XAttribute("difficulty", (int)p.Difficulty), new XAttribute("seconds", p.Duration.TotalSeconds),
			new XAttribute("energy", p.EnergyMultiplier), p.MaximumGrade is { } maximum ? new XAttribute("maxGrade", maximum) : null,
			new XAttribute("speech", p.RequiresSpeech), new XAttribute("hand", p.RequiresFreeHand),
			new XAttribute("movement", p.AllowMovement), PracticeInventoryPlanTemplate?.SaveToXml());

	private void AppendPracticeShow(StringBuilder sb, ICharacter actor)
	{
		if (GradeProfile?.Practice is not { } p) { sb.AppendLine("  Practice: not authored."); return; }
		sb.AppendLine($"  Practice: {p.Enabled.ToColouredString()}, {p.Duration.Describe(actor)}, {p.Difficulty.DescribeEnum()}, energy ×{p.EnergyMultiplier.ToString("N2", actor)}, maximum {(p.MaximumGrade?.ToString() ?? "ordinary admission/profile")}");
		sb.AppendLine($"  Practice requires speech {p.RequiresSpeech.ToColouredString()}, free hand {p.RequiresFreeHand.ToColouredString()}, permits movement {p.AllowMovement.ToColouredString()}.");
		foreach (var action in PracticeInventoryPlanTemplate?.Phases.SelectMany(x => x.Actions) ?? [])
			sb.AppendLine($"    {action.Describe(actor)}");
	}

	private bool BuildingCommandPractice(ICharacter actor, StringStack command, ControlledSpellProfile profile)
	{
		var action = command.PopSpeech().ToLowerInvariant();
		var plan = PracticeInventoryPlanTemplate;
		if (action == "fixture")
		{
			if (!command.IsFinished) throw new FormatException("Unexpected trailing practice fixture input.");
			profile = profile with { Practice = new(true, Difficulty.Normal, TimeSpan.FromSeconds(30), 1, null, true, true, false) };
			plan = new InventoryPlanTemplate(Gameworld, [new InventoryPlanPhaseTemplate(1, [])]);
			actor.OutputHandler.Send("Authored provisional practice: 30 seconds, full configured energy, normal base difficulty, speech/free hand, retained focus and no separate grade maximum. The explicit practice material plan is empty.");
		}
		else
		{
			var p = profile.Practice ?? throw new FormatException("Author grades practice fixture first, then configure its policy and explicit plan.");
			switch (action)
			{
				case "enabled": p = p with { Enabled = Boolean() }; break;
				case "duration": p = p with { Duration = TimeSpan.FromSeconds(Number()) }; break;
				case "difficulty":
					if (!Enum.TryParse<Difficulty>(command.PopSpeech(), true, out var difficulty)) throw new FormatException("Specify a native difficulty.");
					p = p with { Difficulty = difficulty }; break;
				case "energy": p = p with { EnergyMultiplier = Number() }; break;
				case "max":
					var maximum = command.PopSpeech();
					p = p with { MaximumGrade = maximum.EqualTo("none") ? null : int.Parse(maximum, CultureInfo.InvariantCulture) }; break;
				case "speech": p = p with { RequiresSpeech = Boolean() }; break;
				case "hand": p = p with { RequiresFreeHand = Boolean() }; break;
				case "movement": p = p with { AllowMovement = Boolean() }; break;
				case "plan":
					plan = new InventoryPlanTemplate(PracticeInventoryPlanTemplate!.SaveToXml(), Gameworld);
					var operation = command.PopSpeech().ToLowerInvariant();
					if (operation == "add")
					{
						var material = MudSharp.GameItems.Inventory.Plans.InventoryPlanTemplate.ParseActionFromBuilderInput(actor, command);
						if (material is null) return false;
						plan.FirstPhase.AddAction(material);
						command = new StringStack("");
					}
					else if (operation == "remove")
					{
						var index = int.Parse(command.PopSpeech(), CultureInfo.InvariantCulture);
						var materials = plan.FirstPhase.Actions.ToArray();
						if (index < 1 || index > materials.Length) throw new FormatException("Specify an existing one-based practice plan action.");
						plan.FirstPhase.RemoveAction(materials[index - 1]);
					}
					else throw new FormatException("Use practice plan add <native inventory action> or remove <one-based index>.");
					break;
				default: throw new FormatException("Use practice fixture, enabled <true|false>, duration <seconds>, difficulty <native difficulty>, energy <positive multiplier>, max <grade|none>, speech/hand/movement <true|false>, or plan add/remove.");
			}
			if (!command.IsFinished) throw new FormatException("Unexpected trailing practice input.");
			profile = profile with { Practice = p };
		}
		GradeProfile = profile; PracticeInventoryPlanTemplate = plan;
		_unreadableGradeProfile = null; _gradeLoadError = null; Changed = true;
		actor.OutputHandler.Send("Practice policy updated; numerical values are configurable tuning choices.");
		foreach (var error in GradeConfigurationErrors()) actor.OutputHandler.Send(error.ColourError());
		return true;

		bool Boolean() => bool.Parse(command.PopSpeech());
		double Number() => double.Parse(command.PopSpeech(), CultureInfo.InvariantCulture);
	}
}

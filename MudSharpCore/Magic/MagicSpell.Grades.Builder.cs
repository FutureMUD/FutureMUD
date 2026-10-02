using System.Globalization;

#nullable enable
namespace MudSharp.Magic;

public partial class MagicSpell
{
	private bool BuildingCommandGrades(ICharacter actor, StringStack command)
	{
		try
		{
			var action = command.PopSpeech().ToLowerInvariant();
			if (action == "show") { var sb = new StringBuilder(); AppendGradeShow(sb, actor); actor.OutputHandler.Send(sb.ToString()); return false; }
			var p = GradeProfile ?? FixtureGradeProfile();
			switch (action)
			{
				case "fixture": p = FixtureGradeProfile(); break;
				case "version": p = p with { Version = Integer() }; break;
				case "grade":
					var number = Integer();
					if (!Enum.TryParse<SpellPower>(command.PopSpeech(), true, out var power) || !Enum.IsDefined(power)) throw new FormatException("No such native SpellPower.");
					var replacement = new ControlledSpellGrade(number, power, Number(), Integer());
					p = p with { Grades = Array.AsReadOnly(p.Grades.Where(x => x.Grade != number).Append(replacement).OrderBy(x => x.Grade).ToArray()) }; break;
				case "mastery": p = p with { MasteryChance = Number(), MasteryInterval = TimeSpan.FromSeconds(Number()) }; break;
				case "skill": p = p with { SkillInterval = TimeSpan.FromSeconds(Number()), OpeningSkill = Number() }; break;
				case "overreach": p = p with { OverreachMultiplier = Number(), OverreachDifficultySteps = Integer() }; break;
				case "efficiency":
					var efficiencyMode = command.PopSpeech().ToLowerInvariant();
					p = p with { Efficiency = efficiencyMode switch
					{
						"off" => null,
						"source" => new ControlledSpellEfficiency(Number(), Number()),
						_ => throw new FormatException("Use efficiency off or source <minimum> <scale>.")
					} }; break;
				case "scalar":
					var operation = command.PopSpeech().ToLowerInvariant();
					var list = command.PopSpeech().ToLowerInvariant();
					var index = Integer();
					var bindings = p.ScalarBindings.Where(x => x.List != list || x.Index != index).ToList();
					if (operation == "add")
					{
						var effect = command.PopSpeech(); var field = command.PopSpeech();
						bindings.Add(new(list, index, effect, field, command.SafeRemainingArgument));
						command = new StringStack("");
					}
					else if (operation != "remove") throw new FormatException("Use scalar add or remove.");
					p = p with { ScalarBindings = bindings.AsReadOnly() }; break;
				default:
					actor.OutputHandler.Send(@"Grade profile options:
	#3grades fixture#0 - author the ARM-02 seven-grade profile
	#3grades show#0
	#3grades version <positive integer>#0
	#3grades grade <number> <SpellPower> <raw skill> <difficulty steps>#0
	#3grades mastery <chance 0..1> <seconds>#0
	#3grades skill <seconds> <opening value>#0
	#3grades overreach <cost multiplier> <difficulty steps>#0
	#3grades efficiency source <minimum 0..50> <positive scale>#0
	#3grades efficiency off#0
	#3grades scalar add <target|caster> <zero-based index> boost Bonus <expression>#0
	#3grades scalar remove <target|caster> <zero-based index>#0".SubstituteANSIColour()); return false;
			}
			if (!command.IsFinished) throw new FormatException("Unexpected trailing input.");
			GradeProfile = p; _unreadableGradeProfile = null; _gradeLoadError = null; Changed = true;
			actor.OutputHandler.Send("Grade profile updated. Existing acquired records must match its version.");
			foreach (var error in GradeConfigurationErrors()) actor.OutputHandler.Send(error.ColourError());
			return true;
		}
		catch (Exception ex) when (ex is FormatException or OverflowException or ArgumentException)
		{ actor.OutputHandler.Send(ex.Message.ColourError()); return false; }

		int Integer() => int.Parse(command.PopSpeech(), CultureInfo.InvariantCulture);
		double Number() => double.Parse(command.PopSpeech(), CultureInfo.InvariantCulture);
	}
}

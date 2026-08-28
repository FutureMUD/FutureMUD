#nullable enable

using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;

namespace MudSharp.NPC.Templates;

public sealed class NPCSkillPackage : SaveableItem, INPCSkillPackage
{
	public const double MaximumAbsoluteSkewness = 0.99;
	private readonly List<NPCSkillPackageEntry> _skills = [];

	public NPCSkillPackage(MudSharp.Models.NpcSkillPackage package, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = package.Id;
		_name = package.Name;
		foreach (var skill in package.Skills)
		{
			if (gameworld.Traits.Get(skill.TraitDefinitionId) is not ITraitDefinition definition ||
				definition.TraitType != TraitType.Skill)
			{
				continue;
			}

			_skills.Add(new NPCSkillPackageEntry(definition, skill.Chance, skill.Mean,
				skill.StandardDeviation, skill.Skewness));
		}
	}

	public NPCSkillPackage(IFuturemud gameworld, string name)
	{
		Gameworld = gameworld;
		_name = name;
		using (new FMDB())
		{
			var package = new MudSharp.Models.NpcSkillPackage { Name = name };
			FMDB.Context.NpcSkillPackages.Add(package);
			FMDB.Context.SaveChanges();
			_id = package.Id;
		}
	}

	private NPCSkillPackage(NPCSkillPackage rhs, string name) : this(rhs.Gameworld, name)
	{
		_skills.AddRange(rhs._skills);
		Changed = true;
	}

	public override string FrameworkItemType => "NPCSkillPackage";
	public IReadOnlyCollection<NPCSkillPackageEntry> Skills => _skills;

	public INPCSkillPackage Clone(string name) => new NPCSkillPackage(this, name);

	public override void Save()
	{
		var package = FMDB.Context.NpcSkillPackages
			.Include(x => x.Skills)
			.First(x => x.Id == Id);
		package.Name = Name;
		FMDB.Context.NpcSkillPackageSkills.RemoveRange(package.Skills);
		foreach (var skill in _skills)
		{
			package.Skills.Add(new MudSharp.Models.NpcSkillPackageSkill
			{
				TraitDefinitionId = skill.Skill.Id,
				Chance = skill.Chance,
				Mean = skill.Mean,
				StandardDeviation = skill.StandardDeviation,
				Skewness = skill.Skewness
			});
		}

		Changed = false;
	}

	private const string BuildingHelp = @"You can use the following options with this package:

	name <name> - renames this package
	skill <skill> <chance%> <mean> <standard deviation> [<skewness>] - adds or replaces a skill
	skill <skill> 0% - removes a skill

Skewness must be between -0.99 and 0.99. Omit it for an ordinary normal distribution.";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "name":
			case "rename":
				return BuildingCommandName(actor, command);
			case "skill":
			case "skills":
				return BuildingCommandSkill(actor, command);
			case "help":
			case "?":
			case "":
				actor.OutputHandler.Send(BuildingHelp.SubstituteANSIColour());
				return false;
			default:
				actor.OutputHandler.Send(BuildingHelp.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give this NPC skill package?");
			return false;
		}

		if (!TryNormaliseName(command.SafeRemainingArgument, out var name, out var error))
		{
			actor.OutputHandler.Send(error);
			return false;
		}

		if (Gameworld.NpcSkillPackages.Any(x => x.Id != Id && x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already an NPC skill package named {name.ColourName()}.");
			return false;
		}

		actor.OutputHandler.Send($"You rename the {_name.ColourName()} package to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	internal static bool TryNormaliseName(string proposedName, out string name, out string error)
	{
		name = proposedName.TitleCase();
		if (string.IsNullOrWhiteSpace(name))
		{
			error = "Package names cannot be blank.";
			return false;
		}

		if (name.All(char.IsDigit))
		{
			error = "Package names cannot be entirely numeric, because numeric input is reserved for IDs.";
			return false;
		}

		error = string.Empty;
		return true;
	}

	private bool BuildingCommandSkill(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which skill do you want to add, change or remove?");
			return false;
		}

		var skillText = command.PopSpeech();
		var skill = Gameworld.Traits
			.Where(x => x.TraitType == TraitType.Skill)
			.GetByIdOrName(skillText);
		if (skill is null)
		{
			actor.OutputHandler.Send("There is no such skill.");
			return false;
		}

		if (command.IsFinished || !command.PopSpeech().TryParsePercentage(actor.Account.Culture, out var chance) ||
			!double.IsFinite(chance) || chance < 0.0 || chance > 1.0)
		{
			actor.OutputHandler.Send("You must specify a chance between 0% and 100%.");
			return false;
		}

		if (chance == 0.0)
		{
			if (_skills.RemoveAll(x => x.Skill == skill) == 0)
			{
				actor.OutputHandler.Send("That skill is not in this package.");
				return false;
			}

			Changed = true;
			actor.OutputHandler.Send($"You remove {skill.Name.ColourName()} from this package.");
			return true;
		}

		if (command.IsFinished || !double.TryParse(command.PopSpeech(), NumberStyles.Float, actor, out var mean) ||
			!double.IsFinite(mean) || mean < 0.0)
		{
			actor.OutputHandler.Send("You must specify a non-negative mean value.");
			return false;
		}

		if (command.IsFinished || !double.TryParse(command.PopSpeech(), NumberStyles.Float, actor, out var deviation) ||
			!double.IsFinite(deviation) || deviation < 0.0)
		{
			actor.OutputHandler.Send("You must specify a non-negative standard deviation.");
			return false;
		}

		var skewness = 0.0;
		if (!command.IsFinished &&
			(!double.TryParse(command.PopSpeech(), NumberStyles.Float, actor, out skewness) ||
			 !double.IsFinite(skewness) ||
			 Math.Abs(skewness) > MaximumAbsoluteSkewness))
		{
			actor.OutputHandler.Send("Skewness must be between -0.99 and 0.99.");
			return false;
		}

		_skills.RemoveAll(x => x.Skill == skill);
		_skills.Add(new NPCSkillPackageEntry(skill, chance, mean, deviation, skewness));
		Changed = true;
		actor.OutputHandler.Send(
			$"You set {skill.Name.ColourName()} to {chance.ToString("P1", actor).ColourValue()} chance, mean {mean.ToString("N2", actor).ColourValue()}, standard deviation {deviation.ToString("N2", actor).ColourValue()} and skewness {skewness.ToString("N2", actor).ColourValue()}.");
		return true;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"NPC Skill Package #{Id.ToString("N0", actor)} - {Name.ColourName()}".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(StringUtilities.GetTextTable(
			_skills.OrderBy(x => x.Skill.Name).Select(x => new List<string>
			{
				x.Skill.Name,
				x.Chance.ToString("P1", actor),
				x.Mean.ToString("N2", actor),
				x.StandardDeviation.ToString("N2", actor),
				x.Skewness.ToString("N2", actor),
				x.WeightedExpectedValue.ToString("N2", actor)
			}),
			["Skill", "Chance", "Mean", "Std Dev", "Skewness", "Weighted EV"], actor, Telnet.Yellow));
		return sb.ToString();
	}

	public ProgVariableTypes Type => ProgVariableTypes.NPCSkillPackage;
	public object GetObject => this;

	public IProgVariable GetProperty(string property)
	{
		return property.ToLowerInvariant() switch
		{
			"id" => new NumberVariable(Id),
			"name" => new TextVariable(Name),
			"skills" => new CollectionVariable(_skills.Select(x => x.Skill).ToList(), ProgVariableTypes.Trait),
			_ => throw new NotSupportedException($"Unsupported NPC skill package property {property}.")
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.NPCSkillPackage,
			new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
			{
				["id"] = ProgVariableTypes.Number,
				["name"] = ProgVariableTypes.Text,
				["skills"] = ProgVariableTypes.Collection | ProgVariableTypes.Trait
			},
			new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
			{
				["id"] = "The package ID.",
				["name"] = "The package name.",
				["skills"] = "The skill definitions contained in the package."
			});
	}
}

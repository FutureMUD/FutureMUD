using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore.Internal;
using MudSharp.Character;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace MudSharp.RPG.Knowledge;

public class Knowledge : SaveableItem, IKnowledge
{
	private readonly Dictionary<IChargenResource, int> _resourceCosts = new();

	private IFutureProg _canLearnProg;

	private IFutureProg _canPickChargenProg;

	private string _description;

	private string _knowledgeSubtype;

	private string _knowledgeType;

	private LearnableType _learnable;

	private Difficulty _learnDifficulty;

	private int _learnerSessionsRequired;

	private string _longDescription;

	private Difficulty _teachDifficulty;

	public Knowledge(string name)
	{
		using (new FMDB())
		{
			_name = name;
			_knowledgeType = "Type";
			_knowledgeSubtype = "Subtype";
			_description = "";
			_longDescription = "";

			var dbitem = new Models.Knowledge();
			FMDB.Context.Knowledges.Add(dbitem);
			dbitem.Name = Name;
			dbitem.Type = KnowledgeType;
			dbitem.Subtype = KnowledgeSubtype;
			dbitem.Description = Description;
			dbitem.LongDescription = LongDescription;
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public Knowledge(MudSharp.Models.Knowledge knowledge, IFuturemud gameworld)
	{
		_id = knowledge.Id;
		_name = knowledge.Name;
		_description = knowledge.Description;
		_longDescription = knowledge.LongDescription;
		_knowledgeType = knowledge.Type;
		_knowledgeSubtype = knowledge.Subtype;
		_learnable = (LearnableType)knowledge.LearnableType;
		_learnDifficulty = (Difficulty)knowledge.LearnDifficulty;
		_teachDifficulty = (Difficulty)knowledge.TeachDifficulty;
		_learnerSessionsRequired = knowledge.LearningSessionsRequired;
		_canPickChargenProg = knowledge.CanAcquireProgId.HasValue
			? gameworld.FutureProgs.Get(knowledge.CanAcquireProgId.Value)
			: null;
		_canLearnProg = knowledge.CanLearnProgId.HasValue
			? gameworld.FutureProgs.Get(knowledge.CanLearnProgId.Value)
			: null;
		foreach (var cost in knowledge.KnowledgesCosts)
		{
			_resourceCosts[Gameworld.ChargenResources.Get(cost.ChargenResourceId)] = cost.Cost;
		}
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.Knowledges.Find(Id);
		dbitem.Name = Name;
		dbitem.Description = Description;
		dbitem.LongDescription = LongDescription;
		dbitem.Type = KnowledgeType;
		dbitem.Subtype = KnowledgeSubtype;
		dbitem.TeachDifficulty = (int)TeachDifficulty;
		dbitem.LearnDifficulty = (int)LearnDifficulty;
		dbitem.LearnableType = (int)Learnable;
		dbitem.LearningSessionsRequired = LearnerSessionsRequired;
		dbitem.CanAcquireProgId = CanPickChargenProg?.Id;
		dbitem.CanLearnProgId = CanLearnProg?.Id;
		FMDB.Context.KnowledgesCosts.RemoveRange(dbitem.KnowledgesCosts);
		foreach (var cost in _resourceCosts)
		{
			dbitem.KnowledgesCosts.Add(new KnowledgesCosts
				{ Knowledge = dbitem, ChargenResourceId = cost.Key.Id, Cost = cost.Value });
		}

		Changed = false;
	}

	public override string FrameworkItemType => "Knowledge";

	public string Description
	{
		get => _description;
		set
		{
			_description = value;
			Changed = true;
		}
	}

	public string LongDescription
	{
		get => _longDescription;
		set
		{
			_longDescription = value;
			Changed = true;
		}
	}

	public string KnowledgeType
	{
		get => _knowledgeType;
		set
		{
			_knowledgeType = value;
			Changed = true;
		}
	}

	public string KnowledgeSubtype
	{
		get => _knowledgeSubtype;
		set
		{
			_knowledgeSubtype = value;
			Changed = true;
		}
	}

	public IFutureProg CanPickChargenProg
	{
		get => _canPickChargenProg;
		set
		{
			_canPickChargenProg = value;
			Changed = true;
		}
	}

	public IFutureProg CanLearnProg
	{
		get => _canLearnProg;
		set
		{
			_canLearnProg = value;
			Changed = true;
		}
	}

	public LearnableType Learnable
	{
		get => _learnable;
		set
		{
			_learnable = value;
			Changed = true;
		}
	}

	public Difficulty TeachDifficulty
	{
		get => _teachDifficulty;
		set
		{
			_teachDifficulty = value;
			Changed = true;
		}
	}

	public Difficulty LearnDifficulty
	{
		get => _learnDifficulty;
		set
		{
			_learnDifficulty = value;
			Changed = true;
		}
	}

	public int LearnerSessionsRequired
	{
		get => _learnerSessionsRequired;
		set
		{
			_learnerSessionsRequired = value;
			Changed = true;
		}
	}

	public ProgVariableTypes Type => ProgVariableTypes.Knowledge;

	public object GetObject => this;

	public int ResourceCost(IChargenResource resource)
	{
		return _resourceCosts.ValueOrDefault(resource, 0);
	}

	public IProgVariable GetProperty(string property)
	{
		IProgVariable returnVar = null;
		switch (property.ToLowerInvariant())
		{
			case "id":
				returnVar = new NumberVariable(Id);
				break;
			case "name":
				returnVar = new TextVariable(Name);
				break;
			case "desc":
			case "description":
				returnVar = new TextVariable(Description);
				break;
			case "ldesc":
			case "longdescription":
				returnVar = new TextVariable(LongDescription);
				break;
			case "type":
				returnVar = new TextVariable(KnowledgeType);
				break;
			case "subtype":
				returnVar = new TextVariable(KnowledgeSubtype);
				break;
		}

		return returnVar;
	}

	private static ProgVariableTypes DotReferenceHandler(string property)
	{
		var returnVar = ProgVariableTypes.Error;
		switch (property.ToLowerInvariant())
		{
			case "id":
				returnVar = ProgVariableTypes.Number;
				break;
			case "name":
				returnVar = ProgVariableTypes.Text;
				break;
			case "desc":
				returnVar = ProgVariableTypes.Text;
				break;
			case "description":
				returnVar = ProgVariableTypes.Text;
				break;
			case "ldesc":
				returnVar = ProgVariableTypes.Text;
				break;
			case "longdescription":
				returnVar = ProgVariableTypes.Text;
				break;
			case "type":
				returnVar = ProgVariableTypes.Text;
				break;
			case "subtype":
				returnVar = ProgVariableTypes.Text;
				break;
		}

		return returnVar;
	}

	private static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", ProgVariableTypes.Number },
			{ "name", ProgVariableTypes.Text },
			{ "desc", ProgVariableTypes.Text },
			{ "description", ProgVariableTypes.Text },
			{ "ldesc", ProgVariableTypes.Text },
			{ "longdescription", ProgVariableTypes.Text },
			{ "type", ProgVariableTypes.Text },
			{ "subtype", ProgVariableTypes.Text }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", "" },
			{ "name", "" },
			{ "desc", "An alias for the 'description' property" },
			{ "description", "" },
			{ "ldesc", "An alias for the 'ldesc' property" },
			{ "longdescription", "" },
			{ "type", "" },
			{ "subtype", "" }
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Knowledge, DotReferenceHandler(),
			DotReferenceHelp());
	}

	#region Implementation of IEditableItem

	protected const string HelpText = @"You can use the following options with this command:

	#3name <name>#0 - renames the knowledge
	#3desc <desc>#0 - gives a new brief description of the knowledge
	#3ldesc <desc>#0 - sets the long description of a knowledge
	#3type <type>#0 - sets the knowledge type / category
	#3subtype <type>#0 - sets the knowledge subtype / subcategory
	#3sessions <##>#0 - sets the number of #3teach#0 sessions before someone learns the knowledge
	#3learnable LearnableAtSkillUp|LearnableAtChargen|LearnableFromTeacher#0 - toggles a learn type
	#3learnprog <prog>#0 - sets a prog that controls if someone can learn the knowledge
	#3learndifficulty <difficulty>#0 - sets the difficulty of the learn checks
	#3teachdifficulty <difficulty>#0 - sets the difficulty of the teach checks
	#3chargenprog <prog>#0 - sets the prog that controls if it can be taken at chargen
	#3resource <which> <##>#0 - sets the chargne cost of this knowledge (use 0 to remove cost)";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "desc":
			case "description":
				return BuildingCommandDescription(actor, command);
			case "longdescription":
			case "long description":
			case "long_description":
			case "longdesc":
			case "long_desc":
			case "long desc":
			case "ldesc":
			case "ldescription":
				return BuildingCommandLongDescription(actor, command);
			case "type":
				return BuildingCommandType(actor, command);
			case "subtype":
				return BuildingCommandSubtype(actor, command);
			case "sessions":
				return BuildingCommandSessions(actor, command);
			case "learnable":
				return BuildingCommandLearnable(actor, command);
			case "learndifficulty":
			case "learn difficulty":
			case "learn_difficulty":
				return BuildingCommandLearnDifficulty(actor, command);
			case "learnprog":
			case "learn prog":
			case "learn_prog":
				return BuildingCommandLearnProg(actor, command);
			case "chargenprog":
			case "chargen prog":
			case "chargen_prog":
				return BuildingCommandChargenProg(actor, command);
			case "resources":
			case "resource":
				return BuildingCommandResource(actor, command);
			case "teach difficulty":
			case "teachdifficulty":
			case "teach_difficulty":
				return BuildingCommandTeachDifficulty(actor, command);

			default:
				actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to this knowledge?");
			return false;
		}

		var name = command.SafeRemainingArgument;
		if (Gameworld.Knowledges.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a knowledge with that name. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename the knowledge {_name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	private bool BuildingCommandDescription(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want the brief description of this knowledge to be?");
			return false;
		}

		_description = command.SafeRemainingArgument;
		actor.OutputHandler.Send(
			$"You change the description of the {Name.ColourName()} knowledge to {_description.ColourCommand()}.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandLongDescription(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want the long description of this knowledge to be?");
			return false;
		}

		_longDescription = command.SafeRemainingArgument;
		actor.OutputHandler.Send(
			$"You change the long description of the {Name.ColourName()} knowledge to {_longDescription.ColourCommand()}.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandType(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want the type of this knowledge to be?");
			return false;
		}

		_knowledgeType = command.SafeRemainingArgument;
		actor.OutputHandler.Send(
			$"You change the type of the {Name.ColourName()} knowledge to {_knowledgeType.ColourCommand()}.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandSubtype(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want the subtype of this knowledge to be?");
			return false;
		}

		_knowledgeSubtype = command.SafeRemainingArgument;
		actor.OutputHandler.Send(
			$"You change the subtype of the {Name.ColourName()} knowledge to {_knowledgeSubtype.ColourCommand()}.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandSessions(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How many teaching sessions should be required to acquire this knowledge?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value <= 0)
		{
			actor.OutputHandler.Send("You must supply a valid number of teaching sessions.");
			return false;
		}

		_learnerSessionsRequired = value;
		actor.OutputHandler.Send(
			$"The knowledge {Name.ColourName()} will now require {value.ToString("N0", actor).ColourValue()} lessons to fully learn.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandLearnable(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which type of learnability do you want to toggle? The valid types are {Enum.GetValues(typeof(LearnableType)).OfType<LearnableType>().Where(x => x != LearnableType.NotLearnable).Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<LearnableType>(out var value))
		{
			actor.OutputHandler.Send(
				$"That is not a valid learnability type. The valid types are {Enum.GetValues(typeof(LearnableType)).OfType<LearnableType>().Where(x => x != LearnableType.NotLearnable).Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		if (_learnable.HasFlag(value))
		{
			_learnable &= ~value;
		}
		else
		{
			_learnable |= value;
		}

		actor.OutputHandler.Send(
			$"This knowledge now has the following learnability types: {_learnable.GetSingleFlags().Select(x => x.DescribeEnum().ColourValue()).ListToString()}");
		Changed = true;
		return true;
	}

	private bool BuildingCommandLearnDifficulty(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What difficulty should the test for a student in a teaching session be for this knowledge?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<Difficulty>(out var value))
		{
			actor.OutputHandler.Send("That is not a valid difficulty.");
			return false;
		}

		_learnDifficulty = value;
		actor.OutputHandler.Send($"It is now {value.Describe().ColourValue()} to learn this knowledge.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandLearnProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog did you want to set as the learnability prog?");
			return false;
		}

		var prog = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.SafeRemainingArgument);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (!prog.ReturnType.CompatibleWith(ProgVariableTypes.Boolean))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that returns a boolean value, whereas {prog.MXPClickableFunctionName()} returns {prog.ReturnType.Describe().ColourValue()}");
			return false;
		}

		if (!prog.MatchesParameters(new List<ProgVariableTypes> { ProgVariableTypes.Character }))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that accepts a single character as a parameter, whereas {prog.MXPClickableFunctionName()} does not.");
			return false;
		}

		_canLearnProg = prog;
		actor.OutputHandler.Send(
			$"The knowledge {Name.ColourName()} will now use the {prog.MXPClickableFunctionNameWithId()} prog to determine whether it can be learned.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandTeachDifficulty(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What difficulty should the test for a teacher in a teaching session be for this knowledge?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<Difficulty>(out var value))
		{
			actor.OutputHandler.Send("That is not a valid difficulty.");
			return false;
		}

		_teachDifficulty = value;
		actor.OutputHandler.Send($"It is now {value.Describe().ColourValue()} to teach this knowledge.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandChargenProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog did you want to set as the chargen availability prog?");
			return false;
		}

		var prog = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.SafeRemainingArgument);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (!prog.ReturnType.CompatibleWith(ProgVariableTypes.Boolean))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that returns a boolean value, whereas {prog.MXPClickableFunctionName()} returns {prog.ReturnType.Describe().ColourValue()}");
			return false;
		}

		if (!prog.MatchesParameters(new List<ProgVariableTypes>
			    { ProgVariableTypes.Toon, ProgVariableTypes.Trait }))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that accepts a toon and a trait as a parameter, whereas {prog.MXPClickableFunctionName()} does not.");
			return false;
		}

		_canPickChargenProg = prog;
		actor.OutputHandler.Send(
			$"The knowledge {Name.ColourName()} will now use the {prog.MXPClickableFunctionNameWithId()} prog to determine whether it can be picked in chargen.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandResource(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which account resource do you want to set a chargen cost for?");
			return false;
		}

		var resource = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.ChargenResources.Get(value)
			: Gameworld.ChargenResources.GetByName(command.Last);
		if (resource == null)
		{
			actor.OutputHandler.Send("There is no such account resource.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should the cost for this resource be? Use 0 to remove a cost.");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var cost) || cost < 0)
		{
			actor.OutputHandler.Send("You must enter a valid number for the cost.");
			return false;
		}

		if (cost == 0)
		{
			_resourceCosts.Remove(resource);
			actor.OutputHandler.Send(
				$"The {Name.ColourName()} knowledge will no longer cost any {resource.PluralName.ColourValue()}.");
		}
		else
		{
			_resourceCosts[resource] = cost;
			actor.OutputHandler.Send(
				$"The {Name.ColourName()} knowledge will now cost {$"{cost.ToString("N0", actor)} {(cost == 1 ? resource.Name : resource.PluralName)}".ColourValue()} when selected in character creation.");
		}

		Changed = true;
		return true;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Knowledge #{Id.ToString("N0", actor)} - {Name.ColourName()}");
		sb.AppendLine($"Description: {Description.ColourCommand()}");
		sb.AppendLine($"Type: {KnowledgeType.ColourValue()}");
		sb.AppendLine($"Subtype: {KnowledgeSubtype.ColourValue()}");
		sb.AppendLine($"Learnable: {Learnable.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Lessons Required: {LearnerSessionsRequired.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Learn Difficulty: {LearnDifficulty.Describe().ColourValue()}");
		sb.AppendLine($"Teach Difficulty: {TeachDifficulty.Describe().ColourValue()}");
		sb.AppendLine($"Can Learn Prog: {CanLearnProg?.MXPClickableFunctionNameWithId() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine(
			$"Chargen Pick Prog: {CanPickChargenProg?.MXPClickableFunctionNameWithId() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine();
		sb.AppendLine("Long Description:");
		sb.AppendLine();
		sb.AppendLine(LongDescription.Wrap(actor.InnerLineFormatLength));
		sb.AppendLine();
		sb.AppendLine("Chargen Costs:");
		if (_resourceCosts.Count == 0)
		{
			sb.AppendLine("\tNone");
		}
		else
		{
			foreach (var cost in _resourceCosts)
			{
				sb.AppendLine(
					$"\t{cost.Value.ToString("N0", actor)} {(cost.Value == 1 ? cost.Key.Name : cost.Key.PluralName)}"
						.ColourValue());
			}
		}

		return sb.ToString();
	}

	#endregion
}
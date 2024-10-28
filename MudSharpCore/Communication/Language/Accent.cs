using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore.Internal;
using MudSharp.Character;
using MudSharp.CharacterCreation;
using MudSharp.Commands;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.RPG.Checks;

namespace MudSharp.Communication.Language;

public class Accent : SaveableItem, IAccent
{
	public Accent(MudSharp.Models.Accent accent, Language language, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = accent.Id;
		_name = accent.Name;
		AccentSuffix = accent.Suffix;
		VagueSuffix = accent.VagueSuffix;
		Difficulty = (Difficulty)accent.Difficulty;
		Description = accent.Description;
		Language = language;
		Group = accent.Group;
		ChargenAvailabilityProg = gameworld.FutureProgs.Get(accent.ChargenAvailabilityProgId ?? 0);
	}

	public Accent(IFuturemud gameworld, ILanguage language, string name)
	{
		Gameworld = gameworld;
		_name = name;
		Language = language;
		AccentSuffix = $"with {name.A_An()} accent";
		VagueSuffix = "in an unknown accent";
		Difficulty = Difficulty.Normal;
		Description = "An undescribed accent";
		Group = "other";
		using (new FMDB())
		{
			var dbitem = new Models.Accent
			{
				Name = Name,
				LanguageId = language.Id,
				Suffix = AccentSuffix,
				VagueSuffix = VagueSuffix,
				Difficulty = (int)Difficulty,
				Description = Description,
				Group = Group
			};
			FMDB.Context.Accents.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public override string FrameworkItemType => "Accent";

	/// <summary>
	///     The suffix added to the language string in vocal echoes, of a form such as "with a southern drawl"
	/// </summary>
	public string AccentSuffix { get; protected set; }

	/// <summary>
	///     The suffix added to the language string in vocal echoes when the listener is unfamiliar with the accent, of a form
	///     such as "with an american accent"
	/// </summary>
	public string VagueSuffix { get; protected set; }


	public string Description { get; protected set; }

	/// <summary>
	///     The accent group to which this accent belongs, such as American, English, Australian. Speakers of related accents
	///     have an easier time understanding it.
	/// </summary>
	public string Group { get; protected set; }

	public ILanguage Language { get; protected set; }

	/// <summary>
	///     The difficulty of understanding this accent if it is one with which you are unfamiliar
	/// </summary>
	public Difficulty Difficulty { get; protected set; }

	public IFutureProg ChargenAvailabilityProg { get; protected set; }

	public bool IsAvailableInChargen(ICharacterTemplate template)
	{
		return ChargenAvailabilityProg?.Execute<bool?>(template) != false;
	}

	#region IFutureProgVariable Members

	public IProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "id":
				return new NumberVariable(Id);
			case "name":
				return new TextVariable(Name);
			case "language":
				return Language;
			case "suffix":
				return new TextVariable(AccentSuffix);
			case "vague":
				return new TextVariable(VagueSuffix);
			case "difficulty":
				return new NumberVariable((int)Difficulty);
			case "group":
				return new TextVariable(Group);
			case "description":
				return new TextVariable(Description);
			default:
				throw new NotSupportedException();
		}
	}

	public ProgVariableTypes Type => ProgVariableTypes.Accent;

	public object GetObject => this;

	private static ProgVariableTypes DotReferenceHandler(string property)
	{
		ProgVariableTypes returnVar;
		switch (property.ToLowerInvariant())
		{
			case "id":
				returnVar = ProgVariableTypes.Number;
				break;
			case "name":
				returnVar = ProgVariableTypes.Text;
				break;
			case "language":
				returnVar = ProgVariableTypes.Language;
				break;
			case "suffix":
				returnVar = ProgVariableTypes.Text;
				break;
			case "vague":
				returnVar = ProgVariableTypes.Text;
				break;
			case "difficulty":
				returnVar = ProgVariableTypes.Number;
				break;
			case "group":
				returnVar = ProgVariableTypes.Text;
				break;
			case "description":
				returnVar = ProgVariableTypes.Text;
				break;
			default:
				return ProgVariableTypes.Error;
		}

		return returnVar;
	}

	private static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", ProgVariableTypes.Number },
			{ "name", ProgVariableTypes.Text },
			{ "language", ProgVariableTypes.Language },
			{ "suffix", ProgVariableTypes.Text },
			{ "vague", ProgVariableTypes.Text },
			{ "difficulty", ProgVariableTypes.Number },
			{ "group", ProgVariableTypes.Text },
			{ "description", ProgVariableTypes.Text }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", "" },
			{ "name", "" },
			{ "language", "" },
			{ "suffix", "" },
			{ "vague", "" },
			{ "difficulty", "" },
			{ "group", "" },
			{ "description", "" }
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Accent, DotReferenceHandler(),
			DotReferenceHelp());
	}

	#endregion

	#region Overrides of SaveableItem

	public override void Save()
	{
		var dbitem = FMDB.Context.Accents.Find(Id);
		dbitem.Name = Name;
		dbitem.Suffix = AccentSuffix;
		dbitem.VagueSuffix = VagueSuffix;
		dbitem.Difficulty = (int)Difficulty;
		dbitem.Description = Description;
		dbitem.Group = Group;
		Changed = false;
	}

	#endregion

	#region Implementation of IEditableItem

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "desc":
			case "description":
				return BuildingCommandDescription(actor, command);
			case "suffix":
				return BuildingCommandSuffix(actor, command);
			case "vague":
				return BuildingCommandVague(actor, command);
			case "difficulty":
				return BuildingCommandDifficulty(actor, command);
			case "group":
				return BuildingCommandGroup(actor, command);
			case "prog":
			case "chargen":
				return BuildingCommandChargen(actor, command);
			default:
				actor.OutputHandler.Send(@"The valid options for this command are:

    name <name>
    description <description>
    suffix <suffix>
    vague <suffix>
    difficulty <difficulty>
    group <group>
    chargen <prog>");
				return false;
		}
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to this accent?");
			return false;
		}

		var name = command.SafeRemainingArgument.ToLowerInvariant();
		if (Language.Accents.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				$"There is already an accent of the {Language.Name.TitleCase().ColourName()} language with that name. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send(
			$"You rename the {_name.TitleCase().ColourName()} accent of the {Language.Name.TitleCase().ColourName()} language to {name.TitleCase().ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	private bool BuildingCommandDescription(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What description do you want to set for this accent?");
			return false;
		}

		Description = command.SafeRemainingArgument;
		actor.OutputHandler.Send(
			$"You set the description of the {_name.TitleCase().ColourName()} accent of the {Language.Name.TitleCase().ColourName()} language to {Description.ColourCommand()}.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandSuffix(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What suffix do you want to set for this accent?");
			return false;
		}

		AccentSuffix = command.SafeRemainingArgument;
		actor.OutputHandler.Send(
			$"You set the suffix of the {_name.TitleCase().ColourName()} accent of the {Language.Name.TitleCase().ColourName()} language to {AccentSuffix.ColourCommand()}.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandVague(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What vague suffix do you want to set for this accent?");
			return false;
		}

		VagueSuffix = command.SafeRemainingArgument;
		actor.OutputHandler.Send(
			$"You set the vague suffix of the {_name.TitleCase().ColourName()} accent of the {Language.Name.TitleCase().ColourName()} language to {VagueSuffix.ColourCommand()}.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandDifficulty(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What difficulty do you want to set for those unfamiliar with this accent to understand it?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<Difficulty>(out var value))
		{
			actor.OutputHandler.Send("That is not a valid difficulty.");
			return false;
		}

		Difficulty = value;
		actor.OutputHandler.Send(
			$"It will now be {Difficulty.Describe().ColourValue()} for those unfamiliar with this accent to understand it.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandGroup(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What accent group do you want to set for this accent?");
			return false;
		}

		Group = command.SafeRemainingArgument.ToLowerInvariant();
		actor.OutputHandler.Send(
			$"You set the group of the {_name.TitleCase().ColourName()} accent of the {Language.Name.TitleCase().ColourName()} language to {Group.TitleCase().ColourCommand()}.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandChargen(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What prog do you want to use to determine this accent's availability in chargen?");
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
				$"You must specify a prog that returns a boolean, whereas {prog.MXPClickableFunctionName()} returns {prog.ReturnType.Describe().ColourName()}.");
			return false;
		}

		if (!prog.MatchesParameters(new List<ProgVariableTypes> { ProgVariableTypes.Chargen }))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that takes a single chargen parameter, whereas {prog.MXPClickableFunctionName()} does not.");
			return false;
		}

		ChargenAvailabilityProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This accent will now use the prog {prog.MXPClickableFunctionName()} to control availability in chargen.");
		return true;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Accent #{Id.ToString("N0", actor)} - {Name.TitleCase().ColourName()}");
		sb.AppendLine($"Language: {Language.Name.TitleCase().ColourName()}");
		sb.AppendLine($"Group: {Group.TitleCase().ColourValue()}");
		sb.AppendLine($"Suffix: {AccentSuffix.ColourValue()}");
		sb.AppendLine($"Vague: {VagueSuffix.ColourValue()}");
		sb.AppendLine($"Difficulty: {Difficulty.Describe().ColourValue()}");
		sb.AppendLine(
			$"Chargen Prog: {ChargenAvailabilityProg?.MXPClickableFunctionNameWithId() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine($"Description: {Description.ColourCommand()}");
		return sb.ToString();
	}

	#endregion
}
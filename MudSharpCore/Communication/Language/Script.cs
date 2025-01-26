using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore.Internal;
using MudSharp.Character;
using MudSharp.Commands.Trees;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg.Variables;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.RPG.Knowledge;
using Org.BouncyCastle.Asn1.Pkcs;

namespace MudSharp.Communication.Language;

public class Script : SaveableItem, IScript
{
	public Script(IFuturemud gameworld, string name, IKnowledge knowledge)
	{
		Gameworld = gameworld;
		_name = name;
		ScriptKnowledge = knowledge;
		KnownScriptDescription = $"the {name} script";
		UnknownScriptDescription = "an unknown script";
		DocumentLengthModifier = 1.0;
		InkUseModifier = 1.0;
		using (new FMDB())
		{
			var dbitem = new Models.Script
			{
				Name = name,
				UnknownScriptDescription = UnknownScriptDescription,
				KnownScriptDescription = KnownScriptDescription,
				InkUseModifier = InkUseModifier,
				DocumentLengthModifier = DocumentLengthModifier,
				KnowledgeId = ScriptKnowledge.Id
			};
			FMDB.Context.Scripts.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public Script(MudSharp.Models.Script script, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = script.Id;
		_name = script.Name;
		ScriptKnowledge = Gameworld.Knowledges.Get(script.KnowledgeId);
		foreach (var item in script.ScriptsDesignedLanguages)
		{
			_designedLanguages.Add(Gameworld.Languages.Get(item.LanguageId));
		}

		UnknownScriptDescription = script.UnknownScriptDescription;
		KnownScriptDescription = script.KnownScriptDescription;
		DocumentLengthModifier = script.DocumentLengthModifier;
		InkUseModifier = script.InkUseModifier;
	}

	#region Overrides of Item

	public override string FrameworkItemType => "Script";

	#endregion

	#region Overrides of SaveableItem

	public override void Save()
	{
		var dbitem = FMDB.Context.Scripts.Find(Id);
		dbitem.Name = Name;
		dbitem.InkUseModifier = InkUseModifier;
		dbitem.DocumentLengthModifier = DocumentLengthModifier;
		dbitem.UnknownScriptDescription = UnknownScriptDescription;
		dbitem.KnownScriptDescription = KnownScriptDescription;
		dbitem.KnowledgeId = ScriptKnowledge.Id;
		FMDB.Context.ScriptsDesignedLanguages.RemoveRange(dbitem.ScriptsDesignedLanguages);
		foreach (var language in _designedLanguages)
		{
			dbitem.ScriptsDesignedLanguages.Add(new ScriptsDesignedLanguage
				{ LanguageId = language.Id, Script = dbitem });
		}

		Changed = false;
	}

	#endregion

	#region Implementation of IScript

	public IKnowledge ScriptKnowledge { get; set; }
	private readonly List<ILanguage> _designedLanguages = new();
	public IEnumerable<ILanguage> DesignedLanguages => _designedLanguages;
	public string UnknownScriptDescription { get; set; }
	public string KnownScriptDescription { get; set; }
	public double DocumentLengthModifier { get; set; }
	public double InkUseModifier { get; set; }

	#endregion

	#region Implementation of IEditableItem

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "knowledge":
				return BuildingCommandKnowledge(actor, command);
			case "length":
			case "len":
				return BuildingCommandLength(actor, command);
			case "ink":
				return BuildingCommandInk(actor, command);
			case "known":
				return BuildingCommandKnown(actor, command);
			case "unknown":
				return BuildingCommandUnknown(actor, command);
			case "language":
				return BuildingCommandLanguage(actor, command);
			default:
				actor.OutputHandler.Send(@"You can use the following options with this building sub-command:

	name <name> - gives a new name to the script
	knowledge <knowledge> - sets a new knowledge to control who has this script
	length <%> - sets a modifier for document length using this script
	ink <%> - sets a modifier for ink usage using this script
	known <desc> - sets how this script is displayed when the reader knows what it is, e.g. ""the Latin script""
	unknown <desc> - sets how this script is displayed when the reader doesn't know it, e.g. ""an Asian script""
	language <which> - toggles the inclusion of the specified language as a language this script is designed to show");
				return false;
		}
	}

	private bool BuildingCommandLanguage(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which language do you want to toggle as a designed language for this script?");
			return false;
		}

		var language = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.Languages.Get(value)
			: Gameworld.Languages.GetByName(command.SafeRemainingArgument);
		if (language == null)
		{
			actor.OutputHandler.Send("There is no such language.");
			return false;
		}

		if (_designedLanguages.Contains(language))
		{
			_designedLanguages.Remove(language);
			actor.OutputHandler.Send(
				$"The {language.Name.ColourName()} language is no longer a designed language for the {Name.ColourName()} script.");
		}
		else
		{
			_designedLanguages.Add(language);
			actor.OutputHandler.Send(
				$"The {language.Name.ColourName()} language is now a designed language for the {Name.ColourName()} script.");
		}

		Changed = true;
		return true;
	}

	private bool BuildingCommandUnknown(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to set the unknown description of this script to?");
			return false;
		}

		UnknownScriptDescription = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send(
			$"This script will now be shown as {UnknownScriptDescription.ColourCommand()} when the reader doesn't know what it is.");
		return true;
	}

	private bool BuildingCommandKnown(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to set the known description of this script to?");
			return false;
		}

		KnownScriptDescription = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send(
			$"This script will now be shown as {KnownScriptDescription.ColourCommand()} when the reader knows what it is.");
		return true;
	}

	private bool BuildingCommandInk(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What percentage modifier to base ink/paint/pencil usage do you want to apply when using this script?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(out var value))
		{
			actor.OutputHandler.Send("That is not a valid percentage.");
			return false;
		}

		InkUseModifier = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This script will now have a {InkUseModifier.ToString("P2", actor).ColourValue()} modifier to ink consumption.");
		return true;
	}

	private bool BuildingCommandLength(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What percentage modifier to base document length do you want to apply when using this script?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(out var value))
		{
			actor.OutputHandler.Send("That is not a valid percentage.");
			return false;
		}

		DocumentLengthModifier = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This script will now have a {DocumentLengthModifier.ToString("P2", actor).ColourValue()} modifier to document length.");
		return true;
	}

	private bool BuildingCommandKnowledge(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which knowledge do you want to use to control access to this script?");
			return false;
		}

		var knowledge = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.Knowledges.Get(value)
			: Gameworld.Knowledges.GetByName(command.SafeRemainingArgument);
		if (knowledge == null)
		{
			actor.OutputHandler.Send("There is no such knowledge.");
			return false;
		}

		ScriptKnowledge = knowledge;
		Changed = true;
		actor.OutputHandler.Send(
			$"This script will now require someone to have the {knowledge.Name.ColourName()} knowledge in order to use or understand it.");
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to this script?");
			return false;
		}

		var name = command.SafeRemainingArgument.ToLowerInvariant().TitleCase();
		if (Gameworld.Scripts.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already a script called {name.ColourValue()}. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename the script from {_name.ColourName()} to {name.ColourValue()}.");
		_name = name;
		Changed = true;
		return true;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Script #{Id.ToString("N0", actor)} - {Name.ColourName()}");
		sb.AppendLine($"Knowledge: {ScriptKnowledge.Name.ColourName()}");
		sb.AppendLine($"Known Desc: {KnownScriptDescription.ColourCommand()}");
		sb.AppendLine($"Unknown Desc: {UnknownScriptDescription.ColourCommand()}");
		sb.AppendLine($"Document Length Modifier: {DocumentLengthModifier.ToString("P2", actor).ColourValue()}");
		sb.AppendLine($"Ink Use Modifier: {InkUseModifier.ToString("P2", actor).ColourValue()}");
		sb.AppendLine();
		sb.AppendLine("Designed Languages:");
		foreach (var language in _designedLanguages)
		{
			sb.AppendLine($"\t{language.Name.ColourName()}");
		}

		return sb.ToString();
	}

	#endregion

	#region Implementation of IProgVariable
	public IProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "id":
				return new NumberVariable(Id);
			case "name":
				return new TextVariable(Name);
			case "languages":
				return new CollectionVariable(DesignedLanguages.ToList(), ProgVariableTypes.Language);
			case "knowledge":
				return ScriptKnowledge;
			case "unknownscriptdescription":
				return new TextVariable(UnknownScriptDescription);
			case "knownscriptdescription":
				return new TextVariable(KnownScriptDescription);
			case "documentlength":
				return new NumberVariable(DocumentLengthModifier);
			case "inkuse":
				return new NumberVariable(InkUseModifier);
			default:
				throw new NotSupportedException();
		}
	}

	public ProgVariableTypes Type => ProgVariableTypes.Script;

	public object GetObject => this;

	private static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", ProgVariableTypes.Number },
			{ "name", ProgVariableTypes.Text },
			{ "languages", ProgVariableTypes.Collection | ProgVariableTypes.Language},
			{ "knowledge", ProgVariableTypes.Knowledge},
			{ "unknownscriptdescription", ProgVariableTypes.Text},
			{ "knownscriptdescription", ProgVariableTypes.Text},
			{ "documentlength", ProgVariableTypes.Number},
			{ "inkuse", ProgVariableTypes.Number},
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", "The ID of the language" },
			{ "name", "The name of the language" },
			{ "languages", "All the languages this script is designed for"},
			{ "knowledge", "The knowledge required for this script"},
			{ "unknownscriptdescription", "The text description if someone doesn't know this script"},
			{ "knownscriptdescription", "The text description is someone does know this script"},
			{ "documentlength", "A multiplier for how much space on a page this script takes up"},
			{ "inkuse", "A multiplier for how much ink this script uses"},
		};
	}

	public new static void RegisterFutureProgCompiler()
	{
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Script, DotReferenceHandler(), DotReferenceHelp());
	}
	#endregion
}
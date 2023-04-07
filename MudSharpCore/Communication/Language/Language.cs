using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Communication.Language.DifficultyModels;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace MudSharp.Communication.Language;

public class Language : SaveableItem, ILanguage
{
	private readonly List<IAccent> _accents = new();
	private readonly Dictionary<long, Difficulty> _mutuallyIntelligableLanguages = new();

	public Language(MudSharp.Models.Language language, IFuturemud game)
	{
		Gameworld = game;
		_id = language.Id;
		_name = language.Name;
		UnknownLanguageSpokenDescription = language.UnknownLanguageDescription;
		LanguageObfuscationFactor = language.LanguageObfuscationFactor;
		LinkedTrait = game.Traits.Get(language.LinkedTraitId);
		Model = game.LanguageDifficultyModels.Get(language.DifficultyModel);
		foreach (var accent in language.Accents)
		{
			var newAccent = new Accent(accent, this, game);
			_accents.Add(newAccent);
			game.Add(newAccent);
		}

		DefaultLearnerAccent = _accents.FirstOrDefault(x => x.Id == language.DefaultLearnerAccentId);
		foreach (var item in language.MutualIntelligabilitiesListenerLanguage)
		{
			_mutuallyIntelligableLanguages[item.TargetLanguageId] = (Difficulty)item.IntelligabilityDifficulty;
		}
	}

	public Language(IFuturemud gameworld, ITraitDefinition linkedTrait, string name)
	{
		Gameworld = gameworld;
		_name = name;
		LanguageObfuscationFactor = 0.2;
		Model = Gameworld.LanguageDifficultyModels.First();
		LinkedTrait = linkedTrait;
		UnknownLanguageSpokenDescription = "an unknown language";
		using (new FMDB())
		{
			var dbitem = new Models.Language
			{
				Name = name,
				LanguageObfuscationFactor = LanguageObfuscationFactor,
				LinkedTraitId = LinkedTrait.Id,
				DifficultyModel = Model.Id,
				UnknownLanguageDescription = UnknownLanguageSpokenDescription
			};
			FMDB.Context.Languages.Add(dbitem);

			var dbaccent = new Models.Accent
			{
				Name = "foreign",
				Suffix = "with a Foreign accent",
				VagueSuffix = "with a Foreign accent",
				Difficulty = (int)Difficulty.Normal,
				Description = $"The accent of a foreigner who has learned but not yet mastered the {name} language",
				Group = "foreign",
				Language = dbitem
			};
			FMDB.Context.Accents.Add(dbaccent);
			FMDB.Context.SaveChanges();

			dbitem.DefaultLearnerAccent = dbaccent;
			_id = dbitem.Id;
			FMDB.Context.SaveChanges();

			var accent = new Accent(dbaccent, this, Gameworld);
			_accents.Add(accent);
			Gameworld.Add(accent);
			DefaultLearnerAccent = accent;
		}
	}

	public override string FrameworkItemType => "Language";
	public IEnumerable<IAccent> Accents => _accents;
	public ILanguageDifficultyModel Model { get; protected set; }
	public ITraitDefinition LinkedTrait { get; protected set; }

	public IAccent DefaultLearnerAccent { get; set; }

	#region IEditableItem Implementation

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Language #{Id.ToString("N0", actor)} - {Name.ColourName()}");
		sb.AppendLine($"Linked Trait: {LinkedTrait.Name.ColourValue()}");
		sb.AppendLine($"Difficulty Model: {Model.Name.ColourValue()}");
		sb.AppendLine($"Unknown Description: {UnknownLanguageSpokenDescription.ColourValue()}");
		sb.AppendLine($"Obfuscation Factor: {LanguageObfuscationFactor.ToString("P2", actor).ColourValue()}");
		sb.AppendLine(
			$"Default Learned Accent: {DefaultLearnerAccent.Name.ColourValue()} (#{DefaultLearnerAccent.Id.ToString("N0", actor)})");
		sb.AppendLine();
		sb.AppendLine("Mutually Intelligible Languages:");
		if (!_mutuallyIntelligableLanguages.Any())
		{
			sb.AppendLine("\tNone");
		}
		else
		{
			foreach (var language in _mutuallyIntelligableLanguages)
			{
				sb.AppendLine(
					$"\t{Gameworld.Languages.Get(language.Key).Name} ({language.Value.Describe().ColourValue()})");
			}
		}

		sb.AppendLine($"Accents:");
		foreach (var accent in _accents.OrderBy(x => x.Difficulty))
		{
			sb.AppendLine(
				$"\t{accent.Name} (#{accent.Id.ToString("N0", actor)}) - {accent.Difficulty.Describe().ColourValue()}");
		}

		sb.AppendLine($"Scripts:");
		foreach (var script in Gameworld.Scripts.Where(x => x.DesignedLanguages.Contains(this)))
		{
			sb.AppendLine($"\t{script.Name}");
		}

		return sb.ToString();
	}

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "unknown":
			case "description":
			case "desc":
				return BuildingCommandUnknownDescription(actor, command);
			case "trait":
			case "skill":
				return BuildingCommandTrait(actor, command);
			case "obfuscation":
				return BuildingCommandObfuscation(actor, command);
			case "model":
				return BuildingCommandModel(actor, command);
			case "default":
			case "accent":
				return BuildingCommandDefaultAccent(actor, command);
			case "mutual":
				return BuildingCommandMutual(actor, command);
			case "remove":
				return BuildingCommandRemove(actor, command);
			default:
				actor.OutputHandler.Send(
					"You can use the following options with this command:\n\n\tname <name> - sets the name\n\tunknown <text> - sets the \"unknown language description\"\n\ttrait <trait> - sets the skill associated with this language\n\tobfuscation <%> - sets the percentage of words that are obfuscated for each failure degree\n\tmodel <which> - sets a language sentence difficulty model\n\tdefault <accent> - sets a default learner accent\n\tmutual <language> <difficulty> - sets the other language as mutually intelligible to this language at specified minimum difficulty\n\tremove <language> - removes a mutual intelligibility");
				return false;
		}
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to this language?");
			return false;
		}

		var name = command.SafeRemainingArgument.ToLowerInvariant().TitleCase();
		if (Gameworld.Languages.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a language with that name. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename the {Name.ColourName()} language to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	private bool BuildingCommandUnknownDescription(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What text do you want to replace the name of the language to those who don't know it?\nNote: This would commonly be a construction beginning with a/an/the such as 'an unknown Germanic language'.");
			return false;
		}

		UnknownLanguageSpokenDescription = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send(
			$"When a listener does not know this language it will now be presented as {UnknownLanguageSpokenDescription.ColourValue()}.");
		return true;
	}

	private bool BuildingCommandTrait(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which trait (should almost always be a skill) do you want to act as the linked trait to this language?");
			return false;
		}

		var trait = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.Traits.Get(value)
			: Gameworld.Traits.GetByName(command.SafeRemainingArgument);
		if (trait == null)
		{
			actor.OutputHandler.Send("There is no such trait.");
			return false;
		}

		LinkedTrait = trait;
		Changed = true;
		actor.OutputHandler.Send(
			$"This language will now use the {LinkedTrait.Name.ColourValue()} trait to as its linked trait.");
		return true;
	}

	private bool BuildingCommandObfuscation(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What percentage of words in a sentence do you want to be obscured per degree of failure with this language? Typically this would be between {0.1.ToString("P0", actor).ColourValue()} and {0.3.ToString("P0", actor).ColourValue()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value))
		{
			actor.OutputHandler.Send("You must enter a valid percentage.");
			return false;
		}

		LanguageObfuscationFactor = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This language will now obfuscate {value.ToString("P", actor).ColourValue()} of words per degree of failure.");
		return true;
	}

	private bool BuildingCommandModel(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which difficulty model do you want this language to use?");
			return false;
		}

		var model = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.LanguageDifficultyModels.Get(value)
			: Gameworld.LanguageDifficultyModels.GetByName(command.SafeRemainingArgument);
		if (model == null)
		{
			actor.OutputHandler.Send("Which difficulty model do you want to use for this language?");
			return false;
		}

		Model = model;
		Changed = true;
		actor.OutputHandler.Send($"This language will now use the {model.Name.ColourValue()} difficulty model.");
		return true;
	}

	private bool BuildingCommandDefaultAccent(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which accent do you want to set as the default one which people who learn this language acquire?");
			return false;
		}

		var targetText = command.SafeRemainingArgument;
		var accent = long.TryParse(targetText, out var value)
			? _accents.FirstOrDefault(x => x.Id == value)
			: _accents.FirstOrDefault(x => x.Name.EqualTo(targetText));
		if (accent == null)
		{
			actor.OutputHandler.Send("That language has no such accent.");
			return false;
		}

		DefaultLearnerAccent = accent;
		Changed = true;
		actor.OutputHandler.Send(
			$"When characters learn this language in game they will now be given the {DefaultLearnerAccent.Name.ColourName()} accent.");
		return true;
	}

	private bool BuildingCommandMutual(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which other language do you want to set as intelligible by this language?");
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

		if (language == this)
		{
			actor.OutputHandler.Send("You cannot make a language mutually intelligible with itself.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What difficulty do you want to set as the minimum understanding difficulty when using this language for mutual intelligiblility?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<Difficulty>(out var difficulty))
		{
			actor.OutputHandler.Send(
				$"That is not a valid difficulty. See {"show difficulties".FluentTagMXP("send", "href='show difficulties' hint='List all the valid difficulties'")}.");
			return false;
		}

		_mutuallyIntelligableLanguages[language.Id] = difficulty;
		Changed = true;
		actor.OutputHandler.Send(
			$"Speakers of this language will now be able to understand people speaking {language.Name.ColourName()} at a minimum difficulty of {difficulty.Describe().ColourValue()}.");
		return true;
	}

	private bool BuildingCommandRemove(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which other language do you want to remove as mutually intelligible by this language?");
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

		if (!_mutuallyIntelligableLanguages.ContainsKey(language.Id))
		{
			actor.OutputHandler.Send(
				$"The {language.Name.ColourName()} language is not mutually intelligible by this language.");
			return false;
		}

		_mutuallyIntelligableLanguages.Remove(language.Id);
		Changed = true;
		actor.OutputHandler.Send(
			$"The {language.Name.ColourName()} language is no longer mutually intelligible by this language.");
		return true;
	}

	#endregion

	/// <summary>
	///     The string to display when a spoken language is not known to its perceiver, e.g. "an unknown tongue", or "a
	///     glottal, face-paced language"
	/// </summary>
	public string UnknownLanguageSpokenDescription { get; protected set; }

	/// <summary>
	///     Provides a base multiplier in the Language Obfuscation code for failures to understand this language
	/// </summary>
	public double LanguageObfuscationFactor { get; protected set; }

	public Difficulty MutualIntelligability(ILanguage otherLanguage)
	{
		if (_mutuallyIntelligableLanguages.ContainsKey(otherLanguage.Id))
		{
			return _mutuallyIntelligableLanguages[otherLanguage.Id];
		}

		return Difficulty.Impossible;
	}

	#region IFutureProgVariable Members

	public IFutureProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "id":
				return new NumberVariable(Id);
			case "name":
				return new TextVariable(Name);
			case "trait":
				return LinkedTrait;
			case "accents":
				return new CollectionVariable(Accents.ToList(), FutureProgVariableTypes.Accent);
			case "defaultaccent":
				return DefaultLearnerAccent;
			case "unknown":
				return new TextVariable(UnknownLanguageSpokenDescription);
			default:
				throw new NotSupportedException();
		}
	}

	public FutureProgVariableTypes Type => FutureProgVariableTypes.Language;

	public object GetObject => this;

	private static IReadOnlyDictionary<string, FutureProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, FutureProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", FutureProgVariableTypes.Number },
			{ "name", FutureProgVariableTypes.Text },
			{ "trait", FutureProgVariableTypes.Trait },
			{ "accents", FutureProgVariableTypes.Accent | FutureProgVariableTypes.Collection },
			{ "defaultaccent", FutureProgVariableTypes.Text },
			{ "unknown", FutureProgVariableTypes.Text }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", "" },
			{ "name", "" },
			{ "trait", "" },
			{ "accents", "" },
			{ "defaultaccent", "" },
			{ "unknown", "" }
		};
	}

	public new static void RegisterFutureProgCompiler()
	{
		FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.Language, DotReferenceHandler(),
			DotReferenceHelp());
	}

	#endregion

	#region Overrides of SaveableItem

	public override void Save()
	{
		var dbitem = FMDB.Context.Languages.Find(Id);
		dbitem.Name = Name;
		dbitem.DefaultLearnerAccentId = DefaultLearnerAccent?.Id;
		dbitem.LanguageObfuscationFactor = LanguageObfuscationFactor;
		dbitem.DifficultyModel = Model.Id;
		dbitem.UnknownLanguageDescription = UnknownLanguageSpokenDescription;
		dbitem.LinkedTraitId = LinkedTrait.Id;
		FMDB.Context.MutualIntelligabilities.RemoveRange(dbitem.MutualIntelligabilitiesListenerLanguage);
		foreach (var intelligible in _mutuallyIntelligableLanguages)
		{
			FMDB.Context.MutualIntelligabilities.Add(new MutualIntelligability
			{
				ListenerLanguage = dbitem,
				TargetLanguageId = intelligible.Key,
				IntelligabilityDifficulty = (int)intelligible.Value
			});
		}

		Changed = false;
	}

	#endregion
}
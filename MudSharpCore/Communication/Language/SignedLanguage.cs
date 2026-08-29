#nullable enable

using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Communication.Language.DifficultyModels;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Form.Shape;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.RPG.Checks;

namespace MudSharp.Communication.Language;

public class SignedLanguage : SaveableItem, ISignedLanguage
{
	private readonly List<ISignedLanguageVariety> _varieties = [];
	private readonly List<ISignedLanguageArticulationProfile> _articulationProfiles = [];
	private readonly Dictionary<long, Difficulty> _mutualIntelligibilities = [];

	public SignedLanguage(MudSharp.Models.SignedLanguage language, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = language.Id;
		_name = language.Name;
		UnknownLanguageDescription = language.UnknownLanguageDescription;
		LanguageObfuscationFactor = language.LanguageObfuscationFactor;
		LinkedTrait = gameworld.Traits.Get(language.LinkedTraitId)!;
		Model = gameworld.LanguageDifficultyModels.Get(language.DifficultyModelId)!;
		_varieties.AddRange(language.Varieties.Select(x => new SignedLanguageVariety(x, this)));
		_articulationProfiles.AddRange(language.ArticulationProfiles.Select(x =>
			new SignedLanguageArticulationProfile(x, this, gameworld)));
		foreach (var mutual in language.MutualIntelligibilitiesListenerLanguage)
		{
			_mutualIntelligibilities[mutual.TargetLanguageId] = (Difficulty)mutual.IntelligibilityDifficulty;
		}
	}

	public SignedLanguage(IFuturemud gameworld, ITraitDefinition linkedTrait, string name)
	{
		Gameworld = gameworld;
		_name = name.TitleCase();
		LinkedTrait = linkedTrait;
		Model = gameworld.LanguageDifficultyModels.First();
		UnknownLanguageDescription = "an unfamiliar signed language";
		LanguageObfuscationFactor = 0.2;
		using (new FMDB())
		{
			var dbitem = new MudSharp.Models.SignedLanguage
			{
				Name = _name,
				LinkedTraitId = linkedTrait.Id,
				DifficultyModelId = Model.Id,
				UnknownLanguageDescription = UnknownLanguageDescription,
				LanguageObfuscationFactor = LanguageObfuscationFactor
			};
			FMDB.Context.SignedLanguages.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public override string FrameworkItemType => "SignedLanguage";
	public ILanguageDifficultyModel Model { get; private set; }
	public ITraitDefinition LinkedTrait { get; private set; }
	public string UnknownLanguageDescription { get; private set; }
	public double LanguageObfuscationFactor { get; private set; }
	public IEnumerable<ISignedLanguageVariety> Varieties => _varieties;
	public IEnumerable<ISignedLanguageArticulationProfile> ArticulationProfiles => _articulationProfiles;

	public Difficulty MutualIntelligability(ISignedLanguage otherLanguage) =>
		_mutualIntelligibilities.GetValueOrDefault(otherLanguage.Id, Difficulty.Impossible);

	public SignedLanguageArticulationResult EvaluateArticulation(IBody body)
	{
		var profiles = _articulationProfiles
			.Where(x => body.Prototype == x.BodyPrototype || body.Prototype.CountsAs(x.BodyPrototype))
			.Select(x => x.Evaluate(body))
			.ToList();
		if (profiles.Count == 0)
		{
			return SignedLanguageArticulationResult.Impossible(
				$"Your body does not have an articulation profile for {Name.TitleCase()}.");
		}

		return profiles
			.Where(x => x.CanSign)
			.OrderBy(x => x.MissingPreferredParts)
			.FirstOrDefault(SignedLanguageArticulationResult.Impossible(
				$"You do not currently have enough functional body parts to sign in {Name.TitleCase()}."));
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Signed Language #{Id.ToString("N0", actor)} - {Name.ColourName()}");
		sb.AppendLine($"Linked Trait: {LinkedTrait.Name.ColourValue()}");
		sb.AppendLine($"Difficulty Model: {Model.Name.ColourValue()}");
		sb.AppendLine($"Unknown Description: {UnknownLanguageDescription.ColourValue()}");
		sb.AppendLine($"Obfuscation Factor: {LanguageObfuscationFactor.ToString("P2", actor).ColourValue()}");
		sb.AppendLine($"Varieties: {_varieties.Select(x => x.Name.ColourName()).ListToString().IfNullOrWhiteSpace("None")}");
		sb.AppendLine("Articulation Profiles:");
		foreach (var profile in _articulationProfiles)
		{
			sb.AppendLine($"\t{profile.Name.ColourName()} ({profile.BodyPrototype.Name.ColourValue()}): " +
				profile.Requirements.Select(x => $"{x.BodypartShape.Name} {x.MinimumCount}/{x.PreferredCount}")
				.ListToString().IfNullOrWhiteSpace("no requirements"));
		}
		sb.AppendLine("Mutually Intelligible Signed Languages:");
		foreach (var mutual in _mutualIntelligibilities)
		{
			var languageName = Gameworld.SignedLanguages.Get(mutual.Key)?.Name.ColourName() ?? $"#{mutual.Key:N0}";
			sb.AppendLine($"\t{languageName} ({mutual.Value.Describe().ColourValue()})");
		}
		return sb.ToString();
	}

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "name":
				if (command.IsFinished)
				{
					actor.OutputHandler.Send("What new name should this signed language have?");
					return false;
				}
				var name = command.SafeRemainingArgument.TitleCase();
				if (Gameworld.SignedLanguages.Any(x => x != this && x.Name.EqualTo(name)))
				{
					actor.OutputHandler.Send("There is already a signed language with that name.");
					return false;
				}
				_name = name;
				Changed = true;
				actor.OutputHandler.Send($"You rename the signed language to {Name.ColourName()}.");
				return true;
			case "unknown":
			case "description":
				if (command.IsFinished)
				{
					actor.OutputHandler.Send("What description should observers see when they do not recognise this signed language?");
					return false;
				}
				UnknownLanguageDescription = command.SafeRemainingArgument;
				Changed = true;
				actor.OutputHandler.Send($"The unknown description is now {UnknownLanguageDescription.ColourValue()}.");
				return true;
			case "obfuscation":
				if (command.IsFinished || !command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var factor))
				{
					actor.OutputHandler.Send("You must specify a valid obfuscation percentage.");
					return false;
				}
				LanguageObfuscationFactor = factor;
				Changed = true;
				actor.OutputHandler.Send($"The obfuscation factor is now {factor.ToString("P2", actor).ColourValue()}.");
				return true;
			case "trait":
				var trait = Gameworld.Traits.GetByIdOrName(command.SafeRemainingArgument);
				if (trait is null)
				{
					actor.OutputHandler.Send("There is no such trait.");
					return false;
				}
				LinkedTrait = trait;
				Changed = true;
				actor.OutputHandler.Send($"This signed language now uses {trait.Name.ColourName()}.");
				return true;
			case "model":
				var model = Gameworld.LanguageDifficultyModels.GetByIdOrName(command.SafeRemainingArgument);
				if (model is null)
				{
					actor.OutputHandler.Send("There is no such language difficulty model.");
					return false;
				}
				Model = model;
				Changed = true;
				actor.OutputHandler.Send($"This signed language now uses {model.Name.ColourName()}.");
				return true;
			case "mutual":
				var mutualLanguage = Gameworld.SignedLanguages.GetByIdOrName(command.PopSpeech());
				if (mutualLanguage is null || mutualLanguage == this || command.IsFinished ||
				    !command.SafeRemainingArgument.TryParseEnum<Difficulty>(out var mutualDifficulty))
				{
					actor.OutputHandler.Send("Specify another signed language and a valid difficulty. This relationship is directional.");
					return false;
				}
				_mutualIntelligibilities[mutualLanguage.Id] = mutualDifficulty;
				Changed = true;
				actor.OutputHandler.Send($"Users of this language can now understand {mutualLanguage.Name.ColourName()} at {mutualDifficulty.Describe().ColourValue()} difficulty.");
				return true;
			case "removemutual":
				var removedMutual = Gameworld.SignedLanguages.GetByIdOrName(command.SafeRemainingArgument);
				if (removedMutual is null || !_mutualIntelligibilities.Remove(removedMutual.Id))
				{
					actor.OutputHandler.Send("This language has no such mutual-intelligibility link.");
					return false;
				}
				Changed = true;
				actor.OutputHandler.Send($"You remove the directional link to {removedMutual.Name.ColourName()}.");
				return true;
			case "variety":
				return BuildingCommandVariety(actor, command);
			case "profile":
				return BuildingCommandProfile(actor, command);
			default:
				actor.OutputHandler.Send(@"You can use the following settings:

	name <name>
	unknown <description>
	obfuscation <percentage>
	trait <trait>
	model <difficulty model>
	mutual <signed language> <difficulty>
	removemutual <signed language>
	variety add <name> <description> <suffix> <vague suffix> <difficulty>
	variety remove <name>
	profile add <name> <body prototype> <bodypart shape> <minimum> <preferred>
	profile requirement <profile> <bodypart shape> <minimum> <preferred>
	profile remove <name>".SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandVariety(ICharacter actor, StringStack command)
	{
		var action = command.PopSpeech().ToLowerInvariant();
		var name = command.PopSpeech().TitleCase();
		if (action == "remove")
		{
			var existing = _varieties.FirstOrDefault(x => x.Name.EqualTo(name));
			if (existing is null)
			{
				actor.OutputHandler.Send("There is no such variety.");
				return false;
			}
			using (new FMDB())
			{
				FMDB.Context.SignedLanguageVarieties.Remove(FMDB.Context.SignedLanguageVarieties.Find(existing.Id)!);
				FMDB.Context.SaveChanges();
			}
			_varieties.Remove(existing);
			actor.OutputHandler.Send($"You remove the {name.ColourName()} variety.");
			return true;
		}

		var description = command.PopSpeech();
		var suffix = command.PopSpeech();
		var vagueSuffix = command.PopSpeech();
		if (action != "add" || name.Length == 0 || description.Length == 0 || suffix.Length == 0 || vagueSuffix.Length == 0 ||
		    !command.SafeRemainingArgument.TryParseEnum<Difficulty>(out var difficulty))
		{
			actor.OutputHandler.Send("Use: variety add <name> <description> <suffix> <vague suffix> <difficulty>. Quote arguments containing spaces.");
			return false;
		}
		if (_varieties.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a variety with that name.");
			return false;
		}
		using (new FMDB())
		{
			var dbitem = new MudSharp.Models.SignedLanguageVariety
			{
				SignedLanguageId = Id, Name = name, Description = description, Suffix = suffix,
				VagueSuffix = vagueSuffix, RecognitionDifficulty = (int)difficulty
			};
			FMDB.Context.SignedLanguageVarieties.Add(dbitem);
			FMDB.Context.SaveChanges();
			_varieties.Add(new SignedLanguageVariety(dbitem, this));
		}
		actor.OutputHandler.Send($"You add the {name.ColourName()} variety.");
		return true;
	}

	private bool BuildingCommandProfile(ICharacter actor, StringStack command)
	{
		var action = command.PopSpeech().ToLowerInvariant();
		var name = command.PopSpeech().TitleCase();
		if (action == "requirement")
		{
			var existing = _articulationProfiles
				.OfType<SignedLanguageArticulationProfile>()
				.FirstOrDefault(x => x.Name.EqualTo(name));
			var requirementShape = Gameworld.BodypartShapes.GetByIdOrName(command.PopSpeech());
			var requirementMinimumText = command.PopSpeech();
			var requirementPreferredText = command.PopSpeech();
			if (existing is null || requirementShape is null ||
			    !int.TryParse(requirementMinimumText, out var requirementMinimum) ||
			    !int.TryParse(requirementPreferredText, out var requirementPreferred) ||
			    requirementMinimum < 0 || requirementPreferred < requirementMinimum)
			{
				actor.OutputHandler.Send("Use: profile requirement <profile> <bodypart shape> <minimum> <preferred>. Quote names containing spaces.");
				return false;
			}
			using (new FMDB())
			{
				var dbitem = FMDB.Context.SignedLanguageArticulationRequirements.Find(existing.Id, requirementShape.Id);
				if (dbitem is null)
				{
					dbitem = new MudSharp.Models.SignedLanguageArticulationRequirement
					{
						ArticulationProfileId = existing.Id,
						BodypartShapeId = requirementShape.Id
					};
					FMDB.Context.SignedLanguageArticulationRequirements.Add(dbitem);
				}
				dbitem.MinimumCount = requirementMinimum;
				dbitem.PreferredCount = requirementPreferred;
				FMDB.Context.SaveChanges();
				existing.SetRequirement(dbitem, Gameworld);
			}
			actor.OutputHandler.Send(
				$"The {name.ColourName()} profile now requires at least {requirementMinimum.ToString("N0", actor).ColourValue()} and prefers {requirementPreferred.ToString("N0", actor).ColourValue()} functional {requirementShape.Name.Pluralise().ColourName()}.");
			return true;
		}
		if (action == "remove")
		{
			var existing = _articulationProfiles.FirstOrDefault(x => x.Name.EqualTo(name));
			if (existing is null)
			{
				actor.OutputHandler.Send("There is no such articulation profile.");
				return false;
			}
			using (new FMDB())
			{
				FMDB.Context.SignedLanguageArticulationProfiles.Remove(FMDB.Context.SignedLanguageArticulationProfiles.Find(existing.Id)!);
				FMDB.Context.SaveChanges();
			}
			_articulationProfiles.Remove(existing);
			actor.OutputHandler.Send($"You remove the {name.ColourName()} articulation profile.");
			return true;
		}

		var body = Gameworld.BodyPrototypes.GetByIdOrName(command.PopSpeech());
		var shape = Gameworld.BodypartShapes.GetByIdOrName(command.PopSpeech());
		var minimumText = command.PopSpeech();
		var preferredText = command.PopSpeech();
		if (action != "add" || name.Length == 0 || body is null || shape is null ||
		    !int.TryParse(minimumText, out var minimum) || !int.TryParse(preferredText, out var preferred) ||
		    minimum < 0 || preferred < minimum)
		{
			actor.OutputHandler.Send("Use: profile add <name> <body prototype> <bodypart shape> <minimum> <preferred>. Quote names containing spaces.");
			return false;
		}
		if (_articulationProfiles.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already an articulation profile with that name.");
			return false;
		}
		using (new FMDB())
		{
			var dbitem = new MudSharp.Models.SignedLanguageArticulationProfile
			{
				SignedLanguageId = Id, BodyPrototypeId = body.Id, Name = name
			};
			var requirement = new MudSharp.Models.SignedLanguageArticulationRequirement
			{
				ArticulationProfile = dbitem, BodypartShapeId = shape.Id, MinimumCount = minimum,
				PreferredCount = preferred
			};
			dbitem.Requirements.Add(requirement);
			FMDB.Context.SignedLanguageArticulationProfiles.Add(dbitem);
			FMDB.Context.SaveChanges();
			_articulationProfiles.Add(new SignedLanguageArticulationProfile(dbitem, this, Gameworld));
		}
		actor.OutputHandler.Send($"You add the {name.ColourName()} articulation profile.");
		return true;
	}

	public IProgVariable GetProperty(string property)
	{
		return property.ToLowerInvariant() switch
		{
			"id" => new NumberVariable(Id),
			"name" => new TextVariable(Name),
			"trait" => LinkedTrait,
			"unknown" => new TextVariable(UnknownLanguageDescription),
			"varieties" => new CollectionVariable(_varieties.ToList(), ProgVariableTypes.SignedLanguageVariety),
			_ => throw new NotSupportedException()
		};
	}

	public ProgVariableTypes Type => ProgVariableTypes.SignedLanguage;
	public object GetObject => this;

	public static void RegisterFutureProgCompiler()
	{
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.SignedLanguage,
			new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
			{
				["id"] = ProgVariableTypes.Number,
				["name"] = ProgVariableTypes.Text,
				["trait"] = ProgVariableTypes.Trait,
				["unknown"] = ProgVariableTypes.Text,
				["varieties"] = ProgVariableTypes.SignedLanguageVariety | ProgVariableTypes.Collection
			},
			new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
			{
				["id"] = "The ID of the signed language",
				["name"] = "The name of the signed language",
				["trait"] = "The skill trait linked to the signed language",
				["unknown"] = "The description shown when it is not recognised",
				["varieties"] = "The configured varieties of the signed language"
			});
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.SignedLanguages.Find(Id);
		if (dbitem is null)
		{
			return;
		}
		dbitem.Name = Name;
		dbitem.LinkedTraitId = LinkedTrait.Id;
		dbitem.DifficultyModelId = Model.Id;
		dbitem.UnknownLanguageDescription = UnknownLanguageDescription;
		dbitem.LanguageObfuscationFactor = LanguageObfuscationFactor;
		FMDB.Context.SignedLanguageMutualIntelligibilities.RemoveRange(
			FMDB.Context.SignedLanguageMutualIntelligibilities.Where(x => x.ListenerLanguageId == Id));
		foreach (var mutual in _mutualIntelligibilities)
		{
			FMDB.Context.SignedLanguageMutualIntelligibilities.Add(new MudSharp.Models.SignedLanguageMutualIntelligibility
			{
				ListenerLanguageId = Id,
				TargetLanguageId = mutual.Key,
				IntelligibilityDifficulty = (int)mutual.Value
			});
		}
		Changed = false;
	}
}

public class SignedLanguageVariety : FrameworkItem, ISignedLanguageVariety, IProgVariable
{
	public SignedLanguageVariety(MudSharp.Models.SignedLanguageVariety variety, ISignedLanguage language)
	{
		_id = variety.Id;
		_name = variety.Name;
		Language = language;
		Description = variety.Description;
		Suffix = variety.Suffix;
		VagueSuffix = variety.VagueSuffix;
		RecognitionDifficulty = (Difficulty)variety.RecognitionDifficulty;
	}

	public override string FrameworkItemType => "SignedLanguageVariety";
	public ISignedLanguage Language { get; }
	public string Description { get; }
	public string Suffix { get; }
	public string VagueSuffix { get; }
	public Difficulty RecognitionDifficulty { get; }
	public ProgVariableTypes Type => ProgVariableTypes.SignedLanguageVariety;
	public object GetObject => this;
	public IProgVariable GetProperty(string property) => property.ToLowerInvariant() switch
	{
		"id" => new NumberVariable(Id),
		"name" => new TextVariable(Name),
		"language" => Language,
		"description" => new TextVariable(Description),
		_ => throw new NotSupportedException()
	};
}

public class SignedLanguageArticulationProfile : FrameworkItem, ISignedLanguageArticulationProfile
{
	private readonly List<ISignedLanguageArticulationRequirement> _requirements = [];

	public SignedLanguageArticulationProfile(MudSharp.Models.SignedLanguageArticulationProfile profile,
		ISignedLanguage language, IFuturemud gameworld)
	{
		_id = profile.Id;
		_name = profile.Name;
		Language = language;
		BodyPrototype = gameworld.BodyPrototypes.Get(profile.BodyPrototypeId)!;
		_requirements.AddRange(profile.Requirements.Select(x => new SignedLanguageArticulationRequirement(x, gameworld)));
	}

	public override string FrameworkItemType => "SignedLanguageArticulationProfile";
	public ISignedLanguage Language { get; }
	public IBodyPrototype BodyPrototype { get; }
	public IEnumerable<ISignedLanguageArticulationRequirement> Requirements => _requirements;

	internal void SetRequirement(MudSharp.Models.SignedLanguageArticulationRequirement requirement,
		IFuturemud gameworld)
	{
		_requirements.RemoveAll(x => x.BodypartShape.Id == requirement.BodypartShapeId);
		_requirements.Add(new SignedLanguageArticulationRequirement(requirement, gameworld));
	}

	public SignedLanguageArticulationResult Evaluate(IBody body)
	{
		var missingPreferred = 0;
		foreach (var requirement in _requirements)
		{
			var count = body.Bodyparts.Count(x =>
				x.Shape == requirement.BodypartShape &&
				body.CanUseBodypart(x) == CanUseBodypartResult.CanUse);
			if (count < requirement.MinimumCount)
			{
				return SignedLanguageArticulationResult.Impossible(
					$"You need at least {requirement.MinimumCount.ToString("N0", body.Actor)} functional {requirement.BodypartShape.Name.Pluralise()} to use {Language.Name.TitleCase()}.");
			}
			missingPreferred += Math.Max(0, requirement.PreferredCount - count);
		}
		return SignedLanguageArticulationResult.Success(missingPreferred);
	}
}

public class SignedLanguageArticulationRequirement : ISignedLanguageArticulationRequirement
{
	public SignedLanguageArticulationRequirement(MudSharp.Models.SignedLanguageArticulationRequirement requirement,
		IFuturemud gameworld)
	{
		BodypartShape = gameworld.BodypartShapes.Get(requirement.BodypartShapeId)!;
		MinimumCount = requirement.MinimumCount;
		PreferredCount = requirement.PreferredCount;
	}

	public IBodypartShape BodypartShape { get; }
	public int MinimumCount { get; }
	public int PreferredCount { get; }
}

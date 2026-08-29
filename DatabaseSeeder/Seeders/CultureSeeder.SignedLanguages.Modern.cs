using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public partial class CultureSeeder
{
	private static readonly string[] ModernSignedLanguageNames =
	[
		"American Sign Language",
		"Quebec Sign Language",
		"Mexican Sign Language",
		"Brazilian Sign Language",
		"Argentine Sign Language",
		"Chilean Sign Language",
		"British Sign Language",
		"French Sign Language",
		"German Sign Language",
		"Spanish Sign Language",
		"Russian Sign Language",
		"South African Sign Language",
		"Kenyan Sign Language",
		"Nigerian Sign Language",
		"Ethiopian Sign Language",
		"Israeli Sign Language",
		"Jordanian Sign Language",
		"Indian Sign Language",
		"Chinese Sign Language",
		"Japanese Sign Language",
		"Filipino Sign Language",
		"Indonesian Sign Language",
		"Auslan",
		"New Zealand Sign Language"
	];

	private static readonly string[] BritishSignLanguageRegionalVarieties =
	[
		"Belfast", "Birmingham", "Bristol", "Cardiff", "Glasgow", "London", "Manchester", "Newcastle"
	];

	internal static IReadOnlyList<string> ModernSignedLanguageNamesForTesting => ModernSignedLanguageNames;
	internal static IReadOnlyList<string> BritishSignLanguageRegionalVarietiesForTesting => BritishSignLanguageRegionalVarieties;

	private void SeedModernSignedLanguages()
	{
		var difficultyModel = _context.LanguageDifficultyModels.First();
		var humanoidBody = _context.BodyProtos.First(x => x.Name == "Humanoid");
		var handShape = _context.BodypartShapes.First(x => x.Name == "hand");
		var languages = new Dictionary<string, SignedLanguage>(StringComparer.OrdinalIgnoreCase);

		foreach (var name in ModernSignedLanguageNames)
		{
			var trait = EnsureLanguageTrait(name, null);
			var language = _context.SignedLanguages.FirstOrDefault(x => x.Name == name);
			if (language is null)
			{
				language = new SignedLanguage { Name = name };
				_context.SignedLanguages.Add(language);
			}

			language.LinkedTrait = trait;
			language.DifficultyModel = difficultyModel;
			language.UnknownLanguageDescription = "an unfamiliar signed language";
			language.LanguageObfuscationFactor = 0.2;
			_context.SaveChanges();
			languages[name] = language;

			var profile = _context.SignedLanguageArticulationProfiles.FirstOrDefault(x =>
				x.SignedLanguageId == language.Id && x.BodyPrototypeId == humanoidBody.Id &&
				x.Name == "Humanoid Hands");
			if (profile is null)
			{
				profile = new SignedLanguageArticulationProfile
				{
					SignedLanguage = language,
					BodyPrototype = humanoidBody,
					Name = "Humanoid Hands"
				};
				_context.SignedLanguageArticulationProfiles.Add(profile);
				_context.SaveChanges();
			}

			_context.SignedLanguageArticulationRequirements.RemoveRange(
				_context.SignedLanguageArticulationRequirements.Where(x => x.ArticulationProfileId == profile.Id));
			_context.SignedLanguageArticulationRequirements.Add(new SignedLanguageArticulationRequirement
			{
				ArticulationProfile = profile,
				BodypartShape = handShape,
				MinimumCount = 1,
				PreferredCount = 2
			});
		}

		var british = languages["British Sign Language"];
		foreach (var varietyName in BritishSignLanguageRegionalVarieties)
		{
			var variety = _context.SignedLanguageVarieties.FirstOrDefault(x =>
				x.SignedLanguageId == british.Id && x.Name == varietyName);
			if (variety is null)
			{
				variety = new SignedLanguageVariety { SignedLanguage = british, Name = varietyName };
				_context.SignedLanguageVarieties.Add(variety);
			}
			variety.Description = $"The {varietyName} regional variety of British Sign Language.";
			variety.Suffix = $"with a {varietyName} regional variety";
			variety.VagueSuffix = "with a regional British variety";
			variety.RecognitionDifficulty = (int)Difficulty.Normal;
		}

		var banzsl = new[]
		{
			languages["British Sign Language"], languages["Auslan"], languages["New Zealand Sign Language"]
		};
		foreach (var listener in banzsl)
		{
			foreach (var target in banzsl.Where(x => x != listener))
			{
				var mutual = _context.SignedLanguageMutualIntelligibilities.FirstOrDefault(x =>
					x.ListenerLanguageId == listener.Id && x.TargetLanguageId == target.Id);
				if (mutual is null)
				{
					mutual = new SignedLanguageMutualIntelligibility
					{
						ListenerLanguage = listener,
						TargetLanguage = target
					};
					_context.SignedLanguageMutualIntelligibilities.Add(mutual);
				}
				mutual.IntelligibilityDifficulty = (int)Difficulty.Easy;
			}
		}

		_context.SaveChanges();
	}
}

#nullable enable

using MudSharp.Character.Name;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DatabaseSeeder.Seeders;

/// <summary>
/// Late-fifteenth-century companion expansion for the Earth-RenaissanceEurope culture pack.
/// </summary>
/// <remarks>
/// Adds player-facing ethnicities, independently adoptable cultures and historically structured naming
/// cultures across the Middle East, Asia and Africa, using approximately c. 1450 reference anchors.
/// Every naming culture supplies 50 masculine and 50 feminine personal names. Toponymic, nisba and
/// professional-affiliation inventories that feature prominently contain at least 200 elements.
///
/// The ethnicity and culture records are intentionally independent. The seeder does not bind an
/// ethnicity to the culture in the same row, so a character may combine ancestry with an adopted
/// court, urban, regional or political culture.
///
/// Integration point: call from the Earth-RenaissanceWorldExpansion culture pack in <c>CultureSeederPacks.cs</c>:
/// <code>SeedRenaissanceWorldExpansion(questionAnswers);</code>
/// </remarks>
public partial class CultureSeeder
{
	private enum RenaissanceWorldNameForm
	{
		GivenOnly,
		GivenFamily,
		FamilyGiven,
		GivenPatronym,
		GivenToponym,
		GivenClan,
		GivenPatronymToponym,
		GivenPatronymClan
	}

	private sealed record RenaissanceWorldSeed(
		string Key,
		string NameCultureName,
		string EthnicityName,
		string EthnicGroup,
		string EthnicSubgroup,
		string CultureName,
		RenaissanceWorldNameForm NameForm,
		string NameRegex,
		string ReferenceAnchor,
		string EthnicityDescription,
		string CultureDescription,
		string PersonalNameDescription,
		string SecondNameDescription,
		string ThirdNameDescription,
		int MinimumSecondNameCount,
		int MinimumThirdNameCount,
		string BloodModel,
		string SkinProfile,
		string HairProfile,
		string EyeProfile,
		string[] MaleGivenNames,
		string[] FemaleGivenNames,
		string[] MaleSecondNames,
		string[] FemaleSecondNames,
		string[] ThirdNames);

	private void SeedRenaissanceWorldExpansion(IReadOnlyDictionary<string, string> questionAnswers)
	{
		if (questionAnswers["seednames"].EqualToAny("y", "yes"))
		{
			SeedRenaissanceWorldExpansionNames();
		}

		if (questionAnswers.TryGetValue("seedlanguages", out string? seedLanguages) &&
			seedLanguages.EqualToAny("y", "yes"))
		{
			SeedRenaissanceWorldExpansionLanguages();
		}

		if (questionAnswers["seedheritage"].EqualToAny("y", "yes"))
		{
			SeedRenaissanceWorldExpansionHeritage();
		}
	}

	private void SeedRenaissanceWorldExpansionNames()
	{
		foreach (RenaissanceWorldSeed seed in RenaissanceWorldSeeds)
		{
			ValidateRenaissanceWorldSeed(seed);
			NameCulture nameCulture = CreateRenaissanceWorldNameCulture(seed);
			SeedRenaissanceWorldProfile(seed, nameCulture, Gender.Male, seed.MaleGivenNames, seed.MaleSecondNames);
			SeedRenaissanceWorldProfile(seed, nameCulture, Gender.Female, seed.FemaleGivenNames, seed.FemaleSecondNames);
		}

		_context.SaveChanges();
	}

	private NameCulture CreateRenaissanceWorldNameCulture(RenaissanceWorldSeed seed)
	{
		return AddNameCulture(
			seed.NameCultureName,
			seed.NameRegex,
			GetRenaissanceWorldNameElements(seed),
			GetRenaissanceWorldNamePatterns(seed.NameForm));
	}

	private void SeedRenaissanceWorldProfile(
		RenaissanceWorldSeed seed,
		NameCulture nameCulture,
		Gender gender,
		IEnumerable<string> givenNames,
		IEnumerable<string> secondNames)
	{
		RandomNameProfile profile = AddRandomNameProfile($"{seed.NameCultureName} {gender.DescribeEnum()}", gender, nameCulture);
		AddRandomNameDice(profile, NameUsage.BirthName, "1");
		foreach (string name in givenNames)
		{
			AddRandomNameElement(profile, NameUsage.BirthName, name, 100);
		}

		NameUsage? secondUsage = GetRenaissanceWorldSecondUsage(seed.NameForm);
		if (secondUsage.HasValue)
		{
			AddRandomNameDice(profile, secondUsage.Value, "1");
			foreach (string name in secondNames)
			{
				AddRandomNameElement(profile, secondUsage.Value, name, 100);
			}
		}

		NameUsage? thirdUsage = GetRenaissanceWorldThirdUsage(seed.NameForm);
		if (thirdUsage.HasValue)
		{
			AddRandomNameDice(profile, thirdUsage.Value, "1");
			foreach (string name in seed.ThirdNames)
			{
				AddRandomNameElement(profile, thirdUsage.Value, name, 100);
			}
		}

		_context.SaveChanges();
	}

	private void SeedRenaissanceWorldExpansionHeritage()
	{
		foreach (RenaissanceWorldSeed seed in RenaissanceWorldSeeds)
		{
			ValidateRenaissanceWorldSeed(seed);
			AddEthnicity(
				_humanRace,
				seed.EthnicityName,
				seed.EthnicGroup,
				seed.BloodModel,
				0.0,
				0.0,
				seed.EthnicSubgroup,
				description: seed.EthnicityDescription);

			ApplyRenaissanceWorldCharacteristicDefaults(seed);

			string nameCulture = ResolveRenaissanceWorldNameCulture(seed);
			AddCulture(
				seed.CultureName,
				nameCulture,
				nameCulture,
				seed.CultureDescription);

			Ethnicity ethnicity = _ethnicities[seed.EthnicityName];
			ReplaceEthnicityNameLinks(
				ethnicity,
				(Gender.Male, nameCulture),
				(Gender.Female, nameCulture),
				(Gender.Neuter, nameCulture),
				(Gender.NonBinary, nameCulture),
				(Gender.Indeterminate, nameCulture));
		}

		SeedRenaissanceWorldExpansionHeritageExpansion();
		_context.SaveChanges();
	}

	private string ResolveRenaissanceWorldNameCulture(RenaissanceWorldSeed seed)
	{
		if (ContainsNameCulture(_context.NameCultures.AsEnumerable(), seed.NameCultureName))
		{
			return seed.NameCultureName;
		}

		return seed.NameForm switch
		{
			RenaissanceWorldNameForm.GivenOnly => "Simple",
			RenaissanceWorldNameForm.GivenPatronym => "Given and Patronym",
			RenaissanceWorldNameForm.GivenPatronymToponym => "Given and Patronym",
			RenaissanceWorldNameForm.GivenPatronymClan => "Given and Patronym",
			RenaissanceWorldNameForm.GivenToponym => "Given and Toponym",
			_ => "Given and Family"
		};
	}

	internal static bool ContainsNameCultureForTesting(IEnumerable<NameCulture> nameCultures, string name)
	{
		return ContainsNameCulture(nameCultures, name);
	}

	private static bool ContainsNameCulture(IEnumerable<NameCulture> nameCultures, string name)
	{
		return nameCultures.Any(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
	}

	private void ApplyRenaissanceWorldCharacteristicDefaults(RenaissanceWorldSeed seed)
	{
		AddRenaissanceWorldCharacteristicIfAvailable(seed.EthnicityName, "Distinctive Feature", "All Distinctive Features");
		AddRenaissanceWorldCharacteristicIfAvailable(seed.EthnicityName, "Eye Shape", "All Eye Shapes");
		AddRenaissanceWorldCharacteristicIfAvailable(seed.EthnicityName, "Hair Style", "All Hair Styles");
		AddRenaissanceWorldCharacteristicIfAvailable(seed.EthnicityName, "Facial Hair Style", "All Facial Hair Styles");
		AddRenaissanceWorldCharacteristicIfAvailable(seed.EthnicityName, "Humanoid Frame", "All Frames");
		AddRenaissanceWorldCharacteristicIfAvailable(seed.EthnicityName, "Nose", "All Noses");
		AddRenaissanceWorldCharacteristicIfAvailable(seed.EthnicityName, "Person Word", "Weighted Person Words");
		AddRenaissanceWorldCharacteristicIfAvailable(seed.EthnicityName, "Hair Colour", seed.HairProfile);
		AddRenaissanceWorldCharacteristicIfAvailable(seed.EthnicityName, "Facial Hair Colour", seed.HairProfile);
		AddRenaissanceWorldCharacteristicIfAvailable(seed.EthnicityName, "Eye Colour", seed.EyeProfile);
		AddRenaissanceWorldCharacteristicIfAvailable(seed.EthnicityName, "Skin Colour", seed.SkinProfile);
	}

	private void AddRenaissanceWorldCharacteristicIfAvailable(string ethnicity, string definition, string profile)
	{
		if (definition.Equals("Humanoid Frame", StringComparison.OrdinalIgnoreCase) &&
			!_definitions.ContainsKey(definition) &&
			_definitions.ContainsKey("Frame"))
		{
			definition = "Frame";
		}


		if (!_definitions.ContainsKey(definition) || !_profiles.ContainsKey(profile))
		{
			return;
		}

		AddEthnicityVariable(ethnicity, definition, profile);
	}

	private static void ValidateRenaissanceWorldSeed(RenaissanceWorldSeed seed)
	{
		if (string.IsNullOrWhiteSpace(seed.ReferenceAnchor) ||
			string.IsNullOrWhiteSpace(seed.EthnicityDescription) ||
			string.IsNullOrWhiteSpace(seed.CultureDescription) ||
			string.IsNullOrWhiteSpace(seed.PersonalNameDescription))
		{
			throw new InvalidOperationException($"{seed.Key} is missing required reference or player-facing prose.");
		}

		ValidateRenaissanceWorldGivenNames(seed.Key, "male", seed.MaleGivenNames);
		ValidateRenaissanceWorldGivenNames(seed.Key, "female", seed.FemaleGivenNames);

		NameUsage? secondUsage = GetRenaissanceWorldSecondUsage(seed.NameForm);
		if (secondUsage.HasValue)
		{
			if (string.IsNullOrWhiteSpace(seed.SecondNameDescription))
			{
				throw new InvalidOperationException($"{seed.Key} requires player guidance for its second name element.");
			}
			ValidateRenaissanceWorldSupportingNames(
				seed.Key,
				secondUsage.Value.DescribeEnum(),
				seed.MaleSecondNames,
				seed.MinimumSecondNameCount);
			ValidateRenaissanceWorldSupportingNames(
				seed.Key,
				secondUsage.Value.DescribeEnum(),
				seed.FemaleSecondNames,
				seed.MinimumSecondNameCount);
		}

		NameUsage? thirdUsage = GetRenaissanceWorldThirdUsage(seed.NameForm);
		if (thirdUsage.HasValue)
		{
			if (string.IsNullOrWhiteSpace(seed.ThirdNameDescription))
			{
				throw new InvalidOperationException($"{seed.Key} requires player guidance for its third name element.");
			}
			ValidateRenaissanceWorldSupportingNames(
				seed.Key,
				thirdUsage.Value.DescribeEnum(),
				seed.ThirdNames,
				seed.MinimumThirdNameCount);
		}

		ValidateRenaissanceWorldRegex(seed);
	}

	private static void ValidateRenaissanceWorldGivenNames(string culture, string gender, IReadOnlyCollection<string> names)
	{
		if (names.Count is < 50 or > 100)
		{
			throw new InvalidOperationException(
				$"{culture} must define between 50 and 100 {gender} personal names; found {names.Count}.");
		}

		ValidateRenaissanceWorldUniqueSingleTokenNames(culture, $"female personal", names);
	}

	private static void ValidateRenaissanceWorldSupportingNames(
		string culture,
		string usage,
		IReadOnlyCollection<string> names,
		int minimumCount)
	{
		if (names.Count < minimumCount)
		{
			throw new InvalidOperationException(
				$"{culture} requires at least {minimumCount} {usage} elements; found {names.Count}.");
		}

		ValidateRenaissanceWorldUniqueSingleTokenNames(culture, usage, names);
	}

	private static void ValidateRenaissanceWorldUniqueSingleTokenNames(
		string culture,
		string usage,
		IReadOnlyCollection<string> names)
	{
		if (names.Any(string.IsNullOrWhiteSpace) || names.Any(x => x.Any(char.IsWhiteSpace)))
		{
			throw new InvalidOperationException(
				$"{culture} contains a blank or multi-token {usage} element. Use hyphens for compounds.");
		}

		if (names.Distinct(StringComparer.OrdinalIgnoreCase).Count() != names.Count)
		{
			throw new InvalidOperationException($"{culture} contains duplicate {usage} elements.");
		}
	}

	private static void ValidateRenaissanceWorldRegex(RenaissanceWorldSeed seed)
	{
		Regex regex;
		try
		{
			regex = new Regex(seed.NameRegex, RegexOptions.CultureInvariant);
		}
		catch (ArgumentException ex)
		{
			throw new InvalidOperationException($"{seed.Key} has an invalid name-entry regular expression.", ex);
		}

		string? maleSecond = seed.MaleSecondNames.FirstOrDefault();
		string? femaleSecond = seed.FemaleSecondNames.FirstOrDefault();
		string? third = seed.ThirdNames.FirstOrDefault();

		foreach (string givenName in seed.MaleGivenNames)
		{
			ValidateRenaissanceWorldGeneratedName(regex, seed, givenName, maleSecond, third);
		}

		foreach (string givenName in seed.FemaleGivenNames)
		{
			ValidateRenaissanceWorldGeneratedName(regex, seed, givenName, femaleSecond, third);
		}

		foreach (string secondName in seed.MaleSecondNames)
		{
			ValidateRenaissanceWorldGeneratedName(regex, seed, seed.MaleGivenNames[0], secondName, third);
		}

		foreach (string secondName in seed.FemaleSecondNames)
		{
			ValidateRenaissanceWorldGeneratedName(regex, seed, seed.FemaleGivenNames[0], secondName, third);
		}

		foreach (string thirdName in seed.ThirdNames)
		{
			ValidateRenaissanceWorldGeneratedName(regex, seed, seed.MaleGivenNames[0], maleSecond, thirdName);
		}
	}

	private static void ValidateRenaissanceWorldGeneratedName(
		Regex regex,
		RenaissanceWorldSeed seed,
		string givenName,
		string? secondName,
		string? thirdName)
	{
		string fullName = ComposeRenaissanceWorldFullName(seed.NameForm, givenName, secondName, thirdName);
		if (!regex.IsMatch(fullName))
		{
			throw new InvalidOperationException(
				$"{seed.Key} generated name '{fullName}' does not satisfy regex '{seed.NameRegex}'.");
		}
	}

	private static string ComposeRenaissanceWorldFullName(
		RenaissanceWorldNameForm form,
		string givenName,
		string? secondName,
		string? thirdName)
	{
		return form switch
		{
			RenaissanceWorldNameForm.GivenOnly => givenName,
			RenaissanceWorldNameForm.GivenFamily => $"{givenName} {secondName}",
			RenaissanceWorldNameForm.FamilyGiven => $"{secondName} {givenName}",
			RenaissanceWorldNameForm.GivenPatronym => $"{givenName} {secondName}",
			RenaissanceWorldNameForm.GivenToponym => $"{givenName} {secondName}",
			RenaissanceWorldNameForm.GivenClan => $"{givenName} {secondName}",
			RenaissanceWorldNameForm.GivenPatronymToponym => $"{givenName} {secondName} {thirdName}",
			RenaissanceWorldNameForm.GivenPatronymClan => $"{givenName} {secondName} {thirdName}",
			_ => throw new ArgumentOutOfRangeException(nameof(form), form, null)
		};
	}

	private static string CompleteRenaissanceWorldPlayerSentence(string text)
	{
		string value = text.Trim();
		if (value.Length == 0)
		{
			return value;
		}

		return value.EndsWith(".", StringComparison.Ordinal) ? value : $"{value}.";
	}

	private static string FormatRenaissanceWorldPlayerExamples(IEnumerable<string> names, int maximum = 8)
	{
		return string.Join(", ", names.Distinct(StringComparer.OrdinalIgnoreCase).Take(maximum));
	}

	private static string BuildRenaissanceWorldPersonalDescription(RenaissanceWorldSeed seed)
	{
		return
			$"{CompleteRenaissanceWorldPlayerSentence(seed.PersonalNameDescription)} " +
			$"Common masculine examples include {FormatRenaissanceWorldPlayerExamples(seed.MaleGivenNames, 6)}; " +
			$"common feminine examples include {FormatRenaissanceWorldPlayerExamples(seed.FemaleGivenNames, 6)}.";
	}

	private static string BuildRenaissanceWorldSecondDescription(RenaissanceWorldSeed seed)
	{
		string guidance = CompleteRenaissanceWorldPlayerSentence(seed.SecondNameDescription);
		if (seed.MaleSecondNames.SequenceEqual(seed.FemaleSecondNames, StringComparer.OrdinalIgnoreCase))
		{
			return $"{guidance} Examples include {FormatRenaissanceWorldPlayerExamples(seed.MaleSecondNames)}.";
		}

		return
			$"{guidance} Masculine examples include {FormatRenaissanceWorldPlayerExamples(seed.MaleSecondNames, 6)}; " +
			$"feminine examples include {FormatRenaissanceWorldPlayerExamples(seed.FemaleSecondNames, 6)}.";
	}

	private static string BuildRenaissanceWorldThirdDescription(RenaissanceWorldSeed seed)
	{
		return
			$"{CompleteRenaissanceWorldPlayerSentence(seed.ThirdNameDescription)} " +
			$"Examples include {FormatRenaissanceWorldPlayerExamples(seed.ThirdNames)}.";
	}

	private static (string Name, int Minimum, int Maximum, string Description, NameUsage Usage)[]
		GetRenaissanceWorldNameElements(RenaissanceWorldSeed seed)
	{
		(string Name, int Minimum, int Maximum, string Description, NameUsage Usage) personal =
			("Personal Name", 1, 1, BuildRenaissanceWorldPersonalDescription(seed), NameUsage.BirthName);

		NameUsage? secondUsage = GetRenaissanceWorldSecondUsage(seed.NameForm);
		NameUsage? thirdUsage = GetRenaissanceWorldThirdUsage(seed.NameForm);
		var elements = new List<(string Name, int Minimum, int Maximum, string Description, NameUsage Usage)>
		{
			personal
		};

		if (secondUsage.HasValue)
		{
			elements.Add((
				GetRenaissanceWorldElementLabel(secondUsage.Value),
				1,
				1,
				BuildRenaissanceWorldSecondDescription(seed),
				secondUsage.Value));
		}

		if (thirdUsage.HasValue)
		{
			elements.Add((
				GetRenaissanceWorldElementLabel(thirdUsage.Value),
				1,
				1,
				BuildRenaissanceWorldThirdDescription(seed),
				thirdUsage.Value));
		}

		return seed.NameForm switch
		{
			RenaissanceWorldNameForm.FamilyGiven => elements.OrderByDescending(x => x.Usage == NameUsage.Surname).ToArray(),
			_ => elements.ToArray()
		};
	}

	private static string GetRenaissanceWorldElementLabel(NameUsage usage)
	{
		return usage switch
		{
			NameUsage.Surname => "Family or Affiliation Name",
			NameUsage.Patronym => "Patronym or Parentage Name",
			NameUsage.Toponym => "Toponym, Nisba or Affiliation",
			NameUsage.FamilyGroupName => "Clan, House or Corporate Affiliation",
			_ => usage.DescribeEnum()
		};
	}

	private static (NameStyle Style, string Pattern, NameUsage[] Parameters)[]
		GetRenaissanceWorldNamePatterns(RenaissanceWorldNameForm form)
	{
		return form switch
		{
			RenaissanceWorldNameForm.GivenOnly => RenaissanceWorldSingleElementPatterns(NameUsage.BirthName),
			RenaissanceWorldNameForm.GivenFamily => RenaissanceWorldTwoElementPatterns(NameUsage.BirthName, NameUsage.Surname, familyFirst: false),
			RenaissanceWorldNameForm.FamilyGiven => RenaissanceWorldTwoElementPatterns(NameUsage.BirthName, NameUsage.Surname, familyFirst: true),
			RenaissanceWorldNameForm.GivenPatronym => RenaissanceWorldTwoElementPatterns(NameUsage.BirthName, NameUsage.Patronym, familyFirst: false),
			RenaissanceWorldNameForm.GivenToponym => RenaissanceWorldTwoElementPatterns(NameUsage.BirthName, NameUsage.Toponym, familyFirst: false),
			RenaissanceWorldNameForm.GivenClan => RenaissanceWorldTwoElementPatterns(NameUsage.BirthName, NameUsage.FamilyGroupName, familyFirst: false),
			RenaissanceWorldNameForm.GivenPatronymToponym =>
				RenaissanceWorldThreeElementPatterns(NameUsage.BirthName, NameUsage.Patronym, NameUsage.Toponym),
			RenaissanceWorldNameForm.GivenPatronymClan =>
				RenaissanceWorldThreeElementPatterns(NameUsage.BirthName, NameUsage.Patronym, NameUsage.FamilyGroupName),
			_ => throw new ArgumentOutOfRangeException(nameof(form), form, null)
		};
	}

	private static (NameStyle Style, string Pattern, NameUsage[] Parameters)[] RenaissanceWorldSingleElementPatterns(NameUsage usage)
	{
		return new[]
		{
			(NameStyle.GivenOnly, "{0}", new[] { usage }),
			(NameStyle.SimpleFull, "{0}", new[] { usage }),
			(NameStyle.FullName, "{0}", new[] { usage }),
			(NameStyle.Affectionate, "{0}", new[] { usage }),
			(NameStyle.SurnameOnly, "{0}", new[] { usage }),
			(NameStyle.FullWithNickname, "{0}", new[] { usage })
		};
	}

	private static (NameStyle Style, string Pattern, NameUsage[] Parameters)[] RenaissanceWorldTwoElementPatterns(
		NameUsage personal,
		NameUsage second,
		bool familyFirst)
	{
		NameUsage[] fullParameters = familyFirst ? new[] { second, personal } : new[] { personal, second };
		return new[]
		{
			(NameStyle.GivenOnly, "{0}", new[] { personal }),
			(NameStyle.SimpleFull, "{0} {1}", fullParameters),
			(NameStyle.FullName, "{0} {1}", fullParameters),
			(NameStyle.Affectionate, "{0}", new[] { personal }),
			(NameStyle.SurnameOnly, "{0}", new[] { second }),
			(NameStyle.FullWithNickname, "{0} {1}", fullParameters)
		};
	}

	private static (NameStyle Style, string Pattern, NameUsage[] Parameters)[] RenaissanceWorldThreeElementPatterns(
		NameUsage personal,
		NameUsage second,
		NameUsage third)
	{
		NameUsage[] fullParameters = new[] { personal, second, third };
		return new[]
		{
			(NameStyle.GivenOnly, "{0}", new[] { personal }),
			(NameStyle.SimpleFull, "{0} {1} {2}", fullParameters),
			(NameStyle.FullName, "{0} {1} {2}", fullParameters),
			(NameStyle.Affectionate, "{0}", new[] { personal }),
			(NameStyle.SurnameOnly, "{0}", new[] { third }),
			(NameStyle.FullWithNickname, "{0} {1} {2}", fullParameters)
		};
	}

	private static NameUsage? GetRenaissanceWorldSecondUsage(RenaissanceWorldNameForm form)
	{
		return form switch
		{
			RenaissanceWorldNameForm.GivenFamily => NameUsage.Surname,
			RenaissanceWorldNameForm.FamilyGiven => NameUsage.Surname,
			RenaissanceWorldNameForm.GivenPatronym => NameUsage.Patronym,
			RenaissanceWorldNameForm.GivenToponym => NameUsage.Toponym,
			RenaissanceWorldNameForm.GivenClan => NameUsage.FamilyGroupName,
			RenaissanceWorldNameForm.GivenPatronymToponym => NameUsage.Patronym,
			RenaissanceWorldNameForm.GivenPatronymClan => NameUsage.Patronym,
			_ => null
		};
	}

	private static NameUsage? GetRenaissanceWorldThirdUsage(RenaissanceWorldNameForm form)
	{
		return form switch
		{
			RenaissanceWorldNameForm.GivenPatronymToponym => NameUsage.Toponym,
			RenaissanceWorldNameForm.GivenPatronymClan => NameUsage.FamilyGroupName,
			_ => null
		};
	}

	private static readonly string[] RenaissanceKurdishNisbaInventory =
	new[]
	{
		"al-Bidlisi", "al-Hakkari", "al-Ardabili", "al-Dinawari", "al-Shahrazuri", "al-Mardini",
		"al-Jaziri", "al-Mushi", "al-Amedi", "al-Sorani", "al-Bahdinani", "al-Rawandizi",
		"al-Erbili", "al-Sulaymani", "al-Kirkuki", "al-Mosuli", "al-Sinajari", "al-Zakhoyi",
		"al-Duhoki", "al-Aqrai", "al-Shaqlawi", "al-Koysinjaqi", "al-Halabjai", "al-Penjwini",
		"al-Raniyai", "al-Qaladzai", "al-Chamchamali", "al-Kifri", "al-Khanaqini", "al-Mandali",
		"al-Sanandaji", "al-Mahabadi", "al-Mukriyani", "al-Saqizi", "al-Banai", "al-Marivani",
		"al-Piranshahri", "al-Ushnavi", "al-Sardashti", "al-Bukanai", "al-Salmasi", "al-Khuyi",
		"al-Urmavi", "al-Miandabi", "al-Naghadai", "al-Razhani", "al-Kermanshahi", "al-Pavehi",
		"al-Javanrudi", "al-Ravansari", "al-Sahneh-i", "al-Harsini", "al-Kangavari", "al-Ilam-i",
		"al-Dehlorani", "al-Abdanani", "al-Darreshahri", "al-Eyvan-i", "al-Luristani", "al-Khorramabadi",
		"al-Alemdari", "al-Kharputi", "al-Dersimi", "al-Erzincani", "al-Erzurumi", "al-Vani",
		"al-Malazgirti", "al-Akhlati", "al-Mus-i", "al-Siirti", "al-Diyarbakri", "al-Mayyafariqini",
		"al-Silvani", "al-Batmani", "al-Hasankeyfi", "al-Ciziri", "al-Nusaybini", "al-Urfi",
		"al-Ruhawi", "al-Sivereki", "al-Bingoli", "al-Kulp-i", "al-Lice-i", "al-Palu-i",
		"al-Egil-i", "al-Hazro-i", "al-Sasuni", "al-Midyati", "al-Nisibini", "al-Zawzani",
		"al-Botan-i", "al-Garmiyani", "al-Hawrami", "al-Ardalani", "al-Babani", "al-Milli",
		"al-Rishwani", "al-Mahmudi", "al-Mukri", "al-Zaza-i", "al-Barzani", "al-Jafi",
		"al-Colemergi", "al-Soran-i", "al-Shirwani", "al-Hewramani", "al-Ardalan-i", "al-Mukri-i",
		"al-Milli-i", "al-Geveri", "al-Mahmudi-i", "al-Hakkari-i", "al-Herki-i", "al-Bradosti-i",
		"al-Doski-i", "al-Zibari-i", "al-Surchi-i", "al-Hamawand-i", "al-Jaf-i", "al-Talabani",
		"al-Mangur-i", "al-Mamash-i", "al-Piran-i", "al-Shikak-i", "al-Bilbas-i", "al-Milan-i",
		"al-Reshwan-i", "al-Berazi-i", "al-Modan-i", "al-Zirkan-i", "al-Hesenan-i", "al-Haydaran-i",
		"al-Jalali-i", "al-Sipkan-i", "al-Lolan-i", "al-Motikan-i", "al-Atmanekan-i", "al-Pinyanishi-i",
		"al-Rekani-i", "al-Goyan-i", "al-Sindi-i", "al-Slivani-i", "al-Hartushi-i", "al-Gewdan-i",
		"al-Khaltan-i", "al-Mirdasi-i", "al-Buhti-i", "al-Rozhiki-i", "al-Dunbuli-i", "al-Qadiri",
		"al-Naqshbandi", "al-Suhrawardi", "al-Rifai", "al-Yasawi", "al-Sufi", "al-Faqih",
		"al-Mulla", "al-Qadi", "al-Khatib", "al-Katib", "al-Warraq", "al-Attar",
		"al-Haddad", "al-Najjar", "al-Khayyat", "al-Dabbagh", "al-Sarraj", "al-Bazzaz",
		"al-Sayrafi", "al-Zangana", "al-Shaddadi", "al-Marwani", "al-Ayyubi", "al-Hasanwayhi",
		"al-Annaz-i", "al-Hadhbani", "al-Rawadi", "al-Salari", "al-Shabankarai", "al-Mihrani",
		"al-Luri", "al-Fayli", "al-Gurani", "al-Kurmanji", "al-Zazaki", "al-Gilani",
		"al-Daylami", "al-Turkumani", "al-Armani", "al-Rumi", "al-Farisi", "al-Arabi",
		"al-Mawsili", "al-Baghdadi", "al-Dimashqi", "al-Halabi", "al-Tabrizi", "al-Qazwini",
		"al-Hamadani", "al-Khurasani"
	};

	private static readonly string[] RenaissanceMamlukNisbaInventory =
	new[]
	{
		"al-Qahiri", "al-Misri", "al-Iskandari", "al-Fayyumi", "al-Saidi", "al-Dimyati",
		"al-Tinnisi", "al-Manfaluti", "al-Suyuti", "al-Aswani", "al-Qusi", "al-Akhmimi",
		"al-Usyuti", "al-Bahnasi", "al-Bahnasawi", "al-Buhairi", "al-Jizawi", "al-Minufi",
		"al-Qalyubi", "al-Fustati", "al-Mahallawi", "al-Simannudi", "al-Daqahli", "al-Bilbaysi",
		"al-Gharbi", "al-Sharqi", "al-Nili", "al-Rashidi", "al-Burullusi", "al-Faraskuri",
		"al-Dimashqi", "al-Halabi", "al-Hamawi", "al-Himsi", "al-Tarabulusi", "al-Beiruti",
		"al-Saydawi", "al-Suri", "al-Akkawi", "al-Qudsi", "al-Ramli", "al-Ghazzi",
		"al-Asqalani", "al-Nabulsi", "al-Tiberi", "al-Safadi", "al-Baalbakki", "al-Hawrani",
		"al-Ajluni", "al-Karaki", "al-Ma-ani", "al-Busrawi", "al-Balqawi", "al-Mawsili",
		"al-Jaziri", "al-Harrani", "al-Raqqi", "al-Amidi", "al-Mardini", "al-Ruhawi",
		"al-Malati", "al-Ayntabi", "al-Antaki", "al-Tarusi", "al-Adhani", "al-Marashi",
		"al-Hijazi", "al-Makki", "al-Madani", "al-Taifi", "al-Yamani", "al-Sanani",
		"al-Adeni", "al-Zabidi", "al-Hadrami", "al-Taizzi", "al-Jizani", "al-Ghuzzi",
		"al-Lajuni", "al-Nasiri", "al-Haddad", "al-Najjar", "al-Khayyat", "al-Dabbagh",
		"al-Attar", "al-Bazzaz", "al-Sarraj", "al-Warraq", "al-Katib", "al-Qadi",
		"al-Muallim", "al-Tabib", "al-Kahhal", "al-Sayrafi", "al-Sabbagh", "al-Qassab",
		"al-Khabbaz", "al-Tahhan", "al-Fakhkhar", "al-Zajjaj", "al-Nahhas", "al-Saffar",
		"al-Hariri", "al-Qattan", "al-Labbad", "al-Hallaj", "al-Saqqaf", "al-Banna",
		"al-Haffar", "al-Jawhari", "al-Habbal", "al-Bawwab", "al-Hammami", "al-Farran",
		"al-Mallah", "al-Bahhar", "al-Jammal", "al-Rakkab", "al-Baytar", "al-Saati",
		"al-Qurashi", "al-Hashimi", "al-Ansari", "al-Kinani", "al-Tamimi", "al-Thaqafi",
		"al-Azdi", "al-Lakhmi", "al-Judhami", "al-Kalbi", "al-Tanukhi", "al-Hamdani",
		"al-Kindi", "al-Makhzumi", "al-Umari", "al-Bakri", "al-Husayni", "al-Hasani",
		"al-Ayyubi", "al-Abbasi", "al-Alawi", "al-Jafari", "al-Usmani", "al-Sulami",
		"al-Hilali", "al-Sulaymi", "al-Khazraji", "al-Awsi", "al-Qaysi", "al-Muradi",
		"al-Maafiri", "al-Ghassani", "al-Nasri", "al-Kilabi", "al-Uqayli", "al-Juhani",
		"al-Harbi", "al-Hudhali", "al-Damri", "al-Fihri", "al-Shafii", "al-Hanafi",
		"al-Maliki", "al-Hanbali", "al-Ashari", "al-Maturidi", "al-Sufi", "al-Qadiri",
		"al-Rifai", "al-Shadhili", "al-Badawi", "al-Dasuqi", "al-Ahmadi", "al-Khalwati",
		"al-Mawlawi", "al-Zahiri", "al-Salihi", "al-Ashrafi", "al-Zayni", "al-Nuri",
		"al-Shamsi", "al-Badri", "al-Fakhri", "al-Jamali", "al-Kamali", "al-Saifi",
		"al-Izzati", "al-Majdi", "al-Taqawi", "al-Burji", "al-Bahri", "al-Mansuri",
		"al-Muayyadi", "al-Inali", "al-Jaqmaqi", "al-Qaytbai", "al-Dawadari", "al-Ustadar",
		"al-Kashifi", "al-Dawlatshahi"
	};

	private static readonly string[] RenaissanceDelhiAffiliationInventory =
	new[]
	{
		"al-Dihlawi", "al-Lahori", "al-Multani", "al-Badauni", "al-Awadhi", "al-Kannauji",
		"al-Panipati", "al-Sirhindi", "al-Hansi", "al-Ajodhani", "al-Thanesari", "al-Nagauri",
		"al-Ajmeri", "al-Bayanei", "al-Kolwi", "al-Sambhali", "al-Amrohi", "al-Bijnori",
		"al-Moradabadi", "al-Bareli", "al-Budauni", "al-Etawi", "al-Mainpuri", "al-Koili",
		"al-Mathurawi", "al-Agrawi", "al-Fathpuri", "al-Gwaliari", "al-Chanderi", "al-Kalinjari",
		"al-Jaunpuri", "al-Banarasi", "al-Ghazipuri", "al-Lakhnawi", "al-Faizabadi", "al-Gorakhpuri",
		"al-Bahraichi", "al-Ayodhyawi", "al-Karawi", "al-Allahabadi", "al-Kanpuri", "al-Farrukhabadi",
		"al-Rohilkhandi", "al-Mewati", "al-Alwari", "al-Bharatpuri", "al-Dholpuri", "al-Ranthambhori",
		"al-Marwari", "al-Jodhpuri", "al-Bikaneri", "al-Jaisalmeri", "al-Chittori", "al-Mewari",
		"al-Bundi", "al-Kotawi", "al-Malwi", "al-Mandui", "al-Ujjayni", "al-Dhari",
		"al-Indori", "al-Burhanpuri", "al-Khandeshi", "al-Daulatabadi", "al-Aurangabadi", "al-Bidari",
		"al-Gulbargawi", "al-Bijapuri", "al-Hyderabadi", "al-Warangali", "al-Kakati", "al-Raichuri",
		"al-Vijayanagari", "al-Gujari", "al-Ahmadabadi", "al-Khambati", "al-Bharuchi", "al-Surati",
		"al-Patani", "al-Somnathi", "al-Junagadhi", "al-Kachchi", "al-Sindhi", "al-Thatta-i",
		"al-Uchchi", "al-Dipalapuri", "al-Peshawari", "al-Kabuli", "al-Ghaznawi", "al-Kashmiri",
		"al-Srinagari", "al-Jammuwi", "al-Sialkoti", "al-Jalandhari", "al-Ludhianawi", "al-Patiali",
		"al-Baylawi", "al-Hisari", "al-Qadi", "al-Mufti", "al-Faqih", "al-Mudarris",
		"al-Muallim", "al-Katib", "al-Munshi", "al-Dabir", "al-Warraq", "al-Khattat",
		"al-Shair", "al-Adib", "al-Hafiz", "al-Qari", "al-Muqri", "al-Muhaddith",
		"al-Sufi", "al-Chishti", "al-Suhrawardi", "al-Qadiri", "al-Faridi", "al-Sabiri",
		"al-Nizami", "al-Attar", "al-Hakim", "al-Tabib", "al-Kahhal", "al-Sayrafi",
		"al-Bazzaz", "al-Khayyat", "al-Haddad", "al-Najjar", "al-Dabbagh", "al-Sarraj",
		"al-Zargar", "al-Jauhari", "al-Wazzan", "al-Tajir", "al-Sipahi", "al-Askari",
		"al-Amir", "al-Malik", "al-Khan", "al-Bakhshi", "al-Shiqdar", "al-Kotwal",
		"al-Qanungo", "al-Patwari", "al-Muqaddam", "al-Chaudhuri", "al-Lodi", "al-Suri",
		"al-Ghuri", "al-Tughluqi", "al-Khalji", "al-Mamluki", "al-Sayyid", "al-Turki",
		"al-Afghani", "al-Pashtuni", "al-Ghilzai", "al-Lohani", "al-Niazi", "al-Sarwani",
		"al-Farmuli", "al-Kakar", "al-Yusufzai", "al-Khattak", "al-Bangash", "al-Dilazak",
		"al-Qureshi", "al-Hashimi", "al-Ansari", "al-Faruqi", "al-Usmani", "al-Siddiqi",
		"al-Alawi", "al-Husayni", "al-Hasani", "al-Abbasi", "al-Mughal", "al-Chaghatai",
		"al-Barlas", "al-Qarluq", "al-Qipchaq", "al-Tajik", "al-Farisi", "al-Khurasani",
		"al-Bukhari", "al-Samarqandi", "al-Balkhi", "al-Harawi", "al-Gilani", "al-Shirazi",
		"al-Isfahani", "al-Tabrizi", "al-Kirmani", "al-Yazdi", "al-Hindawi", "al-Hindustani",
		"al-Bengali", "al-Deccani"
	};

	private static readonly string[] RenaissanceSwahiliIdentifierInventory =
	new[]
	{
		"al-Kilwi", "al-Mombasi", "al-Malindi", "al-Lamui", "al-Pate", "al-Zanzibari",
		"al-Pembawi", "al-Mogadishu", "al-Brava", "al-Sofali", "al-Mozambiqi", "al-Mafiawi",
		"al-Kiswani", "al-Tangawi", "al-Bagamoyawi", "al-Saadani", "al-Pangani", "al-Tanga-i",
		"al-Mkondoa", "al-Kunduchi", "al-Kaole", "al-Kilindini", "al-Gedi", "al-Shanga",
		"al-Manda", "al-Takwai", "al-Ungujawi", "al-Tumbatu", "al-Panganawi", "al-Faza-i",
		"al-Siyu-i", "al-Kizingitini", "al-Witu-i", "al-Kipini", "al-Ozi", "al-Tana-i",
		"al-Lamu-i", "al-Manda-i", "al-Pokomo", "al-Mijikenda", "al-Vumba", "al-Shimoni",
		"al-Vanga", "al-Takaungu", "al-Kilifi", "al-Mtwapa", "al-Rabai", "al-Ribe",
		"al-Jomvu", "al-Changamwe", "al-Pemba-i", "al-Micheweni", "al-Wete-i", "al-Chake",
		"al-Mkoani", "al-Unguja-i", "al-Makunduchi", "al-Kizimkazi", "al-Mkokotoni", "al-Stone-Town",
		"al-Kilwa-Kisiwani", "al-Kilwa-Kivinje", "al-Lindi", "al-Mikindani", "al-Mtwara", "al-Kiswere",
		"al-Songo-Mnara", "al-Rufiji", "al-Mohoro", "al-Kisiju", "al-Quiloa", "al-Angoche",
		"al-Quelimane", "al-Chinde", "al-Sena", "al-Tete", "al-Inhambane", "al-Sofala-i",
		"al-Ilha", "al-Ibo", "al-Quirimba", "al-Pemba-Mozambique", "al-Mocimboa", "al-Mtwara-i",
		"al-Comori", "al-Ngazidja", "al-Anjouani", "al-Moheli", "al-Mayotte-i", "al-Malagashi",
		"al-Adeni", "al-Hadrami", "al-Yamani", "al-Umani", "al-Hormuzi", "al-Sirafi",
		"al-Basri", "al-Baghdadi", "al-Shirazi", "al-Gujarati", "wa-Shirazi", "wa-Nabhan",
		"wa-Mazrui", "wa-Busaidi", "wa-Harthi", "wa-Hinawi", "wa-Ghafiri", "wa-Mahdali",
		"wa-Hadrami", "wa-Bajuni", "wa-Amu", "wa-Pate", "wa-Faza", "wa-Siyu",
		"wa-Mvita", "wa-Kilwa", "wa-Unguja", "wa-Pemba", "wa-Mafia", "wa-Comoro",
		"wa-Zaramo", "wa-Digo", "wa-Duruma", "wa-Giriama", "wa-Rabai", "wa-Ribe",
		"wa-Jibana", "wa-Kambe", "wa-Chonyi", "wa-Kauma", "wa-Pokomo", "wa-Segeju",
		"wa-Bondei", "wa-Zigua", "wa-Sambaa", "wa-Nguu", "wa-Kwere", "wa-Luguru",
		"wa-Ndengereko", "wa-Matumbi", "wa-Yao", "wa-Makonde", "wa-Nyamwezi", "wa-Sukuma",
		"wa-Kamba", "wa-Taita", "wa-Somali", "wa-Afar", "wa-Oromo", "wa-Habashi",
		"al-Tajiri", "al-Bahhari", "al-Mallah", "al-Nahodha", "al-Mvuvi", "al-Mkulima",
		"al-Mfua", "al-Seremala", "al-Mfinyanzi", "al-Mfumaji", "al-Mshonaji", "al-Mganga",
		"al-Mwalimu", "al-Katib", "al-Qadi", "al-Faqih", "al-Hafiz", "al-Muqri",
		"al-Khatib", "al-Muadhdhin", "al-Attar", "al-Bazzaz", "al-Khayyat", "al-Haddad",
		"al-Najjar", "al-Dabbagh", "al-Sarraj", "al-Sayrafi", "al-Warraq", "al-Jauhari",
		"al-Zargar", "al-Wazzan", "al-Dalali", "al-Wakala", "al-Baniani", "al-Shahbandar",
		"al-Rubban", "al-Jammal", "al-Saqqa", "al-Khabbaz", "al-Tahhan", "al-Qassab",
		"al-Tabib", "al-Kahhal", "al-Hakim", "al-Muallim", "al-Sufi", "al-Sharif",
		"al-Sayyid", "al-Karimi"
	};

	private static readonly string[] RenaissanceHausaAffiliationInventory =
	new[]
	{
		"na-Kano", "na-Katsina", "na-Zazzau", "na-Gobir", "na-Daura", "na-Rano",
		"na-Biram", "na-Gaya", "na-Zamfara", "na-Kebbi", "na-Hadejia", "na-Gumel",
		"na-Kazaure", "na-Dutse", "na-Birnin-Kudu", "na-Wudil", "na-Bichi", "na-Rano-Kasa",
		"na-Gwarzo", "na-Karaye", "na-Rimin-Gado", "na-Kura", "na-Bebeji", "na-Kiru",
		"na-Sumaila", "na-Takai", "na-Garko", "na-Dawakin-Kudu", "na-Dawakin-Tofa", "na-Gezawa",
		"na-Minjibir", "na-Gabasawa", "na-Makoda", "na-Kunchi", "na-Tsanyawa", "na-Shanono",
		"na-Bagwai", "na-Ajingi", "na-Albasu", "na-Warawa", "na-Kabo", "na-Madobi",
		"na-Tofa", "na-Katsina-Arewa", "na-Katsina-Kudu", "na-Dutsin-Ma", "na-Mani", "na-Daura-Kasa",
		"na-Sandamu", "na-Maiadua", "na-Zango", "na-Baure", "na-Bindawa", "na-Kankia",
		"na-Ingawa", "na-Kusada", "na-Musawa", "na-Matazu", "na-Dan-Musa", "na-Safana",
		"na-Batsari", "na-Jibia", "na-Kaita", "na-Rimi", "na-Charanchi", "na-Bakori",
		"na-Funtua", "na-Malumfashi", "na-Kafur", "na-Dandume", "na-Sabuwa", "na-Zaria",
		"na-Soba", "na-Makarfi", "na-Ikara", "na-Kubau", "na-Lere", "na-Kauru",
		"na-Kajuru", "na-Chikun", "na-Birnin-Gwari", "na-Giwa", "na-Igabi", "na-Kagarko",
		"na-Jema-a", "na-Kachia", "na-Kano-Arewa", "na-Kano-Kudu", "na-Kano-Gabas", "na-Kano-Yamma",
		"na-Sokoto", "na-Wurno", "na-Gwadabawa", "na-Illela", "na-Tangaza", "na-Binji",
		"na-Silame", "na-Wamakko", "na-Bodinga", "na-Yabo", "na-Shagari", "na-Tambuwal",
		"na-Kebbe", "na-Gada", "na-Isa", "na-Sabon-Birni", "na-Goronyo", "na-Rabah",
		"na-Kware", "na-Dange", "na-Tureta", "na-Gudu", "na-Argungu", "na-Birnin-Kebbi",
		"na-Bunza", "na-Jega", "na-Aliero", "na-Gwandu", "na-Suru", "na-Bagudo",
		"na-Yauri", "na-Zuru", "na-Fakai", "na-Sakaba", "na-Danko-Wasagu", "na-Arewa-Dandi",
		"na-Gusau", "na-Kaura-Namoda", "na-Talata-Mafara", "na-Anka", "na-Maradun", "na-Maru",
		"na-Bungudu", "na-Tsafe", "na-Zurmi", "na-Shinkafi", "na-Bakura", "na-Bukkuyum",
		"na-Gummi", "na-Bauchi", "na-Katagum", "na-Misau", "na-Jama-are", "na-Ningi",
		"na-Darazo", "na-Ganjuwa", "na-Toro", "na-Dass", "na-Tafawa-Balewa", "na-Alkaleri",
		"na-Kirfi", "na-Bogoro", "na-Warji", "na-Gombe", "na-Akko", "na-Yamaltu-Deba",
		"na-Dukku", "na-Funakaye", "na-Kwami", "na-Nafada", "na-Billiri", "na-Kaltungo",
		"na-Shongom", "na-Balanga", "na-Yola", "na-Mubi", "na-Michika", "na-Madagali",
		"na-Gombi", "na-Hong", "na-Song", "na-Maiha", "na-Fufore", "na-Ganye",
		"na-Jada", "na-Numan", "na-Lamurde", "na-Guyuk", "na-Shelleng", "na-Kukawa",
		"na-Ngazargamu", "na-Maiduguri", "na-Bama", "na-Dikwa", "na-Gwoza", "na-Damboa",
		"na-Konduga", "na-Monguno", "na-Marte", "na-Ngala", "na-Kala-Balge", "na-Agadez",
		"na-Zinder", "na-Maradi", "na-Tahoua", "na-Birni-N-Konni", "na-Dogondoutchi", "na-Tessaoua",
		"na-Tanout", "na-Goure"
	};

	private static readonly RenaissanceWorldSeed[] RenaissanceWorldSeeds =
	new RenaissanceWorldSeed[]
	{
		new(
			Key: "Armenian Highlands",
			NameCultureName: "Renaissance Armenian",
			EthnicityName: "Late Medieval Armenian",
			EthnicGroup: "Caucasian",
			EthnicSubgroup: "Armenian Highlands",
			CultureName: "Armenian Highland (c. 1450)",
			NameForm: RenaissanceWorldNameForm.GivenFamily,
			NameRegex: @"^(?<birthname>[\w'-]{2,}) (?<surname>[\w'-]+(?:ian|yan|uni|akan|etsi|atsi|i))$",
			ReferenceAnchor: "c. 1450",
			EthnicityDescription:
				"The Armenians are a Christian people of the Armenian Highlands and the merchant and ecclesiastical " +
				"communities that reach from Cilicia and the Caucasus into the great trading cities. They recognise one " +
				"another through the Armenian language, the rites and calendar of the Armenian Church, memory of ancestral " +
				"districts and noble houses, and kinship sustained by monasteries, manuscripts and commerce. In the middle " +
				"of the fifteenth century political division has scattered their loyalties without weakening a strong " +
				"shared sense of peoplehood.",
			CultureDescription:
				"Armenian highland culture joins village agriculture, pastoral uplands, fortified lordships, monastic " +
				"learning, manuscript craft and far-ranging commerce. Household, parish, monastery, locality and patronage " +
				"define a person's obligations, while hospitality, church festivals and the reputation of one's family " +
				"carry great weight.",
			PersonalNameDescription:
				"Choose an Armenian personal name used at baptism or within the household. Christian, biblical, saintly " +
				"and older dynastic names all occur, and compounds should remain one hyphenated token.",
			SecondNameDescription:
				"Choose a family, dynastic or place-derived name. Houses ending in -uni and later family forms ending in " +
				"-ian or -yan are common; regional forms such as -etsi or -atsi identify an ancestral town or district.",
			ThirdNameDescription: "",
			MinimumSecondNameCount: 1,
			MinimumThirdNameCount: 0,
			BloodModel: "O-A High Negative",
			SkinProfile: "fair_olive_skin",
			HairProfile: "black_brown_grey_hair",
			EyeProfile: "brown_green_eyes",
			MaleGivenNames: new[]
			{
				"Ashot", "Grigor", "Hovhannes", "Sargis", "Vardan", "Mkhitar",
				"Toros", "Levon", "Kostandin", "Hetum", "Oshin", "Stepanos",
				"Tigran", "Nerses", "Khachatur", "Martiros", "Mkrtich", "Arakel",
				"Avetis", "Bedros", "Davit", "Gagik", "Hamazasp", "Hovsep",
				"Karapet", "Kirakos", "Manvel", "Melkon", "Mesrop", "Movses",
				"Nikoghayos", "Ohannes", "Parsegh", "Petros", "Sahak", "Simavon",
				"Simon", "Smbat", "Tadeos", "Taniel", "Vahram", "Varazdat",
				"Yeghia", "Zakaria", "Zakar", "Atabek", "Jalal", "Prosh",
				"Ruben", "Babken"
			},
			FemaleGivenNames: new[]
			{
				"Anahit", "Mariam", "Hripsime", "Shushan", "Zabel", "Keran",
				"Tamar", "Gohar", "Nane", "Gayane", "Hasmik", "Arpine",
				"Astghik", "Siranuys", "Voski", "Margarit", "Anna", "Mane",
				"Mariun", "Shahandukht", "Khatun", "Tiruhi", "Talitha", "Siran",
				"Nvard", "Varduhi", "Lusine", "Tsovinar", "Parandzem", "Santukht",
				"Taguhi", "Salome", "Sophia", "Khosrovanush", "Katranide", "Arzu",
				"Gulbahar", "Gulizar", "Nazik", "Perchuhi", "Arusyak", "Yeghisabet",
				"Marta", "Hranush", "Goharshah", "Mariamne", "Tamarik", "Zaruhi",
				"Vardeni", "Hranuys"
			},
			MaleSecondNames: new[]
			{
				"Artsruni", "Bagratuni", "Mamikonian", "Pahlavuni", "Proshian", "Orbelian",
				"Zakarian", "Hethumian", "Rubenian", "Arcruni", "Siwni", "Gnuni",
				"Kamsarakan", "Khaghbakian", "Vachutian", "Dopian", "Hasan-Jalalian", "Saharuni",
				"Amatuni", "Rshtuni", "Gabelian", "Taronetsi", "Vanetsi", "Aniatsi",
				"Sisetsi", "Artsakhatsi", "Karinetsi", "Nakhijevantsi", "Shirakatsi", "Loriatsi"
			},
			FemaleSecondNames: new[]
			{
				"Artsruni", "Bagratuni", "Mamikonian", "Pahlavuni", "Proshian", "Orbelian",
				"Zakarian", "Hethumian", "Rubenian", "Arcruni", "Siwni", "Gnuni",
				"Kamsarakan", "Khaghbakian", "Vachutian", "Dopian", "Hasan-Jalalian", "Saharuni",
				"Amatuni", "Rshtuni", "Gabelian", "Taronetsi", "Vanetsi", "Aniatsi",
				"Sisetsi", "Artsakhatsi", "Karinetsi", "Nakhijevantsi", "Shirakatsi", "Loriatsi"
			},
			ThirdNames: Array.Empty<string>()
		),
		new(
			Key: "Georgian Kingdoms",
			NameCultureName: "Renaissance Georgian",
			EthnicityName: "Late Medieval Georgian",
			EthnicGroup: "Caucasian",
			EthnicSubgroup: "Georgian Kingdoms",
			CultureName: "Georgian Court and Highland (c. 1450)",
			NameForm: RenaissanceWorldNameForm.GivenFamily,
			NameRegex: @"^(?<birthname>[\w'-]{2,}) (?<surname>[\w'-]+(?:shvili|dze|iani|ani|eli|uri|vari|i))$",
			ReferenceAnchor: "c. 1450",
			EthnicityDescription:
				"The Georgians are a Kartvelian-speaking Christian people of the eastern and western Georgian lands. Their " +
				"identity rests on the Georgian language and script, the Orthodox Church, descent from a recognised house " +
				"or district, and loyalty to the kingdoms and principalities of Kartli, Kakheti, Imereti, Samtskhe and the " +
				"western coast. Noble genealogies, monasteries and the memory of a united Georgian monarchy bind together " +
				"communities divided by rival rulers.",
			CultureDescription:
				"Georgian courtly culture is organised around royal and princely households, monasteries, fortified " +
				"valleys, vineyards, pastoral uplands and caravan roads. Honour, hospitality, Orthodox observance, " +
				"patronage and service to a house shape both elite and common life, while regional custom remains strong.",
			PersonalNameDescription:
				"Choose a Georgian personal name. Biblical and saintly forms stand beside older Kartvelian and royal " +
				"names, and the same name may appear in several transliterations.",
			SecondNameDescription:
				"Choose a hereditary house or family name. Georgian surnames commonly end in forms such as -shvili, -dze, " +
				"-iani or -eli, which may recall descent, locality or a founding ancestor.",
			ThirdNameDescription: "",
			MinimumSecondNameCount: 1,
			MinimumThirdNameCount: 0,
			BloodModel: "O-A High Negative",
			SkinProfile: "fair_olive_skin",
			HairProfile: "black_brown_grey_hair",
			EyeProfile: "brown_green_eyes",
			MaleGivenNames: new[]
			{
				"Giorgi", "Davit", "Bagrat", "Aleksandre", "Vakhtang", "Konstantine",
				"Demetre", "Levan", "Luarsab", "Simon", "Zurab", "Vameq",
				"Liparit", "Ivane", "Zakaria", "Shota", "Rati", "Kakhaber",
				"Grigol", "Nikoloz", "Teimuraz", "Mamia", "Qvarqvare", "Aghbugha",
				"Sargis", "Beka", "Vardan", "Taqa", "Elizbar", "Revaz",
				"Ramaz", "Rostom", "Kai-Khosro", "Parsadan", "Manuchar", "Otia",
				"Givi", "Gocha", "Jandier", "Zaza", "Chabua", "Papuna",
				"Aslan", "Shermazan", "Iese", "Ioram", "Pharsman", "Mirian",
				"Archil", "Tornike"
			},
			FemaleGivenNames: new[]
			{
				"Tamar", "Rusudan", "Ketevan", "Nino", "Mariam", "Elene",
				"Ana", "Nestan-Darejan", "Tinatin", "Gulshar", "Gvantsa", "Natela",
				"Khvaramze", "Dedis-Imedi", "Rodam", "Darejan", "Salome", "Marta",
				"Sophia", "Theodora", "Borena", "Gurandukht", "Kata", "Lela",
				"Mzevinar", "Mzekhatun", "Tamar-Khatun", "Anuka", "Tekla", "Eliso",
				"Natia", "Nunu", "Khatia", "Manana", "Makrine", "Marina",
				"Kristine", "Barbare", "Sidonia", "Susanna", "Thamar", "Rusudan-Khatun",
				"Khoshak", "Aspasia", "Eudokia", "Irene", "Helena", "Mariam-Khatun",
				"Gulandukht", "Khorashan"
			},
			MaleSecondNames: new[]
			{
				"Bagrationi", "Dadiani", "Jaqeli", "Orbeliani", "Vardanisdze", "Amilakhvari",
				"Abashidze", "Chikovani", "Gurieli", "Shervashidze", "Tsitsishvili", "Baratashvili",
				"Tsereteli", "Mkhargrdzeli", "Panaskerteli", "Avalishvili", "Andronikashvili", "Kvenipneveli",
				"Anchabadze", "Toreli", "Surameli", "Kakhaberidze", "Phanaskerteli", "Cholokashvili",
				"Machabeli", "Eristavi", "Kherkheulidze", "Laskhishvili", "Palavandishvili", "Javakhishvili"
			},
			FemaleSecondNames: new[]
			{
				"Bagrationi", "Dadiani", "Jaqeli", "Orbeliani", "Vardanisdze", "Amilakhvari",
				"Abashidze", "Chikovani", "Gurieli", "Shervashidze", "Tsitsishvili", "Baratashvili",
				"Tsereteli", "Mkhargrdzeli", "Panaskerteli", "Avalishvili", "Andronikashvili", "Kvenipneveli",
				"Anchabadze", "Toreli", "Surameli", "Kakhaberidze", "Phanaskerteli", "Cholokashvili",
				"Machabeli", "Eristavi", "Kherkheulidze", "Laskhishvili", "Palavandishvili", "Javakhishvili"
			},
			ThirdNames: Array.Empty<string>()
		),
		new(
			Key: "Kurdish Principalities",
			NameCultureName: "Renaissance Kurdish",
			EthnicityName: "Late Medieval Kurd",
			EthnicGroup: "Iranian",
			EthnicSubgroup: "Kurdish Principalities",
			CultureName: "Kurdish Emirate and Highland (c. 1450)",
			NameForm: RenaissanceWorldNameForm.GivenPatronymToponym,
			NameRegex: @"^(?<birthname>[\w'-]{2,}) (?<patronym>(?:ibn|bint)-[\w'-]{2,}) (?<toponym>al-[\w'-]{2,})$",
			ReferenceAnchor: "c. 1450",
			EthnicityDescription:
				"The Kurds are an Iranian-speaking people of the Taurus and Zagros highlands and the plains that lie " +
				"between them. They define themselves through dialect, mountain homeland, tribal and dynastic descent, " +
				"customary obligation and allegiance to a local emir or confederation. Sunni Islam is widespread, but " +
				"saintly lineages, local religious traditions and the authority of clan elders give each district its own " +
				"character.",
			CultureDescription:
				"The culture of the Kurdish principalities combines fortified towns, mountain villages, seasonal pasture, " +
				"tribal retinues, caravan routes and service to local emirs or neighbouring dynasties. Kinship, " +
				"guest-right, customary settlement, religious patronage and command of difficult terrain are central " +
				"social facts.",
			PersonalNameDescription:
				"Choose a Kurdish or Islamicate personal name. Iranian, Arabic and Turkic forms all occur in the " +
				"principalities, and religious compounds are entered as one hyphenated token.",
			SecondNameDescription:
				"Enter a patronym as ibn-Father for a man or bint-Father for a woman. The father's personal name follows " +
				"the prefix and is not a hereditary surname.",
			ThirdNameDescription:
				"Enter an al- nisba naming a town, district, tribe, dynasty, religious affiliation or recognised " +
				"occupation, such as al-Bidlisi or al-Hakkari. The complete nisba is one hyphenated token.",
			MinimumSecondNameCount: 1,
			MinimumThirdNameCount: 200,
			BloodModel: "A Dominant",
			SkinProfile: "fair_olive_skin",
			HairProfile: "black_brown_grey_hair",
			EyeProfile: "brown_green_eyes",
			MaleGivenNames: new[]
			{
				"Azad", "Bahram", "Bakhtiyar", "Baran", "Bedir", "Beko",
				"Berxwedan", "Botan", "Cemal", "Dara", "Dawud", "Diyar",
				"Farhad", "Ferhad", "Hesen", "Huseyn", "Idris", "Ismail",
				"Jafar", "Jalal", "Kawa", "Keykhosrow", "Khidir", "Mahmud",
				"Mansur", "Mirza", "Muhammad", "Murad", "Musa", "Nasir",
				"Qadir", "Qasim", "Rashid", "Rustam", "Salah", "Saman",
				"Shahin", "Sharaf", "Sherko", "Sinan", "Suleyman", "Tahir",
				"Timur", "Umar", "Xelil", "Yusuf", "Zayn", "Ziyad",
				"Zubayr", "Zirak"
			},
			FemaleGivenNames: new[]
			{
				"Adila", "Aisha", "Asiya", "Avin", "Barin", "Berfin",
				"Berivan", "Binevsh", "Delal", "Diljin", "Dilan", "Fatima",
				"Gulbahar", "Gulistan", "Gulizar", "Havin", "Hediye", "Helin",
				"Jiyan", "Khanzade", "Khatun", "Khurshid", "Layla", "Mahin",
				"Malak", "Maryam", "Mizgin", "Narin", "Nazdar", "Nergiz",
				"Nesrin", "Perihan", "Rojin", "Ruken", "Sakina", "Shirin",
				"Sitare", "Siti", "Sosan", "Tara", "Zahra", "Zaynab",
				"Zerin", "Zilan", "Zin", "Zozan", "Amina", "Huma",
				"Khanim", "Yasmin"
			},
			MaleSecondNames: new[]
			{
				"ibn-Ahmad", "ibn-Ali", "ibn-Bakr", "ibn-Dawud", "ibn-Hasan", "ibn-Husayn",
				"ibn-Ibrahim", "ibn-Ismail", "ibn-Jafar", "ibn-Khalil", "ibn-Mahmud", "ibn-Mansur",
				"ibn-Muhammad", "ibn-Murad", "ibn-Musa", "ibn-Nasir", "ibn-Qadir", "ibn-Qasim",
				"ibn-Rashid", "ibn-Salah", "ibn-Sinan", "ibn-Suleyman", "ibn-Tahir", "ibn-Umar",
				"ibn-Yusuf"
			},
			FemaleSecondNames: new[]
			{
				"bint-Ahmad", "bint-Ali", "bint-Bakr", "bint-Dawud", "bint-Hasan", "bint-Husayn",
				"bint-Ibrahim", "bint-Ismail", "bint-Jafar", "bint-Khalil", "bint-Mahmud", "bint-Mansur",
				"bint-Muhammad", "bint-Murad", "bint-Musa", "bint-Nasir", "bint-Qadir", "bint-Qasim",
				"bint-Rashid", "bint-Salah", "bint-Sinan", "bint-Suleyman", "bint-Tahir", "bint-Umar",
				"bint-Yusuf"
			},
			ThirdNames: RenaissanceKurdishNisbaInventory
		),
		new(
			Key: "Mamluk Egypt and Syria",
			NameCultureName: "Renaissance Mamluk Arabic",
			EthnicityName: "Mamluk-Era Egyptian Arab",
			EthnicGroup: "Middle Eastern",
			EthnicSubgroup: "Mamluk Sultanate",
			CultureName: "Mamluk Egyptian-Syrian (c. 1450)",
			NameForm: RenaissanceWorldNameForm.GivenPatronymToponym,
			NameRegex: @"^(?<birthname>[\w'-]{2,}) (?<patronym>(?:ibn|bint)-[\w'-]{2,}) (?<toponym>al-[\w'-]{2,})$",
			ReferenceAnchor: "c. 1450",
			EthnicityDescription:
				"The Egyptian Arabs are the Arabic-speaking people of the Nile valley, Delta and the towns that bind Egypt " +
				"to Syria and the Red Sea. They distinguish themselves by attachment to Egypt, family and neighbourhood, " +
				"village or urban quarter, and the local forms of Arabic through which ancestry and reputation are " +
				"remembered. Muslim, Christian and Jewish communities preserve different religious laws and institutions, " +
				"but all share the markets, waterways and settled life of the country.",
			CultureDescription:
				"Mamluk culture is centred on the great cities, market towns, irrigated countryside and caravan corridors " +
				"of Egypt and Syria. Household patronage, neighbourhood, guild, mosque, church, synagogue, madrasa and " +
				"military service structure daily life, while Arabic public culture coexists with Turkic, Circassian, " +
				"Greek, Armenian and other languages.",
			PersonalNameDescription:
				"Choose an Arabic ism, the person's own name. Names such as Ahmad, Ali, Hasan, Aisha and Fatima are " +
				"personal names; Abd-al- compounds remain one hyphenated token.",
			SecondNameDescription:
				"Enter the nasab as ibn-Father for a man or bint-Father for a woman. The element identifies the father; " +
				"use one generation here even when a formal genealogy would continue through several ancestors.",
			ThirdNameDescription:
				"Enter an al- nisba. It may identify a town or region, a tribe or descent group, a craft, a legal school, " +
				"a religious order or a service household; examples include al-Qahiri, al-Dimashqi and al-Haddad.",
			MinimumSecondNameCount: 1,
			MinimumThirdNameCount: 200,
			BloodModel: "Majority O Minor A",
			SkinProfile: "olive_skin",
			HairProfile: "black_brown_grey_hair",
			EyeProfile: "brown_green_eyes",
			MaleGivenNames: new[]
			{
				"Abbas", "Abd-al-Aziz", "Abd-al-Latif", "Abd-al-Rahman", "Abdallah", "Ahmad",
				"Ali", "Ammar", "Anas", "Ayyub", "Badr", "Baybars",
				"Bilal", "Burhan", "Dawud", "Faraj", "Fath", "Ghazi",
				"Hasan", "Husayn", "Ibrahim", "Idris", "Ilyas", "Ismail",
				"Jafar", "Jamal", "Khalil", "Mahmud", "Mansur", "Masud",
				"Muhammad", "Musa", "Mustafa", "Nasir", "Qasim", "Qutuz",
				"Ramadan", "Rashid", "Ridwan", "Saad", "Salah", "Shaban",
				"Shams", "Sinan", "Sulayman", "Tahir", "Umar", "Usama",
				"Yusuf", "Zayn"
			},
			FemaleGivenNames: new[]
			{
				"Aisha", "Amina", "Asma", "Aziza", "Bahiyya", "Badr",
				"Baraka", "Bushra", "Dunya", "Fatima", "Fawziyya", "Ghaliya",
				"Hajar", "Halima", "Hind", "Husn", "Iman", "Jamila",
				"Jawhara", "Khadija", "Khawla", "Layla", "Lubna", "Mahbuba",
				"Malika", "Maryam", "Munira", "Nafisa", "Nura", "Qamar",
				"Rabi'a", "Rahma", "Rayhana", "Ruqayya", "Sa'da", "Safiyya",
				"Salma", "Samra", "Shajarat", "Shams", "Sitt-al-Arab", "Sitt-al-Mulk",
				"Sukayna", "Tahira", "Yasmine", "Zahra", "Zaynab", "Zubayda",
				"Zumurrud", "Ward"
			},
			MaleSecondNames: new[]
			{
				"ibn-Abbas", "ibn-Abdallah", "ibn-Ahmad", "ibn-Ali", "ibn-Ayyub", "ibn-Bakr",
				"ibn-Dawud", "ibn-Faraj", "ibn-Hasan", "ibn-Husayn", "ibn-Ibrahim", "ibn-Ismail",
				"ibn-Jafar", "ibn-Khalil", "ibn-Mahmud", "ibn-Mansur", "ibn-Muhammad", "ibn-Musa",
				"ibn-Nasir", "ibn-Qasim", "ibn-Rashid", "ibn-Salah", "ibn-Sulayman", "ibn-Umar",
				"ibn-Yusuf"
			},
			FemaleSecondNames: new[]
			{
				"bint-Abbas", "bint-Abdallah", "bint-Ahmad", "bint-Ali", "bint-Ayyub", "bint-Bakr",
				"bint-Dawud", "bint-Faraj", "bint-Hasan", "bint-Husayn", "bint-Ibrahim", "bint-Ismail",
				"bint-Jafar", "bint-Khalil", "bint-Mahmud", "bint-Mansur", "bint-Muhammad", "bint-Musa",
				"bint-Nasir", "bint-Qasim", "bint-Rashid", "bint-Salah", "bint-Sulayman", "bint-Umar",
				"bint-Yusuf"
			},
			ThirdNames: RenaissanceMamlukNisbaInventory
		),
		new(
			Key: "Timurid Persianate Central Asia",
			NameCultureName: "Renaissance Timurid Persianate",
			EthnicityName: "Late Medieval Chagatai Turk",
			EthnicGroup: "Central Asian",
			EthnicSubgroup: "Timurid Realm",
			CultureName: "Timurid Persianate Court (c. 1450)",
			NameForm: RenaissanceWorldNameForm.GivenClan,
			NameRegex: @"^(?<birthname>[\w'-]{2,}) (?<familygroupname>[\w'-]{2,})$",
			ReferenceAnchor: "c. 1450",
			EthnicityDescription:
				"The Chagatai Turks are a Turkic-speaking people of Transoxiana, the steppe margins and the military " +
				"households of the Timurid lands. They trace belonging through clan and lineage, remembered descent from " +
				"steppe confederations, and service to a ruler or princely house. Islam and Persian literary culture are " +
				"deeply rooted among the settled elite, while Turkic speech, horse-breeding traditions and kinship remain " +
				"strong marks of identity.",
			CultureDescription:
				"Timurid Persianate culture flourishes in courts, garden cities, workshops, madrasas and mobile military " +
				"households from Samarqand and Herat across Khurasan and Transoxiana. Persian letters, Turkic speech, " +
				"Islamic scholarship, Sufi affiliation, monumental patronage and clan-based service coexist within the " +
				"same social world.",
			PersonalNameDescription:
				"Choose a personal name drawn from Turkic, Persian and Arabic usage. Courtly compounds and titles used as " +
				"names are kept as one hyphenated token.",
			SecondNameDescription:
				"Choose a clan, lineage, service household or recognised regional affiliation. Names such as Barlas, " +
				"Dughlat and Jalayir indicate inherited or political belonging rather than a modern surname.",
			ThirdNameDescription: "",
			MinimumSecondNameCount: 1,
			MinimumThirdNameCount: 0,
			BloodModel: "A Dominant",
			SkinProfile: "fair_olive_skin",
			HairProfile: "black_brown_grey_hair",
			EyeProfile: "brown_green_eyes",
			MaleGivenNames: new[]
			{
				"Abdallah", "Abd-al-Razzaq", "Abu-Said", "Ahmad", "Ali", "Alisher",
				"Amir", "Babur", "Badi", "Baha-al-Din", "Bahram", "Baysunghur",
				"Dawlatshah", "Farrukh", "Ghiyath", "Habib", "Hamza", "Hasan",
				"Husayn", "Ibrahim", "Iskandar", "Jalal", "Jami", "Kamal",
				"Khalil", "Khizr", "Mahmud", "Mansur", "Miran", "Mirza",
				"Muhammad", "Muzaffar", "Nasir", "Nizam", "Pir-Muhammad", "Qasim",
				"Qutb", "Rustam", "Sa'd", "Shah-Malik", "Shah-Rukh", "Sharaf",
				"Sultan-Ahmad", "Tahir", "Timur", "Ulugh-Beg", "Umar-Shaykh", "Yusuf",
				"Zayn-al-Abidin", "Zahir"
			},
			FemaleGivenNames: new[]
			{
				"Agha-Begim", "Aisha", "Anis", "Badi-al-Jamal", "Bibi-Khanum", "Dilshad",
				"Fatima", "Gauhar-Shad", "Gulbadan", "Gulbahar", "Gulchehra", "Gulrukh",
				"Habiba", "Halima", "Hanzada", "Humayun-Begim", "Iffat", "Jahangir-Begim",
				"Jahan-Malik", "Jahanara", "Khadija", "Khanzada", "Khurshid", "Latifa",
				"Mah-Begim", "Mahd-i-Ulya", "Mahin", "Malika", "Maryam", "Mehr-Nigar",
				"Mihr", "Mihriban", "Nigar", "Nur-Jahan", "Pari", "Qutlugh",
				"Rabi'a", "Ruqayya", "Salima", "Shad-Malik", "Shirin", "Sitara",
				"Sultan-Bakht", "Sultan-Nigar", "Turkan", "Uljay", "Yadgar", "Yasmin",
				"Zahra", "Zubayda"
			},
			MaleSecondNames: new[]
			{
				"Barlas", "Chaghatai", "Dughlat", "Arlat", "Jalayir", "Qarluq",
				"Qipchaq", "Qongirat", "Naiman", "Manghit", "Suldus", "Qaraunas",
				"Barlas-Timuri", "Samarqandi", "Bukhari", "Herati", "Balkhi", "Tirmidhi",
				"Khujandi", "Kashghari", "Yazdi", "Shirazi", "Isfahani", "Tabrizi",
				"Khurasani", "Sistani", "Badakhshi", "Farghani", "Marvazi", "Nishapuri"
			},
			FemaleSecondNames: new[]
			{
				"Barlas", "Chaghatai", "Dughlat", "Arlat", "Jalayir", "Qarluq",
				"Qipchaq", "Qongirat", "Naiman", "Manghit", "Suldus", "Qaraunas",
				"Barlas-Timuri", "Samarqandi", "Bukhari", "Herati", "Balkhi", "Tirmidhi",
				"Khujandi", "Kashghari", "Yazdi", "Shirazi", "Isfahani", "Tabrizi",
				"Khurasani", "Sistani", "Badakhshi", "Farghani", "Marvazi", "Nishapuri"
			},
			ThirdNames: Array.Empty<string>()
		),
		new(
			Key: "Delhi Sultanate Hindavi",
			NameCultureName: "Renaissance Delhi Sultanate",
			EthnicityName: "Late Medieval Afghan",
			EthnicGroup: "South Asian",
			EthnicSubgroup: "North Indian Sultanates",
			CultureName: "Delhi Sultanate (c. 1450)",
			NameForm: RenaissanceWorldNameForm.GivenClan,
			NameRegex: @"^(?<birthname>[\w'-]{2,}) (?<familygroupname>al-[\w'-]{2,})$",
			ReferenceAnchor: "c. 1450",
			EthnicityDescription:
				"The Afghans are a Pashto-speaking people of the mountain country between Khurasan and the Indus and of " +
				"the military colonies established farther into northern India. They define themselves through patrilineal " +
				"tribe, clan genealogy, homeland and the obligations of hospitality, protection and feud. Service in the " +
				"sultanates brings many Afghan houses into Persianate court life without erasing the authority of lineage.",
			CultureDescription:
				"Delhi sultanate culture joins Persianate courts and chancelleries to Hindavi-speaking towns, market " +
				"quarters, Sufi hospices, temples, cavalry households and the agrarian life of the Doab. Patronage, " +
				"military or scribal service, religious learning, caste or biradari and attachment to a particular town " +
				"shape social standing.",
			PersonalNameDescription:
				"Choose a personal name suitable to an Islamicate court or military household. Arabic devotional names, " +
				"Persian names and Turkic or Afghan forms all occur, with compounds kept as one token.",
			SecondNameDescription:
				"Choose an affiliation in al- form. It may identify a place of origin, scholarly or Sufi association, " +
				"profession, dynasty, tribe or service group, such as al-Dihlawi, al-Chishti, al-Katib or al-Lodi.",
			ThirdNameDescription: "",
			MinimumSecondNameCount: 200,
			MinimumThirdNameCount: 0,
			BloodModel: "B Dominant",
			SkinProfile: "swarthy_skin",
			HairProfile: "black_brown_grey_hair",
			EyeProfile: "brown_green_eyes",
			MaleGivenNames: new[]
			{
				"Abbas", "Abdallah", "Ahmad", "Ala-al-Din", "Ali", "Amir",
				"Arif", "Bahlul", "Bakhtiyar", "Burhan", "Daud", "Dilawar",
				"Firuz", "Ghazi", "Hasan", "Husayn", "Ibrahim", "Iltutmish",
				"Imad", "Islam", "Jalal", "Jamal", "Kamal", "Khizr",
				"Khusrau", "Lodi", "Mahmud", "Malik", "Mubarak", "Muhammad",
				"Muiz", "Mustafa", "Nasir", "Nizam", "Qadir", "Qasim",
				"Qutb", "Rafi", "Rahim", "Rukn", "Saif", "Salim",
				"Shams", "Sikandar", "Taj", "Tughluq", "Umar", "Yusuf",
				"Zafar", "Zayn"
			},
			FemaleGivenNames: new[]
			{
				"Aisha", "Amina", "Asiya", "Bano", "Bibi", "Chand",
				"Daulat", "Dilshad", "Fatima", "Gauhar", "Gul", "Gulbadan",
				"Gulbahar", "Gulnar", "Habiba", "Hamida", "Huma", "Jahan",
				"Jahanara", "Jamila", "Khadija", "Khanzada", "Khurshid", "Laila",
				"Maham", "Mahin", "Malika", "Maryam", "Mehr", "Mumtaz",
				"Nadira", "Nigar", "Nur", "Pari", "Qamar", "Rabi'a",
				"Razia", "Ruqayya", "Salima", "Shirin", "Sultana", "Taj-Bibi",
				"Turkan", "Yasmin", "Zahra", "Zaynab", "Zohra", "Anarkali",
				"Izzat", "Sughra"
			},
			MaleSecondNames: RenaissanceDelhiAffiliationInventory,
			FemaleSecondNames: RenaissanceDelhiAffiliationInventory,
			ThirdNames: Array.Empty<string>()
		),
		new(
			Key: "Rajputana",
			NameCultureName: "Renaissance Rajput",
			EthnicityName: "Late Medieval Rajput",
			EthnicGroup: "South Asian",
			EthnicSubgroup: "Rajputana",
			CultureName: "Rajput Court and Estate (c. 1450)",
			NameForm: RenaissanceWorldNameForm.GivenClan,
			NameRegex: @"^(?<birthname>[\w'-]{2,}) (?<familygroupname>[\w'-]{2,})$",
			ReferenceAnchor: "c. 1450",
			EthnicityDescription:
				"The Rajputs are warrior and landholding lineages of northern and western India who claim descent from " +
				"remembered royal, heroic or sacred ancestors. Clan genealogy, marriage alliance, possession of forts and " +
				"estates, patronage of temples and bards, and the defence of family honour are the principal signs of " +
				"Rajput identity. A person's standing depends as much on the reputation of the clan and its branch as on " +
				"service to any one ruler.",
			CultureDescription:
				"Rajput court culture is centred on fortified capitals, estate villages, cavalry retinues, temple " +
				"patronage, bardic genealogy and negotiated ties with cultivators, merchants and religious specialists. " +
				"Honour, lineage, marriage alliance, gift-giving and military service govern elite life and influence " +
				"those attached to it.",
			PersonalNameDescription:
				"Choose a Sanskritic or vernacular personal name used within a Rajput household. Regnal epithets and " +
				"titles such as raja or rawal are not part of this element.",
			SecondNameDescription:
				"Choose the clan or dynastic house to which the character belongs, such as Chauhan, Guhila, Rathore or " +
				"Paramara. This is inherited lineage identity, not an occupation.",
			ThirdNameDescription: "",
			MinimumSecondNameCount: 1,
			MinimumThirdNameCount: 0,
			BloodModel: "B Dominant",
			SkinProfile: "swarthy_skin",
			HairProfile: "black_brown_grey_hair",
			EyeProfile: "brown_green_eyes",
			MaleGivenNames: new[]
			{
				"Ajay", "Ajit", "Amar", "Anang", "Arjun", "Arnoraj",
				"Bhan", "Bhim", "Bhoj", "Chand", "Chandra", "Dalpat",
				"Dev", "Dharam", "Durgadas", "Gaj", "Gopal", "Govind",
				"Hammir", "Haridas", "Hariraj", "Jai", "Jaitra", "Jagat",
				"Kalyan", "Kanhad", "Karan", "Kirtipal", "Lakhan", "Lakshman",
				"Madho", "Mangal", "Manik", "Mokul", "Narayan", "Padam",
				"Pratap", "Prithvi", "Rai", "Raj", "Rana", "Rao",
				"Ratan", "Rudra", "Samant", "Sangram", "Suraj", "Tej",
				"Uday", "Vikram"
			},
			FemaleGivenNames: new[]
			{
				"Abhay", "Ajab", "Amrita", "Anupama", "Asha", "Bala",
				"Bhavani", "Chanda", "Charumati", "Deval", "Devaki", "Durgavati",
				"Gauri", "Guna", "Hansa", "Heer", "Indra", "Jaya",
				"Jivanta", "Kalyani", "Kamala", "Kanchan", "Karnavati", "Kesar",
				"Krishna", "Kumbha", "Lada", "Lalita", "Lakshmi", "Madan",
				"Mahadevi", "Manavati", "Meera", "Mohini", "Nanda", "Padma",
				"Padmini", "Parvati", "Prabha", "Rajal", "Ratna", "Rukmini",
				"Sajjan", "Samyukta", "Sati", "Sona", "Tara", "Uma",
				"Vira", "Vijaya"
			},
			MaleSecondNames: new[]
			{
				"Chauhan", "Guhila", "Sisodia", "Rathore", "Kachhwaha", "Paramara",
				"Solanki", "Chalukya", "Tomara", "Chandel", "Bhati", "Jhala",
				"Hada", "Deora", "Songara", "Bundela", "Gaur", "Parihar",
				"Panwar", "Jadeja", "Vaghela", "Nikumbh", "Tanwar", "Dahiya",
				"Gohil", "Sengar", "Bais", "Bargujar", "Khangar", "Mori"
			},
			FemaleSecondNames: new[]
			{
				"Chauhan", "Guhila", "Sisodia", "Rathore", "Kachhwaha", "Paramara",
				"Solanki", "Chalukya", "Tomara", "Chandel", "Bhati", "Jhala",
				"Hada", "Deora", "Songara", "Bundela", "Gaur", "Parihar",
				"Panwar", "Jadeja", "Vaghela", "Nikumbh", "Tanwar", "Dahiya",
				"Gohil", "Sengar", "Bais", "Bargujar", "Khangar", "Mori"
			},
			ThirdNames: Array.Empty<string>()
		),
		new(
			Key: "Bengal Sultanate",
			NameCultureName: "Renaissance Bengali",
			EthnicityName: "Late Medieval Bengali",
			EthnicGroup: "South Asian",
			EthnicSubgroup: "Bengal Delta",
			CultureName: "Bengal Sultanate (c. 1450)",
			NameForm: RenaissanceWorldNameForm.GivenOnly,
			NameRegex: @"^(?<birthname>[\w'-]{2,})$",
			ReferenceAnchor: "c. 1450",
			EthnicityDescription:
				"The Bengalis are the people of the Ganges-Brahmaputra delta, joined by the Bengali language and by life " +
				"among its rivers, rice lands, forests and trading towns. Local lineage, village and district remain " +
				"powerful identities, while Muslim, Hindu and Buddhist traditions give different communities their own " +
				"institutions and genealogies. Poetic speech, riverine commerce and attachment to the fertile delta " +
				"distinguish Bengalis from neighbouring peoples.",
			CultureDescription:
				"Bengal sultanate culture follows river routes through rice-growing villages, ports, market towns, " +
				"mosques, temples, shrines and court centres. Land clearance, boat travel, textile production, patronage " +
				"and the coexistence of Persian administration with Bengali speech shape everyday life.",
			PersonalNameDescription:
				"Choose a single Bengali, Sanskritic or Islamicate personal name. Titles, patronyms and devotional labels " +
				"are separate elements whose order depends on household and religious community.",
			SecondNameDescription: "",
			ThirdNameDescription: "",
			MinimumSecondNameCount: 0,
			MinimumThirdNameCount: 0,
			BloodModel: "B Dominant",
			SkinProfile: "swarthy_skin",
			HairProfile: "black_brown_grey_hair",
			EyeProfile: "brown_green_eyes",
			MaleGivenNames: new[]
			{
				"Alauddin", "Ali", "Ahmad", "Azam", "Bakhtiyar", "Barbak",
				"Bayazid", "Daud", "Fath", "Ghiyas", "Hamza", "Hasan",
				"Husayn", "Ibrahim", "Ilyas", "Ismail", "Jalal", "Kamal",
				"Mahmud", "Mansur", "Muazzam", "Muhammad", "Nasir", "Qadir",
				"Rukn", "Sikandar", "Yusuf", "Aditya", "Ananta", "Chandra",
				"Deva", "Dharmapala", "Ganesha", "Govinda", "Hari", "Jayanta",
				"Lakshman", "Madhava", "Narayan", "Pratap", "Raghava", "Ram",
				"Rudra", "Shashanka", "Shyam", "Surya", "Vijaya", "Vishnu",
				"Udaya", "Keshava"
			},
			FemaleGivenNames: new[]
			{
				"Aisha", "Amina", "Bibi", "Chand", "Fatima", "Gul",
				"Habiba", "Halima", "Jahan", "Jamila", "Khadija", "Laila",
				"Malika", "Maryam", "Nigar", "Nur", "Qamar", "Rabi'a",
				"Ruqayya", "Salima", "Shirin", "Sultana", "Yasmin", "Zahra",
				"Zaynab", "Aparna", "Bhanumati", "Chandra", "Devi", "Durga",
				"Gauri", "Indira", "Jaya", "Kamala", "Kanchana", "Lakshmi",
				"Lalita", "Madhavi", "Malati", "Manjari", "Padma", "Parvati",
				"Prabha", "Radha", "Ratna", "Sarasvati", "Shanti", "Tara",
				"Uma", "Vijaya"
			},
			MaleSecondNames: Array.Empty<string>(),
			FemaleSecondNames: Array.Empty<string>(),
			ThirdNames: Array.Empty<string>()
		),
		new(
			Key: "Vijayanagara Deccan",
			NameCultureName: "Renaissance Vijayanagara",
			EthnicityName: "Late Medieval Kannadiga",
			EthnicGroup: "South Asian",
			EthnicSubgroup: "Southern Deccan",
			CultureName: "Vijayanagara Imperial (c. 1450)",
			NameForm: RenaissanceWorldNameForm.GivenClan,
			NameRegex: @"^(?<birthname>[\w'-]{2,}) (?<familygroupname>[\w'-]{2,})$",
			ReferenceAnchor: "c. 1450",
			EthnicityDescription:
				"The Kannadigas are a Kannada-speaking people of the southern Deccan, tied to the plateau's towns, temple " +
				"lands, farming districts and warrior houses. They recognise kin and status through locality, caste and " +
				"occupational community, lineage deity and service to a chief, temple or king. Kannada literary and " +
				"inscriptional traditions give a shared language to communities whose customs vary between court, market " +
				"and village.",
			CultureDescription:
				"Vijayanagara culture brings together royal and provincial courts, temple complexes, irrigated " +
				"agriculture, merchant corporations, military nayakas, craft quarters and long-distance trade. Kannada, " +
				"Telugu, Tamil and Sanskrit learning all circulate within a political order built on land grants, service, " +
				"devotion and regional patronage.",
			PersonalNameDescription:
				"Choose a southern Indian personal name. Sanskritic devotional names, royal names and vernacular forms all " +
				"occur; honorifics and regnal titles are omitted.",
			SecondNameDescription:
				"Choose a dynasty, house, service lineage, locality or corporate affiliation. It identifies the network " +
				"through which the character claims descent, office or patronage.",
			ThirdNameDescription: "",
			MinimumSecondNameCount: 1,
			MinimumThirdNameCount: 0,
			BloodModel: "B Dominant",
			SkinProfile: "swarthy_skin",
			HairProfile: "black_brown_grey_hair",
			EyeProfile: "brown_green_eyes",
			MaleGivenNames: new[]
			{
				"Achyuta", "Ananta", "Annama", "Appaji", "Basava", "Bukka",
				"Chikka", "Deva", "Devaraya", "Gopala", "Harihara", "Immadi",
				"Irugappa", "Jakkana", "Kampana", "Keshava", "Krishna", "Krishnadeva",
				"Kumara", "Lakshmana", "Madhava", "Mallappa", "Mallikarjuna", "Narasa",
				"Narasimha", "Narayana", "Obanna", "Pampa", "Pemmasani", "Prolaya",
				"Rama", "Ramachandra", "Ramaraya", "Saluva", "Sangama", "Sankara",
				"Siddappa", "Singama", "Someshvara", "Timmappa", "Tirumala", "Vema",
				"Venkatadri", "Venkata", "Virabhadra", "Virupaksha", "Vittala", "Yadava",
				"Yellappa", "Govinda"
			},
			FemaleGivenNames: new[]
			{
				"Abhirami", "Akkadevi", "Alamelu", "Ammanga", "Annapurna", "Chennamma",
				"Chikka", "Devaki", "Devamma", "Gangadevi", "Gauri", "Honnamma",
				"Iruladevi", "Jakkamma", "Kamala", "Kamalamba", "Kanchana", "Komalavalli",
				"Lakshmi", "Lakshmidevi", "Lingamma", "Malladevi", "Mallamma", "Mangamma",
				"Nagaladevi", "Nagamma", "Narasamma", "Padmavati", "Pampa", "Parvati",
				"Rangamma", "Rudramadevi", "Sachi", "Sarasvati", "Shankari", "Shantala",
				"Sita", "Somaladevi", "Subbamma", "Timmakka", "Tirumala", "Umadevi",
				"Vasantha", "Venkatamma", "Vengalamba", "Viramma", "Vittalamba", "Yellamma",
				"Bhagirathi", "Rukmini"
			},
			MaleSecondNames: new[]
			{
				"Sangama", "Saluva", "Tuluva", "Aravidu", "Nayaka", "Wodeyar",
				"Reddy", "Velama", "Pemmasani", "Aravidu-Konda", "Hoysala", "Chalukya",
				"Yadava", "Gajapati", "Kamma", "Ballala", "Banas", "Kadamba",
				"Ganga", "Chola", "Kakatiya", "Kolathiri", "Keladi", "Madurai",
				"Penukonda", "Hampi", "Anegondi", "Chandragiri", "Udayagiri", "Kondavidu"
			},
			FemaleSecondNames: new[]
			{
				"Sangama", "Saluva", "Tuluva", "Aravidu", "Nayaka", "Wodeyar",
				"Reddy", "Velama", "Pemmasani", "Aravidu-Konda", "Hoysala", "Chalukya",
				"Yadava", "Gajapati", "Kamma", "Ballala", "Banas", "Kadamba",
				"Ganga", "Chola", "Kakatiya", "Kolathiri", "Keladi", "Madurai",
				"Penukonda", "Hampi", "Anegondi", "Chandragiri", "Udayagiri", "Kondavidu"
			},
			ThirdNames: Array.Empty<string>()
		),
		new(
			Key: "Ming China",
			NameCultureName: "Renaissance Ming Chinese",
			EthnicityName: "Ming Han Chinese",
			EthnicGroup: "East Asian",
			EthnicSubgroup: "Ming China",
			CultureName: "Ming Chinese (c. 1450)",
			NameForm: RenaissanceWorldNameForm.FamilyGiven,
			NameRegex: @"^(?<surname>[\w'-]{1,}) (?<birthname>[\w'-]{1,})$",
			ReferenceAnchor: "c. 1450",
			EthnicityDescription:
				"The Han Chinese are the majority people of the Ming realm, bound by written Chinese, ancestral rites, " +
				"patrilineal family and a long memory of regional states and dynasties. County and native-place identity, " +
				"lineage hall, household genealogy and local speech distinguish one community from another, while the " +
				"classical tradition supplies a common language of education and government.",
			CultureDescription:
				"Ming culture is organised around patrilineal households, farming villages, market towns, great cities, " +
				"temples, guilds and the institutions of the imperial state. Lineage property, examinations, ancestral " +
				"rites, tax and labour obligations, craft production and river or maritime trade shape opportunity.",
			PersonalNameDescription:
				"Choose the personal name that follows the family name. It may contain one or two syllables, rendered here " +
				"as a single token; courtesy names, childhood names and generational characters belong in aliases rather " +
				"than this element.",
			SecondNameDescription:
				"Choose the patrilineal family name. It is spoken and written before the personal name and is inherited by " +
				"children.",
			ThirdNameDescription: "",
			MinimumSecondNameCount: 1,
			MinimumThirdNameCount: 0,
			BloodModel: "B Dominant",
			SkinProfile: "golden_skin",
			HairProfile: "black_brown_grey_hair",
			EyeProfile: "brown_green_eyes",
			MaleGivenNames: new[]
			{
				"An", "Bao", "Bin", "Bo", "Chang", "Cheng",
				"Chong", "Da", "De", "Ding", "Dong", "Fang",
				"Fu", "Gang", "Gao", "Gong", "Guang", "Gui",
				"Guo", "Hai", "Han", "Hao", "He", "Hong",
				"Hu", "Hua", "Huan", "Hui", "Jian", "Jie",
				"Jin", "Jun", "Kang", "Liang", "Lin", "Long",
				"Ming", "Ning", "Peng", "Ping", "Qiang", "Qing",
				"Rui", "Sheng", "Tao", "Wei", "Wen", "Xiang",
				"Xing", "Yong"
			},
			FemaleGivenNames: new[]
			{
				"Ai", "Bao", "Cai", "Chan", "Chun", "Cui",
				"Dan", "E", "Fang", "Fen", "Gui", "He",
				"Hong", "Hua", "Huan", "Hui", "Jia", "Jiao",
				"Jing", "Juan", "Lan", "Lian", "Ling", "Mei",
				"Min", "Na", "Ning", "Pei", "Qiao", "Qing",
				"Rong", "Ru", "Shan", "Shu", "Su", "Ting",
				"Wan", "Xi", "Xia", "Xian", "Xiang", "Xiu",
				"Xue", "Yan", "Ying", "Yu", "Yue", "Yun",
				"Zhen", "Zhu"
			},
			MaleSecondNames: new[]
			{
				"Zhu", "Wang", "Li", "Zhang", "Liu", "Chen",
				"Yang", "Huang", "Zhao", "Wu", "Zhou", "Xu",
				"Sun", "Ma", "Hu", "Guo", "Lin", "He",
				"Gao", "Liang", "Zheng", "Luo", "Song", "Xie",
				"Han", "Tang", "Feng", "Yu", "Dong", "Xiao",
				"Cheng", "Cao", "Yuan", "Deng", "Xue", "Fu",
				"Shen", "Zeng", "Peng", "Lu"
			},
			FemaleSecondNames: new[]
			{
				"Zhu", "Wang", "Li", "Zhang", "Liu", "Chen",
				"Yang", "Huang", "Zhao", "Wu", "Zhou", "Xu",
				"Sun", "Ma", "Hu", "Guo", "Lin", "He",
				"Gao", "Liang", "Zheng", "Luo", "Song", "Xie",
				"Han", "Tang", "Feng", "Yu", "Dong", "Xiao",
				"Cheng", "Cao", "Yuan", "Deng", "Xue", "Fu",
				"Shen", "Zeng", "Peng", "Lu"
			},
			ThirdNames: Array.Empty<string>()
		),
		new(
			Key: "Joseon Korea",
			NameCultureName: "Renaissance Joseon Korean",
			EthnicityName: "Joseon Korean",
			EthnicGroup: "East Asian",
			EthnicSubgroup: "Joseon Korea",
			CultureName: "Joseon Korean (c. 1450)",
			NameForm: RenaissanceWorldNameForm.FamilyGiven,
			NameRegex: @"^(?<surname>[\w'-]{1,}) (?<birthname>[\w'-]{1,})$",
			ReferenceAnchor: "c. 1450",
			EthnicityDescription:
				"The Koreans are the Korean-speaking people of the peninsula, sharing ancestral rites, a courtly literary " +
				"tradition and a strong memory of descent. Family name and bon-gwan, the ancestral seat of a patrilineal " +
				"clan, place each person within a wider genealogy, while province, village and status group shape daily " +
				"custom.",
			CultureDescription:
				"Early Joseon culture joins aristocratic and official households, county administration, farming villages, " +
				"Buddhist monasteries, Confucian schools, craft communities and markets. Lineage standing, landholding, " +
				"examination learning, corvee and service to court or local office determine much of public life.",
			PersonalNameDescription:
				"Choose the personal name that follows the family name. One- and two-syllable forms are written as a " +
				"single token; courtesy names, childhood names and official titles are separate names.",
			SecondNameDescription:
				"Choose the family name. It comes first and belongs to a patrilineal clan whose fuller identity also " +
				"includes an ancestral seat, or bon-gwan, not shown in this compact form.",
			ThirdNameDescription: "",
			MinimumSecondNameCount: 1,
			MinimumThirdNameCount: 0,
			BloodModel: "B Dominant",
			SkinProfile: "golden_skin",
			HairProfile: "black_brown_grey_hair",
			EyeProfile: "brown_green_eyes",
			MaleGivenNames: new[]
			{
				"Bang-won", "Do-jeon", "Jeong", "Se-jong", "Mun-jong", "Dan-jong",
				"Se-jo", "Ye-jong", "Seong-jong", "Yeon-san", "Jung-jong", "In-jong",
				"Myeong-jong", "Seon-jo", "Gwang-hae", "In-jo", "Hyo-jong", "Hyeon-jong",
				"Suk-jong", "Yeong-jo", "Jeong-jo", "Tae-jong", "Tae-jo", "Ji-won",
				"Gyeong-seok", "Seong-gye", "Jong-seo", "Yu-jeong", "Hwang-hui", "Maeng-sa-seong",
				"Sin-suk-ju", "Han-myeong-hoe", "Kim-jong-seo", "Yi-i", "Yi-hwang", "Jeong-cheol",
				"Seo-gyeong-deok", "Jo-gwang-jo", "Nam-i", "Choe-mu-seon", "Jang-yeong-sil", "Bak-yeon",
				"Seong-sam-mun", "Pak-paeng-nyeon", "Ha-wi-ji", "Yu-seong-won", "Yi-gae", "Yu-eung-bu",
				"Kim-si-seup", "Gang-hui-maeng"
			},
			FemaleGivenNames: new[]
			{
				"Jeong-hui", "So-heon", "Hyeon-deok", "Jeong-sun", "Jeong-hyeon", "Gong-hye",
				"In-su", "Jang-geum", "Shin-sa-im-dang", "Hwang-jin-i", "Eo-u-dong", "Nanseol-heon",
				"Gwi-in", "Suk-ui", "So-ui", "Suk-won", "Sang-gung", "Bok-sil",
				"Deok-jung", "Gyeong-bin", "Hui-bin", "In-bin", "Jeong-bin", "Suk-bin",
				"Yeong-bin", "Myeong-bin", "Won-bin", "Ui-bin", "Hwa-bin", "Su-bin",
				"Sun-bin", "An-bin", "Gyeong-hye", "Gyeong-sun", "Gyeong-shin", "Hyo-ryeong",
				"Hyo-jeong", "Hyo-sun", "Hyo-hye", "Hyo-ui", "Myeong-suk", "Myeong-hye",
				"Myeong-an", "Jeong-myeong", "Yeong-hye", "Sook-myeong", "Sook-an", "Sook-hwi",
				"Sook-gyeong", "Sook-jeong"
			},
			MaleSecondNames: new[]
			{
				"Yi", "Kim", "Pak", "Choe", "Jeong", "Gang",
				"Jo", "Yun", "Jang", "Im", "Han", "O",
				"Seo", "Sin", "Gwon", "Hwang", "An", "Song",
				"Ryu", "Hong", "Jeon", "Go", "Mun", "Yang",
				"Son", "Bae", "Baek", "Heo", "Nam", "Sim",
				"No", "Ha", "Gwak", "Seong", "Cha", "Ju",
				"Woo", "Gu", "Min", "Jin"
			},
			FemaleSecondNames: new[]
			{
				"Yi", "Kim", "Pak", "Choe", "Jeong", "Gang",
				"Jo", "Yun", "Jang", "Im", "Han", "O",
				"Seo", "Sin", "Gwon", "Hwang", "An", "Song",
				"Ryu", "Hong", "Jeon", "Go", "Mun", "Yang",
				"Son", "Bae", "Baek", "Heo", "Nam", "Sim",
				"No", "Ha", "Gwak", "Seong", "Cha", "Ju",
				"Woo", "Gu", "Min", "Jin"
			},
			ThirdNames: Array.Empty<string>()
		),
		new(
			Key: "Muromachi Japan",
			NameCultureName: "Renaissance Muromachi Japanese",
			EthnicityName: "Muromachi Japanese",
			EthnicGroup: "East Asian",
			EthnicSubgroup: "Muromachi Japan",
			CultureName: "Muromachi Japanese (c. 1450)",
			NameForm: RenaissanceWorldNameForm.FamilyGiven,
			NameRegex: @"^(?<surname>[\w'-]{1,}) (?<birthname>[\w'-]{1,})$",
			ReferenceAnchor: "c. 1450",
			EthnicityDescription:
				"The Japanese are the Japanese-speaking people of the islands ruled in name by the emperor and governed " +
				"through court, shogunate, temples and provincial houses. Descent from a recognised clan or local house, " +
				"attachment to an estate or province, and participation in shrine and Buddhist institutions define " +
				"belonging. Court rank, warrior service and village community create sharply different ways of expressing " +
				"the same broader identity.",
			CultureDescription:
				"Muromachi culture spans Kyoto court life, warrior houses, provincial estates, temples, shrines, ports, " +
				"merchant quarters and farming villages. House affiliation, rank, patronage, religious institution and " +
				"local obligation govern conduct, while names and public identity may change with age or office.",
			PersonalNameDescription:
				"Choose the adult personal name that follows the house name. Childhood names, court names, Buddhist names " +
				"and later name changes are separate identities and are not combined here.",
			SecondNameDescription:
				"Choose the clan, house or family name. It precedes the personal name and marks the political and " +
				"genealogical household to which the character belongs.",
			ThirdNameDescription: "",
			MinimumSecondNameCount: 1,
			MinimumThirdNameCount: 0,
			BloodModel: "B Dominant",
			SkinProfile: "golden_skin",
			HairProfile: "black_brown_grey_hair",
			EyeProfile: "brown_green_eyes",
			MaleGivenNames: new[]
			{
				"Yoshimitsu", "Yoshimochi", "Yoshikazu", "Yoshinori", "Yoshikatsu", "Yoshimasa",
				"Yoshihisa", "Yoshitane", "Yoshizumi", "Yoshiharu", "Yoshiaki", "Takauji",
				"Tadayoshi", "Yoshiakira", "Ujimitsu", "Mitsukane", "Mochiuji", "Shigeuji",
				"Masatomo", "Noritada", "Norizane", "Sadamasa", "Mochitomo", "Motouji",
				"Kaneyoshi", "Moronao", "Moroyasu", "Yoriyuki", "Mitsumoto", "Katsumoto",
				"Sozen", "Mochitoyo", "Masanaga", "Yoshikado", "Tsuneyori", "Soseki",
				"Sesshu", "Zeami", "Motomasa", "Kanami", "Yoshimoto", "Norimasa",
				"Harumoto", "Motonaga", "Nagao", "Kageharu", "Dosen", "Shingen",
				"Nobuhide", "Motonari"
			},
			FemaleGivenNames: new[]
			{
				"Tomiko", "Hino", "Takatsukasa", "Kishi", "Yoshiko", "Satoko",
				"Haruko", "Masako", "Motoko", "Fusako", "Noriko", "Tsuneko",
				"Tokuko", "Tamako", "Yasuko", "Shigeko", "Kuniko", "Hideko",
				"Chikako", "Akiko", "Asako", "Ayako", "Fujiko", "Hisako",
				"Hiroko", "Ieko", "Imako", "Kameko", "Kaneko", "Kiyoko",
				"Matsuko", "Naoko", "Nene", "Oichi", "Okuni", "Omatsu",
				"Otatsu", "Otsuya", "Renko", "Ruriko", "Sakiko", "Sen",
				"Toku", "Tora", "Tsuru", "Ume", "Yodo", "Yuri",
				"Zenko", "Oeyo"
			},
			MaleSecondNames: new[]
			{
				"Ashikaga", "Hosokawa", "Yamana", "Hatakeyama", "Shiba", "Uesugi",
				"Ouchi", "Akamatsu", "Rokkaku", "Kyogoku", "Toki", "Imagawa",
				"Takeda", "Satake", "Shimazu", "Otomo", "Kikuchi", "Mori",
				"Amago", "Kono", "Chosokabe", "Date", "Nanbu", "Ogasawara",
				"Kira", "Kitabatake", "Nitta", "Sasaki", "Nikaido", "Isshiki",
				"Kusunoki", "Matsunaga", "Saito", "Asakura", "Azai", "Hino",
				"Konoe", "Takatsukasa", "Ichijo", "Nijo"
			},
			FemaleSecondNames: new[]
			{
				"Ashikaga", "Hosokawa", "Yamana", "Hatakeyama", "Shiba", "Uesugi",
				"Ouchi", "Akamatsu", "Rokkaku", "Kyogoku", "Toki", "Imagawa",
				"Takeda", "Satake", "Shimazu", "Otomo", "Kikuchi", "Mori",
				"Amago", "Kono", "Chosokabe", "Date", "Nanbu", "Ogasawara",
				"Kira", "Kitabatake", "Nitta", "Sasaki", "Nikaido", "Isshiki",
				"Kusunoki", "Matsunaga", "Saito", "Asakura", "Azai", "Hino",
				"Konoe", "Takatsukasa", "Ichijo", "Nijo"
			},
			ThirdNames: Array.Empty<string>()
		),
		new(
			Key: "Tibetan Plateau",
			NameCultureName: "Renaissance Tibetan",
			EthnicityName: "Late Medieval Tibetan",
			EthnicGroup: "Tibeto-Burman",
			EthnicSubgroup: "Tibetan Plateau",
			CultureName: "Tibetan Monastic and Estate (c. 1450)",
			NameForm: RenaissanceWorldNameForm.GivenOnly,
			NameRegex: @"^(?<birthname>[\w']+(?:-[\w']+)?)$",
			ReferenceAnchor: "c. 1450",
			EthnicityDescription:
				"The Tibetans are a Tibetan-speaking people of the plateau and the high Himalayan valleys. They recognise " +
				"belonging through language, valley or pastoral district, clan and estate, monastery or religious school, " +
				"and devotion to local protectors and Buddhist lineages. Aristocratic, monastic, farming and nomadic " +
				"communities differ in rank and custom but share a distinct plateau civilisation.",
			CultureDescription:
				"Tibetan culture combines monasteries, aristocratic estates, farming valleys, pastoral camps, pilgrimage " +
				"routes and competing regional courts. Religious school, teacher-disciple lineage, estate obligation, clan " +
				"and control of herds or land shape a person's position.",
			PersonalNameDescription:
				"Choose a Tibetan personal name of one or two syllabic elements. The elements are joined with a hyphen for " +
				"parsing; hereditary surnames are not normally required, and religious or honorific names may replace an " +
				"earlier name.",
			SecondNameDescription: "",
			ThirdNameDescription: "",
			MinimumSecondNameCount: 0,
			MinimumThirdNameCount: 0,
			BloodModel: "B Dominant",
			SkinProfile: "golden_skin",
			HairProfile: "black_brown_grey_hair",
			EyeProfile: "brown_green_eyes",
			MaleGivenNames: new[]
			{
				"Tashi", "Dorje", "Sonam", "Namgyal", "Tenzin", "Lobsang",
				"Jampa", "Jigme", "Pema", "Karma", "Kunga", "Chokyi",
				"Tsangyang", "Ngawang", "Yeshe", "Gendun", "Sangye", "Rinchen",
				"Dawa", "Norbu", "Lhundrup", "Lodro", "Thubten", "Phuntsok",
				"Palden", "Gyatso", "Trinley", "Wangchuk", "Drakpa", "Sherab",
				"Nyima", "Samten", "Jamyang", "Senge", "Zangpo", "Khyentse",
				"Rabten", "Dondrub", "Topgyal", "Tsering", "Dekyi", "Gyaltsen",
				"Chophel", "Woeser", "Tsewang", "Tenpa", "Dhondup", "Yonten",
				"Namkha", "Tsultrim"
			},
			FemaleGivenNames: new[]
			{
				"Dolma", "Yangchen", "Lhamo", "Drolma", "Paldron", "Tsering",
				"Dekyi", "Pema", "Sonam", "Kelsang", "Choedon", "Choden",
				"Dawa", "Nyima", "Tenzin", "Lhakyi", "Lhatso", "Lhundrup",
				"Metok", "Norzin", "Palmo", "Rinchen", "Sangmo", "Tashi",
				"Tsewang", "Wangmo", "Yeshi", "Yudron", "Zangmo", "Kunga",
				"Jamyang", "Sherab", "Chokyi", "Karma", "Lobsang", "Ngawang",
				"Peldon", "Samten", "Tsomo", "Yangzom", "Yeshidron", "Dechen",
				"Diki", "Gyalyum", "Khandro", "Namdrol", "Rigzin", "Seldon",
				"Tsendon", "Woesel"
			},
			MaleSecondNames: Array.Empty<string>(),
			FemaleSecondNames: Array.Empty<string>(),
			ThirdNames: Array.Empty<string>()
		),
		new(
			Key: "Dai Viet",
			NameCultureName: "Renaissance Dai Viet",
			EthnicityName: "Late Medieval Viet",
			EthnicGroup: "Southeast Asian",
			EthnicSubgroup: "Dai Viet",
			CultureName: "Dai Viet (c. 1450)",
			NameForm: RenaissanceWorldNameForm.FamilyGiven,
			NameRegex: @"^(?<surname>[\w'-]{1,}) (?<birthname>[\w'-]{1,})$",
			ReferenceAnchor: "c. 1450",
			EthnicityDescription:
				"The Viet are the Vietnamese-speaking lowland people of the Red River delta and the cultivated valleys " +
				"extending southward. Patrilineal family, ancestral village, local cults and the written traditions of " +
				"court and examination learning define their common identity. Frontier settlement and contact with Cham " +
				"and upland peoples give strong regional differences without obscuring a shared Viet language and " +
				"political memory.",
			CultureDescription:
				"The culture of Dai Viet is centred on wet-rice villages, lineage halls, markets, Buddhist and local " +
				"temples, scholar-official households and the institutions of the Le court. Ancestral rites, communal " +
				"land, examination learning, military service and village obligation order social life.",
			PersonalNameDescription:
				"Choose the personal name that follows the family name. Middle and generational names, courtesy names and " +
				"temple names are omitted from this compact form.",
			SecondNameDescription: "Choose the patrilineal family name. It is inherited and placed before the personal name.",
			ThirdNameDescription: "",
			MinimumSecondNameCount: 1,
			MinimumThirdNameCount: 0,
			BloodModel: "B Dominant",
			SkinProfile: "golden_skin",
			HairProfile: "black_brown_grey_hair",
			EyeProfile: "brown_green_eyes",
			MaleGivenNames: new[]
			{
				"Anh", "Bao", "Binh", "Canh", "Chieu", "Cong",
				"Cuong", "Dai", "Dao", "Dat", "Dinh", "Dong",
				"Duc", "Duong", "Gia", "Giang", "Hai", "Hieu",
				"Hoang", "Hung", "Huy", "Khang", "Khiem", "Khoi",
				"Kien", "Lam", "Long", "Minh", "Nam", "Nghia",
				"Nhan", "Phong", "Phu", "Phuc", "Quang", "Quoc",
				"Son", "Tai", "Tam", "Thanh", "Thien", "Thinh",
				"Tho", "Tien", "Tri", "Trung", "Tuan", "Van",
				"Viet", "Vinh"
			},
			FemaleGivenNames: new[]
			{
				"An", "Anh", "Bich", "Chau", "Chi", "Cuc",
				"Dao", "Diep", "Dung", "Giang", "Ha", "Hanh",
				"Hoa", "Hong", "Hue", "Huong", "Lan", "Lien",
				"Linh", "Loan", "Mai", "Minh", "My", "Nga",
				"Ngoc", "Nhu", "Oanh", "Phuong", "Quynh", "Sen",
				"Suong", "Tam", "Thanh", "Thao", "Thi", "Thu",
				"Thuy", "Tien", "Tram", "Trinh", "Tuyet", "Uyen",
				"Van", "Vien", "Xuan", "Yen", "Dieu", "Kim",
				"Kieu", "Ly"
			},
			MaleSecondNames: new[]
			{
				"Nguyen", "Tran", "Le", "Pham", "Hoang", "Huynh",
				"Phan", "Vu", "Vo", "Dang", "Bui", "Do",
				"Ho", "Ngo", "Duong", "Ly", "Dinh", "Trinh",
				"Mai", "Cao", "Luong", "Luu", "Ta", "Tong",
				"Chu", "Thai", "Kieu", "La", "Quach", "Dam"
			},
			FemaleSecondNames: new[]
			{
				"Nguyen", "Tran", "Le", "Pham", "Hoang", "Huynh",
				"Phan", "Vu", "Vo", "Dang", "Bui", "Do",
				"Ho", "Ngo", "Duong", "Ly", "Dinh", "Trinh",
				"Mai", "Cao", "Luong", "Luu", "Ta", "Tong",
				"Chu", "Thai", "Kieu", "La", "Quach", "Dam"
			},
			ThirdNames: Array.Empty<string>()
		),
		new(
			Key: "Ayutthaya",
			NameCultureName: "Renaissance Ayutthaya Thai",
			EthnicityName: "Late Medieval Tai",
			EthnicGroup: "Southeast Asian",
			EthnicSubgroup: "Ayutthaya Kingdom",
			CultureName: "Ayutthaya Tai (c. 1450)",
			NameForm: RenaissanceWorldNameForm.GivenOnly,
			NameRegex: @"^(?<birthname>[\w'-]{2,})$",
			ReferenceAnchor: "c. 1450",
			EthnicityDescription:
				"The Tai are a Tai-speaking people of the Chao Phraya basin and neighbouring river valleys. They define " +
				"belonging through language, settlement and irrigation community, kin and patronage, Buddhist monastery, " +
				"and service to a local lord or the Ayutthaya court. Court titles and status groups are important, but " +
				"ordinary people are chiefly known by personal name and home community.",
			CultureDescription:
				"Ayutthayan culture grows around riverine towns, rice villages, Buddhist monasteries, royal service, " +
				"markets and maritime trade. Patron-client ties, corvee, rank, temple patronage and control of waterways " +
				"connect court, town and countryside.",
			PersonalNameDescription:
				"Choose a single Tai personal name. Hereditary surnames are not used; royal names, ranks, monastic names " +
				"and honorifics should be recorded separately.",
			SecondNameDescription: "",
			ThirdNameDescription: "",
			MinimumSecondNameCount: 0,
			MinimumThirdNameCount: 0,
			BloodModel: "B Dominant",
			SkinProfile: "golden_skin",
			HairProfile: "black_brown_grey_hair",
			EyeProfile: "brown_green_eyes",
			MaleGivenNames: new[]
			{
				"Anurak", "Arthit", "Borommaracha", "Borommatrailok", "Bunma", "Chai",
				"Chaiya", "Chaloem", "Chao-Fa", "Chaturong", "Chayaphon", "Chinnarat",
				"Damrong", "Decha", "Ekathotsarot", "Intharacha", "Jakkraphat", "Khamhaeng",
				"Kiet", "Kosum", "Krai", "Maha-Thammaracha", "Mahin", "Mongkut",
				"Naresuan", "Narai", "Narin", "Noppharat", "Ong", "Pha-Mueang",
				"Phichai", "Phra-Ruang", "Phrom", "Ramesuan", "Ramathibodi", "Rattanakosin",
				"Ratchathirat", "Sakkarin", "Sanphet", "Si-Inthrathit", "Songtham", "Sorasak",
				"Sri-Sudachan", "Suriyothai", "Thammaracha", "Thonglan", "Trailok", "U-Thong",
				"Wichai", "Yot-Fa"
			},
			FemaleGivenNames: new[]
			{
				"Achara", "Ambara", "Anong", "Arun", "Bencha", "Boonmee",
				"Bua", "Busaba", "Chan", "Chandra", "Chao-Sri", "Chomphu",
				"Dara", "Dokmai", "Duang", "Fa", "Kaeo", "Kalyani",
				"Kamala", "Kanda", "Kannika", "Kesara", "Kham", "Kulap",
				"Lada", "Lamduan", "Mali", "Manora", "Mekhala", "Montha",
				"Nang-Klao", "Naree", "Nuan", "Pensri", "Phailin", "Phim",
				"Phloi", "Pikul", "Prang", "Ratana", "Ratri", "Saeng",
				"Samorn", "Sirikit", "Sri", "Sudachan", "Suriyothai", "Thip",
				"Ubon", "Wanthong"
			},
			MaleSecondNames: Array.Empty<string>(),
			FemaleSecondNames: Array.Empty<string>(),
			ThirdNames: Array.Empty<string>()
		),
		new(
			Key: "Majapahit Java",
			NameCultureName: "Renaissance Majapahit Javanese",
			EthnicityName: "Late Majapahit Javanese",
			EthnicGroup: "Southeast Asian",
			EthnicSubgroup: "Majapahit Java",
			CultureName: "Majapahit Javanese (c. 1450)",
			NameForm: RenaissanceWorldNameForm.GivenOnly,
			NameRegex: @"^(?<birthname>[\w'-]{2,})$",
			ReferenceAnchor: "c. 1450",
			EthnicityDescription:
				"The Javanese are the Javanese-speaking people of the fertile interior and the ports of Java. Village and " +
				"irrigation community, kin group, court allegiance, religious foundation and the layered etiquette of rank " +
				"are central to their identity. Old Javanese literary culture and the prestige of Majapahit link farmers, " +
				"artisans, priests, merchants and nobles across the island.",
			CultureDescription:
				"Late Majapahit culture joins rice-growing villages, temple lands, court compounds, craft settlements and " +
				"Indian Ocean ports. Rank, patronage, ritual obligation, control of water and land, and Hindu-Buddhist or " +
				"emerging Muslim networks shape public life.",
			PersonalNameDescription:
				"Choose a single Javanese personal, praise or court name. Titles, rank words, childhood names and regnal " +
				"styles are not part of this element.",
			SecondNameDescription: "",
			ThirdNameDescription: "",
			MinimumSecondNameCount: 0,
			MinimumThirdNameCount: 0,
			BloodModel: "B Dominant",
			SkinProfile: "golden_skin",
			HairProfile: "black_brown_grey_hair",
			EyeProfile: "brown_green_eyes",
			MaleGivenNames: new[]
			{
				"Adityawarman", "Airlangga", "Anusapati", "Arya", "Bhre", "Cakradara",
				"Damarwulan", "Gajah-Mada", "Hayam-Wuruk", "Jayabaya", "Jayakatwang", "Jayanagara",
				"Kertabhumi", "Kertajaya", "Kertanegara", "Kertarajasa", "Kudamerta", "Kusumawardhana",
				"Mpu-Nala", "Nambi", "Nararya", "Panji", "Parameswara", "Raden",
				"Rajasa", "Ranggalawe", "Samarawijaya", "Sanjaya", "Sedah", "Singhawikramawardhana",
				"Smarawijaya", "Tribhuwana", "Udayana", "Udara", "Vijayakrama", "Vikramavardhana",
				"Wirabhumi", "Wiraraja", "Wringin-Anom", "Yudhanegara", "Anindita", "Brawijaya",
				"Dhananjaya", "Girindrawardhana", "Kebo-Anabrang", "Lembu-Tal", "Mpu-Tantular", "Rakai-Pikatan",
				"Sanggramawijaya", "Wijaya"
			},
			FemaleGivenNames: new[]
			{
				"Gayatri", "Tribhuwana", "Rajapatni", "Suhita", "Kusumawardhani", "Indudewi",
				"Jayendradewi", "Isyana", "Ken-Dedes", "Ken-Umang", "Mahendradatta", "Pramodhawardhani",
				"Sri-Suhita", "Dyah-Gitarja", "Dyah-Wiyat", "Dyah-Pitaloka", "Citraresmi", "Rara-Jonggrang",
				"Dewi-Sri", "Dewi-Sekartaji", "Dewi-Kilisuci", "Dewi-Ratna", "Dewi-Sasmita", "Dewi-Laksmi",
				"Dewi-Tara", "Dewi-Sinta", "Dewi-Kencana", "Dewi-Rengganis", "Dewi-Angreni", "Dewi-Nawang",
				"Sekar", "Puspa", "Ratih", "Sari", "Wulan", "Laras",
				"Ayu", "Raras", "Kencana", "Melati", "Cempaka", "Ningsih",
				"Pertiwi", "Saraswati", "Padmawati", "Candrakirana", "Kirana", "Rukmini",
				"Sulastri", "Utari"
			},
			MaleSecondNames: Array.Empty<string>(),
			FemaleSecondNames: Array.Empty<string>(),
			ThirdNames: Array.Empty<string>()
		),
		new(
			Key: "Malay Sultanates",
			NameCultureName: "Renaissance Malay",
			EthnicityName: "Late Medieval Malay",
			EthnicGroup: "Southeast Asian",
			EthnicSubgroup: "Malay Sultanates",
			CultureName: "Malay Sultanate (c. 1450)",
			NameForm: RenaissanceWorldNameForm.GivenPatronym,
			NameRegex: @"^(?<birthname>[\w'-]{2,}) (?<patronym>(?:bin|binti)-[\w'-]{2,})$",
			ReferenceAnchor: "c. 1450",
			EthnicityDescription:
				"The Malays are a Malay-speaking maritime people of the Straits and the river-mouth settlements of the " +
				"peninsula and Sumatra. They recognise one another through language, kin and village, allegiance to a " +
				"ruler, seafaring and trading custom, and increasingly through Islam and the institutions of the " +
				"sultanates. Older Indic court traditions remain visible in titles and ceremony.",
			CultureDescription:
				"Malay sultanate culture is centred on riverine courts, port towns, fishing and farming settlements, " +
				"merchant quarters and the sea lanes of Melaka. Royal service, Islamic learning, patronage, navigation and " +
				"exchange among Malay, Chinese, Javanese, Indian and Arab merchants shape social opportunity.",
			PersonalNameDescription:
				"Choose the person's own Malay or Islamicate name. Muhammad, Abdul- and other devotional compounds remain " +
				"part of the personal name and are entered as one token.",
			SecondNameDescription:
				"Enter bin-Father for a man or binti-Father for a woman. This is a patronym naming the father, not a " +
				"hereditary family surname, and it is retained after marriage.",
			ThirdNameDescription: "",
			MinimumSecondNameCount: 1,
			MinimumThirdNameCount: 0,
			BloodModel: "B Dominant",
			SkinProfile: "golden_skin",
			HairProfile: "black_brown_grey_hair",
			EyeProfile: "brown_green_eyes",
			MaleGivenNames: new[]
			{
				"Abdallah", "Abdul-Jalil", "Abdul-Rahman", "Ahmad", "Alauddin", "Ali",
				"Daud", "Ibrahim", "Iskandar", "Jalaluddin", "Kamaluddin", "Mahmud",
				"Malik", "Mansur", "Megat", "Muhammad", "Muzaffar", "Nasiruddin",
				"Raja", "Raden", "Rahmat", "Sulaiman", "Syah", "Yusuf",
				"Zainal", "Adityawarman", "Badrul", "Hamzah", "Hasan", "Husain",
				"Ismail", "Jafar", "Khalid", "Mansur-Shah", "Marhum", "Mudzafar",
				"Parameswara", "Qasim", "Raden-Galuh", "Saifuddin", "Sang-Nila", "Seri",
				"Tajuddin", "Tun-Ali", "Tun-Mutahir", "Umar", "Wan-Ahmad", "Wira",
				"Zainuddin", "Zulkifli"
			},
			FemaleGivenNames: new[]
			{
				"Aminah", "Aisha", "Azizah", "Badriah", "Fatimah", "Halimah",
				"Hamidah", "Hasnah", "Intan", "Jamaliah", "Khadijah", "Khatijah",
				"Laila", "Mahsuri", "Mariam", "Mastura", "Melati", "Mutiara",
				"Noriah", "Nur", "Puteh", "Puteri", "Raja-Hijau", "Raja-Ungu",
				"Ratna", "Rukiah", "Salmah", "Sari", "Sharifah", "Siti",
				"Sofiah", "Suriani", "Tun-Fatimah", "Wan-Kembang", "Zainab", "Zaleha",
				"Zubaidah", "Cempaka", "Dara", "Dewi", "Embun", "Kemala",
				"Kencana", "Kirana", "Mayang", "Nila", "Purnama", "Seri",
				"Wulan", "Zaharah"
			},
			MaleSecondNames: new[]
			{
				"bin-Abdallah", "bin-Ahmad", "bin-Ali", "bin-Daud", "bin-Hasan", "bin-Husain",
				"bin-Ibrahim", "bin-Iskandar", "bin-Ismail", "bin-Jafar", "bin-Jalaluddin", "bin-Khalid",
				"bin-Mahmud", "bin-Malik", "bin-Mansur", "bin-Muhammad", "bin-Muzaffar", "bin-Qasim",
				"bin-Sulaiman", "bin-Umar", "bin-Yusuf", "bin-Zainal", "bin-Hamzah", "bin-Rahmat",
				"bin-Tajuddin"
			},
			FemaleSecondNames: new[]
			{
				"binti-Abdallah", "binti-Ahmad", "binti-Ali", "binti-Daud", "binti-Hasan", "binti-Husain",
				"binti-Ibrahim", "binti-Iskandar", "binti-Ismail", "binti-Jafar", "binti-Jalaluddin", "binti-Khalid",
				"binti-Mahmud", "binti-Malik", "binti-Mansur", "binti-Muhammad", "binti-Muzaffar", "binti-Qasim",
				"binti-Sulaiman", "binti-Umar", "binti-Yusuf", "binti-Zainal", "binti-Hamzah", "binti-Rahmat",
				"binti-Tajuddin"
			},
			ThirdNames: Array.Empty<string>()
		),
		new(
			Key: "Ethiopian Highlands",
			NameCultureName: "Renaissance Ethiopian Highland",
			EthnicityName: "Late Medieval Amhara",
			EthnicGroup: "Horn of Africa",
			EthnicSubgroup: "Ethiopian Highlands",
			CultureName: "Solomonic Ethiopian Highland (c. 1450)",
			NameForm: RenaissanceWorldNameForm.GivenPatronym,
			NameRegex: @"^(?<birthname>[\w'-]{2,}) (?<patronym>[\w'-]{2,})$",
			ReferenceAnchor: "c. 1450",
			EthnicityDescription:
				"The Amhara are a Christian highland people of the Solomonic realm, speaking Amharic and sharing the " +
				"liturgical world of Ge'ez and the Ethiopian Orthodox Church. They define belonging through paternal " +
				"descent, parish and monastery, district and noble allegiance, and the memory of a sacred monarchy " +
				"descended from ancient kings. Farming estates, military service and church festivals bind household life " +
				"to the wider kingdom.",
			CultureDescription:
				"Solomonic highland culture encompasses plough agriculture, noble and military households, monasteries, " +
				"churches, market settlements and itinerant royal camps. Orthodox fasting and feasting, land grants, " +
				"tribute, patronage and service to local or imperial lords order the year.",
			PersonalNameDescription:
				"Choose the person's own baptismal or everyday name. Ge'ez and Amharic devotional compounds are entered as " +
				"one hyphenated token.",
			SecondNameDescription:
				"Choose the father's personal name. Ethiopians do not inherit a fixed family surname: a person is known by " +
				"personal name followed by the father's name, and sometimes the grandfather's name in more formal records.",
			ThirdNameDescription: "",
			MinimumSecondNameCount: 1,
			MinimumThirdNameCount: 0,
			BloodModel: "Majority O",
			SkinProfile: "swarthy_skin",
			HairProfile: "black_brown_grey_hair",
			EyeProfile: "brown_green_eyes",
			MaleGivenNames: new[]
			{
				"Abba-Selama", "Amda-Seyon", "Asfa", "Baeda-Maryam", "Dawit", "Eskender",
				"Gabra-Masqal", "Gabra-Maryam", "Gelawdewos", "Iyasu", "Lebna-Dengel", "Minas",
				"Naod", "Newaya-Krestos", "Newaya-Maryam", "Seyfe-Arad", "Tewodros", "Yagbeu-Seyon",
				"Yeshaq", "Zara-Yaqob", "Abreha", "Atsbeha", "Bazen", "Dil-Naod",
				"Gebre-Mesqel", "Harbe", "Lalibela", "Neakuto-Leab", "Tatadim", "Yemrehana-Krestos",
				"Andreyas", "Beshah", "Fasil", "Habte", "Kidane", "Makonnen",
				"Mamo", "Matyas", "Mikael", "Negasi", "Rufael", "Sahle",
				"Samuel", "Sisay", "Tamrat", "Tekle", "Tesfa", "Wolde",
				"Yohannes", "Zena"
			},
			FemaleGivenNames: new[]
			{
				"Eleni", "Zion-Mogesa", "Seble-Wongel", "Romna", "Del-Wambara", "Mentewab",
				"Tiruwork", "Sabela", "Makeda", "Masqal-Kebra", "Mariam-Sena", "Walatta-Petros",
				"Walatta-Mikael", "Walatta-Gabriel", "Walatta-Maryam", "Walatta-Seyon", "Hirut", "Krestos-Semra",
				"Egzi-Haraya", "Emebet", "Almaz", "Askale", "Atsede", "Ayana",
				"Bezawit", "Desta", "Eden", "Feven", "Genet", "Hanna",
				"Hiwot", "Kalkidan", "Kidist", "Lemlem", "Mahlet", "Mastewal",
				"Meseret", "Rahel", "Roman", "Ruth", "Sara", "Selam",
				"Senait", "Siham", "Tigist", "Tsion", "Woineshet", "Yordanos",
				"Zewditu", "Aster"
			},
			MaleSecondNames: new[]
			{
				"Amda-Seyon", "Asfa", "Baeda-Maryam", "Dawit", "Eskender", "Gabra-Masqal",
				"Gabra-Maryam", "Gelawdewos", "Iyasu", "Lebna-Dengel", "Minas", "Naod",
				"Newaya-Krestos", "Newaya-Maryam", "Seyfe-Arad", "Tewodros", "Yagbeu-Seyon", "Yeshaq",
				"Zara-Yaqob", "Abreha", "Atsbeha", "Bazen", "Dil-Naod", "Gebre-Mesqel",
				"Harbe", "Lalibela", "Neakuto-Leab", "Tatadim", "Yemrehana-Krestos", "Andreyas",
				"Beshah", "Fasil", "Habte", "Kidane", "Makonnen", "Mamo",
				"Matyas", "Mikael", "Negasi", "Rufael", "Sahle", "Samuel",
				"Sisay", "Tamrat", "Tekle", "Tesfa", "Wolde", "Yohannes",
				"Zena", "Petros"
			},
			FemaleSecondNames: new[]
			{
				"Amda-Seyon", "Asfa", "Baeda-Maryam", "Dawit", "Eskender", "Gabra-Masqal",
				"Gabra-Maryam", "Gelawdewos", "Iyasu", "Lebna-Dengel", "Minas", "Naod",
				"Newaya-Krestos", "Newaya-Maryam", "Seyfe-Arad", "Tewodros", "Yagbeu-Seyon", "Yeshaq",
				"Zara-Yaqob", "Abreha", "Atsbeha", "Bazen", "Dil-Naod", "Gebre-Mesqel",
				"Harbe", "Lalibela", "Neakuto-Leab", "Tatadim", "Yemrehana-Krestos", "Andreyas",
				"Beshah", "Fasil", "Habte", "Kidane", "Makonnen", "Mamo",
				"Matyas", "Mikael", "Negasi", "Rufael", "Sahle", "Samuel",
				"Sisay", "Tamrat", "Tekle", "Tesfa", "Wolde", "Yohannes",
				"Zena", "Petros"
			},
			ThirdNames: Array.Empty<string>()
		),
		new(
			Key: "Somali and Adal Sultanates",
			NameCultureName: "Renaissance Somali",
			EthnicityName: "Late Medieval Somali",
			EthnicGroup: "Horn of Africa",
			EthnicSubgroup: "Somali and Adal Sultanates",
			CultureName: "Somali-Adal (c. 1450)",
			NameForm: RenaissanceWorldNameForm.GivenPatronymClan,
			NameRegex: @"^(?<birthname>(?!(?:ibn|bint|ina)-)[\w'-]{2,}) (?<patronym>(?!(?:ibn|bint|ina)-)[\w'-]{2,}) (?<familygroupname>[\w'-]{2,})$",
			ReferenceAnchor: "c. 1450",
			EthnicityDescription:
				"The Somalis are a Cushitic-speaking people of the Horn, joined by language, pastoral and mercantile " +
				"custom, and long genealogies that place every household within clan and sub-clan. Lineage determines " +
				"alliance, compensation and protection, while Islam, saintly centres and the ports and towns of Adal " +
				"connect pastoral communities to the Red Sea world.",
			CultureDescription:
				"Somali and Adal culture links pastoral camps, caravan routes, wells, farming districts, ports, mosques " +
				"and fortified towns. Clan genealogy, diya-paying obligations, hospitality, livestock wealth, trade and " +
				"service to emirs or religious leaders organise public life.",
			PersonalNameDescription:
				"Choose the person's own Somali or Islamicate personal name. It is followed in formal identification by " +
				"the personal names of the father and earlier paternal ancestors.",
			SecondNameDescription:
				"Choose the father's personal name without ibn, bint or a hereditary surname. Somali lineage names are " +
				"successive personal names; the same form is used for men and women.",
			ThirdNameDescription:
				"Choose the clan or major lineage by which wider descent and political obligations are recognised. It " +
				"follows the personal and father's names in this compact gameplay form.",
			MinimumSecondNameCount: 1,
			MinimumThirdNameCount: 1,
			BloodModel: "Majority O",
			SkinProfile: "swarthy_skin",
			HairProfile: "black_brown_grey_hair",
			EyeProfile: "brown_green_eyes",
			MaleGivenNames: new[]
			{
				"Abadir", "Abdi", "Abdallah", "Abdirahman", "Abdisamad", "Abshir",
				"Adan", "Ahmad", "Ali", "Aw-Barkhadle", "Aw-Muhammad", "Barkhad",
				"Barre", "Bile", "Boqor", "Daud", "Dirir", "Farah",
				"Fiqi", "Gaal", "Garaad", "Guled", "Hasan", "Hussein",
				"Ibrahim", "Ismail", "Jama", "Jibril", "Liban", "Mahamud",
				"Mansur", "Muhammad", "Nur", "Omar", "Osman", "Qasim",
				"Roble", "Saad", "Samatar", "Shirwac", "Siyad", "Sugulle",
				"Sulayman", "Ugaas", "Warsame", "Weheliye", "Wiil-Waal", "Yusuf",
				"Zakariya", "Zubayr"
			},
			FemaleGivenNames: new[]
			{
				"Aamina", "Aisha", "Amran", "Anab", "Ardo", "Ayaan",
				"Barni", "Bashira", "Bilan", "Bisharo", "Canab", "Cawo",
				"Dahabo", "Deeqo", "Faduma", "Filsan", "Fowsiya", "Gobaad",
				"Haboon", "Halima", "Hawo", "Hibo", "Hodan", "Idil",
				"Ifrah", "Ilwaad", "Jamaad", "Khadija", "Ladan", "Leyla",
				"Luul", "Mako", "Maryan", "Muna", "Nasteexo", "Nimco",
				"Nura", "Qamar", "Rahma", "Rooda", "Ruqiya", "Sagal",
				"Sahra", "Shukri", "Sucaad", "Ubah", "Waris", "Yasmiin",
				"Zamzam", "Zaynab"
			},
			MaleSecondNames: new[]
			{
				"Abadir", "Abdi", "Abdallah", "Abdirahman", "Abdisamad", "Abshir",
				"Adan", "Ahmad", "Ali", "Barkhad", "Barre", "Bile",
				"Daud", "Dirir", "Farah", "Guled", "Hasan", "Hussein",
				"Ibrahim", "Ismail", "Jama", "Jibril", "Liban", "Mahamud",
				"Mansur", "Muhammad", "Nur", "Omar", "Osman", "Qasim",
				"Roble", "Saad", "Samatar", "Shirwac", "Siyad", "Sugulle",
				"Sulayman", "Warsame", "Weheliye", "Yusuf", "Zakariya", "Zubayr",
				"Bashir", "Hamza", "Khalid", "Malik", "Salih", "Sharif",
				"Yahya", "Yasin"
			},
			FemaleSecondNames: new[]
			{
				"Abadir", "Abdi", "Abdallah", "Abdirahman", "Abdisamad", "Abshir",
				"Adan", "Ahmad", "Ali", "Barkhad", "Barre", "Bile",
				"Daud", "Dirir", "Farah", "Guled", "Hasan", "Hussein",
				"Ibrahim", "Ismail", "Jama", "Jibril", "Liban", "Mahamud",
				"Mansur", "Muhammad", "Nur", "Omar", "Osman", "Qasim",
				"Roble", "Saad", "Samatar", "Shirwac", "Siyad", "Sugulle",
				"Sulayman", "Warsame", "Weheliye", "Yusuf", "Zakariya", "Zubayr",
				"Bashir", "Hamza", "Khalid", "Malik", "Salih", "Sharif",
				"Yahya", "Yasin"
			},
			ThirdNames: new[]
			{
				"Darod", "Dir", "Hawiye", "Isaaq", "Rahanweyn", "Gadabuursi",
				"Issa", "Marehan", "Ogaden", "Majeerteen", "Warsangeli", "Dhulbahante",
				"Habr-Awal", "Habr-Jeclo", "Habr-Yunis", "Abgaal", "Habar-Gidir", "Murusade",
				"Sheekhaal", "Ajuran", "Geri", "Degodia", "Garre", "Digil",
				"Mirifle"
			}
		),
		new(
			Key: "Swahili Coast City-States",
			NameCultureName: "Renaissance Swahili",
			EthnicityName: "Late Medieval Swahili",
			EthnicGroup: "East African",
			EthnicSubgroup: "Swahili Coast",
			CultureName: "Swahili City-State (c. 1450)",
			NameForm: RenaissanceWorldNameForm.GivenPatronymToponym,
			NameRegex: @"^(?<birthname>[\w'-]{2,}) (?<patronym>(?:bin|binti)-[\w'-]{2,}) (?<toponym>(?:al|wa)-[\w'-]{2,})$",
			ReferenceAnchor: "c. 1450",
			EthnicityDescription:
				"The Swahili are a Kiswahili-speaking Muslim people of the East African coast and islands. They define " +
				"themselves through coastal town and neighbourhood, local lineage, Islam, maritime trade and a shared " +
				"urban tradition expressed in coral houses, mosques and poetry. Ancestry from inland communities and " +
				"generations of Indian Ocean contact are woven into a distinctly local peoplehood.",
			CultureDescription:
				"Swahili coast culture is centred on coral-built towns, merchant houses, mosques, craft quarters, fishing " +
				"settlements, plantations and caravan links to the interior. Kinship, neighbourhood, Islamic learning, " +
				"seafaring reputation and hospitality govern status in a world of constant commercial travel.",
			PersonalNameDescription:
				"Choose a Swahili or Arabic-derived personal name. Local Bantu names and Islamicate names both occur, and " +
				"devotional compounds remain one token.",
			SecondNameDescription:
				"Enter bin-Father for a man or binti-Father for a woman. This names the father and is not a hereditary " +
				"surname.",
			ThirdNameDescription:
				"Choose a coast-town, island, lineage, overseas origin or profession in al- or wa- form, such as al-Kilwi, " +
				"wa-Shirazi or al-Tajiri. The whole identifier is one hyphenated token.",
			MinimumSecondNameCount: 1,
			MinimumThirdNameCount: 200,
			BloodModel: "Majority O",
			SkinProfile: "dark_skin",
			HairProfile: "black_brown_grey_hair",
			EyeProfile: "brown_green_eyes",
			MaleGivenNames: new[]
			{
				"Abdallah", "Abubakar", "Ahmad", "Ali", "Bakari", "Baraka",
				"Daudi", "Faraji", "Hamisi", "Hasan", "Hussein", "Ibrahim",
				"Idris", "Ilyas", "Ismail", "Jafari", "Juma", "Khamis",
				"Khalfan", "Khalid", "Mahmud", "Majid", "Makame", "Malik",
				"Mansur", "Masud", "Mbarak", "Mkwawa", "Muhammad", "Musa",
				"Mwinyi", "Nasir", "Omari", "Rashid", "Salim", "Said",
				"Salih", "Shomari", "Suleiman", "Tamim", "Umar", "Yahya",
				"Yusuf", "Zuberi", "Azizi", "Bwana", "Fadhil", "Hemed",
				"Rajab", "Sharif"
			},
			FemaleGivenNames: new[]
			{
				"Aisha", "Amina", "Ashura", "Aziza", "Bahati", "Baraka",
				"Bibi", "Fatuma", "Halima", "Hamida", "Hasina", "Jamila",
				"Khadija", "Latifa", "Leila", "Lulu", "Maimuna", "Malika",
				"Mariam", "Mwana", "Mwanaidi", "Mwanajuma", "Mwanaheri", "Mwanakhamis",
				"Mwanasha", "Mwanahamisi", "Nuru", "Rahma", "Rehema", "Ruqiya",
				"Saada", "Safiya", "Salama", "Salma", "Shani", "Sharifa",
				"Subira", "Tatu", "Yasmini", "Zahra", "Zainabu", "Zawadi",
				"Zubeda", "Asha", "Faiza", "Hawa", "Imani", "Najma",
				"Rukia", "Zuhura"
			},
			MaleSecondNames: new[]
			{
				"bin-Abdallah", "bin-Ahmad", "bin-Ali", "bin-Bakari", "bin-Daudi", "bin-Hasan",
				"bin-Hussein", "bin-Ibrahim", "bin-Ismail", "bin-Jafari", "bin-Juma", "bin-Khalfan",
				"bin-Mahmud", "bin-Malik", "bin-Muhammad", "bin-Musa", "bin-Nasir", "bin-Rashid",
				"bin-Said", "bin-Suleiman", "bin-Umar", "bin-Yahya", "bin-Yusuf", "bin-Baraka",
				"bin-Hamisi"
			},
			FemaleSecondNames: new[]
			{
				"binti-Abdallah", "binti-Ahmad", "binti-Ali", "binti-Bakari", "binti-Daudi", "binti-Hasan",
				"binti-Hussein", "binti-Ibrahim", "binti-Ismail", "binti-Jafari", "binti-Juma", "binti-Khalfan",
				"binti-Mahmud", "binti-Malik", "binti-Muhammad", "binti-Musa", "binti-Nasir", "binti-Rashid",
				"binti-Said", "binti-Suleiman", "binti-Umar", "binti-Yahya", "binti-Yusuf", "binti-Baraka",
				"binti-Hamisi"
			},
			ThirdNames: RenaissanceSwahiliIdentifierInventory
		),
		new(
			Key: "Mande and Songhai Sahel",
			NameCultureName: "Renaissance Mande Sahelian",
			EthnicityName: "Late Medieval Manding",
			EthnicGroup: "West African",
			EthnicSubgroup: "Western Sahel",
			CultureName: "Mande-Songhai Sahelian (c. 1450)",
			NameForm: RenaissanceWorldNameForm.GivenClan,
			NameRegex: @"^(?<birthname>[\w'-]{2,}) (?<familygroupname>[\w'-]{2,})$",
			ReferenceAnchor: "c. 1450",
			EthnicityDescription:
				"The Manding are a Mande-speaking people of the upper Niger and the lands shaped by Mali's royal and " +
				"trading networks. They recognise descent through clan names and remembered founders, while specialist " +
				"lineages of smiths, leatherworkers, praise-singers and other professions hold inherited social roles. " +
				"Islam, royal allegiance and older ritual traditions coexist within a strong oral memory of kin and place.",
			CultureDescription:
				"Manding and western Sahelian culture joins river-valley farming and fishing, trans-Saharan commerce, " +
				"cavalry courts, market towns, craft-specialist lineages, Islamic scholarship and oral historical " +
				"performance. Clan, occupation, patronage and allegiance to town or ruler are central sources of identity.",
			PersonalNameDescription:
				"Choose a Manding or Islamicate personal name. Both local forms and names carried through Islamic " +
				"scholarship and trade are common.",
			SecondNameDescription:
				"Choose the inherited clan or lineage name. It locates the character within a descent group and may also " +
				"signal a long-standing social or occupational tradition.",
			ThirdNameDescription: "",
			MinimumSecondNameCount: 1,
			MinimumThirdNameCount: 0,
			BloodModel: "Majority O",
			SkinProfile: "dark_skin",
			HairProfile: "black_brown_grey_hair",
			EyeProfile: "brown_green_eyes",
			MaleGivenNames: new[]
			{
				"Abubakar", "Ahmad", "Ali", "Amadou", "Askia", "Balla",
				"Bamba", "Barama", "Boubacar", "Daouda", "Demba", "Djibril",
				"Faran", "Fode", "Fodiba", "Fofana", "Hama", "Hamadi",
				"Ibrahim", "Idrissa", "Issa", "Kanku", "Karamoko", "Keita",
				"Koli", "Kombo", "Kunta", "Lamine", "Mahmud", "Mamadi",
				"Mamadou", "Mansa", "Modibo", "Mori", "Moussa", "Muhammad",
				"Nare", "Niani", "Oumar", "Samba", "Sekou", "Sidi",
				"Silamakan", "Sonni", "Souleymane", "Sumanguru", "Sundiata", "Tiramakan",
				"Yoro", "Yusuf"
			},
			FemaleGivenNames: new[]
			{
				"Aissata", "Aminata", "Awa", "Binta", "Dado", "Djeneba",
				"Fanta", "Fatou", "Fatoumata", "Finda", "Hawa", "Kadidia",
				"Kadiatou", "Kankou", "Kumba", "Lalla", "Makoura", "Mariama",
				"Massan", "Moussokoro", "Nana", "Nandi", "Nene", "Niani",
				"Nima", "Oumou", "Ramata", "Rokia", "Sadio", "Sali",
				"Salimata", "Sanata", "Saran", "Sira", "Sitan", "Sogolon",
				"Sona", "Tene", "Tenin", "Yama", "Yaye", "Yelimane",
				"Yira", "Zeynab", "Amina", "Bendu", "Doussou", "Kassa",
				"Maghan", "Tounkara"
			},
			MaleSecondNames: new[]
			{
				"Keita", "Konate", "Traore", "Kante", "Camara", "Coulibaly",
				"Diarra", "Doumbia", "Fofana", "Sissoko", "Toure", "Cisse",
				"Conde", "Kourouma", "Soumare", "Sylla", "Berete", "Kone",
				"Jallow", "Jobarteh", "Kuyate", "Susso", "Tounkara", "Samake",
				"Bagayoko", "Dabo", "Drame", "Kanoute", "Magassouba", "Sangare"
			},
			FemaleSecondNames: new[]
			{
				"Keita", "Konate", "Traore", "Kante", "Camara", "Coulibaly",
				"Diarra", "Doumbia", "Fofana", "Sissoko", "Toure", "Cisse",
				"Conde", "Kourouma", "Soumare", "Sylla", "Berete", "Kone",
				"Jallow", "Jobarteh", "Kuyate", "Susso", "Tounkara", "Samake",
				"Bagayoko", "Dabo", "Drame", "Kanoute", "Magassouba", "Sangare"
			},
			ThirdNames: Array.Empty<string>()
		),
		new(
			Key: "Hausa City-States",
			NameCultureName: "Renaissance Hausa",
			EthnicityName: "Late Medieval Hausa",
			EthnicGroup: "West African",
			EthnicSubgroup: "Central Sahel",
			CultureName: "Hausa City-State (c. 1450)",
			NameForm: RenaissanceWorldNameForm.GivenPatronymToponym,
			NameRegex: @"^(?<birthname>[\w'-]{2,}) (?<patronym>(?:dan|yar)-[\w'-]{2,}) (?<toponym>na-[\w'-]{2,})$",
			ReferenceAnchor: "c. 1450",
			EthnicityDescription:
				"The Hausa are a Chadic-speaking people of the walled cities and farming country of the central Sahel. " +
				"They share the Hausa language, traditions of city kingship, market and craft organisation, and ties to " +
				"ancestral towns such as Kano, Katsina, Zazzau, Gobir and Daura. Islam and older religious institutions " +
				"both shape lineage, office and neighbourhood identity.",
			CultureDescription:
				"Hausa city-state culture is built around walled capitals, dye pits, leather and metalworking quarters, " +
				"markets, farming villages, caravan trade and cavalry aristocracies. Royal office, craft, ward, lineage, " +
				"clientage and Islamic learning provide the principal social anchors.",
			PersonalNameDescription:
				"Choose a Hausa or Islamicate personal name. Praise names and royal titles are separate from the personal " +
				"name entered here.",
			SecondNameDescription:
				"Enter dan-Father for a man or yar-Father for a woman. The prefix identifies the person as a son or " +
				"daughter of the named parent and is not a hereditary surname.",
			ThirdNameDescription:
				"Enter na-Place or another na- affiliation naming the person's city, district, ward or recognised " +
				"occupational quarter, such as na-Kano or na-Kurmi-Market.",
			MinimumSecondNameCount: 1,
			MinimumThirdNameCount: 200,
			BloodModel: "Majority O",
			SkinProfile: "dark_skin",
			HairProfile: "black_brown_grey_hair",
			EyeProfile: "brown_green_eyes",
			MaleGivenNames: new[]
			{
				"Abdullahi", "Abubakar", "Adamu", "Ahmadu", "Ali", "Aminu",
				"Bako", "Bala", "Bawa", "Bello", "Buba", "Dabo",
				"Danjuma", "Dauda", "Garba", "Gidado", "Habibu", "Hamza",
				"Haruna", "Hassan", "Ibrahim", "Idris", "Isah", "Jibril",
				"Kabiru", "Kanta", "Lawal", "Mahmud", "Mamman", "Mansur",
				"Muhammadu", "Musa", "Mustapha", "Nuhu", "Sani", "Shehu",
				"Sule", "Sumaila", "Tanko", "Umar", "Usman", "Yahaya",
				"Yakubu", "Yusuf", "Zubairu", "Audu", "Barau", "Gambo",
				"Jatau", "Magaji"
			},
			FemaleGivenNames: new[]
			{
				"A'isha", "Amina", "Asabe", "Auwalu", "Binta", "Bilkisu",
				"Dije", "Fadima", "Fatima", "Fati", "Hadiza", "Hafsatu",
				"Halima", "Hauwa", "Hindatu", "Jamila", "Jummai", "Khadija",
				"Ladidi", "Lami", "Laraba", "Lubabatu", "Maimuna", "Maryam",
				"Murjanatu", "Nafisa", "Nana", "Rabi", "Rahila", "Ramatu",
				"Rashida", "Rukayya", "Sa'adatu", "Safiya", "Salamatu", "Samira",
				"Saratu", "Shamsiyya", "Talatu", "Yelwa", "Zahra", "Zainab",
				"Zulaiha", "Zuwaira", "Ado", "Balaraba", "Gimbiya", "Kande",
				"Ladi", "Zabi"
			},
			MaleSecondNames: new[]
			{
				"dan-Abdullahi", "dan-Abubakar", "dan-Adamu", "dan-Ahmadu", "dan-Ali", "dan-Bako",
				"dan-Bala", "dan-Bawa", "dan-Bello", "dan-Dauda", "dan-Garba", "dan-Haruna",
				"dan-Hassan", "dan-Ibrahim", "dan-Idris", "dan-Isah", "dan-Jibril", "dan-Kanta",
				"dan-Mahmud", "dan-Musa", "dan-Shehu", "dan-Sule", "dan-Umar", "dan-Usman",
				"dan-Yakubu"
			},
			FemaleSecondNames: new[]
			{
				"yar-Abdullahi", "yar-Abubakar", "yar-Adamu", "yar-Ahmadu", "yar-Ali", "yar-Bako",
				"yar-Bala", "yar-Bawa", "yar-Bello", "yar-Dauda", "yar-Garba", "yar-Haruna",
				"yar-Hassan", "yar-Ibrahim", "yar-Idris", "yar-Isah", "yar-Jibril", "yar-Kanta",
				"yar-Mahmud", "yar-Musa", "yar-Shehu", "yar-Sule", "yar-Umar", "yar-Usman",
				"yar-Yakubu"
			},
			ThirdNames: RenaissanceHausaAffiliationInventory
		),
		new(
			Key: "Kingdom of Kongo",
			NameCultureName: "Renaissance Kongo",
			EthnicityName: "Late Medieval Bakongo",
			EthnicGroup: "Central African",
			EthnicSubgroup: "Kingdom of Kongo",
			CultureName: "Kongo Kingdom (c. 1450)",
			NameForm: RenaissanceWorldNameForm.GivenClan,
			NameRegex: @"^(?<birthname>[\w'-]{2,}) (?<familygroupname>[\w'-]{2,})$",
			ReferenceAnchor: "c. 1450",
			EthnicityDescription:
				"The Bakongo are a Kikongo-speaking people of the lower Congo region and the provinces gathered into the " +
				"kingdom of Kongo. Matrilineal descent, clan membership, village and provincial affiliation, and loyalty " +
				"to rulers and office-holders define belonging. Shared language, markets and sacred authority connect " +
				"communities spread across river, forest and savanna.",
			CultureDescription:
				"Kongo culture encompasses farming and craft villages, market exchange, provincial courts, tribute, iron " +
				"and textile production, river travel and the growing authority of Mbanza Kongo. Matrilineal kin, office, " +
				"clientage and provincial allegiance organise political and domestic life.",
			PersonalNameDescription:
				"Choose a Kikongo personal, praise or office-associated name. A name may recall birth circumstances, " +
				"character, ancestry or the dignity of an office.",
			SecondNameDescription:
				"Choose a matrilineal clan, royal house, province or office affiliation. Mani- compounds identify lordship " +
				"over a place or office and remain one token.",
			ThirdNameDescription: "",
			MinimumSecondNameCount: 1,
			MinimumThirdNameCount: 0,
			BloodModel: "Majority O",
			SkinProfile: "dark_skin",
			HairProfile: "black_brown_grey_hair",
			EyeProfile: "brown_green_eyes",
			MaleGivenNames: new[]
			{
				"Nimi", "Lukeni", "Nanga", "Ntinu", "Mpanzu", "Nzinga",
				"Mvemba", "Nkanga", "Nlaza", "Nsaku", "Nseke", "Nsimba",
				"Ntela", "Nzenze", "Mbemba", "Makaba", "Mbenza", "Mbanza",
				"Mbianda", "Mbumba", "Mpemba", "Mpuku", "Mputu", "Ndongo",
				"Nduka", "Nfinda", "Nganga", "Ngola", "Nkosi", "Nkuwu",
				"Nsanda", "Nsundi", "Ntamba", "Ntondo", "Nzau", "Nzinga-Nkuwu",
				"Lukeni-lua-Nimi", "Nimi-a-Lukeni", "Mpanzu-a-Nsundi", "Nkanga-a-Mvemba", "Mbala", "Mfuama",
				"Miala", "Mpetelo", "Mvika", "Ndombe", "Ngoma", "Nsona",
				"Ntumba", "Zinga"
			},
			FemaleGivenNames: new[]
			{
				"Nzinga", "Mpanzu", "Mvemba", "Nlaza", "Nsaku", "Nsimba",
				"Ntela", "Nzenze", "Makaba", "Mbenza", "Mbianda", "Mbumba",
				"Mpemba", "Mpuku", "Mputu", "Ndona", "Nduka", "Nfinda",
				"Nganga", "Ngudi", "Nkento", "Nkosi", "Nsanda", "Nsundi",
				"Ntamba", "Ntondo", "Nzau", "Nzola", "Zola", "Lemba",
				"Lukeni", "Mambu", "Mavambu", "Mbala", "Mbangi", "Miala",
				"Mpati", "Mvika", "Ndumba", "Nene", "Ngoma", "Nkanza",
				"Nkumba", "Nsenga", "Nsona", "Ntumba", "Nzuzi", "Songa",
				"Wenda", "Yela"
			},
			MaleSecondNames: new[]
			{
				"Kimpanzu", "Kinlaza", "Kinkanga", "Nsaku-Lau", "Mani-Kongo", "Mani-Soyo",
				"Mani-Mbata", "Mani-Nsundi", "Mani-Mpemba", "Mani-Mbamba", "Mbenza", "Mpanzu",
				"Nlaza", "Nkanga", "Nsaku", "Nsimba", "Ntela", "Nzenze",
				"Lukeni", "Nimi", "Mvemba", "Nkuwu", "Nzinga", "Makaba",
				"Mbala", "Mbumba", "Ndongo", "Ngoma", "Nsanda", "Ntumba"
			},
			FemaleSecondNames: new[]
			{
				"Kimpanzu", "Kinlaza", "Kinkanga", "Nsaku-Lau", "Mani-Kongo", "Mani-Soyo",
				"Mani-Mbata", "Mani-Nsundi", "Mani-Mpemba", "Mani-Mbamba", "Mbenza", "Mpanzu",
				"Nlaza", "Nkanga", "Nsaku", "Nsimba", "Ntela", "Nzenze",
				"Lukeni", "Nimi", "Mvemba", "Nkuwu", "Nzinga", "Makaba",
				"Mbala", "Mbumba", "Ndongo", "Ngoma", "Nsanda", "Ntumba"
			},
			ThirdNames: Array.Empty<string>()
		),
		new(
			Key: "Mutapa and Shona Plateau",
			NameCultureName: "Renaissance Shona",
			EthnicityName: "Late Medieval Shona",
			EthnicGroup: "Southern African",
			EthnicSubgroup: "Zimbabwe Plateau",
			CultureName: "Mutapa-Shona Plateau (c. 1450)",
			NameForm: RenaissanceWorldNameForm.GivenClan,
			NameRegex: @"^(?<birthname>[\w'-]{2,}) (?<familygroupname>[\w'-]{2,})$",
			ReferenceAnchor: "c. 1450",
			EthnicityDescription:
				"The Shona are a related group of Shona-speaking peoples of the Zimbabwe plateau, joined by clan and totem " +
				"traditions, cattle and farming life, sacred landscapes and long-distance gold trade. Clan praise, " +
				"ancestral spirit, local chief and rainmaking authority tell a person who they are and where their " +
				"obligations lie. Stone-built centres and dispersed homesteads belong to the same broad plateau " +
				"civilisation.",
			CultureDescription:
				"Shona plateau culture is organised around homesteads, cattle wealth, grain farming, gold production, " +
				"spirit mediums, rainmaking institutions, stone-built centres and trade to the coast. Clan, totem, " +
				"locality, praise tradition and service to chiefs or rulers shape reputation.",
			PersonalNameDescription:
				"Choose a Shona personal or praise name. Many names carry a transparent meaning and are not rigidly " +
				"limited to one gender.",
			SecondNameDescription:
				"Choose the clan or totemic affiliation. It links the character to praise poetry, marriage rules, " +
				"ancestral obligations and a wider descent community.",
			ThirdNameDescription: "",
			MinimumSecondNameCount: 1,
			MinimumThirdNameCount: 0,
			BloodModel: "Majority O",
			SkinProfile: "dark_skin",
			HairProfile: "black_brown_grey_hair",
			EyeProfile: "brown_green_eyes",
			MaleGivenNames: new[]
			{
				"Mutota", "Matope", "Mukombwe", "Nyahuma", "Chisamharu", "Chivere",
				"Gatsi", "Kapararidze", "Changa", "Togwa", "Dlembeu", "Nyatsimba",
				"Mavura", "Mhande", "Muchenje", "Chikuyo", "Chikanga", "Chiremba",
				"Chisango", "Chitauro", "Garikai", "Gondo", "Guta", "Hama",
				"Hungwe", "Kaguvi", "Karigamombe", "Kaseke", "Mambo", "Mapondera",
				"Masimba", "Mavhudzi", "Mhofu", "Moyo", "Mukwati", "Munhumutapa",
				"Murenga", "Mutasa", "Mutenheri", "Nehanda", "Nehoreka", "Nyandoro",
				"Nyamhika", "Nyasha", "Pfumojena", "Rusere", "Sango", "Shumba",
				"Tenda", "Zenda"
			},
			FemaleGivenNames: new[]
			{
				"Nehanda", "Nyamhika", "Chipo", "Chiedza", "Chikondi", "Chipoziso",
				"Chisomo", "Dudziro", "Farai", "Fungai", "Gonai", "Kundai",
				"Maidei", "Makanaka", "Mandisa", "Mavambo", "Mavis", "Mazvita",
				"Mbuya", "Mhofu", "Munashe", "Mutsawashe", "Ndaniso", "Nhamo",
				"Nyaradzo", "Nyasha", "Nyatsitsi", "Ropafadzo", "Rudo", "Rufaro",
				"Rutendo", "Shamiso", "Shingai", "Simbarashe", "Tariro", "Tatenda",
				"Tawananyasha", "Tendai", "Tinashe", "Tsitsi", "Vimbai", "Wadzanai",
				"Yeukai", "Zadzisai", "Zanele", "Zivai", "Chenesai", "Maruva",
				"Mudiwa", "Ruramai"
			},
			MaleSecondNames: new[]
			{
				"Hungwe", "Moyo", "Shumba", "Soko", "Mhofu", "Dziva",
				"Gumbo", "Gushungo", "Matemai", "Mbizi", "Mbeva", "Mhara",
				"Mukanya", "Munyati", "Ngara", "Nzou", "Samanyanga", "Shava",
				"Tembo", "Tsunga", "Zebra", "Zimuto", "Chihota", "Chivero",
				"Duma", "Gwenzi", "Makoni", "Manyika", "Rozvi", "Zezuru"
			},
			FemaleSecondNames: new[]
			{
				"Hungwe", "Moyo", "Shumba", "Soko", "Mhofu", "Dziva",
				"Gumbo", "Gushungo", "Matemai", "Mbizi", "Mbeva", "Mhara",
				"Mukanya", "Munyati", "Ngara", "Nzou", "Samanyanga", "Shava",
				"Tembo", "Tsunga", "Zebra", "Zimuto", "Chihota", "Chivero",
				"Duma", "Gwenzi", "Makoni", "Manyika", "Rozvi", "Zezuru"
			},
			ThirdNames: Array.Empty<string>()
		),
		new(
			Key: "Amazigh Maghreb",
			NameCultureName: "Renaissance Amazigh",
			EthnicityName: "Late Medieval Amazigh",
			EthnicGroup: "North African",
			EthnicSubgroup: "Maghreb",
			CultureName: "Amazigh Maghrebi (c. 1450)",
			NameForm: RenaissanceWorldNameForm.GivenPatronymClan,
			NameRegex: @"^(?<birthname>[\w'-]{2,}) (?<patronym>(?:u|ult|ibn|bint)-[\w'-]{2,}) (?<familygroupname>(?:(?:Ayt|Id|Banu|al)-[\w'-]{2,}|[\w'-]{2,}))$",
			ReferenceAnchor: "c. 1450",
			EthnicityDescription:
				"The Amazigh are the indigenous Berber-speaking peoples of the Maghreb's mountains, plains, oases and " +
				"desert margins. They define themselves through language and dialect, tribe and confederation, locality, " +
				"customary law and descent from remembered ancestors or saintly houses. Islam is deeply rooted, while " +
				"councils, pastoral routes and regional dynasties give each community its own political character.",
			CultureDescription:
				"Amazigh culture spans pastoral and agro-pastoral camps, oasis farms, mountain villages, fortified " +
				"settlements, caravan communities and service to Maghrebi dynasties. Tribal council, customary law, " +
				"saintly patronage, Islamic learning and multilingual exchange regulate life.",
			PersonalNameDescription:
				"Choose an Amazigh or Arabic-derived personal name. Older Berber names and Islamicate devotional names are " +
				"both common.",
			SecondNameDescription:
				"Enter u-Father for a man or ult-Father for a woman; Arabic ibn- and bint- forms are also accepted. The " +
				"father's personal name follows the prefix.",
			ThirdNameDescription:
				"Choose a tribal, confederational or ancestral group name. Ayt- and Id- identify the people or descendants " +
				"of a named ancestor or place; Banu- and al- forms occur in Arabic usage, while some established " +
				"confederation names stand alone.",
			MinimumSecondNameCount: 1,
			MinimumThirdNameCount: 1,
			BloodModel: "Majority O",
			SkinProfile: "olive_skin",
			HairProfile: "black_brown_grey_hair",
			EyeProfile: "brown_green_eyes",
			MaleGivenNames: new[]
			{
				"Abdallah", "Abu-Bakr", "Ahmad", "Ali", "Ammar", "Ayyub",
				"Badis", "Buluggin", "Dunas", "Fannun", "Hammad", "Hammu",
				"Hasan", "Idris", "Ifran", "Ilyas", "Ismail", "Jafar",
				"Khalifa", "Khalil", "Mahdi", "Mahmud", "Mansur", "Marin",
				"Masin", "Masinissa", "Maysara", "Muhammad", "Musa", "Nasir",
				"Qasim", "Rashid", "Salih", "Sanhaj", "Tashfin", "Umar",
				"Yahya", "Yaghmurasan", "Yusuf", "Zayan", "Ziri", "Ziyad",
				"Aksil", "Amastan", "Amezian", "Gaya", "Hmad", "Idir",
				"Mokrani", "Uksum"
			},
			FemaleGivenNames: new[]
			{
				"Aicha", "Amina", "Asma", "Dihya", "Fatima", "Fadma",
				"Ghanima", "Hadda", "Halima", "Hennu", "Hnifa", "Ijja",
				"Itto", "Izza", "Jamila", "Kahina", "Khadija", "Lalla",
				"Layla", "Malika", "Mamma", "Maryam", "Masuda", "Mina",
				"Munia", "Nanna", "Numidia", "Rabi'a", "Rahma", "Rqiya",
				"Safiya", "Salima", "Sekkura", "Shama", "Tafrara", "Tafukt",
				"Tala", "Tamimt", "Tinhinan", "Tiziri", "Tuda", "Yamina",
				"Zahra", "Zaynab", "Zohra", "Aldjia", "Daya", "Fella",
				"Kella", "Tassadit"
			},
			MaleSecondNames: new[]
			{
				"u-Ali", "u-Ahmad", "u-Abdallah", "u-Hasan", "u-Husayn", "u-Idris",
				"u-Ismail", "u-Jafar", "u-Mahmud", "u-Mansur", "u-Muhammad", "u-Musa",
				"u-Nasir", "u-Qasim", "u-Rashid", "u-Salih", "u-Umar", "u-Yahya",
				"u-Yusuf", "u-Ziri", "ibn-Tashfin", "ibn-Tumart", "ibn-Ziri", "ibn-Marin",
				"ibn-Hammad"
			},
			FemaleSecondNames: new[]
			{
				"ult-Ali", "ult-Ahmad", "ult-Abdallah", "ult-Hasan", "ult-Husayn", "ult-Idris",
				"ult-Ismail", "ult-Jafar", "ult-Mahmud", "ult-Mansur", "ult-Muhammad", "ult-Musa",
				"ult-Nasir", "ult-Qasim", "ult-Rashid", "ult-Salih", "ult-Umar", "ult-Yahya",
				"ult-Yusuf", "ult-Ziri", "bint-Tashfin", "bint-Tumart", "bint-Ziri", "bint-Marin",
				"bint-Hammad"
			},
			ThirdNames: new[]
			{
				"Ayt-Atta", "Ayt-Yafelman", "Ayt-Idrasen", "Ayt-Waryaghar", "Ayt-Baamran", "Ayt-Mguild",
				"Ayt-Sadden", "Ayt-Seghrouchen", "Ayt-Iznassen", "Ayt-Ouaouzguite", "Sanhaja", "Masmuda",
				"Zenata", "Kutama", "Hawwara", "Nafusa", "Lawata", "Miknasa",
				"Maghrawa", "Banu-Marin", "Banu-Zayyan", "Banu-Hammad", "al-Fasi", "al-Marrakushi",
				"al-Tilimsani", "al-Tunisi", "al-Qayrawani", "al-Sijilmasi", "al-Susi", "al-Rifi"
			}
		)
	};
}

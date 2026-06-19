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
/// Culture, ethnicity and naming coverage for Dark Ages and Medieval culture families.
/// </summary>
/// <remarks>
/// Covers the ItemSeeder culture families across the requested 500-1400 envelope while preserving each
/// family's own builder-facing reference anchor. Ethnicity describes peoplehood and descent; culture
/// describes the court, regional, urban or political customs a character may adopt.
/// Every naming culture supplies 50 masculine and 50 feminine personal names. Toponymic and occupational
/// surname inventories that feature prominently contain at least 200 elements.
///
/// The ethnicity and culture records are intentionally independent. The seeder does not bind an
/// ethnicity to the culture in the same row, so a character may combine ancestry with an adopted
/// court, urban, regional or political culture.
///
/// Integration point: call from the Earth-DarkAgesAndMedieval culture pack in <c>CultureSeederPacks.cs</c>.
/// <code>SeedDarkAgesAndMedieval(questionAnswers);</code>
/// </remarks>
public partial class CultureSeeder
{
	private enum MedievalPeriodNameForm
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

	private sealed record MedievalPeriodSeed(
		string Key,
		string NameCultureName,
		string EthnicityName,
		string EthnicGroup,
		string EthnicSubgroup,
		string CultureName,
		MedievalPeriodNameForm NameForm,
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

	private void SeedDarkAgesAndMedieval(IReadOnlyDictionary<string, string> questionAnswers)
	{
		if (questionAnswers["seednames"].EqualToAny("y", "yes"))
		{
			SeedDarkAgesAndMedievalNames();
		}

		if (questionAnswers["seedheritage"].EqualToAny("y", "yes"))
		{
			SeedDarkAgesAndMedievalHeritage();
		}
	}

	private void SeedDarkAgesAndMedievalNames()
	{
		foreach (MedievalPeriodSeed seed in MedievalPeriodSeeds)
		{
			ValidateMedievalPeriodSeed(seed);
			NameCulture nameCulture = CreateMedievalPeriodNameCulture(seed);
			SeedMedievalPeriodProfile(seed, nameCulture, Gender.Male, seed.MaleGivenNames, seed.MaleSecondNames);
			SeedMedievalPeriodProfile(seed, nameCulture, Gender.Female, seed.FemaleGivenNames, seed.FemaleSecondNames);
		}

		_context.SaveChanges();
	}

	private NameCulture CreateMedievalPeriodNameCulture(MedievalPeriodSeed seed)
	{
		return AddNameCulture(
			seed.NameCultureName,
			seed.NameRegex,
			GetMedievalPeriodNameElements(seed),
			GetMedievalPeriodNamePatterns(seed.NameForm));
	}

	private void SeedMedievalPeriodProfile(
		MedievalPeriodSeed seed,
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

		NameUsage? secondUsage = GetMedievalPeriodSecondUsage(seed.NameForm);
		if (secondUsage.HasValue)
		{
			AddRandomNameDice(profile, secondUsage.Value, "1");
			foreach (string name in secondNames)
			{
				AddRandomNameElement(profile, secondUsage.Value, name, 100);
			}
		}

		NameUsage? thirdUsage = GetMedievalPeriodThirdUsage(seed.NameForm);
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

	private void SeedDarkAgesAndMedievalHeritage()
	{
		foreach (MedievalPeriodSeed seed in MedievalPeriodSeeds)
		{
			ValidateMedievalPeriodSeed(seed);
			AddEthnicity(
				_humanRace,
				seed.EthnicityName,
				seed.EthnicGroup,
				seed.BloodModel,
				0.0,
				0.0,
				seed.EthnicSubgroup,
				description: seed.EthnicityDescription);

			ApplyMedievalPeriodCharacteristicDefaults(seed);

			string nameCulture = ResolveMedievalPeriodNameCulture(seed);
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

		_context.SaveChanges();
	}

	private string ResolveMedievalPeriodNameCulture(MedievalPeriodSeed seed)
	{
		if (_context.NameCultures.Any(x => x.Name.Equals(seed.NameCultureName, StringComparison.OrdinalIgnoreCase)))
		{
			return seed.NameCultureName;
		}

		return seed.NameForm switch
		{
			MedievalPeriodNameForm.GivenOnly => "Simple",
			MedievalPeriodNameForm.GivenPatronym => "Given and Patronym",
			MedievalPeriodNameForm.GivenPatronymToponym => "Given and Patronym",
			MedievalPeriodNameForm.GivenPatronymClan => "Given and Patronym",
			MedievalPeriodNameForm.GivenToponym => "Given and Toponym",
			_ => "Given and Family"
		};
	}

	private void ApplyMedievalPeriodCharacteristicDefaults(MedievalPeriodSeed seed)
	{
		AddMedievalPeriodCharacteristicIfAvailable(seed.EthnicityName, "Distinctive Feature", "All Distinctive Features");
		AddMedievalPeriodCharacteristicIfAvailable(seed.EthnicityName, "Eye Shape", "All Eye Shapes");
		AddMedievalPeriodCharacteristicIfAvailable(seed.EthnicityName, "Hair Style", "All Hair Styles");
		AddMedievalPeriodCharacteristicIfAvailable(seed.EthnicityName, "Facial Hair Style", "All Facial Hair Styles");
		AddMedievalPeriodCharacteristicIfAvailable(seed.EthnicityName, "Humanoid Frame", "All Frames");
		AddMedievalPeriodCharacteristicIfAvailable(seed.EthnicityName, "Nose", "All Noses");
		AddMedievalPeriodCharacteristicIfAvailable(seed.EthnicityName, "Person Word", "Weighted Person Words");
		AddMedievalPeriodCharacteristicIfAvailable(seed.EthnicityName, "Hair Colour", seed.HairProfile);
		AddMedievalPeriodCharacteristicIfAvailable(seed.EthnicityName, "Facial Hair Colour", seed.HairProfile);
		AddMedievalPeriodCharacteristicIfAvailable(seed.EthnicityName, "Eye Colour", seed.EyeProfile);
		AddMedievalPeriodCharacteristicIfAvailable(seed.EthnicityName, "Skin Colour", seed.SkinProfile);
	}

	private void AddMedievalPeriodCharacteristicIfAvailable(string ethnicity, string definition, string profile)
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

	private static void ValidateMedievalPeriodSeed(MedievalPeriodSeed seed)
	{
		if (string.IsNullOrWhiteSpace(seed.ReferenceAnchor) ||
			string.IsNullOrWhiteSpace(seed.EthnicityDescription) ||
			string.IsNullOrWhiteSpace(seed.CultureDescription) ||
			string.IsNullOrWhiteSpace(seed.PersonalNameDescription))
		{
			throw new InvalidOperationException($"{seed.Key} is missing required reference or player-facing prose.");
		}

		ValidateMedievalPeriodGivenNames(seed.Key, "male", seed.MaleGivenNames);
		ValidateMedievalPeriodGivenNames(seed.Key, "female", seed.FemaleGivenNames);

		NameUsage? secondUsage = GetMedievalPeriodSecondUsage(seed.NameForm);
		if (secondUsage.HasValue)
		{
			if (string.IsNullOrWhiteSpace(seed.SecondNameDescription))
			{
				throw new InvalidOperationException($"{seed.Key} requires player guidance for its second name element.");
			}
			ValidateMedievalPeriodSupportingNames(
				seed.Key,
				secondUsage.Value.DescribeEnum(),
				seed.MaleSecondNames,
				seed.MinimumSecondNameCount);
			ValidateMedievalPeriodSupportingNames(
				seed.Key,
				secondUsage.Value.DescribeEnum(),
				seed.FemaleSecondNames,
				seed.MinimumSecondNameCount);
		}

		NameUsage? thirdUsage = GetMedievalPeriodThirdUsage(seed.NameForm);
		if (thirdUsage.HasValue)
		{
			if (string.IsNullOrWhiteSpace(seed.ThirdNameDescription))
			{
				throw new InvalidOperationException($"{seed.Key} requires player guidance for its third name element.");
			}
			ValidateMedievalPeriodSupportingNames(
				seed.Key,
				thirdUsage.Value.DescribeEnum(),
				seed.ThirdNames,
				seed.MinimumThirdNameCount);
		}

		ValidateMedievalPeriodRegex(seed);
	}

	private static void ValidateMedievalPeriodGivenNames(string culture, string gender, IReadOnlyCollection<string> names)
	{
		if (names.Count is < 50 or > 100)
		{
			throw new InvalidOperationException(
				$"{culture} must define between 50 and 100 {gender} personal names; found {names.Count}.");
		}

		ValidateMedievalPeriodUniqueSingleTokenNames(culture, $"female personal", names);
	}

	private static void ValidateMedievalPeriodSupportingNames(
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

		ValidateMedievalPeriodUniqueSingleTokenNames(culture, usage, names);
	}

	private static void ValidateMedievalPeriodUniqueSingleTokenNames(
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

	private static void ValidateMedievalPeriodRegex(MedievalPeriodSeed seed)
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
			ValidateMedievalPeriodGeneratedName(regex, seed, givenName, maleSecond, third);
		}

		foreach (string givenName in seed.FemaleGivenNames)
		{
			ValidateMedievalPeriodGeneratedName(regex, seed, givenName, femaleSecond, third);
		}

		foreach (string secondName in seed.MaleSecondNames)
		{
			ValidateMedievalPeriodGeneratedName(regex, seed, seed.MaleGivenNames[0], secondName, third);
		}

		foreach (string secondName in seed.FemaleSecondNames)
		{
			ValidateMedievalPeriodGeneratedName(regex, seed, seed.FemaleGivenNames[0], secondName, third);
		}

		foreach (string thirdName in seed.ThirdNames)
		{
			ValidateMedievalPeriodGeneratedName(regex, seed, seed.MaleGivenNames[0], maleSecond, thirdName);
		}
	}

	private static void ValidateMedievalPeriodGeneratedName(
		Regex regex,
		MedievalPeriodSeed seed,
		string givenName,
		string? secondName,
		string? thirdName)
	{
		string fullName = ComposeMedievalPeriodFullName(seed.NameForm, givenName, secondName, thirdName);
		if (!regex.IsMatch(fullName))
		{
			throw new InvalidOperationException(
				$"{seed.Key} generated name '{fullName}' does not satisfy regex '{seed.NameRegex}'.");
		}
	}

	private static string ComposeMedievalPeriodFullName(
		MedievalPeriodNameForm form,
		string givenName,
		string? secondName,
		string? thirdName)
	{
		return form switch
		{
			MedievalPeriodNameForm.GivenOnly => givenName,
			MedievalPeriodNameForm.GivenFamily => $"{givenName} {secondName}",
			MedievalPeriodNameForm.FamilyGiven => $"{secondName} {givenName}",
			MedievalPeriodNameForm.GivenPatronym => $"{givenName} {secondName}",
			MedievalPeriodNameForm.GivenToponym => $"{givenName} {secondName}",
			MedievalPeriodNameForm.GivenClan => $"{givenName} {secondName}",
			MedievalPeriodNameForm.GivenPatronymToponym => $"{givenName} {secondName} {thirdName}",
			MedievalPeriodNameForm.GivenPatronymClan => $"{givenName} {secondName} {thirdName}",
			_ => throw new ArgumentOutOfRangeException(nameof(form), form, null)
		};
	}

	private static string CompleteMedievalPeriodPlayerSentence(string text)
	{
		string value = text.Trim();
		if (value.Length == 0)
		{
			return value;
		}

		return value.EndsWith(".", StringComparison.Ordinal) ? value : $"{value}.";
	}

	private static string FormatMedievalPeriodPlayerExamples(IEnumerable<string> names, int maximum = 8)
	{
		return string.Join(", ", names.Distinct(StringComparer.OrdinalIgnoreCase).Take(maximum));
	}

	private static string BuildMedievalPeriodPersonalDescription(MedievalPeriodSeed seed)
	{
		return
			$"{CompleteMedievalPeriodPlayerSentence(seed.PersonalNameDescription)} " +
			$"Common masculine examples include {FormatMedievalPeriodPlayerExamples(seed.MaleGivenNames, 6)}; " +
			$"common feminine examples include {FormatMedievalPeriodPlayerExamples(seed.FemaleGivenNames, 6)}.";
	}

	private static string BuildMedievalPeriodSecondDescription(MedievalPeriodSeed seed)
	{
		string guidance = CompleteMedievalPeriodPlayerSentence(seed.SecondNameDescription);
		if (seed.MaleSecondNames.SequenceEqual(seed.FemaleSecondNames, StringComparer.OrdinalIgnoreCase))
		{
			return $"{guidance} Examples include {FormatMedievalPeriodPlayerExamples(seed.MaleSecondNames)}.";
		}

		return
			$"{guidance} Masculine examples include {FormatMedievalPeriodPlayerExamples(seed.MaleSecondNames, 6)}; " +
			$"feminine examples include {FormatMedievalPeriodPlayerExamples(seed.FemaleSecondNames, 6)}.";
	}

	private static string BuildMedievalPeriodThirdDescription(MedievalPeriodSeed seed)
	{
		return
			$"{CompleteMedievalPeriodPlayerSentence(seed.ThirdNameDescription)} " +
			$"Examples include {FormatMedievalPeriodPlayerExamples(seed.ThirdNames)}.";
	}

	private static (string Name, int Minimum, int Maximum, string Description, NameUsage Usage)[]
		GetMedievalPeriodNameElements(MedievalPeriodSeed seed)
	{
		(string Name, int Minimum, int Maximum, string Description, NameUsage Usage) personal =
			("Personal Name", 1, 1, BuildMedievalPeriodPersonalDescription(seed), NameUsage.BirthName);

		NameUsage? secondUsage = GetMedievalPeriodSecondUsage(seed.NameForm);
		NameUsage? thirdUsage = GetMedievalPeriodThirdUsage(seed.NameForm);
		var elements = new List<(string Name, int Minimum, int Maximum, string Description, NameUsage Usage)>
		{
			personal
		};

		if (secondUsage.HasValue)
		{
			elements.Add((
				GetMedievalPeriodElementLabel(secondUsage.Value),
				1,
				1,
				BuildMedievalPeriodSecondDescription(seed),
				secondUsage.Value));
		}

		if (thirdUsage.HasValue)
		{
			elements.Add((
				GetMedievalPeriodElementLabel(thirdUsage.Value),
				1,
				1,
				BuildMedievalPeriodThirdDescription(seed),
				thirdUsage.Value));
		}

		return seed.NameForm switch
		{
			MedievalPeriodNameForm.FamilyGiven => elements.OrderByDescending(x => x.Usage == NameUsage.Surname).ToArray(),
			_ => elements.ToArray()
		};
	}

	private static string GetMedievalPeriodElementLabel(NameUsage usage)
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
		GetMedievalPeriodNamePatterns(MedievalPeriodNameForm form)
	{
		return form switch
		{
			MedievalPeriodNameForm.GivenOnly => MedievalPeriodSingleElementPatterns(NameUsage.BirthName),
			MedievalPeriodNameForm.GivenFamily => MedievalPeriodTwoElementPatterns(NameUsage.BirthName, NameUsage.Surname, familyFirst: false),
			MedievalPeriodNameForm.FamilyGiven => MedievalPeriodTwoElementPatterns(NameUsage.BirthName, NameUsage.Surname, familyFirst: true),
			MedievalPeriodNameForm.GivenPatronym => MedievalPeriodTwoElementPatterns(NameUsage.BirthName, NameUsage.Patronym, familyFirst: false),
			MedievalPeriodNameForm.GivenToponym => MedievalPeriodTwoElementPatterns(NameUsage.BirthName, NameUsage.Toponym, familyFirst: false),
			MedievalPeriodNameForm.GivenClan => MedievalPeriodTwoElementPatterns(NameUsage.BirthName, NameUsage.FamilyGroupName, familyFirst: false),
			MedievalPeriodNameForm.GivenPatronymToponym =>
				MedievalPeriodThreeElementPatterns(NameUsage.BirthName, NameUsage.Patronym, NameUsage.Toponym),
			MedievalPeriodNameForm.GivenPatronymClan =>
				MedievalPeriodThreeElementPatterns(NameUsage.BirthName, NameUsage.Patronym, NameUsage.FamilyGroupName),
			_ => throw new ArgumentOutOfRangeException(nameof(form), form, null)
		};
	}

	private static (NameStyle Style, string Pattern, NameUsage[] Parameters)[] MedievalPeriodSingleElementPatterns(NameUsage usage)
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

	private static (NameStyle Style, string Pattern, NameUsage[] Parameters)[] MedievalPeriodTwoElementPatterns(
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

	private static (NameStyle Style, string Pattern, NameUsage[] Parameters)[] MedievalPeriodThreeElementPatterns(
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

	private static NameUsage? GetMedievalPeriodSecondUsage(MedievalPeriodNameForm form)
	{
		return form switch
		{
			MedievalPeriodNameForm.GivenFamily => NameUsage.Surname,
			MedievalPeriodNameForm.FamilyGiven => NameUsage.Surname,
			MedievalPeriodNameForm.GivenPatronym => NameUsage.Patronym,
			MedievalPeriodNameForm.GivenToponym => NameUsage.Toponym,
			MedievalPeriodNameForm.GivenClan => NameUsage.FamilyGroupName,
			MedievalPeriodNameForm.GivenPatronymToponym => NameUsage.Patronym,
			MedievalPeriodNameForm.GivenPatronymClan => NameUsage.Patronym,
			_ => null
		};
	}

	private static NameUsage? GetMedievalPeriodThirdUsage(MedievalPeriodNameForm form)
	{
		return form switch
		{
			MedievalPeriodNameForm.GivenPatronymToponym => NameUsage.Toponym,
			MedievalPeriodNameForm.GivenPatronymClan => NameUsage.FamilyGroupName,
			_ => null
		};
	}

	private static readonly string[] MedievalBritishToponymInventory =
	new[]
	{
		"of-Wessex", "of-Mercia", "of-Kent", "of-Essex", "of-Sussex", "of-East-Anglia",
		"of-Northumbria", "of-Lindsey", "of-Hwicce", "of-Deira", "of-Bernicia", "of-Elmet",
		"of-Rheged", "of-Dumnonia", "of-Winchester", "of-Canterbury", "of-London", "of-York",
		"of-Lincoln", "of-Chester", "of-Exeter", "of-Bath", "of-Gloucester", "of-Worcester",
		"of-Hereford", "of-Oxford", "of-Cambridge", "of-Norwich", "of-Thetford", "of-Ipswich",
		"of-Colchester", "of-Rochester", "of-Dover", "of-Sandwich", "of-Reculver", "of-Lyminge",
		"of-Whitby", "of-Ripon", "of-Hexham", "of-Durham", "of-Bamburgh", "of-Lindisfarne",
		"of-Jarrow", "of-Wearmouth", "of-Beverley", "of-Selby", "of-Stamford", "of-Peterborough",
		"of-Ely", "of-Crowland", "of-Bury-St-Edmunds", "of-Malmesbury", "of-Sherborne", "of-Glastonbury",
		"of-Wells", "of-Salisbury", "of-Wilton", "of-Dorchester", "of-Wareham", "of-Shaftesbury",
		"of-Chichester", "of-Lewes", "of-Hastings", "of-Pevensey", "of-Guildford", "of-Reading",
		"of-Abingdon", "of-Wallingford", "of-Aylesbury", "of-Bedford", "of-Northampton", "of-Leicester",
		"of-Nottingham", "of-Derby", "of-Warwick", "of-Coventry", "of-Tamworth", "of-Lichfield",
		"of-Stafford", "of-Shrewsbury", "of-Bridgnorth", "of-Ludlow", "of-Droitwich", "of-Evesham",
		"of-Tewkesbury", "of-Cirencester", "of-Bristol", "of-Berkeley", "of-Chepstow", "of-Caerleon",
		"of-Cardiff", "of-Carmarthen", "of-St-Davids", "of-Bangor", "of-Carlisle", "of-Appleby",
		"of-Penrith", "of-Lancaster", "of-Manchester", "of-Salford", "of-Preston", "of-Bolton",
		"of-Blackburn", "of-Wakefield", "of-Leeds", "of-Doncaster", "of-Sheffield", "of-Rotherham",
		"of-Hull", "of-Scarborough", "of-Bridlington", "of-Malton", "of-Pickering", "of-Richmond",
		"of-Yarm", "of-Hartlepool", "of-Gateshead", "of-Tynemouth", "of-Alnwick", "of-Morpeth",
		"of-Berwick", "of-Edinburgh", "of-Dunbar", "of-Stirling", "of-Perth", "of-Scone",
		"of-Aberdeen", "of-Elgin", "of-Inverness", "of-Glasgow", "of-Galloway", "of-Argyll",
		"of-Lothian", "of-Fife", "of-Strathclyde", "of-Orkney", "of-Shetland", "of-Man",
		"of-Dublin", "of-Waterford", "of-Wexford", "of-Cork", "of-Limerick", "of-Galway",
		"of-Kildare", "of-Meath", "of-Tara", "of-Armagh", "of-Down", "of-Derry",
		"at-Ash-Ford", "at-Oak-Lea", "at-Birch-Wood", "at-Elm-Ham", "at-Willow-Mere", "at-Thorn-Bury",
		"at-Stone-Bridge", "at-Long-Ford", "at-White-Hill", "at-Black-Wood", "at-Green-Lea", "at-Red-Cliff",
		"at-Cold-Stream", "at-Broad-Moor", "at-High-Ridge", "at-Low-Valley", "at-East-Gate", "at-West-Gate",
		"at-North-Field", "at-South-Field", "at-Kings-Ton", "at-Queens-Bury", "at-Church-Ham", "at-Mill-Ford",
		"at-Market-Stead", "by-Ash-Ford", "by-Oak-Lea", "by-Birch-Wood", "by-Elm-Ham", "by-Willow-Mere",
		"by-Thorn-Bury", "by-Stone-Bridge", "by-Long-Ford", "by-White-Hill", "by-Black-Wood", "by-Green-Lea",
		"by-Red-Cliff", "by-Cold-Stream", "by-Broad-Moor", "by-High-Ridge", "by-Low-Valley", "by-East-Gate",
		"by-West-Gate", "by-North-Field", "by-South-Field", "by-Kings-Ton", "by-Queens-Bury", "by-Church-Ham",
		"by-Mill-Ford", "by-Market-Stead"
	};

	private static readonly string[] MedievalNormanToponymInventory =
	new[]
	{
		"de-Bayeux", "de-Caen", "de-Falaise", "de-Rouen", "de-Dieppe", "de-Evreux",
		"de-Lisieux", "de-Coutances", "de-Avranches", "de-Alencon", "de-Argentan", "de-Domfront",
		"de-Mortain", "de-Gisors", "de-Vernon", "de-Les-Andelys", "de-Pont-Audemer", "de-Bec",
		"de-Jumieges", "de-Mont-Saint-Michel", "de-Cherbourg", "de-Barfleur", "de-Granville", "de-Saint-Lo",
		"de-Vire", "de-Sees", "de-Fecamp", "de-Caudebec", "de-Harfleur", "de-Honfleur",
		"de-Eu", "de-Aumale", "de-Neufchatel", "de-Gournay", "de-Beaumont", "de-Harcourt",
		"de-Tancarville", "de-Ivry", "de-Breteuil", "de-Conches", "de-Nonancourt", "de-Louviers",
		"de-Gaillon", "de-Pacy", "de-Bernay", "de-Brionne", "de-Lillebonne", "de-Bolbec",
		"de-Caux", "de-Bessin", "de-Cotentin", "de-Avranchin", "de-Hiemois", "de-Vexin",
		"de-Perche", "de-Paris", "de-Reims", "de-Chartres", "de-Orleans", "de-Tours",
		"de-Blois", "de-Champagne", "de-Brie", "de-Picardie", "de-Amiens", "de-Beauvais",
		"de-Senlis", "de-Soissons", "de-Laon", "de-Compiegne", "de-Saint-Denis", "de-Etampes",
		"de-Melun", "de-Meaux", "de-Provins", "de-Troyes", "de-Sens", "de-Auxerre",
		"de-Nevers", "de-Bourges", "de-Poitiers", "de-Limoges", "de-Angouleme", "de-Saintes",
		"de-Bordeaux", "de-Bayonne", "de-Toulouse", "de-Carcassonne", "de-Narbonne", "de-Montpellier",
		"de-Nimes", "de-Arles", "de-Avignon", "de-Marseille", "de-Aix", "de-Lyon",
		"de-Vienne", "de-Grenoble", "de-Valence", "de-Dijon", "de-Autun", "de-Beaune",
		"de-Macon", "de-Chalon", "de-Besancon", "de-Langres", "de-Verdun", "de-Metz",
		"de-Toul", "de-Nancy", "de-Strasbourg", "de-Colmar", "de-Mulhouse", "de-Basel",
		"de-Geneva", "de-Lausanne", "de-Liege", "de-Namur", "de-Hainaut", "de-Brabant",
		"de-Flanders", "de-Ghent", "de-Bruges", "de-Ypres", "de-Lille", "de-Arras",
		"de-Cambrai", "de-Tournai", "de-Valenciennes", "de-Mons", "de-Boulogne", "de-Calais",
		"de-Saint-Omer", "de-Abbeville", "de-Ponthieu", "de-Le-Mans", "de-Angers", "de-Saumur",
		"de-Nantes", "de-Rennes", "du-Bois", "du-Pont", "du-Val", "du-Mont",
		"du-Bourg", "du-Moulin", "du-Chateau", "du-Marais", "du-Pre", "du-Clos",
		"du-Puits", "du-Roc", "du-Ruisseau", "du-Four", "du-Verger", "du-Champ",
		"du-Chemin", "du-Port", "du-Gue", "du-Tertre", "du-Buisson", "du-Mesnil",
		"du-Fresne", "du-Chene", "du-Houx", "de-la-Fontaine", "de-la-Roche", "de-la-Riviere",
		"de-la-Tour", "de-la-Porte", "de-la-Croix", "de-la-Ferte", "de-la-Haie", "de-la-Lande",
		"de-la-Vallee", "de-la-Prairie", "de-la-Grange", "de-la-Chapelle", "de-la-Motte", "de-la-Foret",
		"de-la-Cour", "de-la-Cote", "de-la-Fosse", "de-la-Source", "de-la-Bruyere", "des-Bois",
		"des-Ponts", "des-Vignes", "des-Champs", "des-Roches", "des-Fontaines", "des-Moulins",
		"des-Pres", "des-Tours", "des-Forges", "des-Hayes", "des-Monts", "des-Vallees",
		"des-Etangs", "des-Vergers"
	};

	private static readonly string[] MedievalAngloNormanSurnameInventory =
	new[]
	{
		"de-Bayeux", "de-Caen", "de-Falaise", "de-Rouen", "de-Dieppe", "de-Evreux",
		"de-Lisieux", "de-Coutances", "de-Avranches", "de-Alencon", "de-Argentan", "de-Domfront",
		"de-Mortain", "de-Gisors", "de-Vernon", "de-Les-Andelys", "de-Pont-Audemer", "de-Bec",
		"de-Jumieges", "de-Mont-Saint-Michel", "de-Cherbourg", "de-Barfleur", "de-Granville", "de-Saint-Lo",
		"de-Vire", "de-Sees", "de-Fecamp", "de-Caudebec", "de-Harfleur", "de-Honfleur",
		"de-Eu", "de-Aumale", "de-Neufchatel", "de-Gournay", "de-Beaumont", "de-Harcourt",
		"de-Tancarville", "de-Ivry", "de-Breteuil", "de-Conches", "de-Nonancourt", "de-Louviers",
		"de-Gaillon", "de-Pacy", "de-Bernay", "de-Brionne", "de-Lillebonne", "de-Bolbec",
		"de-Caux", "de-Bessin", "de-Cotentin", "de-Avranchin", "de-Hiemois", "de-Vexin",
		"de-Perche", "de-Paris", "de-Reims", "de-Chartres", "de-Orleans", "de-Tours",
		"de-Blois", "de-Champagne", "de-Brie", "de-Picardie", "de-Amiens", "de-Beauvais",
		"de-Senlis", "de-Soissons", "de-Laon", "de-Compiegne", "de-Saint-Denis", "de-Etampes",
		"de-Melun", "de-Meaux", "de-Provins", "de-Troyes", "de-Sens", "de-Auxerre",
		"de-Nevers", "de-Bourges", "de-Poitiers", "de-Limoges", "de-Angouleme", "de-Saintes",
		"de-Bordeaux", "de-Bayonne", "de-Toulouse", "de-Carcassonne", "de-Narbonne", "de-Montpellier",
		"de-Nimes", "de-Arles", "de-Avignon", "de-Marseille", "de-Aix", "de-Lyon",
		"de-Vienne", "de-Grenoble", "de-Valence", "de-Dijon", "Fitz-William", "Fitz-Robert",
		"Fitz-Richard", "Fitz-Hugh", "Fitz-Henry", "Fitz-Geoffrey", "Fitz-Gilbert", "Fitz-Walter",
		"Fitz-Roger", "Fitz-Ralph", "Fitz-Alan", "Fitz-Stephen", "Fitz-Simon", "Fitz-Peter",
		"Fitz-John", "Fitz-Thomas", "Fitz-Philip", "Fitz-Nicholas", "Fitz-Adam", "Fitz-Michael",
		"Fitz-Martin", "Fitz-Jordan", "Fitz-Hamon", "Fitz-Odo", "Fitz-Eustace", "Fitz-Baldwin",
		"Fitz-Reginald", "Fitz-Renaud", "Fitz-Gerard", "Fitz-Guy", "Fitz-Miles", "Fitz-Payne",
		"Fitz-Piers", "Fitz-Ranulf", "Fitz-Fulk", "Fitz-Theobald", "Fitz-Aubrey", "Fitz-Anselm",
		"Fitz-Arnulf", "Fitz-Bernard", "Fitz-Charles", "Fitz-David", "Fitz-Edmund", "Fitz-Edward",
		"Fitz-Everard", "Fitz-Francis", "Fitz-Gervase", "Fitz-Godfrey", "Fitz-Gregory", "Fitz-Herbert",
		"Fitz-Humphrey", "Fitz-Laurence", "Fitz-Luke", "Fitz-Matthew", "Fitz-Maurice", "Fitz-Oliver",
		"Fitz-Osbert", "Fitz-Oswin", "Fitz-Patrick", "Fitz-Raymond", "Fitz-Reynold", "Fitz-Samson",
		"Fitz-Serlo", "Fitz-Theodoric", "Fitz-Tristram", "Fitz-Warin", "Fitz-Waleran", "Marshal",
		"Steward", "Butler", "Chamberlain", "Constable", "Castellan", "Seneschal",
		"Treasurer", "Chancellor", "Clerk", "Chaplain", "Deacon", "Prior",
		"Abbot", "Canon", "Squire", "Knight", "Herald", "Forester",
		"Parker", "Hunter", "Falconer", "Archer", "Bowman", "Fletcher",
		"Armourer", "Smith", "Mason", "Carpenter", "Cooper", "Miller",
		"Baker", "Brewer"
	};

	private static readonly string[] MedievalEnglishSurnameInventory =
	new[]
	{
		"Abbott", "Ackerman", "Archer", "Armourer", "Baker", "Barker",
		"Baxter", "Beadle", "Bellfounder", "Bowman", "Bowyer", "Brewer",
		"Brewster", "Butcher", "Butler", "Carter", "Chandler", "Chapman",
		"Clerk", "Cook", "Cooper", "Cordwainer", "Corviser", "Cottar",
		"Cutler", "Dyer", "Falconer", "Farrier", "Fisher", "Fletcher",
		"Fowler", "Fuller", "Gardiner", "Glover", "Goldsmith", "Harper",
		"Hayward", "Hunter", "Joiner", "Lorimer", "Mason", "Mercer",
		"Miller", "Miner", "Parchmenter", "Parker", "Paver", "Ploughman",
		"Porter", "Potter", "Reeve", "Roper", "Saddler", "Sawyer",
		"Scrivener", "Shepherd", "Shipwright", "Shoemaker", "Skinner", "Smith",
		"Spicer", "Spinner", "Tailor", "Tanner", "Thatcher", "Tiler",
		"Turner", "Vintner", "Weaver", "Webber", "Wheeler", "Woodward",
		"Wright", "Wainwright", "Cartwright", "Wheelwright", "Arkwright", "Boatwright",
		"Millwright", "Plowwright", "Glasswright", "Plater", "Girdler", "Haberdasher",
		"Hosier", "Draper", "Woolman", "Wooler", "Shearman", "Sherman",
		"Tuckerman", "Walker", "Lister", "Litster", "Whitster", "Blacker",
		"Blower", "Foundryman", "Forgeman", "Hammerman", "Nailer", "Needler",
		"Pinner", "Pointer", "Spurrier", "Locksmith", "Gater", "Gaunter",
		"Pouchmaker", "Purser", "Salter", "Maltster", "Tapster", "Hosteller",
		"Innkeeper", "Ostler", "Groom", "Marshall", "Constable", "Bailiff",
		"Steward", "Chamberlain", "Page", "Squire", "Knight", "Herald",
		"Pursuivant", "Crier", "Messenger", "Courier", "Carterman", "Ferryman",
		"Waterman", "Fisherman", "Boatman", "Shipman", "Sailor", "Mariner",
		"Pilgrim", "Palmer", "Pardoner", "Sexton", "Sacristan", "Deacon",
		"Priest", "Bishop", "Canon", "Monk", "Friar", "Prior",
		"Abbot", "Chaplain", "Clerkson", "Scholar", "Schoolman", "Doctor",
		"Leech", "Surgeon", "Barber", "Apothecary", "Chandlerman", "Grocer",
		"Pepperer", "Spicerer", "Fruiterer", "Fishmonger", "Ironmonger", "Cheesemonger",
		"Cornmonger", "Woodmonger", "Clothier", "Tailorling", "Capper", "Hatter",
		"Furrier", "Fellmonger", "Currier", "Tannerman", "Soper", "Chandlerer",
		"Waxman", "Candler", "Broommaker", "Basketmaker", "Cooperage", "Turnerby",
		"Smithson", "Williamson", "Johnson", "Robertson", "Richardson", "Harrison",
		"Jackson", "Atwood", "Atwater", "Atbrook", "Atwell", "Atford",
		"Athill", "Atfield"
	};

	private static readonly string[] MedievalCarolingianToponymInventory =
	new[]
	{
		"of-Aachen", "of-Cologne", "of-Mainz", "of-Trier", "of-Worms", "of-Speyer",
		"of-Frankfurt", "of-Fulda", "of-Lorsch", "of-Corvey", "of-Paderborn", "of-Munster",
		"of-Utrecht", "of-Dorestad", "of-Nijmegen", "of-Liege", "of-Maastricht", "of-Verdun",
		"of-Metz", "of-Toul", "of-Strasbourg", "of-Basel", "of-Constance", "of-Reichenau",
		"of-St-Gall", "of-Salzburg", "of-Regensburg", "of-Passau", "of-Augsburg", "of-Freising",
		"of-Wurzburg", "of-Bamberg", "of-Erfurt", "of-Magdeburg", "of-Halberstadt", "of-Hildesheim",
		"of-Quedlinburg", "of-Goslar", "of-Lombardy", "of-Pavia", "of-Milan", "of-Verona",
		"of-Ravenna", "of-Bologna", "of-Lucca", "of-Pisa", "of-Rome", "of-Spoleto",
		"of-Benevento", "of-Friuli", "of-Carinthia", "of-Bavaria", "of-Saxony", "of-Thuringia",
		"of-Alemannia", "of-Austrasia", "of-Neustria", "of-Septimania", "of-Aquitaine", "of-Burgundy",
		"of-Provence", "of-Flanders", "of-Frisia", "of-Brittany", "of-Gascony", "of-Rhaetia",
		"of-Bayeux", "of-Caen", "of-Falaise", "of-Rouen", "of-Dieppe", "of-Evreux",
		"of-Lisieux", "of-Coutances", "of-Avranches", "of-Alencon", "of-Argentan", "of-Domfront",
		"of-Mortain", "of-Gisors", "of-Vernon", "of-Les-Andelys", "of-Pont-Audemer", "of-Bec",
		"of-Jumieges", "of-Mont-Saint-Michel", "of-Cherbourg", "of-Barfleur", "of-Granville", "of-Saint-Lo",
		"of-Vire", "of-Sees", "of-Fecamp", "of-Caudebec", "of-Harfleur", "of-Honfleur",
		"of-Eu", "of-Aumale", "of-Neufchatel", "of-Gournay", "of-Beaumont", "of-Harcourt",
		"of-Tancarville", "of-Ivry", "of-Breteuil", "of-Conches", "of-Nonancourt", "of-Louviers",
		"of-Gaillon", "of-Pacy", "of-Bernay", "of-Brionne", "of-Lillebonne", "of-Bolbec",
		"of-Caux", "of-Bessin", "of-Cotentin", "of-Avranchin", "of-Hiemois", "of-Vexin",
		"of-Perche", "of-Paris", "of-Reims", "of-Chartres", "of-Orleans", "of-Tours",
		"of-Blois", "of-Champagne", "of-Brie", "of-Picardie", "of-Amiens", "of-Beauvais",
		"of-Senlis", "of-Soissons", "of-Laon", "of-Compiegne", "of-Saint-Denis", "of-Etampes",
		"of-Melun", "of-Meaux", "of-Provins", "of-Troyes", "of-Sens", "of-Auxerre",
		"of-Nevers", "of-Bourges", "of-Poitiers", "of-Limoges", "of-Angouleme", "of-Saintes",
		"of-Bordeaux", "of-Bayonne", "of-Toulouse", "of-Carcassonne", "of-Narbonne", "of-Montpellier",
		"of-Nimes", "of-Arles", "of-Avignon", "of-Marseille", "of-Aix", "of-Lyon",
		"of-Vienne", "of-Grenoble", "of-Valence", "of-Dijon", "of-Autun", "of-Beaune",
		"of-Macon", "of-Chalon", "of-Besancon", "of-Langres", "of-Nancy", "of-Colmar",
		"of-Mulhouse", "of-Geneva", "of-Lausanne", "of-Namur", "of-Hainaut", "of-Brabant",
		"of-Ghent", "of-Bruges", "of-Ypres", "of-Lille", "of-Arras", "of-Cambrai",
		"of-Tournai", "of-Valenciennes", "of-Mons", "of-Boulogne", "of-Calais", "of-Saint-Omer",
		"of-Abbeville", "of-Ponthieu", "of-Le-Mans", "of-Angers", "of-Saumur", "of-Nantes",
		"of-Rennes", "of-Vannes"
	};

	private static readonly string[] MedievalCapetianToponymInventory =
	new[]
	{
		"de-Paris", "de-Reims", "de-Chartres", "de-Orleans", "de-Tours", "de-Blois",
		"de-Champagne", "de-Brie", "de-Picardie", "de-Amiens", "de-Beauvais", "de-Senlis",
		"de-Soissons", "de-Laon", "de-Compiegne", "de-Saint-Denis", "de-Etampes", "de-Melun",
		"de-Meaux", "de-Provins", "de-Troyes", "de-Sens", "de-Auxerre", "de-Nevers",
		"de-Bourges", "de-Poitiers", "de-Limoges", "de-Angouleme", "de-Saintes", "de-Bordeaux",
		"de-Bayonne", "de-Toulouse", "de-Carcassonne", "de-Narbonne", "de-Montpellier", "de-Nimes",
		"de-Arles", "de-Avignon", "de-Marseille", "de-Aix", "de-Lyon", "de-Vienne",
		"de-Grenoble", "de-Valence", "de-Dijon", "de-Autun", "de-Beaune", "de-Macon",
		"de-Chalon", "de-Besancon", "de-Langres", "de-Verdun", "de-Metz", "de-Toul",
		"de-Nancy", "de-Strasbourg", "de-Colmar", "de-Mulhouse", "de-Basel", "de-Geneva",
		"de-Lausanne", "de-Liege", "de-Namur", "de-Hainaut", "de-Brabant", "de-Flanders",
		"de-Ghent", "de-Bruges", "de-Ypres", "de-Lille", "de-Arras", "de-Cambrai",
		"de-Tournai", "de-Valenciennes", "de-Mons", "de-Boulogne", "de-Calais", "de-Saint-Omer",
		"de-Abbeville", "de-Ponthieu", "de-Le-Mans", "de-Angers", "de-Saumur", "de-Nantes",
		"de-Rennes", "de-Vannes", "de-Quimper", "de-Leon", "de-Cornouaille", "de-Dol",
		"de-Saint-Malo", "de-Fougeres", "de-Vitre", "de-Laval", "de-Mayenne", "de-Anjou",
		"de-Maine", "de-Touraine", "de-Normandy", "de-Brittany", "de-Burgundy", "de-Aquitaine",
		"de-Gascony", "de-Auvergne", "de-Languedoc", "de-Provence", "de-Dauphine", "de-Savoy",
		"de-Lorraine", "de-Alsace", "de-Berry", "de-Nivernais", "de-Bourbonnais", "de-Vermandois",
		"de-Valois", "de-Vendome", "de-Chinon", "de-Loches", "de-Amboise", "de-Chateaudun",
		"de-Mantes", "de-Pontoise", "de-Corbeil", "de-Montlhery", "de-Dreux", "de-Joigny",
		"de-Tonnerre", "de-Chatillon", "de-Semur", "de-Vezelay", "de-Cluny", "de-Paray",
		"de-Sancerre", "de-Issoudun", "de-Chateauroux", "de-Gueret", "de-Cahors", "de-Albi",
		"de-Rodez", "de-Agen", "du-Bois", "du-Pont", "du-Val", "du-Mont",
		"du-Bourg", "du-Moulin", "du-Chateau", "du-Marais", "du-Pre", "du-Clos",
		"du-Puits", "du-Roc", "du-Ruisseau", "du-Four", "du-Verger", "du-Champ",
		"du-Chemin", "du-Port", "du-Gue", "du-Tertre", "du-Buisson", "du-Mesnil",
		"du-Fresne", "du-Chene", "du-Houx", "de-la-Fontaine", "de-la-Roche", "de-la-Riviere",
		"de-la-Tour", "de-la-Porte", "de-la-Croix", "de-la-Ferte", "de-la-Haie", "de-la-Lande",
		"de-la-Vallee", "de-la-Prairie", "de-la-Grange", "de-la-Chapelle", "de-la-Motte", "de-la-Foret",
		"de-la-Cour", "de-la-Cote", "de-la-Fosse", "de-la-Source", "de-la-Bruyere", "des-Bois",
		"des-Ponts", "des-Vignes", "des-Champs", "des-Roches", "des-Fontaines", "des-Moulins",
		"des-Pres", "des-Tours", "des-Forges", "des-Hayes", "des-Monts", "des-Vallees",
		"des-Etangs", "des-Vergers"
	};

	private static readonly string[] MedievalGermanSurnameInventory =
	new[]
	{
		"Muller", "Schmidt", "Schneider", "Fischer", "Weber", "Meyer",
		"Wagner", "Becker", "Hoffmann", "Schulz", "Schaefer", "Koch",
		"Bauer", "Richter", "Klein", "Wolf", "Schroder", "Neumann",
		"Schwarz", "Zimmermann", "Braun", "Kruger", "Hofmann", "Hartmann",
		"Lange", "Schmitt", "Werner", "Schmitz", "Krause", "Meier",
		"Lehmann", "Schmid", "Schulze", "Maier", "Kohler", "Herrmann",
		"Konig", "Walter", "Mayer", "Huber", "Kaiser", "Fuchs",
		"Peters", "Lang", "Scholz", "Moller", "Weiss", "Jung",
		"Hahn", "Schubert", "Vogel", "Friedrich", "Keller", "Gunther",
		"Frank", "Berger", "Winkler", "Roth", "Beck", "Lorenz",
		"Baumann", "Fricke", "Albrecht", "Simon", "Ludwig", "Bohm",
		"Winter", "Kraus", "Martin", "Schumacher", "Kramer", "Vogt",
		"Stein", "Jager", "Otto", "Sommer", "Gross", "Seidel",
		"Heinrich", "Brandt", "Haas", "Schreiber", "Graf", "Schulte",
		"Dietrich", "Ziegler", "Kuhn", "Kuehn", "Pohl", "Engel",
		"Horn", "Busch", "Bergmann", "Thomas", "Voigt", "Sauer",
		"Arnold", "Wolff", "Pfeiffer", "Gerber", "Seiler", "Sattler",
		"Wagnerer", "Drechsler", "Tischler", "Schreiner", "Zimmerer", "Maurer",
		"Steinmetz", "Dachdecker", "Glaser", "Hafner", "Topfer", "Bottcher",
		"Fassbinder", "Kupferer", "Zinngiesser", "Goldschmied", "Silberschmied", "Eisenhauer",
		"Hammerschmidt", "Messerschmidt", "Klingenschmidt", "Nagler", "Nadler", "Gurtler",
		"Riemer", "Schuhmacher", "Schuster", "Lederer", "Gerbermann", "Walker",
		"Tucher", "Webermann", "Spinner", "Farber", "Bleicher", "Fuller",
		"Loder", "Schneiderlein", "Kurschner", "Pelzner", "Hutmacher", "Beutler",
		"Taschner", "Seifensieder", "Kerzner", "Mullerlein", "Backer", "Metzger",
		"Fleischer", "Kochmann", "Brauer", "Maltzer", "Winzer", "Gartner",
		"Ackermann", "Pfluger", "Saman", "Bauerlein", "Schafer", "Hirt",
		"Kuhhirt", "Schweinehirt", "Jagermeister", "Falkner", "Vogler", "Fischerling",
		"Schiffer", "Fahrmann", "Fuhrmann", "Karrer", "Wagnerling", "Radmacher",
		"Stellmacher", "Sattelmacher", "Zaumner", "Sporer", "Waffner", "Armbruster",
		"Bogner", "Pfeilmacher", "Schildner", "Plattner", "Harnischer", "Schlosser",
		"Torwart", "Wachter", "Turmer", "Kuster", "Mesner", "Glockner",
		"Schreiberling", "Notar", "Schulmeister", "Meister", "Geselle", "Knecht",
		"Diener", "Kammerer"
	};

	private static readonly string[] MedievalAndalusiNisbaInventory =
	new[]
	{
		"al-Qurtubi", "al-Ishbili", "al-Gharnati", "al-Tulaytuli", "al-Malaqi", "al-Mursi",
		"al-Balansi", "al-Jayyani", "al-Mariyyi", "al-Jaziri", "al-Shatibi", "al-Rundi",
		"al-Bajji", "al-Yaburi", "al-Saraqusti", "al-Turtushi", "al-Laridi", "al-Batalyawsi",
		"al-Uqbani", "al-Qalati", "al-Qarmuni", "al-Labli", "al-Lushbuni", "al-Shantarini",
		"al-Shalabini", "al-Fari", "al-Baji", "al-Ushbuni", "al-Burjasi", "al-Qasri",
		"al-Madriti", "al-Wadi-Ashii", "al-Bayyasi", "al-Ubbadhi", "al-Qurashi", "al-Tudmiri",
		"al-Basti", "al-Lurqi", "al-Shadhuni", "al-Marbalati", "al-Jazirati", "al-Khadrawi",
		"al-Qadisi", "al-Majriti", "al-Talabirati", "al-Zamuri", "al-Salamanti", "al-Samuri",
		"al-Asturqi", "al-Layuni", "al-Burgushi", "al-Najari", "al-Bilbilitani", "al-Tarazuni",
		"al-Washqi", "al-Qalansawi", "al-Dani", "al-Sabti", "al-Tanjawi", "al-Fasi",
		"al-Miknasi", "al-Marrakushi", "al-Tilimsani", "al-Qayrawani", "al-Tunisi", "al-Bijayi",
		"al-Qusantini", "al-Sijilmasi", "al-Ghadamisi", "al-Andalusi", "al-Gharbi", "al-Sharqi",
		"al-Baezi", "al-Ubeda'i", "al-Arjuni", "al-Marti", "al-Qastallani", "al-Madawwari",
		"al-Algecirasi", "al-Tarifi", "al-Sharishi", "al-Arqusi", "al-Luqi", "al-Istijji",
		"al-Qabri", "al-Antaqiri", "al-Iznajari", "al-Lawshi", "al-Ilbiri", "al-Guadixi",
		"al-Bazati", "al-Orihuwali", "al-Alikanti", "al-Daniyawi", "al-Sagunti", "al-Burriani",
		"al-Barsaluni", "al-Jiruni", "al-Tarrakuni", "al-Washqari", "al-Barbastri", "al-Laridani",
		"al-Tudili", "al-Hamrawi", "al-Munastiri", "al-Marbali", "al-Fuengirolawi", "al-Suhayli",
		"al-Balishi", "al-Nerji", "al-Almunakkabi", "al-Guadisi", "al-Hashimi", "al-Ansari",
		"al-Kinani", "al-Tamimi", "al-Thaqafi", "al-Azdi", "al-Lakhmi", "al-Judhami",
		"al-Kalbi", "al-Tanukhi", "al-Hamdani", "al-Kindi", "al-Makhzumi", "al-Umari",
		"al-Bakri", "al-Husayni", "al-Hasani", "al-Ayyubi", "al-Abbasi", "al-Alawi",
		"al-Jafari", "al-Usmani", "al-Sulami", "al-Hilali", "al-Sulaymi", "al-Khazraji",
		"al-Awsi", "al-Qaysi", "al-Muradi", "al-Maafiri", "al-Ghassani", "al-Nasri",
		"al-Kilabi", "al-Uqayli", "al-Juhani", "al-Harbi", "al-Hudhali", "al-Damri",
		"al-Fihri", "al-Haddad", "al-Najjar", "al-Khayyat", "al-Dabbagh", "al-Attar",
		"al-Bazzaz", "al-Sarraj", "al-Warraq", "al-Katib", "al-Qadi", "al-Muallim",
		"al-Tabib", "al-Kahhal", "al-Sayrafi", "al-Sabbagh", "al-Qassab", "al-Khabbaz",
		"al-Tahhan", "al-Fakhkhar", "al-Zajjaj", "al-Nahhas", "al-Saffar", "al-Hariri",
		"al-Qattan", "al-Labbad", "al-Hallaj", "al-Saqqaf", "al-Banna", "al-Haffar",
		"al-Jawhari", "al-Adib", "al-Shair", "al-Nahwi", "al-Lughawi", "al-Hisabi",
		"al-Falaki", "al-Muarrikh", "al-Wazzan", "al-Rassam", "al-Muqri", "al-Zahrawi",
		"al-Ghafiqi", "al-Idrisi", "al-Maqqari", "al-Qurtubani", "al-Ilibiri", "al-Madini",
		"al-Wadiyasi", "al-Zayyani"
	};

	private static readonly string[] MedievalAbbasidNisbaInventory =
	new[]
	{
		"al-Baghdadi", "al-Basri", "al-Kufi", "al-Samarrai", "al-Mawsili", "al-Wasiti",
		"al-Harrani", "al-Raqqi", "al-Anbari", "al-Hiti", "al-Tikriti", "al-Ukbari",
		"al-Madaini", "al-Nahrawani", "al-Dujayli", "al-Dinawari", "al-Jazari", "al-Irbili",
		"al-Karkhi", "al-Rusafi", "al-Kazimi", "al-Bataihi", "al-Ahwazi", "al-Susi",
		"al-Tustari", "al-Jundishapuri", "al-Isfahani", "al-Shirazi", "al-Farisi", "al-Tabrizi",
		"al-Qazwini", "al-Razi", "al-Qummi", "al-Kashani", "al-Hamadani", "al-Nahawandi",
		"al-Jurjani", "al-Tabari", "al-Daylami", "al-Gilani", "al-Nishapuri", "al-Tusi",
		"al-Marwazi", "al-Harawi", "al-Balkhi", "al-Bukhari", "al-Samarqandi", "al-Tirmidhi",
		"al-Khujandi", "al-Khwarizmi", "al-Farghani", "al-Kashghari", "al-Sijistani", "al-Kirmani",
		"al-Yazdi", "al-Bayhaqi", "al-Sabzawari", "al-Badakhshi", "al-Ghazni", "al-Kabuli",
		"al-Dimashqi", "al-Halabi", "al-Himsi", "al-Hamawi", "al-Misri", "al-Hijazi",
		"al-Makki", "al-Madani", "al-Yamani", "al-Sanani", "al-Adeni", "al-Hadrami",
		"al-Bahrayni", "al-Umani", "al-Najdi", "al-Taifi", "al-Yamami", "al-Hajari",
		"al-Baladi", "al-Haddad", "al-Najjar", "al-Khayyat", "al-Dabbagh", "al-Attar",
		"al-Bazzaz", "al-Sarraj", "al-Warraq", "al-Katib", "al-Qadi", "al-Muallim",
		"al-Tabib", "al-Kahhal", "al-Sayrafi", "al-Sabbagh", "al-Qassab", "al-Khabbaz",
		"al-Tahhan", "al-Fakhkhar", "al-Zajjaj", "al-Nahhas", "al-Saffar", "al-Hariri",
		"al-Qattan", "al-Labbad", "al-Hallaj", "al-Saqqaf", "al-Banna", "al-Haffar",
		"al-Jawhari", "al-Habbal", "al-Bawwab", "al-Hammami", "al-Farran", "al-Mallah",
		"al-Bahhar", "al-Jammal", "al-Rakkab", "al-Baytar", "al-Saati", "al-Qurashi",
		"al-Hashimi", "al-Ansari", "al-Kinani", "al-Tamimi", "al-Thaqafi", "al-Azdi",
		"al-Lakhmi", "al-Judhami", "al-Kalbi", "al-Tanukhi", "al-Hamdani", "al-Kindi",
		"al-Makhzumi", "al-Umari", "al-Bakri", "al-Husayni", "al-Hasani", "al-Ayyubi",
		"al-Abbasi", "al-Alawi", "al-Jafari", "al-Usmani", "al-Sulami", "al-Hilali",
		"al-Sulaymi", "al-Khazraji", "al-Awsi", "al-Qaysi", "al-Muradi", "al-Maafiri",
		"al-Ghassani", "al-Nasri", "al-Kilabi", "al-Uqayli", "al-Juhani", "al-Harbi",
		"al-Hudhali", "al-Damri", "al-Fihri", "al-Shafii", "al-Hanafi", "al-Maliki",
		"al-Hanbali", "al-Ashari", "al-Maturidi", "al-Sufi", "al-Qadiri", "al-Rifai",
		"al-Shadhili", "al-Badawi", "al-Dasuqi", "al-Ahmadi", "al-Khalwati", "al-Mawlawi",
		"al-Zahiri", "al-Salihi", "al-Nasiri", "al-Ashrafi", "al-Zayni", "al-Nuri",
		"al-Shamsi", "al-Badri", "al-Fakhri", "al-Jamali", "al-Kamali", "al-Saifi",
		"al-Izzati", "al-Majdi", "al-Taqawi", "al-Burji", "al-Bahri", "al-Mansuri",
		"al-Muayyadi", "al-Inali", "al-Jaqmaqi", "al-Qaytbai", "al-Dawadari", "al-Ustadar",
		"al-Khurasani", "al-Iraqi"
	};

	private static readonly string[] MedievalFatimidNisbaInventory =
	new[]
	{
		"al-Qahiri", "al-Misri", "al-Fustati", "al-Iskandari", "al-Fayyumi", "al-Saidi",
		"al-Dimyati", "al-Tinnisi", "al-Manfaluti", "al-Suyuti", "al-Aswani", "al-Qusi",
		"al-Akhmimi", "al-Usyuti", "al-Bahnasi", "al-Buhairi", "al-Jizawi", "al-Minufi",
		"al-Qalyubi", "al-Mahallawi", "al-Simannudi", "al-Daqahli", "al-Bilbaysi", "al-Rashidi",
		"al-Barqawi", "al-Siwawi", "al-Nubi", "al-Ifriqi", "al-Qayrawani", "al-Mahdawi",
		"al-Tunisi", "al-Susi", "al-Sfaxi", "al-Qafsi", "al-Jaridi", "al-Bijayi",
		"al-Qusantini", "al-Zabidi", "al-Tripolitani", "al-Tarabulusi", "al-Barqai", "al-Maghribi",
		"al-Fasi", "al-Marrakushi", "al-Tilimsani", "al-Taharti", "al-Sijilmasi", "al-Darati",
		"al-Ghadamisi", "al-Warjlani", "al-Siqilli", "al-Balarmiti", "al-Mazini", "al-Sarqusi",
		"al-Qalawri", "al-Maliti", "al-Qabisi", "al-Dimashqi", "al-Halabi", "al-Himsi",
		"al-Hamawi", "al-Qudsi", "al-Ramli", "al-Ghazzi", "al-Asqalani", "al-Akkawi",
		"al-Saydawi", "al-Suri", "al-Beiruti", "al-Hijazi", "al-Makki", "al-Madani",
		"al-Yamani", "al-Sanani", "al-Adeni", "al-Hadrami", "al-Sudani", "al-Bajawi",
		"al-Kanemi", "al-Haddad", "al-Najjar", "al-Khayyat", "al-Dabbagh", "al-Attar",
		"al-Bazzaz", "al-Sarraj", "al-Warraq", "al-Katib", "al-Qadi", "al-Muallim",
		"al-Tabib", "al-Kahhal", "al-Sayrafi", "al-Sabbagh", "al-Qassab", "al-Khabbaz",
		"al-Tahhan", "al-Fakhkhar", "al-Zajjaj", "al-Nahhas", "al-Saffar", "al-Hariri",
		"al-Qattan", "al-Labbad", "al-Hallaj", "al-Saqqaf", "al-Banna", "al-Haffar",
		"al-Jawhari", "al-Habbal", "al-Bawwab", "al-Hammami", "al-Farran", "al-Mallah",
		"al-Bahhar", "al-Jammal", "al-Rakkab", "al-Baytar", "al-Saati", "al-Qurashi",
		"al-Hashimi", "al-Ansari", "al-Kinani", "al-Tamimi", "al-Thaqafi", "al-Azdi",
		"al-Lakhmi", "al-Judhami", "al-Kalbi", "al-Tanukhi", "al-Hamdani", "al-Kindi",
		"al-Makhzumi", "al-Umari", "al-Bakri", "al-Husayni", "al-Hasani", "al-Ayyubi",
		"al-Abbasi", "al-Alawi", "al-Jafari", "al-Usmani", "al-Sulami", "al-Hilali",
		"al-Sulaymi", "al-Khazraji", "al-Awsi", "al-Qaysi", "al-Muradi", "al-Maafiri",
		"al-Ghassani", "al-Nasri", "al-Kilabi", "al-Uqayli", "al-Juhani", "al-Harbi",
		"al-Hudhali", "al-Damri", "al-Fihri", "al-Shafii", "al-Hanafi", "al-Maliki",
		"al-Hanbali", "al-Ashari", "al-Maturidi", "al-Sufi", "al-Qadiri", "al-Rifai",
		"al-Shadhili", "al-Badawi", "al-Dasuqi", "al-Ahmadi", "al-Khalwati", "al-Mawlawi",
		"al-Zahiri", "al-Salihi", "al-Nasiri", "al-Ashrafi", "al-Zayni", "al-Nuri",
		"al-Shamsi", "al-Badri", "al-Fakhri", "al-Jamali", "al-Kamali", "al-Saifi",
		"al-Izzati", "al-Majdi", "al-Taqawi", "al-Burji", "al-Bahri", "al-Mansuri",
		"al-Muayyadi", "al-Inali", "al-Jaqmaqi", "al-Qaytbai", "al-Dawadari", "al-Ustadar",
		"al-Kutami", "al-Ziridi"
	};

	private static readonly string[] MedievalSeljukNisbaInventory =
	new[]
	{
		"al-Dimashqi", "al-Halabi", "al-Hamawi", "al-Himsi", "al-Mawsili", "al-Jaziri",
		"al-Harrani", "al-Raqqi", "al-Amidi", "al-Mardini", "al-Ruhawi", "al-Malati",
		"al-Ayntabi", "al-Antaki", "al-Tarusi", "al-Adhani", "al-Marashi", "al-Qaysari",
		"al-Qunawi", "al-Siwasi", "al-Aqsarayi", "al-Anqarawi", "al-Amasi", "al-Tuqati",
		"al-Kastamuni", "al-Sinubi", "al-Trabzuni", "al-Arzarumi", "al-Akhlati", "al-Vani",
		"al-Bidlisi", "al-Hakkari", "al-Rumi", "al-Turki", "al-Kurdi", "al-Daylami",
		"al-Azari", "al-Tabrizi", "al-Ardabili", "al-Maraghi", "al-Khuyi", "al-Salmasi",
		"al-Urmavi", "al-Qazwini", "al-Razi", "al-Isfahani", "al-Kashani", "al-Hamadani",
		"al-Nahawandi", "al-Shirazi", "al-Farisi", "al-Kirmani", "al-Yazdi", "al-Khurasani",
		"al-Nishapuri", "al-Tusi", "al-Marwazi", "al-Harawi", "al-Balkhi", "al-Bukhari",
		"al-Samarqandi", "al-Tirmidhi", "al-Khwarizmi", "al-Farghani", "al-Kashghari", "al-Sijistani",
		"al-Ghazni", "al-Kabuli", "al-Baghdadi", "al-Basri", "al-Kufi", "al-Wasiti",
		"al-Tikriti", "al-Irbili", "al-Dinawari", "al-Shahrazuri", "al-Aleppine", "al-Anatoli",
		"al-Haddad", "al-Najjar", "al-Khayyat", "al-Dabbagh", "al-Attar", "al-Bazzaz",
		"al-Sarraj", "al-Warraq", "al-Katib", "al-Qadi", "al-Muallim", "al-Tabib",
		"al-Kahhal", "al-Sayrafi", "al-Sabbagh", "al-Qassab", "al-Khabbaz", "al-Tahhan",
		"al-Fakhkhar", "al-Zajjaj", "al-Nahhas", "al-Saffar", "al-Hariri", "al-Qattan",
		"al-Labbad", "al-Hallaj", "al-Saqqaf", "al-Banna", "al-Haffar", "al-Jawhari",
		"al-Habbal", "al-Bawwab", "al-Hammami", "al-Farran", "al-Mallah", "al-Bahhar",
		"al-Jammal", "al-Rakkab", "al-Baytar", "al-Saati", "al-Oghuzi", "al-Qiniqi",
		"al-Qayi", "al-Bayati", "al-Afshari", "al-Salghuri", "al-Artuqi", "al-Danishmandi",
		"al-Saltuqi", "al-Manguchi", "al-Zangi", "al-Ayyubi", "al-Mamluki", "al-Qipchaqi",
		"al-Turkumani", "al-Karluki", "al-Khalaji", "al-Ghuzzi", "al-Tatari", "al-Barlas",
		"al-Jalayiri", "al-Naimani", "al-Qongirati", "al-Manghiti", "al-Suldusi", "al-Begdili",
		"al-Yaziri", "al-Dodurghai", "al-Chepni", "al-Kangli", "al-Yaghmai", "al-Qarluqi",
		"al-Uqayli", "al-Hilali", "al-Kilabi", "al-Hamdani", "al-Tamimi", "al-Qaysi",
		"al-Azdi", "al-Atabaki", "al-Amiri", "al-Sipahi", "al-Ghulami", "al-Hajibi",
		"al-Dawadari", "al-Ustadhdari", "al-Shihnagi", "al-Arizi", "al-Jandari", "al-Silahdari",
		"al-Bunduqdari", "al-Akhuri", "al-Jamamdari", "al-Tashtdari", "al-Saqi", "al-Wazir",
		"al-Mudarris", "al-Faqih", "al-Khatib", "al-Muqri", "al-Muhaddith", "al-Sufi",
		"al-Qadiri", "al-Rifai", "al-Suhrawardi", "al-Kubrawi", "al-Yasawi", "al-Hanafi",
		"al-Shafii", "al-Hanbali", "al-Ashari", "al-Maturidi", "al-Nizami", "al-Mustansiri",
		"al-Salihi", "al-Nasiri", "al-Muzaffari", "al-Kamili", "al-Adili", "al-Ashrafi",
		"al-Maliki", "al-Zahiri"
	};

	private static readonly MedievalPeriodSeed[] MedievalPeriodSeeds =
	new MedievalPeriodSeed[]
	{
		new(
			Key: "Early Anglo-Saxon",
			NameCultureName: "Medieval Early Anglo-Saxon",
			EthnicityName: "Early Anglo-Saxon",
			EthnicGroup: "Germanic",
			EthnicSubgroup: "Early Anglo-Saxon England",
			CultureName: "Medieval Early Anglo-Saxon (c. 800)",
			NameForm: MedievalPeriodNameForm.GivenToponym,
			NameRegex: @"^(?<birthname>[\w'-]{2,}) (?<toponym>(?:of|at|by|from)-[\w'-]{2,})$",
			ReferenceAnchor: "c. 800",
			EthnicityDescription:
				"The Anglo-Saxons are the Old English-speaking people of lowland England, descended from the settlers and " +
				"kingdoms established after Roman rule. They recognise kin, kingdom, local district and lordship, while " +
				"common law, compensation customs and the Christian Church bind communities from Wessex and Mercia to " +
				"Northumbria and Kent. Speech, inherited land and descent from a known kindred are the clearest marks of " +
				"belonging.",
			CultureDescription:
				"Early Anglo-Saxon culture is centred on royal and aristocratic halls, free and unfree farming households, " +
				"monasteries, market settlements and warbands. Oath, lordship, kinship, compensation, hospitality and " +
				"church patronage regulate status and obligation.",
			PersonalNameDescription:
				"Choose an Old English personal name. Dithematic compounds are common, using elements such as Aethel-, " +
				"Ead-, Wulf-, Beorht-, -ric, -wine and -gifu; Christian names also appear.",
			SecondNameDescription:
				"Enter a locative or descriptive byname beginning of-, at-, by- or from-. It identifies a kingdom, town, " +
				"monastery, estate or landscape feature and is not a hereditary surname.",
			ThirdNameDescription: "",
			MinimumSecondNameCount: 200,
			MinimumThirdNameCount: 0,
			BloodModel: "Majority O Minor A",
			SkinProfile: "fair_skin",
			HairProfile: "brown_blonde_grey_hair",
			EyeProfile: "brown_green_blue_eyes",
			MaleGivenNames: new[]
			{
				"Aelfgar", "Aelfheah", "Aelfhere", "Aelfmaer", "Aelfred", "Aelfric",
				"Aelfsige", "Aelfstan", "Aethelbald", "Aethelberht", "Aethelfrith", "Aethelheard",
				"Aethelhelm", "Aethelred", "Aethelric", "Aethelstan", "Aethelwald", "Aethelweard",
				"Aethelwine", "Beornred", "Beornwulf", "Cenred", "Cenric", "Ceol",
				"Ceolred", "Ceolwulf", "Cuthbert", "Cyneheard", "Cyneric", "Cynewulf",
				"Eadberht", "Eadgar", "Eadmund", "Eadred", "Eadric", "Eadweard",
				"Eadwig", "Ealhred", "Ecgberht", "Hereward", "Leofric", "Leofwine",
				"Offa", "Osberht", "Osred", "Osric", "Oswald", "Sigeberht",
				"Wulfhere", "Wulfstan"
			},
			FemaleGivenNames: new[]
			{
				"Aelfgifu", "Aelflaed", "Aelfswith", "Aelfthrith", "Aethelburh", "Aethelflaed",
				"Aethelgifu", "Aethelgyth", "Aethelhild", "Aethelthryth", "Beornflaed", "Ceolburh",
				"Cuthburh", "Cyneburh", "Cyneswith", "Eadburh", "Eadflaed", "Eadgifu",
				"Eadgyth", "Ealhswith", "Frithugyth", "Godgifu", "Godgyth", "Hereswith",
				"Leofgifu", "Leofrun", "Mildburh", "Mildgyth", "Mildthryth", "Osgifu",
				"Osburh", "Osthryth", "Saethryth", "Sexburh", "Sigeburh", "Wulfhild",
				"Wulfrun", "Wynflaed", "Wulfwaru", "Aebbe", "Balthild", "Berhtgyth",
				"Cwenburh", "Eormengyth", "Hild", "Hildelith", "Leoflaed", "Oslafa",
				"Ricthryth", "Wihtburh"
			},
			MaleSecondNames: MedievalBritishToponymInventory,
			FemaleSecondNames: MedievalBritishToponymInventory,
			ThirdNames: Array.Empty<string>()
		),
		new(
			Key: "Anglo-Danish",
			NameCultureName: "Medieval Anglo-Danish",
			EthnicityName: "Anglo-Danish English",
			EthnicGroup: "North Sea Germanic",
			EthnicSubgroup: "Anglo-Danish England",
			CultureName: "Medieval Anglo-Danish (c. 950)",
			NameForm: MedievalPeriodNameForm.GivenToponym,
			NameRegex: @"^(?<birthname>[\w'-]{2,}) (?<toponym>(?:of|at|by|from)-[\w'-]{2,})$",
			ReferenceAnchor: "c. 950",
			EthnicityDescription:
				"The Anglo-Danish English are the people of the Danelaw and neighbouring English lands where Old English " +
				"and Old Norse settlers have become neighbours, kin and fellow Christians. They define themselves through " +
				"local law, town or wapentake, family descent and allegiance to English or Scandinavian lords. Mixed " +
				"speech, naming and custom distinguish them from both earlier Anglo-Saxons and newcomers from overseas.",
			CultureDescription:
				"Anglo-Danish culture grows in estates, boroughs, ports, monasteries and military households around York, " +
				"Lincoln and the eastern shires. English and Scandinavian family memory, Christian worship, trade, " +
				"landholding and local law combine in a recognisable North Sea society.",
			PersonalNameDescription:
				"Choose an Old English or Old Norse personal name. Both traditions use compound elements, and mixed " +
				"households may favour names from either side of the North Sea.",
			SecondNameDescription:
				"Enter a place or origin byname beginning of-, at-, by- or from-. It identifies a town, shire, wapentake, " +
				"port or landscape rather than a fixed family surname.",
			ThirdNameDescription: "",
			MinimumSecondNameCount: 200,
			MinimumThirdNameCount: 0,
			BloodModel: "Majority O Minor A",
			SkinProfile: "fair_skin",
			HairProfile: "brown_blonde_grey_hair",
			EyeProfile: "brown_green_blue_eyes",
			MaleGivenNames: new[]
			{
				"Aelfgar", "Aelfric", "Aethelred", "Aethelstan", "Alfwold", "Anlaf",
				"Arnketil", "Asbjorn", "Beorn", "Beornulf", "Cnut", "Colgrim",
				"Eadgar", "Eadmund", "Eadric", "Eadweard", "Eadwine", "Eric",
				"Fastolf", "Gamal", "Godric", "Godwine", "Grim", "Grimketil",
				"Guthfrith", "Guthmund", "Halfdan", "Harold", "Harthacnut", "Hereward",
				"Hrafn", "Ketil", "Leofric", "Leofwine", "Orm", "Osgod",
				"Osulf", "Ragnald", "Siward", "Skuli", "Stigand", "Swein",
				"Thorkell", "Thurbrand", "Tofi", "Tostig", "Ulf", "Ulfketil",
				"Wulfstan", "Wulfric"
			},
			FemaleGivenNames: new[]
			{
				"Aelfgifu", "Aelflaed", "Aelfswith", "Aethelflaed", "Aethelgifu", "Astrid",
				"Aud", "Bothild", "Estrid", "Freydis", "Frithugyth", "Gerd",
				"Githa", "Godgifu", "Godgyth", "Gunhild", "Gyda", "Harthacnuta",
				"Helga", "Holmfrid", "Ingrid", "Ragnhild", "Sigrid", "Svanhild",
				"Thora", "Thyra", "Tola", "Tove", "Ulfhild", "Yngvild",
				"Eadgifu", "Eadgyth", "Edith", "Elfgiva", "Emma", "Estrith",
				"Goda", "Gunnhild", "Leofgifu", "Leofrun", "Osgifu", "Wulfrun",
				"Wynflaed", "Asa", "Bergljot", "Gudrun", "Hallfrid", "Ragna",
				"Runa", "Signy"
			},
			MaleSecondNames: MedievalBritishToponymInventory,
			FemaleSecondNames: MedievalBritishToponymInventory,
			ThirdNames: Array.Empty<string>()
		),
		new(
			Key: "Norse / Viking Age",
			NameCultureName: "Medieval Norse",
			EthnicityName: "Viking-Age Norse",
			EthnicGroup: "Nordic",
			EthnicSubgroup: "Viking-Age Scandinavia",
			CultureName: "Medieval Norse (c. 950)",
			NameForm: MedievalPeriodNameForm.GivenPatronym,
			NameRegex: @"^(?<birthname>[\w'-]{2,}) (?<patronym>[\w'-]+(?:sson|dottir))$",
			ReferenceAnchor: "c. 950",
			EthnicityDescription:
				"The Norse are the Old Norse-speaking people of Scandinavia and its overseas settlements. They place " +
				"themselves through farm and district, paternal and maternal kin, legal assembly, cult or church, and " +
				"allegiance to a chieftain or king. Seafaring, fosterage, feud settlement, gift exchange and remembered " +
				"genealogy are central signs of Norse belonging.",
			CultureDescription:
				"Viking-Age Norse culture encompasses farm households, fishing and craft settlements, market towns, " +
				"trading or raiding crews, legal assemblies and elite retinues. Reputation, hospitality, gift exchange, " +
				"slavery, feud and the spread of Christianity shape social life.",
			PersonalNameDescription:
				"Choose a single Old Norse personal name. Compounds using elements such as Thor-, Arn-, Gud-, -bjorn, " +
				"-geir, -frid and -hild are common.",
			SecondNameDescription:
				"Form the patronym from the father's personal name: masculine forms end -sson and feminine forms end " +
				"-dottir. The element changes in each generation and is not a hereditary family surname.",
			ThirdNameDescription: "",
			MinimumSecondNameCount: 1,
			MinimumThirdNameCount: 0,
			BloodModel: "A Dominant",
			SkinProfile: "fair_skin",
			HairProfile: "blonde_red_grey_hair",
			EyeProfile: "brown_green_blue_eyes",
			MaleGivenNames: new[]
			{
				"Agnar", "Alfarin", "Anund", "Arinbjorn", "Arnfinn", "Arngeir",
				"Arnkel", "Arnmod", "Asbjorn", "Asgeir", "Asgrim", "Audun",
				"Bard", "Bersi", "Bjorn", "Bolli", "Brand", "Brynjolf",
				"Egil", "Einar", "Eirik", "Finn", "Flosi", "Freystein",
				"Geir", "Gellir", "Gisli", "Grim", "Gudmund", "Gunnar",
				"Gunnlaug", "Halfdan", "Hall", "Hallvard", "Harald", "Hauk",
				"Helgi", "Hjalti", "Hrafn", "Hrolf", "Ingolf", "Ivar",
				"Ketil", "Knut", "Leif", "Njal", "Olaf", "Orm",
				"Sigurd", "Thorkell"
			},
			FemaleGivenNames: new[]
			{
				"Alof", "Arnbjorg", "Asa", "Asdis", "Asfrid", "Astrid",
				"Aud", "Bergljot", "Bodil", "Bothild", "Brynhild", "Dalla",
				"Estrid", "Freydis", "Geirlaug", "Gerd", "Grima", "Groa",
				"Gudrid", "Gudrun", "Gunhild", "Hallbera", "Hallfrid", "Helga",
				"Herdis", "Hild", "Holmfrid", "Ingeborg", "Ingrid", "Jorunn",
				"Ragnhild", "Rannveig", "Runa", "Sigrid", "Signy", "Sigrun",
				"Solveig", "Svanhild", "Thora", "Thorbjorg", "Thordis", "Thurid",
				"Thyra", "Tola", "Torunn", "Tove", "Ulfhild", "Unn",
				"Vigdis", "Yngvild"
			},
			MaleSecondNames: new[]
			{
				"Agnarsson", "Alfarinsson", "Anundsson", "Arnbjornsson", "Arngeirsson", "Asbjornsson",
				"Asgeirsson", "Audunsson", "Bardsson", "Bjornsson", "Brandsson", "Brynjolfsson",
				"Egilsson", "Einarsson", "Eiriksson", "Geirsson", "Grimsson", "Gudmundsson",
				"Gunnarsson", "Halfdansson", "Haraldsson", "Helgisson", "Hrafnsson", "Ivarsson",
				"Ketilsson", "Leifsson", "Olafsson", "Ormsson", "Sigurdsson", "Thorkelsson"
			},
			FemaleSecondNames: new[]
			{
				"Agnarsdottir", "Alfarinsdottir", "Anundsdottir", "Arnbjornsdottir", "Arngeirsdottir", "Asbjornsdottir",
				"Asgeirsdottir", "Audunsdottir", "Bardsdottir", "Bjornsdottir", "Brandsdottir", "Brynjolfsdottir",
				"Egilsdottir", "Einarsdottir", "Eiriksdottir", "Geirsdottir", "Grimsdottir", "Gudmundsdottir",
				"Gunnarsdottir", "Halfdansdottir", "Haraldsdottir", "Helgadottir", "Hrafnsdottir", "Ivarsdottir",
				"Ketilsdottir", "Leifsdottir", "Olafsdottir", "Ormsdottir", "Sigurdsdottir", "Thorkelsdottir"
			},
			ThirdNames: Array.Empty<string>()
		),
		new(
			Key: "Norman",
			NameCultureName: "Medieval Norman",
			EthnicityName: "Conquest-Era Norman",
			EthnicGroup: "Western European",
			EthnicSubgroup: "Ducal Normandy",
			CultureName: "Medieval Norman (c. 1066)",
			NameForm: MedievalPeriodNameForm.GivenToponym,
			NameRegex: @"^(?<birthname>[\w'-]{2,}) (?<toponym>(?:de|du|des|de-la)-[\w'-]{2,})$",
			ReferenceAnchor: "c. 1066",
			EthnicityDescription:
				"The Normans are the French-speaking people of Normandy, shaped by Frankish society and the descendants of " +
				"Scandinavian settlers. They define themselves through the duchy, local lordship, parish and monastery, " +
				"and descent from houses whose lands spread across the Channel and Mediterranean. Military reputation, " +
				"ducal loyalty and a distinct Norman form of French mark them among neighbouring peoples.",
			CultureDescription:
				"Norman culture is organised around ducal and baronial households, castles, monasteries, towns, tenant " +
				"villages and mounted service. Feudal lordship, cross-Channel kinship, church patronage and castle-centred " +
				"administration provide the expected forms of conduct.",
			PersonalNameDescription:
				"Choose a Norman French, biblical or Germanic personal name. Baptismal names and names associated with " +
				"ducal and noble houses are especially common.",
			SecondNameDescription:
				"Enter a locative house name beginning de-, du-, des- or de-la-. It names an estate, town or landscape " +
				"association; titles and Fitz- patronyms are not part of this element.",
			ThirdNameDescription: "",
			MinimumSecondNameCount: 200,
			MinimumThirdNameCount: 0,
			BloodModel: "O-A High Negative",
			SkinProfile: "fair_skin",
			HairProfile: "brown_blonde_grey_hair",
			EyeProfile: "brown_green_blue_eyes",
			MaleGivenNames: new[]
			{
				"Alan", "Ancel", "Arnulf", "Baldwin", "Berenger", "Bernard",
				"Drogo", "Eudes", "Fulk", "Geoffrey", "Gilbert", "Giroie",
				"Goisfrid", "Grimbald", "Guy", "Hamon", "Henry", "Herluin",
				"Hugh", "Humphrey", "Ivo", "Jordan", "Lancelin", "Mauger",
				"Nigel", "Odo", "Osbern", "Ralf", "Ranulf", "Richard",
				"Robert", "Roger", "Rollo", "Serlo", "Tancred", "Theobald",
				"Thorold", "Turstin", "Walter", "William", "Warin", "Waleran",
				"William-FitzOsbern", "Robert-Guiscard", "Bohemond", "Rainald", "Everard", "Fulbert",
				"Gervase", "Samson"
			},
			FemaleGivenNames: new[]
			{
				"Adela", "Adeliza", "Agnes", "Alice", "Amabel", "Avelina",
				"Beatrice", "Bertha", "Cecilia", "Constance", "Emma", "Ermengarde",
				"Eremburga", "Felicia", "Gundreda", "Hawise", "Helisende", "Herleva",
				"Hodierna", "Ida", "Isabel", "Judith", "Juliana", "Lauretta",
				"Mabel", "Margaret", "Matilda", "Muriel", "Petronilla", "Philippa",
				"Richenda", "Rohese", "Sibyl", "Stephanie", "Theophania", "Avice",
				"Basilia", "Clarice", "Denise", "Eleanor", "Emmeline", "Eva",
				"Geva", "Godehild", "Helewise", "Maud", "Millicent", "Richeut",
				"Scolastica", "Wymarc"
			},
			MaleSecondNames: MedievalNormanToponymInventory,
			FemaleSecondNames: MedievalNormanToponymInventory,
			ThirdNames: Array.Empty<string>()
		),
		new(
			Key: "Anglo-Norman",
			NameCultureName: "Medieval Anglo-Norman",
			EthnicityName: "Anglo-Norman",
			EthnicGroup: "Western European",
			EthnicSubgroup: "Post-Conquest Britain",
			CultureName: "Medieval Anglo-Norman (c. 1150)",
			NameForm: MedievalPeriodNameForm.GivenFamily,
			NameRegex: @"^(?<birthname>[\w'-]{2,}) (?<surname>(?:(?:de|du|des|de-la|Fitz)-[\w'-]{2,}|[\w'-]{2,}))$",
			ReferenceAnchor: "c. 1150",
			EthnicityDescription:
				"The Anglo-Normans are the French-speaking and bilingual descendants of the conquest aristocracy and the " +
				"households attached to them in England and the neighbouring lordships. They identify with inherited " +
				"continental houses, English estates, royal and ecclesiastical service and the law and manners of the " +
				"post-Conquest elite. Marriage and local settlement steadily bind them to the lands they rule.",
			CultureDescription:
				"Anglo-Norman culture spans castles, manors, royal and ecclesiastical administration, monasteries, towns " +
				"and the networks of tenants, servants and soldiers attached to lordship. French, Latin and English are " +
				"used in different settings, while house service and landed obligation structure rank.",
			PersonalNameDescription:
				"Choose a French, biblical or Germanic baptismal name used in Anglo-Norman households. The same stock is " +
				"shared by nobles, clergy, townspeople and many tenants.",
			SecondNameDescription:
				"Choose a hereditary or documentary identifier: a de- locative, a Fitz- patronym, a house name or an " +
				"office-derived surname. Enter compounds with hyphens, such as de-Clare or Fitz-William.",
			ThirdNameDescription: "",
			MinimumSecondNameCount: 200,
			MinimumThirdNameCount: 0,
			BloodModel: "O-A High Negative",
			SkinProfile: "fair_skin",
			HairProfile: "brown_blonde_grey_hair",
			EyeProfile: "brown_green_blue_eyes",
			MaleGivenNames: new[]
			{
				"Adam", "Alan", "Alexander", "Andrew", "Anselm", "Baldwin",
				"Bartholomew", "Benedict", "Bernard", "Brian", "David", "Edmund",
				"Edward", "Elias", "Eustace", "Geoffrey", "Gerald", "Gilbert",
				"Guy", "Hamon", "Henry", "Herbert", "Hugh", "Humphrey",
				"Ivo", "John", "Jordan", "Laurence", "Martin", "Matthew",
				"Maurice", "Miles", "Nicholas", "Nigel", "Odo", "Osbert",
				"Peter", "Philip", "Ralph", "Ranulf", "Reginald", "Richard",
				"Robert", "Roger", "Simon", "Stephen", "Theobald", "Thomas",
				"Walter", "William"
			},
			FemaleGivenNames: new[]
			{
				"Adela", "Adelina", "Agnes", "Alice", "Amabel", "Avice",
				"Beatrice", "Bertha", "Cecilia", "Clarice", "Constance", "Denise",
				"Edith", "Eleanor", "Emma", "Emmeline", "Eva", "Felicia",
				"Gundreda", "Hawise", "Helewise", "Idonea", "Isabel", "Joan",
				"Judith", "Juliana", "Lauretta", "Lucy", "Mabel", "Margaret",
				"Matilda", "Maud", "Millicent", "Muriel", "Petronilla", "Philippa",
				"Richenda", "Rohese", "Sabina", "Sibyl", "Stephanie", "Susan",
				"Theophania", "Wymarc", "Basilia", "Christina", "Godehild", "Letitia",
				"Richeut", "Scolastica"
			},
			MaleSecondNames: MedievalAngloNormanSurnameInventory,
			FemaleSecondNames: MedievalAngloNormanSurnameInventory,
			ThirdNames: Array.Empty<string>()
		),
		new(
			Key: "High English / British",
			NameCultureName: "Medieval High English British",
			EthnicityName: "High Medieval English",
			EthnicGroup: "Western European",
			EthnicSubgroup: "High Medieval Britain",
			CultureName: "Medieval High English British (c. 1250)",
			NameForm: MedievalPeriodNameForm.GivenFamily,
			NameRegex: @"^(?<birthname>[\w'-]{2,}) (?<surname>[\w'-]{2,})$",
			ReferenceAnchor: "c. 1250",
			EthnicityDescription:
				"The English are the English-speaking people of the kingdom of England, increasingly conscious of a shared " +
				"realm while retaining strong county, town and regional identities. Common law, parish, lordship and " +
				"inherited land define belonging, and English speech distinguishes them from Welsh, Gaelic and " +
				"French-speaking neighbours. In the Marches, mixed ancestry and bilingual households are common without " +
				"erasing an English identity.",
			CultureDescription:
				"High medieval English and Marcher culture is centred on manors, villages, boroughs, cathedral towns, " +
				"royal administration, frontier lordships and aristocratic households. Common law, customary tenure, guild " +
				"life, parish religion, market exchange and military obligation shape each social rank.",
			PersonalNameDescription:
				"Choose a Christian baptismal name. Biblical, saintly, French and older English names all circulate, with " +
				"a relatively small group of popular names repeated across many families.",
			SecondNameDescription:
				"Choose an occupational, locative, descriptive or inherited surname. Names such as Baker, Fletcher and " +
				"Smith identify trades; Atwood and Bywater identify residence; other forms have become hereditary family " +
				"names.",
			ThirdNameDescription: "",
			MinimumSecondNameCount: 200,
			MinimumThirdNameCount: 0,
			BloodModel: "Majority O Minor A",
			SkinProfile: "fair_skin",
			HairProfile: "brown_blonde_grey_hair",
			EyeProfile: "brown_green_blue_eyes",
			MaleGivenNames: new[]
			{
				"Adam", "Alexander", "Andrew", "Anselm", "Arthur", "Bartholomew",
				"Benedict", "Edmund", "Edward", "Elias", "Francis", "Geoffrey",
				"George", "Gilbert", "Gregory", "Guy", "Henry", "Hugh",
				"Humphrey", "James", "John", "Jordan", "Laurence", "Leonard",
				"Martin", "Matthew", "Maurice", "Michael", "Nicholas", "Oliver",
				"Patrick", "Peter", "Philip", "Ralph", "Reginald", "Richard",
				"Robert", "Roger", "Simon", "Stephen", "Theobald", "Thomas",
				"Walter", "William", "Alan", "Brian", "David", "Eustace",
				"Miles", "Ranulf"
			},
			FemaleGivenNames: new[]
			{
				"Agnes", "Alice", "Amabel", "Anne", "Avice", "Beatrice",
				"Cecilia", "Christina", "Clarice", "Constance", "Denise", "Edith",
				"Eleanor", "Elizabeth", "Emma", "Emmeline", "Eva", "Felicia",
				"Hawise", "Helewise", "Idonea", "Isabel", "Joan", "Joanna",
				"Judith", "Juliana", "Katherine", "Lauretta", "Letitia", "Lucy",
				"Mabel", "Margaret", "Margery", "Matilda", "Maud", "Millicent",
				"Muriel", "Petronilla", "Philippa", "Richenda", "Rohese", "Sabina",
				"Sibyl", "Susan", "Theophania", "Agnesina", "Basilia", "Godehild",
				"Isabelot", "Scolastica"
			},
			MaleSecondNames: MedievalEnglishSurnameInventory,
			FemaleSecondNames: MedievalEnglishSurnameInventory,
			ThirdNames: Array.Empty<string>()
		),
		new(
			Key: "Irish / Gaelic",
			NameCultureName: "Medieval Irish Gaelic",
			EthnicityName: "Medieval Irish Gael",
			EthnicGroup: "Celtic",
			EthnicSubgroup: "Gaelic Ireland",
			CultureName: "Medieval Irish Gaelic (c. 1100)",
			NameForm: MedievalPeriodNameForm.GivenPatronym,
			NameRegex: @"^(?<birthname>[\w'-]{2,}) (?<patronym>(?:mac|ingen)-[\w'-]{2,})$",
			ReferenceAnchor: "c. 1100",
			EthnicityDescription:
				"The Irish Gaels are the Gaelic-speaking people of Ireland, organised through dynastic kindreds, local " +
				"kingdoms and client relationships. Genealogy, fosterage, cattle wealth, learned tradition and allegiance " +
				"to a ruling lineage define who belongs to a community. Monasteries and Norse-Gaelic ports add new " +
				"institutions without displacing the authority of kin and tuath.",
			CultureDescription:
				"Medieval Irish Gaelic culture encompasses royal households, tuath-based lordship, farming and pastoral " +
				"communities, monasteries, poets, jurists, craftspeople and seaborne towns. Fosterage, genealogy, " +
				"hospitality, cattle wealth and negotiated clientship govern social standing.",
			PersonalNameDescription:
				"Choose an Old or Middle Irish personal name. Native heroic and dynastic names stand beside Christian and " +
				"devotional names.",
			SecondNameDescription:
				"Enter mac-Father for a man or ingen-Father for a woman. The father's name follows in its genealogical " +
				"form; this is a true patronym and changes from one generation to the next.",
			ThirdNameDescription: "",
			MinimumSecondNameCount: 1,
			MinimumThirdNameCount: 0,
			BloodModel: "Majority O Minor A",
			SkinProfile: "fair_skin",
			HairProfile: "brown_blonde_red_grey_hair",
			EyeProfile: "brown_green_blue_eyes",
			MaleGivenNames: new[]
			{
				"Aed", "Aengus", "Ailill", "Art", "Brian", "Cairbre",
				"Cathal", "Cenn-Faelad", "Cian", "Cormac", "Conall", "Conchobar",
				"Congal", "Domnall", "Donnchad", "Dubgall", "Eochaid", "Fergal",
				"Fergus", "Find", "Flaithbertach", "Gilla-Patraic", "Gilla-Crist", "Gofraid",
				"Gruffudd", "Lorcan", "Mael-Coluim", "Mael-Muire", "Mael-Sechnaill", "Muirchertach",
				"Murchad", "Niall", "Raghnall", "Ruaidri", "Senchan", "Sitric",
				"Tadg", "Tigernan", "Toirdelbach", "Ualgarg", "Cu-Chulainn", "Diarmait",
				"Domnall-Mor", "Donn-Sleibhe", "Eoghan", "Flaithri", "Muiredach", "Ragnall",
				"Tomaltach", "Uaithne"
			},
			FemaleGivenNames: new[]
			{
				"Aibinn", "Aife", "Bebinn", "Blath", "Brigit", "Cacht",
				"Caillech", "Coblaith", "Creidne", "Dearbhfhorghaill", "Derbail", "Dubchoblaig",
				"Eithne", "Emer", "Etain", "Failend", "Fainche", "Findguala",
				"Gormflaith", "Grainne", "Lassar", "Mael-Muire", "Medb", "Muirenn",
				"Orlaith", "Sadb", "Saerlaith", "Slaine", "Sorcha", "Taileflaith",
				"Uallach", "Una", "Bebhinn", "Ben-Mide", "Cacht-Ingen", "Cairech",
				"Derbforgaill", "Dubessa", "Eilis", "Fionnghuala", "Gormlaith", "Marga",
				"Mor", "Nuala", "Raghnailt", "Ragnhild", "Sadhbh", "Sile",
				"Tuathlaith", "Uaine"
			},
			MaleSecondNames: new[]
			{
				"mac-Aeda", "mac-Aengusa", "mac-Aililla", "mac-Airt", "mac-Briain", "mac-Cathail",
				"mac-Ceallaigh", "mac-Cianain", "mac-Conaill", "mac-Conchobair", "mac-Cormaic", "mac-Diarmait",
				"mac-Domnaill", "mac-Donnchada", "mac-Dubgaill", "mac-Eoghain", "mac-Fergail", "mac-Fergusa",
				"mac-Fhinn", "mac-Gofraidh", "mac-Lorcain", "mac-Murchada", "mac-Neill", "mac-Raghnaill",
				"mac-Ruaidri", "mac-Taig", "mac-Tigernain", "mac-Toirdelbaig", "mac-Ualgairg", "mac-Mael-Sechnaill"
			},
			FemaleSecondNames: new[]
			{
				"ingen-Aeda", "ingen-Aengusa", "ingen-Aililla", "ingen-Airt", "ingen-Briain", "ingen-Cathail",
				"ingen-Ceallaigh", "ingen-Cianain", "ingen-Conaill", "ingen-Conchobair", "ingen-Cormaic", "ingen-Diarmait",
				"ingen-Domnaill", "ingen-Donnchada", "ingen-Dubgaill", "ingen-Eoghain", "ingen-Fergail", "ingen-Fergusa",
				"ingen-Fhinn", "ingen-Gofraidh", "ingen-Lorcain", "ingen-Murchada", "ingen-Neill", "ingen-Raghnaill",
				"ingen-Ruaidri", "ingen-Taig", "ingen-Tigernain", "ingen-Toirdelbaig", "ingen-Ualgairg", "ingen-Mael-Sechnaill"
			},
			ThirdNames: Array.Empty<string>()
		),
		new(
			Key: "Scottish / Gaelic-Lowland",
			NameCultureName: "Medieval Scottish Gaelic Lowland",
			EthnicityName: "High Medieval Scottish Gael",
			EthnicGroup: "Celtic and Western European",
			EthnicSubgroup: "Medieval Scotland",
			CultureName: "Medieval Scottish Gaelic Lowland (c. 1250)",
			NameForm: MedievalPeriodNameForm.GivenPatronym,
			NameRegex: @"^(?<birthname>[\w'-]{2,}) (?<patronym>(?:(?:mac|nic)-[\w'-]{2,}|[\w'-]{2,}))$",
			ReferenceAnchor: "c. 1250",
			EthnicityDescription:
				"The Scottish Gaels are the Gaelic-speaking people of the Highlands, western mainland and many islands of " +
				"the kingdom of Scotland. They define themselves through kindred, lordship, district and inherited " +
				"genealogy, with fosterage, cattle and military following reinforcing clan bonds. Contact with Norse " +
				"islanders and French- or Scots-speaking Lowlanders gives border regions a mixed character.",
			CultureDescription:
				"Scottish Gaelic-Lowland culture spans royal burghs, Lowland estates, Gaelic lordships, island " +
				"communities, monasteries, pastoral uplands and knightly households. Regional law, language, kinship and " +
				"military service determine whether a character moves through Gaelic or Lowland social forms.",
			PersonalNameDescription:
				"Choose a Gaelic, Norse-Gaelic, biblical or Franco-Scottish personal name appropriate to the character's " +
				"district and household.",
			SecondNameDescription:
				"Use mac-Father for a man or nic-Father for a woman when giving a Gaelic patronym. Established Lowland or " +
				"Norman house names such as Stewart or Bruce may instead be entered as the complete element.",
			ThirdNameDescription: "",
			MinimumSecondNameCount: 1,
			MinimumThirdNameCount: 0,
			BloodModel: "Majority O Minor A",
			SkinProfile: "fair_skin",
			HairProfile: "brown_blonde_red_grey_hair",
			EyeProfile: "brown_green_blue_eyes",
			MaleGivenNames: new[]
			{
				"Ailin", "Alexander", "Andrew", "Angus", "Archibald", "Aulay",
				"Brian", "Cailean", "Colin", "David", "Domnall", "Donnchad",
				"Dubgall", "Eoin", "Fergus", "Gille-Brighde", "Gille-Crist", "Gille-Easbaig",
				"Gille-Moire", "Gille-Padraig", "Gilbert", "Gregor", "Hugh", "James",
				"John", "Kenneth", "Lachlan", "Malcolm", "Matthew", "Maurice",
				"Muirchertach", "Murdoch", "Niall", "Patrick", "Ragnall", "Robert",
				"Ronald", "Ruaidri", "Simon", "Somerled", "Thomas", "Uilleam",
				"Walter", "Alan", "Bruce", "Comyn", "Duncan", "Edward",
				"Farquhar", "William"
			},
			FemaleGivenNames: new[]
			{
				"Ada", "Agnes", "Aibinn", "Alice", "Amabel", "Beatrice",
				"Bethoc", "Christina", "Devorgilla", "Ealasaid", "Eleanor", "Emma",
				"Eva", "Findguala", "Forbflaith", "Gormflaith", "Gruoch", "Helen",
				"Isabel", "Joanna", "Juliana", "Katherine", "Margaret", "Marjory",
				"Matilda", "Maud", "Muirenn", "Muriel", "Petronilla", "Philippa",
				"Raghnailt", "Rohese", "Sabina", "Slaine", "Sorcha", "Susanna",
				"Una", "Affraic", "Aife", "Beathag", "Cairistiona", "Derbforgaill",
				"Eithne", "Fenella", "Gormlaith", "Mor", "Nuala", "Sadhbh",
				"Sile", "Tuathlaith"
			},
			MaleSecondNames: new[]
			{
				"mac-Ailein", "mac-Aonghais", "mac-Archibald", "mac-Aulaidh", "mac-Briain", "mac-Cailein",
				"mac-Domhnaill", "mac-Donnchaidh", "mac-Dhubhghaill", "mac-Eoghain", "mac-Fhearghais", "mac-Ghriogair",
				"mac-Iain", "mac-Lachlainn", "mac-Mhaoil-Choluim", "mac-Mhuirich", "mac-Neill", "mac-Raghnaill",
				"mac-Ruaidri", "mac-Shomhairle", "Stewart", "Bruce", "Comyn", "Douglas",
				"Murray", "Campbell", "Sinclair", "Fraser", "Gordon", "Dunbar"
			},
			FemaleSecondNames: new[]
			{
				"nic-Ailein", "nic-Aonghais", "nic-Archibald", "nic-Aulaidh", "nic-Briain", "nic-Cailein",
				"nic-Domhnaill", "nic-Donnchaidh", "nic-Dhubhghaill", "nic-Eoghain", "nic-Fhearghais", "nic-Ghriogair",
				"nic-Iain", "nic-Lachlainn", "nic-Mhaoil-Choluim", "nic-Mhuirich", "nic-Neill", "nic-Raghnaill",
				"nic-Ruaidri", "nic-Shomhairle", "Stewart", "Bruce", "Comyn", "Douglas",
				"Murray", "Campbell", "Sinclair", "Fraser", "Gordon", "Dunbar"
			},
			ThirdNames: Array.Empty<string>()
		),
		new(
			Key: "Carolingian / Frankish",
			NameCultureName: "Medieval Carolingian Frankish",
			EthnicityName: "Carolingian Frank",
			EthnicGroup: "Germanic and Romance",
			EthnicSubgroup: "Carolingian Realm",
			CultureName: "Medieval Carolingian Frankish (c. 800)",
			NameForm: MedievalPeriodNameForm.GivenToponym,
			NameRegex: @"^(?<birthname>[\w'-]{2,}) (?<toponym>of-[\w'-]{2,})$",
			ReferenceAnchor: "c. 800",
			EthnicityDescription:
				"The Franks are the ruling people whose kingdoms dominate much of western and central Europe under the " +
				"Carolingians. Frankish identity rests on descent, military and political allegiance, law and " +
				"participation in royal and ecclesiastical institutions, even where Romance or Germanic local speech " +
				"differs. Aristocratic kindreds and royal service carry the name most strongly, while subject populations " +
				"may adopt Frankish custom.",
			CultureDescription:
				"Carolingian culture is expressed through royal and comital households, monasteries, episcopal centres, " +
				"estates, villages and military followings. Oath, office, benefice, tribute, church reform and local " +
				"customary law coexist across the empire.",
			PersonalNameDescription:
				"Choose a Frankish Germanic or Christian personal name. Dithematic forms using elements such as Adal-, " +
				"Berht-, Chlod-, -ric and -hard are common beside saintly names.",
			SecondNameDescription:
				"Enter an of- place or regional byname. It identifies the person's estate, city, monastery, county or " +
				"homeland and is not a stable hereditary surname at this reference date.",
			ThirdNameDescription: "",
			MinimumSecondNameCount: 200,
			MinimumThirdNameCount: 0,
			BloodModel: "O-A High Negative",
			SkinProfile: "fair_skin",
			HairProfile: "brown_blonde_grey_hair",
			EyeProfile: "brown_green_blue_eyes",
			MaleGivenNames: new[]
			{
				"Adalard", "Adalbert", "Adalbero", "Ansegisel", "Arnulf", "Bego",
				"Bernard", "Berengar", "Carloman", "Charles", "Childebrand", "Drogo",
				"Ebbo", "Eberhard", "Einhard", "Emmo", "Eudes", "Everard",
				"Fulk", "Gausbert", "Gerard", "Gerold", "Giselbert", "Grimoald",
				"Hilduin", "Hincmar", "Hugh", "Lambert", "Lothar", "Louis",
				"Magnus", "Milo", "Nithard", "Odo", "Pepin", "Radbert",
				"Ragenold", "Reginar", "Remigius", "Robert", "Rotrude", "Rorgon",
				"Sigibert", "Theodoric", "Theotbald", "Unroch", "Wala", "Waldo",
				"Warin", "William"
			},
			FemaleGivenNames: new[]
			{
				"Adaltrude", "Adela", "Adelais", "Albina", "Amalberga", "Anstrude",
				"Basina", "Bertrada", "Bertha", "Bilichild", "Chrodechild", "Clothild",
				"Cunegund", "Dhuoda", "Emma", "Engelberga", "Ermengard", "Ermentrude",
				"Fastrada", "Gisela", "Gisla", "Hildegard", "Himiltrude", "Hruodtrud",
				"Ida", "Imma", "Judith", "Liutgard", "Madelgard", "Mathilda",
				"Oda", "Odelia", "Plectrude", "Regina", "Richildis", "Richwara",
				"Rotrude", "Ruodhaid", "Swanahild", "Theodrada", "Theodelinda", "Hereswind",
				"Waldrada", "Williburg", "Begga", "Gerberga", "Godehild", "Irmina",
				"Radegund", "Rothaide"
			},
			MaleSecondNames: MedievalCarolingianToponymInventory,
			FemaleSecondNames: MedievalCarolingianToponymInventory,
			ThirdNames: Array.Empty<string>()
		),
		new(
			Key: "Capetian French",
			NameCultureName: "Medieval Capetian French",
			EthnicityName: "High Medieval French",
			EthnicGroup: "Western European",
			EthnicSubgroup: "Capetian France",
			CultureName: "Medieval Capetian French (c. 1200)",
			NameForm: MedievalPeriodNameForm.GivenToponym,
			NameRegex: @"^(?<birthname>[\w'-]{2,}) (?<toponym>(?:de|du|des|de-la)-[\w'-]{2,})$",
			ReferenceAnchor: "c. 1200",
			EthnicityDescription:
				"The French are the Romance-speaking people of the royal domain and neighbouring principalities of " +
				"northern and central France. They share related langue d'oil speech, Christian institutions and the " +
				"political prestige of the Capetian kings, while province, lordship and town remain powerful identities. " +
				"Courtly custom and expanding royal justice gradually give the name French a wider reach.",
			CultureDescription:
				"Capetian French culture flourishes in royal and princely courts, castles, monasteries, cathedral schools, " +
				"communes, market towns and agricultural estates. Seigneurial relationships, parish life, craft " +
				"organisation, pilgrimage and royal justice define public conduct.",
			PersonalNameDescription:
				"Choose a Christian, French or inherited Germanic personal name. Saintly and royal names are particularly " +
				"common.",
			SecondNameDescription:
				"Enter a locative identifier beginning de-, du-, des- or de-la-. It names a place, estate or topographic " +
				"feature and may later become hereditary.",
			ThirdNameDescription: "",
			MinimumSecondNameCount: 200,
			MinimumThirdNameCount: 0,
			BloodModel: "O-A High Negative",
			SkinProfile: "fair_skin",
			HairProfile: "brown_blonde_grey_hair",
			EyeProfile: "brown_green_blue_eyes",
			MaleGivenNames: new[]
			{
				"Adam", "Aimery", "Alain", "Amaury", "Anseau", "Arnoul",
				"Aubert", "Baudouin", "Bernard", "Bertrand", "Charles", "Eudes",
				"Etienne", "Evrard", "Ferri", "Foulques", "Gautier", "Geoffroi",
				"Geraud", "Gilles", "Girard", "Guillaume", "Guy", "Henri",
				"Herve", "Hugues", "Jacques", "Jean", "Lambert", "Louis",
				"Martin", "Mathieu", "Miles", "Nicolas", "Olivier", "Othon",
				"Philippe", "Pierre", "Raoul", "Renaud", "Richard", "Robert",
				"Roger", "Simon", "Thibaut", "Thomas", "Tristan", "Waleran",
				"Yves", "Gervais"
			},
			FemaleGivenNames: new[]
			{
				"Adele", "Adelais", "Agnes", "Aelis", "Alix", "Ameline",
				"Aveline", "Beatrice", "Blanche", "Clemence", "Constance", "Denise",
				"Ermengarde", "Ermessende", "Eustachie", "Felicie", "Florie", "Gilette",
				"Helisende", "Isabelle", "Jacquette", "Jeanne", "Julienne", "Laure",
				"Lucie", "Mahaut", "Marguerite", "Marie", "Melisende", "Odeline",
				"Peronelle", "Philippe", "Richeut", "Rohese", "Sibylle", "Stephanie",
				"Theophanie", "Yolande", "Aimee", "Alienor", "Berthe", "Emeline",
				"Eremburge", "Guillemette", "Heloise", "Hermine", "Hersende", "Ide",
				"Mabile", "Tiphaine"
			},
			MaleSecondNames: MedievalCapetianToponymInventory,
			FemaleSecondNames: MedievalCapetianToponymInventory,
			ThirdNames: Array.Empty<string>()
		),
		new(
			Key: "Holy Roman Empire / German",
			NameCultureName: "Medieval Imperial German",
			EthnicityName: "High Medieval German",
			EthnicGroup: "Germanic",
			EthnicSubgroup: "Holy Roman Empire",
			CultureName: "Medieval Imperial German (c. 1200)",
			NameForm: MedievalPeriodNameForm.GivenFamily,
			NameRegex: @"^(?<birthname>[\w'-]{2,}) (?<surname>(?:von-[\w'-]{2,}|[\w'-]{2,}))$",
			ReferenceAnchor: "c. 1200",
			EthnicityDescription:
				"The Germans are the German-speaking people of the duchies, bishoprics, lordships and towns of the Holy " +
				"Roman Empire. Dialect and regional identity distinguish Saxon, Franconian, Swabian, Bavarian, Rhineland " +
				"and Alpine communities, but shared speech, law and imperial-Christian institutions mark them from Romance " +
				"and Slavic neighbours. Town citizenship, lordship and native district are often as important as wider " +
				"German identity.",
			CultureDescription:
				"Imperial German culture is found in princely and episcopal courts, ministerial households, imperial and " +
				"territorial towns, monasteries, villages and Alpine routes. Local law, guild, lordship, church " +
				"institution and service to prince or bishop determine status.",
			PersonalNameDescription:
				"Choose a Germanic or Christian baptismal name. Older compounds remain common beside biblical and saintly " +
				"names.",
			SecondNameDescription:
				"Choose a locative, occupational, descriptive or hereditary family name. von- forms indicate an estate or " +
				"place; names such as Schmidt, Muller and Schneider derive from professions.",
			ThirdNameDescription: "",
			MinimumSecondNameCount: 200,
			MinimumThirdNameCount: 0,
			BloodModel: "O-A High Negative",
			SkinProfile: "fair_skin",
			HairProfile: "brown_blonde_grey_hair",
			EyeProfile: "brown_green_blue_eyes",
			MaleGivenNames: new[]
			{
				"Adalbert", "Albrecht", "Anselm", "Arnold", "Berthold", "Bruno",
				"Burkhard", "Conrad", "Dietrich", "Eberhard", "Ekkehard", "Emmerich",
				"Engelbert", "Ernst", "Friedrich", "Gebhard", "Gerhard", "Gottfried",
				"Gunther", "Hartmann", "Heinrich", "Hermann", "Hildebrand", "Hugo",
				"Konrad", "Lambert", "Leopold", "Liudolf", "Lothar", "Ludwig",
				"Manegold", "Markward", "Meinhard", "Otto", "Poppo", "Rainer",
				"Rudolf", "Ruprecht", "Siegfried", "Theoderich", "Ulrich", "Welf",
				"Werner", "Wilhelm", "Wolfram", "Albert", "Burchard", "Everhard",
				"Giselbert", "Walther"
			},
			FemaleGivenNames: new[]
			{
				"Adelheid", "Agnes", "Anna", "Beatrix", "Bertha", "Brunhild",
				"Clementia", "Cunigunde", "Dietlind", "Elisabeth", "Emma", "Ermengard",
				"Gerberga", "Gertrud", "Gisela", "Hadwig", "Heilwig", "Hildegard",
				"Ida", "Irmengard", "Jutta", "Kunigunde", "Liutgard", "Lukardis",
				"Margareta", "Mathilde", "Mechthild", "Oda", "Ottilia", "Richenza",
				"Sophia", "Theophanu", "Uta", "Walburga", "Willa", "Adelgund",
				"Bertrada", "Christina", "Gepa", "Guda", "Hedwig", "Imagina",
				"Irmgard", "Lioba", "Regelinda", "Salome", "Swanahild", "Theodora",
				"Wiltrud", "Wulfhild"
			},
			MaleSecondNames: MedievalGermanSurnameInventory,
			FemaleSecondNames: MedievalGermanSurnameInventory,
			ThirdNames: Array.Empty<string>()
		),
		new(
			Key: "Christian Iberian",
			NameCultureName: "Medieval Christian Iberian",
			EthnicityName: "High Medieval Castilian",
			EthnicGroup: "Iberian",
			EthnicSubgroup: "Christian Iberian Kingdoms",
			CultureName: "Medieval Christian Iberian (c. 1200)",
			NameForm: MedievalPeriodNameForm.GivenPatronym,
			NameRegex: @"^(?<birthname>[\w'-]{2,}) (?<patronym>(?:[\w'-]+(?:ez|az|es|is|oz|enco|ins)|Garcia|Afonso|Pais|Ruiz|Velaz))$",
			ReferenceAnchor: "c. 1200",
			EthnicityDescription:
				"The Castilians are a Romance-speaking people of the central Iberian plateau and the expanding kingdom of " +
				"Castile. They identify with kingdom, town or concejo, Christian parish, family descent and the frontier " +
				"settlements won or chartered by kings and lords. Their speech, law and patronymic naming distinguish them " +
				"from neighbouring Leonese, Galicians, Portuguese, Catalans, Basques and Andalusis.",
			CultureDescription:
				"Christian Iberian culture encompasses royal and noble households, frontier towns, monasteries, municipal " +
				"councils, peasant villages, pastoral routes and military-religious institutions. Fueros, repopulation " +
				"grants, kinship, parish and lordship produce strong local customs that may be adopted by many ethnic " +
				"groups.",
			PersonalNameDescription:
				"Choose a Christian Iberian personal name. Biblical, saintly, Germanic and Romance forms circulate across " +
				"Castile, Leon, Portugal, Navarre and Aragon.",
			SecondNameDescription:
				"Choose a patronym derived from the father's personal name. Castilian and Leonese forms commonly end -ez " +
				"or -az, while Galician-Portuguese forms include -es, -is and related endings; Garcia and several older " +
				"forms are also accepted.",
			ThirdNameDescription: "",
			MinimumSecondNameCount: 1,
			MinimumThirdNameCount: 0,
			BloodModel: "O-A High Negative",
			SkinProfile: "fair_olive_skin",
			HairProfile: "black_brown_grey_hair",
			EyeProfile: "brown_green_eyes",
			MaleGivenNames: new[]
			{
				"Afonso", "Alvaro", "Andres", "Alfonso", "Bermudo", "Bertran",
				"Diego", "Domingo", "Enrique", "Esteban", "Fernando", "Froila",
				"Garcia", "Gil", "Gonzalo", "Guillen", "Gutierre", "Jaime",
				"Joan", "Jorge", "Lope", "Lourenco", "Luis", "Martim",
				"Martin", "Mendo", "Munio", "Nuno", "Ordon", "Paio",
				"Pedro", "Pelayo", "Raimundo", "Ramiro", "Rodrigo", "Rui",
				"Sancho", "Tello", "Vasco", "Ximeno", "Arias", "Beltran",
				"Bernat", "Dinis", "Ermengol", "Ferran", "Goncalo", "Odoario",
				"Ponce", "Salvador"
			},
			FemaleGivenNames: new[]
			{
				"Aldonza", "Andregoto", "Beatriz", "Berenguela", "Blanca", "Brianda",
				"Constanza", "Elvira", "Enderquina", "Ermesenda", "Estefania", "Eulalia",
				"Gontrodo", "Guiomar", "Ines", "Isabel", "Jimena", "Juana",
				"Leonor", "Mafalda", "Maior", "Maria", "Marina", "Mencia",
				"Milia", "Munia", "Oneca", "Orraca", "Sancha", "Teresa",
				"Toda", "Urraca", "Violante", "Aldara", "Berengaria", "Catalina",
				"Dulce", "Elionor", "Estevainha", "Froileuba", "Geloira", "Gracia",
				"Mayor", "Muniadona", "Nunoza", "Sanxia", "Sol", "Toda-Aznarez",
				"Velasquita", "Ximena"
			},
			MaleSecondNames: new[]
			{
				"Alvarez", "Bermudez", "Diaz", "Dominguez", "Enriquez", "Estevez",
				"Fernandez", "Froilaz", "Garcia", "Gonzalez", "Gutierrez", "Jimenez",
				"Lopez", "Martinez", "Mendez", "Munoz", "Nunez", "Ordonez",
				"Perez", "Ramirez", "Rodriguez", "Ruiz", "Sanchez", "Tellez",
				"Vazquez", "Afonso", "Anes", "Eanes", "Fernandes", "Goncalves",
				"Lourenco", "Martins", "Mendes", "Nunes", "Pais", "Peres",
				"Rodrigues", "Soares", "Vasques", "Velaz"
			},
			FemaleSecondNames: new[]
			{
				"Alvarez", "Bermudez", "Diaz", "Dominguez", "Enriquez", "Estevez",
				"Fernandez", "Froilaz", "Garcia", "Gonzalez", "Gutierrez", "Jimenez",
				"Lopez", "Martinez", "Mendez", "Munoz", "Nunez", "Ordonez",
				"Perez", "Ramirez", "Rodriguez", "Ruiz", "Sanchez", "Tellez",
				"Vazquez", "Afonso", "Anes", "Eanes", "Fernandes", "Goncalves",
				"Lourenco", "Martins", "Mendes", "Nunes", "Pais", "Peres",
				"Rodrigues", "Soares", "Vasques", "Velaz"
			},
			ThirdNames: Array.Empty<string>()
		),
		new(
			Key: "Andalusian",
			NameCultureName: "Medieval Andalusian Arabic",
			EthnicityName: "High Medieval Andalusi",
			EthnicGroup: "Iberian and Middle Eastern",
			EthnicSubgroup: "al-Andalus",
			CultureName: "Medieval Andalusian Arabic (c. 1100)",
			NameForm: MedievalPeriodNameForm.GivenPatronymToponym,
			NameRegex: @"^(?<birthname>[\w'-]{2,}) (?<patronym>(?:ibn|bint)-[\w'-]{2,}) (?<toponym>al-[\w'-]{2,})$",
			ReferenceAnchor: "c. 1100",
			EthnicityDescription:
				"The Andalusis are the Arabic-speaking people of al-Andalus, descended from Iberian converts and from " +
				"Arab, Amazigh and other Muslim settlers. They recognise a shared Andalusi manner of speech, learning, " +
				"urbanity, agriculture and music while retaining family memories of tribe, city and ancestry. Attachment " +
				"to a town such as Cordoba, Seville or Granada is one of the strongest marks of identity.",
			CultureDescription:
				"Andalusi culture is centred on irrigated countryside, towns, craft quarters, scholarly circles, courts, " +
				"frontier garrisons and Mediterranean trade. Family, neighbourhood, legal school, patronage and place of " +
				"origin shape reputation under taifa and Almoravid rule.",
			PersonalNameDescription:
				"Choose an Arabic ism, the person's own name. Abd-al- compounds and other devotional names are kept as one " +
				"hyphenated token.",
			SecondNameDescription:
				"Enter ibn-Father for a man or bint-Father for a woman. The father's personal name follows the prefix, and " +
				"the element is genealogical rather than hereditary.",
			ThirdNameDescription:
				"Enter an al- nisba naming a city, region, tribe, craft or scholarly affiliation. Andalusi examples " +
				"include al-Qurtubi, al-Ishbili, al-Gharnati and al-Zahrawi.",
			MinimumSecondNameCount: 1,
			MinimumThirdNameCount: 200,
			BloodModel: "Majority O Minor A",
			SkinProfile: "olive_skin",
			HairProfile: "black_brown_grey_hair",
			EyeProfile: "brown_green_eyes",
			MaleGivenNames: new[]
			{
				"Abbas", "Abdallah", "Abd-al-Aziz", "Abd-al-Malik", "Abd-al-Rahman", "Abu-Bakr",
				"Ahmad", "Ali", "Al-Mutamid", "Al-Mansur", "Ammar", "Bakr",
				"Bashir", "Faraj", "Fath", "Gharsiya", "Habib", "Hamid",
				"Hasan", "Hisham", "Husayn", "Ibrahim", "Idris", "Ilyas",
				"Ismail", "Jafar", "Khalaf", "Khalid", "Mahmud", "Malik",
				"Mansur", "Muhammad", "Musa", "Nasr", "Qasim", "Rashid",
				"Said", "Salim", "Sulayman", "Tahir", "Umar", "Walid",
				"Yahya", "Yusuf", "Zafir", "Zayd", "Ziyad", "Lub",
				"Mundhir", "Saad"
			},
			FemaleGivenNames: new[]
			{
				"Aisha", "Amina", "Asma", "Butayna", "Fatima", "Ghaya",
				"Habiba", "Hafsa", "Halima", "Hamda", "Hind", "Ibtisam",
				"Itimad", "Jamila", "Khadija", "Laila", "Lubna", "Maryam",
				"Muhja", "Muna", "Nuzha", "Qamar", "Radiyya", "Rabi'a",
				"Rahma", "Rayhana", "Rumaykiyya", "Ruqayya", "Safiyya", "Salma",
				"Shams", "Subh", "Sukayna", "Wallada", "Zahra", "Zaynab",
				"Zubayda", "Afiya", "Amat-al-Aziz", "Amat-al-Malik", "Bahiyya", "Hasana",
				"Hawra", "Jawhara", "Malika", "Munya", "Naima", "Nura",
				"Thurayya", "Ward"
			},
			MaleSecondNames: new[]
			{
				"ibn-Abbas", "ibn-Abdallah", "ibn-Abd-al-Rahman", "ibn-Ahmad", "ibn-Ali", "ibn-Bakr",
				"ibn-Faraj", "ibn-Hasan", "ibn-Hisham", "ibn-Husayn", "ibn-Ibrahim", "ibn-Idris",
				"ibn-Ismail", "ibn-Jafar", "ibn-Khalid", "ibn-Mahmud", "ibn-Malik", "ibn-Mansur",
				"ibn-Muhammad", "ibn-Musa", "ibn-Nasr", "ibn-Qasim", "ibn-Said", "ibn-Sulayman",
				"ibn-Umar", "ibn-Yahya", "ibn-Yusuf", "ibn-Zayd", "ibn-Ziyad", "ibn-Saad"
			},
			FemaleSecondNames: new[]
			{
				"bint-Abbas", "bint-Abdallah", "bint-Abd-al-Rahman", "bint-Ahmad", "bint-Ali", "bint-Bakr",
				"bint-Faraj", "bint-Hasan", "bint-Hisham", "bint-Husayn", "bint-Ibrahim", "bint-Idris",
				"bint-Ismail", "bint-Jafar", "bint-Khalid", "bint-Mahmud", "bint-Malik", "bint-Mansur",
				"bint-Muhammad", "bint-Musa", "bint-Nasr", "bint-Qasim", "bint-Said", "bint-Sulayman",
				"bint-Umar", "bint-Yahya", "bint-Yusuf", "bint-Zayd", "bint-Ziyad", "bint-Saad"
			},
			ThirdNames: MedievalAndalusiNisbaInventory
		),
		new(
			Key: "Byzantine",
			NameCultureName: "Medieval Byzantine Greek",
			EthnicityName: "Middle Byzantine Roman",
			EthnicGroup: "Greek and Eastern Roman",
			EthnicSubgroup: "Byzantine Empire",
			CultureName: "Medieval Byzantine Greek (c. 1000)",
			NameForm: MedievalPeriodNameForm.GivenFamily,
			NameRegex: @"^(?<birthname>[\w'-]{2,}) (?<surname>(?:of-[\w'-]{2,}|[\w'-]{2,}))$",
			ReferenceAnchor: "c. 1000",
			EthnicityDescription:
				"The Byzantine Romans, or Rhomaioi, are the Greek-speaking Christian people who regard the empire as the " +
				"continuing Roman state. Orthodox faith, Roman law, imperial allegiance and Greek education are the " +
				"principal signs of Roman identity, though many Armenian, Slavic and other families enter it through " +
				"service, marriage and language. City, province and household remain important beneath the wider Roman " +
				"name.",
			CultureDescription:
				"Middle Byzantine culture is organised through Constantinopolitan and provincial households, themes and " +
				"military estates, monasteries, bishoprics, market towns, craft production and imperial administration. " +
				"Orthodoxy, Roman law, taxation, patronage and rank govern public life.",
			PersonalNameDescription:
				"Choose a Greek Christian personal name. Classical forms, saints' names and names favoured by imperial " +
				"houses all occur.",
			SecondNameDescription:
				"Choose a family or house name, especially appropriate to urban, military and elite characters. A locative " +
				"of- form may be used for someone chiefly known by origin rather than dynasty.",
			ThirdNameDescription: "",
			MinimumSecondNameCount: 1,
			MinimumThirdNameCount: 0,
			BloodModel: "O-A High Negative",
			SkinProfile: "fair_olive_skin",
			HairProfile: "black_brown_grey_hair",
			EyeProfile: "brown_green_eyes",
			MaleGivenNames: new[]
			{
				"Alexios", "Andronikos", "Bardas", "Basil", "Constantine", "Damian",
				"Demetrios", "Doukas", "Eustathios", "George", "Gregory", "Isaac",
				"John", "Joseph", "Konstantinos", "Leo", "Manuel", "Michael",
				"Nikephoros", "Niketas", "Nikolaos", "Peter", "Philaretos", "Philip",
				"Romanos", "Sergios", "Stephen", "Theodore", "Theodosios", "Thomas",
				"Anastasios", "Antiochos", "Apollonios", "Athanasios", "Christophoros", "David",
				"Dionysios", "Eudokimos", "Eustratios", "Germanos", "Ioannikios", "Kallinikos",
				"Kosmas", "Kyriakos", "Methodios", "Neophytos", "Photios", "Symeon",
				"Theophylaktos", "Zosimos"
			},
			FemaleGivenNames: new[]
			{
				"Agatha", "Anna", "Anastasia", "Anthousa", "Barbara", "Christina",
				"Danielis", "Eirene", "Elena", "Eudokia", "Euphemia", "Euphrosyne",
				"Georgia", "Helena", "Hypatia", "Ioanna", "Irene", "Kale",
				"Kassiane", "Katherine", "Maria", "Martha", "Melania", "Sophia",
				"Thekla", "Theodora", "Zoe", "Alexia", "Aikaterine", "Charitine",
				"Drosoula", "Elpis", "Evdokia", "Glykera", "Kallirhoe", "Kyranna",
				"Magdalene", "Marina", "Paraskeve", "Pelagia", "Pulcheria", "Romana",
				"Sebaste", "Simonis", "Synadene", "Theophano", "Thomais", "Xene",
				"Ypatia", "Zampia"
			},
			MaleSecondNames: new[]
			{
				"Doukas", "Komnenos", "Phokas", "Skleros", "Argyros", "Dalassenos",
				"Botaneiates", "Bryennios", "Kourkouas", "Maleinos", "Lekapenos", "Monomachos",
				"Diogenes", "Taronites", "Melissenos", "Angelos", "Palaiologos", "Kantakouzenos",
				"Synadenos", "Vatatzes", "Kaminas", "Choniates", "Psellos", "Attaleiates",
				"Kekaumenos", "Maniakes", "Tornikes", "Gabras", "Euphorbenos", "Xiphilinos",
				"of-Constantinople", "of-Thessalonike", "of-Nicaea", "of-Trebizond", "of-Athens", "of-Corinth",
				"of-Chios", "of-Crete", "of-Ephesos", "of-Antioch"
			},
			FemaleSecondNames: new[]
			{
				"Doukas", "Komnenos", "Phokas", "Skleros", "Argyros", "Dalassenos",
				"Botaneiates", "Bryennios", "Kourkouas", "Maleinos", "Lekapenos", "Monomachos",
				"Diogenes", "Taronites", "Melissenos", "Angelos", "Palaiologos", "Kantakouzenos",
				"Synadenos", "Vatatzes", "Kaminas", "Choniates", "Psellos", "Attaleiates",
				"Kekaumenos", "Maniakes", "Tornikes", "Gabras", "Euphorbenos", "Xiphilinos",
				"of-Constantinople", "of-Thessalonike", "of-Nicaea", "of-Trebizond", "of-Athens", "of-Corinth",
				"of-Chios", "of-Crete", "of-Ephesos", "of-Antioch"
			},
			ThirdNames: Array.Empty<string>()
		),
		new(
			Key: "Abbasid",
			NameCultureName: "Medieval Abbasid Arabic",
			EthnicityName: "Abbasid-Era Iraqi Arab",
			EthnicGroup: "Middle Eastern",
			EthnicSubgroup: "Abbasid Caliphate",
			CultureName: "Medieval Abbasid Arabic (c. 850)",
			NameForm: MedievalPeriodNameForm.GivenPatronymToponym,
			NameRegex: @"^(?<birthname>[\w'-]{2,}) (?<patronym>(?:ibn|bint)-[\w'-]{2,}) (?<toponym>al-[\w'-]{2,})$",
			ReferenceAnchor: "c. 850",
			EthnicityDescription:
				"The Iraqi Arabs are the Arabic-speaking people of the Tigris-Euphrates cities, irrigation districts and " +
				"tribal lands at the heart of the Abbasid caliphate. They define themselves through town or tribe, " +
				"paternal genealogy, neighbourhood, religious community and the prestige of Arabic speech. Baghdad, Basra, " +
				"Kufa and Samarra create a shared Iraqi urban world while preserving strong local loyalties.",
			CultureDescription:
				"Abbasid culture centres on Baghdad, Samarra, Basra, Kufa and the agricultural and caravan regions linked " +
				"to them. Court and military households, merchants, artisans, scholars, religious communities and " +
				"labourers are connected by patronage, law, neighbourhood and long-distance mobility.",
			PersonalNameDescription: "Choose an Arabic ism, the person's own name. Devotional compounds remain one hyphenated token.",
			SecondNameDescription:
				"Enter ibn-Father for a man or bint-Father for a woman. The father's personal name follows the prefix; use " +
				"one generation here even when a formal genealogy would continue through several ancestors.",
			ThirdNameDescription:
				"Enter an al- nisba identifying place, tribe, ancestry, craft, legal school, religious affiliation or " +
				"office, such as al-Baghdadi, al-Tamimi or al-Warraq.",
			MinimumSecondNameCount: 1,
			MinimumThirdNameCount: 200,
			BloodModel: "Majority O Minor A",
			SkinProfile: "olive_skin",
			HairProfile: "black_brown_grey_hair",
			EyeProfile: "brown_green_eyes",
			MaleGivenNames: new[]
			{
				"Abbas", "Abdallah", "Abd-al-Aziz", "Abd-al-Malik", "Abd-al-Rahman", "Abu-Bakr",
				"Ahmad", "Ali", "Amin", "Amr", "Bashir", "Dawud",
				"Fadl", "Faraj", "Harun", "Hasan", "Husayn", "Ibrahim",
				"Idris", "Ilyas", "Isa", "Ishaq", "Ismail", "Jafar",
				"Khalid", "Mahdi", "Mahmud", "Malik", "Mansur", "Mamun",
				"Muhammad", "Musa", "Mutasim", "Muntasir", "Muqtadir", "Nasr",
				"Qasim", "Rashid", "Said", "Salih", "Samarra", "Sulayman",
				"Tahir", "Talha", "Umar", "Uthman", "Yahya", "Yusuf",
				"Zayd", "Zubayr"
			},
			FemaleGivenNames: new[]
			{
				"Aisha", "Aliya", "Amina", "Arib", "Asma", "Atika",
				"Buran", "Dananir", "Dunya", "Fatima", "Fadl", "Farida",
				"Ghader", "Hababa", "Hafsa", "Halima", "Hamda", "Hind",
				"Inan", "Jamila", "Khadija", "Khayzuran", "Lubaba", "Lubna",
				"Mahbuba", "Marajil", "Maryam", "Muta", "Nura", "Qamar",
				"Qaratis", "Rabi'a", "Rahma", "Rayhana", "Ruqayya", "Safiyya",
				"Salma", "Shaghab", "Shariyya", "Shuja", "Subh", "Sukayna",
				"Ulayya", "Ward", "Zahr", "Zahra", "Zaynab", "Zubayda",
				"Zumurrud", "Qabiha"
			},
			MaleSecondNames: new[]
			{
				"ibn-Abbas", "ibn-Abdallah", "ibn-Abd-al-Malik", "ibn-Abd-al-Rahman", "ibn-Ahmad", "ibn-Ali",
				"ibn-Amr", "ibn-Dawud", "ibn-Fadl", "ibn-Harun", "ibn-Hasan", "ibn-Husayn",
				"ibn-Ibrahim", "ibn-Ishaq", "ibn-Ismail", "ibn-Jafar", "ibn-Khalid", "ibn-Mahmud",
				"ibn-Malik", "ibn-Mansur", "ibn-Muhammad", "ibn-Musa", "ibn-Nasr", "ibn-Qasim",
				"ibn-Rashid", "ibn-Salih", "ibn-Sulayman", "ibn-Umar", "ibn-Yahya", "ibn-Yusuf"
			},
			FemaleSecondNames: new[]
			{
				"bint-Abbas", "bint-Abdallah", "bint-Abd-al-Malik", "bint-Abd-al-Rahman", "bint-Ahmad", "bint-Ali",
				"bint-Amr", "bint-Dawud", "bint-Fadl", "bint-Harun", "bint-Hasan", "bint-Husayn",
				"bint-Ibrahim", "bint-Ishaq", "bint-Ismail", "bint-Jafar", "bint-Khalid", "bint-Mahmud",
				"bint-Malik", "bint-Mansur", "bint-Muhammad", "bint-Musa", "bint-Nasr", "bint-Qasim",
				"bint-Rashid", "bint-Salih", "bint-Sulayman", "bint-Umar", "bint-Yahya", "bint-Yusuf"
			},
			ThirdNames: MedievalAbbasidNisbaInventory
		),
		new(
			Key: "Fatimid",
			NameCultureName: "Medieval Fatimid Arabic",
			EthnicityName: "Fatimid-Era Egyptian Arab",
			EthnicGroup: "North African and Middle Eastern",
			EthnicSubgroup: "Fatimid Sphere",
			CultureName: "Medieval Fatimid Arabic (c. 1050)",
			NameForm: MedievalPeriodNameForm.GivenPatronymToponym,
			NameRegex: @"^(?<birthname>[\w'-]{2,}) (?<patronym>(?:ibn|bint)-[\w'-]{2,}) (?<toponym>al-[\w'-]{2,})$",
			ReferenceAnchor: "c. 1050",
			EthnicityDescription:
				"The Egyptian Arabs are the Arabic-speaking people of the Nile valley and Delta whose local identity is " +
				"rooted in Egypt, town or village, paternal kin and religious community. By the Fatimid period Arabic " +
				"unites much public life, while Coptic, Amazigh, Jewish, Nubian and other communities preserve distinct " +
				"ancestries and institutions beside them. Cairo-Fustat and the river system give Egyptian Arabs a common " +
				"commercial and administrative world.",
			CultureDescription:
				"Fatimid culture spans Cairo-Fustat, Alexandria, provincial towns, irrigated villages, Ifriqiyan centres, " +
				"court households and Mediterranean or Red Sea commerce. State ceremony, Isma'ili institutions, Sunni and " +
				"Christian learning, markets and local custom coexist within a plural realm.",
			PersonalNameDescription:
				"Choose an Arabic ism. Personal names may be devotional, biblical or inherited from Arab and Egyptian " +
				"usage; compounds remain one token.",
			SecondNameDescription:
				"Enter ibn-Father for a man or bint-Father for a woman. It records paternal descent rather than a fixed " +
				"family surname.",
			ThirdNameDescription:
				"Enter an al- nisba naming place, tribe, dynasty, profession, school or household. The pool emphasises " +
				"Egypt, Ifriqiya and the western Mediterranean.",
			MinimumSecondNameCount: 1,
			MinimumThirdNameCount: 200,
			BloodModel: "Majority O",
			SkinProfile: "olive_skin",
			HairProfile: "black_brown_grey_hair",
			EyeProfile: "brown_green_eyes",
			MaleGivenNames: new[]
			{
				"Abdallah", "Abd-al-Aziz", "Abd-al-Majid", "Abu-Tamim", "Ahmad", "Ali",
				"Ammar", "Aziz", "Barjawan", "Buluggin", "Dawud", "Fadl",
				"Faraj", "Hasan", "Husayn", "Idris", "Ismail", "Jafar",
				"Jawhar", "Khalaf", "Khalil", "Mahdi", "Mahmud", "Malik",
				"Mansur", "Maadd", "Muhammad", "Mustali", "Mustansir", "Nasir",
				"Nizar", "Qasim", "Qa'id", "Rashid", "Said", "Salih",
				"Sinan", "Sulayman", "Tamim", "Tayyib", "Ubaydallah", "Umar",
				"Yahya", "Yusuf", "Zafir", "Zayd", "Ziri", "Hamid",
				"Lu'lu", "Ridwan"
			},
			FemaleGivenNames: new[]
			{
				"Aisha", "Amina", "Asma", "Aziza", "Badr", "Bahiyya",
				"Dunya", "Fatima", "Fawziyya", "Ghaliya", "Habiba", "Halima",
				"Hind", "Jamila", "Jawhara", "Khadija", "Laila", "Lubna",
				"Malika", "Maryam", "Munira", "Nafisa", "Nura", "Qamar",
				"Rabi'a", "Rahma", "Rayhana", "Ruqayya", "Safiyya", "Salma",
				"Sitt-al-Arab", "Sitt-al-Mulk", "Sitt-al-Qusur", "Sukayna", "Tahira", "Ward",
				"Zahra", "Zaynab", "Zubayda", "Zumurrud", "Arwa", "Hurrat-al-Malika",
				"Jalwa", "Mulk", "Rashida", "Rida", "Shams", "Taqqiya",
				"Turkan", "Yasmine"
			},
			MaleSecondNames: new[]
			{
				"ibn-Abdallah", "ibn-Abd-al-Aziz", "ibn-Ahmad", "ibn-Ali", "ibn-Ammar", "ibn-Dawud",
				"ibn-Fadl", "ibn-Faraj", "ibn-Hasan", "ibn-Husayn", "ibn-Idris", "ibn-Ismail",
				"ibn-Jafar", "ibn-Jawhar", "ibn-Khalil", "ibn-Mahdi", "ibn-Mahmud", "ibn-Mansur",
				"ibn-Muhammad", "ibn-Nasir", "ibn-Nizar", "ibn-Qasim", "ibn-Said", "ibn-Sulayman",
				"ibn-Tamim", "ibn-Umar", "ibn-Yahya", "ibn-Yusuf", "ibn-Zayd", "ibn-Ziri"
			},
			FemaleSecondNames: new[]
			{
				"bint-Abdallah", "bint-Abd-al-Aziz", "bint-Ahmad", "bint-Ali", "bint-Ammar", "bint-Dawud",
				"bint-Fadl", "bint-Faraj", "bint-Hasan", "bint-Husayn", "bint-Idris", "bint-Ismail",
				"bint-Jafar", "bint-Jawhar", "bint-Khalil", "bint-Mahdi", "bint-Mahmud", "bint-Mansur",
				"bint-Muhammad", "bint-Nasir", "bint-Nizar", "bint-Qasim", "bint-Said", "bint-Sulayman",
				"bint-Tamim", "bint-Umar", "bint-Yahya", "bint-Yusuf", "bint-Zayd", "bint-Ziri"
			},
			ThirdNames: MedievalFatimidNisbaInventory
		),
		new(
			Key: "Seljuk / Ayyubid-Mamluk",
			NameCultureName: "Medieval Seljuk Ayyubid Mamluk",
			EthnicityName: "High Medieval Oghuz Turk",
			EthnicGroup: "Turkic and Middle Eastern",
			EthnicSubgroup: "Seljuk-Ayyubid-Mamluk Sphere",
			CultureName: "Medieval Seljuk Ayyubid Mamluk (c. 1200)",
			NameForm: MedievalPeriodNameForm.GivenPatronymToponym,
			NameRegex: @"^(?<birthname>[\w'-]{2,}) (?<patronym>(?:ibn|bint)-[\w'-]{2,}) (?<toponym>al-[\w'-]{2,})$",
			ReferenceAnchor: "c. 1200",
			EthnicityDescription:
				"The Oghuz Turks are a Turkic-speaking people whose clans and military households have spread from the " +
				"steppe into Iran, Anatolia, Syria and northern Mesopotamia. They place themselves through tribal descent, " +
				"service to a ruler or atabeg, pastoral memory and Islam. Persian and Arabic court culture shape elite " +
				"life, but Turkic speech, clan names and mounted traditions remain defining marks.",
			CultureDescription:
				"Seljuk, Ayyubid and early Mamluk culture is found in mounted military households, citadels, towns, " +
				"caravan routes, villages, madrasas, Sufi institutions and courts of the crusading-era eastern " +
				"Mediterranean. Service ties, patronage, religious learning and multilingual administration bridge people " +
				"of many ancestries.",
			PersonalNameDescription:
				"Choose a Turkic, Persian or Arabic personal name used in an Islamicate military or courtly household. " +
				"Honorific compounds included as names remain one token.",
			SecondNameDescription:
				"Enter ibn-Father for a man or bint-Father for a woman. It identifies paternal descent and does not pass " +
				"unchanged to children.",
			ThirdNameDescription:
				"Enter an al- nisba for place, tribe, dynasty, military office, profession or religious affiliation, such " +
				"as al-Rumi, al-Turki or al-Atabaki.",
			MinimumSecondNameCount: 1,
			MinimumThirdNameCount: 200,
			BloodModel: "A Dominant",
			SkinProfile: "fair_olive_skin",
			HairProfile: "black_brown_grey_hair",
			EyeProfile: "brown_green_eyes",
			MaleGivenNames: new[]
			{
				"Alp-Arslan", "Atsiz", "Ayyub", "Badr", "Baybars", "Berkyaruq",
				"Dawud", "Duqaq", "Farrukh", "Ghazi", "Gumushtegin", "Hasan",
				"Husayn", "Ibrahim", "Il-Ghazi", "Imad-al-Din", "Ismail", "Izz-al-Din",
				"Jalal-al-Din", "Kilij-Arslan", "Mahmud", "Malik-Shah", "Mansur", "Masud",
				"Muhammad", "Muin-al-Din", "Muzaffar", "Nasir-al-Din", "Nur-al-Din", "Qutb-al-Din",
				"Qutuz", "Ridwan", "Rukn-al-Din", "Saif-al-Din", "Salah-al-Din", "Sanjar",
				"Shams-al-Din", "Sinan", "Suleyman", "Taj-al-Din", "Toghril", "Tughtakin",
				"Turanshah", "Umar", "Usama", "Yusuf", "Zangi", "Zayn-al-Din",
				"Aq-Sunqur", "Artuq"
			},
			FemaleGivenNames: new[]
			{
				"Adila", "Aisha", "Amina", "Asiya", "Dayfa", "Fatima",
				"Ghaziya", "Gokbori", "Halima", "Ismat", "Jamila", "Khadija",
				"Khatun", "Khayr", "Layla", "Malika", "Maryam", "Mu'mina",
				"Munira", "Nura", "Qamar", "Rabi'a", "Rahma", "Rayhana",
				"Raziyya", "Ruqayya", "Safiyya", "Salma", "Shajar-al-Durr", "Shamsa",
				"Sitt-al-Mulk", "Sukayna", "Turkan", "Zumurrud", "Zaynab", "Zahra",
				"Arwa", "Banu", "Dilshad", "Gawhar", "Gulbahar", "Gulnar",
				"Khurshid", "Mahin", "Nigar", "Pari", "Shirin", "Sitara",
				"Uljay", "Yasmine"
			},
			MaleSecondNames: new[]
			{
				"ibn-Ahmad", "ibn-Ali", "ibn-Ayyub", "ibn-Dawud", "ibn-Hasan", "ibn-Husayn",
				"ibn-Ibrahim", "ibn-Ismail", "ibn-Mahmud", "ibn-Malik", "ibn-Mansur", "ibn-Masud",
				"ibn-Muhammad", "ibn-Muzaffar", "ibn-Nasir", "ibn-Nur", "ibn-Qutb", "ibn-Ridwan",
				"ibn-Salah", "ibn-Sanjar", "ibn-Suleyman", "ibn-Toghril", "ibn-Umar", "ibn-Yusuf",
				"ibn-Zangi", "ibn-Artuq", "ibn-Aq-Sunqur", "ibn-Kilij-Arslan", "ibn-Tughtakin", "ibn-Turanshah"
			},
			FemaleSecondNames: new[]
			{
				"bint-Ahmad", "bint-Ali", "bint-Ayyub", "bint-Dawud", "bint-Hasan", "bint-Husayn",
				"bint-Ibrahim", "bint-Ismail", "bint-Mahmud", "bint-Malik", "bint-Mansur", "bint-Masud",
				"bint-Muhammad", "bint-Muzaffar", "bint-Nasir", "bint-Nur", "bint-Qutb", "bint-Ridwan",
				"bint-Salah", "bint-Sanjar", "bint-Suleyman", "bint-Toghril", "bint-Umar", "bint-Yusuf",
				"bint-Zangi", "bint-Artuq", "bint-Aq-Sunqur", "bint-Kilij-Arslan", "bint-Tughtakin", "bint-Turanshah"
			},
			ThirdNames: MedievalSeljukNisbaInventory
		),
		new(
			Key: "Magyar",
			NameCultureName: "Medieval Magyar",
			EthnicityName: "Conquest-Period Magyar",
			EthnicGroup: "Magyar",
			EthnicSubgroup: "Carpathian Basin",
			CultureName: "Medieval Magyar (c. 950)",
			NameForm: MedievalPeriodNameForm.GivenClan,
			NameRegex: @"^(?<birthname>[\w'-]{2,}) (?<familygroupname>[\w'-]{2,})$",
			ReferenceAnchor: "c. 950",
			EthnicityDescription:
				"The Magyars are a Ugric-speaking people of the Carpathian Basin, organised through kindreds, tribal " +
				"memory and the military followings of chiefs. Horse-breeding, mobile warfare, household descent and the " +
				"distribution of pasture and tribute distinguish them from neighbouring Slavic, German and steppe peoples. " +
				"Conversion and territorial kingship are beginning to alter older forms of identity.",
			CultureDescription:
				"Conquest-period Magyar culture combines mobile and settled pastoralism, farming, mounted retinues, " +
				"tribute, raiding, fortified centres and the early growth of territorial rule. Kindred, military " +
				"allegiance, pre-Christian practice and conversion pressures coexist.",
			PersonalNameDescription:
				"Choose a Magyar personal name suitable to the tenth-century Carpathian Basin. Steppe, Turkic and Ugric " +
				"forms are common, while Christian baptismal names become more prominent in later generations.",
			SecondNameDescription:
				"Choose the clan, kindred or dynastic group through which descent and political allegiance are claimed. It " +
				"is not an occupational surname.",
			ThirdNameDescription: "",
			MinimumSecondNameCount: 1,
			MinimumThirdNameCount: 0,
			BloodModel: "O-A High Negative",
			SkinProfile: "fair_skin",
			HairProfile: "black_brown_grey_hair",
			EyeProfile: "brown_green_blue_eyes",
			MaleGivenNames: new[]
			{
				"Almos", "Arpad", "Bese", "Bogat", "Botond", "Bulcsu",
				"Csaba", "Csanad", "Elod", "Fajsz", "Falicsi", "Geza",
				"Gyula", "Horka", "Huba", "Kadosa", "Kal", "Kende",
				"Kond", "Kurszan", "Lehel", "Levente", "Lelu", "Lompirt",
				"Ond", "Orseolo", "Sarolt", "Solt", "Sur", "Szabolcs",
				"Taksony", "Tarhos", "Tas", "Teteny", "Tormas", "Vajk",
				"Vata", "Zolta", "Aba", "Ajtony", "Becse", "Bors",
				"Csepel", "Doboka", "Erno", "Kean", "Kopasz", "Opour",
				"Torda", "Zombor"
			},
			FemaleGivenNames: new[]
			{
				"Sarolt", "Karold", "Emese", "Eniko", "Reka", "Tunde",
				"Piroska", "Ilona", "Erzsebet", "Judit", "Adelhaid", "Agnes",
				"Anna", "Beatrix", "Borbala", "Cecilia", "Christina", "Erzsebeth",
				"Eufemia", "Gertrud", "Gisela", "Helena", "Irene", "Katalin",
				"Klara", "Kunigunda", "Lucia", "Margit", "Maria", "Matild",
				"Rozsa", "Sophia", "Theodora", "Zsuzsanna", "Ajandek", "Aranka",
				"Boglarka", "Csenge", "Dalma", "Hajnal", "Ibolya", "Jolanka",
				"Kinga", "Liliom", "Orsolya", "Rona", "Szemere", "Virag",
				"Zsanett", "Zselyke"
			},
			MaleSecondNames: new[]
			{
				"Aba", "Arpad", "Becse-Gergely", "Csak", "Gyula-Zombor", "Hont-Pazmany",
				"Kalocsa", "Kaplon", "Kartal", "Koppan", "Kurszan", "Ludany",
				"Miscolc", "Osl", "Ratot", "Szalok", "Tomaj", "Turje",
				"Zovard", "Akos", "Borsa", "Gutkeled", "Herman", "Kacsics",
				"Kan", "Kokenyes-Radnot", "Pecs", "Pok", "Vaja", "Zsido"
			},
			FemaleSecondNames: new[]
			{
				"Aba", "Arpad", "Becse-Gergely", "Csak", "Gyula-Zombor", "Hont-Pazmany",
				"Kalocsa", "Kaplon", "Kartal", "Koppan", "Kurszan", "Ludany",
				"Miscolc", "Osl", "Ratot", "Szalok", "Tomaj", "Turje",
				"Zovard", "Akos", "Borsa", "Gutkeled", "Herman", "Kacsics",
				"Kan", "Kokenyes-Radnot", "Pecs", "Pok", "Vaja", "Zsido"
			},
			ThirdNames: Array.Empty<string>()
		),
		new(
			Key: "Rus / Novgorod",
			NameCultureName: "Medieval Rus Novgorod",
			EthnicityName: "High Medieval Rus",
			EthnicGroup: "Eastern Slavic",
			EthnicSubgroup: "Rus Lands",
			CultureName: "Medieval Rus Novgorod (c. 1100)",
			NameForm: MedievalPeriodNameForm.GivenPatronym,
			NameRegex: @"^(?<birthname>[\w'-]{2,}) (?<patronym>[\w'-]+(?:ovich|evich|ovna|evna|slavna|ichna|ich))$",
			ReferenceAnchor: "c. 1100",
			EthnicityDescription:
				"The Rus are the East Slavic-speaking Christian people of the river principalities from Kyiv to Novgorod. " +
				"They define belonging through paternal descent, town and land, princely dynasty, parish and service " +
				"relationship, while Orthodox worship and the shared written tradition of Rus join distant regions. Norse, " +
				"Finnic, Baltic and steppe ancestry is absorbed into a recognisable Rus identity.",
			CultureDescription:
				"Rus and Novgorodian culture includes princely courts, veche towns, merchant routes, farming settlements, " +
				"monasteries, tribute networks and military druzhina. Orthodox Christianity, customary law, river trade " +
				"and communal institutions vary across the Rus lands.",
			PersonalNameDescription: "Choose an East Slavic or Christian personal name. Older dynastic compounds coexist with baptismal names.",
			SecondNameDescription:
				"Choose a gendered patronym. Masculine forms generally end -ovich or -evich; feminine forms end -ovna or " +
				"-evna. It is formed from the father's personal name and changes each generation.",
			ThirdNameDescription: "",
			MinimumSecondNameCount: 1,
			MinimumThirdNameCount: 0,
			BloodModel: "O-A High Negative",
			SkinProfile: "fair_skin",
			HairProfile: "brown_blonde_grey_hair",
			EyeProfile: "brown_green_blue_eyes",
			MaleGivenNames: new[]
			{
				"Aleksandr", "Andrei", "Boris", "Bryachislav", "Davyd", "Dmitri",
				"Dobrynya", "Gleb", "Gostomysl", "Igor", "Izyaslav", "Konstantin",
				"Mikhail", "Mstislav", "Nikita", "Nikolai", "Oleg", "Ostromir",
				"Pavel", "Petr", "Radoslav", "Roman", "Rostislav", "Rurik",
				"Sviatopolk", "Sviatoslav", "Tverdislav", "Vadim", "Vasilko", "Vasili",
				"Vladimir", "Vsevolod", "Vyacheslav", "Yaroslav", "Yuri", "Zavid",
				"Zhdan", "Vojislav", "Volodar", "Volodislav", "Gavriil", "Ivan",
				"Kirill", "Luka", "Matvei", "Miroslav", "Ratibor", "Stanislav",
				"Sudislav", "Yakov"
			},
			FemaleGivenNames: new[]
			{
				"Agafia", "Agrafena", "Anastasia", "Anna", "Dobrodeya", "Dobronega",
				"Efrosinia", "Elena", "Evdokia", "Feodora", "Gorislava", "Ingeborg",
				"Irina", "Maria", "Marina", "Marta", "Miloslava", "Mstislava",
				"Olga", "Predslava", "Rogneda", "Rostislava", "Sbyslava", "Sofia",
				"Sviatoslava", "Vseslava", "Yaroslava", "Zvenislava", "Zbyslava", "Agatha",
				"Aleksandra", "Fevronia", "Glebava", "Ksenia", "Liubava", "Liudmila",
				"Malfrida", "Marfa", "Nadezhda", "Pelagia", "Praskovia", "Radoslava",
				"Romanova", "Sviatava", "Tatianna", "Ulyana", "Varvara", "Vera",
				"Vira", "Zlata"
			},
			MaleSecondNames: new[]
			{
				"Ivanovich", "Petrovich", "Vasilievich", "Dmitrievich", "Mikhailovich", "Nikolaevich",
				"Andreevich", "Borisovich", "Glebovich", "Igorevich", "Izyaslavich", "Konstantinovich",
				"Mstislavich", "Olegovich", "Pavlovich", "Romanovich", "Rostislavich", "Rurikovich",
				"Sviatopolkovich", "Sviatoslavich", "Vladimirovich", "Vsevolodovich", "Vyacheslavich", "Yaroslavich",
				"Yurievich", "Davydovich", "Gavriilovich", "Kirillovich", "Lukich", "Yakovlevich"
			},
			FemaleSecondNames: new[]
			{
				"Ivanovna", "Petrovna", "Vasilievna", "Dmitrievna", "Mikhailovna", "Nikolaevna",
				"Andreevna", "Borisovna", "Glebovna", "Igorevna", "Izyaslavna", "Konstantinovna",
				"Mstislavna", "Olegovna", "Pavlovna", "Romanovna", "Rostislavna", "Rurikovna",
				"Sviatopolkovna", "Sviatoslavna", "Vladimirovna", "Vsevolodovna", "Vyacheslavna", "Yaroslavna",
				"Yurievna", "Davydovna", "Gavriilovna", "Kirillovna", "Lukinichna", "Yakovlevna"
			},
			ThirdNames: Array.Empty<string>()
		),
		new(
			Key: "Steppe Turkic / Mongol",
			NameCultureName: "Medieval Steppe Turkic Mongol",
			EthnicityName: "High Medieval Kipchak Turk",
			EthnicGroup: "Central Asian",
			EthnicSubgroup: "Turkic-Mongol Steppe",
			CultureName: "Medieval Steppe Turkic Mongol (c. 1200)",
			NameForm: MedievalPeriodNameForm.GivenClan,
			NameRegex: @"^(?<birthname>[\w'-]{2,}) (?<familygroupname>[\w'-]{2,})$",
			ReferenceAnchor: "c. 1200",
			EthnicityDescription:
				"The Kipchaks are a Turkic-speaking confederational people of the western Eurasian steppe. They define " +
				"themselves through clan and tribal descent, pasture and migration routes, mounted military following and " +
				"relationships of alliance or subordination with neighbouring peoples. Household herds, remembered " +
				"genealogy and freedom of movement are central to Kipchak identity.",
			CultureDescription:
				"Steppe Turkic and early Mongol culture is built around mobile pastoral households, camps, herds, hunting, " +
				"mounted warfare, tribute, trade and courts formed from personal allegiance. Clan politics and religious " +
				"traditions ranging from shamanic practice to Christianity, Buddhism and Islam coexist across the steppe.",
			PersonalNameDescription:
				"Choose a Turkic or Mongolic personal name. Animal, quality, celestial and auspicious compounds are " +
				"common, and many forms are not rigidly sex-exclusive.",
			SecondNameDescription:
				"Choose the clan, tribe or confederational affiliation. It places the character within a political and " +
				"genealogical network rather than a fixed modern surname.",
			ThirdNameDescription: "",
			MinimumSecondNameCount: 1,
			MinimumThirdNameCount: 0,
			BloodModel: "A Dominant",
			SkinProfile: "golden_skin",
			HairProfile: "black_brown_grey_hair",
			EyeProfile: "brown_green_eyes",
			MaleGivenNames: new[]
			{
				"Abaqa", "Alchi", "Altan", "Arghun", "Baraq", "Batu",
				"Baydu", "Belgutei", "Berke", "Boroqul", "Bo'orchu", "Buqa",
				"Chagatai", "Chilaun", "Chormaqan", "Dayir", "Eljigidei", "Guyuk",
				"Hulegu", "Jebe", "Jelme", "Jochi", "Kadan", "Kaidu",
				"Khasar", "Khubilai", "Kitbuqa", "Kokochu", "Korguz", "Mongke",
				"Muqali", "Nogai", "Ogedei", "Orda", "Qadan", "Qaidu",
				"Qara", "Qasar", "Qubilai", "Qutlugh", "Sartaq", "Shiban",
				"Subutai", "Temuge", "Temujin", "Toghrul", "Tolui", "Tuda-Mongke",
				"Yesugei", "Yisubugei"
			},
			FemaleGivenNames: new[]
			{
				"Alakhai", "Altani", "Alun", "Beki", "Borte", "Bulughan",
				"Checheyigen", "Chubei", "Doquz", "El-Qutlugh", "Fatima", "Guchulug",
				"Hoelun", "Ibaqa", "Kelmish", "Khutulun", "Kokechin", "Korguz",
				"Mandukhai", "Moge", "Nambui", "Oghul-Qaimish", "Orghana", "Qutui",
				"Qutulun", "Sorghaghtani", "Toghanchuk", "Toregene", "Yisui", "Yisugen",
				"Alaqai", "Babai", "Bektemish", "Bibi", "Chabi", "Ebuskun",
				"Ergene", "Giyuk", "Gurbesu", "Ilinchig", "Kerey", "Khatun",
				"Qadaqach", "Qoruqchin", "Sati", "Tului", "Uljay", "Yesun",
				"Zubeida", "Qutlugh-Timur"
			},
			MaleSecondNames: new[]
			{
				"Borjigin", "Tayichiud", "Kereit", "Naiman", "Merkit", "Onggirat",
				"Oirat", "Jalair", "Manghit", "Qipchaq", "Karluk", "Kimek",
				"Kangli", "Cuman", "Pecheneg", "Khitan", "Tatar", "Uighur",
				"Barlas", "Arlat", "Suldus", "Besud", "Baarin", "Uriankhai",
				"Qongqotan", "Qatagin", "Saljiut", "Dorgin", "Bayaut", "Sulduz"
			},
			FemaleSecondNames: new[]
			{
				"Borjigin", "Tayichiud", "Kereit", "Naiman", "Merkit", "Onggirat",
				"Oirat", "Jalair", "Manghit", "Qipchaq", "Karluk", "Kimek",
				"Kangli", "Cuman", "Pecheneg", "Khitan", "Tatar", "Uighur",
				"Barlas", "Arlat", "Suldus", "Besud", "Baarin", "Uriankhai",
				"Qongqotan", "Qatagin", "Saljiut", "Dorgin", "Bayaut", "Sulduz"
			},
			ThirdNames: Array.Empty<string>()
		),
		new(
			Key: "North Indian / Rajput",
			NameCultureName: "Medieval North Indian Rajput",
			EthnicityName: "Early Medieval Rajput",
			EthnicGroup: "South Asian",
			EthnicSubgroup: "North Indian Rajput Polities",
			CultureName: "Medieval North Indian Rajput (c. 1100)",
			NameForm: MedievalPeriodNameForm.GivenClan,
			NameRegex: @"^(?<birthname>[\w'-]{2,}) (?<familygroupname>[\w'-]{2,})$",
			ReferenceAnchor: "c. 1100",
			EthnicityDescription:
				"The Rajputs are warrior and landholding lineages of northern and north-western India who claim descent " +
				"from royal, heroic or sacred ancestors. Clan, branch, marriage alliance, fort and estate, temple " +
				"patronage and bardic genealogy define their collective honour. Their identity is maintained through " +
				"remembered service and descent rather than residence in any single kingdom.",
			CultureDescription:
				"Early medieval Rajput culture centres on fortified courts, temple patronage, estate villages, cavalry and " +
				"retainer service, merchant links and bardic-genealogical memory. Clan alliance, marriage, land revenue " +
				"and relations with cultivators and religious institutions organise political life.",
			PersonalNameDescription:
				"Choose a Sanskritic or vernacular personal name. Regnal epithets and titles are excluded from this " +
				"element.",
			SecondNameDescription:
				"Choose the clan or dynastic affiliation through which ancestry, marriage rules and political honour are " +
				"recognised.",
			ThirdNameDescription: "",
			MinimumSecondNameCount: 1,
			MinimumThirdNameCount: 0,
			BloodModel: "B Dominant",
			SkinProfile: "swarthy_skin",
			HairProfile: "black_brown_grey_hair",
			EyeProfile: "brown_green_eyes",
			MaleGivenNames: new[]
			{
				"Ajayapala", "Ajayaraja", "Arnoraja", "Bhoja", "Bhima", "Chandradeva",
				"Chahada", "Devapala", "Durlabharaja", "Govindachandra", "Hariraja", "Jayachandra",
				"Jayasimha", "Karanadeva", "Kirtipala", "Kumarapala", "Lakshmanasena", "Madanapala",
				"Mahipala", "Mularaja", "Nagabhata", "Naravarman", "Narayanapala", "Paramardi",
				"Prithviraja", "Rajyapala", "Ramapala", "Ratnapala", "Rudradaman", "Samarasimha",
				"Sangramaraja", "Siddharaja", "Simharaja", "Someshvara", "Trailokyamalla", "Udayaditya",
				"Vakpati", "Vigraharaja", "Vijayachandra", "Vikramaditya", "Yashovarman", "Anangapala",
				"Arjunavarman", "Dharmapala", "Gopala", "Harsha", "Kalasha", "Kirtivarman",
				"Nagadeva", "Pratapadhavala"
			},
			FemaleGivenNames: new[]
			{
				"Abhayamati", "Anangadevi", "Anupama", "Asaladevi", "Bhavadevi", "Bhuvanamati",
				"Chandralekha", "Devaladevi", "Didda", "Durlabhadevi", "Gauri", "Gunamati",
				"Jayamati", "Kanchanadevi", "Karpuradevi", "Karnavati", "Kumaradevi", "Lakshmidevi",
				"Lilavati", "Madalasa", "Mahadevi", "Minaladevi", "Naikidevi", "Nayanadevi",
				"Padmavati", "Prabhavati", "Prithvidevi", "Rajamati", "Rajyashri", "Ratnadevi",
				"Rudramadevi", "Samyukta", "Shashiprabha", "Somaladevi", "Suryamati", "Tribhuvanadevi",
				"Udayamati", "Vasantadevi", "Vijayadevi", "Yashodevi", "Ambika", "Bhagyavati",
				"Devika", "Gandhari", "Kamaladevi", "Kalyanadevi", "Padmini", "Ratnamala",
				"Subhadradevi", "Tilottama"
			},
			MaleSecondNames: new[]
			{
				"Chauhan", "Gahadavala", "Paramara", "Pratihara", "Chandel", "Solanki",
				"Chalukya", "Guhila", "Tomara", "Kalachuri", "Kachchhapaghata", "Chahamana",
				"Chaulukya", "Ganga", "Sena", "Pala", "Rathore", "Kachhwaha",
				"Sisodia", "Bhati", "Parihar", "Hada", "Deora", "Songara",
				"Jhala", "Gohil", "Vaghela", "Bundela", "Mori", "Panwar"
			},
			FemaleSecondNames: new[]
			{
				"Chauhan", "Gahadavala", "Paramara", "Pratihara", "Chandel", "Solanki",
				"Chalukya", "Guhila", "Tomara", "Kalachuri", "Kachchhapaghata", "Chahamana",
				"Chaulukya", "Ganga", "Sena", "Pala", "Rathore", "Kachhwaha",
				"Sisodia", "Bhati", "Parihar", "Hada", "Deora", "Songara",
				"Jhala", "Gohil", "Vaghela", "Bundela", "Mori", "Panwar"
			},
			ThirdNames: Array.Empty<string>()
		),
		new(
			Key: "South Indian / Chola",
			NameCultureName: "Medieval South Indian Chola",
			EthnicityName: "Chola-Era Tamil",
			EthnicGroup: "South Asian",
			EthnicSubgroup: "Chola and Southern India",
			CultureName: "Medieval South Indian Chola (c. 1050)",
			NameForm: MedievalPeriodNameForm.GivenClan,
			NameRegex: @"^(?<birthname>[\w'-]{2,}) (?<familygroupname>[\w'-]{2,})$",
			ReferenceAnchor: "c. 1050",
			EthnicityDescription:
				"The Tamils are the Tamil-speaking people of the far south of India and northern Sri Lanka. They define " +
				"belonging through language, locality, kin and caste or occupational community, temple and devotional " +
				"tradition, and attachment to a crowned dynasty or local assembly. Tamil literary memory and the networks " +
				"of merchants and temples join coast, delta and upland.",
			CultureDescription:
				"Chola culture encompasses irrigated villages, temple-centred institutions, merchant corporations, craft " +
				"communities, royal and local administration, naval and overland trade and military service. Land rights, " +
				"occupational organisation, devotion and locality are the chief social anchors.",
			PersonalNameDescription:
				"Choose a Tamil or Sanskritic personal name. Devotional, royal and vernacular forms all occur; titles and " +
				"regnal phrases are excluded.",
			SecondNameDescription:
				"Choose a dynasty, house, locality or corporate affiliation. It may name a ruling line, a region, a " +
				"temple-linked body or a service community and is kept as one token.",
			ThirdNameDescription: "",
			MinimumSecondNameCount: 1,
			MinimumThirdNameCount: 0,
			BloodModel: "B Dominant",
			SkinProfile: "swarthy_skin",
			HairProfile: "black_brown_grey_hair",
			EyeProfile: "brown_green_eyes",
			MaleGivenNames: new[]
			{
				"Aditya", "Arinjaya", "Arulmozhi", "Athirajendra", "Chola", "Gandaraditya",
				"Karikala", "Kulothunga", "Madhurantaka", "Parantaka", "Rajadhiraja", "Rajaraja",
				"Rajendra", "Uttama", "Vijayalaya", "Virarajendra", "Vikkirama", "Sundara",
				"Kandaraditya", "Kopperunchinga", "Kulotunga", "Maravarman", "Jatavarman", "Vira-Pandya",
				"Srivallabha", "Nedunjeliyan", "Cheraman", "Bhaskara", "Sthanu-Ravi", "Rama-Varma",
				"Kochadaiyan", "Manavikrama", "Govinda", "Kesava", "Narasimha", "Devaraya",
				"Someshvara", "Vishnuvardhana", "Ballala", "Ereyanga", "Bukka", "Harihara",
				"Kampana", "Mallikarjuna", "Tirumala", "Virupaksha", "Appan", "Perumal",
				"Nambi", "Velan"
			},
			FemaleGivenNames: new[]
			{
				"Kundavai", "Sembiyan-Mahadevi", "Vanavan-Mahadevi", "Lokamahadevi", "Panchavan-Mahadevi", "Trailokyamahadevi",
				"Viramahadevi", "Tribhuvanamahadevi", "Mukkokilan", "Madhurantaki", "Ammangadevi", "Arulmoli",
				"Kalyani", "Komalavalli", "Mangai", "Nangai", "Paravai", "Ponnambal",
				"Rajarajadevi", "Rajendradevi", "Sembiyan", "Vanathi", "Valli", "Andal",
				"Avvai", "Ilango", "Karpagam", "Kaveri", "Kayal", "Kundalakesi",
				"Madhavi", "Manimekalai", "Meenakshi", "Nallal", "Padmavati", "Pavai",
				"Ponni", "Selvi", "Sita", "Sivakami", "Sundari", "Thamarai",
				"Tilakavathi", "Uma", "Vaanama", "Vasanthi", "Vennila", "Vimaladevi",
				"Yashodadevi", "Kanchanadevi"
			},
			MaleSecondNames: new[]
			{
				"Chola", "Pandya", "Chera", "Pallava", "Ganga", "Kadamba",
				"Hoysala", "Chalukya", "Rashtrakuta", "Bana", "Muttaraiyar", "Velir",
				"Sambuvaraya", "Irukkuvel", "Malayaman", "Kongu", "Tondai", "Kaveri",
				"Uraiyur", "Thanjavur", "Gangaikonda", "Kanchipuram", "Madurai", "Chidambaram",
				"Nagapattinam", "Korkai", "Vengi", "Kalyani", "Dvarasamudra", "Kollam"
			},
			FemaleSecondNames: new[]
			{
				"Chola", "Pandya", "Chera", "Pallava", "Ganga", "Kadamba",
				"Hoysala", "Chalukya", "Rashtrakuta", "Bana", "Muttaraiyar", "Velir",
				"Sambuvaraya", "Irukkuvel", "Malayaman", "Kongu", "Tondai", "Kaveri",
				"Uraiyur", "Thanjavur", "Gangaikonda", "Kanchipuram", "Madurai", "Chidambaram",
				"Nagapattinam", "Korkai", "Vengi", "Kalyani", "Dvarasamudra", "Kollam"
			},
			ThirdNames: Array.Empty<string>()
		),
		new(
			Key: "Song China",
			NameCultureName: "Medieval Song Chinese",
			EthnicityName: "Song Han Chinese",
			EthnicGroup: "East Asian",
			EthnicSubgroup: "Song China",
			CultureName: "Medieval Song Chinese (c. 1100)",
			NameForm: MedievalPeriodNameForm.FamilyGiven,
			NameRegex: @"^(?<surname>[\w'-]{1,}) (?<birthname>[\w'-]{1,})$",
			ReferenceAnchor: "c. 1100",
			EthnicityDescription:
				"The Han Chinese are the majority people of the Song realm, bound by written Chinese, patrilineal ancestry " +
				"and shared classical and ritual traditions. Native county, lineage, household genealogy and local speech " +
				"remain powerful markers within the wider Han identity. Farming, urban commerce and examination culture " +
				"connect regions of great linguistic diversity.",
			CultureDescription:
				"Song culture is organised around lineage and household life, intensive agriculture, market towns, great " +
				"commercial cities, craft production, examination education, temples, state taxation and river or maritime " +
				"trade.",
			PersonalNameDescription:
				"Choose the personal name that follows the family name. One or two syllables are rendered as a single " +
				"token; courtesy, childhood and literary names are separate aliases.",
			SecondNameDescription: "Choose the patrilineal family name. It is inherited and comes before the personal name.",
			ThirdNameDescription: "",
			MinimumSecondNameCount: 1,
			MinimumThirdNameCount: 0,
			BloodModel: "B Dominant",
			SkinProfile: "golden_skin",
			HairProfile: "black_brown_grey_hair",
			EyeProfile: "brown_green_eyes",
			MaleGivenNames: new[]
			{
				"Kuangyin", "Guangyi", "Heng", "Zhen", "Shu", "Zhao",
				"Xu", "Ji", "Gou", "Shen", "Anshi", "Bi",
				"Chao", "Chun", "Dunyi", "Gong", "Gongliang", "Gu",
				"Jiuyuan", "Kuo", "Meng", "Ouyang-Xiu", "Qi", "Shi",
				"Shihao", "Shiwen", "Shizheng", "Su-Shi", "Su-Zhe", "Tingjian",
				"Wenyan", "Xiang", "Xiu", "Yi", "You", "Zai",
				"Zong", "Zhongyan", "Zhu-Xi", "Zizheng", "Ba", "Bo",
				"Chen", "Dao", "Fu", "Hao", "Jing", "Liang",
				"Ming", "Rui"
			},
			FemaleGivenNames: new[]
			{
				"Ailan", "Bao", "Cai", "Chao", "Chun", "Cui",
				"Dan", "E", "Fang", "Fen", "Gui", "He",
				"Hong", "Hua", "Huan", "Hui", "Jia", "Jiao",
				"Jing", "Juan", "Lan", "Lian", "Ling", "Mei",
				"Min", "Ning", "Pei", "Qiao", "Qing", "Rong",
				"Ru", "Shan", "Shu", "Su", "Ting", "Wan",
				"Xi", "Xia", "Xian", "Xiang", "Xiu", "Xue",
				"Yan", "Ying", "Yu", "Yue", "Yun", "Zhen",
				"Zhu", "Shishi"
			},
			MaleSecondNames: new[]
			{
				"Zhao", "Wang", "Li", "Zhang", "Liu", "Chen",
				"Yang", "Huang", "Zhou", "Wu", "Xu", "Sun",
				"Ma", "Zhu", "Hu", "Guo", "Lin", "He",
				"Gao", "Liang", "Zheng", "Luo", "Song", "Xie",
				"Han", "Tang", "Feng", "Yu", "Dong", "Xiao",
				"Cheng", "Cao", "Yuan", "Deng", "Shen", "Zeng",
				"Peng", "Lu", "Su", "Ouyang"
			},
			FemaleSecondNames: new[]
			{
				"Zhao", "Wang", "Li", "Zhang", "Liu", "Chen",
				"Yang", "Huang", "Zhou", "Wu", "Xu", "Sun",
				"Ma", "Zhu", "Hu", "Guo", "Lin", "He",
				"Gao", "Liang", "Zheng", "Luo", "Song", "Xie",
				"Han", "Tang", "Feng", "Yu", "Dong", "Xiao",
				"Cheng", "Cao", "Yuan", "Deng", "Shen", "Zeng",
				"Peng", "Lu", "Su", "Ouyang"
			},
			ThirdNames: Array.Empty<string>()
		),
		new(
			Key: "Goryeo Korea",
			NameCultureName: "Medieval Goryeo Korean",
			EthnicityName: "Goryeo Korean",
			EthnicGroup: "East Asian",
			EthnicSubgroup: "Goryeo Korea",
			CultureName: "Medieval Goryeo Korean (c. 1150)",
			NameForm: MedievalPeriodNameForm.FamilyGiven,
			NameRegex: @"^(?<surname>[\w'-]{1,}) (?<birthname>[\w'-]{1,})$",
			ReferenceAnchor: "c. 1150",
			EthnicityDescription:
				"The Koreans are the Korean-speaking people of the Goryeo kingdom. Patrilineal clan, family name and " +
				"ancestral seat place a person within a remembered genealogy, while province, village, rank and occupation " +
				"shape local belonging. Court tradition, Buddhism and a shared written culture distinguish them from " +
				"neighbouring Jurchen, Khitan, Chinese and Japanese peoples.",
			CultureDescription:
				"Goryeo culture spans aristocratic and military households, Buddhist monasteries, county administration, " +
				"farming villages, craft production, markets and maritime connections. Lineage status, landholding, " +
				"corvee, temple patronage and court office shape opportunity.",
			PersonalNameDescription:
				"Choose the personal name following the family name. One- and two-syllable names are written as a single " +
				"token; courtesy and Buddhist names are separate.",
			SecondNameDescription:
				"Choose the family name. The fuller clan identity also includes a bon-gwan, or ancestral seat, though the " +
				"compact generated form records only the surname.",
			ThirdNameDescription: "",
			MinimumSecondNameCount: 1,
			MinimumThirdNameCount: 0,
			BloodModel: "B Dominant",
			SkinProfile: "golden_skin",
			HairProfile: "black_brown_grey_hair",
			EyeProfile: "brown_green_eyes",
			MaleGivenNames: new[]
			{
				"Geon", "Mu", "Yo", "So", "Ju", "Song",
				"Sun", "Sun-heon", "Hyeon", "Hwi", "Hun", "Cheol",
				"Hae", "Jeong", "Ong", "Tak", "Wang-u", "Wang-ho",
				"Wang-cheol", "Wang-gi", "Wang-hun", "Wang-yo", "Wang-so", "Wang-sun",
				"Wang-song", "Wang-hyeon", "Wang-hwi", "Gang-gam-chan", "Yun-gwan", "Seo-hui",
				"Choe-chung", "Choe-u", "Choe-hang", "Choe-ui", "Kim-bu-sik", "Jeong-jung-bu",
				"Yi-ui-bang", "Gyeong-dae-seung", "Yi-ui-min", "Choe-chung-heon", "Kim-yun-hu", "An-hyang",
				"Yi-je-hyeon", "Jeong-mong-ju", "Yi-saek", "Choe-yeong", "Yi-seong-gye", "Jo-jun",
				"Jeong-do-jeon", "Yi-bang-won"
			},
			FemaleGivenNames: new[]
			{
				"Sinmyeong", "Sinjeong", "Sinseong", "Jeongdeok", "Daemok", "Heonae",
				"Heonjeong", "Wonjeong", "Wonhwa", "Wonseong", "Wonhye", "Wonmok",
				"Wonpyeong", "Inpyeong", "Inye", "Inmok", "Inwon", "Jeonghwa",
				"Jeongmok", "Jeongui", "Seonjeong", "Seongpyeong", "Seungdeok", "Janggyeong",
				"Jangsin", "Janghwa", "Gyeongseong", "Gyeonghwa", "Gyeongmok", "Gyeongsun",
				"Gongye", "Gongwon", "Myeongui", "Munjeong", "Munhwa", "Sukchang",
				"Sukbi", "Sukgyeong", "Sukmok", "Sukui", "Deoknyeong", "Noguk",
				"Jeongbi", "Sunjeong", "Uihwa", "Hyojeong", "Hyohoe", "Hyesun",
				"Yeonghwa", "Cheonchu"
			},
			MaleSecondNames: new[]
			{
				"Wang", "Kim", "Yi", "Choe", "Jeong", "Yun",
				"Pak", "Seo", "Gang", "Jo", "Im", "Han",
				"O", "Jang", "Sin", "Gwon", "Hwang", "An",
				"Song", "Ryu", "Hong", "Jeon", "Go", "Mun",
				"Yang", "Son", "Bae", "Baek", "Heo", "Nam",
				"Sim", "No", "Ha", "Gwak", "Seong", "Cha",
				"Ju", "Gu", "Min", "Jin"
			},
			FemaleSecondNames: new[]
			{
				"Wang", "Kim", "Yi", "Choe", "Jeong", "Yun",
				"Pak", "Seo", "Gang", "Jo", "Im", "Han",
				"O", "Jang", "Sin", "Gwon", "Hwang", "An",
				"Song", "Ryu", "Hong", "Jeon", "Go", "Mun",
				"Yang", "Son", "Bae", "Baek", "Heo", "Nam",
				"Sim", "No", "Ha", "Gwak", "Seong", "Cha",
				"Ju", "Gu", "Min", "Jin"
			},
			ThirdNames: Array.Empty<string>()
		),
		new(
			Key: "Heian / Kamakura Japan",
			NameCultureName: "Medieval Heian Kamakura Japanese",
			EthnicityName: "Heian-Kamakura Japanese",
			EthnicGroup: "East Asian",
			EthnicSubgroup: "Heian and Kamakura Japan",
			CultureName: "Medieval Heian Kamakura Japanese (c. 1200)",
			NameForm: MedievalPeriodNameForm.FamilyGiven,
			NameRegex: @"^(?<surname>[\w'-]{1,}) (?<birthname>[\w'-]{1,})$",
			ReferenceAnchor: "c. 1200",
			EthnicityDescription:
				"The Japanese are the Japanese-speaking people of the islands under imperial, court, temple and emerging " +
				"warrior authority. Clan descent, house and estate, province and religious institution define belonging, " +
				"with court aristocrats, warriors, townspeople and villagers expressing status in different ways. Shared " +
				"shrine, Buddhist and courtly traditions bind these local identities.",
			CultureDescription:
				"Late Heian and early Kamakura culture encompasses estate agriculture, Kyoto court networks, emerging " +
				"warrior government, provincial households, temples, shrines, ports and craft settlements. Rank, house " +
				"affiliation, patronage and religious institution govern names and social position.",
			PersonalNameDescription:
				"Choose the adult personal name that follows the clan or family name. Childhood names, court titles, " +
				"Buddhist names and later name changes are separate identities.",
			SecondNameDescription:
				"Choose the clan or family name. It precedes the personal name and locates the character within a " +
				"genealogical and political house.",
			ThirdNameDescription: "",
			MinimumSecondNameCount: 1,
			MinimumThirdNameCount: 0,
			BloodModel: "B Dominant",
			SkinProfile: "golden_skin",
			HairProfile: "black_brown_grey_hair",
			EyeProfile: "brown_green_eyes",
			MaleGivenNames: new[]
			{
				"Atsuhira", "Atsumori", "Chikafusa", "Kagetoki", "Kiyomori", "Koremori",
				"Masakado", "Masako", "Michinaga", "Michizane", "Moritomo", "Munemori",
				"Nobunori", "Norimori", "Noriyori", "Sanemori", "Sanetomo", "Shigehira",
				"Shigemori", "Sukemori", "Tadamori", "Tadanori", "Tadatsune", "Takamori",
				"Takamune", "Tameie", "Tametomo", "Tokiwa", "Tomomori", "Tsunemori",
				"Yorimasa", "Yorinobu", "Yoritomo", "Yoritsune", "Yoriyoshi", "Yoshihira",
				"Yoshikata", "Yoshinaka", "Yoshitomo", "Yoshitsune", "Hidesato", "Ietada",
				"Kintoki", "Korechika", "Mitsunaka", "Raiko", "Sadamori", "Tadamichi",
				"Taira-no-Masakado", "Minamoto-no-Yoshiie"
			},
			FemaleGivenNames: new[]
			{
				"Akiko", "Atsuko", "Chikako", "Fujiwara-no-Kenshi", "Fujiwara-no-Teishi", "Hachijo-in",
				"Ishi", "Kenshi", "Kishi", "Masako", "Michiko", "Murasaki",
				"Nariko", "Noriko", "Reishi", "Renshi", "Shikishi", "Shoshi",
				"Shunzei-kyo", "Tamako", "Tashi", "Teishi", "Tokushi", "Toku",
				"Toshi", "Yasuko", "Yoshiko", "Yushi", "Ben-no-Naishi", "Daini-no-Sanmi",
				"Echizen", "Izumi-Shikibu", "Kaga", "Koshikibu", "Lady-Ise", "Lady-Nijo",
				"Lady-Sanuki", "Lady-Sarashina", "Sei-Shonagon", "Suo-no-Naishi", "Takasue-no-Musume", "Tosa",
				"Ukyonomiya", "Ukon", "Akazome", "Gishi", "Kyo-no-Tsubone", "Sen",
				"Tokiwa-Gozen", "Tomoe"
			},
			MaleSecondNames: new[]
			{
				"Fujiwara", "Taira", "Minamoto", "Tachibana", "Sugawara", "Abe",
				"Oe", "Ki", "Miyoshi", "Nakatomi", "Kiyohara", "Sakanoue",
				"Ono", "Mononobe", "Hata", "Otomo", "Hojo", "Miura",
				"Chiba", "Hatakeyama", "Wada", "Kajiwara", "Adachi", "Hiki",
				"Nikaido", "Ashikaga", "Nitta", "Sasaki", "Ogasawara", "Takeda",
				"Shimazu", "Kudo", "Utsunomiya", "Kono", "Doi", "Kumagai",
				"Kiso", "Imai", "Sato", "Kikkawa"
			},
			FemaleSecondNames: new[]
			{
				"Fujiwara", "Taira", "Minamoto", "Tachibana", "Sugawara", "Abe",
				"Oe", "Ki", "Miyoshi", "Nakatomi", "Kiyohara", "Sakanoue",
				"Ono", "Mononobe", "Hata", "Otomo", "Hojo", "Miura",
				"Chiba", "Hatakeyama", "Wada", "Kajiwara", "Adachi", "Hiki",
				"Nikaido", "Ashikaga", "Nitta", "Sasaki", "Ogasawara", "Takeda",
				"Shimazu", "Kudo", "Utsunomiya", "Kono", "Doi", "Kumagai",
				"Kiso", "Imai", "Sato", "Kikkawa"
			},
			ThirdNames: Array.Empty<string>()
		)
	};
}

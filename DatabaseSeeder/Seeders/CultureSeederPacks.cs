using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class CultureSeeder
{
	private FutureProg _alwaysFalseProg;

	private FutureProg _alwaysTrueProg;

	private readonly Dictionary<string, PopulationBloodModel> _bloodModels = new(StringComparer.OrdinalIgnoreCase);
	private FuturemudDatabaseContext _context;

	private readonly Dictionary<string, CharacteristicDefinition> _definitions = new(StringComparer.OrdinalIgnoreCase);

	private Race _humanRace;

	private TraitDefinition _intelligenceTrait;

	private readonly Dictionary<string, CharacteristicProfile> _profiles = new(StringComparer.OrdinalIgnoreCase);

	private FutureProg? _skillStartProg;

	public void SeedCulturePacks(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		_context = context;
		_humanRace = context.Races.First(x => x.Name == "Human");
		_alwaysTrueProg = context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue");
		_alwaysFalseProg = context.FutureProgs.First(x => x.FunctionName == "AlwaysFalse");
		_skillStartProg = context.FutureProgs.FirstOrDefault(x => x.FunctionName == "SkillStartingValue");
		_intelligenceTrait =
			context.TraitDefinitions
				.Where(x => x.Type == 1 || x.Type == 3)
				.AsEnumerable()
				.FirstOrDefault(x => x.Name.In("Intelligence", "Intellect", "Wisdom", "Mind")) ??
			context.TraitDefinitions
				.Where(x => x.Type == 1 || x.Type == 3)
				.AsEnumerable()
				.First(x => x.Name.In("Wisdom"))
			;
		foreach (var value in context.PopulationBloodModels.ToList()) _bloodModels[value.Name] = value;
		if (_skillStartProg == null)
		{
			_skillStartProg = new FutureProg
			{
				FunctionName = "SkillStartingValue",
				Category = "Chargen",
				Subcategory = "Skills",
				FunctionComment = "Used to determine the opening value for a skill at character creation",
				ReturnType = (int)ProgVariableTypes.Number,
				AcceptsAnyParameters = false,
				Public = false,
				StaticType = 0,
				FunctionText = @"if (@trait.group == ""Language"")
   return 100 + (@boosts * 50)
 end if
 return 20 + (@boosts * 15)"
			};
			_skillStartProg.FutureProgsParameters.Add(new FutureProgsParameter
			{
				FutureProg = _skillStartProg, ParameterIndex = 0, ParameterName = "ch",
				ParameterType = (int)ProgVariableTypes.Toon
			});
			_skillStartProg.FutureProgsParameters.Add(new FutureProgsParameter
			{
				FutureProg = _skillStartProg, ParameterIndex = 1, ParameterName = "trait",
				ParameterType = (int)ProgVariableTypes.Trait
			});
			_skillStartProg.FutureProgsParameters.Add(new FutureProgsParameter
			{
				FutureProg = _skillStartProg, ParameterIndex = 2, ParameterName = "boosts",
				ParameterType = (int)ProgVariableTypes.Number
			});
			_context.FutureProgs.Add(_skillStartProg);
			_context.SaveChanges();
		}

		foreach (var definition in _humanRace.RacesAdditionalCharacteristics.Concat(
			         _humanRace.ParentRace?.RacesAdditionalCharacteristics ??
			         Enumerable.Empty<RacesAdditionalCharacteristics>()))
		{
			_definitions[definition.CharacteristicDefinition.Name] = definition.CharacteristicDefinition;
			foreach (var profile in context.CharacteristicProfiles.Where(x =>
				         x.TargetDefinition == definition.CharacteristicDefinition))
				_profiles[profile.Name.ToLowerInvariant()] = profile;
		}

		switch (questionAnswers["culturepacks"].ToLowerInvariant())
		{
			case "earth-modern":
				SeedEarthModern(questionAnswers);
				return;
			case "earth-medievaleurope":
				SeedMedievalEurope(questionAnswers);
				return;
			case "earth-antiquity":
				SeedRepublicanRome(questionAnswers);
				return;
			case "middle-earth":
				SeedMiddleEarth(questionAnswers);
				return;
		}
	}

	public void SeedEarthModern(IReadOnlyDictionary<string, string> questionAnswers)
	{
		if (questionAnswers["seednames"].EqualToAny("y", "yes")) SeedEuropeanNames(_context);

		if (questionAnswers["seedlanguages"].EqualToAny("y", "yes")) SeedModernLanguages();

		if (questionAnswers["seedheritage"].EqualToAny("y", "yes")) SeedModernHeritage();
	}

	public void SeedMedievalEurope(IReadOnlyDictionary<string, string> questionAnswers)
	{
		// TODO
		if (questionAnswers["seednames"].EqualToAny("y", "yes"))
		{
		}

		if (questionAnswers["seedlanguages"].EqualToAny("y", "yes")) SeedMedievalEuropeLanguages();

		if (questionAnswers["seedheritage"].EqualToAny("y", "yes")) SeedMedievalHeritage();
	}

	public void SeedRepublicanRome(IReadOnlyDictionary<string, string> questionAnswers)
	{
		if (questionAnswers["seednames"].EqualToAny("y", "yes")) SeedLatinNames(_context);

		if (questionAnswers["seedlanguages"].EqualToAny("y", "yes")) SeedRomanLanguages();

		if (questionAnswers["seedheritage"].EqualToAny("y", "yes")) SeedRomanHeritage();
	}

	public void SeedMiddleEarth(IReadOnlyDictionary<string, string> questionAnswers)
	{
		if (questionAnswers["seednames"].EqualToAny("y", "yes")) SeedMiddleEarthNames();

		if (questionAnswers["seedheritage"].EqualToAny("y", "yes")) SeedMiddleEarthHeritage();

		if (questionAnswers["seedlanguages"].EqualToAny("y", "yes")) SeedMiddleEarthLanguages();
	}
}
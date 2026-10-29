#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

/// <summary>
/// Gives stock racial person words value-level gates, including when an All profile exposes them.
/// </summary>
internal static class NonHumanPersonWordSeederHelper
{
	internal static bool HasMissing(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, IReadOnlyList<string>> raceWords)
	{
		var definition = context.CharacteristicDefinitions.FirstOrDefault(x => x.Name == "Person Word");
		if (definition is null)
		{
			return false;
		}

		return raceWords.Values.SelectMany(x => x).Distinct(StringComparer.OrdinalIgnoreCase)
			.Any(word => FindValue(context, definition, word) is not { } value ||
			             IsLegacyStock(context, definition, value, raceWords));
	}

	internal static void Ensure(FuturemudDatabaseContext context, CharacteristicDefinition definition,
		string package, IReadOnlyDictionary<string, IReadOnlyList<string>> raceWords)
	{
		foreach (var group in raceWords.SelectMany(x => x.Value.Select(word => (Race: x.Key, Word: word)))
			         .GroupBy(x => x.Word, StringComparer.OrdinalIgnoreCase))
		{
			var value = FindValue(context, definition, group.Key);
			if (value is not null && !IsLegacyStock(context, definition, value, raceWords))
			{
				continue;
			}

			var functionName = $"CanSelect{package}PersonWord{group.Key.CollapseString()}";
			var prog = context.FutureProgs.FirstOrDefault(x => x.FunctionName == functionName);
			if (prog is null)
			{
				prog = new FutureProg
				{
					FunctionName = functionName,
					Category = "Character",
					Subcategory = "Person Words",
					FunctionComment = $"Allows {group.Key} for its stock races and their descendants.",
					FunctionText = "return " + string.Join(" or ", group.Select(x => x.Race)
						.Distinct(StringComparer.OrdinalIgnoreCase)
						.Select(race => $"SameRace(@ch.Race, ToRace(\"{race}\"))")),
					ReturnType = (long)ProgVariableTypes.Boolean,
					StaticType = (int)FutureProgStaticType.NotStatic,
					AcceptsAnyParameters = false,
					Public = true
				};
				prog.FutureProgsParameters.Add(new FutureProgsParameter
				{
					ParameterIndex = 0,
					ParameterName = "ch",
					ParameterType = (long)ProgVariableTypes.Toon
				});
				context.FutureProgs.Add(prog);
			}

			if (value is null)
			{
				value = new CharacteristicValue
				{
					Name = group.Key,
					Value = group.Key,
					Definition = definition,
					AdditionalValue = string.Empty,
					Default = false,
					Pluralisation = 0
				};
				context.CharacteristicValues.Add(value);
			}

			value.FutureProg = prog;
			context.SaveChanges();
		}
	}

	private static CharacteristicValue? FindValue(FuturemudDatabaseContext context,
		CharacteristicDefinition definition, string word)
	{
		return context.CharacteristicValues.SingleOrDefault(x => x.DefinitionId == definition.Id && x.Name == word);
	}

	private static bool IsLegacyStock(FuturemudDatabaseContext context, CharacteristicDefinition definition,
		CharacteristicValue value, IReadOnlyDictionary<string, IReadOnlyList<string>> raceWords)
	{
		if (value.FutureProgId is not null || value.OngoingValidityProgId is not null ||
		    value.Value != value.Name || value.Default || value.Pluralisation != 0 ||
		    !string.IsNullOrEmpty(value.AdditionalValue))
		{
			return false;
		}

		// A name alone does not establish ownership. Recognise the old seeder's complete profile
		// shape as well, and leave altered values, profiles and existing applicability progs alone.
		foreach (var (race, words) in raceWords.Where(x => x.Value.Contains(value.Name)))
		{
			var profileName = $"{race} Person Words";
			var profile = context.CharacteristicProfiles.FirstOrDefault(x => x.Name == profileName);
			if (profile?.TargetDefinitionId != definition.Id || profile.Type != "Standard" ||
			    profile.Description != $"Auto-generated profile for {profileName}")
			{
				continue;
			}

			var expected = new XElement("Values", words.Distinct(StringComparer.OrdinalIgnoreCase)
				.Select(word => new XElement("Value", word)));
			try
			{
				if (XNode.DeepEquals(XElement.Parse(profile.Definition), expected))
				{
					return true;
				}
			}
			catch (System.Xml.XmlException)
			{
				// Builder-edited or malformed XML is not the legacy stock shape.
			}
		}

		return false;
	}
}

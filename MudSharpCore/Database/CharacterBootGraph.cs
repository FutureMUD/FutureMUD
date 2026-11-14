#nullable enable

using Microsoft.EntityFrameworkCore;

namespace MudSharp.Database;

/// <summary>Loads only selected boot actors, with empty navigations marked loaded as well as populated ones.</summary>
internal static class CharacterBootGraph
{
	internal const int BatchSize = 1024;

	internal static Dictionary<long, Models.Character> Load(FuturemudDatabaseContext context,
		IEnumerable<long> characterIds)
	{
		var result = new Dictionary<long, Models.Character>();
		foreach (var ids in characterIds.Distinct().Chunk(BatchSize))
		{
			foreach (var character in Query(context.Characters.Where(x => ids.Contains(x.Id) && !x.IsArchived)).ToList())
			{
				result.Add(character.Id, character);
			}
		}

		return result;
	}

	internal static IQueryable<Models.Character> Query(IQueryable<Models.Character> query)
	{
		query = query
			.Include(x => x.Account)
			.Include(x => x.Guest)
			.Include(x => x.NpcsCharacter).ThenInclude(x => x.NpcsArtificialIntelligences)
			.Include(x => x.Dubs)
			.Include(x => x.PerceiverMerits)
			.Include(x => x.CharacterBodies)
			.Include(x => x.CharacterBodySources)
			.Include(x => x.CharacterInstances)
			.Include(x => x.CharacterTraits)
			.Include(x => x.CharactersChargenRoles)
			.Include(x => x.CharacterKnowledges)
			.Include(x => x.CharactersLanguages)
			.Include(x => x.CharactersSignedLanguages)
			.Include(x => x.CharactersSignedLanguageVarieties)
			.Include(x => x.CharactersAccents)
			.Include(x => x.CharactersScripts)
			.Include(x => x.AlliesCharacter)
			.Include(x => x.CharactersMagicResources)
			.Include(x => x.ActiveProjects)
			.Include(x => x.ProjectLabourQueues)
			.Include(x => x.ActiveJobs)
			.Include(x => x.HooksPerceivables);

		// A form/source can point at another body. EF identity resolution merges shared bodies,
		// while split queries avoid multiplying all of their independent child collections.
		foreach (var body in new[] { "Body", "CharacterBodies.Body", "CharacterBodySources.Body" })
		{
			foreach (var navigation in new[]
			         {
				         "Characteristics", "PerceiverMerits", "Traits.TraitDefinition", "Wounds.Infections",
				         "BodiesSeveredParts", "BodiesProsthetics.Prosthetic", "BodiesImplants.Implant",
				         "Infections", "BodiesDrugDoses", "BodiesDrugExposures", "BodiesGameItems"
			         })
			{
				query = query.Include($"{body}.{navigation}");
			}
		}

		return query.AsSplitQuery().AsTracking();
	}
}

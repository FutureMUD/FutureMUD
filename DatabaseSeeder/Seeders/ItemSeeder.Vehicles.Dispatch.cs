#nullable enable

using MudSharp.Database;
using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	/// <summary>
	/// Runs the vehicle subcomponent after the established item pass has populated the shared
	/// component, material, liquid, trait and item dictionaries it consumes. The database seeder
	/// host invokes ItemSeeder through IDatabaseSeeder, so this explicit implementation preserves
	/// the existing public seeding pipeline while giving the vehicle graph a deterministic final
	/// phase without coupling it to any one era-specific catalogue method.
	/// </summary>
	string IDatabaseSeeder.SeedData(
		FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> questionAnswers)
	{
		var result = SeedData(context, questionAnswers);
		if (questionAnswers.TryGetValue("eras", out var eras))
		{
			SeedVehicleItemsAndPrototypes(eras);
			context.SaveChanges();
		}

		return result;
	}
}

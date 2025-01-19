using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Database;

namespace DatabaseSeeder.Seeders;
internal class FantasySeeder : IDatabaseSeeder
{
	#region Implementation of IDatabaseSeeder

	/// <inheritdoc />
	public IEnumerable<(string Id, string Question, Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter, Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions { get; }

	/// <inheritdoc />
	public int SortOrder => 301;

	/// <inheritdoc />
	public string Name => "Fantasy Seeder";

	/// <inheritdoc />
	public string Tagline => "Install Fantasy Races";

	/// <inheritdoc />
	public string FullDescription => @"";

	/// <inheritdoc />
	public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc />
	public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc />
	public bool Enabled => false;

	#endregion
}

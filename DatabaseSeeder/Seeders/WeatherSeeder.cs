using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Database;

namespace DatabaseSeeder.Seeders
{
	public class WeatherSeeder: IDatabaseSeeder
	{
		#region Implementation of IDatabaseSeeder

		/// <inheritdoc />
		public IEnumerable<(string Id, string Question, Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter, Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions
		=> new List<(string Id, string Question, Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool>
			Filter, Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)>
		{

		};

		/// <inheritdoc />
		public int SortOrder => 7;

		/// <inheritdoc />
		public string Name => "Weather Seeder";

		/// <inheritdoc />
		public string Tagline => "Sets up Weather and Seasons";

		/// <inheritdoc />
		public string FullDescription => @"This seeder will set up the core components of the weather and climate system in your MUD, including seasons. Note that this assumes terrestrial or earth-like weather. 

Once you have installed this seeder you will need to add the WeatherControllers it installs onto your zones yourself.";

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

		#endregion
	}
}

using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
	public partial class ClimateModel
	{
		public ClimateModel()
		{
			ClimateModelSeasons = new HashSet<ClimateModelSeason>();
		}

		public long Id { get; set; }
		public string Name { get; set; }
		public string Type { get; set; }
		public int MinuteProcessingInterval { get; set; }
		public int MinimumMinutesBetweenFlavourEchoes { get; set; }
		public double MinuteFlavourEchoChance { get; set; }

		public virtual ICollection<ClimateModelSeason> ClimateModelSeasons { get; set; }
	}

	public partial class ClimateModelSeason
	{
		public ClimateModelSeason()
		{
			SeasonEvents = new HashSet<ClimateModelSeasonEvent>();
		}

		public long ClimateModelId { get; set; }
		public long SeasonId { get; set; }
		public double MaximumAdditionalChangeChanceFromStableWeather { get; set; }
		public double IncrementalAdditionalChangeChanceFromStableWeather { get; set; }

		public virtual ClimateModel ClimateModel { get; set; }
		public virtual Season Season { get; set; }
		public virtual ICollection<ClimateModelSeasonEvent> SeasonEvents { get; set; }
	}

	public partial class ClimateModelSeasonEvent
	{
		public long ClimateModelId { get; set; }
		public long SeasonId { get; set; }
		public long WeatherEventId { get; set; }
		public double ChangeChance { get; set; }
		public string Transitions { get; set; }

		public virtual ClimateModel ClimateModel { get; set; }
		public virtual Season Season { get; set; }
		public virtual WeatherEvent WeatherEvent { get; set; }
		public virtual ClimateModelSeason ClimateModelSeason { get; set; }
	}
}

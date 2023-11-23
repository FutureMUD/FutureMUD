namespace MudSharp.Climate
{
	public static class WeatherExtensions
	{
		public static string Describe(this PrecipitationLevel level)
		{
			switch (level)
			{
				case PrecipitationLevel.Parched:
					return "Parched";
				case PrecipitationLevel.Dry:
					return "Dry";
				case PrecipitationLevel.Humid:
					return "Humid";
				case PrecipitationLevel.LightRain:
					return "Light Rain";
				case PrecipitationLevel.Rain:
					return "Rain";
				case PrecipitationLevel.HeavyRain:
					return "Heavy Rain";
				case PrecipitationLevel.TorrentialRain:
					return "Torrential Rain";
				case PrecipitationLevel.LightSnow:
					return "Light Snow";
				case PrecipitationLevel.Snow:
					return "Snow";
				case PrecipitationLevel.HeavySnow:
					return "Heavy Snow";
				case PrecipitationLevel.Blizzard:
					return "Blizzard";
				case PrecipitationLevel.Sleet:
					return "Sleet";
			}

			return "Unknown";
		}

		public static string Describe(this WindLevel wind)
		{
			switch (wind)
			{
				case WindLevel.None:
					return "None";
				case WindLevel.Still:
					return "Still";
				case WindLevel.OccasionalBreeze:
					return "Occasional Breeze";
				case WindLevel.Breeze:
					return "Breeze";
				case WindLevel.Wind:
					return "Wind";
				case WindLevel.StrongWind:
					return "Strong Wind";
				case WindLevel.GaleWind:
					return "Gale Wind";
				case WindLevel.HurricaneWind:
					return "Hurricane Wind";
				case WindLevel.MaelstromWind:
					return "Maelstrom Wind";
			}

			return "Unknown";
		}

		public static bool IsSnowing(this PrecipitationLevel level)
		{
			switch (level)
			{
				case PrecipitationLevel.LightSnow:
				case PrecipitationLevel.Snow:
				case PrecipitationLevel.HeavySnow:
				case PrecipitationLevel.Blizzard:
				case PrecipitationLevel.Sleet:
					return true;
			}

			return false;
		}

		public static bool IsRaining(this PrecipitationLevel level)
		{
			switch (level)
			{
				case PrecipitationLevel.LightRain:
				case PrecipitationLevel.Rain:
				case PrecipitationLevel.HeavyRain:
				case PrecipitationLevel.TorrentialRain:
				case PrecipitationLevel.Sleet:
					return true;
			}

			return false;
		}
	}
	}

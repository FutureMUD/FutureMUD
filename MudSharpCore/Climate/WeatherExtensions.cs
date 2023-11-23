using MudSharp.Framework;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Climate;

public static class WeatherExtensions
{
	public static double PrecipitationClimbingBonus(this PrecipitationLevel level)
	{
		switch (level)
		{
			case PrecipitationLevel.Parched:
				return Futuremud.Games.First().GetStaticDouble("PrecipitationClimbingBonusParched");
			case PrecipitationLevel.Dry:
				return Futuremud.Games.First().GetStaticDouble("PrecipitationClimbingBonusDry");
			case PrecipitationLevel.Humid:
				return Futuremud.Games.First().GetStaticDouble("PrecipitationClimbingBonusHumid");
			case PrecipitationLevel.LightRain:
				return Futuremud.Games.First().GetStaticDouble("PrecipitationClimbingBonusLightRain");
			case PrecipitationLevel.Rain:
				return Futuremud.Games.First().GetStaticDouble("PrecipitationClimbingBonusRain");
			case PrecipitationLevel.HeavyRain:
				return Futuremud.Games.First().GetStaticDouble("PrecipitationClimbingBonusHeavyRain");
			case PrecipitationLevel.TorrentialRain:
				return Futuremud.Games.First().GetStaticDouble("PrecipitationClimbingBonusTorrentialRain");
			case PrecipitationLevel.LightSnow:
				return Futuremud.Games.First().GetStaticDouble("PrecipitationClimbingBonusLightSnow");
			case PrecipitationLevel.Snow:
				return Futuremud.Games.First().GetStaticDouble("PrecipitationClimbingBonusSnow");
			case PrecipitationLevel.HeavySnow:
				return Futuremud.Games.First().GetStaticDouble("PrecipitationClimbingBonusHeavySnow");
			case PrecipitationLevel.Blizzard:
				return Futuremud.Games.First().GetStaticDouble("PrecipitationClimbingBonusBlizzard");
			case PrecipitationLevel.Sleet:
				return Futuremud.Games.First().GetStaticDouble("PrecipitationClimbingBonusSleet");
		}

		return 0.0;
	}

	public static double WindClimbingBonus(this WindLevel level)
	{
		switch (level)
		{
			case WindLevel.None:
				return Futuremud.Games.First().GetStaticDouble("WindClimbingBonusNone");
			case WindLevel.Still:
				return Futuremud.Games.First().GetStaticDouble("WindClimbingBonusStill");
			case WindLevel.OccasionalBreeze:
				return Futuremud.Games.First().GetStaticDouble("WindClimbingBonusOccasionalBreeze");
			case WindLevel.Breeze:
				return Futuremud.Games.First().GetStaticDouble("WindClimbingBonusBreeze");
			case WindLevel.Wind:
				return Futuremud.Games.First().GetStaticDouble("WindClimbingBonusWind");
			case WindLevel.StrongWind:
				return Futuremud.Games.First().GetStaticDouble("WindClimbingBonusStrongWind");
			case WindLevel.GaleWind:
				return Futuremud.Games.First().GetStaticDouble("WindClimbingBonusGaleWind");
			case WindLevel.HurricaneWind:
				return Futuremud.Games.First().GetStaticDouble("WindClimbingBonusHurricaneWind");
			case WindLevel.MaelstromWind:
				return Futuremud.Games.First().GetStaticDouble("WindClimbingBonusMaelstromWind");
		}

		return 0.0;
	}

	public static Difficulty MinimumSightDifficulty(this PrecipitationLevel level)
	{
		switch (level)
		{
			case PrecipitationLevel.Parched:
				return (Difficulty)Futuremud.Games.First().GetStaticInt("MinimumSightDifficultyParched");
			case PrecipitationLevel.Dry:
				return (Difficulty)Futuremud.Games.First().GetStaticInt("MinimumSightDifficultyDry");
			case PrecipitationLevel.Humid:
				return (Difficulty)Futuremud.Games.First().GetStaticInt("MinimumSightDifficultyHumid");
			case PrecipitationLevel.LightRain:
				return (Difficulty)Futuremud.Games.First().GetStaticInt("MinimumSightDifficultyLightRain");
			case PrecipitationLevel.Rain:
				return (Difficulty)Futuremud.Games.First().GetStaticInt("MinimumSightDifficultyRain");
			case PrecipitationLevel.HeavyRain:
				return (Difficulty)Futuremud.Games.First().GetStaticInt("MinimumSightDifficultyHeavyRain");
			case PrecipitationLevel.TorrentialRain:
				return (Difficulty)Futuremud.Games.First().GetStaticInt("MinimumSightDifficultyTorrentialRain");
			case PrecipitationLevel.LightSnow:
				return (Difficulty)Futuremud.Games.First().GetStaticInt("MinimumSightDifficultyLightSnow");
			case PrecipitationLevel.Snow:
				return (Difficulty)Futuremud.Games.First().GetStaticInt("MinimumSightDifficultySnow");
			case PrecipitationLevel.HeavySnow:
				return (Difficulty)Futuremud.Games.First().GetStaticInt("MinimumSightDifficultyHeavySnow");
			case PrecipitationLevel.Blizzard:
				return (Difficulty)Futuremud.Games.First().GetStaticInt("MinimumSightDifficultyBlizzard");
			case PrecipitationLevel.Sleet:
				return (Difficulty)Futuremud.Games.First().GetStaticInt("MinimumSightDifficultySleet");
		}

		return Difficulty.Automatic;
	}

	public static Difficulty MinimumHearingDifficulty(this WindLevel level)
	{
		switch (level)
		{
			case WindLevel.Still:
				return (Difficulty)Futuremud.Games.First().GetStaticInt("MinimumHearingDifficultyStill");
			case WindLevel.OccasionalBreeze:
				return (Difficulty)Futuremud.Games.First().GetStaticInt("MinimumHearingDifficultyOccasionalBreeze");
			case WindLevel.Breeze:
				return (Difficulty)Futuremud.Games.First().GetStaticInt("MinimumHearingDifficultyBreeze");
			case WindLevel.Wind:
				return (Difficulty)Futuremud.Games.First().GetStaticInt("MinimumHearingDifficultyWind");
			case WindLevel.StrongWind:
				return (Difficulty)Futuremud.Games.First().GetStaticInt("MinimumHearingDifficultyStrongWind");
			case WindLevel.GaleWind:
				return (Difficulty)Futuremud.Games.First().GetStaticInt("MinimumHearingDifficultyGaleWind");
			case WindLevel.HurricaneWind:
				return (Difficulty)Futuremud.Games.First().GetStaticInt("MinimumHearingDifficultyHurricaneWind");
			case WindLevel.MaelstromWind:
				return (Difficulty)Futuremud.Games.First().GetStaticInt("MinimumHearingDifficultyMaelstromWind");
		}

		return Difficulty.Automatic;
	}
}
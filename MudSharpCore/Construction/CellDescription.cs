using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Climate;
using MudSharp.Economy;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Construction;

public partial class Cell
{
	#region Description Methods

	/// <summary>
	/// Form is environment{conditions,text}{optional more conds,text}{fallback}
	/// </summary>
	private readonly Regex _weatherAndLightRegex = new(
		@"environment\{(?<qualifiers1>[a-zA-Z0-9\*><, ]+)=(?<text1>[^}]*)\}(?:\{(?<qualifiers2>[a-zA-Z0-9\*><, ]+)=(?<text2>[^}]*)\}){0,1}(?:\{(?<qualifiers3>[a-zA-Z0-9\*><, ]+)=(?<text3>[^}]*)\}){0,1}(?:\{(?<qualifiers4>[a-zA-Z0-9\*><, ]+)=(?<text4>[^}]*)\}){0,1}(?:\{(?<qualifiers5>[a-zA-Z0-9\*><, ]+)=(?<text5>[^}]*)\}){0,1}(?:\{(?<qualifiers6>[a-zA-Z0-9\*><, ]+)=(?<text6>[^}]*)\}){0,1}(?:\{(?<qualifiers7>[a-zA-Z0-9\*><, ]+)=(?<text7>[^}]*)\}){0,1}(?:\{(?<qualifiers8>[a-zA-Z0-9\*><, ]+)=(?<text8>[^}]*)\}){0,1}(?:\{(?<fallback>[^}=]*)\}){0,1}");

	private PrecipitationLevel? PrecipitationFromString(string text)
	{
		switch (text.ToLowerInvariant())
		{
			case "parched":
				return PrecipitationLevel.Parched;
			case "dry":
				return PrecipitationLevel.Dry;
			case "humid":
				return PrecipitationLevel.Humid;
			case "lrain":
			case "lightrain":
				return PrecipitationLevel.LightRain;
			case "rain":
				return PrecipitationLevel.Rain;
			case "hrain":
			case "heavyrain":
				return PrecipitationLevel.HeavyRain;
			case "train":
			case "torrential":
			case "torrent":
			case "torrentialrain":
				return PrecipitationLevel.TorrentialRain;
			case "lsnow":
			case "lightsnow":
				return PrecipitationLevel.LightSnow;
			case "snow":
				return PrecipitationLevel.Snow;
			case "hsnow":
			case "heavysnow":
				return PrecipitationLevel.HeavySnow;
			case "blizzard":
				return PrecipitationLevel.Blizzard;
			case "sleet":
				return PrecipitationLevel.Sleet;
		}

		return null;
	}

	private string SubstituteWeatherAndLightText(string baseDescription, IPerceiver voyeur)
	{
		bool QualifiersApply(string qualifiers)
		{
			if (string.IsNullOrEmpty(qualifiers))
			{
				return false;
			}

			bool EvaluteQualifier(string qualifier)
			{
				if (string.IsNullOrWhiteSpace(qualifier))
				{
					return true;
				}

				var text = qualifier.ToLowerInvariant();

				if (text[0] == '!')
				{
					return !EvaluteQualifier(text.Substring(1));
				}

				if (text[0] == '*')
				{
					if (HighestRecentPrecipitationLevel(voyeur) >=
					    (PrecipitationFromString(text.Substring(1)) ?? PrecipitationLevel.Parched))
					{
						return true;
					}

					return false;
				}

				switch (text)
				{
					case "day":
						if (CurrentTimeOfDay.In(TimeOfDay.Morning, TimeOfDay.Afternoon))
						{
							return true;
						}

						return false;
					case "night":
						if (CurrentTimeOfDay == TimeOfDay.Night)
						{
							return true;
						}

						return false;
					case "morning":
						if (CurrentTimeOfDay == TimeOfDay.Morning)
						{
							return true;
						}

						return false;
					case "afternoon":
						if (CurrentTimeOfDay == TimeOfDay.Afternoon)
						{
							return true;
						}

						return false;
					case "dusk":
						if (CurrentTimeOfDay == TimeOfDay.Dusk)
						{
							return true;
						}

						return false;
					case "dawn":
						if (CurrentTimeOfDay == TimeOfDay.Dawn)
						{
							return true;
						}

						return false;
					case "notnight":
						if (CurrentTimeOfDay != TimeOfDay.Night)
						{
							return true;
						}

						return false;
				}

				bool greaterThan = false, lessThan = false;
				if (text[0] == '>')
				{
					greaterThan = true;
					text = text.Substring(1);
				}
				else if (text[0] == '<')
				{
					lessThan = true;
					text = text.Substring(1);
				}

				var season = Zone.WeatherController?.RegionalClimate.Seasons.FirstOrDefault(x => x.Name.EqualTo(text));
				if (season != null)
				{
					return Zone.WeatherController.CurrentSeason == season;
				}

				var precip = PrecipitationFromString(text);
				if (precip != null)
				{
					var weather = CurrentWeather(voyeur);
					if (weather == null)
					{
						return false;
					}

					if (greaterThan)
					{
						if (weather.Precipitation >= precip.Value)
						{
							return true;
						}

						return false;
					}

					if (lessThan)
					{
						if (weather.Precipitation < precip.Value)
						{
							return true;
						}

						return false;
					}

					if (weather.Precipitation == precip.Value)
					{
						return true;
					}

					return false;
				}

				var lightLevel = Gameworld.LightModel.GetMinimumIlluminationForDescription(text);
				if (greaterThan)
				{
					if (CurrentIllumination(voyeur) >= lightLevel)
					{
						return true;
					}

					return false;
				}

				if (lessThan)
				{
					if (CurrentIllumination(voyeur) < lightLevel)
					{
						return true;
					}

					return false;
				}

				if (Gameworld.LightModel.GetIlluminationDescription(CurrentIllumination(voyeur)).EqualTo(text))
				{
					return true;
				}

				return false;
			}

			foreach (var qualifier in qualifiers.Split(','))
			{
				if (!EvaluteQualifier(qualifier))
				{
					return false;
				}
			}

			return true;
		}

		return _weatherAndLightRegex.Replace(baseDescription, match =>
		{
			if (QualifiersApply(match.Groups["qualifiers1"].Value))
			{
				return match.Groups["text1"].Value;
			}

			if (QualifiersApply(match.Groups["qualifiers2"].Value))
			{
				return match.Groups["text2"].Value;
			}

			if (QualifiersApply(match.Groups["qualifiers3"].Value))
			{
				return match.Groups["text3"].Value;
			}

			if (QualifiersApply(match.Groups["qualifiers4"].Value))
			{
				return match.Groups["text4"].Value;
			}

			if (QualifiersApply(match.Groups["qualifiers5"].Value))
			{
				return match.Groups["text5"].Value;
			}

			if (QualifiersApply(match.Groups["qualifiers6"].Value))
			{
				return match.Groups["text6"].Value;
			}

			if (QualifiersApply(match.Groups["qualifiers7"].Value))
			{
				return match.Groups["text7"].Value;
			}

			if (QualifiersApply(match.Groups["qualifiers8"].Value))
			{
				return match.Groups["text8"].Value;
			}

			if (match.Groups["fallback"].Length > 0)
			{
				return match.Groups["fallback"].Value;
			}

			return string.Empty;
		});
	}

#nullable enable
	public override string HowSeen(IPerceiver? voyeur, bool proper = false, DescriptionType type = DescriptionType.Short,
		bool colour = true, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
	{
		var sb = new StringBuilder();
		var overlay = GetOverlayFor(voyeur);
		switch (type)
		{
			case DescriptionType.Short:
				var output = flags.HasFlag(PerceiveIgnoreFlags.IgnoreCanSee) || (voyeur?.CanSee(this) ?? true)
					? overlay.CellName
					: "Somewhere";
				output = overlay.Terrain.RoomNameForLayer(output,
					flags.HasFlag(PerceiveIgnoreFlags.IgnoreLayers) ? RoomLayer.GroundLevel : voyeur?.RoomLayer ?? RoomLayer.GroundLevel);
				output = SubstituteDescriptionVariables(output, voyeur, flags);
				return colour ? output.ColourRoom() : output;
			case DescriptionType.Long:
				var character = voyeur as ICharacter;
				if (character?.IsAdministrator() == true)
				{
					sb
						.AppendLine(HowSeen(voyeur, proper, DescriptionType.Short, colour, flags))
						.AppendLine(AdminInfoStrings(character))
						.Append(ExitStrings(character, overlay, colour));

					return sb.ToString();
				}

				if (!flags.HasFlag(PerceiveIgnoreFlags.IgnoreCanSee) && voyeur?.CanSee(this) == false)
				{
					return "You cannot see your surroundings.";
				}

				sb
					.AppendLine(HowSeen(voyeur, proper, DescriptionType.Short, colour, flags))
					.AppendLine()
					.Append(ExitStrings(character, overlay, colour));

				return sb.ToString();
			case DescriptionType.Full:
				return CellFullDescription(voyeur, proper, colour, flags, overlay);

			default:
				return HowSeen(voyeur, proper);
		}
	}
#nullable restore
	private string SubstituteDescriptionVariables(string description, IPerceiver voyeur, PerceiveIgnoreFlags flags)
	{
		description = SubstituteWeatherAndLightText(description, voyeur);
		description = description.SubstituteANSIColour();
		description = description.Replace("@shop", Shop?.Name ?? "An Empty Shop",
			StringComparison.InvariantCultureIgnoreCase);
		description = description.SubstituteCheckTrait(voyeur, Gameworld);
		description = description.SubstituteWrittenLanguage(voyeur, Gameworld);
		return description;
	}

	private string CellFullDescription(IPerceiver voyeur, bool proper, bool colour, PerceiveIgnoreFlags flags,
		ICellOverlay overlay)
	{
		if (!flags.HasFlag(PerceiveIgnoreFlags.IgnoreCanSee) && voyeur?.CanSee(this) == false)
		{
			return "You cannot see your surroundings.";
		}

		var sb = new StringBuilder();
		var character = voyeur as ICharacter;
		var descSubSB = new StringBuilder();
		var weather = CurrentWeather(voyeur);

		sb.AppendLine(HowSeen(voyeur, proper, DescriptionType.Short, colour, flags));
		if (character?.IsAdministrator() == true)
		{
			sb.AppendLine(AdminInfoStrings(character));
		}
		else
		{
			sb.AppendLine();
		}

		sb.AppendLine(ExitStrings(character, overlay, colour));
		if (character?.Account.TabRoomDescriptions == true)
		{
			descSubSB.Append("\t");
		}

		var addedAdditionalLines = false;
		// Here is the room description itself (comment added to help see through the clutter)
		descSubSB.Append(ProcessedFullDescription(voyeur, flags, overlay));
		if (character?.Account.CodedRoomDescriptionAdditionsOnNewLine == true)
		{
			descSubSB.AppendLine();
			descSubSB.AppendLine();
			if (weather != null &&
			    overlay.OutdoorsType.In(CellOutdoorsType.Outdoors, CellOutdoorsType.IndoorsClimateExposed) &&
			    !IsUnderwaterLayer(voyeur?.RoomLayer ?? RoomLayer.GroundLevel) &&
			    !string.IsNullOrWhiteSpace(weather.WeatherRoomAddendum))
			{
				addedAdditionalLines = true;
				descSubSB.AppendLine(
					$"{(character?.Account.TabRoomDescriptions == true ? "\t" : "")}{weather.WeatherRoomAddendum.SubstituteANSIColour()}");
			}

			foreach (var effect in Zone.EffectsOfType<IDescriptionAdditionEffect>(x => x.DescriptionAdditionApplies(voyeur)))
			{
				var text = effect.GetAdditionalText(voyeur, colour);
				if (string.IsNullOrWhiteSpace(text))
				{
					continue;
				}

				addedAdditionalLines = true;
				descSubSB.AppendLine($"{(character?.Account.TabRoomDescriptions == true ? "\t" : "")}{text}");
			}

			foreach (var effect in EffectsOfType<IDescriptionAdditionEffect>(x => x.DescriptionAdditionApplies(voyeur)))
			{
				var text = effect.GetAdditionalText(voyeur, colour);
				if (string.IsNullOrWhiteSpace(text))
				{
					continue;
				}

				addedAdditionalLines = true;
				descSubSB.AppendLine($"{(character?.Account.TabRoomDescriptions == true ? "\t" : "")}{text}");
			}

			addedAdditionalLines = true;
			descSubSB.AppendLine(
				$"{(character?.Account.TabRoomDescriptions == true ? "\t" : "")}{Gameworld.LightModel.GetIlluminationRoomDescription(CurrentIllumination(character))}");
		}
		else
		{
			descSubSB.Append(" ");
			descSubSB.Append(Gameworld.LightModel.GetIlluminationRoomDescription(CurrentIllumination(character)));
			if (weather != null &&
			    overlay.OutdoorsType.In(CellOutdoorsType.Outdoors, CellOutdoorsType.IndoorsClimateExposed) &&
			    !IsUnderwaterLayer(voyeur?.RoomLayer ?? RoomLayer.GroundLevel) &&
			    !string.IsNullOrWhiteSpace(weather.WeatherRoomAddendum))
			{
				descSubSB.Append(" ");
				descSubSB.Append(weather.WeatherRoomAddendum.SubstituteANSIColour());
			}

			foreach (var effect in Zone.EffectsOfType<IDescriptionAdditionEffect>(x => x.DescriptionAdditionApplies(voyeur)))
			{
				descSubSB.Append(" ");
				descSubSB.AppendLine(effect.GetAdditionalText(voyeur, colour));
			}

			foreach (var effect in EffectsOfType<IDescriptionAdditionEffect>(x => x.DescriptionAdditionApplies(voyeur)))
			{
				descSubSB.Append(" ");
				descSubSB.AppendLine(effect.GetAdditionalText(voyeur, colour));
			}
		}

		IShop shop = null;
		if (Shop is not null)
		{
			shop = Shop;
		}
		else
		{
			var stall = 
			GameItems.
			SelectNotNull(x => x.GetItemType<IShopStall>()).
			FirstOrDefault(x => x.Shop is not null);
			shop = stall?.Shop;
		}
		if (shop != null && Gameworld.GetStaticBool("ShowShopInRoomDescription"))
		{
			if (!addedAdditionalLines)
			{
				addedAdditionalLines = true;
				descSubSB.AppendLine();
			}

			if (character?.Account.TabRoomDescriptions == true)
			{
				descSubSB.Append("\t");
			}

			descSubSB.AppendLine(
				$"A shop is here. You can use the command LIST to view items for sale.".Colour(Telnet.Yellow));
		}

		var bank = Gameworld.Banks.FirstOrDefault(x => x.BranchLocations.Contains(this));
		if (bank != null && Gameworld.GetStaticBool("ShowShopInRoomDescription"))
		{
			if (!addedAdditionalLines)
			{
				addedAdditionalLines = true;
				descSubSB.AppendLine();
			}

			if (character?.Account.TabRoomDescriptions == true)
			{
				descSubSB.Append("\t");
			}

			descSubSB.AppendLine($"This is a branch of {bank.Name.ColourName()}. Use the command BANK here."
				.ColourIncludingReset(Telnet.Yellow));
		}

		var auctionHouse = Gameworld.AuctionHouses.FirstOrDefault(x => x.AuctionHouseCell == this);
		if (auctionHouse != null && Gameworld.GetStaticBool("ShowShopInRoomDescription"))
		{
			if (!addedAdditionalLines)
			{
				addedAdditionalLines = true;
				descSubSB.AppendLine();
			}

			if (character?.Account.TabRoomDescriptions == true)
			{
				descSubSB.Append("\t");
			}

			descSubSB.AppendLine($"This is an auction house. Use the commands AUCTION and AUCTIONS here."
				.ColourIncludingReset(Telnet.Yellow));
		}

		if (Gameworld.EconomicZones.Any(x => x.ConveyancingCells.Contains(this)) &&
		    Gameworld.GetStaticBool("ShowShopInRoomDescription"))
		{
			if (!addedAdditionalLines)
			{
				addedAdditionalLines = true;
				descSubSB.AppendLine();
			}

			if (character?.Account.TabRoomDescriptions == true)
			{
				descSubSB.Append("\t");
			}

			descSubSB.AppendLine(
				$"You can use the PROPERTY and PROPERTIES commands here.".ColourIncludingReset(Telnet.Yellow));
		}

		if (Gameworld.EconomicZones.Any(x => x.JobFindingCells.Contains(this)) &&
		    Gameworld.GetStaticBool("ShowShopInRoomDescription"))
		{
			if (!addedAdditionalLines)
			{
				addedAdditionalLines = true;
				descSubSB.AppendLine();
			}

			if (character?.Account.TabRoomDescriptions == true)
			{
				descSubSB.Append("\t");
			}

			descSubSB.AppendLine($"You can use the JOBS and JOB commands here.".ColourIncludingReset(Telnet.Yellow));
		}

		if (Characters.Any(x => x.AffectedBy<OnTrial>()))
		{
			descSubSB.AppendLine("There is a trial taking place here. You can use the TRIAL command to see details.".ColourIncludingReset(Telnet.BoldOrange));
		}

		sb.Append(descSubSB.ToString().Wrap(character.Account.InnerLineFormatLength));
		return sb.ToString().Wrap(character?.Account.LineFormatLength ?? 120);
	}

	public string ProcessedFullDescription(IPerceiver voyeur, PerceiveIgnoreFlags flags, ICellOverlay overlay)
	{
		return SubstituteDescriptionVariables(overlay.CellDescription, voyeur, flags);
	}

	private string AdminInfoStrings(IPerceiver perceiver)
	{
		var overlay = GetOverlayFor(perceiver);
		var terrain = Terrain(perceiver);
		var terrainString = overlay.Terrain == terrain ? $"{overlay.Terrain.Name.TitleCase().Colour(Telnet.Green)}" : $"{terrain.Name.TitleCase().Colour(Telnet.Magenta)}";
		return
			$"ID[{Id.ToString("N0", perceiver).Colour(Telnet.Green)}] #3|#0 Terrain[{terrainString}] #3|#0 {(overlay.SafeQuit ? "[SafeQuit]".Colour(Telnet.Green) : "[NoQuit]".Colour(Telnet.Yellow))}{(EffectsOfType<IPeacefulEffect>().Any() ? " [Peaceful]".Colour(Telnet.BoldCyan) : "")} #3|#0 Overlay[{overlay.Package.Name.TitleCase().Colour(overlay == CurrentOverlay ? Telnet.Green : Telnet.Red)} #2(##{overlay.Package.Id.ToString("N0", perceiver)}r{overlay.Package.RevisionNumber.ToString("N0", perceiver)})#0] #3|#0 Coordinates[#2{X?.ToString("F0", perceiver) ?? "x"},{Y?.ToString("F0", perceiver) ?? "y"},{Z?.ToString("F0", perceiver) ?? "z"}#0]".SubstituteANSIColour();
	}

	public string ExitStrings(IPerceiver voyeur, ICellOverlay overlay, bool colour = true)
	{
		var sb = new StringBuilder();
		sb.Append(colour ? Telnet.Cyan + "Exits: " + Telnet.Green : "Exits: ");

		var chVoyeur = voyeur as ICharacter;
		var outputStrings =
			Gameworld.ExitManager
			         .GetExitsFor(this, overlay, voyeur?.RoomLayer)
			         .OrderBy(x => x.OutboundDirection)
			         .Where(x => chVoyeur?.CanSee(this, x) != false)
			         .Select(exit => exit.DescribeFor(voyeur, colour))
			         .ToList();
		sb.Append(outputStrings.ListToString());
		if (colour)
		{
			sb.AppendLine(Telnet.RESET);
		}

		return sb.ToString();
	}

	#endregion
}
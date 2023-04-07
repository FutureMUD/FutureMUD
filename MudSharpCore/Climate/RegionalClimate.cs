using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.Climate;

public class RegionalClimate : FrameworkItem, IRegionalClimate
{
	public RegionalClimate(MudSharp.Models.RegionalClimate climate, IFuturemud gameworld)
	{
		_id = climate.Id;
		_name = climate.Name;
		ClimateModel = gameworld.ClimateModels.Get(climate.ClimateModelId);

		foreach (var season in climate.RegionalClimatesSeasons)
		{
			var gseason = gameworld.Seasons.Get(season.SeasonId);
			_seasons.Add(gseason);
			var definition = XElement.Parse(season.TemperatureInfo);
			foreach (var element in definition.Elements("Value"))
			{
				_hourlyBaseTemperaturesBySeason[(gseason, int.Parse(element.Attribute("hour").Value))] =
					double.Parse(element.Value);
			}
		}

		SeasonRotation = new CircularRange<ISeason>(_seasons.First().Celestial.CelestialDaysPerYear,
			Seasons.Select(x => (x, (double)x.CelestialDayOnset)));
	}

	#region Overrides of Item

	public sealed override string FrameworkItemType => "RegionalClimate";

	#endregion

	#region Implementation of IRegionalClimate

	public IClimateModel ClimateModel { get; protected set; }
	private readonly List<ISeason> _seasons = new();
	public IEnumerable<ISeason> Seasons => _seasons;
	private readonly Dictionary<(ISeason Season, int DailyHour), double> _hourlyBaseTemperaturesBySeason = new();
	public CircularRange<ISeason> SeasonRotation { get; }

	public IReadOnlyDictionary<(ISeason Season, int DailyHour), double> HourlyBaseTemperaturesBySeason =>
		_hourlyBaseTemperaturesBySeason;

	public string Show(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Regional Climate #{Id.ToString("N0", voyeur)}: {Name.Colour(Telnet.Cyan)}");
		sb.AppendLine($"Climate Model: {ClimateModel.Name.Colour(Telnet.BoldCyan)}");
		sb.AppendLine("Seasons:");
		var hours = _hourlyBaseTemperaturesBySeason.Select(x => x.Key.DailyHour).Distinct().OrderBy(x => x).ToList();
		sb.AppendLine(StringUtilities.GetTextTable(
			from season in Seasons
			select new[] { season.Name }.Concat(hours.Select(x =>
				voyeur.Gameworld.UnitManager.DescribeBrief(_hourlyBaseTemperaturesBySeason[(season, x)],
					Framework.Units.UnitType.Temperature, voyeur))),
			new[] { "Name" }.Concat(hours.Select(x => x.ToString("N0", voyeur))),
			voyeur.LineFormatLength,
			unicodeTable: voyeur.Account.UseUnicode,
			denseTable: true
		));
		return sb.ToString();
	}

	#endregion
}
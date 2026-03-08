using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Economy.Tax;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Framework.Units;
using MudSharp.Models;
using MudSharp.PerceptionEngine;

namespace MudSharp.Climate;

public class RegionalClimate : SaveableItem, IRegionalClimate
{
	public RegionalClimate(MudSharp.Models.RegionalClimate climate, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = climate.Id;
		_name = climate.Name;
		Description = climate.Description ?? string.Empty;
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

		if (_seasons.Count > 0)
		{
			SeasonRotation = new CircularRange<ISeason>(_seasons.First().Celestial.CelestialDaysPerYear,
				Seasons.Select(x => (x, (double)x.CelestialDayOnset)));
		}
		else
		{
			SeasonRotation = new CircularRange<ISeason>();
		}
	}

	public RegionalClimate(IFuturemud gameworld, string name, IClimateModel model)
	{
		Gameworld = gameworld;
		_name = name;
		Description = "An undescribed regional climate.";
		ClimateModel = model;
		SeasonRotation = new CircularRange<ISeason>();
		DoDatabaseInsert();
	}

	private RegionalClimate(RegionalClimate rhs, string name)
	{
		Gameworld = rhs.Gameworld;
		_name = name;
		Description = rhs.Description;
		ClimateModel = rhs.ClimateModel;
		_seasons.AddRange(rhs._seasons);
		foreach (var item in rhs._hourlyBaseTemperaturesBySeason)
		{
			_hourlyBaseTemperaturesBySeason[item.Key] = item.Value;
		}
		if (_seasons.Count > 0)
		{
			SeasonRotation = new CircularRange<ISeason>(_seasons.First().Celestial.CelestialDaysPerYear,
				Seasons.Select(x => (x, (double)x.CelestialDayOnset)));
		}
		else
		{
			SeasonRotation = new CircularRange<ISeason>();
		}
		DoDatabaseInsert();
	}

	public IRegionalClimate Clone(string name)
	{
		return new RegionalClimate(this, name);
	}

	private void DoDatabaseInsert()
	{
		using (new FMDB())
		{
			var dbitem = new MudSharp.Models.RegionalClimate
			{
				Name = Name,
				Description = Description,
				ClimateModelId = ClimateModel.Id
			};
			FMDB.Context.RegionalClimates.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	#region Overrides of Item

	public sealed override string FrameworkItemType => "RegionalClimate";

	#endregion

	#region Implementation of IRegionalClimate

	public string Description { get; protected set; }
	public IClimateModel ClimateModel { get; protected set; }
	private readonly List<ISeason> _seasons = new();
	public IEnumerable<ISeason> Seasons => _seasons;
	private readonly Dictionary<(ISeason Season, int DailyHour), double> _hourlyBaseTemperaturesBySeason = new();
	public CircularRange<ISeason> SeasonRotation { get; private set; }

	public IReadOnlyDictionary<(ISeason Season, int DailyHour), double> HourlyBaseTemperaturesBySeason =>
		_hourlyBaseTemperaturesBySeason;

	public const string HelpText = @"You can use the following options with this command:

	#3name <name>#0 - renames this regional climate
	#3desc#0 - opens an editor for this regional climate's description
	#3model <model>#0 - changes the climate model
	#3season <which>#0 - toggles a season belonging to this regional climate
	#3temp <season> <hour> <temp>#0 - sets an hourly temperature
	#3temps <season> [<temp0> <temp1> ... <tempn>]#0 - bulk edits the seasonal temperatures";

	/// <inheritdoc />
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "desc":
			case "description":
				return BuildingCommandDescription(actor);
			case "model":
			case "climate":
			case "climatemodel":
				return BuildingCommandClimateModel(actor, command);
			case "season":
				return BuildingCommandSeason(actor, command);
			case "temp":
				return BuildingCommandTemperature(actor, command);
			case "temps":
				return BuildingCommandTemperatures(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandDescription(ICharacter actor)
	{
		actor.OutputHandler.Send("Enter the new description in the editor below.");
		actor.EditorMode(BuildingCommandDescriptionPost, BuildingCommandDescriptionCancel, 1.0, Description);
		return true;
	}

	private void BuildingCommandDescriptionCancel(IOutputHandler handler, object[] parameters)
	{
		handler.Send("You decide not to change the regional climate description.");
	}

	private void BuildingCommandDescriptionPost(string description, IOutputHandler handler, object[] parameters)
	{
		Description = description.Trim();
		Changed = true;
		handler.Send($"The regional climate description is now:\n\n{FormatDescription(Description, 80, "\t")}");
	}

	private bool BuildingCommandTemperatures(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which season do you want to edit the temperature for?");
			return false;
		}

		var season = _seasons.GetByIdOrName(command.PopSpeech());
		if (season is null)
		{
			actor.OutputHandler.Send($"The text {command.Last.ColourCommand()} is not a valid season that this regional climate has.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter at least one temperature.");
			return false;
		}

		var temps = new List<double>();
		while (!command.IsFinished)
		{
			if (!Gameworld.UnitManager.TryGetBaseUnits(command.PopParentheses(), UnitType.Temperature, actor, out var value))
			{
				actor.OutputHandler.Send($"The text {command.Last.ColourCommand()} is not a valid temperature.");
				return false;
			}

			temps.Add(value);
		}

		var i = 0;
		foreach (var temp in temps)
		{
			_hourlyBaseTemperaturesBySeason[(season, i++)] = temp;
		}

		var sb = new StringBuilder();
		sb.AppendLine($"The hourly temperature profile of the {season.Name.ColourValue()} season is now as follows:\n");
		var hours = _hourlyBaseTemperaturesBySeason
		            .Where(x => x.Key.Season == season)
		            .Select(x => x.Key.DailyHour)
		            .Distinct()
		            .OrderBy(x => x)
		            .ToList();
		var dummy = new int[]
		{
			1
		};
		sb.AppendLine(StringUtilities.GetTextTable(
			from item in dummy 
			select new List<string>(
				from x in hours 
				select Gameworld.UnitManager
				                .DescribeBrief(_hourlyBaseTemperaturesBySeason[(season, x)], Framework.Units.UnitType.Temperature, actor)),
			new List<string>(hours.Select(x => x.ToStringN0(actor))),
			actor,
			Telnet.Red
		));
		actor.OutputHandler.Send(sb.ToString());
		Changed = true;
		return true;
	}

	private bool BuildingCommandTemperature(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which season do you want to edit the temperature for?");
			return false;
		}

		var season = _seasons.GetByIdOrName(command.PopSpeech());
		if (season is null)
		{
			actor.OutputHandler.Send($"The text {command.Last.ColourCommand()} is not a valid season that this regional climate has.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which hour of the day do you want to edit for that season?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var hour) || hour < 0)
		{
			actor.OutputHandler.Send($"The text {command.Last.ColourCommand()} is not a valid number 0 or greater.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the baseline temperature for that season and hour or the day?");
			return false;
		}

		if (!Gameworld.UnitManager.TryGetBaseUnits(command.SafeRemainingArgument, UnitType.Temperature, actor, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid temperature.");
			return false;
		}

		_hourlyBaseTemperaturesBySeason[(season, hour)] = value;
		Changed = true;
		actor.OutputHandler.Send($"The temperature for the {hour.ToOrdinal().ColourValue()} hour in {season.Name.ColourValue()} is now {Gameworld.UnitManager.DescribeMostSignificantExact(value, UnitType.Temperature, actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandSeason(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which season do you want to toggle as belonging to this regional climate?");
			return false;
		}

		var season = Gameworld.Seasons.GetByIdOrName(command.SafeRemainingArgument);
		if (season is null)
		{
			actor.OutputHandler.Send($"There is no such season identified by the text {command.SafeRemainingArgument.ColourCommand()}.");
			return false;
		}

		if (_seasons.Contains(season))
		{
			_seasons.Remove(season);
			foreach (var item in _hourlyBaseTemperaturesBySeason.Where(x => x.Key.Season == season).AsEnumerable())
			{
				_hourlyBaseTemperaturesBySeason.Remove(item.Key);
			}

			actor.OutputHandler.Send($"This regional climate no longer contains the {season.Name.ColourValue()} season.");
		}
		else
		{
			_seasons.Add(season);
			var clock = Gameworld.Clocks.First();
			for (var i = 0; i < clock.HoursPerDay; i++)
			{
				_hourlyBaseTemperaturesBySeason[(season, i)] = 15.0;
			}
			actor.OutputHandler.Send($"This regional climate now contains the {season.Name.ColourValue()} season. Default temperature values have been added.");
		}
		if (_seasons.Count > 0)
		{
			SeasonRotation = new CircularRange<ISeason>(_seasons.First().Celestial.CelestialDaysPerYear,
				Seasons.Select(x => (x, (double)x.CelestialDayOnset)));
		}
		else
		{
			SeasonRotation = new CircularRange<ISeason>();
		}
		Changed = true;
		return true;
	}

	private bool BuildingCommandClimateModel(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which climate model should this regional climate use?");
			return false;
		}

		var model = Gameworld.ClimateModels.GetByIdOrName(command.SafeRemainingArgument);
		if (model is null)
		{
			actor.OutputHandler.Send($"There is no climate model identified by the text {command.SafeRemainingArgument.ColourCommand()}.");
			return false;
		}

		ClimateModel = model;
		Changed = true;
		actor.OutputHandler.Send($"This regional climate now uses the {model.Name.ColourName()} climate model.");
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to this regional climate?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.RegionalClimates.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already a regional climate with the name {name.ColourName()}. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename the regional climate from {Name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	public string Show(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Regional Climate #{Id.ToString("N0", voyeur)}: {Name}".GetLineWithTitleInner(voyeur, Telnet.Yellow, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine("Description:");
		sb.AppendLine(FormatDescription(Description, voyeur.InnerLineFormatLength, "\t"));
		sb.AppendLine();
		sb.AppendLine($"Climate Model: {ClimateModel.Name.Colour(Telnet.BoldCyan)}");
		sb.AppendLine();
		sb.AppendLine("Seasons:");
		sb.AppendLine();
		foreach (var range in SeasonRotation.Ranges)
		{
			sb.AppendLine($"\t{range.Value.Name.ColourValue()} from day #{range.LowerLimit.ToStringN0Colour(voyeur)} to day #{range.UpperLimit.ToStringN0Colour(voyeur)} ({range.Length.ToStringN0Colour(voyeur).ColourValue()} days)");
		}
		sb.AppendLine();
		sb.AppendLine("Seasonal Temperatures:");
		sb.AppendLine();
		var hours = _hourlyBaseTemperaturesBySeason.Select(x => x.Key.DailyHour).Distinct().OrderBy(x => x).ToList();
		sb.AppendLine(StringUtilities.GetTextTable(
			from season in Seasons
			select new[] { season.Name }.Concat(hours.Select(x =>
				voyeur.Gameworld.UnitManager.DescribeBrief(_hourlyBaseTemperaturesBySeason[(season, x)],
					Framework.Units.UnitType.Temperature, voyeur))),
			new[] { "Name" }.Concat(hours.Select(x => x.ToString("N0", voyeur))),
			voyeur.LineFormatLength,
			unicodeTable: voyeur.Account.UseUnicode,
			denseTable: false
		));
		return sb.ToString();
	}

	#endregion

	#region Overrides of SaveableItem

	/// <inheritdoc />
	public override void Save()
	{
		var dbitem = FMDB.Context.RegionalClimates.Find(Id);
		dbitem.Name = Name;
		dbitem.Description = Description;
		dbitem.ClimateModelId = ClimateModel.Id;
		FMDB.Context.RegionalClimatesSeasons.RemoveRange(dbitem.RegionalClimatesSeasons);
		foreach (var season in Seasons)
		{
			dbitem.RegionalClimatesSeasons.Add(new RegionalClimatesSeason
			{
				RegionalClimate = dbitem,
				SeasonId = season.Id,
				TemperatureInfo = new XElement("Definition",
					from value in _hourlyBaseTemperaturesBySeason.Where(x => x.Key.Season == season)
					select new XElement("Value",
						new XAttribute("hour", value.Key.DailyHour),
						value.Value
					)
				).ToString()
			});
		}
		Changed = false;
	}

	#endregion

	private static string FormatDescription(string description, int lineLength, string indent)
	{
		if (string.IsNullOrWhiteSpace(description))
		{
			return $"{indent}None";
		}

		return string.Join("\n",
			description
				.Replace("\r\n", "\n")
				.Split('\n')
				.Select(x => string.IsNullOrWhiteSpace(x) ? string.Empty : x.Wrap(lineLength, indent)));
	}
}

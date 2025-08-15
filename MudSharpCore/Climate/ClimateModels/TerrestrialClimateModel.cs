using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;

namespace MudSharp.Climate.ClimateModels;

public class TerrestrialClimateModel : ClimateModelBase
{
       private readonly DictionaryWithDefault<(ISeason Season, IWeatherEvent Event), double> _weatherEventChangeChance = new();

       private readonly CollectionDictionary<(ISeason Season, IWeatherEvent OldEvent), (IWeatherEvent NewEvent, double Chance)>
               _newWeatherEventChances = new();

       private readonly DictionaryWithDefault<ISeason, double> _maximumAdditionalChangeChanceFromStableWeather = new();
       private readonly DictionaryWithDefault<ISeason, double> _incrementalAdditionalChangeChanceFromStableWeather = new();

       private readonly Dictionary<(ISeason Season, IWeatherEvent OldEvent, TimeOfDay Time), (IWeatherEvent NewEvent, double Chance)[]> _cachedTransitions = new();
       private readonly Dictionary<(ISeason Season, IWeatherEvent OldEvent, TimeOfDay Time), double> _cachedTransitionTotals = new();
       private readonly Dictionary<(ISeason Season, TimeOfDay Time), (IWeatherEvent Event, double Chance)[]> _seasonEventCache = new();
       private readonly Dictionary<(ISeason Season, TimeOfDay Time), double> _seasonEventTotals = new();
       private static readonly TimeOfDay[] _timesOfDay = Enum.GetValues<TimeOfDay>();

       public override IEnumerable<ISeason> Seasons => _maximumAdditionalChangeChanceFromStableWeather.Keys.ToList();

       public override IEnumerable<IWeatherEvent> WeatherEvents => _weatherEventChangeChance.Select(x => x.Key.Event).Distinct().ToList();

       private void RecalculateCaches()
       {
               _cachedTransitions.Clear();
               _cachedTransitionTotals.Clear();
               foreach (var kvp in _newWeatherEventChances)
               {
                       foreach (var time in _timesOfDay)
                       {
                               var arr = kvp.Value.Where(x => x.NewEvent.PermittedTimesOfDay.Contains(time)).ToArray();
                               _cachedTransitions[(kvp.Key.Season, kvp.Key.OldEvent, time)] = arr;
                               _cachedTransitionTotals[(kvp.Key.Season, kvp.Key.OldEvent, time)] = arr.Sum(x => x.Chance);
                       }
               }

               _seasonEventCache.Clear();
               _seasonEventTotals.Clear();
               var temp = new Dictionary<(ISeason, TimeOfDay), List<(IWeatherEvent, double)>>();
               foreach (var kvp in _weatherEventChangeChance)
               {
                       foreach (var time in kvp.Key.Event.PermittedTimesOfDay)
                       {
                               var key = (kvp.Key.Season, time);
                               if (!temp.TryGetValue(key, out var list))
                               {
                                       list = new List<(IWeatherEvent, double)>();
                                       temp[key] = list;
                               }
                               list.Add((kvp.Key.Event, kvp.Value));
                       }
               }

               foreach (var kvp in temp)
               {
                       var arr = kvp.Value.ToArray();
                       _seasonEventCache[kvp.Key] = arr;
                       _seasonEventTotals[kvp.Key] = arr.Sum(x => x.Item2);
               }
       }

       private static IWeatherEvent SelectRandom((IWeatherEvent Event, double Chance)[] options, double total)
       {
               if (options.Length == 0)
               {
                       return null;
               }

               if (total <= 0.0)
               {
                       return options.GetRandomElement().Event;
               }

               var roll = Constants.Random.NextDouble() * total;
               foreach (var option in options)
               {
                       if (option.Chance <= 0.0)
                       {
                               continue;
                       }

                       if ((roll -= option.Chance) <= 0.0)
                       {
                               return option.Event;
                       }
               }

               return options.Last().Event;
       }

       private IWeatherEvent RandomEventForSeason(ISeason season, TimeOfDay time)
       {
               return _seasonEventCache.TryGetValue((season, time), out var options)
                       ? SelectRandom(options, _seasonEventTotals[(season, time)])
                       : null;
       }

       #region Overrides of ClimateModelBase

       public override IWeatherEvent HandleWeatherTick(IWeatherEvent currentWeather, ISeason currentSeason,
               TimeOfDay currentTime, int consecutiveUnchangedPeriods)
       {
               if (currentWeather is null)
               {
                       return RandomEventForSeason(currentSeason, currentTime);
               }

               var hasData = _weatherEventChangeChance.ContainsKey((currentSeason, currentWeather)) &&
                             _newWeatherEventChances.ContainsKey((currentSeason, currentWeather));
               var forceChange = !hasData || !currentWeather.PermittedTimesOfDay.Contains(currentTime);

               if (forceChange || RandomUtilities.Roll(1.0,
                           _weatherEventChangeChance[(currentSeason, currentWeather)] + Math.Min(
                                   _maximumAdditionalChangeChanceFromStableWeather[currentSeason],
                                   _incrementalAdditionalChangeChanceFromStableWeather[currentSeason] * consecutiveUnchangedPeriods)))
               {
                       if (_cachedTransitions.TryGetValue((currentSeason, currentWeather, currentTime), out var options) &&
                           options.Length > 0)
                       {
                               return SelectRandom(options,
                                       _cachedTransitionTotals[(currentSeason, currentWeather, currentTime)]);
                       }

                       if (forceChange)
                       {
                               return RandomEventForSeason(currentSeason, currentTime) ?? currentWeather;
                       }
               }

               return currentWeather;
       }

       public override IEnumerable<IWeatherEvent> PermittedTransitions(IWeatherEvent currentEvent, ISeason currentSeason,
               TimeOfDay timeOfDay)
       {
               return _cachedTransitions.TryGetValue((currentSeason, currentEvent, timeOfDay), out var list)
                       ? list.Select(x => x.NewEvent)
                       : Enumerable.Empty<IWeatherEvent>();
       }

	public const string HelpText = @"You can use the following options with this command:

	#3name <name>#0 - sets the name of this climate model
	#3ticks <minutes>#0 - sets the minutes between weather ticks
	#3echochance <%>#0 - sets the per-minute chance of echoes
	#3echoticks <minutes>#0 - sets the minimum number of minutes between echoes
	#3addseason <season>#0 - adds a season to this climate model
	#3cloneseason <clone> <season>#0 - adds a season to this climate model with all the data from another
	#3removeseason <season>#0 - removes a season from the climate model
	#3season <season> chance <event> <%>#0 - sets the base chance of changing per tick for an event/season combo
	#3season <season> transition <from> <to> <weight>#0 - sets a transition from one weather event to another
	#3season <season> transition <from> <to> 0#0 - removes a transition from one weather event to another";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "ticks":
				return BuildingCommandTicks(actor, command);
			case "echo":
			case "echochance":
				return BuildingCommandEchoChance(actor, command);
			case "echoticks":
				return BuildingCommandEchoTicks(actor, command);
			case "addseason":
				return BuildingCommandAddSeason(actor, command);
			case "removeseason":
				return BuildingCommandRemoveSeason(actor, command);
			case "cloneseason":
				return BuildingCommandCloneSeason(actor, command);
			case "season":
				return BuildingCommandSeason(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return true;
	}

	private bool BuildingCommandCloneSeason(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which season do you want to clone?");
			return false;
		}

		var clone = Gameworld.Seasons.GetByIdOrName(command.SafeRemainingArgument);
		if (clone is null)
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} does not represent a valid season.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which season do you want to add?");
			return false;
		}

		var season = Gameworld.Seasons.GetByIdOrName(command.SafeRemainingArgument);
		if (season is null)
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} does not represent a valid season.");
			return false;
		}

		if (Seasons.Contains(season))
		{
			actor.OutputHandler.Send($"The season {season.DisplayName.ColourValue()} is already present on this climate model.");
			return false;
		}

		if (season == clone)
		{
			actor.OutputHandler.Send($"You can't clone a season from itself.");
			return false;
		}

		_maximumAdditionalChangeChanceFromStableWeather[season] = _maximumAdditionalChangeChanceFromStableWeather[clone];
		_incrementalAdditionalChangeChanceFromStableWeather[season] = _incrementalAdditionalChangeChanceFromStableWeather[clone];
		foreach (var weather in WeatherEvents)
		{
			if (!_newWeatherEventChances.ContainsKey((clone, weather)))
			{
				continue;
			}

			_newWeatherEventChances.AddRange((season,weather), _newWeatherEventChances[(clone,weather)]);
			_weatherEventChangeChance[(season, weather)] = _weatherEventChangeChance[(clone, weather)];
		}

               Changed = true;
               RecalculateCaches();
               actor.OutputHandler.Send($"You add the season {season.DisplayName.ColourValue()} as a clone of {clone.DisplayName.ColourValue()}, inheriting all its events and chances.");
               return true;
       }

	private bool BuildingCommandSeason(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which season do you want to edit?");
			return false;
		}

		var season = Seasons.GetByIdOrName(command.PopSpeech());
		if (season is null)
		{
			actor.OutputHandler.Send($"There is no season identified by the text {command.Last.ColourCommand()} present on this model.");
			return false;
		}

		switch (command.PopForSwitch())
		{
			case "chance":
				return BuildingCommandSeasonChance(actor, command, season);
			case "trans":
			case "transition":
				return BuildingCommandSeasonTransition(actor, command, season);
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Season #{season.Id} - {season.DisplayName}".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Change Chance Increase Per Minute: {_incrementalAdditionalChangeChanceFromStableWeather[season].ToBonusPercentageString(actor)}");
		sb.AppendLine($"Maximum Stable Weather Chance Change: {_maximumAdditionalChangeChanceFromStableWeather[season].ToBonusPercentageString(actor)}");
		sb.AppendLine();
		sb.AppendLine("Events:");

		foreach (var item in WeatherEvents)
		{
			sb.AppendLine();
			sb.AppendLine($"\tWeather Event #{item.Id} - {item.Name.ColourValue()} - {_weatherEventChangeChance[(season, item)].ToStringP2Colour(actor)} chance of change");
			sb.AppendLine();
			var total = _newWeatherEventChances[(season, item)].Sum(x => x.Chance);
			foreach (var trans in _newWeatherEventChances[(season, item)].OrderByDescending(x => x.Chance))
			{
				sb.AppendLine($"\t\t{(trans.Chance / total).ToStringP2Colour(actor)} chance to change to {trans.NewEvent.Name.ColourValue()} (#{trans.NewEvent.Id.ToStringN0(actor)}) [weight: {trans.Chance.ToStringN2Colour(actor)}]");
			}
		}

		actor.OutputHandler.Send(sb.ToString());
		return true;
	}

	private bool BuildingCommandSeasonTransition(ICharacter actor, StringStack command, ISeason season)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which weather event do you want to change a transition from?");
			return false;
		}

		var weather = Gameworld.WeatherEvents.GetByIdOrName(command.PopSpeech());
		if (weather is null)
		{
			actor.OutputHandler.Send($"The text {command.Last.ColourCommand()} does not represent a valid weather event.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which weather event do you want to set the transition to?");
			return false;
		}

		var newWeather = Gameworld.WeatherEvents.GetByIdOrName(command.PopSpeech());
		if (newWeather is null)
		{
			actor.OutputHandler.Send($"The text {command.Last.ColourCommand()} does not represent a valid weather event.");
			return false;
		}

		var chance = 1.0;
		if (!command.IsFinished)
		{
			if (!double.TryParse(command.SafeRemainingArgument, out chance))
			{
				actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid number.");
				return false;
			}
		}

               if (chance <= 0.0)
               {
                       _newWeatherEventChances[(season, weather)].RemoveAll(x => x.NewEvent == newWeather);
                       Changed = true;
                       RecalculateCaches();
                       actor.OutputHandler.Send($"It will no longer be possible for the {weather.Name.ColourValue()} event to transition to the {newWeather.Name.ColourValue()} event.");
                       return false;
               }

               _newWeatherEventChances[(season, weather)].RemoveAll(x => x.NewEvent == newWeather);
               _newWeatherEventChances[(season,weather)].Add((newWeather, chance));
               Changed = true;
               RecalculateCaches();
               var total = _newWeatherEventChances[(season, weather)].Sum(x => x.Chance);
               actor.OutputHandler.Send($"The {weather.Name.ColourValue()} event will now transition to the {newWeather.Name.ColourValue()} event with a weight of {chance.ToStringN2Colour(actor)} ({(chance/total).ToStringP2Colour(actor)} chance).");
               return true;
       }

	private bool BuildingCommandSeasonChance(ICharacter actor, StringStack command, ISeason season)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which weather event do you want to set the base change chance for?");
			return false;
		}

		var weather = Gameworld.WeatherEvents.GetByIdOrName(command.PopSpeech());
		if (weather is null)
		{
			actor.OutputHandler.Send($"The text {command.Last.ColourCommand()} does not represent a valid weather event.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the base chance of that event changing to another event?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid percentage.");
			return false;
		}

		if (value < 0.0)
		{
			value = 0.0;
		}

               _weatherEventChangeChance[(season, weather)] = value;
               Changed = true;
               RecalculateCaches();
               actor.OutputHandler.Send($"The weather event {weather.Name.ColourValue()} in the {season.DisplayName.ColourValue()} season now has a {value.ToStringP2Colour(actor)} base chance to change.");
               return true;
       }

	private bool BuildingCommandRemoveSeason(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which season do you want to remove?");
			return false;
		}

		var season = Seasons.GetByIdOrName(command.SafeRemainingArgument);
		if (season is null)
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} does not represent a valid season.");
			return false;
		}

		actor.OutputHandler.Send($"Are you sure you want to remove all information associated with the season {season.DisplayName.ColourValue()}? This is irreversible.\n{Accept.StandardAcceptPhrasing}");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			AcceptAction = text =>
			{
				actor.OutputHandler.Send($"You delete the season {season.DisplayName.ColourValue()} from the climate model.");
				_incrementalAdditionalChangeChanceFromStableWeather.Remove(season);
                               _maximumAdditionalChangeChanceFromStableWeather.Remove(season);
                               _newWeatherEventChances.RemoveAllKeys(x => x.Season == season);
                               _weatherEventChangeChance.RemoveAllKeys<IDictionary<(ISeason,IWeatherEvent),double>,(ISeason,IWeatherEvent),double>(x => x.Item1 == season);
                               Changed = true;
                               RecalculateCaches();
                       },
			RejectAction = text =>
			{
				actor.OutputHandler.Send("You decide not to delete the season from the climate model.");
			},
			ExpireAction= () =>
			{
				actor.OutputHandler.Send("You decide not to delete the season from the climate model.");
			},
			DescriptionString = $"Deleting the season {season.Name} from a climate model",
			Keywords = ["delete", "season"]
		}), TimeSpan.FromSeconds(120));
		return true;
	}

	private bool BuildingCommandAddSeason(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which season do you want to add?");
			return false;
		}

		var season = Gameworld.Seasons.GetByIdOrName(command.SafeRemainingArgument);
		if (season is null)
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} does not represent a valid season.");
			return false;
		}

               if (Seasons.Contains(season))
               {
                       actor.OutputHandler.Send($"The season {season.DisplayName.ColourValue()} is already present on this climate model.");
                       return false;
               }

               _incrementalAdditionalChangeChanceFromStableWeather[season] = 0.0005;
               _maximumAdditionalChangeChanceFromStableWeather[season] = 0.3;
               Changed = true;
               RecalculateCaches();
               actor.OutputHandler.Send($"You add the season {season.DisplayName.ColourValue()} to this climate model.");
               return true;
       }

	private bool BuildingCommandEchoTicks(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How many in game minutes should pass at a minimum between flavour echoes?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value < 0)
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid positive number.");
			return false;
		}

		MinimumMinutesBetweenFlavourEchoes = value;
		Changed = true;
		actor.OutputHandler.Send($"Flavour echoes may now occur a minimum of once every {value.ToStringN0Colour(actor)} minutes.");
		return true;
	}

	private bool BuildingCommandEchoChance(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the percentage chance of getting a flavour echo per minute?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid percentage.");
			return false;
		}

		MinuteFlavourEchoChance = value;
		Changed = true;
		actor.OutputHandler.Send($"There will now be a {value.ToStringP2Colour(actor)} chance of a flavour echo every minute.");
		return true;
	}

	private bool BuildingCommandTicks(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How many in game minutes should pass between ticks on this model?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value <= 0)
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid number greater than zero.");
			return false;
		}

		MinuteProcessingInterval = value;
		Changed = true;
		actor.OutputHandler.Send($"This model now checks for weather changes every {value.ToStringN0Colour(actor)} minutes.");
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to this climate model?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.ClimateModels.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already a climate model with the name {name.ColourName()}. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename the climate model from {Name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	/// <inheritdoc />
	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Climate Model #{Id.ToStringN0(actor)} - {Name}".GetLineWithTitle(actor, Telnet.Blue, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Minutes Between Ticks: {MinuteProcessingInterval.ToStringN0Colour(actor)}");
		sb.AppendLine($"Minimum Minutes Between Echoes: {MinimumMinutesBetweenFlavourEchoes.ToStringN0Colour(actor)}");
		sb.AppendLine($"Echo Chance Per Minute: {MinuteFlavourEchoChance.ToStringP2Colour(actor)}");
		sb.AppendLine();
		foreach (var season in Seasons)
		{
			sb.AppendLine();
			sb.AppendLine($"Season #{season.Id} - {season.DisplayName}".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
			sb.AppendLine();
			sb.AppendLine($"Change Chance Increase Per Minute: {_incrementalAdditionalChangeChanceFromStableWeather[season].ToBonusPercentageString(actor)}");
			sb.AppendLine($"Maximum Stable Weather Chance Change: {_maximumAdditionalChangeChanceFromStableWeather[season].ToBonusPercentageString(actor)}");
			sb.AppendLine();
			sb.AppendLine("Events:");
			
			foreach (var item in WeatherEvents)
			{
				sb.AppendLine();
				sb.AppendLine($"\tWeather Event #{item.Id} - {item.Name.ColourValue()} - {_weatherEventChangeChance[(season,item)].ToStringP2Colour(actor)} chance of change");
				sb.AppendLine();
				var total = _newWeatherEventChances[(season, item)].Sum(x => x.Chance);
				foreach (var trans in _newWeatherEventChances[(season,item)].OrderByDescending(x => x.Chance))
				{
					sb.AppendLine($"\t\t{(trans.Chance/total).ToStringP2Colour(actor)} chance to change to {trans.NewEvent.Name.ColourValue()} (#{trans.NewEvent.Id.ToStringN0(actor)}) [weight: {trans.Chance.ToStringN2Colour(actor)}]");
				}
			}
		}
		return sb.ToString();
	}

	#endregion

	#region Overrides of ClimateModelBase

	/// <inheritdoc />
	public override IClimateModel Clone(string name)
	{
		return new TerrestrialClimateModel(this, name);
	}

	#endregion

	private void DoDatabaseInsert()
	{
		using (new FMDB())
		{
			var dbitem = new Models.ClimateModel
			{
				Name = Name,
				MinimumMinutesBetweenFlavourEchoes = MinimumMinutesBetweenFlavourEchoes,
				MinuteFlavourEchoChance = MinuteFlavourEchoChance,
				MinuteProcessingInterval = MinuteProcessingInterval,
				Type = "terrestrial"
			};
			
			foreach (var season in Seasons)
			{
				var dbseason = new Models.ClimateModelSeason
				{
					ClimateModel = dbitem,
					IncrementalAdditionalChangeChanceFromStableWeather = _incrementalAdditionalChangeChanceFromStableWeather[season],
					MaximumAdditionalChangeChanceFromStableWeather = _maximumAdditionalChangeChanceFromStableWeather[season],
					SeasonId = season.Id
				};
				dbitem.ClimateModelSeasons.Add(dbseason);

				foreach (var weather in WeatherEvents)
				{
					if (!_weatherEventChangeChance.ContainsKey((season, weather)))
					{
						continue;
					}

					var dbevent = new Models.ClimateModelSeasonEvent
					{
						ClimateModelSeason = dbseason,
						ChangeChance = _weatherEventChangeChance[(season, weather)],
						WeatherEventId = weather.Id,
						Transitions = new XElement("Transitions",
							from item in _newWeatherEventChances[(season, weather)]
							select new XElement("Event",
								new XAttribute("id", item.NewEvent.Id),
								new XAttribute("chance", item.Chance)
							)
						).ToString()
					};
					dbseason.SeasonEvents.Add(dbevent);
				}
			}

			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

        public TerrestrialClimateModel(IFuturemud gameworld, string name)
        {
                Gameworld = gameworld;
                _name = name;
                MinimumMinutesBetweenFlavourEchoes = 60;
                MinuteProcessingInterval = 60;
                MinuteFlavourEchoChance = 0.01;
                DoDatabaseInsert();
                RecalculateCaches();
        }

	public TerrestrialClimateModel(TerrestrialClimateModel rhs, string name)
	{
		Gameworld = rhs.Gameworld;
		_name = name;
		MinimumMinutesBetweenFlavourEchoes = rhs.MinimumMinutesBetweenFlavourEchoes;
		MinuteProcessingInterval = rhs.MinuteProcessingInterval;
		MinuteFlavourEchoChance = rhs.MinuteFlavourEchoChance;
                foreach (var season in rhs.Seasons)
                {
                        _incrementalAdditionalChangeChanceFromStableWeather[season] = rhs._incrementalAdditionalChangeChanceFromStableWeather[season];
                        _maximumAdditionalChangeChanceFromStableWeather[season] = rhs._maximumAdditionalChangeChanceFromStableWeather[season];
                        foreach (var weather in rhs.WeatherEvents)
                        {
                                if (!rhs._weatherEventChangeChance.ContainsKey((season, weather)))
                                {
                                        continue;
                                }

                                _weatherEventChangeChance[(season, weather)] = rhs._weatherEventChangeChance[(season, weather)];
                                _newWeatherEventChances[(season, weather)].AddRange(rhs._newWeatherEventChances[(season, weather)]);
                        }
                }
               DoDatabaseInsert();
               RecalculateCaches();
        }

	public TerrestrialClimateModel(Models.ClimateModel model, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = model.Id;
		_name = model.Name;
		MinuteProcessingInterval = model.MinuteProcessingInterval;
		MinimumMinutesBetweenFlavourEchoes = model.MinimumMinutesBetweenFlavourEchoes;
		MinuteFlavourEchoChance = model.MinuteFlavourEchoChance;
		foreach (var dbseason in model.ClimateModelSeasons)
		{
			var season = gameworld.Seasons.Get(dbseason.SeasonId);
			_maximumAdditionalChangeChanceFromStableWeather[season] = dbseason.MaximumAdditionalChangeChanceFromStableWeather;
			_incrementalAdditionalChangeChanceFromStableWeather[season] = dbseason.IncrementalAdditionalChangeChanceFromStableWeather;
			foreach (var dbevent in dbseason.SeasonEvents)
			{
				var we = gameworld.WeatherEvents.Get(dbevent.WeatherEventId);
				_weatherEventChangeChance[(season, we)] = dbevent.ChangeChance;
				var transitions = new List<(IWeatherEvent, double)>();
				foreach (var transition in XElement.Parse(dbevent.Transitions).Elements())
				{
					var other = gameworld.WeatherEvents.Get(long.Parse(transition.Attribute("id").Value));
					if (other == we)
					{
						continue;
					}
					transitions.Add((other, double.Parse(transition.Attribute("chance").Value)));
				}

                                _newWeatherEventChances[(season, we)] = transitions;
                        }
                }
               RecalculateCaches();
        }

	#region Overrides of SaveableItem

	/// <inheritdoc />
	public override void Save()
	{
		var dbitem = FMDB.Context.ClimateModels.Find(Id);
		dbitem.Name = Name;
		dbitem.MinimumMinutesBetweenFlavourEchoes = MinimumMinutesBetweenFlavourEchoes;
		dbitem.MinuteFlavourEchoChance = MinuteFlavourEchoChance;
		dbitem.MinuteProcessingInterval = MinuteProcessingInterval;
		FMDB.Context.ClimateModelSeasonEvent.RemoveRange(dbitem.ClimateModelSeasons.SelectMany(x => x.SeasonEvents));
		FMDB.Context.ClimateModelSeason.RemoveRange(dbitem.ClimateModelSeasons);
		foreach (var season in Seasons)
		{
			var dbseason = new Models.ClimateModelSeason
			{
				ClimateModel = dbitem,
				IncrementalAdditionalChangeChanceFromStableWeather = _incrementalAdditionalChangeChanceFromStableWeather[season],
				MaximumAdditionalChangeChanceFromStableWeather = _maximumAdditionalChangeChanceFromStableWeather[season],
				SeasonId = season.Id
			};
			dbitem.ClimateModelSeasons.Add(dbseason);

			foreach (var weather in WeatherEvents)
			{
				if (!_weatherEventChangeChance.ContainsKey((season, weather)))
				{
					continue;
				}
				var dbevent = new Models.ClimateModelSeasonEvent
				{
					ClimateModelSeason = dbseason,
					ChangeChance = _weatherEventChangeChance[(season, weather)],
					WeatherEventId = weather.Id,
					Transitions = new XElement("Transitions",
						from item in _newWeatherEventChances[(season, weather)]
						select new XElement("Event",
							new XAttribute("id", item.NewEvent.Id),
							new XAttribute("chance", item.Chance)
						)
					).ToString()
				};
				dbseason.SeasonEvents.Add(dbevent);
			}
		}

		Changed = false;
	}

	#endregion
}
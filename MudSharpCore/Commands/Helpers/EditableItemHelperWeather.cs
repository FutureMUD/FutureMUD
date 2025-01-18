using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character.Heritage;
using MudSharp.Climate;
using MudSharp.Climate.ClimateModels;
using MudSharp.Climate.WeatherEvents;
using MudSharp.Commands.Modules;
using MudSharp.Effects.Concrete;
using MudSharp.Framework.Revision;
using MudSharp.Framework;
using MudSharp.Framework.Units;

namespace MudSharp.Commands.Helpers;
public partial class EditableItemHelper
{
	public static EditableItemHelper WeatherEventHelper { get; } = new()
	{
		ItemName = "Weather Event",
		ItemNamePlural = "Weather Events",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IWeatherEvent>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IWeatherEvent>(actor) { EditingItem = (IWeatherEvent)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IWeatherEvent>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.WeatherEvents.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.WeatherEvents.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.WeatherEvents.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IWeatherEvent)item),
		CastToType = typeof(IWeatherEvent),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your weather event.");
				return;
			}

			var name = input.PopSpeech().TitleCase();
			if (actor.Gameworld.WeatherEvents.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send($"There is already a weather event called {name.ColourName()}. Names must be unique.");
				return;
			}

			IWeatherEvent we;
			switch (input.PopForSwitch())
			{
				case "simple":
					we = new SimpleWeatherEvent(actor.Gameworld, name);
					break;
				case "rain":
					we = new RainWeatherEvent(actor.Gameworld, name);
					break;
				default:
					actor.OutputHandler.Send("You must specify either #3simple#0 or #3rain#0 for the event type.".SubstituteANSIColour());
					return;
			}

			actor.Gameworld.Add(we);
			actor.RemoveAllEffects<BuilderEditingEffect<IWeatherEvent>>();
			actor.AddEffect(new BuilderEditingEffect<IWeatherEvent>(actor) { EditingItem = we });
			actor.OutputHandler.Send($"You create a new weather model called {name.ColourName()}, which you are also editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which weather event do you want to clone?");
				return;
			}

			var old = actor.Gameworld.WeatherEvents.GetByIdOrName(input.PopSpeech());
			if (old is null)
			{
				actor.OutputHandler.Send($"There is no weather event identified by the text {input.Last.ColourCommand()}.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your weather event.");
				return;
			}

			var name = input.PopSpeech().TitleCase();
			if (actor.Gameworld.WeatherEvents.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send($"There is already a weather event called {name.ColourName()}. Names must be unique.");
				return;
			}

			var we = old.Clone(name);
			actor.Gameworld.Add(we);
			actor.RemoveAllEffects<BuilderEditingEffect<IWeatherEvent>>();
			actor.AddEffect(new BuilderEditingEffect<IWeatherEvent>(actor) { EditingItem = we });
			actor.OutputHandler.Send($"You clone a new weather event called {name.ColourName()} from {old.Name.ColourName()}, which you are also editing.");
		},

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Times",
			"Obscure Sky?",
			"Light",
			"Precipitation",
			"Wind",
			"Temp",
			"Rain Temp",
			"Wind Temp",
			"Counts As"
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IWeatherEvent>()
														  select new List<string>
														  {
															  proto.Id.ToString("N0", character),
															  proto.Name,
															  proto.PermittedTimesOfDay.ListToColouredString(),
															  proto.ObscuresViewOfSky.ToColouredString(),
															  proto.LightLevelMultiplier.ToStringP2Colour(character),
															  proto.Precipitation.Describe(),
															  proto.Wind.Describe(),
															  character.Gameworld.UnitManager.DescribeExact(proto.TemperatureEffect, UnitType.TemperatureDelta, character),
															  character.Gameworld.UnitManager.DescribeExact(proto.PrecipitationTemperatureEffect, UnitType.TemperatureDelta, character),
															  character.Gameworld.UnitManager.DescribeExact(proto.WindTemperatureEffect, UnitType.TemperatureDelta, character),
															  proto.CountsAs?.Name ?? ""
														  },

		CustomSearch = (protos, keyword, gameworld) =>
		{
			return protos;
		},
		GetEditHeader = item => $"Weather Event #{item.Id:N0} ({item.Name})",
		DefaultCommandHelp = WeatherModule.WeatherEventHelp
	};

	public static EditableItemHelper ClimateModelHelper { get; } = new()
	{
		ItemName = "Climate Model",
		ItemNamePlural = "Climate Models",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IClimateModel>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IClimateModel>(actor) { EditingItem = (IClimateModel)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IClimateModel>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.ClimateModels.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.ClimateModels.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.ClimateModels.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IClimateModel)item),
		CastToType = typeof(IClimateModel),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your climate model.");
				return;
			}

			var name = input.PopSpeech().TitleCase();
			if (actor.Gameworld.ClimateModels.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send($"There is already a climate model called {name.ColourName()}. Names must be unique.");
				return;
			}

			var cm = new TerrestrialClimateModel(actor.Gameworld, name);
			actor.Gameworld.Add(cm);
			actor.RemoveAllEffects<BuilderEditingEffect<IClimateModel>>();
			actor.AddEffect(new BuilderEditingEffect<IClimateModel>(actor) { EditingItem = cm });
			actor.OutputHandler.Send($"You create a new climate model called {name.ColourName()}, which you are also editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which climate model do you want to clone?");
				return;
			}

			var model = actor.Gameworld.ClimateModels.GetByIdOrName(input.PopSpeech());
			if (model is null)
			{
				actor.OutputHandler.Send($"There is no climate model identified by the text {input.Last.ColourCommand()}.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your climate model.");
				return;
			}

			var name = input.PopSpeech().TitleCase();
			if (actor.Gameworld.ClimateModels.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send($"There is already a climate model called {name.ColourName()}. Names must be unique.");
				return;
			}

			var cm = model.Clone(name);
			actor.Gameworld.Add(cm);
			actor.RemoveAllEffects<BuilderEditingEffect<IClimateModel>>();
			actor.AddEffect(new BuilderEditingEffect<IClimateModel>(actor) { EditingItem = cm });
			actor.OutputHandler.Send($"You clone a new climate model called {name.ColourName()} from {model.Name.ColourName()}, which you are also editing.");
		},

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Seasons",
			"# Events"
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IClimateModel>()
														  select new List<string>
														  {
															  proto.Id.ToString("N0", character),
															  proto.Name,
															  proto.Seasons.Select(x => x.DisplayName).ListToColouredString(),
															  proto.WeatherEvents.Count().ToStringN0(character)
														  },

		CustomSearch = (protos, keyword, gameworld) =>
		{
			return protos;
		},
		GetEditHeader = item => $"Climate Model #{item.Id:N0} ({item.Name})",
		DefaultCommandHelp = WeatherModule.ClimateModelHelp
	};

	public static EditableItemHelper RegionalClimateHelper { get; } = new()
	{
		ItemName = "Regional Climate",
		ItemNamePlural = "Regional Climates",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IRegionalClimate>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IRegionalClimate>(actor) { EditingItem = (IRegionalClimate)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IRegionalClimate>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.RegionalClimates.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.RegionalClimates.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.RegionalClimates.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IRegionalClimate)item),
		CastToType = typeof(IRegionalClimate),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your regional climate.");
				return;
			}

			var name = input.PopSpeech().TitleCase();
			if (actor.Gameworld.RegionalClimates.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send($"There is already a regional climate called {name.ColourName()}. Names must be unique.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which climate model should this regional climate use?");
				return;
			}

			var climateModel = actor.Gameworld.ClimateModels.GetByIdOrName(input.PopSpeech());
			if (climateModel is null)
			{
				actor.OutputHandler.Send($"The text {input.Last.ColourCommand()} is not a valid climate model.");
				return;
			}

			var rc = new RegionalClimate(actor.Gameworld, name, climateModel);
			actor.Gameworld.Add(rc);
			actor.RemoveAllEffects<BuilderEditingEffect<IRegionalClimate>>();
			actor.AddEffect(new BuilderEditingEffect<IRegionalClimate>(actor) { EditingItem = rc });
			actor.OutputHandler.Send($"You create a new regional climate called {name.ColourName()}, which you are also editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which regional climate do you want to clone?");
				return;
			}

			var clone = actor.Gameworld.RegionalClimates.GetByIdOrName(input.PopSpeech());
			if (clone is null)
			{
				actor.OutputHandler.Send($"There is no regional climate identified by the text {input.Last.ColourCommand()}");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your regional climate.");
				return;
			}

			var name = input.PopSpeech().TitleCase();
			if (actor.Gameworld.RegionalClimates.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send($"There is already a regional climate called {name.ColourName()}. Names must be unique.");
				return;
			}

			var rc = clone.Clone(name);
			actor.Gameworld.Add(rc);
			actor.RemoveAllEffects<BuilderEditingEffect<IRegionalClimate>>();
			actor.AddEffect(new BuilderEditingEffect<IRegionalClimate>(actor) { EditingItem = rc });
			actor.OutputHandler.Send($"You create a new regional climate called {name.ColourName()} as a clone of {clone.Name.ColourName()}, which you are also editing.");
		},

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Climate",
			"Seasons"
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IRegionalClimate>()
														  select new List<string>
														  {
															  proto.Id.ToString("N0", character),
															  proto.Name,
															  proto.ClimateModel.Name,
															  proto.Seasons.Select(x => x.DisplayName).ListToColouredString(),
														  },

		CustomSearch = (protos, keyword, gameworld) =>
		{
			return protos;
		},
		GetEditHeader = item => $"Regional Climate #{item.Id:N0} ({item.Name})",
		DefaultCommandHelp = WeatherModule.RegionalClimateHelp
	};

	public static EditableItemHelper WeatherControllerHelper { get; } = new()
	{
		ItemName = "Weather Controller",
		ItemNamePlural = "Weather Controllers",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IWeatherController>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IWeatherController>(actor) { EditingItem = (IWeatherController)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IWeatherController>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.WeatherControllers.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.WeatherControllers.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.WeatherControllers.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IWeatherController)item),
		CastToType = typeof(IWeatherController),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your weather controller.");
				return;
			}

			var name = input.PopSpeech().TitleCase();
			if (actor.Gameworld.WeatherControllers.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send($"There is already a weather controller called {name.ColourName()}. Names must be unique.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which regional climate is this a weather controller for?");
				return;
			}

			var regionalClimate = actor.Gameworld.RegionalClimates.GetByIdOrName(input.PopSpeech());
			if (regionalClimate is null)
			{
				actor.OutputHandler.Send($"The text {input.Last.ColourCommand()} is not a valid regional climate.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which zone do you want to use for the geographic information?");
				return;
			}

			var zone = actor.Gameworld.Zones.GetByIdOrName(input.SafeRemainingArgument);
			if (zone is null)
			{
				actor.OutputHandler.Send($"The text {input.SafeRemainingArgument.ColourCommand()} is not a valid zone.");
				return;
			}

			if (!int.TryParse(input.SafeRemainingArgument, out var onset) || onset < 0)
			{
				actor.OutputHandler.Send($"The text {input.SafeRemainingArgument.ColourCommand()} is not a valid number 0 or greater.");
				return;
			}

			var wc = new WeatherController(actor.Gameworld, name, regionalClimate, zone);
			actor.Gameworld.Add(wc);
			actor.RemoveAllEffects<BuilderEditingEffect<IWeatherController>>();
			actor.AddEffect(new BuilderEditingEffect<IWeatherController>(actor) { EditingItem = wc });
			actor.OutputHandler.Send($"You create a new weather controller called {name.ColourName()}, which you are also editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			actor.OutputHandler.Send("Weather controllers can't be cloned. Create a new one instead.");
		},

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Climate",
			"Weather",
			"Temperature",
			"Season"
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IWeatherController>()
														  select new List<string>
														  {
															  proto.Id.ToString("N0", character),
															  proto.Name,
															  proto.RegionalClimate.Name,
															  proto.CurrentWeatherEvent?.Name ?? "",
															  character.Gameworld.UnitManager.DescribeMostSignificantExact(proto.CurrentTemperature, UnitType.Temperature, character).ColourValue(),
															  proto.CurrentSeason?.Name ?? ""
														  },

		CustomSearch = (protos, keyword, gameworld) =>
		{
			return protos;
		},
		GetEditHeader = item => $"Weather Controller #{item.Id:N0} ({item.Name})",
		DefaultCommandHelp = WeatherModule.WeatherControllerHelp
	};

	public static EditableItemHelper SeasonHelper { get; } = new()
	{
		ItemName = "Season",
		ItemNamePlural = "Seasons",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<ISeason>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<ISeason>(actor) { EditingItem = (ISeason)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<ISeason>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.Seasons.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.Seasons.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.Seasons.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((ISeason)item),
		CastToType = typeof(ISeason),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your season.");
				return;
			}

			var name = input.PopSpeech().TitleCase();
			if (actor.Gameworld.Seasons.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send($"There is already a season called {name.ColourName()}. Names must be unique.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("How many days into the celestial day should the onset of this season be?");
				return;
			}

			if (!int.TryParse(input.SafeRemainingArgument, out var onset) || onset < 0)
			{
				actor.OutputHandler.Send($"The text {input.SafeRemainingArgument.ColourCommand()} is not a valid number 0 or greater.");
				return;
			}

			var season = new Season(actor.Gameworld, name, onset);
			actor.Gameworld.Add(season);
			actor.RemoveAllEffects<BuilderEditingEffect<ISeason>>();
			actor.AddEffect(new BuilderEditingEffect<ISeason>(actor) { EditingItem = season });
			actor.OutputHandler.Send($"You create a new season called {name.ColourName()} at onset {onset.ToStringN0Colour(actor)}, which you are also editing.");

		},
		EditableCloneAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which season do you want to clone?");
				return;
			}

			var parent = actor.Gameworld.Seasons.GetByIdOrName(input.PopSpeech());
			if (parent is null)
			{
				actor.OutputHandler.Send("There is no such season for you to clone.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your season.");
				return;
			}

			var name = input.SafeRemainingArgument.TitleCase();
			if (actor.Gameworld.Seasons.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send($"There is already a season called {name.ColourName()}. Names must be unique.");
				return;
			}

			var season = parent.Clone(name);
			actor.Gameworld.Add(season);
			actor.RemoveAllEffects<BuilderEditingEffect<ISeason>>();
			actor.AddEffect(new BuilderEditingEffect<ISeason>(actor) { EditingItem = season });
			actor.OutputHandler.Send($"You clone the season {parent.Name.ColourName()} into a new season called {name.ColourName()}, which you are now editing.");
		},

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Display",
			"Celestial",
			"Onset",
			"Group"
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<ISeason>()
														  select new List<string>
														  {
															  proto.Id.ToString("N0", character),
															  proto.Name,
															  proto.DisplayName,
															  proto.Celestial.Name,
															  proto.CelestialDayOnset.ToStringN0(character),
															  proto.SeasonGroup
														  },

		CustomSearch = (protos, keyword, gameworld) =>
		{
			if (keyword.Length > 1 && keyword[0] == '+')
			{
				keyword = keyword.Substring(1);
				return protos
					   .Cast<ISeason>()
					   .Where(x =>
						   x.Name.Contains(keyword, StringComparison.InvariantCultureIgnoreCase) ||
						   x.DisplayName.Contains(keyword, StringComparison.InvariantCultureIgnoreCase) ||
						   x.SeasonGroup?.Contains(keyword, StringComparison.InvariantCultureIgnoreCase) == true)
					   .Cast<IEditableItem>()
					   .ToList();
			}

			if (keyword.Length > 1 && keyword[0] == '-')
			{
				keyword = keyword.Substring(1);
				return protos
					   .Cast<ISeason>()
					   .Where(x =>
						   !x.Name.Contains(keyword, StringComparison.InvariantCultureIgnoreCase) &&
						   !x.DisplayName.Contains(keyword, StringComparison.InvariantCultureIgnoreCase) &&
						   x.SeasonGroup?.Contains(keyword, StringComparison.InvariantCultureIgnoreCase) != true)
					   .Cast<IEditableItem>()
					   .ToList();
			}

			return protos;
		},
		GetEditHeader = item => $"Season #{item.Id:N0} ({item.Name})",
		DefaultCommandHelp = WeatherModule.SeasonHelp
	};
}

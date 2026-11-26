#nullable enable

using System.Globalization;
using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Climate;
using MudSharp.Construction;
using MudSharp.Framework;

namespace MudSharp.Commands.Modules;

internal partial class RoomBuilderModule
{
	private const string PreviewHelp = """
		room preview <factor> <value> [<factor> <value> ...]
		  time       dawn, morning, afternoon, dusk or night
		  wind       none, still, occasionalbreeze, breeze, wind, strongwind, galewind, hurricanewind or maelstromwind
		  rain       a current precipitation level, such as dry, rain, heavyrain or snow
		  recentrain the highest recent precipitation level
		  season     a season ID or internal name (see season list)
		  light      a non-negative lux value or a light band, such as "very dark"
		Example: room preview time dusk rain dry recentrain heavyrain wind strongwind
		Other conditions use their live values. The preview lasts for this command only.
		""";

	private static void RoomPreview(ICharacter actor, StringStack input)
	{
		if (input.IsFinished || input.PeekSpeech().EqualTo("help"))
		{
			actor.OutputHandler.Send(PreviewHelp.ColourCommand());
			return;
		}

		var conditions = new RoomDescriptionOverrides();
		while (!input.IsFinished)
		{
			var factor = input.PopSpeech().ToLowerInvariant();
			if (input.IsFinished)
			{
				actor.OutputHandler.Send($"Supply a value for {factor.ColourCommand()}. See {"room preview help".ColourCommand()}.");
				return;
			}

			var value = input.PopSpeech();
			switch (factor)
			{
				case "time":
					if (!value.TryParseEnum<TimeOfDay>(out var time))
					{
						actor.OutputHandler.Send("Choose dawn, morning, afternoon, dusk or night.");
						return;
					}
					conditions = conditions with { Time = time };
					break;
				case "wind":
					if (!value.TryParseEnum<WindLevel>(out var wind))
					{
						actor.OutputHandler.Send($"Choose a wind level from {Enum.GetNames<WindLevel>().ListToString()}.");
						return;
					}
					conditions = conditions with { Wind = wind };
					break;
				case "rain":
				case "recentrain":
					if (!value.TryParseEnum<PrecipitationLevel>(out var rain))
					{
						actor.OutputHandler.Send($"Choose a precipitation level from {Enum.GetNames<PrecipitationLevel>().ListToString()}.");
						return;
					}
					conditions = factor == "rain" ? conditions with { Rain = rain } : conditions with { RecentRain = rain };
					break;
				case "season":
					var season = actor.Gameworld.Seasons.GetByIdOrName(value, false);
					if (season is null)
					{
						actor.OutputHandler.Send($"There is no season called {value.ColourCommand()}. See {"season list".ColourCommand()}.");
						return;
					}
					conditions = conditions with { Season = season };
					break;
				case "light":
					var model = actor.Gameworld.LightModel;
					if (!double.TryParse(value, NumberStyles.Float, actor, out var lux))
					{
						if (!model.LightDescriptions.Any(x => x.EqualTo(value)))
						{
							actor.OutputHandler.Send($"Use a lux value or one of {model.LightDescriptions.ListToString()}.");
							return;
						}
						// Adjacent light bands share an inclusive endpoint. Step inside the requested band.
						lux = Math.BitIncrement(model.GetMinimumIlluminationForDescription(value));
					}
					else if (!double.IsFinite(lux) || lux < 0.0)
					{
						actor.OutputHandler.Send("Light must be a finite, non-negative number of lux.");
						return;
					}
					conditions = conditions with { Lux = lux };
					break;
				default:
					actor.OutputHandler.Send($"Unknown preview factor {factor.ColourCommand()}. See {"room preview help".ColourCommand()}.");
					return;
			}
		}

		actor.OutputHandler.Send(actor.Location.PreviewDescription(actor, conditions).Wrap(actor.InnerLineFormatLength));
	}
}

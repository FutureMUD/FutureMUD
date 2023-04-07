using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;

namespace MudSharp.PerceptionEngine;

public static partial class OutputHandlerExtensions
{
	public static bool PrioritySend(this IOutputHandler handler, string text, bool newline = true, bool nopage = false)
	{
		var result = handler.Send(text, newline, nopage);
		Futuremud.Games.First().ForceOutgoingMessages();
		return result;
	}
}

public static class OutputExtensions
{
	public static void Send<T>(this T target, string text, params object[] parameters) where T : IHandleOutput
	{
		if (parameters.Any())
		{
			if (target is IFormatProvider tfp)
			{
				target.OutputHandler?.Send(string.Format(tfp, text, parameters));
			}
			else
			{
				target.OutputHandler?.Send(string.Format(text, parameters));
			}
		}
		else
		{
			target.OutputHandler?.Send(text);
		}
	}

	public static void SendNoNewLine<T>(this T target, string text, params object[] parameters)
		where T : IHandleOutput
	{
		if (parameters.Any())
		{
			if (target is IFormatProvider tfp)
			{
				target.OutputHandler?.Send(string.Format(tfp, text, parameters), false);
			}
			else
			{
				target.OutputHandler?.Send(string.Format(text, parameters), false);
			}
		}
		else
		{
			target.OutputHandler?.Send(text, false);
		}
	}

	public static void SendNoPage<T>(this T target, string text, params object[] parameters) where T : IHandleOutput
	{
		if (parameters.Any())
		{
			if (target is IFormatProvider tfp)
			{
				target.OutputHandler?.Send(string.Format(tfp, text, parameters), nopage: true);
			}
			else
			{
				target.OutputHandler?.Send(string.Format(text, parameters), nopage: true);
			}
		}
		else
		{
			target.OutputHandler?.Send(text);
		}
	}

	public static void Handle<T>(this IEnumerable<T> targets, string text) where T : IHandleOutput
	{
		foreach (var target in targets)
		{
			target.OutputHandler?.Send(text);
		}
	}

	public static void Handle<T>(this IEnumerable<T> targets, Output output) where T : IHandleOutput
	{
		foreach (var target in targets)
		{
			target.OutputHandler?.Send(output, !output.Style.HasFlag(OutputStyle.NoNewLine),
				output.Style.HasFlag(OutputStyle.NoPage));
		}
	}

	public static void Handle(this ILocation location, string text)
	{
		foreach (var ch in location?.Characters ?? Enumerable.Empty<ICharacter>())
		{
			ch.OutputHandler?.Send(text);
		}
	}

	public static void Handle(this ILocation location, IOutput output)
	{
		foreach (var ch in location?.Characters ?? Enumerable.Empty<ICharacter>())
		{
			ch.OutputHandler?.Send(output, !output.Style.HasFlag(OutputStyle.NoNewLine),
				!output.Style.HasFlag(OutputStyle.NoPage));
		}

		if (!output.Flags.HasFlag(OutputFlags.IgnoreWatchers))
		{
			foreach (var effect in location?.EffectsOfType<IRemoteObservationEffect>().ToList() ??
			                       Enumerable.Empty<IRemoteObservationEffect>())
			{
				effect.HandleOutput(output, location);
			}
		}
	}

	public static void Handle(this ILocation location, RoomLayer layer, string text)
	{
		foreach (var ch in location?.LayerCharacters(layer) ?? Enumerable.Empty<ICharacter>())
		{
			ch.OutputHandler?.Send(text);
		}
	}

	public static void Handle(this ILocation location, RoomLayer layer, IOutput output)
	{
		foreach (var ch in location?.LayerCharacters(layer) ?? Enumerable.Empty<ICharacter>())
		{
			ch.OutputHandler?.Send(output, !output.Style.HasFlag(OutputStyle.NoNewLine),
				!output.Style.HasFlag(OutputStyle.NoPage));
		}

		if (!output.Flags.HasFlag(OutputFlags.IgnoreWatchers))
		{
			foreach (var effect in location?.EffectsOfType<IRemoteObservationEffect>().ToList() ??
			                       Enumerable.Empty<IRemoteObservationEffect>())
				// TODO - layer awareness for watching?
			{
				effect.HandleOutput(output, location);
			}
		}
	}

	public static void Handle(this IOutputHandler handler, string text, OutputRange range = OutputRange.Personal)
	{
		var location =
			(handler.Perceiver as ICharacter)?.Corpse?.Parent.Location ??
			handler.Perceiver?.Location ??
			(handler.Perceiver as IGameItem)?.TrueLocations.FirstOrDefault();
		switch (range)
		{
			case OutputRange.Personal:
				handler.Send(text);
				break;

			case OutputRange.Local:
				foreach (var character in location?.Characters.Where(x => x.RoomLayer == handler.Perceiver.RoomLayer) ??
				                          Enumerable.Empty<ICharacter>())
				{
					character.OutputHandler?.Send(text);
				}

				break;

			case OutputRange.Surrounds:
				foreach (
					var character in
					location?.Surrounds.SelectMany(x => x.Characters) ?? Enumerable.Empty<ICharacter>())
				{
					character.OutputHandler?.Send(text);
				}

				break;

			case OutputRange.Room:
				foreach (var character in location?.Characters ?? Enumerable.Empty<ICharacter>())
				{
					character.OutputHandler?.Send(text);
				}

				break;

			case OutputRange.Zone:
				foreach (var character in location?.Zone.Characters ?? Enumerable.Empty<ICharacter>())
				{
					character.OutputHandler?.Send(text);
				}

				break;

			case OutputRange.Shard:
				foreach (var character in location?.Shard.Characters ?? Enumerable.Empty<ICharacter>())
				{
					character.OutputHandler?.Send(text);
				}

				break;

			case OutputRange.All:
				foreach (var character in handler.Perceiver?.Gameworld.Characters ?? Enumerable.Empty<ICharacter>()
				        )
				{
					character.OutputHandler?.Send(text);
				}

				break;

			case OutputRange.Game:
				handler.Perceiver?.Gameworld.Broadcast(text);
				break;
		}
	}

	public static void HandleVicinity(this IOutputHandler handler, IOutput output, uint maxRange)
	{
		if (handler == null || output == null)
		{
#if DEBUG
			Console.WriteLine($"Message to IOutputHandler.HandleVicinity had a null handler: {output.RawString}");
#endif
			return;
		}

		var newline = !output.Style.HasFlag(OutputStyle.NoNewLine);
		var nopage = output.Style.HasFlag(OutputStyle.NoPage);

		var location = (handler.Perceiver as ICharacter)?.Corpse?.Parent.Location ??
		               handler.Perceiver?.Location ??
		               (handler.Perceiver as IGameItem)?.TrueLocations.FirstOrDefault();
		var vicinity = location?.CellsInVicinity(maxRange, false, false).Except(location) ?? Enumerable.Empty<ICell>();
		foreach (var loc in vicinity)
		{
			foreach (var character in loc?.Characters ?? Enumerable.Empty<ICharacter>())
			{
				character.OutputHandler?.Send(output, newline, nopage);
			}

			if (!output.Flags.HasFlag(OutputFlags.IgnoreWatchers))
			{
				foreach (var effect in loc?.EffectsOfType<IRemoteObservationEffect>().ToList() ??
				                       Enumerable.Empty<IRemoteObservationEffect>())
				{
					effect.HandleOutput(output, loc);
				}
			}
		}
	}

	public static void Handle(this IOutputHandler handler, IOutput output, OutputRange range = OutputRange.Local)
	{
		if (handler == null || output == null)
		{
#if DEBUG
			Console.WriteLine($"Message to IOutputHandler.Handle had a null handler: {output.RawString}");
#endif
			return;
		}

		var location = (handler.Perceiver as ICharacter)?.Corpse?.Parent.Location ??
		               handler.Perceiver?.Location ??
		               (handler.Perceiver as IGameItem)?.TrueLocations.FirstOrDefault();
		var newline = !output.Style.HasFlag(OutputStyle.NoNewLine);
		var nopage = output.Style.HasFlag(OutputStyle.NoPage);

		switch (range)
		{
			case OutputRange.Personal:
				handler.Send(output, newline, nopage);
				break;

			case OutputRange.Local:
				var locations = (handler.Perceiver as IGameItem)?.TrueLocations ?? new[] { location };
				foreach (var loc in locations)
				{
					foreach (var character in loc?.Characters.Where(x => x.RoomLayer == handler.Perceiver.RoomLayer) ??
					                          Enumerable.Empty<ICharacter>())
					{
						character.OutputHandler?.Send(output, newline, nopage);
					}

					if (!output.Flags.HasFlag(OutputFlags.IgnoreWatchers))
					{
						foreach (var effect in loc?.EffectsOfType<IRemoteObservationEffect>().ToList() ??
						                       Enumerable.Empty<IRemoteObservationEffect>())
						{
							effect.HandleOutput(output, loc);
						}
					}
				}

				break;

			case OutputRange.Surrounds:
				foreach (var loc in location?.Surrounds ?? Enumerable.Empty<ICell>())
				{
					foreach (var character in loc?.Characters ?? Enumerable.Empty<ICharacter>())
					{
						character.OutputHandler?.Send(output, newline, nopage);
					}

					if (!output.Flags.HasFlag(OutputFlags.IgnoreWatchers))
					{
						foreach (var effect in loc?.EffectsOfType<IRemoteObservationEffect>().ToList() ??
						                       Enumerable.Empty<IRemoteObservationEffect>())
						{
							effect.HandleOutput(output, loc);
						}
					}
				}

				break;

			case OutputRange.Room:
				foreach (var character in location?.Characters ?? Enumerable.Empty<ICharacter>())
				{
					character.OutputHandler?.Send(output, newline, nopage);
				}

				foreach (var cell in location?.Room.Cells ?? Enumerable.Empty<ICell>())
				foreach (var effect in cell.EffectsOfType<IRemoteObservationEffect>().ToList())
				{
					effect.HandleOutput(output, cell);
				}

				break;

			case OutputRange.Zone:
				foreach (var character in location?.Zone.Characters ?? Enumerable.Empty<ICharacter>())
				{
					character.OutputHandler?.Send(output, newline, nopage);
				}

				foreach (var cell in location?.Zone.Cells ?? Enumerable.Empty<ICell>())
				{
					if (!output.Flags.HasFlag(OutputFlags.IgnoreWatchers))
					{
						foreach (var effect in cell.EffectsOfType<IRemoteObservationEffect>().ToList())
						{
							effect.HandleOutput(output, cell);
						}
					}
				}

				break;

			case OutputRange.Shard:
				foreach (var character in location?.Shard.Characters ?? Enumerable.Empty<ICharacter>())
				{
					character.OutputHandler?.Send(output, newline, nopage);
				}

				foreach (var cell in location?.Shard.Cells ?? Enumerable.Empty<ICell>())
				{
					if (!output.Flags.HasFlag(OutputFlags.IgnoreWatchers))
					{
						foreach (var effect in cell.EffectsOfType<IRemoteObservationEffect>().ToList())
						{
							effect.HandleOutput(output, cell);
						}
					}
				}

				break;

			case OutputRange.All:
				foreach (var character in handler.Perceiver?.Gameworld.Characters ?? Enumerable.Empty<ICharacter>()
				        )
				{
					character.OutputHandler?.Send(output, newline, nopage);
				}

				break;

			case OutputRange.Game:
				handler.Perceiver?.Gameworld.Broadcast(output.RawString);
				break;
		}
	}
}
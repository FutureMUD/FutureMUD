using MudSharp.Construction;
using MudSharp.Form.Shape;
using MudSharp.GameItems;

namespace MudSharp.PerceptionEngine;

public static partial class OutputHandlerExtensions
{
    public static bool PrioritySend(this IOutputHandler handler, string text, bool newline = true, bool nopage = false)
    {
        bool result = handler.Send(text, newline, nopage);
        Futuremud.Games.First().ForceOutgoingMessages();
        return result;
    }
}

public static class OutputExtensions
{
	private sealed record RouteLocalRecipientSet(
		bool HasValidSource,
		SpatialLocation? Origin,
		IReadOnlyCollection<ICharacter> Characters);

	private static IPerceiver OutputSource(IOutput output)
	{
		return output switch
		{
			IEmoteOutput emote => emote.DefaultSource,
			Outputs.LanguageOutput { DefaultSource: IPerceiver source } => source,
			_ => null
		};
	}

	private static RouteLocalRecipientSet RouteLocalCharacters(
		ILocation location,
		IPerceiver source,
		RoomLayer layer)
	{
		if (location is not ICell { RouteDefinition: not null } cell)
		{
			return null;
		}

		if (!TryResolveRouteOutputOrigin(cell, source, layer, out var origin))
		{
			return new RouteLocalRecipientSet(false, null, Array.Empty<ICharacter>());
		}

		var gameworld = source.Gameworld ?? cell.Gameworld;
		var maximumDistance = gameworld?.GetStaticDouble("RouteCellVeryDistantDistanceMetres") ?? 0.0;
		if (!double.IsFinite(maximumDistance) || maximumDistance <= 0.0)
		{
			maximumDistance = RouteSpatialConfiguration.Default.VeryDistantDistanceMetres;
		}

		var characters = RouteSpatialService.Instance.GetPerceivablesWithin(
				origin,
				maximumDistance,
				x => x.RoomLayer == layer)
			.OfType<ICharacter>()
			.ToArray();
		return new RouteLocalRecipientSet(true, origin, characters);
	}

	private static bool TryResolveRouteOutputOrigin(
		ICell cell,
		IPerceiver source,
		RoomLayer layer,
		out SpatialLocation origin)
	{
		if (source is null)
		{
			origin = default;
			return false;
		}

		var spatial = RouteSpatialService.Instance;
		var effective = spatial.GetEffectiveLocation(source);
		if (ReferenceEquals(effective.Cell, cell))
		{
			origin = new SpatialLocation(cell, layer, effective.RoutePositionMetres);
			if (spatial.TryValidateLocation(origin, out _))
			{
				return true;
			}
		}

		if (source is IGameItem item)
		{
			var owner = item.InInventoryOf ?? (ILocateable)item.ContainedIn;
			var inheritedPosition = spatial.GetInheritedRoutePosition(item, owner);
			if (inheritedPosition.HasValue)
			{
				origin = new SpatialLocation(cell, layer, inheritedPosition);
				return spatial.TryValidateLocation(origin, out _);
			}
		}

		origin = default;
		return false;
	}

	public static void HandleLocal(
		this ILocation location,
		IPerceiver source,
		RoomLayer layer,
		string text)
	{
		var routeRecipients = RouteLocalCharacters(location, source, layer);
		if (routeRecipients is null)
		{
			location.Handle(layer, text);
			return;
		}
		if (!routeRecipients.HasValidSource)
		{
			return;
		}

		foreach (var character in routeRecipients.Characters)
		{
			character.OutputHandler?.Send(text);
		}

		foreach (var effect in RemoteObservationEffectsFor(location)
			         .Where(x => x.Observes(routeRecipients.Origin!.Value)))
		{
			effect.HandleOutput(text, location);
		}

		// Legacy room-echo subscribers have no source coordinate and therefore cannot safely
		// consume a local RouteCell event without leaking it across the entire linear cell.
	}

	private static void HandleWholeLocation(ILocation location, string text)
	{
		foreach (var character in location?.Characters ?? [])
		{
			character.OutputHandler?.Send(text);
		}

		foreach (var effect in RemoteObservationEffectsFor(location))
		{
			effect.HandleOutput(text, location);
		}

		location.HandleRoomEcho(text);
	}

	private static void HandleWholeLocation(ILocation location, IOutput output)
	{
		foreach (var character in location?.Characters ?? [])
		{
			character.OutputHandler?.Send(output, !output.Style.HasFlag(OutputStyle.NoNewLine),
				!output.Style.HasFlag(OutputStyle.NoPage));
		}

		if (!output.Flags.HasFlag(OutputFlags.IgnoreWatchers))
		{
			foreach (var effect in RemoteObservationEffectsFor(location))
			{
				effect.HandleOutput(output, location);
			}
		}

		if (output is IEmoteOutput emoteOutput)
		{
			location.HandleRoomEcho(emoteOutput);
		}
		else
		{
			location.HandleRoomEcho(output.ParseFor(null));
		}
	}

	public static void HandleLocal(
		this ILocation location,
		IPerceiver source,
		RoomLayer layer,
		IOutput output)
	{
		var routeRecipients = RouteLocalCharacters(location, source, layer);
		if (routeRecipients is null)
		{
			location.Handle(layer, output);
			return;
		}
		if (!routeRecipients.HasValidSource)
		{
			return;
		}

		foreach (var character in routeRecipients.Characters)
		{
			character.OutputHandler?.Send(output, !output.Style.HasFlag(OutputStyle.NoNewLine),
				!output.Style.HasFlag(OutputStyle.NoPage));
		}

		if (!output.Flags.HasFlag(OutputFlags.IgnoreWatchers))
		{
			foreach (var effect in RemoteObservationEffectsFor(location)
			         .Where(x => x.Observes(routeRecipients.Origin!.Value)))
			{
				effect.HandleOutput(output, location);
			}
		}

		// Do not raise the legacy cell-wide room-echo event here: it has no source coordinate.
		// Explicit OutputRange.Room continues to invoke that topology-wide observer surface.
	}

    private static IEnumerable<IRemoteObservationEffect> RemoteObservationEffectsFor(ILocation location)
    {
        if (location is null)
        {
            return Enumerable.Empty<IRemoteObservationEffect>();
        }

        return location.Cells
                       .SelectMany(x => x.EffectsOfType<IRemoteObservationEffect>())
                       .Concat(location.Characters.SelectMany(x => x.EffectsOfType<IRemoteObservationEffect>()))
                       .Distinct()
                       .ToList();
    }

    public static void Send<T>(this T target, string text, params object[] parameters) where T : IHandleOutput
    {
        if (!text.IsValidFormatString(parameters?.Length ?? 0))
        {
            target.OutputHandler?.Send(text);
            return;
        }

        if (parameters.Length != 0)
        {
            if (target is IFormatProvider tfp)
            {
                target.OutputHandler?.Send(string.Format(tfp, text, parameters));
                return;
            }

            target.OutputHandler?.Send(string.Format(text, parameters));
            return;
        }
        target.OutputHandler?.Send(text);
    }

    public static void SendNoNewLine<T>(this T target, string text, params object[] parameters)
        where T : IHandleOutput
    {
        if (!text.IsValidFormatString(parameters?.Length ?? 0))
        {
            target.OutputHandler?.Send(text, false);
            return;
        }

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
        if (!text.IsValidFormatString(parameters?.Length ?? 0))
        {
            target.OutputHandler?.Send(text, nopage: true);
            return;
        }

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
            target.OutputHandler?.Send(text, nopage: true);
        }
    }

    public static void Handle<T>(this IEnumerable<T> targets, string text) where T : IHandleOutput
    {
        foreach (T target in targets)
        {
            target.OutputHandler?.Send(text);
        }
    }

    public static void Handle<T>(this IEnumerable<T> targets, Output output) where T : IHandleOutput
    {
        foreach (T target in targets)
        {
            target.OutputHandler?.Send(output, !output.Style.HasFlag(OutputStyle.NoNewLine),
                output.Style.HasFlag(OutputStyle.NoPage));
        }
    }

    public static void Handle(this ILocation location, string text)
    {
		if (location is ICell { RouteDefinition: not null })
		{
			// Source-less local output cannot be mapped to a longitudinal coordinate. Fail closed;
			// callers that intend a whole RouteCell broadcast must request OutputRange.Room.
			return;
		}

        foreach (ICharacter ch in location?.Characters ?? [])
        {
            ch.OutputHandler?.Send(text);
        }

        foreach (IRemoteObservationEffect effect in RemoteObservationEffectsFor(location))
        {
            effect.HandleOutput(text, location);
        }

        location.HandleRoomEcho(text);
    }

    public static void Handle(this ILocation location, string text, OutputFlags flags)
    {
        if (flags == OutputFlags.Normal)
        {
            location.Handle(text);
            return;
        }

        location.Handle(new RawOutput(text, flags: flags));
    }

    public static void Handle(this ILocation location, IOutput output)
    {
        var source = OutputSource(output);
		if (location is ICell { RouteDefinition: not null })
		{
			location.HandleLocal(source, source?.RoomLayer ?? RoomLayer.GroundLevel, output);
			return;
		}

		foreach (ICharacter ch in location?.Characters ?? [])
        {
            ch.OutputHandler?.Send(output, !output.Style.HasFlag(OutputStyle.NoNewLine),
                !output.Style.HasFlag(OutputStyle.NoPage));
        }

        if (!output.Flags.HasFlag(OutputFlags.IgnoreWatchers))
        {
            foreach (IRemoteObservationEffect effect in RemoteObservationEffectsFor(location))
            {
                effect.HandleOutput(output, location);
            }
        }

        if (output is IEmoteOutput emoteOutput)
        {
            location.HandleRoomEcho(emoteOutput);
        }
        else
        {
            location.HandleRoomEcho(output.ParseFor(null));
        }
    }

    public static void Handle(this ILocation location, RoomLayer layer, string text)
    {
		if (location is ICell { RouteDefinition: not null })
		{
			return;
		}

        foreach (ICharacter ch in location?.LayerCharacters(layer) ?? [])
        {
            ch.OutputHandler?.Send(text);
        }

        foreach (IRemoteObservationEffect effect in RemoteObservationEffectsFor(location))
        {
            effect.HandleOutput(text, location);
        }

        location.HandleRoomEcho(text, layer);
    }

    public static void Handle(this ILocation location, RoomLayer layer, string text, OutputFlags flags)
    {
        if (flags == OutputFlags.Normal)
        {
            location.Handle(layer, text);
            return;
        }

        location.Handle(layer, new RawOutput(text, flags: flags));
    }

    public static void Handle(this ILocation location, RoomLayer layer, IOutput output)
    {
		if (location is ICell { RouteDefinition: not null })
		{
			location.HandleLocal(OutputSource(output), layer, output);
			return;
		}

		foreach (ICharacter ch in location?.LayerCharacters(layer) ?? [])
        {
            ch.OutputHandler?.Send(output, !output.Style.HasFlag(OutputStyle.NoNewLine),
                !output.Style.HasFlag(OutputStyle.NoPage));
        }

        if (!output.Flags.HasFlag(OutputFlags.IgnoreWatchers))
        {
            foreach (IRemoteObservationEffect effect in RemoteObservationEffectsFor(location))
            // TODO - layer awareness for watching?
            {
                effect.HandleOutput(output, location);
            }
        }

        if (output is IEmoteOutput emoteOutput)
        {
            location.HandleRoomEcho(emoteOutput, layer);
        }
        else
        {
            location.HandleRoomEcho(output.ParseFor(null), layer);
        }
    }

    public static void Handle(this IOutputHandler handler, string text, OutputRange range = OutputRange.Personal)
    {
        ICell location =
            (handler.Perceiver as ICharacter)?.Corpse?.Parent.Location ??
            handler.Perceiver?.Location ??
            (handler.Perceiver as IGameItem)?.TrueLocations.FirstOrDefault();
        switch (range)
        {
            case OutputRange.Personal:
                handler.Send(text);
                break;

            case OutputRange.Local:
                if (location is not null && handler.Perceiver is not null)
                {
					location.HandleLocal(handler.Perceiver, handler.Perceiver.RoomLayer, text);
				}
                break;

            case OutputRange.Surrounds:
                foreach (
                    ICell room in location?.Surrounds ?? [])
                {
                    room.Handle(text);
                }

                break;

            case OutputRange.Room:
				if (location is not null)
				{
					HandleWholeLocation(location, text);
				}
                break;

            case OutputRange.Zone:
                foreach (ICell room in location?.Zone.Cells ?? [])
                {
                    room.Handle(text);
                }

                break;

            case OutputRange.Shard:
                foreach (ICell room in location?.Shard.Cells ?? [])
                {
                    room.Handle(text);
                }

                break;

            case OutputRange.All:
                foreach (ICharacter character in handler.Perceiver?.Gameworld.Characters ?? Enumerable.Empty<ICharacter>()
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

        bool newline = !output.Style.HasFlag(OutputStyle.NoNewLine);
        bool nopage = output.Style.HasFlag(OutputStyle.NoPage);

        ICell location = (handler.Perceiver as ICharacter)?.Corpse?.Parent.Location ??
                       handler.Perceiver?.Location ??
                       (handler.Perceiver as IGameItem)?.TrueLocations.FirstOrDefault() ??
					   (output as IEmoteOutput)?.DefaultSource?.Location; ;
        IEnumerable<ICell> vicinity = location?.CellsInVicinity(maxRange, false, false).Except(location) ?? [];
        foreach (ICell loc in vicinity)
        {
            loc.Handle(output);
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

        ICell location = (handler.Perceiver as ICharacter)?.Corpse?.Parent.Location ??
                       handler.Perceiver?.Location ??
                       (handler.Perceiver as IGameItem)?.TrueLocations.FirstOrDefault() ??
                       (output as IEmoteOutput)?.DefaultSource?.Location;
        bool newline = !output.Style.HasFlag(OutputStyle.NoNewLine);
        bool nopage = output.Style.HasFlag(OutputStyle.NoPage);

        switch (range)
        {
            case OutputRange.Personal:
                handler.Send(output, newline, nopage);
                break;

            case OutputRange.Local:
                IEnumerable<ICell> locations = (handler.Perceiver as IGameItem)?.TrueLocations ?? [location];
                foreach (ICell loc in locations)
                {
					if (loc is not null && handler.Perceiver is not null)
					{
						loc.HandleLocal(handler.Perceiver, handler.Perceiver.RoomLayer, output);
					}
                }

                break;

            case OutputRange.Surrounds:
                foreach (ICell loc in location?.Surrounds ?? [])
                {
                    loc.Handle(output);
                }

                break;

            case OutputRange.Room:
				if (location is not null)
				{
					HandleWholeLocation(location, output);
				}
                break;

            case OutputRange.Zone:
                foreach (ICell cell in location?.Zone.Cells ?? [])
                {
                    cell.Handle(output);
                }

                break;

            case OutputRange.Shard:
                foreach (ICell cell in location?.Shard.Cells ?? [])
                {
                    cell.Handle(output);
                }

                break;

            case OutputRange.All:
                foreach (ICharacter character in handler.Perceiver?.Gameworld.Characters ?? Enumerable.Empty<ICharacter>()
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

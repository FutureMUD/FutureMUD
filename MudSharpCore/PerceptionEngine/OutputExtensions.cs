using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine.Outputs;
using System;
using System.Collections.Generic;
using System.Linq;

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
        foreach (ICharacter ch in location?.Characters ?? [])
        {
            ch.OutputHandler?.Send(text);
        }

        foreach (IRemoteObservationEffect effect in location?.Cells.SelectMany(x => x.EffectsOfType<IRemoteObservationEffect>()).ToList() ??
                                   Enumerable.Empty<IRemoteObservationEffect>())
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
        foreach (ICharacter ch in location?.Characters ?? [])
        {
            ch.OutputHandler?.Send(output, !output.Style.HasFlag(OutputStyle.NoNewLine),
                !output.Style.HasFlag(OutputStyle.NoPage));
        }

        if (!output.Flags.HasFlag(OutputFlags.IgnoreWatchers))
        {
            foreach (IRemoteObservationEffect effect in location?.Cells.SelectMany(x => x.EffectsOfType<IRemoteObservationEffect>()).ToList() ??
                                   Enumerable.Empty<IRemoteObservationEffect>())
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
        foreach (ICharacter ch in location?.LayerCharacters(layer) ?? [])
        {
            ch.OutputHandler?.Send(text);
        }

        foreach (IRemoteObservationEffect effect in location?.Cells.SelectMany(x => x.EffectsOfType<IRemoteObservationEffect>()).ToList() ??
                                   Enumerable.Empty<IRemoteObservationEffect>())
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
        foreach (ICharacter ch in location?.LayerCharacters(layer) ?? [])
        {
            ch.OutputHandler?.Send(output, !output.Style.HasFlag(OutputStyle.NoNewLine),
                !output.Style.HasFlag(OutputStyle.NoPage));
        }

        if (!output.Flags.HasFlag(OutputFlags.IgnoreWatchers))
        {
            foreach (IRemoteObservationEffect effect in location?.EffectsOfType<IRemoteObservationEffect>().ToList() ??
                                   Enumerable.Empty<IRemoteObservationEffect>())
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
                location?.Handle(handler.Perceiver.RoomLayer, text);
                break;

            case OutputRange.Surrounds:
                foreach (
                    ICell room in location?.Surrounds ?? [])
                {
                    room.Handle(text);
                }

                break;

            case OutputRange.Room:
                location?.Handle(text);
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
                       (handler.Perceiver as IGameItem)?.TrueLocations.FirstOrDefault();
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
                       (handler.Perceiver as IGameItem)?.TrueLocations.FirstOrDefault();
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
                    loc.Handle(handler.Perceiver.RoomLayer, output);
                }

                break;

            case OutputRange.Surrounds:
                foreach (ICell loc in location?.Surrounds ?? [])
                {
                    loc.Handle(output);
                }

                break;

            case OutputRange.Room:
                location?.Handle(output);
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

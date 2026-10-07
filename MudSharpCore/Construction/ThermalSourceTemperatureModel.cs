#nullable enable
using MudSharp.GameItems;

namespace MudSharp.Construction;

internal static class ThermalSourceTemperatureModel
{
    internal static double AmbientMultiplier(RoomOutdoorsType outdoorsType)
    {
        return outdoorsType switch
        {
            RoomOutdoorsType.Indoors => 1.0,
            RoomOutdoorsType.IndoorsWithWindows => 1.0,
            RoomOutdoorsType.IndoorsNoLight => 1.0,
            RoomOutdoorsType.IndoorsClimateExposed => 0.5,
            _ => 0.0
        };
    }

    internal static IEnumerable<IGameItem> EnumerateThermalSourceItems(IRoom room, IPerceiver? voyeur = null)
    {
        if (room.RouteDefinition is null)
        {
            // Preserve the ordinary-room contract instead of requiring the newer aggregate
            // Perceivables projection from every IRoom implementation.
            return (room.GameItems ?? [])
                   .SelectMany(x => x.DeepItems)
                   .Concat((room.Characters ?? [])
                               .Where(x => x.Body is not null)
                               .SelectMany(x => x.Body.ExternalItems.SelectMany(y => y.DeepItems)))
                   .GroupBy(x => x.Id)
                   .Select(x => x.First());
        }

        // A RouteRoom thermal query without a valid observer coordinate cannot be scoped
        // safely. Fail closed instead of leaking heat across the whole linear room.
        if (voyeur?.Location != room || !voyeur.RoutePositionMetres.HasValue)
        {
            return [];
        }

        var maximumDistance = voyeur.Gameworld?.GetStaticDouble("RouteCellVeryDistantDistanceMetres") ?? 0.0;
        if (!double.IsFinite(maximumDistance) || maximumDistance <= 0.0)
        {
            maximumDistance = RouteSpatialConfiguration.Default.VeryDistantDistanceMetres;
        }

        var localPerceivables = RouteSpatialService.Instance.GetPerceivablesWithin(
            RouteSpatialService.Instance.GetEffectiveLocation(voyeur),
            maximumDistance,
            x => x.RoomLayer == voyeur.RoomLayer);

        return localPerceivables
                   .OfType<IGameItem>()
                   .SelectMany(x => x.DeepItems)
                   .Concat(localPerceivables
                               .OfType<ICharacter>()
                               .Where(x => x.Body is not null)
                               .SelectMany(x => x.Body.ExternalItems.SelectMany(y => y.DeepItems)))
                   .GroupBy(x => x.Id)
                   .Select(x => x.First());
    }

    internal static double AmbientHeatForRoom(IRoom room, RoomOutdoorsType outdoorsType, IPerceiver? voyeur = null)
    {
        return EnumerateThermalSourceItems(room, voyeur)
            .SelectMany(x => x.GetItemTypes<IProduceHeat>())
            .Sum(x => x.CurrentAmbientHeat) * AmbientMultiplier(outdoorsType);
    }

    internal static double ProximityHeatForTarget(IRoom room, IPerceiver? voyeur)
    {
        if (voyeur is null)
        {
            return 0.0;
        }

        return EnumerateThermalSourceItems(room, voyeur)
            .SelectMany(item => item.GetItemTypes<IProduceHeat>()
                                    .Select(component => component.CurrentHeat(voyeur.GetProximity(item))))
            .Sum();
    }
}

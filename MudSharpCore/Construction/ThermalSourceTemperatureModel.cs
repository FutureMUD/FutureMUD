#nullable enable
using MudSharp.GameItems;

namespace MudSharp.Construction;

internal static class ThermalSourceTemperatureModel
{
    internal static double AmbientMultiplier(CellOutdoorsType outdoorsType)
    {
        return outdoorsType switch
        {
            CellOutdoorsType.Indoors => 1.0,
            CellOutdoorsType.IndoorsWithWindows => 1.0,
            CellOutdoorsType.IndoorsNoLight => 1.0,
            CellOutdoorsType.IndoorsClimateExposed => 0.5,
            _ => 0.0
        };
    }

    internal static IEnumerable<IGameItem> EnumerateThermalSourceItems(ICell cell, IPerceiver? voyeur = null)
    {
        if (cell.RouteDefinition is null)
        {
            // Preserve the ordinary-cell contract instead of requiring the newer aggregate
            // Perceivables projection from every ICell implementation.
            return (cell.GameItems ?? [])
                   .SelectMany(x => x.DeepItems)
                   .Concat((cell.Characters ?? [])
                               .Where(x => x.Body is not null)
                               .SelectMany(x => x.Body.ExternalItems.SelectMany(y => y.DeepItems)))
                   .GroupBy(x => x.Id)
                   .Select(x => x.First());
        }

        // A RouteCell thermal query without a valid observer coordinate cannot be scoped
        // safely. Fail closed instead of leaking heat across the whole linear cell.
        if (voyeur?.Location != cell || !voyeur.RoutePositionMetres.HasValue)
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

    internal static double AmbientHeatForCell(ICell cell, CellOutdoorsType outdoorsType, IPerceiver? voyeur = null)
    {
        return EnumerateThermalSourceItems(cell, voyeur)
            .SelectMany(x => x.GetItemTypes<IProduceHeat>())
            .Sum(x => x.CurrentAmbientHeat) * AmbientMultiplier(outdoorsType);
    }

    internal static double ProximityHeatForTarget(ICell cell, IPerceiver? voyeur)
    {
        if (voyeur is null)
        {
            return 0.0;
        }

        return EnumerateThermalSourceItems(cell, voyeur)
            .SelectMany(item => item.GetItemTypes<IProduceHeat>()
                                    .Select(component => component.CurrentHeat(voyeur.GetProximity(item))))
            .Sum();
    }
}

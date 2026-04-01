#nullable enable
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

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

	internal static IEnumerable<IGameItem> EnumerateThermalSourceItems(ICell cell)
	{
		return cell.GameItems
		           .SelectMany(x => x.DeepItems)
		           .Concat(cell.Characters
		                       .Where(x => x.Body is not null)
		                       .SelectMany(x => x.Body.ExternalItems.SelectMany(y => y.DeepItems)))
		           .GroupBy(x => x.Id)
		           .Select(x => x.First());
	}

	internal static double AmbientHeatForCell(ICell cell, CellOutdoorsType outdoorsType)
	{
		return EnumerateThermalSourceItems(cell)
			.SelectMany(x => x.GetItemTypes<IProduceHeat>())
			.Sum(x => x.CurrentAmbientHeat) * AmbientMultiplier(outdoorsType);
	}

	internal static double ProximityHeatForTarget(ICell cell, IPerceiver? voyeur)
	{
		if (voyeur is null)
		{
			return 0.0;
		}

		return EnumerateThermalSourceItems(cell)
			.SelectMany(item => item.GetItemTypes<IProduceHeat>()
			                        .Select(component => component.CurrentHeat(voyeur.GetProximity(item))))
			.Sum();
	}
}

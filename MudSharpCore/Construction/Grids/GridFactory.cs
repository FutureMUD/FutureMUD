using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Construction.Grids;

public static class GridFactory
{
	public static IGrid LoadGrid(Models.Grid grid, IFuturemud gameworld)
	{
		switch (grid.GridType)
		{
			case "Electrical":
				return new ElectricalGrid(grid, gameworld);
		}

		throw new NotImplementedException("Unimplemented grid type in GridFactory.LoadGrid");
	}
}
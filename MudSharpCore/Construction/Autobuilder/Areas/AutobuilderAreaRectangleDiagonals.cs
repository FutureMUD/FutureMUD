using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Database;
using MudSharp.Framework;

namespace MudSharp.Construction.Autobuilder.Areas;

public class AutobuilderAreaRectangleDiagonals : AutobuilderAreaRectangle
{
	public new static void RegisterAutobuilderLoader()
	{
		AutobuilderFactory.RegisterLoader("rectangle diagonals",
			(area, gameworld) => new AutobuilderAreaRectangleDiagonals(area, gameworld));
		AutobuilderFactory.RegisterBuilderLoader("rectangle diagonals",
			(gameworld, name) => new AutobuilderAreaRectangleDiagonals(name, gameworld));
	}

	protected AutobuilderAreaRectangleDiagonals(Models.AutobuilderAreaTemplate area, IFuturemud gameworld) : base(area,
		gameworld)
	{
	}

	protected AutobuilderAreaRectangleDiagonals(string name, IFuturemud gameworld, string type = null) : base(name,
		gameworld, type ?? "rectangle")
	{
	}

	#region Overrides of AutobuilderAreaRectangle

	public override bool ConnectCellsWithDiagonalExits => true;


	public override IAutobuilderArea Clone(string newName)
	{
		using (new FMDB())
		{
			var dbitem = new Models.AutobuilderAreaTemplate
			{
				Name = newName,
				Definition = SaveToXml().ToString(),
				TemplateType = "rectangle diagonals"
			};
			FMDB.Context.AutobuilderAreaTemplates.Add(dbitem);
			FMDB.Context.SaveChanges();
			return new AutobuilderAreaRectangleDiagonals(dbitem, Gameworld);
		}
	}

	#endregion
}
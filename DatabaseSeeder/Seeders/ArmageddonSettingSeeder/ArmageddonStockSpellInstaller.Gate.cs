#nullable enable

using System.Linq;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Magic;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public static partial class ArmageddonStockSpellInstaller
{
	private static ArmageddonCanonicalSpellContribution ResolveGate(FuturemudDatabaseContext db)
	{
		var templateKey = ArmageddonStockSupportInstaller.NilazPassageTemplateKey;
		var templateClaim = CatalogueClaim<AutobuilderRoomTemplate>(db, templateKey, false);
		var template = db.AutobuilderRoomTemplates.AsNoTracking()
			.SingleOrDefault(x => x.Id == templateClaim.LogicalId);
		if (template is null || template.TemplateType != "room by terrain")
			throw new StockDependencyException("Linked Threshold requires its exact owned Nilaz room presentation.");
		try
		{
			var root = XElement.Parse(template.Definition);
			if (root.Name != "Template" || root.Element("Default")?.Element("Terrain") is not { } terrain ||
				(long?)terrain.Element("DefaultTerrain") is not > 0 ||
				(int?)terrain.Element("OutdoorsType") is not ((int)RoomOutdoorsType.Indoors or (int)RoomOutdoorsType.IndoorsNoLight))
				throw new StockDependencyException("The owned Nilaz presentation is no longer a native indoor corridor.");
		}
		catch (System.Xml.XmlException)
		{
			throw new StockDependencyException("The owned Nilaz presentation XML is unreadable.");
		}
		var undeadKey = ArmageddonStockSupportInstaller.UndeadMeritKey;
		var undeadClaim = CatalogueClaim<Merit>(db, undeadKey, false);
		if (!db.Merits.AsNoTracking().Any(x => x.Id == undeadClaim.LogicalId && x.Type == "Simple Character"))
			throw new StockDependencyException("Linked Threshold requires the exact owned undead marker merit.");
		return ArmageddonGateSpellContent.Create(template.Id, undeadClaim.LogicalId!.Value);
	}
}

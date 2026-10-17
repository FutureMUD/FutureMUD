#nullable enable

using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.Magic;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public static partial class ArmageddonStockSpellInstaller
{
	private static ArmageddonCanonicalSpellContribution ResolvePocket(FuturemudDatabaseContext db)
	{
		var claim = CatalogueClaim<GameItemProto>(db, "arm.item.folded_pocket", true);
		var prototype = db.GameItemProtos.AsNoTracking().Include(x => x.EditableItem)
			.Include(x => x.GameItemProtosGameItemComponentProtos).ThenInclude(x => x.GameItemComponent).ThenInclude(x => x.EditableItem)
			.Include(x => x.GameItemProtosOnLoadProgs)
			.SingleOrDefault(x => x.Id == claim.LogicalId && x.RevisionNumber == claim.RevisionNumber);
		if (prototype?.EditableItem?.RevisionStatus != (int)RevisionStatus.Current ||
			prototype.MorphGameItemProtoId is not null || prototype.MorphTimeSeconds > 0 || prototype.GameItemProtosOnLoadProgs.Any() ||
			prototype.GameItemProtosGameItemComponentProtos.Count != 2)
			throw new StockDependencyException("Folded Pocket requires its exact approved unscripted owned carrier prototype.");
		var holdable = CatalogueClaim<GameItemComponentProto>(db, "arm.catalogue.component.holdable", true);
		var pocket = CatalogueClaim<GameItemComponentProto>(db, "arm.item.folded_pocket.component", true);
		foreach (var expected in new[] { (Claim: holdable, Type: "Holdable"), (Claim: pocket, Type: "FoldedPocket") })
		{
			var links = prototype.GameItemProtosGameItemComponentProtos.Where(x => x.GameItemComponentProtoId == expected.Claim.LogicalId &&
				x.GameItemComponentRevision == expected.Claim.RevisionNumber).ToArray();
			if (links.Length != 1 || links[0].GameItemComponent.Type != expected.Type ||
				links[0].GameItemComponent.EditableItem?.RevisionStatus != (int)RevisionStatus.Current)
				throw new StockDependencyException("Folded Pocket requires the exact owned current Holdable/FoldedPocket component links.");
		}
		var settings = ArmageddonInstallation.Read(db)!;
		var kilograms = db.UnitsOfMeasure.Single(x => x.Id == settings.NativeBindings[ArmageddonNativeBinding.Kilogram]).BaseMultiplier;
		if (!double.IsFinite(kilograms) || kilograms <= 0 || !double.IsFinite(14 * kilograms))
			throw new StockDependencyException("Folded Pocket requires its fixed finite native kilogram conversion.");
		var component = prototype.GameItemProtosGameItemComponentProtos.Single(x => x.GameItemComponentProtoId == pocket.LogicalId).GameItemComponent;
		XElement definition;
		try { definition = XElement.Parse(component.Definition); }
		catch (System.Xml.XmlException) { throw new StockDependencyException("Folded Pocket carrier XML is unreadable."); }
		if (definition.Name != "Definition" || definition.DescendantsAndSelf().Any(x => x.Name.Namespace != XNamespace.None) ||
			!double.TryParse((string?)definition.Attribute("Weight"), NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var capacity) ||
			!double.IsFinite(capacity) || capacity < 14 * kilograms ||
			!int.TryParse((string?)definition.Attribute("MaxSize"), out var size) || size < (int)SizeCategory.VeryLarge || !Enum.IsDefined((SizeCategory)size) ||
			definition.Elements("AllowedTags").Count() != 1 || definition.Element("AllowedTags")!.HasElements ||
			definition.Elements("BlockedTags").Count() != 1 || definition.Element("BlockedTags")!.HasElements ||
			!bool.TryParse((string?)definition.Attribute("Closable"), out var closable) || closable ||
			!bool.TryParse((string?)definition.Attribute("OnceOnly"), out var onceOnly) || onceOnly)
			throw new StockDependencyException("Folded Pocket needs its open unfiltered carrier supporting the full14-kilogram grade-seven capacity.");
		var configuration = new SpellPocketConfiguration(prototype.Id, 2 * kilograms, SizeCategory.VeryLarge, 300,
			SpellPocketAccess.Bearer, ResolveFallback(db));
		return new("arm.capability.void", ArmageddonPocketContent.Create(configuration));
	}
}

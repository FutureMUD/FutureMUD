#nullable enable

using MudSharp.Database;
using MudSharp.Models;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	internal CommoditySpoilageRule InstallCatalogueSpoilageForTesting(FuturemudDatabaseContext context, bool flush = true)
	{
		_context = context;
		InitialiseDependencies();
		using var module = UseManifestModule("antiquity", "antiquity");
		EnsureCommoditySpoilageRule("Fixture spoilage", "Fixture stock spoilage rule", _materials["fish"], null, null,
			_materials["oak"], null, System.TimeSpan.FromHours(2), "Fixture spoilage echo");
		if (flush) SaveManifestChanges();
		return context.CommoditySpoilageRules.Local.Single();
	}

	internal GameItemProto InstallCatalogueItemForTesting(FuturemudDatabaseContext context, CatalogueItemDefinition definition)
	{
		_context = context;
		InitialiseDependencies();
		return InstallCatalogueItem(definition)!;
	}

	internal GameItemProto InstallIndustrialCatalogueItemForTesting(FuturemudDatabaseContext context,
		CatalogueItemDefinition definition, bool clothingRequiresSkins)
	{
		_context = context;
		InitialiseDependencies();
		return InstallIndustrialCatalogueItem(definition, definition.Components, clothingRequiresSkins)!;
	}

	internal GameItemProto InstallCatalogueFoodForTesting(FuturemudDatabaseContext context)
	{
		_context = context;
		InitialiseDependencies();
		var entry = ToLegacyFood(RepresentativeCatalogue.Pottage());
		RefreshPreIndustrialFoodIdentifierAllocators();
		foreach (var tag in BuildFoodItemTags(entry)) EnsurePreIndustrialFoodTagPath(tag);
		UpsertPreIndustrialFoodItem(entry);
		return _itemsByStableReference[entry.StableReference];
	}

	internal Craft InstallCatalogueCraftForTesting(FuturemudDatabaseContext context, CatalogueCraftDefinition definition)
	{
		InitialiseCraftAuthoringForTesting(context);
		return InstallCatalogueCraft(definition);
	}

	internal object RepresentativeVehicleForTesting(bool unified) => unified ? RepresentativeHandcart() :
		VehicleExampleSpecs.Single(x => x.StableReference == "vehicle_antiquity_two_wheeled_handcart");

	internal void InstallRepresentativeVehicleForTesting(FuturemudDatabaseContext context)
	{
		_context = context;
		InitialiseDependencies();
		EnsureVehicleSeederTags();
		var spec = RepresentativeHandcart();
		ValidateVehicleSeedSpec(spec);
		UpsertVehiclePrototype(spec);
	}

	internal object RepresentativeOutfitForTesting(bool unified) => unified ? ToLegacyOutfit(RepresentativeCatalogue.EarlyAngloSaxon()) :
		MedievalOutfitManifestSpecs.Single(x => x.StableKey == "medieval_outfit_0001");
}

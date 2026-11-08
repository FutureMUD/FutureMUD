#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Database;

namespace DatabaseSeeder.Seeders;

internal static class ItemPackageMetadata
{
	private static IEnumerable<SeederPrerequisite> Components(string provider, params string[] names) =>
		names.Select(name => Requirement($"Missing item component {name}. Install {provider} in Database Seeder, or supply its equivalent.",
			context => context.GameItemComponentProtos.Any(component => component.Name == name)));

	internal static SeederMetadata Items { get; } = new(
		SeederRepeatabilityMode.Idempotent, SeederUpdateCapability.FullReconcile,
		[
			Requirement("Missing Functions tag. Install Core in Database Seeder, or supply its equivalent.", context => context.Tags.Any(tag => tag.Name == "Functions")),
			.. Components("Kickstart (Standard Item Components)",
				"Container_Table",
				"Container_Armor_Stand",
				"Container_Weapon_Rack",
				"Container_Cot_Surface",
				"Container_Bed_Surface",
				"Container_Couch_Surface",
				"Container_Counter",
				"Container_Bench_Surface",
				"Container_Desk_Surface",
				"Container_Wide_Shelves",
				"Container_Open_Bin",
				"Container_Trunk",
				"TimePiece_Antiquity_Sundial",
				"WaterSource_Antiquity_PublicWell",
				"Dice_Antiquity_Knucklebones",
				"DragAid_Antiquity_FieldStretcher",
				"Locksmithing_Antiquity_BronzePoor",
				"ShopStall_Antiquity_OpenCounter",
				"MarketGoodWeight_Antiquity_StapleFood",
				"SealStamp_Antiquity_BronzeSignet",
				"Sealable_Envelope",
				"MeasuringInstrument_Antiquity_BalanceScale",
				"Insulation_Minor",
				"Destroyable_Misc",
				"Torch_Infinite"),
			.. Components("Animal Seeder", "Wear_Saddle", "Wear_Bridle", "Wear_Chanfron", "Wear_Criniere",
				"Wear_Croupiere", "Wear_Flanchards", "Wear_Peytral", "Wear_Caparison")
		],
		RerunSummary: "Reruns reconcile the installed ItemSeeder eras and can add newly selected implemented eras without removing prior content.",
		UpdateSummary: "Manifest-owned stock aggregates are repaired when untouched; builder-customized aggregates are preserved and reported.",
		OwnershipSummary: "Durable SeederManagedRecords provenance owns stock aggregate definitions and required stock links. Builder additions and customized aggregate graphs are retained; ambiguous untracked identities block before mutation.");

	internal static SeederMetadata PrimaryProduction { get; } = new(
		SeederRepeatabilityMode.Idempotent,
		SeederUpdateCapability.RepairExisting,
		[
			Requirement("The Core seeder must have created at least one account.", context => context.Accounts.Any()),
			Requirement("Core utility progs must include AlwaysTrue.", context => context.FutureProgs.Any(x => x.FunctionName == "AlwaysTrue")),
			Requirement("Primary production tags and materials must already exist.", context =>
				context.Tags.Any(x => x.Name == "Primary Production Commodity") &&
				context.Tags.Any(x => x.Name == "Visible Resource Deposit") &&
				context.Materials.Any(x => x.Name == "hematite") &&
				context.Materials.Any(x => x.Name == "charcoal")),
			Requirement("The Item seeder must have installed primary-production visible resource props.", context =>
				context.GameItemProtos.Any(x => x.UniqueName == "primary_production_hematite_deposit") &&
				context.GameItemProtos.Any(x => x.UniqueName == "primary_production_bloomery_furnace")),
			Requirement("Required stock traits must already exist.", context =>
				HasTrait(context, "Labouring", "Labourer", "Laboring", "Laborer") &&
				HasTrait(context, "Masonry", "Stonecraft", "Stoneworking", "Mason") &&
				HasTrait(context, "Smelting", "Smelter"))
		],
		RerunSummary: "Reruns reuse stock primary-production local project templates by deterministic names.",
		UpdateSummary: "Reruns refresh stock project definitions, labour, material requirements, and resource/commodity actions without duplicating templates.",
		OwnershipSummary: "Stock primary-production project content is tracked by the Stock Primary Production project-name prefix.",
		DependencySeederTypes: [typeof(ItemSeeder)]
	);

	private static SeederPrerequisite Requirement(string description, Func<FuturemudDatabaseContext, bool> predicate)
	{
		return new SeederPrerequisite(description, predicate);
	}

	private static bool HasTrait(FuturemudDatabaseContext context, params string[] aliases)
	{
		return context.TraitDefinitions
			.AsEnumerable()
			.Any(x => aliases.Any(alias => NormaliseName(x.Name) == NormaliseName(alias)));
	}

	private static string NormaliseName(string text)
	{
		return new string(text.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();
	}
}

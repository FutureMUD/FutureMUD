#nullable enable
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using MudSharp.Framework;
using MudSharp.GameItems.Prototypes;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Resources;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static void PreparedRuntimeCapacity(NativeRuntime native, TestDatabase database, ArmageddonPreparedWorldBindings bindings)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		SetPrivateField(native.Body, "_traits", db.Traits.AsNoTracking().Where(x => x.BodyId == native.Body.Id).ToArray()
			.Select(x => CastingRequired(native.World.Traits.Get(x.TraitDefinitionId)).LoadTrait(x, native.Body)).ToList());
		var old = native.Resource; var resource = new SimpleMagicResource(db.MagicResources.AsNoTracking().Single(x => x.Id == old.Id), native.World);
		var amounts = (DoubleCounter<IMagicResource>)native.Actor.MagicResourceAmounts; var balance = amounts[old];
		amounts.Remove(old); amounts[resource] = balance; var catalogue = (All<IMagicResource>)native.World.MagicResources;
		catalogue.Remove(old); catalogue.Add(resource); SetPrivateMember(native, "Resource", resource);
		var raw = native.Actor.TraitRawValue(native.World.Traits.Get(bindings.CapacityAttribute));
		Require(raw > 0 && resource.TryGetResourceCap(native.Actor, out var cap, out var error) && cap == raw * 10 && amounts[resource] == balance,
			"Prepared native capacity did not resolve persisted body attribute/expression or refilled balance.");
		foreach (var variant in ArmageddonTraditionInstaller.Variants)
		{
			var capability = (SkillLevelBasedMagicCapability)native.World.MagicCapabilities.GetByName("Armageddon partial " + variant);
			capability.BuildingCommand(native.Actor, new StringStack("casting validate")); Require(capability.CastingConfigurationErrors().Count == 0, "Guide casting validate native errors.");
		}
		var spell = (MagicSpell)native.World.MagicSpells.Get(db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonReviewedUtilityContent.SenseEnchantmentKey).LogicalId!.Value);
		spell.BuildingCommand(native.Actor, new StringStack("grades show"));
		// Both stock device definitions are read and validated using their actual builder API.
		foreach (var record in db.SeederManagedRecords.AsNoTracking().Where(x => x.EntityType == "GameItemComponentProto" && x.Module == ArmageddonMagicInstaller.Module).ToArray())
		{
			var device = new ChargedMagicDeviceGameItemComponentProto(db.GameItemComponentProtos.Include(x => x.EditableItem).Single(x => x.Id == record.LogicalId && x.RevisionNumber == record.RevisionNumber), native.World);
			Require(device.BuildingCommand(native.Actor, new StringStack("checkconfig")) && device.CanSubmit(), "Guide device checkconfig native errors.");
		}
		Console.WriteLine($"ARMPREP-guide-native=passed selected-body-attribute:{bindings.CapacityAttribute} raw:{raw} reserve-cap:{raw * 10} preserved-balance:{balance} casting-validate grades-show device-checkconfig actual-builder-APIs");
	}
}

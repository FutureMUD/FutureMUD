#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ModernFirearmSeederTests
{
	[TestMethod]
	public void CombatSeederSource_ModernFirearmSamples_AreRerunnableAndBounded()
	{
		var source = SeederSourceTestHelper.ReadPartialFamily("CombatSeeder");

		StringAssert.Contains(source, "EnsureModernFirearmSamples(context)");
		StringAssert.Contains(source, "\"Shotgun_12_Gauge_Pump\"");
		StringAssert.Contains(source, "\"Rifle_556_Select_Fire\"");
		StringAssert.Contains(source, "\"Pistol_9mm_Service\"");
		StringAssert.Contains(source, "\"SMG_9mm_Select_Fire\"");
		StringAssert.Contains(source, "\"Rifle_762_Precision\"");
		StringAssert.Contains(source, "\"GPMG_762_Belt_Fed\"");
		StringAssert.Contains(source, "\"Launcher_Anti_Armour_Shoulder\"");
		StringAssert.Contains(source, "\"12 Gauge Slug\"");
		StringAssert.Contains(source, "\"12 Gauge 00 Buckshot\"");
		StringAssert.Contains(source, "\"5.56x45mm Ball\"");
		StringAssert.Contains(source, "\"40mm Low-Velocity Grenade\"");
		StringAssert.Contains(source, "AmmunitionLoadType.Magazine");
		StringAssert.Contains(source, "RangedScatterType.Spread");
		StringAssert.Contains(source, "RangedScatterType.Arcing");
		StringAssert.Contains(source, "DamageType.BallisticArmourPiercing");
		StringAssert.Contains(source, "new XAttribute(\"rounds\", 6)");
		StringAssert.Contains(source, "\"Sound Suppressor 556\"");
		StringAssert.Contains(source, "\"Reflex Optic\"");
		StringAssert.Contains(source, "\"Low Power Variable Optic\"");
		StringAssert.Contains(source, "\"Precision Scope\"");
		StringAssert.Contains(source, "\"Rifle Bipod\"");
		StringAssert.Contains(source, "\"Machine Gun Tripod\"");
		StringAssert.Contains(source, "\"Underbarrel Launcher Mount\"");
		StringAssert.Contains(source,
			"EnsureAttachment(\"Weapon Light Mount\", FirearmAttachmentSlotType.Underbarrel");
		StringAssert.Contains(source, "requiredCapabilities: [typeof(IProduceLight), typeof(IProducePower)]");
		StringAssert.Contains(source, "requiredCapabilities: [typeof(IRangedWeapon)]");
		StringAssert.Contains(source, "requiredCapabilities: [typeof(IMeleeWeapon)]");
		StringAssert.Contains(source, "context.WeaponTypes.First(x => x.Name == \"Improvised Bludgeon\")");
		StringAssert.Contains(source, "\"Melee_Modern_Bayonet\"");
		StringAssert.Contains(source, "\"Launcher_40mm_Underbarrel\"");
		StringAssert.Contains(source, "\"ElectricLight_WeaponMounted\"");
		StringAssert.Contains(source, "\"BatteryPowered_WeaponLight\"");
		StringAssert.Contains(source, "\"Bomb_40mm_Impact_Grenade\"");
		StringAssert.Contains(source, "\"ImpactDetonator_Fired_Grenade\"");
		StringAssert.Contains(source, "\"Ammunition_40mm_Low_Velocity_Grenade\"");
		StringAssert.Contains(source, "\"ModernFirearms_Bayonet_Example\"");
		StringAssert.Contains(source, "\"ModernFirearms_Underbarrel_Launcher_Example\"");
		StringAssert.Contains(source, "\"ModernFirearms_Weapon_Light_Example\"");
		StringAssert.Contains(source, "\"ModernFirearms_40mm_Grenade_Round_Example\"");
		StringAssert.Contains(source, "\"ModernFirearms_Fragmentation_Grenade\"");
		StringAssert.Contains(source, "\"ModernFirearms_Plastic_Explosive_Charge\"");
		StringAssert.Contains(source, "\"ModernFirearms_Directional_Mine\"");
		StringAssert.Contains(source, "EnsureItemTag(directionalMine, tripwireTriggerTag)");
		StringAssert.Contains(source, "EnsureItemTag(antiPersonnelMine, tripwireTriggerTag)");
		StringAssert.Contains(source, "\"ModernArtillery_{key}_Piece\"");
		StringAssert.Contains(source, "EnsureModernArtillery(\"Howitzer_105mm\"");
		StringAssert.Contains(source, "ArtilleryLoadingMechanism.BreechLoading");
		StringAssert.Contains(source, "ArtilleryLoadingMechanism.DropFireMortar");
		StringAssert.Contains(source, "type.BreakChanceOnHit = 0.0");
		StringAssert.Contains(source, "type.BreakChanceOnMiss = 0.0");
		StringAssert.Contains(source, "case CheckType.AimArtillery:");
		StringAssert.Contains(source, "case CheckType.LoadArtillery:");
		StringAssert.Contains(source, "case CheckType.FireArtillery:");
		StringAssert.Contains(source, "\"ModernFirearms_Soft_Ballistic_Vest\"");
		StringAssert.Contains(source, "\"ModernFirearms_Hard_Plate_Carrier\"");
		StringAssert.Contains(source, "GameItemProtosGameItemComponentProtos.Any");
		StringAssert.Contains(source, "FirstOrDefault(x => x.Type == type && x.Name == name)");
	}
}

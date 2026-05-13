#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace MudSharp_Unit_Tests;

[TestClass]
public class WeaponPoisonSourceIntegrationTests
{
	[TestMethod]
	public void AmmunitionAndProjectilePaths_CopyAndDeliverPoisonCoatings()
	{
		var ammunition = File.ReadAllText(GetSourcePath("MudSharpCore", "GameItems", "Components",
			"AmmunitionGameItemComponent.cs"));
		var firearm = File.ReadAllText(GetSourcePath("MudSharpCore", "GameItems", "Components",
			"FirearmBaseGameItemComponent.cs"));
		var musket = File.ReadAllText(GetSourcePath("MudSharpCore", "GameItems", "Components",
			"MusketGameItemComponent.cs"));

		StringAssert.Contains(ammunition, "WeaponPoisonDeliveryHelper.DeliverFromWeapon(actor, ammo, wounds, false);");
		StringAssert.Contains(firearm, "WeaponPoisonDeliveryHelper.CopyPoisonCoating(ammo.Parent, bullet);");
		StringAssert.Contains(musket, "WeaponPoisonDeliveryHelper.CopyPoisonCoating(ball, bullet);");
	}

	[TestMethod]
	public void StackableAmmoSplits_TransferPoisonVolumeWithoutDuplicating()
	{
		var stackable = File.ReadAllText(GetSourcePath("MudSharpCore", "GameItems", "Components",
			"StackableGameItemComponent.cs"));

		StringAssert.Contains(stackable, "TransferWeaponPoisonCoatingToSplit(newItem, quantity);");
		StringAssert.Contains(stackable, "coating.RemovePoisonVolume(amount)");
		StringAssert.Contains(stackable, "newItem.RemoveEffect(copiedCoating, true)");
	}

	[TestMethod]
	public void ManipulationModule_ExposesApplyAndDipPoisonRoutes()
	{
		var manipulation = File.ReadAllText(GetSourcePath("MudSharpCore", "Commands", "Modules",
			"ManipulationModule.cs"));

		StringAssert.Contains(manipulation, "[PlayerCommand(\"Dip\", \"dip\")]");
		StringAssert.Contains(manipulation, "ApplyPoisonToWeapon(character, item, ss);");
		StringAssert.Contains(manipulation, "CheckType.ApplyPoisonToWeapon");
		StringAssert.Contains(manipulation, "HandleWeaponPoisonMishap");
		StringAssert.Contains(manipulation, "Only held melee weapons and ammunition can be poisoned in this way.");
	}

	[TestMethod]
	public void GenericSpillRoute_DoesNotCreateCombatPoisonCoating()
	{
		var manipulation = File.ReadAllText(GetSourcePath("MudSharpCore", "Commands", "Modules",
			"ManipulationModule.cs"));
		var spillStart = manipulation.IndexOf("protected static void Spill", StringComparison.Ordinal);
		var smokeStart = manipulation.IndexOf("[PlayerCommand(\"Smoke\", \"smoke\")]", StringComparison.Ordinal);
		Assert.IsTrue(spillStart > 0);
		Assert.IsTrue(smokeStart > spillStart);

		var spillMethod = manipulation[spillStart..smokeStart];
		StringAssert.Contains(spillMethod, "target.ExposeToLiquid");
		Assert.IsFalse(spillMethod.Contains("WeaponPoisonCoating", StringComparison.Ordinal));
		Assert.IsFalse(spillMethod.Contains("WeaponPoisonDeliveryHelper", StringComparison.Ordinal));
	}

	private static string GetSourcePath(params string[] parts)
	{
		return Path.GetFullPath(Path.Combine(
			AppContext.BaseDirectory,
			"..",
			"..",
			"..",
			"..",
			Path.Combine(parts)));
	}
}

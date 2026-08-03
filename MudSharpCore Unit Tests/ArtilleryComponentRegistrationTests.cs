#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.GameItems;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArtilleryComponentRegistrationTests
{
	[TestMethod]
	public void GameItemComponentManager_RegistersArtilleryAndWeaponCarrierTypes()
	{
		var manager = new GameItemComponentManager();
		var primaryTypes = manager.PrimaryTypes.ToList();
		var helpTypes = manager.TypeHelpInfo.Select(x => x.Name).ToList();

		CollectionAssert.Contains(primaryTypes, "artillery");
		CollectionAssert.Contains(primaryTypes, "artilleryammo");
		CollectionAssert.Contains(primaryTypes, "artillerymount");
		CollectionAssert.Contains(primaryTypes, "artillerychamber");
		CollectionAssert.Contains(primaryTypes, "weaponcarrier");
		CollectionAssert.Contains(helpTypes, "ArtilleryPiece");
		CollectionAssert.Contains(helpTypes, "ArtilleryAmmunition");
		CollectionAssert.Contains(helpTypes, "ArtilleryMount");
		CollectionAssert.Contains(helpTypes, "ArtilleryChamber");
		CollectionAssert.Contains(helpTypes, "WeaponCarrierAttachment");
	}
}

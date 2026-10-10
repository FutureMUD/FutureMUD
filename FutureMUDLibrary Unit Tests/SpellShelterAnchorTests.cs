#nullable enable

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SpellShelterAnchorTests
{
	[DataTestMethod]
	[DataRow(SpellShelterKind.SpringHaven)]
	[DataRow(SpellShelterKind.BurrowRefuge)]
	[DataRow(SpellShelterKind.SandShelter)]
	public void TypedAnchor_RoundTrips64BitIdsAndRouteCoordinate(SpellShelterKind kind)
	{
		var anchor = new SpellShelterAnchor(kind, long.MaxValue - 1, long.MaxValue, 123.456789, 128);
		Assert.AreEqual(anchor, SpellShelterAnchor.Load(anchor.Save()));
		var ordinary = anchor with { AnchorRoutePosition = null };
		Assert.AreEqual(ordinary, SpellShelterAnchor.Load(ordinary.Save()));
	}

	[DataTestMethod]
	[DataRow("<Shelter version='2' />")]
	[DataRow("<Definition><Room>9</Room></Definition>")]
	[DataRow("<Shelter version='1'><Kind>SandShelter</Kind><AnchorRoom>1</AnchorRoom><AnchorRoom>2</AnchorRoom><FallbackRoom>3</FallbackRoom><MaximumOccupants>1</MaximumOccupants></Shelter>")]
	[DataRow("<Shelter version='1'><Kind>SandShelter</Kind><AnchorRoom>1</AnchorRoom><FallbackRoom>3</FallbackRoom><MaximumOccupants>1</MaximumOccupants><GuessedRoomId>999</GuessedRoomId></Shelter>")]
	public void TypedAnchor_RejectsForeignAndAmbiguousSchemas(string definition)
	{
		Assert.ThrowsException<FormatException>(() => SpellShelterAnchor.Load(definition));
	}

	[TestMethod]
	public void TypedAnchor_InvalidFallbackCapacityOrCoordinateCannotPersist()
	{
		var valid = new SpellShelterAnchor(SpellShelterKind.BurrowRefuge, 1, 2, null, 8);
		foreach (var invalid in new[] { valid with { AnchorRoomId = 0 }, valid with { FallbackRoomId = -1 },
			valid with { MaximumOccupants = 0 }, valid with { MaximumOccupants = 129 },
			valid with { AnchorRoutePosition = double.NaN }, valid with { AnchorRoutePosition = double.PositiveInfinity },
			valid with { AnchorRoutePosition = -1 }, valid with { Kind = (SpellShelterKind)99 } })
			Assert.ThrowsException<ArgumentException>(() => invalid.Save());
	}
}

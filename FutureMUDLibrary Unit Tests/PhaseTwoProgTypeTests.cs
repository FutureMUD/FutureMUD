#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Communication;
using MudSharp.Economy;
using MudSharp.Economy.Property;
using MudSharp.FutureProg;

namespace FutureMUDLibrary_Unit_Tests.FutureProg;

[TestClass]
public class PhaseTwoProgTypeTests
{
	[TestMethod]
	public void PhaseTwoTypes_ParseRoundTripAndParticipateInReferenceCollections()
	{
		foreach (var (type, token, kind) in PhaseTwoTypes)
		{
			Assert.IsTrue(ProgVariableTypeRegistry.TryParse(token, out var parsed), token);
			Assert.AreEqual(type, parsed, token);
			Assert.IsTrue(ProgVariableTypeRegistry.TryParse(type.ToStorageString(), out var roundTripped), token);
			Assert.AreEqual(type, roundTripped, token);
			Assert.AreEqual(kind, type.ExactKind, token);
			Assert.IsTrue(ProgVariableTypes.CollectionItem.HasFlag(type), token);
			Assert.IsTrue(ProgVariableTypes.ReferenceType.HasFlag(type), token);
			Assert.IsTrue(ProgVariableTypes.Anything.HasFlag(type), token);
		}
	}

	[TestMethod]
	public void PhaseTwoReferenceInterfaces_AreProgVariables()
	{
		foreach (var type in new[]
		         {
			         typeof(IProperty),
			         typeof(IPropertyKey),
			         typeof(IPropertyLease),
			         typeof(IPropertyLeaseOrder),
			         typeof(IPropertySaleOrder),
			         typeof(IEconomicZone),
			         typeof(IChannel)
		         })
		{
			Assert.IsTrue(typeof(IProgVariable).IsAssignableFrom(type), type.Name);
		}
	}

	private static readonly (ProgVariableTypes Type, string Token, ProgTypeKind Kind)[] PhaseTwoTypes =
	[
		(ProgVariableTypes.Property, "property", ProgTypeKind.Property),
		(ProgVariableTypes.PropertyKey, "propertykey", ProgTypeKind.PropertyKey),
		(ProgVariableTypes.PropertyLease, "propertylease", ProgTypeKind.PropertyLease),
		(ProgVariableTypes.PropertyLeaseOrder, "propertyleaseorder", ProgTypeKind.PropertyLeaseOrder),
		(ProgVariableTypes.PropertySaleOrder, "propertysaleorder", ProgTypeKind.PropertySaleOrder),
		(ProgVariableTypes.EconomicZone, "economiczone", ProgTypeKind.EconomicZone),
		(ProgVariableTypes.Channel, "channel", ProgTypeKind.Channel)
	];
}

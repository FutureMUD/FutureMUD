#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Body.Implementations;
using System;
using System.Linq;
using System.Reflection;

namespace MudSharp_Unit_Tests;

[TestClass]
public class BodyInventoryInitialisationTests
{
	[TestMethod]
	public void RecalculateItemHelpers_EmptyBodyWithoutLoadedInventory_InitialisesPublicInventoryViews()
	{
		var body = TestObjectFactory.CreateUninitialized<Body>();
		foreach (var fieldName in new[]
		         {
			         "_prosthetics",
			         "_wounds",
			         "_severedRoots",
			         "_wieldedItems",
			         "_heldItems",
			         "_wornItems",
			         "_implants",
			         "_bodyparts",
			         "_organs",
			         "_bones"
		         })
		{
			SetNewCollection(body, fieldName);
		}

		body.RecalculateItemHelpers();

		Assert.IsFalse(body.InventoryLoaded);
		Assert.AreEqual(0, body.WornItems.Count());
		Assert.AreEqual(0, body.DirectWornItems.Count());
		Assert.AreEqual(0, body.Outerwear.Count());
		Assert.AreEqual(0, body.ExposedItems.Count());
		Assert.AreEqual(0, body.ExternalItems.Count());
		Assert.AreEqual(0, body.ExternalItemsForOtherActors.Count());
		Assert.AreEqual(0, body.DirectItems.Count());
		Assert.AreEqual(0, body.CarriedItems.Count());
		Assert.AreEqual(0, body.ExposedBodyparts.Count());
		Assert.AreEqual(0, body.VisiblySeveredBodyparts.Count());
		Assert.AreEqual(0, body.ItemsWornAgainstSkin.Count());
	}

	private static void SetNewCollection(Body body, string fieldName)
	{
		var field = typeof(Body).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.IsNotNull(field, $"Could not find Body field {fieldName}.");
		field.SetValue(body, Activator.CreateInstance(field.FieldType));
	}
}

#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Models;
using CultureInfo = System.Globalization.CultureInfo;

namespace MudSharp_Unit_Tests;

[TestClass]
public class IndustrialisedClothingPhysicalBindingsTests
{
	private static readonly ClothingSourceLocation Source = new("Clothing/bases.tsv", 27);
	private const string Profiles = "<Profiles Default=\"51\"><Profile>51</Profile></Profiles>";
	private static GameItemComponentProto Component(long id, string type, string xml = "<Definition/>") =>
		new() { Id = id, RevisionNumber = 3, Name = $"Custom_{type}_{id}", Type = type, Definition = xml };
	private static WearProfile Profile(long id = 51, string type = "Direct") => new() { Id = id, Name = "Trousers", Type = type };
	private static List<GameItemComponentProto> Components(string extraXml = "") =>
		[Component(1, "Holdable"), Component(2, "Wearable", $"<Definition>{Profiles}{extraXml}</Definition>")];

	[TestMethod]
	public void Wearable_ResolvesActualRevisionAndRuntimeLegacyDefaultsWithoutStockNameAssumptions()
	{
		var result = IndustrialisedClothingPhysicalBindings.Bind(Components(), [Profile()], [], Source);
		Assert.AreEqual(2L, result.ComponentId);
		Assert.AreEqual(3, result.RevisionNumber);
		Assert.AreEqual(51L, result.DefaultProfileId);
		CollectionAssert.AreEqual(new long[] { 51 }, new List<long>(result.ProfileIds));
		Assert.AreEqual(1.0, result.LayerWeight);
		Assert.IsFalse(result.Bulky);
		Assert.ThrowsException<NotSupportedException>(() => ((IList<long>)result.ProfileIds).Add(52));
	}

	[TestMethod]
	public void Wearable_ParsesInvariantLayerWeightAndResolvedDiagnosticProgram()
	{
		var original = CultureInfo.CurrentCulture;
		try
		{
			CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
			var components = Components("<LayerWeightConsumption>0.25</LayerWeightConsumption><WhyCannotWearProg>91</WhyCannotWearProg>");
			components[1].Definition = components[1].Definition.Replace("<Definition>", "<Definition Bulky=\"true\">");
			var result = IndustrialisedClothingPhysicalBindings.Bind(components, [Profile()], [91], Source);
			Assert.AreEqual(0.25, result.LayerWeight);
			Assert.IsTrue(result.Bulky);
		}
		finally { CultureInfo.CurrentCulture = original; }
	}

	[DataTestMethod]
	[DataRow("missing-holdable", "both Holdable and Wearable")]
	[DataRow("missing-wearable", "both Holdable and Wearable")]
	[DataRow("duplicate-logical-id", "multiple revisions")]
	[DataRow("duplicate-wearable", "Exclusive capability IWearable")]
	[DataRow("duplicate-variable", "Exclusive capability IVariable")]
	[DataRow("missing-sibling", "IDetonatable")]
	[DataRow("unknown-type", "Unknown or noncanonical")]
	[DataRow("wrong-case", "Unknown or noncanonical")]
	public void Composition_RejectsInvalidActualCapabilities(string fault, string diagnostic)
	{
		var components = Components();
		switch (fault)
		{
			case "missing-holdable": components.RemoveAt(0); break;
			case "missing-wearable": components.RemoveAt(1); break;
			case "duplicate-logical-id": components.Add(Component(1, "Variable")); break;
			case "duplicate-wearable": components.Add(Component(3, "Wearable")); break;
			case "duplicate-variable": components.AddRange([Component(3, "Variable"), Component(4, "Variable")]); break;
			case "missing-sibling": components.Add(Component(3, "CountdownDetonator")); break;
			case "unknown-type": components[0].Type = "UnknownStockComponent"; break;
			case "wrong-case": components[0].Type = "holdable"; break;
		}
		Failure(() => IndustrialisedClothingPhysicalBindings.Bind(components, [Profile()], [], Source), diagnostic);
	}

	[DataTestMethod]
	[DataRow("<Definition/>", "no Profiles")]
	[DataRow("<Wrong/>", "Definition XML root")]
	[DataRow("<Definition>", "Invalid Wearable XML")]
	[DataRow("<Definition><Profiles Default=\"51\"/></Definition>", "unique, positive")]
	[DataRow("<Definition><Profiles Default=\"51\"><Profile>51</Profile><Profile>51</Profile></Profiles></Definition>", "unique, positive")]
	[DataRow("<Definition><Profiles Default=\"51\"><Profile>-1</Profile></Profiles></Definition>", "unique, positive")]
	[DataRow("<Definition><Profiles Default=\"52\"><Profile>51</Profile></Profiles></Definition>", "default profile")]
	[DataRow("<Definition><Profiles><Profile>51</Profile></Profiles></Definition>", "default profile")]
	[DataRow("<Definition><Profiles Default=\"51\"><Profile>not-an-id</Profile></Profiles></Definition>", "Invalid Wearable XML")]
	public void Wearable_RejectsInvalidXmlAndProfileLists(string xml, string diagnostic)
	{
		var components = Components();
		components[1].Definition = xml;
		Failure(() => IndustrialisedClothingPhysicalBindings.Bind(components, [Profile()], [], Source), diagnostic);
	}

	[DataTestMethod]
	[DataRow("<LayerWeightConsumption>-1</LayerWeightConsumption>", "negative layer")]
	[DataRow("<LayerWeightConsumption>NaN</LayerWeightConsumption>", "finite")]
	[DataRow("<LayerWeightConsumption>Infinity</LayerWeightConsumption>", "finite")]
	[DataRow("<LayerWeightConsumption>0,5</LayerWeightConsumption>", "Invalid Wearable XML")]
	[DataRow("<LayerWeightConsumption>1</LayerWeightConsumption><LayerWeightConsumption>2</LayerWeightConsumption>", "repeats XML")]
	[DataRow("<SeeThroughDamageRatio>1.1</SeeThroughDamageRatio>", "between 0 and 1")]
	[DataRow("<Waterproof>true</Waterproof>", "ratio attribute")]
	[DataRow("<Waterproof ratio=\"-0.1\">true</Waterproof>", "between 0 and 1")]
	[DataRow("<Waterproof ratio=\"0.5\">perhaps</Waterproof>", "Invalid Wearable XML")]
	[DataRow("<WearableProg>92</WearableProg>", "unresolved WearableProg")]
	[DataRow("<WhyCannotWearProg>-1</WhyCannotWearProg>", "unresolved WhyCannotWearProg")]
	[DataRow("<WearableProg>91</WearableProg>", "conditional wear program")]
	public void Wearable_RejectsInvalidPhysicalSettingsAndUnprovedEligibility(string extraXml, string diagnostic) =>
		Failure(() => IndustrialisedClothingPhysicalBindings.Bind(Components(extraXml), [Profile()], [91], Source), diagnostic);

	[TestMethod]
	public void Wearable_RejectsMissingAmbiguousAndUnknownProfileTypes()
	{
		Failure(() => IndustrialisedClothingPhysicalBindings.Bind(Components(), [], [], Source), "missing or ambiguous wear profile 51");
		Failure(() => IndustrialisedClothingPhysicalBindings.Bind(Components(), [Profile(), Profile()], [], Source), "missing or ambiguous wear profile 51");
		Failure(() => IndustrialisedClothingPhysicalBindings.Bind(Components(), [Profile(type: "Unknown")], [], Source), "unknown runtime type");
	}

	private static void Failure(Action action, string diagnostic)
	{
		var exception = Assert.ThrowsException<InvalidDataException>(action);
		StringAssert.Contains(exception.Message, Source.ToString());
		StringAssert.Contains(exception.Message, diagnostic);
	}
}

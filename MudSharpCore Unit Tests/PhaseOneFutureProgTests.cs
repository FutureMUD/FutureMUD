#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions;
using MudSharp.FutureProg.Variables;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PhaseOneFutureProgTests
{
	[ClassInitialize]
	public static void ClassInitialise(TestContext _)
	{
		FutureProgTestBootstrap.EnsureInitialised();
	}

	[TestMethod]
	public void PhaseOneTypes_ExposeKeyDocumentedDotProperties()
	{
		foreach (var (type, property, propertyType) in DotProperties)
		{
			Assert.IsTrue(ProgVariable.DotReferenceCompileInfos.TryGetValue(type, out var compileInfo),
				type.Describe());
			Assert.IsTrue(compileInfo.PropertyTypeMap.TryGetValue(property, out var actualType),
				$"{type.Describe()}.{property}");
			Assert.AreEqual(propertyType, actualType, $"{type.Describe()}.{property}");
			Assert.IsTrue(compileInfo.PropertyHelpInfo.TryGetValue(property, out var help),
				$"{type.Describe()}.{property}");
			Assert.IsFalse(string.IsNullOrWhiteSpace(help), $"{type.Describe()}.{property}");
		}
	}

	[TestMethod]
	public void PhaseOneLookupFunctions_RegisterTypedIdAndNameOverloads()
	{
		var functions = FutureProg.GetFunctionCompilerInformations().ToList();
		foreach (var (name, type) in LookupFunctions)
		{
			AssertFunction(functions, name, [ProgVariableTypes.Number], type);
			AssertFunction(functions, name, [ProgVariableTypes.Text], type);
		}
	}

	[TestMethod]
	public void PhaseOneActionFunctions_RegisterResolvedReferenceOverloads()
	{
		var functions = FutureProg.GetFunctionCompilerInformations().ToList();
		AssertFunction(functions, "istagged", [ProgVariableTypes.Item, ProgVariableTypes.Tag], ProgVariableTypes.Boolean);
		AssertFunction(functions, "loaditem", [ProgVariableTypes.ItemPrototype], ProgVariableTypes.Item);
		AssertFunction(functions, "loadnpc", [ProgVariableTypes.NPCTemplate, ProgVariableTypes.Location], ProgVariableTypes.Character);
		AssertFunction(functions, "loadoutfittemplate", [ProgVariableTypes.OutfitTemplate, ProgVariableTypes.Character], ProgVariableTypes.Outfit);
		AssertFunction(functions, "vehicletrainweight", [ProgVariableTypes.Vehicle], ProgVariableTypes.Number);
		AssertFunction(functions, "celestialelevation", [ProgVariableTypes.Location, ProgVariableTypes.CelestialObject], ProgVariableTypes.Number);
		AssertFunction(functions, "connecttogrid", [ProgVariableTypes.Grid, ProgVariableTypes.Item], ProgVariableTypes.Boolean);
		AssertFunction(functions, "setcharacteristic", [ProgVariableTypes.Character, ProgVariableTypes.CharacteristicDefinition, ProgVariableTypes.CharacteristicValue], ProgVariableTypes.Boolean);
		AssertFunction(functions, "createfield", [ProgVariableTypes.Location, ProgVariableTypes.AgricultureFieldProfile], ProgVariableTypes.AgricultureField);
		AssertFunction(functions, "startfieldproject", [ProgVariableTypes.Character, ProgVariableTypes.AgricultureField, ProgVariableTypes.AgricultureOperation], ProgVariableTypes.Boolean);
		AssertFunction(functions, "drawfieldherd", [ProgVariableTypes.Character, ProgVariableTypes.AgricultureField, ProgVariableTypes.AgricultureHerdDefinition, ProgVariableTypes.Number], ProgVariableTypes.Boolean);
	}

	private static void AssertFunction(IEnumerable<FunctionCompilerInformation> functions, string name,
		IEnumerable<ProgVariableTypes> parameters, ProgVariableTypes returnType)
	{
		var function = functions.SingleOrDefault(x => x.FunctionName.EqualTo(name) &&
		                                             x.Parameters.SequenceEqual(parameters));
		Assert.IsNotNull(function, $"Missing {name}({string.Join(", ", parameters.Select(x => x.Describe()))}).");
		Assert.AreEqual(returnType, function.ReturnType);
		Assert.IsFalse(string.IsNullOrWhiteSpace(function.FunctionHelp));
		Assert.IsTrue(function.ParameterNames.All(x => !string.IsNullOrWhiteSpace(x)));
		Assert.IsTrue(function.ParameterHelp.All(x => !string.IsNullOrWhiteSpace(x)));
	}

	private static readonly (ProgVariableTypes Type, string Property, ProgVariableTypes PropertyType)[] DotProperties =
	[
		(ProgVariableTypes.Tag, "parent", ProgVariableTypes.Tag),
		(ProgVariableTypes.ItemPrototype, "material", ProgVariableTypes.Solid),
		(ProgVariableTypes.NPCTemplate, "templatetype", ProgVariableTypes.Text),
		(ProgVariableTypes.OutfitTemplate, "itemcount", ProgVariableTypes.Number),
		(ProgVariableTypes.Vehicle, "activejourney", ProgVariableTypes.VehicleJourney),
		(ProgVariableTypes.CelestialObject, "currentcelestialday", ProgVariableTypes.Number),
		(ProgVariableTypes.Grid, "locations", ProgVariableTypes.Location | ProgVariableTypes.Collection),
		(ProgVariableTypes.CharacteristicDefinition, "defaultvalue", ProgVariableTypes.CharacteristicValue),
		(ProgVariableTypes.CharacteristicValue, "definition", ProgVariableTypes.CharacteristicDefinition),
		(ProgVariableTypes.AgricultureFieldProfile, "defaultscorecount", ProgVariableTypes.Number),
		(ProgVariableTypes.AgricultureCropDefinition, "perennial", ProgVariableTypes.Boolean),
		(ProgVariableTypes.AgricultureHerdDefinition, "npctemplate", ProgVariableTypes.NPCTemplate),
		(ProgVariableTypes.AgricultureWoodlandDefinition, "harvestcycledays", ProgVariableTypes.Number),
		(ProgVariableTypes.AgricultureOperation, "project", ProgVariableTypes.Project)
	];

	private static readonly (string Name, ProgVariableTypes Type)[] LookupFunctions =
	[
		("tag", ProgVariableTypes.Tag),
		("itemprototype", ProgVariableTypes.ItemPrototype),
		("npctemplate", ProgVariableTypes.NPCTemplate),
		("outfittemplate", ProgVariableTypes.OutfitTemplate),
		("vehicle", ProgVariableTypes.Vehicle),
		("celestial", ProgVariableTypes.CelestialObject),
		("grid", ProgVariableTypes.Grid),
		("characteristicdefinition", ProgVariableTypes.CharacteristicDefinition),
		("characteristicvalue", ProgVariableTypes.CharacteristicValue),
		("fieldprofile", ProgVariableTypes.AgricultureFieldProfile),
		("cropdefinition", ProgVariableTypes.AgricultureCropDefinition),
		("herddefinition", ProgVariableTypes.AgricultureHerdDefinition),
		("woodlanddefinition", ProgVariableTypes.AgricultureWoodlandDefinition),
		("agricultureoperation", ProgVariableTypes.AgricultureOperation)
	];
}

#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions;
using MudSharp.FutureProg.Variables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PhaseTwoFutureProgTests
{
	[ClassInitialize]
	public static void ClassInitialise(TestContext _)
	{
		FutureProgTestBootstrap.EnsureInitialised();
	}

	[TestMethod]
	public void PhaseTwoTypes_ExposeKeyDocumentedDotProperties()
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
	public void PhaseTwoLookupFunctions_RegisterTypedOverloads()
	{
		var functions = FutureProg.GetFunctionCompilerInformations().ToList();
		foreach (var (name, type, supportsName) in LookupFunctions)
		{
			AssertFunction(functions, name, [ProgVariableTypes.Number], type);
			if (supportsName)
			{
				AssertFunction(functions, name, [ProgVariableTypes.Text], type);
			}
		}
	}

	[TestMethod]
	public void PhaseTwoActionFunctions_RegisterResolvedReferenceOverloads()
	{
		var functions = FutureProg.GetFunctionCompilerInformations().ToList();
		AssertFunction(functions, "property", [ProgVariableTypes.Location], ProgVariableTypes.Property);
		AssertFunction(functions, "ispropertyowner", [ProgVariableTypes.Property, ProgVariableTypes.Character],
			ProgVariableTypes.Boolean);
		AssertFunction(functions, "ispropertyleaseholder", [ProgVariableTypes.Property, ProgVariableTypes.Character],
			ProgVariableTypes.Boolean);
		AssertFunction(functions, "ispropertytenant", [ProgVariableTypes.Property, ProgVariableTypes.Character],
			ProgVariableTypes.Boolean);
		AssertFunction(functions, "sendchannel",
			[ProgVariableTypes.Channel, ProgVariableTypes.Character, ProgVariableTypes.Text], ProgVariableTypes.Void);
	}

	[TestMethod]
	public void SendChannel_CanBeUsedAsAStatementInAVoidProg()
	{
		var prog = new FutureProg(FutureProgTestBootstrap.Gameworld, "PhaseTwoSendChannel",
			ProgVariableTypes.Void,
			[
				Tuple.Create(ProgVariableTypes.Channel, "channel"),
				Tuple.Create(ProgVariableTypes.Character, "character"),
				Tuple.Create(ProgVariableTypes.Text, "message")
			],
			"sendchannel(@channel, @character, @message)");

		Assert.IsTrue(prog.Compile(), prog.CompileError);
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
		(ProgVariableTypes.Property, "lease", ProgVariableTypes.PropertyLease),
		(ProgVariableTypes.PropertyKey, "property", ProgVariableTypes.Property),
		(ProgVariableTypes.PropertyLease, "leaseorder", ProgVariableTypes.PropertyLeaseOrder),
		(ProgVariableTypes.PropertyLeaseOrder, "listed", ProgVariableTypes.Boolean),
		(ProgVariableTypes.PropertySaleOrder, "showforsale", ProgVariableTypes.Boolean),
		(ProgVariableTypes.EconomicZone, "properties", ProgVariableTypes.Property | ProgVariableTypes.Collection),
		(ProgVariableTypes.Channel, "commandwords", ProgVariableTypes.Text | ProgVariableTypes.Collection)
	];

	private static readonly (string Name, ProgVariableTypes Type, bool SupportsName)[] LookupFunctions =
	[
		("property", ProgVariableTypes.Property, true),
		("propertykey", ProgVariableTypes.PropertyKey, false),
		("propertylease", ProgVariableTypes.PropertyLease, false),
		("propertyleaseorder", ProgVariableTypes.PropertyLeaseOrder, false),
		("propertysaleorder", ProgVariableTypes.PropertySaleOrder, false),
		("economiczone", ProgVariableTypes.EconomicZone, true),
		("channel", ProgVariableTypes.Channel, true)
	];
}

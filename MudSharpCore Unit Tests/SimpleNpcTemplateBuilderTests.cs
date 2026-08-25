#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.NPC.Templates;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SimpleNpcTemplateBuilderTests
{
	[TestMethod]
	public void FindAttributeForBuilding_AliasFollowedByValue_UsesCapturedAlias()
	{
		var definition = new Mock<IAttributeDefinition>();
		definition.SetupGet(x => x.Id).Returns(42);
		definition.SetupGet(x => x.Alias).Returns("str");
		var trait = new Mock<ITrait>();
		trait.SetupGet(x => x.Definition).Returns(definition.Object);

		var result = SimpleNPCTemplate.FindAttributeForBuilding([trait.Object], "STR");

		Assert.AreSame(trait.Object, result);
	}

	[TestMethod]
	public void FindAttributeForBuilding_Id_UsesDefinitionId()
	{
		var definition = new Mock<IAttributeDefinition>();
		definition.SetupGet(x => x.Id).Returns(42);
		definition.SetupGet(x => x.Alias).Returns("str");
		var trait = new Mock<ITrait>();
		trait.SetupGet(x => x.Definition).Returns(definition.Object);

		var result = SimpleNPCTemplate.FindAttributeForBuilding([trait.Object], "42");

		Assert.AreSame(trait.Object, result);
	}
}

#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Autobuilder;
using MudSharp.Construction.Autobuilder.Rooms;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class AutobuilderTagBindingTests
{
	private sealed class Template(IFuturemud world, XElement xml) : AutobuilderRoomBase(world, "Bindings")
	{
		public void Load() => LoadFromXml(xml);
		public void Apply(IRoom room, params string[] selectors) => ApplyTagsToRoom(room, selectors);
		protected override XElement SaveToXml() => new("Template", SaveFrameworkTagBindings());
		public XElement Export() => SaveToXml();
		public override IAutobuilderRoom Clone(string name) => throw new NotSupportedException();
		public override IRoom CreateRoom(ICharacter builder, ITerrain specifiedTerrain, bool deferDescription, params string[] tags) => throw new NotSupportedException();
		public override string Show(ICharacter builder) => "Bindings";
		protected override string SubtypeHelpText => "";
	}

	private static ITag Tag(long id, string name) => Mock.Of<ITag>(x => x.Id == id && x.Name == name);
	private static Mock<IFuturemud> World(params ITag[] tags)
	{
		var collection = new All<ITag>();
		foreach (var tag in tags) collection.Add(tag);
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.Tags).Returns(collection);
		return world;
	}

	[TestMethod]
	public void ExplicitBinding_SelectsIntendedIdDespiteRepeatedLeafAndLeavesTextLabelsUnbound()
	{
		var wrong = Tag(10, "Mineral Resource");
		var intended = Tag(20, "Mineral Resource");
		var textOnly = Tag(30, "Physical Primary");
		var world = World(wrong, intended, textOnly);
		var template = new Template(world.Object, XElement.Parse("<Template><ApplyAutobuilderTagsAsFrameworkTags>true</ApplyAutobuilderTagsAsFrameworkTags><FrameworkTagBindings><Binding selector='Mineral Resource' id='20'/></FrameworkTagBindings></Template>"));
		template.Load();
		var room = new Mock<IRoom>();
		template.Apply(room.Object, "Mineral Resource", "Physical Primary");
		room.Verify(x => x.AddTag(intended), Times.Once);
		room.Verify(x => x.AddTag(wrong), Times.Never);
		room.Verify(x => x.AddTag(textOnly), Times.Never);
		Assert.AreEqual("Mineral Resource", template.Export().Element("FrameworkTagBindings")!.Element("Binding")!.Attribute("selector")!.Value);
		Assert.AreEqual("20", template.Export().Element("FrameworkTagBindings")!.Element("Binding")!.Attribute("id")!.Value);
	}

	[TestMethod]
	public void LegacyAmbiguousSelector_FailsWithDiagnosticInsteadOfChoosingEnumerationOrder()
	{
		var world = World(Tag(5, "Road"), Tag(10, "Mineral Resource"), Tag(20, "Mineral Resource"));
		var template = new Template(world.Object, XElement.Parse("<Template><ApplyAutobuilderTagsAsFrameworkTags>true</ApplyAutobuilderTagsAsFrameworkTags></Template>"));
		template.Load();
		var room = new Mock<IRoom>();
		var error = Assert.ThrowsException<ApplicationException>(() => template.Apply(room.Object, "Road", "Mineral Resource"));
		StringAssert.Contains(error.Message, "ambiguous legacy selector");
		room.Verify(x => x.AddTag(It.IsAny<ITag>()), Times.Never);
	}

	[TestMethod]
	public void MissingExplicitId_IsRejectedAtLoadAndUniqueLegacySelectorStillWorks()
	{
		var tag = Tag(10, "Road");
		var world = World(tag);
		var invalid = new Template(world.Object, XElement.Parse("<Template><FrameworkTagBindings><Binding selector='Road' id='99'/></FrameworkTagBindings></Template>"));
		Assert.ThrowsException<ApplicationException>(() => invalid.Load());
		var legacy = new Template(world.Object, XElement.Parse("<Template><ApplyAutobuilderTagsAsFrameworkTags>true</ApplyAutobuilderTagsAsFrameworkTags></Template>"));
		legacy.Load();
		var room = new Mock<IRoom>();
		legacy.Apply(room.Object, "Road");
		room.Verify(x => x.AddTag(tag), Times.Once);
	}
}

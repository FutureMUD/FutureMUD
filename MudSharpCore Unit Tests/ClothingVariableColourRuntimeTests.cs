#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ClothingVariableColourRuntimeTests
{
	[DataTestMethod]
	[DataRow("colour=\"oak-gall black\"", "oak-gall black")]
	[DataRow("colour=blue-green", "blue-green")]
	[DataRow("colour=\"banana yellow (stock colour 284)\"", "banana yellow (stock colour 284)")]
	[DataRow("colour=\"madder/red\"", "madder/red")]
	public void PunctuatedColourLookup_IsSelectedWithoutPrefixTruncationOrRandomFallback(string arguments, string name)
	{
		var world = new Mock<IFuturemud>();
		var definition = new Mock<ICharacteristicDefinition>();
		definition.Setup(x => x.Id).Returns(11);
		definition.Setup(x => x.Pattern).Returns(new Regex("^colour$", RegexOptions.IgnoreCase));
		definition.Setup(x => x.IsValue(It.IsAny<ICharacteristicValue>())).Returns(true);
		var fallback = Value(21, "blue", definition.Object);
		var selected = Value(23, name, definition.Object);
		var profile = new Mock<ICharacteristicProfile>();
		profile.Setup(x => x.Id).Returns(31);
		profile.Setup(x => x.GetRandomCharacteristic()).Returns(fallback);
		world.Setup(x => x.Characteristics).Returns(Collection(definition.Object));
		world.Setup(x => x.CharacteristicProfiles).Returns(Collection(profile.Object));
		world.Setup(x => x.CharacteristicValues).Returns(Collection(fallback, selected));
		var proto = new TestVariableProto(world.Object);
		Assert.AreSame(selected, proto.GetValuesFromString(arguments)[definition.Object]);
		profile.Verify(x => x.GetRandomCharacteristic(), Times.Never);
	}

	[DataTestMethod]
	[DataRow("banana yellow", 284L, 6, 255, 255, 0)]
	[DataRow("ebony", 25L, 0, 0, 0, 0)]
	[DataRow("cerulean", 91L, 11, 0, 75, 255)]
	public void QualifiedStockLookup_PreservesVisibleColourAndPersistedSelection(
		string name, long colourId, int basic, int red, int green, int blue)
	{
		var world = new Mock<IFuturemud>();
		var definition = new Mock<ICharacteristicDefinition>();
		definition.Setup(x => x.Id).Returns(11);
		definition.Setup(x => x.Pattern).Returns(new Regex("^colour$", RegexOptions.IgnoreCase));
		definition.Setup(x => x.IsValue(It.IsAny<ICharacteristicValue>()))
			.Returns((ICharacteristicValue x) => x.Definition == definition.Object);
		world.Setup(x => x.Characteristics).Returns(Collection(definition.Object));
		world.Setup(x => x.FutureProgs).Returns(Collection<MudSharp.FutureProg.IFutureProg>());
		var colour = new MudSharp.Form.Colour.Colour(new MudSharp.Models.Colour
		{
			Id = colourId, Name = name, Basic = basic, Red = red, Green = green, Blue = blue, Fancy = $"the colour of {name}"
		}, world.Object);
		world.Setup(x => x.Colours).Returns(Collection<MudSharp.Form.Colour.IColour>(colour));
		var stored = new MudSharp.Models.CharacteristicValue { Id = 23, Name = name, DefinitionId = 11, Value = colourId.ToString() };
		var original = new ColourCharacteristicValue(stored, world.Object);
		stored.Name = $"{name} (stock colour {colourId})";
		var qualified = new ColourCharacteristicValue(stored, world.Object);
		Assert.AreEqual(original.Id, qualified.Id);
		Assert.AreNotEqual(original.Name, qualified.Name);
		Assert.AreEqual(name, qualified.GetValue);
		Assert.AreEqual(original.GetBasicValue, qualified.GetBasicValue);
		Assert.AreEqual(original.GetFancyValue, qualified.GetFancyValue);
		Assert.AreSame(original.Colour, qualified.Colour);

		var profile = new Mock<ICharacteristicProfile>();
		profile.Setup(x => x.Id).Returns(31);
		profile.Setup(x => x.GetRandomCharacteristic()).Returns(qualified);
		world.Setup(x => x.CharacteristicProfiles).Returns(Collection(profile.Object));
		world.Setup(x => x.CharacteristicValues).Returns(Collection<ICharacteristicValue>(qualified));
		var proto = new TestVariableProto(world.Object);
		Assert.AreSame(qualified, proto.GetValuesFromString("colour=23")[definition.Object]);
		Assert.AreSame(qualified, proto.GetValuesFromString($"colour=\"{stored.Name}\"")[definition.Object]);
		profile.Verify(x => x.GetRandomCharacteristic(), Times.Never);
		var parent = new Mock<IGameItem>();
		parent.Setup(x => x.Gameworld).Returns(world.Object);
		var item = new TestVariableComponent(proto, parent.Object);
		item.SetCharacteristic(definition.Object, qualified);
		var xml = item.SaveXml();
		profile.Invocations.Clear();
		var reloaded = new VariableGameItemComponent(new MudSharp.Models.GameItemComponent { Id = 1, Definition = xml }, proto, parent.Object);
		Assert.AreSame(qualified, reloaded.GetCharacteristic(definition.Object));
		Assert.AreEqual(name, reloaded.GetCharacteristic(definition.Object).GetValue);
		var copied = (VariableGameItemComponent)reloaded.Copy(parent.Object, temporary: true);
		Assert.AreSame(qualified, copied.GetCharacteristic(definition.Object));
		profile.Verify(x => x.GetRandomCharacteristic(), Times.Never);
	}

	[TestMethod]
	public void NumericColourDefaultAndOverride_SurviveComponentSaveReloadAndCopy()
	{
		var world = new Mock<IFuturemud>();
		var definition = new Mock<ICharacteristicDefinition>();
		definition.Setup(x => x.Id).Returns(11);
		definition.Setup(x => x.Pattern).Returns(new Regex("^colour$", RegexOptions.IgnoreCase));
		var blue = Value(21, "blue", definition.Object);
		var black = Value(23, "black", definition.Object);
		definition.Setup(x => x.IsValue(It.IsAny<ICharacteristicValue>()))
			.Returns((ICharacteristicValue x) => x.Definition == definition.Object);
		var profile = new Mock<ICharacteristicProfile>();
		profile.Setup(x => x.Id).Returns(31);
		profile.Setup(x => x.GetRandomCharacteristic()).Returns(blue);
		world.Setup(x => x.Characteristics).Returns(Collection(definition.Object));
		world.Setup(x => x.CharacteristicProfiles).Returns(Collection(profile.Object));
		world.Setup(x => x.CharacteristicValues).Returns(Collection(blue, black));
		var proto = new TestVariableProto(world.Object);
		profile.Invocations.Clear();
		Assert.AreSame(blue, proto.GetValuesFromString("colour=21")[definition.Object]);
		Assert.AreSame(black, proto.GetValuesFromString("colour=21 colour=23")[definition.Object]);
		profile.Verify(x => x.GetRandomCharacteristic(), Times.Never);

		var parent = new Mock<IGameItem>();
		parent.Setup(x => x.Gameworld).Returns(world.Object);
		var component = new TestVariableComponent(proto, parent.Object);
		component.SetCharacteristic(definition.Object, black);
		var saved = component.SaveXml();
		profile.Invocations.Clear();
		var reloaded = new VariableGameItemComponent(new MudSharp.Models.GameItemComponent { Id = 1, Definition = saved }, proto, parent.Object);
		Assert.AreSame(black, reloaded.GetCharacteristic(definition.Object));
		var copied = (VariableGameItemComponent)reloaded.Copy(parent.Object, temporary: true);
		Assert.AreSame(black, copied.GetCharacteristic(definition.Object));
		profile.Verify(x => x.GetRandomCharacteristic(), Times.Never);
	}

	private static ICharacteristicValue Value(long id, string name, ICharacteristicDefinition definition)
	{
		var value = new Mock<ICharacteristicValue>();
		value.Setup(x => x.Id).Returns(id);
		value.Setup(x => x.Name).Returns(name);
		value.Setup(x => x.Definition).Returns(definition);
		return value.Object;
	}

	private static IUneditableAll<T> Collection<T>(params T[] items) where T : class, IFrameworkItem
	{
		var collection = new Mock<IUneditableAll<T>>();
		collection.As<IEnumerable<T>>().Setup(x => x.GetEnumerator()).Returns(() => ((IEnumerable<T>)items).GetEnumerator());
		collection.Setup(x => x.Get(It.IsAny<long>())).Returns((long id) => items.SingleOrDefault(x => x.Id == id)!);
		return collection.Object;
	}

	private sealed class TestVariableProto(IFuturemud world) : VariableGameItemComponentProto(new MudSharp.Models.GameItemComponentProto
	{
		Id = 41, Name = "Variable_Garment", Description = "Garment colour", Type = "Variable",
		Definition = "<Definition><Characteristic Value=\"11\" Profile=\"31\"/></Definition>",
		EditableItem = new MudSharp.Models.EditableItem { RevisionStatus = 4 }
	}, world);

	private sealed class TestVariableComponent(VariableGameItemComponentProto proto, IGameItem parent)
		: VariableGameItemComponent(proto, parent, temporary: true)
	{
		internal string SaveXml() => SaveToXml();
	}
}

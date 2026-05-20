#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Communication;
using MudSharp.Communication.Language;
using MudSharp.Form.Colour;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using DbEditableItem = MudSharp.Models.EditableItem;
using DbGameItemComponent = MudSharp.Models.GameItemComponent;
using DbGameItemComponentProto = MudSharp.Models.GameItemComponentProto;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ScribingWritingComponentsTests
{
	private const long ColourId = 7L;

	[TestMethod]
	public void GameItemComponentManager_RegistersScribingAndInscribableSurfaceTypes()
	{
		var manager = new GameItemComponentManager();
		var primaryTypes = manager.PrimaryTypes.ToList();
		var helpTypes = manager.TypeHelpInfo.Select(x => x.Name).ToList();

		CollectionAssert.Contains(primaryTypes, "scribingimplement");
		CollectionAssert.Contains(primaryTypes, "inscribablesurface");
		CollectionAssert.Contains(helpTypes, "ScribingImplement");
		CollectionAssert.Contains(helpTypes, "InscribableSurface");
	}

	[TestMethod]
	public void ScribingImplement_LoadSaveAndFiniteUses_Work()
	{
		var colour = CreateColour(ColourId, "black");
		var gameworld = CreateGameworld(colour);
		var proto = CreateScribingProto(gameworld.Object, WritingImplementType.ReedPen, ColourId, 10);
		var component = (ScribingImplementGameItemComponent)proto.CreateNew(CreateParent(gameworld.Object, 1L));

		Assert.AreEqual(WritingImplementType.ReedPen, component.WritingImplementType);
		Assert.AreEqual("black", component.WritingImplementColour.Name);
		Assert.IsTrue(component.Primed);

		component.Use(7);

		Assert.AreEqual(3, component.RemainingUses);
		Assert.IsTrue(component.Primed);

		component.Use(5);

		Assert.AreEqual(0, component.RemainingUses);
		Assert.IsFalse(component.Primed);
		StringAssert.Contains(InvokeSaveToXml(component), "<RemainingUses>0</RemainingUses>");

		var loaded = (ScribingImplementGameItemComponent)proto.LoadComponent(
			new DbGameItemComponent { Definition = "<Definition><RemainingUses>4</RemainingUses></Definition>" },
			CreateParent(gameworld.Object, 2L));

		Assert.AreEqual(4, loaded.RemainingUses);
		Assert.IsTrue(loaded.Primed);
	}

	[TestMethod]
	public void ScribingImplement_NonConsumingToolsRemainPrimed()
	{
		var gameworld = CreateGameworld(CreateColour(ColourId, "black"));
		var proto = CreateScribingProto(gameworld.Object, WritingImplementType.Stylus, ColourId, 0);
		var component = (ScribingImplementGameItemComponent)proto.CreateNew(CreateParent(gameworld.Object, 3L));

		component.Use(5000);

		Assert.AreEqual(0, component.RemainingUses);
		Assert.IsTrue(component.Primed);
	}

	[TestMethod]
	public void InscribableSurface_UsesConfiguredImplementTypesAndCapacity()
	{
		var gameworld = CreateGameworld(CreateColour(ColourId, "black"));
		var proto = CreateInscribableSurfaceProto(gameworld.Object, 20, WritingImplementType.Stylus);
		var surface = (InscribableSurfaceGameItemComponent)proto.CreateNew(CreateParent(gameworld.Object, 10L));
		var stylus = CreateImplement(WritingImplementType.Stylus, true, gameworld.Object);
		var quill = CreateImplement(WritingImplementType.Quill, true, gameworld.Object);
		var spentStylus = CreateImplement(WritingImplementType.Stylus, false, gameworld.Object);
		var shortWriting = CreateWriting(11L, 10).Object;
		var longWriting = CreateWriting(12L, 25).Object;

		Assert.IsTrue(surface.CanWrite(Mock.Of<ICharacter>(), stylus.Object, shortWriting));
		Assert.IsFalse(surface.CanWrite(Mock.Of<ICharacter>(), quill.Object, shortWriting));
		Assert.IsFalse(surface.CanWrite(Mock.Of<ICharacter>(), spentStylus.Object, shortWriting));
		Assert.IsFalse(surface.CanWrite(Mock.Of<ICharacter>(), stylus.Object, longWriting));

		var stackedSurface =
			(InscribableSurfaceGameItemComponent)proto.CreateNew(CreateParent(gameworld.Object, 11L, quantity: 2));

		Assert.IsFalse(stackedSurface.CanWrite(Mock.Of<ICharacter>(), stylus.Object, shortWriting));
	}

	[TestMethod]
	public void InscribableSurface_LoadsSavesAndCopiesReadableContent()
	{
		var writing = CreateWriting(21L, 8);
		var gameworld = CreateGameworld(CreateColour(ColourId, "black"), writing.Object);
		var proto = CreateInscribableSurfaceProto(gameworld.Object, 40, WritingImplementType.Stylus);
		var component = (InscribableSurfaceGameItemComponent)proto.LoadComponent(
			new DbGameItemComponent
			{
				Definition = new XElement("Definition",
					new XElement("Title", new XCData("Tablet Note")),
					new XElement("Writings", new XElement("Writing", 21L))).ToString()
			},
			CreateParent(gameworld.Object, 20L));

		Assert.AreEqual("Tablet Note", component.Title);
		Assert.AreSame(writing.Object, ((IReadable)component).Writings.Single());
		StringAssert.Contains(InvokeSaveToXml(component), "<Writing>21</Writing>");

		var copy = (InscribableSurfaceGameItemComponent)component.Copy(CreateParent(gameworld.Object, 22L), true);

		Assert.AreEqual("Tablet Note", copy.Title);
		Assert.AreSame(writing.Object, ((IReadable)copy).Writings.Single());
	}

	private static ScribingImplementGameItemComponentProto CreateScribingProto(IFuturemud gameworld,
		WritingImplementType type, long colourId, int totalUses)
	{
		return (ScribingImplementGameItemComponentProto)Activator.CreateInstance(
			typeof(ScribingImplementGameItemComponentProto),
			BindingFlags.Instance | BindingFlags.NonPublic,
			null,
			new object[]
			{
				ComponentProtoModel(100, "ScribingImplement", new XElement("Definition",
					new XElement("ImplementType", type.ToString()),
					new XElement("Colour", colourId),
					new XElement("ColourCharacteristic", 0),
					new XElement("TotalUses", totalUses)).ToString()),
				gameworld
			},
			CultureInfo.InvariantCulture)!;
	}

	private static InscribableSurfaceGameItemComponentProto CreateInscribableSurfaceProto(IFuturemud gameworld,
		int capacity, params WritingImplementType[] allowedTypes)
	{
		return (InscribableSurfaceGameItemComponentProto)Activator.CreateInstance(
			typeof(InscribableSurfaceGameItemComponentProto),
			BindingFlags.Instance | BindingFlags.NonPublic,
			null,
			new object[]
			{
				ComponentProtoModel(101, "InscribableSurface", new XElement("Definition",
					new XElement("MaximumCharacterLengthOfText", capacity),
					new XElement("AllowedImplementTypes",
						allowedTypes.Select(x => new XElement("Type", x.ToString())))).ToString()),
				gameworld
			},
			CultureInfo.InvariantCulture)!;
	}

	private static DbGameItemComponentProto ComponentProtoModel(long id, string type, string definition)
	{
		return new DbGameItemComponentProto
		{
			Id = id,
			Type = type,
			Name = type,
			Description = $"{type} component",
			Definition = definition,
			RevisionNumber = 1,
			EditableItem = new DbEditableItem
			{
				Id = id,
				BuilderAccountId = 1,
				BuilderDate = DateTime.UtcNow,
				RevisionNumber = 1,
				RevisionStatus = (int)RevisionStatus.Current
			}
		};
	}

	private static string InvokeSaveToXml(GameItemComponent component)
	{
		return (string)component.GetType()
		                .GetMethod("SaveToXml", BindingFlags.Instance | BindingFlags.NonPublic)!
		                .Invoke(component, Array.Empty<object>())!;
	}

	private static IGameItem CreateParent(IFuturemud gameworld, long id, int quantity = 1)
	{
		var parent = new Mock<IGameItem>();
		parent.SetupGet(x => x.Id).Returns(id);
		parent.SetupGet(x => x.Gameworld).Returns(gameworld);
		parent.SetupGet(x => x.Quantity).Returns(quantity);
		return parent.Object;
	}

	private static Mock<IWritingImplement> CreateImplement(WritingImplementType type, bool primed, IFuturemud gameworld)
	{
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Gameworld).Returns(gameworld);
		var implement = new Mock<IWritingImplement>();
		implement.SetupGet(x => x.WritingImplementType).Returns(type);
		implement.SetupGet(x => x.Primed).Returns(primed);
		implement.SetupGet(x => x.Parent).Returns(item.Object);
		return implement;
	}

	private static Mock<IWriting> CreateWriting(long id, int length)
	{
		var writing = new Mock<IWriting>();
		writing.SetupGet(x => x.Id).Returns(id);
		writing.SetupGet(x => x.Name).Returns($"writing {id}");
		writing.SetupGet(x => x.DocumentLength).Returns(length);
		return writing;
	}

	private static Mock<IFuturemud> CreateGameworld(IColour colour, params IWriting[] writings)
	{
		var saveManager = new Mock<ISaveManager>();
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.SaveManager).Returns(saveManager.Object);
		gameworld.SetupGet(x => x.Colours).Returns(Repository(colour));
		gameworld.SetupGet(x => x.Writings).Returns(Repository(writings));
		gameworld.SetupGet(x => x.Drawings).Returns(Repository<IDrawing>());
		gameworld.Setup(x => x.GetStaticLong("DefaultWritingColourInText")).Returns(ColourId);
		gameworld.Setup(x => x.GetStaticLong("DefaultBiroColour")).Returns(ColourId);
		return gameworld;
	}

	private static IColour CreateColour(long id, string name)
	{
		var colour = new Mock<IColour>();
		colour.SetupGet(x => x.Id).Returns(id);
		colour.SetupGet(x => x.Name).Returns(name);
		return colour.Object;
	}

	private static IUneditableAll<T> Repository<T>(params T[] items) where T : class, IFrameworkItem
	{
		var repository = new Mock<IUneditableAll<T>>();
		repository.Setup(x => x.Get(It.IsAny<long>()))
		          .Returns<long>(id => items.FirstOrDefault(x => x.Id == id));
		repository.Setup(x => x.GetByName(It.IsAny<string>()))
		          .Returns<string>(name => items.FirstOrDefault(x => x.Name.EqualTo(name)));
		repository.Setup(x => x.Get(It.IsAny<string>()))
		          .Returns<string>(name => items.Where(x => x.Name.EqualTo(name)).ToList());
		repository.Setup(x => x.GetEnumerator())
		          .Returns(() => ((IEnumerable<T>)items).GetEnumerator());
		repository.SetupGet(x => x.Count).Returns(items.Length);
		return repository.Object;
	}
}

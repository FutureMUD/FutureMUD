#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Form.Shape;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using Db = MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonWorldfileMaskTests
{
	[TestMethod]
	public void PlacedMasks_PreserveSourceProseAndBuilderEditsAndUseNativeWornActivation()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		var archive = ArmageddonWorldfileTestData.Load().ForInstallation();
		var writer = new ArmageddonWorldfileWriter(db, archive);
		var placed = archive.Resets.Where(x => x.Opcode is "O" or "G" or "E" or "P").Select(x => x.Arguments[1]).ToHashSet();
		var masks = archive.Items.Where(x => x.ItemType == 31 && placed.Contains(x.Vnum)).ToArray();
		Assert.AreEqual(12, masks.Length);
		var wearable = new Db.GameItemComponentProto { Id = 990000, Type = "Wearable", Definition = "<Definition/>" };
		db.GameItemComponentProtos.Add(wearable);
		foreach (var source in masks)
		{
			AddProto(source.Vnum, source, false);
			db.GameItems.Add(new Db.GameItem { Id = source.Vnum, GameItemProtoId = source.Vnum, EffectData = "<Effects fixture='preserved'/>" });
		}
		AddProto(990001, masks.Single(x => x.Vnum == 14788), true);
		var resetDescription = new Db.GameItemComponentProto { Id = 990001, Type = "Keyword Descriptions", Definition =
			"<Definition><KeywordDescriptions><Entry><Keyword>[mask_desc]</Keyword><Prose>The reset-authored wearer description.</Prose></Entry></KeywordDescriptions></Definition>" };
		db.GameItemComponentProtos.Add(resetDescription);
		Field<Dictionary<string, Db.GameItemProto>>(writer, "_resetItemVariants")["mask-fixture"].GameItemProtosGameItemComponentProtos.Add(
			new Db.GameItemProtosGameItemComponentProtos { GameItemProtoId = 990001, GameItemComponentProtoId = resetDescription.Id });
		AddProto(990002, masks.Single(x => x.Vnum == 14788) with { Vnum = 990002 }, false);
		db.SaveChanges();
		Install();
		db.SaveChanges();
		var components = db.GameItemComponentProtos.Where(x => x.Type == "IdentityObscurer").ToArray();
		Assert.AreEqual(12, components.Length, "Two authored masks, nine fallback masks and the reset variant map.");
		var resetMask = db.GameItemProtosGameItemComponentProtos.Single(x => x.GameItemProtoId == 990001 && x.GameItemComponentProtoId != wearable.Id && x.GameItemComponentProtoId != resetDescription.Id);
		Assert.AreEqual("The reset-authored wearer description.", XElement.Parse(components.Single(x => x.Id == resetMask.GameItemComponentProtoId).Definition).Element("FullDescription")!.Value);
		foreach (var vnum in new[] { 14788, 25414 })
		{
			var link = db.GameItemProtosGameItemComponentProtos.Single(x => x.GameItemProtoId == vnum && x.GameItemComponentProtoId != wearable.Id);
			var component = components.Single(x => x.Id == link.GameItemComponentProtoId);
			Assert.AreEqual(masks.Single(x => x.Vnum == vnum).ExtraDescriptions.First(x => x.Keyword == "[mask_desc]").Description,
				XElement.Parse(component.Definition).Element("FullDescription")!.Value);
			Assert.AreEqual(1, db.GameItemComponents.Count(x => x.GameItemId == vnum));
		}
		Assert.AreEqual(1, db.GameItemProtosGameItemComponentProtos.Count(x => x.GameItemProtoId == 990002), "Unplaced definition is not activated.");
		Assert.AreEqual(1, db.GameItemProtosGameItemComponentProtos.Count(x => x.GameItemProtoId == 14304), "The bracketless unused metadata is not silently promoted to the source's bracketed lookup.");
		Assert.IsTrue(db.GameItems.All(x => x.EffectData == "<Effects fixture='preserved'/>"));
		foreach (var component in components) AssertNativeActivation(component);
		foreach (var vnum in new[] { 14308, 35909, 50391, 58453, 58454, 58456, 58458, 58459, 58460 })
		{
			var link = db.GameItemProtosGameItemComponentProtos.Single(x => x.GameItemProtoId == vnum && x.GameItemComponentProtoId != wearable.Id);
			var definition = XElement.Parse(components.Single(x => x.Id == link.GameItemComponentProtoId).Definition);
			Assert.IsTrue((bool)definition.Element("PreserveFullDescription")!);
			Assert.AreEqual(1, db.GameItemComponents.Count(x => x.GameItemId == vnum));
		}


		var builderMask = components.First(x => (bool?)XElement.Parse(x.Definition).Element("PreserveFullDescription") == true);
		builderMask.Definition = XElement.Parse(builderMask.Definition).ToString().Replace("figure wearing", "builder custom figure wearing", StringComparison.Ordinal)
			.Replace("<PreserveFullDescription>true</PreserveFullDescription>", "<PreserveFullDescription>false</PreserveFullDescription>", StringComparison.Ordinal);
		var builderDefinition = builderMask.Definition;
		Install();
		db.SaveChanges();
		Assert.AreEqual(builderDefinition, builderMask.Definition);
		Assert.AreEqual(12, db.GameItemComponentProtos.Count(x => x.Type == "IdentityObscurer"));
		Assert.AreEqual(11, db.GameItemComponents.Count());
		return;

		void Install() => typeof(ArmageddonWorldfileWriter).GetMethod("InstallPlacedMasks", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		void AddProto(long id, ArmWorldItem source, bool variant)
		{
			var proto = new Db.GameItemProto { Id = id, Name = source.ShortDescription };
			proto.GameItemProtosGameItemComponentProtos.Add(new Db.GameItemProtosGameItemComponentProtos { GameItemProtoId = id, GameItemComponentProtoId = wearable.Id });
			db.GameItemProtos.Add(proto);
			if (variant) Field<Dictionary<string, Db.GameItemProto>>(writer, "_resetItemVariants").Add("mask-fixture", proto);
			else Field<Dictionary<int, Db.GameItemProto>>(writer, "_items").Add(source.Vnum, proto);
			Field<Dictionary<long, (ArmWorldItem Source, int[] Values)>>(writer, "_effectiveItems").Add(id, (source, source.Values));
		}
	}

	[TestMethod]
	public void NativeFallback_DefaultsOffAndBuilderToggleSurvivesSerialization()
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var model = new Db.GameItemComponentProto { Id = 1, EditableItem = new Db.EditableItem(), Type = "IdentityObscurer", Definition =
			"<Definition><RemovalEcho/><ShortDescription>A figure wearing @item</ShortDescription><FullDescription>Authored prose.</FullDescription><Difficulty>9</Difficulty><Keywords/></Definition>" };
		IdentityObscurerGameItemComponentProto Load() => (IdentityObscurerGameItemComponentProto)Activator.CreateInstance(
			typeof(IdentityObscurerGameItemComponentProto), BindingFlags.Instance | BindingFlags.NonPublic,
			null, new object[] { model, world.Object }, null)!;
		var proto = Load();
		Assert.IsFalse(proto.PreserveFullDescription, "Older saved definitions keep their existing override.");
		AssertNativeActivation(model);
		var builder = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock };
		Assert.IsTrue(proto.BuildingCommand(builder.Object, new StringStack("preservefull")));
		Assert.IsTrue(proto.PreserveFullDescription);
		model.Definition = (string)typeof(IdentityObscurerGameItemComponentProto).GetMethod("SaveToXml", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(proto, null)!;
		var reloaded = Load();
		Assert.IsTrue(reloaded.PreserveFullDescription);
		Assert.AreEqual("Authored prose.", reloaded.OverriddenFullDescription, "Toggling retains the builder's authored override for later reuse.");
		AssertNativeActivation(model);
		Assert.IsTrue(reloaded.BuildingCommand(builder.Object, new StringStack("preservefull")));
		Assert.IsFalse(reloaded.PreserveFullDescription);
		model.Definition = (string)typeof(IdentityObscurerGameItemComponentProto).GetMethod("SaveToXml", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(reloaded, null)!;
		AssertNativeActivation(model);
	}

	private static void AssertNativeActivation(Db.GameItemComponentProto model)
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var parent = new Mock<IGameItem>();
		parent.SetupGet(x => x.Gameworld).Returns(world.Object);
		parent.Setup(x => x.HowSeen(It.IsAny<IPerceiver>(), It.IsAny<bool>(), It.IsAny<DescriptionType>(), It.IsAny<bool>(), It.IsAny<PerceiveIgnoreFlags>())).Returns("an authored mask");
		var wearable = new Mock<IWearable>();
		parent.Setup(x => x.GetItemType<IWearable>()).Returns(wearable.Object);
		var proto = (IdentityObscurerGameItemComponentProto)Activator.CreateInstance(typeof(IdentityObscurerGameItemComponentProto),
			BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { model, world.Object }, null)!;
		var component = proto.CreateNew(parent.Object, temporary: true);
		var mask = (IObscureIdentity)component;
		parent.SetupGet(x => x.Components).Returns(new[] { component });
		StringAssert.Contains(mask.OverriddenShortDescription, "an authored mask");
		var definition = XElement.Parse(model.Definition);
		var preserve = (bool?)definition.Element("PreserveFullDescription") ?? false;
		Assert.AreEqual(preserve ? null : definition.Element("FullDescription")!.Value, mask.OverriddenFullDescription);
		var actor = new Mock<ICharacter>();
		var observer = new Mock<ICharacter>();
		var body = new Mock<IBody>();
		actor.SetupGet(x => x.Body).Returns(body.Object);
		body.SetupGet(x => x.WornItems).Returns(Array.Empty<IGameItem>());
		Assert.IsFalse(CharacterRecognitionAuthority.IsObscured(actor.Object, observer.Object), "Held masks do not activate.");
		body.SetupGet(x => x.WornItems).Returns(new[] { parent.Object });
		Assert.IsTrue(CharacterRecognitionAuthority.IsObscured(actor.Object, observer.Object));
		wearable.SetupGet(x => x.GloballyTransparent).Returns(true);
		Assert.IsFalse(CharacterRecognitionAuthority.IsObscured(actor.Object, observer.Object));
		wearable.SetupGet(x => x.GloballyTransparent).Returns(false);
		body.SetupGet(x => x.WornItems).Returns(Array.Empty<IGameItem>());
		Assert.IsFalse(CharacterRecognitionAuthority.IsObscured(actor.Object, observer.Object), "Removing a mask restores native recognition admission.");
	}

	private static T Field<T>(object value, string name) => (T)value.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(value)!;
}

#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;

namespace MudSharp_Unit_Tests;

[TestClass]
public class KeywordDescriptionsTests
{
	[TestMethod]
	public void MatchingAcceptsEachWhitespaceAlternativeAndTheWholePhrase()
	{
		Assert.IsTrue(KeywordDescriptionMatcher.Matches("weathered brass fitting", "weathered"));
		Assert.IsTrue(KeywordDescriptionMatcher.Matches("weathered brass fitting", "brass fitting"));
		Assert.IsFalse(KeywordDescriptionMatcher.Matches("weathered brass fitting", "brass plate"));
	}

	[TestMethod]
	public void ResolverKeepsOrdinaryTargetPrecedenceAndRejectsInvisibleProviders()
	{
		var provider = new TestProvider([new KeywordDescription("brass fitting", "A maker's mark is stamped on it.")]);
		Assert.IsNull(KeywordDescriptionResolver.Find("brass", true, [(true, [provider])]));
		Assert.IsNull(KeywordDescriptionResolver.Find("brass", false, [(false, [provider])]));
		Assert.AreEqual("A maker's mark is stamped on it.",
			KeywordDescriptionResolver.Find("brass", false, [(true, [provider])])?.Description);
	}

	[TestMethod]
	public void RoomEffectRoundTripsLongProseThroughNativeEffectXml()
	{
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg>());
		var room = new Mock<IRoom>();
		room.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var prose = new string('x', 36_000);
		var effect = new KeywordDescriptionsEffect(room.Object, [new KeywordDescription("engraving", prose)]);
		var saved = effect.SaveToXml(new Dictionary<IEffect, System.TimeSpan>());
		KeywordDescriptionsEffect.InitialiseEffectType();
		var loaded = (KeywordDescriptionsEffect)Effect.LoadEffect(saved, room.Object);
		Assert.AreEqual(prose, loaded.KeywordDescriptions.Single().Description);
	}

	[TestMethod]
	public void ItemPrototypePreservesLongKeywordProseThroughXmlAndComponentCopy()
	{
		var prose = new string('z', 36_967);
		var model = new MudSharp.Models.GameItemComponentProto
		{
			Id = 42, Name = "Detail fixture", EditableItem = new MudSharp.Models.EditableItem(),
			Definition = new XElement("Definition", new XElement("KeywordDescriptions", new XElement("Entry",
				new XElement("Keyword", "makers mark"), new XElement("Prose", prose)))).ToString()
		};
		var constructor = typeof(KeywordDescriptionsGameItemComponentProto).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,
			null, [typeof(MudSharp.Models.GameItemComponentProto), typeof(IFuturemud)], null)!;
		var prototype = (KeywordDescriptionsGameItemComponentProto)constructor.Invoke([model, Mock.Of<IFuturemud>()]);
		var component = prototype.CreateNew(Mock.Of<IGameItem>(), temporary: true);
		var copied = component.Copy(Mock.Of<IGameItem>(), true);
		Assert.AreEqual(prose, ((IKeywordDescriptionProvider)copied).KeywordDescriptions.Single().Description);
		var xml = (string)typeof(KeywordDescriptionsGameItemComponentProto).GetMethod("SaveToXml", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(prototype, null)!;
		Assert.AreEqual(prose, XElement.Parse(xml).Element("KeywordDescriptions")!.Element("Entry")!.Element("Prose")!.Value);
	}

	private sealed record TestProvider(IReadOnlyCollection<KeywordDescription> KeywordDescriptions) : IKeywordDescriptionProvider;
}

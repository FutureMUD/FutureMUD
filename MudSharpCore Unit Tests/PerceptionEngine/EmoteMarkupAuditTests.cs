using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Framework;
using MudSharp.Form.Shape;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

#nullable enable

namespace MudSharp_Unit_Tests.PerceptionEngine;

[TestClass]
public class EmoteMarkupAuditTests
{
	[TestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void OptionalTarget_SaveLoad_PreservesChosenBranchAndViewerRendering(bool present)
	{
		var source = new Mock<IPerceiver>();
		source.Setup(x => x.Id).Returns(1);
		source.Setup(x => x.FrameworkItemType).Returns("Character");
		source.Setup(x => x.IsSelf(source.Object)).Returns(true);
		var observer = Mock.Of<IPerceiver>();
		source.Setup(x => x.HowSeen(observer, false, DescriptionType.Short, true, PerceiveIgnoreFlags.IgnoreLayers))
			.Returns("the source");
		var world = new Mock<IFuturemud>();
		world.Setup(x => x.GetPerceivable("Character", 1)).Returns(source.Object);
		var emote = new Emote("$?0|beside $1|without $1|$", source.Object,
			new IPerceivable[] { present ? source.Object : null!, source.Object });
		Assert.IsTrue(emote.Valid, emote.ErrorMessage);
		var loaded = Emote.LoadEmote(emote.SaveToXml(), world.Object, source.Object);
		Assert.IsTrue(loaded.Valid, loaded.ErrorMessage);
		foreach (var candidate in new[] { emote, loaded })
		{
			Assert.AreEqual(present ? "beside you" : "without you", candidate.ParseFor(source.Object));
			Assert.AreEqual(present ? "beside the source" : "without the source", candidate.ParseFor(observer));
		}
	}

	[TestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void OptionalTarget_SaveLoad_RetainsAllNonNullDependencies(bool present)
	{
		var source = Mock.Of<IPerceiver>();
		var tested = Mock.Of<IPerceivable>(x => x.Id == 1 && x.FrameworkItemType == "Item");
		var nested = Mock.Of<IPerceivable>(x => x.Id == 2 && x.FrameworkItemType == "Item");
		var world = new Mock<IFuturemud>();
		world.Setup(x => x.GetPerceivable("Item", 1)).Returns(tested);
		world.Setup(x => x.GetPerceivable("Item", 2)).Returns(nested);
		var original = new Emote("$?0|near $1|without $1|$", source,
			new[] { present ? tested : null!, nested });
		var xml = original.SaveToXml();
		var loaded = Emote.LoadEmote(xml, world.Object, source);
		var expected = present ? new[] { tested, nested } : new[] { nested };
		CollectionAssert.AreEquivalent(expected, original.Targets.ToArray());
		CollectionAssert.AreEquivalent(expected, loaded.Targets.ToArray());
		world.Setup(x => x.GetPerceivable("Item", 2)).Returns((IPerceivable)null!);
		Assert.IsFalse(Emote.LoadEmote(xml, world.Object, source).Valid);
		if (present)
		{
			world.Setup(x => x.GetPerceivable("Item", 2)).Returns(nested);
			world.Setup(x => x.GetPerceivable("Item", 1)).Returns((IPerceivable)null!);
			Assert.IsFalse(Emote.LoadEmote(xml, world.Object, source).Valid);
		}
	}

	[TestMethod]
	public void EmoteOutput_RawString_ContainsTextOrFormatError()
	{
		var source = Mock.Of<IPerceiver>();
		Assert.AreEqual("literal", new EmoteOutput("literal", source).RawString);
		var malformed = new EmoteOutput("{", source);
		malformed.ParseFor(null!);
		StringAssert.StartsWith(malformed.RawString, "Invalid emote");
	}

	[TestMethod]
	public void OptionalTarget_RecipientFlags_AreForwardedForSelfTarget()
	{
		var source = new Mock<IPerceiver>();
		source.Setup(x => x.IsSelf(source.Object)).Returns(true);
		var target = new Mock<IPerceivable>();
		target.Setup(x => x.HowSeen(source.Object, false, DescriptionType.Short, true,
			PerceiveIgnoreFlags.TrueDescription | PerceiveIgnoreFlags.IgnoreLayers)).Returns("true description");
		var emote = new Emote("$?0|$1||$", source.Object, source.Object, target.Object);
		Assert.AreEqual("true description", emote.ParseFor(source.Object, PerceiveIgnoreFlags.TrueDescription));
	}

	[TestMethod]
	public void EmoteOutput_Copy_PreservesIndependentAssignmentsAndNullViewerFallback()
	{
		var source = Mock.Of<IPerceiver>();
		var viewer = new Mock<IPerceiver>();
		viewer.Setup(x => x.InnerLineFormatLength).Returns(80);
		var original = new EmoteOutput("default", source);
		original.AssignEmote("personal", false, viewer.Object);
		var copy = new EmoteOutput(original);
		original.AssignEmote("other", false, source);
		Assert.AreEqual("Personal.", copy.ParseFor(viewer.Object));
		Assert.AreEqual("Default.", copy.ParseFor(null!));
		Assert.AreEqual("Default.", copy.ParseFor(source));
	}

	[TestMethod]
	public void AppendableOutputs_NoAppendOrNullViewer_RenderDefault()
	{
		var source = Mock.Of<IPerceiver>();
		var prior = new PriorEmoteOutput("default", source);
		var mixed = new MixedEmoteOutput("default", source);
		prior.AssignEmote("personal", false, source);
		mixed.AssignEmote("personal", false, source);
		Assert.AreEqual("Default.", prior.ParseFor(null!));
		Assert.AreEqual("Default.", mixed.ParseFor(null!));
	}
}

#nullable enable

using System;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.NPC.AI;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SpellOwnedCorpseAnimationTests
{
	[TestMethod]
	public void DurablePresentation_BoundsNestedXmlEscapingAndMaximumIdentifiers()
	{
		Assert.IsTrue(MudSharp.Magic.Lifecycle.SpellOwnedCorpseAnimationService.CanPersistPresentation("An ordinary short echo."));
		Assert.IsFalse(MudSharp.Magic.Lifecycle.SpellOwnedCorpseAnimationService.CanPersistPresentation(new string('&', 400)));
	}

	[TestMethod]
	public void RuntimeRetirement_CustodyFreezeRefusesMutationAndRestoresOuterState()
	{
		var item = Mock.Of<IGameItem>();
		using (ForeignCustodyTransferContext.FreezeCustody())
		{
			Assert.ThrowsException<InvalidOperationException>(() => ForeignCustodyTransferContext.EnsureItem(item));
			Assert.ThrowsException<InvalidOperationException>(() => ForeignCustodyTransferContext.EnsureItem(item, destructive: true));
			Assert.ThrowsException<InvalidOperationException>(() => ForeignCustodyTransferContext.EnsureFlushOutsideTransfer());
			Assert.ThrowsException<InvalidOperationException>(() => ForeignCustodyTransferContext.FreezeCustody());
		}
		ForeignCustodyTransferContext.EnsureItem(item);
		ForeignCustodyTransferContext.EnsureFlushOutsideTransfer();
	}

	private static AnimateCorpseSpellEffect Load(XElement xml, IFuturemud world) =>
		(AnimateCorpseSpellEffect)typeof(AnimateCorpseSpellEffect).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,
			null, [typeof(XElement), typeof(IMagicSpell)], null)!.Invoke([xml, Mock.Of<IMagicSpell>(x => x.Gameworld == world)]);

	[TestMethod]
	public void Animation_DefaultsRemainLegacyWithOriginalEligibilityAndNoNewLifetime()
	{
		var effect = Load(new("Effect"), Mock.Of<IFuturemud>());
		Assert.IsFalse(effect.DurableLifecycle);
		Assert.IsNull(effect.DefinitionError);
		Assert.IsNull(effect.SaveToXml().Element("Lifecycle"));
		Assert.AreEqual("true", effect.SaveToXml().Element("AllowPCs")!.Value);
		Assert.AreEqual("true", effect.SaveToXml().Element("AllowNPCs")!.Value);
		Assert.AreEqual("false", effect.SaveToXml().Element("AllowAdmins")!.Value);
		Assert.IsFalse(((AnimateCorpseSpellEffect)effect.Clone()).DurableLifecycle);
	}

	[DataTestMethod]
	[DataRow("0", "TemporaryCleanup")]
	[DataRow("1", "Permanent")]
	[DataRow("1", "DeathOnExpiry")]
	[DataRow("1", "unknown")]
	public void Animation_InvalidLifecycleIsPreservedAndCannotFallBackToLegacy(string version, string mode)
	{
		var xml = new XElement("Lifecycle", new XAttribute("version", version), new XAttribute("mode", mode), new XElement("Unknown", "retained"));
		var effect = Load(new("Effect", xml), Mock.Of<IFuturemud>());
		Assert.IsNotNull(effect.DefinitionError);
		var clone = (AnimateCorpseSpellEffect)effect.Clone();
		Assert.IsNotNull(clone.DefinitionError);
		Assert.IsTrue(XNode.DeepEquals(xml, clone.SaveToXml().Element("Lifecycle")));
		Assert.IsFalse(clone.TryPrepareApplication(null!, null!, default, default, TimeSpan.Zero, out var application, out _));
		Assert.IsNull(application);
	}

	[DataTestMethod]
	[DataRow("", "60", true)]
	[DataRow("servitor", "outcome+60", true)]
	[DataRow("servitor", "grade*60", false)]
	[DataRow("servitor", "grade*60", true)]
	public void Animation_DefinitionRequiresFamilyPrecheckLifetimeAndReadySelectedAI(string family, string seconds, bool ready)
	{
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.SpellOwnedCorpseAnimations).Returns(Mock.Of<ISpellOwnedCorpseAnimationService>());
		var ais = new All<IArtificialIntelligence>(); ais.Add(Mock.Of<IArtificialIntelligence>(x => x.Id == 12 && x.IsReadyToBeUsed == ready));
		world.SetupGet(x => x.AIs).Returns(ais);
		var effect = Load(new("Effect", new XElement("ArtificialIntelligences", new XElement("AI", 12)),
			new XElement("Lifecycle", new XAttribute("version", 1), new XAttribute("mode", "TemporaryCleanup"),
				new XElement("Family", family), new XElement("Seconds", seconds))), world.Object);
		if (family == "servitor" && seconds == "grade*60" && ready)
		{
			Assert.IsNull(effect.DefinitionError);
			var clone = (AnimateCorpseSpellEffect)effect.Clone(); Assert.IsTrue(clone.DurableLifecycle);
			Assert.AreEqual("grade*60", clone.SaveToXml().Element("Lifecycle")!.Element("Seconds")!.Value);
		}
		else Assert.IsNotNull(effect.DefinitionError);
	}

	[TestMethod]
	public void InstanceRetire_PublicExistingSignatureAndDefaultsRemainAvailable()
	{
		var method = typeof(CharacterInstanceService).GetMethod("Retire", [typeof(ICharacterInstance), typeof(string).MakeByRefType(), typeof(bool), typeof(bool), typeof(bool)]);
		Assert.IsNotNull(method); Assert.IsTrue(method.IsPublic);
		Assert.AreEqual(true, method.GetParameters()[2].DefaultValue);
		Assert.AreEqual(false, method.GetParameters()[3].DefaultValue);
		Assert.AreEqual(true, method.GetParameters()[4].DefaultValue);
	}
}

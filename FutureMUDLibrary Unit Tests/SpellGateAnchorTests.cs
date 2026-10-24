#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SpellGateAnchorTests
{
	[TestMethod]
	public void Load_ExactGateProvenance_RoundTripsAllAuthorityFields()
	{
		var anchor = new SpellGateAnchor(11, 12, 21, 22, 31, Guid.NewGuid(), 41, 51, 60);
		Assert.AreEqual(anchor, SpellGateAnchor.Load(anchor.Save()));
		var stock = ArmageddonGateSpellContent.Create(41, 51);
		var definition = stock.Content.BuildDefinition(61, 62, 63);
		Assert.AreEqual(ArmageddonGateSpellContent.Key, (string?)definition.Element("StockIdentity"));
		Assert.AreEqual("creategate", (string?)definition.Element("Effects")?.Element("Effect")?.Attribute("type"));
		Assert.AreEqual(41L, (long?)definition.Descendants("AutobuilderTemplate").Single());
		Assert.AreEqual(51L, (long?)definition.Descendants("UndeadMerit").Single());
	}

	[TestMethod]
	public void Load_ChangedOrAmbiguousGateProvenance_RefusesAuthority()
	{
		var original = new SpellGateAnchor(11, 12, 21, 22, 31, Guid.NewGuid(), 41, 51, 60);
		var future = XElement.Parse(original.Save()); future.SetAttributeValue("version", 2);
		Assert.ThrowsExactly<FormatException>(() => SpellGateAnchor.Load(future.ToString()));
		var duplicate = XElement.Parse(original.Save()); duplicate.Add(new XElement("MarkItem", 31));
		Assert.ThrowsExactly<FormatException>(() => SpellGateAnchor.Load(duplicate.ToString()));
		Assert.ThrowsExactly<ArgumentException>(() => (original with { SourceRoomId = 21 }).Save());
	}
}

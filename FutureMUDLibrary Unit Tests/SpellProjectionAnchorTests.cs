#nullable enable

using System;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SpellProjectionAnchorTests
{
	[DataTestMethod]
	[DataRow(SpellProjectionKind.SandEffigy)]
	[DataRow(SpellProjectionKind.WalkingShadow)]
	public void Anchor_RetainsExact64BitReferencesAndAuthoredPolicy(SpellProjectionKind kind)
	{
		var policy = new SpellProjectionConfiguration(kind, long.MaxValue, kind == SpellProjectionKind.SandEffigy ? long.MaxValue - 1 : 0,
			120, kind == SpellProjectionKind.SandEffigy ? 0 : 3, 7);
		var anchor = new SpellProjectionAnchor(long.MaxValue - 2, long.MaxValue - 3, long.MaxValue - 4, 0, null, policy);
		Assert.AreEqual(anchor, SpellProjectionAnchor.Load(anchor.Save().ToString()));
	}

	[TestMethod]
	public void Anchor_RefusesUnknownDuplicateAndInvalidNativeFields()
	{
		var valid = new SpellProjectionAnchor(1, 2, 3, 0, null, new(SpellProjectionKind.WalkingShadow, 4, 0, 60, 2, 0));
		foreach (var mutation in new Action<XElement>[] { x => x.SetAttributeValue("version", 2), x => x.SetAttributeValue("body", 0),
			x => x.Element("Policy")!.Add(new XElement("GuessedCharacterId", 12)),
			x => x.Element("Policy")!.Add(new XElement("Plane", 123)), x => x.SetAttributeValue("point", "NaN"),
			x => { x.Attribute("body")!.Remove(); x.SetAttributeValue("GuessedBody", 2); }, x => x.SetAttributeValue("layer", 999) })
		{
			var xml = valid.Save(); mutation(xml);
			Assert.ThrowsException<FormatException>(() => SpellProjectionAnchor.Load(xml.ToString()));
		}
	}

	[DataTestMethod]
	[DataRow(double.NaN, 2, 1.0)]
	[DataRow(double.PositiveInfinity, 2, 1.0)]
	[DataRow(0.0, 2, 1.0)]
	[DataRow(13000.0, 2, 1.0)]
	[DataRow(60.0, -1, 1.0)]
	[DataRow(60.0, 33, 1.0)]
	[DataRow(60.0, 2, double.NaN)]
	[DataRow(60.0, 2, 1001.0)]
	public void Configuration_RejectsUnboundedDurationRangeOrDamage(double seconds, int range, double damage)
		=> Assert.IsNotNull(new SpellProjectionConfiguration(SpellProjectionKind.WalkingShadow, 1, 0, seconds, range, damage).Error(7));
}

#nullable enable
using System;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PierceContentExtractionTests
{
	// Golden shape and descriptive hash come from cleared a7440194, before extraction.
	[TestMethod]
	[DataRow(11L, 12L, 13L)]
	[DataRow(111L, 112L, 0L)]
	public void PureAndNativeDefinitionsMatchTheClearedAccumulationShape(long resource, long cost, long filter)
	{
		var expected = ArmageddonUtilitySpellContent.Definition("arm.spell.pierce_concealment", "character", resource, cost, filter, 30, 7,
			new XElement("Effect", new XAttribute("type", "detectinvisible"),
				XElement.Parse("<LifetimePolicy version='1' mode='accumulate' group='armageddon.detect_invisibility' unitSeconds='600' maximumUnits='48' retainStrongestGrade='true'/>")));
		var content = ArmageddonReviewedPierceContent.PierceConcealment();
		var native = (XElement)typeof(ArmageddonPierceConcealmentStock).GetMethod("Definition", BindingFlags.NonPublic | BindingFlags.Static)!.Invoke(null, [resource, cost, filter])!;
		Assert.AreEqual(expected.ToString(), content.BuildDefinition(resource, cost, filter).ToString());
		Assert.AreEqual(expected.ToString(), native.ToString());
	}
	[TestMethod] public void ExtractedDescriptionAndFactoryConstantsRetainClearedDefaults()
	{
		var content = ArmageddonReviewedPierceContent.PierceConcealment();
		Assert.AreEqual("8B14FFCA92D59D98E124ED19AFC4D63DAE7EAB0F5CCAC9BE426D6A2AF0B49249", Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(content.Description))));
		Assert.AreEqual("3000*grade", content.DurationFormula); Assert.AreEqual(7.0, content.MinimumEnergy);
		Assert.AreEqual("return lowercase(@caster.location.terrain.name) != \"silt\"", content.EligibilitySource);
		Assert.AreEqual("armageddon.detect_invisibility", ArmageddonPierceConcealmentStock.LifetimeGroup);
		var row = content.SpellRow(1, 2, 3); Assert.IsTrue(row.AppliedEffectsAreExclusive); Assert.IsFalse(row.ScrollInscriptionAllowed);
	}
}

#nullable enable

using System;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.GameItems;
using MudSharp.Magic;

namespace FutureMUDLibrary_Unit_Tests;

[TestClass]
public class ArmageddonPocketContentTests
{
	private static SpellPocketConfiguration Configuration() => new(9, 2000, SizeCategory.Normal, 300, SpellPocketAccess.Bearer, 4);
	[DataTestMethod]
	[DataRow(1)] [DataRow(2)] [DataRow(3)] [DataRow(4)] [DataRow(5)] [DataRow(6)] [DataRow(7)]
	public void PocketBinding_AllGradesRoundTripWithoutChangingCapacityOrSource(int grade)
	{
		var anchor = new SpellPocketAnchor(37, grade, Configuration());
		var loaded = SpellPocketAnchor.Load(anchor.Save());
		Assert.AreEqual(anchor, loaded); Assert.AreEqual(2000.0 * grade, loaded.Capacity);
	}
	[DataTestMethod]
	[DataRow("version")] [DataRow("unknown")] [DataRow("duplicate")] [DataRow("nested")]
	public void PocketEnvelope_UnknownOrAmbiguousPolicyRefuses(string failure)
	{
		var xml = Configuration().Save();
		switch (failure)
		{
			case "version": xml.SetAttributeValue("version", 2); break;
			case "unknown": xml.Add(new XElement("RoomId", 111)); break;
			case "duplicate": xml.Add(new XElement("Access", "Creator")); break;
			case "nested": xml.Element("CapacityPerGrade")!.Add(new XElement("Id", 12)); break;
		}
		Assert.ThrowsException<FormatException>(() => SpellPocketConfiguration.Load(xml));
	}
	[DataTestMethod]
	[DataRow(0.0)] [DataRow(-1.0)] [DataRow(double.NaN)] [DataRow(double.PositiveInfinity)]
	public void PocketCapacity_NonPositiveOrNonFiniteRefuses(double capacity) =>
		Assert.ThrowsException<ArgumentException>(() => (Configuration() with { CapacityPerGrade = capacity }).Validate());
	[TestMethod]
	public void PocketBinding_InvalidGradeOrIdentityRefuses()
	{
		var origin = new SpellOwnedItemOrigin(Guid.NewGuid(), SpellLifecycleMode.TemporaryCleanup, DateTime.UtcNow, 37);
		var (id, mode, deadline) = origin;
		Assert.AreEqual(origin.LifecycleId, id);
		Assert.AreEqual(origin.Mode, mode);
		Assert.AreEqual(origin.DeadlineUtc, deadline);
		Assert.AreEqual(37L, origin.CreatorId);
		Assert.ThrowsException<ArgumentException>(() => new SpellPocketAnchor(0, 1, Configuration()).Save());
		Assert.ThrowsException<ArgumentException>(() => new SpellPocketAnchor(37, 8, Configuration()).Save());
		Assert.ThrowsException<ArgumentException>(() => (Configuration() with { Access = (SpellPocketAccess)999 }).Validate());
		Assert.ThrowsException<ArgumentException>(() => (Configuration() with { SecondsPerGrade = double.NaN }).Validate());
	}
	[TestMethod]
	public void PocketDuration_FractionalPolicySurvivesNativeTimestampPrecision()
	{
		var configuration = Configuration() with { SecondsPerGrade = 0.1000001 };
		var created = new DateTime(2026, 10, 10, 0, 0, 0, DateTimeKind.Utc);
		for (var grade = 1; grade <= 7; grade++)
		{
			var deadline = created + configuration.DurationForGrade(grade);
			var reloaded = new DateTime(deadline.Ticks / 10 * 10, DateTimeKind.Utc);
			Assert.AreEqual(deadline, reloaded);
			Assert.AreEqual(deadline, created + SpellPocketConfiguration.Load(configuration.Save()).DurationForGrade(grade));
		}
		Assert.AreEqual(TimeSpan.FromMilliseconds(100), configuration.DurationForGrade(1));
		Assert.ThrowsException<ArgumentException>(() => (configuration with { SecondsPerGrade = 0.0000001 }).Validate());
	}
	[TestMethod]
	public void PocketStock_UsesItemTriggerAndNativePaidCreationWithoutPortableDelivery()
	{
		var content = ArmageddonPocketContent.Create(Configuration()); var definition = content.BuildDefinition(3, 5, 0);
		Assert.AreEqual("item", definition.Element("Trigger")!.Attribute("type")!.Value);
		Assert.AreEqual("createpocket", definition.Element("Effects")!.Element("Effect")!.Attribute("type")!.Value);
		Assert.AreEqual("12", definition.Element("ControlledPower")!.Element("Efficiency")!.Attribute("minimum")!.Value);
		Assert.IsFalse(content.SpellRow(1, 2, 3).ScrollInscriptionAllowed);
		Assert.AreEqual(Configuration(), SpellPocketConfiguration.Load(definition.Element("Effects")!.Element("Effect")!.Element("Pocket")!));
	}
}

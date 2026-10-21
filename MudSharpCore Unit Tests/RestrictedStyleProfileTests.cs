#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.Framework.Save;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RestrictedStyleProfileTests
{
	[TestMethod]
	public void OptInStyling_RestrictsRegrowthWhileLegacyProfilesKeepTheirExistingChoices()
	{
		var world = new Mock<IFuturemud>();
		var definition = new Mock<ICharacteristicDefinition>();
		var values = new All<ICharacteristicValue>();
		var current = new Mock<IGrowableCharacteristicValue>();
		current.SetupGet(x => x.Id).Returns(1); current.SetupGet(x => x.Definition).Returns(definition.Object);
		current.SetupGet(x => x.GrowthStage).Returns(0);
		var next = new Mock<IGrowableCharacteristicValue>();
		next.SetupGet(x => x.Id).Returns(2); next.SetupGet(x => x.Definition).Returns(definition.Object);
		next.SetupGet(x => x.GrowthStage).Returns(1);
		values.Add(current.Object); values.Add(next.Object);
		world.SetupGet(x => x.CharacteristicValues).Returns(values);
		var profile = new Mock<ICharacteristicProfile>();
		var restricted = false;
		profile.SetupGet(x => x.RestrictsStyleChanges).Returns(() => restricted);
		profile.SetupGet(x => x.Values).Returns(new ICharacteristicValue[] { current.Object });
		var ethnicity = new Mock<IEthnicity>();
		ethnicity.SetupGet(x => x.CharacteristicChoices).Returns(new Dictionary<ICharacteristicDefinition, ICharacteristicProfile> { [definition.Object] = profile.Object });
		var body = new Mock<IBody>();
		body.SetupGet(x => x.Ethnicity).Returns(ethnicity.Object);
		body.Setup(x => x.GetCharacteristic(definition.Object, null)).Returns(current.Object);
		var actor = TestObjectFactory.CreateUninitialized<MudSharp.Character.Character>();
		typeof(MudSharp.Character.Character).GetProperty(nameof(actor.Body))!.SetValue(actor, body.Object);
		typeof(MudSharp.Character.Character).GetProperty(nameof(actor.Gameworld))!.SetValue(actor, world.Object);
		Assert.AreEqual(2, actor.PossibleStyles(definition.Object).Count());
		restricted = true;
		CollectionAssert.AreEqual(new long[] { 1 }, actor.PossibleStyles(definition.Object).Select(x => x.Id).ToArray());
		restricted = false;
		Assert.AreEqual(2, actor.PossibleStyles(definition.Object).Count());
	}

	[TestMethod]
	public void StandardProfile_XmlSaveReloadRetainsOptInAndDefaultsLegacyToFalse()
	{
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
		var definitions = new All<ICharacteristicDefinition>();
		var definition = new Mock<ICharacteristicDefinition>(); definition.SetupGet(x => x.Id).Returns(7);
		definitions.Add(definition.Object); world.SetupGet(x => x.Characteristics).Returns(definitions);
		world.SetupGet(x => x.CharacteristicValues).Returns(new All<ICharacteristicValue>());
		var row = new MudSharp.Models.CharacteristicProfile { Id = 8, Name = "Native hairlessness", TargetDefinitionId = 7, Type = "Standard", Definition = "<Definition RestrictStyles='true'/>" };
		var profile = CharacteristicProfile.LoadProfile(row, world.Object);
		Assert.IsTrue(profile.RestrictsStyleChanges);
		row.Definition = (string)typeof(CharacteristicProfile).GetMethod("SaveToXml", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(profile, null)!;
		Assert.AreEqual(true, (bool?)XElement.Parse(row.Definition).Attribute("RestrictStyles"));
		Assert.IsTrue(CharacteristicProfile.LoadProfile(row, world.Object).RestrictsStyleChanges);
		row.Definition = "<Definition/>";
		Assert.IsFalse(CharacteristicProfile.LoadProfile(row, world.Object).RestrictsStyleChanges);
	}
}

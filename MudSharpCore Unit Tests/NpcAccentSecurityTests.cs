#nullable enable

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Communication.Language;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.NPC.Templates;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests;

[TestClass]
public class NpcAccentSecurityTests
{
	[DataTestMethod]
	[DataRow("nosuchlanguage")]
	[DataRow("999")]
	public void BuildingCommandAccent_UnknownLanguage_RejectsWithoutMutation(string language)
	{
		var (template, actor, _) = Fixture();
		Assert.IsFalse(template.BuildingCommand(actor.Object, new StringStack($"accent {language} local")));
		Assert.AreEqual(0, template.SelectedAccents.Count);
		Assert.IsFalse(template.Changed);
		Mock.Get(actor.Object.OutputHandler).Verify(x => x.Send("There is no such accent.", true, false), Times.Once);
	}

	[TestMethod]
	public void BuildingCommandAccent_UnknownAccent_RejectsWithoutMutation()
	{
		var (template, actor, _) = Fixture();
		Assert.IsFalse(template.BuildingCommand(actor.Object, new StringStack("accent common nosuchaccent")));
		Assert.AreEqual(0, template.SelectedAccents.Count);
		Assert.IsFalse(template.Changed);
	}

	[TestMethod]
	public void BuildingCommandAccent_KnownLanguageAndAccent_PreservesToggle()
	{
		var (template, actor, accent) = Fixture();
		Assert.IsTrue(template.BuildingCommand(actor.Object, new StringStack("accent common local")));
		CollectionAssert.AreEqual(new[] { accent }, template.SelectedAccents);
		Assert.IsTrue(template.BuildingCommand(actor.Object, new StringStack("accent common local")));
		Assert.AreEqual(0, template.SelectedAccents.Count);
	}

	private static (SimpleNPCTemplate Template, Mock<ICharacter> Actor, IAccent Accent) Fixture()
	{
		var trait = Mock.Of<ITraitDefinition>();
		var language = new Mock<ILanguage>();
		language.SetupGet(x => x.Id).Returns(7);
		language.SetupGet(x => x.Name).Returns("Common");
		language.SetupGet(x => x.LinkedTrait).Returns(trait);
		var accent = new Mock<IAccent>();
		accent.SetupGet(x => x.Id).Returns(8);
		accent.SetupGet(x => x.Name).Returns("Local");
		accent.SetupGet(x => x.Language).Returns(language.Object);
		language.SetupGet(x => x.Accents).Returns([accent.Object]);
		var languages = new All<ILanguage>();
		languages.Add(language.Object);
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.Languages).Returns(languages);
		world.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
		var template = TestObjectFactory.CreateUninitialized<SimpleNPCTemplate>();
		typeof(SavableKeywordedItem).GetProperty(nameof(SavableKeywordedItem.Gameworld))!.SetValue(template, world.Object);
		template.SkillValues = [(trait, 20.0)];
		template.SelectedAccents = [];
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
		return (template, actor, accent.Object);
	}
}

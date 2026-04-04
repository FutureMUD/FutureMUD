using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Disfigurements;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Communication.Language;
using MudSharp.Form.Colour;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class TattooTextSlotTests
{
	[TestMethod]
	public void Tattoo_UsesFallbackTextValuesInDescriptionsAndKeywords()
	{
		var context = CreateTattooTestContext();
		var tattoo = new Tattoo(context.Template.Object, context.Gameworld.Object, context.Tattooist.Object, 75.0,
			context.Bodypart.Object, null);
		tattoo.CompletionPercentage = 1.0;

		Assert.AreEqual("Mom", tattoo.TextValues["banner"].Text,
			"Missing text values should be filled from the template slot fallback.");
		Assert.AreEqual("a heart banner reading Mom", tattoo.ShortDescriptionFor(null));
		CollectionAssert.Contains(tattoo.Keywords.ToList(), "Mom",
			"Keyword generation should use the resolved tattoo description rather than raw template markup.");
	}

	[TestMethod]
	public void Tattoo_RendersCustomTextThroughWrittenLanguageParsing()
	{
		var context = CreateTattooTestContext();
		var tattoo = new Tattoo(context.Template.Object, context.Gameworld.Object, context.Tattooist.Object, 75.0,
			context.Bodypart.Object, null,
			[
				new TattooTextValue("banner", context.Language.Object, context.Script.Object,
					WritingStyleDescriptors.None, context.Colour.Object, 25.0, "Forever", "unreadable scrawl")
			]);
		tattoo.CompletionPercentage = 1.0;

		var readable = tattoo.ShortDescriptionFor(context.CreateViewer(canRead: true).Object);
		var unreadable = tattoo.ShortDescriptionFor(context.CreateViewer(canRead: false).Object);

		StringAssert.Contains(readable, "Forever",
			"Readers who know the language and script should see the actual tattoo text.");
		Assert.IsFalse(readable.Contains("$template{banner}"),
			"The rendered tattoo description should not leak raw slot placeholder markup.");
		StringAssert.Contains(unreadable, "unreadable scrawl",
			"Viewers who cannot read the writing should see the alternate text instead.");
	}

	private static TattooTestContext CreateTattooTestContext()
	{
		var languageTrait = new Mock<ITraitDefinition>();
		var language = new Mock<ILanguage>();
		language.SetupGet(x => x.Id).Returns(11);
		language.SetupGet(x => x.Name).Returns("English");
		language.SetupGet(x => x.LinkedTrait).Returns(languageTrait.Object);

		var script = new Mock<IScript>();
		script.SetupGet(x => x.Id).Returns(12);
		script.SetupGet(x => x.Name).Returns("Latin");
		script.SetupGet(x => x.KnownScriptDescription).Returns("Latin");
		script.SetupGet(x => x.UnknownScriptDescription).Returns("unknown script");
		script.SetupGet(x => x.DocumentLengthModifier).Returns(1.0);

		var colour = new Mock<IColour>();
		colour.SetupGet(x => x.Id).Returns(13);
		colour.SetupGet(x => x.Name).Returns("Black");

		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Languages).Returns(CreateCollection(language.Object));
		gameworld.SetupGet(x => x.Scripts).Returns(CreateCollection(script.Object));
		gameworld.SetupGet(x => x.Colours).Returns(CreateCollection(colour.Object));
		gameworld.Setup(x => x.GetStaticInt("DefaultWritingStyleInText")).Returns((int)WritingStyleDescriptors.None);
		gameworld.Setup(x => x.GetStaticLong("DefaultWritingColourInText")).Returns(colour.Object.Id);
		gameworld.Setup(x => x.GetStaticString("DefaultAlternateTextValue")).Returns("unknown writing");

		var slot = new Mock<ITattooTemplateTextSlot>();
		slot.SetupGet(x => x.Name).Returns("banner");
		slot.SetupGet(x => x.MaximumLength).Returns(10);
		slot.SetupGet(x => x.RequiredCustomText).Returns(false);
		slot.SetupGet(x => x.DefaultLanguage).Returns(language.Object);
		slot.SetupGet(x => x.DefaultScript).Returns(script.Object);
		slot.SetupGet(x => x.DefaultStyle).Returns(WritingStyleDescriptors.None);
		slot.SetupGet(x => x.DefaultColour).Returns(colour.Object);
		slot.SetupGet(x => x.DefaultMinimumSkill).Returns(25.0);
		slot.SetupGet(x => x.DefaultText).Returns("Mom");
		slot.SetupGet(x => x.DefaultAlternateText).Returns("decorative text");

		var template = new Mock<ITattooTemplate>();
		template.SetupGet(x => x.Id).Returns(21);
		template.SetupGet(x => x.ShortDescription).Returns("a heart banner reading $template{banner}");
		template.SetupGet(x => x.FullDescription).Returns("A heart banner tattoo reading $template{banner}.");
		template.SetupGet(x => x.TextSlots).Returns([slot.Object]);
		template.Setup(x => x.ResolveDescription(It.IsAny<string>(), It.IsAny<IReadOnlyDictionary<string, ITattooTextValue>>()))
			.Returns<string, IReadOnlyDictionary<string, ITattooTextValue>>((description, textValues) =>
			{
				var value = textValues != null && textValues.TryGetValue("banner", out var customValue)
					? customValue
					: new TattooTextValue(slot.Object);
				var markup = (value as TattooTextValue ?? new TattooTextValue(value.Name, value.Language, value.Script,
					value.Style, value.Colour, value.MinimumSkill, value.Text, value.AlternateText)).ToWritingMarkup();
				return description.Replace("$template{banner}", markup);
			});

		var tattooist = new Mock<ICharacter>();
		tattooist.SetupGet(x => x.Id).Returns(31);

		var bodypart = new Mock<IBodypart>();
		bodypart.SetupGet(x => x.Id).Returns(41);

		return new TattooTestContext(gameworld, template, tattooist, bodypart, language, script, colour, languageTrait);
	}

	private static IUneditableAll<T> CreateCollection<T>(params T[] items)
		where T : class, IFrameworkItem
	{
		var collection = new Mock<IUneditableAll<T>>();
		collection.Setup(x => x.Get(It.IsAny<long>())).Returns<long>(id => items.FirstOrDefault(x => x.Id == id));
		collection.Setup(x => x.GetByName(It.IsAny<string>()))
			.Returns<string>(name => items.FirstOrDefault(x => x.Name == name));
		collection.As<IEnumerable<T>>().Setup(x => x.GetEnumerator()).Returns(() => items.AsEnumerable().GetEnumerator());
		return collection.Object;
	}

	private sealed record TattooTestContext(
		Mock<IFuturemud> Gameworld,
		Mock<ITattooTemplate> Template,
		Mock<ICharacter> Tattooist,
		Mock<IBodypart> Bodypart,
		Mock<ILanguage> Language,
		Mock<IScript> Script,
		Mock<IColour> Colour,
		Mock<ITraitDefinition> LanguageTrait)
	{
		public Mock<IPerceiver> CreateViewer(bool canRead)
		{
			var perceiver = new Mock<IPerceiver>();
			var languageViewer = perceiver.As<IHaveLanguage>();
			languageViewer.SetupGet(x => x.Languages).Returns(canRead ? [Language.Object] : []);
			languageViewer.SetupGet(x => x.Scripts).Returns(canRead ? [Script.Object] : []);
			var trait = new Mock<ITrait>();
			trait.SetupGet(x => x.Value).Returns(canRead ? 100.0 : 0.0);
			languageViewer.Setup(x => x.GetTrait(LanguageTrait.Object)).Returns(trait.Object);
			return perceiver;
		}
	}
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Disfigurements;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Communication.Language;
using MudSharp.Form.Colour;
using MudSharp.Framework;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class TattooTextSlotTests
{
    [TestMethod]
    public void Tattoo_UsesFallbackTextValuesInDescriptionsAndKeywords()
    {
        TattooTestContext context = CreateTattooTestContext();
        Tattoo tattoo = new(context.Template.Object, context.Gameworld.Object, context.Tattooist.Object, 75.0,
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
        TattooTestContext context = CreateTattooTestContext();
        Tattoo tattoo = new(context.Template.Object, context.Gameworld.Object, context.Tattooist.Object, 75.0,
            context.Bodypart.Object, null,
            [
                new TattooTextValue("banner", context.Language.Object, context.Script.Object,
                    WritingStyleDescriptors.None, context.Colour.Object, 25.0, "Forever", "unreadable scrawl")
            ]);
        tattoo.CompletionPercentage = 1.0;

        string readable = tattoo.ShortDescriptionFor(context.CreateViewer(canRead: true).Object);
        string unreadable = tattoo.ShortDescriptionFor(context.CreateViewer(canRead: false).Object);

        StringAssert.Contains(readable, "Forever",
            "Readers who know the language and script should see the actual tattoo text.");
        Assert.IsFalse(readable.Contains("$template{banner}"),
            "The rendered tattoo description should not leak raw slot placeholder markup.");
        StringAssert.Contains(unreadable, "unreadable scrawl",
            "Viewers who cannot read the writing should see the alternate text instead.");
    }

    private static TattooTestContext CreateTattooTestContext()
    {
        Mock<ITraitDefinition> languageTrait = new();
        Mock<ILanguage> language = new();
        language.SetupGet(x => x.Id).Returns(11);
        language.SetupGet(x => x.Name).Returns("English");
        language.SetupGet(x => x.LinkedTrait).Returns(languageTrait.Object);

        Mock<IScript> script = new();
        script.SetupGet(x => x.Id).Returns(12);
        script.SetupGet(x => x.Name).Returns("Latin");
        script.SetupGet(x => x.KnownScriptDescription).Returns("Latin");
        script.SetupGet(x => x.UnknownScriptDescription).Returns("unknown script");
        script.SetupGet(x => x.DocumentLengthModifier).Returns(1.0);

        Mock<IColour> colour = new();
        colour.SetupGet(x => x.Id).Returns(13);
        colour.SetupGet(x => x.Name).Returns("Black");

        Mock<IFuturemud> gameworld = new();
        gameworld.SetupGet(x => x.Languages).Returns(CreateCollection(language.Object));
        gameworld.SetupGet(x => x.Scripts).Returns(CreateCollection(script.Object));
        gameworld.SetupGet(x => x.Colours).Returns(CreateCollection(colour.Object));
        gameworld.Setup(x => x.GetStaticInt("DefaultWritingStyleInText")).Returns((int)WritingStyleDescriptors.None);
        gameworld.Setup(x => x.GetStaticLong("DefaultWritingColourInText")).Returns(colour.Object.Id);
        gameworld.Setup(x => x.GetStaticString("DefaultAlternateTextValue")).Returns("unknown writing");

        Mock<ITattooTemplateTextSlot> slot = new();
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

        Mock<ITattooTemplate> template = new();
        template.SetupGet(x => x.Id).Returns(21);
        template.SetupGet(x => x.ShortDescription).Returns("a heart banner reading $template{banner}");
        template.SetupGet(x => x.FullDescription).Returns("A heart banner tattoo reading $template{banner}.");
        template.SetupGet(x => x.TextSlots).Returns([slot.Object]);
        template.Setup(x => x.ResolveDescription(It.IsAny<string>(), It.IsAny<IReadOnlyDictionary<string, ITattooTextValue>>()))
            .Returns<string, IReadOnlyDictionary<string, ITattooTextValue>>((description, textValues) =>
            {
                ITattooTextValue value = textValues != null && textValues.TryGetValue("banner", out ITattooTextValue customValue)
                    ? customValue
                    : new TattooTextValue(slot.Object);
                string markup = (value as TattooTextValue ?? new TattooTextValue(value.Name, value.Language, value.Script,
                    value.Style, value.Colour, value.MinimumSkill, value.Text, value.AlternateText)).ToWritingMarkup();
                return description.Replace("$template{banner}", markup);
            });

        Mock<ICharacter> tattooist = new();
        tattooist.SetupGet(x => x.Id).Returns(31);

        Mock<IBodypart> bodypart = new();
        bodypart.SetupGet(x => x.Id).Returns(41);

        return new TattooTestContext(gameworld, template, tattooist, bodypart, language, script, colour, languageTrait);
    }

    private static IUneditableAll<T> CreateCollection<T>(params T[] items)
        where T : class, IFrameworkItem
    {
        Mock<IUneditableAll<T>> collection = new();
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
            Mock<IPerceiver> perceiver = new();
            Mock<IHaveLanguage> languageViewer = perceiver.As<IHaveLanguage>();
            languageViewer.SetupGet(x => x.Languages).Returns(canRead ? [Language.Object] : []);
            languageViewer.SetupGet(x => x.Scripts).Returns(canRead ? [Script.Object] : []);
            Mock<ITrait> trait = new();
            trait.SetupGet(x => x.Value).Returns(canRead ? 100.0 : 0.0);
            languageViewer.Setup(x => x.GetTrait(LanguageTrait.Object)).Returns(trait.Object);
            return perceiver;
        }
    }
}

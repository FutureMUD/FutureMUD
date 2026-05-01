using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;
using MudSharp.TimeAndDate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Body.Disfigurements;

public class Tattoo : ITattoo
{
    public Tattoo(XElement root, IFuturemud gameworld)
    {
        Gameworld = gameworld;
        Template = Gameworld.DisfigurementTemplates.Get(long.Parse(root.Element("Template").Value));
        Bodypart = Gameworld.BodypartPrototypes.Get(long.Parse(root.Element("Bodypart").Value));
        _tattooistId = long.Parse(root.Element("Tattooist").Value);
        TattooistSkill = double.Parse(root.Element("TattooistSkill").Value);
        TimeOfInscription = MudDateTime.FromStoredStringOrFallback(root.Element("TimeOfInscription").Value, Gameworld,
            StoredMudDateTimeFallback.CurrentDateTime, "Tattoo", null, Template?.Name, "TimeOfInscription");
        CompletionPercentage = double.Parse(root.Element("CompletionPercentage").Value);
        HasUnreadableCopyPenalty = bool.Parse(root.Element("HasUnreadableCopyPenalty")?.Value ?? "false");
        foreach (XElement element in root.Element("TextValues")?.Elements("TextValue") ?? Enumerable.Empty<XElement>())
        {
            TattooTextValue value = new(element, Gameworld);
            _textValues[value.Name] = value;
        }

        ApplyFallbackTextValues();
    }

    public Tattoo(ITattooTemplate template, IFuturemud gameworld, ICharacter tattooist, double tattooistSkill,
        IBodypart bodypart, MudDateTime timeOfInscription, IEnumerable<ITattooTextValue> textValues = null,
        bool hasUnreadableCopyPenalty = false)
    {
        Template = template;
        Gameworld = gameworld;
        _tattooist = tattooist;
        _tattooistId = tattooist?.Id ?? 0;
        TattooistSkill = tattooistSkill;
        Bodypart = bodypart;
        TimeOfInscription = timeOfInscription;
        HasUnreadableCopyPenalty = hasUnreadableCopyPenalty;
        foreach (ITattooTextValue item in textValues ?? Enumerable.Empty<ITattooTextValue>())
        {
            _textValues[item.Name] = item;
        }

        ApplyFallbackTextValues();
    }

    public XElement SaveToXml()
    {
        return new XElement("Tattoo",
            new XElement("Template", Template.Id),
            new XElement("Bodypart", Bodypart.Id),
            new XElement("Tattooist", _tattooistId),
            new XElement("TattooistSkill", TattooistSkill),
            new XElement("TimeOfInscription", TimeOfInscription.GetDateTimeString()),
            new XElement("CompletionPercentage", CompletionPercentage),
            new XElement("HasUnreadableCopyPenalty", HasUnreadableCopyPenalty),
            new XElement("TextValues",
                from value in _textValues.Values
                select (value as TattooTextValue)?.SaveToXml() ??
                       new TattooTextValue(value.Name, value.Language, value.Script, value.Style, value.Colour,
                           value.MinimumSkill, value.Text, value.AlternateText, value.IsCopiedFromSource,
                           value.WasCopiedWithoutUnderstanding).SaveToXml())
        );
    }

    public IFuturemud Gameworld { get; }

    #region Implementation of IDisfigurement

    public IDisfigurementTemplate Template { get; protected set; }
    public ITattooTemplate TattooTemplate => (ITattooTemplate)Template;

    private string ShortDescriptionInner(IPerceiver voyeur)
    {
        string shortDescription = TattooTemplate.ResolveDescription(TattooTemplate.ShortDescription, _textValues);
        if (CompletionPercentage < 0.05)
        {
            return string.Format(Gameworld.GetStaticString("TattooSdescBarelyStarted"), shortDescription);
        }

        if (CompletionPercentage < 0.2)
        {
            return string.Format(Gameworld.GetStaticString("TattooSdescJustStarted"), shortDescription);
        }

        if (CompletionPercentage < 0.4)
        {
            return string.Format(Gameworld.GetStaticString("TattooSdescFarFromComplete"), shortDescription);
        }

        if (CompletionPercentage < 0.6)
        {
            return string.Format(Gameworld.GetStaticString("TattooSdescHalfwayDone"), shortDescription);
        }

        if (CompletionPercentage < 0.8)
        {
            return string.Format(Gameworld.GetStaticString("TattooSdescMostlyDone"), shortDescription);
        }

        if (CompletionPercentage < 1.0)
        {
            return string.Format(Gameworld.GetStaticString("TattooSdescNearlyDone"), shortDescription);
        }

        return shortDescription;
    }

    private string FullDescriptionInner(IPerceiver voyeur)
    {
        string fullDescription = TattooTemplate.ResolveDescription(TattooTemplate.FullDescription, _textValues);
        string shortDescription = TattooTemplate.ResolveDescription(TattooTemplate.ShortDescription, _textValues);
        if (CompletionPercentage < 0.05)
        {
            return string.Format(Gameworld.GetStaticString("TattooFdescBarelyStarted"), fullDescription, shortDescription);
        }

        if (CompletionPercentage < 0.2)
        {
            return string.Format(Gameworld.GetStaticString("TattooFdescJustStarted"), fullDescription, shortDescription);
        }

        if (CompletionPercentage < 0.4)
        {
            return string.Format(Gameworld.GetStaticString("TattooFdescFarFromComplete"), fullDescription, shortDescription);
        }

        if (CompletionPercentage < 0.6)
        {
            return string.Format(Gameworld.GetStaticString("TattooFdescHalfwayDone"), fullDescription, shortDescription);
        }

        if (CompletionPercentage < 0.8)
        {
            return string.Format(Gameworld.GetStaticString("TattooFdescMostlyDone"), fullDescription, shortDescription);
        }

        if (CompletionPercentage < 1.0)
        {
            return string.Format(Gameworld.GetStaticString("TattooFdescNearlyDone"), fullDescription, shortDescription);
        }

        return fullDescription;
    }

    public string ShortDescription => ShortDescriptionFor(null);
    public string FullDescription => FullDescriptionFor(null);

    #endregion

    #region Implementation of ITattoo

    public IBodypart Bodypart { get; protected set; }
    private long _tattooistId;
    private ICharacter _tattooist;
    public ICharacter Tattooist => _tattooist ??= Gameworld.TryGetCharacter(_tattooistId, true);

    public double TattooistSkill { get; set; }
    public MudDateTime TimeOfInscription { get; set; }
    public double CompletionPercentage { get; set; }
    public SizeCategory Size => ((ITattooTemplate)Template).Size;
    private readonly Dictionary<string, ITattooTextValue> _textValues = new(StringComparer.InvariantCultureIgnoreCase);
    public IReadOnlyDictionary<string, ITattooTextValue> TextValues => _textValues;
    public bool HasUnreadableCopyPenalty { get; protected set; }

    public string ShortDescriptionFor(IPerceiver voyeur)
    {
        return ShortDescriptionInner(voyeur).SubstituteWrittenLanguage(voyeur, Gameworld);
    }

    public string FullDescriptionFor(IPerceiver voyeur)
    {
        return FullDescriptionInner(voyeur).SubstituteWrittenLanguage(voyeur, Gameworld);
    }

    #region Keywords

    private void ApplyFallbackTextValues()
    {
        foreach (ITattooTemplateTextSlot slot in TattooTemplate.TextSlots)
        {
            if (_textValues.ContainsKey(slot.Name))
            {
                continue;
            }

            _textValues[slot.Name] = new TattooTextValue(slot);
        }
    }

    protected IEnumerable<string> GetKeywordsFromSDesc(string sdesc)
    {
        List<string> keywords = new();
        foreach (string keyword in sdesc.RawText().Split(' ', ','))
        {
            if (keyword.EqualToAny("a", "an", "the"))
            {
                continue;
            }

            if (keyword.Contains('-'))
            {
                keywords.AddRange(keyword.Split('-'));
            }

            keywords.Add(keyword);
        }

        return keywords;
    }

    public IEnumerable<string> Keywords => GetKeywordsFromSDesc(ShortDescriptionFor(null));

    public IEnumerable<string> GetKeywordsFor(IPerceiver voyeur)
    {
        return GetKeywordsFromSDesc(ShortDescriptionFor(voyeur));
    }

    #endregion

    #endregion
}

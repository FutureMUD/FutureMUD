using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Body.Disfigurements;
using MudSharp.Communication.Language;
using MudSharp.Form.Colour;
using MudSharp.Framework;
using MudSharp.GameItems;

namespace MudSharp.Body.Disfigurements;

public class TattooTemplateTextSlot : ITattooTemplateTextSlot
{
	public TattooTemplateTextSlot(string name, int maximumLength, bool requiredCustomText, ILanguage defaultLanguage,
		IScript defaultScript, WritingStyleDescriptors defaultStyle, IColour defaultColour, double defaultMinimumSkill,
		string defaultText, string defaultAlternateText)
	{
		Name = name;
		MaximumLength = maximumLength;
		RequiredCustomText = requiredCustomText;
		DefaultLanguage = defaultLanguage;
		DefaultScript = defaultScript;
		DefaultStyle = defaultStyle;
		DefaultColour = defaultColour;
		DefaultMinimumSkill = defaultMinimumSkill;
		DefaultText = defaultText;
		DefaultAlternateText = defaultAlternateText;
	}

	public TattooTemplateTextSlot(XElement element, IFuturemud gameworld)
	{
		Name = element.Attribute("name")?.Value ?? throw new ApplicationException("Tattoo text slot was missing a name.");
		MaximumLength = int.Parse(element.Attribute("maxlength")?.Value ?? "0");
		RequiredCustomText = bool.Parse(element.Attribute("required")?.Value ?? "false");
		DefaultLanguage = gameworld.Languages.Get(long.Parse(element.Element("Language")?.Value ?? "0"));
		DefaultScript = gameworld.Scripts.Get(long.Parse(element.Element("Script")?.Value ?? "0"));
		DefaultStyle = (WritingStyleDescriptors)int.Parse(element.Element("Style")?.Value ?? "0");
		DefaultColour = gameworld.Colours.Get(long.Parse(element.Element("Colour")?.Value ?? "0"));
		DefaultMinimumSkill = double.Parse(element.Element("MinimumSkill")?.Value ?? "0.0");
		DefaultText = element.Element("Text")?.Value ?? string.Empty;
		DefaultAlternateText = element.Element("AlternateText")?.Value ?? string.Empty;
	}

	public string Name { get; set; }
	public int MaximumLength { get; set; }
	public bool RequiredCustomText { get; set; }
	public ILanguage DefaultLanguage { get; set; }
	public IScript DefaultScript { get; set; }
	public WritingStyleDescriptors DefaultStyle { get; set; }
	public IColour DefaultColour { get; set; }
	public double DefaultMinimumSkill { get; set; }
	public string DefaultText { get; set; }
	public string DefaultAlternateText { get; set; }

	public XElement SaveToXml()
	{
		return new XElement("TextSlot",
			new XAttribute("name", Name),
			new XAttribute("maxlength", MaximumLength),
			new XAttribute("required", RequiredCustomText),
			new XElement("Language", DefaultLanguage?.Id ?? 0),
			new XElement("Script", DefaultScript?.Id ?? 0),
			new XElement("Style", (int)DefaultStyle),
			new XElement("Colour", DefaultColour?.Id ?? 0),
			new XElement("MinimumSkill", DefaultMinimumSkill),
			new XElement("Text", new XCData(DefaultText ?? string.Empty)),
			new XElement("AlternateText", new XCData(DefaultAlternateText ?? string.Empty))
		);
	}
}

public class TattooTextValue : ITattooTextValue
{
	public TattooTextValue(string name, ILanguage language, IScript script, WritingStyleDescriptors style, IColour colour,
		double minimumSkill, string text, string alternateText, bool isCopiedFromSource = false,
		bool wasCopiedWithoutUnderstanding = false)
	{
		Name = name;
		Language = language;
		Script = script;
		Style = style;
		Colour = colour;
		MinimumSkill = minimumSkill;
		Text = text;
		AlternateText = alternateText;
		IsCopiedFromSource = isCopiedFromSource;
		WasCopiedWithoutUnderstanding = wasCopiedWithoutUnderstanding;
	}

	public TattooTextValue(ITattooTemplateTextSlot slot)
		: this(slot.Name, slot.DefaultLanguage, slot.DefaultScript, slot.DefaultStyle, slot.DefaultColour,
			slot.DefaultMinimumSkill, slot.DefaultText, slot.DefaultAlternateText)
	{
	}

	public TattooTextValue(XElement element, IFuturemud gameworld)
	{
		Name = element.Attribute("name")?.Value ?? throw new ApplicationException("Tattoo text value was missing a name.");
		Language = gameworld.Languages.Get(long.Parse(element.Element("Language")?.Value ?? "0"));
		Script = gameworld.Scripts.Get(long.Parse(element.Element("Script")?.Value ?? "0"));
		Style = (WritingStyleDescriptors)int.Parse(element.Element("Style")?.Value ?? "0");
		Colour = gameworld.Colours.Get(long.Parse(element.Element("Colour")?.Value ?? "0"));
		MinimumSkill = double.Parse(element.Element("MinimumSkill")?.Value ?? "0.0");
		Text = element.Element("Text")?.Value ?? string.Empty;
		AlternateText = element.Element("AlternateText")?.Value ?? string.Empty;
		IsCopiedFromSource = bool.Parse(element.Element("IsCopiedFromSource")?.Value ?? "false");
		WasCopiedWithoutUnderstanding = bool.Parse(element.Element("WasCopiedWithoutUnderstanding")?.Value ?? "false");
	}

	public string Name { get; set; }
	public ILanguage Language { get; set; }
	public IScript Script { get; set; }
	public WritingStyleDescriptors Style { get; set; }
	public IColour Colour { get; set; }
	public double MinimumSkill { get; set; }
	public string Text { get; set; }
	public string AlternateText { get; set; }
	public bool IsCopiedFromSource { get; set; }
	public bool WasCopiedWithoutUnderstanding { get; set; }

	public XElement SaveToXml()
	{
		return new XElement("TextValue",
			new XAttribute("name", Name),
			new XElement("Language", Language?.Id ?? 0),
			new XElement("Script", Script?.Id ?? 0),
			new XElement("Style", (int)Style),
			new XElement("Colour", Colour?.Id ?? 0),
			new XElement("MinimumSkill", MinimumSkill),
			new XElement("Text", new XCData(Text ?? string.Empty)),
			new XElement("AlternateText", new XCData(AlternateText ?? string.Empty)),
			new XElement("IsCopiedFromSource", IsCopiedFromSource),
			new XElement("WasCopiedWithoutUnderstanding", WasCopiedWithoutUnderstanding)
		);
	}

	public string ToWritingMarkup()
	{
		var details = $"{Language?.Id ?? 0},{Script?.Id ?? 0}";
		if (MinimumSkill > 0.0)
		{
			details += $",skill={MinimumSkill.ToString("F2")}";
		}

		if (Style != WritingStyleDescriptors.None)
		{
			foreach (var flag in Style.GetFlags().OfType<WritingStyleDescriptors>().Where(x => x != WritingStyleDescriptors.None))
			{
				details += $",style={flag.Describe()}";
			}
		}

		if (Colour != null)
		{
			details += $",colour={Colour.Id}";
		}

		return $"writing{{{details}}}{{{Text}}}{{{AlternateText}}}";
	}
}

public class SelectedTattoo : ISelectedTattoo
{
	public SelectedTattoo(ITattooTemplate tattoo, IBodypart bodypart, IReadOnlyDictionary<string, ITattooTextValue> textValues)
	{
		Tattoo = tattoo;
		Bodypart = bodypart;
		TextValues = textValues;
	}

	public ITattooTemplate Tattoo { get; }
	public IBodypart Bodypart { get; }
	public IReadOnlyDictionary<string, ITattooTextValue> TextValues { get; }
}

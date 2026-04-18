using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace MudSharp.Character.Name;

public class PersonalName : FrameworkItem, IPersonalName
{
    protected List<NameElement> NameElements = new();

    public PersonalName(XElement root, IFuturemud gameworld)
    {
        Gameworld = gameworld;
        Culture = Gameworld.NameCultures.Get(
            Convert.ToInt64(
                root.Attribute("culture")?.Value ??
                throw new ApplicationException("Invalid NameCulture in PersonalName")));
        foreach (XElement element in root.Elements("Element"))
        {
            NameElements.Add(
                new NameElement((NameUsage)Enum.Parse(typeof(NameUsage), element.Attribute("usage").Value),
                    element.Value));
        }
    }

    public PersonalName(INameCulture culture, XElement root)
    {
        Gameworld = culture.Gameworld;
        Culture = culture;
        foreach (XElement element in root.Elements("Element"))
        {
            NameElements.Add(
                new NameElement((NameUsage)Enum.Parse(typeof(NameUsage), element.Attribute("usage").Value),
                    element.Value));
        }
    }

    public PersonalName(INameCulture culture, Dictionary<NameUsage, List<string>> elements, bool nonSaving = false)
    {
        Gameworld = culture.Gameworld;
        Culture = culture;
        foreach (KeyValuePair<NameUsage, List<string>> usage in elements)
        {
            foreach (string element in usage.Value)
            {
                NameElements.Add(new NameElement(usage.Key, element));
            }
        }
    }

    public override string FrameworkItemType => "PersonalName";
    public INameCulture Culture { get; protected set; }

    protected IEnumerable<NameElement> ElementsByUsage(NameUsage usage)
    {
        List<NameElement> elements = NameElements.Where(x => x.Usage == usage).ToList();
        // specificly handle dimunative absence
        if (!elements.Any() && usage == NameUsage.Dimunative)
        {
            return NameElements.Where(x => x.Usage == NameUsage.BirthName);
        }

        return elements;
    }

    private static Regex OptionalElementRegex = new(@"\?(?<which>\w+)\[(?<true>[^\]]*)\](?:\[(?<false>[^\]]*)\])*");

    public string GetName(NameStyle style)
    {
        (string pattern, List<NameUsage> usages) = Culture.NamePattern(style);
        pattern = OptionalElementRegex.Replace(pattern, match =>
        {
            NameUsage usage;
            string which = match.Groups["which"].Value;
            if (int.TryParse(which, out int index))
            {
                if (index >= usages.Count)
                {
                    return "";
                }
                usage = usages.ElementAt(index);
            }
            else
            {
                if (!which.TryParseEnum<NameUsage>(out usage))
                {
                    return "";
                }
            }

            if (NameElements.Any(x => x.Usage == usage))
            {
                return match.Groups["true"].Value;
            }

            return match.Groups["false"].Value;
        });
        return string.Format(pattern,
            usages.Select(
                x =>
                    NameElements.Where(y => y.Usage == x)
                                .Select(y => y.Text.Proper())
                                .ListToString(separator: " ", conjunction: "")).ToArray<object>()
        ).Replace("\"\"", "").NormaliseSpacing().Trim();
    }

    public XElement SaveToXml()
    {
        return new XElement("Name", new XAttribute("culture", Culture.Id),
            from item in NameElements
            select new XElement("Element", new XAttribute("usage", item.Usage), new XCData(item.Text)));
    }

    #region Implementation of IHaveFuturemud

    public IFuturemud Gameworld { get; }

    #endregion
}
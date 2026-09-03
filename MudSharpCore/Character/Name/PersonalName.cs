using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Text.RegularExpressions;
using MudSharp.FutureProg.Variables;

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
    public override string Name => GetName(NameStyle.FullName);
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

    private string GetElements(NameUsage usage)
    {
        return ElementsByUsage(usage)
            .Select(x => x.Text.Proper())
            .ListToString(separator: " ", conjunction: "");
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj is not PersonalName other || Culture?.Id != other.Culture?.Id)
        {
            return false;
        }

        foreach (NameUsage usage in Enum.GetValues<NameUsage>())
        {
            if (!NameElements.Where(x => x.Usage == usage).Select(x => x.Text)
                .SequenceEqual(other.NameElements.Where(x => x.Usage == usage).Select(x => x.Text),
                    StringComparer.InvariantCultureIgnoreCase))
            {
                return false;
            }
        }

        return true;
    }

    public override int GetHashCode()
    {
        HashCode hash = new();
        hash.Add(Culture?.Id ?? 0L);
        foreach (NameUsage usage in Enum.GetValues<NameUsage>())
        {
            hash.Add(usage);
            foreach (string element in NameElements.Where(x => x.Usage == usage).Select(x => x.Text))
            {
                hash.Add(element, StringComparer.InvariantCultureIgnoreCase);
            }
        }

        return hash.ToHashCode();
    }

    public IProgVariable GetProperty(string property)
    {
        return property.ToLowerInvariant() switch
        {
            "culture" => Culture,
            "name" or "fullname" => new TextVariable(GetName(NameStyle.FullName)),
            "given" or "givenname" => new TextVariable(GetName(NameStyle.GivenOnly)),
            "simplefullname" => new TextVariable(GetName(NameStyle.SimpleFull)),
            "affectionate" or "affectionatename" => new TextVariable(GetName(NameStyle.Affectionate)),
            "surname" => new TextVariable(GetName(NameStyle.SurnameOnly)),
            "surnameelement" or "rawsurname" => new TextVariable(GetElements(NameUsage.Surname)),
            "withnickname" or "fullwithnickname" => new TextVariable(GetName(NameStyle.FullWithNickname)),
            "elements" => new CollectionVariable(NameElements.Select(x => x.Text.Proper()).ToList(), ProgVariableTypes.Text),
            "birthname" => new TextVariable(GetElements(NameUsage.BirthName)),
            "diminutive" or "dimunative" => new TextVariable(GetElements(NameUsage.Dimunative)),
            "nickname" => new TextVariable(GetElements(NameUsage.Nickname)),
            "middlename" => new TextVariable(GetElements(NameUsage.MiddleName)),
            "adultname" => new TextVariable(GetElements(NameUsage.AdultName)),
            "childname" => new TextVariable(GetElements(NameUsage.ChildName)),
            "patronym" => new TextVariable(GetElements(NameUsage.Patronym)),
            "matronym" => new TextVariable(GetElements(NameUsage.Matronym)),
            "familygroupname" => new TextVariable(GetElements(NameUsage.FamilyGroupName)),
            "generationname" => new TextVariable(GetElements(NameUsage.GenerationName)),
            "regnalname" => new TextVariable(GetElements(NameUsage.RegnalName)),
            "sacredname" => new TextVariable(GetElements(NameUsage.SacredName)),
            "toponym" => new TextVariable(GetElements(NameUsage.Toponym)),
            "ownername" => new TextVariable(GetElements(NameUsage.OwnerName)),
            _ => throw new NotSupportedException()
        };
    }

    public ProgVariableTypes Type => ProgVariableTypes.PersonalName;
    public object GetObject => this;

    public static void RegisterFutureProgCompiler()
    {
        ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.PersonalName,
            new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
            {
                ["culture"] = ProgVariableTypes.NameCulture,
                ["name"] = ProgVariableTypes.Text,
                ["fullname"] = ProgVariableTypes.Text,
                ["given"] = ProgVariableTypes.Text,
                ["givenname"] = ProgVariableTypes.Text,
                ["simplefullname"] = ProgVariableTypes.Text,
                ["affectionate"] = ProgVariableTypes.Text,
                ["affectionatename"] = ProgVariableTypes.Text,
                ["surname"] = ProgVariableTypes.Text,
                ["surnameelement"] = ProgVariableTypes.Text,
                ["rawsurname"] = ProgVariableTypes.Text,
                ["withnickname"] = ProgVariableTypes.Text,
                ["fullwithnickname"] = ProgVariableTypes.Text,
                ["elements"] = ProgVariableTypes.Text | ProgVariableTypes.Collection,
                ["birthname"] = ProgVariableTypes.Text,
                ["diminutive"] = ProgVariableTypes.Text,
                ["dimunative"] = ProgVariableTypes.Text,
                ["nickname"] = ProgVariableTypes.Text,
                ["middlename"] = ProgVariableTypes.Text,
                ["adultname"] = ProgVariableTypes.Text,
                ["childname"] = ProgVariableTypes.Text,
                ["patronym"] = ProgVariableTypes.Text,
                ["matronym"] = ProgVariableTypes.Text,
                ["familygroupname"] = ProgVariableTypes.Text,
                ["generationname"] = ProgVariableTypes.Text,
                ["regnalname"] = ProgVariableTypes.Text,
                ["sacredname"] = ProgVariableTypes.Text,
                ["toponym"] = ProgVariableTypes.Text,
                ["ownername"] = ProgVariableTypes.Text
            },
            new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
            {
                ["culture"] = "The name culture that formats this personal name",
                ["name"] = "The formatted full name",
                ["fullname"] = "The formatted full name",
                ["given"] = "The formatted given name",
                ["givenname"] = "The formatted given name",
                ["simplefullname"] = "The formatted simple full name",
                ["affectionate"] = "The formatted affectionate form of the name",
                ["affectionatename"] = "The formatted affectionate form of the name",
                ["surname"] = "The formatted surname-only form of the name",
                ["surnameelement"] = "The raw surname element or elements",
                ["rawsurname"] = "An alias for SURNAMEELEMENT",
                ["withnickname"] = "The formatted full name including a nickname when the culture uses one",
                ["fullwithnickname"] = "The formatted full name including a nickname when the culture uses one",
                ["elements"] = "All raw name elements in their stored order",
                ["birthname"] = "The birth-name element or elements",
                ["diminutive"] = "The diminutive element or the birth name when no diminutive is stored",
                ["dimunative"] = "An alias for DIMINUTIVE using the legacy NameUsage spelling",
                ["nickname"] = "The nickname element or elements",
                ["middlename"] = "The middle-name element or elements",
                ["adultname"] = "The adult-name element or elements",
                ["childname"] = "The child-name element or elements",
                ["patronym"] = "The patronym element or elements",
                ["matronym"] = "The matronym element or elements",
                ["familygroupname"] = "The family-group-name element or elements",
                ["generationname"] = "The generation-name element or elements",
                ["regnalname"] = "The regnal-name element or elements",
                ["sacredname"] = "The sacred-name element or elements",
                ["toponym"] = "The toponym element or elements",
                ["ownername"] = "The owner-name element or elements"
            });
    }

    #region Implementation of IHaveFuturemud

    public IFuturemud Gameworld { get; }

    #endregion
}

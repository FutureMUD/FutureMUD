using MudSharp.Construction;

namespace MudSharp.Effects.Concrete;

public class LandmarkEffect : Effect
{
    public LandmarkEffect(IRoom owner, bool isMeetingPlace, string sphere, IFutureProg applicabilityProg = null) : base(owner, applicabilityProg)
    {
        Sphere = sphere;
        IsMeetingPlace = isMeetingPlace;
    }

    private LandmarkEffect(XElement effect, IPerceivable owner) : base(effect, owner)
    {
        XElement root = effect.Element("Effect");
        Sphere = root.Element("Sphere").Value;
        IsMeetingPlace = bool.Parse(root.Element("IsMeetingPlace").Value);
        foreach (XElement item in root.Element("ExtraTexts")?.Elements("ExtraText") ?? Enumerable.Empty<XElement>())
        {
            LandmarkDescriptionTexts.Add((long.Parse(item.Attribute("prog").Value), item.Value));
        }
    }

    public override string Describe(IPerceiver voyeur)
    {
        return $"This location is considered a {(IsMeetingPlace ? "meeting place " : "")}landmark{(string.IsNullOrEmpty(Sphere) ? "" : $" for the {Sphere.TitleCase().ColourName()} sphere")}.";
    }

    protected override string SpecificEffectType { get; } = "Landmark";

    public override bool SavingEffect { get; } = true;

    public static void InitialiseEffectType()
    {
        RegisterFactory("Landmark", (effect, owner) => new LandmarkEffect(effect, owner));
    }

    protected override XElement SaveDefinition()
    {
        return new XElement("Effect",
            new XElement("Sphere", new XCData(Sphere)),
            new XElement("IsMeetingPlace", IsMeetingPlace),
            new XElement("ExtraTexts",
                from item in LandmarkDescriptionTexts
                select new XElement("ExtraText",
                    new XAttribute("prog", item.ProgId),
                    new XCData(item.Text)
                )
            )
        );
    }

    public string Sphere { get; set; }

    public bool IsMeetingPlace { get; set; }

	internal bool IsPublicOnDiscord() => ApplicabilityProg is { } filter
		? AllowsDiscordReader(filter, Owner, null)
		: ApplicabilityProgId == 0;

	internal static bool AllowsDiscordReader(IFutureProg filter, params object[] arguments)
	{
		try
		{
			return filter is not null && filter.ExecuteWithStatus(out var result, arguments) && result is true;
		}
		catch (Exception error)
		{
			// Debug progs can throw when they expect a character. Omit this entry and continue the directory.
			filter.Gameworld.DiscordConnection?.NotifyProgError(filter.Id, filter.FunctionName,
				$"Discord landmark filter failed for a null character ({error.GetType().Name}).");
			return false;
		}
	}

    public List<(long ProgId, string Text)> LandmarkDescriptionTexts { get; } = new();
}

using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.TimeAndDate;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace MudSharp.Body.Disfigurements;

public class Scar : IScar
{
	public Scar(XElement root, IFuturemud gameworld, IRace ownerRace)
	{
		Gameworld = gameworld;
		OwnerRace = ownerRace;
		Bodypart = Gameworld.BodypartPrototypes.Get(long.Parse(root.Element("Bodypart")!.Value));
		TimeOfScarring = MudDateTime.FromStoredStringOrFallback(root.Element("TimeOfScarring")!.Value, Gameworld,
			StoredMudDateTimeFallback.CurrentDateTime, "Scar", null, ownerRace?.Name, "TimeOfScarring");
		BaseShortDescription = root.Element("ShortDescription")?.Value ?? "a scar";
		BaseFullDescription = root.Element("FullDescription")?.Value ?? "A scar is visible here.";
		SizeSteps = int.Parse(root.Element("SizeSteps")?.Value ?? "0");
		Distinctiveness = int.Parse(root.Element("Distinctiveness")?.Value ?? "1");
		OverrideCharacteristicPlain = root.Element("OverrideCharacteristicPlain")?.Value;
		OverrideCharacteristicWith = root.Element("OverrideCharacteristicWith")?.Value;
		DamageType = Enum.TryParse<DamageType>(root.Element("DamageType")?.Value, true, out var damageType)
			? damageType
			: DamageType.Slashing;
		Severity = Enum.TryParse<WoundSeverity>(root.Element("Severity")?.Value, true, out var severity)
			? severity
			: WoundSeverity.Moderate;
		IsSurgical = bool.Parse(root.Element("IsSurgical")?.Value ?? "false");
		if (Enum.TryParse<SurgicalProcedureType>(root.Element("SurgicalProcedureType")?.Value, true,
				out var surgicalProcedureType))
		{
			SurgicalProcedureType = surgicalProcedureType;
		}
	}

	public Scar(
		IFuturemud gameworld,
		IRace ownerRace,
		IBodypart bodypart,
		MudDateTime timeOfScarring,
		string baseShortDescription,
		string baseFullDescription,
		int sizeSteps,
		int distinctiveness,
		string overrideCharacteristicPlain,
		string overrideCharacteristicWith,
		DamageType damageType,
		WoundSeverity severity,
		bool isSurgical,
		SurgicalProcedureType? surgicalProcedureType)
	{
		Gameworld = gameworld;
		OwnerRace = ownerRace;
		Bodypart = bodypart;
		TimeOfScarring = timeOfScarring;
		BaseShortDescription = baseShortDescription;
		BaseFullDescription = baseFullDescription;
		SizeSteps = sizeSteps;
		Distinctiveness = distinctiveness;
		OverrideCharacteristicPlain = overrideCharacteristicPlain;
		OverrideCharacteristicWith = overrideCharacteristicWith;
		DamageType = damageType;
		Severity = severity;
		IsSurgical = isSurgical;
		SurgicalProcedureType = surgicalProcedureType;
	}

	public Scar(IScar scar, IFuturemud gameworld, IRace ownerRace)
		: this(scar.SaveToXml(), gameworld, ownerRace)
	{
	}

	public XElement SaveToXml()
	{
		return new XElement("Scar",
			new XElement("Bodypart", Bodypart.Id),
			new XElement("TimeOfScarring", TimeOfScarring.GetDateTimeString()),
			new XElement("ShortDescription", BaseShortDescription),
			new XElement("FullDescription", BaseFullDescription),
			new XElement("SizeSteps", SizeSteps),
			new XElement("Distinctiveness", Distinctiveness),
			new XElement("OverrideCharacteristicPlain", OverrideCharacteristicPlain ?? string.Empty),
			new XElement("OverrideCharacteristicWith", OverrideCharacteristicWith ?? string.Empty),
			new XElement("DamageType", DamageType),
			new XElement("Severity", Severity),
			new XElement("IsSurgical", IsSurgical),
			new XElement("SurgicalProcedureType", SurgicalProcedureType?.ToString() ?? string.Empty)
		);
	}

	public IFuturemud Gameworld { get; }
	public IRace OwnerRace { get; }
	public string BaseShortDescription { get; }
	public string BaseFullDescription { get; }
	public string OverrideCharacteristicPlain { get; }
	public string OverrideCharacteristicWith { get; }

	public IDisfigurementTemplate Template => null;

	public string ShortDescription => Freshness switch
	{
		ScarFreshness.Fresh => string.Format(Gameworld.GetStaticString("ScarSDescFresh"), BaseShortDescription),
		ScarFreshness.Recent => string.Format(Gameworld.GetStaticString("ScarSDescRecent"), BaseShortDescription),
		ScarFreshness.Old => string.Format(Gameworld.GetStaticString("ScarSDescOld"), BaseShortDescription),
		_ => throw new ApplicationException("Unknown ScarFreshness type in Scar.ShortDescription")
	};

	public string FullDescription => Freshness switch
	{
		ScarFreshness.Fresh => string.Format(Gameworld.GetStaticString("ScarFDescFresh"), BaseFullDescription,
			BaseShortDescription),
		ScarFreshness.Recent => string.Format(Gameworld.GetStaticString("ScarFDescRecent"), BaseFullDescription,
			BaseShortDescription),
		ScarFreshness.Old => string.Format(Gameworld.GetStaticString("ScarFDescOld"), BaseFullDescription,
			BaseShortDescription),
		_ => throw new ApplicationException("Unknown ScarFreshness type in Scar.FullDescription")
	};

	public SizeCategory Size => OwnerRace.ModifiedSize(Bodypart).ChangeSize(SizeSteps);
	public IBodypart Bodypart { get; }

	public ScarFreshness Freshness
	{
		get
		{
			var interval = TimeOfScarring.Calendar.CurrentDateTime - TimeOfScarring;
			if (interval.TotalDays > Gameworld.GetStaticDouble("ScarDaysForOld"))
			{
				return ScarFreshness.Old;
			}

			return interval.TotalDays > Gameworld.GetStaticDouble("ScarDaysForRecent")
				? ScarFreshness.Recent
				: ScarFreshness.Fresh;
		}
	}

	public MudDateTime TimeOfScarring { get; }
	public int Distinctiveness { get; }
	public int SizeSteps { get; }
	public bool HasSpecialScarCharacteristicOverride => !string.IsNullOrWhiteSpace(OverrideCharacteristicPlain);
	public bool IsSurgical { get; }
	public DamageType DamageType { get; }
	public WoundSeverity Severity { get; }
	public SurgicalProcedureType? SurgicalProcedureType { get; }

	public string SpecialScarCharacteristicOverride(bool withForm)
	{
		return withForm ? OverrideCharacteristicWith : OverrideCharacteristicPlain;
	}

	protected IEnumerable<string> GetKeywordsFromSDesc(string sdesc)
	{
		List<string> keywords = [];
		foreach (var keyword in sdesc.RawText().Split(' ', ','))
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

	public IEnumerable<string> Keywords => GetKeywordsFromSDesc(ShortDescription);

	public IEnumerable<string> GetKeywordsFor(IPerceiver voyeur)
	{
		return GetKeywordsFromSDesc(ShortDescription.SubstituteWrittenLanguage(voyeur, Gameworld));
	}
}

using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.TimeAndDate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character.Heritage;

namespace MudSharp.Body.Disfigurements;

public class Scar : IScar
{
	public Scar(XElement root, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		Template = Gameworld.DisfigurementTemplates.Get(long.Parse(root.Element("Template").Value));
		Bodypart = Gameworld.BodypartPrototypes.Get(long.Parse(root.Element("Bodypart").Value));
		TimeOfScarring = new MudDateTime(root.Element("TimeOfScarring").Value, Gameworld);
	}

	public Scar(IScarTemplate template, IFuturemud gameworld, ICharacter owner, IBodypart bodypart,
		MudDateTime timeOfScarring)
	{
		Template = template;
		Gameworld = gameworld;
		Bodypart = bodypart;
		TimeOfScarring = timeOfScarring;
		OwnerRace = owner.Race;
	}

	public XElement SaveToXml()
	{
		return new XElement("Scar",
			new XElement("Template", Template.Id),
			new XElement("Bodypart", Bodypart.Id),
			new XElement("TimeOfScarring", TimeOfScarring.GetDateTimeString())
		);
	}

	public IFuturemud Gameworld { get; }

	#region Implementation of IDisfigurement

	public IRace OwnerRace { get; }
	public IDisfigurementTemplate Template { get; protected set; }
	public IScarTemplate ScarTemplate => (IScarTemplate)Template;

	public string ShortDescription
	{
		get
		{
			switch (Freshness)
			{
				case ScarFreshness.Fresh:
					return string.Format(Gameworld.GetStaticString("ScarSDescFresh"), Template.ShortDescription);
				case ScarFreshness.Recent:
					return string.Format(Gameworld.GetStaticString("ScarSDescRecent"), Template.ShortDescription);
				case ScarFreshness.Old:
					return string.Format(Gameworld.GetStaticString("ScarSDescOld"), Template.ShortDescription);
				default:
					throw new ApplicationException("Unknown ScarFreshness type in Scar.ShortDescription");
			}
		}
	}

	public string FullDescription
	{
		get
		{
			switch (Freshness)
			{
				case ScarFreshness.Fresh:
					return string.Format(Gameworld.GetStaticString("ScarFDescFresh"), Template.FullDescription,
						Template.ShortDescription);
				case ScarFreshness.Recent:
					return string.Format(Gameworld.GetStaticString("ScarFDescRecent"), Template.FullDescription,
						Template.ShortDescription);
				case ScarFreshness.Old:
					return string.Format(Gameworld.GetStaticString("ScarFDescOld"), Template.FullDescription,
						Template.ShortDescription);
				default:
					throw new ApplicationException("Unknown ScarFreshness type in Scar.FullDescription");
			}
		}
	}

	public SizeCategory Size => OwnerRace.ModifiedSize(Bodypart).ChangeSize(ScarTemplate.SizeSteps);

	public int Distinctiveness => ScarTemplate.Distinctiveness;

	#endregion

	#region Implementation of IScar

	public IBodypart Bodypart { get; protected set; }

	public ScarFreshness Freshness
	{
		get
		{
			var interval = TimeOfScarring.Calendar.CurrentDateTime - TimeOfScarring;
			if (interval.TotalDays > Gameworld.GetStaticDouble("ScarDaysForOld"))
			{
				return ScarFreshness.Old;
			}
			else if (interval.TotalDays > Gameworld.GetStaticDouble("ScarDaysForRecent"))
			{
				return ScarFreshness.Recent;
			}
			else
			{
				return ScarFreshness.Fresh;
			}
		}
	}

	public MudDateTime TimeOfScarring { get; protected set; }

	protected IEnumerable<string> GetKeywordsFromSDesc(string sdesc)
	{
		var keywords = new List<string>();
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

	#endregion
}
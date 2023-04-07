using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.TimeAndDate;

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
		TimeOfInscription = new MudDateTime(root.Element("TimeOfInscription").Value, Gameworld);
		CompletionPercentage = double.Parse(root.Element("CompletionPercentage").Value);
	}

	public Tattoo(ITattooTemplate template, IFuturemud gameworld, ICharacter tattooist, double tattooistSkill,
		IBodypart bodypart, MudDateTime timeOfInscription)
	{
		Template = template;
		Gameworld = gameworld;
		_tattooist = tattooist;
		_tattooistId = tattooist.Id;
		TattooistSkill = tattooistSkill;
		Bodypart = bodypart;
		TimeOfInscription = timeOfInscription;
	}

	public XElement SaveToXml()
	{
		return new XElement("Tattoo",
			new XElement("Template", Template.Id),
			new XElement("Bodypart", Bodypart.Id),
			new XElement("Tattooist", _tattooistId),
			new XElement("TattooistSkill", TattooistSkill),
			new XElement("TimeOfInscription", TimeOfInscription.GetDateTimeString()),
			new XElement("CompletionPercentage", CompletionPercentage)
		);
	}

	public IFuturemud Gameworld { get; }

	#region Implementation of IDisfigurement

	public IDisfigurementTemplate Template { get; protected set; }
	public ITattooTemplate TattooTemplate => (ITattooTemplate)Template;

	public string ShortDescription
	{
		get
		{
			if (CompletionPercentage < 0.05)
			{
				return string.Format(Gameworld.GetStaticString("TattooSdescBarelyStarted"), Template.ShortDescription);
			}

			if (CompletionPercentage < 0.2)
			{
				return string.Format(Gameworld.GetStaticString("TattooSdescJustStarted"), Template.ShortDescription);
			}

			if (CompletionPercentage < 0.4)
			{
				return string.Format(Gameworld.GetStaticString("TattooSdescFarFromComplete"),
					Template.ShortDescription);
			}

			if (CompletionPercentage < 0.6)
			{
				return string.Format(Gameworld.GetStaticString("TattooSdescHalfwayDone"), Template.ShortDescription);
			}

			if (CompletionPercentage < 0.8)
			{
				return string.Format(Gameworld.GetStaticString("TattooSdescMostlyDone"), Template.ShortDescription);
			}

			if (CompletionPercentage < 1.0)
			{
				return string.Format(Gameworld.GetStaticString("TattooSdescNearlyDone"), Template.ShortDescription);
			}

			return Template.ShortDescription;
		}
	}

	public string FullDescription
	{
		get
		{
			if (CompletionPercentage < 0.05)
			{
				return string.Format(Gameworld.GetStaticString("TattooFdescBarelyStarted"), Template.FullDescription,
					Template.ShortDescription);
			}

			if (CompletionPercentage < 0.2)
			{
				return string.Format(Gameworld.GetStaticString("TattooFdescJustStarted"), Template.FullDescription,
					Template.ShortDescription);
			}

			if (CompletionPercentage < 0.4)
			{
				return string.Format(Gameworld.GetStaticString("TattooFdescFarFromComplete"), Template.FullDescription,
					Template.ShortDescription);
			}

			if (CompletionPercentage < 0.6)
			{
				return string.Format(Gameworld.GetStaticString("TattooFdescHalfwayDone"), Template.FullDescription,
					Template.ShortDescription);
			}

			if (CompletionPercentage < 0.8)
			{
				return string.Format(Gameworld.GetStaticString("TattooFdescMostlyDone"), Template.FullDescription,
					Template.ShortDescription);
			}

			if (CompletionPercentage < 1.0)
			{
				return string.Format(Gameworld.GetStaticString("TattooFdescNearlyDone"), Template.FullDescription,
					Template.ShortDescription);
			}

			return Template.FullDescription;
		}
	}

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

	#region Keywords

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

	#endregion
}
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.TimeAndDate.Date;
using System;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

public class BodyFormApparentAgeEffect : Effect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("BodyFormApparentAge", (effect, owner) =>
			new BodyFormApparentAgeEffect(effect, owner));
	}

	public BodyFormApparentAgeEffect(IPerceivable owner, MudDate apparentBirthday)
		: base(owner)
	{
		ApparentBirthday = apparentBirthday;
	}

	private BodyFormApparentAgeEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
		var text = root.Element("Effect")?.Element("ApparentBirthday")?.Value;
		if (string.IsNullOrWhiteSpace(text))
		{
			LoadErrors = true;
			ApparentBirthday = FallbackBirthday(owner);
			return;
		}

		try
		{
			ApparentBirthday = MudDate.ParseFromText(text, Gameworld);
		}
		catch (Exception)
		{
			LoadErrors = true;
			ApparentBirthday = FallbackBirthday(owner);
		}
	}

	public MudDate ApparentBirthday { get; }

	private static MudDate FallbackBirthday(IPerceivable owner)
	{
		return owner switch
		{
			IBody body => body.Actor.Birthday,
			ICharacter character => character.Birthday,
			_ => owner.Gameworld.Calendars.First().CurrentDate
		};
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("ApparentBirthday", ApparentBirthday.GetRoundtripString())
		);
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Using an apparent body-form birthday of {ApparentBirthday.Display(CalendarDisplayMode.Short)}.";
	}

	public override bool SavingEffect => true;

	protected override string SpecificEffectType => "BodyFormApparentAge";
}

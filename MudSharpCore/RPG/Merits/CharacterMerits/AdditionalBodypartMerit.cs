using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.RPG.Merits.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character.Heritage;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class AdditionalBodypartMerit : CharacterMeritBase, IAdditionalBodypartsMerit
{
	public AdditionalBodypartMerit(Models.Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		foreach (var element in definition.Element("AddedBodyparts").Elements())
		{
			var bodypart = Gameworld.BodypartPrototypes.Get(long.Parse(element.Attribute("bodypart").Value));
			var genderValue = element.Attribute("gender")?.Value;
			Gender? gender = default;
			if (!string.IsNullOrEmpty(genderValue))
			{
				if (!Utilities.TryParseEnum<Gender>(genderValue, out var parsedGender))
				{
					throw new ApplicationException("Invalid gender in AdditionalBodypartMerit");
				}

				gender = parsedGender;
			}

			IRace race = null;
			var raceValue = element.Attribute("race")?.Value;
			if (!string.IsNullOrEmpty(raceValue))
			{
				race = long.TryParse(raceValue, out var value)
					? Gameworld.Races.Get(value)
					: Gameworld.Races.GetByName(raceValue);
			}

			_addedBodyparts.Add((bodypart, gender, race));
		}

		foreach (var element in definition.Element("RemovedBodyparts").Elements())
		{
			var bodypart = Gameworld.BodypartPrototypes.Get(long.Parse(element.Attribute("bodypart").Value));
			var genderValue = element.Attribute("gender")?.Value;
			Gender? gender = default;
			if (!string.IsNullOrEmpty(genderValue))
			{
				if (!Utilities.TryParseEnum<Gender>(genderValue, out var parsedGender))
				{
					throw new ApplicationException("Invalid gender in AdditionalBodypartMerit");
				}

				gender = parsedGender;
			}

			IRace race = null;
			var raceValue = element.Attribute("race")?.Value;
			if (!string.IsNullOrEmpty(raceValue))
			{
				race = long.TryParse(raceValue, out var value)
					? Gameworld.Races.Get(value)
					: Gameworld.Races.GetByName(raceValue);
			}

			_removedBodyparts.Add((bodypart, gender, race));
		}
	}

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Additional Bodyparts",
			(merit, gameworld) => new AdditionalBodypartMerit(merit, gameworld));
	}

	private readonly List<(IBodypart Bodypart, Gender? Gender, IRace Race)> _removedBodyparts = new();
	private readonly List<(IBodypart Bodypart, Gender? Gender, IRace Race)> _addedBodyparts = new();

	public IEnumerable<IBodypart> AddedBodyparts(ICharacter character)
	{
		return _addedBodyparts
		       .Where(x => (!x.Gender.HasValue || character.Gender.Enum == x.Gender.Value) &&
		                   (x.Race == null || character.Race.SameRace(x.Race)))
		       .Select(x => x.Bodypart)
		       .Distinct()
		       .ToList();
	}

	public IEnumerable<IBodypart> RemovedBodyparts(ICharacter character)
	{
		return _removedBodyparts
		       .Where(x => (!x.Gender.HasValue || character.Gender.Enum == x.Gender.Value) &&
		                   (x.Race == null || character.Race.SameRace(x.Race)))
		       .Select(x => x.Bodypart)
		       .Distinct()
		       .ToList();
	}
}
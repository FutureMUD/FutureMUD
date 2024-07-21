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
#nullable enable
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

	private AdditionalBodypartMerit(IFuturemud gameworld, string name) : base(gameworld, name, "Additional Bodyparts", "$0 have|has additional bodyparts")
	{
		DoDatabaseInsert();
	}

	private AdditionalBodypartMerit()
	{

	}

	/// <inheritdoc />
	protected override XElement SaveSubtypeDefinition(XElement root)
	{
		root.Add(new XElement("AddedBodyparts", 
			from item in _addedBodyparts
			select new XElement("Bodypart",
				new XAttribute("bodypart", item.Bodypart.Id),
				new XAttribute("gender", item.Gender.HasValue ? (int)item.Gender.Value : ""),
				new XAttribute("race", item.Race?.Id ?? 0)
			)
		));
		root.Add(new XElement("RemovedBodyparts",
			from item in _removedBodyparts
			select new XElement("Bodypart",
				new XAttribute("bodypart", item.Bodypart.Id),
				new XAttribute("gender", item.Gender.HasValue ? (int)item.Gender.Value : ""),
				new XAttribute("race", item.Race?.Id ?? 0)
			)
		));
		return root;
	}

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Additional Bodyparts",
			(merit, gameworld) => new AdditionalBodypartMerit(merit, gameworld));
		MeritFactory.RegisterBuilderMeritInitialiser("Additional Bodyparts", (gameworld,name) => new AdditionalBodypartMerit(gameworld, name));
		MeritFactory.RegisterMeritHelp(
			"Additional Bodyparts",
			"Adds or removes bodyparts from a character",
			new AdditionalBodypartMerit().HelpText);
	}

	/// <inheritdoc />
	protected override void SubtypeShow(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine();
		sb.AppendLine("Added Bodyparts:");
		sb.AppendLine();
		sb.AppendLine(StringUtilities.GetTextTable(
			from part in _addedBodyparts
			select new List<string>
			{
				part.Bodypart.Id.ToString("N0", actor),
				part.Bodypart.Name,
				part.Gender?.DescribeEnum() ?? "Any",
				part.Race?.Name ?? "Any"
			},
			new List<string>
			{
				"Part ID",
				"Part Name",
				"Gender",
				"Race"
			},
			actor,
			Telnet.Yellow));

		sb.AppendLine();
		sb.AppendLine("Removed Bodyparts:");
		sb.AppendLine();
		sb.AppendLine(StringUtilities.GetTextTable(
			from part in _removedBodyparts
			select new List<string>
			{
				part.Bodypart.Id.ToString("N0", actor),
				part.Bodypart.Name,
				part.Gender?.DescribeEnum() ?? "Any",
				part.Race?.Name ?? "Any"
			},
			new List<string>
			{
				"Part ID",
				"Part Name",
				"Gender",
				"Race"
			},
			actor,
			Telnet.Yellow));
	}

	/// <inheritdoc />
	protected override string SubtypeHelp => @"
	#3addpart <part>#0 - adds or removes a bodypart that the merit adds to the character
	#3addpart <part> [<gender>] [<race>]#0 - adds or sets a part to be only added for specific races or genders
	#3rempart <part>#0 - adds or removes a bodypart that the merit removes from the character
	#3rempart <part> [<gender>] [<race>]#0 - adds or sets a part to be only removed from specific races or genders

	#6Note: gender and race argument can be in either order and are both optional for above commands#0";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "addpart":
				return BuildingCommandPart(actor, command, true);
			case "rempart":
			case "removepart":
				return BuildingCommandPart(actor, command, false);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandPart(ICharacter actor, StringStack command, bool add)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which bodypart do you want set as an {(add ? "additional" : "removed")} bodypart?");
			return false;
		}

		var part = Gameworld.BodyPrototypes.SelectMany(x => x.AllBodypartsBonesAndOrgans).GetByIdOrName(command.PopSpeech());
		if (part is null)
		{
			actor.OutputHandler.Send($"There is no bodypart identified by the text {command.Last.ColourCommand()}.");
			return false;
		}

		Gender? gender = null;
		IRace? race = null;
		var removeIfAlready = false;
		if (command.IsFinished)
		{
			removeIfAlready = true;
			
		}
		else
		{
			while (!command.IsFinished)
			{
				var cmd = command.PopSpeech();
				if (cmd.TryParseEnum(out Gender parsedGender))
				{
					gender = parsedGender;
					continue;
				}

				race = Gameworld.Races.GetByIdOrName(cmd);
				if (race is null)
				{
					actor.OutputHandler.Send($"The text {cmd.ColourCommand()} is neither a valid gender or a valid race.");
					return false;
				}
			}
		}

		if (removeIfAlready)
		{
			if (add)
			{
				if (_addedBodyparts.Any(x => x.Bodypart == part))
				{
					_addedBodyparts.RemoveAll(x => x.Bodypart == part);
					Changed = true;
					actor.OutputHandler.Send($"This merit will no longer add the {part.Name.ColourValue()} (#{part.Id.ToString("N0", actor)}) part.");
					return true;
				}

				_addedBodyparts.Add((part, null, null));
				Changed = true;
				return true;
			}

			if (_removedBodyparts.Any(x => x.Bodypart == part))
			{
				_removedBodyparts.RemoveAll(x => x.Bodypart == part);
				Changed = true;
				actor.OutputHandler.Send($"This merit will no longer remove the {part.Name.ColourValue()} (#{part.Id.ToString("N0", actor)}) part.");
				return true;
			}

			_removedBodyparts.Add((part, null, null));
			Changed = true;
			return true;
		}

		if (add)
		{
			if (_addedBodyparts.Any(x => x.Bodypart == part))
			{
				_addedBodyparts[_addedBodyparts.FindIndex(x => x.Bodypart == part)] = (part, gender, race);
			}
			else
			{
				_addedBodyparts.Add((part, gender, race));
			}

			Changed = true;
			actor.OutputHandler.Send($"This merit will now add the {part.Name.ColourValue()} (#{Id.ToString("N0", actor)}) part for {(gender is not null ? $"the {gender.DescribeEnum()} gender".ColourValue() : $"any gender".ColourValue())} and {(race is not null ? $"the {race.Name.ColourValue()}".ColourValue() : $"any race".ColourValue())}");
			return true;
		}

		if (_removedBodyparts.Any(x => x.Bodypart == part))
		{
			_removedBodyparts[_removedBodyparts.FindIndex(x => x.Bodypart == part)] = (part, gender, race);
		}
		else
		{
			_removedBodyparts.Add((part, gender, race));
		}

		Changed = true;
		actor.OutputHandler.Send($"This merit will now remove the {part.Name.ColourValue()} (#{Id.ToString("N0", actor)}) part for {(gender is not null ? $"the {gender.DescribeEnum()} gender".ColourValue() : $"any gender".ColourValue())} and {(race is not null ? $"the {race.Name.ColourValue()}".ColourValue() : $"any race".ColourValue())}");
		return true;
	}

	private readonly List<(IBodypart Bodypart, Gender? Gender, IRace? Race)> _removedBodyparts = new();
	private readonly List<(IBodypart Bodypart, Gender? Gender, IRace? Race)> _addedBodyparts = new();

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
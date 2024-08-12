using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using MudSharp.CharacterCreation;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;

namespace MudSharp.Character.Name;

public class NameCulture : SaveableItem, INameCulture
{
	private readonly List<IRandomNameProfile> _randomNameProfiles = new();

	protected IList<NameCultureElement> _nameCultureElements = new List<NameCultureElement>();

	private Regex _nameEntryRegex;

	protected Dictionary<NameStyle, Tuple<string, List<NameUsage>>> _styles =
		new();

	public NameCulture(MudSharp.Models.NameCulture culture, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = culture.Id;
		_name = culture.Name;
		foreach (var item in culture.RandomNameProfiles)
		{
			var profile = new RandomNameProfile(item, this);
			_randomNameProfiles.Add(profile);
			Gameworld.Add(profile);
		}

		LoadFromXml(XElement.Parse(culture.Definition));
	}

	public NameCulture(IFuturemud gameworld, string name)
	{
		Gameworld = gameworld;
		_name = name;
		using (new FMDB())
		{
			var dbitem = new Models.NameCulture
			{
				Name = name,
				Definition =
					new XElement("NameCulture",
						new XElement("Patterns",
							new XElement("Pattern", new XAttribute("Style", 0), new XAttribute("Text", "{0}"),
								new XAttribute("Params", "0")),
							new XElement("Pattern", new XAttribute("Style", 1), new XAttribute("Text", "{0}"),
								new XAttribute("Params", "0")),
							new XElement("Pattern", new XAttribute("Style", 2), new XAttribute("Text", "{0}"),
								new XAttribute("Params", "0")),
							new XElement("Pattern", new XAttribute("Style", 3), new XAttribute("Text", "{0}"),
								new XAttribute("Params", "0")),
							new XElement("Pattern", new XAttribute("Style", 4), new XAttribute("Text", "{0}"),
								new XAttribute("Params", "0")),
							new XElement("Pattern", new XAttribute("Style", 5), new XAttribute("Text", "{0}"),
								new XAttribute("Params", "0"))
						),
						new XElement("Elements",
							new XElement("Element", new XAttribute("Usage", "0"), new XAttribute("MinimumCount", "1"),
								new XAttribute("MaximumCount", "1"), new XAttribute("Name", "Name"),
								new XCData(
									"You are known by a single name, and this name defines who you are to those who know you. You should select a name that is appropriate to your chosen culture."))),
						new XElement("NameEntryRegex", new XCData("^(?<birthname>[w '-]+)$"))
					).ToString()
			};
			FMDB.Context.NameCultures.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
			LoadFromXml(XElement.Parse(dbitem.Definition));
		}
	}

	public NameCulture(INameCulture rhs, string name)
	{
		Gameworld = rhs.Gameworld;
		_name = name;
		using (new FMDB())
		{
			var dbitem = new Models.NameCulture
			{
				Name = name,
				Definition = rhs.SaveToXml().ToString()
			};
			FMDB.Context.NameCultures.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
			LoadFromXml(XElement.Parse(dbitem.Definition));
		}
	}

	public override string FrameworkItemType => "NameCulture";

	public IEnumerable<NameCultureElement> NameCultureElements => _nameCultureElements;
	public IEnumerable<IRandomNameProfile> RandomNameProfiles => _randomNameProfiles;


	public Tuple<string, List<NameUsage>> NamePattern(NameStyle style)
	{
		return _styles[style];
	}

	protected void LoadFromXml(XElement root)
	{
		var element = root.Element("Patterns");
		if (element != null)
		{
			foreach (var sub in element.Elements("Pattern"))
			{
				_styles.Add(
					(NameStyle)Convert.ToInt32(sub.Attribute("Style").Value),
					Tuple.Create(
						sub.Attribute("Text").Value,
						(from val in sub.Attribute("Params").Value.Split(",".ToCharArray())
						 select (NameUsage)Convert.ToInt32(val)).ToList())
				);
			}
		}

		element = root.Element("Elements");
		if (element != null)
		{
			foreach (var sub in element.Elements("Element"))
			{
				_nameCultureElements.Add(new NameCultureElement(sub));
			}
		}

		element = root.Element("NameEntryRegex");
		if (element != null)
		{
			_nameEntryRegex = new Regex(element.Value.Trim(), RegexOptions.IgnoreCase);
		}
	}

	public IPersonalName GetPersonalName(string pattern, bool nonSaving = false)
	{
		try
		{
			var match = _nameEntryRegex.Match(pattern);
			if (!match.Success)
			{
				return null;
			}

			var elements = new Dictionary<NameUsage, List<string>>();
			foreach (NameUsage item in Enum.GetValues(typeof(NameUsage)))
			{
				var name = Enum.GetName(typeof(NameUsage), item).ToLowerInvariant();
				if (match.Groups[name].Length > 0)
				{
					elements.Add(item,
						match.Groups[name].Value.Split(new[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries)
							 .ToList());
				}
			}

			return new PersonalName(this, elements, nonSaving);
		}
		catch
		{
			return null;
		}
	}

	public XElement SaveToXml()
	{
		return new XElement("NameCulture",
			new XElement("Patterns",
				from pattern in _styles
				select new XElement("Pattern", new XAttribute("Style", (int)pattern.Key),
					new XAttribute("Text", pattern.Value.Item1),
					new XAttribute("Params",
						pattern.Value.Item2.Select(x => ((int)x).ToString()).ListToCommaSeparatedValues()))),
			new XElement("NameEntryRegex", new XCData(_nameEntryRegex.ToString())),
			new XElement("Elements", from element in _nameCultureElements select element.SaveToXml())
		);
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.NameCultures.Find(Id);
		dbitem.Name = _name;
		dbitem.Definition = SaveToXml().ToString();
		Changed = false;
	}

	private const string NameCultureBuildingHelp = @"You can use the following options with this subcommand:

	#3name <name>#0 - renames the naming culture	
	#3regex <regex>#0 - sets the regex for the naming culture
	#3pattern <which> <pattern>#0 - sets the naming pattern for a particular context
	#3element add <type>#0 - adds a new naming element of the specified type
	#3element remove <type>#0 - removes a naming element of the specified type
	#3element <type> name <name>#0 - renames an element
	#3element <type> min <min>#0 - sets the minimum number of picks for an element
	#3element <type> max <max>#0 - sets the maximum number of picks for an element
	#3element <type> blurb#0 - drops into an editor for the chargen blurb

#6Note 1 - Regex Rules#0

For the regular expression, each of the name elements should be captured as a group, with the name being the string representation of the name element type (not the name). So for example a BirthName element type should be captured in a group like (?<birthname>...)

#6Note 2 - Patterns#0

In the pattern you use the text #3$ElementType#0 to refer to each of the elements. For example, #3$BirthName#0 for a Birth Name element.

You can also use a pattern in the form #3?ElementType[true][false]#0 to show the true or false text if the name has one or more of the specified element. 
For example, #3?Nickname[ a.k.a ""$Nickname""][]#0";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "element":
				return BuildingCommandElement(actor, command);
			case "pattern":
				return BuildingCommandPattern(actor, command);
			case "regex":
				return BuildingCommandRegex(actor, command);
			default:
				actor.OutputHandler.Send(NameCultureBuildingHelp.SubstituteANSIColour());
				return false;
		}
	}


	private bool BuildingCommandRegex(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to set as the regex for validating names against this scheme?");
			return false;
		}

		var regex = new Regex(command.SafeRemainingArgument, RegexOptions.IgnoreCase);
		try
		{
			regex.Match("");
		}
		catch
		{
			actor.OutputHandler.Send("That is not a valid regex.");
			return false;
		}

		foreach (var group in regex.GetGroupNames())
		{
			if (group.IsInteger())
			{
				continue;
			}

			if (!group.TryParseEnum<NameUsage>(out _))
			{
				actor.OutputHandler.Send(
					$"The capture group called '{group.ColourName()}' is not a valid NameUsage type. Valid types are {Enum.GetNames<NameUsage>().Select(x => x.ColourValue()).ListToString()}.");
				return false;
			}
		}

		foreach (var element in NameCultureElements)
		{
			if (!regex.GetGroupNames().Any(x => element.Usage.DescribeEnum().EqualTo(x)))
			{
				actor.OutputHandler.Send(
					$"Your name validation regex doesn't include a capture for the {element.Usage.DescribeEnum().ColourName()} ({element.Name.ColourName()}) element. All elements must be captured, even if optional.");
				return false;
			}
		}

		_nameEntryRegex = regex;
		Changed = true;
		actor.OutputHandler.Send($"You set the name validation regex to {regex.ToString().ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandPattern(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which pattern do you want to edit? The valid options are {Enum.GetNames<NameStyle>().Select(x => x.ColourValue()).ListToString()}.");
			return false;
		}

		if (!command.PopSpeech().TryParseEnum<NameStyle>(out var style))
		{
			actor.OutputHandler.Send(
				$"That is not a valid pattern type. The valid options are {Enum.GetNames<NameStyle>().Select(x => x.ColourValue()).ListToString()}.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What pattern do you want to set for this name style? Use {Enum.GetNames<NameStyle>().Select(x => $"${x}").ListToString()} for the various name elements.");
			return false;
		}

		var regex = new Regex("\\$(?<enum>[a-zA-Z]+)");
		var pattern = command.SafeRemainingArgument;
		var elements = new List<NameUsage>();
		var index = 0;
		pattern = regex.Replace(pattern, m =>
		{
			if (!m.Groups["enum"].Value.TryParseEnum<NameUsage>(out var usage))
			{
				return m.Groups[0].Value;
			}

			elements.Add(usage);
			return $"{{{index++}}}";
		});

		_styles[style] = Tuple.Create(pattern, elements);
		Changed = true;
		actor.OutputHandler.Send(
			$"The {style.DescribeEnum().ColourValue()} pattern is now {command.SafeRemainingArgument.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandElement(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"You must either specify 'add', 'remove' or the name usage of an element you want to edit.");
			return false;
		}

		var cmd = command.PopSpeech().ToLowerInvariant();
		switch (cmd)
		{
			case "add":
				if (command.IsFinished)
				{
					actor.OutputHandler.Send(
						$"Which element type would you like to add? The valid options are {Enum.GetNames<NameUsage>().Select(x => x.ColourValue()).ListToString()}.");
					return false;
				}

				if (!command.SafeRemainingArgument.TryParseEnum<NameUsage>(out var usage))
				{
					actor.OutputHandler.Send(
						$"That is not a valid NameUsage style. The valid options are {Enum.GetNames<NameUsage>().Select(x => x.ColourValue()).ListToString()}.");
					return false;
				}

				if (_nameCultureElements.Any(XAttribute => XAttribute.Usage == usage))
				{
					actor.OutputHandler.Send(
						$"There is already an element with the {usage.DescribeEnum().ColourValue()} name usage value. Each element must use a unique value.");
					return false;
				}

				_nameCultureElements.Add(new NameCultureElement
				{
					Name = usage.DescribeEnum(true),
					MinimumCount = 0,
					MaximumCount = 1,
					Usage = usage,
					ChargenBlurb = usage.DefaultChargenBlurb()
				});
				Changed = true;
				actor.OutputHandler.Send(
					$"You create a new naming element of type {usage.DescribeEnum(true).ColourValue()}.");
				return true;
			case "remove":
			case "rem":
			case "delete":
			case "del":
				if (command.IsFinished)
				{
					actor.OutputHandler.Send(
						$"Which naming element do you want to remove? The valid options are: {_nameCultureElements.Select(x => x.Usage.DescribeEnum().ColourValue()).ListToString()}.");
					return false;
				}

				if (!command.SafeRemainingArgument.TryParseEnum<NameUsage>(out var remove))
				{
					actor.OutputHandler.Send(
						$"That is not a valid naming element type. The valid options in this context are {_nameCultureElements.Select(x => x.Usage.DescribeEnum().ColourValue()).ListToString()}.");
					return false;
				}

				if (!_nameCultureElements.Any(x => x.Usage == remove))
				{
					actor.OutputHandler.Send(
						$"There are no naming elements of that type. The valid options in this context are {_nameCultureElements.Select(x => x.Usage.DescribeEnum().ColourValue()).ListToString()}.");
					return false;
				}

				_nameCultureElements.Remove(_nameCultureElements.First(x => x.Usage == remove));
				actor.OutputHandler.Send(
					$"You remove the naming element {remove.DescribeEnum()} from this naming culture.");
				Changed = true;
				return true;
		}

		if (!cmd.TryParseEnum<NameUsage>(out var which))
		{
			actor.OutputHandler.Send(
				$"That is not a valid name element type to edit. Your options are {_nameCultureElements.Select(x => x.Usage.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		var element = _nameCultureElements.FirstOrDefault(x => x.Usage == which);
		if (element == null)
		{
			actor.OutputHandler.Send(
				"There is no such naming element to edit. The valid options in this context are {_nameCultureElements.Select(x => x.Usage.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "name":
				if (command.IsFinished)
				{
					actor.OutputHandler.Send("What new name do you want to give to this name element?");
					return false;
				}

				var name = command.SafeRemainingArgument.TitleCase();
				_nameCultureElements[_nameCultureElements.IndexOf(element)] = element with { Name = name };
				Changed = true;
				actor.OutputHandler.Send($"You rename that element to {name.ColourName()}.");
				return true;
			case "min":
				if (command.IsFinished)
				{
					actor.OutputHandler.Send(
						"How many of that name element should be the minimum amount a person can have?");
					return false;
				}

				if (!int.TryParse(command.SafeRemainingArgument, out var min) || min < 0)
				{
					actor.OutputHandler.Send("You must specify a valid number zero or greater.");
					return false;
				}

				_nameCultureElements[_nameCultureElements.IndexOf(element)] = element with { MinimumCount = min };
				Changed = true;
				actor.OutputHandler.Send(
					$"The {element.Name.ColourName()} name element now has a minimum of {min.ToString("N0", actor).ColourValue()} choices.");
				return true;
			case "max":
				if (command.IsFinished)
				{
					actor.OutputHandler.Send(
						"How many of that name element should be the maximum amount a person can have?");
					return false;
				}

				if (!int.TryParse(command.SafeRemainingArgument, out var max) || max < 1)
				{
					actor.OutputHandler.Send("You must specify a valid number one or greater.");
					return false;
				}

				if (max < element.MinimumCount)
				{
					actor.OutputHandler.Send("You cannot have a maximum that is smaller than your minimum.");
					return false;
				}

				_nameCultureElements[_nameCultureElements.IndexOf(element)] = element with { MaximumCount = max };
				Changed = true;
				actor.OutputHandler.Send(
					$"The {element.Name.ColourName()} name element now has a maximum of {max.ToString("N0", actor).ColourValue()} choices.");
				return true;
			case "blurb":
				actor.OutputHandler.Send("Please enter the blurb for character creation for this name element.");
				actor.OutputHandler.Send($"Replacing:\n{element.ChargenBlurb.Wrap(actor.InnerLineFormatLength, "\t")}");
				actor.EditorMode(BlurbPost, BlurbCancel, 1.0, element.ChargenBlurb,
					suppliedArguments: new[] { element });
				return true;
			default:
				actor.OutputHandler.Send(
					$"Which property did you want to edit for that naming element? The valid options are {new List<string> { "name", "min", "max", "blurb" }.Select(x => x.ColourValue()).ListToString()}.");
				return false;
		}
	}

	private void BlurbPost(string text, IOutputHandler handler, object[] args)
	{
		var element = (NameCultureElement)args[0];
		_nameCultureElements[_nameCultureElements.IndexOf(element)] = element with
		{
			ChargenBlurb = text.ProperSentences().SubstituteANSIColour().Trim()
		};
		Changed = true;
		handler.Send("You set the chargen blurb for that element.");
	}

	private void BlurbCancel(IOutputHandler handler, object[] args)
	{
		handler.Send("You decide not to change the chargen blurb.");
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to this name culture?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.NameCultures.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a name culture with that name. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename the {_name.ColourName()} naming culture to {name.ColourName()}.");
		Changed = true;
		_name = name;
		return true;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Name Culture #{Id.ToString("N0", actor)} - {Name.ColourName()}");
		sb.AppendLine($"Regex: {_nameEntryRegex.ToString().ColourCommand()}");
		sb.AppendLine();
		sb.AppendLine("Elements:");
		foreach (var element in NameCultureElements)
		{
			sb.AppendLine(
				$"\t{element.Name.ColourName()} ({element.Usage.DescribeEnum().ColourCommand()}) - Min {element.MinimumCount.ToString("N0", actor).ColourValue()} Max {element.MaximumCount.ToString("N0", actor).ColourValue()}");
		}

		sb.AppendLine();
		sb.AppendLine("Patterns:");
		foreach (var pattern in _styles.OrderBy(x => x.Key))
		{
			sb.AppendLine(
				$"\t{pattern.Key.DescribeEnum().ColourName()}: {string.Format(pattern.Value.Item1, pattern.Value.Item2.Select(x => $"${x.DescribeEnum()}".ColourCommand()).ToArray<object>())}");
		}

		sb.AppendLine();
		sb.AppendLine("Chargen Descriptions:");
		foreach (var element in NameCultureElements)
		{
			sb.AppendLine();
			sb.AppendLine(element.Name.ColourName());
			sb.AppendLine();
			sb.AppendLine(element.ChargenBlurb.Wrap(actor.InnerLineFormatLength, "\t"));
			sb.AppendLine();
		}

		return sb.ToString();
	}
}
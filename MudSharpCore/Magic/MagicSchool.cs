using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;

namespace MudSharp.Magic;

public class MagicSchool : SaveableItem, IMagicSchool
{
	public MagicSchool(IFuturemud gameworld, string name, string verb, string adjective, ANSIColour colour)
	{
		Gameworld = gameworld;
		_name = name;
		SchoolVerb = verb;
		SchoolAdjective = adjective;
		PowerListColour = colour;
		using (new FMDB())
		{
			var dbitem = new Models.MagicSchool
			{
				Name = name,
				SchoolVerb = verb,
				SchoolAdjective = adjective,
				PowerListColour = colour.Name
			};
			FMDB.Context.MagicSchools.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public MagicSchool(MudSharp.Models.MagicSchool school, IFuturemud gameworld)
	{
		_id = school.Id;
		_name = school.Name;
		_parentSchoolId = school.ParentSchoolId;
		Gameworld = gameworld;
		SchoolVerb = school.SchoolVerb;
		SchoolAdjective = school.SchoolAdjective;
		PowerListColour = Telnet.GetColour(school.PowerListColour);
	}

	public MagicSchool(MagicSchool rhs, string newName)
	{
		_name = rhs.Name;
		_parentSchool = rhs._parentSchool;
		_parentSchoolId = rhs._parentSchoolId;
		Gameworld = rhs.Gameworld;
		SchoolAdjective = rhs.SchoolAdjective;
		SchoolVerb = rhs.SchoolVerb;
		PowerListColour = rhs.PowerListColour;
		using (new FMDB())
		{
			var dbitem = new Models.MagicSchool
			{
				Name = _name,
				ParentSchoolId = _parentSchoolId,
				SchoolAdjective = SchoolAdjective,
				SchoolVerb = SchoolVerb,
				PowerListColour = PowerListColour.Name
			};
			FMDB.Context.MagicSchools.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public IMagicSchool Clone(string name)
	{
		return new MagicSchool(this, name);
	}

	#region Overrides of Item

	public override string FrameworkItemType => "MagicSchool";

	#endregion

	#region Implementation of IMagicSchool

	private long? _parentSchoolId;
	private IMagicSchool _parentSchool;

	public IMagicSchool ParentSchool
	{
		get
		{
			if (_parentSchool == null && _parentSchoolId.HasValue)
			{
				_parentSchool = Gameworld.MagicSchools.Get(_parentSchoolId.Value);
			}

			return _parentSchool;
		}
	}

	public bool IsChildSchool(IMagicSchool other)
	{
		return ParentSchool == other || (ParentSchool?.IsChildSchool(other) ?? false);
	}

	/// <summary>
	/// The "Verb" used for the command to invoke this school, e.g. "psy", "magic", "invoke", etc
	/// </summary>
	public string SchoolVerb { get; set; }

	/// <summary>
	/// The adjective used when talking about spells and powers of this school, e.g. psychic, magical, mutant, etc
	/// </summary>
	public string SchoolAdjective { get; set; }

	public ANSIColour PowerListColour { get; set; }

	#endregion

	#region Implementation of IFutureProgVariable

	public ProgVariableTypes Type => ProgVariableTypes.MagicSchool;
	public object GetObject => this;

	public IProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "name":
				return new TextVariable(Name);
			case "id":
				return new NumberVariable(Id);
			case "verb":
				return new TextVariable(SchoolVerb);
			case "adjective":
				return new TextVariable(SchoolAdjective);
			case "colour":
			case "color":
				return new TextVariable(PowerListColour.Name);
			case "parent":
				return ParentSchool;
			case "powers":
				return new CollectionVariable(
					Gameworld.MagicPowers.Where(x => x.School == this).Select(x => x.Name).ToList(),
					ProgVariableTypes.Text);
			case "spells":
				return new CollectionVariable(Gameworld.MagicSpells.Where(x => x.School == this).ToList(),
					ProgVariableTypes.MagicSpell);
			case "capabilities":
				return new CollectionVariable(Gameworld.MagicCapabilities.Where(x => x.School == this).ToList(),
					ProgVariableTypes.MagicCapability);
		}

		throw new ApplicationException("Invalid property requested in MagicSchool.GetProperty");
	}

	private static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "name", ProgVariableTypes.Text },
			{ "id", ProgVariableTypes.Number },
			{ "verb", ProgVariableTypes.Text },
			{ "adjective", ProgVariableTypes.Text },
			{ "color", ProgVariableTypes.Text },
			{ "colour", ProgVariableTypes.Text },
			{ "parent", ProgVariableTypes.MagicSchool },
			{ "powers", ProgVariableTypes.Text | ProgVariableTypes.Collection },
			{ "spells", ProgVariableTypes.MagicSpell | ProgVariableTypes.Collection },
			{ "capabilities", ProgVariableTypes.MagicCapability | ProgVariableTypes.Collection }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "name", "The name of the school of magic" },
			{ "id", "The Id of the school of magic" },
			{ "verb", "The verb (command) used when dealing with this magic school" },
			{ "adjective", "The adjective used to describe magic from this school" },
			{ "color", "An alias for the Colour property" },
			{ "colour", "The name of the colour that is used in displaying things from this school" },
			{ "parent", "The parent magical school, if any. Can be null" },
			{ "powers", "A collection of the names of all powers belonging to this school" },
			{ "spells", "A collection of all the spells defined for this school" },
			{ "capabilities", "A collection of all the magic capabilities for this school" }
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.MagicSchool, DotReferenceHandler(),
			DotReferenceHelp());
	}

	#endregion

	#region Implementation of IEditableItem
	public const string HelpText = @"You can use the following options with this command:

	#3name <name>#0 - renames this school
	#3parent <which>#0 - sets a parent school
	#3parent none#0 - clears a parent school
	#3adjective <which>#0 - sets the adjective used to refer to powers in this school
	#3verb <which>#0 - sets the verb (command) used for this school
	#3colour <which>#0 - sets the ANSI colour for display with this school";

	/// <summary>
	/// Executes a building command based on player input
	/// </summary>
	/// <param name="actor">The avatar of the player doing the command</param>
	/// <param name="command">The command they wish to execute</param>
	/// <returns>Returns true if the command was valid and anything was changed. If nothing was changed or the command was invalid, it returns false</returns>
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "parent":
				return BuildingCommandParent(actor, command);
			case "adjective":
				return BuildingCommandAdjective(actor, command);
			case "verb":
				return BuildingCommandVerb(actor, command);
			case "colour":
			case "color":
				return BuildingCommandColour(actor, command);
			default:
				actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandColour(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What colour should be used for display of powers in this magic school? The options are as follows:\n\n{Telnet.GetColourOptions.Select(x => x.Colour(Telnet.GetColour(x))).ListToLines(true)}");
			return false;
		}

		var colour = Telnet.GetColour(command.SafeRemainingArgument);
		if (colour is null)
		{
			actor.OutputHandler.Send($"That is not a valid colour option. The options are as follows:\n\n{Telnet.GetColourOptions.Select(x => x.Colour(Telnet.GetColour(x))).ListToLines(true)}");
			return false;
		}

		PowerListColour = colour;
		Changed = true;
		actor.OutputHandler.Send($"This magic school will now use the colour {colour.Name.Colour(colour)} for its power list.");
		return true;
	}

	private bool BuildingCommandVerb(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the verb (i.e. command) for interacting with this magic school?");
			return false;
		}

		var cmd = command.SafeRemainingArgument.CollapseString();
		SchoolVerb = cmd;
		Changed = true;
		actor.OutputHandler.Send($"This power will now use the verb {cmd.ColourCommand()} to interact with its spells and powers.");
		return true;
	}

	private bool BuildingCommandAdjective(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the adjective for describing things associated with this magic school?");
			return false;
		}

		var cmd = command.SafeRemainingArgument;
		SchoolAdjective = cmd;
		Changed = true;
		actor.OutputHandler.Send($"This power will now use the adjective {cmd.ColourCommand()} to describe its spells and powers.");
		return true;
	}

	private bool BuildingCommandParent(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either specify another magic school, or use #3none#0 to clear one.".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			_parentSchool = null;
			_parentSchoolId = null;
			Changed = true;
			actor.OutputHandler.Send($"This magic school will no longer have any parent school.");
			return true;
		}

		var school = Gameworld.MagicSchools.GetByIdOrName(command.SafeRemainingArgument);
		if (school is null)
		{
			actor.OutputHandler.Send("There is no such magic school.");
			return false;
		}

		if (school.IsChildSchool(school))
		{
			actor.OutputHandler.Send($"Setting this school's parent to {school.Name.ColourName()} would create a loop relationship, which is not allowed.");
			return false;
		}

		_parentSchool = school;
		_parentSchoolId = school.Id;
		Changed = true;
		actor.OutputHandler.Send($"This magic school's parent is now {school.Name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to rename this school to?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.MagicSchools.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already a magic school called {name.ColourName()}. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename the magic school {_name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	/// <summary>
	/// Shows a builder-specific output representing the IEditableItem
	/// </summary>
	/// <param name="actor">The avatar of the player who wants to view the IEditableItem</param>
	/// <returns>A string representing the item textually</returns>
	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Magic School #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, PowerListColour, Telnet.BoldWhite));
		sb.AppendLine($"Parent: {ParentSchool?.Name.Colour(ParentSchool.PowerListColour) ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine($"Verb: {SchoolVerb.ColourCommand()}");
		sb.AppendLine($"Adjective: {SchoolAdjective.ColourValue()}");
		sb.AppendLine($"Colour: {PowerListColour.Name.Colour(PowerListColour)}");
		return sb.ToString();
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.MagicSchools.Find(Id);
		dbitem.Name = Name;
		dbitem.ParentSchoolId = _parentSchoolId;
		dbitem.SchoolVerb = SchoolVerb;
		dbitem.SchoolAdjective = SchoolAdjective;
		dbitem.PowerListColour = PowerListColour.Name;
		Changed = true;
	}
	#endregion
}
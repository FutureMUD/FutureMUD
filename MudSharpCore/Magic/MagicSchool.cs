using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;

namespace MudSharp.Magic;

public class MagicSchool : FrameworkItem, IMagicSchool
{
	public IFuturemud Gameworld { get; protected set; }

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

	public FutureProgVariableTypes Type => FutureProgVariableTypes.MagicSchool;
	public object GetObject => this;

	public IFutureProgVariable GetProperty(string property)
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
					FutureProgVariableTypes.Text);
			case "spells":
				return new CollectionVariable(Gameworld.MagicSpells.Where(x => x.School == this).ToList(),
					FutureProgVariableTypes.MagicSpell);
			case "capabilities":
				return new CollectionVariable(Gameworld.MagicCapabilities.Where(x => x.School == this).ToList(),
					FutureProgVariableTypes.MagicCapability);
		}

		throw new ApplicationException("Invalid property requested in MagicSchool.GetProperty");
	}

	private static IReadOnlyDictionary<string, FutureProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, FutureProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "name", FutureProgVariableTypes.Text },
			{ "id", FutureProgVariableTypes.Number },
			{ "verb", FutureProgVariableTypes.Text },
			{ "adjective", FutureProgVariableTypes.Text },
			{ "color", FutureProgVariableTypes.Text },
			{ "colour", FutureProgVariableTypes.Text },
			{ "parent", FutureProgVariableTypes.MagicSchool },
			{ "powers", FutureProgVariableTypes.Text | FutureProgVariableTypes.Collection },
			{ "spells", FutureProgVariableTypes.MagicSpell | FutureProgVariableTypes.Collection },
			{ "capabilities", FutureProgVariableTypes.MagicCapability | FutureProgVariableTypes.Collection }
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
		FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.MagicSchool, DotReferenceHandler(),
			DotReferenceHelp());
	}

	#endregion
}
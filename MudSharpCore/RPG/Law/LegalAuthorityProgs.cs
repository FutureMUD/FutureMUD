using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.RPG.Law;

public partial class LegalAuthority
{
	public ProgVariableTypes Type => ProgVariableTypes.LegalAuthority;
	public object GetObject => this;

	public IProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "id":
				return new NumberVariable(Id);
			case "name":
				return new TextVariable(Name);
			case "zones":
				return new CollectionVariable(EnforcementZones.ToList(), ProgVariableTypes.Zone);
			case "preparinglocation":
				return PreparingLocation;
			case "marshallinglocation":
				return MarshallingLocation;
			case "enforcerstowinglocation":
				return EnforcerStowingLocation;
			case "prisonlocation":
				return PrisonLocation;
			case "prisonreleaselocation":
				return PrisonReleaseLocation;
			case "prisonerbelongingsstoragelocation":
				return PrisonerBelongingsStorageLocation;
			case "jaillocation":
				return JailLocation;
			case "celllocations":
				return new CollectionVariable(CellLocations.ToList(), ProgVariableTypes.Location);
			case "jaillocations":
				return new CollectionVariable(JailLocations.ToList(), ProgVariableTypes.Location);
			case "laws":
				return new CollectionVariable(Laws.ToList(), ProgVariableTypes.Law);
			default:
				throw new ApplicationException($"Invalid property {property} requested in LegalAuthority.GetProperty");
		}
	}

	private static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", ProgVariableTypes.Number },
			{ "name", ProgVariableTypes.Text },
			{ "zones", ProgVariableTypes.Zone | ProgVariableTypes.Collection },
			{ "preparinglocation", ProgVariableTypes.Location },
			{ "marshallinglocation", ProgVariableTypes.Location },
			{ "enforcerstowinglocation", ProgVariableTypes.Location },
			{ "prisonlocation", ProgVariableTypes.Location },
			{ "prisonreleaselocation", ProgVariableTypes.Location },
			{ "prisonerbelongingsstoragelocation", ProgVariableTypes.Location },
			{ "jaillocation", ProgVariableTypes.Location },
			{ "celllocations", ProgVariableTypes.Location | ProgVariableTypes.Collection },
			{ "jaillocations", ProgVariableTypes.Location | ProgVariableTypes.Collection },
			{ "laws", ProgVariableTypes.Law | ProgVariableTypes.Collection }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", "The ID of the bank account" },
			{ "name", "The name of the bank account" },
			{ "zones", "The zones this legal authority enforces" },
			{ "preparinglocation", "The room patrols will prepare / equip in" },
			{ "marshallinglocation", "The room patrols will begin in after preparing" },
			{ "enforcerstowinglocation", "The room off duty enforcers are returned to" },
			{ "prisonlocation", "The room holding cells are connected to for enforcers to drag prisoners to" },
			{ "prisonreleaselocation", "The room where ex-prisoners are released to" },
			{ "prisonerbelongingsstoragelocation", "The room where prisoner belongings are stored while held" },
			{ "jaillocation", "The entry to the jail where custodial sentences are served" },
			{ "celllocations", "A collection of rooms where criminals in custody are held" },
			{ "jaillocations", "A collection of rooms where custodial prison sentences are served" },
			{ "laws", "A collection of the laws that apply in this zone" }
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.LegalAuthority,
			DotReferenceHandler(), DotReferenceHelp());
	}
}
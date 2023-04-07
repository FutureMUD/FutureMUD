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
	public FutureProgVariableTypes Type => FutureProgVariableTypes.LegalAuthority;
	public object GetObject => this;

	public IFutureProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "id":
				return new NumberVariable(Id);
			case "name":
				return new TextVariable(Name);
			case "zones":
				return new CollectionVariable(EnforcementZones.ToList(), FutureProgVariableTypes.Zone);
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
				return new CollectionVariable(CellLocations.ToList(), FutureProgVariableTypes.Location);
			case "jaillocations":
				return new CollectionVariable(JailLocations.ToList(), FutureProgVariableTypes.Location);
			case "laws":
				return new CollectionVariable(Laws.ToList(), FutureProgVariableTypes.Law);
			default:
				throw new ApplicationException($"Invalid property {property} requested in LegalAuthority.GetProperty");
		}
	}

	private static IReadOnlyDictionary<string, FutureProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, FutureProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", FutureProgVariableTypes.Number },
			{ "name", FutureProgVariableTypes.Text },
			{ "zones", FutureProgVariableTypes.Zone | FutureProgVariableTypes.Collection },
			{ "preparinglocation", FutureProgVariableTypes.Location },
			{ "marshallinglocation", FutureProgVariableTypes.Location },
			{ "enforcerstowinglocation", FutureProgVariableTypes.Location },
			{ "prisonlocation", FutureProgVariableTypes.Location },
			{ "prisonreleaselocation", FutureProgVariableTypes.Location },
			{ "prisonerbelongingsstoragelocation", FutureProgVariableTypes.Location },
			{ "jaillocation", FutureProgVariableTypes.Location },
			{ "celllocations", FutureProgVariableTypes.Location | FutureProgVariableTypes.Collection },
			{ "jaillocations", FutureProgVariableTypes.Location | FutureProgVariableTypes.Collection },
			{ "laws", FutureProgVariableTypes.Law | FutureProgVariableTypes.Collection }
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
		FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.LegalAuthority,
			DotReferenceHandler(), DotReferenceHelp());
	}
}
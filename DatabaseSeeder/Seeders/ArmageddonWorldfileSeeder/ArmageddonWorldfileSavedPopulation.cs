#nullable enable

using System.Linq;
using System.Text;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	// assign_zones + shutdown auto_save, not an inference from reset mode zero.
	internal static bool IsSourceSavedPopulationZone(int zone) => zone is 3 or 13 or 33 or 73;
	internal static string SavedPopulationRegister(int zone) => $"armw_saved_population_{zone}";

	private string SavedPopulationPrelude(int zone, ArmWorldReset[] resets)
	{
		var register = SavedPopulationRegister(zone);
		var owner = ProgVariableTypes.Location.ToStorageString();
		if (!_db.VariableDefinitions.Local.Any(x => x.OwnerTypeDefinition == owner && x.Property == register) &&
			!_db.VariableDefinitions.Any(x => x.OwnerTypeDefinition == owner && x.Property == register))
		{
			_db.VariableDefinitions.Add(new VariableDefinition { OwnerTypeDefinition = owner, Property = register,
				ContainedTypeDefinition = ProgVariableTypes.Boolean.ToStorageString() });
			_db.VariableDefaults.Add(new VariableDefault { OwnerTypeDefinition = owner, Property = register,
				DefaultValue = "<var>false</var>" });
		}

		var code = new StringBuilder("var initialrooms as location collection\n");
		var targets = resets.Where(x => x.ReferenceIssues.Length == 0 && (x.Opcode is "M" or "O"))
			.Select(x => x.Arguments[3]).Distinct().Where(_rooms.ContainsKey).ToArray();
		foreach (var vnum in targets)
		{
			code.AppendLine($"foreach (initial{vnum} in instancerooms(tolocation({_rooms[vnum].Id})))");
			code.AppendLine($"if (not(getregister(@initial{vnum}, \"{register}\")))\nadditem initialrooms @initial{vnum}\nend if\nend foreach");
		}
		Record($"population.saved.{zone}", "ArmageddonSavedPopulation", null, null, new
		{
			Zone = zone, SourceRooms = targets, Register = register,
			Disposition = "Source auto-save zone: initialise each actual room once, then retain native persistent movement, removal and death. New interiors initialise independently."
		});
		return code.ToString();
	}

	internal static string SavedPopulationAdmission(string destination) =>
		$"@initialrooms.Any(initialroom, @initialroom.id == @{destination}.id)";

	internal static string SavedPopulationCompletion(int zone) =>
		$"foreach (initialroom in @initialrooms)\nsetregister @initialroom \"{SavedPopulationRegister(zone)}\" true\nend foreach\n";
}

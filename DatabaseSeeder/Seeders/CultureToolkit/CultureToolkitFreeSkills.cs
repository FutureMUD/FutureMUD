#nullable enable

extern alias EngineCompiler;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.FutureProg;
using OfflineProgCompilation = EngineCompiler::MudSharp.Framework.OfflineProgCompilation;

namespace DatabaseSeeder.Seeders.CultureToolkit;

public static class CultureToolkitFreeSkills
{
	public const string StockBody = """
var skills as trait collection
// Universal Skills (usually perception, athletics, that kind of stuff)
// additem skills ToTrait("Skill Name")

// Racial skills
switch (@ch.Race)
  case (ToRace("Human"))
	// additem skills ToTrait("Skill Name")
	break
end switch

// Class-Based skills?

// Merits-Based, Culture-Based, Role-Based, etc etc

return @skills
""";
	public const string Start = "// <FutureMUD Seeder: Culture Fixed Skills>";
	public const string End = "// </FutureMUD Seeder: Culture Fixed Skills>";

	public static void Reconcile(FuturemudDatabaseContext context, ICollection<string> conflicts)
	{
		var record = CultureToolkitManagedEntities.Find(context, "FutureProg", "language.fixed-skills");
		if (record is null) return;
		var helper = context.FutureProgs.Include(x => x.FutureProgsParameters).Single(x => x.Id == record.LogicalId);
		var configured = context.ChargenScreenStoryboards.AsEnumerable().SelectMany(x =>
			XElement.Parse(x.StageDefinition).Descendants("FreeSkillsProg").Select(p => p.Value)).Distinct().ToArray();
		if (configured.Length == 0) return; // ChargenSeeder also calls this after storyboards are present.
		var updated = new List<long>();
		foreach (var reference in configured)
		{
			var target = long.TryParse(reference, out var id)
				? context.FutureProgs.Include(x => x.FutureProgsParameters).SingleOrDefault(x => x.Id == id)
				: context.FutureProgs.Include(x => x.FutureProgsParameters).SingleOrDefault(x => x.FunctionName == reference);
			if (target is null) throw new InvalidOperationException($"Configured FreeSkillsProg is missing: {reference}");
			var text = Normalize(target.FunctionText);
			var start = text.IndexOf(Start, StringComparison.Ordinal);
			var end = text.IndexOf(End, StringComparison.Ordinal);
			var managed = CultureToolkitManagedEntities.Find(context, "FreeSkillsBlock", $"prog.{target.Id}");
			var isStock = text == Normalize(StockBody);
			if ((start < 0 || end < start || text.LastIndexOf(Start, StringComparison.Ordinal) != start ||
				text.LastIndexOf(End, StringComparison.Ordinal) != end || managed is null) && !isStock)
			{
				conflicts.Add($"FreeSkillsProg {target.Id} ({target.FunctionName}): unrecognised custom body preserved; fixed grants were not wired.");
				continue;
			}
			var parameters = target.FutureProgsParameters.OrderBy(x => x.ParameterIndex).ToArray();
			if (parameters.Length != 1 || parameters[0].ParameterName != "ch" ||
				parameters[0].ParameterType != (long)ProgVariableTypes.Toon && parameters[0].ParameterType != (long)ProgVariableTypes.Chargen ||
				target.ReturnType != (long)(ProgVariableTypes.Collection | ProgVariableTypes.Trait) || target.AcceptsAnyParameters)
			{
				conflicts.Add($"FreeSkillsProg {target.Id}: incompatible configured signature preserved.");
				continue;
			}
			var block = $"{Start}\nforeach (cultureSkill in @{helper.FunctionName}(@ch))\n  if (not(Contains(@skills, @cultureSkill)))\n    additem skills @cultureSkill\n  end if\nend foreach\n{End}";
			var current = start >= 0 ? new Dictionary<string, string> { ["block"] = text[start..(end + End.Length)] } : new Dictionary<string, string>();
			var merged = CultureToolkitManagedEntities.Reconcile(context, record.Module, "FreeSkillsBlock", $"prog.{target.Id}", target.Id,
				managed is null && isStock, current, new Dictionary<string, string> { ["block"] = block }, conflicts);
			if (!merged.TryGetValue("block", out var replacement)) continue;
			target.FunctionText = start >= 0 ? text[..start] + replacement + text[(end + End.Length)..] :
				text[..text.LastIndexOf("return @skills", StringComparison.Ordinal)] + replacement + "\n\nreturn @skills";
			updated.Add(target.Id);
		}
		context.SaveChanges();
		using var compiler = new OfflineProgCompilation(context.FutureProgs.Include(x => x.FutureProgsParameters).ToArray());
		compiler.Compile(helper.Id);
		foreach (var id in updated.Distinct()) compiler.Compile(id);
	}

	private static string Normalize(string text) => text.Replace("\r\n", "\n").Trim();
}

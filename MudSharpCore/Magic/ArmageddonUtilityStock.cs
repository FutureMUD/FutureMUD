#nullable enable
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.RPG.Checks;
using Db = MudSharp.Models;

namespace MudSharp.Magic;

/// <summary>Transactional construction of ordinary editable stock rows, not a casting implementation.</summary>
internal static class ArmageddonUtilityStock
{
	internal static MagicSpell Create(IFuturemud world, IMagicSchool school, ITraitDefinition trait, IMagicResource resource,
		string name, string description, string durationFormula, double minimum, string emote,
		Func<long, long, long, XElement> definition, string? filterSource = null,
		ProgVariableTypes? targetType = null, IFutureProg? existingFilter = null)
		=> Create(world, school, trait, resource, new ArmageddonUtilitySpellContent("runtime.untracked", name,
			description, durationFormula, minimum, emote, definition, filterSource, targetType ?? ProgVariableTypes.Character), existingFilter);

	internal static MagicSpell Create(IFuturemud world, IMagicSchool school, ITraitDefinition trait, IMagicResource resource,
		ArmageddonUtilitySpellContent content, IFutureProg? existingFilter = null)

	{
		var name = content.Name; var targetType = content.TargetType ?? ProgVariableTypes.Character;
		if (!ReferenceEquals(school.Gameworld, world) || world.Traits.Get(trait.Id) != trait ||
			world.MagicResources.Get(resource.Id) != resource || trait.TraitType != TraitType.Skill || trait.OwnerScope != TraitOwnerScope.Character)
			throw new InvalidOperationException("Select an existing school, character skill and magic resource in this world.");
		if (existingFilter is not null && (world.FutureProgs.Get(existingFilter.Id) != existingFilter ||
			existingFilter.ReturnType != ProgVariableTypes.Boolean || !existingFilter.MatchesParameters([targetType, ProgVariableTypes.Character])))
			throw new InvalidOperationException("Select an existing boolean target-eligibility prog with (target, caster) parameters.");
		if (existingFilter is not null && !existingFilter.Compile())
			throw new InvalidOperationException("The selected target-eligibility prog does not compile: " + existingFilter.CompileError);
		if (world.MagicSpells.Any(x => x.School == school && x.Name.EqualTo(name)))
			throw new InvalidOperationException($"{name} already exists in that school; edit or clone it instead.");
		var no = world.AlwaysFalseProg ?? throw new InvalidOperationException("The always-false support prog is unavailable.");
		using var scope = FMDB.BeginIndependentScope(requireWrites: true);
		using var db = new FMDB(); using var transaction = FMDB.Context.Database.BeginTransaction();
		if (FMDB.Context.MagicSpells.Any(x => x.MagicSchoolId == school.Id && x.Name == name))
			throw new InvalidOperationException($"A persisted {name} already exists in that school.");
		var row = content.SpellRow(school.Id, trait.Id, no.Id);
		FMDB.Context.MagicSpells.Add(row); FMDB.Context.SaveChanges();
		var filter = content.EligibilityRow(row.Id);
		if (filter is not null)
		{
			var check = new FutureProg.FutureProg(filter, world);
			if (!check.Compile()) throw new InvalidOperationException("Stock eligibility prog failed: " + check.CompileError);
			FMDB.Context.FutureProgs.Add(filter);
		}
		var duration = content.DurationRow(row.Id);
		var cost = content.CostRow(row.Id);
		FMDB.Context.TraitExpressions.AddRange(duration,cost); FMDB.Context.SaveChanges();
		row.EffectDurationExpressionId = duration.Id;
		row.Definition = content.BuildDefinition(resource.Id,cost.Id,filter?.Id ?? existingFilter?.Id ?? 0).ToString();
		FMDB.Context.SaveChanges(); transaction.Commit();
		if (filter is not null) { var prog = new FutureProg.FutureProg(filter,world); prog.Compile(); world.Add(prog); }
		world.Add(new TraitExpression(duration,world)); world.Add(new TraitExpression(cost,world));
		var result = new MagicSpell(row,world); world.Add(result); return result;
	}

	internal static XElement Definition(string key, string trigger, long resource, long cost, long filter,
		int opening, double minimum, XElement effect) =>
		ArmageddonUtilitySpellContent.Definition(key, trigger, resource, cost, filter, opening, minimum, effect);
}

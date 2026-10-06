extern alias EngineCompiler;
#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Models;
using OfflineProgCompilation = EngineCompiler::MudSharp.Framework.OfflineProgCompilation;
using StackLoader = EngineCompiler::MudSharp.GameItems.Decorators.StackDecorator;

namespace DatabaseSeeder.Seeders;

public static partial class ArmageddonProvisionInstaller
{
	private static void Preflight(FuturemudDatabaseContext db, ArmageddonProvisionInstallPlan plan, List<Contribution> contributions,
		Dictionary<string, SeederManagedRecord> records, List<string> errors)
	{
		foreach (var contribution in contributions)
		{
			if (records.TryGetValue(contribution.Key, out var record))
			{
				if (record.Retired || record.EntityType != contribution.Type.Name || record.RevisionNumber is not null || Find(db, contribution, record) is null)
					errors.Add($"{contribution.Key}: owned row missing/retired/invalid; restore or explicitly rebind, no resurrection.");
				if (db.SeederManagedRecords.Any(x => x.Id != record.Id && x.EntityType == record.EntityType && x.LogicalId == record.LogicalId))
					errors.Add($"{contribution.Key}: competing retained ownership claim.");
			}
			else if (db.SeederManagedRecords.Any(x => x.Seeder == Package && x.StableKey == contribution.Key))
				errors.Add($"{contribution.Key}: cross-module ownership key; no adoption.");
			else if (contribution.Type == typeof(MagicSpell) && (db.MagicSpells.Any(x => x.MagicSchoolId == plan.School && x.Name.ToLower() == contribution.Name.ToLower()) ||
				db.MagicSpells.AsNoTracking().Select(x => x.Definition).AsEnumerable().Any(x => StockIdentity(x) == contribution.Key)))
				errors.Add($"{contribution.Key}: unowned spell name/stock-identity collision.");
		}
		if (records.Keys.Except(contributions.Select(x => x.Key)).Any()) errors.Add("Unknown retained provisions module keys require explicit version reconciliation.");
		if (new[] { plan.School, plan.Resource, plan.AlwaysFalseProg, plan.MealSkill, plan.WineSkill, plan.Wine }.Any(x => x <= 0) ||
			!db.MagicSchools.Any(x => x.Id == plan.School) || !db.MagicResources.Any(x => x.Id == plan.Resource && (x.MagicResourceType & (int)MagicResourceType.PlayerResource) != 0))
			errors.Add("Select existing native school and player-capable source resource IDs.");
		if (plan.MealSkill == plan.WineSkill || new[] { plan.MealSkill, plan.WineSkill }.Any(id =>
			!db.TraitDefinitions.Any(x => x.Id == id && x.Type == 0 && x.OwnerScope == 1))) errors.Add("Select two distinct existing character-owned native spell skills.");
		if (plan.WineBonusPlane is { } plane && (plane <= 0 || !db.Planes.Any(x => x.Id == plane))) errors.Add("Selected native wine bonus plane is missing.");
		var foodProfiles = plan.FoodProfiles.OrderBy(x => x.Order).ToArray(); var recipes = plan.WineRecipes.OrderBy(x => x.Order).ToArray();
		if (!ValidOrders(foodProfiles.Select(x => (x.Order, x.Predicate))) || foodProfiles.Any(x => x.Foods.Count is < 1 or > 32 ||
			x.Foods.Select(y => y.Id).Distinct().Count() != x.Foods.Count) || foodProfiles.LastOrDefault()?.Foods.Count != 3)
			errors.Add("Food profiles require unique native orders 1-32, compiled predicates and a final predicate-zero fallback of three explicit distinct foods.");
		if (!ValidOrders(recipes.Select(x => (x.Order, x.Predicate))) || recipes.LastOrDefault()?.Liquid != plan.Wine)
			errors.Add("Wine recipes require unique orders 1-32 and final predicate-zero fallback matching the explicitly selected wine.");
		foreach (var food in foodProfiles.SelectMany(x => x.Foods).Distinct()) ValidateFood(db, food, errors);
		foreach (var id in recipes.Select(x => x.Liquid).Append(plan.Wine).Distinct())
		{
			var liquid = db.Liquids.AsNoTracking().SingleOrDefault(x => x.Id == id);
			if (id <= 0 || liquid is null || new[] { liquid.AlcoholLitresPerLitre, liquid.WaterLitresPerLitre, liquid.FoodSatiatedHoursPerLitre, liquid.DrinkSatiatedHoursPerLitre }
				.Any(x => !double.IsFinite(x) || x < 0)) errors.Add($"Wine recipe liquid #{id} is missing or has invalid native nutrition.");
		}
		var progs = db.FutureProgs.Include(x => x.FutureProgsParameters).AsNoTracking().ToArray(); using var compiler = new OfflineProgCompilation(progs);
		if (progs.SingleOrDefault(x => x.Id == plan.AlwaysFalseProg) is not { } no || no.FunctionText.Trim() != "return false" ||
			compiler.Compile(no.Id) is var compiled && (compiled.ReturnType != ProgVariableTypes.Boolean || !compiled.MatchesParameters([])))
			errors.Add("Select the ordinary compiled no-argument always-false support prog.");
		foreach (var id in foodProfiles.Select(x => x.Predicate).Concat(recipes.Select(x => x.Predicate)).Where(x => x > 0).Distinct())
			if (progs.All(x => x.Id != id) || compiler.Compile(id) is var predicate &&
				(predicate.ReturnType != ProgVariableTypes.Boolean || !predicate.MatchesParameters([ProgVariableTypes.Character])))
				errors.Add($"Profile predicate #{id} requires a compiled native boolean(character) prog.");
		if (errors.Count != 0) return;
		var generated = Content(plan).Single(x => x.EligibilitySource is not null).EligibilityRow(0)!; generated.Id = -1;
		using var generatedCompiler = new OfflineProgCompilation(progs.Append(generated)); generatedCompiler.Compile(generated.Id);
	}
	private static bool ValidOrders(IEnumerable<(int Order, long Predicate)> input)
	{
		var rows = input.OrderBy(x => x.Order).ToArray();
		return rows.Length is > 0 and <= 32 && rows.All(x => x.Order is >= 1 and <= 32 && x.Predicate >= 0) &&
			rows.Select(x => x.Order).Distinct().Count() == rows.Length && rows[^1].Predicate == 0 && rows.SkipLast(1).All(x => x.Predicate > 0);
	}
	private static string? StockIdentity(string xml) { try { return (string?)XElement.Parse(xml).Element("StockIdentity"); } catch { return null; } }
	private static void ValidateFood(FuturemudDatabaseContext db, ArmageddonFoodPrototype binding, List<string> errors)
	{
		var item = db.GameItemProtos.Include(x => x.EditableItem).Include(x => x.GameItemProtosGameItemComponentProtos).Include(x => x.GameItemProtosOnLoadProgs)
			.AsNoTracking().SingleOrDefault(x => x.Id == binding.Id && x.RevisionNumber == binding.Revision);
		if (binding.Id <= 0 || binding.Revision < 0 || item is null || item.EditableItem.RevisionStatus != (int)RevisionStatus.Current || item.ReadOnly ||
			item.MorphTimeSeconds != 0 || item.GameItemProtosOnLoadProgs.Any() || db.DefaultHooks.Any(x => x.PerceivableType.ToLower() == "gameitem") ||
			db.GameItemProtos.Include(x => x.EditableItem).Any(x => x.Id == binding.Id && x.RevisionNumber > binding.Revision && x.EditableItem.RevisionStatus == (int)RevisionStatus.Current))
		{ errors.Add($"Food #{binding.Id}/{binding.Revision}: select the runtime-current approved loadable unscripted nonmorphing prototype without default GameItem hooks."); return; }
		var components = item.GameItemProtosGameItemComponentProtos.Select(x => db.GameItemComponentProtos.Include(y => y.EditableItem).AsNoTracking()
			.SingleOrDefault(y => y.Id == x.GameItemComponentProtoId && y.RevisionNumber == x.GameItemComponentRevision)).ToArray();
		if (components.Length != 2 || components.Any(x => x is null || x.EditableItem.RevisionStatus != (int)RevisionStatus.Current) ||
			!components.Select(x => x!.Type).Order().SequenceEqual(new[] { "Food", "Holdable" }.Order()))
		{ errors.Add($"Food #{binding.Id}: exactly approved native Food/Holdable components are required."); return; }
		try
		{
			var definition = XElement.Parse(components.Single(x => x!.Type == "Food")!.Definition);
			if ((double?)definition.Attribute("Bites") is not { } bites || !double.IsFinite(bites) || bites <= 0 ||
				new[] { "Satiation", "Water", "Thirst", "Alcohol" }.Any(x => (double?)definition.Attribute(x) is { } value && (!double.IsFinite(value) || value < 0)) ||
				(long?)definition.Element("OnEatProg") is > 0 || (long?)definition.Attribute("Decorator") is not { } decorator ||
				db.StackDecorators.AsNoTracking().SingleOrDefault(x => x.Id == decorator) is not { } row)
			{ errors.Add($"Food #{binding.Id}: finite positive bites, nonnegative nutrition, real decorator and no eating prog required."); return; }
			_ = StackLoader.LoadStackDecorator(row);
		}
		catch (Exception error) { errors.Add($"Food #{binding.Id}: malformed native component/decorator: {error.Message}"); }
	}
}

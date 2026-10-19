#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal partial class SkillPackageSeeder
{
	private string SeedArmageddon(FuturemudDatabaseContext db, IReadOnlyDictionary<string, string> answers)
	{
		if (ArmageddonSkillPackage.IsInstalled(db))
		{
			_ = ArmageddonSkillPackage.Resolve(db);
			return "Armageddon skill identities retained. Builder customisations and player values were preserved; no shared infrastructure was reseeded.";
		}
		ArmageddonSkillPackage.RequireFreshWorld(db);
		if (!answers["skillgainmodel"].Equals("armageddon", StringComparison.OrdinalIgnoreCase) ||
			!answers["skillcapmodel"].Equals("class", StringComparison.OrdinalIgnoreCase) ||
			!answers["branching"].Equals("no", StringComparison.OrdinalIgnoreCase) ||
			!answers["modern"].Equals("no", StringComparison.OrdinalIgnoreCase))
			throw new InvalidOperationException("Armageddon requires class caps, Armageddon improvement, branching=no (owned role trees), modern=no. Classic/Simple magic is selected later.");
		using var transaction = db.Database.IsRelational() ? db.Database.BeginTransaction() : null;
		var templates = SeedCheckTemplates(db, false);
		var (general, crafting, language, veterancy, professional, languageImprover, generalImprover) = SeedSkillImprovers(db, "armageddon");
		var decorators = new Dictionary<string, TraitDecorator>(StringComparer.OrdinalIgnoreCase)
		{
			["General"] = general!, ["Crafting"] = crafting!, ["Language"] = language!,
			["Veterancy"] = veterancy!, ["Professional"] = professional!
		};
		var alwaysTrue = db.FutureProgs.Single(x => x.FunctionName == "AlwaysTrue");
		var alwaysFalse = db.FutureProgs.Single(x => x.FunctionName == "AlwaysFalse");
		foreach (var entry in ArmageddonSkillPackage.Skills)
		{
			if (db.TraitDefinitions.Any(x => x.Name == entry.Name))
				throw new InvalidOperationException($"Armageddon skill name collides with an existing trait: {entry.Name}.");
			var trait = new TraitDefinition
			{
				Name = entry.Name, Type = 0, OwnerScope = (int)MudSharp.Body.Traits.TraitOwnerScope.Character, DerivedType = 0,
				TraitGroup = entry.Group, Hidden = entry.Scaffold,
				Expression = new TraitExpression { Name = $"{entry.Key}.cap", Expression = entry.Language ? "200" : entry.Scaffold ? "100" : "0" },
				DecoratorId = entry.Language ? language!.Id : general!.Id,
				ImproverId = entry.Language ? languageImprover!.Id : generalImprover!.Id,
				AvailabilityProg = alwaysFalse, LearnableProg = entry.Scaffold ? alwaysFalse : alwaysTrue,
				TeachableProg = alwaysFalse, TeachDifficulty = 7, LearnDifficulty = 7,
				ChargenBlurb = "", BranchMultiplier = 1.0
			};
			db.TraitDefinitions.Add(trait);
			db.SaveChanges();
            ArmageddonSettingOwnership.RecordNew(db, ArmageddonSkillPackage.Module, entry.Key, trait,
                nameof(trait.Name), nameof(trait.TraitGroup), nameof(trait.Type), nameof(trait.OwnerScope), nameof(trait.DerivedType),
                nameof(trait.ExpressionId), nameof(trait.DecoratorId), nameof(trait.ImproverId), nameof(trait.Hidden),
                nameof(trait.AvailabilityProgId), nameof(trait.LearnableProgId), nameof(trait.TeachableProgId),
                nameof(trait.TeachDifficulty), nameof(trait.LearnDifficulty), nameof(trait.BranchMultiplier), nameof(trait.ChargenBlurb));
            ArmageddonSettingOwnership.RecordNew(db, ArmageddonSkillPackage.Module, entry.Key + ".cap", trait.Expression,
                nameof(TraitExpression.Name), nameof(TraitExpression.Expression));
		}
		db.SaveChanges();
		SeedAdminLanguage(db, answers, decorators, languageImprover!);
		var bindings = ArmageddonSkillPackage.EngineBindings(db);
		SeedChecks(db, answers, templates, bindings);
		var owned = ArmageddonSkillPackage.Resolve(db);
		ArmageddonSkillPackage.BindStaticTraits(db, owned);
		var direction = DirectionSenseSeederHelper.BindOwnedTrait(db, owned.ToDictionary(x => x.Key, x => x.Value.Id));
		if (!direction.Success) throw new InvalidOperationException(direction.Message);
		if (!GuardingCheckSeederHelper.BindExistingSkill(db, owned["arm.skill.guarding"].Id, out var message))
			throw new InvalidOperationException(message);
		NPCSkillPackageSeederHelper.EnsureUniversalPackage(db, bindings);
		db.SaveChanges();
		transaction?.Commit();
		return "Armageddon canonical skills, explicit ungranted engine scaffolds, checks and Admin Speech installed. Role caps/grants and heritage are later Setting stages; no player values changed.";
	}
}

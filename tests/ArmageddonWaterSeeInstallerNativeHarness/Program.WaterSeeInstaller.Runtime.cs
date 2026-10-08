#nullable enable
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Decorators;
using MudSharp.Body.Traits.Improvement;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.Logging;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Casting;
using MudSharp.Magic.Gathering;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits;
using MudSharp.RPG.Merits.CharacterMerits;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static void LoadWaterSeeNative(NativeRuntime native, TestDatabase database, ArmageddonMagicInstallPlan utilityPlan,
		IReadOnlyDictionary<string, long> utilityIds, IReadOnlyDictionary<string, long> ids, int admissions = 3,
		IReadOnlyDictionary<string, long>? provisionIds = null, IReadOnlyDictionary<string, long>? perceptionIds = null)
	{
		var world = native.World; using var db = NewIndependentContext(database.ConnectionString);
		var progs = (All<IFutureProg>)world.FutureProgs;
		var progIds = utilityIds.Where(x => x.Key.EndsWith(".eligibility")).Select(x => x.Value)
			.Append(utilityPlan.AlwaysFalseProg).Append(utilityPlan.MendEligibilityProg).Append(db.FutureProgs.Single(x => x.FunctionName == "traditionAlwaysTrue").Id)
			.Concat(provisionIds?.Where(x => x.Key.EndsWith(".eligibility")).Select(x => x.Value) ?? [])
			.Concat(perceptionIds?.Where(x => x.Key.EndsWith(".eligibility")).Select(x => x.Value) ?? [])
			.Concat(db.FutureProgs.Where(x => x.Subcategory == "Installed Provisions").Select(x => x.Id)).ToArray();
		foreach (var model in db.FutureProgs.Include(x => x.FutureProgsParameters).AsNoTracking().Where(x => progIds.Contains(x.Id)))
			if (progs.Get(model.Id) is null) { var prog = new FutureProg(model, world); Require(prog.Compile(), prog.CompileError); progs.Add(prog); }
		var decorators = new All<ITraitValueDecorator>();
		foreach (var model in db.TraitDecorators.AsNoTracking()) decorators.Add(DecoratorBase.GetDecorator(model));
		native.WorldMock.SetupGet(x => x.TraitDecorators).Returns(decorators);
		var models = new All<IImprovementModel>();
		foreach (var model in db.Improvers.AsNoTracking()) models.Add(ImprovementModel.LoadModel(model, world));
		native.WorldMock.SetupGet(x => x.ImprovementModels).Returns(models); native.WorldMock.SetupGet(x => x.LogManager).Returns(Mock.Of<ILogManager>());
		var expressions = new All<ITraitExpression>();
		foreach (var model in db.TraitExpressions.Include(x => x.TraitExpressionParameters).AsNoTracking()) expressions.Add(new TraitExpression(model, world));
		native.WorldMock.SetupGet(x => x.TraitExpressions).Returns(expressions);
		var traits = new All<ITraitDefinition>();
		foreach (var model in db.TraitDefinitions.AsNoTracking().Where(x => x.TraitGroup == "ARM02" || x.TraitGroup == "Armageddon Spell"))
		{
			TraitDefinition definition = model.Type == (int)TraitType.Skill ? new SkillDefinition(model, world) : new AttributeDefinition(model, world);
			definition.Initialise(model); traits.Add(definition);
		}
		native.WorldMock.SetupGet(x => x.Traits).Returns(traits);
		SetPrivateField(native.Actor, "_characterTraits", db.CharacterTraits.AsNoTracking().Where(x => x.CharacterId == native.Actor.Id).ToArray()
			.Select(x => CastingRequired(traits.Get(x.TraitDefinitionId)).LoadTrait(new MudSharp.Models.Trait { Value = x.Value, AdditionalValue = x.AdditionalValue }, native.Actor)).ToList());
		var spells = (All<IMagicSpell>)world.MagicSpells;
		foreach (var id in utilityPlan.SpellSkills.Keys.Select(x => utilityIds[x])
			.Concat(provisionIds?.Where(x => x.Key is ArmageddonReviewedProvisionContent.SustainMealKey or ArmageddonReviewedProvisionContent.DrawWineKey).Select(x => x.Value) ?? [])
			.Concat(perceptionIds?.Where(x => x.Key is ArmageddonReviewedPierceContent.Key or ArmageddonWaterSeeInstaller.WaterBreathingKey or ArmageddonWaterSeeInstaller.SeeTheUnbodiedKey).Select(x => x.Value) ?? []))
		{
			if (spells.Get(id) is { } previous) spells.Remove(previous);
			var spell = new MagicSpell(db.MagicSpells.AsNoTracking().Single(x => x.Id == id), world);
			spells.Add(spell); if (!spell.ReadyForGame) throw new InvalidOperationException(spell.WhyNotReadyForGame(native.Actor));
		}
		var capabilities = (All<IMagicCapability>)world.MagicCapabilities;
		foreach (var variant in ArmageddonTraditionInstaller.Variants)
		{
			var id = ids["arm.capability." + variant]; if (capabilities.Get(id) is { } previous) capabilities.Remove(previous);
			var cap = (SkillLevelBasedMagicCapability)MagicCapabilityFactory.LoadCapability(db.MagicCapabilities.AsNoTracking().Single(x => x.Id == id), world);
			capabilities.Add(cap); Require(cap.CastingConfigurationErrors().Count == 0, string.Join("\n", cap.CastingConfigurationErrors()));
			Require(cap.GatheringConfigurationErrors().Count == 0, string.Join("\n", cap.GatheringConfigurationErrors()));
			Require(!cap.Regenerators.Any() && cap.CastingPolicy!.Admissions.Count == admissions && !cap.CastingPolicy.PassiveEntitlement, "Loaded policy gained passive regen or unimplemented spells.");
		}
		MagicCapabilityMerit.RegisterMeritInitialiser(); var merits = new All<IMerit>();
		foreach (var variant in ArmageddonTraditionInstaller.Variants)
			merits.Add(MeritFactory.LoadMerit(db.Merits.AsNoTracking().Single(x => x.Id == ids["arm.merit." + variant]), world));
		native.WorldMock.SetupGet(x => x.Merits).Returns(merits); native.Actor.SetMerits(merits);
		Console.WriteLine("ARMTRAD-load=passed actual-installed-native-skills cap-expressions classic-improver three-capabilities three-real-capability-merits no-passive-regenerators");
	}

}

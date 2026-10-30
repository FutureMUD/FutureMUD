#nullable enable
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Roles;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Models;
using CultureInfo = System.Globalization.CultureInfo;

namespace DatabaseSeeder.Seeders;

/// <summary>Creates native progression foundations. Never reads or writes player state.</summary>
public static partial class ArmageddonProgressionInstaller
{
	public const string Module = "stock-progression";
	// Keep the owned historical identity while exposing a single real character Mana pool.
	public const string SourceResourceKey = "arm.resource.source_energy";
	public const string ManaResourceKey = SourceResourceKey;
	public const string ManaGeneratorKey = "arm.generator.mana";
	internal const string Version = "stock-progression-1";
	private const string ImproverKey = "arm.improver.spell_practice";
	private sealed record Spec(Type Type, string Key, string Name);

	public static ArmageddonInstallResult Install(Func<FuturemudDatabaseContext> factory,
		Action<ArmageddonInstallCheckpoint>? checkpoint = null, bool completeStock = false, bool completeGathering = false)
	{
		using var db = factory();
		var settings = ArmageddonInstallation.Read(db) ?? throw new InvalidOperationException("Fix Armageddon installation settings before progression content.");
		var errors = ArmageddonInstallation.ValidateFoundations(db, settings);
		if (errors.Count != 0) return new(ArmageddonInstallStatus.Blocked, errors, new Dictionary<string, long>());
		var stock = new ArmageddonStockProgression(settings.Mode);
		var specs = Specs(stock, settings.ClassesEnabled);
		var messages = new List<string>();
		var ids = new Dictionary<string, long>(StringComparer.Ordinal);
		var committed = false;
		var committing = false;
		try
		{
			using var transaction = db.Database.IsRelational() ? db.Database.BeginTransaction(IsolationLevel.Serializable) : null;
			var records = db.SeederManagedRecords.Where(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey != ArmageddonInstallation.SettingsKey)
				.ToDictionary(x => x.StableKey, StringComparer.Ordinal);
			foreach (var spec in specs)
			{
				if (records.TryGetValue(spec.Key, out var record))
				{
					if (record.EntityType != spec.Type.Name || record.LogicalId is null || record.RevisionNumber is not null || record.Retired ||
						record.Module is not (Module or ArmageddonTraditionInstaller.Module) || db.Find(spec.Type, record.LogicalId.Value) is null ||
						db.SeederManagedRecords.Any(x => x.Id != record.Id && x.EntityType == record.EntityType && x.LogicalId == record.LogicalId))
						messages.Add($"{spec.Key}: missing, retired, incompatible or competing typed ownership; resolve explicitly.");
				}
				else if (Collision(db, spec)) messages.Add($"{spec.Key}: unowned name/verb collision; no automatic adoption.");
			}
			if (messages.Count > 0) return new(ArmageddonInstallStatus.Blocked, messages.AsReadOnly(), ids);
			var completedPolicies = records.Values.Where(x => x.EntityType == nameof(MagicCapability)).Select(HasCompleteStockPolicy).ToArray();
			var gatheringPolicies = records.Values.Where(x => x.EntityType == nameof(MagicCapability)).Select(HasSourceGatheringPolicy).ToArray();
			var hydrateGathering = completeGathering || gatheringPolicies.Any(x => x);
			var gather = hydrateGathering ? ArmageddonStockGatheringInstaller.Resolve(db) : null;
			var hydrate = completeStock || completedPolicies.Any(x => x) || hydrateGathering;
			var spells = hydrate ? CompleteStockSpells(db, records, messages) : null;
			if (messages.Count > 0) return new(ArmageddonInstallStatus.Blocked, messages.AsReadOnly(), ids);
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.PreflightComplete);
			T Put<T>(string key, T value) where T : class => Apply(db, specs.Single(x => x.Key == key), value, records, messages);
			long Native(ArmageddonNativeBinding binding) => settings.NativeBindings[binding];
			var zero = Put("arm.prog.resource_zero", new FutureProg { FunctionName = "ArmageddonResourceZero", FunctionText = "return 0",
				ReturnType = (long)ProgVariableTypes.Number, AcceptsAnyParameters = true, Category = "Magic", Subcategory = "Armageddon",
				FunctionComment = "No free starting energy. Character capacity uses the native attribute expression.", Public = false, StaticType = 0 });
			var capacity = Put("arm.resource.capacity", new TraitExpression { Name = "Reserve Capacity",
				Expression = "max(2*variable,min(12*variable,100+3*(variable-13)))" });
			// Spells, casting routes, gathering and charged items all use the same character Mana resource.
			var source = Put(SourceResourceKey, new MagicResource { Name = "Mana", ShortName = "mana", Type = "simple",
				MagicResourceType = 1, BottomColour = "red", MidColour = "yellow", TopColour = "green",
				Definition = Resource(Native(ArmageddonNativeBinding.AlwaysFalseProg), zero.Id, capacity.Id, settings.CapacityAttribute).ToString() });
			var improver = Put(ImproverKey, new Improver { Name = "Spell Practice", Type = "classic",
				Definition = "<Definition Chance='0.1' Expression='0.5' ImproveOnFail='true' ImproveOnSuccess='true' DifficultyThresholdInterval='0' NoGainSecondsDiceExpression='60' ImprovementProg='0'/>" });
			var traits = new Dictionary<string, long>(StringComparer.Ordinal);
			foreach (var group in stock.Bindings.GroupBy(x => x.TraitKey))
			{
				var key = group.Key;
				var name = TraitName(stock, key);
				var cap = Put(key + ".cap", new TraitExpression { Name = name + " cap", Expression = group.Max(x => x.RawCap).ToString(CultureInfo.InvariantCulture) });
				var trait = Put(key, new TraitDefinition { Name = name, Type = 0, OwnerScope = 1,
					DecoratorId = Native(ArmageddonNativeBinding.SkillDecorator), ImproverId = improver.Id, ExpressionId = cap.Id,
					TraitGroup = key == ArmageddonStockProgression.CraftTraitKey ? "Support" : "Magic", Hidden = false,
					ChargenBlurb = key == ArmageddonStockProgression.CraftTraitKey ? "Prepare the components used in magical work." : $"Practise {name} to improve your command of its magic.",
					AvailabilityProgId = Native(ArmageddonNativeBinding.AlwaysFalseProg), LearnableProgId = Native(ArmageddonNativeBinding.AlwaysTrueProg),
					TeachableProgId = Native(ArmageddonNativeBinding.AlwaysFalseProg), BranchMultiplier = 0, Alias = "", ValueExpression = "",
					TeachDifficulty = 7, LearnDifficulty = 7, ShowInScoreCommand = true });
				traits.Add(key, trait.Id);
			}
			var resourceIds = new Dictionary<string, long>(StringComparer.Ordinal);
			var schoolIds = new Dictionary<string, long>(StringComparer.Ordinal);
			var generator = Put(ManaGeneratorKey, new MagicGenerator { Name = "Mana regeneration", Type = "linear",
				Definition = new XElement("Definition", new XElement("WhichResource", source.Id), new XElement("AmountPerMinute", 2),
					new XElement("ConsciousOnly", true), new XElement("RestMultiplier", 2)).ToString() });
			foreach (var resourceKey in stock.Capabilities.Select(x => x.ResourceKey).Distinct())
			{
				var suffix = resourceKey["arm.resource.".Length..];
				var name = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(suffix);
				// ResourceKey retains the source school identity; it no longer allocates another pool.
				resourceIds.Add(resourceKey, source.Id);
				var schoolKey = "arm.school." + suffix;
				var school = Put(schoolKey, new MagicSchool { Name = name, SchoolVerb = "cast",
					SchoolAdjective = suffix, PowerListColour = "cyan" });
				schoolIds.Add(schoolKey, school.Id);
			}
			foreach (var cap in stock.Capabilities)
			{
				var casting = stock.Casting(cap, traits, resourceIds[cap.ResourceKey], spells, source.Id);
				casting.SetAttributeValue("enabled", hydrate && settings.StockAvailabilityByDefault);
				var definition = new XElement("Definition", new XElement("ConcentrationTrait", (long)casting.Attribute("trait")!),
					new XElement("ConcentrationCapabilityExpression", "1"), new XElement("ConcentrationDifficultyExpression", "5"),
					new XElement("Regenerators", hydrate && !ArmageddonStockProgression.IsSorcerer(cap)
						? new XElement("Regenerator", generator.Id) : null), casting);
				if (gather is not null && ArmageddonStockProgression.IsSorcerer(cap))
				{
					casting.Add(ArmageddonStockGatheringInstaller.Support(cap.Key, gather));
					definition.Add(ArmageddonStockGatheringInstaller.Methods(cap.Key, resourceIds[cap.ResourceKey], gather));
				}
				var capability = Put(cap.Key, new MagicCapability { Name = RoleName(cap), CapabilityModel = "skilllevel", PowerLevel = 1,
					MagicSchoolId = schoolIds[cap.ResourceKey.Replace("arm.resource.", "arm.school.", StringComparison.Ordinal)],
					Definition = definition.ToString() });
				var merit = Put(cap.MeritKey, new Merit { Name = cap.Name + " caster", Type = "Magic Capability", MeritScope = 2, MeritType = 0,
					Definition = new XElement("Definition", new XElement("ChargenAvailableProg", Native(ArmageddonNativeBinding.AlwaysFalseProg)),
						new XElement("ApplicabilityProg", Native(ArmageddonNativeBinding.AlwaysTrueProg)),
						new XElement("ChargenBlurb", ArmageddonPresentation.MagicDescription(cap, stock)),
						new XElement("DescriptionText", "@ have|has the capacity to channel " + cap.Name.ToLowerInvariant() + " magic."),
						new XElement("Capabilities", new XElement("Capability", capability.Id))).ToString() });
				if (!settings.ClassesEnabled) continue;
				var role = Put(cap.RoleKey, new ChargenRole { Name = RoleName(cap), Type = (int)ChargenRoleType.Class,
					PosterId = Native(ArmageddonNativeBinding.BuilderAccount), ChargenBlurb = ArmageddonPresentation.MagicDescription(cap, stock),
					AvailabilityProgId = Native(hydrate && settings.StockAvailabilityByDefault ? ArmageddonNativeBinding.AlwaysTrueProg : ArmageddonNativeBinding.AlwaysFalseProg), MaximumNumberAlive = 0, MaximumNumberTotal = 0 });
				// Selected-Class enrolment owns magic openings. Native role adjustments would also
				// add culture starting values and then boost those same skills during construction.
				RoleLinks(db, role, records[cap.RoleKey], merit.Id, messages);
			}
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.ContentCreated);
			db.SaveChanges();
			foreach (var spec in specs) ids.Add(spec.Key, records[spec.Key].LogicalId!.Value);
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.BeforeCommit);
			committing = true;
			transaction?.Commit();
			committed = true;
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.AfterCommit);
			messages.Add($"Progression foundations committed: {settings.Mode}, {traits.Count} native skills, ten capability/merit pairs, {(settings.ClassesEnabled ? 10 : 0)} Class roles, one shared Mana resource and one native regenerator. {stock.Bindings.Count(x => !x.Support)} spell admission plans {(hydrate ? "materialised with real canonical identities and native support grants" : "retained in the embedded approved graph")}. Casting and Class defaults: {(hydrate && settings.StockAvailabilityByDefault ? "enabled" : "closed")}; builder overrides are preserved. {(hydrateGathering ? "Four source Self/Land methods and three Gather support grants are installed with an independent editable permission." : "Source gathering remains a separate resumable policy stage.")} No player state changed.");
			return new(ArmageddonInstallStatus.Completed, messages.AsReadOnly(), ids);
		}
		catch (Exception error)
		{
			messages.Add((committed ? "Committed; confirmation failed. Fresh-context rerun retains the same owned identities. " : committing ?
				"Commit outcome unknown. Reconnect and rerun through owned identities. " : "Progression batch failed before commit and rolled back. ") + error.Message);
			if (!committed) ids.Clear();
			return new(committed ? ArmageddonInstallStatus.CommittedConfirmationFailed : committing ? ArmageddonInstallStatus.CommitOutcomeUnknown : ArmageddonInstallStatus.Failed, messages.AsReadOnly(), ids);
		}
	}

	private static XElement Resource(long no, long zero, long expression, long attribute, bool reserve = true) => new("Definition",
		new XElement("ShouldStartWithResourceCharacterProg", no), new XElement("ShouldStartWithResourceItemProg", no), new XElement("ShouldStartWithResourceLocationProg", no),
		new XElement("StartingResourceAmountCharacterProg", zero), new XElement("StartingResourceAmountItemProg", zero), new XElement("StartingResourceAmountLocationProg", zero),
		new XElement("ResourceCapProg", zero), reserve ? new XElement("AttributeCapacity", new XAttribute("version", 1), new XAttribute("attribute", attribute),
			new XAttribute("expression", expression), new XAttribute("basis", "effective")) : null);

	private static string TraitName(ArmageddonStockProgression stock, string key) => key == ArmageddonStockProgression.CraftTraitKey ? "Component Crafting" :
		stock.Capabilities.SingleOrDefault(x => x.MainTraitKey == key) is { } cap ? cap.Name + " Casting" :
		stock.SpellNames[key[..^".skill".Length]];
	private static string RoleName(ArmageddonStockCapability cap) => ArmageddonStockProgression.IsSorcerer(cap) ? cap.Name : cap.Name + " Elementalist";

	private static IReadOnlyList<Spec> Specs(ArmageddonStockProgression stock, bool classes)
	{
		var result = new List<Spec> { new(typeof(FutureProg), "arm.prog.resource_zero", "ArmageddonResourceZero"),
			new(typeof(TraitExpression), "arm.resource.capacity", "Reserve Capacity"), new(typeof(Improver), ImproverKey, "Spell Practice"),
			new(typeof(MagicResource), SourceResourceKey, "Mana"),
			new(typeof(MagicGenerator), ManaGeneratorKey, "Mana regeneration") };
		foreach (var key in stock.Bindings.Select(x => x.TraitKey).Distinct())
		{
			var name = TraitName(stock, key);
			result.Add(new(typeof(TraitExpression), key + ".cap", name + " cap"));
			result.Add(new(typeof(TraitDefinition), key, name));
		}
		foreach (var resource in stock.Capabilities.Select(x => x.ResourceKey).Distinct())
		{
			var suffix = resource["arm.resource.".Length..];
			var name = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(suffix);
			result.Add(new(typeof(MagicSchool), "arm.school." + suffix, name));
		}
		foreach (var cap in stock.Capabilities)
		{
			result.Add(new(typeof(MagicCapability), cap.Key, RoleName(cap)));
			result.Add(new(typeof(Merit), cap.MeritKey, cap.Name + " caster"));
			if (classes) result.Add(new(typeof(ChargenRole), cap.RoleKey, RoleName(cap)));
		}
		return result;
	}
}

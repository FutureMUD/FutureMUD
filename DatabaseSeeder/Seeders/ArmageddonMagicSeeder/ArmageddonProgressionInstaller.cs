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
	public const string SourceResourceKey = "arm.resource.source_energy";
	private const string Version = "stock-progression-1";
	private const string ImproverKey = "arm.improver.spell_practice";
	private sealed record Spec(Type Type, string Key, string Name);

	public static ArmageddonInstallResult Install(Func<FuturemudDatabaseContext> factory,
		Action<ArmageddonInstallCheckpoint>? checkpoint = null)
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
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.PreflightComplete);
			T Put<T>(string key, T value) where T : class => Apply(db, specs.Single(x => x.Key == key), value, records, messages);
			long Native(ArmageddonNativeBinding binding) => settings.NativeBindings[binding];
			var zero = Put("arm.prog.resource_zero", new FutureProg { FunctionName = "ArmageddonResourceZero", FunctionText = "return 0",
				ReturnType = (long)ProgVariableTypes.Number, AcceptsAnyParameters = true, Category = "Magic", Subcategory = "Armageddon",
				FunctionComment = "No free starting energy. Character capacity uses the native attribute expression.", Public = false, StaticType = 0 });
			var capacity = Put("arm.resource.capacity", new TraitExpression { Name = "Armageddon reserve capacity",
				Expression = "max(2*variable,min(12*variable,100+3*(variable-13)))" });
			// All canonical spells use one cost unit. The casting service maps it to the chosen route's reserve.
			var source = Put(SourceResourceKey, new MagicResource { Name = "Armageddon source energy", ShortName = "armsource", Type = "simple",
				MagicResourceType = 0, BottomColour = "red", MidColour = "yellow", TopColour = "green",
				Definition = Resource(Native(ArmageddonNativeBinding.AlwaysFalseProg), zero.Id, capacity.Id, settings.CapacityAttribute, false).ToString() });
			var improver = Put(ImproverKey, new Improver { Name = "Armageddon spell practice", Type = "classic",
				Definition = "<Definition Chance='0.1' Expression='0.5' ImproveOnFail='true' ImproveOnSuccess='true' DifficultyThresholdInterval='0' NoGainSecondsDiceExpression='60' ImprovementProg='0'/>" });
			var traits = new Dictionary<string, long>(StringComparer.Ordinal);
			foreach (var group in stock.Bindings.GroupBy(x => x.TraitKey))
			{
				var key = group.Key;
				var name = TraitName(stock, key);
				var cap = Put(key + ".cap", new TraitExpression { Name = name + " cap", Expression = group.Max(x => x.RawCap).ToString(CultureInfo.InvariantCulture) });
				var trait = Put(key, new TraitDefinition { Name = name, Type = 0, OwnerScope = 1,
					DecoratorId = Native(ArmageddonNativeBinding.SkillDecorator), ImproverId = improver.Id, ExpressionId = cap.Id,
					TraitGroup = key == ArmageddonStockProgression.CraftTraitKey ? "Armageddon Support" : "Armageddon Magic", Hidden = false,
					ChargenBlurb = "Legitimate capability enrolment controls spell knowledge and route caps. A skill alone grants no spell.",
					AvailabilityProgId = Native(ArmageddonNativeBinding.AlwaysFalseProg), LearnableProgId = Native(ArmageddonNativeBinding.AlwaysTrueProg),
					TeachableProgId = Native(ArmageddonNativeBinding.AlwaysFalseProg), BranchMultiplier = 0, Alias = "", ValueExpression = "",
					TeachDifficulty = 7, LearnDifficulty = 7, ShowInScoreCommand = true });
				traits.Add(key, trait.Id);
			}
			var resourceIds = new Dictionary<string, long>(StringComparer.Ordinal);
			var schoolIds = new Dictionary<string, long>(StringComparer.Ordinal);
			var generators = new Dictionary<string, long>(StringComparer.Ordinal);
			foreach (var resourceKey in stock.Capabilities.Select(x => x.ResourceKey).Distinct())
			{
				var suffix = resourceKey["arm.resource.".Length..];
				var name = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(suffix);
				var resource = Put(resourceKey, new MagicResource { Name = "Armageddon " + name + " reserve", ShortName = "arm" + suffix,
					Type = "simple", MagicResourceType = 1, BottomColour = "red", MidColour = "yellow", TopColour = "green",
					Definition = Resource(Native(ArmageddonNativeBinding.AlwaysFalseProg), zero.Id, capacity.Id, settings.CapacityAttribute).ToString() });
				resourceIds.Add(resourceKey, resource.Id);
				var schoolKey = "arm.school." + suffix;
				var school = Put(schoolKey, new MagicSchool { Name = "Armageddon " + name, SchoolVerb = "arm" + suffix,
					SchoolAdjective = suffix, PowerListColour = "cyan" });
				schoolIds.Add(schoolKey, school.Id);
				if (suffix == "sorcerer") continue;
				var generator = Put("arm.generator." + suffix, new MagicGenerator { Name = "Armageddon " + name + " regeneration", Type = "linear",
					Definition = new XElement("Definition", new XElement("WhichResource", resource.Id), new XElement("AmountPerMinute", 2),
						new XElement("ConsciousOnly", true), new XElement("RestMultiplier", 2)).ToString() });
				generators.Add(resourceKey, generator.Id);
			}
			foreach (var cap in stock.Capabilities)
			{
				var casting = stock.Casting(cap, traits, resourceIds[cap.ResourceKey], sourceResource: source.Id);
				var capability = Put(cap.Key, new MagicCapability { Name = "Armageddon " + cap.Name, CapabilityModel = "skilllevel", PowerLevel = 1,
					MagicSchoolId = schoolIds[cap.ResourceKey.Replace("arm.resource.", "arm.school.", StringComparison.Ordinal)],
					Definition = new XElement("Definition", new XElement("ConcentrationTrait", (long)casting.Attribute("trait")!),
						new XElement("ConcentrationCapabilityExpression", "1"), new XElement("ConcentrationDifficultyExpression", "5"),
						new XElement("Regenerators"), casting).ToString() });
				var merit = Put(cap.MeritKey, new Merit { Name = "Armageddon " + cap.Name + " caster", Type = "Magic Capability", MeritScope = 2, MeritType = 0,
					Definition = new XElement("Definition", new XElement("ChargenAvailableProg", Native(ArmageddonNativeBinding.AlwaysFalseProg)),
						new XElement("ApplicabilityProg", Native(ArmageddonNativeBinding.AlwaysTrueProg)),
						new XElement("ChargenBlurb", "Armageddon " + cap.Name + " magic; explicit enrolment after the complete stock policy is activated."),
						new XElement("DescriptionText", "@ have|has the capacity to channel " + cap.Name.ToLowerInvariant() + " magic."),
						new XElement("Capabilities", new XElement("Capability", capability.Id))).ToString() });
				if (!settings.ClassesEnabled) continue;
				var role = Put(cap.RoleKey, new ChargenRole { Name = RoleName(cap), Type = (int)ChargenRoleType.Class,
					PosterId = Native(ArmageddonNativeBinding.BuilderAccount), ChargenBlurb = "Armageddon " + cap.Name + " magic (" + settings.Mode + "). Stock activation awaits the complete spell catalogue.",
					AvailabilityProgId = Native(ArmageddonNativeBinding.AlwaysFalseProg), MaximumNumberAlive = 0, MaximumNumberTotal = 0 });
				RoleLinks(db, role, records[cap.RoleKey], merit.Id, stock.Bindings.Where(x => x.CapabilityKey == cap.Key && x.Starting)
					.GroupBy(x => traits[x.TraitKey]).ToDictionary(x => x.Key, x => x.Max(y => y.Opening)), messages);
			}
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.ContentCreated);
			db.SaveChanges();
			foreach (var spec in specs) ids.Add(spec.Key, records[spec.Key].LogicalId!.Value);
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.BeforeCommit);
			committing = true;
			transaction?.Commit();
			committed = true;
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.AfterCommit);
			messages.Add($"S2 progression foundations committed: {settings.Mode}, {traits.Count} native skills, ten capability/merit pairs, {(settings.ClassesEnabled ? 10 : 0)} Class roles, eight reserves and seven native regenerators. Complete {stock.Bindings.Count(x => !x.Support)} spell admission plans retained in the embedded approved graph. Policies and class availability remain disabled pending qualified S3/S4 stock; no partial active rosters. No player state changed.");
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

	private static string TraitName(ArmageddonStockProgression stock, string key) => key == ArmageddonStockProgression.CraftTraitKey ? "Armageddon Component Crafting" :
		stock.Capabilities.SingleOrDefault(x => x.MainTraitKey == key) is { } cap ? "Armageddon " + cap.Name + " Casting" :
		"Armageddon " + stock.SpellNames[key[..^".skill".Length]];
	private static string RoleName(ArmageddonStockCapability cap) => ArmageddonStockProgression.IsSorcerer(cap) ? cap.Name : cap.Name + " Elementalist";

	private static IReadOnlyList<Spec> Specs(ArmageddonStockProgression stock, bool classes)
	{
		var result = new List<Spec> { new(typeof(FutureProg), "arm.prog.resource_zero", "ArmageddonResourceZero"),
			new(typeof(TraitExpression), "arm.resource.capacity", "Armageddon reserve capacity"), new(typeof(Improver), ImproverKey, "Armageddon spell practice"),
			new(typeof(MagicResource), SourceResourceKey, "Armageddon source energy") };
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
			result.Add(new(typeof(MagicResource), resource, "Armageddon " + name + " reserve"));
			result.Add(new(typeof(MagicSchool), "arm.school." + suffix, "Armageddon " + name));
			if (suffix != "sorcerer") result.Add(new(typeof(MagicGenerator), "arm.generator." + suffix, "Armageddon " + name + " regeneration"));
		}
		foreach (var cap in stock.Capabilities)
		{
			result.Add(new(typeof(MagicCapability), cap.Key, "Armageddon " + cap.Name));
			result.Add(new(typeof(Merit), cap.MeritKey, "Armageddon " + cap.Name + " caster"));
			if (classes) result.Add(new(typeof(ChargenRole), cap.RoleKey, RoleName(cap)));
		}
		return result;
	}
}

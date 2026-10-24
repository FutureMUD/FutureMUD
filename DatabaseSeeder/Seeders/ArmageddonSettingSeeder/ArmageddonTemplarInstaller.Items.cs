#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using CultureInfo = System.Globalization.CultureInfo;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Database;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Magic;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class ArmageddonTemplarInstaller
{
	private const string SouthernCastingKey = "arm.capability.southern_templar";
	private const string SouthernCastingMeritKey = "arm.merit.southern_templar_casting";
	private const string SouthernReserveKey = "arm.resource.southern_templar";
	private const string SouthernMagicTraitKey = "arm.skill.southern_templar_magic";
	private static readonly string[] SouthernElementalistKeys =
	[
		"arm.capability.wind", "arm.capability.earth", "arm.capability.fire", "arm.capability.lightning",
		"arm.capability.water", "arm.capability.shadow", "arm.capability.void", "arm.capability.defiler"
	];

	private static void InstallSouthernMagicAndItems(FuturemudDatabaseContext db, Manifest manifest,
		IReadOnlyDictionary<string, ChargenRole> roles, IDictionary<string, TraitDefinition> skills, ICollection<string> messages)
	{
		var magic = manifest.SouthernMagic;
		var no = db.FutureProgs.Single(x => x.FunctionName == "AlwaysFalse");
		var yes = db.FutureProgs.Single(x => x.FunctionName == "AlwaysTrue");
		var sourceRole = roles["arm.class.southern_templar"];
		var enable = Owned<FutureProg>(db, "arm.class.southern_templar.enabled");
		var military = Prog(db, "arm.templar.southern.military_authority", "SouthernTemplarMilitaryAuthority", "return false",
			[(ProgVariableTypes.Character, "ch")], messages);
		var territory = Prog(db, magic.TerritoryProgKey, "SouthernTemplarAllanakTerritory", "return false",
			[(ProgVariableTypes.Character, "ch")], messages);
		var capIds = SouthernElementalistKeys.ToDictionary(x => x, x => RequireOwned<MagicCapability>(db, x), StringComparer.Ordinal);
		var priorCapId = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == SouthernCastingKey)?.LogicalId;
		var southernClassCapability = $"Contains(magiccapabilities(@ch), ToMagicCapability(\"{(priorCapId is { } existingCap ? existingCap.ToString(CultureInfo.InvariantCulture) : "Southern Templar casting")}\"))";
		var southern = Prog(db, magic.AuthorityProgKey, "SouthernTemplarAuthority",
			$"return @{enable.FunctionName}() and {southernClassCapability} and @{military.FunctionName}(@ch)",
			[(ProgVariableTypes.Character, "ch")], messages);
		var elementalist = Prog(db, "arm.templar.southern.gem.eligible", "TemplarGemEligibleElementalist",
			"return " + string.Join(" or ", capIds.Values.Select(x => $"Contains(magiccapabilities(@ch), ToMagicCapability(\"{x.ToString(CultureInfo.InvariantCulture)}\"))")),
			[(ProgVariableTypes.Character, "ch")], messages);
		var defiler = Prog(db, "arm.templar.southern.gem.incompatible_defiler", "TemplarGemIncompatibleDefiler",
			$"return Contains(magiccapabilities(@ch), ToMagicCapability(\"{capIds["arm.capability.defiler"].ToString(CultureInfo.InvariantCulture)}\"))",
			[(ProgVariableTypes.Character, "ch")], messages);
		var escort = Prog(db, "arm.templar.southern.gem.escort", "SouthernTemplarGemEscort",
			$"return @{enable.FunctionName}() and {southernClassCapability} and @{military.FunctionName}(@ch)",
			[(ProgVariableTypes.Character, "ch")], messages);
		_ = no; _ = yes; _ = sourceRole; _ = military;

		var skill = EnsureMagicSkill(db, skills, SouthernMagicTraitKey, "Southern Templar Magic", messages);
		var permanentKeys = magic.PermanentSpellKeys;
		var choiceKeys = magic.Choices.Values.SelectMany(x => x).ToArray();
		var allKeys = permanentKeys.Concat(choiceKeys).Distinct(StringComparer.Ordinal).ToArray();
		var spellIds = allKeys.ToDictionary(x => x, x => RequireOwned<MagicSpell>(db, x), StringComparer.Ordinal);
		var school = (MagicSchool)Put(db, "arm.school.southern_templar", new MagicSchool { Name = "Southern Templar",
			SchoolVerb = "templar", SchoolAdjective = "templar", PowerListColour = "yellow" },
			[nameof(MagicSchool.Name), nameof(MagicSchool.SchoolVerb), nameof(MagicSchool.SchoolAdjective), nameof(MagicSchool.PowerListColour)], messages);
		var resourceTemplate = Owned<MagicResource>(db, "arm.resource.void");
		var resourceDefinition = XElement.Parse(resourceTemplate.Definition);
		var zeroProg = Owned<FutureProg>(db, "arm.prog.resource_zero");
		var capProg = Prog(db, SouthernReserveKey + ".cap", "SouthernTemplarReserveCap", "return 100", [], messages, ProgVariableTypes.Number);
		var sourceResource = RequireOwned<MagicResource>(db, ArmageddonProgressionInstaller.SourceResourceKey);
		resourceDefinition.SetElementValue("ShouldStartWithResourceCharacterProg", no.Id);
		resourceDefinition.SetElementValue("StartingResourceAmountCharacterProg", zeroProg.Id);
		resourceDefinition.SetElementValue("ResourceCapProg", capProg.Id);
		resourceDefinition.Elements("AttributeCapacity").Remove();
		var priorResourceId = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == SouthernReserveKey)?.LogicalId;
		if (db.MagicResources.Any(x => x.ShortName == "armsouth" && x.Id != priorResourceId))
			throw new InvalidOperationException("An unowned magic resource already uses the Southern Templar reserve short name.");
		var resource = (MagicResource)Put(db, SouthernReserveKey, new MagicResource { Name = "Southern Templar reserve",
			ShortName = "armsouth", Type = "simple", MagicResourceType = resourceTemplate.MagicResourceType,
			BottomColour = resourceTemplate.BottomColour, MidColour = resourceTemplate.MidColour, TopColour = resourceTemplate.TopColour,
			Definition = resourceDefinition.ToString() }, [nameof(MagicResource.Name), nameof(MagicResource.ShortName), nameof(MagicResource.Type),
			nameof(MagicResource.MagicResourceType), nameof(MagicResource.BottomColour), nameof(MagicResource.MidColour), nameof(MagicResource.TopColour), nameof(MagicResource.Definition)], messages);
		var casting = new XElement("Casting", new XAttribute("version", 1), new XAttribute("identity", StableGuid("arm.capability.southern_templar")),
			new XAttribute("enabled", false), new XAttribute("trait", skill.Id), new XAttribute("source", sourceResource),
			new XAttribute("reserve", resource.Id), new XAttribute("passive", false), new XAttribute("startingVersion", 1),
			new XAttribute("enrolOnClassSelection", true));
		foreach (var key in allKeys)
			casting.Add(new XElement("Admission", new XAttribute("key", StableGuid("southern:" + key)), new XAttribute("trait", skill.Id),
				new XAttribute("starting", permanentKeys.Contains(key, StringComparer.Ordinal)), new XAttribute("opening", 1), new XAttribute("rawCap", 100),
				new XAttribute("spell", spellIds[key]), new XAttribute("min", 1), new XAttribute("max", 7), new XAttribute("capRelative", true)));
		var languageId = db.Languages.Single(x => x.LinkedTraitId == skills["arm.skill.sirihish"].Id).Id;
		var incantations = allKeys.ToDictionary(key => spellIds[key], key => new ControlledSpellIncantation(languageId,
			"granted", "templar", key.Split('.').Last(), "invoke", ["templar_" + key.Split('.').Last()],
			[new ControlledSpellDelivery("Shout", MudSharp.Form.Audio.AudioVolume.ExtremelyLoud, 1, 0)], "FutureMUD Southern Templar aliases; historical POWER"));
		var patron = new PatronCastingPolicy(southern.Id, territory.Id, magic.FixedGrade, true, true, true,
			permanentKeys.Select(x => spellIds[x]).ToArray(), magic.SelectionLimit, incantations);
		casting.Add(new XElement("Patron", JsonSerializer.Serialize(patron)));
		if (db.MagicCapabilities.Any(x => x.Name == "Southern Templar casting") && !db.SeederManagedRecords.Any(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == SouthernCastingKey))
			throw new InvalidOperationException("Unowned Southern Templar capability name collision.");
		var cap = (MagicCapability)Put(db, SouthernCastingKey, new MagicCapability { Name = "Southern Templar casting",
			CapabilityModel = "skilllevel", PowerLevel = 1, MagicSchoolId = school.Id,
			Definition = new XElement("Definition", new XElement("ConcentrationTrait", skill.Id), new XElement("ConcentrationCapabilityExpression", "1"),
				new XElement("ConcentrationDifficultyExpression", "5"), new XElement("Regenerators"), casting).ToString() },
			[nameof(MagicCapability.Name), nameof(MagicCapability.CapabilityModel), nameof(MagicCapability.PowerLevel), nameof(MagicCapability.MagicSchoolId), nameof(MagicCapability.Definition)], messages);
		if (priorCapId is null)
		{
			var gate = $"return @{enable.FunctionName}() and Contains(magiccapabilities(@ch), ToMagicCapability(\"{cap.Id.ToString(CultureInfo.InvariantCulture)}\")) and @{military.FunctionName}(@ch)";
			southern = Prog(db, magic.AuthorityProgKey, "SouthernTemplarAuthority", gate, [(ProgVariableTypes.Character, "ch")], messages);
			escort = Prog(db, "arm.templar.southern.gem.escort", "SouthernTemplarGemEscort", gate, [(ProgVariableTypes.Character, "ch")], messages);
		}
		var capabilityMerit = (Merit)Put(db, SouthernCastingMeritKey, new Merit { Name = "Southern Templar caster", Type = "Magic Capability",
			MeritType = 0, MeritScope = 2, Definition = new XElement("Definition", new XElement("ChargenAvailableProg", no.Id),
				new XElement("ApplicabilityProg", yes.Id), new XElement("ChargenBlurb", "Southern Templar casting; role selection provides the authored capability."),
				new XElement("DescriptionText", "@ channel|channels the Southern Templar tradition."), new XElement("Capabilities", new XElement("Capability", cap.Id))).ToString() },
			["Name", "Type", "MeritType", "MeritScope", "Definition"], messages);
		var classSkillRule = new RoleSkillRule(sourceRole.Id, skill.Id, true, 10, 100, null, 90, 0);
		_ = new RoleSkillProgression([classSkillRule]);
		var skillProgression = (Merit)Put(db, "arm.merit.southern_templar_magic_progression", new Merit
		{
			Name = "Southern Templar Magic Progression", Type = "Role Skill Progression", MeritType = 0, MeritScope = 2,
			Definition = new XElement("Definition", new XElement("ChargenAvailableProg", no.Id), new XElement("ApplicabilityProg", yes.Id),
				new XElement("DescriptionText", "@ develop|develops the Southern Templar tradition"), new XElement("ChargenBlurb", "Southern Templar Magic progression"),
				new XElement("Rules", new XElement("Rule", new XAttribute("role", classSkillRule.RoleId), new XAttribute("trait", classSkillRule.TraitId),
					new XAttribute("primary", true), new XAttribute("opening", classSkillRule.Opening), new XAttribute("cap", classSkillRule.Cap),
					new XAttribute("threshold", classSkillRule.Threshold), new XAttribute("bonus", classSkillRule.OverlapBonus)))).ToString()
		}, ["Name", "Type", "MeritType", "MeritScope", "Definition"], messages);

		var material = Owned<GameItemProto>(db, "arm.item.focus_blank").MaterialId;
		var holdable = Owned<GameItemComponentProto>(db, "arm.catalogue.component.holdable");
		var account = db.Accounts.OrderBy(x => x.Id).First();
		var bodyId = db.Races.Single(x => x.Name == "Organic Humanoid").BaseBodyId;
		long Shape(params string[] names)
		{
			foreach (var name in names)
				if (db.BodypartShapes.AsEnumerable().SingleOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) is { } shape) return shape.Id;
			throw new InvalidOperationException("Templar wearable content requires a native body shape: " + string.Join(" / ", names));
		}
		var neck = Shape("Neck");
		var throat = Shape("Throat", "Neck");
		var torso = Shape("Torso", "Chest", "Upper Back", "Abdomen");
		var gem = Item(db, "arm.templar.southern.gem", "an Allanak gem", "gem crystal templar armageddon", "A sharply cut gem used by an authorised Southern Templar to bind an elementalist.",
			material, [holdable], account, messages, isWorn: true, bodyId, [neck, throat]);
		var shrine = Item(db, "arm.templar.southern.shrine", "a Southern Templar spell shrine", "shrine templar armageddon", "A configured site for selecting Southern Templar spells.",
			material, [holdable], account, messages);
		var medallion = Item(db, "arm.templar.southern.regen_medallion", "a Templar reserve medallion", "medallion reserve templar armageddon", "A charged medallion that restores Southern Templar reserve.",
			material, [holdable], account, messages);
		var regaliaGroup = (ItemGroup)Put(db, "arm.templar.southern.regalia.group", new ItemGroup { Name = "Armageddon Southern Templar Regalia", Keywords = "southern templar regalia" },
			[nameof(ItemGroup.Name), nameof(ItemGroup.Keywords)], messages);
		var robe = Item(db, "arm.templar.southern.regalia", "Southern Templar regalia", "robe regalia templar armageddon", "Recognised Southern Templar regalia bearing the authority of the city.",
			material, [holdable], account, messages, isWorn: true, bodyId, [torso], regaliaGroup.Id);

		var choices = magic.Choices.SelectMany(group => group.Value.Select(key => new ShrineSpellChoice(spellIds[key], group.Key))).ToArray();
		var shrinePolicy = new ConditionalMagicItemPolicy { Kind = ConditionalMagicItemKind.SpellSelectionShrine, EligibilityProgId = southern.Id,
			TerritoryProgId = territory.Id, CapabilityId = cap.Id, SelectionLimit = magic.SelectionLimit, AllowReplacement = true, Choices = choices };
		var hostileSpells = magic.GemInterceptSpellKeys.Select(key => RequireOwned<MagicSpell>(db, key)).ToArray();
		var gemPolicy = new ConditionalMagicItemPolicy { Kind = ConditionalMagicItemKind.ConditionalBinding, EligibilityProgId = elementalist.Id,
			ControllerProgId = southern.Id, TerritoryProgId = territory.Id, EscortProgId = escort.Id, ProtectedTargetProgId = military.Id,
			IncompatibleWearerProgId = defiler.Id, RequiredRegaliaPrototypeId = robe.Id, BindingGroup = "armageddon.southern.gem",
			Range = BindingActivationRange.World, WearLocations = ["neck", "throat"], PreventRemoval = true, PunishOutsideTerritory = true,
			IntervalSeconds = 40.0 / 12.0, IntervalJitterSeconds = 5.0 / 12.0, ActivationChance = 0.5,
			Activation = new ItemConsequence(20, 40, 40, 10, true), Removal = new ItemConsequence(15, 20, 30, 10, true),
			Periodic = new ItemConsequence(10, 25, 25, 10, true), IncompatibleWear = new ItemConsequence(HealthFraction: 1.0, Collapse: true, DestroyItem: true, Message: "The gem rejects the incompatible wearer."),
			InterceptHostileSpells = true, InterceptSpellIds = hostileSpells,
			ActivationMessage = "You exert your authority through the gem." };
		var restorerPolicy = new ConditionalMagicItemPolicy { Kind = ConditionalMagicItemKind.ResourceRestorer,
			EligibilityProgId = southern.Id, TerritoryProgId = territory.Id, ResourceId = resource.Id, Capacity = 6,
			ActivationMessage = "The medallion restores your Southern Templar reserve.", RechargeMessage = "You recharge the medallion." };
		var issuerPolicy = new ConditionalMagicItemPolicy { Kind = ConditionalMagicItemKind.ItemIssuer, EligibilityProgId = southern.Id,
			RequireWorn = true, BindingGroup = gemPolicy.BindingGroup, IssuedPrototypeIds = [gem.Id], ActivationMessage = "You form a binding gem from the regalia." };
		var itemTemplates = new[]
		{
			("arm.templar.southern.shrine.component", "SpellSelectionShrine", shrinePolicy),
			("arm.templar.southern.gem.component", "ConditionalBinding", gemPolicy),
			("arm.templar.southern.regen_medallion.component", "ResourceRestorer", restorerPolicy),
			("arm.templar.southern.regalia.issuer", "ItemIssuer", issuerPolicy)
		};
		var components = new Dictionary<string, GameItemComponentProto>(StringComparer.Ordinal);
		foreach (var (key, type, policy) in itemTemplates)
		{
			var component = Component(db, key, type, policy, account, messages);
			components.Add(key, component);
			var item = key == itemTemplates[0].Item1 ? shrine : key == itemTemplates[1].Item1 ? gem : key == itemTemplates[2].Item1 ? medallion : robe;
			var links = db.GameItemProtosGameItemComponentProtos.Where(x => x.GameItemProtoId == item.Id && x.GameItemProtoRevision == item.RevisionNumber &&
				x.GameItemComponentProtoId == component.Id && x.GameItemComponentRevision == component.RevisionNumber).ToArray();
			var merged = ArmageddonSettingLinks.Reconcile(db, key + ".links", nameof(GameItemProto), item.Id,
				new Dictionary<string, string> { ["Attached"] = JsonSerializer.Serialize(links.Length > 0) },
				new Dictionary<string, string> { ["Attached"] = "true" }, messages, item.RevisionNumber);
			if (JsonSerializer.Deserialize<bool>(merged["Attached"])) { if (links.Length == 0) Attach(item, component); }
			else db.GameItemProtosGameItemComponentProtos.RemoveRange(links);
		}
		messages.Add("Southern Templar magic installed with disabled capability, a configured 12-spell shrine, gem binding and regalia-only gem issuer; Allanak, military authority and class activation progs remain builder-configurable and fail closed.");
	}

	private static long RequireOwned<T>(FuturemudDatabaseContext db, string key) where T : class
	{
		return (long)db.Entry(Owned<T>(db, key)).Property("Id").CurrentValue!;
	}
	private static T Owned<T>(FuturemudDatabaseContext db, string key) where T : class
	{
		var row = db.SeederManagedRecords.AsNoTracking().SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == key);
		if (row is null || row.Retired || row.EntityType != typeof(T).Name || row.LogicalId is not > 0 || db.Find(typeof(T), row.RevisionNumber is { } revision ? [row.LogicalId.Value, revision] : [row.LogicalId.Value]) is null)
			throw new InvalidOperationException($"Required owned Armageddon prerequisite is missing or incompatible: {key}.");
		return (T)db.Find(typeof(T), row.RevisionNumber is { } ownedRevision ? [row.LogicalId.Value, ownedRevision] : [row.LogicalId.Value])!;
	}

	private static TraitDefinition EnsureMagicSkill(FuturemudDatabaseContext db, IDictionary<string, TraitDefinition> skills, string key, string name,
		ICollection<string> messages)
	{
		if (skills.TryGetValue(key, out var result)) return result;
		var existing = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.Module == Module && x.StableKey == key);
		if (existing is not null) return skills[key] = db.TraitDefinitions.Include(x => x.Expression).Single(x => x.Id == existing.LogicalId);
		if (db.TraitDefinitions.Any(x => x.Name == name)) throw new InvalidOperationException($"Unowned Templar skill name collision: {name}.");
		var template = skills["arm.skill.contact"];
		var no = db.FutureProgs.Single(x => x.FunctionName == "AlwaysFalse");
		var yes = db.FutureProgs.Single(x => x.FunctionName == "AlwaysTrue");
		var trait = new TraitDefinition { Name = name, Type = 0, OwnerScope = (int)TraitOwnerScope.Character, DerivedType = 0,
			TraitGroup = "Templar Magic", Hidden = false, Expression = new TraitExpression { Name = key + ".cap", Expression = "100" },
			DecoratorId = template.DecoratorId, ImproverId = template.ImproverId, AvailabilityProg = no, LearnableProg = yes, TeachableProg = no,
			TeachDifficulty = 7, LearnDifficulty = 7, ChargenBlurb = "Builder-configured Southern Templar spell skill.", BranchMultiplier = 0 };
		db.TraitDefinitions.Add(trait); db.SaveChanges();
		ArmageddonSettingOwnership.RecordNew(db, Module, key, trait, nameof(TraitDefinition.Name), nameof(TraitDefinition.TraitGroup), nameof(TraitDefinition.Type),
			nameof(TraitDefinition.OwnerScope), nameof(TraitDefinition.DerivedType), nameof(TraitDefinition.ExpressionId), nameof(TraitDefinition.DecoratorId),
			nameof(TraitDefinition.ImproverId), nameof(TraitDefinition.Hidden), nameof(TraitDefinition.AvailabilityProgId), nameof(TraitDefinition.LearnableProgId),
			nameof(TraitDefinition.TeachableProgId), nameof(TraitDefinition.TeachDifficulty), nameof(TraitDefinition.LearnDifficulty), nameof(TraitDefinition.BranchMultiplier),
			nameof(TraitDefinition.ChargenBlurb));
		ArmageddonSettingOwnership.RecordNew(db, Module, key + ".cap", trait.Expression, nameof(TraitExpression.Name), nameof(TraitExpression.Expression));
		skills.Add(key, trait);
		return trait;
	}

	private static Guid StableGuid(string value) => new(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(ArmageddonMagicInstaller.Package + ":" + value)).AsSpan(0, 16));

	private static GameItemComponentProto Component(FuturemudDatabaseContext db, string key, string type, ConditionalMagicItemPolicy policy,
		Account account, ICollection<string> messages)
	{
		var desired = new GameItemComponentProto { Id = db.GameItemComponentProtos.Select(x => (long?)x.Id).Max().GetValueOrDefault() + 1,
			RevisionNumber = 0, Name = "Armageddon Templar " + type + " policy", Type = type, Description = "Reusable conditional magic item content; Armageddon policy instance.",
			Definition = policy.Save().ToString(), EditableItem = Approval(account) };
		return (GameItemComponentProto)PutRevisioned(db, key, desired, [nameof(GameItemComponentProto.Name), nameof(GameItemComponentProto.Type),
			nameof(GameItemComponentProto.Description), nameof(GameItemComponentProto.Definition)], messages);
	}

	private static GameItemProto Item(FuturemudDatabaseContext db, string key, string name, string keywords, string description, long material,
		IReadOnlyList<GameItemComponentProto> components, Account account, ICollection<string> messages, bool isWorn = false,
		long bodyId = 0, IReadOnlyList<long>? shapes = null, long? itemGroupId = null)
	{
		var item = new GameItemProto { Id = db.GameItemProtos.Select(x => (long?)x.Id).Max().GetValueOrDefault() + 1, RevisionNumber = 0,
			Name = name, UniqueName = key, Keywords = keywords, ShortDescription = name, FullDescription = description,
			LongDescription = description, MaterialId = material, ItemGroupId = itemGroupId, Size = (int)SizeCategory.VerySmall, Weight = 0.1,
			BaseItemQuality = (int)ItemQuality.Standard, MorphEmote = "$0 changes.", EditableItem = Approval(account) };
		foreach (var component in components) Attach(item, component);
		if (isWorn)
		{
			var profileIds = (shapes ?? []).Distinct().Select(shape => EnsureWearProfile(db, key + ".wear." + shape.ToString(CultureInfo.InvariantCulture), bodyId, shape, account, messages).Id).ToArray();
			var wearable = new GameItemComponentProto { Id = db.GameItemComponentProtos.Select(x => (long?)x.Id).Max().GetValueOrDefault() + 1,
				RevisionNumber = 0, Name = name + " wearable profile", Type = "Wearable", Description = "Native Armageddon wearable pattern.",
				Definition = new XElement("Definition", new XAttribute("DisplayInventoryWhenWorn", true), new XAttribute("Bulky", false),
					new XElement("WearableProg", 0), new XElement("WhyCannotWearProg", 0), new XElement("SeeThroughDamageRatio", 0.5),
					new XElement("LayerWeightConsumption", 0), new XElement("Waterproof", new XAttribute("ratio", 0.5), false),
					new XElement("Profiles", new XAttribute("Default", profileIds.First()), profileIds.Select(x => new XElement("Profile", x)))).ToString(),
				EditableItem = Approval(account) };
			wearable = (GameItemComponentProto)PutRevisioned(db, key + ".wearable", wearable,
				[nameof(GameItemComponentProto.Name), nameof(GameItemComponentProto.Type), nameof(GameItemComponentProto.Description), nameof(GameItemComponentProto.Definition)], messages);
			Attach(item, wearable);
		}
		return (GameItemProto)PutRevisioned(db, key, item, [nameof(GameItemProto.Name), nameof(GameItemProto.UniqueName), nameof(GameItemProto.Keywords),
			nameof(GameItemProto.ShortDescription), nameof(GameItemProto.FullDescription), nameof(GameItemProto.LongDescription), nameof(GameItemProto.MaterialId),
			nameof(GameItemProto.Size), nameof(GameItemProto.Weight), nameof(GameItemProto.BaseItemQuality), nameof(GameItemProto.MorphEmote), nameof(GameItemProto.ItemGroupId)], messages);
	}

	private static WearProfile EnsureWearProfile(FuturemudDatabaseContext db, string key, long bodyId, long shapeId, Account account, ICollection<string> messages)
	{
		var profile = new WearProfile { Name = "Armageddon " + key.Split('.').Last() + " Templar wearable", Type = "Shape", BodyPrototypeId = bodyId,
			Description = "Templar item wearable shape.", WearStringInventory = "worn", WearAction1st = "wear", WearAction3rd = "wears", WearAffix = "on",
			RequireContainerIsEmpty = false, WearlocProfiles = new XElement("Profiles", new XElement("Shape", new XAttribute("ShapeId", shapeId),
				new XAttribute("Count", 1), new XAttribute("Transparent", false), new XAttribute("NoArmour", true), new XAttribute("PreventsRemoval", false),
				new XAttribute("Mandatory", true), new XAttribute("HidesSevered", false))).ToString() };
		return (WearProfile)Put(db, key, profile, [nameof(WearProfile.Name), nameof(WearProfile.Type), nameof(WearProfile.BodyPrototypeId),
			nameof(WearProfile.Description), nameof(WearProfile.WearStringInventory), nameof(WearProfile.WearAction1st), nameof(WearProfile.WearAction3rd),
			nameof(WearProfile.WearAffix), nameof(WearProfile.RequireContainerIsEmpty), nameof(WearProfile.WearlocProfiles)], messages);
	}

	private static void Attach(GameItemProto item, GameItemComponentProto component)
	{
		if (!item.GameItemProtosGameItemComponentProtos.Any(x => x.GameItemComponentProtoId == component.Id && x.GameItemComponentRevision == component.RevisionNumber))
			item.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos { GameItemProtoId = item.Id,
				GameItemProtoRevision = item.RevisionNumber, GameItemComponentProtoId = component.Id, GameItemComponentRevision = component.RevisionNumber });
	}

	private static object PutRevisioned(FuturemudDatabaseContext db, string key, object desired, string[] fields, ICollection<string> messages)
	{
		var type = desired.GetType();
		var record = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == key);
		if (record is null)
		{
			db.Add(desired); db.SaveChanges();
			var ownership = ArmageddonSettingOwnership.RecordNew(db, Module, key, desired, fields);
			ownership.RevisionNumber = (int)db.Entry(desired).Property("RevisionNumber").CurrentValue!;
			db.SaveChanges();
			return desired;
		}
		if (record.Module != Module || record.EntityType != type.Name || record.Retired || record.LogicalId is not { } id || record.RevisionNumber is not { } revision)
			throw new InvalidOperationException($"Invalid revisioned Templar content ownership: {key}.");
		var actual = db.Find(type, id, revision) ?? throw new InvalidOperationException($"Missing owned Templar item content: {key}.");
		ArmageddonSettingOwnership.Reconcile(db, record, actual, fields.ToDictionary(x => x,
			x => JsonSerializer.Serialize(db.Entry(desired).Property(x).CurrentValue, db.Entry(desired).Property(x).Metadata.ClrType)), messages);
		return actual;
	}

	private static EditableItem Approval(Account account)
	{
		var now = DateTime.UtcNow;
		return new EditableItem { RevisionNumber = 0, RevisionStatus = (int)RevisionStatus.Current,
			BuilderAccountId = account.Id, BuilderDate = now, ReviewerAccountId = account.Id, ReviewerDate = now,
			BuilderComment = "Armageddon Templar stock", ReviewerComment = "Reusable typed item policy instance" };
	}
}

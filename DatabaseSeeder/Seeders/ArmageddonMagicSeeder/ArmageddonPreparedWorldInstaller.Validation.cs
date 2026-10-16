extern alias EngineCompiler;
#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Magic;
using OfflineWorld = EngineCompiler::MudSharp.Framework.Futuremud;
using NativeTrait = EngineCompiler::MudSharp.Body.Traits.TraitDefinition;
using NativeExpression = EngineCompiler::MudSharp.Body.Traits.TraitExpression;
using NativeResource = EngineCompiler::MudSharp.Magic.Resources.SimpleMagicResource;

namespace DatabaseSeeder.Seeders;

public static partial class ArmageddonPreparedWorldInstaller
{
	public static IReadOnlyList<string> Validate(FuturemudDatabaseContext db, ArmageddonPreparedWorldBindings bindings)
	{
		var errors = new List<string>();
		var utility = bindings.Utilities;
		if (utility is null || utility.SpellSkills is null || bindings.SupportSkills is null || bindings.AllowedMethods is null)
			return ["Utilities, SpellSkills, SupportSkills and all three AllowedMethods selections are required."];
		if (!utility.Install) errors.Add("Utilities.Install must be explicitly true for an opted-in package.");
		if (bindings.Provisions is not null && !NewProvisionsQualified) errors.Add(ProvisionReadiness + " Leave Provisions null.");
		if (bindings.Provisions is { } provision && (!provision.Install || provision.School != utility.School || provision.Resource != utility.Resource || provision.AlwaysFalseProg != utility.AlwaysFalseProg))
			errors.Add("An explicitly selected provision module must use the package's selected school, source resource and ordinary false prog.");
		if (new[] { utility.School, utility.Resource, utility.AlwaysFalseProg, utility.MendEligibilityProg, utility.Water,
			utility.LightPrototype, utility.HoldableComponent, utility.Material, utility.BuilderAccount, bindings.ReserveResource,
			bindings.Decorator, bindings.AlwaysTrueProg, bindings.GatheringTemplate, bindings.CapacityAttribute, bindings.CapacityExpression }.Any(x => x <= 0))
			errors.Add("Every required external binding must be a positive existing native ID; zero is not a selection.");
		if (utility.LightRevision < 0 || utility.HoldableRevision < 0 || utility.WaterBonusPlane is <= 0)
			errors.Add("Revisions must be nonnegative; an optional plane must be null or a positive ID.");
		var keys = ArmageddonMagicInstaller.Content(utility).Select(x => x.Key).ToArray();
		if (!utility.SpellSkills.Keys.Order().SequenceEqual(keys.Order()) || utility.SpellSkills.Values.Distinct().Count() != keys.Length)
			errors.Add("Supply exactly the five distinct utility character-skill bindings from the guide.");
		if (utility.SpellSkills.Values.Any(id => id <= 0 || !db.TraitDefinitions.AsNoTracking().Any(x => x.Id == id && x.Type == 0 && x.OwnerScope == 1)))
			errors.Add("Utility skill IDs must identify existing native character-owned skills.");
		if (!db.MagicSchools.AsNoTracking().Any(x => x.Id == utility.School) ||
			!db.MagicResources.AsNoTracking().Any(x => x.Id == utility.Resource && (x.MagicResourceType & (int)MagicResourceType.PlayerResource) != 0) ||
			!db.MagicResources.AsNoTracking().Any(x => x.Id == bindings.ReserveResource && (x.MagicResourceType & (int)MagicResourceType.PlayerResource) != 0))
			errors.Add("School and source/reserve mana must exist; both resources must support characters.");
		if (!db.Accounts.AsNoTracking().Any(x => x.Id == utility.BuilderAccount) || !db.Materials.AsNoTracking().Any(x => x.Id == utility.Material) ||
			!db.Liquids.AsNoTracking().Any(x => x.Id == utility.Water)) errors.Add("Select existing builder account, material and water liquid IDs.");
		if (!db.GameItemProtos.AsNoTracking().Any(x => x.Id == utility.LightPrototype && x.RevisionNumber == utility.LightRevision) ||
			!db.GameItemComponentProtos.AsNoTracking().Any(x => x.Id == utility.HoldableComponent && x.RevisionNumber == utility.HoldableRevision))
			errors.Add("Select existing light and Holdable revisions; the utility module validates approval, components and load safety.");
		if (!db.TraitDecorators.AsNoTracking().Any(x => x.Id == bindings.Decorator) ||
			new[] { utility.AlwaysFalseProg, utility.MendEligibilityProg, bindings.AlwaysTrueProg }.Any(id => !db.FutureProgs.AsNoTracking().Any(x => x.Id == id)))
			errors.Add("Select existing decorator and support progs; modules validate native types, compiled signatures and ordinary true/false bodies.");
		if (!bindings.SupportSkills.TryGetValue("arm.support.gather", out var gather) || gather <= 0 ||
			bindings.SupportSkills.Values.Any(id => !db.TraitDefinitions.AsNoTracking().Any(x => x.Id == id && x.Type == 0 && x.OwnerScope == 1)))
			errors.Add("Gather must map explicitly to an existing native character-owned use/check skill. Optional supports require real native counterparts.");
		if (!db.MagicCapabilities.AsNoTracking().Any(x => x.Id == bindings.GatheringTemplate && x.CapabilityModel == "skilllevel"))
			errors.Add("Select an existing skilllevel gathering template; the tradition module validates prices, signatures and selected routes before its commit.");
		var matrix = new Dictionary<string, MagicGatheringMethodKind[]>
		{
			["sorcerer"] = [MagicGatheringMethodKind.Self, MagicGatheringMethodKind.Gentle, MagicGatheringMethodKind.Land],
			["preserver"] = [MagicGatheringMethodKind.Self, MagicGatheringMethodKind.Gentle],
			["defiler"] = [MagicGatheringMethodKind.Self, MagicGatheringMethodKind.Land]
		};
		if (!bindings.AllowedMethods.Keys.Order().SequenceEqual(matrix.Keys.Order()) || bindings.AllowedMethods.Any(x =>
			x.Value is null || x.Value.Count == 0 || x.Value.Distinct().Count() != x.Value.Count ||
			!matrix.TryGetValue(x.Key, out var approved) || x.Value.Any(y => !approved.Contains(y))))
			errors.Add("Choose a nonempty distinct subset of the approved method matrix for each variant: sorcerer Self/Gentle/Land, preserver Self/Gentle, defiler Self/Land.");
		ValidateCapacity(db, bindings, errors);
		ValidateWaterSee(db, bindings, errors);
		ValidateEmotions(db, bindings, errors);
		try { _ = PreservedPierceSpell(db); }
		catch (Exception error) when (error is InvalidOperationException or System.Xml.XmlException or FormatException)
		{ errors.Add(error.Message); }
		try { _ = PreservedProvisionSpells(db); }
		catch (Exception error) when (error is InvalidOperationException or System.Xml.XmlException or FormatException) { errors.Add(error.Message); }
		return errors.AsReadOnly();
	}

	private static void ValidateCapacity(FuturemudDatabaseContext db, ArmageddonPreparedWorldBindings bindings, List<string> errors)
	{
		var row = db.MagicResources.AsNoTracking().SingleOrDefault(x => x.Id == bindings.ReserveResource);
		if (row?.Type != "simple" || bindings.CapacityBasis is not ("raw" or "effective"))
		{ errors.Add("Select a native simple reserve with an explicitly authored raw/effective attribute capacity."); return; }
		try
		{
			var xml = XElement.Parse(row.Definition).Element("AttributeCapacity");
			if (xml is null || (int?)xml.Attribute("version") != 1 || (long?)xml.Attribute("attribute") != bindings.CapacityAttribute ||
				(long?)xml.Attribute("expression") != bindings.CapacityExpression || ((string?)xml.Attribute("basis") ?? "effective") != bindings.CapacityBasis)
			{ errors.Add("Reserve AttributeCapacity must already match the explicitly selected attribute/expression/basis. Author it first; this installer never rewrites or refills resources."); return; }
			var expression = db.TraitExpressions.AsNoTracking().Include(x => x.TraitExpressionParameters).SingleOrDefault(x => x.Id == bindings.CapacityExpression);
			if (expression is null || !db.TraitDefinitions.AsNoTracking().Any(x => x.Id == bindings.CapacityAttribute && x.Type == 1 && x.OwnerScope == 0))
			{ errors.Add("Select an existing ordinary body-owned attribute and native capacity expression; no guessed attribute or fixture cap."); return; }
			// Native read-model construction only; no player load, prog execution, boot or writes.
			using var world = new OfflineWorld(null!);
			foreach (var attribute in db.TraitDefinitions.AsNoTracking().Where(x => x.Type == 1 && x.OwnerScope == 0))
				world.Add(NativeTrait.LoadTraitDefinition(attribute, world));
			world.Add(new NativeExpression(expression, world));
			var resource = new NativeResource(row, world);
			if (resource.AttributeCapacityError() is { } error) errors.Add("Native reserve capacity: " + error);
		}
		catch (Exception error) when (error is FormatException or InvalidOperationException or ArgumentException or System.Xml.XmlException)
		{ errors.Add("Reserve capacity is unreadable: " + error.Message); }
	}
}

extern alias EngineCompiler;
#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Magic;
using CreateProjectionEffect = EngineCompiler::MudSharp.Magic.SpellEffects.CreateProjectionEffect;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class ArmageddonTemplarInstaller
{
	private static void InstallLirathanShadowwalk(FuturemudDatabaseContext db, MagicSchool school, TraitDefinition skill,
		long canInvokeProgId, long cannotInvokeProgId, IDictionary<string, MagicPower> powers, ICollection<string> messages)
	{
		const string spellKey = "arm.spell.lirathan_walking_shadow";
		const string powerKey = "arm.power.lirathan.shadowwalk";
		const string knownProgKey = "arm.prog.lirathan.shadowwalk_known";
		const string spellName = "Lirathan Walking Shadow";
		var sourceId = RequireOwned<MagicSpell>(db, "arm.spell.walking_shadow");
		var source = db.MagicSpells.Single(x => x.Id == sourceId);
		var planeId = RequireOwned<Plane>(db, ArmageddonStockSupportInstaller.ShadowPlaneKey);
		if (!db.Planes.Any(x => x.Id == planeId && !x.IsDefault))
			throw new InvalidOperationException("Lirathan Shadowwalk requires the explicitly configured non-default Shadow plane.");
		if (db.MagicSpells.Any(x => x.Name == spellName && x.MagicSchoolId != school.Id) &&
			!db.SeederManagedRecords.Any(x => x.Seeder == ArmageddonMagicInstaller.Package && x.Module == Module && x.StableKey == spellKey))
			throw new InvalidOperationException("Unowned Lirathan Walking Shadow spell name collision.");

		var definition = XElement.Parse(source.Definition);
		definition.SetElementValue("StockIdentity", spellKey);
		definition.Element("Effects")?.ReplaceNodes(CreateProjectionEffect.Definition(new SpellProjectionConfiguration(
			SpellProjectionKind.WalkingShadow, planeId, 0, 120, 3, 0)));
		definition.Element("Costs")?.Remove();
		definition.Add(new XElement("Costs"));
		var no = db.FutureProgs.Single(x => x.FunctionName == "AlwaysFalse");
		var desiredSpell = new MagicSpell
		{
			Name = spellName,
			MagicSchoolId = school.Id,
			CastingTraitDefinitionId = skill.Id,
			SpellKnownProgId = no.Id,
			Blurb = "Create a bounded walking shadow of your existing identity.",
			Description = "A Lirathan Templar projects a bounded walking shadow on the Shadow plane. The original body remains anchored; the shadow keeps the caster's identity, cannot manipulate objects, and returns focus when it collapses.",
			CastingDifficulty = source.CastingDifficulty,
			MinimumSuccessThreshold = source.MinimumSuccessThreshold,
			ResistingTraitDefinitionId = source.ResistingTraitDefinitionId,
			ResistingDifficulty = source.ResistingDifficulty,
			EffectDurationExpressionId = source.EffectDurationExpressionId,
			ExclusiveDelay = source.ExclusiveDelay,
			NonExclusiveDelay = source.NonExclusiveDelay,
			AppliedEffectsAreExclusive = true,
			CastingEmote = source.CastingEmote,
			FailCastingEmote = source.FailCastingEmote,
			TargetEmote = source.TargetEmote,
			TargetResistedEmote = source.TargetResistedEmote,
			TargetNullEmote = source.TargetNullEmote,
			ScrollInscriptionAllowed = false,
			Definition = definition.ToString()
		};
		var spell = (MagicSpell)Put(db, spellKey, desiredSpell,
			[nameof(MagicSpell.Name), nameof(MagicSpell.MagicSchoolId), nameof(MagicSpell.CastingTraitDefinitionId), nameof(MagicSpell.SpellKnownProgId),
				nameof(MagicSpell.Blurb), nameof(MagicSpell.Description), nameof(MagicSpell.CastingDifficulty), nameof(MagicSpell.MinimumSuccessThreshold),
				nameof(MagicSpell.ResistingTraitDefinitionId), nameof(MagicSpell.ResistingDifficulty), nameof(MagicSpell.EffectDurationExpressionId),
				nameof(MagicSpell.ExclusiveDelay), nameof(MagicSpell.NonExclusiveDelay), nameof(MagicSpell.AppliedEffectsAreExclusive), nameof(MagicSpell.CastingEmote),
				nameof(MagicSpell.FailCastingEmote), nameof(MagicSpell.TargetEmote), nameof(MagicSpell.TargetResistedEmote), nameof(MagicSpell.TargetNullEmote),
				nameof(MagicSpell.ScrollInscriptionAllowed), nameof(MagicSpell.Definition)], messages);

		var power = (MagicPower)Put(db, powerKey, new MagicPower
		{
			Name = spellName,
			MagicSchoolId = school.Id,
			PowerModel = "spellbacked",
			Blurb = "Invoke the Lirathan Walking Shadow spell through the Shadowwalk skill.",
			ShowHelp = "Use PSI SHADOWWALK SELF to project a bounded walking shadow.",
			Definition = new XElement("Definition", new XElement("SeededIdentity", powerKey), new XElement("Verb", "shadowwalk"),
				new XElement("Spell", spell.Id), new XElement("InvocationGrade", 7), new XElement("IsPsionic", true), new XElement("CanInvokePowerProg", canInvokeProgId),
				new XElement("WhyCantInvokePowerProg", cannotInvokeProgId),
				new XElement("BodilyCost", new XAttribute("base", 18), new XAttribute("skillTrait", skill.Id),
					new XAttribute("willpowerTrait", FindAttribute(db, "Willpower", "Wisdom").Id), new XAttribute("verbs", "shadowwalk")),
				new XElement("InvocationCosts")).ToString()
		}, [nameof(MagicPower.Name), nameof(MagicPower.MagicSchoolId), nameof(MagicPower.PowerModel), nameof(MagicPower.Blurb), nameof(MagicPower.ShowHelp), nameof(MagicPower.Definition)], messages);
		powers.Add("shadowwalk", power);

		var knownProg = (FutureProg)Put(db, knownProgKey, new FutureProg
		{
			FunctionName = "LirathanTemplarKnowsShadowwalk",
			FunctionText = $"return HasMagicPower(@character, {power.Id})",
			ReturnType = (long)ProgVariableTypes.Boolean,
			ReturnTypeDefinition = ProgVariableTypes.Boolean.ToStorageString(),
			Category = "Armageddon",
			Subcategory = "Lirathan Psionics",
			FunctionComment = "Spell knowledge follows builder-granted Lirathan Shadowwalk power access; it grants nothing.",
			Public = false,
			AcceptsAnyParameters = false,
			StaticType = 0
		}, ["FunctionName", "FunctionText", "ReturnTypeDefinition", "Category", "Subcategory", "FunctionComment", "Public", "AcceptsAnyParameters", "StaticType"], messages);
		EnsureParameters(db, knownProgKey, knownProg, [(ProgVariableTypes.Character, "character"), (ProgVariableTypes.MagicSpell, "spell")], messages);
		desiredSpell.SpellKnownProgId = knownProg.Id;
		Put(db, spellKey, desiredSpell,
			[nameof(MagicSpell.Name), nameof(MagicSpell.MagicSchoolId), nameof(MagicSpell.CastingTraitDefinitionId), nameof(MagicSpell.SpellKnownProgId),
				nameof(MagicSpell.Blurb), nameof(MagicSpell.Description), nameof(MagicSpell.CastingDifficulty), nameof(MagicSpell.MinimumSuccessThreshold),
				nameof(MagicSpell.ResistingTraitDefinitionId), nameof(MagicSpell.ResistingDifficulty), nameof(MagicSpell.EffectDurationExpressionId),
				nameof(MagicSpell.ExclusiveDelay), nameof(MagicSpell.NonExclusiveDelay), nameof(MagicSpell.AppliedEffectsAreExclusive), nameof(MagicSpell.CastingEmote),
				nameof(MagicSpell.FailCastingEmote), nameof(MagicSpell.TargetEmote), nameof(MagicSpell.TargetResistedEmote), nameof(MagicSpell.TargetNullEmote),
				nameof(MagicSpell.ScrollInscriptionAllowed), nameof(MagicSpell.Definition)], messages);
		messages.Add("Lirathan Shadowwalk uses its own Lirathan-school spell, builder-granted power knowledge, and the existing Walking Shadow projection policy; it does not grant the Shadow-school spell.");
	}

	private static void EnsureParameters(FuturemudDatabaseContext db, string key, FutureProg prog,
		IReadOnlyList<(ProgVariableTypes Type, string Name)> parameters, ICollection<string> messages)
	{
		var current = db.FutureProgsParameters.Where(x => x.FutureProgId == prog.Id).OrderBy(x => x.ParameterIndex).ToArray();
		var expected = parameters.Select(x => new ProgParameter((long)x.Type, x.Name)).ToArray();
		var merged = ArmageddonSettingLinks.Reconcile(db, key + ".parameters", nameof(FutureProg), prog.Id,
			new Dictionary<string, string> { ["Parameters"] = System.Text.Json.JsonSerializer.Serialize(current.Select(x => new ProgParameter(x.ParameterType, x.ParameterName)).ToArray()) },
			new Dictionary<string, string> { ["Parameters"] = System.Text.Json.JsonSerializer.Serialize(expected) }, messages);
		var next = System.Text.Json.JsonSerializer.Deserialize<ProgParameter[]>(merged["Parameters"])!;
		if (!next.SequenceEqual(expected)) throw new InvalidOperationException($"Templar prog {prog.FunctionName} has a custom incompatible signature; preserve and resolve before composition.");
		if (!current.Select(x => new ProgParameter(x.ParameterType, x.ParameterName)).SequenceEqual(next))
		{
			db.FutureProgsParameters.RemoveRange(current);
			foreach (var (parameter, index) in next.Select((value, index) => (value, index)))
				db.FutureProgsParameters.Add(new FutureProgsParameter { FutureProgId = prog.Id, ParameterIndex = index,
					ParameterType = parameter.ParameterType, ParameterName = parameter.ParameterName });
		}
		db.SaveChanges();
	}
}

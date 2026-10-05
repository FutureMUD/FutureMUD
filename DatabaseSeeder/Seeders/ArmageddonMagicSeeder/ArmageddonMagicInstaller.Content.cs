#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Database;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using MudSharp.Magic;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public static partial class ArmageddonMagicInstaller
{
	private static List<Contribution> Contributions(ArmageddonMagicInstallPlan plan)
	{
		var result = new List<Contribution>();
		foreach (var content in Content(plan))
		{
			var dependencies = new List<string> { content.Key + ".duration", content.Key + ".cost", "external.school", "external.resource", "external.skill:" + content.Key, "external.always_false" };
			if (content.EligibilitySource is not null) dependencies.Add(content.Key + ".eligibility");
			else if (content.Key == ArmageddonReviewedUtilityContent.MendFleshKey) dependencies.Add("external.mend_eligibility");
			if (content.Key == ArmageddonReviewedUtilityContent.DrawWaterKey) dependencies.AddRange(["external.water", "external.water_bonus_plane"]);
			if (content.Key == ArmageddonReviewedUtilityContent.HoveringLightKey) dependencies.AddRange(["external.light", "external.light_revision"]);
			result.Add(new(typeof(MagicSpell), content.Key, content.Name, dependencies.ToArray()));
			result.Add(new(typeof(TraitExpression), content.Key + ".duration", "", []));
			result.Add(new(typeof(TraitExpression), content.Key + ".cost", "", []));
			if (content.EligibilitySource is not null) result.Add(new(typeof(FutureProg), content.Key + ".eligibility", "", []));
		}
		foreach (var kind in new[] { "wand", "staff" })
		{
			result.Add(new(typeof(GameItemComponentProto), $"arm.component.charged_{kind}", $"Armageddon blank {kind} charges", [ArmageddonReviewedUtilityContent.MendFleshKey, "external.builder"]));
			result.Add(new(typeof(GameItemProto), $"arm.item.charged_{kind}", $"Armageddon blank {kind}", [$"arm.component.charged_{kind}", "external.holdable", "external.holdable_revision", "external.material", "external.builder"]));
		}
		var bindings = new SortedDictionary<string, long>(StringComparer.Ordinal)
		{
			["external.school"] = plan.School, ["external.resource"] = plan.Resource, ["external.always_false"] = plan.AlwaysFalseProg,
			["external.mend_eligibility"] = plan.MendEligibilityProg, ["external.water"] = plan.Water,
			["external.light"] = plan.LightPrototype, ["external.light_revision"] = plan.LightRevision,
			["external.holdable"] = plan.HoldableComponent, ["external.holdable_revision"] = plan.HoldableRevision,
			["external.material"] = plan.Material, ["external.builder"] = plan.BuilderAccount, ["external.water_bonus_plane"] = plan.WaterBonusPlane ?? 0
		};
		foreach (var skill in plan.SpellSkills) bindings.Add("external.skill:" + skill.Key, skill.Value);
		return result.Select(x => x with { Bindings = bindings }).ToList();
	}
	private static void WriteContent(FuturemudDatabaseContext db, ArmageddonMagicInstallPlan plan,
		List<Contribution> contributions, Dictionary<string, SeederManagedRecord> records, List<string> messages)
	{
		Contribution Spec(string key) => contributions.Single(x => x.Key == key);
		foreach (var content in Content(plan))
		{
			var newSpell = !records.TryGetValue(content.Key, out var spellRecord);
			MagicSpell spell;
			if (newSpell)
			{
				spell = content.SpellRow(plan.School, plan.SpellSkills[content.Key], plan.AlwaysFalseProg);
				db.MagicSpells.Add(spell); db.SaveChanges();
				spellRecord = new SeederManagedRecord { Seeder = Package, Module = Module, EntityType = nameof(MagicSpell),
					StableKey = content.Key, LogicalId = spell.Id };
				db.SeederManagedRecords.Add(spellRecord); records.Add(content.Key, spellRecord);
			}
			else spell = (MagicSpell)Find(db, Spec(content.Key), spellRecord!)!;
			var duration = Apply(db, Spec(content.Key + ".duration"), content.DurationRow(spell.Id), records, messages);
			var cost = Apply(db, Spec(content.Key + ".cost"), content.CostRow(spell.Id), records, messages);
			var eligibility = content.EligibilityRow(spell.Id);
			var filter = eligibility is not null ? Apply(db, Spec(content.Key + ".eligibility"), eligibility, records, messages).Id :
				content.Key == ArmageddonReviewedUtilityContent.MendFleshKey ? plan.MendEligibilityProg : 0;
			var desired = content.SpellRow(plan.School, plan.SpellSkills[content.Key], plan.AlwaysFalseProg);
			desired.EffectDurationExpressionId = duration.Id;
			desired.Definition = content.BuildDefinition(plan.Resource, cost.Id, filter).ToString();
			Apply(db, Spec(content.Key), desired, records, messages, newSpell);
		}
		var mend = records[ArmageddonReviewedUtilityContent.MendFleshKey].LogicalId!.Value;
		foreach (var (kind, name) in new[] { (MagicDeviceKind.Wand, "wand"), (MagicDeviceKind.Staff, "staff") })
		{
			var componentKey = $"arm.component.charged_{name}";
			var component = Apply(db, Spec(componentKey), new GameItemComponentProto
			{
				Id = records.TryGetValue(componentKey, out var existing) ? existing.LogicalId!.Value : (db.GameItemComponentProtos.Select(x => (long?)x.Id).Max() ?? 0) + 1,
				RevisionNumber = existing?.RevisionNumber ?? 0, Name = Spec(componentKey).Name, Type = "ChargedMagicDevice",
				Description = "Editable dual focus and charged device. Caster eligibility; paid Mend Flesh production. Supplied blank.",
				Definition = MagicDeviceDefinition.ReviewedBlank(kind, mend), EditableItem = Approval(plan)
			}, records, messages);
			var itemKey = $"arm.item.charged_{name}";
			var item = new GameItemProto
			{
				Id = records.TryGetValue(itemKey, out existing) ? existing.LogicalId!.Value : (db.GameItemProtos.Select(x => (long?)x.Id).Max() ?? 0) + 1,
				RevisionNumber = existing?.RevisionNumber ?? 0, Name = Spec(itemKey).Name,
				Keywords = $"blank armageddon {name}", ShortDescription = $"a plain {name}",
				FullDescription = $"This plain {name} can focus Mend Flesh or store one homogeneous bank of legitimately produced charges. It is supplied empty.",
				MaterialId = plan.Material, Size = 1, Weight = name == "staff" ? 2 : 0.25,
				BaseItemQuality = (int)ItemQuality.Standard, MorphEmote = "$0 changes.", EditableItem = Approval(plan)
			};
			item.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = component.Id, GameItemComponentRevision = component.RevisionNumber });
			item.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = plan.HoldableComponent, GameItemComponentRevision = plan.HoldableRevision });
			Apply(db, Spec(itemKey), item, records, messages);
		}
	}
	private static EditableItem Approval(ArmageddonMagicInstallPlan plan) => new()
	{
		BuilderAccountId = plan.BuilderAccount, ReviewerAccountId = plan.BuilderAccount, BuilderDate = DateTime.UtcNow,
		ReviewerDate = DateTime.UtcNow, RevisionStatus = (int)RevisionStatus.Current,
		BuilderComment = "Explicitly selected partial Armageddon stock installation.", ReviewerComment = "Installer dependency preflight."
	};
}

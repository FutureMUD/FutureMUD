#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Effects;
using MudSharp.Events;
using MudSharp.FutureProg;
using MudSharp.GameItems.Interfaces;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	internal static ItemInteractionRestriction ResolveItemRestrictions(int extraFlags, int wearFlags)
	{
		var restrictions = (wearFlags & 1) == 0 ? ItemInteractionRestriction.Pickup : ItemInteractionRestriction.None;
		if ((extraFlags & (1 << 7)) != 0) restrictions |= ItemInteractionRestriction.Drop | ItemInteractionRestriction.Give;
		if ((extraFlags & (1 << 13)) != 0) restrictions |= ItemInteractionRestriction.OneHandedWield;
		if ((extraFlags & (1 << 16)) != 0) restrictions |= ItemInteractionRestriction.Sheathe;
		if ((extraFlags & (1 << 17)) != 0) restrictions |= ItemInteractionRestriction.RoomListing;
		return restrictions;
	}

	private void InstallSourceInventoryDisposal()
	{
		var sources = _source.Npcs.Where(x => (x.Flags[0] & (1 << 15)) != 0 && (x.Flags[2] & (1 << 14)) != 0)
			.Select(x => x.Vnum).ToArray();
		if (sources.Length == 0) return;
		var prog = Prog("ArmageddonWorldfileTransientUndeadInventory", "retirecarrieditems(@npc, \"TransientBodyDeath\")\nreturn false",
			ProgVariableTypes.Boolean, ("npc", ProgVariableTypes.Character));
		InstallNpcEventHook("Transient Undead Inventory Disposal", EventType.CharacterDies, prog, sources);
		Record("npc.transient-undead-disposal", "NpcInventoryDisposal", prog.Id, null, new
		{
			SourceVnums = sources,
			Disposition = "Only source templates marked both undead and summoned, including their reset variants, dispose of marked carried items on death. Worn/wielded goods and ordinary NPC deaths remain native."
		});
	}

	private void InstallItemFlags()
	{
		var shadowId = OwnedBehaviourId("arm.support.plane.shadow", nameof(Plane));
		if (_effectiveItems.Any(x => (x.Value.Source.Flags[1] & ((1 << 14) | (1 << 29))) != 0) && shadowId is null)
			throw new InvalidOperationException("Source ETHEREAL items require the imported Shadow plane.");
		var shadow = shadowId ?? 0;
		var glyphImmunity = InstallSourceItemMagic(shadowId);
		foreach (var (id, effective) in _effectiveItems)
		{
			var source = effective.Source;
			var flags = source.Flags[1];
			bool Has(int bit) => (flags & (1 << bit)) != 0;
			var proto = _db.GameItemProtos.Local.Single(x => x.Id == id);
			var restrictions = ResolveItemRestrictions(flags, source.Flags[2]);
			if (restrictions != ItemInteractionRestriction.None || Has(4))
				AttachDeliveryComponent(proto, Component("itemflags.permissions." + id, "Item Permissions", new XElement("Definition",
					new XElement("Restrictions", (int)restrictions), new XElement("PickupAttemptProg", 0), new XElement("PickupAttemptOnceOnly", false),
					new XElement("Cursed", Has(4) || Has(7)), new XElement("CurseRemovableRestrictions", Has(7) ? (int)(ItemInteractionRestriction.Drop | ItemInteractionRestriction.Give) : 0))), new XElement("Definition"));
			if (Has(25) || Has(12))
				AttachDeliveryComponent(proto, Component("itemflags.retirement." + id, "Transient Item", new XElement("Definition",
					new XElement("RetirementPlane", shadow), new XElement("RetireOnReload", Has(25)), new XElement("RetireOnLogout", Has(25)),
					new XElement("Triggers", Has(12) ? (int)(ItemRetirementTrigger.BulkDrop | ItemRetirementTrigger.EtherealTransition | ItemRetirementTrigger.TransientBodyDeath | ItemRetirementTrigger.HostBodyChange) : 0))), new XElement("Definition"));

			void Effect(string suffix, string command, string type, XElement data)
			{
				var initialisationKey = suffix switch
				{
					"DescriptiveGlow" => "flag:glow",
					"LocalHum" => "flag:hum",
					"IntrinsicMagic" => "flag:magic",
					"InvisibleItem" => "flag:invisible",
					"ImmortalItem" => "flag:immortal-invisible",
					_ => "flag:" + suffix.ToLowerInvariant()
				};
				AddItemOnLoadProg(proto, suffix,
					$"if (not(initialiseitemonce(@item, \"{initialisationKey}\")))\nreturn true\nend if\n" + command + "\nreturn true");
				foreach (var item in _db.GameItems.Local.Where(x => x.GameItemProtoId == id))
				{
					var effects = XElement.Parse(item.EffectData ?? "<Effects/>");
					var alreadyPresent = effects.Elements("Effect").Any(x =>
						(string?)x.Element("Type") == type && x.Elements().Any(payload => payload.Name == data.Name && XNode.DeepEquals(payload, data)));
					if (!alreadyPresent)
						effects.Add(new XElement("Effect", new XElement("ApplicabilityProg", 0), new XElement("Type", type),
							new XElement("Original", 0), new XElement("Remaining", 0), new XElement(data)));
					AddItemInitialisationKey(effects, initialisationKey);
					item.EffectData = effects.ToString();
				}
			}
			if (Has(0)) Effect("DescriptiveGlow", "applyitempresentationeffect(@item, \"glow\")", "ItemPresentation", new XElement("Humming", false));
			if (Has(1)) Effect("LocalHum", "applyitempresentationeffect(@item, \"hum\")", "ItemPresentation", new XElement("Humming", true));
			if (Has(6)) Effect("IntrinsicMagic", "applyitempresentationeffect(@item, \"magic\")", "ItemMagicAura", new XElement("Effect"));
			if (Has(5)) Effect("InvisibleItem", "adddetectionvisibilityeffect(@item, \"VisualMagical\", 0)", "DetectionVisibility",
				new XElement("Visibility", new XAttribute("senses", (long)PerceptionTypes.VisualMagical), new XAttribute("plane", 0)));
			if (Has(14) || Has(29))
			{
				var ready = Has(29);
				var visible = Has(14);
				AddItemOnLoadProg(proto, "EtherealAffinity" + id,
					$"addetherealitemaffinity(@item, {shadow}, {ready.ToString().ToLowerInvariant()}, {visible.ToString().ToLowerInvariant()})\nreturn true");
				foreach (var item in _db.GameItems.Local.Where(x => x.GameItemProtoId == id))
				{
					var effects = XElement.Parse(item.EffectData ?? "<Effects/>");
					if (!effects.Elements("Effect").Any(x => (string?)x.Element("Type") == "EtherealItemAffinity"))
						effects.Add(new XElement("Effect", new XElement("ApplicabilityProg", 0), new XElement("Type", "EtherealItemAffinity"),
							new XElement("Original", 0), new XElement("Remaining", 0),
							new XElement("Definition", new XElement("PlaneId", shadow), new XElement("ReadyForTransition", ready),
								new XElement("VisibleToPlane", visible))));
					item.EffectData = effects.ToString();
				}
			}
			if (Has(15)) Effect("ImmortalItem", "addadmininviseffect(@item)", "AdminInvis", new XElement("Effect"));
			if (Has(18))
			{
				var strength = Math.Max(0, source.Physical[2]);
				var hpDice = strength == 0 ? "0" : $"{strength}d4";
				var stunDice = strength == 0 ? "0" : $"{strength}d6";
				const string glyphInitialised = "flag:glyph";
				const string auraInitialised = "flag:magic";
				var glyphInit = $"if (not(initialiseitemonce(@item, \"{glyphInitialised}\")))\nreturn true\nend if\n" +
					$"addpickuphazard(@item, \"{hpDice}\", \"{stunDice}\", {glyphImmunity}, 3, \"As you reach for it, the glyph flares and its magick strikes at you.\")\n" +
					$"if (initialiseitemonce(@item, \"{auraInitialised}\"))\napplyitempresentationeffect(@item, \"magic\")\nend if\nreturn true";
				AddItemOnLoadProg(proto, "SourceGlyph" + id, glyphInit);
				foreach (var item in _db.GameItems.Local.Where(x => x.GameItemProtoId == id))
				{
					var effects = XElement.Parse(item.EffectData ?? "<Effects/>");
					if (!effects.Elements("Effect").Any(x => (string?)x.Element("Type") == "ItemPickupHazard"))
					{
						effects.Add(new XElement("Effect", new XElement("ApplicabilityProg", 0), new XElement("Type", "ItemPickupHazard"),
							new XElement("Original", 0), new XElement("Remaining", 0),
							new XElement("Hazard", new XElement("DamageDice", hpDice), new XElement("StunDice", stunDice),
								new XElement("ImmunityProg", glyphImmunity), new XElement("MinimumDispelGrade", 3),
								new XElement("Spent", false), new XElement("Echo", new XCData("As you reach for it, the glyph flares and its magick strikes at you.")))));
					}
					if (!effects.Elements("Effect").Any(x => (string?)x.Element("Type") == "ItemMagicAura"))
						effects.Add(new XElement("Effect", new XElement("ApplicabilityProg", 0), new XElement("Type", "ItemMagicAura"),
							new XElement("Original", 0), new XElement("Remaining", 0), new XElement("Effect")));
					AddItemInitialisationKey(effects, glyphInitialised, auraInitialised);
					item.EffectData = effects.ToString();
				}
			}
			Record("itemflags." + id, nameof(GameItemProto), id, proto.RevisionNumber, new
			{
				SourceVnum = source.Vnum, ExtraFlags = flags, WearFlags = source.Flags[2], Restrictions = restrictions.ToString(),
				GlyphStrength = Has(18) ? source.Physical[2] : (int?)null,
					Disposition = "Glyph is a permanent saved one-shot damaging pickup hazard with the source strength dice; Unyielding Veil permits pickup without firing or consuming it. ETHEREAL_OK is a saved readiness marker and ETHEREAL is saved detection visibility; both use native plane-transition retention. Explicit keyword targeting remains available for HIDDEN. Scripted creation and initial outfit fitting bypass pickup restrictions. GLOW adds no light. NOSAVE uses lifecycle retirement. DISINTEGRATE has explicit disposal triggers, never ordinary single drops or ordinary death. Unconsumed flags remain provenance."
			});
		}
		Flush();
	}

	private long InstallSourceItemMagic(long? shadowPlaneId)
	{
		var immunitySpellId = OwnedBehaviourId("arm.spell.unyielding_veil", nameof(MagicSpell));
		var hasSourceGlyph = _effectiveItems.Any(x => (x.Value.Source.Flags[1] & (1 << 18)) != 0);
		var sourceGlyphSpell = OwnedBehaviourId("arm.spell.custodian_glyph", nameof(MagicSpell));
		if ((hasSourceGlyph || sourceGlyphSpell is not null) && immunitySpellId is null)
			throw new InvalidOperationException("Source pickup glyphs require the imported Unyielding Veil spell for their approved immunity exception.");
		var immunityProg = immunitySpellId is { } spellId
			? Prog("ArmageddonWorldfileGlyphVeilImmunity",
				$"return contains(activespells(@actor), tomagicspell({spellId}))", ProgVariableTypes.Boolean,
				("actor", ProgVariableTypes.Character), ("item", ProgVariableTypes.Item)).Id
			: 0;

		if (sourceGlyphSpell is { } glyphId)
		{
			var glyph = _db.MagicSpells.Local.FirstOrDefault(x => x.Id == glyphId) ?? _db.MagicSpells.Single(x => x.Id == glyphId);
			var xml = XElement.Parse(glyph.Definition);
			var effects = xml.Element("Effects") ?? throw new InvalidOperationException("Custodian Glyph has no effect collection.");
			effects.ReplaceNodes(new XElement("Effect", new XAttribute("type", "pickuphazard"),
				new XElement("DamageSides", 4), new XElement("StunSides", 6), new XElement("DicePerGrade", 1),
				new XElement("ImmunityProg", immunityProg), new XElement("MinimumDispelGrade", 3),
				new XElement("Echo", new XCData("As you reach for it, the glyph flares and its magick strikes at you."))));
			glyph.Description = "Place a permanent, owner-agnostic glyph upon an item. The next unprotected pickup inflicts one d4 of HP damage and one d6 of stun per grade, consumes the glyph, and stops the pickup. Unyielding Veil permits pickup without triggering or consuming it. A grade-three dispel can remove the glyph.";
			glyph.Definition = xml.ToString();
			Record("itemmagic.custodian-glyph", nameof(MagicSpell), glyph.Id, null, new
			{
				ImmunitySpell = immunitySpellId,
				Disposition = "Worldfile imports override the setting's finite ownership glyph with the source's permanent one-shot damaging glyph. Unyielding Veil is a pickup exemption, not global immunity."
			});
		}

		if (OwnedBehaviourId("arm.spell.unravel_enchantment", nameof(MagicSpell)) is { } dispelId)
		{
			var dispel = _db.MagicSpells.Local.FirstOrDefault(x => x.Id == dispelId) ?? _db.MagicSpells.Single(x => x.Id == dispelId);
			var xml = XElement.Parse(dispel.Definition);
			var trigger = xml.Element("Trigger") ?? throw new InvalidOperationException("Unravel Enchantment has no trigger.");
			trigger.SetElementValue("CanTargetItems", true);
			var effects = xml.Element("Effects") ?? throw new InvalidOperationException("Unravel Enchantment has no effect collection.");
			var effect = effects.Elements().Single(x => (string?)x.Attribute("type") == "dispelmagic");
			effect.SetElementValue("IncludeIntrinsicItemEffects", true);
			dispel.Definition = xml.ToString();
			Record("itemmagic.intrinsic-dispel", nameof(MagicSpell), dispel.Id, null, new
			{
				Disposition = "Unravel Enchantment retains its contested dispel for spell-owned effects. Its unrestricted item targeting also removes eligible intrinsic item-magic effects through the native Dispel implementation."
			});
		}

		InstallSourceEtherealSpells(shadowPlaneId);

		return immunityProg;
	}

	private void InstallSourceEtherealSpells(long? shadowPlaneId)
	{
		if (shadowPlaneId is not { } planeId) return;
		if (OwnedBehaviourId("arm.spell.shadow_passage", nameof(MagicSpell)) is { } passageId)
		{
			var passage = _db.MagicSpells.Local.FirstOrDefault(x => x.Id == passageId) ?? _db.MagicSpells.Single(x => x.Id == passageId);
			var xml = XElement.Parse(passage.Definition);
			var trigger = xml.Element("Trigger") ?? throw new InvalidOperationException("Shadow Passage has no trigger.");
			trigger.SetElementValue("CanTargetItems", true);
			var planarData = xml.Element("Effects")?.Elements().SingleOrDefault(x => (string?)x.Attribute("type") == "planarstate")?.Element("PlanarData");
			if (planarData is null) throw new InvalidOperationException("Shadow Passage has no saved character planar-state payload.");
			var characterPlanarData = new XElement(planarData);
			var planarFlags = characterPlanarData.Element("Flags");
			if (planarFlags is null)
			{
				planarFlags = new XElement("Flags");
				characterPlanarData.Add(planarFlags);
			}
			planarFlags.SetAttributeValue("retainPlaneSpecificInventory", true);
			var effect = new XElement("Effect", new XAttribute("type", "etherealitemattunement"),
				new XElement("PlaneId", planeId), new XElement("AutoAdvance", true), new XElement("MassPerGrade", 3.0 * _kilograms),
				new XElement("RiskPercentPerGrade", 1),
				new XElement("CharacterPlanarData", characterPlanarData));
			xml.Element("Effects")!.ReplaceNodes(effect);
			passage.Definition = xml.ToString();
			Record("itemmagic.shadow-passage", nameof(MagicSpell), passage.Id, null, new
			{
				Disposition = "Character casting retains the installed planar-state effect; item casting prepares then reveals the item with strict three-kilogram-per-grade eligibility and source grade-percent destruction risk."
			});
		}

		if (OwnedBehaviourId("arm.spell.draw_into_flesh", nameof(MagicSpell)) is { } returnId)
		{
			var spell = _db.MagicSpells.Local.FirstOrDefault(x => x.Id == returnId) ?? _db.MagicSpells.Single(x => x.Id == returnId);
			var xml = XElement.Parse(spell.Definition);
			var trigger = xml.Element("Trigger") ?? throw new InvalidOperationException("Draw Into Flesh has no trigger.");
			trigger.SetElementValue("CanTargetItems", true);
			var effect = xml.Element("Effects")?.Elements().SingleOrDefault(x => (string?)x.Attribute("type") == "removeplanarstate");
			if (effect is not null)
			{
				effect.SetElementValue("OnlySpellOwnedEthereal", true);
				effect.SetElementValue("ClearItemEtherealVisibility", true);
			}
			spell.Definition = xml.ToString();
			Record("itemmagic.draw-into-flesh", nameof(MagicSpell), spell.Id, null, new
			{
				Disposition = "Item targeting clears detection visibility from an ethereal item while retaining its readiness for later Shadow travel. Character casting removes only this spell-owned planar state."
			});
		}
	}

	private static void AddItemInitialisationKey(XElement effects, params string[] keys)
	{
		var initialisation = effects.Elements("Effect").SingleOrDefault(x => (string?)x.Element("Type") == "ItemInitialisation");
		if (initialisation is null)
		{
			initialisation = new XElement("Effect", new XElement("ApplicabilityProg", 0), new XElement("Type", "ItemInitialisation"),
				new XElement("Original", 0), new XElement("Remaining", 0), new XElement("Keys"));
			effects.Add(initialisation);
		}
		var keyList = initialisation.Element("Keys") ?? new XElement("Keys");
		if (keyList.Parent is null) initialisation.Add(keyList);
		foreach (var key in keys.Where(x => !keyList.Elements("Key").Any(y => y.Value == x)))
			keyList.Add(new XElement("Key", new XCData(key)));
	}
}

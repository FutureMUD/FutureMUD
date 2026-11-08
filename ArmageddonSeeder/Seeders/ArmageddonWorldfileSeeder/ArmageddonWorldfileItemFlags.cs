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
		var shadow = OwnedBehaviourId("arm.support.plane.shadow", nameof(Plane)) ?? 0;
		foreach (var (id, effective) in _effectiveItems)
		{
			var source = effective.Source;
			var flags = source.Flags[1];
			bool Has(int bit) => (flags & (1 << bit)) != 0;
			var proto = _db.GameItemProtos.Local.Single(x => x.Id == id);
			var restrictions = ResolveItemRestrictions(flags, source.Flags[2]);
			long glyphProg = 0;
			if (Has(18))
			{
				var strength = Math.Max(0, source.Physical[2]);
				var damage = strength == 0 ? "" : $"adjusthitpoints(@actor, 0 - ({string.Join(" + ", Enumerable.Repeat("random(1, 4)", strength))}), {string.Join(" + ", Enumerable.Repeat("random(1, 6)", strength))})\n";
				glyphProg = Prog($"ArmageddonWorldfileGlyph{id}",
					"send(@actor, \"As you reach for it, the glyph flares and its magick strikes at you.\")\n" + damage +
					"clearitemmagicaura(@item)\nreturn false", ProgVariableTypes.Boolean,
					("actor", ProgVariableTypes.Character), ("item", ProgVariableTypes.Item)).Id;
			}
			if (restrictions != ItemInteractionRestriction.None || glyphProg != 0)
				AttachDeliveryComponent(proto, Component("itemflags.permissions." + id, "Item Permissions", new XElement("Definition",
					new XElement("Restrictions", (int)restrictions), new XElement("PickupAttemptProg", glyphProg), new XElement("PickupAttemptOnceOnly", true))), new XElement("Definition"));
			if (Has(25) || Has(12))
				AttachDeliveryComponent(proto, Component("itemflags.retirement." + id, "Transient Item", new XElement("Definition",
					new XElement("RetirementPlane", shadow), new XElement("RetireOnReload", Has(25)), new XElement("RetireOnLogout", Has(25)),
					new XElement("Triggers", Has(12) ? (int)(ItemRetirementTrigger.BulkDrop | ItemRetirementTrigger.EtherealTransition | ItemRetirementTrigger.TransientBodyDeath | ItemRetirementTrigger.HostBodyChange) : 0))), new XElement("Definition"));

			void Effect(string suffix, string command, string type, XElement data)
			{
				AddItemOnLoadProg(proto, suffix, command + "\nreturn true");
				foreach (var item in _db.GameItems.Local.Where(x => x.GameItemProtoId == id))
				{
					var effects = XElement.Parse(item.EffectData ?? "<Effects/>");
					effects.Add(new XElement("Effect", new XElement("ApplicabilityProg", 0), new XElement("Type", type),
						new XElement("Original", 0), new XElement("Remaining", 0), new XElement(data)));
					item.EffectData = effects.ToString();
				}
			}
			if (Has(0)) Effect("DescriptiveGlow", "applyitempresentationeffect(@item, \"glow\")", "ItemPresentation", new XElement("Humming", false));
			if (Has(1)) Effect("LocalHum", "applyitempresentationeffect(@item, \"hum\")", "ItemPresentation", new XElement("Humming", true));
			if (Has(6)) Effect("IntrinsicMagic", "applyitempresentationeffect(@item, \"magic\")", "ItemMagicAura", new XElement("Effect"));
			if (Has(5)) Effect("InvisibleItem", "adddetectionvisibilityeffect(@item, \"VisualMagical\", 0)", "DetectionVisibility",
				new XElement("Visibility", new XAttribute("senses", (long)PerceptionTypes.VisualMagical), new XAttribute("plane", 0)));
			if (Has(14)) Effect("EtherealVisibility", $"adddetectionvisibilityeffect(@item, \"VisualEthereal, SenseEthereal\", {shadow})", "DetectionVisibility",
				new XElement("Visibility", new XAttribute("senses", (long)(PerceptionTypes.VisualEthereal | PerceptionTypes.SenseEthereal)), new XAttribute("plane", shadow)));
			if (Has(15)) Effect("ImmortalItem", "addadmininviseffect(@item)", "AdminInvis", new XElement("Effect"));
			Record("itemflags." + id, nameof(GameItemProto), id, proto.RevisionNumber, new
			{
				SourceVnum = source.Vnum, ExtraFlags = flags, WearFlags = source.Flags[2], Restrictions = restrictions.ToString(),
				GlyphStrength = Has(18) ? source.Physical[2] : (int?)null,
				Disposition = "Explicit keyword targeting remains available for HIDDEN. Scripted creation and initial outfit fitting bypass pickup restrictions. GLOW adds no light. ETHEREAL changes visibility only. NOSAVE uses lifecycle retirement. DISINTEGRATE has explicit disposal triggers, never ordinary single drops or ordinary death. Unconsumed flags remain provenance."
			});
		}
		Flush();
	}
}

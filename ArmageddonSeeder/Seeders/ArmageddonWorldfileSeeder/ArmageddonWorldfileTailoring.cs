#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private const string TailorVnumRegister = "armw_tailor_vnum";
	private const string TailorItemRegister = "armw_tailor_item";
	private const string TailorReceiptRegister = "armw_tailor_receipt";
	private const string TailorPrototypeRegister = "armw_tailor_proto";
	private const string TailorFitRegister = "armw_tailor_fit";
	private const string TailorDueRegister = "armw_tailor_due";
	private const string TailorPriceRegister = "armw_tailor_price";

	private void InstallTailoring()
	{
		var requested = new[] { 6157, 20038, 21013, 28305, 37746, 50454, 64045, 70105 };
		var tailors = _source.Npcs.Where(x => requested.Contains(x.Vnum) && _npcs.ContainsKey(x.Vnum) &&
			x.Programs.Contains("C tailor tailor") && x.Programs.Contains("C offer tailor") &&
			_source.Resets.Any(r => r.Opcode == "M" && r.ReferenceIssues.Length == 0 && r.Arguments.Length > 3 && r.Arguments[1] == x.Vnum))
			.ToArray();
		if (tailors.Length == 0) return;
		if (!_items.TryGetValue(1598, out var ticketPrototype))
			throw new InvalidOperationException("Tailoring commissions require retained source ticket prototype 1598.");

		var sourceWearables = _source.Items.Where(x => x.ItemType is 9 or 11 && _items.ContainsKey(x.Vnum) &&
			(x.Flags[1] & (1 << 6)) == 0).ToArray();
		var eligiblePrototypes = sourceWearables.SelectMany(x => new[] { _items[x.Vnum] }
			.Concat(_resetItemVariants.Where(y => y.Key.StartsWith(x.Vnum + ":", StringComparison.Ordinal)).Select(y => y.Value)))
			.DistinctBy(x => x.Id).ToArray();
		if (eligiblePrototypes.Length == 0) throw new InvalidOperationException("Tailoring requires retained imported armour or worn item prototypes.");

		DefineTailorRegister(TailorVnumRegister, ProgVariableTypes.Number);
		DefineTailorRegister(TailorItemRegister, ProgVariableTypes.Number);
		DefineTailorRegister(TailorReceiptRegister, ProgVariableTypes.Number);
		DefineTailorRegister(TailorPrototypeRegister, ProgVariableTypes.Number);
		DefineTailorRegister(TailorFitRegister, ProgVariableTypes.Number);
		DefineTailorRegister(TailorDueRegister, ProgVariableTypes.DateTime);
		DefineTailorRegister(TailorPriceRegister, ProgVariableTypes.Number);

		var resetPlacements = tailors.SelectMany(npc => _source.Resets.Where(r => r.Opcode == "M" && r.ReferenceIssues.Length == 0 &&
			r.Arguments.Length > 3 && r.Arguments[1] == npc.Vnum).Select(r => (Npc: npc, Room: _source.Rooms.Single(x => x.Vnum == r.Arguments[3]))))
			.DistinctBy(x => (x.Npc.Vnum, x.Room.Vnum)).ToArray();
		var custody = resetPlacements.ToDictionary(x => (x.Npc.Vnum, x.Room.Vnum), x => CreateAuxiliaryRoom(x.Room.Zone,
			$"tailor-custody.Vnum{x.Npc.Vnum}.Room{x.Room.Vnum}", "Tailor's Workroom", "A private workroom used to hold commissioned garments."));
		var ticketId = ticketPrototype.Id;
		Prog("ArmageddonWorldfileTailorTicketMatches", "return getregister(@item, \"armw_tailor_receipt\") == @chit.id", ProgVariableTypes.Boolean,
			("item", ProgVariableTypes.Item), ("chit", ProgVariableTypes.Item));
		Prog("ArmageddonWorldfileTailorConditionDeficit",
			"return floor(max(0, @capacity) * (1 - max(0, min(1, @condition))))", ProgVariableTypes.Number,
			("capacity", ProgVariableTypes.Number), ("condition", ProgVariableTypes.Number));
		var profile = new StringBuilder("var sourceSize = 0\nvar damageDiff = 0\nvar metallic = false\nvar repairTime = 0\nvar eligible = false\n");
		foreach (var source in sourceWearables)
		{
			var variants = new[] { _items[source.Vnum] }.Concat(_resetItemVariants.Where(y => y.Key.StartsWith(source.Vnum + ":", StringComparison.Ordinal)).Select(y => y.Value)).DistinctBy(x => x.Id);
			foreach (var prototype in variants)
			{
				if (!_effectiveItems.TryGetValue(prototype.Id, out var effective)) continue;
				var values = effective.Values;
				var sourceSize = source.ItemType == 11 ? values[0] : values[2];
				var damageDiff = source.ItemType == 11 ? "1" : $"@ArmageddonWorldfileTailorConditionDeficit({Math.Max(0, values[1])}, @item.condition)";
				var repairTime = sourceSize;
				var metal = source.Material == 2;
				profile.AppendLine($"if (@item.proto == {prototype.Id})\nsourceSize = {sourceSize}\ndamageDiff = {damageDiff}\nrepairTime = {repairTime}\nmetallic = {(metal ? "true" : "false")}\neligible = true\nend if");
			}
		}
		var profileCode = profile.ToString();

		var admission = VendorAdmission + "\nif (@actor.npctemplateid != 0 or not(@actor.isawake) or @args.Count(word, true) < 3 or getindex(@args, 1) != \"at\")\nreturn false\nend if\n";
		var tailorBody = new StringBuilder(admission + "var tailorVnum = 0\n");
		foreach (var source in tailors)
		foreach (var templateId in _npcFamilyIds.GetValueOrDefault(source.Vnum, [_npcs[source.Vnum].Id]))
			tailorBody.AppendLine($"if (@seller.npctemplateid == {templateId})\ntailorVnum = {source.Vnum}\nend if");
		tailorBody.Append("if (@tailorVnum == 0 or targetcharacter(@actor, lastword(@input)) != @seller or @args.Count(word, true) == 0)\nreturn false\nend if\nvar query = getindex(@args, 0)\nvar item = targetitem(@actor, @actor.inventory, @query)\nif (isnull(@item))\nreturn false\nend if\n");
		tailorBody.Append(profileCode);
		tailorBody.AppendLine("if (not(@eligible) or wearablefit(@item) == \"\")\nforce @seller \"say I can only take source garments I know how to fit.\"\nreturn true\nend if");
		tailorBody.AppendLine("if (not(isnull(@item.wearer)))\nforce @seller \"say I need the garment off your body first.\"\nreturn true\nend if\nsourceSize = wearablefitscalar(@item)\nrepairTime = @sourceSize + @damageDiff\nvar targetSize = @ArmageddonWorldfileScalarWearableSize(@actor)\nvar multiplier = 1\nif (@targetSize > @sourceSize)\nmultiplier = 2\nend if\nvar rate = 3\nif (@metallic)\nrate = 33\nend if\nvar price = @damageDiff * @rate * @multiplier\nif (not(@metallic))\nprice = 5 + @damageDiff * @rate * @multiplier\nend if\nvar hours = max(1, max(@targetSize - @repairTime, @repairTime - @targetSize))");
		tailorBody.AppendLine("var custodyRoom = 0");
		foreach (var placement in resetPlacements)
			tailorBody.AppendLine($"if (@tailorVnum == {placement.Npc.Vnum} and @seller.location == tolocation({_rooms[placement.Room.Vnum].Id}))\ncustodyRoom = {custody[(placement.Npc.Vnum, placement.Room.Vnum)].Id}\nend if");
		tailorBody.AppendLine("if (@custodyRoom == 0)\nreturn false\nend if");
		tailorBody.AppendLine($"var chit = loaditem({ticketId})\nif (isnull(@chit))\nforce @seller \"say I cannot issue a work ticket right now.\"\nreturn true\nend if\nsetregister @chit \"{TailorVnumRegister}\" @tailorVnum\nsetregister @chit \"{TailorPrototypeRegister}\" @item.proto\nsetregister @chit \"{TailorFitRegister}\" @targetSize\nsetregister @chit \"{TailorDueRegister}\" (now() + timespanfromseconds(@hours * 600))\nsetregister @chit \"{TailorPriceRegister}\" @price");
		tailorBody.AppendLine($"setregister @chit \"{TailorItemRegister}\" @item.id\nif (not(silentget(@actor, @chit)))\ndeleteitem(@chit)\nforce @seller \"say I cannot give you a work ticket.\"\nreturn true\nend if\nif (not(put(@item, tolocation(@custodyRoom))))\ndeleteitem(@chit)\nforce @seller \"say I cannot take custody of that garment.\"\nreturn true\nend if\nsetregister @item \"{TailorReceiptRegister}\" @chit.id\nsend(@actor, \"You leave $0 with $1 for fitting; the charge is \" + @price + \" bits and it will be ready in \" + @hours + \" hours.\", @item, @seller)\nsendloc(@actor.location, \"$0 leaves a garment with $1 for fitting.\", @actor, @seller)\nreturn true");

		var offerBody = new StringBuilder(admission + "var tailorVnum = 0\n");
		foreach (var source in tailors)
		foreach (var templateId in _npcFamilyIds.GetValueOrDefault(source.Vnum, [_npcs[source.Vnum].Id]))
			offerBody.AppendLine($"if (@seller.npctemplateid == {templateId})\ntailorVnum = {source.Vnum}\nend if");
		offerBody.Append("if (@tailorVnum == 0 or targetcharacter(@actor, lastword(@input)) != @seller or @args.Count(word, true) == 0)\nreturn false\nend if\nvar query = getindex(@args, 0)\nvar item = targetitem(@actor, @actor.inventory, @query)\nif (isnull(@item))\nreturn false\nend if\n");
		offerBody.Append(profileCode);
		offerBody.AppendLine("if (not(@eligible) or wearablefit(@item) == \"\" or not(isnull(@item.wearer)))\nreturn false\nend if\nsourceSize = wearablefitscalar(@item)\nrepairTime = @sourceSize + @damageDiff\nvar targetSize = @ArmageddonWorldfileScalarWearableSize(@actor)\nvar multiplier = 1\nif (@targetSize > @sourceSize)\nmultiplier = 2\nend if\nvar rate = 3\nif (@metallic)\nrate = 33\nend if\nvar price = @damageDiff * @rate * @multiplier\nif (not(@metallic))\nprice = 5 + @damageDiff * @rate * @multiplier\nend if\nvar hours = max(1, max(@targetSize - @repairTime, @repairTime - @targetSize))\nsend(@actor, \"The fitting will cost \" + @price + \" bits and take about \" + @hours + \" hours.\")\nreturn true");

		var collectBody = new StringBuilder(admission + "var tailorVnum = 0\n");
		foreach (var source in tailors)
		foreach (var templateId in _npcFamilyIds.GetValueOrDefault(source.Vnum, [_npcs[source.Vnum].Id]))
			collectBody.AppendLine($"if (@seller.npctemplateid == {templateId})\ntailorVnum = {source.Vnum}\nend if");
		collectBody.Append("if (@tailorVnum == 0 or targetcharacter(@actor, lastword(@input)) != @seller or @args.Count(word, true) == 0)\nreturn false\nend if\nvar query = getindex(@args, 0)\nvar chit = targetitem(@actor, @actor.inventory, @query)\n");
		collectBody.AppendLine($"if (isnull(@chit) or @chit.proto != {ticketId} or getregister(@chit, \"{TailorVnumRegister}\") != @tailorVnum)\nreturn false\nend if\nvar itemid = getregister(@chit, \"{TailorItemRegister}\")\nvar item = getexistingitem(@itemid)\nvar targetSize = getregister(@chit, \"{TailorFitRegister}\")\nvar due = getregister(@chit, \"{TailorDueRegister}\")\nvar price = getregister(@chit, \"{TailorPriceRegister}\")\nvar custodyRoom = 0");
		foreach (var placement in resetPlacements)
			collectBody.AppendLine($"if (@tailorVnum == {placement.Npc.Vnum} and @seller.location == tolocation({_rooms[placement.Room.Vnum].Id}))\ncustodyRoom = {custody[(placement.Npc.Vnum, placement.Room.Vnum)].Id}\nend if");
		collectBody.AppendLine("if (@custodyRoom == 0 or @itemid == 0 or isnull(@item) or @item.proto != getregister(@chit, \"" + TailorPrototypeRegister + "\") or @item.location != tolocation(@custodyRoom))\nforce @seller \"say That ticket does not identify a garment in my workroom.\"\nreturn true\nend if\nif (not(@ArmageddonWorldfileTailorTicketMatches(@item, @chit)))\nforce @seller \"say That is not the original work ticket.\"\nreturn true\nend if\nif (@due > now())\nforce @seller \"say That garment is not ready yet.\"\nreturn true\nend if\nvar fitText = \"\" + @targetSize\nif (not(purchaseitemdelivery(@actor, @seller, @item, tolocation(@custodyRoom), tocurrency(" + _currency.Id + "), @price, @fitText)))\nforce @seller \"say I cannot complete the fitting and payment right now.\"\nreturn true\nend if\nsetregister @item \"armw_tailor_receipt\" 0\ndeleteitem(@chit)\nsilentget(@actor, @item)\nsend(@actor, \"You pay \" + @price + \" bits and collect the fitted garment.\")\nsendloc(@actor.location, \"$0 pays $1 and collects a garment.\", @actor, @seller)\nreturn true");

		var tailoring = NpcServiceProg("ArmageddonWorldfileTailorOrder", tailorBody.ToString());
		var quoting = NpcServiceProg("ArmageddonWorldfileTailorQuote", offerBody.ToString());
		var collection = NpcServiceProg("ArmageddonWorldfileTailorCollection", collectBody.ToString());
		var sourceIds = tailors.Select(x => x.Vnum).ToArray();
		InstallNpcInputHook("Tailoring Commission", "tailor", tailoring, sourceIds);
		InstallNpcInputHook("Tailoring Quote", "offer", quoting, sourceIds);
		InstallNpcInputHook("Tailoring Collection", "offer", collection, sourceIds);
		_counts["tailoring commissions"] = tailors.Length;
		Record("npc-service.tailoring", "NativeTailoringCommission", null, null, new
		{
			Tailors = tailors.Select(x => x.Vnum).OrderBy(x => x), TicketPrototype = 1598,
			CustodyRooms = resetPlacements.Length, EligibleSourceWearables = eligiblePrototypes.Length,
			DelaySecondsPerSourceHour = 600,
			Disposition = "Source-backed tailor quote/order/collection hooks use only retained imported type-9 armour and type-11 worn prototype families, including reset variants. Order binds the quoted charge and due time, snapshots the source-compatible scalar fit, holds the exact item in an unlinked per-tailor workroom and issues source ticket 1598. Collection validates the exact ticket, NPC, item identity, prototype, custody room and ready time before native currency transfer and fit application. No scheduler is used; completion is checked at collection. Native condition supplies the armour protection deficit for pricing and timing and is left unchanged by resizing. Foreign/custom wearable prototypes are declined because no source size/protection data exists."
		});
		Flush();
	}

	private void DefineTailorRegister(string name, ProgVariableTypes type)
	{
		var owner = ProgVariableTypes.Item.ToStorageString();
		var definitions = _db.VariableDefinitions.Where(x => x.OwnerTypeDefinition == owner && x.Property == name).ToArray();
		if (definitions.Length > 1 || definitions.Length == 1 && definitions[0].ContainedTypeDefinition != type.ToStorageString())
			throw new InvalidOperationException($"Item register {name} is ambiguous or has a different type.");
		if (definitions.Length == 0) _db.VariableDefinitions.Add(new VariableDefinition
		{
			OwnerTypeDefinition = owner, Property = name, ContainedTypeDefinition = type.ToStorageString()
		});
		var defaults = _db.VariableDefaults.Where(x => x.OwnerTypeDefinition == owner && x.Property == name).ToArray();
		if (defaults.Length > 1) throw new InvalidOperationException($"Item register {name} has ambiguous defaults.");
		if (defaults.Length == 0) _db.VariableDefaults.Add(new VariableDefault
		{
			OwnerTypeDefinition = owner, Property = name, DefaultValue = type == ProgVariableTypes.Text ? "<var></var>" : type == ProgVariableTypes.DateTime ? "<var>0001-01-01T00:00:00.0000000Z</var>" : "<var>0</var>"
		});
	}
}

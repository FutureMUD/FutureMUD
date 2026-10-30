#nullable enable

using System;
using System.Linq;
using System.Text;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private const string WagonDepotRegister = "armw_wagon_depot";
	private const string WagonIdRegister = "armw_wagon_id";
	private const string WagonTicketRegister = "armw_wagon_ticket";

	private void InstallWagonStorage()
	{
		var sourceNpcs = _source.Npcs.Where(x => new[] { 3842, 20119, 50455, 50605 }.Contains(x.Vnum) && _npcs.ContainsKey(x.Vnum) &&
			x.Programs.Contains("C rent wagon_to_store") && x.Programs.Contains("C offer wagon_to_store") &&
			_source.Resets.Any(r => r.Opcode == "M" && r.ReferenceIssues.Length == 0 && r.Arguments.Length > 3 && r.Arguments[1] == x.Vnum))
			.ToDictionary(x => x.Vnum);
		if (sourceNpcs.Count == 0) return;

		if (!_items.TryGetValue(1597, out var ticketPrototype))
			throw new InvalidOperationException("Wagon storage requires retained source ticket prototype 1597.");

		foreach (var register in new[] { WagonDepotRegister, WagonIdRegister, WagonTicketRegister })
		{
			var owner = ProgVariableTypes.Item.ToStorageString();
			var definitions = _db.VariableDefinitions.Where(x => x.OwnerTypeDefinition == owner && x.Property == register).ToArray();
			if (definitions.Length > 1) throw new InvalidOperationException($"Item register {register} is ambiguous.");
			if (definitions.Length == 1)
			{
				if (definitions[0].ContainedTypeDefinition != ProgVariableTypes.Number.ToStorageString())
					throw new InvalidOperationException($"Item register {register} already exists with a non-numeric type.");
			}
			else
			{
				_db.VariableDefinitions.Add(new VariableDefinition { OwnerTypeDefinition = owner, Property = register,
					ContainedTypeDefinition = ProgVariableTypes.Number.ToStorageString() });
			}
			var defaults = _db.VariableDefaults.Where(x => x.OwnerTypeDefinition == owner && x.Property == register).ToArray();
			if (defaults.Length > 1) throw new InvalidOperationException($"Item register {register} has ambiguous defaults.");
			if (defaults.Length == 0)
				_db.VariableDefaults.Add(new VariableDefault { OwnerTypeDefinition = owner, Property = register, DefaultValue = "<var>0</var>" });
		}

		var depots = _source.Resets.Where(x => x.Opcode == "M" && x.ReferenceIssues.Length == 0 && x.Arguments.Length > 3 &&
			sourceNpcs.ContainsKey(x.Arguments[1]))
			.Select(x => (Npc: sourceNpcs[x.Arguments[1]], Frontage: _source.Rooms.Single(r => r.Vnum == x.Arguments[3])))
			.DistinctBy(x => (x.Npc.Vnum, x.Frontage.Vnum)).ToArray();
		var yards = depots.ToDictionary(x => x.Npc.Vnum + ":" + x.Frontage.Vnum, x => CreateAuxiliaryRoom(
			x.Frontage.Zone, $"wagon-depot.Vnum{x.Npc.Vnum}.Vnum{x.Frontage.Vnum}", "Wagon Storage Yard",
			"A wagon storage yard stands ready here."));
		var ticket = _items[1597].Id;
		var wagonIds = _source.Items.Where(x => x.ItemType == 23 && _items.ContainsKey(x.Vnum))
			.SelectMany(x => ItemFamily(x.Vnum)).Select(x => x.Id).Distinct().ToArray();
		var rentBody = new StringBuilder(VendorAdmission).AppendLine();
		rentBody.AppendLine("if (@args.Count(word, true) == 0)\nreturn false\nend if\nvar query = getindex(@args, 0)");
		rentBody.AppendLine("var depot = 0");
		foreach (var depot in depots)
		{
			var yard = yards[$"{depot.Npc.Vnum}:{depot.Frontage.Vnum}"];
			rentBody.AppendLine($"if (@seller.location == tolocation({_rooms[depot.Frontage.Vnum].Id}))\ndepot = {yard.Id}\nend if");
		}
		rentBody.AppendLine("if (@depot == 0)\nreturn false\nend if");
		rentBody.AppendLine("var wagon = targetitem(@actor, @actor.location.items, @query)");
		rentBody.AppendLine("if (isnull(@wagon) or not(isvehicle(@wagon)))\nreturn false\nend if");
		rentBody.AppendLine("var eligible = false");
		foreach (var id in wagonIds) rentBody.AppendLine($"if (@wagon.proto == {id})\neligible = true\nend if");
		rentBody.AppendLine("if (not(@eligible))\nforce @seller \"say That is not a wagon I can store.\"\nreturn true\nend if");
		rentBody.AppendLine("if (vehicleoccupied(@wagon))\nforce @seller \"say Everyone must leave the wagon before I can store it.\"\nreturn true\nend if");
		rentBody.AppendLine($"var chit = loaditem({ticket})\nif (isnull(@chit))\nforce @seller \"say I cannot issue a storage ticket right now.\"\nreturn true\nend if");
		rentBody.AppendLine($"setregister @chit \"{WagonDepotRegister}\" @depot");
		rentBody.AppendLine($"setregister @chit \"{WagonIdRegister}\" @wagon.id");
		rentBody.AppendLine("if (not(silentget(@actor, @chit)))\ndeleteitem(@chit)\nforce @seller \"say I cannot hand you a storage ticket.\"\nreturn true\nend if");
		rentBody.AppendLine($"setregister @wagon \"{WagonTicketRegister}\" @chit.id");
		rentBody.AppendLine($"if (not(relocateemptyvehicle(@wagon, tolocation(@depot), true)))\nsetregister @wagon \"{WagonTicketRegister}\" 0\nsetregister @chit \"{WagonDepotRegister}\" 0\nsetregister @chit \"{WagonIdRegister}\" 0\ndeleteitem(@chit)\nforce @seller \"say I cannot move that wagon into the yard.\"\nreturn true\nend if");
		rentBody.AppendLine("send(@actor, \"You receive a wagon storage ticket from $0.\", @seller)\nsendloc(@actor.location, \"$0 stores a wagon with $1.\", @actor, @seller)\nreturn true");
		var offerBody = new StringBuilder(VendorAdmission).AppendLine();
		offerBody.AppendLine("if (@args.Count(word, true) == 0)\nreturn false\nend if\nvar query = getindex(@args, 0)\nvar chit = targetitem(@actor, @actor.inventory, @query)");
		offerBody.AppendLine("var sellerDepot = 0");
		foreach (var depot in depots)
		{
			var yard = yards[$"{depot.Npc.Vnum}:{depot.Frontage.Vnum}"];
			offerBody.AppendLine($"if (@seller.location == tolocation({_rooms[depot.Frontage.Vnum].Id}))\nsellerDepot = {yard.Id}\nend if");
		}
		offerBody.AppendLine($"if (isnull(@chit) or @chit.proto != {ticket})\nreturn false\nend if\nvar depot = getregister(@chit, \"{WagonDepotRegister}\")\nvar wagonid = getregister(@chit, \"{WagonIdRegister}\")\nvar wagon = getexistingitem(@wagonid)");
		offerBody.AppendLine($"if (@depot == 0 or @depot != @sellerDepot or @wagonid == 0 or isnull(@wagon) or getregister(@wagon, \"{WagonTicketRegister}\") != @chit.id or @wagon.location != tolocation(@depot))\nforce @seller \"say This ticket does not match a wagon stored in this yard.\"\nreturn true\nend if");
		offerBody.AppendLine($"if (not(purchasevehicletransfer(@actor, @seller, @wagon, @actor.location, tocurrency({_currency.Id}), 50)))\nforce @seller \"say I cannot complete the fifty-bit redemption.\"\nreturn true\nend if\nsetregister @wagon \"{WagonTicketRegister}\" 0\nsetregister @chit \"{WagonDepotRegister}\" 0\nsetregister @chit \"{WagonIdRegister}\" 0\ndeleteitem(@chit)\nsendloc(@actor.location, \"$0 pays $1 fifty bits and redeems a wagon from the yard.\", @actor, @seller)\nreturn true");

		foreach (var source in sourceNpcs.Values)
		{
			var rent = Prog($"ArmageddonWorldfileWagonRent{source.Vnum}", rentBody.ToString(), ProgVariableTypes.Boolean,
				("actor", ProgVariableTypes.Character), ("npc", ProgVariableTypes.Perceivable), ("command", ProgVariableTypes.Text),
				("args", ProgVariableTypes.Text | ProgVariableTypes.Collection));
			InstallNpcInputHook($"Wagon Storage Rent {source.Vnum}", "rent", rent, [source.Vnum]);
			var offer = Prog($"ArmageddonWorldfileWagonOffer{source.Vnum}", offerBody.ToString(), ProgVariableTypes.Boolean,
				("actor", ProgVariableTypes.Character), ("npc", ProgVariableTypes.Perceivable), ("command", ProgVariableTypes.Text),
				("args", ProgVariableTypes.Text | ProgVariableTypes.Collection));
			InstallNpcInputHook($"Wagon Storage Offer {source.Vnum}", "offer", offer, [source.Vnum]);
		}
		_counts["wagon storage yards"] = yards.Count;
		_counts["wagon storage attendants"] = sourceNpcs.Count;
		Record("npc-service.wagon-storage", "NativeVehicleStorage", null, null, new
		{
			Sources = sourceNpcs.Keys.OrderBy(x => x), Yards = depots.Select(x => new { Npc = x.Npc.Vnum, FrontageVnum = x.Frontage.Vnum, Room = yards[$"{x.Npc.Vnum}:{x.Frontage.Vnum}"].UniqueName }),
			TicketPrototype = 1597, RedemptionBits = 50,
			Disposition = "Source-backed store and bearer-ticket redemption for retained attendants 3842, 20119, 50455 and 50605. Occupants, including offline occupants, block storage; successful storage uses native empty-vehicle relocation and unhitches animals, leaving them outside. Redemption requires the exact extant ticket and its tagged wagon in that ticket's yard, then performs native exact cash settlement and relocation. No daily fee, repair or unattached wagon_to_yard command is invented."
		});
		Flush();
	}
}

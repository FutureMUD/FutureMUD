#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private const string CustomClothingNpcRegister = "armw_customclothing_npc";
	private const string CustomClothingGarmentRegister = "armw_customclothing_garment";
	private const string CustomClothingTicketRegister = "armw_customclothing_ticket";
	private const string CustomClothingVnumRegister = "armw_customclothing_vnum";
	private const string CustomClothingFitRegister = "armw_customclothing_fit";
	private const string CustomClothingDueRegister = "armw_customclothing_due";
	private const string CustomClothingPriceRegister = "armw_customclothing_price";

	private sealed record ClothingChoice(string Name, int Weight);
	private static readonly ClothingChoice[] ClothingColours =
	[
		new("red", 5), new("blue", 4), new("green", 4), new("purple", 2), new("black", 3), new("white", 1)
	];
	private static readonly ClothingChoice[] ClothingMaterials =
	[
		new("silk", 20), new("cloth", 3), new("hide", 5), new("sandcloth", 4)
	];
	private static readonly ClothingChoice[] ClothingTypes =
	[
		new("trousers", 5), new("tunic", 6), new("cloak", 3), new("aba", 4), new("skirt", 2), new("vest", 4)
	];

	private void InstallCustomClothing()
	{
		const int npcVnum = 50429;
		const int roomVnum = 50419;
		const int ticketVnum = 1598;
		var npc = _source.Npcs.SingleOrDefault(x => x.Vnum == npcVnum);
		if (npc is null || !_npcs.ContainsKey(npcVnum) ||
			!npc.Programs.Contains("C buy custom_clothing") || !npc.Programs.Contains("C offer custom_clothing") ||
			!npc.Programs.Contains("C list custom_clothing")) return;
		if (!_source.Resets.Any(x => x.Opcode == "M" && x.ReferenceIssues.Length == 0 && x.Arguments.Length > 3 &&
			x.Arguments[1] == npcVnum && x.Arguments[3] == roomVnum) || !_rooms.TryGetValue(roomVnum, out var sourceRoom)) return;
		if (!_items.TryGetValue(ticketVnum, out var ticketPrototype))
			throw new InvalidOperationException("Custom clothing requires retained source ticket prototype 1598.");

		var catalogue = (from colour in ClothingColours.Select((x, i) => (x, i))
			from material in ClothingMaterials.Select((x, i) => (x, i))
			from type in ClothingTypes.Select((x, i) => (x, i))
			select (Vnum: 13700 + colour.i * 24 + material.i * 6 + type.i,
				Colour: colour.x, Material: material.x, Type: type.x)).ToArray();
		if (catalogue.Length != 144 || catalogue.Select(x => x.Vnum).Distinct().Count() != 144 ||
			catalogue[0].Vnum != 13700 || catalogue[^1].Vnum != 13843)
			throw new InvalidOperationException("Custom clothing must map exactly 144 outputs from 13700 through 13843.");
		var missing = catalogue.Where(x => !_items.ContainsKey(x.Vnum)).Select(x => x.Vnum).ToArray();
		if (missing.Length > 0)
			throw new InvalidOperationException($"Custom clothing requires all 144 retained source prototypes; missing {string.Join(", ", missing)}.");

		foreach (var (name, type) in new[]
		{
			(CustomClothingNpcRegister, ProgVariableTypes.Number),
			(CustomClothingGarmentRegister, ProgVariableTypes.Number),
			(CustomClothingTicketRegister, ProgVariableTypes.Number),
			(CustomClothingVnumRegister, ProgVariableTypes.Number),
			(CustomClothingFitRegister, ProgVariableTypes.Number),
			(CustomClothingDueRegister, ProgVariableTypes.DateTime),
			(CustomClothingPriceRegister, ProgVariableTypes.Number),
		}) DefineCustomClothingRegister(name, type);

		var custody = CreateAuxiliaryRoom(_source.Rooms.Single(x => x.Vnum == roomVnum).Zone, "custom-clothing-custody.Vnum50429.Room50419",
			"Ariada's Workroom", "A private room where Ariada keeps prepaid custom garments until collection.");
		var choices = new StringBuilder();
		foreach (var item in catalogue)
		{
			var basePrice = Math.Max(1, item.Colour.Weight * item.Material.Weight * item.Type.Weight);
			choices.AppendLine($"if (@colour == \"{item.Colour.Name}\" and @material == \"{item.Material.Name}\" and @type == \"{item.Type.Name}\")");
			choices.AppendLine($"outputVnum = {item.Vnum}\nbasePrice = {basePrice}\nmaterialPrice = {item.Material.Weight}\nend if");
		}
		var loads = new StringBuilder("var garment as item\n");
		foreach (var item in catalogue)
			loads.AppendLine($"if (@outputVnum == {item.Vnum})\ngarment = loaditem({_items[item.Vnum].Id})\nend if");
		var prototypes = new StringBuilder("var expectedProto = 0\n");
		foreach (var item in catalogue)
			prototypes.AppendLine($"if (@outputVnum == {item.Vnum})\nexpectedProto = {_items[item.Vnum].Id}\nend if");

		var familyIds = _npcFamilyIds.GetValueOrDefault(npcVnum, [_npcs[npcVnum].Id]);
		var templateGuard = string.Join(" and ", familyIds.Select(x => $"@seller.npctemplateid != {x}"));
		var admission = $"var seller = tocharacter(@npc)\nif (isnull(@seller) or ({templateGuard}) or @seller == @actor or not(@seller.isawake) or @seller.incombat or not(@actor.isawake) or @seller.location != @actor.location or not(cansee(@actor, @seller)))\nreturn false\nend if\n";
		var saveReceipt = Prog("ArmageddonWorldfileCustomClothingSaveReceipt",
			$"setregister @garment \"{CustomClothingNpcRegister}\" {npcVnum}\nsetregister @garment \"{CustomClothingTicketRegister}\" @ticket.id\n" +
			$"setregister @ticket \"{CustomClothingNpcRegister}\" {npcVnum}\nsetregister @ticket \"{CustomClothingGarmentRegister}\" @garment.id\n" +
			$"setregister @ticket \"{CustomClothingTicketRegister}\" @ticket.id\nsetregister @ticket \"{CustomClothingVnumRegister}\" @outputVnum\n" +
			$"setregister @ticket \"{CustomClothingFitRegister}\" @fit\nsetregister @ticket \"{CustomClothingDueRegister}\" @due\n" +
			$"setregister @ticket \"{CustomClothingPriceRegister}\" @price\nreturn getregister(@garment, \"{CustomClothingTicketRegister}\") == @ticket.id and " +
			$"getregister(@ticket, \"{CustomClothingGarmentRegister}\") == @garment.id and getregister(@ticket, \"{CustomClothingVnumRegister}\") == @outputVnum and " +
			$"getregister(@ticket, \"{CustomClothingFitRegister}\") == @fit and getregister(@ticket, \"{CustomClothingDueRegister}\") == @due and " +
			$"getregister(@ticket, \"{CustomClothingPriceRegister}\") == @price",
			ProgVariableTypes.Boolean, ("garment", ProgVariableTypes.Item), ("ticket", ProgVariableTypes.Item),
			("outputVnum", ProgVariableTypes.Number), ("fit", ProgVariableTypes.Number), ("due", ProgVariableTypes.DateTime),
			("price", ProgVariableTypes.Number));
		var listBody = admission + "if (@args.Count(word, true) != 2 or lowercase(getindex(@args, 0)) != \"at\" or targetcharacter(@actor, getindex(@args, 1)) != @seller)\nreturn false\nend if\n" +
			"force @seller \"say I make trousers, tunics, cloaks, abas, skirts and vests in red, blue, green, purple, black or white silk, cloth, hide or sandcloth.\"\nsend(@actor, \"Colours: red, blue, green, purple, black, white\\nMaterials: silk, cloth, hide, sandcloth\\nTypes: trousers, tunic, cloak, aba, skirt, vest\")\nreturn true";

		var buyBody = new StringBuilder(admission);
		buyBody.AppendLine("if (@args.Count(word, true) != 5 or lowercase(getindex(@args, 3)) != \"at\" or targetcharacter(@actor, getindex(@args, 4)) != @seller)\nreturn false\nend if");
		buyBody.AppendLine("var colour = lowercase(getindex(@args, 0))\nvar material = lowercase(getindex(@args, 1))\nvar type = lowercase(getindex(@args, 2))\nvar outputVnum = 0\nvar basePrice = 0\nvar materialPrice = 0");
		buyBody.Append(choices);
		buyBody.AppendLine("if (@outputVnum == 0)\nforce @seller \"say I do not make that combination.\"\nreturn true\nend if");
		buyBody.AppendLine("var fit = @ArmageddonWorldfileScalarWearableSize(@actor)\nvar price = max(1, @basePrice) + @fit * @materialPrice\nvar hours = floor(@fit / 4) + random(0, 2)");
		buyBody.Append(loads);
		buyBody.AppendLine("if (isnull(@garment) or not(setwearablefitscalar(@garment, @fit)))\nif (not(isnull(@garment)))\ndeleteitem(@garment)\nend if\nforce @seller \"say I cannot prepare that garment right now.\"\nreturn true\nend if");
		buyBody.AppendLine($"var chit = loaditem({ticketPrototype.Id})\nif (isnull(@chit))\ndeleteitem(@garment)\nforce @seller \"say I cannot prepare a work ticket right now.\"\nreturn true\nend if\nvar custodyRoom = tolocation({custody.Id})\nif (not(put(@garment, @custodyRoom)))\ndeleteitem(@garment)\ndeleteitem(@chit)\nforce @seller \"say I cannot secure the garment for you.\"\nreturn true\nend if\nif (not(put(@chit, @custodyRoom)))\ndeleteitem(@garment)\ndeleteitem(@chit)\nforce @seller \"say I cannot secure your work ticket.\"\nreturn true\nend if");
		buyBody.AppendLine($"var due = now() + timespanfromseconds(@hours * 600)\nif (not(@{saveReceipt.FunctionName}(@garment, @chit, @outputVnum, @fit, @due, @price)))\ndeleteitem(@garment)\ndeleteitem(@chit)\nforce @seller \"say I cannot prepare your order record.\"\nreturn true\nend if\n" +
			$"if (not(purchaseitemdelivery(@actor, @seller, @chit, @custodyRoom, tocurrency({_currency.Id}), @price)))\n" +
			$"setregister @garment \"{CustomClothingTicketRegister}\" 0\ndeleteitem(@garment)\ndeleteitem(@chit)\nforce @seller \"say I cannot take payment and issue the ticket right now.\"\nreturn true\nend if\n" +
			"if (not(silentget(@actor, @chit)))\nsend(@actor, \"Your paid order is ready in the workroom, but the ticket is beside you. Pick it up to collect your garment.\")\nelse\nsend(@actor, \"You pay \" + @price + \" bits for your custom garment. It will be ready in about \" + @hours + \" hours.\")\nend if\nsendloc(@actor.location, \"$0 pays $1 for a custom garment.\", @actor, @seller)\nreturn true");

		var collectBody = new StringBuilder(admission);
		collectBody.AppendLine("if (@args.Count(word, true) != 3 or lowercase(getindex(@args, 1)) != \"at\" or targetcharacter(@actor, getindex(@args, 2)) != @seller)\nreturn false\nend if");
		collectBody.AppendLine($"var chit = targetitem(@actor, @actor.inventory, getindex(@args, 0))\nif (isnull(@chit) or @chit.proto != {ticketPrototype.Id} or getregister(@chit, \"{CustomClothingNpcRegister}\") != {npcVnum})\nforce @seller \"say That is not one of my custom clothing tickets.\"\nreturn true\nend if");
		collectBody.AppendLine($"var garmentId = getregister(@chit, \"{CustomClothingGarmentRegister}\")\nvar garment = getexistingitem(@garmentId)\nvar ticketLink = getregister(@chit, \"{CustomClothingTicketRegister}\")\nvar outputVnum = getregister(@chit, \"{CustomClothingVnumRegister}\")\nvar fit = getregister(@chit, \"{CustomClothingFitRegister}\")\nvar due = getregister(@chit, \"{CustomClothingDueRegister}\")\nif (@garmentId == 0 or isnull(@garment) or @ticketLink != @chit.id or getregister(@garment, \"{CustomClothingNpcRegister}\") != {npcVnum} or getregister(@garment, \"{CustomClothingTicketRegister}\") != @chit.id or @garment.location != tolocation({custody.Id}))\nforce @seller \"say That ticket does not match a garment in my workroom.\"\nreturn true\nend if\n" + prototypes);
		collectBody.AppendLine("if (@expectedProto == 0 or @garment.proto != @expectedProto or wearablefitscalar(@garment) != @fit)\nforce @seller \"say That ticket does not identify its original fitted garment.\"\nreturn true\nend if\nif (@due > now())\nforce @seller \"say That garment is not ready yet.\"\nreturn true\nend if\nvar fitText = \"\" + @fit");
		collectBody.AppendLine($"if (not(purchaseitemdelivery(@actor, @seller, @garment, tolocation({custody.Id}), tocurrency({_currency.Id}), 0)))\nforce @seller \"say I cannot complete delivery right now.\"\nreturn true\nend if\n" +
			$"setregister @garment \"{CustomClothingNpcRegister}\" 0\nsetregister @garment \"{CustomClothingTicketRegister}\" 0\ndeleteitem(@chit)\n" +
			"if (silentget(@actor, @garment))\nsend(@actor, \"You collect your custom garment.\")\nelse\nsend(@actor, \"Your garment is ready and has been delivered beside you.\")\nend if\nsendloc(@actor.location, \"$0 collects a custom garment from $1.\", @actor, @seller)\nreturn true");

		var list = NpcServiceProg("ArmageddonWorldfileCustomClothingList", listBody);
		var buy = NpcServiceProg("ArmageddonWorldfileCustomClothingBuy", buyBody.ToString());
		var quoteBody = new StringBuilder(admission);
		quoteBody.AppendLine("if (@args.Count(word, true) != 5 or lowercase(getindex(@args, 3)) != \"at\" or targetcharacter(@actor, getindex(@args, 4)) != @seller)\nreturn false\nend if");
		quoteBody.AppendLine("var colour = lowercase(getindex(@args, 0))\nvar material = lowercase(getindex(@args, 1))\nvar type = lowercase(getindex(@args, 2))\nvar outputVnum = 0\nvar basePrice = 0\nvar materialPrice = 0");
		quoteBody.Append(choices);
		quoteBody.AppendLine("if (@outputVnum == 0)\nforce @seller \"say I do not make that combination.\"\nreturn true\nend if\nvar fit = @ArmageddonWorldfileScalarWearableSize(@actor)\nvar price = max(1, @basePrice) + @fit * @materialPrice\nforce @seller (\"say That \" + @colour + \" \" + @material + \" \" + @type + \" will cost \" + @price + \" coins.\")\nreturn true");
		var quote = NpcServiceProg("ArmageddonWorldfileCustomClothingQuote", quoteBody.ToString());
		var collect = NpcServiceProg("ArmageddonWorldfileCustomClothingCollection", collectBody.ToString());
		InstallNpcInputHook("Custom Clothing List", "list", list, [npcVnum]);
		InstallNpcInputHook("Custom Clothing Purchase", "buy", buy, [npcVnum]);
		InstallNpcInputHook("Custom Clothing Quote", "offer", quote, [npcVnum]);
		InstallNpcInputHook("Custom Clothing Collection", "offer", collect, [npcVnum]);
		Record("npc-service.custom-clothing", "NativeCustomClothing", null, null, new
		{
			SourceNpc = npcVnum, ResetRoom = roomVnum, TicketPrototype = ticketVnum,
			OutputVnums = catalogue.Select(x => x.Vnum).ToArray(), CustodyRoom = custody.Id,
				Colours = ClothingColours, Materials = ClothingMaterials, Types = ClothingTypes,
			Disposition = "Retained source custom clothing catalogue, weighted price and 600-second-hour delay. Purchases prepare the exact imported garment at the buyer's scalar fit in private custody before atomic payment and reciprocal ticket delivery. Collection validates the original garment/ticket links and due time, then delivers for no second charge."
		});
		_counts["custom clothing services"] = 1;
		Flush();
	}

	private void DefineCustomClothingRegister(string name, ProgVariableTypes type)
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
			OwnerTypeDefinition = owner, Property = name,
			DefaultValue = type == ProgVariableTypes.DateTime ? "<var>0001-01-01T00:00:00.0000000Z</var>" : "<var>0</var>"
		});
	}
}

#nullable enable

using System;
using System.Collections.Generic;
using CultureInfo = System.Globalization.CultureInfo;
using System.Linq;
using System.Text;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private sealed record MountSale(int Vnum, int Price);
	private static readonly IReadOnlyDictionary<int, MountSale[]> MountSaleCatalogues = new Dictionary<int, MountSale[]>
	{
		[3004] = [new(3001, 437), new(3002, 472), new(3003, 450), new(20062, 517)],
		[20054] = [new(50451, 375), new(20062, 450), new(48009, 750), new(20056, 400), new(3002, 650)],
		[37735] = [new(50451, 500), new(20062, 650), new(37780, 875)],
		[50413] = [new(3001, 350), new(20062, 425), new(48002, 500), new(20056, 520), new(3002, 520), new(50451, 520)]
	};

	private bool HasRetainedNpcReset(int vnum) => _source.Resets.Any(x => x.Opcode == "M" &&
		x.ReferenceIssues.Length == 0 && x.Arguments.Length > 3 && x.Arguments[1] == vnum);

	private const string VendorAdmission = """
		var seller = tocharacter(@npc)
		if (isnull(@seller) or @seller == @actor or not(@seller.isawake) or @seller.incombat or not(@actor.isawake))
			return false
		end if
		if (@seller.location != @actor.location or not(cansee(@actor, @seller)))
			return false
		end if
		""";

	private void InstallVendorServices()
	{
		InstallWaterSellers();
		InstallMountSellers();
	}

	private void InstallWaterSellers()
	{
		var specials = new[] { "az_waterseller", "mal_krian_waterseller" };
		var sources = _source.Npcs.Where(x => _npcs.ContainsKey(x.Vnum) && HasRetainedNpcReset(x.Vnum) &&
			specials.Any(s => x.Programs.Contains("C fill " + s))).ToArray();
		if (sources.Length == 0) return;
		var water = _db.Liquids.Single(x => x.Name == "water");
		var unit = (decimal)_db.UnitsOfMeasure.Single(x => x.Id == _settings.NativeBindings[ArmageddonNativeBinding.Litre]).BaseMultiplier * 0.05m;
		var volume = unit.ToString(CultureInfo.InvariantCulture);
		foreach (var command in new[] { "fill", "list", "offer", "barter" })
		{
			var code = new StringBuilder(VendorAdmission).AppendLine();
			if (command == "fill")
			{
				code.AppendLine($$"""
					if (@args.Count(word, true) == 0)
						return false
					end if
					if (@args.Count(word, true) > 1 and lowercase(getindex(@args, 1)) != "well")
						return false
					end if
					var vessel = targetitem(@actor, @actor.inventory, getindex(@args, 0))
					if (isnull(@vessel))
						return false
					end if
					var price = liquidrefillcost(@vessel, {{water.Id}}, {{volume}}, 3)
					if (@price < 0)
						force @seller "say Open your container first, and pour out any other liquid. I sell water only."
						return true
					end if
					if (@price == 0)
						send(@actor, "$0 is already full.", @vessel)
						return true
					end if
					if (not(purchaseliquidrefill(@actor, @seller, @vessel, tocurrency({{_currency.Id}}), {{water.Id}}, {{volume}}, 3)))
						force @seller "say I cannot complete that refill. Keep the container ready and make sure you have enough coins."
						return true
					end if
					send(@actor, "You pay " + @price + " bits to $0 and fill $1 with water.", @seller, @vessel)
					sendloc(@actor.location, "$0 pays $1 to refill $2 with water.", @actor, @seller, @vessel)
					return true
					""");
			}
			else
			{
				code.AppendLine("if (@args.Count(word, true) > 0 and targetcharacter(@actor, getindex(@args, 0)) != @seller)\nreturn false\nend if");
				code.AppendLine(command == "list"
					? "force @seller \"say I sell water for three bits per measure. Fill your container at the well.\""
					: "force @seller \"say Just fill up at the well, traveller. Water is three bits per measure.\"");
				code.AppendLine("return true");
			}
			var prog = Prog("ArmageddonWorldfileWaterSeller" + command, code.ToString(), ProgVariableTypes.Boolean,
				("actor", ProgVariableTypes.Character), ("npc", ProgVariableTypes.Perceivable), ("command", ProgVariableTypes.Text),
				("args", ProgVariableTypes.Text | ProgVariableTypes.Collection));
			var attached = sources.Where(x => specials.Any(s => x.Programs.Contains($"C {command} {s}"))).Select(x => x.Vnum).ToArray();
			if (attached.Length > 0) InstallNpcInputHook("Water Refills " + command, command, prog, attached);
		}
		Record("npc-service.water-refills", "NativeLiquidRefill", null, null, new { Sources = sources.Select(x => x.Vnum),
			Liquid = water.Id, UnitVolume = unit, BitsPerUnit = 3, Disposition = "Full refill of an open directly carried native container, empty or already containing only matching water. Existing matching contents are preserved. Three bits per source 50 mL for quotation, affordability and debit; prorate missing volume and round up to a whole bit. Source wells have no finite stock. Unplaced Silla remains inactive." });
		_counts["water refill sellers"] = sources.Length;
	}

	private void InstallMountSellers()
	{
		foreach (var source in _source.Npcs.Where(x => _npcs.ContainsKey(x.Vnum) && HasRetainedNpcReset(x.Vnum) &&
			x.Programs.Contains("C buy mount_seller")))
		{
			if (!MountSaleCatalogues.TryGetValue(source.Vnum, out var catalogue))
			{
				Record("npc-service.mount-sales.inert." + source.Vnum, "SourceInertService", _npcs[source.Vnum].Id, null,
					new { Source = source.Vnum, Disposition = "Attached mount_seller has no source catalogue case. Keep NPC without inventing stock or sales." });
				continue;
			}
			if (catalogue.Any(x => !_npcs.ContainsKey(x.Vnum))) throw new InvalidOperationException($"Source mount seller {source.Vnum} needs an absent animal definition; review its catalogue before installation.");
			foreach (var command in new[] { "list", "buy", "offer" }.Where(x => source.Programs.Contains($"C {x} mount_seller")))
			{
				var code = new StringBuilder(VendorAdmission).AppendLine();
				if (command is "list" or "offer")
				{
					code.AppendLine("if (@args.Count(word, true) > 0 and targetcharacter(@actor, getindex(@args, 0)) != @seller)\nreturn false\nend if");
					if (command == "list")
					{
						code.AppendLine("send(@actor, \"$0 offers the following animals:\", @seller)");
						foreach (var (sale, index) in catalogue.Select((x, i) => (x, i)))
							code.AppendLine($"send(@actor, {ProgText($"{index + 1}) {_source.Npcs.Single(x => x.Vnum == sale.Vnum).ShortDescription} — {sale.Price} bits")})");
					}
					code.AppendLine("force @seller \"say Buy the animal by its number or keyword.\"\nreturn true");
				}
				else
				{
					code.AppendLine("if (@args.Count(word, true) == 0)\nreturn false\nend if\nvar requested = lowercase(getindex(@args, 0))\nvar prototype = 0\nvar price = 0");
					foreach (var (sale, index) in catalogue.Select((x, i) => (x, i)))
					{
						var keywords = _source.Npcs.Single(x => x.Vnum == sale.Vnum).Name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
						var matches = new[] { (index + 1).ToString(CultureInfo.InvariantCulture), "vnum" + sale.Vnum }.Concat(keywords).Distinct(StringComparer.OrdinalIgnoreCase)
							.Select(x => "@requested == " + ProgText(x.ToLowerInvariant()));
						code.AppendLine($"if (@prototype == 0 and ({string.Join(" or ", matches)}))\nprototype = {_npcs[sale.Vnum].Id}\nprice = {sale.Price}\nend if");
					}
					code.AppendLine($$"""
						if (@prototype == 0)
							return false
						end if
						var animal = purchasenpc(@actor, @seller, @prototype, tocurrency({{_currency.Id}}), @price)
						if (isnull(@animal))
							force @seller "say I cannot complete that purchase. Make sure you have enough coins and can receive the animal here."
							return true
						end if
						sendloc(@actor.location, "$0 pays $1 for $2.", @actor, @seller, @animal)
						if (canmount(@actor, @animal))
							mount(@actor, @animal)
						end if
						return true
						""");
				}
				var prog = Prog($"ArmageddonWorldfileMountSeller{source.Vnum}{command}", code.ToString(), ProgVariableTypes.Boolean,
					("actor", ProgVariableTypes.Character), ("npc", ProgVariableTypes.Perceivable), ("command", ProgVariableTypes.Text),
					("args", ProgVariableTypes.Text | ProgVariableTypes.Collection));
				InstallNpcInputHook($"Mount Sales {source.Vnum} {command}", command, prog, [source.Vnum]);
			}
			Record("npc-service.mount-sales." + source.Vnum, "NativeNpcPurchase", _npcs[source.Vnum].Id, null,
				new { Seller = source.Vnum, Catalogue = catalogue, Disposition = "Only explicitly assigned source stock entries; the uninitialised seventh slot at seller 50413 is omitted. Native cash settlement and one delivered animal, no new ownership/taming restrictions, no replenishing spawner. Native mounting is attempted only when permitted; otherwise the animal remains beside its buyer. Source eligibility restrictions concern other inactive vendor IDs and are not invented here." });
		}
		_counts["mount sale catalogues"] = MountSaleCatalogues.Keys.Count(x => _npcs.ContainsKey(x) && HasRetainedNpcReset(x));
	}
}

#nullable enable

using System;
using System.Linq;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private void InstallMantisVendors()
	{
		var vendors = _source.Npcs.Where(x => HasSpecial(x, "mantis_vendor")).ToArray();
		if (vendors.Length == 0) return;
		var mantis = _db.Races.AsEnumerable().Where(x => x.Name.Split(" (", StringSplitOptions.None)[0].Equals("Mantis", StringComparison.OrdinalIgnoreCase)).Select(x => x.Id).ToArray();
		if (mantis.Length == 0) throw new InvalidOperationException("Mantis vendors require an installed Mantis race.");
		var race = string.Join(" or ", mantis.Select(id => $"@actor.race.id == {id}"));
		var prog = NpcServiceProg("ArmageddonWorldfileMantisVendor", $"""
			var vendor = tocharacter(@npc)
			if (@args.Count(word, true) < 1 or not({race}))
				return false
			end if
			if (isclanmember(@actor, toclan({_clans[7].Id})) and not(isclanmember(@actor, toclan({_clans[37].Id}))))
				return false
			end if
			if (targetcharacter(@actor, getindex(@args, 0)) != @vendor or not(@vendor.isawake) or @vendor.incombat)
				return false
			end if
			if (@input == "list")
				foreach (thing in @vendor.helditems)
					if (cansee(@actor, @thing))
						sendfixed(@actor, howseen(@thing, @actor, false, false))
					end if
				end foreach
				return true
			end if
			if (@args.Count(word, true) < 2)
				sendfixed(@actor, "Ask for what?")
				return true
			end if
			var query = getindex(@args, 1)
			var thing = targethelditem(@vendor, @actor, @query)
			if (isnull(@thing))
				sendfixed(@actor, "The vendor does not have that.")
				return true
			end if
			if (not(isnull(targetitem(@actor, @actor.location.items, @query))) or not(isnull(targetitem(@actor, @actor.inventory, @query))))
				sendfixed(@actor, "You already have that, or one is lying here.")
				return true
			end if
			giveitem(@vendor, @thing, @actor)
			return true
			""");
		foreach (var command in new[] { "list", "ask" })
		{
			var sources = vendors.Where(x => x.Programs.Contains($"C {command} mantis_vendor")).Select(x => x.Vnum).ToArray();
			if (sources.Length > 0) InstallNpcInputHook("Mantis Vendor " + command, command, prog, sources);
		}
		Record("npc-special.mantis-vendor", nameof(FutureProg), prog.Id, null,
			new { Sources = vendors.Select(x => x.Vnum), Disposition = "Free actual carried goods. Mantis race and source clan-7-unless-clan-37 exclusion; addressed awake/noncombat vendor, keyword-based inventory/ground duplicate refusal and ordinary native item transfer checks. No shop, money or replacement items." });
		var pulseSources = vendors.Where(x => x.Programs.Contains("C pulse mantis_vendor")).Select(x => x.Vnum).ToArray();
		if (pulseSources.Length == 0) return;
		var meatIds = _source.Items.Where(x => x.ItemType == 19 && x.Values[1] == 0 && _items.ContainsKey(x.Vnum))
			.SelectMany(x => new[] { _items[x.Vnum].Id }.Concat(_resetItemVariants.Where(y => y.Key.StartsWith(x.Vnum + ":", StringComparison.Ordinal)).Select(y => y.Value.Id))).Distinct().ToArray();
		var meat = meatIds.Length == 0 ? "false" : $"splittext(\"{string.Join("|", meatIds)}\", \"|\").Any(id, tonumber(@id) == @first.proto)";
		var pulse = Prog("ArmageddonWorldfileMantisPit", $"""
			var pit = tolocation({_rooms[13601].Id})
			foreach (thing in @pit.items)
				if (not(@thing.isfood) and not(@thing.iscorpse))
					deleteitem(@thing)
				end if
			end foreach
			if (random(1, 21) == 1 and @pit.items.Count(thing, true) > 0)
				var first = getindex(@pit.items, 0)
				if (@first.iscorpse or ({meat}))
					deleteitem(@first)
				end if
			end if
			return false
			""", ProgVariableTypes.Boolean, ("npc", ProgVariableTypes.Character));
		InstallNpcTickHook("Mantis Pit", pulse, pulseSources);
		Record("npc-special.mantis-pit", nameof(FutureProg), pulse.Id, null,
			new { Sources = pulseSources, Pit = 13601, Disposition = "Preserves nonfood/noncorpse cleanup and 1/21 head-item-only feeding. Source food classification binds meat prototypes and reset variants; un-emitted source eating prose remains silent." });
	}
}

#nullable enable

using System;
using System.Linq;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private void InstallTraderSpecials()
	{
		foreach (var name in new[] { "halfling_trader", "sewer_trader" })
		{
			var sources = _source.Npcs.Where(x => x.Programs.Contains("C trade " + name)).Select(x => x.Vnum).ToArray();
			if (sources.Length == 0) continue;
			var halfling = name == "halfling_trader";
			var eligibility = "";
			var donation = "";
			if (halfling)
			{
				if (!_clans.TryGetValue(9, out var clan)) throw new InvalidOperationException("Halfling barter requires source clan 9.");
				var races = _db.Races.AsEnumerable().Where(x => x.Name.Split(" (", StringSplitOptions.None)[0].Equals("Halfling", StringComparison.OrdinalIgnoreCase)).Select(x => x.Id).ToArray();
				var racePredicate = races.Length == 0 ? "false" : string.Join(" or ", races.Select(id => $"@actor.race.id == {id}"));
				eligibility = $"if (not(isclanmember(@actor, toclan({clan.Id}))) or not({racePredicate}))\nreturn false\nend if\n";
				var ids = _source.Items.Where(x => x.Vnum / 1000 == 39 && _items.ContainsKey(x.Vnum))
					.SelectMany(x => new[] { _items[x.Vnum].Id }.Concat(_resetItemVariants.Where(y => y.Key.StartsWith(x.Vnum + ":", StringComparison.Ordinal)).Select(y => y.Value.Id))).Distinct().ToArray();
				var gift = Prog("ArmageddonWorldfileForestGift", $"var ids = splittext(\"{string.Join("|", ids)}\", \"|\")\nreturn @ids.Any(id, tonumber(@id) == @thing.proto)",
					ProgVariableTypes.Boolean, ("thing", ProgVariableTypes.Item));
				donation = $"if (@{gift.FunctionName}(@offered))\nsetregister @actor \"{ArmageddonGatheringRegisters.Ecology}\" min(100, getregister(@actor, \"{ArmageddonGatheringRegisters.Ecology}\") + 1)\nend if\n";
			}
			var unavailable = halfling ? "The Forest has not provided any " : "I haven't found any ";
			var prog = NpcServiceProg("ArmageddonWorldfileTrader" + name, $"""
var trader = tocharacter(@npc)
if (not(@actor.isawake) or not(@trader.isawake) or @trader.incombat)
	return false
end if
{eligibility}
if (@input == "list")
	force @trader "say {(halfling ? "The Forest has provided these things." : "I've done found these things...")}"
	if (@trader.helditems.Count(thing, true) == 0)
		sendfixed(@actor, "Nothing.")
	else
		foreach (thing in @trader.helditems)
			sendfixed(@actor, howseen(@thing, @actor, false, false))
		end foreach
	end if
	return true
end if
if (@input == "give")
	if (@args.Count(word, true) < 2)
		return false
	end if
	if (targetcharacter(@actor, getindex(@args, 1)) != @trader)
		return false
	end if
else
	if (@args.Count(word, true) < 2)
		force @trader "say Trade what for what?"
		return true
	end if
end if
var offered = targethelditem(@actor, @actor, getindex(@args, 0))
if (isnull(@offered))
	force @trader "say You cannot give what you do not have."
	return true
end if
if (@input == "give")
	if (giveitem(@actor, @offered, @trader))
		{donation}force @trader "say {(halfling ? "Life to you, and peace." : "Catch you later.")}"
	end if
	return true
end if
var requested = targethelditem(@trader, @actor, getindex(@args, 1))
if (isnull(@requested))
	force @trader ("say {unavailable}" + getindex(@args, 1) + ".")
	return true
end if
if (exchangeitems(@actor, @offered, @trader, @requested))
	sendloc(@actor.location, "$0 nod|nods and exchange|exchanges $1's $2 for $3.", @trader, @actor, @offered, @requested)
else
	force @trader "say We cannot exchange those things just now."
end if
return true
""");
			foreach (var command in new[] { "list", "trade", "give" })
			{
				var commandSources = sources.Where(vnum => _source.Npcs.Single(x => x.Vnum == vnum).Programs.Contains("C " + command + " " + name)).ToArray();
				if (commandSources.Length != 0) InstallNpcInputHook("Barter " + name + " " + command, command, prog, commandSources);
			}
			Record("npc-barter." + name, nameof(FutureProg), prog.Id, null, new
			{ Sources = sources, Disposition = "Conditional command progs implement list, one-for-one exchange of actual whole carried objects, and gifts. Native held-item keyword/visibility matching and Give eligibility apply; exchange commits both custody changes before callbacks and never merges or creates replacements. Halfling/clan 9 restriction and zone-39 donation ecology +1 capped at 100 use the installed ecology register; sewer trader is unrestricted. These are separate from permanent monetary shops. The stock list uses one native description per line." });
		}
		Flush();
	}
}

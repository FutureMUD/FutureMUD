#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Events;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private void InstallNpcReactions()
	{
		InstallHoneyProduction();
		InstallGamblerFlavour();
		InstallHornblowers();
		InstallFirePerformance();
		InstallPtarkenInduction();
		InstallSoldierStandDown();
	}

	private void InstallNpcTickHook(string name, FutureProg prog, int[] sources)
	{
		InstallNpcEventHook(name, EventType.TenSecondTick, prog, sources);
		_counts["NPC periodic hooks"] = _counts.GetValueOrDefault("NPC periodic hooks") + 1;
	}

	private void InstallNpcEventHook(string name, EventType eventType, FutureProg prog, int[] sources)
	{
		var hook = new Hooks { Id = Next<Hooks>(), Name = "Worldfile " + name, Type = "FutureProgHook", Category = "Worldfile",
			TargetEventType = (int)eventType, Definition = new XElement("Definition", new XElement("FutureProg", prog.Id)).ToString() };
		_db.Hooks.Add(hook);
		foreach (var vnum in sources)
		foreach (var id in _npcFamilyIds.GetValueOrDefault(vnum, [_npcs[vnum].Id]))
		{
			var template = _db.NpcTemplates.Local.Single(x => x.Id == id); var definition = XElement.Parse(template.Definition);
			definition.Element("TemplateLoadAdditions")!.Element("Hooks")!.Add(new XElement("Hook", new XAttribute("id", hook.Id)));
			template.Definition = definition.ToString();
		}
		_counts["NPC event hooks"] = _counts.GetValueOrDefault("NPC event hooks") + 1;
	}

	private void InstallHoneyProduction()
	{
		var sources = _source.Npcs.Where(x => x.Programs.Contains("C pulse honey_kank")).Select(x => x.Vnum).ToArray();
		if (sources.Length == 0) return;
		if (!_items.TryGetValue(70223, out var honey)) throw new InvalidOperationException("Honey producers require source item 70223.");
		var prog = Prog("ArmageddonWorldfileHoneyProduction", $"""
if (not(@npc.isawake) or @npc.incombat)
	return false
end if
if (random(1, 150) != 1)
	return false
end if
if (@npc.helditems.Any(thing, cansee(@npc, @thing) and splittext(lowercase(@thing.name), " ").Any(word, startswith(@word, "honey"))))
	return false
end if
var honey = loaditem({honey.Id})
if (not(silentget(@npc, @honey)))
	takedelete(@honey)
	return false
end if
sendloc(@npc.location, "$0's beady eyes glaze over with effort.", @npc)
return true
""", ProgVariableTypes.Boolean, ("npc", ProgVariableTypes.Character));
		InstallNpcTickHook("Honey Production", prog, sources);
		Record("npc-special.honey-kank", nameof(FutureProg), prog.Id, null, new
		{ Sources = sources, Item = 70223, Disposition = "Source awake/noncombat one-percent pulse production with visible carried honey-keyword suppression. Native ten-second tick at 1/150 preserves the source mean 25-minute eligible wait (source pulse mean 15 seconds). Failed native inventory delivery deletes only the newly created item. No plain reset duplicates honey; legacy act_wait has no separately imported timer." });
	}

	private void InstallSoldierStandDown()
	{
		var sources = _source.Npcs.Where(x => x.Programs.Contains("C order allanak_soldier_stop")).Select(x => x.Vnum).ToArray();
		if (sources.Length == 0) return;
		if (!_clans.TryGetValue(12, out var clan)) throw new InvalidOperationException("Soldier stand-down requires source militia clan 12.");
		var prog = NpcServiceProg("ArmageddonWorldfileSoldierStandDown", $"""
var soldier = tocharacter(@npc)
if (not(@actor.isawake))
	return false
end if
if (@args.Count(word, true) < 2)
	return false
end if
if (getindex(@args, 1) != "stop" or targetcharacter(@actor, getindex(@args, 0)) != @soldier)
	return false
end if
if (not(isclanmember(@actor, toclan({clan.Id}))))
	force @soldier "say Only Servants of the Dragon may give me orders."
	return true
end if
if (not(@soldier.incombat) or not(ceasecombat(@soldier)))
	force @soldier "say I can't follow those orders."
	return true
end if
sendloc(@actor.location, "$0 give|gives $1 an order.", @actor, @soldier)
sendloc(@actor.location, "$0 step|steps back from the battle to $1's side.", @soldier, @actor)
return true
""");
		InstallNpcInputHook("Militia Stand Down", "order", prog, sources);
		Record("npc-special.allanak-soldier-stop", nameof(FutureProg), prog.Id, null, new { Sources = sources,
			Disposition = "Source visible targeted order <soldier> stop, clan 12 eligibility and ordinary refusal echoes. Native selective cessation removes the soldier and clears incoming pair targeting without terminating unrelated combatants' fight; native retargeting and post-combat delay remain." });
	}

	private void InstallGamblerFlavour()
	{
		foreach (var name in new[] { "jindo_dice", "gypsy_game_two", "gypsy_game_three" })
		{
			var sources = _source.Npcs.Where(x => x.Programs.Contains("C pulse " + name)).Select(x => x.Vnum).ToArray();
			if (sources.Length == 0) continue;
			var first = name switch { "jindo_dice" => "$0 rattle|rattles a pair of dice in $0's hand absent-mindedly.",
				"gypsy_game_two" => "$0 rattle|rattles some pebbles in $0's hand absent-mindedly.", _ => "$0 rattle|rattles a few stones in $0's hand, eying the crowd." };
			var text = new StringBuilder("if (not(@npc.isawake) or @npc.incombat)\nreturn false\nend if\nvar roll = random(1, 150)\n");
			var messages = new[] { first, "$0 hand|hands over some winnings to a gambler.", "", "$0 collect|collects some coins from a gambler.",
				"$0 call|calls to a passing gambler, 'Try your luck here!'", name == "gypsy_game_three" ? "$0 wink|winks knowingly at a passing gambler." : first };
			for (var i = 0; i < messages.Length; i++)
				text.AppendLine($"if (@roll == {i + 1})\n{(i == 2 ? "force @npc \"say We have a winner!\"" : "sendloc(@npc.location, " + ProgText(messages[i]) + ", @npc)")}\nreturn true\nend if");
			text.AppendLine("return false");
			var prog = Prog("ArmageddonWorldfileGamblerFlavour" + name, text.ToString(), ProgVariableTypes.Boolean, ("npc", ProgVariableTypes.Character));
			InstallNpcTickHook("Gambler Flavour " + name, prog, sources);
			Record("npc-pulse." + name, nameof(FutureProg), prog.Id, null, new { Sources = sources,
				Disposition = "Only attached source pulse branches run. Native ten-second tick uses six of 150 outcomes, preserving the source six-of-100 mean rate at mean 15-second pulses. Flavour winnings/coins are narrative only, as in source; they never change bankroll." });
		}
	}

	private void InstallHornblowers()
	{
		var sources = _source.Npcs.Where(x => x.Programs.Contains("C north gypsy_hornblower")).Select(x => x.Vnum).ToArray();
		if (sources.Length == 0) return;
		if (!_clans.TryGetValue(24, out var clan)) throw new InvalidOperationException("Hornblowers require imported clan 24.");
		var prog = NpcServiceProg("ArmageddonWorldfileNorthwardHorn", $"""
if (not(@actor.isawake) or not({SourceDirectionInput("north")}))
	return false
end if
if (isadmin(@actor) or isclanmember(@actor, toclan({clan.Id})) or not(cansee(tocharacter(@npc), @actor)))
	return false
end if
sendzone(@actor.location.zone, "A low, steady baritone horn bellows somewhere nearby.")
return false
""");
		InstallNpcInputHook("Northward Horn", "*", prog, sources);
		Record("npc-special.gypsy-hornblower", nameof(FutureProg), prog.Id, null, new { Sources = sources,
			Disposition = "Public source zone horn on visible ordinary northward movement attempts, with source clan 24/admin exemptions and native abbreviation resolution. Return false preserves movement dispatch. Other directions and injury-pulse code have no source attachment and remain inactive." });
	}

	private void InstallFirePerformance()
	{
		var sources = _source.Npcs.Where(x => x.Programs.Contains("C offer fire_breather")).Select(x => x.Vnum).ToArray();
		if (sources.Length == 0) return;
		var text = new StringBuilder($"""
var artist = tocharacter(@npc)
if (not(@actor.isawake) or @args.Count(word, true) == 0)
	return false
end if
if (targetcharacter(@actor, getindex(@args, 0)) != @artist)
	return false
end if
if (@args.Count(word, true) < 2)
	send(@actor, "Offer him how much?")
	return true
end if
var amount = floor(tonumber(getindex(@args, 1)))
if (@amount < 10)
	force @artist "say I could not possibly perform for that few coins."
	return true
end if
if (countaccessiblecurrency(@actor, tocurrency({_currency.Id})) < @amount)
	force @artist "say You do not have that much."
	return true
end if
if (not(transfercurrency(tocurrency({_currency.Id}), @amount, @actor, @artist)))
	force @artist "say We cannot exchange those coins just now."
	return true
end if
send(@actor, "You dole out your coins for a performance.")
var show = random(1, 4)

""");
		var shows = new[]
		{
			new[] { "$0 juggle|juggles five lit torches high into the air.", "$0 grasp|grasps a torch from the air, pass|passes it under $0's leg, then toss|tosses it back into the air.", "$0 catch|catches the torches, and bow|bows." },
			new[] { "$0 pull|pulls out a two cord polished bone torch and ignite|ignites it.", "$0 tilt|tilts $0's head back, and swallow|swallows the torch.", "$0 remove|removes the torch from $0's mouth, and bow|bows." },
			new[] { "$0 light|lights a two cord torch, which glows orange.", "$0 put|puts the torch in $0's mouth, then remove|removes it, extinguished.", "$0 tilt|tilts $0's head back, and shoot|shoots three balls of fire high into the air.", "$0 bow|bows with a flourish." },
			new[] { "$0 draw|draws out a medium-sized torch and light|lights it.", "$0 cock|cocks $0's head, sheer white smoke trickling from $0's nose, and shoot|shoots a small pillar of fire into the air.", "$0 smack|smacks $0's lips together and bend|bends forward in a bow." }
		};
		for (var i = 0; i < shows.Length; i++)
		{
			text.AppendLine($"if (@show == {i + 1})");
			foreach (var echo in shows[i]) text.AppendLine($"sendloc(@artist.location, {ProgText(echo)}, @artist)");
			text.AppendLine("end if");
		}
		text.AppendLine("return true");
		var prog = NpcServiceProg("ArmageddonWorldfileFirePerformance", text.ToString());
		InstallNpcInputHook("Fire Performance", "offer", prog, sources);
		Record("npc-special.fire-breather", nameof(FutureProg), prog.Id, null, new { Sources = sources,
			Disposition = "Source visible targeted offer, minimum ten coins and acceptance of larger offers, followed by one of four source prose performances. Existing held cash transfers before the performance; no physical torch/fire damage is invented. Unattached pulse advertising remains inactive. Native numeric parsing rejects atoi trailing junk." });
	}

	private void InstallPtarkenInduction()
	{
		var sources = _source.Npcs.Where(x => x.Programs.Contains("C pray lo_ptar")).Select(x => x.Vnum).ToArray();
		if (sources.Length == 0) return;
		const string hometown = "armw_hometown";
		var owner = ProgVariableTypes.Character.ToStorageString();
		if (_db.VariableDefinitions.Any(x => x.OwnerTypeDefinition == owner && x.Property == hometown))
			throw new InvalidOperationException("The source hometown register already exists; do not replace it.");
		_db.VariableDefinitions.Add(new VariableDefinition { OwnerTypeDefinition = owner, Property = hometown, ContainedTypeDefinition = ProgVariableTypes.Text.ToStorageString() });
		_db.VariableDefaults.Add(new VariableDefault { OwnerTypeDefinition = owner, Property = hometown, DefaultValue = "<var></var>" });
		if (!_clans.TryGetValue(9, out var clan)) throw new InvalidOperationException("Lo Ptar requires imported clan 9.");
		var language = _db.Languages.Single(x => x.Name == "Kentu");
		var accent = _db.Accents.Where(x => x.LanguageId == language.Id).OrderBy(x => x.Id).First();
		var races = _db.Races.AsEnumerable().Where(x => x.Name.Split(" (", StringSplitOptions.None)[0].Equals("Halfling", StringComparison.OrdinalIgnoreCase)).Select(x => x.Id).ToArray();
		var race = races.Length == 0 ? "false" : string.Join(" or ", races.Select(id => $"@actor.race.id == {id}"));
		string Say(string message) => $"sendloclang(@actor.location, {ProgText("$0 say|says, \"" + message + "\"")}, tolanguage({language.Id}), toaccent({accent.Id}), @npc)";
		Flush();
		var prog = NpcServiceProg("ArmageddonWorldfilePtarkenInduction", $"""
if (not(@actor.isawake))
	return false
end if
if (not({race}))
	{Say("You are not a halfling. Go away.")}
	return true
end if
if (isclanmember(@actor, toclan({clan.Id})))
	{Say("You are already amoung The People, friend.")}
	return true
end if
if (not(claninvite(@actor, toclan({clan.Id}), torank({_ranks[(9, 0)].Id}))))
	return true
end if
setregister @actor "{hometown}" "Ptarken"
{Say("Welcome to The People, friend halfling.")}
sendloc(@actor.location, "$0 place|places $0's hand on $1's forehead to welcome $1 to the tribe.", @npc, @actor)
return true
""");
		InstallNpcInputHook("Ptarken Induction", "pray", prog, sources);
		Record("npc-special.lo-ptar", nameof(FutureProg), prog.Id, null, new { Sources = sources, Clan = clan.Id, HometownRegister = hometown, Hometown = "Ptarken", SourceCityId = 8,
			Disposition = "Approved native clan induction plus persistent source hometown register. Explicit prayer is the source consent interaction; any prayer reaching this NPC triggers it. Native Kentu public speech retains comprehension without changing the NPC's general speaking selection. Register changes only after successful native membership creation; existing members retain their hometown." });
		Flush();
	}
}

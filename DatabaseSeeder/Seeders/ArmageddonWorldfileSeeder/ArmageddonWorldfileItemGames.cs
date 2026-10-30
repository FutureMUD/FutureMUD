#nullable enable

using System;
using System.Linq;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private void InstallItemGames()
	{
		InstallDicePair();
		InstallDartboard();
	}

	private void InstallDicePair()
	{
		if (!_items.ContainsKey(13602) || !HasRetainedReset(13602)) return;
		ItemNumberRegister("armw_die_one"); ItemNumberRegister("armw_die_two");
		var faces = "var faces = splittext(\"Nothing|Jihae|Lirathu|Knife|Purse|Noose|Kank\", \"|\")";
		var roll = Prog("ArmageddonWorldfileDicePairRoll", $$"""
			if (@args.Count(word, true) == 0)
				return false
			end if
			if (targetitem(@actor, @actor.inventory, getindex(@args, 0)) != @item)
				return false
			end if
			if (@args.Count(word, true) > 1)
				var surface = targetitem(@actor, @actor.location.items, getindex(@args, 1))
				if (isnull(@surface))
					send(@actor, "You do not see that surface here.")
					return true
				end if
				if (not(silentput(@actor, @item, @surface)))
					send(@actor, "You cannot roll the dice there.")
					return true
				end if
			else
				if (not(silentdrop(@actor, @item)))
					send(@actor, "You cannot release the dice right now.")
					return true
				end if
			end if
			var first = random(1, 6)
			var second = random(1, 6)
			setregister @item "armw_die_one" @first
			setregister @item "armw_die_two" @second
			{{faces}}
			sendloc(@actor.location, "$0 roll|rolls $1: " + getindex(@faces, @first) + " and " + getindex(@faces, @second) + ".", @actor, @item)
			return true
			""", ProgVariableTypes.Boolean, ("actor", ProgVariableTypes.Character), ("item", ProgVariableTypes.Item),
			("command", ProgVariableTypes.Text), ("args", ProgVariableTypes.Text | ProgVariableTypes.Collection));
		var inspect = Prog("ArmageddonWorldfileDicePairInspect", $$"""
			if (@args.Count(word, true) == 0)
				return false
			end if
			var target = targetitem(@actor, @actor.inventory, getindex(@args, 0))
			if (isnull(@target))
				target = targetitem(@actor, @actor.location.items, getindex(@args, 0))
			end if
			if (@target != @item)
				return false
			end if
			{{faces}}
			var first = getregister(@item, "armw_die_one")
			var second = getregister(@item, "armw_die_two")
			if (@first == 0 and @second == 0)
				send(@actor, "The dice have not been rolled yet.")
			else
				send(@actor, "The dice show " + getindex(@faces, @first) + " and " + getindex(@faces, @second) + ".")
			end if
			return false
			""", ProgVariableTypes.Boolean, ("actor", ProgVariableTypes.Character), ("item", ProgVariableTypes.Item),
			("command", ProgVariableTypes.Text), ("args", ProgVariableTypes.Text | ProgVariableTypes.Collection));
		var hooks = new[] { ItemCommandHook("Worldfile dice pair roll", "roll", roll), ItemCommandHook("Worldfile dice pair throw", "throw", roll),
			ItemCommandHook("Worldfile dice pair look", "look", inspect), ItemCommandHook("Worldfile dice pair examine", "examine", inspect) };
		foreach (var proto in ItemFamily(13602))
		{
			var values = _effectiveItems[proto.Id].Values;
			AddItemOnLoadProg(proto, "DicePair", $"setregister @item \"armw_die_one\" {Math.Clamp(values[1], 0, 6)}\nsetregister @item \"armw_die_two\" {Math.Clamp(values[2], 0, 6)}\n" +
				string.Join("\n", hooks.Select(x => $"addhook(@item, {x.Id})")) + "\nreturn true");
		}
		Record("item-special.dice-pair", nameof(FutureProg), roll.Id, null, new { Source = 13602,
			Disposition = "One physical source pair, two saved faces, native inventory release/surface placement. Source throw delegates ordinary roll. Unreachable symbolic game remains inactive." });
	}

	private void InstallDartboard()
	{
		var boards = _source.Items.Where(x => _items.ContainsKey(x.Vnum) && HasRetainedReset(x.Vnum) && x.Programs.Contains("C throw dartboard")).ToArray();
		if (boards.Length == 0) return;
		var darts = _source.Items.Where(x => x.ItemType == 24 && _items.ContainsKey(x.Vnum)).SelectMany(x => ItemFamily(x.Vnum)).Select(x => x.Id).ToArray();
		var dartFilter = darts.Length == 0 ? "false" : string.Join(" or ", darts.Select(x => $"@candidate.proto == {x}"));
		var throwing = OwnedBehaviourId("arm.skill.throw", nameof(TraitDefinition)) ?? throw new InvalidOperationException("Dartboards require the native Throwing skill.");
		var attributes = _db.TraitDefinitions.Where(x => x.Type == 1).ToArray();
		var agility = new[] { "Agility", "Dexterity" }.Select(n => attributes.FirstOrDefault(x => x.Name.Equals(n, StringComparison.OrdinalIgnoreCase)))
			.FirstOrDefault(x => x is not null) ?? throw new InvalidOperationException("Dartboards require Agility/Dexterity.");
		var prog = Prog("ArmageddonWorldfileDartboard", $$"""
			if (@args.Count(word, true) < 2)
				return false
			end if
			if (targetitem(@actor, @actor.location.items, getindex(@args, 1)) != @item)
				return false
			end if
			var dart = @actor.helditems.First(candidate, ({{dartFilter}}) and not(isoffhand(@actor, @candidate)))
			if (isnull(@dart))
				dart = @actor.helditems.First(candidate, {{dartFilter}})
			end if
			if (isnull(@dart))
				send(@actor, "You have to be holding a dart to throw it!")
				return true
			end if
			var skill = gettrait(@actor, totrait({{throwing}}))
			var modifier = @skill + 3 * gettrait(@actor, totrait({{agility.Id}})) - 20 * drunkennesslevel(@actor)
			if (isoffhand(@actor, @dart))
				modifier = @modifier - 20
			end if
			var half = floor(@modifier / 2)
			if (@modifier < 0)
				half = 0 - floor((0 - @modifier) / 2)
			end if
			var score = random(1, 100) + @half
			var fell = @score < 35
			if (@score < 25)
				if (not(silentdrop(@actor, @dart)))
					return true
				end if
				sendloc(@actor.location, "$0 throw|throws $1 at $2, but misses.", @actor, @dart, @item)
			else
				var ring = @score
				if (@fell)
					ring = random(35, 100)
				end if
				var name = "King"
				if (@ring < 100)
					name = "Templar"
				end if
				if (@ring < 85)
					name = "Noble"
				end if
				if (@ring < 70)
					name = "Commoner"
				end if
				if (@ring < 50)
					name = "Slave"
				end if
				if (@fell)
					if (not(silentdrop(@actor, @dart)))
						return true
					end if
				else
					forceput(@dart, @item)
				end if
				sendloc(@actor.location, "$0 hurl|hurls $1 into the " + @name + " ring of $2.", @actor, @dart, @item)
				if (@fell)
					sendloc(@actor.location, "$0 wobbles and falls to the ground.", @dart)
				end if
			end if
			if (@skill > 0 and @skill < 40 and @score < 100 and random(0, 3) == 0)
				check(@actor, totrait({{throwing}}), 5)
			end if
			return true
			""", ProgVariableTypes.Boolean, ("actor", ProgVariableTypes.Character), ("item", ProgVariableTypes.Item),
			("command", ProgVariableTypes.Text), ("args", ProgVariableTypes.Text | ProgVariableTypes.Collection));
		var hook = ItemCommandHook("Worldfile dartboard", "throw", prog);
		foreach (var board in boards)
		foreach (var proto in ItemFamily(board.Vnum)) AddItemOnLoadProg(proto, "Dartboard", $"addhook(@item, {hook.Id})\nreturn true");
		Record("item-special.dartboard", nameof(FutureProg), prog.Id, null, new { Sources = boards.Select(x => x.Vnum),
			Disposition = "Source rings, hit/fall boundaries, native dominant hand and intoxication severity. Source training opportunity invokes native skill check/improvement rather than assigning skill points. No combat damage." });
	}
}

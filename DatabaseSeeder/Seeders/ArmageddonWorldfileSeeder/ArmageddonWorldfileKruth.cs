#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Events;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private const string KruthDrawnMask = "armw_kruth_drawn";

	private void InstallKruthDecks()
	{
		var sources = _source.Items.Where(x => x.Programs.Contains("C deal kruth_deck") && _items.ContainsKey(x.Vnum) &&
			_source.Resets.Any(r => r.ReferenceIssues.Length == 0 && r.Opcode is "O" or "P" or "G" or "E" &&
				r.Arguments.Length > 1 && r.Arguments[1] == x.Vnum)).Select(x => x.Vnum).ToArray();
		if (sources.Length == 0) return;
		var cards = new[] { 1810, 1820, 1830, 1840 }.SelectMany(x => Enumerable.Range(x, 6)).ToArray();
		if (cards.Any(x => !_items.ContainsKey(x)))
			throw new InvalidOperationException("Kruth dealing requires all 24 source card definitions.");
		var owner = ProgVariableTypes.Item.ToStorageString();
		if (_db.VariableDefinitions.Any(x => x.OwnerTypeDefinition == owner && x.Property == KruthDrawnMask))
			throw new InvalidOperationException($"Register {KruthDrawnMask} already exists.");
		_db.VariableDefinitions.Add(new VariableDefinition { OwnerTypeDefinition = owner, Property = KruthDrawnMask,
			ContainedTypeDefinition = ProgVariableTypes.Number.ToStorageString() });
		_db.VariableDefaults.Add(new VariableDefault { OwnerTypeDefinition = owner, Property = KruthDrawnMask, DefaultValue = "<var>0</var>" });
		var prototypes = string.Join("\n", cards.Select(x => $"additem prototypes {_items[x].Id}"));
		var bits = string.Join("\n", Enumerable.Range(0, 24).Select(x => $"additem bits {1 << x}"));
		// Uniform selection among remaining cards is equivalent to the source's rejection draw.
		var available = string.Join("\n", Enumerable.Range(0, 24).Select(x =>
			$"if (floor(@mask / {1 << x}) % 2 == 0)\nadditem available {x}\nend if"));
		var prog = Prog("ArmageddonWorldfileKruthDeal", $$"""
			if (@args.Count(word, true) == 0)
				return false
			end if
			var deck = targetitem(@actor, @actor.helditems, getindex(@args, 0))
			if (isnull(@deck))
				return false
			end if
			if (@deck != @item)
				return false
			end if
			if (@args.Count(word, true) == 1)
				send(@actor, "You need to make a list of the people to deal cards to.")
				return true
			end if
			var mask = getregister(@deck, "{{KruthDrawnMask}}")
			var prototypes as number collection
			{{prototypes}}
			var bits as number collection
			{{bits}}
			var available as number collection
			{{available}}
			var argument = 1
			while (@argument < @args.Count(word, true) and @argument <= 24)
				var recipient = targetcharacter(@actor, getindex(@args, @argument))
				if (isnull(@recipient))
					send(@actor, "You don't see that person in the room.")
					return true
				end if
				if (@available.Count(card, true) == 0)
					send(@actor, "$0 is empty.", @deck)
					return true
				end if
				var choice = random(0, @available.Count(card, true) - 1)
				var selected = getindex(@available, @choice)
				var card = loaditem(getindex(@prototypes, @selected))
				if (not(silentget(@recipient, @card)))
					deleteitem(@card)
					send(@actor, "They cannot receive a card right now.")
					return true
				end if
				mask = @mask + getindex(@bits, @selected)
				setregister @deck "{{KruthDrawnMask}}" @mask
				available = @available.Where(candidate, @candidate != @selected)
				if (@recipient == @actor)
					send(@actor, "You deal yourself $0.", @card)
				else
					send(@actor, "You deal a Kruth card to $0.", @recipient)
					send(@recipient, "$0 deals $1 to you.", @actor, @card)
				end if
				foreach (observer in @actor.location.characters)
					if (@observer != @actor and @observer != @recipient)
						if (@recipient == @actor)
							send(@observer, "$0 deals themself a Kruth card.", @actor)
						else
							send(@observer, "$0 deals a Kruth card to $1.", @actor, @recipient)
						end if
					end if
				end foreach
				argument = @argument + 1
			end while
			return true
			""", ProgVariableTypes.Boolean, ("actor", ProgVariableTypes.Character), ("item", ProgVariableTypes.Item),
			("command", ProgVariableTypes.Text), ("args", ProgVariableTypes.Text | ProgVariableTypes.Collection));
		var hook = new Hooks { Id = Next<Hooks>(), Name = "Worldfile Kruth Dealing", Category = "Worldfile", Type = "HookOnInput",
			TargetEventType = (int)EventType.CommandInput, Definition = new XElement("Definition", new XElement("TargetCommand", "deal"),
				new XElement("UseProgResultForHandling", true), new XElement("FutureProg", prog.Id)).ToString() };
		_db.Hooks.Add(hook);
		foreach (var vnum in sources)
		foreach (var proto in new[] { _items[vnum] }.Concat(_resetItemVariants.Where(x => x.Key.StartsWith(vnum + ":", StringComparison.Ordinal)).Select(x => x.Value)).DistinctBy(x => x.Id))
		{
			var initial = _effectiveItems[proto.Id].Values[5] & 0x00FFFFFF;
			AddItemOnLoadProg(proto, "KruthDeck", $"setregister @item \"{KruthDrawnMask}\" {initial}\naddhook(@item, {hook.Id})\nreturn true");
		}
		Record("item-special.kruth-deck", nameof(Hooks), hook.Id, null, new { Sources = sources, Cards = cards, Prog = prog.Id,
			Disposition = "Attached deal only; finite saved per-deck 24-card mask starts from effective source value[5]. Visible held-deck and local-character targeting, ordered recipients, at most 24 per command, private card identities. Native receipt failure deletes only the candidate and leaves its bit available. No shuffle/reset/examine activation, card recycling, game engine or invented winning rules." });
	}
}

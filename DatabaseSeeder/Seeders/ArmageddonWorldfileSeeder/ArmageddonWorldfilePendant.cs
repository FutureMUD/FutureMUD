#nullable enable

using System;
using System.Linq;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private void ItemNumberRegister(string name)
	{
		var owner = ProgVariableTypes.Item.ToStorageString();
		if (_db.VariableDefinitions.Any(x => x.OwnerTypeDefinition == owner && x.Property == name))
			throw new InvalidOperationException($"Register {name} already exists.");
		_db.VariableDefinitions.Add(new VariableDefinition { OwnerTypeDefinition = owner, Property = name,
			ContainedTypeDefinition = ProgVariableTypes.Number.ToStorageString() });
		_db.VariableDefaults.Add(new VariableDefault { OwnerTypeDefinition = owner, Property = name, DefaultValue = "<var>0</var>" });
	}

	private void InstallRubyPendant()
	{
		if (!_items.ContainsKey(69010) || !HasRetainedReset(69010)) return;
		const string charge = "armw_pendant_mana";
		ItemNumberRegister(charge);
		var mana = OwnedBehaviourId(ArmageddonProgressionInstaller.ManaResourceKey, nameof(MagicResource))
			?? throw new InvalidOperationException("The ruby pendant requires the common Mana resource.");
		var prog = Prog("ArmageddonWorldfileRubyPendant", $$"""
			if (not(contains(@actor.wornitems, @item)) or @args.Count(word, true) < 2)
				return false
			end if
			if (targetitem(@actor, @actor.wornitems, getindex(@args, 1)) != @item)
				return false
			end if
			var amount = floor(tonumber(getindex(@args, 0)))
			if (@amount <= 0)
				send(@actor, "Specify a positive amount of Mana.")
				return true
			end if
			var stored = getregister(@item, "{{charge}}")
			var current = magicresourcelevel(@actor, {{mana}})
			if (lowercase(@command) == "donate")
				if (@stored >= 75)
					send(@actor, "The ruby cannot hold any more Mana.")
					return true
				end if
				amount = min(@amount, floor(healthremaining(@actor, "stamina")))
				if (healthremaining(@actor, "stamina") < 5 or @amount > @current)
					send(@actor, "You do not have the strength or Mana for that donation.")
					return true
				end if
				if (spendstamina(@actor, @amount))
					subtractmagicresource(@actor, {{mana}}, @amount)
					setregister @item "{{charge}}" min(75, @stored + floor(@amount / 5))
					send(@actor, "You concentrate, donating your energy to $0.", @item)
				end if
			else
				var delivered = min(min(@amount, @stored), max(0, magicresourcecapacity(@actor, {{mana}}) - @current))
				if (@delivered <= 0)
					send(@actor, "You cannot draw any Mana from the ruby.")
					return true
				end if
				addmagicresource(@actor, {{mana}}, @delivered)
				setregister @item "{{charge}}" (@stored - @delivered)
				send(@actor, "You draw energy from $0.", @item)
			end if
			return true
			""", ProgVariableTypes.Boolean, ("actor", ProgVariableTypes.Character), ("item", ProgVariableTypes.Item),
			("command", ProgVariableTypes.Text), ("args", ProgVariableTypes.Text | ProgVariableTypes.Collection));
		var source = _source.Items.Single(x => x.Vnum == 69010);
		var hooks = new[] { "gather", "donate" }.Where(x => source.Programs.Contains($"C {x} mana_pendant"))
			.Select(x => ItemCommandHook("Worldfile ruby pendant " + x, x, prog)).ToArray();
		foreach (var proto in ItemFamily(69010))
			AddItemOnLoadProg(proto, "RubyPendant", $"setregister @item \"{charge}\" {Math.Clamp(_effectiveItems[proto.Id].Values[5], 0, 75)}\n" +
				string.Join("\n", hooks.Select(x => $"addhook(@item, {x.Id})")) + "\nreturn true");
		Record("item-special.ruby-pendant", nameof(FutureProg), prog.Id, null,
			new { Source = 69010, Mana = mana, Disposition = "Saved charge capped at 75; source five-to-one donation and equal stamina cost. Common Mana, native capacity admission; full ruby refuses donation without wasting resources." });
	}
}

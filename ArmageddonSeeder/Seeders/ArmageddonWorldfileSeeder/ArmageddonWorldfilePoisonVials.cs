using System;
using System.Linq;
using System.Text;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.RPG.Checks;

#nullable enable
namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private void InstallPoisonVials()
	{
		var targets = _effectiveItems.Where(x => x.Value.Source.ItemType == 5 &&
			(x.Value.Values[3] is 1 or 4 || _items.Values.Concat(_resetItemVariants.Values).First(p => p.Id == x.Key).Weight > 6.0 * _kilograms))
			.Select(x => x.Key).ToArray();
		var blockedWeapons = targets.Length == 0 ? "false" : string.Join(" or ", targets.Select(x => $"@target.proto == {x}"));
		foreach (var (protoId, effective) in _effectiveItems.Where(x => x.Value.Source.ItemType == 27).ToArray())
		{
			var proto = _items.Values.Concat(_resetItemVariants.Values).First(x => x.Id == protoId);
			var kind = effective.Values[0];
			InitialisePoisonRegisters(proto, 27, kind is >= 1 and <= 10 ? kind : 0, 0);
			if (!_weaponPoisonProgs.TryGetValue(kind, out var delivery))
			{
				Adapt("poison vial", effective.Source.Vnum, $"Invalid/empty source toxin index {kind}; retained without inventing a toxin.");
				continue;
			}
			var body = new StringBuilder($$"""
				if (@args.Count(word, true) != 0 and @args.Count(word, true) != 1 and @args.Count(word, true) != 3)
				return false
				end if
				var vial = @actor.helditems.First(candidate, getregister(@candidate, "{{PoisonTargetRegister}}") == 27)
				if (@args.Count(word, true) == 3)
				if (getindex(@args, 1) != "with")
				return false
				end if
				vial = targetitem(@actor, @actor.helditems, getindex(@args, 2))
				end if
				if (@vial != @item)
				return false
				end if
				if (@args.Count(word, true) == 0)
				send(@actor, "Use poison <held item> [with <held vial>]. Quote multi-word item names.")
				return true
				end if
				var target = targetitem(@actor, @actor.helditems, getindex(@args, 0))
				if (isnull(@target))
				target = targetitem(@actor, @actor.wieldeditems, getindex(@args, 0))
				end if
				if (isnull(@target))
				send(@actor, "Hold the item you want to poison.")
				return true
				end if
				if (not(canmanipulateitem(@actor, @vial)) or not(canmanipulateitem(@actor, @target)))
				send(@actor, "You cannot apply poison right now.")
				return true
				end if
				var type = getregister(@target, "{{PoisonTargetRegister}}")
				if (@type == 5 or @type == 24)
				if ({{blockedWeapons}})
				send(@actor, "That weapon is too large or of the wrong type to poison.")
				return true
				end if
				if (hasweaponpoison(@target) or @target.quantity != 1)
				send(@actor, "Use one uncoated weapon or dart. Split any stack first.")
				return true
				end if
				if (@type == 5)
				var outcome = actioncheck(@actor, {{(int)CheckType.ApplyPoisonToWeapon}}, 5)
				if (not(canmanipulateitem(@actor, @vial)) or not(canmanipulateitem(@actor, @target)))
				return true
				end if
				if (@outcome < 1)
				deleteitem(@item)
				@{{_poisonDoseProgs[kind].FunctionName}}(@actor, 4, "injected", true)
				sendloc(@actor.location, "$0 slip|slips while applying poison to $1, wasting the vial.", @actor, @target)
				return true
				end if
				end if
				if (not(setweaponpoison(@target, {{delivery.Id}})))
				return true
				end if
				elseif (@type == 17)
				""");
			body.AppendLine();
			body.AppendLine("var originals as number collection\nvar replacements as number collection");
			foreach (var ((baseId, poison), replacement) in _poisonLiquids.Where(x => x.Key.Poison == kind).ToArray())
			{
				body.AppendLine($"additem originals {baseId}\nadditem replacements {replacement.Id}");
				foreach (var (poisonedId, originalId) in _poisonLiquidBases.Where(x => x.Value == baseId))
					body.AppendLine($"additem originals {poisonedId}\nadditem replacements {replacement.Id}");
			}
			body.AppendLine($$"""
				if (not(remapliquids(@target, @originals, @replacements)))
				send(@actor, "That container must be open and contain a supported drink.")
				return true
				end if
				elseif (@type == 19 or @type == 25)
				setregister @target "{{PoisonKindRegister}}" {{kind}}
				else
				send(@actor, "You cannot poison that.")
				return true
				end if
				deleteitem(@item)
				sendloc(@actor.location, "$0 carefully apply|applies the contents of a poison vial to $1.", @actor, @target)
				return true
				""");
			var prog = Prog("ArmageddonWorldfilePoisonVialInput" + proto.Id, body.ToString(), ProgVariableTypes.Boolean,
				("actor", ProgVariableTypes.Character), ("item", ProgVariableTypes.Item), ("command", ProgVariableTypes.Text),
				("args", ProgVariableTypes.Text | ProgVariableTypes.Collection));
			AttachSpiceHook(proto, "PoisonVialInputHook", SpiceHook("poison", prog));
			Record("poison.vial." + proto.Id, "Prog", prog.Id, null, new { SourceVnum = effective.Source.Vnum, PoisonIndex = kind,
				Disposition = "Held vial and held/wielded target; native manipulation checks. Weapon application uses native poisoning check, consumes failed vials and permits a saved 4 mg mishap. Darts/food/spice/drinks use the source's unchecked application route. Hits use approved uniform 1–4 mg, source toxin-specific policies and native poison save." });
		}
	}
}

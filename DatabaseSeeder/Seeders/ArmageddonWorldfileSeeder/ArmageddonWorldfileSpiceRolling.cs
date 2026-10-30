#nullable enable

using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private void InstallSpiceRolling(GameItemProto proto, int skill)
	{
		if (!_items.TryGetValue(64150 + skill, out var rolled)) return;
		var prog = Prog("ArmageddonWorldfileSpiceRoll" + proto.Id, $$"""
			if (@args.Count(word, true) < 3 or @args.Count(word, true) > 4)
				return false
			end if
			if (lowercase(getindex(@args, 0)) != "smoke")
				return false
			end if
			if (targetitem(@actor, @actor.helditems, getindex(@args, 2)) != @item)
				return false
			end if
			var written = false
			if (@args.Count(word, true) == 4)
				if (lowercase(getindex(@args, 3)) != "written")
					send(@actor, "Use make smoke <paper or book> <spice> [written]. The written option deliberately destroys the selected page.")
					return true
				end if
				written = true
			end if
			var paper = targetitem(@actor, @actor.helditems, getindex(@args, 1))
			if (isnull(@paper) or not(@actor.isawake) or @actor.incombat or isincorporeal(@actor))
				send(@actor, "Hold the paper and loose spice while you are able to work.")
				return true
			end if
			var units = getregister(@item, "{{SpiceUnitsRegister}}")
			if (@units <= 0 or @units > 5)
				send(@actor, "You need a small pinch of spice. Shave a large knot first.")
				return true
			end if
			if (not(canconsumepage(@actor, @paper, @written)))
				send(@actor, "You need a blank sheet or an open book with a blank page. To deliberately destroy its selected written page, append written.")
				return true
			end if
			var poison = getregister(@item, "{{PoisonKindRegister}}")
			var roll = loaditem({{rolled.Id}})
			if (isnull(@roll))
				send(@actor, "You cannot prepare that roll right now.")
				return true
			end if
			if (not(contains(@actor.helditems, @item)) or getregister(@item, "{{SpiceUnitsRegister}}") != @units)
				deleteitem(@roll)
				return true
			end if
			if (not(consumepage(@actor, @paper, @written)))
				deleteitem(@roll)
				return true
			end if
			setregister @roll "{{SpiceUnitsRegister}}" @units
			setregister @roll "{{PoisonKindRegister}}" @poison
			deleteitem(@item)
			if (not(silentget(@actor, @roll)))
				put(@roll, @actor.location, @actor.layer)
				send(@actor, "The finished roll is left at your feet because you cannot hold it.")
			end if
			sendloc(@actor.location, "$0 carefully roll|rolls a pinch of spice in paper.", @actor)
			return true
			""", ProgVariableTypes.Boolean, ("actor", ProgVariableTypes.Character), ("item", ProgVariableTypes.Item),
			("command", ProgVariableTypes.Text), ("args", ProgVariableTypes.Text | ProgVariableTypes.Collection));
		AttachSpiceHook(proto, "SpiceRolling", SpiceHook("make", prog));
	}
}

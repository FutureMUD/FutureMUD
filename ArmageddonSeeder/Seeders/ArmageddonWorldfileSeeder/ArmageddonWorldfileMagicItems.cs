#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Events;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private IEnumerable<GameItemProto> ItemFamily(int vnum) => new[] { _items[vnum] }
		.Concat(_resetItemVariants.Where(x => x.Key.StartsWith(vnum + ":", StringComparison.Ordinal)).Select(x => x.Value)).DistinctBy(x => x.Id);

	private bool HasRetainedReset(int vnum) => _source.Resets.Any(x => x.ReferenceIssues.Length == 0 &&
		x.Opcode is "O" or "P" or "G" or "E" && x.Arguments.Length > 1 && x.Arguments[1] == vnum);

	private Hooks ItemCommandHook(string name, string command, FutureProg prog)
	{
		var hook = new Hooks { Id = Next<Hooks>(), Name = name, Category = "Worldfile", Type = "HookOnInput",
			TargetEventType = (int)EventType.CommandInput, Definition = new XElement("Definition",
				new XElement("TargetCommand", command), new XElement("UseProgResultForHandling", true), new XElement("FutureProg", prog.Id)).ToString() };
		_db.Hooks.Add(hook);
		return hook;
	}

	internal static ConditionalMagicItemPolicy AttachedGemPolicy(ConditionalMagicItemPolicy native, long eligible,
		long controller, long territory, long military, bool remove, bool cast) => native with
	{
		EligibilityProgId = eligible, ControllerProgId = controller, TerritoryProgId = territory,
		ProtectedTargetProgId = military, EscortProgId = 0, IncompatibleWearerProgId = 0,
		RequiredRegaliaPrototypeId = 0, BindingGroup = "armageddon.worldfile.allanak.gem",
		PreventRemoval = remove, Removal = new ItemConsequence(15, 20, 30, 10, true),
		Activation = new ItemConsequence(20, 40, 40, 10, true), Range = BindingActivationRange.World,
		ActivationChance = 0, Periodic = new(), IncompatibleWear = new(), InterceptHostileSpells = cast
	};

	private void InstallBindingItems()
	{
		var gems = _source.Items.Where(x => x.Programs.Any(p => p is "C remove allank_gem" or "C cast allank_gem") &&
			_items.ContainsKey(x.Vnum) && (HasRetainedReset(x.Vnum) || x.Vnum == 50606)).ToArray();
		var robes = _source.Items.Where(x => x.Programs.Any(p => p is "C reach allank_robe" or "C slap allank_robe") &&
			_items.ContainsKey(x.Vnum) && HasRetainedReset(x.Vnum)).ToArray();
		if (gems.Length == 0 && robes.Length == 0) return;
		var templateId = OwnedBehaviourId("arm.templar.southern.gem.component", nameof(GameItemComponentProto))
			?? throw new InvalidOperationException("Worldfile gems require the native Southern Templar binding component.");
		var native = ConditionalMagicItemPolicy.Load(XElement.Parse(_db.GameItemComponentProtos.Single(x => x.Id == templateId).Definition));
		var role = OwnedBehaviourId("arm.class.southern_templar", nameof(ChargenRole))
			?? throw new InvalidOperationException("Worldfile robes require the Southern Templar role.");
		var robeIds = robes.SelectMany(x => ItemFamily(x.Vnum)).Select(x => x.Id).ToArray();
		var wornRobe = robeIds.Length == 0 ? "false" : "@actor.wornitems.Any(robe, " + string.Join(" or ", robeIds.Select(x => $"@robe.proto == {x}")) + ")";
		var authority = Prog("ArmageddonWorldfileRobeAuthority", $"return isclanmember(@actor, toclan({_clans[12].Id})) and contains(@actor.roles, torole({role})) and ({wornRobe})",
			ProgVariableTypes.Boolean, ("actor", ProgVariableTypes.Character));
		var military = Prog("ArmageddonWorldfileGemProtectedTarget", $"return isclanmember(@actor, toclan({_clans[12].Id}))",
			ProgVariableTypes.Boolean, ("actor", ProgVariableTypes.Character));
		var territory = Prog("ArmageddonWorldfileGemTerritory", $"return not(isnull(@actor.location)) and @actor.location.zone.id == {_zones[50].Id}",
			ProgVariableTypes.Boolean, ("actor", ProgVariableTypes.Character));
		var capabilities = new[] { "earth", "fire", "water", "wind", "shadow", "lightning", "void", "defiler" }
			.Select(x => OwnedBehaviourId("arm.capability." + x, nameof(MagicCapability))).Where(x => x.HasValue).Select(x => x!.Value).ToArray();
		var eligible = Prog("ArmageddonWorldfileGemWearer", "return " + (capabilities.Length == 0 ? "false" :
			string.Join(" or ", capabilities.Select(x => $"contains(@actor.magiccapabilities, tomagiccapability({x}))"))),
			ProgVariableTypes.Boolean, ("actor", ProgVariableTypes.Character));
		foreach (var gem in gems)
		{
			var policy = AttachedGemPolicy(native, eligible.Id, authority.Id, territory.Id, military.Id,
				gem.Programs.Contains("C remove allank_gem"), gem.Programs.Contains("C cast allank_gem"));
			foreach (var proto in ItemFamily(gem.Vnum)) Attach(proto, Component("binding." + proto.Id, "ConditionalBinding", policy.Save()));
			Record("item-special.binding." + gem.Vnum, "ConditionalBinding", _items[gem.Vnum].Id, null,
				new { Source = gem.Programs, Policy = policy, Disposition = "Only attached removal and hostile-casting callbacks. No pulse, wear rejection or put restrictions are enabled. Native binding activation remains available to worn authorised regalia." });
		}
		var outputs = new Dictionary<string, int> { ["token"] = 2818, ["dustcloak"] = 50102, ["gem"] = 50606, ["pass"] = 50099 }
			.Where(x => _items.ContainsKey(x.Value)).ToArray();
		foreach (var robe in robes)
		{
			var policy = new ConditionalMagicItemPolicy { Kind = ConditionalMagicItemKind.ItemIssuer, EligibilityProgId = authority.Id,
				RequireWorn = true, BindingGroup = "armageddon.worldfile.allanak.gem", IssuedPrototypeIds = outputs.Select(x => _items[x.Value].Id).ToArray() };
			foreach (var proto in ItemFamily(robe.Vnum)) Attach(proto, Component("robe." + proto.Id, "ItemIssuer", policy.Save()));
			foreach (var command in new[] { "reach", "slap" }.Where(c => robe.Programs.Contains($"C {c} allank_robe")))
			{
				var body = command == "reach" ? $$"""
					if (@args.Count(word, true) < 2)
						return false
					end if
					if (targetitem(@actor, @actor.wornitems, getindex(@args, 0)) != @item)
						return false
					end if
					{{string.Join("\n", outputs.Select(x => $"if (lowercase(getindex(@args, 1)) == \"{x.Key}\")\nforce @actor (\"itempower \" + bestkeyword(@actor, @item) + \" issue {_items[x.Value].Id}\")\nreturn true\nend if"))}}
					send(@actor, "Reach into your robe for a token, dustcloak, gem or pass.")
					return true
					""" : "if (@args.Count(word, true) == 0)\nreturn false\nend if\nforce @actor (\"itempower \" + bestkeyword(@actor, @item) + \" shock \" + getindex(@args, 0))\nreturn true";
				var prog = Prog($"ArmageddonWorldfileRobe{robe.Vnum}{command}", $"if (not(contains(@actor.wornitems, @item)) or not(@{authority.FunctionName}(@actor)))\nreturn false\nend if\n" + body,
					ProgVariableTypes.Boolean, ("actor", ProgVariableTypes.Character), ("item", ProgVariableTypes.Item),
					("command", ProgVariableTypes.Text), ("args", ProgVariableTypes.Text | ProgVariableTypes.Collection));
				var hook = ItemCommandHook($"Worldfile robe {robe.Vnum} {command}", command, prog);
				foreach (var proto in ItemFamily(robe.Vnum)) AddItemOnLoadProg(proto, "Robe" + command, $"addhook(@item, {hook.Id})\nreturn true");
			}
		}
	}
}

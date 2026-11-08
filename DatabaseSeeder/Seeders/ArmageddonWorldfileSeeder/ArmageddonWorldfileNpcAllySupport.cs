#nullable enable

using System;
using System.Linq;
using System.Text;
using MudSharp.FutureProg;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private const string NpcTribeRegister = "arm_npc_legacy_tribe";

	private void InstallNpcAllySupport()
	{
		EnsureMountRegister(ProgVariableTypes.Character, NpcTribeRegister, ProgVariableTypes.Number, "<var>0</var>");
		var friends = new StringBuilder($$"""
		if (@npc == @ally or not(@ally.incombat) or not(cansee(@npc, @ally)))
			return false
		end if
		var tribe = getregister(@npc, "{{NpcTribeRegister}}")
		var other = getregister(@ally, "{{NpcTribeRegister}}")
		return (@tribe > 0 and @tribe == @other) or (@tribe == 14 and @other == 24) or (@tribe == 24 and @other == 14) or (@tribe == 66 and @other == 4) or @npc.clans.Any(clan, isclanmember(@ally, @clan))
		""");
		// NPC groups can also recognise player members of the corresponding imported allied clan.
		foreach (var (from, to) in new[] { (14, 24), (24, 14), (66, 4) })
			if (_clans.TryGetValue(to, out var clan)) friends.Append($" or (@tribe == {from} and isclanmember(@ally, toclan({clan.Id})))");
		var relation = Prog("ArmageddonWorldfileCombatAllies", friends.ToString(), ProgVariableTypes.Boolean,
			("npc", ProgVariableTypes.Character), ("ally", ProgVariableTypes.Character));
		foreach (var source in _source.Npcs)
		{
			if (source.Identity[4] > 0)
			{
				var onload = _npcCombatOnloads["ArmageddonWorldfileNpcCash" + source.Vnum];
				onload.FunctionText = $"setregister @npc \"{NpcTribeRegister}\" {source.Identity[4]}\n" + onload.FunctionText;
			}
			if ((source.Flags[0] & 1) != 0 || source.Identity[4] <= 0 && source.Clans.Length == 0) continue;
			var rescue = source.Skills.Any(x => x.Length >= 2 && x[0] >= 0 && x[0] < _source.Skills.Length &&
				_source.Skills[x[0]].Equals("rescue", StringComparison.OrdinalIgnoreCase));
			var code = new StringBuilder("if (not(@npc.isawake) or @npc.incombat or not(@npc.canmove) or @npc.possessednpc or haspausedaieffect(@npc))\nreturn false\nend if\nforeach (ally in @npc.location.characters)\n");
			code.AppendLine($"if (@{relation.FunctionName}(@npc, @ally))");
			code.AppendLine("if (healthremaining(@ally, \"health\") < healthcapacity(@ally, \"health\") * 0.5)");
			if (rescue) code.AppendLine("force @npc (\"rescue \" + bestkeyword(@npc, @ally))\nreturn true");
			else if (_npcHealingRoutes.TryGetValue(source.Vnum, out var healing))
			{
				var level = Math.Max(0, (healing.Proficiency - 30) / 10);
				code.AppendLine($"var mana = magicresourcelevel(@npc, {healing.Reserve})\nif (@mana >= 10)\nvar reduction = 4\nif (@mana >= 50)\nreduction = 1\nelseif (@mana >= 33)\nreduction = 2\nelseif (@mana >= 20)\nreduction = 3\nend if\nvar grade = min(7, max(1, {level} - @reduction + 1))\nvar result = channelspell(@npc, tomagiccapability({healing.Capability}), tomagicspell({healing.Spell}), @grade, false, bestkeyword(@npc, @ally))\nif (not(startswith(@result, \"Refused:\")))\nreturn true\nend if\nend if");
			}
			code.AppendLine("end if\nvar opponent = @ally.combattargetchar\nif (not(isnull(@opponent)) and cansee(@npc, @opponent))\nforce @npc (\"kill \" + bestkeyword(@npc, @opponent))\nreturn true\nend if\nend if\nend foreach");
			if ((source.Flags[0] & 2) == 0)
				code.AppendLine($"foreach (room in @npc.location.surrounds)\nforeach (ally in @room.characters)\nif (@{relation.FunctionName}(@npc, @ally))\nvar path = getpathrespectdoors(@npc, @room)\nif (@path.Count(step, true) > 0 and @path.Count(step, true) <= 2)\nforce @npc getindex(@path, 0)\nreturn true\nend if\nend if\nend foreach\nend foreach");
			code.AppendLine("return false");
			var prog = Prog("ArmageddonWorldfileAssistAllies" + source.Vnum, code.ToString(), ProgVariableTypes.Boolean,
				("npc", ProgVariableTypes.Character));
			InstallNpcTickHook("Ally Assistance " + Key(source.Vnum), prog, [source.Vnum]);
			Record("npc.combat.allies." + source.Vnum, "NpcAllyAssistance", prog.Id, null,
				new { source.Vnum, Tribe = source.Identity[4], Rescue = rescue, Healing = _npcHealingRoutes.ContainsKey(source.Vnum),
					Policy = "No Help suppresses assistance. Native clans plus registered unnamed source groups and explicit source alliances. Rescue precedes paid healing for injured allies, then native engagement. Non-sentinels approach adjacent visible fighting allies through an admitted native path of at most two exits. Existing ten-second hooks replace source scan timing; no companion or clan is invented." });
		}
	}
}

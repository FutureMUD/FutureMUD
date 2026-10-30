#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private static readonly (int Bit, string Function)[] InitialNpcStatuses =
	[
		(0, "addblindnesseffect"), (1, "addvisualinviseffect"), (3, "adddetectinvisibleeffect"),
		(4, "adddetectmagickeffect"), (6, "adddetectetherealeffect"), (11, "addflighteffect"),
		(12, "addclimbingeffect"),
		(24, "addpacifismeffect"),
		(23, "addspottingeffect"), (26, "addlisteningeffect"),
		(17, "addinfravisioneffect"), (19, "addsneakeffect"), (20, "addhideeffect"),
		(27, "adddetectpoisoneffect"), (30, "addsilenceeffect")
	];

	private void InstallNpcInitialStates()
	{
		long? shadow = null;
		foreach (var source in _source.Npcs.Where(x => x.Flags[2] != 0))
		{
			var mask = unchecked((uint)source.Flags[2]);
			var mapped = 0u;
			const uint markerBits = (1u << 2) | (1u << 10) | (1u << 14) | (1u << 18) | (1u << 21) | (1u << 22) | (1u << 25) | (1u << 31);
			var code = new StringBuilder();
			foreach (var (bit, function) in InitialNpcStatuses)
			{
				if ((mask & (1u << bit)) == 0) continue;
				code.AppendLine($"{function}(@npc)");
				if (bit == 11) code.AppendLine("force @npc \"fly\"");
				mapped |= 1u << bit;
			}
			if ((mask & (1u << 16)) != 0)
			{
				shadow ??= OwnedBehaviourId("arm.support.plane.shadow", nameof(Plane))
					?? throw new InvalidOperationException("Ethereal NPCs require the installed Armageddon Shadow plane.");
				if (!_db.Planes.Any(x => x.Id == shadow.Value))
					throw new InvalidOperationException("The installed Armageddon Shadow plane identity is missing.");
				code.AppendLine($"applyplanarstate(@npc, \"noncorporeal\", \"{shadow.Value}\", false, false, false)");
				mapped |= 1u << 16;
			}
			foreach (var (bit, multiplier) in new[] { (8, "0.5"), (13, "2") })
			{
				if ((mask & (1u << bit)) == 0) continue;
				code.AppendLine($"addmovementtempoeffect(@npc, {multiplier})");
				if (bit == 13) code.AppendLine("addcombattempoeffect(@npc, 2)");
				mapped |= 1u << bit;
			}
			if ((mask & (1u << 5)) != 0)
			{
				code.AppendLine("adddamageprotectioneffect(@npc, 0, 1)"); mapped |= 1u << 5;
			}
			if ((mask & (1u << 7)) != 0)
			{
				code.AppendLine("adddamageprotectioneffect(@npc, 0.5, 0)"); mapped |= 1u << 7;
			}
			if (code.Length > 0) AppendNpcOnLoadCode(source.Vnum, code.ToString());
			Record("npc-initial-state." + source.Vnum, "NpcInitialState", _npcs[source.Vnum].Id, null,
				new { Source = source.Vnum, AffectedBy = mask, Mapped = mapped, RetainedMarkers = mask & markerBits, Remaining = mask & ~mapped & ~markerBits,
					Disposition = "Initial native saved effects compose the existing cash/outfit onload prog and its reset variants. Visual invisibility retains other senses; flight attempts native flying posture without changing rooms. Ethereal uses the existing Shadow planar rules with no closed-door/magical-barrier bypass or player transitions. A raw bit without its source ownership/effect payload does not invent a caster, master or resource/attribute modifier." });
		}
	}
}

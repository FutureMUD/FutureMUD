#nullable enable

using System.Xml.Linq;
using MudSharp.FutureProg;
using MudSharp.Health;

namespace MudSharp.Magic;

public static class ArmageddonBarrierSpellContent
{
	public static ArmageddonUtilitySpellContent FlameBarrier() => Create("flame_barrier", "Flame Barrier", DamageType.Burning);
	public static ArmageddonUtilitySpellContent ThornBarrier() => Create("thorn_barrier", "Thorn Barrier", DamageType.Piercing);
	public static ArmageddonUtilitySpellContent BladeBarrier() => Create("blade_barrier", "Blade Barrier", DamageType.Slashing);
	private static ArmageddonUtilitySpellContent Create(string key, string name, DamageType type) => new($"arm.spell.{key}", name,
		$"Block physical passage for 60*external grade seconds. Each admitted crossing attempt inflicts 3*grade native damage, then blocks passage. {type} is an editable authored native damage choice. " +
		"Ordinary movement, door and guard admission precede contact. Queries, denied attempts, replay, and native planar bypass earn no contact injury. The spell parent owns expiry, cold remaining life and dispel; no source component is required.",
		"60*grade", 10, "$0 raise|raises a magical barrier across $1.",
		(resource, cost, filter) => ArmageddonUtilitySpellContent.Definition($"arm.spell.{key}", "exit", resource, cost, filter, 40, 10,
			new XElement("Effect", new XAttribute("type", "damagingexitbarrier"), new XElement("DamageType", (int)type), new XElement("DamagePerGrade", 3))),
		TargetType: ProgVariableTypes.Exit);
}

#nullable enable

using System.Xml.Linq;

namespace MudSharp.Magic;

/// <summary>Finite stock use of native corpse-linked possession.</summary>
public static class ArmageddonBorrowTheDeadSpellContent
{
	public const string Key = "arm.spell.borrow_the_dead";

	public static ArmageddonCanonicalSpellContribution Create() => new("arm.capability.void",
		new ArmageddonUtilitySpellContent(Key, "Borrow the Dead",
			"Possesses one eligible local corpse through its original native body for 60*grade seconds. The exact corpse, body, original character, caster anchor and room are selected and reconfirmed before payment. The spell-owned possession child restores the corpse and primary focus on closure; administrators and skeletal corpses are refused by default. Builder-configured corpse classes remain subject to native possession safety checks.",
			"60*grade", 33, "$0 draw|draws a borrowed will into $1.",
			(resource, cost, filter) => ArmageddonUtilitySpellContent.Definition(Key, "corpse", resource, cost, filter, 30, 33,
				new XElement("Effect", new XAttribute("type", "possesscorpse"),
					new XElement("AllowPCs", true), new XElement("AllowNPCs", true),
					new XElement("AllowAdmins", false), new XElement("AllowFinal", true),
					new XElement("AllowSkeletal", false),
					new XElement("PossessionEcho", "Your will hauls the dead body into motion."),
					new XElement("TargetEcho", ""),
					new XElement("RoomEcho", "@ jerk|jerks upright with a borrowed will."),
					new XElement("CollapseEcho", "The corpse falls still and your focus returns to your body."),
					new XElement("BacklashEcho", ""),
					new XElement("RestoreEcho", "@ collapse|collapses back into $1.")))));
}

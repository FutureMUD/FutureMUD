#nullable enable

using System;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction;

namespace MudSharp.Magic;

/// <summary>A saved physical position, with no ownership claim over the existing traveller or room.</summary>
public sealed record SpellReturnAnchor(long CharacterId, long InstanceId, long BodyId, long RoomId,
	long OverlayId, long TerrainId, RoomLayer Layer)
{
	public const string Family = "delayed-return";
	public string Save() => new XElement("Return", new XAttribute("version", 1), new XAttribute("character", CharacterId),
		new XAttribute("instance", InstanceId), new XAttribute("body", BodyId), new XAttribute("room", RoomId),
		new XAttribute("overlay", OverlayId), new XAttribute("terrain", TerrainId), new XAttribute("layer", (int)Layer)).ToString(SaveOptions.DisableFormatting);
	public static SpellReturnAnchor Parse(string text)
	{
		var xml = XElement.Parse(text);
		if (xml.Name != "Return" || (int?)xml.Attribute("version") != 1) throw new FormatException("Unsupported delayed return anchor.");
		var anchor = new SpellReturnAnchor((long)xml.Attribute("character")!, (long)xml.Attribute("instance")!,
			(long)xml.Attribute("body")!, (long)xml.Attribute("room")!, (long)xml.Attribute("overlay")!,
			(long)xml.Attribute("terrain")!, (RoomLayer)(int)xml.Attribute("layer")!);
		if (anchor.CharacterId <= 0 || anchor.InstanceId <= 0 || anchor.BodyId <= 0 || anchor.RoomId <= 0 ||
			anchor.OverlayId <= 0 || anchor.TerrainId <= 0 || !Enum.IsDefined(anchor.Layer)) throw new FormatException("Invalid delayed return identity or endpoint.");
		return anchor;
	}
}

public sealed record SpellReturnBinding(Guid Id, SpellReturnAnchor Anchor, DateTime DeadlineUtc);

public interface ISpellReturnService
{
	SpellReturnBinding CreatePrepared(ICharacter caster, IMagicSpell spell, int grade, double secondsPerGrade,
		SpellReturnAnchor anchor, Func<bool> inputsCurrent);
	bool TryReturn(SpellReturnBinding binding, ICharacter actor, IMagicSpell spell, Func<bool> stillAttached);
	void Cancel(SpellReturnBinding binding, ICharacter actor, SpellRetirementReason reason);
	int Reconcile(DateTime nowUtc, int limit = 100);
}

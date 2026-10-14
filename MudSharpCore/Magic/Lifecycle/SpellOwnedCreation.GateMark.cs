#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Magic.Lifecycle;

public sealed partial class SpellOwnedCreation
{
	/// <summary>Debit exactly one persisted marked-room use in the topology creation transaction.</summary>
	internal void DebitMarkedRoomUse(long itemId, long bodyId, Guid parentIdentity, long parentSpellId,
		long savedRoomId, int expectedUses)
	{
		if (_gate is null || itemId <= 0 || bodyId <= 0 || parentIdentity == Guid.Empty ||
			parentSpellId <= 0 || savedRoomId <= 0 || expectedUses is < 1 or > 7)
			throw new InvalidOperationException("A prepared Nilaz creation must name one exact active marked item.");
		var item = Context.GameItems.SingleOrDefault(x => x.Id == itemId);
		if (item is null || item.ContainerId is not null ||
			!Context.BodiesGameItems.Any(x => x.GameItemId == itemId && x.BodyId == bodyId) ||
			Context.RoomsGameItems.Any(x => x.GameItemId == itemId))
			throw new InvalidOperationException("The saved mark item is no longer directly carried by the prepared body.");
		XElement root;
		try { root = XElement.Parse(item.EffectData); }
		catch (System.Xml.XmlException) { throw new InvalidOperationException("The saved item effect graph cannot prove the prepared mark."); }
		var parents = root.Elements("Effect").Where(x => x.Element("Type")?.Value == "MagicSpellParent" &&
			Guid.TryParse(x.Element("Effect")?.Element("Identity")?.Value, out var id) && id == parentIdentity &&
			(long?)x.Element("Effect")?.Element("Spell") == parentSpellId).ToArray();
		if (parents.Length != 1) throw new InvalidOperationException("The saved Arcane Mark parent is missing or ambiguous.");
		var marks = parents[0].Element("Effect")?.Element("Children")?.Elements("Effect")
			.Where(x => x.Element("Type")?.Value == "SpellMagicTag" &&
				x.Element("Effect")?.Element("Tag")?.Value == ArmageddonSimpleStockContent.ArcaneMarkTag &&
				(long?)x.Element("Effect")?.Element("ItemId") == itemId &&
				(long?)x.Element("Effect")?.Element("SavedRoomId") == savedRoomId).ToArray() ?? [];
		if (marks.Length != 1 || (int?)marks[0].Element("Effect")?.Element("RemainingUses") != expectedUses)
			throw new InvalidOperationException("The saved Arcane Mark use count changed; no gate was created.");
		if (expectedUses == 1)
		{
			marks[0].Remove();
			if (!parents[0].Element("Effect")!.Element("Children")!.Elements("Effect").Any()) parents[0].Remove();
		}
		else marks[0].Element("Effect")!.Element("RemainingUses")!.Value = (expectedUses - 1).ToString();
		item.EffectData = root.ToString(SaveOptions.DisableFormatting);
		CaptureBorrowedChange(item);
	}
}

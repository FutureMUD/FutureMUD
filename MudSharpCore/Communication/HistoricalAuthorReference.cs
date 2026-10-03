#nullable enable

using System;
using System.Xml;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Framework;
using RuntimeCharacter = MudSharp.Character.Character;

namespace MudSharp.Communication;

/// <summary>Authorship retains canonical IDs, never ownership of a physical actor graph.</summary>
internal static class HistoricalAuthorReference
{
	internal static ICharacter? Live(IFuturemud world, long? id, ref WeakReference<ICharacter>? cached)
	{
		// Another owned native process can commit the archive before this host releases
		// its old runtime object. The durable identity wins over that stale cache.
		if (Archived(world, id, ref cached) is not null) return null;
		if (cached?.TryGetTarget(out var actor) == true)
		{
			if (actor is not RuntimeCharacter { IsArchived: true }) return actor;
			cached = null;
		}
		if (id is not > 0) return null;
		var loaded = world.TryGetCharacter(id.Value, true);
		if (loaded is null or RuntimeCharacter { IsArchived: true }) return null;
		cached = new WeakReference<ICharacter>(loaded);
		return loaded;
	}

	internal static ArchivedCharacterIdentity? Archived(IFuturemud world, long? id,
		ref WeakReference<ICharacter>? cached)
	{
		var archive = id is > 0 ? world.CharacterArchives?.Find(id.Value) : null;
		if (archive is null) return null;
		cached = null;
		if (archive.FullName is not null || string.IsNullOrWhiteSpace(archive.NameInfo)) return archive;
		try
		{
			var name = XElement.Parse(archive.NameInfo).Element("PersonalName")?.Element("Name");
			if (name is not null && name.Elements("Element").All(element =>
					Enum.TryParse<NameUsage>(element.Attribute("usage")?.Value, out _)) &&
				long.TryParse(name.Attribute("culture")?.Value, out var cultureId) &&
				world.NameCultures?.Get(cultureId) is { } culture)
			{
				var fullName = new PersonalName(culture, name).GetName(NameStyle.FullName);
				if (!string.IsNullOrWhiteSpace(fullName)) return archive with { FullName = fullName };
			}
		}
		catch (Exception ex) when (ex is XmlException or FormatException or ArgumentException)
		{
			// Legacy or damaged metadata still has the bounded immutable display name.
		}
		return archive;
	}
}

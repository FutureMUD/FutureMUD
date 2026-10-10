#nullable enable

using System.Globalization;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Effects.Concrete;

namespace MudSharp.Magic.Lifecycle;

public sealed partial class SpellOwnedShelterService
{
	internal void RequireNoForeignRoomEffectReferences(FuturemudDatabaseContext context, IRoom room)
	{
		foreach (var actor in _world.Actors.Concat(_world.CachedActors)
			.SelectMany(x => x.Identity.Instances.OfType<ICharacter>()).Concat(room.Characters)
			.Distinct(ReferenceEqualityComparer.Instance).OfType<ICharacter>())
			RequireNoForeignRoomEffectData(actor.SaveEffects().ToString(), room.Id);
		foreach (var data in context.Characters.AsNoTracking().Select(x => x.EffectData))
			RequireNoForeignRoomEffectData(data, room.Id);
	}

	internal static void RequireNoForeignRoomEffectData(string? data, long roomId)
	{
		if (string.IsNullOrWhiteSpace(data)) return;
		var pending = new Stack<XElement>(XElement.Parse(data).Elements("Effect"));
		while (pending.TryPop(out var envelope))
		{
			var type = envelope.Element("Type")?.Value;
			var definition = envelope.Element("Effect");
			// These two native parents serialize/load child effect envelopes at this exact path.
			if (type is "MagicSpellParent" or "SubstanceExposure")
				foreach (var child in definition?.Element("Children")?.Elements("Effect") ?? []) pending.Push(child);
			IEnumerable<string> values = type switch
			{
				"BodyBackup" or "SpellBodyBackup" => [definition?.Element("DestinationCellId")?.Value ?? "0"],
				"NpcHomeBase" => [definition?.Attribute("HomeCellId")?.Value ?? "0"],
				"AnimalHunt" or "MonsterIntent" => [definition?.Element("Origin")?.Value ?? "0", definition?.Element("LastCell")?.Value ?? "0"],
				"AdminSpyMaster" => definition?.Elements("Spy").Select(x => x.Value) ?? [],
				"NpcKnownThreatLocations" => definition?.Elements("Cell").Select(x => x.Attribute("id")?.Value ?? "0") ?? [],
				"NpcKnownWaterLocations" => definition?.Elements("Cell").Select(x => x.Value) ?? [],
				_ => []
			};
			if (values.Any(x => long.Parse(x, CultureInfo.InvariantCulture) == roomId))
				throw new InvalidOperationException($"Known {type} room reference requires explicit recovery before shelter removal.");
		}
	}

	private void RemoveRoomWorkPermits(IRoom room)
	{
		foreach (var actor in _world.Actors.Concat(_world.CachedActors)
			.SelectMany(x => x.Identity.Instances.OfType<ICharacter>()).Concat(room.Characters)
			.Distinct(ReferenceEqualityComparer.Instance).OfType<ICharacter>())
		{
			foreach (var permit in actor.EffectsOfType<PermitWork>().Where(x => x.Property is null && x.Controller is null && x.Room?.Id == room.Id).ToArray())
				actor.RemoveEffect(permit, true);
		}
	}

	private static void RemovePersistedRoomWorkPermits(FuturemudDatabaseContext context, long roomId)
	{
		foreach (var row in context.Characters.Where(x => x.EffectData != null && x.EffectData != ""))
			row.EffectData = RemoveRoomPermitData(row.EffectData, roomId)!;
	}

	/// <summary>Read only the envelope and Cell field consumed by PermitWork's native loader.</summary>
	internal static string? RemoveRoomPermitData(string? data, long roomId)
	{
		if (string.IsNullOrWhiteSpace(data)) return data;
		var root = XElement.Parse(data, LoadOptions.PreserveWhitespace);
		var removed = false;
		foreach (var envelope in root.Elements("Effect").Where(x => x.Element("Type")?.Value == "PermitWork").ToArray())
		{
			var definition = envelope.Element("Effect") ?? throw new FormatException("PermitWork/Effect is missing.");
			var cell = long.Parse(definition.Element("Cell")?.Value ?? "0", CultureInfo.InvariantCulture);
			if (cell != roomId) continue;
			if (long.Parse(definition.Element("Property")?.Value ?? "0", CultureInfo.InvariantCulture) != 0 ||
				long.Parse(definition.Element("ControllerId")?.Value ?? "0", CultureInfo.InvariantCulture) != 0)
				throw new InvalidOperationException("A property/controller work permit references the shelter; retain its scope for explicit recovery.");
			envelope.Remove(); removed = true;
		}
		return removed ? root.ToString(SaveOptions.DisableFormatting) : data;
	}
}

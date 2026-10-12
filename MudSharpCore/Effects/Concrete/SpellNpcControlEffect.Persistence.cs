#nullable enable

using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public sealed partial class SpellNpcControlEffect
{
	internal sealed record FearParentCleanup(long CharacterId, long InstanceId, long BodyId, Guid Identity,
		long SpellId, long CasterId, long CasterInstanceId);

	private readonly List<FearParentCleanup> _pendingFearParentCleanup = [];
	private sealed record CachedFearParentCleanup(ICharacter Character, MagicSpellParent Parent);

	internal void QueueOfflineFearParentCleanup(FearParentCleanup cleanup)
	{
		if (!_pendingFearParentCleanup.Contains(cleanup)) _pendingFearParentCleanup.Add(cleanup);
	}

	private bool ApplyQueuedOfflineFearParentCleanup(out List<CachedFearParentCleanup> cachedRemovals)
	{
		cachedRemovals = [];
		foreach (var cleanup in _pendingFearParentCleanup)
		{
			// A cached logout target must lose the same exact parent in memory as in its saved row.
			var representations = Gameworld.Actors.Concat(Gameworld.CachedActors)
				.Where(x => CharacterInstanceIdentityComparer.IdentityId(x) == cleanup.CharacterId)
				.Distinct<ICharacter>(ReferenceEqualityComparer.Instance).ToArray();
			if (representations.Length > 1 || representations.Any(x => Gameworld.Actors.Any(y => ReferenceEquals(x, y)))) return false;
			var cached = representations.SingleOrDefault();
			if (cached is not null && (cached.InstanceId != cleanup.InstanceId || cached.Body?.Id != cleanup.BodyId ||
				!cached.IsPrimaryInstance || !cached.State.HasFlag(CharacterState.Stasis))) return false;
			var character = FMDB.Context.Characters.SingleOrDefault(x => x.Id == cleanup.CharacterId);
			if (character is null || character.IsArchived || character.BodyId != cleanup.BodyId) return false;
			var primaries = FMDB.Context.CharacterInstances.AsNoTracking()
				.Where(x => x.CharacterId == cleanup.CharacterId && x.IsPrimary)
				.Select(x => new { x.Id, x.CharacterId, x.BodyId, x.IsPrimary }).ToArray();
			if (primaries.Length != 1 || !IsExactPrimaryFearTarget(primaries[0].Id, primaries[0].CharacterId,
				primaries[0].BodyId, primaries[0].IsPrimary, cleanup.InstanceId, cleanup.CharacterId, cleanup.BodyId)) return false;
			if (!TryRemoveExactFearParentXml(character.EffectData, cleanup, out var updated, out var changed)) return false;
			var parents = cached?.EffectsOfType<MagicSpellParent>().Where(x => x.Identity == cleanup.Identity).ToArray() ?? [];
			if (parents.Length > 1) return false;
			if (parents.Length == 1)
			{
				var cachedXml = new XElement("Effects", parents[0].SaveToXml(new())).ToString();
				if (!TryRemoveExactFearParentXml(cachedXml, cleanup, out _, out var removed) || !removed) return false;
				// Save/commit may fail. Keep the cache unchanged until the exact row edit commits.
				cachedRemovals.Add(new CachedFearParentCleanup(cached!, parents[0]));
			}
			if (changed) character.EffectData = updated;
		}
		return true;
	}

	private void CompleteQueuedOfflineFearParentCleanup(IEnumerable<CachedFearParentCleanup> cachedRemovals)
	{
		foreach (var removal in cachedRemovals) RemoveCachedOwnedFearParent(removal.Character, removal.Parent);
		_pendingFearParentCleanup.Clear();
	}

	internal static void RemoveCachedOwnedFearParent(ICharacter cached, MagicSpellParent parent)
	{
		var wasChanged = cached.Changed;
		var effectsWereChanged = cached.EffectsChanged;
		var wasQueued = cached.Gameworld.SaveManager.IsQueued(cached);
		cached.RemoveEffect(parent, true);
		// This exact effect is persisted in the mission transaction. Do not introduce a later broad
		// Character.Save that could replace unrelated offline fields from an older cached object.
		if (!wasChanged && !effectsWereChanged && !wasQueued)
		{
			cached.EffectsChanged = false;
			cached.Changed = false;
			cached.Gameworld.SaveManager.Abort(cached);
		}
	}

	internal static bool IsExactPrimaryFearTarget(long rowInstanceId, long rowCharacterId, long rowBodyId, bool primary,
		long expectedInstanceId, long expectedCharacterId, long expectedBodyId) => primary && rowInstanceId > 0 &&
		rowInstanceId == expectedInstanceId && rowCharacterId == expectedCharacterId && rowBodyId == expectedBodyId && expectedBodyId > 0;

	internal static bool TryRemoveExactFearParentXml(string? xml, FearParentCleanup cleanup, out string? updated, out bool changed)
	{
		updated = xml;
		changed = false;
		if (cleanup.CharacterId <= 0 || cleanup.InstanceId <= 0 || cleanup.BodyId <= 0 || cleanup.Identity == Guid.Empty ||
			cleanup.SpellId <= 0 || cleanup.CasterId <= 0 || cleanup.CasterInstanceId <= 0) return false;
		if (string.IsNullOrWhiteSpace(xml)) return true;
		try
		{
			var root = XElement.Parse(xml, LoadOptions.None);
			if (root.Name != "Effects") return false;
			var entries = root.Elements("Effect").ToArray();
			if (entries.Any(x => x.Element("Type") is null || x.Element("Effect") is null)) return false;
			var matching = entries.Where(x => x.Element("Type")!.Value == "MagicSpellParent" &&
				Guid.TryParse(x.Element("Effect")!.Element("Identity")?.Value, out var identity) && identity == cleanup.Identity).ToArray();
			if (matching.Length == 0) return true;
			if (matching.Length != 1) return false;
			var parent = matching[0].Element("Effect")!;
			var children = parent.Element("Children")?.Elements("Effect").ToArray();
			if ((bool?)parent.Element("RemoveWhenLoadedChildrenExpire") != true ||
				(long?)parent.Element("Spell") != cleanup.SpellId || (long?)parent.Element("Caster") != cleanup.CasterId ||
				(long?)parent.Element("CasterInstance") != cleanup.CasterInstanceId || children is not { Length: 1 } ||
				children[0].Element("Type")?.Value != "SpellFear") return false;
			matching[0].Remove();
			updated = root.ToString(SaveOptions.DisableFormatting);
			changed = true;
			return true;
		}
		catch (System.Xml.XmlException)
		{
			return false;
		}
		catch (FormatException)
		{
			return false;
		}
	}
}

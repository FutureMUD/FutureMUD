using System.Text.Json;
using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;

#nullable enable
namespace MudSharp.Magic.Casting;

public static class PatronMagicService
{
	public static ControlledSpellIncantation? Incantation(IMagicCastingCapability capability, IControlledMagicSpell spell) =>
		spell.GradeProfile?.Incantation ?? capability.CastingPolicy?.Patron?.Incantations?.GetValueOrDefault(spell.Id);
	private sealed record Choices(int Version, long[] Spells);
	private static readonly ConcurrentDictionary<long, object> SelectionGuards = new();
	public static bool Allowed(IFuturemud world, long progId, ICharacter actor) =>
		progId > 0 && world.FutureProgs.Get(progId)?.Execute<bool?>(actor) == true;

	public static IReadOnlyList<long> Selections(ICharacter actor, long capabilityId)
	{
		using (new FMDB())
		{
			var row = FMDB.Context.CharacterMagicCapabilityStates.Find(MagicCastingService.Owner(actor).Id, capabilityId);
			if (row is null) return [];
			var state = JsonSerializer.Deserialize<Choices>(row.Definition);
			if (state is not { Version: 1 } || state.Spells.Distinct().Count() != state.Spells.Length) throw new InvalidOperationException("Invalid patron spell selections; staff reconciliation required.");
			return state.Spells;
		}
	}

	public static bool HasSpell(ICharacter actor, IMagicCastingCapability capability, long spellId) =>
		capability.CastingPolicy?.Patron is { } patron && Allowed(actor.Gameworld, patron.AuthorityProgId, actor) &&
		(patron.PermanentSpells.Contains(spellId) || Selections(actor, capability.Id).Contains(spellId));

	public static bool Select(ICharacter actor, ConditionalMagicItemPolicy shrine, IReadOnlyList<long> spells, out string reason, Func<bool>? stillAccessible = null)
	{
		var owner = MagicCastingService.Owner(actor);
		var body = actor.Body;
		var location = actor.Location;
		lock (SelectionGuards.GetOrAdd(owner.Id, _ => new object()))
		{
		reason = "This shrine cannot grant that casting route.";
		if (actor.Gameworld.MagicCapabilities.Get(shrine.CapabilityId) is not IMagicCastingCapability { CastingPolicy: { Enabled: true, Patron: { } patron } } capability ||
			!actor.Capabilities.Any(x => x.Id == capability.Id) || !Allowed(actor.Gameworld, patron.AuthorityProgId, actor) ||
			!Allowed(actor.Gameworld, shrine.EligibilityProgId, actor) ||
			shrine.TerritoryProgId != 0 && !Allowed(actor.Gameworld, shrine.TerritoryProgId, actor)) return false;
		if (!ReferenceEquals(actor.Body, body) || actor.Location != location || !ReferenceEquals(owner, MagicCastingService.Owner(actor)) ||
			stillAccessible?.Invoke() == false) return false;
		if (spells.Count > patron.SelectionLimit || spells.Any(x => patron.PermanentSpells.Contains(x) || !capability.CastingPolicy!.Admissions.Any(a => a.SpellId == x))) return false;
		var current = Selections(actor, capability.Id);
		if (!shrine.AcceptsSelection(spells, current, out reason)) return false;
		using var independent = FMDB.BeginIndependentScope();
		using (new FMDB())
		{
			var row = FMDB.Context.CharacterMagicCapabilityStates.Find(owner.Id, capability.Id);
			if (row is null) { row = new CharacterMagicCapabilityState { CharacterId = owner.Id, MagicCapabilityId = capability.Id }; FMDB.Context.CharacterMagicCapabilityStates.Add(row); }
			row.Definition = JsonSerializer.Serialize(new Choices(1, spells.Order().ToArray()));
			row.StateVersion++;
			FMDB.Context.SaveChanges();
		}
		reason = "Your spell selections have been recorded.";
		return true;
		}
	}

	public static bool InFreeTerritory(ICharacter actor, IMagicCastingCapability capability) =>
		capability.CastingPolicy?.Patron is { FreeWithinTerritory: true } p && Allowed(actor.Gameworld, p.AuthorityProgId, actor) && Allowed(actor.Gameworld, p.TerritoryProgId, actor);

	public static void Refill(ICharacter actor)
	{
		// Body construction recalculates inventory capacity before Character assigns its Body.
		if (actor.Body is null) return;
		foreach (var capability in actor.Capabilities.OfType<IMagicCastingCapability>())
		{
			if (capability.CastingPolicy is not { Enabled: true, Patron: { RefillWithinTerritory: true } p } policy ||
				!Allowed(actor.Gameworld, p.AuthorityProgId, actor) || !Allowed(actor.Gameworld, p.TerritoryProgId, actor) ||
				actor.Gameworld.MagicResources.Get(policy.ReserveResourceId) is not { } resource) continue;
			var owner = MagicCastingService.Owner(actor);
			if (MagicResourceCapacity.TryGetCap(resource, owner, out var cap, out _) && double.IsFinite(cap) &&
				owner.MagicResourceAmounts.GetValueOrDefault(resource) < cap) owner.AddResource(resource, cap - owner.MagicResourceAmounts.GetValueOrDefault(resource));
		}
	}
}

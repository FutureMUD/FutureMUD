# Regional Land gathering

This optional native service shares ecological life between physical rooms in a Zone or Area. It is independent of personal mana, ambient magical resources and the existing zone forage-profile inheritance. One indexed owner supplies the first payment; explicitly configured room sources supply the fallback. A normal action never splits a partial regional point with a local payment.

## Ownership, recovery and persistence

A configured Area wins over a configured Zone. Overlapping Areas use descending priority, then ascending stable Area ID. Every physical room contributes to at most one selected owner. Spatial presentation does not change the owner. Explicit environmental `Disabled` rooms neither contribute nor draw regional life. Missing/invalid selected regional profiles refuse safely and retain stock for repair. A local protection prog or unresolved ecological/payment operation still blocks destructive gathering.

`off` on an Area/Zone blocks regional inheritance while allowing a valid configured room fallback. `inherit` restores parent selection. Neither operation deletes dormant ecological stock. A new actual profile binding starts full exactly once. Capacity growth, membership changes, terrain changes, profile changes, rebindings and restarts never refill an existing pool; capacity reductions clamp stock down. All rooms are indexed before startup clamping.

Recovery runs once per shared owner per online real-hour heartbeat, capped at capacity. There is no offline catch-up. One callback and an index of depleted recoverable pools replace per-room regional timers; capacity is cached and updated on membership/terrain/binding changes. Quotes and payment do not scan the world or regenerate life. Ordinary hourly heartbeat timing is global, so a newly created pool may receive its first recovery at the next world heartbeat rather than a full hour after creation.

Pool revisions invalidate a captured quote if another room spends or the selected owner changes while an action waits. Payment and its receipt checkpoint commit together before ecological/body effects and personal credit. Unresolved shared participants quarantine that owner across all rooms. Recovery/reconciliation never automatically refunds, credits or replays an uncertain action. Deleted world owners leave ecological/payment evidence intact.

## Builder activation

Install the schema migration and optional Armageddon package update first. This creates reusable profiles and private terrain presets, without binding any existing Area, Zone or room, editing native terrains, enrolling players or refilling energy/life.

```text
magic environment regional profiles
magic environment regional profile <profile> show
magic environment regional area <id or quoted name> <profile> [priority]
magic environment regional zone <id or quoted name> <profile>
magic environment regional here
magic environment regional area <id> off
magic environment regional area <id> inherit
```

The Area must already have its intended physical room membership. `area here` is accepted only when the room has exactly one Area. The mixed Armageddon profile contributes zero for unlisted terrains: assign the appropriate **private Armageddon terrain** to opted-in rooms, or deliberately add contributions for existing terrain IDs. This protects existing worlds from accidental broad activation.

```text
magic environment regional profile create "My mixed ecology" 0 0.1
magic environment regional profile <profile> terrain <terrain> 3
magic environment regional profile <profile> terrain <terrain> remove
magic environment regional profile <profile> contribution 0
magic environment regional profile <profile> recovery 0.1
```

For a custom Land method, use its existing capability editor:

```text
gather land <method> regional 1
gather land <method> regional off
```

The method retains one to sixteen explicit local source entries. A positive regional cost enables regional-first payment. Zero (the compatibility default for existing custom methods) keeps the old local-only behaviour. Regional-first methods cannot include mandatory local collateral or vegetation-health prices: author a separate local-only method for those combined costs. Existing local profile protections remain effective; a regional-only room needs no local profile.

## Armageddon presets and approved formulas

Luke approved the formulas on 18 October 2026 and requested shared regional recovery with room fallback. The package installs nine reusable profile families plus a mixed profile. These are a source-informed translation of the supplied historical C source, not a claim about the current external game or a balance retune.

| Family | Life contribution per room | Body price class | Native terrain used as a new-preset template |
| --- | ---: | --- | --- |
| Heavy forest | 5 | fertile, 0.1 | Broadleaf Forest |
| Light forest | 4 | fertile, 0.1 | Woodland |
| Field, grassland or thornland | 3 | fertile, 0.1 | Grasslands |
| Cavern, road or ruins | 2 | cavern, 0.5 | Cavern |
| Hills or mountains | 2 | hills, 0.75 | Hills |
| Sewer | 2 | barren, 1 | Cavern |
| Desert, salt, silt or shallows | 1 | barren, 1 | Sandy Desert |
| Inside or city | 0 | urban, 0.25 | Residence |
| Forbidden land | 0 | forbidden, refuse | Grasslands |

Each family profile assumes its whole selected region has that family. Use the mixed profile for varied terrain. The mixed profile maps only installed owned terrain IDs; unknown native terrains contribute zero until explicitly configured. Physical presets clone the native template's terrain behaviour/atmosphere and ordinary movement settings into new rows; they are never made the global default. Missing or ambiguous native template terrain leaves reusable profiles available and reports that no physical preset was fabricated. The templates are ecological families, not a catalogue of separate physical variants of every road, mountain, water depth or city room.

For a physically distinct variant such as a road, ruin or water depth, keep its native terrain behaviour and deliberately configure that terrain's contribution and appropriate local profile/price tag. A shared ecological family does not require replacing its physical terrain with the representative template.

Shared recovery is **0.1 life per online real hour per owner**. One action spends **one life**, independent of requested personal mana. Source body/terrain, ecology, ash, capacity and paid-practice formulas remain unchanged. Ash expiry does not replenish life. See [the source translation](Armageddon_Gathering_Source_Translation_20261016.md) for the distinct source and native clock choices.

Sorcerer has `self`, `land` and `wildland`; Preserver has `self`; Defiler has `land` and `wildland`. Both Land-kind methods try the shared pool first. `land` retains the existing crop/woodland/pasture fallback. `wildland` uses the new exact native forage key `armageddon-life`, authorised by the corresponding private local environmental profile and forage profile. Each fallback starts with its terrain-family life capacity under normal native room-yield initialisation; its seeded hourly recovery is **zero**. Thus shared recovery is not multiplied by the number of rooms. Builders may deliberately author another local recovery policy.

Native forage-profile precedence is explicit room, then Zone, then terrain. If a Zone already supplies a forage profile, explicitly select the approved forage profile on each intended fallback room; do not overwrite the Zone's existing profile to make terrain inheritance win. Native local forage keeps its existing lifecycle/scheduling; the new once-per-owner callback governs regional life. No comparative performance benchmark is claimed.

For fresh installs with Classes enabled, Luke explicitly chose to offer Defiler immediately alongside the other Classes. Its usable Land remains a builder-owned world configuration. This resolves the earlier PR803 admission-policy question without changing the shipped default.

## Upgrade and rollback

Back up the game before applying the normal schema upgrade/package rerun. The migration adds empty tables and a shared-source participant index; it performs no world activation or stock backfill. Three-way seeder reconciliation preserves builder changes, retained identities and unrelated forage keys/outcomes. A rerun does not delete or reset pool rows. Review reported conflicts before activation. Existing custom methods lacking `regionalUnits` stay local-only.

To pause regional use, set the relevant owner `off`, or set the method's regional cost `off`; existing local fallback remains subject to its own permissions. Use room environmental `Disabled` to exclude a room entirely. Keep pool and receipt evidence for later review. Re-enabling a previously used owner does not refill it.

An older binary cannot interpret regional receipt details. Do not downgrade a live database after regional payments by dropping these tables or editing receipts. Stop the server and restore a matching pre-upgrade database/binary backup if a complete rollback is required. Migration `Down` is for disposable/pre-use rollback and destroys regional stock; it is not a player refund procedure.

The dated PR803 readiness and native receipts retain their original local-source scope. They do not certify this added service. Regional tests and a distinct first/cold Release receipt must be reported separately. The known injected Borrow DELETE failure remains **FAIL**, recovery **NOT_RUN**, deferred and nonblocking for ordinary use; this work does not fix it.

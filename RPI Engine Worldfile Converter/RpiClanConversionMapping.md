# RPI Clan Conversion Mapping

This document describes how the `RPI Engine Worldfile Converter` imports RPI Engine clans into live FutureMUD `Clan` records.

## Source Inputs

- `Old SOI Code/src/clan.cpp`
  - Header table rows provide authoritative full names and group ids where present.
  - `get_clan_rank_name(CHAR_DATA*, char*, int)` provides alias-specific rank labels.
  - `clan_flags_to_value(char*, char*)` provides alias-specific textual synonyms for rank slots.
- `soiregions-main`
  - `mobs.*`, `rooms.*`, and `crafts.txt` are scanned for structured clan/rank references.
  - Parsed item clan trailers are also folded into the reference index when available.

## Imported Scope

This pass imports live clans directly through the converter and does not create clan templates.

It deliberately ignores:

- forums
- paygrades
- appointments
- external controls
- `CLAN_LEADER`
- member object / leader object flags

## Canonical Alias Rules

The importer prefers the in-world alias used by code and region data as the canonical FutureMUD alias. Legacy aliases are preserved for provenance and idempotency checks.

Canonical mappings:

- `mordor_char` => `Minas Morgul`
  - Legacy alias: `mm_denizens`
- `malred` => `Malred Family`
  - Legacy alias: `housemalred`
- `rogues` => `Rogues' Fellowship`
  - Legacy alias: `rouges`
- `hawk_dove_2` => `Hawk and Dove`
  - Legacy alias: `hawk_and_dove`

Alias-only clans imported even without header-table rows:

- `seekers` => `Seekers`
- `shadow-cult` => `Shadow Cult`
- `tirithguard` => `Minas Tirith Guard`
- `eradan_battalion` => `Eradan Battalion`

Reference-backed clans are also imported when they appear in structured worldfile clan/rank references with at least one importable observed slot, even if they are absent from `clan.cpp`.

Examples:

- profession guilds such as `healers`, `apothecarists`, and `horticulturist`
- civic or organizational aliases that only survive in worldfile references

These inferred clans:

- use the observed alias as the canonical alias unless an explicit normalization rule applies
- use a title-cased alias as the full name unless a better authoritative name is available
- preserve a source warning noting that the clan was synthesized from structured worldfile references

If a clan has no authoritative table name and no explicit normalization rule, the converter falls back to a title-cased alias.

## Rank Lattice

The old RPI slot lattice is mapped onto three logical paths:

- `Common`
  - `Membership`
- `Military`
  - `Recruit`
  - `Private`
  - `Corporal`
  - `Sergeant`
  - `Lieutenant`
  - `Captain`
  - `General`
  - `Commander`
- `Guild`
  - `Apprentice`
  - `Journeyman`
  - `Master`

Ignored slots:

- `Leadership`
- `MemberObject`
- `LeaderObject`

## Rank Import Rules

For each clan path, the importer uses the following rules:

1. If `get_clan_rank_name` defines any custom names on that path, import every slot from the start of that path through the highest custom slot.
2. Keep generic slots below that highest custom slot even if they were not explicitly renamed.
3. Drop higher slots on that path even if the raw region data references them.
4. If the path has no custom names at all, import only the slots actually observed in region references.

This preserves the RPI behavior where missing higher custom names implied that those higher ranks were not used for that clan.

For reference-backed inferred clans with no `clan.cpp` source metadata, this rule is what drives the entire rank shape. In practice that means:

- craft guild aliases with observed `Apprentice`/`Journeyman`/`Master` references import as guild-path clans
- aliases only observed with `Membership` import as membership-only clans
- aliases only observed on military slots import as military-path clans using generic slot names

Mixed-path clans are supported. In practice this matters especially for:

- `khagdu`
- `mordor_char`

## Rank Naming

For each imported slot:

- The primary FutureMUD rank title is the first non-generic custom label found for that slot.
- Any additional labels from `get_clan_rank_name` are preserved as alternate names in the export payload.
- Synonyms from `clan_flags_to_value` are preserved in the export payload for audit/reference purposes.
- If no custom label exists, the generic slot name is used.

## Privilege Mapping

Every imported rank receives:

- `CanViewClanStructure`
- `CanViewClanOfficeHolders`
- `CanViewMembers`

Military ranks from `Corporal` upward also receive:

- `CanInduct`
- `CanPromote`
- `CanDemote`

The highest imported rank on each authoritative non-common path receives:

- `ClanPrivilegeType.All`

If a clan has no military or guild path at all, the converter falls back to the highest imported remaining path.

## Reference Scanning Rules

The region scanner intentionally only trusts structured clan references:

- quoted rank/alias pairs in mob or craft clan trailers
- `clanrank(alias, rank)` checks
- line-start `clan <alias> <rank>` and `clan set <alias> <rank>` commands
- `clan(-1, alias)` style presence checks
- `clan_echo zone <alias>` calls

Free-text prose is ignored even if it contains words like `clan`, quoted text, or rank-looking tokens.

## Apply-Clans Behavior

`apply-clans` requires a seeded FutureMUD baseline because it creates live `Clan` and `Rank` rows directly.

Dry-run is the default behavior. Actual writes only occur when `--execute` is supplied.

The importer is idempotent by alias:

- if the canonical alias already exists, the clan is skipped
- if a preserved legacy alias already exists, the clan is skipped with a warning

## Unresolved Aliases

The converter reports unresolved aliases seen in region data but not imported in this pass.

This includes intentionally unimported references such as:

- `tirithguard_3`
- `tirithguard_11`
- `com-priests`

Presence-only references with no importable observed rank slot remain unresolved by design unless they are explicitly covered by a normalization rule or a future pass promotes them to first-class imported clans.

The unresolved list is intended as an audit queue for follow-up passes rather than a hard failure condition.

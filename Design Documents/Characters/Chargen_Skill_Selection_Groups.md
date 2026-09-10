# Chargen Skill Selection Groups

Native-language identity, optional heritage defaults, accent roles, acquisition selection, and mastery rules are documented in [Native Languages and Accents](../Communication/Native_Languages_and_Accents.md). The language learner-accent pointer and `language.defaultaccent` FutureProg property have been removed; seeded content uses accent roles and source-language associations. Existing skill grants remain unchanged.

Groups are generic entitlements to choose real installed skill traits. They are independent of trait categories, languages, cultures and storyboard installation order. No groups are installed or enabled automatically.

## Builder workflow

Use `chargenskillgroup` with senior administrator permission. Names with spaces must be quoted when followed by another argument. IDs and exact names must resolve uniquely.

```text
chargenskillgroup create "Household Training"
chargenskillgroup set description
chargenskillgroup set member add <hearthcraft-skill-id>
chargenskillgroup set member add <mending-skill-id>
chargenskillgroup set member add <yardwork-skill-id>
chargenskillgroup set picks 2
chargenskillgroup set eligibility <compiled-boolean-chargen-prog>
chargenskillgroup set existing newonly
chargenskillgroup set order 100
chargenskillgroup validate "Household Training"
chargenskillgroup set enabled yes
```

The description editor accepts in-world prose, for example: “The household teaches its children to tend the hearth, mend useful things and help with the work of the yard.” The skill names above are fixture examples, not assumed stock content. Resolve the actual installed IDs first. The applicant chooses exactly two of these three non-language skills.

Other commands:

| Command | Purpose |
|---|---|
| `list [filter]` | Inspect bounds, enabled state, membership count, eligibility and validation |
| `show <id|name>` | Inspect identity, revision, description and ordered membership |
| `edit <id|name>` | Select a group for subsequent setters |
| `clone <id|name> <new-name>` | Copy to a new stable identity, disabled |
| `set name <name>` | Rename without changing identity |
| `set picks <minimum> <maximum>` | Set ranged bounds; `picks 0 2` offers up to two |
| `set member remove <skill>` | Remove membership and revalidate pending choices |
| `set membereligibility <prog|none>` | Optional compiled Boolean(chargen, trait) filter |
| `set enabled [yes|no]` | Toggle or explicitly set activation; activation validates |
| `retire <id|name>` | Confirm soft retirement, preserving references |
| `validate <id|name|all>` | Check definitions; applicant-specific prog outcomes still need runtime validation |

Attributes, unresolved members and duplicate memberships are rejected. `eligibility` requires a compiled Boolean(chargen) prog; an unset prog never means universal access. An explicit compatible always-true prog provides universal access.

## Applicant workflow and accounting

`SkillGroupScreen` is shared by `SkillPicker`, `SkillCostPicker`, `SkillSkipper` and `SkillBoostSkipper`. Mandatory skills precede group decisions. Suggestions and open selection follow group decisions; skippers skip only the open selection portion. The existing no-group path remains available unchanged.

During groups: `groups`, `group <name|number>`, `pick <full skill name|id>`, `unpick <full skill name|id>`, `help <skill>`, `done`, `skip`, and `back`. An optional group still needs an explicit decision. Group choices are protected from removal in the ordinary picker; use `groups` to reopen the allocation.

`NewOnly` excludes mandatory/independent skills. `CountKnown` permits explicit credits for eligible known skills. A skill can occupy only one group slot. `SkillGroupAllocation.IsFeasible` uses augmenting-path matching for every outstanding required slot, including capacities greater than one. Optional choices cannot strand another group's required minimum. Shortages block progress; counts are never reduced and unused optional capacity never becomes ordinary capacity.

`ChargenSkillClaims` records mandatory grants, recoverable ordinary claims and each group's selected traits, known credits, completion, ID, stable key and revision. Existing `SelectedSkillBoosts` retains boost intent. `SelectedSkills` is projected from acquisition claims at the shared controller boundary. Group ownership is not inferred from an ordinary skill merely appearing in a candidate pool.

An explicit group allocation of an ordinary skill suppresses its base purchase charge/pick usage. Unpicking or invalidating that group allocation restores ordinary ownership and cost. Repricing is derived from claims, not incremental refunds. Trait resource requirements and individual resource charges retain their existing rules. Boost pricing remains separate and applies once to each earned skill.

New skills use the existing culture starting-value prog, caps and boost rules. Independent stronger values remain authoritative. Review displays identify mandatory, group and ordinary ownership. Later award adapters can consume group/trait provenance without imposing a language-specific value today.

Pending applications save claims in their normal XML. Reopening or changing background/definitions re-evaluates a detached pre-group context, excluding pending ordinary, suggested and group choices. Invalid derived claims are removed; ordinary ownership remains recoverable. Definition revisions require reviewing retained choices. Broken active progs fail closed. Approved characters are not retroactively stripped.

## Persistence and seeder boundary

Migration `20260907085537_ChargenSkillSelectionGroups` adds definitions and ordered membership, unique stable keys and composite membership keys. Restrictive foreign keys preserve referenced traits/progs/groups. Definitions are loaded with chargen after progs and traits, independent of storyboard creation order.

`DatabaseSeeder.Seeders.Utilities.Chargen.ChargenSkillSelectionGroupSeeder.Upsert` accepts `SkillGroupSeedDefinition`, installed trait models and already-resolved compiled prog references. It checks references and bounds. A stable key owns only its managed group; a matching display name confers no ownership. `SeedBaseline` stores the preceding seeded fields and per-member order so three-way reconciliation preserves builder prose, counts, progs, additions, removals and ordering, reporting conflicts.

`CultureToolkitGroupSeeder` resolves the CultureSeeder handoff's selected-era identities, compiles its eligibility progs and calls this helper for the 28 optional educational/contact recipes. That consumer does not replace the resolver, screens, claims, cost accounting or NPC adapter described here. Its implementation and verification are tracked in [CultureSeeder Redesign Implementation](../Verification/CultureSeeder_Redesign_Implementation.md). The dependency's own seed/runtime fixtures continue to use non-language household crafts and opt-in definitions.

## Generated templates

Variable NPC templates can explicitly use `npc set skillgroups <integer seed> <decline|fill>` (within the normal template editing workflow); `skillgroups off` disables this path. Existing templates default off. `GeneratedSkillGroupAdapter.Apply` uses the same resolver, ownership rules and feasibility checks, with a seed-based deterministic candidate order and explicit optional policy. Choices are retained in template XML. Hand-authored skill values remain authoritative; display does not generate new choices. Invalid required allocations fail rather than silently granting alternatives.

Opt-in generation also invokes the mandatory free-skill prog configured on the current skill storyboard, using the same four-screen lookup as chargen revalidation. Newly granted skills run through the culture's starting-value prog and stock cap. Newly granted languages receive the existing learner accent when eligible, otherwise the first eligible accent by stable ID; explicitly selected accents are retained. This permits a seeded native-language policy to exclude foreign/learner accents without adding an NPC-only culture rule.

## Verification

Tests named SG01–SG20 cover allocator capacity/overlap, compiled and erroneous progs, all four screen adapters, claim serialization and reversal, actual point/boost pricing, builder persistence, generic seeder order/preservation, and deterministic generation. The shared-library allocator test compares all 512 three-skill candidate-set combinations against exhaustive assignment. Database model tests check identity, membership keys and restrictive relationships.

The UI uses the existing ANSI, full-name wrapping and output encoding paths. No Latin-1 encoder changes are part of this feature.

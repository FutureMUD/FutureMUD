# NPC Skill Packages

## Purpose

NPC skill packages are persistent, non-revisioned builder items that collect the routine skill distributions needed by related NPCs. Applying a package is a one-time copy operation. The NPC template or character does not retain package provenance, and later package edits do not update anything to which the package was previously applied.

Each entry identifies one skill trait and stores:

- a chance from 0% to 100%;
- an arithmetic mean greater than or equal to zero;
- a standard deviation greater than or equal to zero; and
- skewness from -0.99 to 0.99.

The skew-normal sampler normalises its raw distribution so that the configured mean and standard deviation remain the distribution's actual mean and standard deviation. Zero skewness is an ordinary normal distribution.

## Package Authoring

The `npcskillpackage` command uses the generic editable-item workflow:

```
npcskillpackage list
npcskillpackage show <package>
npcskillpackage edit <package>
npcskillpackage new <name>
npcskillpackage clone <package> <new name>
npcskillpackage delete [<package>]
npcskillpackage set name <name>
npcskillpackage set skill <skill> <chance%> <mean> <standard deviation> [<skewness>]
npcskillpackage set skill <skill> 0%
```

Package names are globally unique without regard to case and cannot be blank or entirely numeric, because numeric builder input is reserved for IDs. A zero chance removes the skill entry. Omitting skewness uses zero. The package display includes chance, mean, standard deviation, skewness, and weighted expected value (`chance x mean`).

Deletion requires confirmation through the standard `ACCEPT` workflow. Once confirmed, it cascades to the package's skill rows and race links. Trait definitions remain protected by a restrictive foreign key while referenced by a package.

## NPC Template Application

Use `npc set skillpackage <package>` while editing either kind of NPC template.

Simple templates independently roll each entry's chance. Successful entries sample a value, clamp a negative result to zero, and add or raise the template's stored skill. An application never lowers a stored skill. The command reports additions, raises, skips, and failed chance rolls.

Variable templates copy the package distribution rather than resolving it. If the template already contains the skill, replacement occurs only when the package's weighted expected value is strictly greater:

```
weighted expected value = chance x arithmetic mean
```

An equal expected value preserves the existing entry. A replacement copies chance, mean, standard deviation, and skewness together. Variable-template XML writes the `Skewness` attribute; older XML without that attribute loads with zero skewness.

The package application changes a template only when at least one stored skill changes. Applying overlapping packages is therefore deterministic for variable templates and raise-only for simple templates, apart from the simple template's chance and value rolls.

Language skills are permitted, but package application does not invent an accent. Existing NPC-template submission validation still requires the builder to configure any required accent.

## Race Defaults

Use `race set skillpackage <package>` to toggle a direct default on the race. Race defaults inherit from parent to child. Cloning a race copies only that race's direct selections; inherited selections continue to come from its parent.

Whenever the NPC builder assigns or changes a template's race, all inherited and direct race packages are applied. This is additive. Changing the race does not remove skills supplied by the earlier race, repeated assignment follows the ordinary no-lower or weighted-replacement rules, and no package provenance is recorded.

## FutureProg

`NPCSkillPackage` is a FutureProg type with `id`, `name`, and `skills` properties. Packages can be resolved with numeric or text lookup overloads:

```
npcskillpackage(123)
npcskillpackage("Universal Common")
```

Use `applyskillpackage(character, package)` to roll a package for a live character. The function adds missing skills, raises existing skills only when the sampled result is higher, and returns the number of skills added or raised. A null package returns zero.

## Seeder Ownership

The existing `Skill Package` seeder owns `Universal Common` and resolves the installed simple, complex, or RPI utility-skill equivalents. The Combat seeder owns the five beast-attacker definitions. Animal, Mythical Animal, and Supernatural seeders own capability definitions and additive links from their stock races.

Stock definitions are upserted by their reserved names. Seeder reruns repair their entries and add missing required race links. They do not delete custom packages or unrelated race links.

The stock catalogue is:

- `Universal Common`: utility traits at 100%, mean 25, deviation 5, skew zero;
- `Flying Race`, `Swimming Race`, and `Climbing Race`: the installed capability trait at 100%, mean 35, deviation 7.5, skew zero;
- `Low-Level Beast Attacker`: mean 25;
- `Competent Beast Attacker`: mean 45;
- `Dangerous Beast Attacker`: mean 65;
- `Terrifying Beast Attacker`: mean 75; and
- `Apex Beast Attacker`: mean 85.

Beast packages contain the installed brawling/natural-attack and dodge/defence traits at 100% chance and zero skew. Their deviation is the greater of 5 or 10% of the mean. Non-human combat-balance tiers select the appropriate package; explicit high-threat exceptions such as wargs and tier metadata for dragons retain the intended 75/85 placement.

## Deliberate V1 Limitations

Packages contain skill traits only. They do not contain attributes, knowledges, merits, languages' required accents, or other NPC data. There is no application history, removal workflow, rollback, live link, or retroactive reconciliation. Builders must reapply a changed package manually when they want its new definition copied elsewhere.

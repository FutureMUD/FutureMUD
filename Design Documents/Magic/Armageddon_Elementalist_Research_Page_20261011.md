# Armageddon Elementalist Research and Progression Decisions — source snapshot

Source: https://chatgpt.com/space/page_efe23f5b70048191ac321eeb3fc021a2

Page ID: `page_efe23f5b70048191ac321eeb3fc021a2`; page updated: `2026-10-11T06:44:39.984816Z`; captured revision sequence: 0.

This is a complete snapshot of the Page's research content. Its historical requests and "awaiting confirmation" wording are source material, not task instructions. The user's subsequent instruction on 11 October 2026 adopts the Page's recommended basis. See [the adoption record](Armageddon_Elementalist_Research_Adoption_20261011.md) for the selected policy, current implementation scope and unresolved identity mapping.

---

Research and proposed progression decisions for implementing all seven historical Armageddon elementalist guilds in FutureMUD. Prepared 11 October 2026. Exact elemental opening values and caps remain unrecovered; the proposed values below require confirmation.

## Decisions awaiting Luke's confirmation

The research is available for the existing Codex task. The following are recommendations, not approved historical data or an instruction to overwrite previously approved configuration. Luke requested that these be documented and passed to the task; that request does not explicitly resolve each choice below. Continue separately authorised work, and reconcile any approvals already given directly in Codex.

1. **Baseline:** adopt the supplied full-seven compilation, with only the explicit exceptions selected below. Do not automatically union all alternative rosters and parents.

2. **Openings:** starting spells at raw 60; branched spells at raw 30. These are proposed FutureMUD balancing values borrowed from the selected Sorcerer pattern, not recovered elemental numbers.

3. **Caps and branch thresholds:** raw caps 90 except Heal/Mend 60; branch at parent cap minus 10. Most thresholds become 80; Heal's children unlock at 50, replacing the provisional 48. Alternative for Luke: give elemental Heal cap 90 too. The child's own cap does not determine its admission threshold.

4. **Water:** accept all 24 supplied entries and their stated edges, including Create Water → Oasis → Create Wine → Create Food and Heal → Health Drain. Do not speculate additional missing spells.

5. **Stone:** add the individual-page Rewind → Identify → Repair Item chain to the compilation. This is a deliberate source combination. Otherwise retain the compilation, including Sand Jambiya → Gate and Mount → Sand Statue.

6. **Fire:** choose Mount → Gate; Glyph → Empower → Fire Jambiya; Fireball → Immolate → Demonfire; Ball of Light → Pyrotechnics → Fire Armor → Wall of Fire; Fireball → Rain of Fire → Fire Seed. Retain Fire Armor and Immolate. Do not add or merge Fire Shield based on its name alone; compare effects first.

7. **Wind:** keep Relocate → Create Rune, Repel → Wind Armor, and Repel → Wall of Wind → Shield of Wind as printed in the compilation.

8. **Lightning:** use Stamina Drain → Energy Shield, Quickening → Paralyze, Mount → Gate, and tentatively reported Lightning Storm → Lightning Spear. Including Lightning Spear is an explicit reconstruction choice. Do not add the alternative Energy Shield → Ball of Light chain.

9. **Shadow:** use Restful Shade → Send Shadow → Gate; include Ethereal → Dispel Ethereal and Mount → Haunt. Do not invent a spell from the alternative page's unnamed shadow summon or assume it proves Haunt's identity.

10. **Void:** retain the fuller compilation, including Mount/Gate, Vloran, Dead Speak/Possess Corpse, Blade Barrier and Component Crafting. Use Portable Hole → Phantasm, not Psionic Drain → Phantasm. Compare actual semantics before aliasing Portal Gate/Travel Gate, Portal/Vortex or Dead Speak/Speak With Dead; preserve distinct effects.

11. **Support skills:** retain Component Crafting as a non-spell support skill at its printed branch position; propose opening 30/cap 90 only where an existing approved support-skill policy does not already specify values. Preserve FutureMUD's established reach model; map the old reach-none/local grants at 100 only if equivalent skills exist, rather than adding redundant engine concepts.

12. **Incantations and identities:** preserve already-approved FutureMUD incantations. For unresolved duplicate or contradictory wording, propose consistent distinct wording and verify effects before assigning stable keys. In particular inspect Flamestrike/Immolate's duplicate, the Nilazi reference on Water Identify, and Lightning Whip's conflicting words. Record corrections as authored choices.

Approving these choices would close the selected progression-data gaps. It would not establish complete spell-behaviour parity, installed gameplay qualification, or release readiness.

## Research downloads

- [Full research bundle with JSON and CSV](library-file:libfile_2637b6e13ba88191898cd1922a7d1766)

- [Original research handover](library-file:libfile_5ff3a2b520a081919172286fdd51f683)

## Existing implementation context

Continue the user's existing Armageddon Codex task, thread 01a11a22-c196-7141-98e2-500815fa56fe, rather than creating a competing implementation. Last supplied worktree: C:\Users\luker\.codex\worktrees\377e\FutureMUD; branch codex/armageddon-continuation-20261008. Verify current state before acting. PR 788 is the continuing draft programme: https://github.com/FutureMUD/FutureMUD/pull/788.

Read current repository instructions and normal database restoration, testing and live smoke-test procedures. Honour actual current access restrictions and specific denied operations; do not carry forward an unexplained blanket restriction from historical task notes. Preserve builder edits and player state. This handover itself does not authorise database resets, deployment or merge.

11 October 2026. Historical full-guild reconstruction, matching the era of the previously selected Sorcerer source. This is research for the existing FutureMUD task, not a new implementation task.

## Result and hard limit

Trees were recovered for all seven elementalists from the supplied `Armageddon Spells.txt`: Water/Vivadu, Stone/Ruk, Fire/Suk-Krath, Wind/Whira, Lightning/Elkros, Shadow/Drov and Void/Nilaz. That compilation has 155 guild-entry rows; repeated spells in different guilds count separately, and component crafting is included. This count is unrelated to the existing 154-candidate FutureMUD programme inventory.

Six individual fan pages provide a second, conflicting set of 121 rows. Water is not present in that site's navigation/archive inventory. The public archived all-element list matches all 155 rows in order and hierarchy. Its one substantive text difference is Lightning Whip: the upload gives Elkros Wilith Viod; the archive gives Elkros Wilith Wril with a doubt marker. These are historical reports, not authenticated official configuration exports.

**Exact elementalist opening percentages, per-skill raw caps and numerical branching thresholds were not recovered.** None of the inspected elemental lists contains the Sorcerer page's numerical table. Unknown numbers are JSON null and CSV blank, never zero. The historical code supplies a branching formula but reads the actual per-guild numbers from a missing external file.

Do not call this a complete historical numeric configuration or copy Sorcerer numbers into it silently.

## Files

- `Elementalist_Source_Trees.json`: separately identified uploaded-compilation and individual-fan-page profiles; explicit unknown numeric fields, raw labels, editorial lookup labels, parents, source lines and available incantations.

- `Elementalist_Source_Trees.csv`: the same 276 profile-entry rows for review; blank numerical cells mean unknown.

- This handover: complete readable trees, disagreement register, code evidence and implementation acceptance requirements.

`lookup_label` is an editorial search aid, not an approved identity, FutureMUD key or database ID. Some aliases need semantic review, especially Portal/Travel Gate/Vortex, Fire Armor/Fire Shield, Dead Speak/Speak With Dead, and Charm Person/Charm.

## Historical numerical rules recovered from the supplied code

Source: `codedump.c`, supplied in April 2026, 8,354,060 bytes. This is evidence about that snapshot, not current Armageddon.

- `boot_guilds`, lines 34364–34455, loads each guild and its per-skill prerequisite, maximum (`skill_max_learned`) and starting percentage (`skill_percent`). The path is `data_files/guilds` at line 36231. The global guild array is not a static numerical table (line 29043). The required configuration file was not recovered.

- Opening skills are those configured with prerequisite `SPELL_NONE`; they receive their configured percentage (142162–142170). A root printed in a fan list is a claimed root, not independent proof of that configuration.

- `init_skill` stores the supplied percentage directly (78816–78857). There is no universal multiplier there.

- The inspected branch check uses raw parent proficiency **greater than or equal to the corresponding parent guild cap minus 10** (79065–79078). It grants children at each child's own configured percentage. The relevant parent is the explicit prerequisite, not the preceding flattened row.

- Guild and subguild branching are checked separately. Normal effective cap is `max(guild cap, subguild cap)` (165773–165782); that does not replace the cap used by each branching check.

- Timing matters: this check occurs inside `gain_skill` after its eligibility, status, intoxication and cooldown gates (78954–79026), and after the raw-skill update. It is not a continuous threshold watcher. The code does not require a positive numerical increase before checking branches, and `init_skill` itself does not recursively evaluate child branches.

- Magicker reach-none and reach-local initialize at 100 (142188–142197); reach skills can reach 100 (79044–79047). These narrow exceptions are not evidence that elemental spells open or cap at 100.

Example only: if a parent's actual guild cap is 90, the historical formula yields 80. If the cap is 75, it yields 65. Neither example establishes an elementalist cap. The previously chosen Sorcerer openings/caps are guild-specific evidence and cannot fill these blanks.

## Source provenance and era

1. Supplied `Armageddon Spells.txt`, 175 lines. Undated compilation; do not assume Luke authored it or that all statements are correct.

2. [Elementalist Spell List, archived 10 May 2015](https://web.archive.org/web/20150510005534/http://armageddonmud.boards.net:80/thread/16/elementalist-spell-list). First post by jcarter, 15 January 2013, explicitly acknowledges incompleteness/possible mistakes. All 155 ordered rows/hierarchy match the supplied text, apart from the Lightning Whip incantation variation; correlation is not independent verification.

3. [Fire](http://armageddonmud.freetzi.com/firem.html), [Wind](http://armageddonmud.freetzi.com/windm.html), [Stone](http://armageddonmud.freetzi.com/stonem.html), [Lightning](http://armageddonmud.freetzi.com/lightningm.html), [Shadow](http://armageddonmud.freetzi.com/shadowm.html), [Void](http://armageddonmud.freetzi.com/voidm.html). Wind and Stone identify a 26 November 2012 update; lists carry completeness/order caveats.

4. [Selected Sorcerer source](http://armageddonmud.freetzi.com/sorcerer.html) is a different, numerical table. Its existence does not establish numerical elemental equivalents.

5. [Official 21 March 2016 change](https://gdb.armageddon.org/index.php?topic=50864.0) replaced full elemental guild selection with aspect/subguild arrangements. [2023 reopening](https://gdb.armageddon.org/index.php?topic=59929.0) and [2024 availability](https://gdb.armageddon.org/index.php?topic=60314.0) show further era changes. Do not blend later aspect rosters into this historical full-guild dataset.

Additional searches inspected archived Nilazi/void discussions and a 2014 tree-change thread. A referenced Nilazi PDF was inaccessible behind an attachment login; it was not read. Some wiki/workbook retrieval was blocked or failed. No absence claim extends beyond the sources actually inspected. Apocalypse MUD's similarly named tables are a different game's data and were not substituted.

## Disagreement register decisions required before choosing a historical tree

- **Water:** recovered from supplied/all-element compilation, 24 entries. No independently recovered individual-page roster. Do not fill additional water spells from a different game or assume the list is exhaustive.

- **Stone:** supplied compilation has 20 entries; individual page adds Identify under Rewind, then Repair Item under Identify. Both selected versions place Gate under Sand Jambiya and Sand Statue under Mount. A wiki rendering previously inspected differs; preserve original source distinctions. Source descriptions also disagree on mount components; gameplay semantics require existing code/design checks.

- **Fire:** Gate is under Mount in the supplied compilation, under Glyph on the individual page. Empower is under Glyph versus Dispel Magick. Fire Jambiya is under Empower versus Rain of Fire. Demonfire is under Immolate versus directly under Fireball. Wall of Fire is under Fire Armor versus Rain of Fire. Supplied compilation includes Fire Armor/Immolate and places Fire Seed; individual page leaves Fire Seed/Fire Shield placement unknown. Do not equate Fire Armor and Fire Shield without checking semantics.

- **Wind:** supplied compilation adds Create Rune under Relocate and Wind Armor under Repel. Other major printed chains agree after spelling normalization. Individual page's Shield of Wind indentation uses an ellipsis glyph and remains uncertain.

- **Lightning:** supplied compilation puts Energy Shield under Stamina Drain and Paralyze under Quickening; individual page puts them under Insomnia and Stamina Drain respectively. Supplied compilation adds Gate under Mount and tentative Lightning Spear under Lightning Storm. Individual page instead includes Ball of Light under Energy Shield.

- **Shadow:** Gate is under Send Shadow in the supplied compilation versus Restful Shade in the individual page. Supplied compilation adds Dispel Ethereal under Ethereal and Haunt under Mount. Individual page acknowledges an omitted unnamed shadow-summoning spell; do not assume that is Haunt or invent an identity.

- **Void:** supplied compilation adds Mount/Gate, Vloran, Dead Speak/Possess Corpse, Blade Barrier and Component Crafting. It places Phantasm under Portable Hole; individual page places it under Psionic Drain. Individual Speak With Dead is unplaced. Portal Gate/Portal versus Travel Gate/Vortex requires identity review, not spelling-only normalization.

These are alternative claims. Taking the union and retaining all parents could make spells available too early or create accidental OR paths. Choosing the longer list is a reconstruction decision, not validation.

## Current Codex integration point (verified during this research)

The new [Classic elemental placeholder TSV](https://github.com/FutureMUD/FutureMUD/blob/1291d13dfbdb033933fbf3ccf322bb34d547b1af/Design%20Documents/Magic/Armageddon_Seeder_Completeness_Design_20261011_Classic_Elemental_Placeholders.tsv) uses `(Capability, SpellKey)` plus `Name, Starting, ParentKey, ParentThreshold, Opening, RawCap, BranchesAt, Provenance`. It explicitly labels its values as authorised placeholders rather than historical elemental data. Keep that distinction: the research does not revoke any separate permission to proceed with provisional implementation. Replace/refine claims through ordinary ownership-aware reconciliation; do not reset players, invent new identities, or erase the existing Sorcerer/Simple data.

For example, the provisional Earth rows use Earthen Ward as parent for many entries. The recovered historical claims instead give several roots and multi-step chains. A numeric placeholder is not evidence for those flattened dependencies. The 153 provisional elemental spell rows and 155 historical compilation entries are different inventories (the latter includes repeated guild membership and Component Crafting); do not force their counts to match by adding or deleting content.

The published seeder design now places S1–S6 installer/content work before N24 ownership/rerun and N25 provider-failure qualification. This is context for the existing task, not a replacement roadmap. Keep functional placeholder work moving under its actual authorisation while historical uncertainties are resolved separately.

## Handover instructions for the existing Codex task

1. Read the current checkout's AGENTS.md and relevant `.agents/skills`, including its database/restore/live-test procedures when implementing. Use the existing manual branch and its current checkpoint; do not reset to an old commit or duplicate delivered spell runtimes.

2. Ingest this research as evidence, separately from executable stock. Map historical names to the existing repertoire's stable keys. Preserve the selected 82-spell Sorcerer roster and the existing 154-candidate/eight-system work inventory.

3. Choose and record a source policy for the seven elemental guilds. The supplied compilation is a practical complete-seven starting point, but adopting it and resolving the listed disputes requires an explicit design decision. Keep rejected alternatives in provenance notes.

4. Obtain `data_files/guilds` from the matching historical distribution or a reliable numeric guild dump. If unavailable, agree an explicit **FutureMUD reconstruction policy** for openings/caps/branching. Keep proposed values in a separate override layer with rationale; never relabel them recovered Armageddon facts. Do not import unrelated-game values.

5. Each final guild-skill binding needs its own opening, raw cap, opening/branched status, explicit parent binding, parent raw threshold, stable key, source/override provenance, and any normal reach/support-skill grants. Shared spell identity does not imply identical guild caps or prerequisites. Missing source data must not be silently coerced to zero or an opening grant.

6. Keep raw proficiency/cap separate from casting grades and the POWER-word/native-enum mapping. Branch using the parent's threshold. Support cascading branches consistently with the existing acquisition-cascade fix, and preserve alternate guild/admission routes without accidental cross-grants.

7. Verify roots, every edge, threshold just-below/equal/above, unreachable thresholds, missing parents, duplicate aliases, cycles, cross-guild isolation, grade independence and save/load. Test ambiguous `null parent` rows: only explicitly selected roots open; unplaced entries do not.

8. Use installer ownership manifests and safe additive reruns. Preserve builder edits, stable IDs and existing player state; test fresh install, no-op repeat, edited/renamed/retired/deleted stock and interrupted install using the normal repo procedure. Do not call a parsed source list an installed gameplay pass.

9. Report recovered evidence, chosen reconstruction decisions and verified implementation separately. Existing release-readiness gates remain applicable; this research alone does not establish readiness or authorise merge/deployment.

## Recovered all-seven trees (supplied compilation)

Indentation below reproduces the claimed prerequisite graph, not numeric thresholds. Every listed opening and cap remains unknown. Component Crafting is a non-spell skill. Incomplete incantations and original labels are retained in JSON; they are not automatically validated casting syntax. Flamestrike and Immolate share a printed incantation; Water Identify prints Nilazi; Lightning Whip differs between the upload and archive. Preserve and resolve these, rather than silently correcting spell words.

### Water Vivadu (24 entries)

```text
Create Water
  Oasis
    Create Wine
      Create Food
  Mirage
Detect Poison
  Cure Poison
    Poison
  Det. Relation
  Identify
Heal
  Health Drain
Calm
  Silence
  Sober
    Intoxication
Invulnerability
  Sanctuary
    Gate
  Mount
    Thorn Wall
Thunder
  Deafness
  Components
```

### Stone Ruk (20 entries)

```text
Armor
  Stoneskin
  Alarm
Create Food
  Mount
    Sand Statue
  Sand Jambiya
    Gate
Weaken
  Strength
  Feeblemind
    Sleep
Fury
  Godspeed
Wall of Sand
  Burrow
    Sand Shelter
  Earthquake
Rewind
  Components
```

### Fire Suk-Krath (22 entries)

```text
Detect Magick
  Tongues
  Identify
Ball of Light
  Pyrotechnics
    Fire Armor
      Fire Wall
Dispel Magick
  Mount
    Gate
Flamestrike
  Fireball
    Rain of Fire
      Fire Seed
    Immolate
      Demonfire
Glyph
  Empower
    Components
    Fire Jambiya
Parch
  Firebreather
```

### Shadow Drov (20 entries)

```text
Cure Blindness
  Blind
Remove Curse
  Curse
    Shadow Sword
  Components
Infravision
Detect Ethereal
  Ethereal
    Shadow Armor
    Dispel Ethereal
  Identify
Restful Shade
  Send Shadow
    Gate
  Mount
    Haunt
Create Darkness
  Fear
  Shadowplay
```

### Lightning Elkros (19 entries)

```text
Lightning Bolt
  Chain Lightning
    Light. Storm
      Light. Spear
    Light. Whip
Refresh
  Regenerate
Insomnia
  Mount
    Gate
Slow
  Quickening
    Paralyze
  Illuminant
Stamina Drain
  Energy Shield
Flouro. Footstep
  Identify
  Components
```

### Wind Whira (27 entries)

```text
Teleport
  Summon
    Gate
  Relocate
    Create Rune
  Transference
Levitate
  Fly
Hands of Wind
  Banishment
    Wind Fist
  Repel
    Wall of Wind
      Shield of Wind
    Wind Armor
Guardian
  Mount
    Components
  Messenger
    Stalker
Feather Fall
  Sandstorm
Detect Invis.
  Invisibility
    Delusion
    Dispel Invis.
  Identify
```

### Void Nilaz (23 entries)

```text
Mark
  Portal Gate
    Portal
Pseudo Death
  Animate Dead
    Vloran
    Dead Speak
      Possess Corpse
  Mount
    Gate
Psi. Suppression
  Aura Drain
    Charm Person
  Psi. Drain
Shiled of Nilaz
  Forbid Elements
    Blade Barrier
Portable Hole
  Phantasm
Elemental Fog
  Turn Element
  Identify
    Components
```

## Alternate individual-page graphs

These remain separate claims, not automatic additions. `Unknown placement` is never an opening grant.

### Fire (21 entries)

Source: http://armageddonmud.freetzi.com/firem.html

| Entry | Claimed parent |
|---|---|
| detect magick | Root as printed |
| tongues | detect magick |
| identify | detect magick |
| ball of light | Root as printed |
| pyrotechnics | ball of light |
| dispel magick | Root as printed |
| empower | dispel magick |
| component crafting | empower |
| mount | dispel magick |
| flamestrike | Root as printed |
| fireball | flamestrike |
| demonfire | fireball |
| rain of fire | fireball |
| fire jambiya | rain of fire |
| wall of fire | rain of fire |
| glyph | Root as printed |
| gate | glyph |
| parch | Root as printed |
| firebreather | parch |
| fire seed | Unknown placement |
| fire shield | Unknown placement |

### Wind (25 entries)

Source: http://armageddonmud.freetzi.com/windm.html

| Entry | Claimed parent |
|---|---|
| teleport | Root as printed |
| summon | teleport |
| gate | summon |
| relocate | teleport |
| transference | teleport |
| detect invis | Root as printed |
| invisibility | detect invis |
| delusion | invisibility |
| dispel invisibility | invisibility |
| identify | detect invis |
| levitate | Root as printed |
| fly | levitate |
| guardian | Root as printed |
| mount | guardian |
| component crafting | mount |
| messenger | guardian |
| stalker | messenger |
| hands of wind | Root as printed |
| banishment | hands of wind |
| wind fist | banishment |
| repel | hands of wind |
| wall of wind | repel |
| shield of wind | wall of wind |
| feather fall | Root as printed |
| sandstorm | feather fall |

### Lightning (18 entries)

Source: http://armageddonmud.freetzi.com/lightningm.html

| Entry | Claimed parent |
|---|---|
| lightning bolt | Root as printed |
| chain lightning | lightning bolt |
| lightning whip | chain lightning |
| lightning storm | chain lightning |
| refresh | Root as printed |
| regenerate | refresh |
| insomnia | Root as printed |
| mount | insomnia |
| energy shield | insomnia |
| ball of light | energy shield |
| slow | Root as printed |
| quickening | slow |
| illuminant | slow |
| stamina drain | Root as printed |
| paralyze | stamina drain |
| flourescent footsteps | Root as printed |
| component crafting | fluorescent footsteps |
| identify | fluorescent footsteps |

### Shadow (18 entries)

Source: http://armageddonmud.freetzi.com/shadowm.html

| Entry | Claimed parent |
|---|---|
| Cure Blindness | Root as printed |
| Blind | cure blindness |
| Infravision | Root as printed |
| Restful Shade | Root as printed |
| Send Shadow | restful shade |
| Gate | restful shade |
| Mount | restful shade |
| Remove Curse | Root as printed |
| Curse | remove curse |
| Shadow Sword | curse |
| Component Crafting | remove curse |
| Detect Ethereal | Root as printed |
| Ethereal | detect ethereal |
| Shadow Armor | ethereal |
| Identify | detect ethereal |
| Create Darkness | Root as printed |
| Fear | create darkness |
| Shadow Play | create darkness |

### Void (17 entries)

Source: http://armageddonmud.freetzi.com/voidm.html

| Entry | Claimed parent |
|---|---|
| Speak With Dead | Unknown placement |
| Elemental fog | Root as printed |
| Turn element | elemental fog |
| Identify | elemental fog |
| Mark | Root as printed |
| Travel gate | mark |
| Vortex | travel gate |
| Pseudo death | Root as printed |
| Animate dead | pseudo death |
| Shield of Nilaz | Root as printed |
| Forbid element | shield of nilaz |
| Psionic suppression | Root as printed |
| Psionic drain | psionic suppression |
| Phantasm | psionic drain |
| Aura drain | psionic suppression |
| Charm | aura drain |
| Portable hole | Root as printed |

### Stone (22 entries)

Source: http://armageddonmud.freetzi.com/stonem.html

| Entry | Claimed parent |
|---|---|
| armor | Root as printed |
| stone skin | armor |
| alarm | armor |
| create food | Root as printed |
| sand jambiya | create food |
| gate | sand jambiya |
| mount | create food |
| sand statue | mount |
| weaken | Root as printed |
| feeblemind | weaken |
| sleep | feeblemind |
| strength | weaken |
| fury | Root as printed |
| godspeed | fury |
| wall of sand | Root as printed |
| earthquake | wall of sand |
| burrow | wall of sand |
| sand shelter | burrow |
| rewind | Root as printed |
| component crafting | rewind |
| identify | rewind |
| repair item | identify |

## What would finish the missing numerical research?

The matching historical `data_files/guilds` file is the most direct missing input. A staff-style full numeric dump covering all seven guilds can also supply it. Until one is available, exact openings/caps cannot be recovered from these names, indentation or casting descriptions alone.

# Armageddon worldfile format and code reference

Version examined: the local ArmSolo Platypus Version-2024.08 candidate, 25 October 2026. This is a source-format reference, not a claim that its world has been successfully imported. The [seeder design](Armageddon_Worldfile_Seeder_Design.md) records the FutureMUD policies and decisions.

## Evidence boundary

The candidate root is `C:\Users\luker\Downloads\ArmSolo_Platypus Version-2024.08\ArmSolo`. Source paths below are relative to it. Runtime data paths are relative to `armExe/lib/`; the active constants in `db.h` determine which similarly named files are used. The root-level `races` and `crafting` files are not substitutes for `data_files/races` and `data_files/crafting.xml`.

The archive `ArmSolo_Platypus Version-2024.08.7z` has SHA256 `0fb1c3f048d2b04b1a1dd08a0028549c30fb6014c748b9bf4d3e44c82a6d1912`. Its equivalence to the supplied Spaces folder has not been established. Do not mix these layouts with the separate `codedump.c`, another Armageddon revision, or the current game.

| Code file | SHA256 |
| --- | --- |
| `db.c` | `70cd96804c4c932e08b1d772b1445936a1a06d9b69c1194ff483b7dfea367dc5` |
| `core_structs.h` | `c39a953310ce40f2a3d92c1321d106dd57fc66df341b0847e186995fa7b36c88` |
| `barter.c` | `77061ff3fd77b122d4fe4d737b471045fc688b929deeb88a9a88e7b54b5682d5` |
| `constants.c` | `54b95a366a4b5b9735be2e5a12b7f6e3fe21065efd2db706f7cea99da68bb7e6` |
| `utility.c` | `43124504cd27f587ea4397a6d3be53e8c5a074e062e4bb9dae7e20f663fb5609` |
| `guilds.h` | `86815424d9beace84bd2379d35935f424407f4eef2435a632e5858b526b46fbe` |

Admit authored world content and public support tables through an explicit whitelist. Exclude accounts, passwords, player saves/inventories, mail, private board/notes bodies, logs, bans, live cash balances and runtime caches. The original archive must never become an embedded seeder asset: it includes unrelated private/operational files.

## Lexical rules

The notation `STR` below means the historical tilde-terminated string, not a whitespace token. Preserve raw bytes and source line locations separately from decoded/translated text.

| Reader/writer in `db_file.c` | Actual rule |
| --- | --- |
| `fread_string` | Reads whole lines until the final non-whitespace character is `~`. Embedded newlines are retained and CR is inserted after LF. Empty strings may become null. EOF is fatal. A tilde in the middle of a line is not necessarily a terminator. |
| `fwrite_string` | Removes CR, replaces `#` with `n`, appends `~` and newline. Null writes an empty tilde string. Round-tripping is therefore potentially lossy. |
| `fread_text` (265-287) | Reads through the next `|`; newlines are legal and LF receives a CR. Used for pipe-separated fields. |
| `fread_var` (295-311) | Reads pipe-terminated text then uses `atoi`. |
| `fread_number` (317-357) | Signed decimal integer, skips whitespace; `|` recursively adds flag-number terms. Treat bit masks as their original 32 bits even when the stored decimal is negative. |
| `fread_word` (383-418) | Whitespace word or quoted token. |

Do not implement the format as one regular expression per line. Numeric fields, pipe fields and multiline tilde strings have distinct rules. Validate bounds and references deliberately rather than copying C parser overflow, bad array indices or `atoi`'s silent zero conversion.

## File and runtime map

There are 81 files in each of the five `zN` families, excluding backups. These counts describe files, not records or working content.

| Family | Loader / writer / runtime owner | Data purpose | FutureMUD destination |
| --- | --- | --- | --- |
| `data_files/zones` | `db.c:1959-2040`, `boot_world` 778-800, zone reset code 3832-4181 | Geography, top vnum, reset timing/mode, source-open flag, owner/privileges | Native Zone plus import provenance; native schedulers/spawners handle admitted reset policy |
| `areas/zN.wld` | `db.c:4610-4740`, writer 4786 onward | Rooms, descriptions, flags, exits and observation links | Room, approved overlay, native exits/doors, policies/hooks |
| `background/zN.bdesc` | `db.c` background reader; `core_structs.h:1284` | Indexed zone background names/descriptions | Description composition/provenance; not another room |
| `objects/zN.obj` | `db.c:3364-3499`, writer 5700-5778 | Item prototypes and components encoded in type/value slots | GameItemProto, components, combat profiles, extra descriptions |
| `npcs/zN.npc` | `db.c:2977-3159`, writer 5785-5903; creation 2523-2656 | NPC prototypes, stats, skills, clans, coded specials | Approved Simple NPC templates, health overrides, load additions, AI/hooks |
| `cmds/zN.cmd` | `db.c:1838-1954`, renumber 1660-1695, reset 3832-4181 | Ordered world-population instructions | Native spawners, outfits, progs, schedules and hooks |
| `data_files/races` | `old_boot_races`, `db.c:5572-5655`; `guilds.h:409-441` | Indexed race generation/combat/skinning defaults | Explicit race/body mapping, source-derived NPC stats and butcher products |
| `data_files/guilds`, `subguilds` | `db.c:5333-5533`; `guilds.h:129-183` | Indexed stat/combat adjustments and skill learning tables | Installed class/skill bindings where applicable; NPC generation inputs |
| `data_files/crafting.xml` | `craft.c:122-160,202-242,315-399`; `core_structs.h:2736-2778` | Craft ingredient/output alternatives, eligibility, timing, echoes | Native single-phase crafts and native eligibility/tool requirements |
| `data_files/forage` | `forage.c:43-58,101-148` | Weighted sector/category/rarity item results | Foragables and profiles; imported consumption 0% |
| `data_files/clans`, `clan_ranks/clan_N.ranks` | Clan loaders and `clan.h` | Organisation identities, ranks, political records | Live clans/ranks/memberships; politics initially provenance |
| `data_files/cities` | City tag parser, `cities.h:39` | City identity, room/zone/clan links and commodity price multipliers | Fixed economic-zone/location policy and native pricing |
| `data_files/shops` | `barter.c:1104-1225`, `barter.h:19` | Old merchant-vnum shop stock/prices/hours/messages | Permanent shops, ordinary storerooms and merchandise |
| `data_files/weather` | `db.c:2057,2249` onward | Zone weather ranges and directional neighbours | Installed native climate/region mapping; no foreign weather runtime |
| `data_files/time`, `wagons` | Runtime cache readers/writers | Saved clock/moon state and live wagon condition/location | Excluded runtime state; authored vehicle definitions are considered separately |
| `dmpl/`, `javascripts/`, serialized specials | `special.c:23221-23866`, build Makefile, `special_js.c` | Coded/external procedure associations | Behaviour-by-behaviour native AI/prog mapping, or explicit unavailable/inactive disposition |

## Zones

The file contains records beginning `#<number>`, then `STR name`, then seven integers: `top lifespan reset_mode is_open uid groups flags`. Privilege entries are tilde strings with uid/type; a string beginning `end` terminates them. The header number is read and discarded; runtime table order supplies zone identity. Preserve both header and loaded index rather than assuming they coincide.

Startup historically boots zone zero and zones with `is_open`. The importer does not translate this flag into an activation gate. Native zones remain ordinary environment/geography owners, and the fixed import configuration admits content independently of historical staffing/open status.

Reset modes: 1 waits for an empty zone, 2 permits occupied recurring resets, 0 and 3 do not recur in the demonstrated update path. Startup resets loaded zones independently of later recurrence policy. `lifespan` belongs to that source reset clock; it is not automatically a FutureMUD game-calendar interval.

## Rooms and backgrounds

Room record layout:

```text
n|name|vnum|sector|room_flags|background_index|
STR program ...
ep~
STR room_description
[c|city|]
[d|direction|destination_vnum|keyword|exit_flags|key_vnum|STR exit_description]
[e|keyword|STR extra_description]
[w|watch_room_vnum|text|]
[l|listen_room_vnum|text|]
... next n| record ...
x|
```

Directions are the source six-direction indices and must be translated to native directions, never cast to an unrelated enum. Preserve directed links; a missing reverse edge is not an instruction to invent one. Exit key is an item vnum. Door flags, closed/locked startup state and key identity are separate native concerns. Watch/listen links express remote observation and do not imply movement exits.

The active picking code uses exit flag bits 12–14 as a three-bit difficulty level, with source thresholds 0, 15, 30, 45, 60, 75, 100 and impossible. The configured native difficulty ordinals are respectively 0, 2, 4, 5, 6, 7, 9 and 10; a negative source key remains unpickable. Key codes use source vnum plus one, preserving vnum zero without colliding with an unset native pattern. Source force/smash behaviour is retained as provenance rather than claimed equivalent.

Backgrounds have an exact fixed slot count (`MAX_BDESC`), each `n|name|STR description`. File order supplies the index referenced by rooms; there is no per-record numeric key. Resolve names/descriptions in the room's source zone.

Sector indices, in order: 0 Inside, 1 City, 2 Field, 3 Desert, 4 Hills, 5 Mountain, 6 Silt, 7 Air, 8 Nilaz, 9 Light Forest, 10 Heavy Forest, 11 Salt Flats, 12 Thornlands, 13 Ruins, 14 Cavern, 15 Road, 16 Shallows, 17 Southern Cavern, 18 Sewer, 19 Cottonfield, 20 Grasslands, 21 Fireplane, 22 Water, 23 Earth, 24 Shadow, 25 Air, 26 Lightning. The duplicate Air label does not collapse distinct source indices.

Room flag bit positions (`core_structs.h:957-987`):

| Bit | Meaning | Bit | Meaning |
| --- | --- | --- | --- |
| 0 | Dark | 16 | No wagon |
| 1 | Death | 17 | No flying |
| 2 | No mob | 18 | No forage |
| 3 | Indoors | 19 | Populated |
| 4 | Safe | 20 | Restful shade |
| 5 | Sparse plants | 21 | No psi |
| 6 | Heavy plants | 22 | Ash |
| 7 | No magic | 23 | Spell alarm |
| 8 | Tunnel | 24 | Magic OK |
| 9 | Private | 25 | Allanak |
| 10 | Sandstorm | 26 | Tuluk |
| 11 | Falling | 27 | Lit |
| 12 | Mana regeneration | 28 | Immortal |
| 13 | Unsaved | 29 | Cargo |
| 14 | No climb | 30 | No elemental magic |
| 15 | No mount | 31 | No hide |

Translate exercised effects through their consumers, not their English names. In particular, `RFL_SAFE` is used by quitting (`other.c:1697-1703`, `1828-1832`) and the Quit exit hint (`info.c:7626-7634`); mapping it to universally peaceful combat would be incorrect. The corpus has 633 such rooms, correctly mapped to native SafeQuit. `No forage` must override a sector's ordinary forage profile.

## Item prototypes

```text
#vnum limit
STR keywords/name
STR short_description
STR long_description
STR full_description
type extra_flags wear_flags state_flags material
value0 value1 value2 value3 value4 value5
weight cost temperature
length damage_dice_count damage_die_size damage_plus
STR program ...
ep~
[E
 STR keyword
 STR description]
[A apply_location modifier]
... next #vnum ...
$
```

The loader also accepts the old four-integer header `type extra_flags wear_flags material`, supplying state 0. Detect this header using its line boundary. All six value slots survive parsing even when their type's effective loader ignores some. Armour type 9 repairs value3 from value1 when zero. Temperature is reset to zero. An E keyword equal to the object's name is folded/dropped by the loader. Affects have a fixed maximum and invalid apply locations are discarded by historical code; report them rather than silently accepting arbitrary native trait IDs.

Type numbers: 1 Light; 2 Component; 3 Wand; 4 Staff; 5 Weapon; 6 Art; 7 Tool; 8 Treasure; 9 Armour; 10 Potion; 11 Worn; 12 Other; 13 Skin; 14 Scroll; 15 Container; 16 Note; 17 Drink Container; 18 Key; 19 Food; 20 Money; 21 Pen; 22 Boat; 23 Wagon; 24 Dart; 25 Spice; 26 Chit; 27 Poison; 28 Bow; 29 Teleport; 30 Fire; 31 Mask; 32 Furniture; 33 Herb; 34 Cure; 35 Scent; 36 Playable; 37 Mineral; 38 Powder; 39 Jewelry; 40 Musical; 41 Rumor Board.

Type/value interpretation belongs to `constants.c:3325-4150` and each gameplay consumer; the slots are not six universal properties. In particular, a container's capacity and a weapon's dice use different scales and native component models. A complete type/value table must accompany the generated translation policy before item installation is considered qualified.

Source weight is hundredths of a fictional stone of approximately one kilogram (`ocreation.c:2215-2228,3065`; parser help near 5481), hence approximately 10 grams per stored item-weight unit. Do not use the real-world 14-pound stone. Source container capacity includes its own weight, while native capacity generally describes contents. Source length needs a separately verified unit policy.

Keep source prototype limit, vnum, raw flags, values and affects as provenance. Native primary keys are independently allocated. `UniqueName=Vnum<n>` identifies the imported prototype and is also the lookup key used by generated progs/crafts.

### Item value slots and materials

The following table transcribes `constants.c`'s `oset_field` descriptors, checked against `ocreation.c:1034-1985`. A dash means no labelled slot, not an instruction to repurpose it. A descriptor is not proof of an active gameplay consumer; the caveats below distinguish these cases.

| Type | v0 | v1 | v2 | v3 | v4 | v5 |
| --- | --- | --- | --- | --- | --- | --- |
| Light | Duration hours (-1 permanent) | Maximum fuel | Light source | Illumination range | Colour | State flags |
| Component | Magic sphere | Relative power | - | - | - | - |
| Wand | Maximum mana | Power/level | Current mana | Spell | - | - |
| Staff | Maximum mana | Power/level | Current mana | Spell | - | - |
| Weapon | Damage plus | Dice count | Die sides | Attack class | - | Poison |
| Art | - | - | - | - | - | - |
| Tool | Tool type | Quality 0-10 | - | - | - | - |
| Treasure | Type | Quality | - | - | - | - |
| Armour | Current points | Repairable points | Size | Original maximum points | - | - |
| Potion | Power | Spell 1 | Spell 2 | Spell 3 | - | - |
| Worn | Clothing size | Flags | - | - | - | - |
| Other | - | - | - | - | - | - |
| Skin | - | - | - | - | - | - |
| Scroll | Power | Spell 1 | Spell 2 | Spell 3 | Source (unimplemented setter) | Reading/writing language skill |
| Container | Capacity | Flags | Key vnum | Corpse race | Maximum individual weight | Corpse size |
| Note | Pages | Note-file index | Language (unimplemented setter) | Note flags | Skill taught | Maximum taught percent |
| Drink container | Capacity | Remaining amount | Liquid | Liquid flags | Spice skill | Poison |
| Key | - | - | - | - | - | - |
| Food | Filling hours | Food type | Age/rot | Poison | Race food | Spice skill |
| Money | Amount | Coin type | - | - | - | - |
| Pen | - | - | - | - | - | - |
| Boat | - | - | - | - | - | - |
| Wagon | Entrance room | Maximum mounts | Front room | Flags | Damage | Maximum size |
| Dart | Plus | Dice count | Die sides | Poison amount | - | - |
| Spice | Power | Spice/spell type | Smokeable | Poison | - | - |
| Chit | Tailor vnum | Completion time | Item vnum | Item runtime index | - | - |
| Poison | Type | Amount (editor says unused) | - | - | - | - |
| Bow | Range | Pull strength | Requires loading | Currently loaded bolt vnum | - | - |
| Teleport | - | - | - | - | - | - |
| Fire | Lit | Duration ticks | Heat | - | - | - |
| Mask | - | - | - | - | - | - |
| Furniture | Weight/capacity | Flags | Occupancy | - | - | - |
| Herb | Type | Edible | Colour | Poison | - | - |
| Cure | Poison cured | Vial/tablet/syringe form | Side-effect poison | Taste | Strength 0-100 | - |
| Scent | Scent ID | Current uses | Maximum uses | - | - | - |
| Playable | Play type | - | - | - | - | - |
| Mineral | Type bit | Edible | Requires grinding | Ore type (unimplemented) | - | - |
| Powder | Texture/type | Solubility | - | - | - | - |
| Jewelry | Quality 0-10 | Flags | - | - | - | - |
| Musical | Quality 0-10 | Instrument type | Flags | - | - | - |
| Rumour board | Board index | - | - | - | - | - |

Dart, Chit, Fire and Powder have descriptors but no `set_oval` branch in the examined editor; Food v4 is also descriptor-only there. Smokeable and the note-file index are code-maintained, not general editable fields. A key's lock identity comes from prototype vnum handling, not an invented value-slot key number. Trace real consumers for every exercised imported slot.

Bow slot 3 is maintained load state, not an ammunition restriction: `other.c:2622–2671` accepts any `OFL_BOLT`, stores its vnum and places it inside a loadable bow; unload/fire resolves that contained projectile and clears the slot (`2727–2744`, `2883–2899`, `2982–2985`, `3053–3057`, `3267–3270`). The ordinary-bow path instead uses a wielded `OFL_ARROW` (`2868–2882`). Native imports start unloaded and preserve the original slot as provenance; a per-vnum ammunition grade would introduce a restriction absent from the source.

Source materials (`core_structs.h:294-324`, labels `constants.c:3230-3262`): 0 none, 1 wood, 2 metal, 3 obsidian, 4 stone, 5 chitin, 6 gem, 7 glass, 8 skin, 9 cloth, 10 leather, 11 bone, 12 ceramic, 13 horn, 14 graft, 15 tissue, 16 silk, 17 ivory, 18 duskhorn, 19 tortoiseshell, 20 isilt, 21 salt, 22 gwoshi, 23 dung, 24 plant, 25 paper, 26 food, 27 feather, 28 fungus, 29 clay. `MAX_MATERIAL=31` leaves slot 30 without a named enum. These indices are not native Material IDs. Non-armour item HP uses source material/weight (`db.c:3235-3275`); armour wear uses attack class and material (`combat.c:5150-5217`).

Source weapon damage rolls `dice(v1,v2)+v0` (`combat.c:3660,4174-4177`). v3 is an attack/move class derived from `TYPE_HIT=300` through `TYPE_RAZOR=317`, not directly a FutureMUD DamageType. Source stun/physical multipliers depend on attack family (`3115-3152`). Armour absorbs points in tenths with a random remainder (`5224-5236`), while durability uses the attack/material table. A narrow `wtype > 311` guard despite later enumerated classes can skip wear; retain this as a source caveat, not a reason to reproduce an unsafe/incorrect native index. Translate damage, armour and HP as a calibrated set through native expressions.

## NPC prototypes

```text
#vnum limit
STR keywords/name
STR short_description
STR long_description
STR full_description
STR background
act_flags character_flags affected_flags language S
level offense defense armour max_hit damage
obsidian gp_mod
position default_position sex guild tribe luck race
STR program ...
ep~
[E STR keyword STR description]
[S skill_id percent]
[x age]
[C STR clan_name STR clan_flags rank]
... next #vnum ...
$
```

`db.c:2523-2656` applies race generation, guild/skill defaults and explicit overrides when creating a live NPC. `gp_mod` modifies the starting cash roll, not prices: positive values add a random amount from zero to that value; negatives subtract a random amount down to it, with cash floored at zero. Age -1 defaults from race maximum age. `[UNIQUE_STATS]` extra descriptions can override strength/agility/wisdom/endurance and optional height/weight. Resolve race/guild/skill indices using this version's active tables before mapping them to native identities.

Only age -1 is unspecified: use C integer `max_age / 2`; explicit zero remains zero. The race identity tuple is `[race_type, max_age, attack_type]`, so maximum age is index 1. The corpus has 2,459 unspecified ages and one explicit zero. It has no NPC extra-description entries, hence no authored `[UNIQUE_STATS]` overrides to apply.

Physical generators are the race switches in `utility.c:2839-3223`, resolved using `guilds.h` numeric race identities. The package retains their expressions, support bounds and exact means, including integer division before averaging. Height uses inches and character weight uses ten-stone units (approximately 10 kg per stored unit); item hundredth-stone weights have a different scale. Default generators are `66 + dice(2,8)` inches and `(180 + dice(6,10)) / 20` with C integer division. A similarly shaped native animal's dimensions are not a substitute for the source race's generator.

Spoken language-table indices map to Sirihish, Bendune, Allundean, Mirukkim, Kentu, Nrizkt, Untal, Cavilish, Tatlum, Anyar, Heshrak, Vloran, Domat and Ghatti, in that order (`constants.c:5870+`). Their spoken skill IDs are 101-109, 235, 237, 239, 241 and 243. `init_languages` (`limits.c:461-527`) preserves a parsed language selection when an existing spoken-language skill is present; otherwise it derives primary/additional languages from race and guild. In this corpus 2,083 NPCs have positive spoken-language skills and 377 use racial initialization; three explicit selections are invalid negative indices. Neither namespace is a FutureMUD language ID.

Explicit C memberships supplement the legacy tribe field; historical loading can synthesise legacy membership when no C record exists. Clan rank ordering must be preserved. Do not turn every affected flag into an eternal effect: classify permanent prototype state separately from live conditions.

Native imported templates use Simple definitions with literal descriptions, explicit race/culture/ethnicity/name bindings, stats/skills, and template health override. The selected HP strategy is `BrainHitpoints`. Keep source max_hit meaningful through source-derived HP expressions; do not change the selected native race/player health default. Source weapon dice and armour must be converted together with this HP scale into native combat profiles.

## Reset commands

The parser supports comment lines beginning `*` and S terminators. Vnum arguments are renumbered to prototype/room table positions for historical runtime use. The importer preserves vnums, not those temporary positions.

| Opcode | Serialized fields after opcode | Runtime interpretation |
| --- | --- | --- |
| M | if_flag, mob_vnum, count_cap, room_vnum | Create current NPC; global per-vnum cap and prototype limit apply, subject to boot exception |
| N | if_flag, three integers | Accepted by parser but no demonstrated runtime case; default handler fails. Invalid/inactive source, not an alias for M |
| O | if_flag, item_vnum, room_count_cap, room_vnum, six value overrides, state | Load an item into a room; global prototype limit also applies |
| G | if_flag, item_vnum, count_cap, six values, state | Give to current NPC; inventory/global limits |
| E | if_flag, item_vnum, unused_arg2, wear_slot, six values, state | Equip current NPC if slot available |
| P | if_flag/depth, item_vnum, arg2, arg3, six values, state | Put in previous container ancestry; parsed arg2/arg3 are not effective multiplicity/limits in the shown code |
| A | if_flag/depth, item_vnum, arg2, arg3, six values, state | Associate around the last furniture/table ancestry; not generic placement beside another item |
| D | if_flag, room_vnum, direction, state | Startup only: 0 open, 1 closed, 2 locked and closed |
| X | if_flag, selector, two STR | Selector 1 edits last object's extra description; selectors 2-4 have no demonstrated effect |
| Y | if_flag, selector, two STR | Add last NPC's extra description; selector ignored |

Execution order and success matter. The outer gate is `last_cmd || !if_flag`. P/A overload the same nonzero value as ancestry depth, so depth also makes the command depend on prior success. Follow the container/table stack, last NPC, last object and success state explicitly in translation; do not flatten the file into independent spawn rows.

`mobs_existing_in_zone` (`db.c:1618-1630`) actually returns `npc_index[nr].number`: a global vnum count. Its old zone loop is commented out. Boot `unlimited_spawn` bypasses M's command cap but still respects the prototype's global limit (`3875-3877`). D is enclosed by `if (!done_booting)` (`4147-4169`), so recurring resets do not repeatedly close/lock doors.

Use native NPCSpawners for applicable populations, native outfits/load additions for inventory/equipment, generated progs/hooks/schedules for ordering and remaining admitted behaviours. No foreign reset interpreter or active-zone runtime is part of the design. Native schedules run on native calendar time; conversion must state its time policy.

## Races, guilds and subguilds

Active race records start at loaded index 1; index 0 is synthesised. Layout: name STR, abbreviation STR, labelled strength/agility/wisdom/endurance dice lines, offense base/defense base/natural armour, attack dice, race type/maximum age/attack type, then exactly five skin-vnum slots. Every non--1 skin slot is followed by singular/plural skin-message STRs. The root-level race copy's two skin slots are a different layout.

Guilds are indexed from 1; subguilds from 0. Each has name/abbreviation STRs, four stat modifiers, four combat integers (`off_kpc def_kpc off_bonus def_bonus`), player flag, then repeated skill-name STR, prerequisite-name STR (`none` permitted), maximum learned, starting learned. A skill name beginning S terminates the list. Extra fields in the C struct do not prove serialization. Map native traits/classes by explicit identity rather than numeric equality.

## Crafting

Only `data_files/crafting.xml` is active in this candidate, containing one sample recipe, charred meat. It is not a recovered full Armageddon craft catalogue. `data_files/crafting` is an older tagged format and the root `crafting` another stale copy.

`CRAFT/ITEM` children: `NAME`, `DESC`, `FROM`, `INTO_SUCCESS`, `INTO_FAIL`, `SKILL`, `PERC_MIN`, `PERC_MOD`, `DELAY_MIN`, `DELAY_MAX`, `TO_CHAR_SUCCESS`, `TO_ROOM_SUCCESS`, `TO_CHAR_FAIL`, `TO_ROOM_FAIL`, `ONLY_TRIBE`, `REQUIRE_FIRE`, `TOOL_TYPE`, `TOOL_QUALITY`. Ingredient/output arrays have five slots and default to -1. Skill/tool names resolve through source tables. `ONLY_TRIBE` resolves a clan name; unknown values become unrestricted in the historical loader, so literal `0` is not clan ID zero.

The sample uses input 7, success 6, failure 8, cooking, minimum 90, modifier 10, delay 1-2, no tribe restriction/fire/tool. Validate all prototype and trait references. The source XML writer (`craft.c:569-637`) loses DESC/TOOL_TYPE and has array/zero-writing errors; `.old` output is not a lossless authority.

Map to one native phase, including real failure products and source eligibility/tools/fire where present. Native stored craft echo is `$0 craft|crafts $p1.`; `$p1` is the craft product token and is rewritten for the emote. Literal `$1` is not an interchangeable stored product reference.

## Foraging

Each record: category STR, sector STR, then labelled `VNUM:`, `SHARES:`, `RARE:` integers. Shares are static selection weights, never a stock balance. Rarity flags are 1 common, 2 uncommon, 4 rare, 8 ultra-rare. Selection chooses a band from a roll and skill, then a weighted item; if empty it falls back by shifting to a less rare band. `[CUSTOM_FORAGE]` can provide local tuples of type/shares/vnum/rarity/name.

The candidate's entire active file has five stone results: hills/19258/300/4, mountains/19258/300/4, mountains/19259/300/2, hills/19259/300/1, mountains/19260/300/1. Do not infer plants, herb tables or a complete game ecology from this file.

Use native profiles, output difficulty/outcome ranges and weights. Four historical rarity bands do not directly equal three native successful outcomes; the package must declare that adaptation. Imported Foragables consume 0% of room yield. Native/default Foragables consume 100%, preserving the one-point action debit. Zero bypasses depletion, while profile/type, check, eligibility, positive output and on-forage hooks remain.

## Clans, cities and shops

Clans are a fixed 57-record table. Each has name STR and type/status/store/liege integers; R entries carry other-clan/relation integers, ending with the consumed non-R sentinel (the sample uses `~`). Rank files have count followed by rank-name STRs; a missing/zero file supplies one rank, `a member`. Higher source numeric ranks are superior. Preserve relations as provenance unless a demonstrated native equivalent exists; ExternalClanControl is appointment authority, not a generic diplomacy relation.

Cities have two loaded slots in this candidate, including None and Allanak. Tag layout includes Name/Of/Crim_flag STR, Zone integer, Tribe numeric-or-name STR, Room integer, repeated Commod name STR/float, then End; `*` starts comments. Commodity price multipliers are not native economic-zone IDs or native material IDs.

The active old shop file begins with 173 records. Per record: 15 producing-vnum slots, 20 new-item slots, five buy multipliers, five sell multipliers, five buy types, five sell types, eight message STRs, then merchant NPC vnum/open hour/close hour. Producing vnums are renumbered; new-item conversion is commented out. The alternate per-shop loader and its extra clan/material/barter fields are not ordinary startup's format.

Runtime shop lookup uses the first matching merchant vnum: 165 effective bindings and eight shadowed records survive the declared 173-record table. Sell-type slots pair with buy multipliers for merchant-to-customer prices; buy-type slots pair with sell multipliers for merchant payouts. Both price paths finish with `floor(cost + 0.5)`, producing whole obsidian units. Source room city fields, rather than zone numbers, determine city commodity adjustments; all 26,137 rooms in this corpus have city 0, so Allanak's commodity factors are inactive.

Historical merchandise is the merchant's carrying inventory; money is its obsidian wallet. Produced goods act as unlimited stock. Opening/closing can add/remove stock, and passive sales mutate it. Prices use item cost, the relevant shop multiplier and city commodity adjustments; produced buyback is halved and clan pricing can average margins. Equal open/close hour means always open; ranges can wrap midnight.

The agreed native policy is permanent shops independent of NPC ownership or presence. Keep keeper vnum only as provenance/location evidence. Resolve a fixed storefront map and create normal storerooms called/described `The Storeroom of <shop name>` where required. Use existing native merchandise, price/reorder/stock/till systems. Do not add shop functionality to emulate a merchant's live inventory or wallet.

## Specials, support files and runtime state

Program strings serialize `C command coded_name`, `dmpl command filename`, or `javascript command filename`, terminated by `ep`. `special.c:23353` parses them and the coded-special registry determines C procedure identities. Command names must resolve against this version's constants, not a modern command enum.

The provided Makefile compiles C files; JavaScript includes/libraries are commented out, and `special_js.c` supplies stubs. Real JavaScript support in `CPP/special_js.cpp` is not evidence that this build runs it. DMPL dispatch is commented out in the shown runtime paths (`special.c:23744,23809,23866`), and the supplied directory contains guides rather than referenced gameplay bodies. `data_files/programs` has two lines but its declared/called reader has no matching C definition in this tree; do not invent a grammar from those lines alone.

Static weather A records carry zone/lifetime/minimum and maximum temperature/minimum and maximum condition plus optional median; missing median is derived. N/S/E/W entries describe weather neighbours. `data_files/time` is a six-integer saved hour/day/month/year/two-moon-phase cache; startup derives time from real time elsewhere. `wagons` stores live destination/direction/damage/mount state, with old short rows defaulting missing fields. These caches must not silently become authored seed defaults.

Public socials/actions/poses/combat-message/help tables can be catalogued independently, but are outside the user's initial world-content seeder scope. Operational help-list indexes are not a second world schema. Do not read dynamic board/mail/player text merely because it shares the lib tree.

## Corpus audit and effective source defects

Static public-file audit on 25 October 2026, not a historical engine boot:

| Family | Raw records | Distinct vnums / diagnostics |
| --- | --- | --- |
| Rooms | 26,137 | All unique; 77,368 raw exits, two missing targets omitted, 77,366 effective exits |
| Objects | 7,418 | 7,415 vnums; three conflicting duplicate groups |
| NPCs | 2,475 | 2,460 vnums; 15 duplicate groups, four identical and 11 differing |
| Resets M / O / G / E / P / A | 2,507 / 3,553 / 2,637 / 8,301 / 8,259 / 0 | Three unresolved population references: one room, one NPC, one item |
| Reset D / X | 2,393 / 32 | Eight D commands also reference absent rooms; 11 invalid commands across all opcodes |
| Clans | 58 | Loader admits indices 0-56 (57 records); the last record is archive only |
| Shops | Declared 173 | 173 admitted; one trailing record is outside the declared count; 21 array repairs recorded separately |
| Forage | 5 | All output references resolve; three distinct items |
| Crafting | 1 | Success output 6 absent; input 7 and failure output 8 present |

The historical index builders (`db.c:656-727`) load ascending zone-file order, check for an existing vnum and skip later records. Thus the first definition wins deterministically. Items 2138, 2513 and 2515 duplicate within `objects/z2.obj`; NPC duplicates are within `npcs/z74.npc`. Keep the first, archive the discarded record and its source path/line; do not create suffixed native alternatives for discarded duplicates. Suffixes remain appropriate for genuinely different effective reset variants.

`fix_exits` (`db.c:744-762`) frees exits whose destination cannot resolve, so omit those links while preserving valid rooms. Reset `is_valid_com` (`1796-1829`) rejects missing current rooms and missing/nonpositive item prototypes; later runtime paths also guard absent item lookups. A missing M prototype instead reaches an invalid array index (`3875-3877`): this is malformed source/undefined behaviour, not faithful behaviour to reproduce. Omit that command with an explicit source-defect diagnostic and preserve its dependent-chain disposition.

The sample craft's missing first success output causes `object.c:7973-7976,8139-8145` to tell the player the recipe is broken, restore inputs and abort. Its correct import disposition is disabled/omitted with a diagnostic, not a newly invented product. This candidate supplies no demonstrated usable craft recipe; the converter still needs a tested craft mapping using a valid synthetic/public fixture.

Unresolved exits: room 10500 targets 20050 (`areas/z10.wld:1347` record), and room 49653 targets 50069 (`areas/z49.wld:5165` record). Population defects: `cmds/z46.cmd:2` refers to NPC 6201; `cmds/z46.cmd:49` refers to item 2783; `cmds/z76.cmd:1` loads into absent room 76019. Eight boot-only D commands in `cmds/z76.cmd:3-10` reference absent rooms 76016, 76017, 76018, 76019 and 76022. Thus the three population defects and eight door commands are different counts, not conflicting reports.

Room headers can immediately follow pipe-terminated observation text on the same physical line. For example, `areas/z3.wld:656` contains an observation followed by `|n|...3068|`. A line-anchored header regex misses 76 valid rooms and greatly overstates missing references; the sequential pipe/tilde parser is the authority for these counts.

The shop array repairs are authored adaptations, not claimed historical error recovery: preserve wrapped numeric arrays, pad missing producing slots with -1, pad absent new-item/type slots with 0, and discard only surplus empty sentinel slots. Original rows and every affected path/line/field/value remain in the source manifest. Do not silently continue on an ambiguous array or discard a real product. The C loader has no equivalent validation and can misalign following fields on these malformed rows.

All 415 whitelisted public files (14,848,782 bytes) were compared byte for byte with the local archive through `tar -xOf`; their concatenation in archive order has SHA256 `deeb25038d4dcff601a747f4de021832c6b3d6c2fc96d1fc4794b4351a3eac84`. This validates the local extraction, not equality with the Spaces upload.

## Qualification requirements

Before shipping the data package, produce per-file hashes, parsed record counts, duplicate/reference audits, encoded-text/length audits, source-to-native identity mappings, and a disposition for every behaviour-bearing field. Preserve missing/inactive source behaviour explicitly. Parser acceptance does not establish functioning native content; native startup, spawning, crafting, forage, combat and shop acceptance are separate checks.

The seeder has no configuration questions and no upsert path. Validate foundations, corpus and identity collisions before mutation; use one transaction and persist completion within it. Refuse a completed import inside execution. A failed, fully rolled-back first attempt may retry. Keep native primary keys independent of source vnums and retain `Vnum<n>` unique names in each supported entity type's namespace.

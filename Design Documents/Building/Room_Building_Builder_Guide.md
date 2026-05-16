# Room Building Builder Guide

This guide explains the FutureMUD room-building model for engine users, world builders, and AI agents helping them. It focuses on the builder-facing workflow and command surface rather than the internal persistence model.

In this document, **room** and **cell** mean the same thing.

## Quick Mental Model

FutureMUD locations are built from a small set of concepts:

| Concept | What it means to a builder |
| --- | --- |
| Shard | The highest location container. A shard represents a broad world, plane, planet, region of reality, or other top-level space. It owns sky, clocks, calendars, celestial objects, and minimum light settings. |
| Zone | A geographic or operational region inside a shard. Zones own latitude, longitude, elevation, weather controller, clock time zones, light multiplier, and the default room created for the zone. |
| Room or Cell | The place characters stand, look, move, forage, hear, quit safely, and interact. Cells have names, descriptions, terrain, outdoors type, atmosphere, light settings, exits, and overlays. |
| Area | A cross-cutting grouping of rooms. Areas are not a strict layer between zones and rooms. They can span zones, and rooms may belong to more than one area. |
| Overlay | A versioned set of room presentation and exit data. Builders edit overlays rather than directly overwriting live room state. |
| Overlay Package | A named collection of overlays that move through review, approval, and swap workflows together. |
| Terrain | The mechanical and presentation baseline for a room: movement, layers, natural light expectations, atmosphere, tracks, weather override, forage defaults, and editor display. |
| Exit | A connection between rooms. Exits can be cardinal, non-cardinal, door-capable, climbable, fall exits, layer-blocking, hidden by prog, or limited by character size/posture. |

Most building sessions follow this shape:

1. Open or create an overlay package.
2. Create, revise, or adopt rooms in that package.
3. Set room names, descriptions, terrain, outdoors type, light, atmosphere, exits, and landmark data.
4. Submit the package for review.
5. Accept, decline, swap, or obsolete the package depending on builder permissions and world workflow.

## Location Hierarchy

### Shards

Shards are the highest organisational unit. A shard usually represents a separate world-scale context: a continent-scale campaign map, a planet, a plane, a starship interior shard, an underworld shard, or any other major separation that should have its own sky and time model.

Shards hold:

- Sky template.
- Clocks and calendars.
- Celestial objects.
- Minimum outdoor lux.

Key commands:

```text
shard list
shard new <name> <sky template>
shard set <shard> name <name>
shard set <shard> clocks <clock...>
shard set <shard> calendars <calendar...>
shard set <shard> celestials <celestial...>
shard set <shard> sky <sky template>
shard set <shard> lux <minimum lux>
```

Use shards when the world boundary matters mechanically. If the only difference is neighbourhood, climate, or staff organisation, use zones or areas instead.

### Zones

Zones are the normal geographic or operational unit under a shard. A zone might be a city district, wilderness region, mine complex, ship deck, island, dungeon level, or road network. Zones are especially important because they carry weather, time-zone, light, elevation, and forage defaults.

Zones hold:

- Name.
- Shard membership.
- Latitude, longitude, and elevation.
- Ambient light multiplier.
- Per-clock time zone.
- Weather controller.
- Foragable profile.
- A default cell.

Creating a zone requires an open `Under Design` overlay package. The command creates the zone and a default cell in the current package.

Key commands:

```text
zone new <name> <shard> <timezones...>
zone show <zone>
zone set <zone> name <name>
zone set <zone> latitude <decimal>
zone set <zone> longitude <decimal>
zone set <zone> elevation <metres>
zone set <zone> light <multiplier>
zone set <zone> timezone <clock> <timezone>
zone set <zone> fp <foragable profile>
zone set <zone> fp clear
zone set <zone> weather <weather controller>
zone set <zone> weather none
zones
rooms <zone id|name> [+keyword] [-keyword]
rezone <zone>
```

The `rooms` command can filter by zone and by keywords. Use `+keyword` to require text and `-keyword` to exclude text.

### Rooms and Cells

A cell is the concrete playable location. Rooms and cells are the same thing in the engine. They should be understood as synonyms.

A cell has:

- A parent room identity and zone.
- A current overlay.
- Name and description.
- Terrain.
- Outdoors type.
- Hearing profile.
- Ambient light multiplier.
- Added light.
- Atmosphere override.
- Forage override.
- Safe quit flag.
- Exits.
- Variable register values for progs.
- Optional landmark or meeting-place effect.

Key inspection commands:

```text
cell show
cell overlay <id|name> [revision]
cell overlay clear
rooms <zone id|name> [+keyword] [-keyword]
show terrain [+keyword] [-keyword] [*keyword]
show overlays
show exittemplates
```

`cell overlay` is a builder view/edit helper. It lets you view a specific overlay revision for your current room. `cell overlay clear` returns you to the default current overlay. You will see the information associated with your current overlay when you look at the room, but others will still see the room's default current. This way, you can be working on changes to an area live without impacting on what others are seeing.

### Areas

Areas are independent room groupings. They are not a strict parent of rooms, and they are not a required step between zones and cells. A room can be in more than one area, and an area can include rooms across multiple zones.

Use areas when you need a named collection of rooms for:

- Weather overrides.
- Builder organisation.
- Scripts and effects.
- Shared descriptions or ambient logic.
- Administrative grouping.

Key commands:

```text
area list [zone]
area show <id|name>
area new <name>
area edit <id|name>
area edit
area close
area rename <name>
area add
area remove
area weather <id|name>
area weather clear
```

`area edit` selects the area you are changing. `area add` adds the current room to the open area, and `area remove` removes it.

## Overlay Packages

FutureMUD room building is package-based. You usually do not directly mutate live room presentation. Instead, you work in an overlay package, submit it, and then review or swap it.

Package statuses you will see:

| Status | Meaning |
| --- | --- |
| Under Design | Editable draft. Normal building commands require this. |
| Pending Revision | Submitted for review. Some prog and link operations can still target this status, but normal builder editing should happen before submission. |
| Current | Approved and available as live/current overlay data. |
| Rejected | Declined package revision. Can be revised. |
| Obsolete | Retired package. |

Key commands:

```text
cell package list [all|by <builder>|mine]
cell package new <name>
cell package open <id|name>
cell package close
cell package show [<id|name>]
cell package rename <name>
cell package revise <id|name>
cell package submit <comment>
cell package review list
cell package review all
cell package review <id>
accept edit <comments>
decline edit <comments>
cell package history <id|name>
cell package swap <id|name>
cell package delete
cell package obsolete
```

Common package workflow:

```text
cell package new "North Road Expansion"
cell new
cell set name "A Packed-Dirt Road"
cell set terrain "Dirt Road"
cell set type outdoors
cell set desc
cell package submit "Adds the first road room north of the gate."
cell package review list
cell package review 42
accept edit "Looks good."
cell package swap 42
```

Use `cell package revise` for an existing current or rejected package when you want a new revision. Use `cell package delete` only for an open `Under Design` package you really want to discard.

`cell package review <id>` and `cell package review all` open a temporary review proposal. Type `accept edit <comments>` or `decline edit <comments>` as the follow-up response before the proposal times out.

## Terrain, Outdoors Type, and Environment

Terrain is the main mechanical profile for a room. It is not only display text. Terrain affects movement, stamina, infection risk, weather behaviour, forage defaults, available room layers, natural light expectations, tracks, atmosphere, and map/editor presentation.

Useful terrain commands:

```text
terrain list [+keyword] [-keyword] [*keyword]
terrain new <name>
terrain clone <terrain> <new name>
terrain edit <terrain>
terrain edit
terrain show <terrain>
terrain close
terrain set name <name>
terrain set atmosphere none
terrain set atmosphere gas <gas>
terrain set atmosphere liquid <liquid>
terrain set movement <percentage>
terrain set stamina <cost>
terrain set hide <difficulty>
terrain set spot <difficulty>
terrain set forage none
terrain set forage <profile>
terrain set weather none
terrain set weather <weather controller>
terrain set cover <cover>
terrain set default
terrain set infection <type> <difficulty> <virulence>
terrain set outdoors
terrain set indoors
terrain set exposed
terrain set cave
terrain set windows
terrain set model <model>
terrain set gravity normal
terrain set gravity zerog
terrain set tag <tag>
terrain set tracks
terrain set trackvisual <percentage>
terrain set tracksmell <percentage>
terrain set mapcolour <0-255>
terrain set editorcolour <#AARRGGBB>
terrain set editortext <one or two letters>
terrain override <terrain> [<prog>]
terrain reset
terrain planner
```

Terrain models control room layers. Common model names include:

- `outdoors`: ground plus air layers.
- `indoors`: ground only.
- `cave`: ground plus limited air.
- `cliff`: air levels only.
- `rooftops`: ground and rooftops.
- `trees`: ground, trees, and air.
- `talltrees`: ground, multiple tree layers, and air.
- `cavetrees`: cave-like tree layers.
- `shallowwater`, `deepwater`, `verydeepwater`: water plus surface/air layers.
- `underwater`, `deepunderwater`, `verydeepunderwater`: submerged layers only.
- `shallowwatertrees`, `shallowwatercave`, `deepwatercave`, `verydeepwatercave`: water variants with tree or cave layers.

Cell-level environment commands:

```text
cell set terrain <terrain>
cell set type outdoors
cell set type indoors
cell set type cave
cell set type windows
cell set type exposed
cell set hearing <hearing profile>
cell set lightmultiplier <multiplier>
cell set lightlevel <lux>
cell set forage clear
cell set forage <profile>
cell set atmosphere gas <gas>
cell set atmosphere liquid <liquid>
cell set atmosphere none
cell set safequit
```

Outdoors type:

| Command value | Runtime meaning | Typical use |
| --- | --- | --- |
| `outdoors` | Outdoors | Streets, fields, rooftops open to sky. |
| `indoors` | Indoors | Rooms with no direct weather exposure. |
| `windows` | Indoors with view of outside | Buildings with natural light through windows. |
| `cave` | Indoors with no natural light | Caves, tunnels, sealed vaults. |
| `exposed` | Indoors exposed to climate | Sheds, ruins, covered verandas, open-sided shelters. |

Changing terrain can also set default light multipliers based on the terrain's default outdoors type. If you need a special case, set terrain first and then adjust `cell set type`, `cell set lightmultiplier`, or `cell set lightlevel`.

## Exits

Exits are directional or named connections between cells. They carry both movement behaviour and builder-facing presentation.

### Cardinal Exits

Cardinal exits use standard map directions such as north, south, east, west, up, down, and diagonals.

Create a new room in a cardinal direction:

```text
cell dig <direction>
```

Link the current room to an existing room:

```text
cell link <direction> <cell id|@n>
cell set link <direction> <cell id|@n>
```

`cell dig` creates a new room and transfers you into it. `cell link` connects to an existing cell and creates the reverse exit as well.

### Non-Cardinal Exits

Non-cardinal exits use templates such as seeded `Enter`, `Leave`, `Climb`, `Descend`, `StairsUp`, and `StairsDown`. These are useful for doors, gates, ladders, stairs, portals, hatches, gangways, alleys, and other connections that are better described by words than compass directions.

Inspect templates:

```text
show exittemplates
```

Create a new room through a non-cardinal exit:

```text
cell ndig <template> <outbound keyword> <inbound keyword> "<outbound description>" "<inbound description>"
```

Link to an existing room through a non-cardinal exit:

```text
cell nlink <template> <cell id|@n> <outbound keyword> <inbound keyword> "<outbound description>" "<inbound description>"
cell set nlink <template> <cell id|@n> <outbound keyword> <inbound keyword> "<outbound description>" "<inbound description>"
```

Example:

```text
show exittemplates
cell ndig Enter doorway street "a narrow oak doorway" "the street outside"
cell set name "Inside the Old Shop"
cell set terrain "Shopfront"
cell set type indoors
```

From the street, the outbound keyword is `doorway`. From inside, the inbound keyword is `street`.

### Editing Existing Exits

List exits:

```text
cell exit list
cell exit list <direction|keyword>
cell exit list all
```

Add or remove overlay exits:

```text
cell exit add <direction|keyword>
cell exit remove <direction|keyword>
```

Configure exit size and posture:

```text
cell exit size <exit> <size>
cell exit upright <exit> <size>
cell exit reset <exit>
```

Door support:

```text
cell set door <exit> <size>
cell set door <exit> clear
```

`cell set door` controls whether an exit accepts a door and, if so, what size. It does not by itself install a specific door item.
Player `look` and `exits` output shows door-capable exits without an installed door in bold white; installed doors still show their door state and description directly on the exit.

Climb and fall:

```text
cell exit climb <exit> <difficulty>
cell exit climb <exit>
cell exit fall <exit>
```

`cell exit climb <exit>` without a difficulty toggles climb off if the exit is already climbable. Fall exits are only valid for up/down movement and are used for vertical exits where falling or flying matters.

Layer blocking:

```text
cell exit block <exit> <layer>
cell exit unblock <exit> <layer>
```

Common layers include `GroundLevel`, `Underwater`, `DeepUnderwater`, `VeryDeepUnderwater`, `InTrees`, `HighInTrees`, `InAir`, `HighInAir`, and `OnRooftops`. Terrain controls which layers are available in a room.

Hidden exits:

```text
cell exit hide <exit> <prog>
cell exit unhide <exit>
```

The hide prog must return boolean and accept a location and character. Hidden exits are still real exits; the prog controls whether a character perceives them.

When an exit is shared by other overlays, edit commands copy it into the current overlay package before changing it. This lets a new package adjust exit behaviour without accidentally mutating live or unrelated overlay data.

## Ways to Make Rooms

### Manual Room Creation

Create a blank room in your current zone and current overlay package:

```text
cell new
```

Create a room from an area autobuilder template:

```text
cell new <area template> <arguments...>
```

Dig one room in a direction:

```text
cell dig north
cell set name "A Bend in the Road"
cell set terrain "Dirt Road"
cell set type outdoors
cell set desc
```

Dig using a non-cardinal exit:

```text
cell ndig StairsUp stairs landing "a flight of wooden stairs" "the lower landing"
cell set name "An Upper Landing"
cell set terrain "Hallway"
cell set type indoors
```

Link to existing rooms:

```text
cell link east 12345
cell nlink Enter 12346 gate courtyard "an iron garden gate" "the courtyard behind the gate"
```

### The `@n` Room Syntax

FutureMUD tracks recently built cells since the last reboot. Builders can refer to these cells with `@n`:

- `@1`: the most recently created room.
- `@2`: the second most recently created room.
- `@3`: the third most recently created room.

This is especially useful for paste-able build scripts where you do not yet know the database IDs.

Example:

```text
cell package new "Gatehouse Chain"
cell new
cell set name "Before the Gatehouse"
cell set terrain "Dirt Road"
cell set type outdoors
cell dig north
cell set name "Inside the Gatehouse"
cell set terrain "Gatehouse"
cell set type indoors
cell ndig StairsUp stairs lower "a tight stairwell" "the gatehouse floor below"
cell set name "On the Gatehouse Stair"
cell set terrain "Gatehouse"
cell set type indoors
cell link south @2
```

In this example, `@1` is the stair room after `ndig`, and `@2` is the room inside the gatehouse. The exact numbering depends on what was created most recently, so keep command chains compact and avoid mixing manual building from multiple builders into the same paste sequence.

### Autobuilders

Autobuilders combine an **area template** with a **room template**:

- Area templates create topology: rectangles, diagonal rectangles, terrain masks, feature masks, cylinders, and other shapes.
- Room templates create presentation: room names, descriptions, terrain-specific text, random feature descriptions, lighting, outdoors type, and forage defaults.

Inspect templates:

```text
show autoareas
show autoarea <id|name>
show autorooms
show autoroom <id|name>
autoarea show <id|name>
autoroom show <id|name>
```

Common seeded area templates include:

```text
Rectangle
Rectangle Diagonals
Terrain Rectangle
Terrain Rectangle Diagonals
Feature Rectangle
Feature Rectangle Diagonals
Seeded Terrain Wilderness Grouped Features
```

Common seeded room templates include:

```text
Blank
Seeded Terrain Wilderness Grouped Description
```

Basic rectangle:

```text
cell new Rectangle 3 4 Blank "Grasslands"
```

Terrain mask rectangle:

```text
cell new "Terrain Rectangle" 3 4 Blank 12,12,13,13,12,0,13,14,12,12,14,14
```

The terrain mask is row-major. It must contain exactly `height * width` entries. Use terrain IDs for rooms and `0` for no room.

Seeded wilderness example:

```text
terrain planner
cell package new "Western Woods"
cell new "Seeded Terrain Wilderness Grouped Features" 3 4 "Seeded Terrain Wilderness Grouped Description" 12,12,13,13,12,0,13,14,12,12,14,14
```

The IDs above are examples. Use `terrain planner` or `show terrain` to get the correct terrain IDs in your world.

Feature mask example:

```text
cell new "Feature Rectangle" 2 3 "Seeded Terrain Wilderness Grouped Description" 12,12,12,15,15,15 "Trail Straight|Roadside Marker,Trail Bend,Wildflowers,,Dense Underbrush,Trail Straight"
```

Feature masks are also row-major. Separate cells with commas and multiple features in one cell with `|`. Feature names must match the room template's expected feature tags.

Area template editing:

```text
autoarea list
autoarea edit new <name> <type>
autoarea clone <old> <new>
autoarea edit <id|name>
autoarea close
autoarea show <id|name>
autoarea show
autoarea set <template-specific setting>
```

Room template editing:

```text
autoroom list
autoroom edit new <name> <type>
autoroom clone <old> <new>
autoroom edit <id|name>
autoroom close
autoroom show <id|name>
autoroom show
autoroom set <template-specific setting>
```

Important room-template concepts:

- `simple` room templates define one default name, description, terrain, light, outdoors type, and forage profile.
- `room by terrain` templates define different names and descriptions per terrain.
- `room random description` templates combine weighted description elements, mandatory elements, fixed positions, terrain restrictions, and feature tags.
- UsefulSeeder wilderness templates use grouped description elements and terrain/feature tags seeded by the installer.

### Post-Build Progs

`cell new <area template>` can run one or more progs on every generated room. Put `prog=<prog>` arguments before the area template name.

```text
cell new prog=DecorateRoad prog=AddDistrictRegisters "Terrain Rectangle" 2 3 Blank 12,12,12,12,12,12
```

The prog must accept either a single location or a collection of locations. This is useful for adding variable register values, applying stock effects, placing signs, or doing post-generation cleanup.

FutureProg also exposes room-building functions for scripted workflows. Useful examples include:

- `CreateCell(package, zone)` and `CreateRoom(package, zone)`.
- `CreateCell(package, zone, template)` and `CreateRoom(package, zone, template)`.
- `NameCell(room, package, name)` and `NameRoom(...)`.
- `DescribeCell(room, package, description)` and `DescribeRoom(...)`.
- `SetTerrain(room, package, terrain)`.
- `SetHearingProfile(room, package, profile)`.
- `SetCellLightMultiplier(room, package, multiplier)`.
- `SetCellAddedLight(room, package, lux)`.
- `SetAtmosphere(room, package, ...)`.
- `SetIndoors`, `SetOutdoors`, `SetCave`, `SetWindows`, and `SetExposed`.
- `LinkCells(location, location, package, direction)`.
- `CreateOverlay`, `ReviseOverlay`, `ApproveOverlay`, and `SwapOverlay`.

Use prog-based building when the build is procedural, event-driven, or must be repeated in-world. Use manual commands or autobuilders when a builder needs tight control and readable command history. For non-cardinal scripted links, prefer a tested helper prog or the normal `cell ndig`/`cell nlink` workflow before bulk-generating content.

## Landmarks and Meeting Places

Landmarks help players orient themselves. Meeting places are landmarks that also help group characters in player-facing location summaries such as `who`.

Key commands:

```text
cell landmark [<prog>] [<sphere>]
cell meeting [<prog>] [<sphere>]
cell landmarktext
cell landmarktext add <prog>
cell landmarktext prog <number> <prog>
cell landmarktext text <number>
cell landmarktext swap <number> <number>
cell landmarktext delete <number>
landmarks
landmarks <landmark>
```

`cell landmark` toggles a landmark on or off. If the room is currently a meeting place, it changes it back to a landmark-only room.

`cell meeting` toggles a meeting place on or off. If the room is currently a landmark-only room, it upgrades it into a meeting place.

Optional landmark progs must return boolean and accept either:

- `Location`
- `Location, Character`

Optional landmark text progs must return boolean and accept `Character`. These extra texts let a landmark display different details based on character knowledge, perception, allegiance, or story state.

The optional sphere lets you segment landmarks. A city can have public landmarks, clan-specific landmarks, staff-only landmarks, or any other sphere convention your game uses.

Example:

```text
cell landmark AlwaysTrue Public
cell landmarktext add CanReadOldImperial
cell landmarktext text 1
This is a common public meeting place for traders and townsfolk. There are signs and posters in old imperial all over the city that describe this as the place to be.
@
cell meeting AlwaysTrue Public
```

## Room Descriptions and Markup

Room names and descriptions support runtime substitutions. These are resolved when a character sees the room.

For deeper markup details, see [Room Description Markup](../Markup/Room_Description_Markup.md).

Substitution order:

1. `environment{...}` blocks.
2. ANSI colour substitution.
3. `@shop`.
4. `check{...}` blocks.
5. `writing{...}` blocks.

### Environment Blocks

Environment blocks allow weather, time, light, and season-aware text.

General form:

```text
environment{qualifiers=text}{qualifiers=text}{fallback}
```

Each conditional branch can have one or more comma-separated qualifiers. All qualifiers in a branch must match for that branch to display. You can supply up to eight conditional branches, followed by an optional fallback branch with no `=`.

Examples:

```text
environment{night=The square lies in darkness.}{dawn=Grey light gathers across the stones.}{The square is open to the sky.}
environment{rain=Rainwater runs along the gutter.}{snow=Snow softens the street.}{Dust gathers along the gutter.}
environment{night,>rain=Black rain shines on the cobbles.}{day,!rain=Sunlight warms the cobbles.}{The cobbles stretch east and west.}
```

Time qualifiers include:

```text
day
night
morning
afternoon
dusk
dawn
notnight
```

Precipitation qualifiers include:

```text
parched
dry
humid
lightrain
lrain
rain
heavyrain
hrain
torrentialrain
torrential
torrent
train
lightsnow
lsnow
snow
heavysnow
hsnow
blizzard
sleet
```

Prefix precipitation with `*` to use recent maximum precipitation instead of current precipitation. Prefix any qualifier with `!` to negate it. Prefix light or precipitation with `>` or `<` to compare against a threshold. Current comparison semantics are `>` for greater than or equal to the threshold, and `<` for less than the threshold.

Light qualifiers come from the active light model. Common stock descriptions include:

```text
pitch black
almost completely dark
extremely dark
very dark
dark
dim
soft
normal
bright
very bright
extremely bright
```

Season qualifiers use the current regional climate season names. Do not assume that all games use the same seasons.

### Writing Blocks

Writing blocks let room descriptions include readable text that depends on language, script, and skill.

General form:

```text
writing{language,script,minskill=<value>,style=<style>,colour=<colour>}{readable text}{unreadable text}
```

`skill` and `minskill` are both accepted. `colour` and `color` are both accepted.

Examples:

```text
writing{English,Latin,minskill=30}{The sign reads "Staff Only."}{A painted sign hangs beside the door.}
writing{Aelvish,Runes,skill=60,colour=green}{The lintel names the old gate.}{Green runes mark the lintel.}
```

### Trait Checks

Trait checks let descriptions show different text based on a character trait threshold.

```text
check{Observation,35}{A narrow drainage channel runs beneath the grate.}{}
check{Foraging,50}{You notice edible leaves growing near the wall.}{The wall is lined with climbing plants.}
```

### Shop Substitution

`@shop` substitutes the current room's shop name, or `An Empty Shop` if there is no shop. This is especially helpful in room names where the shop might be leased or sold to players who could rename the shop at a later date.

```text
The sign above the counter reads @shop.
```

### Colour Markup

Room descriptions pass through ANSI colour substitution. Use existing colour markup conventions from the emote and room markup documentation, and keep descriptions readable without colour as a fallback.

## Complete Key Command Rundown

This section gathers the room-building commands most builders need.

### Discovery

```text
cell show
cell overlay <id|name> [revision]
cell overlay clear
rooms <zone id|name> [+keyword] [-keyword]
zones
shards
area list [zone]
show terrain [+keyword] [-keyword] [*keyword]
show overlays
show exittemplates
show autoareas
show autoarea <id|name>
show autorooms
show autoroom <id|name>
```

### Shards

```text
shard list
shard new <name> <sky template>
shard set <shard> name <name>
shard set <shard> clocks <clock...>
shard set <shard> calendars <calendar...>
shard set <shard> celestials <celestial...>
shard set <shard> sky <sky template>
shard set <shard> lux <minimum lux>
```

### Zones

```text
zone new <name> <shard> <timezones...>
zone show <zone>
zone set <zone> name <name>
zone set <zone> latitude <decimal>
zone set <zone> longitude <decimal>
zone set <zone> elevation <metres>
zone set <zone> light <multiplier>
zone set <zone> timezone <clock> <timezone>
zone set <zone> fp <foragable profile>
zone set <zone> fp clear
zone set <zone> weather <weather controller>
zone set <zone> weather none
zones
rooms <zone id|name> [+keyword] [-keyword]
rezone <zone>
```

### Areas

```text
area list [zone]
area show <id|name>
area new <name>
area edit <id|name>
area edit
area close
area rename <name>
area add
area remove
area weather <id|name>
area weather clear
```

### Overlay Packages

```text
cell package list [all|by <builder>|mine]
cell package new <name>
cell package open <id|name>
cell package close
cell package show [<id|name>]
cell package rename <name>
cell package revise <id|name>
cell package submit <comment>
cell package review list
cell package review all
cell package review <id>
accept edit <comments>
decline edit <comments>
cell package history <id|name>
cell package swap <id|name>
cell package delete
cell package obsolete
```

### Room Creation and Editing

```text
cell new
cell new <area template> <arguments...>
cell new prog=<prog> <area template> <arguments...>
cell dig <direction>
cell ndig <template> <outbound keyword> <inbound keyword> "<outbound description>" "<inbound description>"
cell link <direction> <cell id|@n>
cell nlink <template> <cell id|@n> <outbound keyword> <inbound keyword> "<outbound description>" "<inbound description>"
cell set name <name>
cell set desc
cell set description
cell set suggestdesc
cell set terrain <terrain>
cell set hearing <hearing profile>
cell set lightmultiplier <multiplier>
cell set lightlevel <lux>
cell set type outdoors
cell set type indoors
cell set type cave
cell set type windows
cell set type exposed
cell set door <exit> <size>
cell set door <exit> clear
cell set forage clear
cell set forage <profile>
cell set atmosphere gas <gas>
cell set atmosphere liquid <liquid>
cell set atmosphere none
cell set safequit
cell set register <variable> <value>
cell set register delete <variable>
cell delete
```

`cell set suggestdesc` uses configured AI description generation if the game has an OpenAI or Anthropic API key configured.

`cell delete` is restricted to high administrators, asks for confirmation, and permanently deletes the current room if the engine can find a fallback destination. Treat it as a repair tool, not a normal building command.

### Exits

```text
cell exit list
cell exit list <exit>
cell exit list all
cell exit add <exit>
cell exit remove <exit>
cell exit size <exit> <size>
cell exit upright <exit> <size>
cell exit reset <exit>
cell exit fall <exit>
cell exit climb <exit> <difficulty>
cell exit climb <exit>
cell exit block <exit> <layer>
cell exit unblock <exit> <layer>
cell exit hide <exit> <prog>
cell exit unhide <exit>
```

### Terrain

```text
terrain list [+keyword] [-keyword] [*keyword]
terrain new <name>
terrain clone <terrain> <new name>
terrain edit <terrain>
terrain edit
terrain show <terrain>
terrain close
terrain set <setting> <value>
terrain override <terrain> [<prog>]
terrain reset
terrain planner
```

### Autobuilder Templates

```text
autoroom list
autoroom edit new <name> <type>
autoroom clone <old> <new>
autoroom edit <id|name>
autoroom close
autoroom show <id|name>
autoroom show
autoroom set <template-specific setting>

autoarea list
autoarea edit new <name> <type>
autoarea clone <old> <new>
autoarea edit <id|name>
autoarea close
autoarea show <id|name>
autoarea show
autoarea set <template-specific setting>
```

### Landmarks

```text
cell landmark [<prog>] [<sphere>]
cell meeting [<prog>] [<sphere>]
cell landmarktext
cell landmarktext add <prog>
cell landmarktext prog <number> <prog>
cell landmarktext text <number>
cell landmarktext swap <number> <number>
cell landmarktext delete <number>
landmarks
landmarks <landmark>
```

## Workflow Examples

### Example: First Room in a New Package

```text
cell package new "Harbour Starter"
cell new
cell set name "At the Harbour Gate"
cell set terrain "Cobblestone Road"
cell set type outdoors
cell set desc
cell set lightmultiplier 1.0
cell set safequit
cell package submit "Adds the initial harbour gate room."
```

Use this for a carefully authored first room, tutorial entry, or anchor location.

### Example: Road With Side Alley

```text
cell package new "Harbour Road"
cell new
cell set name "The South End of Harbour Road"
cell set terrain "Cobblestone Road"
cell set type outdoors
cell dig north
cell set name "Harbour Road Beside the Market"
cell set terrain "Cobblestone Road"
cell set type outdoors
cell new
cell set name "A Narrow Market Alley"
cell set terrain "Alleyway"
cell set type outdoors
cell link west @2
```

After the alley room is created, `@1` is the alley, `@2` is the market road room, and `@3` is the south road room. The final command links the alley west to the market road without needing to know the market room's database ID.

### Example: Building Interior Through a Doorway

```text
cell package new "Old Shop Interior"
cell new
cell set name "Outside the Old Shop"
cell set terrain "Cobblestone Road"
cell set type outdoors
cell ndig Enter doorway street "a narrow oak doorway" "the street outside"
cell set name "Inside the Old Shop"
cell set terrain "Shopfront"
cell set type indoors
cell set door street Normal
cell set desc
```

Use non-cardinal exits when movement should read as `enter doorway`, `leave street`, `climb ladder`, or another natural phrase.

### Example: Stairs and Climb Difficulty

```text
cell ndig StairsUp stairs hall "a steep stone stair" "the lower hall"
cell set name "At the Top of the Stone Stair"
cell set terrain "Hallway"
cell set type indoors
cell exit climb stairs Easy
cell exit upright stairs Small
```

This creates a stairs exit and then marks it as climbable and cramped for upright posture.

### Example: Hidden Cellar Hatch

```text
cell ndig Descend hatch pantry "a concealed cellar hatch" "the pantry above"
cell set name "A Low Cellar"
cell set terrain "Cellar"
cell set type cave
cell exit hide pantry CanFindCellarHatch
```

The hide prog controls whether a character sees the exit. The exit can still exist in topology even when many characters do not perceive it.

### Example: Weather-Aware Square Description

```text
cell set name "Fountain Square"
cell set terrain "Cobblestone Road"
cell set type outdoors
cell set desc
```

Description body:

```text
environment{night=The fountain square lies under a dark sky.}{dawn=Grey dawn light spills across the fountain square.}{The fountain square opens around a broad stone basin.}

environment{rain=Rainwater ripples across the fountain basin.}{snow=Snow gathers along the rim of the fountain.}{Clear water moves quietly in the basin.}

writing{English,Latin,minskill=25}{A bronze plaque reads "Founders' Square."}{A bronze plaque is fixed to the fountain.}
```

### Example: Creating a New Zone

```text
cell package new "Harbour Zone"
zone new "Harbour Ward" "Prime Material" local
zone set "Harbour Ward" latitude -33.86
zone set "Harbour Ward" longitude 151.21
zone set "Harbour Ward" elevation 5
zone set "Harbour Ward" weather "Coastal Weather"
cell show
```

The number of time-zone arguments depends on the shard's clocks. Use `shard list`, `zone show`, and command feedback to confirm the expected clocks in your world.

### Example: Area Grouping

```text
area new "Dockside Market"
area edit "Dockside Market"
area add
area weather "Coastal Weather"
area close
```

Add each relevant room while the area is open. Use areas for logical groupings that do not need to be strict geography.

### Example: Terrain Mask Build

```text
terrain planner
cell package new "Western Woods"
cell new "Terrain Rectangle" 4 5 Blank 12,12,12,13,13,12,0,12,13,14,12,12,12,14,14,15,15,12,12,12
```

This creates a 4 by 5 grid. The `0` entry leaves a hole in the second row.

### Example: Seeded Wilderness Package

```text
cell package new "Western Woods Detail"
cell new "Seeded Terrain Wilderness Grouped Features" 4 5 "Seeded Terrain Wilderness Grouped Description" 12,12,12,13,13,12,0,12,13,14,12,12,12,14,14,15,15,12,12,12
```

This assumes the UsefulSeeder stock wilderness autobuilder package has been installed. The generated rooms use terrain-aware grouped descriptions and seeded feature options.

### Example: Landmark and Meeting Place

```text
cell landmark AlwaysTrue Public
cell landmarktext add CanReadOldImperial
cell landmarktext text 1
cell meeting AlwaysTrue Public
landmarks
```

Use a plain landmark for orientation. Use a meeting place when the room should appear as a social gathering location.

## Guidance for AI Agents

When assisting a builder:

- Start by discovering live IDs and template names with `cell show`, `show terrain`, `show exittemplates`, `show autoareas`, and `show autorooms`.
- Do not assume terrain IDs from examples. Use the target world's seeded data.
- Keep an overlay package open before issuing creation or edit commands.
- Prefer compact paste chains with `@n` when creating connected rooms.
- Use quoted names for multi-word package, zone, area, terrain, and room names.
- Do not submit, review, swap, delete, or obsolete a package unless the user explicitly wants that workflow step.
- Use terrain masks from `terrain planner` for grids. Remember that `0` means no room.
- Set terrain before adjusting outdoors type and light special cases.
- For non-cardinal exits, inspect `show exittemplates` first and choose the template whose verbs match the player's command.
- Verify important builds with `cell show`, `cell exit list all`, `rooms <zone>`, and `landmarks`.
- Keep room descriptions readable without colour and without successful markup substitutions.

The best build scripts are readable command histories. A future builder should be able to paste them into a test world, understand the intended topology, and revise the result without knowing C#.

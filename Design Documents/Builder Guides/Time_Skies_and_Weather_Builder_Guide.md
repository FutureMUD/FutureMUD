---
slug: time-skies-and-weather
category: World building
summary: Build decimal and nine-hour clocks, a shipboard calendar, leap rules, suns, moons and regional weather through in-game worked examples.
---

# Building Time, Skies and Weather

Two towns can keep the same hour, see the sun rise at different times and have entirely different weather. A temple can even mark the start of tomorrow while the civil calendar still says it is today. FutureMUD gives each of these decisions its own home. Start by deciding what people in your world should share.

## What belongs where

| Builder decision | Where it belongs | Who shares it |
| --- | --- | --- |
| How long is a day? How fast does it pass during a play session? | **Clock** dimensions and speed | Every calendar and consumer using that clock |
| What hour does the harbour clock show? | **Timezone** on that clock, assigned to the zone | Places using that local offset |
| What is the date called? When does it change? | **Calendar**, its feed clock and its day boundary | People using that calendar; a solar boundary has a single authority location |
| Which bodies are in the sky? Which supplies the part of day? | **Celestials** attached to the shard, in order | The shard; physical observations also depend on geography |
| When does autumn begin? What does a clear autumn morning usually feel like? | **Season** onset on a celestial year and **regional climate** hourly temperatures | Regions reusing those definitions |
| Can a bank of fog follow overcast weather? How long might it linger? | **Weather event** and the **climate model** for that season | Controllers using that model |
| Is there fog here *today*? What might tomorrow bring? | **Weather controller** assigned to the location | Places served by that particular controller |
| Can someone see the moon, read the time, feel the cold or forecast the rain? | Room exposure, actual light and weather, equipment and character checks | The individual observer |

Read the concepts in order, then [build the seeded timekeeping examples](#timekeeping-at-the-builder-prompt) or jump to [the port and valley](#a-coastal-port-and-an-inland-valley), [the sunset festival](#a-sunset-festival-and-a-civil-closing-hour), or [the nine-hour world](#a-nine-hour-world-with-a-copper-sun).

For building, clocks, calendar editing, shards and authored celestials require HighAdmin access. The weather editors require SeniorAdmin. Timezone editing and recurring prog schedules require Admin. Players can inspect their surroundings with `time`, `look sky` and `weather`.

## Clocks, local time and calendars

A **clock** counts seconds, minutes and hours. Its speed is game seconds per real second. With conventional units and a speed of 3, one hour of play advances the world three hours. Changing only the speed preserves the number of hours between sunrise and noon in the game while changing how long a player waits for them. Changing hours per day or minutes per hour also changes the units that schedules, weather profiles and authored sky paths must fit.

A **timezone** belongs to one clock. An eastern province can read one hour ahead of the capital with `timezone new <clock> EAST "Eastern Standard Time" 1` and `zone set <zone> timezone <clock> EAST`. Both clocks on the wall describe the same instant. Crossing midnight can also change the local date. Longitude and timezone are separate choices. A kingdom can impose one legal time throughout its provinces while dawn still travels across its physical landscape.

A **calendar** gives days an order and names. Start with a fixed-month calendar for a world with authored months, weekdays, special days and leap rules. A day can sit outside the normal weekday sequence. An intercalary day or month can appear only in qualifying years. Preview an ordinary year and a leap year before using either in festivals or contracts. For calendars modelled on real traditions the engine also offers tabular lunar, calculated Hebrew, solar-equinox, astronomical lunar and East Asian lunisolar algorithms. The selected algorithm determines the shape of the generated year. In particular, a calculated lunar month need not begin when a player first sees your shard's moon.

A date's **day boundary** can be midnight, a fixed clock time, sunrise or sunset. A solar boundary uses the calendar's authority coordinates. A single festival date can therefore begin throughout a realm at sunset at its temple, even though players farther west can still see daylight. Eras, named year cycles and regnal periods supply ways to display the year. A custom display mask can say “third year of the Queen” while a rent interval continues to count ordinary calendar years.

Use `clock list`, `timezone list`, `calendar list` and `shards` before creating records. A shard carries ordered lists of clocks, calendars and celestials. The first clock and calendar matter to systems that ask a room for its ordinary date and time. Each `shard set <shard> clocks|calendars|celestials ...` replaces that **whole list**. Read back the existing IDs and preserve their order when adding a cultural calendar or moon. A newly created clock already has a primary timezone. A freshly created calendar is just a scaffold with a single 30-day month.

### Designing a clock

Choose the length of a play day first. Divide **hours per day × minutes per hour × seconds per minute** by **game seconds per real second** to get the real seconds in a day. A standard 24 × 60 × 60 clock at speed 4 gives a six-real-hour day. The seeded Zalanthan clock has nine 60-minute hours. At speed 6 its day lasts 90 real minutes. A speed of 6 applied to the conventional clock gives a four-real-hour day. The seeder lets each world choose its speed.

Next choose how inhabitants *describe* the time. Three clock settings do different jobs.

| Decision | Builder controls | Example |
| --- | --- | --- |
| Which hour number should someone read? | `hpd`, `intervals`, `nozero`, digit widths | A 24-hour clock can read 00–23 throughout the day or divide it into two twelve-hour intervals. `nozero true` displays 12 at the start of a twelve-hour interval when the mask uses `$j`. |
| What does the clock face say? | `short`, `long`, `super` display masks | At hour 13, `$h:$m` can say `13:05` while `$j:$m $i` says `1:05 p.m` with the stock UTC settings. `$t` adds the timezone alias. |
| How would someone estimate the hour? | Short/long interval names and `crude` bands | `$c` produces a phrase such as “half past one”. `$C` describes a band such as “early morning”. A village can call hours 4–6 “the milking watch” without moving sunrise. |

Clock mask tokens are case-sensitive and one letter long. `$m` and `$s` give numeric minutes and seconds. `$i` and `$I` give the current short and long interval names. `$h` gives the hour counted from zero across the **whole day**. `$j` counts within the chosen interval. The number of intervals must divide the hours per day evenly. A new clock has short and long masks for AM/PM. Replace both when adopting a different convention. The `time` command uses the long mask for an observer with exact time knowledge, a built-in approximate format for vague knowledge, and the crude bands otherwise. A timepiece has its own display settings.

### Designing a calendar

Write down one ordinary year and one exceptional year before opening the editor. Month **aliases** are useful in date input, short names fit a compact display, full names appear in prose and nominal order locates a month within the year. A named observance and a day outside the week are independent settings. A ship can mark Founder's Day while still counting it on the duty roster. A town can instead give its year-end festival no weekday, so the weekly market resumes on the same weekday it would have reached without the festival.

| Decision | Builder controls | What changes |
| --- | --- | --- |
| From which day do we count? | `epoch <year> <weekday>` and `date <day/month-alias/year>` | The epoch anchors weekday arithmetic. The current date says where play begins on that reckoning. Setting the epoch alone does not set today's date. |
| What makes up a year? | Weekdays, ordered month definitions and `algorithm` | Fixed months use their normal lengths and authored intercalary rules. A calculated calendar uses its algorithm to generate each year's months and lengths from the definitions it expects. |
| How does an exceptional date behave? | `month special`, `month nonweekday`, `intercalary day` and `intercalary month` | Name an existing day, take it out of the weekly count, or add days or months in qualifying years. |
| Which year name should people hear? | `era`, `yearcycle`, `regnal` and the short/long/wordy masks | Eras label dates on either side of year 1. A named cycle repeats an ordered list. A regnal period attaches a ruler's name and year of reign to a span with a start date. |
| When does today become tomorrow? | `dayboundary` and, for a solar boundary, `authority` | Month generation and the hour at which a date rolls over are separate choices. |

Use `calendar set preview <year>` to inspect generated **months and lengths**. `calendar set validate` checks that the current year can be generated. To see a weekday, feast name or era as players will read it, use the calendar's long mask and `time` in a zone with the calendar attached. Build structural changes on a new calendar or a clone first. After each warning, type `accept` before the next command. This can apply even to a newly created record. Enter commands individually at the game prompt. Replace angle-bracketed names with the corresponding records from your world.

## Physical and authored skies

Choose a **physical sky** when the world should have geographic differences in sunrise, seasonal daylight or a moon whose apparent position and phase follow its orbit. Start with the physical bodies installed for the setting. An Earth-like sun and planetary moon are one pairing. A setting on a moon can instead have the parent planet in its sky. Attach the intended bodies to the shard and set the zones' latitude and longitude. In a physical world, putting every zone at the same coordinates makes places hundreds of miles apart share the same view.

Choose an **authored sky** when you want to say where a body appears and when. The shard's sky template supplies sky descriptions, and its minimum lux supplies a floor for ambient illumination. A rail gives it a repeatable path between rise and set. A script gives you timed positions, including pauses and jumps. An authored body has the same apparent position at the same instant everywhere on its shard. A timezone changes the clock reading beside that observation. `celestial types` lists the authored presets. High administrators can create one against a calendar and its feed clock. `celestial new` creates a valid base object; further edits happen in a private draft. `celestial validate` and `celestial preview` let you inspect it before `celestial save` activates the changes. `celestial close` discards unsaved draft edits.

Keep four questions distinct when designing the sky.

- **Where is the body?** Bearing runs clockwise from north in degrees. Elevation measures degrees above or below the horizon. Rise, transit and set are positions on the path.
- **What part of the day is it?** The shard's first eligible celestial supplies dawn, morning, afternoon, dusk or night. Additional bodies can contribute light. Suns default to eligible and authored moons default to ineligible. A low sun may produce twilight without ever reaching morning.
- **How much light reaches someone here?** An authored body can use a lux profile by elevation or a separate light timeline. A moon can scale that light by illuminated fraction. Cloud, terrain, shelter and local lamps influence what a player experiences. A brightly lit street may still be in the night part of the day.
- **What recurs?** A body's path period, phase cycle and year can have different lengths. Separate event markers can drive sky narrative. `time event sunset <sun>` asks for a future event at the observer's coordinates. `celestial event` and `celestial preview` inspect an authored draft relative to its anchor. Neither preview advances the world.

## Seasons, climate and today's weather

Build from definitions toward a living region. A **season** has an onset day in a celestial's year, an internal name, a player-facing display name and an optional group. Different local seasons can belong to `Spring` so a planting rule can recognise both. Reuse the same celestial across a regional climate's seasons. Decide how to handle hemispheres once. Use seasons with the intended local onsets, or use `wc set hemisphere opposite` to shift a common rotation by half a year. Applying both shifts can put summer back where you started.

A **weather event** describes conditions while they last. Set the description someone gets from `weather`, any room description addendum, forecast wording, wind, precipitation, temperature offsets, the proportion of natural light transmitted and whether the sky can be seen. Light transmission and sky visibility are independent. A bright overcast day can hide the sun. Events also have permitted parts of day. A dawn-only mist needs other eligible weather for the rest of the day. A rain event can supply a falling liquid. Optional lightning, residue or atmospheric replacement give events consequences beyond their prose. For distant storm ambience, the atmospheric lightning chance and thunder text are separate from the local strike chance. Inspect inherited settings when cloning a storm.

The **climate model** describes possible changes for each season. It first decides whether to change at a weather tick, then selects an eligible destination by relative weight. A 10% base chance to leave fog describes its persistence per tick. Two eligible exits with weights 3 and 1 give the first three quarters of those departures. Existing alternatives and time-of-day restrictions change that proportion. The tick interval and any accumulated chance after stable weather also affect how long players typically see a condition. Give uncommon events a plausible way in *and* a way out.

The **regional climate** pairs that model with participating seasons and a baseline temperature at each game hour in each season. Author the temperature curve first, then layer event effects and any regional temperature fluctuation over it. To make a valley reliably colder before dawn, change its hourly baseline. To make it colder only when a squall passes, change the squall. The **controller** owns today's season, weather, temperature and future sequence. Two zones using the same controller share that sequence. Two controllers can reuse an identical climate and still develop different weather.

A room normally takes its controller from its zone. A weather override on its current terrain takes precedence, followed by an override on an area containing it, then the zone default. An area override suits a mountain pass inside a larger zone. Changing a shared terrain can affect distant rooms using it. A temporary or saved scene can use FutureProg `setroomweather(room, eventId)` for a supported local event and `setroomweather(room, 0)` to restore regional weather. It leaves the controller's forecast and regional temperature baseline in place. Events requiring five-second processing cannot be applied this way.

## What players can observe and use

Use `look sky` for the visible sky and `look <celestial name>` for a particular visible body. `time` combines local calendar information, the character's ability to tell time and any readable timepieces. An administrator seeing an exact time is a poor guide to what a traveller can tell without a watch. `weather` reports current conditions as experienced at the location. Compare the open quay, a room with windows and a sheltered cellar when writing their descriptions. For a quay description that changes with the weather, write `environment{rain=Rain stipples the water between the moored boats.}{*rain=Water drips from the mooring ropes.}{The mooring ropes stretch taut over the water.}`. The first matching branch wins. Here current rain takes precedence over recent rain, with a fallback for dry weather. See [room description conditions](../Markup/Room_Description_Markup.md).

`weather forecast` gives a narrative outlook; `weather forecast table` presents the same reading. A new observation requires an above-water place where the weather can be observed, including outdoors, through windows or in a climate-exposed interior. A character can recall an existing reading indoors. There is one quality roll per character, controller and controller-local game day. The capability check is `WeatherForecastCapability` and the quality check is `WeatherForecast`. Standard skill setups use Meteorology. Set both if using custom skills. Use `wc set forecast <days>` to choose a horizon from 1 to 30 local game days. Skill and distance into the future affect how accurately the observer interprets it. Asking for a forecast leaves the simulation at the current instant.

Choose the signal that matches the in-world rule. A street lamp's light sensor can respond to heavy cloud before sunset. A temple can observe an astronomical sunset even on a cloudy day. Farmers can use a season group for planting and still experience rain or frost on a particular morning. For the receiving ends of these integrations see [street lighting](Automation_Systems_Builder_Guide.md#street-lights-that-come-on-after-dark), [agriculture](Agriculture_Builder_Workflows.md), [environmental exposure](Environmental_Exposure_Builder_Guide.md) and [skill checks](Skills_Checks_and_Progression_Builder_Guide.md).

## Timekeeping at the builder prompt

The following builds use the same choices as the DatabaseSeeder packages. Each uses a fresh alias so an installed clock or calendar can continue serving the world. Start with [decimal time](#a-republican-decimal-clock) for clock mechanics, [the Mission Calendar](#the-mission-calendar-and-its-duty-roster) for an entire calendar from scratch, or [the Julian leap rule](#a-julian-leap-rule-on-a-civil-calendar) for changing an existing reckoning.

### A Republican decimal clock

In the French Republican package a day has ten hours, an hour has one hundred minutes and a minute has one hundred seconds. Its Paris Mean Time and UTC records both have zero offset. This is a good clock for a setting where a dial reading `4h07m09s` should mean exactly what its markings say. Start from a new clock even if the kingdom already has a conventional clock called UTC.

```text
clock new RDEC "Republican Decimal Clock"
clock set intervals 1
clock set hpd 10
clock set mph 100
clock set spm 100
clock set speed 4
clock set hourdigits 0
clock set minutedigits 2
clock set seconddigits 2
clock set nozero true
clock set intervalname 1 o'clock
clock set intervallong 1 of the clock
clock set short $jh$mm$ss
clock set long $c
clock set super $jh$mm$ss $t
```

Read the short mask in pairs. `$j` prints 4 and the following `h` is literal. `$m` prints 07 followed by the literal `m`, and `$s` prints 09 followed by `s`. Hour digits set to zero means no padding. At the start of the day the same face reads `10h00m00s` because `nozero` applies to `$j`. Use `$h` instead if midnight should read `0h00m00s`. The chosen speed of 4 gives 25,000 real seconds per game day, or **6 hours, 56 minutes and 40 seconds**. The seed package makes this speed a setting choice.

Build the named zones on this clock and set its primary reference. `RDEC` already has a zero-offset zone bearing its own alias. `PMT` supplies the setting's civil name. A `UTC` alias belonging to a different clock cannot serve as this one's timezone.

```text
timezone new RDEC PMT "Paris Mean Time" 0
timezone new RDEC UTC "Universal Time" 0
clock edit RDEC
clock set primary PMT
clock set crude clear
clock set crude add 0 2 night
clock set crude add 2 5 morning
clock set crude add 5 7 afternoon
clock set crude add 7 9 evening
clock set crude add 9 10 night
clock show
```

The package's night band wraps over hour zero. The editor accepts increasing bounds within the day, so enter two night bands. They share the same name, and each has its own early/late progression. If this becomes the civil clock for a district, attach it to that shard, set `zone set <district> timezone RDEC PMT`, and attach a calendar using RDEC. A calendar still fed by the old clock continues to show the old hours through `time`. [Stock UTC and decimal definitions](../../DatabaseSeeder/Seeders/TimeSeeder/TimeSeeder.cs)

### Giving the nine hours of Zalanthas their names

The Armageddon package takes a different approach. It keeps familiar minutes and seconds, but inhabitants speak of “before dawn”, “high sun” and “late at night”. Give **each hour one interval**. On a fresh clock use the following sequence, accepting the structural change when prompted.

```text
clock new ZHOUR "Zalanthan Hour Clock"
clock set intervals 1
clock set hpd 9
clock set intervals 9
clock set speed 6
clock set hourdigits 1
clock set nozero false
clock set short $h:$m:$s
clock set super $h:$m:$s $t
clock set long $i
clock set crude clear
```

For each row below, enter `clock set intervalname <slot> <name>`, `clock set intervallong <slot> <name>` and `clock set crude add <from> <to> <name>`. The names in the last column are the complete trailing text. **Interval slots start at 1 and clock hours start at 0.**

| Slot | From hour | To hour | Name |
| ---: | ---: | ---: | --- |
| 1 | 0 | 1 | before dawn |
| 2 | 1 | 2 | dawn |
| 3 | 2 | 3 | early morning |
| 4 | 3 | 4 | late morning |
| 5 | 4 | 5 | high sun |
| 6 | 5 | 6 | early afternoon |
| 7 | 6 | 7 | late afternoon |
| 8 | 7 | 8 | dusk |
| 9 | 8 | 9 | late at night |

For instance, `clock set intervalname 5 high sun` puts “high sun” in the long display at 4:30. `clock set intervallong 5 high sun` supplies that name to formats using the long interval label. `clock set crude add 4 5 high sun` gives an unskilled observer a corresponding time band. To use the package's timezone name, enter `timezone new ZHOUR ZST "Zalanthan Standard Time" 0`, `clock edit ZHOUR` and `clock set primary ZST`. A watch can show `4:30:00` at the same instant. These are clock labels and keep their hours even underground. The [Copper Reach sky](#a-nine-hour-world-with-a-copper-sun) shows how to make a sun move through a nine-hour day.

The same setting calls its 77-year cycle the **King's Age**. On a local clone of an installed Zalanthan calendar, use `calendar set yearcycle show` to inspect the ordered names and preserve all 77. Its long mask uses `$yn` for the year name, `$yi` for its position, `$yc` for the cycle's name and `$ya` for the cycle number. At epoch year 1 the first name is **Jihae's Anger**. It returns at year 78, now year 1 of King's Age 2. A shorter original cycle can be built on a calendar of your own with `calendar set yearcycle create 1 "Ship's Watch" "Voyage" "Return" "Refit"` and `calendar set long $ww, $dt of $mf, $yn (year $yi of $yc $ya)`. The third and fourth years then display Refit and Voyage respectively. [Zalanthan hour and year names](../../DatabaseSeeder/Seeders/TimeSeeder/TimeSeeder.Armageddon.cs)

### The Mission Calendar and its duty roster

A generation ship needs every watch officer to agree on the roster, even after months away from a planet. The Mission package gives it ten 36-day months. Six weekdays divide each month into exactly six weeks. Reassignment Day falls on 18 Lux and Founder's Day on 36 Nexus. Both consume a normal day in the roster. Use an ordinary 24-hour ship clock for this example.

**Build the six-day week.** A new calendar has seven named weekdays and one month. Rename six weekdays in place and remove the spare. In the stock definition, the epoch is year 0 and its first weekday is **Core**, the second name in the list. Using the name at the prompt avoids translating the stored index.

```text
calendar new MSROSTER "Mission Roster" <ship-clock> "The Mission Calendar"
calendar set weekday rename 1 Axis
calendar set weekday rename 2 Core
calendar set weekday rename 3 Drift
calendar set weekday rename 4 Flux
calendar set weekday rename 5 Pulse
calendar set weekday rename 6 Rest
calendar set weekday remove 7
calendar set epoch 0 Core
```

**Build the months.** Reuse the scaffold month as Ignis, then add the other nine in order. In `month add`, enter the short name first, the alias second and the full name third. Use the alias in date inputs such as `1/ignis/42`.

```text
calendar set month alias month ignis
calendar set month short ignis ign
calendar set month rename ignis Ignis
calendar set month days ignis 36
calendar set month add ven ventus Ventus 36
calendar set month add aqu aqua Aqua 36
calendar set month add ter terra Terra 36
calendar set month add lux lux Lux 36
calendar set month add umb umbra Umbra 36
calendar set month add fer ferrum Ferrum 36
calendar set month add sil silica Silica 36
calendar set month add vit vita Vita 36
calendar set month add nex nexus Nexus 36
```

**Name dates and choose what players see.** The short mask below is the ship's filing convention, `year.month.day`. In the long mask, `$nz` supplies the short holiday name and a comma when present, `$ww` supplies the weekday, `$dt` the ordinal day, and `$mf` the full month. `$EE` spells out the appropriate era. Set the operational date on the new calendar after its structure is complete.

```text
calendar set month special add lux 18 "Reassignment Day" Reassignment Day
calendar set month special add nexus 36 "Founder's Day" Founder's Day
calendar set era ancient short b.m
calendar set era ancient long before the mission
calendar set era modern short y.m
calendar set era modern long of the mission
calendar set short $yy.$mo.$dd
calendar set long $nz$ww the $dt of $mf, year $yy $EE
calendar set wordy $nz$ww the $dt of $mf, year $yy $EE
calendar set date 1/ignis/42
calendar set preview 42
calendar set validate
```

The preview should list Ignis through Nexus at 36 days each. The crew files 18 Lux in year 42 as `42.5.18`. Its weekday is Axis, because 1 Ignis is Core and each month is a whole number of weeks. The long date begins “Reassignment Day, Axis”. Use the short filing format for display and `18/lux/42` for builder input. The engine's ordinary year mask `$yy` expresses internal year zero as year 1 of the ancient era. Use a positive example year when showing post-launch paperwork.

For a setting where Founder's Day stands *outside* the duty week, `calendar set month nonweekday add nexus 36` on a clone makes that change. The date keeps its name and the year keeps its 360 dates. Only 359 of them now advance the weekday cycle. The beginning of the next year's roster will move as a consequence. Leave the seeded rule in place if six-day duty rosters should repeat identically every year. [Mission Calendar definition](../../DatabaseSeeder/Seeders/TimeSeeder/TimeSeeder.Calendars.cs)

### A Julian leap rule on a civil calendar

An archivist uses an old civil reckoning in which **every fourth year** has a 29 February, even in a century year. The stock Julian and Gregorian calendars illustrate how much can change while all twelve months keep familiar names. Start with a clone of an installed stock Gregorian calendar that uses a single compound leap rule on February. Preserve its month names, era wording and weekday names. The change below reproduces the Julian **leap rule**. The clone keeps its own epoch and current date.

```text
calendar clone <stock-gregorian> ARCHIVE "Archive Calendar" "The Archive Civil Reckoning"
calendar set preview 1900
calendar set intercalary day remove february 1
calendar set intercalary day add february 1 4
calendar set preview 1900
calendar set preview 1901
calendar set preview 2000
calendar set validate
```

In `remove february 1` the number is a **rule index**. In `add february 1 4` the first number is **how many days to add** and the second is the **year divisor**. Before the change, the Gregorian preview gives February 1900 28 days. Afterwards the same month has 29. February 1901 has 28 under both rules, and February 2000 has 29 under both. The optional offset in `add <month> <days> <divisor> [offset]` shifts the cycle. A divisor of 4 with an offset of 1 selects years 1, 5, 9 and so on. The command creates a simple divisibility rule. Clone a Gregorian source when you need its nested century exceptions. Adding independent four-, hundred- and four-hundred-year rules would add days independently.

### A calculated calendar with a leap month

The seeded Hebrew calendar illustrates a different design. The algorithm chooses between Adar in an ordinary year and Adar I followed by Adar II in a leap year. It also varies Heshvan and Kislev. The seed supplies the matching month aliases and names. A good use of a clone is a second community that follows this reckoning but observes sunset at its own sanctuary. On a world with the Hebrew package and a compatible sun, create its calendar as follows. The coordinates below are the community's choice.

```text
calendar clone hebrew LOCALAM "Sanctuary Calendar" "The Sanctuary Reckoning"
calendar set authority 31.78 35.23
calendar set dayboundary sunset
calendar set preview 5783
calendar set preview 5784
calendar set validate
calendar show
```

The first preview gives twelve months including a single Adar. The second gives thirteen, including 30 days of Adar I and 29 days of Adar II. Both use the same sunset boundary. The boundary governs *when a date begins*. The calculated algorithm governs *which dates exist*. On a fresh build, supply the algorithm's expected month definitions before `calendar set algorithm calculated-hebrew`. Switching a one-month scaffold to that algorithm cannot supply the missing month labels. Cloning preserves the complete vocabulary. Use the [sunset festival](#a-sunset-festival-and-a-civil-closing-hour) for attachment and for checking the authority's sunset in play. [Calculated calendar definitions](../../DatabaseSeeder/Seeders/TimeSeeder/TimeSeeder.Calendars.cs)

## A coastal port and an inland valley

Greyhaven's ferries and Ash Valley's grain wagons run on the same civil time. Sea mist often hides the harbour headland. A farmer up the valley may spend the same morning under a clear sky. Give them one clock, one civil calendar and one physical sun, then two controllers.

In the command blocks, replace angle-bracketed arguments with the names or IDs on your world. Quotes group a name containing spaces. The port and valley can be existing zones on a common shard. For new zones and their first rooms, use the [room building guide](Room_Building_Builder_Guide.md).

### Put the towns on the map

Use `shards` to find the realm and read its current attachments. In this example its existing primary clock, midnight civil calendar and physical sun already suit the setting. Keep those and its existing moon. Put the quay at 51° north, 2° west and the valley at 51.3° north, 1.5° west. Use the same civil timezone in both.

```text
zone set <port> latitude 51
zone set <port> longitude -2
zone set <valley> latitude 51.3
zone set <valley> longitude -1.5
zone set <port> timezone <clock> <civil-timezone>
zone set <valley> timezone <clock> <civil-timezone>
```

### Give the coast and the valley their climates

Use `rc list` and `rc show <id>`. If installed, the northern **Oceanic Temperate** and **Humid Subcontinental** regional climates are good starting points. Choose equivalents that use the port's sun on your world. In each regional climate, identify its model and the ID of its mid-autumn season. Seasonal display names can repeat across profiles. IDs keep the two autumns straight.

Clone the coast's climate **and its model** because we want a new weather pattern here. The valley only needs a different temperature curve, so it can share its original model. A regional climate clone still points at its source model until you change that reference.

```text
clm clone <coastal-model> QuaySky
rc clone <coastal-regional-climate> QuayClimate
rc set model QuaySky
rc set temp <coastal-autumn> 5 9C
rc clone <inland-regional-climate> GrangeClimate
rc set temp <inland-autumn> 5 3C
```

These are baseline temperatures at hour 5, before other effects. Tune neighbouring hours as well if you want the whole early morning to stay cooler. Reuse the installed precipitation and wind events. For the coast's distinctive sea fog, pick a calm, overcast **simple** event already reachable in the coastal autumn. Clone it to retain compatible wind and permitted hours. Give it a recognisable room line and a short phrase the forecast can use.

```text
weatherevent clone <overcast-event> QuayFog
weatherevent set desc A bank of sea fog lies across the harbour.
weatherevent set room Fog beads on the mooring ropes and conceals the headland.
weatherevent set forecast low coastal fog
weatherevent set light 35%
weatherevent show
clm edit QuaySky
clm set season <coastal-autumn> chance QuayFog 10%
clm set season <coastal-autumn> transition <overcast-event> QuayFog 2
clm set season <coastal-autumn> transition QuayFog <overcast-event> 1
```

In the event display, check sky visibility. `weatherevent set sky` **toggles** it; use it once if the inherited event leaves the sky visible. Weight 2 puts the fog in competition with the overcast event's other exits. Weight 1 is enough for the way back because it is the fog's only exit in this example. The fog is now part of this model's autumn; add it to adjoining seasons if it should occur then too.

### Give each region its own weather

Create controllers after setting the zone coordinates. The zone argument supplies initial geography. Assigning the new controller back to the zone is a separate step.

```text
wc edit new HarbourWeather QuayClimate <port>
wc set timezone <civil-timezone>
wc set forecast 7
zone set <port> weather HarbourWeather
wc edit new ValleyWeather GrangeClimate <valley>
wc set timezone <civil-timezone>
wc set forecast 7
zone set <valley> weather ValleyWeather
```

On the quay, `weather` describes the harbour conditions and a navigator with forecasting skill can read the coastal outlook. In the valley, the forecast belongs to ValleyWeather even if today's conditions happen to match. To understand why the fog has yet to appear, use `weather transition` as an administrator on the quay. It lists eligible exits from the **current** event, in the **current** season and part of day. On a clear summer evening the absence of sea fog from that list is exactly what this design calls for.

## A sunset festival and a civil closing hour

Greyhaven holds the Lantern Feast on the fifteenth day of a temple month. Its priests recognise the start of a new date at sunset at the sanctuary. The harbour office makes its closing announcement at the same civil hour every day. Build the festival as a second calendar and the office announcement as a recurring schedule.

### Set the temple's authority

Clone the port's civil calendar so both traditions share month lengths, year and feed clock. Give the temple calendar an identifiable name. Use the sanctuary's latitude and longitude; here it stands beside the quay. Replace `<feast-month>` with an actual month alias from the clone.

```text
calendar clone <civil-calendar> LANT "Lantern Reckoning" "The Reckoning of the Lantern Temple"
calendar set authority 51 -2
calendar set dayboundary sunset
calendar set month special add <feast-month> 15 Lanterns Feast of Lanterns
calendar set long $NZ$ww the $dt of $mf, $yy
calendar set validate
calendar set preview
calendar show
```

Add the new calendar at the end with `shard set <shard> calendars <civil-calendar> <temple-calendar>` on a shard previously using only the civil calendar. Include any others already attached. Keep the civil calendar first for systems that assume the local default. When a structural edit asks for confirmation, read it and enter `accept` before the next edit.

The date boundary uses the **first solar celestial in the world** and the calendar's authority coordinates. Reordering the shard's sun and moon changes which eligible body supplies the part of day, but does not select a different sun for this calendar. Use this build where the physical sun the temple observes is that first solar body. With no authority or usable solar event, the boundary calculation has a midnight fallback. In a multi-sun setting, inspect this relationship before promising a festival on a particular world's sunset.

From the sanctuary an administrator can ask `time event sunset <physical-sun>` for the next sunset. The temple date advances at its authority's astronomical boundary even if the congregation sees only cloud. The `$NZ` token prefixes the long date with “Feast of Lanterns, ” on the named day and contributes nothing on an ordinary day. Preserve other tokens from the inherited mask if you want to retain its cultural wording. Naming the date supplies the observance; any procession, bell or NPC service needs its own behaviour.

### Give the harbour office a fixed hour

Give the clerk a closing announcement at 18:00 civil time. Create a void FutureProg named `HarbourClosingBell` with no parameters. For a harbour office whose room ID is 423, the body can be:

```text
sendloc(ToLocation(423), "The harbour clerk closes the ledger and rings the brass bell for the end of business.")
```

Use the actual office room ID. Compile the prog, then stand in the civil port and give the schedule its reference. For a civil calendar with a month named January the command takes this form. Substitute a reference date appropriate to your world's current year and a timezone alias if required.

```text
schedule add HarbourClosingBell "every 1 day" <civil-timezone> 15/january/1250 6:00:00 pm
```

The reference sets the recurring hour. Omitting it uses the current local date and time. The scheduler takes its calendar and clock from the first entries at the builder's location. The message is all this prog does; a clerk who stops serving customers at that hour also needs the service's availability rule. At midwinter the brass bell might ring well after the priests begin their new day. In summer the same bell can ring in daylight. For a rule that really depends on sunset, FutureProg's `nextsunset` queries the selected body, place and calendar; the result is an instant a caller can use, and a query on its own schedules nothing. See [FutureProg authoring](FutureProg_Builder_Guide.md).

## A nine-hour world with a copper sun

The Copper Reach has a nine-hour day. Its sun climbs from the east for half the day. A pale moon rises as the sun goes down, but provides little light in the dark part of its twenty-day phase cycle. Settlers count a 360-day year in two long seasons. The Reach has its own shard, clock, calendar, sun and weather controller.

### Set the units before drawing the sky

Create a clock with 60 seconds per minute and 60 minutes per hour (the new-clock defaults). Switch to one display interval first. The two default intervals divide 24 hours evenly but cannot divide nine. Use `accept` whenever the clock or calendar asks for confirmation before continuing.

```text
clock new COP "Copper Reach Time"
clock set intervals 1
clock set hpd 9
clock set short $h:$m
clock set long $h:$m $t
clock set intervallong 1 by Reach reckoning
clock set speed 3
clock show
calendar new REACH "Reach Reckoning" <copper-clock> "The Copper Reach Reckoning"
calendar set month days month 180
calendar set month rename month Bright Turning
calendar set month add Dim dim "Dim Turning" 180
calendar set validate
calendar set preview 1
```

The clock masks use `$h` for the zero-based hour and `$t` for the timezone alias. This gives an exact local reading such as `06:00 COP` without an inherited AM or PM label. The scaffold's first month still has the alias `month` after changing its full name. The two 180-day months give the civil year 360 days. With 540 game minutes per day and speed 3, settlers see an entire day pass in three real hours. If the realm does not yet exist, create it with `shard new "Copper Reach" <sky-template>`, using a sky template already installed on the world. On this dedicated shard use `shard set <reach> clocks <copper-clock>` and `shard set <reach> calendars <reach-calendar>`. For a new zone, select a room overlay package under design and use `zone new "Copper Basin" <reach> COP`. For an existing zone use `zone set <reach-zone> timezone <copper-clock> COP`. A zone on the old shard would continue to use that shard's sky.

### Give the sun a rail and the moon a phase

A rail uses integer game minutes from an anchor. `1d` uses this clock's day length. Minute 90 is 01:30 and minute 360 is 06:00 in the primary timezone with anchor zero. A rise bearing of 90° is east, a tilt bearing of 180° gives a southern arc, and the default set bearing is west. Give the sun a 60° maximum elevation and a 360-day year.

```text
celestial new RailSun <copper-clock> <reach-calendar> Copper Sun
celestial set anchor 0
celestial set rail 1d 90 360 90 60 180
celestial set year 360d
celestial set eligible true
celestial validate
celestial preview 90 3 135
celestial event sunset 0
celestial save
```

The three preview rows sample rise, the middle of the daylight arc and set. The year setting supplies the day number used for season onsets. It does not, on its own, lengthen the summer daylight arc. If a consumer needs a predictable synthetic solar longitude, `celestial set longitude 0` gives it a uniform annual longitude starting at zero degrees.

```text
celestial new RailMoon <copper-clock> <reach-calendar> Pearl Moon
celestial set anchor 0
celestial set rail 1d 360 90 90 55 180
celestial set phase regular 20d 0
celestial set light PhaseScaled
celestial set eligible false
celestial validate
celestial preview 450 3 2700
celestial save
shard set <reach> celestials <copper-sun> <pearl-moon>
```

The moon's rise-to-set arc crosses hour zero. The three samples are at the same local hour, five days apart. Read the elevation, phase name and source lux on each line. The moon occupies the same position on all three nights, but its changing phase gives the settlers a bright night, a partly lit one and a dark one. The sun remains the time-of-day authority throughout. Keep the shard's ambient minimum lux low enough for that difference to matter in play.

### Give the Reach seasons and weather that fit its clock

Create two seasons tied to the Copper Sun. Start Bright at day 0 and Dim at day 180. For a small first climate, use two existing simple weather events: one clear and one cloudy, both permitted at every part of day. Read their IDs with `weatherevent list` and check `weatherevent show <id>`. The engine needs an eligible initial event at the local time when the controller is created.

```text
season edit new ReachBright 0
season set display Bright Turning
season set celestial <copper-sun>
season edit new ReachDim 180
season set display Dim Turning
season set celestial <copper-sun>
clm edit new ReachSky
clm set ticks 30
clm set addseason ReachBright
clm set addseason ReachDim
```

Enter the following four lines for each season, replacing `<season>` first with `ReachBright` and then `ReachDim`. In either season clear skies have a 10% base change chance per half-hour tick. Cloud has a 25% base chance to clear. With a single eligible destination each way, weights of 1 suffice.

```text
clm set season <season> chance <clear-event> 10%
clm set season <season> chance <cloudy-event> 25%
clm set season <season> transition <clear-event> <cloudy-event> 1
clm set season <season> transition <cloudy-event> <clear-event> 1
```

Give the climate an hourly curve for each season. `rc set season` toggles membership, so run each addition once on the new regional climate. `rc set temps` writes successive values starting with hour zero. Here there are nine used hours, with the warmest part of the day lagging behind the sun's peak.

```text
rc edit new ReachBasin ReachSky
rc set season ReachBright
rc set season ReachDim
rc set temps ReachBright 11C 10C 12C 16C 20C 22C 19C 16C 13C
rc set temps ReachDim 4C 3C 5C 9C 13C 15C 12C 9C 6C
wc edit new BasinWeather ReachBasin <reach-zone>
wc show
zone set <reach-zone> weather BasinWeather
```

The new controller takes its celestial from the regional climate's first season and, for this authored sun, that celestial's clock and primary timezone. Read those three values back before relying on its outlook. A Reach traveller now has a nine-hour clock, a dawn they can anticipate, nights which differ by moon phase and local forecasts using Reach hours.

### A scripted alternative for a discontinuous sky

Suppose the Reach is an artificial habitat. A lantern appears abruptly above its eastern rim, hangs in place, then disappears. A sparse scripted path expresses this more directly than a rail. Create it as an alternative sun and preview it before deciding which sun to attach.

```text
celestial new ScriptedSun <copper-clock> <reach-calendar> Lantern Star
celestial set anchor 0
celestial set path sparse 1d
celestial set key 0 90 -10 Hold
celestial set key 90 90 -10 Jump
celestial set key 91 90 15 Hold
celestial set key 270 90 15 Jump
celestial set key 271 90 -10 Hold
celestial set key 540 90 -10 Hold
celestial set year 360d
celestial validate
celestial preview 89 4 1
celestial save
```

Read each key's instruction *towards the next key*. The light stays below the horizon through minute 90, jumps up at 91, stays up through 270 and drops at 271. The final key closes the path at its starting position. Use `Travel` for a smooth arc, `Hold` between identical positions and `Jump` for a discontinuity. The path sets position; the body's light profile determines the lux it emits there. If the habitat adopts the lantern, replace the sun in the shard list, keep the moon, and point both seasons and the existing controller's celestial at the lantern as well.

## When the world looks wrong

| What the builder or player sees | Where to look first |
| --- | --- |
| Both towns have identical storms and the same forecast reading | Check each zone's assigned controller, then area and terrain overrides. Cloning a climate alone does not create independent weather. |
| The clock is right but sunrise is in the wrong place | Check physical zone longitude, latitude and local timezone separately. For an authored body, inspect its clock, anchor and path instead. |
| It is officially night but people can see well | Read the authoritative body's part of day, other bodies' lux, weather transmission, shard minimum light and local light sources. |
| A new controller cannot start, or a particular fog never appears | Check season membership in both regional climate and model, positive event chances, permitted parts of day and reachable transitions. Check that the season uses the intended celestial. |
| The temple date changes at an unexpected hour | Inspect the authority coordinates, the world's first solar body, its clock compatibility and whether a solar event can be found. Compare from the sanctuary. |
| A forecast seems unchanged after moving indoors or reopening the table | Recall uses the character's existing reading. To make a new one, the character needs the capability, an observation site and a new local day or another controller. |
| An authored moon's preview is correct but its phase makes no visible difference | Check `PhaseScaled`, the moon's light at that elevation, sky visibility and other sources of light in the room. |

For detailed calendars, date masks and recurrence rules, see [time and date](../World/Time_And_Date_System.md). For numerical sky examples and event capabilities, see [physical celestials](../World/Celestial_System_Overview.md) and [authored celestials](../World/Authored_Celestials.md). For forecast interpretation and hazardous weather, see [the weather system](../World/Weather_System.md).

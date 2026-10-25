# Armageddon time and celestial proposal

Research date: 24 October 2026. Status: lore-led adaptation approved and implemented on 24 October 2026. The named year cycle stores 77 literal, pre-combined names; there is no name-combination runtime architecture. The calendar is recoverable, but the supplied lore and historical implementation do not uniquely specify one celestial model.

## Source provenance

The accessible [Armageddon Skills Investigation Page](https://chatgpt.com/space/page_9f8b5dcf33288191a0090552ea5726a4) identifies the supplied Library original as `codedump.c`, 8,354,060 bytes, SHA-256 `e44778f6a5fb9fd02db5c3ff78d4c468b93ae6c854a058d5af769dc0a95cd59f`. This investigation read `C:/Users/luker/Downloads/codedump.c` and independently obtained that same SHA-256. Thus the local copy is byte-identical to the original described by the Page; the Page's raw Library attachment was not fetched in this task.

Historical source lines below refer to that file, not current live Armageddon. The public [Time page](https://rpimud.com/armageddon/world/time.php) agrees on the main calendar and ten-real-minute hours. The [What You Know page](https://rpimud.com/armageddon/intro/what-you-know/) identifies Jihae as the red moon, Lirathu as the white moon, and Suk-Krath as the sun. Public pages are corroboration, not evidence of this dump's numerical astronomy.

## Recovered calendar and clock

| Property | Recovered value | C source |
| --- | --- | --- |
| Hours/day | 9 | 24974-24986, 27274-27283 |
| Real seconds/game hour | 600 | 24979, 241987-241990 |
| Days/month | 231 | 24976, 24981 |
| Months/year | 3; Descending Sun, Low Sun, Ascending Sun | 21073-21076, 24977 |
| Days/year | 693 | product of the preceding dimensions |
| Week | 11 days; Ocandra, Terrin, Abid, Cingel, Nekrete, Waleuk, Yochem, Huegel, Dzeda, Barani, Detal | 24610-24621, 72330, 72373 |
| King's Age | 77 years | 72331-72334 |

Hour labels at indices 0-8 are before dawn, dawn, early morning, late morning, high sun, early afternoon, late afternoon, dusk, late at night (72336-72362). The requested predawn/night wording is an equivalent presentation choice. Light is dark in hours 8 and 0, low in 1, 2, 6 and 7, high in 3 and 5, and highest in 4 (29552-29569; 242037-242079). This establishes seven light/twilight hours and two dark hours, not a numerical solar altitude curve.

The weekday array starts with Detal because the caller indexes it using the **one-based** day modulo 11; day 1 is Ocandra. This is not evidence to start the new calendar on Detal.

Recommendation: a nine-hour clock with nine named hour intervals, conventional internal 60 minutes/hour and 60 seconds/minute, and default speed **6 game seconds per real second**. That gives exactly 600 real seconds/hour and 90 real minutes/day. The 60-by-60 subdivision is a FutureMUD representation choice; the dump does not define player-facing minutes. The existing TimeSeeder prompt's statement that Armageddon used 24:1 does not yield this speed with a nine-by-sixty-by-sixty clock.

The dump ties its date to Unix time: raw `time_info.year = 1259 + unixSeconds / RT_ZAL_YEAR`. Its displayed absolute year is raw year plus one. For a standalone preset, recommend canonical year 1 as Year 1 of Age 1, with an explicit initial date rather than silently synchronising every installation to Unix time. Historical Unix synchronisation would be a separate, deliberate choice.

## Recovered year names

The historical C source generates the names from two lists, rather than storing 77 separate entries. FutureMUD deliberately seeds the resulting 77 names as literal ordered data. At 21049-21070 the two ordered lists are:

- Eleven possessors: Jihae, Drov, Desert, Ruk, Whira, Dragon, Vivadu, King, Silt, Suk-Krath, Lirathu.
- Seven qualities: Anger, Peace, Vengeance, Slumber, Defiance, Reverence, Agitation.

For canonical absolute year `Y >= 1`, set `n = Y - 1`:

```text
name       = possessors[n mod 11] + "'s " + qualities[n mod 7]
age        = floor(n / 77) + 1
yearInAge  = (n mod 77) + 1
```

This matches the raw zero-based year operations in `cmd_time` (72331-72334, 72384-72385). Both lists advance every year; exhausting all qualities for one possessor before advancing the possessor would produce the wrong order. Since 7 and 11 are coprime, all 77 combinations occur exactly once per Age.

| Year | Name | Year | Name | Year | Name |
| --- | --- | --- | --- | --- | --- |
| 1 | Jihae's Anger | 27 | Whira's Reverence | 53 | Silt's Slumber |
| 2 | Drov's Peace | 28 | Dragon's Agitation | 54 | Suk-Krath's Defiance |
| 3 | Desert's Vengeance | 29 | Vivadu's Anger | 55 | Lirathu's Reverence |
| 4 | Ruk's Slumber | 30 | King's Peace | 56 | Jihae's Agitation |
| 5 | Whira's Defiance | 31 | Silt's Vengeance | 57 | Drov's Anger |
| 6 | Dragon's Reverence | 32 | Suk-Krath's Slumber | 58 | Desert's Peace |
| 7 | Vivadu's Agitation | 33 | Lirathu's Defiance | 59 | Ruk's Vengeance |
| 8 | King's Anger | 34 | Jihae's Reverence | 60 | Whira's Slumber |
| 9 | Silt's Peace | 35 | Drov's Agitation | 61 | Dragon's Defiance |
| 10 | Suk-Krath's Vengeance | 36 | Desert's Anger | 62 | Vivadu's Reverence |
| 11 | Lirathu's Slumber | 37 | Ruk's Peace | 63 | King's Agitation |
| 12 | Jihae's Defiance | 38 | Whira's Vengeance | 64 | Silt's Anger |
| 13 | Drov's Reverence | 39 | Dragon's Slumber | 65 | Suk-Krath's Peace |
| 14 | Desert's Agitation | 40 | Vivadu's Defiance | 66 | Lirathu's Vengeance |
| 15 | Ruk's Anger | 41 | King's Reverence | 67 | Jihae's Slumber |
| 16 | Whira's Peace | 42 | Silt's Agitation | 68 | Drov's Defiance |
| 17 | Dragon's Vengeance | 43 | Suk-Krath's Anger | 69 | Desert's Reverence |
| 18 | Vivadu's Slumber | 44 | Lirathu's Peace | 70 | Ruk's Agitation |
| 19 | King's Defiance | 45 | Jihae's Vengeance | 71 | Whira's Anger |
| 20 | Silt's Reverence | 46 | Drov's Slumber | 72 | Dragon's Peace |
| 21 | Suk-Krath's Agitation | 47 | Desert's Defiance | 73 | Vivadu's Vengeance |
| 22 | Lirathu's Anger | 48 | Ruk's Reverence | 74 | King's Slumber |
| 23 | Jihae's Peace | 49 | Whira's Agitation | 75 | Silt's Defiance |
| 24 | Drov's Vengeance | 50 | Dragon's Anger | 76 | Suk-Krath's Reverence |
| 25 | Desert's Slumber | 51 | Vivadu's Peace | 77 | Lirathu's Agitation |
| 26 | Ruk's Defiance | 52 | King's Vengeance | | |

## What the dump actually does with the sky

The two requested moons are Jihae and Lirathu. The dump also contains optional Echri, a black moon (27290-27300, 72672 onward). The requested package should contain only Suk-Krath, Jihae and Lirathu.

Sky positions are integer day states:

```text
Jihae position   = zeroBasedDayOfMonth mod 7
Lirathu position = zeroBasedDayOfMonth mod 11
```

These are set at startup and updates (29580-29582, 242011-242013). Jihae is low at states 2 and 5, high at 3 and 4; Lirathu is low at 3 and 8, high at 4-7 (27295-27298, 72639-72670). Eastern/western descriptions split at states 4 and 6 respectively (72575-72584). These are multi-day sky passages; they are not 7-day and 11-day physical orbits derived from orbital elements.

Their **joint position state** repeats every `lcm(7,11) = 77` days; a 231-day month has exactly three repeats, and a 693-day year has nine. Resetting day-of-month does not break these cycles because 231 is divisible by both periods. However, recurrence of a pair of integer states does not prove equal angular positions, a solar conjunction, or a midnight event.

The moon phase descriptions are separate hard-coded day-of-month tables (72593-72636):

- Lirathu is new on days 1-2 and 230-231, full on 115-118. Its table repeats every 231 days.
- Jihae is new on 49-51 and 164-167, full on 106-109 and 222-225. It has two unevenly discretised cycles per 231 days; 115.5 days is a useful approximation, not an exact transcription.

Rise/set announcement hours are randomised static variables (242015-242035, 242085-242102). They do not establish deterministic moon trajectories. Solar light follows the hour switch above. Searches of uses of `time_info.month` and the solar phase constants found no numerical annual solar altitude/declination model. The three month names alone do not specify maximum elevations, bearings or latitude.

Thus the dump does not resolve the lore's exact geometric conjunction claim or the shape of the three arcs. Preserving its slow position cycles, independent phase tables and all of the lore's astronomical claims would require an authored adaptation.

## Existing FutureMUD fit

- [TimeSeeder](../../DatabaseSeeder/Seeders/TimeSeeder/TimeSeeder.cs) can emit a new clock; [its calendar helper](../../DatabaseSeeder/Seeders/TimeSeeder/TimeSeeder.Calendars.cs) already supports arbitrary weekday lists and fixed-length months. No special calendar algorithm is needed for the dates.
- [Clock](../../MudSharpCore/TimeAndDate/Time/Clock.cs) supports dimensions and named hour intervals. Named time presentation can use those existing facilities.
- [Calendar date formatting](../../MudSharpCore/TimeAndDate/Date/Calendar.cs) has numeric years, eras and finite regnal periods, but no reusable cyclic-year names/age counters. Add optional generic cycle metadata and date-mask fields, with the Armageddon lists in seeded data. Keep the canonical year continuous; do not reset stored dates every 77 years or hard-code setting names into runtime.
- [Physical Sun](../World/Celestial_Type_Sun.md) and [PlanetaryMoon](../World/Celestial_Type_PlanetaryMoon.md) need orbital elements not supplied by the lore or dump. Physical altitude also varies with observer geography. Merely entering periods 693, 7 and 11 would not establish all requested properties.
- [Authored celestials](../World/Authored_Celestials.md) support geography-independent sparse sky paths, independent phase/light tracks, annual metadata and scheduled events. This is the closest existing mechanism for explicitly prescribed fantasy skies. Phase can preserve the recovered tables without pretending it follows physical solar separation.

## Approved adaptation

The implemented choice is a **lore-led authored sky**, with the recovered clock/calendar/year naming and exact source phase tables. The visible trajectories are explicitly authored; they do not reproduce the dump's slow 7/11-day sky passages.

1. TimeSeeder `Armageddon`: the clock/calendar above; canonical year 1 is Year 1 of Age 1, first date is Ocandra, day 1 of Descending Sun. Preserve normal configurable clock speed, recommending the exact historical 90-real-minute day.
2. CelestialSeeder `Armageddon`: install all three objects as one additive stock-owned package, preserve builder edits on rerun, and use a compatible Armageddon clock/calendar. A different-length calendar cannot silently redefine this package's months/arcs. Follow the current seeder's explicit shard-attachment policy.
3. Anchor the common sky schedule at midnight opening day 1 of Descending Sun. Put subsequent conjunctions at midnight after 77, 154 and 231 elapsed days, and repeat every month. Assign boundary events to the interval `(monthStart, nextMonthStart]` so the midnight ending a month is its third conjunction; do not count the shared starting instant twice.
4. Use a shared closed sky track, shaped for seven hours above the horizon and two below. Midnight is the middle of the below-horizon section. Therefore a literal same-direction conjunction with the sun at midnight is **below the horizon**, not three visible discs in the night sky. A visible moon-moon conjunction opposite the sun would be a different interpretation.
5. Proposed solar maximum elevation: descend smoothly from 60 to 30 degrees during Descending Sun, remain at 30 in Low Sun, ascend to 60 in Ascending Sun. Rise east, set west. These values and bearings are editable artistic defaults, not recovered canon. They change the arc without changing the seven-to-two day/night ratio. Geography-independent appearance matches the original global sky model.
6. Preserve the exact source lunar phase day bands using independent authored phase tracks. Source phase names can be mapped to representative native phase values, with Hold/Jump transitions at day boundaries. This preserves the descriptive timetable, not an unrecorded physical illuminated fraction. Light levels and disc sizes need declared starter tuning; no measurements were recovered.

### One constructive solution for the convergence maths

Let `t` be elapsed days from the shared midnight anchor. At a given instant, give all three objects the same closed, non-self-intersecting sky track, with solar-season-dependent tilt. Parameterise turns around it as:

```text
Suk-Krath: S(t) = t
Jihae:     J(t) = 6t / 7
Lirathu:   L(t) = 10t / 11
```

Take each turn modulo one before mapping it onto the track. The track mapping must be one-to-one around its full circle. Give its above-horizon half 7/9 of the parameter interval, and its below-horizon half 2/9. Put parameter zero at the middle of the dark part. This nonuniform apparent speed creates the requested daylight ratio independently of the chosen peak elevation.

Then all three positions coincide exactly when both `S-J=t/7` and `S-L=t/11` are integers. Since 7 and 11 are coprime, this happens precisely at `t=77k`. Every such instant is an integer day, hence midnight. The moons' **relative-to-sun** cycles are 7 and 11 days; their apparent whole-track passage periods are 7/6 and 11/10 days. This distinction is essential: this is a deliberate replacement of the dump's slow sky passages, not evidence that its 7 and 11 were synodic periods.

At 693 days the turn counts are respectively 693, 594 and 630, all integers. Together with the reset of the annual tilt and independent month phase tracks, all sky channels close at the year boundary. There are nine conjunctions in `(0,693]`.

Represent this solution with generated sparse `ScriptedSun`/`ScriptedMoon` definitions over a 693-day period, including exact common keyframes at each intended conjunction and solar horizon keys at each daily rise/set. Authored paths interpolate spherical segments and hold authoritative values between minute boundaries, so they approximate the continuous construction. The implementation must verify the resulting actual samples, including absence of additional conjunctions, rather than claiming the formula alone proves the compiled interpolation. Avoid a new setting-specific celestial runtime class.

### Units and verification boundary

With the proposed clock, one day is 540 feed-clock minutes:

| Interval | Feed-clock minutes |
| --- | --- |
| Light portion of one day | 420 |
| Dark portion of one day | 120 |
| 7 days | 3,780 |
| 11 days | 5,940 |
| 77 days | 41,580 |
| 115.5 days (approximation only) | 62,370 |
| 231 days | 124,740 |
| 693 days | 374,220 |

A dense annual path exceeds the default 100,000-entry budget; generated sparse paths need only several keys per apparent revolution plus boundary/season keys. Exact definition size, interpolation quality and limits remain implementation checks.

Research arithmetic checked: all 77 names above, the 6:1 clock conversion, and the common 7/11 cycle boundaries over all 374,220 minute indices of a year. This is not runtime or seeder qualification.

Implementation acceptance should cover calendar/weekday/month/year/Age boundaries, all 77 names and repeat at year 78; ten-real-minute hours; all 693 solar day/night intervals; exact source phase bands; intended sampled conjunctions and no extras over a full annual schedule; negative anchors and year seams; package selection/prerequisites; preserving reruns; Debug replay question inventory; Debug/Release builds and affected seeder/runtime suites. A native database/server check must be reported separately if executed.

## Decision

The approved choice is the lore-led authored adaptation, including its below-horizon midnight conjunctions, editable 60/30-degree arcs, and replacement of the historical slow moon sky passages. The alternative is a closer transcription of the dump's slow multi-day moon positions and independent phases, documenting the 77-day recurrence as lore rather than promising literal geometric conjunctions. The owner approved the lore-led alternative. The named year cycle must contain 77 pre-combined literal names; the generic runtime does not contain the two-list combination architecture. The combination formula above records historical provenance only.


## Implementation verification

The library suite passed 811/811; the core suite passed 8,388/8,388; DatabaseSeeder passed 2,204 with three Unix-permission launcher cases skipped on Windows. This includes every authoritative minute in the 693-day sky schedule (nine conjunctions and no extras), all 693 solar rise/set intervals, every source lunar phase band and intraday holds, calendar/month/Age boundaries, all 77 labels, legacy XML and named-cycle roundtrips, builder edits and preserving reruns. The seeder Debug and Release builds passed; metadata inspection found replay types in Debug and absent in Release. Four native-wrapper cleanup tests passed. Prior iteration failures and the timeout remain in local artifacts; final core verification followed corrections to an unrelated incomplete legal-authority mock and stale NamePicker title assertion, with no corresponding runtime changes.

Native MySQL run `run-37bf80360bf1` passed the complete maintained Medieval replay and unchanged nonblank-target refusal (all-table before/after digests matched). It then installed Armageddon through the shared executor, verified saved answers and fresh-EF-context reloads, and preserved edited names, alias capitalisation, date/time, speed, renamed/nonzero-offset timezone and retained celestial IDs while restoring a missing member. No Armageddon body was attached automatically. Owned MySQL shutdown passed; data and receipts remain under `.artifacts/authored-celestials/run-37bf80360bf1/`. This is persistence qualification, not game-server startup or Telnet gameplay qualification; those were not run.

### Weather timing and regional adaptation

WeatherSeeder now resolves daylight timing from a supported celestial and its feed clock for fresh installs. It samples its conventional daily curve by day fraction, scales 365-day season onsets to the celestial year, and converts multi-day fluctuation periods using native minutes per day. Existing catalogues receive no automatic retiming, including during hazards updates. An existing world's custom tables and season dates remain builder-owned.

Full Armageddon setting installation optionally adds Southern Desert, Salt Flats, Northern Scrubland and Northern Forest weather templates. Nine 77-day blocks subdivide the three sun arcs without changing months. These temperature, wind and dust profiles are explicitly fictional editable defaults agreed for this adaptation. They are not recovered source observations: the historical regional weather file is absent. Wind/dust probabilities have no invented monthly source seasonality. The package reuses FutureMUD weather and hazard machinery and does not reproduce the historical global wind-front/advection engine, diseases, ecology or tides. All four regions use the shared sun-arc phase unless a builder explicitly chooses opposite-hemisphere behavior. Exact source authoring and qualification targets are in the [setting guide](../Seeding/Seeders/ArmageddonSettingSeeder.md#regional-weather).

The subsequent weather implementation passed all 50 climate regressions (including 12 profile/seed runs), 52 final runtime weather tests and native MySQL qualification `run-7fabf71fffc7`. Existing weather timing remained unchanged through refusal and hazard updates. Full/focused seeder results, measured occupancy ranges, supported controller creation in mixed-clock worlds and superseded fixture attempts are documented in the [weather verification report](../World/Weather_Verification.md#non-terrestrial-timing-and-armageddon-weather--25-october-2026).

# Weather forecasts and hazardous weather

## Builder and player workflow

`weather` retains its current-conditions output. `weather forecast` presents a narrative outlook; `weather forecast table` presents the same remembered reading as a table. Today is divided into astronomical parts of the day, followed by daily outlooks, approximate temperature ranges, winds, and possible hazards. Older readings identify when they were made and rebase their period labels when recalled.

A new observation requires visible outdoor conditions: outdoors, through windows, or in a climate-exposed interior, above water. A character may recall an existing reading indoors. There is one quality check per character, weather controller, and controller-local game day. Changing views, logging out, restarting, or changing the actual weather does not reroll that reading. A new outdoor observation on a later day replaces it; indoor recall is available within its recorded horizon.

`WeatherForecastCapability` is a static capability check, analogous to exact/vague time knowledge. `WeatherForecast` is the quality check. Standard skill packages install **Meteorology** and bind these checks to it; other skill setups can bind their own trait expressions. Existing installations without these checks load disabled expressions (`0`) so adding the engine feature does not silently grant knowledge. Configure the expressions or install/configure an appropriate skill before enabling forecasts in such a world.

Quality changes the interpretation, not the scheduled weather. Difficulty and uncertainty increase with distance. Failed readings can substitute climate-compatible weather and temperature estimates, so a confidently presented outlook can be wrong. The saved effect contains the resulting interpretation; it never exposes exact transition times, strike targets, or damage rolls.

## Authoritative schedule

Creating a weather controller requires an initial weather event permitted for its current season and local time of day. If the regional climate has no such event, creation returns a builder error without inserting the controller or subscribing it to clock/heartbeat updates. Add a permitted event to the climate before retrying; creation does not override event time restrictions.

Each weather controller owns a rolling queue of climate-processing checkpoints. `weathercontroller set forecast <days>` configures its horizon from 1 to 30 local game days; the default is 7. Growing the horizon extends the existing future. Shrinking it limits what is shown without discarding an already scheduled future.

Queues are limited to 200,000 checkpoints. Horizon settings that would exceed this limit are rejected. If a later clock or climate-interval edit makes the configured horizon too large, live weather continues one checkpoint at a time and player forecasting remains unavailable until the configuration is reduced.

The controller uses a private, version-stable random stream to select weather and temperature drift. Ordinary minute processing consumes those exact checkpoints. Forecast queries may extend the queue, but never advance clocks, deliver echoes, strike targets, wet items, or invoke world effects. Additional hourly samples describe persistent conditions and daily temperatures when climate checkpoints are far apart.

The queue, random state, current-state validation data and minute anchor are stored in `WeatherControllers.ForecastState` (`LONGTEXT`); the horizon is stored in `ForecastHorizonDays`. Invalid or incompatible queues rebuild from current durable weather. There is no offline tick or damage catch-up. Character readings use the persistent `WeatherForecastReading` effect.

Future astronomy samples the controller's own celestial frame and geography. Physical Sun, PlanetaryMoon, SunFromPlanetaryMoon and PlanetFromMoon frames retain their existing time-of-day authority; authored rail/scripted celestials use their pure future evaluator. Controller timezone, nonstandard day lengths and opposite-hemisphere season shifts remain authoritative. Climate analysis shares weighted selection and supplies its own random stream for temperature drift; authored celestials are also supported by analysis.

Forced weather, freezing/unfreezing, relevant builder changes, authored celestial activation and administrative clock or calendar-date changes invalidate the affected future. Calendar date edits also rebase the daily reading key. Issued readings remain memories of the old prediction. The stock hazard updater clears saved schedules for changed climate models. Tools that directly edit weather definitions in SQL must likewise clear affected `ForecastState` values or use the runtime builder paths.

## Combinable event behaviours

Both `simple` and `rain` events support the optional `Hazards` element in their existing XML. Missing metadata means harmless legacy weather. Rain remains a rain subtype when cloned or saved, including its liquid and normal soaking/puddle behaviour.

Use `weatherevent edit <event>` followed by these settings:

| Setting | Meaning |
| --- | --- |
| `forecast <description>` | Short forecast phrase, such as `thunderstorms` |
| `atmosphere <gas>` / `atmosphere none` | Temporary ambient gas override |
| `lightning chance <0..1>` | Local-strike probability per outdoor cell per game minute |
| `lightning atmospheric <0..1>` | Flash/thunder probability when no local strike occurs |
| `lightning damage <damage> <pain> <stun>` | Electrical injury channels for a direct hit |
| `lightning targets <ground> <character> <item>` | Relative weights among available target categories |
| `lightning ground <0..1>` | Ground-level splash fraction; zero disables splash |
| `lightning distance <0..100>` | Thunder propagation distance budget |
| `lightning flash <text>` / `lightning thunder <text>` | Configurable echoes |

Lightning uses ordinary electrical wounds and health processing. Direct targets include exposed characters and loose items. Contained/carried items, submerged targets and targets underneath larger shelter are excluded. Protected interiors, including windows and climate-exposed interiors, do not receive direct strikes. A ground hit can injure exposed ground-level occupants/items according to the configured fraction. Visual flashes obey perception; thunder uses the existing bounded topological sound propagation. Hazard ticks run only for the cell's effective controller, preventing inactive area/zone/terrain controllers from applying duplicate effects.

Atmosphere resolution is: applicable explicit atmosphere effect, then exposed weather gas, then the permanent overlay atmosphere. Weather affects outdoors and climate-exposed interiors; windows and sealed interiors retain their atmosphere. Leaving the event or changing controllers restores the underlying atmosphere without rewriting overlays. Submerged breathing still uses the terrain's water fluid, and ambient gas contact excludes underwater layers. Environment/effect transitions settle prior exposure before refreshing affected cells.

## Stock installation and preservation

The Weather Seeder offers `install` for a fresh catalogue and an explicit `hazards` update for an existing world. Fresh installation includes hazard variants automatically. The update builds a disposable in-memory canonical reference, normalizes generated event/season/liquid identities, and adopts only matching legacy stock definitions. It records climate-graph ownership in `SeederManagedRecords`. Custom climate graphs and unowned naming collisions are reported and preserved; controller assignments, regional temperatures and rain/puddle preferences are retained.

Heavy/torrential rain variants receive lightning in non-polar stock climates. Entry weights allocate 8% of eligible transition weight to thunderstorms. In arid B climates, dry strong-wind/gale states allocate 8% to ordinary dust and 1% to choking dust. These variants preserve their base precipitation, wind and temperature values. Stock lightning has a 0.00002 local-strike chance and 0.0005 atmospheric chance per cell per game minute, damage/pain/stun of 150/150/200, target weights 90/5/5 and a 0.2 ground splash fraction. These are gameplay defaults, adjustable per event.

`dusty air` counts as the stock breathable atmosphere and carries the inhaled Weather Dust Irritant drug. `choking dust` does not count as breathable air. Both reuse existing respiratory/drug systems; dust installation requires the stock `Breathable Atmosphere` gas (or legacy `air`). Local respiratory protection and authored material/exposure rules remain separate configuration.

## Verification ownership

Implementor weather-statistics CSV exports encode all text columns and metadata as spreadsheet-safe cells. Formula prefixes, including those after leading whitespace, receive a leading apostrophe; CSV quoting preserves embedded commas, quotes and line breaks. Numeric measurements retain their invariant numeric representation, including negative temperatures. This export protection does not restrict builder names in the game.

Shared XML/RNG contracts belong to `FutureMUDLibrary Unit Tests`; controller playback, persistence, daily readings and lightning routing belong to `MudSharpCore Unit Tests`. Stock hazard adoption and long climate simulations belong to `MudSharpCore Climate Tests`. Seeder workflow/replay and snapshot contracts remain in `DatabaseSeeder Unit Tests`. Native MySQL/Telnet evidence and limitations are recorded in the [weather verification report](Weather_Verification.md).

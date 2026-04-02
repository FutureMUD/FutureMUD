# Celestial Type: SunFromPlanetaryMoon

## Concept and Real-World Analogy
`SunFromPlanetaryMoon` represents the same physical star as a linked root `Sun`, but transformed into the local sky frame of an observer standing on a moon.

The Earth-system analogy is "the Sun as seen from the Moon," derived from:

- the root Earth-facing `Sun`
- the linked `PlanetaryMoon`

The gas giant stock package uses the same model for Sol as seen from Ganymede, derived from the Jupiter-facing root sun plus the linked Ganymede moon object.

## Data Model and Properties
### Linked properties

| Property | Meaning |
| --- | --- |
| `Moon` | Linked `PlanetaryMoon` providing the moon observer's sidereal timing |
| `Sun` | Linked root `NewSun` providing the base solar orbit and illumination defaults |

### Illumination properties

| Property | Meaning |
| --- | --- |
| `PeakIllumination` | Usually copied from the linked root sun |
| `AlphaScatteringConstant` | Usually copied from the linked root sun |
| `BetaScatteringConstant` | Usually copied from the linked root sun |
| `AtmosphericDensityScalingFactor` | Usually copied from the linked root sun |
| `PlanetaryRadius` | Usually copied from the linked root sun |

If the XML does not include an explicit illumination block, the type copies these values from the linked root `Sun`.

### Derived timing properties

| Property | Source |
| --- | --- |
| `CurrentDayNumber` | Delegated from `Sun.CurrentDayNumber` |
| `CurrentCelestialDay` | Delegated from `Sun.CurrentCelestialDay` |
| `CelestialDaysPerYear` | Delegated from `Sun.CelestialDaysPerYear` |

## Mapping to Real Astronomical Data
This type is mostly derived and should not be authored as though it were an independent star model.

The builder-facing data work is:

- author or seed a correct root `Sun` for the planetary frame
- author or seed a correct `PlanetaryMoon` for the moon frame
- link them
- optionally override illumination values if the moon-local atmosphere should differ from the root sun's defaults

In most cases, illumination should stay synchronized with the root sun unless there is a deliberate gameplay reason to diverge.

## Calculation Pipeline
### 1. Reuse the root sun's day number
The type reuses the linked `Sun` for:

- current day number
- current celestial day
- year/cycle length

This keeps the star's orbital position synchronized with the root planetary-surface representation.

### 2. Reuse the moon's sidereal frame
The type reuses the linked moon for:

- `SiderealTimeAtEpoch`
- `SiderealTimePerDay`
- `DayNumberAtEpoch`

This lets the same physical star be seen using the moon observer's local rotation model.

### 3. Build root-space Cartesian vectors
The type gets:

- moon `RA` and `Dec` from `Moon.EquatorialCoordinates(dayNumber)`
- sun `RA` and `Dec` from `Sun.RightAscension(dayNumber)` and `Sun.Declension(dayNumber)`

Each pair is converted into unit Cartesian components.

### 4. Subtract the moon vector from the sun vector
The implementation computes:

- `x = sunX - moonX`
- `y = sunY - moonY`
- `z = sunZ - moonZ`

This creates the moon-local direction vector to the star.

### 5. Convert back to equatorial coordinates
The type computes:

- `r = sqrt(x*x + y*y + z*z)`
- `RA = atan2(y, x)`
- `Dec = asin(z / r)`

This gives the moon-local apparent equatorial position of the star.

### 6. Compute local sidereal time and hour angle
Using the linked moon's sidereal terms:

`LST = Moon.SiderealTimeAtEpoch + Moon.SiderealTimePerDay * (dayNumber - Moon.DayNumberAtEpoch) + longitude`

Then:

`HA = LST - RA`

### 7. Convert to elevation and azimuth
The local sky conversion is the same standard transform used by the other modern celestial types:

`Elevation = asin(sin(phi)*sin(Dec) + cos(phi)*cos(Dec)*cos(HA))`

`Azimuth = atan2(sin(HA), cos(HA)*sin(phi) - tan(Dec)*cos(phi))`

### 8. Compute illumination
Illumination uses the same atmospheric/scattering pipeline as `NewSun`:

- `U`
- `L`
- `H`
- `RhoH`
- `E1`
- `E2`

The final result is:

`Illumination = E1 + E2`

### 9. Determine direction and time of day
Direction is determined by comparing current elevation to the moon-local value one in-game minute earlier, using the actual clock length from the linked moon's clock.

`CurrentTimeOfDay(...)` then uses the resulting elevation and direction in the same gameplay-oriented way as a root `Sun`.

## Seeder and Testing Considerations
This type should be created as part of a linked package rather than as an isolated one-off object.

Seeder guidance:

- always store both the moon ID and the root sun ID
- default the illumination block from the linked root sun unless there is a deliberate override
- keep the moon-local sun in the same seeder package as its linked moon-view objects

Test guidance:

- verify the type remains synchronized with the linked root sun's day-number progression
- verify local sky position changes when the linked moon's sidereal frame changes
- verify time-of-day classification remains correct
- verify non-24x60 clock direction sampling

## Known Caveats and Implementation Notes
- This is not an independent astrophysical star model.
- A wrong root sun or wrong linked moon will produce a wrong moon-local sun.
- The illumination model is solar-like and copied from the root sun by default, which is usually what you want for linked packages.
- If you are authoring this by hand, treat it as a view transform over two linked inputs, not as a standalone orbital data source.

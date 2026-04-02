# Celestial Type: PlanetFromMoon

## Concept and Real-World Analogy
`PlanetFromMoon` represents the parent planet as seen from the surface of one of its moons.

The Earth-system example is Earth as seen from the Moon. The gas giant stock package uses the same type for Jupiter as seen from Ganymede.

This is not an independent orbit solver. It is a derived representation built from:

- a linked `PlanetaryMoon`
- a linked root `Sun`

It exists because the parent planet and the moon are two sides of the same observer-frame relationship.

## Data Model and Properties
### Linked properties

| Property | Meaning |
| --- | --- |
| `Moon` | The linked `PlanetaryMoon` that defines the shared orbital cycle |
| `Sun` | The linked root `Sun` used for time-of-day and eclipse checks |

### Author-entered properties

| Property | Meaning | Real-world source |
| --- | --- | --- |
| `Name` | Display name of the planet | Builder-facing label |
| `PeakIllumination` | Peak reflected-light contribution | Tuned gameplay value |
| `AngularRadius` | Apparent angular radius of the parent planet in the moon sky | Derived from physical radius and observer distance |

### Derived properties

| Property | Source |
| --- | --- |
| `CurrentDayNumber` | Delegated from `Moon.CurrentDayNumber` |
| `CurrentCelestialDay` | Delegated from `Moon.CurrentCelestialDay` |
| `CelestialDaysPerYear` | Delegated from `Moon.CelestialDaysPerYear` |

## Mapping to Real Astronomical Data
Most of this type is derived rather than directly authored.

Builders mainly need to supply:

- a valid linked moon
- a valid linked root sun
- the apparent angular radius of the parent planet from the moon
- a peak illumination target

### Calculating angular radius
If you know the parent planet radius `R` and the observer distance `d` from the planet center, a good approximation is:

`AngularRadius = asin(R / d)`

For small angles you can also approximate with `R / d`, but the exact inverse-sine form is safer for large apparent discs such as Jupiter from Ganymede.

### Phase relationship
You do not author an independent phase cycle for the planet. The implementation treats the planet phase as the complement of the linked moon phase.

That matches the Earth-from-Moon example:

- when the Moon is full as seen from Earth, Earth is new as seen from the Moon
- when the Moon is new as seen from Earth, Earth is full as seen from the Moon

## Calculation Pipeline
### 1. Inherit the lunar timing model
This type reuses the linked moon's:

- current day number
- current celestial day
- cycle length
- sidereal reference values

It therefore stays locked to the same orbital cycle as the moon by construction.

### 2. Invert equatorial coordinates
The core coordinate rule is:

- `PlanetRA = MoonRA + pi`
- `PlanetDec = -MoonDec`

This makes the parent planet the sky-opposite of the moon representation in the shared orbital frame.

### 3. Compute local sidereal time and hour angle
The type uses the linked moon's sidereal timing:

`LST = Moon.SiderealTimeAtEpoch + Moon.SiderealTimePerDay * (dayNumber - Moon.DayNumberAtEpoch) + longitude`

Then:

`HA = LST - PlanetRA`

### 4. Convert to elevation and azimuth
The local sky conversion is the same standard transform used elsewhere:

`Elevation = asin(sin(phi)*sin(Dec) + cos(phi)*cos(Dec)*cos(HA))`

`Azimuth = atan2(sin(HA), cos(HA)*sin(phi) - tan(Dec)*cos(phi))`

### 5. Compute phase complement
The type computes the moon's cycle fraction, converts it to a moon phase angle, and then adds `pi`:

`PlanetPhaseAngle = MoonPhaseAngle + pi`

wrapped into `[0, 2*pi)`.

### 6. Compute illumination
Illumination uses the same simple Lambertian phase model as `PlanetaryMoon`:

`Illumination = PeakIllumination * (1 + cos(PlanetPhaseAngle)) / 2`

Because the phase angle is shifted by `pi`, the resulting illumination complements the moon's illumination.

### 7. Compute named phase
The implementation maps the linked moon's current phase to its opposite:

- Moon New -> Planet Full
- Moon Waxing Crescent -> Planet Waning Gibbous
- Moon First Quarter -> Planet Last Quarter
- Moon Waxing Gibbous -> Planet Waning Crescent
- Moon Full -> Planet New
- Moon Waning Gibbous -> Planet Waxing Crescent
- Moon Last Quarter -> Planet First Quarter
- Moon Waning Crescent -> Planet Waxing Gibbous

### 8. Eclipse test against the linked sun
`IsSunEclipsed(...)` compares:

- the current local sky position of the parent planet
- the current local sky position of the linked root sun

It computes the angular separation between those two directions and returns true when:

`separation < AngularRadius`

This is a simple disc-overlap test from the moon observer's perspective.

### 9. Time of day
`CurrentTimeOfDay(...)` delegates to the linked root `Sun`.

This is deliberate. The planet does not define the world's day/night cycle.

## Seeder and Testing Considerations
This type should almost always be seeded or authored together with its linked `PlanetaryMoon` and root `Sun`.

Seeder guidance:

- store both linked IDs explicitly
- keep the package name and role markers consistent with the companion objects
- do not treat this as a standalone celestial package unless you are also resolving or creating the required links

Test guidance:

- verify equatorial opposition against the linked moon
- verify illumination complement
- verify eclipse behavior for the chosen angular radius
- verify non-24x60 clock direction sampling if timing logic changes

## Known Caveats and Implementation Notes
- This type is a derived observer-frame representation, not a generic planet ephemeris model.
- A bad linked moon definition will produce a bad `PlanetFromMoon` sky.
- The eclipse model is intentionally simple and based on angular overlap rather than a full umbra/penumbra simulation.
- If you need a planet seen from a planetary surface rather than from a moon surface, this is the wrong type.

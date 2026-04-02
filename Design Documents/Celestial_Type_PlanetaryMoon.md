# Celestial Type: PlanetaryMoon

## Concept and Real-World Analogy
`PlanetaryMoon` represents a moon seen from the surface of its parent planet.

The canonical example is Earth's Moon as seen from Earth. The gas giant stock package uses the same type for Ganymede as seen from Jupiter.

This type is the "planet observer looking up at the moon" half of the linked moon-view model.

## Data Model and Properties
### Identity and timing properties

| Property | Meaning |
| --- | --- |
| `Name` | Display name of the moon |
| `Calendar` | Calendar used to determine current date |
| `Clock` | Feed clock used to determine current time fraction |
| `EpochDate` | Date associated with the orbital element epoch |

### Orbital properties

| Property | Meaning | Real-world source |
| --- | --- | --- |
| `CelestialDaysPerYear` | Phase/orbital cycle length used by the model | Synodic or chosen lunar cycle length |
| `MeanAnomalyAngleAtEpoch` | Mean anomaly at the epoch | Ephemeris mean anomaly |
| `AnomalyChangeAnglePerDay` | Mean anomaly change per day | Mean motion |
| `ArgumentOfPeriapsis` | Argument of periapsis | Standard orbital element |
| `LongitudeOfAscendingNode` | Longitude of ascending node | Standard orbital element |
| `OrbitalInclination` | Inclination to the reference plane | Standard orbital element |
| `OrbitalEccentricity` | Orbital eccentricity | Standard orbital element |
| `DayNumberAtEpoch` | Numeric day number at epoch | Usually J2000-like day count |
| `SiderealTimeAtEpoch` | Sidereal reference at epoch | Derived from the parent world's rotation model |
| `SiderealTimePerDay` | Sidereal advance per day | Parent-world sidereal motion |
| `FullMoonReferenceDay` | Day in the local lunar cycle considered full moon | Phase anchor, often chosen from an epoch almanac |

### Illumination and presentation properties

| Property | Meaning |
| --- | --- |
| `PeakIllumination` | Peak light contribution at full moon |
| `Triggers` | Elevation-angle trigger thresholds and echoes |

## Mapping to Real Astronomical Data
Builders can usually source most of this type directly from published lunar or satellite orbital elements.

Useful mapping rules:

- `MeanAnomalyAngleAtEpoch` maps directly to the moon's mean anomaly at the chosen epoch.
- `ArgumentOfPeriapsis`, `LongitudeOfAscendingNode`, `OrbitalInclination`, and `OrbitalEccentricity` are standard classical orbital elements.
- `AnomalyChangeAnglePerDay` is the moon's mean motion in radians per day.
- `SiderealTimeAtEpoch` and `SiderealTimePerDay` should match the parent planet's local sidereal rotation model.
- `FullMoonReferenceDay` is not usually a published element. It is a gameplay phase anchor. You derive it by choosing a known full-moon date and ensuring the model's cycle day at that date maps to zero.

For Earth's Moon, astronomy almanacs and common ephemeris tables usually provide enough data to populate all fields except the chosen gameplay phase anchor.

## Calculation Pipeline
### 1. Current day number and cycle day
The type derives its current day number from calendar date, clock time fraction, epoch date, and day number at epoch.

`CurrentCelestialDay` is then wrapped by `CelestialDaysPerYear`.

### 2. Mean anomaly
The moon computes:

`M = MeanAnomalyAngleAtEpoch + AnomalyChangeAnglePerDay * (dayNumber - DayNumberAtEpoch)`

and wraps the result into `[0, 2*pi)`.

### 3. True anomaly
The implementation uses a simple second-order approximation:

`v = M + 2*e*sin(M) + 1.25*e^2*sin(2M)`

where `e` is `OrbitalEccentricity`.

### 4. Equatorial coordinates
The type then rotates the orbital position into equatorial coordinates.

It computes:

- `wv = v + ArgumentOfPeriapsis`
- Cartesian orbital-plane components using `LongitudeOfAscendingNode`, `wv`, and `OrbitalInclination`
- `RA = atan2(y, x)`
- `Dec = asin(z)`

This gives the moon's right ascension and declination in the parent planet observer's frame.

### 5. Sidereal time
Local sidereal time is:

`LST = SiderealTimeAtEpoch + SiderealTimePerDay * (dayNumber - DayNumberAtEpoch) + longitude`

wrapped into `[0, 2*pi)`.

### 6. Hour angle
The hour angle is:

`HA = LST - RA`

### 7. Elevation and azimuth
The local sky conversion uses the same standard form as the sun types:

`Elevation = asin(sin(phi)*sin(Dec) + cos(phi)*cos(Dec)*cos(HA))`

`Azimuth = atan2(sin(HA), cos(HA)*sin(phi) - tan(Dec)*cos(phi))`

### 8. Phase angle
The phase cycle is anchored to `FullMoonReferenceDay`.

The runtime computes:

- `cycleDay = (CurrentCelestialDay - FullMoonReferenceDay) mod CelestialDaysPerYear`
- `PhaseAngle = 2*pi*(cycleDay / CelestialDaysPerYear)`

This means the reference day is treated as full moon.

### 9. Illumination
The type uses a simple Lambertian phase model:

`Illumination = PeakIllumination * (1 + cos(PhaseAngle)) / 2`

This gives:

- peak illumination at full moon
- minimum illumination at new moon

### 10. Named moon phases
The type converts the cycle fraction into the eight stock phase names:

- Full
- Waxing Gibbous
- First Quarter
- Waxing Crescent
- New
- Waning Crescent
- Last Quarter
- Waning Gibbous

### 11. Direction
Movement direction is determined by comparing the current elevation to the elevation one in-game minute earlier, using the actual clock length rather than a hard-coded Earth minute fraction.

## Seeder and Testing Considerations
The stock seeder currently uses this type for:

- Earth's Moon in the Earth moon-view package
- Ganymede in the gas giant moon-view package

When implementing new seeded moons:

1. keep the moon's `Calendar` and `Clock` consistent with the intended world frame
2. choose `FullMoonReferenceDay` from a known full-moon epoch
3. if you are also seeding `PlanetFromMoon` or `SunFromPlanetaryMoon`, create those linked objects in the same package and store the linked IDs explicitly

Test considerations:

- changes to phase math need regression tests around full and new moon
- changes to sidereal or local sky transforms need numeric angle tests
- changes to minute sampling need non-24x60 clock tests

## Known Caveats and Implementation Notes
- `CurrentTimeOfDay(...)` always returns `Night` for this type because a moon does not define the zone's day/night cycle.
- The illumination model is intentionally simple and phase-based; it does not attempt a full reflected-light or eclipse model on its own.
- The type assumes the supplied orbital elements are already in the coordinate convention expected by the implementation.
- If you need the parent planet or the same star from the moon observer's frame, use the linked types rather than overloading this one.

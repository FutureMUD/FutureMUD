# Celestial Type: PlanetFromMoon

## Runtime Role
`PlanetFromMoon` models the parent planet as seen from the surface of one of its moons.

It is a derived observer-frame type built from:

- a linked `PlanetaryMoon`
- a linked root `Sun`

## Authored and Derived Data
The authored values are:

- `Name`
- `PeakIllumination`
- `AngularRadius`
- `SunAngularRadius`

The linked moon supplies the orbital timing and sky-opposition relationship. The linked sun supplies time-of-day behavior and the apparent solar disc size used by eclipse tests.

## Calculation Pipeline
The runtime flow is:

1. Reuse the linked moon's current day number and cycle day.
2. Compute equatorial coordinates by inverting the linked moon:
   - `PlanetRA = MoonRA + pi`
   - `PlanetDec = -MoonDec`
3. Reuse the linked moon sidereal model for local sky conversion.
4. Convert to hour angle, elevation, and azimuth.
5. Compute planet phase as the complement of the linked moon phase.
6. Compute illumination as the complement of the linked moon illumination.

## Eclipse Logic
`IsSunEclipsed(...)` compares the angular separation between:

- the parent planet's local sky position
- the linked root sun's local sky position

The corrected threshold is simple disc overlap:

`separation <= AngularRadius + SunAngularRadius`

This remains an intentionally simple model. It is not a full umbra or penumbra simulation, but it is no longer the overly strict "planet centreline falls inside the planet disc only" check.

## Time of Day
`CurrentTimeOfDay(...)` delegates to the linked root `Sun`. The parent planet does not define day versus night for the world.

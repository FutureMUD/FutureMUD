# Celestial Type: SunFromPlanetaryMoon

## Runtime Role
`SunFromPlanetaryMoon` models the root star from the point of view of an observer standing on a moon.

It is derived from:

- a linked `PlanetaryMoon`
- a linked root `Sun`

## Timing Model
The type deliberately mixes two sources:

- the root `Sun` supplies the solar orbital position and current day number
- the linked `PlanetaryMoon` supplies the moon-local sidereal rotation model

That keeps the star synchronized with the root solar orbit while still using the moon observer's local sky frame.

## Corrected Relative-Position Transform
The corrected transform uses physical relative position vectors.

The runtime now:

1. Gets the root sun position vector from `Sun.PositionVector(dayNumber)`.
2. Gets the moon position vector from `Moon.PositionVector(dayNumber)`.
3. Computes the moon-local relative vector:
   - `relative = sunVector - moonVector`
4. Normalizes that relative vector carefully.
5. Converts the normalized result back to right ascension and declination.

This replaced the older unit-vector subtraction approach, which treated the sun and moon directions as equal-length vectors and became numerically poor near conjunction.

## Local Sky and Time of Day
After the relative equatorial direction is computed, the type:

1. Reuses the linked moon sidereal epoch and sidereal rate.
2. Converts to hour angle, elevation, and azimuth.
3. Reuses the solar illumination model.
4. Classifies time of day with the same corrected 12-degree twilight threshold used by `Sun`.

Because the moon-local frame uses the linked moon clock, movement direction sampling still depends on the actual configured clock dimensions rather than a hard-coded Earth minute fraction.

## Illumination Defaults
If the XML omits an illumination block, `SunFromPlanetaryMoon` copies:

- `PeakIllumination`
- `AlphaScatteringConstant`
- `BetaScatteringConstant`
- `AtmosphericDensityScalingFactor`
- `PlanetaryRadius`

from the linked root `Sun`.

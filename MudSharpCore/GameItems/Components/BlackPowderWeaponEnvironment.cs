using MudSharp.Climate;
using MudSharp.Construction;
using MudSharp.Form.Material;

#nullable enable

namespace MudSharp.GameItems.Components;

/// <summary>
/// Shared environmental rules for exposed black-powder weapons. Black powder carries its own
/// oxidiser, so a closed charge may discharge in vacuum, but exposed flame and sound still need
/// an atmosphere. A liquid atmosphere or underwater layer makes handling and firing unsafe.
/// </summary>
public static class BlackPowderWeaponEnvironment
{
	public const double MaximumOpenFlamePrecipitation = 0.5;

	public static bool CanHandlePowder(ICharacter? actor)
	{
		if (actor?.Location is null)
		{
			return true;
		}
		return CanHandlePowder(actor.RoomLayer.IsUnderwater(), actor.Location.Atmosphere is ILiquid);
	}

	public static bool CanHandlePowder(bool isUnderwaterLayer, bool hasLiquidAtmosphere) =>
		!isUnderwaterLayer && !hasLiquidAtmosphere;

	public static bool CanHandleExposedPowder(ICharacter? actor)
	{
		return CanHandleExposedPowder(CanHandlePowder(actor), PrecipitationIntensity(actor));
	}

	public static bool CanHandleExposedPowder(bool canHandlePowder, double precipitationIntensity) =>
		canHandlePowder && precipitationIntensity <= MaximumOpenFlamePrecipitation;

	public static bool CanSustainOpenFlame(ICharacter? actor)
	{
		if (actor?.Location is null)
		{
			return true;
		}
		return CanSustainOpenFlame(CanHandlePowder(actor), actor.Location.Atmosphere is IGas,
			PrecipitationIntensity(actor));
	}

	public static bool CanSustainOpenFlame(bool canHandlePowder, bool hasGaseousAtmosphere,
		double precipitationIntensity) =>
		canHandlePowder && hasGaseousAtmosphere &&
		precipitationIntensity <= MaximumOpenFlamePrecipitation;

	public static bool CanPropagateSound(ICharacter? actor)
	{
		return CanPropagateSound(actor?.Location?.Atmosphere is not null);
	}

	public static bool CanPropagateSound(bool hasAtmosphere) => hasAtmosphere;

	public static double PrecipitationIntensity(ICharacter? actor)
	{
		if (actor?.Location is not { } location)
		{
			return 0.0;
		}
		return location.CurrentWeather(actor)?.Precipitation.PrecipitationIntensityForGunpowder() ?? 0.0;
	}
}

#nullable enable

using MudSharp.Celestial;
using MudSharp.Climate;

namespace MudSharp.Construction;

/// <summary>Optional inputs for a single rendering of authored room text. Omitted inputs use the room's live values.</summary>
public sealed record RoomDescriptionOverrides(
	TimeOfDay? Time = null,
	WindLevel? Wind = null,
	PrecipitationLevel? Rain = null,
	PrecipitationLevel? RecentRain = null,
	ISeason? Season = null,
	double? Lux = null);

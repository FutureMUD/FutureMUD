#nullable enable

using System.Globalization;

namespace MudSharp.Arenas;

internal static class ArenaSideIndexUtilities
{
    public static int ToDisplayIndex(int sideIndex)
    {
        return sideIndex + 1;
    }

    public static string ToDisplayString(IFormatProvider formatProvider, int sideIndex)
    {
        return ToDisplayIndex(sideIndex).ToString(formatProvider);
    }

    public static IReadOnlyDictionary<int, int> ResolveEvenlySpacedStartRooms(
        IReadOnlyList<int>? orderedSideIndices,
        int arenaRoomCount,
        int rotationOffset)
    {
        Dictionary<int, int> result = new();
        if (orderedSideIndices is null || arenaRoomCount <= 0)
        {
            return result;
        }

        List<int> sides = orderedSideIndices
            .Distinct()
            .ToList();
        if (sides.Count == 0)
        {
            return result;
        }

        int normalisedRotation = rotationOffset % arenaRoomCount;
        if (normalisedRotation < 0)
        {
            normalisedRotation += arenaRoomCount;
        }

        for (int i = 0; i < sides.Count; i++)
        {
            int baseIndex = i * arenaRoomCount / sides.Count;
            result[sides[i]] = (baseIndex + normalisedRotation) % arenaRoomCount;
        }

        return result;
    }

    public static bool TryParseDisplayIndex(string? text, out int sideIndex)
    {
        sideIndex = 0;
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        if (!int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int displayIndex) || displayIndex <= 0)
        {
            return false;
        }

        sideIndex = displayIndex - 1;
        return true;
    }
}

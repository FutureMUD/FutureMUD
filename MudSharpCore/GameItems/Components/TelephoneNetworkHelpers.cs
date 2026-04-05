#nullable enable
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.GameItems.Components;

internal static class TelephoneNetworkHelpers
{
    private static readonly HashSet<char> ValidKeypadCharacters = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '*', '#'];

    public static bool TryNormaliseDigits(string? digits, out string normalised)
    {
        normalised = string.IsNullOrWhiteSpace(digits)
            ? string.Empty
            : new string(digits.Where(x => !char.IsWhiteSpace(x)).ToArray());

        return !string.IsNullOrEmpty(normalised) && normalised.All(ValidKeypadCharacters.Contains);
    }

    public static IEnumerable<ITelephone> CollectConnectedTelephones(ITelephoneNumberOwner owner)
    {
        HashSet<long> visited = new()
        { owner.Parent.Id };
        foreach (ITelephone phone in owner.ConnectedTelephones)
        {
            yield return phone;
        }

        foreach (IConnectable connectable in owner.Parent.GetItemTypes<IConnectable>())
        {
            foreach (IGameItem? other in connectable.ConnectedItems.Select(x => x.Item2.Parent))
            {
                foreach (ITelephone phone in CollectConnectedTelephones(other, visited))
                {
                    yield return phone;
                }
            }
        }
    }

    private static IEnumerable<ITelephone> CollectConnectedTelephones(IGameItem item, HashSet<long> visited)
    {
        if (!visited.Add(item.Id))
        {
            yield break;
        }

        if (item.GetItemType<ITelephone>() is { } phone)
        {
            yield return phone;
        }

        if (item.GetItemType<ITelephone>() == null && item.GetItemType<ITelephoneNumberOwner>() != null)
        {
            yield break;
        }

        foreach (IConnectable connectable in item.GetItemTypes<IConnectable>())
        {
            foreach (IGameItem? other in connectable.ConnectedItems.Select(x => x.Item2.Parent))
            {
                foreach (ITelephone connectedPhone in CollectConnectedTelephones(other, visited))
                {
                    yield return connectedPhone;
                }
            }
        }
    }
}

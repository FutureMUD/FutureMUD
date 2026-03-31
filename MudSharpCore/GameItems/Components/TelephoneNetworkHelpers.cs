#nullable enable
using System.Collections.Generic;
using System.Linq;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

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
		var visited = new HashSet<long> { owner.Parent.Id };
		foreach (var phone in owner.ConnectedTelephones)
		{
			yield return phone;
		}

		foreach (var connectable in owner.Parent.GetItemTypes<IConnectable>())
		{
			foreach (var other in connectable.ConnectedItems.Select(x => x.Item2.Parent))
			{
				foreach (var phone in CollectConnectedTelephones(other, visited))
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

		foreach (var connectable in item.GetItemTypes<IConnectable>())
		{
			foreach (var other in connectable.ConnectedItems.Select(x => x.Item2.Parent))
			{
				foreach (var connectedPhone in CollectConnectedTelephones(other, visited))
				{
					yield return connectedPhone;
				}
			}
		}
	}
}

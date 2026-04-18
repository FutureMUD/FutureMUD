#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Computers;

internal static class ComputerFileTransferUtilities
{
	public static IEnumerable<IComputerFileOwner> EnumerateOwners(IComputerHost host)
	{
		yield return host;
		foreach (var storage in host.MountedStorage)
		{
			yield return storage;
		}

		if (host is IGameItemComponent component)
		{
			foreach (var owner in component.Parent.Components
				         .OfType<IComputerFileOwner>()
				         .Where(x => x is not IComputerExecutableOwner)
				         .OrderBy(x => DescribeOwner(x))
				         .ThenBy(x => x.FileOwnerId))
			{
				yield return owner;
			}
		}
	}

	public static IComputerFileOwner? ResolveSelectableOwner(IComputerHost host, string identifier,
		out string? error)
	{
		error = null;
		if (identifier.EqualTo("host") || host.Name.Equals(identifier, StringComparison.InvariantCultureIgnoreCase))
		{
			return host;
		}

		var owners = EnumerateOwners(host).ToList();
		if (long.TryParse(identifier, out var id))
		{
			return owners.FirstOrDefault(x => x.FileOwnerId == id);
		}

		var exact = owners
			.Where(x =>
				x.Name.Equals(identifier, StringComparison.InvariantCultureIgnoreCase) ||
				DescribeOwner(x).Equals(identifier, StringComparison.InvariantCultureIgnoreCase))
			.ToList();
		if (exact.Count == 1)
		{
			return exact.Single();
		}

		if (exact.Count > 1)
		{
			error = $"More than one file owner matches {identifier.ColourName()}.";
			return null;
		}

		var partial = owners
			.Where(x =>
				x.Name.StartsWith(identifier, StringComparison.InvariantCultureIgnoreCase) ||
				DescribeOwner(x).StartsWith(identifier, StringComparison.InvariantCultureIgnoreCase))
			.ToList();
		if (partial.Count == 1)
		{
			return partial.Single();
		}

		if (partial.Count > 1)
		{
			error = $"More than one file owner starts with {identifier.ColourName()}.";
			return null;
		}

		return null;
	}

	public static string DescribeOwner(IComputerFileOwner owner)
	{
		return owner switch
		{
			IComputerStorage storage => storage.Name,
			IComputerHost host => host.Name,
			IGameItemComponent component => $"{component.Parent.Name}@{component.Name}",
			_ => owner.Name
		};
	}

	public static string GetOwnerIdentifier(IComputerHost host, IComputerFileOwner owner)
	{
		return owner switch
		{
			IComputerHost => "host",
			IComputerStorage storage when host.MountedStorage.Contains(storage) => storage.Name,
			IGameItemComponent => DescribeOwner(owner),
			_ => owner.Name
		};
	}
}

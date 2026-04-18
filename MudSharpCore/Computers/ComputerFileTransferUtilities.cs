#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework;

namespace MudSharp.Computers;

internal static class ComputerFileTransferUtilities
{
	public static IEnumerable<IComputerExecutableOwner> EnumerateOwners(IComputerHost host)
	{
		yield return host;
		foreach (var storage in host.MountedStorage)
		{
			yield return storage;
		}
	}

	public static IComputerExecutableOwner? ResolveSelectableOwner(IComputerHost host, string identifier,
		out string? error)
	{
		error = null;
		if (identifier.EqualTo("host") || host.Name.Equals(identifier, StringComparison.InvariantCultureIgnoreCase))
		{
			return host;
		}

		var storages = host.MountedStorage.ToList();
		if (long.TryParse(identifier, out var id))
		{
			return storages.FirstOrDefault(x => x.OwnerStorageItemId == id);
		}

		var exact = storages
			.Where(x => x.Name.Equals(identifier, StringComparison.InvariantCultureIgnoreCase))
			.ToList();
		if (exact.Count == 1)
		{
			return exact.Single();
		}

		if (exact.Count > 1)
		{
			error = $"More than one mounted storage device matches {identifier.ColourName()}.";
			return null;
		}

		var partial = storages
			.Where(x => x.Name.StartsWith(identifier, StringComparison.InvariantCultureIgnoreCase))
			.ToList();
		if (partial.Count == 1)
		{
			return partial.Single();
		}

		if (partial.Count > 1)
		{
			error = $"More than one mounted storage device starts with {identifier.ColourName()}.";
			return null;
		}

		return null;
	}

	public static string DescribeOwner(IComputerExecutableOwner owner)
	{
		return owner switch
		{
			IComputerStorage storage => storage.Name,
			IComputerHost host => host.Name,
			_ => owner.Name
		};
	}

	public static string GetOwnerIdentifier(IComputerHost host, IComputerExecutableOwner owner)
	{
		return owner switch
		{
			IComputerHost => "host",
			IComputerStorage storage when host.MountedStorage.Contains(storage) => storage.Name,
			_ => owner.Name
		};
	}
}

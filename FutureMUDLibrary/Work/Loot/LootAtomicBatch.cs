using System;
using System.Collections.Generic;

namespace MudSharp.Work.Loot;

public static class LootAtomicBatch
{
	public static IReadOnlyList<TCreated> Execute<TPlan, TCreated>(
		IEnumerable<TPlan> plan,
		Func<TPlan, IEnumerable<TCreated>> create,
		Action<IReadOnlyList<TCreated>> preflight,
		Action<IReadOnlyList<TCreated>> commit,
		Action<IReadOnlyList<TCreated>> rollback)
	{
		var created = new List<TCreated>();
		try
		{
			foreach (var entry in plan)
			{
				foreach (var value in create(entry)) created.Add(value);
			}
			preflight(created);
			commit(created);
			return created;
		}
		catch
		{
			rollback(created);
			throw;
		}
	}
}

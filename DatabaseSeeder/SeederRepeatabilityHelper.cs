#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder;

internal static class SeederRepeatabilityHelper
{
	public static ShouldSeedResult ClassifyByPresence(IEnumerable<bool> itemPresence)
	{
		var presence = itemPresence.ToList();
		if (!presence.Any())
		{
			return ShouldSeedResult.ReadyToInstall;
		}

		var presentCount = presence.Count(x => x);
		if (presentCount <= 0)
		{
			return ShouldSeedResult.ReadyToInstall;
		}

		return presentCount == presence.Count
			? ShouldSeedResult.MayAlreadyBeInstalled
			: ShouldSeedResult.ExtraPackagesAvailable;
	}

	public static T EnsureEntity<T>(
		DbSet<T> set,
		Expression<Func<T, bool>> predicate,
		Func<T> create)
		where T : class
	{
		var localPredicate = predicate.Compile();
		return set.Local.FirstOrDefault(localPredicate) ??
		       set.AsEnumerable().FirstOrDefault(localPredicate) ??
		       create();
	}

	public static T EnsureEntity<T>(
		DbSet<T> set,
		Func<T, bool> localPredicate,
		Expression<Func<T, bool>> queryPredicate,
		Func<T> create)
		where T : class
	{
		return set.Local.FirstOrDefault(localPredicate) ??
		       set.Where(queryPredicate).AsEnumerable().FirstOrDefault(localPredicate) ??
		       create();
	}

	public static T EnsureNamedEntity<T>(
		DbSet<T> set,
		string name,
		Expression<Func<T, string>> nameSelector,
		Func<T> create)
		where T : class
	{
		var localSelector = nameSelector.Compile();
		return EnsureEntity(
			set,
			x => string.Equals(localSelector(x), name, StringComparison.OrdinalIgnoreCase),
			_ => true,
			create);
	}

	public static FutureProg EnsureProg(
		FuturemudDatabaseContext context,
		string functionName,
		string category,
		string subcategory,
		ProgVariableTypes returnType,
		string comment,
		string text,
		bool isPublic,
		bool acceptsAnyParameters,
		FutureProgStaticType staticType,
		params (ProgVariableTypes Type, string Name)[] parameters)
	{
		var prog = EnsureNamedEntity(
			context.FutureProgs,
			functionName,
			x => x.FunctionName,
			() =>
			{
				var created = new FutureProg();
				context.FutureProgs.Add(created);
				return created;
			});

		prog.FunctionName = functionName;
		prog.FunctionComment = comment;
		prog.FunctionText = text;
		prog.ReturnType = (long)returnType;
		prog.Category = category;
		prog.Subcategory = subcategory;
		prog.Public = isPublic;
		prog.AcceptsAnyParameters = acceptsAnyParameters;
		prog.StaticType = (int)staticType;

		var existingParameters = prog.FutureProgsParameters.ToList();
		foreach (var existing in existingParameters)
		{
			context.FutureProgsParameters.Remove(existing);
		}

		for (var index = 0; index < parameters.Length; index++)
		{
			var (type, name) = parameters[index];
			prog.FutureProgsParameters.Add(new FutureProgsParameter
			{
				FutureProg = prog,
				ParameterIndex = index,
				ParameterType = (long)type,
				ParameterName = name
			});
		}

		return prog;
	}

	public static void ReplaceChildCollection<TChild, TKey>(
		ICollection<TChild> target,
		IEnumerable<TChild> desired,
		Func<TChild, TKey> keySelector)
		where TKey : notnull
	{
		var desiredByKey = desired.ToDictionary(keySelector);
		var existing = target.ToList();
		foreach (var item in existing.Where(x => !desiredByKey.ContainsKey(keySelector(x))))
		{
			target.Remove(item);
		}

		var existingKeys = target.Select(keySelector).ToHashSet();
		foreach (var item in desiredByKey.Values.Where(x => !existingKeys.Contains(keySelector(x))))
		{
			target.Add(item);
		}
	}

	public static void EnsureLink<TLink>(
		DbSet<TLink> set,
		Expression<Func<TLink, bool>> predicate,
		Func<TLink> create)
		where TLink : class
	{
		var localPredicate = predicate.Compile();
		if (set.Local.Any(localPredicate) || set.Any(predicate))
		{
			return;
		}

		set.Add(create());
	}
}

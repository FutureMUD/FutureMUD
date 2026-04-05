#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DatabaseSeeder;

internal static class SeederRepeatabilityHelper
{
    public static ShouldSeedResult ClassifyByPresence(IEnumerable<bool> itemPresence)
    {
        List<bool> presence = itemPresence.ToList();
        if (!presence.Any())
        {
            return ShouldSeedResult.ReadyToInstall;
        }

        int presentCount = presence.Count(x => x);
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
        Func<T, bool> localPredicate = predicate.Compile();
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
        Func<T, string> localSelector = nameSelector.Compile();
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
        FutureProg prog = EnsureNamedEntity(
            context.FutureProgs,
            functionName,
            x => x.FunctionName,
            () =>
            {
                FutureProg created = new();
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

        List<FutureProgsParameter> existingParameters = prog.FutureProgsParameters.ToList();
        foreach (FutureProgsParameter existing in existingParameters)
        {
            context.FutureProgsParameters.Remove(existing);
        }

        for (int index = 0; index < parameters.Length; index++)
        {
            (ProgVariableTypes type, string? name) = parameters[index];
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
        Dictionary<TKey, TChild> desiredByKey = desired.ToDictionary(keySelector);
        List<TChild> existing = target.ToList();
        foreach (TChild? item in existing.Where(x => !desiredByKey.ContainsKey(keySelector(x))))
        {
            target.Remove(item);
        }

        HashSet<TKey> existingKeys = target.Select(keySelector).ToHashSet();
        foreach (TChild? item in desiredByKey.Values.Where(x => !existingKeys.Contains(keySelector(x))))
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
        Func<TLink, bool> localPredicate = predicate.Compile();
        if (set.Local.Any(localPredicate) || set.Any(predicate))
        {
            return;
        }

        set.Add(create());
    }
}

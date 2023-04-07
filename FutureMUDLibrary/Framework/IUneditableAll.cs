using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace MudSharp.Framework
{
    public interface IUneditableAll<T> : IEnumerable<T> where T : class, IFrameworkItem {
        bool Has(T value);
        bool Has(long id);
        bool Has(string name);

        T? Get(long id);
        bool TryGet(long id, out T? result);
        List<T> Get(string name);
        T? GetByName(string name);
        T? GetByIdOrName(string value, bool permitAbbreviations = true);

        void ForEach(Action<T> action);
    }
}
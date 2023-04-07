using System;
using System.Collections.Generic;
using MudSharp.Framework.Revision;

namespace MudSharp.Framework
{
    public interface IUneditableRevisableAll<T> : IEnumerable<T> where T : class, IRevisableItem {
        bool Has(T value);
        bool Has(long id);
        bool Has(long id, int revision);
        bool Has(string name);
        
        T Get(long id, int revision, Func<T, bool> filter);
        T Get(long id, Func<T, bool> filter);
        T Get(long id);
        T Get(long id, int revision);
        T GetByName(string name, bool ignoreCase = false);
        T GetByName(string name, int revision, bool ignoreCase = false);
        T? GetByIdOrName(string value, bool permitAbbreviations = true);
        T TryGet(long id, out T result);
        T TryGet(long id, int revision, out T result);
        List<T> Get(string name);
        List<T> GetAll(long id);
        List<T> GetAll(long id, Func<T, bool> filter);
        List<T> GetAllByIdOrName(string value, bool permitAbbreviations = true);
        List<T> GetAllByStatus(params RevisionStatus[] statuses);

        /// <summary>
        /// Returns a list of any current, pending revision, under design items for each item, but may also include rejected or obsolete if there are no current
        /// </summary>
        /// <param name="includeAtLeastOne">If true, include obsolete/rejected if there isn't any current/under design items</param>
        /// <returns>A list of items</returns>
        List<T> GetAllApprovedOrMostRecent(bool includeAtLeastOne = false);
        void ForEach(Action<T> action);
    }
}
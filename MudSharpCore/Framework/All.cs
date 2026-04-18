using MudSharp.Framework.Save;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable enable

namespace MudSharp.Framework;

public class All<T> : IAll<T>, IUneditableAll<T> where T : class, IFrameworkItem
{
    private readonly Dictionary<long, T> _idlookup;
    private readonly List<T> _iterlist;

    public All()
    {
        _idlookup = new Dictionary<long, T>();
        _iterlist = new List<T>();
    }

    public int Count => _idlookup.Count;

    public IEnumerable<long> IDs => _idlookup.Keys;

    public IEnumerable<T> Values => _iterlist;

    public void Clear()
    {
        _idlookup.Clear();
        _iterlist.Clear();
    }

    public bool Add(T value)
    {
        if (value is ILateInitialisingItem lateSave)
        {
            if (!lateSave.IdHasBeenRegistered)
            {
                lateSave.IdRegistered += All_IdRegistered;
                _iterlist.Add(value);
                return true;
            }
        }

        if (value.Id == 0)
        {
            throw new ApplicationException(
                "Very unlikely to have an ID of zero. Possibly using default value of ID when adding Item to list?");
        }

        if (_iterlist.Contains(value))
        {
            throw new ApplicationException(
                $"Trying to add a duplicate object {value.Id} / {value?.Name ?? ""} / {value.FrameworkItemType} to All.");
        }

        _idlookup.Add(value.Id, value);
        _iterlist.Add(value);
        return true;
    }

    public bool Has(T value)
    {
        return Has(value.Id);
    }

    public bool Has(long id)
    {
        return _idlookup.ContainsKey(id);
    }

    public bool Has(string name)
    {
        return _iterlist.Any(x => HasExactNameMatch(x, name));
    }

    public T? Get(long id)
    {
        return _idlookup.TryGetValue(id, out T? value) ? value : null;
    }

    public bool TryGet(long id, out T? result)
    {
        return _idlookup.TryGetValue(id, out result);
    }

    public List<T> Get(string name)
    {
        return _iterlist.Where(x => HasExactNameMatch(x, name)).ToList();
    }

    public T? GetByName(string name)
    {
        return _iterlist.FirstOrDefault(x => HasExactNameMatch(x, name)) ??
               _iterlist.FirstOrDefault(x => HasNameMatch(x, name, true));
    }

    public T? GetByIdOrName(string value, bool permitAbbreviations = true)
    {
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }
        if (long.TryParse(value, out long id))
        {
            return Get(id);
        }

        if (!permitAbbreviations)
        {
            return _iterlist.FirstOrDefault(x => HasExactNameMatch(x, value));
        }

        return _iterlist.FirstOrDefault(x => HasExactNameMatch(x, value)) ??
               _iterlist.FirstOrDefault(x => HasNameMatch(x, value, true));
    }

    private static IEnumerable<string> NamesForItem(T item)
    {
        if (item is IHaveMultipleNames multiNameItem)
        {
            return multiNameItem.Names.Where(x => !string.IsNullOrWhiteSpace(x));
        }

        return string.IsNullOrWhiteSpace(item.Name) ? Enumerable.Empty<string>() : [item.Name];
    }

    private static bool HasExactNameMatch(T item, string name)
    {
        return HasNameMatch(item, name, false);
    }

    private static bool HasNameMatch(T item, string name, bool permitAbbreviations)
    {
        return NamesForItem(item)
            .Any(x => permitAbbreviations
                ? x.StartsWith(name, StringComparison.InvariantCultureIgnoreCase)
                : x.Equals(name, StringComparison.InvariantCultureIgnoreCase));
    }

    public void ForEach(Action<T> action)
    {
        foreach (T value in _idlookup.Values)
        {
            action(value);
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _iterlist.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private void All_IdRegistered(ILateInitialisingItem item)
    {
        _idlookup.Add(item.Id, _iterlist.First(x => x == item));
        item.IdRegistered -= All_IdRegistered;
    }

    public bool Add(IFrameworkItem item)
    {
        return Add((T)item);
    }

    public bool Remove(long id)
    {
        _idlookup.TryGetValue(id, out T? lookup);

        if (lookup != null)
        {
            _idlookup.Remove(id);
            return _iterlist.Remove(lookup);
        }

        return false;
    }

    public bool Remove(T value)
    {
        if (value is ILateInitialisingItem lateSave)
        {
            lateSave.IdRegistered -= All_IdRegistered;
        }

        if (_idlookup.ContainsKey(value.Id))
        {
            _idlookup.Remove(value.Id);
            return _iterlist.Remove(value);
        }

        return false;
    }

    public int RemoveAll(Func<T, bool> predicateFunc)
    {
        int numberRemoved = 0;
        foreach (T? item in _iterlist.Where(predicateFunc).ToList())
        {
            if (item is ILateInitialisingItem lateSave)
            {
                lateSave.IdRegistered -= All_IdRegistered;
            }

            _iterlist.Remove(item);
            _idlookup.Remove(item.Id);
            numberRemoved++;
        }

        return numberRemoved;
    }

    public bool Has(IFrameworkItem item)
    {
        return item.Equals(Get(item.Id));
    }

    public override string ToString()
    {
        StringBuilder sb = new();

        foreach (T? item in _idlookup.Values.ToList())
        {
            sb.AppendLine(item.Id + ": " + item.Name.Proper());
        }

        return sb.ToString();
    }

    public T this[int key] => _iterlist[key];

    public void RemoveRange(int count)
    {
        List<T> removed = _iterlist.Take(count).ToList();
        _iterlist.RemoveRange(0, count);
        foreach (T? item in removed)
        {
            _idlookup.Remove(item.Id);
            if (item is ILateInitialisingItem lateSave)
            {
                lateSave.IdRegistered -= All_IdRegistered;
            }
        }
    }

    public void RemoveRange(int index, int count)
    {
        List<T> removed = _iterlist.Skip(index).Take(count).ToList();
        _iterlist.RemoveRange(index, count);
        foreach (T? item in removed)
        {
            _idlookup.Remove(item.Id);
            if (item is ILateInitialisingItem lateSave)
            {
                lateSave.IdRegistered -= All_IdRegistered;
            }
        }
    }
}

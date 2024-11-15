using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Framework.Save;

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
		var lowercaseName = name.ToLowerInvariant();
		return _idlookup.Values.ToList().Exists(x => x.Name?.ToLowerInvariant().Equals(lowercaseName) == true);
	}

	public T? Get(long id)
	{
		return _idlookup.TryGetValue(id, out var value) ? value : null;
	}

	public bool TryGet(long id, out T? result)
	{
		return _idlookup.TryGetValue(id, out result);
	}

	public List<T> Get(string name)
	{
		var values = _idlookup.Values.ToList();
		var lowercaseName = name.ToLowerInvariant();

		var matchesName = from value in values
		                  where value.Name?.ToLowerInvariant().Equals(lowercaseName) == true
		                  select value;

		return matchesName.ToList();
	}

	public T? GetByName(string name)
	{
		return _iterlist.FirstOrDefault(x => x.Name?.Equals(name, StringComparison.InvariantCultureIgnoreCase) == true) ??
		       _iterlist.FirstOrDefault(x => x.Name?.StartsWith(name, StringComparison.InvariantCultureIgnoreCase) == true);
	}

	public T? GetByIdOrName(string value, bool permitAbbreviations = true)
	{
		if (string.IsNullOrEmpty(value))
		{
			return null;
		}
		if (long.TryParse(value, out var id))
		{
			return Get(id);
		}

		if (!permitAbbreviations)
		{
			return _iterlist.FirstOrDefault(x => x.Name?.Equals(value, StringComparison.InvariantCultureIgnoreCase) == true);
		}

		return _iterlist.FirstOrDefault(x => x.Name?.Equals(value, StringComparison.InvariantCultureIgnoreCase) == true) ??
		       _iterlist.FirstOrDefault(x => x.Name?.StartsWith(value, StringComparison.InvariantCultureIgnoreCase) == true);
	}

	public void ForEach(Action<T> action)
	{
		foreach (var value in _idlookup.Values)
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
		_idlookup.TryGetValue(id, out var lookup);

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
		var numberRemoved = 0;
		foreach (var item in _iterlist.Where(predicateFunc).ToList())
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
		var sb = new StringBuilder();

		foreach (var item in _idlookup.Values.ToList())
		{
			sb.AppendLine(item.Id + ": " + item.Name.Proper());
		}

		return sb.ToString();
	}

	public T this[int key] => _iterlist[key];

	public void RemoveRange(int count)
	{
		var removed = _iterlist.Take(count).ToList();
		_iterlist.RemoveRange(0, count);
		foreach (var item in removed)
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
		var removed = _iterlist.Skip(index).Take(count).ToList();
		_iterlist.RemoveRange(index, count);
		foreach (var item in removed)
		{
			_idlookup.Remove(item.Id);
			if (item is ILateInitialisingItem lateSave)
			{
				lateSave.IdRegistered -= All_IdRegistered;
			}
		}
	}
}
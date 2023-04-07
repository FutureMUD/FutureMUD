using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework.Revision;

namespace MudSharp.Framework;

public class RevisableAll<T> : IRevisableAll<T>, IUneditableRevisableAll<T> where T : class, IRevisableItem
{
	private readonly Dictionary<Tuple<long, int>, T> _idlookup = new();
	private readonly List<T> _iterlist = new();
	private readonly CollectionDictionary<long, T> _groupingLookup = new();

	public void Clear()
	{
		_idlookup.Clear();
		_iterlist.Clear();
		_groupingLookup.Clear();
	}

	public bool Add(T item)
	{
		_iterlist.Add(item);
		_idlookup.Add(Tuple.Create(item.Id, item.RevisionNumber), item);
		_groupingLookup.Add(item.Id, item);
		return true;
	}

	#region IEnumerable<T> Members

	public IEnumerator<T> GetEnumerator()
	{
		return _iterlist.GetEnumerator();
	}

	#endregion

	#region IEnumerable Members

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	#endregion

	public bool Remove(T item)
	{
		_iterlist.Remove(item);
		_idlookup.Remove(Tuple.Create(item.Id, item.RevisionNumber));
		_groupingLookup.Remove(item.Id, item);
		return true;
	}

	#region IUneditableRevisableAll<T> Members

	public bool Has(T value)
	{
		return _iterlist.Contains(value);
	}

	public bool Has(long id)
	{
		return _iterlist.Any(x => x.Id == id);
	}

	public bool Has(long id, int revision)
	{
		return _idlookup.ContainsKey(Tuple.Create(id, revision));
	}

	public bool Has(string name)
	{
		return _iterlist.Any(x => x.Name.ToLowerInvariant() == name.ToLowerInvariant());
	}

	public dynamic Get<U>(long id) where U : class, IFrameworkItem
	{
		return typeof(U) != typeof(T) ? null : Get(id);
	}

	public T Get(long id)
	{
		return _iterlist.FirstOrDefault(x => x.Id == id && x.Status == RevisionStatus.Current) ??
		       (_iterlist.Any(x => x.Id == id)
			       ? _iterlist.Where(x => x.Id == id).FirstMax(x => x.RevisionNumber)
			       : null)
			;
	}

	public T Get(long id, Func<T, bool> filter)
	{
		var items = _iterlist.Where(x => x.Id == id && filter(x)).ToList();
		return items.FirstOrDefault(x => x.Status == RevisionStatus.Current) ??
		       items.FirstMax(x => x.RevisionNumber);
	}

	public T Get(long id, int revision)
	{
		return _idlookup.TryGetValue(Tuple.Create(id, revision), out var value) ? value : null;
	}

	public T Get(long id, int revision, Func<T, bool> filter)
	{
		if (_idlookup.TryGetValue(Tuple.Create(id, revision), out var value))
		{
			if (filter(value))
			{
				return value;
			}
		}

		return null;
	}

	public T GetByName(string name, bool ignoreCase = false)
	{
		var matches = _iterlist.Where(x => x.Name.EqualTo(name, ignoreCase)).ToList();
		return matches.FirstOrDefault(x => x.Status == RevisionStatus.Current) ??
		       (matches.Any() ? matches.FirstMax(x => x.RevisionNumber) : null);
	}

	public T GetByName(string name, int revision, bool ignoreCase = false)
	{
		return _iterlist.FirstOrDefault(x => x.RevisionNumber == revision && name.EqualTo(name, ignoreCase));
	}

	public T? GetByIdOrName(string value, bool permitAbbreviations = true)
	{
		if (long.TryParse(value, out var id))
		{
			return Get(id);
		}

		List<T> matches;
		if (!permitAbbreviations)
		{
			matches = _iterlist.Where(x => x.Name.Equals(value, StringComparison.InvariantCultureIgnoreCase)).ToList();
		}
		else
		{
			matches = _iterlist.Where(x => x.Name.Equals(value, StringComparison.InvariantCultureIgnoreCase))
			                   .ToList();
			if (!matches.Any())
			{
				matches = _iterlist.Where(x => x.Name.StartsWith(value, StringComparison.InvariantCultureIgnoreCase))
				                   .ToList();
			}
		}

		return matches.FirstOrDefault(x => x.Status == RevisionStatus.Current) ??
		       (matches.Any() ? matches.FirstMax(x => x.RevisionNumber) : null);
	}

	public List<T> GetAllByIdOrName(string value, bool permitAbbreviations = true)
	{
		if (long.TryParse(value, out var id))
		{
			return GetAll(id);
		}

		List<long> matches;
		if (!permitAbbreviations)
		{
			matches = _iterlist.Where(x => x.Name.EqualTo(value)).Select(x => x.Id).Distinct().ToList();
		}
		else
		{
			matches = _iterlist
			          .Where(x => x.Name.EqualTo(value) ||
			                      x.Name.StartsWith(value, StringComparison.InvariantCultureIgnoreCase))
			          .Select(x => x.Id)
			          .Distinct().ToList();
		}

		var list = new List<T>();
		foreach (var match in matches)
		{
			list.AddRange(GetAll(match));
		}

		return list;
	}

	public T TryGet(long id, out T result)
	{
		result = _iterlist.FirstOrDefault(x => x.Id == id && x.Status == RevisionStatus.Current) ??
		         (_iterlist.Any(x => x.Id == id)
			         ? _iterlist.Where(x => x.Id == id).FirstMax(x => x.RevisionNumber)
			         : null);
		return result;
	}

	public T TryGet(long id, int revision, out T result)
	{
		return _idlookup.TryGetValue(Tuple.Create(id, revision), out result) ? result : null;
	}

	public List<T> Get(string name)
	{
		return _iterlist.Where(x => x.Name.EqualTo(name)).ToList();
	}

	public List<T> GetAll(long id)
	{
		return _iterlist.Where(x => x.Id == id).ToList();
	}

	public List<T> GetAll(long id, Func<T, bool> filter)
	{
		return _iterlist.Where(x => x.Id == id && filter(x)).ToList();
	}

	public void ForEach(Action<T> action)
	{
		_iterlist.ForEach(action);
	}

	public List<T> GetAllByStatus(params RevisionStatus[] statuses)
	{
		var list = new List<T>();
		foreach (var item in _groupingLookup)
		{
			list.AddRange(item.Value.Where(x => x.Status.In(statuses)));
		}

		return list;
	}

	public List<T> GetAllApprovedOrMostRecent(bool includeAtLeastOne)
	{
		var list = new List<T>();
		foreach (var item in _groupingLookup.Values)
		{
			if (item.Count == 0)
			{
				continue;
			}

			var currentFound = false;
			var priorCount = list.Count;
			foreach (var rev in item.OrderByDescending(x => x.RevisionNumber))
			{
				if (rev.Status == RevisionStatus.Obsolete || rev.Status == RevisionStatus.Revised ||
				    (currentFound && rev.Status == RevisionStatus.Rejected))
				{
					break;
				}

				if (rev.Status == RevisionStatus.Current)
				{
					currentFound = true;
				}

				list.Add(rev);
			}

			if (includeAtLeastOne && !currentFound && priorCount == list.Count)
			{
				list.Add(item.WhereMax(x => x.RevisionNumber).First());
			}
		}

		return list;
	}

	#endregion
}
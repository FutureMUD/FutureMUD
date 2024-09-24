using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using MudSharp.Character.Name;
using MudSharp.Community;
using MudSharp.Form.Shape;

namespace MudSharp.Framework;

public static class KeywordedItemExtensions
{
	public static T GetFromItemListByKeyword<T>(this IEnumerable<T> itemList, string pattern, IPerceiver voyeur,
		bool abbreviated = true) where T : IKeyworded
	{
		if (string.IsNullOrEmpty(pattern))
		{
			return default;
		}

		var strings = pattern.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
		                     .Select(x => x.ToLowerInvariant()).ToArray();
		if (!strings.Any())
		{
			return default;
		}

		if (strings[0][0] == '#' && int.TryParse(strings[0].RemoveFirstCharacter(), out var number))
		{
			var keywords = strings.Skip(1).ToList();
			if (!keywords.Any())
			{
				return itemList.ElementAtOrDefault(number - 1);
			}

			return (
				from item in itemList
				where ItemMatches(item, keywords, voyeur, abbreviated, false)
				select item).ElementAtOrDefault(number - 1);
		}

		if (int.TryParse(strings[0], out number))
		{
			var keywords = strings.Skip(1).ToList();
			if (!keywords.Any())
			{
				return default;
			}

			return (
				from item in itemList
				where ItemMatches(item, keywords, voyeur, abbreviated, false)
				select item).ElementAtOrDefault(number - 1);
		}

		return (from item in itemList
		        where ItemMatches(item, strings, voyeur, abbreviated, false)
		        select item).FirstOrDefault() ??
			   (from item in itemList
			    where ItemMatches(item, strings, voyeur, abbreviated, false, true)
			    select item).FirstOrDefault();
	}

	private static bool ItemMatches<T>(T item, IEnumerable<string> keywords, IPerceiver voyeur, bool abbreviated,
		bool includeNames, bool useContainsOverStartsWith = false) where T : IKeyworded
	{
		var ihpn = item as IHavePersonalName;
		var ifi = item as IFrameworkItem;
		var pn = ihpn?.PersonalName.GetName(NameStyle.FullName);
		foreach (var keyword in keywords)
		{
			if (item.HasKeyword(keyword, voyeur, abbreviated, useContainsOverStartsWith))
			{
				continue;
			}

			if (voyeur.HasDubFor(item, keyword))
			{
				continue;
			}

			if (includeNames)
			{
				if (ihpn is not null)
				{
					if (abbreviated)
					{
						if (useContainsOverStartsWith)
						{
							if (pn.Contains(keyword, StringComparison.InvariantCultureIgnoreCase))
							{
								continue;
							}
						}
						else
						{
							if (pn.StartsWith(keyword, StringComparison.InvariantCultureIgnoreCase))
							{
								continue;
							}
						}
					}
					else
					{
						if (pn.Equals(keyword, StringComparison.InvariantCultureIgnoreCase))
						{
							continue;
						}
					}
				}
				else if (ifi is not null)
				{
					if (abbreviated)
					{
						if (useContainsOverStartsWith)
						{
							if (ifi.Name.Contains(keyword, StringComparison.InvariantCultureIgnoreCase))
							{
								continue;
							}
						}
						else
						{
							if (ifi.Name.StartsWith(keyword, StringComparison.InvariantCultureIgnoreCase))
							{
								continue;
							}
						}
					}
					else
					{
						if (ifi.Name.Equals(keyword, StringComparison.InvariantCultureIgnoreCase))
						{
							continue;
						}
					}
				}
			}

			return false;
		}

		return true;
	}

	public static T GetFromItemListByKeywordIncludingNames<T>(this IEnumerable<T> itemList, string pattern,
		IPerceiver voyeur,
		bool abbreviated = true) where T : IKeyworded
	{
		if (string.IsNullOrEmpty(pattern))
		{
			return default;
		}

		var strings = pattern.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
		                     .Select(x => x.ToLowerInvariant()).ToArray();
		if (!strings.Any())
		{
			return default;
		}

		if (strings[0][0] == '#' && int.TryParse(strings[0].RemoveFirstCharacter(), out var number))
		{
			var keywords = strings.Skip(1).ToList();
			return keywords.Any()
				? (from item in itemList
				   where ItemMatches(item, keywords, voyeur, abbreviated, true)
				   select item).ElementAtOrDefault(number - 1)
				: itemList.ElementAtOrDefault(number - 1);
		}

		if (int.TryParse(strings[0], out number))
		{
			var keystrings = strings.Skip(1).ToList();
			return (from item in itemList
			        where ItemMatches(item, keystrings, voyeur, abbreviated, true)
			        select item).ElementAtOrDefault(number - 1);
		}

		return (from item in itemList
		        where ItemMatches(item, strings.ToArray(), voyeur, abbreviated, true)
		        select item).FirstOrDefault() ??
			   (from item in itemList
			    where ItemMatches(item, strings.ToArray(), voyeur, abbreviated, true, true)
			    select item).FirstOrDefault();
	}
}

public abstract class KeywordedItem : FrameworkItem, IKeywordedItem
{
	protected Lazy<List<string>> _keywords;

	public virtual Gendering Gender => Neuter.Instance;

	public virtual IEnumerable<string> Keywords => _keywords?.Value ?? [_name];

	public virtual IEnumerable<string> GetKeywordsFor(IPerceiver voyeur)
	{
		return _keywords == null ? [_name] : _keywords.Value;
	}

	public virtual bool HasKeyword(string targetKeyword, IPerceiver voyeur, bool abbreviated = true, bool useContainsOverStartsWith = false)
	{
		var keywords = GetKeywordsFor(voyeur);
		if (!abbreviated)
		{
			return keywords.Any(x => x.EqualTo(targetKeyword));
		}

		if (useContainsOverStartsWith)
		{
			return keywords.Any(x => x.Contains(targetKeyword, StringComparison.InvariantCultureIgnoreCase));
		}

		return keywords.Any(x => x.StartsWith(targetKeyword, StringComparison.InvariantCultureIgnoreCase));
	}

	public virtual bool HasKeywords(IEnumerable<string> targetKeywords, IPerceiver voyeur, bool abbreviated = true, bool useContainsOverStartsWith = false)
	{
		var keywords = GetKeywordsFor(voyeur);
		if (!abbreviated)
		{
			return targetKeywords.All(x => keywords.Any(y => y.EqualTo(x)));
		}

		if (useContainsOverStartsWith)
		{
			return targetKeywords.All(x =>
				keywords.Any(y => y.Contains(x, StringComparison.InvariantCultureIgnoreCase)));
		}

		return targetKeywords.All(x =>
			keywords.Any(y => y.StartsWith(x, StringComparison.InvariantCultureIgnoreCase)));
	}

	protected IEnumerable<string> GetKeywordsFromSDesc(string sdesc)
	{
		var keywords = new List<string>();
		var explode = new ExplodedString(sdesc);
		foreach (var keyword in explode.Words)
		{
			if (keyword.EqualToAny("a", "an", "the"))
			{
				continue;
			}

			keywords.Add(keyword);
		}

		return keywords;
	}

	protected void SetKeywordsFromSDesc(string sdesc)
	{
		_keywords = new Lazy<List<string>>(() => GetKeywordsFromSDesc(sdesc).ToList());
	}
}

public class DummyKeywordedItem<T> : IKeyworded
{
	public T Object { get; set; }

	public DummyKeywordedItem(T obj, Func<T, List<string>> keywordsFunc)
	{
		Object = obj;
		Keywords = keywordsFunc(obj).ToList();
	}

	#region Implementation of IKeyworded

	public IEnumerable<string> Keywords { get; }

	public IEnumerable<string> GetKeywordsFor(IPerceiver voyeur)
	{
		return Keywords;
	}

	#endregion
}
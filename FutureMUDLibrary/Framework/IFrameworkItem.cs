using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MudSharp.Framework.Revision;

namespace MudSharp.Framework {
    public static class FrameworkItemExtensions {
        public static bool FrameworkItemEquals(this IFrameworkItem item, long? id, string type) {
            if (item == null) {
                return (id ?? 0) == 0;
            }

            return (item.Id == id) && item.FrameworkItemType.Equals(type, StringComparison.InvariantCulture);
        }
    }

    public class FrameworkItemReference
    {
	    public FrameworkItemReference()
	    {
		    
	    }

	    public FrameworkItemReference(string reference, IFuturemud gameworld)
	    {
		    Gameworld = gameworld;
		    Id = long.Parse(reference.Split('|', 2)[0]);
		    FrameworkItemType = reference.Split('|', 2)[1];
	    }

	    public FrameworkItemReference(long id, string frameworkItemType, IFuturemud gameworld)
	    {
		    Id = id;
		    FrameworkItemType = frameworkItemType;
		    Gameworld = gameworld;
	    }

	    public long Id { get; init; }
	    public string FrameworkItemType { get; init; }
        public IFuturemud Gameworld { get; init; }

        #region Overrides of Object

        public override int GetHashCode()
        {
	        return HashCode.Combine(Id, FrameworkItemType);
        }

        public override bool Equals(object? obj)
        {
	        if (obj is IFrameworkItem item)
	        {
		        return item.FrameworkItemType.Equals(FrameworkItemType, StringComparison.OrdinalIgnoreCase) &&
		               item.Id == Id;
	        }
	        return base.Equals(obj);
        }

        public override string ToString()
        {
	        return $"{Id:F0}|{FrameworkItemType}";
        }

        #endregion

        public IFrameworkItem GetItem
	    {
		    get
		    {
			    switch (FrameworkItemType)
			    {
                    case "Character":
	                    return Gameworld.TryGetCharacter(Id, true);
                    case "GameItem":
	                    return Gameworld.TryGetItem(Id);
                    case "Shop":
	                    return Gameworld.Shops.Get(Id);
                    case "Clan":
	                    return Gameworld.Clans.Get(Id);
                    default:
	                    throw new ApplicationException($"Unsupported framework item type '{FrameworkItemType}' in FrameworkItemReference.GetItem()");
			    }
		    }
	    }
    }

    public interface IFrameworkItem {
        string Name { get; }

        long Id { get; }

        string FrameworkItemType { get; }
    }

    public interface IRevisableItem : IFrameworkItem {
        int RevisionNumber { get; }
        RevisionStatus Status { get; }
    }

    public interface IKeyworded {
        IEnumerable<string> Keywords { get; }

        public IEnumerable<string> GetKeywordsFor(IPerceiver voyeur)
        {
	        return Keywords;
        }

        public bool HasKeyword(string targetKeyword, IPerceiver voyeur, bool abbreviated = false)
        {
	        var keywords = GetKeywordsFor(voyeur).ToArray();
	        return 
                abbreviated ?
	                keywords.Any(x => x.StartsWith(targetKeyword, StringComparison.InvariantCultureIgnoreCase)) :
	                keywords.Any(x => x.EqualTo(targetKeyword));
        }

        public bool HasKeywords(IEnumerable<string> targetKeywords, IPerceiver voyeur, bool abbreviated = false)
        {
	        var keywords = GetKeywordsFor(voyeur).ToArray();
	        return abbreviated
		        ? targetKeywords.All(x =>
			        keywords.Any(y => y.StartsWith(x, StringComparison.InvariantCultureIgnoreCase)))
		        : targetKeywords.All(x => keywords.Any(y => y.EqualTo(x)));
        }
    }

    public interface IKeywordedItem : IFrameworkItem, IKeyworded {
	    public new IEnumerable<string> Keywords => new ExplodedString(Name).Words;
    }
}
using System;

#nullable enable

namespace MudSharp.Framework;

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
    public string FrameworkItemType { get; init; } = null!;
    public IFuturemud Gameworld { get; init; } = null!;

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

    public IFrameworkItem? GetItem
    {
        get
        {
            switch (FrameworkItemType)
            {
                case "Character":
                    return Gameworld.TryGetCharacter(Id, true);
                case "GameItem":
                    return Gameworld.TryGetItem(Id);
                case "Estate":
                    return Gameworld.Estates.Get(Id);
                case "Bank":
                    return Gameworld.Banks.Get(Id);
                case "BankAccount":
                    return Gameworld.BankAccounts.Get(Id);
                case "LineOfCreditAccount":
                    return Gameworld.LineOfCreditAccounts.Get(Id);
                case "Property":
                    return Gameworld.Properties.Get(Id);
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

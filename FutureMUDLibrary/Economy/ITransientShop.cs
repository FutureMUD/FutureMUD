using MudSharp.GameItems.Interfaces;

namespace MudSharp.Economy;

public interface ITransientShop : IShop
    {
        IShopStall? CurrentStall { get; set; }
    }

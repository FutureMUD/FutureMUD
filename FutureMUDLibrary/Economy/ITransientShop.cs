using MudSharp.GameItems.Interfaces;

#nullable enable

namespace MudSharp.Economy;

public interface ITransientShop : IShop
{
    IShopStall? CurrentStall { get; set; }
}

using MudSharp.Economy;

#nullable enable
namespace MudSharp.GameItems.Interfaces;

public interface IShopStall : IGameItemComponent, IContainer
    {
	ITransientShop? Shop { get; set; }
	bool IsTrading { get; set; }
}
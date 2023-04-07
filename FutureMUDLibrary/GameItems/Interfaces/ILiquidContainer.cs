using MudSharp.Character;
using MudSharp.Form.Material;

namespace MudSharp.GameItems.Interfaces {
    public interface ILiquidContainer : IGameItemComponent, IOpenable {
        LiquidMixture LiquidMixture { get; set; }
        double LiquidCapacity { get; }
        void AddLiquidQuantity(double amount, ICharacter who, string action);
        void ReduceLiquidQuantity(double amount, ICharacter who, string action);
        void MergeLiquid(LiquidMixture otherMixture, ICharacter who, string action);
        LiquidMixture RemoveLiquidAmount(double amount, ICharacter who, string action);
        double LiquidVolume { get; }
        bool CanBeEmptiedWhenInRoom { get; }
    }
}
using MudSharp.Character;
using MudSharp.Form.Material;

namespace MudSharp.GameItems.Interfaces
{
    public interface ILiquidContainer : IGameItemComponent, IOpenable
    {
        /// <summary>False for projection-only containers such as liquid-grid views.</summary>
        bool OwnsLiquidMixture => true;
        LiquidMixture LiquidMixture { get; set; }
        double LiquidCapacity { get; }
        void AddLiquidQuantity(double amount, ICharacter who, string action);
        void ReduceLiquidQuantity(double amount, ICharacter who, string action);
        void MergeLiquid(LiquidMixture otherMixture, ICharacter who, string action);
        LiquidMixture RemoveLiquidAmount(double amount, ICharacter who, string action);
        double LiquidVolume { get; }
        bool CanBeEmptiedWhenInRoom { get; }

    }

    public static class LiquidContainerFreshnessExtensions
    {
        public static void ResolveLiquidFreshness(this ILiquidContainer container, System.DateTime utcNow)
        {
			if (!container.OwnsLiquidMixture || container.LiquidMixture?.ResolveFreshness(utcNow,
					container.Parent.TimeRateMultiplier(ItemTimeRateType.LiquidFreshness)) != true)
			{
				return;
			}

			container.Changed = true;
        }
    }
}

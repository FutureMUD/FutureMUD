using MudSharp.Form.Material;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.GameItems.Interfaces
{
#nullable enable
    public interface ICommodity : IGameItemComponent
    {
        ISolid Material { get; set; }
        double Weight { get; set; }
        ITag? Tag { get; set; }
        bool UseIndirectQuantityDescription { get; set; }
        IReadOnlyDictionary<ICharacteristicDefinition, ICharacteristicValue> CommodityCharacteristics { get; }
        ICommoditySpoilageRule? ActiveSpoilageRule { get; }
        DateTime? SpoilageTime { get; }
        ICharacteristicValue? GetCommodityCharacteristic(ICharacteristicDefinition definition);
        bool SetCommodityCharacteristic(ICharacteristicDefinition definition, ICharacteristicValue value);
        bool RemoveCommodityCharacteristic(ICharacteristicDefinition definition);
        void ClearCommodityCharacteristics();
        void EvaluateSpoilageRule();
        void CopySpoilageFrom(ICommodity other);
        bool CanMergeSpoilage(ICommodity other);
        void MergeSpoilageFrom(ICommodity other);
        bool CheckSpoilage(DateTime currentTime);

    }
#nullable restore
}

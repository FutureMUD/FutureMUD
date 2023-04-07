using MudSharp.GameItems;

namespace MudSharp.RPG.Merits.Interfaces {
    public interface INaturalAttackQualityMerit : ICharacterMerit {
        ItemQuality GetQuality(ItemQuality baseQuality);
    }
}
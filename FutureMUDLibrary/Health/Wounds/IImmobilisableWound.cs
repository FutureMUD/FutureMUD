using MudSharp.GameItems;

namespace MudSharp.Health.Wounds
{
    public interface IImmobilisableWound : IWound
    {
        IGameItem ImmobilisingItem { get; set; }
    }
}

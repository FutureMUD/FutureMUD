using MudSharp.Body;
using MudSharp.GameItems;

namespace MudSharp.Effects.Interfaces
{
    public interface IRestraintEffect : IEffect
    {
        IBody BodyOwner { get; set; }
        IGameItem TargetItem { get; set; }
        IGameItem RestraintItem { get; set; }
    }
}
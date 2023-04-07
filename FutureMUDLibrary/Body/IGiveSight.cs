using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Form.Shape;
using MudSharp.GameItems;

namespace MudSharp.Body {
    public enum SightResult {
        TrueSight, // i.e. admin
        Normal,
        Indistinct,
        Silhouette,
        CannotSee
    }

    public interface IGiveSight {
        SightResult CanSee(ICharacter actor);
        SightResult CanSee(ILocation location);
        SightResult CanSee(ILocateable locatable);
        SightResult CanSee(IGameItem item);
        SightResult CanSee(ICelestialObject celestial);
    }
}
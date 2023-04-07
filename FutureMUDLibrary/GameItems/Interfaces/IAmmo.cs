using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Interfaces {
    public interface IAmmo : IGameItemComponent {
        IAmmunitionType AmmoType { get; }
        /// <summary>
        /// Loads an IGameItem, if different to the base item, that is the item that gets "lodged" or scattered once fired. Example - bullet
        /// </summary>
        IGameItem GetFiredItem { get; }

        /// <summary>
        /// Loads an IGameItem, if different to the base item, that is the item that gets discarded from the weapon after firing. Example - shell casing
        /// </summary>
        IGameItem GetFiredWasteItem { get; }

        void Fire(ICharacter actor, IPerceiver target, Outcome shotOutcome, Outcome coverOutcome, OpposedOutcome defenseOutcome, IBodypart bodypart, IGameItem ammo, IRangedWeaponType weaponType, IEmoteOutput defenseEmote);
    }
}
using System.Collections.Generic;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Interfaces {

    public interface IRangedWeapon : IWieldable, IDamageSource, IUseTrait {
        string FireVerbForEchoes { get; }
        bool CanBeAimedAtSelf { get; }
        WeaponClassification Classification { get; }
        IRangedWeaponType WeaponType { get; }
        bool ReadyToFire { get; }
        int LoadStage { get; }
        bool IsLoaded { get; }
        bool IsReadied { get; }
        IEnumerable<IGameItem> MagazineContents { get; }
        Difficulty AimDifficulty { get; }
        Difficulty BaseBlockDifficulty { get; }
        Difficulty BaseDodgeDifficulty { get; }
        bool CanLoad(ICharacter loader, bool ignoreEmpty = false);
        string WhyCannotLoad(ICharacter loader, bool ignoreEmpty = false);
        void Load(ICharacter loader, bool ignoreEmpty = false);
        bool CanReady(ICharacter readier);
        string WhyCannotReady(ICharacter readier);
        bool Ready(ICharacter readier);
        bool CanUnready(ICharacter readier);
        string WhyCannotUnready(ICharacter readier);
        bool Unready(ICharacter readier);
        bool CanUnload(ICharacter loader);
        string WhyCannotUnload(ICharacter loader);
        IEnumerable<IGameItem> Unload(ICharacter loader);
        bool CanFire(ICharacter actor, IPerceivable target);
        string WhyCannotFire(ICharacter actor, IPerceivable target);

        void Fire(ICharacter actor, IPerceiver target, Outcome shotOutcome, Outcome coverOutcome, OpposedOutcome defenseOutcome, IBodypart bodypart, IEmoteOutput defenseEmote, IPerceiver originalTarget);
    }
}
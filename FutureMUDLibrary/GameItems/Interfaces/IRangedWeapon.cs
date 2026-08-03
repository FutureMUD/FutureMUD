using System;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
using System.Collections.Generic;

namespace MudSharp.GameItems.Interfaces;

public enum LoadMode
{
    Normal,
    Blank,
    NoClean,
    Tap,
    TapNoClean
}

/// <summary>
/// The common operational surface for both hand-held and emplaced ranged weapons.
/// </summary>
/// <remarks>
/// Historically all ranged weapons were <see cref="IWieldable"/>. Artillery and wall weapons
/// need the same loading and firing pipeline without pretending that a character can wield them.
/// </remarks>
public interface IRangedWeaponPlatform : IGameItemComponent, IUseTrait
{
	/// <summary>Metadata used by the common aim and firing calculations.</summary>
	IRangedWeaponType WeaponType => (this as IRangedWeapon)?.WeaponType ??
		throw new InvalidOperationException("A non-wieldable ranged platform must expose its ranged weapon type.");
	WeaponClassification Classification => (this as IRangedWeapon)?.Classification ??
		throw new InvalidOperationException("A non-wieldable ranged platform must expose its classification.");
    string FireVerbForEchoes { get; }
    bool CanBeAimedAtSelf { get; }
    bool CanFireWhileHidden => false;
    bool ReadyToFire { get; }
    int LoadStage { get; }
    bool IsLoaded { get; }
    bool IsReadied { get; }
    IEnumerable<IGameItem> MagazineContents { get; }
    IEnumerable<IGameItem> AllContainedItems { get; }
    Difficulty AimDifficulty { get; }
    Difficulty BaseBlockDifficulty { get; }
    Difficulty BaseDodgeDifficulty { get; }
    bool CanLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal);
    string WhyCannotLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal);
    void Load(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal);
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

/// <summary>
/// A ranged weapon which is operated by wielding it. Emplaced weapons implement
/// <see cref="IRangedWeaponPlatform"/> directly instead.
/// </summary>
public interface IRangedWeapon : IRangedWeaponPlatform, IWieldable
{
	new WeaponClassification Classification { get; }
	new IRangedWeaponType WeaponType { get; }
}

/// <summary>
/// A handheld-profile ranged weapon that must be physically set in a cell before use.
/// Wall crossbows use this instead of inheriting the crew-served artillery contract.
/// </summary>
public interface IEmplaceableRangedWeapon : IRangedWeapon
{
	bool IsEmplaced { get; }
	bool Emplace(ICharacter actor, out string reason);
	bool Limber(ICharacter actor, out string reason);
}

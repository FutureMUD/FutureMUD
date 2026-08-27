using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using System;
using System.Collections.Generic;

#nullable enable

namespace MudSharp.GameItems.Interfaces;

public enum ArtilleryLoadingMechanism
{
	MuzzleLoading,
	RemovableChamber,
	BreechLoading,
	DropFireMortar
}

/// <summary>
/// The persisted state of an artillery drill. Values are deliberately ordered so that
/// a component can reject skipped stages when loading legacy or hand-authored XML.
/// </summary>
public enum ArtilleryLoadingStage
{
	Empty,
	Cleared,
	Charged,
	Wadded,
	ProjectileLoaded,
	Rammed,
	Vented,
	Primed
}

public enum MusketIgnitionFamily
{
	Flintlock = 0,
	Matchlock,
	Wheellock,
	Snaphaunce,
	Doglock,
	Miquelet,
}

public enum ArtilleryPayloadType
{
	SolidShot,
	StoneShot,
	BarShot,
	Grapeshot,
	CaseShot,
	Shell,
	Carcass
}

public enum ArtilleryCrewAction
{
	Command,
	Sponge,
	LoadCharge,
	LoadWad,
	LoadProjectile,
	Ram,
	Vent,
	Prime,
	Aim,
	Fire
}

public enum WeaponCarrierState
{
	Detached,
	Carried,
	Wielded,
	Hanging
}

/// <summary>
/// A validated spatial artillery target. The item component owns the current solution;
/// the path is represented by stable cell identifiers so it remains version tolerant.
/// </summary>
public sealed record ArtilleryFiringSolution(
	double Bearing,
	double Distance,
	double Elevation,
	double Traverse,
	IReadOnlyList<long> CellPath,
	long? RouteCellPositionId = null,
	TimeSpan? Fuse = null);

public interface IArtilleryPiece : IRangedWeaponPlatform
{
	new WeaponClassification Classification { get; }
	new IRangedWeaponType WeaponType { get; }
	ArtilleryLoadingMechanism LoadingMechanism { get; }
	ArtilleryLoadingStage LoadingStage { get; }
	ArtilleryFiringSolution? FiringSolution { get; }
	bool IsEmplaced { get; }
	bool IsMounted { get; }
	bool IsCrewedBy(ICharacter character);
	bool HasMinimumCrew { get; }
	IEnumerable<ICharacter> Crew { get; }
	IEnumerable<string> CrewRoles { get; }
	ArtilleryCrewAction? NextRequiredAction { get; }
	bool TryJoinCrew(ICharacter character, string role, out string reason);
	bool LeaveCrew(ICharacter character);
	bool CanPerform(ICharacter character, ArtilleryCrewAction action, out string reason);
	bool SetFiringSolution(ICharacter actor, ArtilleryFiringSolution solution, out string reason);
	bool SetFuse(ICharacter actor, TimeSpan? fuse, out string reason);
	void ClearFiringSolution();
	void Limber(ICharacter actor);
	void Emplace(ICharacter actor);
}

public interface IArtilleryAmmunition : IAmmo
{
	ArtilleryPayloadType PayloadType { get; }
	string ArtilleryProfile { get; }
}

public interface IArtilleryChamber : IGameItemComponent
{
	string ArtilleryProfile { get; }
	bool IsLoaded { get; }
	IArtilleryAmmunition? LoadedAmmunition { get; }
	bool TryLoad(IArtilleryAmmunition ammunition);
	IArtilleryAmmunition? Unload();
}

public interface IArtilleryMount : IGameItemComponent
{
	IGameItem? InstalledPiece { get; }
	bool IsFixed { get; }
	double TraverseArc { get; }
	double ElevationArc { get; }
	bool CanInstall(IArtilleryPiece piece);
	string WhyCannotInstall(IArtilleryPiece piece);
	bool Install(IArtilleryPiece piece);
	IGameItem? Remove();
}

public interface IWeaponCarrierAttachment : IGameItemComponent
{
	IGameItem? AttachedWeapon { get; }
	WeaponCarrierState State { get; }
	bool CanAttach(IGameItem weapon, ICharacter actor, out string reason);
	bool Attach(IGameItem weapon, ICharacter actor, out string reason);
	bool Detach(ICharacter actor, out string reason);
	bool TryRetain(IGameItem weapon, ICharacter actor);
}

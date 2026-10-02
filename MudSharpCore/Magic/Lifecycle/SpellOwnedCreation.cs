#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp.Magic.Lifecycle;

/// <summary>
/// A private creation transaction. Register added rows before SaveChanges; expose runtime entities only
/// after Create returns. Existing/borrowed rows and primary projection instances cannot acquire ownership.
/// </summary>
public sealed class SpellOwnedCreation
{
	private readonly List<(SpellOwnedEntityKind Kind, SpellOwnedEntityRole Role, object Row)> _claims = new();
	private readonly long _creatorId;
	internal SpellOwnedCreation(FuturemudDatabaseContext context, long creatorId)
	{
		Context = context;
		_creatorId = creatorId;
	}

	public FuturemudDatabaseContext Context { get; }

	public void Claim(SpellOwnedEntityKind kind, object row,
		SpellOwnedEntityRole role = SpellOwnedEntityRole.CreatedEntity)
	{
		if (!Enum.IsDefined(kind) || !Enum.IsDefined(role) ||
		    role == SpellOwnedEntityRole.GeneratedPossession && kind != SpellOwnedEntityKind.GameItem ||
		    Context.Entry(row).State != EntityState.Added || !Matches(kind, row) ||
		    _claims.Any(x => ReferenceEquals(x.Row, row)))
		{
			throw new InvalidOperationException("Spell ownership requires one exact newly added row of the declared kind.");
		}
		_claims.Add((kind, role, row));
	}

	internal void ValidateBeforeSave()
	{
		if (_claims.Count is 0 or > 256 || _claims.Any(x => Context.Entry(x.Row).State != EntityState.Added) ||
		    _claims.Count(x => x.Kind is SpellOwnedEntityKind.AutonomousCharacter or SpellOwnedEntityKind.CharacterInstance) > 1 ||
		    Context.ChangeTracker.Entries().Any(x => x.State is EntityState.Modified or EntityState.Deleted))
		{
			throw new InvalidOperationException("Creation must retain 1-256 added owned rows until the atomic save.");
		}
		foreach (var claim in _claims)
		{
			if (claim.Row is Models.Character character &&
			    (character.AccountId is not null || character.IsAdminAvatar ||
			     !Context.ChangeTracker.Entries<Npc>().Any(x => x.State == EntityState.Added &&
				     ReferenceEquals(x.Entity.Character, character))))
			{
				throw new InvalidOperationException("Only a newly created autonomous NPC identity can be spell owned.");
			}
			if (claim.Row is Models.CharacterInstance instance &&
			    (instance.IsPrimary || instance.CharacterId != _creatorId &&
			     !_claims.Any(x => x.Row is Models.Character character && ReferenceEquals(instance.Character, character))))
			{
				throw new InvalidOperationException("A projection can own only a new secondary instance, never its canonical identity.");
			}
		}
	}

	internal IReadOnlyList<SpellOwnedEntity> SavedClaims() => Array.AsReadOnly(_claims.Select(x =>
		new SpellOwnedEntity(x.Kind, Id(x.Row), x.Role)).ToArray());

	private static bool Matches(SpellOwnedEntityKind kind, object row) => kind switch
	{
		SpellOwnedEntityKind.GameItem => row is GameItem,
		SpellOwnedEntityKind.AutonomousCharacter => row is Models.Character,
		SpellOwnedEntityKind.CharacterInstance => row is Models.CharacterInstance,
		SpellOwnedEntityKind.Body => row is Models.Body,
		SpellOwnedEntityKind.Cell => row is Cell,
		SpellOwnedEntityKind.Exit => row is Exit,
		_ => false
	};

	private static long Id(object row) => row switch
	{
		GameItem x => x.Id, Models.Character x => x.Id, Models.CharacterInstance x => x.Id,
		Models.Body x => x.Id, Cell x => x.Id, Exit x => x.Id,
		_ => throw new InvalidOperationException("Unsupported owned entity row.")
	};
}

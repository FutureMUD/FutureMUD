#nullable enable

using MudSharp.Framework;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MudSharp.Character;

public enum CharacterActorReferenceKind
{
	IdentityOnly = 0,
	PrimaryInstance = 1,
	SpecificInstance = 2,
	BodyOnly = 3,
	FrameworkItem = 4
}

public enum CharacterActorReferenceResolutionStatus
{
	LoadedSpecificInstance = 0,
	LoadedPrimaryFallback = 1,
	IdentityUnavailable = 2,
	InstanceUnavailable = 3,
	BodyUnavailable = 4,
	InvalidReference = 5
}

public sealed record CharacterActorReference(
	long CharacterId,
	long? CharacterInstanceId = null,
	long? BodyId = null,
	CharacterActorReferenceKind ReferenceKind = CharacterActorReferenceKind.IdentityOnly)
{
	public bool HasSpecificInstance => CharacterInstanceId is > 0L;

	public static CharacterActorReference FromActor(ICharacter actor,
		CharacterActorReferenceKind? referenceKind = null)
	{
		var instanceId = CharacterInstanceIdentityComparer.InstanceId(actor);
		return new CharacterActorReference(
			CharacterInstanceIdentityComparer.IdentityId(actor),
			instanceId,
			actor.Body?.Id,
			referenceKind ?? (instanceId is > 0L
				? CharacterActorReferenceKind.SpecificInstance
				: CharacterActorReferenceKind.IdentityOnly));
	}
}

public sealed record CharacterActorReferenceResolution(
	CharacterActorReference Reference,
	CharacterActorReferenceResolutionStatus Status,
	ICharacter? Actor,
	string Message)
{
	public bool FoundActor => Actor is not null;
	public bool IsStale => Status is CharacterActorReferenceResolutionStatus.IdentityUnavailable or
		CharacterActorReferenceResolutionStatus.InstanceUnavailable or
		CharacterActorReferenceResolutionStatus.BodyUnavailable or
		CharacterActorReferenceResolutionStatus.InvalidReference;
}

public static class CharacterActorReferenceExtensions
{
	public static CharacterActorReferenceResolution ResolveActorReference(
		this IFuturemud gameworld,
		CharacterActorReference reference,
		bool loadIdentityIfNeeded = true)
	{
		if (reference.CharacterId <= 0 && reference.BodyId is null or <= 0)
		{
			return new CharacterActorReferenceResolution(reference,
				CharacterActorReferenceResolutionStatus.InvalidReference,
				null,
				"The actor reference does not identify a character identity or body.");
		}

		var identity = reference.CharacterId > 0
			? gameworld.TryGetCharacter(reference.CharacterId, loadIdentityIfNeeded) ??
			  gameworld.Characters?.Get(reference.CharacterId)
			: null;
		if (identity is null && reference.ReferenceKind != CharacterActorReferenceKind.BodyOnly)
		{
			return new CharacterActorReferenceResolution(reference,
				CharacterActorReferenceResolutionStatus.IdentityUnavailable,
				null,
				$"Character identity #{reference.CharacterId.ToString("N0", CultureInfo.InvariantCulture)} is not loaded or could not be loaded.");
		}

		var instances = identity?.Identity?.Instances
		                       .OfType<ICharacter>()
		                       .ToList() ?? new List<ICharacter>();

		if (reference.ReferenceKind == CharacterActorReferenceKind.BodyOnly)
		{
			var bodyActor = instances.FirstOrDefault(x => x.Body?.Id == reference.BodyId);
			return bodyActor is null
				? new CharacterActorReferenceResolution(reference,
					CharacterActorReferenceResolutionStatus.BodyUnavailable,
					null,
					$"No loaded actor owns body #{(reference.BodyId ?? 0L).ToString("N0", CultureInfo.InvariantCulture)}.")
				: new CharacterActorReferenceResolution(reference,
					CharacterActorReferenceResolutionStatus.LoadedSpecificInstance,
					bodyActor,
					string.Empty);
		}

		if (reference.ReferenceKind == CharacterActorReferenceKind.PrimaryInstance ||
		    reference.CharacterInstanceId is null or <= 0L)
		{
			return new CharacterActorReferenceResolution(reference,
				reference.CharacterInstanceId is null or <= 0L
					? CharacterActorReferenceResolutionStatus.LoadedPrimaryFallback
					: CharacterActorReferenceResolutionStatus.LoadedSpecificInstance,
				identity,
				string.Empty);
		}

		var actor = instances.FirstOrDefault(x => x.InstanceId == reference.CharacterInstanceId.Value);
		if (actor is null)
		{
			return new CharacterActorReferenceResolution(reference,
				CharacterActorReferenceResolutionStatus.InstanceUnavailable,
				null,
				$"Character instance #{reference.CharacterInstanceId.Value.ToString("N0", CultureInfo.InvariantCulture)} for identity #{reference.CharacterId.ToString("N0", CultureInfo.InvariantCulture)} is not loaded.");
		}

		if (reference.BodyId is > 0L && actor.Body?.Id != reference.BodyId.Value)
		{
			return new CharacterActorReferenceResolution(reference,
				CharacterActorReferenceResolutionStatus.BodyUnavailable,
				null,
				$"Character instance #{reference.CharacterInstanceId.Value.ToString("N0", CultureInfo.InvariantCulture)} is loaded with body #{actor.Body?.Id.ToString("N0", CultureInfo.InvariantCulture) ?? "none"} instead of expected body #{reference.BodyId.Value.ToString("N0", CultureInfo.InvariantCulture)}.");
		}

		return new CharacterActorReferenceResolution(reference,
			CharacterActorReferenceResolutionStatus.LoadedSpecificInstance,
			actor,
			string.Empty);
	}

	public static string RenderStaffActorReference(this ICharacter? actor)
	{
		if (actor is null)
		{
			return "missing actor".ColourError();
		}

		var parts = new List<string>
		{
			actor.HowSeen(actor, colour: false, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreSelf),
			$"identity #{CharacterInstanceIdentityComparer.IdentityId(actor).ToString("N0", CultureInfo.InvariantCulture)}",
			$"instance #{actor.InstanceId.ToString("N0", CultureInfo.InvariantCulture)}",
			$"body #{actor.Body?.Id.ToString("N0", CultureInfo.InvariantCulture) ?? "none"}",
			actor.InstanceKind.DescribeEnum(true),
			actor.Location is null
				? "nowhere"
				: $"cell #{actor.Location.Id.ToString("N0", CultureInfo.InvariantCulture)} {actor.RoomLayer.DescribeEnum()}"
		};

		return parts.ListToString(separator: " | ", conjunction: "", twoItemJoiner: " | ");
	}

	public static string RenderStaffActorReference(this CharacterActorReferenceResolution resolution)
	{
		if (resolution.Actor is not null)
		{
			return $"{resolution.Actor.RenderStaffActorReference()} | {resolution.Status.DescribeEnum()}";
		}

		var reference = resolution.Reference;
		var parts = new List<string>
		{
			$"identity #{reference.CharacterId.ToString("N0", CultureInfo.InvariantCulture)}",
			$"instance #{reference.CharacterInstanceId?.ToString("N0", CultureInfo.InvariantCulture) ?? "none"}",
			$"body #{reference.BodyId?.ToString("N0", CultureInfo.InvariantCulture) ?? "none"}",
			reference.ReferenceKind.DescribeEnum(true),
			resolution.Status.DescribeEnum(true)
		};
		if (!string.IsNullOrWhiteSpace(resolution.Message))
		{
			parts.Add(resolution.Message);
		}

		return parts.ListToString(separator: " | ", conjunction: "", twoItemJoiner: " | ");
	}
}

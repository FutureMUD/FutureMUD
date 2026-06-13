using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Framework;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Character;

public enum CharacterInstanceDiagnosticSeverity
{
	Info = 0,
	Warning = 1,
	Error = 2
}

public sealed record CharacterInstanceDiagnostic(
	CharacterInstanceDiagnosticSeverity Severity,
	CharacterInstanceStateScope Scope,
	string Code,
	string Message
);

public static class CharacterInstanceDiagnostics
{
	public static IReadOnlyList<CharacterInstanceDiagnostic> AuditPrimaryInstance(ICharacter character)
	{
		var diagnostics = new List<CharacterInstanceDiagnostic>();
		if (character is null)
		{
			diagnostics.Add(new CharacterInstanceDiagnostic(
				CharacterInstanceDiagnosticSeverity.Error,
				CharacterInstanceStateScope.Instance,
				"character-null",
				"Cannot audit a null character reference."
			));
			return diagnostics;
		}

		if (character.CurrentBody is null)
		{
			diagnostics.Add(new CharacterInstanceDiagnostic(
				CharacterInstanceDiagnosticSeverity.Error,
				CharacterInstanceStateScope.Body,
				"current-body-null",
				$"Character #{character.Id} has no current body."
			));
			return diagnostics;
		}

		if (!ReferenceEquals(character.Body, character.CurrentBody))
		{
			diagnostics.Add(new CharacterInstanceDiagnostic(
				CharacterInstanceDiagnosticSeverity.Error,
				CharacterInstanceStateScope.CompatibilityMirror,
				"body-currentbody-diverged",
				$"Character #{character.Id} has Body #{character.Body?.Id ?? 0} but CurrentBody #{character.CurrentBody.Id}."
			));
		}

		if (!ReferenceEquals(character.CurrentBody.Actor, character))
		{
			diagnostics.Add(new CharacterInstanceDiagnostic(
				CharacterInstanceDiagnosticSeverity.Error,
				CharacterInstanceStateScope.Instance,
				"body-actor-diverged",
				$"Body #{character.CurrentBody.Id} is attached to actor #{character.CurrentBody.Actor?.Id ?? 0} instead of character #{character.Id}."
			));
		}

		if (character.CurrentBody.Location != character.Location)
		{
			diagnostics.Add(new CharacterInstanceDiagnostic(
				CharacterInstanceDiagnosticSeverity.Error,
				CharacterInstanceStateScope.Instance,
				"body-location-diverged",
				$"Character #{character.Id} is in location #{character.Location?.Id ?? 0} but body #{character.CurrentBody.Id} reports location #{character.CurrentBody.Location?.Id ?? 0}."
			));
		}

		if (character.CurrentBody.RoomLayer != character.RoomLayer)
		{
			diagnostics.Add(new CharacterInstanceDiagnostic(
				CharacterInstanceDiagnosticSeverity.Error,
				CharacterInstanceStateScope.Instance,
				"body-layer-diverged",
				$"Character #{character.Id} is on layer {(int)character.RoomLayer} but body #{character.CurrentBody.Id} reports layer {(int)character.CurrentBody.RoomLayer}."
			));
		}

		if (!character.Bodies.Contains(character.CurrentBody))
		{
			diagnostics.Add(new CharacterInstanceDiagnostic(
				CharacterInstanceDiagnosticSeverity.Error,
				CharacterInstanceStateScope.Body,
				"current-body-not-owned",
				$"Character #{character.Id} has current body #{character.CurrentBody.Id}, but it is not present in the owned body list."
			));
		}

		if (!character.Forms.Any(x => ReferenceEquals(x.Body, character.CurrentBody)))
		{
			diagnostics.Add(new CharacterInstanceDiagnostic(
				CharacterInstanceDiagnosticSeverity.Warning,
				CharacterInstanceStateScope.Body,
				"current-body-no-form",
				$"Character #{character.Id} has current body #{character.CurrentBody.Id}, but no form metadata points at it."
			));
		}

		var primaryInstances = character.Identity.Instances
		                                .Where(x => x.IsPrimaryInstance)
		                                .ToList();
		if (primaryInstances.Count > 1)
		{
			diagnostics.Add(new CharacterInstanceDiagnostic(
				CharacterInstanceDiagnosticSeverity.Error,
				CharacterInstanceStateScope.Identity,
				"duplicate-primary-instance",
				$"Identity #{character.Identity.Id} has {primaryInstances.Count} loaded primary instances."
			));
		}

		foreach (var bodyGroup in character.Identity.Instances
		                                  .Where(x => x.IsEmbodied && !x.State.HasFlag(CharacterState.Dead))
		                                  .GroupBy(x => x.Body)
		                                  .Where(x => x.Key is not null && x.Count() > 1))
		{
			diagnostics.Add(new CharacterInstanceDiagnostic(
				CharacterInstanceDiagnosticSeverity.Error,
				CharacterInstanceStateScope.Body,
				"duplicate-embodied-body",
				$"Body #{bodyGroup.Key.Id} is embodied by {bodyGroup.Count()} loaded instances for identity #{character.Identity.Id}."
			));
		}

		return diagnostics;
	}

	public static IReadOnlyList<CharacterInstanceDiagnostic> AuditPersistedInstances(
		IEnumerable<MudSharp.Models.CharacterInstance> instances)
	{
		var diagnostics = new List<CharacterInstanceDiagnostic>();
		var list = instances?.ToList() ?? new List<MudSharp.Models.CharacterInstance>();

		foreach (var group in list.Where(x => x.IsPrimary)
		                          .GroupBy(x => x.CharacterId)
		                          .Where(x => x.Count() > 1))
		{
			diagnostics.Add(new CharacterInstanceDiagnostic(
				CharacterInstanceDiagnosticSeverity.Error,
				CharacterInstanceStateScope.Identity,
				"duplicate-primary-instance",
				$"Identity #{group.Key} has {group.Count()} persisted primary instance rows."
			));
		}

		foreach (var group in list.Where(x =>
			                          x.IsEmbodied &&
			                          !((CharacterState)x.State).HasFlag(CharacterState.Dead) &&
			                          (CharacterStatus)x.Status != CharacterStatus.Deceased)
		                          .GroupBy(x => x.BodyId)
		                          .Where(x => x.Count() > 1))
		{
			diagnostics.Add(new CharacterInstanceDiagnostic(
				CharacterInstanceDiagnosticSeverity.Error,
				CharacterInstanceStateScope.Body,
				"duplicate-embodied-body",
				$"Body #{group.Key} has {group.Count()} live embodied instance rows."
			));
		}

		return diagnostics;
	}
}

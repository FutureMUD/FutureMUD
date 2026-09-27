#nullable enable

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Magic.Environment;

namespace MudSharp.Commands.Modules;

public partial class MagicModule
{
	private const string EnvironmentHelp = @"Environmental resources belong to physical cells. Configure their reusable profiles with #3magic regenerator#0.

	#3magic environment show [here|<cell id>]#0 - inspect profile, inputs, resources and damage
	#3magic environment treatments [here|<cell id>]#0 - inspect active, completed and unresolved land treatments
	#3magic environment treatments confirm <here|cell id> <treatment guid>#0 - reconcile durable evidence without applying repair
	#3magic environment yields [here|<cell id>]#0 - purely inspect authorised native organic sources and accounting
	#3magic environment yields repair [here|<cell id>] [crop|woodland|pasture|all]#0 - clear malformed extension accounting only
	#3magic environment cell <here|cell id> <inherit|disabled|profile>#0 - set a cell binding
	#3magic environment terrain <terrain> <profile|none>#0 - set a terrain default
	#3magic environment damage <here|cell id> <damage> <pressure> <operation guid> <reason>#0 - record staff damage and destructive-use pressure
	#3magic environment repair <here|cell id> <amount> <operation guid> <reason>#0 - repair an actual amount of scar damage
	#3magic environment recheck [here|<cell id>]#0 - request a bounded policy recheck
	#3magic environment diagnostics#0 - inspect coordinator work, queues and ages

Damage and repair require a non-empty GUID operation identity and an audit reason. Repeating that identity reuses its recorded result; it does not apply the operation again. Repair grants no resource and preserves the last destructive-use timestamp. Inspection never advances production.";

	public static void MagicEnvironment(ICharacter actor, StringStack command)
	{
		if (!actor.IsAdministrator())
		{
			actor.OutputHandler.Send("Only administrators can configure or inspect environmental magic.".ColourError());
			return;
		}

		var action = command.PopSpeech().ToLowerInvariant();
		if (action is "help" or "?" || action.Length == 0 && actor.Location == null)
		{
			actor.OutputHandler.Send(EnvironmentHelp.SubstituteANSIColour());
			return;
		}

		var service = actor.Gameworld.EnvironmentalMagic;
		if (service == null)
		{
			actor.OutputHandler.Send("The environmental magic coordinator is unavailable.".ColourError());
			return;
		}

		switch (action)
		{
			case "":
			case "show":
				EnvironmentShow(actor, command, service);
				return;
			case "cell":
				EnvironmentCell(actor, command, service);
				return;
			case "terrain":
				EnvironmentTerrain(actor, command);
				return;
			case "damage":
				EnvironmentOperation(actor, command, service, false);
				return;
			case "repair":
				EnvironmentOperation(actor, command, service, true);
				return;
			case "recheck":
				if (TryEnvironmentCell(actor, command, true, out var cell) && EnvironmentArgumentsFinished(actor, command))
				{
					service.MarkDirty(cell, EnvironmentalMagicDirtyReason.Policy);
					actor.OutputHandler.Send($"Requested an environmental recheck for cell #{cell.Id.ToString("N0", actor).ColourValue()}.");
				}

				return;
			case "diagnostics":
				if (EnvironmentArgumentsFinished(actor, command))
				{
					actor.OutputHandler.Send(service.DescribeDiagnostics());
				}

				return;
			case "yields":
				EnvironmentYields(actor, command, service);
				return;
			case "treatments":
				EnvironmentTreatments(actor, command, service);
				return;
			default:
				actor.OutputHandler.Send(EnvironmentHelp.SubstituteANSIColour());
				return;
		}
	}

	private static void EnvironmentTreatments(ICharacter actor, StringStack command, IEnvironmentalMagicService service)
	{
		var confirm = command.PeekSpeech().EqualTo("confirm");
		if (confirm) command.PopSpeech();
		if (!TryEnvironmentCell(actor, command, !confirm, out var cell)) return;
		if (confirm)
		{
			if (!Guid.TryParse(command.PopSpeech(), out var id) || id == Guid.Empty || !EnvironmentArgumentsFinished(actor, command))
			{
				actor.OutputHandler.Send("Use magic environment treatments confirm <here|cell id> <treatment guid>.".ColourError());
				return;
			}
			actor.OutputHandler.Send(service.ConfirmTreatment(cell, id, out var error)
				? "Treatment evidence reconciled. Confirmation applies no new repair; inspect its status below.".ColourValue()
				: (error ?? "Treatment confirmation failed.").ColourError());
		}
		else if (!EnvironmentArgumentsFinished(actor, command)) return;
		try
		{
			var sb = new StringBuilder($"Land Treatments — Cell #{cell.Id.ToString("N0", actor)}\n");
			var policy = service.InspectRepairPolicy(cell);
			sb.AppendLine($"Magical repair ceiling: {policy.Ceiling?.ToString("G", actor) ?? "none"}; {policy.Error ?? "repair policy valid"}");
			var records = service.InspectTreatments(cell);
			if (records.Count == 0) sb.AppendLine("No recorded land treatments.");
			foreach (var p in records)
			{
				sb.AppendLine($"{p.Id}: {p.Status.DescribeEnum()}, spell #{p.SpellId}, caster #{p.CasterId}, instance #{p.ActingInstanceId}");
				sb.AppendLine($"  Rate {p.Rate.ToString("G", actor)}/minute; budget {p.RemainingBudget.ToString("G", actor)}/{p.InitialBudget.ToString("G", actor)}; repaired {p.TotalRepaired.ToString("G", actor)}; lifetime {TimeSpan.FromSeconds(p.RemainingSeconds).Describe(actor)}; step {p.AcknowledgedSequence}/{p.Sequence}");
				sb.AppendLine($"  Pending operation: {p.PendingRequest?.OperationId.ToString() ?? "none"}; last confirmed: {p.LastOperationId?.ToString() ?? "none"}; cancellation: {p.CancellationRequested.ToColouredString()}");
				if (!string.IsNullOrEmpty(p.Diagnostic)) sb.AppendLine($"  {p.Diagnostic.ColourError()}");
			}
			actor.OutputHandler.Send(sb.ToString());
		}
		catch (Exception ex) { actor.OutputHandler.Send($"Treatment records are unavailable: {ex.Message}".ColourError()); }
	}

	private static void EnvironmentYields(ICharacter actor, StringStack command, IEnvironmentalMagicService service)
	{
		if (command.PeekSpeech().EqualTo("repair"))
		{
			command.PopSpeech();
			EnvironmentYieldsRepair(actor, command, service);
			return;
		}
		if (!TryEnvironmentCell(actor, command, true, out var cell) || !EnvironmentArgumentsFinished(actor, command))
		{
			return;
		}

		var sources = service.InspectOrganicSources(cell);
		var sb = new StringBuilder();
		sb.AppendLine($"Native Organic Yields — Cell #{cell.Id.ToString("N0", actor)}"
			.GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
		if (sources.Count == 0)
		{
			sb.AppendLine("The effective environmental profile authorises no native organic sources.");
			actor.OutputHandler.Send(sb.ToString());
			return;
		}

		sb.AppendLine(StringUtilities.GetTextTable(sources.Select(source => new[]
		{
			source.Selector,
			source.Status.DescribeEnum(),
			source.NativeStock.ToString("G8", actor),
			source.PrepaidFraction.ToString("G8", actor),
			$"{source.RecoveryRemainders.Health.ToString("G6", actor)}/{source.RecoveryRemainders.Yield.ToString("G6", actor)}/{source.RecoveryRemainders.Biomass.ToString("G6", actor)}",
			OrganicLifecycleText(source, actor),
			OrganicFactorText(service, cell, source, actor),
			source.Diagnostic ?? string.Empty
		}), new[] { "Selector", "Status", "Stock", "Prepaid", "Progress H/Y/B", "Lifecycle", "Factors @ +1", "Diagnostic" },
			actor, Telnet.Green));
		sb.AppendLine();
		sb.AppendLine("Stock and progress are pure observations. Factors are dimensionless [0..1]; invalid configured factors fail closed. Inspection does not synchronise, consume, save or grant mana.");
		actor.OutputHandler.Send(sb.ToString());
	}

	private static void EnvironmentYieldsRepair(ICharacter actor, StringStack command,
		IEnvironmentalMagicService service)
	{
		ICell? cell;
		var first = command.PopSpeech();
		string kindText;
		if (first.Length == 0)
		{
			cell = actor.Location;
			kindText = "all";
		}
		else if (first.EqualTo("here"))
		{
			cell = actor.Location;
			kindText = command.PopSpeech();
		}
		else if (long.TryParse(first, out var id))
		{
			cell = actor.Gameworld.Cells.Get(id);
			kindText = command.PopSpeech();
		}
		else
		{
			cell = actor.Location;
			kindText = first;
		}
		if (cell is null)
		{
			actor.OutputHandler.Send("Specify here or the ID of an existing cell.".ColourError());
			return;
		}
		if (!command.IsFinished)
		{
			actor.OutputHandler.Send("Use magic environment yields repair [here|cell id] [crop|woodland|pasture|all].".ColourError());
			return;
		}
		NativeOrganicSourceKind? kind = null;
		if (!string.IsNullOrEmpty(kindText) && !kindText.EqualTo("all") &&
		    (!Enum.TryParse(kindText, true, out NativeOrganicSourceKind parsed) ||
		     !Enum.IsDefined(parsed) || parsed == NativeOrganicSourceKind.Forage))
		{
			actor.OutputHandler.Send("Choose crop, woodland, pasture or all. Forage has no integer accounting extension.".ColourError());
			return;
		}
		else if (!string.IsNullOrEmpty(kindText) && !kindText.EqualTo("all"))
		{
			kind = Enum.Parse<NativeOrganicSourceKind>(kindText, true);
		}
		if (!service.RepairNativeOrganicAccounting(cell, kind, out var result))
		{
			actor.OutputHandler.Send(result.ColourError());
			return;
		}
		actor.OutputHandler.Send(result.ColourValue());
	}

	private static string OrganicLifecycleText(NativeOrganicSourceSnapshot source, ICharacter actor)
	{
		if (source.Lifecycle is not { } lifecycle) return "-";
		return source.Kind == NativeOrganicSourceKind.Forage
			? $"forage #{lifecycle.ForageProfileId?.ToString("N0", actor) ?? "?"} r{lifecycle.ForageProfileRevision.ToString("N0", actor)}/{lifecycle.ForageDefinitionRevision.ToString("N0", actor)}"
			: $"field #{lifecycle.FieldId?.ToString("N0", actor) ?? "?"} g{lifecycle.Generation.ToString("N0", actor)} d{lifecycle.DefinitionId.ToString("N0", actor)}";
	}

	private static string OrganicFactorText(IEnvironmentalMagicService service, ICell cell,
		NativeOrganicSourceSnapshot source, ICharacter actor)
	{
		var channels = source.Kind switch
		{
			NativeOrganicSourceKind.Forage => new[] { ("R", NativeOrganicPenaltyChannel.ForageReplenishment) },
			NativeOrganicSourceKind.Crop => new[]
			{
				("H", NativeOrganicPenaltyChannel.CropHealthRecovery),
				("Y", NativeOrganicPenaltyChannel.CropYieldRecovery),
				("I", NativeOrganicPenaltyChannel.CropInitialisation)
			},
			NativeOrganicSourceKind.Woodland => new[]
			{
				("H", NativeOrganicPenaltyChannel.WoodlandHealthRecovery),
				("Y", NativeOrganicPenaltyChannel.WoodlandYieldRecovery),
				("I", NativeOrganicPenaltyChannel.WoodlandInitialisation)
			},
			NativeOrganicSourceKind.Pasture => new[]
			{
				("R", NativeOrganicPenaltyChannel.PastureRecovery),
				("I", NativeOrganicPenaltyChannel.PastureInitialisation)
			},
			_ => Array.Empty<(string Label, NativeOrganicPenaltyChannel Channel)>()
		};
		return channels.Select(item =>
		{
			var evaluation = service is EnvironmentalMagicCoordinator coordinator
				? coordinator.InspectOrganicPenaltyFactor(cell, source, item.Item2)
				: service.EvaluateOrganicPenalty(cell, item.Item2, new NativeOrganicPenaltyContext(source.Kind,
					source.Selector, source.NativeStock, source.NativeStock, source.NativeStock,
						Math.Max(source.NativeStock, 1.0), 0.0, 1.0));
			return $"{item.Item1}:{(evaluation.IsValid ? evaluation.Factor.ToString("G6", actor) : "Invalid")}";
		}).ListToString(separator: " ", conjunction: "", twoItemJoiner: " ");
	}

	private static void EnvironmentShow(ICharacter actor, StringStack command, IEnvironmentalMagicService service)
	{
		if (!TryEnvironmentCell(actor, command, true, out var cell) || !EnvironmentArgumentsFinished(actor, command))
		{
			return;
		}

		var snapshot = service.Inspect(cell);
		var sb = new StringBuilder();
		sb.AppendLine($"Environmental Magic — Cell #{cell.Id.ToString("N0", actor)}".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine($"Cell: {cell.Name.ColourName()}");
		sb.AppendLine($"Binding: {snapshot.BindingMode.DescribeEnum().ColourName()}");
		sb.AppendLine($"Effective Profile: {(snapshot.ProfileId.HasValue ? $"{snapshot.ProfileName} (#{snapshot.ProfileId.Value.ToString("N0", actor)})".ColourName() : "None".ColourValue())}");
		sb.AppendLine($"Scar Damage: {snapshot.State.ScarDamage.ToString("G8", actor).ColourValue()}");
		sb.AppendLine($"Recent Pressure: {snapshot.Pressure.ToString("G8", actor).ColourValue()}");
		sb.AppendLine($"Last Destructive Use (UTC): {(snapshot.State.LastDefileUtc?.ToString("u", actor) ?? "Never").ColourValue()}");
		sb.AppendLine();
		sb.AppendLine("Resources".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
		if (snapshot.Outputs.Count == 0)
		{
			sb.AppendLine("There are no environmental resource outputs.");
		}
		else
		{
			sb.AppendLine(StringUtilities.GetTextTable(snapshot.Outputs.Select(x => new[]
			{
				x.ResourceId.ToString("N0", actor), x.Name, x.Balance.ToString("G8", actor),
				x.IsValid ? x.Maximum.ToString("G8", actor) : "Invalid",
				x.IsValid ? x.Rate.ToString("G8", actor) : "Invalid"
			}), new[] { "ID", "Resource", "Balance", "Maximum", "Per Minute" }, actor, Telnet.Green));
		}

		sb.AppendLine();
		sb.AppendLine("Inputs".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
		foreach (var input in snapshot.Inputs.OrderBy(x => x.Key))
		{
			sb.AppendLine($"{input.Key.ColourName()}: {input.Value.ToString("G8", actor).ColourValue()}");
		}

		var errors = snapshot.Errors.Concat(snapshot.Outputs.Where(x => !x.IsValid && x.Error != null).Select(x => x.Error!))
			.Distinct().ToList();
		if (errors.Count > 0)
		{
			sb.AppendLine();
			sb.AppendLine("Validation".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
			foreach (var error in errors)
			{
				sb.AppendLine(error.ColourError());
			}
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	private static void EnvironmentCell(ICharacter actor, StringStack command, IEnvironmentalMagicService service)
	{
		if (!TryEnvironmentCell(actor, command, false, out var cell))
		{
			return;
		}

		var binding = command.SafeRemainingArgument;
		if (binding.EqualTo("inherit"))
		{
			if (!TrySetEnvironmentBinding(actor, service, cell, EnvironmentalMagicBindingMode.Inherit, null)) return;
			actor.OutputHandler.Send($"Cell #{cell.Id.ToString("N0", actor).ColourValue()} now inherits its terrain's environmental profile.");
			return;
		}

		if (binding.EqualTo("disabled"))
		{
			if (!TrySetEnvironmentBinding(actor, service, cell, EnvironmentalMagicBindingMode.Disabled, null)) return;
			actor.OutputHandler.Send($"Environmental production is disabled for cell #{cell.Id.ToString("N0", actor).ColourValue()}; its balances and damage are retained.");
			return;
		}

		if (actor.Gameworld.MagicResourceRegenerators.GetByIdOrName(binding) is not IEnvironmentalMagicProfile profile)
		{
			actor.OutputHandler.Send("Specify inherit, disabled, or the name/ID of an environmental regenerator.".ColourError());
			return;
		}

		if (!TrySetEnvironmentBinding(actor, service, cell, EnvironmentalMagicBindingMode.Explicit, profile.Id)) return;
		actor.OutputHandler.Send($"Cell #{cell.Id.ToString("N0", actor).ColourValue()} now explicitly uses {profile.Name.ColourName()}.");
	}

	private static bool TrySetEnvironmentBinding(ICharacter actor, IEnvironmentalMagicService service, ICell cell,
		EnvironmentalMagicBindingMode mode, long? profileId)
	{
		try
		{
			service.SetBinding(cell, mode, profileId);
			return true;
		}
		catch (InvalidOperationException exception)
		{
			actor.OutputHandler.Send(exception.Message.ColourError());
			return false;
		}
	}

	private static void EnvironmentTerrain(ICharacter actor, StringStack command)
	{
		var terrainText = command.PopSpeech();
		if (actor.Gameworld.Terrains.GetByIdOrName(terrainText) is not Terrain terrain)
		{
			actor.OutputHandler.Send("Specify the name or ID of a terrain.".ColourError());
			return;
		}

		var profileText = command.SafeRemainingArgument;
		if (profileText.EqualTo("none"))
		{
			terrain.SetEnvironmentalMagicProfile(null);
			actor.OutputHandler.Send($"Terrain {terrain.Name.ColourName()} no longer supplies an environmental profile.");
			return;
		}

		if (actor.Gameworld.MagicResourceRegenerators.GetByIdOrName(profileText) is not IEnvironmentalMagicProfile profile)
		{
			actor.OutputHandler.Send("Specify none or the name/ID of an environmental regenerator.".ColourError());
			return;
		}

		terrain.SetEnvironmentalMagicProfile(profile.Id);
		actor.OutputHandler.Send($"Terrain {terrain.Name.ColourName()} now supplies {profile.Name.ColourName()} to cells that inherit their environmental profile.");
	}

	private static void EnvironmentOperation(ICharacter actor, StringStack command, IEnvironmentalMagicService service, bool repair)
	{
		if (!TryEnvironmentCell(actor, command, false, out var cell))
		{
			return;
		}

		if (!TryEnvironmentAmount(actor, command.PopSpeech(), out var amount))
		{
			return;
		}

		var pressure = 0.0;
		if (!repair && !TryEnvironmentAmount(actor, command.PopSpeech(), out pressure))
		{
			return;
		}

		if (amount == 0.0 && pressure == 0.0)
		{
			actor.OutputHandler.Send("At least one requested amount must be greater than zero.".ColourError());
			return;
		}

		if (!Guid.TryParse(command.PopSpeech(), out var operationId) || operationId == Guid.Empty)
		{
			actor.OutputHandler.Send("Supply a non-empty operation GUID. Reuse that GUID only when retrying the same operation.".ColourError());
			return;
		}

		var reason = command.SafeRemainingArgument;
		if (string.IsNullOrWhiteSpace(reason))
		{
			actor.OutputHandler.Send("Supply an audit reason for this staff operation.".ColourError());
			return;
		}

		var result = service.ApplyOperation(cell, new EnvironmentalMagicOperationRequest(operationId, actor.Id,
			reason, Damage: repair ? 0.0 : amount, Pressure: pressure, Repair: repair ? amount : 0.0));
		if (!result.Success)
		{
			actor.OutputHandler.Send((result.Error ?? "The environmental operation could not be completed.").ColourError());
			return;
		}

		actor.OutputHandler.Send($"Operation {result.OperationId.ToString().ColourValue()}{(result.Replayed ? " (recorded result)" : string.Empty)}: damage {result.AppliedDamage.ToString("G8", actor).ColourValue()}, pressure {result.AppliedPressure.ToString("G8", actor).ColourValue()}, repaired {result.AppliedRepair.ToString("G8", actor).ColourValue()}.");
	}

	private static bool TryEnvironmentCell(ICharacter actor, StringStack command, bool defaultHere,
		[NotNullWhen(true)] out ICell? cell)
	{
		var reference = command.PopSpeech();
		cell = reference.EqualTo("here") || defaultHere && reference.Length == 0
			? actor.Location
			: long.TryParse(reference, out var id) ? actor.Gameworld.Cells.Get(id) : null;
		if (cell != null)
		{
			return true;
		}

		actor.OutputHandler.Send("Specify here or the ID of an existing cell.".ColourError());
		return false;
	}

	private static bool TryEnvironmentAmount(ICharacter actor, string text, out double amount)
	{
		if (double.TryParse(text, NumberStyles.Float | NumberStyles.AllowThousands, actor, out amount) &&
		    double.IsFinite(amount) && amount >= 0.0)
		{
			return true;
		}

		actor.OutputHandler.Send("Amounts must be finite, non-negative numbers.".ColourError());
		return false;
	}

	private static bool EnvironmentArgumentsFinished(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			return true;
		}

		actor.OutputHandler.Send("There are unexpected arguments. See magic environment help.".ColourError());
		return false;
	}
}

using System.Globalization;
using MudSharp.Magic;
using MudSharp.Magic.Gathering;

#nullable enable
namespace MudSharp.Commands.Modules;

public partial class MagicModule
{
	public const string GatheringHelp = @"Use <schoolverb> gather followed by:
	<capability> methods
	<capability> preview <method> <amount>
	<capability> <method> <amount>
	cancel

Only explicitly configured methods on a capability you currently possess are available. Preview is pure; it does not create a receipt, change a room, or advance environmental production.";

	private static void GatheringPlayer(ICharacter actor, IMagicSchool school, StringStack command)
	{
		try
		{
			IMagicGatheringService? service = actor.Gameworld.MagicGathering;
			if (service is null)
			{
				actor.OutputHandler.Send("The gathering service is unavailable in this gameworld.");
				return;
			}
			if (command.IsFinished || command.PeekSpeech().EqualToAny("help", "?"))
			{
				actor.OutputHandler.Send(GatheringHelp);
				return;
			}

			string capabilityText = command.PopSpeech();
			if (capabilityText.EqualTo("cancel"))
			{
				actor.OutputHandler.Send(service.Cancel(actor).Message);
				return;
			}

			IMagicGatheringCapability? capability = GatheringCapability(actor, school, capabilityText);
			if (capability is null)
			{
				actor.OutputHandler.Send("You do not have a gathering-capable magic capability of this school by that name.");
				return;
			}

			string verb = command.PopForSwitch();
			if (verb is "" or "methods")
			{
				IReadOnlyList<MagicGatheringMethodView> methods = service.Methods(actor, capability);
				if (methods.Count == 0)
				{
					actor.OutputHandler.Send($"{capability.Name.ColourName()} has no configured gathering methods.");
					return;
				}
				actor.OutputHandler.Send(StringUtilities.GetTextTable(methods.Select(x => new[]
				{
					x.Alias,
					x.Name,
					x.Kind.DescribeEnum(),
					$"{x.MinimumAmount.ToString("N2", actor)}-{x.MaximumAmount.ToString("N2", actor)}",
					x.UnavailableReason is null ? "Available" : "Unavailable"
				}), ["Alias", "Method", "Kind", "Amount", "Status"], actor.LineFormatLength,
					unicodeTable: actor.Account.UseUnicode));
				return;
			}

			bool preview = verb == "preview";
			string method = preview ? command.PopSpeech() : verb;
			if (!double.TryParse(command.PopSpeech(), NumberStyles.Float, CultureInfo.InvariantCulture, out double amount) || !double.IsFinite(amount))
			{
				actor.OutputHandler.Send("Specify a finite positive gathering amount.");
				return;
			}

			MagicGatheringResult result = preview
				? service.Preview(actor, capability, method, amount)
				: service.Begin(actor, capability, method, amount);
			actor.OutputHandler.Send(result.Message);
		}
		catch (Exception ex)
		{
			actor.OutputHandler.Send(ex.Message);
		}
	}

	private static IMagicGatheringCapability? GatheringCapability(ICharacter actor, IMagicSchool school, string text)
	{
		IReadOnlyList<IMagicGatheringCapability> capabilities = actor.Capabilities.OfType<IMagicGatheringCapability>()
			.Where(x => x.School.Id == school.Id).DistinctBy(x => x.Id).ToArray();
		if (long.TryParse(text, out long id))
		{
			return capabilities.FirstOrDefault(x => x.Id == id);
		}

		IMagicGatheringCapability[] matches = capabilities.Where(x => x.Name.EqualTo(text) ||
			x.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase)).ToArray();
		return matches.Length == 1 ? matches[0] : null;
	}

	private const string GatheringAdminHelp = "magic gathering show <operation-id>\nmagic gathering acknowledge <operation-id>\nmagic gathering unresolved [<character-id>]";

	private static void GatheringAdmin(ICharacter actor, StringStack command)
	{
		try
		{
			IMagicGatheringService? service = actor.Gameworld.MagicGathering;
			if (service is null)
			{
				actor.OutputHandler.Send("The gathering service is unavailable in this gameworld.");
				return;
			}
			string verb = command.PopForSwitch();
			if (verb is "" or "help" or "?")
			{
				actor.OutputHandler.Send(GatheringAdminHelp);
				return;
			}
			if (verb == "unresolved")
			{
				long? owner = long.TryParse(command.PopSpeech(), out long value) && value > 0 ? value : null;
				IReadOnlyList<MagicGatheringOperationSummary> operations = service.UnresolvedOperations(owner);
				actor.OutputHandler.Send(operations.Count == 0 ? "No unresolved gathering receipts." : StringUtilities.GetTextTable(operations.Select(x => new[]
				{
					x.Id.ToString(), x.OwnerId.ToString("N0", actor), x.Status, x.Kind, x.RequestedAmount.ToString("N2", actor), x.Diagnostic
				}), ["Receipt", "Owner", "Status", "Kind", "Amount", "Diagnostic"], actor.LineFormatLength,
					unicodeTable: actor.Account.UseUnicode));
				return;
			}
			if (!Guid.TryParse(command.PopSpeech(), out Guid operationId))
			{
				actor.OutputHandler.Send("Specify a valid gathering receipt GUID.");
				return;
			}
			if (verb == "acknowledge")
			{
				actor.OutputHandler.Send(service.Acknowledge(operationId).Message);
				return;
			}
			if (verb == "show" && service.Operation(operationId) is { } receipt)
			{
				IReadOnlyDictionary<string, double>? landDetails = service.LandDetails(operationId);
				string landText = landDetails is null ? string.Empty : "\nLand accounting:\n" +
					string.Join("\n", landDetails.OrderBy(x => x.Key, StringComparer.Ordinal)
						.Select(x => $"  {x.Key}: {x.Value.ToString("N4", actor)}"));
				actor.OutputHandler.Send($"Gathering Receipt {receipt.Id}".GetLineWithTitle(actor, Telnet.Magenta, Telnet.BoldWhite) + "\n\n" +
					$"Owner #{receipt.OwnerId.ToString("N0", actor)}, actor #{receipt.ActorId.ToString("N0", actor)}, body #{receipt.BodyId.ToString("N0", actor)}\n" +
					$"Capability #{receipt.CapabilityId.ToString("N0", actor)}, method {receipt.MethodKey}, version {receipt.MethodVersion.ToString("N0", actor)}\n" +
					$"Status: {receipt.Status}\nRequested: {receipt.RequestedAmount.ToString("N2", actor)}, source debit: {receipt.SourceDebit.ToString("N2", actor)}\n" +
					$"Body costs: stamina {receipt.StaminaCost.ToString("N2", actor)}, damage {receipt.DamageCost.ToString("N2", actor)}, pain {receipt.PainCost.ToString("N2", actor)}, stun {receipt.StunCost.ToString("N2", actor)}\n" +
					$"Flags: source {receipt.SourceDebited}, body {receipt.BodilyCostApplied}, destination {receipt.DestinationCredited}, accounting {receipt.AccountingPersisted}, notification {receipt.NotificationCompleted}\n" +
					$"Diagnostic: {receipt.Diagnostic}{landText}");
				return;
			}
			actor.OutputHandler.Send(GatheringAdminHelp);
		}
		catch (Exception ex)
		{
			actor.OutputHandler.Send(ex.Message);
		}
	}
}

#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.Health;
using MudSharp.Magic.Environment;

namespace MudSharp.Magic.Gathering;

public sealed partial class MagicGatheringService
{
	/// <summary>One complete regional payment first; a shortage permits the ordinary configured local route.</summary>
	private MagicGatheringResult? QuoteRegionalLand(ICharacter actor, ICharacter owner, IMagicGatheringCapability capability,
		MagicGatheringMethodDefinition method, double amount, IRoom room, IMagicResource destination, double duration,
		double stamina, double damage, double pain, double stun, DirectHealthCostPlan? healthPlan, RegionalGatheringDebit? captured)
	{
		var environmental = _gameworld.EnvironmentalMagic!;
		var regional = environmental.RegionalGathering;
		if (regional is null) return captured is null ? null : Refused("The captured regional life service is unavailable.");
		if (regional.UnavailableReason(room) is { } reason) return Refused(reason);
		if (!regional.TryPlan(room, method.RegionalLifeUnitsPerAction, out var plan))
			return captured is null ? null : Refused("The captured regional life pool can no longer fund the complete action.");
		if (_store.HasUnresolvedForParticipant(room.Id, plan.Source.Owner.ParticipantKey))
			return Refused("The shared regional life pool has an unresolved gathering payment.");
		var local = environmental.InspectOrganicProfile(room);
		if (local.HasPendingOperation) return Refused("This room has an unresolved ecological operation.");
		if (local.ProfileId is { } localId)
		{
			if (local.Errors.Count > 0 || _gameworld.MagicResourceRegenerators.Get(localId) is not IEnvironmentalMagicProfile profile || profile.Revision != local.Revision)
				return Refused("The configured local environmental profile is invalid; regional gathering cannot bypass it.");
			if (profile.OrganicProtectionProgId is { } protectionId &&
				(profile.OrganicProtectionProg is not { } protection || protection.Id != protectionId ||
				!MagicGatheringPolicy.ValidSignature(protection, "permission") || protection.StaticType != FutureProgStaticType.NotStatic ||
				!MagicGatheringPolicy.Permits(protection, actor, owner, capability, method.Key.ToString(), amount, room)))
				return Refused("The room's protection policy does not permit regional gathering.");
		}
		var key = plan.Source.Owner.ParticipantKey;
		var values = new Dictionary<string, IProgVariable>(StringComparer.OrdinalIgnoreCase)
		{
			[$"funding:{key}"] = new NumberVariable(plan.Units), [$"total:{key}"] = new NumberVariable(plan.Units)
		};
		var dictionary = new DictionaryVariable(values, ProgVariableTypes.Number);
		var scar = method.LandDamageProgId == 0 ? amount * method.LandDamagePerDestinationUnit :
			EvaluateLandEcologicalPrice(method.LandDamageProgId, "Land scar", actor, owner, capability, method, amount, room, dictionary);
		var pressure = method.LandPressureProgId == 0 ? amount * (method.LandPressurePerDestinationUnit ?? method.LandDamagePerDestinationUnit) :
			EvaluateLandEcologicalPrice(method.LandPressureProgId, "Land pressure", actor, owner, capability, method, amount, room, dictionary);
		var state = environmental.InspectState(room);
		if (!double.IsFinite(scar) || scar <= 0 || !double.IsFinite(pressure) || pressure < 0 ||
			!double.IsFinite(state.State.ScarDamage + scar) || state.State.ScarDamage + scar <= state.State.ScarDamage ||
			!double.IsFinite(state.Pressure) || state.Pressure < 0)
			return Refused("The quantified regional Land damage cannot be applied to this room's ecological state.");
		MagicGatheringQuote quote = new(method.Key, method.StructuralVersion, MagicGatheringMethodKind.Land,
			destination.Id, null, room.Id, plan.Source.ProfileId, plan.Source.ProfileRevision, amount, 0, duration,
			stamina, method.MinimumStamina, damage, pain, stun, healthPlan?.Bodypart.Id, healthPlan?.ExistingWound is not null)
		{
			RegionalLifeDebit = plan, LandDamage = scar, LandPressure = pressure
		};
		return new(true, $"Gather {amount:N2} personal units from {plan.Units:N2} shared regional life; " +
			$"local scar {scar:N2}, pressure {pressure:N2}; body price stamina {stamina:N2}, damage {damage:N2}, pain {pain:N2}, stun {stun:N2}; duration {duration:N2} seconds.", null, quote);
	}
}

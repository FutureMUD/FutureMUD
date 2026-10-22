#nullable enable
using MudSharp.Form.Audio;
namespace MudSharp.Magic.Casting;

public sealed partial class MagicCastingService
{
	private MagicCastingIntent PatronDelivery(MagicCastingIntent intent) =>
		intent.Mode == MagicCastingMode.Manifest && _world.MagicCapabilities.Get(intent.CapabilityId) is IMagicCastingCapability { CastingPolicy.Patron.RequireShout: true }
			? intent with { Method = "Shout", Speech = intent.Speech is { Method: "Shout", Volume: AudioVolume.ExtremelyLoud } ? intent.Speech : null } : intent;
	private static ControlledSpellDelivery? RouteDelivery(IMagicCastingCapability capability, MagicCastingMode mode,
		IEnumerable<ControlledSpellDelivery>? deliveries, string method)
	{
		if (deliveries is null) return null;
		if (deliveries.SingleOrDefault(x => x.Method.EqualTo(method)) is { } exact) return exact;
		return mode == MagicCastingMode.Manifest && method.EqualTo("Shout") && capability.CastingPolicy?.Patron?.RequireShout == true &&
			deliveries.SingleOrDefault(x => x.Method.EqualTo("Say")) is { } ordinary
			? ordinary with { Method = "Shout", Volume = AudioVolume.ExtremelyLoud } : null;
	}
	private AcquiredSpell? RouteAcquisition(ICharacter actor, IMagicCastingCapability capability, long spellId, int profileVersion) =>
		capability.CastingPolicy?.Patron is { } p
			? new(Owner(actor).Id, spellId, p.FixedGrade, profileVersion, DateTime.UnixEpoch, "patron route", DateTime.MaxValue, 0)
			: Acquisition(actor, spellId);
}

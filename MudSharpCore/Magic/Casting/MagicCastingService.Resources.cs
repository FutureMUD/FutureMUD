using MudSharp.RPG.Merits.Interfaces;

#nullable enable
namespace MudSharp.Magic.Casting;

public sealed partial class MagicCastingService
{
	private readonly Dictionary<long, HashSet<long>> _permanentEntitlements = [];
	public static bool IsConfiguredReserve(IFuturemud world, long resourceId) => world.MagicCapabilities
		.OfType<IMagicCastingCapability>().Any(x => x.CastingPolicy?.ReserveResourceId == resourceId);
	public static ICharacter ReserveHolder(ICharacter actor, long resourceId) =>
		IsConfiguredReserve(actor.Gameworld, resourceId) ? Owner(actor) : actor;
	private static bool ReserveConflict(ICharacter actor, long reserveId) => actor.Capabilities.OfType<IMagicCastingCapability>()
		.Where(x => x.CastingPolicy is { Enabled: true } p && p.ReserveResourceId == reserveId)
		.Select(x => x.CastingPolicy!.PassiveEntitlement).Distinct().Count() > 1;

	public void Reconcile(ICharacter actor)
	{
		var owner = Owner(actor);
		if (owner is MudSharp.Character.Character runtime) runtime.ReconcileCastingResources();
		var current = ProgressBodies(actor).SelectMany(physical => physical.Merits.OfType<IMagicCapabilityMerit>().Where(x => x.Applies(physical))
			.SelectMany(x => x.Capabilities)).OfType<IMagicCastingCapability>().Where(x => x.CastingPolicy is { Enabled: true })
			.Select(x => x.Id).ToHashSet();
		var previous = _permanentEntitlements.GetValueOrDefault(owner.Id) ?? [];
		_permanentEntitlements[owner.Id] = current;
		foreach (var id in current)
		{
			var c = (IMagicCastingCapability)_world.MagicCapabilities.Get(id)!;
			if (c.CastingConfigurationErrors().Count > 0) continue;
			lock (Guard(actor))
			{
				foreach (var admission in c.CastingPolicy!.Admissions.Where(x => x.RawSkillCap.HasValue && Acquisition(actor, x.SpellId) is not null))
					RecordSkillCap(actor, c, admission);
				foreach (var support in c.CastingPolicy.Supports.Where(x => x.RawSkillCap.HasValue))
					RecordSupportSkillCap(actor, c, support);
			}
		}
		foreach (var id in current.Where(id => !previous.Contains(id) || ((IMagicCastingCapability)_world.MagicCapabilities.Get(id)!).CastingPolicy!.Supports.Count > 0))
		{
			var c = (IMagicCastingCapability)_world.MagicCapabilities.Get(id)!;
			if (_store.Enrolment(owner.Id, c.CastingPolicy!.Identity) is null) continue;
			lock (Guard(actor)) EvaluateEdges(actor, ProgressionNodes(c));
		}
		ValidatePractices(actor);
	}

	public void NotifyCapacityChange(ICharacter actor)
	{
		lock (Guard(actor))
		{
			(Owner(actor) as MudSharp.Character.Character)?.ReconcileCastingResourceCapacities();
			ValidatePractices(actor);
		}
	}
}

#nullable enable

using System.Globalization;
using MudSharp.Body;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework.Scheduling;
using MudSharp.Health;
using MudSharp.NPC.AI;
using MudSharp.Planes;
using MudSharp.GameItems;

namespace MudSharp.Effects.Concrete.SpellEffects;

/// <summary>Parent owns dispel/removal; the persisted deadline cannot reset on cold loading.</summary>
public sealed class SpellDamagingExitBarrierEffect : SpellExitBarrierEffect, IExitBarrierItemTraversalEffect
{
	public new static void InitialiseEffectType() => RegisterFactory("SpellDamagingExitBarrier", (xml, owner) => new SpellDamagingExitBarrierEffect(xml, owner));
	public SpellDamagingExitBarrierEffect(IExit owner, IMagicSpellEffectParent parent, DamageType type, double amount,
		RoomLayer layer, long[] planes, DateTime expiry) : base(owner, parent)
	{
		DamageType = type; DamageAmount = amount; Layer = layer; Planes = planes.ToArray(); ExpiryUtc = expiry;
		RoomIds = owner.Rooms.Select(x => x.Id).Order().ToArray();
	}
	private SpellDamagingExitBarrierEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		var xml = root.Element("Effect")!;
		DamageType = (DamageType)((int?)xml.Element("DamageType") ?? -1);
		DamageAmount = (double?)xml.Element("DamageAmount") ?? 0;
		Layer = (RoomLayer)((int?)xml.Element("Layer") ?? -1);
		Planes = xml.Element("Planes")?.Elements("Plane").Select(x => (long)x).ToArray() ?? [];
		RoomIds = xml.Element("Rooms")?.Elements("Room").Select(x => (long)x).Order().ToArray() ?? [];
		ExpiryUtc = DateTime.TryParse(xml.Element("ExpiryUtc")?.Value, CultureInfo.InvariantCulture,
			DateTimeStyles.RoundtripKind, out var expiry) ? expiry.ToUniversalTime() : DateTime.MinValue;
	}
	public DamageType DamageType { get; }
	public double DamageAmount { get; }
	public RoomLayer Layer { get; }
	private long[] Planes { get; }
	private long[] RoomIds { get; }
	public DateTime ExpiryUtc { get; }
	public bool IsActive => Owner is IExit boundary && ParentEffect is not null && ReferenceEquals(ParentEffect.Owner, Owner) &&
		RuntimeClock.UtcNow < ExpiryUtc && double.IsFinite(DamageAmount) && DamageAmount is > 0 and <= 7000000 && Enum.IsDefined(DamageType) &&
		Enum.IsDefined(Layer) && Planes.Length > 0 && Planes.All(x => x > 0) && RoomIds.Length == 2 && RoomIds.Distinct().Count() == 2 &&
		RoomIds.SequenceEqual(boundary.Rooms.Select(x => x.Id).Order()) &&
		Owner.Effects.Any(x => ReferenceEquals(x, this)) && Owner.Effects.Any(x => ReferenceEquals(x, ParentEffect)) &&
		ParentEffect.SpellEffects.Any(x => ReferenceEquals(x, this));
	public bool OccupiesLayerAndPlane(ICharacter actor)
		=> OccupiesLayerAndPlane((IPerceivable)actor, actor.RoomLayer);
	public bool OccupiesLayerAndPlane(IGameItem item)
		=> OccupiesLayerAndPlane(item, item.RoomLayer);
	private bool OccupiesLayerAndPlane(IPerceivable target, RoomLayer layer)
	{
		if (!IsActive || layer != Layer || !ReferenceEquals(target.Gameworld, Gameworld)) return false;
		var presence = target.GetPlanarPresence();
		return !presence.CanCrossMagicalBarriers && !presence.SuspendsPhysicalContact &&
			presence.InteractionPlaneIds(PlanarInteractionKind.MovementBarrier).Intersect(Planes).Any();
	}
	public void Contact(IGameItem item, IRoomExit exit, Func<bool> continuation)
	{
		if (!IsActive || !ReferenceEquals(exit.Exit, Owner) || !continuation()) return;
		var parent = ParentEffect;
		var caster = parent.Caster;
		var casterBody = caster?.Body;
		var casterIdentity = caster?.Identity;
		var casterInstance = caster?.InstanceId;
		using var sourceFence = new SourceFence(caster);
		var origin = exit.Origin;
		var destination = exit.Destination;
		var world = item.Gameworld;
		bool Current() => !sourceFence.Invalid && IsActive && ReferenceEquals(ParentEffect, parent) &&
			!item.Destroyed && !item.Deleted && ReferenceEquals(item.Gameworld, world) &&
			ReferenceEquals(item.Location, origin) && item.RoomLayer == Layer &&
			ReferenceEquals(exit.Exit, Owner) && ReferenceEquals(exit.Origin, origin) && ReferenceEquals(exit.Destination, destination) &&
			(caster is null || ReferenceEquals(caster.Gameworld, world) && ReferenceEquals(caster.Body, casterBody) &&
				ReferenceEquals(casterBody?.Actor, caster) && ReferenceEquals(caster.Identity, casterIdentity) && caster.InstanceId == casterInstance);
		bool Continue() => Current() && continuation() && Current();
		if (!Continue()) return;
		item.SufferDamage(new Damage
		{
			ActorOrigin = caster!, DamageType = DamageType, DamageAmount = DamageAmount,
			PainAmount = DamageAmount, MutationGuard = new ItemContactGuard(item, caster, Continue)
		}).ToArray();
		if (!Continue()) return;
		origin.Handle(Layer, new EmoteOutput(new Emote($"The {DamageType.DescribeEnum().ToLowerInvariant()} barrier flares against @, blocking &0's passage.", item, item)));
		if (!Continue()) return;
		destination.Handle(Layer, "The magical barrier flares as a vehicle attempts passage from the other side.");
	}
	public void Contact(ICharacter actor, IRoomExit exit, Func<bool> continuation)
	{
		if (!IsActive || !ReferenceEquals(exit.Exit, Owner) || !continuation()) return;
		var parent = ParentEffect;
		var body = actor.Body; var caster = parent.Caster; var casterBody = caster?.Body;
		var casterIdentity = caster?.Identity; var casterInstance = caster?.InstanceId;
		using var sourceFence = new SourceFence(caster);
		var origin = exit.Origin; var destination = exit.Destination; var world = actor.Gameworld;
		bool Current() => !sourceFence.Invalid && IsActive && ReferenceEquals(ParentEffect, parent) && ReferenceEquals(actor.Body, body) && ReferenceEquals(body.Actor, actor) &&
			ReferenceEquals(actor.Gameworld, world) && ReferenceEquals(actor.Location, origin) && actor.RoomLayer == Layer &&
			ReferenceEquals(exit.Exit, Owner) && ReferenceEquals(exit.Origin, origin) && ReferenceEquals(exit.Destination, destination) &&
			(caster is null || ReferenceEquals(caster.Gameworld, world) && ReferenceEquals(caster.Body, casterBody) && ReferenceEquals(casterBody?.Actor, caster) &&
				ReferenceEquals(caster.Identity, casterIdentity) && caster.InstanceId == casterInstance);
		bool Continue() => Current() && continuation() && Current();
		var part = body.RandomBodypart;
		if (part is null || !Continue()) return;
		var guard = new ContactGuard(body, caster, Continue);
		CommandExecutionScope.MarkCommitted();
		body.SufferDamage(new Damage { ActorOrigin = caster!, TargetBody = body, Bodypart = part,
			DamageType = DamageType, DamageAmount = DamageAmount, PainAmount = DamageAmount, MutationGuard = guard }).ToArray();
		if (!Continue()) return;
		origin.Handle(Layer, new EmoteOutput(new Emote($"@ recoil|recoils as the {DamageType.DescribeEnum().ToLowerInvariant()} barrier blocks &0's passage.", actor, actor)));
		if (!Continue()) return;
		destination.Handle(Layer, "The magical barrier flares as someone attempts passage from the other side.");
	}
	private sealed class SourceFence : IDisposable
	{
		private readonly ICharacter? _caster;
		internal bool Invalid { get; private set; }
		internal SourceFence(ICharacter? caster)
		{
			_caster = caster;
			if (caster is null) return;
			caster.CurrentBodyChanged += Changed; caster.OnQuit += Removed; caster.OnDeleted += Removed;
		}
		private void Changed(ICharacter actor, IBody before, IBody after) => Invalid = true;
		private void Removed(IPerceivable actor) => Invalid = true;
		public void Dispose()
		{
			if (_caster is null) return;
			_caster.CurrentBodyChanged -= Changed; _caster.OnQuit -= Removed; _caster.OnDeleted -= Removed;
		}
	}
	private sealed class ContactGuard(IBody body, ICharacter? caster, Func<bool> continuation) : IDamageMutationGuard
	{
		private readonly IGameItem[] _items = body.WornItems.Concat(body.Prosthetics.Select(x => x.Parent)).ToArray();
		public void Validate(IHaveWounds recipient, IDamage damage)
		{
			bool RecipientIsCurrent() => ReferenceEquals(recipient, body) || recipient is IGameItem item &&
				_items.Any(x => ReferenceEquals(x, item)) && !item.Destroyed && !item.Deleted && ReferenceEquals(item.Gameworld, body.Gameworld);
			if (!RecipientIsCurrent() || !ReferenceEquals(damage.MutationGuard, this) ||
				!ReferenceEquals(damage.ActorOrigin, caster) || !continuation())
				throw new InvalidOperationException("The captured barrier contact changed before native injury.");
			if (!RecipientIsCurrent()) throw new InvalidOperationException("The captured contact item changed during continuation.");
		}
	}
	private sealed class ItemContactGuard(IGameItem item, ICharacter? caster, Func<bool> continuation) : IDamageMutationGuard
	{
		public void Validate(IHaveWounds recipient, IDamage damage)
		{
			if (!ReferenceEquals(recipient, item) || !ReferenceEquals(damage.MutationGuard, this) ||
				!ReferenceEquals(damage.ActorOrigin, caster) || !continuation())
				throw new InvalidOperationException("The vehicle barrier contact changed before native injury.");
		}
	}
	protected override string SpecificEffectType => "SpellDamagingExitBarrier";
	protected override XElement SaveDefinition() => new("Effect", new XElement("DamageType", (int)DamageType),
		new XElement("DamageAmount", DamageAmount), new XElement("Layer", (int)Layer),
		new XElement("Planes", Planes.Select(x => new XElement("Plane", x))), new XElement("Rooms", RoomIds.Select(x => new XElement("Room", x))),
		new XElement("ExpiryUtc", ExpiryUtc.ToString("O", CultureInfo.InvariantCulture)));
	public override string Describe(IPerceiver voyeur) => $"Damaging exit barrier: {DamageType.DescribeEnum().ColourName()}, {DamageAmount.ToString("N2", voyeur).ColourValue()} native contact damage.";
}

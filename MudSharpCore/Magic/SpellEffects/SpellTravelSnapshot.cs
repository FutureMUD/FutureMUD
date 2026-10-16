#nullable enable

using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Planes;

namespace MudSharp.Magic.SpellEffects;

/// <summary>Callback-free physical and admission inputs for a prepared single-traveller route.</summary>
internal sealed class SpellTravelSnapshot
{
	internal sealed record Traveller(ICharacter Actor, IBody Body, ICharacterIdentity Identity, long Id, long Instance,
		long BodyId, CharacterState State, IPositionState Position, IRoom Origin, RoomLayer Layer)
	{
		public bool RequireSolo { get; init; } = true;
		public bool Current => Actor.Id == Id && Actor.InstanceId == Instance && Body.Id == BodyId && Actor.State == State &&
			ReferenceEquals(Actor.Body, Body) && ReferenceEquals(Body.Actor, Actor) && ReferenceEquals(Actor.Identity, Identity) &&
			Identity.Instances.Any(x => ReferenceEquals(x, Actor)) && Actor.Gameworld.Actors.Any(x => ReferenceEquals(x, Actor)) &&
			ReferenceEquals(Actor.PositionState, Position) && !Actor.State.IsDead() && !Actor.State.IsInStatis() &&
			(!RequireSolo || Actor.RidingMount is null && !Actor.Riders.Any());
		public bool At(IRoom room, RoomLayer layer) => Current && ReferenceEquals(Actor.Location, room) && Actor.RoomLayer == layer &&
			room.Characters.Count(x => ReferenceEquals(x, Actor)) == 1 &&
			room.OwningZone.Characters.Count(x => ReferenceEquals(x, Actor)) == 1 &&
			room.OwningZone.Shard.Characters.Count(x => ReferenceEquals(x, Actor)) == 1 &&
			(ReferenceEquals(room, Origin) || !Origin.Characters.Any(x => ReferenceEquals(x, Actor)));
	}
	private readonly List<Func<bool>> _checks = [];
	public IReadOnlyList<Traveller> Travellers { get; }
	public bool Current => Travellers.All(x => x.Current) && _checks.All(x => x());
	public bool AtCapturedPositions => Current && Travellers.All(x => x.At(x.Origin, x.Layer));
	public SpellTravelSnapshot(IFuturemud world, IEnumerable<ICharacter> actors, IEnumerable<IRoom> rooms, bool requireSolo = true)
	{
		void Value<T>(Func<T> read) { var before = read(); _checks.Add(() => EqualityComparer<T>.Default.Equals(before, read())); }
		void Reference<T>(Func<T> read) where T : class? { var before = read(); _checks.Add(() => ReferenceEquals(before, read())); }
		void Sequence<T>(Func<IEnumerable<T>> read) where T : class
		{ var before = read().ToArray(); _checks.Add(() => before.SequenceEqual(read(), ReferenceEqualityComparer.Instance)); }
		var seenTags = new HashSet<ITag>(ReferenceEqualityComparer.Instance);
		void Tag(ITag tag)
		{
			if (!seenTags.Add(tag)) return;
			Value(() => tag.Id); Reference(() => tag.Parent); Reference(() => world.Tags.Get(tag.Id));
			if (tag.Parent is { } parent) Tag(parent);
		}
		void Tags(IHaveTags owner) { Sequence(() => owner.Tags); foreach (var tag in owner.Tags) Tag(tag); }
		void Planar(IPerceivable owner)
		{
			if (owner is IHavePlanarPresence physical) Value(() => physical.BasePlanarPresence?.SaveToXml().ToString(SaveOptions.DisableFormatting));
			Sequence(() => owner.Effects.OfType<IPlanarOverlayEffect>());
			foreach (var effect in owner.Effects.OfType<IPlanarOverlayEffect>())
			{
				Value(() => effect.PlanarPresenceDefinition.SaveToXml().ToString(SaveOptions.DisableFormatting));
				Value(() => effect.PlanarPriority); Value(() => effect.OverridesBasePlanarPresence);
			}
			Sequence(() => owner.Effects.OfType<IMagicInterdictionEffect>());
			foreach (var ward in owner.Effects.OfType<IMagicInterdictionEffect>())
			{
				Reference(() => ward.School); Value(() => ward.Mode); Value(() => ward.Coverage); Value(() => ward.IncludesSubschools);
				Reference(() => ward.ApplicabilityProg); Value(() => ward.ApplicabilityProg?.FunctionText);
				Value(() => ward.ApplicabilityProg?.CompileError);
			}
		}
		var seenTerrains = new HashSet<ITerrain>(ReferenceEqualityComparer.Instance);
		void Terrain(ITerrain terrain)
		{ if (seenTerrains.Add(terrain)) { Value(() => terrain.Id); Tags(terrain); Value(() => string.Join(",", terrain.TerrainLayers)); } }
		var seenOverlays = new HashSet<IRoomOverlay>(ReferenceEqualityComparer.Instance);
		void Overlay(IRoomOverlay overlay)
		{
			if (!seenOverlays.Add(overlay)) return;
			Value(() => overlay.Id); Reference(() => overlay.Terrain); Reference(() => overlay.Package);
			Value(() => overlay.Package.Status); Value(() => overlay.OutdoorsType); Terrain(overlay.Terrain);
		}
		Reference(() => world.DefaultPlane);
		var travellers = new List<Traveller>();
		foreach (var actor in actors.Distinct<ICharacter>(ReferenceEqualityComparer.Instance))
		{
			if (actor.Identity is not { } identity || actor.Body is not { Id: > 0 } body || actor.Id <= 0 || actor.InstanceId <= 0)
				throw new InvalidOperationException("Travel requires a saved exact physical identity and body.");
			travellers.Add(new(actor, body, identity, actor.Id, actor.InstanceId, body.Id, actor.State, actor.PositionState, actor.Location, actor.RoomLayer) { RequireSolo = requireSolo });
			Reference(() => actor.Gameworld); Reference(() => actor.CurrentOverlayPackage); Value(() => actor.PermissionLevel);
			Planar(actor); Planar(body); Sequence(() => actor.Merits.OfType<IPlanarOverlayMerit>());
			foreach (var merit in actor.Merits.OfType<IPlanarOverlayMerit>())
			{
				Value(() => merit.PlanarPresenceDefinition.SaveToXml().ToString(SaveOptions.DisableFormatting));
				Value(() => merit.PlanarPriority); Value(() => merit.OverridesBasePlanarPresence);
			}
		}
		Travellers = travellers.AsReadOnly();
		foreach (var room in rooms.Distinct<IRoom>(ReferenceEqualityComparer.Instance))
		{
			Value(() => room.Id); Reference(() => world.Rooms.Get(room.Id)); Reference(() => room.Gameworld);
			Reference(() => room.CurrentOverlay); Reference(() => room.OwningZone); Reference(() => room.OwningZone.Shard);
			Value(() => room.Temporary); Reference(() => room.RouteDefinition); Tags(room); Planar(room); Overlay(room.CurrentOverlay);
			Sequence(() => room.Overlays); foreach (var overlay in room.Overlays) Overlay(overlay);
			Sequence(() => room.Effects.OfType<IOverrideTerrain>());
			foreach (var effect in room.Effects.OfType<IOverrideTerrain>()) { Reference(() => effect.Terrain); Terrain(effect.Terrain); }
		}
	}
}

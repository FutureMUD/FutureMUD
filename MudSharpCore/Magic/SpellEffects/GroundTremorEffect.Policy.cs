#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Planes;

namespace MudSharp.Magic.SpellEffects;

public sealed partial class GroundTremorEffect
{
	// Raw-token checks must never rerun ward, applicability, visibility or eligibility callbacks.
	private Func<bool> CaptureRawPolicy(IEnumerable<IRoom> rooms, IEnumerable<ICharacter> actors)
	{
		var checks = new List<Func<bool>>();
		void Value<T>(Func<T> read) { var before = read(); checks.Add(() => EqualityComparer<T>.Default.Equals(before, read())); }
		void Sequence<T>(Func<IEnumerable<T>> read) where T : class
		{ var before = read().ToArray(); checks.Add(() => before.SequenceEqual(read(), ReferenceEqualityComparer.Instance)); }
		var tags = new HashSet<ITag>(ReferenceEqualityComparer.Instance);
		void Tag(ITag tag)
		{
			if (!tags.Add(tag)) return;
			var parent = tag.Parent; checks.Add(() => ReferenceEquals(tag.Parent, parent));
			if (parent is not null) Tag(parent);
		}
		void Tags(IHaveTags owner)
		{ Sequence(() => owner.Tags); foreach (var tag in owner.Tags) Tag(tag); }
		var terrains = new HashSet<ITerrain>(ReferenceEqualityComparer.Instance);
		void Terrain(ITerrain terrain)
		{
			if (!terrains.Add(terrain)) return;
			Tags(terrain); Value(() => string.Join(",", terrain.TerrainLayers));
		}
		void Planar(IPerceivable owner)
		{
			if (owner is IHavePlanarPresence physical) Value(() => physical.BasePlanarPresence?.SaveToXml().ToString(SaveOptions.DisableFormatting));
			Sequence(() => owner.Effects);
			foreach (var overlay in owner.Effects.OfType<IPlanarOverlayEffect>())
			{
				Value(() => overlay.PlanarPresenceDefinition.SaveToXml().ToString(SaveOptions.DisableFormatting));
				Value(() => overlay.PlanarPriority); Value(() => overlay.OverridesBasePlanarPresence);
			}
			foreach (var ward in owner.Effects.OfType<IMagicInterdictionEffect>())
			{
				Value(() => ward.Mode); Value(() => ward.Coverage); Value(() => ward.IncludesSubschools);
				var school = ward.School; checks.Add(() => ReferenceEquals(ward.School, school));
			}
		}
		var defaultPlane = Gameworld.DefaultPlane; checks.Add(() => ReferenceEquals(Gameworld.DefaultPlane, defaultPlane));
		foreach (var room in rooms.Distinct<IRoom>(ReferenceEqualityComparer.Instance))
		{
			Planar(room); Tags(room); Terrain(room.CurrentOverlay.Terrain);
			Sequence(() => room.Overlays);
			foreach (var overlay in room.Overlays)
			{
				var terrain = overlay.Terrain; var package = overlay.Package;
				checks.Add(() => ReferenceEquals(overlay.Terrain, terrain) && ReferenceEquals(overlay.Package, package));
				Value(() => package.Status); Terrain(terrain);
			}
			foreach (var effect in room.Effects.OfType<IOverrideTerrain>())
			{
				var terrain = effect.Terrain; checks.Add(() => ReferenceEquals(effect.Terrain, terrain)); Terrain(terrain);
			}
			var route = room.RouteDefinition; checks.Add(() => ReferenceEquals(room.RouteDefinition, route));
		}
		foreach (var actor in actors.Distinct<ICharacter>(ReferenceEqualityComparer.Instance))
		{
			Value(() => actor.PermissionLevel); Planar(actor); Planar(actor.Body);
			var package = actor.CurrentOverlayPackage; checks.Add(() => ReferenceEquals(actor.CurrentOverlayPackage, package));
			Sequence(() => actor.Merits);
			foreach (var merit in actor.Merits.OfType<IPlanarOverlayMerit>())
			{
				Value(() => merit.PlanarPresenceDefinition.SaveToXml().ToString(SaveOptions.DisableFormatting));
				Value(() => merit.PlanarPriority); Value(() => merit.OverridesBasePlanarPresence);
			}
			foreach (var parent in actor.Effects.OfType<IMagicSpellEffectParent>())
			{
				Sequence(() => parent.SpellEffects); var spell = parent.Spell;
				checks.Add(() => ReferenceEquals(parent.Spell, spell)); Value(() => parent.Spell.Id);
			}
		}
		return () => checks.All(x => x());
	}
}

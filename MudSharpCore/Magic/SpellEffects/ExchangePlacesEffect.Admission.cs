#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Planes;

namespace MudSharp.Magic.SpellEffects;

public sealed partial class ExchangePlacesEffect
{
	/// <summary>Pure reads of the typed inputs used by the live admission callbacks. No ward, visibility or applicability callbacks run here.</summary>
	private Func<bool> CapturePolicyInputs(ICharacter caster, ICharacter target, IGameItem retained,
		ITag restricted, ITag populated, ITag consumed, IEnumerable<ITag> ranks)
	{
		var checks = new List<Func<bool>>();
		void Value<T>(Func<T> read) { var prior = read(); checks.Add(() => EqualityComparer<T>.Default.Equals(prior, read())); }
		void Reference<T>(Func<T> read) where T : class? { var prior = read(); checks.Add(() => ReferenceEquals(prior, read())); }
		void Sequence<T>(Func<IEnumerable<T>> read) where T : class
		{ var prior = read().ToArray(); checks.Add(() => prior.SequenceEqual(read(), ReferenceEqualityComparer.Instance)); }
		var seenTags = new HashSet<ITag>(ReferenceEqualityComparer.Instance);
		void Tag(ITag tag)
		{
			if (!seenTags.Add(tag)) return;
			Value(() => tag.Id); Reference(() => tag.Parent); Reference(() => Gameworld.Tags.Get(tag.Id));
			if (tag.Parent is { } parent) Tag(parent);
		}
		void Tags(IHaveTags owner)
		{
			Sequence(() => owner.Tags);
			foreach (var tag in owner.Tags) Tag(tag);
		}
		void Planar(IPerceivable owner)
		{
			if (owner is IHavePlanarPresence physical)
				Value(() => physical.BasePlanarPresence?.SaveToXml().ToString(SaveOptions.DisableFormatting));
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
			}
		}
		void Character(ICharacter actor)
		{
			Value(() => actor.PermissionLevel); Reference(() => actor.CurrentOverlayPackage); Reference(() => actor.Combat);
			Planar(actor); Planar(actor.Body);
			Sequence(() => actor.Merits.OfType<IPlanarOverlayMerit>());
			foreach (var merit in actor.Merits.OfType<IPlanarOverlayMerit>())
			{
				Value(() => merit.PlanarPresenceDefinition.SaveToXml().ToString(SaveOptions.DisableFormatting));
				Value(() => merit.PlanarPriority); Value(() => merit.OverridesBasePlanarPresence);
			}
		}
		var seenOverlays = new HashSet<IRoomOverlay>(ReferenceEqualityComparer.Instance);
		void Overlay(IRoomOverlay overlay)
		{
			if (!seenOverlays.Add(overlay)) return;
			Value(() => overlay.OutdoorsType); Reference(() => overlay.Package); Value(() => overlay.Package.Status);
			Reference(() => overlay.Terrain); var terrain = overlay.Terrain;
			Value(() => string.Join(",", terrain.TerrainLayers)); Tags(terrain);
		}
		void Room(IRoom room)
		{
			Reference(() => room.OwningZone); Reference(() => room.OwningZone.Shard);
			Value(() => room.Temporary); Reference(() => room.RouteDefinition); Tags(room); Planar(room);
			Reference(() => room.CurrentOverlay); Sequence(() => room.Overlays);
			Overlay(room.CurrentOverlay);
			foreach (var overlay in room.Overlays) Overlay(overlay);
			Sequence(() => room.Effects.OfType<IOverrideTerrain>());
			foreach (var effect in room.Effects.OfType<IOverrideTerrain>())
			{
				Reference(() => effect.Terrain); Value(() => string.Join(",", effect.Terrain.TerrainLayers)); Tags(effect.Terrain);
			}
		}
		Reference(() => Gameworld.DefaultPlane);
		Character(caster); Character(target); Room(caster.Location); Room(target.Location);
		Reference(() => retained.Prototype); Tags(retained); Tags(retained.Prototype); Value(() => retained.Quantity);
		Reference(() => retained.ContainedIn); Reference(() => retained.InInventoryOf);
		Reference(() => Gameworld.Items.Get(retained.Id)); Sequence(() => retained.Components);
		if (retained.ContainedIn is { } parent)
		{
			Reference(() => parent.Prototype); Tags(parent); Tags(parent.Prototype);
			Reference(() => Gameworld.Items.Get(parent.Id)); Reference(() => parent.InInventoryOf); Reference(() => parent.ContainedIn);
			Value(() => parent.Deleted); Value(() => parent.Destroyed); Sequence(() => parent.Components);
			Reference(() => parent.GetItemType<IContainer>()); Reference(() => parent.GetItemType<IOpenable>());
			if (parent.GetItemType<IContainer>() is { } container) Sequence(() => container.Contents);
			if (parent.GetItemType<IOpenable>() is { } openable) Value(() => openable.IsOpen);
		}
		foreach (var tag in ranks.Append(restricted).Append(populated).Append(consumed)) Tag(tag);
		return () => checks.All(check => check());
	}
}

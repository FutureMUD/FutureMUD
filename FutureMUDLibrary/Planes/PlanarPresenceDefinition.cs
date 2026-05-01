using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Planes;

public class PlanarPresenceDefinition
{
	private readonly Dictionary<PlanarInteractionKind, HashSet<long>> _interactionPlanes = new();

	public PlanarPresenceDefinition(
		IEnumerable<long> presencePlanes,
		IEnumerable<long> visibleToPlanes,
		IEnumerable<long> perceivesPlanes,
		IDictionary<PlanarInteractionKind, IEnumerable<long>> interactionPlanes,
		bool suspendsPhysicalContact,
		bool propagatesInventory,
		bool canCrossClosedDoors,
		bool canCrossMagicalBarriers,
		bool visibleToEtherealPerception,
		bool playerCanManifest,
		bool playerCanDissipate,
		string transitionProfile)
	{
		PresencePlaneIds = presencePlanes.Where(x => x > 0).ToHashSet();
		VisibleToPlaneIds = visibleToPlanes.Where(x => x > 0).ToHashSet();
		PerceivesPlaneIds = perceivesPlanes.Where(x => x > 0).ToHashSet();
		foreach (PlanarInteractionKind kind in Enum.GetValues<PlanarInteractionKind>())
		{
			_interactionPlanes[kind] = interactionPlanes.TryGetValue(kind, out IEnumerable<long> planes)
				? planes.Where(x => x > 0).ToHashSet()
				: new HashSet<long>();
		}

		SuspendsPhysicalContact = suspendsPhysicalContact;
		PropagatesInventory = propagatesInventory;
		CanCrossClosedDoors = canCrossClosedDoors;
		CanCrossMagicalBarriers = canCrossMagicalBarriers;
		VisibleToEtherealPerception = visibleToEtherealPerception;
		PlayerCanManifest = playerCanManifest;
		PlayerCanDissipate = playerCanDissipate;
		TransitionProfile = transitionProfile ?? string.Empty;
	}

	public IReadOnlyCollection<long> PresencePlaneIds { get; }
	public IReadOnlyCollection<long> VisibleToPlaneIds { get; }
	public IReadOnlyCollection<long> PerceivesPlaneIds { get; }
	public bool SuspendsPhysicalContact { get; }
	public bool PropagatesInventory { get; }
	public bool CanCrossClosedDoors { get; }
	public bool CanCrossMagicalBarriers { get; }
	public bool VisibleToEtherealPerception { get; }
	public bool PlayerCanManifest { get; }
	public bool PlayerCanDissipate { get; }
	public string TransitionProfile { get; }

	public IReadOnlyCollection<long> InteractionPlaneIds(PlanarInteractionKind kind)
	{
		return _interactionPlanes.TryGetValue(kind, out HashSet<long> planes) ? planes : Array.Empty<long>();
	}

	public static PlanarPresenceDefinition DefaultMaterial(IFuturemud gameworld)
	{
		return DefaultMaterial(gameworld.DefaultPlane?.Id ?? 1);
	}

	public static PlanarPresenceDefinition DefaultMaterial(long planeId)
	{
		return DefaultMaterial(new[] { planeId });
	}

	public static PlanarPresenceDefinition DefaultMaterial(IEnumerable<long> planeIds)
	{
		var planes = planeIds.Where(x => x > 0).Distinct().ToArray();
		return new PlanarPresenceDefinition(
			planes,
			planes,
			planes,
			Enum.GetValues<PlanarInteractionKind>().ToDictionary(x => x, _ => (IEnumerable<long>)planes),
			false,
			true,
			false,
			false,
			false,
			false,
			false,
			string.Empty);
	}

	public static PlanarPresenceDefinition Corporeal(IEnumerable<IPlane> planes)
	{
		return DefaultMaterial(planes.Select(x => x.Id));
	}

	public static PlanarPresenceDefinition PerceivesPlanes(IEnumerable<IPlane> planes)
	{
		var planeIds = planes.Select(x => x.Id).Where(x => x > 0).Distinct().ToArray();
		return new PlanarPresenceDefinition(
			Array.Empty<long>(),
			Array.Empty<long>(),
			planeIds,
			Enum.GetValues<PlanarInteractionKind>().ToDictionary(x => x, _ => Enumerable.Empty<long>()),
			false,
			true,
			false,
			false,
			false,
			false,
			false,
			string.Empty);
	}

	public static PlanarPresenceDefinition VisibleToPlanes(IEnumerable<IPlane> planes)
	{
		var planeIds = planes.Select(x => x.Id).Where(x => x > 0).Distinct().ToArray();
		return new PlanarPresenceDefinition(
			Array.Empty<long>(),
			planeIds,
			Array.Empty<long>(),
			Enum.GetValues<PlanarInteractionKind>().ToDictionary(x => x, _ => Enumerable.Empty<long>()),
			false,
			true,
			false,
			false,
			false,
			false,
			false,
			string.Empty);
	}

	public static PlanarPresenceDefinition NonCorporeal(IPlane plane, bool visibleToDefaultPlane = false)
	{
		var primaryPlane = plane.Id;
		var presence = new[] { primaryPlane };
		var visible = visibleToDefaultPlane && plane.Gameworld.DefaultPlane is not null
			? new[] { primaryPlane, plane.Gameworld.DefaultPlane.Id }
			: presence;
		var interactions = Enum.GetValues<PlanarInteractionKind>()
			.ToDictionary(
				x => x,
				x => x is PlanarInteractionKind.Observe or PlanarInteractionKind.Hear or PlanarInteractionKind.Speak or PlanarInteractionKind.Magic
					? (IEnumerable<long>)presence
					: Array.Empty<long>());

		return new PlanarPresenceDefinition(
			presence,
			visible,
			presence,
			interactions,
			true,
			false,
			true,
			false,
			true,
			true,
			true,
			"noncorporeal");
	}

	public static PlanarPresenceDefinition Manifested(IPlane plane)
	{
		var planes = new[] { plane.Id };
		return new PlanarPresenceDefinition(
			planes,
			planes,
			planes,
			Enum.GetValues<PlanarInteractionKind>().ToDictionary(x => x, _ => (IEnumerable<long>)planes),
			false,
			true,
			false,
			false,
			false,
			false,
			true,
			"manifested");
	}

	public static PlanarPresenceDefinition FromXml(string xml, IFuturemud gameworld)
	{
		if (string.IsNullOrWhiteSpace(xml))
		{
			return DefaultMaterial(gameworld);
		}

		try
		{
			return FromXml(XElement.Parse(xml), gameworld);
		}
		catch
		{
			return DefaultMaterial(gameworld);
		}
	}

	public static PlanarPresenceDefinition FromXml(XElement root, IFuturemud gameworld)
	{
		if (root is null)
		{
			return DefaultMaterial(gameworld);
		}

		if (!root.Name.LocalName.EqualTo("PlanarData"))
		{
			root = root.Element("PlanarData") ?? root;
		}

		var fallbackPlaneId = gameworld.DefaultPlane?.Id ?? 1;
		var presence = ReadPlanes(root.Element("Presence"), gameworld, fallbackPlaneId);
		var visible = ReadPlanes(root.Element("VisibleTo"), gameworld, fallbackPlaneId);
		var perceives = ReadPlanes(root.Element("Perceives"), gameworld, fallbackPlaneId);
		if (!presence.Any())
		{
			presence.Add(fallbackPlaneId);
		}

		if (!visible.Any())
		{
			visible.UnionWith(presence);
		}

		if (!perceives.Any())
		{
			perceives.UnionWith(presence);
		}

		var interactions = new Dictionary<PlanarInteractionKind, IEnumerable<long>>();
		var interactionsRoot = root.Element("Interactions");
		foreach (PlanarInteractionKind kind in Enum.GetValues<PlanarInteractionKind>())
		{
			var kindElement = interactionsRoot?
				.Elements("Interaction")
				.FirstOrDefault(x => x.Attribute("kind")?.Value.EqualTo(kind.ToString()) == true);
			interactions[kind] = kindElement is null
				? presence
				: ReadPlanes(kindElement, gameworld, fallbackPlaneId);
		}

		var flags = root.Element("Flags");
		bool ReadFlag(string name, bool defaultValue)
		{
			return bool.TryParse(flags?.Attribute(name)?.Value, out var value) ? value : defaultValue;
		}

		return new PlanarPresenceDefinition(
			presence,
			visible,
			perceives,
			interactions,
			ReadFlag("suspendsPhysicalContact", false),
			ReadFlag("propagatesInventory", true),
			ReadFlag("canCrossClosedDoors", false),
			ReadFlag("canCrossMagicalBarriers", false),
			ReadFlag("visibleToEtherealPerception", false),
			ReadFlag("playerCanManifest", false),
			ReadFlag("playerCanDissipate", false),
			root.Element("Transition")?.Attribute("profile")?.Value ?? string.Empty);
	}

	public XElement SaveToXml()
	{
		return new XElement("PlanarData",
			WritePlanes("Presence", PresencePlaneIds),
			WritePlanes("VisibleTo", VisibleToPlaneIds),
			WritePlanes("Perceives", PerceivesPlaneIds),
			new XElement("Interactions",
				from kind in Enum.GetValues<PlanarInteractionKind>()
				select WritePlanes("Interaction", InteractionPlaneIds(kind), new XAttribute("kind", kind))
			),
			new XElement("Flags",
				new XAttribute("suspendsPhysicalContact", SuspendsPhysicalContact),
				new XAttribute("propagatesInventory", PropagatesInventory),
				new XAttribute("canCrossClosedDoors", CanCrossClosedDoors),
				new XAttribute("canCrossMagicalBarriers", CanCrossMagicalBarriers),
				new XAttribute("visibleToEtherealPerception", VisibleToEtherealPerception),
				new XAttribute("playerCanManifest", PlayerCanManifest),
				new XAttribute("playerCanDissipate", PlayerCanDissipate)),
			new XElement("Transition", new XAttribute("profile", TransitionProfile))
		);
	}

	public PlanarPresenceDefinition Merge(PlanarPresenceDefinition overlay)
	{
		var interactions = Enum.GetValues<PlanarInteractionKind>()
			.ToDictionary(
				x => x,
				x => (IEnumerable<long>)InteractionPlaneIds(x).Concat(overlay.InteractionPlaneIds(x)).Distinct().ToList());
		return new PlanarPresenceDefinition(
			PresencePlaneIds.Concat(overlay.PresencePlaneIds),
			VisibleToPlaneIds.Concat(overlay.VisibleToPlaneIds),
			PerceivesPlaneIds.Concat(overlay.PerceivesPlaneIds),
			interactions,
			SuspendsPhysicalContact || overlay.SuspendsPhysicalContact,
			PropagatesInventory && overlay.PropagatesInventory,
			CanCrossClosedDoors || overlay.CanCrossClosedDoors,
			CanCrossMagicalBarriers || overlay.CanCrossMagicalBarriers,
			VisibleToEtherealPerception || overlay.VisibleToEtherealPerception,
			PlayerCanManifest || overlay.PlayerCanManifest,
			PlayerCanDissipate || overlay.PlayerCanDissipate,
			string.IsNullOrWhiteSpace(overlay.TransitionProfile) ? TransitionProfile : overlay.TransitionProfile);
	}

	public string Describe(IFuturemud gameworld)
	{
		string DescribePlanes(IEnumerable<long> ids)
		{
			return ids
				       .Select(x => gameworld.Planes.Get(x)?.Name ?? $"Plane #{x:N0}")
				       .DefaultIfEmpty("none")
				       .ListToString();
		}

		return $"present on {DescribePlanes(PresencePlaneIds)}, visible to {DescribePlanes(VisibleToPlaneIds)}, perceives {DescribePlanes(PerceivesPlaneIds)}";
	}

	private static HashSet<long> ReadPlanes(XElement element, IFuturemud gameworld, long fallbackPlaneId)
	{
		var result = new HashSet<long>();
		if (element is null)
		{
			return result;
		}

		foreach (XElement planeElement in element.Elements("Plane"))
		{
			var idText = planeElement.Attribute("id")?.Value ?? planeElement.Value;
			if (long.TryParse(idText, out var id))
			{
				result.Add(id);
				continue;
			}

			var name = planeElement.Attribute("name")?.Value ?? planeElement.Value;
			var plane = gameworld.Planes.GetByIdOrName(name);
			if (plane is not null)
			{
				result.Add(plane.Id);
			}
		}

		if (bool.TryParse(element.Attribute("default")?.Value, out var useDefault) && useDefault)
		{
			result.Add(fallbackPlaneId);
		}

		return result;
	}

	private static XElement WritePlanes(string name, IEnumerable<long> planeIds, params object[] attributes)
	{
		return new XElement(name,
			attributes,
			from id in planeIds.Distinct().OrderBy(x => x)
			select new XElement("Plane", new XAttribute("id", id)));
	}
}

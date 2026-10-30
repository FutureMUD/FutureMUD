#nullable enable

using System.Xml.Linq;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework.Scheduling;
using System.Runtime.CompilerServices;

namespace MudSharp.Effects.Concrete;

/// <summary>Persistent source identity for rooms instantiated by dwellings or vehicles.</summary>
public sealed class RoomTemplateInstance : Effect
{
	private static readonly ConditionalWeakTable<IFuturemud, HashSet<long>> PendingPopulation = new();
	public long TemplateRoomId { get; }
	public bool IsTemplate { get; }
	public long OnLoadProgId { get; }
	public RoomTemplateInstance(IRoom owner, long templateRoomId, bool isTemplate, long onLoadProgId = 0) : base(owner)
	{
		TemplateRoomId = templateRoomId;
		IsTemplate = isTemplate;
		OnLoadProgId = onLoadProgId;
	}

	private RoomTemplateInstance(XElement root, IPerceivable owner) : base(root, owner)
	{
		var definition = root.Element("Effect")!;
		TemplateRoomId = (long)definition.Element("TemplateRoomId")!;
		IsTemplate = (bool?)definition.Element("IsTemplate") ?? false;
		OnLoadProgId = (long?)definition.Element("OnLoadProg") ?? 0;
	}

	protected override string SpecificEffectType => "RoomTemplateInstance";
	public override bool SavingEffect => true;
	public override string Describe(IPerceiver voyeur) => $"{(IsTemplate ? "Template" : "Instance of template")} room #{TemplateRoomId}.";
	protected override XElement SaveDefinition() => new("Effect", new XElement("TemplateRoomId", TemplateRoomId),
		new XElement("IsTemplate", IsTemplate), new XElement("OnLoadProg", OnLoadProgId));
	public static void InitialiseEffectType() => RegisterFactory("RoomTemplateInstance", (root, owner) => new RoomTemplateInstance(root, owner));

	public static void CopyTemplate(IRoom template, IRoom instance, string context)
	{
		foreach (var xml in template.SaveEffects().Elements("Effect").Where(x => x.Element("Type")?.Value != "RoomTemplateInstance"))
		{
			var effect = LoadEffect(new XElement(xml), instance);
			if (effect is not null) instance.AddEffect(effect);
		}
		var marker = template.EffectsOfType<RoomTemplateInstance>().FirstOrDefault(x => x.IsTemplate);
		instance.AddEffect(new RoomTemplateInstance(instance, template.Id, false, marker?.OnLoadProgId ?? 0));
		if (!string.IsNullOrWhiteSpace(template.UniqueName))
			instance.TrySetUniqueName($"{template.UniqueName}_{context}", out _);
	}

	public static void InitialiseRooms(IEnumerable<IRoom> rooms)
	{
		var list = rooms.ToArray();
		foreach (var group in list.SelectMany(r => r.EffectsOfType<RoomTemplateInstance>()
			.Where(x => !x.IsTemplate && x.OnLoadProgId > 0).Select(x => (Room: r, Prog: x.OnLoadProgId))).GroupBy(x => x.Prog))
		{
			var room = group.First().Room;
			var progId = group.Key;
			var pending = PendingPopulation.GetOrCreateValue(room.Gameworld);
			lock (pending)
				if (!pending.Add(progId)) continue;
			// Placement and NPC outfit loading must finish before a reset can inspect their
			// counts. Running the zone prog on the insertion call stack can re-enter itself.
			room.Gameworld.Scheduler.AddSchedule(new Schedule(() =>
			{
				lock (pending) pending.Remove(progId);
				if (room.Gameworld.Rooms.Get(room.Id) is { } loaded)
					room.Gameworld.FutureProgs.Get(progId)?.Execute(loaded);
			}, ScheduleType.FutureProg, TimeSpan.FromMilliseconds(1), "Populate instantiated interior"));
		}
	}

	public static void CopyExits(IReadOnlyDictionary<IRoom, IRoom> map)
	{
		foreach (var room in map.Values)
		foreach (var observation in room.EffectsOfType<RoomObservationEffect>())
			observation.RemapObserver(map);
		var copied = new Dictionary<IExit, IExit>();
		foreach (var (template, instance) in map)
		foreach (var side in template.ExitsFor(null))
		{
			if (!map.TryGetValue(side.Destination, out var destination) || side.Exit is not Exit original) continue;
			if (!copied.TryGetValue(original, out var exit))
			{
				// Recovery/reload must not duplicate already-persisted passages or their doors.
				exit = instance.ExitsFor(null).FirstOrDefault(x => ReferenceEquals(x.Destination, destination) &&
					x.OutboundDirection == side.OutboundDirection)?.Exit;
				if (exit is null)
				{
					exit = new Exit(template.Gameworld, instance, destination, original, template).Clone();
					if (original.Door is not null)
					{
						var door = original.Door.Parent.DeepCopy(true, false).GetItemType<IDoor>();
						exit.Door = door;
						door.InstalledExit = exit;
						door.HingeRoom = original.Door.HingeRoom is { } hinge && map.TryGetValue(hinge, out var newHinge) ? newHinge : instance;
						door.OpenDirectionRoom = original.Door.OpenDirectionRoom is { } opening && map.TryGetValue(opening, out var newOpening) ? newOpening : destination;
					}
				}
				copied.Add(original, exit);
			}
			instance.GetOrCreateOverlay(instance.CurrentOverlay.Package).AddExit(exit);
		}
		foreach (var room in map.Values) room.Gameworld.ExitManager.UpdateRoomOverlayExits(room, room.CurrentOverlay);
	}
}

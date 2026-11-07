#nullable enable
using System.IO;
using MudSharp.Construction.Boundary;
using MudSharp.Database;
using MudSharp.RPG.Checks;

namespace MudSharp.Construction.Autobuilder.Areas;

public sealed class AutobuilderAreaTerrainStack : AutobuilderAreaBase
{
	public const string TypeName = "terrain stack";
	public static void RegisterAutobuilderLoader()
	{
		AutobuilderFactory.RegisterLoader(TypeName, (area, world) => new AutobuilderAreaTerrainStack(area, world));
		AutobuilderFactory.RegisterBuilderLoader(TypeName, (world, name) => new AutobuilderAreaTerrainStack(name, world));
	}

	private AutobuilderAreaTerrainStack(string name, IFuturemud world) : base(name, world, TypeName) { }
	private AutobuilderAreaTerrainStack(Models.AutobuilderAreaTemplate area, IFuturemud world) : base(area, world) { }
	protected override void LoadFromXml(XElement element) => ShowCommandByLine = element.Element("ShowCommandByLine")?.Value ?? "Stacked terrain grids";
	protected override XElement SaveToXml() => new("Template", new XElement("ShowCommandByLine", new XCData(ShowCommandByLine)));
	public override string SubtypeHelpText => "\n\t#3room new <area> [prog=<prog>] <room-template> <FMS1:token|editor>#0 - imports stacked terrain";
	public override string Show(ICharacter builder) =>
		$"Autobuilder Area Template #{Id.ToString("N0", builder)} ({Name.ColourName()})\n\nStacked terrain and tags with cardinal neighbours and configurable up/down exits.\n{SubtypeHelpText.SubstituteANSIColour()}";

	protected override void SetupParameters()
	{
		_parameters.Add(new AutobuilderRoomTemplateParameter("room template", "Specify a valid room template.", false, Gameworld));
		_parameters.Add(new AutobuilderCustomParameter
		{
			Gameworld = Gameworld, IsOptional = false, ParameterName = "stack", TypeName = "compressed terrain stack",
			MissingErrorMessage = "Supply the FMS1: token from Terrain Planner, or editor for a multiline paste.",
			IsValidArgumentFunction = (arg, _, _) => TryDecode(arg, out _, out _),
			WhyIsNotValidArgumentFunction = (arg, _, _) => { TryDecode(arg, out _, out var error); return error; },
			GetArgumentFunction = (arg, _) => DecodeAndResolve(arg)
		});
	}

	private bool TryDecode(string text, out TerrainStackPayload? payload, out string error)
	{
		try { payload = DecodeAndResolve(text); error = string.Empty; return true; }
		catch (InvalidDataException exception) { payload = null; error = exception.Message; return false; }
	}

	public TerrainStackPayload DecodeAndResolve(string text)
	{
		var payload = TerrainStackCodec.Decode(text);
		ValidateReferences(payload, Gameworld);
		return payload;
	}

	internal static void ValidateReferences(TerrainStackPayload payload, IFuturemud world)
	{
		TerrainStackCodec.Validate(payload);
		foreach (var terrain in payload.Rooms.Select(room => room.Terrain).Distinct())
			if (!world.Terrains.Has(terrain)) throw new InvalidDataException($"Unknown terrain ID {terrain}.");
		foreach (var tag in payload.Rooms.SelectMany(room => room.Tags).Distinct())
			if (!world.Tags.Has(tag)) throw new InvalidDataException($"Unknown tag ID {tag}.");
	}

	public override IEnumerable<IRoom> ExecuteTemplate(ICharacter builder, IEnumerable<object> arguments)
	{
		var args = arguments.ToArray();
		var template = (IAutobuilderRoom)args[0];
		var payload = (TerrainStackPayload)args[1];
		ValidateReferences(payload, Gameworld);
		var package = builder.CurrentOverlayPackage;
		var rooms = new Dictionary<(int X, int Y, int Z), IRoom>();
		var ordered = payload.Rooms.OrderBy(room => room.Z).ThenBy(room => room.Y).ThenBy(room => room.X).ToArray();
		foreach (var entry in ordered)
		{
			var tags = entry.Tags.Select(id => Gameworld.Tags.Get(id)).ToArray();
			rooms.Add((entry.X, entry.Y, entry.Z), template.CreateRoom(builder, Gameworld.Terrains.Get(entry.Terrain),
				false, tags, tags.Select(tag => tag.Name).ToArray()));
		}
		foreach (var connection in GetConnections(payload))
		{
			var origin = rooms[connection.Origin];
			var destination = rooms[connection.Destination];
			var exit = new Exit(Gameworld, origin, destination, connection.Direction, connection.Direction.Opposite(), 1.0);
			if (connection.Settings is { } settings)
			{
				exit.IsClimbExit = settings.Climb;
				exit.ClimbDifficulty = (Difficulty)settings.Difficulty;
				exit.FallRoom = settings.Fall ? origin : null;
				// AddExit initialises the exit manager from database rows immediately.
				// Persist overrides before either overlay can cache a separate Exit instance.
				exit.Save();
			}
			origin.GetOrCreateOverlay(package).AddExit(exit);
			destination.GetOrCreateOverlay(package).AddExit(exit);
		}
		foreach (var room in rooms.Values) Gameworld.ExitManager.UpdateRoomOverlayExits(room, room.GetOrCreateOverlay(package));
		return ordered.Select(entry => rooms[(entry.X, entry.Y, entry.Z)]).ToList();
	}

	internal static IEnumerable<TerrainStackConnection> GetConnections(TerrainStackPayload payload)
	{
		var occupied = payload.Rooms.Select(room => (room.X, room.Y, room.Z)).ToHashSet();
		var overrides = payload.Links.ToDictionary(link => (link.X, link.Y, link.LowerZ));
		foreach (var room in payload.Rooms)
		{
			var origin = (room.X, room.Y, room.Z);
			if (occupied.Contains((room.X + 1, room.Y, room.Z)))
				yield return new(origin, (room.X + 1, room.Y, room.Z), CardinalDirection.East, null);
			if (occupied.Contains((room.X, room.Y + 1, room.Z)))
				yield return new(origin, (room.X, room.Y + 1, room.Z), CardinalDirection.North, null);
			if (room.Z == int.MaxValue || !occupied.Contains((room.X, room.Y, room.Z + 1))) continue;
			var settings = overrides.GetValueOrDefault(origin);
			if (settings?.Suppressed != true)
				yield return new(origin, (room.X, room.Y, room.Z + 1), CardinalDirection.Up, settings);
		}
	}

	public override IAutobuilderArea Clone(string newName)
	{
		using (new FMDB())
		{
			var row = new Models.AutobuilderAreaTemplate { Name = newName, TemplateType = TypeName, Definition = SaveToXml().ToString() };
			FMDB.Context.AutobuilderAreaTemplates.Add(row);
			FMDB.Context.SaveChanges();
			return new AutobuilderAreaTerrainStack(row, Gameworld);
		}
	}
}

internal sealed record TerrainStackConnection((int X, int Y, int Z) Origin, (int X, int Y, int Z) Destination,
	CardinalDirection Direction, TerrainStackLink? Settings);

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private sealed record TableLayout(string Description, int Seats);

	private static readonly IReadOnlyDictionary<int, TableLayout[]> TableLayouts = new Dictionary<int, TableLayout[]>
	{
		[3066] = [
			new TableLayout("a roundish table under a torch", 5),
			new TableLayout("the blackstone bar", 8),
			new TableLayout("a large, central table", 6),
			new TableLayout("a small table at light 's edge", 2)
		],
		[3067] = [
			new TableLayout("a small table at the end of the bar", 3),
			new TableLayout("a round table under the mural", 5),
			new TableLayout("a table near the beaded archway", 3),
			new TableLayout("a table in the dark corner", 4)
		],
		[3305] = [
			new TableLayout("a large, round table", 10)
		],
		[3313] = [
			new TableLayout("some stools at the bar", 5),
			new TableLayout("a large square table in the center", 4),
			new TableLayout("a round table of agafari", 3),
			new TableLayout("a small booth in the back", 2)
		],
		[3740] = [
			new TableLayout("a small table near the door", 2),
			new TableLayout("a bench along the bar", 4),
			new TableLayout("a long rectangular table", 6),
			new TableLayout("a bench along the side wall", 3),
			new TableLayout("a large circular table", 6)
		],
		[4118] = [
			new TableLayout("a long rectangular table", 5),
			new TableLayout("a small table in the corner", 2)
		],
		[4302] = [
			new TableLayout("a small wooden table, rickety with age", 4),
			new TableLayout("a booth near the door", 7),
			new TableLayout("a winestained baobab table", 5)
		],
		[6012] = [
			new TableLayout("some chairs at the bar", 2),
			new TableLayout("several seat cushions about the tables", 5)
		],
		[6065] = [
			new TableLayout("pillows scattered around a low, round table", 2),
			new TableLayout("some seat cushions around a small table", 7),
			new TableLayout("a wide, scarred table of agafari wood", 5),
			new TableLayout("some chairs at the bar", 5)
		],
		[6118] = [
			new TableLayout("a low stone wall", 4),
			new TableLayout("a small wooden bench", 3)
		],
		[6173] = [
			new TableLayout("a large polished hardwood table", 12)
		],
		[20049] = [
			new TableLayout("a bare agafari table", 3)
		],
		[20086] = [
			new TableLayout("wooden chairs at a rickety table", 2)
		],
		[20419] = [
			new TableLayout("a central round table", 6),
			new TableLayout("a few stools at the bar", 3)
		],
		[20434] = [
			new TableLayout("a large, tall-backed booth", 5),
			new TableLayout("a few seats at the bar", 3),
			new TableLayout("an intimate, tall-backed booth", 2)
		],
		[20607] = [
			new TableLayout("a small, rickety table", 2),
			new TableLayout("some scattered chairs near the bar", 5)
		],
		[20625] = [
			new TableLayout("a row of chairs against the wall", 3),
			new TableLayout("a small table in the back", 2),
			new TableLayout("a square table in the center of the room", 4)
		],
		[21056] = [
			new TableLayout("a long, bone-and-stretched-hide table", 8),
			new TableLayout("a table atop some empty crates", 4),
			new TableLayout("a small table in the corner", 2)
		],
		[21751] = [
			new TableLayout("a large, knife-marked table", 5),
			new TableLayout("a cozy table in a back corner", 2)
		],
		[21771] = [
			new TableLayout("an enormous cylini table", 8),
			new TableLayout("a bar along the north wall", 3),
			new TableLayout("a small table beneath the arrow slits", 2),
			new TableLayout("a fancy carved agafari table", 5)
		],
		[28028] = [
			new TableLayout("a circular agafari table", 6),
			new TableLayout("a small table near the door", 2),
			new TableLayout("a few barstools around the bar", 5)
		],
		[28054] = [
			new TableLayout("a short, round table by the southern wall", 5),
			new TableLayout("a small, square table next to the northern doorway", 2),
			new TableLayout("a long, narrow table in the center of the room", 8)
		],
		[28987] = [
			new TableLayout("a rectangular agafari table", 6),
			new TableLayout("a small table near the stairs", 2),
			new TableLayout("several barstools at the bar", 5)
		],
		[29005] = [
			new TableLayout("a semi - circular bar", 5),
			new TableLayout("an elegant, leather couch", 8),
			new TableLayout("a circular table near the stairs", 4),
			new TableLayout("a small, dark - wood table in the back", 2),
			new TableLayout("a small, polished table near the western wall", 3),
			new TableLayout("a large, rectangular table in the middle of the room", 6),
			new TableLayout("a large, circular table in the middle of the room", 7)
		],
		[48061] = [
			new TableLayout("a large booth", 5),
			new TableLayout("a few seats at the bar", 2)
		],
		[50120] = [
			new TableLayout("a small booth against the west wall", 2),
			new TableLayout("a wide table in the center of the tavern", 4),
			new TableLayout("a narrow bench near the south door", 3),
			new TableLayout("a large, square table near the counter", 5)
		],
		[50399] = [
			new TableLayout("an agafari table bearing many scratches", 3)
		],
		[50401] = [
			new TableLayout("a heavy square table", 6),
			new TableLayout("a corner table", 4)
		],
		[50460] = [
			new TableLayout("a bench along the bar", 5),
			new TableLayout("a small table in a dark corner", 2)
		],
		[50615] = [
			new TableLayout("a plain stone table", 4),
			new TableLayout("a narrow cylini table", 3),
			new TableLayout("a large cylini table", 6),
			new TableLayout("a narrow stone ledge protruding from the wall", 2),
			new TableLayout("a bare agafari table", 3)
		],
		[50618] = [
			new TableLayout("an agafari table bearing many scratches", 4)
		],
		[50775] = [
			new TableLayout("some barrels beside a large crate", 5),
			new TableLayout("some seats beside an improvised table", 3)
		],
		[50790] = [
			new TableLayout("some stools along the bar", 2),
			new TableLayout("some seats beside a small table", 3),
			new TableLayout("chairs at a chipped agafari table", 5)
		],
		[50937] = [
			new TableLayout("a large round table in the center of the room", 7),
			new TableLayout("a small table in the back", 4),
			new TableLayout("a few seats at the bar", 3),
			new TableLayout("a large square table at the back of the room", 6),
			new TableLayout("a small table at the north end", 2)
		],
		[54533] = [
			new TableLayout("a rickety table", 3),
			new TableLayout("a small, chipped table", 3)
		],
		[58100] = [
			new TableLayout("a small table near the door", 2),
			new TableLayout("a bench along the bar", 4),
			new TableLayout("a long rectangular table", 6),
			new TableLayout("a bench along the side wall", 3),
			new TableLayout("a large circular table", 6)
		],
		[58102] = [
			new TableLayout("an ornate agafari table", 5),
			new TableLayout("a priceless carven baobob table", 5),
			new TableLayout("a small painted table in back", 2)
		],
		[58103] = [
			new TableLayout("an ornate marble table", 7)
		],
		[64027] = [
			new TableLayout("a heavy, blockish table in the middle of the room", 6),
			new TableLayout("a few chairs around an agafari table", 3),
			new TableLayout("several stools at the bar", 7),
			new TableLayout("a small table near the stairs", 2)
		],
		[64649] = [
			new TableLayout("scattered pillows around a low table", 4),
			new TableLayout("a table below a hanging spice brazier", 7),
			new TableLayout("a small table in the corner", 5)
		],
		[64994] = [
			new TableLayout("a narrow table at the back of the room", 4),
			new TableLayout("a large, wooden table in the center of the room", 7),
			new TableLayout("a wide table at the north end of the room", 5),
			new TableLayout("a wide table at the south end of the room", 5),
			new TableLayout("a set of chairs around the stage", 4)
		]
	};

	private readonly Dictionary<(int Room, int Index), (GameItemProto Table, GameItemProto Bench)> _tableLayoutPrototypes = new();

	private void InstallTables()
	{
		if (!_items.TryGetValue(20610, out var sourceFurniture))
		{
			return;
		}

		var activeRooms = _source.Resets
			.Where(x => x.Opcode == "O" && x.Arguments.Length > 3 && x.Arguments[1] == 20610 &&
			            x.ReferenceIssues.Length == 0 && _rooms.ContainsKey(x.Arguments[3]))
			.Select(x => x.Arguments[3])
			.Where(TableLayouts.ContainsKey)
			.Distinct()
			.OrderBy(x => x)
			.ToArray();
		foreach (var roomVnum in activeRooms)
		{
			var layouts = TableLayouts[roomVnum];
			for (var index = 0; index < layouts.Length; index++)
			{
				var layout = layouts[index];
				var key = $"Vnum20610Room{roomVnum}Table{index + 1}";
				var table = NewFurnitureProto(key, "table", layout.Description, sourceFurniture.MaterialId, sourceFurniture.Weight,
					sourceFurniture.CostInBaseCurrency);
				Attach(table, Component($"armw.table-layout.{key}", "Table", new XElement("Definition",
					new XAttribute("MaximumChairSlots", layout.Seats),
					new XElement("Cover", new XElement("Flipped", 0), new XElement("NotFlipped", 0),
						new XElement("Expression", new XCData("1")), new XElement("Message", new XCData("It cannot be flipped."))))));
				Attach(table, Component($"armw.table-layout-surface.{key}", "Container", new XElement("Definition",
					new XAttribute("Weight", 100.0 * _kilograms), new XAttribute("MaxSize", (int)SizeCategory.Titanic),
					new XAttribute("Preposition", "on"), new XAttribute("Closable", false),
					new XAttribute("Transparent", true), new XAttribute("OnceOnly", false))));

				var benchDescription = $"seating for {layout.Seats} by {layout.Description}";
				var bench = NewFurnitureProto($"Vnum20610Room{roomVnum}Seat{index + 1}", "bench", benchDescription,
					sourceFurniture.MaterialId, sourceFurniture.Weight, sourceFurniture.CostInBaseCurrency);
				Attach(bench, Component($"armw.table-layout-bench.{key}", "Chair", new XElement("Definition",
					new XAttribute("ChairSlotsUsed", layout.Seats), new XAttribute("ChairOccupantCapacity", layout.Seats))));

				_tableLayoutPrototypes.Add((roomVnum, index), (table, bench));
				Record($"table-layout.{roomVnum}.{index + 1}", nameof(GameItemProto), table.Id, table.RevisionNumber,
					new { Room = roomVnum, SourceVnum = 20610, Layout = layout, TableProto = table.Id, BenchProto = bench.Id,
						Disposition = "Approved authored table layout mapped to one native Table with an open surface Container and one native bench Chair with the authored occupant capacity. No conversation or eavesdropping behavior is added." });
			}
		}
		_counts["native table layouts"] = _tableLayoutPrototypes.Count;
	}

	private GameItemProto NewFurnitureProto(string uniqueName, string noun, string description, long materialId, double weight, decimal cost)
	{
		var proto = new GameItemProto
		{
			Id = Next<GameItemProto>(), Name = noun, UniqueName = uniqueName, Keywords = description,
			ShortDescription = description, LongDescription = description + " is here.", FullDescription = description,
			MaterialId = materialId, Size = (int)SizeCategory.Normal, Weight = weight,
			BaseItemQuality = (int)ItemQuality.Standard, ReadOnly = true, CostInBaseCurrency = cost,
			EditableItem = Approval(), PermitPlayerSkins = false
		};
		_db.GameItemProtos.Add(proto);
		return proto;
	}

	/// <summary>
	/// Builds the native reset statements for one source O reset of Vnum 20610. Returns null when
	/// this source room has no approved layout so the caller can retain its ordinary source prop.
	/// targetRoomVariable is a bare FutureProg variable name such as destination12; resetIndex
	/// supplies unique local variable names and commandVariable is the caller's cmdN variable.
	/// </summary>
	internal string? TryBuildTableReset(int sourceRoomVnum, string targetRoomVariable, int resetIndex,
		string commandVariable)
	{
		if (!TableLayouts.TryGetValue(sourceRoomVnum, out var layouts) || !_tableLayoutPrototypes.ContainsKey((sourceRoomVnum, 0)))
		{
			return null;
		}

		var builder = new StringBuilder();
		builder.AppendLine($"{commandVariable} = true");
		string? firstTableVariable = null;
		for (var index = 0; index < layouts.Length; index++)
		{
			if (!_tableLayoutPrototypes.TryGetValue((sourceRoomVnum, index), out var prototypes))
			{
				throw new InvalidOperationException($"Table layout {sourceRoomVnum}/{index + 1} was not installed before population resets.");
			}

			var suffix = $"{resetIndex}_{index}";
			var tableVariable = "table" + suffix;
			var benchVariable = "bench" + suffix;
			firstTableVariable ??= tableVariable;
			builder.AppendLine($"var {tableVariable} as item");
			builder.AppendLine($"{tableVariable} = @{targetRoomVariable}.items.First(x, @x.proto == {prototypes.Table.Id})");
			builder.AppendLine($"if (isnull(@{tableVariable}))");
			builder.AppendLine($"\t{tableVariable} = loaditem({prototypes.Table.Id})");
			builder.AppendLine($"\tif (isnull(@{tableVariable}) or not(put(@{tableVariable}, @{targetRoomVariable})))");
			builder.AppendLine($"\t\tif (not(isnull(@{tableVariable})))\ndeleteitem(@{tableVariable})\nend if");
			builder.AppendLine($"\t\t{commandVariable} = false");
			builder.AppendLine("\tend if");
			builder.AppendLine("end if");
			builder.AppendLine($"var {benchVariable} as item");
			builder.AppendLine($"{benchVariable} = loadeditems().First(x, @x.proto == {prototypes.Bench.Id} and @x.location == @{targetRoomVariable})");
			builder.AppendLine($"if (not(isnull(@{tableVariable})) and isnull(@{benchVariable}))");
			builder.AppendLine($"\t{benchVariable} = loaditem({prototypes.Bench.Id})");
			builder.AppendLine($"\tif (isnull(@{benchVariable}) or not(put(@{benchVariable}, @{targetRoomVariable})))");
			builder.AppendLine($"\t\tif (not(isnull(@{benchVariable})))\ndeleteitem(@{benchVariable})\nend if");
			builder.AppendLine($"\t\t{commandVariable} = false");
			builder.AppendLine("\tend if");
			builder.AppendLine("end if");
			builder.AppendLine($"if (not(isnull(@{tableVariable})) and not(isnull(@{benchVariable})) and not(attachchair(@{benchVariable}, @{tableVariable})))");
			builder.AppendLine($"\t{commandVariable} = false");
			builder.AppendLine("end if");
		}

		if (firstTableVariable is not null)
		{
			builder.AppendLine($"lastobj = @{firstTableVariable}");
		}

		return builder.ToString();
	}
}

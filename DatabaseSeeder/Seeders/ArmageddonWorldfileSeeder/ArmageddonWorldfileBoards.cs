#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private void InstallBoards()
	{
		var sources = _source.Items.Where(x => x.Programs.Any(p => p.EndsWith(" board", StringComparison.Ordinal)) && _items.ContainsKey(x.Vnum) &&
			_source.Resets.Any(r => r.ReferenceIssues.Length == 0 && r.Opcode is "O" or "P" or "G" or "E" &&
				r.Arguments.Length > 1 && r.Arguments[1] == x.Vnum)).ToArray();
		if (sources.Length == 0) return;
		var staff = Prog("ArmageddonWorldfileStaffBoardAccess", "return isadmin(@actor)", ProgVariableTypes.Boolean,
			("actor", ProgVariableTypes.Character));
		var boards = new Dictionary<int, Board>();
		foreach (var source in sources)
		foreach (var proto in new[] { _items[source.Vnum] }.Concat(_resetItemVariants.Where(x => x.Key.StartsWith(source.Vnum + ":", StringComparison.Ordinal)).Select(x => x.Value)).DistinctBy(x => x.Id))
		{
			var id = _effectiveItems[proto.Id].Values[0];
			if (!boards.TryGetValue(id, out var board))
			{
				board = new Board { Id = Next<Board>(), Name = $"{source.ShortDescription} (Vnum{source.Vnum})", ShowOnLogin = false,
					CalendarId = _db.Cultures.Single(x => x.Id == _settings.NativeBindings[ArmageddonNativeBinding.Culture]).PrimaryCalendarId };
				_db.Boards.Add(board); boards.Add(id, board);
				Record("board." + id, nameof(Board), board.Id, null, new { SourceBoard = id,
					Disposition = "Empty native bulletin board. Source IDs 1/3 require staff; absent source metadata adds no clan/language restriction. Native author/admin deletion remains unchanged; no historical posts or identities imported." });
			}
			var access = id is 1 or 3 ? staff.Id : _settings.NativeBindings[ArmageddonNativeBinding.AlwaysTrueProg];
			Attach(proto, Component("board." + proto.Id, "Board", new XElement("Definition",
				new XElement("Board", board.Id), new XElement("CanViewBoard", access), new XElement("CanPostToBoard", access),
				new XElement("CantViewBoardEcho", "You cannot use this board."), new XElement("CantPostToBoardEcho", "You cannot post to this board."),
				new XElement("ShowAuthorName", false), new XElement("ShowAuthorDescription", false), new XElement("ShowAuthorShortDescription", false),
				new XElement("StoredAuthorName", ""), new XElement("StoredAuthorShortDescription", ""), new XElement("StoredAuthorFullDescription", ""))));
		}
		_counts["bulletin boards"] = boards.Count;
	}
}

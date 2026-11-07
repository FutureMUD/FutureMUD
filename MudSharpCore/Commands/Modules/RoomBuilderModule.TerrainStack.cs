#nullable enable
using System.IO;
using MudSharp.Construction.Autobuilder;
using MudSharp.Construction.Autobuilder.Areas;
using MudSharp.Framework.Revision;

namespace MudSharp.Commands.Modules;

internal partial class RoomBuilderModule
{
	private static void RoomNewTerrainStack(ICharacter actor, StringStack input, AutobuilderAreaTerrainStack area,
		IReadOnlyList<IFutureProg> progs)
	{
		var room = actor.Gameworld.AutobuilderRooms.GetByIdOrName(input.PopSpeech());
		if (room is null) { actor.Send("Specify a valid room template."); return; }
		var data = input.PopSpeech();
		if (input.IsFinished == false || string.IsNullOrEmpty(data))
		{
			actor.Send("Supply one FMS1: token, or editor for a multiline paste.");
			return;
		}
		var package = actor.CurrentOverlayPackage;
		var zone = actor.Location.OwningZone;
		void Import(string text, IOutputHandler output)
		{
			if (!CanImportTerrainStack(actor, package) || actor.Location.OwningZone != zone ||
				actor.Gameworld.AutobuilderRooms.Get(room.Id) != room || actor.Gameworld.AutobuilderAreas.Get(area.Id) != area)
			{
				output.Send("The overlay package, zone or templates changed. Start the terrain stack import again.");
				return;
			}
			TerrainStackPayload payload;
			try { payload = area.DecodeAndResolve(text); }
			catch (InvalidDataException exception) { output.Send(exception.Message.ColourError()); return; }
			ExecuteRoomAreaTemplate(actor, area, new object[] { room, payload }, progs, output);
		}
		if (data.EqualTo("editor"))
		{
			actor.Send("Paste the wrapped FMS1: export, then submit with @ on its own line. Use *cancel to abandon the import.");
			actor.EditorMode((text, handler, _) => Import(text, handler), (handler, _) => handler.Send("Terrain stack import cancelled; no rooms were created."));
			return;
		}
		if (data.Length >= TerrainStackCodec.SingleTokenLimit)
		{
			actor.Send("This terrain stack is too long for single-token import. Use editor and paste the wrapped export.");
			return;
		}
		Import(data, actor.OutputHandler);
	}

	internal static bool CanImportTerrainStack(ICharacter actor, MudSharp.Construction.IRoomOverlayPackage? expectedPackage) =>
		expectedPackage is not null && actor.CurrentOverlayPackage == expectedPackage && expectedPackage.Status == RevisionStatus.UnderDesign;
}

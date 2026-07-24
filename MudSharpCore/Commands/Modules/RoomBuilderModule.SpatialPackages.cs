#nullable enable

using System.Text;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.ImportExport;
using MudSharp.Framework;

namespace MudSharp.Commands.Modules;

internal partial class RoomBuilderModule
{
	private const string SpatialPackageHelpText =
		@"The #3SPATIALPACKAGE#0 command exports and imports portable, versioned spatial-area packages.

Version 2 transfers one or more zones: room coordinates, cells, active overlays, route-cell geometry, internal and selected cross-zone exits, zone geography and environment, tags, local covers, and magic-resource state. Imports always create new zones and remap all room, cell, overlay, route and exit IDs. They never merge into or overwrite existing spatial content. Version 1 packages remain importable.

Files are read and written only inside the server's #6Spatial Packages#0 directory.

	#3spatialpackage export zone <zone> <file>#0 - exports a zone; the .fmsa.json suffix is optional
	#3spatialpackage export zones <file> <zone> [zone...]#0 - exports multiple zones and links between them
	#3spatialpackage validate <file> <target shard> [new zone name]#0 - preflights integrity and dependencies
	#3spatialpackage import <file> <target shard> confirm [new zone name]#0 - creates the validated zone(s)

You must be editing an under-design #3CELL PACKAGE#0 to validate or import. A new-name override is only valid for a single-zone package. Quote multi-word zone, shard and file arguments where required. Spatial packages refuse hosted vehicle interiors, temporary cells, agriculture fields, persisted cell effects, surface liquids, installed door items, and external fall destinations rather than silently losing those dependencies. Every deliberate omission is listed by export, validation and import.";

	[PlayerCommand("SpatialPackage", "spatialpackage")]
	[CommandPermission(PermissionLevel.SeniorAdmin)]
	[HelpInfo("spatialpackage", SpatialPackageHelpText, AutoHelp.HelpArgOrNoArg)]
	protected static void SpatialPackage(ICharacter actor, string input)
	{
		var command = new StringStack(input.RemoveFirstWord());
		switch (command.PopForSwitch())
		{
			case "export":
				SpatialPackageExport(actor, command);
				return;
			case "validate":
			case "check":
				SpatialPackageValidate(actor, command);
				return;
			case "import":
				SpatialPackageImport(actor, command);
				return;
			default:
				actor.OutputHandler.Send(SpatialPackageHelpText.SubstituteANSIColour());
				return;
		}
	}

	private static void SpatialPackageExport(ICharacter actor, StringStack command)
	{
		var mode = command.PopForSwitch();
		if (mode.EqualTo("zones"))
		{
			SpatialPackageExportZones(actor, command);
			return;
		}

		if (!mode.EqualTo("zone"))
		{
			actor.OutputHandler.Send(
				$"Use {"spatialpackage export zone <zone> <file>".ColourCommand()} or {"spatialpackage export zones <file> <zone> [zone...]".ColourCommand()}.");
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which zone do you want to export?");
			return;
		}

		var zoneText = command.PopSpeech();
		var zone = actor.Gameworld.Zones.GetByIdOrName(zoneText, false);
		if (zone is null)
		{
			actor.OutputHandler.Send($"There is no zone identified by {zoneText.ColourCommand()}.");
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What file name should be used for the package?");
			return;
		}

		var fileName = command.PopSpeech();
		if (!command.IsFinished)
		{
			actor.OutputHandler.Send("The file name must be one argument and may not contain a path.");
			return;
		}

		var result = SpatialAreaTransferService.Instance.ExportZone(zone, fileName);
		SendSpatialPackageResult(actor, result);
	}

	private static void SpatialPackageExportZones(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What file name should be used for the package?");
			return;
		}

		var fileName = command.PopSpeech();
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which zones do you want to export?");
			return;
		}

		var zones = new List<IZone>();
		while (!command.IsFinished)
		{
			var zoneText = command.PopSpeech();
			var zone = actor.Gameworld.Zones.GetByIdOrName(zoneText, false);
			if (zone is null)
			{
				actor.OutputHandler.Send($"There is no zone identified by {zoneText.ColourCommand()}.");
				return;
			}

			if (zones.Any(x => x.Id == zone.Id))
			{
				actor.OutputHandler.Send($"Zone {zone.Name.ColourName()} was specified more than once.");
				return;
			}

			zones.Add(zone);
		}

		var result = SpatialAreaTransferService.Instance.ExportZones(zones, fileName);
		SendSpatialPackageResult(actor, result);
	}

	private static void SpatialPackageValidate(ICharacter actor, StringStack command)
	{
		if (!TryParseSpatialPackageTarget(actor, command, out var fileName, out var shard))
		{
			return;
		}

		var zoneNameOverride = command.IsFinished ? null : command.SafeRemainingArgument;
		var result = SpatialAreaTransferService.Instance.ValidateImport(
			actor,
			shard!,
			fileName!,
			zoneNameOverride);
		SendSpatialPackageResult(actor, result);
	}

	private static void SpatialPackageImport(ICharacter actor, StringStack command)
	{
		if (!TryParseSpatialPackageTarget(actor, command, out var fileName, out var shard))
		{
			return;
		}

		if (command.IsFinished || !command.PopForSwitch().EqualTo("confirm"))
		{
			actor.OutputHandler.Send(
				$"Import requires the explicit {"confirm".ColourCommand()} keyword after the target shard. Run {"spatialpackage validate".ColourCommand()} first.");
			return;
		}

		var zoneNameOverride = command.IsFinished ? null : command.SafeRemainingArgument;
		var result = SpatialAreaTransferService.Instance.ImportZone(
			actor,
			shard!,
			fileName!,
			zoneNameOverride);
		SendSpatialPackageResult(actor, result);
	}

	private static bool TryParseSpatialPackageTarget(
		ICharacter actor,
		StringStack command,
		out string? fileName,
		out IShard? shard)
	{
		fileName = null;
		shard = null;
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which package file do you want to use?");
			return false;
		}

		fileName = command.PopSpeech();
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Into which existing shard should the new zone be imported?");
			return false;
		}

		var shardText = command.PopSpeech();
		shard = actor.Gameworld.Shards.GetByIdOrName(shardText, false);
		if (shard is null)
		{
			actor.OutputHandler.Send($"There is no shard identified by {shardText.ColourCommand()}.");
			return false;
		}

		return true;
	}

	private static void SendSpatialPackageResult(ICharacter actor, SpatialAreaTransferResult result)
	{
		var text = new StringBuilder();
		text.AppendLine(result.Success ? result.Summary.ColourValue() : result.Summary.ColourError());
		if (result.PackagePath is not null)
		{
			text.AppendLine($"Package: {result.PackagePath.ColourCommand()}");
		}

		if (result.ZoneCount > 0 || result.RoomCount > 0 || result.CellCount > 0 || result.ExitCount > 0)
		{
			text.AppendLine(
				$"Contents: {result.ZoneCount.ToString("N0", actor).ColourValue()} zone(s), {result.RoomCount.ToString("N0", actor).ColourValue()} room(s), {result.CellCount.ToString("N0", actor).ColourValue()} cell(s), {result.ExitCount.ToString("N0", actor).ColourValue()} packaged exit(s)");
		}

		foreach (var diagnostic in result.Diagnostics.Where(x => x.Code != "empty-room-skipped"))
		{
			var line = $"[{diagnostic.Code}] {diagnostic.Message}";
			text.AppendLine(diagnostic.Severity switch
			{
				SpatialAreaTransferDiagnosticSeverity.Error => line.ColourError(),
				SpatialAreaTransferDiagnosticSeverity.Warning => line.Colour(Telnet.Yellow),
				_ => line
			});
		}

		if (result.OmittedItems.Count > 0)
		{
			text.AppendLine();
			text.AppendLine("The following items were not included and will not be imported:".Colour(Telnet.Yellow));
			foreach (var (item, index) in result.OmittedItems.Select((value, index) => (value, index)))
			{
				text.AppendLine($"{(index + 1).ToString("N0", actor)}) {item}");
			}
		}

		actor.OutputHandler.Send(text.ToString());
	}
}

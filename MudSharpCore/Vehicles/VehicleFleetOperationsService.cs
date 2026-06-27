#nullable enable

using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using DB = MudSharp.Models;

namespace MudSharp.Vehicles;

public class VehicleFleetOperationsService : IVehicleFleetOperationsService
{
	public const string VehicleAccessPresetsStaticConfiguration = "VehicleAccessPresets";

	private static readonly IReadOnlyList<VehicleAccessPreset> DefaultAccessPresets =
	[
		new("passenger", [new VehicleAccessPresetGrant("board", 1)]),
		new("operator", [new VehicleAccessPresetGrant("board", 1), new VehicleAccessPresetGrant("control", 2)]),
		new("mechanic", [new VehicleAccessPresetGrant("board", 1), new VehicleAccessPresetGrant("service", 2), new VehicleAccessPresetGrant("repair", 2)]),
		new("hitcher", [new VehicleAccessPresetGrant("board", 1), new VehicleAccessPresetGrant("hitch", 2)]),
		new("crew", [new VehicleAccessPresetGrant("board", 1), new VehicleAccessPresetGrant("control", 2), new VehicleAccessPresetGrant("service", 2), new VehicleAccessPresetGrant("hitch", 2)]),
		new("owner", [new VehicleAccessPresetGrant("all", 3)])
	];

	private readonly IVehicleOperationalReadinessService _readinessService;
	private readonly IVehicleHitchGraphService _graphService;

	public VehicleFleetOperationsService() : this(new VehicleOperationalReadinessService(), new VehicleHitchGraphService())
	{
	}

	public VehicleFleetOperationsService(IVehicleOperationalReadinessService readinessService,
		IVehicleHitchGraphService graphService)
	{
		_readinessService = readinessService;
		_graphService = graphService;
	}

	public IReadOnlyList<VehicleAccessPreset> AccessPresets(IFuturemud gameworld)
	{
		return ParsePresets(gameworld.GetStaticConfiguration(VehicleAccessPresetsStaticConfiguration));
	}

	public bool TryGetAccessPreset(IFuturemud gameworld, string name, out VehicleAccessPreset? preset, out string reason)
	{
		preset = AccessPresets(gameworld).FirstOrDefault(x => x.Name.EqualTo(name));
		if (preset is not null)
		{
			reason = string.Empty;
			return true;
		}

		reason = "There is no such vehicle access preset.";
		return false;
	}

	public VehicleAccessPreset SaveAccessPreset(IFuturemud gameworld, string name,
		IEnumerable<VehicleAccessPresetGrant> grants)
	{
		var preset = new VehicleAccessPreset(name.ToLowerInvariant(), grants.ToList());
		var presets = AccessPresets(gameworld)
			.Where(x => !x.Name.EqualTo(name))
			.Append(preset)
			.OrderBy(x => x.Name)
			.ToList();
		SavePresets(gameworld, presets);
		return preset;
	}

	public bool DeleteAccessPreset(IFuturemud gameworld, string name, out string reason)
	{
		var presets = AccessPresets(gameworld).ToList();
		var removed = presets.RemoveAll(x => x.Name.EqualTo(name));
		if (removed == 0)
		{
			reason = "There is no such vehicle access preset.";
			return false;
		}

		SavePresets(gameworld, presets);
		reason = string.Empty;
		return true;
	}

	public VehicleAccessPreset ResetAccessPreset(IFuturemud gameworld, string name)
	{
		var preset = DefaultAccessPresets.FirstOrDefault(x => x.Name.EqualTo(name)) ??
		             new VehicleAccessPreset(name.ToLowerInvariant(), []);
		var presets = AccessPresets(gameworld)
			.Where(x => !x.Name.EqualTo(name))
			.Append(preset)
			.OrderBy(x => x.Name)
			.ToList();
		SavePresets(gameworld, presets);
		return preset;
	}

	public IReadOnlyList<VehicleAccessPreset> ResetAllAccessPresets(IFuturemud gameworld)
	{
		SavePresets(gameworld, DefaultAccessPresets);
		return DefaultAccessPresets;
	}

	public VehicleAccessPresetApplicationResult ApplyAccessPreset(IVehicle vehicle, ICharacter character,
		VehicleAccessPreset preset)
	{
		var rows = preset.Grants
			.Select(grant => vehicle.GrantAccess(character, grant.AccessTag, grant.AccessLevel))
			.ToList();
		return new VehicleAccessPresetApplicationResult(vehicle, character, preset, rows, string.Empty);
	}

	public int CloneAccess(IVehicle targetVehicle, IVehicle sourceVehicle)
	{
		var count = 0;
		foreach (var access in sourceVehicle.AccessStates.Where(x => x.Character is not null))
		{
			targetVehicle.GrantAccess(access.Character!, access.AccessTag, access.AccessLevel);
			count++;
		}

		return count;
	}

	public VehicleFleetAuditResult Audit(IEnumerable<IVehicle> vehicles, ICharacter voyeur, VehicleFleetAuditMode mode)
	{
		var vehicleList = vehicles.DistinctBy(x => x.Id).OrderBy(x => x.Id).ToList();
		var findings = new List<VehicleFleetAuditFinding>();
		foreach (var vehicle in vehicleList)
		{
			var movementProfile = vehicle.Prototype.MovementProfiles
				.Where(x => x.MovementType == VehicleMovementProfileType.CellExit)
				.OrderByDescending(x => x.IsDefault)
				.FirstOrDefault();
			var report = _readinessService.BuildReport(vehicle, voyeur, null, movementProfile);
			foreach (var issue in report.Issues.Where(x => Includes(mode, x.Subsystem)))
			{
				findings.Add(new VehicleFleetAuditFinding(vehicle, issue.Subsystem, issue.Severity, issue.Reason,
					issue.RepairHint));
			}

			if (Includes(mode, VehicleOperationalSubsystem.Access) && !vehicle.AccessStates.Any())
			{
				findings.Add(new VehicleFleetAuditFinding(vehicle, VehicleOperationalSubsystem.Access,
					VehicleOperationalSeverity.Information, "No explicit access grants; access is permissive.",
					"vehicle access <vehicle> apply <preset> <character>"));
			}

			if (mode is VehicleFleetAuditMode.All or VehicleFleetAuditMode.Recovery)
			{
				findings.AddRange(Recover(vehicle, VehicleRecoveryMode.All, false).Findings.Select(x =>
					new VehicleFleetAuditFinding(vehicle, x.Subsystem, x.Severity, x.Reason, x.Hint)));
			}
		}

		return new VehicleFleetAuditResult(vehicleList, findings);
	}

	public VehicleRecoveryResult Recover(IVehicle vehicle, VehicleRecoveryMode mode, bool apply)
	{
		var findings = new List<VehicleRecoveryFinding>();
		if (Matches(mode, VehicleRecoveryMode.Projection))
		{
			var exterior = vehicle.ExteriorItem?.GetItemType<IVehicleExterior>();
			if (vehicle.ExteriorItem is null || exterior is null || exterior.Vehicle?.Id != vehicle.Id)
			{
				findings.Add(new VehicleRecoveryFinding(vehicle, VehicleOperationalSubsystem.Repair,
					VehicleOperationalSeverity.Blocking, "The exterior projection item is missing or not linked back to this vehicle.",
					"vehicle relink <vehicle> <item>", VehicleRecoveryAction.Warning));
			}
		}

		if (Matches(mode, VehicleRecoveryMode.Install))
		{
			foreach (var installation in vehicle.Installations.Where(x => x.InstalledItem?.Deleted == true || x.InstalledItem?.Destroyed == true))
			{
				findings.Add(new VehicleRecoveryFinding(vehicle, VehicleOperationalSubsystem.Module,
					VehicleOperationalSeverity.Warning,
					$"{installation.Prototype.Name} has a deleted or destroyed installed module.",
					"uninstall, repair, or replace the module", VehicleRecoveryAction.Warning));
			}
		}

		if (Matches(mode, VehicleRecoveryMode.Hitch) && vehicle.ExteriorItem is not null)
		{
			foreach (var link in _graphService.LinksInvolving(vehicle.Gameworld, vehicle.ExteriorItem)
				         .Where(x => x.Kind is VehicleHitchGraphLinkKind.LegacyVehicleTow or VehicleHitchGraphLinkKind.PersistentHitch))
			{
				if (_graphService.ValidateLink(link, out var reason) && !link.IsDisabled)
				{
					continue;
				}

				var action = VehicleRecoveryAction.Warning;
				var hint = "unhitch and re-hitch after repairing the cause";
				if (apply && link.IsDisabled && _readinessService.RepairHitchLink(link, out var repairReason))
				{
					action = VehicleRecoveryAction.Repaired;
					reason = "Persistent hitch/tow link was revalidated and repaired.";
					hint = string.Empty;
				}
				else if (apply && string.IsNullOrWhiteSpace(reason))
				{
					reason = "The link is disabled and could not be automatically repaired.";
				}

				findings.Add(new VehicleRecoveryFinding(vehicle, VehicleOperationalSubsystem.HitchTow,
					VehicleOperationalSeverity.Warning, reason, hint, action));
			}
		}

		return new VehicleRecoveryResult(vehicle, findings, apply);
	}

	private static bool Includes(VehicleFleetAuditMode mode, VehicleOperationalSubsystem subsystem)
	{
		return mode == VehicleFleetAuditMode.All || mode.ToString().EqualTo(subsystem.ToString()) ||
		       mode switch
		       {
			       VehicleFleetAuditMode.Readiness => subsystem != VehicleOperationalSubsystem.Access,
			       VehicleFleetAuditMode.Resources => subsystem is VehicleOperationalSubsystem.Fuel or VehicleOperationalSubsystem.Power,
			       VehicleFleetAuditMode.Hitch => subsystem == VehicleOperationalSubsystem.HitchTow,
			       _ => false
		       };
	}

	private static bool Matches(VehicleRecoveryMode mode, VehicleRecoveryMode target)
	{
		return mode == VehicleRecoveryMode.All || mode == target;
	}

	private static IReadOnlyList<VehicleAccessPreset> ParsePresets(string text)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			return [];
		}

		return text.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			.Select(ParsePreset)
			.Where(x => x is not null)
			.Cast<VehicleAccessPreset>()
			.ToList();
	}

	private static VehicleAccessPreset? ParsePreset(string text)
	{
		var split = text.Split('=', 2, StringSplitOptions.TrimEntries);
		if (split.Length != 2 || string.IsNullOrWhiteSpace(split[0]))
		{
			return null;
		}

		var grants = split[1].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			.Select(x => x.Split(':', 2, StringSplitOptions.TrimEntries))
			.Where(x => x.Length == 2 && int.TryParse(x[1], out _))
			.Select(x => new VehicleAccessPresetGrant(x[0].ToLowerInvariant(), Math.Clamp(int.Parse(x[1]), 1, 3)))
			.ToList();
		return new VehicleAccessPreset(split[0].ToLowerInvariant(), grants);
	}

	private static void SavePresets(IFuturemud gameworld, IEnumerable<VehicleAccessPreset> presets)
	{
		var text = string.Join(';', presets.Select(x =>
			$"{x.Name}={string.Join(',', x.Grants.Select(grant => $"{grant.AccessTag}:{grant.AccessLevel}"))}"));
		using (new FMDB())
		{
			var dbitem = FMDB.Context.StaticConfigurations.Find(VehicleAccessPresetsStaticConfiguration);
			if (dbitem is null)
			{
				dbitem = new DB.StaticConfiguration
				{
					SettingName = VehicleAccessPresetsStaticConfiguration
				};
				FMDB.Context.StaticConfigurations.Add(dbitem);
			}

			dbitem.Definition = text;
			FMDB.Context.SaveChanges();
		}

		gameworld.UpdateStaticConfiguration(VehicleAccessPresetsStaticConfiguration, text);
	}
}
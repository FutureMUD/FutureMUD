#nullable enable

using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.GameItems;
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
			var movementProfile = vehicle.MovementProfile;
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

			if (mode is VehicleFleetAuditMode.All or VehicleFleetAuditMode.Route)
			{
				var routeProfiles = vehicle.Prototype.MovementProfiles
					.Where(x => x.MovementType == VehicleMovementProfileType.Route)
					.ToList();
				if (!routeProfiles.Any())
				{
					findings.Add(new VehicleFleetAuditFinding(vehicle, VehicleOperationalSubsystem.Route,
						VehicleOperationalSeverity.Warning,
						"The vehicle prototype has no longitudinal RouteCell movement profile.",
						"vehicleproto set movement route"));
				}

				if (vehicle.Location?.RouteDefinition is { } route &&
				    (!vehicle.RoutePositionMetres.HasValue ||
				     !double.IsFinite(vehicle.RoutePositionMetres.Value) ||
				     vehicle.RoutePositionMetres.Value < 0.0 ||
				     vehicle.RoutePositionMetres.Value > route.LengthMetres))
				{
					findings.Add(new VehicleFleetAuditFinding(vehicle, VehicleOperationalSubsystem.Route,
						VehicleOperationalSeverity.Blocking,
						"The vehicle has no valid durable coordinate in its current RouteCell.",
						"vehicle recover <vehicle> projection fix, then transfer it to a valid route coordinate"));
				}
			}

			if ((mode is VehicleFleetAuditMode.All or VehicleFleetAuditMode.Journey) &&
			    vehicle.ActiveJourney is { } journey)
			{
				if (journey.Route is VehicleRoute route && !route.TopologyIsCurrent)
				{
					findings.Add(new VehicleFleetAuditFinding(vehicle, VehicleOperationalSubsystem.Journey,
						VehicleOperationalSeverity.Blocking,
						$"Active journey #{journey.Id:N0} has stale RouteCell topology pins.",
						"cancel the journey and recompile/approve a new route revision"));
				}
				else if (journey.State == VehicleJourneyState.Held)
				{
					findings.Add(new VehicleFleetAuditFinding(vehicle, VehicleOperationalSubsystem.Journey,
						VehicleOperationalSeverity.Warning,
						$"Active journey #{journey.Id:N0} is held: {journey.StatusReason}",
						"resolve the readiness reason before the maximum hold expires"));
				}
			}

			var recoveryMode = mode switch
			{
				VehicleFleetAuditMode.All or VehicleFleetAuditMode.Recovery => VehicleRecoveryMode.All,
				VehicleFleetAuditMode.Interior => VehicleRecoveryMode.Interior,
				VehicleFleetAuditMode.Docking => VehicleRecoveryMode.Docking,
				_ => (VehicleRecoveryMode?)null
			};
			if (recoveryMode is { } selectedRecoveryMode)
			{
				findings.AddRange(Recover(vehicle, selectedRecoveryMode, false).Findings.Select(x =>
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

			foreach (var access in vehicle.AccessPoints)
			{
				IGameItem? projection;
				try
				{
					projection = access.ProjectionItem;
				}
				catch (Exception ex)
				{
					findings.Add(new VehicleRecoveryFinding(vehicle, VehicleOperationalSubsystem.Access,
						VehicleOperationalSeverity.Blocking,
						$"{access.Name} projection could not be inspected safely: {ex.Message}",
						"check the engine log and persisted projection item before retrying recovery",
						VehicleRecoveryAction.Warning));
					continue;
				}

				if (projection is { Deleted: false, Destroyed: false })
				{
					continue;
				}

				var action = VehicleRecoveryAction.Warning;
				var reason = $"{access.Name} has no usable authored access-point projection item.";
				var hint = "vehicle recover <vehicle> projection fix";
				var repairReason = string.Empty;
				if (apply && VehicleFactory.TryCreateAccessProjectionItem(access, null, out var created,
					    out repairReason))
				{
					action = VehicleRecoveryAction.Repaired;
					reason = created
						? $"{access.Name} was assigned replacement projection item #{access.ProjectionItemId:N0}."
						: $"{access.Name} already has a usable projection item.";
					hint = string.Empty;
				}
				else if (apply)
				{
					hint = repairReason;
				}

				findings.Add(new VehicleRecoveryFinding(vehicle, VehicleOperationalSubsystem.Access,
					VehicleOperationalSeverity.Blocking, reason, hint, action));
			}

			foreach (var cargo in vehicle.CargoSpaces)
			{
				IGameItem? projection;
				try
				{
					projection = cargo.ProjectionItem;
				}
				catch (Exception ex)
				{
					findings.Add(new VehicleRecoveryFinding(vehicle, VehicleOperationalSubsystem.Module,
						VehicleOperationalSeverity.Warning,
						$"{cargo.Name} projection could not be inspected safely: {ex.Message}",
						"check the engine log and persisted projection item before retrying recovery",
						VehicleRecoveryAction.Warning));
					continue;
				}

				if (projection is { Deleted: false, Destroyed: false })
				{
					continue;
				}

				var action = VehicleRecoveryAction.Warning;
				var reason = $"{cargo.Name} has no usable authored cargo projection item.";
				var hint = "vehicle recover <vehicle> projection fix";
				var repairReason = string.Empty;
				if (apply && VehicleFactory.TryCreateCargoProjectionItem(cargo, null, out var created,
					    out repairReason))
				{
					action = VehicleRecoveryAction.Repaired;
					reason = created
						? $"{cargo.Name} was assigned replacement projection item #{cargo.ProjectionItemId:N0}."
						: $"{cargo.Name} already has a usable projection item.";
					hint = string.Empty;
				}
				else if (apply)
				{
					hint = repairReason;
				}

				findings.Add(new VehicleRecoveryFinding(vehicle, VehicleOperationalSubsystem.Module,
					VehicleOperationalSeverity.Warning, reason, hint, action));
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

		if (Matches(mode, VehicleRecoveryMode.Interior) &&
		    vehicle.Prototype.Scale == VehicleScale.RoomScale && vehicle is Vehicle concreteVehicle)
		{
			foreach (var compartment in vehicle.Compartments)
			{
				if (compartment.InteriorCell is null)
				{
					var occupied = concreteVehicle.IsInteriorOccupied(compartment);
					var action = VehicleRecoveryAction.Warning;
					var reason = compartment.InteriorCellId is null
						? $"{compartment.Name} has no hosted interior cell assigned."
						: $"{compartment.Name} points to missing hosted cell #{compartment.InteriorCellId:N0}.";
					var hint = occupied
						? "restore the missing cell from backup before moving its recorded occupants"
						: "vehicle recover <vehicle> interior fix";
					var repairReason = string.Empty;
					if (apply && !occupied && compartment is VehicleCompartment runtime &&
					    RoomScaleVehicleInteriorService.TryRecoverInterior(concreteVehicle, runtime,
						    out var recoveryAction, out repairReason))
					{
						action = VehicleRecoveryAction.Repaired;
						reason = recoveryAction == RoomScaleVehicleInteriorService.RecoveryAction.Relinked
							? $"{compartment.Name} was relinked to persisted hosted cell #{compartment.InteriorCellId:N0}."
							: $"{compartment.Name} was assigned a new hosted interior cell.";
						hint = string.Empty;
					}
					else if (apply && !occupied && !string.IsNullOrWhiteSpace(repairReason))
					{
						hint = repairReason;
					}

					findings.Add(new VehicleRecoveryFinding(vehicle, VehicleOperationalSubsystem.Interior,
						occupied ? VehicleOperationalSeverity.Blocking : VehicleOperationalSeverity.Warning,
						reason, hint, action));
					continue;
				}

				if (compartment.InteriorCell is Cell hostedCell &&
				    (hostedCell.HostedVehicleId != vehicle.Id ||
				     hostedCell.HostedVehicleCompartmentId != compartment.Id))
				{
					var action = VehicleRecoveryAction.Warning;
					if (apply && compartment is VehicleCompartment runtime)
					{
						RoomScaleVehicleInteriorService.RepairHostMetadata(concreteVehicle, runtime);
						action = VehicleRecoveryAction.Repaired;
					}

					findings.Add(new VehicleRecoveryFinding(vehicle, VehicleOperationalSubsystem.Interior,
						VehicleOperationalSeverity.Warning,
						$"{compartment.Name} cell #{hostedCell.Id:N0} has incorrect hosted-vehicle ownership metadata.",
						apply ? string.Empty : "vehicle recover <vehicle> interior fix", action));
				}
			}

			var brokenLinks = vehicle.Compartments
				.SelectMany(x => x.Links)
				.DistinctBy(x => x.Id)
				.Where(x => x.Exit is null)
				.ToList();
			if (brokenLinks.Any() || vehicle.Prototype.CompartmentLinks.Count() !=
			    vehicle.Compartments.SelectMany(x => x.Links).DistinctBy(x => x.Id).Count())
			{
				if (apply)
				{
					concreteVehicle.RebuildCompartmentLinks();
				}

				findings.Add(new VehicleRecoveryFinding(vehicle, VehicleOperationalSubsystem.Interior,
					VehicleOperationalSeverity.Warning,
					"One or more authored compartment links are missing their live transient exit.",
					apply ? string.Empty : "vehicle recover <vehicle> interior fix",
					apply ? VehicleRecoveryAction.Repaired : VehicleRecoveryAction.Warning));
			}
		}

		if (Matches(mode, VehicleRecoveryMode.Docking) &&
		    vehicle.Prototype.Scale == VehicleScale.RoomScale && vehicle is Vehicle dockingVehicle)
		{
			var expectedAccess = (vehicle.Location?.RouteDefinition is not null
					? vehicle.ActiveJourney?.BoardingOpen == true
						? vehicle.ActiveJourney.CurrentStop?.PlatformBindings
							.Select(binding => vehicle.AccessPoints.FirstOrDefault(x =>
								x.Prototype.Id == binding.AccessPoint.Id))
							.Where(x => x is not null)
							.Cast<IVehicleAccessPoint>() ?? []
						: []
					: vehicle.AccessPoints)
				.Where(x => dockingVehicle.CompartmentFor(x.Prototype.Compartment)?.InteriorCell is not null)
				.ToList();
			var faults = expectedAccess.Where(x =>
				vehicle.Dockings.All(docking => docking.AccessPoint.Id != x.Id) ||
				vehicle.Dockings.Any(docking =>
				{
					if (docking.AccessPoint.Id != x.Id)
					{
						return false;
					}

					var staleLocation = docking is VehicleDocking routeDocking &&
					                    vehicle.Location?.RouteDefinition is not null
						? !VehicleDockingService.IsAtBoundRouteStop(vehicle, routeDocking)
						: docking.ExteriorCell != vehicle.Location || docking.ExteriorLayer != vehicle.RoomLayer;
					return staleLocation || x.IsOpen && !x.IsDisabled && !x.IsLocked &&
						docking is VehicleDocking runtime && !runtime.IsRegistered;
				}))
				.ToList();
			if (faults.Any())
			{
				var repaired = false;
				if (apply && vehicle.ExteriorItem is not null && vehicle.Location is not null)
				{
					dockingVehicle.RebuildDockings();
					repaired = true;
				}

				findings.Add(new VehicleRecoveryFinding(vehicle, VehicleOperationalSubsystem.Docking,
					vehicle.ExteriorItem is null ? VehicleOperationalSeverity.Blocking : VehicleOperationalSeverity.Warning,
					$"{faults.Count:N0} access point{(faults.Count == 1 ? " has" : "s have")} missing, stale, or inactive docking topology.",
					repaired ? string.Empty : "restore/relink the exterior, then use vehicle recover <vehicle> docking fix",
					repaired ? VehicleRecoveryAction.Repaired : VehicleRecoveryAction.Warning));
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

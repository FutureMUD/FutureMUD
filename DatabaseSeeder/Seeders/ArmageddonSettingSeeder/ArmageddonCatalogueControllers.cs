#nullable enable
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

/// <summary>Explicit native controller bundles; no inference from AI names or definition text.</summary>
public static class ArmageddonCatalogueControllers
{
	public static ArmageddonInstallResult Install(Func<FuturemudDatabaseContext> factory,
		Action<ArmageddonInstallCheckpoint>? checkpoint = null)
	{
		using var db = factory();
		var settings = ArmageddonInstallation.Read(db) ?? throw new InvalidOperationException("Fix Armageddon settings before controller bundles.");
		var errors = ArmageddonInstallation.ValidateFoundations(db, settings);
		if (errors.Count != 0) return new(ArmageddonInstallStatus.Blocked, errors, new Dictionary<string, long>());
		var messages = new List<string>(); var ids = new Dictionary<string, long>(StringComparer.Ordinal);
		var committing = false; var committed = false;
		try
		{
			using var transaction = db.Database.IsRelational() ? db.Database.BeginTransaction(IsolationLevel.Serializable) : null;
			var owned = new ArmageddonCatalogueOwnership(db, settings.NativeBindings[ArmageddonNativeBinding.BuilderAccount], messages);
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.PreflightComplete);
			var rideKey = "arm.ai.earth_mount.permission";
			var ride = new FutureProg { FunctionName = "ArmageddonMountPermission", ReturnType = (long)ProgVariableTypes.Boolean,
				FunctionText = "return cancommandspellnpc(@mount, @rider)", AcceptsAnyParameters = false, StaticType = 0,
				Category = "Magic", Subcategory = "Armageddon", FunctionComment = "Exact current spell-owned creator admission; native riding supplies size/posture/slots/movement gates.", Public = false };
			ride.FutureProgsParameters.Add(new() { ParameterIndex = 0, ParameterName = "rider", ParameterType = (long)ProgVariableTypes.Character });
			ride.FutureProgsParameters.Add(new() { ParameterIndex = 1, ParameterName = "mount", ParameterType = (long)ProgVariableTypes.Character });
			ride = owned.Put(rideKey, ride, []);
			var animationKey = "arm.ai.servitor.permission";
			var animation = new FutureProg { FunctionName = "ArmageddonServitorPermission", ReturnType = (long)ProgVariableTypes.Boolean,
				FunctionText = "return cancommandanimation(@animation, @commander)", AcceptsAnyParameters = false, StaticType = 0,
				Category = "Magic", Subcategory = "Armageddon", FunctionComment = "Exact existing native corpse-animation command grant.", Public = false };
			animation.FutureProgsParameters.Add(new() { ParameterIndex = 0, ParameterName = "animation", ParameterType = (long)ProgVariableTypes.Character });
			animation.FutureProgsParameters.Add(new() { ParameterIndex = 1, ParameterName = "commander", ParameterType = (long)ProgVariableTypes.Character });
			animation.FutureProgsParameters.Add(new() { ParameterIndex = 2, ParameterName = "command", ParameterType = (long)ProgVariableTypes.Text });
			animation = owned.Put(animationKey, animation, []);
			foreach (var blueprint in new ArmageddonStockCatalogue().Blueprints.Where(x => x.Kind == ArmageddonBlueprintKind.AiBundle))
			{
				var config = blueprint.Configuration;
				var commands = config.GetProperty("commands").EnumerateArray().Select(x => x.GetString()!).ToArray();
				if (blueprint.Key == "arm.ai.servitor")
				{
					var ai = owned.Put(blueprint.Key, new ArtificialIntelligence { Name = "Armageddon " + blueprint.Name, Type = "Commandable",
						Definition = new XElement("Definition", new XElement("CanCommandProg", animation.Id), new XElement("WhyCannotCommandProg", 0),
							new XElement("CommandIssuedEmote", "$0 issue|issues an order to $1."), new XElement("BannedCommands"),
							new XElement("IncludedCommands", commands.Select(x => new XElement("Command", x)))).ToString() }, [animationKey]);
					ids.Add(blueprint.Key, ai.Id);
					owned.Put(blueprint.Key + ".combat_end", new ArtificialIntelligence { Name = "Armageddon Servitor combat closure", Type = "CombatEnd",
						Definition = new XElement("Definition", new XElement("WillAcceptTruce", settings.NativeBindings[ArmageddonNativeBinding.AlwaysTrueProg]),
							new XElement("WillAcceptTargetIncapacitated", settings.NativeBindings[ArmageddonNativeBinding.AlwaysTrueProg]),
							new XElement("OnOfferedTruce", 0), new XElement("OnTargetIncapacitated", 0), new XElement("OnNoNaturalTargets", 0)).ToString() }, []);
					continue;
				}
				var outsider = blueprint.Key == "arm.ai.outsider";
				var companion = owned.Put(blueprint.Key, new ArtificialIntelligence { Name = "Armageddon " + blueprint.Name, Type = "SpellCompanion",
					Definition = new XElement("Definition", new XElement("Mode", outsider ? "outsider" : "companion"),
						new XElement("Purpose", config.GetProperty("native_behavior").GetString()),
						new XElement("Permitted", commands.Select(x => new XElement("Order", x))),
						outsider ? new XElement("LossChancePerGrade", 0.05) : null, new XElement("LossChanceCap", 0.5)).ToString() }, blueprint.Dependencies);
				ids.Add(blueprint.Key, companion.Id);
				if (blueprint.Key != "arm.ai.earth_mount") continue;
				owned.Put(blueprint.Key + ".riding", new ArtificialIntelligence { Name = "Armageddon Earth Mount riding", Type = "Mount",
					Definition = new XElement("Definition", new XElement("PermitRiderProg", ride.Id), new XElement("PermitControlProg", ride.Id),
						new XElement("WhyCannotPermitRiderProg", 0), new XElement("MountNonConsensualMountDifficulty", (int)Difficulty.Impossible),
						new XElement("MountControlDifficulty", (int)Difficulty.Trivial), new XElement("MountResistBuckDifficulty", (int)Difficulty.Normal),
						new XElement("MaximumNumberOfRiders", 1), new XElement("RawMountEmote", "$1 $1|mount|mounts $0."),
						new XElement("RawDismountEmote", "$1 $1|dismount|dismounts $0."),
						new XElement("RawControlDeniedEmote", "$0 refuse|refuses to obey $1's order."),
						new XElement("RawBuckEmote", "$0 try|tries to buck $1!")).ToString() }, [rideKey, blueprint.Key]);
			}
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.ContentCreated); checkpoint?.Invoke(ArmageddonInstallCheckpoint.BeforeCommit);
			committing = true; transaction?.Commit(); committed = true; checkpoint?.Invoke(ArmageddonInstallCheckpoint.AfterCommit);
			messages.Add("S3 native controller bundles committed: seven spell companions, creator-gated MountAI and separate corpse-animation Commandable/CombatEnd AIs.");
			return new(ArmageddonInstallStatus.Completed, messages.AsReadOnly(), ids);
		}
		catch (Exception error)
		{
			messages.Add((committed ? "Committed; confirmation failed. " : committing ? "Commit outcome unknown; reconnect before rerunning. " :
				"Controller bundles failed before commit; relational transaction rolled back. ") + error.Message);
			if (!committed) ids.Clear();
			return new(committed ? ArmageddonInstallStatus.CommittedConfirmationFailed : committing ? ArmageddonInstallStatus.CommitOutcomeUnknown :
				ArmageddonInstallStatus.Failed, messages.AsReadOnly(), ids);
		}
	}
}

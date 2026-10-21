extern alias EngineCompiler;
#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Magic;
using MudSharp.Models;
using OfflineProgCompilation = EngineCompiler::MudSharp.Framework.OfflineProgCompilation;
using OfflineWorld = EngineCompiler::MudSharp.Framework.Futuremud;
using CapabilityFactory = EngineCompiler::MudSharp.Magic.Capabilities.MagicCapabilityFactory;
using ResourceLoader = EngineCompiler::MudSharp.Magic.Resources.BaseMagicResource;

namespace DatabaseSeeder.Seeders;

public static partial class ArmageddonTraditionInstaller
{
	// Read-model constructors only: no boot, player loading, prog execution or database writes.
	private static void ValidateNativeGathering(FuturemudDatabaseContext db, MagicCapability template,
		OfflineProgCompilation compiler, IReadOnlyList<FutureProg> progs, List<string> errors)
	{
		using var world = new OfflineWorld(null!);
		var definition = XElement.Parse(template.Definition);
		definition.Elements("Casting").Remove();
		definition.Elements("Power").Remove();
		definition.Element("Regenerators")?.Remove();
		definition.Add(new XElement("Regenerators"));
		var capability = (IMagicGatheringCapability)CapabilityFactory.LoadCapability(new MagicCapability
		{
			Id = template.Id, Name = template.Name, MagicSchoolId = template.MagicSchoolId,
			PowerLevel = template.PowerLevel, CapabilityModel = template.CapabilityModel, Definition = definition.ToString()
		}, world);
		var methods = capability.GatheringMethods;
		var resourceIds = methods.SelectMany(x => new[] { x.DestinationResourceId, x.SourceResourceId ?? 0 })
			.Concat(methods.SelectMany(x => x.LandSources).Select(x =>
				x.Selector.Trim().StartsWith("ambient:", StringComparison.OrdinalIgnoreCase) &&
				long.TryParse(x.Selector.Trim()[8..], out var id) ? id : 0)).Where(x => x > 0).Distinct().ToArray();
		foreach (var resource in db.MagicResources.AsNoTracking().Where(x => resourceIds.Contains(x.Id)))
			world.Add(ResourceLoader.LoadResource(resource, world));
		var progIds = methods.SelectMany(x => new[] { x.PermissionProgId, x.DurationProgId, x.StaminaCostProgId,
			x.DamageCostProgId, x.PainCostProgId, x.StunCostProgId, x.OnGatheredProgId, x.LandDamageProgId,
			x.LandPressureProgId, x.CropHealthCostProgId, x.WoodlandHealthCostProgId })
			.Concat(methods.SelectMany(x => x.LandSources).Select(x => x.RatioProgId)).Where(x => x > 0).Distinct();
		foreach (var id in progIds.Where(id => progs.Any(x => x.Id == id))) world.Add(compiler.Compile(id));
		errors.AddRange(capability.GatheringConfigurationErrors().Select(x => "Native gathering template: " + x));
	}
}

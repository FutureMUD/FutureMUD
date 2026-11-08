#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using MudSharp.Models;
using CultureInfo = System.Globalization.CultureInfo;

namespace DatabaseSeeder.Seeders;

/// <summary>
/// Resolves physical prerequisites from the actual attached/proposed component revisions.
/// This validates composition and wearable configuration, not anatomical fit or a whole ensemble.
/// </summary>
internal static class IndustrialisedClothingPhysicalBindings
{
	internal static ClothingWearableBinding Bind(IReadOnlyCollection<GameItemComponentProto> components,
		IReadOnlyCollection<WearProfile> wearProfiles, IReadOnlyCollection<long> progIds,
		ClothingSourceLocation source)
	{
		var metadata = IndustrialisedComponentMetadataCatalogue.Document.Types;
		var resolved = components.Select(component =>
		{
			if (!metadata.TryGetValue(component.Type, out var type) || type.Name != component.Type)
				throw source.Error($"Unknown or noncanonical runtime component type {component.Type} on {component.Name}.");
			return (Component: component, Type: type);
		}).ToArray();
		if (components.Select(x => x.Id).Distinct().Count() != components.Count)
			throw source.Error("Clothing cannot contain multiple revisions or instances of the same logical component.");
		var capabilities = resolved.SelectMany(x => x.Type.Capabilities).ToHashSet(StringComparer.Ordinal);
		foreach (var exclusive in resolved.SelectMany(x => x.Type.ExclusiveTypes).Distinct(StringComparer.Ordinal))
		{
			var providers = resolved.Where(x => x.Type.Capabilities.Contains(exclusive, StringComparer.Ordinal)).ToArray();
			if (providers.Length > 1)
				throw source.Error($"Exclusive capability {exclusive} is supplied by multiple components: {string.Join(", ", providers.Select(x => x.Component.Name))}.");
		}
		foreach (var component in resolved)
		{
			if (component.Type.ContextDependentRequirements)
				throw source.Error($"Component {component.Component.Name} requires context-dependent validation before clothing admission.");
			foreach (var required in component.Type.RequiredSiblingTypes.Where(x => !capabilities.Contains(x)))
				throw source.Error($"Component {component.Component.Name} requires missing sibling capability {required}.");
		}
		if (!capabilities.Contains("IHoldable") || !capabilities.Contains("IWearable"))
			throw source.Error("A standalone clothing base requires both Holdable and Wearable capabilities.");
		var wearable = resolved.Single(x => x.Type.Name == "Wearable").Component;
		return WearableComponentValidation.BindWearable(wearable, wearProfiles, progIds, source);
	}

}

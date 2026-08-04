#nullable enable

using MudSharp.Models;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private const string RenaissanceEarlyModernJewelleryKnowledge = "Renaissance and Early Modern Jewellery";
	private const string RenaissanceEarlyModernJoineryKnowledge = "Renaissance and Early Modern Joinery";
	private const string RenaissanceEarlyModernLocksmithingKnowledge = "Renaissance and Early Modern Locksmithing";

	internal static bool ShouldSeedRenaissanceEarlyModernJewelleryDoorCraftsForTesting(string? eras)
	{
		return !string.IsNullOrWhiteSpace(eras) &&
		       HasAnyEra(eras, "antiquity", "medieval", "renaissance", "earlymodern");
	}

	private bool ShouldSeedRenaissanceEarlyModernJewelleryDoorCrafts()
	{
		return _questionAnswers?.TryGetValue("eras", out var eras) == true &&
		       ShouldSeedRenaissanceEarlyModernJewelleryDoorCraftsForTesting(eras);
	}

	private void SeedRenaissanceEarlyModernJewelleryDoorCrafts()
	{
		if (!ShouldSeedRenaissanceEarlyModernJewelleryDoorCrafts())
		{
			return;
		}

		foreach (var target in _items
		         .Where(x => IsRenaissanceEarlyModernJewelleryDoorReference(x.Key) &&
		                     RenaissanceEarlyModernJewelleryDoorsCraftNamesByStableReference.ContainsKey(x.Key))
		         .OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase))
		{
			SeedRenaissanceEarlyModernJewelleryDoorFinishedCraft(target.Key, target.Value);
		}
	}

	private static bool IsRenaissanceEarlyModernJewelleryDoorReference(string stableReference)
	{
		return stableReference.StartsWith("preindustrial_jewellery_", StringComparison.OrdinalIgnoreCase) ||
		       stableReference.StartsWith("preindustrial_door_", StringComparison.OrdinalIgnoreCase) ||
		       stableReference.StartsWith("renaissance_jewellery_", StringComparison.OrdinalIgnoreCase) ||
		       stableReference.StartsWith("renaissance_door_", StringComparison.OrdinalIgnoreCase) ||
		       stableReference.StartsWith("earlymodern_jewellery_", StringComparison.OrdinalIgnoreCase) ||
		       stableReference.StartsWith("earlymodern_door_", StringComparison.OrdinalIgnoreCase);
	}

	private void SeedRenaissanceEarlyModernJewelleryDoorFinishedCraft(string stableReference, GameItemProto item)
	{
		var material = GetMaterialName(item);
		var path = GetRenaissanceEarlyModernJewelleryDoorCraftPath(stableReference, material);
		var displayName = item.ShortDescription;
		if (!RenaissanceEarlyModernJewelleryDoorsCraftNamesByStableReference.TryGetValue(stableReference,
			out var craftName))
		{
			throw new InvalidOperationException($"Missing deterministic craft name for {stableReference}.");
		}
		var materialAmount = Math.Max(path.MinimumMaterialGrams, item.Weight * path.MaterialRetentionRatio)
			.ToString("0", System.Globalization.CultureInfo.InvariantCulture);

		AddCraft(
			craftName,
			path.Category,
			$"{path.Verb} {displayName}",
			path.Action,
			$"an in-progress {StripLeadingArticle(displayName)} craft",
			path.Knowledge,
			path.Trait,
			path.MinimumTraitValue,
			path.Difficulty,
			Outcome.MinorFail,
			5,
			3,
			false,
			RenaissanceEarlyModernJewelleryDoorCraftingPhases(path.WorkKind),
			[$"Commodity - {materialAmount} grams of {material}"],
			path.Tools,
			[$"SimpleProduct - 1x {displayName} (#{item.Id})"],
			[],
			knowledgeSubtype: path.Category,
			knowledgeDescription: path.KnowledgeDescription,
			knowledgeLongDescription: path.KnowledgeDescription);
	}

	private static RenaissanceEarlyModernJewelleryDoorCraftPath GetRenaissanceEarlyModernJewelleryDoorCraftPath(
		string stableReference, string material)
	{
		if (stableReference.Contains("_jewellery_", StringComparison.OrdinalIgnoreCase))
		{
			return new RenaissanceEarlyModernJewelleryDoorCraftPath(
				"Jewellery and devotional work",
				"make",
				"making jewellery or a devotional item",
				RenaissanceEarlyModernJewelleryKnowledge,
				GetRenaissanceEarlyModernJewelleryTrait(material),
				15,
				Difficulty.Normal,
				0.80,
				8.0,
				"jewellery",
				[
					"TagTool - Held - an item with the Pliers tag",
					"TagTool - Held - an item with the Hammer tag"
				],
				"This knowledge covers visible-form jewellery and devotional work using supported wearable and portable component profiles.");
		}

		if (stableReference.Contains("_door_", StringComparison.OrdinalIgnoreCase) &&
		    !IsRenaissanceEarlyModernLocksmithingReference(stableReference))
		{
			return new RenaissanceEarlyModernJewelleryDoorCraftPath(
				"Carpentry and joinery",
				"build",
				"building a door, gate, shutter, screen, grille, or barrier",
				RenaissanceEarlyModernJoineryKnowledge,
				"Carpentry",
				15,
				Difficulty.Hard,
				0.88,
				250.0,
				"joinery",
				[
					"TagTool - Held - an item with the Hammer tag",
					"TagTool - Held - an item with the Saw tag"
				],
				"This knowledge covers portable supported door, gate, shutter, screen, grille, and barrier construction without asserting installed-exit behaviour.");
		}

		return new RenaissanceEarlyModernJewelleryDoorCraftPath(
			"Locksmithing",
			"forge",
			"forging a loose lock, key, latch, or fitting",
			RenaissanceEarlyModernLocksmithingKnowledge,
			"Blacksmithing",
			15,
			Difficulty.Normal,
			0.82,
			30.0,
			"locksmithing",
			[
				"TagTool - Held - an item with the Hammer tag",
				"TagTool - InRoom - an item with the Anvil tag"
			],
			"This knowledge covers loose warded locks, keys, latches, and fittings using only the supported seeded component behaviour.");
	}

	private static string GetRenaissanceEarlyModernJewelleryTrait(string material)
	{
		if (material.Equals("glass", StringComparison.OrdinalIgnoreCase) ||
		    material.Equals("faience", StringComparison.OrdinalIgnoreCase))
		{
			return "Glassworking";
		}

		if (material.Equals("bone", StringComparison.OrdinalIgnoreCase) ||
		    material.Equals("shell", StringComparison.OrdinalIgnoreCase) ||
		    material.Equals("ivory", StringComparison.OrdinalIgnoreCase) ||
		    material.Equals("coral", StringComparison.OrdinalIgnoreCase))
		{
			return "Scrimshawing";
		}

		if (material.Equals("wood", StringComparison.OrdinalIgnoreCase))
		{
			return "Carpentry";
		}

		if (material.Equals("silk", StringComparison.OrdinalIgnoreCase) ||
		    material.Equals("linen", StringComparison.OrdinalIgnoreCase))
		{
			return "Weaving";
		}

		return material.Equals("copper", StringComparison.OrdinalIgnoreCase) ||
		       material.Equals("bronze", StringComparison.OrdinalIgnoreCase) ||
		       material.Equals("silver", StringComparison.OrdinalIgnoreCase) ||
		       material.Equals("gold", StringComparison.OrdinalIgnoreCase) ||
		       material.Equals("brass", StringComparison.OrdinalIgnoreCase) ||
		       material.Equals("pewter", StringComparison.OrdinalIgnoreCase)
			? "Silversmithing"
			: "Gemcraft";
	}

	private static bool IsRenaissanceEarlyModernLocksmithingReference(string stableReference)
	{
		return stableReference.Contains("warded_lock", StringComparison.OrdinalIgnoreCase) ||
		       stableReference.Contains("warded_key", StringComparison.OrdinalIgnoreCase) ||
		       stableReference.Contains("ring_key", StringComparison.OrdinalIgnoreCase) ||
		       stableReference.Contains("keyring", StringComparison.OrdinalIgnoreCase) ||
		       stableReference.Contains("latch", StringComparison.OrdinalIgnoreCase) ||
		       stableReference.Contains("dropbar", StringComparison.OrdinalIgnoreCase) ||
		       stableReference.Contains("door_bar", StringComparison.OrdinalIgnoreCase) ||
		       stableReference.Contains("pawl", StringComparison.OrdinalIgnoreCase) ||
		       stableReference.Contains("hasp", StringComparison.OrdinalIgnoreCase) ||
		       stableReference.Contains("hook", StringComparison.OrdinalIgnoreCase) ||
		       stableReference.Contains("strike_plate", StringComparison.OrdinalIgnoreCase) ||
		       stableReference.Contains("hinge_pair", StringComparison.OrdinalIgnoreCase) ||
		       stableReference.Contains("escutcheon", StringComparison.OrdinalIgnoreCase);
	}

	private static (int Seconds, string Echo, string FailEcho)[] RenaissanceEarlyModernJewelleryDoorCraftingPhases(
		string workKind)
	{
		return workKind switch
		{
			"jewellery" =>
			[
				(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1."),
				(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops."),
				(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.")
			],
			"joinery" =>
			[
				(35, "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges."),
				(45, "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", "$0 shape|shapes the material and secure|secures its fitted parts with $t1."),
				(35, "$0 inspect|inspects the finished work and set|sets aside $p1.", "$0 find|finds the work has failed to hold together and set|sets the material aside.")
			],
			_ =>
			[
				(25, "$0 heat|heats and prepare|prepares $i1 at the work area with $t1.", "$0 heat|heats and prepare|prepares $i1 at the work area with $t1."),
				(35, "$0 work|works the fitting with $t1, checking its accessible moving surfaces.", "$0 work|works the fitting with $t1, checking its accessible moving surfaces."),
				(30, "$0 finish|finishes the fitting and set|sets aside $p1.", "$0 end|ends the unsuccessful fitting and set|sets the material aside.")
			]
		};
	}

	private sealed record RenaissanceEarlyModernJewelleryDoorCraftPath(
		string Category,
		string Verb,
		string Action,
		string Knowledge,
		string Trait,
		int MinimumTraitValue,
		Difficulty Difficulty,
		double MaterialRetentionRatio,
		double MinimumMaterialGrams,
		string WorkKind,
		IReadOnlyList<string> Tools,
		string KnowledgeDescription);
}

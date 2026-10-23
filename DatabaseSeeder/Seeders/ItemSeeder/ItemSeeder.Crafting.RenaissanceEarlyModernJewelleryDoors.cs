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

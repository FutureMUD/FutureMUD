using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Magic;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonAffordabilityTests
{
	[TestMethod]
	public void SourceRoster_EveryAllowedGradePairHasAttainableEnergy_AndLowerPoolLimitsAreVisible()
	{
		var root = Path.Combine(VancianExampleProgTests.RepositoryRoot(), "Design Documents", "Magic");
		using var tree = JsonDocument.Parse(File.ReadAllText(Path.Combine(root, "Armageddon_Sorcerer_Source_Tree.json")));
		using var input = JsonDocument.Parse(File.ReadAllText(Path.Combine(root, "Armageddon_Capacity_Affordability_Scenarios.json")));
		var config = input.RootElement;
		var spells = tree.RootElement.GetProperty("rows").EnumerateArray().Where(x => x.GetProperty("kind").GetString() == "spell").ToArray();
		var capacities = config.GetProperty("scenarios").EnumerateArray().Select(s =>
		{
			var expression = new ExpressionEngine.Expression(config.GetProperty("capacity_formula").GetString()!);
			Assert.IsTrue(expression.TryEvaluateDoubleWith(new Dictionary<string, object> { ["variable"] = s.GetProperty("attribute").GetDouble() }, out var cap, out var error), error);
			Assert.AreEqual(s.GetProperty("expected_capacity").GetDouble(), cap);
			return cap;
		}).ToArray();
		var rows = 0; var lowerRefusals = 0;
		foreach (var profile in config.GetProperty("profiles").EnumerateArray())
		foreach (var spell in spells)
		{
			var efficiency = new ControlledSpellEfficiency(spell.GetProperty("printed_minimum_mana").GetDouble(), profile.GetProperty("energy_scale").GetDouble());
			for (var mastery = 1; mastery <= 7; mastery++)
			for (var requested = 1; requested <= Math.Min(7, mastery + 1); requested++)
			{
				var cost = efficiency.Cost(mastery, requested) * (requested > mastery ? profile.GetProperty("overreach_multiplier").GetDouble() : 1.0) + profile.GetProperty("secondary_cost").GetDouble();
				Assert.IsTrue(capacities.Any(cap => cap >= cost), $"No attainable scenario for {spell.GetProperty("key").GetString()} grade {requested}, mastery {mastery}, profile {profile.GetProperty("key").GetString()}.");
				if (capacities[0] < cost) lowerRefusals++;
				rows += capacities.Length;
			}
		}
		Assert.AreEqual(82, spells.Length); Assert.AreEqual(config.GetProperty("coverage").GetProperty("expected_matrix_rows").GetInt32(), rows);
		Assert.IsTrue(lowerRefusals > 0, "The matrix must expose lower pools that cannot fund some legal casts.");
	}
}

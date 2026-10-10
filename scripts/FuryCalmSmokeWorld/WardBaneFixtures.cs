#nullable enable

using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using FutureProg = MudSharp.Models.FutureProg;

internal static class WardBaneFixtures
{
	internal static void Run(DbContextOptions<FuturemudDatabaseContext> options, string receipt)
	{
		using var db = new FuturemudDatabaseContext(options);
		using var transaction = db.Database.BeginTransaction();
		var eligible = db.FutureProgs.SingleOrDefault(x => x.FunctionName == "QA_N19_CreatureEligible");
		if (eligible is null)
		{
			var seed = db.FutureProgs.First(x => x.FunctionText == "return true" && !x.FutureProgsParameters.Any());
			eligible = new FutureProg();
			foreach (var property in typeof(FutureProg).GetProperties(BindingFlags.Instance | BindingFlags.Public)
				.Where(x => x.CanWrite && (x.PropertyType.IsValueType || x.PropertyType == typeof(string))))
				property.SetValue(eligible, property.GetValue(seed));
			eligible.Id = 0; eligible.FunctionName = "QA_N19_CreatureEligible"; eligible.StaticType = 0;
			eligible.FunctionText = "return hasmagictag(@target, \"qa-n19-creature\")";
			eligible.FutureProgsParameters.Add(new() { ParameterIndex = 0, ParameterName = "target", ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString() });
			eligible.FutureProgsParameters.Add(new() { ParameterIndex = 1, ParameterName = "caster", ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString() });
			db.FutureProgs.Add(eligible); db.SaveChanges();
		}
		var no = db.FutureProgs.First(x => x.FunctionText == "return false" && !x.FutureProgsParameters.Any()).Id;
		var ids = new Dictionary<string, long>();
		foreach (var (name, tag, trigger) in new[] { ("QA_N19_Mark", "qa-n19-creature", "character"),
			("QA_N19_FireProbe", "qa-n19-fire", "character"), ("QA_N19_ForeignWard", "qa-n19-unused", "room") })
		{
			var row = db.MagicSpells.SingleOrDefault(x => x.Name == name);
			if (row is null)
			{
				var content = new ArmageddonUtilitySpellContent("qa.fixture." + name, name, "Disposable native N19 probe.", "1800", 8,
					"$0 invoke|invokes a qualification enchantment.", (resource, cost, _) => ArmageddonUtilitySpellContent.Definition(
						"qa.fixture." + name, trigger, resource, cost, 0, 30, 1, trigger == "room" ?
						new XElement("Effect", new XAttribute("type", "roomtagward"), new XElement("Tag", tag), new XElement("Value", ""),
							new XElement("MatchValue", false), new XElement("Mode", "Fail"), new XElement("Coverage", "Both"), new XElement("Prog", 0)) :
						new XElement("Effect", new XAttribute("type", "magictag"), new XElement("Tag", tag), new XElement("Value", "fixture"), new XElement("ReplaceExisting", true))));
				row = content.SpellRow(5, 297, no); row.CastingDifficulty = (int)Difficulty.Automatic;
				db.MagicSpells.Add(row); db.SaveChanges();
				var duration = content.DurationRow(row.Id); var cost = content.CostRow(row.Id);
				db.TraitExpressions.AddRange(duration, cost); db.SaveChanges();
				row.EffectDurationExpressionId = duration.Id; row.Definition = content.BuildDefinition(4, cost.Id, 0).ToString(); db.SaveChanges();
			}
			// These exact owned probes can be replayed. Keep the source efficiency floor
			// above 50/7 so their native grade-1 payment is exactly eight units.
			var definition = XElement.Parse(row.Definition);
			if (definition.Element("StockIdentity")?.Value != "qa.fixture." + name)
				throw new InvalidOperationException("The owned probe's stock identity changed.");
			definition.Element("ControlledPower")!.Element("Efficiency")!.SetAttributeValue("minimum", 8);
			row.Definition = definition.ToString();
			var expressionId = (long)definition.Element("Costs")!.Elements("Cost").Single().Attribute("expression")!;
			db.TraitExpressions.Single(x => x.Id == expressionId).Expression = "8*grade";
			db.TraitExpressions.Single(x => x.Id == row.EffectDurationExpressionId).Expression = name == "QA_N19_ForeignWard" ? "90" : "1800";
			db.SaveChanges();
			ids.Add(name, row.Id);
		}
		transaction.Commit();
		File.WriteAllText(receipt, JsonSerializer.Serialize(new { Status = "PASS", Eligibility = eligible.Id, Spells = ids },
			new JsonSerializerOptions { WriteIndented = true }));
	}
}

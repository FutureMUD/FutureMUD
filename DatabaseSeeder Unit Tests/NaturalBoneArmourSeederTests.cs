#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Health;
using DbArmourType = MudSharp.Models.ArmourType;

namespace MudSharp_Unit_Tests;

[TestClass]
public class NaturalBoneArmourSeederTests
{
	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void SeededBoneArmour_LoadsValidFormulaMapsAndEvaluatesChopping(bool animal)
	{
		string definition;
		if (animal)
		{
			using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
			var seeder = new AnimalSeeder();
			typeof(AnimalSeeder).GetField("_context", BindingFlags.Instance | BindingFlags.NonPublic)!
				.SetValue(seeder, context);
			typeof(AnimalSeeder).GetMethod("SetupArmourTypes", BindingFlags.Instance | BindingFlags.NonPublic)!
				.Invoke(seeder, null);
			definition = context.ArmourTypes.Single(x => x.Name == "Non-Human Natural Bone Armour").Definition;
		}
		else
		{
			definition = (string)typeof(HumanSeeder)
				.GetMethod("BuildHumanBoneArmourDefinition", BindingFlags.Static | BindingFlags.NonPublic)!
				.Invoke(null, null)!;
		}

		var armour = new ArmourType(new DbArmourType { Definition = definition }, Mock.Of<IFuturemud>());
		foreach (var map in new[] { armour.DissipateExpressions, armour.DissipateExpressionsPain,
			armour.DissipateExpressionsStun, armour.AbsorbExpressions, armour.AbsorbExpressionsPain,
			armour.AbsorbExpressionsStun })
		{
			Assert.IsTrue(map.Count >= 23);
			foreach (var expression in map.Values) Assert.IsFalse(expression.HasErrors(), expression.OriginalExpression);
		}

		Assert.IsFalse(NpcArchiveReferencePolicy.HasReferenceOrUncertainty(typeof(DbArmourType),
			nameof(DbArmourType.Definition), definition, 10, 11));
		var chopping = armour.DissipateExpressions[DamageType.Chopping];
		foreach (var (damage, quality, expected) in new[] { (10.0, 6.0, 1.0), (10.0, 0.0, 10.0), (200.0, 6.0, 188.0) })
		{
			Assert.IsTrue(chopping.TryEvaluateDoubleWith(new Dictionary<string, object>
				{ ["damage"] = damage, ["quality"] = quality, ["strength"] = 115000.0 }, out var result, out var error), error);
			Assert.AreEqual(expected, result, 0.000001);
		}
	}
}

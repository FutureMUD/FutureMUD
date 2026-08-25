#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Body;
using MudSharp.Database;
using MudSharp.GameItems;
using MudSharp.Models;
using System;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class NonHumanCombatBalanceReconciliationTests
{
	private static FuturemudDatabaseContext BuildContext()
	{
		DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	[TestMethod]
	public void NaturalAttackReconciler_TwoPassesConvergeAndPreserveCustomLinks()
	{
		using FuturemudDatabaseContext context = BuildContext();
		BodyProto body = new()
		{
			Id = 1,
			Name = "Test Body",
			ConsiderString = string.Empty,
			WielderDescriptionSingle = "limb",
			WielderDescriptionPlural = "limbs",
			LegDescriptionSingular = "leg",
			LegDescriptionPlural = "legs"
		};
		BodypartShape shape = new() { Id = 1, Name = "Shoulder" };
		BodypartProto shoulder = new()
		{
			Id = 1,
			Body = body,
			BodyId = body.Id,
			Name = "shoulder",
			Description = "shoulder",
			BodypartShape = shape,
			BodypartShapeId = shape.Id,
			BodypartType = (int)BodypartTypeEnum.Wear,
			IsOrgan = 0
		};
		Race race = new()
		{
			Id = 1,
			Name = "Test Race",
			Description = string.Empty,
			BaseBody = body,
			BaseBodyId = body.Id,
			AllowedGenders = string.Empty,
			DiceExpression = "3d6",
			CommunicationStrategyType = "humanoid",
			HandednessOptions = string.Empty,
			MaximumDragWeightExpression = "0",
			MaximumLiftWeightExpression = "0",
			EatCorpseEmoteText = string.Empty,
			BreathingVolumeExpression = "0",
			HoldBreathLengthExpression = "0"
		};
		WeaponAttack stock = NewAttack(1, "Stock Attack", shape.Id);
		WeaponAttack custom = NewAttack(2, "Builder Clone", shape.Id);
		context.AddRange(body, shape, shoulder, race, stock, custom);
		context.RacesWeaponAttacks.Add(new RacesWeaponAttacks
		{
			Race = race,
			Bodypart = shoulder,
			WeaponAttack = stock,
			Quality = (int)ItemQuality.Terrible
		});
		context.RacesWeaponAttacks.Add(new RacesWeaponAttacks
		{
			Race = race,
			Bodypart = shoulder,
			WeaponAttack = custom,
			Quality = (int)ItemQuality.Legendary
		});
		context.SaveChanges();

		SeededNaturalAttackLink[] expected = [new(stock, ItemQuality.Great)];
		NonHumanNaturalAttackReconciler.Reconcile(context, race, expected, ["Stock Attack"]);
		context.SaveChanges();
		string firstFingerprint = string.Join('|', context.RacesWeaponAttacks
			.OrderBy(x => x.WeaponAttackId)
			.Select(x => $"{x.WeaponAttackId}:{x.BodypartId}:{x.Quality}"));

		NonHumanNaturalAttackReconciler.Reconcile(context, race, expected, ["Stock Attack"]);
		context.SaveChanges();
		string secondFingerprint = string.Join('|', context.RacesWeaponAttacks
			.OrderBy(x => x.WeaponAttackId)
			.Select(x => $"{x.WeaponAttackId}:{x.BodypartId}:{x.Quality}"));

		Assert.AreEqual(firstFingerprint, secondFingerprint);
		Assert.AreEqual((int)ItemQuality.Great,
			context.RacesWeaponAttacks.Single(x => x.WeaponAttackId == stock.Id).Quality);
		Assert.AreEqual((int)ItemQuality.Legendary,
			context.RacesWeaponAttacks.Single(x => x.WeaponAttackId == custom.Id).Quality,
			"Builder-created attack links must remain untouched.");
	}

	private static WeaponAttack NewAttack(long id, string name, long shapeId)
	{
		return new WeaponAttack
		{
			Id = id,
			Name = name,
			BodypartShapeId = shapeId,
			AdditionalInfo = string.Empty,
			RequiredPositionStateIds = string.Empty
		};
	}

	[TestMethod]
	public void BodypartClassification_BonesAreNeverExternalTargets()
	{
		foreach (BodypartTypeEnum type in new[]
		         {
			         BodypartTypeEnum.Bone,
			         BodypartTypeEnum.NonImmobilisingBone,
			         BodypartTypeEnum.MinorBone,
			         BodypartTypeEnum.MinorNonImobilisingBone
		         })
		{
			Assert.IsTrue(SeederBodyUtilities.IsBoneBodypart(new BodypartProto { BodypartType = (int)type }));
		}

		Assert.IsFalse(SeederBodyUtilities.IsBoneBodypart(new BodypartProto
		{
			BodypartType = (int)BodypartTypeEnum.Wear
		}));
	}
}

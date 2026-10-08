#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Xml.Linq;
using Moq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Work.Agriculture;
using Db = MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class NpcArchiveReferencePolicyTests
{
	[DataTestMethod]
	[DataRow("<Definition />")]
	[DataRow("<ArmourType><AbsorbExpressions><Expression damagetype='10'>damage-107</Expression></AbsorbExpressions></ArmourType>")]
	[DataRow("<ArmourType><AbsorbExpressions><Expression damagetype='10'>damage * if(rand(1,100)&lt;=min(80,max(15,(quality*10) + (strength/3500) - damage)),0.40,0.82)</Expression></AbsorbExpressions></ArmourType>")]
	[DataRow("<Definition><DissipateExpressions><Expression damagetype='10'><![CDATA[Max(0, damage-107)]]></Expression><Expression damagetype='11'>pain/209</Expression></DissipateExpressions><DamageTransformations><Transform fromtype='10' totype='11' severity='3' /></DamageTransformations></Definition>")]
	public void Reference_KnownArmourFormulaAndEnums_AreClear(string value) =>
		Assert.IsFalse(NpcArchiveReferencePolicy.HasReferenceOrUncertainty(typeof(Db.ArmourType), "Definition", value, 10, 11, 107, 209));

	[DataTestMethod]
	[DataRow(null)]
	[DataRow("")]
	[DataRow("<Definition")]
	[DataRow("<Armour />")]
	[DataRow("<Definition xmlns='future' />")]
	[DataRow("<Definition owner='999' />")]
	[DataRow("<Definition><Character>999</Character></Definition>")]
	[DataRow("<Definition>107</Definition>")]
	[DataRow("<Definition><DissipateExpressions/><DissipateExpressions/></Definition>")]
	[DataRow("<Definition><AbsorbExpressions><Expression>damage</Expression></AbsorbExpressions></Definition>")]
	[DataRow("<Definition><AbsorbExpressions><Expression damagetype='Claw'>damage</Expression></AbsorbExpressions></Definition>")]
	[DataRow("<Definition><AbsorbExpressions><Expression damagetype='999'>damage</Expression></AbsorbExpressions></Definition>")]
	[DataRow("<Definition><AbsorbExpressions><Expression damagetype='10' character='107'>damage</Expression></AbsorbExpressions></Definition>")]
	[DataRow("<Definition><AbsorbExpressions><Expression damagetype='10'><Character>107</Character></Expression></AbsorbExpressions></Definition>")]
	[DataRow("<Definition><AbsorbExpressions><Expression damagetype='10'>body+107</Expression></AbsorbExpressions></Definition>")]
	[DataRow("<Definition><AbsorbExpressions><Expression damagetype='10'>Character(107)</Expression></AbsorbExpressions></Definition>")]
	[DataRow("<Definition><AbsorbExpressions><Expression damagetype='10'>damage +</Expression></AbsorbExpressions></Definition>")]
	[DataRow("<ArmourType><DissipateExpressions><Expression damagetype='1'>max(damage*0.1,damage-(quality * 2 * strength/115000)))</Expression></DissipateExpressions></ArmourType>")]
	[DataRow("<Definition><AbsorbExpressions><Expression damagetype='10'>damage</Expression><Expression damagetype='010'>pain</Expression></AbsorbExpressions></Definition>")]
	[DataRow("<Definition><DamageTransformations><Transform fromtype='10' totype='11' /></DamageTransformations></Definition>")]
	[DataRow("<Definition><DamageTransformations><Transform fromtype='10' totype='11' severity='999' /></DamageTransformations></Definition>")]
	[DataRow("<Definition><DamageTransformations><Transform fromtype='10' totype='11' severity='3' /><Transform fromtype='010' totype='11' severity='3' /></DamageTransformations></Definition>")]
	public void Reference_ArmourUnknownOrMalformedContract_Holds(string? value) =>
		Assert.IsTrue(NpcArchiveReferencePolicy.HasReferenceOrUncertainty(typeof(Db.ArmourType), "Definition", value, 10, 11, 107));

	[TestMethod]
	public void Reference_ArmourRandomFormula_IsParsedWithoutConsumingRandomness()
	{
		var random = new Mock<System.Random>(MockBehavior.Strict);
		using var scope = ExpressionEngine.Expression.PushRandom(random.Object);
		Assert.IsFalse(NpcArchiveReferencePolicy.HasReferenceOrUncertainty(typeof(Db.ArmourType), "Definition",
			"<ArmourType><DissipateExpressions><Expression damagetype='10'>damage+rand(1,100)</Expression></DissipateExpressions></ArmourType>", 10, 11));
		random.VerifyNoOtherCalls();
	}

	[DataTestMethod]
	[DataRow("Definition")]
	[DataRow("ArmourType")]
	public void Reference_ProductionArmourCodec_WithNumericCollisions_RoundTripsAndIsClear(string root)
	{
		var runtime = new MudSharp.Combat.ArmourType(new Db.ArmourType
		{
			Definition = $"<{root}><AbsorbExpressions><Expression damagetype='10'>Max(0,damage-107)</Expression></AbsorbExpressions><DamageTransformations><Transform fromtype='10' totype='11' severity='3' /></DamageTransformations></{root}>"
		}, new Mock<IFuturemud>().Object);
		var xml = (XElement)runtime.GetType().GetMethod("SaveDefinition", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(runtime, null)!;
		var reloaded = new MudSharp.Combat.ArmourType(new Db.ArmourType { Definition = xml.ToString() }, new Mock<IFuturemud>().Object);
		Assert.AreEqual(runtime.AbsorbExpressions[MudSharp.Health.DamageType.Claw].OriginalExpression,
			reloaded.AbsorbExpressions[MudSharp.Health.DamageType.Claw].OriginalExpression);
		Assert.AreEqual(runtime.DamageTypeTransformations[MudSharp.Health.DamageType.Claw],
			reloaded.DamageTypeTransformations[MudSharp.Health.DamageType.Claw]);
		Assert.IsTrue(NpcArchiveReferencePolicy.HasReferenceOrUncertainty(xml.ToString(), 10, 11, 107));
		Assert.IsFalse(NpcArchiveReferencePolicy.HasReferenceOrUncertainty(typeof(Db.ArmourType), "Definition", xml.ToString(), 10, 11, 107));
		Assert.IsTrue(NpcArchiveReferencePolicy.HasReferenceOrUncertainty(typeof(Db.ArmourType), "Data", xml.ToString(), 10, 11, 107));
	}

	[DataTestMethod]
	[DataRow(typeof(Db.AgricultureFieldProfile), "<Profile uses='Crop; Pasture'><Score type='Moisture' value='107' /><Score type='custom 209' value='303' /></Profile>")]
	[DataRow(typeof(Db.AgricultureOperation), "<Operation woodlandYieldMultiplier='107' herdYieldCost='209'><AllowedUses uses='Crop, Pasture' /><Apiary installHives='303'><Outputs><Commodity material='material 107' weight='209' tag='tag 303' /></Outputs></Apiary><Score type='custom 107' value='209' /></Operation>")]
	[DataRow(typeof(Db.AgricultureCropDefinition), "<Crop growthDays='107' harvestCycleDays='209' perennial='1'><Pollination dependency='Required' healthBonus='1' yieldBonus='2' /><PlantingWindows><Window type='season' value='season 303' /><Window type='group' value='107' /></PlantingWindows><ScoreRanges><Score type='custom 209' min='107' max='303' /></ScoreRanges><Seeds><Commodity material='107' weight='209' tag='303' /></Seeds><Outputs><Commodity material='209' weight='303' /></Outputs></Crop>")]
	public void Reference_KnownAgricultureScalarsAndNames_DoNotReferencePhysicalIds(System.Type type, string value)
	{
		Assert.IsTrue(NpcArchiveReferencePolicy.HasReferenceOrUncertainty(value, 107, 209, 303));
		Assert.IsFalse(NpcArchiveReferencePolicy.HasReferenceOrUncertainty(type, "Definition", value, 107, 209, 303));
	}

	[DataTestMethod]
	[DataRow(typeof(Db.AgricultureOperation), "<Operation />")]
	[DataRow(typeof(Db.AgricultureFieldProfile), "<Profile />")]
	[DataRow(typeof(Db.AgricultureFieldProfile), null)]
	[DataRow(typeof(Db.AgricultureCropDefinition), "<Crop />")]
	[DataRow(typeof(Db.AgricultureOperation), " ")]
	[DataRow(typeof(Db.AgricultureCropDefinition), null)]
	public void Reference_KnownAgricultureLegacyDefaults_AreClear(System.Type type, string? value) =>
		Assert.IsFalse(NpcArchiveReferencePolicy.HasReferenceOrUncertainty(type, "Definition", value, 107, 209));

	[DataTestMethod]
	[DataRow(typeof(Db.AgricultureFieldProfile), "<Profile owner='999' />")]
	[DataRow(typeof(Db.AgricultureFieldProfile), "<Profile><Character>107</Character></Profile>")]
	[DataRow(typeof(Db.AgricultureFieldProfile), "<Profile><Score type='Moisture' value='bad' /></Profile>")]
	[DataRow(typeof(Db.AgricultureFieldProfile), "<Profile><Score type='Moisture' value='107' body='209' /></Profile>")]
	[DataRow(typeof(Db.AgricultureFieldProfile), "<Profile uses='99' />")]
	[DataRow(typeof(Db.AgricultureFieldProfile), "<Profile xmlns='future' />")]
	[DataRow(typeof(Db.AgricultureOperation), "<Operation owner='107' />")]
	[DataRow(typeof(Db.AgricultureOperation), "<Operation owner='999' />")]
	[DataRow(typeof(Db.AgricultureOperation), "<Operation><Target>209</Target></Operation>")]
	[DataRow(typeof(Db.AgricultureOperation), "<Operation><Apiary><Outputs><Commodity material='grain' weight='1' character='303' /></Outputs></Apiary></Operation>")]
	[DataRow(typeof(Db.AgricultureOperation), "<Operation><AllowedUses uses='Unknown' /></Operation>")]
	[DataRow(typeof(Db.AgricultureOperation), "<Operation><Apiary/><Apiary/></Operation>")]
	[DataRow(typeof(Db.AgricultureOperation), "<Operation xmlns='future' />")]
	[DataRow(typeof(Db.AgricultureOperation), "<Operation xmlns:r='future' r:owner='107' />")]
	[DataRow(typeof(Db.AgricultureOperation), "<Operation woodlandYieldMultiplier='NaN' />")]
	[DataRow(typeof(Db.AgricultureOperation), "<Operation woodlandYieldMultiplier='1e309' />")]
	[DataRow(typeof(Db.AgricultureOperation), "<Operation woodlandYieldCost='1.5' />")]
	[DataRow(typeof(Db.AgricultureOperation), "<Operation>107</Operation>")]
	[DataRow(typeof(Db.AgricultureOperation), "<Operation><Score type='Moisture'><Reference>107</Reference></Score></Operation>")]
	[DataRow(typeof(Db.AgricultureOperation), "<Operation><?reference 107?></Operation>")]
	[DataRow(typeof(Db.AgricultureOperation), "<Operation")]
	[DataRow(typeof(Db.AgricultureOperation), "{\"type\":\"Operation\"}")]
	[DataRow(typeof(Db.AgricultureCropDefinition), "<Operation />")]
	[DataRow(typeof(Db.AgricultureCropDefinition), "<Crop character='999' />")]
	[DataRow(typeof(Db.AgricultureCropDefinition), "<Crop><Pollination owner='107' /></Crop>")]
	[DataRow(typeof(Db.AgricultureCropDefinition), "<Crop><PlantingWindows><Window type='season' value='Spring' body='209' /></PlantingWindows></Crop>")]
	[DataRow(typeof(Db.AgricultureCropDefinition), "<Crop><PlantingWindows><Window type='99' value='Spring' /></PlantingWindows></Crop>")]
	[DataRow(typeof(Db.AgricultureCropDefinition), "<Crop><ScoreRanges><Score type='Moisture' min='bad' /></ScoreRanges></Crop>")]
	[DataRow(typeof(Db.AgricultureCropDefinition), "<Crop><Seeds><Commodity material='grain' weight='Infinity' /></Seeds></Crop>")]
	[DataRow(typeof(Db.AgricultureCropDefinition), "<Crop perennial='yes' />")]
	[DataRow(typeof(Db.AgricultureCropDefinition), "<Crop growthDays='2147483648' />")]
	[DataRow(typeof(Db.AgricultureCropDefinition), "<Crop><Outputs/><Outputs/></Crop>")]
	public void Reference_AgricultureUnknownOrMalformedContract_Holds(System.Type type, string value) =>
		Assert.IsTrue(NpcArchiveReferencePolicy.HasReferenceOrUncertainty(type, "Definition", value, 107, 209, 303));

	[TestMethod]
	public void Reference_ClassifierRequiresExactEntityAndProperty_OtherColumnsKeepGenericScan()
	{
		const string value = "<Operation herdYieldCost='107' />";
		Assert.IsTrue(NpcArchiveReferencePolicy.HasReferenceOrUncertainty(typeof(Db.AgricultureOperation), "Data", value, 107, 209));
		Assert.IsTrue(NpcArchiveReferencePolicy.HasReferenceOrUncertainty(typeof(Db.AgricultureField), "Definition", value, 107, 209));
		Assert.IsTrue(NpcArchiveReferencePolicy.HasReferenceOrUncertainty(typeof(Db.MagicSpell), "Definition", value, 107, 209));
		Assert.IsTrue(NpcArchiveReferencePolicy.HasReferenceOrUncertainty(typeof(Db.AgricultureCropDefinition), "Definition", value, 107, 209));
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void Reference_ProductionAgricultureSaveDefinition_WithNumericCollisions_IsClear(bool crop)
	{
		var world = new Mock<IFuturemud>();
		object runtime = crop
			? new AgricultureCropDefinition(new Db.AgricultureCropDefinition
			{
				Definition = "<Crop growthDays='107'><PlantingWindows><Window type='season' value='209' /></PlantingWindows><Outputs><Commodity material='303' weight='107' /></Outputs></Crop>"
			}, world.Object)
			: new AgricultureOperation(new Db.AgricultureOperation
			{
				Definition = "<Operation herdYieldMultiplier='107'><Score type='Moisture' value='209' /><Apiary installHives='303'><Outputs><Commodity material='107' weight='209' /></Outputs></Apiary></Operation>"
			}, world.Object);
		var xml = (XElement)runtime.GetType().GetMethod("SaveDefinition", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(runtime, null)!;
		Assert.IsTrue(NpcArchiveReferencePolicy.HasReferenceOrUncertainty(xml.ToString(), 107, 209, 303));
		Assert.IsFalse(NpcArchiveReferencePolicy.HasReferenceOrUncertainty(
			crop ? typeof(Db.AgricultureCropDefinition) : typeof(Db.AgricultureOperation), "Definition", xml.ToString(), 107, 209, 303));
	}

	[TestMethod]
	public void Reference_ProductionProfileSaveDefinition_WithNumericCollisions_IsClear()
	{
		var runtime = new AgricultureFieldProfile(new Db.AgricultureFieldProfile
		{
			Definition = "<Profile uses='Crop'><Score type='Moisture' value='10' /></Profile>"
		}, new Mock<IFuturemud>().Object);
		var xml = (XElement)runtime.GetType().GetMethod("SaveDefinition", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(runtime, null)!;
		Assert.IsTrue(NpcArchiveReferencePolicy.HasReferenceOrUncertainty(xml.ToString(), 10, 209));
		Assert.IsFalse(NpcArchiveReferencePolicy.HasReferenceOrUncertainty(typeof(Db.AgricultureFieldProfile),
			"Definition", xml.ToString(), 10, 209));
	}

	[DataTestMethod]
	[DataRow("<Effect><Target>107</Target></Effect>")]
	[DataRow("<Effect owner='107' />")]
	[DataRow("<Definition><OriginalBody>209</OriginalBody></Definition>")]
	[DataRow("{\"canonical\":107}")]
	[DataRow("{\"canonical\":\"\\u0031\\u0030\\u0037\"}")]
	[DataRow("{\"canonical\":1.07e2}")]
	[DataRow("{malformed")]
	[DataRow("[107]")]
	[DataRow("107,303,404")]
	[DataRow("<invalid")]
	public void Reference_IdentityBodyOrUncertainty_Holds(string value) =>
		Assert.IsTrue(NpcArchiveReferencePolicy.HasReferenceOrUncertainty(value, 107, 209));

	[DataTestMethod]
	[DataRow("")]
	[DataRow("<Effects />")]
	[DataRow("<Definition><Bodypart>9</Bodypart><Grade>3</Grade></Definition>")]
	[DataRow("1107 2090 10.7 2.09")]
	[DataRow("<Definition><A>10</A><B>7</B></Definition>")]
	public void Reference_NoMatchingTokens_IsClear(string value) =>
		Assert.IsFalse(NpcArchiveReferencePolicy.HasReferenceOrUncertainty(value, 107, 209));

	[TestMethod]
	public void Reference_InstanceAndWoundIds_AreRetained() =>
		Assert.IsTrue(NpcArchiveReferencePolicy.HasReferenceOrUncertainty("<Reference><Wound>303</Wound></Reference>", 107, 209, 303));

	[DataTestMethod]
	[DataRow("<Effects />", true)]
	[DataRow(" ", true)]
	[DataRow("<Effects><Effect /></Effects>", false)]
	[DataRow("<Effects value='something' />", false)]
	[DataRow("<invalid", false)]
	[DataRow("<Definition />", false)]
	public void Effects_OnlyProvenEmptyContainer_PermitsCompaction(string value, bool expected) =>
		Assert.AreEqual(expected, NpcArchiveReferencePolicy.IsEmptyEffects(value));

	[DataTestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void CharacterConstructor_ArchivedOrMissingBody_RefusesBeforeWorldCalls(bool archived)
	{
		var world = new Mock<IFuturemud>(MockBehavior.Strict);
		Assert.ThrowsException<System.InvalidOperationException>(() => new MudSharp.Character.Character(new Db.Character
		{
			Id = 107, IsArchived = archived, BodyId = archived ? 209 : null
		}, world.Object));
		world.VerifyNoOtherCalls();
	}
}

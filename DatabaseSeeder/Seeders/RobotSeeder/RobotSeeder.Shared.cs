#nullable enable

using MudSharp.Combat;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Knowledge;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System;

namespace DatabaseSeeder.Seeders;

public partial class RobotSeeder
{
private static readonly (string Name, string Description, string LongDescription, int Sessions, Difficulty Difficulty)[]
        RobotKnowledgeTemplates =
        [
            (
                "Robot Diagnostics",
                "Knowledge of diagnosing robotic faults and chassis damage",
                "This knowledge covers the inspection, diagnosis, and practical fault-finding techniques used on robotic patients.",
                15,
                Difficulty.Hard
            ),
            (
                "Robot Maintenance",
                "Knowledge of routine robotic maintenance and field repair",
                "This knowledge covers routine maintenance, chassis opening and closure, leak control, and practical repair work on robotic systems.",
                12,
                Difficulty.Normal
            ),
            (
                "Robot Surgery",
                "Knowledge of invasive robotic repair and component replacement",
                "This knowledge covers invasive robotic repair, organ replacement, and limb reattachment procedures.",
                18,
                Difficulty.VeryHard
            )
        ];

    private void SeedSharedRobotContent()
    {
        EnsureBodypartShape("Circular Saw");
        EnsureBodypartShape("Hammer Head");
        EnsureBodypartShape("Sword Blade");
        EnsureBodypartShape("Jet Pod");
        EnsureBodypartShape("Wheel");
        EnsureBodypartShape("Track");
        EnsureBodypartShape("Mandible");
        EnsureBodypartShape("Sensor Cluster");
        EnsureBodypartShape("Power Core");
        EnsureBodypartShape("Processor");
        EnsureBodypartShape("Chassis");

        _hydraulicResidue = EnsureMaterialClone(
            "Dried Hydraulic Residue",
            FindTemplateMaterial(MaterialBehaviourType.Grease),
            material =>
            {
                material.MaterialDescription = "a greasy blue-grey hydraulic residue";
                material.Organic = false;
                material.BehaviourType = (int)MaterialBehaviourType.Grease;
                material.ResidueSdesc = "hydraulic residue";
                material.ResidueDesc = "A smear of dried hydraulic residue clings to the surface.";
                material.ResidueColour = "blue";
            });
        _oilResidue = EnsureMaterialClone(
            "Dried Machine Oil Residue",
            FindTemplateMaterial(MaterialBehaviourType.Grease),
            material =>
            {
                material.MaterialDescription = "a dark, greasy machine oil residue";
                material.Organic = false;
                material.BehaviourType = (int)MaterialBehaviourType.Grease;
                material.ResidueSdesc = "oil residue";
                material.ResidueDesc = "A smear of dried machine oil darkens the surface.";
                material.ResidueColour = "black";
            });
        _chassisAlloy = EnsureMaterialClone(
            "Robot Chassis Alloy",
            FindTemplateMaterial(MaterialBehaviourType.Metal),
            material =>
            {
                material.MaterialDescription = "a reinforced industrial alloy used for robotic chassis plating";
                material.Organic = false;
                material.BehaviourType = (int)MaterialBehaviourType.Metal;
                material.ResidueSdesc = "metal shavings";
                material.ResidueDesc = "Tiny metallic shavings glitter here.";
                material.ResidueColour = "silver";
            });
        _circuitryMaterial = EnsureMaterialClone(
            "Robot Circuitry",
            FindTemplateMaterial(MaterialBehaviourType.Plastic),
            material =>
            {
                material.MaterialDescription = "a composite of circuit boards, wiring, and insulated robotic internals";
                material.Organic = false;
                material.BehaviourType = (int)MaterialBehaviourType.Plastic;
                material.ResidueSdesc = "burnt circuitry";
                material.ResidueDesc = "Fragments of burnt circuitry lie scattered here.";
                material.ResidueColour = "grey";
            });

        _hydraulicFluid = EnsureLiquidClone(
            "hydraulic fluid",
            _blood,
            liquid =>
            {
                liquid.Description = "a slick blue-grey hydraulic fluid";
                liquid.LongDescription = "a slick, pressurised blue-grey hydraulic fluid";
                liquid.TasteText = "It tastes bitter, metallic, and entirely industrial.";
                liquid.VagueTasteText = "It tastes bitter and metallic.";
                liquid.SmellText = "It smells of machine shops, hot seals, and solvent.";
                liquid.VagueSmellText = "It smells industrial and oily.";
                liquid.Organic = false;
                liquid.DisplayColour = "blue";
                liquid.DriedResidue = _hydraulicResidue;
                liquid.LeaveResidueInRooms = true;
                liquid.DampDescription = "It is coated in hydraulic fluid";
                liquid.WetDescription = "It is wet with hydraulic fluid";
                liquid.DrenchedDescription = "It is drenched in hydraulic fluid";
                liquid.DampShortDescription = "(hydraulic fluid)";
                liquid.WetShortDescription = "(wet with hydraulic fluid)";
                liquid.DrenchedShortDescription = "(drenched in hydraulic fluid)";
            });
        _machineOil = EnsureLiquidClone(
            "machine oil",
            _blood,
            liquid =>
            {
                liquid.Description = "a dark machine oil";
                liquid.LongDescription = "a dark, viscous machine oil";
                liquid.TasteText = "It tastes foul, greasy, and metallic.";
                liquid.VagueTasteText = "It tastes greasy and metallic.";
                liquid.SmellText = "It smells of old machinery and hot metal.";
                liquid.VagueSmellText = "It smells oily and metallic.";
                liquid.Organic = false;
                liquid.DisplayColour = "black";
                liquid.Viscosity = Math.Max(liquid.Viscosity, 1.5);
                liquid.DriedResidue = _oilResidue;
                liquid.LeaveResidueInRooms = true;
                liquid.DampDescription = "It is smeared with machine oil";
                liquid.WetDescription = "It is wet with machine oil";
                liquid.DrenchedDescription = "It is drenched in machine oil";
                liquid.DampShortDescription = "(machine oil)";
                liquid.WetShortDescription = "(wet with machine oil)";
                liquid.DrenchedShortDescription = "(drenched in machine oil)";
            });

		_robotFrameArmour = EnsureArmourDefinition(
			"Robot Frame Armour",
			BuildRobotFrameArmourDefinition());
		_robotPlatingArmour = EnsureArmourDefinition(
			"Robot Natural Armour",
			BuildRobotPlatingArmourDefinition(light: false));
		_robotLightPlatingArmour = EnsureArmourDefinition(
			"Robot Light Armour",
			BuildRobotPlatingArmourDefinition(light: true));
		_robotInternalArmour = EnsureArmourDefinition(
			"Robot Internal Armour",
			BuildRobotInternalArmourDefinition());

        _robotHumanoidCorpse = EnsureCorpseClone("Robot Humanoid Wreck", _organicHumanCorpse, "wrecked humanoid robot");
        _robotAnimalCorpse = EnsureCorpseClone("Robot Chassis Wreck", _organicAnimalCorpse, "wrecked robot");

        _robotStaminaRecoveryProg = EnsureFutureProg(
            "RobotStaminaRecovery",
            "Character",
            "Stamina",
            "Determines the stamina gain per 10 seconds for robotic characters",
            (long)ProgVariableTypes.Number,
            $"return max(10, GetTrait(@ch, ToTrait({_healthTrait.Id})) * 3)",
            ("ch", ProgVariableTypes.Toon));

        TraitExpression articulatedMaxHp = EnsureTraitExpression("Robot Max HP Formula", $"140+(con:{_healthTrait.Id}*2)");
        TraitExpression articulatedMaxStun = EnsureTraitExpression("Robot Max Stun Formula",
            $"200+((con:{_healthTrait.Id}+wil:{_secondaryTrait.Id})*2)");
        TraitExpression utilityMaxHp = EnsureTraitExpression("Robot Utility Max HP Formula", $"90+(con:{_healthTrait.Id})");

        _robotArticulatedStrategy = EnsureHealthStrategy(
            "Robot Articulated Model",
            "Robot",
            new XElement("Definition",
                new XElement("MaximumHitPointsExpression", articulatedMaxHp.Id),
                new XElement("MaximumStunExpression", articulatedMaxStun.Id),
                new XElement("PercentageHealthPerPenalty", 0.5),
                new XElement("PercentageStunPerPenalty", 0.35),
                new XElement("BleedMessageCooldown", 10),
                new XElement("PowerCoreCriticalThreshold", 0.35),
                new XElement("HydraulicFluidParalysisThreshold", 0.25)
            ).ToString());
        _robotUtilityStrategy = EnsureHealthStrategy(
            "Robot Utility Construct",
            "BrainConstruct",
            new XElement("Definition",
                new XElement("MaximumHitPointsExpression", utilityMaxHp.Id),
                new XElement("CheckPowerCore", true),
                new XElement("CheckHeart", false),
                new XElement("UseHypoxiaDamage", false),
                new XElement("CriticalInjuryThreshold", 0.85)
            ).ToString());

        EnsureAttackClone("Circular Saw Slash", "Claw High Swipe", "Circular Saw",
            "@ carve|carves into $1 with a whining circular saw.");
        EnsureAttackClone("Pneumatic Hammer Blow", "Animal Barge", "Hammer Head",
            "@ slam|slams a pneumatic hammer into $1.");
        EnsureAttackClone("Sword-Hand Lunge", "Claw High Swipe", "Sword Blade",
            "@ lunge|lunges at $1 with a sword-hand.");
        EnsureAttackClone("Wing Buffet", "Animal Barge", "Wing",
            "@ buffet|buffets $1 with an articulated wing.");
        EnsureAttackClone("Jet Ram", "Animal Barge", "Jet Pod",
            "@ ram|rams into $1 with a roaring jet pod.");
        EnsureAttackClone("Mandible Shear", "Mandible Bite", "Mandible",
            "@ shear|shears at $1 with snapping mandibles.");
        EnsureAttackClone("Wheel Ram", "Animal Barge", "Wheel",
            "@ ram|rams $1 with a drive wheel.");
        EnsureAttackClone("Track Grind", "Animal Barge", "Track",
            "@ grind|grinds into $1 with a track assembly.");
        EnsureAttackClone("Wheel Grind Close", "Bite", "Wheel",
            "@ grind|grinds a drive wheel into $1 at point-blank range.");
        EnsureAttackClone("Track Crush", "Bite", "Track",
            "@ crush|crushes into $1 with a churning track assembly.");
        EnsureAttackClone("Mandible Snap", "Claw High Swipe", "Mandible",
            "@ snap|snaps razor mandibles at $1.");

        _context.SaveChanges();
    }

    private BodypartShape EnsureBodypartShape(string name)
    {
        BodypartShape? existing = _context.BodypartShapes.FirstOrDefault(x => x.Name == name);
        if (existing is not null)
        {
            return existing;
        }

        BodypartShape shape = new() { Name = name };
        _context.BodypartShapes.Add(shape);
        _context.SaveChanges();
        return shape;
    }

    private Material EnsureMaterialClone(string name, Material template, Action<Material> configure)
    {
        Material? existing = _context.Materials.FirstOrDefault(x => x.Name == name);
        if (existing is not null)
        {
            return existing;
        }

        Material material = new()
        {
            Name = name,
            MaterialDescription = template.MaterialDescription,
            Density = template.Density,
            Organic = template.Organic,
            Type = template.Type,
            BehaviourType = template.BehaviourType,
            ThermalConductivity = template.ThermalConductivity,
            ElectricalConductivity = template.ElectricalConductivity,
            SpecificHeatCapacity = template.SpecificHeatCapacity,
            LiquidFormId = template.LiquidFormId,
            Viscosity = template.Viscosity,
            MeltingPoint = template.MeltingPoint,
            BoilingPoint = template.BoilingPoint,
            IgnitionPoint = template.IgnitionPoint,
            HeatDamagePoint = template.HeatDamagePoint,
            ImpactFracture = template.ImpactFracture,
            ImpactYield = template.ImpactYield,
            ImpactStrainAtYield = template.ImpactStrainAtYield,
            ShearFracture = template.ShearFracture,
            ShearYield = template.ShearYield,
            ShearStrainAtYield = template.ShearStrainAtYield,
            YoungsModulus = template.YoungsModulus,
            SolventId = template.SolventId,
            SolventVolumeRatio = template.SolventVolumeRatio,
            ResidueSdesc = template.ResidueSdesc,
            ResidueDesc = template.ResidueDesc,
            ResidueColour = template.ResidueColour,
            Absorbency = template.Absorbency
        };
        configure(material);
        _context.Materials.Add(material);
        _context.SaveChanges();
        return material;
    }

    private Liquid EnsureLiquidClone(string name, Liquid template, Action<Liquid> configure)
    {
        Liquid? existing = _context.Liquids.FirstOrDefault(x => x.Name == name);
        if (existing is not null)
        {
            return existing;
        }

        Liquid liquid = new()
        {
            Name = name,
            Description = template.Description,
            LongDescription = template.LongDescription,
            TasteText = template.TasteText,
            VagueTasteText = template.VagueTasteText,
            SmellText = template.SmellText,
            VagueSmellText = template.VagueSmellText,
            TasteIntensity = template.TasteIntensity,
            SmellIntensity = template.SmellIntensity,
            AlcoholLitresPerLitre = template.AlcoholLitresPerLitre,
            WaterLitresPerLitre = template.WaterLitresPerLitre,
            FoodSatiatedHoursPerLitre = template.FoodSatiatedHoursPerLitre,
            DrinkSatiatedHoursPerLitre = template.DrinkSatiatedHoursPerLitre,
            Viscosity = template.Viscosity,
            Density = template.Density,
            Organic = template.Organic,
            ThermalConductivity = template.ThermalConductivity,
            ElectricalConductivity = template.ElectricalConductivity,
            SpecificHeatCapacity = template.SpecificHeatCapacity,
            IgnitionPoint = template.IgnitionPoint,
            FreezingPoint = template.FreezingPoint,
            BoilingPoint = template.BoilingPoint,
            DraughtProgId = template.DraughtProgId,
            SolventId = template.SolventId,
            CountAsId = template.CountAsId,
            CountAsQuality = template.CountAsQuality,
            DisplayColour = template.DisplayColour,
            DampDescription = template.DampDescription,
            WetDescription = template.WetDescription,
            DrenchedDescription = template.DrenchedDescription,
            DampShortDescription = template.DampShortDescription,
            WetShortDescription = template.WetShortDescription,
            DrenchedShortDescription = template.DrenchedShortDescription,
            SolventVolumeRatio = template.SolventVolumeRatio,
            DriedResidueId = template.DriedResidueId,
            DrugId = template.DrugId,
            DrugGramsPerUnitVolume = template.DrugGramsPerUnitVolume,
            InjectionConsequence = template.InjectionConsequence,
            ResidueVolumePercentage = template.ResidueVolumePercentage,
            RelativeEnthalpy = template.RelativeEnthalpy,
            GasFormId = template.GasFormId,
            LeaveResidueInRooms = template.LeaveResidueInRooms
        };
        configure(liquid);
        _context.Liquids.Add(liquid);
        _context.SaveChanges();
        return liquid;
    }

private static string BuildNaturalArmourDefinition(
		System.Collections.Generic.IEnumerable<(DamageType From, DamageType To, WoundSeverity Threshold)> transforms,
		Func<DamageType, string> damageDissipate,
		Func<DamageType, string> painDissipate,
		Func<DamageType, string> stunDissipate,
		Func<DamageType, string> damageAbsorb,
		Func<DamageType, string> painAbsorb,
		Func<DamageType, string> stunAbsorb)
	{
		static XElement BuildExpressionSet(string name, Func<DamageType, string> factory)
		{
			return new XElement(name,
				Enum.GetValues(typeof(DamageType))
					.OfType<DamageType>()
					.Select(type => new XElement("Expression",
						new XAttribute("damagetype", (int)type),
						factory(type))));
		}

		var root = new XElement("ArmourType",
			new XElement("DamageTransformations",
				transforms.Select(x => new XElement("Transform",
					new XAttribute("fromtype", (int)x.From),
					new XAttribute("totype", (int)x.To),
					new XAttribute("severity", (int)x.Threshold)))),
			BuildExpressionSet("DissipateExpressions", damageDissipate),
			BuildExpressionSet("DissipateExpressionsPain", painDissipate),
			BuildExpressionSet("DissipateExpressionsStun", stunDissipate),
			BuildExpressionSet("AbsorbExpressions", damageAbsorb),
			BuildExpressionSet("AbsorbExpressionsPain", painAbsorb),
			BuildExpressionSet("AbsorbExpressionsStun", stunAbsorb)
		);

		return root.ToString(SaveOptions.DisableFormatting);
	}

	private static System.Collections.Generic.IEnumerable<(DamageType From, DamageType To, WoundSeverity Threshold)>
		RobotFrameTransforms()
	{
		yield return (DamageType.Slashing, DamageType.Crushing, WoundSeverity.Small);
		yield return (DamageType.Chopping, DamageType.Crushing, WoundSeverity.Small);
		yield return (DamageType.Piercing, DamageType.Crushing, WoundSeverity.Superficial);
		yield return (DamageType.Ballistic, DamageType.Crushing, WoundSeverity.Superficial);
		yield return (DamageType.Bite, DamageType.Crushing, WoundSeverity.Small);
		yield return (DamageType.Claw, DamageType.Crushing, WoundSeverity.Small);
		yield return (DamageType.Shearing, DamageType.Crushing, WoundSeverity.Small);
		yield return (DamageType.Wrenching, DamageType.Crushing, WoundSeverity.Small);
		yield return (DamageType.Shrapnel, DamageType.Crushing, WoundSeverity.Superficial);
		yield return (DamageType.ArmourPiercing, DamageType.ArmourPiercing, WoundSeverity.Horrifying);
	}

	private static System.Collections.Generic.IEnumerable<(DamageType From, DamageType To, WoundSeverity Threshold)>
		RobotPlatingTransforms()
	{
		yield return (DamageType.Slashing, DamageType.Crushing, WoundSeverity.VerySevere);
		yield return (DamageType.Chopping, DamageType.Crushing, WoundSeverity.VerySevere);
		yield return (DamageType.Piercing, DamageType.Crushing, WoundSeverity.Moderate);
		yield return (DamageType.Ballistic, DamageType.Crushing, WoundSeverity.Moderate);
		yield return (DamageType.Bite, DamageType.Crushing, WoundSeverity.Severe);
		yield return (DamageType.Claw, DamageType.Crushing, WoundSeverity.VerySevere);
		yield return (DamageType.Shearing, DamageType.Crushing, WoundSeverity.VerySevere);
		yield return (DamageType.Wrenching, DamageType.Crushing, WoundSeverity.Severe);
		yield return (DamageType.Shrapnel, DamageType.Crushing, WoundSeverity.Moderate);
		yield return (DamageType.ArmourPiercing, DamageType.ArmourPiercing, WoundSeverity.Horrifying);
	}

	private static bool IsRobotCutLikeDamage(DamageType damageType)
	{
		return damageType is DamageType.Slashing or
			DamageType.Chopping or
			DamageType.Piercing or
			DamageType.Ballistic or
			DamageType.Bite or
			DamageType.Claw or
			DamageType.Shearing or
			DamageType.BallisticArmourPiercing or
			DamageType.ArmourPiercing or
			DamageType.Shrapnel;
	}

	private static bool IsRobotImpactLikeDamage(DamageType damageType)
	{
		return damageType is DamageType.Crushing or
			DamageType.Shockwave or
			DamageType.Sonic or
			DamageType.Wrenching or
			DamageType.Falling;
	}

	private static string RobotNaturalDissipateExpression(DamageType damageType, string valueName, double cutFactor,
		double impactFactor, double defaultFactor)
	{
		if (IsRobotCutLikeDamage(damageType))
		{
			return $"{valueName} - (quality * strength/25000 * {cutFactor.ToString(System.Globalization.CultureInfo.InvariantCulture)})";
		}

		if (IsRobotImpactLikeDamage(damageType))
		{
			return $"{valueName} - (quality * strength/10000 * {impactFactor.ToString(System.Globalization.CultureInfo.InvariantCulture)})";
		}

		return $"{valueName} - (quality * {defaultFactor.ToString(System.Globalization.CultureInfo.InvariantCulture)})";
	}

	private static string RobotFrameDamageAbsorbExpression(DamageType damageType, string valueName)
	{
		return damageType switch
		{
			DamageType.Hypoxia or DamageType.Cellular => "0",
			DamageType.Slashing or
			DamageType.Chopping or
			DamageType.Piercing or
			DamageType.Ballistic or
			DamageType.Bite or
			DamageType.Claw or
			DamageType.Shearing or
			DamageType.BallisticArmourPiercing or
			DamageType.ArmourPiercing or
			DamageType.Shrapnel => $"{valueName}*0.95",
			DamageType.Crushing or
			DamageType.Shockwave or
			DamageType.Sonic or
			DamageType.Wrenching or
			DamageType.Falling => $"{valueName}*0.9",
			_ => $"{valueName}*0.8"
		};
	}

	private static string RobotPlatingDamageAbsorbExpression(DamageType damageType, string valueName, bool light)
	{
		return damageType switch
		{
			DamageType.Hypoxia or DamageType.Cellular => "0",
			DamageType.Slashing or
			DamageType.Chopping or
			DamageType.Claw or
			DamageType.Shearing => light ? $"{valueName}*0.84" : $"{valueName}*0.76",
			DamageType.Piercing or
			DamageType.Ballistic or
			DamageType.BallisticArmourPiercing or
			DamageType.ArmourPiercing or
			DamageType.Shrapnel => light ? $"{valueName}*0.9" : $"{valueName}*0.84",
			DamageType.Crushing or
			DamageType.Shockwave or
			DamageType.Sonic or
			DamageType.Wrenching or
			DamageType.Falling => light ? $"{valueName}*0.92" : $"{valueName}*0.88",
			DamageType.Electrical => light ? $"{valueName}*0.7" : $"{valueName}*0.6",
			_ => light ? $"{valueName}*0.82" : $"{valueName}*0.75"
		};
	}

	private static string RobotInternalDamageAbsorbExpression(DamageType damageType, string valueName)
	{
		return damageType switch
		{
			DamageType.Hypoxia or DamageType.Cellular => "0",
			DamageType.Electrical => $"{valueName}*0.65",
			DamageType.Burning or DamageType.Freezing or DamageType.Chemical => $"{valueName}*0.75",
			_ => $"{valueName}*0.85"
		};
	}

	private static string RobotNaturalStunAbsorbExpression(DamageType damageType, string valueName)
	{
		return damageType switch
		{
			DamageType.Hypoxia or DamageType.Cellular => "0",
			_ => valueName
		};
	}

	private ArmourType EnsureArmourDefinition(string name, string definition)
	{
		ArmourType? existing = _context.ArmourTypes.FirstOrDefault(x => x.Name == name);
		if (existing is not null)
		{
			existing.Definition = definition;
			return existing;
		}

		ArmourType armour = new()
		{
			Name = name,
			MinimumPenetrationDegree = 1,
			BaseDifficultyDegrees = 0,
			StackedDifficultyDegrees = 0,
			Definition = definition
		};
		_context.ArmourTypes.Add(armour);
		_context.SaveChanges();
		return armour;
	}

    private CorpseModel EnsureCorpseClone(string name, CorpseModel template, string replacementSubject)
    {
        CorpseModel? existing = _context.CorpseModels.FirstOrDefault(x => x.Name == name);
        if (existing is not null)
        {
            return existing;
        }

        CorpseModel corpse = new()
        {
            Name = name,
            Type = template.Type,
            Description = $"a {replacementSubject}",
            Definition = template.Definition
                .Replace("corpse", "wreck", StringComparison.OrdinalIgnoreCase)
                .Replace("flesh", "plating", StringComparison.OrdinalIgnoreCase)
                .Replace("skeletal remains", "component remains", StringComparison.OrdinalIgnoreCase)
                .Replace("skin", "panelling", StringComparison.OrdinalIgnoreCase)
        };
        _context.CorpseModels.Add(corpse);
        _context.SaveChanges();
        return corpse;
    }

    private FutureProg EnsureFutureProg(string functionName, string category, string subcategory, string comment,
        long returnType, string functionText, params (string Name, ProgVariableTypes Type)[] parameters)
    {
        FutureProg? existing = _context.FutureProgs.FirstOrDefault(x => x.FunctionName == functionName);
        if (existing is not null)
        {
            return existing;
        }

        FutureProg prog = new()
        {
            FunctionName = functionName,
            Category = category,
            Subcategory = subcategory,
            FunctionComment = comment,
            ReturnType = returnType,
            AcceptsAnyParameters = false,
            Public = true,
            StaticType = 0,
            FunctionText = functionText
        };
        for (int i = 0; i < parameters.Length; i++)
        {
            prog.FutureProgsParameters.Add(new FutureProgsParameter
            {
                FutureProg = prog,
                ParameterIndex = i,
                ParameterName = parameters[i].Name,
                ParameterTypeDefinition = parameters[i].Type.ToStorageString()
            });
        }

        _context.FutureProgs.Add(prog);
        _context.SaveChanges();
        return prog;
    }

    private TraitExpression EnsureTraitExpression(string name, string expression)
    {
        TraitExpression? existing = _context.TraitExpressions.FirstOrDefault(x => x.Name == name);
        if (existing is not null)
        {
            return existing;
        }

        TraitExpression traitExpression = new()
        {
            Name = name,
            Expression = expression
        };
        _context.TraitExpressions.Add(traitExpression);
        _context.SaveChanges();
        return traitExpression;
    }

    private HealthStrategy EnsureHealthStrategy(string name, string type, string definition)
    {
        HealthStrategy? existing = _context.HealthStrategies.FirstOrDefault(x => x.Name == name);
        if (existing is not null)
        {
            return existing;
        }

        HealthStrategy strategy = new()
        {
            Name = name,
            Type = type,
            Definition = definition
        };
        _context.HealthStrategies.Add(strategy);
        _context.SaveChanges();
        return strategy;
    }

    private WeaponAttack EnsureAttackClone(string name, string donorName, string shapeName, string message)
    {
        WeaponAttack? existing = _context.WeaponAttacks.FirstOrDefault(x => x.Name == name);
        if (existing is not null)
        {
            return existing;
        }

        WeaponAttack donor = _context.WeaponAttacks.First(x => x.Name == donorName);
        BodypartShape shape = _context.BodypartShapes.First(x => x.Name == shapeName);
        WeaponAttack attack = new()
        {
            Name = name,
            WeaponTypeId = donor.WeaponTypeId,
            Verb = donor.Verb,
            FutureProgId = donor.FutureProgId,
            BaseAttackerDifficulty = donor.BaseAttackerDifficulty,
            BaseBlockDifficulty = donor.BaseBlockDifficulty,
            BaseDodgeDifficulty = donor.BaseDodgeDifficulty,
            BaseParryDifficulty = donor.BaseParryDifficulty,
            BaseAngleOfIncidence = donor.BaseAngleOfIncidence,
            RecoveryDifficultySuccess = donor.RecoveryDifficultySuccess,
            RecoveryDifficultyFailure = donor.RecoveryDifficultyFailure,
            MoveType = donor.MoveType,
            Intentions = donor.Intentions,
            ExertionLevel = donor.ExertionLevel,
            DamageType = donor.DamageType,
            DamageExpressionId = donor.DamageExpressionId,
            StunExpressionId = donor.StunExpressionId,
            PainExpressionId = donor.PainExpressionId,
            Weighting = donor.Weighting,
            BodypartShapeId = shape.Id,
            StaminaCost = donor.StaminaCost,
            BaseDelay = donor.BaseDelay,
            Orientation = donor.Orientation,
            Alignment = donor.Alignment,
            AdditionalInfo = donor.AdditionalInfo,
            HandednessOptions = donor.HandednessOptions,
            RequiredPositionStateIds = donor.RequiredPositionStateIds
        };
        _context.WeaponAttacks.Add(attack);
        _context.SaveChanges();

        CombatMessage combatMessage = new()
        {
            Type = donor.MoveType,
            Message = message,
            Priority = 50,
            Verb = donor.Verb,
            Chance = 1.0,
            FailureMessage = message
        };
        combatMessage.CombatMessagesWeaponAttacks.Add(new CombatMessagesWeaponAttacks
        {
            CombatMessage = combatMessage,
            WeaponAttack = attack
        });
        _context.CombatMessages.Add(combatMessage);
        _context.SaveChanges();
        return attack;
    }

    private Material FindTemplateMaterial(MudSharp.Form.Material.MaterialBehaviourType behaviour)
    {
        return _context.Materials.FirstOrDefault(x => x.BehaviourType == (int)behaviour) ??
               _context.Materials.First();
    }
}

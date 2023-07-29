using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class HumanSeeder
{
	private readonly Dictionary<string, BodypartProto> _bodyparts = new(StringComparer.OrdinalIgnoreCase);
	private readonly Dictionary<string, BodypartProto> _bones = new(StringComparer.OrdinalIgnoreCase);

	private readonly List<(BodypartProto Child, BodypartProto Parent)> _cachedBodypartUpstreams = new();

	private readonly CollectionDictionary<string, BodypartProto> _cachedLimbs = new(StringComparer.OrdinalIgnoreCase);
	private readonly Dictionary<string, BodypartProto> _organs = new(StringComparer.OrdinalIgnoreCase);

	private readonly Dictionary<string, BodypartShape> _shapes = new(StringComparer.OrdinalIgnoreCase);
	private ArmourType _boneArmour;

	private Material _compactBone;
	private ArmourType _naturalArmour;
	private ArmourType _organArmour;

	private Material _spongyBone;
	private Material _visceraMaterial;

	public void CreateBodypart(BodyProto body, string alias, string name, string shape, BodypartTypeEnum type,
		string? upstreamPartName, Alignment alignment, Orientation orientation, int hitPoints, int severThreshold,
		int hitChance, int displayOrder, Material material, SizeCategory size, string limb, bool isSignificant = true,
		double infectability = 1.0, bool isVital = false, double hypoxia = 0.0, double implantSpace = 0,
		double implantSpaceOccupied = 0, bool isCore = true, double bleedMultiplier = 1.0,
		double damageMultiplier = 1.0, double painMultiplier = 1.0, double stunMultiplier = 0.0)
	{
		var bodypart = new BodypartProto
		{
			BodypartShape = _shapes[shape],
			Body = body,
			Name = alias,
			Description = name,
			BodypartType = (int)type,
			Alignment = (int)alignment,
			Location = (int)orientation,
			BleedModifier = bleedMultiplier,
			DamageModifier = damageMultiplier,
			PainModifier = painMultiplier,
			StunModifier = stunMultiplier,
			MaxLife = hitPoints,
			SeveredThreshold = _questionAnswers["sever"].EqualToAny("y", "yes") ? severThreshold : -1,
			IsCore = isCore,
			IsVital = isVital,
			Significant = isSignificant,
			RelativeInfectability = infectability,
			HypoxiaDamagePerTick = hypoxia,
			ImplantSpace = implantSpace,
			ImplantSpaceOccupied = implantSpaceOccupied,
			Size = (int)size,
			DisplayOrder = displayOrder,
			RelativeHitChance = hitChance,
			DefaultMaterial = material,
			ArmourType = _naturalArmour
		};

		if (type == BodypartTypeEnum.Grabbing)
		{
			bodypart.Unary = false;
		}
		else if (type == BodypartTypeEnum.Wielding)
		{
			bodypart.Unary = true;
			bodypart.MaxSingleSize = (int)SizeCategory.Normal;
		}
		else if (type == BodypartTypeEnum.GrabbingWielding)
		{
			bodypart.Unary = true;
			bodypart.MaxSingleSize = (int)SizeCategory.Normal;
		}

		_context.BodypartProtos.Add(bodypart);
		_bodyparts[alias] = bodypart;
		_cachedLimbs.Add(limb, bodypart);
		if (!string.IsNullOrEmpty(upstreamPartName))
			_cachedBodypartUpstreams.Add((bodypart, _bodyparts[upstreamPartName]));
	}

	public void AddOrgan(string alias, string description, BodypartTypeEnum type, double implantSpaceOccupied,
		int hitPoints, double bleedModifier, double infectionModifier, double hypoxiaDamage,
		BodyProto body,
		double damageModifier = 1.0, double stunModifier = 0.0, double painModifier = 1.0)
	{
		var organ = new BodypartProto
		{
			Name = alias,
			Description = description,
			Body = body,
			BodypartType = (int)type,
			IsCore = true,
			IsOrgan = 1,
			IsVital = true,
			MaxLife = hitPoints,
			SeveredThreshold = -1,
			DisplayOrder = 1,
			BleedModifier = bleedModifier,
			DamageModifier = damageModifier,
			PainModifier = painModifier,
			StunModifier = stunModifier,
			HypoxiaDamagePerTick = hypoxiaDamage,
			RelativeInfectability = infectionModifier,
			Size = (int)SizeCategory.Small,
			Location = (int)Orientation.Irrelevant,
			Alignment = (int)Alignment.Irrelevant,
			BodypartShape = _shapes["organ"],
			RelativeHitChance = 0,
			DefaultMaterial = _visceraMaterial,
			ImplantSpaceOccupied = implantSpaceOccupied,
			ArmourType = _organArmour
		};
		_context.BodypartProtos.Add(organ);
		_organs[alias] = organ;
	}

	private void SetupBodyparts(BodyProto baseHumanoid, BodyProto organicHumanoid)
	{
		#region Natural Armour Types

		var naturalArmour = new ArmourType
		{
			Name = "Human Natural Armour",
			MinimumPenetrationDegree = 1,
			BaseDifficultyDegrees = 0,
			StackedDifficultyDegrees = 0,
			Definition = @"<ArmourType>

	<!-- Damage Transformations change damage passed on to bones/organs/items into a different damage type when severity is under a certain  threshold 
		
		Damage Types:
		
		Slashing = 0
        Chopping = 1
        Crushing = 2
        Piercing = 3
        Ballistic = 4
        Burning = 5
        Freezing = 6
        Chemical = 7
        Shockwave = 8
        Bite = 9
        Claw = 10
        Electrical = 11
        Hypoxia = 12
        Cellular = 13
        Sonic = 14
        Shearing = 15
        ArmourPiercing = 16
        Wrenching = 17
        Shrapnel = 18
        Necrotic = 19
        Falling = 20
        Eldritch = 21
        Arcane = 22
		
		Severity Values:
		
		None = 0
        Superficial = 1
        Minor = 2
        Small = 3
        Moderate = 4
        Severe = 5
        VerySevere = 6
        Grievous = 7
        Horrifying = 8
	-->
	<DamageTransformations>
 		<Transform fromtype=""0"" totype=""2"" severity=""5""></Transform> <!-- Slashing to Crushing when <= Severe -->
 		<Transform fromtype=""1"" totype=""2"" severity=""5""></Transform> <!-- Chopping to Crushing when <= Severe -->
 		<Transform fromtype=""3"" totype=""2"" severity=""4""></Transform> <!-- Piercing to Crushing when <= Moderate -->
 		<Transform fromtype=""4"" totype=""2"" severity=""4""></Transform> <!-- Ballistic to Crushing when <= Moderate -->
 		<Transform fromtype=""9"" totype=""2"" severity=""5""></Transform> <!-- Bite to Crushing when <= Severe -->
 		<Transform fromtype=""10"" totype=""2"" severity=""5""></Transform> <!-- Claw to Crushing when <= Severe -->
 		<Transform fromtype=""15"" totype=""2"" severity=""5""></Transform> <!-- Shearing to Crushing when <= Severe -->
 		<Transform fromtype=""16"" totype=""2"" severity=""3""></Transform> <!-- ArmourPiercing to Crushing when <= Small -->
 		<Transform fromtype=""17"" totype=""2"" severity=""5""></Transform> <!-- Wrenching to Crushing when <= Severe -->
 	</DamageTransformations>
    <!-- 
	
	    Dissipate expressions are applied before the item/part takes damage. 
		If they reduce the damage to zero, it neither suffers nor passes on any damage. 
		
		Parameters: 
		* damage, pain or stun (as appropriate) = the raw damage/pain/stun suffered
		* quality = the quality of the armour, rated 0 (Abysmal) to 11 (Legendary)
		* angle = the angle in radians of the attack (e.g. 1.5708rad = 90 degrees)
		* density = the density in kg/m3 of the material that the armour is made from
		* electrical = the electrical conductivity of the material that the armour is made from (1/ohm metres)
		* thermal = the thermal conductivity of the material that the armour is made from (watts per meter3 per kelvin)
		* organic = if the material that the armour is made from is organic (1 for true, 0 for false)
		* strength = either ImpactYield or ShearYield of the armour material depending on the damage type, in Pascals.
		
		Hint: 25000 can be considered ""base"" ShearYield and 10000 can be considered ""base"" ImpactYield
	-->
    <DissipateExpressions>
        <Expression damagetype=""0"">damage - (quality * strength/25000 * 0.75)</Expression>    <!-- Slashing -->
 		<Expression damagetype=""1"">damage - (quality * strength/25000 * 0.75)</Expression>    <!-- Chopping -->  
 		<Expression damagetype=""2"">damage - (quality * strength/10000 * 0.75)</Expression>    <!-- Crushing -->  
 		<Expression damagetype=""3"">damage - (quality * strength/25000 * 0.75)</Expression>    <!-- Piercing -->  
 		<Expression damagetype=""4"">damage - (quality * strength/25000 * 0.75)</Expression>    <!-- Ballistic -->  
 		<Expression damagetype=""5"">damage - (quality * 0.75)</Expression>    			      <!-- Burning -->
 		<Expression damagetype=""6"">damage - (quality * 0.75)</Expression>                     <!-- Freezing -->
 		<Expression damagetype=""7"">damage - (quality * 0.75)</Expression>                     <!-- Chemical -->
 		<Expression damagetype=""8"">damage - (quality * strength/10000 * 0.75)</Expression>    <!-- Shockwave -->
 		<Expression damagetype=""9"">damage - (quality * strength/25000 * 0.75)</Expression>    <!-- Bite -->
 		<Expression damagetype=""10"">damage - (quality * strength/25000 * 0.75)</Expression>   <!-- Claw -->
 		<Expression damagetype=""11"">damage - (quality * 0.75)</Expression>                    <!-- Electrical -->
 		<Expression damagetype=""12"">damage - (quality * 0.75)</Expression>                    <!-- Hypoxia -->
 		<Expression damagetype=""13"">damage - (quality * 0.75)</Expression>                    <!-- Cellular -->
		<Expression damagetype=""14"">damage - (quality * strength/10000 * 0.75)</Expression>   <!-- Sonic -->
 		<Expression damagetype=""15"">damage - (quality * strength/25000 * 0.75)</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">damage - (quality * strength/25000 * 0.75)</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">damage - (quality * strength/10000 * 0.75)</Expression>   <!-- Wrenching -->
 		<Expression damagetype=""18"">damage - (quality * strength/25000 * 0.75)</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">damage - (quality * 0.75)</Expression>                    <!-- Necrotic -->   
 		<Expression damagetype=""20"">damage - (quality * strength/10000 * 0.75)</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">damage - (quality * 0.75)</Expression>                    <!-- Eldritch -->   
		<Expression damagetype=""22"">damage - (quality * 0.75)</Expression>                    <!-- Arcane -->   
 	</DissipateExpressions>  
 	<DissipateExpressionsPain>
        <Expression damagetype=""0"">pain - (quality * strength/25000 * 0.75)</Expression>    <!-- Slashing -->
 		<Expression damagetype=""1"">pain - (quality * strength/25000 * 0.75)</Expression>    <!-- Chopping -->  
 		<Expression damagetype=""2"">pain - (quality * strength/10000 * 0.75)</Expression>    <!-- Crushing -->  
 		<Expression damagetype=""3"">pain - (quality * strength/25000 * 0.75)</Expression>    <!-- Piercing -->  
 		<Expression damagetype=""4"">pain - (quality * strength/25000 * 0.75)</Expression>    <!-- Ballistic -->  
 		<Expression damagetype=""5"">pain - (quality * 0.75)</Expression>    			        <!-- Burning -->
 		<Expression damagetype=""6"">pain - (quality * 0.75)</Expression>                     <!-- Freezing -->
 		<Expression damagetype=""7"">pain - (quality * 0.75)</Expression>                     <!-- Chemical -->
 		<Expression damagetype=""8"">pain - (quality * strength/10000 * 0.75)</Expression>    <!-- Shockwave -->
 		<Expression damagetype=""9"">pain - (quality * strength/25000 * 0.75)</Expression>    <!-- Bite -->
 		<Expression damagetype=""10"">pain - (quality * strength/25000 * 0.75)</Expression>   <!-- Claw -->
 		<Expression damagetype=""11"">pain - (quality * 0.75)</Expression>                    <!-- Electrical -->
 		<Expression damagetype=""12"">pain - (quality * 0.75)</Expression>                    <!-- Hypoxia -->
 		<Expression damagetype=""13"">pain - (quality * 0.75)</Expression>                    <!-- Cellular -->
		<Expression damagetype=""14"">pain - (quality * strength/10000 * 0.75)</Expression>   <!-- Sonic -->
 		<Expression damagetype=""15"">pain - (quality * strength/25000 * 0.75)</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">pain - (quality * strength/25000 * 0.75)</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">pain - (quality * strength/10000 * 0.75)</Expression>   <!-- Wrenching -->
 		<Expression damagetype=""18"">pain - (quality * strength/25000 * 0.75)</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">pain - (quality * 0.75)</Expression>                    <!-- Necrotic -->   
 		<Expression damagetype=""20"">pain - (quality * strength/10000 * 0.75)</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">pain - (quality * 0.75)</Expression>                    <!-- Eldritch -->   
		<Expression damagetype=""22"">pain - (quality * 0.75)</Expression>                    <!-- Arcane -->   
 	</DissipateExpressionsPain>  
 	<DissipateExpressionsStun>
        <Expression damagetype=""0"">stun - (quality * strength/25000 * 0.75)</Expression>    <!-- Slashing -->
 		<Expression damagetype=""1"">stun - (quality * strength/25000 * 0.75)</Expression>    <!-- Chopping -->  
 		<Expression damagetype=""2"">stun - (quality * strength/10000 * 0.75)</Expression>    <!-- Crushing -->  
 		<Expression damagetype=""3"">stun - (quality * strength/25000 * 0.75)</Expression>    <!-- Piercing -->  
 		<Expression damagetype=""4"">stun - (quality * strength/25000 * 0.75)</Expression>    <!-- Ballistic -->  
 		<Expression damagetype=""5"">stun - (quality * 0.75)</Expression>    			        <!-- Burning -->
 		<Expression damagetype=""6"">stun - (quality * 0.75)</Expression>                     <!-- Freezing -->
 		<Expression damagetype=""7"">stun - (quality * 0.75)</Expression>                     <!-- Chemical -->
 		<Expression damagetype=""8"">stun - (quality * strength/10000 * 0.75)</Expression>    <!-- Shockwave -->
 		<Expression damagetype=""9"">stun - (quality * strength/25000 * 0.75)</Expression>    <!-- Bite -->
 		<Expression damagetype=""10"">stun - (quality * strength/25000 * 0.75)</Expression>   <!-- Claw -->
 		<Expression damagetype=""11"">stun - (quality * 0.75)</Expression>                    <!-- Electrical -->
 		<Expression damagetype=""12"">stun - (quality * 0.75)</Expression>                    <!-- Hypoxia -->
 		<Expression damagetype=""13"">stun - (quality * 0.75)</Expression>                    <!-- Cellular -->
		<Expression damagetype=""14"">stun - (quality * strength/10000 * 0.75)</Expression>   <!-- Sonic -->
 		<Expression damagetype=""15"">stun - (quality * strength/25000 * 0.75)</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">stun - (quality * strength/25000 * 0.75)</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">stun - (quality * strength/10000 * 0.75)</Expression>   <!-- Wrenching -->
 		<Expression damagetype=""18"">stun - (quality * strength/25000 * 0.75)</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">stun - (quality * 0.75)</Expression>                    <!-- Necrotic -->   
 		<Expression damagetype=""20"">stun - (quality * strength/10000 * 0.75)</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">stun - (quality * 0.75)</Expression>                    <!-- Eldritch -->   
		<Expression damagetype=""22"">stun - (quality * 0.75)</Expression>                    <!-- Arcane -->   
 	</DissipateExpressionsStun>  
	<!-- 
	
	    Absorb expressions are applied after dissipate expressions and item/part damage. 
	    The after-absorb values are what is passed on to anything ""below"" e.g. bones, organs, parts worn under armour, etc 
		
	    Parameters: 
		* damage, pain or stun (as appropriate) = the residual damage/pain/stun after dissipate step
		* quality = the quality of the armour, rated 0 (Abysmal) to 11 (Legendary)
		* angle = the angle in radians of the attack (e.g. 1.5708rad = 90 degrees)
		* density = the density in kg/m3 of the material that the armour is made from
		* electrical = the electrical conductivity of the material that the armour is made from (1/ohm metres)
		* thermal = the thermal conductivity of the material that the armour is made from (watts per meter3 per kelvin)
		* organic = if the material that the armour is made from is organic (1 for true, 0 for false)
		* strength = either ImpactYield or ShearYield of the armour material depending on the damage type, in Pascals.
		
		Hint: 25000 can be considered ""base"" ShearYield and 10000 can be considered ""base"" ImpactYield
		
		-->
 	<AbsorbExpressions>
	 	<Expression damagetype=""0"">damage*0.8</Expression>    <!-- Slashing -->
 		<Expression damagetype=""1"">damage*0.8</Expression>    <!-- Chopping -->  
 		<Expression damagetype=""2"">damage*0.8</Expression>    <!-- Crushing -->  
 		<Expression damagetype=""3"">damage*0.8</Expression>    <!-- Piercing -->  
 		<Expression damagetype=""4"">damage*0.8</Expression>    <!-- Ballistic -->  
 		<Expression damagetype=""5"">damage*0.8</Expression>    <!-- Burning -->
 		<Expression damagetype=""6"">damage*0.8</Expression>    <!-- Freezing -->
 		<Expression damagetype=""7"">damage*0.8</Expression>    <!-- Chemical -->
 		<Expression damagetype=""8"">damage*0.8</Expression>    <!-- Shockwave -->
 		<Expression damagetype=""9"">damage*0.8</Expression>    <!-- Bite -->
 		<Expression damagetype=""10"">damage*0.8</Expression>   <!-- Claw -->
 		<Expression damagetype=""11"">damage*0.8</Expression>   <!-- Electrical -->
 		<Expression damagetype=""12"">0</Expression>        <!-- Hypoxia -->
 		<Expression damagetype=""13"">0</Expression>        <!-- Cellular -->
		<Expression damagetype=""14"">damage*0.8</Expression>   <!-- Sonic -->
 		<Expression damagetype=""15"">damage*0.8</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">damage*0.8</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">damage*0.8</Expression>   <!-- Wrenching -->
 		<Expression damagetype=""18"">damage*0.8</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">damage*0.8</Expression>   <!-- Necrotic -->   
 		<Expression damagetype=""20"">damage*0.8</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">damage*0.8</Expression>   <!-- Eldritch -->   
		<Expression damagetype=""22"">damage*0.8</Expression>   <!-- Arcane -->   
 	</AbsorbExpressions>  
 	<AbsorbExpressionsPain>
        <Expression damagetype=""0"">pain*0.8</Expression>    <!-- Slashing -->
 		<Expression damagetype=""1"">pain*0.8</Expression>    <!-- Chopping -->  
 		<Expression damagetype=""2"">pain*0.8</Expression>    <!-- Crushing -->  
 		<Expression damagetype=""3"">pain*0.8</Expression>    <!-- Piercing -->  
 		<Expression damagetype=""4"">pain*0.8</Expression>    <!-- Ballistic -->  
 		<Expression damagetype=""5"">pain*0.8</Expression>    <!-- Burning -->
 		<Expression damagetype=""6"">pain*0.8</Expression>    <!-- Freezing -->
 		<Expression damagetype=""7"">pain*0.8</Expression>    <!-- Chemical -->
 		<Expression damagetype=""8"">pain*0.8</Expression>    <!-- Shockwave -->
 		<Expression damagetype=""9"">pain*0.8</Expression>    <!-- Bite -->
 		<Expression damagetype=""10"">pain*0.8</Expression>   <!-- Claw -->
 		<Expression damagetype=""11"">pain*0.8</Expression>   <!-- Electrical -->
 		<Expression damagetype=""12"">0</Expression>        <!-- Hypoxia -->
 		<Expression damagetype=""13"">0</Expression>        <!-- Cellular -->
		<Expression damagetype=""14"">pain*0.8</Expression>   <!-- Sonic -->
 		<Expression damagetype=""15"">pain*0.8</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">pain*0.8</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">pain*0.8</Expression>   <!-- Wrenching -->
 		<Expression damagetype=""18"">pain*0.8</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">pain*0.8</Expression>   <!-- Necrotic -->   
 		<Expression damagetype=""20"">pain*0.8</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">pain*0.8</Expression>   <!-- Eldritch -->   
		<Expression damagetype=""22"">pain*0.8</Expression>   <!-- Arcane -->   
 	</AbsorbExpressionsPain>  
 	<AbsorbExpressionsStun>
        <Expression damagetype=""0"">stun</Expression>    <!-- Slashing -->
 		<Expression damagetype=""1"">stun</Expression>    <!-- Chopping -->  
 		<Expression damagetype=""2"">stun</Expression>    <!-- Crushing -->  
 		<Expression damagetype=""3"">stun</Expression>    <!-- Piercing -->  
 		<Expression damagetype=""4"">stun</Expression>    <!-- Ballistic -->  
 		<Expression damagetype=""5"">stun</Expression>    <!-- Burning -->
 		<Expression damagetype=""6"">stun</Expression>    <!-- Freezing -->
 		<Expression damagetype=""7"">stun</Expression>    <!-- Chemical -->
 		<Expression damagetype=""8"">stun</Expression>    <!-- Shockwave -->
 		<Expression damagetype=""9"">stun</Expression>    <!-- Bite -->
 		<Expression damagetype=""10"">stun</Expression>   <!-- Claw -->
 		<Expression damagetype=""11"">stun</Expression>   <!-- Electrical -->
 		<Expression damagetype=""12"">0</Expression>        <!-- Hypoxia -->
 		<Expression damagetype=""13"">0</Expression>        <!-- Cellular -->
		<Expression damagetype=""14"">stun</Expression>   <!-- Sonic -->
 		<Expression damagetype=""15"">stun</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">stun</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">stun</Expression>   <!-- Wrenching -->
 		<Expression damagetype=""18"">stun</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">stun</Expression>   <!-- Necrotic -->   
 		<Expression damagetype=""20"">stun</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">stun</Expression>   <!-- Eldritch -->   
		<Expression damagetype=""22"">stun</Expression>   <!-- Arcane -->   
 	</AbsorbExpressionsStun>
 </ArmourType>"
		};
		_context.ArmourTypes.Add(naturalArmour);
		_naturalArmour = naturalArmour;

		_organArmour = new ArmourType
		{
			Name = "Human Natural Organ Armour",
			MinimumPenetrationDegree = 1,
			BaseDifficultyDegrees = 0,
			StackedDifficultyDegrees = 0,
			Definition = @"<ArmourType>

    <!-- 
	
	    Dissipate expressions are applied before the item/part takes damage. 
		If they reduce the damage to zero, it neither suffers nor passes on any damage. 
		
		Parameters: 
		* damage, pain or stun (as appropriate) = the raw damage/pain/stun suffered
		* quality = the quality of the armour, rated 0 (Abysmal) to 11 (Legendary)
		* angle = the angle in radians of the attack (e.g. 1.5708rad = 90 degrees)
		* density = the density in kg/m3 of the material that the armour is made from
		* electrical = the electrical conductivity of the material that the armour is made from (1/ohm metres)
		* thermal = the thermal conductivity of the material that the armour is made from (watts per meter3 per kelvin)
		* organic = if the material that the armour is made from is organic (1 for true, 0 for false)
		* strength = either ImpactYield or ShearYield of the armour material depending on the damage type, in Pascals.
		
		Hint: 25000 can be considered ""base"" ShearYield and 10000 can be considered ""base"" ImpactYield
	-->
    <DissipateExpressions>
        <Expression damagetype=""0"">damage-(1.0*quality)</Expression>         <!-- Slashing -->
 		<Expression damagetype=""1"">damage-(1.0*quality)</Expression>         <!-- Chopping -->  
 		<Expression damagetype=""2"">damage-(1.0*quality)</Expression>         <!-- Crushing -->  
 		<Expression damagetype=""3"">damage-(1.0*quality)</Expression>         <!-- Piercing -->  
 		<Expression damagetype=""4"">damage*1.15-(1.0*quality)</Expression>    <!-- Ballistic -->  
 		<Expression damagetype=""5"">damage-(1.0*quality)</Expression>    	 <!-- Burning -->
 		<Expression damagetype=""6"">damage-(1.0*quality)</Expression>         <!-- Freezing -->
 		<Expression damagetype=""7"">damage-(1.0*quality)</Expression>         <!-- Chemical -->
 		<Expression damagetype=""8"">damage*1.15-(1.0*quality)</Expression>    <!-- Shockwave -->
 		<Expression damagetype=""9"">damage-(1.0*quality)</Expression>         <!-- Bite -->
 		<Expression damagetype=""10"">damage-(1.0*quality)</Expression>        <!-- Claw -->
 		<Expression damagetype=""11"">damage-(1.0*quality)</Expression>        <!-- Electrical -->
 		<Expression damagetype=""12"">damage-(quality*0.75)</Expression>       <!-- Hypoxia -->
 		<Expression damagetype=""13"">damage-(quality*0.75)</Expression>       <!-- Cellular -->
		<Expression damagetype=""14"">damage-(1.0*quality)</Expression>        <!-- Sonic -->
 		<Expression damagetype=""15"">damage-(1.0*quality)</Expression>        <!-- Shearing --> 
		<Expression damagetype=""16"">damage-(1.0*quality)</Expression>        <!-- ArmourPiercing -->
		<Expression damagetype=""17"">damage-(1.0*quality)</Expression>        <!-- Wrenching -->
 		<Expression damagetype=""18"">damage*1.15-(1.0*quality)</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">damage-(1.0*quality)</Expression>        <!-- Necrotic -->   
 		<Expression damagetype=""20"">damage-(1.0*quality)</Expression>        <!-- Falling -->   
		<Expression damagetype=""21"">damage-(1.0*quality)</Expression>        <!-- Eldritch -->   
		<Expression damagetype=""22"">damage-(1.0*quality)</Expression>        <!-- Arcane -->   
 	</DissipateExpressions>  
 	<DissipateExpressionsPain>
        <Expression damagetype=""0"">pain-(1.0*quality)</Expression>         <!-- Slashing -->
 		<Expression damagetype=""1"">pain-(1.0*quality)</Expression>         <!-- Chopping -->  
 		<Expression damagetype=""2"">pain-(1.0*quality)</Expression>         <!-- Crushing -->  
 		<Expression damagetype=""3"">pain-(1.0*quality)</Expression>         <!-- Piercing -->  
 		<Expression damagetype=""4"">pain*1.15-(1.0*quality)</Expression>    <!-- Ballistic -->  
 		<Expression damagetype=""5"">pain-(1.0*quality)</Expression>    	 <!-- Burning -->
 		<Expression damagetype=""6"">pain-(1.0*quality)</Expression>         <!-- Freezing -->
 		<Expression damagetype=""7"">pain-(1.0*quality)</Expression>         <!-- Chemical -->
 		<Expression damagetype=""8"">pain*1.15-(1.0*quality)</Expression>    <!-- Shockwave -->
 		<Expression damagetype=""9"">pain-(1.0*quality)</Expression>         <!-- Bite -->
 		<Expression damagetype=""10"">pain-(1.0*quality)</Expression>        <!-- Claw -->
 		<Expression damagetype=""11"">pain-(1.0*quality)</Expression>        <!-- Electrical -->
 		<Expression damagetype=""12"">pain-(quality*0.75)</Expression>       <!-- Hypoxia -->
 		<Expression damagetype=""13"">pain-(quality*0.75)</Expression>       <!-- Cellular -->
		<Expression damagetype=""14"">pain-(1.0*quality)</Expression>        <!-- Sonic -->
 		<Expression damagetype=""15"">pain-(1.0*quality)</Expression>        <!-- Shearing --> 
		<Expression damagetype=""16"">pain-(1.0*quality)</Expression>        <!-- ArmourPiercing -->
		<Expression damagetype=""17"">pain-(1.0*quality)</Expression>        <!-- Wrenching -->
 		<Expression damagetype=""18"">pain*1.15-(1.0*quality)</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">pain-(1.0*quality)</Expression>        <!-- Necrotic -->   
 		<Expression damagetype=""20"">pain-(1.0*quality)</Expression>        <!-- Falling -->   
		<Expression damagetype=""21"">pain-(1.0*quality)</Expression>        <!-- Eldritch -->   
		<Expression damagetype=""22"">pain-(1.0*quality)</Expression>        <!-- Arcane -->   
 	</DissipateExpressionsPain>  
 	<DissipateExpressionsStun>
        <Expression damagetype=""0"">stun-(1.0*quality)</Expression>         <!-- Slashing -->
 		<Expression damagetype=""1"">stun-(1.0*quality)</Expression>         <!-- Chopping -->  
 		<Expression damagetype=""2"">stun-(1.0*quality)</Expression>         <!-- Crushing -->  
 		<Expression damagetype=""3"">stun-(1.0*quality)</Expression>         <!-- Piercing -->  
 		<Expression damagetype=""4"">stun*1.15-(1.0*quality)</Expression>    <!-- Ballistic -->  
 		<Expression damagetype=""5"">stun-(1.0*quality)</Expression>    	 <!-- Burning -->
 		<Expression damagetype=""6"">stun-(1.0*quality)</Expression>         <!-- Freezing -->
 		<Expression damagetype=""7"">stun-(1.0*quality)</Expression>         <!-- Chemical -->
 		<Expression damagetype=""8"">stun*1.15-(1.0*quality)</Expression>    <!-- Shockwave -->
 		<Expression damagetype=""9"">stun-(1.0*quality)</Expression>         <!-- Bite -->
 		<Expression damagetype=""10"">stun-(1.0*quality)</Expression>        <!-- Claw -->
 		<Expression damagetype=""11"">stun-(1.0*quality)</Expression>        <!-- Electrical -->
 		<Expression damagetype=""12"">stun-(quality*0.75)</Expression>       <!-- Hypoxia -->
 		<Expression damagetype=""13"">stun-(quality*0.75)</Expression>       <!-- Cellular -->
		<Expression damagetype=""14"">stun-(1.0*quality)</Expression>        <!-- Sonic -->
 		<Expression damagetype=""15"">stun-(1.0*quality)</Expression>        <!-- Shearing --> 
		<Expression damagetype=""16"">stun-(1.0*quality)</Expression>        <!-- ArmourPiercing -->
		<Expression damagetype=""17"">stun-(1.0*quality)</Expression>        <!-- Wrenching -->
 		<Expression damagetype=""18"">stun*1.15-(1.0*quality)</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">stun-(1.0*quality)</Expression>        <!-- Necrotic -->   
 		<Expression damagetype=""20"">stun-(1.0*quality)</Expression>        <!-- Falling -->   
		<Expression damagetype=""21"">stun-(1.0*quality)</Expression>        <!-- Eldritch -->   
		<Expression damagetype=""22"">stun-(1.0*quality)</Expression>        <!-- Arcane -->    
 	</DissipateExpressionsStun>  
	
	<!-- Note: Organ Damage is final - there's no ""lower layer"" to pass on to, therefore there is no need for Absorb expressions -->
 </ArmourType>"
		};
		_context.ArmourTypes.Add(_organArmour);
		_context.SaveChanges();

		#endregion

		#region Bodypart Shapes

		var nextId = _context.BodypartShapes.Select(x => x.Id).AsEnumerable().DefaultIfEmpty(0).Max() + 1;

		void AddShape(string name)
		{
			var shape = new BodypartShape { Id = nextId++, Name = name };
			_shapes[name] = shape;
			_context.BodypartShapes.Add(shape);
		}

		AddShape("Organ");
		AddShape("Bone");
		AddShape("Eye");
		AddShape("Eye Socket");
		AddShape("Ear");
		AddShape("Inner Ear");
		AddShape("Face");
		AddShape("Nose");
		AddShape("Mouth");
		AddShape("Cheek");
		AddShape("Eyebrow");
		AddShape("Forehead");
		AddShape("Chin");
		AddShape("Neck Back");
		AddShape("Scalp");
		AddShape("Head Back");
		AddShape("Temple");
		AddShape("Neck");
		AddShape("Throat");
		AddShape("Tongue");
		AddShape("Shoulder");
		AddShape("Breast");
		AddShape("Shoulder Blade");
		AddShape("Upper Back");
		AddShape("Lower Back");
		AddShape("Nipple");
		AddShape("Abdomen");
		AddShape("Belly");
		AddShape("Groin");
		AddShape("Testicles");
		AddShape("Penis");
		AddShape("Buttock");
		AddShape("Hip");
		AddShape("Thigh");
		AddShape("Thigh Back");
		AddShape("Knee");
		AddShape("Knee Back");
		AddShape("Shin");
		AddShape("Calf");
		AddShape("Ankle");
		AddShape("Heel");
		AddShape("Foot");
		AddShape("Toe");
		AddShape("Upper Arm");
		AddShape("Elbow");
		AddShape("Forearm");
		AddShape("Wrist");
		AddShape("Hand");
		AddShape("Finger");
		AddShape("Thumb");
		if (!_questionAnswers["inventory"].EqualTo("hands")) AddShape("Inventory");

		_context.SaveChanges();

		#endregion

		#region Materials

		// Fleshy materials
		var fattyFlesh = new Material
		{
			Name = "fatty flesh",
			MaterialDescription = "fatty flesh",
			Density = 500,
			Organic = true,
			Type = 0,
			BehaviourType = (int)MaterialBehaviourType.Flesh,
			ThermalConductivity = 0.14,
			ElectricalConductivity = 0.0001,
			SpecificHeatCapacity = 420,
			IgnitionPoint = 555.3722,
			HeatDamagePoint = 412.0389,
			ImpactFracture = 10000,
			ImpactYield = 10000,
			ImpactStrainAtYield = 50000,
			ShearFracture = 25000,
			ShearYield = 25000,
			ShearStrainAtYield = 50000,
			YoungsModulus = 2,
			Absorbency = 0
		};
		_context.Materials.Add(fattyFlesh);

		var flesh = new Material
		{
			Name = "flesh",
			MaterialDescription = "flesh",
			Density = 500,
			Organic = true,
			Type = 0,
			BehaviourType = (int)MaterialBehaviourType.Flesh,
			ThermalConductivity = 0.14,
			ElectricalConductivity = 0.0001,
			SpecificHeatCapacity = 420,
			IgnitionPoint = 555.3722,
			HeatDamagePoint = 412.0389,
			ImpactFracture = 10000,
			ImpactYield = 10000,
			ImpactStrainAtYield = 50000,
			ShearFracture = 25000,
			ShearYield = 25000,
			ShearStrainAtYield = 50000,
			YoungsModulus = 2,
			Absorbency = 0
		};
		_context.Materials.Add(flesh);

		var musclyFlesh = new Material
		{
			Name = "muscly flesh",
			MaterialDescription = "muscly flesh",
			Density = 500,
			Organic = true,
			Type = 0,
			BehaviourType = (int)MaterialBehaviourType.Flesh,
			ThermalConductivity = 0.14,
			ElectricalConductivity = 0.0001,
			SpecificHeatCapacity = 420,
			IgnitionPoint = 555.3722,
			HeatDamagePoint = 412.0389,
			ImpactFracture = 10000,
			ImpactYield = 10000,
			ImpactStrainAtYield = 50000,
			ShearFracture = 25000,
			ShearYield = 25000,
			ShearStrainAtYield = 50000,
			YoungsModulus = 2,
			Absorbency = 0
		};
		_context.Materials.Add(musclyFlesh);

		var bonyFlesh = new Material
		{
			Name = "bony flesh",
			MaterialDescription = "bony flesh",
			Density = 500,
			Organic = true,
			Type = 0,
			BehaviourType = (int)MaterialBehaviourType.Flesh,
			ThermalConductivity = 0.14,
			ElectricalConductivity = 0.0001,
			SpecificHeatCapacity = 420,
			IgnitionPoint = 555.3722,
			HeatDamagePoint = 412.0389,
			ImpactFracture = 60000,
			ImpactYield = 60000,
			ImpactStrainAtYield = 50000,
			ShearFracture = 50000,
			ShearYield = 50000,
			ShearStrainAtYield = 50000,
			YoungsModulus = 20,
			Absorbency = 0
		};
		_context.Materials.Add(bonyFlesh);

		var denseBonyFlesh = new Material
		{
			Name = "dense bony flesh",
			MaterialDescription = "dense bony flesh",
			Density = 500,
			Organic = true,
			Type = 0,
			BehaviourType = (int)MaterialBehaviourType.Flesh,
			ThermalConductivity = 0.14,
			ElectricalConductivity = 0.0001,
			SpecificHeatCapacity = 420,
			IgnitionPoint = 555.3722,
			HeatDamagePoint = 412.0389,
			ImpactFracture = 140000,
			ImpactYield = 140000,
			ImpactStrainAtYield = 50000,
			ShearFracture = 90000,
			ShearYield = 90000,
			ShearStrainAtYield = 50000,
			YoungsModulus = 20,
			Absorbency = 0
		};
		_context.Materials.Add(denseBonyFlesh);

		_visceraMaterial = new Material
		{
			Name = "viscera",
			MaterialDescription = "viscera",
			Density = 500,
			Organic = true,
			Type = 0,
			BehaviourType = (int)MaterialBehaviourType.Flesh,
			ThermalConductivity = 0.14,
			ElectricalConductivity = 0.0001,
			SpecificHeatCapacity = 420,
			IgnitionPoint = 555.3722,
			HeatDamagePoint = 412.0389,
			ImpactFracture = 10000,
			ImpactYield = 10000,
			ImpactStrainAtYield = 50000,
			ShearFracture = 25000,
			ShearYield = 25000,
			ShearStrainAtYield = 50000,
			YoungsModulus = 2,
			Absorbency = 0
		};
		_context.Materials.Add(_visceraMaterial);

		_compactBone = new Material
		{
			Name = "compact bone",
			MaterialDescription = "compact bone",
			Density = 500,
			Organic = true,
			Type = 0,
			BehaviourType = (int)MaterialBehaviourType.Bone,
			ThermalConductivity = 0.14,
			ElectricalConductivity = 0.0001,
			SpecificHeatCapacity = 420,
			IgnitionPoint = 555.3722,
			HeatDamagePoint = 412.0389,
			ImpactFracture = 200000,
			ImpactYield = 200000,
			ImpactStrainAtYield = 50000,
			ShearFracture = 115000,
			ShearYield = 115000,
			ShearStrainAtYield = 50000,
			YoungsModulus = 18,
			Absorbency = 0
		};
		_context.Materials.Add(_compactBone);

		_spongyBone = new Material
		{
			Name = "spongy bone",
			MaterialDescription = "spongy bone",
			Density = 500,
			Organic = true,
			Type = 0,
			BehaviourType = (int)MaterialBehaviourType.Bone,
			ThermalConductivity = 0.14,
			ElectricalConductivity = 0.0001,
			SpecificHeatCapacity = 420,
			IgnitionPoint = 555.3722,
			HeatDamagePoint = 412.0389,
			ImpactFracture = 200000,
			ImpactYield = 200000,
			ImpactStrainAtYield = 50000,
			ShearFracture = 115000,
			ShearYield = 115000,
			ShearStrainAtYield = 50000,
			YoungsModulus = 18,
			Absorbency = 0
		};
		_context.Materials.Add(_spongyBone);

		_context.SaveChanges();

		#endregion

		#region Bodyparts

		var sever = _questionAnswers.ContainsKey("sever") &&
		            _questionAnswers["sever"].ToLowerInvariant().In("yes", "y");
		nextId = _context.BodypartProtos.Select(x => x.Id).AsEnumerable().DefaultIfEmpty(0).Max() + 1;

		// TORSO
		CreateBodypart(baseHumanoid, "abdomen", "abdomen", "abdomen", BodypartTypeEnum.Drapeable, null, Alignment.Front,
			Orientation.Centre, 80, -1, 100, 50, flesh, SizeCategory.Normal, "Torso", isVital: true, implantSpace: 5,
			stunMultiplier: 0.2);
		CreateBodypart(baseHumanoid, "rbreast", "right breast", "breast", BodypartTypeEnum.Drapeable, "abdomen",
			Alignment.FrontRight, Orientation.High, 80, -1, 50, 50, flesh, SizeCategory.Normal, "Torso", isVital: true,
			implantSpace: 5, stunMultiplier: 0.2);
		CreateBodypart(baseHumanoid, "lbreast", "left breast", "breast", BodypartTypeEnum.Drapeable, "abdomen",
			Alignment.FrontLeft, Orientation.High, 80, -1, 50, 50, flesh, SizeCategory.Normal, "Torso", isVital: true,
			implantSpace: 5, stunMultiplier: 0.2);
		CreateBodypart(baseHumanoid, "rnipple", "right nipple", "nipple", BodypartTypeEnum.Drapeable, "rbreast",
			Alignment.FrontRight, Orientation.High, 80, -1, 5, 50, flesh, SizeCategory.VerySmall, "Torso",
			false, isVital: false, implantSpace: 0, stunMultiplier: 0.2);
		CreateBodypart(baseHumanoid, "lnipple", "left nipple", "nipple", BodypartTypeEnum.Drapeable, "lbreast",
			Alignment.FrontLeft, Orientation.High, 80, -1, 5, 50, flesh, SizeCategory.VerySmall, "Torso",
			false, isVital: false, implantSpace: 0, stunMultiplier: 0.2);
		CreateBodypart(baseHumanoid, "uback", "upper back", "upper back", BodypartTypeEnum.Drapeable, "abdomen",
			Alignment.Rear, Orientation.Centre, 80, -1, 100, 51, fattyFlesh, SizeCategory.Normal, "Torso",
			isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		CreateBodypart(baseHumanoid, "belly", "belly", "belly", BodypartTypeEnum.Drapeable, "abdomen", Alignment.Front,
			Orientation.Centre, 80, -1, 100, 51, fattyFlesh, SizeCategory.Normal, "Torso", isVital: true,
			implantSpace: 5, stunMultiplier: 0.2);
		CreateBodypart(baseHumanoid, "lback", "lower back", "lower back", BodypartTypeEnum.Drapeable, "belly",
			Alignment.Rear, Orientation.Centre, 80, -1, 100, 51, fattyFlesh, SizeCategory.Normal, "Torso",
			isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		CreateBodypart(baseHumanoid, "rbuttock", "right buttock", "buttock", BodypartTypeEnum.Drapeable, "lback",
			Alignment.RearRight, Orientation.Centre, 40, -1, 50, 52, flesh, SizeCategory.Normal, "Torso",
			isVital: false, implantSpace: 5, stunMultiplier: 0.2);
		CreateBodypart(baseHumanoid, "lbuttock", "left buttock", "buttock", BodypartTypeEnum.Drapeable, "lback",
			Alignment.RearLeft, Orientation.Centre, 40, -1, 50, 52, flesh, SizeCategory.Normal, "Torso", isVital: false,
			implantSpace: 5, stunMultiplier: 0.2);
		CreateBodypart(baseHumanoid, "rshoulder", "right shoulder", "shoulder", BodypartTypeEnum.Drapeable, "rbreast",
			Alignment.FrontRight, Orientation.High, 80, -1, 50, 50, flesh, SizeCategory.Normal, "Torso", isVital: false,
			implantSpace: 5, stunMultiplier: 0.2);
		CreateBodypart(baseHumanoid, "lshoulder", "left shoulder", "shoulder", BodypartTypeEnum.Drapeable, "lbreast",
			Alignment.FrontLeft, Orientation.High, 80, -1, 50, 50, flesh, SizeCategory.Normal, "Torso", isVital: false,
			implantSpace: 5, stunMultiplier: 0.2);
		CreateBodypart(baseHumanoid, "rshoulderblade", "right shoulder blade", "shoulder blade",
			BodypartTypeEnum.Drapeable, "rshoulder", Alignment.RearRight, Orientation.High, 80, -1, 50, 50, flesh,
			SizeCategory.Normal, "Torso", isVital: false, implantSpace: 5, stunMultiplier: 0.2);
		CreateBodypart(baseHumanoid, "lshoulderblade", "left shoulder blade", "shoulder blade",
			BodypartTypeEnum.Drapeable, "lshoulder", Alignment.RearLeft, Orientation.High, 80, -1, 50, 50, flesh,
			SizeCategory.Normal, "Torso", isVital: false, implantSpace: 5, stunMultiplier: 0.2);
		if (!_questionAnswers["inventory"].EqualTo("hands"))
			CreateBodypart(baseHumanoid, "inventory", "inventory", "inventory", BodypartTypeEnum.Grabbing, null,
				Alignment.Front, Orientation.Centre, 80, -1, 0, 50, flesh, SizeCategory.Normal, "Torso", isVital: true,
				implantSpace: 5, stunMultiplier: 0, damageMultiplier: 0, painMultiplier: 0);

		// HEAD
		CreateBodypart(baseHumanoid, "neck", "neck", "neck", BodypartTypeEnum.Drapeable, "uback", Alignment.Front,
			Orientation.Highest, 80, 100, 50, 50, flesh, SizeCategory.Normal, "Head", isVital: true, implantSpace: 5,
			stunMultiplier: 0.4);
		CreateBodypart(baseHumanoid, "bneck", "back of neck", "neck back", BodypartTypeEnum.Drapeable, "neck",
			Alignment.Rear, Orientation.Highest, 80, 100, 50, 50, flesh, SizeCategory.Normal, "Head", isVital: true,
			implantSpace: 5, stunMultiplier: 0.4);
		CreateBodypart(baseHumanoid, "throat", "throat", "throat", BodypartTypeEnum.Drapeable, "neck", Alignment.Front,
			Orientation.Highest, 40, 100, 50, 50, flesh, SizeCategory.Normal, "Head", isVital: true, implantSpace: 5,
			stunMultiplier: 0.4);
		CreateBodypart(baseHumanoid, "face", "face", "face", BodypartTypeEnum.Drapeable, "neck", Alignment.Front,
			Orientation.Highest, 40, -1, 50, 50, flesh, SizeCategory.Normal, "Head", isVital: true, implantSpace: 5,
			stunMultiplier: 1.0);
		CreateBodypart(baseHumanoid, "chin", "chin", "chin", BodypartTypeEnum.Drapeable, "face", Alignment.Front,
			Orientation.Highest, 40, -1, 50, 50, flesh, SizeCategory.Normal, "Head", isVital: true, implantSpace: 5,
			stunMultiplier: 0.4);
		CreateBodypart(baseHumanoid, "rcheek", "right cheek", "cheek", BodypartTypeEnum.Drapeable, "face",
			Alignment.FrontRight, Orientation.Highest, 40, -1, 50, 50, flesh, SizeCategory.Normal, "Head",
			isVital: true, implantSpace: 5, stunMultiplier: 0.4);
		CreateBodypart(baseHumanoid, "lcheek", "left cheek", "cheek", BodypartTypeEnum.Drapeable, "face",
			Alignment.FrontLeft, Orientation.Highest, 40, -1, 50, 50, flesh, SizeCategory.Normal, "Head", isVital: true,
			implantSpace: 5, stunMultiplier: 0.4);
		CreateBodypart(baseHumanoid, "mouth", "mouth", "mouth", BodypartTypeEnum.Mouth, "face", Alignment.Front,
			Orientation.Highest, 40, -1, 50, 50, flesh, SizeCategory.Normal, "Head", isVital: true, implantSpace: 5,
			stunMultiplier: 0.4);
		CreateBodypart(baseHumanoid, "tongue", "tongue", "tongue", BodypartTypeEnum.Tongue, "mouth", Alignment.Front,
			Orientation.Highest, 20, -1, 5, 50, flesh, SizeCategory.Small, "Head", isVital: false, implantSpace: 5,
			stunMultiplier: 0.4);
		CreateBodypart(baseHumanoid, "nose", "nose", "nose", BodypartTypeEnum.Drapeable, "face", Alignment.Front,
			Orientation.Highest, 40, -1, 50, 50, flesh, SizeCategory.Normal, "Head", isVital: true, implantSpace: 5,
			stunMultiplier: 0.4);
		CreateBodypart(baseHumanoid, "forehead", "forehead", "forehead", BodypartTypeEnum.Drapeable, "face",
			Alignment.Front, Orientation.Highest, 40, -1, 50, 50, flesh, SizeCategory.Normal, "Head", isVital: true,
			implantSpace: 5, stunMultiplier: 1.0);
		CreateBodypart(baseHumanoid, "reyesocket", "right eye socket", "eye socket", BodypartTypeEnum.Drapeable, "face",
			Alignment.FrontRight, Orientation.Highest, 40, -1, 50, 50, flesh, SizeCategory.Normal, "Head",
			isVital: true, implantSpace: 5, stunMultiplier: 1.0);
		CreateBodypart(baseHumanoid, "leyesocket", "left eye socket", "eye socket", BodypartTypeEnum.Drapeable, "face",
			Alignment.FrontLeft, Orientation.Highest, 40, -1, 50, 50, flesh, SizeCategory.Normal, "Head", isVital: true,
			implantSpace: 5, stunMultiplier: 1.0);
		CreateBodypart(baseHumanoid, "reye", "right eye", "eye", BodypartTypeEnum.Eye, "reyesocket",
			Alignment.FrontRight, Orientation.Highest, 10, 30, 50, 50, flesh, SizeCategory.Small, "Head", isVital: true,
			implantSpace: 5, stunMultiplier: 0.4);
		CreateBodypart(baseHumanoid, "leye", "left eye", "eye", BodypartTypeEnum.Eye, "leyesocket", Alignment.FrontLeft,
			Orientation.Highest, 10, 30, 50, 50, flesh, SizeCategory.Small, "Head", isVital: true, implantSpace: 5,
			stunMultiplier: 0.4);
		CreateBodypart(baseHumanoid, "rear", "right ear", "ear", BodypartTypeEnum.Drapeable, "rcheek", Alignment.Right,
			Orientation.Highest, 10, 30, 50, 50, flesh, SizeCategory.Small, "Head", isVital: false, implantSpace: 5,
			stunMultiplier: 0.4);
		CreateBodypart(baseHumanoid, "lear", "left ear", "ear", BodypartTypeEnum.Drapeable, "lcheek", Alignment.Left,
			Orientation.Highest, 10, 30, 50, 50, flesh, SizeCategory.Small, "Head", isVital: false, implantSpace: 5,
			stunMultiplier: 0.4);
		CreateBodypart(baseHumanoid, "bhead", "back of head", "head back", BodypartTypeEnum.Drapeable, "neck",
			Alignment.Rear, Orientation.Highest, 40, -1, 50, 50, flesh, SizeCategory.Normal, "Head", isVital: true,
			implantSpace: 5, stunMultiplier: 1.5);
		CreateBodypart(baseHumanoid, "scalp", "scalp", "scalp", BodypartTypeEnum.Drapeable, "bhead", Alignment.Rear,
			Orientation.Highest, 40, -1, 50, 50, flesh, SizeCategory.Normal, "Head", isVital: true, implantSpace: 5,
			stunMultiplier: 1.0);
		CreateBodypart(baseHumanoid, "rbrow", "right brow", "eyebrow", BodypartTypeEnum.Drapeable, "face",
			Alignment.FrontRight, Orientation.Highest, 40, -1, 50, 50, flesh, SizeCategory.Small, "Head", isVital: true,
			implantSpace: 5, stunMultiplier: 1.0);
		CreateBodypart(baseHumanoid, "lbrow", "left brow", "eyebrow", BodypartTypeEnum.Drapeable, "face",
			Alignment.FrontLeft, Orientation.Highest, 40, -1, 50, 50, flesh, SizeCategory.Small, "Head", isVital: true,
			implantSpace: 5, stunMultiplier: 1.0);
		CreateBodypart(baseHumanoid, "rtemple", "right temple", "temple", BodypartTypeEnum.Drapeable, "rcheek",
			Alignment.Right, Orientation.Highest, 10, 30, 50, 50, flesh, SizeCategory.Small, "Head", isVital: true,
			implantSpace: 5, stunMultiplier: 1.0);
		CreateBodypart(baseHumanoid, "ltemple", "left temple", "temple", BodypartTypeEnum.Drapeable, "lcheek",
			Alignment.Left, Orientation.Highest, 10, 30, 50, 50, flesh, SizeCategory.Small, "Head", isVital: true,
			implantSpace: 5, stunMultiplier: 1.0);

		// ARMS
		CreateBodypart(baseHumanoid, "rupperarm", "right upper arm", "upper arm", BodypartTypeEnum.Drapeable,
			"rshoulder", Alignment.Right, Orientation.Appendage, 80, 100, 50, 50, flesh, SizeCategory.Normal,
			"Right Arm", isVital: false, implantSpace: 1, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "lupperarm", "left upper arm", "upper arm", BodypartTypeEnum.Drapeable,
			"lshoulder", Alignment.Left, Orientation.Appendage, 80, 100, 50, 50, flesh, SizeCategory.Normal, "Left Arm",
			isVital: false, implantSpace: 1, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "relbow", "right elbow", "elbow", BodypartTypeEnum.Drapeable, "rupperarm",
			Alignment.Right, Orientation.Appendage, 80, 100, 25, 50, flesh, SizeCategory.Normal, "Right Arm",
			isVital: false, implantSpace: 1, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "lelbow", "left elbow", "elbow", BodypartTypeEnum.Drapeable, "lupperarm",
			Alignment.Left, Orientation.Appendage, 80, 100, 25, 50, flesh, SizeCategory.Normal, "Left Arm",
			isVital: false, implantSpace: 1, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "rforearm", "right forearm", "forearm", BodypartTypeEnum.Drapeable, "relbow",
			Alignment.Right, Orientation.Appendage, 80, 100, 50, 50, flesh, SizeCategory.Normal, "Right Arm",
			isVital: false, implantSpace: 1, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "lforearm", "left forearm", "forearm", BodypartTypeEnum.Drapeable, "lelbow",
			Alignment.Left, Orientation.Appendage, 80, 100, 50, 50, flesh, SizeCategory.Normal, "Left Arm",
			isVital: false, implantSpace: 1, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "rwrist", "right wrist", "wrist", BodypartTypeEnum.Drapeable, "rforearm",
			Alignment.Right, Orientation.Appendage, 80, 100, 25, 50, flesh, SizeCategory.Normal, "Right Arm",
			isVital: false, implantSpace: 1, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "lwrist", "left wrist", "wrist", BodypartTypeEnum.Drapeable, "lforearm",
			Alignment.Left, Orientation.Appendage, 80, 100, 25, 50, flesh, SizeCategory.Normal, "Left Arm",
			isVital: false, implantSpace: 1, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "rhand", "right hand", "hand",
			_questionAnswers["inventory"].ToLowerInvariant().Equals("hands")
				? BodypartTypeEnum.GrabbingWielding
				: BodypartTypeEnum.Wielding, "rwrist", Alignment.Right, Orientation.Appendage, 20, 100, 25, 50, flesh,
			SizeCategory.Small, "Right Arm", isVital: false, implantSpace: 1, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "lhand", "left hand", "hand",
			_questionAnswers["inventory"].ToLowerInvariant().Equals("hands")
				? BodypartTypeEnum.GrabbingWielding
				: BodypartTypeEnum.Wielding, "lwrist", Alignment.Left, Orientation.Appendage, 20, 100, 25, 50, flesh,
			SizeCategory.Small, "Left Arm", isVital: false, implantSpace: 1, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "rthumb", "right thumb", "thumb", BodypartTypeEnum.Drapeable, "rhand",
			Alignment.Right, Orientation.Appendage, 5, 100, 5, 50, flesh, SizeCategory.VerySmall, "Right Arm",
			false, isVital: false, implantSpace: 0, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "lthumb", "left thumb", "thumb", BodypartTypeEnum.Drapeable, "lhand",
			Alignment.Left, Orientation.Appendage, 5, 100, 5, 50, flesh, SizeCategory.VerySmall, "Left Arm",
			false, isVital: false, implantSpace: 0, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "rindexfinger", "right index finger", "finger", BodypartTypeEnum.Drapeable,
			"rhand", Alignment.Right, Orientation.Appendage, 5, 100, 5, 50, bonyFlesh, SizeCategory.VerySmall,
			"Right Arm", false, isVital: false, implantSpace: 0, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "lindexfinger", "left index finger", "finger", BodypartTypeEnum.Drapeable, "lhand",
			Alignment.Left, Orientation.Appendage, 5, 100, 5, 50, bonyFlesh, SizeCategory.VerySmall, "Left Arm",
			false, isVital: false, implantSpace: 0, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "rmiddlefinger", "right middle finger", "finger", BodypartTypeEnum.Drapeable,
			"rhand", Alignment.Right, Orientation.Appendage, 5, 100, 5, 50, bonyFlesh, SizeCategory.VerySmall,
			"Right Arm", false, isVital: false, implantSpace: 0, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "lmiddlefinger", "left middle finger", "finger", BodypartTypeEnum.Drapeable,
			"lhand", Alignment.Left, Orientation.Appendage, 5, 100, 5, 50, bonyFlesh, SizeCategory.VerySmall,
			"Left Arm", false, isVital: false, implantSpace: 0, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "rringfinger", "right ring finger", "finger", BodypartTypeEnum.Drapeable, "rhand",
			Alignment.Right, Orientation.Appendage, 5, 100, 5, 50, bonyFlesh, SizeCategory.VerySmall, "Right Arm",
			false, isVital: false, implantSpace: 0, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "lringfinger", "left ring finger", "finger", BodypartTypeEnum.Drapeable, "lhand",
			Alignment.Left, Orientation.Appendage, 5, 100, 5, 50, bonyFlesh, SizeCategory.VerySmall, "Left Arm",
			false, isVital: false, implantSpace: 0, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "rpinkyfinger", "right pinky finger", "finger", BodypartTypeEnum.Drapeable,
			"rhand", Alignment.Right, Orientation.Appendage, 5, 100, 5, 50, bonyFlesh, SizeCategory.VerySmall,
			"Right Arm", false, isVital: false, implantSpace: 0, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "lpinkyfinger", "left pinky finger", "finger", BodypartTypeEnum.Drapeable, "lhand",
			Alignment.Left, Orientation.Appendage, 5, 100, 5, 50, bonyFlesh, SizeCategory.VerySmall, "Left Arm",
			false, isVital: false, implantSpace: 0, stunMultiplier: 0);

		// LEGS
		CreateBodypart(baseHumanoid, "rhip", "right hip", "hip", BodypartTypeEnum.Drapeable, "abdomen", Alignment.Right,
			Orientation.Centre, 80, -1, 50, 50, bonyFlesh, SizeCategory.Normal, "Right Leg", isVital: false,
			implantSpace: 1, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "lhip", "left hip", "hip", BodypartTypeEnum.Drapeable, "abdomen", Alignment.Left,
			Orientation.Centre, 80, -1, 50, 50, bonyFlesh, SizeCategory.Normal, "Left Leg", isVital: false,
			implantSpace: 1, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "rthigh", "right thigh", "thigh", BodypartTypeEnum.Drapeable, "rhip",
			Alignment.FrontRight, Orientation.Low, 80, 100, 50, 50, bonyFlesh, SizeCategory.Normal, "Right Leg",
			isVital: false, implantSpace: 1, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "lthigh", "left thigh", "thigh", BodypartTypeEnum.Drapeable, "lhip",
			Alignment.FrontLeft, Orientation.Low, 80, 100, 50, 50, bonyFlesh, SizeCategory.Normal, "Left Leg",
			isVital: false, implantSpace: 1, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "rthighback", "right thigh back", "thigh back", BodypartTypeEnum.Drapeable, "rhip",
			Alignment.RearRight, Orientation.Low, 80, 100, 50, 50, bonyFlesh, SizeCategory.Normal, "Right Leg",
			isVital: false, implantSpace: 1, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "lthighback", "left thigh back", "thigh back", BodypartTypeEnum.Drapeable, "lhip",
			Alignment.RearLeft, Orientation.Low, 80, 100, 50, 50, bonyFlesh, SizeCategory.Normal, "Left Leg",
			isVital: false, implantSpace: 1, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "rknee", "right knee", "knee", BodypartTypeEnum.Drapeable, "rthigh",
			Alignment.FrontRight, Orientation.Low, 40, 100, 50, 50, bonyFlesh, SizeCategory.Normal, "Right Leg",
			isVital: false, implantSpace: 1, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "lknee", "left knee", "knee", BodypartTypeEnum.Drapeable, "lthigh",
			Alignment.FrontLeft, Orientation.Low, 40, 100, 50, 50, bonyFlesh, SizeCategory.Normal, "Left Leg",
			isVital: false, implantSpace: 1, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "rkneeback", "right knee back", "knee back", BodypartTypeEnum.Drapeable, "rthigh",
			Alignment.RearRight, Orientation.Low, 40, 100, 50, 50, bonyFlesh, SizeCategory.Normal, "Right Leg",
			isVital: false, implantSpace: 1, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "lkneeback", "left knee back", "knee back", BodypartTypeEnum.Drapeable, "lthigh",
			Alignment.RearLeft, Orientation.Low, 40, 100, 50, 50, bonyFlesh, SizeCategory.Normal, "Left Leg",
			isVital: false, implantSpace: 1, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "rshin", "right shin", "shin", BodypartTypeEnum.Drapeable, "rknee",
			Alignment.FrontRight, Orientation.Lowest, 40, 100, 50, 50, bonyFlesh, SizeCategory.Normal, "Right Leg",
			isVital: false, implantSpace: 1, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "lshin", "left shin", "shin", BodypartTypeEnum.Drapeable, "lknee",
			Alignment.FrontLeft, Orientation.Lowest, 40, 100, 50, 50, bonyFlesh, SizeCategory.Normal, "Left Leg",
			isVital: false, implantSpace: 1, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "rcalf", "right calf", "calf", BodypartTypeEnum.Drapeable, "rknee",
			Alignment.RearRight, Orientation.Lowest, 40, 100, 50, 50, bonyFlesh, SizeCategory.Normal, "Right Leg",
			isVital: false, implantSpace: 1, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "lcalf", "left calf", "calf", BodypartTypeEnum.Drapeable, "lknee",
			Alignment.RearLeft, Orientation.Lowest, 40, 100, 50, 50, bonyFlesh, SizeCategory.Normal, "Left Leg",
			isVital: false, implantSpace: 1, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "rankle", "right ankle", "ankle", BodypartTypeEnum.Drapeable, "rshin",
			Alignment.Right, Orientation.Lowest, 40, 100, 25, 50, bonyFlesh, SizeCategory.Normal, "Right Leg",
			isVital: false, implantSpace: 1, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "lankle", "left ankle", "ankle", BodypartTypeEnum.Drapeable, "lshin",
			Alignment.Left, Orientation.Lowest, 40, 100, 25, 50, bonyFlesh, SizeCategory.Normal, "Left Leg",
			isVital: false, implantSpace: 1, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "rheel", "right heel", "heel", BodypartTypeEnum.Drapeable, "rankle",
			Alignment.RearRight, Orientation.Lowest, 40, 100, 25, 50, bonyFlesh, SizeCategory.Normal, "Right Leg",
			isVital: false, implantSpace: 1, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "lheel", "left heel", "heel", BodypartTypeEnum.Drapeable, "lankle",
			Alignment.RearLeft, Orientation.Lowest, 40, 100, 25, 50, bonyFlesh, SizeCategory.Normal, "Left Leg",
			isVital: false, implantSpace: 1, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "rfoot", "right foot", "foot", BodypartTypeEnum.Standing, "rankle",
			Alignment.FrontRight, Orientation.Lowest, 40, 100, 25, 50, bonyFlesh, SizeCategory.Normal, "Right Leg",
			isVital: false, implantSpace: 1, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "lfoot", "left foot", "foot", BodypartTypeEnum.Standing, "lankle",
			Alignment.FrontLeft, Orientation.Lowest, 40, 100, 25, 50, bonyFlesh, SizeCategory.Normal, "Left Leg",
			isVital: false, implantSpace: 1, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "rbigtoe", "right big toe", "toe", BodypartTypeEnum.Drapeable, "rfoot",
			Alignment.FrontRight, Orientation.Lowest, 20, 100, 5, 50, bonyFlesh, SizeCategory.VerySmall, "Right Leg",
			isVital: false, implantSpace: 1, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "lbigtoe", "left big toe", "toe", BodypartTypeEnum.Drapeable, "lfoot",
			Alignment.FrontLeft, Orientation.Lowest, 20, 100, 5, 50, bonyFlesh, SizeCategory.VerySmall, "Left Leg",
			isVital: false, implantSpace: 1, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "rindextoe", "right index toe", "toe", BodypartTypeEnum.Drapeable, "rfoot",
			Alignment.FrontRight, Orientation.Lowest, 20, 100, 5, 50, bonyFlesh, SizeCategory.VerySmall, "Right Leg",
			isVital: false, implantSpace: 1, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "lindextoe", "left index toe", "toe", BodypartTypeEnum.Drapeable, "lfoot",
			Alignment.FrontLeft, Orientation.Lowest, 20, 100, 5, 50, bonyFlesh, SizeCategory.VerySmall, "Left Leg",
			isVital: false, implantSpace: 1, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "rmiddletoe", "right middle toe", "toe", BodypartTypeEnum.Drapeable, "rfoot",
			Alignment.FrontRight, Orientation.Lowest, 20, 100, 5, 50, bonyFlesh, SizeCategory.VerySmall, "Right Leg",
			isVital: false, implantSpace: 1, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "lmiddletoe", "left middle toe", "toe", BodypartTypeEnum.Drapeable, "lfoot",
			Alignment.FrontLeft, Orientation.Lowest, 20, 100, 5, 50, bonyFlesh, SizeCategory.VerySmall, "Left Leg",
			isVital: false, implantSpace: 1, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "rringtoe", "right ring toe", "toe", BodypartTypeEnum.Drapeable, "rfoot",
			Alignment.FrontRight, Orientation.Lowest, 20, 100, 5, 50, bonyFlesh, SizeCategory.VerySmall, "Right Leg",
			isVital: false, implantSpace: 1, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "lringtoe", "left ring toe", "toe", BodypartTypeEnum.Drapeable, "lfoot",
			Alignment.FrontLeft, Orientation.Lowest, 20, 100, 5, 50, bonyFlesh, SizeCategory.VerySmall, "Left Leg",
			isVital: false, implantSpace: 1, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "rpinkytoe", "right pinky toe", "toe", BodypartTypeEnum.Drapeable, "rfoot",
			Alignment.FrontRight, Orientation.Lowest, 20, 100, 5, 50, bonyFlesh, SizeCategory.VerySmall, "Right Leg",
			isVital: false, implantSpace: 1, stunMultiplier: 0);
		CreateBodypart(baseHumanoid, "lpinkytoe", "left pinky toe", "toe", BodypartTypeEnum.Drapeable, "lfoot",
			Alignment.FrontLeft, Orientation.Lowest, 20, 100, 5, 50, bonyFlesh, SizeCategory.VerySmall, "Left Leg",
			isVital: false, implantSpace: 1, stunMultiplier: 0);

		// GENITALS
		CreateBodypart(baseHumanoid, "groin", "groin", "groin", BodypartTypeEnum.Drapeable, "belly", Alignment.Front,
			Orientation.Centre, 40, -1, 75, 52, flesh, SizeCategory.Small, "Genitals", implantSpace: 2,
			stunMultiplier: 0.2);
		CreateBodypart(baseHumanoid, "testicles", "testicles", "testicles", BodypartTypeEnum.Drapeable, "groin",
			Alignment.Front, Orientation.Centre, 10, 50, 5, 52, flesh, SizeCategory.Small, "Genitals", implantSpace: 2,
			stunMultiplier: 0.2, isCore: false);
		CreateBodypart(baseHumanoid, "penis", "penis", "penis", BodypartTypeEnum.Drapeable, "groin", Alignment.Front,
			Orientation.Centre, 10, 50, 5, 52, flesh, SizeCategory.Small, "Genitals", implantSpace: 2,
			stunMultiplier: 0.2, isCore: false);
		_context.BodyProtosAdditionalBodyparts.Add(new BodyProtosAdditionalBodyparts
		{
			BodyProto = baseHumanoid,
			Bodypart = _bodyparts["penis"],
			Usage = "male"
		});
		_context.BodyProtosAdditionalBodyparts.Add(new BodyProtosAdditionalBodyparts
		{
			BodyProto = baseHumanoid,
			Bodypart = _bodyparts["testicles"],
			Usage = "male"
		});
		_context.SaveChanges();

		#endregion

		#region Organs

		AddOrgan("brain", "brain", BodypartTypeEnum.Brain, 2.0, 50, 0.2, 0.2, 0.1, organicHumanoid, stunModifier: 1.0);
		AddOrgan("heart", "heart", BodypartTypeEnum.Heart, 1.0, 50, 0.2, 1.0, 1.0, organicHumanoid);
		AddOrgan("liver", "liver", BodypartTypeEnum.Liver, 3.0, 50, 0.2, 1.0, 0.05, organicHumanoid);
		AddOrgan("spleen", "spleen", BodypartTypeEnum.Spleen, 1.0, 50, 0.2, 1.0, 0.05, organicHumanoid);
		AddOrgan("stomach", "stomach", BodypartTypeEnum.Stomach, 1.0, 50, 0.2, 1.0, 0.05, organicHumanoid);
		AddOrgan("lintestines", "large intestines", BodypartTypeEnum.Intestines, 0.5, 50, 0.2, 1.0, 0.05,
			organicHumanoid);
		AddOrgan("sintestines", "small intestines", BodypartTypeEnum.Intestines, 2.0, 50, 0.2, 1.0, 0.05,
			organicHumanoid);
		AddOrgan("rkidney", "right kidney", BodypartTypeEnum.Kidney, 0.5, 50, 0.2, 2.0, 0.05, organicHumanoid,
			painModifier: 3.0);
		AddOrgan("lkidney", "left kidney", BodypartTypeEnum.Kidney, 0.5, 50, 0.2, 2.0, 0.05, organicHumanoid,
			painModifier: 3.0);
		AddOrgan("rlung", "right lung", BodypartTypeEnum.Lung, 2.0, 50, 0.2, 1.0, 0.05, organicHumanoid);
		AddOrgan("llung", "left lung", BodypartTypeEnum.Lung, 2.0, 50, 0.2, 1.0, 0.05, organicHumanoid);
		AddOrgan("trachea", "trachea", BodypartTypeEnum.Trachea, 1.0, 50, 0.2, 1.0, 0.05, organicHumanoid);
		AddOrgan("esophagus", "esophagus", BodypartTypeEnum.Esophagus, 1.0, 50, 0.2, 1.0, 0.05, organicHumanoid);
		AddOrgan("uspinalcord", "upper spinal cord", BodypartTypeEnum.Spine, 1.0, 15, 0.2, 1.0, 0.05, organicHumanoid,
			stunModifier: 1.0,
			painModifier: 2.0);
		AddOrgan("mspinalcord", "middle spinal cord", BodypartTypeEnum.Spine, 1.0, 15, 0.2, 1.0, 0.05, organicHumanoid,
			stunModifier: 1.0, painModifier: 2.0);
		AddOrgan("lspinalcord", "lower spinal cord", BodypartTypeEnum.Spine, 1.0, 15, 0.2, 1.0, 0.05, organicHumanoid,
			stunModifier: 1.0,
			painModifier: 2.0);
		AddOrgan("rinnerear", "lower spinal cord", BodypartTypeEnum.Ear, 1.0, 15, 0.2, 1.0, 0.05, organicHumanoid);
		AddOrgan("linnerear", "lower spinal cord", BodypartTypeEnum.Ear, 1.0, 15, 0.2, 1.0, 0.05, organicHumanoid);

		AddOrganCoverage("brain", "scalp", 100, true);
		AddOrganCoverage("brain", "face", 100);
		AddOrganCoverage("brain", "rcheek", 85);
		AddOrganCoverage("brain", "lcheek", 85);
		AddOrganCoverage("brain", "forehead", 100);
		AddOrganCoverage("brain", "nose", 85);
		AddOrganCoverage("brain", "reyesocket", 85);
		AddOrganCoverage("brain", "leyesocket", 85);
		AddOrganCoverage("brain", "reye", 85);
		AddOrganCoverage("brain", "leye", 85);
		AddOrganCoverage("brain", "lbrow", 100);
		AddOrganCoverage("brain", "rbrow", 100);
		AddOrganCoverage("brain", "mouth", 50);
		AddOrganCoverage("brain", "tongue", 10);
		AddOrganCoverage("brain", "lear", 10);
		AddOrganCoverage("brain", "rear", 10);

		AddOrganCoverage("linnerear", "lear", 33, true);
		AddOrganCoverage("rinnerear", "rear", 33, true);
		AddOrganCoverage("esophagus", "throat", 50, true);
		AddOrganCoverage("esophagus", "neck", 20);
		AddOrganCoverage("esophagus", "bneck", 5);
		AddOrganCoverage("trachea", "throat", 50, true);
		AddOrganCoverage("trachea", "neck", 20);
		AddOrganCoverage("trachea", "bneck", 5);

		AddOrganCoverage("rlung", "rbreast", 100, true);
		AddOrganCoverage("llung", "lbreast", 100, true);
		AddOrganCoverage("rlung", "rnipple", 100);
		AddOrganCoverage("llung", "lnipple", 100);
		AddOrganCoverage("rlung", "uback", 15);
		AddOrganCoverage("llung", "uback", 15);
		AddOrganCoverage("rlung", "rshoulderblade", 66);
		AddOrganCoverage("llung", "lshoulderblade", 66);

		AddOrganCoverage("heart", "lbreast", 33, true);
		AddOrganCoverage("heart", "lshoulderblade", 20);
		AddOrganCoverage("heart", "lnipple", 2);

		AddOrganCoverage("uspinalcord", "bneck", 10, true);
		AddOrganCoverage("uspinalcord", "neck", 2);
		AddOrganCoverage("uspinalcord", "throat", 5);
		AddOrganCoverage("mspinalcord", "uback", 10, true);
		AddOrganCoverage("lspinalcord", "lback", 10, true);

		AddOrganCoverage("liver", "abdomen", 33, true);
		AddOrganCoverage("spleen", "abdomen", 20, true);
		AddOrganCoverage("stomach", "abdomen", 20, true);
		AddOrganCoverage("liver", "uback", 15);
		AddOrganCoverage("spleen", "uback", 10);
		AddOrganCoverage("stomach", "uback", 5);

		AddOrganCoverage("lintestines", "belly", 5, true);
		AddOrganCoverage("sintestines", "belly", 50, true);
		AddOrganCoverage("lintestines", "lback", 5);
		AddOrganCoverage("sintestines", "lback", 33);
		AddOrganCoverage("lintestines", "groin", 5);

		AddOrganCoverage("rkidney", "lback", 20, true);
		AddOrganCoverage("lkidney", "lback", 20, true);
		AddOrganCoverage("rkidney", "belly", 5);
		AddOrganCoverage("lkidney", "belly", 5);

		_context.SaveChanges();

		#endregion

		#region Bones

		if (_questionAnswers["bones"].ToLowerInvariant().In("yes", "y"))
		{
			_boneArmour = new ArmourType
			{
				Name = "Human Natural Bone Armour",
				MinimumPenetrationDegree = 1,
				BaseDifficultyDegrees = 0,
				StackedDifficultyDegrees = 0,
				Definition = @"<ArmourType>

	<!-- Damage Transformations change damage passed on to bones/organs/items into a different damage type when severity is under a certain  threshold 
		
		Damage Types:
		
		Slashing = 0
        Chopping = 1
        Crushing = 2
        Piercing = 3
        Ballistic = 4
        Burning = 5
        Freezing = 6
        Chemical = 7
        Shockwave = 8
        Bite = 9
        Claw = 10
        Electrical = 11
        Hypoxia = 12
        Cellular = 13
        Sonic = 14
        Shearing = 15
        ArmourPiercing = 16
        Wrenching = 17
        Shrapnel = 18
        Necrotic = 19
        Falling = 20
        Eldritch = 21
        Arcane = 22
		
		Severity Values:
		
		None = 0
        Superficial = 1
        Minor = 2
        Small = 3
        Moderate = 4
        Severe = 5
        VerySevere = 6
        Grievous = 7
        Horrifying = 8
	-->
	<DamageTransformations>
 		<Transform fromtype=""0"" totype=""2"" severity=""5""></Transform> <!-- Slashing to Crushing when <= Severe -->
 		<Transform fromtype=""1"" totype=""2"" severity=""5""></Transform> <!-- Chopping to Crushing when <= Severe -->
 		<Transform fromtype=""3"" totype=""2"" severity=""4""></Transform> <!-- Piercing to Crushing when <= Moderate -->
 		<Transform fromtype=""4"" totype=""2"" severity=""4""></Transform> <!-- Ballistic to Crushing when <= Moderate -->
 		<Transform fromtype=""9"" totype=""2"" severity=""5""></Transform> <!-- Bite to Crushing when <= Severe -->
 		<Transform fromtype=""10"" totype=""2"" severity=""5""></Transform> <!-- Claw to Crushing when <= Severe -->
 		<Transform fromtype=""15"" totype=""2"" severity=""5""></Transform> <!-- Shearing to Crushing when <= Severe -->
 		<Transform fromtype=""16"" totype=""2"" severity=""3""></Transform> <!-- ArmourPiercing to Crushing when <= Small -->
 		<Transform fromtype=""17"" totype=""2"" severity=""5""></Transform> <!-- Wrenching to Crushing when <= Severe -->
 	</DamageTransformations>
    <!-- 
	
	    Dissipate expressions are applied before the item/part takes damage. 
		If they reduce the damage to zero, it neither suffers nor passes on any damage. 
		
		Parameters: 
		* damage, pain or stun (as appropriate) = the raw damage/pain/stun suffered
		* quality = the quality of the armour, rated 0 (Abysmal) to 11 (Legendary)
		* angle = the angle in radians of the attack (e.g. 1.5708rad = 90 degrees)
		* density = the density in kg/m3 of the material that the armour is made from
		* electrical = the electrical conductivity of the material that the armour is made from (1/ohm metres)
		* thermal = the thermal conductivity of the material that the armour is made from (watts per meter3 per kelvin)
		* organic = if the material that the armour is made from is organic (1 for true, 0 for false)
		* strength = either ImpactYield or ShearYield of the armour material depending on the damage type, in Pascals.
		
		Hint: 25000 can be considered ""base"" ShearYield and 10000 can be considered ""base"" ImpactYield
	-->
    <DissipateExpressions>
        <Expression damagetype=""0"">max(damage*0.1,damage-(quality * 2 * strength/115000))</Expression>    <!-- Slashing -->
 		<Expression damagetype=""1"">max(damage*0.1,damage-(quality * 2 * strength/115000)))</Expression>    <!-- Chopping -->  
 		<Expression damagetype=""2"">max(damage*0.1,damage-(quality * 2 * strength/200000))</Expression>    <!-- Crushing -->  
 		<Expression damagetype=""3"">max(damage*0.1,damage-(quality * 2 * strength/115000))</Expression>    <!-- Piercing -->  
 		<Expression damagetype=""4"">max(damage*0.1,damage-(quality * 2 * strength/115000))</Expression>    <!-- Ballistic -->  
 		<Expression damagetype=""5"">damage - (quality * 2)</Expression>    			      <!-- Burning -->
 		<Expression damagetype=""6"">damage - (quality * 2)</Expression>                     <!-- Freezing -->
 		<Expression damagetype=""7"">damage - (quality * 2)</Expression>                     <!-- Chemical -->
 		<Expression damagetype=""8"">max(damage*0.1,damage-(quality * 2 * strength/200000))</Expression>    <!-- Shockwave -->
 		<Expression damagetype=""9"">max(damage*0.1,damage-(quality * 2 * strength/115000))</Expression>    <!-- Bite -->
 		<Expression damagetype=""10"">max(damage*0.1,damage-(quality * 2 * strength/115000))</Expression>   <!-- Claw -->
 		<Expression damagetype=""11"">damage - (quality * 2)</Expression>                    <!-- Electrical -->
 		<Expression damagetype=""12"">0</Expression>                    <!-- Hypoxia -->
 		<Expression damagetype=""13"">damage - (quality * 2)</Expression>                    <!-- Cellular -->
		<Expression damagetype=""14"">max(damage*0.1,damage-(quality * 2 * strength/200000))</Expression>   <!-- Sonic -->
 		<Expression damagetype=""15"">max(damage*0.1,damage-(quality * 2 * strength/115000))</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">max(damage*0.1,damage-(quality * 2 * strength/115000))</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">max(damage*0.1,damage-(quality * 2 * strength/200000))</Expression>   <!-- Wrenching -->
 		<Expression damagetype=""18"">max(damage*0.1,damage-(quality * 2 * strength/115000))</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">damage - (quality * 2)</Expression>                    <!-- Necrotic -->   
 		<Expression damagetype=""20"">max(damage*0.1,damage-(quality * 2 * strength/200000))</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">damage - (quality * 2)</Expression>                    <!-- Eldritch -->   
		<Expression damagetype=""22"">damage - (quality * 2)</Expression>                    <!-- Arcane -->   
 	</DissipateExpressions>  
 	<DissipateExpressionsPain>
        <Expression damagetype=""0"">pain - (quality * strength/25000 * 0.75)</Expression>    <!-- Slashing -->
 		<Expression damagetype=""1"">pain - (quality * strength/25000 * 0.75)</Expression>    <!-- Chopping -->  
 		<Expression damagetype=""2"">pain - (quality * strength/10000 * 0.75)</Expression>    <!-- Crushing -->  
 		<Expression damagetype=""3"">pain - (quality * strength/25000 * 0.75)</Expression>    <!-- Piercing -->  
 		<Expression damagetype=""4"">pain - (quality * strength/25000 * 0.75)</Expression>    <!-- Ballistic -->  
 		<Expression damagetype=""5"">pain - (quality * 0.75)</Expression>    			        <!-- Burning -->
 		<Expression damagetype=""6"">pain - (quality * 0.75)</Expression>                     <!-- Freezing -->
 		<Expression damagetype=""7"">pain - (quality * 0.75)</Expression>                     <!-- Chemical -->
 		<Expression damagetype=""8"">pain - (quality * strength/10000 * 0.75)</Expression>    <!-- Shockwave -->
 		<Expression damagetype=""9"">pain - (quality * strength/25000 * 0.75)</Expression>    <!-- Bite -->
 		<Expression damagetype=""10"">pain - (quality * strength/25000 * 0.75)</Expression>   <!-- Claw -->
 		<Expression damagetype=""11"">pain - (quality * 0.75)</Expression>                    <!-- Electrical -->
 		<Expression damagetype=""12"">pain - (quality * 0.75)</Expression>                    <!-- Hypoxia -->
 		<Expression damagetype=""13"">pain - (quality * 0.75)</Expression>                    <!-- Cellular -->
		<Expression damagetype=""14"">pain - (quality * strength/10000 * 0.75)</Expression>   <!-- Sonic -->
 		<Expression damagetype=""15"">pain - (quality * strength/25000 * 0.75)</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">pain - (quality * strength/25000 * 0.75)</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">pain - (quality * strength/10000 * 0.75)</Expression>   <!-- Wrenching -->
 		<Expression damagetype=""18"">pain - (quality * strength/25000 * 0.75)</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">pain - (quality * 0.75)</Expression>                    <!-- Necrotic -->   
 		<Expression damagetype=""20"">pain - (quality * strength/10000 * 0.75)</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">pain - (quality * 0.75)</Expression>                    <!-- Eldritch -->   
		<Expression damagetype=""22"">pain - (quality * 0.75)</Expression>                    <!-- Arcane -->   
 	</DissipateExpressionsPain>  
 	<DissipateExpressionsStun>
        <Expression damagetype=""0"">stun - (quality * strength/25000 * 0.75)</Expression>    <!-- Slashing -->
 		<Expression damagetype=""1"">stun - (quality * strength/25000 * 0.75)</Expression>    <!-- Chopping -->  
 		<Expression damagetype=""2"">stun - (quality * strength/10000 * 0.75)</Expression>    <!-- Crushing -->  
 		<Expression damagetype=""3"">stun - (quality * strength/25000 * 0.75)</Expression>    <!-- Piercing -->  
 		<Expression damagetype=""4"">stun - (quality * strength/25000 * 0.75)</Expression>    <!-- Ballistic -->  
 		<Expression damagetype=""5"">stun - (quality * 0.75)</Expression>    			        <!-- Burning -->
 		<Expression damagetype=""6"">stun - (quality * 0.75)</Expression>                     <!-- Freezing -->
 		<Expression damagetype=""7"">stun - (quality * 0.75)</Expression>                     <!-- Chemical -->
 		<Expression damagetype=""8"">stun - (quality * strength/10000 * 0.75)</Expression>    <!-- Shockwave -->
 		<Expression damagetype=""9"">stun - (quality * strength/25000 * 0.75)</Expression>    <!-- Bite -->
 		<Expression damagetype=""10"">stun - (quality * strength/25000 * 0.75)</Expression>   <!-- Claw -->
 		<Expression damagetype=""11"">stun - (quality * 0.75)</Expression>                    <!-- Electrical -->
 		<Expression damagetype=""12"">stun - (quality * 0.75)</Expression>                    <!-- Hypoxia -->
 		<Expression damagetype=""13"">stun - (quality * 0.75)</Expression>                    <!-- Cellular -->
		<Expression damagetype=""14"">stun - (quality * strength/10000 * 0.75)</Expression>   <!-- Sonic -->
 		<Expression damagetype=""15"">stun - (quality * strength/25000 * 0.75)</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">stun - (quality * strength/25000 * 0.75)</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">stun - (quality * strength/10000 * 0.75)</Expression>   <!-- Wrenching -->
 		<Expression damagetype=""18"">stun - (quality * strength/25000 * 0.75)</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">stun - (quality * 0.75)</Expression>                    <!-- Necrotic -->   
 		<Expression damagetype=""20"">stun - (quality * strength/10000 * 0.75)</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">stun - (quality * 0.75)</Expression>                    <!-- Eldritch -->   
		<Expression damagetype=""22"">stun - (quality * 0.75)</Expression>                    <!-- Arcane -->   
 	</DissipateExpressionsStun>  
	<!-- 
	
	    Absorb expressions are applied after dissipate expressions and item/part damage. 
	    The after-absorb values are what is passed on to anything ""below"" e.g. bones, organs, parts worn under armour, etc 
		
	    Parameters: 
		* damage, pain or stun (as appropriate) = the residual damage/pain/stun after dissipate step
		* quality = the quality of the armour, rated 0 (Abysmal) to 11 (Legendary)
		* angle = the angle in radians of the attack (e.g. 1.5708rad = 90 degrees)
		* density = the density in kg/m3 of the material that the armour is made from
		* electrical = the electrical conductivity of the material that the armour is made from (1/ohm metres)
		* thermal = the thermal conductivity of the material that the armour is made from (watts per meter3 per kelvin)
		* organic = if the material that the armour is made from is organic (1 for true, 0 for false)
		* strength = either ImpactYield or ShearYield of the armour material depending on the damage type, in Pascals.
		
		Hint: 25000 can be considered ""base"" ShearYield and 10000 can be considered ""base"" ImpactYield
		
		-->
 	<AbsorbExpressions>
	 	<Expression damagetype=""0"">damage*(0.8-(quality*0.02))</Expression>    <!-- Slashing -->
 		<Expression damagetype=""1"">damage*(0.8-(quality*0.02))</Expression>    <!-- Chopping -->  
 		<Expression damagetype=""2"">damage*(0.8-(quality*0.02))</Expression>    <!-- Crushing -->  
 		<Expression damagetype=""3"">damage*(0.8-(quality*0.02))</Expression>    <!-- Piercing -->  
 		<Expression damagetype=""4"">damage*(0.9-(quality*0.02))</Expression>    <!-- Ballistic -->  
 		<Expression damagetype=""5"">damage*(0.5-(quality*0.02))</Expression>    <!-- Burning -->
 		<Expression damagetype=""6"">damage*(0.5-(quality*0.02))</Expression>    <!-- Freezing -->
 		<Expression damagetype=""7"">damage*(0.5-(quality*0.02))</Expression>    <!-- Chemical -->
 		<Expression damagetype=""8"">damage*(0.8-(quality*0.02))</Expression>    <!-- Shockwave -->
 		<Expression damagetype=""9"">damage*(0.8-(quality*0.02))</Expression>    <!-- Bite -->
 		<Expression damagetype=""10"">damage*(0.8-(quality*0.02))</Expression>   <!-- Claw -->
 		<Expression damagetype=""11"">damage*(0.5-(quality*0.02))</Expression>   <!-- Electrical -->
 		<Expression damagetype=""12"">0</Expression>                             <!-- Hypoxia -->
 		<Expression damagetype=""13"">0</Expression>                             <!-- Cellular -->
		<Expression damagetype=""14"">damage*(0.8-(quality*0.02))</Expression>   <!-- Sonic -->
 		<Expression damagetype=""15"">damage*(0.8-(quality*0.02))</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">damage*(1.0-(quality*0.02))</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">damage*(0.8-(quality*0.02))</Expression>   <!-- Wrenching -->
 		<Expression damagetype=""18"">damage*(0.8-(quality*0.02))</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">damage*(0.8-(quality*0.02))</Expression>   <!-- Necrotic -->   
 		<Expression damagetype=""20"">damage*(0.8-(quality*0.02))</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">damage*(0.8-(quality*0.02))</Expression>   <!-- Eldritch -->   
		<Expression damagetype=""22"">damage*(0.8-(quality*0.02))</Expression>   <!-- Arcane -->   
 	</AbsorbExpressions>  
 	<AbsorbExpressionsPain>
        <Expression damagetype=""0"">pain*(0.8-(quality*0.02))</Expression>    <!-- Slashing -->
 		<Expression damagetype=""1"">pain*(0.8-(quality*0.02))</Expression>    <!-- Chopping -->  
 		<Expression damagetype=""2"">pain*(0.8-(quality*0.02))</Expression>    <!-- Crushing -->  
 		<Expression damagetype=""3"">pain*(0.8-(quality*0.02))</Expression>    <!-- Piercing -->  
 		<Expression damagetype=""4"">pain*(0.9-(quality*0.02))</Expression>    <!-- Ballistic -->  
 		<Expression damagetype=""5"">pain*(0.5-(quality*0.02))</Expression>    <!-- Burning -->
 		<Expression damagetype=""6"">pain*(0.5-(quality*0.02))</Expression>    <!-- Freezing -->
 		<Expression damagetype=""7"">pain*(0.5-(quality*0.02))</Expression>    <!-- Chemical -->
 		<Expression damagetype=""8"">pain*(0.8-(quality*0.02))</Expression>    <!-- Shockwave -->
 		<Expression damagetype=""9"">pain*(0.8-(quality*0.02))</Expression>    <!-- Bite -->
 		<Expression damagetype=""10"">pain*(0.8-(quality*0.02))</Expression>   <!-- Claw -->
 		<Expression damagetype=""11"">pain*(0.5-(quality*0.02))</Expression>   <!-- Electrical -->
 		<Expression damagetype=""12"">0</Expression>                             <!-- Hypoxia -->
 		<Expression damagetype=""13"">0</Expression>                             <!-- Cellular -->
		<Expression damagetype=""14"">pain*(0.8-(quality*0.02))</Expression>   <!-- Sonic -->
 		<Expression damagetype=""15"">pain*(0.8-(quality*0.02))</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">pain*(1.0-(quality*0.02))</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">pain*(0.8-(quality*0.02))</Expression>   <!-- Wrenching -->
 		<Expression damagetype=""18"">pain*(0.8-(quality*0.02))</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">pain*(0.8-(quality*0.02))</Expression>   <!-- Necrotic -->   
 		<Expression damagetype=""20"">pain*(0.8-(quality*0.02))</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">pain*(0.8-(quality*0.02))</Expression>   <!-- Eldritch -->   
		<Expression damagetype=""22"">pain*(0.8-(quality*0.02))</Expression>   <!-- Arcane -->   
 	</AbsorbExpressionsPain>  
 	<AbsorbExpressionsStun>
        <Expression damagetype=""0"">stun*(0.8-(quality*0.02))</Expression>    <!-- Slashing -->
 		<Expression damagetype=""1"">stun*(0.8-(quality*0.02))</Expression>    <!-- Chopping -->  
 		<Expression damagetype=""2"">stun*(0.8-(quality*0.02))</Expression>    <!-- Crushing -->  
 		<Expression damagetype=""3"">stun*(0.8-(quality*0.02))</Expression>    <!-- Piercing -->  
 		<Expression damagetype=""4"">stun*(0.9-(quality*0.02))</Expression>    <!-- Ballistic -->  
 		<Expression damagetype=""5"">stun*(0.5-(quality*0.02))</Expression>    <!-- Burning -->
 		<Expression damagetype=""6"">stun*(0.5-(quality*0.02))</Expression>    <!-- Freezing -->
 		<Expression damagetype=""7"">stun*(0.5-(quality*0.02))</Expression>    <!-- Chemical -->
 		<Expression damagetype=""8"">stun*(0.8-(quality*0.02))</Expression>    <!-- Shockwave -->
 		<Expression damagetype=""9"">stun*(0.8-(quality*0.02))</Expression>    <!-- Bite -->
 		<Expression damagetype=""10"">stun*(0.8-(quality*0.02))</Expression>   <!-- Claw -->
 		<Expression damagetype=""11"">stun*(0.5-(quality*0.02))</Expression>   <!-- Electrical -->
 		<Expression damagetype=""12"">0</Expression>                             <!-- Hypoxia -->
 		<Expression damagetype=""13"">0</Expression>                             <!-- Cellular -->
		<Expression damagetype=""14"">stun*(0.8-(quality*0.02))</Expression>   <!-- Sonic -->
 		<Expression damagetype=""15"">stun*(0.8-(quality*0.02))</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">stun*(1.0-(quality*0.02))</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">stun*(0.8-(quality*0.02))</Expression>   <!-- Wrenching -->
 		<Expression damagetype=""18"">stun*(0.8-(quality*0.02))</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">stun*(0.8-(quality*0.02))</Expression>   <!-- Necrotic -->   
 		<Expression damagetype=""20"">stun*(0.8-(quality*0.02))</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">stun*(0.8-(quality*0.02))</Expression>   <!-- Eldritch -->   
		<Expression damagetype=""22"">stun*(0.8-(quality*0.02))</Expression>   <!-- Arcane -->   
 	</AbsorbExpressionsStun>
 </ArmourType>"
			};
			_context.ArmourTypes.Add(_boneArmour);
			_context.SaveChanges();


			// TORSO BONES
			AddBone(organicHumanoid, "sternum", "sternum", BodypartTypeEnum.NonImmobilisingBone, 200, _compactBone);
			AddBone(organicHumanoid, "rrib1", "right first rib", BodypartTypeEnum.NonImmobilisingBone, 100,
				_compactBone);
			AddBone(organicHumanoid, "lrib1", "left first rib", BodypartTypeEnum.NonImmobilisingBone, 100,
				_compactBone);
			AddBone(organicHumanoid, "rrib2", "right second rib", BodypartTypeEnum.NonImmobilisingBone, 100,
				_compactBone);
			AddBone(organicHumanoid, "lrib2", "left second rib", BodypartTypeEnum.NonImmobilisingBone, 100,
				_compactBone);
			AddBone(organicHumanoid, "rrib3", "right third rib", BodypartTypeEnum.NonImmobilisingBone, 100,
				_compactBone);
			AddBone(organicHumanoid, "lrib3", "left third rib", BodypartTypeEnum.NonImmobilisingBone, 100,
				_compactBone);
			AddBone(organicHumanoid, "rrib4", "right fourth rib", BodypartTypeEnum.NonImmobilisingBone, 100,
				_compactBone);
			AddBone(organicHumanoid, "lrib4", "left fourth rib", BodypartTypeEnum.NonImmobilisingBone, 100,
				_compactBone);
			AddBone(organicHumanoid, "rrib5", "right fifth rib", BodypartTypeEnum.NonImmobilisingBone, 100,
				_compactBone);
			AddBone(organicHumanoid, "lrib5", "left fifth rib", BodypartTypeEnum.NonImmobilisingBone, 100,
				_compactBone);
			AddBone(organicHumanoid, "rrib6", "right sixth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
				_compactBone);
			AddBone(organicHumanoid, "lrib6", "left sixth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
				_compactBone);
			AddBone(organicHumanoid, "rrib7", "right seventh rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
				_compactBone);
			AddBone(organicHumanoid, "lrib7", "left seventh rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
				_compactBone);
			AddBone(organicHumanoid, "rrib8", "right eighth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
				_compactBone);
			AddBone(organicHumanoid, "lrib8", "left eighth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
				_compactBone);
			AddBone(organicHumanoid, "rrib9", "right ninth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
				_compactBone);
			AddBone(organicHumanoid, "lrib9", "left ninth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
				_compactBone);
			AddBone(organicHumanoid, "rrib10", "right tenth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
				_compactBone);
			AddBone(organicHumanoid, "lrib10", "left tenth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
				_compactBone);
			AddBone(organicHumanoid, "rrib11", "right eleventh rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
				_compactBone);
			AddBone(organicHumanoid, "lrib11", "left eleventh rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
				_compactBone);
			AddBone(organicHumanoid, "rrib12", "right twelth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
				_compactBone);
			AddBone(organicHumanoid, "lrib12", "left twelth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
				_compactBone);
			AddBone(organicHumanoid, "uspine", "upper spine", BodypartTypeEnum.NonImmobilisingBone, 80, _compactBone);
			AddBone(organicHumanoid, "mspine", "middle spine", BodypartTypeEnum.NonImmobilisingBone, 80, _compactBone);
			AddBone(organicHumanoid, "lspine", "lower spine", BodypartTypeEnum.NonImmobilisingBone, 80, _compactBone);
			AddBone(organicHumanoid, "rilium", "right ilium", BodypartTypeEnum.NonImmobilisingBone, 150, _compactBone);
			AddBone(organicHumanoid, "lilium", "left ilium", BodypartTypeEnum.NonImmobilisingBone, 150, _compactBone);
			AddBone(organicHumanoid, "sacrum", "sacrum", BodypartTypeEnum.NonImmobilisingBone, 150, _compactBone);
			AddBone(organicHumanoid, "rpubis", "right pubis", BodypartTypeEnum.NonImmobilisingBone, 150, _compactBone);
			AddBone(organicHumanoid, "lpubis", "left pubis", BodypartTypeEnum.NonImmobilisingBone, 150, _compactBone);
			AddBone(organicHumanoid, "rischium", "right ischium", BodypartTypeEnum.NonImmobilisingBone, 150,
				_compactBone);
			AddBone(organicHumanoid, "lischium", "left ischium", BodypartTypeEnum.NonImmobilisingBone, 150,
				_compactBone);

			// HEAD BONES
			AddBone(organicHumanoid, "parietal", "parietal bone", BodypartTypeEnum.NonImmobilisingBone, 140,
				_compactBone);
			AddBone(organicHumanoid, "fcranialbone", "frontal cranial bone", BodypartTypeEnum.NonImmobilisingBone, 140,
				_compactBone);
			AddBone(organicHumanoid, "occipital", "occipital bone", BodypartTypeEnum.NonImmobilisingBone, 140,
				_compactBone);
			AddBone(organicHumanoid, "jawbone", "jawbone", BodypartTypeEnum.NonImmobilisingBone, 140, _compactBone);
			AddBone(organicHumanoid, "rtemporal", "right temporal bone", BodypartTypeEnum.NonImmobilisingBone, 140,
				_compactBone);
			AddBone(organicHumanoid, "ltemporal", "left temporal bone", BodypartTypeEnum.NonImmobilisingBone, 140,
				_compactBone);
			AddBone(organicHumanoid, "maxilla", "maxilla", BodypartTypeEnum.NonImmobilisingBone, 140, _compactBone);
			AddBone(organicHumanoid, "rzygomatic", "right zygomatic bone", BodypartTypeEnum.NonImmobilisingBone, 140,
				_compactBone);
			AddBone(organicHumanoid, "lzygomatic", "left zygomatic bone", BodypartTypeEnum.NonImmobilisingBone, 140,
				_compactBone);

			// ARM BONES
			AddBone(organicHumanoid, "rclavicle", "right clavicle", BodypartTypeEnum.NonImmobilisingBone, 150,
				_compactBone);
			AddBone(organicHumanoid, "lclavicle", "left clavicle", BodypartTypeEnum.NonImmobilisingBone, 150,
				_compactBone);
			AddBone(organicHumanoid, "rscapula", "right scapula", BodypartTypeEnum.NonImmobilisingBone, 150,
				_compactBone);
			AddBone(organicHumanoid, "lscapula", "left scapula", BodypartTypeEnum.NonImmobilisingBone, 150,
				_compactBone);
			AddBone(organicHumanoid, "rhumerus", "right humerus", BodypartTypeEnum.Bone, 140, _compactBone);
			AddBone(organicHumanoid, "lhumerus", "left humerus", BodypartTypeEnum.Bone, 140, _compactBone);
			AddBone(organicHumanoid, "rradius", "right radius", BodypartTypeEnum.Bone, 140, _compactBone);
			AddBone(organicHumanoid, "lradius", "left radius", BodypartTypeEnum.Bone, 140, _compactBone);
			AddBone(organicHumanoid, "rulna", "right ulna", BodypartTypeEnum.Bone, 120, _compactBone);
			AddBone(organicHumanoid, "lulna", "left ulna", BodypartTypeEnum.Bone, 120, _compactBone);
			AddBone(organicHumanoid, "r1carpal", "right first carpal", BodypartTypeEnum.MinorBone, 40, _compactBone);
			AddBone(organicHumanoid, "l1carpal", "left first carpal", BodypartTypeEnum.MinorBone, 40, _compactBone);
			AddBone(organicHumanoid, "r2carpal", "right second carpal", BodypartTypeEnum.MinorBone, 40, _compactBone);
			AddBone(organicHumanoid, "l2carpal", "left second carpal", BodypartTypeEnum.MinorBone, 40, _compactBone);
			AddBone(organicHumanoid, "r3carpal", "right third carpal", BodypartTypeEnum.MinorBone, 40, _compactBone);
			AddBone(organicHumanoid, "l3carpal", "left third carpal", BodypartTypeEnum.MinorBone, 40, _compactBone);
			AddBone(organicHumanoid, "r4carpal", "right fourth carpal", BodypartTypeEnum.MinorBone, 40, _compactBone);
			AddBone(organicHumanoid, "l4carpal", "left fourth carpal", BodypartTypeEnum.MinorBone, 40, _compactBone);
			AddBone(organicHumanoid, "r5carpal", "right fifth carpal", BodypartTypeEnum.MinorBone, 40, _compactBone);
			AddBone(organicHumanoid, "l5carpal", "left fifth carpal", BodypartTypeEnum.MinorBone, 40, _compactBone);
			AddBone(organicHumanoid, "r1metacarpal", "right first metacarpal", BodypartTypeEnum.MinorBone, 40,
				_compactBone);
			AddBone(organicHumanoid, "l1metacarpal", "left first metacarpal", BodypartTypeEnum.MinorBone, 40,
				_compactBone);
			AddBone(organicHumanoid, "r2metacarpal", "right second metacarpal", BodypartTypeEnum.MinorBone, 40,
				_compactBone);
			AddBone(organicHumanoid, "l2metacarpal", "left second metacarpal", BodypartTypeEnum.MinorBone, 40,
				_compactBone);
			AddBone(organicHumanoid, "r3metacarpal", "right third metacarpal", BodypartTypeEnum.MinorBone, 40,
				_compactBone);
			AddBone(organicHumanoid, "l3metacarpal", "left third metacarpal", BodypartTypeEnum.MinorBone, 40,
				_compactBone);
			AddBone(organicHumanoid, "r4metacarpal", "right fourth metacarpal", BodypartTypeEnum.MinorBone, 40,
				_compactBone);
			AddBone(organicHumanoid, "l4metacarpal", "left fourth metacarpal", BodypartTypeEnum.MinorBone, 40,
				_compactBone);
			AddBone(organicHumanoid, "r5metacarpal", "right fifth metacarpal", BodypartTypeEnum.MinorBone, 40,
				_compactBone);
			AddBone(organicHumanoid, "l5metacarpal", "left fifth metacarpal", BodypartTypeEnum.MinorBone, 40,
				_compactBone);
			AddBone(organicHumanoid, "r1manualproximalphalange", "right first manual proximal phalange",
				BodypartTypeEnum.MinorBone, 20,
				_compactBone);
			AddBone(organicHumanoid, "l1manualproximalphalange", "left first manual proximal phalange",
				BodypartTypeEnum.MinorBone, 20,
				_compactBone);
			AddBone(organicHumanoid, "r2manualproximalphalange", "right second manual proximal phalange",
				BodypartTypeEnum.MinorBone, 20,
				_compactBone);
			AddBone(organicHumanoid, "l2manualproximalphalange", "left second manual proximal phalange",
				BodypartTypeEnum.MinorBone, 20,
				_compactBone);
			AddBone(organicHumanoid, "r3manualproximalphalange", "right third manual proximal phalange",
				BodypartTypeEnum.MinorBone, 20,
				_compactBone);
			AddBone(organicHumanoid, "l3manualproximalphalange", "left third manual proximal phalange",
				BodypartTypeEnum.MinorBone, 20,
				_compactBone);
			AddBone(organicHumanoid, "r4manualproximalphalange", "right fourth manual proximal phalange",
				BodypartTypeEnum.MinorBone, 20,
				_compactBone);
			AddBone(organicHumanoid, "l4manualproximalphalange", "left fourth manual proximal phalange",
				BodypartTypeEnum.MinorBone, 20,
				_compactBone);
			AddBone(organicHumanoid, "r5manualproximalphalange", "right fifth manual proximal phalange",
				BodypartTypeEnum.MinorBone, 20,
				_compactBone);
			AddBone(organicHumanoid, "l5manualproximalphalange", "left fifth manual proximal phalange",
				BodypartTypeEnum.MinorBone, 20,
				_compactBone);
			AddBone(organicHumanoid, "r2manualintermediatephalange", "right second manual intermediate phalange",
				BodypartTypeEnum.MinorBone, 20, _compactBone);
			AddBone(organicHumanoid, "l2manualintermediatephalange", "left second manual intermediate phalange",
				BodypartTypeEnum.MinorBone, 20, _compactBone);
			AddBone(organicHumanoid, "r3manualintermediatephalange", "right third manual intermediate phalange",
				BodypartTypeEnum.MinorBone, 20, _compactBone);
			AddBone(organicHumanoid, "l3manualintermediatephalange", "left third manual intermediate phalange",
				BodypartTypeEnum.MinorBone, 20, _compactBone);
			AddBone(organicHumanoid, "r4manualintermediatephalange", "right fourth manual intermediate phalange",
				BodypartTypeEnum.MinorBone, 20, _compactBone);
			AddBone(organicHumanoid, "l4manualintermediatephalange", "left fourth manual intermediate phalange",
				BodypartTypeEnum.MinorBone, 20, _compactBone);
			AddBone(organicHumanoid, "r5manualintermediatephalange", "right fifth manual intermediate phalange",
				BodypartTypeEnum.MinorBone, 20, _compactBone);
			AddBone(organicHumanoid, "l5manualintermediatephalange", "left fifth manual intermediate phalange",
				BodypartTypeEnum.MinorBone, 20, _compactBone);
			AddBone(organicHumanoid, "r1manualdistalphalange", "right first manual distal phalange",
				BodypartTypeEnum.MinorBone, 20,
				_compactBone);
			AddBone(organicHumanoid, "l1manualdistalphalange", "left first manual distal phalange",
				BodypartTypeEnum.MinorBone, 20,
				_compactBone);
			AddBone(organicHumanoid, "r2manualdistalphalange", "right second manual distal phalange",
				BodypartTypeEnum.MinorBone, 20,
				_compactBone);
			AddBone(organicHumanoid, "l2manualdistalphalange", "left second manual distal phalange",
				BodypartTypeEnum.MinorBone, 20,
				_compactBone);
			AddBone(organicHumanoid, "r3manualdistalphalange", "right third manual distal phalange",
				BodypartTypeEnum.MinorBone, 20,
				_compactBone);
			AddBone(organicHumanoid, "l3manualdistalphalange", "left third manual distal phalange",
				BodypartTypeEnum.MinorBone, 20,
				_compactBone);
			AddBone(organicHumanoid, "r4manualdistalphalange", "right fourth manual distal phalange",
				BodypartTypeEnum.MinorBone, 20,
				_compactBone);
			AddBone(organicHumanoid, "l4manualdistalphalange", "left fourth manual distal phalange",
				BodypartTypeEnum.MinorBone, 20,
				_compactBone);
			AddBone(organicHumanoid, "r5manualdistalphalange", "right fifth manual distal phalange",
				BodypartTypeEnum.MinorBone, 20,
				_compactBone);
			AddBone(organicHumanoid, "l5manualdistalphalange", "left fifth manual distal phalange",
				BodypartTypeEnum.MinorBone, 20,
				_compactBone);

			// LEG BONES
			AddBone(organicHumanoid, "rfemur", "right femur", BodypartTypeEnum.Bone, 200, _compactBone);
			AddBone(organicHumanoid, "lfemur", "left femur", BodypartTypeEnum.Bone, 200, _compactBone);
			AddBone(organicHumanoid, "rpatella", "right patella", BodypartTypeEnum.Bone, 90, _compactBone);
			AddBone(organicHumanoid, "lpatella", "left patella", BodypartTypeEnum.Bone, 90, _compactBone);
			AddBone(organicHumanoid, "rtibia", "right tibia", BodypartTypeEnum.Bone, 150, _compactBone);
			AddBone(organicHumanoid, "ltibia", "left tibia", BodypartTypeEnum.Bone, 150, _compactBone);
			AddBone(organicHumanoid, "rfibula", "right fibula", BodypartTypeEnum.Bone, 150, _compactBone);
			AddBone(organicHumanoid, "lfibula", "left fibula", BodypartTypeEnum.Bone, 150, _compactBone);
			AddBone(organicHumanoid, "rcalcaneus", "right calcaneus", BodypartTypeEnum.Bone, 80, _compactBone);
			AddBone(organicHumanoid, "lcalcaneus", "left calcaneus", BodypartTypeEnum.Bone, 80, _compactBone);
			AddBone(organicHumanoid, "rtalus", "right talus", BodypartTypeEnum.Bone, 80, _compactBone);
			AddBone(organicHumanoid, "ltalus", "left talus", BodypartTypeEnum.Bone, 80, _compactBone);
			AddBone(organicHumanoid, "rnavicular", "right navicular", BodypartTypeEnum.Bone, 40, _compactBone);
			AddBone(organicHumanoid, "lnavicular", "left navicular", BodypartTypeEnum.Bone, 40, _compactBone);
			AddBone(organicHumanoid, "rcuboid", "right cuboid", BodypartTypeEnum.Bone, 40, _compactBone);
			AddBone(organicHumanoid, "lcuboid", "left cuboid", BodypartTypeEnum.Bone, 40, _compactBone);
			AddBone(organicHumanoid, "rmedcuneiform", "right medial cuneiform", BodypartTypeEnum.MinorBone, 40,
				_compactBone);
			AddBone(organicHumanoid, "lmedcuneiform", "left medial cuneiform", BodypartTypeEnum.MinorBone, 40,
				_compactBone);
			AddBone(organicHumanoid, "r1metatarsal", "right first metatarsal", BodypartTypeEnum.MinorBone, 40,
				_compactBone);
			AddBone(organicHumanoid, "l1metatarsal", "left first metatarsal", BodypartTypeEnum.MinorBone, 40,
				_compactBone);
			AddBone(organicHumanoid, "r2metatarsal", "right second metatarsal", BodypartTypeEnum.MinorBone, 40,
				_compactBone);
			AddBone(organicHumanoid, "l2metatarsal", "left second metatarsal", BodypartTypeEnum.MinorBone, 40,
				_compactBone);
			AddBone(organicHumanoid, "r3metatarsal", "right third metatarsal", BodypartTypeEnum.MinorBone, 40,
				_compactBone);
			AddBone(organicHumanoid, "l3metatarsal", "left third metatarsal", BodypartTypeEnum.MinorBone, 40,
				_compactBone);
			AddBone(organicHumanoid, "r4metatarsal", "right fourth metatarsal", BodypartTypeEnum.MinorBone, 40,
				_compactBone);
			AddBone(organicHumanoid, "l4metatarsal", "left fourth metatarsal", BodypartTypeEnum.MinorBone, 40,
				_compactBone);
			AddBone(organicHumanoid, "r5metatarsal", "right fifth metatarsal", BodypartTypeEnum.MinorBone, 40,
				_compactBone);
			AddBone(organicHumanoid, "l5metatarsal", "left fifth metatarsal", BodypartTypeEnum.MinorBone, 40,
				_compactBone);
			AddBone(organicHumanoid, "r1pedalproximalphalange", "right first pedal proximal phalange",
				BodypartTypeEnum.MinorBone, 20,
				_compactBone);
			AddBone(organicHumanoid, "l1pedalproximalphalange", "left first pedal proximal phalange",
				BodypartTypeEnum.MinorBone, 20,
				_compactBone);
			AddBone(organicHumanoid, "r2pedalproximalphalange", "right second pedal proximal phalange",
				BodypartTypeEnum.MinorBone, 20,
				_compactBone);
			AddBone(organicHumanoid, "l2pedalproximalphalange", "left second pedal proximal phalange",
				BodypartTypeEnum.MinorBone, 20,
				_compactBone);
			AddBone(organicHumanoid, "r3pedalproximalphalange", "right third pedal proximal phalange",
				BodypartTypeEnum.MinorBone, 20,
				_compactBone);
			AddBone(organicHumanoid, "l3pedalproximalphalange", "left third pedal proximal phalange",
				BodypartTypeEnum.MinorBone, 20,
				_compactBone);
			AddBone(organicHumanoid, "r4pedalproximalphalange", "right fourth pedal proximal phalange",
				BodypartTypeEnum.MinorBone, 20,
				_compactBone);
			AddBone(organicHumanoid, "l4pedalproximalphalange", "left fourth pedal proximal phalange",
				BodypartTypeEnum.MinorBone, 20,
				_compactBone);
			AddBone(organicHumanoid, "r5pedalproximalphalange", "right fifth pedal proximal phalange",
				BodypartTypeEnum.MinorBone, 20,
				_compactBone);
			AddBone(organicHumanoid, "l5pedalproximalphalange", "left fifth pedal proximal phalange",
				BodypartTypeEnum.MinorBone, 20,
				_compactBone);
			AddBone(organicHumanoid, "r2pedalintermediatephalange", "right second pedal intermediate phalange",
				BodypartTypeEnum.MinorBone, 20, _compactBone);
			AddBone(organicHumanoid, "l2pedalintermediatephalange", "left second pedal intermediate phalange",
				BodypartTypeEnum.MinorBone, 20, _compactBone);
			AddBone(organicHumanoid, "r3pedalintermediatephalange", "right third pedal intermediate phalange",
				BodypartTypeEnum.MinorBone, 20, _compactBone);
			AddBone(organicHumanoid, "l3pedalintermediatephalange", "left third pedal intermediate phalange",
				BodypartTypeEnum.MinorBone,
				20, _compactBone);
			AddBone(organicHumanoid, "r4pedalintermediatephalange", "right fourth pedal intermediate phalange",
				BodypartTypeEnum.MinorBone, 20, _compactBone);
			AddBone(organicHumanoid, "l4pedalintermediatephalange", "left fourth pedal intermediate phalange",
				BodypartTypeEnum.MinorBone, 20, _compactBone);
			AddBone(organicHumanoid, "r5pedalintermediatephalange", "right fifth pedal intermediate phalange",
				BodypartTypeEnum.MinorBone, 20, _compactBone);
			AddBone(organicHumanoid, "l5pedalintermediatephalange", "left fifth pedal intermediate phalange",
				BodypartTypeEnum.MinorBone,
				20, _compactBone);
			AddBone(organicHumanoid, "r1pedaldistalphalange", "right first pedal distal phalange",
				BodypartTypeEnum.MinorBone, 20,
				_compactBone);
			AddBone(organicHumanoid, "l1pedaldistalphalange", "left first pedal distal phalange",
				BodypartTypeEnum.MinorBone, 20,
				_compactBone);
			AddBone(organicHumanoid, "r2pedaldistalphalange", "right second pedal distal phalange",
				BodypartTypeEnum.MinorBone, 20,
				_compactBone);
			AddBone(organicHumanoid, "l2pedaldistalphalange", "left second pedal distal phalange",
				BodypartTypeEnum.MinorBone, 20,
				_compactBone);
			AddBone(organicHumanoid, "r3pedaldistalphalange", "right third pedal distal phalange",
				BodypartTypeEnum.MinorBone, 20,
				_compactBone);
			AddBone(organicHumanoid, "l3pedaldistalphalange", "left third pedal distal phalange",
				BodypartTypeEnum.MinorBone, 20,
				_compactBone);
			AddBone(organicHumanoid, "r4pedaldistalphalange", "right fourth pedal distal phalange",
				BodypartTypeEnum.MinorBone, 20,
				_compactBone);
			AddBone(organicHumanoid, "l4pedaldistalphalange", "left fourth pedal distal phalange",
				BodypartTypeEnum.MinorBone, 20,
				_compactBone);
			AddBone(organicHumanoid, "r5pedaldistalphalange", "right fifth pedal distal phalange",
				BodypartTypeEnum.MinorBone, 20,
				_compactBone);
			AddBone(organicHumanoid, "l5pedaldistalphalange", "left fifth pedal distal phalange",
				BodypartTypeEnum.MinorBone, 20,
				_compactBone);
			_context.SaveChanges();

			// InternalInfos

			// TORSO BONES
			AddBoneInternal("sternum", "abdomen", 50);
			AddBoneInternal("rrib1", "rshoulder", 10);
			AddBoneInternal("lrib1", "lshoulder", 10);
			AddBoneInternal("rrib2", "rbreast", 5);
			AddBoneInternal("lrib2", "lbreast", 5);
			AddBoneInternal("rrib3", "rbreast", 5);
			AddBoneInternal("lrib3", "lbreast", 5);
			AddBoneInternal("rrib4", "rbreast", 5);
			AddBoneInternal("lrib4", "lbreast", 5);
			AddBoneInternal("rrib5", "rbreast", 5);
			AddBoneInternal("lrib5", "lbreast", 5);
			AddBoneInternal("rrib6", "rbreast", 5);
			AddBoneInternal("lrib6", "lbreast", 5);
			AddBoneInternal("rrib7", "rbreast", 5);
			AddBoneInternal("lrib7", "lbreast", 5);
			AddBoneInternal("rrib8", "rbreast", 5);
			AddBoneInternal("lrib8", "lbreast", 5);
			AddBoneInternal("rrib9", "rbreast", 5);
			AddBoneInternal("lrib9", "lbreast", 5);
			AddBoneInternal("rrib10", "rbreast", 5);
			AddBoneInternal("lrib10", "lbreast", 5);
			AddBoneInternal("rrib11", "rbreast", 5);
			AddBoneInternal("lrib11", "lbreast", 5);
			AddBoneInternal("rrib12", "rbreast", 5);
			AddBoneInternal("lrib12", "lbreast", 5);
			AddBoneInternal("uspine", "bneck", 35);
			AddBoneInternal("mspine", "uback", 20);
			AddBoneInternal("lspine", "lback", 20);
			AddBoneInternal("sacrum", "lback", 15);
			AddBoneInternal("rilium", "rhip", 50);
			AddBoneInternal("lilium", "lhip", 50);
			AddBoneInternal("rilium", "rbuttock", 30, false);
			AddBoneInternal("lilium", "lbuttock", 30, false);
			AddBoneInternal("rilium", "lback", 4, false);
			AddBoneInternal("lilium", "lback", 4, false);
			AddBoneInternal("rpubis", "groin", 20);
			AddBoneInternal("lpubis", "groin", 20);
			AddBoneInternal("rischium", "rhip", 20);
			AddBoneInternal("lischium", "lhip", 20);

			// HEAD BONES
			AddBoneInternal("parietal", "scalp", 100);
			AddBoneInternal("fcranialbone", "forehead", 100);
			AddBoneInternal("fcranialbone", "face", 20, false);
			AddBoneInternal("occipital", "bhead", 80);
			AddBoneInternal("jawbone", "chin", 100);
			AddBoneInternal("jawbone", "mouth", 35, false);
			AddBoneInternal("jawbone", "rcheek", 15, false);
			AddBoneInternal("jawbone", "lcheek", 15, false);
			AddBoneInternal("rtemporal", "rtemple", 70);
			AddBoneInternal("ltemporal", "ltemple", 70);
			AddBoneInternal("rtemporal", "rear", 70, false);
			AddBoneInternal("ltemporal", "lear", 70, false);
			AddBoneInternal("rtemporal", "reyesocket", 70, false);
			AddBoneInternal("ltemporal", "leyesocket", 70, false);
			AddBoneInternal("maxilla", "nose", 85);
			AddBoneInternal("maxilla", "mouth", 15, false);
			AddBoneInternal("maxilla", "rcheek", 15, false);
			AddBoneInternal("maxilla", "lcheek", 15, false);
			AddBoneInternal("maxilla", "face", 60, false);
			AddBoneInternal("rzygomatic", "rcheek", 50);
			AddBoneInternal("lzygomatic", "lcheek", 50);
			AddBoneInternal("rzygomatic", "face", 50, false);
			AddBoneInternal("lzygomatic", "face", 50, false);

			// ARM BONES
			AddBoneInternal("rclavicle", "rshoulder", 50);
			AddBoneInternal("lclavicle", "lshoulder", 50);
			AddBoneInternal("rscapula", "rshoulderblade", 100);
			AddBoneInternal("lscapula", "lshoulderblade", 100);
			AddBoneInternal("rhumerus", "rupperarm", 50);
			AddBoneInternal("lhumerus", "lupperarm", 50);
			AddBoneInternal("rhumerus", "relbow", 100, false);
			AddBoneInternal("lhumerus", "lelbow", 100, false);
			AddBoneInternal("rradius", "rforearm", 33);
			AddBoneInternal("lradius", "lforearm", 33);
			AddBoneInternal("rulna", "rforearm", 33);
			AddBoneInternal("lulna", "lforearm", 33);
			AddBoneInternal("rradius", "rwrist", 50, false);
			AddBoneInternal("lradius", "lwrist", 50, false);
			AddBoneInternal("rulna", "rwrist", 50, false);
			AddBoneInternal("lulna", "lwrist", 50, false);
			AddBoneInternal("r1carpal", "rhand", 50);
			AddBoneInternal("l1carpal", "lhand", 50);
			AddBoneInternal("r2carpal", "rhand", 50);
			AddBoneInternal("l2carpal", "lhand", 50);
			AddBoneInternal("r3carpal", "rhand", 50);
			AddBoneInternal("l3carpal", "lhand", 50);
			AddBoneInternal("r4carpal", "rhand", 50);
			AddBoneInternal("l4carpal", "lhand", 50);
			AddBoneInternal("r5carpal", "rhand", 50);
			AddBoneInternal("l5carpal", "lhand", 50);
			AddBoneInternal("r1metacarpal", "rhand", 50);
			AddBoneInternal("l1metacarpal", "lhand", 50);
			AddBoneInternal("r2metacarpal", "rhand", 50);
			AddBoneInternal("l2metacarpal", "lhand", 50);
			AddBoneInternal("r3metacarpal", "rhand", 50);
			AddBoneInternal("l3metacarpal", "lhand", 50);
			AddBoneInternal("r4metacarpal", "rhand", 50);
			AddBoneInternal("l4metacarpal", "lhand", 50);
			AddBoneInternal("r5metacarpal", "rhand", 50);
			AddBoneInternal("l5metacarpal", "lhand", 50);
			AddBoneInternal("r1manualproximalphalange", "rthumb", 50);
			AddBoneInternal("l1manualproximalphalange", "lthumb", 50);
			AddBoneInternal("r2manualproximalphalange", "rindexfinger", 50);
			AddBoneInternal("l2manualproximalphalange", "lindexfinger", 50);
			AddBoneInternal("r3manualproximalphalange", "rmiddlefinger", 50);
			AddBoneInternal("l3manualproximalphalange", "lmiddlefinger", 50);
			AddBoneInternal("r4manualproximalphalange", "rringfinger", 50);
			AddBoneInternal("l4manualproximalphalange", "lringfinger", 50);
			AddBoneInternal("r5manualproximalphalange", "rpinkyfinger", 50);
			AddBoneInternal("l5manualproximalphalange", "lpinkyfinger", 50);
			AddBoneInternal("r2manualintermediatephalange", "rindexfinger", 50);
			AddBoneInternal("l2manualintermediatephalange", "lindexfinger", 50);
			AddBoneInternal("r3manualintermediatephalange", "rmiddlefinger", 50);
			AddBoneInternal("l3manualintermediatephalange", "lmiddlefinger", 50);
			AddBoneInternal("r4manualintermediatephalange", "rringfinger", 50);
			AddBoneInternal("l4manualintermediatephalange", "lringfinger", 50);
			AddBoneInternal("r5manualintermediatephalange", "rpinkyfinger", 50);
			AddBoneInternal("l5manualintermediatephalange", "lpinkyfinger", 50);
			AddBoneInternal("r1manualdistalphalange", "rthumb", 50);
			AddBoneInternal("l1manualdistalphalange", "lthumb", 50);
			AddBoneInternal("r2manualdistalphalange", "rindexfinger", 50);
			AddBoneInternal("l2manualdistalphalange", "lindexfinger", 50);
			AddBoneInternal("r3manualdistalphalange", "rmiddlefinger", 50);
			AddBoneInternal("l3manualdistalphalange", "lmiddlefinger", 50);
			AddBoneInternal("r4manualdistalphalange", "rringfinger", 50);
			AddBoneInternal("l4manualdistalphalange", "lringfinger", 50);
			AddBoneInternal("r5manualdistalphalange", "rpinkyfinger", 50);
			AddBoneInternal("l5manualdistalphalange", "lpinkyfinger", 50);

			// LEG BONES
			AddBoneInternal("rfemur", "rthigh", 50);
			AddBoneInternal("lfemur", "lthigh", 50);
			AddBoneInternal("rfemur", "rthighback", 33, false);
			AddBoneInternal("lfemur", "lthighback", 33, false);
			AddBoneInternal("rpatella", "rknee", 100);
			AddBoneInternal("lpatella", "lknee", 100);
			AddBoneInternal("rpatella", "rkneeback", 50, false);
			AddBoneInternal("lpatella", "lkneeback", 50, false);
			AddBoneInternal("rtibia", "rshin", 100);
			AddBoneInternal("ltibia", "lshin", 100);
			AddBoneInternal("rfibula", "rcalf", 33);
			AddBoneInternal("lfibula", "lcalf", 33);
			AddBoneInternal("rcalcaneus", "rheel", 100);
			AddBoneInternal("lcalcaneus", "lheel", 100);
			AddBoneInternal("rtalus", "rankle", 100);
			AddBoneInternal("ltalus", "lankle", 100);
			AddBoneInternal("rnavicular", "rfoot", 50);
			AddBoneInternal("lnavicular", "lfoot", 50);
			AddBoneInternal("rmedcuneiform", "rfoot", 50);
			AddBoneInternal("lmedcuneiform", "lfoot", 50);
			AddBoneInternal("rcuboid", "rfoot", 50);
			AddBoneInternal("lcuboid", "lfoot", 50);
			AddBoneInternal("r1metatarsal", "rfoot", 50);
			AddBoneInternal("l1metatarsal", "lfoot", 50);
			AddBoneInternal("r2metatarsal", "rfoot", 50);
			AddBoneInternal("l2metatarsal", "lfoot", 50);
			AddBoneInternal("r3metatarsal", "rfoot", 50);
			AddBoneInternal("l3metatarsal", "lfoot", 50);
			AddBoneInternal("r4metatarsal", "rfoot", 50);
			AddBoneInternal("l4metatarsal", "lfoot", 50);
			AddBoneInternal("r5metatarsal", "rfoot", 50);
			AddBoneInternal("l5metatarsal", "lfoot", 50);
			AddBoneInternal("r1pedalproximalphalange", "rbigtoe", 50);
			AddBoneInternal("l1pedalproximalphalange", "lbigtoe", 50);
			AddBoneInternal("r2pedalproximalphalange", "rindextoe", 50);
			AddBoneInternal("l2pedalproximalphalange", "lindextoe", 50);
			AddBoneInternal("r3pedalproximalphalange", "rmiddletoe", 50);
			AddBoneInternal("l3pedalproximalphalange", "lmiddletoe", 50);
			AddBoneInternal("r4pedalproximalphalange", "rringtoe", 50);
			AddBoneInternal("l4pedalproximalphalange", "lringtoe", 50);
			AddBoneInternal("r5pedalproximalphalange", "rpinkytoe", 50);
			AddBoneInternal("l5pedalproximalphalange", "lpinkytoe", 50);
			AddBoneInternal("r2pedalintermediatephalange", "rindextoe", 50);
			AddBoneInternal("l2pedalintermediatephalange", "lindextoe", 50);
			AddBoneInternal("r3pedalintermediatephalange", "rmiddletoe", 50);
			AddBoneInternal("l3pedalintermediatephalange", "lmiddletoe", 50);
			AddBoneInternal("r4pedalintermediatephalange", "rringtoe", 50);
			AddBoneInternal("l4pedalintermediatephalange", "lringtoe", 50);
			AddBoneInternal("r5pedalintermediatephalange", "rpinkytoe", 50);
			AddBoneInternal("l5pedalintermediatephalange", "lpinkytoe", 50);
			AddBoneInternal("r1pedaldistalphalange", "rbigtoe", 50);
			AddBoneInternal("l1pedaldistalphalange", "lbigtoe", 50);
			AddBoneInternal("r2pedaldistalphalange", "rindextoe", 50);
			AddBoneInternal("l2pedaldistalphalange", "lindextoe", 50);
			AddBoneInternal("r3pedaldistalphalange", "rmiddletoe", 50);
			AddBoneInternal("l3pedaldistalphalange", "lmiddletoe", 50);
			AddBoneInternal("r4pedaldistalphalange", "rringtoe", 50);
			AddBoneInternal("l4pedaldistalphalange", "lringtoe", 50);
			AddBoneInternal("r5pedaldistalphalange", "rpinkytoe", 50);
			AddBoneInternal("l5pedaldistalphalange", "lpinkytoe", 50);
			_context.SaveChanges();

			// Bone covers

			AddBoneCover("parietal", "brain", 100);
			AddBoneCover("fcranialbone", "brain", 100);
			AddBoneCover("occipital", "brain", 100);
			AddBoneCover("rtemporal", "brain", 90);
			AddBoneCover("ltemporal", "brain", 90);
			AddBoneCover("maxilla", "brain", 90);
			AddBoneCover("rzygomatic", "brain", 100);
			AddBoneCover("lzygomatic", "brain", 100);

			AddBoneCover("uspine", "uspinalcord", 100);
			AddBoneCover("mspine", "mspinalcord", 100);
			AddBoneCover("lspine", "lspinalcord", 100);

			AddBoneCover("sternum", "heart", 80);
			AddBoneCover("sternum", "rlung", 17.5);
			AddBoneCover("sternum", "llung", 17.5);
			AddBoneCover("lrib1", "heart", 5);
			AddBoneCover("lrib2", "heart", 10);
			AddBoneCover("lrib3", "heart", 15);
			AddBoneCover("lrib4", "heart", 15);
			AddBoneCover("lrib5", "heart", 15);
			AddBoneCover("lrib6", "heart", 15);
			AddBoneCover("lrib1", "llung", 10);
			AddBoneCover("lrib2", "llung", 15);
			AddBoneCover("lrib3", "llung", 20);
			AddBoneCover("lrib4", "llung", 20);
			AddBoneCover("lrib5", "llung", 20);
			AddBoneCover("lrib6", "llung", 20);
			AddBoneCover("lrib7", "llung", 20);
			AddBoneCover("rrib1", "rlung", 10);
			AddBoneCover("rrib2", "rlung", 15);
			AddBoneCover("rrib3", "rlung", 20);
			AddBoneCover("rrib4", "rlung", 20);
			AddBoneCover("rrib5", "rlung", 20);
			AddBoneCover("rrib6", "rlung", 20);
			AddBoneCover("rrib7", "rlung", 20);

			AddBoneCover("rrib6", "liver", 30);
			AddBoneCover("rrib7", "liver", 45);
			AddBoneCover("lrib6", "liver", 30);
			AddBoneCover("lrib7", "liver", 45);

			AddBoneCover("lrib8", "liver", 80);
			AddBoneCover("lrib8", "spleen", 25);
			AddBoneCover("rrib8", "liver", 80);
			AddBoneCover("rrib8", "spleen", 25);

			AddBoneCover("lrib9", "liver", 60);
			AddBoneCover("lrib9", "spleen", 20);
			AddBoneCover("rrib9", "liver", 60);
			AddBoneCover("rrib9", "spleen", 20);

			AddBoneCover("lrib10", "liver", 15);
			AddBoneCover("lrib10", "lkidney", 20);
			AddBoneCover("rrib10", "liver", 15);
			AddBoneCover("rrib10", "rkidney", 20);

			AddBoneCover("rscapula", "rlung", 70);
			AddBoneCover("lscapula", "llung", 70);

			AddBoneCover("rilium", "sintestines", 20);
			AddBoneCover("lilium", "sintestines", 20);
			AddBoneCover("rilium", "lintestines", 40);
			AddBoneCover("lilium", "lintestines", 40);

			AddBoneCover("rischium", "lintestines", 40);
			AddBoneCover("lischium", "lintestines", 40);
			_context.SaveChanges();
		}

		#endregion

		_context.SaveChanges();
		foreach (var (child, parent) in _cachedBodypartUpstreams)
			_context.BodypartProtoBodypartProtoUpstream.Add(new BodypartProtoBodypartProtoUpstream
			{
				Child = child.Id,
				Parent = parent.Id
			});

		_context.SaveChanges();

		#region Limbs

		var limbs = new Dictionary<string, Limb>(StringComparer.OrdinalIgnoreCase);

		void AddLimb(string name, LimbType limbType, string rootPart, double damageThreshold,
			double painThreshold)
		{
			var limb = new Limb
			{
				Name = name,
				LimbType = (int)limbType,
				RootBody = baseHumanoid,
				RootBodypart = _bodyparts[rootPart],
				LimbDamageThresholdMultiplier = damageThreshold,
				LimbPainThresholdMultiplier = painThreshold
			};
			_context.Limbs.Add(limb);
			limbs[name] = limb;
		}

		AddLimb("Torso", LimbType.Torso, "abdomen", 1.0, 1.0);
		AddLimb("Head", LimbType.Head, "neck", 1.0, 1.0);
		AddLimb("Genitals", LimbType.Genitals, "groin", 0.5, 0.5);
		AddLimb("Right Arm", LimbType.Arm, "rupperarm", 0.5, 0.5);
		AddLimb("Left Arm", LimbType.Arm, "lupperarm", 0.5, 0.5);
		AddLimb("Right Leg", LimbType.Leg, "rhip", 0.5, 0.5);
		AddLimb("Left Leg", LimbType.Leg, "lhip", 0.5, 0.5);
		_context.SaveChanges();

		foreach (var limb in limbs.Values)
		{
			foreach (var part in _cachedLimbs[limb.Name])
				_context.LimbsBodypartProto.Add(new LimbBodypartProto { BodypartProto = part, Limb = limb });

			switch (limb.Name)
			{
				case "Torso":
					_context.LimbsSpinalParts.Add(new LimbsSpinalPart
						{ Limb = limb, BodypartProto = _organs["uspinalcord"] });
					break;
				case "Genitals":
				case "Right Arm":
				case "Left Arm":
					_context.LimbsSpinalParts.Add(new LimbsSpinalPart
						{ Limb = limb, BodypartProto = _organs["mspinalcord"] });
					break;
				case "Leg Leg":
				case "Right Leg":
					_context.LimbsSpinalParts.Add(new LimbsSpinalPart
						{ Limb = limb, BodypartProto = _organs["lspinalcord"] });
					break;
			}
		}

		_context.SaveChanges();

		#endregion

		#region Bodypart Group Describers

		void AddBodypartGroupDescriberShape(string describedAs, string comment,
			params (string Shape, int MinCount, int MaxCount)[] includedShapes)
		{
			var describer = new BodypartGroupDescriber
			{
				DescribedAs = describedAs,
				Comment = comment,
				Type = "shape"
			};
			_context.BodypartGroupDescribers.Add(describer);
			foreach (var (shape, minCount, maxCount) in includedShapes)
				describer.BodypartGroupDescribersShapeCount.Add(new BodypartGroupDescribersShapeCount
				{
					Target = _shapes[shape],
					BodypartGroupDescriptionRule = describer,
					MinCount = minCount,
					MaxCount = maxCount
				});

			describer.BodypartGroupDescribersBodyProtos.Add(new BodypartGroupDescribersBodyProtos
				{ BodypartGroupDescriber = describer, BodyProto = baseHumanoid });
		}

		AddBodypartGroupDescriberShape("body", "Torso and optional arms",
			("forearm", 0, 2),
			("upper arm", 0, 2),
			("shoulder", 0, 2),
			("shoulder blade", 0, 2),
			("elbow", 0, 2),
			("wrist", 0, 2),
			("hand", 0, 2),
			("finger", 0, 8),
			("thumb", 0, 2),
			("breast", 2, 2),
			("nipple", 0, 2),
			("upper back", 0, 1),
			("lower back", 0, 1),
			("abdomen", 1, 1),
			("belly", 1, 1),
			("hip", 0, 2),
			("groin", 0, 1),
			("thigh", 0, 2),
			("calf", 0, 2),
			("ankle", 0, 2),
			("foot", 0, 2),
			("toe", 0, 10),
			("knee", 0, 2),
			("knee back", 0, 2),
			("thigh back", 0, 2),
			("buttock", 0, 2),
			("penis", 0, 1),
			("testicles", 0, 1)
		);
		AddBodypartGroupDescriberShape("arms", "2 Full Arms",
			("forearm", 0, 2),
			("upper arm", 2, 2),
			("shoulder", 0, 2),
			("shoulder blade", 0, 2),
			("elbow", 0, 2),
			("wrist", 0, 2),
			("hand", 0, 2),
			("finger", 0, 8),
			("thumb", 0, 2)
		);
		AddBodypartGroupDescriberShape("back", "Back and optional neck",
			("shoulder", 0, 2),
			("shoulder blade", 0, 2),
			("upper back", 1, 1),
			("lower back", 1, 1),
			("neck back", 0, 1),
			("neck", 0, 1)
		);
		AddBodypartGroupDescriberShape("legs", "2 Full Legs",
			("hip", 0, 2),
			("groin", 0, 1),
			("thigh", 2, 2),
			("calf", 0, 2),
			("ankle", 0, 2),
			("foot", 0, 2),
			("shin", 0, 2),
			("heel", 0, 2),
			("toe", 0, 10),
			("knee", 0, 2),
			("knee back", 0, 2),
			("thigh back", 0, 2),
			("buttock", 0, 2),
			("penis", 0, 1),
			("testicles", 0, 1)
		);
		AddBodypartGroupDescriberShape("waist", "Hips, groin, optional lower back",
			("hip", 2, 2),
			("groin", 1, 1),
			("penis", 0, 1),
			("testicles", 0, 1),
			("buttock", 0, 2),
			("lower back", 0, 1)
		);

		AddBodypartGroupDescriberShape("head", "Head and all related",
			("scalp", 1, 1),
			("face", 0, 1),
			("chin", 0, 1),
			("cheek", 0, 2),
			("mouth", 0, 1),
			("tongue", 0, 1),
			("nose", 0, 1),
			("forehead", 0, 1),
			("eye socket", 0, 2),
			("eye", 0, 2),
			("ear", 0, 2),
			("temple", 0, 2),
			("eyebrow", 0, 2),
			("neck", 0, 1),
			("neck back", 0, 1),
			("throat", 0, 1),
			("head back", 0, 1)
		);
		AddBodypartGroupDescriberShape("face", "Face and all related",
			("face", 1, 1),
			("chin", 0, 1),
			("cheek", 0, 2),
			("mouth", 0, 1),
			("tongue", 0, 1),
			("nose", 0, 1),
			("forehead", 0, 1),
			("eye socket", 0, 2),
			("eye", 0, 2),
			("ear", 0, 2),
			("temple", 0, 2),
			("eyebrow", 0, 2)
		);
		AddBodypartGroupDescriberShape("upper legs", "Upper portion of legs",
			("thigh", 2, 2),
			("thigh back", 0, 2),
			("groin", 0, 1),
			("buttock", 0, 2),
			("penis", 0, 1),
			("testicles", 0, 1),
			("hip", 0, 2)
		);
		AddBodypartGroupDescriberShape("chest", "2 or more breasts, optional nipples",
			("breast", 2, 2),
			("nipple", 0, 2)
		);
		AddBodypartGroupDescriberShape("forearms", "2 Forearms", ("forearm", 2, 2));
		AddBodypartGroupDescriberShape("shoulders", "2 Shoulders", ("shoulder", 2, 2), ("shoulder blade", 0, 2));
		AddBodypartGroupDescriberShape("shoulder blades", "2 Shoulder blades", ("shoulder blade", 2, 2));
		AddBodypartGroupDescriberShape("eyes", "Eyes and Eyesockets", ("eye socket", 2, 2), ("eye", 0, 2));
		AddBodypartGroupDescriberShape("thighs", "2 Thighs", ("thigh", 2, 2), ("thigh back", 0, 2));
		AddBodypartGroupDescriberShape("calves", "2 Calves", ("calf", 2, 2));
		AddBodypartGroupDescriberShape("knees", "2 Knees", ("knee", 2, 2), ("knee back", 0, 2));
		AddBodypartGroupDescriberShape("elbows", "2 Elbows", ("elbow", 2, 2));
		AddBodypartGroupDescriberShape("ankles", "2 Ankles", ("ankle", 2, 2));
		AddBodypartGroupDescriberShape("ears", "2 Ears", ("ear", 2, 2));
		AddBodypartGroupDescriberShape("hips", "2 Hips", ("hip", 2, 2));
		AddBodypartGroupDescriberShape("buttocks", "2 Buttocks", ("buttock", 2, 2));
		AddBodypartGroupDescriberShape("wrists", "2 Wrists", ("wrist", 2, 2));
		AddBodypartGroupDescriberShape("fingers", "2 or more Fingers", ("finger", 2, 10), ("thumb", 0, 2));
		AddBodypartGroupDescriberShape("thumbs", "2 or more Thumbs", ("thumb", 2, 2));
		AddBodypartGroupDescriberShape("hands", "2 or more hands and fingers", ("hand", 2, 2), ("finger", 0, 10),
			("thumb", 0, 2), ("wrist", 0, 2));
		AddBodypartGroupDescriberShape("feet", "2 or more feet", ("foot", 2, 2), ("heel", 0, 2), ("ankle", 0, 2),
			("toe", 0, 10));
		AddBodypartGroupDescriberShape("groin", "Groin, genitals", ("groin", 1, 1), ("penis", 0, 1),
			("testicles", 0, 1));
		AddBodypartGroupDescriberShape("heel", "2 or more Heels", ("heel", 2, 2));
		AddBodypartGroupDescriberShape("shins", "2 or more Shins", ("shin", 2, 2));
		AddBodypartGroupDescriberShape("temple", "2 or more Temples", ("temple", 2, 2));
		AddBodypartGroupDescriberShape("cheeks", "2 or more Cheeks", ("cheek", 2, 2));
		AddBodypartGroupDescriberShape("eyebrows", "2 Eyebrows", ("eyebrow", 2, 2));
		_context.SaveChanges();

		void AddBodypartGroupDescriberDirect(string describedAs, string comment,
			params (string Part, bool Mandatory)[] includedParts)
		{
			var describer = new BodypartGroupDescriber
			{
				DescribedAs = describedAs,
				Comment = comment,
				Type = "bodypart"
			};
			_context.BodypartGroupDescribers.Add(describer);
			foreach (var (part, mandatory) in includedParts)
				describer.BodypartGroupDescribersBodypartProtos.Add(new BodypartGroupDescribersBodypartProtos
				{
					BodypartGroupDescriber = describer,
					BodypartProto = _bodyparts[part],
					Mandatory = mandatory
				});

			describer.BodypartGroupDescribersBodyProtos.Add(new BodypartGroupDescribersBodyProtos
				{ BodypartGroupDescriber = describer, BodyProto = baseHumanoid });
		}

		AddBodypartGroupDescriberDirect("right hand", "A right hand",
			("rhand", true),
			("rthumb", false),
			("rindexfinger", false),
			("rmiddlefinger", false),
			("rringfinger", false),
			("rpinkyfinger", false),
			("rwrist", false)
		);
		AddBodypartGroupDescriberDirect("left hand", "A left hand",
			("lhand", true),
			("lthumb", false),
			("lindexfinger", false),
			("lmiddlefinger", false),
			("lringfinger", false),
			("lpinkyfinger", false),
			("lwrist", false)
		);
		AddBodypartGroupDescriberDirect("right arm", "A right arm",
			("rshoulderblade", false),
			("rshoulder", false),
			("rupperarm", false),
			("relbow", false),
			("rforearm", true),
			("rhand", false),
			("rthumb", false),
			("rindexfinger", false),
			("rmiddlefinger", false),
			("rringfinger", false),
			("rpinkyfinger", false),
			("rwrist", false)
		);
		AddBodypartGroupDescriberDirect("left arm", "A left arm",
			("lshoulderblade", false),
			("lshoulder", false),
			("lupperarm", false),
			("lelbow", false),
			("lforearm", true),
			("lhand", false),
			("lthumb", false),
			("lindexfinger", false),
			("lmiddlefinger", false),
			("lringfinger", false),
			("lpinkyfinger", false),
			("lwrist", false)
		);
		AddBodypartGroupDescriberDirect("right foot", "A right foot",
			("rfoot", true),
			("rheel", false),
			("rankle", false),
			("rbigtoe", false),
			("rindextoe", false),
			("rmiddletoe", false),
			("rringtoe", false),
			("rpinkytoe", false)
		);
		AddBodypartGroupDescriberDirect("left foot", "A left foot",
			("lfoot", true),
			("lheel", false),
			("lankle", false),
			("lbigtoe", false),
			("lindextoe", false),
			("lmiddletoe", false),
			("lringtoe", false),
			("lpinkytoe", false)
		);
		AddBodypartGroupDescriberDirect("right leg", "A right leg",
			("rthigh", false),
			("rthighback", false),
			("rknee", false),
			("rkneeback", false),
			("rshin", true),
			("rcalf", false),
			("rfoot", false),
			("rheel", false),
			("rankle", false),
			("rbigtoe", false),
			("rindextoe", false),
			("rmiddletoe", false),
			("rringtoe", false),
			("rpinkytoe", false)
		);
		AddBodypartGroupDescriberDirect("left leg", "A left leg",
			("lthigh", false),
			("lthighback", false),
			("lknee", false),
			("lkneeback", false),
			("lshin", true),
			("lcalf", false),
			("lfoot", false),
			("lheel", false),
			("lankle", false),
			("lbigtoe", false),
			("lindextoe", false),
			("lmiddletoe", false),
			("lringtoe", false),
			("lpinkytoe", false)
		);
		_context.SaveChanges();

		#endregion

		#region Wear Profiles

		var addedProfiles = new List<WearProfile>();
		var bulkies = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
		nextId = _context.WearProfiles.Select(x => x.Id).AsEnumerable().DefaultIfEmpty(0).Max() + 1;

		void AddWearProfileDirect(string name, string wearInv, string wear1st, string wear3rd, string wearAffix,
			string description, bool requireContainerIsEmpty, bool bulky,
			params (string Bodypart, int Count, bool Mandatory, bool NoArmour, bool Transparent, bool PreventsRemoval,
				bool HidesSevered)[] bodyparts)
		{
			var profile = new WearProfile
			{
				Id = nextId++,
				BodyPrototypeId = baseHumanoid.Id,
				Name = name,
				WearStringInventory = wearInv,
				WearAction1st = wear1st,
				WearAction3rd = wear3rd,
				WearAffix = wearAffix,
				Description = description,
				RequireContainerIsEmpty = requireContainerIsEmpty,
				Type = "Direct",
				WearlocProfiles = new XElement("Profiles",
					from bodypart in bodyparts
					select new XElement("Profile",
						new XAttribute("Bodypart", bodypart.Bodypart),
						new XAttribute("Transparent", bodypart.Transparent),
						new XAttribute("NoArmour", bodypart.NoArmour),
						new XAttribute("PreventsRemoval", bodypart.PreventsRemoval),
						new XAttribute("Mandatory", bodypart.Mandatory),
						new XAttribute("HidesSevered", bodypart.HidesSevered)
					)
				).ToString()
			};
			_context.WearProfiles.Add(profile);
			addedProfiles.Add(profile);
			bulkies[name] = bulky;
		}

		#region Headwear

		AddWearProfileDirect("Hat", "worn on", "put", "puts", "on", "Worn as a hat or cap covering the scalp only",
			false, false,
			("scalp", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false));

		AddWearProfileDirect("Glasses", "worn over", "put", "puts", "on",
			"Worn as transparent glasses (e.g. eye glasses, goggles)", false, false,
			("reyesocket", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("leyesocket", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("reye", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("leye", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false)
		);
		AddWearProfileDirect("Sunglasses", "worn over", "put", "puts", "on",
			"Worn as non-transparent glasses (e.g. sunglasses)", false, false,
			("reyesocket", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("leyesocket", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("reye", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("leye", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true)
		);
		AddWearProfileDirect("Eyes", "worn in", "slip", "slips", "in", "Worn in the eyes (e.g. contact lenses)", false,
			false,
			("reye", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("leye", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false)
		);
		AddWearProfileDirect("Sack Hood", "worn over", "put", "puts", "on",
			"Worn over the whole head, such as when wearing a sack hood", true, false,
			("reyesocket", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("leyesocket", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("reye", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("leye", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rbrow", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lbrow", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rcheek", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lcheek", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("chin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("nose", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("face", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("mouth", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("forehead", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("tongue", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("scalp", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("bhead", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rtemple", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("ltemple", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rear", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lear", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("neck", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("bneck", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("throat", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true)
		);
		AddWearProfileDirect("Niqab", "worn on", "put", "puts", "on", "A full face covering that shows only the eyes",
			true, false,
			("rcheek", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lcheek", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("chin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("nose", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("face", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("mouth", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("forehead", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("tongue", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("scalp", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("bhead", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rtemple", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("ltemple", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rear", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lear", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("neck", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("bneck", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("throat", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true)
		);
		AddWearProfileDirect("Mask", "worn over", "put", "puts", "on", "Worn over the face as a full-face mask", false,
			true,
			("reyesocket", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("leyesocket", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rbrow", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lbrow", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rcheek", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lcheek", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("chin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("nose", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("face", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("mouth", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("forehead", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("tongue", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true)
		);
		AddWearProfileDirect("Half Mask", "worn over", "put", "puts", "on", "Worn over the face as a half-face mask",
			false, true,
			("reyesocket", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("leyesocket", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rbrow", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lbrow", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rcheek", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lcheek", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("nose", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("face", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("forehead", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true)
		);
		AddWearProfileDirect("Kerchief", "worn over", "put", "puts", "on",
			"Worn over the lower portion of the face, e.g. a kerchief, a cotton medical mask", false, false,
			("rcheek", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lcheek", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("chin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("nose", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("mouth", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("tongue", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true)
		);
		AddWearProfileDirect("Balaclava", "worn over", "put", "puts", "on", "Worn over the face as a balaclava", false,
			false,
			("rcheek", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lcheek", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("chin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("nose", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("face", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("forehead", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("scalp", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("bhead", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rtemple", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("ltemple", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rear", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lear", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("throat", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true)
		);
		AddWearProfileDirect("Skullcap", "worn on", "put", "puts", "on",
			"Worn as a helmet covering only the top parts of the head", false, false,
			("scalp", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("bhead", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false)
		);

		AddWearProfileDirect("Wig", "worn on", "put", "puts", "on", "Worn as a wig", false, false,
			("scalp", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false));

		#endregion

		#region Bodywear

		AddWearProfileDirect("Waist", "worn about", "put", "puts", "on", "Worn about the waist (e.g. a belt)", false,
			false,
			("rhip", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("lhip", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("groin", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("lback", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false)
		);

		AddWearProfileDirect("Shoulders", "worn on", "put", "puts", "on", "Worn on both shoulders", false, false,
			("rshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false)
		);
		AddWearProfileDirect("Mantle", "worn on", "put", "puts", "on", "Worn on both shoulders, chest and upper back",
			false, false,
			("rshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rupperarm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lupperarm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false)
		);
		AddWearProfileDirect("Sleeveless Mantle", "worn on", "put", "puts", "on",
			"Worn on both shoulders, chest, upper arms and upper back", false, false,
			("rshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true)
		);
		AddWearProfileDirect("Cape", "worn on", "put", "puts", "on", "A cape that covers the back", false, false,
			("rshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("uback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rbuttock", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lbuttock", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false)
		);
		AddWearProfileDirect("Cloak (Open)", "worn on", "put", "puts", "on",
			"A cloak that is not wrapped around the wearer's whole body", false, false,
			("rshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("uback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rbuttock", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lbuttock", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rkneeback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lkneeback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rcalf", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lcalf", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false)
		);
		AddWearProfileDirect("Cloak (Closed)", "worn on", "put", "puts", "on",
			"A cloak that is wrapped around the wearer's whole body", false, false,
			("rshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("uback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("abdomen", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("belly", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("groin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("penis", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("testicles", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rbuttock", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbuttock", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rknee", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lknee", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rkneeback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lkneeback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rshin", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshin", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rcalf", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lcalf", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rupperarm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lupperarm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("relbow", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lelbow", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rforearm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lforearm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rwrist", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: true),
			("lwrist", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: true),
			("rhand", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: true),
			("lhand", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: true),
			("rthumb", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: true),
			("lthumb", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: true),
			("rindexfinger", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: true),
			("lindexfinger", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: true),
			("rmiddlefinger", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: true),
			("lmiddlefinger", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: true),
			("rringfinger", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: true),
			("lringfinger", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: true),
			("rpinkyfinger", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: true),
			("lpinkyfinger", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: true)
		);
		AddWearProfileDirect("Scarf", "worn on", "put", "puts", "on", "Worn as a scarf", false, false,
			("neck", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("bneck", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("throat", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false)
		);
		AddWearProfileDirect("Necktie", "worn on", "put", "puts", "on", "Worn as a necktie", false, false,
			("neck", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("bneck", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false)
		);
		AddWearProfileDirect("Bowtie", "worn on", "put", "puts", "on", "Worn as a bowtie", false, false,
			("neck", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("bneck", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false)
		);
		AddWearProfileDirect("Backpack", "worn on", "put", "puts", "on",
			"Worn on the back with straps on the shoulders", false, true,
			("uback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rshoulder", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulder", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("rshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false)
		);
		AddWearProfileDirect("Necklace", "worn on", "put", "puts", "on", "Worn as a necklace", false, false,
			("neck", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("bneck", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false)
		);
		AddWearProfileDirect("Choker", "worn on", "put", "puts", "on", "Worn as a choker necklace", false, false,
			("throat", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("bneck", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false)
		);
		AddWearProfileDirect("Bra", "worn on", "slip", "slips", "on", "Worn as a bra", false, false,
			("rbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rshoulder", 1, Mandatory: true, NoArmour: true, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulder", 1, Mandatory: true, NoArmour: true, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("rshoulderblade", 1, Mandatory: true, NoArmour: true, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulderblade", 1, Mandatory: true, NoArmour: true, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("uback", 1, Mandatory: true, NoArmour: true, Transparent: true, PreventsRemoval: true, HidesSevered: false)
		);
		AddWearProfileDirect("Strapless Bra", "worn on", "slip", "slips", "on", "Worn as a bra with no shoulder straps",
			false, false,
			("rbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rshoulderblade", 1, Mandatory: true, NoArmour: true, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulderblade", 1, Mandatory: true, NoArmour: true, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("uback", 1, Mandatory: true, NoArmour: true, Transparent: true, PreventsRemoval: true, HidesSevered: false)
		);
		AddWearProfileDirect("Bodysuit", "worn on", "slip", "slips", "on",
			"Worn as a bodysuit or single-piece underwear garment", false, false,
			("groin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("penis", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("testicles", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: true),
			("rbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("uback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("belly", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("abdomen", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false)
		);
		AddWearProfileDirect("Bodysuit Thong", "worn on", "slip", "slips", "on",
			"Worn as a bodysuit or single-piece underwear garment with a thong back", false, false,
			("groin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("penis", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("testicles", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: true),
			("lback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("uback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("belly", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("abdomen", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false)
		);
		AddWearProfileDirect("Backless Bodysuit", "worn on", "slip", "slips", "on",
			"Worn as a bodysuit or single-piece underwear garment with no back", false, false,
			("groin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("penis", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("testicles", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: true),
			("rbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("belly", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("abdomen", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false)
		);
		AddWearProfileDirect("Vest", "worn on", "put", "puts", "on", "Worn as a vest or singlet", false, false,
			("lback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("uback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("belly", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("abdomen", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false)
		);
		AddWearProfileDirect("T-Shirt", "worn on", "put", "puts", "on", "Worn as a T-Shirt", false, false,
			("lback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("uback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("belly", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("abdomen", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rupperarm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lupperarm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false)
		);
		AddWearProfileDirect("Shirt", "worn on", "put", "puts", "on", "Worn as a Shirt", false, false,
			("lback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("uback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("belly", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("abdomen", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rupperarm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lupperarm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("relbow", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lelbow", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rforearm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lforearm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rwrist", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lwrist", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false)
		);
		AddWearProfileDirect("Jacket", "worn on", "put", "puts", "on", "Worn as a Jacket", false, true,
			("lback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("uback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("belly", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("abdomen", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rupperarm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lupperarm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("relbow", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lelbow", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rforearm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lforearm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rwrist", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lwrist", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false)
		);
		AddWearProfileDirect("Sleeveless Tunic", "worn on", "put", "puts", "on", "Worn as a Sleeveless Tunic", false,
			false,
			("lback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("uback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("belly", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("abdomen", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("groin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("penis", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: true),
			("testicles", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: true),
			("rthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false)
		);
		AddWearProfileDirect("Tunic", "worn on", "put", "puts", "on", "Worn as a Tunic", false, false,
			("lback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("uback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("belly", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("abdomen", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rupperarm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lupperarm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("groin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("penis", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: true),
			("testicles", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: true),
			("rthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false)
		);
		AddWearProfileDirect("Long-Sleeved Tunic", "worn on", "put", "puts", "on", "Worn as a Long-Sleeved Tunic",
			false, false,
			("lback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("uback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("belly", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("abdomen", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rupperarm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lupperarm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("relbow", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lelbow", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rforearm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lforearm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rwrist", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lwrist", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("groin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("penis", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: true),
			("testicles", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: true),
			("rthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false)
		);
		AddWearProfileDirect("Jumpsuit", "worn on", "put", "puts", "on", "Worn as a full-length jumpsuit", false,
			false,
			("lback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("uback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("belly", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("abdomen", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rupperarm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lupperarm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("relbow", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lelbow", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rforearm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lforearm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rwrist", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lwrist", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("groin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("penis", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("testicles", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rknee", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lknee", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rkneeback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lkneeback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rshin", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshin", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rcalf", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lcalf", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rankle", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lankle", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false)
		);

		#endregion

		#region Legwear

		AddWearProfileDirect("Briefs", "worn on", "slip", "slips", "on", "Worn as briefs", false, false,
			("groin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("penis", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("testicles", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: true),
			("rbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false)
		);
		AddWearProfileDirect("Shorts", "worn on", "slip", "slips", "on", "Worn as shorts", false, false,
			("groin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("penis", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("testicles", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: true),
			("rbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false)
		);
		AddWearProfileDirect("Capris", "worn on", "slip", "slips", "on",
			"Worn as capris, or three-quarter length trousers", false, false,
			("groin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("penis", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("testicles", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: true),
			("rbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rknee", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lknee", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rkneeback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lkneeback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false)
		);
		AddWearProfileDirect("Trousers", "worn on", "slip", "slips", "on", "Worn as trousers", false, false,
			("groin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("penis", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("testicles", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: true),
			("rbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rknee", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lknee", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rkneeback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lkneeback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rcalf", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lcalf", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rshin", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshin", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false)
		);
		AddWearProfileDirect("Chaps", "worn on", "slip", "slips", "on",
			"Chaps are trousers without a 'seat' - i.e. groin or buttocks covering", false, false,
			("rhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rknee", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lknee", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rkneeback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lkneeback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rcalf", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lcalf", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rshin", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshin", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false)
		);
		AddWearProfileDirect("Thong", "worn on", "slip", "slips", "on", "Worn as a thong or g-string", false,
			false,
			("groin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("penis", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("testicles", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: true)
		);
		AddWearProfileDirect("Shoes", "worn on", "put", "puts", "on", "Worn on the feet as shoes", false, true,
			("rfoot", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lfoot", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rheel", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lheel", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbigtoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lbigtoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rindextoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lindextoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rmiddletoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lmiddletoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rringtoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lringtoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rpinkytoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lpinkytoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true)
		);
		AddWearProfileDirect("Sandals", "worn on", "put", "puts", "on", "Worn on the feet as sandals", false,
			true,
			("rfoot", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("lfoot", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("rheel", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("lheel", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("rbigtoe", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("lbigtoe", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("rindextoe", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("lindextoe", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("rmiddletoe", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("lmiddletoe", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("rringtoe", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("lringtoe", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("rpinkytoe", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("lpinkytoe", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false)
		);
		AddWearProfileDirect("Open-Toed Shoes", "worn on", "put", "puts", "on", "Worn on the feet as open-toed shoes",
			false, true,
			("rfoot", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lfoot", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rheel", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lheel", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbigtoe", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("lbigtoe", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("rindextoe", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("lindextoe", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("rmiddletoe", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("lmiddletoe", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("rringtoe", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("lringtoe", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("rpinkytoe", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("lpinkytoe", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false)
		);
		AddWearProfileDirect("Boots", "worn on", "put", "puts", "on", "Worn on the feet as boots", false, true,
			("rankle", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lankle", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rfoot", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lfoot", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rheel", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lheel", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbigtoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lbigtoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rindextoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lindextoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rmiddletoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lmiddletoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rringtoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lringtoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rpinkytoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lpinkytoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true)
		);
		AddWearProfileDirect("High Boots", "worn on", "put", "puts", "on", "Worn on the feet as high boots", false,
			true,
			("rshin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rcalf", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lcalf", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rankle", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lankle", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rfoot", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lfoot", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rheel", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lheel", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbigtoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lbigtoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rindextoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lindextoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rmiddletoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lmiddletoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rringtoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lringtoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rpinkytoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lpinkytoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true)
		);
		AddWearProfileDirect("Socks", "worn on", "put", "puts", "on", "Worn on the feet as socks", false, false,
			("rankle", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lankle", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rfoot", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lfoot", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rheel", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lheel", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbigtoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lbigtoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rindextoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lindextoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rmiddletoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lmiddletoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rringtoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lringtoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rpinkytoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lpinkytoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true)
		);
		AddWearProfileDirect("Ankle Socks", "worn on", "put", "puts", "on",
			"Worn on the feet as socks that end below the ankle", false, false,
			("rfoot", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lfoot", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rheel", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lheel", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbigtoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lbigtoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rindextoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lindextoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rmiddletoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lmiddletoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rringtoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lringtoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rpinkytoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lpinkytoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true)
		);
		AddWearProfileDirect("Long Socks", "worn on", "put", "puts", "on", "Worn on the feet as long socks", false,
			false,
			("rshin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rcalf", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lcalf", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rankle", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lankle", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rfoot", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lfoot", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rheel", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lheel", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbigtoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lbigtoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rindextoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lindextoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rmiddletoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lmiddletoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rringtoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lringtoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rpinkytoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lpinkytoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true)
		);
		AddWearProfileDirect("Stockings", "worn on", "put", "puts", "on", "Worn as full-length stockings", false,
			false,
			("rthigh", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lthigh", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rthighback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lthighback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rknee", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lknee", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rkneeback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lkneeback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rshin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rcalf", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lcalf", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rankle", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lankle", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rfoot", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lfoot", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rheel", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lheel", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbigtoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lbigtoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rindextoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lindextoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rmiddletoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lmiddletoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rringtoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lringtoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rpinkytoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lpinkytoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true)
		);
		AddWearProfileDirect("Knee-High Stockings", "worn on", "put", "puts", "on", "Worn as knee-high stockings",
			false, false,
			("rknee", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lknee", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rkneeback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lkneeback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rshin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rcalf", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lcalf", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rankle", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lankle", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rfoot", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lfoot", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rheel", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lheel", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbigtoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lbigtoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rindextoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lindextoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rmiddletoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lmiddletoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rringtoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lringtoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rpinkytoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lpinkytoe", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true)
		);

		#endregion

		#region Dresses and Skirts

		AddWearProfileDirect("Skirt", "worn on", "slip", "slips", "into", "A skirt that ends below the knees", false,
			false,
			("groin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("penis", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: true),
			("testicles", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: false,
				HidesSevered: true),
			("rbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rknee", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lknee", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rkneeback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lkneeback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false)
		);
		AddWearProfileDirect("Short Skirt", "worn on", "slip", "slips", "into", "A skirt that ends above the knees",
			false, false,
			("groin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("penis", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: true),
			("testicles", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: false,
				HidesSevered: true),
			("rbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false)
		);
		AddWearProfileDirect("Long Skirt", "worn on", "slip", "slips", "on", "A skirt that ends at the ankles", false,
			false,
			("groin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("penis", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: true),
			("testicles", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: false,
				HidesSevered: true),
			("rbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rknee", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lknee", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rkneeback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lkneeback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rshin", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lshin", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rcalf", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lcalf", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false)
		);
		AddWearProfileDirect("Mini Skirt", "worn on", "slip", "slips", "into",
			"A skirt that ends not far below the crotch", false, false,
			("groin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("penis", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: true),
			("testicles", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: false,
				HidesSevered: true),
			("rbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false)
		);
		AddWearProfileDirect("Dress", "worn on", "slip", "slips", "into",
			"A dress with short sleeves that ends below the knees", false, false,
			("groin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("penis", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: true),
			("testicles", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: false,
				HidesSevered: true),
			("rbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rknee", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lknee", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rkneeback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lkneeback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("uback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("belly", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("abdomen", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rupperarm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lupperarm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false)
		);
		AddWearProfileDirect("Gown", "worn on", "slip", "slips", "into",
			"A long dress with short sleeves that ends at the ankles", false, false,
			("groin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("penis", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: true),
			("testicles", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: false,
				HidesSevered: true),
			("rbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rknee", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lknee", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rkneeback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lkneeback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("uback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("belly", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("abdomen", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rupperarm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lupperarm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rshin", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lshin", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rcalf", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lcalf", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false)
		);
		AddWearProfileDirect("Strapless Dress", "worn on", "slip", "slips", "into",
			"A strapless dress that ends below the knees", false, false,
			("groin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("penis", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: true),
			("testicles", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: false,
				HidesSevered: true),
			("rbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rknee", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lknee", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rkneeback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lkneeback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("uback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("belly", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("abdomen", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true)
		);
		AddWearProfileDirect("Strapless Gown", "worn on", "slip", "slips", "into",
			"A strapless long dress that ends at the ankles", false, false,
			("groin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("penis", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: true),
			("testicles", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: false,
				HidesSevered: true),
			("rbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rknee", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lknee", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rkneeback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lkneeback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("uback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("belly", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("abdomen", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rshin", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lshin", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rcalf", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lcalf", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false)
		);
		AddWearProfileDirect("Backless Dress", "worn on", "slip", "slips", "into",
			"A backless dress that ends below the knees", false, false,
			("groin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("penis", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: true),
			("testicles", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: false,
				HidesSevered: true),
			("rbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rknee", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lknee", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rkneeback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lkneeback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("belly", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("abdomen", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true)
		);
		AddWearProfileDirect("Backless Gown", "worn on", "slip", "slips", "into",
			"A backless long dress that ends at the ankles", false, false,
			("groin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("penis", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: true),
			("testicles", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: false,
				HidesSevered: true),
			("rbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rknee", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lknee", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rkneeback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lkneeback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("belly", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("abdomen", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rshin", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lshin", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rcalf", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lcalf", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false)
		);
		AddWearProfileDirect("Sleeveless Dress", "worn on", "slip", "slips", "into",
			"A dress without sleeves that ends below the knees", false, false,
			("groin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("penis", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: true),
			("testicles", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: false,
				HidesSevered: true),
			("rbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rknee", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lknee", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rkneeback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lkneeback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("uback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("belly", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("abdomen", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false)
		);
		AddWearProfileDirect("Sleeveless Gown", "worn on", "slip", "slips", "into",
			"A long dress without sleeves that ends at the ankles", false, false,
			("groin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("penis", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: true),
			("testicles", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: false,
				HidesSevered: true),
			("rbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rknee", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lknee", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rkneeback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lkneeback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("uback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("belly", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("abdomen", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rshin", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lshin", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rcalf", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lcalf", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false)
		);
		AddWearProfileDirect("Long-Sleeved Dress", "worn on", "slip", "slips", "into",
			"A dress with long sleeves that ends below the knees", false, false,
			("groin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("penis", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: true),
			("testicles", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: false,
				HidesSevered: true),
			("rbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rknee", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lknee", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rkneeback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lkneeback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("uback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("belly", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("abdomen", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rupperarm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lupperarm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("relbow", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lelbow", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rforearm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lforearm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false)
		);
		AddWearProfileDirect("Long-Sleeved Gown", "worn on", "slip", "slips", "into",
			"A long dress with long sleeves that ends at the ankles", false, false,
			("groin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("penis", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: true),
			("testicles", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: false,
				HidesSevered: true),
			("rbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rknee", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lknee", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rkneeback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lkneeback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("uback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("belly", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("abdomen", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rupperarm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lupperarm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rshin", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("relbow", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lelbow", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rforearm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lforearm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lshin", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rcalf", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lcalf", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false)
		);

		#endregion

		#region Bulky Items and Armour

		AddWearProfileDirect("Helmet", "worn on", "put", "puts", "on",
			"Worn as a helmet covering the vital parts of the head (e.g. an M1 helmet, kettle helm)", false,
			true,
			("scalp", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("bhead", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("forehead", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rtemple", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("ltemple", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false)
		);
		AddWearProfileDirect("Half Helmet", "worn on", "put", "puts", "on",
			"Worn as a helmet covering the vital parts of the head and the ears (e.g. roman cavalry helmet)", false,
			true,
			("scalp", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("bhead", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("forehead", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rtemple", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("ltemple", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rear", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lear", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false)
		);
		AddWearProfileDirect("Spangenhelm", "worn on", "put", "puts", "on",
			"Worn as a helmet covering the vital parts of the head, ears and cheeks (e.g. a spangenhelm, open face motorcycle helmet)",
			false, true,
			("scalp", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("bhead", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("forehead", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rtemple", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("ltemple", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rcheek", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lcheek", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rear", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lear", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true)
		);
		AddWearProfileDirect("Nasal Helm", "worn on", "put", "puts", "on",
			"Worn as a helmet covering the vital parts of the head and nose (e.g. a phyrgian helmets, roman ridge helmet)",
			false, true,
			("scalp", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("bhead", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("forehead", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rtemple", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("ltemple", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("nose", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false)
		);
		AddWearProfileDirect("Nasal Spangenhelm", "worn on", "put", "puts", "on",
			"Worn as a helmet covering the vital parts of the head, ears, cheeks and nose", false, true,
			("scalp", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("bhead", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("forehead", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rtemple", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("ltemple", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("nose", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rcheek", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lcheek", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rear", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lear", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true)
		);
		AddWearProfileDirect("Greathelm", "worn on", "put", "puts", "on",
			"Worn as a helmet that covers the entire head", false, true,
			("scalp", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("face", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("bhead", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("forehead", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rtemple", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("ltemple", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("nose", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rcheek", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lcheek", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rear", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lear", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("reyesocket", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("leyesocket", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("reye", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: true),
			("leye", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: true),
			("chin", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("rbrow", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("lbrow", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false)
		);
		AddWearProfileDirect("Armet", "worn on", "put", "puts", "on",
			"Worn as a helmet that covers the entire head and neck", false, true,
			("scalp", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("face", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("bhead", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("forehead", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rtemple", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("ltemple", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("nose", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rcheek", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lcheek", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rear", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lear", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("reyesocket", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("leyesocket", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("reye", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: true),
			("leye", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: true),
			("chin", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("rbrow", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("lbrow", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("neck", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("bneck", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("throat", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false)
		);
		AddWearProfileDirect("Corinthian Helmet", "worn on", "put", "puts", "on",
			"Worn as a helmet that covers the entire head and front of neck, but leaves eyes and back of neck exposed",
			false, true,
			("scalp", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("face", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("bhead", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("forehead", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rtemple", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("ltemple", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("nose", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rcheek", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lcheek", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rear", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lear", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("reyesocket", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("leyesocket", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("chin", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("rbrow", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("lbrow", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("neck", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false)
		);
		AddWearProfileDirect("Coif", "worn on", "put", "puts", "on",
			"Worn as a coif that covers the head, neck and ears but leaves the face exposed", false, false,
			("scalp", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("bhead", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("forehead", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rtemple", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("ltemple", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rear", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lear", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("neck", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("bneck", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("throat", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false)
		);
		AddWearProfileDirect("Aventail Spangenhelm", "worn on", "put", "puts", "on",
			"Worn as a helmet covering the vital parts of the head, ears and cheeks with an aventail attachment", false,
			true,
			("scalp", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("bhead", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("forehead", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rtemple", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("ltemple", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rcheek", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lcheek", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rear", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lear", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("neck", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("bneck", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("throat", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false)
		);
		AddWearProfileDirect("Doublet", "worn on", "put", "puts", "on", "Worn as a doublet", false, true,
			("lback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("uback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("belly", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("abdomen", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rupperarm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lupperarm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("groin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("penis", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: true),
			("testicles", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: true)
		);
		AddWearProfileDirect("Jerkin", "worn on", "put", "puts", "on", "Worn as a jerkin", false, true,
			("lback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("uback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("belly", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("abdomen", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("groin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("penis", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: true),
			("testicles", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: true)
		);
		AddWearProfileDirect("Cuirass", "worn on", "put", "puts", "on", "Worn as a cuirass", false, true,
			("lback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("uback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("belly", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("abdomen", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false)
		);
		AddWearProfileDirect("Faulded Cuirass", "worn on", "put", "puts", "on",
			"Worn as a faulded cuirass (with hip protection)", false, true,
			("lback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("uback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("belly", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("abdomen", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false)
		);
		AddWearProfileDirect("Culeted Cuirass", "worn on", "put", "puts", "on",
			"Worn as a culeted cuirass (with hip and backside protection)", false, true,
			("lback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("uback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("belly", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("abdomen", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("rbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false),
			("lbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: false,
				HidesSevered: false)
		);
		AddWearProfileDirect("Hauberk", "worn on", "put", "puts", "on", "Worn as a hauberk", false, true,
			("lback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("uback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("belly", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("abdomen", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rupperarm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lupperarm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("relbow", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lelbow", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rforearm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lforearm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("groin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("penis", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("testicles", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true)
		);
		AddWearProfileDirect("Haubergeon", "worn on", "put", "puts", "on", "Worn as a haubergeon", false, true,
			("lback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("uback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("belly", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("abdomen", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rupperarm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lupperarm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("relbow", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lelbow", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rforearm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lforearm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rknee", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lknee", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rkneeback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lkneeback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("groin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("penis", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("testicles", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true)
		);
		AddWearProfileDirect("Bevor", "worn on", "put", "puts", "on", "Armour that covers neck, throat, chin", false,
			true,
			("neck", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("throat", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("chin", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("bneck", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false)
		);
		AddWearProfileDirect("Gorget", "worn on", "put", "puts", "on", "Armour that covers neck and throat", false,
			true,
			("neck", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("throat", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("bneck", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false)
		);
		AddWearProfileDirect("Pixane", "worn on", "put", "puts", "on",
			"Armour that covers the shoulders, breast and upper back", false, true,
			("uback", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbreast", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lnipple", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false)
		);
		AddWearProfileDirect("Spaulders", "worn on", "put", "puts", "on", "Armour for the shoulders", false, true,
			("rshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false)
		);
		AddWearProfileDirect("Pauldrons", "worn on", "put", "puts", "on",
			"Armour for the shoulders that also covers the upper arm", false, true,
			("rshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lshoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshoulderblade", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rupperarm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lupperarm", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false)
		);
		AddWearProfileDirect("Brassarts", "worn on", "put", "puts", "on", "Armour for the upper arm", false, true,
			("rupperarm", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lupperarm", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false)
		);
		AddWearProfileDirect("Vambraces", "worn on", "put", "puts", "on", "Armour for the forearm, also known as Bracer",
			false, true,
			("rforearm", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lforearm", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false)
		);
		AddWearProfileDirect("Couters", "worn on", "put", "puts", "on", "Armour for the elbows", false, true,
			("relbow", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lelbow", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false)
		);
		AddWearProfileDirect("Gauntlets", "worn on", "put", "puts", "on", "Armour for the hands and wrists", false,
			true,
			("rhand", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lhand", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rwrist", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lwrist", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rthumb", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lthumb", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rindexfinger", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lindexfinger", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rmiddlefinger", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lmiddlefinger", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rringfinger", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lringfinger", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("rpinkyfinger", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("lpinkyfinger", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true)
		);

		AddWearProfileDirect("Chausses", "worn on", "slip", "slips", "on", "Worn as chausses, armoured trousers", false,
			true,
			("groin", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lhip", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("penis", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: true),
			("testicles", 1, Mandatory: false, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: true),
			("rbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lbuttock", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lthigh", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rknee", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lknee", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rkneeback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lkneeback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rcalf", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lcalf", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rshin", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshin", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false)
		);
		AddWearProfileDirect("Cuisses", "worn on", "slip", "slips", "on", "Worn as a cuisse, upper leg armour", false,
			true,
			("rthigh", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lthigh", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lthighback", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false)
		);
		AddWearProfileDirect("Poleyns", "worn on", "slip", "slips", "on", "Armour for the knees", false, true,
			("rknee", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lknee", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false)
		);
		AddWearProfileDirect("Greaves", "worn on", "slip", "slips", "on", "Worn as greaves, lower leg armour", false,
			true,
			("rcalf", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lcalf", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rshin", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lshin", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("rankle", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false),
			("lankle", 1, Mandatory: false, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false)
		);

		#endregion

		void AddWearProfileShape(string name, string wearInv, string wear1st, string wear3rd, string wearAffix,
			string description, bool requireContainerIsEmpty, bool bulky,
			params (string Shape, int Count, bool Mandatory, bool NoArmour, bool Transparent, bool PreventsRemoval, bool
				HidesSevered)[] shapes)
		{
			var profile = new WearProfile
			{
				Id = nextId++,
				BodyPrototypeId = baseHumanoid.Id,
				Name = name,
				WearStringInventory = wearInv,
				WearAction1st = wear1st,
				WearAction3rd = wear3rd,
				WearAffix = wearAffix,
				Description = description,
				RequireContainerIsEmpty = requireContainerIsEmpty,
				Type = "Shape",
				WearlocProfiles = new XElement("Profiles",
					from shape in shapes
					select new XElement("Shape",
						new XAttribute("ShapeId", shape.Shape),
						new XAttribute("Count", shape.Count),
						new XAttribute("Transparent", shape.Transparent),
						new XAttribute("NoArmour", shape.NoArmour),
						new XAttribute("PreventsRemoval", shape.PreventsRemoval),
						new XAttribute("Mandatory", shape.Mandatory),
						new XAttribute("HidesSevered", shape.HidesSevered)
					)
				).ToString()
			};
			_context.WearProfiles.Add(profile);
			addedProfiles.Add(profile);
			bulkies[name] = bulky;
		}

		AddWearProfileShape("Shoulder", "slung over", "sling", "slings", "over", "Slung over one shoulder", false,
			false,
			("shoulder", 1, Mandatory: true, NoArmour: false, Transparent: false, PreventsRemoval: true,
				HidesSevered: false));
		AddWearProfileShape("Earrings", "worn in", "put", "puts", "on",
			"A pair of earrings inserted into an ear piercing", false, false,
			("ear", 2, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false));
		AddWearProfileShape("Ring", "worn on", "put", "puts", "on", "Worn on a finger", false,
			false,
			("finger", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false));
		AddWearProfileShape("Nose Ring", "worn in", "put", "puts", "on", "Inserted into a nose piercing", false,
			false,
			("nose", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false));
		AddWearProfileShape("Earring", "worn in", "put", "puts", "on", "A single earring inserted into an ear piercing",
			false, false,
			("ear", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false));
		AddWearProfileShape("Brow Ring", "worn in", "put", "puts", "on", "Inserted into a brow piercing", false,
			false,
			("eyebrow", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false));
		AddWearProfileShape("Lip Ring", "worn in", "put", "puts", "on", "Inserted into a brow piercing", false,
			false,
			("eyebrow", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false));
		AddWearProfileShape("Nipple Ring", "worn in", "put", "puts", "on", "Inserted into a nipple piercing", false,
			false,
			("nipple", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false));
		AddWearProfileShape("Penis Ring", "worn in", "put", "puts", "on", "Inserted into a penis piercing", false,
			false,
			("penis", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false));
		AddWearProfileShape("Bellybutton Ring", "worn in", "put", "puts", "on", "Inserted into a bellybutton piercing",
			false, false,
			("belly", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false));
		AddWearProfileShape("Tongue Ring", "worn in", "put", "puts", "on", "Inserted into a tongue piercing", false,
			false,
			("tongue", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false));
		AddWearProfileShape("Bracelets", "worn on", "put", "puts", "on", "A pair of bracelets", false, false,
			("wrist", 2, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false));
		AddWearProfileShape("Bracelet", "worn on", "put", "puts", "on", "A single bracelet", false, false,
			("wrist", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false));
		AddWearProfileShape("Bracer", "worn on", "put", "puts", "on", "A single bracer", false, false,
			("wrist", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false));
		AddWearProfileShape("Bracers", "worn on", "put", "puts", "on", "A pair of bracers", false, false,
			("wrist", 2, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false));
		AddWearProfileShape("Anklets", "worn on", "put", "puts", "on", "A pair of anklets", false, false,
			("ankle", 2, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false));
		AddWearProfileShape("Anklet", "worn on", "put", "puts", "on", "A single anklet", false, false,
			("ankle", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false));
		AddWearProfileShape("Epaulette", "affixed to", "affix", "affixes", "on", "An epaulette affixed to the shoulder",
			false, false,
			("shoulder", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false));
		AddWearProfileShape("Bound Hands", "tied around", "put", "puts", "on",
			"Designed for use with a bindable, e.g. rope, duct tape, etc", false, false,
			("wrist", 2, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false));
		AddWearProfileShape("Bound Feet", "tied around", "put", "puts", "on",
			"Designed for use with a bindable, e.g. rope, duct tape, etc", false, false,
			("foot", 2, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false));
		AddWearProfileShape("Bound Knees", "tied around", "put", "puts", "on",
			"Designed for use with a bindable, e.g. rope, duct tape, etc", false, false,
			("knee", 2, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false));
		AddWearProfileShape("Bound Torso", "tied around", "put", "puts", "on",
			"Designed for use with a bindable, e.g. rope, duct tape, etc", false, false,
			("abdomen", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("upper back", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false));
		AddWearProfileShape("Bound Head", "tied around", "put", "puts", "on",
			"Designed for use with a bindable, e.g. rope, duct tape, etc", false, false,
			("forehead", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false),
			("head back", 1, Mandatory: true, NoArmour: false, Transparent: true, PreventsRemoval: true,
				HidesSevered: false));

		_context.SaveChanges();
		var now = DateTime.UtcNow;
		var dbaccount = _context.Accounts.First();
		var id = _context.GameItemComponentProtos.Max(x => x.Id) + 1;
		foreach (var profile in addedProfiles)
		{
			var component = new GameItemComponentProto
			{
				Id = id++,
				RevisionNumber = 0,
				Name = $"Wear_{profile.Name.Replace(' ', '_')}",
				Description = $"Permits the item to be worn in the {profile.Name} wear configuration",
				Type = "Wearable",
				Definition =
					$"<Definition DisplayInventoryWhenWorn=\"true\" Bulky=\"{bulkies[profile.Name]}\"><Profiles Default=\"{profile.Id}\"><Profile>{profile.Id}</Profile></Profiles></Definition>"
			};
			component.EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			};
			_context.GameItemComponentProtos.Add(component);
		}

		#endregion

		_context.SaveChanges();
	}

	public void AddBone(BodyProto body, string alias, string description, BodypartTypeEnum type, int hitPoints,
		Material material,
		SizeCategory size = SizeCategory.Small)
	{
		var bone = new BodypartProto
		{
			Name = alias,
			Body = body,
			Description = description,
			BodypartType = (int)type,
			MaxLife = hitPoints,
			DefaultMaterial = material,
			Size = (int)size,
			RelativeHitChance = 0,
			StunModifier = 0,
			Location = (int)Orientation.Irrelevant,
			Alignment = (int)Alignment.Irrelevant,
			BodypartShape = _shapes["bone"],
			HypoxiaDamagePerTick = 0,
			BleedModifier = 0,
			DamageModifier = 1.0,
			PainModifier = 1.0,
			RelativeInfectability = 0.0,
			DisplayOrder = 1,
			SeveredThreshold = -1,
			IsCore = true,
			IsOrgan = 0,
			IsVital = false,
			ArmourType = _boneArmour
		};
		_context.BodypartProtos.Add(bone);
		_bones[alias] = bone;
	}

	private void AddBoneInternal(string whichBone, string whichBodypart, int hitChance, bool isPrimary = true)
	{
		_context.BodypartInternalInfos.Add(new BodypartInternalInfos
		{
			BodypartProto = _bodyparts[whichBodypart],
			InternalPart = _bones[whichBone],
			HitChance = hitChance,
			IsPrimaryOrganLocation = isPrimary
		});
	}

	private void AddBoneCover(string bone, string organ, double coverage)
	{
		_context.BoneOrganCoverages.Add(new BoneOrganCoverage
		{
			Bone = _bones[bone],
			Organ = _organs[organ],
			CoverageChance = coverage
		});
	}

	private void AddOrganCoverage(string whichOrgan, string whichBodypart, int hitChance, bool isPrimary = false)
	{
		_context.BodypartInternalInfos.Add(new BodypartInternalInfos
		{
			BodypartProto = _bodyparts[whichBodypart],
			InternalPart = _organs[whichOrgan],
			HitChance = hitChance,
			IsPrimaryOrganLocation = isPrimary
		});
	}
}
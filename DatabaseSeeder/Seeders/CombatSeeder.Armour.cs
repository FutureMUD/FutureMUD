#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Body.Traits;
using MudSharp.Body;
using MudSharp.Combat;
using MudSharp.Database;
using MudSharp.Form.Audio;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System;

namespace DatabaseSeeder.Seeders;

public partial class CombatSeeder
{
    private void SeedArmourTypes(FuturemudDatabaseContext context,
        IReadOnlyDictionary<string, string> questionAnswers)
    {
        Account dbaccount = context.Accounts.First();
        DateTime now = DateTime.UtcNow;

        void AddArmourType(string name, int penetration, int baseDifficulty, int stackedDifficulty, double powerDamage,
            double powerPain, double powerStun, IEnumerable<DamageType> strongTypes, IEnumerable<DamageType> weakTypes,
			IEnumerable<DamageType> zeroTypes, IEnumerable<DamageType>? superTypes = null)
        {
            if (superTypes == null)
            {
                superTypes = Enumerable.Empty<DamageType>();
            }

            ArmourType armour = new()
            {
                Name = name,
                MinimumPenetrationDegree = penetration,
                BaseDifficultyDegrees = baseDifficulty,
                StackedDifficultyDegrees = stackedDifficulty,
                Definition = @$"<ArmourType>

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
		<Expression damagetype=""0"">damage{ArmourDissipateModifier(DamageType.Slashing, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Slashing -->
		<Expression damagetype=""1"">damage{ArmourDissipateModifier(DamageType.Chopping, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Chopping -->  
		<Expression damagetype=""2"">damage{ArmourDissipateModifier(DamageType.Crushing, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Crushing -->  
		<Expression damagetype=""3"">damage{ArmourDissipateModifier(DamageType.Piercing, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Piercing -->  
		<Expression damagetype=""4"">damage{ArmourDissipateModifier(DamageType.Ballistic, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Ballistic -->  
		<Expression damagetype=""5"">damage{ArmourDissipateModifier(DamageType.Burning, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    			      <!-- Burning -->
		<Expression damagetype=""6"">damage{ArmourDissipateModifier(DamageType.Freezing, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>                     <!-- Freezing -->
		<Expression damagetype=""7"">damage{ArmourDissipateModifier(DamageType.Chemical, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>                     <!-- Chemical -->
		<Expression damagetype=""8"">damage{ArmourDissipateModifier(DamageType.Shockwave, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Shockwave -->
		<Expression damagetype=""9"">damage{ArmourDissipateModifier(DamageType.Bite, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Bite -->
		<Expression damagetype=""10"">damage{ArmourDissipateModifier(DamageType.Claw, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Claw -->
		<Expression damagetype=""11"">damage{ArmourDissipateModifier(DamageType.Electrical, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>                    <!-- Electrical -->
		<Expression damagetype=""12"">damage</Expression>                    <!-- Hypoxia -->
		<Expression damagetype=""13"">damage</Expression>                    <!-- Cellular -->
		<Expression damagetype=""14"">damage{ArmourDissipateModifier(DamageType.Sonic, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Sonic -->
		<Expression damagetype=""15"">damage{ArmourDissipateModifier(DamageType.Shearing, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">damage{ArmourDissipateModifier(DamageType.BallisticArmourPiercing, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- BallisticArmourPiercing -->
		<Expression damagetype=""17"">damage{ArmourDissipateModifier(DamageType.Wrenching, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Wrenching -->
		<Expression damagetype=""18"">damage{ArmourDissipateModifier(DamageType.Shrapnel, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">damage{ArmourDissipateModifier(DamageType.Necrotic, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>                    <!-- Necrotic -->   
		<Expression damagetype=""20"">damage{ArmourDissipateModifier(DamageType.Falling, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">damage{ArmourDissipateModifier(DamageType.Eldritch, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>                    <!-- Eldritch -->   
		<Expression damagetype=""22"">damage{ArmourDissipateModifier(DamageType.Arcane, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>                    <!-- Arcane -->   
		<Expression damagetype=""23"">damage{ArmourDissipateModifier(DamageType.ArmourPiercing, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- ArmourPiercing -->
	</DissipateExpressions>
	<DissipateExpressionsPain>
		<Expression damagetype=""0"">pain{ArmourDissipateModifier(DamageType.Slashing, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Slashing -->
		<Expression damagetype=""1"">pain{ArmourDissipateModifier(DamageType.Chopping, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Chopping -->  
		<Expression damagetype=""2"">pain{ArmourDissipateModifier(DamageType.Crushing, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Crushing -->  
		<Expression damagetype=""3"">pain{ArmourDissipateModifier(DamageType.Piercing, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Piercing -->  
		<Expression damagetype=""4"">pain{ArmourDissipateModifier(DamageType.Ballistic, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Ballistic -->  
		<Expression damagetype=""5"">pain{ArmourDissipateModifier(DamageType.Burning, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    			      <!-- Burning -->
		<Expression damagetype=""6"">pain{ArmourDissipateModifier(DamageType.Freezing, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>                     <!-- Freezing -->
		<Expression damagetype=""7"">pain{ArmourDissipateModifier(DamageType.Chemical, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>                     <!-- Chemical -->
		<Expression damagetype=""8"">pain{ArmourDissipateModifier(DamageType.Shockwave, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Shockwave -->
		<Expression damagetype=""9"">pain{ArmourDissipateModifier(DamageType.Bite, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Bite -->
		<Expression damagetype=""10"">pain{ArmourDissipateModifier(DamageType.Claw, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Claw -->
		<Expression damagetype=""11"">pain{ArmourDissipateModifier(DamageType.Electrical, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>                    <!-- Electrical -->
		<Expression damagetype=""12"">pain</Expression>                    <!-- Hypoxia -->
		<Expression damagetype=""13"">pain</Expression>                    <!-- Cellular -->
		<Expression damagetype=""14"">pain{ArmourDissipateModifier(DamageType.Sonic, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Sonic -->
		<Expression damagetype=""15"">pain{ArmourDissipateModifier(DamageType.Shearing, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">pain{ArmourDissipateModifier(DamageType.BallisticArmourPiercing, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">pain{ArmourDissipateModifier(DamageType.Wrenching, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Wrenching -->
		<Expression damagetype=""18"">pain{ArmourDissipateModifier(DamageType.Shrapnel, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">pain{ArmourDissipateModifier(DamageType.Necrotic, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>                    <!-- Necrotic -->   
		<Expression damagetype=""20"">pain{ArmourDissipateModifier(DamageType.Falling, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">pain{ArmourDissipateModifier(DamageType.Eldritch, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>                    <!-- Eldritch -->   
		<Expression damagetype=""22"">pain{ArmourDissipateModifier(DamageType.Arcane, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>                    <!-- Arcane -->   
		<Expression damagetype=""23"">pain{ArmourDissipateModifier(DamageType.ArmourPiercing, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- ArmourPiercing -->
	</DissipateExpressionsPain>  
	<DissipateExpressionsStun>
		<Expression damagetype=""0"">stun{ArmourDissipateModifier(DamageType.Slashing, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Slashing -->
		<Expression damagetype=""1"">stun{ArmourDissipateModifier(DamageType.Chopping, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Chopping -->  
		<Expression damagetype=""2"">stun{ArmourDissipateModifier(DamageType.Crushing, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Crushing -->  
		<Expression damagetype=""3"">stun{ArmourDissipateModifier(DamageType.Piercing, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Piercing -->  
		<Expression damagetype=""4"">stun{ArmourDissipateModifier(DamageType.Ballistic, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Ballistic -->  
		<Expression damagetype=""5"">stun{ArmourDissipateModifier(DamageType.Burning, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    			      <!-- Burning -->
		<Expression damagetype=""6"">stun{ArmourDissipateModifier(DamageType.Freezing, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>                     <!-- Freezing -->
		<Expression damagetype=""7"">stun{ArmourDissipateModifier(DamageType.Chemical, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>                     <!-- Chemical -->
		<Expression damagetype=""8"">stun{ArmourDissipateModifier(DamageType.Shockwave, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Shockwave -->
		<Expression damagetype=""9"">stun{ArmourDissipateModifier(DamageType.Bite, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Bite -->
		<Expression damagetype=""10"">stun{ArmourDissipateModifier(DamageType.Claw, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Claw -->
		<Expression damagetype=""11"">stun{ArmourDissipateModifier(DamageType.Electrical, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>                    <!-- Electrical -->
		<Expression damagetype=""12"">stun</Expression>                    <!-- Hypoxia -->
		<Expression damagetype=""13"">stun</Expression>                    <!-- Cellular -->
		<Expression damagetype=""14"">stun{ArmourDissipateModifier(DamageType.Sonic, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Sonic -->
		<Expression damagetype=""15"">stun{ArmourDissipateModifier(DamageType.Shearing, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">stun{ArmourDissipateModifier(DamageType.BallisticArmourPiercing, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">stun{ArmourDissipateModifier(DamageType.Wrenching, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Wrenching -->
		<Expression damagetype=""18"">stun{ArmourDissipateModifier(DamageType.Shrapnel, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">stun{ArmourDissipateModifier(DamageType.Necrotic, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>                    <!-- Necrotic -->   
		<Expression damagetype=""20"">stun{ArmourDissipateModifier(DamageType.Falling, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">stun{ArmourDissipateModifier(DamageType.Eldritch, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>                    <!-- Eldritch -->   
		<Expression damagetype=""22"">stun{ArmourDissipateModifier(DamageType.Arcane, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>                    <!-- Arcane -->   
		<Expression damagetype=""23"">stun{ArmourDissipateModifier(DamageType.ArmourPiercing, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- ArmourPiercing -->
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
		<Expression damagetype=""0"">damage{ArmourAbsorbModifier(DamageType.Slashing, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Slashing -->
		<Expression damagetype=""1"">damage{ArmourAbsorbModifier(DamageType.Chopping, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Chopping -->  
		<Expression damagetype=""2"">damage{ArmourAbsorbModifier(DamageType.Crushing, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Crushing -->  
		<Expression damagetype=""3"">damage{ArmourAbsorbModifier(DamageType.Piercing, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Piercing -->  
		<Expression damagetype=""4"">damage{ArmourAbsorbModifier(DamageType.Ballistic, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Ballistic -->  
		<Expression damagetype=""5"">damage{ArmourAbsorbModifier(DamageType.Burning, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Burning -->
		<Expression damagetype=""6"">damage{ArmourAbsorbModifier(DamageType.Freezing, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Freezing -->
		<Expression damagetype=""7"">damage{ArmourAbsorbModifier(DamageType.Chemical, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Chemical -->
		<Expression damagetype=""8"">damage{ArmourAbsorbModifier(DamageType.Shockwave, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Shockwave -->
		<Expression damagetype=""9"">damage{ArmourAbsorbModifier(DamageType.Bite, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Bite -->
		<Expression damagetype=""10"">damage{ArmourAbsorbModifier(DamageType.Claw, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Claw -->
		<Expression damagetype=""11"">damage{ArmourAbsorbModifier(DamageType.Electrical, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Electrical -->
		<Expression damagetype=""12"">0</Expression>        <!-- Hypoxia -->
		<Expression damagetype=""13"">0</Expression>        <!-- Cellular -->
		<Expression damagetype=""14"">damage{ArmourAbsorbModifier(DamageType.Sonic, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Sonic -->
		<Expression damagetype=""15"">damage{ArmourAbsorbModifier(DamageType.Shearing, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">damage{ArmourAbsorbModifier(DamageType.BallisticArmourPiercing, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">damage{ArmourAbsorbModifier(DamageType.Wrenching, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Wrenching -->
		<Expression damagetype=""18"">damage{ArmourAbsorbModifier(DamageType.Shrapnel, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">damage{ArmourAbsorbModifier(DamageType.Necrotic, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Necrotic -->   
		<Expression damagetype=""20"">damage{ArmourAbsorbModifier(DamageType.Falling, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">damage{ArmourAbsorbModifier(DamageType.Eldritch, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Eldritch -->   
		<Expression damagetype=""22"">damage{ArmourAbsorbModifier(DamageType.Arcane, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Arcane -->   
		<Expression damagetype=""23"">damage{ArmourAbsorbModifier(DamageType.ArmourPiercing, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- ArmourPiercing -->
	</AbsorbExpressions>  
	<AbsorbExpressionsPain>
		<Expression damagetype=""0"">pain{ArmourAbsorbModifier(DamageType.Slashing, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Slashing -->
		<Expression damagetype=""1"">pain{ArmourAbsorbModifier(DamageType.Chopping, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Chopping -->  
		<Expression damagetype=""2"">pain{ArmourAbsorbModifier(DamageType.Crushing, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Crushing -->  
		<Expression damagetype=""3"">pain{ArmourAbsorbModifier(DamageType.Piercing, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Piercing -->  
		<Expression damagetype=""4"">pain{ArmourAbsorbModifier(DamageType.Ballistic, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Ballistic -->  
		<Expression damagetype=""5"">pain{ArmourAbsorbModifier(DamageType.Burning, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Burning -->
		<Expression damagetype=""6"">pain{ArmourAbsorbModifier(DamageType.Freezing, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Freezing -->
		<Expression damagetype=""7"">pain{ArmourAbsorbModifier(DamageType.Chemical, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Chemical -->
		<Expression damagetype=""8"">pain{ArmourAbsorbModifier(DamageType.Shockwave, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Shockwave -->
		<Expression damagetype=""9"">pain{ArmourAbsorbModifier(DamageType.Bite, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Bite -->
		<Expression damagetype=""10"">pain{ArmourAbsorbModifier(DamageType.Claw, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Claw -->
		<Expression damagetype=""11"">pain{ArmourAbsorbModifier(DamageType.Electrical, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Electrical -->
		<Expression damagetype=""12"">0</Expression>        <!-- Hypoxia -->
		<Expression damagetype=""13"">0</Expression>        <!-- Cellular -->
		<Expression damagetype=""14"">pain{ArmourAbsorbModifier(DamageType.Sonic, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Sonic -->
		<Expression damagetype=""15"">pain{ArmourAbsorbModifier(DamageType.Shearing, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">pain{ArmourAbsorbModifier(DamageType.BallisticArmourPiercing, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">pain{ArmourAbsorbModifier(DamageType.Wrenching, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Wrenching -->
		<Expression damagetype=""18"">pain{ArmourAbsorbModifier(DamageType.Shrapnel, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">pain{ArmourAbsorbModifier(DamageType.Necrotic, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Necrotic -->   
		<Expression damagetype=""20"">pain{ArmourAbsorbModifier(DamageType.Falling, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">pain{ArmourAbsorbModifier(DamageType.Eldritch, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Eldritch -->   
		<Expression damagetype=""22"">pain{ArmourAbsorbModifier(DamageType.Arcane, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Arcane -->   
		<Expression damagetype=""23"">pain{ArmourAbsorbModifier(DamageType.ArmourPiercing, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- ArmourPiercing -->
	</AbsorbExpressionsPain>  
	<AbsorbExpressionsStun>
		<Expression damagetype=""0"">stun{ArmourAbsorbModifier(DamageType.Slashing, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Slashing -->
		<Expression damagetype=""1"">stun{ArmourAbsorbModifier(DamageType.Chopping, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Chopping -->  
		<Expression damagetype=""2"">stun{ArmourAbsorbModifier(DamageType.Crushing, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Crushing -->  
		<Expression damagetype=""3"">stun{ArmourAbsorbModifier(DamageType.Piercing, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Piercing -->  
		<Expression damagetype=""4"">stun{ArmourAbsorbModifier(DamageType.Ballistic, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Ballistic -->  
		<Expression damagetype=""5"">stun{ArmourAbsorbModifier(DamageType.Burning, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Burning -->
		<Expression damagetype=""6"">stun{ArmourAbsorbModifier(DamageType.Freezing, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Freezing -->
		<Expression damagetype=""7"">stun{ArmourAbsorbModifier(DamageType.Chemical, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Chemical -->
		<Expression damagetype=""8"">stun{ArmourAbsorbModifier(DamageType.Shockwave, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Shockwave -->
		<Expression damagetype=""9"">stun{ArmourAbsorbModifier(DamageType.Bite, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>    <!-- Bite -->
		<Expression damagetype=""10"">stun{ArmourAbsorbModifier(DamageType.Claw, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Claw -->
		<Expression damagetype=""11"">stun{ArmourAbsorbModifier(DamageType.Electrical, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Electrical -->
		<Expression damagetype=""12"">0</Expression>        <!-- Hypoxia -->
		<Expression damagetype=""13"">0</Expression>        <!-- Cellular -->
		<Expression damagetype=""14"">stun{ArmourAbsorbModifier(DamageType.Sonic, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Sonic -->
		<Expression damagetype=""15"">stun{ArmourAbsorbModifier(DamageType.Shearing, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">stun{ArmourAbsorbModifier(DamageType.BallisticArmourPiercing, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">stun{ArmourAbsorbModifier(DamageType.Wrenching, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Wrenching -->
		<Expression damagetype=""18"">stun{ArmourAbsorbModifier(DamageType.Shrapnel, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">stun{ArmourAbsorbModifier(DamageType.Necrotic, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Necrotic -->   
		<Expression damagetype=""20"">stun{ArmourAbsorbModifier(DamageType.Falling, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">stun{ArmourAbsorbModifier(DamageType.Eldritch, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Eldritch -->   
		<Expression damagetype=""22"">stun{ArmourAbsorbModifier(DamageType.Arcane, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Arcane -->   
		<Expression damagetype=""23"">stun{ArmourAbsorbModifier(DamageType.ArmourPiercing, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- ArmourPiercing -->
	</AbsorbExpressionsStun>
 </ArmourType>"
            };
            context.ArmourTypes.Add(armour);
            context.SaveChanges();
            GameItemComponentProto component = new()
            {
                Id = context.GameItemComponentProtos.Max(x => x.Id) + 1,
                RevisionNumber = 0,
                EditableItem = new EditableItem
                {
                    RevisionNumber = 0,
                    RevisionStatus = 4,
                    BuilderAccountId = dbaccount.Id,
                    BuilderDate = now,
                    BuilderComment = "Auto-generated by the system",
                    ReviewerAccountId = dbaccount.Id,
                    ReviewerComment = "Auto-generated by the system",
                    ReviewerDate = now
                },
                Type = "Armour",
                Name = $"Armour_{name.CollapseString()}",
                Description = $"Turns an item into {name} armour",
                Definition =
                    $"<Definition><ArmourType>{armour.Id}</ArmourType></Definition>"
            };
            context.GameItemComponentProtos.Add(component);
            context.SaveChanges();
        }

        void AddSpecialArmourType(string name, int penetration, int baseDifficulty, int stackedDifficulty,
            string definition)
        {
            ArmourType armour = new()
            {
                Name = name,
                MinimumPenetrationDegree = penetration,
                BaseDifficultyDegrees = baseDifficulty,
                StackedDifficultyDegrees = stackedDifficulty,
                Definition = definition
            };
            context.ArmourTypes.Add(armour);
            context.SaveChanges();
            GameItemComponentProto component = new()
            {
                Id = context.GameItemComponentProtos.Max(x => x.Id) + 1,
                RevisionNumber = 0,
                EditableItem = new EditableItem
                {
                    RevisionNumber = 0,
                    RevisionStatus = 4,
                    BuilderAccountId = dbaccount.Id,
                    BuilderDate = now,
                    BuilderComment = "Auto-generated by the system",
                    ReviewerAccountId = dbaccount.Id,
                    ReviewerComment = "Auto-generated by the system",
                    ReviewerDate = now
                },
                Type = "Armour",
                Name = $"Armour_{name.CollapseString()}",
                Description = $"Turns an item into {name} armour",
                Definition =
                    $"<Definition><ArmourType>{armour.Id}</ArmourType></Definition>"
            };
            context.GameItemComponentProtos.Add(component);
            context.SaveChanges();
        }

        AddArmourType("Light Clothing", 1, 0, 0, 0.45, 0.75, 0.6,
            new List<DamageType>
            {
                DamageType.Chemical,
                DamageType.Freezing,
                DamageType.Burning
            },
            new List<DamageType>
            {
                DamageType.Piercing,
                DamageType.Chopping,
                DamageType.Slashing,
                DamageType.Ballistic,
                DamageType.Bite
            },
            new List<DamageType>
            {
                DamageType.BallisticArmourPiercing,
                DamageType.ArmourPiercing,
                DamageType.Arcane,
                DamageType.Necrotic,
                DamageType.Eldritch
            }
        );
        AddArmourType("Heavy Clothing", 1, 0, 1, 0.9, 1.2, 0.9,
            new List<DamageType>
            {
                DamageType.Chemical,
                DamageType.Freezing,
                DamageType.Burning,
                DamageType.Electrical,
                DamageType.Crushing,
                DamageType.Shockwave,
                DamageType.Falling
            },
            new List<DamageType>
            {
                DamageType.Piercing,
                DamageType.Chopping,
                DamageType.Slashing,
                DamageType.Ballistic,
                DamageType.Bite
            },
            new List<DamageType>
            {
                DamageType.BallisticArmourPiercing,
                DamageType.ArmourPiercing,
                DamageType.Arcane,
                DamageType.Necrotic,
                DamageType.Eldritch
            }
        );
        AddArmourType("Ultra Heavy Clothing", 1, 1, 2, 1.1, 2.5, 1.5,
            new List<DamageType>
            {
                DamageType.Chemical,
                DamageType.Freezing,
                DamageType.Burning,
                DamageType.Electrical,
                DamageType.Crushing,
                DamageType.Shockwave,
                DamageType.Falling
            },
            new List<DamageType>
            {
                DamageType.Piercing,
                DamageType.Chopping,
                DamageType.Slashing,
                DamageType.Ballistic,
                DamageType.Bite
            },
            new List<DamageType>
            {
                DamageType.BallisticArmourPiercing,
                DamageType.ArmourPiercing,
                DamageType.Arcane,
                DamageType.Necrotic,
                DamageType.Eldritch
            }
        );
        AddArmourType("Level I Ballistic Armour", 1, 1, 2, 3.0, 3.0, 3.5,
            new List<DamageType>
            {
                DamageType.Chemical,
                DamageType.Freezing,
                DamageType.Burning,
                DamageType.Piercing,
                DamageType.Ballistic,
                DamageType.Shrapnel
            },
            new List<DamageType>
            {
                DamageType.BallisticArmourPiercing,
                DamageType.ArmourPiercing,
            },
            new List<DamageType>
            {
                DamageType.Arcane,
                DamageType.Necrotic,
                DamageType.Eldritch
            }
        );
        AddArmourType("Level IIa Ballistic Armour", 1, 1, 2, 3.75, 4.0, 5.0,
            new List<DamageType>
            {
                DamageType.Chemical,
                DamageType.Freezing,
                DamageType.Burning,
                DamageType.Piercing,
                DamageType.Ballistic,
                DamageType.Shrapnel
            },
            new List<DamageType>
            {
                DamageType.BallisticArmourPiercing,
                DamageType.ArmourPiercing,
            },
            new List<DamageType>
            {
                DamageType.Arcane,
                DamageType.Necrotic,
                DamageType.Eldritch
            }
        );
        AddArmourType("Level II Ballistic Armour", 1, 1, 2, 4.5, 5.5, 6.6,
            new List<DamageType>
            {
                DamageType.Chemical,
                DamageType.Freezing,
                DamageType.Burning,
                DamageType.Piercing,
                DamageType.Ballistic,
                DamageType.Shrapnel
            },
            new List<DamageType>
            {
                DamageType.BallisticArmourPiercing,
                DamageType.ArmourPiercing,
            },
            new List<DamageType>
            {
                DamageType.Arcane,
                DamageType.Necrotic,
                DamageType.Eldritch
            }
        );
        AddArmourType("Level IIIa Ballistic Armour", 1, 1, 2, 5.0, 7.0, 7.5,
            new List<DamageType>
            {
                DamageType.Chemical,
                DamageType.Freezing,
                DamageType.Burning,
                DamageType.Piercing,
                DamageType.Ballistic,
                DamageType.Shrapnel
            },
            new List<DamageType>
            {
                DamageType.BallisticArmourPiercing,
                DamageType.ArmourPiercing,
            },
            new List<DamageType>
            {
                DamageType.Arcane,
                DamageType.Necrotic,
                DamageType.Eldritch
            }
        );
        AddArmourType("Level III Ballistic Armour", 1, 2, 3, 5.5, 7.5, 8.0,
            new List<DamageType>
            {
                DamageType.Chemical,
                DamageType.Freezing,
                DamageType.Burning,
                DamageType.Piercing,
                DamageType.Ballistic,
                DamageType.Shrapnel
            },
            new List<DamageType>
            {
                DamageType.BallisticArmourPiercing,
                DamageType.ArmourPiercing,
            },
            new List<DamageType>
            {
                DamageType.Arcane,
                DamageType.Necrotic,
                DamageType.Eldritch
            }
        );
        AddArmourType("Level IV Ballistic Armour", 1, 2, 4, 6.25, 8.5, 8.9,
            new List<DamageType>
            {
                DamageType.Chemical,
                DamageType.Freezing,
                DamageType.Burning,
                DamageType.Piercing,
                DamageType.Ballistic,
                DamageType.Shrapnel
            },
            new List<DamageType>
            {
                DamageType.BallisticArmourPiercing,
                DamageType.ArmourPiercing,
            },
            new List<DamageType>
            {
                DamageType.Arcane,
                DamageType.Necrotic,
                DamageType.Eldritch
            }
        );
        AddArmourType("Level 1 Stab Vest", 1, 0, 1, 4.0, 7.0, 7.5,
            new List<DamageType>
            {
                DamageType.Chemical,
                DamageType.Freezing,
                DamageType.Burning,
                DamageType.Piercing,
                DamageType.Slashing,
                DamageType.Claw,
                DamageType.Bite,
                DamageType.Shearing,
                DamageType.Shrapnel
            },
            new List<DamageType>
            {
                DamageType.Ballistic
            },
            new List<DamageType>
            {
                DamageType.Arcane,
                DamageType.Necrotic,
                DamageType.Eldritch,
                DamageType.BallisticArmourPiercing,
                DamageType.ArmourPiercing,
            }
        );
        AddArmourType("Level 2 Stab Vest", 1, 1, 1, 5.0, 7.0, 7.5,
            new List<DamageType>
            {
                DamageType.Chemical,
                DamageType.Freezing,
                DamageType.Burning,
                DamageType.Piercing,
                DamageType.Slashing,
                DamageType.Claw,
                DamageType.Bite,
                DamageType.Shearing,
                DamageType.Shrapnel
            },
            new List<DamageType>
            {
                DamageType.Ballistic
            },
            new List<DamageType>
            {
                DamageType.Arcane,
                DamageType.Necrotic,
                DamageType.Eldritch,
                DamageType.BallisticArmourPiercing,
                DamageType.ArmourPiercing,
            }
        );
        AddArmourType("Level 3 Stab Vest", 1, 1, 2, 6.0, 7.0, 7.5,
            new List<DamageType>
            {
                DamageType.Chemical,
                DamageType.Freezing,
                DamageType.Burning,
                DamageType.Piercing,
                DamageType.Slashing,
                DamageType.Claw,
                DamageType.Bite,
                DamageType.Shearing,
                DamageType.Shrapnel
            },
            new List<DamageType>
            {
                DamageType.Ballistic
            },
            new List<DamageType>
            {
                DamageType.Arcane,
                DamageType.Necrotic,
                DamageType.Eldritch,
                DamageType.BallisticArmourPiercing,
                DamageType.ArmourPiercing,
            }
        );
        AddArmourType("Boxing Gloves", 1, 1, 2, 1.2, 2.5, 1.5,
            new List<DamageType>
            {
                DamageType.Chemical,
                DamageType.Freezing,
                DamageType.Burning,
                DamageType.Electrical
            },
            new List<DamageType>
            {
                DamageType.Piercing,
                DamageType.Chopping,
                DamageType.Slashing,
                DamageType.Ballistic,
                DamageType.Bite
            },
            new List<DamageType>
            {
                DamageType.BallisticArmourPiercing,
                DamageType.ArmourPiercing,
                DamageType.Arcane,
                DamageType.Necrotic,
                DamageType.Eldritch
            },
            new List<DamageType>
            {
                DamageType.Crushing
            }
        );
        AddArmourType("Boiled Leather", 1, 1, 2, 1.4, 2.9, 1.9,
            new List<DamageType>
            {
                DamageType.Chemical,
                DamageType.Freezing,
                DamageType.Burning,
                DamageType.Electrical,
                DamageType.Slashing,
                DamageType.Chopping
            },
            new List<DamageType>
            {
                DamageType.Piercing,
                DamageType.Crushing,
                DamageType.Ballistic,
                DamageType.Bite,
                DamageType.Claw
            },
            new List<DamageType>
            {
                DamageType.BallisticArmourPiercing,
                DamageType.ArmourPiercing,
                DamageType.Arcane,
                DamageType.Necrotic,
                DamageType.Eldritch
            }
        );
        AddArmourType("Studded Leather", 1, 1, 2, 1.8, 3.5, 2.5,
            new List<DamageType>
            {
                DamageType.Chemical,
                DamageType.Freezing,
                DamageType.Burning,
                DamageType.Electrical,
                DamageType.Slashing
            },
            new List<DamageType>
            {
                DamageType.Piercing,
                DamageType.Crushing,
                DamageType.Ballistic,
                DamageType.Bite,
                DamageType.Chopping
            },
            new List<DamageType>
            {
                DamageType.BallisticArmourPiercing,
                DamageType.ArmourPiercing,
                DamageType.Arcane,
                DamageType.Necrotic,
                DamageType.Eldritch
            }
        );
        AddArmourType("Leather Scale", 1, 1, 2, 2.2, 3.9, 2.9,
            new List<DamageType>
            {
                DamageType.Chemical,
                DamageType.Freezing,
                DamageType.Burning,
                DamageType.Electrical,
                DamageType.Slashing,
                DamageType.Chopping
            },
            new List<DamageType>
            {
                DamageType.Crushing,
                DamageType.Shockwave,
                DamageType.BallisticArmourPiercing,
                DamageType.ArmourPiercing
            },
            new List<DamageType>
            {
                DamageType.Arcane,
                DamageType.Necrotic,
                DamageType.Eldritch
            }
        );
        AddArmourType("Metal Scale", 1, 1, 2, 3.6, 5.0, 4.5,
            new List<DamageType>
            {
                DamageType.Chemical,
                DamageType.Freezing,
                DamageType.Burning,
                DamageType.Electrical,
                DamageType.Slashing,
                DamageType.Chopping
            },
            new List<DamageType>
            {
                DamageType.Crushing,
                DamageType.Shockwave,
                DamageType.BallisticArmourPiercing,
                DamageType.ArmourPiercing,
                DamageType.Ballistic
            },
            new List<DamageType>
            {
                DamageType.BallisticArmourPiercing,
                DamageType.ArmourPiercing,
                DamageType.Arcane,
                DamageType.Necrotic,
                DamageType.Eldritch
            }
        );
        AddArmourType("Laminar", 1, 1, 2, 3.7, 5.0, 4.5,
            new List<DamageType>
            {
                DamageType.Chemical,
                DamageType.Freezing,
                DamageType.Burning,
                DamageType.Electrical,
                DamageType.Slashing,
                DamageType.Chopping
            },
            new List<DamageType>
            {
                DamageType.Crushing,
                DamageType.Shockwave,
                DamageType.BallisticArmourPiercing,
                DamageType.ArmourPiercing,
                DamageType.Ballistic
            },
            new List<DamageType>
            {
                DamageType.BallisticArmourPiercing,
                DamageType.ArmourPiercing,
                DamageType.Arcane,
                DamageType.Necrotic,
                DamageType.Eldritch
            }
        );
        AddArmourType("Lamellar", 1, 1, 2, 3.5, 5.0, 4.5,
            new List<DamageType>
            {
                DamageType.Chemical,
                DamageType.Freezing,
                DamageType.Burning,
                DamageType.Electrical,
                DamageType.Slashing,
                DamageType.Chopping
            },
            new List<DamageType>
            {
                DamageType.Crushing,
                DamageType.Shockwave,
                DamageType.BallisticArmourPiercing,
                DamageType.ArmourPiercing,
                DamageType.Ballistic
            },
            new List<DamageType>
            {
                DamageType.BallisticArmourPiercing,
                DamageType.ArmourPiercing,
                DamageType.Arcane,
                DamageType.Necrotic,
                DamageType.Eldritch
            }
        );
        AddArmourType("Chainmail", 1, 1, 2, 3.5, 5.0, 4.5,
            new List<DamageType>
            {
                DamageType.Chemical,
                DamageType.Freezing,
                DamageType.Burning,
                DamageType.Electrical,
                DamageType.Slashing,
                DamageType.Claw,
                DamageType.Chopping,
                DamageType.Shearing
            },
            new List<DamageType>
            {
                DamageType.Crushing,
                DamageType.Shockwave,
                DamageType.Falling,
                DamageType.Ballistic,
                DamageType.BallisticArmourPiercing,
                DamageType.ArmourPiercing
            },
            new List<DamageType>
            {
                DamageType.Arcane,
                DamageType.Necrotic,
                DamageType.Eldritch
            }
        );
        AddArmourType("Platemail", 1, 2, 3, 4.5, 6.0, 8.0,
            new List<DamageType>
            {
                DamageType.Chemical,
                DamageType.Freezing,
                DamageType.Burning,
                DamageType.Electrical,
                DamageType.Slashing,
                DamageType.Claw,
                DamageType.Chopping,
                DamageType.Shearing
            },
            new List<DamageType>
            {
                DamageType.Crushing,
                DamageType.Shockwave,
                DamageType.Falling,
                DamageType.Ballistic
            },
            new List<DamageType>
            {
                DamageType.BallisticArmourPiercing,
                DamageType.ArmourPiercing,
                DamageType.Arcane,
                DamageType.Necrotic,
                DamageType.Eldritch
            }
        );

        AddSpecialArmourType("Hearing Protection", 0, 0, 0, @"<ArmourType>
  <DissipateExpressions>
	<Expression damagetype=""14"">damage-quality</Expression>
  </DissipateExpressions>
  <DissipateExpressionsPain>
	<Expression damagetype=""14"">pain-quality</Expression>
  </DissipateExpressionsPain>
  <DissipateExpressionsStun>
	<Expression damagetype=""14"">stun-quality</Expression>
  </DissipateExpressionsStun>
  <AbsorbExpressions>
	<Expression damagetype=""14"">damage*(1 - quality*0.1)</Expression>
  </AbsorbExpressions>
  <AbsorbExpressionsPain>
	<Expression damagetype=""14"">pain*(1 - quality*0.1)</Expression>
  </AbsorbExpressionsPain>
  <AbsorbExpressionsStun>
	<Expression damagetype=""14"">stun*(1 - quality*0.1)</Expression>
  </AbsorbExpressionsStun>
 </ArmourType>");
    }
}

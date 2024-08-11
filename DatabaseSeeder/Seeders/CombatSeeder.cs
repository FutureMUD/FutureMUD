using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Combat;
using MudSharp.Database;
using MudSharp.Form.Audio;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public class CombatSeeder : IDatabaseSeeder
{
	#region Implementation of IDatabaseSeeder

	public IEnumerable<(string Id, string Question,
		Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
		Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions
		=> new List<(string Id, string Question,
			Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
			Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)>
		{
			("installunarmed",
				"Do you want to install unarmed melee attacks for humans?\n\nPlease answer #3yes#f or #3no#f: ",
				(context, answers) => true,
				(answer, context) =>
				{
					return (answer.EqualToAny("yes", "y", "no", "n"), "You must answer yes or no.");
				}),
			("installweapons",
				"Do you want to install armed melee attacks, including by necessity the weapon types behind them?\n\nPlease answer #3yes#f or #3no#f: ",
				(context, answers) => true,
				(answer, context) =>
				{
					return (answer.EqualToAny("yes", "y", "no", "n"), "You must answer yes or no.");
				}),
			("installarmour", @"Do you want to install armour types? 

Please answer #3yes#f or #3no#f: ", (context, answers) => true,
				(answer, context) =>
				{
					return (answer.EqualToAny("yes", "y", "no", "n"), "You must answer yes or no.");
				}),
			("installranged",
				"Do you want to install pre-modern ranged weapon types, such as bows, crossbows and throwing weapons?\n\nPlease answer #3yes#f or #3no#f: ",
				(context, answers) => true,
				(answer, context) =>
				{
					return (answer.EqualToAny("yes", "y", "no", "n"), "You must answer yes or no.");
				}),
			("installmuskets",
				"[Not Yet Implemented] Do you want to install some early gunpowder weapon types?\n\nPlease answer #3yes#f or #3no#f: ",
				(context, answers) => true,
				(answer, context) =>
				{
					return (answer.EqualToAny("yes", "y", "no", "n"), "You must answer yes or no.");
				}),
			("installguns",
				"Do you want to install some example modern firearm types?\n\nPlease answer #3yes#f or #3no#f: ",
				(context, answers) => true,
				(answer, context) =>
				{
					return (answer.EqualToAny("yes", "y", "no", "n"), "You must answer yes or no.");
				}),
			("random", @"#DDamage Formulas#F

You can configure your damage formulas to be consistent or random. The engine already takes into account a number of variables such as relative success of attacker and defender, type of defense used, all of which ensure that the damage is mitigated differently each attack. However, a good hit is usually pretty impactful in that kind of setup.

Randomness in damage is sometimes used to add complexity or choice to weapon types when the outcome of the attack is fairly likely (see D20 systems where before long hitting is almost guaranteed). This can work too but it can be disappointing for someone to land a good blow with all the factors right and then simply do little damage because of RNG, whereas another just-barely hit might do full damage.

There are three options that you can choose for randomness:

#BStatic#F: In this option (which was used in LabMUD) base damage is static. A hit with the same quality weapon, the same strength and the same attack/defense result will lead to the same damage
#BPartial#F: In this option 30% of the damage will be random - this adds a little bit of uncertainty and variety but still makes hits /largely/ a function of relative success
#BRandom#F: In this option damage can be 20-100% of the maximum. This means outcomes will vary wildly.

Which option do you want to use for random results in your weapon damage formulas?", (context, answers) => true,
				(answer, context) =>
				{
					return (answer.EqualToAny("static", "partial", "random"),
						"You must answer static, partial or random.");
				}),
			("parryoption",
				@"Do you want to use a separate skill called 'parry' as the skill for parrying with weapons? If you answer no, the weapon will use its attacking trait for parrying instead.

Please answer #3yes#f or #3no#f: ", (context, answers) => answers["installweapons"].EqualToAny("y", "yes"),
				(answer, context) =>
				{
					return (answer.EqualToAny("yes", "y", "no", "n"), "You must answer yes or no.");
				}),
			("skilloption", @"#DWeapon Skills#F

There are many different ways that you could set up combat skills. A few options are presented here. Keep in mind that you can always change the names of these options later, better that you pick something that lines up with the overall shape that you want than fixing on the names or such.

#BWeapons#f: In this option each weapon has its own skill, e.g. Swords, Axes, Spears, Daggers
#BBroad#f: In this option you have broad categories across multiple weapons such as Edged, Bludgeoning, Piercing, Two-Handed
#BSOI#f: This option sets up skills like old SOI had, with 9 combinations of light, medium, heavy and edged, bludgeon and piercing (e.g. medium-edge etc)

Which option do you want to choose for weapon skills? ",
				(context, answers) => answers["installweapons"].EqualToAny("y", "yes"),
				(answer, context) =>
				{
					return (answer.EqualToAny("weapons", "broad", "soi"),
						"You must answer 'weapons', 'broad' or 'soi'.");
				}),
			("messagestyle", @"#DCombat Messages#F

Combat messages can be presented in a number of different styles. Fundamentally, the attack and the defense against the attack are different messages. You can either have them come together to form a single sentence, or you can keep them separate sentences, or you can put them on entirely different lines. For example, here are the three options you could consider:

#BCompact#F

    A tall, bearded man swings a steel longsword at a pudgy, brown-haired codger, who tries to dodge but gets hit on the head!

#BSentences#F

    A tall, bearded man swings a steel longsword at a pudgy, brown-haired codger. He tries to dodge out of the way but is unsuccessful. He is hit on the head.

#BSparse#F

    A tall, bearded man swings a steel longsword at a pudgy, brown-haired codger.
    He tries to dodge out of the way but is unsuccessful.
    He is hit on the head!

You can change your decision later, you're just going to have to go and edit your combat messages (mostly the defenses) to match the style you want. One advantage to doing Sentences or Sparse is that you can easily colour whole elements if you prefer (some people prefer not to of course).

You can choose #3Compact#f, #3Sentences#f or #3Sparse#f",
				(context, answers) => answers["installunarmed"].EqualToAny("y", "yes") ||
				                      answers["installweapons"].EqualToAny("y", "yes"),
				(answer, context) =>
				{
					return (answer.EqualToAny("compact", "sentences", "sparse"),
						"You must answer Compact, Sentences or Sparse.");
				})
		};

	public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		context.Database.BeginTransaction();
		var skills = SeedCoreData(context, questionAnswers);

		if (questionAnswers["installunarmed"].EqualToAny("yes", "y")) SeedDataUnarmed(context, questionAnswers, skills);
		if (questionAnswers["installweapons"].EqualToAny("yes", "y")) SeedDataWeapons(context, questionAnswers, skills);
		if (questionAnswers["installranged"].EqualToAny("yes", "y")) SeedDataRanged(context, questionAnswers, skills);
		if (questionAnswers["installmuskets"].EqualToAny("yes", "y")) SeedDataMuskets(context, questionAnswers);
		if (questionAnswers["installguns"].EqualToAny("yes", "y")) SeedDataGuns(context, questionAnswers);
		if (questionAnswers["installarmour"].EqualToAny("yes", "y")) SeedArmourTypes(context, questionAnswers);

		context.Database.CommitTransaction();

		return "The operation completed successfully.";
	}

	private string ArmourDissipateModifier(DamageType type, double power, IEnumerable<DamageType> strongTypes,
		IEnumerable<DamageType> weakTypes, IEnumerable<DamageType> zeroTypes, IEnumerable<DamageType> superTypes)
	{
		if (zeroTypes.Contains(type)) return "";

		switch (type)
		{
			case DamageType.Slashing:
			case DamageType.Chopping:
			case DamageType.Piercing:
			case DamageType.Ballistic:
			case DamageType.Bite:
			case DamageType.Claw:
			case DamageType.Shearing:
			case DamageType.Shrapnel:
			case DamageType.Crushing:
			case DamageType.Shockwave:
			case DamageType.Wrenching:
			case DamageType.Burning:
			case DamageType.Freezing:
			case DamageType.Chemical:
			case DamageType.Electrical:
			case DamageType.Sonic:
			case DamageType.Necrotic:
			case DamageType.Eldritch:
			case DamageType.Falling:
			case DamageType.Arcane:
			case DamageType.ArmourPiercing:
				if (superTypes.Contains(type)) return $"-(quality*{power * 2.0})";

				if (strongTypes.Contains(type)) return $"-(quality*{power * 0.65})";

				if (weakTypes.Contains(type)) return $"-(quality*{power * 0.1})";
				return $"-(quality*{power * 0.35})";
			case DamageType.Hypoxia:
			case DamageType.Cellular:
				return "";
			default:
				throw new ArgumentOutOfRangeException(nameof(type), type, null);
		}
	}

	private string ArmourAbsorbModifier(DamageType type, double power, IEnumerable<DamageType> strongTypes,
		IEnumerable<DamageType> weakTypes, IEnumerable<DamageType> zeroTypes, IEnumerable<DamageType> superTypes)
	{
		if (zeroTypes.Contains(type)) return "";

		switch (type)
		{
			case DamageType.Slashing:
			case DamageType.Chopping:
			case DamageType.Piercing:
			case DamageType.Ballistic:
			case DamageType.Bite:
			case DamageType.Claw:
			case DamageType.Shearing:
			case DamageType.Shrapnel:
			case DamageType.Crushing:
			case DamageType.Shockwave:
			case DamageType.Wrenching:
			case DamageType.Burning:
			case DamageType.Freezing:
			case DamageType.Chemical:
			case DamageType.Electrical:
			case DamageType.Sonic:
			case DamageType.Necrotic:
			case DamageType.Eldritch:
			case DamageType.Falling:
			case DamageType.Arcane:
			case DamageType.ArmourPiercing:
				if (superTypes.Contains(type)) return $"*({1.0 - power * 0.05}-(quality*{power * 0.05}))";

				if (strongTypes.Contains(type)) return $"*({1.0 - power * 0.03}-(quality*{power * 0.03}))";

				if (weakTypes.Contains(type)) return $"*({1.0 - power * 0.01}-(quality*{power * 0.01}))";
				return $"*({1.0 - power * 0.02}-(quality*{power * 0.02}))";
			case DamageType.Hypoxia:
			case DamageType.Cellular:
				return "";

			default:
				throw new ArgumentOutOfRangeException(nameof(type), type, null);
		}
	}

	private void SeedArmourTypes(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> questionAnswers)
	{
		var dbaccount = context.Accounts.First();
		var now = DateTime.UtcNow;

		void AddArmourType(string name, int penetration, int baseDifficulty, int stackedDifficulty, double powerDamage,
			double powerPain, double powerStun, IEnumerable<DamageType> strongTypes, IEnumerable<DamageType> weakTypes,
			IEnumerable<DamageType> zeroTypes, IEnumerable<DamageType> superTypes = null)
		{
			if (superTypes == null) superTypes = Enumerable.Empty<DamageType>();

			var armour = new ArmourType
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
		<Expression damagetype=""16"">damage{ArmourDissipateModifier(DamageType.ArmourPiercing, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">damage{ArmourDissipateModifier(DamageType.Wrenching, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Wrenching -->
 		<Expression damagetype=""18"">damage{ArmourDissipateModifier(DamageType.Shrapnel, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">damage{ArmourDissipateModifier(DamageType.Necrotic, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>                    <!-- Necrotic -->   
 		<Expression damagetype=""20"">damage{ArmourDissipateModifier(DamageType.Falling, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">damage{ArmourDissipateModifier(DamageType.Eldritch, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>                    <!-- Eldritch -->   
		<Expression damagetype=""22"">damage{ArmourDissipateModifier(DamageType.Arcane, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>                    <!-- Arcane -->   
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
		<Expression damagetype=""16"">pain{ArmourDissipateModifier(DamageType.ArmourPiercing, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">pain{ArmourDissipateModifier(DamageType.Wrenching, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Wrenching -->
 		<Expression damagetype=""18"">pain{ArmourDissipateModifier(DamageType.Shrapnel, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">pain{ArmourDissipateModifier(DamageType.Necrotic, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>                    <!-- Necrotic -->   
 		<Expression damagetype=""20"">pain{ArmourDissipateModifier(DamageType.Falling, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">pain{ArmourDissipateModifier(DamageType.Eldritch, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>                    <!-- Eldritch -->   
		<Expression damagetype=""22"">pain{ArmourDissipateModifier(DamageType.Arcane, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>                    <!-- Arcane -->   
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
		<Expression damagetype=""16"">stun{ArmourDissipateModifier(DamageType.ArmourPiercing, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">stun{ArmourDissipateModifier(DamageType.Wrenching, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Wrenching -->
 		<Expression damagetype=""18"">stun{ArmourDissipateModifier(DamageType.Shrapnel, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">stun{ArmourDissipateModifier(DamageType.Necrotic, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>                    <!-- Necrotic -->   
 		<Expression damagetype=""20"">stun{ArmourDissipateModifier(DamageType.Falling, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">stun{ArmourDissipateModifier(DamageType.Eldritch, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>                    <!-- Eldritch -->   
		<Expression damagetype=""22"">stun{ArmourDissipateModifier(DamageType.Arcane, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>                    <!-- Arcane -->   
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
		<Expression damagetype=""16"">damage{ArmourAbsorbModifier(DamageType.ArmourPiercing, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">damage{ArmourAbsorbModifier(DamageType.Wrenching, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Wrenching -->
 		<Expression damagetype=""18"">damage{ArmourAbsorbModifier(DamageType.Shrapnel, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">damage{ArmourAbsorbModifier(DamageType.Necrotic, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Necrotic -->   
 		<Expression damagetype=""20"">damage{ArmourAbsorbModifier(DamageType.Falling, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">damage{ArmourAbsorbModifier(DamageType.Eldritch, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Eldritch -->   
		<Expression damagetype=""22"">damage{ArmourAbsorbModifier(DamageType.Arcane, powerDamage, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Arcane -->   
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
		<Expression damagetype=""16"">pain{ArmourAbsorbModifier(DamageType.ArmourPiercing, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">pain{ArmourAbsorbModifier(DamageType.Wrenching, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Wrenching -->
 		<Expression damagetype=""18"">pain{ArmourAbsorbModifier(DamageType.Shrapnel, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">pain{ArmourAbsorbModifier(DamageType.Necrotic, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Necrotic -->   
 		<Expression damagetype=""20"">pain{ArmourAbsorbModifier(DamageType.Falling, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">pain{ArmourAbsorbModifier(DamageType.Eldritch, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Eldritch -->   
		<Expression damagetype=""22"">pain{ArmourAbsorbModifier(DamageType.Arcane, powerPain, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Arcane -->   
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
		<Expression damagetype=""16"">stun{ArmourAbsorbModifier(DamageType.ArmourPiercing, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">stun{ArmourAbsorbModifier(DamageType.Wrenching, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Wrenching -->
 		<Expression damagetype=""18"">stun{ArmourAbsorbModifier(DamageType.Shrapnel, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">stun{ArmourAbsorbModifier(DamageType.Necrotic, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Necrotic -->   
 		<Expression damagetype=""20"">stun{ArmourAbsorbModifier(DamageType.Falling, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">stun{ArmourAbsorbModifier(DamageType.Eldritch, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Eldritch -->   
		<Expression damagetype=""22"">stun{ArmourAbsorbModifier(DamageType.Arcane, powerStun, strongTypes, weakTypes, zeroTypes, superTypes)}</Expression>   <!-- Arcane -->   
 	</AbsorbExpressionsStun>
 </ArmourType>"
			};
			context.ArmourTypes.Add(armour);
			context.SaveChanges();
			var component = new GameItemComponentProto
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
			var armour = new ArmourType
			{
				Name = name,
				MinimumPenetrationDegree = penetration,
				BaseDifficultyDegrees = baseDifficulty,
				StackedDifficultyDegrees = stackedDifficulty,
				Definition = definition
			};
			context.ArmourTypes.Add(armour);
			context.SaveChanges();
			var component = new GameItemComponentProto
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

		AddArmourType("Light Clothing", 1, 0, 0, 0.5, 0.75, 0.6,
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
				DamageType.ArmourPiercing,
				DamageType.Arcane,
				DamageType.Necrotic,
				DamageType.Eldritch
			}
		);
		AddArmourType("Heavy Clothing", 1, 0, 1, 0.8, 1.2, 0.9,
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
				DamageType.ArmourPiercing,
				DamageType.Arcane,
				DamageType.Necrotic,
				DamageType.Eldritch
			}
		);
		AddArmourType("Ultra Heavy Clothing", 1, 1, 2, 1.2, 2.5, 1.5,
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
				DamageType.ArmourPiercing
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
				DamageType.ArmourPiercing
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
				DamageType.ArmourPiercing
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
				DamageType.ArmourPiercing
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
				DamageType.ArmourPiercing
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
				DamageType.ArmourPiercing
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
				DamageType.ArmourPiercing
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
				DamageType.ArmourPiercing
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
				DamageType.ArmourPiercing
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
		AddArmourType("Boiled Leather", 1, 1, 2, 1.6, 2.9, 1.9,
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
				DamageType.Bite,
				DamageType.Claw
			},
			new List<DamageType>
			{
				DamageType.ArmourPiercing,
				DamageType.Arcane,
				DamageType.Necrotic,
				DamageType.Eldritch
			}
		);
		AddArmourType("Studded Leather", 1, 1, 2, 2.1, 3.5, 2.5,
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
				DamageType.ArmourPiercing,
				DamageType.Arcane,
				DamageType.Necrotic,
				DamageType.Eldritch
			}
		);
		AddArmourType("Leather Scale", 1, 1, 2, 2.5, 3.9, 2.9,
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
				DamageType.Bite,
				DamageType.Ballistic
			},
			new List<DamageType>
			{
				DamageType.ArmourPiercing,
				DamageType.Arcane,
				DamageType.Necrotic,
				DamageType.Eldritch
			}
		);
		AddArmourType("Metal Scale", 1, 1, 2, 4.0, 5.0, 4.5,
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
				DamageType.Bite,
				DamageType.Ballistic
			},
			new List<DamageType>
			{
				DamageType.ArmourPiercing,
				DamageType.Arcane,
				DamageType.Necrotic,
				DamageType.Eldritch
			}
		);
		AddArmourType("Laminar", 1, 1, 2, 4.0, 5.0, 4.5,
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
				DamageType.Bite,
				DamageType.Ballistic
			},
			new List<DamageType>
			{
				DamageType.ArmourPiercing,
				DamageType.Arcane,
				DamageType.Necrotic,
				DamageType.Eldritch
			}
		);
		AddArmourType("Lamellar", 1, 1, 2, 4.0, 5.0, 4.5,
			new List<DamageType>
			{
				DamageType.Chemical,
				DamageType.Freezing,
				DamageType.Burning,
				DamageType.Electrical,
				DamageType.Claw
			},
			new List<DamageType>
			{
				DamageType.Piercing,
				DamageType.Chopping,
				DamageType.Bite,
				DamageType.Ballistic
			},
			new List<DamageType>
			{
				DamageType.ArmourPiercing,
				DamageType.Arcane,
				DamageType.Necrotic,
				DamageType.Eldritch
			}
		);
		AddArmourType("Chainmail", 1, 1, 2, 4.0, 5.0, 4.5,
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
				DamageType.Piercing,
				DamageType.Bite,
				DamageType.Ballistic
			},
			new List<DamageType>
			{
				DamageType.ArmourPiercing,
				DamageType.Arcane,
				DamageType.Necrotic,
				DamageType.Eldritch
			}
		);
		AddArmourType("Platemail", 1, 2, 3, 6.0, 6.0, 8.0,
			new List<DamageType>
			{
				DamageType.Chemical,
				DamageType.Freezing,
				DamageType.Burning,
				DamageType.Electrical,
				DamageType.Slashing,
				DamageType.Claw,
				DamageType.Shearing
			},
			new List<DamageType>
			{
				DamageType.Ballistic
			},
			new List<DamageType>
			{
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

	private void SeedDataGuns(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		var dbaccount = context.Accounts.First();
		var now = DateTime.UtcNow;
		var attributes = context.TraitDefinitions.Where(x => x.Type == 1 || x.Type == 3)
			.ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
		var strength =
			attributes.GetValueOrDefault("Strength") ??
			attributes.GetValueOrDefault("Physique") ??
			attributes["Body"];

		var expression = new TraitExpression
		{
			Name = "Pistol Skill Cap",
			Expression = $"min(99,5.5*{strength.Alias}:{strength.Id})"
		};
		var pistols = new TraitDefinition
		{
			Name = "Pistols",
			Type = (int)TraitType.Skill,
			Expression = expression,
			TraitGroup = "Combat",
			AvailabilityProg = context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue"),
			TeachableProg = context.FutureProgs.First(x => x.FunctionName == "AlwaysFalse"),
			LearnableProg = context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue"),
			TeachDifficulty = 7,
			LearnDifficulty = 7,
			Hidden = false,
			ImproverId = context.Improvers.First(x => x.Name == "Skill Improver").Id,
			DecoratorId = context.TraitDecorators.First(x => x.Name == "General Skill").Id,
			DerivedType = 0,
			ChargenBlurb = string.Empty,
			BranchMultiplier = 1.0
		};
		context.TraitDefinitions.Add(pistols);
		context.SaveChanges();

		var ranged = new RangedWeaponTypes
		{
			Name = "9mm Pistol",
			Classification = (int)WeaponClassification.Lethal,
			FireTrait = pistols,
			OperateTrait = pistols,
			FireableInMelee = true,
			DefaultRangeInRooms = 2,
			AccuracyBonusExpression = "(-2.0*range)+(pow(1-aim,2)*4.0)",
			DamageBonusExpression = "-10*range",
			AmmunitionLoadType = (int)AmmunitionLoadType.Magazine,
			SpecificAmmunitionGrade = "9x19mm Parabellum",
			AmmunitionCapacity = 1,
			RangedWeaponType = (int)RangedWeaponType.ModernFirearm,
			StaminaToFire = 5.0,
			StaminaPerLoadStage = 7.0,
			CoverBonus = -3.0,
			BaseAimDifficulty = (int)Difficulty.Easy,
			LoadDelay = 1.2,
			ReadyDelay = 0.3,
			FireDelay = 0.1,
			AimBonusLostPerShot = 0.4,
			RequiresFreeHandToReady = true,
			AlwaysRequiresTwoHandsToWield = false
		};
		context.RangedWeaponTypes.Add(ranged);
		context.SaveChanges();

		var component = new GameItemComponentProto
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
			Type = "Gun",
			Name = "Pistol_9mm",
			Description = "Turns an item into a 9mm Pistol",
			Definition =
				@$"<Definition>
   <LoadEmote><![CDATA[@ insert|inserts $2 into $1 and it clicks into place.]]></LoadEmote>
   <ReadyEmote><![CDATA[@ rack|racks the slide on $1, and it clicks back into place.]]></ReadyEmote>
   <UnloadEmote><![CDATA[@ hit|hits the eject button on $1 and $2 is ejected.]]></UnloadEmote>
   <UnreadyEmote><![CDATA[@ open|opens the slide on $1 and work|works out $2 from the chamber.]]></UnreadyEmote>
   <UnreadyEmoteNoChamberedRound><![CDATA[@ open|opens the slide on $1, but there is no round in the chamber.]]></UnreadyEmoteNoChamberedRound>
   <FireEmote><![CDATA[@ squeeze|squeezes the trigger on $2 and it fires a round with an extremely loud bang.]]></FireEmote>
   <FireEmoteNoChamberedRound><![CDATA[@ squeeze|squeezes the trigger on $2, and nothing happens except a quiet click.]]></FireEmoteNoChamberedRound>
   <RangedWeaponType>{ranged.Id}</RangedWeaponType>
 </Definition>"
		};
		context.GameItemComponentProtos.Add(component);
		context.SaveChanges();

		ranged = new RangedWeaponTypes
		{
			Name = ".25 ACP Pistol",
			Classification = (int)WeaponClassification.Lethal,
			FireTrait = pistols,
			OperateTrait = pistols,
			FireableInMelee = true,
			DefaultRangeInRooms = 2,
			AccuracyBonusExpression = "(-2.3*range)+(pow(1-aim,2)*4.0)",
			DamageBonusExpression = "-10*range",
			AmmunitionLoadType = (int)AmmunitionLoadType.Magazine,
			SpecificAmmunitionGrade = ".25 ACP",
			AmmunitionCapacity = 1,
			RangedWeaponType = (int)RangedWeaponType.ModernFirearm,
			StaminaToFire = 3.0,
			StaminaPerLoadStage = 7.0,
			CoverBonus = -3.0,
			BaseAimDifficulty = (int)Difficulty.Easy,
			LoadDelay = 1.2,
			ReadyDelay = 0.3,
			FireDelay = 0.1,
			AimBonusLostPerShot = 0.25,
			RequiresFreeHandToReady = true,
			AlwaysRequiresTwoHandsToWield = false
		};
		context.RangedWeaponTypes.Add(ranged);
		context.SaveChanges();

		component = new GameItemComponentProto
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
			Type = "Gun",
			Name = "Pistol_25ACP",
			Description = "Turns an item into a .25 ACP Pistol",
			Definition =
				@$"<Definition>
   <LoadEmote><![CDATA[@ insert|inserts $2 into $1 and it clicks into place.]]></LoadEmote>
   <ReadyEmote><![CDATA[@ rack|racks the slide on $1, and it clicks back into place.]]></ReadyEmote>
   <UnloadEmote><![CDATA[@ hit|hits the eject button on $1 and $2 is ejected.]]></UnloadEmote>
   <UnreadyEmote><![CDATA[@ open|opens the slide on $1 and work|works out $2 from the chamber.]]></UnreadyEmote>
   <UnreadyEmoteNoChamberedRound><![CDATA[@ open|opens the slide on $1, but there is no round in the chamber.]]></UnreadyEmoteNoChamberedRound>
   <FireEmote><![CDATA[@ squeeze|squeezes the trigger on $2 and it fires a round with an extremely loud bang.]]></FireEmote>
   <FireEmoteNoChamberedRound><![CDATA[@ squeeze|squeezes the trigger on $2, and nothing happens except a quiet click.]]></FireEmoteNoChamberedRound>
   <RangedWeaponType>{ranged.Id}</RangedWeaponType>
 </Definition>"
		};
		context.GameItemComponentProtos.Add(component);
		context.SaveChanges();

		ranged = new RangedWeaponTypes
		{
			Name = ".32 ACP Pistol",
			Classification = (int)WeaponClassification.Lethal,
			FireTrait = pistols,
			OperateTrait = pistols,
			FireableInMelee = true,
			DefaultRangeInRooms = 2,
			AccuracyBonusExpression = "(-2.0*range)+(pow(1-aim,2)*4.0)",
			DamageBonusExpression = "-10*range",
			AmmunitionLoadType = (int)AmmunitionLoadType.Magazine,
			SpecificAmmunitionGrade = ".32 ACP",
			AmmunitionCapacity = 1,
			RangedWeaponType = (int)RangedWeaponType.ModernFirearm,
			StaminaToFire = 5.0,
			StaminaPerLoadStage = 7.0,
			CoverBonus = -3.0,
			BaseAimDifficulty = (int)Difficulty.Easy,
			LoadDelay = 1.2,
			ReadyDelay = 0.3,
			FireDelay = 0.1,
			AimBonusLostPerShot = 0.4,
			RequiresFreeHandToReady = true,
			AlwaysRequiresTwoHandsToWield = false
		};
		context.RangedWeaponTypes.Add(ranged);
		context.SaveChanges();

		component = new GameItemComponentProto
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
			Type = "Gun",
			Name = "Pistol_32ACP",
			Description = "Turns an item into a .32 ACP Pistol",
			Definition =
				@$"<Definition>
   <LoadEmote><![CDATA[@ insert|inserts $2 into $1 and it clicks into place.]]></LoadEmote>
   <ReadyEmote><![CDATA[@ rack|racks the slide on $1, and it clicks back into place.]]></ReadyEmote>
   <UnloadEmote><![CDATA[@ hit|hits the eject button on $1 and $2 is ejected.]]></UnloadEmote>
   <UnreadyEmote><![CDATA[@ open|opens the slide on $1 and work|works out $2 from the chamber.]]></UnreadyEmote>
   <UnreadyEmoteNoChamberedRound><![CDATA[@ open|opens the slide on $1, but there is no round in the chamber.]]></UnreadyEmoteNoChamberedRound>
   <FireEmote><![CDATA[@ squeeze|squeezes the trigger on $2 and it fires a round with an extremely loud bang.]]></FireEmote>
   <FireEmoteNoChamberedRound><![CDATA[@ squeeze|squeezes the trigger on $2, and nothing happens except a quiet click.]]></FireEmoteNoChamberedRound>
   <RangedWeaponType>{ranged.Id}</RangedWeaponType>
 </Definition>"
		};
		context.GameItemComponentProtos.Add(component);
		context.SaveChanges();

		ranged = new RangedWeaponTypes
		{
			Name = ".38 ACP Pistol",
			Classification = (int)WeaponClassification.Lethal,
			FireTrait = pistols,
			OperateTrait = pistols,
			FireableInMelee = true,
			DefaultRangeInRooms = 2,
			AccuracyBonusExpression = "(-2.0*range)+(pow(1-aim,2)*4.0)",
			DamageBonusExpression = "-8*range",
			AmmunitionLoadType = (int)AmmunitionLoadType.Magazine,
			SpecificAmmunitionGrade = ".38 ACP",
			AmmunitionCapacity = 1,
			RangedWeaponType = (int)RangedWeaponType.ModernFirearm,
			StaminaToFire = 5.5,
			StaminaPerLoadStage = 7.0,
			CoverBonus = -3.0,
			BaseAimDifficulty = (int)Difficulty.Easy,
			LoadDelay = 1.2,
			ReadyDelay = 0.3,
			FireDelay = 0.1,
			AimBonusLostPerShot = 0.45,
			RequiresFreeHandToReady = true,
			AlwaysRequiresTwoHandsToWield = false
		};
		context.RangedWeaponTypes.Add(ranged);
		context.SaveChanges();

		component = new GameItemComponentProto
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
			Type = "Gun",
			Name = "Pistol_38ACP",
			Description = "Turns an item into a .38 ACP Pistol",
			Definition =
				@$"<Definition>
   <LoadEmote><![CDATA[@ insert|inserts $2 into $1 and it clicks into place.]]></LoadEmote>
   <ReadyEmote><![CDATA[@ rack|racks the slide on $1, and it clicks back into place.]]></ReadyEmote>
   <UnloadEmote><![CDATA[@ hit|hits the eject button on $1 and $2 is ejected.]]></UnloadEmote>
   <UnreadyEmote><![CDATA[@ open|opens the slide on $1 and work|works out $2 from the chamber.]]></UnreadyEmote>
   <UnreadyEmoteNoChamberedRound><![CDATA[@ open|opens the slide on $1, but there is no round in the chamber.]]></UnreadyEmoteNoChamberedRound>
   <FireEmote><![CDATA[@ squeeze|squeezes the trigger on $2 and it fires a round with an extremely loud bang.]]></FireEmote>
   <FireEmoteNoChamberedRound><![CDATA[@ squeeze|squeezes the trigger on $2, and nothing happens except a quiet click.]]></FireEmoteNoChamberedRound>
   <RangedWeaponType>{ranged.Id}</RangedWeaponType>
 </Definition>"
		};
		context.GameItemComponentProtos.Add(component);
		context.SaveChanges();

		ranged = new RangedWeaponTypes
		{
			Name = ".38 Super ACP Pistol",
			Classification = (int)WeaponClassification.Lethal,
			FireTrait = pistols,
			OperateTrait = pistols,
			FireableInMelee = true,
			DefaultRangeInRooms = 2,
			AccuracyBonusExpression = "(-2.0*range)+(pow(1-aim,2)*4.0)",
			DamageBonusExpression = "-8*range",
			AmmunitionLoadType = (int)AmmunitionLoadType.Magazine,
			SpecificAmmunitionGrade = ".38 Super ACP",
			AmmunitionCapacity = 1,
			RangedWeaponType = (int)RangedWeaponType.ModernFirearm,
			StaminaToFire = 6.0,
			StaminaPerLoadStage = 7.0,
			CoverBonus = -3.0,
			BaseAimDifficulty = (int)Difficulty.Easy,
			LoadDelay = 1.2,
			ReadyDelay = 0.3,
			FireDelay = 0.1,
			AimBonusLostPerShot = 0.45,
			RequiresFreeHandToReady = true,
			AlwaysRequiresTwoHandsToWield = false
		};
		context.RangedWeaponTypes.Add(ranged);
		context.SaveChanges();

		component = new GameItemComponentProto
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
			Type = "Gun",
			Name = "Pistol_38SuperACP",
			Description = "Turns an item into a .38 Super ACP Pistol",
			Definition =
				@$"<Definition>
   <LoadEmote><![CDATA[@ insert|inserts $2 into $1 and it clicks into place.]]></LoadEmote>
   <ReadyEmote><![CDATA[@ rack|racks the slide on $1, and it clicks back into place.]]></ReadyEmote>
   <UnloadEmote><![CDATA[@ hit|hits the eject button on $1 and $2 is ejected.]]></UnloadEmote>
   <UnreadyEmote><![CDATA[@ open|opens the slide on $1 and work|works out $2 from the chamber.]]></UnreadyEmote>
   <UnreadyEmoteNoChamberedRound><![CDATA[@ open|opens the slide on $1, but there is no round in the chamber.]]></UnreadyEmoteNoChamberedRound>
   <FireEmote><![CDATA[@ squeeze|squeezes the trigger on $2 and it fires a round with an extremely loud bang.]]></FireEmote>
   <FireEmoteNoChamberedRound><![CDATA[@ squeeze|squeezes the trigger on $2, and nothing happens except a quiet click.]]></FireEmoteNoChamberedRound>
   <RangedWeaponType>{ranged.Id}</RangedWeaponType>
 </Definition>"
		};
		context.GameItemComponentProtos.Add(component);
		context.SaveChanges();

		ranged = new RangedWeaponTypes
		{
			Name = ".45 ACP Pistol",
			Classification = (int)WeaponClassification.Lethal,
			FireTrait = pistols,
			OperateTrait = pistols,
			FireableInMelee = true,
			DefaultRangeInRooms = 2,
			AccuracyBonusExpression = "(-2.0*range)+(pow(1-aim,2)*4.0)",
			DamageBonusExpression = "-7*range",
			AmmunitionLoadType = (int)AmmunitionLoadType.Magazine,
			SpecificAmmunitionGrade = ".45 ACP",
			AmmunitionCapacity = 1,
			RangedWeaponType = (int)RangedWeaponType.ModernFirearm,
			StaminaToFire = 7.0,
			StaminaPerLoadStage = 7.0,
			CoverBonus = -3.0,
			BaseAimDifficulty = (int)Difficulty.Easy,
			LoadDelay = 1.2,
			ReadyDelay = 0.3,
			FireDelay = 0.1,
			AimBonusLostPerShot = 0.6,
			RequiresFreeHandToReady = true,
			AlwaysRequiresTwoHandsToWield = false
		};
		context.RangedWeaponTypes.Add(ranged);
		context.SaveChanges();

		component = new GameItemComponentProto
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
			Type = "Gun",
			Name = "Pistol_45ACP",
			Description = "Turns an item into a .45 ACP Pistol",
			Definition =
				@$"<Definition>
   <LoadEmote><![CDATA[@ insert|inserts $2 into $1 and it clicks into place.]]></LoadEmote>
   <ReadyEmote><![CDATA[@ rack|racks the slide on $1, and it clicks back into place.]]></ReadyEmote>
   <UnloadEmote><![CDATA[@ hit|hits the eject button on $1 and $2 is ejected.]]></UnloadEmote>
   <UnreadyEmote><![CDATA[@ open|opens the slide on $1 and work|works out $2 from the chamber.]]></UnreadyEmote>
   <UnreadyEmoteNoChamberedRound><![CDATA[@ open|opens the slide on $1, but there is no round in the chamber.]]></UnreadyEmoteNoChamberedRound>
   <FireEmote><![CDATA[@ squeeze|squeezes the trigger on $2 and it fires a round with an extremely loud bang.]]></FireEmote>
   <FireEmoteNoChamberedRound><![CDATA[@ squeeze|squeezes the trigger on $2, and nothing happens except a quiet click.]]></FireEmoteNoChamberedRound>
   <RangedWeaponType>{ranged.Id}</RangedWeaponType>
 </Definition>"
		};
		context.GameItemComponentProtos.Add(component);
		context.SaveChanges();

		var holdable = context.GameItemComponentProtos.First(x => x.Type == "Holdable");
		var stackable = context.GameItemComponentProtos.First(x => x.Name == "Stack_Number");

		(long RoundId, long BulletId, long CaseId) CreateAmmoItems(string name, GameItemComponentProto ammoComponent,
			AmmunitionTypes ammo, string bulletMaterial, string caseMaterial)
		{
			var round = new GameItemProto
			{
				Id = context.GameItemProtos.Max(x => x.Id) + 1,
				Name = "round",
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
				Keywords = $"{name} round",
				Size = (int)SizeCategory.VerySmall,
				Weight = 9.0,
				ReadOnly = false,
				BaseItemQuality = (int)ItemQuality.Standard,
				ShortDescription = $"{name} round",
				FullDescription =
					$"This is a {name} round, made out of {caseMaterial} with a bullet made of {bulletMaterial} inside.",
				MaterialId = context.Materials.First(x => x.Name == caseMaterial).Id
			};
			round.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
			{
				GameItemProto = round,
				GameItemComponent = holdable
			});
			round.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
			{
				GameItemProto = round,
				GameItemComponent = stackable
			});
			round.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
			{
				GameItemProto = round,
				GameItemComponent = ammoComponent
			});
			context.GameItemProtos.Add(round);
			context.SaveChanges();

			var bullet = new GameItemProto
			{
				Id = context.GameItemProtos.Max(x => x.Id) + 1,
				Name = "bullet",
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
				Keywords = $"{name} bullet",
				Size = (int)SizeCategory.Tiny,
				Weight = 7.0,
				ReadOnly = false,
				BaseItemQuality = (int)ItemQuality.Standard,
				ShortDescription = $"@material {name} bullet",
				FullDescription =
					$"This is a tiny {name} bullet, made out of @material. It has clearly been fired out of a gun and is deformed.",
				MaterialId = context.Materials.First(x => x.Name == bulletMaterial).Id
			};
			bullet.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
			{
				GameItemProto = bullet,
				GameItemComponent = holdable
			});
			bullet.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
			{
				GameItemProto = bullet,
				GameItemComponent = stackable
			});
			context.GameItemProtos.Add(bullet);
			context.SaveChanges();

			var casing = new GameItemProto
			{
				Id = context.GameItemProtos.Max(x => x.Id) + 1,
				Name = "casing",
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
				Keywords = $"{name} casing",
				Size = (int)SizeCategory.Tiny,
				Weight = 2.0,
				ReadOnly = false,
				BaseItemQuality = (int)ItemQuality.Standard,
				ShortDescription = $"spent @material {name} casing",
				FullDescription =
					$"This is a tiny {name} casing made of @material. It has been fired and the firing cap and bullet are both gone.",
				MaterialId = context.Materials.First(x => x.Name == caseMaterial).Id
			};
			casing.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
			{
				GameItemProto = casing,
				GameItemComponent = holdable
			});
			casing.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
			{
				GameItemProto = casing,
				GameItemComponent = stackable
			});
			context.GameItemProtos.Add(casing);
			context.SaveChanges();

			return (round.Id, bullet.Id, casing.Id);
		}

		void AddAmmoType(string name, string specific, string shortname, string damage, DamageType damageType)
		{
			var ammo = new AmmunitionTypes
			{
				Name = name,
				SpecificType = specific,
				RangedWeaponTypes = "2",
				BaseAccuracy = 0.0,
				Loudness = (int)AudioVolume.ExtremelyLoud,
				BreakChanceOnHit = 0.2,
				BreakChanceOnMiss = 0.5,
				BaseBlockDifficulty = (int)Difficulty.Insane,
				BaseDodgeDifficulty = (int)Difficulty.Insane,
				DamageExpression = damage,
				StunExpression = damage,
				PainExpression = damage,
				DamageType = (int)damageType
			};
			context.AmmunitionTypes.Add(ammo);
			context.SaveChanges();

			component = new GameItemComponentProto
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
				Type = "Ammunition",
				Name = $"Ammo_{shortname}",
				Description = $"Turns an item into {shortname} rounds",
				Definition = ""
			};
			context.GameItemComponentProtos.Add(component);
			context.SaveChanges();

			var (roundid, bulletid, caseid) = CreateAmmoItems(name, component, ammo, "lead", "brass");
			component.Definition = @$"<Definition>
   <AmmoType>{ammo.Id}</AmmoType>
   <CasingProto>{caseid}</CasingProto>
   <BulletProto>{bulletid}</BulletProto>
 </Definition>";
			context.SaveChanges();
		}

		AddAmmoType("9mm Parabellum", "9x19mm Parabellum", "9mm", "(6+(pointblank*6))*quality*sqrt(degree+1)",
			DamageType.Ballistic);
		AddAmmoType("25ACP", ".25 ACP", ".25 ACP", "(4.5+(pointblank*6))*quality*sqrt(degree+1)", DamageType.Ballistic);
		AddAmmoType("32ACP", ".32 ACP", ".32 ACP", "(6+(pointblank*6))*quality*sqrt(degree+1)", DamageType.Ballistic);
		AddAmmoType("38ACP", ".38 ACP", ".38 ACP", "(6.5+(pointblank*6))*quality*sqrt(degree+1)", DamageType.Ballistic);
		AddAmmoType("38Super", ".38 Super ACP", ".38 Super ACP", "(7.4+(pointblank*6))*quality*sqrt(degree+1)",
			DamageType.Ballistic);
		AddAmmoType("45ACP", ".45 ACP", ".45 ACP", "(9+(pointblank*6))*quality*sqrt(degree+1)", DamageType.Ballistic);
	}

	private void SeedDataMuskets(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		// Do nothing, not yet implemented
	}

	private void SeedDataRanged(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> questionAnswers,
		IReadOnlyDictionary<string, TraitDefinition> skills)
	{
		var dbaccount = context.Accounts.First();
		var now = DateTime.UtcNow;
		var attributes = context.TraitDefinitions.Where(x => x.Type == 1 || x.Type == 3)
			.ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
		var strength =
			attributes.GetValueOrDefault("Strength") ??
			attributes.GetValueOrDefault("Physique") ??
			attributes["Body"];

		var expression = new TraitExpression
		{
			Name = "Bow Skill Cap",
			Expression = $"min(99,5.5*{strength.Alias}:{strength.Id})"
		};
		var bows = new TraitDefinition
		{
			Name = "Bows",
			Type = (int)TraitType.Skill,
			Expression = expression,
			TraitGroup = "Combat",
			AvailabilityProg = context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue"),
			TeachableProg = context.FutureProgs.First(x => x.FunctionName == "AlwaysFalse"),
			LearnableProg = context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue"),
			TeachDifficulty = 7,
			LearnDifficulty = 7,
			Hidden = false,
			ImproverId = context.Improvers.First(x => x.Name == "Skill Improver").Id,
			DecoratorId = context.TraitDecorators.First(x => x.Name == "General Skill").Id,
			DerivedType = 0,
			ChargenBlurb = string.Empty,
			BranchMultiplier = 1.0
		};
		context.TraitDefinitions.Add(bows);
		context.SaveChanges();

		var ranged = new RangedWeaponTypes
		{
			Name = "Shortbow",
			Classification = (int)WeaponClassification.Lethal,
			FireTrait = bows,
			OperateTrait = bows,
			FireableInMelee = false,
			DefaultRangeInRooms = 2,
			AccuracyBonusExpression = "(-1.0*range)+(pow(1-aim,2)*3.0)",
			DamageBonusExpression = $"quality - (4.0*range) - (({strength.Alias}:{strength.Id}-10)*1.5)",
			AmmunitionLoadType = (int)AmmunitionLoadType.Direct,
			SpecificAmmunitionGrade = "Arrow",
			AmmunitionCapacity = 1,
			RangedWeaponType = (int)RangedWeaponType.Bow,
			StaminaToFire = 5,
			StaminaPerLoadStage = 10.0,
			CoverBonus = -2.0,
			BaseAimDifficulty = (int)Difficulty.Normal,
			LoadDelay = 0.5,
			ReadyDelay = 0.1,
			FireDelay = 0.1,
			AimBonusLostPerShot = 1.0,
			RequiresFreeHandToReady = true,
			AlwaysRequiresTwoHandsToWield = false
		};
		context.RangedWeaponTypes.Add(ranged);
		context.SaveChanges();

		var component = new GameItemComponentProto
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
			Type = "Bow",
			Name = "Shortbow",
			Description = "Turns an item into a shortbow",
			Definition =
				$"<Definition><RangedWeaponType>{ranged.Id}</RangedWeaponType></Definition>"
		};
		context.GameItemComponentProtos.Add(component);
		context.SaveChanges();

		ranged = new RangedWeaponTypes
		{
			Name = "Longbow",
			Classification = (int)WeaponClassification.Lethal,
			FireTrait = bows,
			OperateTrait = bows,
			FireableInMelee = false,
			DefaultRangeInRooms = 4,
			AccuracyBonusExpression = "(-1.0*range)+(pow(1-aim,2)*2.0)",
			DamageBonusExpression = $"2*quality - (4.0*range) - (({strength.Alias}:{strength.Id}-10)*2.5)",
			AmmunitionLoadType = (int)AmmunitionLoadType.Direct,
			SpecificAmmunitionGrade = "Arrow",
			AmmunitionCapacity = 1,
			RangedWeaponType = (int)RangedWeaponType.Bow,
			StaminaToFire = 10,
			StaminaPerLoadStage = 15.0,
			CoverBonus = -1.0,
			BaseAimDifficulty = (int)Difficulty.Hard,
			LoadDelay = 0.5,
			ReadyDelay = 0.1,
			FireDelay = 0.1,
			AimBonusLostPerShot = 1.0,
			RequiresFreeHandToReady = true,
			AlwaysRequiresTwoHandsToWield = false
		};
		context.RangedWeaponTypes.Add(ranged);
		context.SaveChanges();

		component = new GameItemComponentProto
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
			Type = "Bow",
			Name = "Longbow",
			Description = "Turns an item into a longbow",
			Definition =
				$"<Definition><RangedWeaponType>{ranged.Id}</RangedWeaponType></Definition>"
		};
		context.GameItemComponentProtos.Add(component);
		context.SaveChanges();

		expression = new TraitExpression
		{
			Name = "Crossbows Skill Cap",
			Expression = $"min(99,5.5*{strength.Alias}:{strength.Id})"
		};
		var crossbows = new TraitDefinition
		{
			Name = "Crossbows",
			Type = (int)TraitType.Skill,
			Expression = expression,
			TraitGroup = "Combat",
			AvailabilityProg = context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue"),
			TeachableProg = context.FutureProgs.First(x => x.FunctionName == "AlwaysFalse"),
			LearnableProg = context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue"),
			TeachDifficulty = 7,
			LearnDifficulty = 7,
			Hidden = false,
			ImproverId = context.Improvers.First(x => x.Name == "Skill Improver").Id,
			DecoratorId = context.TraitDecorators.First(x => x.Name == "General Skill").Id,
			DerivedType = 0,
			ChargenBlurb = string.Empty,
			BranchMultiplier = 1.0
		};
		context.TraitDefinitions.Add(crossbows);
		context.SaveChanges();

		ranged = new RangedWeaponTypes
		{
			Name = "Hand Crossbow",
			Classification = (int)WeaponClassification.Lethal,
			FireTrait = crossbows,
			OperateTrait = crossbows,
			FireableInMelee = false,
			DefaultRangeInRooms = 2,
			AccuracyBonusExpression = "(-1.5*range)+(pow(1-aim,2)*5.0)",
			DamageBonusExpression = "quality - (4.0*range)",
			AmmunitionLoadType = (int)AmmunitionLoadType.Direct,
			SpecificAmmunitionGrade = "Bolt",
			AmmunitionCapacity = 1,
			RangedWeaponType = (int)RangedWeaponType.Crossbow,
			StaminaToFire = 1,
			StaminaPerLoadStage = 20.0,
			CoverBonus = -3.0,
			BaseAimDifficulty = (int)Difficulty.Normal,
			LoadDelay = 1.5,
			ReadyDelay = 0.1,
			FireDelay = 0.1,
			AimBonusLostPerShot = 1.0,
			RequiresFreeHandToReady = true,
			AlwaysRequiresTwoHandsToWield = false
		};
		context.RangedWeaponTypes.Add(ranged);
		context.SaveChanges();

		component = new GameItemComponentProto
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
			Type = "Crossbow",
			Name = "Hand Crossbow",
			Description = "Turns an item into a hand crossbow",
			Definition =
				$"<Definition><RangedWeaponType>{ranged.Id}</RangedWeaponType></Definition>"
		};
		context.GameItemComponentProtos.Add(component);
		context.SaveChanges();

		ranged = new RangedWeaponTypes
		{
			Name = "Crossbow",
			Classification = (int)WeaponClassification.Lethal,
			FireTrait = crossbows,
			OperateTrait = crossbows,
			FireableInMelee = false,
			DefaultRangeInRooms = 4,
			AccuracyBonusExpression = "(-1.0*range)+(pow(1-aim,2)*5.0)",
			DamageBonusExpression = "2*quality - (4.0*range)",
			AmmunitionLoadType = (int)AmmunitionLoadType.Direct,
			SpecificAmmunitionGrade = "Bolt",
			AmmunitionCapacity = 1,
			RangedWeaponType = (int)RangedWeaponType.Crossbow,
			StaminaToFire = 1,
			StaminaPerLoadStage = 30.0,
			CoverBonus = -3.0,
			BaseAimDifficulty = (int)Difficulty.Normal,
			LoadDelay = 2.0,
			ReadyDelay = 0.1,
			FireDelay = 0.1,
			AimBonusLostPerShot = 1.0,
			RequiresFreeHandToReady = true,
			AlwaysRequiresTwoHandsToWield = true
		};
		context.RangedWeaponTypes.Add(ranged);
		context.SaveChanges();

		component = new GameItemComponentProto
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
			Type = "Crossbow",
			Name = "Crossbow",
			Description = "Turns an item into a crossbow",
			Definition =
				$"<Definition><RangedWeaponType>{ranged.Id}</RangedWeaponType></Definition>"
		};
		context.GameItemComponentProtos.Add(component);
		context.SaveChanges();

		void AddAmmoType(string name, string specific, RangedWeaponType type, double accuracy, AudioVolume loudness,
			double breakOnHit, double breakOnMiss, Difficulty block, Difficulty dodge, DamageType damageType,
			string damageExpression)
		{
			var dbammo = new AmmunitionTypes
			{
				Name = name,
				SpecificType = specific,
				DamageType = (int)damageType,
				RangedWeaponTypes = ((int)type).ToString(),
				BaseAccuracy = accuracy,
				BreakChanceOnHit = breakOnHit,
				BreakChanceOnMiss = breakOnMiss,
				Loudness = (int)loudness,
				BaseBlockDifficulty = (int)block,
				BaseDodgeDifficulty = (int)dodge,
				DamageExpression = damageExpression,
				StunExpression = damageExpression,
				PainExpression = damageExpression
			};
			context.AmmunitionTypes.Add(dbammo);
			context.SaveChanges();

			component = new GameItemComponentProto
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
				Type = "Ammunition",
				Name = $"Ammo_{name.CollapseString()}",
				Description = $"Turns an item into {name.A_An()}",
				Definition = $"<Definition><AmmoType>{dbammo.Id}</AmmoType></Definition>"
			};
			context.GameItemComponentProtos.Add(component);
			context.SaveChanges();
		}

		AddAmmoType("Field Point Arrow", "Arrow", RangedWeaponType.Bow, 0.0, AudioVolume.Quiet, 0.5, 0.3,
			Difficulty.Easy, Difficulty.VeryHard, DamageType.ArmourPiercing, "15 + quality * 0.75 * degree");
		AddAmmoType("Broadhead Arrow", "Arrow", RangedWeaponType.Bow, 0.0, AudioVolume.Quiet, 0.5, 0.3, Difficulty.Easy,
			Difficulty.VeryHard, DamageType.Piercing, "30 + quality * 0.75 * degree");
		AddAmmoType("Concussive Arrow", "Arrow", RangedWeaponType.Bow, 0.0, AudioVolume.Quiet, 0.6, 0.4,
			Difficulty.Easy, Difficulty.VeryHard, DamageType.Crushing, "20 + quality * 0.75 * degree");
		AddAmmoType("Target Arrow", "Arrow", RangedWeaponType.Bow, 1.0, AudioVolume.Quiet, 0.05, 0.2, Difficulty.Easy,
			Difficulty.VeryHard, DamageType.Piercing, "quality * 0.75 * degree");
		AddAmmoType("Padded Arrow", "Arrow", RangedWeaponType.Bow, 0.0, AudioVolume.Quiet, 0.1, 0.2, Difficulty.Easy,
			Difficulty.VeryHard, DamageType.Crushing, "10-quality");
		AddAmmoType("Field Point Bolt", "Crossbow Bolt", RangedWeaponType.Crossbow, 0.0, AudioVolume.Quiet, 0.5, 0.3,
			Difficulty.Easy, Difficulty.VeryHard, DamageType.ArmourPiercing, "15 + quality * 0.75 * degree");
		AddAmmoType("Broadhead Bolt", "Crossbow Bolt", RangedWeaponType.Crossbow, 0.0, AudioVolume.Quiet, 0.5, 0.3,
			Difficulty.Easy, Difficulty.VeryHard, DamageType.Piercing, "30 + quality * 0.75 * degree");
		AddAmmoType("Concussive Bolt", "Crossbow Bolt", RangedWeaponType.Crossbow, 0.0, AudioVolume.Quiet, 0.6, 0.4,
			Difficulty.Easy, Difficulty.VeryHard, DamageType.Crushing, "20 + quality * 0.75 * degree");
		AddAmmoType("Target Bolt", "Crossbow Bolt", RangedWeaponType.Crossbow, 1.0, AudioVolume.Quiet, 0.05, 0.2,
			Difficulty.Easy, Difficulty.VeryHard, DamageType.Piercing, "quality * 0.75 * degree");
		AddAmmoType("Padded Bolt", "Crossbow Bolt", RangedWeaponType.Crossbow, 0.0, AudioVolume.Quiet, 0.1, 0.2,
			Difficulty.Easy, Difficulty.VeryHard, DamageType.Crushing, "10-quality");
		AddAmmoType("Sling Bullet", "Sling Bullet", RangedWeaponType.Sling, 0.0, AudioVolume.Quiet, 0.2, 0.2,
			Difficulty.Easy, Difficulty.Hard, DamageType.Crushing, "quality * 0.75 * degree");

		if (context.WeaponTypes.Any())
		{
			var throwing = skills.GetValueOrDefault("Throwing") ?? skills["Throw"];

			ranged = new RangedWeaponTypes
			{
				Name = "Throwing Knife",
				Classification = (int)WeaponClassification.Lethal,
				FireTrait = throwing,
				OperateTrait = throwing,
				FireableInMelee = false,
				DefaultRangeInRooms = 0,
				AccuracyBonusExpression = "(-1.5*range)+(pow(1-aim,2)*5.0)",
				DamageBonusExpression = $"quality/2 * ({strength.Alias}:{strength.Id}/2)",
				AmmunitionLoadType = (int)AmmunitionLoadType.Direct,
				SpecificAmmunitionGrade = "Throwing",
				AmmunitionCapacity = 1,
				RangedWeaponType = (int)RangedWeaponType.Thrown,
				StaminaToFire = 15.0,
				StaminaPerLoadStage = 0.0,
				CoverBonus = -3.0,
				BaseAimDifficulty = (int)Difficulty.VeryHard,
				LoadDelay = 0.0,
				ReadyDelay = 0.0,
				FireDelay = 0.5,
				AimBonusLostPerShot = 1.0,
				RequiresFreeHandToReady = true,
				AlwaysRequiresTwoHandsToWield = false
			};
			context.RangedWeaponTypes.Add(ranged);
			context.SaveChanges();

			component = new GameItemComponentProto
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
				Type = "ThrownWeapon",
				Name = "Throwing_Knife",
				Description = "Turns an item into a throwing knife",
				Definition =
					$"<Definition><RangedWeaponType>{ranged.Id}</RangedWeaponType><MeleeWeaponType>{context.WeaponTypes.First(x => x.Name == "Dagger").Id}</MeleeWeaponType></Definition>"
			};
			context.GameItemComponentProtos.Add(component);
			context.SaveChanges();

			ranged = new RangedWeaponTypes
			{
				Name = "Throwing Axe",
				Classification = (int)WeaponClassification.Lethal,
				FireTrait = throwing,
				OperateTrait = throwing,
				FireableInMelee = false,
				DefaultRangeInRooms = 0,
				AccuracyBonusExpression = "(-1.5*range)+(pow(1-aim,2)*5.0)",
				DamageBonusExpression = $"quality/2 * ({strength.Alias}:{strength.Id})",
				AmmunitionLoadType = (int)AmmunitionLoadType.Direct,
				SpecificAmmunitionGrade = "Throwing",
				AmmunitionCapacity = 1,
				RangedWeaponType = (int)RangedWeaponType.Thrown,
				StaminaToFire = 30.0,
				StaminaPerLoadStage = 0.0,
				CoverBonus = -3.0,
				BaseAimDifficulty = (int)Difficulty.VeryHard,
				LoadDelay = 0.0,
				ReadyDelay = 0.0,
				FireDelay = 0.5,
				AimBonusLostPerShot = 1.0,
				RequiresFreeHandToReady = true,
				AlwaysRequiresTwoHandsToWield = false
			};
			context.RangedWeaponTypes.Add(ranged);
			context.SaveChanges();

			component = new GameItemComponentProto
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
				Type = "ThrownWeapon",
				Name = "Throwing_Axe",
				Description = "Turns an item into a throwing axe",
				Definition =
					$"<Definition><RangedWeaponType>{ranged.Id}</RangedWeaponType><MeleeWeaponType>{context.WeaponTypes.First(x => x.Name == "Axe").Id}</MeleeWeaponType></Definition>"
			};
			context.GameItemComponentProtos.Add(component);
			context.SaveChanges();

			ranged = new RangedWeaponTypes
			{
				Name = "Throwing Spear",
				Classification = (int)WeaponClassification.Lethal,
				FireTrait = throwing,
				OperateTrait = throwing,
				FireableInMelee = false,
				DefaultRangeInRooms = 1,
				AccuracyBonusExpression = "(-3.0*range)+(pow(1-aim,2)*5.0)",
				DamageBonusExpression = $"quality * ({strength.Alias}:{strength.Id})",
				AmmunitionLoadType = (int)AmmunitionLoadType.Direct,
				SpecificAmmunitionGrade = "Throwing",
				AmmunitionCapacity = 1,
				RangedWeaponType = (int)RangedWeaponType.Thrown,
				StaminaToFire = 30.0,
				StaminaPerLoadStage = 0.0,
				CoverBonus = -2.0,
				BaseAimDifficulty = (int)Difficulty.VeryHard,
				LoadDelay = 0.0,
				ReadyDelay = 0.0,
				FireDelay = 0.5,
				AimBonusLostPerShot = 1.0,
				RequiresFreeHandToReady = true,
				AlwaysRequiresTwoHandsToWield = false
			};
			context.RangedWeaponTypes.Add(ranged);
			context.SaveChanges();

			component = new GameItemComponentProto
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
				Type = "ThrownWeapon",
				Name = "Throwing_Spear",
				Description = "Turns an item into a throwing spear",
				Definition =
					$"<Definition><RangedWeaponType>{ranged.Id}</RangedWeaponType><MeleeWeaponType>{context.WeaponTypes.First(x => x.Name == "Short Spear").Id}</MeleeWeaponType></Definition>"
			};
			context.GameItemComponentProtos.Add(component);
			context.SaveChanges();
		}
	}

	private void SeedDataWeapons(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> questionAnswers,
		IReadOnlyDictionary<string, TraitDefinition> coreSkills)
	{
		var attributes = context.TraitDefinitions.Where(x => x.Type == 1 || x.Type == 3)
			.ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
		var strength =
			attributes.GetValueOrDefault("Strength") ??
			attributes.GetValueOrDefault("Physique") ??
			attributes["Body"];

		var dex =
			attributes.GetValueOrDefault("Agility") ??
			attributes.GetValueOrDefault("Dexterity") ??
			attributes.GetValueOrDefault("Agility") ??
			attributes.GetValueOrDefault("Speed") ??
			attributes["Body"];

		var parryoption = questionAnswers["parryoption"].ToLowerInvariant();
		var skilloption = questionAnswers["skilloption"].ToLowerInvariant();

		var attackAddendum = "";
		switch (questionAnswers["messagestyle"].ToLowerInvariant())
		{
			case "sentences":
			case "sparse":
				attackAddendum = ".";
				break;
		}

		var randomPortion = "";
		var startingmultiplier = 1.0;
		switch (questionAnswers["random"].ToLowerInvariant())
		{
			case "static":
				randomPortion = "";
				startingmultiplier = 0.6;
				break;
			case "partial":
				randomPortion = " * rand(0.7,1.0)";
				startingmultiplier = 0.705882;
				break;
			case "random":
				randomPortion = " * rand(0.2,1.0)";
				startingmultiplier = 1.0;
				break;
		}

		var trainingDamage = new TraitExpression
		{
			Name = "Weapon Damage - Training",
			Expression = $"max(1,10-quality){randomPortion}"
		};
		context.TraitExpressions.Add(trainingDamage);
		context.SaveChanges();

		var terribleDamage = new TraitExpression
		{
			Name = "Weapon Damage - Terrible",
			Expression =
				$"max(1,{0.1 * startingmultiplier} * (str:{strength.Id} * quality) * sqrt(degree+1){randomPortion})"
		};
		context.TraitExpressions.Add(terribleDamage);
		context.SaveChanges();
		var badDamage = new TraitExpression
		{
			Name = "Weapon Damage - Bad",
			Expression =
				$"max(1,{0.2 * startingmultiplier} * (str:{strength.Id} * quality) * sqrt(degree+1){randomPortion})"
		};
		context.TraitExpressions.Add(badDamage);
		context.SaveChanges();
		var poorDamage = new TraitExpression
		{
			Name = "Weapon Damage - Poor",
			Expression =
				$"max(1,{0.25 * startingmultiplier} * (str:{strength.Id} * quality) * sqrt(degree+1){randomPortion})"
		};
		context.TraitExpressions.Add(poorDamage);
		context.SaveChanges();
		var normalDamage = new TraitExpression
		{
			Name = "Weapon Damage - Normal",
			Expression =
				$"max(1,{0.3 * startingmultiplier} * (str:{strength.Id} * quality) * sqrt(degree+1){randomPortion})"
		};
		context.TraitExpressions.Add(normalDamage);
		context.SaveChanges();
		var goodDamage = new TraitExpression
		{
			Name = "Weapon Damage - Good",
			Expression =
				$"max(1,{0.4 * startingmultiplier} * (str:{strength.Id} * quality) * sqrt(degree+1){randomPortion})"
		};
		context.TraitExpressions.Add(goodDamage);
		context.SaveChanges();
		var veryGoodDamage = new TraitExpression
		{
			Name = "Weapon Damage - Very Good",
			Expression =
				$"max(1,{0.45 * startingmultiplier} * (str:{strength.Id} * quality) * sqrt(degree+1){randomPortion})"
		};
		context.TraitExpressions.Add(veryGoodDamage);
		context.SaveChanges();
		var greatDamage = new TraitExpression
		{
			Name = "Weapon Damage - Great",
			Expression =
				$"max(1,{0.5 * startingmultiplier} * (str:{strength.Id} * quality) * sqrt(degree+1){randomPortion})"
		};
		context.TraitExpressions.Add(greatDamage);
		context.SaveChanges();
		var coupdegraceDamage = new TraitExpression
		{
			Name = "Weapon Damage - Coup de Grace",
			Expression =
				$"max(1,{1.0 * startingmultiplier} * str:{strength.Id} * quality * sqrt(degree+1){randomPortion})"
		};
		context.TraitExpressions.Add(coupdegraceDamage);
		context.SaveChanges();

		void AddAttack(string name, BuiltInCombatMoveType moveType, MeleeWeaponVerb verb, Difficulty attacker,
			Difficulty dodge, Difficulty parry, Difficulty block, Alignment alignment, Orientation orientation,
			double stamina, double relativeSpeed, WeaponType type, TraitExpression damage, string attackMessage,
			DamageType damageType = DamageType.Crushing, double weighting = 100,
			CombatMoveIntentions intentions =
				CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill,
			string additionalInfo = null, AttackHandednessOptions handedness = AttackHandednessOptions.Any)
		{
			var attack = new WeaponAttack
			{
				Verb = (int)verb,
				BaseAttackerDifficulty = (int)attacker,
				BaseBlockDifficulty = (int)block,
				BaseDodgeDifficulty = (int)dodge,
				BaseParryDifficulty = (int)parry,
				MoveType = (int)moveType,
				RecoveryDifficultySuccess = (int)Difficulty.Easy,
				RecoveryDifficultyFailure = (int)Difficulty.Hard,
				Intentions = (long)intentions,
				Weighting = weighting,
				ExertionLevel = (int)ExertionLevel.Heavy,
				DamageType = (int)damageType,
				DamageExpression = damage,
				StunExpression = damage,
				PainExpression = damage,
				WeaponTypeId = type.Id,
				StaminaCost = stamina,
				BaseDelay = relativeSpeed,
				Name = name,
				Orientation = (int)orientation,
				Alignment = (int)alignment,
				HandednessOptions = (int)handedness,
				AdditionalInfo = additionalInfo
			};
			context.WeaponAttacks.Add(attack);
			context.SaveChanges();

			var message = new CombatMessage
			{
				Type = (int)moveType,
				Message = $"{attackMessage}{attackAddendum}",
				Priority = 50,
				Verb = (int)verb,
				Chance = 1.0,
				FailureMessage = $"{attackMessage}{attackAddendum}"
			};
			message.CombatMessagesWeaponAttacks.Add(new CombatMessagesWeaponAttacks
				{ CombatMessage = message, WeaponAttack = attack });
			context.CombatMessages.Add(message);
			context.SaveChanges();
		}

		var armourUseSkill = context.TraitDefinitions.FirstOrDefault(x => x.Name == "Armour Use");
		var useStats = armourUseSkill?.Expression.Expression != "70";

		TraitDefinition CreateSkill(string name)
		{
			var expression = new TraitExpression
			{
				Name = $"{name} Skill Cap",
				Expression = useStats ? $"min(99,2*{strength.Alias}:{strength.Id}+3*{dex.Alias}:{dex.Id})" : "70"
			};
			var skill = new TraitDefinition
			{
				Name = name,
				Type = (int)TraitType.Skill,
				Expression = expression,
				TraitGroup = "Combat",
				AvailabilityProg = context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue"),
				TeachableProg = context.FutureProgs.First(x => x.FunctionName == "AlwaysFalse"),
				LearnableProg = context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue"),
				TeachDifficulty = 7,
				LearnDifficulty = 7,
				Hidden = false,
				ImproverId = context.Improvers.First(x => x.Name == "Skill Improver").Id,
				DecoratorId = context.TraitDecorators.First(x => x.Name == "General Skill").Id,
				DerivedType = 0,
				ChargenBlurb = string.Empty,
				BranchMultiplier = 1.0
			};
			context.TraitDefinitions.Add(skill);
			context.SaveChanges();
			return skill;
		}

		var dbaccount = context.Accounts.First();
		var now = DateTime.UtcNow;

		void CreateWeaponComponent(WeaponType type)
		{
			var component = new GameItemComponentProto
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
				Type = "MeleeWeapon",
				Name = $"Melee_{type.Name}",
				Description = $"Turns an item into a {type.Name} melee weapon",
				Definition =
					$"<Definition><WeaponType>{type.Id}</WeaponType></Definition>"
			};
			context.GameItemComponentProtos.Add(component);
			context.SaveChanges();
		}

		var parrySkill =
			coreSkills.GetValueOrDefault("Parrying") ?? coreSkills.GetValueOrDefault("Parry");

		var skills = new Dictionary<string, TraitDefinition>(StringComparer.InvariantCultureIgnoreCase);
		var parrySkills = new Dictionary<string, TraitDefinition>(StringComparer.InvariantCultureIgnoreCase);
		TraitDefinition skill;
		switch (skilloption)
		{
			case "soi":
				skill = CreateSkill("Light-Blunt");
				skills["Mace"] = skill;
				parrySkills["Mace"] = parrySkill ?? skill;
				skill = CreateSkill("Light-Edge");
				skills["Short Sword"] = skill;
				parrySkills["Short Sword"] = parrySkill ?? skill;
				skill = CreateSkill("Light-Pierce");
				skills["Dagger"] = skill;
				parrySkills["Dagger"] = parrySkill ?? skill;
				skill = CreateSkill("Medium-Blunt");
				skills["Club"] = skill;
				parrySkills["Club"] = parrySkill ?? skill;
				skills["Improvised"] = skill;
				parrySkills["Improvised"] = parrySkill ?? skill;
				skill = CreateSkill("Medium-Edge");
				skills["Long Sword"] = skill;
				parrySkills["Long Sword"] = parrySkill ?? skill;
				skills["Axe"] = skill;
				parrySkills["Axe"] = parrySkill ?? skill;
				skill = CreateSkill("Medium-Pierce");
				skills["Rapier"] = skill;
				parrySkills["Rapier"] = parrySkill ?? skill;
				skill = CreateSkill("Heavy-Blunt");
				skills["Warhammer"] = skill;
				parrySkills["Warhammer"] = parrySkill ?? skill;
				skill = CreateSkill("Heavy-Edge");
				skills["Two Handed Sword"] = skill;
				parrySkills["Two Handed Sword"] = parrySkill ?? skill;
				skill = CreateSkill("Heavy-Pierce");
				skills["Spear"] = skill;
				parrySkills["Spear"] = parrySkill ?? skill;
				skill = CreateSkill("Staff");
				skill = CreateSkill("Polearm");
				skill = CreateSkill("Dual-Wielding");
				break;
			case "broad":
				skill = CreateSkill("Bludgeoning Weapons");
				skills["Mace"] = skill;
				parrySkills["Mace"] = parrySkill ?? skill;
				skills["Club"] = skill;
				parrySkills["Club"] = parrySkill ?? skill;
				skills["Improvised"] = skill;
				parrySkills["Improvised"] = parrySkill ?? skill;
				skills["Warhammer"] = skill;
				parrySkills["Warhammer"] = parrySkill ?? skill;

				skill = CreateSkill("Edged Weapons");
				skills["Short Sword"] = skill;
				parrySkills["Short Sword"] = parrySkill ?? skill;
				skills["Dagger"] = skill;
				parrySkills["Dagger"] = parrySkill ?? skill;
				skills["Long Sword"] = skill;
				parrySkills["Long Sword"] = parrySkill ?? skill;
				skills["Axe"] = skill;
				parrySkills["Axe"] = parrySkill ?? skill;
				skills["Rapier"] = skill;
				parrySkills["Rapier"] = parrySkill ?? skill;

				skill = CreateSkill("Two Handed Weapons");
				skills["Warhammer"] = skill;
				parrySkills["Warhammer"] = parrySkill ?? skill;
				skills["Two Handed Sword"] = skill;
				parrySkills["Two Handed Sword"] = parrySkill ?? skill;
				skills["Spear"] = skill;
				parrySkills["Spear"] = parrySkill ?? skill;
				break;
			case "weapons":
				skill = CreateSkill("Maces");
				skills["Mace"] = skill;
				parrySkills["Mace"] = parrySkill ?? skill;
				skill = CreateSkill("Swords");
				skills["Short Sword"] = skill;
				parrySkills["Short Sword"] = parrySkill ?? skill;
				skills["Long Sword"] = skill;
				parrySkills["Long Sword"] = parrySkill ?? skill;
				skills["Rapier"] = skill;
				parrySkills["Rapier"] = parrySkill ?? skill;
				skill = CreateSkill("Daggers");
				skills["Dagger"] = skill;
				parrySkills["Dagger"] = parrySkill ?? skill;
				skill = CreateSkill("Clubs");
				skills["Club"] = skill;
				parrySkills["Club"] = parrySkill ?? skill;
				skills["Improvised"] = skill;
				parrySkills["Improvised"] = parrySkill ?? skill;
				skill = CreateSkill("Axes");
				skills["Axe"] = skill;
				parrySkills["Axe"] = parrySkill ?? skill;
				skill = CreateSkill("Warhammers");
				skills["Warhammer"] = skill;
				parrySkills["Warhammer"] = parrySkill ?? skill;
				skill = CreateSkill("Two Handed Swords");
				skills["Two Handed Sword"] = skill;
				parrySkills["Two Handed Sword"] = parrySkill ?? skill;
				skill = CreateSkill("Polearms");
				skills["Spear"] = skill;
				parrySkills["Spear"] = parrySkill ?? skill;
				break;
		}

		#region Dagger

		var dagger = new WeaponType
		{
			Name = "Dagger",
			Classification = (int)WeaponClassification.Lethal,
			AttackTrait = skills["Dagger"],
			ParryTrait = parrySkills["Dagger"],
			ParryBonus = 1,
			Reach = 1,
			StaminaPerParry = 1.5
		};
		context.WeaponTypes.Add(dagger);
		context.SaveChanges();
		CreateWeaponComponent(dagger);
		var trainingDagger = new WeaponType
		{
			Name = "Training Dagger",
			Classification = (int)WeaponClassification.Training,
			AttackTrait = skills["Dagger"],
			ParryTrait = parrySkills["Dagger"],
			ParryBonus = 1,
			Reach = 1,
			StaminaPerParry = 1.5
		};
		context.WeaponTypes.Add(trainingDagger);
		context.SaveChanges();
		CreateWeaponComponent(trainingDagger);

		AddAttack("Dagger Slash", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing, Difficulty.Easy,
			Difficulty.ExtremelyEasy, Difficulty.ExtremelyEasy, Difficulty.Trivial, Alignment.FrontRight,
			Orientation.High, 3.0, 0.9, dagger, terribleDamage, "@ swing|swings $2 across &0's body at $1",
			DamageType.Slashing);
		AddAttack("Dagger Reverse Slash", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing, Difficulty.Easy,
			Difficulty.ExtremelyEasy, Difficulty.ExtremelyEasy, Difficulty.Trivial, Alignment.Front, Orientation.High,
			3.0, 0.9, dagger, terribleDamage, "@ swing|swings $2 in a reverse slash at $1", DamageType.Slashing);
		AddAttack("Dagger Lunge", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust, Difficulty.Normal,
			Difficulty.Easy, Difficulty.Easy, Difficulty.VeryEasy, Alignment.Front, Orientation.High, 4.0, 1.0, dagger,
			badDamage, "@ lunge|lunges forward and attempt|attempts to stab $1 with $2", DamageType.Piercing);
		AddAttack("Dagger High Lunge", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust, Difficulty.Normal,
			Difficulty.VeryEasy, Difficulty.VeryEasy, Difficulty.VeryEasy, Alignment.Front, Orientation.Highest, 4.0,
			1.0, dagger, badDamage, "@ lunge|lunges forward and attempt|attempts to stab $1 with $2",
			DamageType.Piercing);
		AddAttack("Dagger Low Lunge", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust, Difficulty.Normal,
			Difficulty.Easy, Difficulty.Easy, Difficulty.VeryEasy, Alignment.Front, Orientation.Low, 4.0, 1.0, dagger,
			badDamage, "@ lunge|lunges forward and attempt|attempts to stab $1 with $2", DamageType.Piercing);
		AddAttack("Dagger Jab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Jab, Difficulty.Easy,
			Difficulty.VeryEasy, Difficulty.Easy, Difficulty.ExtremelyEasy, Alignment.Front, Orientation.High, 3.0, 0.6,
			dagger, terribleDamage, "@ lash|lashes out with a quick jab of $2 at $1", DamageType.Piercing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
			            CombatMoveIntentions.Fast);
		AddAttack("Dagger Stab", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Stab, Difficulty.VeryEasy,
			Difficulty.VeryHard, Difficulty.Insane, Difficulty.Easy, Alignment.FrontRight, Orientation.High, 3.0, 0.4,
			dagger, badDamage, "@ stab|stabs $1 with $2", DamageType.Piercing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
			            CombatMoveIntentions.Fast);
		AddAttack("Dagger Low Stab", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Stab, Difficulty.VeryEasy,
			Difficulty.VeryHard, Difficulty.Insane, Difficulty.Easy, Alignment.FrontRight, Orientation.Centre, 3.0, 0.4,
			dagger, badDamage, "@ stab|stabs $1 with $2", DamageType.Piercing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
			            CombatMoveIntentions.Fast);
		AddAttack("Dagger Leg Stab", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Stab, Difficulty.VeryEasy,
			Difficulty.VeryHard, Difficulty.Insane, Difficulty.Normal, Alignment.FrontRight, Orientation.Low, 3.0, 0.4,
			dagger, badDamage, "@ stab|stabs down at $1's leg with $2", DamageType.Piercing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
			            CombatMoveIntentions.Fast | CombatMoveIntentions.Hinder);
		AddAttack("Dagger Arm Stab", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Stab, Difficulty.Normal,
			Difficulty.VeryHard, Difficulty.Insane, Difficulty.Normal, Alignment.FrontRight, Orientation.Appendage, 3.0,
			0.4, dagger, badDamage, "@ stab|stabs out at $1's arm with $2", DamageType.Piercing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
			            CombatMoveIntentions.Fast | CombatMoveIntentions.Disarm);

		AddAttack("Dagger Throat Cut", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Stab, Difficulty.Normal,
			Difficulty.Easy, Difficulty.Easy, Difficulty.VeryEasy, Alignment.Front, Orientation.High, 4.0, 1.0, dagger,
			veryGoodDamage, "@ run|runs $2 in a deep slash of $1's throat from ear to ear.", DamageType.Slashing,
			additionalInfo: context.BodypartProtos.First(x => x.Name == "throat").Id.ToString());
		AddAttack("Dagger Throat Stab", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Stab, Difficulty.Normal,
			Difficulty.Easy, Difficulty.Easy, Difficulty.VeryEasy, Alignment.Front, Orientation.High, 4.0, 1.0, dagger,
			veryGoodDamage, "@ plunge|plunges $2 deep into $1's throat.", DamageType.Piercing,
			additionalInfo: context.BodypartProtos.First(x => x.Name == "throat").Id.ToString());
		AddAttack("Dagger Smash", BuiltInCombatMoveType.MeleeWeaponSmashItem, MeleeWeaponVerb.Stab, Difficulty.VeryEasy,
			Difficulty.VeryHard, Difficulty.Insane, Difficulty.Easy, Alignment.FrontRight, Orientation.High, 3.0, 0.4,
			dagger, badDamage, "@ stab|stabs $1 with $2", DamageType.Piercing);

		AddAttack("Training Dagger Slash", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.VeryEasy, Difficulty.ExtremelyEasy, Difficulty.ExtremelyEasy, Difficulty.Trivial,
			Alignment.FrontRight, Orientation.High, 3.0, 0.9, trainingDagger, trainingDamage,
			"@ swing|swings $2 across &0's body at $1", DamageType.Crushing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Dagger Reverse Slash", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.VeryEasy, Difficulty.ExtremelyEasy, Difficulty.ExtremelyEasy, Difficulty.Trivial,
			Alignment.Front, Orientation.High, 3.0, 0.9, trainingDagger, trainingDamage,
			"@ swing|swings $2 in a reverse slash at $1", DamageType.Crushing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Dagger Lunge", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust,
			Difficulty.VeryEasy, Difficulty.Easy, Difficulty.Easy, Difficulty.VeryEasy, Alignment.Front,
			Orientation.High, 4.0, 1.0, trainingDagger, trainingDamage,
			"@ lunge|lunges forward and attempt|attempts to stab $1 with $2", DamageType.Crushing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Dagger High Lunge", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust,
			Difficulty.VeryEasy, Difficulty.VeryEasy, Difficulty.VeryEasy, Difficulty.VeryEasy, Alignment.Front,
			Orientation.Highest, 4.0, 1.0, trainingDagger, trainingDamage,
			"@ lunge|lunges forward and attempt|attempts to stab $1 with $2", DamageType.Crushing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Dagger Low Lunge", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust,
			Difficulty.VeryEasy, Difficulty.Easy, Difficulty.Easy, Difficulty.VeryEasy, Alignment.Front,
			Orientation.Low, 4.0, 1.0, trainingDagger, trainingDamage,
			"@ lunge|lunges forward and attempt|attempts to stab $1 with $2", DamageType.Crushing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Dagger Jab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Jab,
			Difficulty.VeryEasy, Difficulty.VeryEasy, Difficulty.Easy, Difficulty.ExtremelyEasy, Alignment.Front,
			Orientation.High, 3.0, 0.6, trainingDagger, trainingDamage,
			"@ lash|lashes out with a quick jab of $2 at $1", DamageType.Crushing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast);
		AddAttack("Training Dagger Stab", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Stab, Difficulty.VeryEasy,
			Difficulty.VeryHard, Difficulty.Insane, Difficulty.Easy, Alignment.FrontRight, Orientation.High, 3.0, 0.4,
			trainingDagger, trainingDamage, "@ stab|stabs $1 with $2", DamageType.Crushing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast);
		AddAttack("Training Dagger Low Stab", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Stab,
			Difficulty.VeryEasy, Difficulty.VeryHard, Difficulty.Insane, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Centre, 3.0, 0.4, trainingDagger, trainingDamage, "@ stab|stabs $1 with $2",
			DamageType.Crushing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast);
		AddAttack("Training Dagger Leg Stab", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Stab,
			Difficulty.VeryEasy, Difficulty.VeryHard, Difficulty.Insane, Difficulty.Normal, Alignment.FrontRight,
			Orientation.Low, 3.0, 0.4, trainingDagger, trainingDamage, "@ stab|stabs down at $1's leg with $2",
			DamageType.Crushing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast |
			            CombatMoveIntentions.Hinder);
		AddAttack("Training Dagger Arm Stab", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Stab,
			Difficulty.VeryEasy, Difficulty.VeryHard, Difficulty.Insane, Difficulty.Normal, Alignment.FrontRight,
			Orientation.Appendage, 3.0, 0.4, trainingDagger, trainingDamage, "@ stab|stabs out at $1's arm with $2",
			DamageType.Crushing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast |
			            CombatMoveIntentions.Disarm);

		#endregion

		#region Clubs, Maces, Improvised

		var club = new WeaponType
		{
			Name = "Club",
			Classification = (int)WeaponClassification.NonLethal,
			AttackTrait = skills["Club"],
			ParryTrait = parrySkills["Club"],
			ParryBonus = 0,
			Reach = 2,
			StaminaPerParry = 3.0
		};
		context.WeaponTypes.Add(club);
		context.SaveChanges();
		CreateWeaponComponent(club);
		var trainingClub = new WeaponType
		{
			Name = "Training Club",
			Classification = (int)WeaponClassification.Training,
			AttackTrait = skills["Club"],
			ParryTrait = parrySkills["Club"],
			ParryBonus = 0,
			Reach = 2,
			StaminaPerParry = 3.0
		};
		context.WeaponTypes.Add(trainingClub);
		context.SaveChanges();
		CreateWeaponComponent(trainingClub);
		var mace = new WeaponType
		{
			Name = "Mace",
			Classification = (int)WeaponClassification.NonLethal,
			AttackTrait = skills["Mace"],
			ParryTrait = parrySkills["Mace"],
			ParryBonus = -1,
			Reach = 1,
			StaminaPerParry = 2.0
		};
		context.WeaponTypes.Add(mace);
		context.SaveChanges();
		CreateWeaponComponent(mace);
		var trainingmace = new WeaponType
		{
			Name = "Mace",
			Classification = (int)WeaponClassification.NonLethal,
			AttackTrait = skills["Mace"],
			ParryTrait = parrySkills["Mace"],
			ParryBonus = -1,
			Reach = 1,
			StaminaPerParry = 2.0
		};
		context.WeaponTypes.Add(trainingmace);
		context.SaveChanges();
		CreateWeaponComponent(trainingmace);
		var improvised = new WeaponType
		{
			Name = "Improvised Bludgeon",
			Classification = (int)WeaponClassification.Improvised,
			AttackTrait = skills["Improvised"],
			ParryTrait = parrySkills["Improvised"],
			ParryBonus = -2,
			Reach = 0,
			StaminaPerParry = 3.0
		};
		context.WeaponTypes.Add(improvised);
		context.SaveChanges();
		context.StaticConfigurations.Add(new StaticConfiguration
			{ SettingName = "DefaultBowMeleeWeaponType", Definition = improvised.Id.ToString() });
		context.StaticConfigurations.Add(new StaticConfiguration
			{ SettingName = "DefaultCrossbowMeleeWeaponType", Definition = improvised.Id.ToString() });
		context.StaticConfigurations.Add(new StaticConfiguration
			{ SettingName = "DefaultGunMeleeWeaponType", Definition = improvised.Id.ToString() });
		CreateWeaponComponent(improvised);

		AddAttack("Club 1-Handed Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight,
			Orientation.High, 5.0, 1.0, club, normalDamage, "@ swing|swings $2 at $1", DamageType.Crushing,
			handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Club 1-Handed High Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
			Orientation.Highest, 5.0, 1.1, club, normalDamage, "@ swing|swings $2 in a high blow at $1",
			DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Club 1-Handed Low Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Centre, 5.0, 1.0, club, normalDamage, "@ swing|swings $2 in a low blow at $1",
			DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Club 1-Handed Leg Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Low, 5.0, 1.1, club, normalDamage, "@ swing|swings $2 at $1's legs", DamageType.Crushing,
			handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Club 1-Handed Arm Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Hard, Difficulty.VeryEasy, Alignment.FrontRight,
			Orientation.Appendage, 5.0, 1.1, club, normalDamage, "@ swing|swings $2 at $1's arms", DamageType.Crushing,
			handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Club 1-Handed Overhead Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Hard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
			Orientation.Highest, 6.0, 1.3, club, goodDamage, "@ swing|swings $2 in an overhead blow at $1",
			DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Club 1-Handed Haft Bash", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Bash,
			Difficulty.VeryHard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
			Orientation.Highest, 4.0, 1.3, club, badDamage, "@ heave|heaves the haft of $2 down towards $1's head");

		AddAttack("Club Smash", BuiltInCombatMoveType.MeleeWeaponSmashItem, MeleeWeaponVerb.Swing, Difficulty.Easy,
			Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 6.0, 1.3, club,
			goodDamage, "@ swing|swings $2 at $1");
		AddAttack("Club Crush Head", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
			Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5, club,
			coupdegraceDamage,
			"@ hold|holds $2 up above &0's head and bring|brings it down at $1 in a devastating swing",
			DamageType.Crushing, additionalInfo: context.BodypartProtos.First(x => x.Name == "scalp").Id.ToString());
		AddAttack("Club Crush Neck", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
			Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5, club,
			coupdegraceDamage,
			"@ hold|holds $2 up above &0's head and bring|brings it down at $1 in a devastating swing",
			DamageType.Crushing, additionalInfo: context.BodypartProtos.First(x => x.Name == "bneck").Id.ToString());
		AddAttack("Club Crush Right Hand", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
			Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5, club,
			coupdegraceDamage,
			"@ hold|holds $2 up above &0's head and bring|brings it down at $1 in a devastating swing",
			DamageType.Crushing, additionalInfo: context.BodypartProtos.First(x => x.Name == "rhand").Id.ToString());
		AddAttack("Club Crush Left Hand", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
			Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5, club,
			coupdegraceDamage,
			"@ hold|holds $2 up above &0's head and bring|brings it down at $1 in a devastating swing",
			DamageType.Crushing, additionalInfo: context.BodypartProtos.First(x => x.Name == "lhand").Id.ToString());
		AddAttack("Club Crush Right Leg", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
			Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5, club,
			coupdegraceDamage,
			"@ hold|holds $2 up above &0's head and bring|brings it down at $1 in a devastating swing",
			DamageType.Crushing, additionalInfo: context.BodypartProtos.First(x => x.Name == "rshin").Id.ToString());
		AddAttack("Club Crush Left Leg", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
			Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5, club,
			coupdegraceDamage,
			"@ hold|holds $2 up above &0's head and bring|brings it down at $1 in a devastating swing",
			DamageType.Crushing, additionalInfo: context.BodypartProtos.First(x => x.Name == "lshin").Id.ToString());


		AddAttack("Club 2-Handed Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing, Difficulty.Normal,
			Difficulty.Normal, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight, Orientation.High, 6.0, 1.0, club,
			normalDamage, "@ swing|swings $2 at $1", DamageType.Crushing,
			handedness: AttackHandednessOptions.TwoHandedOnly, additionalInfo:
			((int)Difficulty.Normal).ToString());
		AddAttack("Club 2-Handed High Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
			Orientation.Highest, 6.0, 1.1, club, normalDamage, "@ swing|swings $2 in a high blow at $1",
			DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly, additionalInfo:
			((int)Difficulty.Normal).ToString());
		AddAttack("Club 2-Handed Low Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Centre, 6.0, 1.0, club, normalDamage, "@ swing|swings $2 in a low blow at $1",
			DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly, additionalInfo:
			((int)Difficulty.Normal).ToString());
		AddAttack("Club 2-Handed Leg Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Low, 6.0, 1.1, club, normalDamage, "@ swing|swings $2 at $1's legs", DamageType.Crushing,
			handedness: AttackHandednessOptions.TwoHandedOnly, additionalInfo:
			((int)Difficulty.Normal).ToString());
		AddAttack("Club 2-Handed Arm Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Hard, Difficulty.VeryEasy, Alignment.FrontRight,
			Orientation.Appendage, 6.0, 1.1, club, normalDamage, "@ swing|swings $2 at $1's arms", DamageType.Crushing,
			handedness: AttackHandednessOptions.TwoHandedOnly, additionalInfo:
			((int)Difficulty.Normal).ToString());
		AddAttack("Club 2-Handed Overhead Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
			Difficulty.Hard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
			Orientation.Highest, 7.5, 1.3, club, goodDamage, "@ swing|swings $2 in an overhead blow at $1",
			DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly, additionalInfo:
			((int)Difficulty.Normal).ToString());

		AddAttack("Training Club 1-Handed Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight,
			Orientation.High, 5.0, 1.0, trainingClub, trainingDamage, "@ swing|swings $2 at $1", DamageType.Crushing,
			handedness: AttackHandednessOptions.OneHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Club 1-Handed High Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.VeryEasy, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
			Orientation.Highest, 5.0, 1.1, trainingClub, trainingDamage, "@ swing|swings $2 in a high blow at $1",
			DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Club 1-Handed Low Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Centre, 5.0, 1.0, trainingClub, trainingDamage, "@ swing|swings $2 in a low blow at $1",
			DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Club 1-Handed Leg Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.VeryEasy, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Low, 5.0, 1.1, trainingClub, trainingDamage, "@ swing|swings $2 at $1's legs",
			DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Club 1-Handed Arm Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.VeryEasy, Difficulty.Easy, Difficulty.Hard, Difficulty.VeryEasy, Alignment.FrontRight,
			Orientation.Appendage, 5.0, 1.1, trainingClub, trainingDamage, "@ swing|swings $2 at $1's arms",
			DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Club 1-Handed Overhead Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.VeryEasy, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
			Orientation.Highest, 6.0, 1.3, trainingClub, trainingDamage, "@ swing|swings $2 in an overhead blow at $1",
			DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);

		AddAttack("Training Club 2-Handed Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight,
			Orientation.High, 6.0, 1.0, trainingClub, trainingDamage, "@ swing|swings $2 at $1", DamageType.Crushing,
			handedness: AttackHandednessOptions.TwoHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Club 2-Handed High Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.VeryEasy, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
			Orientation.Highest, 6.0, 1.1, trainingClub, trainingDamage, "@ swing|swings $2 in a high blow at $1",
			DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Club 2-Handed Low Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Centre, 6.0, 1.0, trainingClub, trainingDamage, "@ swing|swings $2 in a low blow at $1",
			DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Club 2-Handed Leg Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.VeryEasy, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Low, 6.0, 1.1, trainingClub, trainingDamage, "@ swing|swings $2 at $1's legs",
			DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Club 2-Handed Arm Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.VeryEasy, Difficulty.Easy, Difficulty.Hard, Difficulty.VeryEasy, Alignment.FrontRight,
			Orientation.Appendage, 6.0, 1.1, trainingClub, trainingDamage, "@ swing|swings $2 at $1's arms",
			DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Club 2-Handed Overhead Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.VeryEasy, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
			Orientation.Highest, 7.5, 1.3, trainingClub, trainingDamage, "@ swing|swings $2 in an overhead blow at $1",
			DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);

		AddAttack("Mace Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing, Difficulty.Normal,
			Difficulty.Normal, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight, Orientation.High, 4.0, 0.85,
			mace, poorDamage, "@ swing|swings $2 at $1");
		AddAttack("Mace High Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing, Difficulty.Normal,
			Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight, Orientation.Highest, 4.0, 0.95,
			mace, poorDamage, "@ swing|swings $2 in a high blow at $1");
		AddAttack("Mace Low Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing, Difficulty.Normal,
			Difficulty.Normal, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight, Orientation.Centre, 4.0, 0.85,
			mace, poorDamage, "@ swing|swings $2 in a low blow at $1");
		AddAttack("Mace Leg Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing, Difficulty.Normal,
			Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight, Orientation.Low, 4.0, 0.95, mace,
			poorDamage, "@ swing|swings $2 at $1's legs");
		AddAttack("Mace Arm Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing, Difficulty.Normal,
			Difficulty.Easy, Difficulty.Hard, Difficulty.VeryEasy, Alignment.FrontRight, Orientation.Appendage, 4.0,
			0.95, mace, poorDamage, "@ swing|swings $2 at $1's arms");
		AddAttack("Mace Overhead Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing, Difficulty.Hard,
			Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 5.0, 1.15,
			mace, normalDamage, "@ swing|swings $2 in an overhead blow at $1");
		AddAttack("Mace Haft Bash", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Bash, Difficulty.VeryHard,
			Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 3.0, 1.15,
			mace, badDamage, "@ heave|heaves the haft of $2 down towards $1's head");

		AddAttack("Mace Smash", BuiltInCombatMoveType.MeleeWeaponSmashItem, MeleeWeaponVerb.Swing, Difficulty.Easy,
			Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 6.0, 1.3, mace,
			goodDamage, "@ swing|swings $2 at $1");
		AddAttack("Mace Crush Head", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
			Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5, mace,
			greatDamage, "@ hold|holds $2 up above &0's head and bring|brings it down at $1 in a devastating swing",
			DamageType.Crushing, additionalInfo: context.BodypartProtos.First(x => x.Name == "scalp").Id.ToString());

		AddAttack("Training Mace Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight,
			Orientation.High, 4.0, 0.85, trainingmace, trainingDamage, "@ swing|swings $2 at $1");
		AddAttack("Training Mace High Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
			Orientation.Highest, 4.0, 0.95, trainingmace, trainingDamage, "@ swing|swings $2 in a high blow at $1",
			DamageType.Crushing, intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Mace Low Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Centre, 4.0, 0.85, trainingmace, trainingDamage, "@ swing|swings $2 in a low blow at $1",
			DamageType.Crushing, intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Mace Leg Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Low, 4.0, 0.95, trainingmace, trainingDamage, "@ swing|swings $2 at $1's legs",
			DamageType.Crushing, intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Mace Arm Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Hard, Difficulty.VeryEasy, Alignment.FrontRight,
			Orientation.Appendage, 4.0, 0.95, trainingmace, trainingDamage, "@ swing|swings $2 at $1's arms",
			DamageType.Crushing, intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Mace Overhead Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Hard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
			Orientation.Highest, 5.0, 1.15, trainingmace, trainingDamage, "@ swing|swings $2 in an overhead blow at $1",
			DamageType.Crushing, intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Mace Haft Bash", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Bash,
			Difficulty.VeryHard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
			Orientation.Highest, 3.0, 1.15, trainingmace, trainingDamage,
			"@ heave|heaves the haft of $2 down towards $1's head", DamageType.Crushing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);

		AddAttack("Improvised Bludgeon Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Hard, Difficulty.Normal, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight,
			Orientation.High, 4.0, 0.85, improvised, poorDamage, "@ swing|swings $2 at $1");
		AddAttack("Improvised Bludgeon High Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Hard, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
			Orientation.Highest, 4.0, 0.95, improvised, poorDamage, "@ swing|swings $2 in a high blow at $1");
		AddAttack("Improvised Bludgeon Low Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Hard, Difficulty.Normal, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Centre, 4.0, 0.85, improvised, poorDamage, "@ swing|swings $2 in a low blow at $1");
		AddAttack("Improvised Bludgeon Leg Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Hard, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight, Orientation.Low,
			4.0, 0.95, improvised, poorDamage, "@ swing|swings $2 at $1's legs");
		AddAttack("Improvised Bludgeon Arm Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Hard, Difficulty.Easy, Difficulty.Hard, Difficulty.VeryEasy, Alignment.FrontRight,
			Orientation.Appendage, 4.0, 0.95, improvised, poorDamage, "@ swing|swings $2 at $1's arms");
		AddAttack("Improvised Bludgeon Overhead Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.VeryHard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
			Orientation.Highest, 5.0, 1.15, improvised, normalDamage, "@ swing|swings $2 in an overhead blow at $1");
		AddAttack("Improvised Bludgeon Clinch Bash", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Bash,
			Difficulty.ExtremelyHard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
			Orientation.Highest, 3.0, 1.15, improvised, terribleDamage, "@ heave|heaves $2 down towards $1's head");

		AddAttack("Improvised Bludgeon Smash", BuiltInCombatMoveType.MeleeWeaponSmashItem, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
			Orientation.Highest, 6.0, 1.3, improvised, badDamage, "@ swing|swings $2 at $1");
		AddAttack("Improvised Bludgeon Crush Head", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing,
			Difficulty.VeryHard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
			Orientation.Highest, 8.0, 1.5, improvised, goodDamage,
			"@ hold|holds $2 up above &0's head and bring|brings it down at $1 in a devastating swing",
			DamageType.Crushing, additionalInfo: context.BodypartProtos.First(x => x.Name == "scalp").Id.ToString());

		#endregion

		#region Swords

		#region Shortsword

		var shortsword = new WeaponType
		{
			Name = "Shortsword",
			Classification = (int)WeaponClassification.Lethal,
			AttackTrait = skills["Short Sword"],
			ParryTrait = parrySkills["Short Sword"],
			ParryBonus = 0,
			Reach = 2,
			StaminaPerParry = 2.0
		};
		context.WeaponTypes.Add(shortsword);
		context.SaveChanges();
		CreateWeaponComponent(shortsword);
		var trainingshortsword = new WeaponType
		{
			Name = "Training Shortsword",
			Classification = (int)WeaponClassification.Training,
			AttackTrait = skills["Short Sword"],
			ParryTrait = parrySkills["Short Sword"],
			ParryBonus = 0,
			Reach = 2,
			StaminaPerParry = 2.0
		};
		context.WeaponTypes.Add(trainingshortsword);
		context.SaveChanges();
		CreateWeaponComponent(trainingshortsword);

		AddAttack("Shortsword Slash", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing, Difficulty.Normal,
			Difficulty.ExtremelyEasy, Difficulty.ExtremelyEasy, Difficulty.Trivial, Alignment.FrontRight,
			Orientation.High, 4.2, 1.0, shortsword, badDamage, "@ swing|swings $2 across &0's body at $1",
			DamageType.Slashing);
		AddAttack("Shortsword Reverse Slash", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.ExtremelyEasy, Difficulty.ExtremelyEasy, Difficulty.Trivial, Alignment.Front,
			Orientation.High, 4.2, 1.0, shortsword, badDamage, "@ swing|swings $2 in a reverse slash at $1",
			DamageType.Slashing);
		AddAttack("Shortsword Lunge", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust, Difficulty.Normal,
			Difficulty.Easy, Difficulty.Easy, Difficulty.VeryEasy, Alignment.Front, Orientation.High, 5.0, 1.2,
			shortsword, poorDamage, "@ lunge|lunges forward and attempt|attempts to stab $1 with $2",
			DamageType.Piercing);
		AddAttack("Shortsword High Lunge", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust,
			Difficulty.Normal, Difficulty.VeryEasy, Difficulty.VeryEasy, Difficulty.VeryEasy, Alignment.Front,
			Orientation.Highest, 5.0, 1.2, shortsword, poorDamage,
			"@ lunge|lunges forward and attempt|attempts to stab $1 with $2", DamageType.Piercing);
		AddAttack("Shortsword Low Lunge", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.VeryEasy, Alignment.Front, Orientation.Low,
			5.0, 1.2, shortsword, poorDamage, "@ lunge|lunges forward and attempt|attempts to stab $1 with $2",
			DamageType.Piercing);
		AddAttack("Shortsword Jab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Jab, Difficulty.Easy,
			Difficulty.VeryEasy, Difficulty.Easy, Difficulty.ExtremelyEasy, Alignment.Front, Orientation.High, 4.2, 0.8,
			shortsword, badDamage, "@ lash|lashes out with a quick jab of $2 at $1", DamageType.Piercing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
			            CombatMoveIntentions.Fast);
		AddAttack("Shortsword Stab", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Stab, Difficulty.Normal,
			Difficulty.VeryHard, Difficulty.Insane, Difficulty.Easy, Alignment.FrontRight, Orientation.High, 4.2, 0.7,
			shortsword, poorDamage, "@ stab|stabs $1 with $2", DamageType.Piercing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
			            CombatMoveIntentions.Fast);
		AddAttack("Shortsword Low Stab", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Stab, Difficulty.Normal,
			Difficulty.VeryHard, Difficulty.Insane, Difficulty.Easy, Alignment.FrontRight, Orientation.Centre, 4.2, 0.7,
			shortsword, poorDamage, "@ stab|stabs $1 with $2", DamageType.Piercing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
			            CombatMoveIntentions.Fast);
		AddAttack("Shortsword Leg Stab", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Stab, Difficulty.Normal,
			Difficulty.VeryHard, Difficulty.Insane, Difficulty.Normal, Alignment.FrontRight, Orientation.Low, 4.2, 0.7,
			shortsword, poorDamage, "@ stab|stabs down at $1's leg with $2", DamageType.Piercing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
			            CombatMoveIntentions.Fast | CombatMoveIntentions.Hinder);
		AddAttack("Shortsword Arm Stab", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Stab, Difficulty.Hard,
			Difficulty.VeryHard, Difficulty.Insane, Difficulty.Normal, Alignment.FrontRight, Orientation.Appendage, 4.2,
			0.7, shortsword, poorDamage, "@ stab|stabs out at $1's arm with $2", DamageType.Piercing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
			            CombatMoveIntentions.Fast | CombatMoveIntentions.Disarm);

		AddAttack("Shortsword Throat Cut", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Stab, Difficulty.Hard,
			Difficulty.Easy, Difficulty.Easy, Difficulty.VeryEasy, Alignment.Front, Orientation.High, 5.0, 1.0,
			shortsword, goodDamage, "@ run|runs $2 in a deep slash of $1's throat from ear to ear.",
			DamageType.Slashing, additionalInfo: context.BodypartProtos.First(x => x.Name == "throat").Id.ToString());
		AddAttack("Shortsword Throat Stab", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Stab, Difficulty.Normal,
			Difficulty.Easy, Difficulty.Easy, Difficulty.VeryEasy, Alignment.Front, Orientation.High, 5.0, 1.0,
			shortsword, goodDamage, "@ plunge|plunges $2 deep into $1's throat.", DamageType.Piercing,
			additionalInfo: context.BodypartProtos.First(x => x.Name == "throat").Id.ToString());
		AddAttack("Shortsword Smash", BuiltInCombatMoveType.MeleeWeaponSmashItem, MeleeWeaponVerb.Stab,
			Difficulty.VeryEasy, Difficulty.VeryHard, Difficulty.Insane, Difficulty.Easy, Alignment.FrontRight,
			Orientation.High, 4.2, 0.4, shortsword, poorDamage, "@ stab|stabs $1 with $2", DamageType.Piercing);

		AddAttack("Training Shortsword Slash", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.VeryEasy, Difficulty.ExtremelyEasy, Difficulty.ExtremelyEasy, Difficulty.Trivial,
			Alignment.FrontRight, Orientation.High, 4.2, 1.0, trainingshortsword, trainingDamage,
			"@ swing|swings $2 across &0's body at $1", DamageType.Crushing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Shortsword Reverse Slash", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.VeryEasy, Difficulty.ExtremelyEasy, Difficulty.ExtremelyEasy, Difficulty.Trivial,
			Alignment.Front, Orientation.High, 4.2, 1.0, trainingshortsword, trainingDamage,
			"@ swing|swings $2 in a reverse slash at $1", DamageType.Crushing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Shortsword Lunge", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust,
			Difficulty.VeryEasy, Difficulty.Easy, Difficulty.Easy, Difficulty.VeryEasy, Alignment.Front,
			Orientation.High, 5.0, 1.2, trainingshortsword, trainingDamage,
			"@ lunge|lunges forward and attempt|attempts to stab $1 with $2", DamageType.Crushing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Shortsword High Lunge", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust,
			Difficulty.VeryEasy, Difficulty.VeryEasy, Difficulty.VeryEasy, Difficulty.VeryEasy, Alignment.Front,
			Orientation.Highest, 5.0, 1.2, trainingshortsword, trainingDamage,
			"@ lunge|lunges forward and attempt|attempts to stab $1 with $2", DamageType.Crushing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Shortsword Low Lunge", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust,
			Difficulty.VeryEasy, Difficulty.Easy, Difficulty.Easy, Difficulty.VeryEasy, Alignment.Front,
			Orientation.Low, 5.0, 1.2, trainingshortsword, trainingDamage,
			"@ lunge|lunges forward and attempt|attempts to stab $1 with $2", DamageType.Crushing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Shortsword Jab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Jab,
			Difficulty.VeryEasy, Difficulty.VeryEasy, Difficulty.Easy, Difficulty.ExtremelyEasy, Alignment.Front,
			Orientation.High, 4.2, 0.8, trainingshortsword, trainingDamage,
			"@ lash|lashes out with a quick jab of $2 at $1", DamageType.Crushing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Shortsword Stab", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Stab,
			Difficulty.VeryEasy, Difficulty.VeryHard, Difficulty.Insane, Difficulty.Easy, Alignment.FrontRight,
			Orientation.High, 4.2, 0.7, trainingshortsword, trainingDamage, "@ stab|stabs $1 with $2",
			DamageType.Crushing, intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Shortsword Low Stab", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Stab,
			Difficulty.VeryEasy, Difficulty.VeryHard, Difficulty.Insane, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Centre, 4.2, 0.7, trainingshortsword, trainingDamage, "@ stab|stabs $1 with $2",
			DamageType.Crushing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast);
		AddAttack("Training Shortsword Leg Stab", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Stab,
			Difficulty.VeryEasy, Difficulty.VeryHard, Difficulty.Insane, Difficulty.Normal, Alignment.FrontRight,
			Orientation.Low, 4.2, 0.7, trainingshortsword, trainingDamage, "@ stab|stabs down at $1's leg with $2",
			DamageType.Crushing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast |
			            CombatMoveIntentions.Hinder);
		AddAttack("Training Shortsword Arm Stab", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Stab,
			Difficulty.VeryEasy, Difficulty.VeryHard, Difficulty.Insane, Difficulty.Normal, Alignment.FrontRight,
			Orientation.Appendage, 4.2, 0.7, trainingshortsword, trainingDamage, "@ stab|stabs out at $1's arm with $2",
			DamageType.Crushing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast |
			            CombatMoveIntentions.Disarm);

		#endregion

		#region Longswords and 2Handers

		var longsword = new WeaponType
		{
			Name = "Longsword",
			Classification = (int)WeaponClassification.Lethal,
			AttackTrait = skills["Long Sword"],
			ParryTrait = parrySkills["Long Sword"],
			ParryBonus = 1,
			Reach = 3,
			StaminaPerParry = 3.0
		};
		context.WeaponTypes.Add(longsword);
		context.SaveChanges();
		CreateWeaponComponent(longsword);
		var traininglongsword = new WeaponType
		{
			Name = "Training Longsword",
			Classification = (int)WeaponClassification.Training,
			AttackTrait = skills["Long Sword"],
			ParryTrait = parrySkills["Long Sword"],
			ParryBonus = 1,
			Reach = 3,
			StaminaPerParry = 3.0
		};
		context.WeaponTypes.Add(traininglongsword);
		context.SaveChanges();
		CreateWeaponComponent(traininglongsword);

		var zweihander = new WeaponType
		{
			Name = "Two Handed Sword",
			Classification = (int)WeaponClassification.Lethal,
			AttackTrait = skills["Two Handed Sword"],
			ParryTrait = parrySkills["Two Handed Sword"],
			ParryBonus = 0,
			Reach = 4,
			StaminaPerParry = 5.0
		};
		context.WeaponTypes.Add(zweihander);
		context.SaveChanges();
		CreateWeaponComponent(zweihander);
		var trainingzweihander = new WeaponType
		{
			Name = "Training Two Handed Sword",
			Classification = (int)WeaponClassification.Training,
			AttackTrait = skills["Two Handed Sword"],
			ParryTrait = parrySkills["Two Handed Sword"],
			ParryBonus = 0,
			Reach = 4,
			StaminaPerParry = 5.0
		};
		context.WeaponTypes.Add(trainingzweihander);
		context.SaveChanges();
		CreateWeaponComponent(trainingzweihander);

		AddAttack("Longsword 1-Handed Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.High, 5.0, 1.0, longsword, normalDamage, "@ swing|swings $2 at $1", DamageType.Slashing,
			handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Longsword 1-Handed High Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
			Orientation.Highest, 5.0, 1.1, longsword, normalDamage, "@ swing|swings $2 in a high blow at $1",
			DamageType.Slashing, handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Longsword 1-Handed Low Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Centre, 5.0, 1.0, longsword, normalDamage, "@ swing|swings $2 in a low blow at $1",
			DamageType.Slashing, handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Longsword 1-Handed Leg Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Low, 5.0, 1.1, longsword, normalDamage, "@ swing|swings $2 at $1's legs", DamageType.Slashing,
			handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Longsword 1-Handed Arm Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.VeryEasy, Alignment.FrontRight,
			Orientation.Appendage, 5.0, 1.1, longsword, normalDamage, "@ swing|swings $2 at $1's arms",
			DamageType.Slashing, handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Longsword 1-Handed Overhead Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Hard, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.Front, Orientation.Highest,
			6.0, 1.3, longsword, goodDamage, "@ swing|swings $2 in an overhead blow at $1", DamageType.Slashing,
			handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Longsword 1-Handed Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.High, 5.0, 1.0, longsword, normalDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
			DamageType.Piercing, handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Longsword 1-Handed High Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Highest, 5.0, 1.0, longsword, normalDamage,
			"@ lunge|lunges forward and stab|stabs $2 high at $1", DamageType.Piercing,
			handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Longsword 1-Handed Low Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
			Orientation.Low, 5.0, 1.0, longsword, normalDamage, "@ lunge|lunges forward and stab|stabs $2 low at $1",
			DamageType.Piercing, handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Longsword 1-Handed Foot Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.VeryHard, Alignment.FrontRight,
			Orientation.Lowest, 5.0, 1.0, longsword, normalDamage,
			"@ lunge|lunges forward and stab|stabs $2 at $1's feet", DamageType.Piercing,
			handedness: AttackHandednessOptions.OneHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Kill | CombatMoveIntentions.Wound |
			            CombatMoveIntentions.Hinder);
		AddAttack("Longsword 1-Handed Jab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Jab, Difficulty.Easy,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight, Orientation.High, 5.0, 0.8,
			longsword, badDamage, "@ quickly jab|jabs $2 at $1", DamageType.Piercing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Kill | CombatMoveIntentions.Wound |
			            CombatMoveIntentions.Fast, handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Longsword 1-Handed Counter Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
			Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.High, 5.0, 0.8, longsword, badDamage, "@ quickly counter jab|jabs $2 at $1",
			DamageType.Piercing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Kill | CombatMoveIntentions.Wound |
			            CombatMoveIntentions.Fast, handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Longsword 1-Handed Low Counter Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
			Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Centre, 5.0, 0.8, longsword, badDamage, "@ quickly counter jab|jabs $2 at $1",
			DamageType.Piercing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Kill | CombatMoveIntentions.Wound |
			            CombatMoveIntentions.Fast, handedness: AttackHandednessOptions.OneHandedOnly);

		AddAttack("Longsword Smash", BuiltInCombatMoveType.MeleeWeaponSmashItem, MeleeWeaponVerb.Swing, Difficulty.Easy,
			Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 6.0, 1.3,
			longsword, goodDamage, "@ swing|swings $2 at $1", DamageType.Slashing);
		AddAttack("Longsword Slash Head", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
			Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5,
			longsword, coupdegraceDamage,
			"@ hold|holds $2 up above &0's head and bring|brings it down at $1's {0} in a devastating swing",
			DamageType.Slashing, additionalInfo: context.BodypartProtos.First(x => x.Name == "scalp").Id.ToString());
		AddAttack("Longsword Slash Neck", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
			Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5,
			longsword, coupdegraceDamage,
			"@ hold|holds $2 up above &0's head and bring|brings it down at $1's {0} in a devastating swing",
			DamageType.Slashing, additionalInfo: context.BodypartProtos.First(x => x.Name == "bneck").Id.ToString());
		AddAttack("Longsword Slash Right Hand", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing,
			Difficulty.Hard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
			Orientation.Highest, 8.0, 1.5, longsword, coupdegraceDamage,
			"@ hold|holds $2 up above &0's head and bring|brings it down at $1's {0} in a devastating swing",
			DamageType.Slashing, additionalInfo: context.BodypartProtos.First(x => x.Name == "rhand").Id.ToString());
		AddAttack("Longsword Slash Left Hand", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing,
			Difficulty.Hard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
			Orientation.Highest, 8.0, 1.5, longsword, coupdegraceDamage,
			"@ hold|holds $2 up above &0's head and bring|brings it down at $1's {0} in a devastating swing",
			DamageType.Slashing, additionalInfo: context.BodypartProtos.First(x => x.Name == "lhand").Id.ToString());
		AddAttack("Longsword Slash Right Leg", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing,
			Difficulty.Hard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
			Orientation.Highest, 8.0, 1.5, longsword, coupdegraceDamage,
			"@ hold|holds $2 up above &0's head and bring|brings it down at $1's {0} in a devastating swing",
			DamageType.Slashing, additionalInfo: context.BodypartProtos.First(x => x.Name == "rshin").Id.ToString());
		AddAttack("Longsword Slash Left Leg", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
			Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5,
			longsword, coupdegraceDamage,
			"@ hold|holds $2 up above &0's head and bring|brings it down at $1's {0} in a devastating swing",
			DamageType.Slashing, additionalInfo: context.BodypartProtos.First(x => x.Name == "lshin").Id.ToString());
		AddAttack("Longsword Chest Stab", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
			Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5,
			longsword, coupdegraceDamage,
			"@ position|positions $2 above $1's heart and thrust|thrusts down in a brutal stab", DamageType.Piercing,
			additionalInfo: context.BodypartProtos.First(x => x.Name == "rbreast").Id.ToString());
		AddAttack("Longsword Throat Stab", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
			Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5,
			longsword, coupdegraceDamage,
			"@ position|positions $2 above $1's throat and thrust|thrusts down in a brutal stab", DamageType.Piercing,
			additionalInfo: context.BodypartProtos.First(x => x.Name == "throat").Id.ToString());


		AddAttack("Longsword 2-Handed Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.High, 5.0, 0.9, longsword, normalDamage, "@ swing|swings $2 at $1", DamageType.Slashing,
			handedness: AttackHandednessOptions.TwoHandedOnly);
		AddAttack("Longsword 2-Handed High Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
			Orientation.Highest, 5.0, 1.0, longsword, normalDamage, "@ swing|swings $2 in a high blow at $1",
			DamageType.Slashing, handedness: AttackHandednessOptions.TwoHandedOnly);
		AddAttack("Longsword 2-Handed Low Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Centre, 5.0, 0.9, longsword, normalDamage, "@ swing|swings $2 in a low blow at $1",
			DamageType.Slashing, handedness: AttackHandednessOptions.TwoHandedOnly);
		AddAttack("Longsword 2-Handed Leg Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Low, 5.0, 1.0, longsword, normalDamage, "@ swing|swings $2 at $1's legs", DamageType.Slashing,
			handedness: AttackHandednessOptions.TwoHandedOnly);
		AddAttack("Longsword 2-Handed Arm Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.VeryEasy, Alignment.FrontRight,
			Orientation.Appendage, 5.0, 1.0, longsword, normalDamage, "@ swing|swings $2 at $1's arms",
			DamageType.Slashing, handedness: AttackHandednessOptions.TwoHandedOnly);
		AddAttack("Longsword 2-Handed Overhead Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Hard, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.Front, Orientation.Highest,
			6.0, 1.2, longsword, goodDamage, "@ swing|swings $2 in an overhead blow at $1", DamageType.Slashing,
			handedness: AttackHandednessOptions.TwoHandedOnly);
		AddAttack("Longsword 2-Handed Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.High, 5.0, 0.9, longsword, normalDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
			DamageType.Piercing, handedness: AttackHandednessOptions.TwoHandedOnly);
		AddAttack("Longsword 2-Handed High Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Highest, 5.0, 0.9, longsword, normalDamage,
			"@ lunge|lunges forward and stab|stabs $2 high at $1", DamageType.Piercing,
			handedness: AttackHandednessOptions.TwoHandedOnly);
		AddAttack("Longsword 2-Handed Low Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
			Orientation.Low, 5.0, 0.9, longsword, normalDamage, "@ lunge|lunges forward and stab|stabs $2 low at $1",
			DamageType.Piercing, handedness: AttackHandednessOptions.TwoHandedOnly);
		AddAttack("Longsword 2-Handed Foot Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.VeryHard, Alignment.FrontRight,
			Orientation.Lowest, 5.0, 0.9, longsword, normalDamage,
			"@ lunge|lunges forward and stab|stabs $2 at $1's feet", DamageType.Piercing,
			handedness: AttackHandednessOptions.TwoHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Kill | CombatMoveIntentions.Wound |
			            CombatMoveIntentions.Hinder);
		AddAttack("Longsword 2-Handed Jab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Jab, Difficulty.Easy,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight, Orientation.High, 5.0, 0.7,
			longsword, badDamage, "@ quickly jab|jabs $2 at $1", DamageType.Piercing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Kill | CombatMoveIntentions.Wound |
			            CombatMoveIntentions.Fast, handedness: AttackHandednessOptions.TwoHandedOnly);
		AddAttack("Longsword 2-Handed Counter Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
			Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.High, 5.0, 0.7, longsword, badDamage, "@ quickly counter jab|jabs $2 at $1",
			DamageType.Piercing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Kill | CombatMoveIntentions.Wound |
			            CombatMoveIntentions.Fast, handedness: AttackHandednessOptions.TwoHandedOnly);
		AddAttack("Longsword 2-Handed Low Counter Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
			Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Centre, 5.0, 0.7, longsword, badDamage, "@ quickly counter jab|jabs $2 at $1",
			DamageType.Piercing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Kill | CombatMoveIntentions.Wound |
			            CombatMoveIntentions.Fast, handedness: AttackHandednessOptions.TwoHandedOnly);


		AddAttack("Training Longsword 1-Handed Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.High, 5.0, 1.0, traininglongsword, trainingDamage, "@ swing|swings $2 at $1",
			DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Longsword 1-Handed High Swing", BuiltInCombatMoveType.UseWeaponAttack,
			MeleeWeaponVerb.Swing, Difficulty.Easy, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal,
			Alignment.FrontRight, Orientation.Highest, 5.0, 1.1, traininglongsword, trainingDamage,
			"@ swing|swings $2 in a high blow at $1", DamageType.Crushing,
			handedness: AttackHandednessOptions.OneHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Longsword 1-Handed Low Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Centre, 5.0, 1.0, traininglongsword, trainingDamage, "@ swing|swings $2 in a low blow at $1",
			DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Longsword 1-Handed Leg Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Easy, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight, Orientation.Low,
			5.0, 1.1, traininglongsword, trainingDamage, "@ swing|swings $2 at $1's legs", DamageType.Crushing,
			handedness: AttackHandednessOptions.OneHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Longsword 1-Handed Arm Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Easy, Difficulty.Easy, Difficulty.Normal, Difficulty.VeryEasy, Alignment.FrontRight,
			Orientation.Appendage, 5.0, 1.1, traininglongsword, trainingDamage, "@ swing|swings $2 at $1's arms",
			DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Longsword 1-Handed Overhead Swing", BuiltInCombatMoveType.UseWeaponAttack,
			MeleeWeaponVerb.Swing, Difficulty.Easy, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy,
			Alignment.Front, Orientation.Highest, 6.0, 1.3, traininglongsword, trainingDamage,
			"@ swing|swings $2 in an overhead blow at $1", DamageType.Crushing,
			handedness: AttackHandednessOptions.OneHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Longsword 1-Handed Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.High, 5.0, 1.0, traininglongsword, trainingDamage,
			"@ lunge|lunges forward and stab|stabs $2 at $1", DamageType.Crushing,
			handedness: AttackHandednessOptions.OneHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Longsword 1-Handed High Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Highest, 5.0, 1.0, traininglongsword, trainingDamage,
			"@ lunge|lunges forward and stab|stabs $2 high at $1", DamageType.Crushing,
			handedness: AttackHandednessOptions.OneHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Longsword 1-Handed Low Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
			Orientation.Low, 5.0, 1.0, traininglongsword, trainingDamage,
			"@ lunge|lunges forward and stab|stabs $2 low at $1", DamageType.Crushing,
			handedness: AttackHandednessOptions.OneHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Longsword 1-Handed Foot Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Easy, Difficulty.Easy, Difficulty.Easy, Difficulty.VeryHard, Alignment.FrontRight,
			Orientation.Lowest, 5.0, 1.0, traininglongsword, trainingDamage,
			"@ lunge|lunges forward and stab|stabs $2 at $1's feet", DamageType.Crushing,
			handedness: AttackHandednessOptions.OneHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Hinder);
		AddAttack("Training Longsword 1-Handed Jab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Jab,
			Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.High, 5.0, 0.8, traininglongsword, trainingDamage, "@ quickly jab|jabs $2 at $1",
			DamageType.Crushing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast,
			handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Training Longsword 2-Handed Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.High, 5.0, 0.9, traininglongsword, trainingDamage, "@ swing|swings $2 at $1",
			DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Longsword 2-Handed High Swing", BuiltInCombatMoveType.UseWeaponAttack,
			MeleeWeaponVerb.Swing, Difficulty.Easy, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal,
			Alignment.FrontRight, Orientation.Highest, 5.0, 1.0, traininglongsword, trainingDamage,
			"@ swing|swings $2 in a high blow at $1", DamageType.Crushing,
			handedness: AttackHandednessOptions.TwoHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Longsword 2-Handed Low Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Centre, 5.0, 0.9, traininglongsword, trainingDamage, "@ swing|swings $2 in a low blow at $1",
			DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Longsword 2-Handed Leg Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Easy, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight, Orientation.Low,
			5.0, 1.0, traininglongsword, trainingDamage, "@ swing|swings $2 at $1's legs", DamageType.Crushing,
			handedness: AttackHandednessOptions.TwoHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Longsword 2-Handed Arm Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Easy, Difficulty.Easy, Difficulty.Normal, Difficulty.VeryEasy, Alignment.FrontRight,
			Orientation.Appendage, 5.0, 1.0, traininglongsword, trainingDamage, "@ swing|swings $2 at $1's arms",
			DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Longsword 2-Handed Overhead Swing", BuiltInCombatMoveType.UseWeaponAttack,
			MeleeWeaponVerb.Swing, Difficulty.Easy, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy,
			Alignment.Front, Orientation.Highest, 6.0, 1.2, traininglongsword, trainingDamage,
			"@ swing|swings $2 in an overhead blow at $1", DamageType.Crushing,
			handedness: AttackHandednessOptions.TwoHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Longsword 2-Handed Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.High, 5.0, 0.9, traininglongsword, trainingDamage,
			"@ lunge|lunges forward and stab|stabs $2 at $1", DamageType.Crushing,
			handedness: AttackHandednessOptions.TwoHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Longsword 2-Handed High Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Highest, 5.0, 0.9, traininglongsword, trainingDamage,
			"@ lunge|lunges forward and stab|stabs $2 high at $1", DamageType.Crushing,
			handedness: AttackHandednessOptions.TwoHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Longsword 2-Handed Low Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
			Orientation.Low, 5.0, 0.9, traininglongsword, trainingDamage,
			"@ lunge|lunges forward and stab|stabs $2 low at $1", DamageType.Crushing,
			handedness: AttackHandednessOptions.TwoHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Longsword 2-Handed Foot Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Easy, Difficulty.Easy, Difficulty.Easy, Difficulty.VeryHard, Alignment.FrontRight,
			Orientation.Lowest, 5.0, 0.9, traininglongsword, trainingDamage,
			"@ lunge|lunges forward and stab|stabs $2 at $1's feet", DamageType.Crushing,
			handedness: AttackHandednessOptions.TwoHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Hinder);
		AddAttack("Training Longsword 2-Handed Jab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Jab,
			Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.High, 5.0, 0.7, traininglongsword, trainingDamage, "@ quickly jab|jabs $2 at $1",
			DamageType.Crushing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast,
			handedness: AttackHandednessOptions.TwoHandedOnly);
		AddAttack("Training Longsword 1-Handed Counter Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
			Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.High, 5.0, 0.8, traininglongsword, trainingDamage, "@ quickly counter jab|jabs $2 at $1",
			DamageType.Crushing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast,
			handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Training Longsword 1-Handed Low Counter Jab", BuiltInCombatMoveType.WardFreeAttack,
			MeleeWeaponVerb.Jab, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy,
			Alignment.FrontRight, Orientation.Centre, 5.0, 0.8, traininglongsword, trainingDamage,
			"@ quickly counter jab|jabs $2 at $1", DamageType.Crushing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast,
			handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Training Longsword 2-Handed Counter Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
			Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.High, 5.0, 0.7, traininglongsword, trainingDamage, "@ quickly counter jab|jabs $2 at $1",
			DamageType.Crushing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast,
			handedness: AttackHandednessOptions.TwoHandedOnly);
		AddAttack("Training Longsword 2-Handed Low Counter Jab", BuiltInCombatMoveType.WardFreeAttack,
			MeleeWeaponVerb.Jab, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy,
			Alignment.FrontRight, Orientation.Centre, 5.0, 0.7, traininglongsword, trainingDamage,
			"@ quickly counter jab|jabs $2 at $1", DamageType.Crushing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast,
			handedness: AttackHandednessOptions.TwoHandedOnly);

		AddAttack("Zweihander 1-Handed Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.High, 5.0, 1.2, zweihander, goodDamage, "@ swing|swings $2 at $1", DamageType.Slashing,
			handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Zweihander 1-Handed High Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
			Orientation.Highest, 5.0, 1.3, zweihander, goodDamage, "@ swing|swings $2 in a high blow at $1",
			DamageType.Slashing, handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Zweihander 1-Handed Low Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Centre, 5.0, 1.2, zweihander, goodDamage, "@ swing|swings $2 in a low blow at $1",
			DamageType.Slashing, handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Zweihander 1-Handed Leg Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Low, 5.0, 1.3, zweihander, goodDamage, "@ swing|swings $2 at $1's legs", DamageType.Slashing,
			handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Zweihander 1-Handed Arm Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.VeryEasy, Alignment.FrontRight,
			Orientation.Appendage, 5.0, 1.3, zweihander, goodDamage, "@ swing|swings $2 at $1's arms",
			DamageType.Slashing, handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Zweihander 1-Handed Overhead Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Hard, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.Front, Orientation.Highest,
			6.0, 1.5, zweihander, greatDamage, "@ swing|swings $2 in an overhead blow at $1", DamageType.Slashing,
			handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Zweihander 1-Handed Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.High, 5.0, 1.2, zweihander, goodDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
			DamageType.Piercing, handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Zweihander 1-Handed High Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Highest, 5.0, 1.2, zweihander, goodDamage,
			"@ lunge|lunges forward and stab|stabs $2 high at $1", DamageType.Piercing,
			handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Zweihander 1-Handed Low Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
			Orientation.Low, 5.0, 1.2, zweihander, goodDamage, "@ lunge|lunges forward and stab|stabs $2 low at $1",
			DamageType.Piercing, handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Zweihander 1-Handed Foot Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.VeryHard, Alignment.FrontRight,
			Orientation.Lowest, 5.0, 1.2, zweihander, goodDamage,
			"@ lunge|lunges forward and stab|stabs $2 at $1's feet", DamageType.Piercing,
			handedness: AttackHandednessOptions.OneHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Kill | CombatMoveIntentions.Wound |
			            CombatMoveIntentions.Hinder);
		AddAttack("Zweihander 1-Handed Jab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Jab,
			Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.High, 5.0, 1.0, zweihander, badDamage, "@ quickly jab|jabs $2 at $1", DamageType.Piercing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Kill | CombatMoveIntentions.Wound |
			            CombatMoveIntentions.Fast, handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Zweihander 1-Handed Counter Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
			Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.High, 5.0, 1.0, zweihander, badDamage, "@ quickly counter jab|jabs $2 at $1",
			DamageType.Piercing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Kill | CombatMoveIntentions.Wound |
			            CombatMoveIntentions.Fast, handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Zweihander 1-Handed Low Counter Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
			Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Centre, 5.0, 1.0, zweihander, badDamage, "@ quickly counter jab|jabs $2 at $1",
			DamageType.Piercing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Kill | CombatMoveIntentions.Wound |
			            CombatMoveIntentions.Fast, handedness: AttackHandednessOptions.OneHandedOnly);

		AddAttack("Zweihander Smash", BuiltInCombatMoveType.MeleeWeaponSmashItem, MeleeWeaponVerb.Swing,
			Difficulty.Easy, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
			Orientation.Highest, 6.0, 1.5, zweihander, greatDamage, "@ swing|swings $2 at $1", DamageType.Slashing);
		AddAttack("Zweihander Slash Head", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
			Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5,
			zweihander, coupdegraceDamage,
			"@ hold|holds $2 up above &0's head and bring|brings it down at $1's {0} in a devastating swing",
			DamageType.Slashing, additionalInfo: context.BodypartProtos.First(x => x.Name == "scalp").Id.ToString());
		AddAttack("Zweihander Slash Neck", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
			Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5,
			zweihander, coupdegraceDamage,
			"@ hold|holds $2 up above &0's head and bring|brings it down at $1's {0} in a devastating swing",
			DamageType.Slashing, additionalInfo: context.BodypartProtos.First(x => x.Name == "bneck").Id.ToString());
		AddAttack("Zweihander Slash Right Hand", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing,
			Difficulty.Hard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
			Orientation.Highest, 8.0, 1.5, zweihander, coupdegraceDamage,
			"@ hold|holds $2 up above &0's head and bring|brings it down at $1's {0} in a devastating swing",
			DamageType.Slashing, additionalInfo: context.BodypartProtos.First(x => x.Name == "rhand").Id.ToString());
		AddAttack("Zweihander Slash Left Hand", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing,
			Difficulty.Hard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
			Orientation.Highest, 8.0, 1.5, zweihander, coupdegraceDamage,
			"@ hold|holds $2 up above &0's head and bring|brings it down at $1's {0} in a devastating swing",
			DamageType.Slashing, additionalInfo: context.BodypartProtos.First(x => x.Name == "lhand").Id.ToString());
		AddAttack("Zweihander Slash Right Leg", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing,
			Difficulty.Hard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
			Orientation.Highest, 8.0, 1.5, zweihander, coupdegraceDamage,
			"@ hold|holds $2 up above &0's head and bring|brings it down at $1's {0} in a devastating swing",
			DamageType.Slashing, additionalInfo: context.BodypartProtos.First(x => x.Name == "rshin").Id.ToString());
		AddAttack("Zweihander Slash Left Leg", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing,
			Difficulty.Hard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
			Orientation.Highest, 8.0, 1.5, zweihander, coupdegraceDamage,
			"@ hold|holds $2 up above &0's head and bring|brings it down at $1's {0} in a devastating swing",
			DamageType.Slashing, additionalInfo: context.BodypartProtos.First(x => x.Name == "lshin").Id.ToString());
		AddAttack("Zweihander Chest Stab", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
			Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5,
			zweihander, coupdegraceDamage,
			"@ position|positions $2 above $1's heart and thrust|thrusts down in a brutal stab", DamageType.Piercing,
			additionalInfo: context.BodypartProtos.First(x => x.Name == "rbreast").Id.ToString());
		AddAttack("Zweihander Throat Stab", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
			Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5,
			zweihander, coupdegraceDamage,
			"@ position|positions $2 above $1's throat and thrust|thrusts down in a brutal stab", DamageType.Piercing,
			additionalInfo: context.BodypartProtos.First(x => x.Name == "throat").Id.ToString());

		AddAttack("Zweihander 2-Handed Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.High, 5.0, 1.1, zweihander, goodDamage, "@ swing|swings $2 at $1", DamageType.Slashing,
			handedness: AttackHandednessOptions.TwoHandedOnly);
		AddAttack("Zweihander 2-Handed High Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
			Orientation.Highest, 5.0, 1.2, zweihander, goodDamage, "@ swing|swings $2 in a high blow at $1",
			DamageType.Slashing, handedness: AttackHandednessOptions.TwoHandedOnly);
		AddAttack("Zweihander 2-Handed Low Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Centre, 5.0, 1.1, zweihander, goodDamage, "@ swing|swings $2 in a low blow at $1",
			DamageType.Slashing, handedness: AttackHandednessOptions.TwoHandedOnly);
		AddAttack("Zweihander 2-Handed Leg Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Low, 5.0, 1.2, zweihander, goodDamage, "@ swing|swings $2 at $1's legs", DamageType.Slashing,
			handedness: AttackHandednessOptions.TwoHandedOnly);
		AddAttack("Zweihander 2-Handed Arm Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.VeryEasy, Alignment.FrontRight,
			Orientation.Appendage, 5.0, 1.2, zweihander, goodDamage, "@ swing|swings $2 at $1's arms",
			DamageType.Slashing, handedness: AttackHandednessOptions.TwoHandedOnly);
		AddAttack("Zweihander 2-Handed Overhead Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Hard, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.Front, Orientation.Highest,
			6.0, 1.2, zweihander, greatDamage, "@ swing|swings $2 in an overhead blow at $1", DamageType.Slashing,
			handedness: AttackHandednessOptions.TwoHandedOnly);
		AddAttack("Zweihander 2-Handed Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.High, 5.0, 1.1, zweihander, goodDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
			DamageType.Piercing, handedness: AttackHandednessOptions.TwoHandedOnly);
		AddAttack("Zweihander 2-Handed High Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Highest, 5.0, 1.1, zweihander, goodDamage,
			"@ lunge|lunges forward and stab|stabs $2 high at $1", DamageType.Piercing,
			handedness: AttackHandednessOptions.TwoHandedOnly);
		AddAttack("Zweihander 2-Handed Low Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
			Orientation.Low, 5.0, 1.1, zweihander, goodDamage, "@ lunge|lunges forward and stab|stabs $2 low at $1",
			DamageType.Piercing, handedness: AttackHandednessOptions.TwoHandedOnly);
		AddAttack("Zweihander 2-Handed Foot Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.VeryHard, Alignment.FrontRight,
			Orientation.Lowest, 5.0, 1.1, zweihander, goodDamage,
			"@ lunge|lunges forward and stab|stabs $2 at $1's feet", DamageType.Piercing,
			handedness: AttackHandednessOptions.TwoHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Kill | CombatMoveIntentions.Wound |
			            CombatMoveIntentions.Hinder);
		AddAttack("Zweihander 2-Handed Jab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Jab,
			Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.High, 5.0, 0.9, zweihander, poorDamage, "@ quickly jab|jabs $2 at $1", DamageType.Piercing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Kill | CombatMoveIntentions.Wound |
			            CombatMoveIntentions.Fast, handedness: AttackHandednessOptions.TwoHandedOnly);
		AddAttack("Zweihander 2-Handed Counter Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
			Difficulty.Hard, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.High, 5.0, 0.9, zweihander, poorDamage, "@ quickly counter jab|jabs $2 at $1",
			DamageType.Piercing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Kill | CombatMoveIntentions.Wound |
			            CombatMoveIntentions.Fast, handedness: AttackHandednessOptions.TwoHandedOnly);
		AddAttack("Zweihander 2-Handed Low Counter Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
			Difficulty.Hard, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Centre, 5.0, 0.9, zweihander, poorDamage, "@ quickly counter jab|jabs $2 at $1",
			DamageType.Piercing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Kill | CombatMoveIntentions.Wound |
			            CombatMoveIntentions.Fast, handedness: AttackHandednessOptions.TwoHandedOnly);

		#endregion

		#region Rapier

		var rapier = new WeaponType
		{
			Name = "Rapier",
			Classification = (int)WeaponClassification.Lethal,
			AttackTrait = skills["Rapier"],
			ParryTrait = parrySkills["Rapier"],
			ParryBonus = 2,
			Reach = 3,
			StaminaPerParry = 1.0
		};
		context.WeaponTypes.Add(rapier);
		context.SaveChanges();
		CreateWeaponComponent(rapier);
		var trainingrapier = new WeaponType
		{
			Name = "Training Longsword",
			Classification = (int)WeaponClassification.Training,
			AttackTrait = skills["Rapier"],
			ParryTrait = parrySkills["Rapier"],
			ParryBonus = 2,
			Reach = 3,
			StaminaPerParry = 1.0
		};
		context.WeaponTypes.Add(trainingrapier);
		context.SaveChanges();
		CreateWeaponComponent(trainingrapier);

		AddAttack("Rapier 1-Handed Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Normal, Difficulty.Hard, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.High, 5.0, 0.9, rapier, normalDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
			DamageType.Piercing, handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Rapier 1-Handed High Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Normal, Difficulty.Hard, Difficulty.Easy, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Highest, 5.0, 0.9, rapier, normalDamage, "@ lunge|lunges forward and stab|stabs $2 high at $1",
			DamageType.Piercing, handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Rapier 1-Handed Low Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
			Orientation.Low, 5.0, 0.9, rapier, normalDamage, "@ lunge|lunges forward and stab|stabs $2 low at $1",
			DamageType.Piercing, handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Rapier 1-Handed Foot Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.VeryHard, Alignment.FrontRight,
			Orientation.Lowest, 5.0, 0.9, rapier, normalDamage, "@ lunge|lunges forward and stab|stabs $2 at $1's feet",
			DamageType.Piercing, handedness: AttackHandednessOptions.OneHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Kill | CombatMoveIntentions.Wound |
			            CombatMoveIntentions.Hinder);
		AddAttack("Rapier 1-Handed Cross Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.Front, Orientation.High,
			5.0, 0.9, rapier, normalDamage, "@ lunge|lunges forward and stab|stabs $2 across the body at $1",
			DamageType.Piercing, handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Rapier 1-Handed Jab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Jab, Difficulty.Easy,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight, Orientation.High, 5.0, 0.7,
			rapier, poorDamage, "@ quickly jab|jabs $2 at $1", DamageType.Piercing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Kill | CombatMoveIntentions.Wound |
			            CombatMoveIntentions.Fast, handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Rapier 1-Handed Counter Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
			Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.High, 5.0, 0.7, rapier, poorDamage, "@ quickly counter jab|jabs $2 at $1", DamageType.Piercing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Kill | CombatMoveIntentions.Wound |
			            CombatMoveIntentions.Fast, handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Rapier 1-Handed Low Counter Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
			Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Centre, 5.0, 0.7, rapier, poorDamage, "@ quickly counter jab|jabs $2 at $1",
			DamageType.Piercing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Kill | CombatMoveIntentions.Wound |
			            CombatMoveIntentions.Fast, handedness: AttackHandednessOptions.OneHandedOnly);

		AddAttack("Training Rapier 1-Handed Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.VeryEasy, Difficulty.Hard, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.High, 5.0, 0.9, trainingrapier, trainingDamage,
			"@ lunge|lunges forward and stab|stabs $2 at $1", DamageType.Crushing,
			handedness: AttackHandednessOptions.OneHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Rapier 1-Handed High Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.VeryEasy, Difficulty.Hard, Difficulty.Easy, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Highest, 5.0, 0.9, trainingrapier, trainingDamage,
			"@ lunge|lunges forward and stab|stabs $2 high at $1", DamageType.Crushing,
			handedness: AttackHandednessOptions.OneHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Rapier 1-Handed Low Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
			Orientation.Low, 5.0, 0.9, trainingrapier, trainingDamage,
			"@ lunge|lunges forward and stab|stabs $2 low at $1", DamageType.Crushing,
			handedness: AttackHandednessOptions.OneHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Rapier 1-Handed Foot Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.VeryEasy, Difficulty.Easy, Difficulty.Easy, Difficulty.VeryHard, Alignment.FrontRight,
			Orientation.Lowest, 5.0, 0.9, trainingrapier, trainingDamage,
			"@ lunge|lunges forward and stab|stabs $2 at $1's feet", DamageType.Crushing,
			handedness: AttackHandednessOptions.OneHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Hinder);
		AddAttack("Training Rapier 1-Handed Cross Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.Front,
			Orientation.High, 5.0, 0.9, trainingrapier, trainingDamage,
			"@ lunge|lunges forward and stab|stabs $2 across the body at $1", DamageType.Crushing,
			handedness: AttackHandednessOptions.OneHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Rapier 1-Handed Jab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Jab,
			Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.High, 5.0, 0.7, trainingrapier, trainingDamage, "@ quickly jab|jabs $2 at $1",
			DamageType.Crushing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast,
			handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Training Rapier 1-Handed Counter Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
			Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.High, 5.0, 0.7, trainingrapier, trainingDamage, "@ quickly counter jab|jabs $2 at $1",
			DamageType.Crushing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast,
			handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Training Rapier 1-Handed Low Counter Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
			Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Centre, 5.0, 0.7, trainingrapier, trainingDamage, "@ quickly counter jab|jabs $2 at $1",
			DamageType.Crushing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast,
			handedness: AttackHandednessOptions.OneHandedOnly);

		#endregion

		#endregion

		#region Axes

		var axe = new WeaponType
		{
			Name = "Axe",
			Classification = (int)WeaponClassification.Lethal,
			AttackTrait = skills["Axe"],
			ParryTrait = parrySkills["Axe"],
			ParryBonus = -2,
			Reach = 3,
			StaminaPerParry = 3.5
		};
		context.WeaponTypes.Add(axe);
		context.SaveChanges();
		CreateWeaponComponent(axe);
		var trainingaxe = new WeaponType
		{
			Name = "Training Axe",
			Classification = (int)WeaponClassification.Training,
			AttackTrait = skills["Axe"],
			ParryTrait = parrySkills["Axe"],
			ParryBonus = -2,
			Reach = 3,
			StaminaPerParry = 3.5
		};
		context.WeaponTypes.Add(trainingaxe);
		context.SaveChanges();
		CreateWeaponComponent(trainingaxe);

		AddAttack("Axe 1-Handed Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing, Difficulty.Normal,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight, Orientation.High, 5.0, 1.0,
			axe, goodDamage, "@ swing|swings $2 at $1", DamageType.Chopping,
			handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Axe 1-Handed High Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
			Orientation.Highest, 5.0, 1.1, axe, goodDamage, "@ swing|swings $2 in a high blow at $1",
			DamageType.Chopping, handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Axe 1-Handed Low Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Centre, 5.0, 1.0, axe, goodDamage, "@ swing|swings $2 in a low blow at $1", DamageType.Chopping,
			handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Axe 1-Handed Leg Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Low, 5.0, 1.1, axe, goodDamage, "@ swing|swings $2 at $1's legs", DamageType.Chopping,
			handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Axe 1-Handed Arm Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.VeryEasy, Alignment.FrontRight,
			Orientation.Appendage, 5.0, 1.1, axe, goodDamage, "@ swing|swings $2 at $1's arms", DamageType.Chopping,
			handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Axe 1-Handed Overhead Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Hard, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.Front, Orientation.Highest,
			6.0, 1.3, axe, veryGoodDamage, "@ swing|swings $2 in an overhead blow at $1", DamageType.Chopping,
			handedness: AttackHandednessOptions.OneHandedOnly);

		AddAttack("Axe Smash", BuiltInCombatMoveType.MeleeWeaponSmashItem, MeleeWeaponVerb.Swing, Difficulty.Easy,
			Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 6.0, 1.3, axe,
			goodDamage, "@ swing|swings $2 at $1", DamageType.Chopping);
		AddAttack("Axe Chop Head", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
			Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5, axe,
			coupdegraceDamage,
			"@ hold|holds $2 up above &0's head and bring|brings it down at $1's {0} in a devastating swing",
			DamageType.Chopping, additionalInfo: context.BodypartProtos.First(x => x.Name == "scalp").Id.ToString());
		AddAttack("Axe Chop Neck", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
			Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5, axe,
			coupdegraceDamage,
			"@ hold|holds $2 up above &0's head and bring|brings it down at $1's {0} in a devastating swing",
			DamageType.Chopping, additionalInfo: context.BodypartProtos.First(x => x.Name == "bneck").Id.ToString());
		AddAttack("Axe Chop Right Hand", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
			Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5, axe,
			coupdegraceDamage,
			"@ hold|holds $2 up above &0's head and bring|brings it down at $1's {0} in a devastating swing",
			DamageType.Chopping, additionalInfo: context.BodypartProtos.First(x => x.Name == "rhand").Id.ToString());
		AddAttack("Axe Chop Left Hand", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
			Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5, axe,
			coupdegraceDamage,
			"@ hold|holds $2 up above &0's head and bring|brings it down at $1's {0} in a devastating swing",
			DamageType.Chopping, additionalInfo: context.BodypartProtos.First(x => x.Name == "lhand").Id.ToString());
		AddAttack("Axe Chop Right Leg", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
			Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5, axe,
			coupdegraceDamage,
			"@ hold|holds $2 up above &0's head and bring|brings it down at $1's {0} in a devastating swing",
			DamageType.Chopping, additionalInfo: context.BodypartProtos.First(x => x.Name == "rshin").Id.ToString());
		AddAttack("Axe Chop Left Leg", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
			Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5, axe,
			coupdegraceDamage,
			"@ hold|holds $2 up above &0's head and bring|brings it down at $1's {0} in a devastating swing",
			DamageType.Chopping, additionalInfo: context.BodypartProtos.First(x => x.Name == "lshin").Id.ToString());

		AddAttack("Axe 2-Handed Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing, Difficulty.Normal,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight, Orientation.High, 5.0, 0.9,
			axe, goodDamage, "@ swing|swings $2 at $1", DamageType.Chopping,
			handedness: AttackHandednessOptions.TwoHandedOnly);
		AddAttack("Axe 2-Handed High Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
			Orientation.Highest, 5.0, 1.0, axe, goodDamage, "@ swing|swings $2 in a high blow at $1",
			DamageType.Chopping, handedness: AttackHandednessOptions.TwoHandedOnly);
		AddAttack("Axe 2-Handed Low Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Centre, 5.0, 0.9, axe, goodDamage, "@ swing|swings $2 in a low blow at $1", DamageType.Chopping,
			handedness: AttackHandednessOptions.TwoHandedOnly);
		AddAttack("Axe 2-Handed Leg Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Low, 5.0, 1.0, axe, goodDamage, "@ swing|swings $2 at $1's legs", DamageType.Chopping,
			handedness: AttackHandednessOptions.TwoHandedOnly);
		AddAttack("Axe 2-Handed Arm Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.VeryEasy, Alignment.FrontRight,
			Orientation.Appendage, 5.0, 1.0, axe, goodDamage, "@ swing|swings $2 at $1's arms", DamageType.Chopping,
			handedness: AttackHandednessOptions.TwoHandedOnly);
		AddAttack("Axe 2-Handed Overhead Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Hard, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.Front, Orientation.Highest,
			6.0, 1.2, axe, veryGoodDamage, "@ swing|swings $2 in an overhead blow at $1", DamageType.Chopping,
			handedness: AttackHandednessOptions.TwoHandedOnly);

		AddAttack("Training Axe 1-Handed Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.High, 5.0, 1.0, trainingaxe, trainingDamage, "@ swing|swings $2 at $1", DamageType.Crushing,
			handedness: AttackHandednessOptions.OneHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Axe 1-Handed High Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
			Orientation.Highest, 5.0, 1.1, trainingaxe, trainingDamage, "@ swing|swings $2 in a high blow at $1",
			DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Axe 1-Handed Low Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Centre, 5.0, 1.0, trainingaxe, trainingDamage, "@ swing|swings $2 in a low blow at $1",
			DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Axe 1-Handed Leg Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Low, 5.0, 1.1, trainingaxe, trainingDamage, "@ swing|swings $2 at $1's legs",
			DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Axe 1-Handed Arm Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.VeryEasy, Alignment.FrontRight,
			Orientation.Appendage, 5.0, 1.1, trainingaxe, trainingDamage, "@ swing|swings $2 at $1's arms",
			DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Axe 1-Handed Overhead Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Hard, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.Front, Orientation.Highest,
			6.0, 1.3, trainingaxe, trainingDamage, "@ swing|swings $2 in an overhead blow at $1", DamageType.Crushing,
			handedness: AttackHandednessOptions.OneHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Axe 2-Handed Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.High, 5.0, 0.9, trainingaxe, trainingDamage, "@ swing|swings $2 at $1", DamageType.Crushing,
			handedness: AttackHandednessOptions.TwoHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Axe 2-Handed High Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
			Orientation.Highest, 5.0, 1.0, trainingaxe, trainingDamage, "@ swing|swings $2 in a high blow at $1",
			DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Axe 2-Handed Low Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Centre, 5.0, 0.9, trainingaxe, trainingDamage, "@ swing|swings $2 in a low blow at $1",
			DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Axe 2-Handed Leg Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Low, 5.0, 1.0, trainingaxe, trainingDamage, "@ swing|swings $2 at $1's legs",
			DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Axe 2-Handed Arm Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.VeryEasy, Alignment.FrontRight,
			Orientation.Appendage, 5.0, 1.0, trainingaxe, trainingDamage, "@ swing|swings $2 at $1's arms",
			DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Axe 2-Handed Overhead Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
			Difficulty.Hard, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.Front, Orientation.Highest,
			6.0, 1.2, trainingaxe, trainingDamage, "@ swing|swings $2 in an overhead blow at $1", DamageType.Crushing,
			handedness: AttackHandednessOptions.TwoHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);

		#endregion

		#region Spears

		var spear = new WeaponType
		{
			Name = "Short Spear",
			Classification = (int)WeaponClassification.Lethal,
			AttackTrait = skills["Spear"],
			ParryTrait = parrySkills["Spear"],
			ParryBonus = 1,
			Reach = 4,
			StaminaPerParry = 1.5
		};
		context.WeaponTypes.Add(spear);
		context.SaveChanges();
		CreateWeaponComponent(spear);
		var longspear = new WeaponType
		{
			Name = "Long Spear",
			Classification = (int)WeaponClassification.Lethal,
			AttackTrait = skills["Spear"],
			ParryTrait = parrySkills["Spear"],
			ParryBonus = 1,
			Reach = 5,
			StaminaPerParry = 2.0
		};
		context.WeaponTypes.Add(longspear);
		context.SaveChanges();
		CreateWeaponComponent(longspear);
		var trainingspear = new WeaponType
		{
			Name = "Training Spear",
			Classification = (int)WeaponClassification.Training,
			AttackTrait = skills["Spear"],
			ParryTrait = parrySkills["Spear"],
			ParryBonus = 1,
			Reach = 4,
			StaminaPerParry = 1.5
		};
		context.WeaponTypes.Add(trainingspear);
		context.SaveChanges();
		CreateWeaponComponent(trainingspear);

		AddAttack("Spear 1-Handed Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab, Difficulty.Normal,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight, Orientation.High, 5.0, 1.0,
			spear, normalDamage, "@ lunge|lunges forward and stab|stabs $2 at $1", DamageType.Piercing,
			handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Spear 1-Handed High Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
			Orientation.Highest, 5.0, 1.0, spear, normalDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
			DamageType.Piercing, handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Spear 1-Handed Low Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
			Orientation.Centre, 5.0, 1.0, spear, normalDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
			DamageType.Piercing, handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Spear 1-Handed Leg Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.Hard, Alignment.FrontRight, Orientation.Low,
			5.0, 1.0, spear, normalDamage, "@ lunge|lunges forward and stab|stabs $2 at $1's leg", DamageType.Piercing,
			handedness: AttackHandednessOptions.OneHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
			            CombatMoveIntentions.Hinder);
		AddAttack("Spear 1-Handed Foot Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Normal, Difficulty.Easy, Difficulty.VeryEasy, Difficulty.VeryHard, Alignment.FrontRight,
			Orientation.Lowest, 5.0, 1.0, spear, normalDamage, "@ lunge|lunges forward and stab|stabs $2 at $1's foot",
			DamageType.Piercing, handedness: AttackHandednessOptions.OneHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
			            CombatMoveIntentions.Hinder);
		AddAttack("Spear 1-Handed Trip", BuiltInCombatMoveType.UnbalancingBlow, MeleeWeaponVerb.Sweep, Difficulty.Hard,
			Difficulty.Easy, Difficulty.Easy, Difficulty.VeryHard, Alignment.FrontRight, Orientation.Lowest, 3.0, 0.6,
			spear, terribleDamage, "@ knock|knocks at $1's legs and feet with $2 in an attempt to trip &1 up",
			DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Trip |
			            CombatMoveIntentions.Hinder, additionalInfo: ((int)Difficulty.Normal).ToString());
		AddAttack("Spear 1-Handed Counter Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
			Difficulty.VeryHard, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.High, 5.0, 1.0, spear, poorDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
			DamageType.Piercing, handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Spear 1-Handed Low Counter Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
			Difficulty.VeryHard, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Centre, 5.0, 1.0, spear, poorDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
			DamageType.Piercing, handedness: AttackHandednessOptions.OneHandedOnly);

		AddAttack("Spear 2-Handed Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab, Difficulty.Normal,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight, Orientation.High, 5.0, 1.0,
			spear, goodDamage, "@ lunge|lunges forward and stab|stabs $2 at $1", DamageType.Piercing,
			handedness: AttackHandednessOptions.TwoHandedOnly);
		AddAttack("Spear 2-Handed High Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
			Orientation.Highest, 5.0, 1.0, spear, goodDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
			DamageType.Piercing, handedness: AttackHandednessOptions.TwoHandedOnly);
		AddAttack("Spear 2-Handed Low Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
			Orientation.Centre, 5.0, 1.0, spear, goodDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
			DamageType.Piercing, handedness: AttackHandednessOptions.TwoHandedOnly);
		AddAttack("Spear 2-Handed Leg Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.Hard, Alignment.FrontRight, Orientation.Low,
			5.0, 1.0, spear, goodDamage, "@ lunge|lunges forward and stab|stabs $2 at $1's leg", DamageType.Piercing,
			handedness: AttackHandednessOptions.TwoHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
			            CombatMoveIntentions.Hinder);
		AddAttack("Spear 2-Handed Foot Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Normal, Difficulty.Easy, Difficulty.VeryEasy, Difficulty.VeryHard, Alignment.FrontRight,
			Orientation.Lowest, 5.0, 1.0, spear, goodDamage, "@ lunge|lunges forward and stab|stabs $2 at $1's foot",
			DamageType.Piercing, handedness: AttackHandednessOptions.TwoHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
			            CombatMoveIntentions.Hinder);
		AddAttack("Spear 2-Handed Leg Sweep", BuiltInCombatMoveType.UnbalancingBlow, MeleeWeaponVerb.Sweep,
			Difficulty.Hard, Difficulty.Easy, Difficulty.Easy, Difficulty.Hard, Alignment.FrontRight, Orientation.Low,
			5.0, 1.0, spear, badDamage, "@ sweep|sweeps $2 around at $1's legs in an attempt to knock &1 off balance",
			DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Trip |
			            CombatMoveIntentions.Hinder, additionalInfo: ((int)Difficulty.Normal).ToString());
		AddAttack("Spear 2-Handed Trip", BuiltInCombatMoveType.UnbalancingBlow, MeleeWeaponVerb.Sweep, Difficulty.Hard,
			Difficulty.Easy, Difficulty.Easy, Difficulty.VeryHard, Alignment.FrontRight, Orientation.Lowest, 3.0, 0.6,
			spear, terribleDamage, "@ knock|knocks at $1's legs and feet with $2 in an attempt to trip &1 up",
			DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Trip |
			            CombatMoveIntentions.Hinder, additionalInfo: ((int)Difficulty.Hard).ToString());
		AddAttack("Spear 2-Handed Counter Stab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Stab,
			Difficulty.Hard, Difficulty.VeryHard, Difficulty.Normal, Difficulty.Easy, Alignment.Front, Orientation.High,
			5.0, 1.0, spear, goodDamage,
			"@ duck|ducks inside the blow and lunge|lunges forward with a devastating stab of $2 at $1",
			DamageType.Piercing, handedness: AttackHandednessOptions.TwoHandedOnly);
		AddAttack("Spear 2-Handed Low Counter Stab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Stab,
			Difficulty.Hard, Difficulty.VeryHard, Difficulty.Normal, Difficulty.Easy, Alignment.Front,
			Orientation.Centre, 5.0, 1.0, spear, goodDamage,
			"@ duck|ducks inside the blow and lunge|lunges forward with a devastating low stab of $2 at $1",
			DamageType.Piercing, handedness: AttackHandednessOptions.TwoHandedOnly);
		AddAttack("Spear 2-Handed High Counter Stab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Stab,
			Difficulty.VeryHard, Difficulty.ExtremelyHard, Difficulty.Hard, Difficulty.Hard, Alignment.Front,
			Orientation.Highest, 5.0, 1.0, spear, goodDamage,
			"@ duck|ducks inside the blow and lunge|lunges forward with a devastating high stab of $2 at $1",
			DamageType.Piercing, handedness: AttackHandednessOptions.TwoHandedOnly);

		AddAttack("Spear Smash", BuiltInCombatMoveType.MeleeWeaponSmashItem, MeleeWeaponVerb.Stab, Difficulty.Normal,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight, Orientation.High, 5.0, 1.0,
			spear, normalDamage, "@ stab|stabs $2 at $1", DamageType.Piercing);
		AddAttack("Spear Throat Stab", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Stab, Difficulty.Normal,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight, Orientation.High, 5.0, 1.0,
			spear, greatDamage, "@ brutally stab|stabs $2 into $1's throat", DamageType.Piercing,
			additionalInfo: context.BodypartProtos.First(x => x.Name == "throat").Id.ToString());
		AddAttack("Spear Heart Stab", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Stab, Difficulty.Normal,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight, Orientation.High, 5.0, 1.0,
			spear, greatDamage, "@ brutally stab|stabs $2 into $1's chest right above &1's heart", DamageType.Piercing,
			additionalInfo: context.BodypartProtos.First(x => x.Name == "rbreast").Id.ToString());
		AddAttack("Spear Belly Stab", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Stab, Difficulty.Normal,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight, Orientation.High, 5.0, 1.0,
			spear, greatDamage, "@ brutally stab|stabs $2 into $1's belly", DamageType.Piercing,
			additionalInfo: context.BodypartProtos.First(x => x.Name == "belly").Id.ToString());
		AddAttack("Spear Neck Stab", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Stab, Difficulty.Normal,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight, Orientation.High, 5.0, 1.0,
			spear, greatDamage, "@ line|lines up $2 with the back of $1's neck and brutally push|pushes the spear in",
			DamageType.Piercing, additionalInfo: context.BodypartProtos.First(x => x.Name == "bneck").Id.ToString());

		AddAttack("Spear 1-Handed Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab, Difficulty.Normal,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight, Orientation.High, 5.0, 1.0,
			longspear, normalDamage, "@ lunge|lunges forward and stab|stabs $2 at $1", DamageType.Piercing,
			handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Spear 1-Handed High Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
			Orientation.Highest, 5.0, 1.0, longspear, normalDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
			DamageType.Piercing, handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Spear 1-Handed Low Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
			Orientation.Centre, 5.0, 1.0, longspear, normalDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
			DamageType.Piercing, handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Spear 1-Handed Leg Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.Hard, Alignment.FrontRight, Orientation.Low,
			5.0, 1.0, longspear, normalDamage, "@ lunge|lunges forward and stab|stabs $2 at $1's leg",
			DamageType.Piercing, handedness: AttackHandednessOptions.OneHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
			            CombatMoveIntentions.Hinder);
		AddAttack("Spear 1-Handed Foot Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Normal, Difficulty.Easy, Difficulty.VeryEasy, Difficulty.VeryHard, Alignment.FrontRight,
			Orientation.Lowest, 5.0, 1.0, longspear, normalDamage,
			"@ lunge|lunges forward and stab|stabs $2 at $1's foot", DamageType.Piercing,
			handedness: AttackHandednessOptions.OneHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
			            CombatMoveIntentions.Hinder);
		AddAttack("Spear 1-Handed Counter Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
			Difficulty.VeryHard, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.High, 5.0, 1.0, longspear, poorDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
			DamageType.Piercing, handedness: AttackHandednessOptions.OneHandedOnly);
		AddAttack("Spear 1-Handed Low Counter Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
			Difficulty.VeryHard, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Centre, 5.0, 1.0, longspear, poorDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
			DamageType.Piercing, handedness: AttackHandednessOptions.OneHandedOnly);

		AddAttack("Spear 2-Handed Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab, Difficulty.Normal,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight, Orientation.High, 5.0, 1.0,
			longspear, goodDamage, "@ lunge|lunges forward and stab|stabs $2 at $1", DamageType.Piercing,
			handedness: AttackHandednessOptions.TwoHandedOnly);
		AddAttack("Spear 2-Handed High Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
			Orientation.Highest, 5.0, 1.0, longspear, goodDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
			DamageType.Piercing, handedness: AttackHandednessOptions.TwoHandedOnly);
		AddAttack("Spear 2-Handed Low Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
			Orientation.Centre, 5.0, 1.0, longspear, goodDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
			DamageType.Piercing, handedness: AttackHandednessOptions.TwoHandedOnly);
		AddAttack("Spear 2-Handed Leg Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.Hard, Alignment.FrontRight, Orientation.Low,
			5.0, 1.0, longspear, goodDamage, "@ lunge|lunges forward and stab|stabs $2 at $1's leg",
			DamageType.Piercing, handedness: AttackHandednessOptions.TwoHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
			            CombatMoveIntentions.Hinder);
		AddAttack("Spear 2-Handed Foot Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.Normal, Difficulty.Easy, Difficulty.VeryEasy, Difficulty.VeryHard, Alignment.FrontRight,
			Orientation.Lowest, 5.0, 1.0, longspear, goodDamage,
			"@ lunge|lunges forward and stab|stabs $2 at $1's foot", DamageType.Piercing,
			handedness: AttackHandednessOptions.TwoHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
			            CombatMoveIntentions.Hinder);
		AddAttack("Spear 2-Handed Leg Sweep", BuiltInCombatMoveType.UnbalancingBlow, MeleeWeaponVerb.Sweep,
			Difficulty.Hard, Difficulty.Easy, Difficulty.Easy, Difficulty.Hard, Alignment.FrontRight, Orientation.Low,
			5.0, 1.0, longspear, badDamage,
			"@ sweep|sweeps $2 around at $1's legs in an attempt to knock &1 off balance", DamageType.Crushing,
			handedness: AttackHandednessOptions.TwoHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Trip |
			            CombatMoveIntentions.Hinder, additionalInfo: ((int)Difficulty.Normal).ToString());
		AddAttack("Spear 2-Handed Trip", BuiltInCombatMoveType.UnbalancingBlow, MeleeWeaponVerb.Sweep, Difficulty.Hard,
			Difficulty.Easy, Difficulty.Easy, Difficulty.VeryHard, Alignment.FrontRight, Orientation.Lowest, 3.0, 0.6,
			longspear, terribleDamage, "@ knock|knocks at $1's legs and feet with $2 in an attempt to trip &1 up",
			DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Trip |
			            CombatMoveIntentions.Hinder, additionalInfo: ((int)Difficulty.Hard).ToString());
		AddAttack("Spear 2-Handed Counter Stab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Stab,
			Difficulty.Hard, Difficulty.VeryHard, Difficulty.Normal, Difficulty.Easy, Alignment.Front, Orientation.High,
			5.0, 1.0, longspear, goodDamage,
			"@ duck|ducks inside the blow and lunge|lunges forward with a devastating stab of $2 at $1",
			DamageType.Piercing, handedness: AttackHandednessOptions.TwoHandedOnly);
		AddAttack("Spear 2-Handed Low Counter Stab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Stab,
			Difficulty.Hard, Difficulty.VeryHard, Difficulty.Normal, Difficulty.Easy, Alignment.Front,
			Orientation.Centre, 5.0, 1.0, longspear, goodDamage,
			"@ duck|ducks inside the blow and lunge|lunges forward with a devastating low stab of $2 at $1",
			DamageType.Piercing, handedness: AttackHandednessOptions.TwoHandedOnly);
		AddAttack("Spear 2-Handed High Counter Stab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Stab,
			Difficulty.VeryHard, Difficulty.ExtremelyHard, Difficulty.Hard, Difficulty.Hard, Alignment.Front,
			Orientation.Highest, 5.0, 1.0, longspear, goodDamage,
			"@ duck|ducks inside the blow and lunge|lunges forward with a devastating high stab of $2 at $1",
			DamageType.Piercing, handedness: AttackHandednessOptions.TwoHandedOnly);

		AddAttack("Spear Smash", BuiltInCombatMoveType.MeleeWeaponSmashItem, MeleeWeaponVerb.Stab, Difficulty.Normal,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight, Orientation.High, 5.0, 1.0,
			longspear, badDamage, "@ stab|stabs $2 at $1", DamageType.Piercing);
		AddAttack("Spear Throat Stab", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Stab, Difficulty.Normal,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight, Orientation.High, 5.0, 1.0,
			longspear, greatDamage, "@ brutally stab|stabs $2 into $1's throat", DamageType.Piercing,
			additionalInfo: context.BodypartProtos.First(x => x.Name == "throat").Id.ToString());
		AddAttack("Spear Heart Stab", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Stab, Difficulty.Normal,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight, Orientation.High, 5.0, 1.0,
			longspear, greatDamage, "@ brutally stab|stabs $2 into $1's chest right above &1's heart",
			DamageType.Piercing, additionalInfo: context.BodypartProtos.First(x => x.Name == "rbreast").Id.ToString());
		AddAttack("Spear Belly Stab", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Stab, Difficulty.Normal,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight, Orientation.High, 5.0, 1.0,
			longspear, greatDamage, "@ brutally stab|stabs $2 into $1's belly", DamageType.Piercing,
			additionalInfo: context.BodypartProtos.First(x => x.Name == "belly").Id.ToString());
		AddAttack("Spear Neck Stab", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Stab, Difficulty.Normal,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight, Orientation.High, 5.0, 1.0,
			longspear, greatDamage,
			"@ line|lines up $2 with the back of $1's neck and brutally push|pushes the spear in", DamageType.Piercing,
			additionalInfo: context.BodypartProtos.First(x => x.Name == "bneck").Id.ToString());

		AddAttack("Training Spear 1-Handed Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
			Orientation.High, 5.0, 1.0, trainingspear, trainingDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
			DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Spear 1-Handed High Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
			Orientation.Highest, 5.0, 1.0, trainingspear, trainingDamage,
			"@ lunge|lunges forward and stab|stabs $2 at $1", DamageType.Crushing,
			handedness: AttackHandednessOptions.OneHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Spear 1-Handed Low Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
			Orientation.Centre, 5.0, 1.0, trainingspear, trainingDamage,
			"@ lunge|lunges forward and stab|stabs $2 at $1", DamageType.Crushing,
			handedness: AttackHandednessOptions.OneHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Spear 1-Handed Leg Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.VeryEasy, Difficulty.Easy, Difficulty.Easy, Difficulty.Hard, Alignment.FrontRight,
			Orientation.Low, 5.0, 1.0, trainingspear, trainingDamage,
			"@ lunge|lunges forward and stab|stabs $2 at $1's leg", DamageType.Crushing,
			handedness: AttackHandednessOptions.OneHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Spear 1-Handed Foot Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.VeryEasy, Difficulty.Easy, Difficulty.VeryEasy, Difficulty.VeryHard, Alignment.FrontRight,
			Orientation.Lowest, 5.0, 1.0, trainingspear, trainingDamage,
			"@ lunge|lunges forward and stab|stabs $2 at $1's foot", DamageType.Crushing,
			handedness: AttackHandednessOptions.OneHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Spear 1-Handed Trip", BuiltInCombatMoveType.UnbalancingBlow, MeleeWeaponVerb.Sweep,
			Difficulty.VeryEasy, Difficulty.Easy, Difficulty.Easy, Difficulty.VeryHard, Alignment.FrontRight,
			Orientation.Lowest, 3.0, 0.6, trainingspear, trainingDamage,
			"@ knock|knocks at $1's legs and feet with $2 in an attempt to trip &1 up", DamageType.Crushing,
			handedness: AttackHandednessOptions.OneHandedOnly, additionalInfo: ((int)Difficulty.Normal).ToString(),
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Spear 1-Handed Counter Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
			Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.High, 5.0, 1.0, trainingspear, trainingDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
			DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Spear 1-Handed Low Counter Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
			Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Centre, 5.0, 1.0, trainingspear, trainingDamage,
			"@ lunge|lunges forward and stab|stabs $2 at $1", DamageType.Crushing,
			handedness: AttackHandednessOptions.OneHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);

		AddAttack("Training Spear 2-Handed Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
			Orientation.High, 5.0, 1.0, trainingspear, trainingDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
			DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Spear 2-Handed High Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
			Orientation.Highest, 5.0, 1.0, trainingspear, trainingDamage,
			"@ lunge|lunges forward and stab|stabs $2 at $1", DamageType.Crushing,
			handedness: AttackHandednessOptions.TwoHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Spear 2-Handed Low Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
			Orientation.Centre, 5.0, 1.0, trainingspear, trainingDamage,
			"@ lunge|lunges forward and stab|stabs $2 at $1", DamageType.Crushing,
			handedness: AttackHandednessOptions.TwoHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Spear 2-Handed Leg Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.VeryEasy, Difficulty.Easy, Difficulty.Easy, Difficulty.Hard, Alignment.FrontRight,
			Orientation.Low, 5.0, 1.0, trainingspear, trainingDamage,
			"@ lunge|lunges forward and stab|stabs $2 at $1's leg", DamageType.Crushing,
			handedness: AttackHandednessOptions.TwoHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Spear 2-Handed Foot Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
			Difficulty.VeryEasy, Difficulty.Easy, Difficulty.VeryEasy, Difficulty.VeryHard, Alignment.FrontRight,
			Orientation.Lowest, 5.0, 1.0, trainingspear, trainingDamage,
			"@ lunge|lunges forward and stab|stabs $2 at $1's foot", DamageType.Crushing,
			handedness: AttackHandednessOptions.TwoHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Spear 2-Handed Leg Sweep", BuiltInCombatMoveType.UnbalancingBlow, MeleeWeaponVerb.Sweep,
			Difficulty.VeryEasy, Difficulty.Easy, Difficulty.Easy, Difficulty.Hard, Alignment.FrontRight,
			Orientation.Low, 5.0, 1.0, trainingspear, trainingDamage,
			"@ sweep|sweeps $2 around at $1's legs in an attempt to knock &1 off balance", DamageType.Crushing,
			handedness: AttackHandednessOptions.TwoHandedOnly, additionalInfo: ((int)Difficulty.Normal).ToString(),
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Spear 2-Handed Trip", BuiltInCombatMoveType.UnbalancingBlow, MeleeWeaponVerb.Sweep,
			Difficulty.VeryEasy, Difficulty.Easy, Difficulty.Easy, Difficulty.VeryHard, Alignment.FrontRight,
			Orientation.Lowest, 3.0, 0.6, trainingspear, trainingDamage,
			"@ knock|knocks at $1's legs and feet with $2 in an attempt to trip &1 up", DamageType.Crushing,
			handedness: AttackHandednessOptions.TwoHandedOnly, additionalInfo: ((int)Difficulty.Hard).ToString(),
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Spear 2-Handed Counter Stab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Stab,
			Difficulty.Easy, Difficulty.VeryHard, Difficulty.Normal, Difficulty.Easy, Alignment.Front, Orientation.High,
			5.0, 1.0, trainingspear, trainingDamage,
			"@ duck|ducks inside the blow and lunge|lunges forward with a devastating stab of $2 at $1",
			DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Spear 2-Handed Low Counter Stab", BuiltInCombatMoveType.WardFreeAttack,
			MeleeWeaponVerb.Stab, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Normal, Difficulty.Easy,
			Alignment.Front, Orientation.Centre, 5.0, 1.0, trainingspear, trainingDamage,
			"@ duck|ducks inside the blow and lunge|lunges forward with a devastating low stab of $2 at $1",
			DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
		AddAttack("Training Spear 2-Handed High Counter Stab", BuiltInCombatMoveType.WardFreeAttack,
			MeleeWeaponVerb.Stab, Difficulty.Easy, Difficulty.ExtremelyHard, Difficulty.Hard, Difficulty.Hard,
			Alignment.Front, Orientation.Highest, 5.0, 1.0, trainingspear, trainingDamage,
			"@ duck|ducks inside the blow and lunge|lunges forward with a devastating high stab of $2 at $1",
			DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);

		#endregion

		#region Warhammers

		var warhammer = new WeaponType
		{
			Name = "Warhammer",
			Classification = (int)WeaponClassification.Lethal,
			AttackTrait = skills["Warhammer"],
			ParryTrait = parrySkills["Warhammer"],
			ParryBonus = -3,
			Reach = 4,
			StaminaPerParry = 5.0
		};
		context.WeaponTypes.Add(warhammer);
		context.SaveChanges();
		CreateWeaponComponent(warhammer);
		var trainingwarhammer = new WeaponType
		{
			Name = "Training Warhammer",
			Classification = (int)WeaponClassification.Training,
			AttackTrait = skills["Warhammer"],
			ParryTrait = parrySkills["Warhammer"],
			ParryBonus = -3,
			Reach = 4,
			StaminaPerParry = 5.0
		};
		context.WeaponTypes.Add(trainingwarhammer);
		context.SaveChanges();
		CreateWeaponComponent(trainingwarhammer);

		AddAttack("Warhammer 1-Handed Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight,
			Orientation.High, 6.5, 1.4, warhammer, veryGoodDamage, "@ swing|swings $2 at $1", DamageType.Crushing,
			handedness: AttackHandednessOptions.OneHandedOnly, additionalInfo: ((int)Difficulty.Hard).ToString());
		AddAttack("Warhammer 1-Handed High Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Highest, 6.5, 1.6, warhammer, veryGoodDamage, "@ swing|swings $2 in a high blow at $1",
			DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
			additionalInfo: ((int)Difficulty.Hard).ToString());
		AddAttack("Warhammer 1-Handed Low Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Centre, 6.5, 1.4, warhammer, veryGoodDamage, "@ swing|swings $2 in a low blow at $1",
			DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
			additionalInfo: ((int)Difficulty.Hard).ToString());
		AddAttack("Warhammer 1-Handed Leg Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight, Orientation.Low,
			6.5, 1.6, warhammer, veryGoodDamage, "@ swing|swings $2 at $1's legs", DamageType.Crushing,
			handedness: AttackHandednessOptions.OneHandedOnly, additionalInfo: ((int)Difficulty.Hard).ToString());
		AddAttack("Warhammer 1-Handed Arm Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Hard, Difficulty.VeryEasy, Alignment.FrontRight,
			Orientation.Appendage, 6.5, 1.6, warhammer, veryGoodDamage, "@ swing|swings $2 at $1's arms",
			DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
			additionalInfo: ((int)Difficulty.Hard).ToString());
		AddAttack("Warhammer 1-Handed Overhead Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
			Difficulty.Hard, Difficulty.VeryEasy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
			Orientation.Highest, 8.0, 1.8, warhammer, greatDamage, "@ swing|swings $2 in an overhead blow at $1",
			DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
			additionalInfo: ((int)Difficulty.Hard).ToString());
		AddAttack("Warhammer 1-Handed Haft Bash", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Bash,
			Difficulty.VeryHard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
			Orientation.Highest, 5.0, 1.3, warhammer, badDamage,
			"@ heave|heaves the haft of $2 down towards $1's head");

		AddAttack("Warhammer Smash", BuiltInCombatMoveType.MeleeWeaponSmashItem, MeleeWeaponVerb.Swing, Difficulty.Easy,
			Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 6.0, 1.3,
			warhammer, veryGoodDamage, "@ swing|swings $2 at $1");
		AddAttack("Warhammer Crush Head", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
			Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5,
			warhammer, coupdegraceDamage,
			"@ hold|holds $2 up above &0's head and bring|brings it down at $1 in a devastating swing",
			DamageType.Crushing, additionalInfo: context.BodypartProtos.First(x => x.Name == "scalp").Id.ToString());
		AddAttack("Warhammer Crush Neck", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
			Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5,
			warhammer, coupdegraceDamage,
			"@ hold|holds $2 up above &0's head and bring|brings it down at $1 in a devastating swing",
			DamageType.Crushing, additionalInfo: context.BodypartProtos.First(x => x.Name == "bneck").Id.ToString());
		AddAttack("Warhammer Crush Right Hand", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing,
			Difficulty.Hard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
			Orientation.Highest, 8.0, 1.5, warhammer, coupdegraceDamage,
			"@ hold|holds $2 up above &0's head and bring|brings it down at $1 in a devastating swing",
			DamageType.Crushing, additionalInfo: context.BodypartProtos.First(x => x.Name == "rhand").Id.ToString());
		AddAttack("Warhammer Crush Left Hand", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing,
			Difficulty.Hard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
			Orientation.Highest, 8.0, 1.5, warhammer, coupdegraceDamage,
			"@ hold|holds $2 up above &0's head and bring|brings it down at $1 in a devastating swing",
			DamageType.Crushing, additionalInfo: context.BodypartProtos.First(x => x.Name == "lhand").Id.ToString());
		AddAttack("Warhammer Crush Right Leg", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing,
			Difficulty.Hard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
			Orientation.Highest, 8.0, 1.5, warhammer, coupdegraceDamage,
			"@ hold|holds $2 up above &0's head and bring|brings it down at $1 in a devastating swing",
			DamageType.Crushing, additionalInfo: context.BodypartProtos.First(x => x.Name == "rshin").Id.ToString());
		AddAttack("Warhammer Crush Left Leg", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
			Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5,
			warhammer, coupdegraceDamage,
			"@ hold|holds $2 up above &0's head and bring|brings it down at $1 in a devastating swing",
			DamageType.Crushing, additionalInfo: context.BodypartProtos.First(x => x.Name == "lshin").Id.ToString());

		AddAttack("Warhammer 2-Handed Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
			Difficulty.Hard, Difficulty.Easy, Difficulty.ExtremelyHard, Difficulty.VeryEasy, Alignment.FrontRight,
			Orientation.High, 6.5, 1.2, warhammer, veryGoodDamage, "@ swing|swings $2 at $1", DamageType.Crushing,
			handedness: AttackHandednessOptions.TwoHandedOnly, additionalInfo: ((int)Difficulty.VeryHard).ToString());
		AddAttack("Warhammer 2-Handed High Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
			Difficulty.Hard, Difficulty.VeryEasy, Difficulty.ExtremelyHard, Difficulty.VeryEasy, Alignment.FrontRight,
			Orientation.Highest, 6.5, 1.2, warhammer, veryGoodDamage, "@ swing|swings $2 high at $1",
			DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
			additionalInfo: ((int)Difficulty.VeryHard).ToString());
		AddAttack("Warhammer 2-Handed Low Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
			Difficulty.Hard, Difficulty.Normal, Difficulty.ExtremelyHard, Difficulty.ExtremelyEasy,
			Alignment.FrontRight, Orientation.Centre, 6.5, 1.2, warhammer, veryGoodDamage,
			"@ swing|swings $2 low at $1", DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
			additionalInfo: ((int)Difficulty.VeryHard).ToString());
		AddAttack("Warhammer 2-Handed Heavy Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
			Difficulty.Hard, Difficulty.VeryEasy, Difficulty.ExtremelyHard, Difficulty.VeryEasy, Alignment.FrontRight,
			Orientation.High, 8.5, 1.4, warhammer, greatDamage, "@ swing|swings $2 at $1", DamageType.Crushing,
			handedness: AttackHandednessOptions.TwoHandedOnly,
			additionalInfo: ((int)Difficulty.ExtremelyHard).ToString());
		AddAttack("Warhammer 2-Handed Heavy High Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
			Difficulty.Hard, Difficulty.ExtremelyEasy, Difficulty.ExtremelyHard, Difficulty.VeryEasy,
			Alignment.FrontRight, Orientation.Highest, 8.5, 1.4, warhammer, greatDamage, "@ swing|swings $2 high at $1",
			DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
			additionalInfo: ((int)Difficulty.ExtremelyHard).ToString());
		AddAttack("Warhammer 2-Handed Heavy Low Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
			Difficulty.Hard, Difficulty.Easy, Difficulty.ExtremelyHard, Difficulty.ExtremelyEasy, Alignment.FrontRight,
			Orientation.Centre, 8.5, 1.4, warhammer, greatDamage, "@ swing|swings $2 low at $1", DamageType.Crushing,
			handedness: AttackHandednessOptions.TwoHandedOnly,
			additionalInfo: ((int)Difficulty.ExtremelyHard).ToString());
		AddAttack("Warhammer 2-Handed Downed Killing Blow", BuiltInCombatMoveType.DownedAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.ExtremelyHard, Difficulty.ExtremelyEasy, Alignment.Front,
			Orientation.Highest, 8.5, 1.4, warhammer, greatDamage,
			"@ raise|raises $2 above &0's head and bring|brings it crashing down towards $1's {0} as #1 %1|lay|lays prone and vulnerable",
			DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
			additionalInfo: ((int)Difficulty.ExtremelyHard).ToString());
		AddAttack("Warhammer 2-Handed Downed Hobbling Blow", BuiltInCombatMoveType.DownedAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.ExtremelyHard, Difficulty.ExtremelyEasy,
			Alignment.FrontRight, Orientation.Lowest, 8.5, 1.4, warhammer, greatDamage,
			"@ raise|raises $2 above &0's head and bring|brings it crashing down towards $1's {0} as #1 %1|lay|lays prone and vulnerable",
			DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
			additionalInfo: ((int)Difficulty.ExtremelyHard).ToString());
		AddAttack("Warhammer 2-Handed Downed Maiming Blow", BuiltInCombatMoveType.DownedAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.ExtremelyHard, Difficulty.ExtremelyEasy,
			Alignment.FrontRight, Orientation.Appendage, 8.5, 1.4, warhammer, greatDamage,
			"@ raise|raises $2 above &0's head and bring|brings it crashing down towards $1's {0} as #1 %1|lay|lays prone and vulnerable",
			DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
			additionalInfo: ((int)Difficulty.ExtremelyHard).ToString());
		AddAttack("Warhammer 2-Handed Downed Finishing Blow", BuiltInCombatMoveType.DownedAttack, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.ExtremelyHard, Difficulty.ExtremelyEasy,
			Alignment.FrontRight, Orientation.Centre, 8.5, 1.4, warhammer, greatDamage,
			"@ raise|raises $2 above &0's head and bring|brings it crashing down towards $1's {0} as #1 %1|lay|lays prone and vulnerable",
			DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
			additionalInfo: ((int)Difficulty.ExtremelyHard).ToString());

		AddAttack("Training Warhammer 1-Handed Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight,
			Orientation.High, 6.5, 1.4, trainingwarhammer, trainingDamage, "@ swing|swings $2 at $1",
			DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
			additionalInfo: ((int)Difficulty.Hard).ToString());
		AddAttack("Training Warhammer 1-Handed High Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Highest, 6.5, 1.6, trainingwarhammer, trainingDamage, "@ swing|swings $2 in a high blow at $1",
			DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
			additionalInfo: ((int)Difficulty.Hard).ToString());
		AddAttack("Training Warhammer 1-Handed Low Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight,
			Orientation.Centre, 6.5, 1.4, trainingwarhammer, trainingDamage, "@ swing|swings $2 in a low blow at $1",
			DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
			additionalInfo: ((int)Difficulty.Hard).ToString());
		AddAttack("Training Warhammer 1-Handed Leg Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight, Orientation.Low,
			6.5, 1.6, trainingwarhammer, trainingDamage, "@ swing|swings $2 at $1's legs", DamageType.Crushing,
			handedness: AttackHandednessOptions.OneHandedOnly, additionalInfo: ((int)Difficulty.Hard).ToString());
		AddAttack("Training Warhammer 1-Handed Arm Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
			Difficulty.Normal, Difficulty.Easy, Difficulty.Hard, Difficulty.VeryEasy, Alignment.FrontRight,
			Orientation.Appendage, 6.5, 1.6, trainingwarhammer, trainingDamage, "@ swing|swings $2 at $1's arms",
			DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
			additionalInfo: ((int)Difficulty.Hard).ToString());
		AddAttack("Training Warhammer 1-Handed Overhead Swing", BuiltInCombatMoveType.StaggeringBlow,
			MeleeWeaponVerb.Swing, Difficulty.Hard, Difficulty.VeryEasy, Difficulty.VeryHard, Difficulty.Easy,
			Alignment.Front, Orientation.Highest, 8.0, 1.8, trainingwarhammer, trainingDamage,
			"@ swing|swings $2 in an overhead blow at $1", DamageType.Crushing,
			handedness: AttackHandednessOptions.OneHandedOnly, additionalInfo: ((int)Difficulty.Hard).ToString());
		AddAttack("Training Warhammer 1-Handed Haft Bash", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Bash,
			Difficulty.VeryHard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
			Orientation.Highest, 5.0, 1.3, trainingwarhammer, trainingDamage,
			"@ heave|heaves the haft of $2 down towards $1's head");

		AddAttack("Training Warhammer 2-Handed Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
			Difficulty.Hard, Difficulty.Easy, Difficulty.ExtremelyHard, Difficulty.VeryEasy, Alignment.FrontRight,
			Orientation.High, 6.5, 1.2, trainingwarhammer, trainingDamage, "@ swing|swings $2 at $1",
			DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
			additionalInfo: ((int)Difficulty.VeryHard).ToString());
		AddAttack("Training Warhammer 2-Handed High Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
			Difficulty.Hard, Difficulty.VeryEasy, Difficulty.ExtremelyHard, Difficulty.VeryEasy, Alignment.FrontRight,
			Orientation.Highest, 6.5, 1.2, trainingwarhammer, trainingDamage, "@ swing|swings $2 high at $1",
			DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
			additionalInfo: ((int)Difficulty.VeryHard).ToString());
		AddAttack("Training Warhammer 2-Handed Low Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
			Difficulty.Hard, Difficulty.Normal, Difficulty.ExtremelyHard, Difficulty.ExtremelyEasy,
			Alignment.FrontRight, Orientation.Centre, 6.5, 1.2, trainingwarhammer, trainingDamage,
			"@ swing|swings $2 low at $1", DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
			additionalInfo: ((int)Difficulty.VeryHard).ToString());
		AddAttack("Training Warhammer 2-Handed Heavy Swing", BuiltInCombatMoveType.StaggeringBlow,
			MeleeWeaponVerb.Swing, Difficulty.Hard, Difficulty.VeryEasy, Difficulty.ExtremelyHard, Difficulty.VeryEasy,
			Alignment.FrontRight, Orientation.High, 8.5, 1.4, trainingwarhammer, trainingDamage,
			"@ swing|swings $2 at $1", DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
			additionalInfo: ((int)Difficulty.ExtremelyHard).ToString());
		AddAttack("Training Warhammer 2-Handed Heavy High Swing", BuiltInCombatMoveType.StaggeringBlow,
			MeleeWeaponVerb.Swing, Difficulty.Hard, Difficulty.ExtremelyEasy, Difficulty.ExtremelyHard,
			Difficulty.VeryEasy, Alignment.FrontRight, Orientation.Highest, 8.5, 1.4, trainingwarhammer, trainingDamage,
			"@ swing|swings $2 high at $1", DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
			additionalInfo: ((int)Difficulty.ExtremelyHard).ToString());
		AddAttack("Training Warhammer 2-Handed Heavy Low Swing", BuiltInCombatMoveType.StaggeringBlow,
			MeleeWeaponVerb.Swing, Difficulty.Hard, Difficulty.Easy, Difficulty.ExtremelyHard, Difficulty.ExtremelyEasy,
			Alignment.FrontRight, Orientation.Centre, 8.5, 1.4, trainingwarhammer, trainingDamage,
			"@ swing|swings $2 low at $1", DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
			additionalInfo: ((int)Difficulty.ExtremelyHard).ToString());

		#endregion

		#region Shields

		var blockingSkill = coreSkills.GetValueOrDefault("Blocking") ?? coreSkills["Block"];
		var shield = new WeaponType
		{
			Name = "Shield",
			Classification = (int)WeaponClassification.Shield,
			AttackTrait = blockingSkill,
			ParryTrait = blockingSkill,
			ParryBonus = -2,
			Reach = 0,
			StaminaPerParry = 3.5
		};
		context.WeaponTypes.Add(shield);
		context.SaveChanges();
		CreateWeaponComponent(shield);
		context.StaticConfigurations.Add(new StaticConfiguration
			{ SettingName = "DefaultShieldMeleeWeaponType", Definition = shield.Id.ToString() });
		context.SaveChanges();

		AddAttack("Shield Bash", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Bash, Difficulty.Normal,
			Difficulty.Easy, Difficulty.Insane, Difficulty.Hard, Alignment.FrontLeft, Orientation.High, 4.0, 0.8,
			shield, poorDamage, "@ quickly jolt|jolts forward and try|tries to bash $1 with $2", DamageType.Crushing,
			additionalInfo: ((int)Difficulty.Trivial).ToString(),
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Shield);
		AddAttack("Shield Head Smash", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Slam, Difficulty.Normal,
			Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Alignment.FrontLeft, Orientation.Highest, 4.0, 0.8,
			shield, poorDamage, "@ raise|raises $2 up high and try|tries to smash down at $1", DamageType.Crushing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Shield);
		AddAttack("Shield Foot Stomp", BuiltInCombatMoveType.DownedAttack, MeleeWeaponVerb.Slam, Difficulty.Hard,
			Difficulty.Easy, Difficulty.Easy, Difficulty.Easy, Alignment.FrontLeft, Orientation.Lowest, 4.0, 0.8,
			shield, poorDamage, "@ slam|slams $2 downward towards $1's legs", DamageType.Crushing,
			additionalInfo: ((int)Difficulty.Easy).ToString(),
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Shield |
			            CombatMoveIntentions.Cripple);
		AddAttack("Shield Smash Head", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
			Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5,
			shield, coupdegraceDamage,
			"@ hold|holds $2 up above &0's head and bring|brings it down at $1's {0} in a devastating swing",
			DamageType.Crushing, additionalInfo: context.BodypartProtos.First(x => x.Name == "scalp").Id.ToString());

		#endregion
	}

	private void SeedUnarmedCombatMessage(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> questionAnswers)
	{
		void AddCombatMessage(string message, string? failmessage, BuiltInCombatMoveType move, double chance,
			int priority,
			MeleeWeaponVerb? verb, Outcome? outcome)
		{
			context.CombatMessages.Add(new CombatMessage
			{
				Message = message,
				FailureMessage = failmessage ?? message,
				Type = (int)move,
				Chance = chance,
				Priority = priority,
				Verb = verb.HasValue ? (int)verb.Value : null,
				Outcome = outcome.HasValue ? (int)outcome.Value : null
			});
			context.SaveChanges();
		}

		var attackAddendum = "";
		switch (questionAnswers["messagestyle"].ToLowerInvariant())
		{
			case "sentences":
			case "sparse":
				attackAddendum = ".";
				break;
		}

		#region Attack Fallbacks

		AddCombatMessage($"$0 attack|attacks $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 1, null, null);
		AddCombatMessage($"$0 attack|attacks $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 1, null, null);
		AddCombatMessage($"$0 attack|attacks $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 1, null, null);
		AddCombatMessage($"$0 attack|attacks $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 1, null, null);

		AddCombatMessage($"$0 swing|swings &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 2, MeleeWeaponVerb.Swing, null);
		AddCombatMessage($"$0 thrust|thrusts &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 2, MeleeWeaponVerb.Thrust, null);
		AddCombatMessage($"$0 jab|jabs at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 2, MeleeWeaponVerb.Jab, null);
		AddCombatMessage($"$0 stab|stabs at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 2, MeleeWeaponVerb.Stab, null);
		AddCombatMessage($"$0 attempt|attempts to bash $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 2, MeleeWeaponVerb.Bash, null);
		AddCombatMessage($"$0 attempt|attempts to bite $1{attackAddendum}", null,
			BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 2, MeleeWeaponVerb.Bite, null);
		AddCombatMessage($"$0 rake|rakes at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 2, MeleeWeaponVerb.Claw, null);
		AddCombatMessage($"$0 attempt|attempts to kick $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 2, MeleeWeaponVerb.Kick, null);
		AddCombatMessage($"$0 attempt|attempts to strike $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 2, MeleeWeaponVerb.Strike, null);
		AddCombatMessage($"$0 sweep|sweeps &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 2, MeleeWeaponVerb.Sweep, null);
		AddCombatMessage($"$0 slams|slams &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 2, MeleeWeaponVerb.Slam, null);
		AddCombatMessage($"$0 attempt|attempts to punch $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 2, MeleeWeaponVerb.Punch, null);
		AddCombatMessage($"$0 attack|attacks $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 1, null, null);

		AddCombatMessage($"$0 clumsily swing|swings &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Swing, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily thrust|thrusts &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Thrust, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily jab|jabs at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Jab, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily stab|stabs at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Stab, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily attempt|attempts to bash $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Bash, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily attempt|attempts to bite $1{attackAddendum}", null,
			BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Bite, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily rake|rakes at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Claw, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily attempt|attempts to kick $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Kick, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily attempt|attempts to strike $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Strike, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily sweep|sweeps &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Sweep, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily slams|slams &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Slam, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily attempt|attempts to punch $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Punch, Outcome.MajorFail);

		AddCombatMessage($"$0 adeptly swing|swings &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Swing, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly thrust|thrusts &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Thrust, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly jab|jabs at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Jab, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly stab|stabs at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Stab, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly attempt|attempts to bash $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Bash, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly attempt|attempts to bite $1{attackAddendum}", null,
			BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Bite, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly rake|rakes at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Claw, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly attempt|attempts to kick $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Kick, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly attempt|attempts to strike $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Strike, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly sweep|sweeps &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Sweep, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly slams|slams &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Slam, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly attempt|attempts to punch $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Punch, Outcome.MajorPass);

		#region Clinc

		AddCombatMessage($"$0 swing|swings &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 2, MeleeWeaponVerb.Swing, null);
		AddCombatMessage($"$0 thrust|thrusts &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 2, MeleeWeaponVerb.Thrust, null);
		AddCombatMessage($"$0 jab|jabs at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 2, MeleeWeaponVerb.Jab, null);
		AddCombatMessage($"$0 stab|stabs at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 2, MeleeWeaponVerb.Stab, null);
		AddCombatMessage($"$0 attempt|attempts to bash $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 2, MeleeWeaponVerb.Bash, null);
		AddCombatMessage($"$0 attempt|attempts to bite $1{attackAddendum}", null,
			BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 2, MeleeWeaponVerb.Bite, null);
		AddCombatMessage($"$0 rake|rakes at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 2, MeleeWeaponVerb.Claw, null);
		AddCombatMessage($"$0 attempt|attempts to kick $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 2, MeleeWeaponVerb.Kick, null);
		AddCombatMessage($"$0 attempt|attempts to strike $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 2, MeleeWeaponVerb.Strike, null);
		AddCombatMessage($"$0 sweep|sweeps &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 2, MeleeWeaponVerb.Sweep, null);
		AddCombatMessage($"$0 slams|slams &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 2, MeleeWeaponVerb.Slam, null);
		AddCombatMessage($"$0 attempt|attempts to punch $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 2, MeleeWeaponVerb.Punch, null);
		AddCombatMessage($"$0 attack|attacks $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 1, null, null);

		AddCombatMessage($"$0 clumsily swing|swings &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Swing, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily thrust|thrusts &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Thrust, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily jab|jabs at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Jab, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily stab|stabs at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Stab, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily attempt|attempts to bash $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Bash, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily attempt|attempts to bite $1{attackAddendum}", null,
			BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Bite, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily rake|rakes at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Claw, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily attempt|attempts to kick $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Kick, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily attempt|attempts to strike $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Strike, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily sweep|sweeps &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Sweep, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily slams|slams &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Slam, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily attempt|attempts to punch $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Punch, Outcome.MajorFail);

		AddCombatMessage($"$0 adeptly swing|swings &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Swing, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly thrust|thrusts &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Thrust, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly jab|jabs at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Jab, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly stab|stabs at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Stab, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly attempt|attempts to bash $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Bash, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly attempt|attempts to bite $1{attackAddendum}", null,
			BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Bite, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly rake|rakes at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Claw, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly attempt|attempts to kick $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Kick, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly attempt|attempts to strike $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Strike, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly sweep|sweeps &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Sweep, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly slams|slams &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Slam, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly attempt|attempts to punch $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Punch, Outcome.MajorPass);

		#endregion

		#region Trip

		AddCombatMessage($"$0 swing|swings &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Swing, null);
		AddCombatMessage($"$0 thrust|thrusts &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Thrust, null);
		AddCombatMessage($"$0 jab|jabs at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Jab, null);
		AddCombatMessage($"$0 stab|stabs at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Stab, null);
		AddCombatMessage($"$0 attempt|attempts to bash $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Bash, null);
		AddCombatMessage($"$0 attempt|attempts to bite $1{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Bite, null);
		AddCombatMessage($"$0 rake|rakes at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Claw, null);
		AddCombatMessage($"$0 attempt|attempts to kick $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Kick, null);
		AddCombatMessage($"$0 attempt|attempts to strike $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Strike, null);
		AddCombatMessage($"$0 sweep|sweeps &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Sweep, null);
		AddCombatMessage($"$0 slams|slams &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Slam, null);
		AddCombatMessage($"$0 attempt|attempts to punch $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Punch, null);
		AddCombatMessage($"$0 attack|attacks $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 1, null, null);

		AddCombatMessage($"$0 clumsily swing|swings &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Swing, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily thrust|thrusts &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Thrust, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily jab|jabs at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Jab, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily stab|stabs at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Stab, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily attempt|attempts to bash $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Bash, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily attempt|attempts to bite $1{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Bite, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily rake|rakes at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Claw, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily attempt|attempts to kick $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Kick, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily attempt|attempts to strike $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Strike, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily sweep|sweeps &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Sweep, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily slams|slams &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Slam, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily attempt|attempts to punch $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Punch, Outcome.MajorFail);

		AddCombatMessage($"$0 adeptly swing|swings &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Swing, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly thrust|thrusts &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Thrust, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly jab|jabs at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Jab, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly stab|stabs at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Stab, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly attempt|attempts to bash $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Bash, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly attempt|attempts to bite $1{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Bite, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly rake|rakes at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Claw, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly attempt|attempts to kick $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Kick, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly attempt|attempts to strike $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Strike, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly sweep|sweeps &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Sweep, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly slams|slams &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Slam, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly attempt|attempts to punch $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Punch, Outcome.MajorPass);

		AddCombatMessage($"$0 swing|swings &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 2, MeleeWeaponVerb.Swing, null);
		AddCombatMessage($"$0 thrust|thrusts &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 2, MeleeWeaponVerb.Thrust, null);
		AddCombatMessage($"$0 jab|jabs at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 2, MeleeWeaponVerb.Jab, null);
		AddCombatMessage($"$0 stab|stabs at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 2, MeleeWeaponVerb.Stab, null);
		AddCombatMessage($"$0 attempt|attempts to bash $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 2, MeleeWeaponVerb.Bash, null);
		AddCombatMessage($"$0 attempt|attempts to bite $1{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 2, MeleeWeaponVerb.Bite, null);
		AddCombatMessage($"$0 rake|rakes at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 2, MeleeWeaponVerb.Claw, null);
		AddCombatMessage($"$0 attempt|attempts to kick $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 2, MeleeWeaponVerb.Kick, null);
		AddCombatMessage($"$0 attempt|attempts to strike $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 2, MeleeWeaponVerb.Strike, null);
		AddCombatMessage($"$0 sweep|sweeps &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 2, MeleeWeaponVerb.Sweep, null);
		AddCombatMessage($"$0 slams|slams &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 2, MeleeWeaponVerb.Slam, null);
		AddCombatMessage($"$0 attempt|attempts to punch $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 2, MeleeWeaponVerb.Punch, null);
		AddCombatMessage($"$0 attack|attacks $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 1, null, null);

		AddCombatMessage($"$0 clumsily swing|swings &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Swing, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily thrust|thrusts &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Thrust, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily jab|jabs at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Jab, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily stab|stabs at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Stab, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily attempt|attempts to bash $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Bash, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily attempt|attempts to bite $1{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Bite, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily rake|rakes at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Claw, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily attempt|attempts to kick $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Kick, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily attempt|attempts to strike $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Strike, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily sweep|sweeps &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Sweep, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily slams|slams &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Slam, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily attempt|attempts to punch $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Punch, Outcome.MajorFail);

		AddCombatMessage($"$0 adeptly swing|swings &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Swing, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly thrust|thrusts &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Thrust, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly jab|jabs at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Jab, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly stab|stabs at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Stab, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly attempt|attempts to bash $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Bash, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly attempt|attempts to bite $1{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Bite, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly rake|rakes at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Claw, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly attempt|attempts to kick $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Kick, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly attempt|attempts to strike $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Strike, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly sweep|sweeps &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Sweep, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly slams|slams &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Slam, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly attempt|attempts to punch $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Punch, Outcome.MajorPass);

		#endregion

		#region Stagger

		AddCombatMessage($"$0 swing|swings &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Swing, null);
		AddCombatMessage($"$0 thrust|thrusts &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Thrust, null);
		AddCombatMessage($"$0 jab|jabs at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Jab, null);
		AddCombatMessage($"$0 stab|stabs at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Stab, null);
		AddCombatMessage($"$0 attempt|attempts to bash $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Bash, null);
		AddCombatMessage($"$0 attempt|attempts to bite $1{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Bite, null);
		AddCombatMessage($"$0 rake|rakes at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Claw, null);
		AddCombatMessage($"$0 attempt|attempts to kick $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Kick, null);
		AddCombatMessage($"$0 attempt|attempts to strike $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Strike, null);
		AddCombatMessage($"$0 sweep|sweeps &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Sweep, null);
		AddCombatMessage($"$0 slams|slams &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Slam, null);
		AddCombatMessage($"$0 attempt|attempts to punch $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Punch, null);
		AddCombatMessage($"$0 attack|attacks $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 1, null, null);

		AddCombatMessage($"$0 clumsily swing|swings &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Swing, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily thrust|thrusts &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Thrust, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily jab|jabs at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Jab, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily stab|stabs at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Stab, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily attempt|attempts to bash $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Bash, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily attempt|attempts to bite $1{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Bite, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily rake|rakes at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Claw, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily attempt|attempts to kick $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Kick, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily attempt|attempts to strike $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Strike, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily sweep|sweeps &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Sweep, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily slams|slams &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Slam, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily attempt|attempts to punch $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Punch, Outcome.MajorFail);

		AddCombatMessage($"$0 adeptly swing|swings &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Swing, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly thrust|thrusts &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Thrust, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly jab|jabs at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Jab, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly stab|stabs at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Stab, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly attempt|attempts to bash $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Bash, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly attempt|attempts to bite $1{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Bite, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly rake|rakes at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Claw, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly attempt|attempts to kick $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Kick, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly attempt|attempts to strike $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Strike, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly sweep|sweeps &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Sweep, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly slams|slams &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Slam, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly attempt|attempts to punch $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Punch, Outcome.MajorPass);

		AddCombatMessage($"$0 swing|swings &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 2, MeleeWeaponVerb.Swing, null);
		AddCombatMessage($"$0 thrust|thrusts &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 2, MeleeWeaponVerb.Thrust, null);
		AddCombatMessage($"$0 jab|jabs at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 2, MeleeWeaponVerb.Jab, null);
		AddCombatMessage($"$0 stab|stabs at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 2, MeleeWeaponVerb.Stab, null);
		AddCombatMessage($"$0 attempt|attempts to bash $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 2, MeleeWeaponVerb.Bash, null);
		AddCombatMessage($"$0 attempt|attempts to bite $1{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 2, MeleeWeaponVerb.Bite, null);
		AddCombatMessage($"$0 rake|rakes at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 2, MeleeWeaponVerb.Claw, null);
		AddCombatMessage($"$0 attempt|attempts to kick $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 2, MeleeWeaponVerb.Kick, null);
		AddCombatMessage($"$0 attempt|attempts to strike $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 2, MeleeWeaponVerb.Strike, null);
		AddCombatMessage($"$0 sweep|sweeps &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 2, MeleeWeaponVerb.Sweep, null);
		AddCombatMessage($"$0 slams|slams &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 2, MeleeWeaponVerb.Slam, null);
		AddCombatMessage($"$0 attempt|attempts to punch $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 2, MeleeWeaponVerb.Punch, null);
		AddCombatMessage($"$0 attack|attacks $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 1, null, null);

		AddCombatMessage($"$0 clumsily swing|swings &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Swing, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily thrust|thrusts &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Thrust, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily jab|jabs at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Jab, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily stab|stabs at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Stab, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily attempt|attempts to bash $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Bash, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily attempt|attempts to bite $1{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Bite, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily rake|rakes at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Claw, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily attempt|attempts to kick $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Kick, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily attempt|attempts to strike $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Strike, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily sweep|sweeps &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Sweep, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily slams|slams &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Slam, Outcome.MajorFail);
		AddCombatMessage($"$0 clumsily attempt|attempts to punch $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Punch, Outcome.MajorFail);

		AddCombatMessage($"$0 adeptly swing|swings &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Swing, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly thrust|thrusts &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Thrust, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly jab|jabs at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Jab, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly stab|stabs at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Stab, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly attempt|attempts to bash $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Bash, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly attempt|attempts to bite $1{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Bite, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly rake|rakes at $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Claw, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly attempt|attempts to kick $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Kick, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly attempt|attempts to strike $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Strike, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly sweep|sweeps &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Sweep, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly slams|slams &0's {{0}} at $1{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Slam, Outcome.MajorPass);
		AddCombatMessage($"$0 adeptly attempt|attempts to punch $1 with &0's {{0}}{attackAddendum}", null,
			BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Punch, Outcome.MajorPass);

		#endregion

		#endregion

		#region Built in Moves

		AddCombatMessage("$0 rescue|rescues $1 from combat with $2",
			"$0 attempt|attempts to rescue $1 from combat with $2, but is unsuccessful", BuiltInCombatMoveType.Rescue,
			1.0, 1, null, null);
		AddCombatMessage($"$0 attempt|attempts to grapple $1{attackAddendum}", null,
			BuiltInCombatMoveType.InitiateGrapple, 1.0, 1, null, null);
		AddCombatMessage($"$0 attempt|attempts to put $1's {{1}} into a lock{attackAddendum}", null,
			BuiltInCombatMoveType.ExtendGrapple, 1.0, 1, null, null);
		AddCombatMessage($"$0 wrench|wrenches $1's {{1}} in an attempt to disable it{attackAddendum}", null,
			BuiltInCombatMoveType.WrenchAttack, 1.0, 1, null, null);
		AddCombatMessage("$0 viciously strangle|strangles $1's {1}", null, BuiltInCombatMoveType.StrangleAttack, 1.0, 1,
			null, null);
		AddCombatMessage("$0 attempt|attempts to disarm $1 with $2", null, BuiltInCombatMoveType.Disarm, 1.0, 1, null,
			null);
		AddCombatMessage("$0 attempt|attempts to flee from combat", null, BuiltInCombatMoveType.Flee, 1.0, 1, null,
			null);
		AddCombatMessage("$0 attempt|attempts to grab $1", null, BuiltInCombatMoveType.RetrieveItem, 1.0, 1, null,
			null);
		AddCombatMessage("$0 charge|charges into melee range with $1",
			"$0 attempt|attempts to charge into melee range with $1, but fall|falls short",
			BuiltInCombatMoveType.ChargeToMelee, 1.0, 1, null, null);
		AddCombatMessage("$0 advance|advances into melee range with $1",
			"$0 attempt|attempts to advance into melee range with $1, but fall|falls short",
			BuiltInCombatMoveType.MoveToMelee, 1.0, 1, null, null);
		AddCombatMessage("$0 {0} $2 at $1 as #0 %0|advance|advances into melee range with &1",
			"$0 {0} $2 at $1 as #0 %0|attempt|attempts to advance into melee range with &1, but %0|fall|falls short",
			BuiltInCombatMoveType.AdvanceAndFire, 1.0, 1, null, null);
		AddCombatMessage("$0 receive|receives $1's charge with an attack of $2", null,
			BuiltInCombatMoveType.ReceiveCharge, 1.0, 1, null, null);
		AddCombatMessage("$0 step|steps close to $1 and attempt|attempts to begin a clinch with &1", null,
			BuiltInCombatMoveType.StartClinch, 1.0, 1, null, null);
		AddCombatMessage("$0 try|tries to break free of the clinch with $1", null, BuiltInCombatMoveType.BreakClinch,
			1.0, 1, null, null);
		AddCombatMessage("@ stand|stands and {0} $2 at $1", null, BuiltInCombatMoveType.StandAndFire, 1.0, 1, null,
			null);
		AddCombatMessage("@ {0} $2 at $1 as &0 fall|falls back", null, BuiltInCombatMoveType.SkirmishAndFire, 1.0, 1,
			null, null);
		AddCombatMessage("@ {0} $2 at $1", null, BuiltInCombatMoveType.RangedWeaponAttack, 1.0, 1, null, null);
		AddCombatMessage("@ aim|aims $2 at $1", "@ continue|continues to aim $2 at $1",
			BuiltInCombatMoveType.AimRangedWeapon, 1.0, 1, null, null);
		AddCombatMessage("$0 attempt|attempts to coup-de-grace $1 with a blow to &1's {0} from $2", null,
			BuiltInCombatMoveType.CoupDeGrace, 1.0, 1, null, null);
		AddCombatMessage("$0 attack|attacks $1$?2| on $2||$ with &0's {0}, causing {1}", null,
			BuiltInCombatMoveType.UnarmedSmashItem, 1.0, 1, null, null);
		AddCombatMessage($"$0 emit|emits a beam of energy towards $1{attackAddendum}", null,
			BuiltInCombatMoveType.BeamAttack, 1.0, 1, null, null);
		AddCombatMessage("$0 emit|emits a a horrid screeching that hurts your ears", null,
			BuiltInCombatMoveType.ScreechAttack, 1.0, 1, null, null);
		AddCombatMessage("$0 reach|reaches out and attempt|attempts to get $1's {1} in a position to strangle &1", null,
			BuiltInCombatMoveType.StrangleAttackExtendGrapple, 1.0, 1, null, null);

		#endregion

		#region Defenses

		var prependSuccess = "";
		var prependFailure = "";
		var append = "";
		var appendalternate = "";
		switch (questionAnswers["messagestyle"].ToLowerInvariant())
		{
			case "compact":
				prependSuccess = ", but ";
				prependFailure = ", and ";
				append = "but %1|get|gets ";
				appendalternate = "and %1|get|gets ";
				break;
			case "sparse":
				prependSuccess = ".\n";
				prependFailure = ".\n";
				append = "\n#1 %1|are|is ";
				appendalternate = "\n#1 %1|are|is ";
				break;
			case "sentences":
				prependSuccess = ". ";
				prependFailure = ". ";
				append = ". #1 %1|get|gets ";
				appendalternate = ". #1 %1|are|is ";
				break;
		}

		AddCombatMessage($"{prependSuccess}#1 %1|dodge|dodges out of the way",
			$"{prependFailure}#1 %1|attempt|attempts to dodge out of the way{append}hit on &1's {{1}}",
			BuiltInCombatMoveType.Dodge, 1.0, 1, null, null);
		AddCombatMessage($"{prependSuccess}#1 %1|parry|parries with $3",
			$"{prependFailure}#1 %1|attempt|attempts to parry with $3{append}hit on &1's {{1}}",
			BuiltInCombatMoveType.Parry, 1.0, 1, null, null);
		AddCombatMessage($"{prependSuccess}#1 %1|block|blocks with $3",
			$"{prependFailure}#1 %1|attempt|attempts to block with $3{append}hit on &1's {{1}}",
			BuiltInCombatMoveType.Block, 1.0, 1, null, null);
		AddCombatMessage($"{prependSuccess}#1 gracelessly %1|dodge|dodges out of the way",
			$"{prependFailure}#1 gracelessly %1|attempt|attempts to dodge out of the way{append}hit on &1's {{1}}",
			BuiltInCombatMoveType.Dodge, 1.0, 2, null, Outcome.MajorFail);
		AddCombatMessage($"{prependSuccess}#1 ineptly %1|parry|parries with $3",
			$"{prependFailure}#1 ineptly %1|attempt|attempts to parry with $3{append}hit on &1's {{1}}",
			BuiltInCombatMoveType.Parry, 1.0, 2, null, Outcome.MajorFail);
		AddCombatMessage($"{prependSuccess}#1 ineptly %1|block|blocks with $3",
			$"{prependFailure}#1 ineptly %1|attempt|attempts to block with $3{append}hit on &1's {{1}}",
			BuiltInCombatMoveType.Block, 1.0, 2, null, Outcome.MajorFail);
		AddCombatMessage($"{prependSuccess}#1 awkwardly %1|dodge|dodges out of the way",
			$"{prependFailure}#1 awkwardly %1|attempt|attempts to dodge out of the way{append}hit on &1's {{1}}",
			BuiltInCombatMoveType.Dodge, 1.0, 2, null, Outcome.Fail);
		AddCombatMessage($"{prependSuccess}#1 clumsily %1|parry|parries with $3",
			$"{prependFailure}#1 clumsily %1|attempt|attempts to parry with $3{append}hit on &1's {{1}}",
			BuiltInCombatMoveType.Parry, 1.0, 2, null, Outcome.Fail);
		AddCombatMessage($"{prependSuccess}#1 clumsily %1|block|blocks with $3",
			$"{prependFailure}#1 clumsily %1|attempt|attempts to block with $3{append}hit on &1's {{1}}",
			BuiltInCombatMoveType.Block, 1.0, 2, null, Outcome.Fail);
		AddCombatMessage($"{prependSuccess}#1 nimbly %1|dodge|dodges out of the way",
			$"{prependFailure}#1 nimbly %1|attempt|attempts to dodge out of the way{append}hit on &1's {{1}}",
			BuiltInCombatMoveType.Dodge, 1.0, 2, null, Outcome.Pass);
		AddCombatMessage($"{prependSuccess}#1 skillfully %1|parry|parries with $3",
			$"{prependFailure}#1 skillfully %1|attempt|attempts to parry with $3{append}hit on &1's {{1}}",
			BuiltInCombatMoveType.Parry, 1.0, 2, null, Outcome.Pass);
		AddCombatMessage($"{prependSuccess}#1 skillfully %1|block|blocks with $3",
			$"{prependFailure}#1 skillfully %1|attempt|attempts to block with $3{append}hit on &1's {{1}}",
			BuiltInCombatMoveType.Block, 1.0, 2, null, Outcome.Pass);
		AddCombatMessage($"{prependSuccess}#1 deftly %1|dodge|dodges out of the way",
			$"{prependFailure}#1 deftly %1|attempt|attempts to dodge out of the way{append}hit on &1's {{1}}",
			BuiltInCombatMoveType.Dodge, 1.0, 2, null, Outcome.MajorPass);
		AddCombatMessage($"{prependSuccess}#1 masterfully %1|parry|parries with $3",
			$"{prependFailure}#1 masterfully %1|attempt|attempts to parry with $3{append}hit on &1's {{1}}",
			BuiltInCombatMoveType.Parry, 1.0, 2, null, Outcome.MajorPass);
		AddCombatMessage($"{prependSuccess}#1 masterfully %1|block|blocks with $3",
			$"{prependFailure}#1 masterfully %1|attempt|attempts to block with $3{append}hit on &1's {{1}}",
			BuiltInCombatMoveType.Block, 1.0, 2, null, Outcome.MajorPass);
		AddCombatMessage($"{prependSuccess}#1 desperately %1|dodge|dodges out of the way",
			$"{prependFailure}#1 desperately %1|attempt|attempts to dodge out of the way{append}hit on &1's {{1}}",
			BuiltInCombatMoveType.DesperateDodge, 1.0, 1, null, null);
		AddCombatMessage($"{prependSuccess}#1 desperately %1|parry|parries with $3",
			$"{prependFailure}#1 desperately %1|attempt|attempts to parry with $3{append}hit on &1's {{1}}",
			BuiltInCombatMoveType.DesperateParry, 1.0, 1, null, null);
		AddCombatMessage($"{prependSuccess}#1 desperately %1|block|blocks with $3",
			$"{prependFailure}#1 desperately %1|attempt|attempts to block with $3{append}hit on &1's {{1}}",
			BuiltInCombatMoveType.DesperateBlock, 1.0, 1, null, null);

		AddCombatMessage(
			$"{prependSuccess}#1 %1|manage|manages to partially dodge the worst of the blow{append}hit on &1's {{1}}",
			$"{prependFailure}#1 %1|offer|offers no defense{appendalternate}hit on &1's {{1}}",
			BuiltInCombatMoveType.ClinchDodge, 1.0, 1, null, null);
		AddCombatMessage("@ dodge|dodges out of the way", "@ try|tries and fail|fails to dodge out of the way",
			BuiltInCombatMoveType.DodgeRange, 1.0, 1, null, null);
		AddCombatMessage("@ manage|manages to put $3 in the way", "@ try|tries and fail|fails to put $3 in the way",
			BuiltInCombatMoveType.BlockRange, 1.0, 1, null, null);

		AddCombatMessage($"{prependSuccess}#1 %1|are|is able to avoid the attempt",
			$"{prependFailure}#1 %1|aren't|isn't able to avoid it", BuiltInCombatMoveType.DodgeGrapple, 1.0, 1, null,
			null);
		AddCombatMessage($"{prependSuccess}#1 %1|manage|manages to wriggle free",
			$"{prependFailure}#1 %1|aren't|isn't able to wriggle free", BuiltInCombatMoveType.DodgeExtendGrapple, 1.0,
			1, null, null);
		AddCombatMessage($"{prependSuccess}#1 %1|manage|manages to turn it around and become the grappler!",
			$"{prependFailure}#1 %1|attempt|attempts to turn the grapple around, but is unsuccessful!",
			BuiltInCombatMoveType.CounterGrapple, 1.0, 1, null, null);

		#endregion
	}

	private void SeedShields(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> questionAnswers,
		IReadOnlyDictionary<string, TraitDefinition> skills)
	{
		var dbaccount = context.Accounts.First();
		var now = DateTime.UtcNow;

		void CreateShieldComponent(ShieldType type)
		{
			var component = new GameItemComponentProto
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
				Type = "Shield",
				Name = $"Shield_{type.Name.Replace(' ', '_')}",
				Description = $"Turns an item into a {type.Name} shield",
				Definition =
					$"<Definition><ShieldType>{type.Id}</ShieldType></Definition>"
			};
			context.GameItemComponentProtos.Add(component);
			context.SaveChanges();
		}

		var shieldArmour = new ArmourType
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
 		<Transform fromtype=""0"" totype=""2"" severity=""6""></Transform> <!-- Slashing to Crushing when <= VerySevere -->
 		<Transform fromtype=""1"" totype=""2"" severity=""6""></Transform> <!-- Chopping to Crushing when <= VerySevere -->
 		<Transform fromtype=""3"" totype=""2"" severity=""5""></Transform> <!-- Piercing to Crushing when <= Severe -->
 		<Transform fromtype=""4"" totype=""2"" severity=""5""></Transform> <!-- Ballistic to Crushing when <= Severe -->
 		<Transform fromtype=""9"" totype=""2"" severity=""6""></Transform> <!-- Bite to Crushing when <= VerySevere -->
 		<Transform fromtype=""10"" totype=""2"" severity=""6""></Transform> <!-- Claw to Crushing when <= VerySevere -->
 		<Transform fromtype=""15"" totype=""2"" severity=""6""></Transform> <!-- Shearing to Crushing when <= VerySevere -->
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
        <Expression damagetype=""0"">damage - (quality * strength/25000 * 0.25)</Expression>    <!-- Slashing -->
 		<Expression damagetype=""1"">damage - (quality * strength/25000 * 0.25)</Expression>    <!-- Chopping -->  
 		<Expression damagetype=""2"">damage - (quality * strength/10000 * 0.25)</Expression>    <!-- Crushing -->  
 		<Expression damagetype=""3"">damage - (quality * strength/25000 * 0.25)</Expression>    <!-- Piercing -->  
 		<Expression damagetype=""4"">damage - (quality * strength/25000 * 0.25)</Expression>    <!-- Ballistic -->  
 		<Expression damagetype=""5"">damage - (quality * 0.25)</Expression>    			      <!-- Burning -->
 		<Expression damagetype=""6"">damage - (quality * 0.25)</Expression>                     <!-- Freezing -->
 		<Expression damagetype=""7"">damage - (quality * 0.25)</Expression>                     <!-- Chemical -->
 		<Expression damagetype=""8"">damage - (quality * strength/10000 * 0.25)</Expression>    <!-- Shockwave -->
 		<Expression damagetype=""9"">damage - (quality * strength/25000 * 0.25)</Expression>    <!-- Bite -->
 		<Expression damagetype=""10"">damage - (quality * strength/25000 * 0.25)</Expression>   <!-- Claw -->
 		<Expression damagetype=""11"">damage - (quality * 0.25)</Expression>                    <!-- Electrical -->
 		<Expression damagetype=""12"">damage - (quality * 0.25)</Expression>                    <!-- Hypoxia -->
 		<Expression damagetype=""13"">damage - (quality * 0.25)</Expression>                    <!-- Cellular -->
		<Expression damagetype=""14"">damage - (quality * strength/10000 * 0.25)</Expression>   <!-- Sonic -->
 		<Expression damagetype=""15"">damage - (quality * strength/25000 * 0.25)</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">damage - (quality * strength/25000 * 0.25)</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">damage - (quality * strength/10000 * 0.25)</Expression>   <!-- Wrenching -->
 		<Expression damagetype=""18"">damage - (quality * strength/25000 * 0.25)</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">damage - (quality * 0.25)</Expression>                    <!-- Necrotic -->   
 		<Expression damagetype=""20"">damage - (quality * strength/10000 * 0.25)</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">damage - (quality * 0.25)</Expression>                    <!-- Eldritch -->   
		<Expression damagetype=""22"">damage - (quality * 0.25)</Expression>                    <!-- Arcane -->   
 	</DissipateExpressions>  
 	<DissipateExpressionsPain>
        <Expression damagetype=""0"">pain - (quality * strength/25000 * 0.25)</Expression>    <!-- Slashing -->
 		<Expression damagetype=""1"">pain - (quality * strength/25000 * 0.25)</Expression>    <!-- Chopping -->  
 		<Expression damagetype=""2"">pain - (quality * strength/10000 * 0.25)</Expression>    <!-- Crushing -->  
 		<Expression damagetype=""3"">pain - (quality * strength/25000 * 0.25)</Expression>    <!-- Piercing -->  
 		<Expression damagetype=""4"">pain - (quality * strength/25000 * 0.25)</Expression>    <!-- Ballistic -->  
 		<Expression damagetype=""5"">pain - (quality * 0.25)</Expression>    			        <!-- Burning -->
 		<Expression damagetype=""6"">pain - (quality * 0.25)</Expression>                     <!-- Freezing -->
 		<Expression damagetype=""7"">pain - (quality * 0.25)</Expression>                     <!-- Chemical -->
 		<Expression damagetype=""8"">pain - (quality * strength/10000 * 0.25)</Expression>    <!-- Shockwave -->
 		<Expression damagetype=""9"">pain - (quality * strength/25000 * 0.25)</Expression>    <!-- Bite -->
 		<Expression damagetype=""10"">pain - (quality * strength/25000 * 0.25)</Expression>   <!-- Claw -->
 		<Expression damagetype=""11"">pain - (quality * 0.25)</Expression>                    <!-- Electrical -->
 		<Expression damagetype=""12"">pain - (quality * 0.25)</Expression>                    <!-- Hypoxia -->
 		<Expression damagetype=""13"">pain - (quality * 0.25)</Expression>                    <!-- Cellular -->
		<Expression damagetype=""14"">pain - (quality * strength/10000 * 0.25)</Expression>   <!-- Sonic -->
 		<Expression damagetype=""15"">pain - (quality * strength/25000 * 0.25)</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">pain - (quality * strength/25000 * 0.25)</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">pain - (quality * strength/10000 * 0.25)</Expression>   <!-- Wrenching -->
 		<Expression damagetype=""18"">pain - (quality * strength/25000 * 0.25)</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">pain - (quality * 0.25)</Expression>                    <!-- Necrotic -->   
 		<Expression damagetype=""20"">pain - (quality * strength/10000 * 0.25)</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">pain - (quality * 0.25)</Expression>                    <!-- Eldritch -->   
		<Expression damagetype=""22"">pain - (quality * 0.25)</Expression>                    <!-- Arcane -->   
 	</DissipateExpressionsPain>  
 	<DissipateExpressionsStun>
        <Expression damagetype=""0"">stun - (quality * strength/25000 * 0.25)</Expression>    <!-- Slashing -->
 		<Expression damagetype=""1"">stun - (quality * strength/25000 * 0.25)</Expression>    <!-- Chopping -->  
 		<Expression damagetype=""2"">stun - (quality * strength/10000 * 0.25)</Expression>    <!-- Crushing -->  
 		<Expression damagetype=""3"">stun - (quality * strength/25000 * 0.25)</Expression>    <!-- Piercing -->  
 		<Expression damagetype=""4"">stun - (quality * strength/25000 * 0.25)</Expression>    <!-- Ballistic -->  
 		<Expression damagetype=""5"">stun - (quality * 0.25)</Expression>    			        <!-- Burning -->
 		<Expression damagetype=""6"">stun - (quality * 0.25)</Expression>                     <!-- Freezing -->
 		<Expression damagetype=""7"">stun - (quality * 0.25)</Expression>                     <!-- Chemical -->
 		<Expression damagetype=""8"">stun - (quality * strength/10000 * 0.25)</Expression>    <!-- Shockwave -->
 		<Expression damagetype=""9"">stun - (quality * strength/25000 * 0.25)</Expression>    <!-- Bite -->
 		<Expression damagetype=""10"">stun - (quality * strength/25000 * 0.25)</Expression>   <!-- Claw -->
 		<Expression damagetype=""11"">stun - (quality * 0.25)</Expression>                    <!-- Electrical -->
 		<Expression damagetype=""12"">stun - (quality * 0.25)</Expression>                    <!-- Hypoxia -->
 		<Expression damagetype=""13"">stun - (quality * 0.25)</Expression>                    <!-- Cellular -->
		<Expression damagetype=""14"">stun - (quality * strength/10000 * 0.25)</Expression>   <!-- Sonic -->
 		<Expression damagetype=""15"">stun - (quality * strength/25000 * 0.25)</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">stun - (quality * strength/25000 * 0.25)</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">stun - (quality * strength/10000 * 0.25)</Expression>   <!-- Wrenching -->
 		<Expression damagetype=""18"">stun - (quality * strength/25000 * 0.25)</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">stun - (quality * 0.25)</Expression>                    <!-- Necrotic -->   
 		<Expression damagetype=""20"">stun - (quality * strength/10000 * 0.25)</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">stun - (quality * 0.25)</Expression>                    <!-- Eldritch -->   
		<Expression damagetype=""22"">stun - (quality * 0.25)</Expression>                    <!-- Arcane -->   
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
	 	<Expression damagetype=""0"">damage*0.2</Expression>    <!-- Slashing -->
 		<Expression damagetype=""1"">damage*0.2</Expression>    <!-- Chopping -->  
 		<Expression damagetype=""2"">damage*0.2</Expression>    <!-- Crushing -->  
 		<Expression damagetype=""3"">damage*0.2</Expression>    <!-- Piercing -->  
 		<Expression damagetype=""4"">damage*0.2</Expression>    <!-- Ballistic -->  
 		<Expression damagetype=""5"">damage*0.2</Expression>    <!-- Burning -->
 		<Expression damagetype=""6"">damage*0.2</Expression>    <!-- Freezing -->
 		<Expression damagetype=""7"">damage*0.2</Expression>    <!-- Chemical -->
 		<Expression damagetype=""8"">damage*0.2</Expression>    <!-- Shockwave -->
 		<Expression damagetype=""9"">damage*0.2</Expression>    <!-- Bite -->
 		<Expression damagetype=""10"">damage*0.2</Expression>   <!-- Claw -->
 		<Expression damagetype=""11"">damage*0.2</Expression>   <!-- Electrical -->
 		<Expression damagetype=""12"">0</Expression>        <!-- Hypoxia -->
 		<Expression damagetype=""13"">0</Expression>        <!-- Cellular -->
		<Expression damagetype=""14"">damage*0.2</Expression>   <!-- Sonic -->
 		<Expression damagetype=""15"">damage*0.2</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">damage*0.2</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">damage*0.2</Expression>   <!-- Wrenching -->
 		<Expression damagetype=""18"">damage*0.2</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">damage*0.2</Expression>   <!-- Necrotic -->   
 		<Expression damagetype=""20"">damage*0.2</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">damage*0.2</Expression>   <!-- Eldritch -->   
		<Expression damagetype=""22"">damage*0.2</Expression>   <!-- Arcane -->   
 	</AbsorbExpressions>  
 	<AbsorbExpressionsPain>
        <Expression damagetype=""0"">pain*0.2</Expression>    <!-- Slashing -->
 		<Expression damagetype=""1"">pain*0.2</Expression>    <!-- Chopping -->  
 		<Expression damagetype=""2"">pain*0.2</Expression>    <!-- Crushing -->  
 		<Expression damagetype=""3"">pain*0.2</Expression>    <!-- Piercing -->  
 		<Expression damagetype=""4"">pain*0.2</Expression>    <!-- Ballistic -->  
 		<Expression damagetype=""5"">pain*0.2</Expression>    <!-- Burning -->
 		<Expression damagetype=""6"">pain*0.2</Expression>    <!-- Freezing -->
 		<Expression damagetype=""7"">pain*0.2</Expression>    <!-- Chemical -->
 		<Expression damagetype=""8"">pain*0.2</Expression>    <!-- Shockwave -->
 		<Expression damagetype=""9"">pain*0.2</Expression>    <!-- Bite -->
 		<Expression damagetype=""10"">pain*0.2</Expression>   <!-- Claw -->
 		<Expression damagetype=""11"">pain*0.2</Expression>   <!-- Electrical -->
 		<Expression damagetype=""12"">0</Expression>        <!-- Hypoxia -->
 		<Expression damagetype=""13"">0</Expression>        <!-- Cellular -->
		<Expression damagetype=""14"">pain*0.2</Expression>   <!-- Sonic -->
 		<Expression damagetype=""15"">pain*0.2</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">pain*0.2</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">pain*0.2</Expression>   <!-- Wrenching -->
 		<Expression damagetype=""18"">pain*0.2</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">pain*0.2</Expression>   <!-- Necrotic -->   
 		<Expression damagetype=""20"">pain*0.2</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">pain*0.2</Expression>   <!-- Eldritch -->   
		<Expression damagetype=""22"">pain*0.2</Expression>   <!-- Arcane -->   
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
		context.ArmourTypes.Add(shieldArmour);
		context.SaveChanges();

		var skill = context.TraitDefinitions.First(x => x.Name == "Blocking" || x.Name == "Block");

		var shield = new ShieldType
		{
			Name = "Improvised",
			EffectiveArmourType = shieldArmour,
			BlockBonus = -1.0,
			BlockTrait = skill,
			StaminaPerBlock = 10.0
		};
		context.ShieldTypes.Add(shield);
		context.SaveChanges();
		CreateShieldComponent(shield);

		shield = new ShieldType
		{
			Name = "Buckler",
			EffectiveArmourType = shieldArmour,
			BlockBonus = 1.0,
			BlockTrait = skill,
			StaminaPerBlock = 5.0
		};
		context.ShieldTypes.Add(shield);
		context.SaveChanges();
		CreateShieldComponent(shield);

		shield = new ShieldType
		{
			Name = "Kite",
			EffectiveArmourType = shieldArmour,
			BlockBonus = 0.0,
			BlockTrait = skill,
			StaminaPerBlock = 8.0
		};
		context.ShieldTypes.Add(shield);
		context.SaveChanges();
		CreateShieldComponent(shield);

		shield = new ShieldType
		{
			Name = "Round",
			EffectiveArmourType = shieldArmour,
			BlockBonus = 0.0,
			BlockTrait = skill,
			StaminaPerBlock = 8.0
		};
		context.ShieldTypes.Add(shield);
		context.SaveChanges();
		CreateShieldComponent(shield);

		shield = new ShieldType
		{
			Name = "Heater",
			EffectiveArmourType = shieldArmour,
			BlockBonus = 0.0,
			BlockTrait = skill,
			StaminaPerBlock = 8.0
		};
		context.ShieldTypes.Add(shield);
		context.SaveChanges();
		CreateShieldComponent(shield);

		shield = new ShieldType
		{
			Name = "Tower",
			EffectiveArmourType = shieldArmour,
			BlockBonus = 1.0,
			BlockTrait = skill,
			StaminaPerBlock = 10.0
		};
		context.ShieldTypes.Add(shield);
		context.SaveChanges();
		CreateShieldComponent(shield);
	}

	private void SeedCombatStrategies(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> questionAnswers)
	{
		var humanProg = context.FutureProgs.First(x => x.FunctionName == "IsHumanoid");

		void SeedCombatStrategy(string name, string description, double weaponUse, double naturalUse,
			double auxilliaryUse, bool preferFavourite, bool preferArmed, bool preferNonContact, bool preferShields,
			bool attackCritical, bool attackUnarmed, bool skirmish, bool fallbackToUnarmed,
			bool automaticallyMoveToTarget, bool manualPositionManagement, bool moveToMeleeIfCannotRange,
			PursuitMode pursuit, CombatStrategyMode melee, CombatStrategyMode ranged,
			AutomaticInventorySettings inventory, AutomaticMovementSettings movement,
			AutomaticRangedSettings rangesettings, AttackHandednessOptions setup, GrappleResponse grapple,
			double requiredMinimumAim, double minmumStamina, DefenseType defaultDefenseType,
			IEnumerable<MeleeAttackOrderPreference> order,
			CombatMoveIntentions forbiddenIntentions = CombatMoveIntentions.Savage | CombatMoveIntentions.Cruel,
			CombatMoveIntentions preferredIntentions = CombatMoveIntentions.None)
		{
			var strategy = new CharacterCombatSetting
			{
				Name = name,
				Description = description,
				GlobalTemplate = true,
				AvailabilityProg = humanProg,
				WeaponUsePercentage = weaponUse,
				MagicUsePercentage = 0.0,
				PsychicUsePercentage = 0.0,
				NaturalWeaponPercentage = naturalUse,
				AuxiliaryPercentage = auxilliaryUse,
				PreferFavouriteWeapon = preferFavourite,
				PreferToFightArmed = preferArmed,
				PreferNonContactClinchBreaking = preferNonContact,
				PreferShieldUse = preferShields,
				ClassificationsAllowed = "1 2 3 4 5 7",
				RequiredIntentions = 0,
				ForbiddenIntentions = (long)forbiddenIntentions,
				PreferredIntentions = (long)preferredIntentions,
				AttackCriticallyInjured = attackCritical,
				AttackUnarmedOrHelpless = attackUnarmed,
				SkirmishToOtherLocations = skirmish,
				PursuitMode = (int)pursuit,
				DefaultPreferredDefenseType = (int)defaultDefenseType,
				PreferredMeleeMode = (int)melee,
				PreferredRangedMode = (int)ranged,
				FallbackToUnarmedIfNoWeapon = fallbackToUnarmed,
				AutomaticallyMoveTowardsTarget = automaticallyMoveToTarget,
				InventoryManagement = (int)inventory,
				MovementManagement = (int)movement,
				RangedManagement = (int)rangesettings,
				ManualPositionManagement = manualPositionManagement,
				MinimumStaminaToAttack = minmumStamina,
				MoveToMeleeIfCannotEngageInRangedCombat = moveToMeleeIfCannotRange,
				PreferredWeaponSetup = (int)setup,
				RequiredMinimumAim = requiredMinimumAim,
				MeleeAttackOrderPreference = order.Select(x => ((int)x).ToString()).ListToCommaSeparatedValues(" "),
				GrappleResponse = (int)grapple
			};
			context.CharacterCombatSettings.Add(strategy);
			context.SaveChanges();
		}

		var defaultOrder = new List<MeleeAttackOrderPreference>
		{
			MeleeAttackOrderPreference.Weapon,
			MeleeAttackOrderPreference.Implant,
			MeleeAttackOrderPreference.Prosthetic,
			MeleeAttackOrderPreference.Magic,
			MeleeAttackOrderPreference.Psychic
		};

		SeedCombatStrategy("Melee", "Fight with a weapon, move to melee, not afraid of using unarmed if disarmed", 1.0,
			0.0, 0.0, false, true, true, true, true, true, false, true, false, false, true,
			PursuitMode.OnlyAttemptToStop, CombatStrategyMode.StandardMelee, CombatStrategyMode.FullAdvance,
			AutomaticInventorySettings.AutomaticButDontDiscard, AutomaticMovementSettings.SeekCoverOnly,
			AutomaticRangedSettings.ContinueFiringOnly, AttackHandednessOptions.Any, GrappleResponse.Avoidance, 0.5,
			5.0, DefenseType.None, defaultOrder);
		SeedCombatStrategy("Manual",
			"A fighting style that is FULLY MANUAL. This means that all inventory management, ranged combat and combat movement is fully manual and controlled by the player. While this gives you the greatest degree of control, it is assuredly slower than using the automatic systems. Use with caution - only for advanced players.",
			1.0, 0.0, 0.0, false, true, true, true, true, true, false, true, false, true, false,
			PursuitMode.OnlyAttemptToStop, CombatStrategyMode.StandardMelee, CombatStrategyMode.FullAdvance,
			AutomaticInventorySettings.FullyManual, AutomaticMovementSettings.FullyManual,
			AutomaticRangedSettings.FullyManual, AttackHandednessOptions.Any, GrappleResponse.Avoidance, 0.5, 5.0,
			DefenseType.None, defaultOrder);
		SeedCombatStrategy("Brawler", "Fight unarmed in melee using a wide variety of moves", 0.0, 1.0, 0.0, false,
			false, false, false, true, true, false, true, false, false, true, PursuitMode.OnlyAttemptToStop,
			CombatStrategyMode.StandardMelee, CombatStrategyMode.FullAdvance,
			AutomaticInventorySettings.AutomaticButDontDiscard, AutomaticMovementSettings.SeekCoverOnly,
			AutomaticRangedSettings.ContinueFiringOnly, AttackHandednessOptions.Any, GrappleResponse.Avoidance, 0.5,
			5.0, DefenseType.Dodge, defaultOrder);
		SeedCombatStrategy("Shielder",
			"Fight with a weapon and shield, move to melee, not afraid of using unarmed if disarmed", 1.0, 0.0, 0.0,
			false, true, true, true, true, true, false, true, false, false, true, PursuitMode.OnlyAttemptToStop,
			CombatStrategyMode.StandardMelee, CombatStrategyMode.FullAdvance,
			AutomaticInventorySettings.AutomaticButDontDiscard, AutomaticMovementSettings.SeekCoverOnly,
			AutomaticRangedSettings.ContinueFiringOnly, AttackHandednessOptions.SwordAndBoardOnly,
			GrappleResponse.Avoidance, 0.5, 10.0, DefenseType.Block, defaultOrder);
		SeedCombatStrategy("Zweihander",
			"Fight with a 2-hand weapon, move to melee, not afraid of using unarmed if disarmed", 1.0, 0.0, 0.0, false,
			true, true, false, true, true, false, true, false, false, true, PursuitMode.OnlyAttemptToStop,
			CombatStrategyMode.StandardMelee, CombatStrategyMode.FullAdvance,
			AutomaticInventorySettings.AutomaticButDontDiscard, AutomaticMovementSettings.SeekCoverOnly,
			AutomaticRangedSettings.ContinueFiringOnly, AttackHandednessOptions.TwoHandedOnly,
			GrappleResponse.Avoidance, 0.5, 0.0, DefenseType.Dodge, defaultOrder);
		SeedCombatStrategy("Clincher", "Fight with a weapon, move into clinch, not afraid of using unarmed if disarmed",
			1.0, 0.0, 0.0, false, true, true, true, true, true, false, true, false, false, true,
			PursuitMode.OnlyAttemptToStop, CombatStrategyMode.Clinch, CombatStrategyMode.FullAdvance,
			AutomaticInventorySettings.AutomaticButDontDiscard, AutomaticMovementSettings.SeekCoverOnly,
			AutomaticRangedSettings.ContinueFiringOnly, AttackHandednessOptions.Any, GrappleResponse.Avoidance, 0.5,
			5.0, DefenseType.None, defaultOrder);
		SeedCombatStrategy("Warder",
			"Fight with a weapon, move to melee, ward, not afraid of using unarmed if disarmed", 1.0, 0.0, 0.0, false,
			true, true, true, true, true, false, true, false, false, true, PursuitMode.OnlyAttemptToStop,
			CombatStrategyMode.Ward, CombatStrategyMode.FullAdvance, AutomaticInventorySettings.AutomaticButDontDiscard,
			AutomaticMovementSettings.SeekCoverOnly, AutomaticRangedSettings.ContinueFiringOnly,
			AttackHandednessOptions.Any, GrappleResponse.Avoidance, 0.5, 10.0, DefenseType.None, defaultOrder);
		SeedCombatStrategy("Defender", "Try to stay out of fights and full defend if you get into them", 1.0, 0.0, 0.0,
			false, true, true, true, true, true, false, true, false, false, true, PursuitMode.NeverPursue,
			CombatStrategyMode.FullDefense, CombatStrategyMode.FullCover,
			AutomaticInventorySettings.AutomaticButDontDiscard, AutomaticMovementSettings.SeekCoverOnly,
			AutomaticRangedSettings.ContinueFiringOnly, AttackHandednessOptions.Any, GrappleResponse.Avoidance, 0.5,
			20.0, DefenseType.None, defaultOrder);
		SeedCombatStrategy("Pistolier",
			"Try to fight at range, but keep using a pistol in melee if you get engaged there", 1.0, 0.0, 0.0, false,
			true, true, true, true, true, false, true, false, false, true, PursuitMode.NeverPursue,
			CombatStrategyMode.MeleeShooter, CombatStrategyMode.StandardRange,
			AutomaticInventorySettings.AutomaticButDontDiscard, AutomaticMovementSettings.SeekCoverOnly,
			AutomaticRangedSettings.FullyAutomatic, AttackHandednessOptions.Any, GrappleResponse.Avoidance, 0.2, 5.0,
			DefenseType.Dodge, defaultOrder);
		SeedCombatStrategy("Musketeer", "Try to fight at range, seek no cover, and ward if you do get into melee", 1.0,
			0.0, 0.0, false, true, true, true, true, true, false, true, false, false, true, PursuitMode.NeverPursue,
			CombatStrategyMode.Ward, CombatStrategyMode.FireNoCover, AutomaticInventorySettings.AutomaticButDontDiscard,
			AutomaticMovementSettings.SeekCoverOnly, AutomaticRangedSettings.FullyAutomatic,
			AttackHandednessOptions.Any, GrappleResponse.Avoidance, 0.7, 5.0, DefenseType.None, defaultOrder);
		SeedCombatStrategy("Infantryman", "Find cover if attacked suddely, return fire, but fight in melee if engaged.",
			1.0, 0.0, 0.0, false, true, true, true, true, true, false, true, false, false, true,
			PursuitMode.NeverPursue, CombatStrategyMode.StandardMelee, CombatStrategyMode.StandardRange,
			AutomaticInventorySettings.AutomaticButDontDiscard, AutomaticMovementSettings.SeekCoverOnly,
			AutomaticRangedSettings.FullyAutomatic, AttackHandednessOptions.Any, GrappleResponse.Avoidance, 0.7, 5.0,
			DefenseType.None, defaultOrder);
		SeedCombatStrategy("Skirmisher", "Stay out of melee and fire your weapon. Prioritise mobility over safety.",
			1.0, 0.0, 0.0, false, true, true, true, true, true, false, true, false, false, true,
			PursuitMode.NeverPursue, CombatStrategyMode.FullSkirmish, CombatStrategyMode.FireNoCover,
			AutomaticInventorySettings.AutomaticButDontDiscard, AutomaticMovementSettings.SeekCoverOnly,
			AutomaticRangedSettings.FullyAutomatic, AttackHandednessOptions.Any, GrappleResponse.Avoidance, 0.5, 5.0,
			DefenseType.None, defaultOrder);
		SeedCombatStrategy("Marksman", "Get into cover, aim well, and make your shots count.", 1.0, 0.0, 0.0, false,
			true, true, true, true, true, false, true, false, false, true, PursuitMode.NeverPursue,
			CombatStrategyMode.FullSkirmish, CombatStrategyMode.StandardRange,
			AutomaticInventorySettings.AutomaticButDontDiscard, AutomaticMovementSettings.SeekCoverOnly,
			AutomaticRangedSettings.FullyAutomatic, AttackHandednessOptions.Any, GrappleResponse.Avoidance, 1.0, 5.0,
			DefenseType.None, defaultOrder);
		SeedCombatStrategy("Pitfighter", "Fight unarmed in melee with no holds barred and nothing off the table", 0.0,
			1.0, 0.0, false, false, false, false, true, true, false, true, false, false, true,
			PursuitMode.OnlyAttemptToStop, CombatStrategyMode.StandardMelee, CombatStrategyMode.FullAdvance,
			AutomaticInventorySettings.AutomaticButDontDiscard, AutomaticMovementSettings.SeekCoverOnly,
			AutomaticRangedSettings.ContinueFiringOnly, AttackHandednessOptions.Any, GrappleResponse.Avoidance, 0.5,
			5.0, DefenseType.Dodge, defaultOrder, CombatMoveIntentions.None);
		SeedCombatStrategy("Swarmer", "Fight unarmed in melee and try to get into clinches", 0.0, 1.0, 0.0, false,
			false, false, false, true, true, false, true, false, false, true, PursuitMode.OnlyAttemptToStop,
			CombatStrategyMode.Clinch, CombatStrategyMode.FullAdvance,
			AutomaticInventorySettings.AutomaticButDontDiscard, AutomaticMovementSettings.SeekCoverOnly,
			AutomaticRangedSettings.ContinueFiringOnly, AttackHandednessOptions.Any, GrappleResponse.Avoidance, 0.5,
			5.0, DefenseType.Dodge, defaultOrder);
		SeedCombatStrategy("Outboxer", "Fight unarmed in melee and try to keep range and use counter attacks", 0.0, 1.0,
			0.0, false, false, false, false, true, true, false, true, false, false, true, PursuitMode.OnlyAttemptToStop,
			CombatStrategyMode.Ward, CombatStrategyMode.FullAdvance, AutomaticInventorySettings.AutomaticButDontDiscard,
			AutomaticMovementSettings.SeekCoverOnly, AutomaticRangedSettings.ContinueFiringOnly,
			AttackHandednessOptions.Any, GrappleResponse.Avoidance, 0.5, 5.0, DefenseType.Dodge, defaultOrder);
		SeedCombatStrategy("Grappler", "Fight unarmed in melee and try to grapple your opponent into control", 0.0, 1.0,
			0.0, false, false, false, false, true, true, false, true, false, false, true, PursuitMode.OnlyAttemptToStop,
			CombatStrategyMode.GrappleForControl, CombatStrategyMode.FullAdvance,
			AutomaticInventorySettings.AutomaticButDontDiscard, AutomaticMovementSettings.SeekCoverOnly,
			AutomaticRangedSettings.ContinueFiringOnly, AttackHandednessOptions.Any, GrappleResponse.Avoidance, 0.5,
			5.0, DefenseType.Dodge, defaultOrder);
		SeedCombatStrategy("Bonebreaker",
			"Fight unarmed in melee and try to grapple your opponent and break their limbs", 0.0, 1.0, 0.0, false,
			false, false, false, true, true, false, true, false, false, true, PursuitMode.OnlyAttemptToStop,
			CombatStrategyMode.GrappleForIncapacitation, CombatStrategyMode.FullAdvance,
			AutomaticInventorySettings.AutomaticButDontDiscard, AutomaticMovementSettings.SeekCoverOnly,
			AutomaticRangedSettings.ContinueFiringOnly, AttackHandednessOptions.Any, GrappleResponse.Avoidance, 0.5,
			5.0, DefenseType.Dodge, defaultOrder);
		SeedCombatStrategy("Strangler", "Fight unarmed in melee and try to grapple, strangle and kill them", 0.0, 1.0,
			0.0, false, false, false, false, true, true, false, true, false, false, true, PursuitMode.OnlyAttemptToStop,
			CombatStrategyMode.GrappleForKill, CombatStrategyMode.FullAdvance,
			AutomaticInventorySettings.AutomaticButDontDiscard, AutomaticMovementSettings.SeekCoverOnly,
			AutomaticRangedSettings.ContinueFiringOnly, AttackHandednessOptions.Any, GrappleResponse.Avoidance, 0.5,
			5.0, DefenseType.Dodge, defaultOrder);
	}

	private void SeedDataUnarmed(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> questionAnswers,
		IReadOnlyDictionary<string, TraitDefinition> skills)
	{
		var human = context.Races.First(x => x.Name == "Humanoid");

		var handshape = context.BodypartShapes.First(x => x.Name == "Hand");
		var footshape = context.BodypartShapes.First(x => x.Name == "Foot");
		var elbowshape = context.BodypartShapes.First(x => x.Name == "Elbow");
		var kneeshape = context.BodypartShapes.First(x => x.Name == "Knee");
		var shouldershape = context.BodypartShapes.First(x => x.Name == "Shoulder");
		var foreheadshape = context.BodypartShapes.First(x => x.Name == "Forehead");
		var mouthshape = context.BodypartShapes.First(x => x.Name == "Mouth");

		var attributes = context.TraitDefinitions.Where(x => x.Type == 1 || x.Type == 3)
			.ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
		var strength =
			attributes.GetValueOrDefault("Strength") ??
			attributes.GetValueOrDefault("Physique") ??
			attributes["Body"];
		var dex =
			attributes.GetValueOrDefault("Agility") ??
			attributes.GetValueOrDefault("Dexterity") ??
			attributes.GetValueOrDefault("Agility") ??
			attributes.GetValueOrDefault("Speed") ??
			attributes["Body"];
		context.SaveChanges();

		context.StaticConfigurations.Add(new StaticConfiguration
		{
			SettingName = "StaggeringBlowExpressionAttacker",
			Definition = $"(2*{strength.Alias}:{strength.Id})+(damage/6)+(stun/12)"
		});
		context.StaticConfigurations.Add(new StaticConfiguration
		{
			SettingName = "StaggeringBlowExpressionDefender",
			Definition = $"(2*{strength.Alias}:{strength.Id})"
		});

		context.StaticConfigurations.Add(new StaticConfiguration
		{
			SettingName = "UnbalancingBlowExpressionAttacker",
			Definition = $"((2*{dex.Alias}:{dex.Id})+(damage/6)+(stun/12)) * ((3 + degree) / 3)"
		});
		context.StaticConfigurations.Add(new StaticConfiguration
		{
			SettingName = "UnbalancingBlowExpressionDefender",
			Definition = $"(2*{dex.Alias}:{dex.Id})* ((3 + degree - 2 * limbs)/3)"
		});

		context.StaticConfigurations.Add(new StaticConfiguration
		{
			SettingName = "BreakoutAttackerStrengthExpression",
			Definition = $"((2*{strength.Alias}:{strength.Id})-(3*limbs)"
		});
		context.StaticConfigurations.Add(new StaticConfiguration
		{
			SettingName = "BreakoutDefenderStrengthExpression",
			Definition = $"(2*{strength.Alias}:{strength.Id})"
		});

		context.StaticConfigurations.Add(new StaticConfiguration
		{
			SettingName = "DownedMeleeStaggerEffectLength",
			Definition = "12000"
		});

		var attackAddendum = "";
		switch (questionAnswers["messagestyle"].ToLowerInvariant())
		{
			case "sentences":
			case "sparse":
				attackAddendum = ".";
				break;
		}

		var randomPortion = "";
		switch (questionAnswers["random"].ToLowerInvariant())
		{
			case "static":
				randomPortion = "";
				break;
			case "partial":
				randomPortion = " * rand(0.7,1.0)";
				break;
			case "random":
				randomPortion = " * rand(0.2,1.0)";
				break;
		}

		var terribleDamage = new TraitExpression
		{
			Name = "Unarmed Damage - Terrible",
			Expression = $"0.33333 * (str:{strength.Id} + (2 * quality)) * sqrt(degree+1){randomPortion}"
		};
		context.TraitExpressions.Add(terribleDamage);
		context.SaveChanges();
		var badDamage = new TraitExpression
		{
			Name = "Unarmed Damage - Bad",
			Expression = $"0.66666 * (str:{strength.Id} + (2 * quality)) * sqrt(degree+1){randomPortion}"
		};
		context.TraitExpressions.Add(badDamage);
		context.SaveChanges();
		var normalDamage = new TraitExpression
		{
			Name = "Unarmed Damage - Normal",
			Expression = $"1.0 * (str:{strength.Id} + (2 * quality)) * sqrt(degree+1){randomPortion}"
		};
		context.TraitExpressions.Add(normalDamage);
		context.SaveChanges();
		var goodDamage = new TraitExpression
		{
			Name = "Unarmed Damage - Good",
			Expression = $"1.25 * (str:{strength.Id} + (2 * quality)) * sqrt(degree+1){randomPortion}"
		};
		context.TraitExpressions.Add(goodDamage);
		context.SaveChanges();
		var greatDamage = new TraitExpression
		{
			Name = "Unarmed Damage - Great",
			Expression = $"1.5 * (str:{strength.Id} + (2 * quality)) * sqrt(degree+1){randomPortion}"
		};
		context.TraitExpressions.Add(greatDamage);
		context.SaveChanges();

		void AddAttack(string name, BuiltInCombatMoveType moveType, MeleeWeaponVerb verb, Difficulty attacker,
			Difficulty dodge, Difficulty parry, Difficulty block, Alignment alignment, Orientation orientation,
			double stamina, double relativeSpeed, BodypartShape shape, TraitExpression damage, string attackMessage,
			DamageType damageType = DamageType.Crushing, double weighting = 100,
			CombatMoveIntentions intentions = CombatMoveIntentions.Attack | CombatMoveIntentions.Wound,
			string additionalInfo = null)
		{
			var attack = new WeaponAttack
			{
				Verb = (int)verb,
				BaseAttackerDifficulty = (int)attacker,
				BaseBlockDifficulty = (int)block,
				BaseDodgeDifficulty = (int)dodge,
				BaseParryDifficulty = (int)parry,
				MoveType = (int)moveType,
				RecoveryDifficultySuccess = (int)Difficulty.Easy,
				RecoveryDifficultyFailure = (int)Difficulty.Hard,
				Intentions = (long)intentions,
				Weighting = weighting,
				ExertionLevel = (int)ExertionLevel.Heavy,
				DamageType = (int)damageType,
				DamageExpression = damage,
				StunExpression = damage,
				PainExpression = damage,
				BodypartShapeId = shape.Id,
				StaminaCost = stamina,
				BaseDelay = relativeSpeed,
				Name = name,
				Orientation = (int)orientation,
				Alignment = (int)alignment,
				HandednessOptions = 0,
				AdditionalInfo = additionalInfo
			};
			context.WeaponAttacks.Add(attack);
			context.SaveChanges();

			foreach (var bodypart in context.BodypartProtos.Where(x => x.BodypartShapeId == shape.Id))
				context.RacesWeaponAttacks.Add(new RacesWeaponAttacks
				{
					Bodypart = bodypart,
					Race = human,
					WeaponAttack = attack,
					Quality = (int)ItemQuality.Standard
				});

			context.SaveChanges();

			var message = new CombatMessage
			{
				Type = (int)moveType,
				Message = attackMessage,
				Priority = 50,
				Verb = (int)verb,
				Chance = 1.0,
				FailureMessage = attackMessage
			};
			message.CombatMessagesWeaponAttacks.Add(new CombatMessagesWeaponAttacks
				{ CombatMessage = message, WeaponAttack = attack });
			context.CombatMessages.Add(message);
			context.SaveChanges();
		}

		AddAttack("Jab", BuiltInCombatMoveType.NaturalWeaponAttack, MeleeWeaponVerb.Jab, Difficulty.Normal,
			Difficulty.Hard, Difficulty.Hard, Difficulty.Normal, Alignment.FrontRight, Orientation.High, 3.0, 0.75,
			handshape, badDamage, $"@ throw|throws a quick @hand-hand jab at $1{attackAddendum}",
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Fast);
		AddAttack("High Jab", BuiltInCombatMoveType.NaturalWeaponAttack, MeleeWeaponVerb.Jab, Difficulty.Normal,
			Difficulty.Hard, Difficulty.Hard, Difficulty.Normal, Alignment.FrontRight, Orientation.Highest, 3.0, 0.8,
			handshape, badDamage, $"@ throw|throws a high @hand-hand jab at $1{attackAddendum}",
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Fast |
			            CombatMoveIntentions.Stun);
		AddAttack("Low Jab", BuiltInCombatMoveType.NaturalWeaponAttack, MeleeWeaponVerb.Jab, Difficulty.Normal,
			Difficulty.Hard, Difficulty.Hard, Difficulty.Normal, Alignment.FrontRight, Orientation.Centre, 3.0, 0.8,
			handshape, badDamage, $"@ throw|throws a low @hand-hand jab at $1{attackAddendum}",
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Fast);

		AddAttack("Hook", BuiltInCombatMoveType.NaturalWeaponAttack, MeleeWeaponVerb.Swing, Difficulty.Normal,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight, Orientation.High, 5.0, 1.0,
			handshape, normalDamage, $"@ throw|throws a @hand-hand hook punch at $1{attackAddendum}");
		AddAttack("High Hook", BuiltInCombatMoveType.NaturalWeaponAttack, MeleeWeaponVerb.Swing, Difficulty.Normal,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight, Orientation.Highest, 5.0,
			1.05, handshape, normalDamage, $"@ throw|throws a high @hand-hand hook punch at $1{attackAddendum}",
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Stun);
		AddAttack("Low Hook", BuiltInCombatMoveType.NaturalWeaponAttack, MeleeWeaponVerb.Swing, Difficulty.Normal,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight, Orientation.Centre, 5.0, 1.05,
			handshape, normalDamage, $"@ throw|throws a low @hand-hand hook punch at $1{attackAddendum}");

		AddAttack("Cross", BuiltInCombatMoveType.NaturalWeaponAttack, MeleeWeaponVerb.Punch, Difficulty.Normal,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.Front, Orientation.High, 5.0, 1.0,
			handshape, normalDamage, $"@ throw|throws a @hand-hand cross punch at $1{attackAddendum}");
		AddAttack("High Cross", BuiltInCombatMoveType.NaturalWeaponAttack, MeleeWeaponVerb.Punch, Difficulty.Normal,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.Front, Orientation.Highest, 5.0, 1.05,
			handshape, normalDamage, $"@ throw|throws a high @hand-hand cross punch at $1{attackAddendum}",
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Stun);
		AddAttack("Low Cross", BuiltInCombatMoveType.NaturalWeaponAttack, MeleeWeaponVerb.Punch, Difficulty.Normal,
			Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.Front, Orientation.Centre, 5.0, 1.05,
			handshape, normalDamage, $"@ throw|throws a low @hand-hand cross punch at $1{attackAddendum}");

		AddAttack("Haymaker", BuiltInCombatMoveType.StaggeringBlowUnarmed, MeleeWeaponVerb.Jab, Difficulty.Normal,
			Difficulty.Easy, Difficulty.Easy, Difficulty.VeryEasy, Alignment.FrontRight, Orientation.High, 6.0, 1.25,
			handshape, goodDamage, $"@ throw|throws a @hand-hand haymaker punch at $1{attackAddendum}",
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Slow,
			additionalInfo: "4");
		AddAttack("High Haymaker", BuiltInCombatMoveType.StaggeringBlowUnarmed, MeleeWeaponVerb.Jab, Difficulty.Normal,
			Difficulty.Easy, Difficulty.Easy, Difficulty.Normal, Alignment.FrontRight, Orientation.Highest, 6.0, 1.3,
			handshape, goodDamage, $"@ throw|throws a high @hand-hand haymaker punch at $1{attackAddendum}",
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Slow |
			            CombatMoveIntentions.Stun, additionalInfo: "4");
		AddAttack("Low Haymaker", BuiltInCombatMoveType.StaggeringBlowUnarmed, MeleeWeaponVerb.Jab, Difficulty.Normal,
			Difficulty.Easy, Difficulty.Easy, Difficulty.VeryEasy, Alignment.FrontRight, Orientation.Centre, 6.0, 1.3,
			handshape, goodDamage, $"@ throw|throws a low @hand-hand haymaker punch at $1{attackAddendum}",
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Slow,
			additionalInfo: "4");

		AddAttack("Uppercut", BuiltInCombatMoveType.NaturalWeaponAttack, MeleeWeaponVerb.Strike, Difficulty.Hard,
			Difficulty.Easy, Difficulty.Easy, Difficulty.VeryEasy, Alignment.Front, Orientation.High, 6.0, 1.25,
			handshape, greatDamage, $"@ throw|throws a @hand-hand uppercut at $1{attackAddendum}",
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Slow |
			            CombatMoveIntentions.Hard);
		AddAttack("High Uppercut", BuiltInCombatMoveType.NaturalWeaponAttack, MeleeWeaponVerb.Strike, Difficulty.Hard,
			Difficulty.Easy, Difficulty.Easy, Difficulty.Normal, Alignment.Front, Orientation.Highest, 6.0, 1.3,
			handshape, greatDamage, $"@ throw|throws a high @hand-hand uppercut at $1{attackAddendum}",
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Slow |
			            CombatMoveIntentions.Stun | CombatMoveIntentions.Hard);
		AddAttack("Low Uppercut", BuiltInCombatMoveType.NaturalWeaponAttack, MeleeWeaponVerb.Strike, Difficulty.Hard,
			Difficulty.Easy, Difficulty.Easy, Difficulty.VeryEasy, Alignment.Front, Orientation.Centre, 6.0, 1.3,
			handshape, greatDamage, $"@ throw|throws a low @hand-hand uppercut at $1{attackAddendum}",
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Slow |
			            CombatMoveIntentions.Hard);

		AddAttack("Check Hook", BuiltInCombatMoveType.WardFreeUnarmedAttack, MeleeWeaponVerb.Swing, Difficulty.Hard,
			Difficulty.VeryHard, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight, Orientation.High, 5.0, 1.0,
			handshape, normalDamage,
			$"@ step|steps back and throw|throws a @hand-hand check hook counter-punch at $1{attackAddendum}");
		AddAttack("High Check Hook", BuiltInCombatMoveType.WardFreeUnarmedAttack, MeleeWeaponVerb.Swing,
			Difficulty.Hard, Difficulty.VeryHard, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
			Orientation.Highest, 5.0, 1.05, handshape, normalDamage,
			$"@ step|steps back and throw|throws a high @hand-hand check hook counter-punch at $1{attackAddendum}",
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Stun);
		AddAttack("Low Check Hook", BuiltInCombatMoveType.WardFreeUnarmedAttack, MeleeWeaponVerb.Swing, Difficulty.Hard,
			Difficulty.VeryHard, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight, Orientation.Centre, 5.0,
			1.05, handshape, normalDamage,
			$"@ step|steps back and throw|throws a low @hand-hand check hook counter-punch at $1{attackAddendum}");

		AddAttack("Stepback Jab", BuiltInCombatMoveType.WardFreeUnarmedAttack, MeleeWeaponVerb.Jab, Difficulty.Easy,
			Difficulty.Hard, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight, Orientation.High, 2.0, 0.5,
			handshape, badDamage,
			$"@ step|steps back and throw|throws a quick @hand-hand jab counter-punch at $1{attackAddendum}",
			intentions: CombatMoveIntentions.Fast | CombatMoveIntentions.Attack | CombatMoveIntentions.Wound);
		AddAttack("High Stepback Jab", BuiltInCombatMoveType.WardFreeUnarmedAttack, MeleeWeaponVerb.Jab,
			Difficulty.Easy, Difficulty.Hard, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
			Orientation.Highest, 2.0, 0.5, handshape, badDamage,
			$"@ step|steps back and throw|throws a high @hand-hand jab counter-punch at $1{attackAddendum}",
			intentions: CombatMoveIntentions.Fast | CombatMoveIntentions.Attack | CombatMoveIntentions.Wound |
			            CombatMoveIntentions.Stun);
		AddAttack("Low Stepback Jab", BuiltInCombatMoveType.WardFreeUnarmedAttack, MeleeWeaponVerb.Jab, Difficulty.Easy,
			Difficulty.Hard, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight, Orientation.Centre, 2.0, 0.5,
			handshape, badDamage,
			$"@ step|steps back and throw|throws a low @hand-hand jab counter-punch at $1{attackAddendum}",
			intentions: CombatMoveIntentions.Fast | CombatMoveIntentions.Attack | CombatMoveIntentions.Wound);

		AddAttack("Stepback Roundhouse", BuiltInCombatMoveType.WardFreeUnarmedAttack, MeleeWeaponVerb.Kick,
			Difficulty.VeryHard, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard, Alignment.Right, Orientation.High,
			7.0, 1.0, footshape, greatDamage,
			$"@ step|steps back and throw|throws a @hand-leg roundhouse counter-kick at $1{attackAddendum}",
			intentions: CombatMoveIntentions.Hard | CombatMoveIntentions.Attack | CombatMoveIntentions.Wound);
		AddAttack("High Stepback Roundhouse", BuiltInCombatMoveType.WardFreeUnarmedAttack, MeleeWeaponVerb.Kick,
			Difficulty.VeryHard, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard, Alignment.Right,
			Orientation.Highest, 7.0, 1.0, footshape, greatDamage,
			$"@ step|steps back and throw|throws a high @hand-leg roundhouse counter-kick at $1{attackAddendum}",
			intentions: CombatMoveIntentions.Hard | CombatMoveIntentions.Attack | CombatMoveIntentions.Wound |
			            CombatMoveIntentions.Stun);
		AddAttack("Low Stepback Roundhouse", BuiltInCombatMoveType.WardFreeUnarmedAttack, MeleeWeaponVerb.Kick,
			Difficulty.VeryHard, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard, Alignment.Right, Orientation.Centre,
			7.0, 1.0, footshape, greatDamage,
			$"@ step|steps back and throw|throws a low @hand-leg roundhouse counter-kick at $1{attackAddendum}",
			intentions: CombatMoveIntentions.Hard | CombatMoveIntentions.Attack | CombatMoveIntentions.Wound);

		AddAttack("Stepback Snap Kick", BuiltInCombatMoveType.WardFreeUnarmedAttack, MeleeWeaponVerb.Kick,
			Difficulty.Hard, Difficulty.Hard, Difficulty.Normal, Difficulty.Easy, Alignment.Right, Orientation.High,
			7.0, 1.0, footshape, goodDamage,
			$"@ step|steps back and throw|throws a @hand-leg snap counter-kick at $1{attackAddendum}",
			intentions: CombatMoveIntentions.Hard | CombatMoveIntentions.Attack | CombatMoveIntentions.Wound);
		AddAttack("High Stepback Snap Kick", BuiltInCombatMoveType.WardFreeUnarmedAttack, MeleeWeaponVerb.Kick,
			Difficulty.Hard, Difficulty.Hard, Difficulty.Normal, Difficulty.Normal, Alignment.Right,
			Orientation.Highest, 7.0, 1.0, footshape, goodDamage,
			$"@ step|steps back and throw|throws a high  @hand-leg snap counter-kick at $1{attackAddendum}",
			intentions: CombatMoveIntentions.Hard | CombatMoveIntentions.Attack | CombatMoveIntentions.Wound |
			            CombatMoveIntentions.Stun);
		AddAttack("Low Stepback Snap Kick", BuiltInCombatMoveType.WardFreeUnarmedAttack, MeleeWeaponVerb.Kick,
			Difficulty.Hard, Difficulty.Hard, Difficulty.Normal, Difficulty.Easy, Alignment.Right, Orientation.Centre,
			7.0, 1.0, footshape, goodDamage,
			$"@ step|steps back and throw|throws a low @hand-leg snap counter-kick at $1{attackAddendum}",
			intentions: CombatMoveIntentions.Hard | CombatMoveIntentions.Attack | CombatMoveIntentions.Wound);

		AddAttack("Roundhouse", BuiltInCombatMoveType.StaggeringBlowUnarmed, MeleeWeaponVerb.Kick, Difficulty.VeryHard,
			Difficulty.Hard, Difficulty.Hard, Difficulty.Hard, Alignment.Right, Orientation.High, 7.0, 1.5, footshape,
			greatDamage, $"@ throw|throws a @hand-leg roundhouse kick at $1{attackAddendum}",
			intentions: CombatMoveIntentions.Hard | CombatMoveIntentions.Attack | CombatMoveIntentions.Wound,
			additionalInfo: "6");
		AddAttack("High Roundhouse", BuiltInCombatMoveType.StaggeringBlowUnarmed, MeleeWeaponVerb.Kick,
			Difficulty.VeryHard, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard, Alignment.Right,
			Orientation.Highest, 7.0, 1.5, footshape, greatDamage,
			$"@ throw|throws a high @hand-leg roundhouse kick at $1{attackAddendum}",
			intentions: CombatMoveIntentions.Hard | CombatMoveIntentions.Attack | CombatMoveIntentions.Wound |
			            CombatMoveIntentions.Stun, additionalInfo: "6");
		AddAttack("Low Roundhouse", BuiltInCombatMoveType.StaggeringBlowUnarmed, MeleeWeaponVerb.Kick,
			Difficulty.VeryHard, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard, Alignment.Right, Orientation.Centre,
			7.0, 1.5, footshape, greatDamage, $"@ throw|throws a low @hand-leg roundhouse kick at $1{attackAddendum}",
			intentions: CombatMoveIntentions.Hard | CombatMoveIntentions.Attack | CombatMoveIntentions.Wound,
			additionalInfo: "6");

		AddAttack("Snap Kick", BuiltInCombatMoveType.NaturalWeaponAttack, MeleeWeaponVerb.Kick, Difficulty.Hard,
			Difficulty.Hard, Difficulty.Normal, Difficulty.Easy, Alignment.Right, Orientation.High, 7.0, 1.0, footshape,
			goodDamage, $"@ throw|throws a @hand-leg snap kick at $1{attackAddendum}",
			intentions: CombatMoveIntentions.Hard | CombatMoveIntentions.Attack | CombatMoveIntentions.Wound);
		AddAttack("High Snap Kick", BuiltInCombatMoveType.NaturalWeaponAttack, MeleeWeaponVerb.Kick, Difficulty.Hard,
			Difficulty.Hard, Difficulty.Normal, Difficulty.Normal, Alignment.Right, Orientation.Highest, 7.0, 1.0,
			footshape, goodDamage, $"@ throw|throws a high  @hand-leg snap kick at $1{attackAddendum}",
			intentions: CombatMoveIntentions.Hard | CombatMoveIntentions.Attack | CombatMoveIntentions.Wound |
			            CombatMoveIntentions.Stun);
		AddAttack("Low Snap Kick", BuiltInCombatMoveType.NaturalWeaponAttack, MeleeWeaponVerb.Kick, Difficulty.Hard,
			Difficulty.Hard, Difficulty.Normal, Difficulty.Easy, Alignment.Right, Orientation.Centre, 7.0, 1.0,
			footshape, goodDamage, $"@ throw|throws a low @hand-leg snap kick at $1{attackAddendum}",
			intentions: CombatMoveIntentions.Hard | CombatMoveIntentions.Attack | CombatMoveIntentions.Wound);

		AddAttack("Sweep Kick", BuiltInCombatMoveType.UnbalancingBlowUnarmed, MeleeWeaponVerb.Kick, Difficulty.Hard,
			Difficulty.Hard, Difficulty.Hard, Difficulty.VeryHard, Alignment.Right, Orientation.High, 6.0, 1.2,
			footshape, normalDamage, $"@ throw|throws a @hand-leg snap counter-kick at $1{attackAddendum}",
			intentions: CombatMoveIntentions.Hard | CombatMoveIntentions.Attack | CombatMoveIntentions.Wound,
			additionalInfo: "7");
		AddAttack("Low Sweep Kick", BuiltInCombatMoveType.UnbalancingBlowUnarmed, MeleeWeaponVerb.Kick, Difficulty.Hard,
			Difficulty.Hard, Difficulty.Hard, Difficulty.ExtremelyHard, Alignment.Right, Orientation.High, 6.0, 1.2,
			footshape, normalDamage, $"@ throw|throws a low @hand-leg sweep kick at $1{attackAddendum}",
			intentions: CombatMoveIntentions.Hard | CombatMoveIntentions.Attack | CombatMoveIntentions.Wound,
			additionalInfo: "8");

		AddAttack("Prone Body Kick", BuiltInCombatMoveType.DownedAttackUnarmed, MeleeWeaponVerb.Kick,
			Difficulty.VeryEasy, Difficulty.Hard, Difficulty.Hard, Difficulty.VeryHard, Alignment.Front,
			Orientation.Centre, 4.0, 1.2, footshape, normalDamage,
			$"@ throw|throws a cruel @hand-leg kick at $1's prone body{attackAddendum}",
			intentions: CombatMoveIntentions.Easy | CombatMoveIntentions.Attack | CombatMoveIntentions.Wound |
			            CombatMoveIntentions.Cruel | CombatMoveIntentions.Kill, additionalInfo: "6");
		AddAttack("Prone Back Kick", BuiltInCombatMoveType.DownedAttackUnarmed, MeleeWeaponVerb.Kick,
			Difficulty.VeryEasy, Difficulty.Hard, Difficulty.Hard, Difficulty.VeryHard, Alignment.Rear,
			Orientation.Centre, 4.0, 1.2, footshape, normalDamage,
			$"@ throw|throws a cruel @hand-leg kick at $1's prone back{attackAddendum}",
			intentions: CombatMoveIntentions.Easy | CombatMoveIntentions.Attack | CombatMoveIntentions.Wound |
			            CombatMoveIntentions.Cruel | CombatMoveIntentions.Kill, additionalInfo: "6");
		AddAttack("Prone Head Kick", BuiltInCombatMoveType.DownedAttackUnarmed, MeleeWeaponVerb.Kick, Difficulty.Easy,
			Difficulty.Hard, Difficulty.Hard, Difficulty.VeryHard, Alignment.Front, Orientation.Highest, 4.0, 1.2,
			footshape, normalDamage, $"@ throw|throws a cruel @hand-leg kick at $1's head{attackAddendum}",
			intentions: CombatMoveIntentions.Easy | CombatMoveIntentions.Attack | CombatMoveIntentions.Wound |
			            CombatMoveIntentions.Cruel | CombatMoveIntentions.Kill, additionalInfo: "6");
		AddAttack("Prone Leg Kick", BuiltInCombatMoveType.DownedAttackUnarmed, MeleeWeaponVerb.Kick,
			Difficulty.ExtremelyEasy, Difficulty.Hard, Difficulty.Hard, Difficulty.VeryHard, Alignment.Front,
			Orientation.Low, 4.0, 1.2, footshape, normalDamage,
			$"@ throw|throws a cruel @hand-leg kick at $1's prone legs{attackAddendum}",
			intentions: CombatMoveIntentions.Easy | CombatMoveIntentions.Attack | CombatMoveIntentions.Wound |
			            CombatMoveIntentions.Cruel | CombatMoveIntentions.Hinder, additionalInfo: "2");
		AddAttack("Prone Arm Kick", BuiltInCombatMoveType.DownedAttackUnarmed, MeleeWeaponVerb.Kick,
			Difficulty.ExtremelyEasy, Difficulty.Hard, Difficulty.Hard, Difficulty.VeryHard, Alignment.Front,
			Orientation.Appendage, 4.0, 1.2, footshape, normalDamage,
			$"@ throw|throws a cruel @hand-leg kick at $1's prone arms{attackAddendum}",
			intentions: CombatMoveIntentions.Easy | CombatMoveIntentions.Attack | CombatMoveIntentions.Wound |
			            CombatMoveIntentions.Cruel | CombatMoveIntentions.Hinder, additionalInfo: "2");

		AddAttack("Body Punch", BuiltInCombatMoveType.ClinchUnarmedAttack, MeleeWeaponVerb.Punch, Difficulty.Easy,
			Difficulty.ExtremelyHard, Difficulty.ExtremelyHard, Difficulty.Easy, Alignment.Right, Orientation.Centre,
			3.0, 0.7, handshape, badDamage, $"@ throw|throws a @hand-hand punch at $1's side{attackAddendum}",
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Easy);
		AddAttack("Front Body Punch", BuiltInCombatMoveType.ClinchUnarmedAttack, MeleeWeaponVerb.Punch,
			Difficulty.Normal, Difficulty.ExtremelyHard, Difficulty.ExtremelyHard, Difficulty.Easy,
			Alignment.FrontRight, Orientation.Centre, 3.0, 0.7, handshape, badDamage,
			$"@ throw|throws a @hand-hand punch at $1's mid-section{attackAddendum}");
		AddAttack("Overhand Punch", BuiltInCombatMoveType.ClinchUnarmedAttack, MeleeWeaponVerb.Punch, Difficulty.Hard,
			Difficulty.Hard, Difficulty.ExtremelyHard, Difficulty.Easy, Alignment.FrontRight, Orientation.High, 5.0,
			0.7, handshape, goodDamage, $"@ throw|throws a @hand-handed drop punch at $1{attackAddendum}",
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Stun |
			            CombatMoveIntentions.Hard);
		AddAttack("Headbutt", BuiltInCombatMoveType.StaggeringBlowClinch, MeleeWeaponVerb.Strike, Difficulty.VeryHard,
			Difficulty.VeryHard, Difficulty.ExtremelyHard, Difficulty.VeryHard, Alignment.Front, Orientation.Highest,
			5.0, 1.0, foreheadshape, greatDamage,
			$"@ lunge|lunges forward and throw|throws a headbutt at $1{attackAddendum}",
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Stun |
			            CombatMoveIntentions.Hard | CombatMoveIntentions.SelfDamaging | CombatMoveIntentions.Risky |
			            CombatMoveIntentions.Savage, additionalInfo: "6");
		AddAttack("Bite", BuiltInCombatMoveType.ClinchUnarmedAttack, MeleeWeaponVerb.Bite, Difficulty.VeryHard,
			Difficulty.Normal, Difficulty.ExtremelyHard, Difficulty.VeryHard, Alignment.Front, Orientation.High, 3.0,
			1.4, mouthshape, normalDamage, $"@ lean|leans in and try|tries to bite $1{attackAddendum}",
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Stun |
			            CombatMoveIntentions.Hard | CombatMoveIntentions.Risky | CombatMoveIntentions.Savage);
		AddAttack("Elbow", BuiltInCombatMoveType.ClinchUnarmedAttack, MeleeWeaponVerb.Strike, Difficulty.Hard,
			Difficulty.Easy, Difficulty.ExtremelyHard, Difficulty.Easy, Alignment.FrontRight, Orientation.Highest, 5.0,
			1.0, elbowshape, greatDamage, $"@ try|tries to strike $1 with &0's {{0}}{attackAddendum}",
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Stun |
			            CombatMoveIntentions.Risky | CombatMoveIntentions.Hard);
		AddAttack("Foot Stomp", BuiltInCombatMoveType.ClinchUnarmedAttack, MeleeWeaponVerb.Kick, Difficulty.Hard,
			Difficulty.Easy, Difficulty.ExtremelyHard, Difficulty.Easy, Alignment.FrontRight, Orientation.Lowest, 2.0,
			0.8, footshape, goodDamage, $"@ try|tries to stomp on $1 with &0's {{0}}{attackAddendum}",
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Stun |
			            CombatMoveIntentions.Risky | CombatMoveIntentions.Hard);
		AddAttack("Roundhouse Knee", BuiltInCombatMoveType.ClinchUnarmedAttack, MeleeWeaponVerb.Kick, Difficulty.Hard,
			Difficulty.Hard, Difficulty.ExtremelyHard, Difficulty.Easy, Alignment.Right, Orientation.Centre, 6.0, 0.8,
			kneeshape, greatDamage, $"@ swing|swings &0's @hand leg in a roundhouse knee strike at $1{attackAddendum}",
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Hard);
		AddAttack("Low Roundhouse Knee", BuiltInCombatMoveType.ClinchUnarmedAttack, MeleeWeaponVerb.Kick,
			Difficulty.Hard, Difficulty.Hard, Difficulty.ExtremelyHard, Difficulty.Easy, Alignment.Right,
			Orientation.Low, 6.0, 0.8, kneeshape, greatDamage,
			$"@ swing|swings &0's @hand leg in a low roundhouse knee strike at $1{attackAddendum}",
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Hard);
		AddAttack("Shoulder Push", BuiltInCombatMoveType.UnbalancingBlowClinch, MeleeWeaponVerb.Punch,
			Difficulty.VeryEasy, Difficulty.Hard, Difficulty.ExtremelyHard, Difficulty.Easy, Alignment.FrontRight,
			Orientation.High, 3.0, 0.8, shouldershape, badDamage,
			$"@ strike|strikes $1 with &0's @hand shoulder in an attempt to knock &1 back{attackAddendum}",
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Hard,
			additionalInfo: "5");
	}

	private IReadOnlyDictionary<string, TraitDefinition> SeedCoreData(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> questionAnswers)
	{
		var skills = SeedUniversalSkills(context, questionAnswers);
		if (!context.CombatMessages.Any(x => x.Type == (int)BuiltInCombatMoveType.Dodge))
			SeedUnarmedCombatMessage(context, questionAnswers);

		// Seed Combat Strategies
		if (!context.CharacterCombatSettings.Any()) SeedCombatStrategies(context, questionAnswers);

		// Set up shield types
		if (!context.ShieldTypes.Any()) SeedShields(context, questionAnswers, skills);

		SeedChecks(context, questionAnswers, skills);

		return skills;
	}

	private IReadOnlyDictionary<string, TraitDefinition> SeedUniversalSkills(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> questionAnswers)
	{
		var skills = new Dictionary<string, TraitDefinition>(StringComparer.OrdinalIgnoreCase);
		foreach (var skill in context.TraitDefinitions.Where(x => x.Type == 0)) skills[skill.Name] = skill;
		var gerund =
			context.TraitDefinitions.FirstOrDefault(x => x.Name == "Track" || x.Name == "Tracking")?.Name ==
			"Tracking";
		var attributes = context.TraitDefinitions.Where(x => x.Type == 1 || x.Type == 3)
			.ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
		var strength =
			attributes.GetValueOrDefault("Strength") ??
			attributes.GetValueOrDefault("Physique") ??
			attributes["Body"];
		var dex =
			attributes.GetValueOrDefault("Agility") ??
			attributes.GetValueOrDefault("Dexterity") ??
			attributes.GetValueOrDefault("Agility") ??
			attributes.GetValueOrDefault("Speed") ??
			attributes["Body"];
		var armourUseSkill = context.TraitDefinitions.FirstOrDefault(x => x.Name == "Armour Use");
		var useStats = armourUseSkill?.Expression.Expression != "70";

		TraitDefinition CreateSkill(string name, string decorator = "General Skill")
		{
			var expression = new TraitExpression
			{
				Name = $"{name} Skill Cap",
				Expression = useStats ? $"min(99,2*{strength.Alias}:{strength.Id}+3*{dex.Alias}:{dex.Id})" : "70"
			};
			var skill = new TraitDefinition
			{
				Name = name,
				Type = (int)TraitType.Skill,
				Expression = expression,
				TraitGroup = "Combat",
				AvailabilityProg = context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue"),
				TeachableProg = context.FutureProgs.First(x => x.FunctionName == "AlwaysFalse"),
				LearnableProg = context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue"),
				TeachDifficulty = 7,
				LearnDifficulty = 7,
				Hidden = false,
				ImproverId = context.Improvers.First(x => x.Name == "Skill Improver").Id,
				DecoratorId = context.TraitDecorators.First(x => x.Name == decorator).Id,
				DerivedType = 0,
				ChargenBlurb = string.Empty,
				BranchMultiplier = 1.0
			};
			context.TraitDefinitions.Add(skill);
			context.SaveChanges();
			skills.Add(name, skill);
			return skill;
		}

		CreateSkill(gerund ? "Blocking" : "Block");
		CreateSkill(gerund ? "Dodging" : "Dodge");
		CreateSkill(gerund ? "Brawling" : "Brawl");
		CreateSkill(gerund ? "Wrestling" : "Wrestle");
		CreateSkill(gerund ? "Warding" : "Ward");
		CreateSkill(gerund ? "Throwing" : "Throw");
		CreateSkill(gerund ? "Veterancy" : "Veterancy", "Veterancy Skill");
		if (questionAnswers["parryoption"].ToLowerInvariant().EqualToAny("yes", "y"))
			CreateSkill(gerund ? "Parrying" : "Parry");

		return skills;
	}


	private void SeedChecks(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers,
		IReadOnlyDictionary<string, TraitDefinition> skills)
	{
		void AddCheck(CheckType type, TraitExpression expression, long templateId,
			Difficulty maximumImprovementDifficulty)
		{
			var teName = $"{type.DescribeEnum(true)} Formula";
			var existingExpression = context.TraitExpressions.FirstOrDefault(x => x.Name == teName);
			if (existingExpression is not null)
			{
				existingExpression.Expression = expression.Expression;
				expression = existingExpression;
			}
			else
			{
				context.TraitExpressions.Add(expression);
			}

			var intType = (int)type;
			var existingCheck = context.Checks.FirstOrDefault(x => x.Type == intType);
			if (existingCheck is null)
			{
				context.Checks.Add(new Check
				{
					Type = (int)type,
					CheckTemplateId = templateId,
					MaximumDifficultyForImprovement = (int)maximumImprovementDifficulty,
					TraitExpression = expression
				});
			}
			else
			{
				existingCheck.TraitExpression = expression;
				existingCheck.CheckTemplateId = templateId;
				existingCheck.MaximumDifficultyForImprovement = (int)maximumImprovementDifficulty;
			}

			context.SaveChanges();
		}

		var attributes = context.TraitDefinitions.Where(x => x.Type == 1 || x.Type == 3)
			.ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
		var wilAttribute =
			attributes.GetValueOrDefault("Willpower") ??
			attributes.GetValueOrDefault("Wisdom") ??
			attributes["Spirit"];
		var template = context.CheckTemplates.First(x => x.Name == "Skill Check");
		var parryoption = questionAnswers["parryoption"].ToLowerInvariant();

		foreach (var check in Enum.GetValues(typeof(CheckType)).OfType<CheckType>().Distinct().ToList())
			switch (check)
			{
				case CheckType.NaturalWeaponAttack:
					AddCheck(check,
						new TraitExpression
							{ Expression = $"brawl:{(skills.GetValueOrDefault("Brawling") ?? skills["Brawl"]).Id}" },
						template.Id, Difficulty.Impossible);
					continue;
				case CheckType.DodgeCheck:
					AddCheck(check,
						new TraitExpression
							{ Expression = $"dodge:{(skills.GetValueOrDefault("Dodging") ?? skills["Dodge"]).Id}" },
						template.Id, Difficulty.Impossible);
					continue;
				case CheckType.ParryCheck:
					if (parryoption.EqualToAny("yes", "y"))
					{
						AddCheck(check,
							new TraitExpression
							{
								Expression = $"parry:{(skills.GetValueOrDefault("Parrying") ?? skills["Parry"]).Id}"
							}, template.Id, Difficulty.Impossible);
						continue;
					}

					AddCheck(check, new TraitExpression { Expression = "variable" }, template.Id,
						Difficulty.Impossible);
					continue;
				case CheckType.BlockCheck:
					AddCheck(check,
						new TraitExpression
							{ Expression = $"block:{(skills.GetValueOrDefault("Blocking") ?? skills["Block"]).Id}" },
						template.Id, Difficulty.Impossible);
					continue;
				case CheckType.FleeMeleeCheck:
					AddCheck(check,
						new TraitExpression
						{
							Expression =
								$"running:{(skills.GetValueOrDefault("Running") ?? skills.GetValueOrDefault("Run") ?? skills["Athletics"]).Id} + (0.5 * {skills["Veterancy"].Id})"
						}, template.Id, Difficulty.Impossible);
					continue;
				case CheckType.OpposeFleeMeleeCheck:
					AddCheck(check,
						new TraitExpression
						{
							Expression =
								$"running:{(skills.GetValueOrDefault("Running") ?? skills.GetValueOrDefault("Run") ?? skills["Athletics"]).Id} + (0.5 * {skills["Veterancy"].Id})"
						}, template.Id, Difficulty.Impossible);
					continue;
				case CheckType.Ward:
					AddCheck(check,
						new TraitExpression
							{ Expression = $"ward:{(skills.GetValueOrDefault("Warding") ?? skills["Ward"]).Id}" },
						template.Id, Difficulty.Impossible);
					continue;
				case CheckType.WardDefense:
					AddCheck(check, new TraitExpression { Expression = $"veterancy:{skills["Veterancy"].Id}" },
						template.Id, Difficulty.Impossible);
					continue;
				case CheckType.WardIgnore:
					AddCheck(check,
						new TraitExpression
						{
							Expression =
								$"(0.5 * veterancy:{skills["Veterancy"].Id}) + ({wilAttribute.Alias}:{wilAttribute.Id} * 2.5)"
						}, template.Id, Difficulty.Impossible);
					continue;
				case CheckType.StartClinch:
					AddCheck(check,
						new TraitExpression
						{
							Expression =
								$"(0.5 * dodge:{(skills.GetValueOrDefault("Dodging") ?? skills["Dodge"]).Id}) + (0.5 * brawl:{(skills.GetValueOrDefault("Brawling") ?? skills["Brawl"]).Id})"
						}, template.Id, Difficulty.Impossible);
					continue;
				case CheckType.ResistClinch:
					AddCheck(check,
						new TraitExpression
						{
							Expression =
								$"(0.5 * dodge:{(skills.GetValueOrDefault("Dodging") ?? skills["Dodge"]).Id}) + (0.5 * brawl:{(skills.GetValueOrDefault("Brawling") ?? skills["Brawl"]).Id})"
						}, template.Id, Difficulty.Impossible);
					continue;
				case CheckType.BreakClinch:
					AddCheck(check,
						new TraitExpression
						{
							Expression =
								$"(0.5 * dodge:{(skills.GetValueOrDefault("Dodging") ?? skills["Dodge"]).Id}) + (0.5 * brawl:{(skills.GetValueOrDefault("Brawling") ?? skills["Brawl"]).Id})"
						}, template.Id, Difficulty.Impossible);
					continue;
				case CheckType.ResistBreakClinch:
					AddCheck(check,
						new TraitExpression
						{
							Expression =
								$"(0.5 * dodge:{(skills.GetValueOrDefault("Dodging") ?? skills["Dodge"]).Id}) + (0.5 * brawl:{(skills.GetValueOrDefault("Brawling") ?? skills["Brawl"]).Id})"
						}, template.Id, Difficulty.Impossible);
					continue;
				case CheckType.RescueCheck:
					AddCheck(check,
						new TraitExpression
						{
							Expression =
								$"(0.5 * veterancy:{skills["Veterancy"].Id}) + (0.5 * brawl:{(skills.GetValueOrDefault("Brawling") ?? skills["Brawl"]).Id})"
						}, template.Id, Difficulty.Impossible);
					continue;
				case CheckType.OpposeRescueCheck:
					AddCheck(check,
						new TraitExpression
						{
							Expression =
								$"(0.5 * veterancy:{skills["Veterancy"].Id}) + (0.5 * brawl:{(skills.GetValueOrDefault("Brawling") ?? skills["Brawl"]).Id})"
						}, template.Id, Difficulty.Impossible);
					continue;
				case CheckType.StaggeringBlowDefense:
					AddCheck(check,
						new TraitExpression
						{
							Expression =
								$"(0.25 * veterancy:{skills["Veterancy"].Id}) + (0.75 * endure:{(skills.GetValueOrDefault("Enduring") ?? skills.GetValueOrDefault("Endurance") ?? skills["Athletics"]).Id})"
						}, template.Id, Difficulty.Impossible);
					continue;
				case CheckType.StruggleFreeFromDrag:
					AddCheck(check,
						new TraitExpression
						{
							Expression =
								$"(0.5 * dodge:{(skills.GetValueOrDefault("Dodging") ?? skills["Dodge"]).Id}) + (0.5 * wrestle:{(skills.GetValueOrDefault("Wrestling") ?? skills["Wrestle"]).Id})"
						}, template.Id, Difficulty.Impossible);
					continue;
				case CheckType.OpposeStruggleFreeFromDrag:
					AddCheck(check,
						new TraitExpression
						{
							Expression =
								$"(0.5 * dodge:{(skills.GetValueOrDefault("Dodging") ?? skills["Dodge"]).Id}) + (0.5 * wrestle:{(skills.GetValueOrDefault("Wrestling") ?? skills["Wrestle"]).Id})"
						}, template.Id, Difficulty.Impossible);
					continue;
				case CheckType.CounterGrappleCheck:
					AddCheck(check,
						new TraitExpression
						{
							Expression = $"wrestle:{(skills.GetValueOrDefault("Wrestling") ?? skills["Wrestle"]).Id}"
						}, template.Id, Difficulty.Impossible);
					continue;
				case CheckType.StruggleFreeFromGrapple:
					AddCheck(check,
						new TraitExpression
						{
							Expression = $"wrestle:{(skills.GetValueOrDefault("Wrestling") ?? skills["Wrestle"]).Id}"
						}, template.Id, Difficulty.Impossible);
					continue;
				case CheckType.OpposeStruggleFreeFromGrapple:
					AddCheck(check,
						new TraitExpression
						{
							Expression = $"wrestle:{(skills.GetValueOrDefault("Wrestling") ?? skills["Wrestle"]).Id}"
						}, template.Id, Difficulty.Impossible);
					continue;
				case CheckType.ExtendGrappleCheck:
					AddCheck(check,
						new TraitExpression
						{
							Expression = $"wrestle:{(skills.GetValueOrDefault("Wrestling") ?? skills["Wrestle"]).Id}"
						}, template.Id, Difficulty.Impossible);
					continue;
				case CheckType.InitiateGrapple:
					AddCheck(check,
						new TraitExpression
						{
							Expression = $"wrestle:{(skills.GetValueOrDefault("Wrestling") ?? skills["Wrestle"]).Id}"
						}, template.Id, Difficulty.Impossible);
					continue;
				case CheckType.ScreechAttack:
					AddCheck(check,
						new TraitExpression
							{ Expression = $"brawl:{(skills.GetValueOrDefault("Brawling") ?? skills["Brawl"]).Id}" },
						template.Id, Difficulty.Impossible);
					continue;
				case CheckType.StrangleCheck:
					AddCheck(check,
						new TraitExpression
						{
							Expression = $"wrestle:{(skills.GetValueOrDefault("Wrestling") ?? skills["Wrestle"]).Id}"
						}, template.Id, Difficulty.Impossible);
					continue;
				case CheckType.WrenchAttackCheck:
					AddCheck(check,
						new TraitExpression
						{
							Expression = $"wrestle:{(skills.GetValueOrDefault("Wrestling") ?? skills["Wrestle"]).Id}"
						}, template.Id, Difficulty.Impossible);
					continue;
				case CheckType.TakedownCheck:
					AddCheck(check,
						new TraitExpression
						{
							Expression = $"wrestle:{(skills.GetValueOrDefault("Wrestling") ?? skills["Wrestle"]).Id}"
						}, template.Id, Difficulty.Impossible);
					continue;
				case CheckType.BreakoutCheck:
					AddCheck(check,
						new TraitExpression
						{
							Expression = $"wrestle:{(skills.GetValueOrDefault("Wrestling") ?? skills["Wrestle"]).Id}"
						}, template.Id, Difficulty.Impossible);
					continue;
				case CheckType.OpposeBreakoutCheck:
					AddCheck(check,
						new TraitExpression
						{
							Expression = $"wrestle:{(skills.GetValueOrDefault("Wrestling") ?? skills["Wrestle"]).Id}"
						}, template.Id, Difficulty.Impossible);
					continue;
				case CheckType.TossItemCheck:
					AddCheck(check,
						new TraitExpression
							{ Expression = $"throw:{(skills.GetValueOrDefault("Throwing") ?? skills["throw"]).Id}+30" },
						template.Id, Difficulty.Impossible);
					continue;
			}
	}

	public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
	{
		if (context.WeaponAttacks.Any()) return ShouldSeedResult.MayAlreadyBeInstalled;

		if (!context.Races.Any(x => x.Name == "Human")) return ShouldSeedResult.PrerequisitesNotMet;

		return ShouldSeedResult.ReadyToInstall;
	}

	public int SortOrder => 90;
	public string Name => "Combat";
	public string Tagline => "Attacks, Echoes, Weapon Types and all the fun stuff";

	public string FullDescription =>
		@"This seeder will set up everything you need to get combat going in your MUD, including things like weapon types and attacks. There is a lot of customisation you can do with this system and it's easily possible to further extend it once you run this importer. 

However, I will say that there are actually some very big decisions up front in setting it up, particularly when it comes to weapon attacks. 

I'd recommend that if you're uncertain about what options you might select or you're unfamiliar with how the engine does things that you might want to play around with a local copy of your database with a few iterations of this seeder until you find something you're comfortable with.";

	#endregion
}
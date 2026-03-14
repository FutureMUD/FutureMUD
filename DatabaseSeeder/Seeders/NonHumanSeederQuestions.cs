#nullable enable

using System;
using System.Collections.Generic;
using MudSharp.Database;
using MudSharp.Framework;

namespace DatabaseSeeder.Seeders;

internal static class NonHumanSeederQuestions
{
	internal static IEnumerable<(string Id, string Question,
		Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
		Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> GetQuestions()
	{
		return
		[
			("model",
				@"#DHealth Models#F

Which health model should non-human creatures use by default? This can be overriden for individual NPCs (so you can make HP-based mooks even if you use the full medical system). Even if you use the full medical model for humans, you may not want to use it for all non-human creatures by default.

The valid choices are as follows:

#Bhp#F	- this system will use hitpoints or destruction of the brain only to determine death.
#Bhpplus#F	- this system uses hp and brain destruction, but also enables heart and organ damage.
#Bfull#F	- this system uses the full medical model, where the only way to die is via death of the brain.

Your choice: ",
				(context, answers) => true,
				(text, context) => text.ToLowerInvariant() switch
				{
					"hp" or "hpplus" or "full" => (true, string.Empty),
					_ => (false, "That is not a valid selection.")
				}
			),
			("random",
				@"#DDamage Formulas#F

You can configure your damage formulas to be consistent or random. The engine already takes into account a number of variables such as relative success of attacker and defender, type of defense used, all of which ensure that the damage is mitigated differently each attack. However, a good hit is usually pretty impactful in that kind of setup.

Randomness in damage is sometimes used to add complexity or choice to weapon types when the outcome of the attack is fairly likely (see D20 systems where before long hitting is almost guaranteed). This can work too but it can be disappointing for someone to land a good blow with all the factors right and then simply do little damage because of RNG, whereas another just-barely hit might do full damage.

There are three options that you can choose for randomness:

#BStatic:#F In this option (which was used in LabMUD) base damage is static. A hit with the same quality weapon, the same strength and the same attack/defense result will lead to the same damage
#BPartial#F: In this option 30% of the damage will be random - this adds a little bit of uncertainty and variety but still makes hits /largely/ a function of relative success
#BRandom#F: In this option damage can be 20-100% of the maximum. This means outcomes will vary wildly.

Which option do you want to use for random results in your non-human damage formulas? ",
				(context, answers) => true,
				(answer, context) =>
				{
					return (answer.EqualToAny("static", "partial", "random"),
						"You must answer static, partial or random.");
				}
			),
			("messagestyle",
				@"#DCombat Messages#F

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

You can choose #3Compact#f, #3Sentences#f or #3Sparse#f: ",
				(context, answers) => string.IsNullOrWhiteSpace(CombatSeederMessageStyleHelper.GetRecordedChoice(context)),
				(answer, context) =>
				{
					return (answer.EqualToAny("compact", "sentences", "sparse"),
						"You must answer Compact, Sentences or Sparse.");
				}
			)
		];
	}
}

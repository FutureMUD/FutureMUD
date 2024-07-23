using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.RPG.Checks;

public class PassivePerceptionCheck : TimeboundCheck
{
	public PassivePerceptionCheck(Models.Check check, IFuturemud game) : base(check, game)
	{
		var root = XElement.Parse(check.CheckTemplate.Definition);
		var element = root.Element("CoreTraitExpression");
		if (element == null)
		{
			throw new ApplicationException(
				$"Check Template #{check.CheckTemplateId} had a definition that did not specify a CoreTraitExpression element");
		}

		CoreTraitExpression = new TraitExpression(element.Value, game);

		element = root.Element("PassiveFuzziness");
		if (element == null)
		{
			throw new ApplicationException(
				$"Check Template #{check.CheckTemplateId} had a definition that did not specify a PassiveFuzziness element");
		}

		PassiveFuzziness = double.Parse(element.Value);
	}

	public TraitExpression CoreTraitExpression { get; private set; }

	public double PassiveFuzziness { get; private set; }

	public override CheckOutcome Check(IPerceivableHaveTraits checkee, Difficulty difficulty,
		ITraitDefinition trait, IPerceivable target = null, double externalBonus = 0.0,
		TraitUseType traitUseType = TraitUseType.Practical,
		params (string Parameter, object value)[] customParameters)
	{
		var checkeeAsCharacter = checkee as ICharacter ?? (checkee as IBody)?.Actor;
		var targetAsCharacter = target as ICharacter ?? (checkee as IBody)?.Actor;
		var targetAsItem = target as IGameItem;

		var core = CoreTraitExpression.EvaluateWith(checkee, values: customParameters);
		var opposed = (targetAsCharacter != null
			              ? targetAsCharacter.EffectsOfType<IHideEffect>().FirstOrDefault()?.EffectiveHideSkill
			              : targetAsItem.EffectsOfType<IItemHiddenEffect>().FirstOrDefault()?.EffectiveHideSkill)
		              ?? 0.0
			;

		var bonuses = new List<double>();

		var innateBonus = checkee.GetCurrentBonusLevel();
		if (innateBonus != 0)
		{
			bonuses.Add(innateBonus);
		}

		if (externalBonus != 0)
		{
			bonuses.Add(externalBonus);
		}

		bonuses.AddRange(checkee.EffectsOfType<ICheckBonusEffect>()
		                        .Where(x => x.AppliesToCheck(CheckType.PassiveStealthCheck))
		                        .ToList()
		                        .Select(x => x.CheckBonus));

		bonuses.AddRange(checkee.Merits.OfType<ICheckBonusMerit>()
		                        .Where(x => x.Applies(checkee, target))
		                        .Select(x => x.CheckBonus(checkeeAsCharacter, target, CheckType.PassiveStealthCheck)));

		var bonus = bonuses.Sum();
		difficulty = bonus > 0
			? difficulty.StageDown((int)(bonus / BonusesPerDifficultyLevel))
			: difficulty.StageUp((int)(-1 * bonus / BonusesPerDifficultyLevel));

		if (core + Modifiers[difficulty] >= opposed * (1 - PassiveFuzziness))
		{
			return base.Check(checkee, difficulty, trait, target, externalBonus, traitUseType, customParameters);
		}

		return new CheckOutcome
		{
			Outcome = Outcome.MajorFail
		};
	}
}
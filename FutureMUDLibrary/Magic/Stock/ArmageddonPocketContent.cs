#nullable enable

using System.Globalization;
using System.Xml.Linq;
using MudSharp.FutureProg;

namespace MudSharp.Magic;

public static class ArmageddonPocketContent
{
	public static ArmageddonUtilitySpellContent Create(SpellPocketConfiguration configuration)
	{
		configuration.Validate();
		return new("arm.spell.folded_pocket", "Folded Pocket", "Finite portable item storage with conserved collapse and explicit access.",
			configuration.SecondsPerGrade.ToString(CultureInfo.InvariantCulture) + "*grade", 12,
			"$0 fold|folds a finite magical pocket around $1.", (resource, cost, filter) =>
				ArmageddonUtilitySpellContent.Definition("arm.spell.folded_pocket", "item", resource, cost, filter, 30, 12,
					new XElement("Effect", new XAttribute("type", "createpocket"), new XAttribute("version", 1), configuration.Save())),
			TargetType: ProgVariableTypes.Item);
	}
}

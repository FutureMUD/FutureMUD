using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic
{
	public interface IMagicSpell : ISaveable, IEditableItem, IProgVariable
	{
		IFutureProg SpellKnownProg { get; }
		IMagicSchool School { get; }
		TimeSpan ExclusiveDelay { get; }
		TimeSpan NonExclusiveDelay { get; }
		IMagicTrigger Trigger { get; }
		IEnumerable<IMagicSpellEffectTemplate> SpellEffects { get; }
		IEnumerable<IMagicSpellEffectTemplate> CasterSpellEffects { get; }

		string Blurb { get; }
		string Description { get; }
		void CastSpell(ICharacter magician, IPerceivable target, SpellPower power, params SpellAdditionalParameter[] additionalParameters);
		bool ReadyForGame { get; }
		string WhyNotReadyForGame(ICharacter builder);
		string ShowPlayerHelp(ICharacter actor);
		bool AppliedEffectsAreExclusive { get; }
	}
}

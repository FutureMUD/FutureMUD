using MudSharp.Character;
using MudSharp.Magic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Effects.Interfaces
{
    public interface IMagicSpellEffectParent : IEffect
    {
        IMagicSpell Spell { get; }
        ICharacter Caster { get; }
        IEnumerable<IMagicSpellEffect> SpellEffects { get; }

        void AddSpellEffect(IMagicSpellEffect effect);
    }
}

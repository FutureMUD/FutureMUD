using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Framework;
using MudSharp.Magic;

namespace MudSharp.Effects.Interfaces
{
    public interface IMagicSpellEffect : IEffect
    {
        IMagicSpellEffectParent ParentEffect { get; set; }
        IMagicSpell Spell { get; }
    }
}

#nullable enable

using MudSharp.Character;
using MudSharp.Magic;

namespace MudSharp.Effects.Interfaces;

public interface IMagicTagEffect : IEffectSubtype
{
	string Tag { get; }
	string Value { get; }
	ICharacter? Caster { get; }
	IMagicSpell Spell { get; }
}

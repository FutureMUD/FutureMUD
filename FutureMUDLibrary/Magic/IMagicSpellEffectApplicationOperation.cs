#nullable enable

using MudSharp.Effects.Interfaces;

namespace MudSharp.Magic;

/// <summary>A prepared application that reports its actual mutation independently of child creation.</summary>
public interface IMagicSpellEffectApplicationOperation : IMagicSpellEffectApplication
{
	MagicEffectOperation Apply(IMagicSpellEffectParent parent);
}

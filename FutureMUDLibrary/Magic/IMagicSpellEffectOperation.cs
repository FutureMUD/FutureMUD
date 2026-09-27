using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

#nullable enable
namespace MudSharp.Magic;

public enum MagicEffectOperationStatus { Applied, NoChange, Rejected, Unknown }

public sealed record MagicEffectOperation(MagicEffectOperationStatus Status, IMagicSpellEffect? Effect);

/// <summary>Apply once and report the actual operation independently of persistent child creation.</summary>
public interface IMagicSpellEffectOperation
{
	MagicEffectOperation Apply(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters);
}

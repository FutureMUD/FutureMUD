#nullable enable
using MudSharp.Character;
using MudSharp.Framework;
namespace MudSharp.Magic;

/// <summary>An invocation-local, immutable choice; never a global cache or persistence receipt.</summary>
public interface IMagicSpellEffectPreparedSelectionToken { }

/// <summary>Reuses a choice across fresh casting copies while independently checking live admission.</summary>
public interface IMagicSpellEffectPreparedSelection
{
    IMagicSpellEffectPreparedSelectionToken? CapturePreparedSelection(ICharacter caster, IPerceivable recipient);
    bool TryReusePreparedSelection(IMagicSpellEffectPreparedSelectionToken selection,
        ICharacter caster, IPerceivable recipient, out string? error);
    /// <summary>Last live policy evaluation before payment. Must confirm an existing choice without drawing.</summary>
    bool TryConfirmPreparedSelection(ICharacter caster, IPerceivable recipient, out string? error);
}

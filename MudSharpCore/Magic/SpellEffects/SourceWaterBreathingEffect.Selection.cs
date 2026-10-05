#nullable enable

namespace MudSharp.Magic.SpellEffects;

public sealed partial class SourceWaterBreathingEffect
{
	private IMagicSpellEffectPreparedSelectionToken? _selection;
	internal IMagicSpellEffectPreparedSelectionToken? Selection => _selection;
	public IMagicSpellEffectPreparedSelectionToken CapturePreparedSelection(ICharacter caster, IPerceivable recipient) =>
		_selection = ((MagicSpell)Spell).CaptureWaterSelection(this, caster, recipient, _selection);

	public bool TryReusePreparedSelection(IMagicSpellEffectPreparedSelectionToken selection,
		ICharacter caster, IPerceivable recipient, out string? error)
	{
		try
		{
			((MagicSpell)Spell).CaptureWaterSelection(this, caster, recipient, selection);
			_selection = selection; error = null; return true;
		}
		catch (InvalidOperationException exception) { error = exception.Message; return false; }
	}

	public bool TryConfirmPreparedSelection(ICharacter caster, IPerceivable recipient, out string? error)
	{
		try
		{
			if (_selection is null) throw new InvalidOperationException("Water breathing has no prepared duration selection.");
			((MagicSpell)Spell).CaptureWaterSelection(this, caster, recipient, _selection);
			error = null; return true;
		}
		catch (InvalidOperationException exception) { error = exception.Message; return false; }
	}
}

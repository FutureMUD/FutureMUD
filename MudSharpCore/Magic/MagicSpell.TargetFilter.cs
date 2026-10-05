#nullable enable
using MudSharp.Magic.SpellTriggers;
namespace MudSharp.Magic;

public partial class MagicSpell
{
	private string? TargetFilterConfigurationError => Trigger switch {
		CastingTriggerCharacter character => character.TargetFilterError,
		CastingTriggerItem item => item.TargetFilterError,
		_ => null
	};
}

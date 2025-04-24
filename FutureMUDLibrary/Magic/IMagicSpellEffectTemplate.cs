using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.RPG.Checks;

#nullable enable
namespace MudSharp.Magic;

public interface IMagicSpellEffectTemplate : IXmlSavable
{
    bool IsInstantaneous { get; }
    bool RequiresTarget { get; }
    IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters);
    /// <summary>
    /// Executes a building command based on player input
    /// </summary>
    /// <param name="actor">The avatar of the player doing the command</param>
    /// <param name="command">The command they wish to execute</param>
    /// <returns>Returns true if the command was valid and anything was changed. If nothing was changed or the command was invalid, it returns false</returns>
    bool BuildingCommand(ICharacter actor, StringStack command);

    /// <summary>
    /// Shows a builder-specific output representing the IEditableItem
    /// </summary>
    /// <param name="actor">The avatar of the player who wants to view the IEditableItem</param>
    /// <returns>A string representing the item textually</returns>
    string Show(ICharacter actor);
    IMagicSpellEffectTemplate Clone();
    bool IsCompatibleWithTrigger(IMagicTrigger trigger);
}
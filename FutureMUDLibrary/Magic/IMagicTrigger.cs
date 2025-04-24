using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;

namespace MudSharp.Magic
{
	public interface IMagicTrigger : IXmlSavable
	{
		MagicTriggerType TriggerType { get; }
		IMagicTrigger Clone();
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

		string ShowPlayer(ICharacter actor);

		/// <summary>
		/// This trigger will lead to a target being identified for the spell
		/// </summary>
		bool TriggerYieldsTarget { get; }

		/// <summary>
		/// This trigger could fail to find a target
		/// </summary>
		bool TriggerMayFailToYieldTarget { get; }

		/// <summary>
		/// A list of target types produced by this magic trigger
		/// </summary>
		string TargetTypes { get; }
	}
}
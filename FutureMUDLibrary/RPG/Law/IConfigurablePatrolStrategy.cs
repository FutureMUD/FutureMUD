using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.RPG.Law;

public interface IConfigurablePatrolStrategy : IPatrolStrategy
{
	string HelpText { get; }
	bool BuildingCommand(ICharacter actor, IPatrolRoute patrol, StringStack command);
	string ShowConfiguration(ICharacter actor, IPatrolRoute patrol);
	string SaveStrategyData();
	bool ReadyToBegin(IPatrolRoute patrol);
	string WhyCannotBegin(IPatrolRoute patrol)
	{
		return ReadyToBegin(patrol) ? string.Empty : "The patrol strategy's configuration is not ready.";
	}
}

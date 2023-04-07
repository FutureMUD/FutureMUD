using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Combat.Strategies;

public class FullSkirmishStrategy : SkirmishStrategy
{
	protected FullSkirmishStrategy()
	{
	}

	public new static FullSkirmishStrategy Instance { get; } = new();
	public override CombatStrategyMode Mode => CombatStrategyMode.FullSkirmish;
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class Scanning : MultiStageBlockingDelayedAction, IRemoveOnMovementEffect, ILDescSuffixEffect
{
	public Scanning(IPerceivable owner, IEnumerable<Action<IPerceivable>> actions, string actionDescription,
		string block, int fireOnCount, TimeSpan timeBetweenTicks) : base(owner, actions, actionDescription, block,
		fireOnCount, timeBetweenTicks)
	{
	}

	public Scanning(IPerceivable owner, IEnumerable<Action<IPerceivable>> actions, string actionDescription,
		string block, int fireOnCount, IEnumerable<TimeSpan> timesBetweenTicks) : base(owner, actions,
		actionDescription, block, fireOnCount, timesBetweenTicks)
	{
	}

	public Scanning(IPerceivable owner, IEnumerable<Action<IPerceivable>> actions, string actionDescription,
		IEnumerable<string> blocks, int fireOnCount, TimeSpan timeBetweenTicks) : base(owner, actions,
		actionDescription, blocks, fireOnCount, timeBetweenTicks)
	{
	}

	public bool SuffixApplies()
	{
		return true;
	}

	public string SuffixFor(IPerceiver voyeur)
	{
		return "scanning the horizon";
	}

	bool IRemoveOnMovementEffect.ShouldRemove()
	{
		return true;
	}
}
using System;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;

namespace MudSharp.Events.Hooks;

public interface IHook : IFrameworkItem, IHaveFuturemud, ISaveable {
	Func<EventType, object[], bool> Function { get; }
	EventType Type { get; }
	string Category { get; set; }
	string InfoForHooklist { get; }
	new string Name { get; set; }
}

public interface ICommandHook : IHook
{
	string CommandText { get; set; }
}

public interface IExecuteProgHook : IHook, IHookWithProgs
{
	void AddProg(IFutureProg prog);
	bool RemoveProg(IFutureProg prog);
}
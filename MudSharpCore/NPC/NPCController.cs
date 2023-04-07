using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Handlers;

namespace MudSharp.NPC;

public class NPCController : IFuturemudAccountController, IFuturemudPlayerController
{
	private readonly List<IMonitor> _observedBy = new();
	private readonly List<IMonitorable> _observes = new();
	private IControllable _context;
	public long InactivityMilliseconds => 0L;

	#region IHandleCommands Members

	public void HandleCommand(string command)
	{
		if (_context.HandleSubContext(command))
		{
			return;
		}

		_context.ExecuteCommand(command);

		if (_context.NextContext == null)
		{
			return;
		}

		_context.LoseControl(this);
		_context = _context.NextContext;
		_context.AssumeControl(this);
	}

	#endregion

	#region IDisposable Members

	public void Dispose()
	{
		_context?.LoseControl(this);
		_context = null;
		foreach (var observee in _observes)
		{
			RemoveObservee(observee);
		}

		GC.SuppressFinalize(this);
	}

	#endregion

	#region IHaveFuturemud Members

	public IFuturemud Gameworld { get; protected set; }

	#endregion

	#region ITimeout Members

	public int Timeout => int.MaxValue;

	#endregion

	#region IAccountController Members

	public IAccount Account => DummyAccount.Instance;

	void IAccountController.BindAccount(IAccount account)
	{
		throw new NotSupportedException();
	}

	void IAccountController.DetachConnection()
	{
		throw new NotSupportedException();
	}

	#endregion

	#region IController Members

	public void Close()
	{
	}

	public void SetContext(IControllable context)
	{
		_context?.LoseControl(this);
		_context = context;
		_context?.AssumeControl(this);
	}

	#endregion

	#region IHandleOutput Members

	public IOutputHandler OutputHandler { get; private set; } = new NonPlayerOutputHandler();

	public void Register(IOutputHandler handler)
	{
		OutputHandler = handler;
		Actor?.Register(handler);
	}

	#endregion

	#region IMonitorable Members

	void IMonitorable.AddObserver(IMonitor observer)
	{
		if (!_observedBy.Contains(observer))
		{
			if (!_observedBy.Any())
			{
				OutputHandler.QuietMode = false;
			}

			_observedBy.Add(observer);
		}
	}

	void IMonitorable.RemoveObserver(IMonitor observer)
	{
		if (_observedBy.Contains(observer))
		{
			_observedBy.Remove(observer);
			if (!_observedBy.Any())
			{
				OutputHandler.QuietMode = true;
			}
		}
	}

	#endregion

	#region IMonitor Members

	public void AddObservee(IMonitorable observee)
	{
		if (_observes.Contains(observee))
		{
			return;
		}

		_observes.Add(observee);
		observee.AddObserver(this);
	}

	public void RemoveObservee(IMonitorable observee)
	{
		if (!_observes.Contains(observee))
		{
			return;
		}

		_observes.Remove(observee);
		observee.RemoveObserver(this);
	}

	public void UpdateObservers()
	{
	}

	#endregion

	#region IPlayerController Members

	public bool Closing { get; protected set; }

	public string IPAddress => "127.0.0.1";

	public void CuePrompt()
	{
		if (_context.HasPrompt)
		{
			OutputHandler.Send(_context.Prompt, false);
		}
	}

	#endregion

	#region ICharacterController Members

	public ICharacter Actor { get; protected set; }

	public string LDescAdditionalTags => "";

	public void UpdateControlFocus(ICharacter newFocus)
	{
		Actor = newFocus;
		_context = newFocus;
	}

	#endregion
}
using System;
using System.Collections.Generic;
using System.Text;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.GameItems;
using MudSharp.Menus;
using MudSharp.Network;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Handlers;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Framework;

public sealed class FuturemudControlContext : IFuturemudControlContext
{
	private readonly List<IMonitor> _observedBy = new();

	private readonly List<IMonitorable> _observes = new();
	private IPlayerConnection _connection;
	private IControllable _context;

	//static FuturemudControlContext() {
	//    BuildCommandsTree();
	//}

	public FuturemudControlContext(IPlayerConnection player, IFuturemud game)
	{
		_connection = player;
		_context = new MainMenu(game, this);
		OutputHandler = new PlayerOutputHandler(new StringBuilder(), null);
		Gameworld = game;

		_context.AssumeControl(this);
	}

	public IGameItemProto GameItemEdit { get; private set; }

	public string Prompt => "\n> ";

	public IOutputHandler OutputHandler { get; private set; }

	public IAccount Account { get; private set; }

	public IFuturemud Gameworld { get; }

	/// <summary>
	///     This function passes through a command to the relevant controllable object, which will return the
	///     string results, and if there is a context change, a reference to the new context.
	/// </summary>
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

	void IAccountController.BindAccount(IAccount loginAccount)
	{
		if (loginAccount.ControllingContext != null)
		{
			if (loginAccount.ControllingContext != this)
			{
				loginAccount.ControllingContext.DetachConnection();
				Console.WriteLine($"Connection closed due to account {loginAccount.Name} being logged into from another connection.");
			}
		}

		if (Account != null)
		{
			if (Account.ControllingContext == this && Account == loginAccount)
			{
				return;
			}

			Console.WriteLine("Account " + Account.Name + " has logged out.");
		}

		Account = loginAccount;
		Account.Register(this);
		_connection?.NegotiateClientSet();
	}

	public void Close()
	{
		Account?.Register(default);
		_connection?.PrepareOutgoing();
		_connection?.SendOutgoing();
		Closing = true;
		_connection?.Dispose(); // SendOutgoing will dispose a closing connection
	}

	void IAccountController.DetachConnection()
	{
		if (_connection == null)
		{
			return;
		}

		_connection.State = ConnectionState.Closing;
		_connection = null;
		Account?.Gameworld.SystemMessage($"Account {Account.Name.Proper()} has disconnected.", true);
		var characterContext = _context as ICharacter;
		if (characterContext?.IsGuest ?? false)
		{
			characterContext.AddEffect(new LinkdeadLogout(characterContext), TimeSpan.FromSeconds(1));
		}
		else
		{
			characterContext?.OutputHandler.Handle(
				new EmoteOutput(new Emote("@ have|has gone linkdead.", characterContext),
					flags: OutputFlags.SuppressObscured));
			characterContext?.AddEffect(new LinkdeadLogout(characterContext), TimeSpan.FromMinutes(10));
		}
	}

	public void AddObservee(IMonitorable observee)
	{
		if (_observes.Contains(observee))
		{
			return;
		}

		_observes.Add(observee);
		observee.AddObserver(this);
	}

	// TODO Reimplement
	public void UpdateObservers()
	{
		//if (_observedBy.Count <= 0) return;
		//var sb = new StringBuilder(OutgoingStream.ToString());
		//sb.Insert(0, "\nWatch: ", 1);
		//sb.AppendLine("\nEnd Watch");
		//_observedBy.ForEach(x => x.OutgoingStream.Append(sb.ToString()));
	}

	void IMonitorable.AddObserver(IMonitor observer)
	{
		if (!_observedBy.Contains(observer))
		{
			_observedBy.Add(observer);
		}
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

	void IMonitorable.RemoveObserver(IMonitor observer)
	{
		if (_observedBy.Contains(observer))
		{
			_observedBy.Remove(observer);
		}
	}

	public void Dispose()
	{
		Account = null;
		_context?.LoseControl(this);
		_context = null;
		foreach (var observee in _observes)
		{
			RemoveObservee(observee);
		}
	}

	public void Register(IOutputHandler handler)
	{
		OutputHandler = handler;
		Actor?.Register(handler);
	}

	public void SetContext(IControllable context)
	{
		_context?.LoseControl(this);
		_context = context;
		_context?.AssumeControl(this);
	}

	public string LDescAdditionalTags
	{
		get
		{
			if (_connection == null || _connection.State != ConnectionState.Open)
			{
				return " (link dead)".Colour(Telnet.Red);
			}

			// Idle after 10 minutes
			return _connection.InactivityMilliseconds > 600000 ? " (idle)".Colour(Telnet.Red) : "";
		}
	}

	public long InactivityMilliseconds => _connection?.InactivityMilliseconds ?? 0L;

	public bool Closing { get; private set; }

	public ICharacter Actor { get; private set; }

	public int Timeout => _context?.Timeout ?? 1000 * 60 * 60;

	public string IPAddress => _connection?.IP ?? "n/a";

	public void UpdateControlFocus(ICharacter newFocus)
	{
		Actor = newFocus;
	}

	public void CuePrompt()
	{
		if (_context?.HasPrompt ?? false)
		{
			OutputHandler.Send(_context.Prompt, false);
		}
	}

	public void Quit()
	{
		Console.WriteLine("Account {0} has quit.", Account.Name);
		Dispose();
	}

	public void CloseSubContext()
	{
		throw new NotImplementedException();
	}
}
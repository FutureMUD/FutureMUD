using System;
using MudSharp.Framework;
using MudSharp.Network;
using MudSharp.PerceptionEngine;

namespace MudSharp.Menus;

public abstract class Menu : IControllable, IHaveFuturemud, ISubContextController
{
	protected string _menuText = "";

	protected Menu()
	{
	}

	protected Menu(string text, IFuturemud gameworld)
	{
		_menuText = text;
		Gameworld = gameworld;
	}

	public IOutputHandler OutputHandler { get; protected set; }

	public abstract int Timeout { get; }

	public void Register(IOutputHandler handler)
	{
		OutputHandler = handler;
	}

	public IFuturemud Gameworld { get; protected set; }

	public virtual void Close()
	{
		_nextContext = new Quit();
	}

	public virtual void CloseSubContext()
	{
		throw new NotImplementedException();
	}

	public override string ToString()
	{
		return _menuText;
	}

	// This is an abstract class interface enforcement.

	#region IControllable Members

	protected IControllable _subContext;
	IControllable IControllable.SubContext => _subContext;

	protected IControllable _nextContext;
	IControllable IControllable.NextContext => _nextContext;

	public virtual bool HandleSubContext(string command)
	{
		if (_subContext == null)
		{
			return false;
		}

		return _subContext.HandleSubContext(command) || _subContext.ExecuteCommand(command);
	}

	public virtual bool HasPrompt => false;

	public virtual string Prompt => "";


	public abstract void AssumeControl(IController controller);
	public abstract void SilentAssumeControl(IController controller);
	public abstract bool ExecuteCommand(string command);
	public abstract void LoseControl(IController controller);

	public void SetContext(IControllable context)
	{
		// Do nothing
	}

	public IController Controller { get; protected set; }

	#endregion
}
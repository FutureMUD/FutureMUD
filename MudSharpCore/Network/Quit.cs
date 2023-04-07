using System;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Handlers;

namespace MudSharp.Network;

/// <summary>
///     This class creates a context that immediately informs the ControlContext to dispose all connection data.
/// </summary>
public class Quit : IControllable
{
	IControllable IControllable.SubContext { get; } = null;

	IControllable IControllable.NextContext { get; } = null;

	public int Timeout => 0;

	bool IControllable.HasPrompt => false;

	string IControllable.Prompt => throw new NotSupportedException();

	public IOutputHandler OutputHandler => new NonPlayerOutputHandler();

	public void AssumeControl(IController controller)
	{
		Controller = controller;
		controller.Close();
		(Controller as ICharacterController)?.UpdateControlFocus(null);
	}

	public void SilentAssumeControl(IController controller)
	{
		AssumeControl(controller);
	}

	public bool ExecuteCommand(string command)
	{
		return false;
	}

	public void LoseControl(IController controller)
	{
	}

	public bool HandleSubContext(string command)
	{
		return false;
	}

	public void Register(IOutputHandler handler)
	{
	}

	public IController Controller { get; protected set; }
}
#nullable enable


namespace MudSharp.Character;

public sealed class PossessionVictimContext : IControllable
{
	private readonly string _startMessage;
	private readonly string _endMessage;

	public PossessionVictimContext(ICharacter victim, ICharacter possessor, string startMessage, string endMessage)
	{
		Victim = victim;
		Possessor = possessor;
		_startMessage = startMessage;
		_endMessage = endMessage;
	}

	public ICharacter Victim { get; }
	public ICharacter Possessor { get; }
	public IControllable SubContext => null!;
	public IControllable NextContext => null!;
	public IController Controller { get; private set; } = null!;
	public IOutputHandler OutputHandler => Controller.OutputHandler;
	public bool HasPrompt => true;
	public string Prompt => "\n[possessed]> ";
	public int Timeout => 1000 * 60 * 60;

	public void Register(IOutputHandler handler)
	{
	}

	public bool HandleSubContext(string command)
	{
		return false;
	}

	public void AssumeControl(IController controller)
	{
		SilentAssumeControl(controller);
		SendStatus(_startMessage);
	}

	public void SilentAssumeControl(IController controller)
	{
		Controller = controller;
		if (Controller is ICharacterController characterController)
		{
			characterController.UpdateControlFocus(Victim);
		}
	}

	public bool ExecuteCommand(string command)
	{
		if (command.EqualToAny("quit", "logout", "exit"))
		{
			Controller.Close();
			return true;
		}

		if (command.EqualToAny("look", "l", "status"))
		{
			SendStatus("You are bound as a spectator while another will commands your body.");
			return true;
		}

		SendStatus("You are bound as a spectator and cannot command your body until the possession ends.");
		return true;
	}

	public void LoseControl(IController controller)
	{
		if (!string.IsNullOrWhiteSpace(_endMessage))
		{
			controller.OutputHandler.Send(_endMessage.SubstituteANSIColour());
		}

		if (Controller is ICharacterController characterController)
		{
			characterController.UpdateControlFocus(null!);
		}

		Controller = null!;
	}

	private void SendStatus(string message)
	{
		if (!string.IsNullOrWhiteSpace(message))
		{
			Controller.OutputHandler.Send(message.SubstituteANSIColour());
		}
	}
}

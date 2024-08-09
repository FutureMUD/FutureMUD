using System.Linq;
using MudSharp.Accounts;
using MudSharp.Framework;
using MudSharp.Menus;

namespace MudSharp.CharacterCreation;

public class ChargenMenu : Menu, IChargenMenu
{
	private readonly IAccount _account;
	private readonly IChargen _chargen;

	public ChargenMenu(IAccount account, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_account = account;
		_chargen = new Chargen(this, gameworld, account);
	}

	public ChargenMenu(MudSharp.Models.Chargen existing, IAccount account, IFuturemud gameworld)
	{
		_chargen = new Chargen(existing, this, gameworld, account);
		_account = account;
		Gameworld = gameworld;
	}

	public override int Timeout => 60000 * 30;

	public override bool HasPrompt => true;

	public override string Prompt
	{
		get
		{
			if (_chargen.CurrentCosts.Any(x => x.Value > 0))
			{
				return
					$"\nCost: {_chargen.CurrentCosts.Where(x => x.Value > 0).Select(x => $"{x.Value.ToString("N0", _account)}/{_chargen.Account.AccountResources[x.Key].ToString("N0", _account)} {x.Key.Alias}".Colour(x.Value < 0 ? Telnet.Red : Telnet.Green)).ListToString()}>\n\n";
			}

			return $"\n>\n\n";
		}
	}

	public string Display()
	{
		return _chargen.Display();
	}

	public override void AssumeControl(IController controller)
	{
		OutputHandler = controller.OutputHandler;
		_chargen.ControlReturned();
		_nextContext = null;
	}

	#region Overrides of Menu

	public override void SilentAssumeControl(IController controller)
	{
		OutputHandler = controller.OutputHandler;
		_nextContext = null;
	}

	#endregion

	public override bool ExecuteCommand(string command)
	{
		OutputHandler.Send(_chargen.HandleCommand(command), nopage: true);

		if (_chargen.State == ChargenState.Submitted || _chargen.State == ChargenState.Deleted ||
		    _chargen.State == ChargenState.Halt)
		{
			_nextContext = new LoggedInMenu(_account, Gameworld);
		}

		return true;
	}

	public void MenuSetContext(IControllable context)
	{
		_nextContext = context;
	}

	public override void LoseControl(IController controller)
	{
		OutputHandler = null;
	}
}
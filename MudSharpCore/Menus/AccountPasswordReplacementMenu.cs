#nullable enable
using System;
using MudSharp.Accounts;
using MudSharp.Framework;

namespace MudSharp.Menus;

/// <summary>Restricted replacement permission: no character/account menu access before committing the password.</summary>
internal sealed class AccountPasswordReplacementMenu : Menu
{
	private readonly AccountPasswordPermission _permission;
	private readonly IControllable _returnContext;
	private readonly string _reason;
	private readonly Func<bool>? _stillAuthorised;
	private string? _confirmationHash;
	private long _confirmationSalt;
	private bool _pending;

	internal AccountPasswordReplacementMenu(IFuturemud world, AccountPasswordPermission permission,
		IControllable returnContext, string reason, Func<bool>? stillAuthorised = null) : base(string.Empty, world)
	{
		_permission = permission;
		_returnContext = returnContext;
		_reason = reason;
		_stillAuthorised = stillAuthorised;
	}

	public override int Timeout => 5 * 60 * 1000;
	public override bool IsSensitiveInput => true;
	public override bool AwaitingAccountSecurityWork => _pending;
	public override void AssumeControl(IController controller)
	{
		Controller = controller;
		OutputHandler = controller.OutputHandler;
		OutputHandler.Send("Enter the new account password, or 0 to cancel. Use 15 to 128 characters; spaces are allowed.");
	}
	public override void SilentAssumeControl(IController controller) { Controller = controller; OutputHandler = controller.OutputHandler; }
	public override void LoseControl(IController controller) { OutputHandler = null!; _confirmationHash = null; }
	internal override void SecurityWorkFailed()
	{
		OutputHandler?.Send("The password was not confirmed as changed. Please retry the workflow.");
		_nextContext = _returnContext;
	}

	public override bool ExecuteCommand(string command)
	{
		if (command == "0") { _confirmationHash = null; _nextContext = _returnContext; return true; }
		if (_pending) { OutputHandler.Send("Please wait for the password operation to finish."); return true; }
		if (_permission.ExpiresUtc <= DateTime.UtcNow || _stillAuthorised?.Invoke() == false)
		{
			OutputHandler.Send("The password replacement permission expired. Start the workflow again.");
			_nextContext = _returnContext;
			return true;
		}
		if (_confirmationHash is null)
		{
			if (!AccountPasswordPolicy.TryValidate(command, out var reason)) { OutputHandler.Send(reason); return true; }
			_confirmationSalt = SecurityUtilities.GetSalt64();
			_confirmationHash = SecurityUtilities.GetPasswordHash(command, _confirmationSalt);
			OutputHandler.Send("Enter the new password again:");
			return true;
		}
		if (!SecurityUtilities.VerifyPassword(command, _confirmationHash, _confirmationSalt))
		{
			_confirmationHash = null;
			OutputHandler.Send("The passwords did not match. Enter a new password:");
			return true;
		}
		_pending = true;
		if (!AccountSecurityWorkQueue.TryQueue(this, () => AccountSecurityService.Hasher.Hash(command), hash =>
		    {
			    _pending = false;
			    _confirmationHash = null;
			    var player = Controller as IPlayerController;
			    var changed = _stillAuthorised?.Invoke() != false && AccountSecurityService.ReplacePassword(Gameworld,
				    _permission, hash, Controller as IAccountController, player?.IPAddress ?? "implementor", _reason);
			    OutputHandler.Send(changed ? "Password updated. Use the new password on your next login." :
				    "The permission is no longer valid or the credentials changed elsewhere. Start the workflow again.");
			    _nextContext = _returnContext;
		    }))
		{
			_pending = false;
			OutputHandler.Send("Account security is busy. Reenter the confirmation shortly.");
		}
		return true;
	}
}

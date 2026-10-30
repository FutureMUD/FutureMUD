#nullable enable

using System;
using MudSharp.Character;

namespace MudSharp.Economy;

public static class BankAccountAccessRules
{
	public static bool CanPerform(IBankAccount account, ICharacter actor, string operation) =>
		account.IsAuthorisedAccountUser(actor) ||
		operation.Equals("deposit", StringComparison.OrdinalIgnoreCase) && account.IsAuthorisedAccountDepositor(actor);
}

using Microsoft.EntityFrameworkCore.Metadata;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Economy.Banking;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Combat.Arenas
{
	internal class CombatArena : SaveableItem, ICombatArena
	{
		public override void Save()
		{
			// TODO on DB stuff

			Changed = false;
		}

		public override string FrameworkItemType => "CombatArena";

		private readonly List<ICell> _arenaCells = new();
		public IEnumerable<ICell> ArenaCells => _arenaCells;

		private readonly List<ICell> _stagingCells = new();
		public IEnumerable<ICell> StagingCells => _stagingCells;

		private readonly List<ICell> _stableCells = new();
		public IEnumerable<ICell> StableCells => _stableCells;

		private readonly List<ICell> _spectatorCells = new();
		public IEnumerable<ICell> SpectatorCells => _spectatorCells;

		private readonly List<IArenaCombatantType> _arenaCombatantTypes = new();
		public IEnumerable<IArenaCombatantType> ArenaCombatantTypes => _arenaCombatantTypes;

		private readonly List<IArenaMatchType> _arenaMatchTypes = new();
		public IEnumerable<IArenaMatchType> ArenaMatchTypes => _arenaMatchTypes;

		private readonly List<IArenaCombatantProfile> _arenaCombatantProfiles = new();
		public IEnumerable<IArenaCombatantProfile> ArenaCombatantProfiles => _arenaCombatantProfiles;

		private readonly List<IArenaMatchBet> _arenaMatchBets = new();

		private readonly List<IArenaMatch> _arenaMatches = new();

		private IArenaMatch? _activeMatch;

		public ICurrency? Currency { get; private set; }
		public decimal CashBalance { get; private set; }
		public IBankAccount? BankAccount { get; private set; }

		private readonly HashSet<long> _managerIDs = new();
		public bool IsManager(ICharacter actor)
		{
			return _managerIDs.Contains(actor.Id);
		}



		#region Building
		public const string BuildingHelp = @"";

		public bool BuildingCommand(ICharacter actor, StringStack command)
		{
			switch (command.PopSpeech().ToLowerInvariant().CollapseString()) {
				case "name":
					return BuildingCommandName(actor, command);
				case "bank":
				case "bankaccount":
				case "account":
					return BuildingCommandBankAccount(actor, command);
			}

			actor.OutputHandler.Send(BuildingHelp.SubstituteANSIColour());
			return false;
		}

		private bool BuildingCommandBankAccount(ICharacter actor, StringStack command)
		{
			if (command.IsFinished)
			{
				actor.OutputHandler.Send(
					$"You must either specify a bank account or use {"none".ColourCommand()} to remove a bank account.");
				return false;
			}

			if (command.PeekSpeech().EqualToAny("none", "remove", "delete", "clear"))
			{
				BankAccount = null;
				Changed = true;
				actor.OutputHandler.Send("This combat arena will no longer use any bank account to back its transactions.");
				return true;
			}

			var (account, error) = Bank.FindBankAccount(command.SafeRemainingArgument, null, actor);
			if (account is null)
			{
				actor.OutputHandler.Send(error);
				return false;
			}

			BankAccount = account;
			Changed = true;
			actor.OutputHandler.Send(
				$"This combat arena will now use the bank account {account.AccountReference.ColourName()} to back its financial transactions.");
			return true;
		}

		private bool BuildingCommandName(ICharacter actor, StringStack command)
		{
			if (command.IsFinished)
			{
				actor.OutputHandler.Send("What do you want to rename this arena to?");
				return false;
			}

			var name = command.SafeRemainingArgument.TitleCase();
			if (Gameworld.CombatArenas.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send("There is already a combat arena with that name. Names must be unique.");
				return false;
			}

			actor.OutputHandler.Send($"You rename the combat arena {Name.ColourName()} to {name.ColourName()}.");
			_name = name;
			Changed = true;
			return true;
		}

		public string Show(ICharacter actor)
		{
			var sb = new StringBuilder();
			sb.AppendLine($"Combat Arena #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, Telnet.BoldRed, Telnet.BoldWhite));
			return sb.ToString();
		}
		#endregion
	}
}

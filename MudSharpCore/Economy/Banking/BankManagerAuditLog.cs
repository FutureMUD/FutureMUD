using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.TimeAndDate;

namespace MudSharp.Economy.Banking;

public class BankManagerAuditLog : LateInitialisingItem, IBankManagerAuditLog
{
	public BankManagerAuditLog(Models.BankManagerAuditLog dbitem, IBank bank)
	{
		Gameworld = bank.Gameworld;
		_id = dbitem.Id;
		IdInitialised = true;
		Bank = bank;
		DateTime = new MudDateTime(dbitem.DateTime, Gameworld);
		Detail = dbitem.Detail;
		_characterId = dbitem.CharacterId;
	}

	public BankManagerAuditLog(IBank bank, ICharacter character, MudDateTime datetime, string detail)
	{
		Gameworld = bank.Gameworld;
		Bank = bank;
		DateTime = datetime;
		Detail = detail;
		_characterId = character.Id;
		_character = character;
		Gameworld.SaveManager.AddInitialisation(this);
	}

	public IBank Bank { get; init; }
	public MudDateTime DateTime { get; init; }
	public string Detail { get; init; }
	private long _characterId;
	private ICharacter _character;

	public ICharacter Character
	{
		get
		{
			if (_character == null)
			{
				_character = Gameworld.TryGetCharacter(_characterId, true);
			}

			return _character;
		}
	}

	public override void Save()
	{
		throw new ApplicationException("BankManagerAuditLogs should not be saved after creation");
	}

	public override object DatabaseInsert()
	{
		var dbitem = new Models.BankManagerAuditLog
		{
			BankId = Bank.Id,
			CharacterId = _characterId,
			DateTime = DateTime.GetDateTimeString(),
			Detail = Detail
		};
		FMDB.Context.BankManagerAuditLogs.Add(dbitem);
		return dbitem;
	}

	public override void SetIDFromDatabase(object dbitem)
	{
		_id = ((Models.BankManagerAuditLog)dbitem).Id;
	}

	public override string FrameworkItemType => "BankManagerAuditLog";
}
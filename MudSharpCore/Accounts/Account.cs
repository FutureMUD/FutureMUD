using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MudSharp.Character;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Database;
using MudSharp.Email;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.Models;
using TimeZoneConverter;

namespace MudSharp.Accounts;

public sealed class Account : SaveableItem, IAccount
{
	private static readonly object _creationMutex = new();

	private System.Globalization.CultureInfo _culture;

	private int _innerLineFormatLength;

	private int _lineFormatLength;

	private int _pageLength;

	private System.TimeZoneInfo _timeZone;

	private string _unitPreference;

	private bool _useMccp;

	private bool _useMsp;

	private bool _useUnicode;

	private bool _appendNewlinesBetweenMultipleEchoesPerPrompt = true; // TODO save


	public Account(MudSharp.Models.Account account, IFuturemud gameworld, IAuthority authority = null)
	{
		Gameworld = gameworld;
		AccountResources = new Dictionary<IChargenResource, int>();
		AccountResourcesLastAwarded = new Dictionary<IChargenResource, DateTime?>();
		_name = account.Name.Proper();
		_id = account.Id;
		ControllingContext = null;
		ActLawfully = account.ActLawfully;
		Authority = authority;
		ActiveCharactersAllowed = account.ActiveCharactersAllowed;
		LastIP = account.LastLoginIp;
		PreviousLastLoginTime = account.LastLoginTime ?? DateTime.MinValue;
		LastLoginTime = account.LastLoginTime ?? DateTime.MinValue;
		CreationDate = account.CreationDate;
		EmailAddress = account.Email;
		UseMSP = account.UseMsp;
		UseMCCP = account.UseMccp;
		UseUnicode = account.UseUnicode;
		LineFormatLength = account.FormatLength;
		InnerLineFormatLength = account.InnerFormatLength;
		PageLength = account.PageLength;
		TimeZone = TZConvert.GetTimeZoneInfo(account.TimeZoneId);
		Culture = System.Globalization.CultureInfo.GetCultureInfo(account.CultureName);
		UnitPreference = account.UnitPreference;
		IsRegistered = account.IsRegistered;
		AccountStatus = (AccountStatus)account.AccessStatus;
		PromptType = (PromptType)account.PromptType;
		TabRoomDescriptions = account.TabRoomDescriptions;
		CodedRoomDescriptionAdditionsOnNewLine = account.CodedRoomDescriptionAdditionsOnNewLine;
		AppendNewlinesBetweenMultipleEchoesPerPrompt = account.AppendNewlinesBetweenMultipleEchoesPerPrompt;
		CharacterNameOverlaySetting = (CharacterNameOverlaySetting)account.CharacterNameOverlaySetting;

		foreach (var resource in Gameworld.ChargenResources)
		{
			var dbres = account.AccountsChargenResources.FirstOrDefault(x => x.ChargenResourceId == resource.Id);
			if (dbres == null)
			{
				AccountResources[resource] = 0;
				AccountResourcesLastAwarded[resource] = null;
			}
			else
			{
				AccountResources[resource] = dbres.Amount;
				AccountResourcesLastAwarded[resource] = dbres.LastAwardDate;
			}
		}
	}

	public override string FrameworkItemType => "Account";

	public AccountStatus AccountStatus { get; set; }

	public IAll<ICharacter> Characters { get; } = new All<ICharacter>();

	public Dictionary<IChargenResource, int> AccountResources { get; }
	public Dictionary<IChargenResource, DateTime?> AccountResourcesLastAwarded { get; }

	public IAccountController ControllingContext { get; private set; }

	public IOutputHandler OutputHandler => ControllingContext?.OutputHandler;

	private bool _actLawfully = true;

	public bool ActLawfully
	{
		get => _actLawfully;
		set
		{
			_actLawfully = value;
			Changed = true;
		}
	}

	public IAuthority Authority { get; private set; }

	public int ActiveCharactersAllowed { get; set; }

	public string LastIP { get; }

	public DateTime PreviousLastLoginTime { get; set; }

	public DateTime LastLoginTime { get; set; }

	public DateTime CreationDate { get; }

	public string EmailAddress { get; set; }

	public bool UseUnicode
	{
		get => _useUnicode;
		set
		{
			_useUnicode = value;
			Changed = true;
		}
	}

	public bool UseMSP
	{
		get => _useMsp;
		set
		{
			_useMsp = value;
			Changed = true;
		}
	}

	public bool UseMCCP
	{
		get => _useMccp;
		set
		{
			_useMccp = value;
			Changed = true;
		}
	}

	public int LineFormatLength
	{
		get => _lineFormatLength;
		set
		{
			_lineFormatLength = value;
			Changed = true;
		}
	}

	public int InnerLineFormatLength
	{
		get => _innerLineFormatLength;
		set
		{
			_innerLineFormatLength = value;
			Changed = true;
		}
	}

	public int PageLength
	{
		get => _pageLength;
		set
		{
			_pageLength = value;
			Changed = true;
		}
	}

	public bool AppendNewlinesBetweenMultipleEchoesPerPrompt
	{
		get => _appendNewlinesBetweenMultipleEchoesPerPrompt;
		set
		{
			_appendNewlinesBetweenMultipleEchoesPerPrompt = value;
			Changed = true;
		}
	}

	private CharacterNameOverlaySetting _characterNameOverlaySetting = CharacterNameOverlaySetting.None;

	public CharacterNameOverlaySetting CharacterNameOverlaySetting
	{
		get => _characterNameOverlaySetting;
		set
		{
			_characterNameOverlaySetting = value;
			Changed = true;
		}
	}

	private bool _safeMoveMode;

	public bool SafeMoveMode
	{
		get => _safeMoveMode;
		set
		{
			_safeMoveMode = value;
			Changed = true;
		}
	}

	public System.TimeZoneInfo TimeZone
	{
		get => _timeZone;
		set
		{
			_timeZone = value;
			Changed = true;
		}
	}

	public System.Globalization.CultureInfo Culture
	{
		get => _culture;
		set
		{
			_culture = value;
			Changed = true;
		}
	}

	public string UnitPreference
	{
		get => _unitPreference;
		set
		{
			_unitPreference = value;
			Changed = true;
		}
	}

	private PromptType _promptType;

	public PromptType PromptType
	{
		get => _promptType;
		set
		{
			_promptType = value;
			Changed = true;
		}
	}

	private bool _tabRoomDescriptions;

	public bool TabRoomDescriptions
	{
		get => _tabRoomDescriptions;
		set
		{
			_tabRoomDescriptions = value;
			Changed = true;
		}
	}

	private bool _codedRoomDescriptionAdditionsOnNewLine;

	public bool CodedRoomDescriptionAdditionsOnNewLine
	{
		get => _codedRoomDescriptionAdditionsOnNewLine;
		set
		{
			_codedRoomDescriptionAdditionsOnNewLine = value;
			Changed = true;
		}
	}

	public bool IsRegistered { get; private set; }

	public bool TryAccountRegistration(string text)
	{
		using (new FMDB())
		{
			var dbaccount = FMDB.Context.Accounts.Find(Id);
			if (dbaccount == null)
			{
				return false;
			}

			if (text.Equals(dbaccount.RegistrationCode, StringComparison.InvariantCultureIgnoreCase))
			{
				dbaccount.IsRegistered = true;
				FMDB.Context.SaveChanges();
				IsRegistered = true;
				return true;
			}

			return false;
		}
	}

	public bool Register(IAccountController controller)
	{
		ControllingContext = controller;
		return true;
	}

	public void Register(IOutputHandler handler)
	{
	}

	public void SetAccountAuthority(IAuthority newAuthority)
	{
		Authority = newAuthority;
	}

	#region IDisposable Members

	public void Dispose()
	{
		Gameworld.Destroy(this);
	}

	#endregion

	#region IFormatProvider Members

	public object GetFormat(Type formatType)
	{
		return Culture.GetFormat(formatType);
	}

	#endregion

	public override void Save()
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.Accounts.Find(Id);
			dbitem.FormatLength = LineFormatLength;
			dbitem.InnerFormatLength = InnerLineFormatLength;
			dbitem.CultureName = Culture.Name;
			dbitem.TimeZoneId = TimeZone.Id;
			dbitem.UseUnicode = UseUnicode;
			dbitem.PageLength = PageLength;
			dbitem.UnitPreference = UnitPreference;
			dbitem.UseMsp = UseMSP;
			dbitem.UseMccp = UseMCCP;
			dbitem.PageLength = PageLength;
			dbitem.PromptType = (int)PromptType;
			dbitem.CodedRoomDescriptionAdditionsOnNewLine = CodedRoomDescriptionAdditionsOnNewLine;
			dbitem.TabRoomDescriptions = TabRoomDescriptions;
			dbitem.CharacterNameOverlaySetting = (int)CharacterNameOverlaySetting;
			dbitem.AppendNewlinesBetweenMultipleEchoesPerPrompt = AppendNewlinesBetweenMultipleEchoesPerPrompt;
			dbitem.ActLawfully = ActLawfully;
			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}

	public static bool Create(string name, string password, long salt, string culture, string timezone, bool unicode,
		int linewidth, string email, string unitPreference, string IP)
	{
		var nameLower = name.ToLowerInvariant().Trim();

		lock (_creationMutex)
		{
			using (new FMDB())
			{
				if (FMDB.Context.Accounts.Any(acc => acc.Name == nameLower))
				{
					return false;
				}

				var newAccount = new MudSharp.Models.Account
				{
					Name = name.ToLowerInvariant(),
					Password = password,
					Salt = salt,
					FormatLength = linewidth,
					InnerFormatLength = 80,
					PageLength = 50,
					CultureName = culture,
					TimeZoneId = timezone,
					UseUnicode = unicode,
					Email = email,
					IsRegistered = false,
					UnitPreference = unitPreference,
					ActiveCharactersAllowed = 1,
					ActLawfully = true,
					CreationDate = DateTime.UtcNow,
					RegistrationCode = SecurityUtilities.GetRandomString(8,
						Constants.ValidRandomCharacters.ToCharArray()),
					AuthorityGroup =
						FMDB.Context.AuthorityGroups.First(x => x.AuthorityLevel == (int)PermissionLevel.Player)
				};

				var ipLog = new LoginIp
				{
					Account = newAccount,
					IpAddress = IP,
					FirstDate = newAccount.CreationDate
				};

				EmailHelper.Instance.SendEmail(EmailTemplateTypes.NewAccountVerification, email, name.Proper(),
					newAccount.RegistrationCode);

				FMDB.Context.Accounts.Add(newAccount);
				FMDB.Context.LoginIps.Add(ipLog);

				if (FMDB.Context.SaveChanges() > 0)
				{
					return true;
				}
			}

			return false;
		}
	}
}
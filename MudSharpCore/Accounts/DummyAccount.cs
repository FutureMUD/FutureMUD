using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MudSharp.Character;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.PerceptionEngine;

namespace MudSharp.Accounts;

public sealed class DummyAccount : FrameworkItem, IAccount
{
	private DummyAccount()
	{
		AccountResources = new Dictionary<IChargenResource, int>();
		AccountResourcesLastAwarded = new Dictionary<IChargenResource, DateTime?>();
	}

	public static DummyAccount Instance { get; } = new();
	public override string FrameworkItemType => "DummyAccount";

	public AccountStatus AccountStatus => AccountStatus.Normal;

	#region IDisposable Members

	public void Dispose()
	{
	}

	#endregion

	#region IHaveFuturemud Members

	public IFuturemud Gameworld { get; private set; }

	#endregion

	#region IFormatProvider Members

	public object GetFormat(Type formatType)
	{
		return Culture.GetFormat(formatType);
	}

	#endregion

	public void SetupGameworld(IFuturemud gameworld)
	{
		Gameworld = gameworld;
		Authority = Gameworld.Authorities.Get("Player").First();
		foreach (var resource in Gameworld.ChargenResources)
		{
			AccountResources[resource] = 0;
			AccountResourcesLastAwarded[resource] = null;
		}
	}

	#region IAccount Members

	public IAll<ICharacter> Characters { get; } = new All<ICharacter>();

	public string UnitPreference
	{
		get { return Gameworld.UnitManager.Units.Select(x => x.System).First(); }
		set { }
	}

	public Dictionary<IChargenResource, int> AccountResources { get; }
	public Dictionary<IChargenResource, DateTime?> AccountResourcesLastAwarded { get; }

	public IAccountController ControllingContext => null;

	public IAuthority Authority { get; set; }

	public int ActiveCharactersAllowed
	{
		get => 0;
		set { }
	}

	public bool ActLawfully
	{
		get => false;
		set { }
	}

	public string LastIP => "127.0.0.1";

	public DateTime PreviousLastLoginTime
	{
		get => DateTime.MinValue;
		set { }
	}

	public DateTime LastLoginTime
	{
		get => DateTime.MinValue;
		set { }
	}

	public DateTime CreationDate => DateTime.MinValue;

	public string EmailAddress { get; set; }

	public bool IsRegistered => true;

	public bool TryAccountRegistration(string text)
	{
		return false;
	}

	public bool UseUnicode
	{
		get => false;
		set { }
	}

	public bool UseMSP
	{
		get => false;
		set { }
	}

	public bool UseMCCP
	{
		get => false;
		set { }
	}

	public int LineFormatLength
	{
		get => 110;
		set { }
	}

	public int InnerLineFormatLength
	{
		get => 80;
		set { }
	}

	public int PageLength
	{
		get => int.MaxValue;
		set { }
	}

	public bool AppendNewlinesBetweenMultipleEchoesPerPrompt
	{
		get => false;
		set { }
	}

	public CharacterNameOverlaySetting CharacterNameOverlaySetting
	{
		get => CharacterNameOverlaySetting.None;
		set { }
	}

	public bool SafeMoveMode
	{
		get => false;
		set { }
	}

	public bool Register(IAccountController controller)
	{
		return true;
	}

	public TimeZoneInfo TimeZone
	{
		get => TimeZoneInfo.Local;
		set { }
	}

	public CultureInfo Culture
	{
		get => CultureInfo.InvariantCulture;
		set { }
	}

	public void SetAccountAuthority(IAuthority newAuthority)
	{
		// Do nothing
	}

	public PromptType PromptType
	{
		get => PromptType.Full;
		set
		{
			// Do nothing
		}
	}

	public bool TabRoomDescriptions
	{
		get => true;
		set
		{
			// Do nothing
		}
	}

	public bool CodedRoomDescriptionAdditionsOnNewLine
	{
		get => true;
		set
		{
			// Do nothing
		}
	}

	#endregion

	#region IHandleOutput Members

	public IOutputHandler OutputHandler => null;

	public void Register(IOutputHandler handler)
	{
	}

	#endregion

	#region ISaveable Members

	public bool Changed { get; set; }

	public void Save()
	{
		// Do nothing
	}

	#endregion
}
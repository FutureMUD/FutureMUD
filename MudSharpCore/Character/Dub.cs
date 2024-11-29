using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Models;

namespace MudSharp.Character;

public class Dub : SaveableItem, IDub
{
	private string _introducedName;

	public Dub(MudSharp.Models.Dub dub, ICharacter owner, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		Id = dub.Id;
		Owner = owner;
		TargetId = dub.TargetId;
		TargetType = dub.TargetType;
		Keywords = dub.Keywords.Split(' ').ToList();
		LastDescription = dub.LastDescription;
		LastUsage = dub.LastUsage;
		_introducedName = dub.IntroducedName;
		// TODO - save/load identity concealed
	}

	public override string Name => Keywords.First();

	public ICharacter Owner { get; set; }
	public long TargetId { get; set; }
	public string TargetType { get; set; }
	public IList<string> Keywords { get; }
	public string LastDescription { get; set; }
	public DateTime LastUsage { get; set; }
	public bool WasIdentityConcealed { get; set; }

	public string IntroducedName
	{
		get => _introducedName;
		set
		{
			_introducedName = value;
			Changed = true;
		}
	}

	public override string FrameworkItemType => "Dub";

	IEnumerable<string> IKeyworded.Keywords => Keywords;

	public override void Save()
	{
		var dbitem = FMDB.Context.Dubs.Find(Id);
		dbitem.Keywords = Keywords.ListToString(separator: " ", conjunction: "");
		dbitem.LastUsage = LastUsage;
		dbitem.LastDescription = LastDescription;
		dbitem.IntroducedName = IntroducedName;
		Changed = false;
	}

	public string HowSeen(ICharacter actor)
	{
		LastUsage = DateTime.UtcNow;
		Changed = true;
		if (!string.IsNullOrEmpty(IntroducedName))
		{
			switch (actor.Account.CharacterNameOverlaySetting)
			{
				case Accounts.CharacterNameOverlaySetting.AppendWithBrackets:
					return $"{LastDescription} {IntroducedName.Parentheses().Colour(Telnet.BoldWhite)}";
				case Accounts.CharacterNameOverlaySetting.Replace:
					return IntroducedName;
			}
		}
		else
		{
			switch (actor.Account.CharacterNameOverlaySetting)
			{
				case Accounts.CharacterNameOverlaySetting.AppendWithBrackets:
					return $"{LastDescription} {Name.TitleCase().Parentheses().Colour(Telnet.BoldWhite)}";
				case Accounts.CharacterNameOverlaySetting.Replace:
					return Name.TitleCase();
			}
		}

		return LastDescription;
	}
}
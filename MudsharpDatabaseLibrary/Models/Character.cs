using System;
using System.Collections.Generic;

namespace MudSharp.Models;

public partial class Character
{
	public Character()
	{
		ActiveProjects = new HashSet<ActiveProject>();
		ActiveJobs = new HashSet<ActiveJob>();
		AlliesAlly = new HashSet<Ally>();
		AlliesCharacter = new HashSet<Ally>();
		CharacterCombatSettings = new HashSet<CharacterCombatSetting>();
		CharacterKnowledges = new HashSet<CharacterKnowledge>();
		CharacterLog = new HashSet<CharacterLog>();
		CharactersAccents = new HashSet<CharacterAccent>();
		CharactersChargenRoles = new HashSet<CharactersChargenRoles>();
		CharactersLanguages = new HashSet<CharactersLanguages>();
		CharactersMagicResources = new HashSet<CharactersMagicResources>();
		CharactersScripts = new HashSet<CharactersScripts>();
		ClanMembershipsCharacter = new HashSet<ClanMembership>();
		ClanMembershipsManager = new HashSet<ClanMembership>();
		Clans = new HashSet<Clan>();
		CrimesAccuser = new HashSet<Crime>();
		CrimesCriminal = new HashSet<Crime>();
		CrimesVictim = new HashSet<Crime>();
		Drawings = new HashSet<Drawing>();
		DreamsAlreadyDreamt = new HashSet<DreamsAlreadyDreamt>();
		DreamsCharacters = new HashSet<DreamsCharacters>();
		Dubs = new HashSet<Dub>();
		HooksPerceivables = new HashSet<HooksPerceivable>();
		NpcsBodyguardCharacter = new HashSet<Npc>();
		NpcsCharacter = new HashSet<Npc>();
		PerceiverMerits = new HashSet<PerceiverMerit>();
		Wounds = new HashSet<Wound>();
		WritingsAuthor = new HashSet<Writing>();
		WritingsTrueAuthor = new HashSet<Writing>();
		Patrols = new HashSet<Patrol>();
		GPTMessages = new HashSet<GPTMessage>();
	}

	public string Name { get; set; }
	public long Id { get; set; }
	public long? AccountId { get; set; }
	public DateTime CreationTime { get; set; }
	public DateTime? DeathTime { get; set; }
	public int Status { get; set; }
	public int State { get; set; }
	public short Gender { get; set; }
	public long Location { get; set; }
	public long BodyId { get; set; }
	public long CultureId { get; set; }
	public string EffectData { get; set; }
	public string BirthdayDate { get; set; }
	public long BirthdayCalendarId { get; set; }
	public bool IsAdminAvatar { get; set; }
	public long? CurrencyId { get; set; }
	public int TotalMinutesPlayed { get; set; }
	public double AlcoholLitres { get; set; }
	public double WaterLitres { get; set; }
	public double FoodSatiatedHours { get; set; }
	public double DrinkSatiatedHours { get; set; }
	public double Calories { get; set; }
	public string NeedsModel { get; set; }
	public string LongTermPlan { get; set; }
	public string ShortTermPlan { get; set; }
	public bool ShownIntroductionMessage { get; set; }
	public string IntroductionMessage { get; set; }
	public long? ChargenId { get; set; }
	public long? CurrentCombatSettingId { get; set; }
	public int PreferredDefenseType { get; set; }
	public int PositionId { get; set; }
	public int PositionModifier { get; set; }
	public long? PositionTargetId { get; set; }
	public string PositionTargetType { get; set; }
	public string PositionEmote { get; set; }
	public long? CurrentLanguageId { get; set; }
	public long? CurrentAccentId { get; set; }
	public long? CurrentWritingLanguageId { get; set; }
	public int WritingStyle { get; set; }
	public long? CurrentScriptId { get; set; }
	public int DominantHandAlignment { get; set; }
	public DateTime? LastLoginTime { get; set; }
	public bool CombatBrief { get; set; }
	public bool RoomBrief { get; set; }
	public DateTime? LastLogoutTime { get; set; }
	public string Outfits { get; set; }
	public long? CurrentProjectLabourId { get; set; }
	public long? CurrentProjectId { get; set; }
	public double CurrentProjectHours { get; set; }
	public string NameInfo { get; set; }
	public int RoomLayer { get; set; }
	public bool NoMercy { get; set; }

	public virtual Account Account { get; set; }
	public virtual Body Body { get; set; }
	public virtual Chargen Chargen { get; set; }
	public virtual Culture Culture { get; set; }
	public virtual Currency Currency { get; set; }
	public virtual Accent CurrentAccent { get; set; }
	public virtual Language CurrentLanguage { get; set; }
	public virtual ActiveProject CurrentProject { get; set; }
	public virtual ProjectLabourRequirement CurrentProjectLabour { get; set; }
	public virtual Script CurrentScript { get; set; }
	public virtual Language CurrentWritingLanguage { get; set; }
	public virtual Cell LocationNavigation { get; set; }
	public virtual Guest Guest { get; set; }
	public virtual ICollection<ActiveProject> ActiveProjects { get; set; }
	public virtual ICollection<ActiveJob> ActiveJobs { get; set; }
	public virtual ICollection<Ally> AlliesAlly { get; set; }
	public virtual ICollection<Ally> AlliesCharacter { get; set; }
	public virtual ICollection<CharacterCombatSetting> CharacterCombatSettings { get; set; }
	public virtual ICollection<CharacterKnowledge> CharacterKnowledges { get; set; }
	public virtual ICollection<CharacterLog> CharacterLog { get; set; }
	public virtual ICollection<CharacterAccent> CharactersAccents { get; set; }
	public virtual ICollection<CharactersChargenRoles> CharactersChargenRoles { get; set; }
	public virtual ICollection<CharactersLanguages> CharactersLanguages { get; set; }
	public virtual ICollection<CharactersMagicResources> CharactersMagicResources { get; set; }
	public virtual ICollection<CharactersScripts> CharactersScripts { get; set; }
	public virtual ICollection<ClanMembership> ClanMembershipsCharacter { get; set; }
	public virtual ICollection<ClanMembership> ClanMembershipsManager { get; set; }
	public virtual ICollection<Clan> Clans { get; set; }
	public virtual ICollection<Crime> CrimesAccuser { get; set; }
	public virtual ICollection<Crime> CrimesCriminal { get; set; }
	public virtual ICollection<Crime> CrimesVictim { get; set; }
	public virtual ICollection<Drawing> Drawings { get; set; }
	public virtual ICollection<DreamsAlreadyDreamt> DreamsAlreadyDreamt { get; set; }
	public virtual ICollection<DreamsCharacters> DreamsCharacters { get; set; }
	public virtual ICollection<Dub> Dubs { get; set; }
	public virtual ICollection<HooksPerceivable> HooksPerceivables { get; set; }
	public virtual ICollection<Npc> NpcsBodyguardCharacter { get; set; }
	public virtual ICollection<Npc> NpcsCharacter { get; set; }
	public virtual ICollection<PerceiverMerit> PerceiverMerits { get; set; }
	public virtual ICollection<Wound> Wounds { get; set; }
	public virtual ICollection<Writing> WritingsAuthor { get; set; }
	public virtual ICollection<Writing> WritingsTrueAuthor { get; set; }
	public virtual ICollection<Patrol> Patrols { get; set; }
	public virtual ICollection<LegalAuthorityFine> Fines { get; set; }
	public virtual ICollection<GPTMessage> GPTMessages { get; set; }
}
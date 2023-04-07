using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Account
    {
        public Account()
        {
            AccountNotesAccount = new HashSet<AccountNote>();
            AccountNotesAuthor = new HashSet<AccountNote>();
            AccountsChargenResources = new HashSet<AccountsChargenResources>();
            Bans = new HashSet<Ban>();
            ChannelIgnorers = new HashSet<ChannelIgnorer>();
            CharacterLog = new HashSet<CharacterLog>();
            Characters = new HashSet<Character>();
            ChargenRoles = new HashSet<ChargenRole>();
            ChargenRolesApprovers = new HashSet<ChargenRolesApprovers>();
            Chargens = new HashSet<Chargen>();
            LoginIps = new HashSet<LoginIp>();
        }

        public string Name { get; set; }
        public long Id { get; set; }
        public string Password { get; set; }
        public long Salt { get; set; }
        public int AccessStatus { get; set; }
        public long? AuthorityGroupId { get; set; }
        public string Email { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public string LastLoginIp { get; set; }
        public int FormatLength { get; set; }
        public int InnerFormatLength { get; set; }
        public bool UseMxp { get; set; }
        public bool UseMsp { get; set; }
        public bool UseMccp { get; set; }
        public int ActiveCharactersAllowed { get; set; }
        public bool UseUnicode { get; set; }
        public string TimeZoneId { get; set; }
        public string CultureName { get; set; }
        public string RegistrationCode { get; set; }
        public bool IsRegistered { get; set; }
        public string RecoveryCode { get; set; }
        public string UnitPreference { get; set; }
        public DateTime CreationDate { get; set; }
        public int PageLength { get; set; }
        public int PromptType { get; set; }
        public bool TabRoomDescriptions { get; set; }
        public bool CodedRoomDescriptionAdditionsOnNewLine { get; set; }
        public int CharacterNameOverlaySetting { get; set; }
        public bool AppendNewlinesBetweenMultipleEchoesPerPrompt { get; set; }
        public bool ActLawfully { get; set; }
        public bool HasBeenActiveInWeek { get; set; }

        public virtual AuthorityGroup AuthorityGroup { get; set; }
        public virtual ICollection<AccountNote> AccountNotesAccount { get; set; }
        public virtual ICollection<AccountNote> AccountNotesAuthor { get; set; }
        public virtual ICollection<AccountsChargenResources> AccountsChargenResources { get; set; }
        public virtual ICollection<Ban> Bans { get; set; }
        public virtual ICollection<ChannelIgnorer> ChannelIgnorers { get; set; }
        public virtual ICollection<CharacterLog> CharacterLog { get; set; }
        public virtual ICollection<Character> Characters { get; set; }
        public virtual ICollection<ChargenRole> ChargenRoles { get; set; }
        public virtual ICollection<ChargenRolesApprovers> ChargenRolesApprovers { get; set; }
        public virtual ICollection<Chargen> Chargens { get; set; }
        public virtual ICollection<LoginIp> LoginIps { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using MudSharp.Character;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Health;
using MudSharp.PerceptionEngine;

namespace MudSharp.Accounts {
    public enum AccountStatus {
        Normal = 0,
        Suspended
    }
    
    public enum CharacterStatus
    {
        Creating = 0,
        Submitted = 1,
        Active = 2,
        Deceased = 3,
        Suspended = 4,
        Retired = 5
    }

    public static class AccountExtensions {
        public static string Describe(this CharacterStatus status) {
            switch (status) {
                case CharacterStatus.Active:
                    return "Active";
                case CharacterStatus.Creating:
                    return "Creating";
                case CharacterStatus.Deceased:
                    return "Deceased";
                case CharacterStatus.Retired:
                    return "Retired";
                case CharacterStatus.Submitted:
                    return "Submitted";
                case CharacterStatus.Suspended:
                    return "Suspended";
                default:
                    throw new NotSupportedException();
            }
        }
    }

    public enum ApprovalRisk {
        None = 0,
        Low = 1,
        Medium = 2,
        High = 3,
        Extreme = 4,
        Ridiculous = 8
    }

    public enum ApprovalLevel {
        None = 0,
        Newbie = 1,
        Player = 2,
        AdminCharacter = 3,
        SeniorAdminCharacter = 4
    }

    public enum PermissionLevel {
        Any = -1,
        Guest = 0,
        NPC = 1,
        Player = 2,
        Guide = 3,
        JuniorAdmin = 4,
        Admin = 5,
        SeniorAdmin = 6,
        HighAdmin = 7,
        Founder = 8,
        Inaccessible = 9
    }

    [Flags]
    public enum Permissions {
        None = 0x0,
        Read = 0x1,
        Write = 0x2,
        Approve = 0x4,
        Founder = 0x8,
        Commit = Write | Founder,
        Approval = (Read & Approve) | Founder
    }

    public enum CharacterNameOverlaySetting
    {
        None = 0,
        AppendWithBrackets = 1,
        Replace = 2
    }

    public interface IHaveAccount {
        IAccount Account { get; set; }
    }

    public interface IAccount : IFrameworkItem, IFormatProvider, IHandleOutput, IDisposable, ISaveable
    {
        IAll<ICharacter> Characters { get; }

        AccountStatus AccountStatus { get; }

        IAccountController ControllingContext { get; }

        IAuthority Authority { get; }

        int ActiveCharactersAllowed { get; set; }

        string LastIP { get; }

        DateTime LastLoginTime { get; set; }
        DateTime PreviousLastLoginTime { get; set; }

        DateTime CreationDate { get; }

        string EmailAddress { get; set; }
        bool AppendNewlinesBetweenMultipleEchoesPerPrompt { get; set; }

        bool UseUnicode { get; set; }

        bool UseMSP { get; set; }

        bool UseMCCP { get; set; }

        int LineFormatLength { get; set; }

        int InnerLineFormatLength { get; set; }

        int PageLength { get; set; }

        TimeZoneInfo TimeZone { get; set; }

        CultureInfo Culture { get; set; }

        string UnitPreference { get; set; }

        bool IsRegistered { get; }

        Dictionary<IChargenResource, int> AccountResources { get; }
        Dictionary<IChargenResource, DateTime?> AccountResourcesLastAwarded { get; }

        bool TryAccountRegistration(string text);

        bool Register(IAccountController controller);

        void SetAccountAuthority(IAuthority newAuthority);

        PromptType PromptType { get; set; }

        bool TabRoomDescriptions { get; set; }
        bool CodedRoomDescriptionAdditionsOnNewLine { get; set; }
        bool SafeMoveMode { get; set; }
        CharacterNameOverlaySetting CharacterNameOverlaySetting { get; set; }
        bool ActLawfully { get; set; }
    }
}
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.Disfigurements;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Character.Name;
using MudSharp.CharacterCreation.Resources;
using MudSharp.CharacterCreation.Roles;
using MudSharp.CharacterCreation.Screens;
using MudSharp.Communication.Language;
using MudSharp.Construction;
using MudSharp.Editor;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Knowledge;
using MudSharp.RPG.Merits;
using MudSharp.TimeAndDate.Date;

namespace MudSharp.CharacterCreation
{
    
    public enum DiscordRequestType
    {
        Show,
        Approve,
        Reject
    }

    public enum ApplicationType
    {
        Normal,
        Simple,
        Special
    }

    public interface IChargen : IFrameworkItem, IHaveFuturemud, ICharacterTemplate, IHaveMerits, IHaveAccount
    {
        ApplicationType ApplicationType { get; set; }
        bool CanSubmit { get; }
        IEnumerable<Tuple<string, string>> PriorRejections { get; }
        IEnumerable<ChargenStage> CompletedStages { get; }
        IChargenScreen CurrentScreen { get; }
        IChargenMenu Menu { get; set; }
        List<Tuple<string, string>> SelectedNotes { get; set; }
        ChargenStage Stage { get; }
        ChargenState State { get; }
        [CanBeNull] IAccount ApprovedBy { get; }
        StartingLocation StartingLocation { get; set; }
        PermissionLevel MinimumApprovalAuthority { get; }
        IReadOnlyDictionary<IChargenResource, int> CurrentCosts { get; }
        Dictionary<IChargenResource, int> ApplicationCosts { get; }
        bool ApplicationLocked { get; }
        Dictionary<ITraitDefinition, int> SelectedSkillBoosts { get; set; }
        Dictionary<IChargenResource, int> SelectedSkillBoostCosts { get; set; }
        IEnumerable<IChargenAdvice> AllAdvice { get; }
        void RecalculateCurrentCosts();
        string Display();

        /// <summary>
        ///     Locks the application and prevents anybody from reviewing it
        /// </summary>
        void LockApplication();

        /// <summary>
        ///     Releases any previous held application locks
        /// </summary>
        void ReleaseApplication();

        string DisplayForReviewForDiscord(IAccount character, PermissionLevel permission);
        string DisplayForReview(IAccount character, PermissionLevel permission);
        long ApproveApplicationExternal();
        long ApproveApplication(ICharacter approver, IAccount approverAccount, string comment, IOutputHandler handler);
        void RejectApplication(ICharacter rejecter, IAccount rejecterAccount, string comment, IOutputHandler handler);
        string HandleCommand(string command);
        void ResetStage(ChargenStage stage);
        void SetEditor(EditorController controller);
        void SetStage(ChargenStage stage);
        void ControlReturned();

        new List<IAccent> SelectedAccents { get; set; }
        new List<ITrait> SelectedAttributes { get; set; }
        new MudDate SelectedBirthday { get; set; }
        new List<(ICharacteristicDefinition, ICharacteristicValue)> SelectedCharacteristics { get; set; }
        new ICulture SelectedCulture { get; set; }
        new List<IEntityDescriptionPattern> SelectedEntityDescriptionPatterns { get; set; }
        new IEthnicity SelectedEthnicity { get; set; }
        new string SelectedFullDesc { get; set; }
        new Gender SelectedGender { get; set; }
        new double SelectedHeight { get; set; }
        new IPersonalName SelectedName { get; set; }
        new IRace SelectedRace { get; set; }
        new string SelectedSdesc { get; set; }
        new List<ITraitDefinition> SelectedSkills { get; set; }
        new List<(ITraitDefinition, double)> SkillValues { get; set; }
        new double SelectedWeight { get; set; }
        new List<IChargenRole> SelectedRoles { get; set; }
        new IAccount Account { get; set; }
        new List<ICharacterMerit> SelectedMerits { get; set; }
        new List<IKnowledge> SelectedKnowledges { get; set; }
        new Alignment Handedness { get; set; }
        new List<IBodypart> MissingBodyparts { get; set; }
        new List<(IDisfigurementTemplate Disfigurement, IBodypart Bodypart)> SelectedDisfigurements { get; set; }
        new List<IGameItemProto> SelectedProstheses { get; set; }
    }
}
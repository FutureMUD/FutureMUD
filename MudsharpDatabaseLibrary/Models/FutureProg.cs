using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class FutureProg
    {
        public FutureProg()
        {
            AppointmentsAbbreviations = new HashSet<AppointmentsAbbreviations>();
            AppointmentsTitles = new HashSet<AppointmentsTitles>();
            ButcheryProducts = new HashSet<ButcheryProducts>();
            ChannelsChannelListenerProg = new HashSet<Channel>();
            ChannelsChannelSpeakerProg = new HashSet<Channel>();
            CharacterCombatSettings = new HashSet<CharacterCombatSetting>();
            CharacterIntroTemplates = new HashSet<CharacterIntroTemplate>();
            CharacteristicValues = new HashSet<CharacteristicValue>();
            ChargenAdvices = new HashSet<ChargenAdvice>();
            ChargenRoles = new HashSet<ChargenRole>();
            Clans = new HashSet<Clan>();
            CombatMessages = new HashSet<CombatMessage>();
            CraftsAppearInCraftsListProg = new HashSet<Craft>();
            CraftsCanUseProg = new HashSet<Craft>();
            CraftsOnUseProgCancel = new HashSet<Craft>();
            CraftsOnUseProgComplete = new HashSet<Craft>();
            CraftsOnUseProgStart = new HashSet<Craft>();
            CraftsWhyCannotUseProg = new HashSet<Craft>();
            CulturesAvailabilityProg = new HashSet<Culture>();
            CulturesSkillStartingValueProg = new HashSet<Culture>();
            CurrencyDescriptionPatterns = new HashSet<CurrencyDescriptionPattern>();
            DefaultHooks = new HashSet<DefaultHook>();
            DreamsCanDreamProg = new HashSet<Dream>();
            DreamsOnDreamProg = new HashSet<Dream>();
            DreamsOnWakeDuringDreamingProg = new HashSet<Dream>();
            EntityDescriptionPatterns = new HashSet<EntityDescriptionPattern>();
            Ethnicities = new HashSet<Ethnicity>();
            FutureProgsParameters = new HashSet<FutureProgsParameter>();
            GameItemProtosOnLoadProgs = new HashSet<GameItemProtosOnLoadProgs>();
            Helpfiles = new HashSet<Helpfile>();
            HelpfilesExtraTexts = new HashSet<HelpfilesExtraText>();
            KnowledgesCanAcquireProg = new HashSet<Knowledge>();
            KnowledgesCanLearnProg = new HashSet<Knowledge>();
            Laws = new HashSet<Law>();
            LegalClasses = new HashSet<LegalClass>();
            ProgSchedules = new HashSet<ProgSchedule>();
            RaceButcheryProfilesCanButcherProg = new HashSet<RaceButcheryProfile>();
            RaceButcheryProfilesWhyCannotButcherProg = new HashSet<RaceButcheryProfile>();
            RacesAttributeBonusProg = new HashSet<Race>();
            RacesAvailabilityProg = new HashSet<Race>();
            RanksAbbreviations = new HashSet<RanksAbbreviations>();
            RanksTitles = new HashSet<RanksTitle>();
            ShopsCanShopProg = new HashSet<Shop>();
            ShopsWhyCannotShopProg = new HashSet<Shop>();
            Socials = new HashSet<Social>();
            SurgicalProcedurePhases = new HashSet<SurgicalProcedurePhase>();
            SurgicalProceduresAbortProg = new HashSet<SurgicalProcedure>();
            SurgicalProceduresCompletionProg = new HashSet<SurgicalProcedure>();
            SurgicalProceduresUsabilityProg = new HashSet<SurgicalProcedure>();
            SurgicalProceduresWhyCannotUseProg = new HashSet<SurgicalProcedure>();
            Tags = new HashSet<Tag>();
            TraitDefinitionsAvailabilityProg = new HashSet<TraitDefinition>();
            TraitDefinitionsLearnableProg = new HashSet<TraitDefinition>();
            TraitDefinitionsTeachableProg = new HashSet<TraitDefinition>();
            WeaponAttacks = new HashSet<WeaponAttack>();
            WitnessProfilesIdentityKnownProg = new HashSet<WitnessProfile>();
            WitnessProfilesReportingMultiplierProg = new HashSet<WitnessProfile>();
            EnforcementAuthorities = new HashSet<EnforcementAuthority>();
            PatrolRoutes = new HashSet<PatrolRoute>();
        }

        public long Id { get; set; }
        public string FunctionName { get; set; }
        public string FunctionComment { get; set; }
        public string FunctionText { get; set; }
        public long ReturnType { get; set; }
        public string Category { get; set; }
        public string Subcategory { get; set; }
        public bool Public { get; set; }
        public bool AcceptsAnyParameters { get; set; }
        public int StaticType { get; set; }

        public virtual ICollection<AppointmentsAbbreviations> AppointmentsAbbreviations { get; set; }
        public virtual ICollection<AppointmentsTitles> AppointmentsTitles { get; set; }
        public virtual ICollection<ButcheryProducts> ButcheryProducts { get; set; }
        public virtual ICollection<Channel> ChannelsChannelListenerProg { get; set; }
        public virtual ICollection<Channel> ChannelsChannelSpeakerProg { get; set; }
        public virtual ICollection<CharacterCombatSetting> CharacterCombatSettings { get; set; }
        public virtual ICollection<CharacterIntroTemplate> CharacterIntroTemplates { get; set; }
        public virtual ICollection<CharacteristicValue> CharacteristicValues { get; set; }
        public virtual ICollection<ChargenAdvice> ChargenAdvices { get; set; }
        public virtual ICollection<ChargenRole> ChargenRoles { get; set; }
        public virtual ICollection<Clan> Clans { get; set; }
        public virtual ICollection<CombatMessage> CombatMessages { get; set; }
        public virtual ICollection<Craft> CraftsAppearInCraftsListProg { get; set; }
        public virtual ICollection<Craft> CraftsCanUseProg { get; set; }
        public virtual ICollection<Craft> CraftsOnUseProgCancel { get; set; }
        public virtual ICollection<Craft> CraftsOnUseProgComplete { get; set; }
        public virtual ICollection<Craft> CraftsOnUseProgStart { get; set; }
        public virtual ICollection<Craft> CraftsWhyCannotUseProg { get; set; }
        public virtual ICollection<Culture> CulturesAvailabilityProg { get; set; }
        public virtual ICollection<Culture> CulturesSkillStartingValueProg { get; set; }
        public virtual ICollection<CurrencyDescriptionPattern> CurrencyDescriptionPatterns { get; set; }
        public virtual ICollection<DefaultHook> DefaultHooks { get; set; }
        public virtual ICollection<Dream> DreamsCanDreamProg { get; set; }
        public virtual ICollection<Dream> DreamsOnDreamProg { get; set; }
        public virtual ICollection<Dream> DreamsOnWakeDuringDreamingProg { get; set; }
        public virtual ICollection<EntityDescriptionPattern> EntityDescriptionPatterns { get; set; }
        public virtual ICollection<Ethnicity> Ethnicities { get; set; }
        public virtual ICollection<FutureProgsParameter> FutureProgsParameters { get; set; }
        public virtual ICollection<GameItemProtosOnLoadProgs> GameItemProtosOnLoadProgs { get; set; }
        public virtual ICollection<Helpfile> Helpfiles { get; set; }
        public virtual ICollection<HelpfilesExtraText> HelpfilesExtraTexts { get; set; }
        public virtual ICollection<Knowledge> KnowledgesCanAcquireProg { get; set; }
        public virtual ICollection<Knowledge> KnowledgesCanLearnProg { get; set; }
        public virtual ICollection<Law> Laws { get; set; }
        public virtual ICollection<LegalClass> LegalClasses { get; set; }
        public virtual ICollection<ProgSchedule> ProgSchedules { get; set; }
        public virtual ICollection<RaceButcheryProfile> RaceButcheryProfilesCanButcherProg { get; set; }
        public virtual ICollection<RaceButcheryProfile> RaceButcheryProfilesWhyCannotButcherProg { get; set; }
        public virtual ICollection<Race> RacesAttributeBonusProg { get; set; }
        public virtual ICollection<Race> RacesAvailabilityProg { get; set; }
        public virtual ICollection<RanksAbbreviations> RanksAbbreviations { get; set; }
        public virtual ICollection<RanksTitle> RanksTitles { get; set; }
        public virtual ICollection<Shop> ShopsCanShopProg { get; set; }
        public virtual ICollection<Shop> ShopsWhyCannotShopProg { get; set; }
        public virtual ICollection<Social> Socials { get; set; }
        public virtual ICollection<SurgicalProcedurePhase> SurgicalProcedurePhases { get; set; }
        public virtual ICollection<SurgicalProcedure> SurgicalProceduresAbortProg { get; set; }
        public virtual ICollection<SurgicalProcedure> SurgicalProceduresCompletionProg { get; set; }
        public virtual ICollection<SurgicalProcedure> SurgicalProceduresUsabilityProg { get; set; }
        public virtual ICollection<SurgicalProcedure> SurgicalProceduresWhyCannotUseProg { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }
        public virtual ICollection<TraitDefinition> TraitDefinitionsAvailabilityProg { get; set; }
        public virtual ICollection<TraitDefinition> TraitDefinitionsLearnableProg { get; set; }
        public virtual ICollection<TraitDefinition> TraitDefinitionsTeachableProg { get; set; }
        public virtual ICollection<WeaponAttack> WeaponAttacks { get; set; }
        public virtual ICollection<WitnessProfile> WitnessProfilesIdentityKnownProg { get; set; }
        public virtual ICollection<WitnessProfile> WitnessProfilesReportingMultiplierProg { get; set; }
        public virtual ICollection<EnforcementAuthority> EnforcementAuthorities { get; set; }
        public virtual ICollection<PatrolRoute> PatrolRoutes { get; set; }
    }
}

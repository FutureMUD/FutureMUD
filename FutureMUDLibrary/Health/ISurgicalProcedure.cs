using System;
using System.Collections.Generic;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.GameItems.Inventory;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Knowledge;

namespace MudSharp.Health {
    public enum SurgicalProcedureType {
        /// <summary>
        ///     A physical, albeit cursory examination of a patient's medical condition
        /// </summary>
        Triage,

        /// <summary>
        ///     A longer, detailed physical examination of a patient's medical condition
        /// </summary>
        DetailedExamination,

        /// <summary>
        ///     Stitching up and repairing surgical access from another procedure
        /// </summary>
        InvasiveProcedureFinalisation,

        /// <summary>
        ///     Exploratory internal surgery to determine the source of an internal trauma
        /// </summary>
        ExploratorySurgery,

        /// <summary>
        ///     Permanent removal of a bodypart
        /// </summary>
        Amputation,

        /// <summary>
        ///     Reattachment of a previously removed bodypart
        /// </summary>
        Replantation,

        /// <summary>
        ///     Installing a Cannula for use in an IV setup
        /// </summary>
        Cannulation,
        TraumaControl,
        OrganExtraction,
        OrganTransplant,
        Decannulation,
        OrganStabilisation,
        SurgicalBoneSetting,
        InstallImplant,
        RemoveImplant,
        ConfigureImplantPower,
        ConfigureImplantInterface
    }

    public class SurgicalProcedurePhase {
        public required TimeSpan BaseLength { get; init; }
        public required string PhaseEmote { get; set; }
        public required IFutureProg OnPhaseProg { get; set; }
        public required IInventoryPlanTemplate InventoryPlanTemplate { get; set; }
        public required string PhaseSpecialEffects { get; set; }
        public string PhaseSpecialEffectsDescription { get; set; } = string.Empty;

        public Func<ICharacter,ICharacter,object[], bool> PhaseSuccessful { get; set; } = (surgeon,patient,paramaters) => true;
        public Func<ICharacter, ICharacter, object[], string> WhyPhaseNotSuccessful { get; set; } = (surgeon, patient, paramaters) => "of an unknown reason";
    }

    public interface ISurgicalProcedure : IEditableItem {
        string ProcedureDescription { get; }
        SurgicalProcedureType Procedure { get; }
        CheckType Check { get; }
        ITraitDefinition CheckTrait { get; }

        double BaseCheckBonus { get; }

        /// <summary>
        ///     e.g. "Medical Examination", "Amputation"
        /// </summary>
        string ProcedureName { get; }

        /// <summary>
        ///     e.g. "Examining", "Amputating"
        /// </summary>
        string ProcedureGerund { get; }

        string ProcedureBeginEmote { get;}

        IEnumerable<SurgicalProcedurePhase> Phases { get; }
        IKnowledge KnowledgeRequired { get; }
        string MedicalSchool { get; }
        IFutureProg UsabilityProg { get; }
        IFutureProg WhyCannotUseProg { get; }
        IFutureProg CompletionProg { get; }
        IFutureProg AbortProg { get; }
        bool RequiresUnconsciousPatient { get; }
        bool RequiresInvasiveProcedureFinalisation { get; }
        bool RequiresLivingPatient { get; }
		IBodyPrototype TargetBodyType { get; }
		void PerformProcedure(ICharacter surgeon, ICharacter patient, params object[] additionalArguments);

        void CompleteProcedure(ICharacter surgeon, ICharacter patient, CheckOutcome result,
                               params object[] additionalArguments);

        void AbortProcedure(ICharacter surgeon, ICharacter patient, Outcome result, params object[] additionalArguments);
        IEffect GetActionEffect(ICharacter surgeon, ICharacter patient, params object[] additionalArguments);
        Difficulty GetProcedureDifficulty(ICharacter surgeon, ICharacter patient, params object[] additionalArguments);
        bool CanPerformProcedure(ICharacter surgeon, ICharacter patient, params object[] additionalArguments);
        string WhyCannotPerformProcedure(ICharacter surgeon, ICharacter patient, params object[] additionalArguments);
        string DescribeProcedureGerund(ICharacter surgeon, ICharacter patient, params object[] additionalArguments);

        string DressPhaseEmote(string emote, ICharacter surgeon, ICharacter patient,
                               params object[] additionalArguments);
    }
}
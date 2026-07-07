using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.FutureProg;
using System.Collections.Generic;

namespace MudSharp.Work.Projects
{
    public interface IActiveProject : IFrameworkItem, IProgVariable
    {
        ICharacter CharacterOwner { get; }
        IProject ProjectDefinition { get; }
        IProjectPhase CurrentPhase { get; }
        ICell Location { get; }
        IReadOnlyDictionary<IProjectLabourRequirement, double> LabourProgress { get; }
        IReadOnlyDictionary<IProjectMaterialRequirement, double> MaterialProgress { get; }
        ICurrency PaymentCurrency { get; }
        decimal CashBalance { get; }
        IReadOnlyDictionary<IProjectLabourRequirement, decimal> LabourPaymentRates { get; }
        IReadOnlyDictionary<IProjectMaterialRequirement, decimal> MaterialPaymentRates { get; }
        void Cancel(ICharacter actor);
        bool FulfilLabour(IProjectLabourRequirement labour, double progress);
        void FulfilMaterial(IProjectMaterialRequirement material, double progress);
        decimal LabourPaymentRateFor(IProjectLabourRequirement labour);
        decimal MaterialPaymentRateFor(IProjectMaterialRequirement material);
        void SetPaymentCurrency(ICurrency currency);
        void SetLabourPaymentRate(IProjectLabourRequirement labour, decimal amount);
        void SetMaterialPaymentRate(IProjectMaterialRequirement material, decimal amount);
        bool DepositFunds(ICharacter actor, decimal amount, out string error);
        bool WithdrawFunds(ICharacter actor, decimal amount, out string error);
        bool CanPayLabourContribution(IProjectLabourRequirement labour, double hours, out string error);
        bool CanPayMaterialContribution(IProjectMaterialRequirement material, double progress, out string error);
        decimal AwardLabourPayment(ICharacter actor, IProjectLabourRequirement labour, double hours);
        decimal PayMaterialContribution(ICharacter actor, IProjectMaterialRequirement material, double progress);
        IEnumerable<(ICharacter Character, IProjectLabourRequirement Labour)> ActiveLabour { get; }
        bool HasFreeLabourSlot(IProjectLabourRequirement labour);
        bool HasDisplaceableNpcWorker(ICharacter actor, IProjectLabourRequirement labour);
        bool CanJoinLabour(ICharacter actor, IProjectLabourRequirement labour);
        bool TryJoinLabour(ICharacter actor, IProjectLabourRequirement labour, bool allowNpcDisplacement,
            out ICharacter displacedWorker);
        IReadOnlyCollection<ICharacter> RemoveWorkersFromLabour(IProjectLabourRequirement labour);
        void Join(ICharacter actor, IProjectLabourRequirement labour);
        void Leave(ICharacter actor);
        string ProjectsCommandOutput(ICharacter actor);
        void DoProjectsTick();
        string ShowToPlayer(ICharacter actor);
    }
}

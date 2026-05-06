using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Intervals;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Community
{
    public interface IClan : IFrameworkItem, ISaveable, IProgVariable, IHaveMultipleNames
    {
        bool IsTemplate { get; set; }
        string Alias { get; set; }
        string FullName { get; set; }
        string Description { get; set; }
        bool ShowClanMembersInWho { get; set; }
        string Sphere { get; set; }
        bool ShowFamousMembersInNotables { get; set; }
        ulong? DiscordChannelId { get; set; }

        IEnumerable<ICell> TreasuryCells { get; }
        IEnumerable<ICell> AdministrationCells { get; }
        void AddTreasuryCell(ICell cell);
        void RemoveTreasuryCell(ICell cell);
        void AddAdministrationCell(ICell cell);
        void RemoveAdministrationCell(ICell cell);

        IBankAccount ClanBankAccount { get; set; }

        List<IRank> Ranks { get; }
        List<IAppointment> Appointments { get; }
        List<IClanMembership> Memberships { get; }
        List<IPaygrade> Paygrades { get; }
        List<IClanBudget> Budgets { get; }
        List<IExternalClanControl> ExternalControls { get; }

        RecurringInterval PayInterval { get; set; }
        MudDateTime NextPay { get; set; }

        /// <summary>
        ///     If not null, the Paymaster must be present for pay to be collected
        /// </summary>
        ICharacter Paymaster { get; set; }

        /// <summary>
        ///     If not null, an instance of the paymaster item proto must exist in the location for pay to be collected
        /// </summary>
        IGameItemProto PaymasterItemProto { get; set; }
        int? MaximumPeriodsOfUncollectedBackPay { get; set; }

        /// <summary>
        ///     Executed when someone collects pay in this clan
        /// </summary>
        IFutureProg OnPayProg { get; set; }

        ICalendar Calendar { get; set; }

        void FinaliseLoad(MudSharp.Models.Clan clan, IEnumerable<Models.ClanMembership> memberships);
        void SetRank(IClanMembership membership, IRank newRank);
        void SetPaygrade(IClanMembership membership, IPaygrade newPaygrade);
        void RemoveMembership(IClanMembership membership);
        void DismissAppointment(IClanMembership membership, IAppointment appointment);
        void Show(ICharacter actor, StringStack command);
        void ShowMembers(ICharacter actor);
        string DescribeElections(ICharacter actor);
        void ShowElectionHistory(ICharacter actor, StringStack command);
        void Nominate(ICharacter actor, StringStack command);
        void WithdrawNomination(ICharacter actor, StringStack command);
        void Vote(ICharacter actor, StringStack command);
        void Appoint(ICharacter actor, StringStack command);
        void Dismiss(ICharacter actor, StringStack command);
        void SubmitControl(ICharacter actor, StringStack command);
        void ReleaseControl(ICharacter actor, StringStack command);
        void TransferControl(ICharacter actor, StringStack command);
        void SetControllingAppointment(ICharacter actor, StringStack command);
        void AppointExternal(ICharacter actor, StringStack command);
        void DismissExternal(ICharacter actor, StringStack command);
        void BudgetCommand(ICharacter actor, StringStack command);
        void ShowBalanceSheet(ICharacter actor);
        void ShowPayrollHistory(ICharacter actor, StringStack command);

        bool FreePosition(IAppointment appointment);
        bool FreePosition(IAppointment appointment, IClan liegeClan);

        void Disband(ICharacter disbander);

        void DeleteRank(IRank rank);
        void DeleteAppointment(IAppointment appointment);
        void DeletePaygrade(IPaygrade paygrade);
    }

    public static class ClanExtensions
    {
        public static IClan GetClan<T>(this T items, string targetText) where T : IEnumerable<IClan>
        {
            List<IClan> itemList = items.ToList();
            return itemList.FirstOrDefault(x => x.Name.EqualTo(targetText) || x.FullName.EqualTo(targetText) || x.Alias.EqualTo(targetText)) ??
                   itemList.FirstOrDefault(x =>
                            x.Name.StartsWith(targetText, StringComparison.InvariantCultureIgnoreCase) ||
                            x.FullName.StartsWith(targetText, StringComparison.InvariantCultureIgnoreCase) ||
                            x.Alias.StartsWith(targetText, StringComparison.InvariantCultureIgnoreCase)
                        );
        }
    }
}

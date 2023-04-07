using System;
using System.Collections.Generic;
using System.Linq;
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

namespace MudSharp.Community {
    public interface IClan : IFrameworkItem, ISaveable, IFutureProgVariable {
        bool IsTemplate { get; set; }
        string Alias { get; set; }
        string FullName { get; set; }
        string Description { get; set; }
        bool ShowClanMembersInWho { get; set; }
        string Sphere { get; set; }
        bool ShowFamousMembersInNotables { get; set; }
        ulong? DiscordChannelId { get; set; }

        List<ICell> TreasuryCells { get; }
        List<ICell> AdministrationCells { get; }
        IBankAccount ClanBankAccount { get; set; }

        List<IRank> Ranks { get; }
        List<IAppointment> Appointments { get; }
        List<IClanMembership> Memberships { get; }
        List<IPaygrade> Paygrades { get; }
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

        bool FreePosition(IAppointment appointment);
        bool FreePosition(IAppointment appointment, IClan liegeClan);

        void Disband(ICharacter disbander);
    }

    public static class ClanExtensions
    {
	    public static IClan GetClan<T>(this T items, string targetText) where T : IEnumerable<IClan>
	    {
		    var itemList = items.ToList();
		    return itemList.FirstOrDefault(x => x.Name.EqualTo(targetText) || x.FullName.EqualTo(targetText) || x.Alias.EqualTo(targetText)) ??
		           itemList.FirstOrDefault(x =>
							x.Name.StartsWith(targetText, StringComparison.InvariantCultureIgnoreCase) ||
							x.FullName.StartsWith(targetText, StringComparison.InvariantCultureIgnoreCase) ||
							x.Alias.StartsWith(targetText, StringComparison.InvariantCultureIgnoreCase)
						);
	    }
    }
}
using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.TimeAndDate.Date;

namespace MudSharp.Community
{
    public interface IClanMembership : ISaveable, IFrameworkItem
    {
        long MemberId { get; set; }
        IClan Clan { get; set; }
        IRank Rank { get; set; }
        IPaygrade Paygrade { get; set; }
        List<IAppointment> Appointments { get; set; }
        MudDate JoinDate { get; set; }
        long? ManagerId { get; set; }
        Dictionary<ICurrency, decimal> BackPayDiciontary { get; set; }
        IPersonalName PersonalName { get; set; }
        bool IsArchivedMembership { get; set; }
        ClanPrivilegeType NetPrivileges { get; }
        void DeleteFromDb();
        void AwardPay(ICurrency currency, decimal amount);
        ICharacter MemberCharacter { get; }

        void SetRank(IRank rank);
        void AppointToPosition(IAppointment appointment);
    }
}
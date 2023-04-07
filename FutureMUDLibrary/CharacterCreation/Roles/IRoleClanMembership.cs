using System.Collections.Generic;
using MudSharp.Community;

namespace MudSharp.CharacterCreation.Roles
{
    public interface IRoleClanMembership
    {
        IClan Clan { get; set; }
        IRank Rank { get; set; }
        IPaygrade Paygrade { get; set; }
        List<IAppointment> Appointments { get; set; }
    }
}
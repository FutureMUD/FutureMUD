using MudSharp.Community;
using System.Collections.Generic;

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
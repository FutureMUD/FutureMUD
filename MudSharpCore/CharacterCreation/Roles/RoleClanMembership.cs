using MudSharp.Community;
using MudSharp.Framework;
using MudSharp.Models;
using System.Collections.Generic;
using System.Linq;

#nullable enable
#nullable disable warnings

namespace MudSharp.CharacterCreation.Roles;

public class RoleClanMembership : IRoleClanMembership
{
    public RoleClanMembership(ChargenRolesClanMemberships membership, IFuturemud gameworld)
    {
        Clan = gameworld.Clans.Get(membership.ClanId);
        Rank = Clan.Ranks.First(x => x.Id == membership.RankId);
        if (membership.PaygradeId.HasValue)
        {
            Paygrade = Clan.Paygrades.First(x => x.Id == membership.PaygradeId);
        }

        Appointments =
            Clan.Appointments.Where(
                    x => membership.ChargenRolesClanMembershipsAppointments.Any(y => y.AppointmentId == x.Id))
                .ToList();
    }

    public RoleClanMembership(IClan clan, IRank rank, IPaygrade? paygrade = null)
    {
        Clan = clan;
        Rank = rank;
        Paygrade = paygrade;
        Appointments = new();
    }

    public IClan Clan { get; set; }
    public IRank Rank { get; set; }
    public IPaygrade Paygrade { get; set; }
    public List<IAppointment> Appointments { get; set; }
}

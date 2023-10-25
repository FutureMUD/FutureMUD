using System.Collections.Generic;
using MudSharp.Framework;
using MudSharp.Framework.Save;

namespace MudSharp.Community
{
    public interface IExternalClanControl : ISaveable, IFrameworkItem
    {
        /// <summary>
        ///     The clan being controlled
        /// </summary>
        IClan VassalClan { get; set; }

        /// <summary>
        ///     The clan doing the controlling
        /// </summary>
        IClan LiegeClan { get; set; }

        /// <summary>
        ///     The Appointment from the Vassal clan subject to control
        /// </summary>
        IAppointment ControlledAppointment { get; set; }

        /// <summary>
        ///     The optional Appointment from the Liege Clan exerting the control
        /// </summary>
        IAppointment ControllingAppointment { get; set; }

        /// <summary>
        ///     The maximum number of appointments available to the liege clan
        /// </summary>
        int NumberOfAppointments { get; set; }

        /// <summary>
        ///     The current appointees subject to the will of the liege clan
        /// </summary>
        List<IClanMembership> Appointees { get; set; }

        void Delete();
    }
}
using System.Collections.Generic;
using System.Linq;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Models;

namespace MudSharp.Community;

public class ExternalClanControl : SaveableItem, IExternalClanControl
{
	public ExternalClanControl(MudSharp.Models.ExternalClanControl control, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		Appointees = new List<IClanMembership>();
		VassalClan = Gameworld.Clans.Get(control.VassalClanId);
		VassalClan.ExternalControls.Add(this);
		LiegeClan = Gameworld.Clans.Get(control.LiegeClanId);
		LiegeClan.ExternalControls.Add(this);
		ControlledAppointment = VassalClan.Appointments.First(x => x.Id == control.ControlledAppointmentId);
		ControllingAppointment = LiegeClan.Appointments.FirstOrDefault(x => x.Id == control.ControllingAppointmentId);
		NumberOfAppointments = control.NumberOfAppointments;
		foreach (var appointment in control.ExternalClanControlsAppointments)
		{
			Appointees.Add(VassalClan.Memberships.First(x => x.MemberId == appointment.CharacterId));
		}
	}

	public override string FrameworkItemType => "ExternalClanControl";

	/// <summary>
	///     The clan being controlled
	/// </summary>
	public IClan VassalClan { get; set; }

	/// <summary>
	///     The clan doing the controlling
	/// </summary>
	public IClan LiegeClan { get; set; }

	/// <summary>
	///     The Appointment from the Vassal clan subject to control
	/// </summary>
	public IAppointment ControlledAppointment { get; set; }

	/// <summary>
	///     The optional Appointment from the Liege Clan exerting the control
	/// </summary>
	public IAppointment ControllingAppointment { get; set; }

	/// <summary>
	///     The maximum number of appointments available to the liege clan
	/// </summary>
	public int NumberOfAppointments { get; set; }

	/// <summary>
	///     The current appointees subject to the will of the liege clan
	/// </summary>
	public List<IClanMembership> Appointees { get; set; }

	public override void Save()
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.ExternalClanControls.Find(VassalClan.Id, LiegeClan.Id,
				ControlledAppointment.Id);
			dbitem.ControllingAppointmentId = ControllingAppointment?.Id;
			dbitem.NumberOfAppointments = NumberOfAppointments;
			FMDB.Context.ExternalClanControlsAppointments.RemoveRange(dbitem.ExternalClanControlsAppointments);
			foreach (var item in Appointees)
			{
				var appointee = new ExternalClanControlsAppointment
				{
					CharacterId = item.MemberId
				};
				dbitem.ExternalClanControlsAppointments.Add(appointee);
			}

			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}
}
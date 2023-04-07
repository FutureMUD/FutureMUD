using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Models;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Intervals;
using MudSharp.TimeAndDate.Listeners;

namespace MudSharp.FutureProg;

public class ProgSchedule : SaveableItem, IProgSchedule
{
	public ProgSchedule(MudDateTime referencetime, RecurringInterval interval, IFutureProg prog)
	{
		Gameworld = prog.Gameworld;
		using (new FMDB())
		{
			var dbitem = new Models.ProgSchedule();
			FMDB.Context.ProgSchedules.Add(dbitem);
			dbitem.Name = $"Prog {prog.FunctionName} {interval.Describe(referencetime.Calendar)}";
			dbitem.IntervalType = (int)interval.Type;
			dbitem.IntervalModifier = interval.IntervalAmount;
			dbitem.IntervalOther = interval.Modifier;
			dbitem.ReferenceDate = referencetime.GetDateTimeString();
			dbitem.ReferenceTime = string.Empty;
			dbitem.FutureProgId = prog.Id;
			FMDB.Context.SaveChanges();
			LoadFromDB(dbitem);
		}
	}

	public ProgSchedule(MudSharp.Models.ProgSchedule schedule, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		LoadFromDB(schedule);
	}

	public RecurringInterval Interval { get; set; }
	public MudDateTime NextReferenceTime { get; set; }
	public IFutureProg Prog { get; set; }
	private ITemporalListener _listener;

	#region Overrides of Item

	public override string FrameworkItemType { get; } = "ProgSchedule";

	#endregion

	#region Overrides of SaveableItem

	/// <summary>
	///     Tells the object to perform whatever save action it needs to do
	/// </summary>
	public override void Save()
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.ProgSchedules.Find(Id);
			dbitem.ReferenceDate = NextReferenceTime.GetDateTimeString();
			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}

	#endregion

	public void Delete()
	{
		Gameworld.Destroy(this);
		Gameworld.SaveManager.Abort(this);
		if (_id != 0)
		{
			using (new FMDB())
			{
				Gameworld.SaveManager.Flush();
				var dbitem = FMDB.Context.ProgSchedules.Find(Id);
				if (dbitem != null)
				{
					FMDB.Context.ProgSchedules.Remove(dbitem);
					FMDB.Context.SaveChanges();
				}
			}
		}

		if (_listener is not null)
		{
			_listener.CancelListener();
			_listener = null;
		}
	}

	private void LoadFromDB(MudSharp.Models.ProgSchedule schedule)
	{
		_id = schedule.Id;
		_name = schedule.Name;
		Interval = new RecurringInterval
		{
			Type = (IntervalType)schedule.IntervalType,
			IntervalAmount = schedule.IntervalModifier,
			Modifier = schedule.IntervalOther
		};
		Prog = Gameworld.FutureProgs.Get(schedule.FutureProgId);
		NextReferenceTime = Interval.GetNextDateTime(new MudDateTime(schedule.ReferenceDate, Gameworld));
		_listener = Interval.CreateListenerFromInterval(NextReferenceTime, SchedulePayload, null);
		Changed = true;
	}

	public void SchedulePayload(object[] parameters)
	{
		Prog.Execute();

		NextReferenceTime = Interval.GetNextDateTime(NextReferenceTime);
		_listener = Interval.CreateListenerFromInterval(NextReferenceTime, SchedulePayload, null);
		Changed = true;
	}
}
using System.Text;
using System.Data.Common;
using System.Reflection;
using FutureMUD.RoomSpatialAcceptance;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;

// Required global name for the .NET host. This assembly is only installed in the
// owned acceptance child environment; no production entrypoint is replaced.
internal static class StartupHook
{
	public static void Initialize()
	{
		var admission = OwnedWorldAdmission.FromEnvironment();
		admission.VerifyTarget();
#if !LEGACY
		typeof(FMDB).GetProperty("ConnectionValidator", BindingFlags.Static | BindingFlags.NonPublic)!
			.SetValue(null, (Action<string, DbConnection>)admission.Validate);
#endif
		Environment.SetEnvironmentVariable("DOTNET_STARTUP_HOOKS", null);
		Console.SetOut(new ReadyObserver(Console.Out));
	}

	private sealed class ReadyObserver(TextWriter inner) : TextWriter
	{
		private readonly StringBuilder _line = new();
		private bool _finalised;
		private bool _scheduled;
		public override Encoding Encoding => inner.Encoding;
		public override void Flush() => inner.Flush();
		public override void Write(char value)
		{
			inner.Write(value);
			if (value != '\n') { _line.Append(value); return; }
			var text = _line.ToString(); _line.Clear();
			if (text.Contains("Done Finalising Cells.", StringComparison.Ordinal)) _finalised = true;
			if (_scheduled || !text.Contains("MUD is now ready to connect", StringComparison.Ordinal)) return;
			_scheduled = true;
			if (!_finalised) throw new InvalidOperationException("Ready preceded normal Cell.PostLoadTasks/CompleteMagicLoad completion.");
			var world = Futuremud.Games.Single();
			// This write runs on the normal game thread before its loop. The scheduler
			// is not thread safe; no worker mutates it or the loaded world.
			world.Scheduler.AddSchedule(new Schedule(() => FullBootScenario.Run(world), ScheduleType.TEST,
				TimeSpan.Zero, "Owned cell structural full boot acceptance"));
		}
	}
}

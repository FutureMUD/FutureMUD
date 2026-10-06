#nullable enable
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	// This optional Windows smoke handoff owns only its child and descendants. Its job
	// survives child failure so the database owner can independently verify shutdown.
	private sealed class PreparedBootProcessJob : IDisposable
	{
		private IntPtr _handle;
		public bool Attached { get; private set; }
		public PreparedBootProcessJob()
		{
			Require(OperatingSystem.IsWindows(), "The optional lane boot handoff requires Windows process jobs.");
			_handle = CreateJobObject(IntPtr.Zero, null);
			if (_handle == IntPtr.Zero) throw new Win32Exception(Marshal.GetLastWin32Error());
			var limits = new ExtendedLimits { Basic = new BasicLimits { Flags = 0x2000 } }; // Kill on job close.
			if (SetInformationJobObject(_handle, 9, ref limits, Marshal.SizeOf<ExtendedLimits>())) return;
			var error = Marshal.GetLastWin32Error(); CloseHandle(_handle); _handle = IntPtr.Zero;
			throw new Win32Exception(error);
		}
		public void Attach(Process process)
		{
			if (!AssignProcessToJobObject(_handle, process.Handle)) throw new Win32Exception(Marshal.GetLastWin32Error());
			Attached = true;
		}
		private uint ActiveProcesses()
		{
			if (!QueryInformationJobObject(_handle, 1, out var information, Marshal.SizeOf<Accounting>(), IntPtr.Zero))
				throw new Win32Exception(Marshal.GetLastWin32Error());
			return information.ActiveProcesses;
		}
		public void StopAndVerify()
		{
			if (ActiveProcesses() != 0 && !TerminateJobObject(_handle, 1)) throw new Win32Exception(Marshal.GetLastWin32Error());
			var deadline = Stopwatch.StartNew();
			while (ActiveProcesses() != 0 && deadline.Elapsed < TimeSpan.FromSeconds(30)) Thread.Sleep(50);
			Require(ActiveProcesses() == 0, "Owned boot job still has active processes; shutdown verification failed.");
		}
		public void Dispose()
		{
			if (_handle == IntPtr.Zero) return;
			try { StopAndVerify(); }
			finally
			{
				var handle = _handle; _handle = IntPtr.Zero;
				if (!CloseHandle(handle)) throw new Win32Exception(Marshal.GetLastWin32Error());
			}
		}
		[StructLayout(LayoutKind.Sequential)] private struct BasicLimits
		{
			public long ProcessUserTime, JobUserTime;
			public uint Flags;
			public UIntPtr MinimumWorkingSet, MaximumWorkingSet;
			public uint ActiveProcessLimit;
			public UIntPtr Affinity;
			public uint PriorityClass, SchedulingClass;
		}
		[StructLayout(LayoutKind.Sequential)] private struct IoCounters
		{
			public ulong ReadOperations, WriteOperations, OtherOperations, ReadBytes, WriteBytes, OtherBytes;
		}
		[StructLayout(LayoutKind.Sequential)] private struct ExtendedLimits
		{
			public BasicLimits Basic;
			public IoCounters Io;
			public UIntPtr ProcessMemory, JobMemory, PeakProcessMemory, PeakJobMemory;
		}
		[StructLayout(LayoutKind.Sequential)] private struct Accounting
		{
			public long UserTime, KernelTime, PeriodUserTime, PeriodKernelTime;
			public uint PageFaults, TotalProcesses, ActiveProcesses, TerminatedProcesses;
		}
		[DllImport("kernel32.dll", EntryPoint = "CreateJobObjectW", CharSet = CharSet.Unicode, SetLastError = true)]
		private static extern IntPtr CreateJobObject(IntPtr attributes, string? name);
		[DllImport("kernel32.dll", SetLastError = true)] [return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool SetInformationJobObject(IntPtr job, int informationClass, ref ExtendedLimits information, int length);
		[DllImport("kernel32.dll", SetLastError = true)] [return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool AssignProcessToJobObject(IntPtr job, IntPtr process);
		[DllImport("kernel32.dll", SetLastError = true)] [return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool QueryInformationJobObject(IntPtr job, int informationClass, out Accounting information, int length, IntPtr returnedLength);
		[DllImport("kernel32.dll", SetLastError = true)] [return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool TerminateJobObject(IntPtr job, uint exitCode);
		[DllImport("kernel32.dll", SetLastError = true)] [return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool CloseHandle(IntPtr handle);
	}
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Process = System.Diagnostics.Process;

namespace MudSharp_Watcher
{
	internal class ProcessWatcher
	{
		private Process? _process;

		public event DataReceivedEventHandler? DataReceived;
		public event EventHandler? MudStoppedBooting;
		public event EventHandler? MudStarted;

		private string _databaseString;
		public void SetDatbaseString(string text)
		{
			_databaseString = text;
		}

		public void StartMud()
		{
			_process = new Process();
			_process.EnableRaisingEvents = true;
			_process.OutputDataReceived += Process_OutputDataReceived;
			_process.Exited += ProcessOnExited;
			foreach (var file in Directory.GetFiles("Binaries", "*.dll"))
			{
				File.Copy(file, file.Replace("Binaries\\", ""), true);
			}
			foreach (var file in Directory.GetFiles("Binaries", "*.exe"))
			{
				File.Copy(file, file.Replace("Binaries\\", ""), true);
			}
			foreach (var file in Directory.GetFiles("Binaries", "*.pdb"))
			{
				File.Copy(file, file.Replace("Binaries\\", ""), true);
			}
			File.Delete("STOP-REBOOTING");
			File.Delete("BOOTING");
			_process.StartInfo = new ProcessStartInfo("MudSharp.exe", $"\"MySql.Data.MySqlClient\" \"{_databaseString}\"");
			MudStarted?.Invoke(this, EventArgs.Empty);
			_process.Start();
		}

		private void ProcessOnExited(object? sender, EventArgs e)
		{
			if (File.Exists("BOOTING") || File.Exists("STOP-REBOOTING"))
			{
				MudStoppedBooting?.Invoke(this, EventArgs.Empty);
				return;
			}

			StartMud();
		}

		private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			DataReceived?.Invoke(this, e);
		}
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Logging;
public class DamageLogger : ICustomLogger
{
	public string FileName { get; init; }
	private StringBuilder _pendingLogEntries = new();

	public DamageLogger()
	{
		//_pendingLogEntries.AppendLine("id,name,severity,damage,original,current,pain,stun,result,roll,healing");
	}

	public void SaveLog()
	{
		if (_pendingLogEntries.Length <= 0)
		{
			return;
		}

		using (var writer = new StreamWriter(new FileStream(FileName, FileMode.Append)))
		{
			writer.Write(_pendingLogEntries.ToString());
		}

		_pendingLogEntries = new StringBuilder();
	}

	public void HandleLog(LogEntryType type, params object[] data)
	{
		switch (type)
		{
			case LogEntryType.SufferDamage:

				break;
		}
	}
}

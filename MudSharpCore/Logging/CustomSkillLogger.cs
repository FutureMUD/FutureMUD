using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.RPG.Checks;
using MudSharp.Framework;

namespace MudSharp.Logging;

public class CustomSkillLogger : ICustomLogger
{
	public string FileName { get; init; }
	private StringBuilder _pendingLogEntries = new();

	#region Implementation of ICustomLogger

	public void HandleLog(LogEntryType type, params object[] data)
	{
		switch (type)
		{
			case LogEntryType.SkillUse:
				HandleSkillUse(data);
				return;
			case LogEntryType.SkillBranch:
				HandleSkillBranch(data);
				return;
			case LogEntryType.SkillImprovement:
				HandleSkillImprovement(data);
				return;
		}
	}

	private void HandleSkillBranch(object[] data)
	{
		var character = data[0] is IBody b ? b.Actor : (ICharacter)data[0];
		var skill = (ISkillDefinition)data[1];
		var checkOutcome = (CheckOutcome)data[2];
		var outcome = checkOutcome.Outcome;
		_pendingLogEntries.AppendLine(
			$"{DateTime.UtcNow:G} : Character #{character.Id} ({character.Name}) rolled to branch {skill.Name} and {(outcome.IsPass() ? "passed" : "failed")} [{checkOutcome.Rolls.Select(x => x.ToString("N7")).ListToString()} vs {checkOutcome.TargetNumber:N7}].");
	}

	private void HandleSkillUse(object[] data)
	{
		var character = data[0] is IBody b ? b.Actor : (ICharacter)data[0];
		var bonuses = (IEnumerable<Tuple<string, double>>)data[5] ?? Enumerable.Empty<Tuple<string,double>>();
		_pendingLogEntries.AppendLine(
			$"{DateTime.UtcNow:G} : Character #{character.Id} ({character.PersonalName.GetName(NameStyle.SimpleFull)}) used skill {((ISkillDefinition)data[1]).Name} ({((TraitUseType)data[4]).Describe()} @ {((Difficulty)data[3]).Describe()} - {((Outcome)data[2]).Describe()} - bonuses: {bonuses.Select(x => $"{x.Item1}: {x.Item2.ToBonusString(colour: false)}").ListToString()}");
	}

	private void HandleSkillImprovement(object[] data)
	{
		var text = (string)data[0];
		_pendingLogEntries.AppendLine($"{DateTime.UtcNow:G} : {text}");
	}

	public void SaveLog()
	{
		using (var writer = new StreamWriter(new FileStream(FileName, FileMode.Append)))
		{
			writer.Write(_pendingLogEntries.ToString());
		}

		_pendingLogEntries = new StringBuilder();
	}

	#endregion
}
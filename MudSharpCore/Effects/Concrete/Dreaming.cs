using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Dreams;

namespace MudSharp.Effects.Concrete;

public class Dreaming : Effect, IDreamingEffect
{
	public static Regex DreamRegex =
		new(@"(?<linked>\*(?<linknum>\d+)){0,1}\{(?<options>[^}]+)\}", RegexOptions.IgnoreCase);

	public Dreaming(IPerceivable owner, IDream dream, IFutureProg applicabilityProg = null)
		: base(owner, applicabilityProg)
	{
		Dream = dream;
		Phase = 0;
	}

	public Dictionary<int, int> LinkedOptionsDictionary = new();

	#region Overrides of Effect

	public override string Describe(IPerceiver voyeur)
	{
		return $"{Owner.HowSeen(voyeur, true)} is having the dream {Dream.Name.TitleCase().Colour(Telnet.Green)}";
	}

	#region Overrides of Object

	/// <summary>
	///     Returns a string that represents the current object.
	/// </summary>
	/// <returns>
	///     A string that represents the current object.
	/// </returns>
	public override string ToString()
	{
		return $"Owner is having the dream {Dream.Name.TitleCase()}";
	}

	#endregion

	protected override string SpecificEffectType { get; } = "Dreaming";

	#endregion

	#region Implementation of IDreamingEffect

	public IDream Dream { get; set; }

	public int Phase { get; set; }

	#endregion

	#region Overrides of Effect

	/// <summary>
	///     Fires when the scheduled effect "matures"
	/// </summary>
	public override void RemovalEffect()
	{
		Dream.FinishDream((ICharacter)Owner);
		Owner.RemoveEffect(this);
	}

	#region Overrides of Effect

	private void DoDreamPhase()
	{
		var ownerAsCharacter = Owner as ICharacter;
		if (ownerAsCharacter?.State.HasFlag(CharacterState.Sleeping) != true)
		{
			return;
		}

		var phase = Dream.DreamStages.ElementAtOrDefault(Phase++);
		if (phase == null)
		{
			Owner.RemoveEffect(this, true);
			return;
		}

		var text = DreamRegex.Replace(phase.DreamerText, match =>
		{
			var options = match.Groups["options"].Value.Split('|').ToList();
			if (match.Groups["linked"].Length == 0)
			{
				return options.GetRandomElement();
			}

			var linknum = int.Parse(match.Groups["linknum"].Value);
			if (LinkedOptionsDictionary.ContainsKey(linknum))
			{
				return options.ElementAtOrDefault(
					LinkedOptionsDictionary[linknum]) ?? options.GetRandomElement();
			}

			var option = RandomUtilities.Random(0, options.Count);
			LinkedOptionsDictionary[linknum] = option;
			return options.ElementAtOrDefault(option) ?? options.GetRandomElement();
		});


		Owner.Send(text.NormaliseSpacing().SubstituteANSIColour().ProperSentences());
		if (!string.IsNullOrWhiteSpace(phase.DreamerCommand))
		{
			foreach (var command in phase.DreamerCommand.Split('\n'))
			{
				if (command.StartsWith("wake", StringComparison.InvariantCultureIgnoreCase))
				{
					ownerAsCharacter.RemoveAllEffects(x => x.IsEffectType<NoWake>());
				}

				ownerAsCharacter.OutOfContextExecuteCommand(command);
			}
		}

		if (Dream.DreamStages.Last() != phase)
		{
			Gameworld.Scheduler.AddSchedule(new Schedule(DoDreamPhase, ScheduleType.System,
				TimeSpan.FromSeconds(phase.WaitSeconds),
				$"Dreaming for {ownerAsCharacter.PersonalName.GetName(NameStyle.FullName)}"));
		}
		else
		{
			ownerAsCharacter.RemoveEffect(this, true);
		}
	}

	/// <summary>
	///     Fires when an effect is first added to an individual
	/// </summary>
	public override void InitialEffect()
	{
		DoDreamPhase();
	}

	#endregion

	#endregion
}
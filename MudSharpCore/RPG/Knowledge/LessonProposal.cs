using System;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;

namespace MudSharp.RPG.Knowledge;

public abstract class LessonProposal : Proposal
{
	protected static double[,] TeachLearnMatrix =
	{
		{ 0, 0, 0, 0, 0, 0 },
		{ 0, 0, 0, 0, 0, 1.0 },
		{ 0, 0, 0, 0, 1.0, 1.2 },
		{ 0, 0, 0, 1.0, 1.2, 1.4 },
		{ 0, 0, 0, 1.2, 1.4, 1.6 },
		{ 0, 0, 0, 1.4, 1.6, 1.9 }
	};

	public ICharacter Teacher { get; set; }
	public ICharacter Student { get; set; }

	protected void AddFatigue(ILearningFatigueEffect effect)
	{
		if (effect == null)
		{
			effect = new LearningFatigueEffect(Student, true, 1);
			Student.AddEffect(effect, TimeSpan.FromSeconds(LearningFatigueEffect.GetFatigueLength(1)));
		}
		else
		{
			effect.FatigueDegrees++;
			effect.BlockUntil = DateTime.UtcNow + LearningFatigueEffect.GetBlockLength();
			effect.Changed = true;
			Student.Reschedule(effect,
				TimeSpan.FromSeconds(LearningFatigueEffect.GetFatigueLength(effect.FatigueDegrees)));
		}
	}
}
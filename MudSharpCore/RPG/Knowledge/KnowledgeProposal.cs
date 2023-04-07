using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.RPG.Knowledge;

public class KnowledgeLessonProposal : LessonProposal, IProposal
{
	public KnowledgeLessonProposal(ICharacter teacher, ICharacter student, IKnowledge knowledge)
	{
		Teacher = teacher;
		Student = student;
		Knowledge = knowledge;
	}

	public IKnowledge Knowledge { get; set; }

	#region IKeyworded Members

	public override IEnumerable<string> Keywords => new[] { "teach" };

	#endregion

	#region IProposal Members

	public override void Accept(string message = "")
	{
		var teachDifficulty = Knowledge.TeachDifficulty;
		var learnDifficulty = Knowledge.LearnDifficulty;
		var fatigueEffect = Student.EffectsOfType<ILearningFatigueEffect>().FirstOrDefault();
		if (fatigueEffect != null)
		{
			learnDifficulty = fatigueEffect.BlockUntil > DateTime.UtcNow
				? Difficulty.Impossible
				: learnDifficulty.StageUp(fatigueEffect.FatigueDegrees);
		}

		if (Student.Knowledges.Contains(Knowledge))
		{
			learnDifficulty = Difficulty.Impossible;
		}

		if (Knowledge.CanLearnProg != null &&
		    !((bool?)Knowledge.CanLearnProg.Execute(Student, Knowledge) ?? false))
		{
			learnDifficulty = Difficulty.Impossible;
		}

		var teachoutcome = Teacher.Gameworld.GetCheck(CheckType.KnowledgeTeachCheck)
		                          .Check(Teacher, teachDifficulty, Student);
		// Skip the check on impossible difficulty to avoid skill boosting behaviour
		var learnoutcome = learnDifficulty != Difficulty.Impossible
			? Student.Gameworld.GetCheck(CheckType.KnowledgeLearnCheck).Check(Student, learnDifficulty, Teacher)
			: Outcome.MajorFail;

		Teacher.OutputHandler.Handle(
			new EmoteOutput(new Emote($"@ teach|teaches $1 about {Knowledge.Name}.", Teacher, Teacher, Student),
				flags: OutputFlags.SuppressObscured));
		AddFatigue(fatigueEffect);
		var teacherKnowledge = Teacher.CharacterKnowledges.FirstOrDefault(x => x.Knowledge == Knowledge);
		if (teacherKnowledge != null)
		{
			teacherKnowledge.TimesTaught += 1;
		}

		if (TeachLearnMatrix[(int)teachoutcome.Outcome - 2, (int)learnoutcome - 2] <= 0)
		{
			Teacher.Send("You do not feel like your student learned much from your lesson.");
			Student.Send("You do not feel like you learned much from your teacher's lesson.");
		}
		else
		{
			var effect = Student.EffectsOfType<IncreasedBranchChance>().FirstOrDefault();
			if (effect == null)
			{
				effect = new IncreasedBranchChance(Student);
				Student.AddEffect(effect);
			}

			if (effect.KnowledgeLesson(
				    Knowledge, TeachLearnMatrix[(int)teachoutcome.Outcome - 2, (int)learnoutcome - 2]))
			{
				Student.AddKnowledge(new CharacterKnowledge(Student, Knowledge, $"Taught by {Teacher.Id}"));
				Student.Send($"You have successfully learned the {Knowledge.Name.Colour(Telnet.Green)} knowledge!");
				if (effect.BranchKnowledge(Knowledge))
				{
					Student.RemoveEffect(effect);
				}
			}
			else
			{
				Student.Send(
					"You feel like you learned something, but that you will require more lessons before you truly master the knowledge.");
			}
		}
	}

	public override void Reject(string message = "")
	{
		Teacher.Send("Your student has declined the lesson.");
		Student.Send("You decline the lesson.");
	}

	public override void Expire()
	{
		Teacher.Send("Your student has declined the lesson.");
		Student.Send("You decline the lesson.");
	}

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"{Teacher.HowSeen(voyeur)} is proposing to teach {Student.HowSeen(voyeur)} about the {Knowledge.Name} knowledge";
	}

	#endregion
}
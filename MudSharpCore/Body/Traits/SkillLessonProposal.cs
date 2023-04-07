using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Knowledge;

namespace MudSharp.Body.Traits;

public class SkillLessonProposal : LessonProposal
{
	public SkillLessonProposal(ICharacter teacher, ICharacter student, ISkillDefinition skill)
	{
		Teacher = teacher;
		Student = student;
		Skill = skill;
	}

	public ISkillDefinition Skill { get; set; }

	public override IEnumerable<string> Keywords => new[] { "skill", "teach" };

	public override void Accept(string message = "")
	{
		var teachDifficulty = Skill.TeachDifficulty;
		var learnDifficulty = Skill.LearnDifficulty;
		var fatigueEffect = Student.EffectsOfType<ILearningFatigueEffect>().FirstOrDefault();
		if (fatigueEffect != null)
		{
			learnDifficulty = fatigueEffect.BlockUntil > DateTime.UtcNow
				? Difficulty.Impossible
				: learnDifficulty.StageUp(fatigueEffect.FatigueDegrees);
		}

		if (Student.TraitsOfType(TraitType.Skill).Any(x => x.Definition == Skill) &&
		    Student.TraitValue(Skill) >= Teacher.TraitValue(Skill))
		{
			learnDifficulty = Difficulty.Impossible;
		}

		if (!Skill.CanLearn(Student))
		{
			learnDifficulty = Difficulty.Impossible;
		}

		var teachoutcome = Teacher.Gameworld.GetCheck(CheckType.SkillTeachCheck)
		                          .Check(Teacher, teachDifficulty, Skill, Student);
		// Skip the check on impossible difficulty to avoid skill boosting behaviour
		var learnoutcome = learnDifficulty != Difficulty.Impossible
			? Student.Gameworld.GetCheck(CheckType.SkillLearnCheck).Check(Student, learnDifficulty, Skill, Teacher)
			: Outcome.MajorFail;

		Teacher.OutputHandler.Handle(
			new EmoteOutput(new Emote($"@ teach|teaches $1 about {Skill.Name}.", Teacher, Teacher, Student),
				flags: OutputFlags.SuppressObscured));
		AddFatigue(fatigueEffect);

		if (TeachLearnMatrix[(int)teachoutcome.Outcome - 2, (int)learnoutcome - 2] <= 0)
		{
			//Teacher.Send("You do not feel like your student learned much from your lesson.");
			Student.Send("You do not feel like you learned much from your teacher's lesson.");
		}
		else
		{
			if (Student.TraitsOfType(TraitType.Skill).Any(x => x.Definition == Skill))
			{
				Student.Traits.First(x => x.Definition == Skill).Value +=
					Student.Gameworld.GetStaticDouble("BaseTeachSkillAmount") *
					TeachLearnMatrix[(int)teachoutcome.Outcome - 2, (int)learnoutcome - 2];
				Student.OutputHandler.Send("You feel like you learned something from the lesson.");
			}
			else
			{
				Student.AddTrait(Skill,
					Student.Gameworld.GetStaticDouble("BaseTeachSkillOpeningValue") *
					TeachLearnMatrix[(int)teachoutcome.Outcome - 2, (int)learnoutcome - 2]);
				Student.OutputHandler.Send(
					$"You have successfully learned the {Skill.Name.Colour(Telnet.Green)} skill.");
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
			$"{Teacher.HowSeen(voyeur)} is proposing to teach {Student.HowSeen(voyeur)} about the {Skill.Name} skill";
	}
}
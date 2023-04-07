using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.RPG.Knowledge;

namespace MudSharp.Effects.Concrete;

public class IncreasedBranchChance : Effect, IEffectSubtype
{
	private static TraitExpression _increasedCapExpression;

	public static TraitExpression IncreasedCapExpression => _increasedCapExpression ??= new TraitExpression(
		Futuremud.Games.First().GetStaticConfiguration("IncreasedCapExpressionForBranching"),
		Futuremud.Games.First());

	public IEnumerable<(ISkillDefinition Skill, int Attempts)> GetSkills()
	{
		return _dictionary.Select(x => (x.Key, x.Value));
	}

	public IEnumerable<(IKnowledge Knowledge, double Lessons)> GetKnowledges()
	{
		return _knowledgeDictionary.Select(x => (x.Key, x.Value));
	}

	public int GetAttemptsForSkill(ISkillDefinition skill)
	{
		return _dictionary.ValueOrDefault(skill, 0);
	}

	public void UseSkill(ISkillDefinition skill)
	{
		if (!_dictionary.ContainsKey(skill))
		{
			_dictionary[skill] = 0;
		}

		_dictionary[skill] += 1;
		Changed = true;
	}

	public bool KnowledgeLesson(IKnowledge knowledge, double amount)
	{
		if (!_knowledgeDictionary.ContainsKey(knowledge))
		{
			_knowledgeDictionary.Add(knowledge, 0.0);
		}

		_knowledgeDictionary[knowledge] += amount;
		Changed = true;

		return _knowledgeDictionary[knowledge] >= knowledge.LearnerSessionsRequired;
	}

	public bool BranchSkill(ISkillDefinition skill)
	{
		if (_dictionary.ContainsKey(skill))
		{
			_dictionary.Remove(skill);
			Changed = true;
		}

		return !_dictionary.Any() && !_knowledgeDictionary.Any();
	}

	public bool BranchKnowledge(IKnowledge knowledge)
	{
		if (_knowledgeDictionary.ContainsKey(knowledge))
		{
			_knowledgeDictionary.Remove(knowledge);
			Changed = true;
		}

		return !_dictionary.Any() && !_knowledgeDictionary.Any();
	}

	private readonly Dictionary<ISkillDefinition, int> _dictionary = new();
	private readonly Dictionary<IKnowledge, double> _knowledgeDictionary = new();

	public IncreasedBranchChance(IPerceivable owner) : base(owner)
	{
	}

	public IncreasedBranchChance(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		var root = effect.Element("Effect");
		foreach (var item in root.Element("Skills")?.Elements("Skill") ?? Enumerable.Empty<XElement>())
		{
			_dictionary[(ISkillDefinition)Gameworld.Traits.Get(long.Parse(item.Element("SkillId").Value))] =
				int.Parse(item.Element("Attempts").Value);
		}

		foreach (var item in root.Element("Knowledges")?.Elements("Knowledge") ?? Enumerable.Empty<XElement>())
		{
			_knowledgeDictionary[Gameworld.Knowledges.Get(long.Parse(item.Element("KnowledgeId").Value))] =
				double.Parse(item.Element("Lessons").Value);
		}

		if (Owner is ICharacter ch)
		{
			foreach (var item in ch.TraitsOfType(TraitType.Skill).OfType<ISkill>())
			{
				if (_dictionary.ContainsKey(item.SkillDefinition))
				{
					_dictionary.Remove(item.SkillDefinition);
				}
			}

			foreach (var item in ch.Knowledges)
			{
				if (_knowledgeDictionary.ContainsKey(item))
				{
					_knowledgeDictionary.Remove(item);
				}
			}
		}
	}

	#region Overrides of Effect

	public override string Describe(IPerceiver voyeur)
	{
		var strings = new List<string>();
		if (_dictionary.Any())
		{
			strings.Add(
				$"Tried to branch {_dictionary.Select(x => $"{x.Key.Name.Colour(Telnet.Green)} {x.Value} {(x.Value == 1 ? "time" : "times")}").ListToString()}");
		}

		if (_knowledgeDictionary.Any())
		{
			strings.Add(
				$"Has had lessons with {_knowledgeDictionary.Select(x => $"{x.Key.Name.Colour(Telnet.Cyan)} {x.Value} {(x.Value == 1 ? "time" : "times")}")}");
		}

		return strings.ListToString();
	}

	protected override string SpecificEffectType => "IncreasedBranchChance";

	public override bool SavingEffect => true;

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("Skills",
				from item in _dictionary
				select new XElement("Skill", new XElement("SkillId", item.Key.Id),
					new XElement("Attempts", item.Value))),
			new XElement("Knowledges",
				from item in _knowledgeDictionary
				select new XElement("Knowledge", new XElement("KnowledgeId", item.Key.Id),
					new XElement("Lessons", item.Value))
			)
		);
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("IncreasedBranchChance", (effect, owner) => new IncreasedBranchChance(effect, owner));
	}

	#endregion
}
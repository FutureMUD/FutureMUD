using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;

namespace MudSharp.Effects.Concrete;

public class GuardingExit : Effect, IGuardExitEffect
{
	public ICharacter CharacterOwner { get; set; }

	public GuardingExit(ICharacter owner, ICellExit exit, bool permitAllies) : base(owner)
	{
		CharacterOwner = owner;
		Exit = exit;
		PermitAllies = permitAllies;
	}

	protected GuardingExit(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		CharacterOwner = (ICharacter)owner;
		var root = effect.Element("Element");
		var exitId = long.Parse(root.Element("Exit").Value);
		Exit = CharacterOwner.Location.ExitsFor(CharacterOwner).FirstOrDefault(x => x.Exit.Id == exitId);
		PermitAllies = bool.Parse(root.Element("PermitAllies").Value);
		foreach (var item in root.Element("Exemptions").Elements())
		{
			_permittedExemptionCharacterIds.Add(long.Parse(item.Attribute("id").Value));
			_permittedExemptionCharacterLastDescs[long.Parse(item.Attribute("id").Value)] = item.Value;
		}
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Element",
			new XElement("Exit", Exit?.Exit.Id ?? 0),
			new XElement("PermitAllies", PermitAllies),
			new XElement("Exemptions",
				from item in _permittedExemptionCharacterLastDescs
				select new XElement("Exemption",
					new XAttribute("id", item.Key),
					new XCData(item.Value)
				)
			)
		);
	}


	public static void InitialiseEffectType()
	{
		RegisterFactory("GuardingExit", (effect, owner) => new GuardingExit(effect, owner));
	}

	#region Overrides of Effect

	public override string Describe(IPerceiver voyeur)
	{
		return $"Owner is {SuffixFor(voyeur)}";
	}

	public bool ShouldRemove(IAffectedByChangeInGuarding newEffect)
	{
		if (newEffect == this)
		{
			return false;
		}

		return true;
	}

	public bool ShouldRemove(CharacterState newState)
	{
		return newState.HasFlag(CharacterState.Dead) || !CharacterState.Able.HasFlag(newState);
	}

	protected override string SpecificEffectType => "GuardingExit";

	public override bool SavingEffect => true;

	#endregion

	#region Implementation of ILDescSuffixEffect

	public string SuffixFor(IPerceiver voyeur)
	{
		if (Exit.Exit.Door?.IsOpen == false)
		{
			return $"guarding {Exit.Exit.Door.Parent.HowSeen(voyeur)} to {Exit.OutboundDirectionDescription}";
		}

		return $"guarding the exit to {Exit.OutboundDirectionDescription}";
	}

	public bool SuffixApplies()
	{
		return true;
	}

	#endregion

	#region Implementation of IGuardExitEffect

	private readonly List<long> _permittedExemptionCharacterIds = new();
	private readonly Dictionary<long, string> _permittedExemptionCharacterLastDescs = new();

	public ICellExit Exit { get; set; }
	private bool _permitAllies;

	public bool PermitAllies
	{
		get => _permitAllies;
		set
		{
			_permitAllies = value;
			Changed = true;
		}
	}

	public bool PermittedToCross(ICharacter ch, ICellExit exit)
	{
		if (Exit != exit)
		{
			return true;
		}

		if (_permittedExemptionCharacterIds.Contains(ch.Id))
		{
			return true;
		}

		if (PermitAllies && CharacterOwner.IsAlly(ch))
		{
			return true;
		}

		if (PermitAllies && (ch.Party?.ActiveCharacterMembers.Any(x => CharacterOwner.IsAlly(x)) ?? false))
		{
			return true;
		}

		if (ch.Party?.ActiveCharacterMembers.Any(x => _permittedExemptionCharacterIds.Contains(x.Id)) ==
		    true)
		{
			return true;
		}

		return false;
	}

	public void Exempt(ICharacter ch)
	{
		if (!_permittedExemptionCharacterIds.Contains(ch.Id))
		{
			_permittedExemptionCharacterIds.Add(ch.Id);
			_permittedExemptionCharacterLastDescs[ch.Id] = ch.HowSeen(CharacterOwner, colour: false);
			Changed = true;
		}
	}

	public void Exempt(long id, string lastDescription)
	{
		if (!_permittedExemptionCharacterIds.Contains(id))
		{
			_permittedExemptionCharacterIds.Add(id);
			_permittedExemptionCharacterLastDescs[id] = lastDescription;
			Changed = true;
		}
	}

	public void RemoveExemption(ICharacter ch)
	{
		_permittedExemptionCharacterIds.Remove(ch.Id);
		_permittedExemptionCharacterLastDescs.Remove(ch.Id);
		Changed = true;
	}

	public void RemoveExemption(long id)
	{
		_permittedExemptionCharacterIds.Remove(id);
		_permittedExemptionCharacterLastDescs.Remove(id);
		Changed = true;
	}

	public void RemoveExemption(string playerInput)
	{
		var options = new List<DummyKeywordedItem<long>>(
			from kvp in _permittedExemptionCharacterLastDescs
			select new DummyKeywordedItem<long>(kvp.Key, item => kvp.Value.Split(' ', '-').ToList())
		);

		var option = options.GetFromItemListByKeyword(playerInput, null);
		if (option == null)
		{
			CharacterOwner.Send("You don't have any such guard exemption to remove.");
			return;
		}

		_permittedExemptionCharacterIds.Remove(option.Object);
		CharacterOwner.Send(
			$"You will no longer permit {_permittedExemptionCharacterLastDescs[option.Object]} to pass by you.");
		_permittedExemptionCharacterLastDescs.Remove(option.Object);
		Changed = true;
	}

	public IEnumerable<(long Id, string Description)> Exemptions =>
		_permittedExemptionCharacterLastDescs.Select(x => (x.Key, x.Value)).ToList();

	bool IRemoveOnMovementEffect.ShouldRemove()
	{
		return true;
	}

	#endregion
}
using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Construction;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Components;

public class TreatmentGameItemComponent : GameItemComponent, ITreatment
{
	private TreatmentGameItemComponentProto _prototype;
	public int UsesRemaining { get; set; }
	public override IGameItemComponentProto Prototype => _prototype;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new TreatmentGameItemComponent(this, newParent, temporary);
	}

	public override int DecorationPriority => 0;

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Full;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		var check = Gameworld.GetCheck(CheckType.TreatmentItemRecognitionCheck);
		if (voyeur is not IPerceivableHaveTraits perceiver)
		{
			return description;
		}

		if (check.Check(perceiver, Difficulty.Normal).IsPass())
		{
			var sb = new StringBuilder(description);
			sb.AppendLine();
			sb.AppendLine();
			sb.AppendFormat("This item can be used for {0}. ",
				_prototype.TreatmentTypes.Select(x => x.Describe().Colour(Telnet.Green)).ListToString());
			if (UsesRemaining == -1 || UsesRemaining == _prototype.MaximumUses)
			{
				sb.AppendFormat("It is fully stocked.");
			}
			else if (UsesRemaining == 0)
			{
				sb.AppendFormat("It does not have any stock remaining.");
			}
			else
			{
				sb.AppendFormat(voyeur, "It is appproximately {0:P0} stocked.",
					(double)UsesRemaining / _prototype.MaximumUses);
			}

			return sb.ToString();
		}

		return description;
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (TreatmentGameItemComponentProto)newProto;
		if (UsesRemaining > _prototype.MaximumUses)
		{
			UsesRemaining = _prototype.MaximumUses;
		}
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("UsesRemaining", UsesRemaining)
		).ToString();
	}

	#region Constructors

	public TreatmentGameItemComponent(TreatmentGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
		UsesRemaining = _prototype.MaximumUses;
		Changed = true;
	}

	public TreatmentGameItemComponent(MudSharp.Models.GameItemComponent component,
		TreatmentGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	private void LoadFromXml(XElement root)
	{
		var element = root.Element("UsesRemaining");
		if (element != null)
		{
			UsesRemaining = int.Parse(element.Value);
		}
	}

	public TreatmentGameItemComponent(TreatmentGameItemComponent rhs, IGameItem newParent, bool temporary = false) :
		base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		UsesRemaining = rhs.UsesRemaining;
	}

	#endregion

	#region ITreatment Members

	public bool IsTreatmentType(TreatmentType type)
	{
		return _prototype.TreatmentTypes.Contains(type) && UsesRemaining != 0;
	}

	public void UseTreatment()
	{
		if (_prototype.MaximumUses < 1)
		{
			return;
		}

		UsesRemaining -= 1;
		if (UsesRemaining == 0)
		{
			Parent.OutputHandler.Handle(new EmoteOutput(new Emote("@ have|has been completely used up!", Parent)));
			if (!_prototype.Refillable)
			{
				Parent.InInventoryOf?.Take(Parent);
				Parent.Delete();
			}
		}

		Changed = true;
	}

	public Difficulty GetTreatmentDifficulty(Difficulty baseDifficulty)
	{
		return _prototype.DifficultyStages < 0
			? baseDifficulty.StageUp(Math.Abs(_prototype.DifficultyStages))
			: baseDifficulty.StageDown(_prototype.DifficultyStages);
	}

	public override bool HandleDieOrMorph(IGameItem newItem, ICell location)
	{
		var newItemTreatment = newItem?.GetItemType<TreatmentGameItemComponent>();
		if (newItemTreatment == null)
		{
			return false;
		}

		newItemTreatment.UsesRemaining = UsesRemaining;
		return false;
	}

	#endregion
}
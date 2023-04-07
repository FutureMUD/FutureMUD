using System;
using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class BottomlessTorchGameItemComponent : TorchGameItemComponent
{
	public override bool Lit
	{
		get => base.Lit;
		set
		{
			_lit = value;
			Changed = true;
		}
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new BottomlessTorchGameItemComponent(this, newParent, temporary);
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		switch (type)
		{
			case DescriptionType.Short:
				return base.Decorate(voyeur, name, description, type, colour, flags);
			case DescriptionType.Full:
				return $"{description}\n\n{(Lit ? "It is currently lit." : "It is not currently lit.")}";
		}

		throw new NotSupportedException("Invalid Decorate type in BottomlessTorchGameItemComponent.Decorate");
	}

	public override bool CanLight(ICharacter lightee, IPerceivable ignitionSource)
	{
		return !Lit;
	}

	public override string WhyCannotLight(ICharacter lightee, IPerceivable ignitionSource)
	{
		if (Lit)
		{
			return $"You cannot light {Parent.HowSeen(lightee)} because it is already lit.";
		}

		throw new NotSupportedException("Invalid reason in BottomlessTorchGameItemComponent.WhyCannotLight");
	}

	#region Constructors

	public BottomlessTorchGameItemComponent(BottomlessTorchGameItemComponent rhs, IGameItem newParent,
		bool temporary = false)
		: base(rhs, newParent, temporary)
	{
	}

	public BottomlessTorchGameItemComponent(TorchGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(proto, parent, temporary)
	{
	}

	public BottomlessTorchGameItemComponent(MudSharp.Models.GameItemComponent component,
		TorchGameItemComponentProto proto,
		IGameItem parent)
		: base(component, proto, parent)
	{
	}

	#endregion
}
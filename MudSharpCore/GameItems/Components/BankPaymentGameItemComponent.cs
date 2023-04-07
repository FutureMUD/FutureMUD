using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using MudSharp.Commands.Trees;
using MudSharp.Construction;
using MudSharp.Economy;
using MudSharp.Form.Shape;

namespace MudSharp.GameItems.Components;

public class BankPaymentGameItemComponent : GameItemComponent, IBankPaymentItem
{
	protected BankPaymentGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (BankPaymentGameItemComponentProto)newProto;
	}

	public int MaximumUses => _prototype.MaximumUses;
	private int _currentUsesRemaining;

	public int CurrentUsesRemaining
	{
		get => _currentUsesRemaining;
		set
		{
			_currentUsesRemaining = value;
			Changed = true;
			if (CurrentUsesRemaining == 0)
			{
				Parent.Handle(new EmoteOutput(new Emote("$0 is all used up.", Parent, Parent)), OutputRange.Local);
				BankAccount?.CancelExistingPaymentItem(this);
				Parent.Delete();
			}
		}
	}

	private long? _bankAccountId;
	private IBankAccount _bankAccount;

	public IBankAccount BankAccount
	{
		get
		{
			if (_bankAccount is null && _bankAccountId is not null)
			{
				_bankAccount = Gameworld.BankAccounts.Get(_bankAccountId.Value);
			}

			return _bankAccount;
		}
		set
		{
			_bankAccount = value;
			_bankAccountId = value?.Id;
			Changed = true;
		}
	}

	#region Constructors

	public BankPaymentGameItemComponent(BankPaymentGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
		_currentUsesRemaining = _prototype.MaximumUses;
	}

	public BankPaymentGameItemComponent(Models.GameItemComponent component, BankPaymentGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public BankPaymentGameItemComponent(BankPaymentGameItemComponent rhs, IGameItem newParent, bool temporary = false) :
		base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_bankAccount = rhs._bankAccount;
		_bankAccountId = rhs._bankAccountId;
		_currentUsesRemaining = rhs._currentUsesRemaining;
	}

	protected void LoadFromXml(XElement root)
	{
		_bankAccountId = long.Parse(root.Element("BankAccount").Value);
		_currentUsesRemaining = int.Parse(root.Element("UsesRemaining").Value);
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new BankPaymentGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("BankAccount", BankAccount?.Id ?? 0),
			new XElement("UsesRemaining", _currentUsesRemaining)
		).ToString();
	}

	#endregion

	#region Overrides of GameItemComponent

	/// <inheritdoc />
	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Evaluate;
	}

	/// <inheritdoc />
	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour,
		PerceiveIgnoreFlags flags)
	{
		if (type == DescriptionType.Evaluate)
		{
			var sb = new StringBuilder();
			sb.AppendLine("This item can be used to make payments at shops.");
			if (BankAccount is null)
			{
				sb.AppendLine("It has not been configured to work with any bank accounts.");
			}
			else
			{
				sb.AppendLine(
					$"It is tied to account {BankAccount.AccountNumber.ToString("F0", voyeur).ColourValue()} from {BankAccount.Bank.Name.ColourName()}.");
			}

			return sb.ToString();
		}

		return base.Decorate(voyeur, name, description, type, colour, flags);
	}

	#endregion
}
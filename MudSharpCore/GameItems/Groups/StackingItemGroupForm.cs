using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.GameItems.Decorators;

namespace MudSharp.GameItems.Groups;

public class StackingItemGroupForm : GameItemGroupForm
{
	public StackingItemGroupForm(IGameItemGroup parent, IFuturemud gameworld) : base(parent)
	{
	}

	public StackingItemGroupForm(ItemGroupForm form, IGameItemGroup parent, IFuturemud gameworld)
		: base(form, parent)
	{
	}

	protected override string GameItemGroupFormType => "Stacking";
	public string ItemName { get; set; }
	public string ItemDescription { get; set; }
	public IStackDecorator Decorator { get; set; }
	public override string Describe(IPerceiver voyeur, IEnumerable<IGameItem> items)
	{
		return Decorator.Describe(ItemName, ItemDescription, items.Count());
	}

	public override string LookDescription(IPerceiver voyeur, IEnumerable<IGameItem> items)
	{
		throw new NotImplementedException();
	}

	public override void Save()
	{
		throw new NotImplementedException();
	}

	public override string Show(IPerceiver voyeur)
	{
		throw new NotImplementedException();
	}
}
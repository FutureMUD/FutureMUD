﻿using System.Collections.Generic;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.FutureProg.Functions.Manipulation;

internal class AttachFunction : BuiltInFunction
{
	public AttachFunction(IList<IFunction> parameters) : base(parameters)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Boolean;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var attachableItem = (IGameItem)ParameterFunctions[0].Result;
		if (attachableItem == null)
		{
			ErrorMessage = "Attachable GameItem was null in Attach function.";
			return StatementResult.Error;
		}

		var attachedtoItem = (IGameItem)ParameterFunctions[1].Result;
		if (attachedtoItem == null)
		{
			ErrorMessage = "Attachedto GameItem was null in Attach function.";
			return StatementResult.Error;
		}

		var attachable = attachableItem.GetItemType<IBeltable>();
		if (attachable == null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var attachedto = attachedtoItem.GetItemType<IBelt>();
		if (attachedto == null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		if (attachable.ConnectedTo != null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var result = attachedto.CanAttachBeltable(attachable);
		switch (result)
		{
			case IBeltCanAttachBeltableResult.FailureExceedMaximumNumber:
				Result = new BooleanVariable(false);
				return StatementResult.Normal;
			case IBeltCanAttachBeltableResult.FailureTooLarge:
				Result = new BooleanVariable(false);
				return StatementResult.Normal;
			case IBeltCanAttachBeltableResult.NotValidType:
				Result = new BooleanVariable(false);
				return StatementResult.Normal;
		}

		var holdable = attachableItem.GetItemType<IHoldable>();
		if (holdable?.HeldBy != null)
		{
			holdable.HeldBy.Take(attachableItem);
		}
		else if (attachableItem.ContainedIn != null)
		{
			var containedInContainer = attachableItem.ContainedIn.GetItemType<IContainer>();
			containedInContainer?.Take(null, attachableItem, 0);
		}
		else
		{
			attachableItem.Location?.Extract(attachableItem);
		}

		attachable.ConnectedTo?.RemoveConnectedItem(attachable);
		attachedto.AddConnectedItem(attachable);

		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"attach",
			new[] { ProgVariableTypes.Item, ProgVariableTypes.Item },
			(pars, gameworld) => new AttachFunction(pars),
			[
				"attachable",
				"attachto",
			],
			[
				"The item to be attached",
				"The item the first thing it to be attached to"
			],
			"Causes an item to become attached to another item. Returns true if it succeeded.",
			"Manipulation",
			ProgVariableTypes.Boolean
		));
	}
}
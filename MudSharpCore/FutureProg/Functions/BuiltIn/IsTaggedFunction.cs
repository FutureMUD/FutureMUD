using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class IsTaggedFunction : BuiltInFunction
{
	public bool UseId { get; }

	public IFuturemud Gameworld { get; }

	protected IsTaggedFunction(IList<IFunction> parameters, bool useId, IFuturemud gameworld)
		: base(parameters)
	{
		UseId = useId;
		Gameworld = gameworld;
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

		if (ParameterFunctions[0].Result?.GetObject is not IHaveTags tagged)
		{
			ErrorMessage = "The tagged variable in the IsTagged function cannot be null";
			return StatementResult.Error;
		}

		if (ParameterFunctions[1].Result?.GetObject == null)
		{
			ErrorMessage = "The tag variable in the IsTagged function cannot be null";
			return StatementResult.Error;
		}

		ITag tag;
		if (UseId)
		{
			tag = Gameworld.Tags.Get((int)(decimal)ParameterFunctions[1].Result.GetObject);
		}
		else
		{
			var matchedtags = Gameworld.Tags.FindMatchingTags(ParameterFunctions[1].Result.GetObject.ToString());
			if (matchedtags.Count == 0)
			{
				ErrorMessage =
					$"There was no tag found with reference {ParameterFunctions[1].Result.GetObject.ToString()}";
				return StatementResult.Error;
			}

			if (matchedtags.Count > 1)
			{
				ErrorMessage =
					$"Multiple tags found with reference {ParameterFunctions[1].Result.GetObject.ToString()}";
				return StatementResult.Error;
			}

			tag = matchedtags.Single();
		}

		if (tag == null)
		{
			ErrorMessage = $"There was no tag found with reference {ParameterFunctions[1].Result.GetObject.ToString()}";
			return StatementResult.Error;
		}

		Result = new BooleanVariable(tagged.IsA(tag));
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"istagged",
				new[] { ProgVariableTypes.Item, ProgVariableTypes.Text },
				(pars, gameworld) => new IsTaggedFunction(pars, false, gameworld),
				new List<string> { "thing", "tag" },
				new List<string>
					{ "The thing whose tags you want to interrogate", "The name of the tag you want to test for" },
				"Returns whether or not the thing has or counts as the specified tag. For example, if you specify the tag 'Cutting Implement' and thing is tagged with the 'Knife' tag, and Knife is a Cutting Implement, then thing is a cutting implement.",
				"Tags",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"istagged",
				new[] { ProgVariableTypes.Item, ProgVariableTypes.Number },
				(pars, gameworld) => new IsTaggedFunction(pars, true, gameworld),
				new List<string> { "thing", "tag" },
				new List<string>
					{ "The thing whose tags you want to interrogate", "The id of the tag you want to test for" },
				"Returns whether or not the thing has or counts as the specified tag. For example, if you specify the tag 'Cutting Implement' and thing is tagged with the 'Knife' tag, and Knife is a Cutting Implement, then thing is a cutting implement.",
				"Tags",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"istagged",
				new[] { ProgVariableTypes.Location, ProgVariableTypes.Text },
				(pars, gameworld) => new IsTaggedFunction(pars, false, gameworld),
				new List<string> { "thing", "tag" },
				new List<string>
					{ "The thing whose tags you want to interrogate", "The name of the tag you want to test for" },
				"Returns whether or not the thing has or counts as the specified tag. For example, if you specify the tag 'Cutting Implement' and thing is tagged with the 'Knife' tag, and Knife is a Cutting Implement, then thing is a cutting implement.",
				"Tags",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"istagged",
				new[] { ProgVariableTypes.Location, ProgVariableTypes.Number },
				(pars, gameworld) => new IsTaggedFunction(pars, true, gameworld),
				new List<string> { "thing", "tag" },
				new List<string>
					{ "The thing whose tags you want to interrogate", "The id of the tag you want to test for" },
				"Returns whether or not the thing has or counts as the specified tag. For example, if you specify the tag 'Cutting Implement' and thing is tagged with the 'Knife' tag, and Knife is a Cutting Implement, then thing is a cutting implement.",
				"Tags",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"istagged",
				new[] { ProgVariableTypes.Terrain, ProgVariableTypes.Text },
				(pars, gameworld) => new IsTaggedFunction(pars, false, gameworld),
				new List<string> { "thing", "tag" },
				new List<string>
					{ "The thing whose tags you want to interrogate", "The name of the tag you want to test for" },
				"Returns whether or not the thing has or counts as the specified tag. For example, if you specify the tag 'Cutting Implement' and thing is tagged with the 'Knife' tag, and Knife is a Cutting Implement, then thing is a cutting implement.",
				"Tags",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"istagged",
				new[] { ProgVariableTypes.Terrain, ProgVariableTypes.Number },
				(pars, gameworld) => new IsTaggedFunction(pars, true, gameworld),
				new List<string> { "thing", "tag" },
				new List<string>
					{ "The thing whose tags you want to interrogate", "The id of the tag you want to test for" },
				"Returns whether or not the thing has or counts as the specified tag. For example, if you specify the tag 'Cutting Implement' and thing is tagged with the 'Knife' tag, and Knife is a Cutting Implement, then thing is a cutting implement.",
				"Tags",
				ProgVariableTypes.Boolean
			)
		);
	}
}
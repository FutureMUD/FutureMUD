using System;
using System.Collections.Generic;
using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;

namespace MudSharp.RPG.Merits;

public abstract class MeritBase : FrameworkItem, IMerit
{
	protected MeritBase(Merit merit)
	{
		_id = merit.Id;
		_name = merit.Name;
		MeritType = (MeritType)merit.MeritType;
		MeritScope = (MeritScope)merit.MeritScope;
	}

	public override string FrameworkItemType => "Merit";

	#region IMerit Members

	public MeritScope MeritScope { get; protected set; }

	public MeritType MeritType { get; protected set; }

	public abstract bool Applies(IHaveMerits owner);

	public abstract string Describe(IHaveMerits owner, IPerceiver voyeur);

	#endregion

	#region IFutureProgVariable Members

	private static IReadOnlyDictionary<string, FutureProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, FutureProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", FutureProgVariableTypes.Number },
			{ "name", FutureProgVariableTypes.Text }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", "" },
			{ "name", "" }
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.Merit, DotReferenceHandler(),
			DotReferenceHelp());
	}


	public IFutureProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "id":
				return new NumberVariable(Id);
			case "name":
				return new TextVariable(Name);
			default:
				throw new NotSupportedException("MeritBase.GetProperty requested invalid Merit Property " + property);
		}
	}

	public FutureProgVariableTypes Type => FutureProgVariableTypes.Merit;

	public object GetObject => this;

	#endregion
}
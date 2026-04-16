#nullable enable

using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MudSharp.FutureProg.Compiler;

internal class StatementCompilerInformation
{
	private readonly HashSet<FutureProgCompilationContext> _allowedContexts;

	public StatementCompilerInformation(Regex regex,
		Func<IEnumerable<string>, IDictionary<string, ProgVariableTypes>, int, IFuturemud, ICompileInfo> compilerFunction,
		IEnumerable<FutureProgCompilationContext>? allowedContexts = null)
	{
		Regex = regex;
		CompilerFunction = compilerFunction;
		_allowedContexts = new HashSet<FutureProgCompilationContext>(
			allowedContexts != null && allowedContexts.Any()
				? allowedContexts.Distinct()
				:
			[FutureProgCompilationContext.StandardFutureProg]);
	}

	public Regex Regex { get; }
	public Func<IEnumerable<string>, IDictionary<string, ProgVariableTypes>, int, IFuturemud, ICompileInfo> CompilerFunction { get; }
	public IEnumerable<FutureProgCompilationContext> AllowedContexts => _allowedContexts;

	public bool SupportsContext(FutureProgCompilationContext context)
	{
		return _allowedContexts.Contains(context);
	}
}

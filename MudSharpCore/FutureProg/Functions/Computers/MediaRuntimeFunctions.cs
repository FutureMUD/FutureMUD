#nullable enable

using MudSharp.Computers;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Computers;

internal abstract class ComputerMediaBuiltInFunction : ComputerRuntimeBuiltInFunction
{
	protected ComputerMediaBuiltInFunction(IList<IFunction> parameterFunctions)
		: base(parameterFunctions)
	{
	}

	protected bool TryGetMediaContext(out ComputerExecutionContext context)
	{
		context = CurrentContext!;
		if (CurrentContext is not null)
		{
			return true;
		}

		ErrorMessage = "This media function requires an active computer execution context.";
		return false;
	}

	protected static string TextArgument(IFunction function)
	{
		return function.Result?.GetObject?.ToString() ?? string.Empty;
	}
}

internal sealed class GetMediaInputsFunction : ComputerMediaBuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"getmediainputs",
			Array.Empty<ProgVariableTypes>(),
			(pars, _) => new GetMediaInputsFunction(pars),
			Array.Empty<string>(),
			Array.Empty<string>(),
			"Returns the names of all powered media inputs connected to the current computer host.",
			"Computers",
			ProgVariableTypes.Collection | ProgVariableTypes.Text,
			allowedContexts: ComputerRuntimeFunctionContexts.ProgramOnly));
	}

	private GetMediaInputsFunction(IList<IFunction> parameterFunctions)
		: base(parameterFunctions)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Collection | ProgVariableTypes.Text;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error || !TryGetMediaContext(out var context))
		{
			return StatementResult.Error;
		}

		Result = new CollectionVariable(context.Gameworld.ComputerMediaService.GetMediaInputs(context.Host).ToList(),
			ProgVariableTypes.Text);
		return StatementResult.Normal;
	}
}

internal sealed class GetMediaOutputsFunction : ComputerMediaBuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"getmediaoutputs",
			Array.Empty<ProgVariableTypes>(),
			(pars, _) => new GetMediaOutputsFunction(pars),
			Array.Empty<string>(),
			Array.Empty<string>(),
			"Returns the names of all powered media outputs connected to the current computer host.",
			"Computers",
			ProgVariableTypes.Collection | ProgVariableTypes.Text,
			allowedContexts: ComputerRuntimeFunctionContexts.ProgramOnly));
	}

	private GetMediaOutputsFunction(IList<IFunction> parameterFunctions)
		: base(parameterFunctions)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Collection | ProgVariableTypes.Text;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error || !TryGetMediaContext(out var context))
		{
			return StatementResult.Error;
		}

		Result = new CollectionVariable(context.Gameworld.ComputerMediaService.GetMediaOutputs(context.Host).ToList(),
			ProgVariableTypes.Text);
		return StatementResult.Normal;
	}
}

internal sealed class StartMediaRecordingFunction : ComputerMediaBuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"startmediarecording",
			[ProgVariableTypes.Text, ProgVariableTypes.Text],
			(pars, _) => new StartMediaRecordingFunction(pars),
			["input", "filename"],
			["The local media input name", "The immutable media file name to create"],
			"Starts a recording job and returns its positive job id. The function raises a runtime error if the input or file cannot be used.",
			"Computers",
			ProgVariableTypes.Number,
			allowedContexts: ComputerRuntimeFunctionContexts.ProgramOnly));
	}

	private StartMediaRecordingFunction(IList<IFunction> parameterFunctions)
		: base(parameterFunctions)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Number;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error || !TryGetMediaContext(out var context))
		{
			return StatementResult.Error;
		}

		var jobId = context.Gameworld.ComputerMediaService.StartRecording(context.Host,
			TextArgument(ParameterFunctions[0]), TextArgument(ParameterFunctions[1]), out var error);
		if (jobId <= 0L)
		{
			ErrorMessage = error;
			return StatementResult.Error;
		}

		Result = new NumberVariable(jobId);
		return StatementResult.Normal;
	}
}

internal sealed class StartMediaPlaybackFunction : ComputerMediaBuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"startmediaplayback",
			[ProgVariableTypes.Text, ProgVariableTypes.Text],
			(pars, _) => new StartMediaPlaybackFunction(pars),
			["filename", "output"],
			["The recorded media file", "The local media output name"],
			"Starts a playback job and returns its positive job id. The function raises a runtime error if playback cannot begin.",
			"Computers",
			ProgVariableTypes.Number,
			allowedContexts: ComputerRuntimeFunctionContexts.ProgramOnly));
	}

	private StartMediaPlaybackFunction(IList<IFunction> parameterFunctions)
		: base(parameterFunctions)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Number;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error || !TryGetMediaContext(out var context))
		{
			return StatementResult.Error;
		}

		var jobId = context.Gameworld.ComputerMediaService.StartPlayback(context.Host,
			TextArgument(ParameterFunctions[0]), TextArgument(ParameterFunctions[1]), out var error);
		if (jobId <= 0L)
		{
			ErrorMessage = error;
			return StatementResult.Error;
		}

		Result = new NumberVariable(jobId);
		return StatementResult.Normal;
	}
}

internal sealed class CaptureMediaStillFunction : ComputerMediaBuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"capturemediastill",
			[ProgVariableTypes.Text, ProgVariableTypes.Text],
			(pars, _) => new CaptureMediaStillFunction(pars),
			["input", "filename"],
			["The local media input name", "The immutable still file name to create"],
			"Captures the current video frame from an input and returns whether a still media file was created.",
			"Computers",
			ProgVariableTypes.Boolean,
			allowedContexts: ComputerRuntimeFunctionContexts.ProgramOnly));
	}

	private CaptureMediaStillFunction(IList<IFunction> parameterFunctions)
		: base(parameterFunctions)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Boolean;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error || !TryGetMediaContext(out var context))
		{
			return StatementResult.Error;
		}

		Result = new BooleanVariable(context.Gameworld.ComputerMediaService.CaptureStill(context.Host,
			TextArgument(ParameterFunctions[0]), TextArgument(ParameterFunctions[1]), out _));
		return StatementResult.Normal;
	}
}

internal sealed class StopMediaJobFunction : ComputerMediaBuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"stopmediajob",
			[ProgVariableTypes.Number],
			(pars, _) => new StopMediaJobFunction(pars),
			["jobid"],
			["The active media job id"],
			"Stops a media recording or playback job on the current host and returns whether it was stopped.",
			"Computers",
			ProgVariableTypes.Boolean,
			allowedContexts: ComputerRuntimeFunctionContexts.ProgramOnly));
	}

	private StopMediaJobFunction(IList<IFunction> parameterFunctions)
		: base(parameterFunctions)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Boolean;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error || !TryGetMediaContext(out var context))
		{
			return StatementResult.Error;
		}

		var jobId = Convert.ToInt64((decimal?)(ParameterFunctions[0].Result?.GetObject) ?? 0M);
		Result = new BooleanVariable(context.Gameworld.ComputerMediaService.StopJob(context.Host, jobId, out _));
		return StatementResult.Normal;
	}
}

internal sealed class WaitMediaEventFunction : ComputerMediaBuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"waitmediaevent",
			[ProgVariableTypes.Text],
			(pars, _) => new WaitMediaEventFunction(pars),
			["endpoint"],
			["The local media input name"],
			"Suspends until the next packet reaches the named local input and returns a text dictionary of packet metadata. It never exposes transcript or scene contents.",
			"Computers",
			ProgVariableTypes.Dictionary | ProgVariableTypes.Text,
			allowedContexts: ComputerRuntimeFunctionContexts.ProgramOnly));
	}

	private WaitMediaEventFunction(IList<IFunction> parameterFunctions)
		: base(parameterFunctions)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Dictionary | ProgVariableTypes.Text;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error || !TryGetMediaContext(out var context))
		{
			return StatementResult.Error;
		}

		if (context.Process is null)
		{
			ErrorMessage = "The waitmediaevent function requires a running computer-program process.";
			return StatementResult.Error;
		}

		var pending = context.ConsumePendingMediaInput();
		if (pending is not null)
		{
			Result = CreateMetadataDictionary(pending);
			return StatementResult.Normal;
		}

		if (!context.Gameworld.ComputerMediaService.TryResolveMediaInput(context.Host,
			TextArgument(ParameterFunctions[0]), out var endpoint, out var error))
		{
			ErrorMessage = error;
			return StatementResult.Error;
		}

		throw new ComputerProgramWaitException(ComputerProcessWaitType.Media,
			ComputerProcessWaitArguments.CreateMedia(endpoint));
	}

	private static DictionaryVariable CreateMetadataDictionary(MediaPacket packet)
	{
		return new DictionaryVariable(new Dictionary<string, IProgVariable>(StringComparer.InvariantCultureIgnoreCase)
		{
			["event"] = new TextVariable(packet.Kind.ToString()),
			["source"] = new TextVariable($"{packet.Source.ItemId}/{packet.Source.ComponentId}/{packet.Source.EndpointKey}"),
			["capabilities"] = new TextVariable(packet.Capabilities.ToString()),
			["timestamputc"] = new TextVariable(packet.TimestampUtc.ToUniversalTime().ToString("O")),
			["sequence"] = new TextVariable(packet.Sequence.ToString()),
			["recordingid"] = new TextVariable("0"),
			["feed"] = new TextVariable(string.Empty),
			["jobid"] = new TextVariable("0")
		}, ProgVariableTypes.Text);
	}
}

internal sealed class PublishMediaFeedFunction : ComputerMediaBuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"publishmediafeed",
			[ProgVariableTypes.Text, ProgVariableTypes.Text, ProgVariableTypes.Boolean],
			(pars, _) => new PublishMediaFeedFunction(pars),
			["input", "feed", "ispublic"],
			["The local media input name", "The feed name", "Whether the feed is public"],
			"Publishes a local media input as a network feed using the host's existing feed ACL configuration, and returns whether it was configured.",
			"Computers",
			ProgVariableTypes.Boolean,
			allowedContexts: ComputerRuntimeFunctionContexts.ProgramOnly));
	}

	private PublishMediaFeedFunction(IList<IFunction> parameterFunctions)
		: base(parameterFunctions)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Boolean;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error || !TryGetMediaContext(out var context))
		{
			return StatementResult.Error;
		}

		var isPublic = ParameterFunctions[2].Result?.GetObject as bool? ?? false;
		Result = new BooleanVariable(context.Gameworld.ComputerMediaNetworkService.PublishFeed(context.Host,
			TextArgument(ParameterFunctions[0]), TextArgument(ParameterFunctions[1]), isPublic, out _));
		return StatementResult.Normal;
	}
}

internal sealed class SubscribeMediaFeedFunction : ComputerMediaBuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"subscribemediafeed",
			[ProgVariableTypes.Text, ProgVariableTypes.Text, ProgVariableTypes.Boolean],
			(pars, _) => new SubscribeMediaFeedFunction(pars),
			["address", "output", "savedsubscription"],
			["A host-address/feed value", "The local media output name", "True to use a pre-authorized saved private subscription"],
			"Subscribes the host to a public feed or validates an existing saved private subscription. Programs never receive network credentials.",
			"Computers",
			ProgVariableTypes.Boolean,
			allowedContexts: ComputerRuntimeFunctionContexts.ProgramOnly));
	}

	private SubscribeMediaFeedFunction(IList<IFunction> parameterFunctions)
		: base(parameterFunctions)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Boolean;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error || !TryGetMediaContext(out var context))
		{
			return StatementResult.Error;
		}

		var savedSubscription = ParameterFunctions[2].Result?.GetObject as bool? ?? false;
		Result = new BooleanVariable(context.Gameworld.ComputerMediaNetworkService.SubscribeFromProgram(context.Host,
			TextArgument(ParameterFunctions[0]), TextArgument(ParameterFunctions[1]), savedSubscription, out _));
		return StatementResult.Normal;
	}
}

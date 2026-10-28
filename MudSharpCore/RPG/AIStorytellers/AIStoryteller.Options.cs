#nullable enable
#pragma warning disable OPENAI001
using System.Globalization;
using System.Xml.Linq;
using OpenAI.Responses;

namespace MudSharp.RPG.AIStorytellers;

public partial class AIStoryteller
{
	public int EventOutputTokens { get; private set; } = 4096;
	public int TimeOutputTokens { get; private set; } = 4096;
	public int AttentionOutputTokens { get; private set; } = 2048;
	public int RequestTimeoutSeconds { get; private set; } = 120;
	public int BatchWindowSeconds { get; private set; }
	public bool UseExplicitCache { get; private set; } = true;
	private XElement? _preservedExecutionOptions;

	private void LoadExecutionOptions(XElement? options)
	{
		AttentionOutputTokens = AttentionClassifierReasoningEffort == ResponseReasoningEffortLevel.None ? 256 : 2048;
		if (options is null) return;
		_preservedExecutionOptions = new XElement(options);
		EventOutputTokens = ReadOption(options, "EventOutputTokens", 4096, 128, 128000);
		TimeOutputTokens = ReadOption(options, "TimeOutputTokens", 4096, 128, 128000);
		AttentionOutputTokens = ReadOption(options, "AttentionOutputTokens", AttentionOutputTokens, 128, 128000);
		RequestTimeoutSeconds = ReadOption(options, "RequestTimeoutSeconds", 120, 5, 600);
		BatchWindowSeconds = ReadOption(options, "BatchWindowSeconds", 0, 0, 60);
		UseExplicitCache = (string?)options.Element("CachePolicy") != "provider";
	}

	private static int ReadOption(XElement element, string name, int fallback, int minimum, int maximum) =>
		int.TryParse((string?)element.Element(name), NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
		&& value >= minimum && value <= maximum ? value : fallback;

	internal XElement SaveExecutionOptions()
	{
		var root = _preservedExecutionOptions is null ? new XElement("ExecutionOptions") : new XElement(_preservedExecutionOptions);
		root.SetAttributeValue("version", 1);
		root.SetElementValue("EventOutputTokens", EventOutputTokens);
		root.SetElementValue("TimeOutputTokens", TimeOutputTokens);
		root.SetElementValue("AttentionOutputTokens", AttentionOutputTokens);
		root.SetElementValue("RequestTimeoutSeconds", RequestTimeoutSeconds);
		root.SetElementValue("BatchWindowSeconds", BatchWindowSeconds);
		root.SetElementValue("CachePolicy", UseExplicitCache ? "stable" : "provider");
		return root;
	}

	internal static bool SupportsExplicitCache(string model) => model is
		"gpt-6-luna" or "gpt-6-sol" or "gpt-6-astra" or "gpt-6.1-terra" or "gpt-6.1-sol" or
		"gpt-5.6-luna" or "gpt-5.6-terra" or "gpt-5.6-sol" or "gpt-5.6";

	internal static bool IsSupportedEffort(string model, ResponseReasoningEffortLevel effort)
	{
		if (SupportsExplicitCache(model))
			return effort != ResponseReasoningEffortLevel.Minimal &&
			       (!(model is "gpt-6-astra" or "gpt-6.1-sol") || effort != ResponseReasoningEffortLevel.None);
		if (model is "gpt-5" or "gpt-5-mini" or "gpt-5-nano")
			return effort == ResponseReasoningEffortLevel.Minimal || effort == ResponseReasoningEffortLevel.Low ||
			       effort == ResponseReasoningEffortLevel.Medium || effort == ResponseReasoningEffortLevel.High;
		return true; // Unknown model IDs remain usable; the provider validates them.
	}

	private bool BuildingCommandExecutionOption(ICharacter actor, StringStack command, string option)
	{
		if (option == "cache")
		{
			var policy = command.PopSpeech().ToLowerInvariant();
			if (policy is not ("stable" or "provider"))
			{
				actor.OutputHandler.Send("Use cache stable or cache provider.");
				return false;
			}
			UseExplicitCache = policy == "stable";
		}
		else
		{
			var scope = AIStorytellerPromptScope.Event;
			if (option == "outputtokens" && !TryParsePromptScope(command.PopSpeech(), out scope))
			{
				actor.OutputHandler.Send("Specify event, time or attention followed by the output token budget.");
				return false;
			}
			var min = option == "batch" ? 0 : option == "timeout" ? 5 : 128;
			var max = option == "batch" ? 60 : option == "timeout" ? 600 : 128000;
			if (!int.TryParse(command.PopSpeech(), out var value) || value < min || value > max || !command.IsFinished)
			{
				actor.OutputHandler.Send($"Specify a whole number from {min.ToString("N0", actor)} to {max.ToString("N0", actor)}.");
				return false;
			}
			if (option == "batch") BatchWindowSeconds = value;
			else if (option == "timeout") RequestTimeoutSeconds = value;
			else if (scope == AIStorytellerPromptScope.Time) TimeOutputTokens = value;
			else if (scope == AIStorytellerPromptScope.Attention) AttentionOutputTokens = value;
			else EventOutputTokens = value;
		}
		Changed = true;
		actor.OutputHandler.Send("Storyteller request settings updated.");
		return true;
	}
}

using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Magic.Casting;

#nullable enable
namespace MudSharp.Magic.Resources;

public partial class SimpleMagicResource : IMagicResourceCapacity
{
	public MagicResourceAttributeCapacity? AttributeCapacity { get; private set; }
	private XElement? _unreadableAttributeCapacity;
	public bool HasAttributeCapacity => AttributeCapacity is not null || _unreadableAttributeCapacity is not null;

	private void LoadAttributeCapacity(XElement root)
	{
		if (root.Element("AttributeCapacity") is not { } xml) return;
		try
		{
			var basis = (string?)xml.Attribute("basis") ?? "effective";
			if ((int?)xml.Attribute("version") != 1 || basis is not ("raw" or "effective")) throw new FormatException();
			var attribute = (long)xml.Attribute("attribute")!; var expression = (long)xml.Attribute("expression")!;
			if (attribute <= 0 || expression <= 0) throw new FormatException();
			AttributeCapacity = new(attribute, expression, basis == "raw");
		}
		catch (Exception ex) when (ex is FormatException or OverflowException or ArgumentException or InvalidOperationException)
		{
			_unreadableAttributeCapacity = new(xml);
		}
	}

	private XElement? SaveAttributeCapacity() => _unreadableAttributeCapacity is not null ? new(_unreadableAttributeCapacity) :
		AttributeCapacity is { } c ? new XElement("AttributeCapacity", new XAttribute("version", 1),
			new XAttribute("attribute", c.AttributeId), new XAttribute("expression", c.ExpressionId),
			new XAttribute("basis", c.UseRawAttribute ? "raw" : "effective")) : null;

	public string? AttributeCapacityError()
	{
		if (_unreadableAttributeCapacity is not null) return "The attribute capacity XML is unreadable; repair it explicitly.";
		if (AttributeCapacity is not { } c) return null;
		if (Gameworld.Traits.Get(c.AttributeId) is not IAttributeDefinition { OwnerScope: TraitOwnerScope.Body })
			return "The selected capacity attribute must be a current native body-owned attribute.";
		if (Gameworld.TraitExpressions.Get(c.ExpressionId) is not TraitExpression expression)
			return "The selected capacity expression is missing or unsupported.";
		if (expression.HasErrors()) return $"Capacity expression: {expression.Error}";
		if (expression.Formula.FunctionNames.Any(x => x.EqualToAny("rand", "drand", "dice")))
			return "Attribute capacity expressions must not sample randomness.";
		if (expression.Parameters.Any(x => x.Key.EqualTo("variable") || x.Value.Trait is not IAttributeDefinition { OwnerScope: TraitOwnerScope.Body }))
			return "Capacity expression bindings must be native body attributes; variable is reserved for the selected attribute.";
		if (expression.NumericalBindingNames.Except(expression.Parameters.Keys.Append("variable"), StringComparer.OrdinalIgnoreCase).Any() ||
			expression.Formula.ParameterNames.Except(expression.Parameters.Keys.Append("variable"), StringComparer.OrdinalIgnoreCase).Any())
			return "Capacity expressions may reference only the selected variable and explicitly bound native attributes.";
		return null;
	}

	private bool TryAttributeCapacity(ICharacter actor, out double cap, out string? error)
	{
		cap = double.NaN; error = AttributeCapacityError();
		if (error is not null) return false;
		var c = AttributeCapacity!;
		var holder = MagicCastingService.ReserveHolder(actor, Id);
		if (holder is MudSharp.Character.Character { CastingCapacityRestorationActive: true })
		{
			error = "Capacity attributes are still being restored.";
			return false;
		}
		var attribute = Gameworld.Traits.Get(c.AttributeId)!;
		var expression = (TraitExpression)Gameworld.TraitExpressions.Get(c.ExpressionId)!;
		if (!holder.HasTrait(attribute) || expression.Parameters.Values.Any(x => !holder.HasTrait(x.Trait)))
		{
			error = "A required native capacity attribute is missing from the resource holder.";
			return false;
		}
		try
		{
			var bindings = c.UseRawAttribute
				? expression.Parameters.ToDictionary(x => x.Key, x => holder.TraitRawValue(x.Value.Trait), StringComparer.OrdinalIgnoreCase)
				: new Dictionary<string, double>(expression.CaptureNumericalBindings(holder, attribute, TraitBonusContext.None), StringComparer.OrdinalIgnoreCase);
			if (c.UseRawAttribute) bindings["variable"] = holder.TraitRawValue(attribute);
			if (bindings.Values.Any(x => !double.IsFinite(x))) { error = "A capacity attribute is not finite."; return false; }
			if (!expression.Formula.TryEvaluateDoubleWith(bindings.ToDictionary(x => x.Key, x => (object)x.Value, StringComparer.OrdinalIgnoreCase), out cap, out var diagnostic))
			{
				error = $"Capacity expression: {diagnostic}";
				return false;
			}
			if (cap < 0) { error = "Attribute capacity must be non-negative."; return false; }
			return true;
		}
		catch (Exception ex)
		{
			error = $"Capacity attributes cannot be resolved: {ex.GetBaseException().Message}";
			return false;
		}
	}

	public bool TryGetResourceCap(IHaveMagicResource holder, out double cap, out string? error)
	{
		if (HasAttributeCapacity && holder is ICharacter actor) return TryAttributeCapacity(actor, out cap, out error);
		cap = ResourceCap(holder); error = null;
		return true;
	}

	private void CapacityDefinitionChanged()
	{
		Changed = true;
		foreach (var actor in Gameworld.Characters.DistinctBy(x => MagicCastingService.Owner(x).Id))
			Gameworld.MagicCasting?.NotifyCapacityChange(actor);
	}

	private void ClearAttributeCapacity()
	{
		AttributeCapacity = null; _unreadableAttributeCapacity = null;
	}

	private bool BuildingCommandAttributeCapacity(ICharacter actor, StringStack command)
	{
		var name = command.PopSpeech();
		if (name.EqualTo("none") && command.IsFinished)
		{
			ClearAttributeCapacity(); CapacityDefinitionChanged();
			actor.OutputHandler.Send("Character capacity now uses the existing resource cap Prog.");
			return true;
		}
		var expressionName = command.PopSpeech(); var basis = command.IsFinished ? "effective" : command.PopForSwitch();
		if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(expressionName) || !command.IsFinished || basis is not ("raw" or "effective"))
		{
			actor.OutputHandler.Send("Use capattribute <native attribute> <trait expression> [raw|effective], or capattribute none.");
			return false;
		}
		if (Gameworld.Traits.GetByIdOrName(name) is not IAttributeDefinition attribute || Gameworld.TraitExpressions.GetByIdOrName(expressionName) is not { } expression)
		{
			actor.OutputHandler.Send("Select an existing native attribute and trait expression explicitly.");
			return false;
		}
		var previous = AttributeCapacity; var unreadable = _unreadableAttributeCapacity;
		AttributeCapacity = new(attribute.Id, expression.Id, basis == "raw"); _unreadableAttributeCapacity = null;
		if (AttributeCapacityError() is { } error)
		{
			AttributeCapacity = previous; _unreadableAttributeCapacity = unreadable;
			actor.OutputHandler.Send(error.ColourError());
			return false;
		}
		CapacityDefinitionChanged();
		actor.OutputHandler.Send($"Character capacity now uses {attribute.Name.ColourName()} ({basis.ColourValue()}) in expression {expression.Name.ColourName()}. Raising capacity does not fill the reserve.");
		return true;
	}
}

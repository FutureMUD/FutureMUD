#nullable enable

using System.Xml.Linq;
using MudSharp.Construction;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public sealed class PrivatePropertyEffect : Effect
{
	private readonly FrameworkItemReference _controllerReference;
	private IFrameworkItem? _controller;

	public PrivatePropertyEffect(ICell owner, IFrameworkItem controller) : base(owner)
	{
		_controller = controller;
		_controllerReference = new FrameworkItemReference(controller.Id, controller.FrameworkItemType, owner.Gameworld);
	}

	private PrivatePropertyEffect(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		var root = effect.Element("Effect")!;
		_controllerReference = new FrameworkItemReference(
			long.Parse(root.Element("ControllerId")!.Value),
			root.Element("ControllerType")!.Value,
			Gameworld);
	}

	public IFrameworkItem? Controller => _controller ??= _controllerReference.GetItem;
	public long ControllerId => _controllerReference.Id;
	public string ControllerType => _controllerReference.FrameworkItemType;

	protected override string SpecificEffectType => "PrivateProperty";
	public override bool SavingEffect => true;

	public override string Describe(IPerceiver voyeur)
	{
		return Controller is { } controller
			? $"This location is private property controlled by {controller.Name.ColourName()}."
			: $"This location is private property controlled by missing {ControllerType} #{ControllerId:N0}.";
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("ControllerType", ControllerType),
			new XElement("ControllerId", ControllerId));
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("PrivateProperty", (effect, owner) => new PrivatePropertyEffect(effect, owner));
	}
}

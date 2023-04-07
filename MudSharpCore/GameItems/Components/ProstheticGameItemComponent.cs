using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using System.Linq;

namespace MudSharp.GameItems.Components;

public class ProstheticGameItemComponent : GameItemComponent, IProsthetic
{
	public ProstheticGameItemComponent(ProstheticGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public ProstheticGameItemComponent(MudSharp.Models.GameItemComponent component,
		ProstheticGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public ProstheticGameItemComponent(ProstheticGameItemComponent rhs, IGameItem newParent, bool temporary = false) :
		base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (ProstheticGameItemComponentProto)newProto;
	}

	private void LoadFromXml(XElement root)
	{
	}

	#region Overrides of GameItemComponent

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new ProstheticGameItemComponent(this, newParent, temporary);
	}

	protected override string SaveToXml()
	{
		return "<Definition/>";
	}

	private ProstheticGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	#endregion

	#region Implementation of IProsthetic

	public bool Obvious => _prototype.Obvious;
	public bool Functional => _prototype.Functional;
	public IBodyPrototype TargetBody => _prototype.TargetBody;
	public IBodypart TargetBodypart => _prototype.TargetBodypart;

	public IEnumerable<IBodypart> IncludedParts => TargetBody
	                                               .BodypartsFor(_prototype.Race, _prototype.Gender.Enum)
	                                               .Where(x => x.DownstreamOfPart(TargetBodypart))
	                                               .Plus(TargetBodypart);

	public IBody InstalledBody { get; set; }

	public void InstallProsthetic(IBody body)
	{
		InstalledBody = body;
		Changed = true;
	}

	public void RemoveProsthetic()
	{
		InstalledBody = null;
		Changed = true;
	}

	#endregion

	#region Overrides of GameItemComponent

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		return type == DescriptionType.Full
			? $"{description}\n\n{$"It is a prosthetic limb for the {TargetBody.Name.Colour(Telnet.Green)} body type, attaching to the {TargetBodypart.FullDescription().Colour(Telnet.Green)} bodypart. It {(Obvious ? "is" : "is not")} obviously fake, and {(Functional ? "functions as a natural limb would" : "is not functional, and purely asthetic.")}.".Colour(Telnet.Yellow)}"
			: base.Decorate(voyeur, name, description, type, colour, flags);
	}

	/// <summary>
	///     This property indicates whether this IGameItemComponent acts as a decorator for the IGameItem's description
	/// </summary>
	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Full;
	}

	#endregion
}
#nullable enable
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Grids;
using MudSharp.Events;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class TelecommunicationsGridCreatorGameItemComponent : GameItemComponent
{
	protected TelecommunicationsGridCreatorGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (TelecommunicationsGridCreatorGameItemComponentProto)newProto;
	}

	public ITelecommunicationsGrid Grid { get; protected set; }

	public TelecommunicationsGridCreatorGameItemComponent(TelecommunicationsGridCreatorGameItemComponentProto proto,
		IGameItem parent, ICharacter? loader, bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
		Grid = CreateGrid(loader?.Location, temporary);
	}

	public TelecommunicationsGridCreatorGameItemComponent(Models.GameItemComponent component,
		TelecommunicationsGridCreatorGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public TelecommunicationsGridCreatorGameItemComponent(TelecommunicationsGridCreatorGameItemComponent rhs,
		IGameItem newParent, bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		Grid = temporary ? rhs.Grid : CreateGridCopy(rhs.Grid);
	}

	private void LoadFromXml(XElement root)
	{
		var gridId = long.Parse(root.Element("Grid")!.Value);
		Grid = Gameworld.Grids.Get(gridId) as ITelecommunicationsGrid ?? CreateGrid(Parent.TrueLocations.FirstOrDefault());
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new TelecommunicationsGridCreatorGameItemComponent(this, newParent, temporary);
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Grid", Grid.Id)
		).ToString();
	}

	public override void Delete()
	{
		base.Delete();
		Grid?.Delete();
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		if (type == EventType.ItemFinishedLoading && !Grid.Locations.Any() && Parent.TrueLocations.Any())
		{
			Grid.ExtendTo(Parent.TrueLocations.First());
		}

		return base.HandleEvent(type, arguments);
	}

	public override bool HandlesEvent(params EventType[] types)
	{
		return types.Contains(EventType.ItemFinishedLoading) || base.HandlesEvent(types);
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Full;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		if (type != DescriptionType.Full)
		{
			return base.Decorate(voyeur, name, description, type, colour, flags);
		}

		return string.Join("\n",
			description,
			"",
		$"The telecommunications grid uses the prefix {Grid.Prefix.ColourValue()} and {Grid.NumberLength.ToString("N0", voyeur).ColourValue()} subscriber digits.",
		$"Hosted voicemail is {(Grid.HostedVoicemailEnabled ? "enabled".ColourValue() : "disabled".ColourError())} and uses access number {Grid.HostedVoicemailAccessNumber.ColourValue()}."
		);
	}

	private ITelecommunicationsGrid CreateGrid(ICell? initialLocation, bool temporary = false)
	{
		var grid = new TelecommunicationsGrid(Gameworld, initialLocation, _prototype.Prefix, _prototype.NumberLength,
			_prototype.HostedVoicemailEnabled, _prototype.HostedVoicemailAccessCode);
		if (temporary)
		{
			return grid;
		}

		Gameworld.Add(grid);
		Gameworld.SaveManager.DirectInitialise((ILateInitialisingItem)grid);
		return grid;
	}

	private ITelecommunicationsGrid CreateGridCopy(ITelecommunicationsGrid source)
	{
		var grid = new TelecommunicationsGrid(source);
		Gameworld.Add(grid);
		Gameworld.SaveManager.DirectInitialise((ILateInitialisingItem)grid);
		return grid;
	}
}

using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;

namespace MudSharp.Construction.Boundary;

internal class NonCardinalExitTemplate : SaveableItem, INonCardinalExitTemplate
{
	private string _destinationInboundPreface;

	private string _destinationOutboundPreface;

	private string _inboundVerb;

	private string _originInboundPreface;

	private string _originOutboundPreface;

	private string _outboundVerb;

	public NonCardinalExitTemplate(MudSharp.Models.NonCardinalExitTemplate template, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		LoadFromDatabase(template);
	}

	public override string FrameworkItemType => "NonCardinalExitTemplate";

	public string OriginOutboundPreface
	{
		get => _originOutboundPreface;
		set
		{
			_originOutboundPreface = value;
			Changed = true;
		}
	}

	public string OriginInboundPreface
	{
		get => _originInboundPreface;
		set
		{
			_originInboundPreface = value;
			Changed = true;
		}
	}

	public string DestinationOutboundPreface
	{
		get => _destinationOutboundPreface;
		set
		{
			_destinationOutboundPreface = value;
			Changed = true;
		}
	}

	public string DestinationInboundPreface
	{
		get => _destinationInboundPreface;
		set
		{
			_destinationInboundPreface = value;
			Changed = true;
		}
	}

	public string OutboundVerb
	{
		get => _outboundVerb;
		set
		{
			_outboundVerb = value;
			Changed = true;
		}
	}

	public string InboundVerb
	{
		get => _inboundVerb;
		set
		{
			_inboundVerb = value;
			Changed = true;
		}
	}

	public override void Save()
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.NonCardinalExitTemplates.Find(Id);
			dbitem.OriginInboundPreface = OriginInboundPreface;
			dbitem.OriginOutboundPreface = OriginOutboundPreface;
			dbitem.DestinationInboundPreface = DestinationInboundPreface;
			dbitem.DestinationOutboundPreface = DestinationOutboundPreface;
			dbitem.OutboundVerb = OutboundVerb;
			dbitem.InboundVerb = InboundVerb;
		}

		Changed = false;
	}

	private void LoadFromDatabase(MudSharp.Models.NonCardinalExitTemplate template)
	{
		_noSave = true;
		_id = template.Id;
		_name = template.Name;
		OriginInboundPreface = template.OriginInboundPreface;
		OriginOutboundPreface = template.OriginOutboundPreface;
		DestinationInboundPreface = template.DestinationInboundPreface;
		DestinationOutboundPreface = template.DestinationOutboundPreface;
		OutboundVerb = template.OutboundVerb;
		InboundVerb = template.InboundVerb;
		_noSave = false;
	}
}
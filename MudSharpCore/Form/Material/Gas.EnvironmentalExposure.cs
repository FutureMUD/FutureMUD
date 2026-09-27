#nullable enable

namespace MudSharp.Form.Material;

public partial class Gas
{
	private readonly List<ILiquidSurfaceReaction> _exposureReactions = new();
	public IEnumerable<IEnvironmentalReaction> EnvironmentalReactions => _exposureReactions.OfType<IEnvironmentalReaction>();
	private string SaveExposureReactions() => new XElement("Reactions", _exposureReactions.OfType<LiquidSurfaceReaction>().Select(x => x.SaveToXml())).ToString();
	private void LoadExposureReactions(string? xml)
	{
		_exposureReactions.Clear();
		if (string.IsNullOrWhiteSpace(xml)) return;
		try { _exposureReactions.AddRange(XElement.Parse(xml).Elements("Reaction").Select(x => new LiquidSurfaceReaction(x, Gameworld))); }
		catch (System.Xml.XmlException) { Gameworld.SystemMessage($"Gas #{Id}: invalid exposure XML; reactions quarantined.", true); }
	}
	private bool BuildingCommandExposure(ICharacter actor, StringStack command)
	{
		if (!ExposureReactionBuilder.Edit(actor, command, _exposureReactions, true, this)) return false;
		Changed = true; return true;
	}
}

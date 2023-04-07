using MudSharp.Framework;

namespace MudSharp.Form.Colour;

public class Colour : FrameworkItem, IColour
{
	public bool _isValid;

	public Colour(MudSharp.Models.Colour proto)
	{
		_id = proto.Id;
		_name = proto.Name;
		Red = proto.Red;
		Green = proto.Green;
		Blue = proto.Blue;
		Basic = (BasicColour)proto.Basic;
		Fancy = string.IsNullOrEmpty(proto.Fancy) ? _name : proto.Fancy;
	}

	public override string FrameworkItemType => "Colour";

	/// <summary>
	///     Returns the Red component of the RGB value of this colour
	/// </summary>
	public int Red { get; protected set; }

	/// <summary>
	///     Returns the Green component of the RGB value of this colour
	/// </summary>
	public int Green { get; protected set; }

	/// <summary>
	///     Returns the Blue component of the RGB value of this colour
	/// </summary>
	public int Blue { get; protected set; }

	/// <summary>
	///     Returns the BasicColour equivalent of this colour. For instance, "Navy Blue" would return BasicColour.Blue
	/// </summary>
	public BasicColour Basic { get; protected set; }

	/// <summary>
	///     A fancified string version of the colour, for example, "Midnight Black" might have "The colour of a starless
	///     midnight sky" as its Fancy.
	/// </summary>
	public string Fancy { get; protected set; }
}

public interface IHaveColour
{
	IColour Colour { get; }
}
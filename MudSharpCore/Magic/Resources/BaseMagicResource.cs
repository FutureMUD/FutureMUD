using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;

namespace MudSharp.Magic.Resources;

public abstract class BaseMagicResource : FrameworkItem, IMagicResource, IHaveFuturemud
{
	public static IMagicResource LoadResource(Models.MagicResource resource, IFuturemud gameworld)
	{
		switch (resource.Type.ToLowerInvariant())
		{
			case "simple":
				return new SimpleMagicResource(resource, gameworld);
		}

		throw new NotImplementedException("Unknown MagicResource type: " + resource.Type);
	}

	protected BaseMagicResource(Models.MagicResource resource, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = resource.Id;
		_name = resource.Name;
		ResourceType = (MagicResourceType)resource.MagicResourceType;
		// TODO - load shortname
		ShortName = Name;
	}

	#region Overrides of Item

	public sealed override string FrameworkItemType => "MagicResource";

	#endregion

	#region Implementation of IMagicResource

	public MagicResourceType ResourceType { get; set; }
	public string ShortName { get; set; }
	public abstract bool ShouldStartWithResource(IHaveMagicResource thing);
	public abstract double StartingResourceAmount(IHaveMagicResource thing);
	public abstract double ResourceCap(IHaveMagicResource thing);
	public string BottomColour { get; set; } = Telnet.Magenta.Colour;
	public string MidColour { get; set; } = Telnet.BoldMagenta.Colour;
	public string TopColour { get; set; } = $"{Telnet.RESET}{Telnet.BoldPink}";

	public virtual string ClassicPromptString(double percentage)
	{
		if (percentage <= 0.0)
		{
			return $"      ";
		}
		else if (percentage <= 0.1667)
		{
			return $"{BottomColour}|     {Telnet.RESET}";
		}
		else if (percentage <= 0.3333)
		{
			return $"{BottomColour}||    {Telnet.RESET}";
		}
		else if (percentage <= 0.5)
		{
			return $"{BottomColour}||{MidColour}|   {Telnet.RESET}";
		}
		else if (percentage <= 0.6667)
		{
			return $"{BottomColour}||{MidColour}||  {Telnet.RESET}";
		}
		else if (percentage <= 0.8335)
		{
			return $"{BottomColour}||{MidColour}||{TopColour}| {Telnet.RESET}";
		}
		else
		{
			return $"{BottomColour}||{MidColour}||{TopColour}||{Telnet.RESET}";
		}
	}

	#endregion

	#region Implementation of IHaveFuturemud

	public IFuturemud Gameworld { get; }

	#endregion
}
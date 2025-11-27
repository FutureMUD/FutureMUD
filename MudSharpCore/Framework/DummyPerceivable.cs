using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Construction;
using MudSharp.Form.Shape;

namespace MudSharp.Framework;

public class DummyPerceivable : TemporaryPerceivable
{
	public string SDesc { get; set; }
	public string FDesc { get; set; }
	public Func<IPerceiver, string> SDescFunc { get; set; }
	public Func<IPerceiver, string> FDescFunc { get; set; }
	public ANSIColour CustomColour { get; set; }

	public DummyPerceivable(string sdesc = "a thing", string fdesc = "it is a thing", ICell location = null,
		bool sentient = false, double illumination = 0.0, ANSIColour customColour = null)
	{
		SDesc = sdesc;
		FDesc = fdesc;
		_location = location;
		_sentient = sentient;
		_illuminationProvided = illumination;
		CustomColour = customColour;
	}

	public DummyPerceivable(Func<IPerceiver, string> sdescFunc, Func<IPerceiver, string> fdescFunc = null,
		ICell location = null, bool sentient = false, double illumination = 0.0, ANSIColour customColour = null)
	{
		SDescFunc = sdescFunc;
		FDescFunc = fdescFunc ?? (voyeur => "it is a thing");
		_location = location;
		_sentient = sentient;
		_illuminationProvided = illumination;
		CustomColour = customColour;
	}

	private bool _sentient;
	public override bool Sentient => _sentient;

	private double _illuminationProvided;
	public override double IlluminationProvided => _illuminationProvided;

	public override string HowSeen(IPerceiver voyeur, bool proper = false, DescriptionType type = DescriptionType.Short,
		bool colour = true, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
	{
		switch (type)
		{
			case DescriptionType.Short:
				return (SDescFunc?.Invoke(voyeur) ?? SDesc).FluentProper(proper)
				                                           .FluentColourIncludingReset(
					                                           CustomColour ??
					                                           (Sentient ? Telnet.Magenta : Telnet.Green), colour);
			case DescriptionType.Possessive:
				return $"{SDescFunc?.Invoke(voyeur) ?? SDesc}'s".FluentProper(proper)
				                                                .FluentColourIncludingReset(
					                                                CustomColour ?? (Sentient
						                                                ? Telnet.Magenta
						                                                : Telnet.Green), colour);
			case DescriptionType.Long:
				return
					$"{(SDescFunc?.Invoke(voyeur) ?? SDesc).FluentProper(proper).FluentColourIncludingReset(CustomColour ?? (Sentient ? Telnet.Magenta : Telnet.Green), colour)} is here";
			case DescriptionType.Full:
			case DescriptionType.Contents:
			case DescriptionType.Evaluate:
				return (FDescFunc?.Invoke(voyeur) ?? FDesc).FluentProper(proper);
		}

		return "";
	}

	private ICell _location;
	public override ICell Location => _location;

	#region Overrides of TemporaryPerceivable

	/// <inheritdoc />
	public override bool ColocatedWith(IPerceivable otherThing)
	{
		return Location == otherThing?.Location && RoomLayer == otherThing?.RoomLayer;
	}

	#endregion

	public override string FrameworkItemType => "DummyPerceivable";
}
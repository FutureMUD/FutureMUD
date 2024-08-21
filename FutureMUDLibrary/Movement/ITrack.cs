using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.TimeAndDate;

namespace MudSharp.Movement;
#nullable enable

[Flags]
public enum TrackCircumstances
{
	None = 0,
	Bleeding = 1,
	Fleeing = 2,
	Careful = 4,
	Dragged = 8,
}

public interface ITrack : IFrameworkItem, ISaveable
{
	ICharacter Character { get; }
	IBodyPrototype BodyProtoType { get; }
	ICell Cell { get; }
	RoomLayer RoomLayer { get; }
	IExit? FromExit { get; }
	IExit? ToExit { get; }
	IMoveSpeed? FromSpeed { get; }
	IMoveSpeed? ToSpeed { get; }
	TrackCircumstances TrackCircumstances { get; }
	ExertionLevel ExertionLevel { get; }
	MudDateTime MudDateTime { get; }
	double TrackIntensityVisual { get; set; }
	double TrackIntensityOlfactory { get; set; }
	bool TurnedAround { get; set; }
	string DescribeForTracksCommand(ICharacter actor);
	bool Deleted { get; set; }
}

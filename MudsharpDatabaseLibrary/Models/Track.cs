using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Models;
public class Track
{
	public Track()
	{

	}

	public long Id { get; set; }

	public long CharacterId { get; set; }
	public virtual Character Character { get; set; }
	public long BodyPrototypeId { get; set; }
	public virtual BodyProto BodyPrototype { get; set; }
	public long CellId { get; set; }
	public virtual Cell Cell { get; set; }
	public int RoomLayer { get; set; }
	public string MudDateTime { get; set; }
	public long? FromDirectionExitId { get; set; }
	public virtual Exit FromDirectionExit { get; set; }
	public long? ToDirectionExitId { get; set; }
	public virtual Exit ToDirectionExit { get; set; }
	public int TrackCircumstances { get; set; }
	public long? FromMoveSpeedId { get; set; }
	public virtual MoveSpeed FromMoveSpeed { get; set; }
	public long? ToMoveSpeedId { get; set; }
	public virtual MoveSpeed ToMoveSpeed { get; set; }
	public int ExertionLevel { get; set; }
	public double TrackIntensityVisual { get; set; }
	public double TrackIntensityOlfactory { get; set; }
	public bool TurnedAround { get; set; }
}

using MudSharp.Body.Position;
using MudSharp.Framework;

namespace MudSharp.Movement;

public class MoveSpeed : FrameworkItem, IMoveSpeed
{
	public MoveSpeed(MudSharp.Models.MoveSpeed speed)
	{
		_id = speed.Id;
		_name = speed.Alias;
		Multiplier = speed.Multiplier;
		FirstPersonVerb = speed.FirstPersonVerb;
		ThirdPersonVerb = speed.ThirdPersonVerb;
		PresentParticiple = speed.PresentParticiple;
		StaminaMultiplier = speed.StaminaMultiplier;
		Position = PositionState.GetState(speed.PositionId);
	}

	public override string FrameworkItemType => "MoveSpeed";

	public double Multiplier { get; protected set; }
	public string FirstPersonVerb { get; protected set; }
	public string ThirdPersonVerb { get; protected set; }
	public string PresentParticiple { get; protected set; }
	public double StaminaMultiplier { get; protected set; }

	public IPositionState Position { get; protected set; }
}
namespace MudSharp.Celestial;

public class CelestialTrigger
{
	/// <summary>
	///     The direction of movement of the celestial object that is required for this trigger
	/// </summary>
	protected CelestialMoveDirection _direction;

	/// <summary>
	///     The payload delivered when this trigger goes off, in the form of an echo to the zone
	/// </summary>
	protected string _echo;

	/// <summary>
	///     The angle in radians that will set off this trigger
	/// </summary>
	protected double _threshold;

	public CelestialTrigger()
	{
	}

	public CelestialTrigger(double threshold, CelestialMoveDirection direction, string echo)
	{
		Threshold = threshold;
		Direction = direction;
		Echo = echo;
	}

	public double Threshold
	{
		get => _threshold;
		protected init => _threshold = value;
	}

	public CelestialMoveDirection Direction
	{
		get => _direction;
		protected init => _direction = value;
	}

	public string Echo
	{
		get => _echo;
		protected init => _echo = value;
	}
}
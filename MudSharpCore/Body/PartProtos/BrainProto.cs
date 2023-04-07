using MudSharp.Models;
using MudSharp.Framework;

namespace MudSharp.Body.PartProtos;

public class BrainProto : InternalOrganProto
{
	public BrainProto(BodypartProto proto, IFuturemud game)
		: base(proto, game)
	{
	}

	public BrainProto(BrainProto rhs, string newName) : base(rhs, newName)
	{
	}

	public override IBodypart Clone(string newName)
	{
		return new BrainProto(this, newName);
	}

	public override BodypartTypeEnum BodypartType => BodypartTypeEnum.Brain;

	public override string FrameworkItemType => "BrainProto";

	#region Overrides of InternalOrganProto

	protected override bool AffectedByBloodBuildup => true;
	protected override double BloodVolumeForTotalFailure => 1.0;

	private double _minorConcussionRatio;

	public double MinorConcussionRatio
	{
		get
		{
			if (_minorConcussionRatio == 0.0)
			{
				_minorConcussionRatio = Gameworld.GetStaticDouble("MinorConcussionRatio");
			}

			return _minorConcussionRatio;
		}
	}

	private double _concussionRatio;

	public double ConcussionRatio
	{
		get
		{
			if (_concussionRatio == 0.0)
			{
				_concussionRatio = Gameworld.GetStaticDouble("ConcussionRatio");
			}

			return _concussionRatio;
		}
	}

	private double _majorConcussionRatio;

	public double MajorConcussionRatio
	{
		get
		{
			if (_majorConcussionRatio == 0.0)
			{
				_majorConcussionRatio = Gameworld.GetStaticDouble("MajorConcussionRatio");
			}

			return _majorConcussionRatio;
		}
	}

	public override void HandleChangedOrganFunction(IBody body, double oldFunctionFactor, double newFunctionFactor)
	{
		if (newFunctionFactor == oldFunctionFactor)
		{
			return;
		}

		if (newFunctionFactor < oldFunctionFactor)
		{
			if (newFunctionFactor <= MajorConcussionRatio && oldFunctionFactor > MajorConcussionRatio)
			{
				body.OutputHandler.Send(
					"Your head feels like someone has it in a vice; you're having trouble focusing and your vision is thoroughly blurred."
						.Colour(Telnet.BoldMagenta));
			}
			else if (newFunctionFactor <= ConcussionRatio && oldFunctionFactor > ConcussionRatio)
			{
				body.OutputHandler.Send(
					"You have a whopping headache and your vision is starting to get a little blurry.".Colour(
						Telnet.BoldMagenta));
			}
			else if (newFunctionFactor <= MinorConcussionRatio && oldFunctionFactor > MinorConcussionRatio)
			{
				body.OutputHandler.Send(
					"You have a bit of a headache, and some very minor disorientation.".Colour(Telnet.BoldMagenta));
			}
		}
		else
		{
			if (newFunctionFactor >= MajorConcussionRatio && oldFunctionFactor < MajorConcussionRatio)
			{
				body.OutputHandler.Send(
					"Your head feels a little bit better, though you still have a whopping headache. Your vision is blurry but less so; you also feel a little less disoriented."
						.Colour(Telnet.BoldGreen));
			}

			if (newFunctionFactor >= ConcussionRatio && oldFunctionFactor < ConcussionRatio)
			{
				body.OutputHandler.Send(
					"Your whopping headache has cleared up a little, and your vision is mostly better. You still feel mildly disoriented."
						.Colour(Telnet.BoldGreen));
			}

			if (newFunctionFactor >= MinorConcussionRatio && oldFunctionFactor < MinorConcussionRatio)
			{
				body.OutputHandler.Send("Your headache seems to be clearing up.".Colour(Telnet.BoldGreen));
			}
		}
	}

	#endregion
}
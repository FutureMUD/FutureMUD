using MudSharp.Models;
using MudSharp.Framework;

namespace MudSharp.Body.PartProtos;

public class LungProto : InternalOrganProto
{
	public LungProto(BodypartProto proto, IFuturemud game)
		: base(proto, game)
	{
	}

	public LungProto(LungProto rhs, string newName) : base(rhs, newName)
	{
	}

	public override IBodypart Clone(string newName)
	{
		return new LungProto(this, newName);
	}

	public override BodypartTypeEnum BodypartType => BodypartTypeEnum.Lung;

	public new double PainFactor => 0.0;

	public override bool RequiresSpinalConnection => true;

	public override string FrameworkItemType => "LungProto";

	#region Overrides of InternalOrganProto

	protected override bool AffectedByBloodBuildup => true;
	protected override double BloodVolumeForTotalFailure => 0.2;

	#endregion
}
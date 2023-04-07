using MudSharp.Models;
using MudSharp.Framework;

namespace MudSharp.Body.PartProtos;

public class EsophagusProto : InternalOrganProto
{
	public EsophagusProto(BodypartProto proto, IFuturemud game)
		: base(proto, game)
	{
	}

	public EsophagusProto(EsophagusProto rhs, string newName) : base(rhs, newName)
	{
	}

	public override IBodypart Clone(string newName)
	{
		return new EsophagusProto(this, newName);
	}

	public override BodypartTypeEnum BodypartType => BodypartTypeEnum.Esophagus;

	public new double PainFactor => 0.0;

	public override string FrameworkItemType => "EsophagusProto";

	#region Overrides of InternalOrganProto

	protected override bool AffectedByBloodBuildup => true;
	protected override double BloodVolumeForTotalFailure => 0.2;

	#endregion
}
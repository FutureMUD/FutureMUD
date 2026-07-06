using MudSharp.Character;
using MudSharp.Economy;
using MudSharp.Framework;
using MudSharp.FutureProg;

#nullable enable

namespace MudSharp.Effects.Concrete;

public class HospitalTreatmentPermissionEffect : Effect, IEffectSubtype
{
	public HospitalTreatmentPermissionEffect(ICharacter owner, ICharacter medic, IHospitalServiceRequest request,
		IFutureProg? applicabilityProg = null) : base(owner, applicabilityProg)
	{
		Medic = medic;
		RequestId = request.Id;
	}

	public ICharacter Medic { get; }
	public long RequestId { get; }

	public override string Describe(IPerceiver voyeur)
	{
		return $"Permitting hospital treatment from {Medic.HowSeen(voyeur)} for request #{RequestId.ToString("N0", voyeur)}.";
	}

	protected override string SpecificEffectType => "HospitalTreatmentPermission";
}

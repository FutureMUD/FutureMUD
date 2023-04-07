using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class Notify : Effect, INotifyEffect
{
	public Notify(IPerceivable owner, ICharacter notifyTarget, bool clanNotification)
		: base(owner)
	{
		NotifyTarget = notifyTarget;
		ClanNotification = clanNotification;
	}

	protected override string SpecificEffectType => "Notify";

	public bool ClanNotification { get; set; }
	public ICharacter NotifyTarget { get; set; }

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"Has had a {(ClanNotification ? "clan" : "personal")} notification request from {NotifyTarget.HowSeen(voyeur)}.";
	}

	public override void ExpireEffect()
	{
		Owner.RemoveEffect(this);
	}

	public override string ToString()
	{
		return "Notify Effect";
	}
}
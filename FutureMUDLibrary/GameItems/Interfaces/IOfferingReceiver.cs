using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;

#nullable enable

namespace MudSharp.GameItems.Interfaces;

public enum OfferingConsumptionMode
{
	ManualBurn = 0,
	BurnOnOffer = 1,
	RecordOnly = 2
}

public interface IOfferingReceiver : IContainer
{
	OfferingConsumptionMode ConsumptionMode { get; }
	bool CanOffer(ICharacter actor, IGameItem offering);
	string WhyCannotOffer(ICharacter actor, IGameItem offering);
	bool Offer(ICharacter actor, IGameItem offering, IEmote? playerEmote);
	bool CanBurnOffering(ICharacter actor, IGameItem offering);
	string WhyCannotBurnOffering(ICharacter actor, IGameItem offering);
	bool BurnOffering(ICharacter actor, IGameItem offering, IEmote? playerEmote);
}

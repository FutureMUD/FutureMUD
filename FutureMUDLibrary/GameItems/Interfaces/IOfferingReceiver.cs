using System;
using MudSharp.Character;
using MudSharp.Form.Material;
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
	bool AcceptsLiquidOfferings { get; }
	long LiquidOfferingCount { get; }
	double TotalOfferedLiquidVolume { get; }
	long? LastOffererId { get; }
	string? LastOffererName { get; }
	string? LastOfferedLiquid { get; }
	double LastOfferedLiquidVolume { get; }
	DateTime? LastLiquidOfferingUtc { get; }
	bool CanOffer(ICharacter actor, IGameItem offering);
	string WhyCannotOffer(ICharacter actor, IGameItem offering);
	bool Offer(ICharacter actor, IGameItem offering, IEmote? playerEmote);
	bool CanBurnOffering(ICharacter actor, IGameItem offering);
	string WhyCannotBurnOffering(ICharacter actor, IGameItem offering);
	bool BurnOffering(ICharacter actor, IGameItem offering, IEmote? playerEmote);
	bool CanOfferLiquid(ICharacter actor, IGameItem source, double amount);
	string WhyCannotOfferLiquid(ICharacter actor, IGameItem source, double amount);
	bool OfferLiquid(ICharacter actor, IGameItem source, double amount, IEmote? playerEmote);
}

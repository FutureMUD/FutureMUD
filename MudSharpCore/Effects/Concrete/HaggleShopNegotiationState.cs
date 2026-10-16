#nullable enable
using System.Globalization;
using MudSharp.Economy;
using MudSharp.Framework.Scheduling;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Concrete;

/// <summary>Unscheduled: absolute UTC, including time spent offline, is authoritative.</summary>
public sealed class HaggleShopNegotiationState : Effect
{
	public HaggleShopNegotiationState(ICharacter owner, long shopId, ShopDealApplicability scope,
		HaggleNegotiationScope mode, Difficulty nextDifficulty, int attempts, DateTime expiresAtUtc) : base(owner)
	{
		ShopId = shopId;
		Scope = scope;
		Mode = mode;
		NextDifficulty = nextDifficulty;
		CompletedAttempts = attempts;
		WindowExpiresAtUtc = expiresAtUtc;
	}

	private HaggleShopNegotiationState(XElement root, IPerceivable owner) : base(root, owner)
	{
		var data = root.Element("Haggle")!;
		ShopId = (long)data.Attribute("Shop")!;
		Scope = (ShopDealApplicability)(int)data.Attribute("Scope")!;
		Mode = (HaggleNegotiationScope)(int)data.Attribute("Mode")!;
		NextDifficulty = (Difficulty)(int)data.Attribute("Difficulty")!;
		CompletedAttempts = (int)data.Attribute("Attempts")!;
		WindowExpiresAtUtc = DateTime.Parse(data.Attribute("Expires")!.Value, CultureInfo.InvariantCulture,
			DateTimeStyles.RoundtripKind).ToUniversalTime();
		LoadErrors = WindowExpiresAtUtc <= RuntimeClock.UtcNow;
	}

	public long ShopId { get; }
	public ShopDealApplicability Scope { get; }
	public HaggleNegotiationScope Mode { get; }
	public Difficulty NextDifficulty { get; }
	public int CompletedAttempts { get; }
	public DateTime WindowExpiresAtUtc { get; }
	public override bool SavingEffect => true;
	protected override string SpecificEffectType => "HaggleShopNegotiationState";
	public override bool Applies() => WindowExpiresAtUtc > RuntimeClock.UtcNow;
	protected override XElement SaveDefinition() => new("Haggle",
		new XAttribute("Shop", ShopId), new XAttribute("Scope", (int)Scope), new XAttribute("Mode", (int)Mode),
		new XAttribute("Difficulty", (int)NextDifficulty), new XAttribute("Attempts", CompletedAttempts),
		new XAttribute("Expires", WindowExpiresAtUtc.ToString("O", CultureInfo.InvariantCulture)));
	public static void InitialiseEffectType() => RegisterFactory("HaggleShopNegotiationState",
		(root, owner) => new HaggleShopNegotiationState(root, owner));
	public override string Describe(IPerceiver voyeur) =>
		$"Haggle shop #{ShopId.ToString("N0", voyeur)} ({Scope.DescribeEnum()}): {CompletedAttempts.ToString("N0", voyeur)} completed, next {NextDifficulty.DescribeEnum()}, expires {WindowExpiresAtUtc:O}.";
}

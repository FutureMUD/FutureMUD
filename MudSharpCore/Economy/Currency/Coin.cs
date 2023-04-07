using MudSharp.Framework;

namespace MudSharp.Economy.Currency;

public class Coin : FrameworkItem, ICoin
{
	public Coin(MudSharp.Models.Coin coin)
	{
		_id = coin.Id;
		_name = coin.Name;
		ShortDescription = coin.ShortDescription;
		FullDescription = coin.FullDescription;
		Weight = coin.Weight;
		Value = coin.Value;
		GeneralForm = coin.GeneralForm;
		PluralWord = coin.PluralWord;
	}

	public override string FrameworkItemType => "Coin";

	#region ICoin Members

	public string GeneralForm { get; }

	public string PluralWord { get; }

	public string ShortDescription { get; }

	public string FullDescription { get; }

	public decimal Value { get; }

	public double Weight { get; }

	#endregion
}
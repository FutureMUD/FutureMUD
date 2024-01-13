using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using System.Text;
using MudSharp.Database;

namespace MudSharp.Economy.Currency;

public class Coin : SaveableItem, ICoin
{
	public Coin(IFuturemud gameworld, MudSharp.Models.Coin coin)
	{
		Gameworld = gameworld;
		_id = coin.Id;
		_name = coin.Name;
		ShortDescription = coin.ShortDescription;
		FullDescription = coin.FullDescription;
		Weight = coin.Weight;
		Value = coin.Value;
		GeneralForm = coin.GeneralForm;
		PluralWord = coin.PluralWord;
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.Coins.Find(Id);
		dbitem.Name = Name;
		dbitem.ShortDescription = ShortDescription;
		dbitem.FullDescription = FullDescription;
		dbitem.Weight = Weight;
		dbitem.Value = Value;
		dbitem.GeneralForm = GeneralForm;
		dbitem.PluralWord = PluralWord;
		Changed = false;
	}

	public override string FrameworkItemType => "Coin";

	#region ICoin Members

	public string GeneralForm { get; }

	public string PluralWord { get; }

	public string ShortDescription { get; }

	public string FullDescription { get; }

	public decimal Value { get; }

	public double Weight { get; }

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		throw new System.NotImplementedException();
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Coin #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, Telnet.FunctionYellow, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"SDesc: {ShortDescription.ColourValue()}");
		sb.AppendLine($"General: {GeneralForm.ColourValue()}");
		sb.AppendLine($"Plural: {PluralWord.ColourValue()}");
		sb.AppendLine($"Value: {Value.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Weight: {Gameworld.UnitManager.Describe(Weight, Framework.Units.UnitType.Mass, actor).ColourValue()}");
		sb.AppendLine();
		sb.AppendLine("Full Description:");
		sb.AppendLine();
		sb.AppendLine(FullDescription.Wrap(actor.InnerLineFormatLength, "\t").SubstituteANSIColour());
		return sb.ToString();
	}

	#endregion
}
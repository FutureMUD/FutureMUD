using System;
using System.Xml.Linq;
using MudSharp.Database;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class NewPlayer : Effect, INewPlayerEffect
{
	public NewPlayer(IPerceivable owner)
		: base(owner)
	{
	}

	public NewPlayer(XElement effect, IPerceivable owner)
		: base(effect, owner)
	{
	}

	public static TimeSpan NewPlayerEffectLength { get; set; }

	protected override string SpecificEffectType => "NewPlayer";

	public override bool SavingEffect => true;

	public override string Describe(IPerceiver voyeur)
	{
		return "New Player";
	}

	public override void ExpireEffect()
	{
		Owner.OutputHandler.Send(
			$"{"[System Message]".Colour(Telnet.Green)} You are no longer considered to be a new player as you have now played for {NewPlayerEffectLength.Describe()}.");
		Owner.RemoveEffect(this);
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("NewPlayer", (effect, owner) => new NewPlayer(effect, owner));
		var dbitem = FMDB.Context.StaticConfigurations.Find("NewPlayerEffectLength");
		NewPlayerEffectLength = dbitem != null && int.TryParse(dbitem.Definition, out var value)
			? TimeSpan.FromMinutes(value)
			: TimeSpan.FromMinutes(60 * 12);
	}

	public override string ToString()
	{
		return "New Player Effect";
	}
}
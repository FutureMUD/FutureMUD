using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class LearningFatigueEffect : Effect, ILearningFatigueEffect
{
	public LearningFatigueEffect(IPerceivable owner, bool freshBlock, int fatigueDegrees) : base(owner)
	{
		BlockUntil = freshBlock
			? DateTime.UtcNow + TimeSpan.FromSeconds(Dice.Roll(LearningBlockLengthDiceExpression))
			: DateTime.MinValue;
		FatigueDegrees = fatigueDegrees;
	}

	public LearningFatigueEffect(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		LoadFromXml(effect.Element("Effect"));
	}

	private static string LearningFatigueLengthDiceExpression
		=> Futuremud.Games.First().GetStaticConfiguration("LearningFatigueLengthDiceExpression");

	private static string LearningBlockLengthDiceExpression
		=> Futuremud.Games.First().GetStaticConfiguration("LearningBlockLengthDiceExpression");

	public override void ExpireEffect()
	{
		base.ExpireEffect();
		if (--FatigueDegrees > 0)
		{
			Owner.AddEffect(new LearningFatigueEffect(Owner, false, FatigueDegrees),
				TimeSpan.FromSeconds(GetFatigueLength(FatigueDegrees)));
		}
	}

	public override bool SavingEffect { get; } = true;

	public DateTime BlockUntil { get; set; }

	public int FatigueDegrees { get; set; }

	public static int GetFatigueLength(int existingFatigueDegrees)
	{
		return Dice.Roll(LearningFatigueLengthDiceExpression);
	}

	public static TimeSpan GetBlockLength()
	{
		return TimeSpan.FromSeconds(Dice.Roll(LearningBlockLengthDiceExpression));
	}

	private void LoadFromXml(XElement root)
	{
		var element = root.Element("BlockUntil");
		var timespan = TimeSpan.FromSeconds(double.Parse(element.Value));
		if (timespan.TotalSeconds <= 0.0)
		{
			BlockUntil = DateTime.MinValue;
		}
		else
		{
			BlockUntil = DateTime.UtcNow + timespan;
		}

		element = root.Element("FatigueDegrees");
		FatigueDegrees = int.Parse(element.Value);
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("LearningFatigueEffect", (effect, owner) => new LearningFatigueEffect(effect, owner));
	}

	protected override XElement SaveDefinition()
	{
		return
			new XElement("Effect",
				new XElement("BlockUntil",
					(BlockUntil > DateTime.UtcNow ? BlockUntil - DateTime.UtcNow : TimeSpan.FromSeconds(0))
					.TotalSeconds), new XElement("FatigueDegrees", FatigueDegrees));
	}

	#region Overrides of Effect

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"{Owner.HowSeen(voyeur, true)} is fatigued of learning, making it {FatigueDegrees:N0} degrees harder{(BlockUntil > DateTime.UtcNow ? $" and completely blocking learning for {(BlockUntil - DateTime.UtcNow).Describe(voyeur).Colour(Telnet.Green)}" : "")}";
	}

	#region Overrides of Object

	/// <summary>
	///     Returns a string that represents the current object.
	/// </summary>
	/// <returns>
	///     A string that represents the current object.
	/// </returns>
	public override string ToString()
	{
		return
			$"Owner is fatigued of learning, making it {FatigueDegrees:N0} degrees harder{(BlockUntil > DateTime.UtcNow ? $" and completely blocking learning for {(BlockUntil - DateTime.UtcNow).Describe().Colour(Telnet.Green)}" : "")}";
	}

	#endregion

	protected override string SpecificEffectType { get; } = "LearningFatigueEffect";

	#endregion
}
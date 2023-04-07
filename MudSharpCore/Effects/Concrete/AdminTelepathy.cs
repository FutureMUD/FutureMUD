using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

public class AdminTelepathy : Effect, ITelepathyEffect
{
	public AdminTelepathy(IPerceivable owner, IFutureProg applicabilityProg = null) : base(owner, applicabilityProg)
	{
	}

	public AdminTelepathy(XElement effect, IPerceivable owner) : base(effect, owner)
	{
	}

	#region Overrides of Effect

	public override string Describe(IPerceiver voyeur)
	{
		return $"{Owner.HowSeen(voyeur, true)} can hear everyone's thoughts.";
	}

	#region Overrides of Effect

	public override bool SavingEffect { get; } = true;

	#region Overrides of Effect

	public static void InitialiseEffectType()
	{
		RegisterFactory("AdminTelepathy", (effect, owner) => new AdminTelepathy(effect, owner));
	}

	#endregion

	#endregion

	#region Overrides of Object

	/// <summary>
	///     Returns a string that represents the current object.
	/// </summary>
	/// <returns>
	///     A string that represents the current object.
	/// </returns>
	public override string ToString()
	{
		return "Owner can hear everyone's thoughts.";
	}

	#endregion

	protected override string SpecificEffectType { get; } = "AdminTelepathy";

	#endregion

	#region Overrides of Effect

	public override bool Applies()
	{
		return true;
	}

	#region Overrides of Effect

	public override bool Applies(object target)
	{
		return true;
	}

	#region Overrides of Effect

	public override bool Applies(object target, object thirdparty)
	{
		return true;
	}

	#endregion

	#endregion

	#endregion

	#region Implementation of ITelepathyEffect

	public bool ShowDescription(ICharacter thinker)
	{
		return true;
	}

	public bool ShowName(ICharacter thinker)
	{
		return true;
	}

	public bool ShowThinkEmote(ICharacter thinker)
	{
		return true;
	}

	public bool ShowThinks { get; } = true;
	public bool ShowFeels { get; } = true;

	#endregion
}
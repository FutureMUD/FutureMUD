using System;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Law;
using MudSharp.TimeAndDate;

namespace MudSharp.Effects.Concrete;

public class AwaitingExecution : Effect
{
	#region Static Initialisation

	public static void InitialiseEffectType()
	{
		RegisterFactory("AwaitingExecution", (effect, owner) => new AwaitingExecution(effect, owner));
	}

	#endregion

	public ILegalAuthority LegalAuthority { get; set; }
	public MudDateTime ExecutionDate { get; set; }

	#region Constructors

	public AwaitingExecution(ICharacter owner, ILegalAuthority legalAuthority,
		MudDateTime executionDate) : base(owner, null)
	{
		LegalAuthority = legalAuthority;
		ExecutionDate = executionDate;
	}

	protected AwaitingExecution(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		var root = effect.Element("Effect");
		LegalAuthority = Gameworld.LegalAuthorities.Get(long.Parse(root.Element("LegalAuthority").Value));
		ExecutionDate = new MudDateTime(root.Element("ExecutionDate").Value, Gameworld);
	}

	#endregion

	#region Saving and Loading

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("LegalAuthority", LegalAuthority.Id),
			new XElement("ExecutionDate", new XCData(ExecutionDate.GetDateTimeString()))
		);
	}

	#endregion

	#region Overrides of Effect

	protected override string SpecificEffectType => "AwaitingExecution";

	public override string Describe(IPerceiver voyeur)
	{
		return $"Awaiting execution at {ExecutionDate.ToString(TimeAndDate.Date.CalendarDisplayMode.Short, TimeAndDate.Time.TimeDisplayTypes.Short).ColourValue()}.";
	}

	public override bool SavingEffect => true;

	public override bool Applies(object target)
	{
		if (target is ILegalAuthority authority)
		{
			return base.Applies(target) && authority == LegalAuthority;
		}

		return base.Applies(target);
	}

	#endregion

	public void ExtendSentence(TimeSpan extension)
	{
		ExecutionDate += extension;
		Changed = true;
	}
}
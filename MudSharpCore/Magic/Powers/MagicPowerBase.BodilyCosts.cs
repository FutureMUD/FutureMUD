using MudSharp.Magic.Casting;

#nullable enable
namespace MudSharp.Magic.Powers;

public abstract partial class MagicPowerBase
{
	public PsionicBodilyCostPolicy? BodilyCost { get; private set; }
	public PsionicStunUpkeepPolicy? StunUpkeep { get; private set; }
	private void LoadBodilyCosts(XElement root)
	{
		if (root.Element("BodilyCost") is { } cost)
		{
			BodilyCost = new((int)cost.Attribute("base")!, (long)cost.Attribute("skillTrait")!, (long?)cost.Attribute("willpowerTrait") ?? 0,
				((string?)cost.Attribute("verbs") ?? "").Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
			if (BodilyCost.Base is < 0 or > 1000 || Gameworld.Traits.Get(BodilyCost.SkillTraitId) is null || BodilyCost.WillpowerTraitId != 0 && Gameworld.Traits.Get(BodilyCost.WillpowerTraitId) is null) throw new FormatException("Invalid bodily invocation price.");
		}
		if (root.Element("StunUpkeep") is { } upkeep)
		{
			StunUpkeep = new((long)upkeep.Attribute("skillTrait")!, (long)upkeep.Attribute("wisdomTrait")!, (long?)upkeep.Attribute("freeProg") ?? 0, (int?)upkeep.Attribute("attemptsPerMinute") ?? 60);
			if (StunUpkeep.AttemptsPerMinute is < 1 or > 600 || Gameworld.Traits.Get(StunUpkeep.SkillTraitId) is null || Gameworld.Traits.Get(StunUpkeep.WisdomTraitId) is null || StunUpkeep.FreeProgId != 0 && Gameworld.FutureProgs.Get(StunUpkeep.FreeProgId) is null) throw new FormatException("Invalid bodily upkeep price.");
		}
	}
	private void SaveBodilyCosts(XElement root)
	{
		if (BodilyCost is { } cost) root.Add(new XElement("BodilyCost", new XAttribute("base", cost.Base), new XAttribute("skillTrait", cost.SkillTraitId), new XAttribute("willpowerTrait", cost.WillpowerTraitId), new XAttribute("verbs", string.Join(',', cost.Verbs))));
		if (StunUpkeep is { } upkeep) root.Add(new XElement("StunUpkeep", new XAttribute("skillTrait", upkeep.SkillTraitId), new XAttribute("wisdomTrait", upkeep.WisdomTraitId), new XAttribute("freeProg", upkeep.FreeProgId), new XAttribute("attemptsPerMinute", upkeep.AttemptsPerMinute)));
	}
	private double Raw(ICharacter actor, long traitId) => Gameworld.Traits.Get(traitId) is { } trait && actor.HasTrait(trait) ? actor.TraitRawValue(trait) : 1;
	private int BodilyPrice(ICharacter actor, int random) => BodilyCost is { } c ? PsionicBodilyCostPolicy.Price(c.Base, Raw(actor, c.SkillTraitId), Raw(actor, c.WillpowerTraitId), random) : 0;
	private bool BodilyCostReady(ICharacter actor) => BodilyCost is null ||
		actor.HealthStrategy is not null && actor.HealthStrategy.MaxStun(actor) - actor.Wounds.Sum(x => x.CurrentStun) > BodilyPrice(actor, 3) &&
		ConfiguredItemConsequence.CanApply(actor, new ItemConsequence(Stun: BodilyPrice(actor, 3)));
	private void ConsumeBodilyCost(ICharacter actor, string verb)
	{
		if (BodilyCost is not { } c || c.Verbs.Length > 0 && !c.Verbs.Any(x => x.EqualTo(verb))) return;
		if (!ConfiguredItemConsequence.Apply(actor, new ItemConsequence(Stun: BodilyPrice(actor, Random.Shared.Next(4)), Message: "You suffer from use of the Way.")))
			throw new InvalidOperationException("This body cannot pay its configured psionic stun cost.");
	}
	protected bool PayStunUpkeep(ICharacter actor, double multiplier)
	{
		if (StunUpkeep is not { } upkeep || PatronMagicService.Allowed(Gameworld, upkeep.FreeProgId, actor)) return true;
		var skill = Math.Clamp(Raw(actor, upkeep.SkillTraitId), 1, 100);
		var wisdom = Math.Clamp(Raw(actor, upkeep.WisdomTraitId), 1, 100);
		var chance = Math.Max(1, (int)(wisdom + skill) / 3);
		var amount = 0;
		for (var i = 0; i < Math.Clamp((int)Math.Ceiling(upkeep.AttemptsPerMinute * multiplier), 0, 600); i++)
			if (Random.Shared.Next(chance + 1) == 0) amount += Random.Shared.Next(1, Math.Max(1, (int)(101 - skill) / 10) + 1);
		return amount == 0 || ConfiguredItemConsequence.Apply(actor, new ItemConsequence(Stun: amount, Message: "You suffer from use of the Way.")) && actor.State.IsAble();
	}
	private bool BuildingCommandBodilyCost(ICharacter actor, StringStack command, bool upkeep)
	{
		if (command.PeekSpeech().EqualTo("none")) { if (upkeep) StunUpkeep = null; else BodilyCost = null; Changed = true; actor.Send("The bodily cost policy has been removed."); return true; }
		try
		{
			long Trait() => Gameworld.Traits.GetByIdOrName(command.PopSpeech())?.Id ?? throw new FormatException("Choose a trait.");
			if (upkeep)
			{
				var skill = Trait(); var wisdom = Trait();
				var progText = command.PopSpeech();
				var prog = progText.EqualTo("none") ? 0 : Gameworld.FutureProgs.GetByIdOrName(progText)?.Id ?? throw new FormatException("Choose a boolean(character) exemption prog.");
				if (prog != 0 && (Gameworld.FutureProgs.Get(prog) is not { } exemption ||
					!exemption.ReturnType.CompatibleWith(ProgVariableTypes.Boolean) || !exemption.MatchesParameters([ProgVariableTypes.Character])))
					throw new FormatException("Choose a boolean(character) exemption prog.");
				var attempts = int.Parse(command.PopSpeech());
				if (attempts is < 1 or > 600) throw new FormatException("Choose 1-600 upkeep attempts per minute.");
				StunUpkeep = new(skill, wisdom, prog, attempts);
			}
			else
			{
				var basis = int.Parse(command.PopSpeech());
				if (basis < 0 || basis > 1000) throw new FormatException("Choose a base price of 0-1000.");
				var skill = Trait(); var will = Trait();
				BodilyCost = new(basis, skill, will, command.SafeRemainingArgument.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
			}
			Changed = true;
			actor.Send("The bodily cost policy has been updated.");
			return true;
		}
		catch (Exception ex) when (ex is FormatException or OverflowException)
		{
			actor.Send($"{ex.Message} Use bodilycost <base> <skill> <willpower> [comma-separated verbs], stunupkeep <skill> <wisdom> <exemption prog|none> <attempts per minute>, or either command with none."); return false;
		}
	}
}

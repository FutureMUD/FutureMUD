using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.Magic.SpellTriggers
{
	internal class CastingTriggerCorpse : CastingTriggerBase
	{
		public static void RegisterFactory()
		{
			SpellTriggerFactory.RegisterBuilderFactory("corpse", DoBuilderLoad,
				"Targets a corpse item in the same room",
				"item",
				new CastingTriggerCorpse().BuildingCommandHelp
			);
			SpellTriggerFactory.RegisterLoadTimeFactory("corpse",
				(root, spell) => new CastingTriggerCorpse(root, spell));
		}

		private static (IMagicTrigger Trigger, string Error) DoBuilderLoad(StringStack command, IMagicSpell spell)
		{
			return (
				new CastingTriggerCorpse(
					new XElement("Trigger", new XElement("MinimumPower", (int)SpellPower.Insignificant),
						new XElement("MaximumPower", (int)SpellPower.RecklesslyPowerful), new XElement("TargetFilterProg", 0L)), spell), string.Empty);
		}

		public IFutureProg TargetFilterProg { get; private set; }

		protected CastingTriggerCorpse(XElement root, IMagicSpell spell) : base(root, spell)
		{
			TargetFilterProg = spell.Gameworld.FutureProgs.Get(long.Parse(root.Element("TargetFilterProg").Value));
		}

		protected CastingTriggerCorpse() : base() { }

		#region Overrides of CastingTriggerBase

		public override XElement SaveToXml()
		{
			return new XElement("Trigger",
				new XAttribute("type", "corpse"),
				new XElement("MinimumPower", (int)MinimumPower),
				new XElement("MaximumPower", (int)MaximumPower),
				new XElement("TargetFilterProg", TargetFilterProg?.Id ?? 0L)
			);
		}

		public override IMagicTrigger Clone()
		{
			return new CastingTriggerCorpse(SaveToXml(), Spell);
		}

		public override string SubtypeBuildingCommandHelp =>
			@"
	#3filterprog <prog>#0 - sets the optional prog to filter targets by
	#3filterprog clear#0 - clears the filter prog";

		public override bool BuildingCommand(ICharacter actor, StringStack command)
		{
			switch (command.PopForSwitch())
			{
				case "filter":
				case "filterprog":
				case "prog":
					return BuildingCommandFilterProg(actor, command);
				default:
					return base.BuildingCommand(actor, command.GetUndo());
			}
		}

		private bool BuildingCommandFilterProg(ICharacter actor, StringStack command)
		{
			if (command.IsFinished)
			{
				actor.OutputHandler.Send(
					$"You must specify a prog to use as a filter prog, or use {"none".ColourCommand()} to clear and existing one.");
				return false;
			}

			var text = command.SafeRemainingArgument;
			if (text.EqualToAny("none", "clear", "delete"))
			{
				TargetFilterProg = null;
				Spell.Changed = true;
				actor.OutputHandler.Send(
					$"This spell will no longer use a prog to filter which characters can be targeted by it.");
				return true;
			}

			var prog = actor.Gameworld.FutureProgs.GetByIdOrName(text);
			if (prog == null)
			{
				actor.OutputHandler.Send($"There is no such prog.");
				return false;
			}

			if (!prog.ReturnType.CompatibleWith(ProgVariableTypes.Boolean))
			{
				actor.OutputHandler.Send(
					$"You must specify a prog that returns a boolean value, whereas {prog.MXPClickableFunctionName()} returns {prog.ReturnType.Describe().ColourName()}.");
				return false;
			}

			if (!prog.MatchesParameters(new List<ProgVariableTypes>
				{ ProgVariableTypes.Character, ProgVariableTypes.Character }) &&
				!prog.MatchesParameters(new List<ProgVariableTypes> { ProgVariableTypes.Character }))
			{
				actor.OutputHandler.Send(
					$"You must specify a prog that accepts either a single character (the target), or two characters (the target and the caster), whereas {prog.MXPClickableFunctionName()} does not.");
				return false;
			}

			TargetFilterProg = prog;
			Spell.Changed = true;
			actor.OutputHandler.Send(
				$"This spell will now use the {prog.MXPClickableFunctionNameWithId()} prog to filter valid character targets.");
			return true;
		}

		public override void DoTriggerCast(ICharacter actor, StringStack additionalArguments)
		{
			if (!CheckBaseTriggerCase(actor, additionalArguments, out var power))
			{
				return;
			}

			if (additionalArguments.IsFinished)
			{
				actor.OutputHandler.Send("That spell requires a target corpse. You must specify a corpse to target.");
				return;
			}

			var target = actor.TargetCorpse(additionalArguments.SafeRemainingArgument);
			if (target == null)
			{
				actor.OutputHandler.Send("You don't see any corpse like that to target.");
				return;
			}

			if (TargetFilterProg?.Execute<bool?>(target.OriginalCharacter, actor) == false)
			{
				actor.OutputHandler.Send(
					$"{target.Parent.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreSelf)} is not a valid target for that spell.");
				return;
			}

			Spell.CastSpell(actor, target.Parent, power);
		}

		public override bool TriggerYieldsTarget => true;

		public override string TargetTypes => "item";

		public override string Show(ICharacter actor)
		{
			return
				$"{"Cast@Corpse".ColourName()} - {base.Show(actor)}{(TargetFilterProg != null ? $" Filter: {TargetFilterProg.MXPClickableFunctionName()}" : "")}";
		}

		public override string ShowPlayer(ICharacter actor)
		{
			return
				$"Cast Command - {Spell.School.SchoolVerb} cast {(Spell.Name.Contains(' ') ? Spell.Name.ToLowerInvariant().DoubleQuotes() : Spell.Name.ToLowerInvariant())} <power> <target corpse>";
		}

		#endregion
	}
}

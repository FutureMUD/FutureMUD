using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Character;
using MudSharp.Combat.AuxillaryEffects;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.Combat
{
	internal class AuxillaryCombatAction : CombatAction, IAuxillaryCombatAction
	{
		protected AuxillaryCombatAction(AuxillaryCombatAction rhs)
		{
			Gameworld = rhs.Gameworld;
			using (new FMDB())
			{
				var dbitem = new Models.CombatAction();
				dbitem.BaseDelay = rhs.BaseDelay;
				dbitem.ExertionLevel = (int)rhs.ExertionLevel;
				dbitem.UsabilityProgId = rhs.UsabilityProg?.Id;
				dbitem.Intentions = (long)rhs.Intentions;
				dbitem.MoveType = (int)rhs.MoveType;
				dbitem.Name = rhs.Name;
				dbitem.RecoveryDifficultyFailure = (int)rhs.RecoveryDifficultyFailure;
				dbitem.RecoveryDifficultySuccess = (int)rhs.RecoveryDifficultySuccess;
				dbitem.StaminaCost = rhs.StaminaCost;
				dbitem.Weighting = rhs.Weighting;
				dbitem.RequiredPositionStateIds =
					rhs._requiredPositionStates.Select(x => x.Id.ToString("F0")).ListToCommaSeparatedValues(" ");
				SaveMoveSpecificData(dbitem);
				FMDB.Context.CombatActions.Add(dbitem);
				FMDB.Context.SaveChanges();
				LoadFromDatabase(dbitem);
			}
		}

		public AuxillaryCombatAction(Models.CombatAction dbitem, IFuturemud gameworld)
		{
			Gameworld = gameworld;
			LoadFromDatabase(dbitem);
		}

		private void LoadFromDatabase(Models.CombatAction dbitem)
		{
			_id = dbitem.Id;
			_name = dbitem.Name;
			UsabilityProg = Gameworld.FutureProgs.Get(dbitem.UsabilityProgId ?? 0);
			MoveType = (BuiltInCombatMoveType)dbitem.MoveType;
			Intentions = (CombatMoveIntentions)dbitem.Intentions;
			RecoveryDifficultySuccess = (Difficulty)dbitem.RecoveryDifficultySuccess;
			RecoveryDifficultyFailure = (Difficulty)dbitem.RecoveryDifficultyFailure;
			StaminaCost = dbitem.StaminaCost;
			BaseDelay = dbitem.BaseDelay;
			Weighting = dbitem.Weighting;
			ExertionLevel = (ExertionLevel)dbitem.ExertionLevel;
			_requiredPositionStates.AddRange(dbitem.RequiredPositionStateIds.Split(' ').Select(x => long.Parse(x))
												   .Select(x => PositionState.GetState(x)));
		}

		public override void Save()
		{
			var dbitem = FMDB.Context.CombatActions.Find(Id);
			dbitem.BaseDelay = BaseDelay;
			dbitem.ExertionLevel = (int)ExertionLevel;
			dbitem.UsabilityProgId = UsabilityProg?.Id;
			dbitem.Intentions = (long)Intentions;
			dbitem.MoveType = (int)MoveType;
			dbitem.Name = Name;
			dbitem.RecoveryDifficultyFailure = (int)RecoveryDifficultyFailure;
			dbitem.RecoveryDifficultySuccess = (int)RecoveryDifficultySuccess;
			dbitem.StaminaCost = StaminaCost;
			dbitem.Weighting = Weighting;
			dbitem.RequiredPositionStateIds =
				_requiredPositionStates.Select(x => x.Id.ToString("F0")).ListToCommaSeparatedValues(" ");
			SaveMoveSpecificData(dbitem);
			Changed = false;
		}

		protected static IAuxillaryEffect LoadEffect(XElement definition, IFuturemud gameworld, AuxillaryCombatAction action)
		{
			switch (definition.Attribute("type").Value)
			{
				case "attackeradvantage":
					return new AttackerAdvantage(definition, gameworld);
				case "defenderadvantage":
					return new DefenderAdvantage(definition, gameworld);
				default:
					throw new NotImplementedException();
			}
		}

		protected virtual void SaveMoveSpecificData(Models.CombatAction dbitem)
		{
			var root = new XElement("Root");
			foreach (var effect in _auxillaryEffects)
			{
				root.Add(effect.Save());
			}
			dbitem.AdditionalInfo = root.ToString();
		}

		public override string FrameworkItemType => "AuxillaryCombatAction";
		
		public string ShowBuilder(ICharacter actor)
		{
			var sb = new StringBuilder();
			sb.AppendLine($"Combat Action {Id.ToString("N0", actor)} - {Name}");
			sb.AppendLine($"Move Type: {MoveType.Describe().Colour(Telnet.Green)}");
			sb.AppendLine($"Position States: {RequiredPositionStates.Select(x => x.DescribeLocationMovementParticiple.TitleCase().ColourValue()).ListToCommaSeparatedValues(", ")}");
			sb.AppendLineColumns((uint)actor.LineFormatLength, 3,
				$"Base Delay: {BaseDelay.ToString("N2", actor)}s".ColourValue(),
				$"Base Stamina: {StaminaCost.ToString("N3", actor).Colour(Telnet.Green)}",
				$"Weighting: {Weighting.ToString("N2", actor).Colour(Telnet.Green)}"
			);
			sb.AppendLineColumns((uint)actor.LineFormatLength, 3,
				$"Exertion: {ExertionLevel.Describe().Colour(Telnet.Green)}",
				$"Recover Failure: {RecoveryDifficultyFailure.Describe().Colour(Telnet.Green)}",
				$"Recover Success: {RecoveryDifficultySuccess.Describe().Colour(Telnet.Green)}"
			);
			sb.AppendLineColumns((uint)actor.LineFormatLength, 3,
				$"Usability Prog: {(UsabilityProg != null ? $"{UsabilityProg.FunctionName}".FluentTagMXP("send", $"href='show futureprog {UsabilityProg.Id}'") : "None".Colour(Telnet.Red))}",
				"",
				""
			);
			sb.AppendLine($"Intentions: {Intentions.Describe()}");
			sb.AppendLine();
			sb.AppendLine("Effects:");
			sb.AppendLine();
			foreach (var effect in _auxillaryEffects)
			{
				sb.AppendLine(effect.DescribeForShow(actor));
			}
			sb.AppendLine();
			sb.AppendLine("Combat Message Hierarchy:");
			var messages = Gameworld.CombatMessageManager.CombatMessages.Where(x => x.CouldApply(this))
									.OrderByDescending(x => x.Priority).ThenByDescending(x => x.Outcome ?? Outcome.None)
									.ThenBy(x => x.Prog != null).ToList();
			var i = 1;
			foreach (var message in messages)
			{
				sb.AppendLine(
					$"{i++.ToOrdinal()}) [#{message.Id.ToString("N0", actor)}] {message.Message.ColourCommand()} [{message.Chance.ToString("P3", actor).Colour(Telnet.Green)}]{(message.Outcome.HasValue ? $" [{message.Outcome.Value.DescribeColour()}]" : "")}{(message.Prog != null ? $" [{message.Prog.FunctionName} (#{message.Prog.Id})]".FluentTagMXP("send", $"href='show futureprog {message.Prog.Id}'") : "")}");
			}

			return sb.ToString();
		}

		public override bool BuildingCommand(ICharacter actor, StringStack command)
		{
			switch (command.PopSpeech().ToLowerInvariant())
			{
				default:
					return base.BuildingCommand(actor, command.GetUndo());
			}
		}

		public IAuxillaryCombatAction Clone()
		{
			return new AuxillaryCombatAction(this);
		}
		public string DescribeForCombatMessageShow(ICharacter actor)
		{
			return $"";
		}

		public override string ActionTypeName => "auxillary move";
		public override string HelpText { get; }

		private readonly List<IAuxillaryEffect> _auxillaryEffects = new();
		public IEnumerable<IAuxillaryEffect> AuxillaryEffects => _auxillaryEffects;
	}
}

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body.Traits;

namespace MudSharp.Magic.Powers;

public class SensePower : MagicPowerBase
{
	public override string PowerType => "Sense";
	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("sense", (power, gameworld) => new SensePower(power, gameworld));
	}

	public SensePower(Models.MagicPower power, IFuturemud gameworld) : base(power, gameworld)
	{
		var definition = XElement.Parse(power.Definition);
		Verb = definition.Element("Verb")?.Value ??
		       throw new ApplicationException($"Missing Verb element in SensePower definition {Id}");
		EmoteText = definition.Element("EmoteText")?.Value ??
		            throw new ApplicationException($"Missing EmoteText element in SensePower definition {Id}");
		EchoHeader = definition.Element("EchoHeader")?.Value ??
		             throw new ApplicationException($"Missing EchoHeader element in SensePower definition {Id}");
		NoTargetsFoundEcho = definition.Element("NoTargetsFoundEcho")?.Value ??
		                     throw new ApplicationException(
			                     $"Missing NoTargetsFoundEcho element in SensePower definition {Id}");
		if (!Utilities.TryParseEnum<MagicPowerDistance>(
			    definition.Element("PowerDistance")?.Value ??
			    throw new ApplicationException($"Missing PowerDistance element in SensePower definition {Id}"),
			    out var powerDistance))
		{
			throw new ApplicationException($"Invalid MagicPowerDistance in SensePower definition {Id}");
		}

		PowerDistance = powerDistance;

		if (!Utilities.TryParseEnum<FutureProgVariableTypes>(
			    definition.Element("SenseType")?.Value ??
			    throw new ApplicationException($"Missing SenseType element in SensePower definition {Id}"),
			    out var senseType))
		{
			throw new ApplicationException($"Invalid SenseType in SensePower definition {Id}");
		}

		SenseType = senseType;

		EmoteVisible = bool.Parse(definition.Element("EmoteVisible")?.Value ??
		                          throw new ApplicationException(
			                          $"Missing EmoteVisible element in SensePower definition {Id}"));

		CommandDelay = TimeSpan.FromSeconds(double.Parse(definition.Element("CommandDelay")?.Value ??
		                                                 throw new ApplicationException(
			                                                 $"Missing CommandDelay element in SensePower definition {Id}")));
		CheckTrait =
			long.TryParse(
				definition.Element("CheckTrait")?.Value ??
				throw new ApplicationException($"Missing CheckTrait element in SensePower definition {Id}"),
				out var value)
				? Gameworld.Traits.Get(value)
				: Gameworld.Traits.GetByName(definition.Element("CheckTrait").Value);
		if (CheckTrait == null)
		{
			throw new ApplicationException($"CheckTrait not found in SeensePower definition {Id}");
		}

		var targetFilterProg =
			long.TryParse(
				definition.Element("SenseTargetFilterProg")?.Value ??
				throw new ApplicationException($"Missing SenseTargetFilterProg element in SensePower definition {Id}"),
				out value)
				? Gameworld.FutureProgs.Get(value)
				: Gameworld.FutureProgs.GetByName(definition.Element("SenseTargetFilterProg").Value);
		if (targetFilterProg == null)
		{
			throw new ApplicationException($"SenseTargetFilterProg not found in SensePower definition {Id}");
		}

		if (!targetFilterProg.ReturnType.CompatibleWith(FutureProgVariableTypes.Boolean))
		{
			throw new ApplicationException(
				$"SenseTargetFilterProg ({targetFilterProg.Name} #{targetFilterProg.Id}) did not return boolean in SensePower definition {Id}");
		}

		SenseTargetFilterProg = targetFilterProg;

		var targetDifficultyProg =
			long.TryParse(
				definition.Element("TargetDifficultyProg")?.Value ??
				throw new ApplicationException($"Missing TargetDifficultyProg element in SensePower definition {Id}"),
				out value)
				? Gameworld.FutureProgs.Get(value)
				: Gameworld.FutureProgs.GetByName(definition.Element("TargetDifficultyProg").Value);
		if (targetDifficultyProg == null)
		{
			throw new ApplicationException($"TargetDifficultyProg not found in SensePower definition {Id}");
		}

		if (!targetDifficultyProg.ReturnType.CompatibleWith(FutureProgVariableTypes.Number))
		{
			throw new ApplicationException(
				$"TargetDifficultyProg ({targetFilterProg.Name} #{targetFilterProg.Id}) did not return number in SensePower definition {Id}");
		}

		TargetDifficultyProg = targetDifficultyProg;

		switch (senseType)
		{
			case FutureProgVariableTypes.Perceivable:
				if (!targetFilterProg.MatchesParameters(new[] { FutureProgVariableTypes.Perceivable }))
				{
					throw new ApplicationException(
						$"SenseTargetFilterProg ({targetFilterProg.FunctionName} #{targetFilterProg.Id}) did not accept a Perceivable parameter as specified by the SenseType in SensePower {Id}.");
				}

				if (!targetDifficultyProg.MatchesParameters(new[] { FutureProgVariableTypes.Perceivable }))
				{
					throw new ApplicationException(
						$"TargetDifficultyProg ({targetFilterProg.FunctionName} #{targetFilterProg.Id}) did not accept a Perceivable parameter as specified by the SenseType in SensePower {Id}.");
				}

				break;
			case FutureProgVariableTypes.Perceiver:
				if (!targetFilterProg.MatchesParameters(new[] { FutureProgVariableTypes.Perceiver }))
				{
					throw new ApplicationException(
						$"SenseTargetFilterProg ({targetFilterProg.FunctionName} #{targetFilterProg.Id}) did not accept a Perceiver parameter as specified by the SenseType in SensePower {Id}.");
				}

				if (!targetDifficultyProg.MatchesParameters(new[] { FutureProgVariableTypes.Perceiver }))
				{
					throw new ApplicationException(
						$"TargetDifficultyProg ({targetFilterProg.FunctionName} #{targetFilterProg.Id}) did not accept a Perceiver parameter as specified by the SenseType in SensePower {Id}.");
				}

				break;
			case FutureProgVariableTypes.Character:
				if (!targetFilterProg.MatchesParameters(new[] { FutureProgVariableTypes.Character }))
				{
					throw new ApplicationException(
						$"SenseTargetFilterProg ({targetFilterProg.FunctionName} #{targetFilterProg.Id}) did not accept a Character parameter as specified by the SenseType in SensePower {Id}.");
				}

				if (!targetDifficultyProg.MatchesParameters(new[] { FutureProgVariableTypes.Character }))
				{
					throw new ApplicationException(
						$"TargetDifficultyProg ({targetFilterProg.FunctionName} #{targetFilterProg.Id}) did not accept a Character parameter as specified by the SenseType in SensePower {Id}.");
				}

				break;
			case FutureProgVariableTypes.Item:
				if (!targetFilterProg.MatchesParameters(new[] { FutureProgVariableTypes.Item }))
				{
					throw new ApplicationException(
						$"SenseTargetFilterProg ({targetFilterProg.FunctionName} #{targetFilterProg.Id}) did not accept an Item parameter as specified by the SenseType in SensePower {Id}.");
				}

				if (!targetDifficultyProg.MatchesParameters(new[] { FutureProgVariableTypes.Item }))
				{
					throw new ApplicationException(
						$"TargetDifficultyProg ({targetFilterProg.FunctionName} #{targetFilterProg.Id}) did not accept an Item parameter as specified by the SenseType in SensePower {Id}.");
				}

				break;
			default:
				throw new ApplicationException($"SenseType in SensePower definition {Id} was of an invalid type.");
		}
	}

	public string Verb { get; protected set; }
	public MagicPowerDistance PowerDistance { get; protected set; }
	public TimeSpan CommandDelay { get; protected set; }

	public string EmoteText { get; protected set; }

	public bool EmoteVisible { get; protected set; }

	public string EchoHeader { get; protected set; }

	public string NoTargetsFoundEcho { get; protected set; }

	public ITraitDefinition CheckTrait { get; protected set; }

	public IFutureProg TargetDifficultyProg { get; protected set; }

	public FutureProgVariableTypes SenseType { get; protected set; }

	public IFutureProg SenseTargetFilterProg { get; protected set; }

	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		var (truth, missing) = CanAffordToInvokePower(actor, verb);
		if (!truth)
		{
			actor.OutputHandler.Send(
				$"You can't do that because you lack sufficient {missing.Name.Colour(Telnet.BoldMagenta)}.");
			return;
		}

		if (CanInvokePowerProg?.Execute<bool?>(actor) == false)
		{
			actor.OutputHandler.Send(WhyCantInvokePowerProg.Execute(actor)?.ToString() ??
			                         "You can't use that power at this time.");
			return;
		}

		if (!HandleGeneralUseRestrictions(actor))
		{
			return;
		}

		ConsumePowerCosts(actor, verb);
		actor.AddEffect(new CommandDelayMagicPower(actor, this), CommandDelay);
		if (EmoteVisible)
		{
			actor.OutputHandler.Handle(new EmoteOutput(new Emote(EmoteText, actor, actor)));
		}
		else
		{
			actor.OutputHandler.Send(new EmoteOutput(new Emote(EmoteText, actor, actor)));
		}

		var targets = new List<(IPerceivable Target, ICell Location, RoomLayer Layer)>();

		switch (SenseType)
		{
			case FutureProgVariableTypes.Perceivable:
			case FutureProgVariableTypes.Perceiver:
				switch (PowerDistance)
				{
					case MagicPowerDistance.SameLocationOnly:
						targets.AddRange(
							actor.Location.Characters.Select(x => ((IPerceivable)x, x.Location, x.RoomLayer)));
						targets.AddRange(
							actor.Location.GameItems.Select(x => ((IPerceivable)x, x.Location, x.RoomLayer)));
						break;
					case MagicPowerDistance.AdjacentLocationsOnly:
						targets.AddRange(
							actor.Location.Characters.Select(x => ((IPerceivable)x, x.Location, x.RoomLayer)));
						targets.AddRange(
							actor.Location.GameItems.Select(x => ((IPerceivable)x, x.Location, x.RoomLayer)));

						targets.AddRange(
							actor.Location.Characters.Select(x => ((IPerceivable)x, x.Location, x.RoomLayer)));
						targets.AddRange(
							actor.Location.GameItems.Select(x => ((IPerceivable)x, x.Location, x.RoomLayer)));

						targets.AddRange(actor.Location.ExitsFor(actor, true).Select(x => x.Destination).Distinct()
						                      .SelectMany(x =>
							                      x.Characters.Select(y => ((IPerceivable)y, x, y.RoomLayer))));
						targets.AddRange(actor.Location.ExitsFor(actor, true).Select(x => x.Destination).Distinct()
						                      .SelectMany(x =>
							                      x.GameItems.Select(y => ((IPerceivable)y, x, y.RoomLayer))));
						break;
					case MagicPowerDistance.SameAreaOnly:
						if (actor.Location.Areas.Any())
						{
							foreach (var area in actor.Location.Areas)
							{
								targets.AddRange(
									area.Characters.Select(x => ((IPerceivable)x, x.Location, x.RoomLayer)));
								targets.AddRange(area.GameItems.Select(x =>
									((IPerceivable)x, x.Location, x.RoomLayer)));
							}
						}
						else
						{
							goto case MagicPowerDistance.AdjacentLocationsOnly;
						}

						break;
					case MagicPowerDistance.SameZoneOnly:
						targets.AddRange(
							actor.Location.Zone.Characters.Select(x => ((IPerceivable)x, x.Location, x.RoomLayer)));
						targets.AddRange(
							actor.Location.Zone.GameItems.Select(x => ((IPerceivable)x, x.Location, x.RoomLayer)));
						break;
					case MagicPowerDistance.SameShardOnly:
						targets.AddRange(
							actor.Location.Shard.Characters.Select(x => ((IPerceivable)x, x.Location, x.RoomLayer)));
						targets.AddRange(
							actor.Location.Shard.GameItems.Select(x => ((IPerceivable)x, x.Location, x.RoomLayer)));
						break;
					case MagicPowerDistance.SamePlaneOnly:
						goto case MagicPowerDistance.SameShardOnly;
				}

				break;
			case FutureProgVariableTypes.Character:
				switch (PowerDistance)
				{
					case MagicPowerDistance.SameLocationOnly:
						targets.AddRange(
							actor.Location.Characters.Select(x => ((IPerceivable)x, x.Location, x.RoomLayer)));
						break;
					case MagicPowerDistance.AdjacentLocationsOnly:
						targets.AddRange(
							actor.Location.Characters.Select(x => ((IPerceivable)x, x.Location, x.RoomLayer)));
						targets.AddRange(
							actor.Location.Characters.Select(x => ((IPerceivable)x, x.Location, x.RoomLayer)));
						targets.AddRange(actor.Location.ExitsFor(actor, true).Select(x => x.Destination).Distinct()
						                      .SelectMany(x =>
							                      x.Characters.Select(y => ((IPerceivable)y, x, y.RoomLayer))));
						break;
					case MagicPowerDistance.SameAreaOnly:
						if (actor.Location.Areas.Any())
						{
							foreach (var area in actor.Location.Areas)
							{
								targets.AddRange(
									area.Characters.Select(x => ((IPerceivable)x, x.Location, x.RoomLayer)));
							}
						}
						else
						{
							goto case MagicPowerDistance.AdjacentLocationsOnly;
						}

						break;
					case MagicPowerDistance.SameZoneOnly:
						targets.AddRange(
							actor.Location.Zone.Characters.Select(x => ((IPerceivable)x, x.Location, x.RoomLayer)));
						break;
					case MagicPowerDistance.SameShardOnly:
						targets.AddRange(
							actor.Location.Shard.Characters.Select(x => ((IPerceivable)x, x.Location, x.RoomLayer)));
						break;
					case MagicPowerDistance.SamePlaneOnly:
						goto case MagicPowerDistance.SameShardOnly;
				}

				break;
			case FutureProgVariableTypes.Item:
				switch (PowerDistance)
				{
					case MagicPowerDistance.SameLocationOnly:
						targets.AddRange(
							actor.Location.GameItems.Select(x => ((IPerceivable)x, x.Location, x.RoomLayer)));
						break;
					case MagicPowerDistance.AdjacentLocationsOnly:
						targets.AddRange(
							actor.Location.GameItems.Select(x => ((IPerceivable)x, x.Location, x.RoomLayer)));
						targets.AddRange(
							actor.Location.GameItems.Select(x => ((IPerceivable)x, x.Location, x.RoomLayer)));
						targets.AddRange(actor.Location.ExitsFor(actor, true).Select(x => x.Destination).Distinct()
						                      .SelectMany(x =>
							                      x.GameItems.Select(y => ((IPerceivable)y, x, y.RoomLayer))));
						break;
					case MagicPowerDistance.SameAreaOnly:
						if (actor.Location.Areas.Any())
						{
							foreach (var area in actor.Location.Areas)
							{
								targets.AddRange(area.GameItems.Select(x =>
									((IPerceivable)x, x.Location, x.RoomLayer)));
							}
						}
						else
						{
							goto case MagicPowerDistance.AdjacentLocationsOnly;
						}

						break;
					case MagicPowerDistance.SameZoneOnly:
						targets.AddRange(
							actor.Location.Zone.GameItems.Select(x => ((IPerceivable)x, x.Location, x.RoomLayer)));
						break;
					case MagicPowerDistance.SameShardOnly:
						targets.AddRange(
							actor.Location.Shard.GameItems.Select(x => ((IPerceivable)x, x.Location, x.RoomLayer)));
						break;
					case MagicPowerDistance.SamePlaneOnly:
						goto case MagicPowerDistance.SameShardOnly;
				}

				break;
		}

		var check = Gameworld.GetCheck(CheckType.MagicSensePower);
		var results = check.CheckAgainstAllDifficulties(actor, Difficulty.Normal, CheckTrait);

		var final = targets
		            .Distinct()
		            .Where(x => SenseTargetFilterProg.Execute<bool?>(x.Target) == true)
		            .Where(x => results[(Difficulty)TargetDifficultyProg.ExecuteInt(x.Target)].IsPass())
		            .ToList();

		if (!final.Any())
		{
			actor.OutputHandler.Send(NoTargetsFoundEcho);
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine(EchoHeader);
		foreach (var grouping in final.GroupBy(x => x.Location))
		{
			sb.AppendLine();
			if (grouping.Key == actor.Location)
			{
				sb.AppendLine("Here:".ColourRoom());
			}
			else
			{
				var directions = actor.Location.PathBetween(grouping.Key, 50, exit => true).ToList();
				if (!directions.Any())
				{
					sb.AppendLine(grouping.Key.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee));
				}
				else
				{
					sb.AppendLine(
						$"{grouping.Key.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee)} ({directions.DescribeDirectionsToFrom()})");
				}
			}

			foreach (var item in grouping.OrderByDescending(x => x.Layer))
			{
				sb.AppendLine(
					$"\t{item.Target.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee)} [{item.Layer.PositionalDescription()}]");
			}
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	public override IEnumerable<string> Verbs => new[] { Verb };
}
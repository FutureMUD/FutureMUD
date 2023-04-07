using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Form.Audio;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using ExpressionEngine;

namespace MudSharp.GameItems.Prototypes;

public class BombGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "Bomb";

	private readonly List<(DamageType DamageType, Expression DamageExpression, Expression StunExpression)> _damages =
		new();

	public IEnumerable<(DamageType DamageType, Expression DamageExpression, Expression StunExpression)> Damages =>
		_damages;

	public Proximity MaximumProximity { get; protected set; }

	public string ExplosionEmoteText { get; protected set; }

	public AudioVolume ExplosionVolume { get; protected set; }

	public SizeCategory ExplosionSize { get; protected set; }

	public bool FeltInZone { get; protected set; }

	#region Constructors

	protected BombGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator,
		"Bomb")
	{
		MaximumProximity = Proximity.Proximate;
		ExplosionEmoteText = $"@ explode|explodes with a brilliant flash of light and heat!";
		FeltInZone = true;
		ExplosionVolume = AudioVolume.DangerouslyLoud;
		ExplosionSize = SizeCategory.Large;
		_damages.AddRange(new[]
		{
			(DamageType.Burning, new Expression(Gameworld.GetStaticConfiguration("DefaultBombDamageExpressionBurning")),
				new Expression(Gameworld.GetStaticConfiguration("DefaultBombStunExpressionBurning"))),
			(DamageType.Shockwave,
				new Expression(Gameworld.GetStaticConfiguration("DefaultBombDamageExpressionShockwave")),
				new Expression(Gameworld.GetStaticConfiguration("DefaultBombStunExpressionShockwave"))),
			(DamageType.Shrapnel,
				new Expression(Gameworld.GetStaticConfiguration("DefaultBombDamageExpressionShrapnel")),
				new Expression(Gameworld.GetStaticConfiguration("DefaultBombStunExpressionShrapnel")))
		});
	}

	protected BombGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) : base(
		proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		MaximumProximity = (Proximity)int.Parse(root.Element("MaximumProximity").Value);
		ExplosionEmoteText = root.Element("ExplosionEmoteText").Value;
		FeltInZone = bool.Parse(root.Element("FeltInZone").Value);
		ExplosionVolume = (AudioVolume)int.Parse(root.Element("ExplosionVolume").Value);
		ExplosionSize = (SizeCategory)int.Parse(root.Element("ExplosionSize").Value);
		foreach (var item in root.Element("Damages").Elements())
		{
			_damages.Add(((DamageType)int.Parse(item.Element("DamageType").Value),
				new Expression(item.Element("Damage").Value), new Expression(item.Element("Stun").Value)));
		}
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("MaximumProximity", (int)MaximumProximity),
			new XElement("ExplosionVolume", (int)ExplosionVolume),
			new XElement("ExplosionSize", (int)ExplosionSize),
			new XElement("FeltInZone", FeltInZone),
			new XElement("ExplosionEmoteText", new XCData(ExplosionEmoteText)),
			new XElement("Damages",
				from damage in Damages
				select new XElement("Damage",
					new XElement("DamageType", (int)damage.DamageType),
					new XElement("Damage", new XCData(damage.DamageExpression.OriginalExpression)),
					new XElement("Stun", new XCData(damage.StunExpression.OriginalExpression)))
			)
		).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new BombGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new BombGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("Bomb".ToLowerInvariant(), true,
			(gameworld, account) => new BombGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Bomb", (proto, gameworld) => new BombGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"Bomb",
			$"Makes an item an {"[explosive]".Colour(Telnet.BoldRed)} when triggered. Must be combined with a {"[trigger]".Colour(Telnet.Yellow)}.",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new BombGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\temote <emote> - sets the on-explode emote. Use $0 for the bomb item.\n\tsize <size> - sets the size of the explosion.\n\tvolume <volume> - sets the loudness of the explosion.\n\tproximity <proximity> - sets the maximum proximity affected by the explosion.\n\tzone - toggles whether it echoes to the entire zone that there is an explosion\n\tdamage <type> 0 - removes an existing damage type\n\tdamage <type> \"damage formula\" \"stun formula\" - specifies a new damage formula. Use rand(min,max) for random amounts";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "zone":
				return BuildingCommandZone(actor, command);
			case "size":
				return BuildingCommandSize(actor, command);
			case "volume":
			case "noise":
				return BuildingCommandVolume(actor, command);
			case "proximity":
				return BuildingCommandProximity(actor, command);
			case "explosion":
			case "emote":
				return BuildingCommandExplosion(actor, command);
			case "damage":
				return BuildingCommandDamage(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandDamage(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"You must specify a damage type, and then either a formula or 0 to remove an existing damage.");
			return false;
		}

		if (!WoundExtensions.TryGetDamageType(command.PopSpeech(), out var damageType))
		{
			actor.OutputHandler.Send(
				$"You must specify a valid damage type. Valid types are {Enum.GetValues(typeof(DamageType)).OfType<DamageType>().Select(x => x.Describe().Colour(Telnet.Cyan)).ListToString()}.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"You must specify a damage formula, or simply the number 0 to remove an existing damage type.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("0"))
		{
			var count = _damages.RemoveAll(x => x.DamageType == damageType);
			actor.OutputHandler.Send(
				$"Removed {count.ToString("N0", actor).Colour(Telnet.Green)} damages of type {damageType.Describe().Colour(Telnet.Cyan)}.");
			Changed = true;
			return true;
		}

		var expression = new Expression(command.PopSpeech());
		if (expression.HasErrors())
		{
			actor.OutputHandler.Send($"Error with the formula: {expression.Error.Colour(Telnet.Red)}.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a stun formula.");
			return false;
		}

		var stun = new Expression(command.SafeRemainingArgument);
		if (stun.HasErrors())
		{
			actor.OutputHandler.Send($"Error with the stun formula: {stun.Error.Colour(Telnet.Red)}.");
			return false;
		}

		_damages.Add((damageType, expression, stun));
		Changed = true;
		actor.OutputHandler.Send(
			$"You add another damage of type {damageType.Describe().Colour(Telnet.Cyan)} that does {expression.OriginalExpression.ColourCommand()} damage and {stun.OriginalExpression.ColourCommand()} stun.");
		return true;
	}

	private bool BuildingCommandExplosion(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify an explosion emote for this bomb. Use $0 for the bomb item.");
			return false;
		}

		var emote = new Emote(command.RemainingArgument, new DummyPerceiver(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		ExplosionEmoteText = command.RemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"The explosion emote for this bomb is now {ExplosionEmoteText.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandProximity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must specify a maximum proximity at which targets will be affected by this bomb's damage. The valid values, in order of closeness, are: {Enum.GetValues(typeof(Proximity)).OfType<Proximity>().Select(x => x.Describe().Colour(Telnet.Cyan)).ListToString()}.");
			return false;
		}

		if (!Enum.TryParse<Proximity>(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send(
				$"That is not a valid proximity. The valid values, in order of closeness, are: {Enum.GetValues(typeof(Proximity)).OfType<Proximity>().Select(x => x.Describe().Colour(Telnet.Cyan)).ListToString()}.");
			return false;
		}

		MaximumProximity = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This bomb will now only affect targets who are {MaximumProximity.Describe().Colour(Telnet.Green)} or closer to the explosion.");
		return true;
	}

	private bool BuildingCommandVolume(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must specify a valid audio volume for the explosion. Valid values are {Enum.GetValues(typeof(AudioVolume)).OfType<AudioVolume>().Select(x => x.Describe().Colour(Telnet.Cyan)).ListToString()}.");
			return false;
		}

		if (!Enum.TryParse<AudioVolume>(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send(
				$"That is not a valid audio volume for the explosion. Valid values are {Enum.GetValues(typeof(AudioVolume)).OfType<AudioVolume>().Select(x => x.Describe().Colour(Telnet.Cyan)).ListToString()}.");
			return false;
		}

		ExplosionVolume = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This explosion will now have a volume of {ExplosionVolume.Describe().Colour(Telnet.Cyan)}.");
		return true;
	}

	private bool BuildingCommandSize(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must specify a valid size. The valid values are {Enum.GetValues(typeof(SizeCategory)).OfType<SizeCategory>().Select(x => x.Describe().Colour(Telnet.Cyan)).ListToString()}.");
			return false;
		}

		if (!GameItemEnumExtensions.TryParseSize(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send(
				$"That is not a valid size. The valid values are {Enum.GetValues(typeof(SizeCategory)).OfType<SizeCategory>().Select(x => x.Describe().Colour(Telnet.Cyan)).ListToString()}.");
			return false;
		}

		ExplosionSize = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This bomb will now have an explosion of size {ExplosionSize.Describe().Colour(Telnet.Cyan)}.");
		return true;
	}

	private bool BuildingCommandZone(ICharacter actor, StringStack command)
	{
		FeltInZone = !FeltInZone;
		Changed = true;
		actor.OutputHandler.Send(
			$"The explosion caused by this bomb will {(FeltInZone ? "now" : "no longer")} be felt in the entire zone.");
		return true;
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis is a bomb. It produces a {4} sized explosion of volume {5}, and {6} be felt throughout the zone. It affects targets at a maximum {7} proximity.\n\nExplode Emote: {8}\n\nDamages:\n{9}",
			"Bomb Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			ExplosionSize.Describe().Colour(Telnet.Green),
			ExplosionVolume.Describe().Colour(Telnet.Green),
			FeltInZone ? "can".Colour(Telnet.Green) : "cannot".Colour(Telnet.Red),
			MaximumProximity.Describe().Colour(Telnet.Green),
			ExplosionEmoteText.Colour(Telnet.Yellow),
			string.Join("\n",
				_damages.Select(x =>
					$"{x.DamageType.Describe()} @ {x.DamageExpression.OriginalExpression.Colour(Telnet.Red)} / {x.StunExpression.OriginalExpression.Colour(Telnet.Cyan)} stun"))
		);
	}
}
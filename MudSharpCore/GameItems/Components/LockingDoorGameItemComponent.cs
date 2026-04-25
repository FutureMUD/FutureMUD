using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System.Linq;
using System.Text;
using System.Xml.Linq;

#nullable enable
#nullable disable warnings

namespace MudSharp.GameItems.Components;

public class LockingDoorGameItemComponent : DoorGameItemComponentBase, ILock
{
	protected LockingDoorGameItemComponentProto _lockingPrototype;
	private bool _isLocked;
	private int _pattern;

	public LockingDoorGameItemComponent(LockingDoorGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(proto, parent, temporary)
	{
		_lockingPrototype = proto;
	}

	public LockingDoorGameItemComponent(MudSharp.Models.GameItemComponent component,
		LockingDoorGameItemComponentProto proto, IGameItem parent)
		: base(component, proto, parent)
	{
		_lockingPrototype = proto;
	}

	public LockingDoorGameItemComponent(LockingDoorGameItemComponent rhs, IGameItem newParent, bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_lockingPrototype = rhs._lockingPrototype;
		_isLocked = rhs._isLocked;
		_pattern = rhs._pattern;
	}

	public override IGameItemComponentProto Prototype => _lockingPrototype;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new LockingDoorGameItemComponent(this, newParent, temporary);
	}

	public override bool PreventsMerging(IGameItemComponent component)
	{
		return Pattern != 0;
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		base.UpdateComponentNewPrototype(newProto);
		_lockingPrototype = (LockingDoorGameItemComponentProto)newProto;
	}

	protected override void AppendAdditionalDecorations(StringBuilder builder, IPerceiver voyeur)
	{
		builder.AppendLine();
		builder.AppendLine(
			$"It has a built-in lock that accepts keys of type {LockType.ColourName()} and {(IsLocked ? "is currently locked." : "is currently unlocked.").Colour(Telnet.Yellow)}");
	}

	protected override bool IsOpeningBlocked(IBody? body)
	{
		return IsLocked || base.IsOpeningBlocked(body);
	}

	protected override void LoadAdditionalFromXml(XElement root)
	{
		var element = root.Element("IsLocked");
		if (element is not null)
		{
			IsLocked = bool.Parse(element.Value);
		}

		element = root.Element("Pattern");
		if (element is not null)
		{
			_pattern = int.Parse(element.Value);
		}
	}

	protected override void SaveAdditionalToXml(XElement root)
	{
		root.Add(new XElement("Pattern", Pattern));
		root.Add(new XElement("IsLocked", IsLocked));
	}

	public bool CanBeInstalled => false;

	public void InstallLock(ILockable lockable, IExit exit, ICell installLocation)
	{
	}

	public bool SetLocked(bool locked, bool echo)
	{
		if (locked == IsLocked)
		{
			return false;
		}

		IsLocked = locked;
		if (echo)
		{
			Parent.OutputHandler.Handle(
				new EmoteOutput(
					new Emote(IsLocked ? _lockingPrototype.LockEmoteNoActor : _lockingPrototype.UnlockEmoteNoActor,
						Parent, Parent),
					flags: OutputFlags.SuppressObscured));
		}

		return true;
	}

	public bool CanUnlock(ICharacter actor, IKey key)
	{
		if (actor?.IsAdministrator() != false)
		{
			return true;
		}

		if (Parent.EffectsOfType<IOverrideLockEffect>().Any(x => x.Applies(actor)))
		{
			return false;
		}

		return key?.Unlocks(LockType, Pattern) == true;
	}

	public bool Unlock(ICharacter actor, IKey key, IPerceivable containingPerceivable, IEmote playerEmote)
	{
		if (!CanUnlock(actor, key))
		{
			return false;
		}

		IsLocked = false;
		if (actor is not null && key is not null)
		{
			actor.OutputHandler.Handle(new MixedEmoteOutput(
				new Emote(_lockingPrototype.UnlockEmote, actor, actor, Parent, key.Parent),
				flags: OutputFlags.SuppressObscured).Append(playerEmote));
			InstalledExit?.Cells.Except(actor.Location)
				.Single()
				.Handle(new EmoteOutput(new Emote(_lockingPrototype.UnlockEmoteOtherSide, actor, actor, Parent,
					key.Parent)));
		}
		else
		{
			foreach (var cell in Parent.TrueLocations)
			{
				cell.Handle(new EmoteOutput(new Emote(_lockingPrototype.UnlockEmoteNoActor, Parent, Parent)));
			}
		}

		return true;
	}

	public bool CanLock(ICharacter actor, IKey key)
	{
		return actor?.IsAdministrator() != false || key?.Unlocks(LockType, Pattern) == true;
	}

	public bool Lock(ICharacter actor, IKey key, IPerceivable containingPerceivable, IEmote playerEmote)
	{
		if (!CanLock(actor, key))
		{
			return false;
		}

		IsLocked = true;
		if (actor is not null && key is not null)
		{
			actor.OutputHandler.Handle(new MixedEmoteOutput(
				new Emote(_lockingPrototype.LockEmote, actor, actor, Parent, key.Parent),
				flags: OutputFlags.SuppressObscured).Append(playerEmote));
			InstalledExit?.Cells.Except(actor.Location)
				.Single()
				.Handle(new EmoteOutput(new Emote(_lockingPrototype.LockEmoteOtherSide, actor, actor, Parent,
					key.Parent)));
		}
		else
		{
			foreach (var cell in Parent.TrueLocations)
			{
				cell.Handle(new EmoteOutput(new Emote(_lockingPrototype.LockEmoteNoActor, Parent, Parent)));
			}
		}

		return true;
	}

	public bool IsLocked
	{
		get => _isLocked || Parent.EffectsOfType<IOverrideLockEffect>().Any(x => x.Applies());
		set
		{
			_isLocked = value;
			Changed = true;
		}
	}

	public string LockType => _lockingPrototype.LockType;

	public int Pattern
	{
		get => _pattern;
		set
		{
			_pattern = value;
			Changed = true;
		}
	}

	public Difficulty ForceDifficulty => _lockingPrototype.ForceDifficulty;
	public Difficulty PickDifficulty => _lockingPrototype.PickDifficulty;

	public string Inspect(ICharacter actor, string description)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"You identify the following information about {Parent.HowSeen(actor)}");
		sb.AppendLine(string.Empty);
		sb.AppendLine($"\tThis door's in-built lock is considered a {LockType.Colour(Telnet.Green)} style lock.");
		sb.AppendLine(Pattern == 0
			? "\tThis door's in-built lock has no combination set."
			: "\tThis door's in-built lock has a combination set so can have keys paired to it.");
		sb.AppendLine($"\tThis door's in-built lock appears to be {PickDifficulty.DescribeColoured()} to pick.");
		sb.AppendLine($"\tThis door's in-built lock appears to be {ForceDifficulty.DescribeColoured()} to force.");
		if (actor.IsAdministrator())
		{
			sb.AppendLine(
				$"  The combination for this door's in-built lock is: {Pattern.ToString("N0", actor).ColourValue()}");
		}

		return sb.ToString();
	}
}

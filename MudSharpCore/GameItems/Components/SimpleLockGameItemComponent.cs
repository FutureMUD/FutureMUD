using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.Construction;
using System.Text;

namespace MudSharp.GameItems.Components;

public class SimpleLockGameItemComponent : GameItemComponent, ILock
{
	private SimpleLockGameItemComponentProto _prototype;
	public IExit InstalledExit { get; set; }
	public override IGameItemComponentProto Prototype => _prototype;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new SimpleLockGameItemComponent(this, newParent, temporary);
	}

	public override bool PreventsMerging(IGameItemComponent component)
	{
		return Pattern != 0; // Locks that have been set don't stack
	}

	public override int DecorationPriority => -1;

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Full || type == DescriptionType.Contents;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		switch (type)
		{
			case DescriptionType.Full:
			case DescriptionType.Contents:
				return
					$"{description}\nIt is a lock that accepts key of type {LockType.ColourName()}.\n{(IsLocked ? "It is currently locked." : "It is currently unlocked.").Colour(Telnet.Yellow)}";
			default:
				return base.Decorate(voyeur, name, description, type, colour, flags);
		}
	}

	public override bool Die(IGameItem newItem, ICell location)
	{
		var newItemLock = newItem?.GetItemType<SimpleLockGameItemComponent>();
		if (newItemLock != null)
		{
			newItemLock._isLocked = IsLocked;
			newItemLock.InstalledExit = InstalledExit;
			newItemLock.Pattern = Pattern;
		}

		return false;
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (SimpleLockGameItemComponentProto)newProto;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("InstalledExit", InstalledExit?.Id ?? 0),
			new XElement("Pattern", Pattern),
			new XElement("IsLocked", IsLocked)
		).ToString();
	}

	#region Constructors

	public SimpleLockGameItemComponent(SimpleLockGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(parent, proto, temporary)
	{
		Pattern = Constants.Random.Next(0, 1000000);
		_prototype = proto;
	}

	public SimpleLockGameItemComponent(MudSharp.Models.GameItemComponent component,
		SimpleLockGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	private void LoadFromXml(XElement root)
	{
		var element = root.Element("IsLocked");
		if (element != null)
		{
			IsLocked = bool.Parse(element.Value);
		}

		element = root.Element("InstalledExit");
		if (element != null)
		{
			InstalledExit = Gameworld.ExitManager.GetExitByID(long.Parse(element.Value));
		}

		element = root.Element("Pattern");
		if (element != null)
		{
			_pattern = int.Parse(element.Value);
		}
	}

	public SimpleLockGameItemComponent(SimpleLockGameItemComponent rhs, IGameItem newParent, bool temporary = false) :
		base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	#endregion

	#region ILock Members

	public bool CanBeInstalled => true;

	public void InstallLock(ILockable lockable, IExit exit, ICell installLocation)
	{
		InstalledExit = exit;
		Changed = true;
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
					new Emote(IsLocked ? _prototype.LockEmoteNoActor : _prototype.UnlockEmoteNoActor, Parent, Parent,
						Parent.ContainedIn),
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
		if (actor != null && key != null)
		{
			actor.OutputHandler.Handle(new MixedEmoteOutput(
				new Emote(_prototype.UnlockEmote, actor, actor, Parent, key?.Parent,
					containingPerceivable),
				flags: OutputFlags.SuppressObscured).Append(playerEmote));
			InstalledExit?.Cells.Except(actor.Location)
			             .Single()
			             .Handle(
				             new EmoteOutput(new Emote(_prototype.UnlockEmoteOtherSide, actor, actor, Parent,
					             key?.Parent, containingPerceivable)));
		}
		else
		{
			foreach (var cell in Parent.TrueLocations)
			{
				cell.Handle(
					new EmoteOutput(new Emote(_prototype.UnlockEmoteNoActor, Parent, Parent, containingPerceivable)));
			}
		}

		return true;
	}

	public bool CanLock(ICharacter actor, IKey key)
	{
		if (actor?.IsAdministrator() != false)
		{
			return true;
		}

		return key?.Unlocks(LockType, Pattern) == true;
	}

	public bool Lock(ICharacter actor, IKey key, IPerceivable containingPerceivable, IEmote playerEmote)
	{
		if (!CanLock(actor, key))
		{
			return false;
		}

		IsLocked = true;
		if (actor != null && key != null)
		{
			actor.OutputHandler.Handle(new MixedEmoteOutput(
				new Emote(_prototype.LockEmote, actor, actor, Parent, key.Parent,
					containingPerceivable),
				flags: OutputFlags.SuppressObscured).Append(playerEmote));
			InstalledExit?.Cells.Except(actor.Location)
			             .Single()
			             .Handle(
				             new EmoteOutput(new Emote(_prototype.LockEmoteOtherSide, actor, actor, Parent,
					             key?.Parent, containingPerceivable)));
		}
		else
		{
			foreach (var cell in Parent.TrueLocations)
			{
				cell.Handle(
					new EmoteOutput(new Emote(_prototype.LockEmoteNoActor, Parent, Parent, containingPerceivable)));
			}
		}

		return true;
	}

	private bool _isLocked;

	public bool IsLocked
	{
		get { return _isLocked || Parent.EffectsOfType<IOverrideLockEffect>().Any(x => x.Applies()); }
		set
		{
			_isLocked = value;
			Changed = true;
		}
	}

	public string LockType => _prototype.LockType;

	private int _pattern;

	public int Pattern
	{
		get => _pattern;
		set
		{
			_pattern = value;
			Changed = true;
		}
	}

	public Difficulty ForceDifficulty => _prototype.ForceDifficulty;
	public Difficulty PickDifficulty => _prototype.PickDifficulty;

	public string Inspect(ICharacter actor, string description)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"You identify the following information about {Parent.HowSeen(actor)}");
		sb.AppendLine("");
		sb.AppendLine($"\tThis lock is considered a {LockType.Colour(Telnet.Green)} style lock.");
		sb.AppendLine(Pattern == 0
			? "\tThis lock has no combination set."
			: "\tThis lock has a combination set so can have keys paired to it.");
		sb.AppendLine($"\tThis lock appears to be {PickDifficulty.DescribeColoured()} to pick.");
		sb.AppendLine($"\tThis lock appears to be {ForceDifficulty.DescribeColoured()} to force.");
		if (actor.IsAdministrator())
		{
			sb.AppendLine($"  The combination for this lock is: {Pattern.ToString("N0", actor).ColourValue()}");
		}

		return sb.ToString();
	}

	#endregion
}
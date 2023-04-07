using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction.Boundary;
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

public class ProgLockGameItemComponent : GameItemComponent, ILock
{
	private ProgLockGameItemComponentProto _prototype;
	public IExit InstalledExit { get; set; }
	public override IGameItemComponentProto Prototype => _prototype;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new ProgLockGameItemComponent(this, newParent, temporary);
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
				return description + "\n" +
				       (IsLocked ? "It is currently locked." : "It is currently unlocked.").Colour(Telnet.Yellow);
			default:
				return base.Decorate(voyeur, name, description, type, colour, flags);
		}
	}

	public override bool Die(IGameItem newItem, ICell location)
	{
		var newItemLock = newItem?.GetItemType<ProgLockGameItemComponent>();
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
		_prototype = (ProgLockGameItemComponentProto)newProto;
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

	public ProgLockGameItemComponent(ProgLockGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public ProgLockGameItemComponent(MudSharp.Models.GameItemComponent component, ProgLockGameItemComponentProto proto,
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

	public ProgLockGameItemComponent(ProgLockGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(
		rhs, newParent, temporary)
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
					new Emote(IsLocked ? _prototype.LockEmoteNoActor : _prototype.UnlockEmoteNoActor, Parent),
					flags: OutputFlags.SuppressObscured));
		}

		return true;
	}

	public bool CanUnlock(ICharacter actor, IKey key)
	{
		return actor?.IsAdministrator() != false;
	}

	public bool Unlock(ICharacter actor, IKey key, IPerceivable containingPerceivable, IEmote playerEmote)
	{
		if (!CanUnlock(actor, key))
		{
			return false;
		}

		IsLocked = false;
		if (actor != null)
		{
			actor.OutputHandler.Handle(new MixedEmoteOutput(
				new Emote(_prototype.UnlockEmoteNoActor, actor, Parent, containingPerceivable),
				flags: OutputFlags.SuppressObscured).Append(playerEmote));
			InstalledExit?.Cells.Except(actor.Location)
			             .Single()
			             .Handle(
				             new EmoteOutput(new Emote(_prototype.UnlockEmoteOtherSide, actor, Parent,
					             key?.Parent, containingPerceivable)));
		}
		else
		{
			foreach (var cell in Parent.TrueLocations)
			{
				cell.Handle(
					new EmoteOutput(new Emote(_prototype.UnlockEmoteNoActor, Parent, containingPerceivable)));
			}
		}

		return true;
	}

	public bool CanLock(ICharacter actor, IKey key)
	{
		return actor?.IsAdministrator() != false;
	}

	public bool Lock(ICharacter actor, IKey key, IPerceivable containingPerceivable, IEmote playerEmote)
	{
		if (!CanLock(actor, key))
		{
			return false;
		}

		IsLocked = true;
		if (actor != null)
		{
			actor.OutputHandler.Handle(new MixedEmoteOutput(
				new Emote(_prototype.LockEmoteNoActor, actor, Parent, containingPerceivable),
				flags: OutputFlags.SuppressObscured).Append(playerEmote));
			InstalledExit?.Cells.Except(actor.Location)
			             .Single()
			             .Handle(
				             new EmoteOutput(new Emote(_prototype.LockEmoteOtherSide, actor, Parent,
					             key?.Parent, containingPerceivable)));
		}
		else
		{
			foreach (var cell in Parent.TrueLocations)
			{
				cell.Handle(
					new EmoteOutput(new Emote(_prototype.LockEmoteNoActor, Parent, containingPerceivable)));
			}
		}

		return true;
	}

	private bool _isLocked;

	public bool IsLocked
	{
		get => _isLocked;
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
		sb.AppendLine(" ");
		sb.AppendLine("  This lock is considered a " + LockType.Colour(Telnet.Green) + " style lock.");
		sb.AppendLine(Pattern == 0
			? "  This lock has no combination set."
			: "  This lock has a combination set so can have keys paired to it.");
		sb.AppendLine("  This lock appears to be " + PickDifficulty.ToString().Colour(Telnet.Green) + " to pick.");
		sb.AppendLine("  This lock appears to be " + ForceDifficulty.ToString().Colour(Telnet.Green) + " to force.");
		if (actor.IsAdministrator())
		{
			sb.AppendLine("  The combination for this lock is: " + Pattern.ToString().Colour(Telnet.Green));
		}

		return sb.ToString();
	}

	#endregion
}
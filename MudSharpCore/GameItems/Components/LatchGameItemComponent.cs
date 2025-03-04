using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System.Text;

namespace MudSharp.GameItems.Components;

public class LatchGameItemComponent : GameItemComponent, ILock
{
	private LatchGameItemComponentProto _prototype;
	public IExit InstalledExit { get; set; }
	public ICell LatchSideCell { get; set; }
	public override IGameItemComponentProto Prototype => _prototype;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new LatchGameItemComponent(this, newParent, temporary);
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

	public override bool HandleDieOrMorph(IGameItem newItem, ICell location)
	{
		var newItemLatch = newItem?.GetItemType<LatchGameItemComponent>();
		if (newItemLatch != null)
		{
			newItemLatch.InstalledExit = InstalledExit;
			newItemLatch.LatchSideCell = LatchSideCell;
			newItemLatch._isLocked = IsLocked;
		}

		return false;
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (LatchGameItemComponentProto)newProto;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("LatchSideCell", LatchSideCell?.Id ?? 0),
			new XElement("InstalledExit", InstalledExit?.Id ?? 0),
			new XElement("IsLocked", IsLocked)
		).ToString();
	}

	#region Constructors

	public LatchGameItemComponent(LatchGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public LatchGameItemComponent(MudSharp.Models.GameItemComponent component, LatchGameItemComponentProto proto,
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

		element = root.Element("LatchSideCell");
		if (element != null)
		{
			LatchSideCell = Gameworld.Cells.Get(long.Parse(element.Value));
		}

		element = root.Element("InstalledExit");
		if (element != null)
		{
			InstalledExit = Gameworld.ExitManager.GetExitByID(long.Parse(element.Value));
		}
	}

	public LatchGameItemComponent(LatchGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs,
		newParent, temporary)
	{
		_prototype = rhs._prototype;
		IsLocked = rhs.IsLocked;
	}

	#endregion

	#region ILock Members

	public bool CanBeInstalled => true;

	public bool CanUnlock(ICharacter actor, IKey key)
	{
		if (actor?.IsAdministrator() != false)
		{
			return true;
		}

		if (key != null)
		{
			return false;
		}

		return InstalledExit == null || LatchSideCell == null || actor.Location == LatchSideCell;
	}

	public bool Unlock(ICharacter actor, IKey key, IPerceivable containingPerceivable, IEmote playerEmote)
	{
		if (!CanUnlock(actor, key))
		{
			return false;
		}

		IsLocked = false;
		actor.OutputHandler.Handle(
			new MixedEmoteOutput(new Emote(_prototype.UnlockEmote, actor, actor, Parent, containingPerceivable),
				flags: OutputFlags.SuppressObscured).Append(playerEmote));
		InstalledExit?.Cells.Except(LatchSideCell)
		             .Single()
		             .Handle(
			             new EmoteOutput(new Emote(_prototype.UnlockEmoteOtherSide, actor, actor, Parent,
				             containingPerceivable)));
		return true;
	}

	public bool CanLock(ICharacter actor, IKey key)
	{
		if (actor?.IsAdministrator() != false)
		{
			return true;
		}

		if (key != null)
		{
			return false;
		}

		return InstalledExit == null || LatchSideCell == null || actor.Location == LatchSideCell;
	}

	public bool Lock(ICharacter actor, IKey key, IPerceivable containingPerceivable, IEmote playerEmote)
	{
		if (!CanLock(actor, key))
		{
			return false;
		}

		IsLocked = true;
		actor.OutputHandler.Handle(
			new MixedEmoteOutput(new Emote(_prototype.LockEmote, actor, actor, Parent, containingPerceivable),
				flags: OutputFlags.SuppressObscured).Append(playerEmote));
		InstalledExit?.Cells.Except(LatchSideCell)
		             .Single()
		             .Handle(
			             new EmoteOutput(new Emote(_prototype.LockEmoteOtherSide, actor, actor, Parent,
				             containingPerceivable)));
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
						Parent?.ContainedIn),
					flags: OutputFlags.SuppressObscured));
		}

		return true;
	}

	public string LockType => string.Empty;

	public void InstallLock(ILockable lockable, IExit exit, ICell installLocation)
	{
		InstalledExit = exit;
		LatchSideCell = installLocation;
		Changed = true;
	}

	public int Pattern
	{
		get => 0;
		set { }
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
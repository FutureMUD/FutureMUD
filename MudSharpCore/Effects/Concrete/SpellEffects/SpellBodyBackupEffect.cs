#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete.SpellEffects;

public class SpellBodyBackupEffect : SimpleSpellStatusEffectBase, IBodyBackupEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellBodyBackup", (effect, owner) => new SpellBodyBackupEffect(effect, owner));
	}

	public SpellBodyBackupEffect(IPerceivable owner, IMagicSpellEffectParent parent, string formKey, long backupBodyId,
		long destinationCellId, RoomLayer destinationLayer, int priority, BodyRemainsContext remainsContext,
		string oldLocationEcho, string newLocationEcho, string selfEcho, bool consumeOnUse, IFutureProg? prog = null)
		: base(owner, parent, prog)
	{
		FormKey = formKey;
		BackupBodyId = backupBodyId;
		DestinationCellId = destinationCellId;
		DestinationLayer = destinationLayer;
		Priority = priority;
		RemainsContext = BodyBackupEffect.NormaliseBackupRemainsContext(remainsContext);
		OldLocationEcho = oldLocationEcho;
		NewLocationEcho = newLocationEcho;
		SelfEcho = selfEcho;
		ConsumeOnUse = consumeOnUse;
	}

	private SpellBodyBackupEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
		var trueRoot = root.Element("Effect");
		FormKey = trueRoot?.Element("FormKey")?.Value ?? string.Empty;
		BackupBodyId = long.Parse(trueRoot?.Element("BackupBodyId")?.Value ?? "0");
		DestinationCellId = long.Parse(trueRoot?.Element("DestinationCellId")?.Value ?? "0");
		DestinationLayer = (RoomLayer)int.Parse(trueRoot?.Element("DestinationLayer")?.Value ?? "0");
		Priority = int.Parse(trueRoot?.Element("Priority")?.Value ?? "0");
		RemainsContext =
			BodyBackupEffect.NormaliseBackupRemainsContext((BodyRemainsContext)int.Parse(trueRoot?.Element("RemainsContext")?.Value ?? "1"));
		OldLocationEcho = trueRoot?.Element("OldLocationEcho")?.Value ?? BodyBackupEffect.DefaultOldLocationEcho;
		NewLocationEcho = trueRoot?.Element("NewLocationEcho")?.Value ?? BodyBackupEffect.DefaultNewLocationEcho;
		SelfEcho = trueRoot?.Element("SelfEcho")?.Value ?? BodyBackupEffect.DefaultSelfEcho;
		ConsumeOnUse = bool.Parse(trueRoot?.Element("ConsumeOnUse")?.Value ?? "true");
	}

	public string FormKey { get; }
	public long BackupBodyId { get; }
	public long DestinationCellId { get; }
	public ICell? DestinationCell => Gameworld.Cells.Get(DestinationCellId);
	public RoomLayer DestinationLayer { get; }
	public int Priority { get; }
	public BodyRemainsContext RemainsContext { get; }
	public bool ConsumeOnUse { get; }
	public string OldLocationEcho { get; }
	public string NewLocationEcho { get; }
	public string SelfEcho { get; }
	public string SourceDescription => ParentEffect?.Spell is null
		? $"spell:{FormKey}"
		: $"spell #{ParentEffect.Spell.Id.ToString("N0")}:{FormKey}";

	public void ConsumeBackup(ICharacter character)
	{
		if (ConsumeOnUse)
		{
			character.RemoveEffect(this, true);
		}
	}

	protected override XElement SaveDefinition()
	{
		return SimpleSaveDefinition(
			new XElement("FormKey", FormKey),
			new XElement("BackupBodyId", BackupBodyId),
			new XElement("DestinationCellId", DestinationCellId),
			new XElement("DestinationLayer", (int)DestinationLayer),
			new XElement("Priority", Priority),
			new XElement("RemainsContext", (int)RemainsContext),
			new XElement("OldLocationEcho", new XCData(OldLocationEcho)),
			new XElement("NewLocationEcho", new XCData(NewLocationEcho)),
			new XElement("SelfEcho", new XCData(SelfEcho)),
			new XElement("ConsumeOnUse", ConsumeOnUse)
		);
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Magically ready to transfer into body #{BackupBodyId.ToString("N0", voyeur)} on death.";
	}

	protected override string SpecificEffectType => "SpellBodyBackup";
}

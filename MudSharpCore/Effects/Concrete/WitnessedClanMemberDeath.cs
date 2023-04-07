using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character.Name;
using MudSharp.Community;

namespace MudSharp.Effects.Concrete;

public class WitnessedClanMemberDeath : Effect
{
	private long _clanMemberId;
	private ICharacter _clanMember;

	public ICharacter ClanMember
	{
		get { return _clanMember ??= Gameworld.TryGetCharacter(_clanMemberId, true); }
	}

	public IClan Clan { get; private set; }

	#region Static Initialisation

	public static void InitialiseEffectType()
	{
		RegisterFactory("WitnessedClanMemberDeath", (effect, owner) => new WitnessedClanMemberDeath(effect, owner));
	}

	#endregion

	#region Constructors

	public WitnessedClanMemberDeath(IPerceivable owner, ICharacter clanMember, IClan clan) : base(owner, null)
	{
		Clan = clan;
		_clanMember = clanMember;
		_clanMemberId = clanMember.Id;
		SubscribeEvents();
	}

	protected WitnessedClanMemberDeath(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		var root = effect.Element("Effect");
		_clanMemberId = long.Parse(root.Element("Member").Value);
		Clan = Gameworld.Clans.Get(long.Parse(root.Element("Clan").Value));
	}

	#endregion

	// Note: You can safely delete this entire region if your effect acts more like a flag and doesn't actually save any specific data on it (e.g. immwalk, admin telepathy, etc)

	#region Saving and Loading

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("Member", _clanMemberId),
			new XElement("Clan", Clan.Id)
		);
	}

	#endregion

	#region Overrides of Effect

	protected override string SpecificEffectType => "WitnessedClanMemberDeath";

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"Witnessed the death of clan member {ClanMember.PersonalName.GetName(NameStyle.FullName).ColourName()} in clan {Clan.FullName.ColourName()}";
	}

	public override bool SavingEffect => true;

	public override void Login()
	{
		SubscribeEvents();
	}

	public override void RemovalEffect()
	{
		ReleaseEvents();
	}

	#endregion

	private void SubscribeEvents()
	{
		Gameworld.HeartbeatManager.FuzzyHourHeartbeat -= HeartbeatManager_FuzzyHourHeartbeat;
		Gameworld.HeartbeatManager.FuzzyHourHeartbeat += HeartbeatManager_FuzzyHourHeartbeat;
	}

	private void HeartbeatManager_FuzzyHourHeartbeat()
	{
		var membership = Clan.Memberships.FirstOrDefault(x => x.MemberId == _clanMemberId);
		if (membership is null || membership.IsArchivedMembership)
		{
			Owner.RemoveEffect(this, true);
			return;
		}
	}

	private void ReleaseEvents()
	{
		Gameworld.HeartbeatManager.FuzzyHourHeartbeat -= HeartbeatManager_FuzzyHourHeartbeat;
	}

	#region Overrides of Effect

	#endregion
}
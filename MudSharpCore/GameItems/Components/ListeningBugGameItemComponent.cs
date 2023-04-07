using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Communication.Language;
using MudSharp.Events;
using MudSharp.Form.Audio;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Components;

public class ListeningBugGameItemComponent : GameItemComponent, IConsumePower, ITransmit
{
	protected ListeningBugGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (ListeningBugGameItemComponentProto)newProto;
	}

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition").ToString();
	}

	#endregion

	#region IConsumePower Implementation

	public double PowerConsumptionInWatts => _prototype.PowerConsumptionInWatts;

	private bool _powered;

	public void OnPowerCutIn()
	{
		_powered = true;
	}

	public void OnPowerCutOut()
	{
		_powered = false;
	}

	#endregion

	#region ITransmit Implementation

	public bool ManualTransmit => false;

	public string TransmitPremote => "";

	public void Transmit(SpokenLanguageInfo spokenLanguage)
	{
		if (!_powered)
		{
			return;
		}

		var location = Parent.TrueLocations.FirstOrDefault();
		if (location == null)
		{
#if DEBUG
			Console.WriteLine("Transmit had no location.");
#endif
			return;
		}

		var zones =
			Gameworld.Zones.Where(x => x.Shard == location.Shard)
			         .Where(x => x.Geography.DistanceTo(location.Zone.Geography) < _prototype.BroadcastRange)
			         .ToList();

		foreach (
			var item in
			Gameworld.Items.Where(x => x.TrueLocations.Any(y => zones.Contains(y.Room.Zone)))
			         .SelectNotNull(x => x.GetItemType<IReceive>()))
		{
			item.ReceiveTransmission(_prototype.BroadcastFrequency, spokenLanguage, 0, this);
		}
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		switch (type)
		{
			case EventType.CharacterSpeaksWitness:
				HandleSpeechEvent((ICharacter)arguments[0], (AudioVolume)arguments[2], (Language)arguments[3],
					(Accent)arguments[4], (string)arguments[5]);
				return false;
			case EventType.CharacterSpeaksDirectWitness:
				HandleSpeechEvent((ICharacter)arguments[0], (AudioVolume)arguments[3], (Language)arguments[4],
					(Accent)arguments[5], (string)arguments[6]);
				return false;
		}

		return false;
	}

	private void HandleSpeechEvent(ICharacter character, AudioVolume audioVolume, Language language, Accent accent,
		string text)
	{
		if (!_powered)
		{
			return;
		}

		var eText = new ExplodedString(text);
		var skill = _prototype.BaseListenSkill + _prototype.ListenSkillPerQuality * (int)Parent.Quality;
		switch (character.Location.LocalAudioDifficulty(Parent, audioVolume, Parent.GetProximity(character)))
		{
			case Difficulty.Automatic:
				skill += 100;
				break;
			case Difficulty.Trivial:
				skill += 80;
				break;
			case Difficulty.ExtremelyEasy:
				skill += 50;
				break;
			case Difficulty.VeryEasy:
				skill += 25;
				break;
			case Difficulty.Easy:
				skill += 10;
				break;
			case Difficulty.Hard:
				skill -= 10;
				break;
			case Difficulty.VeryHard:
				skill -= 25;
				break;
			case Difficulty.ExtremelyHard:
				skill -= 50;
				break;
			case Difficulty.Insane:
				skill -= 80;
				break;
			case Difficulty.Impossible:
				skill -= 100;
				break;
		}

		var roll = RandomUtilities.ConsecutiveRoll(
			100,
			skill,
			3
		);

		// TODO - scrambling affected by volume
		Transmit(new SpokenLanguageInfo(language, accent, audioVolume,
			Gameworld.ElectronicLanguageScrambler.Scramble(eText, 0.15 * Math.Min(roll, 0)), Outcome.MajorPass,
			character, null, Parent));
	}

	#endregion

	#region Constructors

	public ListeningBugGameItemComponent(ListeningBugGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public ListeningBugGameItemComponent(MudSharp.Models.GameItemComponent component,
		ListeningBugGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public ListeningBugGameItemComponent(ListeningBugGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	protected void LoadFromXml(XElement root)
	{
		// TODO
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new ListeningBugGameItemComponent(this, newParent, temporary);
	}

	#endregion

	/// <summary>
	///     Handles any finalisation that this component needs to perform before being deleted.
	/// </summary>
	public override void Delete()
	{
		base.Delete();
		Parent.GetItemType<IProducePower>()?.EndDrawdown(this);
	}

	public override void Login()
	{
		base.Login();
		Parent.GetItemType<IProducePower>()?.BeginDrawdown(this);
	}

	/// <summary>
	///     Handles any finalisation that this component needs to perform before being removed from the game (e.g. in inventory
	///     when player quits)
	/// </summary>
	public override void Quit()
	{
		base.Quit();
		Parent.GetItemType<IProducePower>()?.EndDrawdown(this);
	}
}
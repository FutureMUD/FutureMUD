using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using System.Collections.Generic;
using System.Text;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.Framework.Save;
using MudSharp.RPG.Checks;
using MudSharp.Events;

namespace MudSharp.GameItems.Components;

public class CorpseGameItemComponent : GameItemComponent, ICorpse, ILazyLoadDuringIdleTime
{
	protected CorpseGameItemComponentProto _prototype;

	public override bool PreventsMerging(IGameItemComponent component)
	{
		// Corpses never merge
		return true;
	}

	public override bool WarnBeforePurge => OriginalCharacter.Body.AllItems.Any();

	public override IGameItemComponentProto Prototype => _prototype;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new CorpseGameItemComponent(this, newParent, temporary);
	}

	public override int DecorationPriority => 1;

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Short || type == DescriptionType.Full ||
		       type == DescriptionType.Contents;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		if (flags.HasFlag(PerceiveIgnoreFlags.IgnoreLoadThings))
		{
			return "a corpse";
		}

		return Model.Describe(type, Decay, OriginalCharacter, voyeur,
			       EatenWeight / (Model.EdiblePercentage * OriginalCharacter.Weight)) +
		       (Skinned && type == DescriptionType.Short ? " (Skinned)".Colour(Telnet.Red) : "");
	}

	public override double ComponentWeight
	{
		get
		{
			return OriginalCharacter.Weight + OriginalCharacter.Body.ExternalItems.Sum(x => x.Weight) +
				OriginalCharacter.Body.Implants.Sum(x => x.Parent.Weight) +
				OriginalCharacter.Body.Prosthetics.Sum(x => x.Parent.Weight) - EatenWeight;
		}
	}

	public override double ComponentBuoyancy(double fluidDensity)
	{
		return (fluidDensity - 1.01) * (OriginalCharacter.Weight - EatenWeight) +
		       OriginalCharacter.Body.AllItems.Sum(x => x.Buoyancy(fluidDensity));
	}

	public override bool OverridesMaterial => OverridenMaterial != null;

	public override ISolid OverridenMaterial => Model.CorpseMaterial(_decayPoints);

	#region Overrides of GameItemComponent

	public override bool WrapFullDescription => false;

	#endregion

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (CorpseGameItemComponentProto)newProto;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("OriginalCharacter", _originalCharacterId),
			new XElement("Model", Model?.Id ?? 0),
			new XElement("DecayPoints", DecayPoints),
			new XElement("DecayState", (int)Decay),
			new XElement("EatenWeight", EatenWeight),
			new XElement("TimeOfDeath", new XText(TimeOfDeath.ToString(CultureInfo.InvariantCulture)))
		).ToString();
	}

	private void LoadFromXml(XElement definition)
	{
		_originalCharacterId = long.Parse(definition.Element("OriginalCharacter").Value);
		Model = Gameworld.CorpseModels.Get(long.Parse(definition.Element("Model").Value));
		DecayPoints = double.Parse(definition.Element("DecayPoints").Value);
		Decay = (DecayState)int.Parse(definition.Element("DecayState").Value);
		TimeOfDeath = DateTime.Parse(definition.Element("TimeOfDeath").Value, CultureInfo.InvariantCulture,
			DateTimeStyles.AssumeUniversal);
		EatenWeight = double.Parse(definition.Element("EatenWeight")?.Value ?? "0.0");
	}

	#region Constructors

	public CorpseGameItemComponent(CorpseGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
		Decay = DecayState.Fresh;
		TimeOfDeath = DateTime.UtcNow;
		if (!temporary)
		{
			SetupDecayListener();
		}
	}

	public CorpseGameItemComponent(MudSharp.Models.GameItemComponent component, CorpseGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		Gameworld.SaveManager.AddLazyLoad(this);
		_noSave = false;
		SetupDecayListener();
	}

	public CorpseGameItemComponent(CorpseGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs,
		newParent, temporary)
	{
		_prototype = rhs._prototype;
		_originalCharacterId = rhs._originalCharacterId;
		_originalCharacter = rhs._originalCharacter;
		Model = rhs.Model;
		Decay = rhs.Decay;
		DecayPoints = rhs.DecayPoints;
		TimeOfDeath = rhs.TimeOfDeath;
		Skinned = rhs.Skinned;
		if (!temporary)
		{
			SetupDecayListener();
		}
	}

	private void SetupDecayListener()
	{
		Gameworld.HeartbeatManager.MinuteHeartbeat += HeartbeatManagerOnMinuteHeartbeat;
	}

	private void HeartbeatManagerOnMinuteHeartbeat()
	{
		// TODO - check for effects that halt or arrest decay
		if (!Parent.TrueLocations.Any())
		{
			Console.WriteLine("Corpse did not have any true location.");
			return;
		}

		DecayPoints += Model.DecayRate(
			Parent.Location != null
				? Parent.Location.CurrentOverlay.Terrain
				: Parent.TrueLocations.First().CurrentOverlay.Terrain
		);
	}

	public override void Delete()
	{
		OriginalCharacter.Corpse = null;
		base.Delete();
		Gameworld.HeartbeatManager.MinuteHeartbeat -= HeartbeatManagerOnMinuteHeartbeat;
		// If a corpse is deleted and its owner is still dead (i.e. hasn't been resurrected), delete the inventory
		if (OriginalCharacter.Status == CharacterStatus.Deceased)
		{
			foreach (var item in OriginalCharacter.Body.ExternalItems.ToList())
			{
				item.Delete();
			}
		}
	}

	public override void Quit()
	{
		base.Quit();
		Gameworld.HeartbeatManager.MinuteHeartbeat -= HeartbeatManagerOnMinuteHeartbeat;
	}

	public override bool Take(IGameItem item)
	{
		if (OriginalCharacter.Body.ExternalItems.Contains(item))
		{
			OriginalCharacter.Body.Take(item);
			return true;
		}

		return false;
	}

	#endregion

	#region ICorpse Members

	private long _originalCharacterId;
	private ICharacter _originalCharacter;

	private double _eatenWeight;

	public double EatenWeight
	{
		get => _eatenWeight;
		set
		{
			_eatenWeight = value;
			Changed = true;
		}
	}

	public double RemainingEdibleWeight => OriginalCharacter.Weight * Model.EdiblePercentage - EatenWeight;

	private void LoadOriginalCharacter(bool viaSaveManager)
	{
		_originalCharacter = Gameworld.TryGetCharacter(_originalCharacterId, true);
		_originalCharacter.Corpse = this;
		if (!viaSaveManager)
		{
			Gameworld.SaveManager.AbortLazyLoad(this);
		}
	}

	public ICharacter OriginalCharacter
	{
		get
		{
			if (_originalCharacter == null)
			{
				LoadOriginalCharacter(false);
			}

			return _originalCharacter;
		}
		set
		{
			_originalCharacter = value;
			_originalCharacterId = value?.Id ?? 0;
			Changed = true;
		}
	}

	public ICorpseModel Model { get; set; }

	private double _decayPoints;
	private double _nextSaveDecayPoints;

	public double DecayPoints
	{
		get => _decayPoints;
		set
		{
			if (value == _decayPoints)
			{
				return;
			}

			_decayPoints = value;
			Decay = Model.GetDecayState(_decayPoints);
			if (_nextSaveDecayPoints == 0)
			{
				_nextSaveDecayPoints = _decayPoints + 50 + Constants.Random.NextDouble() * 100.0;
			}

			if (_decayPoints >= _nextSaveDecayPoints)
			{
				//We only save body part decay point changes when the decayPoints have changed
				//by between 50-150 points in order to avoid hammering the save manager with
				//every severed body part all at once. 
				_nextSaveDecayPoints = _decayPoints + 50 + Constants.Random.NextDouble() * 100.0;
				Changed = true;
			}
		}
	}

	public DecayState Decay { get; set; }

	public DateTime TimeOfDeath { get; set; }

	private bool _skinned;

	public bool Skinned
	{
		get => _skinned;
		set
		{
			_skinned = value;
			Changed = true;
		}
	}

	public IEnumerable<IBodypart> Parts => OriginalCharacter.Body.Bodyparts;

	private readonly List<string> _butcheredSubcategories = new();
	public IEnumerable<string> ButcheredSubcategories => _butcheredSubcategories;

	public bool Butcher(ICharacter butcher, string subcategory = null)
	{
		var productSB = new StringBuilder();
		var products = new List<IGameItem>();
		var count = 0;

		void LoadItem(IGameItemProto proto, int quantity)
		{
			if (proto.Components.Any(x => x is StackableGameItemComponentProto))
			{
				var newItem = proto.CreateNew(butcher);
				newItem.RoomLayer = Parent.RoomLayer;
				newItem.GetItemType<IStackable>().Quantity = quantity;
				butcher.Location.Insert(newItem);
				newItem.HandleEvent(EventType.ItemFinishedLoading, newItem);
				newItem.Login();
				productSB.AppendLine($"\t${count++} has been produced.");
				products.Add(newItem);
				return;
			}

			for (var i = 0; i < quantity; i++)
			{
				var newItem = proto.CreateNew(butcher);
				newItem.RoomLayer = Parent.RoomLayer;
				Gameworld.Add(newItem);
				butcher.Location.Insert(newItem);
				newItem.HandleEvent(EventType.ItemFinishedLoading, newItem);
				newItem.Login();
				productSB.AppendLine($"\t${count++} has been produced.");
				products.Add(newItem);
			}
		}

		var check = Gameworld.GetCheck(CheckType.ButcheryCheck);
		var effect = butcher.EffectsOfType<Butchering>().First();
		foreach (var product in OriginalCharacter.Race.ButcheryProfile.Products.Where(x =>
			         !x.IsPelt && x.CanProduce(butcher, Parent) &&
			         (string.IsNullOrEmpty(subcategory) || x.Subcategory.EqualTo(subcategory))))
		{
			var damageRatio =
				Parent.Wounds.Where(x => product.RequiredBodyparts.Contains(x.Bodypart)).Sum(x => x.CurrentDamage) /
				product.RequiredBodyparts.Sum(x => OriginalCharacter.Body.HitpointsForBodypart(x));
			if (double.IsNaN(damageRatio))
			{
				damageRatio = 1.0;
			}

			foreach (var item in product.ProductItems)
			{
				var result = check.Check(butcher, effect.CheckDifficulty, effect.Trait);
				if (damageRatio >= item.DamagedThreshold || result.Outcome.In(Outcome.MajorFail, Outcome.Fail))
				{
					if (item.DamagedProto != null)
					{
						LoadItem(item.DamagedProto, item.DamagedQuantity);
						continue;
					}
				}

				LoadItem(item.NormalProto, item.NormalQuantity);
			}

			foreach (var part in product.RequiredBodyparts)
			foreach (var item in OriginalCharacter.Body.AllItemsAtOrDownstreamOfPart(part).ToList())
			{
				OriginalCharacter.Body.Take(item);
				butcher.Location.Insert(item);
			}
		}

		if (productSB.Length > 0)
		{
			Parent.OutputHandler.Handle(
				new EmoteOutput(new Emote(productSB.ToString(), Parent, products.ToArray<IPerceivable>())));
		}

		if (!string.IsNullOrEmpty(subcategory))
		{
			_butcheredSubcategories.Add(subcategory);
			Changed = true;
			return false;
		}

		foreach (var item in OriginalCharacter.Body.AllItems.ToList())
		{
			OriginalCharacter.Body.Take(item);
			butcher.Location.Insert(item);
		}

		return true;
	}

	public void Skin(ICharacter skinner)
	{
		void LoadItem(IGameItemProto proto, int quantity)
		{
			if (proto.Components.Any(x => x is StackableGameItemComponentProto))
			{
				var newItem = proto.CreateNew(skinner);
				newItem.RoomLayer = Parent.RoomLayer;
				newItem.GetItemType<IStackable>().Quantity = quantity;
				skinner.Location.Insert(newItem);
				newItem.HandleEvent(EventType.ItemFinishedLoading, newItem);
				newItem.Login();
				return;
			}

			for (var i = 0; i < quantity; i++)
			{
				var newItem = proto.CreateNew(skinner);
				newItem.RoomLayer = Parent.RoomLayer;
				Gameworld.Add(newItem);
				skinner.Location.Insert(newItem);
				newItem.HandleEvent(EventType.ItemFinishedLoading, newItem);
				newItem.Login();
			}
		}

		var check = Gameworld.GetCheck(CheckType.SkinningCheck);
		var effect = skinner.EffectsOfType<Skinning>().First();
		foreach (var product in OriginalCharacter.Race.ButcheryProfile.Products.Where(x =>
			         x.IsPelt && x.CanProduce(skinner, Parent)))
		{
			var damageRatio =
				Parent.Wounds.Where(x => product.RequiredBodyparts.Contains(x.Bodypart)).Sum(x => x.CurrentDamage) /
				product.RequiredBodyparts.Sum(x => OriginalCharacter.Body.HitpointsForBodypart(x));
			if (double.IsNaN(damageRatio))
			{
				damageRatio = 1.0;
			}

			foreach (var item in product.ProductItems)
			{
				var result = check.Check(skinner, effect.CheckDifficulty, effect.Trait);
				if (damageRatio >= item.DamagedThreshold || result.Outcome.In(Outcome.MajorFail, Outcome.Fail))
				{
					if (damageRatio >= item.DamagedThreshold)
					{
						if (item.DamagedProto != null)
						{
							LoadItem(item.DamagedProto, item.DamagedQuantity);
							continue;
						}
					}
				}

				LoadItem(item.NormalProto, item.NormalQuantity);
			}
		}

		Skinned = true;
	}

	void ILazyLoadDuringIdleTime.DoLoad()
	{
		LoadOriginalCharacter(true);
	}

	#endregion

	#region IOverrideItemWoundBehaviour Implementation

	public IHealthStrategy HealthStrategy => OriginalCharacter.HealthStrategy;
	public IEnumerable<IWound> Wounds => OriginalCharacter.Wounds;

	public IEnumerable<IWound> VisibleWounds(IPerceiver voyeur, WoundExaminationType examinationType)
	{
		return OriginalCharacter.VisibleWounds(voyeur, examinationType);
	}

	public IEnumerable<IWound> SufferDamage(IDamage damage)
	{
		return OriginalCharacter.SufferDamage(damage);
	}

	public IEnumerable<IWound> PassiveSufferDamage(IDamage damage)
	{
		return OriginalCharacter.PassiveSufferDamage(damage);
	}

	public IEnumerable<IWound> PassiveSufferDamage(IExplosiveDamage damage, Proximity proximity, Facing facing)
	{
		return OriginalCharacter.PassiveSufferDamage(damage, proximity, facing);
	}

	public void ProcessPassiveWound(IWound wound)
	{
		OriginalCharacter.ProcessPassiveWound(wound);
	}

	public WoundSeverity GetSeverityFor(IWound wound)
	{
		return OriginalCharacter.GetSeverityFor(wound);
	}

	public double GetSeverityFloor(WoundSeverity severity, bool usePercentageModel = false)
	{
		return OriginalCharacter.GetSeverityFloor(severity, usePercentageModel);
	}

	public void EvaluateWounds()
	{
		OriginalCharacter.EvaluateWounds();
	}

	public void CureAllWounds()
	{
		OriginalCharacter.CureAllWounds();
	}

	public void StartHealthTick(bool initial = false)
	{
		OriginalCharacter.StartHealthTick(initial);
	}

	public void EndHealthTick()
	{
		OriginalCharacter.EndHealthTick();
	}

	public void AddWound(IWound wound)
	{
		OriginalCharacter.AddWound(wound);
	}

	public void AddWounds(IEnumerable<IWound> wounds)
	{
		OriginalCharacter.AddWounds(wounds);
	}

	#endregion
}
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine.Parsers;
using Chargen = MudSharp.CharacterCreation.Chargen;

namespace MudSharp.RPG.Merits.CharacterMerits;

public abstract class CharacterMeritBase : MeritBase, ICharacterMerit, IHaveFuturemud
{
	private readonly List<ChargenResourceCost> _costs = new();
	public IFutureProg ApplicabilityProg { get; private set; }

	private IFutureProg _chargenAvailableProg;

	public IFutureProg ChargenAvailableProg => _chargenAvailableProg;

	private long? _parentMeritId;
	private ICharacterMerit _parentMerit;
	private string _descriptionText;

	public string DatabaseType { get; }

	public ICharacterMerit ParentMerit
	{
		get
		{
			if (_parentMerit is null && _parentMeritId.HasValue)
			{
				_parentMerit = Gameworld.Merits.Get(_parentMeritId.Value) as ICharacterMerit;
			}

			return _parentMerit;
		}
	}

	protected CharacterMeritBase(Merit merit, IFuturemud gameworld) : base(merit)
	{
		Gameworld = gameworld;
		DatabaseType = merit.Type;
		foreach (var resource in merit.MeritsChargenResources)
		{
			_costs.Add(new ChargenResourceCost
			{
				Amount = resource.Amount,
				RequirementOnly = resource.RequirementOnly,
				Resource = gameworld.ChargenResources.Get(resource.ChargenResourceId)
			});
		}

		_parentMeritId = merit.ParentId;
		LoadFromDb(XElement.Parse(merit.Definition), gameworld);
	}

	public override bool Applies(IHaveMerits owner)
	{
		if (ParentMerit is not null)
		{
			return ParentMerit.Applies(owner);
		}

		if (owner is ICharacter ownerAsCharacter)
		{
			return Applies(ownerAsCharacter);
		}

		if (owner is IBody ownerAsBody)
		{
			return Applies(ownerAsBody.Actor);
		}

		return owner is Chargen ownerAsChargen;
	}

	public virtual bool Applies(IHaveMerits owner, IPerceivable target)
	{
		return Applies(owner);
	}

	#region Implementation of IMerit

	public override string Describe(IHaveMerits owner, IPerceiver voyeur)
	{
		if (owner is ICharacter ownerAsCharacter)
		{
			return new Emote(_descriptionText, ownerAsCharacter, ownerAsCharacter, voyeur).ParseFor(voyeur);
		}

		return owner is IBody ownerAsBody
			? new Emote(_descriptionText, ownerAsBody.Actor, ownerAsBody.Actor, voyeur).ParseFor(voyeur)
			: "";
	}

	#endregion

	private void LoadFromDb(XElement root, IFuturemud gameworld)
	{
		var element = root.Element("ChargenAvailableProg");
		if (element != null)
		{
			_chargenAvailableProg = gameworld.FutureProgs.Get(long.Parse(element.Value));
		}

		element = root.Element("ChargenBlurb");
		if (element != null)
		{
			ChargenBlurb = element.Value;
		}

		element = root.Element("ApplicabilityProg");
		if (element != null)
		{
			ApplicabilityProg = gameworld.FutureProgs.Get(long.Parse(element.Value));
		}

		element = root.Element("DescriptionText");
		if (element != null)
		{
			_descriptionText = element.Value;
		}
	}

	protected virtual bool Applies(ICharacter character)
	{
		return (bool?)ApplicabilityProg?.Execute(character) ?? true;
	}

	#region ICharacterMerit Members

	public bool ChargenAvailable(IChargen chargen)
	{
		return
			_costs.Where(x => x.RequirementOnly)
			      .All(x => chargen.Account.AccountResources.ValueOrDefault(x.Resource, 0) >= x.Amount) &&
			((bool?)_chargenAvailableProg?.Execute(chargen) ?? true);
	}

	public string ChargenBlurb { get; protected set; }

	public int ResourceCost(IChargenResource resource)
	{
		return _costs.FirstOrDefault(x => !x.RequirementOnly && x.Resource == resource)?.Amount ?? 0;
	}

	public int ResourceRequirement(IChargenResource resource)
	{
		return _costs.FirstOrDefault(x => x.RequirementOnly && x.Resource == resource)?.Amount ?? 0;
	}

	public bool DisplayInCharacterMeritsCommand(ICharacter character)
	{
		return _parentMerit == null || character.IsAdministrator();
	}

	public IFuturemud Gameworld { get; protected set; }

	#endregion
}
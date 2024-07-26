using MudSharp.Models;
using MudSharp.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class ComboMerit : CharacterMeritBase
{
	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Combo",
			(merit, gameworld) => new ComboMerit(merit, gameworld));
		MeritFactory.RegisterBuilderMeritInitialiser("Combo", (gameworld, name) => new ComboMerit(gameworld, name));
		MeritFactory.RegisterMeritHelp("Combo", "Controls multiple child quirks at once", new ComboMerit().HelpText);
	}

	public ComboMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		foreach (var element in definition.Element("Children")?.Elements() ?? Enumerable.Empty<XElement>())
		{
			_childMeritIds.Add(long.Parse(element.Value));
		}
	}

	protected ComboMerit(){}

	protected ComboMerit(IFuturemud gameworld, string name) : base(gameworld, name, "Combo", "@ have|has some combination of other quirks")
	{

	}

	/// <inheritdoc />
	protected override XElement SaveSubtypeDefinition(XElement root)
	{
		root.Add(new XElement("Children",
			from item in _childMeritIds
			select new XElement("Child", item)
		));
		return root;
	}

	/// <inheritdoc />
	protected override void SubtypeShow(ICharacter actor, StringBuilder sb)
	{
		InitialiseChildren();
		sb.AppendLine($"Child Merits:");
		sb.AppendLine();
		foreach (var item in CharacterMerits)
		{
			sb.AppendLine($"\t#{item.Id.ToString("N0", actor)}) {item.Name.ColourValue()}");
		}
	}

	private readonly List<long> _childMeritIds = new();

	private readonly List<ICharacterMerit> _characterMerits = new();

	private void InitialiseChildren()
	{
		if (!_characterMerits.Any() && _childMeritIds.Any())
		{
			_characterMerits.AddRange(_childMeritIds.SelectNotNull(x => Gameworld.Merits.Get(x) as ICharacterMerit));
		}
	}

	public IEnumerable<ICharacterMerit> CharacterMerits
	{
		get
		{
			InitialiseChildren();
			return _characterMerits;
		}
	}

	public void AddChild(ICharacterMerit merit)
	{
		InitialiseChildren();
		_childMeritIds.Add(merit.Id);
		_characterMerits.Add(merit);
		Changed = true;
	}

	public void RemoveChild(ICharacterMerit merit)
	{
		InitialiseChildren();
		_childMeritIds.Remove(merit.Id);
		_characterMerits.Remove(merit);
		Changed = true;
	}
}
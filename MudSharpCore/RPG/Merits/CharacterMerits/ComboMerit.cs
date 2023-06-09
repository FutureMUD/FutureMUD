﻿using MudSharp.Models;
using MudSharp.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class ComboMerit : CharacterMeritBase
{
	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Combo",
			(merit, gameworld) => new ComboMerit(merit, gameworld));
	}

	public ComboMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		foreach (var element in definition.Element("Children")?.Elements() ?? Enumerable.Empty<XElement>())
		{
			_childMeritIds.Add(long.Parse(element.Value));
		}
	}

	private readonly List<long> _childMeritIds = new();

	private readonly List<ICharacterMerit> _characterMerits = new();

	public IEnumerable<ICharacterMerit> CharacterMerits
	{
		get
		{
			if (!_characterMerits.Any() && _childMeritIds.Any())
			{
				_characterMerits.AddRange(
					_childMeritIds.SelectNotNull(x => Gameworld.Merits.Get(x) as ICharacterMerit));
			}

			return _characterMerits;
		}
	}
}
using System;
using System.Xml.Linq;
using MudSharp.Framework;

namespace MudSharp.CharacterCreation;

public class ChargenScreenStoryboardFactory
{
	private readonly Func<IFuturemud, Models.ChargenScreenStoryboard, IChargenScreenStoryboard> _generationFunction;
	private readonly Func<IFuturemud, IChargenScreenStoryboard, IChargenScreenStoryboard> _swapFunction;

	public ChargenScreenStoryboardFactory(string typeName,
		Func<IFuturemud, Models.ChargenScreenStoryboard, IChargenScreenStoryboard> function,
		Func<IFuturemud, IChargenScreenStoryboard, IChargenScreenStoryboard> swap = null)
	{
		_generationFunction = function;
		TypeName = typeName;
		if (swap is not null)
		{
			_swapFunction = swap;
		}
	}

	public string TypeName { get; protected set; }

	public IChargenScreenStoryboard CreateNew(IFuturemud gameworld, Models.ChargenScreenStoryboard dbitem)
	{
		return _generationFunction(gameworld, dbitem);
	}

	public IChargenScreenStoryboard CreateNew(IFuturemud gameworld, IChargenScreenStoryboard existing)
	{
		return _swapFunction(gameworld, existing);
	}
}
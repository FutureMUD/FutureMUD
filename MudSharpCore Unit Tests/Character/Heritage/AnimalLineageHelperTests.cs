#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character.Heritage;
using MudSharp.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests.Character.Heritage;

[TestClass]
public class AnimalLineageHelperTests
{
	private sealed class TestAll<T>(IEnumerable<T> values) : IUneditableAll<T> where T : class, IFrameworkItem
	{
		private readonly List<T> _values = values.ToList();

		public bool Has(T value)
		{
			return _values.Contains(value);
		}

		public bool Has(long id)
		{
			return _values.Any(x => x.Id == id);
		}

		public bool Has(string name)
		{
			return _values.Any(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
		}

		public T? Get(long id)
		{
			return _values.FirstOrDefault(x => x.Id == id);
		}

		public bool TryGet(long id, out T? result)
		{
			result = Get(id);
			return result is not null;
		}

		public List<T> Get(string name)
		{
			return _values.Where(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).ToList();
		}

		public T? GetByName(string name)
		{
			return _values.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
		}

		public T? GetByIdOrName(string value, bool permitAbbreviations = true)
		{
			return long.TryParse(value, out long id) ? Get(id) : GetByName(value);
		}

		public void ForEach(Action<T> action)
		{
			foreach (T item in _values)
			{
				action(item);
			}
		}

		public int Count => _values.Count;

		public IEnumerator<T> GetEnumerator()
		{
			return _values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	[TestMethod]
	public void IsAnimal_ReturnsTrue_WhenRaceCountsAsStockAnimalBody()
	{
		Mock<IBodyPrototype> animalFamily = CreateBodyPrototype(10L, "Quadruped Base");
		Mock<IBodyPrototype> baseBody = CreateBodyPrototype(11L, "Wolf");
		baseBody.Setup(x => x.CountsAs(animalFamily.Object)).Returns(true);

		Mock<IRace> race = new();
		race.SetupGet(x => x.BaseBody).Returns(baseBody.Object);

		Mock<IFuturemud> gameworld = new();
		gameworld.SetupGet(x => x.BodyPrototypes).Returns(new TestAll<IBodyPrototype>([animalFamily.Object]));

		Assert.IsTrue(AnimalLineageHelper.IsAnimal(race.Object, gameworld.Object));
	}

	[TestMethod]
	public void IsAnimal_ReturnsFalse_ForHumanoidRace()
	{
		Mock<IBodyPrototype> animalFamily = CreateBodyPrototype(20L, "Quadruped Base");
		Mock<IBodyPrototype> baseBody = CreateBodyPrototype(21L, "Humanoid");
		baseBody.Setup(x => x.CountsAs(animalFamily.Object)).Returns(false);

		Mock<IRace> race = new();
		race.SetupGet(x => x.BaseBody).Returns(baseBody.Object);

		Mock<IFuturemud> gameworld = new();
		gameworld.SetupGet(x => x.BodyPrototypes).Returns(new TestAll<IBodyPrototype>([animalFamily.Object]));

		Assert.IsFalse(AnimalLineageHelper.IsAnimal(race.Object, gameworld.Object));
	}

	private static Mock<IBodyPrototype> CreateBodyPrototype(long id, string name)
	{
		Mock<IBodyPrototype> mock = new();
		mock.SetupGet(x => x.Id).Returns(id);
		mock.SetupGet(x => x.Name).Returns(name);
		mock.SetupGet(x => x.FrameworkItemType).Returns("BodyPrototype");
		return mock;
	}
}

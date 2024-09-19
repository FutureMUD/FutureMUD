using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MudSharp.ThirdPartyCode;

public class Node<T> : IEqualityComparer, IEnumerable<T>, IEnumerable<Node<T>>
{
	private readonly List<Node<T>> _children = new();

	public Node(T value)
	{
		Value = value;
	}

	public Node<T> Parent { get; private set; }
	public T Value { get; set; }
	public double GScore { get; set; }

	public Node<T> this[int index] => _children[index];

	public IEnumerable<Node<T>> Ancestors =>
		IsRoot ? Enumerable.Empty<Node<T>>() : Parent.ToIEnumarable().Concat(Parent.Ancestors);

	public IEnumerable<Node<T>> Descendants => SelfAndDescendants.Skip(1);

	public IEnumerable<Node<T>> Children => _children;

	public IEnumerable<Node<T>> Siblings => SelfAndSiblings.Where(Other);

	public IEnumerable<Node<T>> SelfAndChildren => this.ToIEnumarable().Concat(Children);

	public IEnumerable<Node<T>> SelfAndAncestors => this.ToIEnumarable().Concat(Ancestors);

	public IEnumerable<Node<T>> SelfAndDescendants
	{
		get { return this.ToIEnumarable().Concat(Children.SelectMany(c => c.SelfAndDescendants)); }
	}

	public IEnumerable<Node<T>> SelfAndSiblings => IsRoot ? this.ToIEnumarable() : Parent.Children;

	public IEnumerable<Node<T>> All => Root.SelfAndDescendants;


	public IEnumerable<Node<T>> SameLevel => SelfAndSameLevel.Where(Other);

	public int Level => Ancestors.Count();

	public IEnumerable<Node<T>> SelfAndSameLevel => GetNodesAtLevel(Level);

	public Node<T> Root => SelfAndAncestors.Last();

	public bool IsRoot => Parent == null;

	public IEnumerable<Node<T>> Leaves
	{
		get { return Descendants.Where(n => !n.Children.Any()); }
	}

	public IEnumerator<Node<T>> GetEnumerator()
	{
		return _children.GetEnumerator();
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return _children.Values().GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return _children.GetEnumerator();
	}

	public Node<T> Add(T value, int index = -1)
	{
		var childNode = new Node<T>(value);
		Add(childNode, index);
		return childNode;
	}

	public void Add(Node<T> childNode, int index = -1)
	{
		if (index < -1)
		{
			throw new ArgumentException("The index can not be lower then -1");
		}

		if (index > Children.Count() - 1)
		{
			throw new ArgumentException(
				"The index ({0}) can not be higher then index of the last iten. Use the AddChild() method without an index to add at the end"
					.FormatInvariant(index));
		}

		if (!childNode.IsRoot)
		{
			throw new ArgumentException(
				"The child node with value [{0}] can not be added because it is not a root node.".FormatInvariant(
					childNode.Value));
		}

		if (Root == childNode)
		{
			throw new ArgumentException(
				"The child node with value [{0}] is the rootnode of the parent.".FormatInvariant(childNode.Value));
		}

		if (childNode.SelfAndDescendants.Any(n => this == n))
		{
			throw new ArgumentException(
				"The childnode with value [{0}] can not be added to itself or its descendants.".FormatInvariant(
					childNode.Value));
		}

		childNode.Parent = this;
		if (index == -1)
		{
			_children.Add(childNode);
		}
		else
		{
			_children.Insert(index, childNode);
		}
	}

	public Node<T> AddFirstChild(T value)
	{
		var childNode = new Node<T>(value);
		AddFirstChild(childNode);
		return childNode;
	}

	public void AddFirstChild(Node<T> childNode)
	{
		Add(childNode, 0);
	}

	public Node<T> AddFirstSibling(T value)
	{
		var childNode = new Node<T>(value);
		AddFirstSibling(childNode);
		return childNode;
	}

	public void AddFirstSibling(Node<T> childNode)
	{
		Parent.AddFirstChild(childNode);
	}

	public Node<T> AddLastSibling(T value)
	{
		var childNode = new Node<T>(value);
		AddLastSibling(childNode);
		return childNode;
	}

	public void AddLastSibling(Node<T> childNode)
	{
		Parent.Add(childNode);
	}

	public Node<T> AddParent(T value)
	{
		var newNode = new Node<T>(value);
		AddParent(newNode);
		return newNode;
	}

	public void AddParent(Node<T> parentNode)
	{
		if (!IsRoot)
		{
			throw new ArgumentException("This node [{0}] already has a parent".FormatInvariant(Value),
				nameof(parentNode));
		}

		parentNode.Add(this);
	}

	private bool Other(Node<T> node)
	{
		return !ReferenceEquals(node, this);
	}

	public IEnumerable<Node<T>> GetNodesAtLevel(int level)
	{
		return Root.GetNodesAtLevelInternal(level);
	}

	private IEnumerable<Node<T>> GetNodesAtLevelInternal(int level)
	{
		return level == Level ? this.ToIEnumarable() : Children.SelectMany(c => c.GetNodesAtLevelInternal(level));
	}

	public void Disconnect()
	{
		if (IsRoot)
		{
			throw new InvalidOperationException(
				"The root node [{0}] can not get disconnected from a parent.".FormatInvariant(Value));
		}

		Parent._children.Remove(this);
		Parent = null;
	}

	public override string ToString()
	{
		return Value.ToString();
	}

	public static IEnumerable<Node<T>> CreateTree<TId>(IEnumerable<T> values, Func<T, TId> idSelector,
		Func<T, TId?> parentIdSelector)
		where TId : struct
	{
		var valuesCache = values.ToList();
		if (!valuesCache.Any())
		{
			return Enumerable.Empty<Node<T>>();
		}

		var itemWithIdAndParentIdIsTheSame =
			valuesCache.FirstOrDefault(v => IsSameId(idSelector(v), parentIdSelector(v)));
		if (itemWithIdAndParentIdIsTheSame != null) // Hier verwacht je ook een null terug te kunnen komen
		{
			throw new ArgumentException(
				"At least one value has the samen Id and parentId [{0}]".FormatInvariant(
					itemWithIdAndParentIdIsTheSame));
		}

		var nodes = valuesCache.Select(v => new Node<T>(v));
		return CreateTree(nodes, idSelector, parentIdSelector);
	}

	public static IEnumerable<Node<T>> CreateTree<TId>(IEnumerable<Node<T>> rootNodes, Func<T, TId> idSelector,
		Func<T, TId?> parentIdSelector)
		where TId : struct
	{
		var rootNodesCache = rootNodes.ToList();
		var duplicates = rootNodesCache.Duplicates(n => n).ToList();
		if (duplicates.Any())
		{
			throw new ArgumentException(
				"One or more values contains {0} duplicate keys. The first duplicate is: [{1}]".FormatInvariant(
					duplicates.Count, duplicates[0]));
		}

		foreach (var rootNode in rootNodesCache)
		{
			var parentId = parentIdSelector(rootNode.Value);
			var parent = rootNodesCache.FirstOrDefault(n => IsSameId(idSelector(n.Value), parentId));

			if (parent != null)
			{
				parent.Add(rootNode);
			}
			else if (parentId != null)
			{
				throw new ArgumentException(
					"A value has the parent ID [{0}] but no other nodes has this ID".FormatInvariant(parentId.Value));
			}
		}

		var result = rootNodesCache.Where(n => n.IsRoot);
		return result;
	}

	private static bool IsSameId<TId>(TId id, TId? parentId)
		where TId : struct
	{
		return parentId != null && id.Equals(parentId.Value);
	}

	#region Equals en ==

	public static bool operator ==(Node<T> value1, Node<T> value2)
	{
		if ((object)value1 == null && (object)value2 == null)
		{
			return true;
		}

		return ReferenceEquals(value1, value2);
	}

	public static bool operator !=(Node<T> value1, Node<T> value2)
	{
		return !(value1 == value2);
	}

	public override bool Equals(object anderePeriode)
	{
		var valueThisType = anderePeriode as Node<T>;
		return this == valueThisType;
	}

	public bool Equals(Node<T> value)
	{
		return this == value;
	}

	public bool Equals(Node<T> value1, Node<T> value2)
	{
		return value1 == value2;
	}

	bool IEqualityComparer.Equals(object value1, object value2)
	{
		var valueThisType1 = value1 as Node<T>;
		var valueThisType2 = value2 as Node<T>;

		return Equals(valueThisType1, valueThisType2);
	}

	public int GetHashCode(object obj)
	{
		return GetHashCode(obj as Node<T>);
	}

	public override int GetHashCode()
	{
		return GetHashCode(this);
	}

	public int GetHashCode(Node<T> value)
	{
		return base.GetHashCode();
	}

	#endregion
}

public class PolyNode<T> : IEqualityComparer, IEnumerable<T>, IEnumerable<PolyNode<T>>
{
	private readonly List<PolyNode<T>> _children = new();
	private readonly List<PolyNode<T>> _parents = new();

	public PolyNode(T value)
	{
		Value = value;
	}

	public T Value { get; set; }
	public double GScore { get; set; }

	public PolyNode<T> this[int index] => _children[index];

	public IEnumerable<PolyNode<T>> Ancestors =>
		IsRoot ? Enumerable.Empty<PolyNode<T>>() : _parents.SelectMany(x => x.SelfAndAncestors).Distinct();

	public IEnumerable<PolyNode<T>> Descendants => SelfAndDescendants.Skip(1);

	public IEnumerable<PolyNode<T>> Children => _children;

	public IEnumerable<PolyNode<T>> SelfAndChildren => this.ToIEnumarable().Concat(Children);

	public IEnumerable<PolyNode<T>> SelfAndAncestors => this.ToIEnumarable().Concat(Ancestors);

	public IEnumerable<PolyNode<T>> SelfAndDescendants =>
		this.ToIEnumarable().Concat(Children.SelectMany(c => c.SelfAndDescendants));

	public IEnumerable<PolyNode<T>> All => Root.SelfAndDescendants;


	public IEnumerable<PolyNode<T>> SameLevel => SelfAndSameLevel.Where(Other);

	public int Level => Ancestors.Count();

	public IEnumerable<PolyNode<T>> SelfAndSameLevel => GetNodesAtLevel(Level);

	public PolyNode<T> Root => SelfAndAncestors.Last();

	public bool IsRoot => !_parents.Any();

	public IEnumerable<PolyNode<T>> Leaves
	{
		get { return Descendants.Where(n => !n.Children.Any()); }
	}

	public IEnumerator<PolyNode<T>> GetEnumerator()
	{
		return _children.GetEnumerator();
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return _children.Values().GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return _children.GetEnumerator();
	}

	public PolyNode<T> Add(T value, int index = -1)
	{
		var childNode = new PolyNode<T>(value);
		Add(childNode, index);
		return childNode;
	}

	public void Add(PolyNode<T> childNode, int index = -1)
	{
		if (index < -1)
		{
			throw new ArgumentException("The index can not be lower then -1");
		}

		if (index > Children.Count() - 1)
		{
			throw new ArgumentException(
				"The index ({0}) can not be higher then index of the last iten. Use the AddChild() method without an index to add at the end"
					.FormatInvariant(index));
		}

		if (Root == childNode)
		{
			throw new ArgumentException(
				"The child node with value [{0}] is the rootnode of the parent.".FormatInvariant(childNode.Value));
		}

		if (childNode.SelfAndDescendants.Any(n => this == n))
		{
			throw new ArgumentException(
				"The childnode with value [{0}] can not be added to itself or its descendants.".FormatInvariant(
					childNode.Value));
		}

		childNode._parents.Add(this);
		if (index == -1)
		{
			_children.Add(childNode);
		}
		else
		{
			_children.Insert(index, childNode);
		}
	}

	public PolyNode<T> AddFirstChild(T value)
	{
		var childNode = new PolyNode<T>(value);
		AddFirstChild(childNode);
		return childNode;
	}

	public void AddFirstChild(PolyNode<T> childNode)
	{
		Add(childNode, 0);
	}

	public PolyNode<T> AddParent(T value)
	{
		var newNode = new PolyNode<T>(value);
		AddParent(newNode);
		return newNode;
	}

	public void AddParent(PolyNode<T> parentNode)
	{
		parentNode.Add(this);
	}

	private bool Other(PolyNode<T> node)
	{
		return !ReferenceEquals(node, this);
	}

	public IEnumerable<PolyNode<T>> GetNodesAtLevel(int level)
	{
		return Root.GetNodesAtLevelInternal(level);
	}

	private IEnumerable<PolyNode<T>> GetNodesAtLevelInternal(int level)
	{
		return level == Level ? this.ToIEnumarable() : Children.SelectMany(c => c.GetNodesAtLevelInternal(level));
	}

	public void Disconnect()
	{
		if (IsRoot)
		{
			throw new InvalidOperationException(
				"The root node [{0}] can not get disconnected from a parent.".FormatInvariant(Value));
		}

		foreach (var parent in _parents)
		{
			parent._children.Remove(this);
		}

		_parents.Clear();
	}

	public override string ToString()
	{
		return Value.ToString();
	}

	public static IEnumerable<PolyNode<T>> CreateTree<TId>(IEnumerable<T> values, Func<T, TId> idSelector,
		Func<T, TId?> parentIdSelector)
		where TId : struct
	{
		var valuesCache = values.ToList();
		if (!valuesCache.Any())
		{
			return Enumerable.Empty<PolyNode<T>>();
		}

		var itemWithIdAndParentIdIsTheSame =
			valuesCache.FirstOrDefault(v => IsSameId(idSelector(v), parentIdSelector(v)));
		if (itemWithIdAndParentIdIsTheSame != null) // Hier verwacht je ook een null terug te kunnen komen
		{
			throw new ArgumentException(
				"At least one value has the same Id and parentId [{0}]".FormatInvariant(
					itemWithIdAndParentIdIsTheSame));
		}

		var nodes = valuesCache.Select(v => new PolyNode<T>(v));
		return CreateTree(nodes, idSelector, parentIdSelector);
	}

	public static IEnumerable<PolyNode<T>> CreateTree<TId>(IEnumerable<PolyNode<T>> rootNodes,
		Func<T, TId> idSelector, Func<T, TId?> parentIdSelector)
		where TId : struct
	{
		var rootNodesCache = rootNodes.ToList();
		var duplicates = rootNodesCache.Duplicates(n => n).ToList();
		if (duplicates.Any())
		{
			throw new ArgumentException(
				"One or more values contains {0} duplicate keys. The first duplicate is: [{1}]".FormatInvariant(
					duplicates.Count, duplicates[0]));
		}

		foreach (var rootNode in rootNodesCache)
		{
			var parentId = parentIdSelector(rootNode.Value);
			var parent = rootNodesCache.FirstOrDefault(n => IsSameId(idSelector(n.Value), parentId));

			if (parent != null)
			{
				parent.Add(rootNode);
			}
			else if (parentId != null)
			{
				throw new ArgumentException(
					"A value has the parent ID [{0}] but no other nodes has this ID".FormatInvariant(parentId.Value));
			}
		}

		var result = rootNodesCache.Where(n => n.IsRoot);
		return result;
	}

	private static bool IsSameId<TId>(TId id, TId? parentId)
		where TId : struct
	{
		return parentId != null && id.Equals(parentId.Value);
	}

	#region Equals en ==

	public static bool operator ==(PolyNode<T> value1, PolyNode<T> value2)
	{
		if ((object)value1 == null && (object)value2 == null)
		{
			return true;
		}

		return ReferenceEquals(value1, value2);
	}

	public static bool operator !=(PolyNode<T> value1, PolyNode<T> value2)
	{
		return !(value1 == value2);
	}

	public override bool Equals(object anderePeriode)
	{
		var valueThisType = anderePeriode as PolyNode<T>;
		return this == valueThisType;
	}

	public bool Equals(PolyNode<T> value)
	{
		return this == value;
	}

	public bool Equals(PolyNode<T> value1, PolyNode<T> value2)
	{
		return value1 == value2;
	}

	bool IEqualityComparer.Equals(object value1, object value2)
	{
		var valueThisType1 = value1 as PolyNode<T>;
		var valueThisType2 = value2 as PolyNode<T>;

		return Equals(valueThisType1, valueThisType2);
	}

	public int GetHashCode(object obj)
	{
		return GetHashCode(obj as PolyNode<T>);
	}

	public override int GetHashCode()
	{
		return GetHashCode(this);
	}

	public int GetHashCode(PolyNode<T> value)
	{
		return base.GetHashCode();
	}

	#endregion
}

public static class NodeExtensions
{
	public static IEnumerable<T> Values<T>(this IEnumerable<Node<T>> nodes)
	{
		return nodes.Select(n => n.Value);
	}

	public static IEnumerable<T> Values<T>(this IEnumerable<PolyNode<T>> nodes)
	{
		return nodes.Select(n => n.Value);
	}
}

public static class OtherExtensions
{
	public static IEnumerable<TSource> Duplicates<TSource, TKey>(this IEnumerable<TSource> source,
		Func<TSource, TKey> selector)
	{
		var grouped = source.GroupBy(selector);
		var moreThen1 = grouped.Where(i => i.IsMultiple());

		return moreThen1.SelectMany(i => i);
	}

	public static bool IsMultiple<T>(this IEnumerable<T> source)
	{
		using var enumerator = source.GetEnumerator();
		return enumerator.MoveNext() && enumerator.MoveNext();
	}

	public static IEnumerable<T> ToIEnumarable<T>(this T item)
	{
		yield return item;
	}

	public static string FormatInvariant(this string text, params object[] parameters)
	{
		// This is not the "real" implementation, but that would go out of Scope
		return string.Format(CultureInfo.InvariantCulture, text, parameters);
	}
}
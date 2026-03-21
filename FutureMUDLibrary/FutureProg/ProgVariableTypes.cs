#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using MudSharp.Framework;

namespace MudSharp.FutureProg;

public readonly struct ProgVariableTypes : IEquatable<ProgVariableTypes>
{
	private const string StoragePrefix = "v1:";
	private readonly BigInteger _mask;

	internal ProgVariableTypes(BigInteger mask)
	{
		_mask = mask;
	}

	internal BigInteger Mask => _mask;

	public static readonly ProgVariableTypes Void = new(BigInteger.Zero);
	public static readonly ProgVariableTypes Text = FromLegacyBitIndex(0);
	public static readonly ProgVariableTypes Number = FromLegacyBitIndex(1);
	public static readonly ProgVariableTypes Boolean = FromLegacyBitIndex(2);
	public static readonly ProgVariableTypes Character = FromLegacyBitIndex(3);
	public static readonly ProgVariableTypes Location = FromLegacyBitIndex(4);
	public static readonly ProgVariableTypes Item = FromLegacyBitIndex(5);
	public static readonly ProgVariableTypes Shard = FromLegacyBitIndex(6);
	public static readonly ProgVariableTypes Error = FromLegacyBitIndex(7);
	public static readonly ProgVariableTypes Gender = FromLegacyBitIndex(8);
	public static readonly ProgVariableTypes Zone = FromLegacyBitIndex(9);
	public static readonly ProgVariableTypes Collection = FromLegacyBitIndex(10);
	public static readonly ProgVariableTypes Race = FromLegacyBitIndex(11);
	public static readonly ProgVariableTypes Culture = FromLegacyBitIndex(12);
	public static readonly ProgVariableTypes Chargen = FromLegacyBitIndex(13);
	public static readonly ProgVariableTypes Trait = FromLegacyBitIndex(14);
	public static readonly ProgVariableTypes Clan = FromLegacyBitIndex(15);
	public static readonly ProgVariableTypes ClanRank = FromLegacyBitIndex(16);
	public static readonly ProgVariableTypes ClanAppointment = FromLegacyBitIndex(17);
	public static readonly ProgVariableTypes ClanPaygrade = FromLegacyBitIndex(18);
	public static readonly ProgVariableTypes Currency = FromLegacyBitIndex(19);
	public static readonly ProgVariableTypes Exit = FromLegacyBitIndex(20);
	public static readonly ProgVariableTypes Literal = FromLegacyBitIndex(21);
	public static readonly ProgVariableTypes DateTime = FromLegacyBitIndex(22);
	public static readonly ProgVariableTypes TimeSpan = FromLegacyBitIndex(23);
	public static readonly ProgVariableTypes Language = FromLegacyBitIndex(24);
	public static readonly ProgVariableTypes Accent = FromLegacyBitIndex(25);
	public static readonly ProgVariableTypes Merit = FromLegacyBitIndex(26);
	public static readonly ProgVariableTypes MudDateTime = FromLegacyBitIndex(27);
	public static readonly ProgVariableTypes Calendar = FromLegacyBitIndex(28);
	public static readonly ProgVariableTypes Clock = FromLegacyBitIndex(29);
	public static readonly ProgVariableTypes Effect = FromLegacyBitIndex(30);
	public static readonly ProgVariableTypes Knowledge = FromLegacyBitIndex(31);
	public static readonly ProgVariableTypes Role = FromLegacyBitIndex(32);
	public static readonly ProgVariableTypes Ethnicity = FromLegacyBitIndex(33);
	public static readonly ProgVariableTypes Drug = FromLegacyBitIndex(34);
	public static readonly ProgVariableTypes WeatherEvent = FromLegacyBitIndex(35);
	public static readonly ProgVariableTypes Shop = FromLegacyBitIndex(36);
	public static readonly ProgVariableTypes Merchandise = FromLegacyBitIndex(37);
	public static readonly ProgVariableTypes Outfit = FromLegacyBitIndex(38);
	public static readonly ProgVariableTypes OutfitItem = FromLegacyBitIndex(39);
	public static readonly ProgVariableTypes Project = FromLegacyBitIndex(40);
	public static readonly ProgVariableTypes OverlayPackage = FromLegacyBitIndex(41);
	public static readonly ProgVariableTypes Terrain = FromLegacyBitIndex(42);
	public static readonly ProgVariableTypes Solid = FromLegacyBitIndex(43);
	public static readonly ProgVariableTypes Liquid = FromLegacyBitIndex(44);
	public static readonly ProgVariableTypes Gas = FromLegacyBitIndex(45);
	public static readonly ProgVariableTypes Dictionary = FromLegacyBitIndex(46);
	public static readonly ProgVariableTypes CollectionDictionary = FromLegacyBitIndex(47);
	public static readonly ProgVariableTypes MagicSpell = FromLegacyBitIndex(48);
	public static readonly ProgVariableTypes MagicSchool = FromLegacyBitIndex(49);
	public static readonly ProgVariableTypes MagicCapability = FromLegacyBitIndex(50);
	public static readonly ProgVariableTypes Bank = FromLegacyBitIndex(51);
	public static readonly ProgVariableTypes BankAccount = FromLegacyBitIndex(52);
	public static readonly ProgVariableTypes BankAccountType = FromLegacyBitIndex(53);
	public static readonly ProgVariableTypes LegalAuthority = FromLegacyBitIndex(54);
	public static readonly ProgVariableTypes Law = FromLegacyBitIndex(55);
	public static readonly ProgVariableTypes Crime = FromLegacyBitIndex(56);
	public static readonly ProgVariableTypes Market = FromLegacyBitIndex(57);
	public static readonly ProgVariableTypes MarketCategory = FromLegacyBitIndex(58);
	public static readonly ProgVariableTypes LiquidMixture = FromLegacyBitIndex(59);
	public static readonly ProgVariableTypes Script = FromLegacyBitIndex(60);
	public static readonly ProgVariableTypes Writing = FromLegacyBitIndex(61);
	public static readonly ProgVariableTypes Area = FromLegacyBitIndex(62);

	public static readonly ProgVariableTypes CollectionItem =
		Number | Boolean | Gender | Text | DateTime | TimeSpan | Character | Item | Chargen | Location | Zone |
		Shard | Accent | Language | Race | Culture | Trait | Clan | ClanRank | ClanAppointment | ClanPaygrade |
		Currency | Exit | Merit | MudDateTime | Calendar | Clock | Effect | Knowledge | Role | Ethnicity | Drug |
		WeatherEvent | Shop | Merchandise | Outfit | OutfitItem | OverlayPackage | Terrain | Project | Solid |
		Liquid | Gas | MagicSchool | MagicCapability | MagicSpell | Bank | BankAccount | BankAccountType |
		LegalAuthority | Law | Crime | Market | MarketCategory | LiquidMixture | Script | Writing | Area;

	public static readonly ProgVariableTypes Perceivable = Item | Character | Location | Zone | Shard;
	public static readonly ProgVariableTypes Perceiver = Item | Character;
	public static readonly ProgVariableTypes MagicResourceHaver = Item | Character | Location;
	public static readonly ProgVariableTypes ValueType = Text | Number | Boolean | DateTime | TimeSpan | Literal | Gender | MudDateTime | LiquidMixture;
	public static readonly ProgVariableTypes ReferenceType = CollectionItem ^ (ValueType ^ Literal);
	public static readonly ProgVariableTypes Anything = CollectionItem | ValueType | Collection | Dictionary | CollectionDictionary | Literal;
	public static readonly ProgVariableTypes Toon = Character | Chargen;
	public static readonly ProgVariableTypes Tagged = Location | Item | Terrain;
	public static readonly ProgVariableTypes Material = Solid | Gas | Liquid;

	internal static ProgVariableTypes FromLegacyBitIndex(int bitIndex)
	{
		return new ProgVariableTypes(BigInteger.One << bitIndex);
	}

	public ProgTypeKind ExactKind => ProgVariableTypeRegistry.GetExactKind(WithoutLiteral());

	public ProgTypeKind ElementKind => ProgVariableTypeRegistry.GetExactKind(WithoutContainerModifiers().WithoutLiteral());

	public ProgVariableTypeCode LegacyCode => TryGetLegacyCode(out var code) ? code : ProgVariableTypeCode.Unknown;

	public bool IsCollection => HasFlag(Collection);

	public bool IsDictionary => HasFlag(Dictionary);

	public bool IsCollectionDictionary => HasFlag(CollectionDictionary);

	public bool IsLiteral => HasFlag(Literal);

	public bool IsExactType
	{
		get
		{
			if (IsCollection || IsDictionary || IsCollectionDictionary)
			{
				return false;
			}

			return ExactKind != ProgTypeKind.Unknown;
		}
	}

	public bool HasFlag(ProgVariableTypes other)
	{
		if (other._mask.IsZero)
		{
			return true;
		}

		return (_mask & other._mask) == other._mask;
	}

	public bool CompatibleWith(ProgVariableTypes other)
	{
		return FutureProgVariableComparer.Instance.Equals(other, this);
	}

	public ProgVariableTypes WithoutLiteral()
	{
		return HasFlag(Literal) ? this & ~Literal : this;
	}

	public ProgVariableTypes WithoutContainerModifiers()
	{
		var value = this;
		if (value.HasFlag(Collection))
		{
			value ^= Collection;
		}

		if (value.HasFlag(Dictionary))
		{
			value ^= Dictionary;
		}

		if (value.HasFlag(CollectionDictionary))
		{
			value ^= CollectionDictionary;
		}

		return value;
	}

	public ProgVariableTypes WithoutModifiers()
	{
		return WithoutContainerModifiers().WithoutLiteral();
	}

	public string Describe()
	{
		return ProgVariableTypeRegistry.Describe(this);
	}

	public string DescribeEnum(bool explodeCamelCase = false, ANSIColour? colour = null)
	{
		var text = Describe();
		text = explodeCamelCase ? text.SplitCamelCase() : text;
		return colour is null ? text : text.Colour(colour);
	}

	public IEnumerable<ProgVariableTypes> GetAllFlags()
	{
		return ProgVariableTypeRegistry.GetAllFlags(this);
	}

	public IEnumerable<ProgVariableTypes> GetFlags()
	{
		return ProgVariableTypeRegistry.GetFlags(this);
	}

	public IEnumerable<ProgVariableTypes> GetSingleFlags()
	{
		return GetFlags();
	}

	public string ToStorageString()
	{
		if (_mask.IsZero)
		{
			return $"{StoragePrefix}0";
		}

		var bytes = _mask.ToByteArray(isUnsigned: true, isBigEndian: true);
		var hex = Convert.ToHexString(bytes).TrimStart('0').ToLowerInvariant();
		if (string.IsNullOrEmpty(hex))
		{
			hex = "0";
		}

		return $"{StoragePrefix}{hex}";
	}

	public bool IsLegacyRepresentable()
	{
		return _mask >= BigInteger.Zero && _mask <= long.MaxValue;
	}

	public bool TryGetLegacyCode(out ProgVariableTypeCode code)
	{
		if (IsLegacyRepresentable())
		{
			var raw = (long)_mask;
			if (Enum.IsDefined(typeof(ProgVariableTypeCode), raw))
			{
				code = (ProgVariableTypeCode)raw;
				return true;
			}
		}

		code = ProgVariableTypeCode.Unknown;
		return false;
	}

	public long ToLegacyLong()
	{
		if (!IsLegacyRepresentable())
		{
			throw new OverflowException($"The prog variable type {Describe()} cannot be represented by a legacy 64-bit value.");
		}

		return (long)_mask;
	}

	public override string ToString()
	{
		return Describe();
	}

	public bool Equals(ProgVariableTypes other)
	{
		return _mask == other._mask;
	}

	public override bool Equals(object? obj)
	{
		return obj is ProgVariableTypes other && Equals(other);
	}

	public override int GetHashCode()
	{
		return _mask.GetHashCode();
	}

	public static ProgVariableTypes FromStorageString(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			return Void;
		}

		if (TryParse(value, out var parsed))
		{
			return parsed;
		}

		throw new FormatException($"The value \"{value}\" is not a valid prog variable type definition.");
	}

	public static ProgVariableTypes FromLegacyLong(long value)
	{
		return new ProgVariableTypes(new BigInteger(value));
	}

	public static bool TryParse(string? value, out ProgVariableTypes type)
	{
		return ProgVariableTypeRegistry.TryParse(value, out type);
	}

	public static explicit operator long(ProgVariableTypes value)
	{
		return value.ToLegacyLong();
	}

	public static explicit operator int(ProgVariableTypes value)
	{
		return checked((int)value.ToLegacyLong());
	}

	public static explicit operator ProgVariableTypes(long value)
	{
		return FromLegacyLong(value);
	}

	public static explicit operator ProgVariableTypes(int value)
	{
		return FromLegacyLong(value);
	}

	public static bool operator ==(ProgVariableTypes left, ProgVariableTypes right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(ProgVariableTypes left, ProgVariableTypes right)
	{
		return !left.Equals(right);
	}

	public static ProgVariableTypes operator |(ProgVariableTypes left, ProgVariableTypes right)
	{
		return new ProgVariableTypes(left._mask | right._mask);
	}

	public static ProgVariableTypes operator &(ProgVariableTypes left, ProgVariableTypes right)
	{
		return new ProgVariableTypes(left._mask & right._mask);
	}

	public static ProgVariableTypes operator ^(ProgVariableTypes left, ProgVariableTypes right)
	{
		return new ProgVariableTypes(left._mask ^ right._mask);
	}

	public static ProgVariableTypes operator ~(ProgVariableTypes value)
	{
		return new ProgVariableTypes(ProgVariableTypeRegistry.KnownMask ^ value._mask);
	}
}

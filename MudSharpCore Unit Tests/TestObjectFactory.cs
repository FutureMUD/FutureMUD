#nullable enable

using System;
using System.Runtime.CompilerServices;

namespace MudSharp_Unit_Tests;

internal static class TestObjectFactory
{
	public static T CreateUninitialized<T>() where T : class
	{
		return (T)RuntimeHelpers.GetUninitializedObject(typeof(T));
	}
}

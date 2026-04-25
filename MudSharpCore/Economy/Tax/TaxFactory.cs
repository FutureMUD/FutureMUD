using JetBrains.Annotations;
using MudSharp.Framework;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MudSharp.Economy.Tax;

public static class TaxFactory
{
    private static Dictionary<string, Func<EconomicZoneTax, IEconomicZone, ISalesTax>> _salesTaxTypes =
        new(StringComparer.InvariantCultureIgnoreCase);

    private static Dictionary<string, Func<EconomicZoneTax, IEconomicZone, IProfitTax>> _profitTaxTypes =
        new(StringComparer.InvariantCultureIgnoreCase);

    private static Dictionary<string, Func<EconomicZoneTax, IEconomicZone, IHotelTax>> _hotelTaxTypes =
        new(StringComparer.InvariantCultureIgnoreCase);

    private static Dictionary<string, Func<string, IEconomicZone, ISalesTax>> _salesTaxBuilderInitialisers = new();
    private static Dictionary<string, Func<string, IEconomicZone, IProfitTax>> _profitTaxBuilderInitialisers = new();
    private static Dictionary<string, Func<string, IEconomicZone, IHotelTax>> _hotelTaxBuilderInitialisers = new();


    public static void RegisterSalesTax(string typeName,
        Func<EconomicZoneTax, IEconomicZone, ISalesTax> constructor,
        Func<string, IEconomicZone, ISalesTax> builderConstructor)
    {
        _salesTaxTypes[typeName] = constructor;
        _salesTaxBuilderInitialisers[typeName] = builderConstructor;
    }

    public static void RegisterProfitTax(string typeName,
        Func<EconomicZoneTax, IEconomicZone, IProfitTax> constructor,
        Func<string, IEconomicZone, IProfitTax> builderConstructor)
    {
        _profitTaxTypes[typeName] = constructor;
        _profitTaxBuilderInitialisers[typeName] = builderConstructor;
    }

    public static void RegisterHotelTax(string typeName,
        Func<EconomicZoneTax, IEconomicZone, IHotelTax> constructor,
        Func<string, IEconomicZone, IHotelTax> builderConstructor)
    {
        _hotelTaxTypes[typeName] = constructor;
        _hotelTaxBuilderInitialisers[typeName] = builderConstructor;
    }

    private static bool _initialised = false;

    private static void RegisterAllTaxes()
    {
        Type fpType = typeof(ISalesTax);
        foreach (
            Type type in Futuremud.GetAllTypes().Where(x => x.GetInterfaces().Contains(fpType)))
        {
            MethodInfo method = type.GetMethod("RegisterFactory", BindingFlags.Public | BindingFlags.Static);
            method?.Invoke(null, null);
        }

        fpType = typeof(IProfitTax);
        foreach (
            Type type in Futuremud.GetAllTypes().Where(x => x.GetInterfaces().Contains(fpType)))
        {
            MethodInfo method = type.GetMethod("RegisterFactory", BindingFlags.Public | BindingFlags.Static);
            method?.Invoke(null, null);
        }

        fpType = typeof(IHotelTax);
        foreach (
            Type type in Futuremud.GetAllTypes().Where(x => x.GetInterfaces().Contains(fpType)))
        {
            MethodInfo method = type.GetMethod("RegisterFactory", BindingFlags.Public | BindingFlags.Static);
            method?.Invoke(null, null);
        }

        _initialised = true;
    }

    [CanBeNull]
    public static ISalesTax LoadSalesTax(EconomicZoneTax tax, IEconomicZone zone)
    {
        if (!_initialised)
        {
            RegisterAllTaxes();
        }

        if (_salesTaxTypes.ContainsKey(tax.TaxType))
        {
            return _salesTaxTypes[tax.TaxType](tax, zone);
        }

        return null;
    }

    [CanBeNull]
    public static IProfitTax LoadProfitTax(EconomicZoneTax tax, IEconomicZone zone)
    {
        if (!_initialised)
        {
            RegisterAllTaxes();
        }

        if (_profitTaxTypes.ContainsKey(tax.TaxType))
        {
            return _profitTaxTypes[tax.TaxType](tax, zone);
        }

        return null;
    }

    [CanBeNull]
    public static IHotelTax LoadHotelTax(EconomicZoneTax tax, IEconomicZone zone)
    {
        if (!_initialised)
        {
            RegisterAllTaxes();
        }

        if (_hotelTaxTypes.ContainsKey(tax.TaxType))
        {
            return _hotelTaxTypes[tax.TaxType](tax, zone);
        }

        return null;
    }

    [CanBeNull]
    public static ISalesTax CreateSalesTax(string type, string name, IEconomicZone zone)
    {
        if (!_initialised)
        {
            RegisterAllTaxes();
        }

        if (_salesTaxBuilderInitialisers.ContainsKey(type))
        {
            return _salesTaxBuilderInitialisers[type](name, zone);
        }

        return null;
    }

    [CanBeNull]
    public static IProfitTax CreateProfitTax(string type, string name, IEconomicZone zone)
    {
        if (!_initialised)
        {
            RegisterAllTaxes();
        }

        if (_profitTaxBuilderInitialisers.ContainsKey(type))
        {
            return _profitTaxBuilderInitialisers[type](name, zone);
        }

        return null;
    }

    [CanBeNull]
    public static IHotelTax CreateHotelTax(string type, string name, IEconomicZone zone)
    {
        if (!_initialised)
        {
            RegisterAllTaxes();
        }

        if (_hotelTaxBuilderInitialisers.ContainsKey(type))
        {
            return _hotelTaxBuilderInitialisers[type](name, zone);
        }

        return null;
    }

    public static bool IsSalesTax(EconomicZoneTax tax)
    {
        if (!_initialised)
        {
            RegisterAllTaxes();
        }

        return _salesTaxTypes.ContainsKey(tax.TaxType);
    }

    public static bool IsHotelTax(EconomicZoneTax tax)
    {
        if (!_initialised)
        {
            RegisterAllTaxes();
        }

        return _hotelTaxTypes.ContainsKey(tax.TaxType);
    }

    public static IEnumerable<string> SalesTaxes
    {
        get
        {
            if (!_initialised)
            {
                RegisterAllTaxes();
            }

            return _salesTaxTypes.Keys.ToList();
        }
    }

    public static IEnumerable<string> ProfitTaxes
    {
        get
        {
            if (!_initialised)
            {
                RegisterAllTaxes();
            }

            return _profitTaxTypes.Keys.ToList();
        }
    }

    public static IEnumerable<string> HotelTaxes
    {
        get
        {
            if (!_initialised)
            {
                RegisterAllTaxes();
            }

            return _hotelTaxTypes.Keys.ToList();
        }
    }
}

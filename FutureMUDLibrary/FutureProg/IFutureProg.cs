using MudSharp.Framework;
using MudSharp.Framework.Save;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MudSharp.FutureProg
{
    public interface IFutureProg : IFrameworkItem, ISaveable
    {
        /// <summary>
        ///     The name of the program, used to invoke it. Must be unique for each variable space pattern.
        /// </summary>
        string FunctionName { get; set; }

        /// <summary>
        ///     An optional comment from the creator of the FutureProg about the operations or purpose of the program
        /// </summary>
        string FunctionComment { get; set; }

        /// <summary>
        ///     The original uncompiled text of the program.
        /// </summary>
        string FunctionText { get; set; }

        /// <summary>
        ///     The category to which this program belongs.
        /// </summary>
        string Category { get; set; }

        /// <summary>
        ///     The subcategory within the category to which this program belongs.
        /// </summary>
        string Subcategory { get; set; }

        /// <summary>
        ///     The original uncompiled text of the program coloured by the lexer
        /// </summary>
        string ColourisedFunctionText { get; }

        /// <summary>
        ///     If the program fails to compile, this will contain the error message.
        /// </summary>
        string CompileError { get; }

        /// <summary>
        ///     Whether or not the futureProg should be visible to those who are not administrators
        /// </summary>
        bool Public { get; set; }

        /// <summary>
        ///     An ordered collection of the parameters required to be used to invoke this program
        /// </summary>
        IEnumerable<ProgVariableTypes> Parameters { get; }

        List<Tuple<ProgVariableTypes, string>> NamedParameters { get; }

        bool AcceptsAnyParameters { get; set; }

        /// <summary>
        ///     The return type of this program
        /// </summary>
        ProgVariableTypes ReturnType { get; set; }

        /// <summary>
        ///     The time that the program took to compile
        /// </summary>
        TimeSpan CompileTime { get; }

        bool MatchesParameters(IEnumerable<ProgVariableTypes> parameters);

        /// <summary>
        ///     Executes the program and returns the result
        /// </summary>
        /// <param name="variables">The parameters to be passed in to execute the program</param>
        /// <returns>An object of the ReturnType of the program, or null in the case of an error</returns>
        object Execute(params object[] variables);
        object ExecuteWithRecursionProtection(params object[] variables);
        T Execute<T>(params object[] variables);

        double ExecuteDouble(params object[] variables);
        double ExecuteDouble(double defaultIfNull, params object[] variables);
        decimal ExecuteDecimal(params object[] variables);
        decimal ExecuteDecimal(decimal defaultIfNull, params object[] variables);
        int ExecuteInt(params object[] variables);
        int ExecuteInt(int defaultIfNull, params object[] variables);
        long ExecuteLong(params object[] variables);
        long ExecuteLong(long defaultIfNull, params object[] variables);
        bool ExecuteBool(params object[] variables);
        bool ExecuteBool(bool defaultIfNull, params object[] variables);
        string ExecuteString(params object[] variables);
        IEnumerable<T> ExecuteCollection<T>(params object[] variables);
        IReadOnlyDictionary<string, T> ExecuteDictionary<T>(params object[] variables);
        IReadOnlyCollectionDictionary<string, T> ExecuteCollectionDictionary<T>(params object[] variables);

        bool Compile();

        string MXPClickableFunctionName();
        string MXPClickableFunctionNameWithId();
        void ColouriseFunctionText();

        FutureProgStaticType StaticType { get; set; }
    }

    public enum FutureProgStaticType
    {
        NotStatic,
        StaticByParameters,
        FullyStatic
    }

    public static class FutureProgExtensions
    {
        public static bool CompatibleWith(this IEnumerable<ProgVariableTypes> parameters,
            IEnumerable<ProgVariableTypes> referenceParameters)
        {
            if (referenceParameters.Count() != parameters.Count())
            {
                return false;
            }

            for (int i = 0; i < referenceParameters.Count(); i++)
            {
                if (!parameters.ElementAt(i).CompatibleWith(referenceParameters.ElementAt(i)))
                {
                    return false;
                }
            }

            return true;
        }

        public static string DescribeParameters(this IFutureProg prog)
        {
            return prog.NamedParameters.Select(x => $"{x.Item1.Describe()}").ListToString(conjunction: "");
        }

        public static string Describe(this ProgVariableTypes type)
        {
            return ProgVariableTypeRegistry.Describe(type);
        }
    }
}

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using FutureMUD_Analyzers;
using FutureMUD_Analyzers.Test.Helpers;

namespace FutureMUD_Analyzers.Test
{
    [TestClass]
    public class UnitTest : CodeFixVerifier
    {
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new FutureProgStatementCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new FutureProgStatementDiagnosticAnalyzer();
        }
    }
}
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SwifterSharp.Analyzers.Analysis
{
    internal static class SyntaxExtensions
    {
        internal static bool IsNamed(this ArgumentSyntax argumentSyntax)
        {
            return argumentSyntax.NameColon != null;
        }
    }
}

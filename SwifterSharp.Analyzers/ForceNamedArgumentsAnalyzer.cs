using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using SwifterSharp.Analyzers.Analysis;

namespace SwifterSharp.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ForceNamedArgumentsAnalyzer : DiagnosticAnalyzer
    {
        private static readonly DiagnosticDescriptor _descriptor = Descriptors.SwifterSharp1000ArgumentsMustBeNamed;
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(_descriptor);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.RegisterCompilationStartAction(compilationContext => AnalyzeCompilation(compilationContext, new SwifterSharpContext(compilationContext.Compilation)));
        }

        private void AnalyzeCompilation(CompilationStartAnalysisContext compilationStartContext, SwifterSharpContext context)
        {
            compilationStartContext.RegisterSyntaxNodeAction(syntaxNodeContext => AnalyzeSyntaxNode(syntaxNodeContext, context), SyntaxKind.InvocationExpression);
        }

        private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext syntaxContext, SwifterSharpContext swifterSharpContext)
        {
            var invocation = (InvocationExpressionSyntax)syntaxContext.Node;

            var symbolInfo = syntaxContext.SemanticModel.GetSymbolInfo(invocation, syntaxContext.CancellationToken);
            if (symbolInfo.Symbol?.Kind != SymbolKind.Method)
            {
                return;
            }

            var methodSymbol = (IMethodSymbol)symbolInfo.Symbol;
            var methodAttributes = methodSymbol.GetAttributes();
            if (!methodAttributes.ContainsAttributeType(swifterSharpContext.AttributesContext.ForceNamedArgumentsAttribute))
            {
                return;
            }

            if (invocation.ArgumentList.Arguments.All(x => x.IsNamed()))
            {
                return;
            }

            var diagnostic = Diagnostic.Create(_descriptor, invocation.GetLocation(), methodSymbol.Name,
                swifterSharpContext.AttributesContext.ForceNamedArgumentsAttribute.Name);

            syntaxContext.ReportDiagnostic(diagnostic);
        }
    }
}

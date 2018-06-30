using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Xunit;

namespace SwifterSharp.Tests
{
    public abstract class AnalyzerTest
    {
        protected Task<ImmutableArray<Diagnostic>> GetDiagnostics(DiagnosticAnalyzer analyzer, string source, params string[] additionalSources)
        {
            return GetDiagnostics(analyzer, CompilationReporting.FailOnErrors, source, additionalSources);
        }

        protected async Task<ImmutableArray<Diagnostic>> GetDiagnostics(DiagnosticAnalyzer analyzer, CompilationReporting compilationReporting, string source, params string[] additionalSources)
        {
            var (compilation, _, workspace) = await GetCompilation(compilationReporting, source, additionalSources);

            using (workspace)
            {
                return await ApplyAnalyzers(compilation, analyzer);
            }
        }

        private async Task<ImmutableArray<Diagnostic>> ApplyAnalyzers(Compilation compilation, params DiagnosticAnalyzer[] analyzers)
        {
            var compilationWithAnalyzers = compilation
                .WithOptions(((CSharpCompilationOptions)compilation.Options).WithWarningLevel(4))
                .WithAnalyzers(ImmutableArray.Create(analyzers));

            var allDiagnostics = await compilationWithAnalyzers.GetAllDiagnosticsAsync();

            Assert.DoesNotContain(allDiagnostics, d => d.Id == "AD0001");

            return await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync();
        }

        private async Task<(Compilation, Document, Workspace)> GetCompilation(CompilationReporting compilationReporting, string source, params string[] additionalSources)
        {
            const string fileNamePrefix = "Source";
            const string projectName = "Project";

            var projectId = ProjectId.CreateNewId(projectName);

            var workspace = new AdhocWorkspace();
            var solution = workspace
                .CurrentSolution
                .AddProject(projectId, projectName, projectName, LanguageNames.CSharp)
                .AddMetadataReferences(projectId, BuildReferences());

            var count = 0;
            var firstDocument = default(Document);

            foreach (var text in new[] { source }.Concat(additionalSources))
            {
                var newFileName = $"{fileNamePrefix}{count++}.cs";
                var documentId = DocumentId.CreateNewId(projectId, newFileName);
                solution = solution.AddDocument(documentId, newFileName, SourceText.From(text));
                if (firstDocument == default(Document))
                {
                    firstDocument = solution.GetDocument(documentId);
                }
            }

            var compileWarningLevel = Math.Max(0, (int)compilationReporting);
            var project = solution.GetProject(projectId);
            var compilationOptions = ((CSharpCompilationOptions)project.CompilationOptions)
                .WithOutputKind(OutputKind.DynamicallyLinkedLibrary)
                .WithWarningLevel(compileWarningLevel);
            project = project.WithCompilationOptions(compilationOptions);

            var compilation = await project.GetCompilationAsync();
            if (compilationReporting != CompilationReporting.IgnoreErrors)
            {
                var compilationDiagnostics = compilation.GetDiagnostics();
                if (compilationDiagnostics.Length > 0)
                {
                    var messages = compilationDiagnostics
                        .Select(d => (diag: d, line: d.Location.GetLineSpan().StartLinePosition))
                        .Select(t => $"source.cs({t.line.Line},{t.line.Character}): {t.diag.Severity.ToString().ToLowerInvariant()} {t.diag.Id}: {t.diag.GetMessage()}");
                    throw new InvalidOperationException($"Compilation has issues:{Environment.NewLine}{string.Join(Environment.NewLine, messages)}");
                }
            }

            return (compilation, firstDocument, workspace);
        }

        protected virtual List<MetadataReference> BuildReferences()
        {
            var corlibReference = MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location);
            var systemCoreReference = MetadataReference.CreateFromFile(typeof(Enumerable).GetTypeInfo().Assembly.Location);
            var systemTextReference = MetadataReference.CreateFromFile(typeof(System.Text.RegularExpressions.Regex).GetTypeInfo().Assembly.Location);
            var systemRuntimeReference = MetadataReference.CreateFromFile(typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly.Location);
            var swifterSharpReference = MetadataReference.CreateFromFile(typeof(ForceNamedArgumentsAttribute).GetTypeInfo().Assembly.Location);

            var referencedAssemblies = typeof(FactAttribute).Assembly.GetReferencedAssemblies();
            var systemRuntimeReference2 = GetAssemblyReference(referencedAssemblies, "System.Runtime");

            return new List<MetadataReference>
            {
                corlibReference,
                systemCoreReference,
                systemTextReference,
                systemRuntimeReference,
                systemRuntimeReference2,
                swifterSharpReference,
            };
        }

        private static MetadataReference GetAssemblyReference(IEnumerable<AssemblyName> assemblies, string name)
        {
            return MetadataReference.CreateFromFile(Assembly.Load(assemblies.First(n => n.Name == name)).Location);
        }
    }
}

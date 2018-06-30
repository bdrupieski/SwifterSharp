using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using SwifterSharp.Analyzers;
using SwifterSharp.Analyzers.Analysis;
using Xunit;

namespace SwifterSharp.Tests
{
    public class ForceNamedArgumentsAnalyzerTests : AnalyzerTest
    {
        [Fact]
        public async Task When_NoNamedArguments_Then_FindsError()
        {
            var analyzer = new ForceNamedArgumentsAnalyzer();

            var diagnostics = await GetDiagnostics(analyzer, @"
class TestClass 
{
    [SwifterSharp.ForceNamedArguments] 
    public void TestMethod(int a, int b, int c) { }

    public void SomeOtherMethod()
    {
        TestMethod(1, 2, 3);
    }
}
");
            Assert.Single(diagnostics);
            Assert.Collection(diagnostics, d =>
            {
                Assert.Equal("Name each of the arguments when calling TestMethod or remove ForceNamedArgumentsAttribute from its declaration.", d.GetMessage());
                Assert.Equal(Descriptors.SwifterSharp1000ArgumentsMustBeNamed.Id, d.Descriptor.Id);
                Assert.Equal(DiagnosticSeverity.Error, d.Severity);
            });
        }

        [Fact]
        public async Task When_AllArgumentsNamed_Then_NoError()
        {
            var analyzer = new ForceNamedArgumentsAnalyzer();

            var diagnostics = await GetDiagnostics(analyzer, @"
class TestClass 
{
    [SwifterSharp.ForceNamedArguments] 
    public void TestMethod(int a, int b, int c) { }

    public void SomeOtherMethod()
    {
        TestMethod(a: 1, b: 2, c: 3);
    }
}
");
            Assert.Empty(diagnostics);
        }

        [Fact]
        public async Task When_SomeArgumentsNamed_Then_FindsError()
        {
            var analyzer = new ForceNamedArgumentsAnalyzer();

            var diagnostics = await GetDiagnostics(analyzer, @"
class TestClass 
{
    [SwifterSharp.ForceNamedArguments] 
    public void TestMethod(int a, int b, int c) { }

    public void SomeOtherMethod()
    {
        TestMethod(1, b: 2, c: 3);
    }
}
");
            Assert.Single(diagnostics);
            Assert.Collection(diagnostics, d =>
            {
                Assert.Equal("Name each of the arguments when calling TestMethod or remove ForceNamedArgumentsAttribute from its declaration.", d.GetMessage());
                Assert.Equal(Descriptors.SwifterSharp1000ArgumentsMustBeNamed.Id, d.Descriptor.Id);
                Assert.Equal(DiagnosticSeverity.Error, d.Severity);
            });
        }
    }
}

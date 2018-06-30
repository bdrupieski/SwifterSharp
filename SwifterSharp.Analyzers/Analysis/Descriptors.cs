using System.Collections.Concurrent;
using Microsoft.CodeAnalysis;

namespace SwifterSharp.Analyzers.Analysis
{
    internal enum Category
    {
        Usage,           // 1xxx
        Assertions,      // 2xxx
        Extensibility,   // 3xxx
    }

    internal static class Descriptors
    {
        static readonly ConcurrentDictionary<Category, string> _categoryMapping = new ConcurrentDictionary<Category, string>();

        static DiagnosticDescriptor Rule(string id, string title, Category category, DiagnosticSeverity defaultSeverity, string messageFormat, string description = null)
        {
            const string helpLink = "https://www.github.com/bdrupieski/swiftersharp";
            var categoryText = _categoryMapping.GetOrAdd(category, c => c.ToString());
            return new DiagnosticDescriptor(id, title, messageFormat, categoryText, defaultSeverity, isEnabledByDefault: true, description, helpLink);
        }

        internal static DiagnosticDescriptor SwifterSharp1000ArgumentsMustBeNamed { get; } =
            Rule("SwifterSharp1000", "Arguments must be named", Category.Usage, DiagnosticSeverity.Error,
                "Name each of the arguments when calling the method '{0}' or remove {1} from its declaration.");
    }
}
